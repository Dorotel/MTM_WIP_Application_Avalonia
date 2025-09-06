-- ============================================================================
-- MTM WIP Application Database Diagnostic Script
-- Run this to check the stored procedure and table structure issues
-- ============================================================================

-- 1. Check if the stored procedure exists
SELECT 
    ROUTINE_NAME,
    ROUTINE_TYPE,
    CREATED,
    LAST_ALTERED,
    ROUTINE_DEFINITION
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'mtm_wip_application' 
AND ROUTINE_NAME = 'inv_inventory_Add_Item';

-- 2. Check the inventory table structure
DESCRIBE mtm_wip_application.inventory;

-- Alternative way to see table structure with more details
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH,
    COLUMN_KEY,
    EXTRA
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'mtm_wip_application' 
AND TABLE_NAME = 'inventory'
ORDER BY ORDINAL_POSITION;

-- 3. Check for any constraints on the inventory table
SELECT 
    CONSTRAINT_NAME,
    CONSTRAINT_TYPE,
    COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE TABLE_SCHEMA = 'mtm_wip_application' 
AND TABLE_NAME = 'inventory';

-- 4. Check foreign key constraints
SELECT 
    CONSTRAINT_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE TABLE_SCHEMA = 'mtm_wip_application' 
AND TABLE_NAME = 'inventory'
AND REFERENCED_TABLE_NAME IS NOT NULL;

-- 5. Try to manually execute the stored procedure with test data
-- (This will show us the exact error)
CALL mtm_wip_application.inv_inventory_Add_Item(
    'TEST123',          -- p_PartID
    'TEST-LOCATION',    -- p_Location  
    '90',               -- p_Operation
    1,                  -- p_Quantity
    'WIP',              -- p_ItemType
    'TESTUSER',         -- p_User
    'Test notes',       -- p_Notes
    @status,            -- p_Status (output)
    @message            -- p_ErrorMsg (output)
);

-- Get the output parameters
SELECT @status as Status, @message as ErrorMessage;

-- 6. Check recent error logs (if available)
SHOW VARIABLES LIKE 'log_error';
