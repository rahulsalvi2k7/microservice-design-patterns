using OrderService;
using ServiceRegistry.Lib.BackgroundServices;
using ServiceRegistry.Lib.Extensions;
using ServiceRegistry.Lib.Implementations;
using ServiceRegistry.Lib.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IServiceClient, ServiceClient>();
builder.Services.AddSingleton<IServiceInfoResolver, ServiceInfoResolver>();
builder.Services.AddHostedService<HeartbeatService>();

var app = builder.Build();

app.RegisterLifetimeEvents();

// Configure the HTTP request pipeline.

app.RegisterRoutes();

await app.RunAsync();
