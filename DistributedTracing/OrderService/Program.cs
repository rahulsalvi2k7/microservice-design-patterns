using Microsoft.AspNetCore.Mvc;
using Tracer.Lib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddTransient<ITracer, Tracer.Lib.Tracer>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<TracerMiddleware>();

app.MapGet("/create/{id}", async ([FromRoute] string id, [FromServices] IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory) =>
{
    var httpClient = clientFactory.CreateClient();
    httpClient.BaseAddress = new Uri("http://localhost:5264");
    var traceId = httpContextAccessor?.HttpContext?.Request.Headers["x-trace-id"].ToString() ?? Guid.NewGuid().ToString();

    httpClient.DefaultRequestHeaders.Clear();
    httpClient.DefaultRequestHeaders.Add("x-trace-id", traceId);

    var response = await httpClient.GetAsync($"/pay/{id}");

    return Results.Accepted();
});

await app.RunAsync();
