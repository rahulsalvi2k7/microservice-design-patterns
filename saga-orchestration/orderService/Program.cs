using Microsoft.AspNetCore.Mvc;
using orchestratorService.lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();    
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});
builder.Services.AddSingleton<IOrchestratorClient, OrchestratorClient>();

var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

var orchestratorClient = app.Services.GetRequiredService<IOrchestratorClient>();

lifetime.ApplicationStarted.Register(async () =>
{
    await orchestratorClient.Subscribe("payment-completed", "orderService");
    await orchestratorClient.Subscribe("payment-failed", "orderService");
});

lifetime.ApplicationStopped.Register(async () =>
{
    await orchestratorClient.Unsubscribe("payment-completed", "orderService");
    await orchestratorClient.Unsubscribe("payment-failed", "orderService");
});


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

await app.RunAsync();
