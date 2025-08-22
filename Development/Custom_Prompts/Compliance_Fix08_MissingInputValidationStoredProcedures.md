# Custom Prompt: Fix Missing Input Validation in Stored Procedures

## ⚠️ HIGH PRIORITY FIX #8

**Issue**: Stored procedures don't validate required parameters or enforce business rules before processing.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- `inv_inventory_Add_Item` - Doesn't validate PartID exists in md_part_ids
- Location validation missing in inventory procedures
- Operation number validation missing
- User validation missing in most procedures

**Priority**: ⚠️ **HIGH - DATA QUALITY IMPACT**

---

## Custom Prompt

```
HIGH PRIORITY DATA INTEGRITY: Implement comprehensive input validation in all stored procedures to enforce business rules and prevent invalid data from entering the database.

REQUIREMENTS:
1. Add parameter null/empty checks at procedure start
2. Validate foreign key relationships (PartID, Location, Operation, User)
3. Implement business rule validation (positive quantities, valid dates)
4. Return specific validation error messages
5. Use LEAVE statements to exit procedures on validation failures
6. Create validation helper procedures for reusable checks
7. Test all validation scenarios thoroughly

VALIDATION PATTERNS TO IMPLEMENT:

**Basic Parameter Validation**:
```sql
-- Template for all procedures
DELIMITER ;;

CREATE PROCEDURE procedure_name(
    IN p_PartID VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_Quantity INT,
    IN p_Location VARCHAR(50),
    IN p_User VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    -- STEP 1: Parameter Null/Empty Validation
    IF p_PartID IS NULL OR TRIM(p_PartID) = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be null or empty';
        LEAVE procedure_name;
    END IF;

    IF p_Operation IS NULL OR TRIM(p_Operation) = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Operation is required and cannot be null or empty';
        LEAVE procedure_name;
    END IF;

    IF p_Location IS NULL OR TRIM(p_Location) = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Location is required and cannot be null or empty';
        LEAVE procedure_name;
    END IF;

    IF p_User IS NULL OR TRIM(p_User) = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'User is required and cannot be null or empty';
        LEAVE procedure_name;
    END IF;

    -- STEP 2: Data Type and Range Validation
    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Quantity must be a positive integer, received: ', COALESCE(p_Quantity, 'NULL'));
        LEAVE procedure_name;
    END IF;

    -- STEP 3: Foreign Key Validation
    -- Validate PartID exists
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID AND IsActive = 1) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Invalid PartID: "', p_PartID, '" does not exist in master data or is inactive');
        LEAVE procedure_name;
    END IF;

    -- Validate Location exists
    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE LocationID = p_Location AND IsActive = 1) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Invalid Location: "', p_Location, '" does not exist in master data or is inactive');
        LEAVE procedure_name;
    END IF;

    -- Validate Operation exists
    IF NOT EXISTS (SELECT 1 FROM md_operations WHERE OperationID = p_Operation AND IsActive = 1) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Invalid Operation: "', p_Operation, '" does not exist in master data or is inactive');
        LEAVE procedure_name;
    END IF;

    -- Validate User exists
    IF NOT EXISTS (SELECT 1 FROM md_users WHERE UserID = p_User AND IsActive = 1) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Invalid User: "', p_User, '" does not exist in user directory or is inactive');
        LEAVE procedure_name;
    END IF;

    -- STEP 4: Business Rule Validation
    -- Add specific business rules here...

    START TRANSACTION;
    
    -- Business logic here after all validation passes
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = 'Operation completed successfully';
    
END;;

DELIMITER ;
```

SPECIFIC VALIDATION PROCEDURES TO CREATE:

1. **inv_inventory_Validate_PartID_New** (New Helper Procedure):
```sql
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_PartID_New(
    IN p_PartID VARCHAR(50),
    OUT p_IsValid BOOLEAN,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_count INT DEFAULT 0;
    DECLARE v_description VARCHAR(200);
    DECLARE v_isActive BOOLEAN DEFAULT FALSE;
    
    SET p_IsValid = FALSE;
    SET p_ErrorMsg = '';
    
    -- Check if PartID is provided
    IF p_PartID IS NULL OR TRIM(p_PartID) = '' THEN
        SET p_ErrorMsg = 'PartID cannot be null or empty';
        LEAVE inv_inventory_Validate_PartID_New;
    END IF;
    
    -- Check if PartID exists and get details
    SELECT COUNT(*), MAX(Description), MAX(IsActive)
    INTO v_count, v_description, v_isActive
    FROM md_part_ids 
    WHERE PartID = p_PartID;
    
    IF v_count = 0 THEN
        SET p_ErrorMsg = CONCAT('PartID "', p_PartID, '" does not exist in master data');
        LEAVE inv_inventory_Validate_PartID_New;
    END IF;
    
    IF NOT v_isActive THEN
        SET p_ErrorMsg = CONCAT('PartID "', p_PartID, '" is inactive and cannot be used');
        LEAVE inv_inventory_Validate_PartID_New;
    END IF;
    
    -- All validation passed
    SET p_IsValid = TRUE;
    SET p_ErrorMsg = CONCAT('Valid PartID: ', v_description);
    
END;;

DELIMITER ;
```

2. **inv_inventory_Validate_Location_New** (New Helper Procedure):
```sql
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_Location_New(
    IN p_Location VARCHAR(50),
    OUT p_IsValid BOOLEAN,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_count INT DEFAULT 0;
    DECLARE v_description VARCHAR(200);
    DECLARE v_isActive BOOLEAN DEFAULT FALSE;
    DECLARE v_locationType VARCHAR(50);
    
    SET p_IsValid = FALSE;
    SET p_ErrorMsg = '';
    
    IF p_Location IS NULL OR TRIM(p_Location) = '' THEN
        SET p_ErrorMsg = 'Location cannot be null or empty';
        LEAVE inv_inventory_Validate_Location_New;
    END IF;
    
    SELECT COUNT(*), MAX(Description), MAX(IsActive), MAX(LocationType)
    INTO v_count, v_description, v_isActive, v_locationType
    FROM md_locations 
    WHERE LocationID = p_Location;
    
    IF v_count = 0 THEN
        SET p_ErrorMsg = CONCAT('Location "', p_Location, '" does not exist in master data');
        LEAVE inv_inventory_Validate_Location_New;
    END IF;
    
    IF NOT v_isActive THEN
        SET p_ErrorMsg = CONCAT('Location "', p_Location, '" is inactive and cannot be used');
        LEAVE inv_inventory_Validate_Location_New;
    END IF;
    
    -- Additional location-specific validation
    IF v_locationType = 'RESTRICTED' THEN
        SET p_ErrorMsg = CONCAT('Location "', p_Location, '" is restricted and requires special authorization');
        LEAVE inv_inventory_Validate_Location_New;
    END IF;
    
    SET p_IsValid = TRUE;
    SET p_ErrorMsg = CONCAT('Valid Location: ', v_description, ' (', v_locationType, ')');
    
END;;

DELIMITER ;
```

3. **inv_inventory_Validate_Operation_New** (New Helper Procedure):
```sql
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_Operation_New(
    IN p_Operation VARCHAR(10),
    OUT p_IsValid BOOLEAN,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_count INT DEFAULT 0;
    DECLARE v_description VARCHAR(200);
    DECLARE v_isActive BOOLEAN DEFAULT FALSE;
    DECLARE v_workCenter VARCHAR(50);
    
    SET p_IsValid = FALSE;
    SET p_ErrorMsg = '';
    
    IF p_Operation IS NULL OR TRIM(p_Operation) = '' THEN
        SET p_ErrorMsg = 'Operation cannot be null or empty';
        LEAVE inv_inventory_Validate_Operation_New;
    END IF;
    
    -- Validate operation format (should be numeric string)
    IF p_Operation NOT REGEXP '^[0-9]+$' THEN
        SET p_ErrorMsg = CONCAT('Operation "', p_Operation, '" must be a numeric string (e.g., "90", "100", "110")');
        LEAVE inv_inventory_Validate_Operation_New;
    END IF;
    
    SELECT COUNT(*), MAX(Description), MAX(IsActive), MAX(WorkCenter)
    INTO v_count, v_description, v_isActive, v_workCenter
    FROM md_operations 
    WHERE OperationID = p_Operation;
    
    IF v_count = 0 THEN
        SET p_ErrorMsg = CONCAT('Operation "', p_Operation, '" does not exist in master data');
        LEAVE inv_inventory_Validate_Operation_New;
    END IF;
    
    IF NOT v_isActive THEN
        SET p_ErrorMsg = CONCAT('Operation "', p_Operation, '" is inactive and cannot be used');
        LEAVE inv_inventory_Validate_Operation_New;
    END IF;
    
    SET p_IsValid = TRUE;
    SET p_ErrorMsg = CONCAT('Valid Operation: ', v_description, ' (Work Center: ', v_workCenter, ')');
    
END;;

DELIMITER ;
```

4. **inv_inventory_Validate_User_New** (New Helper Procedure):
```sql
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_User_New(
    IN p_User VARCHAR(50),
    OUT p_IsValid BOOLEAN,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_count INT DEFAULT 0;
    DECLARE v_fullName VARCHAR(200);
    DECLARE v_isActive BOOLEAN DEFAULT FALSE;
    DECLARE v_department VARCHAR(50);
    
    SET p_IsValid = FALSE;
    SET p_ErrorMsg = '';
    
    IF p_User IS NULL OR TRIM(p_User) = '' THEN
        SET p_ErrorMsg = 'User cannot be null or empty';
        LEAVE inv_inventory_Validate_User_New;
    END IF;
    
    SELECT COUNT(*), MAX(CONCAT(FirstName, ' ', LastName)), MAX(IsActive), MAX(Department)
    INTO v_count, v_fullName, v_isActive, v_department
    FROM md_users 
    WHERE UserID = p_User;
    
    IF v_count = 0 THEN
        SET p_ErrorMsg = CONCAT('User "', p_User, '" does not exist in user directory');
        LEAVE inv_inventory_Validate_User_New;
    END IF;
    
    IF NOT v_isActive THEN
        SET p_ErrorMsg = CONCAT('User "', p_User, '" is inactive and cannot perform operations');
        LEAVE inv_inventory_Validate_User_New;
    END IF;
    
    SET p_IsValid = TRUE;
    SET p_ErrorMsg = CONCAT('Valid User: ', v_fullName, ' (', v_department, ')');
    
END;;

DELIMITER ;
```

ENHANCED INVENTORY PROCEDURES WITH VALIDATION:

5. **inv_inventory_Add_Item_Enhanced** (Updated with Full Validation):
```sql
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Add_Item_Enhanced(
    IN p_PartID VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_Quantity INT,
    IN p_Location VARCHAR(50),
    IN p_User VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_isValid BOOLEAN DEFAULT FALSE;
    DECLARE v_validationMsg VARCHAR(255);
    DECLARE v_currentQty INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in inv_inventory_Add_Item_Enhanced: ', p_ErrorMsg);
    END;

    -- Validate PartID
    CALL inv_inventory_Validate_PartID_New(p_PartID, v_isValid, v_validationMsg);
    IF NOT v_isValid THEN
        SET p_Status = 1;
        SET p_ErrorMsg = v_validationMsg;
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    -- Validate Location
    CALL inv_inventory_Validate_Location_New(p_Location, v_isValid, v_validationMsg);
    IF NOT v_isValid THEN
        SET p_Status = 1;
        SET p_ErrorMsg = v_validationMsg;
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    -- Validate Operation
    CALL inv_inventory_Validate_Operation_New(p_Operation, v_isValid, v_validationMsg);
    IF NOT v_isValid THEN
        SET p_Status = 1;
        SET p_ErrorMsg = v_validationMsg;
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    -- Validate User
    CALL inv_inventory_Validate_User_New(p_User, v_isValid, v_validationMsg);
    IF NOT v_isValid THEN
        SET p_Status = 1;
        SET p_ErrorMsg = v_validationMsg;
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    -- Validate Quantity
    IF p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Quantity must be positive, received: ', p_Quantity);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    -- Business rule: Check for reasonable quantity limits
    IF p_Quantity > 10000 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Quantity ', p_Quantity, ' exceeds maximum allowed limit of 10,000 units');
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    START TRANSACTION;

    -- Get current quantity
    SELECT COALESCE(SUM(Quantity), 0) INTO v_currentQty
    FROM inventory_table 
    WHERE PartID = p_PartID AND Location = p_Location AND Operation = p_Operation;

    -- Insert or update inventory record
    INSERT INTO inventory_table (PartID, Operation, Quantity, Location, LastModified, LastModifiedBy)
    VALUES (p_PartID, p_Operation, p_Quantity, p_Location, NOW(), p_User)
    ON DUPLICATE KEY UPDATE
        Quantity = Quantity + p_Quantity,
        LastModified = NOW(),
        LastModifiedBy = p_User;

    -- Log transaction
    INSERT INTO transaction_log (PartID, Operation, TransactionType, Quantity, Location, UserID, Timestamp)
    VALUES (p_PartID, p_Operation, 'IN', p_Quantity, p_Location, p_User, NOW());

    COMMIT;
    
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully added ', p_Quantity, ' units. New total: ', v_currentQty + p_Quantity);
    
END;;

DELIMITER ;
```

BUSINESS RULE VALIDATION EXAMPLES:

6. **inv_inventory_Validate_StockRemoval_New** (Business Logic Validation):
```sql
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_StockRemoval_New(
    IN p_PartID VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_Location VARCHAR(50),
    IN p_RequestedQty INT,
    OUT p_CanRemove BOOLEAN,
    OUT p_AvailableQty INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_currentStock INT DEFAULT 0;
    DECLARE v_reservedStock INT DEFAULT 0;
    DECLARE v_minimumLevel INT DEFAULT 0;
    
    SET p_CanRemove = FALSE;
    SET p_AvailableQty = 0;
    SET p_ErrorMsg = '';
    
    -- Get current stock level
    SELECT COALESCE(SUM(Quantity), 0) INTO v_currentStock
    FROM inventory_table 
    WHERE PartID = p_PartID AND Location = p_Location AND Operation = p_Operation;
    
    -- Get reserved stock (if applicable)
    SELECT COALESCE(SUM(ReservedQuantity), 0) INTO v_reservedStock
    FROM inventory_reservations 
    WHERE PartID = p_PartID AND Location = p_Location AND Operation = p_Operation
    AND ExpirationDate > NOW();
    
    -- Get minimum stock level
    SELECT COALESCE(MinimumStockLevel, 0) INTO v_minimumLevel
    FROM md_part_ids 
    WHERE PartID = p_PartID;
    
    -- Calculate available quantity
    SET p_AvailableQty = v_currentStock - v_reservedStock;
    
    -- Validate removal request
    IF p_RequestedQty > p_AvailableQty THEN
        SET p_ErrorMsg = CONCAT('Insufficient stock. Requested: ', p_RequestedQty, 
                               ', Available: ', p_AvailableQty, 
                               ' (Current: ', v_currentStock, ', Reserved: ', v_reservedStock, ')');
        LEAVE inv_inventory_Validate_StockRemoval_New;
    END IF;
    
    -- Check minimum stock level warning
    IF (p_AvailableQty - p_RequestedQty) < v_minimumLevel THEN
        SET p_ErrorMsg = CONCAT('Warning: Removal will reduce stock below minimum level. ',
                               'Remaining: ', (p_AvailableQty - p_RequestedQty), 
                               ', Minimum: ', v_minimumLevel);
        -- Still allow removal but with warning
    END IF;
    
    SET p_CanRemove = TRUE;
    
END;;

DELIMITER ;
```

VALIDATION ERROR REPORTING:

7. **sys_validation_Log_Error_New** (Error Logging Procedure):
```sql
DELIMITER ;;

CREATE PROCEDURE sys_validation_Log_Error_New(
    IN p_ProcedureName VARCHAR(100),
    IN p_ErrorType VARCHAR(50),
    IN p_ErrorMessage VARCHAR(500),
    IN p_InputParameters JSON,
    IN p_UserID VARCHAR(50)
)
BEGIN
    INSERT INTO validation_error_log (
        ProcedureName,
        ErrorType,
        ErrorMessage,
        InputParameters,
        UserID,
        ErrorTimestamp
    ) VALUES (
        p_ProcedureName,
        p_ErrorType,
        p_ErrorMessage,
        p_InputParameters,
        p_UserID,
        NOW()
    );
END;;

DELIMITER ;
```

TESTING VALIDATION SCENARIOS:

Create test procedures for each validation:
```sql
-- Test with NULL values
CALL inv_inventory_Add_Item_Enhanced(NULL, '90', 10, 'WC001', 'USER001', @status, @msg);
-- Expected: p_Status = 1, p_ErrorMsg = 'PartID is required and cannot be null or empty'

-- Test with invalid PartID
CALL inv_inventory_Add_Item_Enhanced('INVALID_PART', '90', 10, 'WC001', 'USER001', @status, @msg);
-- Expected: p_Status = 1, p_ErrorMsg = 'Invalid PartID: "INVALID_PART" does not exist...'

-- Test with negative quantity
CALL inv_inventory_Add_Item_Enhanced('PART001', '90', -5, 'WC001', 'USER001', @status, @msg);
-- Expected: p_Status = 1, p_ErrorMsg = 'Quantity must be positive, received: -5'
```

After implementing validation, create Development/Database_Files/README_ValidationProcedures.md documenting:
- Validation patterns and standards
- Helper procedure usage
- Business rule validation examples
- Error message formatting guidelines
- Testing procedures for validation scenarios
```

---

## Expected Deliverables

1. **Validation helper procedures** for PartID, Location, Operation, User
2. **Enhanced inventory procedures** with comprehensive validation
3. **Business rule validation** for stock levels and constraints
4. **Error logging procedures** for validation failures
5. **Test scenarios** for all validation cases
6. **Standardized error messages** with specific, actionable information
7. **Documentation** of validation patterns and business rules

---

## Validation Steps

1. Test each validation helper procedure with valid and invalid inputs
2. Verify all enhanced procedures reject invalid data appropriately
3. Confirm error messages are specific and helpful
4. Test business rule validation (stock levels, constraints)
5. Validate error logging captures necessary information
6. Test performance impact of validation procedures
7. Verify validation doesn't break existing functionality

---

## Success Criteria

- [ ] All required parameters validated for null/empty values
- [ ] Foreign key relationships validated against master data
- [ ] Business rules enforced (positive quantities, stock levels)
- [ ] Specific, actionable error messages returned
- [ ] Helper procedures created for reusable validation
- [ ] Enhanced procedures include comprehensive validation
- [ ] Error logging captures validation failures
- [ ] Test scenarios verify all validation works correctly
- [ ] Performance impact is minimal
- [ ] Documentation explains validation patterns and rules

---

*Priority: HIGH - Essential for preventing invalid data and maintaining data integrity.*