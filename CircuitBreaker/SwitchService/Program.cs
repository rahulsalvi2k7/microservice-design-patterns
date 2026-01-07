using Microsoft.AspNetCore.Mvc;
using SwitchService.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ServiceStatus>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/status", ([FromServices] ServiceStatus serviceStatus) =>
{
    return serviceStatus;
});

app.MapGet("/break", ([FromServices] ServiceStatus serviceStatus) =>
{
    serviceStatus.Id = 0;
    serviceStatus.Name = "Broken";

    return serviceStatus;
});

app.MapGet("/restore", ([FromServices] ServiceStatus serviceStatus) =>
{
    serviceStatus.Id = 1;
    serviceStatus.Name = "Working";

    return serviceStatus;
});

await app.RunAsync();
