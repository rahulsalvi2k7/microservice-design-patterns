# Microservice Design Patterns

A compact collection of .NET sample projects demonstrating common microservice design patterns: aggregation, circuit breaker, distributed tracing, event sourcing, rate limiting, saga orchestration, service discovery, sidecar, and transactional outbox.

> [!note]
> These samples target .NET 8.0. Use the `dotnet` CLI to run each sample project.

## Quickstart

Prerequisites:

- Install the .NET 8 SDK: https://dotnet.microsoft.com/
- A terminal (PowerShell on Windows is used in examples below)

To run a sample, open a terminal and `cd` to the sample folder, then run:

```powershell
cd <sample-folder>
dotnet run
```

Example — run the Saga orchestration orchestrator:

```powershell
cd SagaOrchestration/orchestratorService
dotnet run
```

## Projects (overview)

- **Aggregator/** — Example of an API aggregator that composes responses from `OrderHeader` and `OrderItems` services.
- **CircuitBreaker/** — Sample services and a switch demonstrating circuit-breaker and fallback behaviors.
- **DistributedTracing/** — Minimal services instrumented for distributed tracing and centralized logging.
- **EventSourcing/** — Event-sourcing sample with a small domain API, event store library, and a simple client UI.
- **RateLimiter/** — Tenant-aware rate limiting middleware and a small rate-limiter service.
- **SagaOrchestration/** — Orchestrator pattern example with `orchestratorService`, `orderService`, and `paymentService` (see below).
- **ServiceDiscovery/** — Examples showing simple service registry and services that register/discover.
- **Sidecar/** — Demonstrates a sidecar pattern with central logging and example services.
- **TransactionalOutbox/** — Example showing the transactional outbox pattern to reliably publish events.

See the solution file [DesignPatterns.sln](DesignPatterns.sln) for the full set of projects.

## SagaOrchestration example

This folder contains a small orchestrator + service pair that demonstrates a basic saga flow:

- `SagaOrchestration/orchestratorService` — the orchestrator that coordinates the order saga.
- `SagaOrchestration/orderService` — service that handles order commands.
- `SagaOrchestration/paymentService` — service that handles payments.

Run them in separate terminals (order matters only if you want the orchestrator to call an already-running service):

```powershell
# Terminal 1
cd SagaOrchestration/orderService
dotnet run

# Terminal 2
cd SagaOrchestration/paymentService
dotnet run

# Terminal 3
cd SagaOrchestration/orchestratorService
dotnet run
```

The orchestrator exposes endpoints described in `orchestratorService.http` for local testing using HTTP clients or the .http files in each project.

## EventSourcing example

This folder contains a small event-sourcing sample that stores order domain events in memory and replays them to rebuild the current state:

- `EventSourcing/Domain.api` — minimal API that creates, ships, and receives orders and exposes hydrate/replay endpoints.
- `EventSourcing/DomainEvent.Lib` — shared event and in-memory event-store implementation.
- `EventSourcing/Domain.Client` — simple client UI for exploring the sample.

Run the API from the repository root:

```powershell
cd EventSourcing/Domain.api
dotnet run
```

Then open the `Domain.Client/index.html` page in a browser or use the `.http` file in `Domain.api` to exercise the endpoints.

## Running other samples

Each sample folder contains a `.http` file and `Program.cs` with minimal configuration. Typical steps:

```powershell
cd Aggregator/OrderAggregator
dotnet run
```

If a sample depends on multiple projects, start each required service in its own terminal.

## Development notes

- Configuration is provided via `appsettings.json` and `appsettings.Development.json` in each project.
- Most projects use minimal APIs and can be inspected/modified quickly; use `dotnet build` to verify compilation.
- The `.http` files included in many folders are convenient for VS Code REST client or JetBrains HTTP client testing.

## Tests & automation

Some folders include test projects (e.g., under `Sidecar`). Use the `dotnet test` command inside the test project folder.

## What's next

- Run a sample you care about and open any `.http` file in an editor to exercise endpoints.
- If you want, I can add step-by-step walkthroughs for a specific pattern (e.g., Saga or Circuit Breaker).

---

README created to document the collection of microservice pattern samples. For details about a specific project, open its folder and review `Program.cs` and the included `.http` test files.
# Microservice Design Patterns

[![Build Status](https://img.shields.io/badge/Built%20with-.NET%208-512bd4?style=flat-square)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)](LICENSE)
![C#](https://img.shields.io/badge/C%23-latest-239120?style=flat-square&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512bd4?style=flat-square&logo=dotnet&logoColor=white)

A comprehensive repository demonstrating real-world implementations of common microservice design patterns using .NET and ASP.NET Core. Learn how to build scalable, resilient, and maintainable microservice architectures through practical examples.

> [!TIP]
> Each pattern directory contains a complete working example with sample services that you can run, study, and adapt to your own projects.

## Table of Contents

- [Overview](#overview)
- [Patterns Included](#patterns-included)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Pattern Descriptions](#pattern-descriptions)
- [Project Structure](#project-structure)
- [Running the Examples](#running-the-examples)
- [Resources](#resources)
- [Troubleshooting](#troubleshooting)
- [Issues & Help](#issues--help)

## Overview

This project serves as a learning resource for understanding and implementing critical microservice design patterns. Each pattern is implemented with practical examples that demonstrate real-world scenarios and best practices.

Microservices architecture breaks down large applications into smaller, independent services that communicate over the network. However, this distributed nature introduces complexity. These patterns help address common challenges like service coordination, resilience, observability, and scalability.

## Patterns Included

### 1. **Aggregator** ⚙️
Combines data from multiple microservices into a single response. The aggregator service acts as a facade that calls multiple backend services and aggregates their responses before returning to the client.

- **Use Case**: Creating composite API responses
- **Services**: OrderAggregator, OrderHeader, OrderItems

### 2. **Circuit Breaker** 🔌
Prevents cascading failures by monitoring for failures and temporarily blocking requests to a failing service. Once the service recovers, requests are gradually resumed.

- **Use Case**: Building fault-tolerant distributed systems
- **Services**: OrderService, PaymentService, SwitchService

### 3. **Distributed Tracing** 📊
Tracks requests as they flow through multiple services, providing visibility into the entire request lifecycle and helping identify performance bottlenecks.

- **Use Case**: Debugging and monitoring microservice interactions
- **Services**: OrderService, PaymentService

### 4. **Rate Limiter** ⏱️
Controls the rate at which clients can make requests to prevent abuse and ensure fair resource allocation. Implements per-tenant rate limiting.

- **Use Case**: API protection and throttling
- **Service**: RateLimiterService

### 5. **Saga Orchestration** 🎼
Manages distributed transactions across multiple services using a centralized orchestrator that coordinates the transaction steps.

- **Use Case**: Handling multi-service workflows (e.g., order processing with payment and inventory)
- **Services**: OrchestratorService, OrderService, PaymentService

### 6. **Service Discovery** 🔍
Automatically detects and registers available service instances, enabling dynamic service lookup without hardcoded addresses.

- **Use Case**: Managing service location in dynamic environments
- **Services**: ServiceRegistry, OrderService, PaymentService

### 7. **Sidecar** 🚀
Deploys auxiliary logic in a separate process alongside the main application service, handling cross-cutting concerns without modifying the service code.

- **Use Case**: Adding observability, configuration, or communication logic
- **Services**: CentralLoggingService, ServiceA, ServiceB

### 8. **Event Sourcing** 🧾
Records domain state changes as immutable events and replays them to reconstruct current state. This sample demonstrates event creation, hydration, and replay using an in-memory event store.

- **Use Case**: Auditing, debugging, and rebuilding aggregate state from history
- **Services**: Domain.api, DomainEvent.Lib, Domain.Client

### 9. **Transactional Outbox** 📮
Ensures reliable message publishing by storing messages in a database transaction alongside business data, then publishing them asynchronously.

- **Use Case**: Maintaining consistency between database state and external message systems
- **Services**: NotificationService, OrderService

## Prerequisites

- **.NET 8 SDK** or later - [Download](https://dotnet.microsoft.com/download)
- **Visual Studio 2022** (or VS Code with C# extension) - Optional but recommended
- **Git** - [Download](https://git-scm.com/downloads)

> [!NOTE]
> For the Distributed Tracing pattern, you may want to have tools like Jaeger or Zipkin running locally to visualize traces.

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/rahulsalvi2k7/microservice-design-patterns.git
cd microservice-design-patterns
```

### Open in Visual Studio

Open the `DesignPatterns.sln` solution file in Visual Studio 2022:

```bash
start DesignPatterns.sln
```

### Or with .NET CLI

Build the entire solution:

```bash
dotnet build
```

## Pattern Descriptions

### Aggregator Pattern

The aggregator service acts as a facade that calls multiple backend services and combines their responses into a single payload.

**Files**: `Aggregator/OrderAggregator/`

```csharp
// Aggregates data from OrderHeader and OrderItems services
app.MapGet("/aggregate/{id}", async (int id, HeaderClient headerClient, ItemsClient itemsClient) =>
{
    var headerTask = headerClient.GetHeaderAsync(id);
    var itemsTask = itemsClient.GetItemsAsync(id);
    
    await Task.WhenAll(headerTask, itemsTask);
});
```

### Circuit Breaker Pattern

Prevents cascading failures through a switch mechanism that monitors the health of dependent services.

**Files**: `CircuitBreaker/`

The pattern monitors requests to dependent services and automatically opens a circuit when failure threshold is reached, failing fast instead of wasting resources.

### Distributed Tracing Pattern

Provides end-to-end visibility into request flows across multiple services.

**Files**: `DistributedTracing/`

Each service logs trace information that can be correlated using trace IDs and viewed in a centralized monitoring system.

### Rate Limiter Pattern

Controls how many requests can be processed per tenant/client within a specified time window.

**Files**: `RateLimiter/RateLimiterService/`

Implements middleware-based rate limiting with per-tenant quotas and sliding window algorithms.

### Saga Orchestration Pattern

Coordinates distributed transactions by having a central orchestrator service manage the transaction workflow.

**Files**: `SagaOrchestration/orchestratorService/`

The orchestrator subscribes to events from services and sends commands to coordinate the workflow.

### Service Discovery Pattern

Dynamically registers and discovers service instances without hardcoding endpoints.

**Files**: `ServiceDiscovery/ServiceRegistry/`

Services register their endpoints at startup, and clients query the registry to discover available instances.

### Sidecar Pattern

Deploys a separate container/process alongside the main service to handle cross-cutting concerns.

**Files**: `Sidecar/`

The sidecar intercepts traffic, logs, and provides utilities without modifying the main service code.

### Transactional Outbox Pattern

Ensures messages are published reliably by storing them alongside business data in a database transaction.

**Files**: `TransactionalOutbox/`

A background worker periodically publishes messages from the outbox table to external systems.

## Project Structure

```
microservice-design-patterns/
├── Aggregator/              # Aggregator pattern implementation
│   ├── OrderAggregator/
│   ├── OrderHeader/
│   └── OrderItems/
├── CircuitBreaker/          # Circuit Breaker pattern implementation
│   ├── OrderService/
│   ├── PaymentService/
│   └── SwitchService/
├── DistributedTracing/      # Distributed Tracing pattern implementation
│   ├── OrderService/
│   └── PaymentService/
├── EventSourcing/            # Event Sourcing pattern implementation
│   ├── Domain.api/
│   ├── Domain.Client/
│   └── DomainEvent.Lib/
├── RateLimiter/             # Rate Limiter pattern implementation
│   └── RateLimiterService/
├── SagaOrchestration/       # Saga Orchestration pattern implementation
│   ├── orchestratorService/
│   ├── orderService/
│   └── paymentService/
├── ServiceDiscovery/        # Service Discovery pattern implementation
│   ├── ServiceRegistry/
│   ├── OrderService/
│   └── PaymentService/
├── Sidecar/                 # Sidecar pattern implementation
│   ├── CentralLoggingService/
│   ├── ServiceA/
│   └── ServiceB/
├── TransactionalOutbox/     # Transactional Outbox pattern implementation
│   ├── NotificationService/
│   └── OrderService/
└── DesignPatterns.sln       # Solution file
```

## Running the Examples

### Build All Projects

From the repository root:

```bash
dotnet build
```

### Run a Specific Pattern

Each pattern can be run independently. For example, to run the Aggregator pattern:

```bash
cd Aggregator/OrderAggregator
dotnet run
```

Open `OrderAggregator.http` in Visual Studio to test the endpoints using the REST Client extension.

### Running Multiple Services

Some patterns require multiple services running simultaneously. Start each service in a separate terminal:

```bash
# Terminal 1
cd Aggregator/OrderHeader
dotnet run

# Terminal 2
cd Aggregator/OrderItems
dotnet run

# Terminal 3
cd Aggregator/OrderAggregator
dotnet run
```

> [!IMPORTANT]
> Make sure ports don't conflict. Each service typically runs on a different port (check `appsettings.Development.json` for port configuration).

## Resources

### Microsoft Learn
- [Microservices Architecture Style](https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/microservices)
- [Design Patterns for Microservices](https://learn.microsoft.com/en-us/azure/architecture/microservices/design/patterns)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)

### Books & Articles
- "Building Microservices" by Sam Newman
- "Microservices Patterns" by Chris Richardson
- "Release It!" by Michael T. Nygard (Circuit Breaker pattern)

### Related Patterns
- [API Gateway Pattern](https://learn.microsoft.com/en-us/azure/architecture/microservices/design/gateway)
- [Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
- [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)

## Troubleshooting

### Port Already in Use

If you get an error that a port is already in use:

```
A connection attempt failed because the connected party did not properly respond
```

**Solution**: Change the port in `Properties/launchSettings.json` or `appsettings.Development.json`:

```json
"applicationUrl": "https://localhost:5001;http://localhost:5000"
```

### Service Discovery Not Finding Services

Ensure all services are running and properly registered:

1. Verify the ServiceRegistry is running first
2. Check the service endpoints in `appsettings.json`
3. Verify network connectivity between services

### HTTP Client Timeout

If services timeout when calling each other:

1. Ensure all required services are running on the expected ports
2. Check firewall settings allowing inter-service communication
3. Increase timeout in HttpClient configuration if needed

### Database Connection Issues

For patterns using database (like Transactional Outbox):

1. Ensure SQL Server is running locally or the connection string is correct
2. Run migrations if needed: `dotnet ef database update`

## Issues & Help

### Have a Question?

- **GitHub Issues**: [Open an issue](https://github.com/rahulsalvi2k7/microservice-design-patterns/issues) for bugs and feature requests
- **Discussions**: Use GitHub Discussions for questions and general help
- **Code Examples**: Each pattern includes `.http` files for testing with REST Client extension

### Contributing

Contributions are welcome! Feel free to:

- Report bugs and suggest enhancements
- Improve documentation
- Add new pattern examples
- Fix or improve existing implementations

### Resources

- [Official Microservices Patterns Catalog](https://microservices.io/patterns/index.html)
- [Azure Architecture Center - Microservices](https://learn.microsoft.com/en-us/azure/architecture/)
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
