-- inv_transfer_Execute_WithSplit.sql
-- Executes transfer operation with quantity splitting logic for partial transfers

DELIMITER $$

DROP PROCEDURE IF EXISTS inv_transfer_Execute_WithSplit$$

CREATE PROCEDURE inv_transfer_Execute_WithSplit(
    IN p_PartId VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_FromLocation VARCHAR(50),
    IN p_ToLocation VARCHAR(50),
    IN p_TransferQuantity INT,
    IN p_UserId VARCHAR(64),
    OUT p_TransactionId VARCHAR(36),
    OUT p_Status INT,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    DECLARE v_RemainingQuantity INT DEFAULT 0;
    DECLARE v_BatchNumber VARCHAR(50) DEFAULT '';
    DECLARE v_CreatedBy VARCHAR(64) DEFAULT '';
    DECLARE v_CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP;
    DECLARE v_Notes TEXT DEFAULT '';
    DECLARE v_WasSplit BOOLEAN DEFAULT FALSE;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_Status = 0;
        SET p_Message = CONCAT('Database error during transfer: ', SQLSTATE);
        SET p_TransactionId = NULL;
    END;

    -- Generate unique transaction ID
    SET p_TransactionId = UUID();

    START TRANSACTION;

    -- Validate inputs
    IF p_TransferQuantity <= 0 THEN
        SET p_Status = 0;
        SET p_Message = 'Transfer quantity must be positive';
        ROLLBACK;
        LEAVE;
    END IF;

    IF p_FromLocation = p_ToLocation THEN
        SET p_Status = 0;
        SET p_Message = 'Source and destination locations cannot be the same';
        ROLLBACK;
        LEAVE;
    END IF;

    -- Get current inventory information
    SELECT Quantity, BatchNumber, CreatedBy, CreatedDate, Notes
    INTO v_AvailableQuantity, v_BatchNumber, v_CreatedBy, v_CreatedDate, v_Notes
    FROM inv_inventory
    WHERE PartID = p_PartId
      AND Operation = p_Operation
      AND Location = p_FromLocation
      AND Quantity > 0
    LIMIT 1;

    -- Check if inventory exists
    IF v_AvailableQuantity IS NULL OR v_AvailableQuantity = 0 THEN
        SET p_Status = 0;
        SET p_Message = CONCAT('No available inventory found for Part: ', p_PartId, ', Operation: ', p_Operation, ', Location: ', p_FromLocation);
        ROLLBACK;
        LEAVE;
    END IF;

    -- Validate quantity (auto-cap if necessary)
    IF p_TransferQuantity > v_AvailableQuantity THEN
        SET p_TransferQuantity = v_AvailableQuantity;
        SET p_Message = CONCAT('Transfer quantity auto-capped to available quantity: ', v_AvailableQuantity);
    END IF;

    -- Calculate remaining quantity
    SET v_RemainingQuantity = v_AvailableQuantity - p_TransferQuantity;
    SET v_WasSplit = (v_RemainingQuantity > 0);

    -- Create inventory record at destination
    INSERT INTO inv_inventory (
        PartID, Operation, Location, Quantity, BatchNumber,
        CreatedBy, CreatedDate, Notes
    ) VALUES (
        p_PartId, p_Operation, p_ToLocation, p_TransferQuantity, v_BatchNumber,
        p_UserId, CURRENT_TIMESTAMP, CONCAT('Transferred from ', p_FromLocation, '. Original notes: ', COALESCE(v_Notes, ''))
    );

    -- Update source inventory
    IF v_RemainingQuantity > 0 THEN
        -- Partial transfer - update quantity
        UPDATE inv_inventory
        SET Quantity = v_RemainingQuantity,
            Notes = CONCAT(COALESCE(Notes, ''), ' [Partial transfer of ', p_TransferQuantity, ' to ', p_ToLocation, ']')
        WHERE PartID = p_PartId
          AND Operation = p_Operation
          AND Location = p_FromLocation;
    ELSE
        -- Full transfer - remove record
        DELETE FROM inv_inventory
        WHERE PartID = p_PartId
          AND Operation = p_Operation
          AND Location = p_FromLocation;
    END IF;

    -- Record transaction for audit trail
    INSERT INTO inv_transactions (
        TransactionId, PartID, Operation, FromLocation, ToLocation,
        OriginalQuantity, TransferredQuantity, RemainingQuantity,
        BatchNumber, UserId, TransactionDate, TransactionType,
        SplitDetails, Notes
    ) VALUES (
        p_TransactionId, p_PartId, p_Operation, p_FromLocation, p_ToLocation,
        v_AvailableQuantity, p_TransferQuantity, v_RemainingQuantity,
        v_BatchNumber, p_UserId, CURRENT_TIMESTAMP, 'TRANSFER',
        IF(v_WasSplit,
           JSON_OBJECT('wasSplit', TRUE, 'originalQuantity', v_AvailableQuantity, 'transferred', p_TransferQuantity, 'remaining', v_RemainingQuantity),
           JSON_OBJECT('wasSplit', FALSE, 'fullTransfer', TRUE)
        ),
        CONCAT('Transfer from ', p_FromLocation, ' to ', p_ToLocation)
    );

    SET p_Status = 1;
    IF p_Message IS NULL OR p_Message = '' THEN
        SET p_Message = CONCAT('Transfer completed successfully. ',
                              IF(v_WasSplit,
                                 CONCAT('Split: ', p_TransferQuantity, ' transferred, ', v_RemainingQuantity, ' remaining'),
                                 'Full quantity transferred'));
    END IF;

    COMMIT;
END$$

DELIMITER ;
