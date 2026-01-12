using Microsoft.AspNetCore.Mvc;

namespace orchestratorService
{
    public static class SubscriptionRoutes
    {
        public static void RegisterSubscriptionRoutes(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapGet("/subscriptions", ([FromServices] Subscriptions subscriptions) =>
            {
                return Results.Ok(subscriptions.subscriptions);
            });

            endpointRouteBuilder.MapGet("/subscribe/{eventName}/{serviceName}", (
                [FromServices] Subscriptions subscriptions,
                [FromRoute] string eventName,
                [FromRoute] string serviceName) =>
            {
                Console.WriteLine($"/subscribe/{eventName}/{serviceName}");

                subscriptions.Subscribe(eventName, serviceName);

                return Results.Accepted();
            });

            endpointRouteBuilder.MapGet("/unsubscribe/{eventName}/{serviceName}", (
                [FromServices] Subscriptions subscriptions,
                [FromRoute] string eventName,
                [FromRoute] string serviceName) =>
            {
                Console.WriteLine($"/unsubscribe/{eventName}/{serviceName}");

                subscriptions.Unsubscribe(eventName, serviceName);

                return Results.Accepted();
            });

            endpointRouteBuilder.MapGet("/publish/{eventName}", (
                [FromServices] Subscriptions subscriptions,
                [FromRoute] string eventName
                ) =>
            {
                var subscriptionsForEvent = subscriptions
                    .subscriptions
                    .Where(s => s.EventName == eventName);

                // todo : fan-out and send to all subscribers in parallel 

                foreach (var subscription in subscriptionsForEvent)
                {
                    Console.WriteLine($"{subscription.EventName} sent to {subscription.ServiceName}");
                }

                return Results.Ok();
            });
        }
    }
}