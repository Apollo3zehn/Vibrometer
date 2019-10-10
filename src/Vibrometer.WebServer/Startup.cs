using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Mime;
using Vibrometer.API;

namespace Vibrometer.WebServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSignalR(options => options.KeepAliveInterval = TimeSpan.FromSeconds(5))
                    .AddMessagePackProtocol();

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    "application/wasm"
                });
            });

            services.AddSingleton<AppState>();
            services.AddSingleton<VibrometerApi>();
            services.AddSingleton<ClientPushService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<WebClient.Startup>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<VibrometerHub>("/vibrometerhub", options =>
                {
                    options.ApplicationMaxBufferSize = 3 * 1024 * 1024; // to send the bitstream (see this issue: https://github.com/aspnet/SignalR/issues/2266#issuecomment-389143453)
                });
                endpoints.MapFallbackToClientSideBlazor<WebClient.Startup>("index.html");
            });
        }
    }
}
