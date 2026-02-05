namespace orchestratorService.lib.Interfaces
{
    public interface IServiceInfoResolver
    {
        string GetServiceName();

        string[] GetServiceSubscriptions();
    }
}
