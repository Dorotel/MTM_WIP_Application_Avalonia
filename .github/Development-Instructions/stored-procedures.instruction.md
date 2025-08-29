# Database Stored Procedures Reference Guide

<details>
<summary><strong>🎯 MTM Database Stored Procedures</strong></summary>

Comprehensive reference for all MTM WIP Application stored procedures including parameters, usage patterns, and implementation examples.

### **Core Principles**
- **When to Use**: Specific business scenarios for each stored procedure
- **Parameter Requirements**: All required and optional parameters
- **Return Values**: Expected data structures and status codes
- **Usage Examples**: Complete C# implementation examples
- **Error Handling**: Common error scenarios and handling patterns

</details>

<details>
<summary><strong>📊 Inventory Management Procedures</strong></summary>

#### inv_inventory_Add_Item
**Purpose**: Add new inventory items to the system
**When to Use**: User adding stock to inventory, receiving new parts

**Parameters**:
```sql
p_PartID VARCHAR(50) -- Required: Part identifier
p_Location VARCHAR(50) -- Required: Storage location
p_Operation VARCHAR(10) -- Required: Operation number (workflow step)
p_Quantity INT -- Required: Quantity to add (must be > 0)
p_ItemType VARCHAR(50) -- Required: Type of item
p_User VARCHAR(50) -- Required: User performing operation
p_Notes TEXT -- Optional: Additional notes
```

**Usage Example**:
```csharp
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = partId,
    ["p_Location"] = location,
    ["p_Operation"] = operation,
    ["p_Quantity"] = quantity,
    ["p_ItemType"] = itemType,
    ["p_User"] = currentUser,
    ["p_Notes"] = !string.IsNullOrWhiteSpace(notes) ? notes : DBNull.Value
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    parameters
);

if (result.IsSuccess)
{
    Logger.LogInformation("Inventory item added successfully");
}
```

#### inv_inventory_Get_ByPartID
**Purpose**: Retrieve all inventory records for a specific part
**When to Use**: Search functionality, part lookup, inventory verification

**Parameters**:
```sql
p_PartID VARCHAR(50) -- Required: Part identifier to search for
```

**Usage Example**:
```csharp
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = partId
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Get_ByPartID",
    parameters
);

if (result.IsSuccess)
{
    foreach (DataRow row in result.Data.Rows)
    {
        var inventoryItem = new InventoryItem
        {
            ID = Convert.ToInt32(row["ID"]),
            PartID = row["PartID"]?.ToString() ?? string.Empty,
            Location = row["Location"]?.ToString() ?? string.Empty,
            // ... map other properties
        };
    }
}
```

#### inv_inventory_Get_ByPartIDandOperation
**Purpose**: Retrieve inventory records for specific part and operation combination
**When to Use**: Refined search, operation-specific inventory checks

**Parameters**:
```sql
p_PartID VARCHAR(50) -- Required: Part identifier
p_Operation VARCHAR(10) -- Required: Operation number
```

#### inv_inventory_Get_ByUser
**Purpose**: Retrieve inventory history for a specific user
**When to Use**: User activity tracking, audit trails, personal history

**Parameters**:
```sql
p_User VARCHAR(50) -- Required: Username to search for
```

#### inv_inventory_Remove_Item
**Purpose**: Remove inventory items from the system
**When to Use**: Parts consumed, shipped, scrapped, or transferred out

**Parameters**:
```sql
p_PartID VARCHAR(50) -- Required: Part identifier
p_Location VARCHAR(50) -- Required: Current location
p_Operation VARCHAR(10) -- Required: Operation number
p_Quantity INT -- Required: Quantity to remove (must be > 0)
p_ItemType VARCHAR(50) -- Required: Type of item
p_User VARCHAR(50) -- Required: User performing operation
p_BatchNumber VARCHAR(100) -- Optional: Batch identifier for tracking
p_Notes TEXT -- Optional: Reason for removal
```

**Usage Example**:
```csharp
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = partId,
    ["p_Location"] = location,
    ["p_Operation"] = operation,
    ["p_Quantity"] = quantity,
    ["p_ItemType"] = itemType,
    ["p_User"] = currentUser,
    ["p_BatchNumber"] = !string.IsNullOrWhiteSpace(batchNumber) ? batchNumber : DBNull.Value,
    ["p_Notes"] = "Removed via Remove Item interface"
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Remove_Item",
    parameters
);
```

#### inv_inventory_Transfer_Part
**Purpose**: Transfer entire part quantity to new location
**When to Use**: Moving complete part inventory between locations

**Parameters**:
```sql
p_BatchNumber VARCHAR(100) -- Required: Batch identifier
p_PartID VARCHAR(50) -- Required: Part identifier
p_Operation VARCHAR(10) -- Required: Operation number
p_NewLocation VARCHAR(50) -- Required: Destination location
```

#### inv_inventory_Transfer_Quantity
**Purpose**: Transfer partial quantity to new location
**When to Use**: Splitting inventory between locations

**Parameters**:
```sql
p_BatchNumber VARCHAR(100) -- Required: Batch identifier
p_PartID VARCHAR(50) -- Required: Part identifier
p_Operation VARCHAR(10) -- Required: Operation number
p_TransferQuantity INT -- Required: Quantity to transfer
p_OriginalQuantity INT -- Required: Original total quantity
p_NewLocation VARCHAR(50) -- Required: Destination location
p_User VARCHAR(50) -- Required: User performing transfer
```

</details>

<details>
<summary><strong>🔧 Master Data Management Procedures</strong></summary>

## Parts Management

#### md_part_ids_Get_All
**Purpose**: Retrieve all parts in the system
**When to Use**: Populating dropdown lists, part selection interfaces

**Parameters**: None

**Usage Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "md_part_ids_Get_All",
    new Dictionary<string, object>()
);

if (result.IsSuccess)
{
    foreach (DataRow row in result.Data.Rows)
    {
        var partId = row["PartID"]?.ToString();
        if (!string.IsNullOrEmpty(partId))
        {
            PartOptions.Add(partId);
        }
    }
}
```

#### md_part_ids_Add_Part
**Purpose**: Add new part to the system
**When to Use**: Part master data creation

**Parameters**:
```sql
p_PartID VARCHAR(50) -- Required: Unique part identifier
p_Customer VARCHAR(100) -- Required: Customer name
p_Description TEXT -- Required: Part description
p_IssuedBy VARCHAR(50) -- Required: User creating the part
p_ItemType VARCHAR(50) -- Required: Type classification
```

#### md_part_ids_Update_Part
**Purpose**: Update existing part information
**When to Use**: Part master data maintenance

**Parameters**:
```sql
p_ID INT -- Required: Database ID of part record
p_PartID VARCHAR(50) -- Required: Part identifier
p_Customer VARCHAR(100) -- Required: Customer name
p_Description TEXT -- Required: Part description
p_IssuedBy VARCHAR(50) -- Required: User updating the part
p_ItemType VARCHAR(50) -- Required: Type classification
```

#### md_part_ids_Delete_ByItemNumber
**Purpose**: Remove part from the system
**When to Use**: Part discontinuation, cleanup operations

**Parameters**:
```sql
p_ItemNumber VARCHAR(50) -- Required: Part identifier to delete
```

#### md_part_ids_Get_ByItemNumber
**Purpose**: Retrieve specific part details
**When to Use**: Part validation, detailed part information display

**Parameters**:
```sql
p_ItemNumber VARCHAR(50) -- Required: Part identifier
```

## Operations Management

#### md_operation_numbers_Get_All
**Purpose**: Retrieve all operation numbers
**When to Use**: Populating operation dropdowns, workflow management

**Parameters**: None

#### md_operation_numbers_Add_Operation
**Purpose**: Add new operation number
**When to Use**: Workflow expansion, new process steps

**Parameters**:
```sql
p_Operation VARCHAR(10) -- Required: Operation number (e.g., "90", "100")
p_IssuedBy VARCHAR(50) -- Required: User creating the operation
```

#### md_operation_numbers_Update_Operation
**Purpose**: Update operation number
**When to Use**: Operation number changes, process refinement

**Parameters**:
```sql
p_Operation VARCHAR(10) -- Required: Current operation number
p_NewOperation VARCHAR(10) -- Required: New operation number
p_IssuedBy VARCHAR(50) -- Required: User updating the operation
```

#### md_operation_numbers_Delete_ByOperation
**Purpose**: Remove operation number
**When to Use**: Process elimination, workflow cleanup

**Parameters**:
```sql
p_Operation VARCHAR(10) -- Required: Operation number to delete
```

## Locations Management

#### md_locations_Get_All
**Purpose**: Retrieve all storage locations
**When to Use**: Location dropdowns, inventory placement

**Parameters**: None

#### md_locations_Add_Location
**Purpose**: Add new storage location
**When to Use**: Facility expansion, new storage areas

**Parameters**:
```sql
p_Location VARCHAR(50) -- Required: Location identifier
p_IssuedBy VARCHAR(50) -- Required: User creating the location
p_Building VARCHAR(50) -- Required: Building designation
```

#### md_locations_Update_Location
**Purpose**: Update location information
**When to Use**: Location changes, facility reorganization

**Parameters**:
```sql
p_OldLocation VARCHAR(50) -- Required: Current location identifier
p_Location VARCHAR(50) -- Required: New location identifier
p_IssuedBy VARCHAR(50) -- Required: User updating the location
p_Building VARCHAR(50) -- Required: Building designation
```

#### md_locations_Delete_ByLocation
**Purpose**: Remove storage location
**When to Use**: Facility closure, location consolidation

**Parameters**:
```sql
p_Location VARCHAR(50) -- Required: Location identifier to delete
```

## Item Types Management

#### md_item_types_Get_All
**Purpose**: Retrieve all item type classifications
**When to Use**: Type selection, classification dropdowns

**Parameters**: None

#### md_item_types_Add_ItemType
**Purpose**: Add new item type
**When to Use**: Expanding classification system

**Parameters**:
```sql
p_ItemType VARCHAR(50) -- Required: Item type name
p_IssuedBy VARCHAR(50) -- Required: User creating the type
```

#### md_item_types_Update_ItemType
**Purpose**: Update item type information
**When to Use**: Classification refinement

**Parameters**:
```sql
p_ID INT -- Required: Database ID of item type record
p_ItemType VARCHAR(50) -- Required: Item type name
p_IssuedBy VARCHAR(50) -- Required: User updating the type
```

#### md_item_types_Delete_ByType
**Purpose**: Remove item type
**When to Use**: Classification cleanup

**Parameters**:
```sql
p_Type VARCHAR(50) -- Required: Item type name to delete
```

</details>

<details>
<summary><strong>👥 User Management Procedures</strong></summary>

#### usr_users_Get_All
**Purpose**: Retrieve all system users
**When to Use**: User administration, user selection interfaces

**Parameters**: None

#### usr_users_Get_ByUser
**Purpose**: Retrieve specific user information
**When to Use**: User profile display, authentication

**Parameters**:
```sql
p_User VARCHAR(50) -- Required: Username to retrieve
```

#### usr_users_Exists
**Purpose**: Check if user exists in system
**When to Use**: User validation, duplicate prevention

**Parameters**:
```sql
p_User VARCHAR(50) -- Required: Username to check
```

#### usr_users_Add_User
**Purpose**: Add new user to the system
**When to Use**: User account creation

**Parameters**:
```sql
p_User VARCHAR(50) -- Required: Username
p_FullName VARCHAR(100) -- Optional: Full display name
p_Shift VARCHAR(10) -- Required: Work shift designation
p_VitsUser BOOLEAN -- Required: VITS system access flag
p_Pin VARCHAR(20) -- Optional: Security PIN
p_LastShownVersion VARCHAR(20) -- Required: Last application version shown
p_HideChangeLog VARCHAR(10) -- Required: Hide changelog preference
p_ThemeName VARCHAR(50) -- Required: UI theme preference
p_ThemeFontSize INT -- Required: Font size preference
p_VisualUserName VARCHAR(50) -- Required: Display username
p_VisualPassword VARCHAR(50) -- Required: Display password
p_WipServerAddress VARCHAR(50) -- Required: Database server address
p_WipServerPort VARCHAR(10) -- Required: Database server port
p_WipDatabase VARCHAR(50) -- Required: Database name
```

#### usr_users_Update_User
**Purpose**: Update existing user information
**When to Use**: User profile maintenance, preference updates

**Parameters**: Same as usr_users_Add_User

#### usr_users_Delete_User
**Purpose**: Remove user from system
**When to Use**: User account deactivation

**Parameters**:
```sql
p_User VARCHAR(50) -- Required: Username to delete
```

</details>

<details>
<summary><strong>⚙️ System Configuration Procedures</strong></summary>

#### usr_ui_settings_Get
**Purpose**: Retrieve user UI settings
**When to Use**: Loading user preferences, UI initialization

**Parameters**:
```sql
p_UserId VARCHAR(50) -- Required: User identifier
```

#### usr_ui_settings_SetJsonSetting
**Purpose**: Save user UI settings as JSON
**When to Use**: Persisting user preferences

**Parameters**:
```sql
p_UserId VARCHAR(50) -- Required: User identifier
p_SettingsJson TEXT -- Required: JSON settings data
```

#### usr_ui_settings_SetThemeJson
**Purpose**: Save user theme settings
**When to Use**: Theme preference persistence

**Parameters**:
```sql
p_UserId VARCHAR(50) -- Required: User identifier
p_ThemeJson TEXT -- Required: JSON theme data
```

#### usr_ui_settings_GetShortcutsJson
**Purpose**: Retrieve user keyboard shortcuts
**When to Use**: Loading custom keyboard mappings

**Parameters**:
```sql
p_UserId VARCHAR(50) -- Required: User identifier
```

#### usr_ui_settings_SetShortcutsJson
**Purpose**: Save user keyboard shortcuts
**When to Use**: Custom keyboard mapping persistence

**Parameters**:
```sql
p_UserId VARCHAR(50) -- Required: User identifier
p_ShortcutsJson TEXT -- Required: JSON shortcuts data
```

</details>

<details>
<summary><strong>📋 Common Usage Patterns</strong></summary>

## Standard Implementation Pattern

```csharp
public async Task<StoredProcedureResult> ExecuteStoredProcedureAsync(
    string procedureName, 
    Dictionary<string, object> parameters)
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            procedureName,
            parameters
        );

        if (result.IsSuccess)
        {
            Logger.LogInformation("Stored procedure {ProcedureName} executed successfully", procedureName);
        }
        else
        {
            Logger.LogError("Stored procedure {ProcedureName} failed: {Message}", 
                procedureName, result.Message);
        }

        return result;
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
        throw;
    }
}
```

## Error Handling Pattern

```csharp
var result = await ExecuteStoredProcedureAsync("procedure_name", parameters);

if (!result.IsSuccess)
{
    // Log error
    Logger.LogError("Operation failed: {Message}", result.Message);
    
    // Handle specific error scenarios
    if (result.Status == 1001) // Custom error code
    {
        // Handle specific business logic error
    }
    
    // Return user-friendly error
    return new OperationResult { IsSuccess = false, Message = result.Message };
}
```

## Parameter Building Pattern

```csharp
private Dictionary<string, object> BuildParameters(object data)
{
    var parameters = new Dictionary<string, object>();
    
    // Required parameters
    parameters["p_RequiredField"] = data.RequiredField;
    
    // Optional parameters with null handling
    parameters["p_OptionalField"] = !string.IsNullOrWhiteSpace(data.OptionalField) 
        ? data.OptionalField 
        : DBNull.Value;
    
    return parameters;
}
```

</details>

<details>
<summary><strong>🔍 Best Practices</strong></summary>

## Performance Optimization
- Use specific procedures for targeted queries rather than generic fetch-all operations
- Implement pagination for large result sets
- Cache master data that changes infrequently
- Use batch operations for multiple related changes

## Security Considerations
- Always use parameterized queries through stored procedures
- Validate user permissions before executing operations
- Log all data modification operations for audit trails
- Sanitize input data before processing

## Error Handling
- Always check `result.IsSuccess` before proceeding
- Log detailed error information for debugging
- Provide user-friendly error messages
- Implement retry logic for transient failures

## Transaction Management
- Use database transactions for related operations
- Implement rollback logic for failed operations
- Consider using stored procedure transactions for complex operations
- Handle concurrent access scenarios

## Logging Standards
- Log all database operations with context
- Include relevant parameters in log messages
- Use structured logging for better searchability
- Log performance metrics for optimization

</details>