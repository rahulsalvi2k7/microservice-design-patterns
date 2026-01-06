using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

builder.Services.AddHttpClient();
builder.Services.RegisterClients();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/aggregate/{id}", async (
    [FromRoute] int id, 
    [FromServices] HeaderClient headerClient,
    [FromServices] ItemsClient itemsClient) =>
{
    var headerTask = headerClient.GetHeaderAsync(id);
    var itemsTask = itemsClient.GetItemsAsync(id);

    await Task.WhenAll(headerTask, itemsTask);

    var header = headerTask.Result;
    var items = itemsTask.Result;

    header["items"] = items;

    return Results.Ok(JsonConvert.SerializeObject(header, Formatting.Indented));
});

await app.RunAsync();
