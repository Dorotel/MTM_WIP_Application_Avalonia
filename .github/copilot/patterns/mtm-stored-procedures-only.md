# MTM Stored Procedures Only Pattern

## 🗄️ Stored Procedures Only Database Access Pattern

### **Core Principle**
**ALL database operations in the MTM WIP Application MUST use stored procedures. Direct SQL queries are NEVER allowed.**

### **Standard Database Operation Pattern**

#### **Basic Stored Procedure Call**
```csharp
public async Task<DatabaseResult<DataTable>> ExecuteStoredProcedureAsync(
    string procedureName, 
    MySqlParameter[] parameters)
{
    try
    {
        var connectionString = Helper_Database_Variables.GetConnectionString();
        
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            procedureName,
            parameters ?? Array.Empty<MySqlParameter>()
        );
        
        if (result.Status == 1)
        {
            Logger.LogInformation("Stored procedure {Procedure} executed successfully", procedureName);
            return DatabaseResult<DataTable>.Success(result.Data);
        }
        else
        {
            Logger.LogWarning("Stored procedure {Procedure} returned non-success status: {Status}", 
                procedureName, result.Status);
            return DatabaseResult<DataTable>.Failed($"Database operation failed: {procedureName}");
        }
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Error executing stored procedure: {Procedure}", procedureName);
        await Services.ErrorHandling.HandleErrorAsync(ex, $"Database operation: {procedureName}");
        return DatabaseResult<DataTable>.Failed($"Database error: {ex.Message}");
    }
}
```

### **Parameter Construction Patterns**

#### **Simple Parameters**
```csharp
// Basic parameter array construction
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Location", location),
    new("p_Operation", operation),
    new("p_Quantity", quantity)
};
```

#### **Parameters with Null Handling**
```csharp
// Handle nullable parameters properly
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Location", location ?? (object)DBNull.Value),
    new("p_Notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes),
    new("p_Quantity", quantity),
    new("p_Timestamp", DateTime.UtcNow)
};
```

#### **Output Parameters**
```csharp
// For stored procedures that return status via output parameters
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Quantity", quantity),
    new("@p_Status", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
    new("@p_ErrorMsg", MySqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Remove_Item",
    parameters
);

// Extract output parameter values
var status = Convert.ToInt32(parameters.First(p => p.ParameterName == "@p_Status").Value);
var errorMessage = parameters.First(p => p.ParameterName == "@p_ErrorMsg").Value?.ToString();

if (status == 1)
{
    return OperationResult.Success();
}
else
{
    return OperationResult.Failed(errorMessage ?? "Database operation failed");
}
```

### **MTM-Specific Stored Procedure Patterns**

#### **Inventory Operations**
```csharp
// Add inventory item
public async Task<OperationResult> AddInventoryAsync(InventoryItem item)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", item.PartId),
        new("p_Location", item.Location),
        new("p_Operation", item.Operation),
        new("p_Quantity", item.Quantity),
        new("p_ItemType", item.ItemType),
        new("p_User", item.User),
        new("p_Notes", item.Notes ?? (object)DBNull.Value)
    };

    var result = await ExecuteStoredProcedureAsync("inv_inventory_Add_Item", parameters);
    
    return result.IsSuccess ? OperationResult.Success() : OperationResult.Failed(result.ErrorMessage);
}

// Get inventory by part ID and operation
public async Task<DatabaseResult<List<InventoryItem>>> GetInventoryAsync(string partId, string operation)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", partId),
        new("p_Operation", operation)
    };

    var result = await ExecuteStoredProcedureAsync("inv_inventory_Get_ByPartIDandOperation", parameters);
    
    if (result.IsSuccess)
    {
        var inventory = ConvertDataTableToInventoryItems(result.Data);
        return DatabaseResult<List<InventoryItem>>.Success(inventory);
    }
    
    return DatabaseResult<List<InventoryItem>>.Failed(result.ErrorMessage);
}

// Remove inventory with validation
public async Task<OperationResult> RemoveInventoryAsync(RemoveInventoryRequest request)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartID", request.PartId),
        new("p_Location", request.Location),
        new("p_Operation", request.Operation),
        new("p_Quantity", request.Quantity),
        new("p_ItemType", request.ItemType),
        new("p_User", request.User),
        new("p_BatchNumber", request.BatchNumber),
        new("p_Notes", request.Notes ?? (object)DBNull.Value),
        new("@p_Status", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
        new("@p_ErrorMsg", MySqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
    };

    await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Helper_Database_Variables.GetConnectionString(),
        "inv_inventory_Remove_Item",
        parameters
    );

    var status = Convert.ToInt32(parameters.First(p => p.ParameterName == "@p_Status").Value);
    var errorMessage = parameters.First(p => p.ParameterName == "@p_ErrorMsg").Value?.ToString();

    return status == 1 ? OperationResult.Success() : OperationResult.Failed(errorMessage);
}
```

#### **Master Data Operations**
```csharp
// Get all parts
public async Task<DatabaseResult<List<PartInfo>>> GetAllPartsAsync()
{
    var result = await ExecuteStoredProcedureAsync(
        "md_part_ids_Get_All", 
        Array.Empty<MySqlParameter>()
    );

    if (result.IsSuccess)
    {
        var parts = ConvertDataTableToParts(result.Data);
        return DatabaseResult<List<PartInfo>>.Success(parts);
    }
    
    return DatabaseResult<List<PartInfo>>.Failed(result.ErrorMessage);
}

// Get all locations
public async Task<DatabaseResult<List<string>>> GetAllLocationsAsync()
{
    var result = await ExecuteStoredProcedureAsync(
        "md_locations_Get_All",
        Array.Empty<MySqlParameter>()
    );

    if (result.IsSuccess)
    {
        var locations = result.Data.AsEnumerable()
            .Select(row => row.Field<string>("Location"))
            .Where(loc => !string.IsNullOrEmpty(loc))
            .ToList();
        
        return DatabaseResult<List<string>>.Success(locations);
    }
    
    return DatabaseResult<List<string>>.Failed(result.ErrorMessage);
}

// Add new part
public async Task<OperationResult> AddPartAsync(PartInfo part)
{
    var parameters = new MySqlParameter[]
    {
        new("p_ItemNumber", part.ItemNumber),
        new("p_Customer", part.Customer),
        new("p_Description", part.Description),
        new("p_IssuedBy", part.IssuedBy),
        new("p_ItemType", part.ItemType)
    };

    var result = await ExecuteStoredProcedureAsync("md_part_ids_Add_Part", parameters);
    
    return result.IsSuccess ? OperationResult.Success() : OperationResult.Failed(result.ErrorMessage);
}
```

#### **Transaction Logging**
```csharp
// Log transaction (all inventory changes must be logged)
public async Task<OperationResult> LogTransactionAsync(TransactionData transaction)
{
    var parameters = new MySqlParameter[]
    {
        new("in_TransactionType", transaction.TransactionType), // "IN", "OUT", "TRANSFER"
        new("in_PartID", transaction.PartId),
        new("in_BatchNumber", transaction.BatchNumber),
        new("in_FromLocation", transaction.FromLocation ?? (object)DBNull.Value),
        new("in_ToLocation", transaction.ToLocation),
        new("in_Operation", transaction.Operation),
        new("in_Quantity", transaction.Quantity),
        new("in_Notes", transaction.Notes ?? (object)DBNull.Value),
        new("in_User", transaction.User),
        new("in_ItemType", transaction.ItemType),
        new("in_ReceiveDate", transaction.ReceiveDate)
    };

    var result = await ExecuteStoredProcedureAsync("inv_transaction_Add", parameters);
    
    if (!result.IsSuccess)
    {
        Logger.LogError("Failed to log transaction for {PartId}: {Error}", 
            transaction.PartId, result.ErrorMessage);
    }
    
    return result.IsSuccess ? OperationResult.Success() : OperationResult.Failed(result.ErrorMessage);
}
```

### **Data Conversion Patterns**

#### **DataTable to Strongly Typed Objects**
```csharp
/// <summary>
/// Convert DataTable to InventoryItem list
/// </summary>
private List<InventoryItem> ConvertDataTableToInventoryItems(DataTable dataTable)
{
    return dataTable.AsEnumerable().Select(row => new InventoryItem
    {
        Id = row.Field<int>("ID"),
        PartId = row.Field<string>("PartID") ?? string.Empty,
        Location = row.Field<string>("Location") ?? string.Empty,
        Operation = row.Field<string>("Operation") ?? string.Empty,
        Quantity = row.Field<int>("Quantity"),
        ItemType = row.Field<string>("ItemType") ?? string.Empty,
        BatchNumber = row.Field<string>("BatchNumber"),
        Notes = row.Field<string>("Notes"),
        CreatedDate = row.Field<DateTime>("CreatedDate"),
        User = row.Field<string>("User") ?? string.Empty
    }).ToList();
}

/// <summary>
/// Convert DataTable to PartInfo list
/// </summary>
private List<PartInfo> ConvertDataTableToParts(DataTable dataTable)
{
    return dataTable.AsEnumerable().Select(row => new PartInfo
    {
        Id = row.Field<int>("ID"),
        ItemNumber = row.Field<string>("ItemNumber") ?? string.Empty,
        Customer = row.Field<string>("Customer") ?? string.Empty,
        Description = row.Field<string>("Description") ?? string.Empty,
        ItemType = row.Field<string>("ItemType") ?? string.Empty,
        IssuedBy = row.Field<string>("IssuedBy") ?? string.Empty,
        CreatedDate = row.Field<DateTime>("CreatedDate")
    }).ToList();
}
```

#### **Generic DataTable Conversion**
```csharp
/// <summary>
/// Generic DataTable to object list conversion using reflection
/// </summary>
public List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
{
    var result = new List<T>();
    var properties = typeof(T).GetProperties();
    
    foreach (DataRow row in dataTable.Rows)
    {
        var item = new T();
        
        foreach (var property in properties)
        {
            if (dataTable.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
            {
                var value = row[property.Name];
                var convertedValue = Convert.ChangeType(value, 
                    Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                property.SetValue(item, convertedValue);
            }
        }
        
        result.Add(item);
    }
    
    return result;
}
```

### **Error Handling in Stored Procedure Calls**

#### **Comprehensive Error Handling**
```csharp
public async Task<DatabaseResult<T>> ExecuteStoredProcedureSafelyAsync<T>(
    string procedureName,
    MySqlParameter[] parameters,
    Func<DataTable, T> converter)
{
    try
    {
        var connectionString = Helper_Database_Variables.GetConnectionString();
        
        // Log the operation attempt
        Logger.LogInformation("Executing stored procedure: {Procedure} with {ParameterCount} parameters", 
            procedureName, parameters.Length);
        
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            procedureName,
            parameters
        );

        if (result.Status == 1)
        {
            var convertedData = converter(result.Data);
            Logger.LogInformation("Stored procedure {Procedure} executed successfully", procedureName);
            return DatabaseResult<T>.Success(convertedData);
        }
        else
        {
            var message = $"Stored procedure {procedureName} returned status: {result.Status}";
            Logger.LogWarning(message);
            return DatabaseResult<T>.Failed(message);
        }
    }
    catch (MySqlException ex)
    {
        var message = $"MySQL error executing {procedureName}: {ex.Message} (Error Code: {ex.Number})";
        Logger.LogError(ex, message);
        await Services.ErrorHandling.HandleErrorAsync(ex, $"Database operation: {procedureName}");
        return DatabaseResult<T>.Failed(message);
    }
    catch (Exception ex)
    {
        var message = $"Unexpected error executing {procedureName}: {ex.Message}";
        Logger.LogError(ex, message);
        await Services.ErrorHandling.HandleErrorAsync(ex, $"Database operation: {procedureName}");
        return DatabaseResult<T>.Failed(message);
    }
}
```

### **Connection Management**

#### **Connection String Management**
```csharp
// Always use Helper_Database_Variables for connection strings
public class DatabaseService
{
    private readonly string _connectionString;
    
    public DatabaseService()
    {
        // Get connection string through helper - handles environment detection
        _connectionString = Helper_Database_Variables.GetConnectionString();
    }
    
    // All methods use _connectionString for consistency
}
```

#### **Development vs Production Database**
```csharp
// Connection string is automatically selected based on environment
// Development: mtm_wip_application_test
// Production: mtm_wip_application

// Helper_Database_Variables.GetConnectionString() handles this automatically
// No manual connection string management needed in business logic
```

### **Anti-Patterns (What NOT to do)**

#### **❌ Direct SQL Queries**
```csharp
// NEVER DO THIS - Direct SQL is forbidden
var sql = "SELECT * FROM inv_inventory WHERE PartID = @partId";
var command = new MySqlCommand(sql, connection);
command.Parameters.AddWithValue("@partId", partId);
```

#### **❌ String Concatenation in SQL**
```csharp
// NEVER DO THIS - SQL injection risk
var sql = $"SELECT * FROM inv_inventory WHERE PartID = '{partId}'";
```

#### **❌ Manual Connection Management**
```csharp
// AVOID - Let Helper_Database_StoredProcedure manage connections
using var connection = new MySqlConnection(connectionString);
connection.Open();
// Manual connection handling
```

### **✅ Correct Pattern Summary**

```csharp
// ALWAYS USE THIS PATTERN
public async Task<DatabaseResult<T>> DatabaseOperationAsync<T>(parameters...)
{
    var sqlParameters = new MySqlParameter[]
    {
        new("p_Parameter1", value1),
        new("p_Parameter2", value2 ?? (object)DBNull.Value)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Helper_Database_Variables.GetConnectionString(),
        "stored_procedure_name", // Always use actual stored procedure names
        sqlParameters
    );

    if (result.Status == 1)
    {
        var convertedData = ConvertDataTableToType<T>(result.Data);
        return DatabaseResult<T>.Success(convertedData);
    }
    
    return DatabaseResult<T>.Failed("Database operation failed");
}
```