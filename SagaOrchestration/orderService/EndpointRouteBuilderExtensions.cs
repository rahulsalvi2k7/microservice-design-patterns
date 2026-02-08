using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using orchestratorService.lib.Interfaces;

namespace orderService
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder RegisterOrderRoutes(this IEndpointRouteBuilder app)
        {
            app.AddPlaceOrderRoute()
                .AddCompleteOrderRoute()
                .AddCancelOrderRoute()
                .AddSubscriptionRoute();

            return app;
        }

        private static IEndpointRouteBuilder AddPlaceOrderRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/place/{id}/{amount}", async ([FromRoute] string id, [FromRoute] int amount, [FromServices] IOrchestratorClient orchestratorClient) =>
            {
                Console.WriteLine($"{DateTime.UtcNow:s} => order placed {id} {amount}");

                await orchestratorClient.Publish("order-placed", JObject.FromObject(new { orderId = id, amount }));

                return Results.Accepted();
            });

            return app;
        }

        private static IEndpointRouteBuilder AddCompleteOrderRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/complete/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
            {
                Console.WriteLine($"{DateTime.UtcNow:s} => order completed {id}");

                await orchestratorClient.Publish("order-completed", JObject.FromObject(new { orderId = id }));

                return Results.Accepted();
            });

            return app;
        }

        private static IEndpointRouteBuilder AddCancelOrderRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/cancel/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
            {
                Console.WriteLine($"{DateTime.UtcNow:s} => order cancelled {id}");

                await orchestratorClient.Publish("order-cancelled", JObject.FromObject(new { orderId = id }));

                return Results.Accepted();
            });

            return app;
        }

        private static IEndpointRouteBuilder AddSubscriptionRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/subscription/{eventName}", async (
                [FromRoute] string eventName,
                [FromServices] IServiceInfoResolver serviceInfoResolver) =>
            {
                Console.WriteLine($"{serviceInfoResolver.GetServiceName()} reacting to event {eventName}");

                return Results.Ok();
            });

            return app;
        }
    }
}