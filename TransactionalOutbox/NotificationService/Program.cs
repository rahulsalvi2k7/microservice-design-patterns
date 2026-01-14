using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/notify/{id}", ([FromRoute] string id) =>
{
    Console.WriteLine($"Notified for order {id}");

    return Results.Ok();
});

await app.RunAsync();
