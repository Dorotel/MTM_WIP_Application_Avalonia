---
description: 'Technology stack context for MTM WIP Application - .NET 8, Avalonia UI, MVVM Community Toolkit, MySQL patterns'
context_type: 'technology'
applies_to: '**/*'
priority: 'high'
---

# MTM Technology Context - Core Technology Stack Knowledge

## Context Overview

This context provides comprehensive knowledge about the MTM WIP Application technology stack, patterns, and implementation requirements. Use this context to understand technology-specific decisions, constraints, and best practices.

## Technology Stack Foundation

### .NET 8 with C# 12 (Target Framework)
```xml
<TargetFramework>net8.0</TargetFramework>
<LangVersion>12.0</LangVersion>
<Nullable>enable</Nullable>
```

**Key C# 12 Features in Use:**
- **Primary Constructors**: Used for simple data classes
- **Collection Expressions**: `["item1", "item2"]` syntax
- **Required Members**: `required` keyword for mandatory properties
- **Null Parameter Checking**: `ArgumentNullException.ThrowIfNull(param)`

**Pattern Example:**
```csharp
// C# 12 pattern in MTM application
public record InventoryItem(string PartId, string Operation, int Quantity, string Location)
{
    public required string UserId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
}

public class InventoryService(ILogger<InventoryService> logger, IConfiguration configuration)
{
    public async Task<ServiceResult> ProcessAsync(InventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(item.PartId);
        
        // Implementation using modern patterns
    }
}
```

### Avalonia UI 11.3.4 (Cross-Platform UI Framework)
**NOT WPF** - Avalonia has specific syntax requirements and capabilities.

**Critical Avalonia-Specific Requirements:**
```xml
<!-- CORRECT Avalonia namespace -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

<!-- CRITICAL: Use x:Name NOT Name on Grid -->
<Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">

<!-- DynamicResource for theme support -->
<Border Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}">
</Grid>
```

**Avalonia vs WPF Differences:**
- Use `TextBlock` instead of `Label`
- Use `Flyout` instead of `Popup`
- Grid naming: `x:Name` is mandatory, `Name` causes AVLN2000 errors
- Namespace: `https://github.com/avaloniaui` NOT WPF namespace

**Cross-Platform Considerations:**
- **Windows**: Full feature support, native look and feel
- **macOS**: Cocoa integration, menu bar support
- **Linux**: GTK backend, X11/Wayland support
- **Android**: Mobile-optimized controls and touch support

### MVVM Community Toolkit 8.3.2 (Source Generator MVVM)
**Completely replaces ReactiveUI** - No ReactiveUI patterns in MTM application.

**Mandatory Patterns:**
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    [ObservableProperty]
    private string partId = string.Empty; // Must be private field
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteTransaction))]
    private bool isLoading;
    
    [RelayCommand(CanExecute = nameof(CanExecuteTransaction))]
    private async Task SaveAsync()
    {
        IsLoading = true;
        try
        {
            await _inventoryService.SaveAsync(PartId);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanExecuteTransaction => !IsLoading && !string.IsNullOrWhiteSpace(PartId);
}
```

**Source Generator Benefits:**
- Automatic property change notifications
- Automatic command implementations
- Compile-time code generation
- Better performance than reflection-based MVVM

### MySQL 9.4.0 (Database with Stored Procedures Only)
**Absolutely NO direct SQL queries** - All database operations through stored procedures.

**Mandatory Database Pattern:**
```csharp
// ONLY pattern allowed in MTM application
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_OperationNumber", operation),
    new("p_Quantity", quantity),
    new("p_Location", location),
    new("p_User", userId)
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item", // Always use actual stored procedure names
    parameters
);

// Process standardized result
if (result.Status == 1)
{
    // Success - process result.Data (DataTable)
}
else
{
    // Error - handle result.Message
}
```

**45+ Available Stored Procedures:**
- **Inventory**: `inv_inventory_Add_Item`, `inv_inventory_Remove_Item`, `inv_inventory_Get_ByPartID`
- **Transactions**: `inv_transaction_Add`, `inv_transaction_Get_History`
- **Master Data**: `md_part_ids_Get_All`, `md_locations_Get_All`, `md_operation_numbers_Get_All`
- **QuickButtons**: `qb_quickbuttons_Get_ByUser`, `qb_quickbuttons_Save`
- **Error Logging**: `log_error_Add_Error`, `log_error_Get_All`

### Microsoft Extensions 9.0.8 (DI, Logging, Configuration)
**Comprehensive dependency injection and configuration system.**

**Service Registration Pattern:**
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Singleton services (application lifetime)
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IThemeService, ThemeService>();
        
        // Scoped services (request/operation lifetime)
        services.TryAddScoped<IDatabaseService, DatabaseService>();
        services.TryAddScoped<IInventoryService, InventoryService>();
        
        // Transient services (created each time)
        services.TryAddTransient<InventoryTabViewModel>();
        
        return services;
    }
}
```

**Logging Integration:**
```csharp
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    
    public InventoryService(ILogger<InventoryService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ServiceResult> ProcessAsync(string partId)
    {
        _logger.LogInformation("Processing inventory for part {PartId}", partId);
        
        try
        {
            // Implementation
            _logger.LogInformation("Successfully processed part {PartId}", partId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process part {PartId}", partId);
            throw;
        }
    }
}
```

## Key Technology Patterns

### Dependency Injection Pattern
```csharp
// Constructor injection (preferred)
public class InventoryTabViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly IMasterDataService _masterDataService;
    
    public InventoryTabViewModel(
        ILogger<InventoryTabViewModel> logger,
        IInventoryService inventoryService,
        IMasterDataService masterDataService)
        : base(logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
    }
}
```

### Async/Await Pattern
```csharp
// Proper async implementation
public async Task<ServiceResult<T>> ProcessAsync<T>(T data)
{
    try
    {
        // Use ConfigureAwait(false) in library code
        var result = await DatabaseOperation().ConfigureAwait(false);
        return ServiceResult<T>.Success(result);
    }
    catch (OperationCanceledException)
    {
        return ServiceResult<T>.Cancelled();
    }
    catch (Exception ex)
    {
        await ErrorHandling.HandleErrorAsync(ex, "Process operation");
        return ServiceResult<T>.Failure(ex.Message);
    }
}
```

### Error Handling Pattern
```csharp
// Centralized error handling (mandatory)
try
{
    await SomeBusinessOperation();
}
catch (Exception ex)
{
    // ALWAYS use centralized error handling
    await Services.ErrorHandling.HandleErrorAsync(ex, "Business operation context");
}
```

## Configuration Management

### Configuration Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mtm_wip;Uid=user;Pwd=password;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "MTM_WIP_Application_Avalonia": "Debug"
    }
  },
  "MTMSettings": {
    "DefaultTheme": "MTM_Blue",
    "MaxRecentTransactions": 100,
    "AutoSaveInterval": 300
  }
}
```

### Configuration Access Pattern
```csharp
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    
    public string GetConnectionString() => 
        _configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string not configured");
        
    public T GetSetting<T>(string key, T defaultValue = default) =>
        _configuration.GetValue<T>(key, defaultValue);
}
```

## Performance Considerations

### Memory Management
```csharp
// Proper resource disposal
public class DatabaseService : IDatabaseService, IDisposable
{
    private bool _disposed = false;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Dispose managed resources
            _disposed = true;
        }
    }
}
```

### Caching Strategy
```csharp
// Memory caching for master data
public class MasterDataService : IMasterDataService
{
    private readonly IMemoryCache _cache;
    
    public async Task<List<string>> GetPartIdsAsync()
    {
        const string cacheKey = "master_data_part_ids";
        
        if (_cache.TryGetValue(cacheKey, out List<string> cachedPartIds))
        {
            return cachedPartIds;
        }
        
        var partIds = await LoadPartIdsFromDatabase();
        
        _cache.Set(cacheKey, partIds, TimeSpan.FromMinutes(5));
        return partIds;
    }
}
```

## Common Anti-Patterns (Avoid These)

### ❌ ReactiveUI Patterns
```csharp
// DON'T USE - ReactiveUI completely removed from MTM
public class SomeViewModel : ReactiveObject
{
    private string _property;
    public string Property
    {
        get => _property;
        set => this.RaiseAndSetIfChanged(ref _property, value);
    }
    
    public ReactiveCommand<Unit, Unit> SomeCommand { get; }
}
```

### ❌ Direct SQL Queries
```csharp
// DON'T USE - Direct SQL forbidden in MTM
var sql = $"SELECT * FROM inventory WHERE part_id = '{partId}'";
var command = new MySqlCommand(sql, connection);
```

### ❌ WPF Syntax in Avalonia
```xml
<!-- DON'T USE - WPF namespace in Avalonia -->
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">

<!-- DON'T USE - Name instead of x:Name -->
<Grid Name="MainGrid">
```

### ❌ Synchronous Database Operations
```csharp
// DON'T USE - Synchronous calls block UI thread
var result = SomeAsyncOperation().Result;
var data = SomeAsyncMethod().GetAwaiter().GetResult();
```

## Technology Integration Points

### UI to ViewModel Binding
```xml
<TextBox Text="{Binding PartId}" />
<Button Command="{Binding SaveCommand}" Content="Save" />
<DataGrid ItemsSource="{Binding TransactionHistory}" />
```

### ViewModel to Service Integration
```csharp
[RelayCommand]
private async Task LoadDataAsync()
{
    try
    {
        IsLoading = true;
        var data = await _dataService.GetDataAsync();
        Items.Clear();
        foreach (var item in data)
        {
            Items.Add(item);
        }
    }
    finally
    {
        IsLoading = false;
    }
}
```

### Service to Database Integration
```csharp
public async Task<ServiceResult> SaveAsync(InventoryItem item)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", item.PartId),
        new("p_Operation", item.Operation),
        new("p_Quantity", item.Quantity)
    };
    
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString, "inv_inventory_Add_Item", parameters);
        
    return result.Status == 1 
        ? ServiceResult.Success() 
        : ServiceResult.Failure(result.Message);
}
```

This technology context provides the foundation for all MTM WIP Application development decisions and implementations. Always reference this context when making technology-related choices or implementing new features.

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
