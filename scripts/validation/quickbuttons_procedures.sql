-- Deployment script for QuickButtons related objects
-- MySQL 5.7 COMPATIBLE VERSION (no GET DIAGNOSTICS usage)
-- MTM Status Pattern: 1 = Success (data/row affected), 0 = Success (no data), -1 = Error
-- Run in target database (e.g., mtm_wip_application) before executing validation script.

DELIMITER $$

-- =============================================
-- Table Definition (if not exists)
-- =============================================
CREATE TABLE IF NOT EXISTS qb_quickbuttons (
  ID INT AUTO_INCREMENT PRIMARY KEY,
  User VARCHAR(50) NOT NULL,
  Position INT NOT NULL,
  PartID VARCHAR(100) NOT NULL,
  Operation VARCHAR(20) NOT NULL,
  Quantity INT NOT NULL DEFAULT 0,
  Location VARCHAR(100) NOT NULL DEFAULT '',
  ItemType VARCHAR(20) NOT NULL DEFAULT 'WIP',
  Created_Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  DateModified TIMESTAMP NULL ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_qb_user_position (User, Position),
  KEY idx_qb_user (User)
)$$

-- =============================================
-- DROP existing procedures (idempotent rebuild)
-- =============================================
DROP PROCEDURE IF EXISTS qb_quickbuttons_Get_ByUser$$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Save$$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Remove$$
DROP PROCEDURE IF EXISTS qb_quickbuttons_Clear_ByUser$$

-- =============================================
-- qb_quickbuttons_Get_ByUser
-- Returns all quick buttons for a user ordered by Position
-- Status Codes: 0=Success (may have 0 rows); -1=Error
-- =============================================
CREATE PROCEDURE qb_quickbuttons_Get_ByUser(
  IN p_User VARCHAR(50),
  OUT p_Status INT,
  OUT p_ErrorMsg VARCHAR(500)
)
BEGIN
  DECLARE v_rowcount INT DEFAULT 0;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    SET p_Status = -1;
    SET p_ErrorMsg = 'SQL exception in qb_quickbuttons_Get_ByUser';
  END;

  SET p_Status = 0;
  SET p_ErrorMsg = '';

  SELECT ID, Position, PartID, Operation, Quantity, Location, ItemType, Created_Date, DateModified
  FROM qb_quickbuttons
  WHERE `User` = p_User
  ORDER BY Position ASC;

  SELECT COUNT(*) INTO v_rowcount FROM qb_quickbuttons WHERE `User` = p_User;
  IF v_rowcount > 0 THEN
    SET p_Status = 1; -- success with data
  END IF;
END$$

-- =============================================
-- qb_quickbuttons_Save
-- Inserts or Updates a quick button for a user at a given position.
-- Enforces Position range 1..10, Quantity>0, non-empty PartID.
-- If record exists at Position it is replaced.
-- Status Codes: 0=Success, -1=Validation/SQL Error
-- =============================================
CREATE PROCEDURE qb_quickbuttons_Save(
  IN p_User VARCHAR(50),
  IN p_Position INT,
  IN p_PartID VARCHAR(100),
  IN p_Operation VARCHAR(20),
  IN p_Quantity INT,
  IN p_Location VARCHAR(100),
  IN p_ItemType VARCHAR(50), -- Reserved for future use / compatibility
  OUT p_Status INT,
  OUT p_ErrorMsg VARCHAR(500)
)
proc: BEGIN
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    ROLLBACK;
    SET p_Status = -1;
    SET p_ErrorMsg = 'SQL exception in qb_quickbuttons_Save';
  END;

  -- Input Validation
  IF p_Position < 1 OR p_Position > 10 THEN
    SET p_Status = -1;
    SET p_ErrorMsg = 'Position must be between 1 and 10';
    LEAVE proc;
  END IF;
  IF p_PartID IS NULL OR LENGTH(TRIM(p_PartID)) = 0 THEN
    SET p_Status = -1;
    SET p_ErrorMsg = 'PartID cannot be empty';
    LEAVE proc;
  END IF;
  IF p_Quantity <= 0 THEN
    SET p_Status = -1;
    SET p_ErrorMsg = 'Quantity must be greater than 0';
    LEAVE proc;
  END IF;

  START TRANSACTION;
    DELETE FROM qb_quickbuttons WHERE `User` = p_User AND Position = p_Position;
    INSERT INTO qb_quickbuttons(`User`, Position, PartID, Operation, Quantity, Location, ItemType)
      VALUES(p_User, p_Position, p_PartID, p_Operation, p_Quantity, p_Location, p_ItemType);
  COMMIT;

  SET p_Status = 1; -- success (row affected)
  SET p_ErrorMsg = '';
END$$

-- =============================================
-- qb_quickbuttons_Remove
-- Removes a quick button at a given position (idempotent).
-- Status Codes: 0=Success, -1=Error
-- =============================================
CREATE PROCEDURE qb_quickbuttons_Remove(
  IN p_User VARCHAR(50),
  IN p_Position INT,
  OUT p_Status INT,
  OUT p_ErrorMsg VARCHAR(500)
)
BEGIN
  DECLARE v_rows INT DEFAULT 0;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    SET p_Status = -1;
    SET p_ErrorMsg = 'SQL exception in qb_quickbuttons_Remove';
  END;

  DELETE FROM qb_quickbuttons WHERE `User` = p_User AND Position = p_Position;
  SET v_rows = ROW_COUNT();
  IF v_rows > 0 THEN
    SET p_Status = 1; -- success (row removed)
  ELSE
    SET p_Status = 0; -- success, nothing to remove
  END IF;
  SET p_ErrorMsg = '';
END$$

-- =============================================
-- qb_quickbuttons_Clear_ByUser
-- Deletes all quick buttons for a user.
-- Status Codes: 0=Success, -1=Error
-- =============================================
CREATE PROCEDURE qb_quickbuttons_Clear_ByUser(
  IN p_User VARCHAR(50),
  OUT p_Status INT,
  OUT p_ErrorMsg VARCHAR(500)
)
BEGIN
  DECLARE v_rows INT DEFAULT 0;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    SET p_Status = -1;
    SET p_ErrorMsg = 'SQL exception in qb_quickbuttons_Clear_ByUser';
  END;

  DELETE FROM qb_quickbuttons WHERE `User` = p_User;
  SET v_rows = ROW_COUNT();
  IF v_rows > 0 THEN
    SET p_Status = 1; -- success (rows deleted)
  ELSE
    SET p_Status = 0; -- success (no rows existed)
  END IF;
  SET p_ErrorMsg = '';
END$$

DELIMITER ;

-- Verification queries (optional)
-- SHOW PROCEDURE STATUS WHERE Db = DATABASE() AND Name LIKE 'qb_quickbuttons%';
-- SELECT * FROM qb_quickbuttons WHERE User = 'QA_TEST_USER1';
