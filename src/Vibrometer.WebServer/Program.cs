using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Mono.Unix.Native;
using System;
using System.Threading;

namespace Vibrometer.WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static void PInvokeTest()
        {
            int fd;
            IntPtr cfg;
            uint count;
            const int freq = 124998750; // Hz

            fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);
            cfg = Syscall.mmap(IntPtr.Zero, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE), MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, 0x42000000);

            Console.WriteLine($"Conf: {cfg.ToInt32()}");

            unsafe
            {
                Console.WriteLine("Clear timer!");
                *((UInt32*)(cfg + 0)) = 2;   // clear timer
                Console.WriteLine("Start timer!");
                *((UInt32*)(cfg + 0)) = 1;   // start timer

                Thread.Sleep(5000);

                Console.WriteLine("Stop timer!");
                *((UInt32*)(cfg + 0)) = 0;   // stop timer

                Console.WriteLine("Stop timer!");
                count = *((UInt32*)(cfg + 8));
            }

            Console.WriteLine($"Clock count: { count }, calculated time: { (double)count / freq } s\n");

            Syscall.munmap(cfg, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE));
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
