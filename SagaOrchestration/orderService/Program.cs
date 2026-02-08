using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using orchestratorService.lib.Extensions;
using orderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.RegisterOrchestrationServices();

var app = builder.Build();

app.RegisterLifetimeEvents();

// Configure the HTTP request pipeline.
app.RegisterOrderRoutes();

await app.RunAsync();
