using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services.Interfaces
{
    /// <summary>
    /// Service interface for transfer operations with quantity splitting logic.
    /// Follows MTM service patterns with ServiceResult return types.
    /// </summary>
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
        Task<ServiceResult<TransferValidationResult>> ValidateTransferAsync(TransferOperation transfer);

        /// <summary>
        /// Get list of valid destination locations
        /// </summary>
        /// <returns>List of location identifiers</returns>
        Task<ServiceResult<List<string>>> GetValidLocationsAsync();
    }

    /// <summary>
    /// Service interface for column configuration persistence using MySQL usr_ui_settings table.
    /// Specific to TransferTabView DataGrid column customization.
    /// </summary>
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
}
