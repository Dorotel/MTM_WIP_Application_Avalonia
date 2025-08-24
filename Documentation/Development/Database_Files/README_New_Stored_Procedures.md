# README: New Stored Procedures Documentation

## ?? CRITICAL FIX #1 - COMPLETED
**Status**: ? **COMPLETE - DEVELOPMENT UNBLOCKED**

---

## Overview

This document provides comprehensive documentation for all new stored procedures created in `Documentation/Development/Database_Files/New_Stored_Procedures.sql`. These procedures implement the standardized MTM inventory management operations with full error handling, input validation, and transaction management.

## ?? Complete Procedure List

### ? Enhanced Inventory Management Procedures

| Procedure Name | Purpose | Status |
|---|---|---|
| `inv_inventory_Add_Item_Enhanced` | Enhanced add inventory with full error handling | ? Complete |
| `inv_inventory_Remove_Item_Enhanced` | Enhanced remove inventory with stock validation | ? Complete |
| `inv_inventory_Transfer_Item_New` | Transfer inventory between locations | ? Complete |

### ? Inventory Query Procedures

| Procedure Name | Purpose | Status |
|---|---|---|
| `inv_inventory_Get_ByLocation_New` | Get all inventory items at specific location | ? Complete |
| `inv_inventory_Get_ByOperation_New` | Get all inventory items for specific operation | ? Complete |
| `inv_part_Get_Info_New` | Get detailed part information with inventory summary | ? Complete |
| `inv_inventory_Get_Summary_New` | Get inventory summary across all locations/operations | ? Complete |

### ? Validation and Utility Procedures

| Procedure Name | Purpose | Status |
|---|---|---|
| `inv_inventory_Validate_Stock_New` | Validate sufficient stock before removal operations | ? Complete |
| `inv_transaction_Log_New` | Log inventory transactions with comprehensive validation | ? Complete |
| `inv_location_Validate_New` | Validate location exists and is active | ? Complete |
| `inv_operation_Validate_New` | Validate operation exists and is active | ? Complete |
| `sys_user_Validate_New` | Validate user exists and is active | ? Complete |

---

## ??? Standardized Error Handling Pattern

All procedures implement the following error handling pattern:

### Output Parameters
- `OUT p_Status INT` - Status code (0=Success, 1=Warning, -1=Error)
- `OUT p_ErrorMsg VARCHAR(255)` - Descriptive error message

### Error Handler
```sql
DECLARE EXIT HANDLER FOR SQLEXCEPTION
BEGIN
    ROLLBACK;
    GET DIAGNOSTICS CONDITION 1
        p_Status = MYSQL_ERRNO,
        p_ErrorMsg = MESSAGE_TEXT;
    SET p_Status = -1;
    
    INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
    VALUES (p_ErrorMsg, 'procedure_name', p_UserID, 'Error');
END;
```

### Status Codes
- **0** = Success - Operation completed successfully
- **1** = Warning - Input validation failed or business rule violated
- **-1** = Error - SQL exception or system error occurred

---

## ?? Detailed Procedure Documentation

### 1. inv_inventory_Add_Item_Enhanced

**Purpose**: Add inventory items with comprehensive error handling and validation

**Parameters**:
- `IN p_PartID VARCHAR(50)` - Part identifier (required, must exist in parts table)
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number (required, must exist)
- `IN p_LocationID VARCHAR(50)` - Location identifier (required, must exist)
- `IN p_Quantity INT` - Quantity to add (required, must be positive)
- `IN p_UnitCost DECIMAL(10,4)` - Cost per unit (optional, defaults to 0.0000)
- `IN p_ReferenceNumber VARCHAR(100)` - Reference for transaction (optional)
- `IN p_Notes TEXT` - Additional notes (optional)
- `IN p_UserID VARCHAR(50)` - User performing operation (required, must exist)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Business Rules**:
- PartID must exist in parts table and be active
- OperationID must exist in operations table and be active
- LocationID must exist in locations table and be active
- UserID must exist in users table and be active
- Quantity must be positive integer
- Creates inventory record or updates existing quantity
- Logs transaction with TransactionType = 'IN' (user intent is adding stock)

**Example Usage**:
```sql
CALL inv_inventory_Add_Item_Enhanced(
    'PART001', '90', 'RECEIVING', 100, 5.25, 
    'PO12345', 'Initial stock receipt', 'admin', 
    @status, @msg
);
SELECT @status, @msg;
```

**Related Tables**: inventory, inventory_transactions, parts, operations, locations, users, error_log

---

### 2. inv_inventory_Remove_Item_Enhanced

**Purpose**: Remove inventory items with stock validation and error handling

**Parameters**:
- `IN p_PartID VARCHAR(50)` - Part identifier (required, must exist)
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number (required, must exist)
- `IN p_LocationID VARCHAR(50)` - Location identifier (required, must exist)
- `IN p_Quantity INT` - Quantity to remove (required, must be positive and available)
- `IN p_ReferenceNumber VARCHAR(100)` - Reference for transaction (optional)
- `IN p_Notes TEXT` - Additional notes (optional)
- `IN p_UserID VARCHAR(50)` - User performing operation (required, must exist)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Business Rules**:
- All standard validation rules apply
- Must have sufficient available stock (quantity_available >= requested quantity)
- Updates existing inventory record (cannot remove from non-existent inventory)
- Logs transaction with TransactionType = 'OUT' (user intent is removing stock)

**Example Usage**:
```sql
CALL inv_inventory_Remove_Item_Enhanced(
    'PART001', '90', 'RECEIVING', 50, 
    'WO54321', 'Production consumption', 'admin', 
    @status, @msg
);
SELECT @status, @msg;
```

**Related Tables**: inventory, inventory_transactions, parts, operations, locations, users, error_log

---

### 3. inv_inventory_Transfer_Item_New

**Purpose**: Transfer inventory items between locations with comprehensive validation

**Parameters**:
- `IN p_PartID VARCHAR(50)` - Part identifier (required, must exist)
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number (required, must exist)
- `IN p_FromLocationID VARCHAR(50)` - Source location (required, must exist and have stock)
- `IN p_ToLocationID VARCHAR(50)` - Destination location (required, must exist and be different)
- `IN p_Quantity INT` - Quantity to transfer (required, must be positive and available)
- `IN p_ReferenceNumber VARCHAR(100)` - Reference for transaction (optional)
- `IN p_Notes TEXT` - Additional notes (optional)
- `IN p_UserID VARCHAR(50)` - User performing operation (required, must exist)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Business Rules**:
- All standard validation rules apply
- FromLocationID and ToLocationID must be different
- Must have sufficient stock at source location
- Atomically removes from source and adds to destination
- Logs transaction with TransactionType = 'TRANSFER' (user intent is moving stock)

**Example Usage**:
```sql
CALL inv_inventory_Transfer_Item_New(
    'PART001', '90', 'RECEIVING', 'PRODUCTION', 25, 
    'MOVE123', 'Move to production floor', 'admin', 
    @status, @msg
);
SELECT @status, @msg;
```

**Related Tables**: inventory, inventory_transactions, parts, operations, locations, users, error_log

---

### 4. inv_inventory_Get_ByLocation_New

**Purpose**: Retrieve inventory data for a specific location with validation

**Parameters**:
- `IN p_LocationID VARCHAR(50)` - Location identifier (required, must exist)
- `IN p_UserID VARCHAR(50)` - User performing query (required for audit)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Returns**: Result set with inventory details including part descriptions, operation descriptions, quantities

**Business Rules**:
- LocationID must exist and be active
- Only returns active parts, operations, and locations
- Only returns inventory with quantity_on_hand > 0
- Results ordered by part_id, operation_id

**Example Usage**:
```sql
CALL inv_inventory_Get_ByLocation_New('RECEIVING', 'admin', @status, @msg);
SELECT @status, @msg;
```

**Related Tables**: inventory, parts, operations, locations, users, error_log

---

### 5. inv_inventory_Get_ByOperation_New

**Purpose**: Retrieve inventory data for a specific operation workflow step

**Parameters**:
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number (required, must exist)
- `IN p_UserID VARCHAR(50)` - User performing query (required for audit)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Returns**: Result set with inventory details including part descriptions, location descriptions, quantities

**Business Rules**:
- OperationID must exist and be active
- Only returns active parts, operations, and locations
- Only returns inventory with quantity_on_hand > 0
- Results ordered by part_id, location_id

**Example Usage**:
```sql
CALL inv_inventory_Get_ByOperation_New('90', 'admin', @status, @msg);
SELECT @status, @msg;
```

**Related Tables**: inventory, parts, operations, locations, users, error_log

---

### 6. inv_inventory_Validate_Stock_New

**Purpose**: Validate sufficient stock availability before removal/transfer operations

**Parameters**:
- `IN p_PartID VARCHAR(50)` - Part identifier (required, must exist)
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number (required, must exist)
- `IN p_LocationID VARCHAR(50)` - Location identifier (required, must exist)
- `IN p_RequiredQuantity INT` - Quantity needed (required, must be positive)
- `IN p_UserID VARCHAR(50)` - User performing validation (required for audit)
- `OUT p_Status INT` - Status code (0=Sufficient, 1=Insufficient)
- `OUT p_ErrorMsg VARCHAR(255)` - Status message with availability details

**Business Rules**:
- All standard validation rules apply
- Compares RequiredQuantity against quantity_available
- Returns detailed availability information in error message
- Status 0 = sufficient stock, Status 1 = insufficient stock

**Example Usage**:
```sql
CALL inv_inventory_Validate_Stock_New(
    'PART001', '90', 'RECEIVING', 100, 'admin', 
    @status, @msg
);
SELECT @status, @msg;
```

**Related Tables**: inventory, parts, operations, locations, users, error_log

---

### 7. inv_transaction_Log_New

**Purpose**: Log inventory transactions with comprehensive validation

**Parameters**:
- `IN p_TransactionType ENUM('IN', 'OUT', 'TRANSFER', 'ADJUSTMENT')` - Transaction type (required)
- `IN p_PartID VARCHAR(50)` - Part identifier (required, must exist)
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number (required, must exist)
- `IN p_FromLocationID VARCHAR(50)` - Source location (required for OUT/TRANSFER)
- `IN p_ToLocationID VARCHAR(50)` - Destination location (required for IN/TRANSFER)
- `IN p_Quantity INT` - Transaction quantity (required, must be positive)
- `IN p_UnitCost DECIMAL(10,4)` - Cost per unit (optional, defaults to 0.0000)
- `IN p_ReferenceNumber VARCHAR(100)` - Reference for transaction (optional)
- `IN p_Notes TEXT` - Additional notes (optional)
- `IN p_UserID VARCHAR(50)` - User performing operation (required, must exist)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Business Rules**:
- Transaction type specific validation:
  - IN: Requires ToLocationID
  - OUT: Requires FromLocationID
  - TRANSFER: Requires both FromLocationID and ToLocationID (must be different)
  - ADJUSTMENT: Flexible location requirements
- All referenced entities must exist and be active
- Creates transaction log entry for audit trail

**Example Usage**:
```sql
CALL inv_transaction_Log_New(
    'IN', 'PART001', '90', NULL, 'RECEIVING', 100, 5.25, 
    'PO12345', 'Initial receipt', 'admin', 
    @status, @msg
);
SELECT @status, @msg;
```

**Related Tables**: inventory_transactions, parts, operations, locations, users, error_log

---

### 8. inv_location_Validate_New

**Purpose**: Validate that a location exists and is active

**Parameters**:
- `IN p_LocationID VARCHAR(50)` - Location identifier to validate (required)
- `IN p_UserID VARCHAR(50)` - User performing validation (required for audit)
- `OUT p_Status INT` - Status code (0=Valid, 1=Invalid)
- `OUT p_ErrorMsg VARCHAR(255)` - Status message with location details

**Business Rules**:
- Checks if location exists in locations table
- Verifies location is active (active_status = TRUE)
- Returns location description in success message

**Example Usage**:
```sql
CALL inv_location_Validate_New('RECEIVING', 'admin', @status, @msg);
SELECT @status, @msg;
```

**Related Tables**: locations, users, error_log

---

### 9. inv_operation_Validate_New

**Purpose**: Validate that an operation workflow step exists and is active

**Parameters**:
- `IN p_OperationID VARCHAR(10)` - Operation workflow step number to validate (required)
- `IN p_UserID VARCHAR(50)` - User performing validation (required for audit)
- `OUT p_Status INT` - Status code (0=Valid, 1=Invalid)
- `OUT p_ErrorMsg VARCHAR(255)` - Status message with operation details

**Business Rules**:
- Checks if operation exists in operations table
- Verifies operation is active (active_status = TRUE)
- Returns operation description in success message
- Remember: Operations are workflow step identifiers, NOT transaction type indicators

**Example Usage**:
```sql
CALL inv_operation_Validate_New('90', 'admin', @status, @msg);
SELECT @status, @msg;
```

**Related Tables**: operations, users, error_log

---

### 10. sys_user_Validate_New

**Purpose**: Validate that a user exists and is active

**Parameters**:
- `IN p_UserID VARCHAR(50)` - User identifier to validate (required)
- `IN p_ValidatingUserID VARCHAR(50)` - User performing validation (required for audit)
- `OUT p_Status INT` - Status code (0=Valid, 1=Invalid)
- `OUT p_ErrorMsg VARCHAR(255)` - Status message with user details

**Business Rules**:
- Checks if user exists in users table
- Verifies user is active (active_status = TRUE)
- Validates that the validating user also exists and is active
- Returns user full name and role in success message

**Example Usage**:
```sql
CALL sys_user_Validate_New('testuser', 'admin', @status, @msg);
SELECT @status, @msg;
```

**Related Tables**: users, error_log

---

### 11. inv_part_Get_Info_New

**Purpose**: Get detailed part information with inventory summary

**Parameters**:
- `IN p_PartID VARCHAR(50)` - Part identifier (required, must exist)
- `IN p_UserID VARCHAR(50)` - User performing query (required for audit)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Returns**: Result set with part details and aggregated inventory information

**Business Rules**:
- PartID must exist and be active
- Returns part master data plus inventory totals
- Includes total quantities across all locations/operations
- Shows count of inventory locations for the part

**Example Usage**:
```sql
CALL inv_part_Get_Info_New('PART001', 'admin', @status, @msg);
SELECT @status, @msg;
```

**Related Tables**: parts, inventory, users, error_log

---

### 12. inv_inventory_Get_Summary_New

**Purpose**: Get inventory summary for parts across all locations/operations

**Parameters**:
- `IN p_PartID VARCHAR(50)` - Part identifier (optional, if NULL returns all parts)
- `IN p_UserID VARCHAR(50)` - User performing query (required for audit)
- `OUT p_Status INT` - Status code
- `OUT p_ErrorMsg VARCHAR(255)` - Status message

**Returns**: Result set with consolidated inventory summary

**Business Rules**:
- If PartID is NULL or empty, returns summary for all parts with inventory
- If PartID is specified, returns summary for that part only
- Only includes active parts
- Shows totals across all locations and operations
- Includes location count, operation count, and last activity date

**Example Usage**:
```sql
-- Get summary for specific part
CALL inv_inventory_Get_Summary_New('PART001', 'admin', @status, @msg);

-- Get summary for all parts
CALL inv_inventory_Get_Summary_New(NULL, 'admin', @status, @msg);

SELECT @status, @msg;
```

**Related Tables**: parts, inventory, users, error_log

---

## ?? Integration with Service Layer

### C# Service Layer Integration Pattern

All procedures are designed to work with the existing `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` method:

```csharp
// Example: Add inventory item
var parameters = new Dictionary<string, object>
{
    ["p_PartID"] = "PART001",
    ["p_OperationID"] = "90",
    ["p_LocationID"] = "RECEIVING",
    ["p_Quantity"] = 100,
    ["p_UnitCost"] = 5.25m,
    ["p_ReferenceNumber"] = "PO12345",
    ["p_Notes"] = "Initial stock receipt",
    ["p_UserID"] = "admin"
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item_Enhanced",
    parameters
);

// Check result status
int status = result.Status;
string message = result.Message;
```

### Error Handling Pattern in Service Layer

```csharp
public async Task<Result<bool>> AddInventoryItemAsync(
    string partId, string operationId, string locationId, int quantity,
    decimal? unitCost = null, string referenceNumber = null, 
    string notes = null, string userId = null)
{
    try
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_PartID"] = partId,
            ["p_OperationID"] = operationId,
            ["p_LocationID"] = locationId,
            ["p_Quantity"] = quantity,
            ["p_UnitCost"] = unitCost,
            ["p_ReferenceNumber"] = referenceNumber,
            ["p_Notes"] = notes,
            ["p_UserID"] = userId ?? _currentUser.UserId
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Add_Item_Enhanced",
            parameters
        );

        if (result.Status == 0)
        {
            return Result<bool>.Success(true, result.Message);
        }
        else if (result.Status == 1)
        {
            return Result<bool>.Warning(false, result.Message);
        }
        else
        {
            return Result<bool>.Error(result.Message);
        }
    }
    catch (Exception ex)
    {
        return Result<bool>.Error($"Service layer error: {ex.Message}");
    }
}
```

---

## ?? MTM Business Logic Compliance

### Critical Business Rule: TransactionType Determination

**? CORRECT IMPLEMENTATION**: All procedures follow the correct pattern where **TransactionType is determined by USER INTENT, NOT operation numbers**:

- **inv_inventory_Add_Item_Enhanced**: Always logs TransactionType = 'IN' because user intent is adding stock
- **inv_inventory_Remove_Item_Enhanced**: Always logs TransactionType = 'OUT' because user intent is removing stock  
- **inv_inventory_Transfer_Item_New**: Always logs TransactionType = 'TRANSFER' because user intent is moving stock

### Operation Numbers Are Workflow Steps

Operation numbers ("90", "100", "110", etc.) are **workflow step identifiers only**:
- They represent manufacturing or processing stages
- They help track which phase of production a part is in
- They do NOT determine transaction type
- The same operation can be used with any TransactionType depending on user action

### Transaction Validation Rules

- **Negative quantities**: Not allowed (all procedures validate quantity > 0)
- **Location transfers**: Both source and destination must be validated and different
- **Stock availability**: Always checked before removals and transfers
- **Entity existence**: All referenced parts, operations, locations, and users must exist and be active
- **Audit trail**: All transactions logged with proper user context

---

## ?? Testing and Validation

### Validation Checklist

- [x] **Syntax Validation**: All procedures compile without syntax errors
- [x] **Error Handling**: All procedures implement standardized error handling pattern
- [x] **Input Validation**: All required parameters validated for NULL/empty values
- [x] **Business Rules**: All MTM business rules implemented and enforced
- [x] **Transaction Management**: All data modifications wrapped in transactions
- [x] **Output Parameters**: All procedures return standardized status and message
- [x] **Documentation**: All procedures fully documented with examples
- [x] **Audit Trail**: All procedures log to error_log on exceptions
- [x] **User Context**: All procedures set @current_user_id for audit triggers

### Test Scenarios

#### Successful Operations
```sql
-- Test successful add
CALL inv_inventory_Add_Item_Enhanced('PART001', '90', 'RECEIVING', 100, 5.25, 'PO123', 'Test add', 'admin', @status, @msg);

-- Test successful remove  
CALL inv_inventory_Remove_Item_Enhanced('PART001', '90', 'RECEIVING', 50, 'WO456', 'Test remove', 'admin', @status, @msg);

-- Test successful transfer
CALL inv_inventory_Transfer_Item_New('PART001', '90', 'RECEIVING', 'PRODUCTION', 25, 'TF789', 'Test transfer', 'admin', @status, @msg);
```

#### Error Scenarios
```sql
-- Test invalid part
CALL inv_inventory_Add_Item_Enhanced('INVALID', '90', 'RECEIVING', 100, 5.25, 'PO123', 'Test', 'admin', @status, @msg);

-- Test insufficient stock
CALL inv_inventory_Remove_Item_Enhanced('PART001', '90', 'RECEIVING', 999999, 'WO456', 'Test', 'admin', @status, @msg);

-- Test same location transfer
CALL inv_inventory_Transfer_Item_New('PART001', '90', 'RECEIVING', 'RECEIVING', 25, 'TF789', 'Test', 'admin', @status, @msg);
```

#### Validation Scenarios
```sql
-- Test stock validation
CALL inv_inventory_Validate_Stock_New('PART001', '90', 'RECEIVING', 100, 'admin', @status, @msg);

-- Test entity validation
CALL inv_location_Validate_New('RECEIVING', 'admin', @status, @msg);
CALL inv_operation_Validate_New('90', 'admin', @status, @msg);
CALL sys_user_Validate_New('admin', 'admin', @status, @msg);
```

---

## ?? Ready for Service Layer Integration

### Next Steps

1. **? Database Procedures**: All 12 procedures created and documented
2. **?? Service Layer**: Ready to implement C# service classes that call these procedures
3. **?? UI Integration**: Ready to bind ViewModels to service layer methods
4. **?? Error Handling**: Ready to implement UI error handling using standardized status codes

### Service Layer Development

The procedures are now ready for integration into the following service classes:
- **InventoryService**: Add, remove, transfer operations
- **ValidationService**: Stock validation, entity validation
- **QueryService**: Location queries, operation queries, summaries
- **TransactionService**: Transaction logging and history

### Benefits Achieved

- **??? Error Resilience**: Comprehensive error handling prevents system crashes
- **?? Input Validation**: All user inputs validated before processing
- **?? Business Rules**: MTM-specific logic properly implemented
- **??? Transaction Safety**: All operations are atomic and consistent
- **?? Audit Trail**: Complete logging for troubleshooting and compliance
- **?? Service Ready**: Standardized interface for C# service layer integration

---

## ?? Support and Troubleshooting

### Common Error Patterns

1. **p_Status = 1**: Input validation or business rule violation
   - Check error message for specific validation failure
   - Verify all required parameters are provided
   - Ensure referenced entities exist and are active

2. **p_Status = -1**: SQL exception or system error
   - Check error_log table for detailed exception information
   - Verify database connectivity and permissions
   - Check for constraint violations or data type mismatches

3. **Transaction Rollbacks**: Automatic rollback on any exception
   - Review transaction logs to understand rollback cause
   - Ensure proper transaction boundaries in calling code
   - Check for deadlocks or locking issues

### Debugging Tips

- Always check both p_Status and p_ErrorMsg after calling procedures
- Use error_log table to investigate SQL exceptions
- Verify entity existence before calling operational procedures
- Test with validation procedures before performing operations
- Use transaction history to verify expected behavior

---

*End of Documentation - All procedures ready for production use*