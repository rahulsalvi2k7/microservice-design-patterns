# OrderService

[![Built with .NET 8](https://img.shields.io/badge/Built%20with-.NET%208-512bd4?style=flat-square)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](../../LICENSE)

The **OrderService** is the primary component in the Transactional Outbox pattern demonstration. It manages orders and maintains a reliable outbox for event publishing.

## Overview

OrderService is a minimal .NET 8 API that demonstrates how to:

- **Create orders** with associated outbox messages
- **Manage message lifecycle** from creation to delivery confirmation
- **Process messages asynchronously** using a background service
- **Track message status** (Waiting, Sent, Failed) for reliability and debugging

The service uses an in-memory outbox for simplicity, but the pattern is designed to work with persistent storage in production environments.

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/) or later
- [NotificationService](../NotificationService) running on `http://localhost:5153`

### Run the Service

```powershell
dotnet run
```

The service will start on `http://localhost:5295` by default.

## API Endpoints

### Create Order

Creates a new order and adds an outbox message for notification.

**Request:**
```http
GET /create/{id}
```

**Parameters:**
- `{id}` — Order identifier (can be any string, e.g., "ORDER-123")

**Response:**
```
Status: 202 Accepted
```

**Example:**
```bash
curl http://localhost:5295/create/ORDER-001
```

**What happens:**
1. A new `Order` is created with the provided ID
2. An `OutboxMessage` is added with status `Waiting`
3. The background service will process it within the next 10 seconds

### Get Outbox Status

Retrieves the current state of all outbox messages.

**Request:**
```http
GET /status
```

**Response:**
```json
[
  {
    "id": "ORDER-001",
    "messageStatus": "Waiting"
  },
  {
    "id": "ORDER-002",
    "messageStatus": "Sent"
  },
  {
    "id": "ORDER-003",
    "messageStatus": "Failed"
  }
]
```

**Example:**
```bash
curl http://localhost:5295/status
```

## Architecture

### Components

#### Models

**Order.cs**
- Simple entity representing an order
- Contains `Id` and `Name` properties
- Identified by a unique key

**OutboxMessage.cs**
- Represents a single event to be published
- Contains `Id` (order identifier) and `MessageStatus`
- Status transitions: `Waiting` → `Sent` or `Waiting` → `Failed`

**MessageStatus.cs**
- Enum with three states:
  - `Waiting` — Ready for processing
  - `Sent` — Successfully delivered
  - `Failed` — Delivery failed

**Outbox.cs**
- Container for all outbox messages
- Provides in-memory collection of `OutboxMessage` instances
- In production, this would be a database table

#### Services

**OutboxProcessingService.cs**
- Background service that runs continuously
- **Polling interval:** Every 10 seconds
- **Processing logic:**
  1. Finds all messages with status `Waiting`
  2. Sends HTTP GET request to NotificationService: `/notify/{id}`
  3. Updates status to `Sent` on success
  4. Updates status to `Failed` on error
  5. Logs all activities to console

### Startup Flow

1. **Program.cs** registers:
   - `HttpClientFactory` for HTTP communication
   - `Outbox` as a singleton (shared instance)
   - `OutboxProcessingService` as a hosted service
   - Order API routes

2. **OutboxProcessingService** starts automatically when the application boots

3. **Endpoints are available** for creating orders and checking status

## Message Processing Flow

```
┌─────────────────┐
│ Order Created   │
│ (Via API Call)  │
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────┐
│ OutboxMessage Added             │
│ Status: Waiting                 │
└────────┬────────────────────────┘
         │
         │ (10-second polling interval)
         ▼
┌─────────────────────────────────┐
│ OutboxProcessingService         │
│ Finds Waiting Messages          │
└────────┬────────────────────────┘
         │
         ▼
┌─────────────────────────────────┐
│ HTTP GET /notify/{id}           │
│ Send to NotificationService     │
└────────┬────────────────────────┘
         │
    ┌────┴────┐
    │          │
    ▼          ▼
  SUCCESS    FAILURE
    │          │
    ▼          ▼
 Status:    Status:
  "Sent"    "Failed"
```

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

### NotificationService Address

The NotificationService URL is hardcoded in `OutboxProcessingService.cs`:

```csharp
notificationHttpClient.BaseAddress = new Uri("http://localhost:5153");
```

To change it, modify this line or use environment variables/configuration.

## Console Output

When running, you'll see messages like:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5295
Order ORDER-001 created
Order ORDER-002 created
no messages waiting...wait for 10s
Notified for order ORDER-001 successfully
no messages waiting...wait for 10s
```

## Testing the Service

### Using REST Client (VS Code)

Create a file `test.http` and use the REST Client extension:

```http
@base = http://localhost:5295

### Create orders
GET {{base}}/create/ORDER-001
Accept: application/json

###

GET {{base}}/create/ORDER-002
Accept: application/json

###

### Check status while messages are processing
GET {{base}}/status
Accept: application/json

###

### Check status again after processing completes
GET {{base}}/status
Accept: application/json
```

### Using PowerShell

```powershell
# Create an order
Invoke-WebRequest -Uri "http://localhost:5295/create/ORDER-123" -Method Get

# Check status
Invoke-WebRequest -Uri "http://localhost:5295/status" -Method Get | ConvertFrom-Json | ConvertTo-Json
```

## Project Structure

```
OrderService/
├── Models/
│   ├── Order.cs                 # Order entity
│   ├── Outbox.cs                # Outbox message container
│   ├── OutboxMessage.cs         # Single event message
│   └── MessageStatus.cs         # Status enum
├── Services/
│   └── OutboxProcessingService.cs # Background processing
├── Extensions/
│   └── EndpointRouteBuilderExtensions.cs # API routes
├── Program.cs                   # Service startup and DI
├── appsettings.json            # Configuration
├── appsettings.Development.json # Dev configuration
├── OrderService.csproj         # Project file
└── OrderService.http           # HTTP test file
```

## Dependencies

- **Microsoft.EntityFrameworkCore.Sqlite** — SQLite database support (for future persistence)
- **Microsoft.EntityFrameworkCore.Design** — EF Core design-time tools
- **Microsoft.AspNetCore** — Web framework (implicit via Web SDK)

## Production Deployment Checklist

When moving this to production:

- [ ] Replace in-memory `Outbox` with database table
- [ ] Use Entity Framework Core or Dapper for persistence
- [ ] Implement idempotence checks in NotificationService
- [ ] Add structured logging (Serilog, Application Insights)
- [ ] Implement retry policy (Polly)
- [ ] Add circuit breaker for NotificationService
- [ ] Configure message TTL and cleanup policies
- [ ] Add monitoring and alerting
- [ ] Implement dead-letter queue for failed messages
- [ ] Use transactional boundaries properly
- [ ] Add comprehensive error handling

## Troubleshooting

**NotificationService connection fails:**
- Verify NotificationService is running: `http://localhost:5153/`
- Check firewall isn't blocking localhost communication
- Review console logs for detailed error messages

**Messages not being processed:**
- Ensure NotificationService is accessible
- Check that `OutboxProcessingService` is running (look for log messages)
- Verify the 10-second polling interval hasn't passed
- Check console output for errors

**Port 5295 already in use:**
- Change the port in `appsettings.json`
- Find what's using the port: `netstat -ano | findstr :5295` (Windows)
- Kill the process or use a different port

## Related Services

- [NotificationService](../NotificationService) — Receives order notifications
- [TransactionalOutbox Pattern](../) — Pattern overview and documentation

## Learn More

- [Transactional Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)
- [.NET 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Background Services in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service)
- [Reliable Messaging](https://www.enterpriseintegrationpatterns.com/patterns/messaging/GuaranteedMessaging.html)

## Next Steps

Try these exercises to deepen your understanding:

1. **Add persistence:** Replace `Outbox` with an Entity Framework Core DbContext
2. **Add retries:** Implement exponential backoff in `OutboxProcessingService`
3. **Monitor messages:** Add a database view to inspect message history
4. **Handle duplicates:** Make NotificationService idempotent
5. **Add tests:** Create unit tests for the processing service

---

This service is part of the [Microservice Design Patterns](../../) collection.
