-- ========================================
-- MTM WIP Application - Full Inventory Editing Stored Procedures
-- Date: [Current Date]
-- Purpose: Add comprehensive inventory item editing functionality
-- Includes all fields: PartID, Location, Operation, Quantity, ItemType, BatchNumber, Notes
-- ========================================

USE mtm_wip_application_test;

DELIMITER $$

-- ========================================
-- Procedure: inv_inventory_Update_Item
-- Purpose: Update all editable fields of inventory items with comprehensive validation
-- Parameters:
--   p_ID: Inventory record ID (PRIMARY KEY) - Required for record identification
--   p_PartID: Part identifier - Can be updated (with validation)
--   p_Location: Location - Can be updated (with validation) 
--   p_Operation: Operation number - Can be updated (with validation)
--   p_Quantity: Item quantity - Can be updated (must be non-negative)
--   p_ItemType: Item type - Can be updated (with validation)
--   p_BatchNumber: Batch number - Can be updated
--   p_Notes: Notes text - Can be updated (can be NULL or empty)
--   p_User: User making the change - Required
--   p_Original_PartID: Original Part ID for validation (prevents concurrent modification)
--   p_Original_BatchNumber: Original Batch Number for validation
-- Returns: Status and error message via OUT parameters
--   Status codes:
--     1 = Success
--     0 = No changes made (record unchanged)
--    -1 = SQL Exception/Error
--    -2 = Record not found or validation failed
--    -3 = Update failed (no rows affected)
--    -4 = Invalid data provided
-- ========================================
DROP PROCEDURE IF EXISTS `inv_inventory_Update_Item`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Update_Item`(
    IN p_ID INT,
    IN p_PartID VARCHAR(300),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(10),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(50),
    IN p_BatchNumber VARCHAR(100),
    IN p_Notes VARCHAR(1000),
    IN p_User VARCHAR(100),
    IN p_Original_PartID VARCHAR(300),
    IN p_Original_BatchNumber VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(500)
)
BEGIN
    DECLARE v_RowsAffected INT DEFAULT 0;
    DECLARE v_CurrentPartID VARCHAR(300) DEFAULT '';
    DECLARE v_CurrentBatchNumber VARCHAR(100) DEFAULT '';
    DECLARE v_HasChanges BOOLEAN DEFAULT FALSE;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1 p_Status = MYSQL_ERRNO, p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
    END;

    -- Input validation
    IF p_ID IS NULL OR p_ID <= 0 THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'Invalid inventory ID provided';
    ELSEIF p_PartID IS NULL OR TRIM(p_PartID) = '' THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'Part ID is required and cannot be empty';
    ELSEIF p_Location IS NULL OR TRIM(p_Location) = '' THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'Location is required and cannot be empty';
    ELSEIF p_Operation IS NULL OR TRIM(p_Operation) = '' THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'Operation is required and cannot be empty';
    ELSEIF p_Quantity IS NULL OR p_Quantity < 0 THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'Quantity must be non-negative';
    ELSEIF p_ItemType IS NULL OR TRIM(p_ItemType) = '' THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'Item Type is required and cannot be empty';
    ELSEIF p_User IS NULL OR TRIM(p_User) = '' THEN
        SET p_Status = -4;
        SET p_ErrorMsg = 'User is required and cannot be empty';
    ELSE
        START TRANSACTION;

        -- Validate that the record exists and get current values for concurrent modification check
        SELECT PartID, BatchNumber 
        INTO v_CurrentPartID, v_CurrentBatchNumber
        FROM inv_inventory
        WHERE ID = p_ID;

        -- Check if record was found
        IF v_CurrentPartID IS NULL THEN
            SET p_Status = -2;
            SET p_ErrorMsg = CONCAT('Inventory record not found with ID: ', p_ID);
            ROLLBACK;
        -- Check for concurrent modification
        ELSEIF v_CurrentPartID != p_Original_PartID OR 
               COALESCE(v_CurrentBatchNumber, '') != COALESCE(p_Original_BatchNumber, '') THEN
            SET p_Status = -2;
            SET p_ErrorMsg = CONCAT('Record has been modified by another user. Expected PartID: ', p_Original_PartID, ', Current PartID: ', v_CurrentPartID);
            ROLLBACK;
        ELSE
            -- Check if any values have actually changed (avoid unnecessary updates)
            SELECT COUNT(*) > 0
            INTO v_HasChanges
            FROM inv_inventory
            WHERE ID = p_ID 
              AND (
                  PartID != p_PartID OR
                  Location != p_Location OR
                  COALESCE(Operation, '') != COALESCE(p_Operation, '') OR
                  Quantity != p_Quantity OR
                  ItemType != p_ItemType OR
                  COALESCE(BatchNumber, '') != COALESCE(p_BatchNumber, '') OR
                  COALESCE(Notes, '') != COALESCE(p_Notes, '')
              );

            IF NOT v_HasChanges THEN
                SET p_Status = 0;
                SET p_ErrorMsg = 'No changes detected - record unchanged';
                ROLLBACK;
            ELSE
                -- Perform the update with all fields
                UPDATE inv_inventory 
                SET PartID = p_PartID,
                    Location = p_Location,
                    Operation = p_Operation,
                    Quantity = p_Quantity,
                    ItemType = p_ItemType,
                    BatchNumber = p_BatchNumber,
                    Notes = p_Notes,
                    User = p_User,
                    LastUpdated = NOW()
                WHERE ID = p_ID;
                
                SET v_RowsAffected = ROW_COUNT();
                
                IF v_RowsAffected = 0 THEN
                    SET p_Status = -3;
                    SET p_ErrorMsg = 'Failed to update inventory item - no rows affected';
                    ROLLBACK;
                ELSE
                    SET p_Status = 1;
                    SET p_ErrorMsg = CONCAT('Inventory item updated successfully. Rows affected: ', v_RowsAffected);
                    COMMIT;
                END IF;
            END IF;
        END IF;
    END IF;
END$$

-- ========================================
-- Procedure: inv_inventory_Get_ByID_ForEdit
-- Purpose: Get inventory record by ID with all fields for editing
-- Returns complete record with all editable fields
-- Parameters:
--   p_ID: Inventory record ID (PRIMARY KEY)
-- Returns: Single record or empty result set with all columns
-- ========================================
DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByID_ForEdit`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByID_ForEdit`(
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
        Notes,
        -- Add calculated fields for UI
        CASE WHEN Notes IS NULL OR TRIM(Notes) = '' THEN 0 ELSE 1 END AS HasNotes
    FROM inv_inventory
    WHERE ID = p_ID;
END$$

-- ========================================
-- Procedure: inv_inventory_Validate_MasterData
-- Purpose: Validate PartID, Operation, and Location against master data
-- Used by edit operations to ensure referential integrity
-- Parameters:
--   p_PartID: Part ID to validate
--   p_Operation: Operation to validate  
--   p_Location: Location to validate
-- Returns: Validation results via SELECT statement
-- ========================================
DROP PROCEDURE IF EXISTS `inv_inventory_Validate_MasterData`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Validate_MasterData`(
    IN p_PartID VARCHAR(300),
    IN p_Operation VARCHAR(10),
    IN p_Location VARCHAR(100)
)
BEGIN
    SELECT 
        -- Check PartID exists in master data
        (SELECT COUNT(*) FROM md_part_ids WHERE PartID = p_PartID) > 0 AS PartID_Valid,
        
        -- Check Operation exists in master data
        (SELECT COUNT(*) FROM md_operation_numbers WHERE Operation = p_Operation) > 0 AS Operation_Valid,
        
        -- Check Location exists in master data
        (SELECT COUNT(*) FROM md_locations WHERE Location = p_Location) > 0 AS Location_Valid,
        
        -- Return the input values for reference
        p_PartID AS Validated_PartID,
        p_Operation AS Validated_Operation,
        p_Location AS Validated_Location;
END$$

DELIMITER ;

-- ========================================
-- Verification Queries (for testing)
-- ========================================

-- Test that the procedures exist
SHOW PROCEDURE STATUS WHERE Db = 'mtm_wip_application_test' 
  AND Name IN ('inv_inventory_Update_Item', 'inv_inventory_Get_ByID_ForEdit', 'inv_inventory_Validate_MasterData');

-- Sample test calls (commented out - uncomment to test)
-- Get record for editing
-- CALL inv_inventory_Get_ByID_ForEdit(1);

-- Validate master data
-- CALL inv_inventory_Validate_MasterData('62 090 27', '90', 'A1-01-01');

-- Update complete inventory item (example - adjust parameters as needed)
-- CALL inv_inventory_Update_Item(
--     1,                    -- p_ID
--     '62 090 27',         -- p_PartID (can be changed)
--     'A1-01-02',          -- p_Location (can be changed) 
--     '100',               -- p_Operation (can be changed)
--     150,                 -- p_Quantity (can be changed)
--     'WIP',               -- p_ItemType (can be changed)
--     '0000000001',        -- p_BatchNumber (can be changed)
--     'Updated via full edit procedure', -- p_Notes (can be changed)
--     'JKOLL',             -- p_User
--     '62 090 27',         -- p_Original_PartID (for validation)
--     '0000000001',        -- p_Original_BatchNumber (for validation)
--     @status, @msg
-- );
-- SELECT @status AS Status, @msg AS Message;

-- ========================================
-- Migration Notes
-- ========================================
-- This procedure replaces inv_inventory_Update_Notes for comprehensive editing
-- The original procedure is kept for backward compatibility
-- New applications should use inv_inventory_Update_Item for full editing
-- inv_inventory_Update_Notes can still be used for notes-only updates