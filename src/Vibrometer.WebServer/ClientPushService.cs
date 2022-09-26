using Microsoft.AspNetCore.SignalR;
using System;
using System.Buffers;
using System.Collections.Generic;
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
        private List<(TcpClient, NetworkStream)> _clients = new();

        #endregion

        #region "Constructors"

        public ClientPushService(IHubContext<VibrometerHub> hubContext, VibrometerApi api)
        {
            _hubContext = hubContext;
            _api = api;

            _updateBufferContentTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromMilliseconds(500).TotalMilliseconds };
            _updateBufferContentTimer.Elapsed += this.OnUpdateBufferContent;


            _ = AcceptTcpClientsAsync();
            _ = PushFastAsync();
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
                    var buffer = ArrayPool<int>.Shared.Rent(length);

                    _api.FillBuffer(buffer);

                    var fpgaData = new FpgaData(lowerTreshold, upperThreshold, buffer);
                    await _hubContext.Clients.All.SendAsync("SendFpgaData", fpgaData);

                    ArrayPool<int>.Shared.Return(buffer);
                });
            }
        }

        private async Task AcceptTcpClientsAsync()
        {
            var listener = new TcpListener(IPAddress.Any, 5555);
            listener.Start();
            Console.WriteLine("Listening on port 5555 ...");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                var networkStream = client.GetStream();

                Console.WriteLine($"Client {client.Client.RemoteEndPoint.ToString()} connected on port 5555.");

                lock (_clients) 
                {
                    _clients.Add((client, networkStream));
                }
            }
        }

        private async Task PushFastAsync()
        {
            var clientsToRemove = new List<(TcpClient, NetworkStream)>();

            while (true)
            {
                if (_api.RamWriter.Enabled)
                {
                    var length = (int)Math.Pow(2, _api.RamWriter.LogLength);
                    using var memoryOwner = MemoryPool<int>.Shared.Rent(length);
                    var memory = memoryOwner.Memory;

                    while (true)
                    {
                        lock (_lock)
                        {
                            // _api.FillBuffer(memory.Span);
                            var random = new Random();

                            for (int i = 0; i < memory.Span.Length; i++)
                            {
                                memory.Span[i] = random.Next(0, 10) + 5;
                            }

                            memory.Span[400] = 15;

                            lock (_clients)
                            {
                                foreach (var client in _clients)
                                {
                                    try
                                    {
                                        client.Item2.Write(MemoryMarshal.AsBytes(memory.Span));
                                    }
                                    catch
                                    {
                                        Console.WriteLine($"Client {client.Item1.Client.RemoteEndPoint.ToString()} disconnected.");
                                        client.Item1.Close();
                                        clientsToRemove.Add(client);
                                    }
                                }

                                foreach (var client in clientsToRemove)
                                {
                                    _clients.Remove(client);
                                }

                                clientsToRemove.Clear();
                            }
                        }
                    }
                }

                else
                {
                    Console.WriteLine("Waiting for RAM writer to become enabled.");
                }

                await Task.Delay(1000);
            }
        }

        #endregion
    }
}
