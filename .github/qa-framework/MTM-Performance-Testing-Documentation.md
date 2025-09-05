# MTM Performance Testing Documentation

## 📋 Overview

This document establishes comprehensive performance testing guidelines for the MTM WIP Application, specifically designed for manufacturing environments where performance directly impacts production efficiency. The framework covers load testing, stress testing, endurance testing, and manufacturing-specific performance scenarios.

## 🎯 **Performance Testing Strategy**

### **Manufacturing Performance Requirements**
```yaml
Critical Performance Metrics:
  - Response Time: Core operations must complete within 2 seconds
  - Throughput: System must handle 100+ concurrent users
  - Reliability: 99.9% uptime during production hours
  - Resource Usage: Memory usage must remain stable over 24-hour periods
  - Database Performance: Query response times under 500ms
  - UI Responsiveness: Interface updates within 100ms
```

### **Performance Testing Categories**
```
                [Endurance Testing]
                     (24/7 Operations)
                Manufacturing Shift Testing
                Memory Leak Detection
                
              [Stress Testing]
                  (Peak Load Scenarios)
              End-of-Shift Processing
              Inventory Count Operations
              
            [Load Testing]
                (Normal Operations)
            Concurrent User Scenarios
            Typical Manufacturing Workflows
            
          [Component Testing]
              (Individual Components)
          Database Query Performance
          UI Component Rendering
          Service Response Times
```

## 🧪 **Performance Testing Framework**

### **Testing Stack Configuration**
```xml
<!-- Performance Testing Dependencies -->
<PackageReference Include="NBomber" Version="5.6.0" />
<PackageReference Include="NBomber.Http" Version="5.6.0" />
<PackageReference Include="BenchmarkDotNet" Version="0.13.10" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.2" />
<PackageReference Include="MySql.Data" Version="9.4.0" />

<!-- Monitoring and Metrics -->
<PackageReference Include="System.Diagnostics.PerformanceCounter" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="8.0.0" />
<PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
```

### **Load Testing Framework**
```csharp
// Manufacturing Load Testing Scenarios
public class ManufacturingLoadTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    [Fact]
    public void InventoryOperations_LoadTest()
    {
        var scenario = Scenario.Create("inventory_operations", async context =>
        {
            var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
            
            // Simulate typical manufacturing operations
            var operations = new[]
            {
                () => AddInventoryAsync(inventoryService, GenerateTestPartId()),
                () => SearchInventoryAsync(inventoryService, "PART"),
                () => TransferInventoryAsync(inventoryService, GenerateTestPartId()),
                () => RemoveInventoryAsync(inventoryService, GenerateTestPartId())
            };

            var randomOperation = operations[Random.Shared.Next(operations.Length)];
            
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await randomOperation();
                stopwatch.Stop();
                
                return Response.Ok(statusCode: "success", sizeBytes: 1024)
                              .SetLatency(stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return Response.Fail(ex.Message)
                              .SetLatency(stopwatch.Elapsed);
            }
        })
        .WithLoadSimulations(
            Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(5)), // Ramp up
            Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(10)), // Normal load
            Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromMinutes(5)), // Peak load
            Simulation.InjectPerSec(rate: 25, during: TimeSpan.FromMinutes(5))  // Ramp down
        );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder("performance-reports")
            .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
            .Run();

        // Assert performance requirements
        var scnStats = stats.AllScenarioStats.First();
        
        // Response time requirements
        Assert.True(scnStats.Ok.Response.Mean < TimeSpan.FromMilliseconds(2000),
            $"Mean response time {scnStats.Ok.Response.Mean.TotalMilliseconds}ms exceeds 2000ms requirement");
        
        // Success rate requirements
        Assert.True(scnStats.Ok.Request.Count / (double)scnStats.AllRequestCount > 0.99,
            $"Success rate {scnStats.Ok.Request.Count / (double)scnStats.AllRequestCount:P2} below 99% requirement");
    }

    [Fact]
    public void ConcurrentUsers_LoadTest()
    {
        var scenario = Scenario.Create("concurrent_users", async context =>
        {
            // Simulate a complete user session
            var sessionId = context.ScenarioInfo.InstanceId;
            
            // Login simulation
            await SimulateUserLoginAsync(sessionId);
            
            // Typical user workflow
            await SimulateInventorySearchAsync(sessionId);
            await SimulateInventoryAdditionAsync(sessionId);
            await SimulateTransactionViewAsync(sessionId);
            
            // Logout simulation
            await SimulateUserLogoutAsync(sessionId);
            
            return Response.Ok();
        })
        .WithLoadSimulations(
            Simulation.KeepConstant(copies: 25, during: TimeSpan.FromMinutes(10)), // 25 concurrent users
            Simulation.KeepConstant(copies: 50, during: TimeSpan.FromMinutes(10)), // 50 concurrent users
            Simulation.KeepConstant(copies: 100, during: TimeSpan.FromMinutes(10)) // 100 concurrent users
        );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder("concurrent-user-reports")
            .Run();

        // Validate concurrent user performance
        var scnStats = stats.AllScenarioStats.First();
        Assert.True(scnStats.Ok.Response.Mean < TimeSpan.FromMilliseconds(3000),
            "Concurrent user sessions must complete within 3 seconds");
    }

    private async Task<Response> AddInventoryAsync(IInventoryService service, string partId)
    {
        var item = new InventoryItem
        {
            PartId = partId,
            Quantity = Random.Shared.Next(1, 100),
            Location = $"WH-{Random.Shared.Next(1, 10):D3}",
            Operation = "100",
            TransactionType = "IN"
        };

        var result = await service.AddInventoryAsync(item);
        return result.IsSuccess ? Response.Ok() : Response.Fail(result.ErrorMessage);
    }

    private string GenerateTestPartId() => $"PERF_TEST_{Random.Shared.Next(1000, 9999)}";
}
```

### **Database Performance Testing**
```csharp
// Database Performance Benchmarks
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[RPlotExporter]
public class DatabasePerformanceBenchmarks
{
    private string _connectionString;
    private List<InventoryItem> _testData;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _connectionString = GetTestConnectionString();
        _testData = GenerateTestData(1000);
        
        // Setup test database
        await SetupPerformanceTestDatabaseAsync();
    }

    [Benchmark]
    [Arguments(1)]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task<int> StoredProcedure_AddInventory_Batch(int batchSize)
    {
        var batch = _testData.Take(batchSize);
        var successCount = 0;

        foreach (var item in batch)
        {
            var parameters = new MySqlParameter[]
            {
                new("p_PartID", item.PartId),
                new("p_Quantity", item.Quantity),
                new("p_Location", item.Location),
                new("p_Operation", item.Operation),
                new("p_TransactionType", "IN")
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Add_Item",
                parameters
            );

            if (result.Status == 1) successCount++;
        }

        return successCount;
    }

    [Benchmark]
    [Arguments("PART%")]
    [Arguments("PART001")]
    [Arguments("%001")]
    public async Task<int> StoredProcedure_SearchInventory(string searchPattern)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_SearchPattern", searchPattern)
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Search_ByPattern",
            parameters
        );

        return result.Data?.Rows.Count ?? 0;
    }

    [Benchmark]
    public async Task<int> StoredProcedure_GetTransactionHistory()
    {
        var parameters = new MySqlParameter[]
        {
            new("p_StartDate", DateTime.Today.AddDays(-30)),
            new("p_EndDate", DateTime.Today)
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_transaction_Get_History",
            parameters
        );

        return result.Data?.Rows.Count ?? 0;
    }

    [Benchmark]
    public async Task<TimeSpan> DatabaseConnection_EstablishConnection()
    {
        var stopwatch = Stopwatch.StartNew();
        
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    [Benchmark]
    public async Task<int> ConcurrentOperations_InventoryUpdates()
    {
        const int concurrentOperations = 10;
        var tasks = new List<Task<bool>>();

        for (int i = 0; i < concurrentOperations; i++)
        {
            var partId = $"CONCURRENT_{i:D3}";
            tasks.Add(PerformConcurrentInventoryOperationAsync(partId));
        }

        var results = await Task.WhenAll(tasks);
        return results.Count(r => r);
    }

    private async Task<bool> PerformConcurrentInventoryOperationAsync(string partId)
    {
        try
        {
            // Add inventory
            var addResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Add_Item",
                new MySqlParameter[]
                {
                    new("p_PartID", partId),
                    new("p_Quantity", 100),
                    new("p_Location", "PERF_TEST"),
                    new("p_Operation", "100"),
                    new("p_TransactionType", "IN")
                }
            );

            if (addResult.Status != 1) return false;

            // Update inventory
            var updateResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Update_Quantity",
                new MySqlParameter[]
                {
                    new("p_PartID", partId),
                    new("p_Location", "PERF_TEST"),
                    new("p_Operation", "100"),
                    new("p_NewQuantity", 150)
                }
            );

            return updateResult.Status == 1;
        }
        catch
        {
            return false;
        }
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await CleanupPerformanceTestDataAsync();
    }
}
```

## 🏭 **Manufacturing-Specific Performance Scenarios**

### **Shift Change Performance Testing**
```csharp
// End-of-Shift Performance Testing
[TestFixture]
[Category("Performance")]
public class ShiftChangePerformanceTests
{
    [Test]
    public async Task EndOfShift_InventoryCount_PerformanceTest()
    {
        // Arrange - Setup shift change scenario with large inventory
        var inventoryItems = GenerateShiftInventoryData(10000); // 10,000 items
        await SeedShiftTestDataAsync(inventoryItems);

        var stopwatch = new Stopwatch();
        var memoryBefore = GC.GetTotalMemory(true);

        // Act - Simulate end-of-shift inventory processing
        stopwatch.Start();
        
        var tasks = new[]
        {
            ProcessInventoryCountAsync(),
            GenerateShiftReportAsync(),
            UpdateShiftTotalsAsync(),
            BackupShiftDataAsync()
        };

        await Task.WhenAll(tasks);
        
        stopwatch.Stop();
        var memoryAfter = GC.GetTotalMemory(true);

        // Assert - Performance requirements
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromMinutes(5)),
            "End-of-shift processing must complete within 5 minutes");
        
        Assert.That(memoryAfter - memoryBefore, Is.LessThan(50_000_000),
            "Memory usage increase must be less than 50MB");
    }

    [Test]
    public async Task ShiftHandoff_ConcurrentAccess_PerformanceTest()
    {
        // Simulate concurrent access during shift change
        const int outgoingShiftUsers = 25;
        const int incomingShiftUsers = 25;
        
        var outgoingTasks = Enumerable.Range(0, outgoingShiftUsers)
            .Select(i => SimulateOutgoingShiftUserAsync(i))
            .ToArray();
            
        var incomingTasks = Enumerable.Range(0, incomingShiftUsers)
            .Select(i => SimulateIncomingShiftUserAsync(i))
            .ToArray();

        var stopwatch = Stopwatch.StartNew();
        
        // Execute concurrent shift change operations
        await Task.WhenAll(outgoingTasks.Concat(incomingTasks));
        
        stopwatch.Stop();

        // Assert performance during concurrent access
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromMinutes(2)),
            "Shift handoff must complete within 2 minutes");
            
        // Verify data integrity after concurrent operations
        var integrityCheck = await VerifyDataIntegrityAsync();
        Assert.That(integrityCheck, Is.True,
            "Data integrity must be maintained during concurrent access");
    }

    private async Task<bool> SimulateOutgoingShiftUserAsync(int userId)
    {
        // Simulate end-of-shift activities
        var activities = new[]
        {
            () => FinalizeWorkOrdersAsync(userId),
            () => UpdateInventoryCountsAsync(userId),
            () => GenerateShiftSummaryAsync(userId),
            () => LogoutUserAsync(userId)
        };

        foreach (var activity in activities)
        {
            await activity();
            await Task.Delay(Random.Shared.Next(100, 500)); // Random delay between activities
        }

        return true;
    }

    private async Task<bool> SimulateIncomingShiftUserAsync(int userId)
    {
        // Simulate beginning-of-shift activities
        var activities = new[]
        {
            () => LoginUserAsync(userId),
            () => ReviewShiftHandoffAsync(userId),
            () => InitializeWorkstationAsync(userId),
            () => StartInitialInventoryCheckAsync(userId)
        };

        foreach (var activity in activities)
        {
            await activity();
            await Task.Delay(Random.Shared.Next(50, 200)); // Faster startup activities
        }

        return true;
    }
}
```

### **Production Line Performance Testing**
```csharp
// Production Line Flow Performance
[TestFixture]
[Category("Performance")]
public class ProductionLinePerformanceTests
{
    [Test]
    public async Task ProductionLine_HighThroughput_PerformanceTest()
    {
        // Arrange - Setup high-volume production scenario
        const int partsPerMinute = 100;
        const int testDurationMinutes = 10;
        const int totalParts = partsPerMinute * testDurationMinutes;

        var productionQueue = GenerateProductionBatch(totalParts);
        var stopwatch = new Stopwatch();
        var completedParts = 0;
        var errors = 0;

        // Act - Simulate high-throughput production processing
        stopwatch.Start();
        
        await Parallel.ForEachAsync(productionQueue, 
            new ParallelOptions { MaxDegreeOfParallelism = 20 },
            async (part, cancellationToken) =>
            {
                try
                {
                    await ProcessProductionPartAsync(part);
                    Interlocked.Increment(ref completedParts);
                }
                catch
                {
                    Interlocked.Increment(ref errors);
                }
            });
        
        stopwatch.Stop();

        // Assert - Production performance requirements
        var throughput = completedParts / stopwatch.Elapsed.TotalMinutes;
        Assert.That(throughput, Is.GreaterThan(partsPerMinute * 0.95),
            $"Throughput {throughput:F2} parts/minute below required {partsPerMinute}");
        
        var errorRate = errors / (double)totalParts;
        Assert.That(errorRate, Is.LessThan(0.01),
            $"Error rate {errorRate:P2} exceeds 1% threshold");
    }

    [Test]
    public async Task WorkstationHandoff_OperationFlow_PerformanceTest()
    {
        // Test performance of parts flowing through operations 90→100→110→120
        var operations = new[] { "90", "100", "110", "120" };
        var batchSize = 1000;
        var partId = "FLOW_TEST_BATCH";

        var stopwatch = Stopwatch.StartNew();

        // Process through each operation
        foreach (var operation in operations)
        {
            var operationStart = stopwatch.Elapsed;
            
            if (operation == "90")
            {
                // Initial raw material addition
                await AddInventoryBatchAsync(partId, operation, batchSize);
            }
            else
            {
                // Transfer from previous operation
                var previousOperation = operations[Array.IndexOf(operations, operation) - 1];
                await TransferBetweenOperationsAsync(partId, previousOperation, operation, batchSize);
            }

            var operationTime = stopwatch.Elapsed - operationStart;
            
            // Assert operation performance
            Assert.That(operationTime, Is.LessThan(TimeSpan.FromSeconds(30)),
                $"Operation {operation} processing time {operationTime.TotalSeconds:F1}s exceeds 30s limit");
        }

        stopwatch.Stop();
        
        // Assert total flow time
        Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromMinutes(3)),
            $"Complete operation flow time {stopwatch.Elapsed.TotalMinutes:F1} minutes exceeds 3-minute limit");
    }

    private async Task ProcessProductionPartAsync(ProductionPart part)
    {
        // Simulate realistic production part processing
        var operations = new[]
        {
            () => ValidatePartAsync(part),
            () => UpdateInventoryAsync(part),
            () => LogTransactionAsync(part),
            () => UpdateWorkOrderAsync(part)
        };

        foreach (var operation in operations)
        {
            await operation();
        }
    }
}
```

## 📊 **Performance Monitoring and Metrics**

### **Real-time Performance Monitoring**
```csharp
// Performance Metrics Collection
public class PerformanceMetricsService : IPerformanceMetricsService
{
    private readonly ILogger<PerformanceMetricsService> _logger;
    private readonly PerformanceCounters _counters;

    public class PerformanceCounters
    {
        public PerformanceCounter CpuUsage { get; set; }
        public PerformanceCounter MemoryUsage { get; set; }
        public PerformanceCounter DiskIOps { get; set; }
        public PerformanceCounter DatabaseConnections { get; set; }
        
        // Custom application counters
        public Counter InventoryOperationsPerSecond { get; set; }
        public Counter DatabaseQueryLatency { get; set; }
        public Counter ConcurrentUsers { get; set; }
        public Counter ErrorRate { get; set; }
    }

    public async Task StartMonitoringAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var metrics = await CollectMetricsAsync();
            await ProcessMetricsAsync(metrics);
            await EvaluatePerformanceThresholds(metrics);
            
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }

    private async Task<PerformanceMetrics> CollectMetricsAsync()
    {
        return new PerformanceMetrics
        {
            Timestamp = DateTime.UtcNow,
            CpuUsagePercent = _counters.CpuUsage.NextValue(),
            MemoryUsageMB = _counters.MemoryUsage.NextValue() / 1024 / 1024,
            DatabaseConnections = (int)_counters.DatabaseConnections.NextValue(),
            InventoryOpsPerSecond = _counters.InventoryOperationsPerSecond.GetCurrentValue(),
            AverageQueryLatencyMs = _counters.DatabaseQueryLatency.GetAverage(),
            ConcurrentUsers = _counters.ConcurrentUsers.GetCurrentValue(),
            ErrorRatePercent = _counters.ErrorRate.GetRate() * 100
        };
    }

    private async Task EvaluatePerformanceThresholds(PerformanceMetrics metrics)
    {
        var alerts = new List<PerformanceAlert>();

        // CPU Usage Alert
        if (metrics.CpuUsagePercent > 80)
        {
            alerts.Add(new PerformanceAlert
            {
                Type = AlertType.HighCpuUsage,
                Message = $"CPU usage is {metrics.CpuUsagePercent:F1}% (threshold: 80%)",
                Severity = metrics.CpuUsagePercent > 95 ? AlertSeverity.Critical : AlertSeverity.Warning
            });
        }

        // Memory Usage Alert
        if (metrics.MemoryUsageMB > 2048)
        {
            alerts.Add(new PerformanceAlert
            {
                Type = AlertType.HighMemoryUsage,
                Message = $"Memory usage is {metrics.MemoryUsageMB:F0}MB (threshold: 2GB)",
                Severity = metrics.MemoryUsageMB > 4096 ? AlertSeverity.Critical : AlertSeverity.Warning
            });
        }

        // Response Time Alert
        if (metrics.AverageQueryLatencyMs > 1000)
        {
            alerts.Add(new PerformanceAlert
            {
                Type = AlertType.SlowDatabaseResponse,
                Message = $"Database queries averaging {metrics.AverageQueryLatencyMs:F0}ms (threshold: 1000ms)",
                Severity = metrics.AverageQueryLatencyMs > 5000 ? AlertSeverity.Critical : AlertSeverity.Warning
            });
        }

        // Error Rate Alert
        if (metrics.ErrorRatePercent > 1)
        {
            alerts.Add(new PerformanceAlert
            {
                Type = AlertType.HighErrorRate,
                Message = $"Error rate is {metrics.ErrorRatePercent:F2}% (threshold: 1%)",
                Severity = metrics.ErrorRatePercent > 5 ? AlertSeverity.Critical : AlertSeverity.Warning
            });
        }

        // Process alerts
        foreach (var alert in alerts)
        {
            await ProcessPerformanceAlertAsync(alert);
        }
    }
}

public class PerformanceMetrics
{
    public DateTime Timestamp { get; set; }
    public float CpuUsagePercent { get; set; }
    public float MemoryUsageMB { get; set; }
    public int DatabaseConnections { get; set; }
    public double InventoryOpsPerSecond { get; set; }
    public double AverageQueryLatencyMs { get; set; }
    public int ConcurrentUsers { get; set; }
    public double ErrorRatePercent { get; set; }
}
```

### **Performance Baseline and Benchmarking**
```csharp
// Performance Baseline Tests
[TestFixture]
[Category("Baseline")]
public class PerformanceBaselineTests
{
    private readonly Dictionary<string, PerformanceBaseline> _baselines = new()
    {
        ["InventoryAddSingle"] = new() { MaxResponseTimeMs = 500, MinThroughputPerSecond = 20 },
        ["InventorySearch"] = new() { MaxResponseTimeMs = 1000, MinThroughputPerSecond = 100 },
        ["TransactionHistory"] = new() { MaxResponseTimeMs = 2000, MinThroughputPerSecond = 10 },
        ["DatabaseConnection"] = new() { MaxResponseTimeMs = 100, MinThroughputPerSecond = 50 },
        ["ConcurrentUsers50"] = new() { MaxResponseTimeMs = 3000, MinThroughputPerSecond = 15 }
    };

    [Test]
    [TestCaseSource(nameof(BaselineTestCases))]
    public async Task PerformanceBaseline_Validation(string operationName, Func<Task<PerformanceResult>> operation)
    {
        // Run the operation multiple times to establish baseline
        var results = new List<PerformanceResult>();
        
        for (int i = 0; i < 10; i++)
        {
            var result = await operation();
            results.Add(result);
            
            // Small delay between runs
            await Task.Delay(100);
        }

        // Calculate statistics
        var avgResponseTime = results.Average(r => r.ResponseTimeMs);
        var maxResponseTime = results.Max(r => r.ResponseTimeMs);
        var minResponseTime = results.Min(r => r.ResponseTimeMs);
        var throughput = 1000.0 / avgResponseTime; // Operations per second

        var baseline = _baselines[operationName];
        
        // Assert against baselines
        Assert.That(avgResponseTime, Is.LessThan(baseline.MaxResponseTimeMs),
            $"Average response time {avgResponseTime:F1}ms exceeds baseline {baseline.MaxResponseTimeMs}ms");
        
        Assert.That(throughput, Is.GreaterThan(baseline.MinThroughputPerSecond),
            $"Throughput {throughput:F1} ops/sec below baseline {baseline.MinThroughputPerSecond} ops/sec");
        
        // Log baseline results for monitoring trends
        _logger.LogInformation("Baseline {Operation}: Avg={AvgMs:F1}ms, Max={MaxMs:F1}ms, Min={MinMs:F1}ms, Throughput={Throughput:F1}ops/sec",
            operationName, avgResponseTime, maxResponseTime, minResponseTime, throughput);
    }

    public static IEnumerable<object[]> BaselineTestCases => new[]
    {
        new object[] { "InventoryAddSingle", (Func<Task<PerformanceResult>>)(() => TestSingleInventoryAddAsync()) },
        new object[] { "InventorySearch", (Func<Task<PerformanceResult>>)(() => TestInventorySearchAsync()) },
        new object[] { "TransactionHistory", (Func<Task<PerformanceResult>>)(() => TestTransactionHistoryAsync()) },
        new object[] { "DatabaseConnection", (Func<Task<PerformanceResult>>)(() => TestDatabaseConnectionAsync()) },
        new object[] { "ConcurrentUsers50", (Func<Task<PerformanceResult>>)(() => TestConcurrentUsersAsync(50)) }
    };
}

public class PerformanceBaseline
{
    public double MaxResponseTimeMs { get; set; }
    public double MinThroughputPerSecond { get; set; }
}

public class PerformanceResult
{
    public double ResponseTimeMs { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
```

This comprehensive performance testing framework ensures the MTM WIP Application meets the demanding performance requirements of manufacturing environments, providing thorough validation of system performance under various load conditions and operational scenarios.
