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

app.MapPost("/log", async (HttpRequest httpRequest) =>
{
    using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8);

    var rawBody = await reader.ReadToEndAsync();
     
    await File.AppendAllTextAsync(fileName, rawBody + Environment.NewLine);
});

await app.RunAsync();
