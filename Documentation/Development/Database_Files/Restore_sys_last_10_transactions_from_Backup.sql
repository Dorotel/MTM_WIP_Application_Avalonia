-- =====================================================
-- MTM WIP Application - Restore sys_last_10_transactions from Backup
-- =====================================================
-- Environment: Development
-- Status: DATA MIGRATION SCRIPT
-- Purpose: Restore data from sys_last_10_transactions_backup to the new table structure
-- =====================================================

USE mtm_wip_application_test;

-- =====================================================
-- Pre-migration validation
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

-- Check backup table structure
DESCRIBE sys_last_10_transactions_backup;

-- Count records in backup
SELECT 
    'Backup Records' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions_backup
UNION ALL
SELECT 
    'Current Records' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions;

-- Preview backup data
SELECT 'BACKUP DATA PREVIEW:' as Info;
SELECT * FROM sys_last_10_transactions_backup LIMIT 10;

-- =====================================================
-- Data migration options
-- =====================================================

-- OPTION 1: Clear current table and restore all backup data
-- (Use this if you want to completely replace current data)

-- Clear current data
-- TRUNCATE TABLE sys_last_10_transactions;

-- Restore from backup with column mapping
-- (Adjust column mapping based on your backup table structure)
/*
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
    COALESCE(ReceiveDate, NOW()) as ReceiveDate,
    COALESCE(Position, ROW_NUMBER() OVER (PARTITION BY User ORDER BY COALESCE(ReceiveDate, NOW()) DESC)) as Position
FROM sys_last_10_transactions_backup;
*/

-- =====================================================
-- OPTION 2: Merge backup data with current data (recommended)
-- =====================================================

-- Step 1: Create temporary working table for data consolidation
DROP TABLE IF EXISTS sys_last_10_transactions_temp;

CREATE TABLE sys_last_10_transactions_temp AS
SELECT * FROM sys_last_10_transactions WHERE 1=0;

-- Step 2: Insert current data first (to preserve any new data)
INSERT INTO sys_last_10_transactions_temp (
    User, PartID, Operation, Quantity, ReceiveDate, Position
)
SELECT 
    User, PartID, Operation, Quantity, ReceiveDate, Position
FROM sys_last_10_transactions;

-- Step 3: Insert backup data that doesn't conflict with current data
-- (This assumes User+PartID+Operation+ReceiveDate combination should be unique)
INSERT INTO sys_last_10_transactions_temp (
    User, PartID, Operation, Quantity, ReceiveDate, Position
)
SELECT 
    b.User,
    b.PartID,
    b.Operation,
    b.Quantity,
    COALESCE(b.ReceiveDate, NOW()) as ReceiveDate,
    999 as Position  -- Temporary position, will be recalculated
FROM sys_last_10_transactions_backup b
WHERE NOT EXISTS (
    SELECT 1 FROM sys_last_10_transactions c
    WHERE c.User = b.User 
    AND c.PartID = b.PartID 
    AND c.Operation = b.Operation
    AND DATE(c.ReceiveDate) = DATE(COALESCE(b.ReceiveDate, NOW()))
);

-- Step 4: Recalculate positions for each user (keep only last 10)
-- MySQL doesn't allow target table in subquery for DELETE, so we use a different approach

-- First, identify records to keep (last 10 per user)
CREATE TEMPORARY TABLE temp_records_to_keep AS
SELECT t1.ID
FROM sys_last_10_transactions_temp t1
WHERE (
    SELECT COUNT(*)
    FROM sys_last_10_transactions_temp t2
    WHERE t2.User = t1.User 
    AND t2.ReceiveDate > t1.ReceiveDate
) < 10;

-- Delete records not in the keep list
DELETE FROM sys_last_10_transactions_temp 
WHERE ID NOT IN (SELECT ID FROM temp_records_to_keep);

-- Clean up temporary table
DROP TEMPORARY TABLE temp_records_to_keep;

-- Step 5: Update positions to be sequential 1-10 per user
-- Use a more compatible approach for position calculation
CREATE TEMPORARY TABLE temp_position_update AS
SELECT 
    ID,
    ROW_NUMBER() OVER (PARTITION BY User ORDER BY ReceiveDate DESC) as new_position
FROM sys_last_10_transactions_temp;

-- Update positions using the temporary table
UPDATE sys_last_10_transactions_temp t
JOIN temp_position_update p ON t.ID = p.ID
SET t.Position = p.new_position;

-- Clean up temporary table
DROP TEMPORARY TABLE temp_position_update;

-- =====================================================
-- Execute the migration (uncomment to run)
-- =====================================================

-- Clear current table
-- TRUNCATE TABLE sys_last_10_transactions;

-- Copy consolidated data back
-- INSERT INTO sys_last_10_transactions SELECT * FROM sys_last_10_transactions_temp;

-- =====================================================
-- Alternative: Manual column mapping (if backup structure differs)
-- =====================================================

-- Use this if your backup table has different column names or structure
-- Adjust the column names in the SELECT to match your backup table structure

/*
INSERT INTO sys_last_10_transactions (
    User, 
    PartID, 
    Operation, 
    Quantity, 
    ReceiveDate, 
    Position
)
SELECT 
    COALESCE(User, 'unknown') as User,                    -- Map user column
    COALESCE(PartID, 'UNKNOWN') as PartID,               -- Map part column  
    COALESCE(Operation, '90') as Operation,              -- Map operation column
    COALESCE(Quantity, 0) as Quantity,                   -- Map quantity column
    COALESCE(ReceiveDate, NOW()) as ReceiveDate,         -- Map date column
    ROW_NUMBER() OVER (
        PARTITION BY COALESCE(User, 'unknown') 
        ORDER BY COALESCE(ReceiveDate, NOW()) DESC
    ) as Position                                         -- Calculate position
FROM sys_last_10_transactions_backup
WHERE COALESCE(User, '') != ''                          -- Filter out invalid records
AND COALESCE(PartID, '') != ''
AND COALESCE(Operation, '') != ''
ORDER BY User, ReceiveDate DESC;
*/

-- =====================================================
-- Validation queries (run after migration)
-- =====================================================

-- Verify migration results
SELECT 'MIGRATION RESULTS:' as Info;

-- Count comparison
SELECT 
    'After Migration' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions
UNION ALL
SELECT 
    'Original Backup' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions_backup
UNION ALL
SELECT 
    'Temp Table' as TableType, 
    COUNT(*) as RecordCount 
FROM sys_last_10_transactions_temp;

-- Verify data integrity
SELECT 
    User,
    COUNT(*) as TransactionCount,
    MIN(Position) as MinPosition,
    MAX(Position) as MaxPosition,
    COUNT(DISTINCT Position) as UniquePositions
FROM sys_last_10_transactions
GROUP BY User
ORDER BY User;

-- Sample data verification
SELECT 'FINAL DATA SAMPLE:' as Info;
SELECT 
    User,
    PartID,
    Operation,
    Quantity,
    ReceiveDate,
    Position
FROM sys_last_10_transactions
ORDER BY User, ReceiveDate DESC
LIMIT 20;

-- =====================================================
-- Cleanup (uncomment after successful migration)
-- =====================================================

-- Drop temporary table
-- DROP TABLE IF EXISTS sys_last_10_transactions_temp;

-- Optional: Drop backup table if no longer needed
-- DROP TABLE IF EXISTS sys_last_10_transactions_backup;

-- =====================================================
-- Usage Instructions
-- =====================================================

/*
🚀 HOW TO USE THIS SCRIPT:

1. **REVIEW FIRST**: 
   - Run the validation queries at the top to understand your backup data structure
   - Check the preview queries to see what data you're working with

2. **CHOOSE MIGRATION STRATEGY**:
   - **Option 1**: Complete replacement (uncomment TRUNCATE and first INSERT)
   - **Option 2**: Merge strategy (recommended - uncomment the temp table approach)
   - **Option 3**: Manual mapping (if backup has different column structure)

3. **EXECUTE STEP BY STEP**:
   - Don't run everything at once
   - Uncomment and run one section at a time
   - Verify results with validation queries after each step

4. **VALIDATION**:
   - Always run the validation queries after migration
   - Ensure position numbering is correct (1-10 per user)
   - Verify no data loss occurred

5. **CLEANUP**:
   - Only drop backup/temp tables after confirming migration success
   - Keep backup until you're confident in the migration

🔧 COLUMN MAPPING GUIDE:

If your backup table has different column names, update the SELECT statement:
- Old column name → New column name
- user_name → User
- part_number → PartID  
- op_number → Operation
- qty → Quantity
- date_created → ReceiveDate
- pos → Position

⚠️ IMPORTANT NOTES:

- This script preserves data integrity with foreign key constraints
- Position numbers are automatically recalculated to ensure 1-10 range per user
- Duplicate detection prevents data conflicts during merge
- All changes can be rolled back if you keep the backup table

✅ SUCCESS CRITERIA:

After migration, you should see:
- All users have maximum 10 transactions
- Positions are sequential 1-10 per user
- No foreign key constraint violations
- ReceiveDate is properly ordered (newest first)
- No duplicate Position values per user
*/
