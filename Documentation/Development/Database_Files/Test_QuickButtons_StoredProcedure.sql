-- =====================================================
-- MTM WIP Application - Test QuickButtons Stored Procedure
-- =====================================================
-- Environment: Development
-- Status: TEST SCRIPT
-- Purpose: Test the sys_last_10_transactions_Get_ByUser stored procedure
-- =====================================================

USE mtm_wip_application_test;

-- =====================================================
-- Pre-test diagnostics
-- =====================================================

-- Check if stored procedure exists
SELECT 
    ROUTINE_NAME,
    ROUTINE_TYPE,
    CREATED,
    LAST_ALTERED
FROM information_schema.ROUTINES 
WHERE ROUTINE_SCHEMA = 'mtm_wip_application_test' 
AND ROUTINE_NAME = 'sys_last_10_transactions_Get_ByUser';

-- Check if table exists and has data
SELECT 
    TABLE_NAME,
    TABLE_ROWS,
    CREATE_TIME,
    UPDATE_TIME
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'mtm_wip_application_test' 
AND TABLE_NAME = 'sys_last_10_transactions';

-- =====================================================
-- Check current data in table
-- =====================================================

-- Show all data in sys_last_10_transactions
SELECT 
    'Current table data:' as Info,
    COUNT(*) as TotalRecords
FROM sys_last_10_transactions;

SELECT * FROM sys_last_10_transactions ORDER BY ReceiveDate DESC;

-- Check what users exist
SELECT DISTINCT User FROM sys_last_10_transactions;

-- Check what users exist in usr_users table
SELECT User, `Full Name` FROM usr_users LIMIT 10;

-- =====================================================
-- Test the stored procedure directly
-- =====================================================

-- Test with 'admin' user
SET @p_Status = 0;
SET @p_ErrorMsg = '';

CALL sys_last_10_transactions_Get_ByUser('admin', 10, @p_Status, @p_ErrorMsg);

-- Check output parameters
SELECT @p_Status as Status, @p_ErrorMsg as ErrorMessage;

-- Test with different case variations
SET @p_Status = 0;
SET @p_ErrorMsg = '';

CALL sys_last_10_transactions_Get_ByUser('ADMIN', 10, @p_Status, @p_ErrorMsg);

-- Check output parameters
SELECT @p_Status as Status_Upper, @p_ErrorMsg as ErrorMessage_Upper;

-- Test with actual user from usr_users table
SET @p_Status = 0;
SET @p_ErrorMsg = '';
SET @actual_user = (SELECT User FROM usr_users LIMIT 1);

SELECT CONCAT('Testing with actual user: ', @actual_user) as TestInfo;

CALL sys_last_10_transactions_Get_ByUser(@actual_user, 10, @p_Status, @p_ErrorMsg);

-- Check output parameters
SELECT @p_Status as Status_Actual, @p_ErrorMsg as ErrorMessage_Actual;

-- =====================================================
-- Debugging: Test with direct query
-- =====================================================

-- Test the exact query from the stored procedure
SELECT 
    t.ID,
    t.User as UserID,
    t.PartID,
    t.Operation,
    t.Quantity,
    t.ReceiveDate as CreatedDate,
    t.ReceiveDate as LastUsedDate,
    t.Position,
    p.Description as PartDescription,
    p.Customer,
    '' as Notes
FROM sys_last_10_transactions t
    LEFT JOIN md_part_ids p ON t.PartID = p.PartID
WHERE t.User = 'admin'
ORDER BY t.ReceiveDate DESC, t.Position ASC
LIMIT 10;

-- Test with any user that has data
SELECT 
    'Testing with any existing user:' as Info;
    
SELECT 
    t.ID,
    t.User as UserID,
    t.PartID,
    t.Operation,
    t.Quantity,
    t.ReceiveDate as CreatedDate,
    t.ReceiveDate as LastUsedDate,
    t.Position,
    p.Description as PartDescription,
    p.Customer,
    '' as Notes
FROM sys_last_10_transactions t
    LEFT JOIN md_part_ids p ON t.PartID = p.PartID
ORDER BY t.ReceiveDate DESC, t.Position ASC
LIMIT 10;

-- =====================================================
-- Add test data if none exists
-- =====================================================

-- Check if we need to add sample data
SELECT 
    CASE 
        WHEN (SELECT COUNT(*) FROM sys_last_10_transactions) = 0 
        THEN 'No data found - adding sample data'
        ELSE 'Data exists - skipping sample data creation'
    END as DataStatus;

-- Add sample data if table is empty (only if no data exists)
INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, ReceiveDate, Position)
SELECT * FROM (
    SELECT 'admin' as User, 'PART001' as PartID, '90' as Operation, 25 as Quantity, NOW() - INTERVAL 5 MINUTE as ReceiveDate, 1 as Position
    UNION ALL
    SELECT 'admin', 'PART002', '100', 15, NOW() - INTERVAL 4 MINUTE, 2
    UNION ALL
    SELECT 'admin', 'PART001', '110', 30, NOW() - INTERVAL 3 MINUTE, 3
    UNION ALL
    SELECT 'admin', '24733444-PKG', '90', 10, NOW() - INTERVAL 2 MINUTE, 4
    UNION ALL
    SELECT 'admin', 'PART002', '110', 20, NOW() - INTERVAL 1 MINUTE, 5
) sample_data
WHERE NOT EXISTS (SELECT 1 FROM sys_last_10_transactions WHERE User = 'admin');

-- =====================================================
-- Final test after potential data insertion
-- =====================================================

-- Final count check
SELECT 
    'Final data check:' as Info,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT User) as UniqueUsers
FROM sys_last_10_transactions;

-- Final stored procedure test
SET @p_Status = 0;
SET @p_ErrorMsg = '';

CALL sys_last_10_transactions_Get_ByUser('admin', 10, @p_Status, @p_ErrorMsg);

-- Check final output parameters
SELECT 
    @p_Status as Final_Status, 
    @p_ErrorMsg as Final_ErrorMessage,
    'Expected: Status=0, Message contains success' as Expected_Result;

-- =====================================================
-- Test Results Summary
-- =====================================================

/*
🔍 DIAGNOSTIC RESULTS:

This enhanced test will show you:

1. ✅ Whether the stored procedure exists
2. ✅ Whether the table exists and has data  
3. ✅ What users actually exist in the table
4. ✅ Whether the stored procedure returns data for existing users
5. ✅ The exact query results that should be returned
6. ✅ Sample data creation if table is empty

🎯 EXPECTED OUTCOMES:

If everything is working:
- Status should be 0 (success)
- ErrorMessage should contain "Retrieved last X transactions for user"
- Data rows should be returned showing ID, UserID, PartID, etc.

If no data is loading:
- Check if the User parameter matches exactly (case sensitive)
- Verify sample data was inserted
- Confirm foreign key relationships are working
- Check if md_part_ids table has matching PartID records

🚨 TROUBLESHOOTING:

If still no data after this test:
1. User case sensitivity issues
2. Empty sys_last_10_transactions table
3. Foreign key constraint blocking data insertion
4. Application not calling stored procedure correctly
5. Current user in application doesn't match database users
*/
