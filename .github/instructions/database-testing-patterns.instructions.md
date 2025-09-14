---
description: 'Database testing patterns and stored procedure validation for MTM MySQL integration'
applies_to: '**/*'
---

# MTM Database Testing Patterns Instructions

## 🎯 Overview

Comprehensive database testing patterns for MTM WIP Application, focusing on MySQL stored procedure validation, transaction integrity, and cross-platform database compatibility testing.

## 🗄️ Database Testing Architecture

### Core Database Test Framework

```csharp
[TestFixture]
[Category("Database")]
public abstract class DatabaseTestBase
{
    protected string ConnectionString { get; private set; }
    protected DatabaseTestFixture DatabaseFixture { get; private set; }
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        DatabaseFixture = new DatabaseTestFixture();
        await DatabaseFixture.SetupAsync();
        ConnectionString = DatabaseFixture.ConnectionString;
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await DatabaseFixture?.TearDownAsync();
    }
    
    [SetUp]
    public async Task SetUp()
    {
        await DatabaseFixture.CleanupTestDataAsync();
        await SeedTestDataAsync();
    }
    
    protected abstract Task SeedTestDataAsync();
    
    protected async Task<DatabaseResult> ExecuteStoredProcedureAsync(string procedureName, params MySqlParameter[] parameters)
    {
        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            ConnectionString, procedureName, parameters);
    }
    
    protected void AssertSuccessResult(DatabaseResult result, string operation)
    {
        Assert.That(result.Status, Is.EqualTo(1), $"{operation} should return success status (1)");
        Assert.That(result.Data, Is.Not.Null, $"{operation} should return data");
    }
    
    protected void AssertValidationError(DatabaseResult result, string operation)
    {
        Assert.That(result.Status, Is.EqualTo(0), $"{operation} should return validation warning (0)");
    }
    
    protected void AssertSystemError(DatabaseResult result, string operation)
    {
        Assert.That(result.Status, Is.EqualTo(-1), $"{operation} should return system error (-1)");
    }
}
```

## 📦 Inventory Stored Procedures Testing

### Inventory Management Core Operations

```csharp
[TestFixture]
[Category("Database")]
[Category("InventoryProcedures")]
public class InventoryStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        // Seed master data required for inventory operations
        await SeedMasterDataAsync();
    }
    
    [Test]
    [TestCase("INV_SP_001", "90", 25, "STATION_A", "TestUser")]
    [TestCase("INV_SP_002", "100", 50, "STATION_B", "TestUser")]
    [TestCase("INV_SP_003", "110", 15, "STATION_C", "TestUser")]
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
        
        // Verify data was inserted
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
        
        // Act - Add to same part/operation
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
        Assert.That(finalQuantity, Is.EqualTo(initialQuantity - removeQuantity));
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
    }
    
    [Test]
    public async Task inv_inventory_Get_ByPartID_ExistingPart_ShouldReturnAllOperations()
    {
        // Arrange - Add inventory for same part with different operations
        var partId = "MULTI_OP_TEST";
        var operations = new[] { "90", "100", "110", "120" };
        
        foreach (var operation in operations)
        {
            await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                new MySqlParameter("p_PartID", partId),
                new MySqlParameter("p_OperationNumber", operation),
                new MySqlParameter("p_Quantity", 10),
                new MySqlParameter("p_Location", "STATION_A"),
                new MySqlParameter("p_User", "TestUser"));
        }
        
        // Act
        var result = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartID",
            new MySqlParameter("p_PartID", partId));
        
        // Assert
        AssertSuccessResult(result, "Get inventory by part ID");
        Assert.That(result.Data.Rows.Count, Is.EqualTo(operations.Length),
            "Should return all operations for the part");
        
        // Verify all operations are present
        var returnedOperations = result.Data.AsEnumerable()
            .Select(row => row["OperationNumber"].ToString())
            .ToArray();
            
        CollectionAssert.AreEquivalent(operations, returnedOperations,
            "Should return all expected operations");
    }
    
    [Test]
    public async Task inv_inventory_Update_Quantity_ValidData_ShouldUpdateSuccessfully()
    {
        // Arrange - Add initial inventory
        var partId = "UPDATE_TEST";
        var operation = "100";
        var initialQuantity = 25;
        var updatedQuantity = 40;
        
        await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", initialQuantity),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Act - Update quantity
        var result = await ExecuteStoredProcedureAsync("inv_inventory_Update_Quantity",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_NewQuantity", updatedQuantity),
            new MySqlParameter("p_User", "TestUser"));
        
        // Assert
        AssertSuccessResult(result, "Update inventory quantity");
        
        // Verify quantity was updated
        var checkResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
            
        var finalQuantity = Convert.ToInt32(checkResult.Data.Rows[0]["Quantity"]);
        Assert.That(finalQuantity, Is.EqualTo(updatedQuantity));
    }
    
    private async Task SeedMasterDataAsync()
    {
        // Seed required master data for tests
        var masterDataSets = new[]
        {
            ("md_part_ids_Add", new[] { new MySqlParameter("p_PartID", "TEST_PART_001") }),
            ("md_locations_Add", new[] { new MySqlParameter("p_Location", "STATION_A") }),
            ("md_operation_numbers_Add", new[] { new MySqlParameter("p_OperationNumber", "100") })
        };
        
        foreach (var (procedure, parameters) in masterDataSets)
        {
            try
            {
                await ExecuteStoredProcedureAsync(procedure, parameters);
            }
            catch
            {
                // Ignore errors for master data that might already exist
            }
        }
    }
}
```

## 🔄 Transaction History Testing

```csharp
[TestFixture]
[Category("Database")]
[Category("TransactionProcedures")]
public class TransactionStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        // Seed base inventory for transaction testing
        await SeedInventoryDataAsync();
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
        Assert.That(transactionRow["TransactionType"].ToString(), Is.EqualTo(transactionData.TransactionType));
        Assert.That(Convert.ToInt32(transactionRow["Quantity"]), Is.EqualTo(transactionData.Quantity));
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
    }
    
    [Test]
    public async Task inv_transaction_Get_History_WithDateRange_ShouldFilterCorrectly()
    {
        // Arrange - Add transactions over time
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
        
        // Act - Get recent transactions only
        var result = await ExecuteStoredProcedureAsync("inv_transaction_Get_History_ByDateRange",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_StartDate", DateTime.Now.AddDays(-7)),
            new MySqlParameter("p_EndDate", DateTime.Now.AddDays(1)));
        
        // Assert
        AssertSuccessResult(result, "Get transactions by date range");
        Assert.That(result.Data.Rows.Count, Is.EqualTo(1), "Should return only recent transaction");
        
        var transactionRow = result.Data.Rows[0];
        Assert.That(Convert.ToInt32(transactionRow["Quantity"]), Is.EqualTo(10));
    }
    
    [Test]
    public async Task inv_transaction_Get_ByUser_ValidUser_ShouldReturnUserTransactions()
    {
        // Arrange - Add transactions for different users
        var testUsers = new[] { "User1", "User2", "User3" };
        var targetUser = "User2";
        
        for (int i = 0; i < testUsers.Length; i++)
        {
            await ExecuteStoredProcedureAsync("inv_transaction_Add",
                new MySqlParameter("p_PartID", $"USER_TEST_{i}"),
                new MySqlParameter("p_OperationNumber", "100"),
                new MySqlParameter("p_Quantity", 10 + i),
                new MySqlParameter("p_Location", "STATION_A"),
                new MySqlParameter("p_TransactionType", "IN"),
                new MySqlParameter("p_User", testUsers[i]));
        }
        
        // Act
        var result = await ExecuteStoredProcedureAsync("inv_transaction_Get_ByUser",
            new MySqlParameter("p_User", targetUser));
        
        // Assert
        AssertSuccessResult(result, "Get transactions by user");
        Assert.That(result.Data.Rows.Count, Is.GreaterThan(0), "Should return user transactions");
        
        // Verify all returned transactions belong to target user
        foreach (DataRow row in result.Data.Rows)
        {
            Assert.That(row["User"].ToString(), Is.EqualTo(targetUser));
        }
    }
    
    private async Task SeedInventoryDataAsync()
    {
        // Add some base inventory for transaction testing
        var testInventoryItems = new[]
        {
            new { PartID = "TRANS_BASE_001", Operation = "100", Quantity = 100 },
            new { PartID = "TRANS_BASE_002", Operation = "110", Quantity = 50 },
            new { PartID = "TRANS_BASE_003", Operation = "90", Quantity = 75 }
        };
        
        foreach (var item in testInventoryItems)
        {
            await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                new MySqlParameter("p_PartID", item.PartID),
                new MySqlParameter("p_OperationNumber", item.Operation),
                new MySqlParameter("p_Quantity", item.Quantity),
                new MySqlParameter("p_Location", "STATION_A"),
                new MySqlParameter("p_User", "SeedUser"));
        }
    }
}
```

## 📊 Master Data Procedures Testing

```csharp
[TestFixture]
[Category("Database")]
[Category("MasterDataProcedures")]
public class MasterDataStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        // Master data tests should start with clean slate
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
        
        // Verify data quality
        if (result.Data.Rows.Count > 0)
        {
            foreach (DataRow row in result.Data.Rows)
            {
                var value = row[expectedColumn]?.ToString();
                Assert.That(string.IsNullOrWhiteSpace(value), Is.False,
                    $"Master data values should not be null or empty");
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
            new MySqlParameter("p_Description", "Test part for unit testing"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Assert
        AssertSuccessResult(result, "Add new part ID");
        
        // Verify part was added
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
        await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", duplicatePartId),
            new MySqlParameter("p_Description", "First addition"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Act - Try to add same part again
        var result = await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", duplicatePartId),
            new MySqlParameter("p_Description", "Duplicate addition"),
            new MySqlParameter("p_User", "TestUser"));
        
        // Assert
        Assert.That(result.Status, Is.LessThan(1), "Should not allow duplicate part IDs");
        Assert.That(result.Message, Does.Contain("duplicate").Or.Contain("exists").IgnoreCase,
            "Error message should indicate duplicate");
    }
    
    [Test]
    public async Task md_locations_GetActive_ShouldReturnOnlyActiveLocations()
    {
        // Arrange - Add active and inactive locations
        await ExecuteStoredProcedureAsync("md_locations_Add",
            new MySqlParameter("p_Location", "ACTIVE_STATION_A"),
            new MySqlParameter("p_IsActive", true),
            new MySqlParameter("p_User", "TestUser"));
            
        await ExecuteStoredProcedureAsync("md_locations_Add",
            new MySqlParameter("p_Location", "INACTIVE_STATION_B"),
            new MySqlParameter("p_IsActive", false),
            new MySqlParameter("p_User", "TestUser"));
        
        // Act
        var result = await ExecuteStoredProcedureAsync("md_locations_Get_Active");
        
        // Assert
        AssertSuccessResult(result, "Get active locations");
        
        var locations = result.Data.AsEnumerable()
            .Select(row => row["Location"].ToString())
            .ToList();
            
        Assert.That(locations, Contains.Item("ACTIVE_STATION_A"));
        Assert.That(locations, Does.Not.Contain("INACTIVE_STATION_B"));
    }
    
    [Test]
    public async Task md_operation_numbers_ValidateWorkflow_ShouldConfirmValidSequence()
    {
        // Arrange - Add operation sequence
        var workflowOperations = new[] { "90", "100", "110", "120", "130" };
        
        foreach (var operation in workflowOperations)
        {
            await ExecuteStoredProcedureAsync("md_operation_numbers_Add",
                new MySqlParameter("p_OperationNumber", operation),
                new MySqlParameter("p_Description", $"Operation {operation}"),
                new MySqlParameter("p_SequenceOrder", int.Parse(operation)),
                new MySqlParameter("p_User", "TestUser"));
        }
        
        // Act
        var result = await ExecuteStoredProcedureAsync("md_operation_numbers_Get_WorkflowSequence");
        
        // Assert
        AssertSuccessResult(result, "Get workflow sequence");
        
        var returnedOperations = result.Data.AsEnumerable()
            .OrderBy(row => Convert.ToInt32(row["SequenceOrder"]))
            .Select(row => row["OperationNumber"].ToString())
            .ToArray();
            
        CollectionAssert.AreEqual(workflowOperations, returnedOperations,
            "Operations should be returned in workflow sequence");
    }
    
    [Test]
    public async Task MasterData_CrossReference_ShouldMaintainReferentialIntegrity()
    {
        // Arrange - Add related master data
        var partId = "INTEGRITY_TEST_PART";
        var location = "INTEGRITY_TEST_LOCATION";
        var operation = "INTEGRITY_TEST_OP";
        
        await ExecuteStoredProcedureAsync("md_part_ids_Add",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_Description", "Integrity test part"),
            new MySqlParameter("p_User", "TestUser"));
            
        await ExecuteStoredProcedureAsync("md_locations_Add",
            new MySqlParameter("p_Location", location),
            new MySqlParameter("p_IsActive", true),
            new MySqlParameter("p_User", "TestUser"));
            
        await ExecuteStoredProcedureAsync("md_operation_numbers_Add",
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Description", "Integrity test operation"),
            new MySqlParameter("p_SequenceOrder", 999),
            new MySqlParameter("p_User", "TestUser"));
        
        // Act - Use master data in inventory operation
        var inventoryResult = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation),
            new MySqlParameter("p_Quantity", 10),
            new MySqlParameter("p_Location", location),
            new MySqlParameter("p_User", "TestUser"));
        
        // Assert
        AssertSuccessResult(inventoryResult, "Use master data in inventory operation");
        
        // Verify referential integrity is maintained
        var verifyResult = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation",
            new MySqlParameter("p_PartID", partId),
            new MySqlParameter("p_OperationNumber", operation));
            
        AssertSuccessResult(verifyResult, "Verify referential integrity");
        Assert.That(verifyResult.Data.Rows[0]["Location"].ToString(), Is.EqualTo(location));
    }
}
```

## 🎯 QuickButtons Procedures Testing

```csharp
[TestFixture]
[Category("Database")]
[Category("QuickButtonsProcedures")]
public class QuickButtonsStoredProcedureTests : DatabaseTestBase
{
    protected override async Task SeedTestDataAsync()
    {
        // Seed some transaction history for QuickButtons testing
        await SeedTransactionHistoryAsync();
    }
    
    [Test]
    public async Task qb_quickbuttons_Get_ByUser_ValidUser_ShouldReturnRecentTransactions()
    {
        // Arrange
        var testUser = "QB_TEST_USER";
        
        // Add some transactions for this user
        var testTransactions = new[]
        {
            new { PartID = "QB_PART_001", Operation = "100", Quantity = 10 },
            new { PartID = "QB_PART_002", Operation = "110", Quantity = 15 },
            new { PartID = "QB_PART_003", Operation = "90", Quantity = 5 }
        };
        
        foreach (var transaction in testTransactions)
        {
            await ExecuteStoredProcedureAsync("inv_transaction_Add",
                new MySqlParameter("p_PartID", transaction.PartID),
                new MySqlParameter("p_OperationNumber", transaction.Operation),
                new MySqlParameter("p_Quantity", transaction.Quantity),
                new MySqlParameter("p_Location", "STATION_A"),
                new MySqlParameter("p_TransactionType", "IN"),
                new MySqlParameter("p_User", testUser));
        }
        
        // Act
        var result = await ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser",
            new MySqlParameter("p_User", testUser));
        
        // Assert
        AssertSuccessResult(result, "Get QuickButtons by user");
        Assert.That(result.Data.Rows.Count, Is.GreaterThan(0), "Should return QuickButtons");
        Assert.That(result.Data.Rows.Count, Is.LessThanOrEqualTo(10), "Should limit to recent transactions");
        
        // Verify all returned transactions belong to the user
        foreach (DataRow row in result.Data.Rows)
        {
            Assert.That(row["User"].ToString(), Is.EqualTo(testUser));
        }
        
        // Verify transactions are in reverse chronological order (newest first)
        var timestamps = result.Data.AsEnumerable()
            .Select(row => Convert.ToDateTime(row["TransactionDate"]))
            .ToList();
            
        for (int i = 1; i < timestamps.Count; i++)
        {
            Assert.That(timestamps[i], Is.LessThanOrEqualTo(timestamps[i - 1]),
                "QuickButtons should be ordered by most recent first");
        }
    }
    
    [Test]
    public async Task qb_quickbuttons_Save_ValidButton_ShouldSaveCustomButton()
    {
        // Arrange
        var customButton = new
        {
            User = "CUSTOM_USER",
            PartID = "CUSTOM_PART",
            OperationNumber = "CUSTOM_OP",
            Quantity = 25,
            Location = "CUSTOM_LOCATION",
            DisplayText = "Custom Button (25)",
            ButtonOrder = 1
        };
        
        var parameters = new MySqlParameter[]
        {
            new("p_User", customButton.User),
            new("p_PartID", customButton.PartID),
            new("p_OperationNumber", customButton.OperationNumber),
            new("p_Quantity", customButton.Quantity),
            new("p_Location", customButton.Location),
            new("p_DisplayText", customButton.DisplayText),
            new("p_ButtonOrder", customButton.ButtonOrder)
        };
        
        // Act
        var result = await ExecuteStoredProcedureAsync("qb_quickbuttons_Save", parameters);
        
        // Assert
        AssertSuccessResult(result, "Save custom QuickButton");
        
        // Verify button was saved
        var checkResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser",
            new MySqlParameter("p_User", customButton.User));
            
        AssertSuccessResult(checkResult, "Get saved QuickButton");
        
        var savedButton = checkResult.Data.AsEnumerable()
            .FirstOrDefault(row => row["PartID"].ToString() == customButton.PartID);
            
        Assert.That(savedButton, Is.Not.Null, "Custom button should be saved");
        Assert.That(savedButton["DisplayText"].ToString(), Is.EqualTo(customButton.DisplayText));
        Assert.That(Convert.ToInt32(savedButton["ButtonOrder"]), Is.EqualTo(customButton.ButtonOrder));
    }
    
    [Test]
    public async Task qb_quickbuttons_Remove_ValidButton_ShouldRemoveSuccessfully()
    {
        // Arrange - First save a button
        var testUser = "REMOVE_TEST_USER";
        var testPartId = "REMOVE_TEST_PART";
        
        await ExecuteStoredProcedureAsync("qb_quickbuttons_Save",
            new MySqlParameter("p_User", testUser),
            new MySqlParameter("p_PartID", testPartId),
            new MySqlParameter("p_OperationNumber", "100"),
            new MySqlParameter("p_Quantity", 10),
            new MySqlParameter("p_Location", "STATION_A"),
            new MySqlParameter("p_DisplayText", "Remove Test Button"),
            new MySqlParameter("p_ButtonOrder", 1));
        
        // Verify button exists
        var beforeResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser",
            new MySqlParameter("p_User", testUser));
        Assert.That(beforeResult.Data.Rows.Count, Is.GreaterThan(0), "Button should exist before removal");
        
        // Act - Remove button
        var removeResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Remove",
            new MySqlParameter("p_User", testUser),
            new MySqlParameter("p_PartID", testPartId));
        
        // Assert
        AssertSuccessResult(removeResult, "Remove QuickButton");
        
        // Verify button was removed
        var afterResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser",
            new MySqlParameter("p_User", testUser));
            
        var remainingButtons = afterResult.Data.AsEnumerable()
            .Where(row => row["PartID"].ToString() == testPartId)
            .ToList();
            
        Assert.That(remainingButtons.Count, Is.EqualTo(0), "Button should be removed");
    }
    
    [Test]
    public async Task qb_quickbuttons_Clear_ByUser_ShouldRemoveAllUserButtons()
    {
        // Arrange - Add multiple buttons for user
        var testUser = "CLEAR_TEST_USER";
        var testButtons = new[]
        {
            new { PartID = "CLEAR_PART_001", Operation = "100" },
            new { PartID = "CLEAR_PART_002", Operation = "110" },
            new { PartID = "CLEAR_PART_003", Operation = "90" }
        };
        
        foreach (var button in testButtons)
        {
            await ExecuteStoredProcedureAsync("qb_quickbuttons_Save",
                new MySqlParameter("p_User", testUser),
                new MySqlParameter("p_PartID", button.PartID),
                new MySqlParameter("p_OperationNumber", button.Operation),
                new MySqlParameter("p_Quantity", 10),
                new MySqlParameter("p_Location", "STATION_A"),
                new MySqlParameter("p_DisplayText", $"Clear Test {button.PartID}"),
                new MySqlParameter("p_ButtonOrder", 1));
        }
        
        // Verify buttons exist
        var beforeResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser",
            new MySqlParameter("p_User", testUser));
        Assert.That(beforeResult.Data.Rows.Count, Is.EqualTo(testButtons.Length));
        
        // Act - Clear all buttons for user
        var clearResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Clear_ByUser",
            new MySqlParameter("p_User", testUser));
        
        // Assert
        AssertSuccessResult(clearResult, "Clear all QuickButtons for user");
        
        // Verify all buttons were cleared
        var afterResult = await ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser",
            new MySqlParameter("p_User", testUser));
            
        Assert.That(afterResult.Data.Rows.Count, Is.EqualTo(0), 
            "All QuickButtons should be cleared for user");
    }
    
    private async Task SeedTransactionHistoryAsync()
    {
        // Add some base transaction history
        var baseTransactions = new[]
        {
            new { PartID = "SEED_PART_001", Operation = "100", Quantity = 10, User = "SeedUser1" },
            new { PartID = "SEED_PART_002", Operation = "110", Quantity = 15, User = "SeedUser2" },
            new { PartID = "SEED_PART_003", Operation = "90", Quantity = 20, User = "SeedUser1" }
        };
        
        foreach (var transaction in baseTransactions)
        {
            await ExecuteStoredProcedureAsync("inv_transaction_Add",
                new MySqlParameter("p_PartID", transaction.PartID),
                new MySqlParameter("p_OperationNumber", transaction.Operation),
                new MySqlParameter("p_Quantity", transaction.Quantity),
                new MySqlParameter("p_Location", "STATION_A"),
                new MySqlParameter("p_TransactionType", "IN"),
                new MySqlParameter("p_User", transaction.User));
        }
    }
}
```

## 🔧 Database Test Infrastructure

### Database Test Fixture Implementation

```csharp
public class DatabaseTestFixture : IDisposable
{
    private const string TestDatabasePrefix = "mtm_test_";
    private readonly string _testDatabaseName;
    private readonly string _masterConnectionString;
    
    public string ConnectionString { get; private set; }
    
    public DatabaseTestFixture()
    {
        _testDatabaseName = $"{TestDatabasePrefix}{Guid.NewGuid():N}";
        _masterConnectionString = GetMasterConnectionString();
        ConnectionString = GetTestConnectionString();
    }
    
    public async Task SetupAsync()
    {
        await CreateTestDatabaseAsync();
        await CreateTestSchemaAsync();
        await SeedBaseMasterDataAsync();
    }
    
    public async Task TearDownAsync()
    {
        await DropTestDatabaseAsync();
    }
    
    public async Task CleanupTestDataAsync()
    {
        var cleanupQueries = new[]
        {
            "DELETE FROM inv_transactions WHERE User LIKE '%Test%'",
            "DELETE FROM inv_inventory WHERE User LIKE '%Test%'",
            "DELETE FROM qb_quickbuttons WHERE User LIKE '%Test%'",
            "DELETE FROM md_part_ids WHERE PartID LIKE '%TEST%'",
            "DELETE FROM md_locations WHERE Location LIKE '%TEST%'",
            "DELETE FROM md_operation_numbers WHERE OperationNumber LIKE '%TEST%'"
        };
        
        foreach (var query in cleanupQueries)
        {
            try
            {
                await ExecuteNonQueryAsync(ConnectionString, query);
            }
            catch (Exception ex)
            {
                // Log cleanup warnings but don't fail tests
                Console.WriteLine($"Cleanup warning: {ex.Message}");
            }
        }
    }
    
    private async Task CreateTestDatabaseAsync()
    {
        using var connection = new MySqlConnection(_masterConnectionString);
        await connection.OpenAsync();
        
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE `{_testDatabaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
        await command.ExecuteNonQueryAsync();
    }
    
    private async Task CreateTestSchemaAsync()
    {
        var schemaScripts = new[]
        {
            "TestData/Schema/CreateTables.sql",
            "TestData/Schema/CreateStoredProcedures.sql",
            "TestData/Schema/CreateIndexes.sql"
        };
        
        foreach (var scriptFile in schemaScripts)
        {
            if (File.Exists(scriptFile))
            {
                var script = await File.ReadAllTextAsync(scriptFile);
                await ExecuteNonQueryAsync(ConnectionString, script);
            }
        }
    }
    
    private async Task SeedBaseMasterDataAsync()
    {
        // Seed essential master data for testing
        var baseMasterData = new[]
        {
            ("md_part_ids_Add", new[] { new MySqlParameter("p_PartID", "BASE_PART_001"), new MySqlParameter("p_Description", "Base test part"), new MySqlParameter("p_User", "System") }),
            ("md_locations_Add", new[] { new MySqlParameter("p_Location", "STATION_A"), new MySqlParameter("p_IsActive", true), new MySqlParameter("p_User", "System") }),
            ("md_operation_numbers_Add", new[] { new MySqlParameter("p_OperationNumber", "100"), new MySqlParameter("p_Description", "Standard operation"), new MySqlParameter("p_SequenceOrder", 100), new MySqlParameter("p_User", "System") })
        };
        
        foreach (var (procedure, parameters) in baseMasterData)
        {
            try
            {
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    ConnectionString, procedure, parameters);
                // Ignore status - base data seeding is best effort
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Master data seeding warning: {ex.Message}");
            }
        }
    }
    
    private async Task DropTestDatabaseAsync()
    {
        try
        {
            using var connection = new MySqlConnection(_masterConnectionString);
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
    
    private async Task ExecuteNonQueryAsync(string connectionString, string sql)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        
        // Split and execute multiple statements
        var statements = sql.Split(new[] { "GO", ";" }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var statement in statements)
        {
            var trimmedStatement = statement.Trim();
            if (!string.IsNullOrEmpty(trimmedStatement))
            {
                using var command = connection.CreateCommand();
                command.CommandText = trimmedStatement;
                await command.ExecuteNonQueryAsync();
            }
        }
    }
    
    private string GetMasterConnectionString()
    {
        var configuration = TestConfigurationHelper.GetConfiguration();
        var baseConnection = configuration.GetConnectionString("DefaultConnection");
        
        // Replace database name with mysql system database
        return Regex.Replace(baseConnection, @"Database=\w+;", "Database=mysql;");
    }
    
    private string GetTestConnectionString()
    {
        var configuration = TestConfigurationHelper.GetConfiguration();
        var baseConnection = configuration.GetConnectionString("DefaultConnection");
        
        // Replace database name with test database
        return Regex.Replace(baseConnection, @"Database=\w+;", $"Database={_testDatabaseName};");
    }
    
    public void Dispose()
    {
        Task.Run(async () => await TearDownAsync()).Wait(TimeSpan.FromSeconds(30));
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
        // Seed performance test data
        await SeedLargeDatasetAsync();
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
                Operation = (i % 4) switch
                {
                    0 => "90",
                    1 => "100",
                    2 => "110",
                    _ => "120"
                },
                Quantity = 10 + (i % 50),
                Location = $"STATION_{(i % 5) + 1}"
            }).ToList();
        
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var errorCount = 0;
        
        // Act - Execute operations
        var tasks = operations.Select(async op =>
        {
            try
            {
                var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                    new MySqlParameter("p_PartID", op.PartID),
                    new MySqlParameter("p_OperationNumber", op.Operation),
                    new MySqlParameter("p_Quantity", op.Quantity),
                    new MySqlParameter("p_Location", op.Location),
                    new MySqlParameter("p_User", "PerformanceTest"));
                
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
        
        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(60000), 
            $"1000 operations should complete within 60 seconds, took {stopwatch.ElapsedMilliseconds}ms");
        Assert.That(successCount, Is.GreaterThanOrEqualTo(950), 
            $"At least 95% should succeed, got {successCount}/{operationCount}");
        Assert.That(errorCount, Is.LessThanOrEqualTo(50),
            $"No more than 5% should fail, got {errorCount}/{operationCount}");
        
        Console.WriteLine($"Database Performance Results:");
        Console.WriteLine($"  Operations: {operationCount}");
        Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Average per Operation: {stopwatch.ElapsedMilliseconds / (double)operationCount:F2}ms");
        Console.WriteLine($"  Success Rate: {successCount}/{operationCount} ({successCount * 100.0 / operationCount:F1}%)");
        Console.WriteLine($"  Error Rate: {errorCount}/{operationCount} ({errorCount * 100.0 / operationCount:F1}%)");
    }
    
    [Test]
    public async Task StoredProcedures_ConcurrentAccess_ShouldHandleContention()
    {
        // Test database locking and concurrency
        var partId = "CONCURRENT_TEST_PART";
        var operation = "100";
        var concurrentUsers = 20;
        var operationsPerUser = 10;
        
        // Each user performs multiple operations on the same part
        var tasks = Enumerable.Range(1, concurrentUsers).Select(async userIndex =>
        {
            var userName = $"ConcurrentUser{userIndex:00}";
            var userSuccessCount = 0;
            
            for (int opIndex = 1; opIndex <= operationsPerUser; opIndex++)
            {
                try
                {
                    var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
                        new MySqlParameter("p_PartID", $"{partId}_{userIndex}"),
                        new MySqlParameter("p_OperationNumber", operation),
                        new MySqlParameter("p_Quantity", opIndex),
                        new MySqlParameter("p_Location", "CONCURRENT_STATION"),
                        new MySqlParameter("p_User", userName));
                    
                    if (result.Status == 1)
                        userSuccessCount++;
                }
                catch
                {
                    // Count failures but don't fail the test
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
    }
    
    private async Task SeedLargeDatasetAsync()
    {
        // Seed master data for performance testing
        var partCount = 100;
        var locationCount = 20;
        var operationCount = 10;
        
        // Seed parts
        for (int i = 1; i <= partCount; i++)
        {
            try
            {
                await ExecuteStoredProcedureAsync("md_part_ids_Add",
                    new MySqlParameter("p_PartID", $"PERF_PART_{i:000}"),
                    new MySqlParameter("p_Description", $"Performance test part {i}"),
                    new MySqlParameter("p_User", "PerformanceSetup"));
            }
            catch { /* Ignore duplicates */ }
        }
        
        // Seed locations
        for (int i = 1; i <= locationCount; i++)
        {
            try
            {
                await ExecuteStoredProcedureAsync("md_locations_Add",
                    new MySqlParameter("p_Location", $"PERF_STATION_{i:00}"),
                    new MySqlParameter("p_IsActive", true),
                    new MySqlParameter("p_User", "PerformanceSetup"));
            }
            catch { /* Ignore duplicates */ }
        }
        
        // Seed operations
        for (int i = 1; i <= operationCount; i++)
        {
            try
            {
                await ExecuteStoredProcedureAsync("md_operation_numbers_Add",
                    new MySqlParameter("p_OperationNumber", $"PERF_OP_{i:00}"),
                    new MySqlParameter("p_Description", $"Performance operation {i}"),
                    new MySqlParameter("p_SequenceOrder", i * 10),
                    new MySqlParameter("p_User", "PerformanceSetup"));
            }
            catch { /* Ignore duplicates */ }
        }
    }
}
```

## 📊 Database Testing Coverage Requirements

### Coverage Targets

| Database Test Category | Coverage Target | Key Areas |
|------------------------|----------------|-----------|
| Stored Procedures | 100% | All 45+ procedures tested |
| Data Validation | 100% | All input validation rules |
| Error Handling | 100% | All error conditions covered |
| Transaction Integrity | 100% | Multi-step operation consistency |
| Performance | 90%+ | High-volume and concurrent scenarios |
| Cross-Platform | 100% | Database compatibility across platforms |

### Critical Database Test Scenarios

1. **All Stored Procedures** - Individual testing of each procedure with valid/invalid data
2. **Transaction Workflows** - Multi-step operations with rollback testing
3. **Data Integrity** - Foreign key constraints and referential integrity
4. **Concurrent Access** - Multiple users accessing same data simultaneously
5. **Performance Under Load** - High-volume operations and query optimization
6. **Error Recovery** - Database connection failures and reconnection
7. **Cross-Platform Compatibility** - MySQL behavior consistency across platforms

This comprehensive database testing framework ensures the MTM application maintains data integrity, performance, and reliability across all database operations while supporting manufacturing-grade transaction volumes.