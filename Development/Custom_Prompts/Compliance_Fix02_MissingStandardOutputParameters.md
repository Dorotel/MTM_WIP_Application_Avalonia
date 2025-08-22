# Custom Prompt: Fix Missing Standard Output Parameters

## 🚨 CRITICAL PRIORITY FIX #2

**Issue**: Most existing stored procedures lack standardized `p_Status INT` and `p_ErrorMsg VARCHAR(255)` output parameters.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- `inv_inventory_Add_Item` - No status/error outputs
- `inv_inventory_Get_ByPartID` - No status/error outputs
- All `md_*_Get_All` procedures - No status/error outputs
- Many other procedures missing standardized outputs

**Priority**: 🚨 **CRITICAL - AFFECTS ALL DATABASE OPERATIONS**

---

## Custom Prompt

```
CRITICAL DATABASE STANDARDIZATION: Add standard output parameters to all existing stored procedures that lack them.

REQUIREMENTS:
1. Copy ALL existing procedures without standard outputs to Development/Database_Files/Updated_Stored_Procedures.sql
2. Add standard output parameters: OUT p_Status INT, OUT p_ErrorMsg VARCHAR(255) to each procedure
3. Implement status codes: 0=success, 1=warning, -1=error
4. Add meaningful error messages for all failure scenarios
5. Maintain exact same functionality while adding standardized error handling
6. Update service layer integration points to handle new output parameters

PROCEDURES TO UPDATE (from Database_Files/Existing_Stored_Procedures.sql):

CRITICAL PROCEDURES:
1. inv_inventory_Add_Item - Add outputs and error handling
2. inv_inventory_Get_ByPartID - Add outputs and validation
3. inv_inventory_Get_ByLocation - Add outputs and validation
4. inv_inventory_Get_All - Add outputs and error handling

METADATA PROCEDURES:
5. md_part_ids_Get_All - Add outputs and validation
6. md_locations_Get_All - Add outputs and validation  
7. md_operations_Get_All - Add outputs and validation
8. md_users_Get_All - Add outputs and validation

SYSTEM PROCEDURES:
9. sys_last_10_transactions_Get_ByUser - Add outputs and validation
10. Any other procedures missing standard outputs

STANDARDIZATION PATTERN:
```sql
-- BEFORE (existing procedure):
CREATE PROCEDURE inv_inventory_Add_Item(
    IN p_PartID VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_Quantity INT,
    IN p_Location VARCHAR(50),
    IN p_User VARCHAR(50)
)
BEGIN
    -- Original logic without error handling
END;;

-- AFTER (standardized procedure):
CREATE PROCEDURE inv_inventory_Add_Item(
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

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Add_Item;
    END IF;

    IF p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be positive';
        LEAVE inv_inventory_Add_Item;
    END IF;

    -- Validate PartID exists
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('PartID "', p_PartID, '" does not exist in master data');
        LEAVE inv_inventory_Add_Item;
    END IF;

    START TRANSACTION;
    
    -- Original business logic here
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = 'Item added successfully';
END;;
```

STATUS CODE STANDARDS:
- p_Status = 0: Success - Operation completed successfully
- p_Status = 1: Warning - Operation completed with warnings (e.g., partial success)
- p_Status = -1: Error - Operation failed, transaction rolled back

ERROR MESSAGE STANDARDS:
- Clear, specific error descriptions
- Include parameter names and values when helpful
- Use consistent language and format
- Provide actionable information for debugging

VALIDATION RULES TO ADD:
1. **Parameter Validation**:
   - Check for NULL or empty required parameters
   - Validate data types and ranges
   - Ensure positive quantities for inventory operations

2. **Business Rule Validation**:
   - PartID must exist in md_part_ids
   - Location must exist in valid locations
   - Operation must be valid workflow step
   - User must exist in user system

3. **Data Integrity Validation**:
   - Foreign key relationships
   - Constraint checking
   - Duplicate prevention where appropriate

SPECIFIC UPDATES REQUIRED:

FOR INVENTORY PROCEDURES:
- Add transaction management (START TRANSACTION/COMMIT/ROLLBACK)
- Validate all foreign key relationships
- Check for sufficient stock before removals
- Log all changes appropriately

FOR METADATA PROCEDURES:
- Add validation for data retrieval
- Handle empty result sets gracefully
- Provide meaningful messages for no data found

FOR SYSTEM PROCEDURES:
- Validate user permissions
- Handle date range validations
- Ensure proper data filtering

MIGRATION STRATEGY:
1. Copy existing procedure to Updated_Stored_Procedures.sql
2. Add output parameters to signature
3. Add input validation at beginning
4. Wrap data operations in transactions
5. Add error handler with diagnostics
6. Set success status and message at end
7. Test thoroughly before deployment

TESTING REQUIREMENTS:
- Test with valid inputs (should return p_Status = 0)
- Test with invalid inputs (should return p_Status = 1 with message)
- Test with database errors (should return p_Status = -1 with message)
- Verify rollback works on transaction failures
- Confirm original functionality preserved

After updating procedures, create Development/Database_Files/README_UpdatedProcedures.md documenting:
- Which procedures were updated
- What changes were made
- New output parameter usage
- Testing results
- Migration notes for production deployment
```

---

## Expected Deliverables

1. **Development/Database_Files/Updated_Stored_Procedures.sql** - All updated procedures with standard outputs
2. **Development/Database_Files/README_UpdatedProcedures.md** - Documentation of changes
3. **Standard error handling** in all updated procedures
4. **Comprehensive input validation** for all parameters
5. **Consistent status codes** across all procedures
6. **Detailed error messages** for all failure scenarios

---

## Validation Steps

1. Verify all updated procedures compile without syntax errors
2. Test each procedure with valid inputs (status = 0)
3. Test each procedure with invalid inputs (status = 1)
4. Test error handling scenarios (status = -1)
5. Confirm original functionality is preserved
6. Validate transaction rollback works properly

---

## Service Layer Integration

Once procedures are updated, service layer can be implemented to:
- Handle standard output parameters consistently
- Process status codes appropriately
- Convert error messages to user-friendly format
- Implement proper exception handling patterns

---

## Success Criteria

- [ ] All procedures have standard output parameters
- [ ] Consistent status code implementation (0, 1, -1)
- [ ] Comprehensive error messages for all failure scenarios
- [ ] Input validation implemented for all parameters
- [ ] Transaction management added where needed
- [ ] Original functionality preserved and tested
- [ ] Documentation updated with changes
- [ ] Ready for service layer integration

---

*Priority: CRITICAL - Required for consistent error handling across entire application.*