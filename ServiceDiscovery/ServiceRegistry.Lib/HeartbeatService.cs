using Microsoft.Extensions.Hosting;

namespace ServiceRegistry.Lib
{
    public class HeartbeatService : BackgroundService
    {
        private readonly IServiceClient serviceClient;

        public HeartbeatService(IServiceClient serviceClient)
        {
            this.serviceClient = serviceClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await serviceClient.SendHeartbeat("orderService");

                await Task.Delay(30000);
            }
        }
    }

}
