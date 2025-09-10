---
description: 'Generate secure database operations using MySQL stored procedures with proper parameter handling and error management'
tools: ['codebase', 'search', 'usages', 'editFiles']
---

# MTM Database Operation Template

Create secure and efficient database operations for the MTM WIP Application using the established stored procedures pattern with MySQL 9.4.0.

## Database Operation Framework

### 1. Operation Analysis
- **Business Purpose**: Define the manufacturing business function this operation serves
- **Data Requirements**: Identify input parameters, validation rules, and expected outputs
- **Security Considerations**: Ensure SQL injection prevention and parameter validation
- **Performance Requirements**: Consider query optimization and transaction handling

### 2. Stored Procedure Integration
- **Procedure Selection**: Use existing stored procedures from the 45+ available procedures
- **Parameter Mapping**: Map C# types to MySQL parameter types securely
- **Result Processing**: Handle DataTable results and status codes properly
- **Error Handling**: Integrate with centralized error management system

## Database Operation Template

### Service Method Structure
```csharp
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public class [ServiceName]Service : I[ServiceName]Service
{
    private readonly ILogger<[ServiceName]Service> _logger;
    private readonly IConfigurationService _configurationService;

    public [ServiceName]Service(
        ILogger<[ServiceName]Service> logger,
        IConfigurationService configurationService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _logger = logger;
        _configurationService = configurationService;
    }

    /// <summary>
    /// [Operation Description] - [Business Purpose]
    /// Uses stored procedure: [procedure_name]
    /// </summary>
    /// <param name="request">Operation request containing validated parameters</param>
    /// <returns>Service result with operation outcome</returns>
    public async Task<ServiceResult<[ReturnType]>> [OperationName]Async([RequestModel] request)
    {
        try
        {
            // Validate input parameters
            if (request == null)
            {
                return ServiceResult<[ReturnType]>.Failure("Request cannot be null");
            }

            var validationResult = ValidateRequest(request);
            if (!validationResult.IsValid)
            {
                return ServiceResult<[ReturnType]>.Failure($"Validation failed: {validationResult.ErrorMessage}");
            }

            var connectionString = await _configurationService.GetConnectionStringAsync();
            
            // Create parameters array with proper types and validation
            var parameters = CreateParameters(request);
            
            // Execute stored procedure
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "[actual_stored_procedure_name]", // Use real procedure name from MTM database
                parameters
            );

            // Process results based on status code
            if (result.Status == 1)
            {
                // Success - process data
                var processedData = ProcessSuccessResult(result.Data);
                
                _logger.LogInformation("[OperationName] completed successfully. Processed {Count} records", 
                    result.Data.Rows.Count);
                
                return ServiceResult<[ReturnType]>.Success(processedData, result.Message);
            }
            else
            {
                // Database operation failed
                var errorMessage = $"Database operation failed with status: {result.Status}. {result.Message}";
                _logger.LogWarning("[OperationName] failed: {Error}", errorMessage);
                
                return ServiceResult<[ReturnType]>.Failure(errorMessage);
            }
        }
        catch (MySqlException ex)
        {
            // Database-specific errors
            _logger.LogError(ex, "[OperationName] database error: {Error}", ex.Message);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"[OperationName] database operation");
            
            return ServiceResult<[ReturnType]>.Failure("Database operation failed. Please try again.");
        }
        catch (Exception ex)
        {
            // General errors
            _logger.LogError(ex, "[OperationName] unexpected error: {Error}", ex.Message);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"[OperationName] operation");
            
            return ServiceResult<[ReturnType]>.Failure("An unexpected error occurred. Please try again.");
        }
    }

    /// <summary>
    /// Create MySQL parameters with proper validation and type mapping
    /// </summary>
    private MySqlParameter[] CreateParameters([RequestModel] request)
    {
        return new MySqlParameter[]
        {
            // String parameters with null/empty validation
            new("p_PartID", MySqlDbType.VarChar, 50) 
            { 
                Value = string.IsNullOrWhiteSpace(request.PartId) ? DBNull.Value : request.PartId.Trim().ToUpperInvariant() 
            },
            
            // Operation parameter (manufacturing workflow step)
            new("p_Operation", MySqlDbType.VarChar, 10) 
            { 
                Value = string.IsNullOrWhiteSpace(request.Operation) ? DBNull.Value : request.Operation.Trim() 
            },
            
            // Integer parameters with range validation
            new("p_Quantity", MySqlDbType.Int32) 
            { 
                Value = Math.Max(1, Math.Min(999999, request.Quantity)) 
            },
            
            // Location parameter
            new("p_Location", MySqlDbType.VarChar, 20) 
            { 
                Value = string.IsNullOrWhiteSpace(request.Location) ? DBNull.Value : request.Location.Trim() 
            },
            
            // User tracking (always include for audit trail)
            new("p_User", MySqlDbType.VarChar, 50) 
            { 
                Value = string.IsNullOrWhiteSpace(request.UserId) ? Environment.UserName : request.UserId 
            },
            
            // Timestamp tracking
            new("p_Timestamp", MySqlDbType.DateTime) 
            { 
                Value = request.Timestamp == default ? DateTime.Now : request.Timestamp 
            },
            
            // Transaction type (determined by user intent, NOT operation numbers)
            new("p_TransactionType", MySqlDbType.VarChar, 20) 
            { 
                Value = DetermineTransactionType(request.UserIntent) 
            }
        };
    }

    /// <summary>
    /// Validate request parameters against business rules
    /// </summary>
    private ValidationResult ValidateRequest([RequestModel] request)
    {
        // Part ID validation
        if (string.IsNullOrWhiteSpace(request.PartId))
        {
            return ValidationResult.Invalid("Part ID is required");
        }

        if (request.PartId.Length > 50)
        {
            return ValidationResult.Invalid("Part ID cannot exceed 50 characters");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(request.PartId, @"^[A-Z0-9\-]+$"))
        {
            return ValidationResult.Invalid("Part ID can only contain uppercase letters, numbers, and hyphens");
        }

        // Operation validation (manufacturing workflow steps)
        var validOperations = new[] { "90", "100", "110", "120", "130" };
        if (!validOperations.Contains(request.Operation))
        {
            return ValidationResult.Invalid($"Invalid operation. Must be one of: {string.Join(", ", validOperations)}");
        }

        // Quantity validation
        if (request.Quantity < 1 || request.Quantity > 999999)
        {
            return ValidationResult.Invalid("Quantity must be between 1 and 999,999");
        }

        // Location validation
        if (string.IsNullOrWhiteSpace(request.Location))
        {
            return ValidationResult.Invalid("Location is required");
        }

        if (request.Location.Length > 20)
        {
            return ValidationResult.Invalid("Location cannot exceed 20 characters");
        }

        return ValidationResult.Valid();
    }

    /// <summary>
    /// Process successful database result into business objects
    /// </summary>
    private [ReturnType] ProcessSuccessResult(DataTable dataTable)
    {
        if (dataTable.Rows.Count == 0)
        {
            return new [ReturnType](); // Return empty result, NO fallback data
        }

        var items = new List<[ItemModel]>();
        
        foreach (DataRow row in dataTable.Rows)
        {
            try
            {
                var item = new [ItemModel]
                {
                    // ✅ CRITICAL: Use exact database column names (not assumed names)
                    PartId = row["PartID"].ToString() ?? string.Empty,  // Database column: "PartID"
                    Operation = row["OperationNumber"].ToString() ?? string.Empty,  // Database column: "OperationNumber"
                    Quantity = Convert.ToInt32(row["Quantity"]),  // Database column: "Quantity"
                    Location = row["Location"].ToString() ?? string.Empty,  // Database column: "Location"
                    
                    // User column special case (database column "User", property "User_Name" to avoid conflicts)
                    User_Name = row["User"].ToString() ?? string.Empty,  // Database column: "User"
                    
                    // Date handling with proper conversion
                    LastUpdated = row["LastUpdated"] != DBNull.Value 
                        ? Convert.ToDateTime(row["LastUpdated"]) 
                        : DateTime.MinValue,
                    
                    // Transaction type (business logic, not derived from operation)
                    TransactionType = row["TransactionType"].ToString() ?? string.Empty,
                    
                    // Additional fields as needed
                    Notes = row["Notes"].ToString() ?? string.Empty,
                    IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"])
                };
                
                items.Add(item);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to process data row, skipping. Error: {Error}", ex.Message);
                // Continue processing other rows rather than failing entire operation
            }
        }

        return new [ReturnType] { Items = items, TotalCount = items.Count };
    }

    /// <summary>
    /// Determine transaction type based on user intent (NOT operation numbers)
    /// </summary>
    private string DetermineTransactionType(UserIntent intent)
    {
        return intent switch
        {
            UserIntent.AddingInventory => "IN",      // User adding inventory to system
            UserIntent.RemovingInventory => "OUT",   // User removing inventory from system  
            UserIntent.TransferringInventory => "TRANSFER", // User moving inventory between locations/operations
            UserIntent.AdjustingInventory => "ADJUSTMENT",  // User correcting inventory quantities
            _ => "UNKNOWN"
        };
    }
}
```

## MTM Stored Procedure Reference

### Inventory Operations
```csharp
// Available inventory stored procedures (use exact names)
public static class InventoryProcedures
{
    // Primary inventory operations
    public const string AddItem = "inv_inventory_Add_Item";
    public const string RemoveItem = "inv_inventory_Remove_Item";
    public const string GetByPartID = "inv_inventory_Get_ByPartID";
    public const string GetByPartIDAndOperation = "inv_inventory_Get_ByPartIDandOperation";
    public const string GetCurrentQuantity = "inv_inventory_Get_CurrentQty_ByPartIDandOperation";
    public const string UpdateQuantity = "inv_inventory_Update_Quantity";
    public const string TransferBetweenOperations = "inv_inventory_Transfer_Between_Operations";
    public const string GetAll = "inv_inventory_Get_All";
    public const string GetLowStock = "inv_inventory_Get_LowStock";
    public const string DeleteItem = "inv_inventory_Delete_Item";
}

// Example usage with proper procedure selection
public async Task<ServiceResult<List<InventoryItem>>> GetInventoryByPartAsync(string partId, string? operation = null)
{
    var procedureName = string.IsNullOrEmpty(operation) 
        ? InventoryProcedures.GetByPartID 
        : InventoryProcedures.GetByPartIDAndOperation;
        
    var parameters = string.IsNullOrEmpty(operation)
        ? new MySqlParameter[] { new("p_PartID", partId) }
        : new MySqlParameter[] { new("p_PartID", partId), new("p_Operation", operation) };
    
    // Execute with selected procedure and parameters
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString, procedureName, parameters);
    
    // Process result...
}
```

### Transaction Operations
```csharp
public static class TransactionProcedures
{
    public const string AddTransaction = "inv_transaction_Add";
    public const string GetHistory = "inv_transaction_Get_History";
    public const string GetByPartID = "inv_transaction_Get_ByPartID";
    public const string GetByUser = "inv_transaction_Get_ByUser";
    public const string GetByDateRange = "inv_transaction_Get_ByDateRange";
    public const string GetRecent = "inv_transaction_Get_Recent";
    public const string CancelTransaction = "inv_transaction_Cancel";
    public const string GetSummary = "inv_transaction_Get_Summary";
}
```

### Master Data Operations
```csharp
public static class MasterDataProcedures
{
    // Part IDs
    public const string GetAllPartIds = "md_part_ids_Get_All";
    public const string AddPartId = "md_part_ids_Add";
    public const string UpdatePartId = "md_part_ids_Update";
    public const string DeletePartId = "md_part_ids_Delete";
    public const string SearchPartIds = "md_part_ids_Search";
    public const string GetActivePartIds = "md_part_ids_Get_Active";
    
    // Locations
    public const string GetAllLocations = "md_locations_Get_All";
    public const string AddLocation = "md_locations_Add";
    public const string UpdateLocation = "md_locations_Update";
    public const string DeleteLocation = "md_locations_Delete";
    public const string GetActiveLocations = "md_locations_Get_Active";
    
    // Operations
    public const string GetAllOperations = "md_operation_numbers_Get_All";
    public const string AddOperation = "md_operation_numbers_Add";
    public const string UpdateOperation = "md_operation_numbers_Update";
    public const string DeleteOperation = "md_operation_numbers_Delete";
    public const string GetActiveOperations = "md_operation_numbers_Get_Active";
}
```

## Security and Best Practices

### SQL Injection Prevention
```csharp
// ✅ CORRECT: Always use MySqlParameter for all values
var parameters = new MySqlParameter[]
{
    new("p_PartID", MySqlDbType.VarChar, 50) { Value = partId },
    new("p_Quantity", MySqlDbType.Int32) { Value = quantity }
};

// ❌ WRONG: Never use string concatenation or interpolation
string sql = $"CALL some_procedure('{partId}', {quantity})"; // SQL injection risk!
```

### Column Name Validation
```csharp
// ✅ CORRECT: Use exact database column names
public InventoryItem MapFromDataRow(DataRow row)
{
    return new InventoryItem
    {
        // Always use documented column names from database schema
        PartId = row["PartID"].ToString() ?? string.Empty,  // Column: "PartID"
        Operation = row["OperationNumber"].ToString() ?? string.Empty,  // Column: "OperationNumber"
        User_Name = row["User"].ToString() ?? string.Empty  // Column: "User", Property: "User_Name"
    };
}

// ❌ WRONG: Assuming column names without verification
var partId = row["part_id"].ToString(); // May not exist - will throw ArgumentException
```

### No Fallback Data Pattern
```csharp
// ✅ CORRECT: Return empty collections on database failure
public async Task<List<InventoryItem>> GetInventoryAsync()
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString, "inv_inventory_Get_All", Array.Empty<MySqlParameter>());

        if (result.Status == 1)
        {
            return ProcessDataTable(result.Data);
        }

        // Database operation failed - return empty, NO fallback data
        await Services.ErrorHandling.HandleErrorAsync(
            new InvalidOperationException($"Database operation failed with status: {result.Status}"),
            "Failed to retrieve inventory data");
        
        return new List<InventoryItem>(); // Empty list, not dummy data
    }
    catch (Exception ex)
    {
        await Services.ErrorHandling.HandleErrorAsync(ex, "Database connection failed");
        return new List<InventoryItem>(); // Empty list on any error
    }
}

// ❌ WRONG: Providing fallback data masks database issues
if (result.Status != 1)
{
    return new List<InventoryItem> { new() { PartId = "DEFAULT", Quantity = 0 } }; // Don't do this!
}
```

## Error Handling Integration

### Centralized Error Management
```csharp
try
{
    var result = await ExecuteDatabaseOperationAsync(request);
    return result;
}
catch (MySqlException ex) when (ex.Number == 1205) // Deadlock
{
    _logger.LogWarning(ex, "Database deadlock detected, retrying operation");
    await Task.Delay(TimeSpan.FromMilliseconds(100)); // Brief delay
    return await ExecuteDatabaseOperationAsync(request); // Single retry
}
catch (MySqlException ex)
{
    _logger.LogError(ex, "MySQL error in {Operation}: {Error}", nameof(ExecuteDatabaseOperationAsync), ex.Message);
    await Services.ErrorHandling.HandleErrorAsync(ex, "Database operation");
    return ServiceResult<T>.Failure("Database operation failed. Please try again.");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error in {Operation}: {Error}", nameof(ExecuteDatabaseOperationAsync), ex.Message);
    await Services.ErrorHandling.HandleErrorAsync(ex, "Database operation");
    return ServiceResult<T>.Failure("An unexpected error occurred. Please contact support.");
}
```

## Performance Optimization

### Connection Management
```csharp
// Connection string retrieved from configuration service (managed connection pooling)
var connectionString = await _configurationService.GetConnectionStringAsync();

// Helper_Database_StoredProcedure handles connection lifecycle automatically
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString, procedureName, parameters);
```

### Batch Operations
```csharp
public async Task<ServiceResult<bool>> ProcessBatchOperationsAsync(List<InventoryRequest> requests)
{
    const int batchSize = 100;
    var batches = requests.Chunk(batchSize);
    
    foreach (var batch in batches)
    {
        var batchParameters = batch.Select(CreateParameters).ToArray();
        
        // Process batch with single database call
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString, "inv_inventory_Process_Batch", CreateBatchParameters(batchParameters));
        
        if (result.Status != 1)
        {
            return ServiceResult<bool>.Failure($"Batch processing failed: {result.Message}");
        }
    }
    
    return ServiceResult<bool>.Success(true);
}
```

Use this template when creating database operations that require secure parameter handling, proper error management, and integration with the MTM stored procedure architecture.