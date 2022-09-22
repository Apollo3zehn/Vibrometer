using Microsoft.AspNetCore.SignalR;
using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Vibrometer.API;
using Vibrometer.Infrastructure;

namespace Vibrometer.WebServer
{
    public class ClientPushService
    {
        #region "Fields"

        private object _lock = new Object();
        private VibrometerApi _api;
        private Timer _updateBufferContentTimer;
        private IHubContext<VibrometerHub> _hubContext;

        #endregion

        #region "Constructors"

        public ClientPushService(IHubContext<VibrometerHub> hubContext, VibrometerApi api)
        {
            _hubContext = hubContext;
            _api = api;

            _updateBufferContentTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromMilliseconds(500).TotalMilliseconds };
            _updateBufferContentTimer.Elapsed += this.OnUpdateBufferContent;
        }

        #endregion

        #region "Methods"

        private void OnUpdateBufferContent(object sender, ElapsedEventArgs e)
        {
            int lowerTreshold;
            int upperThreshold;

            (lowerTreshold, upperThreshold) = _api.PositionTracker.Threshold;

            if (_api.RamWriter.Enabled)
            {
                Task.Run(async () =>
                {
                    var length = (int)Math.Pow(2, _api.RamWriter.LogLength);
                    using var memoryOwner = MemoryPool<int>.Shared.Rent(length);
                    var memory = memoryOwner.Memory;

                    lock (_lock)
                    {
                        _api.FillBuffer(memory.Span);
                    }

                    var fpgaData = new FpgaData(lowerTreshold, upperThreshold, memory);
                    await _hubContext.Clients.All.SendAsync("SendFpgaData", fpgaData);
                });
            }
        }

        private async Task PushFast()
        {
            while (true)
            {
                var listener = new TcpListener(IPAddress.Any, 5555);
                using var client = listener.AcceptTcpClient();
                using var networkStream = client.GetStream();

                if (_api.RamWriter.Enabled)
                {
                    var length = (int)Math.Pow(2, _api.RamWriter.LogLength);
                    using var memoryOwner = MemoryPool<int>.Shared.Rent(length);
                    var memory = memoryOwner.Memory;

                    try
                    {
                        while (true)
                        {
                            lock (_lock)
                            {
                                _api.FillBuffer(memory.Span);
                                networkStream.Write(MemoryMarshal.AsBytes(memory.Span));
                                Task.Delay(50);
                            }
                        }
                    }
                    finally
                    {
                        listener.Stop();
                        await Task.Delay(1000);
                    }
                }
            }
        }

        #endregion
    }
}
