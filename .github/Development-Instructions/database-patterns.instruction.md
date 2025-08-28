# Database Patterns and Architecture

<details>
<summary><strong>🎯 MTM Database Architecture</strong></summary>

The MTM WIP Application uses a **stored procedure only** approach with comprehensive error handling and logging.

### **Core Principles**
- **Stored Procedures Only**: No direct SQL queries allowed
- **Standardized Result Pattern**: All procedures return status and message
- **Comprehensive Logging**: All database operations logged
- **Error Handling**: Integrated with ErrorHandling service
- **Connection Management**: Centralized through Database service

</details>

<details>
<summary><strong>📋 Database Service Architecture</strong></summary>

### **Consolidated Database Service**
**File: `Services/Database.cs`**

```csharp
namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Database service interface for MTM operations.
/// </summary>
public interface IDatabaseService
{
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<bool> TestConnectionAsync();
}

/// <summary>
/// Comprehensive database service for MTM WIP Application.
/// Provides centralized database access with error handling and logging.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IConfigurationService configurationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connectionString = configurationService?.GetConnectionString() ?? throw new ArgumentNullException(nameof(configurationService));
        
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured");
        }

        _logger.LogInformation("DatabaseService initialized with connection string");
    }
    
    // Implementation includes comprehensive error handling and logging
}

/// <summary>
/// Helper class for stored procedure execution with status return.
/// Maintains compatibility with existing Helper_Database_StoredProcedure usage.
/// </summary>
public static class Helper_Database_StoredProcedure
{
    private static ILogger? _logger;

    public static void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes a stored procedure with status return - MTM standard pattern.
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteDataTableWithStatus(
        string connectionString, 
        string procedureName, 
        Dictionary<string, object> parameters)
    {
        // Implementation with comprehensive error handling
    }
}

/// <summary>
/// Standard result structure for stored procedure execution.
/// </summary>
public class StoredProcedureResult
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DataTable Data { get; set; } = new DataTable();

    public bool IsSuccess => Status == 0;
}
```

</details>

<details>
<summary><strong>🚨 CRITICAL: Stored Procedure Only Pattern</strong></summary>

### **ALWAYS Use Stored Procedures**
```csharp
// ✅ CORRECT: Use stored procedures via Helper_Database_StoredProcedure
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = partId,
    ["p_OperationID"] = operation,
    ["p_LocationID"] = location,
    ["p_Quantity"] = quantity,
    ["p_UserID"] = userId,
    ["p_TransactionType"] = "IN"
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    parameters
);

if (result.IsSuccess)
{
    // Process result.Data
}
else
{
    // Handle error using result.Message
}
```

### **NEVER Use Direct SQL**
```csharp
// ❌ WRONG: Direct SQL queries are not allowed
var query = "SELECT * FROM inventory WHERE part_id = @partId";
var command = new MySqlCommand(query, connection);
```

</details>

<details>
<summary><strong>📊 Standard MTM Stored Procedures</strong></summary>

### **Inventory Management Procedures**
```sql
-- Add inventory item
CALL inv_inventory_Add_Item(p_PartID, p_OperationID, p_LocationID, p_Quantity, p_UserID, p_TransactionType, @status, @message)

-- Remove inventory item  
CALL inv_inventory_Remove_Item(p_PartID, p_OperationID, p_LocationID, p_Quantity, p_UserID, @status, @message)

-- Transfer inventory between locations
CALL inv_inventory_Transfer_Item(p_PartID, p_OperationID, p_FromLocation, p_ToLocation, p_Quantity, p_UserID, @status, @message)

-- Search inventory
CALL inv_inventory_Search(p_PartID, p_OperationID, p_LocationID, @status, @message)
```

### **Master Data Procedures**
```sql
-- Get part IDs for autocomplete
CALL md_part_ids_Get_Active(@status, @message)

-- Get operations for autocomplete
CALL md_operation_numbers_Get_Active(@status, @message)

-- Get locations for autocomplete  
CALL md_locations_Get_Active(@status, @message)

-- Validate part ID
CALL md_part_ids_Validate(p_PartID, @status, @message)
```

### **Transaction History Procedures**
```sql
-- Get transaction history
CALL inv_transactions_Get_History(p_PartID, p_StartDate, p_EndDate, p_Limit, @status, @message)

-- Get user transactions
CALL inv_transactions_Get_By_User(p_UserID, p_StartDate, p_EndDate, @status, @message)
```

</details>

<details>
<summary><strong>🔧 Implementation Patterns</strong></summary>

### **ViewModel Database Access Pattern**
```csharp
public class InventoryTabViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    public async Task ExecuteSaveAsync()
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = SelectedPart,
                ["p_OperationID"] = SelectedOperation,
                ["p_LocationID"] = SelectedLocation,
                ["p_Quantity"] = Quantity,
                ["p_Notes"] = Notes,
                ["p_UserID"] = _applicationStateService.CurrentUser,
                ["p_TransactionType"] = "IN"
            };

            var connectionString = _configurationService.GetConnectionString();
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "inv_inventory_Add_Item",
                parameters
            );

            if (result.IsSuccess)
            {
                Logger.LogInformation("Inventory item saved successfully");
                // Handle success
            }
            else
            {
                HasError = true;
                ErrorMessage = result.Message;
                Logger.LogWarning("Failed to save inventory item: {Error}", result.Message);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ErrorHandling.GetUserFriendlyMessage(ex);
            await ErrorHandling.HandleErrorAsync(ex, "SaveInventoryItem", userId, context);
        }
    }
}
```

### **Service Layer Database Access Pattern**
```csharp
public class InventoryService : IInventoryService
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<InventoryService> _logger;

    public async Task<Result<List<InventoryItem>>> SearchInventoryAsync(string partId, string operation, string location)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId ?? "",
                ["p_OperationID"] = operation ?? "",
                ["p_LocationID"] = location ?? ""
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Search",
                parameters
            );

            if (result.IsSuccess)
            {
                var items = ConvertDataTableToInventoryItems(result.Data);
                return Result<List<InventoryItem>>.Success(items);
            }
            else
            {
                return Result<List<InventoryItem>>.Failure(result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search inventory");
            return Result<List<InventoryItem>>.Failure($"Search failed: {ex.Message}");
        }
    }
}
```

</details>

<details>
<summary><strong>🎯 Transaction Type Logic (CRITICAL)</strong></summary>

### **User Intent-Based Transaction Types**
```csharp
// ✅ CORRECT: Base transaction type on user intent, not operation numbers
public enum UserIntent
{
    AddingStock,    // User is adding inventory (TransactionType = "IN")
    RemovingStock,  // User is removing inventory (TransactionType = "OUT")
    MovingStock     // User is transferring between locations (TransactionType = "TRANSFER")
}

public string DetermineTransactionType(UserIntent userIntent)
{
    return userIntent switch
    {
        UserIntent.AddingStock => "IN",       
        UserIntent.RemovingStock => "OUT",    
        UserIntent.MovingStock => "TRANSFER",
        _ => "IN" // Default to IN for unknown intent
    };
}
```

### **Operation Numbers Are Workflow Steps**
```csharp
// ✅ CORRECT: Operations are workflow identifiers, not transaction indicators
var operations = new[] { "90", "100", "110", "120", "130" }; // Workflow steps

// ❌ WRONG: Don't use operations to determine transaction type
if (operation == "90") transactionType = "IN"; // This is incorrect logic
```

</details>

<details>
<summary><strong>📊 Data Validation Patterns</strong></summary>

### **Input Validation Before Database Call**
```csharp
private bool ValidateInput()
{
    if (string.IsNullOrWhiteSpace(SelectedPart))
    {
        HasError = true;
        ErrorMessage = "Part ID is required";
        return false;
    }

    if (string.IsNullOrWhiteSpace(SelectedOperation))
    {
        HasError = true;
        ErrorMessage = "Operation is required";
        return false;
    }

    if (string.IsNullOrWhiteSpace(SelectedLocation))
    {
        HasError = true;
        ErrorMessage = "Location is required";
        return false;
    }

    if (Quantity <= 0)
    {
        HasError = true;
        ErrorMessage = "Quantity must be greater than zero";
        return false;
    }

    return true;
}
```

### **Database-Level Validation**
```csharp
// Database procedures handle business rule validation
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "md_part_ids_Validate",
    new Dictionary<string, object> { ["p_PartID"] = partId }
);

bool isValid = result.IsSuccess && result.Data.Rows.Count > 0;
```

</details>

<details>
<summary><strong>🔄 Connection Management</strong></summary>

### **Centralized Connection String Management**
```csharp
// ✅ Connection strings managed through Configuration service
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IConfigurationService configurationService)
    {
        _connectionString = configurationService.GetConnectionString();
    }
}
```

### **Connection String Access Pattern**
```csharp
// ✅ Get connection string when needed
var connectionString = _configurationService.GetConnectionString();
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    procedureName,
    parameters
);
```

</details>

<details>
<summary><strong>🛡️ Error Handling Integration</strong></summary>

### **Comprehensive Error Handling**
```csharp
public async Task<StoredProcedureResult> ExecuteDataTableWithStatus(
    string connectionString, 
    string procedureName, 
    Dictionary<string, object> parameters)
{
    try
    {
        // Database execution logic
        return result;
    }
    catch (Exception ex)
    {
        _logger?.LogError(ex, "Failed to execute stored procedure: {ProcedureName}", procedureName);
        
        await ErrorHandling.HandleErrorAsync(ex, "ExecuteDataTableWithStatus", Environment.UserName, 
            new Dictionary<string, object> 
            { 
                ["ProcedureName"] = procedureName, 
                ["Parameters"] = parameters 
            });

        return new StoredProcedureResult
        {
            Status = -1,
            Message = $"Error executing stored procedure: {ex.Message}",
            Data = new DataTable()
        };
    }
}
```

</details>

<details>
<summary><strong>📈 Performance Considerations</strong></summary>

### **Connection Pooling**
- MySQL connection pooling handled automatically
- Proper using statements ensure connection disposal
- No persistent connections maintained

### **Parameter Binding**
```csharp
// ✅ Always use parameterized queries
command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);

// ❌ Never use string concatenation
var query = $"SELECT * FROM table WHERE id = {id}"; // SQL injection risk
```

### **Data Table Usage**
```csharp
// ✅ Use DataTable for result sets
using var adapter = new MySqlDataAdapter(command);
var dataTable = new DataTable();
adapter.Fill(dataTable);
```

</details>

<details>
<summary><strong>🧪 Testing Database Operations</strong></summary>

### **Unit Test Pattern**
```csharp
[Test]
public async Task ExecuteDataTableWithStatus_ValidProcedure_ReturnsSuccess()
{
    // Arrange
    var parameters = new Dictionary<string, object>
    {
        ["p_PartID"] = "TEST001"
    };

    // Act
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "md_part_ids_Validate",
        parameters
    );

    // Assert
    Assert.IsTrue(result.IsSuccess);
    Assert.IsNotNull(result.Data);
}
```

### **Integration Test Pattern**
```csharp
[Test]
public async Task DatabaseService_TestConnection_ReturnsTrue()
{
    // Arrange
    var databaseService = serviceProvider.GetRequiredService<IDatabaseService>();

    // Act
    var result = await databaseService.TestConnectionAsync();

    // Assert
    Assert.IsTrue(result);
}
```

</details>

<details>
<summary><strong>📋 Best Practices Summary</strong></summary>

1. **✅ Always use stored procedures** - Never direct SQL
2. **✅ Use Helper_Database_StoredProcedure** - Consistent execution pattern
3. **✅ Check StoredProcedureResult.IsSuccess** - Handle errors properly
4. **✅ Log all database operations** - Use ILogger for comprehensive logging
5. **✅ Validate inputs before database calls** - Reduce unnecessary calls
6. **✅ Use parameterized queries** - Prevent SQL injection
7. **✅ Handle exceptions comprehensively** - Use ErrorHandling service
8. **✅ Manage connections properly** - Use using statements
9. **✅ Base transaction types on user intent** - Not operation numbers
10. **✅ Use centralized connection management** - Through Configuration service

</details>
