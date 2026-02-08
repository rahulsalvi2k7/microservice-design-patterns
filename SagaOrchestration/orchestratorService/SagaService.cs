namespace orchestratorService
{
    public class SagaService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ChannelProvider channelProvider;

        public SagaService(IServiceScopeFactory serviceScopeFactory, ChannelProvider channelProvider)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.channelProvider = channelProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in channelProvider.ChannelReader.ReadAllAsync(stoppingToken))
            {
                using var serviceScope = serviceScopeFactory.CreateScope();

                var saga = serviceScope.ServiceProvider.GetRequiredService<OrderSaga>();

                await saga.ExecuteAsync(item);
            }
        }
    }
}
