-- Stored procedure for updating Notes column in inventory items
-- Created for MTM WIP Application Note Editing System

DELIMITER $$

DROP PROCEDURE IF EXISTS `inv_inventory_Update_Note`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Update_Note` (
    IN `p_PartID` VARCHAR(300), 
    IN `p_Operation` VARCHAR(100), 
    IN `p_Location` VARCHAR(100), 
    IN `p_Notes` VARCHAR(1000), 
    IN `p_User` VARCHAR(100), 
    OUT `p_Status` INT, 
    OUT `p_ErrorMsg` VARCHAR(255)
) BEGIN
    DECLARE v_RowsAffected INT DEFAULT 0;
    DECLARE v_ErrorMessage TEXT DEFAULT '';
    DECLARE v_RecordCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1 v_ErrorMessage = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error occurred while updating note for part: ', p_PartID);
        ROLLBACK;
    END;
    
    START TRANSACTION;
    
    -- Validate input parameters
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        ROLLBACK;
    ELSEIF p_Operation IS NULL OR p_Operation = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Operation is required and cannot be empty';
        ROLLBACK;
    ELSEIF p_Location IS NULL OR p_Location = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Location is required and cannot be empty';
        ROLLBACK;
    ELSEIF p_User IS NULL OR p_User = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'User is required and cannot be empty';
        ROLLBACK;
    ELSE
        -- Check if record exists
        SELECT COUNT(*) INTO v_RecordCount 
        FROM inv_inventory 
        WHERE PartID = p_PartID AND Operation = p_Operation AND Location = p_Location;
        
        IF v_RecordCount = 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = CONCAT('No inventory record found for PartID: ', p_PartID, ', Operation: ', p_Operation, ', Location: ', p_Location);
        ELSE
            -- Update the Notes field
            UPDATE inv_inventory 
            SET Notes = p_Notes,
                LastUpdated = NOW(),
                User = p_User
            WHERE PartID = p_PartID AND Operation = p_Operation AND Location = p_Location
            LIMIT 1;
            
            SET v_RowsAffected = ROW_COUNT();
            
            IF v_RowsAffected > 0 THEN
                SET p_Status = 0;
                SET p_ErrorMsg = CONCAT('Note updated successfully for part: ', p_PartID);
            ELSE
                SET p_Status = 1;
                SET p_ErrorMsg = CONCAT('Failed to update note for part: ', p_PartID, ', Operation: ', p_Operation, ', Location: ', p_Location);
            END IF;
        END IF;
        
        COMMIT;
    END IF;
END$$

DELIMITER ;