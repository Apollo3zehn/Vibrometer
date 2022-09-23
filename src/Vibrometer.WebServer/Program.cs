using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Net.Mime;
using Vibrometer.API;

namespace Vibrometer.WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            
            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                configurationBuilder
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            }

            var configuration = configurationBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // configuration
            builder.Configuration.AddConfiguration(configuration);

            // add services
            builder.Services.AddMvc();

            builder.Services.AddSignalR(options => options.KeepAliveInterval = TimeSpan.FromSeconds(5))
                    .AddMessagePackProtocol();

            builder.Services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    "application/wasm"
                });
            });

            builder.Services.AddSingleton<AppState>();
            builder.Services.AddSingleton<VibrometerApi>();
            builder.Services.AddSingleton<ClientPushService>();

            // build
            var app = builder.Build();

            // configure pipeline
            app.UseResponseCompression();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            // Serilog Request Logging (https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-reducing-log-verbosity/)
            // LogContext properties are not included by default in request logging, workaround: https://nblumhardt.com/2019/10/serilog-mvc-logging/
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<VibrometerHub>("/vibrometerhub", options =>
                {
                    options.ApplicationMaxBufferSize = 3 * 1024 * 1024; // to send the bitstream (see this issue: https://github.com/aspnet/SignalR/issues/2266#issuecomment-389143453)
                });
                endpoints.MapFallbackToFile("index.html");
            });

            // Run
            var baseUrl = "http://0.0.0.0:5000";
            app.Run(baseUrl);
        }
    }
}
