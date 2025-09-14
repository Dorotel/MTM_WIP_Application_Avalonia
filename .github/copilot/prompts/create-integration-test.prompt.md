---
description: 'Prompt template for creating integration tests for MTM WIP Application service interactions and database operations'
applies_to: '**/*'
---

# Create Integration Test Prompt Template

## 🎯 Objective

Generate comprehensive integration tests for MTM WIP Application that validate service interactions, database operations, and end-to-end workflows. Focus on real component integration while maintaining test isolation and reliability.

## 📋 Instructions

When creating integration tests, follow these specific requirements:

### Integration Test Structure

1. **Use MTM Integration Test Base Class**
   ```csharp
   [TestFixture]
   [Category("Integration")]
   [Category("{IntegrationCategory}")]  // e.g., Database, ServiceIntegration, Workflow
   public class {ComponentName}IntegrationTests : IntegrationTestBase
   {
       // Test implementation with real service interactions
   }
   ```

2. **Integration Test Categories**
   - Database Integration: Real database operations with test database
   - Service Integration: Multiple service interactions
   - Workflow Integration: Complete business process flows
   - API Integration: External system interactions
   - File System Integration: Configuration and logging operations

### Database Integration Tests

#### Stored Procedure Integration
```csharp
[TestFixture]
[Category("Integration")]
[Category("Database")]
public class InventoryDatabaseIntegrationTests : DatabaseIntegrationTestBase
{
    [Test]
    public async Task AddInventory_CompleteWorkflow_ShouldUpdateAllTables()
    {
        // Arrange
        var inventoryData = new InventoryItem
        {
            PartId = $"INT_TEST_{Guid.NewGuid():N[..8]}",
            Operation = "100",
            Quantity = 25,
            Location = "INTEGRATION_STATION",
            User = "IntegrationTestUser"
        };
        
        // Act - Execute complete workflow
        var addResult = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", inventoryData.PartId),
            new MySqlParameter("p_OperationNumber", inventoryData.Operation),
            new MySqlParameter("p_Quantity", inventoryData.Quantity),
            new MySqlParameter("p_Location", inventoryData.Location),
            new MySqlParameter("p_User", inventoryData.User));
        
        // Assert inventory was added
        Assert.That(addResult.Status, Is.EqualTo(1), "Inventory should be added successfully");
        
        // Verify in inventory table
        var inventoryResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", inventoryData.PartId),
            new MySqlParameter("p_OperationNumber", inventoryData.Operation));
        
        Assert.That(inventoryResult.Status, Is.EqualTo(1));
        Assert.That(inventoryResult.Data.Rows.Count, Is.EqualTo(1));
        
        var inventoryRow = inventoryResult.Data.Rows[0];
        Assert.That(inventoryRow["PartID"].ToString(), Is.EqualTo(inventoryData.PartId));
        Assert.That(Convert.ToInt32(inventoryRow["Quantity"]), Is.EqualTo(inventoryData.Quantity));
        
        // Verify transaction was recorded
        var transactionResult = await ExecuteStoredProcedureAsync("inv_transaction_Get_History",
            new MySqlParameter("p_PartID", inventoryData.PartId),
            new MySqlParameter("p_OperationNumber", inventoryData.Operation));
        
        Assert.That(transactionResult.Status, Is.EqualTo(1));
        Assert.That(transactionResult.Data.Rows.Count, Is.GreaterThan(0), "Transaction should be recorded");
        
        var transactionRow = transactionResult.Data.Rows[0];
        Assert.That(transactionRow["TransactionType"].ToString(), Is.EqualTo("IN"));
        Assert.That(Convert.ToInt32(transactionRow["Quantity"]), Is.EqualTo(inventoryData.Quantity));
    }
    
    [Test]
    public async Task MasterData_ReferentialIntegrity_ShouldMaintainConstraints()
    {
        // Arrange - Add master data first
        var partId = $"INTEGRITY_TEST_{Guid.NewGuid():N[..8]}";
        var location = "INTEGRITY_LOCATION";
        var operation = "INTEGRITY_OP";
        
        // Add master data
        await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_Description", "Integration test part"),
            new MySqlParameter("p_User", "IntegrationTest"));
        
        await ExecuteStoredProcedureAsync("md_locations_Add",
            new MySqlParameter("p_Location", location),
            new MySqlParameter("p_IsActive", true),
            new MySqlParameter("p_User", "IntegrationTest"));
        
        await ExecuteStoredProcedureAsync("md_operation_numbers_Add",
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Description", "Integration test operation"),
            new MySqlParameter("p_SequenceOrder", 999),
            new MySqlParameter("p_User", "IntegrationTest"));
        
        // Act - Use master data in inventory operation
        var inventoryResult = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", 10),
            new MySqlParameter("p_Location", location),
            new MySqlParameter("p_User", "IntegrationTest"));
        
        // Assert - Should succeed with valid master data
        Assert.That(inventoryResult.Status, Is.EqualTo(1), "Inventory addition should succeed with valid master data");
        
        // Test with invalid master data
        var invalidResult = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", "INVALID_PART_ID"),
            new MySqlParameter("p_OperationNumber", "INVALID_OP"),
            new MySqlParameter("p_Quantity", 10),
            new MySqlParameter("p_Location", "INVALID_LOCATION"),
            new MySqlParameter("p_User", "IntegrationTest"));
        
        // Assert - Should fail with invalid master data
        Assert.That(invalidResult.Status, Is.LessThan(1), "Should reject invalid master data");
    }
    
    [Test]
    public async Task ConcurrentOperations_SameInventory_ShouldMaintainConsistency()
    {
        // Arrange
        var partId = $"CONCURRENT_TEST_{Guid.NewGuid():N[..8]}";
        var operation = "100";
        var concurrentUsers = 5;
        var quantityPerUser = 10;
        
        // Act - Execute concurrent inventory additions
        var tasks = Enumerable.Range(1, concurrentUsers)
            .Select(async userIndex =>
            {
                var userName = $"ConcurrentUser{userIndex}";
                return await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                    new MySqlParameter("p_PartID", partId),
                    new MySqlParameter("p_OperationNumber", operation),
                    new MySqlParameter("p_Quantity", quantityPerUser),
                    new MySqlParameter("p_Location", "CONCURRENT_STATION"),
                    new MySqlParameter("p_User", userName));
            })
            .ToArray();
        
        var results = await Task.WhenAll(tasks);
        
        // Assert - All operations should succeed
        foreach (var result in results)
        {
            Assert.That(result.Status, Is.EqualTo(1), "Concurrent operations should succeed");
        }
        
        // Verify final quantity is sum of all additions
        var finalResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
        
        var finalQuantity = Convert.ToInt32(finalResult.Data.Rows[0]["Quantity"]);
        var expectedQuantity = concurrentUsers * quantityPerUser;
        
        Assert.That(finalQuantity, Is.EqualTo(expectedQuantity),
            $"Final quantity should be {expectedQuantity}, got {finalQuantity}");
        
        // Verify all transactions were recorded
        var transactionResult = await ExecuteStoredProcedureAsync("inv_transaction_Get_History",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
        
        Assert.That(transactionResult.Data.Rows.Count, Is.EqualTo(concurrentUsers),
            "Should have transaction record for each concurrent operation");
    }
}
```

### Service Integration Tests

#### Multi-Service Workflow Integration
```csharp
[TestFixture]
[Category("Integration")]
[Category("ServiceIntegration")]
public class ManufacturingWorkflowIntegrationTests : ServiceIntegrationTestBase
{
    private IInventoryService _inventoryService;
    private ITransactionService _transactionService;
    private IQuickButtonsService _quickButtonsService;
    private IConfigurationService _configurationService;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        // Setup real services with test configuration
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMTMServices(TestConfiguration);
        
        var serviceProvider = services.BuildServiceProvider();
        
        _inventoryService = serviceProvider.GetRequiredService<IInventoryService>();
        _transactionService = serviceProvider.GetRequiredService<ITransactionService>();
        _quickButtonsService = serviceProvider.GetRequiredService<IQuickButtonsService>();
        _configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
        
        await base.OneTimeSetUp();
    }
    
    [Test]
    public async Task CompleteInventoryWorkflow_AddTransactionQuickButton_ShouldIntegrateCorrectly()
    {
        // Arrange
        var testUser = "WorkflowIntegrationUser";
        var inventoryItem = new InventoryItem
        {
            PartId = $"WORKFLOW_TEST_{Guid.NewGuid():N[..8]}",
            Operation = "100",
            Quantity = 25,
            Location = "WORKFLOW_STATION",
            User = testUser
        };
        
        // Act - Step 1: Add inventory through service
        var addResult = await _inventoryService.AddInventoryAsync(inventoryItem);
        
        // Assert Step 1
        Assert.That(addResult.Success, Is.True, "Inventory addition should succeed");
        Assert.That(addResult.Message, Is.Not.Null.And.Not.Empty, "Should provide feedback");
        
        // Act - Step 2: Verify transaction was created
        var transactions = await _transactionService.GetTransactionHistoryAsync(
            inventoryItem.PartId, inventoryItem.Operation);
        
        // Assert Step 2
        Assert.That(transactions, Is.Not.Null.And.Count.GreaterThan(0), 
            "Transaction should be recorded");
        
        var transaction = transactions.First();
        Assert.That(transaction.PartId, Is.EqualTo(inventoryItem.PartId));
        Assert.That(transaction.Operation, Is.EqualTo(inventoryItem.Operation));
        Assert.That(transaction.Quantity, Is.EqualTo(inventoryItem.Quantity));
        Assert.That(transaction.TransactionType, Is.EqualTo("IN"));
        Assert.That(transaction.User, Is.EqualTo(testUser));
        
        // Act - Step 3: Create QuickButton from transaction
        var quickButtonInfo = new QuickButtonInfo
        {
            PartId = inventoryItem.PartId,
            Operation = inventoryItem.Operation,
            Quantity = inventoryItem.Quantity,
            Location = inventoryItem.Location,
            User = testUser,
            DisplayText = $"{inventoryItem.PartId} ({inventoryItem.Quantity})"
        };
        
        var quickButtonResult = await _quickButtonsService.SaveQuickButtonAsync(quickButtonInfo);
        
        // Assert Step 3
        Assert.That(quickButtonResult.Success, Is.True, "QuickButton creation should succeed");
        
        // Act - Step 4: Verify QuickButton can be retrieved
        var savedQuickButtons = await _quickButtonsService.GetQuickButtonsAsync(testUser);
        
        // Assert Step 4
        Assert.That(savedQuickButtons, Is.Not.Null.And.Count.GreaterThan(0),
            "QuickButton should be retrievable");
        
        var savedButton = savedQuickButtons.FirstOrDefault(qb => qb.PartId == inventoryItem.PartId);
        Assert.That(savedButton, Is.Not.Null, "Saved QuickButton should be found");
        Assert.That(savedButton.DisplayText, Is.EqualTo(quickButtonInfo.DisplayText));
        
        // Act - Step 5: Execute QuickButton (add more inventory)
        var executeResult = await _quickButtonsService.ExecuteQuickButtonAsync(savedButton);
        
        // Assert Step 5
        Assert.That(executeResult.Success, Is.True, "QuickButton execution should succeed");
        
        // Final verification - inventory should show doubled quantity
        var finalInventory = await _inventoryService.GetInventoryAsync(
            inventoryItem.PartId, inventoryItem.Operation);
        
        Assert.That(finalInventory, Is.Not.Null.And.Count.EqualTo(1));
        var finalItem = finalInventory.First();
        Assert.That(finalItem.Quantity, Is.EqualTo(inventoryItem.Quantity * 2),
            "Quantity should be doubled after QuickButton execution");
        
        // Verify transaction history shows both operations
        var finalTransactions = await _transactionService.GetTransactionHistoryAsync(
            inventoryItem.PartId, inventoryItem.Operation);
        
        Assert.That(finalTransactions.Count, Is.EqualTo(2),
            "Should have two transactions: original add and QuickButton execution");
    }
    
    [Test]
    public async Task ServiceConfiguration_Integration_ShouldLoadAndUseCorrectly()
    {
        // Arrange & Act
        var connectionString = await _configurationService.GetConnectionStringAsync();
        var appSettings = await _configurationService.GetAllSettingsAsync();
        
        // Assert configuration is loaded
        Assert.That(connectionString, Is.Not.Null.And.Not.Empty,
            "Connection string should be loaded");
        Assert.That(appSettings, Is.Not.Null.And.Count.GreaterThan(0),
            "App settings should be loaded");
        
        // Test configuration usage in services
        var partIds = await _inventoryService.GetAllPartIdsAsync();
        
        // This should work if configuration is correct
        Assert.That(() => partIds, Throws.Nothing,
            "Services should use configuration correctly");
        
        // Test that services use the same configuration
        var configFromInventory = await _inventoryService.GetConnectionStringAsync();
        var configFromTransaction = await _transactionService.GetConnectionStringAsync();
        
        Assert.That(configFromInventory, Is.EqualTo(connectionString),
            "All services should use the same connection string");
        Assert.That(configFromTransaction, Is.EqualTo(connectionString),
            "All services should use the same connection string");
    }
    
    [Test]
    public async Task ErrorHandling_ServiceFailure_ShouldPropagateCorrectly()
    {
        // Arrange - Force service failure by using invalid data
        var invalidInventoryItem = new InventoryItem
        {
            PartId = "", // Invalid - empty part ID
            Operation = "",
            Quantity = -1, // Invalid - negative quantity
            Location = "",
            User = ""
        };
        
        // Act & Assert - Error should be handled gracefully
        var result = await _inventoryService.AddInventoryAsync(invalidInventoryItem);
        
        Assert.That(result.Success, Is.False, "Invalid data should not succeed");
        Assert.That(result.Message, Does.Contain("error").Or.Contain("invalid").IgnoreCase,
            "Error message should indicate the problem");
        
        // Verify no partial data was created
        var searchResult = await _inventoryService.GetInventoryAsync(
            invalidInventoryItem.PartId, invalidInventoryItem.Operation);
        Assert.That(searchResult, Is.Empty, "No inventory should be created for invalid data");
        
        // Verify no transaction was recorded
        var transactionResult = await _transactionService.GetTransactionHistoryAsync(
            invalidInventoryItem.PartId, invalidInventoryItem.Operation);
        Assert.That(transactionResult, Is.Empty, "No transaction should be recorded for failed operation");
    }
}
```

### File System Integration Tests

```csharp
[TestFixture]
[Category("Integration")]
[Category("FileSystem")]
public class ConfigurationFileIntegrationTests : IntegrationTestBase
{
    private string _testConfigDirectory;
    private IConfigurationService _configurationService;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _testConfigDirectory = Path.Combine(Path.GetTempPath(), $"MTM_Config_Test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testConfigDirectory);
        
        // Create test configuration files
        await CreateTestConfigurationFilesAsync();
        
        // Setup configuration service
        var services = new ServiceCollection();
        services.AddLogging();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(_testConfigDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        services.AddScoped<IConfigurationService, ConfigurationService>();
        
        var serviceProvider = services.BuildServiceProvider();
        _configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
        
        await base.OneTimeSetUp();
    }
    
    [Test]
    public async Task ConfigurationService_LoadFromFile_ShouldReadCorrectly()
    {
        // Act
        var connectionString = await _configurationService.GetConnectionStringAsync();
        var appName = await _configurationService.GetSettingAsync<string>("MTM_Configuration:AppName");
        var logLevel = await _configurationService.GetSettingAsync<string>("Logging:LogLevel:Default");
        
        // Assert
        Assert.That(connectionString, Does.Contain("Server=localhost"),
            "Connection string should be loaded from file");
        Assert.That(appName, Is.EqualTo("MTM WIP Application Integration Test"),
            "App name should be loaded from file");
        Assert.That(logLevel, Is.EqualTo("Debug"),
            "Log level should be loaded from file");
    }
    
    [Test]
    public async Task ConfigurationService_UpdateSetting_ShouldPersistToFile()
    {
        // Arrange
        var newLogLevel = "Information";
        
        // Act
        await _configurationService.UpdateSettingAsync("Logging:LogLevel:Default", newLogLevel);
        
        // Assert - Read directly from file to verify persistence
        var configJson = await File.ReadAllTextAsync(Path.Combine(_testConfigDirectory, "appsettings.json"));
        var configDoc = JsonDocument.Parse(configJson);
        
        var actualLogLevel = configDoc.RootElement
            .GetProperty("Logging")
            .GetProperty("LogLevel")
            .GetProperty("Default")
            .GetString();
        
        Assert.That(actualLogLevel, Is.EqualTo(newLogLevel),
            "Setting should be persisted to file");
        
        // Also verify through service
        var serviceLogLevel = await _configurationService.GetSettingAsync<string>("Logging:LogLevel:Default");
        Assert.That(serviceLogLevel, Is.EqualTo(newLogLevel),
            "Service should return updated setting");
    }
    
    private async Task CreateTestConfigurationFilesAsync()
    {
        var testConfig = new
        {
            ConnectionStrings = new
            {
                DefaultConnection = "Server=localhost;Database=mtm_integration_test;Uid=test_user;Pwd=test_password;"
            },
            MTM_Configuration = new
            {
                AppName = "MTM WIP Application Integration Test",
                Version = "1.0.0-integration",
                Environment = "Integration"
            },
            Logging = new
            {
                LogLevel = new
                {
                    Default = "Debug",
                    MTM_WIP_Application_Avalonia = "Trace"
                }
            }
        };
        
        var configJson = JsonSerializer.Serialize(testConfig, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await File.WriteAllTextAsync(
            Path.Combine(_testConfigDirectory, "appsettings.json"), 
            configJson);
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        try
        {
            if (Directory.Exists(_testConfigDirectory))
            {
                Directory.Delete(_testConfigDirectory, recursive: true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not cleanup test directory: {ex.Message}");
        }
        
        await base.OneTimeTearDown();
    }
}
```

### Performance Integration Tests

```csharp
[TestFixture]
[Category("Integration")]
[Category("Performance")]
public class PerformanceIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task HighVolumeInventoryOperations_Performance_ShouldMeetThresholds()
    {
        // Arrange
        var operationCount = 1000;
        var inventoryItems = Enumerable.Range(1, operationCount)
            .Select(i => new InventoryItem
            {
                PartId = $"PERF_PART_{i:0000}",
                Operation = (i % 4) switch { 0 => "90", 1 => "100", 2 => "110", _ => "120" },
                Quantity = 10 + (i % 50),
                Location = $"PERF_STATION_{(i % 5) + 1}",
                User = "PerformanceTestUser"
            })
            .ToList();
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        
        // Act - Execute high volume operations
        var tasks = inventoryItems.Select(async item =>
        {
            var result = await _inventoryService.AddInventoryAsync(item);
            if (result.Success)
                Interlocked.Increment(ref successCount);
            return result.Success;
        });
        
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // Assert performance thresholds
        var totalTime = stopwatch.ElapsedMilliseconds;
        var averageTime = totalTime / (double)operationCount;
        var successRate = successCount / (double)operationCount;
        
        Assert.That(totalTime, Is.LessThan(60000), // 60 seconds max
            $"1000 operations should complete within 60 seconds, took {totalTime}ms");
        Assert.That(averageTime, Is.LessThan(60), // 60ms average per operation
            $"Average operation time should be under 60ms, got {averageTime:F2}ms");
        Assert.That(successRate, Is.GreaterThanOrEqualTo(0.95), // 95% success rate
            $"Success rate should be at least 95%, got {successRate:P}");
        
        Console.WriteLine($"Performance Integration Results:");
        Console.WriteLine($"  Total Operations: {operationCount}");
        Console.WriteLine($"  Total Time: {totalTime}ms ({totalTime / 1000.0:F1}s)");
        Console.WriteLine($"  Average Time: {averageTime:F2}ms per operation");
        Console.WriteLine($"  Success Rate: {successRate:P} ({successCount}/{operationCount})");
    }
}
```

## ✅ Integration Test Checklist

When creating integration tests, ensure:

- [ ] Tests use real implementations (not mocks) where possible
- [ ] Database tests use isolated test database
- [ ] File system tests use temporary directories
- [ ] Cross-service interactions are validated
- [ ] Transaction integrity is verified
- [ ] Error propagation between services is tested
- [ ] Performance under realistic load is measured
- [ ] Configuration integration is validated
- [ ] Cleanup is performed after each test
- [ ] Tests are isolated and can run in parallel
- [ ] Real-world scenarios are covered
- [ ] Integration points between major components are tested

This template ensures comprehensive integration test coverage while maintaining reliability and performance of the MTM WIP Application integration testing suite.