-- =====================================================
-- MTM WIP Application - Quick Diagnostic for Empty Data Issue
-- =====================================================
-- Environment: Development
-- Status: DIAGNOSTIC SCRIPT
-- Purpose: Quick check for why QuickButtons are not loading data
-- =====================================================

USE mtm_wip_application_test;

-- =====================================================
-- Quick Diagnostic Checks
-- =====================================================

-- 1. Check if table exists and has any data at all
SELECT 
    'sys_last_10_transactions Table Status' as CheckType,
    CASE 
        WHEN EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'mtm_wip_application_test' AND table_name = 'sys_last_10_transactions')
        THEN 'EXISTS'
        ELSE 'MISSING'
    END as Status,
    COALESCE((SELECT COUNT(*) FROM sys_last_10_transactions), 0) as RecordCount;

-- 2. Show all data in table (if any)
SELECT 'All data in sys_last_10_transactions:' as Info;
SELECT * FROM sys_last_10_transactions ORDER BY ReceiveDate DESC;

-- 3. Check if 'admin' user exists in usr_users
SELECT 
    'admin user in usr_users' as CheckType,
    CASE 
        WHEN EXISTS (SELECT 1 FROM usr_users WHERE User = 'admin')
        THEN 'EXISTS'
        ELSE 'MISSING'
    END as Status;

-- 4. Show what users are available
SELECT 'Available users in usr_users:' as Info;
SELECT User, `Full Name` FROM usr_users ORDER BY User LIMIT 10;

-- 5. Quick test of stored procedure with 'admin'
SET @p_Status = 0;
SET @p_ErrorMsg = '';

CALL sys_last_10_transactions_Get_ByUser('admin', 10, @p_Status, @p_ErrorMsg);

SELECT 
    'Stored Procedure Test Results' as TestType,
    @p_Status as Status,
    @p_ErrorMsg as Message,
    CASE @p_Status
        WHEN 0 THEN 'SUCCESS'
        WHEN 1 THEN 'WARNING'
        ELSE 'ERROR'
    END as Interpretation;

-- 6. If no data exists, add minimal sample data
INSERT IGNORE INTO sys_last_10_transactions (User, PartID, Operation, Quantity, ReceiveDate, Position) VALUES
('admin', 'TEST001', '90', 10, NOW(), 1),
('admin', 'TEST002', '100', 20, NOW() - INTERVAL 1 MINUTE, 2);

-- 7. Re-test after adding data
SET @p_Status = 0;
SET @p_ErrorMsg = '';

CALL sys_last_10_transactions_Get_ByUser('admin', 10, @p_Status, @p_ErrorMsg);

SELECT 
    'After Adding Sample Data' as TestType,
    @p_Status as Status,
    @p_ErrorMsg as Message,
    'Should now return data if issue was empty table' as Note;

-- =====================================================
-- Summary and Next Steps
-- =====================================================

SELECT 'DIAGNOSTIC SUMMARY:' as Info;

SELECT 
    'Check Results:' as Summary,
    (SELECT COUNT(*) FROM sys_last_10_transactions) as TableRecords,
    (SELECT COUNT(*) FROM usr_users WHERE User = 'admin') as AdminUserExists,
    @p_Status as LastTestStatus,
    @p_ErrorMsg as LastTestMessage;

/*
🔍 NEXT STEPS BASED ON RESULTS:

1. If TableRecords = 0:
   - Table is empty, need to add data or run table update script
   - Check if application is calling AddTransactionToLast10Async after inventory operations

2. If AdminUserExists = 0:
   - 'admin' user doesn't exist in usr_users table
   - Need to use a different username or create 'admin' user
   - Check what username the application is using

3. If LastTestStatus = 1:
   - Stored procedure validation failed
   - Check error message for specific issue
   - May be foreign key constraint issues

4. If LastTestStatus = 0 but application still shows empty:
   - Stored procedure works, issue is in application
   - Check application logs for user parameter being passed
   - Verify Helper_Database_StoredProcedure parameter mapping

5. Common Issues:
   - Case sensitivity: 'admin' vs 'ADMIN' vs 'Admin'
   - Application using different username than database
   - Foreign key constraints preventing data insertion
   - Parameter name mismatch in stored procedure calls
*/
