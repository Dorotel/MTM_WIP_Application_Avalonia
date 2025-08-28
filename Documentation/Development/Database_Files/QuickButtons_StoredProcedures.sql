-- =====================================================
-- MTM WIP Application - Quick Buttons Stored Procedures
-- =====================================================
-- Environment: Development
-- Status: NEW - Quick buttons management procedures
-- Purpose: Manage sys_last_10_transactions table for quick button functionality
-- =====================================================

USE mtm_wip_application_test;

-- =====================================================
-- Drop Existing Procedures (Clean Deployment)
-- =====================================================

DROP PROCEDURE IF EXISTS sys_last_10_transactions_Get_ByUser;
DROP PROCEDURE IF EXISTS sys_last_10_transactions_Add_Transaction;
DROP PROCEDURE IF EXISTS sys_last_10_transactions_Clear_ByUser;

-- =====================================================
-- Quick Buttons Management Procedures
-- =====================================================

-- 1. Get Last 10 Transactions for User (for Quick Buttons)
DELIMITER ;;

CREATE PROCEDURE sys_last_10_transactions_Get_ByUser(
    IN p_UserID VARCHAR(100),
    IN p_Limit INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
sys_last_10_transactions_Get_ByUser: BEGIN
    -- Get Last 10 Transactions for Quick Button Creation from sys_last_10_transactions table
    -- Purpose: Retrieve user's recent transactions for populating quick buttons
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Limit: Number of transactions to return (optional, defaults to 10, max 10)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with transaction details for quick buttons
    -- Example: CALL sys_last_10_transactions_Get_ByUser('admin', 10, @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (COALESCE(p_UserID, 'system'), 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_last_10_transactions_Get_ByUser', 'Database', 
                CONCAT('UserID: ', COALESCE(p_UserID, 'NULL'), ', Limit: ', COALESCE(p_Limit, 0)));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sys_last_10_transactions_Get_ByUser;
    END IF;

    -- Set default limit if not provided and validate range
    SET p_Limit = COALESCE(p_Limit, 10);
    IF p_Limit <= 0 OR p_Limit > 10 THEN
        SET p_Limit = 10;
    END IF;

    -- Validate UserID exists
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('UserID "', p_UserID, '" does not exist');
        LEAVE sys_last_10_transactions_Get_ByUser;
    END IF;

    -- Get transactions from sys_last_10_transactions table
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
    WHERE t.User = p_UserID
    ORDER BY t.ReceiveDate DESC, t.Position ASC
    LIMIT p_Limit;

    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Retrieved last ', p_Limit, ' transactions for user');
END;;

DELIMITER ;

-- 2. Add Transaction to sys_last_10_transactions
DELIMITER ;;

CREATE PROCEDURE sys_last_10_transactions_Add_Transaction(
    IN p_UserID VARCHAR(100),
    IN p_PartID VARCHAR(300),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
sys_last_10_transactions_Add_Transaction: BEGIN
    -- Add new transaction to sys_last_10_transactions, maintaining only last 10
    -- Purpose: Add a new transaction and auto-manage the 10-item limit
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_PartID: Part identifier (required, must exist)
    --   p_Operation: Operation workflow step (required, must exist)
    --   p_Quantity: Transaction quantity (required, positive)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL sys_last_10_transactions_Add_Transaction('admin', 'PART001', '90', 100, @status, @msg);
    
    DECLARE v_MaxPosition INT DEFAULT 0;
    DECLARE v_TransactionCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (COALESCE(p_UserID, 'system'), 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_last_10_transactions_Add_Transaction', 'Database', 
                CONCAT('PartID: ', COALESCE(p_PartID, 'NULL'), ', Operation: ', COALESCE(p_Operation, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    IF p_Operation IS NULL OR p_Operation = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Operation is required and cannot be empty';
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('UserID "', p_UserID, '" does not exist');
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('PartID "', p_PartID, '" does not exist in master data');
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_Operation) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation "', p_Operation, '" does not exist in master data');
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    START TRANSACTION;

    -- Check if user already has 10 transactions
    SELECT COUNT(*), COALESCE(MAX(Position), 0)
    INTO v_TransactionCount, v_MaxPosition
    FROM sys_last_10_transactions
    WHERE User = p_UserID;

    -- If user has 10 transactions, remove the oldest one
    IF v_TransactionCount >= 10 THEN
        DELETE FROM sys_last_10_transactions 
        WHERE User = p_UserID 
        ORDER BY ReceiveDate ASC, Position ASC 
        LIMIT 1;
        
        -- Update positions to maintain sequence
        SET @new_position = 0;
        UPDATE sys_last_10_transactions 
        SET Position = (@new_position := @new_position + 1)
        WHERE User = p_UserID 
        ORDER BY ReceiveDate ASC, Position ASC;
        
        SET v_MaxPosition = 9; -- Next position will be 10
    END IF;

    -- Add new transaction at the next position
    INSERT INTO sys_last_10_transactions (
        User, PartID, Operation, Quantity, ReceiveDate, Position
    ) VALUES (
        p_UserID, p_PartID, p_Operation, p_Quantity, NOW(), v_MaxPosition + 1
    );

    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = 'Transaction added successfully to last 10 list';
END;;

DELIMITER ;

-- 3. Clear All Transactions for User
DELIMITER ;;

CREATE PROCEDURE sys_last_10_transactions_Clear_ByUser(
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
sys_last_10_transactions_Clear_ByUser: BEGIN
    -- Clear all transactions for a user from sys_last_10_transactions
    -- Purpose: Remove all stored transactions for a specific user
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL sys_last_10_transactions_Clear_ByUser('admin', @status, @msg);
    
    DECLARE v_TransactionCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (COALESCE(p_UserID, 'system'), 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_last_10_transactions_Clear_ByUser', 'Database', 
                CONCAT('UserID: ', COALESCE(p_UserID, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sys_last_10_transactions_Clear_ByUser;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('UserID "', p_UserID, '" does not exist');
        LEAVE sys_last_10_transactions_Clear_ByUser;
    END IF;

    -- Count existing transactions
    SELECT COUNT(*) INTO v_TransactionCount
    FROM sys_last_10_transactions 
    WHERE User = p_UserID;

    START TRANSACTION;

    -- Remove all transactions for user
    DELETE FROM sys_last_10_transactions 
    WHERE User = p_UserID;

    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Cleared ', v_TransactionCount, ' transactions for user ', p_UserID);
END;;

DELIMITER ;

-- =====================================================
-- End of Quick Buttons Stored Procedures
-- =====================================================

-- DEPLOYMENT NOTES:
-- 
-- ✅ SCHEMA COMPATIBILITY: These procedures work with the actual sys_last_10_transactions table
-- ✅ ERROR HANDLING: Comprehensive error handling with status/message output parameters  
-- ✅ VALIDATION: Input validation and business rule checking
-- ✅ TRANSACTION MANAGEMENT: Proper transaction handling for data consistency
-- ✅ LOGGING: Error logging to log_error table
-- ✅ MTM COMPLIANCE: Follows MTM business logic and naming conventions
--
-- INTEGRATION POINTS:
-- - Called by QuickButtonsService.LoadLast10TransactionsAsync()
-- - Used by DatabaseService.GetLastTransactionsForUserAsync()
-- - Supports Quick Buttons functionality in QuickButtonsViewModel
--
-- TABLE STRUCTURE REQUIREMENTS:
-- - sys_last_10_transactions table with columns: ID, User, PartID, Operation, Quantity, ReceiveDate, Position
-- - md_part_ids table for part validation
-- - md_operation_numbers table for operation validation  
-- - usr_users table for user validation
-- - log_error table for error logging
