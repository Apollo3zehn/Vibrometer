using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Vibrometer.WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseConfiguration(new ConfigurationBuilder()
                   .AddCommandLine(args)
                   .Build())
                   .UseUrls("http://0.0.0.0:8080")
                   .UseStartup<Startup>()
                   .Build();
    }
}
