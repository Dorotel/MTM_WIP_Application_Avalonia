# MTM Database Stored Procedures Context

## 🗄️ Complete Database Stored Procedures Catalog

### **Inventory Management Procedures**

#### **inv_inventory_* Procedures**
```sql
-- Add inventory item
inv_inventory_Add_Item(
    p_PartID VARCHAR(50),
    p_Location VARCHAR(50), 
    p_Operation VARCHAR(10),
    p_Quantity INT,
    p_ItemType VARCHAR(50),
    p_User VARCHAR(50),
    p_Notes TEXT
)

-- Get inventory by part ID
inv_inventory_Get_ByPartID(p_PartID VARCHAR(50))

-- Get inventory by part ID and operation
inv_inventory_Get_ByPartIDandOperation(
    p_PartID VARCHAR(50),
    p_Operation VARCHAR(10)
)

-- Remove inventory item with status output
inv_inventory_Remove_Item(
    p_PartID VARCHAR(50),
    p_Location VARCHAR(50),
    p_Operation VARCHAR(10), 
    p_Quantity INT,
    p_ItemType VARCHAR(50),
    p_User VARCHAR(50),
    p_BatchNumber VARCHAR(50),
    p_Notes TEXT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)

-- Transfer inventory between locations
inv_inventory_Transfer_Part(
    in_BatchNumber VARCHAR(50),
    in_PartID VARCHAR(50),
    in_Operation VARCHAR(10),
    in_NewLocation VARCHAR(50)
)

-- Transfer specific quantity 
inv_inventory_Transfer_Quantity(
    in_BatchNumber VARCHAR(50),
    in_PartID VARCHAR(50),
    in_Operation VARCHAR(10),
    in_TransferQuantity INT,
    in_OriginalQuantity INT,
    in_NewLocation VARCHAR(50),
    in_User VARCHAR(50)
)

-- Get all inventory records
inv_inventory_Get_All()

-- Get inventory by location
inv_inventory_Get_ByLocation(p_Location VARCHAR(50))

-- Get inventory by operation
inv_inventory_Get_ByOperation(p_Operation VARCHAR(10))

-- Update inventory item
inv_inventory_Update_Item(
    p_ID INT,
    p_PartID VARCHAR(50),
    p_Location VARCHAR(50),
    p_Operation VARCHAR(10),
    p_Quantity INT,
    p_ItemType VARCHAR(50),
    p_Notes TEXT
)
```

#### **inv_transaction_* Procedures**
```sql
-- Add transaction record
inv_transaction_Add(
    in_TransactionType VARCHAR(10), -- 'IN', 'OUT', 'TRANSFER'
    in_PartID VARCHAR(50),
    in_BatchNumber VARCHAR(50),
    in_FromLocation VARCHAR(50),
    in_ToLocation VARCHAR(50),
    in_Operation VARCHAR(10),
    in_Quantity INT,
    in_Notes TEXT,
    in_User VARCHAR(50),
    in_ItemType VARCHAR(50),
    in_ReceiveDate DATETIME
)

-- Get transaction history
inv_transaction_Get_History(
    p_PartID VARCHAR(50),
    p_StartDate DATETIME,
    p_EndDate DATETIME
)

-- Get all transactions
inv_transaction_Get_All()

-- Get transactions by user
inv_transaction_Get_ByUser(p_User VARCHAR(50))

-- Get transactions by type
inv_transaction_Get_ByType(p_TransactionType VARCHAR(10))
```

### **Master Data Procedures**

#### **md_part_ids_* Procedures**
```sql
-- Add new part
md_part_ids_Add_Part(
    p_ItemNumber VARCHAR(50),
    p_Customer VARCHAR(100),
    p_Description TEXT,
    p_IssuedBy VARCHAR(50),
    p_ItemType VARCHAR(50)
)

-- Get all parts
md_part_ids_Get_All()

-- Get part by item number
md_part_ids_Get_ByItemNumber(p_ItemNumber VARCHAR(50))

-- Update part information
md_part_ids_Update_Part(
    p_ID INT,
    p_ItemNumber VARCHAR(50),
    p_Customer VARCHAR(100),
    p_Description TEXT,
    p_IssuedBy VARCHAR(50),
    p_ItemType VARCHAR(50)
)

-- Delete part by item number
md_part_ids_Delete_ByItemNumber(p_ItemNumber VARCHAR(50))

-- Get parts by customer
md_part_ids_Get_ByCustomer(p_Customer VARCHAR(100))

-- Get parts by item type
md_part_ids_Get_ByItemType(p_ItemType VARCHAR(50))
```

#### **md_locations_* Procedures**
```sql
-- Add new location
md_locations_Add_Location(
    p_Location VARCHAR(50),
    p_IssuedBy VARCHAR(50),
    p_Building VARCHAR(50)
)

-- Get all locations
md_locations_Get_All()

-- Get location by name
md_locations_Get_ByLocation(p_Location VARCHAR(50))

-- Update location
md_locations_Update_Location(
    p_OldLocation VARCHAR(50),
    p_Location VARCHAR(50), 
    p_IssuedBy VARCHAR(50),
    p_Building VARCHAR(50)
)

-- Delete location
md_locations_Delete_ByLocation(p_Location VARCHAR(50))

-- Get locations by building
md_locations_Get_ByBuilding(p_Building VARCHAR(50))
```

#### **md_operation_numbers_* Procedures**
```sql
-- Add new operation
md_operation_numbers_Add_Operation(
    p_Operation VARCHAR(10),
    p_IssuedBy VARCHAR(50)
)

-- Get all operations
md_operation_numbers_Get_All()

-- Get operation by number
md_operation_numbers_Get_ByOperation(p_Operation VARCHAR(10))

-- Update operation
md_operation_numbers_Update_Operation(
    p_Operation VARCHAR(10),
    p_NewOperation VARCHAR(10),
    p_IssuedBy VARCHAR(50)
)

-- Delete operation
md_operation_numbers_Delete_ByOperation(p_Operation VARCHAR(10))
```

#### **md_item_types_* Procedures**
```sql
-- Add new item type
md_item_types_Add_ItemType(
    p_ItemType VARCHAR(50),
    p_IssuedBy VARCHAR(50)
)

-- Get all item types
md_item_types_Get_All()

-- Get item type by name
md_item_types_Get_ByType(p_ItemType VARCHAR(50))

-- Update item type
md_item_types_Update_ItemType(
    p_ID INT,
    p_ItemType VARCHAR(50),
    p_IssuedBy VARCHAR(50)
)

-- Delete item type
md_item_types_Delete_ByType(p_ItemType VARCHAR(50))
```

### **Error Logging Procedures**

#### **log_error_* Procedures**
```sql
-- Add error log entry
log_error_Add_Error(
    p_User VARCHAR(50),
    p_Severity VARCHAR(20),
    p_ErrorType VARCHAR(50),
    p_ErrorMessage TEXT,
    p_StackTrace TEXT,
    p_ModuleName VARCHAR(100),
    p_MethodName VARCHAR(100),
    p_AdditionalInfo TEXT,
    p_MachineName VARCHAR(100),
    p_OSVersion VARCHAR(50),
    p_AppVersion VARCHAR(20),
    p_ErrorTime DATETIME,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)

-- Get all error logs
log_error_Get_All(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)

-- Get errors by user
log_error_Get_ByUser(
    p_User VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)

-- Get errors by severity
log_error_Get_BySeverity(
    p_Severity VARCHAR(20),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)

-- Delete all error logs
log_error_Delete_All(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)

-- Get errors by date range
log_error_Get_ByDateRange(
    p_StartDate DATETIME,
    p_EndDate DATETIME,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)
```

### **User Management Procedures**

#### **user_* Procedures**
```sql
-- Add new user
user_Add_User(
    p_Username VARCHAR(50),
    p_Password VARCHAR(255), -- Hashed
    p_FirstName VARCHAR(50),
    p_LastName VARCHAR(50),
    p_Email VARCHAR(100),
    p_Role VARCHAR(20),
    p_Department VARCHAR(50),
    p_IssuedBy VARCHAR(50)
)

-- Get user by username
user_Get_ByUsername(p_Username VARCHAR(50))

-- Update user information
user_Update_User(
    p_Username VARCHAR(50),
    p_FirstName VARCHAR(50),
    p_LastName VARCHAR(50),
    p_Email VARCHAR(100),
    p_Role VARCHAR(20),
    p_Department VARCHAR(50)
)

-- Get all users
user_Get_All()

-- Delete user
user_Delete_User(p_Username VARCHAR(50))

-- Update user settings
user_Update_Settings(
    p_Username VARCHAR(50),
    p_SettingName VARCHAR(50),
    p_SettingValue TEXT
)

-- Get user settings
user_Get_Settings(p_Username VARCHAR(50))
```

### **Reporting Procedures**

#### **report_* Procedures**
```sql
-- Inventory summary report
report_Inventory_Summary(
    p_StartDate DATETIME,
    p_EndDate DATETIME
)

-- Transaction summary report
report_Transaction_Summary(
    p_StartDate DATETIME,
    p_EndDate DATETIME,
    p_TransactionType VARCHAR(10)
)

-- Inventory by location report
report_Inventory_ByLocation(p_Location VARCHAR(50))

-- Low inventory alert
report_LowInventory_Alert(p_ThresholdQuantity INT)

-- Inventory aging report
report_Inventory_Aging(p_DaysThreshold INT)

-- Part usage report
report_Part_Usage(
    p_PartID VARCHAR(50),
    p_StartDate DATETIME,
    p_EndDate DATETIME
)
```

### **Helper/Utility Procedures**

#### **util_* Procedures**
```sql
-- Generate batch numbers
util_Generate_BatchNumber()

-- Validate inventory levels
util_Validate_Inventory_Levels()

-- Data cleanup utilities
util_Cleanup_OldTransactions(p_DaysToKeep INT)

-- Database maintenance
util_Rebuild_Indexes()

-- Backup operations
util_Backup_Database()
```

### **Stored Procedure Usage Patterns**

#### **Standard Call Pattern**
```csharp
// C# usage example
var parameters = new MySqlParameter[]
{
    new("p_PartID", "PART001"),
    new("p_Location", "WAREHOUSE_A"),
    new("p_Operation", "90")
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Get_ByPartIDandOperation",
    parameters
);

if (result.Status == 1)
{
    // Success - process result.Data
    var inventory = ProcessInventoryData(result.Data);
}
else
{
    // Handle error condition
    Logger.LogWarning("Stored procedure failed with status: {Status}", result.Status);
}
```

#### **Output Parameter Pattern**
```csharp
// For procedures with output parameters
var parameters = new MySqlParameter[]
{
    new("p_PartID", "PART001"),
    new("p_Quantity", 10),
    new("@p_Status", MySqlDbType.Int32) { Direction = ParameterDirection.Output },
    new("@p_ErrorMsg", MySqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
};

await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Remove_Item",
    parameters
);

var status = (int)parameters.First(p => p.ParameterName == "@p_Status").Value;
var errorMessage = parameters.First(p => p.ParameterName == "@p_ErrorMsg").Value?.ToString();
```

### **Database Schema Relationships**

#### **Key Relationships**
```
inv_inventory
├── FK: PartID → md_part_ids.ItemNumber
├── FK: Location → md_locations.Location  
├── FK: Operation → md_operation_numbers.Operation
└── FK: ItemType → md_item_types.ItemType

inv_transaction
├── FK: PartID → md_part_ids.ItemNumber
├── FK: FromLocation → md_locations.Location
├── FK: ToLocation → md_locations.Location
└── FK: Operation → md_operation_numbers.Operation
```

#### **Common Data Types**
```
PartID: VARCHAR(50) - Part identifier
Location: VARCHAR(50) - Location code  
Operation: VARCHAR(10) - Usually numeric string ("90", "100")
Quantity: INT - Always positive integers
BatchNumber: VARCHAR(50) - Batch tracking number
User: VARCHAR(50) - Username performing operation
Notes: TEXT - Optional descriptive text
Timestamp: DATETIME - UTC timestamps
```