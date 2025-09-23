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
    
    protected async Task<PerformanceResult> MeasurePerformanceAsync<T>(
        Func<Task<T>> operation, 
        string operationName,
        PerformanceCriteria criteria = null)
    {
        criteria ??= GetDefaultCriteria(operationName);
        
        var stopwatch = Stopwatch.StartNew();
        var initialMemory = GC.GetTotalMemory(false);
        
        T result;
        Exception exception = null;
        
        try
        {
            result = await operation();
        }
        catch (Exception ex)
        {
            exception = ex;
            result = default(T);
        }
        finally
        {
            stopwatch.Stop();
        }
        
        var finalMemory = GC.GetTotalMemory(true);
        var memoryUsed = Math.Max(0, finalMemory - initialMemory);
        
        var performanceResult = new PerformanceResult
        {
            OperationName = operationName,
            ElapsedTime = stopwatch.Elapsed,
            MemoryUsed = memoryUsed,
            IsSuccessful = exception == null,
            Exception = exception,
            Result = result,
            Criteria = criteria
        };
        
        await _metricsCollector.RecordPerformanceAsync(performanceResult);
        
        ValidatePerformanceCriteria(performanceResult);
        
        return performanceResult;
    }
    
    protected void ValidatePerformanceCriteria(PerformanceResult result)
    {
        if (result.ElapsedTime > result.Criteria.MaxExecutionTime)
        {
            Assert.Fail($"Operation '{result.OperationName}' exceeded maximum execution time. " +
                       $"Expected: <{result.Criteria.MaxExecutionTime.TotalMilliseconds}ms, " +
                       $"Actual: {result.ElapsedTime.TotalMilliseconds}ms");
        }
        
        if (result.MemoryUsed > result.Criteria.MaxMemoryUsage)
        {
            Assert.Fail($"Operation '{result.OperationName}' exceeded maximum memory usage. " +
                       $"Expected: <{result.Criteria.MaxMemoryUsage / 1024 / 1024}MB, " +
                       $"Actual: {result.MemoryUsed / 1024 / 1024}MB");
        }
        
        if (!result.IsSuccessful && result.Criteria.RequireSuccess)
        {
            Assert.Fail($"Operation '{result.OperationName}' failed: {result.Exception?.Message}");
        }
    }
    
    protected PerformanceCriteria GetDefaultCriteria(string operationName)
    {
        return operationName.ToLowerInvariant() switch
        {
            var name when name.Contains("inventory") => new PerformanceCriteria
            {
                MaxExecutionTime = TimeSpan.FromMilliseconds(500),
                MaxMemoryUsage = 10 * 1024 * 1024, // 10MB
                RequireSuccess = true
            },
            var name when name.Contains("transaction") => new PerformanceCriteria
            {
                MaxExecutionTime = TimeSpan.FromMilliseconds(1000),
                MaxMemoryUsage = 5 * 1024 * 1024, // 5MB
                RequireSuccess = true
            },
            var name when name.Contains("database") => new PerformanceCriteria
            {
                MaxExecutionTime = TimeSpan.FromSeconds(2),
                MaxMemoryUsage = 20 * 1024 * 1024, // 20MB
                RequireSuccess = true
            },
            _ => new PerformanceCriteria
            {
                MaxExecutionTime = TimeSpan.FromSeconds(5),
                MaxMemoryUsage = 50 * 1024 * 1024, // 50MB
                RequireSuccess = false
            }
        };
    }
    
    protected virtual async Task SetupPerformanceTestEnvironmentAsync()
    {
        // Warm up JIT compilation
        await WarmupJITAsync();
        
        // Clear garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
    
    private async Task WarmupJITAsync()
    {
        // JIT warmup for key operations
        var warmupTasks = new List<Task>
        {
            Task.Run(() => { var _ = new List<string>(); }),
            Task.Run(() => { var _ = new Dictionary<string, object>(); }),
            Task.Run(() => { var _ = JsonSerializer.Serialize(new { test = "warmup" }); })
        };
        
        await Task.WhenAll(warmupTasks);
    }
    
    protected virtual async Task CleanupPerformanceTestEnvironmentAsync()
    {
        // Final garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        await Task.CompletedTask;
    }
}

public class PerformanceResult
{
    public string OperationName { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public long MemoryUsed { get; set; }
    public bool IsSuccessful { get; set; }
    public Exception Exception { get; set; }
    public object Result { get; set; }
    public PerformanceCriteria Criteria { get; set; }
}

public class PerformanceCriteria
{
    public TimeSpan MaxExecutionTime { get; set; }
    public long MaxMemoryUsage { get; set; }
    public bool RequireSuccess { get; set; } = true;
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
    public async Task HighVolume_InventoryOperations_ShouldMaintainPerformance()
    {
        // Arrange - Simulate shift changeover with 1000+ transactions
        var operationCount = 1000;
        var partNumbers = GenerateTestPartNumbers(100);
        var operations = new[] { "90", "100", "110", "120" };
        var locations = new[] { "STATION_A", "STATION_B", "STATION_C", "RECEIVING", "SHIPPING" };
        
        var inventoryOperations = Enumerable.Range(1, operationCount)
            .Select(i => new InventoryItem
            {
                PartId = partNumbers[i % partNumbers.Count],
                Operation = operations[i % operations.Length],
                Quantity = Random.Shared.Next(1, 100),
                Location = locations[i % locations.Length],
                TransactionType = (i % 3) switch
                {
                    0 => "IN",
                    1 => "OUT", 
                    _ => "TRANSFER"
                },
                UserId = $"ShiftUser_{(i % 10) + 1}"
            })
            .ToList();
        
        // Act - Execute high-volume operations with performance measurement
        var result = await MeasurePerformanceAsync(async () =>
        {
            var concurrency = 20; // Simulate 20 concurrent users
            var semaphore = new SemaphoreSlim(concurrency, concurrency);
            var tasks = new List<Task<bool>>();
            
            foreach (var operation in inventoryOperations)
            {
                tasks.Add(ProcessInventoryOperationAsync(operation, semaphore));
            }
            
            var results = await Task.WhenAll(tasks);
            return results.Count(r => r); // Count successful operations
        }, "HighVolumeInventoryOperations", new PerformanceCriteria
        {
            MaxExecutionTime = TimeSpan.FromMinutes(2), // 2 minutes for 1000 operations
            MaxMemoryUsage = 100 * 1024 * 1024, // 100MB
            RequireSuccess = true
        });
        
        // Assert - Performance and success rate validation
        var successRate = (int)result.Result / (double)operationCount;
        Assert.That(successRate, Is.GreaterThan(0.95), 
            $"Success rate should be >95%, actual: {successRate:P2}");
        
        var operationsPerSecond = operationCount / result.ElapsedTime.TotalSeconds;
        Assert.That(operationsPerSecond, Is.GreaterThan(10), 
            $"Should process >10 operations/second, actual: {operationsPerSecond:F1}");
        
        Console.WriteLine($"High Volume Performance Results:");
        Console.WriteLine($"  Operations: {operationCount}");
        Console.WriteLine($"  Total Time: {result.ElapsedTime.TotalSeconds:F1}s");
        Console.WriteLine($"  Operations/Second: {operationsPerSecond:F1}");
        Console.WriteLine($"  Success Rate: {successRate:P2}");
        Console.WriteLine($"  Memory Used: {result.MemoryUsed / 1024 / 1024:F1}MB");
    }
    
    [Test]
    public async Task DatabaseConnection_HighConcurrency_ShouldHandleLoad()
    {
        // Arrange - Test database connection pool under high load
        var concurrentConnections = 50;
        var operationsPerConnection = 20;
        
        // Act - Simulate high concurrent database operations
        var result = await MeasurePerformanceAsync(async () =>
        {
            var tasks = Enumerable.Range(1, concurrentConnections)
                .Select(async connectionId =>
                {
                    var connectionTasks = Enumerable.Range(1, operationsPerConnection)
                        .Select(async operationId =>
                        {
                            var parameters = new MySqlParameter[]
                            {
                                new("p_ConnectionId", connectionId),
                                new("p_OperationId", operationId),
                                new("p_TestData", $"ConcurrencyTest_{connectionId}_{operationId}")
                            };
                            
                            var dbResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                                _databaseService.ConnectionString, 
                                "test_concurrent_operations", 
                                parameters);
                            
                            return dbResult.Status == 1;
                        });
                    
                    var connectionResults = await Task.WhenAll(connectionTasks);
                    return connectionResults.Count(r => r);
                });
            
            var results = await Task.WhenAll(tasks);
            return results.Sum(); // Total successful operations
        }, "HighConcurrencyDatabaseOperations", new PerformanceCriteria
        {
            MaxExecutionTime = TimeSpan.FromSeconds(30),
            MaxMemoryUsage = 200 * 1024 * 1024, // 200MB
            RequireSuccess = true
        });
        
        // Assert - Connection pool handling validation
        var totalOperations = concurrentConnections * operationsPerConnection;
        var successfulOperations = (int)result.Result;
        var successRate = successfulOperations / (double)totalOperations;
        
        Assert.That(successRate, Is.GreaterThan(0.98), 
            $"Database connection pool should handle >98% success rate, actual: {successRate:P2}");
        
        Console.WriteLine($"Database Concurrency Results:");
        Console.WriteLine($"  Concurrent Connections: {concurrentConnections}");
        Console.WriteLine($"  Operations per Connection: {operationsPerConnection}");
        Console.WriteLine($"  Total Operations: {totalOperations}");
        Console.WriteLine($"  Successful: {successfulOperations}");
        Console.WriteLine($"  Success Rate: {successRate:P2}");
        Console.WriteLine($"  Total Time: {result.ElapsedTime.TotalSeconds:F1}s");
    }
    
    [Test]
    public async Task ShiftChangeover_Simulation_ShouldMaintainResponsiveness()
    {
        // Arrange - Simulate manufacturing shift changeover scenario
        var shiftUsers = 25; // 25 users changing shift simultaneously
        var operationsPerUser = 20;
        
        // Simulate shift changeover operations
        var shiftChangeoverOperations = new[]
        {
            "Complete pending work orders",
            "Record production metrics",
            "Transfer WIP to next shift", 
            "Update equipment status",
            "Generate shift report"
        };
        
        // Act - Execute shift changeover simulation
        var result = await MeasurePerformanceAsync(async () =>
        {
            var userTasks = Enumerable.Range(1, shiftUsers)
                .Select(async userId =>
                {
                    var userOperationTasks = Enumerable.Range(1, operationsPerUser)
                        .Select(async operationIndex =>
                        {
                            var operationType = shiftChangeoverOperations[operationIndex % shiftChangeoverOperations.Length];
                            return await SimulateShiftOperationAsync(userId, operationType, operationIndex);
                        });
                    
                    var userResults = await Task.WhenAll(userOperationTasks);
                    return userResults.Count(r => r);
                });
            
            var results = await Task.WhenAll(userTasks);
            return results.Sum();
        }, "ShiftChangeoverSimulation", new PerformanceCriteria
        {
            MaxExecutionTime = TimeSpan.FromMinutes(1), // Shift changeover should complete within 1 minute
            MaxMemoryUsage = 150 * 1024 * 1024, // 150MB
            RequireSuccess = true
        });
        
        // Assert - Shift changeover performance validation
        var totalOperations = shiftUsers * operationsPerUser;
        var successfulOperations = (int)result.Result;
        var operationsPerSecond = successfulOperations / result.ElapsedTime.TotalSeconds;
        
        Assert.That(successfulOperations, Is.GreaterThanOrEqualTo(totalOperations * 0.95),
            "At least 95% of shift changeover operations should succeed");
        Assert.That(operationsPerSecond, Is.GreaterThan(15),
            $"Should maintain >15 operations/second during shift changeover, actual: {operationsPerSecond:F1}");
        
        Console.WriteLine($"Shift Changeover Performance:");
        Console.WriteLine($"  Concurrent Users: {shiftUsers}");
        Console.WriteLine($"  Operations/User: {operationsPerUser}");
        Console.WriteLine($"  Success Rate: {successfulOperations}/{totalOperations} ({successfulOperations * 100.0 / totalOperations:F1}%)");
        Console.WriteLine($"  Operations/Second: {operationsPerSecond:F1}");
        Console.WriteLine($"  Total Time: {result.ElapsedTime.TotalSeconds:F1}s");
    }
    
    private async Task<bool> ProcessInventoryOperationAsync(InventoryItem operation, SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            var result = operation.TransactionType switch
            {
                "IN" => await _inventoryService.AddInventoryAsync(operation),
                "OUT" => await _inventoryService.RemoveInventoryAsync(operation),
                "TRANSFER" => await _inventoryService.TransferInventoryAsync(new TransferRequest
                {
                    PartId = operation.PartId,
                    FromOperation = operation.Operation,
                    ToOperation = GetNextOperation(operation.Operation),
                    Quantity = operation.Quantity,
                    Location = operation.Location,
                    UserId = operation.UserId
                }),
                _ => throw new ArgumentException($"Unknown transaction type: {operation.TransactionType}")
            };
            
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Operation failed: {ex.Message}");
            return false;
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    private async Task<bool> SimulateShiftOperationAsync(int userId, string operationType, int operationIndex)
    {
        try
        {
            // Simulate different shift operations with appropriate delays
            var operationDelay = operationType switch
            {
                "Complete pending work orders" => TimeSpan.FromMilliseconds(100),
                "Record production metrics" => TimeSpan.FromMilliseconds(50),
                "Transfer WIP to next shift" => TimeSpan.FromMilliseconds(200),
                "Update equipment status" => TimeSpan.FromMilliseconds(75),
                "Generate shift report" => TimeSpan.FromMilliseconds(300),
                _ => TimeSpan.FromMilliseconds(100)
            };
            
            await Task.Delay(operationDelay);
            
            // Simulate actual database operation
            var parameters = new MySqlParameter[]
            {
                new("p_UserId", userId),
                new("p_OperationType", operationType),
                new("p_OperationIndex", operationIndex),
                new("p_ShiftTime", DateTime.Now)
            };
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.ConnectionString,
                "shift_operation_record",
                parameters);
            
            return result.Status == 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Shift operation failed for user {userId}, operation {operationType}: {ex.Message}");
            return false;
        }
    }
    
    private string GetNextOperation(string currentOperation)
    {
        return currentOperation switch
        {
            "90" => "100",
            "100" => "110", 
            "110" => "120",
            "120" => "130",
            _ => "100"
        };
    }
    
    private List<string> GenerateTestPartNumbers(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => $"PERF_PART_{i:000}")
            .ToList();
    }
    
    private IConfiguration CreateTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=mtm_performance_test;Uid=test_user;Pwd=test_password;",
                ["MTMSettings:DatabaseTimeout"] = "60",
                ["MTMSettings:MaxRetryAttempts"] = "5",
                ["MTMSettings:DefaultOperation"] = "100"
            })
            .Build();
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
        var memoryCheckpoints = new List<long>();
        
        // Act - Execute operations in batches with memory monitoring
        var result = await MeasurePerformanceAsync(async () =>
        {
            var services = CreateServicesForMemoryTest();
            var inventoryService = services.GetRequiredService<IInventoryService>();
            
            for (int batch = 0; batch < operationCount / batchSize; batch++)
            {
                // Execute batch operations
                var batchOperations = CreateInventoryOperationBatch(batchSize, batch);
                
                var batchTasks = batchOperations.Select(async operation =>
                {
                    var result = await inventoryService.AddInventoryAsync(operation);
                    return result.IsSuccess;
                });
                
                await Task.WhenAll(batchTasks);
                
                // Force garbage collection and measure memory
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                var memoryUsage = GC.GetTotalMemory(false);
                memoryCheckpoints.Add(memoryUsage);
                
                Console.WriteLine($"Batch {batch + 1}/{operationCount / batchSize}: Memory = {memoryUsage / 1024 / 1024:F1}MB");
                
                // Small delay to simulate real-world timing
                await Task.Delay(10);
            }
            
            return memoryCheckpoints.Count;
        }, "LongRunningMemoryTest", new PerformanceCriteria
        {
            MaxExecutionTime = TimeSpan.FromMinutes(5),
            MaxMemoryUsage = 500 * 1024 * 1024, // 500MB max
            RequireSuccess = true
        });
        
        // Assert - Memory growth validation
        var initialMemory = memoryCheckpoints.First();
        var finalMemory = memoryCheckpoints.Last();
        var maxMemory = memoryCheckpoints.Max();
        
        var memoryGrowthPercent = ((finalMemory - initialMemory) / (double)initialMemory) * 100;
        
        Assert.That(memoryGrowthPercent, Is.LessThan(50),
            $"Memory growth should be <50% over long-running operations, actual: {memoryGrowthPercent:F1}%");
        
        Assert.That(maxMemory, Is.LessThan(300 * 1024 * 1024),
            $"Peak memory usage should be <300MB, actual: {maxMemory / 1024 / 1024:F1}MB");
        
        // Check for memory growth trend
        var recentMemory = memoryCheckpoints.TakeLast(10).Average();
        var earlyMemory = memoryCheckpoints.Take(10).Average();
        var trendGrowth = ((recentMemory - earlyMemory) / earlyMemory) * 100;
        
        Assert.That(trendGrowth, Is.LessThan(20),
            $"Memory growth trend should be <20%, actual: {trendGrowth:F1}%");
        
        Console.WriteLine($"Memory Performance Summary:");
        Console.WriteLine($"  Initial Memory: {initialMemory / 1024 / 1024:F1}MB");
        Console.WriteLine($"  Final Memory: {finalMemory / 1024 / 1024:F1}MB");
        Console.WriteLine($"  Peak Memory: {maxMemory / 1024 / 1024:F1}MB");
        Console.WriteLine($"  Memory Growth: {memoryGrowthPercent:F1}%");
        Console.WriteLine($"  Trend Growth: {trendGrowth:F1}%");
    }
    
    [Test]
    public async Task UI_DataBinding_ShouldNotLeakMemoryWithLargeDatasets()
    {
        // Arrange - Simulate UI with large manufacturing datasets
        var datasetSizes = new[] { 1000, 5000, 10000, 25000 };
        var memoryMeasurements = new List<(int Size, long Memory)>();
        
        // Act - Test memory usage with increasing dataset sizes
        foreach (var size in datasetSizes)
        {
            var memoryBefore = GC.GetTotalMemory(true);
            
            var result = await MeasurePerformanceAsync(async () =>
            {
                // Create large dataset similar to manufacturing inventory
                var inventoryData = CreateLargeInventoryDataset(size);
                
                // Simulate data binding operations
                var observableCollection = new ObservableCollection<InventoryItem>(inventoryData);
                
                // Simulate UI operations on the data
                for (int i = 0; i < 100; i++)
                {
                    var randomIndex = Random.Shared.Next(0, observableCollection.Count);
                    var item = observableCollection[randomIndex];
                    
                    // Simulate property changes that would trigger UI updates
                    item.Quantity += 1;
                    item.LastUpdated = DateTime.Now;
                }
                
                return observableCollection.Count;
            }, $"UIDataBinding_{size}_items");
            
            var memoryAfter = GC.GetTotalMemory(true);
            var memoryUsed = memoryAfter - memoryBefore;
            
            memoryMeasurements.Add((size, memoryUsed));
            
            Console.WriteLine($"Dataset Size: {size:N0} items, Memory Used: {memoryUsed / 1024 / 1024:F1}MB");
        }
        
        // Assert - Memory usage should scale reasonably with dataset size
        for (int i = 1; i < memoryMeasurements.Count; i++)
        {
            var current = memoryMeasurements[i];
            var previous = memoryMeasurements[i - 1];
            
            var sizeRatio = (double)current.Size / previous.Size;
            var memoryRatio = (double)current.Memory / previous.Memory;
            
            // Memory usage should not grow exponentially
            Assert.That(memoryRatio, Is.LessThan(sizeRatio * 2),
                $"Memory growth should be roughly linear with dataset size. " +
                $"Size ratio: {sizeRatio:F1}x, Memory ratio: {memoryRatio:F1}x");
        }
    }
    
    private List<InventoryItem> CreateInventoryOperationBatch(int batchSize, int batchNumber)
    {
        return Enumerable.Range(1, batchSize)
            .Select(i => new InventoryItem
            {
                PartId = $"MEMORY_TEST_PART_{batchNumber * batchSize + i:0000}",
                Operation = "100",
                Quantity = Random.Shared.Next(1, 100),
                Location = $"STATION_{(i % 5) + 1}",
                TransactionType = "IN",
                UserId = "MemoryTest"
            })
            .ToList();
    }
    
    private List<InventoryItem> CreateLargeInventoryDataset(int size)
    {
        var parts = new[] { "MOTOR_ASSEMBLY", "BEARING_UNIT", "DRIVE_SHAFT", "HOUSING_MAIN", "CONTROL_UNIT" };
        var operations = new[] { "90", "100", "110", "120" };
        var locations = new[] { "STATION_A", "STATION_B", "STATION_C", "STATION_D", "STATION_E" };
        
        return Enumerable.Range(1, size)
            .Select(i => new InventoryItem
            {
                PartId = $"{parts[i % parts.Length]}_{i:00000}",
                Operation = operations[i % operations.Length],
                Quantity = Random.Shared.Next(1, 1000),
                Location = locations[i % locations.Length],
                TransactionType = "IN",
                UserId = $"User_{(i % 20) + 1}",
                LastUpdated = DateTime.Now.AddHours(-Random.Shared.Next(0, 48))
            })
            .ToList();
    }
    
    private IServiceProvider CreateServicesForMemoryTest()
    {
        var services = new ServiceCollection();
        services.AddMTMServices(CreateTestConfiguration());
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        
        return services.BuildServiceProvider();
    }
    
    private IConfiguration CreateTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=mtm_memory_test;Uid=test_user;Pwd=test_password;",
                ["MTMSettings:DefaultOperation"] = "100",
                ["MTMSettings:MaxCacheSize"] = "1000"
            })
            .Build();
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
    private static readonly Dictionary<OSPlatform, PerformanceBaseline> _platformBaselines = new()
    {
        [OSPlatform.Windows] = new PerformanceBaseline
        {
            InventoryOperationTime = TimeSpan.FromMilliseconds(200),
            DatabaseQueryTime = TimeSpan.FromMilliseconds(500),
            UIRenderTime = TimeSpan.FromMilliseconds(100),
            MemoryEfficiency = 0.85 // 85% efficiency
        },
        [OSPlatform.macOS] = new PerformanceBaseline
        {
            InventoryOperationTime = TimeSpan.FromMilliseconds(250),
            DatabaseQueryTime = TimeSpan.FromMilliseconds(600),
            UIRenderTime = TimeSpan.FromMilliseconds(120),
            MemoryEfficiency = 0.80
        },
        [OSPlatform.Linux] = new PerformanceBaseline
        {
            InventoryOperationTime = TimeSpan.FromMilliseconds(300),
            DatabaseQueryTime = TimeSpan.FromMilliseconds(700),
            UIRenderTime = TimeSpan.FromMilliseconds(150),
            MemoryEfficiency = 0.75
        },
        [OSPlatform.Create("ANDROID")] = new PerformanceBaseline
        {
            InventoryOperationTime = TimeSpan.FromMilliseconds(500),
            DatabaseQueryTime = TimeSpan.FromMilliseconds(1000),
            UIRenderTime = TimeSpan.FromMilliseconds(200),
            MemoryEfficiency = 0.70
        }
    };
    
    [Test]
    public async Task InventoryOperations_CrossPlatform_ShouldMeetPlatformBaselines()
    {
        // Arrange - Get platform-specific baseline
        var currentPlatform = GetCurrentPlatform();
        var baseline = _platformBaselines[currentPlatform];
        
        var testOperations = CreateStandardInventoryOperations(100);
        
        // Act - Execute operations and measure performance
        var results = new List<PerformanceResult>();
        
        foreach (var operation in testOperations)
        {
            var result = await MeasurePerformanceAsync(async () =>
            {
                var inventoryService = CreateInventoryService();
                return await inventoryService.AddInventoryAsync(operation);
            }, $"InventoryOperation_{operation.PartId}", new PerformanceCriteria
            {
                MaxExecutionTime = baseline.InventoryOperationTime,
                MaxMemoryUsage = 10 * 1024 * 1024,
                RequireSuccess = true
            });
            
            results.Add(result);
        }
        
        // Assert - Platform-specific performance validation
        var averageTime = TimeSpan.FromMilliseconds(results.Average(r => r.ElapsedTime.TotalMilliseconds));
        var successRate = results.Count(r => r.IsSuccessful) / (double)results.Count;
        var averageMemory = results.Average(r => r.MemoryUsed);
        
        Assert.That(averageTime, Is.LessThan(baseline.InventoryOperationTime),
            $"Average operation time should meet {currentPlatform} baseline. " +
            $"Expected: <{baseline.InventoryOperationTime.TotalMilliseconds}ms, " +
            $"Actual: {averageTime.TotalMilliseconds}ms");
        
        Assert.That(successRate, Is.GreaterThan(0.95),
            $"Success rate should be >95% on {currentPlatform}, actual: {successRate:P2}");
        
        Console.WriteLine($"Cross-Platform Performance Results for {currentPlatform}:");
        Console.WriteLine($"  Operations: {testOperations.Count}");
        Console.WriteLine($"  Average Time: {averageTime.TotalMilliseconds:F0}ms (Baseline: {baseline.InventoryOperationTime.TotalMilliseconds:F0}ms)");
        Console.WriteLine($"  Success Rate: {successRate:P2}");
        Console.WriteLine($"  Average Memory: {averageMemory / 1024 / 1024:F1}MB");
        Console.WriteLine($"  Platform Efficiency: {(baseline.InventoryOperationTime.TotalMilliseconds / averageTime.TotalMilliseconds):P1}");
    }
    
    [Test]
    public async Task DatabaseOperations_CrossPlatform_ShouldScaleConsistently()
    {
        // Arrange - Test database operations across different scales
        var operationScales = new[] { 10, 50, 100, 250, 500 };
        var currentPlatform = GetCurrentPlatform();
        var baseline = _platformBaselines[currentPlatform];
        
        var scaleResults = new List<(int Scale, TimeSpan Time, bool MeetsBaseline)>();
        
        foreach (var scale in operationScales)
        {
            var result = await MeasurePerformanceAsync(async () =>
            {
                var databaseService = CreateDatabaseService();
                var tasks = Enumerable.Range(1, scale)
                    .Select(async i =>
                    {
                        var parameters = new MySqlParameter[]
                        {
                            new("p_PartID", $"SCALE_TEST_{i:000}"),
                            new("p_Operation", "100"),
                            new("p_Quantity", 10),
                            new("p_Location", "PERF_STATION"),
                            new("p_User", "ScaleTest")
                        };
                        
                        var dbResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                            databaseService.ConnectionString,
                            "inv_inventory_Add_Item",
                            parameters);
                        
                        return dbResult.Status == 1;
                    });
                
                var results = await Task.WhenAll(tasks);
                return results.Count(r => r);
            }, $"DatabaseScale_{scale}", new PerformanceCriteria
            {
                MaxExecutionTime = TimeSpan.FromMilliseconds(baseline.DatabaseQueryTime.TotalMilliseconds * Math.Log(scale)),
                MaxMemoryUsage = 50 * 1024 * 1024,
                RequireSuccess = false
            });
            
            var averageTimePerOperation = result.ElapsedTime.TotalMilliseconds / scale;
            var meetsBaseline = averageTimePerOperation <= baseline.DatabaseQueryTime.TotalMilliseconds;
            
            scaleResults.Add((scale, result.ElapsedTime, meetsBaseline));
            
            Console.WriteLine($"Scale {scale}: {result.ElapsedTime.TotalMilliseconds:F0}ms total, " +
                           $"{averageTimePerOperation:F1}ms/op, Baseline: {(meetsBaseline ? "PASS" : "FAIL")}");
        }
        
        // Assert - Scaling characteristics validation
        var baselineFailures = scaleResults.Count(r => !r.MeetsBaseline);
        Assert.That(baselineFailures, Is.LessThanOrEqualTo(1),
            $"At most 1 scale should fail baseline on {currentPlatform}, actual failures: {baselineFailures}");
        
        // Check that scaling is not exponential
        for (int i = 1; i < scaleResults.Count; i++)
        {
            var current = scaleResults[i];
            var previous = scaleResults[i - 1];
            
            var scaleRatio = (double)current.Scale / previous.Scale;
            var timeRatio = current.Time.TotalMilliseconds / previous.Time.TotalMilliseconds;
            
            Assert.That(timeRatio, Is.LessThan(scaleRatio * 2),
                $"Time scaling should be roughly linear. Scale {previous.Scale}->{current.Scale}: " +
                $"Scale ratio {scaleRatio:F1}x, Time ratio {timeRatio:F1}x");
        }
    }
    
    private OSPlatform GetCurrentPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return OSPlatform.Windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.macOS))
            return OSPlatform.macOS;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return OSPlatform.Linux;
        if (OperatingSystem.IsAndroid())
            return OSPlatform.Create("ANDROID");
        
        throw new PlatformNotSupportedException("Current platform not supported for testing");
    }
    
    private List<InventoryItem> CreateStandardInventoryOperations(int count)
    {
        var parts = new[] { "STD_PART_001", "STD_PART_002", "STD_PART_003" };
        var operations = new[] { "90", "100", "110", "120" };
        var locations = new[] { "STATION_A", "STATION_B" };
        
        return Enumerable.Range(1, count)
            .Select(i => new InventoryItem
            {
                PartId = parts[i % parts.Length],
                Operation = operations[i % operations.Length],
                Quantity = 10,
                Location = locations[i % locations.Length],
                TransactionType = "IN",
                UserId = "PerfTest"
            })
            .ToList();
    }
    
    private IInventoryService CreateInventoryService()
    {
        var services = new ServiceCollection();
        services.AddMTMServices(CreateTestConfiguration());
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IInventoryService>();
    }
    
    private IDatabaseService CreateDatabaseService()
    {
        var services = new ServiceCollection();
        services.AddMTMServices(CreateTestConfiguration());
        
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IDatabaseService>();
    }
    
    private IConfiguration CreateTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=mtm_cross_platform_test;Uid=test_user;Pwd=test_password;",
                ["MTMSettings:DefaultOperation"] = "100"
            })
            .Build();
    }
}

public class PerformanceBaseline
{
    public TimeSpan InventoryOperationTime { get; set; }
    public TimeSpan DatabaseQueryTime { get; set; }
    public TimeSpan UIRenderTime { get; set; }
    public double MemoryEfficiency { get; set; }
}
```

## 📊 Performance Reporting and Analysis

### Automated Performance Reporting

```csharp
public class PerformanceReporter
{
    private readonly List<PerformanceResult> _results = new();
    
    public void AddResult(PerformanceResult result)
    {
        _results.Add(result);
    }
    
    public async Task GeneratePerformanceReportAsync(string outputPath)
    {
        var report = new
        {
            TestRun = new
            {
                Timestamp = DateTime.Now,
                Platform = RuntimeInformation.OSDescription,
                Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
                DotNetVersion = RuntimeInformation.FrameworkDescription
            },
            Summary = new
            {
                TotalTests = _results.Count,
                PassedTests = _results.Count(r => r.IsSuccessful),
                FailedTests = _results.Count(r => !r.IsSuccessful),
                AverageExecutionTime = _results.Average(r => r.ElapsedTime.TotalMilliseconds),
                TotalMemoryUsed = _results.Sum(r => r.MemoryUsed),
                AverageMemoryPerTest = _results.Average(r => r.MemoryUsed)
            },
            CategoryBreakdown = _results
                .GroupBy(r => GetTestCategory(r.OperationName))
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count(),
                    AverageTime = g.Average(r => r.ElapsedTime.TotalMilliseconds),
                    AverageMemory = g.Average(r => r.MemoryUsed / 1024.0 / 1024.0),
                    SuccessRate = g.Count(r => r.IsSuccessful) / (double)g.Count()
                }),
            DetailedResults = _results.Select(r => new
            {
                r.OperationName,
                ExecutionTimeMs = r.ElapsedTime.TotalMilliseconds,
                MemoryUsedMB = r.MemoryUsed / 1024.0 / 1024.0,
                r.IsSuccessful,
                ErrorMessage = r.Exception?.Message
            })
        };
        
        var jsonReport = JsonSerializer.Serialize(report, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await File.WriteAllTextAsync(outputPath, jsonReport);
        
        Console.WriteLine($"Performance report generated: {outputPath}");
        Console.WriteLine($"Test Summary: {report.Summary.PassedTests}/{report.Summary.TotalTests} passed");
        Console.WriteLine($"Average Execution Time: {report.Summary.AverageExecutionTime:F1}ms");
        Console.WriteLine($"Average Memory Usage: {report.Summary.AverageMemoryPerTest / 1024.0 / 1024.0:F1}MB");
    }
    
    private string GetTestCategory(string operationName)
    {
        return operationName.ToLowerInvariant() switch
        {
            var name when name.Contains("inventory") => "Inventory Operations",
            var name when name.Contains("database") => "Database Operations",
            var name when name.Contains("ui") => "UI Operations",
            var name when name.Contains("memory") => "Memory Tests",
            var name when name.Contains("concurrent") => "Concurrency Tests",
            var name when name.Contains("shift") => "Shift Operations",
            _ => "Other"
        };
    }
}
```

## 🎯 Performance Testing Guidelines

### Critical Performance Metrics

1. **Inventory Operations**: <500ms per transaction, 95%+ success rate
2. **Database Operations**: <2s for complex queries, connection pool efficiency
3. **UI Responsiveness**: <100ms for user interactions, smooth scrolling
4. **Memory Usage**: <200MB baseline, <50% growth over 8-hour shifts
5. **Concurrent Operations**: Support 50+ simultaneous users
6. **Shift Changeover**: Complete within 1 minute for 25 users
7. **Cross-Platform Consistency**: Performance variance <25% between platforms

This comprehensive performance testing framework ensures MTM WIP Application maintains manufacturing-grade performance standards under real-world operating conditions across all supported platforms.

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