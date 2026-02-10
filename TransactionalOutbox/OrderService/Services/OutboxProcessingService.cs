// Configure the HTTP request pipeline.


using OrderService.Models;

public class OutboxProcessingService : BackgroundService
{
    private readonly Outbox orderOutbox;
    private readonly HttpClient notificationHttpClient;

    public OutboxProcessingService(Outbox orderOutbox, IHttpClientFactory httpClientFactory)
    {
        this.orderOutbox = orderOutbox;
        this.notificationHttpClient = httpClientFactory.CreateClient();

        notificationHttpClient.BaseAddress = new Uri("http://localhost:5153");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var waitingMessage = orderOutbox.Messages.FirstOrDefault(m => m.MessageStatus == MessageStatus.Waiting);

            if (waitingMessage is null)
            {
                Console.WriteLine($"no messages waiting...wait for 10s");

                // Check every 10 sec
                await Task.Delay(10_000, stoppingToken);

                continue;
            }

            try
            {
                await SendNotificaiton(waitingMessage, stoppingToken);

                waitingMessage.MessageStatus = MessageStatus.Sent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}");

                waitingMessage.MessageStatus = MessageStatus.Failed;
            }
        }
    }

    private async Task SendNotificaiton(OutboxMessage waitingMessage, CancellationToken cancellationToken)
    {
        var response = await notificationHttpClient.GetAsync($"/notify/{waitingMessage.Id}", cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
