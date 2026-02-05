using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using orchestratorService.lib.Interfaces;

namespace orchestratorService.lib.Extensions
{
    public static class HostApplicationLifetimeExtensions
    {
        public static IHostApplicationLifetime RegisterLifetimeEvents(this IApplicationBuilder app)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            lifetime
                .OnStart(app)
                .OnStop(app);

            return lifetime;
        }

        private static IHostApplicationLifetime OnStart(this IHostApplicationLifetime lifetime, IApplicationBuilder app)
        {
            var orchestratorClient = app.ApplicationServices.GetRequiredService<IOrchestratorClient>();
            var serviceInfoResolver = app.ApplicationServices.GetRequiredService<IServiceInfoResolver>();
            var serviceName = serviceInfoResolver.GetServiceName();
            var subscriptions = serviceInfoResolver.GetServiceSubscriptions();

            lifetime.ApplicationStarted.Register(async () =>
            {
                foreach (var subscription in subscriptions)
                {
                    await orchestratorClient.Subscribe(subscription, serviceName);
                }
            });

            return lifetime;
        }

        private static IHostApplicationLifetime OnStop(this IHostApplicationLifetime lifetime, IApplicationBuilder app)
        {
            var orchestratorClient = app.ApplicationServices.GetRequiredService<IOrchestratorClient>();
            var serviceInfoResolver = app.ApplicationServices.GetRequiredService<IServiceInfoResolver>();
            var serviceName = serviceInfoResolver.GetServiceName();
            var subscriptions = serviceInfoResolver.GetServiceSubscriptions();

            lifetime.ApplicationStopping.Register(async () =>
            {
                foreach (var subscription in subscriptions)
                {
                    await orchestratorClient.Unsubscribe(subscription, serviceName);
                }
            });

            return lifetime;
        }
    }
}
