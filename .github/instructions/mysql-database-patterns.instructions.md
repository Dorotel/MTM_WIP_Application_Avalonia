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

## 🚀 Advanced Database Integration Patterns

### Advanced Stored Procedure Parameter Patterns

#### Complex Parameter Handling for Manufacturing Operations
```csharp
// Advanced parameter builder for manufacturing batch operations
public class AdvancedParameterBuilder
{
    public static MySqlParameter[] CreateBatchInventoryParameters(
        IEnumerable<InventoryOperation> operations, 
        string userId, 
        string batchId)
    {
        var operationsJson = JsonSerializer.Serialize(operations, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        return new MySqlParameter[]
        {
            new("p_BatchId", batchId) { MySqlDbType = MySqlDbType.VarChar, Size = 50 },
            new("p_Operations", operationsJson) { MySqlDbType = MySqlDbType.JSON },
            new("p_User", userId) { MySqlDbType = MySqlDbType.VarChar, Size = 100 },
            new("p_Timestamp", DateTime.UtcNow) { MySqlDbType = MySqlDbType.DateTime },
            new("p_ProcessingMode", "BATCH") { MySqlDbType = MySqlDbType.VarChar, Size = 20 },
            // Output parameters for batch processing results
            new("p_out_ProcessedCount", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
            new("p_out_ErrorCount", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
            new("p_out_BatchStatus", MySqlDbType.VarChar) 
            { 
                Direction = ParameterDirection.Output, 
                Size = 50 
            }
        };
    }

    // Advanced parameter handling for manufacturing search operations
    public static MySqlParameter[] CreateAdvancedSearchParameters(
        ManufacturingSearchCriteria criteria)
    {
        var parameters = new List<MySqlParameter>
        {
            // Always include user context
            new("p_User", criteria.UserId ?? Environment.UserName),
            new("p_SearchId", Guid.NewGuid().ToString()),
            new("p_SearchTimestamp", DateTime.UtcNow)
        };

        // Dynamic parameter building based on criteria
        if (!string.IsNullOrWhiteSpace(criteria.PartId))
        {
            parameters.Add(new("p_PartID", criteria.PartId) 
            { 
                MySqlDbType = MySqlDbType.VarChar, 
                Size = 50 
            });
        }
        else
        {
            parameters.Add(new("p_PartID", DBNull.Value) 
            { 
                MySqlDbType = MySqlDbType.VarChar 
            });
        }

        // Handle operation array parameters
        if (criteria.Operations?.Any() == true)
        {
            var operationsJson = JsonSerializer.Serialize(criteria.Operations);
            parameters.Add(new("p_Operations", operationsJson) 
            { 
                MySqlDbType = MySqlDbType.JSON 
            });
        }

        // Date range handling with timezone considerations
        parameters.Add(new("p_StartDate", criteria.StartDate?.ToUniversalTime() ?? DBNull.Value));
        parameters.Add(new("p_EndDate", criteria.EndDate?.ToUniversalTime() ?? DBNull.Value));

        // Manufacturing-specific parameters
        parameters.Add(new("p_MinQuantity", criteria.MinQuantity ?? DBNull.Value));
        parameters.Add(new("p_MaxQuantity", criteria.MaxQuantity ?? DBNull.Value));
        parameters.Add(new("p_LocationPattern", criteria.LocationPattern ?? DBNull.Value));

        return parameters.ToArray();
    }
}
```

#### Advanced Transaction Management for Manufacturing Workflows
```csharp
// Complex transaction handling for manufacturing operations
public class ManufacturingTransactionManager
{
    private readonly string _connectionString;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _transactionSemaphore = new(5, 5); // Limit concurrent transactions

    public async Task<ManufacturingOperationResult> ExecuteManufacturingWorkflowAsync(
        ManufacturingWorkflow workflow)
    {
        await _transactionSemaphore.WaitAsync();
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var workflowResult = new ManufacturingOperationResult 
                { 
                    WorkflowId = workflow.Id,
                    StartTime = DateTime.UtcNow
                };

                // Step 1: Validate manufacturing business rules
                var validationResult = await ValidateWorkflowAsync(connection, transaction, workflow);
                if (!validationResult.IsValid)
                {
                    await transaction.RollbackAsync();
                    return ManufacturingOperationResult.ValidationFailed(validationResult.Errors);
                }

                // Step 2: Reserve inventory for the operation
                var reservationResult = await ReserveInventoryAsync(connection, transaction, workflow);
                if (!reservationResult.Success)
                {
                    await transaction.RollbackAsync();
                    return ManufacturingOperationResult.ReservationFailed(reservationResult.ErrorMessage);
                }

                // Step 3: Execute the manufacturing operations in sequence
                foreach (var operation in workflow.Operations)
                {
                    var operationResult = await ExecuteOperationAsync(connection, transaction, operation);
                    if (!operationResult.Success)
                    {
                        await transaction.RollbackAsync();
                        return ManufacturingOperationResult.OperationFailed(
                            operation.Id, operationResult.ErrorMessage);
                    }
                    workflowResult.CompletedOperations.Add(operationResult);
                }

                // Step 4: Update manufacturing state and create audit records
                await UpdateManufacturingStateAsync(connection, transaction, workflow, workflowResult);
                
                // Step 5: Create audit trail
                await CreateAuditTrailAsync(connection, transaction, workflow, workflowResult);

                await transaction.CommitAsync();
                
                workflowResult.Success = true;
                workflowResult.EndTime = DateTime.UtcNow;
                
                _logger.LogInformation(
                    "Manufacturing workflow completed successfully: {WorkflowId} in {Duration}ms",
                    workflow.Id, 
                    (workflowResult.EndTime - workflowResult.StartTime).TotalMilliseconds);

                return workflowResult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Manufacturing workflow failed: {WorkflowId}", workflow.Id);
                throw;
            }
        }
        finally
        {
            _transactionSemaphore.Release();
        }
    }

    private async Task<ValidationResult> ValidateWorkflowAsync(
        MySqlConnection connection, 
        MySqlTransaction transaction, 
        ManufacturingWorkflow workflow)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_WorkflowId", workflow.Id),
            new("p_WorkflowType", workflow.Type),
            new("p_OperationsJson", JsonSerializer.Serialize(workflow.Operations)),
            new("p_User", workflow.UserId),
            new("p_out_IsValid", MySqlDbType.Bit) { Direction = ParameterDirection.Output },
            new("p_out_ValidationErrors", MySqlDbType.JSON) { Direction = ParameterDirection.Output }
        };

        await Helper_Database_StoredProcedure.ExecuteStoredProcedureAsync(
            connection, transaction, "mfg_workflow_Validate", parameters);

        var isValid = Convert.ToBoolean(parameters.First(p => p.ParameterName == "p_out_IsValid").Value);
        var errorsJson = parameters.First(p => p.ParameterName == "p_out_ValidationErrors").Value?.ToString();
        
        var errors = string.IsNullOrEmpty(errorsJson) 
            ? new List<string>() 
            : JsonSerializer.Deserialize<List<string>>(errorsJson);

        return new ValidationResult { IsValid = isValid, Errors = errors };
    }
}
```

### Advanced Error Recovery and Retry Mechanisms

#### Resilient Database Operations for Manufacturing
```csharp
public class ResilientDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger _logger;
    private readonly RetryPolicy _retryPolicy;

    public ResilientDatabaseService(string connectionString, ILogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
        _retryPolicy = CreateManufacturingRetryPolicy();
    }

    // Advanced retry policy for manufacturing operations
    private RetryPolicy CreateManufacturingRetryPolicy()
    {
        return new RetryPolicy
        {
            MaxRetries = 5,
            BaseDelay = TimeSpan.FromMilliseconds(500),
            MaxDelay = TimeSpan.FromSeconds(30),
            BackoffType = BackoffType.ExponentialWithJitter,
            RetriableExceptions = new[]
            {
                typeof(MySqlException), // Database connection issues
                typeof(TimeoutException), // Query timeout
                typeof(TaskCanceledException) // Network issues
            },
            NonRetriableExceptions = new[]
            {
                typeof(ManufacturingBusinessRuleException), // Business rule violations
                typeof(ArgumentException), // Invalid parameters
                typeof(UnauthorizedAccessException) // Security violations
            }
        };
    }

    public async Task<DatabaseResult> ExecuteWithRetryAsync<T>(
        string procedureName,
        MySqlParameter[] parameters,
        Func<DatabaseResult, T> resultProcessor = null,
        CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async (attempt) =>
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                
                // Connection timeout handling for manufacturing environments
                connection.ConnectionTimeout = Math.Min(30 + (attempt * 10), 120);
                await connection.OpenAsync(cancellationToken);

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connection, procedureName, parameters, cancellationToken);

                // Manufacturing-specific result validation
                if (result.Status == -999) // Manufacturing system unavailable
                {
                    throw new ManufacturingSystemUnavailableException(result.Message);
                }

                if (result.Status == -1 && result.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
                {
                    // Deadlocks are retriable in manufacturing scenarios
                    throw new DatabaseDeadlockException(result.Message);
                }

                _logger.LogDebug(
                    "Database operation succeeded on attempt {Attempt}: {Procedure}",
                    attempt, procedureName);

                return result;
            }
            catch (MySqlException ex) when (IsRetriableException(ex))
            {
                _logger.LogWarning(ex,
                    "Retriable database error on attempt {Attempt}/{MaxAttempts} for {Procedure}: {Error}",
                    attempt, _retryPolicy.MaxRetries, procedureName, ex.Message);

                if (attempt == _retryPolicy.MaxRetries)
                {
                    _logger.LogError(ex,
                        "Database operation failed after {MaxAttempts} attempts: {Procedure}",
                        _retryPolicy.MaxRetries, procedureName);
                }

                throw;
            }
        }, cancellationToken);
    }

    private bool IsRetriableException(MySqlException ex)
    {
        // MySQL error codes that are retriable in manufacturing context
        return ex.Number switch
        {
            1040 => true, // Too many connections
            1205 => true, // Lock wait timeout
            1213 => true, // Deadlock found
            2002 => true, // Can't connect to server
            2003 => true, // Can't connect to server on socket
            2006 => true, // MySQL server has gone away
            2013 => true, // Lost connection during query
            _ => false
        };
    }
}
```

### Advanced Performance Optimization Patterns

#### Connection Pooling and Performance Monitoring
```csharp
public class OptimizedDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger _logger;
    private readonly IMetrics _metrics;
    private readonly ConnectionPool _connectionPool;

    public OptimizedDatabaseService(IConfiguration configuration, ILogger logger, IMetrics metrics)
    {
        _connectionString = BuildOptimizedConnectionString(configuration);
        _logger = logger;
        _metrics = metrics;
        _connectionPool = new ConnectionPool(_connectionString, maxPoolSize: 20);
    }

    private string BuildOptimizedConnectionString(IConfiguration configuration)
    {
        var builder = new MySqlConnectionStringBuilder(
            configuration.GetConnectionString("DefaultConnection"))
        {
            // Performance optimizations for manufacturing workloads
            ConnectionTimeout = 30,
            DefaultCommandTimeout = 120,
            Pooling = true,
            MinimumPoolSize = 5,
            MaximumPoolSize = 50,
            ConnectionLifeTime = 300, // 5 minutes
            ConnectionReset = false,
            UseCompression = false, // Disable for local networks
            AllowBatch = true,
            InteractiveSession = false,
            
            // Manufacturing-specific optimizations
            UseAffectedRows = true,
            RespectBinaryFlags = false,
            TreatTinyAsBoolean = true,
            ConvertZeroDateTime = true,
            AllowZeroDateTime = true,
            
            // Security settings
            SslMode = MySqlSslMode.Preferred,
            CheckParameters = false // Disable for performance in trusted environment
        };

        return builder.ConnectionString;
    }

    // Batch processing for high-volume manufacturing operations
    public async Task<BatchProcessingResult> ExecuteBatchOperationsAsync(
        string procedureName,
        IEnumerable<MySqlParameter[]> parameterSets,
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var parametersList = parameterSets.ToList();
        var result = new BatchProcessingResult
        {
            TotalOperations = parametersList.Count,
            StartTime = DateTime.UtcNow
        };

        using var connection = await _connectionPool.GetConnectionAsync();
        using var performanceTracker = _metrics.StartTimer("database.batch_operations");

        try
        {
            // Process in optimized batches
            for (int i = 0; i < parametersList.Count; i += batchSize)
            {
                var batch = parametersList.Skip(i).Take(batchSize);
                var batchResults = await ProcessBatchAsync(
                    connection, procedureName, batch, cancellationToken);

                result.SuccessfulOperations += batchResults.SuccessCount;
                result.FailedOperations += batchResults.FailureCount;
                result.BatchResults.AddRange(batchResults.Results);

                // Progress reporting for manufacturing operations
                var progressPercentage = ((i + batchSize) * 100.0) / parametersList.Count;
                _logger.LogInformation(
                    "Batch processing progress: {Progress:F1}% ({Processed}/{Total})",
                    progressPercentage, i + batchSize, parametersList.Count);

                // Allow cancellation between batches
                cancellationToken.ThrowIfCancellationRequested();
            }

            result.EndTime = DateTime.UtcNow;
            result.Success = result.FailedOperations == 0;

            // Performance metrics
            _metrics.Gauge("database.batch_operations.total", result.TotalOperations);
            _metrics.Gauge("database.batch_operations.success_rate", 
                result.SuccessfulOperations / (double)result.TotalOperations);
            _metrics.Gauge("database.batch_operations.duration_ms", 
                (result.EndTime - result.StartTime).TotalMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Batch processing failed after processing {ProcessedCount}/{TotalCount} operations",
                result.SuccessfulOperations + result.FailedOperations, result.TotalOperations);
            throw;
        }
    }

    // Query plan optimization for manufacturing reporting
    public async Task<DataTable> ExecuteOptimizedReportQueryAsync(
        string procedureName,
        MySqlParameter[] parameters,
        QueryOptimizationHints hints = null)
    {
        using var connection = await _connectionPool.GetConnectionAsync();
        using var command = new MySqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = hints?.TimeoutSeconds ?? 300 // Extended timeout for reports
        };

        // Add optimization hints for manufacturing reports
        if (hints != null)
        {
            if (hints.UseIndex != null)
            {
                command.CommandText = $"CALL {procedureName}() /*+ USE INDEX({hints.UseIndex}) */";
            }

            if (hints.ForceJoinOrder)
            {
                command.CommandText = $"/*+ STRAIGHT_JOIN */ {command.CommandText}";
            }

            if (hints.MaxExecutionTime > 0)
            {
                command.CommandText = $"/*+ MAX_EXECUTION_TIME({hints.MaxExecutionTime}) */ {command.CommandText}";
            }
        }

        command.Parameters.AddRange(parameters);

        using var performanceTracker = _metrics.StartTimer($"database.report.{procedureName}");
        
        var adapter = new MySqlDataAdapter(command);
        var dataTable = new DataTable();
        
        await Task.Run(() => adapter.Fill(dataTable)); // Offload to thread pool
        
        _metrics.Gauge($"database.report.{procedureName}.rows", dataTable.Rows.Count);
        
        return dataTable;
    }
}
```

### ❌ Database Anti-Patterns (Avoid These)

#### Connection Management Anti-Patterns
```csharp
// ❌ WRONG: Creating connections without proper disposal
public async Task<DataTable> GetInventoryDataBadAsync()
{
    var connection = new MySqlConnection(_connectionString);
    connection.Open(); // Never disposed - memory leak!
    
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Get_All", Array.Empty<MySqlParameter>());
    
    return result.Data;
}

// ✅ CORRECT: Proper connection management
public async Task<DataTable> GetInventoryDataGoodAsync()
{
    using var connection = new MySqlConnection(_connectionString);
    await connection.OpenAsync();
    
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connection, "inv_inventory_Get_All", Array.Empty<MySqlParameter>());
    
    return result.Data;
}
```

#### Transaction Scope Anti-Patterns
```csharp
// ❌ WRONG: Long-running transactions that lock manufacturing data
public async Task ProcessLargeBatchBadAsync(List<InventoryOperation> operations)
{
    using var connection = new MySqlConnection(_connectionString);
    await connection.OpenAsync();
    
    using var transaction = await connection.BeginTransactionAsync();
    
    // BAD: This locks tables for too long in manufacturing environment
    foreach (var operation in operations) // Could be thousands of operations!
    {
        await ExecuteOperationAsync(connection, transaction, operation);
        await Task.Delay(100); // Simulates slow processing - TERRIBLE!
    }
    
    await transaction.CommitAsync(); // Holds locks for entire batch duration
}

// ✅ CORRECT: Process in smaller transaction batches
public async Task ProcessLargeBatchGoodAsync(List<InventoryOperation> operations)
{
    const int batchSize = 50; // Keep transactions small
    
    for (int i = 0; i < operations.Count; i += batchSize)
    {
        var batch = operations.Skip(i).Take(batchSize);
        
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var transaction = await connection.BeginTransactionAsync();
        
        foreach (var operation in batch)
        {
            await ExecuteOperationAsync(connection, transaction, operation);
        }
        
        await transaction.CommitAsync(); // Short-lived transaction
    }
}
```

#### SQL Injection Vulnerabilities
```csharp
// ❌ WRONG: String concatenation creates SQL injection risk
public async Task<DataTable> SearchInventoryBadAsync(string partId, string operation)
{
    // NEVER DO THIS - SQL injection vulnerability!
    var sql = $"CALL inv_inventory_Search('{partId}', '{operation}')";
    
    using var connection = new MySqlConnection(_connectionString);
    using var command = new MySqlCommand(sql, connection);
    
    // Attacker could inject: "'; DROP TABLE inventory; --"
    var adapter = new MySqlDataAdapter(command);
    var dataTable = new DataTable();
    adapter.Fill(dataTable);
    return dataTable;
}

// ✅ CORRECT: Always use parameterized queries through Helper class
public async Task<DataTable> SearchInventoryGoodAsync(string partId, string operation)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId ?? string.Empty),
        new("p_Operation", operation ?? string.Empty)
    };
    
    // Safe: Parameters are properly escaped and typed
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Search", parameters);
    
    return result.Data;
}
```

## 🔧 Manufacturing Database Troubleshooting Guide

### Common Database Issues in Manufacturing Context

#### Issue: Database Connection Timeouts During Shift Changes
**Symptoms**: Connection timeouts during peak manufacturing hours (shift changes)

**Solution**: Implement connection pooling with proper sizing
```csharp
// Configure connection string for manufacturing load
var connectionString = new MySqlConnectionStringBuilder
{
    Server = "manufacturing-db-server",
    Database = "mtm_manufacturing",
    UserID = "mtm_app",
    Password = "secure_password",
    
    // Manufacturing-optimized settings
    MinimumPoolSize = 10,  // Always keep connections ready
    MaximumPoolSize = 100, // Handle peak shift loads
    ConnectionLifeTime = 600, // 10 minutes for stability
    ConnectionTimeout = 30,
    DefaultCommandTimeout = 120 // Manufacturing operations can be slow
}.ConnectionString;
```

#### Issue: Deadlocks During Concurrent Manufacturing Operations
**Symptoms**: Multiple operators processing same parts simultaneously causing deadlocks

**Solution**: Implement proper transaction ordering and retry logic
```csharp
// Always access tables in consistent order to prevent deadlocks
public async Task TransferInventoryAsync(string partId, string fromOperation, string toOperation, int quantity)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_FromOperation", fromOperation),
        new("p_ToOperation", toOperation), 
        new("p_Quantity", quantity),
        new("p_User", CurrentUser)
    };
    
    // Stored procedure handles locking order: inventory -> transactions -> audit
    var result = await ExecuteWithRetryAsync(
        "inv_inventory_Transfer_WithLocking", 
        parameters,
        maxRetries: 5); // Retry deadlocks automatically
}
```

#### Issue: Slow Performance on Manufacturing Reports  
**Symptoms**: Daily/weekly reports taking too long, blocking operations

**Solution**: Use dedicated read replicas and query optimization
```csharp
// Use read replica for reporting to avoid impacting operations
public async Task<ManufacturingReport> GenerateShiftReportAsync(DateTime shiftStart, DateTime shiftEnd)
{
    var readOnlyConnectionString = _configuration.GetConnectionString("ReportingReplica");
    
    var parameters = new MySqlParameter[]
    {
        new("p_ShiftStart", shiftStart),
        new("p_ShiftEnd", shiftEnd),
        new("p_ReportType", "SHIFT_SUMMARY"),
        new("p_MaxExecutionTimeMs", 300000) // 5 minute limit
    };
    
    // Use reporting-optimized stored procedure
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        readOnlyConnectionString, "rpt_shift_summary_optimized", parameters);
        
    return MapToManufacturingReport(result.Data);
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


## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**
- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**
- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**
- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**
- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**
- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**
- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**
- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.