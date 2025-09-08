using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.Threading;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for the inventory removal interface (Control_RemoveTab).
/// Provides comprehensive functionality for removing inventory items from the system,
/// including search capabilities, batch deletion operations, undo functionality, 
/// and transaction history tracking.
/// </summary>
public partial class RemoveItemViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;
    private readonly ISuggestionOverlayService _suggestionOverlayService;
    private readonly ISuccessOverlayService _successOverlayService;
    private readonly IQuickButtonsService _quickButtonsService;

    #region Observable Collections (InventoryTabView Pattern)
    
    /// <summary>
    /// Available part IDs for filtering (InventoryTabView pattern)
    /// </summary>
    public ObservableCollection<string> PartIds { get; } = new();
    
    /// <summary>
    /// Available operations for filtering (InventoryTabView pattern)
    /// </summary>
    public ObservableCollection<string> Operations { get; } = new();
    
    /// <summary>
    /// Available locations for filtering (InventoryTabView pattern)
    /// </summary>
    public ObservableCollection<string> Locations { get; } = new();
    
    /// <summary>
    /// Available users for filtering (InventoryTabView pattern)
    /// </summary>
    public ObservableCollection<string> Users { get; } = new();

    #endregion

    #region Legacy Observable Collections (for backward compatibility)
    
    /// <summary>
    /// Available part options for filtering
    /// </summary>
    public ObservableCollection<string> PartOptions { get; } = new();
    
    /// <summary>
    /// Available operation options for refined filtering
    /// </summary>
    public ObservableCollection<string> OperationOptions { get; } = new();
    
    /// <summary>
    /// Available location options for filtering by location
    /// </summary>
    public ObservableCollection<string> LocationOptions { get; } = new();
    
    /// <summary>
    /// Available user options for advanced filtering by user
    /// </summary>
    public ObservableCollection<string> UserOptions { get; } = new();
    
    /// <summary>
    /// Current inventory items displayed in the DataGrid
    /// </summary>
    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();
    
    /// <summary>
    /// Currently selected items in the DataGrid for batch operations
    /// </summary>
    public ObservableCollection<InventoryItem> SelectedItems { get; } = new();

    /// <summary>
    /// Currently selected inventory item in the DataGrid
    /// </summary>
    [ObservableProperty]
    private InventoryItem? _selectedItem;

    #endregion

    #region Watermark Properties (InventoryTabView Pattern)

    /// <summary>
    /// Dynamic watermark for Part field - shows error or placeholder
    /// </summary>
    public string PartWatermark => string.IsNullOrWhiteSpace(SelectedPart) ? "Enter part ID to search..." : 
                                  "Enter part ID to search...";

    /// <summary>
    /// Dynamic watermark for Operation field - shows error or placeholder
    /// </summary>
    public string OperationWatermark => "Enter operation (optional)...";

    /// <summary>
    /// Dynamic watermark for Location field - shows error or placeholder
    /// </summary>
    public string LocationWatermark => "Enter location (optional)...";

    /// <summary>
    /// Dynamic watermark for User field - shows error or placeholder
    /// </summary>
    public string UserWatermark => "Enter user (advanced filtering)...";

    #endregion

    #region Search Criteria Properties

    /// <summary>
    /// Selected part ID for filtering inventory.
    /// Must be a valid part ID from the available options.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDelete))]
    private string? _selectedPart;

    /// <summary>
    /// Selected operation for refined filtering (optional).
    /// Must be a valid operation number if specified.
    /// </summary>
    [ObservableProperty]
    private string? _selectedOperation;

    /// <summary>
    /// Selected location for filtering (optional).
    /// Must be a valid location if specified.
    /// </summary>
    [ObservableProperty]
    private string? _selectedLocation;

    /// <summary>
    /// Selected user for advanced filtering (optional).
    /// Must be a valid user if specified.
    /// </summary>
    [ObservableProperty]
    private string? _selectedUser;

    /// <summary>
    /// Text content for Part AutoCompleteBox.
    /// Synchronized with SelectedPart property.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Part text is required for search operations")]
    [StringLength(50, ErrorMessage = "Part text cannot exceed 50 characters")]
    private string _partText = string.Empty;

    /// <summary>
    /// Text content for Operation AutoCompleteBox.
    /// Synchronized with SelectedOperation property.
    /// </summary>
    [ObservableProperty]
    [StringLength(10, ErrorMessage = "Operation text cannot exceed 10 characters")]
    private string _operationText = string.Empty;

    /// <summary>
    /// Text content for Location filtering.
    /// Used for location-based inventory filtering.
    /// </summary>
    [ObservableProperty]
    [StringLength(20, ErrorMessage = "Location text cannot exceed 20 characters")]
    private string _locationText = string.Empty;

    /// <summary>
    /// Text content for User filtering (advanced filtering).
    /// Used for filtering inventory by user who created/modified records.
    /// </summary>
    [ObservableProperty]
    [StringLength(50, ErrorMessage = "User text cannot exceed 50 characters")]
    private string _userText = string.Empty;

    #endregion

    #region State Properties

    /// <summary>
    /// Indicates if a background operation is in progress.
    /// When true, prevents user interactions that could cause conflicts.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDelete), nameof(CanUndo))]
    private bool _isLoading;

    /// <summary>
    /// Indicates if there are items available for undo operation.
    /// Used to enable/disable the undo functionality.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUndo))]
    private bool _hasUndoItems;

    /// <summary>
    /// Indicates if there are inventory items to display
    /// </summary>
    public bool HasInventoryItems => InventoryItems.Count > 0;

    /// <summary>
    /// Indicates if delete operation can be performed (items selected)
    /// </summary>
    public bool CanDelete => SelectedItems.Count > 0 && !IsLoading;

    /// <summary>
    /// Indicates if undo operation is available
    /// </summary>
    public bool CanUndo => HasUndoItems && !IsLoading;

    #endregion

    #region Undo Functionality

    /// <summary>
    /// Stores items from the last removal operation for undo capability
    /// </summary>
    private readonly List<InventoryItem> _lastRemovedItems = new();

    #endregion

    #region Events

    /// <summary>
    /// Event fired when items are successfully removed
    /// </summary>
    public event EventHandler<ItemsRemovedEventArgs>? ItemsRemoved;

    /// <summary>
    /// Event fired when panel toggle is requested
    /// </summary>
    public event EventHandler? PanelToggleRequested;

    /// <summary>
    /// Event fired when advanced removal is requested
    /// </summary>
    public event EventHandler? AdvancedRemovalRequested;

    /// <summary>
    /// Event fired when the success overlay should be shown
    /// </summary>
    public event EventHandler<MTM_WIP_Application_Avalonia.Models.SuccessEventArgs>? ShowSuccessOverlay;

    #endregion

    #region Constructor

    public RemoveItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ISuggestionOverlayService suggestionOverlayService,
        ISuccessOverlayService successOverlayService,
        IQuickButtonsService quickButtonsService,
        ILogger<RemoveItemViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _suggestionOverlayService = suggestionOverlayService ?? throw new ArgumentNullException(nameof(suggestionOverlayService));
        _successOverlayService = successOverlayService ?? throw new ArgumentNullException(nameof(successOverlayService));
        _quickButtonsService = quickButtonsService ?? throw new ArgumentNullException(nameof(quickButtonsService));

        Logger.LogInformation("RemoveItemViewModel initialized with dependency injection and overlay services");

        _ = LoadData(); // Load real data from database
        
        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update computed properties when dependencies change
        switch (e.PropertyName)
        {
            case nameof(SelectedItems):
            case nameof(IsLoading):
                OnPropertyChanged(nameof(CanDelete));
                break;
            case nameof(HasUndoItems):
                OnPropertyChanged(nameof(CanUndo));
                break;
            case nameof(InventoryItems):
                OnPropertyChanged(nameof(HasInventoryItems));
                break;
            case nameof(SelectedPart):
                PartText = SelectedPart ?? string.Empty;
                OnPropertyChanged(nameof(PartWatermark));
                break;
            case nameof(SelectedOperation):
                OperationText = SelectedOperation ?? string.Empty;
                OnPropertyChanged(nameof(OperationWatermark));
                break;
            case nameof(SelectedLocation):
                LocationText = SelectedLocation ?? string.Empty;
                OnPropertyChanged(nameof(LocationWatermark));
                break;
            case nameof(SelectedUser):
                UserText = SelectedUser ?? string.Empty;
                OnPropertyChanged(nameof(UserWatermark));
                break;
            case nameof(PartText):
                if (!string.IsNullOrEmpty(PartText) && (PartOptions.Contains(PartText) || PartIds.Contains(PartText)))
                    SelectedPart = PartText;
                break;
            case nameof(OperationText):
                if (!string.IsNullOrEmpty(OperationText) && (OperationOptions.Contains(OperationText) || Operations.Contains(OperationText)))
                    SelectedOperation = OperationText;
                break;
            case nameof(LocationText):
                if (!string.IsNullOrEmpty(LocationText) && (LocationOptions.Contains(LocationText) || Locations.Contains(LocationText)))
                    SelectedLocation = LocationText;
                break;
            case nameof(UserText):
                if (!string.IsNullOrEmpty(UserText) && (UserOptions.Contains(UserText) || Users.Contains(UserText)))
                    SelectedUser = UserText;
                break;
        }
    }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Opens advanced removal features
    /// </summary>
    [RelayCommand]
    private void AdvancedRemoval()
    {
        AdvancedRemovalRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Toggles quick actions panel
    /// </summary>
    [RelayCommand]
    private void TogglePanel()
    {
        PanelToggleRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Executes inventory search based on selected criteria with progress tracking
    /// </summary>
    [RelayCommand]
    private async Task Search()
    {
        try
        {
            IsLoading = true;
            InventoryItems.Clear();

            using var scope = Logger.BeginScope("InventorySearch");
            Logger.LogInformation("Executing search for Part: {PartId}, Operation: {Operation}, Location: {Location}, User: {User}", 
                SelectedPart, SelectedOperation, LocationText, UserText);

            // Validate search criteria
            if (string.IsNullOrWhiteSpace(SelectedPart))
            {
                Logger.LogWarning("No part ID specified for search operation");
                throw new InvalidOperationException("Part ID is required for inventory search");
            }

            // Dynamic search based on selection criteria
            System.Data.DataTable result;
            
            if (!string.IsNullOrWhiteSpace(SelectedPart) && !string.IsNullOrWhiteSpace(SelectedOperation))
            {
                // Search by both part and operation
                result = await _databaseService.GetInventoryByPartAndOperationAsync(SelectedPart, SelectedOperation)
                    .ConfigureAwait(false);
            }
            else if (!string.IsNullOrWhiteSpace(SelectedPart))
            {
                // Search by part only
                result = await _databaseService.GetInventoryByPartIdAsync(SelectedPart)
                    .ConfigureAwait(false);
            }
            else
            {
                // No search criteria specified, don't load anything
                Logger.LogWarning("No search criteria specified");
                return;
            }

            // Convert DataTable to InventoryItem objects and apply client-side filtering
            foreach (System.Data.DataRow row in result.Rows)
            {
                var inventoryItem = new InventoryItem
                {
                    ID = Convert.ToInt32(row["ID"]),
                    PartID = row["PartID"]?.ToString() ?? string.Empty,
                    Location = row["Location"]?.ToString() ?? string.Empty,
                    Operation = row["Operation"]?.ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ItemType = row["ItemType"]?.ToString() ?? "WIP",
                    ReceiveDate = Convert.ToDateTime(row["ReceiveDate"]),
                    LastUpdated = Convert.ToDateTime(row["LastUpdated"]),
                    User = row["User"]?.ToString() ?? string.Empty,
                    BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                    Notes = row["Notes"]?.ToString() ?? string.Empty
                };
                
                // Apply client-side filtering for Location and User if specified
                var includeItem = true;
                
                // Filter by Location if specified (use InventoryTabView pattern)
                if (!string.IsNullOrWhiteSpace(SelectedLocation) || !string.IsNullOrWhiteSpace(LocationText))
                {
                    var locationFilter = !string.IsNullOrWhiteSpace(SelectedLocation) ? SelectedLocation : LocationText;
                    includeItem = includeItem && 
                        inventoryItem.Location.Contains(locationFilter, StringComparison.OrdinalIgnoreCase);
                }
                
                // Filter by User if specified (use InventoryTabView pattern)
                if (!string.IsNullOrWhiteSpace(SelectedUser) || !string.IsNullOrWhiteSpace(UserText))
                {
                    var userFilter = !string.IsNullOrWhiteSpace(SelectedUser) ? SelectedUser : UserText;
                    includeItem = includeItem && 
                        inventoryItem.User.Contains(userFilter, StringComparison.OrdinalIgnoreCase);
                }
                
                if (includeItem)
                {
                    InventoryItems.Add(inventoryItem);
                }
            }

            Logger.LogInformation("Search completed. Found {Count} inventory items", InventoryItems.Count);
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Invalid search operation: {Message}", ex.Message);
            throw; // Re-throw for UI handling
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during inventory search for Part: {PartId}, Operation: {Operation}", 
                SelectedPart, SelectedOperation);
            throw new ApplicationException($"Failed to search inventory: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Resets search criteria and refreshes all data
    /// </summary>
    [RelayCommand]
    private async Task Reset()
    {
        try
        {
            IsLoading = true;

            using var scope = Logger.BeginScope("InventoryReset");
            Logger.LogInformation("Resetting search criteria and refreshing data");

            // Clear search criteria
            SelectedPart = null;
            SelectedOperation = null;
            SelectedLocation = null; // InventoryTabView pattern
            SelectedUser = null; // InventoryTabView pattern
            PartText = string.Empty;
            OperationText = string.Empty;
            LocationText = string.Empty;
            UserText = string.Empty;
            InventoryItems.Clear();
            SelectedItem = null;

            // Reload all ComboBox data
            await LoadData().ConfigureAwait(false);

            Logger.LogInformation("Search criteria reset and data refreshed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to reset search criteria and refresh data");
            throw new ApplicationException("Failed to reset inventory data", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Batch deletes selected items with transaction logging.
    /// Validates item state before deletion and maintains undo capability.
    /// Supports both single item and multi-item batch operations.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task Delete()
    {
        if (SelectedItems.Count == 0)
        {
            Logger.LogWarning("Delete operation attempted with no items selected");
            throw new InvalidOperationException("No items selected for deletion");
        }

        try
        {
            IsLoading = true;
            var itemsToRemove = SelectedItems.ToList(); // Create a copy to avoid collection modification during iteration
            var successfulRemovals = new List<InventoryItem>();
            var failures = new List<(InventoryItem Item, string Error)>();

            using var scope = Logger.BeginScope("BatchInventoryDeletion");
            Logger.LogInformation("Removing {Count} inventory items", itemsToRemove.Count);

            // Process each item in the batch
            foreach (var itemToRemove in itemsToRemove)
            {
                try
                {
                    // Validate item data
                    if (string.IsNullOrWhiteSpace(itemToRemove.PartID))
                    {
                        failures.Add((itemToRemove, "Invalid Part ID"));
                        continue;
                    }

                    if (itemToRemove.Quantity <= 0)
                    {
                        failures.Add((itemToRemove, "Invalid quantity"));
                        continue;
                    }

                    Logger.LogDebug("Processing removal: {PartId}, Operation: {Operation}, Quantity: {Quantity}", 
                        itemToRemove.PartID, itemToRemove.Operation, itemToRemove.Quantity);

                    // Remove item using database service with proper async handling
                    var removeResult = await _databaseService.RemoveInventoryItemAsync(
                        itemToRemove.PartID,
                        itemToRemove.Location,
                        itemToRemove.Operation ?? string.Empty,
                        itemToRemove.Quantity,
                        itemToRemove.ItemType,
                        _applicationState.CurrentUser,
                        itemToRemove.BatchNumber ?? string.Empty,
                        "Removed via Remove Item interface - Batch Operation"
                    ).ConfigureAwait(false);

                    if (removeResult.IsSuccess)
                    {
                        successfulRemovals.Add(itemToRemove);
                        Logger.LogDebug("Successfully removed inventory item: {PartId}", itemToRemove.PartID);
                    }
                    else
                    {
                        failures.Add((itemToRemove, removeResult.Message ?? "Database operation failed"));
                        Logger.LogError("Failed to remove inventory item {PartId}: {Error}", itemToRemove.PartID, removeResult.Message);
                    }
                }
                catch (Exception itemEx)
                {
                    failures.Add((itemToRemove, itemEx.Message));
                    Logger.LogError(itemEx, "Exception removing inventory item {PartId}", itemToRemove.PartID);
                }
            }

            // Update UI and undo capability based on results
            if (successfulRemovals.Count > 0)
            {
                // Store for undo capability (replace previous undo items)
                _lastRemovedItems.Clear();
                _lastRemovedItems.AddRange(successfulRemovals);
                HasUndoItems = _lastRemovedItems.Count > 0;

                // Remove successful items from UI collections
                foreach (var removedItem in successfulRemovals)
                {
                    InventoryItems.Remove(removedItem);
                }

                // Clear selection since items were deleted
                SelectedItems.Clear();
                SelectedItem = null;

                // Log successful removals to QuickButtons history (RED color for OUT transactions)
                foreach (var removedItem in successfulRemovals)
                {
                    try
                    {
                        await _quickButtonsService.AddTransactionToLast10Async(
                            _applicationState.CurrentUser,
                            removedItem.PartID,
                            removedItem.Operation ?? string.Empty,
                            removedItem.Quantity
                        );
                        Logger.LogDebug("Logged removal to QuickButtons history: {PartId}", removedItem.PartID);
                    }
                    catch (Exception logEx)
                    {
                        Logger.LogWarning(logEx, "Failed to log removal to QuickButtons history for {PartId}", removedItem.PartID);
                    }
                }

                // Show SuccessOverlay for successful operations
                var successMessage = successfulRemovals.Count == 1
                    ? $"Successfully removed inventory item"
                    : $"Successfully removed {successfulRemovals.Count} inventory items";

                var detailsText = successfulRemovals.Count == 1
                    ? $"Part ID: {successfulRemovals[0].PartID}\nOperation: {successfulRemovals[0].Operation}\nLocation: {successfulRemovals[0].Location}\nQuantity: {successfulRemovals[0].Quantity}"
                    : $"Batch operation completed\nItems removed: {successfulRemovals.Count}\nTotal quantity removed: {successfulRemovals.Sum(x => x.Quantity)}";

                // Fire SuccessOverlay event for View to handle
                var successArgs = new MTM_WIP_Application_Avalonia.Models.SuccessEventArgs
                {
                    Message = successMessage,
                    Details = detailsText,
                    IconKind = "CheckCircle",
                    Duration = 4000, // 4 seconds for removal confirmation
                    SuccessTime = DateTime.Now
                };

                Logger.LogInformation("About to fire ShowSuccessOverlay event for {Count} removed items", successfulRemovals.Count);
                ShowSuccessOverlay?.Invoke(this, successArgs);

                // Also use SuccessOverlay service directly
                try
                {
                    if (_successOverlayService != null)
                    {
                        // Use service to show overlay directly in MainView
                        _ = _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                            null, // Auto-resolve MainView
                            successMessage,
                            detailsText,
                            "CheckCircle",
                            4000 // 4 seconds total
                        );
                        Logger.LogInformation("Success overlay started for inventory removal operation (4 second duration)");
                    }
                }
                catch (Exception overlayEx)
                {
                    Logger.LogWarning(overlayEx, "Failed to show success overlay via service");
                }

                // Fire event for integration
                ItemsRemoved?.Invoke(this, new ItemsRemovedEventArgs
                {
                    RemovedItems = successfulRemovals,
                    RemovalTime = DateTime.Now
                });

                Logger.LogInformation("Batch deletion completed: {SuccessCount} successful, {FailureCount} failed", 
                    successfulRemovals.Count, failures.Count);
            }

            // Report any failures
            if (failures.Count > 0)
            {
                var failureMessage = $"Failed to remove {failures.Count} items:\n" + 
                    string.Join("\n", failures.Select(f => $"• {f.Item.PartID}: {f.Error}"));
                Logger.LogWarning("Batch deletion had failures: {FailureMessage}", failureMessage);
                
                // In a real implementation, you might want to show this to the user
                // For now, we'll just log it
            }

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during batch inventory deletion");
            throw new ApplicationException($"Failed to delete inventory items: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Restores last deleted items using undo functionality
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
    {
        if (_lastRemovedItems.Count == 0)
        {
            Logger.LogWarning("Undo operation attempted with no items to restore");
            return;
        }

        try
        {
            IsLoading = true;

            // TODO: Implement database restoration
            // foreach (var item in _lastRemovedItems)
            // {
            //     await Dao_Inventory.AddInventoryItemAsync(
            //         user: Model_AppVariables.User,
            //         partId: item.PartId,
            //         operation: item.Operation,
            //         location: item.Location,
            //         quantity: item.Quantity,
            //         notes: $"Restored via Undo: {item.Notes}"
            //     );
            // }

            // Restore to UI collections
            foreach (var item in _lastRemovedItems)
            {
                InventoryItems.Add(item);
            }

            Logger.LogInformation("Successfully restored {Count} inventory items via undo", 
                _lastRemovedItems.Count);

            _lastRemovedItems.Clear();
            HasUndoItems = _lastRemovedItems.Count > 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Prints current inventory view with formatted output
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasInventoryItems))]
    private async Task Print()
    {
        try
        {
            IsLoading = true;

            // TODO: Implement print functionality using Core_DgvPrinter equivalent
            // var printer = new AvaloniaDataGridPrinter();
            // await printer.PrintDataGridAsync(InventoryItems, 
            //     title: "Inventory Removal Report",
            //     searchCriteria: $"Part: {SelectedPart}, Operation: {SelectedOperation}");

            Logger.LogInformation("Print operation initiated for {Count} inventory items", 
                InventoryItems.Count);

            await Task.Delay(1000); // Simulate print operation
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Loads ComboBox data from database using stored procedures.
    /// Populates PartOptions and OperationOptions collections.
    /// </summary>
    [RelayCommand]
    private async Task LoadData()
    {
        try
        {
            using var scope = Logger.BeginScope("DataLoading");
            Logger.LogInformation("Loading ComboBox data from database");

            // Load Parts using md_part_ids_Get_All stored procedure
            var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            ).ConfigureAwait(false);

            if (partResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    PartOptions.Clear();
                    PartIds.Clear(); // InventoryTabView pattern
                    foreach (System.Data.DataRow row in partResult.Data.Rows)
                    {
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartOptions.Add(partId);
                            PartIds.Add(partId); // InventoryTabView pattern
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} parts", PartOptions.Count);
            }
            
            // Load Operations using md_operation_numbers_Get_All stored procedure
            var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            ).ConfigureAwait(false);

            if (operationResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    OperationOptions.Clear();
                    Operations.Clear(); // InventoryTabView pattern
                    foreach (System.Data.DataRow row in operationResult.Data.Rows)
                    {
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            OperationOptions.Add(operation);
                            Operations.Add(operation); // InventoryTabView pattern
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} operations", OperationOptions.Count);
            }

            // Load Locations using md_locations_Get_All stored procedure
            var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_locations_Get_All",
                new Dictionary<string, object>()
            ).ConfigureAwait(false);

            if (locationResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    LocationOptions.Clear();
                    Locations.Clear(); // InventoryTabView pattern
                    foreach (System.Data.DataRow row in locationResult.Data.Rows)
                    {
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            LocationOptions.Add(location);
                            Locations.Add(location); // InventoryTabView pattern
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} locations", LocationOptions.Count);
            }

            // Load Users using usr_users_Get_All stored procedure  
            var userResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "usr_users_Get_All",
                new Dictionary<string, object>()
            ).ConfigureAwait(false);

            if (userResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    UserOptions.Clear();
                    Users.Clear(); // InventoryTabView pattern
                    foreach (System.Data.DataRow row in userResult.Data.Rows)
                    {
                        // Note: User table column is "User" but property is User_Name to avoid conflicts
                        var user = row["User"]?.ToString();
                        if (!string.IsNullOrEmpty(user))
                        {
                            UserOptions.Add(user);
                            Users.Add(user); // InventoryTabView pattern
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} users", UserOptions.Count);
            }

            Logger.LogInformation("ComboBox data loaded successfully - Parts: {PartCount}, Operations: {OperationCount}, Locations: {LocationCount}, Users: {UserCount}", 
                PartOptions.Count, OperationOptions.Count, LocationOptions.Count, UserOptions.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load ComboBox data from database");
            throw new ApplicationException("Failed to initialize application data", ex);
        }
    }

    /// <summary>
    /// Loads sample data for demonstration purposes
    /// </summary>
    private Task LoadSampleDataAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            // Clear existing data
            PartOptions.Clear();
            PartIds.Clear(); // InventoryTabView pattern
            OperationOptions.Clear();
            Operations.Clear(); // InventoryTabView pattern
            LocationOptions.Clear();
            Locations.Clear(); // InventoryTabView pattern
            UserOptions.Clear();
            Users.Clear(); // InventoryTabView pattern

            // Sample parts
            var sampleParts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005" };
            foreach (var part in sampleParts)
            {
                PartOptions.Add(part);
                PartIds.Add(part); // InventoryTabView pattern
            }

            // Sample operations (MTM uses string numbers)
            var sampleOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in sampleOperations)
            {
                OperationOptions.Add(operation);
                Operations.Add(operation); // InventoryTabView pattern
            }

            // Sample locations
            var sampleLocations = new[] { "A01", "A02", "B01", "B02", "WC01", "WC02", "LINE1", "LINE2" };
            foreach (var location in sampleLocations)
            {
                LocationOptions.Add(location);
                Locations.Add(location); // InventoryTabView pattern
            }

            // Sample users
            var sampleUsers = new[] { "TestUser", "Operator1", "Operator2", "Supervisor", "Admin" };
            foreach (var user in sampleUsers)
            {
                UserOptions.Add(user);
                Users.Add(user); // InventoryTabView pattern
            }
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads sample inventory data for demonstration
    /// </summary>
    private Task LoadSampleInventoryDataAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var sampleItems = new[]
            {
                new InventoryItem
                {
                    ID = 1,
                    PartID = "PART001",
                    Operation = "100",
                    Location = "WC01",
                    Quantity = 25,
                    Notes = "Ready for next operation",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddHours(-2)
                },
                new InventoryItem
                {
                    ID = 2,
                    PartID = "PART001", 
                    Operation = "110",
                    Location = "WC02",
                    Quantity = 15,
                    Notes = "Quality check required",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddHours(-1)
                },
                new InventoryItem
                {
                    ID = 3,
                    PartID = "PART002",
                    Operation = "90",
                    Location = "WC01",
                    Quantity = 40,
                    Notes = "Incoming from supplier",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddMinutes(-30)
                }
            };

            // Filter sample data based on search criteria
            var filteredItems = sampleItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedPart))
            {
                filteredItems = filteredItems.Where(item => 
                    item.PartID.Equals(SelectedPart, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedOperation))
            {
                filteredItems = filteredItems.Where(item => 
                    item.Operation?.Equals(SelectedOperation, StringComparison.OrdinalIgnoreCase) == true);
            }

            foreach (var item in filteredItems)
            {
                InventoryItems.Add(item);
            }
        });
        return Task.CompletedTask;
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles exceptions from command operations
    /// </summary>
    /// <summary>
    /// Handles exceptions with user-friendly error presentation
    /// </summary>
    private void HandleException(Exception ex)
    {
        Logger.LogError(ex, "Error in RemoveItemViewModel operation");
        
        // Present user-friendly error message via centralized error service
        _ = Services.ErrorHandling.HandleErrorAsync(ex, "Remove Operation", _applicationState.CurrentUser);
        
        // Update UI state to reflect error
        // Note: StatusMessage property may need to be added to this ViewModel for UI feedback
        Logger.LogInformation("User-friendly error message: {Message}", GetUserFriendlyErrorMessage(ex));
    }
    
    /// <summary>
    /// Gets a user-friendly error message based on the exception type
    /// </summary>
    private string GetUserFriendlyErrorMessage(Exception ex) => ex switch
    {
        InvalidOperationException => "The removal operation could not be completed. Please verify the item details and try again.",
        TimeoutException => "The removal operation timed out. Please check your connection and try again.",
        UnauthorizedAccessException => "You do not have permission to perform this removal operation.",
        ArgumentException => "Invalid removal details provided. Please check your input and try again.",
        _ => "An unexpected error occurred during the removal operation. Please contact support if this continues."
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Programmatically triggers a search operation
    /// </summary>
    public async Task TriggerSearchAsync()
    {
        if (SearchCommand.CanExecute(null))
        {
            await SearchCommand.ExecuteAsync(null);
        }
    }

    /// <summary>
    /// Selects all visible inventory items
    /// </summary>
    public void SelectAllItems()
    {
        SelectedItems.Clear();
        foreach (var item in InventoryItems)
        {
            SelectedItems.Add(item);
        }
    }

    /// <summary>
    /// Clears all selected items
    /// </summary>
    public void ClearSelection()
    {
        SelectedItem = null;
        SelectedItems.Clear();
    }

    #region SuggestionOverlay Integration Methods

    /// <summary>
    /// Shows part ID suggestions using the SuggestionOverlay service
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowPartSuggestionsAsync(Control targetControl, string userInput)
    {
        try
        {
            Logger.LogDebug("Showing part suggestions for input: {Input}", userInput);
            return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, PartOptions, userInput);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show part suggestions");
            return null;
        }
    }

    /// <summary>
    /// Shows operation suggestions using the SuggestionOverlay service
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowOperationSuggestionsAsync(Control targetControl, string userInput)
    {
        try
        {
            Logger.LogDebug("Showing operation suggestions for input: {Input}", userInput);
            return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, OperationOptions, userInput);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show operation suggestions");
            return null;
        }
    }

    /// <summary>
    /// Shows location suggestions using the SuggestionOverlay service
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowLocationSuggestionsAsync(Control targetControl, string userInput)
    {
        try
        {
            Logger.LogDebug("Showing location suggestions for input: {Input}", userInput);
            return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, LocationOptions, userInput);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show location suggestions");
            return null;
        }
    }

    /// <summary>
    /// Shows user suggestions using the SuggestionOverlay service
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowUserSuggestionsAsync(Control targetControl, string userInput)
    {
        try
        {
            Logger.LogDebug("Showing user suggestions for input: {Input}", userInput);
            return await _suggestionOverlayService.ShowSuggestionsAsync(targetControl, UserOptions, userInput);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to show user suggestions");
            return null;
        }
    }

    /// <summary>
    /// Handles QuickButton integration for field population
    /// </summary>
    /// <param name="partId">Part ID from QuickButton</param>
    /// <param name="operation">Operation from QuickButton</param>
    /// <param name="location">Location from QuickButton</param>
    public void PopulateFromQuickButton(string? partId, string? operation, string? location)
    {
        try
        {
            Logger.LogInformation("Populating fields from QuickButton: Part={PartId}, Operation={Operation}, Location={Location}", 
                partId, operation, location);

            if (!string.IsNullOrWhiteSpace(partId))
            {
                SelectedPart = partId; // InventoryTabView pattern
            }

            if (!string.IsNullOrWhiteSpace(operation))
            {
                SelectedOperation = operation; // InventoryTabView pattern
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                SelectedLocation = location; // InventoryTabView pattern
            }

            // Auto-execute search after populating fields
            _ = Search();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to populate fields from QuickButton");
        }
    }

    #endregion

    #endregion
}

#region Event Args

/// <summary>
/// Event arguments for items removed event
/// </summary>
public class ItemsRemovedEventArgs : EventArgs
{
    public List<InventoryItem> RemovedItems { get; set; } = new();
    public DateTime RemovalTime { get; set; }
    public int TotalQuantityRemoved => RemovedItems.Sum(item => item.Quantity);
}
#endregion
