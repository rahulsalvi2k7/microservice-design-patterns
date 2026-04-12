using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder RegisterOrderRoutes(this IEndpointRouteBuilder app)
        {
            app.RegisterCreateRoute().RegisterStatusRoute();

            return app;
        }

        private static IEndpointRouteBuilder RegisterCreateRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/create/{id}", async (
                [FromRoute] string id,
                [FromServices] Outbox orderOutbox,
                [FromServices] SemaphoreSlimProvider semaphoreSlimProvider,
                CancellationToken cancellationToken) =>
            {
                await semaphoreSlimProvider.WaitAsync(cancellationToken);

                try
                {
                    orderOutbox.Messages.Add(new OutboxMessage(id, MessageStatus.Waiting));

                    Console.WriteLine($"Order {id} created");

                    return Results.Accepted();
                }
                finally
                {
                    semaphoreSlimProvider.Release();
                }
            });

            return app;
        }

        private static IEndpointRouteBuilder RegisterStatusRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/status", ([FromServices] Outbox orderOutbox) =>
            {
                return Results.Ok(orderOutbox.Messages);
            });

            return app;
        }
    }
}
