DROP PROCEDURE IF EXISTS `inv_inventory_Add_Item`;
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
    DECLARE v_RecordCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT LPAD(COALESCE(MAX(CAST(last_batch_number AS UNSIGNED)), 0) + 1, 10, '0') 
    INTO v_BatchNumber 
    FROM inv_inventory_batch_seq;

    UPDATE inv_inventory_batch_seq SET last_batch_number = CAST(v_BatchNumber AS UNSIGNED);

    INSERT INTO inv_inventory (
        PartID, Location, Operation, Quantity, ItemType, ReceiveDate, User, Notes, BatchNumber
    ) VALUES (
        p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, NOW(), p_User, p_Notes, v_BatchNumber
    );

    INSERT INTO inv_transaction (
        TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
        Quantity, ItemType, ReceiveDate, User, Notes
    ) VALUES (
        'IN', v_BatchNumber, p_PartID, p_Location, NULL, p_Operation, 
        p_Quantity, p_ItemType, NOW(), p_User, p_Notes
    );

    SET p_Status = 0;
    SET p_ErrorMsg = 'Item added successfully';
    COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByPartID`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartID`(
    IN p_PartID VARCHAR(300),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM inv_inventory WHERE PartID = p_PartID;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByPartIDandOperation`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartIDandOperation`(
    IN p_PartID VARCHAR(300),
    IN p_Operation VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM inv_inventory WHERE PartID = p_PartID AND Operation = p_Operation;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByUser`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByUser`(
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM inv_inventory WHERE User = p_User ORDER BY ReceiveDate DESC;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `inv_inventory_Remove_Item`;
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
    DECLARE v_CurrentQuantity INT DEFAULT 0;
    DECLARE v_NewBatchNumber VARCHAR(100);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT Quantity INTO v_CurrentQuantity 
    FROM inv_inventory 
    WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID;

    IF v_CurrentQuantity IS NULL THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Batch not found';
        ROLLBACK;
    ELSEIF v_CurrentQuantity < p_Quantity THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Insufficient quantity';
        ROLLBACK;
    ELSEIF v_CurrentQuantity = p_Quantity THEN
        DELETE FROM inv_inventory WHERE BatchNumber = p_BatchNumber;
    ELSE
        UPDATE inv_inventory 
        SET Quantity = Quantity - p_Quantity 
        WHERE BatchNumber = p_BatchNumber;
    END IF;

    SELECT LPAD(COALESCE(MAX(CAST(last_batch_number AS UNSIGNED)), 0) + 1, 10, '0') 
    INTO v_NewBatchNumber 
    FROM inv_inventory_batch_seq;

    UPDATE inv_inventory_batch_seq SET last_batch_number = CAST(v_NewBatchNumber AS UNSIGNED);

    INSERT INTO inv_transaction (
        TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
        Quantity, ItemType, ReceiveDate, User, Notes
    ) VALUES (
        'OUT', v_NewBatchNumber, p_PartID, p_Location, NULL, p_Operation, 
        p_Quantity, p_ItemType, NOW(), p_User, p_Notes
    );

    SET p_Status = 0;
    SET p_ErrorMsg = 'Item removed successfully';
    COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `inv_inventory_Transfer_Part`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Transfer_Part`(
    IN p_BatchNumber VARCHAR(100),
    IN p_PartID VARCHAR(300),
    IN p_Operation VARCHAR(100),
    IN p_NewLocation VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_CurrentLocation VARCHAR(100);
    DECLARE v_TransferBatchNumber VARCHAR(100);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT Location INTO v_CurrentLocation 
    FROM inv_inventory 
    WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID;

    IF v_CurrentLocation IS NULL THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Part not found';
        ROLLBACK;
    END IF;

    UPDATE inv_inventory 
    SET Location = p_NewLocation 
    WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID;

    SELECT LPAD(COALESCE(MAX(CAST(last_batch_number AS UNSIGNED)), 0) + 1, 10, '0') 
    INTO v_TransferBatchNumber 
    FROM inv_inventory_batch_seq;

    UPDATE inv_inventory_batch_seq SET last_batch_number = CAST(v_TransferBatchNumber AS UNSIGNED);

    INSERT INTO inv_transaction (
        TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
        Quantity, ItemType, ReceiveDate, User, Notes
    ) VALUES (
        'TRANSFER', v_TransferBatchNumber, p_PartID, v_CurrentLocation, p_NewLocation, p_Operation, 
        (SELECT Quantity FROM inv_inventory WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID),
        (SELECT ItemType FROM inv_inventory WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID),
        NOW(), 'SYSTEM', 'Part transfer'
    );

    SET p_Status = 0;
    SET p_ErrorMsg = 'Part transferred successfully';
    COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `inv_inventory_Transfer_Quantity`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Transfer_Quantity`(
    IN p_BatchNumber VARCHAR(255),
    IN p_PartID VARCHAR(255),
    IN p_Operation VARCHAR(255),
    IN p_TransferQuantity INT,
    IN p_OriginalQuantity INT,
    IN p_NewLocation VARCHAR(255),
    IN p_User VARCHAR(255),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_CurrentLocation VARCHAR(100);
    DECLARE v_NewBatchNumber VARCHAR(100);
    DECLARE v_ItemType VARCHAR(100);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT Location, ItemType INTO v_CurrentLocation, v_ItemType
    FROM inv_inventory 
    WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID;

    IF v_CurrentLocation IS NULL THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Original batch not found';
        ROLLBACK;
    END IF;

    UPDATE inv_inventory 
    SET Quantity = Quantity - p_TransferQuantity 
    WHERE BatchNumber = p_BatchNumber AND PartID = p_PartID;

    SELECT LPAD(COALESCE(MAX(CAST(last_batch_number AS UNSIGNED)), 0) + 1, 10, '0') 
    INTO v_NewBatchNumber 
    FROM inv_inventory_batch_seq;

    UPDATE inv_inventory_batch_seq SET last_batch_number = CAST(v_NewBatchNumber AS UNSIGNED);

    INSERT INTO inv_inventory (
        PartID, Location, Operation, Quantity, ItemType, ReceiveDate, User, Notes, BatchNumber
    ) VALUES (
        p_PartID, p_NewLocation, p_Operation, p_TransferQuantity, v_ItemType, NOW(), p_User, 'Quantity transfer', v_NewBatchNumber
    );

    INSERT INTO inv_transaction (
        TransactionType, BatchNumber, PartID, FromLocation, ToLocation, Operation, 
        Quantity, ItemType, ReceiveDate, User, Notes
    ) VALUES (
        'TRANSFER', v_NewBatchNumber, p_PartID, v_CurrentLocation, p_NewLocation, p_Operation, 
        p_TransferQuantity, v_ItemType, NOW(), p_User, 'Quantity transfer'
    );

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quantity transferred successfully';
    COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_part_ids_Add_Part`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Add_Part`(
    IN p_PartID VARCHAR(300),
    IN p_Customer VARCHAR(100),
    IN p_Description VARCHAR(500),
    IN p_IssuedBy VARCHAR(100),
    IN p_ItemType VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_part_ids WHERE PartID = p_PartID;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Part ID already exists';
        ROLLBACK;
    ELSE
        INSERT INTO md_part_ids (PartID, Customer, Description, IssuedBy, ItemType, DateCreated)
        VALUES (p_PartID, p_Customer, p_Description, p_IssuedBy, p_ItemType, NOW());
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Part added successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_part_ids_Get_All`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM md_part_ids ORDER BY PartID;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_part_ids_Get_ByItemNumber`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Get_ByItemNumber`(
    IN p_PartID VARCHAR(300),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM md_part_ids WHERE PartID = p_PartID;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_part_ids_Update_Part`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Update_Part`(
    IN p_ID INT,
    IN p_PartID VARCHAR(300),
    IN p_Customer VARCHAR(100),
    IN p_Description VARCHAR(500),
    IN p_IssuedBy VARCHAR(100),
    IN p_ItemType VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_part_ids WHERE ID = p_ID;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Part not found';
        ROLLBACK;
    ELSE
        UPDATE md_part_ids 
        SET PartID = p_PartID, Customer = p_Customer, Description = p_Description, 
            IssuedBy = p_IssuedBy, ItemType = p_ItemType, DateModified = NOW()
        WHERE ID = p_ID;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Part updated successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_part_ids_Delete_ByItemNumber`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Delete_ByItemNumber`(
    IN p_PartID VARCHAR(300),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM inv_inventory WHERE PartID = p_PartID;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Cannot delete part with existing inventory';
        ROLLBACK;
    ELSE
        DELETE FROM md_part_ids WHERE PartID = p_PartID;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Part deleted successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_operation_numbers_Add_Operation`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Add_Operation`(
    IN p_Operation VARCHAR(100),
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_operation_numbers WHERE Operation = p_Operation;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Operation already exists';
        ROLLBACK;
    ELSE
        INSERT INTO md_operation_numbers (Operation, IssuedBy, DateCreated)
        VALUES (p_Operation, p_IssuedBy, NOW());
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Operation added successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_operation_numbers_Get_All`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM md_operation_numbers ORDER BY Operation;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_operation_numbers_Update_Operation`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Update_Operation`(
    IN p_Operation VARCHAR(100),
    IN p_NewOperation VARCHAR(100),
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_operation_numbers WHERE Operation = p_Operation;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Operation not found';
        ROLLBACK;
    ELSE
        UPDATE md_operation_numbers 
        SET Operation = p_NewOperation, IssuedBy = p_IssuedBy, DateModified = NOW()
        WHERE Operation = p_Operation;
        
        UPDATE inv_inventory SET Operation = p_NewOperation WHERE Operation = p_Operation;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Operation updated successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_operation_numbers_Delete_ByOperation`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Delete_ByOperation`(
    IN p_Operation VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM inv_inventory WHERE Operation = p_Operation;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Cannot delete operation with existing inventory';
        ROLLBACK;
    ELSE
        DELETE FROM md_operation_numbers WHERE Operation = p_Operation;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Operation deleted successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_locations_Add_Location`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Add_Location`(
    IN p_Location VARCHAR(100),
    IN p_IssuedBy VARCHAR(100),
    IN p_Building VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_locations WHERE Location = p_Location;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Location already exists';
        ROLLBACK;
    ELSE
        INSERT INTO md_locations (Location, IssuedBy, Building, DateCreated)
        VALUES (p_Location, p_IssuedBy, p_Building, NOW());
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Location added successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_locations_Get_All`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM md_locations ORDER BY Location;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_locations_Update_Location`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Update_Location`(
    IN p_OldLocation VARCHAR(100),
    IN p_Location VARCHAR(100),
    IN p_IssuedBy VARCHAR(100),
    IN p_Building VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_locations WHERE Location = p_OldLocation;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Location not found';
        ROLLBACK;
    ELSE
        UPDATE md_locations 
        SET Location = p_Location, IssuedBy = p_IssuedBy, Building = p_Building, DateModified = NOW()
        WHERE Location = p_OldLocation;
        
        UPDATE inv_inventory SET Location = p_Location WHERE Location = p_OldLocation;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Location updated successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_locations_Delete_ByLocation`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Delete_ByLocation`(
    IN p_Location VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM inv_inventory WHERE Location = p_Location;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Cannot delete location with existing inventory';
        ROLLBACK;
    ELSE
        DELETE FROM md_locations WHERE Location = p_Location;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Location deleted successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_item_types_Add_ItemType`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Add_ItemType`(
    IN p_ItemType VARCHAR(100),
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_item_types WHERE ItemType = p_ItemType;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Item type already exists';
        ROLLBACK;
    ELSE
        INSERT INTO md_item_types (ItemType, IssuedBy, DateCreated)
        VALUES (p_ItemType, p_IssuedBy, NOW());
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Item type added successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_item_types_Get_All`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM md_item_types ORDER BY ItemType;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_item_types_Update_ItemType`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Update_ItemType`(
    IN p_ID INT,
    IN p_ItemType VARCHAR(100),
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM md_item_types WHERE ID = p_ID;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Item type not found';
        ROLLBACK;
    ELSE
        UPDATE md_item_types 
        SET ItemType = p_ItemType, IssuedBy = p_IssuedBy, DateModified = NOW()
        WHERE ID = p_ID;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Item type updated successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `md_item_types_Delete_ByType`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_item_types_Delete_ByType`(
    IN p_ItemType VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM inv_inventory WHERE ItemType = p_ItemType;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Cannot delete item type with existing inventory';
        ROLLBACK;
    ELSE
        DELETE FROM md_item_types WHERE ItemType = p_ItemType;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'Item type deleted successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Add`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Add`(
    IN p_Username VARCHAR(100),
    IN p_FirstName VARCHAR(100),
    IN p_LastName VARCHAR(100),
    IN p_Email VARCHAR(255),
    IN p_Role VARCHAR(50),
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM usr_users WHERE Username = p_Username;
    
    IF v_Count > 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Username already exists';
        ROLLBACK;
    ELSE
        INSERT INTO usr_users (Username, FirstName, LastName, Email, Role, IsActive, IssuedBy, DateCreated)
        VALUES (p_Username, p_FirstName, p_LastName, p_Email, p_Role, 1, p_IssuedBy, NOW());
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'User added successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Add_User`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Add_User`(
    IN p_Username VARCHAR(100),
    IN p_FirstName VARCHAR(100),
    IN p_LastName VARCHAR(100),
    IN p_Email VARCHAR(255),
    IN p_Role VARCHAR(50),
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    CALL usr_users_Add(p_Username, p_FirstName, p_LastName, p_Email, p_Role, p_IssuedBy, p_Status, p_ErrorMsg);
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Get_All`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM usr_users ORDER BY Username;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Get_ByUser`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Get_ByUser`(
    IN p_Username VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM usr_users WHERE Username = p_Username;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Exists`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Exists`(
    IN p_Username VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT COUNT(*) INTO v_Count FROM usr_users WHERE Username = p_Username;
    
    SELECT v_Count AS UserExists;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Update`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Update`(
    IN p_ID INT,
    IN p_Username VARCHAR(100),
    IN p_FirstName VARCHAR(100),
    IN p_LastName VARCHAR(100),
    IN p_Email VARCHAR(255),
    IN p_Role VARCHAR(50),
    IN p_IsActive TINYINT,
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM usr_users WHERE ID = p_ID;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'User not found';
        ROLLBACK;
    ELSE
        UPDATE usr_users 
        SET Username = p_Username, FirstName = p_FirstName, LastName = p_LastName, 
            Email = p_Email, Role = p_Role, IsActive = p_IsActive, 
            IssuedBy = p_IssuedBy, DateModified = NOW()
        WHERE ID = p_ID;
        
        SET p_Status = 0;
        SET p_ErrorMsg = 'User updated successfully';
        COMMIT;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Update_User`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Update_User`(
    IN p_ID INT,
    IN p_Username VARCHAR(100),
    IN p_FirstName VARCHAR(100),
    IN p_LastName VARCHAR(100),
    IN p_Email VARCHAR(255),
    IN p_Role VARCHAR(50),
    IN p_IsActive TINYINT,
    IN p_IssuedBy VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    CALL usr_users_Update(p_ID, p_Username, p_FirstName, p_LastName, p_Email, p_Role, p_IsActive, p_IssuedBy, p_Status, p_ErrorMsg);
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Delete_ByID`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Delete_ByID`(
    IN p_ID INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    DECLARE v_Username VARCHAR(100);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*), Username INTO v_Count, v_Username FROM usr_users WHERE ID = p_ID;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'User not found';
        ROLLBACK;
    ELSE
        SELECT COUNT(*) INTO v_Count FROM inv_inventory WHERE User = v_Username;
        
        IF v_Count > 0 THEN
            SET p_Status = -1;
            SET p_ErrorMsg = 'Cannot delete user with existing inventory records';
            ROLLBACK;
        ELSE
            DELETE FROM usr_users WHERE ID = p_ID;
            
            SET p_Status = 0;
            SET p_ErrorMsg = 'User deleted successfully';
            COMMIT;
        END IF;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_users_Delete_User`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_users_Delete_User`(
    IN p_Username VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE v_Count INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    SELECT COUNT(*) INTO v_Count FROM usr_users WHERE Username = p_Username;
    
    IF v_Count = 0 THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'User not found';
        ROLLBACK;
    ELSE
        SELECT COUNT(*) INTO v_Count FROM inv_inventory WHERE User = p_Username;
        
        IF v_Count > 0 THEN
            SET p_Status = -1;
            SET p_ErrorMsg = 'Cannot delete user with existing inventory records';
            ROLLBACK;
        ELSE
            DELETE FROM usr_users WHERE Username = p_Username;
            
            SET p_Status = 0;
            SET p_ErrorMsg = 'User deleted successfully';
            COMMIT;
        END IF;
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sys_roles_Get_All`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_roles_Get_All`(
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT * FROM sys_roles ORDER BY RoleName;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sys_last_10_transactions_Get_ByUser`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `sys_last_10_transactions_Get_ByUser`(
    IN p_UserID VARCHAR(100),
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
    WHERE User = p_UserID 
    ORDER BY ReceiveDate DESC 
    LIMIT p_Limit;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `sys_last_10_transactions_Add_Transaction`;
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

DROP PROCEDURE IF EXISTS `qb_quickbuttons_Get_ByUser`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Get_ByUser`(
    IN p_User VARCHAR(100)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        RESIGNAL;
    END;

    SELECT * FROM qb_quickbuttons WHERE User = p_User ORDER BY Position;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `qb_quickbuttons_Save`;
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

DROP PROCEDURE IF EXISTS `qb_quickbuttons_Remove`;
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

DROP PROCEDURE IF EXISTS `qb_quickbuttons_Clear_ByUser`;
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

DROP PROCEDURE IF EXISTS `usr_ui_settings_Get`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_Get`(
    IN p_UserId VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT SettingsJson FROM usr_ui_settings WHERE UserId = p_UserId;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_ui_settings_SetJsonSetting`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetJsonSetting`(
    IN p_UserId VARCHAR(100),
    IN p_SettingsJson TEXT,
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

    INSERT INTO usr_ui_settings (UserId, SettingsJson, DateCreated, DateModified)
    VALUES (p_UserId, p_SettingsJson, NOW(), NOW())
    ON DUPLICATE KEY UPDATE
        SettingsJson = VALUES(SettingsJson),
        DateModified = NOW();

    SET p_Status = 0;
    SET p_ErrorMsg = 'Settings saved successfully';
    COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_ui_settings_SetThemeJson`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetThemeJson`(
    IN p_UserId VARCHAR(100),
    IN p_ThemeJson TEXT,
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

    INSERT INTO usr_ui_settings (UserId, ThemeJson, DateCreated, DateModified)
    VALUES (p_UserId, p_ThemeJson, NOW(), NOW())
    ON DUPLICATE KEY UPDATE
        ThemeJson = VALUES(ThemeJson),
        DateModified = NOW();

    SET p_Status = 0;
    SET p_ErrorMsg = 'Theme settings saved successfully';
    COMMIT;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_ui_settings_GetShortcutsJson`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_GetShortcutsJson`(
    IN p_UserId VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    SELECT ShortcutsJson FROM usr_ui_settings WHERE UserId = p_UserId;
    
    SET p_Status = 0;
    SET p_ErrorMsg = 'Query executed successfully';
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS `usr_ui_settings_SetShortcutsJson`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `usr_ui_settings_SetShortcutsJson`(
    IN p_UserId VARCHAR(100),
    IN p_ShortcutsJson TEXT,
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

    INSERT INTO usr_ui_settings (UserId, ShortcutsJson, DateCreated, DateModified)
    VALUES (p_UserId, p_ShortcutsJson, NOW(), NOW())
    ON DUPLICATE KEY UPDATE
        ShortcutsJson = VALUES(ShortcutsJson),
        DateModified = NOW();

    SET p_Status = 0;
    SET p_ErrorMsg = 'Shortcuts saved successfully';
    COMMIT;
END$$
DELIMITER ;
