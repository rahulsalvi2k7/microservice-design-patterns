# Event Sourcing sample

This folder contains a minimal event-sourcing example built with ASP.NET Core and a small HTML client.

## What is included

- `Domain.api/` — minimal API that stores order events in an in-memory event store.
- `DomainEvent.Lib/` — shared domain objects, event types, and the event-store abstraction.
- `Domain.Client/` — simple browser client that streams replayed events from the API.

## Key concepts demonstrated

- Event creation for an order lifecycle (`OrderCreatedEvent`, `OrderShippedEvent`, `OrderReceivedEvent`).
- Rebuilding current state by replaying stored events.
- Hydrating an aggregate from its event history.
- Using an in-memory event store for learning and local experimentation.

## How to run

1. Start the API:

   ```powershell
   cd EventSourcing/Domain.api
   dotnet run
   ```

2. Open the client page in a browser:

   ```powershell
   start EventSourcing/Domain.Client/index.html
   ```

   The client currently calls `http://localhost:5135/replay/1`, so the API must be running on that port.

## API endpoints

The sample exposes these routes in `Domain.api`:

- `POST /order-create/{id}` — append an `OrderCreatedEvent`.
- `POST /order-ship/{id}` — append an `OrderShippedEvent`.
- `POST /order-receive/{id}` — append an `OrderReceivedEvent`.
- `GET /hydrate/{id}` — rebuild the order state from all stored events.
- `GET /replay/{id}` — stream the event history back to the client.

## Project structure

```text
EventSourcing/
├── Domain.api/          # Web API and endpoint mapping
├── Domain.Client/       # Simple browser UI
└── DomainEvent.Lib/     # Event models and in-memory event store
```

## Notes

- This sample uses an in-memory event store, so events are lost when the API process stops.
- It is intended as a learning example rather than a production-ready persistence solution.
