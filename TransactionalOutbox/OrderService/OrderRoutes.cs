using Microsoft.AspNetCore.Mvc;

public static class OrderRoutes
{
    public static void RegisterOrderRoutes(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/create/{id}", (
            [FromRoute] string id,
            [FromServices] OrderOutbox orderOutbox) =>
        {
            orderOutbox.Messages.Add(new OrderOutboxMessage
            {
                Id = id,
                MessageStatus = MessageStatus.Waiting
            });

            Console.WriteLine($"Order {id} created");

            return Results.Accepted();
        });

        endpointRouteBuilder.MapGet("/status", ([FromServices] OrderOutbox orderOutbox) =>
        {
            return Results.Ok(orderOutbox.Messages);
        });
    }
}
