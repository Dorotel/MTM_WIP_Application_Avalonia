-- Fix for inv_inventory_Add_Item stored procedure
-- This updates the procedure to support OUT parameters as expected by the C# code

USE mtm_wip_application_test;

-- Drop the old procedure
DROP PROCEDURE IF EXISTS `inv_inventory_Add_Item`;

-- Create the updated procedure with OUT parameters
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

-- Test the procedure
SELECT 'Stored procedure inv_inventory_Add_Item updated successfully' as Result;
