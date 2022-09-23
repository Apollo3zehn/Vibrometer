using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Vibrometer.WebClient.Model;

namespace Vibrometer.WebClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddSingleton<AppStateViewModel>();

            builder.Services.AddSingleton(sp => 
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();

                return new HubConnectionBuilder()
                  .WithUrl(navigationManager.ToAbsoluteUri("/vibrometerhub"))
                  .AddMessagePackProtocol()
                  .WithAutomaticReconnect()
                  .Build();
            });

            await builder.Build().RunAsync();
        }
    }
}
