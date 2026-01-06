using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var items = new[]
{
    new { headerId = 1, itemName = "item 1" },
    new { headerId = 2, itemName = "item 2" },
    new { headerId = 3, itemName = "item 3" },
};

// Add services to the container.

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

app.MapGet("/items/{headerId}", ([FromRoute] int headerId) =>
{
    return Results.Ok(items.Where(h => h.headerId == headerId).ToList());
});

// Configure the HTTP request pipeline.
await app.RunAsync();
