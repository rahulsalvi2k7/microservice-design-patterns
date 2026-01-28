using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiceRegistry.Lib
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
            var serviceClient = app.ApplicationServices.GetRequiredService<IServiceClient>();
            var serviceInfoResolver = app.ApplicationServices.GetRequiredService<IServiceInfoResolver>();
            var serviceName = serviceInfoResolver.GetServiceName();
            var serviceLocation = serviceInfoResolver.GetServiceLocation();

            lifetime.ApplicationStarted.Register(async () =>
            {
                await serviceClient.Register(serviceName, serviceLocation);
            });

            return lifetime;
        }

        private static IHostApplicationLifetime OnStop(this IHostApplicationLifetime lifetime, IApplicationBuilder app)
        {
            var serviceClient = app.ApplicationServices.GetRequiredService<IServiceClient>();
            var serviceInfoResolver = app.ApplicationServices.GetRequiredService<IServiceInfoResolver>();
            var serviceName = serviceInfoResolver.GetServiceName();

            lifetime.ApplicationStopped.Register(async () =>
            {
                await serviceClient.Unregister(serviceName);
            });

            return lifetime;
        }
    }
}
