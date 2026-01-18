using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LoggerSidecar.Lib
{
    public class LogAggregationSideCarService : BackgroundService
    {
        private readonly HttpClient httpClient;
        private readonly LogMessageStore logMessageStore;

        public LogAggregationSideCarService(IHttpClientFactory httpClientFactory, LogMessageStore logMessageStore)
        {
            httpClient = httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri("http://localhost:5006");
            this.logMessageStore = logMessageStore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.UtcNow:s} Checking if there are any logs...");

                while (logMessageStore.LogMessages.TryDequeue(out var message))
                {
                    await httpClient.PostAsync("/log", new StringContent(JsonConvert.SerializeObject(message)), stoppingToken);
                }

                await Task.Delay(10_000);
            }
        }
    }
}
