
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<OrderOutbox>();
builder.Services.AddHostedService<OutboxProcessingService>();
// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
app.RegisterOrderRoutes();

await app.RunAsync();
