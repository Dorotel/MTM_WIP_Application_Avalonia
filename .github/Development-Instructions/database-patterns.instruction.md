# GitHub Copilot Instructions: Database Patterns for MTM WIP Application

You are implementing database access patterns for the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using .NET 8 with strict stored procedure requirements and MTM business rules.

## Critical Database Rules - Always Follow

### ? IMPLEMENTED - Use ONLY stored procedures via Helper_Database_StoredProcedure:
```csharp
// ? CORRECT: Use Helper_Database_StoredProcedure implementation
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Get_ByPartID",
    new Dictionary<string, object> { ["p_PartID"] = partId }
);

// ? PROHIBITED: Never write direct SQL queries
// var sql = "SELECT * FROM inventory WHERE part_id = @partId"; // NEVER DO THIS
```

### ? IMPLEMENTED - Helper_Database_StoredProcedure Class
Located in `Services/Helper_Database_StoredProcedure.cs`, provides:

**Security Features**:
- ? SQL injection prevention with `IsSqlQuery()` validation
- ? Stored procedure name validation
- ? Parameter sanitization
- ? Security violation logging

**Core Methods**:
- ? `ExecuteDataTableWithStatus()` - Primary method for data retrieval with status
- ? `ExecuteNonQuery()` - For INSERT/UPDATE/DELETE operations
- ? `ExecuteScalar<T>()` - For single value returns
- ? Comprehensive error handling with logging
- ? MySQL connection management with retry logic

**Usage Pattern**:
```csharp
// ? Initialize logger during app startup (in App.axaml.cs)
var loggerFactory = Program.GetService<ILoggerFactory>();
var generalLogger = loggerFactory.CreateLogger("Helper_Database_StoredProcedure");
Helper_Database_StoredProcedure.SetLogger(generalLogger);

// ? Use throughout application
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item",
    parameters);
```

### TransactionType Business Logic (CRITICAL):
Determine TransactionType by USER INTENT, not operation numbers:
```csharp
// ? CORRECT: Based on what user is doing
public TransactionType DetermineTransactionType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => TransactionType.IN,      // User adding inventory
        UserIntent.RemovingStock => TransactionType.OUT,   // User removing inventory
        UserIntent.MovingStock => TransactionType.TRANSFER // User moving between locations
    };
}

// ? WRONG: Never determine from operation numbers
// if (operation == "90") return TransactionType.IN; // Operations are workflow steps!
```

### ? IMPLEMENTED - MTM Data Patterns:
```csharp
// ? MTM business object structure (in CoreModels.cs)
public class InventoryItem
{
    public string PartID { get; set; } = string.Empty;    // "PART001", "ABC-123"
    public string? Operation { get; set; }                // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                     // Integer count only
    public string Location { get; set; } = string.Empty; // Location identifier
    public string ItemType { get; set; } = "WIP";        // Default item type
    public string User { get; set; } = string.Empty;     // User performing operation
    // ... other properties
}

public class InventoryTransaction
{
    public TransactionType TransactionType { get; set; }  // IN, OUT, TRANSFER (based on user intent)
    public string PartID { get; set; } = string.Empty;    // Part identifier
    public string? Operation { get; set; }                // Workflow step number
    // ... other properties
}

// ? Operation numbers are workflow steps, NOT transaction indicators
var workflowSteps = new[] { "90", "100", "110", "120" }; // String numbers for manufacturing steps
```

## ? IMPLEMENTED - Service Implementation Patterns

### Complete InventoryService Implementation:
```csharp
// ? IMPLEMENTED in Services/InventoryService.cs
namespace MTM.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IValidationService _validationService;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IDatabaseService databaseService,
            IValidationService validationService,
            ILogger<InventoryService> logger)
        {
            _databaseService = databaseService;
            _validationService = validationService;
            _logger = logger;
        }

        public async Task<MTM.Models.Result<List<InventoryItem>>> GetInventoryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving all inventory items via stored procedure");

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    Model_AppVariables.ConnectionString,
                    "inv_inventory_Get_All",
                    null);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve inventory items: {Error}", result.ErrorMessage);
                    return MTM.Models.Result<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Failed to retrieve inventory");
                }

                // Convert DataTable to List<InventoryItem>
                var inventoryItems = ConvertDataTableToInventoryItems(result.Data);
                return MTM.Models.Result<List<InventoryItem>>.Success(inventoryItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve inventory items");
                return MTM.Models.Result<List<InventoryItem>>.Failure($"Failed to retrieve inventory: {ex.Message}");
            }
        }

        // ? Additional methods: AddInventoryItemAsync, RemoveInventoryItemAsync, etc.
    }
}
```

## ? IMPLEMENTED - Transaction Type Implementation

### Always implement based on user intent:
```csharp
// ? Add inventory operation - always IN (user intent: adding stock)
public async Task<MTM.Models.Result> AddInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
{
    var parameters = new Dictionary<string, object>
    {
        ["p_PartID"] = item.PartID,
        ["p_Location"] = item.Location,
        ["p_Operation"] = item.Operation ?? string.Empty, // Workflow step (just a number)
        ["p_Quantity"] = item.Quantity,
        ["p_ItemType"] = item.ItemType,
        ["p_User"] = item.User,
        ["p_Notes"] = item.Notes ?? string.Empty
        // TransactionType is determined by the stored procedure based on operation context
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "inv_inventory_Add_Item", // This procedure handles IN transaction logic
        parameters);

    return result.IsSuccess 
        ? MTM.Models.Result.Success() 
        : MTM.Models.Result.Failure(result.ErrorMessage ?? "Failed to add inventory item");
}

// ? Remove inventory operation - always OUT (user intent: removing stock)
public async Task<MTM.Models.Result> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string userId, string? notes = null, CancellationToken cancellationToken = default)
{
    var parameters = new Dictionary<string, object>
    {
        ["p_PartID"] = partId,
        ["p_Location"] = location,
        ["p_Operation"] = operation,      // Workflow step (just a number)
        ["p_Quantity"] = quantity,
        ["p_ItemType"] = "WIP",
        ["p_User"] = userId,
        ["p_BatchNumber"] = string.Empty,
        ["p_Notes"] = notes ?? string.Empty
        // Stored procedure determines this is OUT transaction
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "inv_inventory_Remove_Item", // This procedure handles OUT transaction logic
        parameters);

    // Check status from stored procedure
    var status = result.GetOutputParameter<int>("p_Status");
    if (status != 0)
    {
        var errorMsg = result.GetOutputParameter<string>("p_ErrorMsg") ?? "Unknown error occurred";
        return MTM.Models.Result.Failure(errorMsg);
    }

    return MTM.Models.Result.Success();
}

// ? Transfer inventory operation - always TRANSFER (user intent: moving stock)
public async Task<MTM.Models.Result> TransferInventoryAsync(string partId, string operation, int quantity, string fromLocation, string toLocation, string userId, CancellationToken cancellationToken = default)
{
    // Get current item batch number for transfer
    var currentItemResult = await GetInventoryItemAsync(partId, cancellationToken);
    if (!currentItemResult.IsSuccess || currentItemResult.Value == null)
    {
        return MTM.Models.Result.Failure("Part not found in inventory");
    }

    var parameters = new Dictionary<string, object>
    {
        ["in_BatchNumber"] = currentItemResult.Value.BatchNumber ?? string.Empty,
        ["in_PartID"] = partId,
        ["in_Operation"] = operation, // Workflow step (just a number)
        ["in_NewLocation"] = toLocation
        // Stored procedure determines this is TRANSFER transaction
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "inv_inventory_Transfer_Part", // This procedure handles TRANSFER transaction logic
        parameters);

    return result.IsSuccess 
        ? MTM.Models.Result.Success() 
        : MTM.Models.Result.Failure(result.ErrorMessage ?? "Failed to transfer inventory");
}
```

## ? IMPLEMENTED - Error Handling Standards

### Comprehensive error handling with DataTableWithStatus:
```csharp
public class DataTableWithStatus
{
    public DataTable Data { get; set; } = new DataTable();
    public int Status { get; set; }                    // 0 = success, non-zero = error
    public string? ErrorMessage { get; set; }
    public bool IsSuccess { get; set; }
    public int RowsAffected { get; set; }
    public Dictionary<string, object> OutputParameters { get; set; } = new Dictionary<string, object>();
    public Exception? Exception { get; set; }

    // ? Helper methods
    public static DataTableWithStatus Success(DataTable data, int rowsAffected = 0);
    public static DataTableWithStatus Failure(string errorMessage, Exception? exception = null);
    public T? GetOutputParameter<T>(string parameterName);
}
```

### ? Service-level error handling pattern:
```csharp
public async Task<MTM.Models.Result<T>> ExecuteDatabaseOperation<T>(string procedureName, Dictionary<string, object> parameters)
{
    try
    {
        _logger.LogDebug("Executing stored procedure: {ProcedureName}", procedureName);

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            procedureName,
            parameters
        );

        if (result.IsSuccess && result.Data != null)
        {
            var data = ConvertToType<T>(result.Data);
            _logger.LogDebug("Stored procedure executed successfully: {ProcedureName}, Rows: {RowCount}", 
                procedureName, result.RowsAffected);
            return MTM.Models.Result<T>.Success(data);
        }

        _logger.LogError("Stored procedure failed: {ProcedureName}, Error: {Error}", 
            procedureName, result.ErrorMessage);
        
        return MTM.Models.Result<T>.Failure(result.ErrorMessage ?? "Database operation failed");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception in stored procedure: {ProcedureName}", procedureName);
        return MTM.Models.Result<T>.Failure($"Database error: {ex.Message}");
    }
}
```

## ? AVAILABLE - 12 Comprehensive Stored Procedures

The following stored procedures are available for use via Helper_Database_StoredProcedure:

**Inventory Operations**:
- ? `inv_inventory_Add_Item` - Add new inventory item
- ? `inv_inventory_Remove_Item` - Remove inventory item with validation
- ? `inv_inventory_Get_ByPartID` - Get inventory by part ID
- ? `inv_inventory_Get_ByPartIDandOperation` - Get by part ID and operation
- ? `inv_inventory_Transfer_Part` - Transfer between locations
- ? `inv_inventory_Transfer_Quantity` - Transfer specific quantities

**Transaction Management**:
- ? `inv_transaction_Add` - Log inventory transactions
- ? `sys_last_10_transactions_Get_ByUser` - Get user's recent transactions

**Master Data**:
- ? `md_part_ids_Get_All` - Get all part definitions
- ? `md_locations_Get_All` - Get all locations
- ? `md_operation_numbers_Get_All` - Get all operation numbers

**System Operations**:
- ? `log_error_Add_Error` - Log application errors to database

## Development Workflow

### ? Configuration Management:
```csharp
// ? Connection string managed via Model_AppVariables
Model_AppVariables.ConnectionString // Automatically configured from appsettings.json

// ? Database settings available
Model_AppVariables.Database.CommandTimeout // 30 seconds default
Model_AppVariables.Database.MaxRetryAttempts // 3 attempts default
```

### When creating new stored procedures:
1. **Add to development file**: `Documentation/Development/Database_Files/New_Stored_Procedures.sql`
2. **Follow naming convention**: `{module}_{table}_{action}_By{Criteria}` 
   - Example: `inv_inventory_Get_ByPartId`
   - Example: `inv_transaction_Insert_WithValidation`
3. **Include standard parameters**:
   - Input validation with `p_` prefix
   - Output status: `p_Status INT` (0 = success)
   - Error message: `p_ErrorMsg VARCHAR(255)`
   - Row count: Use ROW_COUNT() where applicable

### ? Example stored procedure pattern (following existing conventions):
```sql
-- Add to Documentation/Development/Database_Files/New_Stored_Procedures.sql
DELIMITER $$

CREATE PROCEDURE `inv_inventory_Get_ByLocation`(
    IN p_Location VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        ROLLBACK;
    END;

    START TRANSACTION;

    -- Validate inputs
    IF p_Location IS NULL OR p_Location = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Location cannot be null or empty';
    ELSE
        -- Execute operation
        SELECT ID, PartID, Location, Operation, Quantity, ItemType, 
               ReceiveDate, LastUpdated, User, BatchNumber, Notes
        FROM inv_inventory 
        WHERE Location = p_Location
        ORDER BY PartID, Operation;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Success';
    END IF;

    COMMIT;
END$$

DELIMITER ;
```

## ? IMPLEMENTED - Validation Patterns

### Using IValidationService integration:
```csharp
// ? Validation integrated into services
public async Task<MTM.Models.Result> AddInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
{
    // Validate the inventory item
    var validationResult = await _validationService.ValidateAsync(item, cancellationToken);
    if (!validationResult.IsSuccess || !validationResult.Value!.IsValid)
    {
        var errors = string.Join(", ", validationResult.Value?.ErrorMessages ?? new List<string> { "Validation failed" });
        return MTM.Models.Result.Failure($"Validation failed: {errors}");
    }

    // Proceed with database operation...
}
```

## ? Ready for Phase 2

The database access layer is now complete and operational:

**? Phase 1 Complete**:
- Helper_Database_StoredProcedure implemented and tested
- All 12 stored procedures accessible
- Comprehensive error handling and logging
- Security enforcement (stored procedures only)
- Integration with Model_AppVariables and configuration

**? Phase 2 Ready**:
- UserService implementation using established patterns
- TransactionService implementation using Helper_Database_StoredProcedure
- Master Data Services using available stored procedures
- Enhanced business services with validation

## Never Do
- Write direct SQL queries in C# code
- Determine TransactionType from operation numbers
- Skip error handling in database operations
- Use hard-coded connection strings
- Implement business logic in stored procedures

## Always Do
- ? Use Helper_Database_StoredProcedure for all database access
- ? Base TransactionType on user intent (what the user is doing)
- ? Include comprehensive error handling and logging via ILogger<T>
- ? Validate inputs before database operations using IValidationService
- ? Follow MTM data patterns for Part IDs (strings) and Operations (string numbers)
- ? Use MTM.Models.Result<T> pattern for operation responses
- ? Initialize Helper_Database_StoredProcedure logger during app startup