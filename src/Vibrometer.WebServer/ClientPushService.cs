using Microsoft.AspNetCore.SignalR;
using System;
using System.Timers;
using Vibrometer.BaseTypes.API;

namespace Vibrometer.WebServer
{
    public class ClientPushService
    {
        #region "Fields"

        private Timer _updateBufferContentTimer;
        private VibrometerApi _api;
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
            if (_api.RamWriter.Enabled)
            {
                _hubContext.Clients.All.SendAsync("SendBufferContent", _api.GetBuffer());
            }
        }

        #endregion
    }
}
