using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MndpService.Core
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseSystemd()            
            .ConfigureServices(services =>
            {
                services.AddLogging();
                services.AddHostedService<MndpBackgroundService>();
            });

            return hostBuilder;
        }

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            CreateHostBuilder(args).Build().Run();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Console.WriteLine("UnhandledException");
                Console.WriteLine(e.ToString());
            }
            catch { }
        }
    }
}
