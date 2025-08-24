# Database Access Patterns

## **CRITICAL DATABASE RULE: NO HARD-CODED MYSQL**

**All MySQL commands MUST be executed through stored procedures only.**

- **Prohibited**: Direct SQL queries in code (SELECT, INSERT, UPDATE, DELETE)
- **Required**: Use stored procedure calls via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- **Exception**: Schema operations during development (handled by database scripts)

## **Correct Database Access Pattern**

```csharp
// ? CORRECT: Using stored procedure
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sp_ProcedureName",
    new Dictionary<string, object> 
    {
        ["Parameter1"] = value1,
        ["Parameter2"] = value2
    }
);

// ? PROHIBITED: Direct SQL
// var query = "SELECT * FROM table WHERE column = @value"; // NEVER DO THIS
```

## **MTM Business Logic Rules**

### **CRITICAL: Transaction Type Determination**
**TransactionType is determined by the USER'S INTENT, NOT the Operation number.**

#### **Correct TransactionType Logic**
- **IN**: User is adding stock to inventory (regardless of operation number)
- **OUT**: User is removing stock from inventory (regardless of operation number)
- **TRANSFER**: User is moving stock from one location to another (regardless of operation number)

#### **Incorrect Pattern (DO NOT USE)**
```csharp
// ? WRONG - DO NOT determine TransactionType from Operation
private static TransactionType GetTransactionType(string operation)
{
    return operation switch
    {
        "90" => TransactionType.IN,    // Wrong!
        "100" => TransactionType.OUT,  // Wrong!
        "110" => TransactionType.TRANSFER, // Wrong!
        _ => TransactionType.OTHER
    };
}
```

#### **Correct Pattern**
```csharp
// ? CORRECT - TransactionType based on user action
public async Task<Result> AddStockAsync(string partId, string operation, int quantity, string location, string userId)
{
    // Always IN when adding stock
    var transaction = new InventoryTransaction
    {
        TransactionType = TransactionType.IN, // User is adding stock
        Operation = operation, // Operation is just a workflow step number
        // ... other properties
    };
}

public async Task<Result> RemoveStockAsync(string partId, string operation, int quantity, string location, string userId)
{
    // Always OUT when removing stock
    var transaction = new InventoryTransaction
    {
        TransactionType = TransactionType.OUT, // User is removing stock
        Operation = operation, // Operation is just a workflow step number
        // ... other properties
    };
}

public async Task<Result> TransferStockAsync(string partId, string operation, int quantity, string fromLocation, string toLocation, string userId)
{
    // Always TRANSFER when moving stock
    var transaction = new InventoryTransaction
    {
        TransactionType = TransactionType.TRANSFER, // User is moving stock
        Operation = operation, // Operation is just a workflow step number
        FromLocation = fromLocation,
        ToLocation = toLocation,
        // ... other properties
    };
}
```

#### **MTM Operation Numbers**
Operation numbers ("90", "100", "110", etc.) are **workflow step identifiers**, NOT transaction type indicators:
- Operations represent manufacturing or processing steps
- They help track which stage of production a part is in
- They do NOT determine whether inventory is being added, removed, or transferred
- The same operation number can be used with any TransactionType depending on user intent

## **Database Development Workflow**

### **New Stored Procedures**
1. **Create in development file**: Add to `Development/Database_Files/New_Stored_Procedures.sql`
2. **Follow naming convention**: `{module}_{action}_{details}` (e.g., `inv_inventory_Get_ByPartID`)
3. **Include error handling**: Standard error handling pattern in all procedures
4. **Test thoroughly**: Validate with different parameter combinations
5. **Document parameters**: Clear documentation for all input/output parameters

### **Updating Existing Procedures**
1. **Copy to development file**: Add to `Development/Database_Files/Updated_Stored_Procedures.sql`
2. **Mark as modified**: Include original procedure name and modification date
3. **Preserve compatibility**: Ensure existing calls continue to work
4. **Version tracking**: Include version comments in procedure

### **Service Integration**
```csharp
public async Task<DataResult<List<InventoryItem>>> GetInventoryAsync(string partId = null)
{
    try
    {
        var parameters = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(partId))
        {
            parameters["PartID"] = partId;
        }

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Get_ByPartID",
            parameters
        );

        if (result.IsSuccess && result.Data != null)
        {
            var items = DataTableToInventoryItems(result.Data);
            return DataResult<List<InventoryItem>>.Success(items);
        }

        return DataResult<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Unknown database error");
    }
    catch (Exception ex)
    {
        // Log error via stored procedure
        await LogErrorAsync(ex, nameof(GetInventoryAsync));
        return DataResult<List<InventoryItem>>.Failure($"Database operation failed: {ex.Message}");
    }
}
```

## **Custom Database Prompts**

### **Common Database Operation Prompts**
1. **"Create stored procedure for [operation]"** - Generate new SP in `Development/Database_Files/New_Stored_Procedures.sql`
2. **"Update existing stored procedure [name]"** - Copy to `Updated_Stored_Procedures.sql` and modify  
3. **"Create CRUD operations for [table]"** - Generate full set of Create, Read, Update, Delete procedures
4. **"Add error handling to stored procedure"** - Implement standard error handling pattern
5. **"Create data access service for [entity]"** - Generate service class with stored procedure calls

### **Database Documentation Prompts**
1. **"Document database schema"** - Update README files with table relationships and column descriptions
2. **"Explain stored procedure [name]"** - Generate detailed documentation for procedure functionality
3. **"Create database migration script"** - Generate script to update production schema
4. **"Document data flow for [process]"** - Explain how data moves through system tables

## **Error Handling in Database Operations**

### **Standard Error Handling Pattern**
```csharp
public async Task LogErrorAsync(Exception ex, string methodName)
{
    try
    {
        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "sys_error_Log_Insert",
            new Dictionary<string, object>
            {
                ["ErrorMessage"] = ex.Message,
                ["StackTrace"] = ex.StackTrace,
                ["MethodName"] = methodName,
                ["Timestamp"] = DateTime.UtcNow,
                ["UserId"] = GetCurrentUserId()
            }
        );
    }
    catch
    {
        // Fall back to file logging if database logging fails
        await LogToFileAsync(ex, methodName);
    }
}
```

### **Database Validation Patterns**
```csharp
public async Task<ValidationResult> ValidateInventoryOperationAsync(string partId, string operation, int quantity)
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "inv_validation_Validate_Operation",
        new Dictionary<string, object>
        {
            ["PartID"] = partId,
            ["Operation"] = operation,
            ["Quantity"] = quantity
        }
    );

    if (result.IsSuccess && result.Data?.Rows.Count > 0)
    {
        var row = result.Data.Rows[0];
        return new ValidationResult
        {
            IsValid = Convert.ToBoolean(row["IsValid"]),
            ErrorMessage = row["ErrorMessage"]?.ToString(),
            ValidationDetails = row["ValidationDetails"]?.ToString()
        };
    }

    return ValidationResult.Failure("Database validation failed");
}
```

## **Process Continuation Rule**

**When stopping current process, must notify in:**
# ???? **STOPPING CURRENT PROCESS** ????

This ensures proper handoff and context preservation for continued work.