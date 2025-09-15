using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for the comprehensive inventory edit dialog.
/// Provides editing capabilities for all inventory fields with validation and change tracking.
/// </summary>
public partial class EditInventoryViewModel : BaseViewModel
{
    private readonly IInventoryEditingService _editingService;
    private readonly IMasterDataService _masterDataService;

    [ObservableProperty]
    private EditInventoryModel editModel = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool hasValidationErrors;

    [ObservableProperty]
    private ObservableCollection<string> availablePartIds = new();

    [ObservableProperty]
    private ObservableCollection<string> availableLocations = new();

    [ObservableProperty]
    private ObservableCollection<string> availableOperations = new();

    [ObservableProperty]
    private ObservableCollection<string> availableItemTypes = new();

    // Field validation properties
    [ObservableProperty]
    private bool isPartIdValid;

    [ObservableProperty]
    private bool isPartIdInvalid;

    [ObservableProperty]
    private bool isLocationValid;

    [ObservableProperty]
    private bool isLocationInvalid;

    [ObservableProperty]
    private bool isOperationValid;

    [ObservableProperty]
    private bool isOperationInvalid;

    [ObservableProperty]
    private bool isQuantityValid = true;

    [ObservableProperty]
    private bool isQuantityInvalid;

    [ObservableProperty]
    private bool isItemTypeValid = true;

    // Computed properties
    public bool CanSave => EditModel.HasChanges && 
                          !HasValidationErrors && 
                          !IsLoading &&
                          IsPartIdValid && 
                          IsLocationValid && 
                          IsOperationValid && 
                          IsQuantityValid &&
                          IsItemTypeValid;

    // Events
    public event EventHandler? DialogClosed;
    public event EventHandler<InventorySavedEventArgs>? InventorySaved;

    public EditInventoryViewModel(
        ILogger<EditInventoryViewModel> logger,
        IInventoryEditingService editingService,
        IMasterDataService masterDataService)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(editingService);
        ArgumentNullException.ThrowIfNull(masterDataService);

        _editingService = editingService;
        _masterDataService = masterDataService;

        // Initialize item types with common values
        AvailableItemTypes = new ObservableCollection<string>
        {
            "Raw Material",
            "Work in Process",
            "Finished Good",
            "Component",
            "Tool",
            "Consumable"
        };

        // Wire up property change notifications for validation
        EditModel.PropertyChanged += (s, e) => 
        {
            ValidateField(e.PropertyName);
            OnPropertyChanged(nameof(CanSave));
        };
    }

    /// <summary>
    /// Initialize the dialog with an inventory item for editing.
    /// </summary>
    /// <param name="inventoryId">The inventory ID to edit</param>
    public async Task InitializeAsync(int inventoryId)
    {
        try
        {
            IsLoading = true;
            Logger.LogInformation("Initializing edit dialog for inventory ID: {InventoryId}", inventoryId);

            // Load master data in parallel
            var loadMasterDataTask = LoadMasterDataAsync();
            var loadItemTask = _editingService.LoadInventoryItemForEditAsync(inventoryId);

            await Task.WhenAll(loadMasterDataTask, loadItemTask);

            var inventoryItem = await loadItemTask;
            if (inventoryItem == null)
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Inventory item with ID {inventoryId} not found"),
                    "Failed to load inventory item for editing",
                    "SYSTEM"
                );
                return;
            }

            // Initialize the edit model (inventoryItem is already an EditInventoryModel)
            EditModel = inventoryItem;
            EditModel.ResetChangeTracking(); // Start fresh change tracking

            // Perform initial validation
            ValidateAllFields();
            
            Logger.LogInformation("Edit dialog initialized successfully for Part ID: {PartId}", EditModel.PartId);
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to initialize inventory edit dialog", "SYSTEM");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Load master data for dropdown lists and validation.
    /// </summary>
    private async Task LoadMasterDataAsync()
    {
        try
        {
            Logger.LogDebug("Loading master data for edit dialog");

            // Load master data using the service
            await _masterDataService.LoadAllMasterDataAsync();

            // Copy to local collections
            AvailablePartIds.Clear();
            foreach (var partId in _masterDataService.PartIds)
                AvailablePartIds.Add(partId);

            AvailableLocations.Clear();
            foreach (var location in _masterDataService.Locations)
                AvailableLocations.Add(location);

            AvailableOperations.Clear();
            foreach (var operation in _masterDataService.Operations)
                AvailableOperations.Add(operation);

            Logger.LogDebug("Master data loaded: {PartIdCount} part IDs, {LocationCount} locations, {OperationCount} operations",
                AvailablePartIds.Count, AvailableLocations.Count, AvailableOperations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load master data for edit dialog");
            // Continue with empty lists rather than failing completely
        }
    }

    /// <summary>
    /// Validate a specific field when it changes.
    /// </summary>
    /// <param name="fieldName">The name of the field to validate</param>
    private void ValidateField(string? fieldName)
    {
        switch (fieldName)
        {
            case nameof(EditModel.PartId):
                IsPartIdValid = !string.IsNullOrWhiteSpace(EditModel.PartId) && 
                               AvailablePartIds.Contains(EditModel.PartId);
                IsPartIdInvalid = !IsPartIdValid;
                break;

            case nameof(EditModel.Location):
                IsLocationValid = !string.IsNullOrWhiteSpace(EditModel.Location) && 
                                 AvailableLocations.Contains(EditModel.Location);
                IsLocationInvalid = !IsLocationValid;
                break;

            case nameof(EditModel.Operation):
                IsOperationValid = !string.IsNullOrWhiteSpace(EditModel.Operation) && 
                                  AvailableOperations.Contains(EditModel.Operation);
                IsOperationInvalid = !IsOperationValid;
                break;

            case nameof(EditModel.Quantity):
                IsQuantityValid = EditModel.Quantity >= 0;
                IsQuantityInvalid = !IsQuantityValid;
                break;

            case nameof(EditModel.ItemType):
                IsItemTypeValid = !string.IsNullOrWhiteSpace(EditModel.ItemType);
                break;
        }

        // Update overall validation status
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var validationContext = new ValidationContext(EditModel);
        var isModelValid = Validator.TryValidateObject(EditModel, validationContext, validationResults, true);

        HasValidationErrors = !isModelValid || IsPartIdInvalid || IsLocationInvalid || IsOperationInvalid || IsQuantityInvalid || !IsItemTypeValid;
    }

    /// <summary>
    /// Validate all fields at once.
    /// </summary>
    private void ValidateAllFields()
    {
        ValidateField(nameof(EditModel.PartId));
        ValidateField(nameof(EditModel.Location));
        ValidateField(nameof(EditModel.Operation));
        ValidateField(nameof(EditModel.Quantity));
        ValidateField(nameof(EditModel.ItemType));
    }

    /// <summary>
    /// Save changes to the inventory item.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!CanSave)
        {
            Logger.LogWarning("Save attempted but validation failed or no changes present");
            return;
        }

        try
        {
            IsLoading = true;
            Logger.LogInformation("Saving changes to inventory item: {PartId}", EditModel.PartId);

            // Validate the model one more time
            ValidateAllFields();
            if (HasValidationErrors)
            {
                Logger.LogWarning("Validation errors prevent saving");
                return;
            }

            // Save using EditModel directly
            var result = await _editingService.SaveInventoryItemAsync(EditModel);

            if (result.Success)
            {
                Logger.LogInformation("Successfully saved changes to inventory item: {PartId}", EditModel.PartId);
                
                // Raise saved event with the updated inventory item
                if (result.UpdatedInventoryItem != null)
                {
                    var inventoryItem = new InventoryItem
                    {
                        Id = result.UpdatedInventoryItem.ID,
                        PartId = result.UpdatedInventoryItem.PartID,
                        Location = result.UpdatedInventoryItem.Location,
                        Operation = result.UpdatedInventoryItem.Operation ?? string.Empty,
                        Quantity = result.UpdatedInventoryItem.Quantity,
                        ItemType = result.UpdatedInventoryItem.ItemType,
                        BatchNumber = result.UpdatedInventoryItem.BatchNumber ?? string.Empty,
                        Notes = result.UpdatedInventoryItem.Notes ?? string.Empty,
                        User = result.UpdatedInventoryItem.User,
                        ReceiveDate = result.UpdatedInventoryItem.ReceiveDate,
                        LastUpdated = result.UpdatedInventoryItem.LastUpdated
                    };
                    
                    InventorySaved?.Invoke(this, new InventorySavedEventArgs(inventoryItem));
                }
                
                // Close dialog
                DialogClosed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Save operation failed: {result.StatusMessage}"),
                    "Failed to save inventory changes",
                    "SYSTEM"
                );
            }
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Error saving inventory changes", "SYSTEM");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Revert all changes back to original values.
    /// </summary>
    [RelayCommand]
    private void Revert()
    {
        try
        {
            Logger.LogInformation("Reverting changes for inventory item: {PartId}", EditModel.PartId);
            
            EditModel.RevertChanges();
            ValidateAllFields();
            
            Logger.LogInformation("Changes reverted successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reverting changes");
        }
    }

    /// <summary>
    /// Cancel editing and close the dialog.
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        try
        {
            Logger.LogInformation("Cancelling edit dialog for: {PartId}", EditModel.PartId);
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing edit dialog");
        }
    }

    /// <summary>
    /// Check if there are unsaved changes.
    /// </summary>
    /// <returns>True if there are unsaved changes</returns>
    public bool HasUnsavedChanges()
    {
        return EditModel.HasChanges;
    }
}