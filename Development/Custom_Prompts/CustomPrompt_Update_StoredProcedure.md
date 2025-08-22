# Update Existing Stored Procedure - Custom Prompt

## Instructions
Use this prompt when you need to modify an existing stored procedure from production.

## Prompt Template

```
Update an existing stored procedure with the following requirements:

**Procedure Name**: [existing_procedure_name] (from Existing_Stored_Procedures.sql)

**Reason for Update**: [Detailed explanation of why this procedure needs to be modified]

**Current Issues**: 
- [Issue 1]
- [Issue 2]
- [Issue 3]

**Required Changes**:
- [Change 1 with specific details]
- [Change 2 with specific details]
- [Change 3 with specific details]

**Backwards Compatibility**: [Must be maintained / Breaking changes acceptable]

**Performance Requirements**: [Any specific performance improvements needed]

**Security Enhancements**: [Any security improvements needed]

**Requirements**:
1. Copy original procedure from Database_Files/Existing_Stored_Procedures.sql
2. Make modifications in Development/Database_Files/Updated_Stored_Procedures.sql
3. Maintain backwards compatibility unless explicitly noted
4. Include comprehensive change documentation
5. Follow MTM database rules - NO direct SQL in application code
6. Test thoroughly with existing application code
7. Create migration plan for production deployment

**Testing Requirements**:
- Regression testing with existing functionality
- Test new features/fixes
- Performance validation
- Integration testing with application code

Please provide:
1. Updated stored procedure SQL code
2. Detailed change documentation
3. Impact analysis on application code
4. Migration/deployment plan
5. Rollback strategy
```

## Usage Examples

### Example 1: Performance Optimization
```
Update an existing stored procedure with the following requirements:

**Procedure Name**: inv_inventory_Get_ByPartID

**Reason for Update**: Current procedure is slow when dealing with large datasets and doesn't utilize indexes effectively

**Current Issues**: 
- Query takes >5 seconds with large inventory tables
- Full table scan instead of using indexes
- Returns unnecessary columns
- No result limiting

**Required Changes**:
- Optimize WHERE clause to use PartID index
- Return only necessary columns instead of SELECT *
- Add LIMIT clause for large result sets
- Add execution time monitoring

**Backwards Compatibility**: Must be maintained - existing application code cannot change

**Performance Requirements**: Query execution time must be under 1 second for normal datasets

**Security Enhancements**: Add parameter validation for PartID input

[Continue with requirements...]
```

### Example 2: Bug Fix
```
Update an existing stored procedure with the following requirements:

**Procedure Name**: inv_inventory_Remove_Item

**Reason for Update**: Procedure has a bug where it doesn't properly validate batch numbers before removal

**Current Issues**: 
- Missing validation for batch number existence
- Allows removal of items with incorrect batch numbers
- No audit trail for failed removal attempts
- Error messages are not descriptive enough

**Required Changes**:
- Add batch number validation against inv_inventory table
- Improve error messages with specific failure reasons
- Add audit logging for all removal attempts
- Add transaction rollback for validation failures

**Backwards Compatibility**: Must be maintained

**Performance Requirements**: No performance degradation

**Security Enhancements**: Enhanced validation to prevent data inconsistencies

[Continue with requirements...]
```

### Example 3: Feature Enhancement
```
Update an existing stored procedure with the following requirements:

**Procedure Name**: usr_users_Add_User

**Reason for Update**: Need to add support for role assignment during user creation

**Current Issues**: 
- Users created without any role assignment
- Requires separate call to assign role
- No default role assignment logic

**Required Changes**:
- Add optional role parameter to procedure
- Add logic to assign default role if none specified
- Validate role exists before assignment
- Create user-role relationship in same transaction

**Backwards Compatibility**: Must be maintained - existing calls without role parameter should work

**Performance Requirements**: No significant performance impact

**Security Enhancements**: Validate role permissions before assignment

[Continue with requirements...]
```

## Update Categories

### Performance Optimization
Common performance improvements:
- Index utilization optimization
- Query plan improvements
- Reduced cursor usage
- Batch processing enhancements
- Memory usage optimization
- Result set limiting

### Bug Fixes
Common bug fix scenarios:
- Logic error corrections
- Data validation fixes
- Transaction management improvements
- Error handling corrections
- Edge case handling

### Security Enhancements
Common security improvements:
- Enhanced parameter validation
- Access control improvements
- SQL injection prevention
- Audit logging enhancements
- Error message sanitization

### Feature Enhancements
Common feature additions:
- Additional parameters
- Enhanced return values
- New functionality
- Business rule updates
- Integration improvements

## Change Documentation Template

```sql
-- ========================================
-- PROCEDURE UPDATE DOCUMENTATION
-- ========================================
-- Procedure Name: [procedure_name]
-- Original Version: Found in Existing_Stored_Procedures.sql
-- Update Date: [YYYY-MM-DD]
-- Updated By: [Developer Name]
-- Version: [New Version Number]
--
-- REASON FOR UPDATE:
-- [Detailed explanation]
--
-- CHANGES MADE:
-- 1. [Specific change 1]
-- 2. [Specific change 2]
-- 3. [Additional changes...]
--
-- IMPACT ANALYSIS:
-- - Application Code Changes Required: [Yes/No - Details]
-- - Backward Compatibility: [Maintained/Breaking Changes]
-- - Performance Impact: [Improved/No Change/Degraded - Details]
--
-- TESTING PERFORMED:
-- - Unit Testing: [Description]
-- - Integration Testing: [Description]
-- - Performance Testing: [Description]
-- - Regression Testing: [Description]
--
-- DEPLOYMENT NOTES:
-- - Prerequisites: [Any requirements]
-- - Dependencies: [Other updates needed]
-- - Rollback Plan: [Steps to revert]
-- - Verification Steps: [How to confirm success]
-- ========================================
```

## Guidelines

### Backwards Compatibility
- Maintain existing parameter names and types
- Don't change existing return data structure unless necessary
- Add new parameters as optional with defaults
- Preserve existing behavior for current usage

### Error Handling Improvements
```sql
-- Enhanced error handling pattern
DECLARE EXIT HANDLER FOR SQLEXCEPTION
BEGIN
    ROLLBACK;
    GET DIAGNOSTICS CONDITION 1
        @sqlstate = RETURNED_SQLSTATE, 
        @errno = MYSQL_ERRNO, 
        @text = MESSAGE_TEXT;
    
    -- Enhanced logging
    INSERT INTO log_error (
        User, Severity, ErrorType, ErrorMessage, 
        ModuleName, MethodName, ErrorTime
    ) VALUES (
        IFNULL(p_User, 'SYSTEM'), 'Error', 'DATABASE', 
        CONCAT('Updated Procedure Error: ', @text),
        'DATABASE', 'procedure_name', NOW()
    );
    
    SET p_Status = -1;
    SET p_ErrorMsg = CONCAT('Database error in ', procedure_name, ': ', @text);
END;
```

### Performance Optimization Patterns
- Convert cursors to set-based operations
- Add appropriate indexes to support queries
- Use LIMIT clauses for large result sets
- Optimize JOIN operations
- Add execution time monitoring

### Testing Requirements
- **Regression Testing**: Ensure existing functionality works
- **Integration Testing**: Test with existing application code
- **Performance Testing**: Validate performance improvements
- **Security Testing**: Verify security enhancements
- **User Acceptance Testing**: Get stakeholder approval

## Application Impact Considerations

### Service Layer Updates
Consider if service layer methods need updates:
- New parameters may require method signature changes
- Error handling may need adjustments
- Return value processing may change
- Timeout settings may need adjustment

### UI Impact Assessment
- Verify UI components still work correctly
- Test error message display
- Validate performance is acceptable
- Check for any new data that needs display

### Integration Points
- External system integrations
- API endpoints
- Report generation
- Batch processing jobs

## Related Files
- Copy from: `Database_Files/Existing_Stored_Procedures.sql` (READ-ONLY)
- Update in: `Development/Database_Files/Updated_Stored_Procedures.sql`
- Document in: `Development/Database_Files/README_Updated_Stored_Procedures.md`
- Service updates: Update appropriate service classes if needed
- Tests: Update existing tests and add new ones