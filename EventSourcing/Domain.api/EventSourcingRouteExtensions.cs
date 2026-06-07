using DomainEvents.Lib.BusinessObjects;
using DomainEvents.Lib.Events;
using DomainEvents.Lib.EventStore;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Domain.api
{
    public static class EventSourcingRouteExtensions
    {
        public static IEndpointRouteBuilder MapHydrateRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/hydrate/{id}", ([FromServices] IEventStore<OrderBusinessObject> eventStore, [FromRoute] int id) =>
            {
                var events = eventStore
                    .GetAllEvents<OrderBusinessObject>(e => e.BusinessObject != null && e.BusinessObject.Id == id)
                    .OrderBy(e => e.CreatedAt);

                var order = new OrderBusinessObject
                {
                    Id = id
                };

                eventStore.Apply(order, events);

                return Results.Ok(order);
            });

            return app;
        }

        public static IEndpointRouteBuilder MapReplayRoute(this IEndpointRouteBuilder app)
        {
            app.MapGet("/replay/{id}", (CancellationToken cancellationToken, [FromServices] IEventStore<OrderBusinessObject> eventStore, [FromRoute] int id) =>
            {
                async IAsyncEnumerable<DomainEvent<OrderBusinessObject>> GetEvents([EnumeratorCancellation] CancellationToken cancellationToken)
                {
                    var events = eventStore
                        .GetAllEvents<OrderBusinessObject>(e => e.BusinessObject != null && e.BusinessObject.Id == id)
                        .OrderBy(e => e.CreatedAt);

                    foreach (var e in events)
                    {
                        // Simulate delay for replaying events
                        await Task.Delay(1000, cancellationToken);

                        yield return e;
                    }
                }

                return GetEvents(cancellationToken);
            });

            return app;
        }
    }
}