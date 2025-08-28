-- =====================================================
-- MTM WIP Application - Quick Buttons Stored Procedures - CORRECTED
-- =====================================================
-- Environment: Development
-- Status: Updated for sys_last_10_transactions schema
-- Last Updated: Schema corrected to match actual database structure
-- =====================================================

-- Quick Button procedures that work with the sys_last_10_transactions table
-- This table has the following structure:
-- - ID (int, AUTO_INCREMENT, PRIMARY KEY)
-- - User (varchar(100)) 
-- - PartID (varchar(300))
-- - Operation (varchar(100))
-- - Quantity (int)
-- - ReceiveDate (datetime)
-- - Position (int) - for tracking user's last 10 transactions order

USE mtm_wip_application_test;

-- =====================================================
-- Drop Existing Quick Button Procedures
-- =====================================================

DROP PROCEDURE IF EXISTS qb_quickbuttons_Save;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Remove;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Clear_ByUser;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Get_ByUser;
DROP PROCEDURE IF EXISTS sys_last_10_transactions_Get_ByUser;
DROP PROCEDURE IF EXISTS sys_last_10_transactions_Add_Transaction;

-- =====================================================
-- Quick Buttons Management Procedures
-- =====================================================

-- 1. Save Quick Button - Works with sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Save(
    IN p_UserID VARCHAR(100),
    IN p_Position INT,
    IN p_PartID VARCHAR(300),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_Notes TEXT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
qb_quickbuttons_Save: BEGIN
    -- Save/Update Quick Button in sys_last_10_transactions table
    -- Purpose: Save or update a quick action button for a specific user
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Position: Button position (1-10, required)
    --   p_PartID: Part identifier (required)
    --   p_Operation: Operation workflow step (required)
    --   p_Quantity: Quantity for quick action (required, positive)
    --   p_Notes: Additional notes (optional - not used in sys_last_10_transactions)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL qb_quickbuttons_Save('JKOLL', 1, 'PART001', '90', 100, 'Quick add', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'qb_quickbuttons_Save', 'Database', CONCAT('Position: ', COALESCE(p_Position, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE qb_quickbuttons_Save;
    END IF;

    IF p_Position IS NULL OR p_Position < 1 OR p_Position > 10 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Position must be between 1 and 10';
        LEAVE qb_quickbuttons_Save;
    END IF;

    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE qb_quickbuttons_Save;
    END IF;

    IF p_Operation IS NULL OR p_Operation = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Operation is required and cannot be empty';
        LEAVE qb_quickbuttons_Save;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE qb_quickbuttons_Save;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE qb_quickbuttons_Save;
    END IF;

    START TRANSACTION;
    
    -- Insert or update quick button in sys_last_10_transactions
    -- Use REPLACE to handle duplicates based on User + Position
    REPLACE INTO sys_last_10_transactions (User, PartID, Operation, Quantity, Position, ReceiveDate)
    VALUES (p_UserID, p_PartID, p_Operation, p_Quantity, p_Position, NOW());
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Quick button saved successfully at position ', p_Position);
END;;

DELIMITER ;

-- 2. Remove Quick Button - Works with sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Remove(
    IN p_ButtonID INT,
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
qb_quickbuttons_Remove: BEGIN
    -- Remove Quick Button by Position from sys_last_10_transactions
    -- Purpose: Remove a specific quick button at the given position
    -- Parameters:
    --   p_ButtonID: Button position to remove (1-10, required)
    --   p_UserID: User identifier (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL qb_quickbuttons_Remove(1, 'JKOLL', @status, @msg);
    
    DECLARE v_RecordCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'qb_quickbuttons_Remove', 'Database', CONCAT('ButtonID: ', COALESCE(p_ButtonID, 'NULL')));
    END;

    -- Input validation
    IF p_ButtonID IS NULL OR p_ButtonID < 1 OR p_ButtonID > 10 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ButtonID must be between 1 and 10';
        LEAVE qb_quickbuttons_Remove;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE qb_quickbuttons_Remove;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE qb_quickbuttons_Remove;
    END IF;

    -- Check if button exists at this position
    SELECT COUNT(*) INTO v_RecordCount
    FROM sys_last_10_transactions 
    WHERE User = p_UserID AND Position = p_ButtonID;

    IF v_RecordCount = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Quick button not found at position ', p_ButtonID);
        LEAVE qb_quickbuttons_Remove;
    END IF;

    START TRANSACTION;
    
    -- Remove the button from sys_last_10_transactions
    DELETE FROM sys_last_10_transactions 
    WHERE User = p_UserID AND Position = p_ButtonID;
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Quick button removed successfully from position ', p_ButtonID);
END;;

DELIMITER ;

-- 3. Clear All Quick Buttons for User - Works with sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Clear_ByUser(
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
qb_quickbuttons_Clear_ByUser: BEGIN
    -- Clear All Quick Buttons for a User from sys_last_10_transactions
    -- Purpose: Remove all quick buttons for a specific user
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL qb_quickbuttons_Clear_ByUser('JKOLL', @status, @msg);
    
    DECLARE v_ButtonCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'qb_quickbuttons_Clear_ByUser', 'Database', CONCAT('UserID: ', COALESCE(p_UserID, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE qb_quickbuttons_Clear_ByUser;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE qb_quickbuttons_Clear_ByUser;
    END IF;

    -- Count existing buttons
    SELECT COUNT(*) INTO v_ButtonCount
    FROM sys_last_10_transactions 
    WHERE User = p_UserID;

    START TRANSACTION;
    
    -- Remove all buttons for user from sys_last_10_transactions
    DELETE FROM sys_last_10_transactions 
    WHERE User = p_UserID;
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Cleared ', v_ButtonCount, ' quick buttons for user ', p_UserID);
END;;

DELIMITER ;

-- 4. Get Quick Buttons by User - Works with sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Get_ByUser(
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
qb_quickbuttons_Get_ByUser: BEGIN
    -- Get All Quick Buttons for a User from sys_last_10_transactions
    -- Purpose: Retrieve all quick buttons for a specific user ordered by position
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with quick button details
    -- Example: CALL qb_quickbuttons_Get_ByUser('JKOLL', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'qb_quickbuttons_Get_ByUser', 'Database', CONCAT('UserID: ', COALESCE(p_UserID, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE qb_quickbuttons_Get_ByUser;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE qb_quickbuttons_Get_ByUser;
    END IF;

    -- Return quick buttons for user from sys_last_10_transactions
    SELECT 
        ID,
        User as UserID,
        Position,
        PartID,
        Operation,
        Quantity,
        '' as Notes,  -- sys_last_10_transactions doesn't have Notes column
        ReceiveDate as CreatedDate,
        ReceiveDate as LastUsedDate
    FROM sys_last_10_transactions
    WHERE User = p_UserID
        AND Position BETWEEN 1 AND 10
    ORDER BY Position;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quick buttons retrieved successfully';
END;;

DELIMITER ;

-- 5. Get Last 10 Transactions for User - Works with sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE sys_last_10_transactions_Get_ByUser(
    IN p_UserID VARCHAR(100),
    IN p_Limit INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
sys_last_10_transactions_Get_ByUser: BEGIN
    -- Get Last 10 Transactions for Quick Button Creation from sys_last_10_transactions
    -- Purpose: Retrieve the most recent transactions for a user for quick button creation
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_Limit: Maximum number of transactions to return (optional, default 10)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with transaction details formatted for quick buttons
    -- Example: CALL sys_last_10_transactions_Get_ByUser('JKOLL', 10, @status, @msg);
    
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

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE sys_last_10_transactions_Get_ByUser;
    END IF;

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
        LEFT JOIN md_part_ids p ON t.PartID = p.PartID
        CROSS JOIN (SELECT @row_num := 0) r
    WHERE t.User = p_UserID
    ORDER BY t.ReceiveDate DESC, t.ID DESC
    LIMIT p_Limit;

    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Retrieved last ', p_Limit, ' transactions for user');
END;;

DELIMITER ;

-- 6. Add Transaction to Last 10 - Works with sys_last_10_transactions schema
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
    -- Add Transaction to Last 10 Transactions for Quick Button Creation
    -- Purpose: Add a new transaction to the user's last 10 transactions list
    -- Parameters:
    --   p_UserID: User identifier (required, must exist)
    --   p_PartID: Part identifier (required)
    --   p_Operation: Operation workflow step (required)
    --   p_Quantity: Transaction quantity (required, positive)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL sys_last_10_transactions_Add_Transaction('JKOLL', 'PART001', '90', 100, @status, @msg);
    
    DECLARE v_MaxPosition INT DEFAULT 0;
    DECLARE v_NewPosition INT DEFAULT 1;
    DECLARE v_RecordCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_last_10_transactions_Add_Transaction', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
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
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE sys_last_10_transactions_Add_Transaction;
    END IF;

    START TRANSACTION;
    
    -- Count current transactions for this user
    SELECT COUNT(*) INTO v_RecordCount
    FROM sys_last_10_transactions
    WHERE User = p_UserID;
    
    -- Get the next available position (max + 1, or 1 if no records)
    SELECT COALESCE(MAX(Position), 0) + 1 INTO v_NewPosition
    FROM sys_last_10_transactions
    WHERE User = p_UserID;
    
    -- If we already have 10 or more transactions, remove the oldest ones
    IF v_RecordCount >= 10 THEN
        -- Delete oldest records to keep only 9 most recent
        DELETE t1 FROM sys_last_10_transactions t1
        WHERE t1.User = p_UserID
        AND t1.ID NOT IN (
            SELECT * FROM (
                SELECT t2.ID 
                FROM sys_last_10_transactions t2
                WHERE t2.User = p_UserID
                ORDER BY t2.ReceiveDate DESC, t2.Position DESC
                LIMIT 9
            ) AS temp_table
        );
        
        -- Resequence positions for remaining records
        SET @row_number = 0;
        UPDATE sys_last_10_transactions 
        SET Position = (@row_number := @row_number + 1)
        WHERE User = p_UserID
        ORDER BY ReceiveDate DESC, Position DESC;
        
        SET v_NewPosition = 10;
    END IF;
    
    -- Insert the new transaction
    INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, Position, ReceiveDate)
    VALUES (p_UserID, p_PartID, p_Operation, p_Quantity, v_NewPosition, NOW());
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Transaction added successfully at position ', v_NewPosition);
END;;

DELIMITER ;

-- =====================================================
-- End of Quick Buttons Stored Procedures
-- =====================================================

-- These procedures are designed to work with the existing sys_last_10_transactions table:
-- 
-- ✅ SCHEMA COMPATIBILITY:
-- - Uses actual table name: sys_last_10_transactions
-- - Uses actual column names: ID, User, PartID, Operation, Quantity, ReceiveDate, Position
-- - Handles missing columns gracefully (Notes, ItemType, BatchNumber)
-- - Uses proper data types: VARCHAR(100), VARCHAR(300), INT, DATETIME
-- 
-- ✅ OUT PARAMETER HANDLING:
-- - All procedures use proper OUT parameter declarations
-- - Compatible with Helper_Database_StoredProcedure.ExecuteWithStatus()
-- - Includes comprehensive error handling with EXIT HANDLER FOR SQLEXCEPTION
-- 
-- ✅ MYSQL SYNTAX FIXES:
-- - Fixed ROW_NUMBER() window function usage with user variable approach
-- - Fixed subquery syntax in DELETE statements for MySQL compatibility
-- - Proper CASE statement usage instead of complex COALESCE operations
-- 
-- ✅ BUSINESS LOGIC:
-- - Position-based quick button management (1-10)
-- - Automatic cleanup when exceeding 10 transactions
-- - User validation against usr_users table
-- - Comprehensive input validation
-- 
-- Ready for deployment to MTM WIP Application database.
