# MTM.Core - Universal Infrastructure

Core infrastructure components for the MTM Universal Framework, providing application startup, configuration management, error handling, and logging services.

## Features

- **Application Startup**: Structured initialization with validation and health checks
- **Configuration Management**: Hierarchical configuration with environment-specific overrides
- **Error Handling**: Centralized error handling with structured logging
- **Logging Services**: File-based logging with network and local fallback
- **Cross-Platform Support**: Compatible with Windows, macOS, Linux, and Android

## Usage

### Application Startup

```csharp
using MTM.UniversalFramework.Core.Startup;

// Initialize application with services
var serviceProvider = UniversalApplicationStartup.InitializeApplication(services, args);
```

### Configuration Service

```csharp
using MTM.UniversalFramework.Core.Configuration;

// Access configuration
var config = serviceProvider.GetRequiredService<IUniversalConfigurationService>();
var connectionString = config.GetConnectionString("DefaultConnection");
```

### Error Handling

```csharp
using MTM.UniversalFramework.Core.ErrorHandling;

// Handle errors centrally
try
{
    // Business logic
}
catch (Exception ex)
{
    await UniversalErrorHandling.HandleErrorAsync(ex, "Operation context");
}
```

## Dependencies

- Microsoft.Extensions.Hosting 9.0.8
- Microsoft.Extensions.Configuration 9.0.8
- Microsoft.Extensions.DependencyInjection 9.0.8
- Microsoft.Extensions.Logging 9.0.8

## License

MIT License - see LICENSE.txt for details.