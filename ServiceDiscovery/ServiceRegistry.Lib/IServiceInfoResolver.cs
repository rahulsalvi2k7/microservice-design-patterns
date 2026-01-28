namespace ServiceRegistry.Lib
{
    public interface IServiceInfoResolver
    {
        public string GetServiceName();

        public string GetServiceLocation();
    }
}
