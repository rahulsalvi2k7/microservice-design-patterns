using Microsoft.AspNetCore.Mvc;
using ServiceRegistry.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IServiceClient, ServiceClient>();
builder.Services.AddSingleton<IServiceInfoResolver, ServiceInfoResolver>();
builder.Services.AddHostedService<HeartbeatService>();

var app = builder.Build();

app.RegisterLifetimeEvents();

// Configure the HTTP request pipeline.

app.MapGet("/pay/{amount}", ([FromRoute] decimal amount) =>
{
    Console.WriteLine($"{DateTime.UtcNow:s} Paid {amount}");

    return Results.Accepted();
});

await app.RunAsync();
