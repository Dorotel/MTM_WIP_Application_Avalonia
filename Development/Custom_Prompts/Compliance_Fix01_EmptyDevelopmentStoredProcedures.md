# Custom Prompt: Fix Empty Development Stored Procedures

## 🚨 CRITICAL PRIORITY FIX #1 - COMPLETE

**Issue**: The development stored procedure files are completely empty, preventing any new database functionality development.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- `Development/Database_Files/New_Stored_Procedures.sql` - EMPTY
- `Development/Database_Files/Updated_Stored_Procedures.sql` - EMPTY

**Priority**: 🚨 **CRITICAL - BLOCKS DEVELOPMENT**

---

## Custom Prompt

```
CRITICAL DATABASE FIX: Create standardized inventory management stored procedures in the empty development files.

REQUIREMENTS:
1. Create comprehensive stored procedures in Development/Database_Files/New_Stored_Procedures.sql
2. Implement ALL standard MTM inventory operations with proper error handling
3. Add standard output parameters: OUT p_Status INT, OUT p_ErrorMsg VARCHAR(255)
4. Include comprehensive error handling with EXIT HANDLER FOR SQLEXCEPTION
5. Add input validation and business rule checking
6. Implement transaction management for data consistency
7. Follow the exact format from existing procedures in Database_Files/Existing_Stored_Procedures.sql

SPECIFIC PROCEDURES TO CREATE:
1. inv_inventory_Add_Item_Enhanced - Enhanced version with full error handling
2. inv_inventory_Remove_Item_Enhanced - Enhanced version with validation
3. inv_inventory_Transfer_Item_New - Transfer between locations
4. inv_inventory_Get_ByLocation_New - Get all items by location
5. inv_inventory_Get_ByOperation_New - Get all items by operation
6. inv_inventory_Validate_Stock_New - Validate sufficient stock before removal
7. inv_transaction_Log_New - Log all inventory transactions
8. inv_location_Validate_New - Validate location exists
9. inv_operation_Validate_New - Validate operation number
10. sys_user_Validate_New - Validate user exists

ERROR HANDLING PATTERN TO IMPLEMENT:
```sql
DELIMITER ;;

CREATE PROCEDURE procedure_name(
    IN p_Parameter1 VARCHAR(50),
    IN p_Parameter2 INT,
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
    IF p_Parameter1 IS NULL OR p_Parameter1 = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Parameter1 is required';
        LEAVE procedure_name;
    END IF;

    START TRANSACTION;
    
    -- Business logic here
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = 'Success';
END;;

DELIMITER ;
```

VALIDATION RULES TO IMPLEMENT:
- PartID must exist in md_part_ids table
- Location must be valid manufacturing location
- Operation must be valid workflow step number
- User must exist in user table
- Quantity must be positive integer
- For removals, sufficient stock must exist

TRANSACTION PATTERNS:
- Wrap all data modifications in START TRANSACTION/COMMIT
- Use ROLLBACK in error handlers
- Implement proper transaction boundaries
- Test rollback scenarios

BUSINESS RULES:
- TransactionType determined by user intent, NOT operation number
- Operations are workflow step identifiers ("90", "100", "110")
- All inventory changes must be logged in transaction history
- Negative quantities not allowed
- Location transfers require validation of both locations

OUTPUT REQUIREMENTS:
- p_Status = 0 for success
- p_Status = 1 for warnings (like insufficient stock)
- p_Status = -1 for errors
- p_ErrorMsg contains specific error description for troubleshooting

DOCUMENTATION:
- Include header comment for each procedure explaining purpose
- Document all parameters and their validation rules
- Include example usage in comments
- Reference related procedures and tables

After creating procedures, update Development/Database_Files/README_NewProcedures.md with comprehensive documentation of all new procedures, their purposes, parameters, and usage examples.

Follow MTM naming conventions: {module}_{table}_{action}_{details}
Example: inv_inventory_Add_Item_Enhanced
```

---

## Expected Deliverables

1. **Development/Database_Files/New_Stored_Procedures.sql** - Complete with 10+ standardized procedures
2. **Development/Database_Files/README_NewProcedures.md** - Documentation for all new procedures
3. **Comprehensive error handling** in all procedures
4. **Input validation** for all parameters
5. **Transaction management** with proper rollback
6. **Standard output parameters** for consistent error reporting

---

## Validation Steps

1. Verify all procedures compile without syntax errors
2. Test error handling scenarios (invalid inputs, constraint violations)
3. Confirm transaction rollback works properly
4. Validate all business rules are enforced
5. Check that output parameters return correct status codes

---

## Related Fixes

This fix enables:
- **Fix #2**: Missing Standard Output Parameters (provides template)
- **Fix #3**: Inadequate Error Handling (implements comprehensive pattern)
- **Fix #6**: Missing Service Layer Database Integration (provides procedures to call)

---

## Success Criteria

- [ ] New_Stored_Procedures.sql contains 10+ comprehensive procedures
- [ ] All procedures have standard error handling pattern
- [ ] Input validation implemented for all parameters
- [ ] Transaction management implemented properly
- [ ] Documentation created for all new procedures
- [ ] No syntax errors in any procedure
- [ ] Error scenarios tested and working
- [ ] Ready for service layer integration

---

*Priority: CRITICAL - Must be completed before any other database or service layer work can proceed.*