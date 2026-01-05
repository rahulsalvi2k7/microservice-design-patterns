using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/process/{id}", ([FromRoute] string id) =>
{
    Console.WriteLine($"ServiceB => Received {id}");
});

await app.RunAsync();
