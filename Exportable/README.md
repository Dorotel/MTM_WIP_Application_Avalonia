# MTM Core Systems - Exportable Package

This folder contains exportable versions of all core systems from the MTM WIP Application that can be integrated into any C# application. These systems are framework-agnostic and follow clean architecture principles.

## ?? Table of Contents

- [Installation Guide](#installation-guide)
- [Available Systems](#available-systems)
- [System Dependencies](#system-dependencies)
- [Implementation Examples](#implementation-examples)
- [Custom Prompts](#custom-prompts)
- [Maintenance Instructions](#maintenance-instructions)

---

## ?? Installation Guide

### Prerequisites
- .NET 8.0 or higher
- C# project (Console, Web API, WPF, Avalonia, etc.)

### Step 1: Copy Core Systems
Copy the required system folders from this `Exportable/` directory to your target project:

```
Your_Project/
??? Models/           # Data models and entities
??? Services/         # Service interfaces and implementations
??? Infrastructure/   # Cross-cutting concerns
??? Configuration/    # Configuration management
??? Extensions/       # Dependency injection setup
```

### Step 2: Install NuGet Dependencies
Install the required NuGet packages:

```powershell
# Core Dependencies (Required for all systems)
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Logging

# Error Handling and Logging
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package MySql.Data

# Validation (Optional)
dotnet add package FluentValidation

# Caching (Optional)
dotnet add package Microsoft.Extensions.Caching.Memory

# Testing (Optional)
dotnet add package xUnit
dotnet add package Moq
dotnet add package FluentAssertions
```

### Step 3: Configure Dependency Injection
In your application startup (Program.cs or equivalent):

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

// Add this line in your service configuration
services.AddMTMCoreServices(configuration);
```

### Step 4: Initialize Configuration
Create `appsettings.json` in your project root:

```json
{
  "Application": {
    "Name": "Your Application Name",
    "Environment": "Development",
    "Theme": "Default",
    "Language": "en-US"
  },
  "ErrorHandling": {
    "EnableFileServerLogging": true,
    "EnableMySqlLogging": false,
    "EnableConsoleLogging": true,
    "FileServerBasePath": "C:\\Logs",
    "MySqlConnectionString": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "File": {
      "Enable": true,
      "Path": "Logs/application.log"
    }
  },
  "Database": {
    "Provider": "MySql",
    "ConnectionString": "Server=localhost;Database=your_db;Uid=user;Pwd=password;"
  }
}
```

### Step 5: Initialize Error Handling
In your application startup:

```csharp
using MTM.Core.Services;

// Initialize error handling system
ErrorHandlingInitializer.Initialize();

// Or for development
ErrorHandlingInitializer.InitializeForDevelopment();
```

---

## ?? Available Systems

### ? Implemented Systems

| System | Files | Description | Dependencies |
|--------|-------|-------------|--------------|
| **Error Handling** | `Services/ErrorHandler/` | Comprehensive error logging and management | MySql.Data |
| **Logging Utility** | `Services/Logging/` | File and database logging utilities | Microsoft.Extensions.Logging |
| **Configuration** | `Configuration/` | Settings management and validation | Microsoft.Extensions.Configuration |

### ?? Missing Core Systems (Placeholders Available)

| Priority | System | Status | Custom Prompt |
|----------|--------|--------|---------------|
| **CRITICAL** | Result Pattern | ? Missing | [Prompt 21](exportable-customprompt.instruction.md#21-implement-result-pattern-system) |
| **CRITICAL** | Data Models | ? Missing | [Prompt 22](exportable-customprompt.instruction.md#22-create-data-models-foundation) |
| **CRITICAL** | Service Layer | ? Missing | [Prompt 25](exportable-customprompt.instruction.md#25-implement-service-layer) |
| **CRITICAL** | Dependency Injection | ? Missing | [Prompt 23](exportable-customprompt.instruction.md#23-setup-dependency-injection-container) |
| **CRITICAL** | Database Service | ? Missing | [Prompt 26](exportable-customprompt.instruction.md#26-create-database-service-layer) |
| **HIGH** | Application State | ? Missing | [Prompt 27](exportable-customprompt.instruction.md#27-setup-application-state-management) |
| **HIGH** | Navigation Service | ? Missing | [Prompt 29](exportable-customprompt.instruction.md#29-create-navigation-service) |
| **HIGH** | Repository Pattern | ? Missing | [Prompt 31](exportable-customprompt.instruction.md#31-setup-repository-pattern) |
| **HIGH** | Validation System | ? Missing | [Prompt 32](exportable-customprompt.instruction.md#32-create-validation-system) |
| **MEDIUM** | Caching Layer | ? Missing | [Prompt 35](exportable-customprompt.instruction.md#35-create-caching-layer) |
| **MEDIUM** | Security Infrastructure | ? Missing | [Prompt 36](exportable-customprompt.instruction.md#36-setup-security-infrastructure) |
| **LOW** | Unit Testing | ? Missing | [Prompt 33](exportable-customprompt.instruction.md#33-create-unit-testing-infrastructure) |

---

## ?? System Dependencies

### Core Systems Dependency Graph
```
Configuration Service ??
                      ?? Error Handling System
Logging Utility ???????

Result Pattern ?? Service Layer ?? Repository Pattern
                      ?
                      ?? Data Models

Dependency Injection ?? All Service Systems

Application State ?? Configuration Service
Navigation Service ?? Application State (optional)

Validation System ?? Data Models
Caching Layer ?? Service Layer
Security Infrastructure ?? All Services
```

### Recommended Implementation Order
1. **Configuration Service** - Foundation for all other systems
2. **Result Pattern** - Error handling pattern for services
3. **Data Models** - Core entities and DTOs
4. **Dependency Injection** - Service container setup
5. **Service Layer** - Business logic implementations
6. **Repository Pattern** - Data access abstraction
7. **Validation System** - Business rule enforcement
8. **Remaining Systems** - Based on application needs

---

## ?? Implementation Examples

### Basic Error Handling
```csharp
try
{
    // Your business logic
    var result = await inventoryService.ProcessOperationAsync(partId, operation);
    if (!result.IsSuccess)
    {
        // Handle business logic failure
        Console.WriteLine($"Operation failed: {result.Error}");
    }
}
catch (Exception ex)
{
    // Log unexpected errors
    Service_ErrorHandler.HandleException(ex, ErrorSeverity.High, 
        controlName: "MainProcessor",
        additionalData: new Dictionary<string, object>
        {
            ["PartId"] = partId,
            ["Operation"] = operation
        });
}
```

### Configuration Usage
```csharp
// Access configuration values
var connectionString = ConfigurationService.GetConnectionString("DefaultConnection");
var logLevel = ConfigurationService.GetValue<string>("Logging:LogLevel:Default");

// Validate configuration
if (!ConfigurationService.ValidateConfiguration())
{
    throw new InvalidOperationException("Configuration validation failed");
}
```

### Service Implementation Pattern
```csharp
public class InventoryService : IInventoryService
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IDatabaseService databaseService, ILogger<InventoryService> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    public async Task<Result<InventoryItem>> GetInventoryItemAsync(string partId)
    {
        try
        {
            _logger.LogInformation("Retrieving inventory item {PartId}", partId);
            
            var item = await _databaseService.QuerySingleAsync<InventoryItem>(
                "SELECT * FROM inventory WHERE PartId = @PartId",
                new { PartId = partId });

            return item != null 
                ? Result<InventoryItem>.Success(item)
                : Result<InventoryItem>.Failure($"Inventory item {partId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve inventory item {PartId}", partId);
            return Result<InventoryItem>.Failure($"Database error: {ex.Message}");
        }
    }
}
```

---

## ?? Custom Prompts

All missing systems have corresponding custom prompts for implementation. See [exportable-customprompt.instruction.md](exportable-customprompt.instruction.md) for detailed prompts.

### Quick Start Prompts
- **Foundation Setup**: Prompts 21-24 (Result Pattern, Data Models, DI, Core Services)
- **Service Implementation**: Prompts 25-28 (Service Layer, Database, State, Configuration)
- **Infrastructure**: Prompts 29-32 (Navigation, Theme, Repository, Validation)
- **Quality Assurance**: Prompts 33-36 (Testing, Logging, Caching, Security)

---

## ?? Maintenance Instructions

### When Adding/Updating Systems

1. **Update This README**: Add new system to the "Available Systems" table
2. **Update Custom Prompts**: Add corresponding prompt in `exportable-customprompt.instruction.md`
3. **Update Dependencies**: Modify NuGet package list if needed
4. **Update Examples**: Add usage examples for new systems
5. **Version Documentation**: Update system compatibility information

### Synchronization with Main Project

When systems are updated in the main MTM WIP Application:

1. **Copy Updated Files**: Replace corresponding files in Exportable folder
2. **Remove Framework Dependencies**: Strip out Avalonia/ReactiveUI specific code
3. **Update Documentation**: Reflect changes in README and custom prompts
4. **Test Integration**: Verify systems work in standalone scenarios
5. **Update Version**: Increment version numbers if applicable

### Quality Assurance

Before releasing exportable systems:

1. **Compile Check**: Ensure all systems compile independently
2. **Dependency Verification**: Confirm NuGet dependencies are minimal
3. **Framework Agnostic**: Remove UI framework specific code
4. **Documentation Update**: Ensure all documentation is current
5. **Example Testing**: Verify all code examples work correctly

---

## ?? Version History

| Version | Date | Changes | Systems Added |
|---------|------|---------|---------------|
| 1.0.0 | 2025-01-27 | Initial exportable package | Error Handling, Logging, Configuration |

---

## ?? Support

For questions about integrating these systems:

1. Check the [Custom Prompts](exportable-customprompt.instruction.md) for implementation guidance
2. Review the implementation examples above
3. Refer to the original MTM WIP Application for reference implementations
4. Contact the project maintainer for assistance

---

**Note**: This package is maintained in sync with the MTM WIP Application project. When systems are updated in the main project, corresponding updates should be made to this exportable package following the maintenance instructions above.