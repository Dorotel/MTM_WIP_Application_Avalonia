-- Fix all QuickButton procedures to use consistent Status=1 for success pattern
-- This ensures compatibility between office and home databases

-- Fix Remove procedure
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Remove`;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Remove`
(
    IN p_User VARCHAR
(50),
    IN p_Position INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR
(255)
)
BEGIN
    DECLARE v_rows_affected INT;
DECLARE v_errno INT;
DECLARE v_msg TEXT;

DECLARE EXIT HANDLER FOR SQLEXCEPTION
BEGIN
GET DIAGNOSTICS CONDITION 1
            v_errno = MYSQL_ERRNO,
            v_msg = MESSAGE_TEXT;
ROLLBACK;
SET p_Status
= -1;
SET p_ErrorMsg
= CONCAT
('MySQL Error ', v_errno, ': ', v_msg);
END;

SET p_Status
= -1;
SET p_ErrorMsg
= 'Unknown error';

    START TRANSACTION;

IF p_User IS NULL OR TRIM(p_User) = '' THEN
SET p_Status
= -1;
SET p_ErrorMsg
= 'User parameter is required';
ROLLBACK;
ELSEIF p_Position IS NULL OR p_Position < 1 OR p_Position > 10 THEN
SET p_Status
= -1;
SET p_ErrorMsg
= 'Position must be between 1 and 10';
ROLLBACK;
ELSE
DELETE FROM qb_quickbuttons WHERE User = p_User AND Position = p_Position;
SET v_rows_affected
= ROW_COUNT
();

IF v_rows_affected > 0 THEN
SET p_Status
= 1;
-- SUCCESS WITH ACTION
SET p_ErrorMsg
= CONCAT
('Quick button removed from position ', p_Position);
        ELSE
SET p_Status
= 0;
-- SUCCESS NO ACTION (nothing to remove)
SET p_ErrorMsg
= CONCAT
('No quick button found at position ', p_Position);
END
IF;

        COMMIT;
END
IF;
END$$

-- Fix Clear procedure
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Clear_ByUser`;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Clear_ByUser`
(
    IN p_User VARCHAR
(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR
(255)
)
BEGIN
    DECLARE v_rows_affected INT;
DECLARE v_errno INT;
DECLARE v_msg TEXT;

DECLARE EXIT HANDLER FOR SQLEXCEPTION
BEGIN
GET DIAGNOSTICS CONDITION 1
            v_errno = MYSQL_ERRNO,
            v_msg = MESSAGE_TEXT;
ROLLBACK;
SET p_Status
= -1;
SET p_ErrorMsg
= CONCAT
('MySQL Error ', v_errno, ': ', v_msg);
END;

SET p_Status
= -1;
SET p_ErrorMsg
= 'Unknown error';

    START TRANSACTION;

IF p_User IS NULL OR TRIM(p_User) = '' THEN
SET p_Status
= -1;
SET p_ErrorMsg
= 'User parameter is required';
ROLLBACK;
ELSE
DELETE FROM qb_quickbuttons WHERE User = p_User;
SET v_rows_affected
= ROW_COUNT
();

IF v_rows_affected > 0 THEN
SET p_Status
= 1;
-- SUCCESS WITH ACTION
SET p_ErrorMsg
= CONCAT
('Cleared ', v_rows_affected, ' quick buttons for user');
        ELSE
SET p_Status
= 0;
-- SUCCESS NO ACTION (nothing to clear)
SET p_ErrorMsg
= 'No quick buttons found to clear';
END
IF;

        COMMIT;
END
IF;
END$$
DELIMITER ;

-- Test all procedures
CALL qb_quickbuttons_Save
('JKOLL', 3, 'TEST-PART', '100', 5, 'SHIPPING', 'WIP', @status, @msg);
SELECT 'Save Test' as Test, @status as Status, @msg as Message;

CALL qb_quickbuttons_Get_ByUser
('JKOLL', @status, @msg);
SELECT 'Get Test' as Test, @status as Status, @msg as Message;

CALL qb_quickbuttons_Remove
('JKOLL', 3, @status, @msg);
SELECT 'Remove Test' as Test, @status as Status, @msg as Message;

CALL qb_quickbuttons_Clear_ByUser
('JKOLL', @status, @msg);
SELECT 'Clear Test' as Test, @status as Status, @msg as Message;
