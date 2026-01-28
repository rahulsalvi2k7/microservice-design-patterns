using Microsoft.AspNetCore.Mvc;
using ServiceRegistry;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder RegisterRoutes(this IEndpointRouteBuilder app) 
    {
        app.RegisterHeartbeatRoute()
            .RegisterLocationRoute()
            .RegisterRegistrationRoutes();

        return app;
    }

    private static IEndpointRouteBuilder RegisterRegistrationRoutes(this IEndpointRouteBuilder app) 
    {
        app.MapPost("/register", (
            [FromBody] ServiceRegistrationRequest serviceRegistrationRequest,
            [FromServices] RegistryDictionary registryDictionary) =>
        {
            Console.WriteLine($"{DateTime.UtcNow:s} registering... {serviceRegistrationRequest.Name}");

            registryDictionary.Register(serviceRegistrationRequest);

            return Results.Accepted();
        });

        app.MapPost("/unregister", (
            [FromBody] ServiceRegistrationRequest serviceRegistrationRequest,
            [FromServices] RegistryDictionary registryDictionary) =>
        {
            Console.WriteLine($"{DateTime.UtcNow:s} unregistering... {serviceRegistrationRequest.Name}");

            registryDictionary.Unregister(serviceRegistrationRequest.Name);

            return Results.Accepted();
        });

        return app;
    }

    private static IEndpointRouteBuilder RegisterHeartbeatRoute(this IEndpointRouteBuilder app) 
    {
        app.MapGet("/heartbeat/{name}", ([FromRoute] string name) =>
        {
            Console.WriteLine($"{DateTime.UtcNow:s} heartbeat recevied from {name}");

            return Results.Ok();
        });        

        return app;
    }

    private static IEndpointRouteBuilder RegisterLocationRoute(this IEndpointRouteBuilder app) 
    {
        app.MapGet("/location/{name}", (
            [FromRoute] string name,
            [FromServices] RegistryDictionary registryDictionary) =>
        {
            var location = registryDictionary.Get(name);

            return Results.Ok(location);
        });

        return app;
    }
}
