using LoggerSidecar.Lib;
using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICustomLogger, CustomLogger>();
builder.Services.AddSingleton<LogMessageStore>();
builder.Services.AddHostedService<LogAggregationSideCarService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapPost("/update", async ([FromServices] ICustomLogger customLogger, HttpRequest httpRequest) =>
{
    using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);

    var rawBody = await reader.ReadToEndAsync();

    await customLogger.Info("serviceA", rawBody);

    return Results.Ok(rawBody);
});

await app.RunAsync();
