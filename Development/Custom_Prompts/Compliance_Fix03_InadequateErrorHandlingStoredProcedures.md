# Custom Prompt: Fix Inadequate Error Handling in Stored Procedures

## 🚨 CRITICAL PRIORITY FIX #3

**Issue**: Most stored procedures lack comprehensive SQL exception handlers and proper error management.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes
**Files Affected**:
- `inv_inventory_Add_Item` - No error handling at all
- Most `md_*` procedures - No validation or error handling
- Transaction procedures missing rollback on errors

**Priority**: 🚨 **CRITICAL - DATA INTEGRITY RISK**

---

## Custom Prompt

```
CRITICAL DATABASE SECURITY: Implement comprehensive error handling in all stored procedures to prevent data corruption and ensure transaction integrity.

REQUIREMENTS:
1. Add EXIT HANDLER FOR SQLEXCEPTION to ALL procedures in Development/Database_Files/
2. Implement automatic ROLLBACK on database errors
3. Capture and log specific error details (MYSQL_ERRNO, MESSAGE_TEXT)
4. Return meaningful error messages to application layer
5. Use consistent error handling template across all procedures
6. Test all error scenarios thoroughly

COMPREHENSIVE ERROR HANDLING PATTERN:
```sql
DELIMITER ;;

CREATE PROCEDURE procedure_name(
    IN p_Parameter1 VARCHAR(50),
    IN p_Parameter2 INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Declare variables for error handling
    DECLARE v_error_count INT DEFAULT 0;
    DECLARE v_errno INT;
    DECLARE v_msg TEXT;
    
    -- Comprehensive error handler
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        -- Capture error details
        GET DIAGNOSTICS CONDITION 1
            v_errno = MYSQL_ERRNO,
            v_msg = MESSAGE_TEXT;
        
        -- Rollback any pending transaction
        ROLLBACK;
        
        -- Set error status and detailed message
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database Error ', v_errno, ': ', v_msg);
        
        -- Log error for debugging (optional)
        INSERT INTO error_log (procedure_name, error_code, error_message, error_timestamp)
        VALUES ('procedure_name', v_errno, v_msg, NOW());
    END;
    
    -- Additional handlers for specific conditions
    DECLARE CONTINUE HANDLER FOR NOT FOUND
    BEGIN
        SET v_error_count = v_error_count + 1;
    END;
    
    DECLARE CONTINUE HANDLER FOR SQLWARNING
    BEGIN
        SET v_error_count = v_error_count + 1;
    END;

    -- Input validation with detailed messages
    IF p_Parameter1 IS NULL OR p_Parameter1 = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Parameter1 is required and cannot be null or empty';
        LEAVE procedure_name;
    END IF;

    -- Business rule validation
    IF p_Parameter2 <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Parameter2 must be a positive integer';
        LEAVE procedure_name;
    END IF;

    -- Start transaction for data modifications
    START TRANSACTION;
    
    -- Business logic with error checking
    -- Example: Check if record exists before updating
    SELECT COUNT(*) INTO v_error_count 
    FROM target_table 
    WHERE key_field = p_Parameter1;
    
    IF v_error_count = 0 THEN
        ROLLBACK;
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Record not found for key: ', p_Parameter1);
        LEAVE procedure_name;
    END IF;
    
    -- Perform data modification
    UPDATE target_table 
    SET field1 = p_Parameter2
    WHERE key_field = p_Parameter1;
    
    -- Check if update affected expected number of rows
    IF ROW_COUNT() = 0 THEN
        ROLLBACK;
        SET p_Status = 1;
        SET p_ErrorMsg = 'No rows were updated - operation may have failed';
        LEAVE procedure_name;
    END IF;
    
    -- Commit successful transaction
    COMMIT;
    
    -- Set success status
    SET p_Status = 0;
    SET p_ErrorMsg = 'Operation completed successfully';
    
END;;

DELIMITER ;
```

SPECIFIC ERROR HANDLING TO IMPLEMENT:

1. **INVENTORY PROCEDURES** (inv_inventory_*):
   ```sql
   -- Add to inv_inventory_Add_Item
   DECLARE EXIT HANDLER FOR SQLEXCEPTION
   BEGIN
       ROLLBACK;
       GET DIAGNOSTICS CONDITION 1
           p_Status = MYSQL_ERRNO,
           p_ErrorMsg = MESSAGE_TEXT;
       SET p_Status = -1;
       SET p_ErrorMsg = CONCAT('Failed to add inventory item: ', p_ErrorMsg);
   END;
   
   -- Validate PartID exists before adding
   IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
       SET p_Status = 1;
       SET p_ErrorMsg = CONCAT('Invalid PartID: ', p_PartID, ' not found in master data');
       LEAVE inv_inventory_Add_Item;
   END IF;
   ```

2. **METADATA PROCEDURES** (md_*):
   ```sql
   -- Add to md_part_ids_Get_All and similar procedures
   DECLARE EXIT HANDLER FOR SQLEXCEPTION
   BEGIN
       GET DIAGNOSTICS CONDITION 1
           p_Status = MYSQL_ERRNO,
           p_ErrorMsg = MESSAGE_TEXT;
       SET p_Status = -1;
       SET p_ErrorMsg = CONCAT('Failed to retrieve master data: ', p_ErrorMsg);
   END;
   
   -- Check for empty results
   SELECT COUNT(*) INTO @record_count FROM md_part_ids;
   IF @record_count = 0 THEN
       SET p_Status = 1;
       SET p_ErrorMsg = 'No part IDs found in master data';
       LEAVE md_part_ids_Get_All;
   END IF;
   ```

3. **TRANSACTION PROCEDURES**:
   ```sql
   -- Ensure all multi-step operations are properly wrapped
   START TRANSACTION;
   
   -- Step 1: Insert transaction log
   INSERT INTO transaction_log (...);
   IF ROW_COUNT() = 0 THEN
       ROLLBACK;
       SET p_Status = -1;
       SET p_ErrorMsg = 'Failed to log transaction';
       LEAVE procedure_name;
   END IF;
   
   -- Step 2: Update inventory
   UPDATE inventory_table SET ...;
   IF ROW_COUNT() = 0 THEN
       ROLLBACK;
       SET p_Status = -1;
       SET p_ErrorMsg = 'Failed to update inventory';
       LEAVE procedure_name;
   END IF;
   
   COMMIT;
   ```

ERROR SCENARIOS TO HANDLE:

1. **Connection Errors**:
   - Database connection lost
   - Timeout errors
   - Lock wait timeouts

2. **Constraint Violations**:
   - Foreign key constraint failures
   - Unique key violations
   - Check constraint failures

3. **Data Type Errors**:
   - Invalid data type conversions
   - Out of range values
   - Character set issues

4. **Business Logic Errors**:
   - Insufficient inventory for removal
   - Duplicate operations
   - Invalid state transitions

5. **System Errors**:
   - Disk space issues
   - Memory errors
   - Permission denied

LOGGING STRATEGY:
Create error_log table if not exists:
```sql
CREATE TABLE IF NOT EXISTS error_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    procedure_name VARCHAR(100),
    error_code INT,
    error_message TEXT,
    input_parameters JSON,
    error_timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    user_context VARCHAR(50)
);
```

TESTING MATRIX:
1. **Valid Input Tests**: Verify normal operation with p_Status = 0
2. **Invalid Input Tests**: Test validation with p_Status = 1
3. **Constraint Violation Tests**: Force FK violations, expect p_Status = -1
4. **Connection Tests**: Simulate connection loss during operation
5. **Transaction Tests**: Verify rollback works on mid-operation failures
6. **Load Tests**: Test error handling under high concurrency

ERROR MESSAGE STANDARDS:
- Include procedure name in error context
- Provide specific parameter values when helpful
- Use consistent error message format
- Include suggested resolution when possible
- Maintain security by not exposing sensitive data

IMPLEMENTATION ORDER:
1. Start with critical inventory procedures (highest data risk)
2. Update metadata procedures (used by many operations)
3. Enhance system/utility procedures
4. Add comprehensive logging to all procedures
5. Test error scenarios thoroughly

After implementing error handling, create Development/Database_Files/README_ErrorHandling.md documenting:
- Error handling patterns used
- Status code meanings (-1, 0, 1)
- Error message formats
- Testing procedures and results
- Recovery procedures for common errors
```

---

## Expected Deliverables

1. **Comprehensive error handlers** in all stored procedures
2. **Automatic rollback** on database errors
3. **Detailed error logging** with specific error information
4. **Consistent error message formats** across all procedures
5. **Robust transaction management** preventing data corruption
6. **Error handling documentation** and testing procedures

---

## Validation Steps

1. Test each procedure with various error conditions
2. Verify rollback functionality works correctly
3. Confirm error messages are helpful and specific
4. Test transaction integrity under failure conditions
5. Validate error logging captures necessary information
6. Ensure application can handle returned error statuses

---

## Critical Error Scenarios to Test

1. **Foreign Key Violations**: Try to insert invalid PartID
2. **Connection Loss**: Simulate database disconnection during transaction
3. **Lock Timeouts**: Test concurrent access to same records
4. **Disk Space**: Test behavior when database runs out of space
5. **Invalid Data Types**: Pass wrong data types to parameters
6. **Constraint Violations**: Violate unique constraints and check constraints

---

## Success Criteria

- [ ] All procedures have comprehensive error handling
- [ ] Automatic rollback implemented on all errors
- [ ] Detailed error logging captures all failure scenarios
- [ ] Error messages are specific and actionable
- [ ] Transaction integrity maintained under all conditions
- [ ] Error scenarios tested and verified
- [ ] Documentation updated with error handling procedures
- [ ] Application layer can properly handle error responses

---

*Priority: CRITICAL - Essential for preventing data corruption and ensuring system reliability.*