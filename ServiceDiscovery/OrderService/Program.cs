using Microsoft.AspNetCore.Mvc;
using ServiceRegistry.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IServiceClient, ServiceClient>();

var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var serviceClient = app.Services.GetRequiredService<IServiceClient>();

lifetime.ApplicationStarted.Register(async () =>
{
    await serviceClient.Register("orderService", "http://localhost:5247");
});

lifetime.ApplicationStopped.Register(async () =>
{
    await serviceClient.Unregister("orderService");
});

// Configure the HTTP request pipeline.

app.MapGet("/order/{id}/pay/{amount}", async (
    [FromServices] IServiceClient serviceClient,
    [FromServices] IHttpClientFactory httpClientFactory,
    [FromRoute] int id,
    [FromRoute] decimal amount) =>
{
    Console.WriteLine($"order {id} pay {amount}");
    
    var location = await serviceClient.GetLocation("paymentService");

    var client = httpClientFactory.CreateClient();

    client.BaseAddress = new Uri(location.Replace("\"", string.Empty));

    var response = await client.GetAsync($"/pay/{amount}");

    response.EnsureSuccessStatusCode();

    return Results.Accepted();
});


await app.RunAsync();
