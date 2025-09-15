DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Add_Item`(
    IN p_PartID VARCHAR(300),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(100),
    IN p_User VARCHAR(100),
    IN p_Notes VARCHAR(1000),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_BatchNumber VARCHAR(100);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    -- Get the next batch number
    SELECT LPAD(COALESCE(MAX(CAST(last_batch_number AS UNSIGNED)), 0) + 1, 10, '0') 
    INTO v_BatchNumber 
    FROM inv_inventory_batch_seq;

    -- Update the sequence table
    UPDATE inv_inventory_batch_seq SET last_batch_number = CAST(v_BatchNumber AS UNSIGNED);

    -- Insert into inv_inventory
    INSERT INTO inv_inventory (
        PartID, Location, Operation, Quantity, ItemType, ReceiveDate, User, Notes, BatchNumber
    ) VALUES (
        p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, NOW(), p_User, p_Notes, v_BatchNumber
    );

    -- Insert into inv_transaction
    INSERT INTO inv_transaction (
        TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
        Quantity, ItemType, ReceiveDate, User, Notes
    ) VALUES (
        'IN', v_BatchNumber, p_PartID, p_Location, NULL, p_Operation, 
        p_Quantity, p_ItemType, NOW(), p_User, p_Notes
    );

    SET p_Status = 1;
    SET p_ErrorMsg = 'Item added successfully';
    COMMIT;
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
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartIDandOperation`(IN `p_PartID` VARCHAR(300), IN `p_Operation` VARCHAR(300))
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
    WHERE PartID = p_PartID AND Operation = p_Operation;
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
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Update_Notes`(
    IN p_ID INT,
    IN p_PartID VARCHAR(300),
    IN p_BatchNumber VARCHAR(100),
    IN p_Notes VARCHAR(1000),
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_RowsAffected INT DEFAULT 0;
    DECLARE v_OldNotes VARCHAR(1000) DEFAULT '';
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    -- Validate that the record exists and get current notes
    SELECT Notes INTO v_OldNotes
    FROM inv_inventory
    WHERE ID = p_ID 
      AND PartID = p_PartID 
      AND BatchNumber = p_BatchNumber;

    -- Check if record was found
    IF ROW_COUNT() = 0 THEN
        SET p_Status = -2;
        SET p_ErrorMsg = 'Inventory record not found or parameters do not match';
        ROLLBACK;
    ELSE
        -- Update the notes and user who made the change
        UPDATE inv_inventory 
        SET Notes = p_Notes,
            User = p_User,
            LastUpdated = NOW()
        WHERE ID = p_ID 
          AND PartID = p_PartID 
          AND BatchNumber = p_BatchNumber;
        
        SET v_RowsAffected = ROW_COUNT();
        
        IF v_RowsAffected = 0 THEN
            SET p_Status = -3;
            SET p_ErrorMsg = 'Failed to update inventory notes';
            ROLLBACK;
        ELSE
            -- Log the note change in transaction history
            INSERT INTO inv_transaction (
                TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
                Quantity, ItemType, ReceiveDate, User, Notes
            )
            SELECT 
                'NOTE_EDIT', 
                BatchNumber, 
                PartID, 
                Location, 
                NULL, 
                Operation,
                0, -- Zero quantity for note edit
                ItemType, 
                NOW(), 
                p_User, 
                CONCAT('Note changed from: "', COALESCE(v_OldNotes, ''), '" to: "', COALESCE(p_Notes, ''), '"')
            FROM inv_inventory 
            WHERE ID = p_ID;
            
            SET p_Status = 1;
            SET p_ErrorMsg = 'Notes updated successfully';
            COMMIT;
        END IF;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByID`(IN `p_ID` INT)
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
    WHERE ID = p_ID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Remove_Item`(IN `p_PartID` VARCHAR(300), IN `p_Location` VARCHAR(100), IN `p_Operation` VARCHAR(100), IN `p_Quantity` INT, IN `p_ItemType` VARCHAR(100), IN `p_User` VARCHAR(100), IN `p_BatchNumber` VARCHAR(100), IN `p_Notes` VARCHAR(1000), OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
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
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    DELETE FROM qb_quickbuttons WHERE User = p_User;

    SET p_Status = 0;
    SET p_ErrorMsg = 'All quick buttons cleared successfully';
    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`%` PROCEDURE `qb_quickbuttons_Get_ByUser`(
    IN p_User VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Database error occurred while retrieving quick buttons';
        ROLLBACK;
    END;
    
    
    SET p_Status = -1;
    SET p_ErrorMsg = '';
    
    START TRANSACTION;
    
    
    IF p_User IS NULL OR TRIM(p_User) = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'User parameter is required';
        ROLLBACK;
    ELSE
        
        SELECT 
            ID,
            User,
            Position,
            PartID,
            Operation,
            Quantity,
            Location,
            ItemType,
            DateCreated,
            DateModified
        FROM qb_quickbuttons 
        WHERE User = p_User 
        ORDER BY Position;
        
        
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Retrieved ', ROW_COUNT(), ' quick buttons successfully');
        
        COMMIT;
    END IF;
    
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Remove`(
    IN p_User VARCHAR(100),
    IN p_Position INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    DELETE FROM qb_quickbuttons WHERE User = p_User AND Position = p_Position;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quick button removed successfully';
    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Save`(
    IN p_User VARCHAR(100),
    IN p_Position INT,
    IN p_PartID VARCHAR(300),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    INSERT INTO qb_quickbuttons (User, Position, PartID, Location, Operation, Quantity, ItemType, DateCreated)
    VALUES (p_User, p_Position, p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, NOW())
    ON DUPLICATE KEY UPDATE
        PartID = VALUES(PartID),
        Location = VALUES(Location),
        Operation = VALUES(Operation),
        Quantity = VALUES(Quantity),
        ItemType = VALUES(ItemType),
        DateModified = NOW();

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quick button saved successfully';
    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Add_Transaction`(
    IN p_TransactionType ENUM('IN','OUT','TRANSFER'),
    IN p_BatchNumber VARCHAR(100),
    IN p_PartID VARCHAR(300),
    IN p_FromLocation VARCHAR(300),
    IN p_ToLocation VARCHAR(100),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_Notes VARCHAR(1000),
    IN p_User VARCHAR(100),
    IN p_ItemType VARCHAR(100),
    IN p_ReceiveDate DATETIME,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    INSERT INTO inv_transaction (
        TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
        Quantity, ItemType, ReceiveDate, User, Notes
    ) VALUES (
        p_TransactionType, p_BatchNumber, p_PartID, p_FromLocation, p_ToLocation, p_Operation, 
        p_Quantity, p_ItemType, p_ReceiveDate, p_User, p_Notes
    );

    SET p_Status = 0;
    SET p_ErrorMsg = 'Transaction added successfully';
    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Get_ByUser`(
    IN p_User VARCHAR(100),
    IN p_Limit INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        RESIGNAL;
    END;

    SELECT 
        TransactionType,
        BatchNumber,
        PartID,
        FromLocation,
        ToLocation,
        Operation,
        Quantity,
        ItemType,
        ReceiveDate,
        User,
        Notes
    FROM inv_transaction 
    WHERE User = p_User 
    ORDER BY ReceiveDate DESC 
    LIMIT p_Limit;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_roles_Get_ById`(IN `p_ID` INT)
BEGIN
    SELECT * FROM sys_roles WHERE ID = p_ID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_Validate`(IN `p_User` VARCHAR(100), IN `p_ValidatingUserID` VARCHAR(100), OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
sys_user_Validate:BEGIN
    -- Validate that a user exists and is active
    -- Purpose: Verify user exists and is available for system operations
    -- Parameters:
    --   p_User: User identifier to validate (required)
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
        VALUES (p_ValidatingUserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'sys_user_Validate', 'Database', CONCAT('UserID: ', COALESCE(p_User, 'NULL')));
    END;

    -- Input validation
    IF p_User IS NULL OR p_User = '' THEN
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
    WHERE User = p_User;

    IF v_UserCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('User is valid: ', p_User, ' - ', v_FullName, ' (', v_Role, ')');
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User does not exist or is inactive: ', p_User);
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_roles_Add`(IN `p_User` INT, IN `p_RoleID` INT, IN `p_AssignedBy` VARCHAR(100))
BEGIN
    INSERT INTO sys_user_roles (UserID, RoleID, AssignedBy)
    VALUES (p_User, p_RoleID, p_AssignedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_roles_Delete`(IN `p_User` INT, IN `p_RoleID` INT)
BEGIN
    DELETE FROM sys_user_roles
    WHERE UserID = p_User AND RoleID = p_RoleID;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_user_roles_Update`(IN `p_User` INT, IN `p_NewRoleID` INT, IN `p_AssignedBy` VARCHAR(100))
BEGIN
    -- Remove all roles for the user
    DELETE FROM sys_user_roles WHERE UserID = p_User;

    -- Add the new role
    INSERT INTO sys_user_roles (UserID, RoleID, AssignedBy)
    VALUES (p_User, p_NewRoleID, p_AssignedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_Get`(IN `p_User` VARCHAR(64), OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Database error occurred';
    END;

    SELECT SettingsJson
    FROM usr_ui_settings
    WHERE UserId = p_User;

    SET p_Status = 0;
    SET p_ErrorMsg = NULL;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_GetShortcutsJson`(IN `p_User` VARCHAR(255), OUT `p_ShortcutsJson` JSON)
BEGIN
    SELECT ShortcutsJson INTO p_ShortcutsJson
    FROM usr_ui_settings
    WHERE UserId = p_User
    LIMIT 1;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetJsonSetting`(IN `p_User` VARCHAR(64), IN `p_DgvName` VARCHAR(128), IN `p_SettingJson` JSON, OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
BEGIN
    DECLARE existing INT DEFAULT 0;
    DECLARE currentJson JSON;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    -- Check if a row exists for this user
    SELECT COUNT(*) INTO existing
    FROM usr_ui_settings
    WHERE UserId = p_User;

    IF existing = 0 THEN
        -- Insert new row with this DGV branch
        INSERT INTO usr_ui_settings (UserId, SettingsJson)
        VALUES (p_User, JSON_OBJECT(p_DgvName, p_SettingJson));
    ELSE
        -- Update only the DGV branch in SettingsJson
        SELECT SettingsJson INTO currentJson FROM usr_ui_settings WHERE UserId = p_User LIMIT 1;
        SET currentJson = JSON_SET(IFNULL(currentJson, JSON_OBJECT()), CONCAT('$.', p_DgvName), p_SettingJson);
        UPDATE usr_ui_settings SET SettingsJson = currentJson, UpdatedAt = NOW() WHERE UserId = p_User;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetShortcutsJson`(IN `p_User` VARCHAR(255), IN `p_ShortcutsJson` JSON, OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
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
    WHERE UserId = p_User;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetThemeJson`(IN `p_User` VARCHAR(64), IN `p_ThemeJson` JSON, OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
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

    SELECT COUNT(*) INTO v_exists FROM usr_ui_settings WHERE UserId = p_User;
    IF v_exists = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'User does not exist in usr_ui_settings.';
        ROLLBACK;
        LEAVE main_block;
    END IF;

    SELECT SettingsJson INTO v_settingsJson FROM usr_ui_settings WHERE UserId = p_User FOR UPDATE;

    IF v_settingsJson IS NULL THEN
        SET v_settingsJson = p_ThemeJson;
    ELSE
        SET v_settingsJson = JSON_MERGE_PATCH(v_settingsJson, p_ThemeJson);
    END IF;

    UPDATE usr_ui_settings
    SET SettingsJson = v_settingsJson
    WHERE UserId = p_User;

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
