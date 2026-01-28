namespace ServiceRegistry.Lib.Interfaces
{
    public interface IServiceInfoResolver
    {
        public string GetServiceName();

        public string GetServiceLocation();
    }
}
