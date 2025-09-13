-- Fix definer issue for qb_quickbuttons_Get_ByUser
DROP PROCEDURE IF EXISTS `qb_quickbuttons_Get_ByUser`;

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `qb_quickbuttons_Get_ByUser`(
    IN p_User VARCHAR(50),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION
    BEGIN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Database error occurred while retrieving quick buttons';
        ROLLBACK;
    END;

    SET p_Status = -1;
    SET p_ErrorMsg = '';

    START TRANSACTION;

    IF p_User IS NULL OR TRIM(p_User) = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'User parameter is required';
        ROLLBACK;
    ELSE
        SELECT
            ID,
            User,
            Position,
            PartID,
            Operation,
            Quantity,
            Location,
            ItemType,
            DateCreated,
            DateModified
        FROM qb_quickbuttons
        WHERE User = p_User
        ORDER BY Position;

        IF FOUND_ROWS() > 0 THEN
            SET p_Status = 1;
            SET p_ErrorMsg = CONCAT('Retrieved ', FOUND_ROWS(), ' quick buttons successfully');
        ELSE
            SET p_Status = 0;
            SET p_ErrorMsg = 'No quick buttons found for user';
        END IF;

        COMMIT;
    END IF;
END$$
DELIMITER ;
