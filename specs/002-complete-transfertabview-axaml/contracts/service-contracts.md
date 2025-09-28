# Service Contracts: TransferTabView Implementation

## ITransferService Contract

```csharp
namespace MTM_WIP_Application_Avalonia.Services
{
    public interface ITransferService
    {
        /// <summary>
        /// Search inventory items available for transfer
        /// </summary>
        /// <param name="partId">Part identifier (optional)</param>
        /// <param name="operation">Operation number (optional)</param>
        /// <returns>List of available inventory items</returns>
        Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string? partId = null, string? operation = null);

        /// <summary>
        /// Execute transfer operation with quantity splitting logic
        /// </summary>
        /// <param name="transfer">Transfer operation details</param>
        /// <returns>Success/failure result with transaction details</returns>
        Task<ServiceResult<TransferResult>> ExecuteTransferAsync(TransferOperation transfer);

        /// <summary>
        /// Validate transfer operation before execution
        /// </summary>
        /// <param name="transfer">Transfer operation to validate</param>
        /// <returns>Validation result with error details if invalid</returns>
        Task<ServiceResult<ValidationResult>> ValidateTransferAsync(TransferOperation transfer);

        /// <summary>
        /// Get list of valid destination locations
        /// </summary>
        /// <returns>List of location identifiers</returns>
        Task<ServiceResult<List<string>>> GetValidLocationsAsync();
    }

    public class TransferResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public bool WasSplit { get; set; }
        public int OriginalQuantity { get; set; }
        public int TransferredQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
```

## IColumnConfigurationService Contract

```csharp
namespace MTM_WIP_Application_Avalonia.Services
{
    public interface IColumnConfigurationService
    {
        /// <summary>
        /// Load user's column configuration preferences
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Column configuration or default if not found</returns>
        Task<ServiceResult<ColumnConfiguration>> LoadColumnConfigAsync(string userId);

        /// <summary>
        /// Save user's column configuration preferences
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="config">Column configuration to save</param>
        /// <returns>Success/failure result</returns>
        Task<ServiceResult> SaveColumnConfigAsync(string userId, ColumnConfiguration config);

        /// <summary>
        /// Reset user preferences to default column configuration
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <returns>Success/failure result</returns>
        Task<ServiceResult> ResetToDefaultsAsync(string userId);

        /// <summary>
        /// Get default column configuration
        /// </summary>
        /// <returns>Default column settings</returns>
        ColumnConfiguration GetDefaultConfiguration();
    }

    public class ColumnConfiguration
    {
        public List<string> VisibleColumns { get; set; } = new();
        public Dictionary<string, int> ColumnOrder { get; set; } = new();
        public Dictionary<string, int> ColumnWidths { get; set; } = new();
        public DateTime LastModified { get; set; } = DateTime.Now;

        public static ColumnConfiguration Default => new()
        {
            VisibleColumns = new List<string> { "PartID", "Operation", "FromLocation", "AvailableQuantity", "TransferQuantity", "Notes" },
            ColumnOrder = new Dictionary<string, int>
            {
                { "PartID", 0 }, { "Operation", 1 }, { "FromLocation", 2 },
                { "AvailableQuantity", 3 }, { "TransferQuantity", 4 }, { "Notes", 5 }
            },
            ColumnWidths = new Dictionary<string, int>
            {
                { "PartID", 120 }, { "Operation", 80 }, { "FromLocation", 100 },
                { "AvailableQuantity", 120 }, { "TransferQuantity", 120 }, { "Notes", 200 }
            }
        };
    }
}
```

## Database Contract Specifications

### MySQL Stored Procedures

#### usr_ui_settings_Get_TransferColumns

```sql
DELIMITER $$
CREATE PROCEDURE usr_ui_settings_Get_TransferColumns(
    IN p_UserId VARCHAR(64)
)
BEGIN
    SELECT 
        JSON_EXTRACT(SettingsJson, '$.TransferTabColumns') as ColumnConfig
    FROM usr_ui_settings 
    WHERE UserId = p_UserId;
END$$
DELIMITER ;
```

#### usr_ui_settings_Set_TransferColumns

```sql
DELIMITER $$
CREATE PROCEDURE usr_ui_settings_Set_TransferColumns(
    IN p_UserId VARCHAR(64),
    IN p_ColumnConfig JSON
)
BEGIN
    INSERT INTO usr_ui_settings (UserId, SettingsJson, UpdatedAt)
    VALUES (p_UserId, JSON_OBJECT('TransferTabColumns', p_ColumnConfig), CURRENT_TIMESTAMP)
    ON DUPLICATE KEY UPDATE
        SettingsJson = JSON_SET(SettingsJson, '$.TransferTabColumns', p_ColumnConfig),
        UpdatedAt = CURRENT_TIMESTAMP;
END$$
DELIMITER ;
```

#### inv_transfer_Execute_WithSplit

```sql
DELIMITER $$
CREATE PROCEDURE inv_transfer_Execute_WithSplit(
    IN p_PartId VARCHAR(50),
    IN p_Operation VARCHAR(10),
    IN p_FromLocation VARCHAR(50),
    IN p_ToLocation VARCHAR(50),
    IN p_TransferQuantity INT,
    IN p_UserId VARCHAR(64),
    OUT p_TransactionId VARCHAR(36),
    OUT p_Status INT,
    OUT p_Message VARCHAR(500)
)
BEGIN
    -- Implementation details for transfer with quantity splitting
    -- Creates new inventory row at destination
    -- Updates source inventory quantity
    -- Records single transaction with split details
END$$
DELIMITER ;
```

## Error Handling Contracts

### ServiceResult Pattern

```csharp
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public int StatusCode { get; set; } = 200;

    public static ServiceResult Success(string message = "") => 
        new() { IsSuccess = true, Message = message };

    public static ServiceResult Failure(string message, Exception? ex = null) => 
        new() { IsSuccess = false, Message = message, Exception = ex, StatusCode = 500 };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Success(T data, string message = "") => 
        new() { IsSuccess = true, Data = data, Message = message };

    public static new ServiceResult<T> Failure(string message, Exception? ex = null) => 
        new() { IsSuccess = false, Message = message, Exception = ex, StatusCode = 500 };
}
```
