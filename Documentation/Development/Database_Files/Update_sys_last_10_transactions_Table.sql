-- =====================================================
-- MTM WIP Application - sys_last_10_transactions Table Update
-- =====================================================
-- Environment: Development
-- Status: TABLE STRUCTURE UPDATE
-- Purpose: Update sys_last_10_transactions table to match stored procedure expectations
-- =====================================================

USE mtm_wip_application_test;

-- =====================================================
-- Backup existing data (if table exists and has data)
-- =====================================================

-- Create backup table if sys_last_10_transactions exists with data
CREATE TABLE IF NOT EXISTS sys_last_10_transactions_backup AS 
SELECT * FROM sys_last_10_transactions WHERE 1=0;

-- Backup existing data if any exists
INSERT INTO sys_last_10_transactions_backup 
SELECT * FROM sys_last_10_transactions 
WHERE EXISTS (SELECT 1 FROM sys_last_10_transactions LIMIT 1);

-- =====================================================
-- Drop and recreate sys_last_10_transactions table
-- =====================================================

DROP TABLE IF EXISTS sys_last_10_transactions;

CREATE TABLE sys_last_10_transactions (
    ID int(11) NOT NULL AUTO_INCREMENT,
    User varchar(100) NOT NULL COMMENT 'User identifier - matches usr_users.User',
    PartID varchar(300) NOT NULL COMMENT 'Part identifier - matches md_part_ids.PartID',
    Operation varchar(100) NOT NULL COMMENT 'Operation workflow step - matches md_operation_numbers.Operation',
    Quantity int(11) NOT NULL COMMENT 'Transaction quantity - must be positive',
    ReceiveDate datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Transaction timestamp',
    Position int(11) NOT NULL COMMENT 'Position in user''s last 10 list (1-10)',
    PRIMARY KEY (ID),
    KEY idx_user_datetime (User, ReceiveDate),
    KEY idx_user_position (User, Position),
    KEY idx_partid (PartID),
    KEY idx_operation (Operation),
    UNIQUE KEY uq_user_position (User, Position) COMMENT 'Ensure unique position per user'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Stores last 10 transactions per user for quick button functionality';

-- =====================================================
-- Create indexes for optimal performance
-- =====================================================

-- Composite index for quick button queries (most common query pattern)
CREATE INDEX idx_user_receive_position ON sys_last_10_transactions (User, ReceiveDate DESC, Position ASC);

-- Index for cleanup operations
CREATE INDEX idx_receive_date ON sys_last_10_transactions (ReceiveDate ASC);

-- =====================================================
-- Ensure required reference data exists for sample data
-- =====================================================

-- Sample users (ensure these exist in usr_users table first)
INSERT INTO usr_users (User, `Full Name`, Shift, VitsUser, Pin, LastShownVersion, HideChangeLog, Theme_Name, Theme_FontSize, VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort) 
VALUES 
('admin', 'Administrator', '1', 0, NULL, '0.0.0.0', 'false', 'Default (Black and White)', 9, 'User Name', 'Password', '172.16.1.104', 'mtm_wip_application', '3306'),
('testuser', 'Test User', '1', 0, NULL, '0.0.0.0', 'false', 'Default (Black and White)', 9, 'User Name', 'Password', '172.16.1.104', 'mtm_wip_application', '3306')
ON DUPLICATE KEY UPDATE User = VALUES(User);

-- Sample item types (ensure these exist first)
INSERT INTO md_item_types (ItemType, IssuedBy) VALUES
('WIP', 'admin'),
('RAW', 'admin'),
('FG', 'admin')
ON DUPLICATE KEY UPDATE ItemType = VALUES(ItemType);

-- Sample parts (ensure these exist in md_part_ids table first)
INSERT INTO md_part_ids (PartID, Customer, Description, IssuedBy, ItemType, Operations) VALUES
('PART001', 'MTM', 'Sample Part 1', 'admin', 'WIP', NULL),
('PART002', 'MTM', 'Sample Part 2', 'admin', 'WIP', NULL),
('24733444-PKG', 'MTM', 'Package Part', 'admin', 'WIP', NULL)
ON DUPLICATE KEY UPDATE PartID = VALUES(PartID);

-- Sample operations (ensure these exist in md_operation_numbers table first)
INSERT INTO md_operation_numbers (Operation, IssuedBy) VALUES
('90', 'admin'),
('100', 'admin'),
('110', 'admin')
ON DUPLICATE KEY UPDATE Operation = VALUES(Operation);

-- =====================================================
-- Add foreign key constraints (after reference data exists)
-- =====================================================

-- Add foreign key to usr_users table
ALTER TABLE sys_last_10_transactions 
ADD CONSTRAINT fk_sys_last_10_transactions_user 
FOREIGN KEY (User) REFERENCES usr_users(User) 
ON DELETE CASCADE ON UPDATE CASCADE;

-- Add foreign key to md_part_ids table
ALTER TABLE sys_last_10_transactions 
ADD CONSTRAINT fk_sys_last_10_transactions_partid 
FOREIGN KEY (PartID) REFERENCES md_part_ids(PartID) 
ON DELETE CASCADE ON UPDATE CASCADE;

-- Add foreign key to md_operation_numbers table  
ALTER TABLE sys_last_10_transactions 
ADD CONSTRAINT fk_sys_last_10_transactions_operation 
FOREIGN KEY (Operation) REFERENCES md_operation_numbers(Operation) 
ON DELETE CASCADE ON UPDATE CASCADE;

-- =====================================================
-- Insert sample data for testing (now safe with FK constraints)
-- =====================================================

-- Sample transactions for testing
INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, ReceiveDate, Position) VALUES
('admin', 'PART001', '90', 25, NOW() - INTERVAL 5 MINUTE, 1),
('admin', 'PART002', '100', 15, NOW() - INTERVAL 4 MINUTE, 2),
('admin', 'PART001', '110', 30, NOW() - INTERVAL 3 MINUTE, 3),
('admin', '24733444-PKG', '90', 10, NOW() - INTERVAL 2 MINUTE, 4),
('admin', 'PART002', '110', 20, NOW() - INTERVAL 1 MINUTE, 5)
ON DUPLICATE KEY UPDATE 
    PartID = VALUES(PartID),
    Operation = VALUES(Operation),
    Quantity = VALUES(Quantity),
    ReceiveDate = VALUES(ReceiveDate);

-- =====================================================
-- Verification queries
-- =====================================================

-- Verify table structure
DESCRIBE sys_last_10_transactions;

-- Verify reference data exists
SELECT 'Users' as TableName, COUNT(*) as RecordCount FROM usr_users WHERE User IN ('admin', 'testuser')
UNION ALL
SELECT 'Parts' as TableName, COUNT(*) as RecordCount FROM md_part_ids WHERE PartID IN ('PART001', 'PART002', '24733444-PKG')
UNION ALL
SELECT 'Operations' as TableName, COUNT(*) as RecordCount FROM md_operation_numbers WHERE Operation IN ('90', '100', '110')
UNION ALL
SELECT 'Transactions' as TableName, COUNT(*) as RecordCount FROM sys_last_10_transactions;

-- Verify sample data
SELECT 
    ID,
    User,
    PartID,
    Operation,
    Quantity,
    ReceiveDate,
    Position
FROM sys_last_10_transactions 
ORDER BY User, ReceiveDate DESC;

-- Verify indexes
SHOW INDEX FROM sys_last_10_transactions;

-- Verify foreign key constraints
SELECT 
    CONSTRAINT_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'mtm_wip_application_test'
    AND TABLE_NAME = 'sys_last_10_transactions'
    AND REFERENCED_TABLE_NAME IS NOT NULL;

-- =====================================================
-- Performance test query (matches stored procedure)
-- =====================================================

-- Test the exact query used by the stored procedure
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

-- =====================================================
-- Clean up backup table (uncomment if backup not needed)
-- =====================================================

-- DROP TABLE IF EXISTS sys_last_10_transactions_backup;

-- =====================================================
-- Table Update Summary
-- =====================================================

/*
🔧 FIXED FOREIGN KEY CONSTRAINT ISSUES:

ISSUE RESOLUTION:
✅ Added complete reference data setup BEFORE creating foreign keys
✅ Ensured usr_users, md_part_ids, and md_operation_numbers have required data
✅ Moved foreign key creation AFTER reference data insertion
✅ Used ON DUPLICATE KEY UPDATE to safely handle existing data

TABLE STRUCTURE CHANGES MADE:
✅ ID - AUTO_INCREMENT primary key
✅ User - VARCHAR(100) for user identifier (matches usr_users.User)
✅ PartID - VARCHAR(300) for part identifier (matches md_part_ids.PartID) 
✅ Operation - VARCHAR(100) for operation workflow step (matches md_operation_numbers.Operation)
✅ Quantity - INT(11) for transaction quantity
✅ ReceiveDate - DATETIME with DEFAULT CURRENT_TIMESTAMP
✅ Position - INT(11) for position in user's last 10 list (1-10)

INDEXES CREATED:
✅ PRIMARY KEY on ID
✅ idx_user_datetime (User, ReceiveDate) - for quick button queries
✅ idx_user_position (User, Position) - for position management
✅ idx_partid (PartID) - for part lookups
✅ idx_operation (Operation) - for operation lookups  
✅ idx_user_receive_position (User, ReceiveDate DESC, Position ASC) - optimized composite index
✅ uq_user_position (User, Position) - unique constraint for position per user

FOREIGN KEY CONSTRAINTS (FIXED):
✅ fk_sys_last_10_transactions_user → usr_users(User)
✅ fk_sys_last_10_transactions_partid → md_part_ids(PartID)
✅ fk_sys_last_10_transactions_operation → md_operation_numbers(Operation)

REFERENCE DATA SETUP:
✅ Creates required users: 'admin', 'testuser'
✅ Creates required parts: 'PART001', 'PART002', '24733444-PKG'
✅ Creates required operations: '90', '100', '110'
✅ Creates required item types: 'WIP', 'RAW', 'FG'

STORED PROCEDURE COMPATIBILITY:
✅ Matches sys_last_10_transactions_Get_ByUser column expectations
✅ Supports sys_last_10_transactions_Add_Transaction insert operations
✅ Compatible with sys_last_10_transactions_Clear_ByUser delete operations

APPLICATION INTEGRATION:
✅ Supports QuickButtonsService.LoadLast10TransactionsAsync() 
✅ Compatible with QuickButtonsService.AddTransactionToLast10Async()
✅ Works with QuickButtonsViewModel quick button display
✅ Proper column mapping for QuickButtonData model

PERFORMANCE OPTIMIZATIONS:
✅ Composite index for most common query pattern (User + ReceiveDate DESC + Position)
✅ Individual indexes for foreign key relationships
✅ Unique constraint prevents duplicate positions per user
✅ Proper engine (InnoDB) for ACID compliance and foreign keys

DEPLOYMENT ORDER (FIXED):
1. Create table structure
2. Create indexes
3. Insert reference data (users, parts, operations)
4. Add foreign key constraints
5. Insert sample transaction data
6. Verify everything works

This script now handles the foreign key constraint error by ensuring all referenced data exists before creating the constraints and inserting sample data.
*/
