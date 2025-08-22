DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `assign_BatchNumber_Step0`()
BEGIN
    UPDATE inv_inventory SET BatchNumber = 'MIGRATED';
    UPDATE inv_transaction SET BatchNumber = 'MIGRATED';
    UPDATE inv_inventory_batch_seq SET last_batch_number = '0' WHERE 1;
    UPDATE inv_inventory_batch_seq SET current_match = '0' WHERE 1;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `assign_BatchNumber_Step1`()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE inv_id INT;
    DECLARE batch_num INT DEFAULT 0;
    DECLARE inv_part VARCHAR(100);
    DECLARE inv_loc VARCHAR(100);
    DECLARE inv_op VARCHAR(100);
    DECLARE inv_qty INT;
    DECLARE inv_itemtype VARCHAR(100);
    DECLARE inv_recv DATETIME;
    DECLARE inv_user VARCHAR(100);
    DECLARE inv_notes VARCHAR(1000);

    DECLARE inv_cursor CURSOR FOR
        SELECT ID, PartID, Location, Operation, Quantity, ItemType, ReceiveDate, User, Notes
        FROM inv_inventory
        WHERE BatchNumber = 'MIGRATED'
        ORDER BY ID ASC;

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    OPEN inv_cursor;
    inv_loop: LOOP
        FETCH inv_cursor INTO inv_id, inv_part, inv_loc, inv_op, inv_qty, inv_itemtype, inv_recv, inv_user, inv_notes;
        IF done THEN
            LEAVE inv_loop;
        END IF;

        SET batch_num = batch_num + 1;

        UPDATE inv_inventory
            SET BatchNumber = LPAD(batch_num, 10, '0')
            WHERE ID = inv_id;

        -- Remove any existing transaction that matches what is about to be inserted
        DELETE FROM inv_transaction
        WHERE
            TransactionType = 'IN'
            AND BatchNumber = LPAD(batch_num, 10, '0')
            AND PartID = inv_part
            AND FromLocation = inv_loc
            AND ToLocation IS NULL
            AND Operation = inv_op
            AND Quantity = inv_qty
            AND ItemType = inv_itemtype
            AND ReceiveDate = inv_recv
            AND User = inv_user
            AND Notes = inv_notes;

        -- Now insert the new transaction
        INSERT INTO inv_transaction
            (TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, Quantity, ItemType, ReceiveDate, User, Notes)
        VALUES
            ('IN', LPAD(batch_num, 10, '0'), inv_part, inv_loc, NULL, inv_op, inv_qty, inv_itemtype, inv_recv, inv_user, inv_notes);

        UPDATE inv_inventory_batch_seq SET last_batch_number = batch_num WHERE 1 LIMIT 1;
    END LOOP;
    CLOSE inv_cursor;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `assign_BatchNumber_Step2`()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE txn_id INT;
    DECLARE batch_num INT;

    -- Declare cursor and handler FIRST, before any executable statements!
    DECLARE txn_cursor CURSOR FOR
        SELECT id FROM inv_transaction
        WHERE TransactionType = 'IN' AND BatchNumber = 'MIGRATED'
        ORDER BY id ASC;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    -- Now you can initialize batch_num
    SELECT last_batch_number INTO batch_num FROM inv_inventory_batch_seq LIMIT 1;

    OPEN txn_cursor;
    read_loop: LOOP
        FETCH txn_cursor INTO txn_id;
        IF done THEN
            LEAVE read_loop;
        END IF;

        SET batch_num = batch_num + 1;
        UPDATE inv_transaction SET BatchNumber = LPAD(batch_num, 10, '0') WHERE id = txn_id;
        UPDATE inv_inventory_batch_seq SET last_batch_number = batch_num WHERE 1 LIMIT 1;
    END LOOP;
    CLOSE txn_cursor;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `assign_BatchNumber_Step3`()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE loop_counter INT DEFAULT 0;
    DECLARE in_id INT;
    DECLARE in_part VARCHAR(100);
    DECLARE in_loc VARCHAR(100);
    DECLARE in_batch VARCHAR(100);
    DECLARE in_date DATETIME;
    DECLARE out_id INT;
    DECLARE out_part VARCHAR(100);
    DECLARE out_loc VARCHAR(100);
    DECLARE out_batch VARCHAR(100);
    DECLARE updated_rows INT DEFAULT 0;
    DECLARE start_id INT DEFAULT 0;

    -- Cursor declaration
    DECLARE in_cursor CURSOR FOR
        SELECT id, PartID, FromLocation, BatchNumber, ReceiveDate
        FROM inv_transaction
        WHERE TransactionType = 'IN'
          AND BatchNumber IS NOT NULL
          AND BatchNumber <> 'MIGRATED'
          AND id > start_id
        ORDER BY id ASC, BatchNumber ASC;

    -- Handler declaration

    -- Get the last processed id
    SELECT current_match INTO start_id FROM inv_inventory_batch_seq LIMIT 1;

    OPEN in_cursor;
    match_loop: LOOP
        IF loop_counter >= 1000 THEN
            LEAVE match_loop;
        END IF;

        FETCH in_cursor INTO in_id, in_part, in_loc, in_batch, in_date;
        IF done THEN
            LEAVE match_loop;
        END IF;

        -- Update current_match pointer
        UPDATE inv_inventory_batch_seq SET current_match = in_id;

        SET loop_counter = loop_counter + 1;

        SELECT id, PartID, FromLocation, BatchNumber
          INTO out_id, out_part, out_loc, out_batch
        FROM inv_transaction
        WHERE TransactionType = 'OUT'
          AND FromLocation = in_loc
          AND PartID = in_part
          AND BatchNumber = 'MIGRATED'
        ORDER BY ReceiveDate ASC
        LIMIT 1;

        IF out_id IS NOT NULL THEN
            UPDATE inv_transaction
            SET BatchNumber = in_batch
            WHERE id = out_id;
            SET updated_rows = updated_rows + ROW_COUNT();
        END IF;
    END LOOP;
    CLOSE in_cursor;

    SELECT loop_counter AS total_loops, updated_rows AS total_updated_rows;

    IF loop_counter >= 1000 THEN
        SELECT 'Looped 1000 rows, please run the procedure again to continue.' AS message;
    ELSE
        SELECT 'Processing complete.' AS message;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `assign_BatchNumber_Step4`()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE loop_counter INT DEFAULT 0;
    DECLARE in_id INT;
    DECLARE in_part VARCHAR(100);
    DECLARE in_loc VARCHAR(100);
    DECLARE in_batch VARCHAR(100);
    DECLARE in_date DATETIME;
    DECLARE out_id INT;
    DECLARE out_part VARCHAR(100);
    DECLARE out_loc VARCHAR(100);
    DECLARE out_batch VARCHAR(100);
    DECLARE updated_rows INT DEFAULT 0;
    DECLARE start_id INT DEFAULT 0;

    -- Cursor declaration
    DECLARE in_cursor CURSOR FOR
        SELECT id, PartID, FromLocation, BatchNumber, ReceiveDate
        FROM inv_transaction
        WHERE TransactionType = 'IN'
          AND BatchNumber IS NOT NULL
          AND BatchNumber <> 'MIGRATED'
          AND id > start_id
        ORDER BY id ASC, BatchNumber ASC;

    -- Handler declaration

    UPDATE inv_inventory_batch_seq SET current_match = '0' WHERE 1;
    -- Get the last processed id
    SELECT current_match INTO start_id FROM inv_inventory_batch_seq LIMIT 1;

    OPEN in_cursor;
    match_loop: LOOP
        IF loop_counter >= 1000 THEN
            LEAVE match_loop;
        END IF;

        FETCH in_cursor INTO in_id, in_part, in_loc, in_batch, in_date;
        IF done THEN
            LEAVE match_loop;
        END IF;

        -- Update current_match pointer
        UPDATE inv_inventory_batch_seq SET current_match = in_id;

        SET loop_counter = loop_counter + 1;

        SELECT id, PartID, FromLocation, BatchNumber
          INTO out_id, out_part, out_loc, out_batch
        FROM inv_transaction
        WHERE TransactionType = 'TRANSFER'
          AND FromLocation = in_loc
          AND PartID = in_part
          AND BatchNumber = 'MIGRATED'
        ORDER BY ReceiveDate ASC
        LIMIT 1;

        IF out_id IS NOT NULL THEN
            UPDATE inv_transaction
            SET BatchNumber = in_batch
            WHERE id = out_id;
            SET updated_rows = updated_rows + ROW_COUNT();
        END IF;
    END LOOP;
    CLOSE in_cursor;

    SELECT loop_counter AS total_loops, updated_rows AS total_updated_rows;

    IF loop_counter >= 1000 THEN
        SELECT 'Looped 1000 rows, please run the procedure again to continue.' AS message;
    ELSE
        SELECT 'Processing complete.' AS message;
    END IF;
END$$
DELIMITER ;

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
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Fix_BatchNumbers`(OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
BEGIN
    -- 1. Declare variables first
    DECLARE done INT DEFAULT FALSE;
    DECLARE null_id INT;
    DECLARE nextBatch BIGINT;

    -- 2. Declare cursor next
    DECLARE cur CURSOR FOR SELECT ID FROM inv_inventory WHERE BatchNumber IS NULL ORDER BY ID;

    -- 3. Declare handlers last
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Database error occurred in db_maint_inv_inventory_AssignBatchNumbers';
        ROLLBACK;
    END;

    START TRANSACTION;

    -- Get the last used batch number
    SELECT last_batch_number INTO nextBatch FROM inv_inventory_batch_seq;

    OPEN cur;
    read_loop: LOOP
        FETCH cur INTO null_id;
        IF done THEN
            LEAVE read_loop;
        END IF;
        SET nextBatch = nextBatch + 1;
        UPDATE inv_inventory SET BatchNumber = LPAD(nextBatch, 10, '0') WHERE ID = null_id;
        -- Update the sequence table
        UPDATE inv_inventory_batch_seq SET last_batch_number = nextBatch;
    END LOOP;
    CLOSE cur;

    COMMIT;

    SET p_Status = 0;
    SET p_ErrorMsg = NULL;
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
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Remove_Item_1_1`(IN `p_PartID` VARCHAR(300), IN `p_Location` VARCHAR(100), IN `p_Operation` VARCHAR(100), IN `p_Quantity` INT, IN `p_ItemType` VARCHAR(100), IN `p_User` VARCHAR(100), IN `p_BatchNumber` VARCHAR(100), IN `p_Notes` VARCHAR(1000), OUT `p_Status` INT, OUT `p_ErrorMsg` VARCHAR(255))
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
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_transaction_Add`(IN `in_TransactionType` ENUM('IN','OUT','TRANSFER'), IN `in_PartID` VARCHAR(300), IN `in_BatchNumber` VARCHAR(100), IN `in_FromLocation` VARCHAR(300), IN `in_ToLocation` VARCHAR(100), IN `in_Operation` VARCHAR(100), IN `in_Quantity` INT, IN `in_Notes` VARCHAR(1000), IN `in_User` VARCHAR(100), IN `in_ItemType` VARCHAR(100), IN `in_ReceiveDate` DATETIME)
INSERT INTO inv_transaction (
        TransactionType, PartID, `BatchNumber`, FromLocation, ToLocation, Operation, Quantity, Notes, User, ItemType, ReceiveDate
    ) VALUES (
        in_TransactionType, in_PartID, in_BatchNumber, in_FromLocation, in_ToLocation, in_Operation, in_Quantity, in_Notes, in_User, in_ItemType, in_ReceiveDate
    )$$
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
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Add_Error`(
    IN p_User VARCHAR(100),
    IN p_Severity VARCHAR(50),
    IN p_ErrorType VARCHAR(100),
    IN p_ErrorMessage TEXT,
    IN p_StackTrace TEXT,
    IN p_ModuleName VARCHAR(100),
    IN p_MethodName VARCHAR(100),
    IN p_AdditionalInfo TEXT,
    IN p_MachineName VARCHAR(100),
    IN p_OSVersion VARCHAR(100),
    IN p_AppVersion VARCHAR(50),
    IN p_ErrorTime DATETIME,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Add_Error: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    START TRANSACTION;

    
    IF p_User IS NULL OR p_User = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Warning: User parameter is empty, using default';
        SET p_User = 'Unknown';
    END IF;

    IF p_Severity IS NULL OR p_Severity = '' THEN
        SET p_Severity = 'Error';
    END IF;

    IF p_ErrorTime IS NULL THEN
        SET p_ErrorTime = NOW();
    END IF;

    
    INSERT INTO `log_error` (
        `User`, `Severity`, `ErrorType`, `ErrorMessage`, `StackTrace`, 
        `ModuleName`, `MethodName`, `AdditionalInfo`, `MachineName`, 
        `OSVersion`, `AppVersion`, `ErrorTime`
    ) VALUES (
        p_User, p_Severity, p_ErrorType, p_ErrorMessage, p_StackTrace,
        p_ModuleName, p_MethodName, p_AdditionalInfo, p_MachineName,
        p_OSVersion, p_AppVersion, p_ErrorTime
    );

    IF ROW_COUNT() > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Error logged successfully for user: ', p_User);
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = 'Warning: Error log entry was not created';
    END IF;

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Delete_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Delete_All: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    START TRANSACTION;

    
    SELECT COUNT(*) INTO v_Count FROM `log_error`;
    
    IF v_Count = 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Warning: No error log entries found to delete';
    ELSE
        DELETE FROM `log_error`;
        
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 0;
            SET p_ErrorMsg = CONCAT('Successfully deleted ', v_Count, ' error log entries');
        ELSE
            SET p_Status = 1;
            SET p_ErrorMsg = 'Warning: No error log entries were deleted';
        END IF;
    END IF;

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Delete_ById`(
    IN p_Id INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Delete_ById: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    START TRANSACTION;

    
    IF p_Id IS NULL OR p_Id <= 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Error: Valid ID parameter is required';
    ELSE
        
        SELECT COUNT(*) INTO v_Count FROM `log_error` WHERE `ID` = p_Id;
        
        IF v_Count = 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = CONCAT('Warning: Error log entry with ID ', p_Id, ' not found');
        ELSE
            DELETE FROM `log_error` WHERE `ID` = p_Id;
            
            IF ROW_COUNT() > 0 THEN
                SET p_Status = 0;
                SET p_ErrorMsg = CONCAT('Error log entry with ID ', p_Id, ' deleted successfully');
            ELSE
                SET p_Status = 1;
                SET p_ErrorMsg = CONCAT('Warning: Error log entry with ID ', p_Id, ' was not deleted');
            END IF;
        END IF;
    END IF;

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Get_All: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    SELECT COUNT(*) INTO v_Count FROM log_error;
    
    SELECT 
        ID,
        User,
        ErrorMessage,
        StackTrace,
        MethodName,
        ErrorType,
        LoggedDate
    FROM log_error 
    ORDER BY LoggedDate DESC;

    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Retrieved ', v_Count, ' error log entries successfully');
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Get_ByDateRange`(
    IN p_StartDate DATETIME,
    IN p_EndDate DATETIME,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Get_ByDateRange: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    
    IF p_StartDate IS NULL OR p_EndDate IS NULL THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Error: Start date and end date parameters are required';
    ELSEIF p_StartDate > p_EndDate THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Error: Start date must be less than or equal to end date';
    ELSE
        SELECT * FROM `log_error` 
        WHERE `ErrorTime` BETWEEN p_StartDate AND p_EndDate 
        ORDER BY `ErrorTime` DESC;

        SELECT COUNT(*) INTO v_Count FROM `log_error` 
        WHERE `ErrorTime` BETWEEN p_StartDate AND p_EndDate;
        
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Retrieved ', v_Count, ' error log entries between ', 
                               DATE_FORMAT(p_StartDate, '%Y-%m-%d %H:%i:%s'), ' and ', 
                               DATE_FORMAT(p_EndDate, '%Y-%m-%d %H:%i:%s'));
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Get_ByUser`(
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Get_ByUser: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    
    IF p_User IS NULL OR p_User = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Error: User parameter is required';
    ELSE
        SELECT * FROM `log_error` 
        WHERE `User` = p_User 
        ORDER BY `ErrorTime` DESC;

        SELECT COUNT(*) INTO v_Count FROM `log_error` WHERE `User` = p_User;
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Retrieved ', v_Count, ' error log entries for user: ', p_User);
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `log_error_Get_Unique`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in log_error_Get_Unique: ', @text);
    END;

    SET p_Status = 0;
    SET p_ErrorMsg = '';

    SELECT DISTINCT `MethodName`, `ErrorMessage` 
    FROM `log_error` 
    WHERE `MethodName` IS NOT NULL AND `ErrorMessage` IS NOT NULL
    ORDER BY `MethodName`, `ErrorMessage`;

    SELECT COUNT(DISTINCT CONCAT(IFNULL(`MethodName`, ''), '|', IFNULL(`ErrorMessage`, ''))) 
    INTO v_Count FROM `log_error` 
    WHERE `MethodName` IS NOT NULL AND `ErrorMessage` IS NOT NULL;

    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Retrieved ', v_Count, ' unique error combinations');
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `maint_InsertMissingUserUiSettings`()
BEGIN
    -- For each user in usr_users, if no usr_ui_settings row exists, insert default settings
    INSERT INTO `mtm_wip_application`.usr_ui_settings (UserId, SettingsJson, ShortcutsJson, UpdatedAt)
    SELECT u.User, '{"Theme_Name": "Default"}', '{}', NOW()
    FROM `mtm_wip_application`.usr_users u
    LEFT JOIN `mtm_wip_application`.usr_ui_settings s ON s.UserId = u.User
    WHERE s.UserId IS NULL;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `maint_reload_part_ids_and_operation_numbers`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Error handler for the whole procedure
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Database error occurred in maint_reload_part_ids_and_operation_numbers';
        ROLLBACK;
    END;

    -- Step 1: Rebuild md_part_ids from part_requirement
    START TRANSACTION;

    TRUNCATE TABLE mtm_wip_application.md_part_ids;

    INSERT INTO mtm_wip_application.md_part_ids (
        `PartID`,
        `Operations`,
        `Customer`,
        `Description`,
        `IssuedBy`,
        `ItemType`
    )
    SELECT
        seqs.ID AS `PartID`,
        CAST(seqs.sequence_array AS JSON) AS `Operations`,
        '' AS `Customer`,
        descs.Description AS `Description`,
        '[ System ]' AS `IssuedBy`,
        'WIP' AS `ItemType`
    FROM (
        SELECT
            ID,
            CONCAT('[', GROUP_CONCAT(DISTINCT SEQUENCE_NO ORDER BY SEQUENCE_NO), ']') AS sequence_array
        FROM
            `mtm database`.part_requirement
        GROUP BY
            ID
    ) seqs
    LEFT JOIN (
        SELECT
            ID,
            MIN(Description) AS Description
        FROM
            `mtm database`.part_requirement
        WHERE
            Description IS NOT NULL AND Description <> ''
        GROUP BY
            ID
    ) descs
    ON seqs.ID = descs.ID;

    COMMIT;

    -- Step 2: Reload md_operation_numbers from the now-updated md_part_ids
    START TRANSACTION;

    INSERT IGNORE INTO mtm_wip_application.md_operation_numbers (`Operation`)
    SELECT DISTINCT op_num
    FROM (
        SELECT
            TRIM(BOTH '"' FROM
                JSON_UNQUOTE(
                    JSON_EXTRACT(mpi.Operations, CONCAT('$[', n.n, ']'))
                )
            ) AS op_num
        FROM
            mtm_wip_application.md_part_ids mpi
        JOIN (
            SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL
            SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL
            SELECT 8 UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL
            SELECT 12 UNION ALL SELECT 13 UNION ALL SELECT 14 UNION ALL SELECT 15
        ) n
        ON n.n < JSON_LENGTH(mpi.Operations)
        WHERE mpi.Operations IS NOT NULL
    ) AS all_ops
    WHERE op_num IS NOT NULL AND op_num <> '';

    COMMIT;

    SET p_Status = 0;
    SET p_ErrorMsg = NULL;
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
CREATE DEFINER=`root`@`localhost` PROCEDURE `migrate_user_roles_debug`()
BEGIN
    DECLARE done INT DEFAULT FALSE;
    DECLARE v_old_username VARCHAR(100);
    DECLARE v_new_userid INT;
    DECLARE v_roleid INT;
    DECLARE v_rolename VARCHAR(50);
    DECLARE v_existing_roleid INT;

    DECLARE user_cur CURSOR FOR
        SELECT User FROM `mtm database`.users;
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    OPEN user_cur;

    read_loop: LOOP
        FETCH user_cur INTO v_old_username;
        IF done THEN
            LEAVE read_loop;
        END IF;

        INSERT INTO migration_debug_log (message) VALUES (CONCAT('Processing user: ', v_old_username));

        SET v_new_userid = NULL;
        SELECT ID INTO v_new_userid
        FROM mtm_wip_application.usr_users
        WHERE User = v_old_username
        LIMIT 1;

        IF v_new_userid IS NULL THEN
            INSERT INTO migration_debug_log (message) VALUES (CONCAT('User NOT found in new system: ', v_old_username));
            ITERATE read_loop;
        END IF;

        -- Assign role based on old tables
        IF EXISTS (SELECT 1 FROM `mtm database`.leads WHERE USER = v_old_username) THEN
            SET v_rolename = 'Admin';
        ELSEIF EXISTS (SELECT 1 FROM `mtm database`.readonly WHERE USER = v_old_username) THEN
            SET v_rolename = 'ReadOnly';
        ELSE
            SET v_rolename = 'User';
        END IF;

        INSERT INTO migration_debug_log (message) VALUES (CONCAT('Assigned role: ', v_rolename));

        -- Find the role ID in the new system
        SET v_roleid = NULL;
        SELECT ID INTO v_roleid
        FROM mtm_wip_application.sys_roles
        WHERE RoleName = v_rolename
        LIMIT 1;

        IF v_roleid IS NULL THEN
            INSERT INTO migration_debug_log (message) VALUES (CONCAT('Role NOT found: ', v_rolename));
            ITERATE read_loop;
        END IF;

        -- Check if this user-role mapping already exists
        SET v_existing_roleid = NULL;
        SELECT RoleID INTO v_existing_roleid
        FROM mtm_wip_application.sys_user_roles
        WHERE UserID = v_new_userid
        LIMIT 1;

        IF v_existing_roleid IS NOT NULL THEN
            IF v_existing_roleid = v_roleid THEN
                INSERT INTO migration_debug_log (message) VALUES (CONCAT('User-role already exists for: ', v_old_username, ' | Role: ', v_rolename));
                ITERATE read_loop;
            ELSE
                -- Update to new role
                UPDATE mtm_wip_application.sys_user_roles
                SET RoleID = v_roleid, AssignedBy = '[ System Migration ]', AssignedAt = NOW()
                WHERE UserID = v_new_userid;

                INSERT INTO migration_debug_log (message) VALUES (CONCAT('User-role updated for: ', v_old_username, ' to role: ', v_rolename));
                ITERATE read_loop;
            END IF;
        END IF;

        -- Insert the mapping, AssignedBy = '[ System Migration ]'
        INSERT INTO mtm_wip_application.sys_user_roles
            (UserID, RoleID, AssignedBy, AssignedAt)
        VALUES
            (v_new_userid, v_roleid, '[ System Migration ]', NOW());

        INSERT INTO migration_debug_log (message) VALUES (CONCAT('Migrated user: ', v_old_username, ' with role: ', v_rolename));

    END LOOP;

    CLOSE user_cur;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `query_get_all_usernames_and_roles`()
BEGIN
    SELECT u.User AS Username, s.RoleName
    FROM `mtm_wip_application`.usr_users u
    JOIN `mtm_wip_application`.sys_user_roles r ON r.UserID = u.ID
    JOIN `mtm_wip_application`.sys_roles s ON s.ID = r.RoleID
    ORDER BY u.User;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sp_ReassignBatchNumbers`()
BEGIN
    -- Declare all variables
    DECLARE done INT DEFAULT 0;
    DECLARE curINID BIGINT;
    DECLARE curPartID VARCHAR(50);
    DECLARE curINDate DATETIME;
    DECLARE curQty DECIMAL(18,4);
    DECLARE curFromLocation VARCHAR(50);
    DECLARE curToLocation VARCHAR(50);
    DECLARE newBatch BIGINT;
    DECLARE batch_str VARCHAR(20);
    DECLARE curOtherID BIGINT;
    DECLARE curOtherPartID VARCHAR(50);

    -- Declare all cursors and handlers before any statements
    DECLARE in_cursor CURSOR FOR
        SELECT ID, PartID, ReceiveDate, Quantity, FromLocation, ToLocation
        FROM mtm_wip_application.inv_transaction
        WHERE TransactionType = 'IN' AND BatchNumber IS NULL
        ORDER BY PartID, ReceiveDate;

    DECLARE other_cursor CURSOR FOR
        SELECT ID, PartID
        FROM mtm_wip_application.inv_transaction
        WHERE TransactionType IN ('TRANSFER', 'OUT') AND BatchNumber IS NULL
        ORDER BY PartID;

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;

    -- Now, ALL your procedural code
    SELECT last_batch_number INTO newBatch FROM mtm_wip_application.inv_inventory_batch_seq;

    OPEN in_cursor;
    in_loop: LOOP
        FETCH in_cursor INTO curINID, curPartID, curINDate, curQty, curFromLocation, curToLocation;
        IF done THEN
            LEAVE in_loop;
        END IF;
        SET newBatch = newBatch + 1;
        SET batch_str = LPAD(newBatch, 10, '0');
        UPDATE mtm_wip_application.inv_transaction
        SET BatchNumber = batch_str
        WHERE ID = curINID;
    END LOOP;
    CLOSE in_cursor;

    SET done = 0;

    OPEN other_cursor;
    other_loop: LOOP
        FETCH other_cursor INTO curOtherID, curOtherPartID;
        IF done THEN
            LEAVE other_loop;
        END IF;
        SET newBatch = newBatch + 1;
        SET batch_str = LPAD(newBatch, 10, '0');
        UPDATE mtm_wip_application.inv_transaction
        SET BatchNumber = batch_str
        WHERE ID = curOtherID;
    END LOOP;
    CLOSE other_cursor;

    UPDATE mtm_wip_application.inv_inventory_batch_seq
    SET last_batch_number = newBatch;

    COMMIT;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_AddQuickButton_1`(
    IN p_User VARCHAR(255),
    IN p_PartID VARCHAR(255),
    IN p_Operation VARCHAR(255),
    IN p_Quantity INT,
    IN p_Position INT
)
BEGIN
    UPDATE sys_last_10_transactions
    SET Position = Position + 1
    WHERE User = p_User AND Position >= p_Position;

    INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, Position)
    VALUES (p_User, p_PartID, p_Operation, p_Quantity, p_Position);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Get_ByUser`(IN `p_User` VARCHAR(100))
BEGIN
    -- 1. Remove older duplicates for this user (keep only the most recent for each PartID+Operation)
    DELETE t1 FROM sys_last_10_transactions t1
    INNER JOIN sys_last_10_transactions t2
        ON t1.User = t2.User
        AND t1.PartID = t2.PartID
        AND t1.Operation = t2.Operation
        AND t1.ReceiveDate < t2.ReceiveDate
    WHERE t1.User = p_User;

    -- 2. If more than 10 entries, keep only the 10 most recent and delete the rest
    DELETE FROM sys_last_10_transactions
    WHERE User = p_User
      AND ID NOT IN (
        SELECT id FROM (
            SELECT id
            FROM sys_last_10_transactions
            WHERE User = p_User
            ORDER BY ReceiveDate DESC
            LIMIT 10
        ) AS keepers
    );

    -- 3. Return the 10 most recent unique transactions for the user
    SELECT *
    FROM sys_last_10_transactions
    WHERE User = p_User
    ORDER BY ReceiveDate DESC
    LIMIT 10;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Get_ByUser_1`(IN p_User VARCHAR(255))
BEGIN
    SELECT *
    FROM sys_last_10_transactions
    WHERE User = p_User
    ORDER BY Position ASC
    LIMIT 10;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_MoveToLast_ByUser`(
    IN p_User VARCHAR(255),
    IN p_ReceiveDate DATETIME
)
BEGIN
    DECLARE latest_date DATETIME;
    DECLARE target_id INT;
    
    -- Get the ID of the record to move
    SELECT id INTO target_id
    FROM sys_last_10_transactions
    WHERE User = p_User AND ReceiveDate = p_ReceiveDate
    LIMIT 1;
    
    -- Find the latest date in the system
    SELECT MAX(ReceiveDate) INTO latest_date
    FROM sys_last_10_transactions
    WHERE User = p_User;
    
    -- If both records exist and they're not the same, update
    IF target_id IS NOT NULL AND p_ReceiveDate < latest_date THEN
        -- Update the target record with a date later than the current latest
        UPDATE sys_last_10_transactions
        SET ReceiveDate = DATE_ADD(latest_date, INTERVAL 1 SECOND)
        WHERE id = target_id;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Move_1`(
    IN p_User VARCHAR(255),
    IN p_FromPosition INT,
    IN p_ToPosition INT
)
BEGIN
    DECLARE tempPartID VARCHAR(255);
    DECLARE tempOperation VARCHAR(255);
    DECLARE tempQuantity INT;

    SELECT PartID, Operation, Quantity
    INTO tempPartID, tempOperation, tempQuantity
    FROM sys_last_10_transactions
    WHERE User = p_User AND Position = p_FromPosition;

    DELETE FROM sys_last_10_transactions
    WHERE User = p_User AND Position = p_FromPosition;

    IF p_FromPosition < p_ToPosition THEN
        UPDATE sys_last_10_transactions
        SET Position = Position - 1
        WHERE User = p_User AND Position > p_FromPosition AND Position <= p_ToPosition;
    ELSE
        UPDATE sys_last_10_transactions
        SET Position = Position + 1
        WHERE User = p_User AND Position >= p_ToPosition AND Position < p_FromPosition;
    END IF;

    INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, Position)
    VALUES (p_User, tempPartID, tempOperation, tempQuantity, p_ToPosition);
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_RemoveAndShift_ByUser`(
    IN p_User VARCHAR(255),
    IN p_ReceiveDate DATETIME
)
BEGIN
    -- Variables must be declared first
    DECLARE target_exists INT;
    DECLARE last_id INT;
    DECLARE done INT DEFAULT 0;
    DECLARE curr_id INT;
    DECLARE curr_date DATETIME;
    DECLARE prev_date DATETIME;
    
    -- Cursor declarations come after variables
    DECLARE dates_to_shift CURSOR FOR
        SELECT id, ReceiveDate
        FROM sys_last_10_transactions
        WHERE User = p_User AND ReceiveDate > p_ReceiveDate
        ORDER BY ReceiveDate;
    
    -- Handler declarations come last
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
    
    -- Check if the target date exists
    SELECT COUNT(*) INTO target_exists 
    FROM sys_last_10_transactions
    WHERE User = p_User AND ReceiveDate = p_ReceiveDate;
    
    IF target_exists > 0 THEN
        -- Set the previous date to the one we're removing
        SET prev_date = p_ReceiveDate;
        
        -- Open cursor to process each date that needs to be shifted
        OPEN dates_to_shift;
        read_loop: LOOP
            FETCH dates_to_shift INTO curr_id, curr_date;
            IF done THEN
                LEAVE read_loop;
            END IF;
            
            -- Update current record with previous date
            UPDATE sys_last_10_transactions
            SET ReceiveDate = prev_date
            WHERE id = curr_id;
            
            -- Store current date as previous for next iteration
            SET prev_date = curr_date;
            SET last_id = curr_id;
        END LOOP;
        CLOSE dates_to_shift;
        
        -- Delete the record with the original target date
        DELETE FROM sys_last_10_transactions
        WHERE User = p_User AND ReceiveDate = p_ReceiveDate
        LIMIT 1;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_RemoveAndShift_ByUser_1`(
    IN p_User VARCHAR(255),
    IN p_Position INT
)
BEGIN
    DELETE FROM sys_last_10_transactions
    WHERE User = p_User AND Position = p_Position;

    UPDATE sys_last_10_transactions
    SET Position = Position - 1
    WHERE User = p_User AND Position > p_Position;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Reorder_ByUser`(IN `p_User` VARCHAR(255), IN `p_SrcReceiveDate` DATETIME, IN `p_TargetIndex` INT)
BEGIN
    -- Create a temp table with the current order
    CREATE TEMPORARY TABLE tempDates (idx INT AUTO_INCREMENT PRIMARY KEY, dt DATETIME);
    INSERT INTO tempDates (dt)
        SELECT ReceiveDate FROM sys_last_10_transactions WHERE User = p_User ORDER BY ReceiveDate;

    -- Get the source index
    SET @srcIdx := NULL;
    SELECT idx INTO @srcIdx FROM tempDates WHERE dt = p_SrcReceiveDate;

    -- Get the total count
    SELECT COUNT(*) INTO @totalCount FROM tempDates;

    -- If invalid, exit
    IF @srcIdx IS NULL OR p_TargetIndex IS NULL OR @srcIdx = p_TargetIndex + 1 THEN
        DROP TEMPORARY TABLE tempDates;
    ELSE
        -- Get the src date
        SELECT dt INTO @srcDate FROM tempDates WHERE idx = @srcIdx;

        -- Remove src date from tempDates
        DELETE FROM tempDates WHERE idx = @srcIdx;

        -- Shift idx values accordingly
        UPDATE tempDates SET idx = idx - 1 WHERE idx > @srcIdx;

        -- Insert srcDate at the new position (target index is 0-based)
        UPDATE tempDates SET idx = idx + 1 WHERE idx > p_TargetIndex;
        INSERT INTO tempDates (idx, dt) VALUES (p_TargetIndex + 1, @srcDate);

        -- Update the actual table to match the new order
        SET @i := 1;
        SET @baseTime := NOW();

WHILE @i <= @totalCount DO
    SELECT dt INTO @curDate FROM tempDates WHERE idx = @i LIMIT 1;
    IF @curDate IS NOT NULL THEN
        UPDATE sys_last_10_transactions
        SET ReceiveDate = DATE_ADD(@baseTime, INTERVAL @i SECOND)
        WHERE User = p_User AND ReceiveDate = @curDate;
    END IF;
    SET @i := @i + 1;
END WHILE;

        DROP TEMPORARY TABLE tempDates;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_SwapPositions_ByUser`(
    IN p_User VARCHAR(255),
    IN p_ReceiveDate1 DATETIME,
    IN p_ReceiveDate2 DATETIME
)
BEGIN
    DECLARE id1 INT;
    DECLARE id2 INT;
    DECLARE temp_date DATETIME;
    
    -- Find the IDs of the records to swap
    SELECT id INTO id1 
    FROM sys_last_10_transactions 
    WHERE User = p_User AND ReceiveDate = p_ReceiveDate1
    LIMIT 1;
    
    SELECT id INTO id2 
    FROM sys_last_10_transactions 
    WHERE User = p_User AND ReceiveDate = p_ReceiveDate2
    LIMIT 1;
    
    -- If both records exist, swap their dates
    IF id1 IS NOT NULL AND id2 IS NOT NULL THEN
        -- Create a temporary date to hold the value during swap
        SET temp_date = p_ReceiveDate1;
        
        -- Update first record with second date
        UPDATE sys_last_10_transactions
        SET ReceiveDate = p_ReceiveDate2
        WHERE id = id1;
        
        -- Update second record with first date
        UPDATE sys_last_10_transactions
        SET ReceiveDate = temp_date
        WHERE id = id2;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Update_ByUserAndDate`(
    IN p_User VARCHAR(255),
    IN p_OldReceiveDate DATETIME,
    IN p_PartID VARCHAR(255),
    IN p_Operation VARCHAR(255),
    IN p_Quantity INT
)
BEGIN
    UPDATE sys_last_10_transactions
    SET PartID = p_PartID,
        Operation = p_Operation,
        Quantity = p_Quantity,
        ReceiveDate = NOW()
    WHERE User = p_User AND ReceiveDate = p_OldReceiveDate;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Update_ByUserAndPosition_1`(
    IN p_User VARCHAR(255),
    IN p_Position INT,
    IN p_PartID VARCHAR(255),
    IN p_Operation VARCHAR(255),
    IN p_Quantity INT
)
BEGIN
    UPDATE sys_last_10_transactions
    SET PartID = p_PartID,
        Operation = p_Operation,
        Quantity = p_Quantity
    WHERE User = p_User AND Position = p_Position;
END$$
DELIMITER ;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_roles_Get_ById`(IN `p_ID` INT)
BEGIN
    SELECT * FROM sys_roles WHERE ID = p_ID;
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
