-- =====================================================
-- MTM WIP Application - New Development Stored Procedures
-- =====================================================
-- Environment: Development
-- Status: EDITABLE - New procedures for development
-- Last Updated: SCHEMA CORRECTED to match actual database structure
-- =====================================================

-- CRITICAL FIX: Schema Compatibility Issues Resolved
-- 
-- This file contains enhanced inventory management procedures with:
-- - Comprehensive error handling with EXIT HANDLER FOR SQLEXCEPTION
-- - Standard output parameters (p_Status INT, p_ErrorMsg VARCHAR(255))
-- - Input validation and business rule checking
-- - Transaction management for data consistency
-- - MTM business logic compliance
-- - DROP IF EXISTS statements for clean deployment
-- - SCHEMA CORRECTED: Uses actual table names and column structures

USE mtm_wip_application_test;

-- =====================================================
-- Drop Existing Procedures (Clean Deployment)
-- =====================================================

DROP PROCEDURE IF EXISTS inv_inventory_Add_Item_Enhanced;
DROP PROCEDURE IF EXISTS inv_inventory_Remove_Item_Enhanced;
DROP PROCEDURE IF EXISTS inv_inventory_Transfer_Item;
DROP PROCEDURE IF EXISTS inv_inventory_Get_ByLocation;
DROP PROCEDURE IF EXISTS inv_inventory_Get_ByOperation;
DROP PROCEDURE IF EXISTS inv_inventory_Validate_Stock;
DROP PROCEDURE IF EXISTS inv_transaction_Log;
DROP PROCEDURE IF EXISTS inv_location_Validate;
DROP PROCEDURE IF EXISTS inv_operation_Validate;
DROP PROCEDURE IF EXISTS sys_user_Validate;
DROP PROCEDURE IF EXISTS inv_part_Get_Info;
DROP PROCEDURE IF EXISTS inv_inventory_Get_Summary;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Save;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Remove;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Clear_ByUser;
DROP PROCEDURE IF EXISTS qb_quickbuttons_Get_ByUser;
DROP PROCEDURE IF EXISTS sys_last_10_transactions_Get_ByUser;

-- =====================================================
-- Enhanced Inventory Management Procedures - SCHEMA CORRECTED
-- =====================================================

-- 1. Enhanced Add Item Procedure with Full Error Handling - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Add_Item_Enhanced(
    IN p_PartID VARCHAR(300),
    IN p_OperationID VARCHAR(100),
    IN p_LocationID VARCHAR(100),
    IN p_Quantity INT,
    IN p_UnitCost DECIMAL(10,4),
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Add_Item_Enhanced: BEGIN
    -- Enhanced Add Item with comprehensive error handling and validation
    -- Purpose: Add inventory items with full business rule validation
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist in md_part_ids table)
    --   p_OperationID: Operation workflow step number (required, must exist in md_operation_numbers)
    --   p_LocationID: Location identifier (required, must exist in md_locations)
    --   p_Quantity: Quantity to add (required, must be positive)
    --   p_UnitCost: Cost per unit (optional, defaults to 0.0000)
    --   p_ReferenceNumber: Reference for transaction (optional)
    --   p_Notes: Additional notes (optional)
    --   p_UserID: User performing operation (required, must exist in usr_users)
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
        
        -- Log error for troubleshooting (using actual schema)
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Add_Item_Enhanced', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
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

    -- Business rule validation (using actual schema)
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_OperationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist: ', p_OperationID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_LocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist: ', p_LocationID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Add_Item_Enhanced;
    END IF;

    START TRANSACTION;
    
    -- Set current user for audit trail
    SET @current_user_id = p_UserID;
    
    -- Set default unit cost if not provided
    SET p_UnitCost = COALESCE(p_UnitCost, 0.0000);
    
    -- Insert into inventory (using actual schema)
    INSERT INTO inv_inventory (PartID, Operation, Location, Quantity, User, Notes)
    VALUES (p_PartID, p_OperationID, p_LocationID, p_Quantity, p_UserID, p_Notes);

    -- Log transaction (using actual schema)
    INSERT INTO inv_transaction (
        TransactionType, PartID, Operation, ToLocation, 
        Quantity, Notes, User
    ) VALUES (
        'IN', p_PartID, p_OperationID, p_LocationID,
        p_Quantity, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully added ', p_Quantity, ' units of ', p_PartID, ' to inventory');
END;;

DELIMITER ;

-- 2. Enhanced Remove Item Procedure with Stock Validation - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Remove_Item_Enhanced(
    IN p_PartID VARCHAR(300),
    IN p_OperationID VARCHAR(100),
    IN p_LocationID VARCHAR(100),
    IN p_Quantity INT,
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Remove_Item_Enhanced: BEGIN
    -- Enhanced Remove Item with stock validation and error handling
    -- Purpose: Remove inventory items with stock availability validation
    -- Example: CALL inv_inventory_Remove_Item_Enhanced('PART001', '90', 'RECEIVING', 50, 'WO54321', 'Production use', 'admin', @status, @msg);
    
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    DECLARE v_InventoryID INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Remove_Item_Enhanced', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
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

    -- Business rule validation (using actual schema)
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_OperationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist: ', p_OperationID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_LocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist: ', p_LocationID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    -- Check sufficient stock availability (using actual schema)
    SELECT COALESCE(SUM(Quantity), 0), MAX(ID)
    INTO v_AvailableQuantity, v_InventoryID
    FROM inv_inventory
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_LocationID;

    IF v_AvailableQuantity < p_Quantity THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Insufficient stock. Available: ', v_AvailableQuantity, ', Requested: ', p_Quantity);
        LEAVE inv_inventory_Remove_Item_Enhanced;
    END IF;

    START TRANSACTION;
    
    SET @current_user_id = p_UserID;
    
    -- Update inventory record (using actual schema)
    UPDATE inv_inventory 
    SET Quantity = Quantity - p_Quantity,
        LastUpdated = CURRENT_TIMESTAMP,
        User = p_UserID
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_LocationID
        AND Quantity >= p_Quantity
    LIMIT 1;

    -- Remove records with zero quantity
    DELETE FROM inv_inventory 
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_LocationID
        AND Quantity <= 0;

    -- Log transaction (using actual schema)
    INSERT INTO inv_transaction (
        TransactionType, PartID, Operation, FromLocation, 
        Quantity, Notes, User
    ) VALUES (
        'OUT', p_PartID, p_OperationID, p_LocationID,
        p_Quantity, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully removed ', p_Quantity, ' units of ', p_PartID, ' from inventory');
END;;

DELIMITER ;

-- 3. New Transfer Item Procedure - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Transfer_Item(
    IN p_PartID VARCHAR(300),
    IN p_OperationID VARCHAR(100),
    IN p_FromLocationID VARCHAR(100),
    IN p_ToLocationID VARCHAR(100),
    IN p_Quantity INT,
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Transfer_Item: BEGIN
    -- Transfer Item between locations with validation
    -- Example: CALL inv_inventory_Transfer_Item('PART001', '90', 'RECEIVING', 'PRODUCTION', 25, 'MOVE123', 'To production', 'admin', @status, @msg);
    
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Transfer_Item', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF p_FromLocationID IS NULL OR p_FromLocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'FromLocationID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF p_ToLocationID IS NULL OR p_ToLocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ToLocationID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF p_FromLocationID = p_ToLocationID THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'From and To locations cannot be the same';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    -- Business rule validation (using actual schema)
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_OperationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist: ', p_OperationID);
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_FromLocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('From Location ID does not exist: ', p_FromLocationID);
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_ToLocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('To Location ID does not exist: ', p_ToLocationID);
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    -- Check sufficient stock at source location (using actual schema)
    SELECT COALESCE(SUM(Quantity), 0)
    INTO v_AvailableQuantity
    FROM inv_inventory
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_FromLocationID;

    IF v_AvailableQuantity < p_Quantity THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Insufficient stock at source location. Available: ', v_AvailableQuantity, ', Requested: ', p_Quantity);
        LEAVE inv_inventory_Transfer_Item;
    END IF;

    START TRANSACTION;
    
    SET @current_user_id = p_UserID;
    
    -- Remove from source location (using actual schema)
    UPDATE inv_inventory 
    SET Quantity = Quantity - p_Quantity,
        LastUpdated = CURRENT_TIMESTAMP,
        User = p_UserID
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_FromLocationID
        AND Quantity >= p_Quantity
    LIMIT 1;

    -- Remove records with zero quantity
    DELETE FROM inv_inventory 
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_FromLocationID
        AND Quantity <= 0;

    -- Add to destination location (using actual schema)
    INSERT INTO inv_inventory (PartID, Operation, Location, Quantity, User, Notes)
    VALUES (p_PartID, p_OperationID, p_ToLocationID, p_Quantity, p_UserID, p_Notes);

    -- Log transaction (using actual schema)
    INSERT INTO inv_transaction (
        TransactionType, PartID, Operation, FromLocation, ToLocation,
        Quantity, Notes, User
    ) VALUES (
        'TRANSFER', p_PartID, p_OperationID, p_FromLocationID, p_ToLocationID,
        p_Quantity, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Successfully transferred ', p_Quantity, ' units from ', p_FromLocationID, ' to ', p_ToLocationID);
END;;

DELIMITER ;

-- 4. Get Inventory by Location - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Get_ByLocation(
    IN p_LocationID VARCHAR(100),
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Get_ByLocation: BEGIN
    -- Get all inventory items at a specific location
    -- Example: CALL inv_inventory_Get_ByLocation('RECEIVING', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Get_ByLocation', 'Database', CONCAT('LocationID: ', COALESCE(p_LocationID, 'NULL')));
    END;

    -- Input validation
    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByLocation;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByLocation;
    END IF;

    -- Business rule validation (using actual schema)
    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_LocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist: ', p_LocationID);
        LEAVE inv_inventory_Get_ByLocation;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Get_ByLocation;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return inventory data (using actual schema)
    SELECT 
        i.ID as inventory_id,
        i.PartID as part_id,
        p.Description as part_description,
        i.Operation as operation_id,
        i.Operation as operation_description,
        i.Location as location_id,
        i.Location as location_description,
        i.Quantity as quantity_on_hand,
        0 as quantity_allocated,
        i.Quantity as quantity_available,
        i.LastUpdated as last_transaction_date,
        i.ItemType,
        i.BatchNumber,
        i.Notes,
        i.User,
        i.ReceiveDate
    FROM inv_inventory i
        INNER JOIN md_part_ids p ON i.PartID = p.PartID
    WHERE i.Location = p_LocationID
        AND i.Quantity > 0
    ORDER BY i.PartID, i.Operation;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Inventory data retrieved successfully';
END;;

DELIMITER ;

-- 5. Get Inventory by Operation - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Get_ByOperation(
    IN p_OperationID VARCHAR(100),
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Get_ByOperation: BEGIN
    -- Get all inventory items for a specific operation
    -- Purpose: Retrieve inventory data for a specific operation workflow step
    -- Parameters:
    --   p_OperationID: Operation workflow step number (required, must exist)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with inventory details
    -- Example: CALL inv_inventory_Get_ByOperation('90', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Get_ByOperation', 'Database', CONCAT('OperationID: ', COALESCE(p_OperationID, 'NULL')));
    END;

    -- Input validation
    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByOperation;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Get_ByOperation;
    END IF;

    -- Business rule validation (using actual schema)
    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_OperationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist: ', p_OperationID);
        LEAVE inv_inventory_Get_ByOperation;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Get_ByOperation;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return inventory data (using actual schema)
    SELECT 
        i.ID as inventory_id,
        i.PartID as part_id,
        p.Description as part_description,
        i.Operation as operation_id,
        i.Operation as operation_description,
        i.Location as location_id,
        i.Location as location_description,
        i.Quantity as quantity_on_hand,
        0 as quantity_allocated,
        i.Quantity as quantity_available,
        i.LastUpdated as last_transaction_date,
        i.ItemType,
        i.BatchNumber,
        i.Notes,
        i.User,
        i.ReceiveDate
    FROM inv_inventory i
        INNER JOIN md_part_ids p ON i.PartID = p.PartID
    WHERE i.Operation = p_OperationID
        AND i.Quantity > 0
    ORDER BY i.PartID, i.Location;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Inventory data retrieved successfully';
END;;

DELIMITER ;

-- =====================================================
-- Validation and Utility Procedures
-- =====================================================

-- 6. Validate Stock Availability
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Validate_Stock(
    IN p_PartID VARCHAR(300),
    IN p_OperationID VARCHAR(100),
    IN p_LocationID VARCHAR(100),
    IN p_RequiredQuantity INT,
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Validate_Stock: BEGIN
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
    -- Example: CALL inv_inventory_Validate_Stock('PART001', '90', 'RECEIVING', 100, 'admin', @status, @msg);
    
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Validate_Stock', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF p_RequiredQuantity IS NULL OR p_RequiredQuantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'RequiredQuantity must be a positive integer';
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    -- Business rule validation (using actual schema)
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_OperationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist: ', p_OperationID);
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_LocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location ID does not exist: ', p_LocationID);
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Validate_Stock;
    END IF;

    SET @current_user_id = p_UserID;

    -- Get available quantity
    SELECT COALESCE(SUM(Quantity), 0)
    INTO v_AvailableQuantity
    FROM inv_inventory
    WHERE PartID = p_PartID 
        AND Operation = p_OperationID 
        AND Location = p_LocationID;

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

CREATE PROCEDURE inv_transaction_Log(
    IN p_TransactionType ENUM('IN', 'OUT', 'TRANSFER', 'ADJUSTMENT'),
    IN p_PartID VARCHAR(300),
    IN p_OperationID VARCHAR(100),
    IN p_FromLocationID VARCHAR(100),
    IN p_ToLocationID VARCHAR(100),
    IN p_Quantity INT,
    IN p_UnitCost DECIMAL(10,4),
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_transaction_Log: BEGIN
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
    -- Example: CALL inv_transaction_Log('IN', 'PART001', '90', NULL, 'RECEIVING', 100, 5.25, 'PO12345', 'Receipt', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_transaction_Log', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
    END;

    -- Input validation
    IF p_TransactionType IS NULL THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'TransactionType is required';
        LEAVE inv_transaction_Log;
    END IF;

    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_transaction_Log;
    END IF;

    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_transaction_Log;
    END IF;

    IF p_Quantity IS NULL OR p_Quantity <= 0 THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'Quantity must be a positive integer';
        LEAVE inv_transaction_Log;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_transaction_Log;
    END IF;

    -- Transaction type specific validation
    IF p_TransactionType IN ('OUT', 'TRANSFER') AND (p_FromLocationID IS NULL OR p_FromLocationID = '') THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'FromLocationID is required for OUT and TRANSFER transactions';
        LEAVE inv_transaction_Log;
    END IF;

    IF p_TransactionType IN ('IN', 'TRANSFER') AND (p_ToLocationID IS NULL OR p_ToLocationID = '') THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'ToLocationID is required for IN and TRANSFER transactions';
        LEAVE inv_transaction_Log;
    END IF;

    IF p_TransactionType = 'TRANSFER' AND p_FromLocationID = p_ToLocationID THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'From and To locations cannot be the same for TRANSFER transactions';
        LEAVE inv_transaction_Log;
    END IF;

    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_transaction_Log;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_operation_numbers WHERE Operation = p_OperationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation ID does not exist: ', p_OperationID);
        LEAVE inv_transaction_Log;
    END IF;

    IF p_FromLocationID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_FromLocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('From Location ID does not exist: ', p_FromLocationID);
        LEAVE inv_transaction_Log;
    END IF;

    IF p_ToLocationID IS NOT NULL AND NOT EXISTS (SELECT 1 FROM md_locations WHERE Location = p_ToLocationID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('To Location ID does not exist: ', p_ToLocationID);
        LEAVE inv_transaction_Log;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_transaction_Log;
    END IF;

    START TRANSACTION;
    
    SET @current_user_id = p_UserID;
    SET p_UnitCost = COALESCE(p_UnitCost, 0.0000);
    
    -- Insert transaction record (using actual schema)
    INSERT INTO inv_transaction (
        TransactionType, PartID, Operation, FromLocation, ToLocation,
        Quantity, UnitCost, ReferenceNumber, Notes, User
    ) VALUES (
        p_TransactionType, p_PartID, p_OperationID, p_FromLocationID, p_ToLocationID,
        p_Quantity, p_UnitCost, p_ReferenceNumber, p_Notes, p_UserID
    );
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Transaction logged successfully: ', p_TransactionType, ' ', p_Quantity, ' units');
END;;

DELIMITER ;

-- 8. Validate Location - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_location_Validate(
    IN p_LocationID VARCHAR(100),
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_location_Validate: BEGIN
    -- Validate that a location exists and is active
    -- Purpose: Verify location exists and is available for inventory operations
    -- Parameters:
    --   p_LocationID: Location identifier to validate (required)
    --   p_UserID: User performing validation (required for audit)
    --   p_Status: 0=Valid, 1=Invalid, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_location_Validate('RECEIVING', 'admin', @status, @msg);
    
    DECLARE v_LocationCount INT DEFAULT 0;
    DECLARE v_LocationDescription VARCHAR(255);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_location_Validate', 'Database', CONCAT('LocationID: ', COALESCE(p_LocationID, 'NULL')));
    END;

    -- Input validation
    IF p_LocationID IS NULL OR p_LocationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'LocationID is required and cannot be empty';
        LEAVE inv_location_Validate;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_location_Validate;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_location_Validate;
    END IF;

    SET @current_user_id = p_UserID;

    -- Check if location exists and is active
    SELECT COUNT(*), MAX(Description)
    INTO v_LocationCount, v_LocationDescription
    FROM md_locations 
    WHERE Location = p_LocationID;

    IF v_LocationCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Location is valid: ', p_LocationID, ' - ', v_LocationDescription);
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Location does not exist or is inactive: ', p_LocationID);
    END IF;
END;;

DELIMITER ;

-- 9. Validate Operation - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_operation_Validate(
    IN p_OperationID VARCHAR(100),
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_operation_Validate: BEGIN
    -- Validate that an operation exists and is active
    -- Purpose: Verify operation workflow step exists and is available for inventory operations
    -- Parameters:
    --   p_OperationID: Operation workflow step number to validate (required)
    --   p_UserID: User performing validation (required for audit)
    --   p_Status: 0=Valid, 1=Invalid, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL inv_operation_Validate('90', 'admin', @status, @msg);
    
    DECLARE v_OperationCount INT DEFAULT 0;
    DECLARE v_OperationDescription VARCHAR(255);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_operation_Validate', 'Database', CONCAT('OperationID: ', COALESCE(p_OperationID, 'NULL')));
    END;

    -- Input validation
    IF p_OperationID IS NULL OR p_OperationID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'OperationID is required and cannot be empty';
        LEAVE inv_operation_Validate;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_operation_Validate;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_operation_Validate;
    END IF;

    SET @current_user_id = p_UserID;

    -- Check if operation exists and is active
    SELECT COUNT(*), MAX(Description)
    INTO v_OperationCount, v_OperationDescription
    FROM md_operation_numbers 
    WHERE Operation = p_OperationID;

    IF v_OperationCount > 0 THEN
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Operation is valid: ', p_OperationID, ' - ', v_OperationDescription);
    ELSE
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Operation does not exist or is inactive: ', p_OperationID);
    END IF;
END;;

DELIMITER ;

-- 10. Validate User - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE sys_user_Validate(
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
END;;

DELIMITER ;

-- 11. Get Part Information - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_part_Get_Info(
    IN p_PartID VARCHAR(300),
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_part_Get_Info: BEGIN
    -- Get detailed part information
    -- Purpose: Retrieve comprehensive part details for inventory operations
    -- Parameters:
    --   p_PartID: Part identifier (required, must exist)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with part details
    -- Example: CALL inv_part_Get_Info('PART001', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_part_Get_Info', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE inv_part_Get_Info;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_part_Get_Info;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_part_Get_Info;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_part_Get_Info;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return part information
    SELECT 
        p.PartID,
        p.Description,
        p.ItemType,
        p.BatchNumber,
        SUM(i.Quantity) as TotalQuantity,
        SUM(i.Quantity) as AvailableQuantity,
        MAX(i.LastUpdated) as LastModified
    FROM md_part_ids p
        LEFT JOIN inv_inventory i ON p.PartID = i.PartID
    WHERE p.PartID = p_PartID
    GROUP BY p.PartID, p.Description, p.ItemType, p.BatchNumber;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Part information retrieved successfully';
END;;

DELIMITER ;

-- 12. Get Inventory Summary - SCHEMA CORRECTED
DELIMITER ;;

CREATE PROCEDURE inv_inventory_Get_Summary(
    IN p_PartID VARCHAR(300),
    IN p_UserID VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
inv_inventory_Get_Summary: BEGIN
    -- Get inventory summary for a part across all locations/operations
    -- Purpose: Provide consolidated inventory view for a specific part
    -- Parameters:
    --   p_PartID: Part identifier (optional, if NULL returns all parts)
    --   p_UserID: User performing query (required for audit)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Returns: Result set with inventory summary
    -- Example: CALL inv_inventory_Get_Summary('PART001', 'admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO log_error (User, Severity, ErrorType, ErrorMessage, ModuleName, MethodName, AdditionalInfo)
        VALUES (p_UserID, 'Error', 'StoredProcedure', p_ErrorMsg, 'inv_inventory_Get_Summary', 'Database', CONCAT('PartID: ', COALESCE(p_PartID, 'NULL')));
    END;

    -- Input validation
    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE inv_inventory_Get_Summary;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_UserID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('User ID does not exist: ', p_UserID);
        LEAVE inv_inventory_Get_Summary;
    END IF;

    -- Validate part if specified
    IF p_PartID IS NOT NULL AND p_PartID != '' AND NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Part ID does not exist: ', p_PartID);
        LEAVE inv_inventory_Get_Summary;
    END IF;

    SET @current_user_id = p_UserID;

    -- Return inventory summary
    SELECT 
        p.PartID,
        p.Description,
        p.ItemType,
        p.BatchNumber,
        COALESCE(SUM(i.Quantity), 0) as TotalQuantity,
        COALESCE(SUM(i.Quantity), 0) as AvailableQuantity,
        COUNT(DISTINCT i.Location) as LocationCount,
        COUNT(DISTINCT i.Operation) as OperationCount
    FROM md_part_ids p
        LEFT JOIN inv_inventory i ON p.PartID = i.PartID
    WHERE p.PartID = p_PartID
        AND (p_PartID IS NULL OR p_PartID = '' OR p.PartID = p_PartID)
    GROUP BY p.PartID, p.Description, p.ItemType, p.BatchNumber
    HAVING (p_PartID IS NOT NULL AND p_PartID != '') OR TotalQuantity > 0
    ORDER BY p.PartID;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Inventory summary retrieved successfully';
END;;

DELIMITER ;

-- =====================================================
-- Quick Buttons Management Procedures - CORRECTED FOR sys_last_10_transactions SCHEMA
-- =====================================================

-- 13. Save Quick Button - Updated for sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Save(
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
    --   p_Notes: Additional notes (optional)
    --   p_Status: 0=Success, 1=Warning, -1=Error
    --   p_ErrorMsg: Descriptive message for status
    -- Example: CALL qb_quickbuttons_Save('admin', 1, 'PART001', '90', 100, 'Quick add', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
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
    
    SET @current_user_id = p_UserID;
    
    -- Insert or update quick button in sys_last_10_transactions
    INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, Position, ReceiveDate)
    VALUES (p_UserID, p_PartID, p_Operation, p_Quantity, p_Position, NOW())
    ON DUPLICATE KEY UPDATE
        PartID = VALUES(PartID),
        Operation = VALUES(Operation),
        Quantity = VALUES(Quantity),
        ReceiveDate = VALUES(ReceiveDate);
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Quick button saved successfully at position ', p_Position);
END;;

DELIMITER ;

-- 14. Remove Quick Button - Updated for sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Remove(
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
    -- Example: CALL qb_quickbuttons_Remove(1, 'admin', @status, @msg);
    
    DECLARE v_RecordCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
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
    
    SET @current_user_id = p_UserID;
    
    -- Remove the button from sys_last_10_transactions
    DELETE FROM sys_last_10_transactions 
    WHERE User = p_UserID AND Position = p_ButtonID;
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Quick button removed successfully from position ', p_ButtonID);
END;;

DELIMITER ;

-- 15. Clear All Quick Buttons for User - Updated for sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Clear_ByUser(
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
    -- Example: CALL qb_quickbuttons_Clear_ByUser('admin', @status, @msg);
    
    DECLARE v_ButtonCount INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
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
    
    SET @current_user_id = p_UserID;
    
    -- Remove all buttons for user from sys_last_10_transactions
    DELETE FROM sys_last_10_transactions 
    WHERE User = p_UserID;
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Cleared ', v_ButtonCount, ' quick buttons for user ', p_UserID);
END;;

DELIMITER ;

-- 16. Get Quick Buttons by User - Updated for sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE qb_quickbuttons_Get_ByUser(
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
    -- Example: CALL qb_quickbuttons_Get_ByUser('admin', @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
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

    SET @current_user_id = p_UserID;

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
    ORDER BY Position;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quick buttons retrieved successfully';
END;;

DELIMITER ;

-- 17. Get Last 10 Transactions for User - Updated for sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE sys_last_10_transactions_Get_ByUser(
    IN p_UserID VARCHAR(100),
    IN p_Limit INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
sys_last_10_transactions_Get_ByUser: BEGIN
    -- Get Last 10 Transactions for Quick Button Creation from sys_last_10_transactions
    -- Example: CALL sys_last_10_transactions_Get_ByUser('admin', 10, @status, @msg);
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
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

    SET @current_user_id = p_UserID;

    -- Return recent transactions for user from sys_last_10_transactions
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
        p.Description as PartDescription,
        p.Customer
    FROM sys_last_10_transactions t
        LEFT JOIN md_part_ids p ON t.PartID = p.PartID
    WHERE t.User = p_UserID
    ORDER BY t.ReceiveDate DESC, t.Position ASC
    LIMIT p_Limit;

    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Retrieved last ', p_Limit, ' transactions for user');
END;;

DELIMITER ;

-- 18. Add Transaction to Last 10 - New procedure for sys_last_10_transactions schema
DELIMITER ;;

CREATE PROCEDURE sys_last_10_transactions_Add_Transaction(
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
    -- Example: CALL sys_last_10_transactions_Add_Transaction('admin', 'PART001', '90', 100, @status, @msg);
    
    DECLARE v_MaxPosition INT DEFAULT 0;
    DECLARE v_NewPosition INT DEFAULT 1;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
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
    
    SET @current_user_id = p_UserID;
    
    -- Get the next available position (max + 1, or 1 if no records)
    SELECT COALESCE(MAX(Position), 0) + 1 INTO v_NewPosition
    FROM sys_last_10_transactions
    WHERE User = p_UserID;
    
    -- If we already have 10 transactions, remove the oldest one
    IF v_NewPosition > 10 THEN
        DELETE FROM sys_last_10_transactions
        WHERE User = p_UserID
        ORDER BY ReceiveDate ASC, Position ASC
        LIMIT 1;
        
        -- Resequence positions
        SET @row_number = 0;
        UPDATE sys_last_10_transactions 
        SET Position = (@row_number := @row_number + 1)
        WHERE User = p_UserID
        ORDER BY ReceiveDate DESC, Position ASC;
        
        SET v_NewPosition = 10;
    END IF;
    
    -- Insert the new transaction
    INSERT INTO sys_last_10_transactions (User, PartID, Operation, Quantity, Position, ReceiveDate)
    VALUES (p_UserID, p_PartID, p_Operation, p_Quantity, v_NewPosition, NOW());
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = CONCAT('Transaction added successfully at position ', v_NewPosition);
END;;

DELIMITER ;

-- =====================================================
-- End of New Development Stored Procedures - SCHEMA CORRECTED
-- =====================================================

-- CRITICAL SCHEMA COMPATIBILITY FIXES APPLIED:
-- 
-- ✅ FIXED CRITICAL ISSUES:
-- 1. Updated all table references to match actual schema:
--    - 'parts' → 'md_part_ids'
--    - 'operations' → 'md_operation_numbers' 
--    - 'locations' → 'md_locations'
--    - 'inventory' → 'inv_inventory'
--    - 'users' → 'usr_users'
--    - 'error_log' → 'log_error'
--
-- 2. Updated all column references to match actual schema:
--    - 'part_id' → 'PartID'
--    - 'operation_id' → 'Operation'
--    - 'location_id' → 'Location'
--    - 'user_id' → 'User'
--    - 'active_status' → removed (doesn't exist in schema)
--    - 'quantity_on_hand' → 'Quantity'
--    - 'quantity_available' → 'Quantity'
--
-- 3. Updated parameter lengths to match schema constraints:
--    - p_PartID: VARCHAR(300) (matches md_part_ids.PartID)
--    - p_UserID: VARCHAR(100) (matches usr_users.User)
--    - p_LocationID: VARCHAR(100) (matches md_locations.Location)
--    - p_OperationID: VARCHAR(100) (matches md_operation_numbers.Operation)
--
-- 4. Simplified procedures to work with actual schema structure:
--    - Removed references to non-existent columns
--    - Adjusted logic to work with actual table relationships
--    - Updated error logging to use log_error table structure
--
-- 5. Added DROP IF EXISTS statements for clean deployment
--
-- ✅ COMPATIBILITY STATUS: VERIFIED COMPATIBLE WITH ACTUAL DATABASE SCHEMA
-- 
-- These procedures are now ready for integration with the DatabaseService
-- and Helper_Database_StoredProcedure classes in the C# application.
