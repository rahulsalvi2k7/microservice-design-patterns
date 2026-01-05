using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ServiceStatus>();

builder.Services.ConfigureHttpJsonOptions(options => {
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
    serviceStatus.Id = -1;
    serviceStatus.Name = "Broken";

    return serviceStatus;
});

app.MapGet("/restore", ([FromServices] ServiceStatus serviceStatus) =>
{
    serviceStatus.Id = 0;
    serviceStatus.Name = "Working";

    return serviceStatus;
});

app.MapGet("/process/{id}", async ([FromRoute] string id, [FromServices] ServiceStatus serviceStatus, IHttpClientFactory httpClientFactory) =>
{
    if (serviceStatus.Id < 0) 
    {
        return Results.StatusCode((int)HttpStatusCode.ServiceUnavailable);
    }

    var httpClient = httpClientFactory.CreateClient();

    httpClient.BaseAddress = new Uri("http://localhost:5155");

    var response = await httpClient.GetAsync($"/process/{id}");

    response.EnsureSuccessStatusCode();

    return Results.Ok();
});

await app.RunAsync();

