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

app.MapGet("/complete/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
{
    Console.WriteLine($"payment completed {id}");

    await orchestratorClient.Publish("payment-completed");

    return Results.Accepted();
});

app.MapGet("/fail/{id}", async ([FromRoute] string id, [FromServices] IOrchestratorClient orchestratorClient) =>
{
    Console.WriteLine($"payment failed {id}");

    await orchestratorClient.Publish("payment-failed");

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
