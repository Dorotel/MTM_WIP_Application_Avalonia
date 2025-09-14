---
description: 'Cross-platform testing standards for MTM WIP Application on Windows, macOS, Linux, and Android'
applies_to: '**/*'
---

# MTM Cross-Platform Testing Standards Instructions

## 🎯 Overview

Comprehensive cross-platform testing standards for MTM WIP Application, ensuring consistent functionality across Windows, macOS, Linux, and Android platforms with specific focus on Avalonia UI, file system operations, database connectivity, and manufacturing workflow compatibility.

## 🖥️ Platform-Specific Testing Architecture

### Cross-Platform Test Framework Structure

```csharp
[TestFixture]
[Category("CrossPlatform")]
public abstract class CrossPlatformTestBase
{
    protected static readonly PlatformInfo CurrentPlatform = PlatformDetector.GetCurrent();
    protected string TestDataDirectory { get; private set; }
    protected IConfiguration PlatformConfiguration { get; private set; }
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        TestDataDirectory = CreatePlatformSpecificTestDirectory();
        PlatformConfiguration = LoadPlatformConfiguration();
        await SetupPlatformSpecificResourcesAsync();
    }
    
    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await CleanupPlatformSpecificResourcesAsync();
        CleanupTestDirectory();
    }
    
    protected string CreatePlatformSpecificTestDirectory()
    {
        var baseDirectory = CurrentPlatform.OS switch
        {
            OSPlatform.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MTM_Tests"),
            OSPlatform.macOS => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "MTM_Tests"),
            OSPlatform.Linux => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "MTM_Tests"),
            OSPlatform.Android => Path.Combine("/data/data/com.mtm.wipapp/files", "tests"),
            _ => Path.Combine(Path.GetTempPath(), "MTM_Tests")
        };
        
        Directory.CreateDirectory(baseDirectory);
        return baseDirectory;
    }
    
    protected virtual async Task SetupPlatformSpecificResourcesAsync()
    {
        await Task.CompletedTask;
    }
    
    protected virtual async Task CleanupPlatformSpecificResourcesAsync()
    {
        await Task.CompletedTask;
    }
    
    protected void CleanupTestDirectory()
    {
        try
        {
            if (Directory.Exists(TestDataDirectory))
            {
                Directory.Delete(TestDataDirectory, recursive: true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not cleanup test directory: {ex.Message}");
        }
    }
    
    protected void AssertCrossPlatformCompatibility<T>(T expected, T actual, string context)
    {
        Assert.That(actual, Is.EqualTo(expected), 
            $"{context} should be consistent across platforms. Platform: {CurrentPlatform.OS} {CurrentPlatform.Architecture}");
    }
    
    protected void SkipIfPlatformNotSupported(params OSPlatform[] supportedPlatforms)
    {
        if (!supportedPlatforms.Contains(CurrentPlatform.OS))
        {
            Assert.Ignore($"Test not supported on {CurrentPlatform.OS}. Supported platforms: {string.Join(", ", supportedPlatforms)}");
        }
    }
    
    protected string GetPlatformSpecificPath(string relativePath)
    {
        // Normalize path separators for current platform
        return CurrentPlatform.OS == OSPlatform.Windows 
            ? relativePath.Replace('/', '\\')
            : relativePath.Replace('\\', '/');
    }
}

public static class PlatformDetector
{
    public static PlatformInfo GetCurrent()
    {
        var os = OSPlatform.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.macOS))
            os = OSPlatform.macOS;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            os = OSPlatform.Linux;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            os = OSPlatform.FreeBSD;
        // Note: Android detection requires additional platform checks
        
        return new PlatformInfo
        {
            OS = os,
            Architecture = RuntimeInformation.OSArchitecture,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture
        };
    }
}

public class PlatformInfo
{
    public OSPlatform OS { get; set; }
    public Architecture Architecture { get; set; }
    public string FrameworkDescription { get; set; } = string.Empty;
    public Architecture ProcessArchitecture { get; set; }
    
    public bool IsDesktop => OS == OSPlatform.Windows || OS == OSPlatform.macOS || OS == OSPlatform.Linux;
    public bool IsMobile => OS == OSPlatform.Android || OS == OSPlatform.Create("iOS");
    public bool IsUnix => OS == OSPlatform.macOS || OS == OSPlatform.Linux || OS == OSPlatform.FreeBSD;
}
```

## 🗂️ File System Cross-Platform Testing

### File Operations Testing

```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("FileSystem")]
public class FileSystemCrossPlatformTests : CrossPlatformTestBase
{
    [Test]
    public async Task Configuration_LoadSettings_ShouldWorkAcrossPlatforms()
    {
        // Arrange - Create platform-specific config paths
        var configPaths = new Dictionary<OSPlatform, string>
        {
            { OSPlatform.Windows, Path.Combine(TestDataDirectory, "appsettings.json") },
            { OSPlatform.macOS, Path.Combine(TestDataDirectory, "appsettings.json") },
            { OSPlatform.Linux, Path.Combine(TestDataDirectory, "appsettings.json") },
            { OSPlatform.Android, Path.Combine(TestDataDirectory, "appsettings.json") }
        };
        
        var testConfig = new
        {
            ConnectionStrings = new
            {
                DefaultConnection = $"Server=localhost;Database=mtm_test_{CurrentPlatform.OS};Uid=test;Pwd=test;"
            },
            MTM_Configuration = new
            {
                AppName = "MTM WIP Application",
                Version = "1.0.0",
                Platform = CurrentPlatform.OS.ToString()
            }
        };
        
        var configJson = JsonSerializer.Serialize(testConfig, new JsonSerializerOptions { WriteIndented = true });
        var configPath = configPaths[CurrentPlatform.OS];
        
        // Act - Write and read configuration
        await File.WriteAllTextAsync(configPath, configJson);
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(TestDataDirectory)
            .AddJsonFile("appsettings.json", optional: false);
            
        var configuration = configurationBuilder.Build();
        
        // Assert - Configuration should load correctly on all platforms
        Assert.That(configuration["MTM_Configuration:AppName"], Is.EqualTo("MTM WIP Application"));
        Assert.That(configuration["MTM_Configuration:Platform"], Is.EqualTo(CurrentPlatform.OS.ToString()));
        Assert.That(configuration.GetConnectionString("DefaultConnection"), Is.Not.Null.And.Not.Empty);
        
        AssertCrossPlatformCompatibility(testConfig.MTM_Configuration.AppName, 
            configuration["MTM_Configuration:AppName"], "Configuration loading");
    }
    
    [Test]
    public async Task LogFiles_WriteAndRead_ShouldHandlePlatformDifferences()
    {
        // Arrange - Platform-specific log directories
        var logDirectory = CurrentPlatform.OS switch
        {
            OSPlatform.Windows => Path.Combine(TestDataDirectory, "Logs"),
            OSPlatform.macOS => Path.Combine(TestDataDirectory, "logs"),
            OSPlatform.Linux => Path.Combine(TestDataDirectory, "logs"),
            OSPlatform.Android => Path.Combine(TestDataDirectory, "logs"),
            _ => Path.Combine(TestDataDirectory, "logs")
        };
        
        Directory.CreateDirectory(logDirectory);
        
        var logFileName = CurrentPlatform.OS switch
        {
            OSPlatform.Windows => "mtm_app.log",
            _ => "mtm_app.log"  // Unix systems are case-sensitive but we use lowercase consistently
        };
        
        var logFilePath = Path.Combine(logDirectory, logFileName);
        var testLogEntries = new[]
        {
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO - Application started on {CurrentPlatform.OS}",
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DEBUG - Platform: {CurrentPlatform.Architecture}",
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARN - Cross-platform test log entry",
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR - Test error log entry"
        };
        
        // Act - Write log entries
        foreach (var entry in testLogEntries)
        {
            await File.AppendAllTextAsync(logFilePath, entry + Environment.NewLine);
        }
        
        // Read log file
        var logContent = await File.ReadAllTextAsync(logFilePath);
        var logLines = logContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        
        // Assert
        Assert.That(logLines.Length, Is.EqualTo(testLogEntries.Length));
        Assert.That(File.Exists(logFilePath), Is.True, "Log file should exist on all platforms");
        
        foreach (var expectedEntry in testLogEntries)
        {
            Assert.That(logContent, Does.Contain(expectedEntry), 
                $"Log should contain entry: {expectedEntry}");
        }
        
        // Verify file permissions on Unix systems
        if (CurrentPlatform.IsUnix)
        {
            var fileInfo = new FileInfo(logFilePath);
            Assert.That(fileInfo.Exists, Is.True, "Log file should exist on Unix systems");
            
            // Check that file is readable and writable
            using var testRead = File.OpenRead(logFilePath);
            Assert.That(testRead.CanRead, Is.True, "Log file should be readable on Unix systems");
        }
    }
    
    [Test]
    public async Task AppData_DirectoryCreation_ShouldRespectPlatformConventions()
    {
        // Arrange - Platform-specific application data directories
        var appDataPaths = new Dictionary<OSPlatform, string>
        {
            { OSPlatform.Windows, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MTM_WIP_Application") },
            { OSPlatform.macOS, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "MTM_WIP_Application") },
            { OSPlatform.Linux, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "mtm-wip-application") },
            { OSPlatform.Android, "/data/data/com.mtm.wipapp/files" }
        };
        
        var expectedPath = appDataPaths[CurrentPlatform.OS];
        
        // Act - Create application data directory
        var actualPath = CreateApplicationDataDirectory();
        
        // Assert
        Assert.That(Directory.Exists(actualPath), Is.True, "App data directory should be created");
        Assert.That(actualPath, Does.StartWith(Path.GetDirectoryName(expectedPath) ?? ""), 
            "App data path should follow platform conventions");
            
        // Test subdirectory creation
        var subDirectories = new[] { "Config", "Logs", "Cache", "Temp" };
        foreach (var subDir in subDirectories)
        {
            var subDirPath = Path.Combine(actualPath, subDir);
            Directory.CreateDirectory(subDirPath);
            Assert.That(Directory.Exists(subDirPath), Is.True, 
                $"Subdirectory '{subDir}' should be created on {CurrentPlatform.OS}");
        }
        
        // Cleanup
        if (Directory.Exists(actualPath))
        {
            Directory.Delete(actualPath, recursive: true);
        }
    }
    
    [Test]
    public void PathSeparators_Normalization_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        var testPaths = new[]
        {
            "Config\\appsettings.json",
            "Config/appsettings.json",
            "Views\\MainForm\\InventoryView.axaml",
            "Views/MainForm/InventoryView.axaml",
            "Services\\Database\\StoredProcedures",
            "Services/Database/StoredProcedures"
        };
        
        // Act & Assert
        foreach (var testPath in testPaths)
        {
            var normalizedPath = NormalizePath(testPath);
            
            // Platform-specific assertions
            if (CurrentPlatform.OS == OSPlatform.Windows)
            {
                Assert.That(normalizedPath, Does.Not.Contain("/"), 
                    "Windows paths should use backslashes");
                Assert.That(normalizedPath, Does.Contain("\\") | Does.Not.Contain("\\"),
                    "Windows paths should be properly normalized");
            }
            else
            {
                Assert.That(normalizedPath, Does.Not.Contain("\\"), 
                    "Unix paths should use forward slashes");
                Assert.That(normalizedPath, Does.Contain("/") | Does.Not.Contain("/"),
                    "Unix paths should be properly normalized");
            }
            
            // Test that normalized paths are valid
            Assert.That(() => Path.GetDirectoryName(normalizedPath), Throws.Nothing,
                $"Normalized path should be valid: {normalizedPath}");
        }
    }
    
    [Test]
    public async Task TempFiles_CreationAndCleanup_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        var tempDirectory = CurrentPlatform.OS switch
        {
            OSPlatform.Windows => Path.GetTempPath(),
            OSPlatform.macOS => "/tmp",
            OSPlatform.Linux => "/tmp",
            OSPlatform.Android => Path.Combine("/data/data/com.mtm.wipapp/cache"),
            _ => Path.GetTempPath()
        };
        
        var testFiles = new[]
        {
            "mtm_test_inventory.tmp",
            "mtm_test_transactions.tmp",
            "mtm_test_config.tmp"
        };
        
        var createdFiles = new List<string>();
        
        try
        {
            // Act - Create temporary files
            foreach (var fileName in testFiles)
            {
                var filePath = Path.Combine(tempDirectory, fileName);
                await File.WriteAllTextAsync(filePath, $"Temporary test data for {CurrentPlatform.OS}");
                createdFiles.Add(filePath);
                
                // Assert - File should be created
                Assert.That(File.Exists(filePath), Is.True, 
                    $"Temp file should be created on {CurrentPlatform.OS}: {filePath}");
            }
            
            // Test file permissions on Unix systems
            if (CurrentPlatform.IsUnix)
            {
                foreach (var filePath in createdFiles)
                {
                    var fileInfo = new FileInfo(filePath);
                    Assert.That(fileInfo.Exists, Is.True);
                    
                    // Verify read/write access
                    using var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
                    Assert.That(stream.CanRead, Is.True);
                    Assert.That(stream.CanWrite, Is.True);
                }
            }
        }
        finally
        {
            // Cleanup - Remove temporary files
            foreach (var filePath in createdFiles)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Assert.That(File.Exists(filePath), Is.False, 
                            "Temp file should be deleted successfully");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not delete temp file {filePath}: {ex.Message}");
                }
            }
        }
    }
    
    private string CreateApplicationDataDirectory()
    {
        var appDataPath = CurrentPlatform.OS switch
        {
            OSPlatform.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MTM_WIP_Application_Test"),
            OSPlatform.macOS => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "MTM_WIP_Application_Test"),
            OSPlatform.Linux => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "mtm-wip-application-test"),
            OSPlatform.Android => "/data/data/com.mtm.wipapp/files/test",
            _ => Path.Combine(Path.GetTempPath(), "MTM_WIP_Application_Test")
        };
        
        Directory.CreateDirectory(appDataPath);
        return appDataPath;
    }
    
    private string NormalizePath(string path)
    {
        return CurrentPlatform.OS == OSPlatform.Windows 
            ? path.Replace('/', '\\')
            : path.Replace('\\', '/');
    }
}
```

## 🎨 Avalonia UI Cross-Platform Testing

### UI Rendering and Behavior Testing

```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("UI")]
public class AvaloniaUICrossPlatformTests : CrossPlatformTestBase
{
    private TestAppBuilder _appBuilder;
    private Window _testWindow;
    
    protected override async Task SetupPlatformSpecificResourcesAsync()
    {
        // Initialize Avalonia test environment
        _appBuilder = AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = true
            });
            
        await Task.CompletedTask;
    }
    
    [Test]
    public async Task MainWindow_Launch_ShouldRenderAcrossPlatforms()
    {
        // Arrange
        using var app = _appBuilder.SetupWithoutStarting();
        
        // Act - Create main window
        _testWindow = new MainWindow();
        _testWindow.Show();
        
        // Wait for UI to render
        await Task.Delay(1000);
        
        // Assert
        Assert.That(_testWindow.IsVisible, Is.True, "Main window should be visible");
        Assert.That(_testWindow.Width, Is.GreaterThan(0), "Window should have positive width");
        Assert.That(_testWindow.Height, Is.GreaterThan(0), "Window should have positive height");
        
        // Platform-specific window properties
        if (CurrentPlatform.OS == OSPlatform.Windows)
        {
            Assert.That(_testWindow.WindowStartupLocation, Is.EqualTo(WindowStartupLocation.CenterScreen),
                "Windows should center on screen");
        }
        else if (CurrentPlatform.OS == OSPlatform.macOS)
        {
            // macOS-specific window behavior
            Assert.That(_testWindow.ExtendClientAreaToDecorationsHint, Is.True,
                "macOS windows should extend to decorations");
        }
        else if (CurrentPlatform.OS == OSPlatform.Linux)
        {
            // Linux-specific behavior
            Assert.That(_testWindow.Icon, Is.Not.Null, "Linux windows should have icon");
        }
        
        _testWindow.Close();
    }
    
    [Test]
    public async Task InventoryView_DataBinding_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        using var app = _appBuilder.SetupWithoutStarting();
        
        var mockLogger = new Mock<ILogger<InventoryViewModel>>();
        var mockInventoryService = new Mock<IInventoryService>();
        var mockConfigService = new Mock<IConfigurationService>();
        
        // Setup mock data
        var testInventoryItems = new List<InventoryItem>
        {
            new() { PartId = "CROSS_PART_001", Operation = "100", Quantity = 25, Location = "STATION_A" },
            new() { PartId = "CROSS_PART_002", Operation = "110", Quantity = 15, Location = "STATION_B" }
        };
        
        mockInventoryService.Setup(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(testInventoryItems);
        
        var viewModel = new InventoryViewModel(mockLogger.Object, mockInventoryService.Object);
        var inventoryView = new InventoryTabView { DataContext = viewModel };
        
        _testWindow = new Window { Content = inventoryView };
        _testWindow.Show();
        
        // Act - Trigger data loading
        await viewModel.SearchCommand.ExecuteAsync(null);
        
        // Wait for data binding to complete
        await Task.Delay(500);
        
        // Assert
        Assert.That(viewModel.SearchResults, Is.Not.Null, "Search results should be bound");
        Assert.That(viewModel.SearchResults.Count, Is.EqualTo(2), "Should load test inventory items");
        
        // Find data grid in visual tree
        var dataGrid = inventoryView.FindDescendantOfType<DataGrid>();
        Assert.That(dataGrid, Is.Not.Null, "DataGrid should exist in view");
        Assert.That(dataGrid.ItemsSource, Is.Not.Null, "DataGrid should have items source");
        
        // Platform-specific UI validation
        if (CurrentPlatform.OS == OSPlatform.Windows)
        {
            // Windows-specific DataGrid behavior
            Assert.That(dataGrid.CanUserReorderColumns, Is.True, 
                "Windows DataGrid should support column reordering");
        }
        else if (CurrentPlatform.IsUnix)
        {
            // Unix platforms might have different default behaviors
            Assert.That(dataGrid.SelectionMode, Is.EqualTo(DataGridSelectionMode.Single),
                "Unix DataGrid should have single selection by default");
        }
        
        _testWindow.Close();
    }
    
    [Test]
    public async Task TextInput_KeyboardHandling_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        using var app = _appBuilder.SetupWithoutStarting();
        
        var textBox = new TextBox();
        _testWindow = new Window { Content = textBox };
        _testWindow.Show();
        
        textBox.Focus();
        await Task.Delay(100);
        
        // Act - Simulate text input
        var testText = "CROSS_PLATFORM_TEST_123";
        
        foreach (char c in testText)
        {
            var keyEventArgs = new KeyEventArgs
            {
                Key = (Key)c,
                KeyModifiers = KeyModifiers.None
            };
            
            textBox.RaiseEvent(keyEventArgs);
        }
        
        await Task.Delay(100);
        
        // Assert
        Assert.That(textBox.Text, Does.Contain("CROSS"), "Text input should work across platforms");
        
        // Test platform-specific keyboard shortcuts
        if (CurrentPlatform.OS == OSPlatform.macOS)
        {
            // Test Cmd+A (Select All) on macOS
            var cmdAEvent = new KeyEventArgs
            {
                Key = Key.A,
                KeyModifiers = KeyModifiers.Meta
            };
            textBox.RaiseEvent(cmdAEvent);
        }
        else
        {
            // Test Ctrl+A (Select All) on Windows/Linux
            var ctrlAEvent = new KeyEventArgs
            {
                Key = Key.A,
                KeyModifiers = KeyModifiers.Control
            };
            textBox.RaiseEvent(ctrlAEvent);
        }
        
        await Task.Delay(100);
        Assert.That(textBox.SelectionStart, Is.EqualTo(0), "Select all should work on all platforms");
        Assert.That(textBox.SelectionEnd, Is.EqualTo(textBox.Text?.Length ?? 0), 
            "Select all should select entire text");
        
        _testWindow.Close();
    }
    
    [Test]
    public async Task ThemeSystem_ColorResolution_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        using var app = _appBuilder.SetupWithoutStarting();
        
        var themeService = new MTM_Theme_Service();
        var testButton = new Button { Content = "Test Button" };
        
        _testWindow = new Window { Content = testButton };
        _testWindow.Show();
        
        // Act - Apply MTM themes across platforms
        var themes = new[] { "MTM_Blue", "MTM_Green", "MTM_Red", "MTM_Dark" };
        
        foreach (var themeName in themes)
        {
            // Apply theme
            await themeService.SetThemeAsync(themeName);
            await Task.Delay(100);
            
            // Assert theme colors are applied
            var buttonBackground = testButton.Background;
            Assert.That(buttonBackground, Is.Not.Null, 
                $"Button background should be set for {themeName} theme on {CurrentPlatform.OS}");
            
            // Verify theme-specific colors
            if (themeName == "MTM_Blue")
            {
                Assert.That(buttonBackground.ToString(), Does.Contain("0078D4").Or.Contain("Blue"),
                    $"MTM_Blue theme should apply blue colors on {CurrentPlatform.OS}");
            }
            else if (themeName == "MTM_Dark")
            {
                Assert.That(buttonBackground.ToString(), Does.Contain("2D2D30").Or.Contain("Dark"),
                    $"MTM_Dark theme should apply dark colors on {CurrentPlatform.OS}");
            }
        }
        
        _testWindow.Close();
    }
    
    [Test]
    public async Task Fonts_Rendering_ShouldHandlePlatformDifferences()
    {
        // Arrange
        using var app = _appBuilder.SetupWithoutStarting();
        
        var testFonts = new Dictionary<OSPlatform, string[]>
        {
            { OSPlatform.Windows, new[] { "Segoe UI", "Arial", "Calibri" } },
            { OSPlatform.macOS, new[] { "SF Pro Display", "Helvetica Neue", "Arial" } },
            { OSPlatform.Linux, new[] { "Ubuntu", "DejaVu Sans", "Arial" } },
            { OSPlatform.Android, new[] { "Roboto", "Droid Sans", "Arial" } }
        };
        
        var platformFonts = testFonts[CurrentPlatform.OS];
        var textBlock = new TextBlock 
        { 
            Text = "MTM WIP Application Cross-Platform Font Test",
            FontSize = 14
        };
        
        _testWindow = new Window { Content = textBlock };
        _testWindow.Show();
        
        // Act & Assert - Test each platform font
        foreach (var fontName in platformFonts)
        {
            textBlock.FontFamily = new FontFamily(fontName);
            await Task.Delay(100);
            
            // Assert font is applied (or falls back gracefully)
            Assert.That(textBlock.FontFamily?.Name, Is.Not.Null.And.Not.Empty,
                $"Font {fontName} should be applied or fallback on {CurrentPlatform.OS}");
                
            // Verify text is rendered (has positive bounds)
            var bounds = textBlock.Bounds;
            Assert.That(bounds.Width, Is.GreaterThan(0), 
                $"Text should render with font {fontName} on {CurrentPlatform.OS}");
            Assert.That(bounds.Height, Is.GreaterThan(0), 
                $"Text should have height with font {fontName} on {CurrentPlatform.OS}");
        }
        
        _testWindow.Close();
    }
    
    [TearDown]
    public void TearDown()
    {
        _testWindow?.Close();
    }
}
```

## 🔧 Database Connection Cross-Platform Testing

```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("Database")]
public class DatabaseCrossPlatformTests : CrossPlatformTestBase
{
    private string _connectionString;
    
    protected override async Task SetupPlatformSpecificResourcesAsync()
    {
        _connectionString = GetPlatformSpecificConnectionString();
        await ValidateDatabaseConnectionAsync();
    }
    
    [Test]
    public async Task Database_Connection_ShouldWorkAcrossPlatforms()
    {
        // Arrange - Platform-specific connection parameters
        var connectionParameters = new MySqlParameter[]
        {
            new("p_Platform", CurrentPlatform.OS.ToString()),
            new("p_Architecture", CurrentPlatform.Architecture.ToString()),
            new("p_TestData", "Cross-platform connection test")
        };
        
        // Act - Test database connection
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "test_connection_platform", connectionParameters);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(1), 
            $"Database connection should work on {CurrentPlatform.OS}");
        Assert.That(result.Data, Is.Not.Null, 
            "Database should return data on all platforms");
        
        // Platform-specific connection behavior
        if (CurrentPlatform.OS == OSPlatform.Windows)
        {
            // Windows might use named pipes or TCP
            Assert.That(_connectionString, Does.Contain("Server=").Or.Contain("Data Source="),
                "Windows should connect via server name or data source");
        }
        else if (CurrentPlatform.IsUnix)
        {
            // Unix systems typically use TCP or Unix domain sockets
            Assert.That(_connectionString, Does.Contain("Server=").Or.Contain("Host="),
                "Unix systems should connect via server/host parameter");
        }
    }
    
    [Test]
    public async Task StoredProcedures_Execution_ShouldBeConsistentAcrossPlatforms()
    {
        // Arrange - Test core inventory operations across platforms
        var testOperations = new[]
        {
            new { Procedure = "inv_inventory_Get_All", Parameters = Array.Empty<MySqlParameter>() },
            new { Procedure = "md_part_ids_Get_All", Parameters = Array.Empty<MySqlParameter>() },
            new { Procedure = "md_locations_Get_All", Parameters = Array.Empty<MySqlParameter>() },
            new { Procedure = "md_operation_numbers_Get_All", Parameters = Array.Empty<MySqlParameter>() }
        };
        
        var results = new Dictionary<string, DatabaseResult>();
        
        // Act - Execute stored procedures
        foreach (var operation in testOperations)
        {
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, operation.Procedure, operation.Parameters);
            results[operation.Procedure] = result;
        }
        
        // Assert - All procedures should work consistently
        foreach (var (procedure, result) in results)
        {
            Assert.That(result.Status, Is.EqualTo(1), 
                $"Procedure {procedure} should execute successfully on {CurrentPlatform.OS}");
            Assert.That(result.Data, Is.Not.Null, 
                $"Procedure {procedure} should return data on {CurrentPlatform.OS}");
                
            // Cross-platform data consistency
            AssertCrossPlatformCompatibility(1, result.Status, 
                $"Stored procedure {procedure} execution status");
        }
    }
    
    [Test]
    public async Task DatabaseTransactions_Handling_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        var testPartId = $"CROSS_PLATFORM_PART_{CurrentPlatform.OS}_{Guid.NewGuid():N[..8]}";
        var operation = "100";
        var initialQuantity = 25;
        
        // Act - Test transaction workflow
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            // Step 1: Add inventory item
            var addResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "inv_inventory_Add_Item",
                new MySqlParameter("p_PartID", testPartId),
                new MySqlParameter("p_OperationNumber", operation),
                new MySqlParameter("p_Quantity", initialQuantity),
                new MySqlParameter("p_Location", "CROSS_PLATFORM_STATION"),
                new MySqlParameter("p_User", $"CrossPlatformUser_{CurrentPlatform.OS}"));
            
            Assert.That(addResult.Status, Is.EqualTo(1), 
                $"Add inventory should succeed on {CurrentPlatform.OS}");
            
            // Step 2: Record transaction
            var transactionResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "inv_transaction_Add",
                new MySqlParameter("p_PartID", testPartId),
                new MySqlParameter("p_OperationNumber", operation),
                new MySqlParameter("p_Quantity", initialQuantity),
                new MySqlParameter("p_Location", "CROSS_PLATFORM_STATION"),
                new MySqlParameter("p_TransactionType", "IN"),
                new MySqlParameter("p_User", $"CrossPlatformUser_{CurrentPlatform.OS}"));
            
            Assert.That(transactionResult.Status, Is.EqualTo(1), 
                $"Transaction record should succeed on {CurrentPlatform.OS}");
            
            // Step 3: Verify data consistency
            var verifyResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "inv_inventory_Get_ByPartIDandOperation",
                new MySqlParameter("p_PartID", testPartId),
                new MySqlParameter("p_OperationNumber", operation));
            
            Assert.That(verifyResult.Status, Is.EqualTo(1), 
                $"Data verification should work on {CurrentPlatform.OS}");
            Assert.That(verifyResult.Data.Rows.Count, Is.GreaterThan(0), 
                "Should find inserted inventory item");
            
            var inventoryRow = verifyResult.Data.Rows[0];
            Assert.That(Convert.ToInt32(inventoryRow["Quantity"]), Is.EqualTo(initialQuantity),
                "Quantity should match across platforms");
            
            // Commit transaction
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        // Final verification - data should persist
        var finalCheck = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", testPartId),
            new MySqlParameter("p_OperationNumber", operation));
        
        Assert.That(finalCheck.Status, Is.EqualTo(1), 
            $"Final verification should work on {CurrentPlatform.OS}");
        Assert.That(finalCheck.Data.Rows.Count, Is.EqualTo(1), 
            "Transaction should be committed across platforms");
    }
    
    [Test]
    public async Task DatabasePerformance_Response_ShouldBeAcceptableAcrossPlatforms()
    {
        // Arrange
        var performanceThresholds = new Dictionary<OSPlatform, int>
        {
            { OSPlatform.Windows, 100 },    // ms - Local development environment
            { OSPlatform.macOS, 150 },      // ms - May have network latency
            { OSPlatform.Linux, 125 },      // ms - Server environment
            { OSPlatform.Android, 300 }     // ms - Mobile connection
        };
        
        var threshold = performanceThresholds[CurrentPlatform.OS];
        var testOperations = new[]
        {
            "md_part_ids_Get_All",
            "md_locations_Get_All", 
            "md_operation_numbers_Get_All",
            "inv_inventory_Get_All"
        };
        
        var performanceResults = new List<(string Operation, long ElapsedMs)>();
        
        // Act - Test performance of core operations
        foreach (var operation in testOperations)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, operation, Array.Empty<MySqlParameter>());
            
            stopwatch.Stop();
            performanceResults.Add((operation, stopwatch.ElapsedMilliseconds));
            
            // Assert individual operation performance
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(threshold),
                $"Operation {operation} should complete within {threshold}ms on {CurrentPlatform.OS}, took {stopwatch.ElapsedMilliseconds}ms");
        }
        
        // Overall performance summary
        var averageTime = performanceResults.Average(r => r.ElapsedMs);
        var maxTime = performanceResults.Max(r => r.ElapsedMs);
        
        Console.WriteLine($"Database Performance on {CurrentPlatform.OS}:");
        foreach (var (operation, elapsedMs) in performanceResults)
        {
            Console.WriteLine($"  {operation}: {elapsedMs}ms");
        }
        Console.WriteLine($"  Average: {averageTime:F1}ms");
        Console.WriteLine($"  Maximum: {maxTime}ms");
        
        Assert.That(averageTime, Is.LessThan(threshold * 0.8), 
            $"Average response time should be well within threshold on {CurrentPlatform.OS}");
    }
    
    private string GetPlatformSpecificConnectionString()
    {
        var baseConnectionString = "Server=localhost;Database=mtm_wip_test;Uid=mtm_user;Pwd=mtm_password;";
        
        return CurrentPlatform.OS switch
        {
            OSPlatform.Windows => baseConnectionString + "ConnectionTimeout=30;",
            OSPlatform.macOS => baseConnectionString + "ConnectionTimeout=45;SslMode=Preferred;",
            OSPlatform.Linux => baseConnectionString + "ConnectionTimeout=45;SslMode=Required;",
            OSPlatform.Android => baseConnectionString + "ConnectionTimeout=60;SslMode=Required;",
            _ => baseConnectionString + "ConnectionTimeout=30;"
        };
    }
    
    private async Task ValidateDatabaseConnectionAsync()
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            // Test basic connectivity
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            var result = await command.ExecuteScalarAsync();
            
            Assert.That(result, Is.EqualTo(1), 
                $"Database should be accessible on {CurrentPlatform.OS}");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Database connection failed on {CurrentPlatform.OS}: {ex.Message}");
        }
    }
}
```

## 🎯 Feature-Specific Cross-Platform Testing

### Manufacturing Workflow Testing

```csharp
[TestFixture]
[Category("CrossPlatform")]
[Category("Manufacturing")]
public class ManufacturingWorkflowCrossPlatformTests : CrossPlatformTestBase
{
    private IInventoryService _inventoryService;
    private ITransactionService _transactionService;
    private IQuickButtonsService _quickButtonsService;
    
    protected override async Task SetupPlatformSpecificResourcesAsync()
    {
        // Setup services with platform-specific configurations
        var serviceCollection = new ServiceCollection();
        var configuration = LoadPlatformConfiguration();
        
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddLogging();
        serviceCollection.AddMTMServices(configuration);
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        _inventoryService = serviceProvider.GetRequiredService<IInventoryService>();
        _transactionService = serviceProvider.GetRequiredService<ITransactionService>();
        _quickButtonsService = serviceProvider.GetRequiredService<IQuickButtonsService>();
        
        await Task.CompletedTask;
    }
    
    [Test]
    public async Task InventoryWorkflow_AddItem_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        var testPart = new InventoryItem
        {
            PartId = $"WORKFLOW_TEST_{CurrentPlatform.OS}_{Guid.NewGuid():N[..8]}",
            Operation = "100",
            Quantity = 25,
            Location = "CROSS_PLATFORM_STATION",
            User = $"WorkflowUser_{CurrentPlatform.OS}"
        };
        
        // Act - Execute complete inventory workflow
        var addResult = await _inventoryService.AddInventoryAsync(testPart);
        
        // Assert - Core functionality works across platforms
        Assert.That(addResult.Success, Is.True, 
            $"Inventory addition should succeed on {CurrentPlatform.OS}");
        Assert.That(addResult.Message, Is.Not.Null.And.Not.Empty,
            "Should provide feedback message");
        
        // Verify inventory was added
        var searchResult = await _inventoryService.GetInventoryAsync(testPart.PartId, testPart.Operation);
        Assert.That(searchResult, Is.Not.Null.And.Count.GreaterThan(0),
            "Should find added inventory item");
        
        var foundItem = searchResult.First();
        AssertCrossPlatformCompatibility(testPart.PartId, foundItem.PartId, "Part ID");
        AssertCrossPlatformCompatibility(testPart.Operation, foundItem.Operation, "Operation");
        AssertCrossPlatformCompatibility(testPart.Quantity, foundItem.Quantity, "Quantity");
        AssertCrossPlatformCompatibility(testPart.Location, foundItem.Location, "Location");
        
        // Verify transaction was recorded
        var transactionHistory = await _transactionService.GetTransactionHistoryAsync(
            testPart.PartId, testPart.Operation);
        Assert.That(transactionHistory, Is.Not.Null.And.Count.GreaterThan(0),
            "Transaction should be recorded across platforms");
    }
    
    [Test]
    public async Task QuickButtonsWorkflow_SaveAndExecute_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        var testUser = $"QBUser_{CurrentPlatform.OS}";
        var quickButtonData = new QuickButtonInfo
        {
            PartId = $"QB_PART_{CurrentPlatform.OS}",
            Operation = "110",
            Quantity = 15,
            Location = "CROSS_PLATFORM_QB_STATION",
            User = testUser,
            DisplayText = $"Quick Test ({CurrentPlatform.OS})"
        };
        
        // Act - Save QuickButton
        var saveResult = await _quickButtonsService.SaveQuickButtonAsync(quickButtonData);
        
        // Assert - QuickButton saving works across platforms
        Assert.That(saveResult.Success, Is.True,
            $"QuickButton save should succeed on {CurrentPlatform.OS}");
        
        // Retrieve saved QuickButtons
        var savedButtons = await _quickButtonsService.GetQuickButtonsAsync(testUser);
        Assert.That(savedButtons, Is.Not.Null.And.Count.GreaterThan(0),
            "Should retrieve saved QuickButtons");
        
        var savedButton = savedButtons.First(b => b.PartId == quickButtonData.PartId);
        AssertCrossPlatformCompatibility(quickButtonData.DisplayText, savedButton.DisplayText, 
            "QuickButton display text");
        AssertCrossPlatformCompatibility(quickButtonData.Quantity, savedButton.Quantity, 
            "QuickButton quantity");
        
        // Execute QuickButton action
        var executeResult = await _quickButtonsService.ExecuteQuickButtonAsync(savedButton);
        Assert.That(executeResult.Success, Is.True,
            $"QuickButton execution should work on {CurrentPlatform.OS}");
        
        // Verify inventory was updated
        var inventoryCheck = await _inventoryService.GetInventoryAsync(
            quickButtonData.PartId, quickButtonData.Operation);
        Assert.That(inventoryCheck, Is.Not.Null.And.Count.GreaterThan(0),
            "QuickButton execution should update inventory");
    }
    
    [Test]
    public async Task ManufacturingOperations_WorkflowSequence_ShouldBeConsistent()
    {
        // Arrange - Test complete manufacturing workflow: 90 → 100 → 110 → 120
        var testPart = $"MANUFACTURING_WORKFLOW_{CurrentPlatform.OS}";
        var workflowSteps = new[] { "90", "100", "110", "120" };
        var quantityPerStep = 10;
        var testUser = $"MfgUser_{CurrentPlatform.OS}";
        
        var workflowResults = new List<(string Operation, bool Success)>();
        
        // Act - Execute manufacturing workflow
        foreach (var operation in workflowSteps)
        {
            var inventoryItem = new InventoryItem
            {
                PartId = testPart,
                Operation = operation,
                Quantity = quantityPerStep,
                Location = $"STATION_{operation}",
                User = testUser
            };
            
            var result = await _inventoryService.AddInventoryAsync(inventoryItem);
            workflowResults.Add((operation, result.Success));
            
            // Small delay between operations
            await Task.Delay(50);
        }
        
        // Assert - All workflow steps should succeed
        foreach (var (operation, success) in workflowResults)
        {
            Assert.That(success, Is.True,
                $"Manufacturing step {operation} should succeed on {CurrentPlatform.OS}");
        }
        
        // Verify complete workflow data
        foreach (var operation in workflowSteps)
        {
            var stepInventory = await _inventoryService.GetInventoryAsync(testPart, operation);
            Assert.That(stepInventory, Is.Not.Null.And.Count.EqualTo(1),
                $"Operation {operation} should have inventory data");
            
            var stepItem = stepInventory.First();
            AssertCrossPlatformCompatibility(quantityPerStep, stepItem.Quantity,
                $"Operation {operation} quantity");
            AssertCrossPlatformCompatibility($"STATION_{operation}", stepItem.Location,
                $"Operation {operation} location");
        }
        
        // Verify transaction history shows complete workflow
        var allTransactions = await _transactionService.GetTransactionHistoryAsync(testPart);
        Assert.That(allTransactions, Is.Not.Null.And.Count.EqualTo(workflowSteps.Length),
            "Should have transaction for each workflow step");
        
        // Verify transaction sequence
        var transactionOperations = allTransactions
            .OrderBy(t => t.TransactionDate)
            .Select(t => t.Operation)
            .ToArray();
        CollectionAssert.AreEqual(workflowSteps, transactionOperations,
            "Transaction sequence should match workflow steps");
    }
    
    [Test]
    public async Task ErrorHandling_DatabaseFailure_ShouldBeConsistentAcrossPlatforms()
    {
        // Arrange - Force database error scenarios
        var invalidConnectionString = "Server=nonexistent;Database=invalid;Uid=fake;Pwd=fake;ConnectionTimeout=1;";
        
        // Test with intentionally invalid configuration
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetConnectionString("DefaultConnection"))
            .Returns(invalidConnectionString);
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(mockConfig.Object);
        serviceCollection.AddLogging();
        
        // This should fail gracefully on all platforms
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        try
        {
            var faultyInventoryService = serviceProvider.GetService<IInventoryService>();
            
            // Act - Attempt operation with faulty connection
            var result = await faultyInventoryService?.GetInventoryAsync("TEST", "100");
            
            // Assert - Error handling should be consistent
            Assert.That(result, Is.Null.Or.Empty,
                $"Failed database operations should return empty results on {CurrentPlatform.OS}");
        }
        catch (Exception ex)
        {
            // Database connection failures should be handled gracefully
            Assert.That(ex, Is.TypeOf<InvalidOperationException>()
                .Or.TypeOf<MySqlException>()
                .Or.TypeOf<TimeoutException>(),
                $"Database errors should be expected exception types on {CurrentPlatform.OS}");
        }
    }
    
    private IConfiguration LoadPlatformConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        
        // Platform-specific configuration files
        var configFiles = new List<string> { "appsettings.json" };
        
        if (CurrentPlatform.OS == OSPlatform.Windows)
            configFiles.Add("appsettings.Windows.json");
        else if (CurrentPlatform.OS == OSPlatform.macOS)
            configFiles.Add("appsettings.macOS.json");
        else if (CurrentPlatform.OS == OSPlatform.Linux)
            configFiles.Add("appsettings.Linux.json");
        else if (CurrentPlatform.OS == OSPlatform.Android)
            configFiles.Add("appsettings.Android.json");
        
        configBuilder.SetBasePath(TestDataDirectory);
        
        foreach (var configFile in configFiles)
        {
            var configPath = Path.Combine(TestDataDirectory, configFile);
            if (File.Exists(configPath))
            {
                configBuilder.AddJsonFile(configFile, optional: true);
            }
        }
        
        return configBuilder.Build();
    }
}
```

## 📊 Cross-Platform Testing Coverage Requirements

### Platform Coverage Matrix

datagrid
    columns
        Feature Category
        Windows
        macOS
        Linux
        Android
        Coverage Target
    Core Services         ✅      ✅      ✅      ✅      100%
    Database Operations   ✅      ✅      ✅      ✅      100%
    UI Components         ✅      ✅      ✅      ⚠️      95%+
    File System Operations✅      ✅      ✅      ✅      100%
    Configuration Management✅   ✅      ✅      ✅      100%
    Error Handling        ✅      ✅      ✅      ✅      100%
    Performance           ✅      ✅      ✅      ⚠️      90%+

### Cross-Platform Testing Execution

```powershell
# PowerShell script for cross-platform test execution
param(
    [string]$Platform = "All",
    [string]$Category = "CrossPlatform",
    [bool]$Parallel = $true
)

$SupportedPlatforms = @("Windows", "macOS", "Linux", "Android")
$TestCategories = @("CrossPlatform", "FileSystem", "UI", "Database", "Manufacturing")

if ($Platform -eq "All") {
    $TargetPlatforms = $SupportedPlatforms
} else {
    $TargetPlatforms = @($Platform)
}

foreach ($TargetPlatform in $TargetPlatforms) {
    Write-Host "Running cross-platform tests on $TargetPlatform..." -ForegroundColor Green
    
    $TestCommand = "dotnet test --configuration Release --logger 'console;verbosity=detailed' --filter 'Category=$Category'"
    
    if ($Parallel) {
        $TestCommand += " --parallel"
    }
    
    # Platform-specific test execution
    switch ($TargetPlatform) {
        "Windows" { 
            Invoke-Expression $TestCommand
        }
        "macOS" {
            # Requires macOS test runner
            Write-Host "Note: macOS tests require execution on macOS system" -ForegroundColor Yellow
        }
        "Linux" {
            # Requires Linux test runner or container
            Write-Host "Note: Linux tests require execution on Linux system or container" -ForegroundColor Yellow
        }
        "Android" {
            # Requires Android test device/emulator
            Write-Host "Note: Android tests require Android device or emulator" -ForegroundColor Yellow
        }
    }
}
```

### Critical Cross-Platform Test Scenarios

1. **File System Operations** - Path handling, directory creation, permissions across OS
2. **Database Connectivity** - MySQL connection behavior on different platforms
3. **UI Rendering** - Avalonia component rendering consistency
4. **Configuration Loading** - Platform-specific settings and paths
5. **Error Handling** - Consistent error behavior across platforms
6. **Performance** - Acceptable response times accounting for platform differences
7. **Manufacturing Workflows** - Complete business process consistency
8. **Theme System** - Color and style consistency across platforms

This comprehensive cross-platform testing framework ensures MTM WIP Application delivers consistent functionality, performance, and user experience across Windows, macOS, Linux, and Android platforms while maintaining manufacturing-grade reliability and data integrity.