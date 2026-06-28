using Config.Library;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InitializeExternalConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/load/{applicationId}", async ([FromRoute] int applicationId, [FromServices] Task<ApplicationConfiguration> configTask) =>
{
    var config = await configTask;

    if (config.TryGetValue(new Application { Id = applicationId }, out var configurations))
    {
        return Results.Ok(configurations);
    }

    return Results.NotFound();
});

app.MapPost("/set/{applicationId}", async (
    [FromRoute] int applicationId, 
    [FromServices] Task<ApplicationConfiguration> configTask, 
    [FromBody] Dictionary<string, string> newConfigurations) =>
{
    var config = await configTask;

    if (config.TryGetValue(new Application { Id = applicationId }, out var configurations))
    {
        foreach (var kvp in newConfigurations)
        {
            configurations[kvp.Key] = kvp.Value;
        }
        return Results.Ok(configurations);
    }

    return Results.NotFound();
});


await app.RunAsync();
