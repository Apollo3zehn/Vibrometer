using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Vibrometer.BaseTypes;
using Vibrometer.BaseTypes.API;

namespace Vibrometer.WebServer
{
    public class VibrometerHub : Hub
    {
        private VibrometerApi _api;

        public VibrometerHub(VibrometerApi api)
        {
            _api = api;
        }

        public override Task OnConnectedAsync()
        {
            this.Clients.Client(this.Context.ConnectionId).SendAsync("SendVibrometerState", _api.GetState());

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public Task<VibrometerState> GetVibrometerState()
        {
            return Task.Run(() =>
            {
                return _api.GetState();
            });
        }

        public Task ToggleHubEnabled()
        {
            return Task.Run(() =>
            {
                _api.FourierTransform.Enabled = !_api.FourierTransform.Enabled;
                this.Clients.All.SendAsync("SendVibrometerState", _api.GetState());
            });
        }
    }
}
