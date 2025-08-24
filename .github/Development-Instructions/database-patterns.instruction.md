# GitHub Copilot Instructions: Database Patterns for MTM WIP Application

You are implementing database access patterns for the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using .NET 8 with strict stored procedure requirements and MTM business rules.

## Critical Database Rules - Always Follow

### Use ONLY stored procedures - NEVER direct SQL:
```csharp
// CORRECT: Use stored procedures only
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sp_GetInventoryByPart",
    new Dictionary<string, object> { ["PartId"] = partId }
);

// PROHIBITED: Never write direct SQL queries
// var sql = "SELECT * FROM inventory WHERE part_id = @partId"; // NEVER DO THIS
```

### TransactionType Business Logic (CRITICAL):
Determine TransactionType by USER INTENT, not operation numbers:
```csharp
// CORRECT: Based on what user is doing
public TransactionType DetermineTransactionType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => TransactionType.IN,      // User adding inventory
        UserIntent.RemovingStock => TransactionType.OUT,   // User removing inventory
        UserIntent.MovingStock => TransactionType.TRANSFER // User moving between locations
    };
}

// WRONG: Never determine from operation numbers
// if (operation == "90") return TransactionType.IN; // Operations are workflow steps!
```

### MTM Data Patterns:
```csharp
// MTM business object structure
public class InventoryTransaction
{
    public string PartId { get; set; } = string.Empty;    // "PART001", "ABC-123"
    public string Operation { get; set; } = string.Empty; // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                     // Integer count only
    public TransactionType Type { get; set; }             // Based on user intent
    public string Location { get; set; } = string.Empty;  // Location identifier
}

// Operation numbers are workflow steps, NOT transaction indicators
var workflowSteps = new[] { "90", "100", "110", "120" }; // String numbers for manufacturing steps
```

## Service Implementation Patterns

### Generate database services using this template:
```csharp
namespace MTM_WIP_Application_Avalonia.Services;

public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(ILogger<InventoryService> logger)
    {
        _logger = logger;
    }

    public async Task<Result<List<InventoryItem>>> GetInventoryAsync(string? partId = null)
    {
        try
        {
            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(partId))
            {
                parameters["PartId"] = partId;
            }

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "sp_GetInventoryByPart",
                parameters
            );

            if (result.IsSuccess && result.Data != null)
            {
                var items = ConvertToInventoryItems(result.Data);
                return Result<List<InventoryItem>>.Success(items);
            }

            return Result<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Database operation failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get inventory for PartId: {PartId}", partId);
            await LogErrorAsync(ex, nameof(GetInventoryAsync));
            return Result<List<InventoryItem>>.Failure($"Database error: {ex.Message}");
        }
    }

    private async Task LogErrorAsync(Exception ex, string methodName)
    {
        try
        {
            await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "sp_LogError",
                new Dictionary<string, object>
                {
                    ["ErrorMessage"] = ex.Message,
                    ["StackTrace"] = ex.StackTrace,
                    ["MethodName"] = methodName,
                    ["Timestamp"] = DateTime.UtcNow
                }
            );
        }
        catch
        {
            // Fallback to file logging if database fails
        }
    }
}
```

## Transaction Type Implementation

### Always implement based on user intent:
```csharp
// Add inventory operation - always IN
public async Task<Result> AddInventoryAsync(string partId, string operation, int quantity, string location)
{
    var parameters = new Dictionary<string, object>
    {
        ["PartId"] = partId,
        ["Operation"] = operation,      // Workflow step (just a number)
        ["Quantity"] = quantity,
        ["Location"] = location,
        ["TransactionType"] = "IN",     // User is adding - always IN
        ["UserId"] = GetCurrentUserId(),
        ["Timestamp"] = DateTime.UtcNow
    };

    return await ExecuteInventoryOperation("sp_AddInventory", parameters);
}

// Remove inventory operation - always OUT
public async Task<Result> RemoveInventoryAsync(string partId, string operation, int quantity, string location)
{
    var parameters = new Dictionary<string, object>
    {
        ["PartId"] = partId,
        ["Operation"] = operation,      // Workflow step (just a number)
        ["Quantity"] = quantity,
        ["Location"] = location,
        ["TransactionType"] = "OUT",    // User is removing - always OUT
        ["UserId"] = GetCurrentUserId(),
        ["Timestamp"] = DateTime.UtcNow
    };

    return await ExecuteInventoryOperation("sp_RemoveInventory", parameters);
}

// Transfer inventory operation - always TRANSFER
public async Task<Result> TransferInventoryAsync(string partId, string operation, int quantity, string fromLocation, string toLocation)
{
    var parameters = new Dictionary<string, object>
    {
        ["PartId"] = partId,
        ["Operation"] = operation,      // Workflow step (just a number)
        ["Quantity"] = quantity,
        ["FromLocation"] = fromLocation,
        ["ToLocation"] = toLocation,
        ["TransactionType"] = "TRANSFER", // User is moving - always TRANSFER
        ["UserId"] = GetCurrentUserId(),
        ["Timestamp"] = DateTime.UtcNow
    };

    return await ExecuteInventoryOperation("sp_TransferInventory", parameters);
}
```

## Error Handling Standards

### Implement comprehensive error handling:
```csharp
public async Task<Result<T>> ExecuteDatabaseOperation<T>(string procedureName, Dictionary<string, object> parameters)
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            procedureName,
            parameters
        );

        if (result.IsSuccess && result.Data != null)
        {
            var data = ConvertToType<T>(result.Data);
            return Result<T>.Success(data);
        }

        _logger.LogWarning("Database operation failed: {ProcedureName}, Error: {Error}", 
            procedureName, result.ErrorMessage);
        
        return Result<T>.Failure(result.ErrorMessage ?? "Database operation failed");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception in database operation: {ProcedureName}", procedureName);
        await LogErrorToDatabase(ex, procedureName);
        return Result<T>.Failure($"Database error: {ex.Message}");
    }
}
```

## Development Workflow

### When creating new stored procedures:
1. **Add to development file**: `Development/Database_Files/New_Stored_Procedures.sql`
2. **Follow naming convention**: `{module}_{table}_{action}_By{Criteria}` 
   - Example: `inv_inventory_Get_ByPartId`
   - Example: `inv_transaction_Insert_WithValidation`
3. **Include standard parameters**:
   - Input validation
   - Output status codes
   - Error message handling
   - Audit trail fields

### Example stored procedure pattern:
```sql
-- Add to Development/Database_Files/New_Stored_Procedures.sql
DELIMITER $$

CREATE PROCEDURE `inv_inventory_Get_ByPartId`(
    IN p_PartId VARCHAR(50),
    OUT p_StatusCode INT,
    OUT p_ErrorMessage TEXT,
    OUT p_RowsAffected INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_StatusCode = MYSQL_ERRNO,
            p_ErrorMessage = MESSAGE_TEXT;
        SET p_RowsAffected = 0;
    END;

    -- Validate inputs
    IF p_PartId IS NULL OR p_PartId = '' THEN
        SET p_StatusCode = 1001;
        SET p_ErrorMessage = 'PartId cannot be null or empty';
        SET p_RowsAffected = 0;
    ELSE
        -- Execute operation
        SELECT * FROM inventory WHERE part_id = p_PartId;
        
        SET p_StatusCode = 0;
        SET p_ErrorMessage = 'Success';
        SET p_RowsAffected = ROW_COUNT();
    END IF;
END$$

DELIMITER ;
```

## Validation Patterns

### Always validate before database operations:
```csharp
public async Task<ValidationResult> ValidateInventoryOperation(string partId, string operation, int quantity)
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "sp_ValidateInventoryOperation",
        new Dictionary<string, object>
        {
            ["PartId"] = partId,
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

    return ValidationResult.Failure("Validation failed");
}
```

## Never Do
- Write direct SQL queries in C# code
- Determine TransactionType from operation numbers
- Skip error handling in database operations
- Use hard-coded connection strings
- Implement business logic in stored procedures

## Always Do
- Use stored procedures for all database access
- Base TransactionType on user intent
- Include comprehensive error handling and logging
- Validate inputs before database operations
- Follow MTM data patterns for Part IDs and Operations
- Use Result<T> pattern for operation responses