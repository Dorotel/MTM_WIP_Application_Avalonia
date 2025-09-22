# .NET Architecture Good Practices - MTM WIP Application Instructions

**Framework**: .NET 8 with C# 12  
**Application Type**: Avalonia Desktop Application  
**Architecture Pattern**: MVVM with Service-Oriented Design  
**Created**: September 4, 2025  
**Updated**: 2025-09-21 (Phase 1 Material.Avalonia Integration)

---

## 📚 Comprehensive Avalonia Documentation Reference

**IMPORTANT**: This repository contains the complete Avalonia documentation straight from the official website in the `.github/Avalonia-Documentation/` folder. For architectural guidance:

- **Application Architecture**: `.github/Avalonia-Documentation/guides/development-guides/application-architecture/`
- **Dependency Injection**: `.github/Avalonia-Documentation/guides/development-guides/dependency-injection.md`
- **Configuration**: `.github/Avalonia-Documentation/guides/development-guides/application-configuration.md`
- **Performance Tips**: `.github/Avalonia-Documentation/guides/basics/performance-tips.md`
- **Cross-Platform Development**: `.github/Avalonia-Documentation/guides/platforms/`

**Always reference the local Avalonia-Documentation folder for the most current architectural patterns and best practices.**

---

## 🏗️ Core Architecture Principles

### .NET 8 Foundation Standards

```csharp
// Project file configuration - MTM Standard
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>false</UseWPF>
    <UseWindowsForms>false</UseWindowsForms>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
  </PropertyGroup>
</Project>

// C# 12 Language Features - MANDATORY Usage
public class InventoryService : IInventoryService
{
    // Required null validation pattern
    public InventoryService(ILogger<InventoryService> logger, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(logger);    // C# 12 pattern
        ArgumentNullException.ThrowIfNull(config);
        
        _logger = logger;
        _configuration = config;
    }
    
    // Collection expressions (C# 12)
    private readonly string[] _supportedOperations = ["90", "100", "110", "120"];
    
    // Primary constructor usage for simple classes
    public record InventoryItem(string PartId, string Operation, int Quantity, string Location);
}
```

### Dependency Injection Architecture (Microsoft.Extensions.DependencyInjection)

```csharp
// Service registration pattern - ESTABLISHED in Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Singleton services (Application lifetime)
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IThemeService, ThemeService>();
        services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();

        // Scoped services (Per logical operation)
        services.TryAddScoped<IInventoryService, InventoryService>();
        services.TryAddScoped<IMasterDataService, MasterDataService>();
        services.TryAddScoped<IQuickButtonsService, QuickButtonsService>();

        // Transient services (Per request)
        services.TryAddTransient<InventoryTabViewModel>();
        services.TryAddTransient<SettingsViewModel>();
        
        // Logging configuration
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        return services;
    }
}

// Constructor injection standard
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IMasterDataService _masterDataService;

    public InventoryService(
        ILogger<InventoryService> logger,
        IConfigurationService configurationService,
        IMasterDataService masterDataService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(masterDataService);

        _logger = logger;
        _configurationService = configurationService;
        _masterDataService = masterDataService;
    }
}
```

### Service Layer Design Patterns

#### Category-Based Service Organization (MTM ESTABLISHED PATTERN)

```csharp
// CORRECT: Category-based service consolidation in single files
// File: Services/QuickButtons.cs (700+ lines - ESTABLISHED PATTERN)
namespace MTM_WIP_Application_Avalonia.Services
{
    public interface IQuickButtonsService 
    { 
        Task<ServiceResult<List<QuickButtonItem>>> GetLast10TransactionsAsync();
        Task<ServiceResult> ExecuteQuickActionAsync(QuickButtonItem item);
    }
    
    public class QuickButtonsService : IQuickButtonsService 
    { 
        // Full implementation with all related functionality
    }
    
    public class QuickButtonItem 
    { 
        // Related data model
    }
    
    public class QuickActionExecutedEventArgs : EventArgs 
    { 
        // Related event args
    }
}

// File: Services/Configuration.cs
namespace MTM_WIP_Application_Avalonia.Services
{
    public interface IConfigurationService { /* configuration operations */ }
    public class ConfigurationService : IConfigurationService { /* implementation */ }
    
    public interface IApplicationStateService { /* application state */ }
    public class ApplicationStateService : IApplicationStateService { /* implementation */ }
}
```

#### Service Result Pattern (MANDATORY)

```csharp
// Standard service result pattern used throughout MTM application
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    
    public static ServiceResult Success(string message = "") 
        => new() { IsSuccess = true, Message = message };
    
    public static ServiceResult Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }
    
    public static ServiceResult<T> Success(T data, string message = "")
        => new() { IsSuccess = true, Data = data, Message = message };
    
    public static new ServiceResult<T> Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}

// Usage in service methods
public async Task<ServiceResult<List<InventoryItem>>> GetInventoryAsync(string partId)
{
    try
    {
        var result = await _databaseService.GetInventoryAsync(partId);
        
        if (result.IsSuccess)
        {
            return ServiceResult<List<InventoryItem>>.Success(result.Data, "Inventory retrieved successfully");
        }
        else
        {
            return ServiceResult<List<InventoryItem>>.Failure("Failed to retrieve inventory");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving inventory for {PartId}", partId);
        return ServiceResult<List<InventoryItem>>.Failure("Database error", ex);
    }
}
```

---

## 🔍 Error Handling and Logging Architecture

### Centralized Error Handling (MANDATORY PATTERN)

```csharp
// All exceptions MUST use the centralized error handler - Services/ErrorHandling.cs
try
{
    await SomeBusinessOperationAsync();
}
catch (Exception ex)
{
    // ALWAYS use centralized error handling
    await Services.ErrorHandling.HandleErrorAsync(ex, "Context description");
}

// ErrorHandling service implementation
public static class ErrorHandling
{
    public static async Task HandleErrorAsync(Exception exception, string context)
    {
        // Structured logging
        Logger.LogError(exception, "Error in {Context}: {Message}", context, exception.Message);
        
        // Database audit logging
        await LogErrorToDatabaseAsync(exception, context);
        
        // User notification
        await ShowUserFriendlyNotificationAsync(GetUserFriendlyMessage(exception));
    }

    private static async Task LogErrorToDatabaseAsync(Exception exception, string context)
    {
        try
        {
            var parameters = new MySqlParameter[]
            {
                new("p_ErrorMessage", exception.Message),
                new("p_StackTrace", exception.StackTrace ?? string.Empty),
                new("p_Context", context),
                new("p_User", Environment.UserName),
                new("p_Timestamp", DateTime.Now)
            };

            await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "log_error_Add_Error", parameters);
        }
        catch (Exception logEx)
        {
            Logger.LogCritical(logEx, "Failed to log error to database");
        }
    }
}
```

### Structured Logging Standards (Microsoft.Extensions.Logging)

```csharp
// Logging patterns used throughout MTM application
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;

    public async Task<ServiceResult> AddInventoryAsync(InventoryItem item)
    {
        // Information logging with structured data
        _logger.LogInformation("Starting inventory add operation for {PartId} at {Operation} by {User}",
            item.PartId, item.Operation, Environment.UserName);

        try
        {
            var result = await ProcessInventoryAsync(item);
            
            // Success logging
            _logger.LogInformation("Successfully added inventory: {PartId}, Quantity: {Quantity}",
                item.PartId, item.Quantity);
                
            return result;
        }
        catch (Exception ex)
        {
            // Error logging with full context
            _logger.LogError(ex, "Failed to add inventory for {PartId} in {Operation}",
                item.PartId, item.Operation);
                
            throw; // Re-throw for centralized handling
        }
    }

    // Performance logging pattern
    public async Task<ServiceResult> GetInventoryWithPerformanceLoggingAsync(string partId)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["PartId"] = partId,
            ["Operation"] = nameof(GetInventoryWithPerformanceLoggingAsync)
        });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var result = await GetInventoryInternalAsync(partId);
            stopwatch.Stop();
            
            _logger.LogInformation("Inventory retrieval completed in {ElapsedMs}ms for {PartId}",
                stopwatch.ElapsedMilliseconds, partId);
                
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Inventory retrieval failed after {ElapsedMs}ms for {PartId}",
                stopwatch.ElapsedMilliseconds, partId);
            throw;
        }
    }
}
```

---

## 💾 Data Access Architecture

### Database Integration Pattern (MySQL with Stored Procedures ONLY)

```csharp
// MANDATORY: All database operations MUST use stored procedures
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public async Task<ServiceResult<DataTable>> ExecuteStoredProcedureAsync(
        string procedureName, 
        MySqlParameter[] parameters)
    {
        try
        {
            _logger.LogInformation("Executing stored procedure {ProcedureName} with {ParameterCount} parameters",
                procedureName, parameters.Length);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, procedureName, parameters);

            if (result.Status == 1)
            {
                _logger.LogInformation("Stored procedure {ProcedureName} executed successfully, returned {RowCount} rows",
                    procedureName, result.Data.Rows.Count);
                    
                return ServiceResult<DataTable>.Success(result.Data);
            }
            else
            {
                _logger.LogWarning("Stored procedure {ProcedureName} returned status {Status}",
                    procedureName, result.Status);
                    
                return ServiceResult<DataTable>.Failure("Database operation failed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            return ServiceResult<DataTable>.Failure("Database error", ex);
        }
    }

    // Example: Inventory operations using stored procedures
    public async Task<ServiceResult<List<InventoryItem>>> GetInventoryByPartIdAsync(string partId)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", partId)
        };

        var result = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartID", parameters);

        if (result.IsSuccess)
        {
            var items = ConvertDataTableToInventoryItems(result.Data);
            return ServiceResult<List<InventoryItem>>.Success(items);
        }

        return ServiceResult<List<InventoryItem>>.Failure(result.Message, result.Exception);
    }
}

// WRONG: Direct SQL queries are prohibited
public async Task<List<InventoryItem>> GetInventoryDirectSQL(string partId) // NEVER DO THIS
{
    var sql = $"SELECT * FROM inventory WHERE part_id = '{partId}'"; // SQL INJECTION RISK
    // This pattern is completely prohibited
}
```

### Data Model and Entity Patterns

```csharp
// Clean data models with proper validation attributes
public class InventoryItem
{
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    public string PartId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Operation is required")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Operation must be numeric")]
    public string Operation { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; } = 1;

    [Required(ErrorMessage = "Location is required")]
    [StringLength(20, ErrorMessage = "Location cannot exceed 20 characters")]
    public string Location { get; set; } = string.Empty;

    // Audit fields
    public string UserId { get; set; } = Environment.UserName;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string TransactionType { get; set; } = string.Empty; // IN, OUT, TRANSFER
}

// Result pattern for database operations
public class DatabaseResult
{
    public int Status { get; set; }        // 1 = Success, 0 = Failure
    public DataTable Data { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
```

---

## ⚡ Performance and Memory Management

### Async/Await Best Practices

```csharp
// Proper async/await implementation
public class InventoryService : IInventoryService
{
    // Use ConfigureAwait(false) in library code
    public async Task<ServiceResult> ProcessInventoryAsync(InventoryItem item)
    {
        var validationResult = await ValidateInventoryItemAsync(item).ConfigureAwait(false);
        if (!validationResult.IsSuccess)
            return validationResult;

        var databaseResult = await SaveToDatabase(item).ConfigureAwait(false);
        return databaseResult;
    }

    // Avoid blocking on async calls
    public ServiceResult ProcessInventorySync(InventoryItem item)
    {
        // WRONG: Never use .Result or .Wait()
        // var result = ProcessInventoryAsync(item).Result; // DEADLOCK RISK

        // CORRECT: Async all the way down
        return ProcessInventoryAsync(item).GetAwaiter().GetResult(); // Only if absolutely necessary
    }

    // Proper cancellation token usage
    public async Task<ServiceResult> GetInventoryWithCancellationAsync(
        string partId, 
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _databaseService.GetInventoryAsync(partId, cancellationToken);
        return result;
    }
}
```

### Memory Management and Disposal

```csharp
// Proper resource disposal patterns
public class DatabaseService : IDatabaseService, IDisposable
{
    private readonly MySqlConnection _connection;
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Dispose();
                // Dispose other managed resources
            }
            
            // Clean up unmanaged resources if any
            _disposed = true;
        }
    }

    // Async disposal for .NET 8
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
        }
    }
}

// Using statement patterns
public async Task<ServiceResult> ProcessWithDisposableResource()
{
    using var resource = new SomeDisposableResource();
    await resource.ProcessAsync();
    
    // Resource automatically disposed
    return ServiceResult.Success("Processing completed");
}

// Async using pattern
public async Task<ServiceResult> ProcessWithAsyncDisposableResource()
{
    await using var resource = new SomeAsyncDisposableResource();
    await resource.ProcessAsync();
    
    // Resource automatically disposed asynchronously
    return ServiceResult.Success("Processing completed");
}
```

---

## 🔧 Configuration and Environment Management

### Configuration Pattern (Microsoft.Extensions.Configuration)

```csharp
// Configuration service implementation
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetValue(string key, string defaultValue = "")
    {
        try
        {
            var value = _configuration[key];
            
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogWarning("Configuration key {Key} not found, using default value {DefaultValue}", 
                    key, defaultValue);
                return defaultValue;
            }

            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading configuration key {Key}", key);
            return defaultValue;
        }
    }

    public T GetValue<T>(string key, T defaultValue = default(T)!)
    {
        try
        {
            var section = _configuration.GetSection(key);
            
            if (!section.Exists())
            {
                _logger.LogWarning("Configuration section {Key} not found, using default value", key);
                return defaultValue;
            }

            return section.Get<T>() ?? defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading configuration section {Key}", key);
            return defaultValue;
        }
    }
}

// appsettings.json structure
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MTM_WIP;Uid=root;Pwd=password;"
  },
  "MTMSettings": {
    "DefaultLocation": "A01",
    "DefaultOperation": "100",
    "MaxQuickButtons": 10,
    "AutoSaveInterval": 30
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### Environment-Specific Configuration

```csharp
// Program.cs configuration setup
public static void Main(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);

    // Configure services with environment awareness
    var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
    
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddCommandLine(args);

    // Register services
    builder.Services.AddMTMServices(builder.Configuration);

    var app = builder.Build();
    
    // Configure application
    app.Run();
}
```

---

## 🧪 Testing Architecture and Standards

### Unit Testing Patterns (xUnit + Moq)

```csharp
public class InventoryServiceTests : IDisposable
{
    private readonly Mock<ILogger<InventoryService>> _mockLogger;
    private readonly Mock<IConfigurationService> _mockConfiguration;
    private readonly Mock<IDatabaseService> _mockDatabase;
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _mockLogger = new Mock<ILogger<InventoryService>>();
        _mockConfiguration = new Mock<IConfigurationService>();
        _mockDatabase = new Mock<IDatabaseService>();

        _inventoryService = new InventoryService(
            _mockLogger.Object,
            _mockConfiguration.Object,
            _mockDatabase.Object);
    }

    [Fact]
    public async Task AddInventoryAsync_ValidItem_ReturnsSuccess()
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            PartId = "TEST001",
            Operation = "100",
            Quantity = 5,
            Location = "A01"
        };

        _mockDatabase
            .Setup(d => d.ExecuteStoredProcedureAsync("inv_inventory_Add_Item", It.IsAny<MySqlParameter[]>()))
            .ReturnsAsync(ServiceResult<DataTable>.Success(new DataTable()));

        // Act
        var result = await _inventoryService.AddInventoryAsync(inventoryItem);

        // Assert
        Assert.True(result.IsSuccess);
        _mockDatabase.Verify(d => d.ExecuteStoredProcedureAsync(
            "inv_inventory_Add_Item", 
            It.Is<MySqlParameter[]>(p => p.Any(param => param.ParameterName == "p_PartID" && param.Value.ToString() == "TEST001"))), 
            Times.Once);
    }

    [Theory]
    [InlineData("", "100", 1, "A01")] // Empty PartId
    [InlineData("TEST001", "", 1, "A01")] // Empty Operation
    [InlineData("TEST001", "100", 0, "A01")] // Zero Quantity
    [InlineData("TEST001", "100", 1, "")] // Empty Location
    public async Task AddInventoryAsync_InvalidItem_ReturnsFailure(
        string partId, string operation, int quantity, string location)
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            PartId = partId,
            Operation = operation,
            Quantity = quantity,
            Location = location
        };

        // Act
        var result = await _inventoryService.AddInventoryAsync(inventoryItem);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("validation", result.Message.ToLowerInvariant());
    }

    public void Dispose()
    {
        _inventoryService?.Dispose();
    }
}
```

### Integration Testing Patterns

```csharp
public class DatabaseIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ITestOutputHelper _output;

    public DatabaseIntegrationTests(DatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task StoredProcedure_inv_inventory_Add_Item_ExecutesSuccessfully()
    {
        // Arrange
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", "INTEGRATION_TEST_001"),
            new("p_Operation", "100"),
            new("p_Quantity", 10),
            new("p_Location", "TEST_LOC"),
            new("p_User", "TestUser"),
            new("p_TransactionType", "IN")
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _fixture.ConnectionString, "inv_inventory_Add_Item", parameters);

        // Assert
        Assert.Equal(1, result.Status);
        _output.WriteLine($"Integration test completed with status: {result.Status}");

        // Cleanup
        await CleanupTestDataAsync("INTEGRATION_TEST_001");
    }

    private async Task CleanupTestDataAsync(string partId)
    {
        var cleanupParameters = new MySqlParameter[]
        {
            new("p_PartID", partId)
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _fixture.ConnectionString, "inv_inventory_Remove_Item", cleanupParameters);
    }
}

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }

    public DatabaseFixture()
    {
        // Setup test database connection
        ConnectionString = "Server=localhost;Database=MTM_WIP_Test;Uid=root;Pwd=password;";
        
        // Initialize test database
        InitializeTestDatabase();
    }

    private void InitializeTestDatabase()
    {
        // Create test database schema if needed
        // Seed test data if required
    }

    public void Dispose()
    {
        // Cleanup test database
    }
}
```

---

## 📚 Related Architecture Documentation

- **MVVM Patterns**: [MTM Community Toolkit Implementation](./mvvm-community-toolkit.instructions.md)
- **Database Patterns**: [MySQL Stored Procedures Guide](./mysql-database-patterns.instructions.md)
- **UI Architecture**: [Avalonia UI Guidelines](./avalonia-ui-guidelines.instructions.md)
- **Service Documentation**: [Service Layer Architecture](./service-layer-documentation.md)

---

**Document Status**: ✅ Complete Architecture Reference  
**Framework Version**: .NET 8.0  
**Last Updated**: September 4, 2025  
**Maintained By**: MTM Development Team

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**

- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**

- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**

- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**

- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**

- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**

- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**

- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
