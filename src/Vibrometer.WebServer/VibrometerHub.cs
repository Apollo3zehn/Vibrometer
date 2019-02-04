using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Vibrometer.WebServer
{
    public class VibrometerHub : Hub
    {
        public VibrometerHub()
        {
            //
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
