using Microsoft.Extensions.Configuration;
using orchestratorService.lib.Interfaces;

namespace orchestratorService.lib.Implementation
{
    public class ServiceInfoResolver : IServiceInfoResolver
    {
        private readonly IConfiguration _configuration;

        public ServiceInfoResolver(IConfiguration configuration) => _configuration = configuration;

        public string GetServiceName()
        {
            var serviceName = _configuration["orchestration:serviceName"] ?? throw new ApplicationException("missing config");

            return serviceName;
        }

        public string[] GetServiceSubscriptions()
        {
            var subscriptions = _configuration.GetSection("orchestration:subscriptions").Get<string[]>() ?? [];

            return subscriptions;
        }
    }
}
