using Domain.api;
using DomainEvent.Lib.EventStore;
using DomainEvents.Lib.BusinessObjects;
using DomainEvents.Lib.EventStore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IEventStore<OrderBusinessObject>, InMemoryEventStore<OrderBusinessObject>>();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

// Configure the HTTP request pipeline.

app.MapRoutes()
   .MapHydrateRoute()
   .MapReplayRoute();

await app.RunAsync();
