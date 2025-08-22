# Create Stored Procedure - Custom Prompt

## Instructions
Use this prompt when you need to create a new stored procedure for the MTM WIP Application database.

## MTM Business Logic Rules

### **CRITICAL: Transaction Type Determination**
**TransactionType is determined by the USER'S INTENT, NOT the Operation number.**

#### **Correct TransactionType Logic**
- **IN**: User is adding stock to inventory (regardless of operation number)
- **OUT**: User is removing stock from inventory (regardless of operation number)
- **TRANSFER**: User is moving stock from one location to another (regardless of operation number)

#### **MTM Operation Numbers**
Operation numbers ("90", "100", "110", etc.) are **workflow step identifiers**, NOT transaction type indicators:
- Operations represent manufacturing or processing steps
- They help track which stage of production a part is in
- They do NOT determine whether inventory is being added, removed, or transferred
- The same operation number can be used with any TransactionType depending on user intent

## Prompt Template

```
Create a new stored procedure for [OPERATION_DESCRIPTION] with the following requirements:

**Procedure Name**: [procedure_name] (follow naming convention: {module}_{action}_{details})

**Purpose**: [Detailed description of what this procedure should do]

**Parameters**:
- IN p_[parameter1] [TYPE] - [Description]
- IN p_[parameter2] [TYPE] - [Description]
- IN p_TransactionType ENUM('IN','OUT','TRANSFER') - Transaction type based on USER INTENT
- IN p_Operation VARCHAR(100) - Workflow step number (e.g., "90", "100", "110")
- OUT p_Status INT - Operation status (0=success, 1=warning, -1=error)
- OUT p_ErrorMsg VARCHAR(255) - Status/error message

**Business Rules**:
- [Rule 1]
- [Rule 2]
- TransactionType MUST be based on user action, NOT operation number
- Operation is only used for workflow tracking, NOT transaction type determination

**Return Data**: [Description of any data returned]

**Related Tables**: [List tables this procedure will read from or write to]

**Requirements**:
1. Add to Development/Database_Files/New_Stored_Procedures.sql
2. Include comprehensive error handling with transactions
3. Validate all input parameters
4. Follow MTM database rules - NO direct SQL in application code
5. Include proper documentation in the procedure
6. Create corresponding service layer integration example
7. Include unit test examples
8. Ensure TransactionType is determined by user intent, not operation number

**Testing Requirements**:
- Test with valid parameters
- Test with invalid parameters
- Test error conditions
- Test performance with realistic data
- Verify TransactionType logic is correct (based on user intent)

Please provide:
1. Complete stored procedure SQL code
2. Documentation for the README file
3. C# service integration example
4. Unit test examples
```

## Usage Examples

### Example 1: Inventory Add Stock Procedure
```
Create a new stored procedure for adding stock to inventory with the following requirements:

**Procedure Name**: inv_inventory_Add_Stock

**Purpose**: Add stock to existing inventory item, creating IN transaction regardless of operation number

**Parameters**:
- IN p_PartID VARCHAR(300) - Part identifier
- IN p_Location VARCHAR(100) - Storage location
- IN p_Operation VARCHAR(100) - Workflow step number (for tracking only)
- IN p_Quantity INT - Quantity to add
- IN p_ItemType VARCHAR(200) - Type of inventory item
- IN p_User VARCHAR(100) - User performing action
- IN p_Notes VARCHAR(1000) - Additional notes
- IN p_TransactionType ENUM('IN','OUT','TRANSFER') - Must be 'IN' for adding stock
- OUT p_Status INT - Operation status
- OUT p_ErrorMsg VARCHAR(255) - Status message

**Business Rules**:
- Quantity must be greater than zero
- Part must exist in inventory
- User must be valid
- TransactionType MUST be 'IN' when adding stock (regardless of operation number)
- Operation number is for workflow tracking only

**Return Data**: None (status via OUT parameters)

**Related Tables**: inv_inventory, inv_transaction, md_part_ids, usr_users

[Continue with requirements...]
```

### Example 2: Inventory Transfer Procedure
```
Create a new stored procedure for transferring inventory between locations with the following requirements:

**Procedure Name**: inv_inventory_Transfer_Stock

**Purpose**: Transfer inventory from one location to another, creating TRANSFER transaction regardless of operation number

**Parameters**:
- IN p_PartID VARCHAR(300) - Part identifier
- IN p_FromLocation VARCHAR(100) - Source location
- IN p_ToLocation VARCHAR(100) - Destination location
- IN p_Operation VARCHAR(100) - Workflow step number (for tracking only)
- IN p_Quantity INT - Quantity to transfer
- IN p_User VARCHAR(100) - User performing transfer
- IN p_Notes VARCHAR(1000) - Transfer notes
- IN p_TransactionType ENUM('IN','OUT','TRANSFER') - Must be 'TRANSFER' for moving stock
- OUT p_Status INT - Operation status
- OUT p_ErrorMsg VARCHAR(255) - Status message

**Business Rules**:
- Source and destination locations must be different
- Sufficient quantity must exist at source location
- TransactionType MUST be 'TRANSFER' when moving stock (regardless of operation number)
- Operation number is for workflow tracking only
- Both locations must be valid

[Continue with requirements...]
```

### Example 3: Inventory Report Procedure
```
Create a new stored procedure for generating advanced inventory reports with the following requirements:

**Procedure Name**: inv_inventory_Get_AdvancedReport

**Purpose**: Generate comprehensive inventory report with filtering options, totals, and analytics data

**Parameters**:
- IN p_StartDate DATETIME - Report start date (NULL for all time)
- IN p_EndDate DATETIME - Report end date (NULL for current date)
- IN p_PartID VARCHAR(300) - Specific part filter (NULL for all parts)
- IN p_Location VARCHAR(100) - Location filter (NULL for all locations)
- IN p_Operation VARCHAR(100) - Operation filter (NULL for all operations) - workflow tracking only
- IN p_TransactionType ENUM('IN','OUT','TRANSFER') - Filter by transaction type (NULL for all)
- IN p_ItemType VARCHAR(100) - Item type filter (NULL for all types)
- IN p_User VARCHAR(100) - User requesting the report
- OUT p_Status INT - Operation status
- OUT p_ErrorMsg VARCHAR(255) - Status message

**Business Rules**:
- Start date must be before end date if both provided
- User must exist in usr_users table
- Only return data user has access to
- Include summary totals and analytics
- Filter invalid/test data
- Operation is used for filtering workflow steps, NOT transaction types

**Return Data**: DataTable with inventory details, quantities, values, and summary information

**Related Tables**: inv_inventory, inv_transaction, md_part_ids, md_locations, md_item_types, usr_users

[Continue with requirements...]
```

## Guidelines

### Naming Conventions
- **Inventory**: `inv_inventory_[Action]_[Details]`
- **Transactions**: `inv_transaction_[Action]_[Details]`
- **Master Data**: `md_[entity]_[Action]_[Details]`
- **User Management**: `usr_[entity]_[Action]_[Details]`
- **System**: `sys_[entity]_[Action]_[Details]`
- **Maintenance**: `maint_[Action]_[Details]`
- **Logging**: `log_[entity]_[Action]_[Details]`

### Common Actions
- **Get**: Retrieve data
- **Add**: Insert new records
- **Update**: Modify existing records
- **Delete**: Remove records
- **Process**: Complex operations
- **Calculate**: Calculations and analytics
- **Validate**: Data validation
- **Sync**: Synchronization operations

### TransactionType Parameter Requirements
All inventory-related procedures should include:
```sql
-- Always include TransactionType parameter for inventory operations
IN p_TransactionType ENUM('IN','OUT','TRANSFER') COMMENT 'Based on user intent, not operation number',
IN p_Operation VARCHAR(100) COMMENT 'Workflow step identifier only',
```

### Error Handling Requirements
All procedures must include:
- Transaction management (START TRANSACTION, COMMIT, ROLLBACK)
- SQL exception handling with proper error messages
- Input parameter validation
- Business rule validation
- Proper status codes and error messages
- TransactionType validation against user intent

### Performance Considerations
- Use indexed columns in WHERE clauses
- Limit result sets appropriately
- Consider batch processing for large operations
- Include execution time monitoring for complex procedures
- Avoid cursors when possible, use set-based operations

### Correct Transaction Logic Examples
```sql
-- ? CORRECT: TransactionType based on user intent
IF p_TransactionType = 'IN' THEN
    -- User is adding stock
    INSERT INTO inv_transaction (..., TransactionType, Operation, ...) 
    VALUES (..., 'IN', p_Operation, ...);
ELSEIF p_TransactionType = 'OUT' THEN
    -- User is removing stock
    INSERT INTO inv_transaction (..., TransactionType, Operation, ...) 
    VALUES (..., 'OUT', p_Operation, ...);
ELSEIF p_TransactionType = 'TRANSFER' THEN
    -- User is moving stock
    INSERT INTO inv_transaction (..., TransactionType, Operation, ...) 
    VALUES (..., 'TRANSFER', p_Operation, ...);
END IF;

-- ? WRONG: Don't determine TransactionType from Operation
-- IF p_Operation = '90' THEN SET p_TransactionType = 'IN'; -- NEVER DO THIS
```

## Related Files
- Add procedure to: `Development/Database_Files/New_Stored_Procedures.sql`
- Document in: `Development/Database_Files/README_New_Stored_Procedures.md`
- Service integration: Update appropriate service classes
- Tests: Create unit tests for the new procedure