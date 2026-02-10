using OrderService.Extensions;
using OrderService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Outbox>();
builder.Services.AddHostedService<OutboxProcessingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.RegisterOrderRoutes();

await app.RunAsync();
