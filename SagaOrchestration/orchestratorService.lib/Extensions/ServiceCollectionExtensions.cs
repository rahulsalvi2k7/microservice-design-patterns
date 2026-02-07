using Microsoft.Extensions.DependencyInjection;
using orchestratorService.lib.Implementation;
using orchestratorService.lib.Interfaces;

namespace orchestratorService.lib.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterOrchestrationServices(this IServiceCollection services)
        {
            services
                .AddHttpClient()
                .AddSingleton<IOrchestratorClient, OrchestratorClient>()
                .AddSingleton<IServiceInfoResolver, ServiceInfoResolver>();

            return services;
        }
    }
}
