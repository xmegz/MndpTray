using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MndpTray.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MndpService.Core
{
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
                Log.SetInfoAction(this.InfoAction);
            }

            MndpSender.Instance.Start(MndpHostInfo.Instance);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger?.LogInformation("Stopping...");

            MndpSender.Instance.Stop();
            
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        protected void InfoAction(string format, params object[] args)
        {
            string str = string.Format(format, args);
            this._logger?.LogInformation(str);
        }
    }
}
