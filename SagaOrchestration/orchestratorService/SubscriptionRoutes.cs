using Microsoft.AspNetCore.Mvc;

namespace orchestratorService
{
    public static class SubscriptionRoutes
    {
        public static IEndpointRouteBuilder RegisterSubscriptionRoutes(this IEndpointRouteBuilder app)
        {
            app.AddSubscriptionsRoute()
                .AddSubscribeRoute()
                .AddUnsubscribeRoute()
                .AddPublishRoute();

            return app;
        }

        private static IEndpointRouteBuilder AddSubscriptionsRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/subscriptions", ([FromServices] Subscriptions subscriptions) =>
            {
                return Results.Ok(subscriptions.subscriptions);
            });

            return app;
        }

        private static IEndpointRouteBuilder AddSubscribeRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/subscribe/{eventName}/{serviceName}", (
                [FromServices] Subscriptions subscriptions,
                [FromRoute] string eventName,
                [FromRoute] string serviceName) =>
            {
                Console.WriteLine($"/subscribe/{eventName}/{serviceName}");

                subscriptions.Subscribe(eventName, serviceName);

                return Results.Accepted();
            });

            return app;
        }

        private static IEndpointRouteBuilder AddUnsubscribeRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/unsubscribe/{eventName}/{serviceName}", (
                [FromServices] Subscriptions subscriptions,
                [FromRoute] string eventName,
                [FromRoute] string serviceName) =>
            {
                Console.WriteLine($"/unsubscribe/{eventName}/{serviceName}");

                subscriptions.Unsubscribe(eventName, serviceName);

                return Results.Accepted();
            });

            return app;
        }

        private static IEndpointRouteBuilder AddPublishRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/publish/{eventName}", (
                [FromServices] Subscriptions subscriptions,
                [FromRoute] string eventName) =>
            {
                var subscriptionsForEvent = subscriptions
                    .subscriptions
                    .Where(s => s.EventName == eventName);

                Parallel.ForEach(subscriptionsForEvent, (subscription) =>
                {
                    Console.WriteLine($"{subscription.EventName} sent to {subscription.ServiceName}");
                });

                return Results.Ok();
            });

            return app;
        }
    }
}
