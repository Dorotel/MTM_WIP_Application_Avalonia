# MTM Stored Procedures Documentation

**Generated**: September 5, 2025  
**Database**: MySQL 9.4.0  
**Application**: MTM WIP Application Avalonia  
**Pattern**: Stored Procedures Only (No Direct SQL)

## 📋 Overview

This document provides comprehensive documentation for all stored procedures used in the MTM WIP Application. The application follows a **stored procedures only** pattern using the `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` method for all database operations.

## 🏗️ Architecture Pattern

### Database Access Pattern (MANDATORY)
```csharp
// ✅ CORRECT: All database operations MUST use this pattern
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Operation", operation),
    new("p_Quantity", quantity)
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",  // Use actual stored procedure names
    parameters
);

// Process standardized result
if (result.Status == 1)
{
    // Success - process result.Data (DataTable)
    var dataTable = result.Data;
    // Handle successful operation
}
else
{
    // Handle error - result.Status contains error code
    await ErrorHandling.HandleErrorAsync(
        new InvalidOperationException($"Database operation failed with status: {result.Status}"),
        $"Failed to execute stored procedure"
    );
}
```

## 📚 Stored Procedures Catalog

### Inventory Management Procedures

#### `assign_BatchNumber_Step0()`
**Purpose**: Initialize batch number migration process  
**Parameters**: None  
**Returns**: No result set  
**Usage**: Data migration and batch number initialization  
**Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "assign_BatchNumber_Step0",
    new MySqlParameter[0]
);
```

#### `assign_BatchNumber_Step1()` 
**Purpose**: Process batch number assignment for inventory items  
**Parameters**: None  
**Returns**: No result set  
**Usage**: Batch processing of inventory records for migration  
**Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "assign_BatchNumber_Step1",
    new MySqlParameter[0]
);
```

#### `inv_inventory_Add_Item(p_PartID, p_Operation, p_Quantity, p_Location, p_UserID)`
**Purpose**: Add inventory quantity for specified part and operation  
**Parameters**:
- `p_PartID` (VARCHAR): Part identifier (e.g., "PART001")
- `p_Operation` (VARCHAR): Operation number (e.g., "90", "100", "110")
- `p_Quantity` (INT): Quantity to add (positive integer)
- `p_Location` (VARCHAR): Physical location identifier
- `p_UserID` (VARCHAR): User performing the operation
**Returns**: Status code and affected rows  
**Example**:
```csharp
var parameters = new MySqlParameter[]
{
    new("p_PartID", "PART001"),
    new("p_Operation", "100"),
    new("p_Quantity", 5),
    new("p_Location", "A01"),
    new("p_UserID", "user123")
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    parameters
);
```

#### `inv_inventory_Get_ByPartID(p_PartID)`
**Purpose**: Retrieve inventory records for a specific part  
**Parameters**:
- `p_PartID` (VARCHAR): Part identifier to search for
**Returns**: DataTable with inventory records
**Example**:
```csharp
var parameters = new MySqlParameter[]
{
    new("p_PartID", "PART001")
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Get_ByPartID",
    parameters
);

if (result.Status == 1)
{
    foreach (DataRow row in result.Data.Rows)
    {
        var partId = row["PartID"].ToString();
        var quantity = Convert.ToInt32(row["Quantity"]);
        // Process inventory data
    }
}
```

### Master Data Procedures

#### `md_part_ids_Get_All()`
**Purpose**: Retrieve all active part IDs for dropdown population  
**Parameters**: None  
**Returns**: DataTable with PartID column  
**Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "md_part_ids_Get_All",
    new MySqlParameter[0]
);
```

#### `md_operation_numbers_Get_All()`
**Purpose**: Retrieve all operation numbers for manufacturing workflow  
**Parameters**: None  
**Returns**: DataTable with Operation column  
**Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "md_operation_numbers_Get_All", 
    new MySqlParameter[0]
);
```

#### `md_locations_Get_All()`
**Purpose**: Retrieve all physical locations for inventory management  
**Parameters**: None  
**Returns**: DataTable with Location column  
**Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "md_locations_Get_All",
    new MySqlParameter[0]
);
```

### Transaction Management Procedures

#### `inv_transaction_Add(p_PartID, p_Operation, p_Quantity, p_TransactionType, p_UserID, p_Notes)`
**Purpose**: Record inventory transaction for audit trail  
**Parameters**:
- `p_PartID` (VARCHAR): Part identifier
- `p_Operation` (VARCHAR): Operation number
- `p_Quantity` (INT): Transaction quantity
- `p_TransactionType` (VARCHAR): "IN", "OUT", or "TRANSFER"
- `p_UserID` (VARCHAR): User performing transaction
- `p_Notes` (TEXT): Optional transaction notes
**Returns**: Transaction ID and status
**Example**:
```csharp
var parameters = new MySqlParameter[]
{
    new("p_PartID", "PART001"),
    new("p_Operation", "100"), 
    new("p_Quantity", 5),
    new("p_TransactionType", "IN"),
    new("p_UserID", "user123"),
    new("p_Notes", "Initial inventory")
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_transaction_Add",
    parameters
);
```

## 🔄 Integration with MTM Application

### Service Layer Integration
All stored procedures are accessed through the `MasterDataService`, `DatabaseService`, and related service classes using the standardized pattern:

```csharp
public class InventoryService : IInventoryService
{
    public async Task<bool> AddInventoryAsync(string partId, string operation, int quantity, string location)
    {
        try
        {
            var parameters = new MySqlParameter[]
            {
                new("p_PartID", partId),
                new("p_Operation", operation),
                new("p_Quantity", quantity),
                new("p_Location", location),
                new("p_UserID", Environment.UserName)
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Add_Item",
                parameters
            );

            return result.Status == 1;
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "AddInventoryAsync");
            return false;
        }
    }
}
```

## 📊 Manufacturing Domain Context

### Operation Numbers
- **90**: Receiving (parts enter system)
- **100**: First manufacturing operation
- **110**: Second manufacturing operation  
- **120**: Final manufacturing operation
- **130**: Shipping preparation

### Transaction Types
- **IN**: Adding inventory (receiving, production completion)
- **OUT**: Removing inventory (consumption, shipping)
- **TRANSFER**: Moving between locations/operations

## 🚨 Security and Error Handling

### Parameter Validation
All stored procedures include parameter validation and return standardized status codes:
- **1**: Success
- **0**: General failure
- **Negative values**: Specific error conditions

### Error Logging
Failed stored procedure calls are automatically logged through the centralized `ErrorHandling.HandleErrorAsync()` method with context information.

## 📝 Development Guidelines

### Adding New Stored Procedures
1. Create procedure in both Development and Production schema files
2. Add documentation entry to this file
3. Create corresponding service method
4. Add integration tests
5. Update master data services if applicable

### Testing Stored Procedures
```csharp
[Fact]
public async Task StoredProcedure_ReturnsExpectedResult()
{
    // Arrange
    var parameters = new MySqlParameter[] { /* test parameters */ };
    
    // Act
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        testConnectionString,
        "test_procedure_name",
        parameters
    );
    
    // Assert
    Assert.Equal(1, result.Status);
    Assert.NotNull(result.Data);
}
```

## 📚 Related Documentation

- **Database Schema**: `Development_Database_Schema.sql`
- **Service Integration**: `Services/DatabaseService.cs`
- **Error Handling**: `Services/ErrorHandling.cs`
- **Master Data Management**: `Services/MasterDataService.cs`

---

**Last Updated**: September 5, 2025  
**Maintained By**: MTM Development Team  
**Documentation Status**: ✅ Active and Complete