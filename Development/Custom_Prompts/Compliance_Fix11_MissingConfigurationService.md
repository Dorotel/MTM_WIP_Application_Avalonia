# Custom Prompt: Create Missing Configuration Service

## ⚠️ MEDIUM PRIORITY FIX #11

**Issue**: While appsettings.json exists, there's no configuration service to read and provide settings to the application.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- `Config/appsettings.json` exists but unused
- Missing `Services/IConfigurationService.cs`
- Hard-coded connection strings in Model_AppVariables
- No environment-specific configuration

**Priority**: ⚠️ **MEDIUM - CONFIGURATION MANAGEMENT**

---

## Custom Prompt

```
MEDIUM PRIORITY INFRASTRUCTURE: Create comprehensive configuration service to centralize application settings management, replace hard-coded values, and enable environment-specific configuration.

REQUIREMENTS:
1. Create IConfigurationService interface for settings access
2. Implement ConfigurationService using Microsoft.Extensions.Configuration
3. Load appsettings.json and environment-specific files
4. Provide strongly-typed configuration access
5. Replace hard-coded values with configuration service calls
6. Support environment-specific overrides
7. Enable configuration validation and error handling

CONFIGURATION SERVICE INTERFACE:

**Services/IConfigurationService.cs**:
```csharp
using System.Collections.Generic;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IConfigurationService
{
    // Connection Strings
    string GetConnectionString(string name = "DefaultConnection");
    string GetDatabaseConnectionString();
    
    // Application Settings
    T GetValue<T>(string key, T defaultValue = default!);
    string GetValue(string key, string defaultValue = "");
    bool GetBoolValue(string key, bool defaultValue = false);
    int GetIntValue(string key, int defaultValue = 0);
    double GetDoubleValue(string key, double defaultValue = 0.0);
    
    // Section-based Configuration
    T GetSection<T>(string sectionName) where T : class, new();
    IEnumerable<T> GetSectionArray<T>(string sectionName) where T : class, new();
    
    // Environment and Application Info
    string Environment { get; }
    string ApplicationName { get; }
    string ApplicationVersion { get; }
    
    // Configuration Validation
    bool ValidateConfiguration();
    IEnumerable<string> GetConfigurationErrors();
    
    // Configuration Refresh
    void RefreshConfiguration();
    event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;
}

public class ConfigurationChangedEventArgs : EventArgs
{
    public string Key { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
```

CONFIGURATION SERVICE IMPLEMENTATION:

**Services/ConfigurationService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;
    private readonly string _environment;
    private readonly List<string> _configurationErrors = new();

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _environment = System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") 
                      ?? System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                      ?? "Production";

        _logger.LogInformation("Configuration service initialized for environment: {Environment}", _environment);
        
        ValidateConfiguration();
    }

    public string Environment => _environment;
    
    public string ApplicationName => GetValue("Application:Name", "MTM WIP Application");
    
    public string ApplicationVersion => 
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

    // Connection Strings
    public string GetConnectionString(string name = "DefaultConnection")
    {
        var connectionString = _configuration.GetConnectionString(name);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Connection string '{Name}' not found in configuration", name);
            
            // Fallback to legacy Model_AppVariables if available
            if (name == "DefaultConnection" && !string.IsNullOrEmpty(Model_AppVariables.ConnectionString))
            {
                _logger.LogInformation("Using fallback connection string from Model_AppVariables");
                return Model_AppVariables.ConnectionString;
            }
            
            throw new InvalidOperationException($"Connection string '{name}' not found in configuration");
        }
        
        _logger.LogDebug("Retrieved connection string for '{Name}'", name);
        return connectionString;
    }

    public string GetDatabaseConnectionString()
    {
        return GetConnectionString("DefaultConnection");
    }

    // Generic Value Access
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        try
        {
            var value = _configuration[key];
            
            if (value == null)
            {
                _logger.LogDebug("Configuration key '{Key}' not found, using default value: {DefaultValue}", 
                    key, defaultValue);
                return defaultValue;
            }

            // Handle string type directly
            if (typeof(T) == typeof(string))
                return (T)(object)value;

            // Handle nullable types
            var targetType = typeof(T);
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = Nullable.GetUnderlyingType(targetType)!;
            }

            // Convert value to target type
            var convertedValue = Convert.ChangeType(value, targetType);
            return (T)convertedValue;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading configuration key '{Key}', using default value: {DefaultValue}", 
                key, defaultValue);
            return defaultValue;
        }
    }

    public string GetValue(string key, string defaultValue = "")
    {
        return GetValue<string>(key, defaultValue);
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        return GetValue<bool>(key, defaultValue);
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        return GetValue<int>(key, defaultValue);
    }

    public double GetDoubleValue(string key, double defaultValue = 0.0)
    {
        return GetValue<double>(key, defaultValue);
    }

    // Section-based Configuration
    public T GetSection<T>(string sectionName) where T : class, new()
    {
        try
        {
            var section = _configuration.GetSection(sectionName);
            var result = new T();
            section.Bind(result);
            
            _logger.LogDebug("Retrieved configuration section '{SectionName}'", sectionName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading configuration section '{SectionName}'", sectionName);
            return new T();
        }
    }

    public IEnumerable<T> GetSectionArray<T>(string sectionName) where T : class, new()
    {
        try
        {
            var section = _configuration.GetSection(sectionName);
            var results = new List<T>();
            section.Bind(results);
            
            _logger.LogDebug("Retrieved configuration section array '{SectionName}' with {Count} items", 
                sectionName, results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading configuration section array '{SectionName}'", sectionName);
            return Enumerable.Empty<T>();
        }
    }

    // Configuration Validation
    public bool ValidateConfiguration()
    {
        _configurationErrors.Clear();
        
        try
        {
            // Validate connection strings
            var defaultConnection = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(defaultConnection) && string.IsNullOrEmpty(Model_AppVariables.ConnectionString))
            {
                _configurationErrors.Add("No default connection string found in configuration or Model_AppVariables");
            }

            // Validate application settings
            var appName = GetValue("Application:Name", "");
            if (string.IsNullOrEmpty(appName))
            {
                _configurationErrors.Add("Application:Name is not configured");
            }

            // Validate logging configuration
            var logLevel = GetValue("Logging:LogLevel:Default", "");
            if (string.IsNullOrEmpty(logLevel))
            {
                _configurationErrors.Add("Logging:LogLevel:Default is not configured");
            }

            // Validate MTM-specific settings
            ValidateMTMSettings();

            if (_configurationErrors.Any())
            {
                _logger.LogWarning("Configuration validation found {ErrorCount} issues", _configurationErrors.Count);
                foreach (var error in _configurationErrors)
                {
                    _logger.LogWarning("Configuration error: {Error}", error);
                }
                return false;
            }

            _logger.LogInformation("Configuration validation passed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during configuration validation");
            _configurationErrors.Add($"Configuration validation exception: {ex.Message}");
            return false;
        }
    }

    private void ValidateMTMSettings()
    {
        // Validate MTM-specific configuration sections
        var databaseSettings = GetSection<DatabaseSettings>("Database");
        if (databaseSettings.CommandTimeout <= 0)
        {
            _configurationErrors.Add("Database:CommandTimeout must be greater than 0");
        }

        var inventorySettings = GetSection<InventorySettings>("Inventory");
        if (inventorySettings.DefaultLocation == null)
        {
            _configurationErrors.Add("Inventory:DefaultLocation is not configured");
        }

        var userSettings = GetSection<UserSettings>("User");
        if (userSettings.SessionTimeout <= 0)
        {
            _configurationErrors.Add("User:SessionTimeout must be greater than 0");
        }
    }

    public IEnumerable<string> GetConfigurationErrors()
    {
        return _configurationErrors.AsReadOnly();
    }

    // Configuration Refresh
    public void RefreshConfiguration()
    {
        try
        {
            // If using reloadable configuration, trigger reload
            if (_configuration is IConfigurationRoot configRoot)
            {
                configRoot.Reload();
                _logger.LogInformation("Configuration refreshed");
                
                // Re-validate after refresh
                ValidateConfiguration();
                
                // Trigger configuration changed event
                ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs 
                { 
                    Key = "Configuration", 
                    OldValue = "Previous", 
                    NewValue = "Refreshed" 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing configuration");
        }
    }
}
```

CONFIGURATION MODELS:

**Models/Configuration/DatabaseSettings.cs**:
```csharp
namespace MTM_WIP_Application_Avalonia.Models.Configuration;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public int ConnectionTimeout { get; set; } = 15;
    public bool EnableRetryLogic { get; set; } = true;
    public int MaxRetryAttempts { get; set; } = 3;
    public bool LogSqlCommands { get; set; } = false;
}
```

**Models/Configuration/InventorySettings.cs**:
```csharp
namespace MTM_WIP_Application_Avalonia.Models.Configuration;

public class InventorySettings
{
    public string DefaultLocation { get; set; } = string.Empty;
    public string DefaultOperation { get; set; } = "90";
    public int MaxQuantityPerTransaction { get; set; } = 10000;
    public int MinStockWarningLevel { get; set; } = 10;
    public bool AllowNegativeInventory { get; set; } = false;
    public bool RequireLocationValidation { get; set; } = true;
    public bool RequireOperationValidation { get; set; } = true;
}
```

**Models/Configuration/UserSettings.cs**:
```csharp
namespace MTM_WIP_Application_Avalonia.Models.Configuration;

public class UserSettings
{
    public int SessionTimeout { get; set; } = 480; // 8 hours in minutes
    public bool RememberLastUser { get; set; } = true;
    public bool RequireUserValidation { get; set; } = true;
    public string DefaultUser { get; set; } = string.Empty;
}
```

**Models/Configuration/ApplicationSettings.cs**:
```csharp
namespace MTM_WIP_Application_Avalonia.Models.Configuration;

public class ApplicationSettings
{
    public string Name { get; set; } = "MTM WIP Application";
    public string Version { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Production";
    public bool EnableDiagnostics { get; set; } = false;
    public string LogLevel { get; set; } = "Information";
    public string DataDirectory { get; set; } = "Data";
    public string BackupDirectory { get; set; } = "Backups";
}
```

UPDATED APPSETTINGS.JSON:

**Config/appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MTM_WIP;Uid=mtm_user;Pwd=your_password_here;"
  },
  "Application": {
    "Name": "MTM WIP Application",
    "Version": "1.0.0",
    "Environment": "Production",
    "EnableDiagnostics": false,
    "DataDirectory": "Data",
    "BackupDirectory": "Backups"
  },
  "Database": {
    "CommandTimeout": 30,
    "ConnectionTimeout": 15,
    "EnableRetryLogic": true,
    "MaxRetryAttempts": 3,
    "LogSqlCommands": false
  },
  "Inventory": {
    "DefaultLocation": "WC001",
    "DefaultOperation": "90",
    "MaxQuantityPerTransaction": 10000,
    "MinStockWarningLevel": 10,
    "AllowNegativeInventory": false,
    "RequireLocationValidation": true,
    "RequireOperationValidation": true
  },
  "User": {
    "SessionTimeout": 480,
    "RememberLastUser": true,
    "RequireUserValidation": true,
    "DefaultUser": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "MTM_WIP_Application_Avalonia": "Debug"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
    },
    "File": {
      "Path": "Logs/mtm-wip-{Date}.log",
      "RollingInterval": "Day",
      "RetainedFileCountLimit": 30,
      "FileSizeLimitBytes": 10485760
    }
  }
}
```

**Config/appsettings.Development.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MTM_WIP_DEV;Uid=mtm_user;Pwd=dev_password_here;"
  },
  "Application": {
    "Environment": "Development",
    "EnableDiagnostics": true
  },
  "Database": {
    "LogSqlCommands": true,
    "CommandTimeout": 120
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "MTM_WIP_Application_Avalonia": "Trace"
    }
  }
}
```

**Config/appsettings.Testing.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MTM_WIP_TEST;Uid=mtm_user;Pwd=test_password_here;"
  },
  "Application": {
    "Environment": "Testing",
    "EnableDiagnostics": true
  },
  "Database": {
    "LogSqlCommands": true
  },
  "User": {
    "SessionTimeout": 60,
    "RequireUserValidation": false,
    "DefaultUser": "TEST_USER"
  }
}
```

UPDATE MODEL_APPVARIABLES TO USE CONFIGURATION:

**Database_Engine/Model_AppVariables.cs** (Updated):
```csharp
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Database_Engine;

public static class Model_AppVariables
{
    private static IConfigurationService? _configurationService;
    
    public static void Initialize(IConfigurationService configurationService)
    {
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
    }

    // Legacy property maintained for backward compatibility
    public static string ConnectionString
    {
        get
        {
            if (_configurationService != null)
            {
                return _configurationService.GetDatabaseConnectionString();
            }
            
            // Fallback to hard-coded value if configuration not initialized
            return "Server=localhost;Database=MTM_WIP;Uid=mtm_user;Pwd=password123;";
        }
    }

    // New configuration-based properties
    public static string DefaultLocation => _configurationService?.GetValue("Inventory:DefaultLocation", "WC001") ?? "WC001";
    public static string DefaultOperation => _configurationService?.GetValue("Inventory:DefaultOperation", "90") ?? "90";
    public static int MaxQuantityPerTransaction => _configurationService?.GetIntValue("Inventory:MaxQuantityPerTransaction", 10000) ?? 10000;
    public static int SessionTimeout => _configurationService?.GetIntValue("User:SessionTimeout", 480) ?? 480;
    public static bool EnableDiagnostics => _configurationService?.GetBoolValue("Application:EnableDiagnostics", false) ?? false;
}
```

PROGRAM.CS INTEGRATION:

**Program.cs** (Updated ConfigureServices):
```csharp
private static void ConfigureServices()
{
    var services = new ServiceCollection();

    // Configuration setup with environment support
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
    
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"Config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    // Configuration Service
    services.AddSingleton<IConfigurationService, ConfigurationService>();

    // Initialize Model_AppVariables after configuration service is built
    var serviceProvider = services.BuildServiceProvider();
    var configService = serviceProvider.GetRequiredService<IConfigurationService>();
    Model_AppVariables.Initialize(configService);

    // Rest of service registration...
}
```

SERVICE USAGE EXAMPLE:

**Services/DatabaseService.cs** (Updated to use Configuration):
```csharp
public class DatabaseService : IDatabaseService
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfigurationService configurationService, ILogger<DatabaseService> logger)
    {
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<DataTable>> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters)
    {
        try
        {
            var connectionString = _configurationService.GetDatabaseConnectionString();
            var commandTimeout = _configurationService.GetIntValue("Database:CommandTimeout", 30);
            
            _logger.LogDebug("Executing stored procedure: {ProcedureName} with timeout: {Timeout}s", 
                procedureName, commandTimeout);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                procedureName,
                parameters ?? new Dictionary<string, object>(),
                commandTimeout
            );

            // Rest of implementation...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {ProcedureName}", procedureName);
            return Result<DataTable>.Failure($"Database error: {ex.Message}");
        }
    }
}
```

After implementing configuration service, create Development/Services/README_ConfigurationService.md documenting:
- Configuration service usage patterns
- Environment-specific configuration setup
- Configuration validation procedures
- Hard-coded value replacement guidelines
- Configuration model creation
- Error handling strategies
```

---

## Expected Deliverables

1. **IConfigurationService interface** with comprehensive configuration access methods
2. **ConfigurationService implementation** using Microsoft.Extensions.Configuration
3. **Configuration models** for strongly-typed sections (Database, Inventory, User, Application)
4. **Updated appsettings.json** with complete MTM application settings
5. **Environment-specific configuration files** (Development, Testing)
6. **Updated Model_AppVariables** to use configuration service
7. **Configuration validation** with error reporting
8. **Service integration examples** showing configuration usage
9. **Environment variable support** for deployment flexibility
10. **Configuration refresh capability** for runtime updates

---

## Validation Steps

1. Verify configuration service reads from appsettings.json correctly
2. Test environment-specific configuration overrides work
3. Confirm strongly-typed configuration models bind properly
4. Validate configuration validation catches missing/invalid settings
5. Test fallback to Model_AppVariables for backward compatibility
6. Verify all hard-coded values are replaced with configuration calls
7. Test configuration refresh functionality

---

## Success Criteria

- [ ] Configuration service provides centralized settings access
- [ ] Environment-specific configuration files work correctly
- [ ] Strongly-typed configuration models implemented
- [ ] Configuration validation identifies missing/invalid settings
- [ ] Hard-coded values replaced with configuration service calls
- [ ] Backward compatibility maintained with Model_AppVariables
- [ ] Error handling covers configuration access failures
- [ ] Documentation explains configuration patterns and usage
- [ ] Ready for deployment-specific configuration management
- [ ] Configuration refresh supports runtime updates

---

*Priority: MEDIUM - Important for maintainable configuration management and deployment flexibility.*