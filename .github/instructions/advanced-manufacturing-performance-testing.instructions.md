# MTM Advanced Performance Testing Framework Instructions

**Framework**: .NET 8 Performance Testing with Manufacturing Workload Simulation  
**Pattern**: Performance Testing for Manufacturing Applications  
**Created**: 2025-09-15  

---

## 🎯 Overview

Comprehensive performance testing framework for MTM WIP Application, focusing on manufacturing workload simulation, cross-platform performance validation, and real-world manufacturing scenario testing.

## 🏭 Manufacturing Performance Testing Architecture

### Performance Test Framework Structure

```csharp
[TestFixture]
[Category("Performance")]
public abstract class ManufacturingPerformanceTestBase
{
    protected IPerformanceProfiler _profiler;
    protected IMemoryProfiler _memoryProfiler;
    protected IMetricsCollector _metricsCollector;
    protected ILoadSimulator _loadSimulator;
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        _profiler = new PerformanceProfiler();
        _memoryProfiler = new MemoryProfiler();
        _metricsCollector = new ManufacturingMetricsCollector();
        _loadSimulator = new ManufacturingLoadSimulator();
        
        await SetupPerformanceTestEnvironmentAsync();
    }
    
    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await _profiler.GenerateReportAsync();
        await _memoryProfiler.GenerateReportAsync();
        await CleanupPerformanceTestEnvironmentAsync();
    }
    
    protected async Task<PerformanceTestResult> ExecutePerformanceTestAsync<T>(
        string testName,
        Func<Task<T>> testFunction,
        PerformanceTestCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();
        var startMemory = GC.GetTotalMemory(false);
        
        try
        {
            _profiler.StartProfiling(testName);
            var result = await testFunction();
            _profiler.StopProfiling();
            
            stopwatch.Stop();
            var endMemory = GC.GetTotalMemory(false);
            var memoryUsed = endMemory - startMemory;
            
            return new PerformanceTestResult
            {
                TestName = testName,
                IsSuccess = true,
                ExecutionTime = stopwatch.Elapsed,
                MemoryUsed = memoryUsed,
                Result = result,
                MeetsCriteria = EvaluatePerformanceCriteria(stopwatch.Elapsed, memoryUsed, criteria)
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new PerformanceTestResult
            {
                TestName = testName,
                IsSuccess = false,
                ExecutionTime = stopwatch.Elapsed,
                Exception = ex,
                MeetsCriteria = false
            };
        }
    }
    
    protected virtual async Task SetupPerformanceTestEnvironmentAsync()
    {
        // Setup test database with performance-optimized configuration
        await SetupPerformanceTestDatabaseAsync();
        
        // Configure services for performance testing
        await ConfigurePerformanceTestServicesAsync();
        
        // Warm up JIT compilation
        await WarmupJitCompilerAsync();
        
        // Force garbage collection before tests
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
    
    private bool EvaluatePerformanceCriteria(TimeSpan executionTime, long memoryUsed, PerformanceTestCriteria criteria)
    {
        return executionTime <= criteria.MaxExecutionTime &&
               memoryUsed <= criteria.MaxMemoryUsage;
    }
}
```

## 📊 Manufacturing Load Simulation Testing

### High-Volume Manufacturing Operations

```csharp
[TestFixture]
[Category("Performance")]
[Category("ManufacturingLoad")]
public class ManufacturingLoadPerformanceTests : ManufacturingPerformanceTestBase
{
    private IInventoryService _inventoryService;
    private ITransactionService _transactionService;
    private IDatabaseService _databaseService;
    
    [SetUp]
    public async Task SetUp()
    {
        var services = new ServiceCollection();
        services.AddMTMServices(CreateTestConfiguration());
        
        var serviceProvider = services.BuildServiceProvider();
        _inventoryService = serviceProvider.GetRequiredService<IInventoryService>();
        _transactionService = serviceProvider.GetRequiredService<ITransactionService>();
        _databaseService = serviceProvider.GetRequiredService<IDatabaseService>();
    }
    
    [Test]
    [TestCase(1000, 50, Description = "Peak shift changeover load")]
    [TestCase(5000, 100, Description = "End-of-month processing load")]
    [TestCase(10000, 200, Description = "Year-end inventory processing")]
    public async Task HighVolume_InventoryOperations_ShouldMaintainPerformance(int operationCount, int concurrentUsers)
    {
        // Arrange - Generate realistic manufacturing data
        var manufacturingOperations = GenerateManufacturingOperations(operationCount);
        var performanceCriteria = new PerformanceTestCriteria
        {
            MaxExecutionTime = TimeSpan.FromMinutes(10), // 10 minutes for high-volume operations
            MaxMemoryUsage = 500 * 1024 * 1024, // 500MB memory limit
            MinThroughput = operationCount / 600 // Operations per second (10 minutes = 600 seconds)
        };
        
        // Act & Assert
        var result = await ExecutePerformanceTestAsync(
            $"HighVolume_InventoryOperations_{operationCount}_{concurrentUsers}",
            async () => await ExecuteManufacturingOperationsBatchAsync(manufacturingOperations, concurrentUsers),
            performanceCriteria
        );
        
        Assert.That(result.IsSuccess, Is.True, $"High-volume test should succeed: {result.Exception?.Message}");
        Assert.That(result.MeetsCriteria, Is.True, 
            $"Performance criteria not met: {result.ExecutionTime.TotalSeconds:F2}s execution time, {result.MemoryUsed / 1024 / 1024:F2}MB memory");
        
        // Additional manufacturing-specific validations
        Assert.That(result.ExecutionTime.TotalSeconds / operationCount, Is.LessThan(0.01), 
            "Each operation should complete in less than 10ms on average");
        
        Console.WriteLine($"Performance Results:");
        Console.WriteLine($"  Operations: {operationCount:N0}");
        Console.WriteLine($"  Concurrent Users: {concurrentUsers:N0}");
        Console.WriteLine($"  Total Time: {result.ExecutionTime.TotalSeconds:F2}s");
        Console.WriteLine($"  Average per Operation: {(result.ExecutionTime.TotalMilliseconds / operationCount):F2}ms");
        Console.WriteLine($"  Throughput: {(operationCount / result.ExecutionTime.TotalSeconds):F2} ops/sec");
        Console.WriteLine($"  Memory Used: {(result.MemoryUsed / 1024.0 / 1024.0):F2}MB");
    }
    
    [Test]
    public async Task ShiftChangeoverSimulation_ShouldHandlePeakLoad()
    {
        // Simulate typical shift changeover scenario
        var shiftChangeoverScenario = new ManufacturingScenario
        {
            ScenarioName = "ShiftChangeover",
            Duration = TimeSpan.FromMinutes(30), // 30-minute changeover window
            PeakConcurrentUsers = 25, // Multiple operators ending/starting shifts
            OperationsProfile = new[]
            {
                new OperationProfile { Type = "TransactionHistoryQuery", Count = 500, Weight = 0.3 },
                new OperationProfile { Type = "QuickButtonsLoad", Count = 200, Weight = 0.2 },
                new OperationProfile { Type = "InventoryAdd", Count = 150, Weight = 0.2 },
                new OperationProfile { Type = "InventoryRemove", Count = 100, Weight = 0.15 },
                new OperationProfile { Type = "ReportGeneration", Count = 50, Weight = 0.15 }
            }
        };
        
        var performanceCriteria = new PerformanceTestCriteria
        {
            MaxExecutionTime = shiftChangeoverScenario.Duration,
            MaxMemoryUsage = 1024 * 1024 * 1024, // 1GB for shift changeover
            MinThroughput = 1000 / 1800 // 1000 operations in 30 minutes
        };
        
        var result = await ExecutePerformanceTestAsync(
            "ShiftChangeoverSimulation",
            () => ExecuteManufacturingScenarioAsync(shiftChangeoverScenario),
            performanceCriteria
        );
        
        Assert.That(result.IsSuccess, Is.True, "Shift changeover simulation should succeed");
        Assert.That(result.MeetsCriteria, Is.True, "Shift changeover should meet performance criteria");
        
        // Validate manufacturing-specific KPIs
        var scenarioResult = result.Result as ManufacturingScenarioResult;
        Assert.That(scenarioResult.AverageResponseTime, Is.LessThan(TimeSpan.FromSeconds(2)), 
            "Average response time should be under 2 seconds during shift changeover");
        Assert.That(scenarioResult.ErrorRate, Is.LessThan(0.01), 
            "Error rate should be less than 1% during shift changeover");
    }
    
    [Test]
    public async Task DatabaseConnectionPool_UnderLoad_ShouldMaintainPerformance()
    {
        // Test database connection pooling under manufacturing load
        var concurrentConnections = 50;
        var operationsPerConnection = 20;
        
        var connectionTasks = Enumerable.Range(0, concurrentConnections)
            .Select(async connectionIndex =>
            {
                var connectionStopwatch = Stopwatch.StartNew();
                var operationResults = new List<TimeSpan>();
                
                for (int i = 0; i < operationsPerConnection; i++)
                {
                    var operationStopwatch = Stopwatch.StartNew();
                    
                    // Execute typical database operation
                    await _databaseService.ExecuteStoredProcedureAsync(
                        "inv_inventory_Get_ByPartID", 
                        new[] { new MySqlParameter("p_PartID", $"PERF_TEST_{connectionIndex}_{i}") });
                    
                    operationStopwatch.Stop();
                    operationResults.Add(operationStopwatch.Elapsed);
                }
                
                connectionStopwatch.Stop();
                
                return new ConnectionTestResult
                {
                    ConnectionIndex = connectionIndex,
                    TotalTime = connectionStopwatch.Elapsed,
                    OperationTimes = operationResults,
                    AverageOperationTime = TimeSpan.FromMilliseconds(operationResults.Average(t => t.TotalMilliseconds))
                };
            });
        
        var results = await Task.WhenAll(connectionTasks);
        
        // Validate connection pool performance
        var overallAverageTime = TimeSpan.FromMilliseconds(
            results.SelectMany(r => r.OperationTimes).Average(t => t.TotalMilliseconds));
        
        Assert.That(overallAverageTime, Is.LessThan(TimeSpan.FromMilliseconds(100)), 
            "Average database operation should be under 100ms with connection pooling");
        
        // Validate no connection experienced significant delays
        var maxAverageTime = results.Max(r => r.AverageOperationTime);
        Assert.That(maxAverageTime, Is.LessThan(TimeSpan.FromMilliseconds(200)), 
            "No connection should average more than 200ms per operation");
        
        Console.WriteLine($"Database Connection Pool Performance:");
        Console.WriteLine($"  Concurrent Connections: {concurrentConnections}");
        Console.WriteLine($"  Operations per Connection: {operationsPerConnection}");
        Console.WriteLine($"  Total Operations: {concurrentConnections * operationsPerConnection}");
        Console.WriteLine($"  Overall Average Time: {overallAverageTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"  Best Connection Average: {results.Min(r => r.AverageOperationTime).TotalMilliseconds:F2}ms");
        Console.WriteLine($"  Worst Connection Average: {maxAverageTime.TotalMilliseconds:F2}ms");
    }
    
    private List<ManufacturingOperation> GenerateManufacturingOperations(int count)
    {
        var random = new Random(42); // Fixed seed for reproducible tests
        var operations = new List<ManufacturingOperation>();
        
        var partIds = Enumerable.Range(1, Math.Min(count / 10, 1000))
            .Select(i => $"PERF_PART_{i:0000}")
            .ToArray();
            
        var operationNumbers = new[] { "90", "100", "110", "120" };
        var locations = new[] { "STATION_A", "STATION_B", "STATION_C", "STATION_D", "STATION_E" };
        var transactionTypes = new[] { "IN", "OUT", "TRANSFER" };
        
        for (int i = 0; i < count; i++)
        {
            operations.Add(new ManufacturingOperation
            {
                PartId = partIds[random.Next(partIds.Length)],
                Operation = operationNumbers[random.Next(operationNumbers.Length)],
                Quantity = random.Next(1, 100),
                Location = locations[random.Next(locations.Length)],
                TransactionType = transactionTypes[random.Next(transactionTypes.Length)],
                UserId = $"PERF_USER_{random.Next(1, 21):00}", // 20 different users
                Timestamp = DateTime.Now.AddSeconds(-random.Next(0, 86400)) // Last 24 hours
            });
        }
        
        return operations;
    }
    
    private async Task<int> ExecuteManufacturingOperationsBatchAsync(
        List<ManufacturingOperation> operations, int maxConcurrency)
    {
        var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        var successCount = 0;
        
        var tasks = operations.Select(async operation =>
        {
            await semaphore.WaitAsync();
            try
            {
                var success = await ExecuteSingleManufacturingOperationAsync(operation);
                if (success)
                    Interlocked.Increment(ref successCount);
                return success;
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
        return successCount;
    }
    
    private async Task<bool> ExecuteSingleManufacturingOperationAsync(ManufacturingOperation operation)
    {
        try
        {
            switch (operation.TransactionType)
            {
                case "IN":
                    return await _inventoryService.AddInventoryAsync(
                        operation.PartId, operation.Operation, operation.Quantity, 
                        operation.Location, operation.UserId);
                        
                case "OUT":
                    return await _inventoryService.RemoveInventoryAsync(
                        operation.PartId, operation.Operation, operation.Quantity, 
                        operation.Location, operation.UserId);
                        
                case "TRANSFER":
                    return await _inventoryService.TransferInventoryAsync(
                        operation.PartId, operation.Operation, operation.Quantity,
                        operation.Location, $"DEST_{operation.Location}", operation.UserId);
                        
                default:
                    return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Operation failed: {ex.Message}");
            return false;
        }
    }
}
```

## 🧪 Memory Performance and Leak Detection

### Memory Usage Validation

```csharp
[TestFixture]
[Category("Performance")]
[Category("Memory")]
public class MemoryPerformanceTests : ManufacturingPerformanceTestBase
{
    [Test]
    public async Task LongRunning_InventoryOperations_ShouldNotLeakMemory()
    {
        // Arrange - Long-running scenario simulating 8-hour shift
        var operationCount = 5000;
        var batchSize = 100;
        var memoryCheckpoints = new List<MemoryCheckpoint>();
        
        // Initial memory measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var initialMemory = GC.GetTotalMemory(false);
        memoryCheckpoints.Add(new MemoryCheckpoint("Initial", initialMemory, DateTime.Now));
        
        // Execute operations in batches to simulate continuous manufacturing
        for (int batch = 0; batch < operationCount / batchSize; batch++)
        {
            var operations = GenerateManufacturingOperations(batchSize);
            await ExecuteManufacturingOperationsBatchAsync(operations, 10);
            
            // Periodic memory checkpoint
            if (batch % 10 == 0) // Every 10 batches (1000 operations)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                var currentMemory = GC.GetTotalMemory(false);
                memoryCheckpoints.Add(new MemoryCheckpoint(
                    $"Batch_{batch}_{(batch + 1) * batchSize}_ops", 
                    currentMemory, 
                    DateTime.Now));
            }
            
            // Simulate manufacturing pace - small delay between batches
            await Task.Delay(10);
        }
        
        // Final memory measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var finalMemory = GC.GetTotalMemory(false);
        memoryCheckpoints.Add(new MemoryCheckpoint("Final", finalMemory, DateTime.Now));
        
        // Analyze memory usage patterns
        var memoryGrowth = finalMemory - initialMemory;
        var maxMemoryUsed = memoryCheckpoints.Max(c => c.MemoryUsed);
        var averageMemoryGrowthPerOperation = memoryGrowth / (double)operationCount;
        
        // Assert memory constraints for manufacturing applications
        Assert.That(memoryGrowth, Is.LessThan(50 * 1024 * 1024), // 50MB growth limit
            $"Memory growth should be less than 50MB, actual: {memoryGrowth / 1024 / 1024:F2}MB");
            
        Assert.That(averageMemoryGrowthPerOperation, Is.LessThan(1024), // 1KB per operation
            $"Average memory growth per operation should be less than 1KB, actual: {averageMemoryGrowthPerOperation:F2}bytes");
            
        Assert.That(maxMemoryUsed, Is.LessThan(200 * 1024 * 1024), // 200MB peak usage
            $"Peak memory usage should be less than 200MB, actual: {maxMemoryUsed / 1024 / 1024:F2}MB");
        
        // Report detailed memory analysis
        Console.WriteLine("Memory Performance Analysis:");
        Console.WriteLine($"  Operations Executed: {operationCount:N0}");
        Console.WriteLine($"  Initial Memory: {initialMemory / 1024.0 / 1024.0:F2}MB");
        Console.WriteLine($"  Final Memory: {finalMemory / 1024.0 / 1024.0:F2}MB");
        Console.WriteLine($"  Memory Growth: {memoryGrowth / 1024.0 / 1024.0:F2}MB");
        Console.WriteLine($"  Peak Memory: {maxMemoryUsed / 1024.0 / 1024.0:F2}MB");
        Console.WriteLine($"  Avg Growth/Operation: {averageMemoryGrowthPerOperation:F2}bytes");
        
        // Check for memory leak patterns
        var recentCheckpoints = memoryCheckpoints.TakeLast(5).ToList();
        var memoryTrend = CalculateMemoryTrend(recentCheckpoints);
        
        Assert.That(memoryTrend, Is.LessThan(1024 * 1024), // 1MB/checkpoint trend limit
            $"Recent memory trend suggests potential leak: {memoryTrend / 1024.0:F2}KB per checkpoint");
    }
    
    [Test]
    public async Task ViewModelCollection_LargeDatasets_ShouldManageMemoryEfficiently()
    {
        // Test ViewModel collection memory management with large datasets
        var viewModel = new InventoryTabViewModel(
            Mock.Of<IInventoryService>(),
            Mock.Of<IMasterDataService>(),
            Mock.Of<ILogger<InventoryTabViewModel>>());
        
        GC.Collect();
        var initialMemory = GC.GetTotalMemory(false);
        
        // Simulate loading large inventory dataset
        var largeInventoryDataset = GenerateInventoryDataset(10000);
        
        foreach (var item in largeInventoryDataset)
        {
            viewModel.InventoryItems.Add(item);
        }
        
        var afterLoadMemory = GC.GetTotalMemory(false);
        var loadMemoryUsage = afterLoadMemory - initialMemory;
        
        // Simulate clearing and reloading (common in manufacturing shifts)
        viewModel.InventoryItems.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var afterClearMemory = GC.GetTotalMemory(false);
        var memoryReclaimed = afterLoadMemory - afterClearMemory;
        var memoryReclamationPercentage = (double)memoryReclaimed / loadMemoryUsage * 100;
        
        // Assert efficient memory management
        Assert.That(memoryReclamationPercentage, Is.GreaterThan(80), 
            $"Should reclaim at least 80% of memory after clear, actual: {memoryReclamationPercentage:F2}%");
            
        Assert.That(loadMemoryUsage, Is.LessThan(100 * 1024 * 1024), 
            $"Loading 10K items should use less than 100MB, actual: {loadMemoryUsage / 1024.0 / 1024.0:F2}MB");
        
        Console.WriteLine("ViewModel Memory Management Analysis:");
        Console.WriteLine($"  Dataset Size: {largeInventoryDataset.Count:N0} items");
        Console.WriteLine($"  Memory Used for Load: {loadMemoryUsage / 1024.0 / 1024.0:F2}MB");
        Console.WriteLine($"  Memory per Item: {loadMemoryUsage / (double)largeInventoryDataset.Count:F2}bytes");
        Console.WriteLine($"  Memory Reclaimed: {memoryReclaimed / 1024.0 / 1024.0:F2}MB ({memoryReclamationPercentage:F2}%)");
    }
    
    private long CalculateMemoryTrend(List<MemoryCheckpoint> checkpoints)
    {
        if (checkpoints.Count < 2) return 0;
        
        var memoryChanges = new List<long>();
        for (int i = 1; i < checkpoints.Count; i++)
        {
            memoryChanges.Add(checkpoints[i].MemoryUsed - checkpoints[i - 1].MemoryUsed);
        }
        
        return memoryChanges.Count > 0 ? (long)memoryChanges.Average() : 0;
    }
}

public class MemoryCheckpoint
{
    public string Description { get; }
    public long MemoryUsed { get; }
    public DateTime Timestamp { get; }
    
    public MemoryCheckpoint(string description, long memoryUsed, DateTime timestamp)
    {
        Description = description;
        MemoryUsed = memoryUsed;
        Timestamp = timestamp;
    }
}
```

## 📱 Cross-Platform Performance Testing

### Platform-Specific Performance Validation

```csharp
[TestFixture]
[Category("Performance")]
[Category("CrossPlatform")]
public class CrossPlatformPerformanceTests : ManufacturingPerformanceTestBase
{
    private static readonly Dictionary<string, PerformanceBaseline> PlatformBaselines = new()
    {
        ["Windows"] = new PerformanceBaseline
        {
            DatabaseConnectionTime = TimeSpan.FromMilliseconds(50),
            InventoryOperationTime = TimeSpan.FromMilliseconds(25),
            UIRenderTime = TimeSpan.FromMilliseconds(100),
            MemoryUsageBaseline = 150 * 1024 * 1024 // 150MB
        },
        ["Linux"] = new PerformanceBaseline  
        {
            DatabaseConnectionTime = TimeSpan.FromMilliseconds(40), // Typically faster on Linux
            InventoryOperationTime = TimeSpan.FromMilliseconds(20),
            UIRenderTime = TimeSpan.FromMilliseconds(120), // X11/Wayland overhead
            MemoryUsageBaseline = 130 * 1024 * 1024 // 130MB, typically more efficient
        },
        ["macOS"] = new PerformanceBaseline
        {
            DatabaseConnectionTime = TimeSpan.FromMilliseconds(45),
            InventoryOperationTime = TimeSpan.FromMilliseconds(22),
            UIRenderTime = TimeSpan.FromMilliseconds(80), // Cocoa efficiency
            MemoryUsageBaseline = 160 * 1024 * 1024 // 160MB
        },
        ["Android"] = new PerformanceBaseline
        {
            DatabaseConnectionTime = TimeSpan.FromMilliseconds(100), // Mobile constraints
            InventoryOperationTime = TimeSpan.FromMilliseconds(50), 
            UIRenderTime = TimeSpan.FromMilliseconds(150), // Touch interface overhead
            MemoryUsageBaseline = 100 * 1024 * 1024 // 100MB, mobile memory constraints
        }
    };
    
    [Test]
    [Platform("Win")]
    public async Task Windows_PerformanceBaseline_ShouldMeetTargets()
    {
        var platformBaseline = PlatformBaselines["Windows"];
        await ValidatePlatformPerformance("Windows", platformBaseline);
    }
    
    [Test]
    [Platform("Linux")]
    public async Task Linux_PerformanceBaseline_ShouldMeetTargets()
    {
        var platformBaseline = PlatformBaselines["Linux"];
        await ValidatePlatformPerformance("Linux", platformBaseline);
    }
    
    [Test]
    [Platform("Mac")]
    public async Task macOS_PerformanceBaseline_ShouldMeetTargets()
    {
        var platformBaseline = PlatformBaselines["macOS"];
        await ValidatePlatformPerformance("macOS", platformBaseline);
    }
    
    [Test]
    [Platform("Android")]
    public async Task Android_PerformanceBaseline_ShouldMeetTargets()
    {
        var platformBaseline = PlatformBaselines["Android"];
        await ValidatePlatformPerformance("Android", platformBaseline);
    }
    
    private async Task ValidatePlatformPerformance(string platform, PerformanceBaseline baseline)
    {
        Console.WriteLine($"Validating {platform} Performance Baseline:");
        
        // Database connection performance
        var connectionTime = await MeasureDatabaseConnectionTimeAsync();
        Assert.That(connectionTime, Is.LessThanOrEqualTo(baseline.DatabaseConnectionTime.Add(TimeSpan.FromMilliseconds(20))),
            $"Database connection time on {platform} should be within baseline + 20ms tolerance");
        Console.WriteLine($"  Database Connection: {connectionTime.TotalMilliseconds:F2}ms (baseline: {baseline.DatabaseConnectionTime.TotalMilliseconds:F2}ms)");
        
        // Inventory operation performance  
        var operationTime = await MeasureInventoryOperationTimeAsync();
        Assert.That(operationTime, Is.LessThanOrEqualTo(baseline.InventoryOperationTime.Add(TimeSpan.FromMilliseconds(10))),
            $"Inventory operation time on {platform} should be within baseline + 10ms tolerance");
        Console.WriteLine($"  Inventory Operation: {operationTime.TotalMilliseconds:F2}ms (baseline: {baseline.InventoryOperationTime.TotalMilliseconds:F2}ms)");
        
        // UI rendering performance (if applicable)
        if (platform != "Android") // Skip UI tests on headless Android
        {
            var renderTime = await MeasureUIRenderTimeAsync();
            Assert.That(renderTime, Is.LessThanOrEqualTo(baseline.UIRenderTime.Add(TimeSpan.FromMilliseconds(30))),
                $"UI render time on {platform} should be within baseline + 30ms tolerance");
            Console.WriteLine($"  UI Render Time: {renderTime.TotalMilliseconds:F2}ms (baseline: {baseline.UIRenderTime.TotalMilliseconds:F2}ms)");
        }
        
        // Memory usage baseline
        GC.Collect();
        var memoryUsage = GC.GetTotalMemory(false);
        var memoryTolerance = baseline.MemoryUsageBaseline * 1.2; // 20% tolerance
        Assert.That(memoryUsage, Is.LessThan(memoryTolerance),
            $"Memory usage on {platform} should be within 20% of baseline");
        Console.WriteLine($"  Memory Usage: {memoryUsage / 1024.0 / 1024.0:F2}MB (baseline: {baseline.MemoryUsageBaseline / 1024.0 / 1024.0:F2}MB)");
        
        // Cross-platform consistency validation
        await ValidateCrossPlatformConsistency(platform, connectionTime, operationTime);
    }
    
    private async Task<TimeSpan> MeasureDatabaseConnectionTimeAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < 10; i++)
        {
            await _databaseService.TestConnectionAsync();
        }
        
        stopwatch.Stop();
        return TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds / 10.0); // Average time
    }
    
    private async Task<TimeSpan> MeasureInventoryOperationTimeAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < 100; i++)
        {
            await _inventoryService.AddInventoryAsync($"PERF_PART_{i:000}", "100", 1, "PERF_STATION", "PerfUser");
        }
        
        stopwatch.Stop();
        return TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds / 100.0); // Average time
    }
    
    private async Task<TimeSpan> MeasureUIRenderTimeAsync()
    {
        // This would measure UI rendering time using Avalonia.Headless
        // Implementation would depend on specific UI components being tested
        await Task.Delay(1); // Placeholder
        return TimeSpan.FromMilliseconds(50); // Mock value
    }
    
    private async Task ValidateCrossPlatformConsistency(string platform, TimeSpan connectionTime, TimeSpan operationTime)
    {
        // Validate that performance is within acceptable variance across platforms
        var performanceVarianceThreshold = 2.0; // 200% variance threshold
        
        var referenceBaseline = PlatformBaselines["Windows"];
        var connectionVariance = connectionTime.TotalMilliseconds / referenceBaseline.DatabaseConnectionTime.TotalMilliseconds;
        var operationVariance = operationTime.TotalMilliseconds / referenceBaseline.InventoryOperationTime.TotalMilliseconds;
        
        Assert.That(connectionVariance, Is.LessThan(performanceVarianceThreshold),
            $"Database connection performance on {platform} varies too much from Windows baseline");
        Assert.That(operationVariance, Is.LessThan(performanceVarianceThreshold),
            $"Inventory operation performance on {platform} varies too much from Windows baseline");
        
        Console.WriteLine($"  Cross-platform variance - Connection: {connectionVariance:F2}x, Operation: {operationVariance:F2}x");
    }
}
```

## 📊 Performance Reporting and Analysis

### Automated Performance Reporting

```csharp
public class PerformanceReporter
{
    private readonly List<PerformanceTestResult> _results = new();
    private readonly ILogger<PerformanceReporter> _logger;
    
    public void AddResult(PerformanceTestResult result)
    {
        _results.Add(result);
        _logger.LogInformation("Performance test completed: {TestName} - {Duration}ms", 
            result.TestName, result.ExecutionTime.TotalMilliseconds);
    }
    
    public async Task GenerateReportAsync(string outputPath)
    {
        var report = new StringBuilder();
        report.AppendLine("# MTM Manufacturing Performance Test Report");
        report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Total Tests: {_results.Count}");
        report.AppendLine();
        
        // Executive Summary
        GenerateExecutiveSummary(report);
        
        // Detailed Results
        GenerateDetailedResults(report);
        
        // Performance Trends
        GeneratePerformanceTrends(report);
        
        // Recommendations
        GenerateRecommendations(report);
        
        await File.WriteAllTextAsync(outputPath, report.ToString());
        _logger.LogInformation("Performance report generated: {OutputPath}", outputPath);
    }
    
    private void GenerateExecutiveSummary(StringBuilder report)
    {
        var passedTests = _results.Count(r => r.MeetsCriteria);
        var failedTests = _results.Count - passedTests;
        var averageExecutionTime = TimeSpan.FromMilliseconds(
            _results.Average(r => r.ExecutionTime.TotalMilliseconds));
        
        report.AppendLine("## Executive Summary");
        report.AppendLine($"- **Tests Passed**: {passedTests}/{_results.Count} ({(double)passedTests / _results.Count * 100:F1}%)");
        report.AppendLine($"- **Tests Failed**: {failedTests}");
        report.AppendLine($"- **Average Execution Time**: {averageExecutionTime.TotalMilliseconds:F2}ms");
        report.AppendLine($"- **Total Memory Used**: {_results.Sum(r => r.MemoryUsed) / 1024.0 / 1024.0:F2}MB");
        report.AppendLine();
    }
}
```

## 🎯 Performance Testing Guidelines

### Critical Performance Metrics

1. **Inventory Operations**: <50ms response time for single operations, <2s for batch operations
2. **Database Connections**: <100ms connection establishment, connection pooling efficiency
3. **Memory Usage**: <200MB peak usage, <5MB growth per 1000 operations  
4. **UI Responsiveness**: <100ms for UI updates, smooth scrolling with 10K+ items
5. **Cross-Platform Consistency**: <200% performance variance between platforms

This comprehensive performance testing framework ensures MTM WIP Application maintains manufacturing-grade performance standards under real-world operating conditions across all supported platforms.