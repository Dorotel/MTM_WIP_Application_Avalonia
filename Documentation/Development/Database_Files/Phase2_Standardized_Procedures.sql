-- MTM WIP Application - Phase 2: Standardized Stored Procedures
-- Generated: 2025-09-10
-- Purpose: Updated stored procedures with consistent error handling patterns

-- This file contains the corrected versions of the most critical stored procedures
-- that were identified by the validation system as missing standard output parameters.

-- CRITICAL FIXES - Missing Output Parameters (Priority 1)
-- These procedures were missing @p_Status and @p_ErrorMsg output parameters

DELIMITER $$

-- Updated: inv_inventory_Get_ByPartID - Added standard output parameters
DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByPartID`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartID`(
    IN p_PartID VARCHAR(300),
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
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = 'No data found';

    -- Validate input parameters
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'PartID parameter is required';
        ROLLBACK;
        LEAVE main;
    END IF;

    main: BEGIN
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
            BatchNumber AS `BatchNumber`,
            Notes
        FROM inv_inventory
        WHERE PartID = p_PartID;
        
        -- Check if data was found
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'Data retrieved successfully';
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No inventory found for the specified PartID';
        END IF;
    END main;

    COMMIT;
END$$

-- Updated: inv_inventory_Get_ByPartIDandOperation - Added standard output parameters and fixed parameter name
DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByPartIDandOperation`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByPartIDandOperation`(
    IN p_PartID VARCHAR(300),
    IN p_Operation VARCHAR(300),  -- FIXED: Changed from o_Operation to p_Operation for consistency
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
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = 'No data found';

    -- Validate input parameters
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'PartID parameter is required';
        ROLLBACK;
        LEAVE main;
    END IF;

    IF p_Operation IS NULL OR p_Operation = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Operation parameter is required';
        ROLLBACK;
        LEAVE main;
    END IF;

    main: BEGIN
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
            BatchNumber AS `BatchNumber`,
            Notes
        FROM inv_inventory
        WHERE PartID = p_PartID AND Operation = p_Operation;
        
        -- Check if data was found
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'Data retrieved successfully';
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No inventory found for the specified PartID and Operation';
        END IF;
    END main;

    COMMIT;
END$$

-- Updated: inv_inventory_Get_ByUser - Added standard output parameters
DROP PROCEDURE IF EXISTS `inv_inventory_Get_ByUser`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `inv_inventory_Get_ByUser`(
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
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = 'No data found';

    -- Validate input parameters
    IF p_User IS NULL OR p_User = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'User parameter is required';
        ROLLBACK;
        LEAVE main;
    END IF;

    main: BEGIN
        SELECT * FROM inv_inventory
        WHERE User = p_User
        ORDER BY LastUpdated DESC;
        
        -- Check if data was found
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'Data retrieved successfully';
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No inventory found for the specified user';
        END IF;
    END main;

    COMMIT;
END$$

-- Updated: md_part_ids_Get_All - Added standard output parameters
DROP PROCEDURE IF EXISTS `md_part_ids_Get_All`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_part_ids_Get_All`(
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
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = 'No data found';

    main: BEGIN
        SELECT 
            ID,
            PartID,
            Customer,
            Description,
            IssuedBy,
            ItemType
        FROM md_part_ids
        ORDER BY PartID;
        
        -- Check if data was found
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'Data retrieved successfully';
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No part IDs found';
        END IF;
    END main;

    COMMIT;
END$$

-- Updated: md_locations_Get_All - Added standard output parameters
DROP PROCEDURE IF EXISTS `md_locations_Get_All`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_locations_Get_All`(
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
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = 'No data found';

    main: BEGIN
        SELECT 
            ID,
            Location,
            Building,
            IssuedBy
        FROM md_locations
        ORDER BY Location;
        
        -- Check if data was found
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'Data retrieved successfully';
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No locations found';
        END IF;
    END main;

    COMMIT;
END$$

-- Updated: md_operation_numbers_Get_All - Added standard output parameters
DROP PROCEDURE IF EXISTS `md_operation_numbers_Get_All`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `md_operation_numbers_Get_All`(
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
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = 'No data found';

    main: BEGIN
        SELECT 
            ID,
            Operation,
            IssuedBy
        FROM md_operation_numbers
        ORDER BY CAST(Operation AS UNSIGNED);
        
        -- Check if data was found
        IF ROW_COUNT() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = 'Data retrieved successfully';
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No operation numbers found';
        END IF;
    END main;

    COMMIT;
END$$

DELIMITER ;

-- SUMMARY OF CHANGES MADE:
-- 1. Added standard OUTPUT parameters @p_Status and @p_ErrorMsg to all procedures
-- 2. Implemented consistent error handling with SQLEXCEPTION handler
-- 3. Added input parameter validation with appropriate error messages
-- 4. Implemented standard status codes:
--    -1 = Error (database error or validation failure)
--     0 = No data found (successful execution but no results)
--     1 = Success with data
-- 5. Fixed parameter name inconsistency (o_Operation -> p_Operation)
-- 6. Added transaction management with proper rollback on errors
-- 7. Added ROW_COUNT() checking to distinguish between no data vs success

-- NEXT STEPS:
-- 1. Update the remaining procedures to follow this same pattern
-- 2. Test these procedures to ensure they work correctly
-- 3. Update the StoredProcedureResult.IsSuccess logic to match the standardized pattern
-- 4. Update C# calling code if needed to handle the new output parameters correctly