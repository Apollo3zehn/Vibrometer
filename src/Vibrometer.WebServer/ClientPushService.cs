﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Timers;
using Vibrometer.Shared.API;

namespace Vibrometer.WebServer
{
    public class ClientPushService
    {
        #region "Fields"

        private int _nextSubscriptionId;
        private Timer _updateBufferContentTimer;
        private VibrometerApi _api;
        private IHubContext<VibrometerHub> _hubContext;

        #endregion

        #region "Constructors"

        public ClientPushService(IHubContext<VibrometerHub> hubContext, VibrometerApi api)
        {
            _hubContext = hubContext;
            _api = api;

            _updateBufferContentTimer = new Timer() { AutoReset = true, Enabled = true, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };
            _updateBufferContentTimer.Elapsed += OnUpdateBufferContent;
        }

        #endregion

        #region "Methods"

        private void OnUpdateBufferContent(object sender, ElapsedEventArgs e)
        {
            _hubContext.Clients.All.SendAsync("SendBufferContent", _api.GetState());
        }

        #endregion
    }
}