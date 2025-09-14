using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;

namespace MTM.Tests.PerformanceTests
{
    /// <summary>
    /// Performance tests for manufacturing operations and high-load scenarios
    /// Tests system performance under manufacturing-grade workloads
    /// </summary>
    [TestFixture]
    [Category("Performance")]
    [Category("Manufacturing")]
    [Category("LoadTesting")]
    public class ManufacturingPerformanceTests
    {
        #region Test Setup & Configuration

        private IServiceProvider _serviceProvider = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var services = new ServiceCollection();
            ConfigurePerformanceTestServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void ConfigurePerformanceTestServices(IServiceCollection services)
        {
            // Add logging for performance monitoring
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

            // Add mocked services for performance testing
            var mockConfig = new Mock<IConfigurationService>();
            mockConfig.Setup(s => s.GetConnectionString(It.IsAny<string>()))
                .Returns("Server=localhost;Database=mtm_perf_test;Uid=test;Pwd=test;");
            services.AddSingleton(mockConfig.Object);

            var mockAppState = new Mock<IApplicationStateService>();
            services.AddSingleton(mockAppState.Object);

            var mockNavigation = new Mock<INavigationService>();
            services.AddSingleton(mockNavigation.Object);

            // Add real services for performance testing
            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddScoped<IMasterDataService, MasterDataService>();
        }

        #endregion

        #region Manufacturing Workload Performance Tests

        [Test]
        public async Task ManufacturingWorkload_HighVolumeInventoryOperations_ShouldMeetPerformanceRequirements()
        {
            // Arrange
            const int operationsCount = 1000;
            const int maxAllowedTimeMs = 10000; // 10 seconds for 1000 operations
            
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var stopwatch = Stopwatch.StartNew();

            // Act - Simulate high-volume manufacturing operations
            var operations = new List<Task>();
            for (int i = 0; i < operationsCount; i++)
            {
                operations.Add(Task.Run(async () =>
                {
                    // Simulate inventory operation
                    await masterDataService.LoadAllMasterDataAsync();
                    
                    // Access collections (simulates UI data binding)
                    var partCount = masterDataService.PartIds.Count;
                    var operationCount = masterDataService.Operations.Count;
                    var locationCount = masterDataService.Locations.Count;
                    
                    return partCount + operationCount + locationCount;
                }));
            }

            await Task.WhenAll(operations);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxAllowedTimeMs,
                $"1000 manufacturing operations should complete within {maxAllowedTimeMs}ms, took {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine($"Manufacturing Performance: {operationsCount} operations in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Average: {(double)stopwatch.ElapsedMilliseconds / operationsCount:F2}ms per operation");
        }

        [Test]
        public async Task ManufacturingWorkload_ConcurrentPartLookups_ShouldHandleLoad()
        {
            // Arrange
            const int concurrentUsers = 20;
            const int lookupsPerUser = 50;
            const int maxAllowedTimeMs = 15000; // 15 seconds for all lookups
            
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var stopwatch = Stopwatch.StartNew();
            var successCount = 0;
            var errorCount = 0;

            // Act - Simulate concurrent manufacturing part lookups
            var userTasks = Enumerable.Range(1, concurrentUsers).Select(async userId =>
            {
                var userSuccess = 0;
                var userErrors = 0;

                for (int lookup = 0; lookup < lookupsPerUser; lookup++)
                {
                    try
                    {
                        // Simulate part lookup operations
                        await masterDataService.RefreshPartIdsAsync();
                        var partIds = masterDataService.PartIds;
                        
                        // Simulate processing
                        await Task.Delay(1);
                        
                        if (partIds != null)
                        {
                            userSuccess++;
                        }
                    }
                    catch
                    {
                        userErrors++;
                    }
                }

                return new { Success = userSuccess, Errors = userErrors };
            });

            var results = await Task.WhenAll(userTasks);
            stopwatch.Stop();

            // Aggregate results
            successCount = results.Sum(r => r.Success);
            errorCount = results.Sum(r => r.Errors);

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxAllowedTimeMs,
                $"Concurrent lookups should complete within {maxAllowedTimeMs}ms");

            var totalOperations = concurrentUsers * lookupsPerUser;
            var successRate = (double)successCount / totalOperations * 100;
            
            successRate.Should().BeGreaterThan(95, "At least 95% of operations should succeed under load");

            Console.WriteLine($"Concurrent Lookup Performance:");
            Console.WriteLine($"  Total Operations: {totalOperations}");
            Console.WriteLine($"  Success: {successCount} ({successRate:F1}%)");
            Console.WriteLine($"  Errors: {errorCount}");
            Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Throughput: {totalOperations * 1000.0 / stopwatch.ElapsedMilliseconds:F1} ops/sec");
        }

        #endregion

        #region Memory Performance Tests

        [Test]
        public void MemoryPerformance_ManufacturingOperations_ShouldNotCauseMemoryLeaks()
        {
            // Arrange
            const int iterations = 500;
            var initialMemory = GC.GetTotalMemory(true);

            // Act - Perform many manufacturing operations
            for (int i = 0; i < iterations; i++)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var masterDataService = scope.ServiceProvider.GetRequiredService<IMasterDataService>();
                    var databaseService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();

                    // Simulate manufacturing operations
                    _ = masterDataService.PartIds;
                    _ = masterDataService.Operations;
                    _ = masterDataService.Locations;
                    _ = databaseService.GetConnectionString();
                }

                // Force garbage collection every 100 iterations
                if (i % 100 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }

            // Final cleanup and measurement
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(false);
            var memoryIncrease = finalMemory - initialMemory;

            // Assert
            memoryIncrease.Should().BeLessThan(50 * 1024 * 1024, // 50MB limit
                $"Memory usage should not increase significantly. Increase: {memoryIncrease / 1024 / 1024}MB");

            Console.WriteLine($"Memory Performance:");
            Console.WriteLine($"  Initial Memory: {initialMemory / 1024 / 1024:F2}MB");
            Console.WriteLine($"  Final Memory: {finalMemory / 1024 / 1024:F2}MB");
            Console.WriteLine($"  Memory Increase: {memoryIncrease / 1024 / 1024:F2}MB");
            Console.WriteLine($"  Operations: {iterations}");
        }

        [Test]
        public async Task MemoryPerformance_ViewModelCreation_ShouldBeEfficient()
        {
            // Arrange
            const int viewModelCount = 100;
            var initialMemory = GC.GetTotalMemory(true);
            var viewModels = new List<InventoryTabViewModel>();

            var mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
            var mockAppState = _serviceProvider.GetRequiredService<IApplicationStateService>();
            var mockNavigation = _serviceProvider.GetRequiredService<INavigationService>();
            var mockDatabase = _serviceProvider.GetRequiredService<IDatabaseService>();
            var mockConfig = _serviceProvider.GetRequiredService<IConfigurationService>();
            var mockMasterData = _serviceProvider.GetRequiredService<IMasterDataService>();

            // Act - Create multiple ViewModels  
            for (int i = 0; i < viewModelCount; i++)
            {
                var viewModel = new InventoryTabViewModel(
                    mockAppState,               // IApplicationStateService
                    mockNavigation,             // INavigationService
                    mockDatabase,               // IDatabaseService
                    mockConfig,                 // IConfigurationService
                    null!,                      // ISuggestionOverlayService
                    mockMasterData,             // IMasterDataService
                    null                        // ISuccessOverlayService (optional)
                );
                
                viewModels.Add(viewModel);
            }

            var afterCreationMemory = GC.GetTotalMemory(false);

            // Clean up ViewModels
            foreach (var vm in viewModels)
            {
                vm.Dispose();
            }
            viewModels.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(false);

            // Assert
            var creationMemoryIncrease = afterCreationMemory - initialMemory;
            var memoryPerViewModel = creationMemoryIncrease / viewModelCount;

            memoryPerViewModel.Should().BeLessThan(1024 * 1024, // 1MB per ViewModel should be reasonable
                $"Each ViewModel should not consume excessive memory. Average: {memoryPerViewModel / 1024:F2}KB per ViewModel");

            var memoryLeak = finalMemory - initialMemory;
            memoryLeak.Should().BeLessThan(10 * 1024 * 1024, // 10MB leak tolerance
                $"Memory should be properly released after disposal. Potential leak: {memoryLeak / 1024 / 1024:F2}MB");

            Console.WriteLine($"ViewModel Memory Performance:");
            Console.WriteLine($"  ViewModels Created: {viewModelCount}");
            Console.WriteLine($"  Memory per ViewModel: {memoryPerViewModel / 1024:F2}KB");
            Console.WriteLine($"  Total Creation Memory: {creationMemoryIncrease / 1024 / 1024:F2}MB");
            Console.WriteLine($"  Memory Leak: {memoryLeak / 1024 / 1024:F2}MB");

            await Task.CompletedTask; // Satisfy async test requirement
        }

        #endregion

        #region Threading and Concurrency Performance Tests

        [Test]
        public async Task ConcurrencyPerformance_MultipleManufacturingStations_ShouldHandleLoad()
        {
            // Arrange - Simulate multiple manufacturing stations operating simultaneously
            const int stationCount = 10;
            const int operationsPerStation = 100;
            const int maxAllowedTimeMs = 20000; // 20 seconds

            var stopwatch = Stopwatch.StartNew();
            var results = new ConcurrentBag<TimeSpan>();
            var errors = new ConcurrentBag<Exception>();

            // Act - Simulate multiple manufacturing stations
            var stationTasks = Enumerable.Range(1, stationCount).Select(async stationId =>
            {
                var stationStopwatch = Stopwatch.StartNew();

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var masterDataService = scope.ServiceProvider.GetRequiredService<IMasterDataService>();

                    for (int operation = 0; operation < operationsPerStation; operation++)
                    {
                        // Simulate manufacturing station operations
                        await masterDataService.LoadAllMasterDataAsync();
                        
                        // Simulate part lookup
                        var partIds = masterDataService.PartIds;
                        var operations = masterDataService.Operations;
                        var locations = masterDataService.Locations;

                        // Simulate processing time
                        await Task.Delay(1);
                    }

                    stationStopwatch.Stop();
                    results.Add(stationStopwatch.Elapsed);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            });

            await Task.WhenAll(stationTasks);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(maxAllowedTimeMs,
                $"All manufacturing stations should complete within {maxAllowedTimeMs}ms");

            errors.Should().BeEmpty("No errors should occur during concurrent manufacturing operations");

            var totalOperations = stationCount * operationsPerStation;
            var throughput = totalOperations * 1000.0 / stopwatch.ElapsedMilliseconds;
            var averageStationTime = results.Any() ? results.Average(r => r.TotalMilliseconds) : 0;

            Console.WriteLine($"Manufacturing Concurrency Performance:");
            Console.WriteLine($"  Stations: {stationCount}");
            Console.WriteLine($"  Operations per Station: {operationsPerStation}");
            Console.WriteLine($"  Total Operations: {totalOperations}");
            Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Throughput: {throughput:F1} operations/second");
            Console.WriteLine($"  Average Station Time: {averageStationTime:F2}ms");
            Console.WriteLine($"  Errors: {errors.Count}");
        }

        [Test]
        public void ThreadSafety_ConcurrentServiceAccess_ShouldBeSafe()
        {
            // Arrange
            const int threadCount = 20;
            const int operationsPerThread = 50;
            var exceptions = new ConcurrentBag<Exception>();
            var operationCounts = new ConcurrentBag<int>();

            // Act - Test thread safety of services
            var threads = new Task[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = Task.Run(() =>
                {
                    try
                    {
                        var operationCount = 0;
                        
                        for (int op = 0; op < operationsPerThread; op++)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var masterDataService = scope.ServiceProvider.GetRequiredService<IMasterDataService>();
                            var configService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();

                            // Perform thread-safe operations
                            _ = masterDataService.PartIds.Count;
                            _ = configService.GetConnectionString();
                            
                            operationCount++;
                        }
                        
                        operationCounts.Add(operationCount);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                });
            }

            Task.WaitAll(threads, TimeSpan.FromSeconds(30));

            // Assert
            exceptions.Should().BeEmpty("No thread safety exceptions should occur");
            operationCounts.Sum().Should().Be(threadCount * operationsPerThread, 
                "All operations should complete successfully");

            Console.WriteLine($"Thread Safety Performance:");
            Console.WriteLine($"  Threads: {threadCount}");
            Console.WriteLine($"  Operations per Thread: {operationsPerThread}");
            Console.WriteLine($"  Total Operations: {operationCounts.Sum()}");
            Console.WriteLine($"  Exceptions: {exceptions.Count}");
        }

        #endregion

        #region Manufacturing-Specific Performance Requirements

        [Test]
        public async Task ManufacturingRequirements_InventorySaveOperations_ShouldMeetResponseTime()
        {
            // Manufacturing requirement: Inventory saves should complete within 200ms
            const int maxResponseTimeMs = 200;
            const int testIterations = 50;

            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var responseTimes = new List<long>();

            // Act - Test inventory save response times
            for (int i = 0; i < testIterations; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                
                // Simulate inventory save operation
                await masterDataService.LoadAllMasterDataAsync();
                
                // Access collections (simulates validation and save)
                _ = masterDataService.PartIds;
                _ = masterDataService.Operations;
                _ = masterDataService.Locations;

                stopwatch.Stop();
                responseTimes.Add(stopwatch.ElapsedMilliseconds);
            }

            // Assert
            var averageResponseTime = responseTimes.Average();
            var maxResponseTime = responseTimes.Max();
            var responseTimesUnderLimit = responseTimes.Count(rt => rt <= maxResponseTimeMs);
            var successRate = (double)responseTimesUnderLimit / testIterations * 100;

            averageResponseTime.Should().BeLessThan(maxResponseTimeMs,
                $"Average response time should be under {maxResponseTimeMs}ms, was {averageResponseTime:F2}ms");

            successRate.Should().BeGreaterThan(90,
                $"At least 90% of operations should meet response time requirements, {successRate:F1}% succeeded");

            Console.WriteLine($"Manufacturing Response Time Requirements:");
            Console.WriteLine($"  Target: {maxResponseTimeMs}ms");
            Console.WriteLine($"  Average: {averageResponseTime:F2}ms");
            Console.WriteLine($"  Maximum: {maxResponseTime}ms");
            Console.WriteLine($"  Success Rate: {successRate:F1}%");
            Console.WriteLine($"  Operations under limit: {responseTimesUnderLimit}/{testIterations}");
        }

        #endregion
    }
}