/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpService
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MndpTray.Protocol;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    public class MndpBackgroundService : BackgroundService
    {
        private readonly ILogger<MndpBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        public MndpBackgroundService(ILogger<MndpBackgroundService> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger?.LogInformation("Starting...");

            if (this._configuration.GetValue<bool>("isLogging", false))
            {
                Log.SetInfoAction((format, param) =>
                    {
                        this._logger?.LogInformation(format, param);
                    });
            }

            MndpSender.Instance.Start(MndpHostInfo.Instance);
            MndpListener.Instance.Start();

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger?.LogInformation("Stopping...");

            MndpSender.Instance.Stop();
            MndpListener.Instance.Stop();

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
