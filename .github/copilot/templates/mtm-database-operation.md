# MTM Database Operation Template

## 🗄️ Database Operation Instructions

For database operations in the MTM WIP Application - **STORED PROCEDURES ONLY**:

### **Stored Procedure Call Pattern**
```csharp
/// <summary>
/// Execute database operation via stored procedure
/// </summary>
/// <param name="parameters">Operation parameters</param>
/// <returns>Operation result with data</returns>
public async Task<DatabaseResult<DataTable>> ExecuteDatabaseOperationAsync(Dictionary<string, object> parameters)
{
    try
    {
        var connectionString = Helper_Database_Variables.GetConnectionString();
        
        var sqlParameters = new MySqlParameter[]
        {
            new("p_Parameter1", parameters["Parameter1"]),
            new("p_Parameter2", parameters["Parameter2"]),
            new("p_Parameter3", parameters.GetValueOrDefault("Parameter3", DBNull.Value))
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            "stored_procedure_name", // Use actual stored procedure names
            sqlParameters
        );

        if (result.Status == 1)
        {
            return DatabaseResult<DataTable>.Success(result.Data);
        }
        else
        {
            Logger.LogWarning("Stored procedure returned non-success status: {Status}", result.Status);
            return DatabaseResult<DataTable>.Failed("Database operation failed");
        }
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Error executing stored procedure");
        return DatabaseResult<DataTable>.Failed($"Database error: {ex.Message}");
    }
}
```

### **Common MTM Stored Procedures**

#### **Inventory Operations**
```csharp
// Add inventory item
public async Task<OperationResult> AddInventoryItemAsync(InventoryItem item)
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

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Add_Item",
        parameters
    );
    
    return result.Status == 1 ? OperationResult.Success() : OperationResult.Failed("Add operation failed");
}

// Get inventory by part ID
public async Task<DatabaseResult<List<InventoryItem>>> GetInventoryByPartIdAsync(string partId)
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

    if (result.Status == 1)
    {
        var items = ConvertDataTableToInventoryItems(result.Data);
        return DatabaseResult<List<InventoryItem>>.Success(items);
    }
    
    return DatabaseResult<List<InventoryItem>>.Failed("Failed to retrieve inventory");
}

// Remove inventory item
public async Task<OperationResult> RemoveInventoryItemAsync(RemoveInventoryRequest request)
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
        new("p_Notes", request.Notes ?? (object)DBNull.Value)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_inventory_Remove_Item",
        parameters
    );
    
    return result.Status == 1 ? OperationResult.Success() : OperationResult.Failed("Remove operation failed");
}
```

#### **Master Data Operations**
```csharp
// Get all part IDs
public async Task<DatabaseResult<List<PartInfo>>> GetAllPartsAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "md_part_ids_Get_All",
        Array.Empty<MySqlParameter>()
    );

    if (result.Status == 1)
    {
        var parts = ConvertDataTableToParts(result.Data);
        return DatabaseResult<List<PartInfo>>.Success(parts);
    }
    
    return DatabaseResult<List<PartInfo>>.Failed("Failed to retrieve parts");
}

// Get all locations
public async Task<DatabaseResult<List<string>>> GetAllLocationsAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "md_locations_Get_All",
        Array.Empty<MySqlParameter>()
    );

    if (result.Status == 1)
    {
        var locations = result.Data.AsEnumerable()
            .Select(row => row.Field<string>("Location"))
            .Where(loc => !string.IsNullOrEmpty(loc))
            .ToList();
        
        return DatabaseResult<List<string>>.Success(locations);
    }
    
    return DatabaseResult<List<string>>.Failed("Failed to retrieve locations");
}
```

#### **Transaction Logging**
```csharp
// Log transaction
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

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "inv_transaction_Add",
        parameters
    );
    
    return result.Status == 1 ? OperationResult.Success() : OperationResult.Failed("Transaction logging failed");
}
```

### **Data Conversion Helpers**
```csharp
/// <summary>
/// Convert DataTable to strongly typed objects
/// </summary>
private List<InventoryItem> ConvertDataTableToInventoryItems(DataTable dataTable)
{
    return dataTable.AsEnumerable().Select(row => new InventoryItem
    {
        PartId = row.Field<string>("PartID") ?? string.Empty,
        Location = row.Field<string>("Location") ?? string.Empty,
        Operation = row.Field<string>("Operation") ?? string.Empty,
        Quantity = row.Field<int>("Quantity"),
        ItemType = row.Field<string>("ItemType") ?? string.Empty,
        BatchNumber = row.Field<string>("BatchNumber"),
        Notes = row.Field<string>("Notes")
    }).ToList();
}
```

### **Error Handling in Database Operations**
```csharp
public async Task<DatabaseResult<T>> ExecuteDatabaseOperationSafelyAsync<T>(
    string storedProcedureName, 
    MySqlParameter[] parameters,
    Func<DataTable, T> converter)
{
    try
    {
        var connectionString = Helper_Database_Variables.GetConnectionString();
        
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            storedProcedureName,
            parameters
        );

        if (result.Status == 1)
        {
            var convertedData = converter(result.Data);
            return DatabaseResult<T>.Success(convertedData);
        }
        else
        {
            Logger.LogWarning("Database operation failed for procedure {Procedure}: Status {Status}", 
                storedProcedureName, result.Status);
            return DatabaseResult<T>.Failed($"Database operation failed: {storedProcedureName}");
        }
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Error executing database operation: {Procedure}", storedProcedureName);
        await Services.ErrorHandling.HandleErrorAsync(ex, $"Database operation: {storedProcedureName}");
        return DatabaseResult<T>.Failed($"Database error: {ex.Message}");
    }
}
```