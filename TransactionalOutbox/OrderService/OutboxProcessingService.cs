// Configure the HTTP request pipeline.


public class OutboxProcessingService : BackgroundService
{
    private readonly OrderOutbox orderOutbox;
    private readonly HttpClient httpClient;

    public OutboxProcessingService(OrderOutbox orderOutbox, IHttpClientFactory httpClientFactory)
    {
        this.orderOutbox = orderOutbox;
        this.httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5153");
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

    private async Task SendNotificaiton(OrderOutboxMessage waitingMessage, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"/notify/{waitingMessage.Id}", cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}