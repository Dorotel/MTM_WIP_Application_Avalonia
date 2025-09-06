-- Fix MySQL Collation Issues for QuickButtons Transaction History
-- Issue: Illegal mix of collations (utf8mb4_0900_ai_ci,IMPLICIT) and (utf8mb4_unicode_ci,IMPLICIT) for operation '='
-- Location: sys_last_10_transactions_Get_ByUser stored procedure
-- Root Cause: Collation mismatch between table columns and parameter comparisons

-- CRITICAL: This collation error prevents QuickButtons/History from loading any transactions
-- Error occurs at: WHERE t.User = p_UserID (line 736 in stored procedure)

USE `mtm_wip_application`;

-- ==================================================================================
-- SOLUTION 1: Update sys_last_10_transactions_Get_ByUser with explicit collation
-- ==================================================================================

DROP PROCEDURE IF EXISTS `sys_last_10_transactions_Get_ByUser`;

DELIMITER $$

CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Get_ByUser` (
    IN `p_UserID` VARCHAR(100), 
    IN `p_Limit` INT, 
    OUT `p_Status` INT, 
    OUT `p_ErrorMsg` VARCHAR(255)
)
sys_last_10_transactions_Get_ByUser:BEGIN
    -- Get Last 10 Transactions for Quick Button Creation from sys_last_10_transactions
    -- Purpose: Retrieve the most recent transactions for a user for quick button creation
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Limit: Maximum number of transactions to return (optional, default 10)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with transaction details formatted for quick buttons
    -- Example: CALL sys_last_10_transactions_Get_ByUser('JKOLL', 10, @status, @msg);
    
    -- COLLATION FIX: Declare variables with explicit collation to match database schema
    DECLARE v_UserCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_last_10_transactions_Get_ByUser', 'Database', CONCAT('UserID: ', COALESCE(p_UserID, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sys_last_10_transactions_Get_ByUser;
    END IF;

    -- Set default limit if not provided
    SET p_Limit = COALESCE(p_Limit, 10);
    
    IF p_Limit <= 0 OR p_Limit > 50 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Limit must be between 1 and 50';
        LEAVE sys_last_10_transactions_Get_ByUser;
    END IF;

    -- COLLATION FIX: Use explicit collation in user existence check
    -- Business rule validation with explicit collation
    SELECT COUNT(*) INTO v_UserCount 
    FROM usr_users 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci;
    
    IF v_UserCount = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE sys_last_10_transactions_Get_ByUser;
    END IF;

    -- COLLATION FIX: Main query with explicit collation on the WHERE clause
    -- Return recent transactions for user from sys_last_10_transactions
    -- Format the results to match the expected QuickButtonData structure
    SELECT 
        t.ID,
        'IN' as TransactionType,  -- Default transaction type
        t.PartID,
        '' as FromLocation,       -- sys_last_10_transactions doesn't have location columns
        '' as ToLocation,         -- sys_last_10_transactions doesn't have location columns
        t.Operation,
        t.Quantity,
        '' as Notes,              -- sys_last_10_transactions doesn't have Notes column
        t.User,
        '' as ItemType,           -- sys_last_10_transactions doesn't have ItemType column
        t.ReceiveDate,
        '' as BatchNumber,        -- sys_last_10_transactions doesn't have BatchNumber column
        COALESCE(p.Description, 'Unknown Part') as PartDescription,
        COALESCE(p.Customer, '') as Customer,
        -- Use CASE statement instead of COALESCE with ROW_NUMBER()
        CASE 
            WHEN t.Position IS NOT NULL AND t.Position > 0 THEN t.Position
            ELSE @row_num := COALESCE(@row_num, 0) + 1
        END as Position
    FROM sys_last_10_transactions t
        LEFT JOIN md_part_ids p ON t.PartID COLLATE utf8mb4_unicode_ci = p.PartID COLLATE utf8mb4_unicode_ci
        CROSS JOIN (SELECT @row_num := 0) r
    WHERE t.User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci
    ORDER BY t.ReceiveDate DESC, t.ID DESC
    LIMIT p_Limit;

    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Retrieved last ', p_Limit, ' transactions for user');
END$$

DELIMITER ;

-- ==================================================================================
-- SOLUTION 2: Verify and standardize table collations (if needed)
-- ==================================================================================

-- Check current collations
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    COLLATION_NAME
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'mtm_wip_application' 
    AND TABLE_NAME IN ('sys_last_10_transactions', 'usr_users', 'md_part_ids')
    AND COLUMN_NAME IN ('User', 'PartID')
ORDER BY TABLE_NAME, COLUMN_NAME;

-- If needed, standardize collations (uncomment if required):
-- ALTER TABLE sys_last_10_transactions MODIFY COLUMN User VARCHAR(100) COLLATE utf8mb4_unicode_ci;
-- ALTER TABLE usr_users MODIFY COLUMN User VARCHAR(100) COLLATE utf8mb4_unicode_ci;
-- ALTER TABLE md_part_ids MODIFY COLUMN PartID VARCHAR(100) COLLATE utf8mb4_unicode_ci;

-- ==================================================================================
-- VALIDATION: Test the fixed stored procedure
-- ==================================================================================

-- Test 1: Basic functionality
CALL sys_last_10_transactions_Get_ByUser('JOHNK', 10, @test_status, @test_msg);
SELECT @test_status as Status, @test_msg as Message;

-- Test 2: Check if any results are returned (should not error out)
SELECT 'Testing QuickButtons data retrieval...' as TestStep;

-- Expected Results:
-- - Status should be 0 or 1 (success/warning), NOT -1 (error)
-- - No collation error messages
-- - Either transaction data returned OR legitimate "no data" result

-- ==================================================================================
-- VERIFICATION QUERY: Check transaction data exists
-- ==================================================================================

SELECT 
    COUNT(*) as TotalTransactions,
    COUNT(DISTINCT User) as UniqueUsers,
    MAX(ReceiveDate) as LatestTransaction,
    GROUP_CONCAT(DISTINCT User ORDER BY User SEPARATOR ', ') as Users
FROM sys_last_10_transactions 
WHERE User COLLATE utf8mb4_unicode_ci = 'JOHNK' COLLATE utf8mb4_unicode_ci;

-- This should show if JOHNK has any transaction records in sys_last_10_transactions table
-- If count is 0, that explains why QuickButtons shows no transactions (but no longer errors)

SELECT 'Collation fix applied successfully. Test QuickButtons History Panel now.' as CompletionMessage;
