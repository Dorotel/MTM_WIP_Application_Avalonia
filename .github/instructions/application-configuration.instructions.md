# Application Configuration - MTM WIP Application Instructions

**Framework**: .NET 8 with Microsoft Extensions  
**Pattern**: Configuration and Dependency Injection  
**Created**: 2025-09-14  

---

## 🎯 Core Application Configuration Patterns

### Program.cs Bootstrap Pattern
```csharp
// MTM application startup and service configuration
public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = CreateApplicationBuilder(args);
            var app = builder.Build();
            
            ConfigureApplication(app);
            app.Run();
        }
        catch (Exception ex)
        {
            HandleStartupException(ex);
        }
    }
    
    private static HostApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Configure services
        ConfigureConfiguration(builder);
        ConfigureServices(builder.Services, builder.Configuration);
        ConfigureLogging(builder);
        
        return builder;
    }
    
    private static void ConfigureConfiguration(HostApplicationBuilder builder)
    {
        // Manufacturing-specific configuration sources
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("mtm-settings.json", optional: true, reloadOnChange: true) // Manufacturing-specific settings
            .AddEnvironmentVariables("MTM_")
            .AddCommandLine(args);
    }
    
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register MTM services with manufacturing context
        services.AddMTMServices(configuration);
        
        // Register ViewModels for dependency injection
        services.AddMTMViewModels();
        
        // Register Views
        services.AddMTMViews();
        
        // Configure application options
        services.Configure<MTMApplicationOptions>(configuration.GetSection("MTMSettings"));
        services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseSettings"));
        services.Configure<LoggingOptions>(configuration.GetSection("Logging"));
    }
    
    private static void ConfigureLogging(HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddFile(builder.Configuration.GetSection("Logging:File"));
        
        // Manufacturing-specific logging
        builder.Logging.AddFilter("MTM_WIP_Application_Avalonia", LogLevel.Debug);
        builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    }
    
    private static void ConfigureApplication(IHost app)
    {
        // Initialize manufacturing data services
        InitializeManufacturingServices(app.Services);
        
        // Start Avalonia application
        StartAvaloniaApplication(app.Services);
    }
    
    private static void InitializeManufacturingServices(IServiceProvider services)
    {
        // Ensure database connectivity for manufacturing operations
        var databaseService = services.GetRequiredService<IDatabaseService>();
        databaseService.ValidateConnectionAsync().GetAwaiter().GetResult();
        
        // Initialize master data cache for manufacturing
        var masterDataService = services.GetRequiredService<IMasterDataService>();
        masterDataService.PreloadCacheAsync().GetAwaiter().GetResult();
    }
    
    private static void StartAvaloniaApplication(IServiceProvider services)
    {
        // Start Avalonia with dependency injection
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseServiceProvider(services)
            .StartWithClassicDesktopLifetime(Environment.GetCommandLineArgs());
    }
    
    private static void HandleStartupException(Exception ex)
    {
        // Manufacturing-grade startup error handling
        Console.WriteLine($"Fatal startup error: {ex.Message}");
        
        try
        {
            // Log to file if possible
            File.WriteAllText("startup_error.log", 
                $"Startup Error {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{ex}");
        }
        catch
        {
            // Ignore file logging errors during startup failure
        }
        
        Environment.Exit(-1);
    }
}
```

### Service Registration Extension Pattern
```csharp
// Extensions/ServiceCollectionExtensions.cs - Manufacturing service registration
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Core services (Singleton - application lifetime)
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IThemeService, ThemeService>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
        
        // Database and data services (Scoped - per operation)
        services.TryAddScoped<IDatabaseService, DatabaseService>();
        services.TryAddScoped<IInventoryService, InventoryService>();
        services.TryAddScoped<ITransactionService, TransactionService>();
        services.TryAddScoped<IMasterDataService, MasterDataService>();
        services.TryAddScoped<IQuickButtonsService, QuickButtonsService>();
        
        // Manufacturing domain services
        services.TryAddScoped<IManufacturingValidationService, ManufacturingValidationService>();
        services.TryAddScoped<IPartValidationService, PartValidationService>();
        services.TryAddScoped<IOperationValidationService, OperationValidationService>();
        
        // Infrastructure services
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        services.TryAddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        
        // HTTP clients for external integrations
        services.AddHttpClient<IExternalSystemClient, ExternalSystemClient>(client =>
        {
            var baseUrl = configuration["ExternalSystems:BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        // Background services for manufacturing operations
        services.AddHostedService<ManufacturingDataSyncService>();
        services.AddHostedService<InventoryCleanupService>();
        
        return services;
    }
    
    public static IServiceCollection AddMTMViewModels(this IServiceCollection services)
    {
        // Register all ViewModels as Transient for fresh instances
        services.TryAddTransient<MainWindowViewModel>();
        services.TryAddTransient<InventoryTabViewModel>();
        services.TryAddTransient<QuickButtonsTabViewModel>();
        services.TryAddTransient<RemoveTabViewModel>();
        services.TryAddTransient<AdvancedRemoveViewModel>();
        services.TryAddTransient<TransactionHistoryTabViewModel>();
        services.TryAddTransient<SettingsFormViewModel>();
        
        return services;
    }
    
    public static IServiceCollection AddMTMViews(this IServiceCollection services)
    {
        // Register Views as Transient (typically created via DI)
        services.TryAddTransient<MainWindow>();
        services.TryAddTransient<InventoryTabView>();
        services.TryAddTransient<QuickButtonsTabView>();
        services.TryAddTransient<RemoveTabView>();
        services.TryAddTransient<AdvancedRemoveView>();
        
        return services;
    }
}
```

---

## 🏭 Manufacturing-Specific Configuration Patterns

### MTM Application Options
```csharp
// Configuration model for manufacturing-specific settings
public class MTMApplicationOptions
{
    public const string SectionName = "MTMSettings";
    
    // Manufacturing operation settings
    public string DefaultOperation { get; set; } = "90"; // Receiving operation
    public string DefaultLocation { get; set; } = "";
    public int DefaultQuantity { get; set; } = 1;
    
    // User interface settings
    public string DefaultTheme { get; set; } = "MTM_Blue";
    public bool ShowAdvancedFeatures { get; set; } = false;
    public bool EnableAutoSave { get; set; } = true;
    public int AutoSaveIntervalMinutes { get; set; } = 5;
    
    // Manufacturing workflow settings
    public bool EnableQuickButtons { get; set; } = true;
    public int MaxQuickButtons { get; set; } = 20;
    public int MaxRecentTransactions { get; set; } = 100;
    public bool EnableBarcodeScanning { get; set; } = false;
    
    // Performance settings
    public int DatabaseTimeoutSeconds { get; set; } = 30;
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
    public int MaxCacheSize { get; set; } = 10000;
    public TimeSpan CacheExpirationTime { get; set; } = TimeSpan.FromMinutes(5);
    
    // Validation settings
    public int MaxPartIdLength { get; set; } = 50;
    public int MinQuantity { get; set; } = 1;
    public int MaxQuantity { get; set; } = 999999;
    public bool StrictPartIdValidation { get; set; } = true;
    public bool RequireLocationValidation { get; set; } = true;
    
    // Integration settings
    public bool EnableExternalSystemIntegration { get; set; } = false;
    public string ExternalSystemApiKey { get; set; } = "";
    public TimeSpan ExternalSystemTimeout { get; set; } = TimeSpan.FromSeconds(30);
    
    // Logging and audit settings
    public bool EnableAuditLogging { get; set; } = true;
    public bool LogDatabaseOperations { get; set; } = false;
    public bool EnablePerformanceMonitoring { get; set; } = false;
    
    // Manufacturing shift settings
    public TimeSpan ShiftStartTime { get; set; } = TimeSpan.FromHours(6); // 6 AM
    public TimeSpan ShiftDuration { get; set; } = TimeSpan.FromHours(8);
    public int NumberOfShifts { get; set; } = 3;
    
    // Backup and recovery settings
    public bool EnableAutoBackup { get; set; } = false;
    public string BackupDirectory { get; set; } = "Backups";
    public TimeSpan BackupInterval { get; set; } = TimeSpan.FromHours(4);
    public int MaxBackupFiles { get; set; } = 10;
}
```

### Database Configuration Options
```csharp
// Database-specific configuration for manufacturing operations
public class DatabaseOptions
{
    public const string SectionName = "DatabaseSettings";
    
    public string ConnectionString { get; set; } = "";
    public string BackupConnectionString { get; set; } = "";
    
    // Connection pool settings for manufacturing load
    public int MinPoolSize { get; set; } = 5;
    public int MaxPoolSize { get; set; } = 50;
    public int ConnectionLifetime { get; set; } = 300; // 5 minutes
    public int ConnectionTimeout { get; set; } = 30;
    public int CommandTimeout { get; set; } = 120;
    
    // Manufacturing-specific database settings
    public bool EnableConnectionRetry { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
    
    // Performance optimization
    public bool UseCompression { get; set; } = false;
    public bool AllowBatch { get; set; } = true;
    public bool UseAffectedRows { get; set; } = true;
    
    // Security settings
    public bool UseSsl { get; set; } = false;
    public bool ValidateServerCertificate { get; set; } = true;
    
    // Monitoring and diagnostics
    public bool EnableSqlLogging { get; set; } = false;
    public bool EnablePerformanceCounters { get; set; } = false;
    public TimeSpan SlowQueryThreshold { get; set; } = TimeSpan.FromSeconds(5);
}
```

### Configuration Service Implementation
```csharp
// Service for accessing manufacturing configuration
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<MTMApplicationOptions> _mtmOptions;
    private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;
    private readonly ILogger<ConfigurationService> _logger;
    
    public ConfigurationService(
        IConfiguration configuration,
        IOptionsMonitor<MTMApplicationOptions> mtmOptions,
        IOptionsMonitor<DatabaseOptions> databaseOptions,
        ILogger<ConfigurationService> logger)
    {
        _configuration = configuration;
        _mtmOptions = mtmOptions;
        _databaseOptions = databaseOptions;
        _logger = logger;
        
        // Monitor configuration changes
        _mtmOptions.OnChange(OnMTMOptionsChanged);
        _databaseOptions.OnChange(OnDatabaseOptionsChanged);
    }
    
    // Manufacturing settings access
    public MTMApplicationOptions GetMTMSettings() => _mtmOptions.CurrentValue;
    public DatabaseOptions GetDatabaseSettings() => _databaseOptions.CurrentValue;
    
    public string GetConnectionString(string name = "DefaultConnection")
    {
        var connectionString = _configuration.GetConnectionString(name);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("Connection string '{Name}' not found or empty", name);
            throw new InvalidOperationException($"Connection string '{name}' is not configured");
        }
        
        return connectionString;
    }
    
    public T GetSetting<T>(string key, T defaultValue = default!)
    {
        try
        {
            var value = _configuration[key];
            if (value == null)
                return defaultValue;
            
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading setting '{Key}', using default value", key);
            return defaultValue;
        }
    }
    
    public bool IsFeatureEnabled(string featureName)
    {
        return GetSetting($"Features:{featureName}", false);
    }
    
    public string GetManufacturingWorkflowSetting(string settingName)
    {
        return GetSetting($"ManufacturingWorkflow:{settingName}", "");
    }
    
    // Configuration change handling for manufacturing operations
    private void OnMTMOptionsChanged(MTMApplicationOptions newOptions)
    {
        _logger.LogInformation("MTM application options changed");
        
        // Validate manufacturing settings
        ValidateManufacturingSettings(newOptions);
        
        // Notify services of configuration changes
        PublishConfigurationChanged(nameof(MTMApplicationOptions), newOptions);
    }
    
    private void OnDatabaseOptionsChanged(DatabaseOptions newOptions)
    {
        _logger.LogInformation("Database options changed");
        
        // Validate database settings
        ValidateDatabaseSettings(newOptions);
        
        // Update connection pool if needed
        UpdateConnectionPoolSettings(newOptions);
    }
    
    private void ValidateManufacturingSettings(MTMApplicationOptions options)
    {
        if (options.DefaultQuantity < options.MinQuantity || options.DefaultQuantity > options.MaxQuantity)
        {
            _logger.LogWarning("Default quantity {DefaultQuantity} is outside valid range ({MinQuantity}-{MaxQuantity})", 
                options.DefaultQuantity, options.MinQuantity, options.MaxQuantity);
        }
        
        if (!IsValidOperationNumber(options.DefaultOperation))
        {
            _logger.LogWarning("Invalid default operation number: {DefaultOperation}", options.DefaultOperation);
        }
        
        if (options.AutoSaveIntervalMinutes < 1 || options.AutoSaveIntervalMinutes > 60)
        {
            _logger.LogWarning("Auto-save interval {Interval} minutes is outside recommended range (1-60)", 
                options.AutoSaveIntervalMinutes);
        }
    }
    
    private void ValidateDatabaseSettings(DatabaseOptions options)
    {
        if (options.MaxPoolSize < options.MinPoolSize)
        {
            _logger.LogError("MaxPoolSize ({MaxPoolSize}) cannot be less than MinPoolSize ({MinPoolSize})", 
                options.MaxPoolSize, options.MinPoolSize);
        }
        
        if (options.ConnectionTimeout < 10)
        {
            _logger.LogWarning("Connection timeout {Timeout} seconds is very low for manufacturing operations", 
                options.ConnectionTimeout);
        }
    }
    
    private bool IsValidOperationNumber(string operation)
    {
        return int.TryParse(operation, out var number) && number >= 90 && number <= 999;
    }
    
    private void UpdateConnectionPoolSettings(DatabaseOptions options)
    {
        // Update connection pool configuration
        _logger.LogInformation("Updating connection pool: Min={MinPoolSize}, Max={MaxPoolSize}", 
            options.MinPoolSize, options.MaxPoolSize);
    }
    
    private void PublishConfigurationChanged(string sectionName, object newOptions)
    {
        // Publish configuration change event for other services
        var eventArgs = new ConfigurationChangedEventArgs(sectionName, newOptions);
        ConfigurationChanged?.Invoke(this, eventArgs);
    }
    
    public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;
}

public class ConfigurationChangedEventArgs : EventArgs
{
    public string SectionName { get; }
    public object NewConfiguration { get; }
    
    public ConfigurationChangedEventArgs(string sectionName, object newConfiguration)
    {
        SectionName = sectionName;
        NewConfiguration = newConfiguration;
    }
}
```

---

## ❌ Anti-Patterns (Avoid These)

### Configuration Access Anti-Patterns
```csharp
// ❌ WRONG: Direct configuration access in business logic
public class BadInventoryService
{
    public async Task ProcessInventoryAsync(string partId)
    {
        // BAD: Hardcoded configuration values
        var maxQuantity = 1000;
        var timeout = TimeSpan.FromSeconds(30);
        
        // BAD: Direct file access for configuration
        var config = File.ReadAllText("config.json");
        var settings = JsonSerializer.Deserialize<Settings>(config);
        
        // BAD: Environment variable access in business logic
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    }
}

// ✅ CORRECT: Dependency injection with options pattern
public class GoodInventoryService
{
    private readonly IOptionsMonitor<MTMApplicationOptions> _options;
    private readonly IDatabaseService _databaseService;
    
    public GoodInventoryService(
        IOptionsMonitor<MTMApplicationOptions> options,
        IDatabaseService databaseService)
    {
        _options = options;
        _databaseService = databaseService;
    }
    
    public async Task ProcessInventoryAsync(string partId)
    {
        var settings = _options.CurrentValue;
        
        // GOOD: Use injected configuration through options pattern
        var maxQuantity = settings.MaxQuantity;
        var timeout = TimeSpan.FromSeconds(settings.DatabaseTimeoutSeconds);
        
        // GOOD: Use injected database service
        await _databaseService.ProcessAsync(partId, maxQuantity);
    }
}
```

### Service Registration Anti-Patterns
```csharp
// ❌ WRONG: Improper service lifetimes and registration
public static class BadServiceRegistration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // BAD: Singleton for services that should be scoped
        services.AddSingleton<IDatabaseService, DatabaseService>();
        
        // BAD: Creating instances directly instead of using DI
        var config = new ConfigurationBuilder().Build();
        services.AddSingleton(new DatabaseService(config));
        
        // BAD: Not checking for existing registrations
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IInventoryService, AnotherInventoryService>(); // Overwrites first registration
        
        // BAD: Circular dependencies
        services.AddScoped<IServiceA, ServiceA>();
        services.AddScoped<IServiceB, ServiceB>(); // Where ServiceA depends on ServiceB and vice versa
    }
}

// ✅ CORRECT: Proper service registration patterns
public static class GoodServiceRegistration
{
    public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // GOOD: Appropriate lifetimes
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddScoped<IDatabaseService, DatabaseService>();
        services.TryAddTransient<IInventoryService, InventoryService>();
        
        // GOOD: Options pattern configuration
        services.Configure<MTMApplicationOptions>(configuration.GetSection(MTMApplicationOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        
        // GOOD: Factory patterns for complex object creation
        services.AddSingleton<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
        
        // GOOD: Conditional registration based on configuration
        if (configuration.GetValue<bool>("Features:EnableExternalIntegration"))
        {
            services.TryAddScoped<IExternalSystemClient, ExternalSystemClient>();
        }
        else
        {
            services.TryAddScoped<IExternalSystemClient, NoOpExternalSystemClient>();
        }
        
        return services;
    }
}
```

### Configuration Hot-Reload Anti-Patterns
```csharp
// ❌ WRONG: Not handling configuration changes
public class StaticConfigurationService
{
    // BAD: Static configuration that never updates
    private static readonly MTMApplicationOptions _options = LoadOptions();
    
    public static MTMApplicationOptions GetOptions() => _options;
    
    private static MTMApplicationOptions LoadOptions()
    {
        // BAD: Load once and never update
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        return config.GetSection("MTMSettings").Get<MTMApplicationOptions>();
    }
}

// ✅ CORRECT: Reactive configuration that handles hot-reload
public class ReactiveConfigurationService : IDisposable
{
    private readonly IOptionsMonitor<MTMApplicationOptions> _optionsMonitor;
    private readonly IDisposable _changeListener;
    
    public ReactiveConfigurationService(IOptionsMonitor<MTMApplicationOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        
        // GOOD: Listen for configuration changes
        _changeListener = _optionsMonitor.OnChange(OnConfigurationChanged);
    }
    
    public MTMApplicationOptions CurrentOptions => _optionsMonitor.CurrentValue;
    
    private void OnConfigurationChanged(MTMApplicationOptions newOptions)
    {
        // GOOD: Handle configuration changes appropriately
        ValidateNewConfiguration(newOptions);
        NotifyServicesOfChange(newOptions);
    }
    
    public void Dispose()
    {
        _changeListener?.Dispose();
    }
}
```

---

## 🔧 Manufacturing Configuration Troubleshooting

### Issue: Configuration Not Loading
**Symptoms**: Default values used instead of configured values

**Solution**: Verify configuration file paths and structure
```csharp
// ✅ CORRECT: Configuration troubleshooting
public static void DiagnoseConfiguration(IConfiguration configuration)
{
    Console.WriteLine("Configuration Sources:");
    if (configuration is IConfigurationRoot root)
    {
        foreach (var provider in root.Providers)
        {
            Console.WriteLine($"  {provider.GetType().Name}");
            if (provider is FileConfigurationProvider fileProvider)
            {
                Console.WriteLine($"    Source: {fileProvider.Source.Path}");
                Console.WriteLine($"    Optional: {fileProvider.Source.Optional}");
                Console.WriteLine($"    Exists: {File.Exists(fileProvider.Source.Path)}");
            }
        }
    }
    
    // Test specific settings
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine($"Connection String: {(connectionString != null ? "Found" : "Missing")}");
    
    var mtmSection = configuration.GetSection("MTMSettings");
    Console.WriteLine($"MTM Settings Section Exists: {mtmSection.Exists()}");
}
```

### Issue: Service Resolution Failures
**Symptoms**: DI container cannot resolve services

**Solution**: Check service registration order and dependencies
```csharp
// ✅ CORRECT: Service resolution diagnostics
public static void DiagnoseServiceRegistration(IServiceCollection services)
{
    var serviceProvider = services.BuildServiceProvider();
    
    try
    {
        var configService = serviceProvider.GetRequiredService<IConfigurationService>();
        Console.WriteLine("✓ Configuration service resolved");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Configuration service failed: {ex.Message}");
    }
    
    try
    {
        var databaseService = serviceProvider.GetRequiredService<IDatabaseService>();
        Console.WriteLine("✓ Database service resolved");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Database service failed: {ex.Message}");
    }
    
    // Check for circular dependencies
    CheckForCircularDependencies(services);
}

private static void CheckForCircularDependencies(IServiceCollection services)
{
    var serviceTypes = services.Select(s => s.ServiceType).ToHashSet();
    
    foreach (var service in services)
    {
        if (service.ImplementationType != null)
        {
            var constructors = service.ImplementationType.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                foreach (var param in parameters)
                {
                    if (serviceTypes.Contains(param.ParameterType))
                    {
                        Console.WriteLine($"{service.ServiceType.Name} depends on {param.ParameterType.Name}");
                    }
                }
            }
        }
    }
}
```

### Issue: Configuration Validation Failures
**Symptoms**: Application starts but behaves incorrectly due to invalid configuration

**Solution**: Implement configuration validation
```csharp
// ✅ CORRECT: Configuration validation
public class MTMConfigurationValidator : IValidateOptions<MTMApplicationOptions>
{
    public ValidateOptionsResult Validate(string name, MTMApplicationOptions options)
    {
        var failures = new List<string>();
        
        // Validate manufacturing settings
        if (options.DefaultQuantity < 1 || options.DefaultQuantity > 999999)
        {
            failures.Add($"DefaultQuantity must be between 1 and 999999, got {options.DefaultQuantity}");
        }
        
        if (!IsValidOperationNumber(options.DefaultOperation))
        {
            failures.Add($"DefaultOperation must be a valid operation number, got '{options.DefaultOperation}'");
        }
        
        if (options.AutoSaveIntervalMinutes < 1 || options.AutoSaveIntervalMinutes > 1440)
        {
            failures.Add($"AutoSaveIntervalMinutes must be between 1 and 1440, got {options.AutoSaveIntervalMinutes}");
        }
        
        if (options.MaxQuickButtons < 1 || options.MaxQuickButtons > 100)
        {
            failures.Add($"MaxQuickButtons must be between 1 and 100, got {options.MaxQuickButtons}");
        }
        
        return failures.Any() 
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
    
    private bool IsValidOperationNumber(string operation)
    {
        return int.TryParse(operation, out var number) && number >= 90 && number <= 999;
    }
}

// Register validation in DI container
services.AddSingleton<IValidateOptions<MTMApplicationOptions>, MTMConfigurationValidator>();
```

---

## 🧪 Configuration Testing Patterns

### Unit Testing Configuration
```csharp
[TestFixture]
public class ConfigurationServiceTests
{
    private IConfiguration _configuration;
    private IServiceProvider _serviceProvider;
    
    [SetUp]
    public void SetUp()
    {
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=test;Database=test;",
                ["MTMSettings:DefaultOperation"] = "100",
                ["MTMSettings:DefaultQuantity"] = "5",
                ["MTMSettings:MaxQuickButtons"] = "20"
            });
        
        _configuration = configBuilder.Build();
        
        var services = new ServiceCollection();
        services.AddSingleton(_configuration);
        services.Configure<MTMApplicationOptions>(_configuration.GetSection("MTMSettings"));
        services.AddScoped<IConfigurationService, ConfigurationService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [Test]
    public void ConfigurationService_GetConnectionString_ReturnsCorrectValue()
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        
        // Act
        var connectionString = configService.GetConnectionString();
        
        // Assert
        Assert.That(connectionString, Is.EqualTo("Server=test;Database=test;"));
    }
    
    [Test]
    public void ConfigurationService_GetMTMSettings_ReturnsCorrectValues()
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        
        // Act
        var settings = configService.GetMTMSettings();
        
        // Assert
        Assert.That(settings.DefaultOperation, Is.EqualTo("100"));
        Assert.That(settings.DefaultQuantity, Is.EqualTo(5));
        Assert.That(settings.MaxQuickButtons, Is.EqualTo(20));
    }
    
    [TestCase("InvalidSetting", "defaultValue", "defaultValue")]
    [TestCase("MTMSettings:DefaultOperation", "defaultValue", "100")]
    public void ConfigurationService_GetSetting_HandlesValidAndInvalidKeys(
        string key, string defaultValue, string expectedResult)
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        
        // Act
        var result = configService.GetSetting(key, defaultValue);
        
        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }
}
```

### Integration Testing with Real Configuration Files
```csharp
[TestFixture]
[Category("Integration")]
public class ConfigurationIntegrationTests
{
    private string _testConfigDirectory;
    
    [SetUp]
    public void SetUp()
    {
        _testConfigDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testConfigDirectory);
        
        // Create test configuration file
        var testConfig = new
        {
            ConnectionStrings = new
            {
                DefaultConnection = "Server=localhost;Database=mtm_test;Uid=test;Pwd=test;"
            },
            MTMSettings = new
            {
                DefaultOperation = "90",
                DefaultQuantity = 10,
                EnableAutoSave = true,
                AutoSaveIntervalMinutes = 5
            }
        };
        
        var configJson = JsonSerializer.Serialize(testConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(_testConfigDirectory, "appsettings.json"), configJson);
    }
    
    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testConfigDirectory))
        {
            Directory.Delete(_testConfigDirectory, true);
        }
    }
    
    [Test]
    public void Configuration_LoadFromFile_ParsesCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .SetBasePath(_testConfigDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.Configure<MTMApplicationOptions>(configuration.GetSection("MTMSettings"));
        services.AddScoped<IConfigurationService, ConfigurationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        var configService = serviceProvider.GetRequiredService<IConfigurationService>();
        var connectionString = configService.GetConnectionString();
        var mtmSettings = configService.GetMTMSettings();
        
        // Assert
        Assert.That(connectionString, Contains.Substring("mtm_test"));
        Assert.That(mtmSettings.DefaultOperation, Is.EqualTo("90"));
        Assert.That(mtmSettings.DefaultQuantity, Is.EqualTo(10));
        Assert.That(mtmSettings.EnableAutoSave, Is.True);
        Assert.That(mtmSettings.AutoSaveIntervalMinutes, Is.EqualTo(5));
    }
}
```

---

## 📚 Related Documentation

- **Service Architecture**: [Service Implementation Patterns](./service-architecture.instructions.md)
- **Dependency Injection**: [DI Container Patterns](./dotnet-architecture-good-practices.instructions.md)
- **Database Configuration**: [MySQL Database Patterns](./mysql-database-patterns.instructions.md)
- **MVVM Integration**: [MVVM Community Toolkit](./mvvm-community-toolkit.instructions.md)

---

**Document Status**: ✅ Complete Application Configuration Reference  
**Framework Version**: .NET 8 with Microsoft Extensions  
**Last Updated**: 2025-09-14  
**Configuration Owner**: MTM Development Team

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
