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
    await serviceClient.Register("paymentService", "http://localhost:5185");
});

lifetime.ApplicationStopped.Register(async () =>
{
    await serviceClient.Unregister("paymentService");
});

// Configure the HTTP request pipeline.

app.MapGet("/pay/{amount}", ([FromRoute] decimal amount) =>
{
    Console.WriteLine($"Paid {amount}");

    return Results.Accepted();
});

await app.RunAsync();
