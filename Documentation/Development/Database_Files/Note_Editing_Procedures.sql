-- ========================================
-- MTM WIP Application - Note Editing Stored Procedures
-- Date: September 14, 2025
-- Purpose: Add note editing functionality to inventory items
-- ========================================

USE mtm_wip_application_test;

DELIMITER $$

-- ========================================
-- Procedure: inv_inventory_Update_Notes
-- Purpose: Update notes field for inventory items with transaction logging
-- Parameters:
--   p_ID: Inventory record ID (PRIMARY KEY)
--   p_PartID: Part identifier for validation
--   p_BatchNumber: Batch number for validation
--   p_Notes: New notes text (can be NULL or empty)
--   p_User: User making the change
-- Returns: Status and error message via OUT parameters
-- ========================================
DROP PROCEDURE IF EXISTS `inv_inventory_Update_Notes`$$
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
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    START TRANSACTION;

    -- Validate that the record exists
    SELECT COUNT(*) 
    INTO @record_exists
    FROM inv_inventory
    WHERE ID = p_ID 
      AND PartID = p_PartID 
      AND BatchNumber = p_BatchNumber;

    -- Check if record was found
    IF @record_exists = 0 THEN
        SET p_Status = -2;
        SET p_ErrorMsg = CONCAT('Inventory record not found: ID=', p_ID, ', PartID=', p_PartID, ', Batch=', p_BatchNumber);
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
            SET p_ErrorMsg = 'Failed to update inventory notes - no rows affected';
            ROLLBACK;
        ELSE
            SET p_Status = 1;
            SET p_ErrorMsg = 'Notes updated successfully';
            COMMIT;
        END IF;
    END IF;
END$$

-- ========================================
-- Procedure: inv_inventory_Get_ByID
-- Purpose: Get inventory record by ID for editing
-- Parameters:
--   p_ID: Inventory record ID (PRIMARY KEY)
-- Returns: Single record or empty result set
-- ========================================
DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByID`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByID`(
    IN p_ID INT
)
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
        BatchNumber,
        Notes
    FROM inv_inventory
    WHERE ID = p_ID;
END$$

DELIMITER ;

-- ========================================
-- Verification Queries (for testing)
-- ========================================

-- Test the procedures exist
SHOW PROCEDURE STATUS WHERE Db = 'mtm_wip_application_test' 
  AND Name IN ('inv_inventory_Update_Notes', 'inv_inventory_Get_ByID');

-- Sample test calls (commented out - uncomment to test)
-- CALL inv_inventory_Get_ByID(1);
-- CALL inv_inventory_Update_Notes(1, '62 090 27', '0000000001', 'Updated note via stored procedure', 'JKOLL', @status, @msg);
-- SELECT @status, @msg;