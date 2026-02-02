namespace SwitchService
{
    using Microsoft.AspNetCore.Mvc;
    using SwitchService.Lib;

    public static class SwitchServiceRoutes
    {
        public static IEndpointRouteBuilder RegisterRoutes(this IEndpointRouteBuilder app)
        {
            app.AddStatusRoute()
                .AddOpenRoute()
                .AddCloseRoute()
                .AddHalfOpenRoute();

            return app;
        }

        private static IEndpointRouteBuilder AddStatusRoute(this IEndpointRouteBuilder app) 
        {
            app.MapGet("/status", ([FromServices] ServiceStatus serviceStatus) =>
            {
                return serviceStatus;
            });

            return app;
        }

        private static IEndpointRouteBuilder AddOpenRoute(this IEndpointRouteBuilder app) 
        {
            app.MapGet("/open", ([FromServices] ServiceStatus serviceStatus) =>
            {
                serviceStatus.Code = ServiceStatusCode.Open;

                return serviceStatus;
            });

            return app;
        }

        private static IEndpointRouteBuilder AddCloseRoute(this IEndpointRouteBuilder app) 
        {
            app.MapGet("/close", ([FromServices] ServiceStatus serviceStatus) =>
            {
                serviceStatus.Code = ServiceStatusCode.Closed;

                return serviceStatus;
            });

            return app;
        }

        private static IEndpointRouteBuilder AddHalfOpenRoute(this IEndpointRouteBuilder app) 
        {
            app.MapGet("/halfopen", ([FromServices] ServiceStatus serviceStatus) =>
            {
                serviceStatus.Code = ServiceStatusCode.HalfOpen;

                return serviceStatus;
            });

            return app;
        }
    }
}
