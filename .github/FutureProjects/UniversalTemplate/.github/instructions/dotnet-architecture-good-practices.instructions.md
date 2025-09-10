# .NET 8 Avalonia Architecture Good Practices - Universal Instructions

**Framework**: .NET 8 with C# 12  
**Application Type**: Avalonia Desktop Application  
**Architecture Pattern**: MVVM with Service-Oriented Design  
**Universal Template**: Extracted from MTM WIP Application

---

## 🏗️ Core Architecture Principles

### .NET 8 Foundation Standards
```csharp
// Project file configuration - Universal Standard
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>
</Project>

// C# 12 Language Features - MANDATORY Usage
public class UniversalService : IUniversalService
{
    // Required null validation pattern
    public UniversalService(ILogger<UniversalService> logger, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(logger);    // C# 12 pattern
        ArgumentNullException.ThrowIfNull(config);
        
        _logger = logger;
        _configuration = config;
    }
    
    // Collection expressions (C# 12)
    private readonly string[] _supportedThemes = ["Light", "Dark", "Auto"];
    
    // Primary constructor usage for simple classes
    public record ConfigurationItem(string Key, string Value, bool IsRequired);
}
```

### Dependency Injection Architecture (Microsoft.Extensions.DependencyInjection)
```csharp
// Service registration pattern - UNIVERSAL
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Singleton services (Application lifetime)
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IThemeService, ThemeService>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        
        // Scoped services (Per operation lifetime)
        services.TryAddScoped<IDataService, DataService>();
        services.TryAddScoped<IBusinessService, BusinessService>();
        
        // Transient services (New instance each time)
        services.TryAddTransient<ViewModelFactory>();
        
        return services;
    }
}
```

### Service Layer Design Patterns

#### Category-Based Service Organization (UNIVERSAL PATTERN)
```csharp
// CORRECT: Category-based service consolidation in single files
// File: Services/ApplicationCore.cs
namespace YourProject.Services
{
    public interface IConfigurationService { /* Configuration operations */ }
    public class ConfigurationService : IConfigurationService { /* Implementation */ }
    
    public interface IApplicationStateService { /* State management */ }
    public class ApplicationStateService : IApplicationStateService { /* Implementation */ }
    
    public static class ApplicationConfiguration
    {
        public const int DefaultTimeoutSeconds = 30;
        public const string DefaultTheme = "Light";
        // Configuration constants
    }
}
```

#### Service Result Pattern (MANDATORY)
```csharp
// Universal service result pattern
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
```

---

## 🔍 Error Handling and Logging Architecture

### Centralized Error Handling (UNIVERSAL PATTERN)
```csharp
// Universal error handling pattern
public static class ErrorHandling
{
    private static readonly ILogger Logger = CreateLogger();
    
    public static async Task HandleErrorAsync(Exception exception, string context)
    {
        // Structured logging
        Logger.LogError(exception, "Error in {Context}: {Message}", context, exception.Message);
        
        // Application-specific error logging
        await LogErrorToSystemAsync(exception, context);
        
        // User notification
        await ShowUserFriendlyNotificationAsync(GetUserFriendlyMessage(exception));
    }

    private static async Task LogErrorToSystemAsync(Exception exception, string context)
    {
        try
        {
            // TODO: Implement your error logging system
            // Could be database, file, external service, etc.
            await Task.Delay(1); // Placeholder
        }
        catch (Exception logEx)
        {
            Logger.LogCritical(logEx, "Failed to log error to system: {OriginalError}", exception.Message);
        }
    }
    
    private static string GetUserFriendlyMessage(Exception exception) =>
        exception switch
        {
            ArgumentException => "Invalid input provided. Please check your data and try again.",
            UnauthorizedAccessException => "You don't have permission to perform this operation.",
            TimeoutException => "The operation took too long to complete. Please try again.",
            _ => "An unexpected error occurred. Please contact support if the problem persists."
        };
        
    private static async Task ShowUserFriendlyNotificationAsync(string message)
    {
        // TODO: Integrate with your UI notification system
        await Task.Delay(1); // Placeholder
    }
    
    private static ILogger CreateLogger()
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger("ErrorHandling");
    }
}
```

### Structured Logging Standards (Microsoft.Extensions.Logging)
```csharp
// Universal logging patterns
public class UniversalService : IUniversalService
{
    private readonly ILogger<UniversalService> _logger;

    public UniversalService(ILogger<UniversalService> logger)
    {
        _logger = logger;
    }

    public async Task<ServiceResult<T>> PerformOperationAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting operation: {OperationName}", operationName);

        try
        {
            var result = await operation();
            
            _logger.LogInformation("Operation completed successfully: {OperationName} in {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
                
            return ServiceResult<T>.Success(result, "Operation completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed: {OperationName} after {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
                
            await ErrorHandling.HandleErrorAsync(ex, operationName);
            return ServiceResult<T>.Failure($"Operation {operationName} failed", ex);
        }
    }
}
```

---

## 💾 Data Access Architecture

### Universal Data Service Pattern
```csharp
// Universal data access pattern
public interface IDataService
{
    Task<ServiceResult<T>> GetAsync<T>(string key);
    Task<ServiceResult> SaveAsync<T>(string key, T data);
    Task<ServiceResult<List<T>>> GetAllAsync<T>();
    Task<ServiceResult> DeleteAsync(string key);
}

public class FileDataService : IDataService
{
    private readonly ILogger<FileDataService> _logger;
    private readonly string _dataDirectory;

    public FileDataService(ILogger<FileDataService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _dataDirectory = configuration.GetValue("DataDirectory", "Data");
        
        if (!Directory.Exists(_dataDirectory))
            Directory.CreateDirectory(_dataDirectory);
    }

    public async Task<ServiceResult<T>> GetAsync<T>(string key)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{key}.json");
            
            if (!File.Exists(filePath))
                return ServiceResult<T>.Failure("Data not found");

            var json = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<T>(json);
            
            return ServiceResult<T>.Success(data, "Data retrieved successfully");
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Get data for key: {key}");
            return ServiceResult<T>.Failure("Failed to retrieve data", ex);
        }
    }

    public async Task<ServiceResult> SaveAsync<T>(string key, T data)
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, $"{key}.json");
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            
            await File.WriteAllTextAsync(filePath, json);
            
            return ServiceResult.Success("Data saved successfully");
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Save data for key: {key}");
            return ServiceResult.Failure("Failed to save data", ex);
        }
    }

    // Implement other methods similarly...
}
```

---

## ⚡ Performance and Memory Management

### Async/Await Best Practices
```csharp
// Universal async patterns
public class UniversalAsyncService
{
    public async Task<ServiceResult<T>> ProcessDataAsync<T>(IEnumerable<T> items, Func<T, Task<T>> processor)
    {
        try
        {
            // Use ConfigureAwait(false) in service/library code
            var results = new List<T>();
            
            // Process items in parallel with controlled concurrency
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            var tasks = items.Select(async item =>
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    return await processor(item).ConfigureAwait(false);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var processedItems = await Task.WhenAll(tasks).ConfigureAwait(false);
            return ServiceResult<T[]>.Success(processedItems);
        }
        catch (Exception ex)
        {
            return ServiceResult<T[]>.Failure("Batch processing failed", ex);
        }
    }

    public async Task<ServiceResult> ProcessWithTimeoutAsync(Func<Task> operation, TimeSpan timeout)
    {
        try
        {
            using var cts = new CancellationTokenSource(timeout);
            await operation().WaitAsync(cts.Token).ConfigureAwait(false);
            return ServiceResult.Success();
        }
        catch (TimeoutException)
        {
            return ServiceResult.Failure("Operation timed out");
        }
        catch (Exception ex)
        {
            return ServiceResult.Failure("Operation failed", ex);
        }
    }
}
```

### Memory Management and Disposal
```csharp
// Universal disposal pattern
public class UniversalResourceManager : IDisposable, IAsyncDisposable
{
    private readonly List<IDisposable> _disposables = new();
    private readonly List<IAsyncDisposable> _asyncDisposables = new();
    private bool _disposed = false;

    public void RegisterDisposable(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }

    public void RegisterAsyncDisposable(IAsyncDisposable disposable)
    {
        _asyncDisposables.Add(disposable);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            foreach (var disposable in _disposables)
            {
                try
                {
                    disposable?.Dispose();
                }
                catch (Exception ex)
                {
                    // Log disposal errors but don't throw
                    Console.WriteLine($"Error disposing resource: {ex.Message}");
                }
            }
            
            _disposed = true;
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        foreach (var disposable in _asyncDisposables)
        {
            try
            {
                if (disposable != null)
                    await disposable.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log disposal errors but don't throw
                Console.WriteLine($"Error disposing async resource: {ex.Message}");
            }
        }
    }
}
```

---

## 🔧 Configuration and Environment Management

### Universal Configuration Pattern
```csharp
// Universal configuration service implementation
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GetConnectionString(string name)
    {
        var connectionString = _configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Connection string '{Name}' not found", name);
            return string.Empty;
        }
        return connectionString;
    }

    public T GetSection<T>(string sectionName) where T : new()
    {
        try
        {
            var section = _configuration.GetSection(sectionName);
            return section.Get<T>() ?? new T();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bind configuration section '{SectionName}'", sectionName);
            return new T();
        }
    }

    public string GetValue(string key, string defaultValue = "")
    {
        return _configuration.GetValue(key, defaultValue);
    }

    public bool IsEnvironment(string environmentName)
    {
        var currentEnvironment = _configuration.GetValue("DOTNET_ENVIRONMENT", "Production");
        return string.Equals(currentEnvironment, environmentName, StringComparison.OrdinalIgnoreCase);
    }
}
```

### Environment-Specific Configuration
```csharp
// Universal Program.cs configuration setup
public static class Program
{
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

        // Register universal services
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Services.AddViewModels();
        builder.Services.AddLogging(builder.Configuration);

        var app = builder.Build();
        
        // Configure Avalonia application
        BuildAvaloniaApp(app.Services).StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp(IServiceProvider services)
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseServiceProvider(services);
}
```

---

## 📚 Related Universal Documentation

- **MVVM Patterns**: [Universal MVVM Community Toolkit Implementation](./mvvm-community-toolkit.instructions.md)
- **UI Guidelines**: [Universal Avalonia UI Patterns](./avalonia-ui-guidelines.instructions.md)
- **Service Architecture**: [Universal Service Layer Design](./service-architecture.instructions.md)
- **Testing Procedures**: [Universal Testing Strategies](./testing-procedures.instructions.md)

---

**Document Status**: ✅ Complete Universal Architecture Reference  
**Framework Version**: .NET 8.0 + Avalonia UI 11.3.4  
**Last Updated**: 2025-01-27  
**Maintained By**: Universal Template Extraction Project