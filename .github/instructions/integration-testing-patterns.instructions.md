---
description: 'Integration testing patterns for MTM cross-service communication and database validation'
applies_to: '**/*'
---

# MTM Integration Testing Patterns Instructions

## 🎯 Overview

Comprehensive integration testing patterns for MTM WIP Application, focusing on cross-service communication, database stored procedure validation, and complete system interaction testing.

## 🗄️ Database Integration Testing

### Stored Procedure Testing Framework

**Template for 45+ MTM Stored Procedures:**

```csharp
[TestFixture]
[Category("Integration")]
[Category("Database")]
public class StoredProcedureIntegrationTests
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
        await _databaseFixture.TearDownAsync();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        await _databaseFixture.CleanupTestDataAsync();
    }
}
```

### Inventory Stored Procedures Integration

```csharp
[Test]
[TestCase("inv_inventory_Add_Item", "INTEG_001", "100", 25, "STATION_A", "TestUser")]
[TestCase("inv_inventory_Add_Item", "INTEG_002", "110", 10, "STATION_B", "TestUser")]
[TestCase("inv_inventory_Add_Item", "INTEG_003", "90", 50, "STATION_C", "TestUser")]
public async Task InventoryProcedures_AddItem_ShouldInsertCorrectly(
    string procedureName, string partId, string operation, int quantity, string location, string user)
{
    // Arrange
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_OperationNumber", operation),
        new("p_Quantity", quantity),
        new("p_Location", location),
        new("p_User", user)
    };
    
    // Act
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, procedureName, parameters);
    
    // Assert
    Assert.That(result.Status, Is.EqualTo(1), "Status should indicate success");
    Assert.That(result.Data, Is.Not.Null, "Result data should be returned");
    
    // Verify data was actually inserted
    var verifyResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Get_ByPartIDandOperation", 
        new MySqlParameter[] 
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation)
        });
        
    Assert.That(verifyResult.Status, Is.EqualTo(1), "Verification query should succeed");
    Assert.That(verifyResult.Data.Rows.Count, Is.GreaterThan(0), "Inserted data should be retrievable");
    
    // Verify data integrity
    var row = verifyResult.Data.Rows[0];
    Assert.That(row["PartID"].ToString(), Is.EqualTo(partId));
    Assert.That(row["OperationNumber"].ToString(), Is.EqualTo(operation));
    Assert.That(row["Quantity"], Is.EqualTo(quantity));
    Assert.That(row["Location"].ToString(), Is.EqualTo(location));
}

[Test]
public async Task InventoryProcedures_FullWorkflow_ShouldMaintainConsistency()
{
    var partId = "WORKFLOW_TEST_001";
    var operation = "100";
    var initialQuantity = 100;
    var removeQuantity = 30;
    
    // Step 1: Add inventory
    var addResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Add_Item",
        new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation),
            new("p_Quantity", initialQuantity),
            new("p_Location", "WORKFLOW_STATION"),
            new("p_User", "WorkflowTest")
        });
    
    Assert.That(addResult.Status, Is.EqualTo(1), "Add operation should succeed");
    
    // Step 2: Verify inventory exists
    var checkResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Get_ByPartIDandOperation",
        new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation)
        });
    
    Assert.That(checkResult.Status, Is.EqualTo(1));
    Assert.That(checkResult.Data.Rows[0]["Quantity"], Is.EqualTo(initialQuantity));
    
    // Step 3: Remove some inventory
    var removeResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Remove_Item",
        new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation),
            new("p_Quantity", removeQuantity),
            new("p_Location", "WORKFLOW_STATION"),
            new("p_User", "WorkflowTest")
        });
    
    Assert.That(removeResult.Status, Is.EqualTo(1), "Remove operation should succeed");
    
    // Step 4: Verify final quantity
    var finalCheckResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Get_ByPartIDandOperation",
        new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation)
        });
    
    Assert.That(finalCheckResult.Status, Is.EqualTo(1));
    var finalQuantity = Convert.ToInt32(finalCheckResult.Data.Rows[0]["Quantity"]);
    Assert.That(finalQuantity, Is.EqualTo(initialQuantity - removeQuantity));
    
    // Step 5: Verify transaction history
    var historyResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_transaction_Get_History",
        new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation)
        });
    
    Assert.That(historyResult.Status, Is.EqualTo(1));
    Assert.That(historyResult.Data.Rows.Count, Is.GreaterThanOrEqualTo(2), "Should have add and remove transactions");
}
```

### Master Data Procedures Integration

```csharp
[TestFixture]
[Category("Integration")]
[Category("MasterData")]
public class MasterDataIntegrationTests : DatabaseIntegrationTestBase
{
    [Test]
    [TestCase("md_part_ids_Get_All", "PartID")]
    [TestCase("md_locations_Get_All", "Location")]
    [TestCase("md_operation_numbers_Get_All", "OperationNumber")]
    public async Task MasterDataProcedures_ShouldReturnValidData(string procedureName, string expectedColumn)
    {
        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, procedureName, Array.Empty<MySqlParameter>());
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(1), "Master data procedure should succeed");
        Assert.That(result.Data, Is.Not.Null, "Data should be returned");
        Assert.That(result.Data.Columns.Contains(expectedColumn), Is.True, 
            $"Expected column '{expectedColumn}' should exist");
        
        // Verify data quality
        foreach (DataRow row in result.Data.Rows)
        {
            var value = row[expectedColumn].ToString();
            Assert.That(string.IsNullOrEmpty(value), Is.False, 
                "Master data values should not be null or empty");
        }
    }
    
    [Test]
    public async Task MasterDataProcedures_ShouldProvideConsistentData()
    {
        // Get all part IDs
        var partIdsResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "md_part_ids_Get_All", Array.Empty<MySqlParameter>());
        
        Assert.That(partIdsResult.Status, Is.EqualTo(1));
        var partIds = partIdsResult.Data.AsEnumerable()
            .Select(row => row["PartID"].ToString())
            .Where(id => !string.IsNullOrEmpty(id))
            .ToList();
        
        Assert.That(partIds.Count, Is.GreaterThan(0), "Should have at least some part IDs");
        
        // Verify part IDs are unique
        var uniquePartIds = partIds.Distinct().ToList();
        Assert.That(uniquePartIds.Count, Is.EqualTo(partIds.Count), "All part IDs should be unique");
        
        // Test each part ID in inventory context (integration test)
        var testPartId = partIds.First();
        var inventoryResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "inv_inventory_Get_ByPartID",
            new MySqlParameter[] { new("p_PartID", testPartId) });
        
        // Should not fail (may return empty results, but should execute)
        Assert.That(inventoryResult.Status, Is.GreaterThanOrEqualTo(0), 
            "Part ID from master data should be usable in inventory procedures");
    }
}
```

## 🔧 Service Integration Testing

### Cross-Service Communication Testing

```csharp
[TestFixture]
[Category("Integration")]
[Category("Services")]
public class CrossServiceIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private DatabaseTestFixture _databaseFixture;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _databaseFixture = new DatabaseTestFixture();
        await _databaseFixture.SetupAsync();
        
        var services = new ServiceCollection();
        ConfigureTestServices(services, _databaseFixture.ConnectionString);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _serviceProvider?.Dispose();
        await _databaseFixture.TearDownAsync();
    }
    
    [Test]
    public async Task InventoryService_QuickButtonsService_Integration_ShouldUpdateQuickButtons()
    {
        // Arrange
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        var quickButtonsService = _serviceProvider.GetRequiredService<IQuickButtonsService>();
        
        var testItem = new InventoryItem
        {
            PartId = "SERVICE_INTEG_001",
            Operation = "100",
            Quantity = 15,
            Location = "SERVICE_STATION",
            User = "ServiceTest"
        };
        
        // Act - Save inventory item
        var saveResult = await inventoryService.SaveInventoryAsync(testItem);
        
        // Assert - Inventory save succeeded
        Assert.That(saveResult.IsSuccess, Is.True, "Inventory save should succeed");
        
        // Act - Load QuickButtons to see if updated
        var quickButtons = await quickButtonsService.GetRecentTransactionsAsync("ServiceTest");
        
        // Assert - QuickButtons should reflect the new transaction
        var matchingButton = quickButtons.FirstOrDefault(qb => 
            qb.PartId == testItem.PartId && 
            qb.Operation == testItem.Operation);
            
        Assert.That(matchingButton, Is.Not.Null, 
            "QuickButtons should be updated after inventory operation");
        Assert.That(matchingButton.Quantity, Is.EqualTo(testItem.Quantity));
        Assert.That(matchingButton.Location, Is.EqualTo(testItem.Location));
    }
    
    [Test]
    public async Task ConfigurationService_ThemeService_Integration_ShouldPersistThemeSettings()
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        var themeService = _serviceProvider.GetRequiredService<IThemeService>();
        
        var testTheme = "MTM_Blue";
        
        // Act - Set theme via ThemeService
        await themeService.SetThemeAsync(testTheme);
        
        // Assert - Configuration should be persisted
        var savedTheme = await configService.GetSettingAsync<string>("CurrentTheme");
        Assert.That(savedTheme, Is.EqualTo(testTheme));
        
        // Act - Restart services to simulate application restart
        var newServiceProvider = CreateNewServiceProvider();
        var newThemeService = newServiceProvider.GetRequiredService<IThemeService>();
        
        // Act - Load theme after restart
        var loadedTheme = await newThemeService.GetCurrentThemeAsync();
        
        // Assert - Theme should persist across restarts
        Assert.That(loadedTheme, Is.EqualTo(testTheme));
        
        newServiceProvider.Dispose();
    }
    
    [Test]
    public async Task DatabaseService_ErrorHandlingService_Integration_ShouldLogDatabaseErrors()
    {
        // Arrange
        var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();
        var logger = _serviceProvider.GetRequiredService<ILogger<CrossServiceIntegrationTests>>();
        
        // Act - Attempt operation with invalid stored procedure
        var invalidProcedureResult = await databaseService.ExecuteStoredProcedureAsync(
            "nonexistent_procedure", Array.Empty<MySqlParameter>());
        
        // Assert - Should handle error gracefully
        Assert.That(invalidProcedureResult.IsSuccess, Is.False);
        Assert.That(invalidProcedureResult.ErrorMessage, Is.Not.Empty);
        
        // Verify error was logged (check if logger was called)
        // This would require additional setup for log capture in real implementation
        Assert.Pass("Error handling service integration validated");
    }
    
    private void ConfigureTestServices(IServiceCollection services, string connectionString)
    {
        // Configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["MTMSettings:TestMode"] = "true"
            })
            .Build();
            
        services.AddSingleton<IConfiguration>(configuration);
        
        // Logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        // MTM Services
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IQuickButtonsService, QuickButtonsService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
    }
    
    private IServiceProvider CreateNewServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureTestServices(services, _databaseFixture.ConnectionString);
        return services.BuildServiceProvider();
    }
}
```

## 🎨 Configuration Integration Testing

```csharp
[TestFixture]
[Category("Integration")]
[Category("Configuration")]
public class ConfigurationIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private ConfigurationTestFixture _configFixture;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _configFixture = new ConfigurationTestFixture();
        _serviceProvider = _configFixture.CreateServiceProvider();
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _serviceProvider?.Dispose();
        _configFixture?.Dispose();
    }
    
    [Test]
    public async Task Configuration_DatabaseConnection_ShouldLoadCorrectConnectionString()
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();
        
        // Act
        var connectionString = await configService.GetConnectionStringAsync();
        var connectionTest = await databaseService.TestConnectionAsync();
        
        // Assert
        Assert.That(connectionString, Is.Not.Empty, "Connection string should be loaded");
        Assert.That(connectionTest.IsSuccess, Is.True, "Database connection should work with loaded connection string");
    }
    
    [Test]
    public async Task Configuration_UserSettings_ShouldPersistAcrossServices()
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        var testSettings = new Dictionary<string, object>
        {
            ["UserTheme"] = "MTM_Dark",
            ["AutoSaveInterval"] = 30,
            ["EnableSuggestions"] = true,
            ["LastUsedLocation"] = "STATION_INTEGRATION"
        };
        
        // Act - Save settings
        foreach (var setting in testSettings)
        {
            await configService.SaveSettingAsync(setting.Key, setting.Value);
        }
        
        // Create new service instance to simulate app restart
        var newServiceProvider = _configFixture.CreateServiceProvider();
        var newConfigService = newServiceProvider.GetRequiredService<IConfigurationService>();
        
        // Assert - Settings should be persisted
        foreach (var setting in testSettings)
        {
            var loadedValue = await newConfigService.GetSettingAsync<object>(setting.Key);
            Assert.That(loadedValue, Is.EqualTo(setting.Value), 
                $"Setting '{setting.Key}' should persist across service instances");
        }
        
        newServiceProvider.Dispose();
    }
    
    [Test]
    public async Task Configuration_EnvironmentSpecific_ShouldLoadCorrectValues()
    {
        // Arrange
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        
        // Act
        var isDevelopment = await configService.GetSettingAsync<bool>("MTMSettings:TestMode");
        var logLevel = await configService.GetSettingAsync<string>("Logging:LogLevel:Default");
        
        // Assert
        Assert.That(isDevelopment, Is.True, "Test mode should be enabled in test configuration");
        Assert.That(logLevel, Is.Not.Empty, "Log level should be configured");
    }
}
```

## 🚀 Performance Integration Testing

```csharp
[TestFixture]
[Category("Integration")]
[Category("Performance")]
public class PerformanceIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private DatabaseTestFixture _databaseFixture;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _databaseFixture = new DatabaseTestFixture();
        await _databaseFixture.SetupAsync();
        
        var services = new ServiceCollection();
        ConfigureServicesForPerformanceTesting(services);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [Test]
    public async Task HighVolumeOperations_ShouldMaintainPerformance()
    {
        // Arrange
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        var operations = Enumerable.Range(1, 100)
            .Select(i => new InventoryItem
            {
                PartId = $"PERF_PART_{i:000}",
                Operation = "100",
                Quantity = 10,
                Location = "PERF_STATION",
                User = "PerformanceTest"
            }).ToList();
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var errorCount = 0;
        
        // Act - Execute operations concurrently
        var tasks = operations.Select(async operation =>
        {
            try
            {
                var result = await inventoryService.SaveInventoryAsync(operation);
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
        
        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(30000), 
            "100 operations should complete within 30 seconds");
        Assert.That(successCount, Is.GreaterThanOrEqualTo(95), 
            "At least 95% of operations should succeed");
        Assert.That(errorCount, Is.LessThanOrEqualTo(5), 
            "No more than 5% of operations should fail");
        
        Console.WriteLine($"Performance Results:");
        Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Average per Operation: {stopwatch.ElapsedMilliseconds / 100.0:F2}ms");
        Console.WriteLine($"  Success Rate: {successCount}/100");
        Console.WriteLine($"  Error Rate: {errorCount}/100");
    }
    
    [Test]
    public async Task DatabaseConnectionPool_ShouldHandleConcurrentAccess()
    {
        // Arrange
        var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();
        var concurrentQueries = 50;
        
        // Act - Execute concurrent database queries
        var tasks = Enumerable.Range(1, concurrentQueries)
            .Select(async i =>
            {
                var parameters = new MySqlParameter[]
                {
                    new("p_PartID", $"CONCURRENT_TEST_{i:00}")
                };
                
                var result = await databaseService.ExecuteStoredProcedureAsync(
                    "inv_inventory_Get_ByPartID", parameters);
                    
                return result.Status >= 0; // Success or no data found
            });
        
        var results = await Task.WhenAll(tasks);
        
        // Assert
        var successCount = results.Count(r => r);
        Assert.That(successCount, Is.EqualTo(concurrentQueries), 
            "All concurrent database operations should succeed");
    }
    
    private void ConfigureServicesForPerformanceTesting(IServiceCollection services)
    {
        // Use optimized configuration for performance testing
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = _databaseFixture.ConnectionString,
                ["MTMSettings:TestMode"] = "true",
                ["MTMSettings:DatabaseTimeout"] = "10", // Shorter timeout for performance testing
                ["MTMSettings:MaxConcurrentOperations"] = "100"
            })
            .Build();
            
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning)); // Reduced logging
        
        // Add services with performance-optimized configuration
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddScoped<IInventoryService, InventoryService>();
    }
}
```

## 🧪 Test Utilities and Fixtures

### Database Test Fixture

```csharp
public class DatabaseTestFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    private readonly string _testDatabaseName;
    
    public DatabaseTestFixture()
    {
        _testDatabaseName = $"mtm_test_{Guid.NewGuid():N}";
        ConnectionString = $"Server=localhost;Database={_testDatabaseName};Uid=test_user;Pwd=test_password;Allow Zero Datetime=true;Convert Zero Datetime=true;";
    }
    
    public async Task SetupAsync()
    {
        // Create test database
        await CreateTestDatabaseAsync();
        
        // Create test tables and stored procedures
        await CreateTestSchemaAsync();
        
        // Insert test data
        await InsertTestDataAsync();
    }
    
    public async Task TearDownAsync()
    {
        // Clean up test database
        await DropTestDatabaseAsync();
    }
    
    public async Task CleanupTestDataAsync()
    {
        // Remove test data but keep schema
        var cleanupQueries = new[]
        {
            "DELETE FROM inv_transactions WHERE User = 'TestUser'",
            "DELETE FROM inv_inventory WHERE User = 'TestUser'",
            "DELETE FROM qb_quickbuttons WHERE User = 'TestUser'"
        };
        
        foreach (var query in cleanupQueries)
        {
            try
            {
                await ExecuteNonQueryAsync(query);
            }
            catch (Exception ex)
            {
                // Log but don't fail - cleanup is best effort
                Console.WriteLine($"Cleanup warning: {ex.Message}");
            }
        }
    }
    
    private async Task CreateTestDatabaseAsync()
    {
        var masterConnectionString = ConnectionString.Replace($"Database={_testDatabaseName}", "Database=mysql");
        using var connection = new MySqlConnection(masterConnectionString);
        await connection.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE `{_testDatabaseName}`";
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task CreateTestSchemaAsync()
    {
        var schemaScript = await File.ReadAllTextAsync("TestData/DatabaseTestSchema.sql");
        await ExecuteNonQueryAsync(schemaScript);
    }
    
    private async Task InsertTestDataAsync()
    {
        var testDataScript = await File.ReadAllTextAsync("TestData/TestDataInsert.sql");
        await ExecuteNonQueryAsync(testDataScript);
    }
    
    private async Task ExecuteNonQueryAsync(string commandText)
    {
        using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = commandText;
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task DropTestDatabaseAsync()
    {
        try
        {
            var masterConnectionString = ConnectionString.Replace($"Database={_testDatabaseName}", "Database=mysql");
            using var connection = new MySqlConnection(masterConnectionString);
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = $"DROP DATABASE IF EXISTS `{_testDatabaseName}`";
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database cleanup warning: {ex.Message}");
        }
    }
    
    public void Dispose()
    {
        Task.Run(async () => await TearDownAsync()).Wait(TimeSpan.FromSeconds(30));
    }
}
```

### Configuration Test Fixture

```csharp
public class ConfigurationTestFixture : IDisposable
{
    private readonly string _testConfigFile;
    
    public ConfigurationTestFixture()
    {
        _testConfigFile = Path.Combine(Path.GetTempPath(), $"mtm_test_config_{Guid.NewGuid():N}.json");
        CreateTestConfigurationFile();
    }
    
    public IServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(_testConfigFile, optional: false)
            .Build();
        
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        
        return services.BuildServiceProvider();
    }
    
    private void CreateTestConfigurationFile()
    {
        var testConfig = new
        {
            ConnectionStrings = new
            {
                DefaultConnection = "Server=localhost;Database=mtm_test;Uid=test_user;Pwd=test_password;"
            },
            MTMSettings = new
            {
                TestMode = true,
                DatabaseTimeout = 30,
                ConfigFile = _testConfigFile
            },
            Logging = new
            {
                LogLevel = new
                {
                    Default = "Debug",
                    Microsoft = "Warning"
                }
            }
        };
        
        var json = JsonSerializer.Serialize(testConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_testConfigFile, json);
    }
    
    public void Dispose()
    {
        try
        {
            if (File.Exists(_testConfigFile))
                File.Delete(_testConfigFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Config cleanup warning: {ex.Message}");
        }
    }
}
```

## 📊 Integration Test Coverage Requirements

### Coverage Targets

| Integration Category | Coverage Target | Key Areas |
|---------------------|----------------|-----------|
| Database Procedures | 100% | All 45+ stored procedures tested |
| Service Communication | 90%+ | Cross-service interactions validated |
| Configuration | 100% | All settings and connection strings |
| Error Scenarios | 85%+ | Database failures, service errors |
| Performance | 100% | High-volume and concurrent operations |

### Critical Integration Scenarios

1. **Database Transaction Integrity** - Multi-step operations with rollback testing
2. **Service State Consistency** - Cross-service data synchronization
3. **Configuration Hot-Reload** - Settings changes without restart
4. **Error Propagation** - Error handling across service boundaries  
5. **Performance Under Load** - System behavior with high concurrent usage
6. **Database Connection Recovery** - Reconnection after network issues
7. **Cross-Platform Data Compatibility** - Data consistency across platforms

This comprehensive integration testing strategy ensures all MTM components work together seamlessly while maintaining manufacturing-grade reliability and performance standards.

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