using LoggerSidecar.Lib;
using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICustomLogger, CustomLogger>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();


app.MapPost("/info", async ([FromServices] ICustomLogger customLogger, HttpRequest httpRequest) =>
{
    using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);

    var rawBody = await reader.ReadToEndAsync();

    await customLogger.Info(rawBody);

    return Results.Ok(rawBody);
});

app.MapPost("/error", async ([FromServices] ICustomLogger customLogger, HttpRequest httpRequest) =>
{
    using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);

    var rawBody = await reader.ReadToEndAsync();

    await customLogger.Error(rawBody);

    return Results.Ok(rawBody);
});

// Configure the HTTP request pipeline.
await app.RunAsync();