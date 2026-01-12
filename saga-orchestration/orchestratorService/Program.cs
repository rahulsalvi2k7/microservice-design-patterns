using orchestratorService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Subscriptions>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.RegisterSubscriptionRoutes();

await app.RunAsync();
