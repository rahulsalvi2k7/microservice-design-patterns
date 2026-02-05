using Microsoft.AspNetCore.Mvc;
using orchestratorService.lib.Extensions;
using orchestratorService.lib.Implementation;
using orchestratorService.lib.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();    
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();
builder.Services.AddSingleton<IServiceInfoResolver, ServiceInfoResolver>();

var app = builder.Build();

app.RegisterLifetimeEvents();

// Configure the HTTP request pipeline.

app.MapGet("/place/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
{
    Console.WriteLine($"order placed {id}");

    await orchestratorClient.Publish("order-placed");

    return Results.Accepted();
});

app.MapGet("/cancel/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
{
    Console.WriteLine($"order cancelled {id}");

    await orchestratorClient.Publish("order-cancelled");

    return Results.Accepted();
});

app.MapGet("/subscription/{eventName}", async (
    [FromRoute] string eventName,
    [FromServices] IServiceInfoResolver serviceInfoResolver) =>
{
    Console.WriteLine($"{serviceInfoResolver.GetServiceName()} reacting to event {eventName}");

    return Results.Ok();
});

await app.RunAsync();
