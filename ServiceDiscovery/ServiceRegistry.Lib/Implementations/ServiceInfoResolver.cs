using Microsoft.Extensions.Configuration;
using ServiceRegistry.Lib.Interfaces;

namespace ServiceRegistry.Lib.Implementations
{
    public class ServiceInfoResolver : IServiceInfoResolver
    {
        private readonly IConfiguration _configuration;

        public ServiceInfoResolver(IConfiguration configuration) => _configuration = configuration;

        public string GetServiceLocation()
        {
            var serviceLocation = _configuration["ServiceDiscovery:serviceLocation"] ?? throw new ApplicationException("missing config");

            return serviceLocation;
        }

        public string GetServiceName()
        {
            var serviceName = _configuration["ServiceDiscovery:serviceName"] ?? throw new ApplicationException("missing config");

            return serviceName;
        }
    }
}
