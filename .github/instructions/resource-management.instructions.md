# Resource Management - MTM WIP Application Instructions

**Framework**: Avalonia UI 11.3.4  
**Pattern**: Resource Dictionary and Theme Management  
**Created**: 2025-09-14  

---

## 🎯 Core Resource Management Patterns

### Resource Dictionary Organization
```csharp
// App.axaml - Main resource dictionary structure
<Application x:Class="MTM_WIP_Application_Avalonia.App"
             xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:MTM_WIP_Application_Avalonia">
    
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- Core Avalonia resources -->
                <ResourceInclude Source="avares://Avalonia.Themes.Fluent/FluentTheme.xaml" />
                
                <!-- MTM core styles -->
                <ResourceInclude Source="/Resources/Styles/MTMCore.axaml" />
                
                <!-- MTM theme resources -->
                <ResourceInclude Source="/Resources/Themes/MTMBlue.axaml" />
                <ResourceInclude Source="/Resources/Themes/MTMGreen.axaml" />
                <ResourceInclude Source="/Resources/Themes/MTMRed.axaml" />
                <ResourceInclude Source="/Resources/Themes/MTMDark.axaml" />
                
                <!-- Component-specific styles -->
                <ResourceInclude Source="/Resources/Styles/Buttons.axaml" />
                <ResourceInclude Source="/Resources/Styles/DataGrids.axaml" />
                <ResourceInclude Source="/Resources/Styles/Forms.axaml" />
                <ResourceInclude Source="/Resources/Styles/CustomControls.axaml" />
                
                <!-- Manufacturing-specific resources -->
                <ResourceInclude Source="/Resources/Manufacturing/OperationColors.axaml" />
                <ResourceInclude Source="/Resources/Manufacturing/StatusIndicators.axaml" />
                
                <!-- Value converters -->
                <ResourceInclude Source="/Resources/Converters/Converters.axaml" />
            </ResourceDictionary.MergedDictionaries>
            
            <!-- Global application resources -->
            <local:ServiceLocator x:Key="ServiceLocator" />
            
            <!-- Manufacturing constants -->
            <x:String x:Key="ApplicationTitle">MTM WIP Application</x:String>
            <x:String x:Key="ApplicationVersion">1.0.0</x:String>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### Dynamic Theme Management Service
```csharp
// ThemeService implementation for MTM manufacturing themes
public class ThemeService : IThemeService, INotifyPropertyChanged
{
    private readonly Application _application;
    private readonly ILogger<ThemeService> _logger;
    private readonly IConfigurationService _configurationService;
    
    private string _currentTheme = "MTM_Blue";
    private readonly Dictionary<string, ResourceDictionary> _themeCache = new();
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public ThemeService(
        Application application,
        ILogger<ThemeService> logger,
        IConfigurationService configurationService)
    {
        _application = application;
        _logger = logger;
        _configurationService = configurationService;
        
        InitializeThemes();
        LoadSavedTheme();
    }
    
    public string CurrentTheme
    {
        get => _currentTheme;
        private set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged();
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(_currentTheme));
            }
        }
    }
    
    public IReadOnlyList<string> AvailableThemes { get; } = new[]
    {
        "MTM_Blue",
        "MTM_Green", 
        "MTM_Red",
        "MTM_Dark"
    };
    
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
    
    private void InitializeThemes()
    {
        // Pre-load all theme resources for fast switching
        foreach (var theme in AvailableThemes)
        {
            try
            {
                var themeUri = GetThemeResourceUri(theme);
                var themeResource = new ResourceInclude(themeUri)
                {
                    Source = themeUri
                };
                
                _themeCache[theme] = themeResource;
                _logger.LogDebug("Cached theme resource: {Theme}", theme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load theme resource: {Theme}", theme);
            }
        }
    }
    
    public async Task<bool> SetThemeAsync(string themeName)
    {
        if (!AvailableThemes.Contains(themeName))
        {
            _logger.LogWarning("Unknown theme requested: {Theme}", themeName);
            return false;
        }
        
        if (CurrentTheme == themeName)
        {
            return true; // Already set
        }
        
        try
        {
            // Apply theme on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                ApplyTheme(themeName);
            });
            
            CurrentTheme = themeName;
            
            // Save theme preference
            await SaveThemePreferenceAsync(themeName);
            
            _logger.LogInformation("Theme changed to: {Theme}", themeName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply theme: {Theme}", themeName);
            return false;
        }
    }
    
    private void ApplyTheme(string themeName)
    {
        if (!_themeCache.TryGetValue(themeName, out var themeResource))
        {
            throw new InvalidOperationException($"Theme '{themeName}' not found in cache");
        }
        
        // Remove current theme resources
        RemoveCurrentThemeResources();
        
        // Add new theme resources
        _application.Resources.MergedDictionaries.Add(themeResource);
        
        // Update theme-specific brush resources
        UpdateThemeSpecificResources(themeName);
    }
    
    private void RemoveCurrentThemeResources()
    {
        // Remove existing theme resources
        var themeDictionaries = _application.Resources.MergedDictionaries
            .Where(dict => IsThemeResourceDictionary(dict))
            .ToList();
        
        foreach (var dict in themeDictionaries)
        {
            _application.Resources.MergedDictionaries.Remove(dict);
        }
    }
    
    private bool IsThemeResourceDictionary(ResourceDictionary dictionary)
    {
        // Identify theme dictionaries by source URI pattern
        if (dictionary is ResourceInclude include && include.Source != null)
        {
            var path = include.Source.ToString();
            return path.Contains("/Resources/Themes/MTM");
        }
        
        return false;
    }
    
    private void UpdateThemeSpecificResources(string themeName)
    {
        // Update global theme-specific resources
        var themeColors = GetThemeColors(themeName);
        
        foreach (var (key, color) in themeColors)
        {
            _application.Resources[key] = new SolidColorBrush(color);
        }
    }
    
    private Dictionary<string, Color> GetThemeColors(string themeName)
    {
        return themeName switch
        {
            "MTM_Blue" => new Dictionary<string, Color>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = Color.FromRgb(0x00, 0x78, 0xD4),
                ["MTM_Shared_Logic.SecondaryAction"] = Color.FromRgb(0x10, 0x6E, 0xBE),
                ["MTM_Shared_Logic.MainBackground"] = Color.FromRgb(0xFA, 0xFA, 0xFA),
                ["MTM_Shared_Logic.ContentAreas"] = Color.FromRgb(0xFF, 0xFF, 0xFF)
            },
            "MTM_Green" => new Dictionary<string, Color>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = Color.FromRgb(0x4C, 0xAF, 0x50),
                ["MTM_Shared_Logic.SecondaryAction"] = Color.FromRgb(0x2E, 0x7D, 0x32),
                ["MTM_Shared_Logic.MainBackground"] = Color.FromRgb(0xF1, 0xF8, 0xE9),
                ["MTM_Shared_Logic.ContentAreas"] = Color.FromRgb(0xFF, 0xFF, 0xFF)
            },
            "MTM_Red" => new Dictionary<string, Color>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = Color.FromRgb(0xF4, 0x43, 0x36),
                ["MTM_Shared_Logic.SecondaryAction"] = Color.FromRgb(0xD3, 0x2F, 0x2F),
                ["MTM_Shared_Logic.MainBackground"] = Color.FromRgb(0xFF, 0xEB, 0xEE),
                ["MTM_Shared_Logic.ContentAreas"] = Color.FromRgb(0xFF, 0xFF, 0xFF)
            },
            "MTM_Dark" => new Dictionary<string, Color>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = Color.FromRgb(0x90, 0xCA, 0xF9),
                ["MTM_Shared_Logic.SecondaryAction"] = Color.FromRgb(0x64, 0xB5, 0xF6),
                ["MTM_Shared_Logic.MainBackground"] = Color.FromRgb(0x12, 0x12, 0x12),
                ["MTM_Shared_Logic.ContentAreas"] = Color.FromRgb(0x1E, 0x1E, 0x1E)
            },
            _ => throw new ArgumentException($"Unknown theme: {themeName}")
        };
    }
    
    private Uri GetThemeResourceUri(string theme)
    {
        return new Uri($"avares://MTM_WIP_Application_Avalonia/Resources/Themes/{theme}.axaml");
    }
    
    private void LoadSavedTheme()
    {
        try
        {
            var savedTheme = _configurationService.GetSetting("UI:Theme", "MTM_Blue");
            if (AvailableThemes.Contains(savedTheme))
            {
                // Don't await - this is synchronous initialization
                SetThemeAsync(savedTheme);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load saved theme, using default");
        }
    }
    
    private async Task SaveThemePreferenceAsync(string theme)
    {
        try
        {
            // Save to user settings file
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MTM_WIP_Application",
                "user-settings.json");
            
            var settings = await LoadUserSettingsAsync(settingsPath);
            settings["theme"] = theme;
            
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
            await File.WriteAllTextAsync(settingsPath, 
                JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save theme preference");
        }
    }
    
    private async Task<Dictionary<string, object>> LoadUserSettingsAsync(string settingsPath)
    {
        if (!File.Exists(settingsPath))
            return new Dictionary<string, object>();
        
        try
        {
            var json = await File.ReadAllTextAsync(settingsPath);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json) 
                ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ThemeChangedEventArgs : EventArgs
{
    public string NewTheme { get; }
    
    public ThemeChangedEventArgs(string newTheme)
    {
        NewTheme = newTheme;
    }
}
```

---

## 🏭 Manufacturing-Specific Resource Patterns

### Manufacturing Operation Color Resources
```xml
<!-- Resources/Manufacturing/OperationColors.axaml -->
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Manufacturing operation color coding -->
    <SolidColorBrush x:Key="Operation90Brush" Color="#2196F3" />  <!-- Receiving - Blue -->
    <SolidColorBrush x:Key="Operation100Brush" Color="#4CAF50" /> <!-- First Op - Green -->
    <SolidColorBrush x:Key="Operation110Brush" Color="#FF9800" /> <!-- Second Op - Orange -->
    <SolidColorBrush x:Key="Operation120Brush" Color="#F44336" /> <!-- Final Op - Red -->
    <SolidColorBrush x:Key="Operation130Brush" Color="#9C27B0" /> <!-- Shipping - Purple -->
    
    <!-- Manufacturing status indicators -->
    <SolidColorBrush x:Key="InStockBrush" Color="#4CAF50" />      <!-- Green for in stock -->
    <SolidColorBrush x:Key="LowStockBrush" Color="#FF9800" />     <!-- Orange for low stock -->
    <SolidColorBrush x:Key="OutOfStockBrush" Color="#F44336" />   <!-- Red for out of stock -->
    <SolidColorBrush x:Key="UnknownStockBrush" Color="#9E9E9E" /> <!-- Gray for unknown -->
    
    <!-- Transaction type colors -->
    <SolidColorBrush x:Key="TransactionInBrush" Color="#4CAF50" />      <!-- Green for incoming -->
    <SolidColorBrush x:Key="TransactionOutBrush" Color="#F44336" />     <!-- Red for outgoing -->
    <SolidColorBrush x:Key="TransactionTransferBrush" Color="#2196F3" /><!-- Blue for transfer -->
    <SolidColorBrush x:Key="TransactionAdjustBrush" Color="#FF9800" />  <!-- Orange for adjustment -->
    <SolidColorBrush x:Key="TransactionScrapBrush" Color="#795548" />   <!-- Brown for scrap -->
    
    <!-- Manufacturing priority levels -->
    <SolidColorBrush x:Key="HighPriorityBrush" Color="#D32F2F" />
    <SolidColorBrush x:Key="MediumPriorityBrush" Color="#F57C00" />
    <SolidColorBrush x:Key="LowPriorityBrush" Color="#388E3C" />
    
    <!-- Quality status colors -->
    <SolidColorBrush x:Key="QualityPassBrush" Color="#4CAF50" />
    <SolidColorBrush x:Key="QualityFailBrush" Color="#F44336" />
    <SolidColorBrush x:Key="QualityPendingBrush" Color="#FF9800" />
    <SolidColorBrush x:Key="QualityInspectionBrush" Color="#2196F3" />
</ResourceDictionary>
```

### Manufacturing Status Indicator Resources
```xml
<!-- Resources/Manufacturing/StatusIndicators.axaml -->
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Manufacturing status icons (using Unicode symbols) -->
    <x:String x:Key="ReceivingIcon">📦</x:String>
    <x:String x:Key="ProductionIcon">🔧</x:String>
    <x:String x:Key="QualityIcon">🔍</x:String>
    <x:String x:Key="ShippingIcon">🚚</x:String>
    <x:String x:Key="InStockIcon">✅</x:String>
    <x:String x:Key="LowStockIcon">⚠️</x:String>
    <x:String x:Key="OutOfStockIcon">❌</x:String>
    
    <!-- Transaction type icons -->
    <x:String x:Key="TransactionInIcon">⬇️</x:String>
    <x:String x:Key="TransactionOutIcon">⬆️</x:String>
    <x:String x:Key="TransactionTransferIcon">↔️</x:String>
    <x:String x:Key="TransactionAdjustIcon">⚖️</x:String>
    <x:String x:Key="TransactionScrapIcon">🗑️</x:String>
    
    <!-- Manufacturing operation templates -->
    <DataTemplate x:Key="OperationStatusTemplate" DataType="system:String">
        <StackPanel Orientation="Horizontal" Spacing="4">
            <TextBlock Text="{Binding Converter={StaticResource OperationIconConverter}}" />
            <TextBlock Text="{Binding}" />
            <Rectangle Width="12" Height="12" 
                       Fill="{Binding Converter={StaticResource OperationColorConverter}}" />
        </StackPanel>
    </DataTemplate>
    
    <DataTemplate x:Key="StockLevelTemplate" DataType="system:Int32">
        <StackPanel Orientation="Horizontal" Spacing="4">
            <TextBlock Text="{Binding Converter={StaticResource StockIconConverter}}" />
            <TextBlock Text="{Binding StringFormat=N0}" />
            <TextBlock Text="{Binding Converter={StaticResource StockLevelConverter}}" 
                       FontSize="10" Opacity="0.7" />
        </StackPanel>
    </DataTemplate>
</ResourceDictionary>
```

### Resource Loading Service
```csharp
// Service for dynamic resource loading and management
public class ResourceManagerService : IResourceManagerService
{
    private readonly Application _application;
    private readonly ILogger<ResourceManagerService> _logger;
    private readonly Dictionary<string, WeakReference<ResourceDictionary>> _resourceCache = new();
    
    public ResourceManagerService(Application application, ILogger<ResourceManagerService> logger)
    {
        _application = application;
        _logger = logger;
    }
    
    public async Task<ResourceDictionary> LoadResourceDictionaryAsync(Uri resourceUri)
    {
        var cacheKey = resourceUri.ToString();
        
        // Check cache first
        if (_resourceCache.TryGetValue(cacheKey, out var weakRef) && 
            weakRef.TryGetTarget(out var cachedDict))
        {
            return cachedDict;
        }
        
        try
        {
            // Load resource dictionary
            var resourceDict = await Task.Run(() =>
            {
                var include = new ResourceInclude(resourceUri)
                {
                    Source = resourceUri
                };
                return include;
            });
            
            // Cache with weak reference to allow GC when not in use
            _resourceCache[cacheKey] = new WeakReference<ResourceDictionary>(resourceDict);
            
            _logger.LogDebug("Loaded resource dictionary: {Uri}", resourceUri);
            return resourceDict;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load resource dictionary: {Uri}", resourceUri);
            throw;
        }
    }
    
    public T? GetResource<T>(string resourceKey) where T : class
    {
        try
        {
            if (_application.TryGetResource(resourceKey, out var resource) && resource is T typedResource)
            {
                return typedResource;
            }
            
            _logger.LogWarning("Resource not found: {Key}", resourceKey);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing resource: {Key}", resourceKey);
            return null;
        }
    }
    
    public void SetResource(string resourceKey, object resource)
    {
        try
        {
            _application.Resources[resourceKey] = resource;
            _logger.LogDebug("Set resource: {Key}", resourceKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting resource: {Key}", resourceKey);
            throw;
        }
    }
    
    public bool TryGetResource<T>(string resourceKey, out T? resource) where T : class
    {
        resource = GetResource<T>(resourceKey);
        return resource != null;
    }
    
    public IEnumerable<string> GetResourceKeys()
    {
        return _application.Resources.Keys.OfType<string>();
    }
    
    public void ClearResourceCache()
    {
        _resourceCache.Clear();
        GC.Collect(); // Force cleanup of weak references
        _logger.LogInformation("Resource cache cleared");
    }
    
    // Manufacturing-specific resource helpers
    public SolidColorBrush? GetOperationBrush(string operationNumber)
    {
        var resourceKey = $"Operation{operationNumber}Brush";
        return GetResource<SolidColorBrush>(resourceKey);
    }
    
    public string? GetOperationIcon(string operationNumber)
    {
        return operationNumber switch
        {
            "90" => GetResource<string>("ReceivingIcon"),
            "100" or "110" or "120" => GetResource<string>("ProductionIcon"),
            "130" => GetResource<string>("ShippingIcon"),
            _ => "❓"
        };
    }
    
    public SolidColorBrush? GetStockLevelBrush(int quantity)
    {
        var resourceKey = quantity switch
        {
            0 => "OutOfStockBrush",
            < 10 => "LowStockBrush",
            _ => "InStockBrush"
        };
        
        return GetResource<SolidColorBrush>(resourceKey);
    }
    
    public SolidColorBrush? GetTransactionTypeBrush(string transactionType)
    {
        var resourceKey = $"Transaction{transactionType}Brush";
        return GetResource<SolidColorBrush>(resourceKey);
    }
}
```

---

## ❌ Anti-Patterns (Avoid These)

### Resource Access Anti-Patterns
```csharp
// ❌ WRONG: Direct resource access without error handling
public class BadResourceAccess
{
    public void UpdateTheme()
    {
        // BAD: Direct resource access without null checking
        var brush = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
        
        // BAD: Hardcoded resource keys
        var color = Application.Current.Resources["#FF0078D4"];
        
        // BAD: Casting without type checking
        var template = (DataTemplate)Application.Current.Resources["ItemTemplate"];
    }
}

// ✅ CORRECT: Safe resource access with proper error handling
public class SafeResourceAccess
{
    private readonly IResourceManagerService _resourceManager;
    
    public SafeResourceAccess(IResourceManagerService resourceManager)
    {
        _resourceManager = resourceManager;
    }
    
    public void UpdateTheme()
    {
        // GOOD: Safe resource access with null checking
        var brush = _resourceManager.GetResource<SolidColorBrush>("MTM_Shared_Logic.PrimaryAction");
        if (brush != null)
        {
            // Use brush safely
        }
        
        // GOOD: Try pattern for optional resources
        if (_resourceManager.TryGetResource<DataTemplate>("ItemTemplate", out var template))
        {
            // Use template
        }
    }
}
```

### Resource Loading Performance Anti-Patterns
```csharp
// ❌ WRONG: Synchronous resource loading on UI thread
public class SynchronousResourceLoader
{
    public void LoadTheme(string themeName)
    {
        // BAD: Blocking UI thread with synchronous I/O
        var themeUri = new Uri($"/Resources/Themes/{themeName}.axaml", UriKind.Relative);
        var resource = new ResourceInclude(themeUri) { Source = themeUri };
        
        // BAD: Large resource loading without progress indication
        Application.Current.Resources.MergedDictionaries.Add(resource);
    }
}

// ✅ CORRECT: Asynchronous resource loading with progress
public class AsyncResourceLoader
{
    public async Task LoadThemeAsync(string themeName, IProgress<string>? progress = null)
    {
        progress?.Report($"Loading theme: {themeName}");
        
        // GOOD: Async resource loading
        var themeUri = new Uri($"/Resources/Themes/{themeName}.axaml", UriKind.Relative);
        var resource = await Task.Run(() => new ResourceInclude(themeUri) { Source = themeUri });
        
        // GOOD: UI update on UI thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Application.Current.Resources.MergedDictionaries.Add(resource);
        });
        
        progress?.Report($"Theme loaded: {themeName}");
    }
}
```

### Resource Memory Management Anti-Patterns
```csharp
// ❌ WRONG: Resource leaks and excessive memory usage
public class LeakyResourceManager
{
    private readonly Dictionary<string, ResourceDictionary> _cache = new(); // Strong references
    
    public ResourceDictionary LoadResource(Uri uri)
    {
        var key = uri.ToString();
        
        // BAD: Strong reference cache grows indefinitely
        if (!_cache.ContainsKey(key))
        {
            _cache[key] = new ResourceInclude(uri) { Source = uri };
        }
        
        return _cache[key];
    }
    
    // BAD: No cleanup or disposal
}

// ✅ CORRECT: Proper resource lifecycle management
public class ManagedResourceLoader : IDisposable
{
    private readonly Dictionary<string, WeakReference<ResourceDictionary>> _cache = new();
    private readonly Timer _cleanupTimer;
    
    public ManagedResourceLoader()
    {
        // GOOD: Periodic cleanup of unused resources
        _cleanupTimer = new Timer(CleanupCache, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    public ResourceDictionary LoadResource(Uri uri)
    {
        var key = uri.ToString();
        
        // GOOD: Check weak reference cache
        if (_cache.TryGetValue(key, out var weakRef) && weakRef.TryGetTarget(out var resource))
        {
            return resource;
        }
        
        // GOOD: Load and cache with weak reference
        var newResource = new ResourceInclude(uri) { Source = uri };
        _cache[key] = new WeakReference<ResourceDictionary>(newResource);
        
        return newResource;
    }
    
    private void CleanupCache(object? state)
    {
        // GOOD: Remove dead weak references
        var deadKeys = _cache.Where(kvp => !kvp.Value.TryGetTarget(out _)).Select(kvp => kvp.Key).ToList();
        
        foreach (var key in deadKeys)
        {
            _cache.Remove(key);
        }
    }
    
    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        _cache.Clear();
    }
}
```

---

## 🔧 Manufacturing Resource Troubleshooting

### Issue: Theme Not Applying Correctly
**Symptoms**: UI elements don't reflect theme changes

**Solution**: Verify resource dictionary merge order and key conflicts
```csharp
// ✅ CORRECT: Theme application diagnostics
public static void DiagnoseThemeApplication(Application app)
{
    Console.WriteLine("Merged Dictionaries:");
    for (int i = 0; i < app.Resources.MergedDictionaries.Count; i++)
    {
        var dict = app.Resources.MergedDictionaries[i];
        Console.WriteLine($"  [{i}] {dict.GetType().Name}");
        
        if (dict is ResourceInclude include && include.Source != null)
        {
            Console.WriteLine($"      Source: {include.Source}");
        }
        
        Console.WriteLine($"      Keys: {dict.Keys.Count}");
        
        // Check for key conflicts
        foreach (var key in dict.Keys)
        {
            if (key is string stringKey && stringKey.StartsWith("MTM_"))
            {
                Console.WriteLine($"        {stringKey}");
            }
        }
    }
    
    // Test specific theme resources
    var primaryBrush = app.TryGetResource("MTM_Shared_Logic.PrimaryAction", out var resource);
    Console.WriteLine($"Primary Action Brush Found: {primaryBrush}");
    if (resource is SolidColorBrush brush)
    {
        Console.WriteLine($"  Color: {brush.Color}");
    }
}
```

### Issue: Resource Loading Performance Problems
**Symptoms**: Application startup slow, theme switching laggy

**Solution**: Implement resource preloading and caching
```csharp
// ✅ CORRECT: Optimized resource preloading
public class OptimizedResourcePreloader
{
    private readonly Dictionary<string, Task<ResourceDictionary>> _preloadTasks = new();
    
    public async Task PreloadAllResourcesAsync()
    {
        var resourceUris = new[]
        {
            "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMBlue.axaml",
            "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMGreen.axaml",
            "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMRed.axaml",
            "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMDark.axaml"
        };
        
        // Start all preload tasks concurrently
        var preloadTasks = resourceUris.Select(uri => 
            Task.Run(() => PreloadResourceAsync(new Uri(uri)))
        ).ToArray();
        
        try
        {
            await Task.WhenAll(preloadTasks);
            Console.WriteLine("All resources preloaded successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Resource preloading failed: {ex.Message}");
        }
    }
    
    private async Task<ResourceDictionary> PreloadResourceAsync(Uri uri)
    {
        var key = uri.ToString();
        
        if (_preloadTasks.TryGetValue(key, out var existingTask))
        {
            return await existingTask;
        }
        
        var task = Task.Run(() =>
        {
            var resource = new ResourceInclude(uri) { Source = uri };
            // Force loading of all resources in the dictionary
            _ = resource.Keys.Count;
            return resource;
        });
        
        _preloadTasks[key] = task;
        return await task;
    }
}
```

### Issue: Cross-Platform Resource Compatibility
**Symptoms**: Resources work on one platform but fail on others

**Solution**: Implement platform-aware resource loading
```csharp
// ✅ CORRECT: Cross-platform resource handling
public class PlatformAwareResourceManager
{
    private readonly ILogger<PlatformAwareResourceManager> _logger;
    
    public PlatformAwareResourceManager(ILogger<PlatformAwareResourceManager> logger)
    {
        _logger = logger;
    }
    
    public async Task<ResourceDictionary> LoadPlatformResourceAsync(string basePath, string resourceName)
    {
        var platform = GetCurrentPlatform();
        var platformSpecificUri = GetPlatformSpecificUri(basePath, resourceName, platform);
        
        try
        {
            // Try platform-specific resource first
            return await LoadResourceSafeAsync(platformSpecificUri);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Platform-specific resource failed, falling back to generic: {Uri}", platformSpecificUri);
            
            // Fall back to generic resource
            var genericUri = GetGenericUri(basePath, resourceName);
            return await LoadResourceSafeAsync(genericUri);
        }
    }
    
    private string GetCurrentPlatform()
    {
        if (OperatingSystem.IsWindows()) return "Windows";
        if (OperatingSystem.IsMacOS()) return "macOS";
        if (OperatingSystem.IsLinux()) return "Linux";
        if (OperatingSystem.IsAndroid()) return "Android";
        return "Generic";
    }
    
    private Uri GetPlatformSpecificUri(string basePath, string resourceName, string platform)
    {
        return new Uri($"{basePath}/Platforms/{platform}/{resourceName}");
    }
    
    private Uri GetGenericUri(string basePath, string resourceName)
    {
        return new Uri($"{basePath}/{resourceName}");
    }
    
    private async Task<ResourceDictionary> LoadResourceSafeAsync(Uri uri)
    {
        return await Task.Run(() =>
        {
            try
            {
                return new ResourceInclude(uri) { Source = uri };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load resource: {Uri}", uri);
                throw;
            }
        });
    }
}
```

---

## 🧪 Resource Management Testing Patterns

### Unit Testing Resource Loading
```csharp
[TestFixture]
public class ResourceManagerServiceTests
{
    private Application _testApplication;
    private ResourceManagerService _resourceManager;
    
    [SetUp]
    public void SetUp()
    {
        // Create test application with minimal resources
        _testApplication = new Application();
        
        // Add test resources
        _testApplication.Resources["TestResource"] = "TestValue";
        _testApplication.Resources["TestBrush"] = new SolidColorBrush(Colors.Red);
        
        _resourceManager = new ResourceManagerService(_testApplication, Mock.Of<ILogger<ResourceManagerService>>());
    }
    
    [Test]
    public void GetResource_ExistingResource_ReturnsCorrectValue()
    {
        // Act
        var resource = _resourceManager.GetResource<string>("TestResource");
        
        // Assert
        Assert.That(resource, Is.EqualTo("TestValue"));
    }
    
    [Test]
    public void GetResource_NonExistentResource_ReturnsNull()
    {
        // Act
        var resource = _resourceManager.GetResource<string>("NonExistentResource");
        
        // Assert
        Assert.That(resource, Is.Null);
    }
    
    [Test]
    public void TryGetResource_ExistingResource_ReturnsTrueAndResource()
    {
        // Act
        var success = _resourceManager.TryGetResource<SolidColorBrush>("TestBrush", out var brush);
        
        // Assert
        Assert.That(success, Is.True);
        Assert.That(brush, Is.Not.Null);
        Assert.That(brush.Color, Is.EqualTo(Colors.Red));
    }
    
    [TestCase("90", "ReceivingIcon")]
    [TestCase("100", "ProductionIcon")]
    [TestCase("130", "ShippingIcon")]
    public void GetOperationIcon_ValidOperations_ReturnsCorrectIcon(string operation, string expectedResourceKey)
    {
        // Arrange
        _testApplication.Resources[expectedResourceKey] = "TestIcon";
        
        // Act
        var icon = _resourceManager.GetOperationIcon(operation);
        
        // Assert
        Assert.That(icon, Is.EqualTo("TestIcon"));
    }
}
```

### Integration Testing with Theme Service
```csharp
[TestFixture]
[Category("Integration")]
public class ThemeResourceIntegrationTests
{
    private Application _testApp;
    private ThemeService _themeService;
    private Mock<IConfigurationService> _mockConfig;
    
    [SetUp]
    public async Task SetUp()
    {
        _testApp = new Application();
        _mockConfig = new Mock<IConfigurationService>();
        _mockConfig.Setup(c => c.GetSetting("UI:Theme", "MTM_Blue")).Returns("MTM_Blue");
        
        _themeService = new ThemeService(
            _testApp,
            Mock.Of<ILogger<ThemeService>>(),
            _mockConfig.Object);
        
        // Allow initialization to complete
        await Task.Delay(100);
    }
    
    [Test]
    public async Task SetThemeAsync_ValidTheme_UpdatesApplicationResources()
    {
        // Act
        var result = await _themeService.SetThemeAsync("MTM_Green");
        
        // Assert
        Assert.That(result, Is.True);
        Assert.That(_themeService.CurrentTheme, Is.EqualTo("MTM_Green"));
        
        // Verify theme resources are loaded
        var primaryBrush = _testApp.TryGetResource("MTM_Shared_Logic.PrimaryAction", out var resource);
        Assert.That(primaryBrush, Is.True);
    }
    
    [Test]
    public async Task SetThemeAsync_InvalidTheme_ReturnsFalse()
    {
        // Act
        var result = await _themeService.SetThemeAsync("InvalidTheme");
        
        // Assert
        Assert.That(result, Is.False);
        Assert.That(_themeService.CurrentTheme, Is.EqualTo("MTM_Blue")); // Should remain unchanged
    }
    
    [Test]
    public void AvailableThemes_ContainsExpectedThemes()
    {
        // Assert
        var expectedThemes = new[] { "MTM_Blue", "MTM_Green", "MTM_Red", "MTM_Dark" };
        CollectionAssert.AreEquivalent(expectedThemes, _themeService.AvailableThemes);
    }
}
```

---

## 🔗 AXAML Resource Usage Examples

### Theme Resource References
```xml
<!-- Using theme-aware resources -->
<Button Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
        Foreground="{DynamicResource MTM_Shared_Logic.ContentAreas}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}">
    
    <!-- Manufacturing operation status display -->
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="{StaticResource ReceivingIcon}" />
        <TextBlock Text="Receive Parts" Margin="8,0,0,0" />
    </StackPanel>
</Button>
```

### Manufacturing Data Templates
```xml
<!-- Using manufacturing-specific templates -->
<ListBox ItemsSource="{Binding Transactions}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Border Background="{Binding TransactionType, Converter={StaticResource TransactionTypeBrushConverter}}"
                    Padding="8" Margin="2">
                <StackPanel>
                    <TextBlock Text="{Binding PartId}" FontWeight="Bold" />
                    <TextBlock>
                        <Run Text="{Binding TransactionType, Converter={StaticResource TransactionTypeIconConverter}}" />
                        <Run Text="{Binding Quantity, StringFormat='Qty: {0:N0}'}" />
                        <Run Text="{Binding Operation, StringFormat='Op: {0}'}" />
                    </TextBlock>
                </StackPanel>
            </Border>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

### Dynamic Resource Binding
```xml
<!-- Dynamic theme-aware styling -->
<DataGrid ItemsSource="{Binding InventoryItems}">
    <DataGrid.Resources>
        <!-- Operation-specific row styles -->
        <Style Selector="DataGridRow.operation-90">
            <Setter Property="Background" Value="{DynamicResource Operation90Brush}" />
        </Style>
        <Style Selector="DataGridRow.operation-100">
            <Setter Property="Background" Value="{DynamicResource Operation100Brush}" />
        </Style>
    </DataGrid.Resources>
</DataGrid>
```

---

## 📚 Related Documentation

- **Theme Management**: [Avalonia UI Guidelines](./avalonia-ui-guidelines.instructions.md)
- **Application Configuration**: [Configuration Management](./application-configuration.instructions.md)
- **Custom Controls**: [Custom Control Implementation](./custom-controls.instructions.md)
- **Value Converters**: [Converter Patterns](./value-converters.instructions.md)

---

**Document Status**: ✅ Complete Resource Management Reference  
**Framework Version**: Avalonia UI 11.3.4  
**Last Updated**: 2025-09-14  
**Resource Management Owner**: MTM Development Team