namespace ServiceRegistry.Lib.Interfaces
{
    public interface IServiceClient
    {
        Task Register(string name, string location);

        Task Unregister(string name);

        Task<string> GetLocation(string name);

        Task SendHeartbeat();
    }
}
