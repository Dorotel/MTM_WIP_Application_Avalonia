# MySQL Database Patterns - MTM WIP Application Instructions

**Database**: MySQL 9.4.0  
**Package**: MySql.Data  
**Pattern**: Stored Procedures ONLY  
**Created**: September 4, 2025  

---

## 🛡️ CRITICAL DATABASE ACCESS PATTERN

### MANDATORY: Stored Procedures Only
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
    foreach (DataRow row in dataTable.Rows)
    {
        var partId = row["PartID"].ToString();
        var currentQty = Convert.ToInt32(row["CurrentQuantity"]);
        // Process row data
    }
}
else
{
    // Handle error - result.Status contains error code
    await ErrorHandling.HandleErrorAsync(
        new InvalidOperationException($"Database operation failed with status: {result.Status}"),
        $"Failed to execute {storedProcedureName}"
    );
}
```

### FORBIDDEN: Never Use Direct SQL
```csharp
// ❌ WRONG: Never use direct SQL queries
string sql = $"SELECT * FROM inventory WHERE part_id = '{partId}'"; // SQL injection risk
var command = new MySqlCommand(sql, connection);

// ❌ WRONG: Never use string concatenation for SQL
string sql = "INSERT INTO inventory (part_id, quantity) VALUES ('" + partId + "', " + quantity + ")";

// ❌ WRONG: Never bypass Helper_Database_StoredProcedure
var command = new MySqlCommand("sp_GetInventory", connection)
{
    CommandType = CommandType.StoredProcedure
};
```

---

## 📋 Complete MTM Stored Procedure Catalog

### Inventory Management Procedures
```csharp
// Primary inventory operations
"inv_inventory_Add_Item"                    // Add inventory quantity
"inv_inventory_Remove_Item"                 // Remove inventory quantity  
"inv_inventory_Get_ByPartID"               // Get inventory by part ID only
"inv_inventory_Get_ByPartIDandOperation"   // Get inventory by part ID and operation
"inv_inventory_Get_CurrentQty_ByPartIDandOperation" // Get current quantity
"inv_inventory_Update_Quantity"            // Update inventory quantity
"inv_inventory_Transfer_Between_Operations" // Transfer between operations
"inv_inventory_Get_All"                    // Get all inventory records
"inv_inventory_Get_LowStock"              // Get low stock items
"inv_inventory_Delete_Item"               // Delete inventory item

// Usage Examples:
public async Task<bool> AddInventoryAsync(string partId, string operation, int quantity, string location)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_Operation", operation),
        new("p_Quantity", quantity),
        new("p_Location", location),
        new("p_User", currentUserId)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Add_Item",
        parameters
    );

    return result.Status == 1;
}

public async Task<int> GetCurrentQuantityAsync(string partId, string operation)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_Operation", operation)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Get_CurrentQty_ByPartIDandOperation",
        parameters
    );

    if (result.Status == 1 && result.Data.Rows.Count > 0)
    {
        return Convert.ToInt32(result.Data.Rows[0]["CurrentQuantity"]);
    }

    return 0;
}
```

### Transaction Management Procedures
```csharp
// Transaction operations
"inv_transaction_Add"                      // Add new transaction record
"inv_transaction_Get_History"             // Get transaction history
"inv_transaction_Get_ByPartID"           // Get transactions by part ID
"inv_transaction_Get_ByUser"             // Get transactions by user
"inv_transaction_Get_ByDateRange"        // Get transactions by date range
"inv_transaction_Get_Recent"             // Get recent transactions
"inv_transaction_Cancel"                 // Cancel/reverse transaction
"inv_transaction_Get_Summary"            // Get transaction summary

// MTM Transaction Type Logic (CRITICAL)
public async Task<bool> RecordTransactionAsync(string partId, string operation, int quantity, string transactionType, string location)
{
    // Transaction type determined by USER INTENT, not operation number
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_Operation", operation),      // "90", "100", "110" - workflow step
        new("p_Quantity", quantity),
        new("p_TransactionType", transactionType), // "IN", "OUT", "TRANSFER" - user intent
        new("p_Location", location),
        new("p_User", currentUserId),
        new("p_Timestamp", DateTime.Now)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_transaction_Add",
        parameters
    );

    return result.Status == 1;
}

// CORRECT: Transaction type by user action
public string DetermineTransactionType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => "IN",      // User adding inventory
        UserIntent.RemovingStock => "OUT",   // User removing inventory  
        UserIntent.MovingStock => "TRANSFER" // User moving between locations
    };
}

// WRONG: Don't use operation numbers for transaction type
public string DetermineTransactionType(string operation)
{
    // This is incorrect logic - operations are workflow steps, not transaction indicators
    return operation switch
    {
        "90" => "IN",   // WRONG
        "100" => "OUT", // WRONG
        _ => "TRANSFER" // WRONG
    };
}
```

### Master Data Management Procedures
```csharp
// Part ID management
"md_part_ids_Get_All"                     // Get all part IDs
"md_part_ids_Add"                        // Add new part ID
"md_part_ids_Update"                     // Update part ID
"md_part_ids_Delete"                     // Delete part ID
"md_part_ids_Search"                     // Search part IDs
"md_part_ids_Get_Active"                 // Get active part IDs only

// Location management  
"md_locations_Get_All"                   // Get all locations
"md_locations_Add"                       // Add new location
"md_locations_Update"                    // Update location
"md_locations_Delete"                    // Delete location
"md_locations_Get_Active"                // Get active locations only

// Operation number management
"md_operation_numbers_Get_All"           // Get all operation numbers
"md_operation_numbers_Add"               // Add new operation number
"md_operation_numbers_Update"            // Update operation number
"md_operation_numbers_Delete"            // Delete operation number
"md_operation_numbers_Get_Active"        // Get active operation numbers only

// Usage in MasterDataService
public async Task<List<string>> GetAllPartIdsAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "md_part_ids_Get_All",
        Array.Empty<MySqlParameter>()
    );

    var partIds = new List<string>();
    if (result.Status == 1)
    {
        foreach (DataRow row in result.Data.Rows)
        {
            partIds.Add(row["PartID"].ToString() ?? string.Empty);
        }
    }

    return partIds;
}

public async Task<List<string>> GetAllOperationsAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "md_operation_numbers_Get_All",
        Array.Empty<MySqlParameter>()
    );

    var operations = new List<string>();
    if (result.Status == 1)
    {
        foreach (DataRow row in result.Data.Rows)
        {
            // Operations are string numbers: "90", "100", "110", "120"
            operations.Add(row["OperationNumber"].ToString() ?? string.Empty);
        }
    }

    return operations;
}
```

### User and Session Management Procedures
```csharp
// User management
"usr_users_Get_All"                      // Get all users
"usr_users_Add"                          // Add new user
"usr_users_Update"                       // Update user
"usr_users_Delete"                       // Delete user
"usr_users_Get_ByUsername"              // Get user by username
"usr_users_Authenticate"                // Authenticate user
"usr_users_Get_Active"                  // Get active users only

// Session management
"usr_sessions_Create"                   // Create new session
"usr_sessions_Update"                   // Update session
"usr_sessions_End"                      // End session
"usr_sessions_Get_Active"              // Get active sessions
"usr_sessions_Cleanup"                 // Cleanup old sessions

// CRITICAL: User Table Column Validation
// The User model shows: "Column is 'User' but property is User_Name to avoid conflicts"
// ALWAYS use "User" column name when accessing User table data
public async Task<List<string>> GetAllUsersAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "usr_users_Get_All",
        Array.Empty<MySqlParameter>()
    );

    var users = new List<string>();
    if (result.Status == 1)
    {
        foreach (DataRow row in result.Data.Rows)
        {
            // ✅ CORRECT: Use "User" column (not "UserId" or "User_Name")
            users.Add(row["User"].ToString() ?? string.Empty);
        }
    }

    return users;
}

// Usage Example:
public async Task<bool> CreateUserSessionAsync(string username, string sessionId)
{
    var parameters = new MySqlParameter[]
    {
        new("p_Username", username),
        new("p_SessionID", sessionId),
        new("p_StartTime", DateTime.Now),
        new("p_IPAddress", GetClientIPAddress())
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "usr_sessions_Create",
        parameters
    );

    return result.Status == 1;
}
```

### Error Logging and Audit Procedures
```csharp
// Error logging
"log_error_Add_Error"                   // Add error log entry
"log_error_Get_All"                     // Get all error logs
"log_error_Get_ByDateRange"            // Get errors by date range
"log_error_Get_ByUser"                 // Get errors by user
"log_error_Get_Recent"                 // Get recent errors
"log_error_Delete_Old"                 // Delete old error logs

// Audit trail
"log_audit_Add_Entry"                  // Add audit log entry
"log_audit_Get_All"                    // Get all audit logs
"log_audit_Get_ByEntity"              // Get audit logs by entity
"log_audit_Get_ByUser"                // Get audit logs by user
"log_audit_Get_ByDateRange"           // Get audit logs by date range

// Integration with ErrorHandling service
public static async Task LogErrorToDatabaseAsync(Exception ex, string context, string userId)
{
    try
    {
        var parameters = new MySqlParameter[]
        {
            new("p_ErrorMessage", ex.Message),
            new("p_StackTrace", ex.StackTrace ?? string.Empty),
            new("p_Context", context),
            new("p_User", userId),
            new("p_Timestamp", DateTime.Now),
            new("p_MachineName", Environment.MachineName),
            new("p_ApplicationVersion", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown")
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            ConfigurationService.GetConnectionString(),
            "log_error_Add_Error",
            parameters
        );
    }
    catch (Exception dbEx)
    {
        // If database logging fails, fall back to file logging
        Logger.LogError(dbEx, "Failed to log error to database: {OriginalError}", ex.Message);
    }
}
```

### Quick Actions and Bookmarks Procedures
```csharp
// Quick actions management
"qa_quick_actions_Get_ByUser"           // Get quick actions by user
"qa_quick_actions_Add"                  // Add new quick action
"qa_quick_actions_Update"               // Update quick action
"qa_quick_actions_Delete"               // Delete quick action
"qa_quick_actions_Get_All"              // Get all quick actions
"qa_quick_actions_Execute"              // Execute quick action

// Bookmarks and favorites
"usr_bookmarks_Get_ByUser"             // Get user bookmarks
"usr_bookmarks_Add"                    // Add new bookmark
"usr_bookmarks_Delete"                 // Delete bookmark
"usr_bookmarks_Update"                 // Update bookmark

// QuickButtonsService integration
public async Task<List<QuickActionModel>> GetQuickActionsForUserAsync(string userId)
{
    var parameters = new MySqlParameter[]
    {
        new("p_User", userId),
        new("p_IsActive", true)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "qa_quick_actions_Get_ByUser",
        parameters
    );

    var quickActions = new List<QuickActionModel>();
    if (result.Status == 1)
    {
        foreach (DataRow row in result.Data.Rows)
        {
            quickActions.Add(new QuickActionModel
            {
                Id = Convert.ToInt32(row["ID"]),
                PartId = row["PartID"].ToString() ?? string.Empty,
                Operation = row["Operation"].ToString() ?? string.Empty,
                Quantity = Convert.ToInt32(row["Quantity"]),
                Location = row["Location"].ToString() ?? string.Empty,
                ActionType = row["ActionType"].ToString() ?? string.Empty,
                DisplayName = row["DisplayName"].ToString() ?? string.Empty
            });
        }
    }

    return quickActions;
}
```

---

## 🔧 Helper_Database_StoredProcedure Implementation

### Standard Result Pattern
```csharp
// Helper_Database_StoredProcedure.ExecuteDataTableWithStatus() returns:
public class DatabaseResult
{
    public int Status { get; set; }           // 1 = Success, 0 = Error, negative = specific error code
    public DataTable Data { get; set; }       // Result data (empty DataTable on error)
    public string Message { get; set; }       // Error message or success message
    public int RowsAffected { get; set; }     // Number of rows affected by operation
}

// Standard usage pattern:
public async Task<ServiceResult<List<T>>> GetDataAsync<T>(string procedureName, MySqlParameter[] parameters, Func<DataRow, T> mapper)
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            procedureName,
            parameters
        );

        if (result.Status == 1)
        {
            var items = new List<T>();
            foreach (DataRow row in result.Data.Rows)
            {
                items.Add(mapper(row));
            }
            
            return ServiceResult<List<T>>.Success(items);
        }
        else
        {
            return ServiceResult<List<T>>.Failure($"Database operation failed: {result.Message}");
        }
    }
    catch (Exception ex)
    {
        await ErrorHandling.HandleErrorAsync(ex, $"Database operation failed: {procedureName}");
        return ServiceResult<List<T>>.Failure($"Database error: {ex.Message}");
    }
}
```

### Parameter Handling Best Practices
```csharp
// ✅ CORRECT: Parameter creation and null handling
public MySqlParameter[] CreateInventoryParameters(string partId, string operation, int quantity, string location)
{
    return new MySqlParameter[]
    {
        new("p_PartID", partId ?? string.Empty),
        new("p_Operation", operation ?? string.Empty),
        new("p_Quantity", quantity),
        new("p_Location", location ?? string.Empty),
        new("p_User", CurrentUser?.UserId ?? "SYSTEM"),
        new("p_Timestamp", DateTime.Now)
    };
}

// ✅ CORRECT: Handling optional parameters
public MySqlParameter[] CreateSearchParameters(string? partId = null, string? operation = null, DateTime? startDate = null, DateTime? endDate = null)
{
    var parameters = new List<MySqlParameter>
    {
        new("p_PartID", partId ?? DBNull.Value),
        new("p_Operation", operation ?? DBNull.Value),
        new("p_StartDate", startDate ?? DBNull.Value),
        new("p_EndDate", endDate ?? DBNull.Value)
    };

    return parameters.ToArray();
}

// ✅ CORRECT: Handling different data types
public MySqlParameter[] CreateComplexParameters(InventoryItem item)
{
    return new MySqlParameter[]
    {
        new("p_PartID", MySqlDbType.VarChar, 50) { Value = item.PartId },
        new("p_Quantity", MySqlDbType.Int32) { Value = item.Quantity },
        new("p_LastUpdated", MySqlDbType.DateTime) { Value = item.LastUpdated },
        new("p_IsActive", MySqlDbType.Bit) { Value = item.IsActive },
        new("p_Cost", MySqlDbType.Decimal) { Value = item.Cost ?? DBNull.Value }
    };
}
```

---

## 🏭 MTM Manufacturing Domain Database Logic

### Part ID and Operation Relationship
```csharp
// Understanding MTM domain relationships
public class InventoryItem
{
    public string PartId { get; set; } = string.Empty;        // "PART001", "ABC-123" - unique part identifier
    public string Operation { get; set; } = string.Empty;     // "90", "100", "110", "120" - manufacturing workflow step
    public int Quantity { get; set; }                         // Integer quantity only
    public string Location { get; set; } = string.Empty;      // Physical location identifier
    public string TransactionType { get; set; } = string.Empty; // "IN", "OUT", "TRANSFER" - based on user intent
}

// CORRECT: Operation numbers are workflow steps
public async Task<bool> MovePartThroughWorkflowAsync(string partId, string fromOperation, string toOperation, int quantity)
{
    // Operations represent manufacturing workflow steps
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_FromOperation", fromOperation), // "90" -> "100" (workflow progression)
        new("p_ToOperation", toOperation),
        new("p_Quantity", quantity),
        new("p_TransactionType", "TRANSFER"),  // Always TRANSFER for workflow moves
        new("p_User", currentUserId)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Transfer_Between_Operations",
        parameters
    );

    return result.Status == 1;
}

// CORRECT: Transaction types based on user intent
public async Task<bool> ProcessInventoryTransactionAsync(string partId, string operation, int quantity, UserAction action)
{
    var transactionType = action.Intent switch
    {
        UserIntent.AddingStock => "IN",      // User physically adding parts
        UserIntent.RemovingStock => "OUT",   // User physically removing parts
        UserIntent.MovingStock => "TRANSFER" // User moving parts between locations
    };

    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_Operation", operation),       // Still track which workflow step
        new("p_Quantity", quantity),
        new("p_TransactionType", transactionType), // Based on what user is actually doing
        new("p_User", currentUserId)
    };

    // Choose appropriate stored procedure based on transaction type
    var procedureName = transactionType switch
    {
        "IN" => "inv_inventory_Add_Item",
        "OUT" => "inv_inventory_Remove_Item",
        "TRANSFER" => "inv_inventory_Transfer_Between_Operations",
        _ => throw new InvalidOperationException($"Unknown transaction type: {transactionType}")
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        procedureName,
        parameters
    );

    return result.Status == 1;
}
```

### Manufacturing Workflow Integration
```csharp
// Operations represent manufacturing process steps
public static class ManufacturingOperations
{
    public const string Receiving = "90";      // Parts received into system
    public const string FirstOperation = "100"; // First manufacturing step
    public const string SecondOperation = "110"; // Second manufacturing step  
    public const string FinalOperation = "120"; // Final manufacturing step
    public const string Shipping = "130";      // Parts ready for shipping
}

// CORRECT: Get inventory at specific workflow step
public async Task<List<InventoryItem>> GetInventoryAtOperationAsync(string partId, string operation)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_Operation", operation) // Get inventory at specific workflow step
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Get_ByPartIDandOperation",
        parameters
    );

    var inventory = new List<InventoryItem>();
    if (result.Status == 1)
    {
        foreach (DataRow row in result.Data.Rows)
        {
            inventory.Add(new InventoryItem
            {
                PartId = row["PartID"].ToString() ?? string.Empty,
                Operation = row["Operation"].ToString() ?? string.Empty,
                Quantity = Convert.ToInt32(row["CurrentQuantity"]),
                Location = row["Location"].ToString() ?? string.Empty,
                LastUpdated = Convert.ToDateTime(row["LastUpdated"])
            });
        }
    }

    return inventory;
}

// CORRECT: Track workflow progression
public async Task<WorkflowStatus> GetPartWorkflowStatusAsync(string partId)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Get_ByPartID",
        parameters
    );

    var workflowStatus = new WorkflowStatus { PartId = partId };
    
    if (result.Status == 1)
    {
        foreach (DataRow row in result.Data.Rows)
        {
            var operation = row["Operation"].ToString() ?? string.Empty;
            var quantity = Convert.ToInt32(row["CurrentQuantity"]);
            
            workflowStatus.OperationQuantities[operation] = quantity;
        }
    }

    return workflowStatus;
}

public class WorkflowStatus
{
    public string PartId { get; set; } = string.Empty;
    public Dictionary<string, int> OperationQuantities { get; set; } = new();
    
    public int QuantityAtOperation(string operation) => OperationQuantities.GetValueOrDefault(operation, 0);
    public bool IsCompleted => QuantityAtOperation("130") > 0; // Has parts at shipping
    public string CurrentStage => OperationQuantities.Where(kv => kv.Value > 0).OrderBy(kv => kv.Key).LastOrDefault().Key ?? "90";
}
```

---

## 🚫 NO FALLBACK DATA PATTERN (CRITICAL)

### Database Connectivity Validation Rules
**MANDATORY: Never provide fallback or dummy data when database operations fail.**

```csharp
// ✅ CORRECT: Return empty collections with server connectivity error
public async Task<List<string>> GetAllPartIdsAsync()
{
    try 
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            "md_part_ids_Get_All",
            Array.Empty<MySqlParameter>()
        );

        if (result.Status == 1)
        {
            var partIds = new List<string>();
            foreach (DataRow row in result.Data.Rows)
            {
                partIds.Add(row["PartID"].ToString() ?? string.Empty);
            }
            return partIds;
        }
        else
        {
            // Database operation failed - return empty collection
            await ErrorHandling.HandleErrorAsync(
                new InvalidOperationException($"Database operation failed with status: {result.Status}"),
                "Failed to load Part IDs from database"
            );
            return new List<string>();
        }
    }
    catch (Exception ex)
    {
        // Database connection failed - return empty collection
        await ErrorHandling.HandleErrorAsync(ex, "Failed to connect to database for Part IDs");
        return new List<string>();
    }
}

// ✅ CORRECT: UI validation with empty collection detection
private async void OnPartLostFocus(object? sender, RoutedEventArgs e)
{
    if (sender is not TextBox textBox || DataContext is not InventoryViewModel viewModel)
        return;

    var inputValue = textBox.Text?.Trim() ?? string.Empty;
    
    if (string.IsNullOrEmpty(inputValue))
        return;

    // Check if master data is available
    if (!viewModel.MasterData.PartIds.Any())
    {
        // Show server connectivity error - don't clear textbox
        textBox.BorderBrush = new SolidColorBrush(Colors.Orange);
        MessageBox.Show("No Part IDs available - check server connection.", "Server Connection Warning");
        return;
    }

    // Validate against available data
    if (!viewModel.MasterData.PartIds.Contains(inputValue))
    {
        textBox.Text = string.Empty; // Clear invalid entry
        textBox.BorderBrush = new SolidColorBrush(Colors.Red);
        MessageBox.Show($"Part ID '{inputValue}' not found. TextBox cleared.", "Invalid Part ID");
    }
    else
    {
        textBox.BorderBrush = new SolidColorBrush(Colors.Green);
    }
}

// ❌ WRONG: Never provide fallback data
public async Task<List<string>> GetAllPartIdsAsync_WRONG()
{
    try 
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            "md_part_ids_Get_All",
            Array.Empty<MySqlParameter>()
        );

        if (result.Status == 1)
        {
            // Process actual data...
        }
        else
        {
            // ❌ WRONG: Never return fallback data
            return new List<string> { "FALLBACK001", "FALLBACK002" };
        }
    }
    catch (Exception ex)
    {
        // ❌ WRONG: Never return fallback data on database errors
        return new List<string> { "ERROR001", "ERROR002" };
    }
}
```

### Master Data Service No-Fallback Pattern
```csharp
// ✅ CORRECT: MasterDataService without fallback methods
public class MasterDataService : IMasterDataService
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<MasterDataService> _logger;

    // Master data collections - empty when database unavailable
    public ObservableCollection<string> PartIds { get; } = new();
    public ObservableCollection<string> Operations { get; } = new();
    public ObservableCollection<string> Locations { get; } = new();
    public ObservableCollection<string> Users { get; } = new();

    // Load data methods - return empty on failure
    public async Task LoadAllDataAsync()
    {
        try
        {
            var partIds = await GetAllPartIdsAsync();
            var operations = await GetAllOperationsAsync();
            var locations = await GetAllLocationsAsync();
            var users = await GetAllUsersAsync();

            Application.Current.Dispatcher.Invoke(() =>
            {
                PartIds.Clear();
                Operations.Clear();
                Locations.Clear();
                Users.Clear();

                foreach (var partId in partIds) PartIds.Add(partId);
                foreach (var operation in operations) Operations.Add(operation);
                foreach (var location in locations) Locations.Add(location);
                foreach (var user in users) Users.Add(user);
            });

            _logger.LogInformation("Master data loaded successfully. PartIds: {PartCount}, Operations: {OpCount}, Locations: {LocCount}, Users: {UserCount}",
                partIds.Count, operations.Count, locations.Count, users.Count);
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to load master data");
            
            // Collections remain empty - UI will show "no data available" messages
            Application.Current.Dispatcher.Invoke(() =>
            {
                PartIds.Clear();
                Operations.Clear(); 
                Locations.Clear();
                Users.Clear();
            });
        }
    }

    // ❌ REMOVED: All fallback data methods eliminated
    // - GetFallbackPartIds()
    // - GetFallbackOperations()
    // - GetFallbackLocations()
    // - GetFallbackUsers()
}
```

### TextBox Validation Behavior Pattern
```csharp
// ✅ CORRECT: TextBoxClearOnNoMatchBehavior with server connectivity awareness
public class TextBoxClearOnNoMatchBehavior : Behavior<TextBox>
{
    public static readonly StyledProperty<ObservableCollection<string>> DataSourceProperty =
        AvaloniaProperty.Register<TextBoxClearOnNoMatchBehavior, ObservableCollection<string>>(nameof(DataSource));

    public ObservableCollection<string> DataSource
    {
        get => GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            AssociatedObject.LostFocus += OnLostFocus;
        }
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject?.Text is not string inputValue || string.IsNullOrEmpty(inputValue.Trim()))
            return;

        inputValue = inputValue.Trim();

        // Check if data source is available
        if (DataSource == null || !DataSource.Any())
        {
            // Show server connectivity warning - don't clear textbox
            AssociatedObject.BorderBrush = new SolidColorBrush(Colors.Orange);
            
            // Optional: Show user-friendly message
            MessageBox.Show(
                "No values exist due to no data able to be pulled from the server. Check your connection.",
                "Server Connection Warning",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        // Validate against available data
        if (!DataSource.Contains(inputValue))
        {
            AssociatedObject.Text = string.Empty; // Clear invalid entry
            AssociatedObject.BorderBrush = new SolidColorBrush(Colors.Red);
            
            MessageBox.Show(
                $"'{inputValue}' not found in available options. TextBox cleared.",
                "Invalid Entry",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        else
        {
            AssociatedObject.BorderBrush = new SolidColorBrush(Colors.Green);
        }
    }
}
```

### Column Name Validation Rules
```csharp
// ✅ CRITICAL: Always validate column names against actual database schema
public async Task<List<string>> LoadDataFromTableAsync(string procedureName, string columnName)
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        procedureName,
        Array.Empty<MySqlParameter>()
    );

    if (result.Status == 1)
    {
        var data = new List<string>();
        foreach (DataRow row in result.Data.Rows)
        {
            try
            {
                // Validate column exists before accessing
                if (result.Data.Columns.Contains(columnName))
                {
                    data.Add(row[columnName].ToString() ?? string.Empty);
                }
                else
                {
                    throw new ArgumentException($"Column '{columnName}' does not exist in result set. Available columns: {string.Join(", ", result.Data.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                }
            }
            catch (ArgumentException ex)
            {
                await ErrorHandling.HandleErrorAsync(ex, $"Column validation failed for procedure {procedureName}");
                return new List<string>(); // Return empty on column validation failure
            }
        }
        return data;
    }
    
    return new List<string>(); // Return empty on database failure
}

// Database Model Column Mapping Reference:
// User Table: Column = "User", Property = "User_Name" (to avoid conflicts)
// Part Table: Column = "PartID", Property = "PartId"
// Operation Table: Column = "OperationNumber", Property = "OperationNumber"  
// Location Table: Column = "Location", Property = "Location"
```

---

## 🔄 Connection Management and Performance

### Connection String Management
```csharp
// Connection string retrieved from ConfigurationService
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfigurationService configurationService, ILogger<DatabaseService> logger)
    {
        _connectionString = configurationService.GetConnectionString();
        _logger = logger;
    }

    // Helper method for consistent connection string usage
    protected async Task<DatabaseResult> ExecuteStoredProcedureAsync(string procedureName, MySqlParameter[] parameters)
    {
        try
        {
            return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                procedureName,
                parameters
            );
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "MySQL error executing {ProcedureName}", procedureName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "General error executing {ProcedureName}", procedureName);
            throw;
        }
    }
}
```

### Performance Optimization Patterns
```csharp
// Batch operations for better performance
public async Task<bool> ProcessBatchInventoryUpdatesAsync(List<InventoryUpdateRequest> updates)
{
    // Use transaction for batch operations
    var parameters = new MySqlParameter[]
    {
        new("p_UpdatesJson", JsonSerializer.Serialize(updates)),
        new("p_User", currentUserId),
        new("p_BatchTimestamp", DateTime.Now)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Batch_Update",
        parameters
    );

    return result.Status == 1;
}

// Pagination for large result sets
public async Task<PagedResult<InventoryItem>> GetInventoryPagedAsync(int pageNumber, int pageSize, string? filter = null)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PageNumber", pageNumber),
        new("p_PageSize", pageSize),
        new("p_Filter", filter ?? DBNull.Value)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Get_Paged",
        parameters
    );

    var pagedResult = new PagedResult<InventoryItem>();
    
    if (result.Status == 1)
    {
        // First row contains total count
        if (result.Data.Rows.Count > 0)
        {
            pagedResult.TotalCount = Convert.ToInt32(result.Data.Rows[0]["TotalCount"]);
        }

        // Map data rows to inventory items
        foreach (DataRow row in result.Data.Rows)
        {
            pagedResult.Items.Add(MapRowToInventoryItem(row));
        }
    }

    pagedResult.PageNumber = pageNumber;
    pagedResult.PageSize = pageSize;
    pagedResult.TotalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / pageSize);

    return pagedResult;
}
```

---

## 🧪 Database Testing Patterns

### Integration Testing with Test Database
```csharp
// Database integration tests
[Fact]
public async Task InventoryService_AddInventory_SuccessfullyAddsToDatabase()
{
    // Arrange
    using var testDatabase = new TestDatabaseFixture();
    var service = new InventoryService(testDatabase.ConnectionString, mockLogger);
    
    var partId = "TEST001";
    var operation = "100";
    var quantity = 5;
    var location = "A01";

    // Act
    var result = await service.AddInventoryAsync(partId, operation, quantity, location);

    // Assert
    Assert.True(result);
    
    // Verify data was actually added
    var inventory = await service.GetInventoryAsync(partId, operation);
    Assert.NotEmpty(inventory);
    Assert.Equal(quantity, inventory.First().Quantity);
}

// Stored procedure parameter testing
[Theory]
[InlineData("PART001", "100", 5, "A01")]
[InlineData("PART-WITH-DASH", "110", 100, "B02")]
[InlineData("", "90", 1, "")] // Test empty strings
public async Task InventoryService_AddInventory_HandlesVariousInputs(string partId, string operation, int quantity, string location)
{
    // Test parameter handling with various inputs
    using var testDatabase = new TestDatabaseFixture();
    var service = new InventoryService(testDatabase.ConnectionString, mockLogger);

    var result = await service.AddInventoryAsync(partId, operation, quantity, location);
    
    if (string.IsNullOrEmpty(partId))
    {
        Assert.False(result); // Should fail for empty part ID
    }
    else
    {
        Assert.True(result);
    }
}

// Mock Helper_Database_StoredProcedure for unit tests
public class MockDatabaseHelper
{
    public static DatabaseResult CreateSuccessResult(DataTable data)
    {
        return new DatabaseResult
        {
            Status = 1,
            Data = data,
            Message = "Success",
            RowsAffected = data.Rows.Count
        };
    }

    public static DatabaseResult CreateErrorResult(string message)
    {
        return new DatabaseResult
        {
            Status = 0,
            Data = new DataTable(),
            Message = message,
            RowsAffected = 0
        };
    }
}
```

---

## 📚 Related Database Documentation

- **.NET Architecture**: [Good Practices](./dotnet-architecture-good-practices.instructions.md)
- **Error Handling**: [Error Management Patterns](../../Services/ErrorHandling.cs)
- **Configuration**: [Database Configuration](./configuration-management.instructions.md)
- **Service Layer**: [Service Implementation Patterns](./service-layer-patterns.instructions.md)

---

**Document Status**: ✅ Complete Database Reference  
**Database Version**: MySQL 8.0.0  
**Last Updated**: September 4, 2025  
**Database Owner**: MTM Development Team
