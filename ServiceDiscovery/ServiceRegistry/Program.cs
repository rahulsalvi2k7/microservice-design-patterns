using ServiceRegistry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<RegistryDictionary>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.RegisterRoutes();

await app.RunAsync();
