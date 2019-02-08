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

        // clientPushService is only requested to create an instance
        public VibrometerHub(VibrometerApi api, ClientPushService clientPushService)
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

        public Task UpdateSetting(ApiParameter parameter, uint value)
        {
            return Task.Run(() =>
            {
                switch (parameter)
                {
                    case ApiParameter.AS_Source:
                        _api.AxisSwitch.Source = (ApiSource)value;
                        break;
                    case ApiParameter.SG_PhaseSignal:
                        _api.SignalGenerator.PhaseSignal = value;
                        break;
                    case ApiParameter.SG_PhaseCarrier:
                        _api.SignalGenerator.PhaseCarrier = value;
                        break;
                    case ApiParameter.PT_LogScale:
                        _api.PositionTracker.LogScale = value;
                        break;
                    case ApiParameter.PT_LogCountExtremum:
                        _api.PositionTracker.LogCountExtremum = value;
                        break;
                    case ApiParameter.PT_ShiftExtremum:
                        _api.PositionTracker.ShiftExtremum = value;
                        break;
                    case ApiParameter.FI_LogThrottle:
                        _api.Filter.LogThrottle = value;
                        break;
                    case ApiParameter.FT_LogCountAverages:
                        _api.FourierTransform.LogCountAverages = value;
                        break;
                    case ApiParameter.FT_LogThrottle:
                        _api.FourierTransform.LogThrottle = value;
                        break;
                    case ApiParameter.RW_LogLength:
                        _api.RamWriter.LogLength = value;
                        break;
                    case ApiParameter.RW_LogThrottle:
                        _api.RamWriter.LogThrottle = value;
                        break;
                    default:
                        throw new ArgumentException();
                }

                this.OnVibrometerStateChanged();
            });
        }

        public Task UpdateSettingBool(ApiParameter parameter, bool value)
        {
            return Task.Run(() =>
            {
                switch (parameter)
                {
                    case ApiParameter.SG_FmEnabled:
                        _api.SignalGenerator.FmEnabled = value;
                        break;
                    case ApiParameter.DA_SwitchEnabled:
                        _api.DataAcquisition.SwitchEnabled = value;
                        break;
                    case ApiParameter.FT_Enabled:
                        _api.FourierTransform.Enabled = value;
                        break;
                    case ApiParameter.RW_Enabled:
                        _api.RamWriter.Enabled = value;
                        break;
                    case ApiParameter.RW_RequestEnabled:
                        _api.RamWriter.RequestEnabled = value;
                        break;
                    default:
                        throw new ArgumentException();
                }

                this.OnVibrometerStateChanged();
            });
        }

        public Task ActivateProfile(string profileName)
        {
            return Task.Run(() =>
            {
                _api.SetDefaults();
                this.OnVibrometerStateChanged();
            });
        }

        private void OnVibrometerStateChanged()
        {
            this.Clients.All.SendAsync("SendVibrometerState", _api.GetState());
        }
    }
}
