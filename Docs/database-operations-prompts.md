# Database Operations Custom Prompts

## Overview
This document provides specialized custom prompts for database operations within the MTM WIP Application. All prompts enforce the **"No Hard-Coded MySQL Commands"** rule and ensure proper use of stored procedures.

## ?? CRITICAL DATABASE RULES ??

### **STORED PROCEDURES ONLY**
- **ALL** database operations must use stored procedures
- **ZERO** hard-coded SQL commands in application code
- Direct SQL execution will result in security violations and build errors
- This rule applies to ALL database interactions without exception

### **DATABASE FILE ORGANIZATION**
- **Production Files** (`Database_Files/`) - **READ-ONLY** - Current production state
- **Development Files** (`Development/Database_Files/`) - **EDITABLE** - All development work
- **NEVER** edit production files directly
- **ALWAYS** work in development files and deploy through proper change management

---

## Database Operation Prompts

### 1. Create New Stored Procedure
**Persona:** Database Architect Copilot  
**Purpose:** Creating new stored procedures following MTM standards

**Prompt:**  
"Create a new stored procedure called `[procedure_name]` for [business purpose].  
Add the procedure to `Development/Database_Files/New_Stored_Procedures.sql` (NEVER edit production files).  
Include comprehensive error handling with p_Status and p_ErrorMsg output parameters.  
Follow MTM naming conventions: [category]_[entity]_[action]_[specifics].  
Add full documentation following the standards in README_New_Stored_Procedures.md.  
Include parameter validation, transaction management, and audit logging where appropriate.  
Ensure the procedure follows the security requirements and performance guidelines."

**Example Usage:**  
`Create a new stored procedure called inv_inventory_Get_LowStock for retrieving inventory items below reorder point`

---

### 2. Update Existing Stored Procedure
**Persona:** Database Architect Copilot  
**Purpose:** Modifying existing stored procedures safely

**Prompt:**  
"Update the existing stored procedure `[procedure_name]` to [modification description].  
Copy the original procedure from `Database_Files/Existing_Stored_Procedures.sql` (READ-ONLY) to `Development/Database_Files/Updated_Stored_Procedures.sql`.  
NEVER edit the original file in Database_Files/ - it represents current production state.  
Document all changes using the change documentation template.  
Maintain backward compatibility unless explicitly noted as a breaking change.  
Update the corresponding service layer methods if the interface changes.  
Include comprehensive testing notes and deployment instructions."

**Example Usage:**  
`Update the existing stored procedure inv_inventory_Add_Item to include additional validation for negative quantities`

---

### 3. Create Database Service Method
**Persona:** Application Logic Copilot + Database Architect Copilot  
**Purpose:** Creating service layer methods that call stored procedures

**Prompt:**  
"Create a service method in `[ServiceClass]` to call the stored procedure `[procedure_name]`.  
Use Helper_Database_StoredProcedure.ExecuteDataTableWithStatus() as required by MTM standards.  
Include comprehensive error handling with Service_ErrorHandler integration.  
Return Result<T> pattern for all operations.  
Add XML documentation explaining the business purpose and parameters.  
Ensure the method follows MTM data patterns (Part ID strings, Operation numbers, etc.).  
Include parameter validation before calling the stored procedure.  
Follow the connection string pattern using Model_AppVariables.ConnectionString."

**Example Usage:**  
`Create a service method in InventoryService to call the stored procedure inv_inventory_Get_ByLocation`

---

### 4. Validate Database Code Compliance
**Persona:** Security Auditor Copilot  
**Purpose:** Ensuring no hard-coded SQL exists in codebase and proper file organization

**Prompt:**  
"Audit all database-related code in [file/directory] for compliance with MTM database rules.  
Check for violations of the 'No Hard-Coded MySQL' rule - identify any direct SQL statements.  
Verify proper use of Helper_Database_StoredProcedure.ExecuteDataTableWithStatus() methods.  
Ensure no code is directly editing files in Database_Files/ (production files are READ-ONLY).  
Verify all development work is being done in Development/Database_Files/.  
Report any violations with specific file locations and recommended fixes.  
Ensure all database interactions go through the approved service layer."

**Example Usage:**  
`Audit all database-related code in Services/ directory for compliance with MTM database rules`

---

### 5. Convert Hard-Coded SQL to Stored Procedure
**Persona:** Database Architect Copilot + Application Logic Copilot  
**Purpose:** Refactoring existing SQL code to use stored procedures

**Prompt:**  
"Convert the hard-coded SQL statement `[SQL_CODE]` to use a stored procedure approach.  
Create the appropriate stored procedure in `Development/Database_Files/New_Stored_Procedures.sql`.  
Update the application code to call the new stored procedure using Helper_Database_StoredProcedure.ExecuteDataTableWithStatus().  
Ensure the conversion maintains the same functionality and performance.  
Add comprehensive error handling and parameter validation.  
Document the migration with before/after examples.  
Follow MTM connection string patterns with Model_AppVariables.ConnectionString."

**Example Usage:**  
`Convert the hard-coded SQL statement "SELECT * FROM inv_inventory WHERE PartID = @partId" to use a stored procedure approach`

---

### 6. Create Database Transaction Workflow
**Persona:** Database Architect Copilot  
**Purpose:** Creating multi-step database operations with transaction safety

**Prompt:**  
"Create a transaction workflow for [business process] that involves multiple stored procedures.  
Create coordinating stored procedures in `Development/Database_Files/New_Stored_Procedures.sql`.  
Use proper transaction management with BEGIN TRANSACTION, COMMIT, and ROLLBACK.  
Include rollback handling for any step failures.  
Call the following stored procedures in sequence: [procedure_list].  
Add comprehensive error handling and status tracking.  
Ensure each step validates the previous step's success before proceeding.  
Include audit logging for the complete transaction."

**Example Usage:**  
`Create a transaction workflow for inventory transfer that involves inv_inventory_Remove_Item and inv_inventory_Add_Item stored procedures`

---

### 7. Create Batch Processing Stored Procedure
**Persona:** Database Architect Copilot  
**Purpose:** Creating procedures that handle large data sets efficiently

**Prompt:**  
"Create a batch processing stored procedure `[procedure_name]` for [business purpose].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` following MTM standards.  
Process records in configurable batch sizes (default 1000 records).  
Include progress tracking and resumption capability for long-running operations.  
Add loop counters and maximum execution limits to prevent runaway processes.  
Return status information including processed count and completion status.  
Include cursor-based processing with proper memory management.  
Follow the patterns established in MTM database architecture."

**Example Usage:**  
`Create a batch processing stored procedure maint_archive_OldTransactions for moving old transaction records to archive tables`

---

### 8. Create Database Reporting Procedure
**Persona:** Database Architect Copilot + Reporting Specialist  
**Purpose:** Creating procedures for data analysis and reporting

**Prompt:**  
"Create a reporting stored procedure `[procedure_name]` that generates [report_description].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` with full documentation.  
Include date range parameters with proper validation.  
Add optional filtering parameters for user, location, part, etc.  
Return comprehensive result sets with calculated fields and aggregations.  
Include proper indexing hints and performance optimizations.  
Add documentation explaining the business logic and calculations.  
Ensure the procedure can handle large data volumes efficiently."

**Example Usage:**  
`Create a reporting stored procedure rpt_inventory_Usage_Summary that generates monthly inventory usage by part and location`

---

### 9. Create Data Validation Procedure
**Persona:** Database Architect Copilot + Data Validation Specialist  
**Purpose:** Creating procedures for data integrity checking

**Prompt:**  
"Create a data validation stored procedure `[procedure_name]` that checks [validation_rules].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` with comprehensive documentation.  
Return detailed validation results with specific error descriptions.  
Include options for validation-only mode and auto-correction mode.  
Add comprehensive logging of all validation failures.  
Include rollback capability if validation fails during corrections.  
Return statistics on total records checked, errors found, and corrections made.  
Follow the error handling patterns established in MTM procedures."

**Example Usage:**  
`Create a data validation stored procedure maint_validate_InventoryIntegrity that checks for orphaned transactions and batch number inconsistencies`

---

### 10. Debug Database Performance Issue
**Persona:** Database Performance Specialist Copilot  
**Purpose:** Analyzing and optimizing stored procedure performance

**Prompt:**  
"Analyze the performance of stored procedure `[procedure_name]` from production (`Database_Files/Existing_Stored_Procedures.sql`).  
Create optimized version in `Development/Database_Files/Updated_Stored_Procedures.sql`.  
Review the execution plan and identify slow queries or missing indexes.  
Suggest improvements for cursor usage, joins, and data access patterns.  
Recommend index additions or modifications.  
Check for parameter sniffing issues and suggest solutions.  
Provide before/after performance comparisons.  
Ensure optimizations maintain data integrity and business logic correctness."

**Example Usage:**  
`Analyze the performance of stored procedure inv_transaction_Get_ByDateRange and identify optimization opportunities`

---

### 11. Create Migration Stored Procedure
**Persona:** Database Architect Copilot + Migration Specialist  
**Purpose:** Creating procedures for data migration tasks

**Prompt:**  
"Create a data migration stored procedure `[procedure_name]` for [migration_purpose].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` with full documentation.  
Include comprehensive backup and rollback capabilities.  
Add progress tracking and status reporting throughout the migration.  
Include data validation before, during, and after migration.  
Implement resumption capability for large migrations.  
Add comprehensive logging of all migration steps and any issues.  
Include pre-migration checks to validate system readiness.  
Follow MTM transaction safety patterns."

**Example Usage:**  
`Create a data migration stored procedure migrate_inventory_NewBatchFormat for updating batch number format from 8 digits to 10 digits`

---

### 12. Create Emergency Data Recovery Procedure
**Persona:** Database Architect Copilot + Disaster Recovery Specialist  
**Purpose:** Creating procedures for data recovery scenarios

**Prompt:**  
"Create an emergency data recovery stored procedure `[procedure_name]` for [recovery_scenario].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` with emergency deployment notes.  
Include comprehensive data validation and integrity checking.  
Add detailed logging of all recovery operations.  
Include rollback capabilities if recovery fails.  
Provide detailed status reporting and progress tracking.  
Include verification steps to confirm successful recovery.  
Add emergency stop functionality for critical situations.  
Document all prerequisites and recovery steps."

**Example Usage:**  
`Create an emergency data recovery stored procedure emergency_restore_InventoryFromTransactions for rebuilding inventory from transaction history`

---

## Database Security Prompts

### 13. Security Audit Database Procedures
**Persona:** Security Auditor Copilot  
**Purpose:** Auditing stored procedures for security vulnerabilities

**Prompt:**  
"Conduct a comprehensive security audit of stored procedure `[procedure_name]` from `Database_Files/Existing_Stored_Procedures.sql`.  
Check for SQL injection vulnerabilities in dynamic SQL.  
Verify proper parameter validation and sanitization.  
Ensure sensitive data is properly protected.  
Check for privilege escalation possibilities.  
Verify proper error handling that doesn't expose sensitive information.  
Ensure audit logging is comprehensive and tamper-resistant.  
If fixes are needed, create updated version in `Development/Database_Files/Updated_Stored_Procedures.sql`.  
Provide specific recommendations for security improvements."

**Example Usage:**  
`Conduct a comprehensive security audit of stored procedure usr_users_Add_User`

---

### 14. Create Access Control Procedure
**Persona:** Security Engineer Copilot + Database Architect Copilot  
**Purpose:** Creating procedures with role-based access control

**Prompt:**  
"Create a stored procedure `[procedure_name]` with role-based access control for [operation].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` with security documentation.  
Include user permission checking against appropriate user roles tables.  
Add comprehensive audit logging of all access attempts.  
Return appropriate error messages for unauthorized access.  
Include parameter validation to prevent privilege escalation.  
Add session validation and timeout checking.  
Follow the security patterns established in MTM user management procedures."

**Example Usage:**  
`Create a stored procedure admin_modify_UserRoles with role-based access control for modifying user permissions`

---

## Performance Optimization Prompts

### 15. Optimize Slow Database Query
**Persona:** Database Performance Specialist Copilot  
**Purpose:** Optimizing specific database performance issues

**Prompt:**  
"Optimize the slow-performing section in stored procedure `[procedure_name]`: [slow_code_section].  
Copy original from `Database_Files/Existing_Stored_Procedures.sql` to `Development/Database_Files/Updated_Stored_Procedures.sql` for optimization.  
Analyze the execution plan and identify bottlenecks.  
Suggest index improvements or query restructuring.  
Consider replacing cursors with set-based operations where possible.  
Recommend partitioning strategies for large tables.  
Provide performance benchmarks before and after optimization.  
Ensure optimizations don't affect data integrity or business logic."

**Example Usage:**  
`Optimize the slow-performing section in stored procedure assign_BatchNumber_Step3 that matches transactions`

---

### 16. Create Database Maintenance Procedure
**Persona:** Database Architect Copilot + System Administrator  
**Purpose:** Creating automated maintenance procedures

**Prompt:**  
"Create a database maintenance stored procedure `[procedure_name]` for [maintenance_task].  
Add to `Development/Database_Files/New_Stored_Procedures.sql` with maintenance documentation.  
Include comprehensive status reporting and progress tracking.  
Add scheduling parameters and execution windows.  
Include automatic rollback if maintenance fails.  
Add performance impact monitoring during execution.  
Include cleanup of temporary data and log files.  
Provide detailed execution summaries and recommendations.  
Follow maintenance patterns from MTM database architecture."

**Example Usage:**  
`Create a database maintenance stored procedure maint_cleanup_ErrorLogs for archiving old error log entries`

---

## Usage Guidelines

### ??? File Organization Rules
1. **NEVER** edit files in `Database_Files/` - they are READ-ONLY production files
2. **ALWAYS** work in `Development/Database_Files/` for all changes
3. **New Procedures**: Add to `Development/Database_Files/New_Stored_Procedures.sql`
4. **Update Existing**: Copy to `Development/Database_Files/Updated_Stored_Procedures.sql` and modify
5. **Production Deployment**: Move tested procedures through proper change management

### Before Using These Prompts
1. **Review Current Procedures**: Check `Database_Files/Existing_Stored_Procedures.sql` to avoid duplication
2. **Understand Business Logic**: Ensure you understand the MTM-specific requirements
3. **Plan Testing**: Prepare comprehensive testing in development environment
4. **Review Security**: Ensure all security requirements are understood

### After Creating Database Code
1. **Test Thoroughly**: Test all code paths and error conditions in development
2. **Document Changes**: Update appropriate README files in `Development/Database_Files/`
3. **Review Performance**: Validate performance with realistic data
4. **Plan Deployment**: Create deployment procedures to move from Development to Production

### Security Reminders
- **Never** allow direct SQL execution in application code
- **Always** validate parameters in stored procedures
- **Always** use proper error handling and logging
- **Always** include audit trails for sensitive operations
- **Always** respect the READ-ONLY nature of production database files

### MTM-Specific Requirements
- Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` for all database calls
- Follow MTM connection string pattern with `Model_AppVariables.ConnectionString`
- Respect MTM transaction type logic (determined by user intent, not operation numbers)
- Include Service_ErrorHandler integration for comprehensive error management

These prompts ensure that all database operations follow MTM WIP Application standards while maintaining the highest levels of security, performance, and maintainability with proper file organization.