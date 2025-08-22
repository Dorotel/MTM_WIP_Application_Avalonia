-- =====================================================
-- MTM WIP Application - Existing Stored Procedures
-- =====================================================
-- Environment: Production
-- Status: READ-ONLY - Current Production Procedures
-- Last Updated: [To be updated during deployment]
-- =====================================================

-- ?? CRITICAL WARNING: This file is READ-ONLY
-- 
-- FOR MODIFICATIONS:
-- - New procedures: Add to Development/Database_Files/New_Stored_Procedures.sql
-- - Update existing: Copy to Development/Database_Files/Updated_Stored_Procedures.sql and modify
--
-- ?? NO HARD-CODED SQL RULE: ALL application database operations MUST use these procedures

USE mtm_wip_application;

-- =====================================================
-- Inventory Management Procedures
-- =====================================================

-- Get Inventory by Part ID
DELIMITER $$
CREATE PROCEDURE sp_inventory_Get_ByPartID(
    IN p_PartID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_inventory_Get_ByPartID', p_UserID);
    END;

    SET @current_user_id = p_UserID;

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
    WHERE i.part_id = p_PartID
        AND p.active_status = TRUE
        AND o.active_status = TRUE
        AND l.active_status = TRUE
    ORDER BY i.operation_id, i.location_id;

    SET p_Success = TRUE;
    SET p_Message = 'Inventory retrieved successfully';
END$$
DELIMITER ;

-- Get Inventory by Location
DELIMITER $$
CREATE PROCEDURE sp_inventory_Get_ByLocation(
    IN p_LocationID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_inventory_Get_ByLocation', p_UserID);
    END;

    SET @current_user_id = p_UserID;

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
    ORDER BY i.part_id, i.operation_id;

    SET p_Success = TRUE;
    SET p_Message = 'Inventory retrieved successfully';
END$$
DELIMITER ;

-- =====================================================
-- Transaction Processing Procedures
-- =====================================================

-- Create IN Transaction
DELIMITER $$
CREATE PROCEDURE sp_transaction_Create_IN(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_LocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_UnitCost DECIMAL(10,4),
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE v_CurrentQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_transaction_Create_IN', p_UserID);
    END;

    START TRANSACTION;
    SET @current_user_id = p_UserID;

    -- Validate inputs
    IF p_Quantity <= 0 THEN
        SET p_Success = FALSE;
        SET p_Message = 'Quantity must be greater than zero';
        ROLLBACK;
        LEAVE proc_label;
    END IF;

    -- Get current inventory quantity
    SELECT COALESCE(quantity_on_hand, 0)
    INTO v_CurrentQuantity
    FROM inventory
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_LocationID;

    -- Insert/Update inventory
    INSERT INTO inventory (part_id, operation_id, location_id, quantity_on_hand, last_transaction_date)
    VALUES (p_PartID, p_OperationID, p_LocationID, p_Quantity, NOW())
    ON DUPLICATE KEY UPDATE
        quantity_on_hand = quantity_on_hand + p_Quantity,
        last_transaction_date = NOW();

    -- Insert transaction record
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, to_location_id, 
        quantity, unit_cost, reference_number, notes, user_id
    ) VALUES (
        'IN', p_PartID, p_OperationID, p_LocationID,
        p_Quantity, p_UnitCost, p_ReferenceNumber, p_Notes, p_UserID
    );

    COMMIT;
    SET p_Success = TRUE;
    SET p_Message = CONCAT('IN transaction completed: Added ', p_Quantity, ' units');
END$$
DELIMITER ;

-- Create OUT Transaction
DELIMITER $$
CREATE PROCEDURE sp_transaction_Create_OUT(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_LocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_transaction_Create_OUT', p_UserID);
    END;

    START TRANSACTION;
    SET @current_user_id = p_UserID;

    -- Validate inputs
    IF p_Quantity <= 0 THEN
        SET p_Success = FALSE;
        SET p_Message = 'Quantity must be greater than zero';
        ROLLBACK;
        LEAVE proc_label;
    END IF;

    -- Check available quantity
    SELECT COALESCE(quantity_available, 0)
    INTO v_AvailableQuantity
    FROM inventory
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_LocationID;

    IF v_AvailableQuantity < p_Quantity THEN
        SET p_Success = FALSE;
        SET p_Message = CONCAT('Insufficient inventory: Available=', v_AvailableQuantity, ', Requested=', p_Quantity);
        ROLLBACK;
        LEAVE proc_label;
    END IF;

    -- Update inventory
    UPDATE inventory 
    SET quantity_on_hand = quantity_on_hand - p_Quantity,
        last_transaction_date = NOW()
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_LocationID;

    -- Insert transaction record
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, from_location_id, 
        quantity, reference_number, notes, user_id
    ) VALUES (
        'OUT', p_PartID, p_OperationID, p_LocationID,
        p_Quantity, p_ReferenceNumber, p_Notes, p_UserID
    );

    COMMIT;
    SET p_Success = TRUE;
    SET p_Message = CONCAT('OUT transaction completed: Removed ', p_Quantity, ' units');
END$$
DELIMITER ;

-- Create TRANSFER Transaction
DELIMITER $$
CREATE PROCEDURE sp_transaction_Create_TRANSFER(
    IN p_PartID VARCHAR(50),
    IN p_OperationID VARCHAR(10),
    IN p_FromLocationID VARCHAR(50),
    IN p_ToLocationID VARCHAR(50),
    IN p_Quantity INT,
    IN p_ReferenceNumber VARCHAR(100),
    IN p_Notes TEXT,
    IN p_UserID VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE v_AvailableQuantity INT DEFAULT 0;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_transaction_Create_TRANSFER', p_UserID);
    END;

    START TRANSACTION;
    SET @current_user_id = p_UserID;

    -- Validate inputs
    IF p_Quantity <= 0 THEN
        SET p_Success = FALSE;
        SET p_Message = 'Quantity must be greater than zero';
        ROLLBACK;
        LEAVE proc_label;
    END IF;

    IF p_FromLocationID = p_ToLocationID THEN
        SET p_Success = FALSE;
        SET p_Message = 'From and To locations cannot be the same';
        ROLLBACK;
        LEAVE proc_label;
    END IF;

    -- Check available quantity at source location
    SELECT COALESCE(quantity_available, 0)
    INTO v_AvailableQuantity
    FROM inventory
    WHERE part_id = p_PartID 
        AND operation_id = p_OperationID 
        AND location_id = p_FromLocationID;

    IF v_AvailableQuantity < p_Quantity THEN
        SET p_Success = FALSE;
        SET p_Message = CONCAT('Insufficient inventory at source: Available=', v_AvailableQuantity, ', Requested=', p_Quantity);
        ROLLBACK;
        LEAVE proc_label;
    END IF;

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

    -- Insert transaction record
    INSERT INTO inventory_transactions (
        transaction_type, part_id, operation_id, from_location_id, to_location_id,
        quantity, reference_number, notes, user_id
    ) VALUES (
        'TRANSFER', p_PartID, p_OperationID, p_FromLocationID, p_ToLocationID,
        p_Quantity, p_ReferenceNumber, p_Notes, p_UserID
    );

    COMMIT;
    SET p_Success = TRUE;
    SET p_Message = CONCAT('TRANSFER completed: Moved ', p_Quantity, ' units from ', p_FromLocationID, ' to ', p_ToLocationID);
END$$
DELIMITER ;

-- =====================================================
-- User Management Procedures
-- =====================================================

-- User Authentication
DELIMITER $$
CREATE PROCEDURE sp_user_Authenticate(
    IN p_Username VARCHAR(100),
    OUT p_UserID VARCHAR(50),
    OUT p_FullName VARCHAR(255),
    OUT p_Role VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_user_Authenticate', p_Username);
    END;

    SELECT user_id, full_name, role
    INTO p_UserID, p_FullName, p_Role
    FROM users
    WHERE username = p_Username 
        AND active_status = TRUE;

    IF p_UserID IS NOT NULL THEN
        -- Update last login
        UPDATE users 
        SET last_login = NOW() 
        WHERE user_id = p_UserID;
        
        SET p_Success = TRUE;
        SET p_Message = 'Authentication successful';
    ELSE
        SET p_Success = FALSE;
        SET p_Message = 'Invalid username or user not active';
    END IF;
END$$
DELIMITER ;

-- =====================================================
-- System Utility Procedures
-- =====================================================

-- Get System Configuration
DELIMITER $$
CREATE PROCEDURE sp_system_Get_Configuration(
    IN p_ConfigKey VARCHAR(100),
    IN p_UserID VARCHAR(50),
    OUT p_ConfigValue TEXT,
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_system_Get_Configuration', p_UserID);
    END;

    SELECT config_value
    INTO p_ConfigValue
    FROM system_configuration
    WHERE config_key = p_ConfigKey;

    IF p_ConfigValue IS NOT NULL THEN
        SET p_Success = TRUE;
        SET p_Message = 'Configuration retrieved successfully';
    ELSE
        SET p_Success = FALSE;
        SET p_Message = CONCAT('Configuration key not found: ', p_ConfigKey);
    END IF;
END$$
DELIMITER ;

-- Log Error
DELIMITER $$
CREATE PROCEDURE sp_error_Log_Exception(
    IN p_ErrorMessage TEXT,
    IN p_StackTrace TEXT,
    IN p_ProcedureName VARCHAR(255),
    IN p_UserID VARCHAR(50),
    IN p_SeverityLevel VARCHAR(20),
    IN p_AdditionalInfo JSON,
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
    END;

    INSERT INTO error_log (
        error_message, stack_trace, procedure_name, user_id, 
        severity_level, additional_info
    ) VALUES (
        p_ErrorMessage, p_StackTrace, p_ProcedureName, p_UserID,
        COALESCE(p_SeverityLevel, 'Error'), p_AdditionalInfo
    );

    SET p_Success = TRUE;
    SET p_Message = 'Error logged successfully';
END$$
DELIMITER ;

-- =====================================================
-- Transaction History and Reporting
-- =====================================================

-- Get Transaction History
DELIMITER $$
CREATE PROCEDURE sp_transaction_Get_History(
    IN p_PartID VARCHAR(50),
    IN p_DaysBack INT,
    IN p_UserID VARCHAR(50),
    OUT p_Success BOOLEAN,
    OUT p_Message VARCHAR(500)
)
BEGIN
    DECLARE v_StartDate DATETIME;
    
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        GET DIAGNOSTICS CONDITION 1
            p_Message = MESSAGE_TEXT;
        SET p_Success = FALSE;
        INSERT INTO error_log (error_message, procedure_name, user_id)
        VALUES (p_Message, 'sp_transaction_Get_History', p_UserID);
    END;

    SET v_StartDate = DATE_SUB(NOW(), INTERVAL COALESCE(p_DaysBack, 30) DAY);

    SELECT 
        t.transaction_id,
        t.transaction_type,
        t.part_id,
        p.part_description,
        t.operation_id,
        o.operation_description,
        t.from_location_id,
        fl.location_description as from_location_description,
        t.to_location_id,
        tl.location_description as to_location_description,
        t.quantity,
        t.unit_cost,
        t.reference_number,
        t.notes,
        t.user_id,
        u.full_name as user_name,
        t.transaction_date
    FROM inventory_transactions t
        INNER JOIN parts p ON t.part_id = p.part_id
        INNER JOIN operations o ON t.operation_id = o.operation_id
        LEFT JOIN locations fl ON t.from_location_id = fl.location_id
        LEFT JOIN locations tl ON t.to_location_id = tl.location_id
        INNER JOIN users u ON t.user_id = u.user_id
    WHERE (p_PartID IS NULL OR t.part_id = p_PartID)
        AND t.transaction_date >= v_StartDate
    ORDER BY t.transaction_date DESC
    LIMIT 1000;

    SET p_Success = TRUE;
    SET p_Message = 'Transaction history retrieved successfully';
END$$
DELIMITER ;

-- =====================================================
-- End of Existing Stored Procedures
-- =====================================================
-- ?? FOR MODIFICATIONS:
-- - New procedures: Add to Development/Database_Files/New_Stored_Procedures.sql
-- - Update existing: Copy to Development/Database_Files/Updated_Stored_Procedures.sql and modify
-- 
-- Related files:
-- - Development/Database_Files/New_Stored_Procedures.sql
-- - Development/Database_Files/Updated_Stored_Procedures.sql
-- - Database_Files/Production_Database_Schema.sql