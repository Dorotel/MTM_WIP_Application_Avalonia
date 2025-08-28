DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Add_Item`(
    IN p_PartID VARCHAR(100),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(200),
    IN p_User VARCHAR(100),
    IN p_Notes VARCHAR(1000)
)
BEGIN
    DECLARE nextBatch BIGINT;
    DECLARE batchStr VARCHAR(10);

    -- Get the next batch number
    SELECT last_batch_number INTO nextBatch FROM inv_inventory_batch_seq FOR UPDATE;
    SET nextBatch = nextBatch + 1;
    SET batchStr = LPAD(nextBatch, 10, '0');

    -- Update the sequence table
    UPDATE inv_inventory_batch_seq SET last_batch_number = nextBatch;

    -- Insert into inv_inventory
    INSERT INTO inv_inventory
        (PartID, Location, Operation, Quantity, ItemType, User, BatchNumber, Notes)
    VALUES
        (p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, p_User, batchStr, p_Notes);

    -- Insert into inv_transaction
    INSERT INTO inv_transaction
        (
            TransactionType, 
            BatchNumber, 
            PartID, 
            FromLocation, 
            ToLocation, 
            Operation, 
            Quantity, 
            Notes, 
            User, 
            ItemType
        )
    VALUES
        (
            'IN', 
            batchStr, 
            p_PartID, 
            p_Location, 
            NULL, 
            p_Operation, 
            p_Quantity, 
            p_Notes, 
            p_User, 
            p_ItemType
        );
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartID`(IN `p_PartID` VARCHAR(300))
BEGIN
SELECT 
    ID,
    PartID,
    Location,
    Operation,
    Quantity,
    ItemType,
    ReceiveDate,
    LastUpdated,
    User,
    BatchNumber AS `BatchNumber`,
    Notes
FROM inv_inventory
    WHERE PartID = p_PartID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartIDandOperation`(IN `p_PartID` VARCHAR(300), IN `o_Operation` VARCHAR(300))
BEGIN
SELECT 
    ID,
    PartID,
    Location,
    Operation,
    Quantity,
    ItemType,
    ReceiveDate,
    LastUpdated,
    User,
    BatchNumber AS `BatchNumber`,
    Notes
FROM inv_inventory
    WHERE PartID = p_PartID AND Operation = o_Operation;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByUser`(IN `p_User` VARCHAR(100))
BEGIN
    SELECT * FROM inv_inventory
    WHERE User = p_User
    ORDER BY LastUpdated DESC;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Remove_Item`(
    IN p_PartID VARCHAR(300),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(100),
    IN p_User VARCHAR(100),
    IN p_BatchNumber VARCHAR(100),
    IN p_Notes VARCHAR(1000),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_RowsAffected INT DEFAULT 0;
    DECLARE v_ErrorMessage TEXT DEFAULT '';
    DECLARE v_RecordCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_ErrorMessage = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error occurred while removing inventory item for part: ', p_PartID);
        ROLLBACK;
    END;
    
    START TRANSACTION;
    
    IF p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be greater than zero';
        ROLLBACK;
    ELSE
        SELECT COUNT(*) INTO v_RecordCount FROM inv_inventory WHERE PartID = p_PartID AND Location = p_Location AND Operation = p_Operation;
          
        IF v_RecordCount = 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = CONCAT('No inventory records found for PartID: ', p_PartID, ', Location: ', p_Location, ', Operation: ', p_Operation);
        ELSE
            DELETE FROM inv_inventory WHERE PartID = p_PartID AND Location = p_Location AND Operation = p_Operation AND Quantity = p_Quantity
                AND (p_BatchNumber IS NULL OR p_BatchNumber = '' OR BatchNumber = p_BatchNumber)
                AND (p_Notes IS NULL OR p_Notes = '' OR Notes IS NULL OR Notes = '' OR Notes = p_Notes) LIMIT 1;
            
            SET v_RowsAffected = ROW_COUNT();
            
            IF v_RowsAffected = 0 THEN
                DELETE FROM inv_inventory WHERE PartID = p_PartID AND Location = p_Location AND Operation = p_Operation AND Quantity = p_Quantity LIMIT 1;
                SET v_RowsAffected = ROW_COUNT();
            END IF;
            
            IF v_RowsAffected > 0 THEN
                INSERT INTO inv_transaction (TransactionType, PartID, FromLocation, Operation, Quantity, ItemType, User, BatchNumber, Notes, ReceiveDate)
                VALUES ('OUT', p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, p_User, p_BatchNumber, p_Notes, NOW());
                
                SET p_Status = 0;
                SET p_ErrorMsg = CONCAT('Inventory item removed successfully for part: ', p_PartID, ', quantity: ', p_Quantity);
            ELSE
                SET p_Status = 1;
                SET p_ErrorMsg = CONCAT('No matching inventory item found for removal. Found ', v_RecordCount, ' records for PartID: ', p_PartID, ', Location: ', p_Location, ', Operation: ', p_Operation, ' but none matched Quantity: ', p_Quantity);
            END IF;
        END IF;
        COMMIT;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Transfer_Part`(IN `in_BatchNumber` VARCHAR(300), IN `in_PartID` VARCHAR(300), IN `in_Operation` VARCHAR(100), IN `in_NewLocation` VARCHAR(100))
BEGIN
    -- Validate that the record exists
    IF EXISTS (
        SELECT 1 FROM inv_inventory
        WHERE BatchNumber = in_BatchNumber
          AND PartID = in_PartID
          AND Operation = in_Operation
    ) THEN
        -- Update the location
        UPDATE inv_inventory
        SET Location = in_NewLocation,
            LastUpdated = CURRENT_TIMESTAMP
        WHERE BatchNumber = in_BatchNumber
          AND PartID = in_PartID
          AND Operation = in_Operation;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Transfer_Quantity`(IN `in_BatchNumber` VARCHAR(255), IN `in_PartID` VARCHAR(255), IN `in_Operation` VARCHAR(255), IN `in_TransferQuantity` INT, IN `in_OriginalQuantity` INT, IN `in_NewLocation` VARCHAR(255), IN `in_User` VARCHAR(255))
BEGIN
    -- Check if transfer quantity is valid
    IF in_TransferQuantity <= 0 OR in_TransferQuantity > in_OriginalQuantity THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Invalid transfer quantity';
    END IF;

    -- Subtract the transfer quantity from the original inventory record and update User
    UPDATE inv_inventory
    SET Quantity = Quantity - in_TransferQuantity,
        User = in_User
    WHERE BatchNumber = in_BatchNumber
      AND PartID = in_PartID
      AND Operation = in_Operation
      AND Quantity = in_OriginalQuantity;

    -- Insert a new record for the transferred quantity at the new location with User
    INSERT INTO inv_inventory (BatchNumber, PartID, Operation, Quantity, Location, User)
    VALUES (in_BatchNumber, in_PartID, in_Operation, in_TransferQuantity, in_NewLocation, in_User);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_changelog_Get_Current`()
BEGIN
    SELECT *
    FROM log_changelog
    ORDER BY Version DESC
    LIMIT 1;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Add_ItemType`(IN `p_ItemType` VARCHAR(100), IN `p_IssuedBy` VARCHAR(100))
BEGIN
    INSERT INTO `md_item_types` (`ItemType`, `IssuedBy`)
    VALUES (p_ItemType, p_IssuedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Delete_ByID`(IN `p_ID` INT)
BEGIN
    DELETE FROM `md_item_types`
    WHERE `ID` = p_ID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Delete_ByType`(IN `p_ItemType` VARCHAR(100))
BEGIN
    DELETE FROM `md_item_types`
    WHERE `ItemType` = p_ItemType;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Get_All`()
BEGIN
    SELECT * FROM `md_item_types`;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Update_ItemType`(IN `p_ID` INT, IN `p_ItemType` VARCHAR(100), IN `p_IssuedBy` VARCHAR(100))
BEGIN
    UPDATE `md_item_types`
    SET `ItemType` = p_ItemType,
        `IssuedBy` = p_IssuedBy
    WHERE `ID` = p_ID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Add_Location`(IN `p_Location` VARCHAR(100), IN `p_IssuedBy` VARCHAR(100), IN `p_Building` VARCHAR(100))
BEGIN
    INSERT INTO `md_locations` (`Location`, `Building` , `IssuedBy`)
    VALUES (p_Location, p_Building ,p_IssuedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Delete_ByLocation`(IN `p_Location` VARCHAR(100))
BEGIN
    DELETE FROM `md_locations`
    WHERE `Location` = p_Location;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Get_All`()
BEGIN
    SELECT * FROM `md_locations`;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Update_Location`(IN `p_OldLocation` VARCHAR(100), IN `p_Location` VARCHAR(100), IN `p_IssuedBy` VARCHAR(100), IN `p_Building` VARCHAR(100))
BEGIN
    UPDATE `md_locations`
    SET `Location` = p_Location,
    	`Building` = p_Building,
        `IssuedBy` = p_IssuedBy
    WHERE `Location` = p_OldLocation;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Add_Operation`(IN `p_Operation` VARCHAR(100), IN `p_IssuedBy` VARCHAR(100))
BEGIN
    INSERT INTO `md_operation_numbers` (`Operation`, `IssuedBy`)
    VALUES (p_Operation, p_IssuedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Delete_ByOperation`(IN `p_Operation` VARCHAR(100))
BEGIN
    DELETE FROM `md_operation_numbers`
    WHERE `Operation` = p_Operation;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Get_All`()
BEGIN
    SELECT * FROM `md_operation_numbers`;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Update_Operation`(IN `p_Operation` VARCHAR(100), IN `p_NewOperation` VARCHAR(100), IN `p_IssuedBy` VARCHAR(100))
BEGIN
    UPDATE `md_operation_numbers`
    SET `Operation` = p_NewOperation,
        `IssuedBy` = p_IssuedBy
    WHERE `Operation` = p_Operation;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Add_Part`(IN `p_ItemNumber` VARCHAR(300), IN `p_Customer` VARCHAR(300), IN `p_Description` VARCHAR(300), IN `p_IssuedBy` VARCHAR(100), IN `p_ItemType` VARCHAR(100))
BEGIN
    INSERT INTO `md_part_ids` (`PartID`, `Customer`, `Description`, `IssuedBy`, `ItemType`)
    VALUES (p_ItemNumber, p_Customer, p_Description, p_IssuedBy, p_ItemType);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Delete_ByItemNumber`(IN `p_ItemNumber` VARCHAR(300))
BEGIN
    DELETE FROM `md_part_ids`
    WHERE `PartID` = p_ItemNumber;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Get_All`()
BEGIN
    SELECT * FROM `md_part_ids`;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Get_ByItemNumber`(IN `p_ItemNumber` VARCHAR(300))
BEGIN
    SELECT * FROM `md_part_ids`
    WHERE `PartID` = p_ItemNumber;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Update_Part`(IN `p_ID` INT, IN `p_ItemNumber` VARCHAR(300), IN `p_Customer` VARCHAR(300), IN `p_Description` VARCHAR(300), IN `p_IssuedBy` VARCHAR(100), IN `p_ItemType` VARCHAR(100))
BEGIN
    UPDATE `md_part_ids`
    SET `PartID` = p_ItemNumber,
        `Customer` = p_Customer,
        `Description` = p_Description,
        `IssuedBy` = p_IssuedBy,
        `ItemType` = p_ItemType
    WHERE `ID` = p_ID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Clear_ByUser`(
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
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Get_ByUser`(
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
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Remove`(
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
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Save`(
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
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Add_Transaction`(
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
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Get_ByUser`(
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
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_roles_Get_ById`(IN `p_ID` INT)
BEGIN
    SELECT * FROM sys_roles WHERE ID = p_ID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_Validate`(
    IN p_UserID VARCHAR(100),
    IN p_ValidatingUserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
sys_user_Validate: BEGIN
    -- Validate that a user exists and is active
    -- Purpose: Verify user exists and is available for system operations
    -- Parameters:
    --   p_UserID: User identifier to validate (required)
    --   p_ValidatingUserID: User performing validation (required for audit)
    --   p_Status: 0=Valid, 1=Invalid, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL sys_user_Validate('testuser', 'admin', @status, @msg);
    
    DECLARE v_UserCount INT DEFAULT 0;
    DECLARE v_FullName VARCHAR(255);
    DECLARE v_Role VARCHAR(50);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_ValidatingUserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_user_Validate', 'Database', CONCAT('UserID: ', COALESCE(p_UserID, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sys_user_Validate;
    END IF;

    IF p_ValidatingUserID IS NULL OR p_ValidatingUserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ValidatingUserID is required and cannot be empty';
        LEAVE sys_user_Validate;
    END IF;

    -- Check if validating user exists
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_ValidatingUserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Validating User ID does not exist: ', p_ValidatingUserID);
        LEAVE sys_user_Validate;
    END IF;

    SET @current_user_id = p_ValidatingUserID;

    -- Check if target user exists and is active
    SELECT COUNT(*), MAX(full_name), MAX(role)
    INTO v_UserCount, v_FullName, v_Role
    FROM usr_users 
    WHERE User = p_UserID;

    IF v_UserCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('User is valid: ', p_UserID, ' - ', v_FullName, ' (', v_Role, ')');
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User does not exist or is inactive: ', p_UserID);
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_roles_Add`(IN `p_UserID` INT, IN `p_RoleID` INT, IN `p_AssignedBy` VARCHAR(100))
BEGIN
    INSERT INTO sys_user_roles (UserID, RoleID, AssignedBy)
    VALUES (p_UserID, p_RoleID, p_AssignedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_roles_Delete`(IN `p_UserID` INT, IN `p_RoleID` INT)
BEGIN
    DELETE FROM sys_user_roles
    WHERE UserID = p_UserID AND RoleID = p_RoleID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_roles_Update`(IN `p_UserID` INT, IN `p_NewRoleID` INT, IN `p_AssignedBy` VARCHAR(100))
BEGIN
    -- Remove all roles for the user
    DELETE FROM sys_user_roles WHERE UserID = p_UserID;

    -- Add the new role
    INSERT INTO sys_user_roles (UserID, RoleID, AssignedBy)
    VALUES (p_UserID, p_NewRoleID, p_AssignedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_Get`(IN `p_UserId` VARCHAR(64), OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Database error occurred';
    END;

    SELECT SettingsJson
    FROM usr_ui_settings
    WHERE UserId = p_UserId;

    SET p_Status = 0;
    SET p_ErrorMsg = NULL;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_GetShortcutsJson`(IN `p_UserId` VARCHAR(255), OUT `p_ShortcutsJson` JSON)
BEGIN
    SELECT ShortcutsJson INTO p_ShortcutsJson
    FROM usr_ui_settings
    WHERE UserId = p_UserId
    LIMIT 1;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetJsonSetting`(
    IN p_UserId VARCHAR(64),
    IN p_DgvName VARCHAR(128),
    IN p_SettingJson JSON,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE existing INT DEFAULT 0;
    DECLARE currentJson JSON;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    -- Check if a row exists for this user
    SELECT COUNT(*) INTO existing
    FROM usr_ui_settings
    WHERE UserId = p_UserId;

    IF existing = 0 THEN
        -- Insert new row with this DGV branch
        INSERT INTO usr_ui_settings (UserId, SettingsJson)
        VALUES (p_UserId, JSON_OBJECT(p_DgvName, p_SettingJson));
    ELSE
        -- Update only the DGV branch in SettingsJson
        SELECT SettingsJson INTO currentJson FROM usr_ui_settings WHERE UserId = p_UserId LIMIT 1;
        SET currentJson = JSON_SET(IFNULL(currentJson, JSON_OBJECT()), CONCAT('$.', p_DgvName), p_SettingJson);
        UPDATE usr_ui_settings SET SettingsJson = currentJson, UpdatedAt = NOW() WHERE UserId = p_UserId;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetShortcutsJson`(IN `p_UserId` VARCHAR(255), IN `p_ShortcutsJson` JSON, OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Database error while saving shortcuts JSON.';
        ROLLBACK;
    END;

    START TRANSACTION;

    UPDATE usr_ui_settings
    SET ShortcutsJson = p_ShortcutsJson
    WHERE UserId = p_UserId;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetThemeJson`(IN `p_UserId` VARCHAR(64), IN `p_ThemeJson` JSON, OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
main_block: BEGIN
    DECLARE v_sqlstate CHAR(5) DEFAULT '';
    DECLARE v_message TEXT DEFAULT '';
    DECLARE v_exists INT DEFAULT 0;
    DECLARE v_settingsJson JSON DEFAULT NULL;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN

        GET DIAGNOSTICS CONDITION 1 v_sqlstate = RETURNED_SQLSTATE, v_message = MESSAGE_TEXT;
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Database error [', v_sqlstate, ']: ', v_message);
        ROLLBACK;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_exists FROM usr_ui_settings WHERE UserId = p_UserId;
    IF v_exists = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'User does not exist in usr_ui_settings.';
        ROLLBACK;
        LEAVE main_block;
    END IF;

    SELECT SettingsJson INTO v_settingsJson FROM usr_ui_settings WHERE UserId = p_UserId FOR UPDATE;

    IF v_settingsJson IS NULL THEN
        SET v_settingsJson = p_ThemeJson;
    ELSE
        SET v_settingsJson = JSON_MERGE_PATCH(v_settingsJson, p_ThemeJson);
    END IF;

    UPDATE usr_ui_settings
    SET SettingsJson = v_settingsJson
    WHERE UserId = p_UserId;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Add_User`(IN `p_User` VARCHAR(100), IN `p_FullName` VARCHAR(200), IN `p_Shift` VARCHAR(50), IN `p_VitsUser` TINYINT, IN `p_Pin` VARCHAR(50), IN `p_LastShownVersion` VARCHAR(50), IN `p_HideChangeLog` VARCHAR(50), IN `p_Theme_Name` VARCHAR(50), IN `p_Theme_FontSize` INT, IN `p_VisualUserName` VARCHAR(50), IN `p_VisualPassword` VARCHAR(50), IN `p_WipServerAddress` VARCHAR(15), IN `p_WipServerPort` VARCHAR(10), IN `p_WipDatabase` VARCHAR(100))
BEGIN
    -- Insert into application users table
    INSERT INTO usr_users (
        `User`, `Full Name`, `Shift`, `VitsUser`, `Pin`, `LastShownVersion`, `HideChangeLog`, 
        `Theme_Name`, `Theme_FontSize`, `VisualUserName`, `VisualPassword`, `WipServerAddress`, `WipDatabase`, `WipServerPort`
    ) VALUES (
        p_User, p_FullName, p_Shift, p_VitsUser, p_Pin, p_LastShownVersion, p_HideChangeLog, 
        p_Theme_Name, p_Theme_FontSize, p_VisualUserName, p_VisualPassword, p_WipServerAddress, p_WipDatabase, p_WipServerPort
    );

    -- Create a MySQL user IF NOT EXISTS, with NO password
    SET @createUserQuery := CONCAT(
        'CREATE USER IF NOT EXISTS \'', REPLACE(p_User, '\'', '\\\''), 
        '\'@\'%\''
    );
    PREPARE stmt FROM @createUserQuery;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;

    -- Grant ALL PRIVILEGES to the user on the database
    SET @grantAllQuery := CONCAT(
        'GRANT ALL PRIVILEGES ON *.* TO \'', REPLACE(p_User, '\'', '\\\''), '\'@\'%\';'
    );
    PREPARE stmt FROM @grantAllQuery;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;

    -- Flush privileges to apply changes
    FLUSH PRIVILEGES;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Delete_User`(IN `p_User` VARCHAR(100))
BEGIN
    -- Remove MySQL user
    SET @d := CONCAT('DROP USER IF EXISTS \'', REPLACE(p_User, '\'', '\\\''), '\'@\'%\';');
    PREPARE stmt FROM @d;
    EXECUTE stmt;
    DEALLOCATE PREPARE stmt;

    -- Remove from application users table
    DELETE FROM usr_users WHERE `User` = p_User;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Exists`(IN `p_User` VARCHAR(100))
BEGIN
    SELECT COUNT(*) AS UserExists FROM usr_users WHERE `User` = p_User;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Get_All`()
BEGIN
    SELECT * FROM usr_users;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Get_ByUser`(IN `p_User` VARCHAR(100))
BEGIN
    SELECT * FROM usr_users WHERE `User` = p_User;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Update_User`(IN `p_User` VARCHAR(100), IN `p_FullName` VARCHAR(200), IN `p_Shift` VARCHAR(50), IN `p_Pin` VARCHAR(50), IN `p_VisualUserName` VARCHAR(50), IN `p_VisualPassword` VARCHAR(50))
BEGIN
    UPDATE usr_users SET
        `Full Name` = p_FullName,
        `Shift` = p_Shift,
        `Pin` = p_Pin,
        `VisualUserName` = p_VisualUserName,
        `VisualPassword` = p_VisualPassword
    WHERE `User` = p_User;
END$$
DELIMITER ;
