namespace orchestratorService.lib
{
    public interface IOrchestratorClient 
    {
        Task Subscribe(string eventName, string serviceName);

        Task Unsubscribe(string eventName, string serviceName);

        Task Publish(string eventName);
    }
}
