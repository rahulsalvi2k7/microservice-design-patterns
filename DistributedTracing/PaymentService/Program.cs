using Microsoft.AspNetCore.Mvc;
using Tracer.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddTransient<ITracer, Tracer.Lib.Tracer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<TracerMiddleware>();

app.MapGet("/pay/{id}", ([FromRoute] string id) =>
{
    return Results.Accepted(id);
});

await app.RunAsync();
