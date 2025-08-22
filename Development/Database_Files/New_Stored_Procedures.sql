-- =====================================================
-- MTM WIP Application - New Development Stored Procedures
-- =====================================================
-- Environment: Development
-- Status: EDITABLE - New procedures for development
-- Last Updated: Auto-generated from Compliance Fix #1
-- =====================================================

-- ?? CRITICAL FIX #1: Empty Development Stored Procedures
-- 
-- This file contains enhanced inventory management procedures with:
-- - Comprehensive error handling with EXIT HANDLER FOR SQLEXCEPTION
-- - Standard output parameters (p_Status INT, p_ErrorMsg VARCHAR(255))
-- - Input validation and business rule checking
-- - Transaction management for data consistency
-- - MTM business logic compliance

USE mtm_wip_application;

-- =====================================================
-- Enhanced Inventory Management Procedures
-- =====================================================

-- 1. Enhanced Add Item Procedure with Full Error Handling
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Add_Item_Enhanced(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_LocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_UnitCost DECIMAL(10,4),
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Enhanced Add Item with comprehensive error handling and validation
    -- Purpose: Add inventory items with full business rule validation
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist in parts table)
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_LocationID: Location identifier (required, must exist)
    --   p_Quantity: Quantity to add (required, must be positive)
    --   p_UnitCost: Cost per unit (optional, defaults to 0.0000)
    --   p_ReferenceNumber: Reference for transaction (optional)
    --   p_Notes: Additional notes (optional)
    --   p_UserID: User performing operation (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_inventory_Add_Item_Enhanced('PART001', '90', 'RECEIVING', 100, 5.25, 'PO12345', 'Initial stock', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        -- Log error for troubleshooting
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Add_Item_Enhanced', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM operations WHERE operation_id = p_OperationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist or is inactive: ', p_OperationID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_LocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist or is inactive: ', p_LocationID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    START TRANSACTION;
    
    -- Set current user for audit trail
    SET @current_user_id = p_UserID;
    
    -- Set default unit cost if not provided
    SET p_UnitCost = COALESCE(p_UnitCost, 0.0000);
    
    -- Insert/Update inventory record
    INSERT INTO inventory (part_id, operation_id, location_id, quantity_on_hand, last_transaction_date)
    VALUES (p_PartID, p_OperationID, p_LocationID, p_Quantity, NOW())
    ON DUPLICATE KEY UPDATE
        quantity_on_hand = quantity_on_hand + p_Quantity,
        last_transaction_date = NOW();

    -- Log transaction - TransactionType is IN because user is adding stock
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, to_location_id, 
        quantity, unit_cost, reference_number, notes, user_id
    ) VALUES (
        'IN', p_PartID, p_OperationID, p_LocationID,
        p_Quantity, p_UnitCost, p_ReferenceNumber, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully added ', p_Quantity, ' units of ', p_PartID, ' to inventory');
END;;

DELIMITER ;

-- 2. Enhanced Remove Item Procedure with Stock Validation
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Remove_Item_Enhanced(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_LocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Enhanced Remove Item with stock validation and error handling
    -- Purpose: Remove inventory items with stock availability validation
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist in parts table)
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_LocationID: Location identifier (required, must exist)
    --   p_Quantity: Quantity to remove (required, must be positive and available)
    --   p_ReferenceNumber: Reference for transaction (optional)
    --   p_Notes: Additional notes (optional)
    --   p_UserID: User performing operation (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_inventory_Remove_Item_Enhanced('PART001', '90', 'RECEIVING', 50, 'WO54321', 'Production use', 'admin', @status, @msg);
    
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Remove_Item_Enhanced', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM operations WHERE operation_id = p_OperationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist or is inactive: ', p_OperationID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_LocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist or is inactive: ', p_LocationID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    -- Check sufficient stock availability
    SELECT COALESCE(quantity_available, 0)
    INTO v_AvailableQuantity
    FROM inventory
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_LocationID;

    IF v_AvailableQuantity < p_Quantity THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Insufficient stock. Available: ', v_AvailableQuantity, ', Requested: ', p_Quantity);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    START TRANSACTION;
    
    SET @current_user_id = p_UserID;
    
    -- Update inventory record
    UPDATE inventory 
    SET quantity_on_hand = quantity_on_hand - p_Quantity,
        last_transaction_date = NOW()
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_LocationID;

    -- Log transaction - TransactionType is OUT because user is removing stock
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, from_location_id, 
        quantity, reference_number, notes, user_id
    ) VALUES (
        'OUT', p_PartID, p_OperationID, p_LocationID,
        p_Quantity, p_ReferenceNumber, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully removed ', p_Quantity, ' units of ', p_PartID, ' from inventory');
END;;

DELIMITER ;

-- 3. New Transfer Item Procedure
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Transfer_Item_New(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_FromLocationID VARCHAR(50),
    IN p_ToLocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Transfer Item between locations with validation
    -- Purpose: Transfer inventory items between locations with comprehensive validation
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist in parts table)
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_FromLocationID: Source location (required, must exist and have stock)
    --   p_ToLocationID: Destination location (required, must exist and be different from source)
    --   p_Quantity: Quantity to transfer (required, must be positive and available)
    --   p_ReferenceNumber: Reference for transaction (optional)
    --   p_Notes: Additional notes (optional)
    --   p_UserID: User performing operation (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_inventory_Transfer_Item_New('PART001', '90', 'RECEIVING', 'PRODUCTION', 25, 'MOVE123', 'To production', 'admin', @status, @msg);
    
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Transfer_Item_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF p_FromLocationID IS NULL OR p_FromLocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'FromLocationID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF p_ToLocationID IS NULL OR p_ToLocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ToLocationID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF p_FromLocationID = p_ToLocationID THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'From and To locations cannot be the same';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM operations WHERE operation_id = p_OperationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist or is inactive: ', p_OperationID);
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_FromLocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('From Location ID does not exist or is inactive: ', p_FromLocationID);
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_ToLocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('To Location ID does not exist or is inactive: ', p_ToLocationID);
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    -- Check sufficient stock at source location
    SELECT COALESCE(quantity_available, 0)
    INTO v_AvailableQuantity
    FROM inventory
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_FromLocationID;

    IF v_AvailableQuantity < p_Quantity THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Insufficient stock at source location. Available: ', v_AvailableQuantity, ', Requested: ', p_Quantity);
        LEAVE inv_inventory_Transfer_Item_New;
    END IF;

    START TRANSACTION;
    
    SET @current_user_id = p_UserID;
    
    -- Remove from source location
    UPDATE inventory 
    SET quantity_on_hand = quantity_on_hand - p_Quantity,
        last_transaction_date = NOW()
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_FromLocationID;

    -- Add to destination location
    INSERT INTO inventory (part_id, operation_id, location_id, quantity_on_hand, last_transaction_date)
    VALUES (p_PartID, p_OperationID, p_ToLocationID, p_Quantity, NOW())
    ON DUPLICATE KEY UPDATE
        quantity_on_hand = quantity_on_hand + p_Quantity,
        last_transaction_date = NOW();

    -- Log transaction - TransactionType is TRANSFER because user is moving stock
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, from_location_id, to_location_id,
        quantity, reference_number, notes, user_id
    ) VALUES (
        'TRANSFER', p_PartID, p_OperationID, p_FromLocationID, p_ToLocationID,
        p_Quantity, p_ReferenceNumber, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully transferred ', p_Quantity, ' units from ', p_FromLocationID, ' to ', p_ToLocationID);
END;;

DELIMITER ;

-- =====================================================
-- Inventory Query Procedures
-- =====================================================

-- 4. Get Inventory by Location
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Get_ByLocation_New(
    IN p_LocationID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Get all inventory items at a specific location
    -- Purpose: Retrieve inventory data for a specific location with validation
    -- Parameters:
    --   p_LocationID: Location identifier (required, must exist)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with inventory details
    -- Example: CALL inv_inventory_Get_ByLocation_New('RECEIVING', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Get_ByLocation_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByLocation_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByLocation_New;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_LocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist or is inactive: ', p_LocationID);
        LEAVE inv_inventory_Get_ByLocation_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Get_ByLocation_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return inventory data
    SELECT 
        i.inventory_id,
        i.part_id,
        p.part_description,
        i.operation_id,
        o.operation_description,
        i.location_id,
        l.location_description,
        i.quantity_on_hand,
        i.quantity_allocated,
        i.quantity_available,
        i.last_transaction_date
    FROM inventory i
        INNER JOIN parts p ON i.part_id = p.part_id
        INNER JOIN operations o ON i.operation_id = o.operation_id
        INNER JOIN locations l ON i.location_id = l.location_id
    WHERE i.location_id = p_LocationID
        AND p.active_status = TRUE
        AND o.active_status = TRUE
        AND l.active_status = TRUE
        AND i.quantity_on_hand > 0
    ORDER BY i.part_id, i.operation_id;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Inventory data retrieved successfully';
END;;

DELIMITER ;

-- 5. Get Inventory by Operation
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Get_ByOperation_New(
    IN p_OperationID VARCHAR(10),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Get all inventory items for a specific operation
    -- Purpose: Retrieve inventory data for a specific operation workflow step
    -- Parameters:
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with inventory details
    -- Example: CALL inv_inventory_Get_ByOperation_New('90', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Get_ByOperation_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByOperation_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByOperation_New;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM operations WHERE operation_id = p_OperationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist or is inactive: ', p_OperationID);
        LEAVE inv_inventory_Get_ByOperation_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Get_ByOperation_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return inventory data
    SELECT 
        i.inventory_id,
        i.part_id,
        p.part_description,
        i.operation_id,
        o.operation_description,
        i.location_id,
        l.location_description,
        i.quantity_on_hand,
        i.quantity_allocated,
        i.quantity_available,
        i.last_transaction_date
    FROM inventory i
        INNER JOIN parts p ON i.part_id = p.part_id
        INNER JOIN operations o ON i.operation_id = o.operation_id
        INNER JOIN locations l ON i.location_id = l.location_id
    WHERE i.operation_id = p_OperationID
        AND p.active_status = TRUE
        AND o.active_status = TRUE
        AND l.active_status = TRUE
        AND i.quantity_on_hand > 0
    ORDER BY i.part_id, i.location_id;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Inventory data retrieved successfully';
END;;

DELIMITER ;

-- =====================================================
-- Validation and Utility Procedures
-- =====================================================

-- 6. Validate Stock Availability
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_Stock_New(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_LocationID VARCHAR(50),
    IN p_RequiredQuantity INT,
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Validate sufficient stock before removal operations
    -- Purpose: Check if sufficient stock is available for removal/transfer operations
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist in parts table)
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_LocationID: Location identifier (required, must exist)
    --   p_RequiredQuantity: Quantity needed (required, must be positive)
    --   p_UserID: User performing validation (required for audit)
    --   p_Status: 0=Sufficient, 1=Insufficient, -1=Error
    --   p_ErrorMsg: Descriptive message with availability details
    -- Example: CALL inv_inventory_Validate_Stock_New('PART001', '90', 'RECEIVING', 100, 'admin', @status, @msg);
    
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Validate_Stock_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF p_RequiredQuantity IS NULL OR p_RequiredQuantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'RequiredQuantity must be a positive integer';
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM operations WHERE operation_id = p_OperationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist or is inactive: ', p_OperationID);
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_LocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist or is inactive: ', p_LocationID);
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Validate_Stock_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Get available quantity
    SELECT COALESCE(quantity_available, 0)
    INTO v_AvailableQuantity
    FROM inventory
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_LocationID;

    -- Check availability
    IF v_AvailableQuantity >= p_RequiredQuantity THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Stock validation successful. Available: ', v_AvailableQuantity, ', Required: ', p_RequiredQuantity);
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Insufficient stock. Available: ', v_AvailableQuantity, ', Required: ', p_RequiredQuantity);
    END IF;
END;;

DELIMITER ;

-- 7. Log Transaction
DELIMITER ;;

CREATE PROCEDURE inv_transaction_Log_New(
    IN p_TransactionType ENUM('IN', 'OUT', 'TRANSFER', 'ADJUSTMENT'),
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_FromLocationID VARCHAR(50),
    IN p_ToLocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_UnitCost DECIMAL(10,4),
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Log inventory transaction with comprehensive validation
    -- Purpose: Create transaction log entry with full validation
    -- Parameters:
    --   p_TransactionType: Type of transaction (IN/OUT/TRANSFER/ADJUSTMENT)
    --   p_PartID: Part identifier (required, must exist in parts table)
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_FromLocationID: Source location (required for OUT/TRANSFER)
    --   p_ToLocationID: Destination location (required for IN/TRANSFER)
    --   p_Quantity: Transaction quantity (required, must be positive)
    --   p_UnitCost: Cost per unit (optional, defaults to 0.0000)
    --   p_ReferenceNumber: Reference for transaction (optional)
    --   p_Notes: Additional notes (optional)
    --   p_UserID: User performing operation (required, must exist)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_transaction_Log_New('IN', 'PART001', '90', NULL, 'RECEIVING', 100, 5.25, 'PO12345', 'Receipt', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_transaction_Log_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_TransactionType IS NULL THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'TransactionType is required';
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_transaction_Log_New;
    END IF;

    -- Transaction type specific validation
    IF p_TransactionType IN ('OUT', 'TRANSFER') AND (p_FromLocationID IS NULL OR p_FromLocationID = '') THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'FromLocationID is required for OUT and TRANSFER transactions';
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_TransactionType IN ('IN', 'TRANSFER') AND (p_ToLocationID IS NULL OR p_ToLocationID = '') THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ToLocationID is required for IN and TRANSFER transactions';
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_TransactionType = 'TRANSFER' AND p_FromLocationID = p_ToLocationID THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'From and To locations cannot be the same for TRANSFER transactions';
        LEAVE inv_transaction_Log_New;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_transaction_Log_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM operations WHERE operation_id = p_OperationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist or is inactive: ', p_OperationID);
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_FromLocationID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_FromLocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('From Location ID does not exist or is inactive: ', p_FromLocationID);
        LEAVE inv_transaction_Log_New;
    END IF;

    IF p_ToLocationID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM locations WHERE location_id = p_ToLocationID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('To Location ID does not exist or is inactive: ', p_ToLocationID);
        LEAVE inv_transaction_Log_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_transaction_Log_New;
    END IF;

    START TRANSACTION;
    
    SET @current_user_id = p_UserID;
    SET p_UnitCost = COALESCE(p_UnitCost, 0.0000);
    
    -- Insert transaction record
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, from_location_id, to_location_id,
        quantity, unit_cost, reference_number, notes, user_id
    ) VALUES (
        p_TransactionType, p_PartID, p_OperationID, p_FromLocationID, p_ToLocationID,
        p_Quantity, p_UnitCost, p_ReferenceNumber, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Transaction logged successfully: ', p_TransactionType, ' ', p_Quantity, ' units');
END;;

DELIMITER ;

-- 8. Validate Location
DELIMITER ;;

CREATE PROCEDURE inv_location_Validate_New(
    IN p_LocationID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Validate that a location exists and is active
    -- Purpose: Verify location exists and is available for inventory operations
    -- Parameters:
    --   p_LocationID: Location identifier to validate (required)
    --   p_UserID: User performing validation (required for audit)
    --   p_Status: 0=Valid, 1=Invalid, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_location_Validate_New('RECEIVING', 'admin', @status, @msg);
    
    DECLARE v_LocationCount INT DEFAULT 0;
    DECLARE v_LocationDescription VARCHAR(255);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_location_Validate_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_location_Validate_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_location_Validate_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_location_Validate_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Check if location exists and is active
    SELECT COUNT(*), MAX(location_description)
    INTO v_LocationCount, v_LocationDescription
    FROM locations 
    WHERE location_id = p_LocationID AND active_status = TRUE;

    IF v_LocationCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Location is valid: ', p_LocationID, ' - ', v_LocationDescription);
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location does not exist or is inactive: ', p_LocationID);
    END IF;
END;;

DELIMITER ;

-- 9. Validate Operation
DELIMITER ;;

CREATE PROCEDURE inv_operation_Validate_New(
    IN p_OperationID VARCHAR(10),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Validate that an operation exists and is active
    -- Purpose: Verify operation workflow step exists and is available for inventory operations
    -- Parameters:
    --   p_OperationID: Operation workflow step number to validate (required)
    --   p_UserID: User performing validation (required for audit)
    --   p_Status: 0=Valid, 1=Invalid, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_operation_Validate_New('90', 'admin', @status, @msg);
    
    DECLARE v_OperationCount INT DEFAULT 0;
    DECLARE v_OperationDescription VARCHAR(255);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_operation_Validate_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_operation_Validate_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_operation_Validate_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_operation_Validate_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Check if operation exists and is active
    SELECT COUNT(*), MAX(operation_description)
    INTO v_OperationCount, v_OperationDescription
    FROM operations 
    WHERE operation_id = p_OperationID AND active_status = TRUE;

    IF v_OperationCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Operation is valid: ', p_OperationID, ' - ', v_OperationDescription);
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation does not exist or is inactive: ', p_OperationID);
    END IF;
END;;

DELIMITER ;

-- 10. Validate User
DELIMITER ;;

CREATE PROCEDURE sys_user_Validate_New(
    IN p_UserID VARCHAR(50),
    IN p_ValidatingUserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Validate that a user exists and is active
    -- Purpose: Verify user exists and is available for system operations
    -- Parameters:
    --   p_UserID: User identifier to validate (required)
    --   p_ValidatingUserID: User performing validation (required for audit)
    --   p_Status: 0=Valid, 1=Invalid, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL sys_user_Validate_New('testuser', 'admin', @status, @msg);
    
    DECLARE v_UserCount INT DEFAULT 0;
    DECLARE v_FullName VARCHAR(255);
    DECLARE v_Role VARCHAR(50);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'sys_user_Validate_New', p_ValidatingUserID, 'Error');
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sys_user_Validate_New;
    END IF;

    IF p_ValidatingUserID IS NULL OR p_ValidatingUserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ValidatingUserID is required and cannot be empty';
        LEAVE sys_user_Validate_New;
    END IF;

    -- Check if validating user exists
    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_ValidatingUserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Validating User ID does not exist or is inactive: ', p_ValidatingUserID);
        LEAVE sys_user_Validate_New;
    END IF;

    SET @current_user_id = p_ValidatingUserID;

    -- Check if target user exists and is active
    SELECT COUNT(*), MAX(full_name), MAX(role)
    INTO v_UserCount, v_FullName, v_Role
    FROM users 
    WHERE user_id = p_UserID AND active_status = TRUE;

    IF v_UserCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('User is valid: ', p_UserID, ' - ', v_FullName, ' (', v_Role, ')');
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User does not exist or is inactive: ', p_UserID);
    END IF;
END;;

DELIMITER ;

-- =====================================================
-- Additional Utility Procedures
-- =====================================================

-- 11. Get Part Information
DELIMITER ;;

CREATE PROCEDURE inv_part_Get_Info_New(
    IN p_PartID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Get detailed part information
    -- Purpose: Retrieve comprehensive part details for inventory operations
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with part details
    -- Example: CALL inv_part_Get_Info_New('PART001', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_part_Get_Info_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_part_Get_Info_New;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_part_Get_Info_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_part_Get_Info_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_part_Get_Info_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return part information
    SELECT 
        p.part_id,
        p.part_description,
        p.part_type,
        p.unit_of_measure,
        p.standard_cost,
        p.active_status,
        p.created_date,
        p.created_by,
        COALESCE(SUM(i.quantity_on_hand), 0) as total_on_hand,
        COALESCE(SUM(i.quantity_allocated), 0) as total_allocated,
        COALESCE(SUM(i.quantity_available), 0) as total_available,
        COUNT(DISTINCT CONCAT(i.operation_id, '|', i.location_id)) as location_count
    FROM parts p
        LEFT JOIN inventory i ON p.part_id = i.part_id
    WHERE p.part_id = p_PartID
        AND p.active_status = TRUE
    GROUP BY p.part_id, p.part_description, p.part_type, p.unit_of_measure, 
             p.standard_cost, p.active_status, p.created_date, p.created_by;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Part information retrieved successfully';
END;;

DELIMITER ;

-- 12. Get Inventory Summary
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Get_Summary_New(
    IN p_PartID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Get inventory summary for a part across all locations/operations
    -- Purpose: Provide consolidated inventory view for a specific part
    -- Parameters:
    --   p_PartID: Part identifier (optional, if NULL returns all parts)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with inventory summary
    -- Example: CALL inv_inventory_Get_Summary_New('PART001', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'inv_inventory_Get_Summary_New', p_UserID, 'Error');
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Get_Summary_New;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist or is inactive: ', p_UserID);
        LEAVE inv_inventory_Get_Summary_New;
    END IF;

    -- Validate part if specified
    IF p_PartID IS NOT NULL AND p_PartID != '' AND NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist or is inactive: ', p_PartID);
        LEAVE inv_inventory_Get_Summary_New;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return inventory summary
    SELECT 
        p.part_id,
        p.part_description,
        p.part_type,
        p.unit_of_measure,
        COALESCE(SUM(i.quantity_on_hand), 0) as total_on_hand,
        COALESCE(SUM(i.quantity_allocated), 0) as total_allocated,
        COALESCE(SUM(i.quantity_available), 0) as total_available,
        COUNT(DISTINCT i.location_id) as location_count,
        COUNT(DISTINCT i.operation_id) as operation_count,
        MAX(i.last_transaction_date) as last_activity_date
    FROM parts p
        LEFT JOIN inventory i ON p.part_id = i.part_id
    WHERE p.active_status = TRUE
        AND (p_PartID IS NULL OR p_PartID = '' OR p.part_id = p_PartID)
    GROUP BY p.part_id, p.part_description, p.part_type, p.unit_of_measure
    HAVING (p_PartID IS NOT NULL AND p_PartID != '') OR total_on_hand > 0
    ORDER BY p.part_id;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Inventory summary retrieved successfully';
END;;

DELIMITER ;

-- =====================================================
-- End of New Development Stored Procedures
-- =====================================================

-- ?? CRITICAL FIX #1 COMPLETED
-- 
-- Created 12 comprehensive stored procedures with:
-- ? Standard error handling pattern with EXIT HANDLER FOR SQLEXCEPTION
-- ? Standard output parameters (p_Status INT, p_ErrorMsg VARCHAR(255))
-- ? Comprehensive input validation for all parameters
-- ? Business rule validation (part exists, location exists, user exists, etc.)
-- ? Transaction management with proper START TRANSACTION/COMMIT/ROLLBACK
-- ? MTM business logic compliance (TransactionType based on user intent)
-- ? Detailed documentation with purpose, parameters, and examples
-- ? Proper audit trail logging with @current_user_id
-- ? Consistent error messages for troubleshooting
-- 
-- Procedures created:
-- 1. inv_inventory_Add_Item_Enhanced - Enhanced add with full error handling
-- 2. inv_inventory_Remove_Item_Enhanced - Enhanced remove with stock validation
-- 3. inv_inventory_Transfer_Item_New - Transfer between locations
-- 4. inv_inventory_Get_ByLocation_New - Get inventory by location
-- 5. inv_inventory_Get_ByOperation_New - Get inventory by operation
-- 6. inv_inventory_Validate_Stock_New - Validate stock availability
-- 7. inv_transaction_Log_New - Log inventory transactions
-- 8. inv_location_Validate_New - Validate location exists
-- 9. inv_operation_Validate_New - Validate operation exists
-- 10. sys_user_Validate_New - Validate user exists
-- 11. inv_part_Get_Info_New - Get detailed part information
-- 12. inv_inventory_Get_Summary_New - Get inventory summary
-- 
-- All procedures ready for service layer integration via Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()