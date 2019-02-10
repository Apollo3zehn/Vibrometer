using Blazor.Extensions;
using System;
using System.Threading.Tasks;
using Vibrometer.BaseTypes;
using Vibrometer.WebClient.Model;

namespace Vibrometer.WebClient
{
    public class SignalRService
    {
        #region Constructors

        public SignalRService(AppState state)
        {
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

            this.Connection.On<int[]>("SendBufferContent", bufferContent =>
            {
                state.BufferContent = bufferContent;

                return Task.CompletedTask;
            });

            this.Connection.On<VibrometerState>("SendVibrometerState", vibrometerState =>
            {
                state.VibrometerState = vibrometerState;

                return Task.CompletedTask;
            });

            Task.Run(async () =>
            {
                state.VibrometerState = await this.Connection.InvokeAsync<VibrometerState>("GetVibrometerState");
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
            return new HubConnectionBuilder()
                 .WithUrl("/vibrometerhub")
                 .AddMessagePackProtocol()
                 .Build();
        }

        #endregion
    }
}
