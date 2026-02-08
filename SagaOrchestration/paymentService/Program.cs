using orchestratorService.lib.Extensions;
using paymentService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterOrchestrationServices();

var app = builder.Build();

app.RegisterLifetimeEvents();

// Configure the HTTP request pipeline.
app.RegisterPaymentRoutes();

await app.RunAsync();
