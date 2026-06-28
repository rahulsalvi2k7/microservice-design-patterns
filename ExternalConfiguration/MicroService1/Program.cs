Dictionary<string, string>? _configuration = null;

var builder = WebApplication.CreateBuilder(args);

int ApplicationId = int.Parse(builder.Configuration["ApplicationId"]);

// Add services to the container.

builder.Services.AddHttpClient("ConfigService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ConfigService:BaseUrl"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.Lifetime.ApplicationStarted.Register(() =>
{
    var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();

    var client = httpClientFactory.CreateClient("ConfigService");

    var response = client.GetAsync($"/load/{ApplicationId}").Result;

    response.EnsureSuccessStatusCode();

    _configuration = response.Content.ReadFromJsonAsync<Dictionary<string, string>>().Result;
});

app.MapGet("/", () =>
{
    return Results.Ok(_configuration);
});

await app.RunAsync();