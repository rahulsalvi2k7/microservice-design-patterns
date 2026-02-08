using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using orchestratorService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Subscriptions>();
builder.Services.AddTransient<OrderSaga>();
builder.Services.AddSingleton<ChannelProvider>();
builder.Services.AddHostedService<SagaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.RegisterSubscriptionRoutes();

await app.RunAsync();
