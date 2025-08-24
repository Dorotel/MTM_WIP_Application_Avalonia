-- =====================================================
-- MTM WIP Application - Updated Development Stored Procedures
-- =====================================================
-- Environment: Development
-- Status: EDITABLE - Modified versions of existing procedures with standardized output parameters
-- Last Updated: Standardized error handling implementation
-- =====================================================

-- ?? PURPOSE: This file contains modified versions of existing procedures with standardized output parameters
-- 
-- STANDARDIZATION PATTERN:
-- - All procedures now include: OUT p_Status INT, OUT p_ErrorMsg VARCHAR(255)
-- - Status codes: 0=success, 1=warning, -1=error
-- - Comprehensive input validation
-- - Transaction management where appropriate
-- - Consistent error handling

USE mtm_wip_application;

-- =====================================================
-- Inventory Management Procedures - UPDATED
-- =====================================================

-- Get Inventory by Part ID - STANDARDIZED
DELIMITER ;;
CREATE PROCEDURE sp_inventory_Get_ByPartID_Standardized(
    IN p_PartID VARCHAR(50),
    IN p_UserID VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            p_Status = MYSQL_ERRNO,
            p_ErrorMsg = MESSAGE_TEXT;
        SET p_Status = -1;
        
        INSERT INTO error_log (error_message, procedure_name, user_id, severity_level)
        VALUES (p_ErrorMsg, 'sp_inventory_Get_ByPartID_Standardized', COALESCE(p_UserID, 'system'), 'Error');
    END;

    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'PartID is required and cannot be empty';
        LEAVE sp_inventory_Get_ByPartID_Standardized;
    END IF;

    IF p_UserID IS NULL OR p_UserID = '' THEN
        SET p_Status = 1;
        SET p_ErrorMsg = 'UserID is required and cannot be empty';
        LEAVE sp_inventory_Get_ByPartID_Standardized;
    END IF;

    -- Validate PartID exists in master data
    IF NOT EXISTS (SELECT 1 FROM parts WHERE part_id = p_PartID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('PartID "', p_PartID, '" does not exist in master data or is inactive');
        LEAVE sp_inventory_Get_ByPartID_Standardized;
    END IF;

    -- Validate UserID exists
    IF NOT EXISTS (SELECT 1 FROM users WHERE user_id = p_UserID AND active_status = TRUE) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('UserID "', p_UserID, '" does not exist or is inactive');
        LEAVE sp_inventory_Get_ByPartID_Standardized;
    END IF;

    SET @current_user_id = p_UserID;

    -- Execute query with error handling
    BEGIN
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

        SET p_Status = 0;
        SET p_ErrorMsg = 'Inventory retrieved successfully';
    END;
END;;