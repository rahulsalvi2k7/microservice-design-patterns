using Microsoft.AspNetCore.Mvc;
using orchestratorService.lib.Interfaces;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder RegisterOrderRoutes(this IEndpointRouteBuilder app)
    {
        app.AddPlaceOrderRoute()
            .AddCancelOrderRoute()
            .AddSubscriptionRoute();

        return app;
    }

    private static IEndpointRouteBuilder AddPlaceOrderRoute(this IEndpointRouteBuilder app)
    {
        app.MapGet("/place/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
        {
            Console.WriteLine($"order placed {id}");

            await orchestratorClient.Publish("order-placed");

            return Results.Accepted();
        });

        return app;
    }

    private static IEndpointRouteBuilder AddCancelOrderRoute(this IEndpointRouteBuilder app)
    {
        app.MapGet("/cancel/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
        {
            Console.WriteLine($"order cancelled {id}");

            await orchestratorClient.Publish("order-cancelled");

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
