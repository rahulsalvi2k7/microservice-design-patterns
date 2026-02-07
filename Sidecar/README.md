# Sidecar Pattern

[![.NET](https://img.shields.io/badge/.NET-8.0-512bd4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-latest-239120?style=flat-square&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)

A demonstration of the **Sidecar Design Pattern** where a separate auxiliary service (sidecar) handles cross-cutting concerns like logging without modifying the main application code.

## Overview

The Sidecar pattern deploys auxiliary functionality in a separate process alongside the main application. Instead of embedding cross-cutting concerns directly into your service, you delegate them to a dedicated sidecar that handles the responsibility independently.

This implementation shows how two services (ServiceA and ServiceB) utilize a central logging sidecar (CentralLoggingService) to handle all logging operations without tight coupling or code intrusion.

```
┌─────────────────────────────────────────────────────────────┐
│                         Services                             │
├──────────────────┬──────────────────┬──────────────────────┤
│                  │                  │                      │
│   ServiceA       │   ServiceB       │ CentralLogging       │
│  (Main App)      │  (Main App)      │ Service (Sidecar)    │
│                  │                  │                      │
│  ┌────────────┐  │  ┌────────────┐  │  ┌──────────────┐    │
│  │ICustom     │  │  │ICustom     │  │  │Log File      │    │
│  │Logger      │──┼──│Logger      │──┼──│Handler       │    │
│  │            │  │  │            │  │  │              │    │
│  └────────────┘  │  └────────────┘  │  └──────────────┘    │
│                  │                  │                      │
└──────────────────┴──────────────────┴──────────────────────┘
```

## Key Components

### 1. CentralLoggingService (Sidecar)
The auxiliary service that handles all logging concerns:
- Receives log entries via HTTP POST endpoints
- Writes logs to daily-rotated log files
- Provides `/log` endpoint for direct log submission
- Manages file I/O operations centrally

**Ports**: 5006

### 2. ServiceA (Main Service)
A sample service that demonstrates sidecar integration:
- Uses `ICustomLogger` interface for logging operations
- Sends log entries to the sidecar asynchronously
- Focuses on business logic without logging implementation details
- Provides `/update` endpoint

**Ports**: 5062

### 3. ServiceB (Main Service)
Another sample service showing the same sidecar integration pattern:
- Uses `ICustomLogger` interface for logging operations
- Sends log entries to the sidecar asynchronously
- Maintains separation of concerns
- Provides `/create` endpoint

**Ports**: 5123

## Architecture Diagram

```
Service Request Flow:
┌─────────────┐
│ Client      │
└──────┬──────┘
       │
       ├──→ ServiceA ──┐
       │     (port 5062)│
       │                │
       ├──→ ServiceB ──┐│
       │     (port 5123)││
       │                ││
       │     ┌────────────→ CentralLoggingService (Sidecar)
       │     │          │  (port 5006)
       │     │          │  
       │     │          ├─→ Write to Log File
       │     │          │
       └─────┴──────────┘
```

## Why Use the Sidecar Pattern?

| Challenge | Solution |
|-----------|----------|
| **Cross-cutting Concerns** | Offload logging, monitoring, security to sidecar |
| **Code Reusability** | Avoid duplicating concern logic across services |
| **Independent Scaling** | Scale sidecar independently from main service |
| **Technology Differences** | Sidecar can be written in different language/framework |
| **Loose Coupling** | Services don't depend on concern implementation |
| **Testability** | Easier to mock sidecar interactions in tests |

## Getting Started

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Visual Studio 2022** or VS Code with C# extension (optional)
- **Git** - [Download](https://git-scm.com/downloads)

### Clone the Repository

```bash
git clone https://github.com/rahulsalvi2k7/microservice-design-patterns.git
cd microservice-design-patterns/Sidecar
```

### Build the Projects

From the Sidecar directory:

```bash
dotnet build
```

## Running the Example

### Option 1: Using Visual Studio

1. Open the parent solution `DesignPatterns.sln` in Visual Studio 2022
2. Set `CentralLoggingService`, `ServiceA`, and `ServiceB` as startup projects:
   - Right-click solution → Properties → Startup Project → Select Multiple startup projects
   - Set Action to `Start` for all three projects
3. Press F5 to run all services

### Option 2: Using Command Line

Start each service in a separate terminal:

```bash
# Terminal 1 - Start the Central Logging Sidecar Service
cd CentralLoggingService
dotnet run

# Terminal 2 - Start ServiceA
cd ServiceA
dotnet run

# Terminal 3 - Start ServiceB
cd ServiceB
dotnet run
```

### Option 3: Using Visual Studio Code

Open the Sidecar folder and use the integrated terminal to run each service in separate terminals.

## Testing the Services

### Using REST Client Extension

Open the `.http` files in Visual Studio and execute the requests:

**CentralLoggingService** (`LoggerSidecarService.http`):
```http
POST http://localhost:5006/log
Content-Type: text/plain

Hello from CentralLoggingService
```

**ServiceA** (`ServiceA.http`):
```http
POST http://localhost:5062/update
Content-Type: application/json

{"message": "Update request from ServiceA"}
```

**ServiceB** (`ServiceB.http`):
```http
POST http://localhost:5123/create
Content-Type: application/json

{"message": "Create request from ServiceB"}
```

### Using cURL

```bash
# Log via CentralLoggingService
curl -X POST http://localhost:5006/log \
  -H "Content-Type: text/plain" \
  -d "Test log message"

# Update via ServiceA
curl -X POST http://localhost:5062/update \
  -H "Content-Type: application/json" \
  -d '{"message": "Update from ServiceA"}'

# Create via ServiceB
curl -X POST http://localhost:5123/create \
  -H "Content-Type: application/json" \
  -d '{"message": "Create from ServiceB"}'
```

### Using Postman

1. Create a new POST request
2. URL: `http://localhost:5006/log`
3. Headers: `Content-Type: text/plain`
4. Body (raw): Enter your log message
5. Send

## Project Structure

```
Sidecar/
├── CentralLoggingService/          # Sidecar service for logging
│   ├── Program.cs                  # Main entry point
│   ├── LoggerSidecarService.http   # HTTP test file
│   ├── appsettings.json            # Configuration
│   ├── appsettings.Development.json
│   ├── CentralLoggingService.csproj
│   ├── Properties/
│   │   └── launchSettings.json     # Port configuration
│   └── bin/, obj/
│
├── ServiceA/                        # Main service A (uses sidecar)
│   ├── Program.cs                  # Main entry point
│   ├── ServiceA.http               # HTTP test file
│   ├── appsettings.json            # Configuration
│   ├── appsettings.Development.json
│   ├── ServiceA.csproj
│   ├── Properties/
│   │   └── launchSettings.json     # Port configuration
│   └── bin/, obj/
│
├── ServiceB/                        # Main service B (uses sidecar)
│   ├── Program.cs                  # Main entry point
│   ├── ServiceB.http               # HTTP test file
│   ├── appsettings.json            # Configuration
│   ├── appsettings.Development.json
│   ├── ServiceB.csproj
│   ├── Properties/
│   │   └── launchSettings.json     # Port configuration
│   └── bin/, obj/
│
└── README.md                        # This file
```

## Implementation Details

### ICustomLogger Interface

Both ServiceA and ServiceB use the `ICustomLogger` interface to log messages:

```csharp
public interface ICustomLogger
{
    Task Info(string source, string message);
    Task Error(string source, string message);
    Task Warning(string source, string message);
}
```

### CustomLogger Implementation

The `CustomLogger` class handles the HTTP communication with the sidecar:

```csharp
public class CustomLogger : ICustomLogger
{
    private readonly HttpClient _httpClient;
    
    public async Task Info(string source, string message)
    {
        // Sends to CentralLoggingService via HTTP POST
        await _httpClient.PostAsync("http://localhost:5006/log", 
            new StringContent(message));
    }
}
```

### CentralLoggingService Endpoints

```
POST /log
- Accepts raw text or JSON
- Appends to daily log file
- File format: YYYY-MM-DD.log
```

### Log File Rotation

Logs are automatically rotated by date:
- Log files are named: `2024-02-07.log`, `2024-02-08.log`, etc.
- Each day creates a new log file
- Old logs are preserved for archival

## Configuration

### Port Configuration

Each service has its default port in `Properties/launchSettings.json`:

| Service | Port |
|---------|------|
| CentralLoggingService | 5006 |
| ServiceA | 5062 |
| ServiceB | 5123 |

To change ports, modify the `applicationUrl` in each service's `launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "applicationUrl": "https://localhost:5006;http://localhost:5006"
    }
  }
}
```

### Sidecar URL Configuration

Update the sidecar URL in ServiceA and ServiceB's `Program.cs` if you change the sidecar port:

```csharp
// Current configuration
var sidecarUrl = "http://localhost:5006";
```

## Benefits of This Implementation

✅ **Separation of Concerns** - Logging is completely decoupled from business logic  
✅ **Reusability** - Both ServiceA and ServiceB use the same logging sidecar  
✅ **Independent Scaling** - Can scale the sidecar independently from services  
✅ **Easy Testing** - Can mock the sidecar in unit tests  
✅ **Flexible Technology** - Sidecar could be implemented in a different language  
✅ **Centralized Management** - All logs are managed in one place  

## Troubleshooting

### Port Already in Use

If you get a "port already in use" error:

```bash
# Windows - Find process using port 5006
Get-Process | Where-Object {$_.Name -eq "dotnet"} | Stop-Process -Force

# Or change the port in launchSettings.json
```

### Services Can't Connect to Sidecar

1. Verify all services are running
2. Check the sidecar URL in ServiceA and ServiceB's `Program.cs`
3. Ensure firewall allows localhost connections
4. Check port configuration in `launchSettings.json`

### Log File Permissions Error

If you get a file access error:

1. Ensure the application has write permissions in its directory
2. Run the application with Administrator privileges (Windows)
3. Check if antivirus is blocking file operations

### HttpClient Timeout

If requests timeout:

1. Ensure CentralLoggingService is running
2. Verify network connectivity between services
3. Check if services are on the same network
4. Increase timeout in HttpClient configuration if needed

## Real-World Use Cases

### 1. **Distributed Logging**
- Aggregate logs from multiple services in one place
- Implement centralized log storage

### 2. **Monitoring & Metrics**
- Sidecar collects metrics and sends to monitoring service
- Health checks and performance monitoring

### 3. **Configuration Management**
- Sidecar provides centralized configuration
- Services request configuration from sidecar

### 4. **Secret Management**
- Sidecar handles credential rotation
- Services request secrets from sidecar

### 5. **Traffic Management**
- Sidecar handles rate limiting
- Request routing and load balancing

### 6. **Security & Policy Enforcement**
- Sidecar validates authentication/authorization
- Enforces organizational security policies

## Architecture Patterns Related to Sidecar

- **Container Pattern** - Often used with containers (Docker/Kubernetes)
- **Ambassador Pattern** - Similar, but focuses on network communication
- **Adapter Pattern** - Can be used to adapt legacy services
- **Proxy Pattern** - Sidecar acts as a proxy for cross-cutting concerns

## Resources

### Microsoft Documentation
- [Sidecar Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/sidecar)
- [Ambassador Pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/ambassador)
- [Cross-Cutting Concerns](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

### Cloud-Native Implementation
- [Kubernetes Sidecars](https://kubernetes.io/docs/concepts/workloads/pods/sidecar-containers/)
- [Docker Multi-Container Applications](https://docs.docker.com/compose/)

### Books & Articles
- "Building Event-Driven Microservices" by Adam Bellemare
- "Service Mesh Patterns" - Communication and observability patterns

## Next Steps

1. **Extend the Sidecar**:
   - Add metrics collection
   - Implement structured logging (JSON)
   - Add log level filtering

2. **Containerize the Services**:
   - Create Dockerfile for each service
   - Use Docker Compose to orchestrate
   - Deploy to Kubernetes with sidecar containers

3. **Add More Features**:
   - Implement rate limiting in sidecar
   - Add authentication/authorization checking
   - Implement health checks

4. **Integration with External Services**:
   - Send logs to cloud storage
   - Integrate with log aggregation services (ELK, Splunk)
   - Send metrics to monitoring tools (Prometheus, DataDog)

## Summary

The Sidecar pattern is a powerful approach for handling cross-cutting concerns in microservice architectures. This implementation demonstrates:

- How to delegate logging to a dedicated sidecar service
- How to communicate with a sidecar using HTTP
- How multiple services can share the same sidecar
- How to maintain independence between services and concerns

Use this pattern when you need to add functionality to services without modifying their code or creating tight coupling.
