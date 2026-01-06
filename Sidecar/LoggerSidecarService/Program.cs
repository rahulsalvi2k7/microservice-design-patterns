using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

var fileName = $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log";

// Configure the HTTP request pipeline.

app.MapPost("/info", async (HttpRequest httpRequest) =>
{
    using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);

    var rawBody = await reader.ReadToEndAsync();

    var message = $"{DateTime.UtcNow:s} : {rawBody}{Environment.NewLine}";

    await File.AppendAllTextAsync(fileName, message);
});

app.MapPost("/error", async (HttpRequest httpRequest) =>
{
    using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);

    var rawBody = await reader.ReadToEndAsync();

    var message = $"{DateTime.UtcNow:s} : *** ERROR*** : {rawBody}{Environment.NewLine}";

    await File.AppendAllTextAsync(fileName, message);
});

await app.RunAsync();
