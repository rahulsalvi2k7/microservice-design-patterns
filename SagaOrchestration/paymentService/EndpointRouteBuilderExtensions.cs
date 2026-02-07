using Microsoft.AspNetCore.Mvc;
using orchestratorService.lib.Interfaces;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder RegisterPaymentRoutes(this IEndpointRouteBuilder app)
    {
        app.AddCompleteRoute()
            .AddFailRoute()
            .AddSubscriptionRoute();

        return app;
    }

    private static IEndpointRouteBuilder AddCompleteRoute(this IEndpointRouteBuilder app)
    {
        app.MapGet("/complete/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
        {
            Console.WriteLine($"payment completed {id}");

            await orchestratorClient.Publish("payment-completed");

            return Results.Accepted();
        });

        return app;
    }

    private static IEndpointRouteBuilder AddFailRoute(this IEndpointRouteBuilder app)
    {
        app.MapGet("/fail/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
        {
            Console.WriteLine($"payment failed {id}");

            await orchestratorClient.Publish("payment-failed");

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

            return await Task.FromResult(Results.Ok());
        });

        return app;
    }
}
