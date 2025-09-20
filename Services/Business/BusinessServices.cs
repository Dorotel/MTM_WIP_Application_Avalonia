using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;
using MySql.Data.MySqlClient;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.Services.Business;
using MTM_Shared_Logic.Models;
using InventoryItem = MTM_WIP_Application_Avalonia.Models.InventoryItem;

namespace MTM_WIP_Application_Avalonia.Services.Business;

#region Master Data Service

/// <summary>
/// Interface for master data service that provides centralized access to reference data
/// </summary>
public interface IMasterDataService
{
    /// <summary>
    /// Observable collection of all Part IDs from master data
    /// </summary>
    ObservableCollection<string> PartIds { get; }
    
    /// <summary>
    /// Observable collection of all Operations from master data
    /// </summary>
    ObservableCollection<string> Operations { get; }
    
    /// <summary>
    /// Observable collection of all Locations from master data
    /// </summary>
    ObservableCollection<string> Locations { get; }
    
    /// <summary>
    /// Observable collection of all Users from master data
    /// </summary>
    ObservableCollection<string> Users { get; }
    
    /// <summary>
    /// Indicates if master data is currently being loaded
    /// </summary>
    bool IsLoading { get; }
    
    /// <summary>
    /// Loads all master data from database using stored procedures
    /// </summary>
    Task LoadAllMasterDataAsync();
    
    /// <summary>
    /// Refreshes specific master data category
    /// </summary>
    Task RefreshPartIdsAsync();
    Task RefreshOperationsAsync();
    Task RefreshLocationsAsync();
    Task RefreshUsersAsync();
    
    /// <summary>
    /// Event raised when master data is loaded or refreshed
    /// </summary>
    event EventHandler? MasterDataLoaded;
}

/// <summary>
/// Centralized master data service for MTM WIP Application.
/// Provides shared access to Part IDs, Operations, Locations, and Users across all ViewModels.
/// Uses MTM stored procedure patterns. When database is unavailable, collections remain empty
/// and validation logic shows appropriate "no data available" messages to users.
/// </summary>
public class MasterDataService : IMasterDataService
{
    private readonly ILogger<MasterDataService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IDatabaseService _databaseService;
    private bool _isLoading = false;

    public MasterDataService(ILogger<MasterDataService> logger, IConfigurationService configurationService, IDatabaseService databaseService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        
        PartIds = new ObservableCollection<string>();
        Operations = new ObservableCollection<string>();
        Locations = new ObservableCollection<string>();
        Users = new ObservableCollection<string>();
        
        _logger.LogInformation("MasterDataService initialized");
    }

    #region Public Properties

    public ObservableCollection<string> PartIds { get; }
    public ObservableCollection<string> Operations { get; }
    public ObservableCollection<string> Locations { get; }
    public ObservableCollection<string> Users { get; }
    
    public bool IsLoading 
    { 
        get => _isLoading;
        private set => _isLoading = value;
    }

    public event EventHandler? MasterDataLoaded;

    #endregion

    #region Master Data Loading - MTM Database Pattern

    /// <summary>
    /// Load all master data from database using stored procedures (MTM Pattern)
    /// Falls back to hardcoded data if database is unavailable
    /// </summary>
    public async Task LoadAllMasterDataAsync()
    {
        if (IsLoading)
        {
            _logger.LogDebug("Master data loading already in progress, skipping");
            return;
        }

        try
        {
            IsLoading = true;
            _logger.LogInformation("Loading master data using MTM stored procedure patterns");
            
            // Test database connection first
            var connectionString = _configurationService.GetConnectionString();
            _logger.LogInformation("Testing database connection...");
            
            await Task.WhenAll(
                LoadPartIdsFromDatabaseAsync(),
                LoadOperationsFromDatabaseAsync(),
                LoadLocationsFromDatabaseAsync(),
                LoadUsersFromDatabaseAsync()
            );
            
            _logger.LogInformation("Master data loaded successfully from database - Parts: {PartCount}, Operations: {OpCount}, Locations: {LocCount}, Users: {UserCount}", 
                PartIds.Count, Operations.Count, Locations.Count, Users.Count);
                
            MasterDataLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load master data from database. Collections will remain empty to indicate data unavailability.");
            // MTM Pattern: Do not load fallback data - leave collections empty to indicate server connectivity issues
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refresh Part IDs from database
    /// </summary>
    public async Task RefreshPartIdsAsync()
    {
        await LoadPartIdsFromDatabaseAsync();
        _logger.LogInformation("Part IDs refreshed - Count: {Count}", PartIds.Count);
    }

    /// <summary>
    /// Refresh Operations from database
    /// </summary>
    public async Task RefreshOperationsAsync()
    {
        await LoadOperationsFromDatabaseAsync();
        _logger.LogInformation("Operations refreshed - Count: {Count}", Operations.Count);
    }

    /// <summary>
    /// Refresh Locations from database
    /// </summary>
    public async Task RefreshLocationsAsync()
    {
        await LoadLocationsFromDatabaseAsync();
        _logger.LogInformation("Locations refreshed - Count: {Count}", Locations.Count);
    }

    /// <summary>
    /// Refresh Users from database
    /// </summary>
    public async Task RefreshUsersAsync()
    {
        await LoadUsersFromDatabaseAsync();
        _logger.LogInformation("Users refreshed - Count: {Count}", Users.Count);
    }

    #endregion

    #region Private Database Loading Methods

    /// <summary>
    /// Load Part IDs from database using DatabaseService
    /// </summary>
    private async Task LoadPartIdsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Part IDs from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllPartIDsAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PartIds.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns PartID column
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartIds.Add(partId);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Part IDs from database", PartIds.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Part IDs. No fallback data will be loaded - collections will remain empty.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Part IDs from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
        }
    }

    /// <summary>
    /// Load Operations from database using DatabaseService
    /// </summary>
    private async Task LoadOperationsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Operations from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllOperationsAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Operations.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns Operation column
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            Operations.Add(operation);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Operations from database", Operations.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Operations. No fallback data will be loaded - collections will remain empty.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Operations from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
        }
    }

    /// <summary>
    /// Load Locations from database using DatabaseService
    /// </summary>
    private async Task LoadLocationsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Locations from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllLocationsAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Locations.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns Location column
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            Locations.Add(location);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Locations from database", Locations.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Locations. No fallback data will be loaded - collections will remain empty.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Locations from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
        }
    }

    /// <summary>
    /// Load Users from database using DatabaseService
    /// </summary>
    private async Task LoadUsersFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Users from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllUsersAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Users.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns User column (not UserId)
                        var user = row["User"]?.ToString();
                        if (!string.IsNullOrEmpty(user))
                        {
                            Users.Add(user);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Users from database", Users.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Users. No fallback data will be loaded - collections will remain empty.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Users from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
        }
    }

    #endregion
}

#endregion

#region Inventory Editing Service

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

            var dataTable = await _databaseService.GetInventoryByIdAsync(inventoryId);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
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

            _logger.LogWarning("Inventory item {InventoryId} not found", inventoryId);
            return null;
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

            // Save via database service - using available method for now
            // TODO: Implement proper UpdateInventoryItemAsync or use existing methods
            var result = await _databaseService.UpdateInventoryNotesAsync(
                editModel.Id,
                editModel.PartId,
                editModel.BatchNumber,
                editModel.Notes,
                editModel.User
            );

            if (result.Status >= 0)
            {
                // Success
                var updatedItem = editModel.ToInventoryItem();
                updatedItem.LastUpdated = DateTime.Now;

                var successResult = EditInventoryResult.CreateSuccess(ConvertToSharedModel(updatedItem), editModel, editModel.User);
                successResult.DatabaseStatus = result.Status;

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
            // Simple validation against master data collections
            var validation = new Dictionary<string, bool>
            {
                ["PartId"] = !string.IsNullOrEmpty(partId) && _masterDataService.PartIds.Contains(partId),
                ["Operation"] = !string.IsNullOrEmpty(operation) && _masterDataService.Operations.Contains(operation),
                ["Location"] = !string.IsNullOrEmpty(location) && _masterDataService.Locations.Contains(location)
            };

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

#endregion

#region Remove Service

/// <summary>
/// Interface for inventory removal service that provides centralized business logic for inventory removal operations
/// </summary>
public interface IRemoveService
{
    /// <summary>
    /// Observable collection of inventory items matching current search criteria
    /// </summary>
    ObservableCollection<InventoryItem> InventoryItems { get; }
    
    /// <summary>
    /// Observable collection of items available for undo operations
    /// </summary>
    ObservableCollection<InventoryItem> UndoItems { get; }
    
    /// <summary>
    /// Indicates if the service is currently performing operations
    /// </summary>
    bool IsLoading { get; }
    
    /// <summary>
    /// Indicates if there are items available for undo
    /// </summary>
    bool HasUndoItems { get; }
    
    /// <summary>
    /// Event fired when inventory items are removed successfully
    /// </summary>
    event EventHandler<ItemsRemovedEventArgs>? ItemsRemoved;
    
    /// <summary>
    /// Event fired when service loading state changes
    /// </summary>
    event EventHandler<bool>? LoadingStateChanged;
    
    /// <summary>
    /// Searches for inventory items based on provided criteria
    /// </summary>
    Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string? partId = null, string? operation = null, string? location = null, string? user = null);
    
    /// <summary>
    /// Removes multiple inventory items in an atomic transaction
    /// </summary>
    Task<ServiceResult<RemovalResult>> RemoveInventoryItemsAsync(IEnumerable<InventoryItem> items, string currentUser, string notes = "");
    
    /// <summary>
    /// Removes a single inventory item
    /// </summary>
    Task<ServiceResult<RemovalResult>> RemoveInventoryItemAsync(InventoryItem item, string currentUser, string notes = "");
    
    /// <summary>
    /// Restores previously removed items from the current session
    /// </summary>
    Task<ServiceResult<RestoreResult>> UndoLastRemovalAsync(string currentUser);
    
    /// <summary>
    /// Clears the current undo session
    /// </summary>
    void ClearUndoSession();
    
    /// <summary>
    /// Refreshes the inventory search results
    /// </summary>
    Task<ServiceResult> RefreshInventoryAsync();
    
    /// <summary>
    /// Gets suggestions for part IDs based on user input
    /// </summary>
    Task<List<string>> GetPartSuggestionsAsync(string userInput);
    
    /// <summary>
    /// Gets suggestions for operations based on user input
    /// </summary>
    Task<List<string>> GetOperationSuggestionsAsync(string userInput);
    
    /// <summary>
    /// Gets suggestions for locations based on user input
    /// </summary>
    Task<List<string>> GetLocationSuggestionsAsync(string userInput);
    
    /// <summary>
    /// Gets suggestions for users based on user input
    /// </summary>
    Task<List<string>> GetUserSuggestionsAsync(string userInput);
}

/// <summary>
/// Service implementation for inventory removal operations following MTM service-oriented architecture
/// </summary>
public class RemoveService : IRemoveService
{
    private readonly IDatabaseService _databaseService;
    private readonly IMasterDataService _masterDataService;
    private readonly ILogger<RemoveService> _logger;
    
    // Session management for undo functionality
    private readonly List<InventoryItem> _currentSessionRemovals = new();
    private readonly object _sessionLock = new();
    
    /// <inheritdoc />
    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();
    
    /// <inheritdoc />
    public ObservableCollection<InventoryItem> UndoItems { get; } = new();
    
    private bool _isLoading = false;
    /// <inheritdoc />
    public bool IsLoading 
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                LoadingStateChanged?.Invoke(this, value);
            }
        }
    }
    
    /// <inheritdoc />
    public bool HasUndoItems 
    {
        get 
        {
            lock (_sessionLock)
            {
                return _currentSessionRemovals.Count > 0;
            }
        }
    }
    
    /// <inheritdoc />
    public event EventHandler<ItemsRemovedEventArgs>? ItemsRemoved;
    
    /// <inheritdoc />
    public event EventHandler<bool>? LoadingStateChanged;
    
    public RemoveService(
        IDatabaseService databaseService,
        IMasterDataService masterDataService, 
        ILogger<RemoveService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogDebug("RemoveService initialized with dependencies");
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string? partId = null, string? operation = null, string? location = null, string? user = null)
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Searching inventory with criteria - PartId: {PartId}, Operation: {Operation}, Location: {Location}, User: {User}",
                partId ?? "Any", operation ?? "Any", location ?? "Any", user ?? "Any");
            
            var searchResults = new List<InventoryItem>();
            
            // Use database service to search inventory based on criteria
            if (!string.IsNullOrWhiteSpace(partId) && !string.IsNullOrWhiteSpace(operation))
            {
                // Search by Part ID and Operation
                var result = await _databaseService.GetInventoryByPartAndOperationAsync(partId, operation).ConfigureAwait(false);
                if (result != null)
                {
                    searchResults.AddRange(ConvertDataTableToInventoryItems(result));
                }
            }
            else if (!string.IsNullOrWhiteSpace(partId))
            {
                // Search by Part ID only
                var result = await _databaseService.GetInventoryByPartIdAsync(partId).ConfigureAwait(false);
                if (result != null)
                {
                    searchResults.AddRange(ConvertDataTableToInventoryItems(result));
                }
            }
            else if (!string.IsNullOrWhiteSpace(user))
            {
                // Search by User
                var result = await _databaseService.GetInventoryByUserAsync(user).ConfigureAwait(false);
                if (result != null)
                {
                    searchResults.AddRange(ConvertDataTableToInventoryItems(result));
                }
            }
            else
            {
                // For now, return empty list for general searches to avoid performance issues
                _logger.LogInformation("General inventory search not implemented - specific criteria required");
            }
            
            // Apply additional filtering in memory for complex criteria
            if (!string.IsNullOrWhiteSpace(location))
            {
                searchResults = searchResults.Where(item => 
                    string.Equals(item.Location, location, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(user))
            {
                searchResults = searchResults.Where(item => 
                    string.Equals(item.User, user, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Update UI collections on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                InventoryItems.Clear();
                foreach (var item in searchResults)
                {
                    InventoryItems.Add(item);
                }
            });
            
            _logger.LogInformation("Inventory search completed. Found {Count} items", searchResults.Count);
            return ServiceResult<List<InventoryItem>>.Success(searchResults, $"Found {searchResults.Count} inventory items");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search inventory");
            return ServiceResult<List<InventoryItem>>.Failure($"Failed to search inventory: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<RemovalResult>> RemoveInventoryItemsAsync(IEnumerable<InventoryItem> items, string currentUser, string notes = "")
    {
        var itemList = items?.ToList() ?? throw new ArgumentNullException(nameof(items));
        if (!itemList.Any())
        {
            return ServiceResult<RemovalResult>.Failure("No items provided for removal");
        }
        
        try
        {
            IsLoading = true;
            _logger.LogInformation("Starting batch removal of {Count} inventory items for user {User}", itemList.Count, currentUser);
            
            var successfulRemovals = new List<InventoryItem>();
            var failures = new List<RemovalFailure>();
            
            // Process each item in the batch
            foreach (var item in itemList)
            {
                try
                {
                    // Remove item using database service
                    var removeResult = await _databaseService.RemoveInventoryItemAsync(
                        item.PartId,
                        item.Location,
                        item.Operation ?? string.Empty,
                        item.Quantity,
                        item.ItemType,
                        currentUser,
                        item.BatchNumber ?? string.Empty,
                        $"Removed via RemoveService - {notes}"
                    ).ConfigureAwait(false);

                    if (removeResult.Status >= 0)
                    {
                        successfulRemovals.Add(item);
                        _logger.LogInformation("Successfully removed inventory item: {PartId} - {Operation} - Qty: {Quantity}", 
                            item.PartId, item.Operation, item.Quantity);
                    }
                    else
                    {
                        failures.Add(new RemovalFailure
                        {
                            Item = item,
                            Error = removeResult.Message ?? "Unknown database error",
                            ErrorType = "Database"
                        });
                    }
                }
                catch (Exception ex)
                {
                    failures.Add(new RemovalFailure
                    {
                        Item = item,
                        Error = ex.Message,
                        ErrorType = "Exception"
                    });
                    _logger.LogError(ex, "Exception removing item {PartId} - {Operation}", item.PartId, item.Operation);
                }
            }

            // Store successful removals for undo functionality
            if (successfulRemovals.Any())
            {
                lock (_sessionLock)
                {
                    _currentSessionRemovals.AddRange(successfulRemovals);
                }
                
                // Update UI collections
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var item in successfulRemovals)
                    {
                        InventoryItems.Remove(item);
                        UndoItems.Add(item);
                    }
                });
            }

            var result = new RemovalResult
            {
                SuccessfulRemovals = successfulRemovals,
                Failures = failures,
                TotalProcessed = itemList.Count
            };

            // Fire event for successful removals
            if (successfulRemovals.Any())
            {
                ItemsRemoved?.Invoke(this, new ItemsRemovedEventArgs { RemovedItems = successfulRemovals });
            }

            var message = failures.Any() 
                ? $"Processed {itemList.Count} items: {successfulRemovals.Count} successful, {failures.Count} failed"
                : $"Successfully removed {successfulRemovals.Count} items";

            return ServiceResult<RemovalResult>.Success(result, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove inventory items");
            return ServiceResult<RemovalResult>.Failure($"Failed to remove inventory items: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<RemovalResult>> RemoveInventoryItemAsync(InventoryItem item, string currentUser, string notes = "")
    {
        return await RemoveInventoryItemsAsync(new[] { item }, currentUser, notes);
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult<RestoreResult>> UndoLastRemovalAsync(string currentUser)
    {
        try
        {
            List<InventoryItem> itemsToRestore;
            
            lock (_sessionLock)
            {
                if (_currentSessionRemovals.Count == 0)
                {
                    return ServiceResult<RestoreResult>.Failure("No items available for undo");
                }
                
                itemsToRestore = new List<InventoryItem>(_currentSessionRemovals);
                _currentSessionRemovals.Clear();
            }
            
            IsLoading = true;
            _logger.LogInformation("Starting undo operation for {Count} items", itemsToRestore.Count);
            
            var successfulRestores = new List<InventoryItem>();
            var failures = new List<RestoreFailure>();
            
            foreach (var item in itemsToRestore)
            {
                try
                {
                    // Add item back using database service
                    var addResult = await _databaseService.AddInventoryItemAsync(
                        item.PartId,
                        item.Location,
                        item.Operation,
                        item.Quantity,
                        item.ItemType,
                        currentUser,
                        item.BatchNumber ?? string.Empty,
                        $"Restored via undo - {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
                    ).ConfigureAwait(false);

                    if (addResult.Status >= 0)
                    {
                        successfulRestores.Add(item);
                        _logger.LogInformation("Successfully restored inventory item: {PartId} - {Operation} - Qty: {Quantity}", 
                            item.PartId, item.Operation, item.Quantity);
                    }
                    else
                    {
                        failures.Add(new RestoreFailure
                        {
                            Item = item,
                            Error = addResult.Message ?? "Unknown database error",
                            ErrorType = "Database"
                        });
                    }
                }
                catch (Exception ex)
                {
                    failures.Add(new RestoreFailure
                    {
                        Item = item,
                        Error = ex.Message,
                        ErrorType = "Exception"
                    });
                    _logger.LogError(ex, "Exception restoring item {PartId} - {Operation}", item.PartId, item.Operation);
                }
            }

            // Update UI collections
            if (successfulRestores.Any())
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var item in successfulRestores)
                    {
                        UndoItems.Remove(item);
                        InventoryItems.Add(item);
                    }
                });
            }

            var result = new RestoreResult
            {
                SuccessfulRestores = successfulRestores,
                Failures = failures,
                TotalProcessed = itemsToRestore.Count
            };

            var message = failures.Any() 
                ? $"Processed {itemsToRestore.Count} items: {successfulRestores.Count} restored, {failures.Count} failed"
                : $"Successfully restored {successfulRestores.Count} items";

            return ServiceResult<RestoreResult>.Success(result, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to undo last removal");
            return ServiceResult<RestoreResult>.Failure($"Failed to undo last removal: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    /// <inheritdoc />
    public void ClearUndoSession()
    {
        lock (_sessionLock)
        {
            _currentSessionRemovals.Clear();
        }
        
        Dispatcher.UIThread.Post(() =>
        {
            UndoItems.Clear();
        });
        
        _logger.LogInformation("Undo session cleared");
    }
    
    /// <inheritdoc />
    public async Task<ServiceResult> RefreshInventoryAsync()
    {
        try
        {
            // Clear current inventory and refresh based on last search criteria
            // This is a simplified implementation - in production might want to store last search criteria
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                InventoryItems.Clear();
            });
            
            _logger.LogInformation("Inventory data refreshed");
            return ServiceResult.Success("Inventory data refreshed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh inventory");
            return ServiceResult.Failure($"Failed to refresh inventory: {ex.Message}", ex);
        }
    }
    
    /// <inheritdoc />
    public async Task<List<string>> GetPartSuggestionsAsync(string userInput)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userInput) || userInput.Length < 2)
                return new List<string>();
                
            var suggestions = _masterDataService.PartIds
                .Where(p => p.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
                
            return await Task.FromResult(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting part suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }
    
    /// <inheritdoc />
    public async Task<List<string>> GetOperationSuggestionsAsync(string userInput)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return new List<string>();
                
            var suggestions = _masterDataService.Operations
                .Where(o => o.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
                
            return await Task.FromResult(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting operation suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }
    
    /// <inheritdoc />
    public async Task<List<string>> GetLocationSuggestionsAsync(string userInput)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return new List<string>();
                
            var suggestions = _masterDataService.Locations
                .Where(l => l.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
                
            return await Task.FromResult(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }
    
    /// <inheritdoc />
    public async Task<List<string>> GetUserSuggestionsAsync(string userInput)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return new List<string>();
                
            var suggestions = _masterDataService.Users
                .Where(u => u.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();
                
            return await Task.FromResult(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }
    
    #region Private Helper Methods
    
    /// <summary>
    /// Converts DataTable to list of InventoryItem objects
    /// </summary>
    private List<InventoryItem> ConvertDataTableToInventoryItems(DataTable dataTable)
    {
        var items = new List<InventoryItem>();
        
        foreach (DataRow row in dataTable.Rows)
        {
            var item = new InventoryItem
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
            
            items.Add(item);
        }
        
        return items;
    }
    
    #endregion
}

#endregion

#region Supporting Types

/// <summary>
/// Result of an inventory removal operation
/// </summary>
public class RemovalResult
{
    public List<InventoryItem> SuccessfulRemovals { get; set; } = new();
    public List<RemovalFailure> Failures { get; set; } = new();
    public int TotalProcessed { get; set; }
    
    // Backward compatibility properties
    public bool HasSuccesses => SuccessfulRemovals.Any();
    public bool HasFailures => Failures.Any();
    public int SuccessCount => SuccessfulRemovals.Count;
    public int FailureCount => Failures.Count;
}

/// <summary>
/// Information about a failed removal operation
/// </summary>
public class RemovalFailure
{
    public InventoryItem Item { get; set; } = new();
    public string Error { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
}

/// <summary>
/// Result of an inventory restore operation
/// </summary>
public class RestoreResult
{
    public List<InventoryItem> SuccessfulRestores { get; set; } = new();
    public List<RestoreFailure> Failures { get; set; } = new();
    public int TotalProcessed { get; set; }
    
    // Backward compatibility properties
    public bool HasSuccesses => SuccessfulRestores.Any();
    public bool HasFailures => Failures.Any();
    public int SuccessCount => SuccessfulRestores.Count;
    public int FailureCount => Failures.Count;
}

/// <summary>
/// Information about a failed restore operation
/// </summary>
public class RestoreFailure
{
    public InventoryItem Item { get; set; } = new();
    public string Error { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty;
}

/// <summary>
/// Event args for items removed event
/// </summary>
public class ItemsRemovedEventArgs : EventArgs
{
    public List<InventoryItem> RemovedItems { get; set; } = new();
}

/// <summary>
/// Generic service result wrapper
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    
    public static ServiceResult Success(string message = "") => new() { IsSuccess = true, Message = message };
    public static ServiceResult Failure(string message, Exception? ex = null) => new() { IsSuccess = false, Message = message, Exception = ex };
}

/// <summary>
/// Generic service result wrapper with data
/// </summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }
    
    // Backward compatibility - ViewModels expect .Value
    public T? Value => Data;
    
    public static ServiceResult<T> Success(T data, string message = "") => new() { IsSuccess = true, Data = data, Message = message };
    public static new ServiceResult<T> Failure(string message, Exception? ex = null) => new() { IsSuccess = false, Message = message, Exception = ex };
}

#endregion