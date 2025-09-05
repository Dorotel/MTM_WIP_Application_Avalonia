using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Models.Database;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for adding new inventory items to the MTM system.
/// Provides functionality for item creation with validation and master data loading.
/// Uses MVVM Community Toolkit for property change notifications and command handling.
/// </summary>
public partial class AddItemViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the part identifier for the new inventory item.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    [NotifyCanExecuteChangedFor(nameof(ExecuteAddItemCommand))]
    private string partId = string.Empty;

    /// <summary>
    /// Gets or sets the location where the inventory item will be stored.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Location is required")]
    [StringLength(30, ErrorMessage = "Location cannot exceed 30 characters")]
    [NotifyCanExecuteChangedFor(nameof(ExecuteAddItemCommand))]
    private string location = string.Empty;

    /// <summary>
    /// Gets or sets the operation number for the inventory item workflow.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Operation is required")]
    [StringLength(10, ErrorMessage = "Operation cannot exceed 10 characters")]
    [NotifyCanExecuteChangedFor(nameof(ExecuteAddItemCommand))]
    private string operation = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of items to add to inventory.
    /// </summary>
    [ObservableProperty]
    [Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999,999")]
    [NotifyCanExecuteChangedFor(nameof(ExecuteAddItemCommand))]
    private int quantity = 1;

    /// <summary>
    /// Gets or sets the type of inventory item being added.
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Item type is required")]
    [StringLength(20, ErrorMessage = "Item type cannot exceed 20 characters")]
    [NotifyCanExecuteChangedFor(nameof(ExecuteAddItemCommand))]
    private string itemType = string.Empty;

    /// <summary>
    /// Gets or sets optional notes for the inventory item.
    /// </summary>
    [ObservableProperty]
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    private string notes = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the ViewModel is currently loading data.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteAddItemCommand))]
    private bool isLoading = false;

    /// <summary>
    /// Gets or sets the current status message for user feedback.
    /// </summary>
    [ObservableProperty]
    private string statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets the collection of available locations for selection.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> availableLocations = new();

    /// <summary>
    /// Gets or sets the collection of available operations for selection.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> availableOperations = new();

    /// <summary>
    /// Gets or sets the collection of available item types for selection.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> availableItemTypes = new();

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="AddItemViewModel"/> class.
    /// </summary>
    /// <param name="applicationState">The application state service for managing user context.</param>
    /// <param name="databaseService">The database service for inventory operations.</param>
    /// <param name="logger">The logger for this ViewModel.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
    public AddItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<AddItemViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("AddItemViewModel initialized with dependency injection");

        _ = LoadMasterDataAsync(); // Load master data on initialization
    }

    #region Command Methods

    /// <summary>
    /// Determines whether the add item command can be executed.
    /// </summary>
    /// <returns>True if all required fields are valid and not loading; otherwise, false.</returns>
    private bool CanExecuteAddItem()
    {
        return !IsLoading && 
               !string.IsNullOrWhiteSpace(PartId) && 
               !string.IsNullOrWhiteSpace(Location) && 
               !string.IsNullOrWhiteSpace(Operation) && 
               !string.IsNullOrWhiteSpace(ItemType) && 
               Quantity > 0;
    }

    /// <summary>
    /// Adds a new inventory item to the database with comprehensive validation and error handling.
    /// </summary>
    /// <returns>A task representing the asynchronous add operation.</returns>
    [RelayCommand(CanExecute = nameof(CanExecuteAddItem))]
    private async Task ExecuteAddItemAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Adding inventory item...";
            
            using var scope = Logger.BeginScope("AddInventoryItem");
            Logger.LogInformation("Adding inventory item: PartId={PartId}, Location={Location}, Operation={Operation}, Quantity={Quantity}", 
                PartId, Location, Operation, Quantity);

            var currentUser = _applicationState.CurrentUser ?? "System";
            
            var request = new AddInventoryRequest
            {
                PartId = PartId,
                Location = Location,
                Operation = Operation,
                Quantity = Quantity,
                ItemType = ItemType,
                User = currentUser,
                Notes = Notes
            };
            
            var result = await _databaseService.AddInventoryItemAsync(request).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                StatusMessage = $"Successfully added {Quantity} {ItemType}(s) of part {PartId} to {Location}";
                Logger.LogInformation("Successfully added inventory item: {Message}", result.Message);
                
                // Clear form after successful addition
                ExecuteClearForm();
            }
            else
            {
                StatusMessage = $"Failed to add inventory item: {result.Message}";
                Logger.LogError("Failed to add inventory item: {Message}", result.Message);
                
                await ErrorHandling.HandleErrorAsync(
                    new Exception(result.Message),
                    "Add Inventory Item",
                    _applicationState.CurrentUser ?? "System",
                    new Dictionary<string, object> { ["Message"] = result.Message, ["PartId"] = PartId }).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "An unexpected error occurred while adding the inventory item.";
            Logger.LogError(ex, "Unexpected error in ExecuteAddItemAsync");
            
            await ErrorHandling.HandleErrorAsync(
                ex,
                "Add Inventory Item",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["PartId"] = PartId, ["Operation"] = "ExecuteAddItemAsync" }).ConfigureAwait(false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Clears all form fields and resets the form to initial state.
    /// </summary>
    [RelayCommand]
    private void ExecuteClearForm()
    {
        PartId = string.Empty;
        Location = string.Empty;
        Operation = string.Empty;
        Quantity = 1;
        ItemType = string.Empty;
        Notes = string.Empty;
        StatusMessage = string.Empty;
        
        Logger.LogDebug("Form cleared in AddItemViewModel");
    }

    /// <summary>
    /// Loads master data including available locations, operations, and item types.
    /// </summary>
    /// <returns>A task representing the asynchronous load operation.</returns>
    [RelayCommand]
    private async Task LoadMasterDataAsync()
    {
        try
        {
            IsLoading = true;
            
            using var scope = Logger.BeginScope("LoadMasterData");
            Logger.LogDebug("Loading master data for AddItemViewModel");

            // Load available item types
            var itemTypesData = await _databaseService.GetAllItemTypesAsync().ConfigureAwait(false);
            
            // Update collections on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                var itemTypes = new ObservableCollection<string>();
                foreach (System.Data.DataRow row in itemTypesData.Rows)
                {
                    if (row["item_type"] != null)
                    {
                        itemTypes.Add(row["item_type"].ToString() ?? string.Empty);
                    }
                }
                AvailableItemTypes = itemTypes;
            });

            // Load available locations
            var locationsData = await _databaseService.GetAllLocationsAsync().ConfigureAwait(false);
            
            Dispatcher.UIThread.Post(() =>
            {
                var locations = new ObservableCollection<string>();
                foreach (System.Data.DataRow row in locationsData.Rows)
                {
                    if (row["location"] != null)
                    {
                        locations.Add(row["location"].ToString() ?? string.Empty);
                    }
                }
                AvailableLocations = locations;
            });

            // Load available operations
            var operationsData = await _databaseService.GetAllOperationsAsync().ConfigureAwait(false);
            
            Dispatcher.UIThread.Post(() =>
            {
                var operations = new ObservableCollection<string>();
                foreach (System.Data.DataRow row in operationsData.Rows)
                {
                    if (row["operation"] != null)
                    {
                        operations.Add(row["operation"].ToString() ?? string.Empty);
                    }
                }
                AvailableOperations = operations;
            });

            Logger.LogInformation("Master data loaded successfully: {ItemTypes} item types, {Locations} locations, {Operations} operations", 
                AvailableItemTypes.Count, AvailableLocations.Count, AvailableOperations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading master data for AddItemViewModel");
            StatusMessage = "Error loading master data. Some dropdowns may be empty.";
            
            await ErrorHandling.HandleErrorAsync(
                ex,
                "Load Master Data",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadMasterDataAsync" }).ConfigureAwait(false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}
