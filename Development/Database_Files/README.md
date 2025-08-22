# Development Database Files

This directory contains **development and testing** database files for the MTM WIP Application. Files here are used for new feature development, testing, and experimentation before being promoted to production.

## ?? **CRITICAL FIX #1 COMPLETED** ?

**Status**: ? **DEVELOPMENT UNBLOCKED - 12 Comprehensive Stored Procedures Implemented**

The empty development stored procedure files that were blocking all new database functionality development have been resolved with **12 comprehensive stored procedures** featuring:
- ? **Standardized Error Handling**: All procedures return `p_Status` and `p_ErrorMsg`
- ? **Input Validation**: Complete parameter validation for all procedures
- ? **Transaction Management**: Proper START/COMMIT/ROLLBACK patterns
- ? **Business Rule Enforcement**: MTM-specific validation and constraints
- ? **Service Layer Ready**: Designed for `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`

## ?? CRITICAL RULE: NO DIRECT SQL IN CODE ??

**ALL database operations in the application code MUST use stored procedures ONLY.**

### ? PROHIBITED
```csharp
// Direct SQL queries - NEVER DO THIS
var query = "SELECT * FROM inv_inventory WHERE PartID = @partId";
var query = "INSERT INTO inv_transaction (PartID, Quantity) VALUES (@part, @qty)";
var query = "UPDATE inv_inventory SET Quantity = @qty WHERE ID = @id";
var query = "DELETE FROM inv_inventory WHERE ID = @id";
```

### ? REQUIRED - NOW AVAILABLE WITH 12 NEW PROCEDURES
```csharp
// Use new enhanced stored procedures for ALL database operations
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item_Enhanced", // ? New comprehensive procedure
    new Dictionary<string, object> 
    { 
        ["p_PartID"] = partId,
        ["p_OperationID"] = "90",
        ["p_LocationID"] = "RECEIVING",
        ["p_Quantity"] = 100,
        ["p_UserID"] = "admin"
    }
);

// Standard status handling for all new procedures
if (result.Status == 0)
{
    // Success - operation completed
}
else if (result.Status == 1)
{
    // Warning - validation issue (show user-friendly message)
    ShowWarning(result.Message);
}
else
{
    // Error - SQL exception (log and show error)
    LogError(result.Message);
    ShowError("Operation failed. Please try again.");
}
```

## ?? MTM Business Logic Rules

### **CRITICAL: Transaction Type Determination** ? **IMPLEMENTED CORRECTLY**
**TransactionType is determined by the USER'S INTENT, NOT the Operation number.**

#### **Correct TransactionType Logic** ? **ENFORCED IN NEW PROCEDURES**
- **IN**: User is adding stock to inventory ? **inv_inventory_Add_Item_Enhanced**
- **OUT**: User is removing stock from inventory ? **inv_inventory_Remove_Item_Enhanced**  
- **TRANSFER**: User is moving stock from one location to another ? **inv_inventory_Transfer_Item_New**

#### **MTM Operation Numbers**
Operation numbers ("90", "100", "110", etc.) are **workflow step identifiers**, NOT transaction type indicators:
- Operations represent manufacturing or processing steps
- They help track which stage of production a part is in
- They do NOT determine whether inventory is being added, removed, or transferred
- The same operation number can be used with any TransactionType depending on user intent

## ?? File Structure ? **ENHANCED**

```
Development/Database_Files/
?? Development_Database_Schema.sql          # Development database structure
?? New_Stored_Procedures.sql               # ? 12 COMPREHENSIVE PROCEDURES IMPLEMENTED
?? Updated_Stored_Procedures.sql           # ? Ready template for future updates
?? README_Development_Database_Schema.md   # Development schema docs
?? README_NewProcedures.md                 # ? COMPLETE DOCUMENTATION WITH EXAMPLES
?? README_Updated_Stored_Procedures.md     # Updated procedures docs
?? README.md                               # This file
```

## ?? Available Database Operations ? **READY FOR USE**

### **Enhanced Inventory Management Procedures:**
1. **`inv_inventory_Add_Item_Enhanced`** - Add inventory with full error handling and validation
2. **`inv_inventory_Remove_Item_Enhanced`** - Remove inventory with stock availability validation  
3. **`inv_inventory_Transfer_Item_New`** - Transfer inventory between locations with comprehensive validation

### **Inventory Query Procedures:**
4. **`inv_inventory_Get_ByLocation_New`** - Get all inventory items at a specific location
5. **`inv_inventory_Get_ByOperation_New`** - Get all inventory items for a specific operation
6. **`inv_part_Get_Info_New`** - Get detailed part information with inventory summary
7. **`inv_inventory_Get_Summary_New`** - Get consolidated inventory summary across locations/operations

### **Validation & Utility Procedures:**
8. **`inv_inventory_Validate_Stock_New`** - Validate sufficient stock before removal operations
9. **`inv_transaction_Log_New`** - Log inventory transactions with comprehensive validation
10. **`inv_location_Validate_New`** - Validate location exists and is active
11. **`inv_operation_Validate_New`** - Validate operation exists and is active
12. **`sys_user_Validate_New`** - Validate user exists and is active

## ?? Development Workflow ? **STREAMLINED**

### 1. Creating New Stored Procedures ? **TEMPLATE AVAILABLE**
- Add new procedures to `New_Stored_Procedures.sql` ? **Follow pattern from existing 12 procedures**
- Document in `README_NewProcedures.md` ? **Template established**
- Test thoroughly before promoting to production ? **Validation procedures available**

### 2. Updating Existing Procedures ? **PROCESS READY**
- **NEVER** edit `Database_Files/Existing_Stored_Procedures.sql` (read-only)
- Copy procedure from `Existing_Stored_Procedures.sql` 
- Modify copy in `Updated_Stored_Procedures.sql` ? **Template prepared**
- Document changes in `README_Updated_Stored_Procedures.md`

### 3. Updating New Procedures ? **DIRECT EDITING AVAILABLE**
- Directly overwrite procedures in `New_Stored_Procedures.sql`
- Update documentation as needed

## ??? Database Environments

| Environment | Database Name | Purpose | Status |
|-------------|---------------|---------|---------|
| **Development** | `mtm_wip_application_test` | Feature development and testing | ? **12 Procedures Available** |
| **Production** | `mtm_wip_application` | Live production system | Ready for deployment |

## ?? Code Integration Patterns ? **ENHANCED WITH NEW PROCEDURES**

### Service Layer Pattern ? **READY FOR IMPLEMENTATION**
```csharp
public class InventoryService 
{
    public async Task<Result<DataTable>> GetInventoryByLocationAsync(string locationId, string userId)
    {
        try
        {
            // ? Use new comprehensive stored procedure
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "inv_inventory_Get_ByLocation_New",
                new Dictionary<string, object> 
                { 
                    ["p_LocationID"] = locationId,
                    ["p_UserID"] = userId
                }
            );
            
            if (result.Status == 0)
            {
                return Result<DataTable>.Success(result.Data, result.Message);
            }
            else if (result.Status == 1)
            {
                return Result<DataTable>.Warning(null, result.Message);
            }
            else
            {
                return Result<DataTable>.Error(result.Message);
            }
        }
        catch (Exception ex)
        {
            return Result<DataTable>.Error($"Service error: {ex.Message}");
        }
    }
}
```

### Correct Transaction Type Assignment ? **IMPLEMENTED IN NEW PROCEDURES**
```csharp
public async Task<Result<bool>> AddInventoryItemAsync(string partId, string operationId, string locationId, int quantity, string userId)
{
    try 
    {
        // ? Use new enhanced procedure - automatically uses correct TransactionType = 'IN'
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Add_Item_Enhanced",
            new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_OperationID"] = operationId, // Just a workflow step number
                ["p_LocationID"] = locationId,
                ["p_Quantity"] = quantity,
                ["p_UnitCost"] = null, // Optional
                ["p_ReferenceNumber"] = null, // Optional
                ["p_Notes"] = "Added via service layer",
                ["p_UserID"] = userId
                // TransactionType = 'IN' automatically set by procedure (user intent: adding stock)
            }
        );
        
        if (result.Status == 0)
        {
            return Result<bool>.Success(true, result.Message);
        }
        else
        {
            return Result<bool>.Warning(false, result.Message);
        }
    }
    catch (Exception ex)
    {
        return Result<bool>.Error($"Failed to add inventory item: {ex.Message}");
    }
}

public async Task<Result<bool>> RemoveInventoryItemAsync(string partId, string operationId, string locationId, int quantity, string userId)
{
    try 
    {
        // ? Use new enhanced procedure - automatically uses correct TransactionType = 'OUT'
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Remove_Item_Enhanced",
            new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_OperationID"] = operationId, // Just a workflow step number
                ["p_LocationID"] = locationId,
                ["p_Quantity"] = quantity,
                ["p_ReferenceNumber"] = null, // Optional
                ["p_Notes"] = "Removed via service layer",
                ["p_UserID"] = userId
                // TransactionType = 'OUT' automatically set by procedure (user intent: removing stock)
            }
        );
        
        if (result.Status == 0)
        {
            return Result<bool>.Success(true, result.Message);
        }
        else
        {
            return Result<bool>.Warning(false, result.Message);
        }
    }
    catch (Exception ex)
    {
        return Result<bool>.Error($"Failed to remove inventory item: {ex.Message}");
    }
}

public async Task<Result<bool>> TransferInventoryItemAsync(string partId, string operationId, string fromLocationId, string toLocationId, int quantity, string userId)
{
    try 
    {
        // ? Use new enhanced procedure - automatically uses correct TransactionType = 'TRANSFER'
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Transfer_Item_New",
            new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_OperationID"] = operationId, // Just a workflow step number
                ["p_FromLocationID"] = fromLocationId,
                ["p_ToLocationID"] = toLocationId,
                ["p_Quantity"] = quantity,
                ["p_ReferenceNumber"] = null, // Optional
                ["p_Notes"] = "Transferred via service layer",
                ["p_UserID"] = userId
                // TransactionType = 'TRANSFER' automatically set by procedure (user intent: moving stock)
            }
        );
        
        if (result.Status == 0)
        {
            return Result<bool>.Success(true, result.Message);
        }
        else
        {
            return Result<bool>.Warning(false, result.Message);
        }
    }
    catch (Exception ex)
    {
        return Result<bool>.Error($"Failed to transfer inventory item: {ex.Message}");
    }
}
```

### Validation Before Operations ? **NEW VALIDATION PROCEDURES AVAILABLE**
```csharp
public async Task<Result<bool>> ValidateInventoryOperationAsync(string partId, string operationId, string locationId, int quantity, string userId)
{
    try 
    {
        // ? Validate stock availability before removal/transfer
        var stockResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Validate_Stock_New",
            new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_OperationID"] = operationId,
                ["p_LocationID"] = locationId,
                ["p_RequiredQuantity"] = quantity,
                ["p_UserID"] = userId
            }
        );

        if (stockResult.Status != 0)
        {
            return Result<bool>.Warning(false, stockResult.Message);
        }

        // ? Validate location exists
        var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_location_Validate_New",
            new Dictionary<string, object>
            {
                ["p_LocationID"] = locationId,
                ["p_UserID"] = userId
            }
        );

        if (locationResult.Status != 0)
        {
            return Result<bool>.Warning(false, locationResult.Message);
        }

        // ? Validate operation exists
        var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_operation_Validate_New",
            new Dictionary<string, object>
            {
                ["p_OperationID"] = operationId,
                ["p_UserID"] = userId
            }
        );

        if (operationResult.Status != 0)
        {
            return Result<bool>.Warning(false, operationResult.Message);
        }

        return Result<bool>.Success(true, "All validation checks passed");
    }
    catch (Exception ex)
    {
        return Result<bool>.Error($"Validation failed: {ex.Message}");
    }
}
```

### Error Handling Pattern ? **STANDARDIZED ACROSS ALL NEW PROCEDURES**
```csharp
public async Task<Result<bool>> ProcessInventoryOperationWithValidationAsync(InventoryOperation operation)
{
    try 
    {
        // ? Pre-validate using new validation procedures
        var validationResult = await ValidateInventoryOperationAsync(
            operation.PartId, operation.OperationId, operation.LocationId, 
            operation.Quantity, operation.UserId);

        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        // ? Execute operation using appropriate enhanced procedure
        var result = operation.Action switch
        {
            "ADD" => await AddInventoryItemAsync(operation.PartId, operation.OperationId, 
                operation.LocationId, operation.Quantity, operation.UserId),
            "REMOVE" => await RemoveInventoryItemAsync(operation.PartId, operation.OperationId, 
                operation.LocationId, operation.Quantity, operation.UserId),
            "TRANSFER" => await TransferInventoryItemAsync(operation.PartId, operation.OperationId, 
                operation.FromLocationId, operation.ToLocationId, operation.Quantity, operation.UserId),
            _ => Result<bool>.Error($"Unknown operation action: {operation.Action}")
        };

        // ? Log transaction using new logging procedure
        if (result.IsSuccess)
        {
            await LogTransactionAsync(operation);
        }

        return result;
    }
    catch (Exception ex)
    {
        return Result<bool>.Error($"Failed to process inventory operation: {ex.Message}");
    }
}

private async Task LogTransactionAsync(InventoryOperation operation)
{
    try
    {
        // ? Use new transaction logging procedure
        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_transaction_Log_New",
            new Dictionary<string, object>
            {
                ["p_TransactionType"] = operation.Action switch
                {
                    "ADD" => "IN",
                    "REMOVE" => "OUT", 
                    "TRANSFER" => "TRANSFER",
                    _ => "ADJUSTMENT"
                },
                ["p_PartID"] = operation.PartId,
                ["p_OperationID"] = operation.OperationId,
                ["p_FromLocationID"] = operation.FromLocationId,
                ["p_ToLocationID"] = operation.ToLocationId ?? operation.LocationId,
                ["p_Quantity"] = operation.Quantity,
                ["p_UnitCost"] = operation.UnitCost,
                ["p_ReferenceNumber"] = operation.ReferenceNumber,
                ["p_Notes"] = operation.Notes,
                ["p_UserID"] = operation.UserId
            }
        );
    }
    catch (Exception ex)
    {
        // Log error but don't fail main operation
        // TODO: Use error logging procedure
    }
}
```

## ?? Stored Procedure Naming Conventions ? **ESTABLISHED WITH NEW PROCEDURES**

| Pattern | Example | Purpose | Status |
|---------|---------|---------|---------|
| `inv_{table}_{action}_{details}` | `inv_inventory_Add_Item_Enhanced` | Enhanced inventory operations | ? **Implemented** |
| `inv_{table}_{action}_{details}` | `inv_inventory_Get_ByLocation_New` | New query operations | ? **Implemented** |
| `inv_{table}_{action}_{details}` | `inv_inventory_Validate_Stock_New` | Validation operations | ? **Implemented** |
| `inv_{entity}_{action}_New` | `inv_location_Validate_New` | Entity validation | ? **Implemented** |
| `sys_{entity}_{action}_New` | `sys_user_Validate_New` | System operations | ? **Implemented** |

## ?? Testing Procedures ? **ENHANCED WITH NEW VALIDATION**

### 1. Unit Testing ? **VALIDATION PROCEDURES AVAILABLE**
```csharp
[Test]
public async Task AddInventoryItem_ShouldCreateINTransaction_WhenUserAddsStock()
{
    // Arrange
    var partId = "TEST001";
    var operationId = "90";
    var locationId = "RECEIVING";
    var quantity = 10;
    var userId = "testuser";
    
    // Act - Adding stock should create IN transaction (user intent)
    var result = await _inventoryService.AddInventoryItemAsync(partId, operationId, locationId, quantity, userId);
    
    // Assert
    Assert.True(result.IsSuccess);
    
    // ? Verify using new query procedure
    var transactions = await _transactionService.GetTransactionHistoryAsync(partId, userId);
    Assert.AreEqual("IN", transactions.Value.First().TransactionType);
    Assert.AreEqual(operationId, transactions.Value.First().OperationId); // Operation is workflow step only
}

[Test]
public async Task RemoveInventoryItem_ShouldCreateOUTTransaction_WhenUserRemovesStock()
{
    // Arrange
    var partId = "TEST001";
    var operationId = "100"; // Different operation number
    var locationId = "PRODUCTION";
    var quantity = 5;
    var userId = "testuser";
    
    // Act - Removing stock should create OUT transaction (user intent)
    var result = await _inventoryService.RemoveInventoryItemAsync(partId, operationId, locationId, quantity, userId);
    
    // Assert
    Assert.True(result.IsSuccess);
    
    // ? Verify using new query procedure
    var transactions = await _transactionService.GetTransactionHistoryAsync(partId, userId);
    Assert.AreEqual("OUT", transactions.Value.First().TransactionType);
    Assert.AreEqual(operationId, transactions.Value.First().OperationId); // Operation is workflow step only
}

[Test]
public async Task ValidateStock_ShouldReturnWarning_WhenInsufficientStock()
{
    // Arrange
    var partId = "TEST001";
    var operationId = "90";
    var locationId = "RECEIVING";
    var requiredQuantity = 1000; // More than available
    var userId = "testuser";
    
    // Act - ? Use new validation procedure
    var result = await _inventoryService.ValidateStockAsync(partId, operationId, locationId, requiredQuantity, userId);
    
    // Assert
    Assert.False(result.IsSuccess);
    Assert.Contains("Insufficient stock", result.Message);
}
```

### 2. Integration Testing ? **COMPREHENSIVE PROCEDURES AVAILABLE**
- Test stored procedures directly against development database ? **12 procedures ready**
- Validate data integrity and constraints ? **Input validation implemented**
- Performance testing with realistic data volumes ? **Optimized procedures**
- Verify TransactionType is correctly determined by user intent ? **Implemented correctly**
- Test error handling scenarios ? **Comprehensive error handling**

## ?? Migration to Production ? **READY FOR DEPLOYMENT**

### Promotion Process
1. **Development Complete**: All tests pass in development environment ? **12 procedures tested**
2. **Code Review**: Peer review of stored procedures and documentation ? **Documentation complete**
3. **Staging Test**: Deploy to staging environment for final validation
4. **Production Deploy**: Move files to `Database_Files/` (production folder)
5. **Database Update**: Execute procedures against production database

### File Movement
```bash
# Move new procedures to production (when ready)
cp Development/Database_Files/New_Stored_Procedures.sql Database_Files/Production_New_Procedures_2024-01-XX.sql

# Move updated procedures to production (when available)
cp Development/Database_Files/Updated_Stored_Procedures.sql Database_Files/Production_Updates_2024-01-XX.sql
```

## ?? Common Operations ? **ENHANCED PROCEDURES AVAILABLE**

### Adding Inventory (IN Transaction) ? **ENHANCED PROCEDURE**
```sql
CALL inv_inventory_Add_Item_Enhanced(
    'PART001',        -- p_PartID
    '90',             -- p_OperationID (workflow step)
    'RECEIVING',      -- p_LocationID
    100,              -- p_Quantity
    5.25,             -- p_UnitCost (optional)
    'PO12345',        -- p_ReferenceNumber (optional)
    'Initial stock',  -- p_Notes (optional)
    'admin',          -- p_UserID
    @status,          -- OUT p_Status
    @msg              -- OUT p_ErrorMsg
);
-- TransactionType = 'IN' automatically set (user intent: adding stock)
```

### Removing Inventory (OUT Transaction) ? **ENHANCED PROCEDURE**
```sql
CALL inv_inventory_Remove_Item_Enhanced(
    'PART001',        -- p_PartID
    '90',             -- p_OperationID (workflow step)
    'RECEIVING',      -- p_LocationID
    50,               -- p_Quantity
    'WO54321',        -- p_ReferenceNumber (optional)
    'Production use', -- p_Notes (optional)
    'admin',          -- p_UserID
    @status,          -- OUT p_Status
    @msg              -- OUT p_ErrorMsg
);
-- TransactionType = 'OUT' automatically set (user intent: removing stock)
```

### Transferring Inventory (TRANSFER Transaction) ? **NEW PROCEDURE**
```sql
CALL inv_inventory_Transfer_Item_New(
    'PART001',        -- p_PartID
    '90',             -- p_OperationID (workflow step)
    'RECEIVING',      -- p_FromLocationID
    'PRODUCTION',     -- p_ToLocationID
    25,               -- p_Quantity
    'MOVE123',        -- p_ReferenceNumber (optional)
    'To production',  -- p_Notes (optional)
    'admin',          -- p_UserID
    @status,          -- OUT p_Status
    @msg              -- OUT p_ErrorMsg
);
-- TransactionType = 'TRANSFER' automatically set (user intent: moving stock)
```

### Stock Validation ? **NEW PROCEDURE**
```sql
CALL inv_inventory_Validate_Stock_New(
    'PART001',        -- p_PartID
    '90',             -- p_OperationID
    'RECEIVING',      -- p_LocationID
    100,              -- p_RequiredQuantity
    'admin',          -- p_UserID
    @status,          -- OUT p_Status (0=Sufficient, 1=Insufficient)
    @msg              -- OUT p_ErrorMsg (includes availability details)
);
```

## ?? Related Documentation ? **COMPREHENSIVE**

- [Complete New Procedures Documentation](README_NewProcedures.md) - ? **Detailed documentation with examples**
- [Development Database Schema](README_Development_Database_Schema.md) - Complete development schema
- [Updated Stored Procedures](README_Updated_Stored_Procedures.md) - Modified procedures (when available)
- [Production Database Files](../../Database_Files/README.md) - Production procedures and schema

## ?? Development Guidelines ? **ENHANCED**

1. **Always use stored procedures** - No exceptions for direct SQL ? **12 comprehensive procedures available**
2. **Test thoroughly** - All procedures must be tested before promotion ? **Validation procedures available**
3. **Document everything** - Update README files for all changes ? **Complete documentation provided**
4. **Follow naming conventions** - Consistent procedure naming ? **Pattern established**
5. **Handle errors properly** - Use standard error handling patterns ? **Standardized pattern implemented**
6. **Version control** - Track all changes in git
7. **Security first** - Validate all inputs in stored procedures ? **Comprehensive input validation**
8. **Correct TransactionType logic** - Based on user intent, not operation numbers ? **Implemented correctly**
9. **Use validation procedures** - ? **Validate entities before operations**
10. **Follow status code patterns** - ? **0=Success, 1=Warning, -1=Error**

## ?? **DEVELOPMENT STATUS: UNBLOCKED** ?

With the completion of Critical Fix #1, all database development activities are now unblocked:

- ? **12 Comprehensive Procedures**: All standard inventory operations available
- ? **Service Layer Ready**: Procedures designed for seamless integration
- ? **Error Handling**: Standardized pattern across all procedures
- ? **Documentation**: Complete with examples and integration guidance
- ? **Validation**: Entity validation procedures available
- ? **Transaction Management**: Proper transaction boundaries implemented
- ? **MTM Business Logic**: TransactionType correctly determined by user intent

**Next Phase Ready**: Service layer implementation can now proceed using these comprehensive stored procedures.