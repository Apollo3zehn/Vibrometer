using Blazor.Extensions;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Vibrometer.WebClient
{
    public class Program
    {
        private static HubConnection _connection;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            JSRuntime.Current.InvokeAsync<object>("OnLoaded");

            Program._connection = new HubConnectionBuilder()
            .WithUrl("/vibrometerhub")
            .AddMessagePackProtocol()
            .Build();

            Program._connection.On<string>("Send", (message) => Task.Run(() => Console.WriteLine(message)));
            Program._connection.StartAsync();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
