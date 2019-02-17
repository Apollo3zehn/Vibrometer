using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Vibrometer.API;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.WebServer
{
    public class VibrometerHub : Hub
    {
        private AppState _state;
        private VibrometerApi _api;

        // clientPushService is only requested to create a singleton instance
        public VibrometerHub(AppState state, VibrometerApi api, ClientPushService clientPushService)
        {
            _state = state;
            _api = api;
        }

        public override Task OnConnectedAsync()
        {
            this.Clients.Client(this.Context.ConnectionId).SendAsync("SendFpgaSettings", _api.GetState());
            this.Clients.Client(this.Context.ConnectionId).SendAsync("SendBitstreamState", _state.IsBitstreamLoaded);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public Task<FpgaSettings> GetFpgaSettings()
        {
            return Task.Run(() =>
            {
                return _api.GetState();
            });
        }

        public Task LoadBitstream(string bitstreamBase64)
        {
            return Task.Run(() =>
            {
                byte[] bitstream;

                bitstream = Convert.FromBase64String(bitstreamBase64);

                _api.LoadBitstream(bitstream);
                this.OnFpgaSettingsChanged();
            });
        }

        public Task UpdateBitstreamState(bool state)
        {
            return Task.Run(() =>
            {
                _state.IsBitstreamLoaded = state;
                this.Clients.All.SendAsync("SendBitstreamState", _state.IsBitstreamLoaded);
            });
        }

        public Task UpdateSetting(ApiParameter parameter, uint value)
        {
            return Task.Run(() =>
            {
                switch (parameter)
                {
                    case ApiParameter.AS_Source:
                        _api.SetStateSafe(() => _api.AxisSwitch.Source = (ApiSource)value);
                        break;
                    case ApiParameter.SG_ShiftCarrier:
                        _api.SignalGenerator.ShiftCarrier = value;
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
                        _api.SetStateSafe(() => _api.RamWriter.LogLength = value);
                        break;
                    case ApiParameter.RW_LogThrottle:
                        _api.RamWriter.LogThrottle = value;
                        break;
                    default:
                        throw new ArgumentException();
                }

                this.OnFpgaSettingsChanged();
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
                    case ApiParameter.FI_Enabled:
                        _api.Filter.Enabled = value;
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

                this.OnFpgaSettingsChanged();
            });
        }

        public Task ApplyConfiguration(FpgaSettings fpgaSettings)
        {
            return Task.Run(() =>
            {
                _api.SetState(fpgaSettings);
                this.OnFpgaSettingsChanged();
            });
        }

        private void OnFpgaSettingsChanged()
        {
            this.Clients.All.SendAsync("SendFpgaSettings", _api.GetState());
        }
    }
}
