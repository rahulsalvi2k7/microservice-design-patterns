namespace orchestratorService.lib.Interfaces
{
    public interface IOrchestratorClient
    {
        Task Subscribe(string eventName, string serviceName);

        Task Unsubscribe(string eventName, string serviceName);

        Task Publish(string eventName);
    }
}
