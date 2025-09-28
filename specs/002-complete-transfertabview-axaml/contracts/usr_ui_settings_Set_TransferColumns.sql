-- usr_ui_settings_Set_TransferColumns.sql
-- Saves transfer tab column configuration for a specific user

DELIMITER $$

DROP PROCEDURE IF EXISTS usr_ui_settings_Set_TransferColumns$$

CREATE PROCEDURE usr_ui_settings_Set_TransferColumns(
    IN p_UserId VARCHAR(64),
    IN p_ColumnConfig JSON
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Validate that the JSON contains required structure
    IF JSON_VALID(p_ColumnConfig) = 0 OR
       JSON_EXTRACT(p_ColumnConfig, '$.VisibleColumns') IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Invalid column configuration JSON structure';
    END IF;

    -- Insert or update the user settings with TransferTabColumns configuration
    INSERT INTO usr_ui_settings (UserId, SettingsJson, UpdatedAt)
    VALUES (
        p_UserId,
        JSON_OBJECT('TransferTabColumns', p_ColumnConfig),
        CURRENT_TIMESTAMP
    )
    ON DUPLICATE KEY UPDATE
        SettingsJson = JSON_SET(
            COALESCE(SettingsJson, JSON_OBJECT()),
            '$.TransferTabColumns',
            p_ColumnConfig
        ),
        UpdatedAt = CURRENT_TIMESTAMP;

    -- Return success status
    SELECT 1 as Status, 'Column configuration saved successfully' as Message;

    COMMIT;
END$$

DELIMITER ;
