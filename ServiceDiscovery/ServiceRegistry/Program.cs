using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<RegistryDictionary>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapPost("/register", (
    [FromBody] ServiceRegistrationRequest serviceRegistrationRequest,
    [FromServices] RegistryDictionary registryDictionary) =>
{
    Console.WriteLine($"registering... {serviceRegistrationRequest.Name}");

    registryDictionary.Register(serviceRegistrationRequest);

    return Results.Accepted();
});

app.MapPost("/unregister", (
    [FromBody] ServiceRegistrationRequest serviceRegistrationRequest,
    [FromServices] RegistryDictionary registryDictionary) =>
{
    Console.WriteLine($"unregistering... {serviceRegistrationRequest.Name}");

    registryDictionary.Unregister(serviceRegistrationRequest.Name);

    return Results.Accepted();
});

app.MapGet("/heartbeat/{name}", ([FromRoute] string name) =>
{
    Console.WriteLine($"heartbeat recevied from {name}");

    return Results.Ok();
});

app.MapGet("/location/{name}", (
    [FromRoute] string name,
    [FromServices] RegistryDictionary registryDictionary) =>
{
    var location = registryDictionary.Get(name);

    return Results.Ok(location);
});

await app.RunAsync();
