using DomainEvents.Lib.BusinessObjects;
using DomainEvents.Lib.Events;
using DomainEvents.Lib.EventStore;
using Microsoft.AspNetCore.Mvc;

namespace Domain.api
{
    public static class OrderRouteExtensions
    {
        public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder app)
        {
            app.MapCreateOrderRoute()
                .MapShipOrderRoute()
                .MapReceiveOrderRoute();

            return app;
        }

        public static IEndpointRouteBuilder MapCreateOrderRoute(this IEndpointRouteBuilder app)
        {
            app.MapPost("/order-create/{id}", ([FromServices] IEventStore<OrderBusinessObject> eventStore, [FromRoute] int id) =>
            {
                Console.WriteLine($"Order created : {id}");

                eventStore.Save(new OrderCreatedEvent()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    BusinessObject = new OrderBusinessObject
                    {
                        Id = id
                    }
                });
            });

            return app;
        }

        public static IEndpointRouteBuilder MapShipOrderRoute(this IEndpointRouteBuilder app)
        {
            app.MapPost("/order-ship/{id}", ([FromServices] IEventStore<OrderBusinessObject> eventStore, [FromRoute] int id) =>
            {
                Console.WriteLine($"Order shipped : {id}");
                eventStore.Save(new OrderShippedEvent()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    BusinessObject = new OrderBusinessObject
                    {
                        Id = id
                    }
                });
            });

            return app;
        }

        public static IEndpointRouteBuilder MapReceiveOrderRoute(this IEndpointRouteBuilder app)
        {
            app.MapPost("/order-receive/{id}", ([FromServices] IEventStore<OrderBusinessObject> eventStore, [FromRoute] int id) =>
            {
                Console.WriteLine($"Order received : {id}");
                eventStore.Save(new OrderReceivedEvent()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    BusinessObject = new OrderBusinessObject
                    {
                        Id = id
                    }
                });
            });

            return app;
        }
    }
}