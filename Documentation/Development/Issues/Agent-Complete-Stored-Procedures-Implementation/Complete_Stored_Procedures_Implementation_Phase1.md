# Complete_Stored_Procedures_Implementation_Phase1 - Agent Instructions

## 🎯 **AGENT EXECUTION CONTEXT**
**Issue Type:** Phase
**Complexity:** Complex
**Estimated Time:** 4hr
**Dependencies:** Current Database.cs service analysis, MySQL 5.7 compatibility requirements

## 📋 **PRECISE OBJECTIVES**
### Primary Goal
Analyze the entire MTM WIP Application codebase to identify all required stored procedures, then generate a comprehensive Updated_Stored_Procedures.sql file with MySQL 5.7 compatible procedures including proper DROP statements.

### Acceptance Criteria
- [x] Complete analysis of all database operations in the application
- [x] Generated Updated_Stored_Procedures.sql with all required procedures
- [x] All procedures include proper DROP statements at the beginning
- [x] MySQL 5.7 compatibility verified for all procedures
- [x] No SQL comments included in the generated file
- [x] Proper MTM status/error message output parameters implemented

## 🔧 **IMPLEMENTATION DETAILS**

### Files to Modify/Create
```
Documentation/Development/Database_Files/Updated_Stored_Procedures.sql - Complete procedure definitions
Services/Database.cs - Reference for required procedures
Services/QuickButtons.cs - QuickButton procedure requirements
ViewModels/MainForm/*.cs - ViewModel procedure usage analysis
Models/Shared/CoreModels.cs - Data model requirements
```

### Code Patterns Required
```sql
-- MTM Standard Stored Procedure Pattern
DROP PROCEDURE IF EXISTS `procedure_name`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `procedure_name`(
    IN p_Parameter1 VARCHAR(100),
    IN p_Parameter2 INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;
    
    START TRANSACTION;
    -- Implementation logic
    SET p_Status = 0;
    SET p_ErrorMsg = 'Success message';
    COMMIT;
END$$
DELIMITER ;
```

### Database Operations (If Applicable)
```sql
-- Required procedure categories to implement:
-- Inventory Management: inv_inventory_*
-- Master Data: md_part_ids_*, md_locations_*, md_operations_*, md_item_types_*
-- User Management: usr_users_*, usr_ui_settings_*
-- Quick Buttons: qb_quickbuttons_*, sys_last_10_transactions_*
-- System Functions: sys_roles_*, sys_user_roles_*, log_*
```

## ⚡ **EXECUTION SEQUENCE**
1. **Step 1:** ✅ Analyze Database.cs service to identify all stored procedure calls
2. **Step 2:** ✅ Analyze QuickButtons.cs and other services for additional procedure requirements
3. **Step 3:** ✅ Analyze all ViewModels to identify missing database operations
4. **Step 4:** ✅ Clear existing Updated_Stored_Procedures.sql content (preserve work already done)
5. **Step 5:** ✅ Generate comprehensive DROP statements for all procedures
6. **Step 6:** ✅ Generate all required stored procedures with MySQL 5.7 compatibility
7. **Step 7:** ✅ Validate each procedure follows MTM patterns with proper error handling

## 🧪 **VALIDATION REQUIREMENTS**
### Automated Tests
- [x] MySQL 5.7 syntax validation for all procedures
- [x] Parameter count and type validation for each procedure

### Manual Verification
- [x] All Database.cs method calls have corresponding stored procedures
- [x] All procedures include proper DROP statements
- [x] No SQL comments present in the generated file
- [x] MTM status/error pattern implemented consistently

## 🔗 **CONTEXT REFERENCES**
### Related Files
- [database-patterns.instruction.md](.github/Development-Instructions/database-patterns.instruction.md) - MTM database patterns
- [Services/Database.cs](../../../../Services/Database.cs) - Current database service implementation
- [Services/QuickButtons.cs](../../../../Services/QuickButtons.cs) - QuickButton requirements

### MTM-Specific Requirements
- **Transaction Type Logic:** TransactionType determined by user intent, not operation number
- **Database Pattern:** All operations must use stored procedures only
- **UI Pattern:** Support for Avalonia + Standard .NET MVVM requirements

## 🚨 **ERROR HANDLING**
### Expected Issues
- MySQL 5.7 compatibility issues with JSON functions - Use TEXT with JSON validation
- Procedure parameter mismatches - Verify all calls in Database.cs service
- Missing procedures for ViewModel operations - Complete codebase analysis required

### Rollback Plan
- Backup current Updated_Stored_Procedures.sql before clearing
- Maintain existing procedure functionality during updates
- Preserve all working procedures from current implementation

---
*Agent-Optimized Instructions for GitHub Copilot*
