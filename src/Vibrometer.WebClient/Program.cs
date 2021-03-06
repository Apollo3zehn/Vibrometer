﻿using Microsoft.AspNetCore.Blazor.Hosting;

namespace Vibrometer.WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Program.CreateHostBuilder(args)
                   .Build()
                   .Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
