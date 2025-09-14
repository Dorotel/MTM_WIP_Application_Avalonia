---
description: 'Prompt template for creating performance tests for MTM WIP Application using NBomber and performance monitoring'
applies_to: '**/*'
---

# Create Performance Test Prompt Template

## 🎯 Objective

Generate comprehensive performance tests for MTM WIP Application using NBomber framework, focusing on load testing, stress testing, and performance profiling of critical manufacturing workflows. Ensure application meets manufacturing-grade performance requirements.

## 📋 Instructions

When creating performance tests, follow these specific requirements:

### Performance Test Structure

1. **Use MTM Performance Test Base Class**
   ```csharp
   [TestFixture]
   [Category("Performance")]
   [Category("{PerformanceCategory}")]  // e.g., Load, Stress, Spike, Volume, Endurance
   public class {ComponentName}PerformanceTests : PerformanceTestBase
   {
       // Performance test implementation with NBomber
   }
   ```

2. **Performance Test Categories**
   - Load Tests: Normal expected load scenarios
   - Stress Tests: Beyond normal capacity testing
   - Spike Tests: Sudden load increase testing
   - Volume Tests: Large data set processing
   - Endurance Tests: Extended duration testing
   - Memory Tests: Memory usage and leak detection

### NBomber Performance Framework Setup

#### Base Performance Test Class
```csharp
public abstract class PerformanceTestBase
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IConfiguration TestConfiguration { get; private set; }
    protected PerformanceMetrics TestMetrics { get; private set; }
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        // Setup test services
        var services = new ServiceCollection();
        ConfigurePerformanceTestServices(services);
        ServiceProvider = services.BuildServiceProvider();
        
        // Initialize performance metrics
        TestMetrics = new PerformanceMetrics();
        
        await Task.CompletedTask;
    }
    
    [SetUp]
    public virtual async Task SetUp()
    {
        TestMetrics.Reset();
        await Task.CompletedTask;
    }
    
    [TearDown]
    public virtual async Task TearDown()
    {
        // Report performance results
        ReportPerformanceMetrics();
        await Task.CompletedTask;
    }
    
    protected virtual void ConfigurePerformanceTestServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddMTMServices(GetTestConfiguration());
    }
    
    protected IConfiguration GetTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.Performance.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    protected T GetService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }
    
    protected void ReportPerformanceMetrics()
    {
        Console.WriteLine("=== Performance Test Results ===");
        Console.WriteLine($"Test Duration: {TestMetrics.Duration.TotalSeconds:F1}s");
        Console.WriteLine($"Operations: {TestMetrics.TotalOperations}");
        Console.WriteLine($"Success Rate: {TestMetrics.SuccessRate:P}");
        Console.WriteLine($"Average Response Time: {TestMetrics.AverageResponseTime:F2}ms");
        Console.WriteLine($"95th Percentile: {TestMetrics.Percentile95:F2}ms");
        Console.WriteLine($"99th Percentile: {TestMetrics.Percentile99:F2}ms");
        Console.WriteLine($"Throughput: {TestMetrics.OperationsPerSecond:F1} ops/sec");
        Console.WriteLine($"Peak Memory: {TestMetrics.PeakMemoryMB:F1} MB");
        Console.WriteLine("===============================");
    }
    
    protected async Task<NBomberResponse[]> RunLoadTestAsync(Scenario scenario, LoadTestConfig config)
    {
        var nbomberConfig = NBomberRunner
            .RegisterScenarios(scenario)
            .WithLoadSimulations(config.LoadSimulations)
            .WithTestSuite(config.TestSuiteName)
            .WithTestName(config.TestName);
        
        if (config.Duration.HasValue)
            nbomberConfig = nbomberConfig.WithScenarioSettings(ScenarioSettings.Create().WithDuration(config.Duration.Value));
        
        var stats = nbomberConfig.Run();
        
        // Update test metrics
        TestMetrics.UpdateFromNBomberStats(stats);
        
        return stats.AllOkResponses;
    }
}

public class LoadTestConfig
{
    public string TestName { get; set; } = "Performance Test";
    public string TestSuiteName { get; set; } = "MTM Performance Suite";
    public TimeSpan? Duration { get; set; }
    public LoadSimulation[] LoadSimulations { get; set; } = Array.Empty<LoadSimulation>();
}

public class PerformanceMetrics
{
    public TimeSpan Duration { get; set; }
    public int TotalOperations { get; set; }
    public int SuccessfulOperations { get; set; }
    public double SuccessRate => TotalOperations > 0 ? (double)SuccessfulOperations / TotalOperations : 0;
    public double AverageResponseTime { get; set; }
    public double Percentile95 { get; set; }
    public double Percentile99 { get; set; }
    public double OperationsPerSecond { get; set; }
    public double PeakMemoryMB { get; set; }
    
    public void Reset()
    {
        Duration = TimeSpan.Zero;
        TotalOperations = 0;
        SuccessfulOperations = 0;
        AverageResponseTime = 0;
        Percentile95 = 0;
        Percentile99 = 0;
        OperationsPerSecond = 0;
        PeakMemoryMB = 0;
    }
    
    public void UpdateFromNBomberStats(NBomberStats stats)
    {
        var scenario = stats.AllOkScenarios.FirstOrDefault();
        if (scenario != null)
        {
            Duration = scenario.Duration;
            TotalOperations = scenario.AllOkCount + scenario.AllFailCount;
            SuccessfulOperations = scenario.AllOkCount;
            AverageResponseTime = scenario.Ok.Mean;
            Percentile95 = scenario.Ok.Percentile95;
            Percentile99 = scenario.Ok.Percentile99;
            OperationsPerSecond = scenario.Ok.Request.Count / Duration.TotalSeconds;
        }
        
        // Memory usage would be tracked separately
        PeakMemoryMB = GC.GetTotalMemory(false) / (1024.0 * 1024.0);
    }
}
```

### Database Performance Tests

#### High-Volume Database Operations
```csharp
[TestFixture]
[Category("Performance")]
[Category("Database")]
public class DatabasePerformanceTests : PerformanceTestBase
{
    private IInventoryService _inventoryService;
    private ITransactionService _transactionService;
    
    protected override void ConfigurePerformanceTestServices(IServiceCollection services)
    {
        base.ConfigurePerformanceTestServices(services);
        services.AddTransient<IInventoryService, InventoryService>();
        services.AddTransient<ITransactionService, TransactionService>();
    }
    
    [OneTimeSetUp]
    public override async Task OneTimeSetUp()
    {
        await base.OneTimeSetUp();
        _inventoryService = GetService<IInventoryService>();
        _transactionService = GetService<ITransactionService>();
    }
    
    [Test]
    public async Task InventoryOperations_HighVolumeLoad_ShouldMaintainPerformance()
    {
        // Arrange - Performance thresholds
        var performanceThresholds = new
        {
            MaxAverageResponseTime = 100.0, // ms
            MinSuccessRate = 0.95, // 95%
            MinThroughput = 50.0, // operations per second
            MaxP95ResponseTime = 200.0 // ms
        };
        
        var testData = GenerateInventoryTestData(1000);
        var dataEnumerator = testData.GetEnumerator();
        var dataLock = new object();
        
        // Create NBomber scenario
        var scenario = Scenario.Create("high_volume_inventory", async context =>
        {
            InventoryItem testItem;
            lock (dataLock)
            {
                if (!dataEnumerator.MoveNext())
                {
                    dataEnumerator.Reset();
                    dataEnumerator.MoveNext();
                }
                testItem = dataEnumerator.Current;
            }
            
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var result = await _inventoryService.AddInventoryAsync(testItem);
                stopwatch.Stop();
                
                if (result.Success)
                {
                    return Response.Ok(statusCode: 200, sizeBytes: 1024);
                }
                else
                {
                    return Response.Fail(error: result.Message);
                }
            }
            catch (Exception ex)
            {
                return Response.Fail(error: ex.Message);
            }
        });
        
        var config = new LoadTestConfig
        {
            TestName = "High Volume Inventory Operations",
            TestSuiteName = "Database Performance Tests",
            Duration = TimeSpan.FromMinutes(2),
            LoadSimulations = new[]
            {
                Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromSeconds(30)), // Ramp up
                Simulation.KeepConstant(copies: 20, during: TimeSpan.FromMinutes(1)), // Sustained load
                Simulation.InjectPerSec(rate: 5, during: TimeSpan.FromSeconds(30))   // Ramp down
            }
        };
        
        // Act
        var responses = await RunLoadTestAsync(scenario, config);
        
        // Assert performance thresholds
        Assert.That(TestMetrics.AverageResponseTime, Is.LessThan(performanceThresholds.MaxAverageResponseTime),
            $"Average response time should be under {performanceThresholds.MaxAverageResponseTime}ms, got {TestMetrics.AverageResponseTime:F2}ms");
        
        Assert.That(TestMetrics.SuccessRate, Is.GreaterThanOrEqualTo(performanceThresholds.MinSuccessRate),
            $"Success rate should be at least {performanceThresholds.MinSuccessRate:P}, got {TestMetrics.SuccessRate:P}");
        
        Assert.That(TestMetrics.OperationsPerSecond, Is.GreaterThanOrEqualTo(performanceThresholds.MinThroughput),
            $"Throughput should be at least {performanceThresholds.MinThroughput} ops/sec, got {TestMetrics.OperationsPerSecond:F1}");
        
        Assert.That(TestMetrics.Percentile95, Is.LessThan(performanceThresholds.MaxP95ResponseTime),
            $"95th percentile should be under {performanceThresholds.MaxP95ResponseTime}ms, got {TestMetrics.Percentile95:F2}ms");
    }
    
    [Test]
    public async Task DatabaseConnections_ConcurrentAccess_ShouldScaleEfficiently()
    {
        // Arrange - Test database connection pooling under high concurrency
        var scenario = Scenario.Create("concurrent_database_access", async context =>
        {
            var partId = $"CONCURRENT_PART_{context.ScenarioInfo.ThreadId}_{context.InvocationNumber}";
            var testItem = new InventoryItem
            {
                PartId = partId,
                Operation = "100",
                Quantity = 1,
                Location = "PERF_STATION",
                User = $"PerfUser_{context.ScenarioInfo.ThreadId}"
            };
            
            try
            {
                var addResult = await _inventoryService.AddInventoryAsync(testItem);
                if (!addResult.Success)
                    return Response.Fail(error: addResult.Message);
                
                // Also test read operations
                var searchResult = await _inventoryService.GetInventoryAsync(partId, "100");
                if (searchResult == null || !searchResult.Any())
                    return Response.Fail(error: "Failed to retrieve added inventory");
                
                return Response.Ok();
            }
            catch (Exception ex)
            {
                return Response.Fail(error: ex.Message);
            }
        });
        
        var config = new LoadTestConfig
        {
            TestName = "Concurrent Database Access",
            Duration = TimeSpan.FromMinutes(1),
            LoadSimulations = new[]
            {
                Simulation.KeepConstant(copies: 50, during: TimeSpan.FromMinutes(1)) // High concurrency
            }
        };
        
        // Act
        var responses = await RunLoadTestAsync(scenario, config);
        
        // Assert - Should handle high concurrency without significant degradation
        Assert.That(TestMetrics.SuccessRate, Is.GreaterThanOrEqualTo(0.90),
            $"Should maintain 90%+ success rate under high concurrency, got {TestMetrics.SuccessRate:P}");
        
        Assert.That(TestMetrics.Percentile99, Is.LessThan(1000),
            $"99th percentile should be under 1 second even under high concurrency, got {TestMetrics.Percentile99:F2}ms");
    }
    
    private IEnumerable<InventoryItem> GenerateInventoryTestData(int count)
    {
        var operations = new[] { "90", "100", "110", "120" };
        var locations = new[] { "STATION_A", "STATION_B", "STATION_C", "STATION_D", "STATION_E" };
        
        for (int i = 1; i <= count; i++)
        {
            yield return new InventoryItem
            {
                PartId = $"PERF_PART_{i:0000}",
                Operation = operations[i % operations.Length],
                Quantity = 1 + (i % 100),
                Location = locations[i % locations.Length],
                User = $"PerfUser_{(i % 10) + 1}"
            };
        }
    }
}
```

### UI Performance Tests

#### View Rendering Performance
```csharp
[TestFixture]
[Category("Performance")]
[Category("UI")]
public class UIRenderingPerformanceTests : PerformanceTestBase
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
    public async Task InventoryView_LargeDataSet_ShouldRenderWithinThreshold()
    {
        // Arrange - Large data set for performance testing
        var largeInventoryData = GenerateInventoryTestData(10000).ToList();
        var renderingThresholds = new
        {
            MaxInitialRenderTime = 1000.0, // ms
            MaxScrollRenderTime = 100.0,   // ms
            MaxMemoryUsageMB = 500.0       // MB
        };
        
        using var app = _appBuilder.SetupWithoutStarting();
        
        var mockService = new Mock<IInventoryService>();
        mockService.Setup(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(largeInventoryData);
        
        var mockLogger = new Mock<ILogger<InventoryViewModel>>();
        var viewModel = new InventoryViewModel(mockLogger.Object, mockService.Object);
        
        var inventoryView = new InventoryTabView { DataContext = viewModel };
        var testWindow = new Window 
        { 
            Content = inventoryView,
            Width = 1200,
            Height = 800
        };
        
        // Act - Measure initial rendering time
        var initialMemory = GC.GetTotalMemory(true) / (1024.0 * 1024.0);
        var renderStopwatch = Stopwatch.StartNew();
        
        testWindow.Show();
        await viewModel.SearchCommand.ExecuteAsync(null); // Load large data set
        
        // Wait for UI to render completely
        await Task.Delay(500);
        renderStopwatch.Stop();
        
        var afterRenderMemory = GC.GetTotalMemory(false) / (1024.0 * 1024.0);
        var memoryUsed = afterRenderMemory - initialMemory;
        
        // Find DataGrid for scroll testing
        var dataGrid = inventoryView.FindDescendantOfType<DataGrid>();
        Assert.That(dataGrid, Is.Not.Null, "DataGrid should be rendered");
        
        // Test scroll performance
        var scrollStopwatch = Stopwatch.StartNew();
        
        // Simulate scrolling through data
        for (int i = 0; i < 100; i += 10)
        {
            dataGrid.ScrollIntoView(largeInventoryData[i * 50], null);
            await Task.Delay(1); // Allow UI to update
        }
        
        scrollStopwatch.Stop();
        
        // Assert performance thresholds
        Assert.That(renderStopwatch.ElapsedMilliseconds, Is.LessThan(renderingThresholds.MaxInitialRenderTime),
            $"Initial render should take less than {renderingThresholds.MaxInitialRenderTime}ms for {largeInventoryData.Count} items, took {renderStopwatch.ElapsedMilliseconds}ms");
        
        Assert.That(scrollStopwatch.ElapsedMilliseconds / 10.0, Is.LessThan(renderingThresholds.MaxScrollRenderTime),
            $"Average scroll render should take less than {renderingThresholds.MaxScrollRenderTime}ms, took {scrollStopwatch.ElapsedMilliseconds / 10.0:F1}ms");
        
        Assert.That(memoryUsed, Is.LessThan(renderingThresholds.MaxMemoryUsageMB),
            $"Memory usage should be less than {renderingThresholds.MaxMemoryUsageMB}MB for large data set, used {memoryUsed:F1}MB");
        
        // Performance reporting
        Console.WriteLine($"UI Rendering Performance Results:");
        Console.WriteLine($"  Data Set Size: {largeInventoryData.Count:N0} items");
        Console.WriteLine($"  Initial Render Time: {renderStopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Average Scroll Time: {scrollStopwatch.ElapsedMilliseconds / 10.0:F1}ms");
        Console.WriteLine($"  Memory Used: {memoryUsed:F1}MB");
        Console.WriteLine($"  Items per MS (render): {largeInventoryData.Count / (double)renderStopwatch.ElapsedMilliseconds:F1}");
        
        testWindow.Close();
    }
    
    [Test]
    public async Task NavigationPerformance_TabSwitching_ShouldBeResponsive()
    {
        // Arrange
        using var app = _appBuilder.SetupWithoutStarting();
        
        var mainView = new MainView();
        var testWindow = new Window 
        { 
            Content = mainView,
            Width = 1024,
            Height = 768
        };
        
        testWindow.Show();
        await Task.Delay(100); // Allow initial render
        
        var tabControl = mainView.FindDescendantOfType<TabControl>();
        var tabs = tabControl.Items.Cast<TabItem>().ToList();
        
        var navigationThreshold = 200.0; // ms per tab switch
        
        // Act - Measure tab switching performance
        var switchTimes = new List<double>();
        
        for (int cycle = 0; cycle < 3; cycle++) // Test multiple cycles
        {
            foreach (var tab in tabs)
            {
                var switchStopwatch = Stopwatch.StartNew();
                
                tabControl.SelectedItem = tab;
                await Task.Delay(50); // Allow tab content to render
                
                switchStopwatch.Stop();
                switchTimes.Add(switchStopwatch.ElapsedMilliseconds);
            }
        }
        
        // Assert
        var averageSwitchTime = switchTimes.Average();
        var maxSwitchTime = switchTimes.Max();
        
        Assert.That(averageSwitchTime, Is.LessThan(navigationThreshold),
            $"Average tab switch should take less than {navigationThreshold}ms, took {averageSwitchTime:F1}ms");
        
        Assert.That(maxSwitchTime, Is.LessThan(navigationThreshold * 2),
            $"Maximum tab switch should take less than {navigationThreshold * 2}ms, took {maxSwitchTime:F1}ms");
        
        Console.WriteLine($"Navigation Performance Results:");
        Console.WriteLine($"  Tab Count: {tabs.Count}");
        Console.WriteLine($"  Switch Cycles: 3");
        Console.WriteLine($"  Average Switch Time: {averageSwitchTime:F1}ms");
        Console.WriteLine($"  Maximum Switch Time: {maxSwitchTime:F1}ms");
        Console.WriteLine($"  Minimum Switch Time: {switchTimes.Min():F1}ms");
        
        testWindow.Close();
    }
}
```

### Service Performance Tests

#### Business Logic Performance
```csharp
[TestFixture]
[Category("Performance")]
[Category("Services")]
public class ServicePerformanceTests : PerformanceTestBase
{
    [Test]
    public async Task QuickButtonsService_HighFrequencyExecution_ShouldMaintainPerformance()
    {
        // Arrange
        var quickButtonsService = GetService<IQuickButtonsService>();
        var testUser = "PerformanceTestUser";
        
        // Create test QuickButtons
        var quickButtons = new List<QuickButtonInfo>();
        for (int i = 1; i <= 100; i++)
        {
            quickButtons.Add(new QuickButtonInfo
            {
                PartId = $"QB_PERF_{i:000}",
                Operation = "100",
                Quantity = i % 50 + 1,
                Location = $"STATION_{(i % 5) + 1}",
                User = testUser,
                DisplayText = $"Quick Button {i} ({i % 50 + 1})"
            });
        }
        
        // Save all QuickButtons
        foreach (var qb in quickButtons)
        {
            await quickButtonsService.SaveQuickButtonAsync(qb);
        }
        
        // Create NBomber scenario for high-frequency execution
        var scenario = Scenario.Create("quickbutton_execution", async context =>
        {
            try
            {
                var randomIndex = Random.Shared.Next(quickButtons.Count);
                var quickButton = quickButtons[randomIndex];
                
                var result = await quickButtonsService.ExecuteQuickButtonAsync(quickButton);
                
                return result.Success 
                    ? Response.Ok() 
                    : Response.Fail(error: result.Message);
            }
            catch (Exception ex)
            {
                return Response.Fail(error: ex.Message);
            }
        });
        
        var config = new LoadTestConfig
        {
            TestName = "High Frequency QuickButton Execution",
            Duration = TimeSpan.FromMinutes(1),
            LoadSimulations = new[]
            {
                Simulation.InjectPerSec(rate: 5, during: TimeSpan.FromSeconds(15)),   // Ramp up
                Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(30)), // Sustained
                Simulation.InjectPerSec(rate: 2, during: TimeSpan.FromSeconds(15))    // Ramp down
            }
        };
        
        // Act
        var responses = await RunLoadTestAsync(scenario, config);
        
        // Assert
        Assert.That(TestMetrics.SuccessRate, Is.GreaterThanOrEqualTo(0.95),
            $"QuickButton execution should maintain 95%+ success rate, got {TestMetrics.SuccessRate:P}");
        
        Assert.That(TestMetrics.AverageResponseTime, Is.LessThan(150),
            $"Average QuickButton execution should be under 150ms, got {TestMetrics.AverageResponseTime:F2}ms");
        
        Assert.That(TestMetrics.OperationsPerSecond, Is.GreaterThanOrEqualTo(5),
            $"Should maintain throughput of at least 5 ops/sec, got {TestMetrics.OperationsPerSecond:F1}");
    }
    
    [Test]
    public async Task ConfigurationService_HighFrequencyAccess_ShouldCacheEffectively()
    {
        // Test that configuration service caching works under load
        var configService = GetService<IConfigurationService>();
        
        var scenario = Scenario.Create("config_access", async context =>
        {
            try
            {
                // Access various configuration settings
                var connectionString = await configService.GetConnectionStringAsync();
                var appName = await configService.GetSettingAsync<string>("MTM_Configuration:AppName");
                var logLevel = await configService.GetSettingAsync<string>("Logging:LogLevel:Default");
                
                if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(appName))
                    return Response.Fail(error: "Configuration not loaded");
                
                return Response.Ok();
            }
            catch (Exception ex)
            {
                return Response.Fail(error: ex.Message);
            }
        });
        
        var config = new LoadTestConfig
        {
            TestName = "High Frequency Configuration Access",
            Duration = TimeSpan.FromSeconds(30),
            LoadSimulations = new[]
            {
                Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30))
            }
        };
        
        // Act
        var responses = await RunLoadTestAsync(scenario, config);
        
        // Assert - Configuration access should be very fast due to caching
        Assert.That(TestMetrics.AverageResponseTime, Is.LessThan(10),
            $"Configuration access should be under 10ms (cached), got {TestMetrics.AverageResponseTime:F2}ms");
        
        Assert.That(TestMetrics.SuccessRate, Is.EqualTo(1.0),
            $"Configuration access should have 100% success rate, got {TestMetrics.SuccessRate:P}");
    }
}
```

### Memory Performance Tests

```csharp
[TestFixture]
[Category("Performance")]
[Category("Memory")]
public class MemoryPerformanceTests : PerformanceTestBase
{
    [Test]
    public async Task LongRunningOperations_MemoryUsage_ShouldNotLeak()
    {
        // Arrange
        var inventoryService = GetService<IInventoryService>();
        var initialMemory = GC.GetTotalMemory(true) / (1024.0 * 1024.0);
        var memoryMeasurements = new List<double>();
        
        // Act - Run operations for extended period and monitor memory
        var testDuration = TimeSpan.FromMinutes(5);
        var stopwatch = Stopwatch.StartNew();
        var operationCount = 0;
        
        while (stopwatch.Elapsed < testDuration)
        {
            // Perform inventory operations
            var testItem = new InventoryItem
            {
                PartId = $"MEMORY_TEST_{operationCount % 1000}",
                Operation = "100",
                Quantity = 1,
                Location = "MEMORY_STATION",
                User = "MemoryTestUser"
            };
            
            await inventoryService.AddInventoryAsync(testItem);
            operationCount++;
            
            // Measure memory every 100 operations
            if (operationCount % 100 == 0)
            {
                var currentMemory = GC.GetTotalMemory(false) / (1024.0 * 1024.0);
                memoryMeasurements.Add(currentMemory);
                
                // Force garbage collection every 1000 operations to check for leaks
                if (operationCount % 1000 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }
            
            // Small delay to prevent overwhelming the system
            if (operationCount % 100 == 0)
                await Task.Delay(10);
        }
        
        stopwatch.Stop();
        
        // Final memory check after cleanup
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(true) / (1024.0 * 1024.0);
        
        // Assert
        var memoryGrowth = finalMemory - initialMemory;
        var maxMemoryUsed = memoryMeasurements.Max();
        var averageMemoryUsed = memoryMeasurements.Average();
        
        Assert.That(memoryGrowth, Is.LessThan(100), // Should not grow by more than 100MB
            $"Memory should not grow significantly over time (indicates leak), grew by {memoryGrowth:F1}MB");
        
        Assert.That(maxMemoryUsed, Is.LessThan(initialMemory + 200), // Peak usage threshold
            $"Peak memory usage should be reasonable, peaked at {maxMemoryUsed:F1}MB (initial: {initialMemory:F1}MB)");
        
        // Performance reporting
        Console.WriteLine($"Memory Performance Results:");
        Console.WriteLine($"  Test Duration: {testDuration.TotalMinutes:F1} minutes");
        Console.WriteLine($"  Operations Performed: {operationCount:N0}");
        Console.WriteLine($"  Operations per Second: {operationCount / testDuration.TotalSeconds:F1}");
        Console.WriteLine($"  Initial Memory: {initialMemory:F1}MB");
        Console.WriteLine($"  Final Memory: {finalMemory:F1}MB");
        Console.WriteLine($"  Memory Growth: {memoryGrowth:F1}MB");
        Console.WriteLine($"  Peak Memory: {maxMemoryUsed:F1}MB");
        Console.WriteLine($"  Average Memory: {averageMemoryUsed:F1}MB");
    }
}
```

## ✅ Performance Test Checklist

When creating performance tests, ensure:

- [ ] Critical user workflows are performance tested
- [ ] Load thresholds are realistic for manufacturing environment
- [ ] Response time percentiles (95th, 99th) are measured
- [ ] Throughput requirements are validated
- [ ] Memory usage and leak detection are included
- [ ] Database connection pooling efficiency is tested
- [ ] UI rendering performance with large data sets is verified
- [ ] Concurrent user scenarios are tested
- [ ] Performance degradation under stress is measured
- [ ] Resource utilization (CPU, memory, connections) is monitored
- [ ] Performance regression detection is implemented
- [ ] Test results are clearly reported with metrics

## 🏷️ Performance Test Categories

Use these category attributes for performance tests:

```csharp
[Category("Performance")]       // All performance tests
[Category("Load")]             // Normal load testing
[Category("Stress")]           // Beyond normal capacity
[Category("Spike")]            // Sudden load increases
[Category("Volume")]           // Large data set processing
[Category("Endurance")]        // Extended duration testing
[Category("Memory")]           // Memory usage and leak testing
[Category("Database")]         // Database performance
[Category("UI")]               // User interface performance
[Category("Services")]         // Service layer performance
```

This template ensures comprehensive performance test coverage using NBomber framework while maintaining manufacturing-grade performance standards for the MTM WIP Application.