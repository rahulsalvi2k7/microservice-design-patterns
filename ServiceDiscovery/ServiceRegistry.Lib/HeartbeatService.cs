using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ServiceRegistry.Lib
{
    public class HeartbeatService : BackgroundService
    {
        private readonly IServiceClient _serviceClient;
        private readonly IConfiguration _configuration;

        public HeartbeatService(IServiceClient serviceClient, IConfiguration configuration)
        {
            this._serviceClient = serviceClient;
            this._configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var heartbeatRate = _configuration["ServiceDiscovery:heartBeatRate"] ?? throw new ApplicationException("missing config");

            int.TryParse(heartbeatRate, out var heartBeat);

            while (!stoppingToken.IsCancellationRequested)
            {
                await _serviceClient.SendHeartbeat();

                await Task.Delay(heartBeat, stoppingToken);
            }
        }
    }
}
