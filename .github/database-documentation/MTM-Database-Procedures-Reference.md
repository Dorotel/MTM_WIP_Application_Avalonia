# MTM Database Documentation - Stored Procedures Reference

## 📋 Overview

The MTM WIP Application uses **MySQL 9.4.0** with a **stored procedures only** architecture for all database operations. This ensures consistent data access, enhanced security, and centralized business logic.

### 🔗 Database Access Pattern
All database operations use the standardized helper method:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "stored_procedure_name",
    parameters
);
```

## 📊 Stored Procedures Catalog

### 🏭 **Inventory Management Procedures**

#### **Core Inventory Operations**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `inv_inventory_Add_Item` | Add new inventory item | `p_PartID`, `p_Operation`, `p_Quantity`, `p_Location`, `p_UserID` | Status + ItemID |
| `inv_inventory_Remove_Item` | Remove inventory item | `p_PartID`, `p_Operation`, `p_Quantity`, `p_Location`, `p_UserID` | Status + RemovedQty |
| `inv_inventory_Transfer_Item` | Transfer between locations | `p_PartID`, `p_FromLocation`, `p_ToLocation`, `p_Quantity`, `p_UserID` | Status + TransferID |
| `inv_inventory_Get_ByPartID` | Get inventory by part ID | `p_PartID` | DataTable with inventory details |
| `inv_inventory_Get_ByLocation` | Get inventory by location | `p_Location` | DataTable with location inventory |
| `inv_inventory_Get_ByPartIDandOperation` | Get specific part/operation | `p_PartID`, `p_Operation` | DataTable with specific inventory |

#### **Advanced Inventory Queries**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `inv_inventory_Get_All` | Get complete inventory | None | DataTable with all inventory |
| `inv_inventory_Get_LowStock` | Get low stock alerts | `p_ThresholdQty` | DataTable with low stock items |
| `inv_inventory_Get_ByDateRange` | Get inventory changes by date | `p_StartDate`, `p_EndDate` | DataTable with date-filtered inventory |
| `inv_inventory_Get_Summary` | Get inventory summary stats | `p_Location` (optional) | DataTable with summary statistics |
| `inv_inventory_Validate_Operation` | Validate operation exists | `p_PartID`, `p_Operation` | Status + ValidationResult |

### 💱 **Transaction Management Procedures**

#### **Transaction Operations**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `inv_transaction_Add` | Add new transaction | `p_PartID`, `p_Operation`, `p_TransactionType`, `p_Quantity`, `p_Location`, `p_UserID`, `p_Notes` | Status + TransactionID |
| `inv_transaction_Get_History` | Get transaction history | `p_PartID`, `p_StartDate`, `p_EndDate` | DataTable with transaction history |
| `inv_transaction_Get_ByType` | Get transactions by type | `p_TransactionType`, `p_StartDate`, `p_EndDate` | DataTable with filtered transactions |
| `inv_transaction_Get_ByUser` | Get user transactions | `p_UserID`, `p_StartDate`, `p_EndDate` | DataTable with user transactions |
| `inv_transaction_Get_Daily_Summary` | Get daily transaction summary | `p_Date` | DataTable with daily summary |

#### **Transaction Analysis**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `inv_transaction_Get_Weekly_Report` | Weekly transaction report | `p_WeekStartDate` | DataTable with weekly analytics |
| `inv_transaction_Get_Monthly_Report` | Monthly transaction report | `p_Month`, `p_Year` | DataTable with monthly analytics |
| `inv_transaction_Get_Top_Parts` | Most active parts | `p_StartDate`, `p_EndDate`, `p_Limit` | DataTable with top parts by activity |

### 🗂️ **Master Data Procedures**

#### **Part Management**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `md_part_ids_Get_All` | Get all valid part IDs | None | DataTable with all part IDs |
| `md_part_ids_Add` | Add new part ID | `p_PartID`, `p_Description`, `p_UserID` | Status + Success indicator |
| `md_part_ids_Validate` | Validate part ID exists | `p_PartID` | Status + ValidationResult |
| `md_part_ids_Get_Details` | Get part details | `p_PartID` | DataTable with part information |
| `md_part_ids_Update_Description` | Update part description | `p_PartID`, `p_Description`, `p_UserID` | Status + Success indicator |

#### **Location Management**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `md_locations_Get_All` | Get all valid locations | None | DataTable with all locations |
| `md_locations_Add` | Add new location | `p_Location`, `p_Description`, `p_UserID` | Status + Success indicator |
| `md_locations_Validate` | Validate location exists | `p_Location` | Status + ValidationResult |
| `md_locations_Get_Details` | Get location details | `p_Location` | DataTable with location information |
| `md_locations_Update_Description` | Update location description | `p_Location`, `p_Description`, `p_UserID` | Status + Success indicator |

#### **Operation Management**
| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `md_operation_numbers_Get_All` | Get all operation numbers | None | DataTable with all operations |
| `md_operation_numbers_Add` | Add new operation | `p_Operation`, `p_Description`, `p_UserID` | Status + Success indicator |
| `md_operation_numbers_Validate` | Validate operation exists | `p_Operation` | Status + ValidationResult |
| `md_operation_numbers_Get_Details` | Get operation details | `p_Operation` | DataTable with operation information |

### 📝 **Error Logging Procedures**

| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `log_error_Add_Error` | Log application error | `p_ErrorMessage`, `p_StackTrace`, `p_UserID`, `p_Severity` | Status + ErrorID |
| `log_error_Get_All` | Get all error logs | `p_StartDate`, `p_EndDate` | DataTable with error logs |
| `log_error_Get_By_Severity` | Get errors by severity | `p_Severity`, `p_StartDate`, `p_EndDate` | DataTable with filtered errors |
| `log_error_Get_Statistics` | Get error statistics | `p_StartDate`, `p_EndDate` | DataTable with error analytics |

### 🔧 **System Maintenance Procedures**

| Procedure Name | Purpose | Parameters | Returns |
|----------------|---------|------------|---------|
| `sys_maintenance_Archive_Old_Transactions` | Archive old transactions | `p_ArchiveDate` | Status + ArchivedCount |
| `sys_maintenance_Cleanup_Error_Logs` | Cleanup old error logs | `p_RetentionDays` | Status + DeletedCount |
| `sys_maintenance_Get_Database_Stats` | Get database statistics | None | DataTable with DB stats |
| `sys_maintenance_Rebuild_Indexes` | Rebuild database indexes | `p_TableName` (optional) | Status + RebuildCount |

## 🏗️ **Database Schema Overview**

### **Core Tables Structure**
```sql
-- Primary inventory table
inventory_items (
    ItemID, PartID, Operation, Quantity, 
    Location, CreatedDate, UpdatedDate, UserID
)

-- Transaction history
inventory_transactions (
    TransactionID, PartID, Operation, TransactionType,
    Quantity, Location, TransactionDate, UserID, Notes
)

-- Master data tables
part_ids (PartID, Description, IsActive, CreatedDate)
locations (Location, Description, IsActive, CreatedDate)
operation_numbers (Operation, Description, IsActive, CreatedDate)

-- System tables
error_logs (ErrorID, ErrorMessage, StackTrace, Severity, CreatedDate, UserID)
user_sessions (SessionID, UserID, LoginTime, LastActivity)
```

## 🔄 **Transaction Types & Business Logic**

### **Transaction Type Mapping**
- **"IN"** - Adding inventory to the system
- **"OUT"** - Removing inventory from the system  
- **"TRANSFER"** - Moving inventory between locations
- **"ADJUSTMENT"** - Inventory corrections/adjustments

### **Operation Numbers (Manufacturing Workflow Steps)**
- **"90"** - Initial operation/receipt
- **"100"** - First processing step
- **"110"** - Second processing step
- **"120"** - Final processing step

**Important**: Operation numbers represent workflow stages, NOT transaction types.

## 🛡️ **Security & Best Practices**

### **Parameter Security**
All procedures use parameterized queries to prevent SQL injection:
```csharp
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Operation", operation),
    new("p_Quantity", quantity)
};
```

### **Error Handling Pattern**
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    procedureName,
    parameters
);

if (result.Status == 1)
{
    // Success - process result.Data
    var dataTable = result.Data;
}
else
{
    // Error - handle failure
    await Services.ErrorHandling.HandleErrorAsync(
        new Exception($"Database operation failed: {procedureName}"), 
        "Database operation context"
    );
}
```

### **Connection Management**
- All procedures use connection pooling
- Connections are automatically managed by the helper
- No manual connection lifecycle management required

## 📈 **Performance Considerations**

### **Indexing Strategy**
- Primary keys on all ID columns
- Composite indexes on frequently queried combinations:
  - `(PartID, Operation)` for inventory lookups
  - `(Location, CreatedDate)` for location-based queries
  - `(TransactionDate, TransactionType)` for reporting

### **Query Optimization**
- All procedures include execution plans
- Regular ANALYZE TABLE maintenance
- Automated statistics updates via maintenance procedures

## 🔍 **Monitoring & Maintenance**

### **Database Health Monitoring**
Use `sys_maintenance_Get_Database_Stats` for:
- Table sizes and row counts
- Index usage statistics
- Query performance metrics
- Connection pool status

### **Automated Maintenance Schedule**
- **Daily**: Error log cleanup (30-day retention)
- **Weekly**: Transaction archival (1-year retention)
- **Monthly**: Index rebuilding and statistics updates

This documentation provides comprehensive coverage of all database operations in the MTM WIP Application, ensuring developers have complete reference material for the stored procedures architecture.
