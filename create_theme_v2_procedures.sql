-- Theme V2 Database Stored Procedures
-- MTM WIP Application - Theme V2 Implementation
-- Creates user theme preference procedures following MTM database patterns

-- ====================================================
-- THEME PREFERENCE STORAGE PROCEDURES
-- ====================================================

-- Drop existing procedures if they exist
DROP PROCEDURE IF EXISTS `usr_theme_preferences_Get`;
DROP PROCEDURE IF EXISTS `usr_theme_preferences_Set`;

-- Create table for theme preferences if it doesn't exist
CREATE TABLE IF NOT EXISTS `usr_theme_preferences` (
    `ID` INT AUTO_INCREMENT PRIMARY KEY,
    `UserId` VARCHAR(50) NOT NULL,
    `ThemeName` VARCHAR(20) NOT NULL DEFAULT 'System',
    `CreatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `ModifiedDate` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY `unique_user_theme` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

DELIMITER $$

-- ====================================================
-- usr_theme_preferences_Get
-- Gets user's saved theme preference
-- ====================================================
CREATE PROCEDURE `usr_theme_preferences_Get`(
    IN p_UserId VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SELECT
        UserId,
        ThemeName,
        CreatedDate,
        ModifiedDate
    FROM usr_theme_preferences
    WHERE UserId = p_UserId;

    COMMIT;
END$$

-- ====================================================
-- usr_theme_preferences_Set
-- Sets user's theme preference
-- ====================================================
CREATE PROCEDURE `usr_theme_preferences_Set`(
    IN p_UserId VARCHAR(50),
    IN p_ThemeName VARCHAR(20)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Validate theme name
    IF p_ThemeName NOT IN ('Light', 'Dark', 'System') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Invalid theme name. Must be Light, Dark, or System.';
    END IF;

    -- Insert or update theme preference
    INSERT INTO usr_theme_preferences (UserId, ThemeName)
    VALUES (p_UserId, p_ThemeName)
    ON DUPLICATE KEY UPDATE
        ThemeName = p_ThemeName,
        ModifiedDate = CURRENT_TIMESTAMP;

    -- Return success indicator
    SELECT 1 as Status, 'Theme preference saved successfully' as Message;

    COMMIT;
END$$

-- ====================================================
-- usr_theme_preferences_Delete
-- Removes user's theme preference (resets to system default)
-- ====================================================
CREATE PROCEDURE `usr_theme_preferences_Delete`(
    IN p_UserId VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    DELETE FROM usr_theme_preferences
    WHERE UserId = p_UserId;

    -- Return success indicator
    SELECT 1 as Status, 'Theme preference reset successfully' as Message;

    COMMIT;
END$$

-- ====================================================
-- usr_theme_preferences_Get_All
-- Gets all user theme preferences (for administration)
-- ====================================================
CREATE PROCEDURE `usr_theme_preferences_Get_All`()
BEGIN
    SELECT
        UserId,
        ThemeName,
        CreatedDate,
        ModifiedDate
    FROM usr_theme_preferences
    ORDER BY UserId;
END$$

DELIMITER ;

-- ====================================================
-- VERIFICATION QUERIES
-- ====================================================

-- Test the procedures (uncomment to run)
/*
-- Test setting theme preference
CALL usr_theme_preferences_Set('TestUser', 'Dark');

-- Test getting theme preference
CALL usr_theme_preferences_Get('TestUser');

-- Test getting all preferences
CALL usr_theme_preferences_Get_All();

-- Test deleting preference
CALL usr_theme_preferences_Delete('TestUser');
*/

-- Grant permissions to application user (adjust as needed for your MTM database setup)
-- GRANT EXECUTE ON PROCEDURE usr_theme_preferences_Get TO 'mtm_app_user'@'%';
-- GRANT EXECUTE ON PROCEDURE usr_theme_preferences_Set TO 'mtm_app_user'@'%';
-- GRANT EXECUTE ON PROCEDURE usr_theme_preferences_Delete TO 'mtm_app_user'@'%';
-- GRANT EXECUTE ON PROCEDURE usr_theme_preferences_Get_All TO 'mtm_app_user'@'%';
