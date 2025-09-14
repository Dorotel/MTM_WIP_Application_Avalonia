---
description: 'Prompt template for creating database tests for MTM WIP Application MySQL stored procedures and data operations'
applies_to: '**/*'
---

# Create Database Test Prompt Template

## 🎯 Objective

Generate comprehensive database tests for MTM WIP Application MySQL stored procedures, focusing on data integrity, transaction consistency, and manufacturing workflow validation. Use real database connections with test isolation.

## 📋 Instructions

When creating database tests, follow these specific requirements:

### Database Test Structure

1. **Use MTM Database Test Base Class**
   ```csharp
   [TestFixture]
   [Category("Database")]
   [Category("{DatabaseCategory}")]  // e.g., StoredProcedures, Transactions, MasterData
   public class {ProcedureGroup}DatabaseTests : DatabaseTestBase
   {
       protected override async Task SeedTestDataAsync()
       {
           // Seed required test data for this procedure group
           await SeedSpecificMasterDataAsync();
       }
   }
   ```

2. **Database Test Categories**
   - StoredProcedures: Individual stored procedure testing
   - Transactions: Multi-step transaction workflows
   - MasterData: Master data management procedures
   - Integrity: Referential integrity and constraints
   - Performance: High-volume and concurrent operations
   - ErrorHandling: Database error scenarios

### Stored Procedure Testing Patterns

#### Inventory Stored Procedures
```csharp
[TestFixture]
[Category("Database")]
[Category("StoredProcedures")]
public class InventoryStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        // Seed master data required for inventory operations
        await SeedInventoryMasterDataAsync();
    }
    
    [Test]
    [TestCase("INV_TEST_001", "90", 25, "STATION_A", "TestUser")]
    [TestCase("INV_TEST_002", "100", 50, "STATION_B", "TestUser")]
    [TestCase("INV_TEST_003", "110", 15, "STATION_C", "TestUser")]
    public async Task inv_inventory_Add_Item_ValidData_ShouldAddSuccessfully(
        string partId, string operation, int quantity, string location, string user)
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
        var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item", parameters);
        
        // Assert
        AssertSuccessResult(result, "Add inventory item");
        
        // Verify data was inserted correctly
        var verifyResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
            
        AssertSuccessResult(verifyResult, "Verify inserted data");
        Assert.That(verifyResult.Data.Rows.Count, Is.GreaterThan(0), "Should find inserted inventory item");
        
        var row = verifyResult.Data.Rows[0];
        Assert.That(row["PartID"].ToString(), Is.EqualTo(partId));
        Assert.That(row["OperationNumber"].ToString(), Is.EqualTo(operation));
        Assert.That(Convert.ToInt32(row["Quantity"]), Is.EqualTo(quantity));
        Assert.That(row["Location"].ToString(), Is.EqualTo(location));
        Assert.That(row["User"].ToString(), Is.EqualTo(user));
    }
    
    [Test]
    public async Task inv_inventory_Add_Item_DuplicateEntry_ShouldUpdateQuantity()
    {
        // Arrange - Add initial item
        var partId = "DUPLICATE_TEST";
        var operation = "100";
        var initialQuantity = 10;
        var additionalQuantity = 5;
        
        var initialParams = new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation),
            new("p_Quantity", initialQuantity),
            new("p_Location", "STATION_A"),
            new("p_User", "TestUser")
        };
        
        await ExecuteStoredProcedureAsync("inv_inventory_Add_Item", initialParams);
        
        // Act - Add to same part/operation combination
        var duplicateParams = new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_OperationNumber", operation),
            new("p_Quantity", additionalQuantity),
            new("p_Location", "STATION_A"),
            new("p_User", "TestUser")
        };
        
        var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item", duplicateParams);
        
        // Assert
        AssertSuccessResult(result, "Add duplicate inventory item");
        
        // Verify quantity was updated (not duplicated)
        var checkResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
            
        var finalQuantity = Convert.ToInt32(checkResult.Data.Rows[0]["Quantity"]);
        Assert.That(finalQuantity, Is.EqualTo(initialQuantity + additionalQuantity),
            "Quantity should be sum of both additions");
    }
    
    [Test]
    [TestCase("", "100", 10, "STATION_A", "TestUser")] // Empty Part ID
    [TestCase("VALID_PART", "", 10, "STATION_A", "TestUser")] // Empty Operation
    [TestCase("VALID_PART", "100", 0, "STATION_A", "TestUser")] // Zero quantity
    [TestCase("VALID_PART", "100", -5, "STATION_A", "TestUser")] // Negative quantity
    [TestCase("VALID_PART", "100", 10, "", "TestUser")] // Empty location
    [TestCase("VALID_PART", "100", 10, "STATION_A", "")] // Empty user
    public async Task inv_inventory_Add_Item_InvalidData_ShouldReturnValidationError(
        string partId, string operation, int quantity, string location, string user)
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
        var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item", parameters);
        
        // Assert
        Assert.That(result.Status, Is.LessThan(1), "Invalid data should not return success");
        Assert.That(string.IsNullOrEmpty(result.Message), Is.False, "Error message should be provided");
        
        // Verify no data was inserted
        if (!string.IsNullOrEmpty(partId) && !string.IsNullOrEmpty(operation))
        {
            var checkResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
                new MySqlParameter("p_PartID", partId),
                new MySqlParameter("p_OperationNumber", operation));
                
            // Should either return no data or show validation failed
            if (checkResult.Status == 1)
            {
                Assert.That(checkResult.Data.Rows.Count, Is.EqualTo(0), "No data should be inserted for invalid input");
            }
        }
    }
    
    [Test]
    public async Task inv_inventory_Remove_Item_ValidData_ShouldReduceQuantity()
    {
        // Arrange - First add inventory
        var partId = "REMOVE_TEST";
        var operation = "100";
        var initialQuantity = 50;
        var removeQuantity = 20;
        
        await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", initialQuantity),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Act - Remove some inventory
        var removeResult = await ExecuteStoredProcedureAsync("inv_inventory_Remove_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", removeQuantity),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Assert
        AssertSuccessResult(removeResult, "Remove inventory item");
        
        // Verify quantity was reduced
        var checkResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
            
        var finalQuantity = Convert.ToInt32(checkResult.Data.Rows[0]["Quantity"]);
        Assert.That(finalQuantity, Is.EqualTo(initialQuantity - removeQuantity),
            $"Final quantity should be {initialQuantity - removeQuantity}, got {finalQuantity}");
    }
    
    [Test]
    public async Task inv_inventory_Remove_Item_InsufficientQuantity_ShouldReturnError()
    {
        // Arrange - Add small amount of inventory
        var partId = "INSUFFICIENT_TEST";
        var operation = "100";
        var availableQuantity = 5;
        var attemptRemoveQuantity = 10;
        
        await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", availableQuantity),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Act - Attempt to remove more than available
        var result = await ExecuteStoredProcedureAsync("inv_inventory_Remove_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", attemptRemoveQuantity),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Assert
        Assert.That(result.Status, Is.LessThan(1), "Should not allow removing more than available");
        Assert.That(result.Message, Does.Contain("insufficient").Or.Contain("not enough").IgnoreCase,
            "Error message should indicate insufficient quantity");
        
        // Verify original quantity is unchanged
        var checkResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
            
        var remainingQuantity = Convert.ToInt32(checkResult.Data.Rows[0]["Quantity"]);
        Assert.That(remainingQuantity, Is.EqualTo(availableQuantity),
            "Quantity should remain unchanged when removal fails");
    }
    
    private async Task SeedInventoryMasterDataAsync()
    {
        // Seed required master data for inventory tests
        var masterDataItems = new[]
        {
            new { Procedure = "md_part_ids_Add", Parameters = new[] { new MySqlParameter("p_PartID", "INV_TEST_001"), new MySqlParameter("p_Description", "Test part 1"), new MySqlParameter("p_User", "SystemSetup") } },
            new { Procedure = "md_part_ids_Add", Parameters = new[] { new MySqlParameter("p_PartID", "INV_TEST_002"), new MySqlParameter("p_Description", "Test part 2"), new MySqlParameter("p_User", "SystemSetup") } },
            new { Procedure = "md_locations_Add", Parameters = new[] { new MySqlParameter("p_Location", "STATION_A"), new MySqlParameter("p_IsActive", true), new MySqlParameter("p_User", "SystemSetup") } },
            new { Procedure = "md_operation_numbers_Add", Parameters = new[] { new MySqlParameter("p_OperationNumber", "100"), new MySqlParameter("p_Description", "Standard operation"), new MySqlParameter("p_SequenceOrder", 100), new MySqlParameter("p_User", "SystemSetup") } }
        };
        
        foreach (var item in masterDataItems)
        {
            try
            {
                await ExecuteStoredProcedureAsync(item.Procedure, item.Parameters);
            }
            catch
            {
                // Ignore errors for master data that might already exist
            }
        }
    }
}
```

#### Transaction History Stored Procedures
```csharp
[TestFixture]
[Category("Database")]
[Category("Transactions")]
public class TransactionStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        await SeedTransactionTestDataAsync();
    }
    
    [Test]
    public async Task inv_transaction_Add_ValidTransaction_ShouldRecordSuccessfully()
    {
        // Arrange
        var transactionData = new
        {
            PartID = "TRANS_TEST_001",
            OperationNumber = "100",
            Quantity = 15,
            Location = "STATION_A",
            TransactionType = "IN",
            User = "TransactionUser"
        };
        
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", transactionData.PartID),
            new("p_OperationNumber", transactionData.OperationNumber),
            new("p_Quantity", transactionData.Quantity),
            new("p_Location", transactionData.Location),
            new("p_TransactionType", transactionData.TransactionType),
            new("p_User", transactionData.User)
        };
        
        // Act
        var result = await ExecuteStoredProcedureAsync("inv_transaction_Add", parameters);
        
        // Assert
        AssertSuccessResult(result, "Add transaction");
        
        // Verify transaction was recorded
        var historyResult = await ExecuteStoredProcedureAsync("inv_transaction_Get_History",
            new MySqlParameter("p_PartID", transactionData.PartID),
            new MySqlParameter("p_OperationNumber", transactionData.OperationNumber));
            
        AssertSuccessResult(historyResult, "Get transaction history");
        Assert.That(historyResult.Data.Rows.Count, Is.GreaterThan(0), "Should have transaction history");
        
        var transactionRow = historyResult.Data.Rows[0];
        Assert.That(transactionRow["PartID"].ToString(), Is.EqualTo(transactionData.PartID));
        Assert.That(transactionRow["TransactionType"].ToString(), Is.EqualTo(transactionData.TransactionType));
        Assert.That(Convert.ToInt32(transactionRow["Quantity"]), Is.EqualTo(transactionData.Quantity));
        Assert.That(transactionRow["User"].ToString(), Is.EqualTo(transactionData.User));
    }
    
    [Test]
    [TestCase("IN")]
    [TestCase("OUT")]
    [TestCase("TRANSFER")]
    [TestCase("ADJUSTMENT")]
    public async Task inv_transaction_Add_ValidTransactionTypes_ShouldAcceptAllTypes(string transactionType)
    {
        // Arrange
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", $"TYPE_TEST_{transactionType}"),
            new("p_OperationNumber", "100"),
            new("p_Quantity", 10),
            new("p_Location", "STATION_A"),
            new("p_TransactionType", transactionType),
            new("p_User", "TypeTestUser")
        };
        
        // Act
        var result = await ExecuteStoredProcedureAsync("inv_transaction_Add", parameters);
        
        // Assert
        AssertSuccessResult(result, $"Add {transactionType} transaction");
        
        // Verify transaction type was recorded correctly
        var historyResult = await ExecuteStoredProcedureAsync("inv_transaction_Get_History",
            new MySqlParameter("p_PartID", $"TYPE_TEST_{transactionType}"),
            new MySqlParameter("p_OperationNumber", "100"));
            
        var transactionRow = historyResult.Data.Rows[0];
        Assert.That(transactionRow["TransactionType"].ToString(), Is.EqualTo(transactionType));
    }
    
    [Test]
    public async Task inv_transaction_Get_History_WithDateRange_ShouldFilterCorrectly()
    {
        // Arrange - Add transactions over different time periods
        var partId = "DATE_FILTER_TEST";
        var operation = "100";
        
        // Add old transaction
        await ExecuteStoredProcedureAsync("inv_transaction_Add",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", 5),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_TransactionType", "IN"),
            new MySqlParameter("p_User", "DateTestUser"),
            new MySqlParameter("p_TransactionDate", DateTime.Now.AddDays(-30)));
        
        // Add recent transaction
        await ExecuteStoredProcedureAsync("inv_transaction_Add",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", 10),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_TransactionType", "IN"),
            new MySqlParameter("p_User", "DateTestUser"),
            new MySqlParameter("p_TransactionDate", DateTime.Now));
        
        // Act - Get recent transactions only (last 7 days)
        var result = await ExecuteStoredProcedureAsync("inv_transaction_Get_History_ByDateRange",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_StartDate", DateTime.Now.AddDays(-7)),
            new MySqlParameter("p_EndDate", DateTime.Now.AddDays(1)));
        
        // Assert
        AssertSuccessResult(result, "Get transactions by date range");
        Assert.That(result.Data.Rows.Count, Is.EqualTo(1), "Should return only recent transaction");
        
        var transactionRow = result.Data.Rows[0];
        Assert.That(Convert.ToInt32(transactionRow["Quantity"]), Is.EqualTo(10),
            "Should return the recent transaction with quantity 10");
    }
    
    private async Task SeedTransactionTestDataAsync()
    {
        // Seed master data for transaction tests
        var seedItems = new[]
        {
            new { Procedure = "md_part_ids_Add", Parameters = new[] { new MySqlParameter("p_PartID", "TRANS_TEST_001"), new MySqlParameter("p_Description", "Transaction test part"), new MySqlParameter("p_User", "SystemSetup") } },
            new { Procedure = "md_locations_Add", Parameters = new[] { new MySqlParameter("p_Location", "STATION_A"), new MySqlParameter("p_IsActive", true), new MySqlParameter("p_User", "SystemSetup") } },
            new { Procedure = "md_operation_numbers_Add", Parameters = new[] { new MySqlParameter("p_OperationNumber", "100"), new MySqlParameter("p_Description", "Transaction test op"), new MySqlParameter("p_SequenceOrder", 100), new MySqlParameter("p_User", "SystemSetup") } }
        };
        
        foreach (var item in seedItems)
        {
            try
            {
                await ExecuteStoredProcedureAsync(item.Procedure, item.Parameters);
            }
            catch { /* Ignore seed errors */ }
        }
    }
}
```

### Master Data Stored Procedures
```csharp
[TestFixture]
[Category("Database")]
[Category("MasterData")]
public class MasterDataStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        // Master data tests start with clean slate
        await Task.CompletedTask;
    }
    
    [Test]
    [TestCase("md_part_ids_Get_All", "PartID")]
    [TestCase("md_locations_Get_All", "Location")]
    [TestCase("md_operation_numbers_Get_All", "OperationNumber")]
    public async Task MasterDataProcedures_GetAll_ShouldReturnValidStructure(
        string procedureName, string expectedColumn)
    {
        // Act
        var result = await ExecuteStoredProcedureAsync(procedureName);
        
        // Assert
        AssertSuccessResult(result, $"Execute {procedureName}");
        Assert.That(result.Data.Columns.Contains(expectedColumn), Is.True,
            $"Result should contain '{expectedColumn}' column");
        
        // Verify data quality if any data exists
        if (result.Data.Rows.Count > 0)
        {
            foreach (DataRow row in result.Data.Rows)
            {
                var value = row[expectedColumn]?.ToString();
                Assert.That(string.IsNullOrWhiteSpace(value), Is.False,
                    $"Master data values should not be null or empty in {procedureName}");
            }
        }
    }
    
    [Test]
    public async Task md_part_ids_Add_ValidPartId_ShouldAddSuccessfully()
    {
        // Arrange
        var newPartId = $"NEW_PART_{Guid.NewGuid():N[..8]}";
        
        // Act
        var result = await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", newPartId),
            new MySqlParameter("p_Description", "Test part for database testing"),
            new MySqlParameter("p_User", "DatabaseTestUser"));
        
        // Assert
        AssertSuccessResult(result, "Add new part ID");
        
        // Verify part was added to master data
        var checkResult = await ExecuteStoredProcedureAsync("md_part_ids_Get_All");
        var partIds = checkResult.Data.AsEnumerable()
            .Select(row => row["PartID"].ToString())
            .ToList();
            
        Assert.That(partIds, Contains.Item(newPartId), "New part ID should be in master data");
    }
    
    [Test]
    public async Task md_part_ids_Add_DuplicatePartId_ShouldReturnError()
    {
        // Arrange
        var duplicatePartId = "DUPLICATE_PART_TEST";
        
        // Add part first time
        var firstResult = await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", duplicatePartId),
            new MySqlParameter("p_Description", "First addition"),
            new MySqlParameter("p_User", "DatabaseTestUser"));
        
        AssertSuccessResult(firstResult, "First part addition should succeed");
        
        // Act - Try to add same part again
        var duplicateResult = await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", duplicatePartId),
            new MySqlParameter("p_Description", "Duplicate addition attempt"),
            new MySqlParameter("p_User", "DatabaseTestUser"));
        
        // Assert
        Assert.That(duplicateResult.Status, Is.LessThan(1), "Should not allow duplicate part IDs");
        Assert.That(duplicateResult.Message, Does.Contain("duplicate").Or.Contain("exists").IgnoreCase,
            "Error message should indicate duplicate part ID");
    }
}
```

### Database Performance Testing
```csharp
[TestFixture]
[Category("Database")]
[Category("Performance")]
public class DatabasePerformanceTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        await SeedPerformanceTestDataAsync();
    }
    
    [Test]
    public async Task StoredProcedures_HighVolumeOperations_ShouldMaintainPerformance()
    {
        // Arrange
        var operationCount = 1000;
        var operations = Enumerable.Range(1, operationCount)
            .Select(i => new
            {
                PartID = $"PERF_PART_{i:0000}",
                Operation = (i % 4) switch { 0 => "90", 1 => "100", 2 => "110", _ => "120" },
                Quantity = 10 + (i % 50),
                Location = $"STATION_{(i % 5) + 1}"
            }).ToList();
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var errorCount = 0;
        
        // Act - Execute high-volume operations
        var tasks = operations.Select(async op =>
        {
            try
            {
                var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                    new MySqlParameter("p_PartID", op.PartID),
                    new MySqlParameter("p_OperationNumber", op.Operation),
                    new MySqlParameter("p_Quantity", op.Quantity),
                    new MySqlParameter("p_Location", op.Location),
                    new MySqlParameter("p_User", "PerformanceTestUser"));
                
                if (result.Status == 1)
                    Interlocked.Increment(ref successCount);
                else
                    Interlocked.Increment(ref errorCount);
                    
                return result.Status == 1;
            }
            catch
            {
                Interlocked.Increment(ref errorCount);
                return false;
            }
        });
        
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // Assert performance thresholds
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(60000), 
            $"1000 operations should complete within 60 seconds, took {stopwatch.ElapsedMilliseconds}ms");
        Assert.That(successCount, Is.GreaterThanOrEqualTo(950), 
            $"At least 95% should succeed, got {successCount}/{operationCount}");
        Assert.That(errorCount, Is.LessThanOrEqualTo(50),
            $"No more than 5% should fail, got {errorCount}/{operationCount}");
        
        // Performance summary
        var averageTime = stopwatch.ElapsedMilliseconds / (double)operationCount;
        Console.WriteLine($"Database Performance Results:");
        Console.WriteLine($"  Operations: {operationCount}");
        Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedMilliseconds / 1000.0:F1}s)");
        Console.WriteLine($"  Average Time: {averageTime:F2}ms per operation");
        Console.WriteLine($"  Success Rate: {successCount}/{operationCount} ({successCount * 100.0 / operationCount:F1}%)");
        Console.WriteLine($"  Throughput: {operationCount * 1000.0 / stopwatch.ElapsedMilliseconds:F0} operations/second");
    }
    
    [Test]
    public async Task StoredProcedures_ConcurrentAccess_ShouldHandleContentionCorrectly()
    {
        // Arrange
        var partId = "CONCURRENT_TEST_PART";
        var operation = "100";
        var concurrentUsers = 10;
        var operationsPerUser = 5;
        
        // Act - Execute concurrent operations on same part
        var tasks = Enumerable.Range(1, concurrentUsers).Select(async userIndex =>
        {
            var userName = $"ConcurrentUser{userIndex:00}";
            var userSuccessCount = 0;
            
            for (int opIndex = 1; opIndex <= operationsPerUser; opIndex++)
            {
                try
                {
                    var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                        new MySqlParameter("p_PartID", $"{partId}_{userIndex}"), // Use different part per user to avoid deadlocks
                        new MySqlParameter("p_OperationNumber", operation),
                        new MySqlParameter("p_Quantity", opIndex),
                        new MySqlParameter("p_Location", "CONCURRENT_STATION"),
                        new MySqlParameter("p_User", userName));
                    
                    if (result.Status == 1)
                        userSuccessCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Concurrent operation failed for {userName}: {ex.Message}");
                }
            }
            
            return userSuccessCount;
        });
        
        var userSuccessCounts = await Task.WhenAll(tasks);
        var totalSuccesses = userSuccessCounts.Sum();
        var expectedTotal = concurrentUsers * operationsPerUser;
        
        // Assert
        Assert.That(totalSuccesses, Is.GreaterThanOrEqualTo(expectedTotal * 0.9),
            $"At least 90% of concurrent operations should succeed, got {totalSuccesses}/{expectedTotal}");
        
        Console.WriteLine($"Concurrent Database Operations Results:");
        Console.WriteLine($"  Concurrent Users: {concurrentUsers}");
        Console.WriteLine($"  Operations per User: {operationsPerUser}");
        Console.WriteLine($"  Total Expected: {expectedTotal}");
        Console.WriteLine($"  Total Successful: {totalSuccesses}");
        Console.WriteLine($"  Success Rate: {totalSuccesses * 100.0 / expectedTotal:F1}%");
    }
    
    private async Task SeedPerformanceTestDataAsync()
    {
        // Seed master data needed for performance tests
        var masterDataSets = new[]
        {
            new { Count = 100, Procedure = "md_part_ids_Add", Format = "PERF_PART_{0:000}", DescFormat = "Performance test part {0}" },
            new { Count = 20, Procedure = "md_locations_Add", Format = "PERF_STATION_{0:00}", DescFormat = "Performance test station {0}" },
            new { Count = 10, Procedure = "md_operation_numbers_Add", Format = "PERF_OP_{0:00}", DescFormat = "Performance test operation {0}" }
        };
        
        foreach (var dataSet in masterDataSets)
        {
            for (int i = 1; i <= dataSet.Count; i++)
            {
                try
                {
                    var parameters = dataSet.Procedure switch
                    {
                        "md_part_ids_Add" => new MySqlParameter[]
                        {
                            new("p_PartID", string.Format(dataSet.Format, i)),
                            new("p_Description", string.Format(dataSet.DescFormat, i)),
                            new("p_User", "PerformanceSetup")
                        },
                        "md_locations_Add" => new MySqlParameter[]
                        {
                            new("p_Location", string.Format(dataSet.Format, i)),
                            new("p_IsActive", true),
                            new("p_User", "PerformanceSetup")
                        },
                        "md_operation_numbers_Add" => new MySqlParameter[]
                        {
                            new("p_OperationNumber", string.Format(dataSet.Format, i)),
                            new("p_Description", string.Format(dataSet.DescFormat, i)),
                            new("p_SequenceOrder", i * 10),
                            new("p_User", "PerformanceSetup")
                        },
                        _ => throw new ArgumentException($"Unknown procedure: {dataSet.Procedure}")
                    };
                    
                    await ExecuteStoredProcedureAsync(dataSet.Procedure, parameters);
                }
                catch
                {
                    // Ignore seed errors (data might already exist)
                }
            }
        }
    }
}
```

## ✅ Database Test Checklist

When creating database tests, ensure:

- [ ] All stored procedures have success scenario tests
- [ ] Invalid data scenarios are tested with proper error handling
- [ ] Data validation rules are verified
- [ ] Transaction integrity is maintained across operations
- [ ] Concurrent access scenarios are tested
- [ ] Performance under load is measured
- [ ] Referential integrity constraints are validated
- [ ] All transaction types are tested
- [ ] Master data procedures maintain consistency
- [ ] Error messages are meaningful and actionable
- [ ] Test data cleanup is performed
- [ ] Database schema changes are handled gracefully

## 🏷️ Database Test Categories

Use these category attributes for database tests:

```csharp
[Category("Database")]           // All database tests
[Category("StoredProcedures")]   // Individual procedure tests
[Category("Transactions")]       // Transaction workflow tests
[Category("MasterData")]         // Master data management tests
[Category("Integrity")]          // Referential integrity tests
[Category("Performance")]        // Performance and load tests
[Category("ErrorHandling")]      // Database error scenarios
[Category("Concurrency")]        // Concurrent access tests
```

This template ensures comprehensive database test coverage for all MTM WIP Application MySQL stored procedures while maintaining manufacturing-grade data integrity and performance standards.