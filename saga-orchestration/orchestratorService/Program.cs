using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Subscriptions>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/subscriptions", ([FromServices] Subscriptions subscriptions) =>
{
    return Results.Ok(subscriptions.subscriptions);
});

app.MapGet("/subscribe/{eventName}/{serviceName}", (
    [FromServices] Subscriptions subscriptions,
    [FromRoute] string eventName,
    [FromRoute] string serviceName) =>
{
    subscriptions.Subscribe(eventName, serviceName);

    return Results.Accepted();
});

app.MapGet("/unsubscribe/{eventName}/{serviceName}", (
    [FromServices] Subscriptions subscriptions,
    [FromRoute] string eventName,
    [FromRoute] string serviceName) =>
{
    subscriptions.Unsubscribe(eventName, serviceName);

    return Results.Accepted();
});

app.MapPost("/publish/{eventName}", (
    [FromServices] Subscriptions subscriptions,
    [FromRoute] string eventName
    ) =>
{
    var subscriptionsForEvent = subscriptions
        .subscriptions
        .Where(s => s.EventName == eventName);

    foreach (var subscription in subscriptionsForEvent)
    {
        Console.WriteLine($"{subscription.EventName} sent to {subscription.ServiceName}");
    }

    return Results.Ok();
});

await app.RunAsync();
