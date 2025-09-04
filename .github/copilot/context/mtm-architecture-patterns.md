# MTM Architecture Patterns Context

## 🏗️ Application Architecture Patterns

### **MVVM Architecture with MVVM Community Toolkit**

#### **ViewModel Layer Patterns**
```csharp
// Standard ViewModel structure
[ObservableObject]
public partial class FeatureViewModel : BaseViewModel
{
    // Observable properties with source generators
    [ObservableProperty]
    private string inputValue = string.Empty;
    
    // Commands with automatic CanExecute
    [RelayCommand(CanExecute = nameof(CanExecuteAction))]
    private async Task ExecuteActionAsync() { }
    
    private bool CanExecuteAction => !IsLoading && !string.IsNullOrEmpty(InputValue);
    
    // Constructor with dependency injection
    public FeatureViewModel(ILogger<FeatureViewModel> logger, IService service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
    }
}
```

#### **View Layer Patterns**
```csharp
// Minimal code-behind pattern
public partial class FeatureView : UserControl
{
    public FeatureView()
    {
        InitializeComponent();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Cleanup subscriptions and resources
        base.OnDetachedFromVisualTree(e);
    }
}
```

#### **Model Layer Patterns**
```csharp
// Data models with proper validation
public class InventoryItem
{
    [Required]
    public string PartId { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### **Service Layer Architecture**

#### **Category-Based Service Consolidation**
Services are organized by functional area, not individual operations:

```
Services/
├── ErrorHandling.cs      # All error management functionality
├── Configuration.cs      # Application configuration and state  
├── Database.cs           # Database connection and operations
├── ThemeService.cs       # UI theming and appearance
├── NavigationService.cs  # View navigation and routing
└── [Domain]Service.cs    # Domain-specific services
```

#### **Service Interface Pattern**
```csharp
// Service interfaces for testability
public interface IInventoryService
{
    Task<OperationResult<List<InventoryItem>>> GetInventoryAsync(string partId);
    Task<OperationResult> AddInventoryAsync(InventoryItem item);
    Task<OperationResult> RemoveInventoryAsync(RemoveRequest request);
}

// Service implementation with error handling
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    
    public async Task<OperationResult<List<InventoryItem>>> GetInventoryAsync(string partId)
    {
        try
        {
            // Implementation with database calls
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Get inventory");
            return OperationResult<List<InventoryItem>>.Failed(ex.Message);
        }
    }
}
```

### **Dependency Injection Patterns**

#### **Service Registration**
```csharp
// Extension method for service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Use TryAdd methods to prevent duplicate registrations
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddScoped<IInventoryService, InventoryService>();
        services.TryAddTransient<InventoryViewModel>();
        
        return services;
    }
}
```

#### **Constructor Injection Pattern**
```csharp
// ViewModels with dependency injection
public InventoryViewModel(
    ILogger<InventoryViewModel> logger,
    IInventoryService inventoryService,
    IConfigurationService configurationService)
    : base(logger)
{
    // Validate all dependencies
    ArgumentNullException.ThrowIfNull(logger);
    ArgumentNullException.ThrowIfNull(inventoryService);
    ArgumentNullException.ThrowIfNull(configurationService);
    
    _inventoryService = inventoryService;
    _configurationService = configurationService;
}
```

### **Database Access Patterns**

#### **Stored Procedure Only Pattern**
```csharp
// All database operations via stored procedures
public async Task<DatabaseResult<DataTable>> ExecuteDatabaseOperation(
    string procedureName, 
    MySqlParameter[] parameters)
{
    var connectionString = Helper_Database_Variables.GetConnectionString();
    
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        procedureName,
        parameters
    );
    
    return result.Status == 1 
        ? DatabaseResult<DataTable>.Success(result.Data)
        : DatabaseResult<DataTable>.Failed("Database operation failed");
}
```

#### **Data Conversion Patterns**
```csharp
// Type-safe data table conversion
private List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
{
    var properties = typeof(T).GetProperties();
    var result = new List<T>();
    
    foreach (DataRow row in dataTable.Rows)
    {
        var item = new T();
        foreach (var property in properties)
        {
            if (dataTable.Columns.Contains(property.Name))
            {
                var value = row[property.Name];
                if (value != DBNull.Value)
                {
                    property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                }
            }
        }
        result.Add(item);
    }
    
    return result;
}
```

### **Error Handling Architecture**

#### **Centralized Error Management**
```csharp
// All errors handled through centralized service
public static class ErrorHandling
{
    public static async Task HandleErrorAsync(Exception exception, string context)
    {
        // Log error with structured logging
        Logger.LogError(exception, "Error in {Context}: {Message}", context, exception.Message);
        
        // Store error in database for audit
        await LogErrorToDatabase(exception, context);
        
        // Show user-friendly message
        await ShowUserErrorMessage(GetUserFriendlyMessage(exception));
    }
}
```

#### **Async Operation Patterns**
```csharp
// Standard async operation wrapper
protected async Task ExecuteWithLoadingAsync(Func<Task> operation)
{
    IsLoading = true;
    ErrorMessage = null;
    
    try
    {
        await operation().ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        await Services.ErrorHandling.HandleErrorAsync(ex, GetType().Name);
        ErrorMessage = "An error occurred. Please try again.";
    }
    finally
    {
        IsLoading = false;
    }
}
```

### **UI Architectural Patterns**

#### **Avalonia AXAML Patterns**
```xml
<!-- Standard UserControl structure -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Views.FeatureView">
    
    <!-- Use x:Name, NOT Name on Grid -->
    <Grid x:Name="MainGrid" ColumnDefinitions="Auto,*" RowDefinitions="Auto,*">
        
        <!-- MTM Design System -->
        <Border Background="#6a0dad" CornerRadius="8" Padding="16">
            <TextBlock Text="Title" Foreground="White" FontWeight="Bold" />
        </Border>
        
    </Grid>
</UserControl>
```

#### **Theme System Integration**
```csharp
// Dynamic theme switching
public class ThemeService : IThemeService
{
    public void ApplyTheme(string themeName)
    {
        var themeUri = new Uri($"avares://MTM_WIP_Application_Avalonia/Resources/Themes/{themeName}.axaml");
        var theme = new StyleInclude(themeUri) { Source = themeUri };
        
        Application.Current?.Styles.Clear();
        Application.Current?.Styles.Add(theme);
    }
}
```

### **Configuration Management Patterns**

#### **Hierarchical Configuration**
```csharp
// Configuration precedence: Database > User Settings > appsettings.json > Defaults
public class ConfigurationService : IConfigurationService
{
    public T GetSetting<T>(string key, T defaultValue = default)
    {
        // 1. Check database user settings
        var userSetting = GetUserSetting<T>(key);
        if (userSetting != null) return userSetting;
        
        // 2. Check appsettings.json
        var appSetting = _configuration.GetValue<T>(key);
        if (appSetting != null) return appSetting;
        
        // 3. Return default
        return defaultValue;
    }
}
```

### **Performance Optimization Patterns**

#### **Lazy Loading**
```csharp
// Lazy initialization of expensive resources
private readonly Lazy<IExpensiveService> _expensiveService;

public ExpensiveService => _expensiveService.Value;
```

#### **Caching Patterns**
```csharp
// Simple memory caching for frequently accessed data
private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

public async Task<T> GetCachedDataAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiry)
{
    if (_cache.TryGetValue(key, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
    {
        return (T)entry.Value;
    }
    
    var data = await factory();
    _cache[key] = new CacheEntry(data, DateTime.UtcNow.Add(expiry));
    return data;
}
```

### **Testing Patterns**

#### **Testable Architecture**
```csharp
// Services designed for testability with interfaces
public interface ITimeProvider
{
    DateTime UtcNow { get; }
}

public class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

// Easy to mock for testing
public class TestTimeProvider : ITimeProvider
{
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
}
```