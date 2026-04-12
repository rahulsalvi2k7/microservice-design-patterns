using OrderService.Models;
using OrderService.Services;

public class OutboxProcessingService : BackgroundService
{
    private readonly Outbox orderOutbox;
    private readonly SemaphoreSlimProvider semaphoreSlimProvider;
    private readonly HttpClient notificationHttpClient;

    private static readonly int PollingIntervalInMilliseconds = 5_000;
    private static readonly int NotificationIntervalInMilliseconds = 1_000;

    public OutboxProcessingService(
        Outbox orderOutbox,
        IHttpClientFactory httpClientFactory,
        SemaphoreSlimProvider semaphoreSlimProvider)
    {
        this.orderOutbox = orderOutbox;
        this.semaphoreSlimProvider = semaphoreSlimProvider;
        this.notificationHttpClient = httpClientFactory.CreateClient();

        notificationHttpClient.BaseAddress = new Uri("http://localhost:5153");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await semaphoreSlimProvider.WaitAsync(stoppingToken);

            var waitingMessage = orderOutbox.Messages.FirstOrDefault(m => m.MessageStatus == MessageStatus.Waiting);

            if (waitingMessage is null)
            {
                // No messages so release the lock and wait for 5sec before checking again                    
                semaphoreSlimProvider.Release();
                Console.WriteLine($"no messages waiting...wait for 5s");

                // Check every 5 sec
                await Task.Delay(PollingIntervalInMilliseconds, stoppingToken);

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
            finally
            {
                semaphoreSlimProvider.Release();
            }

            await Task.Delay(NotificationIntervalInMilliseconds, stoppingToken);
        }
    }

    private async Task SendNotificaiton(OutboxMessage waitingMessage, CancellationToken cancellationToken)
    {
        var response = await notificationHttpClient.GetAsync($"/notify/{waitingMessage.Id}", cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
