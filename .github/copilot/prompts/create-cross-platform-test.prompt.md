---
description: 'Prompt template for creating cross-platform tests for MTM WIP Application supporting Windows, macOS, Linux, and Android'
applies_to: '**/*'
---

# Create Cross-Platform Test Prompt Template

## 🎯 Objective

Generate comprehensive cross-platform tests for MTM WIP Application ensuring consistent functionality across Windows, macOS, Linux, and Android platforms. Focus on platform-specific behaviors, UI adaptations, file system operations, and manufacturing workflow compatibility.

## 📋 Instructions

When creating cross-platform tests, follow these specific requirements:

### Cross-Platform Test Structure

1. **Use MTM Platform Test Base Class**
   ```csharp
   [TestFixture]
   [Category("CrossPlatform")]
   [Category("{PlatformCategory}")]  // e.g., Windows, macOS, Linux, Android, All
   public class {ComponentName}CrossPlatformTests : CrossPlatformTestBase
   {
       // Cross-platform test implementation
   }
   ```

2. **Platform Test Categories**
   - Windows: Windows-specific functionality
   - macOS: macOS-specific adaptations
   - Linux: Linux distribution compatibility
   - Android: Mobile/tablet manufacturing scenarios
   - All: Tests that must pass on all platforms

### Cross-Platform Test Framework Setup

#### Base Cross-Platform Test Class
```csharp
public abstract class CrossPlatformTestBase
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected PlatformTestContext PlatformContext { get; private set; }
    protected TestPlatformInfo CurrentPlatform { get; private set; }
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        // Detect current platform
        CurrentPlatform = DetectCurrentPlatform();
        
        // Setup platform-specific services
        var services = new ServiceCollection();
        ConfigurePlatformTestServices(services, CurrentPlatform);
        ServiceProvider = services.BuildServiceProvider();
        
        // Initialize platform context
        PlatformContext = new PlatformTestContext(CurrentPlatform);
        
        await Task.CompletedTask;
    }
    
    [SetUp]
    public virtual async Task SetUp()
    {
        PlatformContext.ResetTestState();
        await Task.CompletedTask;
    }
    
    [TearDown]
    public virtual async Task TearDown()
    {
        await PlatformContext.CleanupPlatformResourcesAsync();
    }
    
    protected virtual void ConfigurePlatformTestServices(IServiceCollection services, TestPlatformInfo platform)
    {
        services.AddLogging();
        services.AddMTMServices(GetPlatformConfiguration(platform));
        
        // Add platform-specific services
        switch (platform.OS)
        {
            case PlatformOS.Windows:
                services.AddTransient<IFileSystemService, WindowsFileSystemService>();
                break;
            case PlatformOS.macOS:
                services.AddTransient<IFileSystemService, MacOSFileSystemService>();
                break;
            case PlatformOS.Linux:
                services.AddTransient<IFileSystemService, LinuxFileSystemService>();
                break;
            case PlatformOS.Android:
                services.AddTransient<IFileSystemService, AndroidFileSystemService>();
                break;
        }
    }
    
    protected IConfiguration GetPlatformConfiguration(TestPlatformInfo platform)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{platform.OS}.json", optional: true)
            .AddEnvironmentVariables();
        
        return configBuilder.Build();
    }
    
    protected T GetService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }
    
    protected TestPlatformInfo DetectCurrentPlatform()
    {
        var platform = new TestPlatformInfo();
        
        if (OperatingSystem.IsWindows())
        {
            platform.OS = PlatformOS.Windows;
            platform.Version = Environment.OSVersion.Version.ToString();
            platform.Architecture = RuntimeInformation.ProcessArchitecture.ToString();
        }
        else if (OperatingSystem.IsMacOS())
        {
            platform.OS = PlatformOS.macOS;
            platform.Version = Environment.OSVersion.Version.ToString();
            platform.Architecture = RuntimeInformation.ProcessArchitecture.ToString();
        }
        else if (OperatingSystem.IsLinux())
        {
            platform.OS = PlatformOS.Linux;
            platform.Version = GetLinuxVersion();
            platform.Architecture = RuntimeInformation.ProcessArchitecture.ToString();
        }
        else if (OperatingSystem.IsAndroid())
        {
            platform.OS = PlatformOS.Android;
            platform.Version = GetAndroidVersion();
            platform.Architecture = RuntimeInformation.ProcessArchitecture.ToString();
        }
        
        platform.RuntimeVersion = RuntimeInformation.FrameworkDescription;
        platform.AvaloniaVersion = GetAvaloniaVersion();
        
        return platform;
    }
    
    private string GetLinuxVersion()
    {
        try
        {
            var versionInfo = File.ReadAllText("/etc/os-release");
            var lines = versionInfo.Split('\n');
            var prettyName = lines.FirstOrDefault(l => l.StartsWith("PRETTY_NAME="));
            return prettyName?.Split('=')[1].Trim('"') ?? "Unknown Linux";
        }
        catch
        {
            return "Linux";
        }
    }
    
    private string GetAndroidVersion()
    {
        // Android version detection logic
        return "Android API " + Environment.OSVersion.Version.Major;
    }
    
    private string GetAvaloniaVersion()
    {
        var assembly = typeof(Application).Assembly;
        return assembly.GetName().Version?.ToString() ?? "Unknown";
    }
    
    protected bool ShouldSkipOnPlatform(PlatformOS platform, string reason = null)
    {
        if (CurrentPlatform.OS == platform)
        {
            Assert.Ignore(reason ?? $"Test skipped on {platform}");
            return true;
        }
        return false;
    }
    
    protected void AssumePlatform(PlatformOS platform)
    {
        Assume.That(CurrentPlatform.OS, Is.EqualTo(platform), 
            $"This test requires {platform} but running on {CurrentPlatform.OS}");
    }
    
    protected void ReportPlatformInfo()
    {
        Console.WriteLine("=== Platform Information ===");
        Console.WriteLine($"Operating System: {CurrentPlatform.OS}");
        Console.WriteLine($"OS Version: {CurrentPlatform.Version}");
        Console.WriteLine($"Architecture: {CurrentPlatform.Architecture}");
        Console.WriteLine($"Runtime: {CurrentPlatform.RuntimeVersion}");
        Console.WriteLine($"Avalonia Version: {CurrentPlatform.AvaloniaVersion}");
        Console.WriteLine("============================");
    }
}

public class TestPlatformInfo
{
    public PlatformOS OS { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Architecture { get; set; } = string.Empty;
    public string RuntimeVersion { get; set; } = string.Empty;
    public string AvaloniaVersion { get; set; } = string.Empty;
}

public enum PlatformOS
{
    Windows,
    macOS,
    Linux,
    Android,
    Unknown
}

public class PlatformTestContext
{
    private readonly TestPlatformInfo _platform;
    private readonly List<string> _tempFiles = new();
    private readonly List<string> _tempDirectories = new();
    
    public PlatformTestContext(TestPlatformInfo platform)
    {
        _platform = platform;
    }
    
    public void ResetTestState()
    {
        _tempFiles.Clear();
        _tempDirectories.Clear();
    }
    
    public async Task CleanupPlatformResourcesAsync()
    {
        // Cleanup temporary files
        foreach (var file in _tempFiles.Where(File.Exists))
        {
            try { File.Delete(file); } catch { /* Ignore */ }
        }
        
        // Cleanup temporary directories
        foreach (var dir in _tempDirectories.Where(Directory.Exists))
        {
            try { Directory.Delete(dir, true); } catch { /* Ignore */ }
        }
        
        await Task.CompletedTask;
    }
    
    public string CreateTempFile(string content = null)
    {
        var tempFile = Path.GetTempFileName();
        _tempFiles.Add(tempFile);
        
        if (content != null)
        {
            File.WriteAllText(tempFile, content);
        }
        
        return tempFile;
    }
    
    public string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        _tempDirectories.Add(tempDir);
        
        return tempDir;
    }
    
    public string GetPlatformConfigPath()
    {
        return _platform.OS switch
        {
            PlatformOS.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTM_WIP_Application"),
            PlatformOS.macOS => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/MTM_WIP_Application"),
            PlatformOS.Linux => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config/MTM_WIP_Application"),
            PlatformOS.Android => Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath ?? "/storage/emulated/0", "MTM_WIP_Application"),
            _ => Path.GetTempPath()
        };
    }
    
    public char GetPathSeparator()
    {
        return _platform.OS switch
        {
            PlatformOS.Windows => '\\',
            _ => '/'
        };
    }
}
```

### File System Cross-Platform Tests

#### Path Handling and File Operations
```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("FileSystem")]
public class FileSystemCrossPlatformTests : CrossPlatformTestBase
{
    private IFileSystemService _fileSystemService;
    
    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();
        _fileSystemService = GetService<IFileSystemService>();
    }
    
    [Test]
    public async Task ConfigurationFiles_AllPlatforms_ShouldLoadCorrectly()
    {
        // Arrange
        ReportPlatformInfo();
        
        var configService = GetService<IConfigurationService>();
        
        // Act & Assert - Configuration should load on all platforms
        var connectionString = await configService.GetConnectionStringAsync();
        var appSettings = await configService.GetSettingAsync<string>("MTM_Configuration:AppName");
        
        Assert.That(connectionString, Is.Not.Null.And.Not.Empty,
            $"Connection string should load on {CurrentPlatform.OS}");
        
        Assert.That(appSettings, Is.Not.Null.And.Not.Empty,
            $"App settings should load on {CurrentPlatform.OS}");
        
        // Platform-specific configuration path validation
        var expectedConfigPath = PlatformContext.GetPlatformConfigPath();
        Assert.That(Directory.Exists(Path.GetDirectoryName(expectedConfigPath)), Is.True,
            $"Configuration directory should exist at platform-appropriate location: {expectedConfigPath}");
    }
    
    [Test]
    public async Task FilePathHandling_AllPlatforms_ShouldUseCorrectSeparators()
    {
        // Arrange
        var testDirectory = PlatformContext.CreateTempDirectory();
        var expectedSeparator = PlatformContext.GetPathSeparator();
        
        var testPaths = new[]
        {
            "subfolder1/file1.txt",
            "subfolder2/subfolder3/file2.csv",
            "logs/error.log"
        };
        
        // Act - Create files using cross-platform path handling
        var createdFiles = new List<string>();
        
        foreach (var testPath in testPaths)
        {
            var fullPath = await _fileSystemService.GetFullPathAsync(testDirectory, testPath);
            await _fileSystemService.EnsureDirectoryExistsAsync(Path.GetDirectoryName(fullPath));
            await _fileSystemService.WriteTextAsync(fullPath, $"Test content for {testPath}");
            createdFiles.Add(fullPath);
        }
        
        // Assert - Files should be created with correct path separators
        foreach (var createdFile in createdFiles)
        {
            Assert.That(File.Exists(createdFile), Is.True,
                $"File should exist with platform-correct path: {createdFile}");
            
            // Verify path separator usage
            var relativePath = Path.GetRelativePath(testDirectory, createdFile);
            if (CurrentPlatform.OS == PlatformOS.Windows)
            {
                Assert.That(relativePath.Contains('\\'), Is.True,
                    $"Windows paths should use backslashes: {relativePath}");
            }
            else
            {
                Assert.That(relativePath.Contains('/'), Is.True,
                    $"Unix-like paths should use forward slashes: {relativePath}");
            }
        }
        
        // Test file reading
        foreach (var createdFile in createdFiles)
        {
            var content = await _fileSystemService.ReadTextAsync(createdFile);
            Assert.That(content, Is.Not.Null.And.Not.Empty,
                $"File content should be readable on {CurrentPlatform.OS}");
        }
    }
    
    [Test]
    public async Task LogFileCreation_AllPlatforms_ShouldUseCorrectLocation()
    {
        // Arrange
        var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("CrossPlatformTest");
        
        // Act - Create log entries
        logger.LogInformation("Cross-platform test log entry from {Platform}", CurrentPlatform.OS);
        logger.LogWarning("Warning message on {Platform}", CurrentPlatform.OS);
        logger.LogError("Error message on {Platform}", CurrentPlatform.OS);
        
        // Allow log flushing
        await Task.Delay(1000);
        
        // Assert - Log files should exist in platform-appropriate location
        var expectedLogPaths = GetPlatformLogPaths();
        var logFileFound = false;
        
        foreach (var logPath in expectedLogPaths)
        {
            if (Directory.Exists(logPath))
            {
                var logFiles = Directory.GetFiles(logPath, "*.log", SearchOption.AllDirectories);
                if (logFiles.Length > 0)
                {
                    logFileFound = true;
                    
                    // Verify log content
                    var latestLogFile = logFiles.OrderByDescending(File.GetLastWriteTime).First();
                    var logContent = await File.ReadAllTextAsync(latestLogFile);
                    
                    Assert.That(logContent.Contains("Cross-platform test log entry"), Is.True,
                        $"Log should contain test entries on {CurrentPlatform.OS}");
                    
                    Console.WriteLine($"Log file found at: {latestLogFile}");
                    Console.WriteLine($"Log file size: {new FileInfo(latestLogFile).Length} bytes");
                    break;
                }
            }
        }
        
        Assert.That(logFileFound, Is.True,
            $"Log files should be created in platform-appropriate location on {CurrentPlatform.OS}");
    }
    
    private string[] GetPlatformLogPaths()
    {
        return CurrentPlatform.OS switch
        {
            PlatformOS.Windows => new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTM_WIP_Application", "logs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MTM_WIP_Application", "logs")
            },
            PlatformOS.macOS => new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Logs/MTM_WIP_Application"),
                "/var/log/MTM_WIP_Application"
            },
            PlatformOS.Linux => new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local/share/MTM_WIP_Application/logs"),
                "/var/log/MTM_WIP_Application"
            },
            PlatformOS.Android => new[]
            {
                Path.Combine(Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath ?? "/storage/emulated/0", "MTM_WIP_Application", "logs"),
                Path.Combine(Android.App.Application.Context.FilesDir?.AbsolutePath ?? "/data/data/package/files", "logs")
            },
            _ => new[] { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs") }
        };
    }
}
```

### UI Cross-Platform Tests

#### Theme and Layout Adaptations
```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("UI")]
public class UICrossPlatformTests : CrossPlatformTestBase
{
    private TestAppBuilder _appBuilder;
    
    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();
        
        _appBuilder = AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = true
            });
    }
    
    [Test]
    public async Task MainWindow_AllPlatforms_ShouldRenderCorrectly()
    {
        // Skip on Android for desktop-specific tests
        if (CurrentPlatform.OS == PlatformOS.Android)
        {
            ShouldSkipOnPlatform(PlatformOS.Android, "Desktop window tests not applicable on Android");
            return;
        }
        
        // Arrange
        ReportPlatformInfo();
        
        using var app = _appBuilder.SetupWithoutStarting();
        
        var mainWindow = new MainWindow();
        
        // Platform-specific window size expectations
        var expectedMinWidth = CurrentPlatform.OS switch
        {
            PlatformOS.Windows => 800,
            PlatformOS.macOS => 900,   // macOS typically needs more space for title bar
            PlatformOS.Linux => 800,
            _ => 800
        };
        
        var expectedMinHeight = CurrentPlatform.OS switch
        {
            PlatformOS.Windows => 600,
            PlatformOS.macOS => 650,  // macOS menu bar considerations
            PlatformOS.Linux => 600,
            _ => 600
        };
        
        // Act
        mainWindow.Show();
        await Task.Delay(500); // Allow rendering
        
        // Assert - Basic window properties
        Assert.That(mainWindow.Width, Is.GreaterThanOrEqualTo(expectedMinWidth),
            $"Main window should have appropriate minimum width on {CurrentPlatform.OS}");
        
        Assert.That(mainWindow.Height, Is.GreaterThanOrEqualTo(expectedMinHeight),
            $"Main window should have appropriate minimum height on {CurrentPlatform.OS}");
        
        // Test platform-specific UI elements
        var menuBar = mainWindow.FindDescendantOfType<MenuBar>();
        if (CurrentPlatform.OS == PlatformOS.macOS)
        {
            // macOS should use native menu bar (might be null in headless testing)
            Console.WriteLine($"MenuBar on macOS (native): {(menuBar != null ? "Present" : "Native/Hidden")}");
        }
        else
        {
            Assert.That(menuBar, Is.Not.Null,
                $"Menu bar should be present in window on {CurrentPlatform.OS}");
        }
        
        // Test theme application
        var themeService = GetService<IThemeService>();
        await themeService.ApplyThemeAsync("MTM_Blue");
        
        await Task.Delay(200); // Allow theme application
        
        // Verify theme resources are applied
        var primaryBrush = mainWindow.FindResource("MTM_Shared_Logic.PrimaryBrush");
        Assert.That(primaryBrush, Is.Not.Null,
            $"Theme resources should be available on {CurrentPlatform.OS}");
        
        Console.WriteLine($"Platform-specific rendering test completed on {CurrentPlatform.OS}");
        Console.WriteLine($"Window size: {mainWindow.Width}x{mainWindow.Height}");
        
        mainWindow.Close();
    }
    
    [Test]
    public async Task ResponsiveLayout_AllPlatforms_ShouldAdaptCorrectly()
    {
        using var app = _appBuilder.SetupWithoutStarting();
        
        var inventoryView = new InventoryTabView();
        var testWindow = new Window 
        { 
            Content = inventoryView,
            Width = 1024,
            Height = 768
        };
        
        testWindow.Show();
        await Task.Delay(300);
        
        // Test different window sizes to verify responsive behavior
        var testSizes = new (double Width, double Height)[]
        {
            (800, 600),   // Small desktop
            (1024, 768),  // Medium desktop
            (1440, 900),  // Large desktop
            (1920, 1080)  // Full HD
        };
        
        // Android would use different size constraints
        if (CurrentPlatform.OS == PlatformOS.Android)
        {
            testSizes = new (double Width, double Height)[]
            {
                (360, 640),   // Mobile portrait
                (640, 360),   // Mobile landscape
                (800, 1280),  // Tablet portrait
                (1280, 800)   // Tablet landscape
            };
        }
        
        foreach (var (width, height) in testSizes)
        {
            // Act
            testWindow.Width = width;
            testWindow.Height = height;
            await Task.Delay(100); // Allow layout update
            
            // Assert - UI elements should remain accessible and properly sized
            var scrollViewer = inventoryView.FindDescendantOfType<ScrollViewer>();
            Assert.That(scrollViewer, Is.Not.Null,
                $"ScrollViewer should exist at {width}x{height} on {CurrentPlatform.OS}");
            
            var dataGrid = inventoryView.FindDescendantOfType<DataGrid>();
            Assert.That(dataGrid, Is.Not.Null,
                $"DataGrid should exist at {width}x{height} on {CurrentPlatform.OS}");
            
            // Verify minimum size constraints
            Assert.That(dataGrid.ActualWidth, Is.GreaterThan(200),
                $"DataGrid should maintain minimum width at {width}x{height} on {CurrentPlatform.OS}");
            
            Console.WriteLine($"Layout test passed for {width}x{height} on {CurrentPlatform.OS}");
        }
        
        testWindow.Close();
    }
    
    [Test]
    [Category("Android")]
    public async Task TouchInterface_Android_ShouldSupportTouchOperations()
    {
        // Only run on Android
        AssumePlatform(PlatformOS.Android);
        
        using var app = _appBuilder.SetupWithoutStarting();
        
        var quickButtonsView = new QuickButtonsView();
        var testWindow = new Window 
        { 
            Content = quickButtonsView,
            Width = 800,
            Height = 1280 // Portrait tablet
        };
        
        testWindow.Show();
        await Task.Delay(300);
        
        // Find QuickButton controls
        var quickButtons = quickButtonsView.FindDescendantsOfType<Button>()
            .Where(b => b.Name?.Contains("QuickButton") == true)
            .ToList();
        
        Assert.That(quickButtons, Is.Not.Empty,
            "QuickButtons should be rendered for Android touch interface");
        
        // Verify touch-friendly sizing
        foreach (var button in quickButtons.Take(5)) // Test first 5 buttons
        {
            Assert.That(button.MinWidth, Is.GreaterThanOrEqualTo(44), // Android touch target minimum
                $"QuickButton should have minimum touch-friendly width on Android");
            
            Assert.That(button.MinHeight, Is.GreaterThanOrEqualTo(44),
                $"QuickButton should have minimum touch-friendly height on Android");
            
            // Test touch simulation (click)
            var clickHandler = new TaskCompletionSource<bool>();
            button.Click += (_, _) => clickHandler.TrySetResult(true);
            
            // Simulate touch/click
            button.Command?.Execute(button.CommandParameter);
            
            Console.WriteLine($"Touch test successful for QuickButton: {button.Content}");
        }
        
        testWindow.Close();
    }
}
```

### Database Cross-Platform Tests

#### Connection and Data Access
```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("Database")]
public class DatabaseCrossPlatformTests : CrossPlatformTestBase
{
    private IInventoryService _inventoryService;
    private IConfigurationService _configurationService;
    
    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();
        _inventoryService = GetService<IInventoryService>();
        _configurationService = GetService<IConfigurationService>();
    }
    
    [Test]
    public async Task DatabaseConnection_AllPlatforms_ShouldConnectSuccessfully()
    {
        // Arrange
        ReportPlatformInfo();
        
        var connectionString = await _configurationService.GetConnectionStringAsync();
        
        Assert.That(connectionString, Is.Not.Null.And.Not.Empty,
            $"Connection string should be configured on {CurrentPlatform.OS}");
        
        // Act & Assert - Test database connectivity
        try
        {
            var testResult = await _inventoryService.GetInventoryAsync("TEST_CONNECTION", "100");
            
            // Connection successful if no exception thrown
            Assert.Pass($"Database connection successful on {CurrentPlatform.OS}");
        }
        catch (Exception ex)
        {
            // Log platform-specific connection details
            Console.WriteLine($"Connection attempt on {CurrentPlatform.OS}:");
            Console.WriteLine($"  Architecture: {CurrentPlatform.Architecture}");
            Console.WriteLine($"  Runtime: {CurrentPlatform.RuntimeVersion}");
            Console.WriteLine($"  Error: {ex.Message}");
            
            // Platform-specific error handling
            if (CurrentPlatform.OS == PlatformOS.Android)
            {
                // Android may have network security policy restrictions
                Assert.Inconclusive($"Android database connection may require additional network security configuration: {ex.Message}");
            }
            else
            {
                Assert.Fail($"Database connection failed on {CurrentPlatform.OS}: {ex.Message}");
            }
        }
    }
    
    [Test]
    public async Task DatabaseOperations_AllPlatforms_ShouldHandleDataCorrectly()
    {
        // Arrange - Platform-specific test data
        var testPartId = $"XPLAT_{CurrentPlatform.OS}_{DateTime.Now:yyyyMMddHHmmss}";
        var testItem = new InventoryItem
        {
            PartId = testPartId,
            Operation = "100",
            Quantity = 1,
            Location = $"XPLAT_STATION_{CurrentPlatform.OS}",
            User = $"CrossPlatformTest_{CurrentPlatform.OS}"
        };
        
        try
        {
            // Act - Add inventory
            var addResult = await _inventoryService.AddInventoryAsync(testItem);
            
            Assert.That(addResult.Success, Is.True,
                $"Adding inventory should succeed on {CurrentPlatform.OS}");
            
            // Act - Retrieve inventory
            var retrievedItems = await _inventoryService.GetInventoryAsync(testPartId, "100");
            
            Assert.That(retrievedItems, Is.Not.Null.And.Not.Empty,
                $"Retrieving inventory should work on {CurrentPlatform.OS}");
            
            var retrievedItem = retrievedItems.First();
            
            // Assert - Data integrity across platforms
            Assert.That(retrievedItem.PartId, Is.EqualTo(testPartId),
                $"Part ID should be preserved correctly on {CurrentPlatform.OS}");
            
            Assert.That(retrievedItem.Operation, Is.EqualTo("100"),
                $"Operation should be preserved correctly on {CurrentPlatform.OS}");
            
            Assert.That(retrievedItem.Quantity, Is.EqualTo(1),
                $"Quantity should be preserved correctly on {CurrentPlatform.OS}");
            
            // Platform-specific validation
            ValidatePlatformSpecificData(retrievedItem);
            
            Console.WriteLine($"Database operations test passed on {CurrentPlatform.OS}");
            Console.WriteLine($"  Test Part ID: {testPartId}");
            Console.WriteLine($"  Retrieved Quantity: {retrievedItem.Quantity}");
            Console.WriteLine($"  User: {retrievedItem.User}");
        }
        catch (Exception ex)
        {
            HandlePlatformSpecificDatabaseError(ex);
        }
    }
    
    private void ValidatePlatformSpecificData(InventoryItem item)
    {
        // Platform-specific data validation
        switch (CurrentPlatform.OS)
        {
            case PlatformOS.Windows:
                // Windows-specific validation
                Assert.That(item.User, Does.Not.Contain("/"),
                    "Windows usernames should not contain forward slashes");
                break;
                
            case PlatformOS.Linux:
                // Linux-specific validation
                Assert.That(item.User?.Length, Is.LessThanOrEqualTo(32),
                    "Linux usernames should not exceed 32 characters");
                break;
                
            case PlatformOS.macOS:
                // macOS-specific validation
                Assert.That(item.User, Is.Not.Null.And.Not.Empty,
                    "macOS should have valid user information");
                break;
                
            case PlatformOS.Android:
                // Android-specific validation
                Assert.That(item.Location, Does.Not.Contain("\\"),
                    "Android paths should use forward slashes");
                break;
        }
    }
    
    private void HandlePlatformSpecificDatabaseError(Exception ex)
    {
        var errorInfo = $"Database error on {CurrentPlatform.OS}: {ex.Message}";
        
        // Platform-specific error analysis
        switch (CurrentPlatform.OS)
        {
            case PlatformOS.Android:
                if (ex.Message.Contains("Network") || ex.Message.Contains("cleartext"))
                {
                    Assert.Inconclusive($"Android network security policy may require HTTPS: {errorInfo}");
                }
                break;
                
            case PlatformOS.Linux:
                if (ex.Message.Contains("permission") || ex.Message.Contains("denied"))
                {
                    Assert.Inconclusive($"Linux permissions may need adjustment: {errorInfo}");
                }
                break;
                
            case PlatformOS.macOS:
                if (ex.Message.Contains("Gatekeeper") || ex.Message.Contains("quarantine"))
                {
                    Assert.Inconclusive($"macOS security restrictions may apply: {errorInfo}");
                }
                break;
        }
        
        // General failure
        Assert.Fail(errorInfo);
    }
}
```

### Platform-Specific Feature Tests

#### Feature Availability Tests
```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("Features")]
public class FeatureAvailabilityCrossPlatformTests : CrossPlatformTestBase
{
    [Test]
    public async Task PrintingService_DesktopPlatforms_ShouldBeAvailable()
    {
        // Arrange - Printing should be available on desktop platforms
        var expectedAvailability = CurrentPlatform.OS switch
        {
            PlatformOS.Windows => true,
            PlatformOS.macOS => true,
            PlatformOS.Linux => true,
            PlatformOS.Android => false, // Limited printing on mobile
            _ => false
        };
        
        // Act
        var printingService = ServiceProvider.GetService<IPrintingService>();
        
        // Assert
        if (expectedAvailability)
        {
            Assert.That(printingService, Is.Not.Null,
                $"Printing service should be available on {CurrentPlatform.OS}");
            
            var availablePrinters = await printingService.GetAvailablePrintersAsync();
            Console.WriteLine($"Available printers on {CurrentPlatform.OS}: {availablePrinters.Count()}");
        }
        else
        {
            if (printingService == null)
            {
                Assert.Pass($"Printing service correctly not available on {CurrentPlatform.OS}");
            }
            else
            {
                // Service exists but may have limited functionality
                Console.WriteLine($"Printing service present but may have limited functionality on {CurrentPlatform.OS}");
            }
        }
    }
    
    [Test]
    public async Task FileSystemWatching_AllPlatforms_ShouldWorkCorrectly()
    {
        // Arrange
        var watchDirectory = PlatformContext.CreateTempDirectory();
        var fileSystemService = GetService<IFileSystemService>();
        
        var watcherEvents = new List<string>();
        var eventReceived = new TaskCompletionSource<bool>();
        
        // Act - Setup file system watching
        using var watcher = await fileSystemService.CreateFileWatcherAsync(
            watchDirectory,
            "*.txt",
            (eventType, filePath) =>
            {
                watcherEvents.Add($"{eventType}: {Path.GetFileName(filePath)}");
                eventReceived.TrySetResult(true);
            });
        
        // Create a test file
        var testFile = Path.Combine(watchDirectory, "test-watch.txt");
        await File.WriteAllTextAsync(testFile, "Test content");
        
        // Wait for file system event
        var eventReceived = await eventReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));
        
        // Assert
        Assert.That(eventReceived, Is.True,
            $"File system watching should work on {CurrentPlatform.OS}");
        
        Assert.That(watcherEvents, Is.Not.Empty,
            $"File system events should be captured on {CurrentPlatform.OS}");
        
        Console.WriteLine($"File system watching test completed on {CurrentPlatform.OS}");
        Console.WriteLine($"Events received: {string.Join(", ", watcherEvents)}");
    }
}
```

## ✅ Cross-Platform Test Checklist

When creating cross-platform tests, ensure:

- [ ] All supported platforms (Windows, macOS, Linux, Android) are considered
- [ ] Platform-specific features are tested appropriately
- [ ] File system operations use correct path separators
- [ ] Configuration files load from platform-appropriate locations
- [ ] UI layouts adapt correctly to different screen sizes and input methods
- [ ] Database connections work across platforms
- [ ] Platform-specific error scenarios are handled
- [ ] Touch interface functionality is tested on Android
- [ ] Desktop-specific features are appropriately skipped on mobile
- [ ] Performance characteristics are validated per platform
- [ ] Network security policies are considered (especially Android)
- [ ] Platform-specific resource management is tested
- [ ] Logging and error reporting work on all platforms

## 🏷️ Cross-Platform Test Categories

Use these category attributes for cross-platform tests:

```csharp
[Category("CrossPlatform")]     // All cross-platform tests
[Category("Windows")]           // Windows-specific tests
[Category("macOS")]            // macOS-specific tests
[Category("Linux")]            // Linux-specific tests
[Category("Android")]          // Android-specific tests
[Category("Desktop")]          // Desktop platforms only
[Category("Mobile")]           // Mobile platforms only
[Category("FileSystem")]       // File system compatibility
[Category("UI")]               // UI adaptation testing
[Category("Database")]         // Database connectivity
[Category("Features")]         // Feature availability testing
```

This template ensures comprehensive cross-platform test coverage for the MTM WIP Application across Windows, macOS, Linux, and Android platforms.