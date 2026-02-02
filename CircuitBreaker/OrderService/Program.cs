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

builder.Services.AddScoped<IServiceStatusReader, ServiceStatusReader>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ServiceStatusReaderMiddleware>();

app.MapGet("/process/{id}", ([FromRoute] string id) =>
{
    Console.WriteLine($"process {id}");

    return Results.Accepted();
});

await app.RunAsync();
