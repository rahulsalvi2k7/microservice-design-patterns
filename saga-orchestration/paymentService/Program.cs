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
    await orchestratorClient.Subscribe("order-placed", "paymentService");
    await orchestratorClient.Subscribe("order-cancelled", "paymentService");
});

lifetime.ApplicationStopped.Register(async () =>
{
    await orchestratorClient.Unsubscribe("order-placed", "paymentService");
    await orchestratorClient.Unsubscribe("order-cancelled", "paymentService");
});

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

await app.RunAsync();
