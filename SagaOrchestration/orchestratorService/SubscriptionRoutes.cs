using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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
            app.MapPost("/publish/{eventName}", async (
                [FromServices] ChannelProvider channelProvider,
                [FromRoute] string eventName,
                HttpRequest request) =>
            {
                #region valiadte request body

                request.EnableBuffering();

                using var reader = new StreamReader(request.Body);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(body)) 
                {
                    return Results.BadRequest(new { error = "Missing JSON body" });
                }

                #endregion valiadte request body

                Console.WriteLine($"{DateTime.UtcNow:s} => event received {eventName}");

                try
                {
                    if (eventName.Equals("order-placed"))
                    {
                        var data = JObject.Parse(body);

                        var sagaRequest = new OrderSagaRequest(
                            sagaId: Guid.NewGuid(),
                            orderId: (string)data["orderId"]);

                        await channelProvider.ChannelWriter.WriteAsync(sagaRequest);
                    }

                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { error = "Invalid JSON", details = ex.Message });
                }
                
            });

            return app;
        }
    }
}
