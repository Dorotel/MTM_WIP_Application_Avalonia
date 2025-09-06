-- COMPLETE QUICKBUTTONS FIX
-- Addresses collation issues and parameter mismatches

USE mtm_wip_application_test;

-- Fix qb_quickbuttons_Get_ByUser stored procedure
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Get_ByUser`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Get_ByUser`(
    IN p_UserID VARCHAR(100)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        RESIGNAL;
    END;

    -- Use explicit collation to prevent collation mismatch
    SELECT * FROM qb_quickbuttons 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci 
    ORDER BY Position;
END$$
DELIMITER ;

-- Fix qb_quickbuttons_Save stored procedure to use p_UserID parameter name
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Save`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Save`(
    IN p_UserID VARCHAR(100),
    IN p_Position INT,
    IN p_PartID VARCHAR(300),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(100),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(100),
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

    -- Use explicit collation to prevent collation mismatch
    INSERT INTO qb_quickbuttons (User, Position, PartID, Location, Operation, Quantity, ItemType, DateCreated)
    VALUES (p_UserID, p_Position, p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, NOW())
    ON DUPLICATE KEY UPDATE
        PartID = VALUES(PartID),
        Location = VALUES(Location),
        Operation = VALUES(Operation),
        Quantity = VALUES(Quantity),
        ItemType = VALUES(ItemType),
        DateModified = NOW();

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quick button saved successfully';
    COMMIT;
END$$
DELIMITER ;

-- Fix qb_quickbuttons_Remove stored procedure
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Remove`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Remove`(
    IN p_UserID VARCHAR(100),
    IN p_Position INT,
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

    -- Use explicit collation to prevent collation mismatch
    DELETE FROM qb_quickbuttons 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci 
    AND Position = p_Position;

    SET p_Status = 0;
    SET p_ErrorMsg = 'Quick button removed successfully';
    COMMIT;
END$$
DELIMITER ;

-- Fix qb_quickbuttons_Clear_ByUser stored procedure
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Clear_ByUser`;
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Clear_ByUser`(
    IN p_UserID VARCHAR(100),
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

    -- Use explicit collation to prevent collation mismatch
    DELETE FROM qb_quickbuttons 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci;

    SET p_Status = 0;
    SET p_ErrorMsg = 'All quick buttons cleared successfully';
    COMMIT;
END$$
DELIMITER ;
