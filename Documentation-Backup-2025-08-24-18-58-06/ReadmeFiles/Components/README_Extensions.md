# Extensions Directory

This directory contains extension methods and utility classes that extend the functionality of the MTM WIP Application Avalonia.

## ?? Extension Architecture

### Extension Categories

#### Service Registration Extensions
- **ServiceCollectionExtensions**: Dependency injection container setup
- **Configuration Extensions**: Application configuration and settings
- **Database Extensions**: Database connection and service registration

#### Utility Extensions
- **String Extensions**: String manipulation and validation utilities
- **Collection Extensions**: LINQ and collection operation helpers
- **Validation Extensions**: Data validation and error handling utilities

#### UI Extensions
- **Control Extensions**: Avalonia UI control enhancements
- **Binding Extensions**: MVVM binding utilities and helpers
- **Theme Extensions**: Dynamic theming and styling support

## ?? Extension Files

### Service Registration (`ServiceCollectionExtensions.cs`)

Primary dependency injection setup for the application:

```csharp
/// <summary>
/// Extension methods for IServiceCollection to register application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all core services required for basic application functionality
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Database services
        services.AddSingleton<IDatabaseService, DatabaseService>();
        
        // Configuration services
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        
        // Logging services
        services.AddSingleton<LoggingUtility>();
        
        return services;
    }

    /// <summary>
    /// Registers business logic services for inventory and operations
    /// </summary>
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Inventory management
        services.AddScoped<IInventoryService, InventoryService>();
        
        // User and transaction services
        services.AddScoped<IUserAndTransactionServices, UserAndTransactionServices>();
        
        // Application state management
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();
        
        return services;
    }

    /// <summary>
    /// Registers utility and support services
    /// </summary>
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        // Error handling
        services.AddTransient<Service_ErrorHandler>();
        
        // Background services
        services.AddHostedService<BackgroundTaskService>();
        
        return services;
    }

    /// <summary>
    /// Registers all application services in the correct order
    /// </summary>
    public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core infrastructure first
        services.AddCoreServices();
        
        // Business services
        services.AddBusinessServices();
        
        // Utility services
        services.AddUtilityServices();
        
        // Configure options
        services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));
        
        return services;
    }
}
```

### Service Lifetime Management
Proper service lifetime configuration:

```csharp
public static class ServiceLifetimeExtensions
{
    /// <summary>
    /// Registers services with appropriate lifetimes for optimal performance
    /// </summary>
    public static IServiceCollection AddServicesWithLifetimes(this IServiceCollection services)
    {
        // Singleton: Services that maintain state across the application
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();
        services.AddSingleton<LoggingUtility>();

        // Scoped: Services that are created per operation scope
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IUserAndTransactionServices, UserAndTransactionServices>();

        // Transient: Services that are created each time they're requested
        services.AddTransient<Service_ErrorHandler>();
        services.AddTransient<IValidationService, ValidationService>();

        return services;
    }
}
```

## ?? Utility Extensions

### String Extensions
Common string operations and validations:

```csharp
/// <summary>
/// Extension methods for string manipulation and validation
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Checks if a string is null, empty, or whitespace
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Safely trims a string, returning empty string if null
    /// </summary>
    public static string SafeTrim(this string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Validates if string is a valid part ID format
    /// </summary>
    public static bool IsValidPartId(this string partId)
    {
        if (string.IsNullOrWhiteSpace(partId))
            return false;

        // Part ID validation rules
        return partId.Length <= 300 && 
               !partId.Contains("  ") && // No double spaces
               partId.All(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c) || c == ' ' || c == '-');
    }

    /// <summary>
    /// Validates if string is a valid operation code
    /// </summary>
    public static bool IsValidOperation(this string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
            return false;

        // Operations are typically numeric workflow step identifiers
        return operation.Length <= 100 && 
               operation.All(c => char.IsDigit(c) || c == '.');
    }

    /// <summary>
    /// Safely converts string to integer with default value
    /// </summary>
    public static int ToIntSafe(this string value, int defaultValue = 0)
    {
        return int.TryParse(value, out int result) ? result : defaultValue;
    }

    /// <summary>
    /// Truncates string to specified length with ellipsis
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// Converts string to title case for display
    /// </summary>
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(value.ToLowerInvariant());
    }
}
```

### Collection Extensions
Enhanced collection operations:

```csharp
/// <summary>
/// Extension methods for collection operations
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Safely gets an item from collection by index, returning default if out of range
    /// </summary>
    public static T? SafeGet<T>(this IList<T> list, int index, T? defaultValue = default)
    {
        return index >= 0 && index < list.Count ? list[index] : defaultValue;
    }

    /// <summary>
    /// Adds range of items to ObservableCollection
    /// </summary>
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    /// <summary>
    /// Removes items that match the predicate
    /// </summary>
    public static int RemoveWhere<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
    {
        var itemsToRemove = collection.Where(predicate).ToList();
        foreach (var item in itemsToRemove)
        {
            collection.Remove(item);
        }
        return itemsToRemove.Count;
    }

    /// <summary>
    /// Groups items and returns dictionary for quick lookup
    /// </summary>
    public static Dictionary<TKey, List<TValue>> ToLookupDictionary<TValue, TKey>(
        this IEnumerable<TValue> source, 
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        return source.GroupBy(keySelector)
                    .ToDictionary(g => g.Key, g => g.ToList());
    }

    /// <summary>
    /// Batch processes items in chunks of specified size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        
        foreach (var item in source)
        {
            batch.Add(item);
            
            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }
        
        if (batch.Count > 0)
            yield return batch;
    }
}
```

### Validation Extensions
Data validation and error handling utilities:

```csharp
/// <summary>
/// Extension methods for data validation
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates inventory item data and returns validation result
    /// </summary>
    public static ValidationResult ValidateInventoryItem(this InventoryItem item)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(item.PartId))
            errors.Add("Part ID is required");
        else if (!item.PartId.IsValidPartId())
            errors.Add("Part ID format is invalid");

        if (string.IsNullOrWhiteSpace(item.Operation))
            errors.Add("Operation is required");
        else if (!item.Operation.IsValidOperation())
            errors.Add("Operation format is invalid");

        if (string.IsNullOrWhiteSpace(item.Location))
            errors.Add("Location is required");

        if (item.Quantity <= 0)
            errors.Add("Quantity must be greater than zero");

        if (item.Notes?.Length > 1000)
            errors.Add("Notes cannot exceed 1000 characters");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }

    /// <summary>
    /// Validates user input for required fields
    /// </summary>
    public static bool IsValidRequired(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Validates numeric input within range
    /// </summary>
    public static bool IsValidRange(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }
}

/// <summary>
/// Validation result container
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public string ErrorMessage => string.Join(Environment.NewLine, Errors);
}
```

## ?? UI Extensions

### Control Extensions
Avalonia UI control enhancements:

```csharp
/// <summary>
/// Extension methods for Avalonia UI controls
/// </summary>
public static class ControlExtensions
{
    /// <summary>
    /// Sets error state styling on a control
    /// </summary>
    public static void SetErrorState(this Control control, bool hasError)
    {
        if (hasError)
            control.Classes.Add("error");
        else
            control.Classes.Remove("error");
    }

    /// <summary>
    /// Finds a control by name in the visual tree
    /// </summary>
    public static T? FindControlByName<T>(this Control control, string name) where T : Control
    {
        return control.FindNameScope()?.Find<T>(name);
    }

    /// <summary>
    /// Applies MTM theme colors to a control
    /// </summary>
    public static void ApplyMTMTheme(this Control control, MTMTheme theme)
    {
        switch (theme)
        {
            case MTMTheme.Primary:
                control.SetValue(Control.BackgroundProperty, new SolidColorBrush(Color.Parse("#4B45ED")));
                break;
            case MTMTheme.Secondary:
                control.SetValue(Control.BackgroundProperty, new SolidColorBrush(Color.Parse("#8345ED")));
                break;
            case MTMTheme.Accent:
                control.SetValue(Control.BackgroundProperty, new SolidColorBrush(Color.Parse("#BA45ED")));
                break;
        }
    }

    /// <summary>
    /// Animates a control with fade in/out effect
    /// </summary>
    public static async Task AnimateFadeAsync(this Control control, bool fadeIn, TimeSpan duration)
    {
        var startOpacity = fadeIn ? 0.0 : 1.0;
        var endOpacity = fadeIn ? 1.0 : 0.0;
        
        control.Opacity = startOpacity;
        
        var animation = new DoubleTransition
        {
            Property = Control.OpacityProperty,
            Duration = duration
        };
        
        await control.Animate(animation, endOpacity);
    }
}

public enum MTMTheme
{
    Primary,
    Secondary,
    Accent,
    Success,
    Warning,
    Error
}
```

### Binding Extensions
MVVM binding utilities and helpers:

```csharp
/// <summary>
/// Extension methods for reactive binding support
/// </summary>
public static class BindingExtensions
{
    /// <summary>
    /// Creates a two-way binding between properties
    /// </summary>
    public static IDisposable BindTwoWay<TSource, TTarget, TProperty>(
        this TSource source,
        TTarget target,
        Expression<Func<TSource, TProperty>> sourceProperty,
        Expression<Func<TTarget, TProperty>> targetProperty)
        where TSource : INotifyPropertyChanged
        where TTarget : INotifyPropertyChanged
    {
        // Implementation for two-way binding
        var disposables = new CompositeDisposable();
        
        // Bind source to target
        source.WhenAnyValue(sourceProperty)
              .Subscribe(value => SetPropertyValue(target, targetProperty, value))
              .DisposeWith(disposables);
        
        // Bind target to source
        target.WhenAnyValue(targetProperty)
              .Subscribe(value => SetPropertyValue(source, sourceProperty, value))
              .DisposeWith(disposables);
        
        return disposables;
    }

    /// <summary>
    /// Creates a command binding with automatic enable/disable logic
    /// </summary>
    public static IDisposable BindCommand<T>(
        this Button button,
        ReactiveCommand<Unit, Unit> command,
        IObservable<bool> canExecute)
    {
        var disposables = new CompositeDisposable();
        
        // Bind command
        button.Command = command;
        
        // Bind enable state
        canExecute.Subscribe(enabled => button.IsEnabled = enabled)
                  .DisposeWith(disposables);
        
        return disposables;
    }

    private static void SetPropertyValue<T, TProperty>(T target, Expression<Func<T, TProperty>> property, TProperty value)
    {
        if (property.Body is MemberExpression memberExpression)
        {
            var propertyInfo = memberExpression.Member as PropertyInfo;
            propertyInfo?.SetValue(target, value);
        }
    }
}
```

## ?? Configuration Extensions

### Application Configuration
Enhanced configuration management:

```csharp
/// <summary>
/// Extension methods for application configuration
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets connection string with validation
    /// </summary>
    public static string GetConnectionStringRequired(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{name}' is required but not configured");
        
        return connectionString;
    }

    /// <summary>
    /// Gets configuration value with type conversion and default
    /// </summary>
    public static T GetValueSafe<T>(this IConfiguration configuration, string key, T defaultValue = default!)
    {
        try
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Validates required configuration sections
    /// </summary>
    public static void ValidateConfiguration(this IConfiguration configuration)
    {
        var requiredSections = new[]
        {
            "ConnectionStrings:DefaultConnection",
            "AppConfiguration:Database:CommandTimeout",
            "AppConfiguration:UI:DefaultTheme"
        };

        var missingConfigs = new List<string>();

        foreach (var section in requiredSections)
        {
            if (string.IsNullOrWhiteSpace(configuration[section]))
                missingConfigs.Add(section);
        }

        if (missingConfigs.Any())
        {
            throw new InvalidOperationException(
                $"Missing required configuration sections: {string.Join(", ", missingConfigs)}");
        }
    }
}
```

## ?? Database Extensions

### Database Connection and Query Extensions
Enhanced database operations:

```csharp
/// <summary>
/// Extension methods for database operations
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Registers database services with connection string validation
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionStringRequired("DefaultConnection");
        
        // Validate connection string format
        if (!IsValidMySqlConnectionString(connectionString))
            throw new InvalidOperationException("Invalid MySQL connection string format");
        
        services.AddSingleton<IDatabaseService>(provider => 
            new DatabaseService(connectionString));
        
        return services;
    }

    /// <summary>
    /// Validates MySQL connection string format
    /// </summary>
    private static bool IsValidMySqlConnectionString(string connectionString)
    {
        try
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            return !string.IsNullOrWhiteSpace(builder.Server) && 
                   !string.IsNullOrWhiteSpace(builder.Database);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates parameters dictionary for stored procedures
    /// </summary>
    public static Dictionary<string, object> ToParameterDictionary(this object parameters)
    {
        var result = new Dictionary<string, object>();
        
        if (parameters == null)
            return result;
        
        var properties = parameters.GetType().GetProperties();
        
        foreach (var property in properties)
        {
            var value = property.GetValue(parameters);
            result[property.Name] = value ?? DBNull.Value;
        }
        
        return result;
    }
}
```

## ?? Testing Extensions

### Unit Testing Utilities
Extensions for easier unit testing:

```csharp
/// <summary>
/// Extension methods for unit testing
/// </summary>
public static class TestingExtensions
{
    /// <summary>
    /// Creates a mock service collection for testing
    /// </summary>
    public static IServiceCollection CreateMockServiceCollection()
    {
        var services = new ServiceCollection();
        
        // Add mock services
        services.AddSingleton(Mock.Of<IDatabaseService>());
        services.AddSingleton(Mock.Of<IConfigurationService>());
        services.AddScoped(Mock.Of<IInventoryService>());
        
        return services;
    }

    /// <summary>
    /// Creates test inventory item with default values
    /// </summary>
    public static InventoryItem CreateTestInventoryItem(
        string partId = "TEST-PART-001",
        string operation = "100",
        string location = "TEST-LOCATION",
        int quantity = 10)
    {
        return new InventoryItem
        {
            PartId = partId,
            Operation = operation,
            Location = location,
            Quantity = quantity,
            CreatedDate = DateTime.Now,
            CreatedBy = "TEST-USER",
            Notes = "Test inventory item"
        };
    }

    /// <summary>
    /// Asserts that a result is successful
    /// </summary>
    public static void AssertSuccess(this Result result)
    {
        Assert.That(result.IsSuccess, Is.True, 
            $"Expected success but got error: {result.ErrorMessage}");
    }

    /// <summary>
    /// Asserts that a result has failed with specific error
    /// </summary>
    public static void AssertFailure(this Result result, string expectedError)
    {
        Assert.That(result.IsSuccess, Is.False, "Expected failure but got success");
        Assert.That(result.ErrorMessage, Does.Contain(expectedError));
    }
}
```

## ?? Development Guidelines

### Adding New Extensions
1. **Purpose-Specific**: Create extensions for specific, well-defined purposes
2. **Null Safety**: Always handle null inputs gracefully
3. **Performance**: Consider performance implications of extension methods
4. **Documentation**: Add comprehensive XML comments
5. **Unit Tests**: Create tests for all extension methods

### Extension Conventions
- **Naming**: Use descriptive names that clearly indicate functionality
- **Parameters**: Use this parameter for the type being extended
- **Return Types**: Return appropriate types for method chaining when possible
- **Exception Handling**: Handle exceptions gracefully or document when they might occur

## ?? Related Documentation

- **Service Registration**: See individual service classes for injection patterns
- **Configuration**: Application configuration documentation
- **Testing**: Unit testing guidelines and patterns
- **UI Components**: Avalonia UI control documentation

---

*This directory extends the functionality of the MTM WIP Application with utility methods, service registration patterns, and helper functions that enhance development productivity and code maintainability.*