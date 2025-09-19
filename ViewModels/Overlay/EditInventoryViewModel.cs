using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
/// Only allows editing if current user matches the record's user.
/// </summary>
public partial class EditInventoryViewModel : BaseViewModel
{
    private readonly IInventoryEditingService _editingService;
    private readonly IMasterDataService _masterDataService;

    [ObservableProperty]
    private EditInventoryModel editModel = new();

    /// <summary>
    /// Override to handle EditModel property changes - no longer needed since TextBox bindings work correctly
    /// </summary>
    partial void OnEditModelChanged(EditInventoryModel? oldValue, EditInventoryModel newValue)
    {
        Logger.LogInformation("🔍 QA DEBUG: EditModel property CHANGED! Old BatchNumber: '{OldBatch}', New BatchNumber: '{NewBatch}', New User: '{NewUser}', New ID: {NewId}",
            oldValue?.BatchNumber ?? "NULL",
            newValue?.BatchNumber ?? "NULL",
            newValue?.User ?? "NULL",
            newValue?.Id ?? -1);

        // Unsubscribe from old model if it exists
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnEditModelPropertyChanged;
        }

        // Subscribe to new model property changes
        if (newValue != null)
        {
            newValue.PropertyChanged -= OnEditModelPropertyChanged; // Ensure no double subscription
            newValue.PropertyChanged += OnEditModelPropertyChanged;

            Logger.LogDebug("🔍 Subscribed to new EditModel property changes");

            // Trigger initial CanSave evaluation
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
        }

        // TextBox bindings work correctly - no manual refresh needed
        Logger.LogInformation("🔍 QA TEXTBOX-BINDINGS: TextBox controls should auto-refresh with EditModel changes");
    }

    /// <summary>
    /// Handle EditModel property changes
    /// </summary>
    private void OnEditModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Logger.LogDebug("🔍 EditModel property changed: {PropertyName}", e.PropertyName);

        ValidateField(e.PropertyName);
        OnPropertyChanged(nameof(CanSave)); // Notify CanSave when EditModel changes
        SaveCommand.NotifyCanExecuteChanged(); // Notify the command as well

        // Subscribe to HasChanges property specifically to ensure CanSave updates
        if (e.PropertyName == nameof(EditModel.HasChanges))
        {
            Logger.LogDebug("EditModel.HasChanges changed to: {HasChanges}", EditModel.HasChanges);
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged(); // Critical: Notify SaveCommand when HasChanges updates
        }
    }

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool hasValidationErrors;

    [ObservableProperty]
    private bool canEditRecord = false;

    [ObservableProperty]
    private string permissionErrorMessage = string.Empty;

    [ObservableProperty]
    private string quantityErrorMessage = string.Empty;

    [ObservableProperty]
    private string quantityText = string.Empty;

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

    // ObservableProperty pattern for TextBlock bindings (like other ViewModels in codebase)
    [ObservableProperty]
    private string displayItemType = string.Empty;

    [ObservableProperty]
    private string displayBatchNumber = string.Empty;

    [ObservableProperty]
    private string displayUser = string.Empty;

    [ObservableProperty]
    private DateTime displayReceiveDate = DateTime.MinValue;

    [ObservableProperty]
    private DateTime displayLastUpdated = DateTime.MinValue;

    // Computed properties
    public bool CanSave
    {
        get
        {
            var canEdit = CanEditRecord;
            var hasChanges = EditModel.HasChanges;
            var hasErrors = HasValidationErrors;
            var loading = IsLoading;
            var operationValid = IsOperationValid;
            var quantityValid = IsQuantityValid;

            var result = canEdit &&
                        hasChanges &&
                        !hasErrors &&
                        !loading &&
                        operationValid &&
                        quantityValid;

            // Debug logging to identify which condition is failing
            Logger.LogDebug("🔍 CanSave DEBUG: CanEditRecord={CanEdit}, HasChanges={HasChanges}, " +
                          "HasValidationErrors={HasErrors}, IsLoading={Loading}, " +
                          "IsOperationValid={OperationValid}, IsQuantityValid={QuantityValid}, " +
                          "RESULT={Result}",
                          canEdit, hasChanges, hasErrors, loading,
                          operationValid, quantityValid, result);

            return result;
        }
    }

    // Events
    public event EventHandler? DialogClosed;
    public event EventHandler<InventorySavedEventArgs>? InventorySaved;

    /// <summary>
    /// Updates display properties for TextBlock bindings using ObservableProperty pattern
    /// </summary>
    private void UpdateDisplayProperties()
    {
        if (EditModel != null)
        {
            DisplayItemType = EditModel.ItemType ?? string.Empty;
            DisplayBatchNumber = EditModel.BatchNumber ?? string.Empty;
            DisplayUser = EditModel.User ?? string.Empty;
            DisplayReceiveDate = EditModel.ReceiveDate;
            DisplayLastUpdated = EditModel.LastUpdated;

            Logger.LogInformation("🔍 QA DISPLAY-UPDATE: Updated display properties - ItemType: {ItemType}, BatchNumber: {BatchNumber}, User: {User}",
                DisplayItemType, DisplayBatchNumber, DisplayUser);
        }
    }

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

        // QA: Track ViewModel creation
        Logger.LogInformation("🔍 QA CONSTRUCTOR: EditInventoryViewModel created at {Timestamp}", DateTime.Now);

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

        // Note: EditModel property change subscriptions are handled in OnEditModelChanged
        // This ensures proper subscription after EditModel is set
    }

    /// <summary>
    /// Override property change handlers to ensure CanSave is notified when dependencies change
    /// </summary>
    partial void OnCanEditRecordChanged(bool value)
    {
        Logger.LogDebug("CanEditRecord changed to: {CanEditRecord}", value);
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnHasValidationErrorsChanged(bool value)
    {
        Logger.LogDebug("HasValidationErrors changed to: {HasValidationErrors}", value);
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsLoadingChanged(bool value)
    {
        Logger.LogDebug("IsLoading changed to: {IsLoading}", value);
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsOperationValidChanged(bool value)
    {
        Logger.LogDebug("IsOperationValid changed to: {IsOperationValid}", value);
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsQuantityValidChanged(bool value)
    {
        Logger.LogDebug("IsQuantityValid changed to: {IsQuantityValid}", value);
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnQuantityTextChanged(string value)
    {
        // Handle quantity text input with proper validation
        bool isValid = false;
        int quantity = 0;

        if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out quantity) && quantity > 0)
        {
            // Valid positive quantity
            isValid = true;
        }

        // Update EditModel
        EditModel.Quantity = quantity;

        // Set validation state and error message
        IsQuantityValid = isValid;
        IsQuantityInvalid = !isValid;
        QuantityErrorMessage = isValid ? string.Empty : "Enter Quantity";

        // Update overall validation state
        HasValidationErrors = IsOperationInvalid || IsQuantityInvalid;
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
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

            // Immediately update display properties for TextBlock bindings
            UpdateDisplayProperties();

            // Check if current user can edit this record
            var currentUser = Environment.UserName.ToUpper();
            var recordUser = EditModel.User?.ToUpper() ?? string.Empty;

            CanEditRecord = currentUser == recordUser;

            if (!CanEditRecord)
            {
                PermissionErrorMessage = $"You can only edit records you created. This record was created by '{EditModel.User}' but you are '{currentUser}'.";
                Logger.LogWarning("User {CurrentUser} attempted to edit record created by {RecordUser}", currentUser, EditModel.User);
            }
            else
            {
                PermissionErrorMessage = string.Empty;
                Logger.LogInformation("User {CurrentUser} has permission to edit this record", currentUser);
            }

            // Perform initial validation (only for editable fields if user has permission)
            if (CanEditRecord)
            {
                ValidateAllFields();

                // Force initial CanSave evaluation and notification
                Logger.LogDebug("🔍 InitializeAsync(int) - Initial validation completed, triggering CanSave notification");
                OnPropertyChanged(nameof(CanSave));
                SaveCommand.NotifyCanExecuteChanged();
            }
            else
            {
                // Even if user can't edit, we should set the validation states properly
                IsOperationValid = true; // Don't block on validation if can't edit anyway
                IsQuantityValid = true;
                Logger.LogDebug("🔍 InitializeAsync(int) - User can't edit, setting validation states to true");
                OnPropertyChanged(nameof(CanSave));
                SaveCommand.NotifyCanExecuteChanged();
            }

            Logger.LogInformation("Edit dialog initialized successfully for Part ID: {PartId}, CanEdit: {CanEdit}", EditModel.PartId, CanEditRecord);
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
    /// Initialize the dialog with a selected inventory item for editing.
    /// This method accepts the InventoryItem directly from CustomDataGrid selection.
    /// </summary>
    /// <param name="inventoryItem">The selected inventory item to edit</param>
    public async Task InitializeAsync(InventoryItem inventoryItem)
    {
        try
        {
            IsLoading = true;
            Logger.LogInformation("Initializing edit dialog for selected inventory item: {PartId}", inventoryItem.PartId);

            // Load master data first
            await LoadMasterDataAsync();

            // QUALITY ASSURANCE DEBUG: Log before and after EditModel replacement
            Logger.LogInformation("🔍 QA DEBUG: BEFORE EditModel replacement - Current BatchNumber: {CurrentBatch}, Current User: {CurrentUser}, New BatchNumber: {NewBatch}, New User: {NewUser}, New ID: {NewId}",
                EditModel?.BatchNumber ?? "NULL", EditModel?.User ?? "NULL", inventoryItem.BatchNumber ?? "NULL", inventoryItem.User ?? "NULL", inventoryItem.Id);

            // 🔍 QA: Log all InventoryItem properties to see what data we're starting with
            Logger.LogCritical("🚨 QA SOURCE DATA: InventoryItem ALL Properties - ID: {Id}, PartId: '{PartId}', Operation: '{Operation}', Quantity: {Quantity}, ItemType: '{ItemType}', BatchNumber: '{BatchNumber}', User: '{User}', Location: '{Location}', Notes: '{Notes}', ReceiveDate: {ReceiveDate}, LastUpdated: {LastUpdated}",
                inventoryItem.Id, inventoryItem.PartId ?? "NULL", inventoryItem.Operation ?? "NULL", inventoryItem.Quantity, inventoryItem.ItemType ?? "NULL", inventoryItem.BatchNumber ?? "NULL", inventoryItem.User ?? "NULL", inventoryItem.Location ?? "NULL", inventoryItem.Notes ?? "NULL", inventoryItem.ReceiveDate, inventoryItem.LastUpdated);

            // Initialize the edit model directly from the InventoryItem
            EditModel = new EditInventoryModel(inventoryItem);
            EditModel.ResetChangeTracking(); // Start fresh change tracking

            // Initialize QuantityText to prevent binding errors
            QuantityText = EditModel.Quantity.ToString();

            // Immediately update display properties for TextBlock bindings
            UpdateDisplayProperties();

            Logger.LogInformation("🔍 QA DEBUG: AFTER EditModel replacement - EditModel.BatchNumber: {BatchNumber}, EditModel.User: {User}, EditModel.Id: {Id}",
                EditModel.BatchNumber ?? "NULL", EditModel.User ?? "NULL", EditModel.Id);

            // 🔍 QA: Log all EditModel properties to see what's actually set
            Logger.LogCritical("🚨 QA DETAILED: EditModel ALL Properties - ID: {Id}, PartId: '{PartId}', Operation: '{Operation}', Quantity: {Quantity}, ItemType: '{ItemType}', BatchNumber: '{BatchNumber}', User: '{User}', Location: '{Location}', Notes: '{Notes}', ReceiveDate: {ReceiveDate}, LastUpdated: {LastUpdated}",
                EditModel.Id, EditModel.PartId ?? "NULL", EditModel.Operation ?? "NULL", EditModel.Quantity, EditModel.ItemType ?? "NULL", EditModel.BatchNumber ?? "NULL", EditModel.User ?? "NULL", EditModel.Location ?? "NULL", EditModel.Notes ?? "NULL", EditModel.ReceiveDate, EditModel.LastUpdated);

            // Check if current user can edit this record
            var currentUser = Environment.UserName.ToUpper();
            var recordUser = EditModel.User?.ToUpper() ?? string.Empty;

            CanEditRecord = currentUser == recordUser;

            if (!CanEditRecord)
            {
                PermissionErrorMessage = $"You can only edit records you created. This record was created by '{EditModel.User}' but you are '{currentUser}'.";
                Logger.LogWarning("User {CurrentUser} attempted to edit record created by {RecordUser}", currentUser, EditModel.User);
            }
            else
            {
                PermissionErrorMessage = string.Empty;
                Logger.LogInformation("User {CurrentUser} has permission to edit this record", currentUser);
            }

            // Perform initial validation (only for editable fields if user has permission)
            if (CanEditRecord)
            {
                ValidateAllFields();

                // Force initial CanSave evaluation and notification
                Logger.LogDebug("🔍 InitializeAsync(InventoryItem) - Initial validation completed, triggering CanSave notification");
                OnPropertyChanged(nameof(CanSave));
                SaveCommand.NotifyCanExecuteChanged();
            }
            else
            {
                // Even if user can't edit, we should set the validation states properly
                IsOperationValid = true; // Don't block on validation if can't edit anyway
                IsQuantityValid = true;
                Logger.LogDebug("🔍 InitializeAsync(InventoryItem) - User can't edit, setting validation states to true");
                OnPropertyChanged(nameof(CanSave));
                SaveCommand.NotifyCanExecuteChanged();
            }

            Logger.LogInformation("Edit dialog initialized successfully for Part ID: {PartId}, CanEdit: {CanEdit}", EditModel.PartId, CanEditRecord);
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
    /// Only validates editable fields (Operation, Quantity, Notes).
    /// </summary>
    /// <param name="fieldName">The name of the field to validate</param>
    private void ValidateField(string? fieldName)
    {
        if (!CanEditRecord) return; // Don't validate if user can't edit

        switch (fieldName)
        {
            case nameof(EditModel.Operation):
                IsOperationValid = !string.IsNullOrWhiteSpace(EditModel.Operation) &&
                                  AvailableOperations.Contains(EditModel.Operation);
                IsOperationInvalid = !IsOperationValid;
                break;

                // Quantity validation is handled in OnQuantityTextChanged
        }

        // Update overall validation status (only for editable fields)
        HasValidationErrors = IsOperationInvalid || IsQuantityInvalid;

        // Notify that CanSave may have changed
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Validate all editable fields at once (Operation, Quantity, Notes).
    /// </summary>
    private void ValidateAllFields()
    {
        if (!CanEditRecord) return; // Don't validate if user can't edit

        ValidateField(nameof(EditModel.Operation));
        // Quantity validation is handled automatically in OnQuantityTextChanged
        // Notes don't need validation as they're optional
    }

    /// <summary>
    /// Save changes to the inventory item.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSave))]
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

                // Cleanup ViewModel state before closing
                Cleanup();

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

            // Clean up the ViewModel state before closing
            Cleanup();

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

    /// <summary>
    /// Cleans up the ViewModel state when dialog is closed.
    /// Preserves updated data for CustomDataGrid updates while clearing UI state.
    /// </summary>
    public void Cleanup()
    {
        try
        {
            Logger.LogDebug("Cleaning up EditInventoryViewModel state");

            // Reset loading and UI states only
            IsLoading = false;
            HasValidationErrors = false;
            PermissionErrorMessage = string.Empty;

            // Reset validation states
            IsPartIdValid = false;
            IsPartIdInvalid = false;
            IsLocationValid = false;
            IsLocationInvalid = false;
            IsOperationValid = false;
            IsOperationInvalid = false;
            IsQuantityValid = true;
            IsQuantityInvalid = false;
            IsItemTypeValid = true;

            // Note: DO NOT clear master data collections as they are expensive to reload
            // and should be reused across dialog instances for performance

            // Note: DO NOT reset EditModel here - it should only be reset when new data is loaded
            // This preserves data for potential CustomDataGrid updates

            Logger.LogDebug("EditInventoryViewModel cleanup completed - Data preserved for updates");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during EditInventoryViewModel cleanup");
        }
    }
}
