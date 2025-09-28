-- usr_ui_settings_Get_TransferColumns.sql
-- Retrieves transfer tab column configuration for a specific user

DELIMITER $$

DROP PROCEDURE IF EXISTS usr_ui_settings_Get_TransferColumns$$

CREATE PROCEDURE usr_ui_settings_Get_TransferColumns(
    IN p_UserId VARCHAR(64)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    -- Get the TransferTabColumns configuration from SettingsJson
    SELECT
        p_UserId as UserId,
        COALESCE(
            JSON_EXTRACT(SettingsJson, '$.TransferTabColumns'),
            JSON_OBJECT(
                'VisibleColumns', JSON_ARRAY('PartID', 'Operation', 'FromLocation', 'AvailableQuantity', 'TransferQuantity', 'Notes'),
                'ColumnOrder', JSON_OBJECT(
                    'PartID', 0, 'Operation', 1, 'FromLocation', 2,
                    'AvailableQuantity', 3, 'TransferQuantity', 4, 'Notes', 5
                ),
                'ColumnWidths', JSON_OBJECT(
                    'PartID', 120, 'Operation', 80, 'FromLocation', 100,
                    'AvailableQuantity', 120, 'TransferQuantity', 120, 'Notes', 200
                )
            )
        ) as ColumnConfig,
        COALESCE(UpdatedAt, CURRENT_TIMESTAMP) as LastModified
    FROM usr_ui_settings
    WHERE UserId = p_UserId

    UNION ALL

    -- If no record exists, return default configuration
    SELECT
        p_UserId as UserId,
        JSON_OBJECT(
            'VisibleColumns', JSON_ARRAY('PartID', 'Operation', 'FromLocation', 'AvailableQuantity', 'TransferQuantity', 'Notes'),
            'ColumnOrder', JSON_OBJECT(
                'PartID', 0, 'Operation', 1, 'FromLocation', 2,
                'AvailableQuantity', 3, 'TransferQuantity', 4, 'Notes', 5
            ),
            'ColumnWidths', JSON_OBJECT(
                'PartID', 120, 'Operation', 80, 'FromLocation', 100,
                'AvailableQuantity', 120, 'TransferQuantity', 120, 'Notes', 200
            )
        ) as ColumnConfig,
        CURRENT_TIMESTAMP as LastModified
    WHERE NOT EXISTS (
        SELECT 1 FROM usr_ui_settings WHERE UserId = p_UserId
    )

    LIMIT 1;

    COMMIT;
END$$

DELIMITER ;
