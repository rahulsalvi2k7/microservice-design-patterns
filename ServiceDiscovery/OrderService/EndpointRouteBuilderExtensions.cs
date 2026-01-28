using Microsoft.AspNetCore.Mvc;
using ServiceRegistry.Lib;

namespace OrderService
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder RegisterRoutes(this IEndpointRouteBuilder app)
        {
            app.RegisterOrderRoutes();

            return app;
        }

        private static IEndpointRouteBuilder RegisterOrderRoutes(this IEndpointRouteBuilder app)
        {
            app.MapGet("/order/{id}/pay/{amount}", async (
                [FromServices] IServiceClient serviceClient,
                [FromServices] IHttpClientFactory httpClientFactory,
                [FromRoute] int id,
                [FromRoute] decimal amount) =>
            {
                Console.WriteLine($"{DateTime.UtcNow:s} order {id} pay {amount}");

                var location = await serviceClient.GetLocation("paymentService");

                var client = httpClientFactory.CreateClient();

                client.BaseAddress = new Uri(location.Replace("\"", string.Empty));

                var response = await client.GetAsync($"/pay/{amount}");

                response.EnsureSuccessStatusCode();

                return Results.Accepted();
            });

            return app;
        }
    }
}