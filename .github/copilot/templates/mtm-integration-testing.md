---
description: 'Integration testing template for MTM cross-service and cross-platform validation'
applies_to: '**/*'
---

# MTM Integration Testing Template

## 🔗 Integration Testing Instructions

Comprehensive integration testing patterns for MTM WIP Application, focusing on cross-service communication, database integration, and cross-platform compatibility validation.

## Cross-Service Integration Testing

### Service Integration Test Template
```csharp
[TestFixture]
[Category("Integration")]
[Category("CrossService")]
public class [Feature]ServiceIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private DatabaseTestFixture _databaseFixture;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Setup test database
        _databaseFixture = new DatabaseTestFixture();
        await _databaseFixture.SetupAsync();
        
        // Configure services with test dependencies
        var services = new ServiceCollection();
        var configuration = CreateTestConfiguration();
        
        services.AddMTMServices(configuration);
        services.AddSingleton<IConfiguration>(configuration);
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _serviceProvider?.Dispose();
        await _databaseFixture?.TearDownAsync();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        await _databaseFixture.CleanupTestDataAsync();
    }
    
    [Test]
    public async Task [Feature]Services_CrossServiceCommunication_ShouldMaintainDataConsistency()
    {
        // Arrange
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        var transactionService = _serviceProvider.GetRequiredService<ITransactionService>();
        var quickButtonsService = _serviceProvider.GetRequiredService<IQuickButtonsService>();
        
        var testData = new InventoryItem
        {
            PartId = "INTEG_TEST_001",
            Operation = "100",
            Quantity = 50,
            Location = "STATION_A",
            TransactionType = "IN"
        };
        
        // Act - Execute cross-service workflow
        var inventoryResult = await inventoryService.AddInventoryAsync(testData);
        Assert.That(inventoryResult.IsSuccess, Is.True, "Inventory service should succeed");
        
        var transactionResult = await transactionService.RecordTransactionAsync(testData);
        Assert.That(transactionResult.IsSuccess, Is.True, "Transaction service should succeed");
        
        var quickButtonResult = await quickButtonsService.UpdateFromTransactionAsync(testData);
        Assert.That(quickButtonResult.IsSuccess, Is.True, "QuickButtons service should succeed");
        
        // Assert - Verify data consistency across services
        var inventoryCheck = await inventoryService.GetInventoryAsync(testData.PartId, testData.Operation);
        Assert.That(inventoryCheck.Quantity, Is.EqualTo(testData.Quantity));
        
        var transactionHistory = await transactionService.GetTransactionHistoryAsync(testData.PartId, testData.Operation);
        Assert.That(transactionHistory.Count, Is.GreaterThan(0), "Transaction should be recorded");
        
        var quickButtons = await quickButtonsService.GetUserQuickButtonsAsync("TestUser");
        Assert.That(quickButtons.Any(qb => qb.PartId == testData.PartId), Is.True, "QuickButton should be created");
    }
    
    [Test]
    public async Task [Feature]Services_ConcurrentAccess_ShouldHandleRaceConditions()
    {
        // Arrange
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        var partId = "CONCURRENT_TEST";
        var operation = "100";
        var initialQuantity = 100;
        
        // Add initial inventory
        await inventoryService.AddInventoryAsync(new InventoryItem
        {
            PartId = partId,
            Operation = operation,
            Quantity = initialQuantity,
            Location = "STATION_A"
        });
        
        // Act - Simulate concurrent access
        var concurrentTasks = new List<Task<ServiceResult>>();
        
        // Multiple users trying to remove inventory simultaneously
        for (int i = 0; i < 10; i++)
        {
            concurrentTasks.Add(inventoryService.RemoveInventoryAsync(new InventoryItem
            {
                PartId = partId,
                Operation = operation,
                Quantity = 5, // Each tries to remove 5
                Location = "STATION_A",
                UserId = $"User{i}"
            }));
        }
        
        var results = await Task.WhenAll(concurrentTasks);
        
        // Assert - Verify data integrity maintained
        var successCount = results.Count(r => r.IsSuccess);
        var finalInventory = await inventoryService.GetInventoryAsync(partId, operation);
        
        // Should have exactly (initialQuantity - (successCount * 5)) remaining
        var expectedRemaining = initialQuantity - (successCount * 5);
        Assert.That(finalInventory.Quantity, Is.EqualTo(expectedRemaining));
        Assert.That(finalInventory.Quantity, Is.GreaterThanOrEqualTo(0), "Inventory should not go negative");
    }
    
    private IConfiguration CreateTestConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", _databaseFixture.ConnectionString),
                new KeyValuePair<string, string>("Logging:LogLevel:Default", "Debug")
            })
            .Build();
            
        return configuration;
    }
}
```

## Database Integration Testing

### Database Integration Test Template
```csharp
[TestFixture]
[Category("Integration")]
[Category("Database")]
public class [Component]DatabaseIntegrationTests
{
    private string _connectionString;
    private DatabaseTestFixture _databaseFixture;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _databaseFixture = new DatabaseTestFixture();
        await _databaseFixture.SetupAsync();
        _connectionString = _databaseFixture.ConnectionString;
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _databaseFixture?.TearDownAsync();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        await _databaseFixture.CleanupTestDataAsync();
    }
    
    [Test]
    public async Task StoredProcedure_[ProcedureName]_ShouldExecuteSuccessfully()
    {
        // Arrange
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", "DB_INTEG_001"),
            new("p_OperationNumber", "100"),
            new("p_Quantity", 25),
            new("p_Location", "STATION_A"),
            new("p_User", "IntegrationTest")
        };
        
        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "[stored_procedure_name]", parameters);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(1), "Stored procedure should return success status");
        Assert.That(result.Data, Is.Not.Null, "Result data should not be null");
        
        // Verify data was actually modified
        var verifyResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "[verification_procedure]", 
            new MySqlParameter[] { new("p_PartID", "DB_INTEG_001") });
            
        Assert.That(verifyResult.Status, Is.EqualTo(1), "Verification query should succeed");
        Assert.That(verifyResult.Data.Rows.Count, Is.GreaterThan(0), "Should find inserted/modified data");
    }
    
    [Test]
    public async Task DatabaseTransaction_MultipleOperations_ShouldMaintainConsistency()
    {
        // Arrange
        var partId = "TRANS_INTEG_001";
        var operation = "100";
        
        // Act - Perform multiple related operations
        var addResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "inv_inventory_Add_Item",
            new MySqlParameter[]
            {
                new("p_PartID", partId),
                new("p_OperationNumber", operation),
                new("p_Quantity", 100),
                new("p_Location", "STATION_A"),
                new("p_User", "TransactionTest")
            });
        
        Assert.That(addResult.Status, Is.EqualTo(1), "Add operation should succeed");
        
        var removeResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "inv_inventory_Remove_Item",
            new MySqlParameter[]
            {
                new("p_PartID", partId),
                new("p_OperationNumber", operation),
                new("p_Quantity", 30),
                new("p_Location", "STATION_A"),
                new("p_User", "TransactionTest")
            });
        
        Assert.That(removeResult.Status, Is.EqualTo(1), "Remove operation should succeed");
        
        // Assert - Verify final state
        var finalResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter[]
            {
                new("p_PartID", partId),
                new("p_OperationNumber", operation)
            });
        
        Assert.That(finalResult.Status, Is.EqualTo(1), "Final check should succeed");
        var finalQuantity = Convert.ToInt32(finalResult.Data.Rows[0]["Quantity"]);
        Assert.That(finalQuantity, Is.EqualTo(70), "Final quantity should be 100 - 30 = 70");
    }
    
    [Test]
    public async Task DatabaseConnection_Resilience_ShouldRecoverFromFailures()
    {
        // Arrange - Test connection recovery
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", "RESILIENCE_TEST"),
            new("p_OperationNumber", "100")
        };
        
        // Act & Assert - Multiple attempts should eventually succeed
        var attempts = 0;
        var maxAttempts = 3;
        DatabaseResult result = null;
        
        while (attempts < maxAttempts)
        {
            try
            {
                result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString, "inv_inventory_Get_ByPartIDandOperation", parameters);
                    
                if (result.Status >= 0) break; // Success or recoverable error
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Database operation attempt {Attempt} failed", attempts + 1);
            }
            
            attempts++;
            if (attempts < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempts))); // Exponential backoff
            }
        }
        
        Assert.That(result, Is.Not.Null, "Should eventually get a result");
        Assert.That(attempts, Is.LessThan(maxAttempts), "Should succeed within retry limit");
    }
}
```

## Cross-Platform Integration Testing

### Cross-Platform Test Template
```csharp
[TestFixture]
[Category("Integration")]
[Category("CrossPlatform")]
public class [Feature]CrossPlatformIntegrationTests
{
    private static readonly PlatformInfo CurrentPlatform = PlatformDetector.GetCurrent();
    private string _testDataDirectory;
    private IConfiguration _platformConfiguration;
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        _testDataDirectory = CreatePlatformSpecificTestDirectory();
        _platformConfiguration = LoadPlatformConfiguration();
        await SetupPlatformSpecificResourcesAsync();
    }
    
    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await CleanupPlatformSpecificResourcesAsync();
        CleanupTestDirectory();
    }
    
    [Test]
    public async Task [Feature]_CrossPlatformCompatibility_ShouldWorkOnAllPlatforms()
    {
        // Arrange
        var service = CreatePlatformSpecificService();
        var testData = CreateTestData();
        
        // Act
        var result = await service.ProcessAsync(testData);
        
        // Assert - Core functionality should work on all platforms
        Assert.That(result.IsSuccess, Is.True, 
            $"Feature should work on {CurrentPlatform.OS}");
        
        // Platform-specific assertions
        if (CurrentPlatform.OS == OSPlatform.Windows)
        {
            // Windows-specific validations
            Assert.That(result.Data.WindowsSpecificProperty, Is.Not.Null);
        }
        else if (CurrentPlatform.OS == OSPlatform.macOS)
        {
            // macOS-specific validations
            Assert.That(result.Data.MacOSSpecificProperty, Is.Not.Null);
        }
        else if (CurrentPlatform.OS == OSPlatform.Linux)
        {
            // Linux-specific validations
            Assert.That(result.Data.LinuxSpecificProperty, Is.Not.Null);
        }
        else if (CurrentPlatform.OS == OSPlatform.Android)
        {
            // Android-specific validations
            Assert.That(result.Data.AndroidSpecificProperty, Is.Not.Null);
        }
    }
    
    [Test]
    public async Task FileOperations_CrossPlatform_ShouldHandlePathDifferences()
    {
        // Arrange
        var testFileName = "crossplatform_test.json";
        var testData = new { Message = "Cross-platform test data" };
        
        // Act - File operations using platform-specific paths
        var filePath = Path.Combine(_testDataDirectory, testFileName);
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(testData));
        
        // Assert - File should be created and readable
        Assert.That(File.Exists(filePath), Is.True, "File should be created successfully");
        
        var readData = await File.ReadAllTextAsync(filePath);
        var deserializedData = JsonSerializer.Deserialize<dynamic>(readData);
        
        Assert.That(deserializedData, Is.Not.Null, "File should be readable");
        
        // Cleanup
        File.Delete(filePath);
    }
    
    [Test]
    public async Task DatabaseConnectivity_CrossPlatform_ShouldWork()
    {
        // Arrange
        var connectionString = _platformConfiguration.GetConnectionString("DefaultConnection");
        
        // Act - Test database connectivity
        var testParameters = new MySqlParameter[]
        {
            new("p_Platform", CurrentPlatform.OS.ToString()),
            new("p_TestData", "Cross-platform connectivity test")
        };
        
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString, "test_connectivity", testParameters);
        
        // Assert - Database operations should work on all platforms
        Assert.That(result.Status, Is.GreaterThanOrEqualTo(0), 
            $"Database should be accessible from {CurrentPlatform.OS}");
    }
    
    private string CreatePlatformSpecificTestDirectory()
    {
        var baseDirectory = CurrentPlatform.OS switch
        {
            OSPlatform.Windows => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MTM_IntegrationTests"),
            OSPlatform.macOS => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "MTM_IntegrationTests"),
            OSPlatform.Linux => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share", "MTM_IntegrationTests"),
            OSPlatform.Android => Path.Combine("/data/data/com.mtm.wipapp/files", "tests"),
            _ => Path.Combine(Path.GetTempPath(), "MTM_IntegrationTests")
        };
        
        Directory.CreateDirectory(baseDirectory);
        return baseDirectory;
    }
}
```

## UI Integration Testing

### UI Integration Test Template
```csharp
[TestFixture]
[Category("Integration")]
[Category("UI")]
public class [View]UIIntegrationTests
{
    private TestAppBuilder _appBuilder;
    private IServiceProvider _serviceProvider;
    
    [SetUp]
    public void SetUp()
    {
        _appBuilder = AppBuilder.Configure<App>()
                               .UseHeadless(new AvaloniaHeadlessPlatformOptions
                               {
                                   UseHeadlessDrawing = true,
                                   FrameBufferFormat = PixelFormat.Rgba8888
                               })
                               .SetupWithoutStarting();
        
        // Configure test services
        var services = new ServiceCollection();
        services.AddMTMServices(CreateTestConfiguration());
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [TearDown]
    public void TearDown()
    {
        _serviceProvider?.Dispose();
    }
    
    [Test]
    public async Task [View]_ViewModelIntegration_ShouldBindCorrectly()
    {
        // Arrange
        using var app = _appBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>());
        
        var viewModel = _serviceProvider.GetRequiredService<[ViewModel]>();
        var view = new [View]
        {
            DataContext = viewModel
        };
        
        var window = new Window { Content = view };
        
        // Act
        window.Show();
        await Task.Delay(500); // Allow for binding to complete
        
        // Simulate user interaction
        viewModel.PartId = "UI_INTEG_001";
        viewModel.Operation = "100";
        viewModel.Quantity = 25;
        
        await viewModel.SaveCommand.ExecuteAsync(null);
        
        // Assert
        Assert.That(viewModel.IsLoading, Is.False, "Loading state should be reset");
        Assert.That(string.IsNullOrEmpty(viewModel.ErrorMessage), Is.True, "Should not have error message");
        Assert.That(viewModel.StatusMessage, Does.Contain("success").IgnoreCase, "Should show success message");
    }
    
    [Test]
    public async Task [View]_FormValidation_ShouldPreventInvalidSubmission()
    {
        // Arrange
        using var app = _appBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>());
        
        var viewModel = _serviceProvider.GetRequiredService<[ViewModel]>();
        var view = new [View] { DataContext = viewModel };
        var window = new Window { Content = view };
        
        // Act
        window.Show();
        await Task.Delay(500);
        
        // Try to save with invalid data
        viewModel.PartId = ""; // Invalid empty part ID
        viewModel.Quantity = -1; // Invalid negative quantity
        
        // Assert
        Assert.That(viewModel.SaveCommand.CanExecute(null), Is.False, 
            "Save command should be disabled with invalid data");
        
        // Fix validation and try again
        viewModel.PartId = "VALID_PART";
        viewModel.Operation = "100";
        viewModel.Quantity = 10;
        
        Assert.That(viewModel.SaveCommand.CanExecute(null), Is.True,
            "Save command should be enabled with valid data");
    }
    
    private IConfiguration CreateTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("ConnectionStrings:DefaultConnection", "TestConnectionString"),
                new KeyValuePair<string, string>("Logging:LogLevel:Default", "Debug")
            })
            .Build();
    }
}
```

## Performance Integration Testing

### Performance Integration Test Template
```csharp
[TestFixture]
[Category("Integration")]
[Category("Performance")]
public class [Feature]PerformanceIntegrationTests
{
    [Test]
    public async Task [Feature]_HighVolumeOperations_ShouldMaintainPerformance()
    {
        // Arrange
        var service = CreateServiceWithRealDependencies();
        var testDataVolume = 1000;
        var testData = GenerateTestData(testDataVolume);
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var errorCount = 0;
        
        // Act - Execute high-volume operations
        var tasks = testData.Select(async data =>
        {
            try
            {
                var result = await service.ProcessAsync(data);
                if (result.IsSuccess)
                    Interlocked.Increment(ref successCount);
                else
                    Interlocked.Increment(ref errorCount);
                    
                return result.IsSuccess;
            }
            catch
            {
                Interlocked.Increment(ref errorCount);
                return false;
            }
        });
        
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // Assert - Performance requirements
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(30000), 
            $"1000 operations should complete within 30 seconds, took {stopwatch.ElapsedMilliseconds}ms");
        Assert.That(successCount, Is.GreaterThanOrEqualTo(950), 
            $"At least 95% should succeed, got {successCount}/{testDataVolume}");
        
        // Log performance metrics
        Console.WriteLine($"Performance Results:");
        Console.WriteLine($"  Operations: {testDataVolume}");
        Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Average per Operation: {stopwatch.ElapsedMilliseconds / (double)testDataVolume:F2}ms");
        Console.WriteLine($"  Success Rate: {successCount}/{testDataVolume} ({successCount * 100.0 / testDataVolume:F1}%)");
    }
    
    [Test]
    public async Task [Feature]_MemoryUsage_ShouldNotLeak()
    {
        // Arrange
        var service = CreateServiceWithRealDependencies();
        var initialMemory = GC.GetTotalMemory(true);
        
        // Act - Perform operations that might cause memory leaks
        for (int i = 0; i < 100; i++)
        {
            var testData = CreateTestData($"MEMORY_TEST_{i}");
            await service.ProcessAsync(testData);
            
            // Force garbage collection periodically
            if (i % 10 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        
        // Final cleanup
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var finalMemory = GC.GetTotalMemory(false);
        var memoryIncrease = finalMemory - initialMemory;
        
        // Assert - Memory should not increase significantly
        Assert.That(memoryIncrease, Is.LessThan(50_000_000), 
            $"Memory increase should be less than 50MB, was {memoryIncrease / 1024.0 / 1024.0:F2}MB");
    }
}
```

## Integration Test Infrastructure

### Database Test Fixture
```csharp
public class DatabaseTestFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    private readonly string _testDatabaseName;
    
    public DatabaseTestFixture()
    {
        _testDatabaseName = $"mtm_integ_test_{Guid.NewGuid():N}";
        ConnectionString = CreateTestConnectionString();
    }
    
    public async Task SetupAsync()
    {
        await CreateTestDatabaseAsync();
        await CreateTestSchemaAsync();
        await SeedTestDataAsync();
    }
    
    public async Task TearDownAsync()
    {
        await DropTestDatabaseAsync();
    }
    
    public async Task CleanupTestDataAsync()
    {
        // Clean up test data without dropping schema
        var cleanupQueries = new[]
        {
            "DELETE FROM inv_transactions WHERE User LIKE '%Test%'",
            "DELETE FROM inv_inventory WHERE User LIKE '%Test%'",
            "DELETE FROM qb_quickbuttons WHERE User LIKE '%Test%'"
        };
        
        foreach (var query in cleanupQueries)
        {
            try
            {
                await ExecuteNonQueryAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup warning: {ex.Message}");
            }
        }
    }
    
    // Implementation details...
}
```

This integration testing template provides comprehensive coverage for cross-service communication, database integration, cross-platform compatibility, and performance validation in the MTM WIP Application.