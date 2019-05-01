using Blazor.Extensions;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using Vibrometer.Infrastructure;

namespace Vibrometer.WebClient.Model
{
    public class SignalRService
    {
        #region Fields

        public IJSRuntime _jsRuntime;

        #endregion

        #region Constructors

        public SignalRService(AppStateViewModel state, IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _jsRuntime.InvokeAsync<object>("OnLoaded");

            this.Connection = this.BuildHubConnection();

            this.Connection.OnClose(e =>
            {
                return Task.Run(async () =>
                {
                    state.IsConnected = false;

                    while (true)
                    {
                        try
                        {
                            await this.Connection.StartAsync();
                            state.IsConnected = true;

                            break;
                        }
                        catch
                        {
                            await Task.Delay(TimeSpan.FromSeconds(3));
                        }
                    }
                });
            });

            this.Connection.On<FpgaData>("SendFpgaData", fpgaData =>
            {
                state.FpgaData = fpgaData;

                return Task.CompletedTask;
            });

            this.Connection.On<FpgaSettings>("SendFpgaSettings", fpgaSettings =>
            {
                state.FpgaSettings = fpgaSettings;

                return Task.CompletedTask;
            });

            this.Connection.On<bool>("SendBitstreamState", bitstreamState =>
            {
                state.IsBitstreamLoaded = bitstreamState;

                return Task.CompletedTask;
            });

            Task.Run(async () =>
            {
                state.FpgaSettings = await this.Connection.InvokeAsync<FpgaSettings>("GetFpgaSettings");
            });

            this.Connection.StartAsync();
        }

        #endregion

        #region Properties

        public HubConnection Connection { get; }

        #endregion

        #region Methods

        private HubConnection BuildHubConnection()
        {
            return new HubConnectionBuilder(_jsRuntime)
                 .WithUrl("/vibrometerhub")
                 .AddMessagePackProtocol()
                 .Build();
        }

        #endregion
    }
}
