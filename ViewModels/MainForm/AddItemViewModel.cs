using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class AddItemViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Private Fields
    private string _partId = string.Empty;
    private string _location = string.Empty;
    private string _operation = string.Empty;
    private int _quantity = 1;
    private string _itemType = string.Empty;
    private string _notes = string.Empty;
    private bool _isLoading = false;
    private string _statusMessage = string.Empty;
    private ObservableCollection<string> _availableLocations = new();
    private ObservableCollection<string> _availableOperations = new();
    private ObservableCollection<string> _availableItemTypes = new();
    #endregion

    #region Public Properties
    public string PartId
    {
        get => _partId;
        set => SetProperty(ref _partId, value);
    }

    public string Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public string Operation
    {
        get => _operation;
        set => SetProperty(ref _operation, value);
    }

    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public string ItemType
    {
        get => _itemType;
        set => SetProperty(ref _itemType, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ObservableCollection<string> AvailableLocations
    {
        get => _availableLocations;
        set => SetProperty(ref _availableLocations, value);
    }

    public ObservableCollection<string> AvailableOperations
    {
        get => _availableOperations;
        set => SetProperty(ref _availableOperations, value);
    }

    public ObservableCollection<string> AvailableItemTypes
    {
        get => _availableItemTypes;
        set => SetProperty(ref _availableItemTypes, value);
    }
    #endregion

    #region Commands
    public ICommand AddItemCommand { get; private set; }
    public ICommand ClearFormCommand { get; private set; }
    public ICommand LoadMasterDataCommand { get; private set; }
    #endregion

    public AddItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<AddItemViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("AddItemViewModel initialized with dependency injection");

        InitializeCommands();
        _ = LoadMasterDataAsync(); // Load master data on initialization
    }

    private void InitializeCommands()
    {
        AddItemCommand = new AsyncCommand(ExecuteAddItemAsync, CanExecuteAddItem);
        ClearFormCommand = new RelayCommand(ExecuteClearForm);
        LoadMasterDataCommand = new AsyncCommand(LoadMasterDataAsync);

        Logger.LogDebug("Commands initialized for AddItemViewModel");
    }

    private bool CanExecuteAddItem()
    {
        return !IsLoading && 
               !string.IsNullOrWhiteSpace(PartId) && 
               !string.IsNullOrWhiteSpace(Location) && 
               !string.IsNullOrWhiteSpace(Operation) && 
               !string.IsNullOrWhiteSpace(ItemType) && 
               Quantity > 0;
    }

    private async Task ExecuteAddItemAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Adding inventory item...";
            Logger.LogInformation("Adding inventory item: PartId={PartId}, Location={Location}, Operation={Operation}, Quantity={Quantity}", 
                PartId, Location, Operation, Quantity);

            var currentUser = _applicationState.CurrentUser ?? "System";
            
            var result = await _databaseService.AddInventoryItemAsync(
                PartId, Location, Operation, Quantity, ItemType, currentUser, Notes);

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
                    new Dictionary<string, object> { ["Message"] = result.Message, ["PartId"] = PartId });
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
                new Dictionary<string, object> { ["PartId"] = PartId, ["Operation"] = "ExecuteAddItemAsync" });
        }
        finally
        {
            IsLoading = false;
        }
    }

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

    private async Task LoadMasterDataAsync()
    {
        try
        {
            IsLoading = true;
            Logger.LogDebug("Loading master data for AddItemViewModel");

            // Load available item types
            var itemTypesData = await _databaseService.GetAllItemTypesAsync();
            
            // Update collections on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
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
            var locationsData = await _databaseService.GetAllLocationsAsync();
            
            await Dispatcher.UIThread.InvokeAsync(() =>
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
            var operationsData = await _databaseService.GetAllOperationsAsync();
            
            await Dispatcher.UIThread.InvokeAsync(() =>
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
                new Dictionary<string, object> { ["Operation"] = "LoadMasterDataAsync" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
