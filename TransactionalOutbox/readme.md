# Transactional Outbox Pattern

[![Built with .NET 8](https://img.shields.io/badge/Built%20with-.NET%208-512bd4?style=flat-square)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](../LICENSE)

A demonstration of the **Transactional Outbox pattern** for reliable event publishing in distributed systems. This pattern ensures that events are consistently stored within the same transaction as the business operation, solving the distributed transaction problem and preventing message loss.

## Overview

The Transactional Outbox pattern is a proven approach to maintain consistency when publishing events from microservices. Instead of directly publishing events to external systems, events are:

1. **Stored locally** in an outbox table within the same database transaction as the business operation
2. **Processed asynchronously** by a background service
3. **Published reliably** to downstream services, with built-in retry logic and failure handling

This approach eliminates the risk of data being committed while the event publication fails, ensuring strong consistency guarantees in distributed systems.

> [!NOTE]
> This sample implements a simplified in-memory outbox for demonstration purposes. In production, you would typically use a database table to persist the outbox messages.

## Pattern Components

### OrderService

The primary service that:
- Creates orders with associated outbox messages
- Provides endpoints to create orders and check the status of messages
- Maintains an in-memory outbox containing pending, sent, and failed messages

**Key files:**
- `Models/Order.cs` — Order entity
- `Models/Outbox.cs` — In-memory outbox storage
- `Models/OutboxMessage.cs` — Individual outbox message
- `Services/OutboxProcessingService.cs` — Background service that processes outbox messages

**Endpoints:**
- `GET /create/{id}` — Creates a new order and adds a message to the outbox
- `GET /status` — Returns the current state of all outbox messages

### NotificationService

A lightweight service that:
- Receives notifications from OrderService
- Simulates a downstream consumer in the event-driven architecture

**Endpoints:**
- `GET /notify/{id}` — Receives notification for an order

## Architecture

```
┌─────────────┐
│  Orderervice│
│ ┌─────────┐ │
│ │ Outbox  │ └─── Pending Messages ───┐
│ └─────────┘ │                        │
└─────────────┘                        │
     │                                 │
     │ (Background Service polls)      │
     │                                 │
     └─────────────────────────────┐───┘
                                   │
                    ┌──────────────▼─────────────┐
                    │ OutboxProcessingService    │
                    │ (Processes & Retries)      │
                    └──────────────┬─────────────┘
                                   │
                                   ▼
                    ┌────────────────────────────┐
                    │ NotificationService        │
                    │ (Sends Notification)       │
                    └────────────────────────────┘
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/) or later
- A terminal (PowerShell on Windows, bash on macOS/Linux)

### Quickstart

1. **Terminal 1 — Start the NotificationService:**

```powershell
cd .\NotificationService
dotnet run
```

The service will listen on `http://localhost:5153`

2. **Terminal 2 — Start the OrderService:**

```powershell
cd .\OrderService
dotnet run
```

The service will listen on `http://localhost:5295`

3. **Test the sample:**

Use your browser, VS Code REST Client, or a tool like curl to send requests:

```bash
# Create an order (adds a message to the outbox)
curl http://localhost:5295/create/ORDER-001

# Check outbox status
curl http://localhost:5295/status
```

The OrderService will automatically process the outbox messages every 10 seconds and send notifications to the NotificationService.

## How It Works

1. **Order Creation**: When you call `/create/{id}`, the OrderService:
   - Creates a new order
   - Adds an `OutboxMessage` with status `Waiting` to the in-memory outbox
   - Returns immediately

2. **Background Processing**: The `OutboxProcessingService`:
   - Polls the outbox every 10 seconds
   - Finds messages with status `Waiting`
   - Attempts to send them to the NotificationService
   - Updates the message status to `Sent` or `Failed`

3. **Status Tracking**: Call `/status` at any time to see:
   - `Waiting` — Messages queued for processing
   - `Sent` — Successfully delivered messages
   - `Failed` — Messages that could not be delivered

## Message States

| Status | Meaning | Action |
|--------|---------|--------|
| **Waiting** | Message added to outbox, not yet processed | Processed by background service |
| **Sent** | Successfully delivered to downstream service | No further action |
| **Failed** | Delivery attempt failed | Needs manual intervention or retry |

## Example Flow

```
1. Request: GET /create/ORDER-123
   ✓ Order persisted
   ✓ Message added to outbox with status "Waiting"
   Response: 202 Accepted

2. Background service polls every 10 seconds:
   - Finds message with status "Waiting"
   - Attempts: POST to NotificationService

3. Response from NotificationService: 200 OK
   - Updates message status to "Sent"

4. Request: GET /status
   Response: [{ Id: "ORDER-123", MessageStatus: "Sent" }]
```

## Production Considerations

When implementing this pattern in production environments:

- **Persistence**: Replace the in-memory outbox with a database table and use proper transactions
- **Idempotency**: Ensure downstream consumers can handle duplicate messages
- **Monitoring**: Add observability to track message processing and failed deliveries
- **Retry Strategy**: Implement exponential backoff and dead-letter queues
- **Cleanup**: Periodically archive or delete successfully processed messages
- **Configuration**: Make the polling interval and timeout values configurable

## Running Tests

Currently, this sample does not include unit tests. To add tests:

```powershell
dotnet new xunit -n TransactionalOutbox.Tests
dotnet test
```

## Related Patterns

- **Saga Pattern**: For distributed transactions across multiple services
- **Event Sourcing**: For maintaining an audit log of all state changes
- **Inbox Pattern**: The counterpart pattern for consuming events reliably
- **Dead Letter Queue**: For handling messages that cannot be processed

## Learn More

- [Transactional Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html) — Chris Richardson's microservices patterns
- [Event-Driven Architecture](https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/event-driven) — Microsoft Learn
- [Reliable Message Delivery with Event Sourcing](https://www.red-gate.com/simple-talk/) — Database best practices

## Troubleshooting

**Notifications not being sent?**
- Ensure NotificationService is running on `http://localhost:5153`
- Check the console output for any error messages
- Verify no firewall is blocking communication between services

**Messages stuck in "Waiting" status?**
- Check if OutboxProcessingService is running
- Verify NotificationService is accessible
- Check the console logs for connection errors

**Port already in use?**
- Update the port in `appsettings.json` or environment variables
- Ensure no other services are using the same ports

## Files Structure

```
TransactionalOutbox/
├── OrderService/
│   ├── Models/
│   │   ├── Order.cs
│   │   ├── Outbox.cs
│   │   ├── OutboxMessage.cs
│   │   └── MessageStatus.cs
│   ├── Services/
│   │   └── OutboxProcessingService.cs
│   ├── Extensions/
│   │   └── EndpointRouteBuilderExtensions.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── OrderService.csproj
├── NotificationService/
│   ├── Program.cs
│   ├── appsettings.json
│   └── NotificationService.csproj
└── README.md
```

## Summary

The Transactional Outbox pattern provides a robust solution for reliable event publishing in microservices. By storing events locally before publishing them, this pattern ensures no events are lost even if external systems fail, making it essential for building resilient distributed systems.
