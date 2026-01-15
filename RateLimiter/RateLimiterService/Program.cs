using Microsoft.AspNetCore.Mvc;
using RateLimiterService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<TenantRate>();

var app = builder.Build();

app.UseMiddleware<TenantResolverMiddleware>();
app.UseMiddleware<TenantRateLimiterMiddleware>();

// Configure the HTTP request pipeline.

app.MapGet("/", async () =>
{
    Console.WriteLine("request received");

    await Task.Delay(10_000);

    return Results.Ok();
});

await app.RunAsync();
