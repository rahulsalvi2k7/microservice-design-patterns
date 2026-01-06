using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var headers = new[]
{
    new { id = 1, name = "Order 1" },
    new { id = 2, name = "Order 2" },
    new { id = 3, name = "Order 3" },
};

// Add services to the container.

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

app.MapGet("/header/{id}", ([FromRoute] int id) =>
{
    return Results.Ok(headers.First(h => h.id == id));
});

// Configure the HTTP request pipeline.
await app.RunAsync();
