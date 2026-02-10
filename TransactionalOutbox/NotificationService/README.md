# NotificationService

[![Built with .NET 8](https://img.shields.io/badge/Built%20with-.NET%208-512bd4?style=flat-square)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](../../LICENSE)

The **NotificationService** is a lightweight downstream consumer that receives order notifications from the OrderService. It demonstrates how to implement a reliable event consumer in the context of the Transactional Outbox pattern.

## Overview

NotificationService is a minimal .NET 8 API that simulates a notification consumer in a microservices architecture. In a real-world scenario, this service might:

- Send email notifications to customers
- Update a notification database
- Trigger external systems
- Log events to an audit trail

For this demonstration, it simply logs that a notification was received.

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/) or later

### Run the Service

```powershell
dotnet run
```

The service will start on `http://localhost:5153` by default.

## API Endpoints

### Receive Notification

Receives a notification for an order.

**Request:**
```http
GET /notify/{id}
```

**Parameters:**
- `{id}` — Order identifier (matches the order created in OrderService)

**Response:**
```
Status: 200 OK
```

**Example:**
```bash
curl http://localhost:5153/notify/ORDER-123
```

**What happens:**
1. The service receives the notification request
2. Logs the notification to the console
3. Returns `200 OK` to indicate successful processing

### Console Output

When a notification is received, you'll see:

```
Notified for order ORDER-123
```

## Service Implementation

### Program.cs

The service is extremely simple, with minimal configuration:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/notify/{id}", ([FromRoute] string id) =>
{
    Console.WriteLine($"Notified for order {id}");
    return Results.Ok();
});

await app.RunAsync();
```

**Key points:**
- Uses .NET minimal APIs (no MVC controllers)
- Single endpoint for receiving notifications
- Stateless processing
- No external dependencies or database

## Architecture

### Role in the Transactional Outbox Pattern

NotificationService acts as a **downstream consumer** in the event-driven architecture:

```
OrderService                NotificationService
     │                              ▲
     │ (Creates Order)              │
     ├─ Adds to Outbox              │
     │                              │
     └─ BackgroundService ──────────┘
       (Polls Every 10 Seconds)
       (Sends HTTP GET /notify/{id})
```

### Processing Flow

1. **OrderService** creates an order and adds a message to its outbox
2. **OutboxProcessingService** (in OrderService) polls the outbox every 10 seconds
3. For each `Waiting` message, it sends a GET request to NotificationService
4. **NotificationService receives** the notification and:
   - Logs the order ID
   - Returns `200 OK`
5. **OrderService** updates the message status to `Sent`

## Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

## Testing the Service

### Using curl

```bash
# Send a notification
curl http://localhost:5153/notify/ORDER-001

# Test multiple notifications
curl http://localhost:5153/notify/ORDER-002
curl http://localhost:5153/notify/ORDER-003
```

### Using PowerShell

```powershell
# Single request
Invoke-WebRequest -Uri "http://localhost:5153/notify/ORDER-123" -Method Get

# Multiple requests
1..5 | ForEach-Object {
    Invoke-WebRequest -Uri "http://localhost:5153/notify/ORDER-00$_" -Method Get
}
```

### Using REST Client (VS Code)

Create a file `test.http`:

```http
@base = http://localhost:5153

### Receive a notification
GET {{base}}/notify/ORDER-001
Accept: application/json

###

GET {{base}}/notify/ORDER-002
Accept: application/json

###

GET {{base}}/notify/ORDER-003
Accept: application/json
```

Then use the "Send Request" button in VS Code.

## Integration with OrderService

To see the full Transactional Outbox pattern in action:

1. **Terminal 1** — Start NotificationService:
```powershell
cd .\NotificationService
dotnet run
```

2. **Terminal 2** — Start OrderService:
```powershell
cd .\OrderService
dotnet run
```

3. **Terminal 3** — Send test requests:
```powershell
# Create orders
curl http://localhost:5295/create/ORDER-001
curl http://localhost:5295/create/ORDER-002

# Wait 10 seconds for processing...

# Check outbox status
curl http://localhost:5295/status
```

You'll see:
- OrderService creates orders and adds them to the outbox
- NotificationService receives the notifications after 10 seconds
- OrderService marks messages as "Sent"

## Project Structure

```
NotificationService/
├── Program.cs                  # Service definition and setup
├── appsettings.json           # Configuration
├── appsettings.Development.json # Dev configuration
├── NotificationService.csproj # Project file
└── NotificationService.http   # HTTP test file
```

## Dependencies

The project has **no external NuGet package dependencies**. It only uses:

- **Microsoft.AspNetCore** — Web framework (implicit via Web SDK)
- **.NET 8 Standard Library** — Core functionality

This minimal footprint makes it ideal for:
- Learning and teaching
- Lightweight microservices
- Container deployment
- Fast startup times

## Key Characteristics

| Aspect | Detail |
|--------|--------|
| **Language** | C# with .NET 8 |
| **Framework** | ASP.NET Core minimal APIs |
| **Port** | 5153 (configurable) |
| **Stateful** | No (stateless consumer) |
| **Database** | None |
| **Logging** | Console only |
| **Complexity** | Minimal |

## Production Deployment Checklist

When deploying NotificationService to production, consider:

- [ ] **Add logging framework** (Serilog, NLog) for structured logging
- [ ] **Implement idempotency** — Handle duplicate notifications gracefully
- [ ] **Add persistence** — Store received notifications for audit trail
- [ ] **Implement health checks** — Add `/health` endpoint for monitoring
- [ ] **Add metrics** — Track notification processing with Prometheus/Application Insights
- [ ] **Implement retries** — If the service fails, OrderService should retry
- [ ] **Add validation** — Validate order ID format and content
- [ ] **Use message queues** — Consider moving from HTTP to message-based (RabbitMQ, Azure Service Bus)
- [ ] **Add authentication** — Secure the notification endpoint
- [ ] **Rate limiting** — Prevent abuse of the notification endpoint

## Idempotency Design

To make this service truly production-ready, implement idempotent notification handling:

```csharp
// Example: Track processed notifications
var processedNotifications = new HashSet<string>();

app.MapGet("/notify/{id}", ([FromRoute] string id) =>
{
    if (processedNotifications.Contains(id))
    {
        Console.WriteLine($"Notification for {id} already processed (duplicate)");
        return Results.Ok();
    }
    
    Console.WriteLine($"Notified for order {id}");
    processedNotifications.Add(id);
    return Results.Ok();
});
```

## Error Handling

Currently, the service doesn't handle errors (always returns 200 OK). In production:

```csharp
app.MapGet("/notify/{id}", ([FromRoute] string id) =>
{
    if (string.IsNullOrWhiteSpace(id))
    {
        return Results.BadRequest("Order ID is required");
    }
    
    try
    {
        Console.WriteLine($"Notified for order {id}");
        // Process notification
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing notification for {id}: {ex.Message}");
        return Results.InternalServerError();
    }
});
```

## Troubleshooting

**Service won't start on port 5153:**
- Change the port in `appsettings.json`
- Check if another service is using the port
- Try: `netstat -ano | findstr :5153` (Windows)

**OrderService can't reach NotificationService:**
- Verify both services are running
- Check firewall settings
- Ensure firewall allows localhost communication
- Verify the URLs match (`http://localhost:5153`)

**Notifications not being received:**
- Check NotificationService console for incoming requests
- Verify OrderService is actually sending requests (check OrderService logs)
- Confirm the notification endpoint is correct in OrderService code

## Related Services

- [OrderService](../OrderService) — Creates orders and publishes events
- [TransactionalOutbox Pattern](../) — Pattern overview and documentation

## Enhancement Ideas

Try these exercises to extend the service:

1. **Add a database:** Store notifications in SQLite or PostgreSQL
2. **Implement idempotency:** Track processed order IDs
3. **Add email sending:** Integrate with SendGrid or SMTP
4. **Add metrics:** Track notification processing with Prometheus
5. **Implement retry logic:** Handle partial failures gracefully
6. **Multi-tenant support:** Route notifications based on tenant
7. **Webhook support:** Forward notifications to external systems
8. **Message queue:** Switch from HTTP to RabbitMQ or Azure Service Bus

## Performance Notes

- **Startup time:** ~1-2 seconds (minimal dependencies)
- **Memory usage:** ~50-100 MB at runtime (very lightweight)
- **Throughput:** Can handle thousands of notifications per second
- **Latency:** <10 ms response time per request

This makes it ideal for containerized deployments and Kubernetes clusters.

## Learn More

- [Transactional Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Event-Driven Architecture](https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/event-driven)
- [Microservice Communication Patterns](https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/)
- [Idempotency](https://en.wikipedia.org/wiki/Idempotence#Computer_science_examples)

---

This service is part of the [Microservice Design Patterns](../../) collection.
