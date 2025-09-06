-- ==========================================
-- MTM QuickButtons Database Setup Script
-- FOR DATABASE: mtm_wip_application_test
-- ==========================================
-- This script creates the complete QuickButtons infrastructure
-- Execute this script on your mtm_wip_application_test database

USE mtm_wip_application_test;

-- ==========================================
-- STEP 1: Create qb_quickbuttons table
-- ==========================================
DROP TABLE IF EXISTS qb_quickbuttons;

CREATE TABLE qb_quickbuttons (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    User VARCHAR(50) NOT NULL COLLATE utf8mb4_unicode_ci,
    Position INT NOT NULL,
    PartID VARCHAR(100) NOT NULL COLLATE utf8mb4_unicode_ci,
    Operation VARCHAR(20) NOT NULL COLLATE utf8mb4_unicode_ci,
    Quantity INT NOT NULL,
    Location VARCHAR(100) NOT NULL COLLATE utf8mb4_unicode_ci,
    ItemType VARCHAR(20) NOT NULL DEFAULT 'WIP' COLLATE utf8mb4_unicode_ci,
    Created_Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY unique_user_position (User, Position),
    INDEX idx_user (User),
    INDEX idx_position (Position)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ==========================================
-- STEP 2: Create/Replace stored procedures
-- ==========================================

-- Procedure: qb_quickbuttons_Get_ByUser
DELIMITER $$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Get_ByUser$$
CREATE PROCEDURE qb_quickbuttons_Get_ByUser(
    IN p_UserID VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SELECT -1 AS Status, 'Database error occurred' AS Message;
    END;

    -- Return data FIRST (MTM pattern)
    SELECT 
        User,
        Position,
        PartID,
        Operation,
        Quantity,
        Location,
        ItemType,
        Created_Date
    FROM qb_quickbuttons 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci
    ORDER BY Position ASC;
    
    -- Return status LAST (MTM pattern)
    SELECT 1 AS Status, 'Success' AS Message;
END$$
DELIMITER ;

-- Procedure: qb_quickbuttons_Save
DELIMITER $$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Save$$
CREATE PROCEDURE qb_quickbuttons_Save(
    IN p_UserID VARCHAR(50),
    IN p_Position INT,
    IN p_PartID VARCHAR(100),
    IN p_Location VARCHAR(100),
    IN p_Operation VARCHAR(20),
    IN p_Quantity INT,
    IN p_ItemType VARCHAR(20)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SELECT -1 AS Status, 'Database error occurred' AS Message;
    END;

    START TRANSACTION;

    -- Insert or update the quick button
    INSERT INTO qb_quickbuttons (
        User, Position, PartID, Operation, Quantity, Location, ItemType
    ) VALUES (
        p_UserID, p_Position, p_PartID, p_Operation, p_Quantity, p_Location, p_ItemType
    )
    ON DUPLICATE KEY UPDATE
        PartID = VALUES(PartID),
        Operation = VALUES(Operation),
        Quantity = VALUES(Quantity),
        Location = VALUES(Location),
        ItemType = VALUES(ItemType),
        Created_Date = CURRENT_TIMESTAMP;

    COMMIT;
    
    -- Return success status (MTM pattern)
    SELECT 1 AS Status, 'Quick button saved successfully' AS Message;
END$$
DELIMITER ;

-- Procedure: qb_quickbuttons_Remove
DELIMITER $$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Remove$$
CREATE PROCEDURE qb_quickbuttons_Remove(
    IN p_UserID VARCHAR(50),
    IN p_Position INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SELECT -1 AS Status, 'Database error occurred' AS Message;
    END;

    START TRANSACTION;

    DELETE FROM qb_quickbuttons 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci 
      AND Position = p_Position;

    COMMIT;
    
    -- Return success status (MTM pattern)
    SELECT 1 AS Status, 'Quick button removed successfully' AS Message;
END$$
DELIMITER ;

-- Procedure: qb_quickbuttons_Clear_ByUser
DELIMITER $$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Clear_ByUser$$
CREATE PROCEDURE qb_quickbuttons_Clear_ByUser(
    IN p_UserID VARCHAR(50)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SELECT -1 AS Status, 'Database error occurred' AS Message;
    END;

    START TRANSACTION;

    DELETE FROM qb_quickbuttons 
    WHERE User COLLATE utf8mb4_unicode_ci = p_UserID COLLATE utf8mb4_unicode_ci;

    COMMIT;
    
    -- Return success status (MTM pattern)
    SELECT 1 AS Status, 'All quick buttons cleared successfully' AS Message;
END$$
DELIMITER ;

-- ==========================================
-- STEP 3: Insert test data for user JOHNK
-- ==========================================
INSERT INTO qb_quickbuttons (User, Position, PartID, Operation, Quantity, Location, ItemType) VALUES
('JOHNK', 1, '21-28841-006', '90', 100, 'R-A0-39', 'WIP'),
('JOHNK', 2, '21-28841-007', '40', 200, 'R-B1-15', 'WIP'),
('JOHNK', 3, '21-28841-008', '20', 300, 'R-C2-25', 'WIP'),
('JOHNK', 4, '21094864', '15', 14, 'R-A0-39', 'WIP'),
('JOHNK', 5, 'TEST-PART-001', '90', 5, 'R-B1-15', 'WIP')
ON DUPLICATE KEY UPDATE
    PartID = VALUES(PartID),
    Operation = VALUES(Operation),
    Quantity = VALUES(Quantity),
    Location = VALUES(Location),
    ItemType = VALUES(ItemType),
    Created_Date = CURRENT_TIMESTAMP;

-- ==========================================
-- STEP 4: Verification queries
-- ==========================================
SELECT '=== TABLE STRUCTURE ===' AS Info;
DESCRIBE qb_quickbuttons;

SELECT '=== TEST DATA ===' AS Info;
SELECT * FROM qb_quickbuttons WHERE User = 'JOHNK' ORDER BY Position;

SELECT '=== STORED PROCEDURES ===' AS Info;
SHOW PROCEDURE STATUS WHERE Db = 'mtm_wip_application_test' AND Name LIKE 'qb_%';

-- ==========================================
-- STEP 5: Test stored procedures
-- ==========================================
SELECT '=== TESTING qb_quickbuttons_Get_ByUser ===' AS Info;
CALL qb_quickbuttons_Get_ByUser('JOHNK');

SELECT '=== SETUP COMPLETE ===' AS Info;
SELECT 'QuickButtons database setup completed successfully!' AS Message;
SELECT 'Updated stored procedures to follow MTM pattern: data first, then status' AS Note;
