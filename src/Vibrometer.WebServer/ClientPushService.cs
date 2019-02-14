using Microsoft.AspNetCore.SignalR;
using System;
using System.Timers;
using Vibrometer.API;
using Vibrometer.Infrastructure;

namespace Vibrometer.WebServer
{
    public class ClientPushService
    {
        #region "Fields"

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
                _hubContext.Clients.All.SendAsync("SendFpgaData", new FpgaData(lowerTreshold, upperThreshold, _api.GetBuffer()));
            }
        }

        #endregion
    }
}
