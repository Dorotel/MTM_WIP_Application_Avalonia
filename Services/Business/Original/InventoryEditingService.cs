using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_Shared_Logic.Models;
using InventoryItem = MTM_WIP_Application_Avalonia.Models.InventoryItem;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Interface for comprehensive inventory editing service
    /// Provides business logic for full inventory item editing with validation
    /// </summary>
    public interface IInventoryEditingService
    {
        /// <summary>
        /// Loads an inventory item for editing by ID
        /// </summary>
        /// <param name="inventoryId">The inventory item ID</param>
        /// <returns>EditInventoryModel ready for editing or null if not found</returns>
        Task<EditInventoryModel?> LoadInventoryItemForEditAsync(int inventoryId);

        /// <summary>
        /// Saves changes from an edit model to the database
        /// </summary>
        /// <param name="editModel">The edit model with changes</param>
        /// <returns>Result of the edit operation</returns>
        Task<EditInventoryResult> SaveInventoryItemAsync(EditInventoryModel editModel);

        /// <summary>
        /// Validates an edit model against business rules and master data
        /// </summary>
        /// <param name="editModel">The edit model to validate</param>
        /// <returns>Result with validation details</returns>
        Task<EditInventoryResult> ValidateInventoryItemAsync(EditInventoryModel editModel);

        /// <summary>
        /// Validates specific fields against master data
        /// </summary>
        /// <param name="partId">Part ID to validate</param>
        /// <param name="operation">Operation to validate</param>
        /// <param name="location">Location to validate</param>
        /// <returns>Dictionary of field validation results</returns>
        Task<Dictionary<string, bool>> ValidateMasterDataAsync(string partId, string operation, string location);

        /// <summary>
        /// Gets the original values for an inventory item (for change comparison)
        /// </summary>
        /// <param name="inventoryId">The inventory item ID</param>
        /// <returns>Original InventoryItem or null if not found</returns>
        Task<InventoryItem?> GetOriginalInventoryItemAsync(int inventoryId);

        /// <summary>
        /// Updates only the notes field of an inventory item (backward compatibility)
        /// </summary>
        /// <param name="inventoryId">Inventory item ID</param>
        /// <param name="partId">Part ID for validation</param>
        /// <param name="batchNumber">Batch number for validation</param>
        /// <param name="notes">New notes content</param>
        /// <param name="user">User making the change</param>
        /// <returns>Result of the notes update</returns>
        Task<EditInventoryResult> UpdateNotesOnlyAsync(int inventoryId, string partId, string batchNumber, string notes, string user);
    }

    /// <summary>
    /// Service for comprehensive inventory editing operations
    /// Handles full inventory item editing with validation, master data checks, and business logic
    /// </summary>
    public class InventoryEditingService : IInventoryEditingService
    {
        private readonly ILogger<InventoryEditingService> _logger;
        private readonly IDatabaseService _databaseService;
        private readonly IMasterDataService _masterDataService;

        public InventoryEditingService(
            ILogger<InventoryEditingService> logger,
            IDatabaseService databaseService,
            IMasterDataService masterDataService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
        }

        /// <summary>
        /// Loads an inventory item for editing by ID
        /// </summary>
        public async Task<EditInventoryModel?> LoadInventoryItemForEditAsync(int inventoryId)
        {
            try
            {
                _logger.LogInformation("Loading inventory item {InventoryId} for editing", inventoryId);

                // Use new stored procedure for comprehensive data loading
                var parameters = new Dictionary<string, object>
                {
                    ["p_ID"] = inventoryId
                };

                var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _databaseService.GetConnectionString(),
                    "inv_inventory_Get_ByID_ForEdit",
                    parameters
                );

                if (result.Status != 1 || result.Data == null || result.Data.Rows.Count == 0)
                {
                    _logger.LogWarning("Inventory item {InventoryId} not found or failed to load. Status: {Status}",
                        inventoryId, result.Status);
                    return null;
                }

                // Convert DataRow to EditInventoryModel
                var row = result.Data.Rows[0];
                var editModel = new EditInventoryModel
                {
                    Id = Convert.ToInt32(row["ID"]),
                    PartId = row["PartID"]?.ToString() ?? string.Empty,
                    Location = row["Location"]?.ToString() ?? string.Empty,
                    Operation = row["Operation"]?.ToString() ?? string.Empty,
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ItemType = row["ItemType"]?.ToString() ?? "WIP",
                    BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                    Notes = row["Notes"]?.ToString() ?? string.Empty,
                    User = row["User"]?.ToString() ?? "SYSTEM",
                    ReceiveDate = Convert.ToDateTime(row["ReceiveDate"]),
                    LastUpdated = Convert.ToDateTime(row["LastUpdated"])
                };

                // Save original values for change tracking
                editModel.SaveOriginalValues();

                _logger.LogInformation("Successfully loaded inventory item {InventoryId} for editing: {PartId}, {Operation}, {Location}",
                    inventoryId, editModel.PartId, editModel.Operation, editModel.Location);

                return editModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory item {InventoryId} for editing", inventoryId);
                await Services.Core.ErrorHandling.HandleErrorAsync(ex, $"Failed to load inventory item {inventoryId} for editing", "SYSTEM");
                return null;
            }
        }

        /// <summary>
        /// Saves changes from an edit model to the database
        /// </summary>
        public async Task<EditInventoryResult> SaveInventoryItemAsync(EditInventoryModel editModel)
        {
            try
            {
                if (editModel == null)
                    return EditInventoryResult.ValidationFailure(new List<string> { "Edit model cannot be null" });

                _logger.LogInformation("Saving inventory item {InventoryId} changes", editModel.Id);

                // First validate the model
                var validationResult = await ValidateInventoryItemAsync(editModel);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Check if there are actually changes to save
                if (!editModel.CheckForChanges())
                {
                    _logger.LogInformation("No changes detected for inventory item {InventoryId}", editModel.Id);
                    return EditInventoryResult.CreateSuccess(ConvertToSharedModel(editModel.ToInventoryItem()), editModel, editModel.User);
                }

                // Get original values for validation
                var originalItem = await GetOriginalInventoryItemAsync(editModel.Id);
                if (originalItem == null)
                {
                    return EditInventoryResult.DatabaseFailure("Original inventory item not found", -2, editModel);
                }

                // Call the comprehensive update stored procedure
                var parameters = new Dictionary<string, object>
                {
                    ["p_ID"] = editModel.Id,
                    ["p_PartID"] = editModel.PartId,
                    ["p_Location"] = editModel.Location,
                    ["p_Operation"] = editModel.Operation,
                    ["p_Quantity"] = editModel.Quantity,
                    ["p_ItemType"] = editModel.ItemType,
                    ["p_BatchNumber"] = editModel.BatchNumber,
                    ["p_Notes"] = editModel.Notes,
                    ["p_User"] = editModel.User,
                    ["p_Original_PartID"] = originalItem.PartId,
                    ["p_Original_BatchNumber"] = originalItem.BatchNumber ?? string.Empty
                };

                var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteWithStatus(
                    _databaseService.GetConnectionString(),
                    "inv_inventory_Update_Item",
                    parameters
                );

                _logger.LogInformation("Database update result for inventory {InventoryId}: Status={Status}, Message='{Message}'",
                    editModel.Id, result.Status, result.Message);

                if (result.Status == 1)
                {
                    // Success - create result with updated item
                    var updatedItem = editModel.ToInventoryItem();
                    updatedItem.LastUpdated = DateTime.Now;

                    var successResult = EditInventoryResult.CreateSuccess(ConvertToSharedModel(updatedItem), editModel, editModel.User);
                    successResult.DatabaseStatus = result.Status;
                    successResult.RowsAffected = 1;

                    // Record field changes for audit
                    RecordFieldChanges(successResult, originalItem, editModel);

                    _logger.LogInformation("Successfully saved inventory item {InventoryId} changes", editModel.Id);
                    return successResult;
                }
                else
                {
                    // Database operation failed
                    return EditInventoryResult.DatabaseFailure(
                        result.Message ?? "Unknown database error",
                        result.Status,
                        editModel
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving inventory item {InventoryId} changes", editModel?.Id ?? 0);
                await Services.Core.ErrorHandling.HandleErrorAsync(ex, $"Failed to save inventory item {editModel?.Id ?? 0} changes", "SYSTEM");

                return EditInventoryResult.DatabaseFailure(
                    $"Exception during save: {ex.Message}",
                    -1,
                    editModel
                );
            }
        }

        /// <summary>
        /// Validates an edit model against business rules and master data
        /// </summary>
        public async Task<EditInventoryResult> ValidateInventoryItemAsync(EditInventoryModel editModel)
        {
            if (editModel == null)
                return EditInventoryResult.ValidationFailure(new List<string> { "Edit model cannot be null" });

            try
            {
                _logger.LogInformation("Validating inventory item {InventoryId} for editing", editModel.Id);

                // First check model validation (data annotations)
                if (!editModel.IsValid())
                {
                    return EditInventoryResult.ValidationFailure(
                        new List<string> { editModel.ValidationError },
                        editModel
                    );
                }

                // Validate against master data
                var masterDataValidation = await ValidateMasterDataAsync(
                    editModel.PartId,
                    editModel.Operation,
                    editModel.Location
                );

                var validationErrors = new List<string>();

                // Check each master data field
                if (!masterDataValidation["PartId"])
                    validationErrors.Add($"Part ID '{editModel.PartId}' not found in master data");

                if (!masterDataValidation["Operation"])
                    validationErrors.Add($"Operation '{editModel.Operation}' not found in master data");

                if (!masterDataValidation["Location"])
                    validationErrors.Add($"Location '{editModel.Location}' not found in master data");

                // Additional business rule validation
                if (editModel.Quantity < 0)
                    validationErrors.Add("Quantity cannot be negative");

                if (string.IsNullOrWhiteSpace(editModel.ItemType))
                    validationErrors.Add("Item Type is required");

                if (editModel.Notes?.Length > 1000)
                    validationErrors.Add("Notes cannot exceed 1000 characters");

                if (validationErrors.Any())
                {
                    var failureResult = EditInventoryResult.MasterDataFailure(masterDataValidation, editModel);
                    foreach (var error in validationErrors)
                    {
                        failureResult.AddValidationError(error);
                    }
                    return failureResult;
                }

                // All validation passed
                var successResult = EditInventoryResult.CreateSuccess(ConvertToSharedModel(editModel.ToInventoryItem()), editModel, editModel.User);
                successResult.MasterDataValidation = masterDataValidation;

                _logger.LogInformation("Validation successful for inventory item {InventoryId}", editModel.Id);
                return successResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating inventory item {InventoryId}", editModel.Id);
                await Services.Core.ErrorHandling.HandleErrorAsync(ex, $"Failed to validate inventory item {editModel.Id}", "SYSTEM");

                return EditInventoryResult.ValidationFailure(
                    new List<string> { $"Validation error: {ex.Message}" },
                    editModel
                );
            }
        }

        /// <summary>
        /// Validates specific fields against master data
        /// </summary>
        public async Task<Dictionary<string, bool>> ValidateMasterDataAsync(string partId, string operation, string location)
        {
            try
            {
                _logger.LogDebug("Validating master data: PartId='{PartId}', Operation='{Operation}', Location='{Location}'",
                    partId, operation, location);

                // Use the stored procedure for master data validation
                // Note: This procedure uses SELECT to return results, not output parameters
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = partId ?? string.Empty,
                    ["p_Operation"] = operation ?? string.Empty,
                    ["p_Location"] = location ?? string.Empty
                };

                // Use ExecuteDataTableDirect since inv_inventory_Validate_MasterData returns data via SELECT
                var dataTable = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                    _databaseService.GetConnectionString(),
                    "inv_inventory_Validate_MasterData",
                    parameters
                );

                var validation = new Dictionary<string, bool>
                {
                    ["PartId"] = false,
                    ["Operation"] = false,
                    ["Location"] = false
                };

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    validation["PartId"] = Convert.ToBoolean(row["PartID_Valid"]);
                    validation["Operation"] = Convert.ToBoolean(row["Operation_Valid"]);
                    validation["Location"] = Convert.ToBoolean(row["Location_Valid"]);

                    _logger.LogDebug("Master data validation successful - returned {RowCount} rows with columns: {Columns}",
                        dataTable.Rows.Count, string.Join(", ", dataTable.Columns.Cast<System.Data.DataColumn>().Select(c => c.ColumnName)));
                }
                else
                {
                    _logger.LogWarning("Master data validation procedure returned no results for PartId='{PartId}', Operation='{Operation}', Location='{Location}'",
                        partId, operation, location);
                }

                _logger.LogDebug("Master data validation results: PartId={PartIdValid}, Operation={OperationValid}, Location={LocationValid}",
                    validation["PartId"], validation["Operation"], validation["Location"]);

                return validation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating master data");
                await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Failed to validate master data", "SYSTEM");

                // Return all false on error
                return new Dictionary<string, bool>
                {
                    ["PartId"] = false,
                    ["Operation"] = false,
                    ["Location"] = false
                };
            }
        }

        /// <summary>
        /// Gets the original values for an inventory item (for change comparison)
        /// </summary>
        public async Task<InventoryItem?> GetOriginalInventoryItemAsync(int inventoryId)
        {
            try
            {
                var dataTable = await _databaseService.GetInventoryByIdAsync(inventoryId);

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    return new InventoryItem
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        PartId = row["PartID"]?.ToString() ?? string.Empty,
                        Location = row["Location"]?.ToString() ?? string.Empty,
                        Operation = row["Operation"]?.ToString() ?? string.Empty,
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        ItemType = row["ItemType"]?.ToString() ?? "WIP",
                        BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                        Notes = row["Notes"]?.ToString() ?? string.Empty,
                        User = row["User"]?.ToString() ?? "SYSTEM",
                        ReceiveDate = Convert.ToDateTime(row["ReceiveDate"]),
                        LastUpdated = Convert.ToDateTime(row["LastUpdated"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting original inventory item {InventoryId}", inventoryId);
                await Services.Core.ErrorHandling.HandleErrorAsync(ex, $"Failed to get original inventory item {inventoryId}", "SYSTEM");
                return null;
            }
        }

        /// <summary>
        /// Updates only the notes field of an inventory item (backward compatibility)
        /// </summary>
        public async Task<EditInventoryResult> UpdateNotesOnlyAsync(int inventoryId, string partId, string batchNumber, string notes, string user)
        {
            try
            {
                _logger.LogInformation("Updating notes only for inventory item {InventoryId}", inventoryId);

                var result = await _databaseService.UpdateInventoryNotesAsync(inventoryId, partId, batchNumber, notes, user);

                if (result.Status >= 0)
                {
                    // Success - load updated item
                    var updatedItem = await GetOriginalInventoryItemAsync(inventoryId);
                    if (updatedItem != null)
                    {
                        var editModel = new EditInventoryModel(updatedItem);
                        var successResult = EditInventoryResult.CreateSuccess(ConvertToSharedModel(updatedItem), editModel, user);
                        successResult.RecordFieldChange("Notes", notes, notes);
                        return successResult;
                    }
                }

                return EditInventoryResult.DatabaseFailure(
                    result.Message ?? "Failed to update notes",
                    result.Status
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notes for inventory item {InventoryId}", inventoryId);
                await Services.Core.ErrorHandling.HandleErrorAsync(ex, $"Failed to update notes for inventory item {inventoryId}", "SYSTEM");

                return EditInventoryResult.DatabaseFailure($"Exception during notes update: {ex.Message}", -1);
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Records field changes between original and edited values for audit purposes
        /// </summary>
        private void RecordFieldChanges(EditInventoryResult result, InventoryItem original, EditInventoryModel edited)
        {
            if (original.PartId != edited.PartId)
                result.RecordFieldChange("PartId", original.PartId, edited.PartId);

            if (original.Location != edited.Location)
                result.RecordFieldChange("Location", original.Location, edited.Location);

            if (original.Operation != edited.Operation)
                result.RecordFieldChange("Operation", original.Operation, edited.Operation);

            if (original.Quantity != edited.Quantity)
                result.RecordFieldChange("Quantity", original.Quantity, edited.Quantity);

            if (original.ItemType != edited.ItemType)
                result.RecordFieldChange("ItemType", original.ItemType, edited.ItemType);

            if (original.BatchNumber != edited.BatchNumber)
                result.RecordFieldChange("BatchNumber", original.BatchNumber, edited.BatchNumber);

            if (original.Notes != edited.Notes)
                result.RecordFieldChange("Notes", original.Notes, edited.Notes);
        }

        /// <summary>
        /// Converts MTM_WIP_Application_Avalonia.Models.InventoryItem to MTM_Shared_Logic.Models.InventoryItem
        /// </summary>
        /// <param name="item">The source inventory item</param>
        /// <returns>Converted inventory item for MTM_Shared_Logic</returns>
        private static MTM_Shared_Logic.Models.InventoryItem ConvertToSharedModel(InventoryItem item)
        {
            return new MTM_Shared_Logic.Models.InventoryItem
            {
                ID = item.Id,           // Map Id to ID
                PartID = item.PartId,   // Map PartId to PartID
                Location = item.Location,
                Operation = item.Operation,
                Quantity = item.Quantity,
                ItemType = item.ItemType,
                ReceiveDate = item.ReceiveDate,
                LastUpdated = item.LastUpdated,
                User = item.User,
                BatchNumber = item.BatchNumber,
                Notes = item.Notes
            };
        }

        #endregion
    }
}
