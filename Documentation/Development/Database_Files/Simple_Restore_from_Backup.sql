-- =====================================================
-- MTM WIP Application - Simple Backup Restore
-- =====================================================
-- Environment: Development
-- Status: SIMPLE DATA COPY SCRIPT
-- Purpose: Copy all data from backup to new sys_last_10_transactions table
-- =====================================================

USE mtm_wip_application_test;

-- =====================================================
-- Simple verification (optional - can skip)
-- =====================================================

-- Check if backup table exists
SELECT 
    CASE 
        WHEN EXISTS (SELECT 1 FROM information_schema.tables 
                    WHERE table_schema = 'mtm_wip_application_test' 
                    AND table_name = 'sys_last_10_transactions_backup') 
        THEN 'Backup table exists' 
        ELSE 'ERROR: Backup table not found!' 
    END as BackupTableStatus;

-- Count records
SELECT 
    'Backup Records' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions_backup
UNION ALL
SELECT 
    'Current Records' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions;

-- =====================================================
-- Simple copy operation
-- =====================================================

-- Clear current table
TRUNCATE TABLE sys_last_10_transactions;

-- Copy all data from backup to new table
INSERT INTO sys_last_10_transactions (
    User, 
    PartID, 
    Operation, 
    Quantity, 
    ReceiveDate, 
    Position
)
SELECT 
    User,
    PartID,
    Operation,
    Quantity,
    ReceiveDate,
    Position
FROM sys_last_10_transactions_backup;

-- =====================================================
-- Simple verification (optional)
-- =====================================================

-- Count after copy
SELECT 
    'After Copy' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions;

-- Show sample of copied data
SELECT * FROM sys_last_10_transactions LIMIT 10;

-- =====================================================
-- Cleanup (optional - uncomment if you want to remove backup)
-- =====================================================

-- DROP TABLE IF EXISTS sys_last_10_transactions_backup;

-- =====================================================
-- Done!
-- =====================================================

/*
✅ SIMPLE COPY COMPLETE

This script:
1. Clears the current sys_last_10_transactions table
2. Copies ALL data from sys_last_10_transactions_backup
3. Shows verification counts
4. Optionally removes the backup table

No complex logic, no position recalculation, no merging.
Just a straight copy from backup to new table.
*/
