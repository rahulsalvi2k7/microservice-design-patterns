using Microsoft.AspNetCore.Mvc;
using SwitchService.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

builder.Services.AddTransient<IServiceStatusReader, ServiceStatusReader>();

var app = builder.Build();

app.UseMiddleware<ServiceStatusReaderMiddleware>();

// Configure the HTTP request pipeline.

app.MapGet("/pay/{id}", ([FromRoute] string id) =>
{
    Console.WriteLine($"pay {id}");

    return Results.Accepted();
});

await app.RunAsync();
