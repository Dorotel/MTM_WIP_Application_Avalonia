using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// MainViewViewModel manages the main application view including tab navigation,
/// child ViewModels, and status display. Uses MVVM Community Toolkit for property
/// and command management with comprehensive dependency injection.
/// </summary>
public partial class MainViewViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Gets or sets the currently selected tab index (0-based)
    /// </summary>
    [ObservableProperty]
    private int _selectedTabIndex;

    /// <summary>
    /// Gets or sets the content for the inventory tab (normal or advanced view)
    /// </summary>
    [ObservableProperty]
    private object? _inventoryContent;

    /// <summary>
    /// Gets or sets the content for the remove tab (normal or advanced view)
    /// </summary>
    [ObservableProperty]
    private object? _removeContent;

    /// <summary>
    /// Gets or sets the content for the transfer tab
    /// </summary>
    [ObservableProperty]
    private object? _transferContent;

    /// <summary>
    /// Gets or sets the content for the test controls tab
    /// </summary>
    [ObservableProperty]
    private object? _testControlsContent;

    /// <summary>
    /// Gets or sets whether the Quick Actions panel is visible
    /// </summary>
    [ObservableProperty]
    private bool _isAdvancedPanelVisible = true;

    /// <summary>
    /// Gets or sets whether the Quick Actions panel is expanded
    /// </summary>
    [ObservableProperty]
    private bool _isQuickActionsPanelExpanded = true;

    /// <summary>
    /// Gets or sets whether the inventory tab is in advanced mode
    /// </summary>
    [ObservableProperty]
    private bool _isAdvancedInventoryMode;

    /// <summary>
    /// Gets or sets whether the remove tab is in advanced mode
    /// </summary>
    [ObservableProperty]
    private bool _isAdvancedRemoveMode;

    /// <summary>
    /// Gets or sets the database connection status text
    /// </summary>
    [ObservableProperty]
    private string _connectionStatus = "Disconnected";

    /// <summary>
    /// Gets or sets the connection strength percentage (0-100)
    /// </summary>
    [ObservableProperty]
    private int _connectionStrength = 0;

    /// <summary>
    /// Gets or sets the progress value percentage (0-100)
    /// </summary>
    [ObservableProperty]
    private int _progressValue = 0;

    /// <summary>
    /// Gets or sets the status text displayed in the status bar
    /// </summary>
    [ObservableProperty]
    private string _statusText = "Ready";

    /// <summary>
    /// Gets or sets whether the development menu is visible
    /// </summary>
    [ObservableProperty]
    private bool _showDevelopmentMenu = true; // TODO: Bind to build config/environment

    /// <summary>
    /// Gets the toggle text for the advanced panel button
    /// </summary>
    public string AdvancedPanelToggleText => IsAdvancedPanelVisible ? "Hide" : "Show";

    // Child ViewModels
    public QuickButtonsViewModel QuickButtonsViewModel { get; }
    public InventoryTabViewModel InventoryTabViewModel { get; }
    public AdvancedInventoryViewModel AdvancedInventoryViewModel { get; }
    public RemoveItemViewModel RemoveItemViewModel { get; }
    public AdvancedRemoveViewModel AdvancedRemoveViewModel { get; }
    public TransferItemViewModel TransferItemViewModel { get; }

    /// <summary>
    /// Handles tab selection changes and triggers mode resets
    /// </summary>
    partial void OnSelectedTabIndexChanged(int value)
    {
        OnTabSelectionChanged(value);
        HandleTabModeResets(value);
    }

    /// <summary>
    /// Handles advanced panel visibility changes and updates toggle text
    /// </summary>
    partial void OnIsAdvancedPanelVisibleChanged(bool value)
    {
        OnPropertyChanged(nameof(AdvancedPanelToggleText));
    }

    public MainViewViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        InventoryTabViewModel inventoryTabViewModel,
        RemoveItemViewModel removeItemViewModel,
        TransferItemViewModel transferItemViewModel,
        AdvancedInventoryViewModel advancedInventoryViewModel,
        AdvancedRemoveViewModel advancedRemoveViewModel,
        QuickButtonsViewModel quickButtonsViewModel,
        IServiceProvider serviceProvider,
    // SuggestionOverlayViewModel removed for overlay ViewModel reset
        ILogger<MainViewViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        Logger.LogInformation("MainViewViewModel initialized with dependency injection");

        // Bind progress properties to ApplicationStateService for centralized progress system
        _applicationState.PropertyChanged += OnApplicationStateChanged;

        // Inject child ViewModels instead of creating them
        QuickButtonsViewModel = quickButtonsViewModel ?? throw new ArgumentNullException(nameof(quickButtonsViewModel));
        InventoryTabViewModel = inventoryTabViewModel ?? throw new ArgumentNullException(nameof(inventoryTabViewModel));
        RemoveItemViewModel = removeItemViewModel ?? throw new ArgumentNullException(nameof(removeItemViewModel));
        TransferItemViewModel = transferItemViewModel ?? throw new ArgumentNullException(nameof(transferItemViewModel));
        AdvancedInventoryViewModel = advancedInventoryViewModel ?? throw new ArgumentNullException(nameof(advancedInventoryViewModel));
        AdvancedRemoveViewModel = advancedRemoveViewModel ?? throw new ArgumentNullException(nameof(advancedRemoveViewModel));
    // SuggestionOverlayViewModel removed for overlay ViewModel reset

        // Wire up Advanced Inventory events
        AdvancedInventoryViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalInventory();

        // Wire up Advanced Remove events
        AdvancedRemoveViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalRemove();

        // Set up initial tab content (normal mode)
        UpdateInventoryContent();
        UpdateRemoveContent();

        TransferContent = new Views.TransferTabView
        {
            DataContext = TransferItemViewModel
        };



        // Wire up events for inter-component communication (TODO: Implement events in ViewModels)
        // InventoryTabViewModel.InventoryItemSaved += OnInventoryItemSaved;
        // InventoryTabViewModel.PanelToggleRequested += OnPanelToggleRequested;
        // InventoryTabViewModel.AdvancedEntryRequested += (sender, e) => OnAdvancedEntryRequested();
        QuickButtonsViewModel.QuickActionExecuted += OnQuickActionExecuted;
        
        // Wire up RemoveTab events (TODO: Implement events in ViewModels)
        // RemoveItemViewModel.ItemsRemoved += OnItemsRemoved;
        RemoveItemViewModel.PanelToggleRequested += OnPanelToggleRequested;
        RemoveItemViewModel.AdvancedRemovalRequested += OnAdvancedRemovalRequested;

        // Wire up TransferTab events
        TransferItemViewModel.ItemsTransferred += OnItemsTransferred;
        TransferItemViewModel.PanelToggleRequested += OnPanelToggleRequested;

        // UI-only initialization; child contents are provided elsewhere via composition
        SelectedTabIndex = 0;
    }

    #region RelayCommand Methods

    /// <summary>
    /// Opens the settings view using the navigation service
    /// </summary>
    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        try
        {
            // Get SettingsViewModel from DI container
            var settingsViewModel = Program.GetService<SettingsViewModel>();
            
            // Create SettingsView with the ViewModel
            var settingsView = new Views.SettingsView
            {
                DataContext = settingsViewModel
            };
            
            // Navigate to the settings view
            await Task.Run(() => _navigationService.NavigateTo(settingsView)).ConfigureAwait(false);
            
            Logger.LogInformation("Navigated to Settings view");
            StatusText = "Settings opened";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open Settings view");
            StatusText = "Failed to open settings";
        }
    }

    /// <summary>
    /// Opens the advanced settings form
    /// </summary>
    [RelayCommand]
    private async Task OpenAdvancedSettingsAsync()
    {
        try
        {
            // Get SettingsViewModel from DI container
            var SettingsViewModel = Program.GetService<SettingsViewModel>();
            
            // Create SettingsView with the ViewModel
            var SettingsView = new Views.SettingsView
            {
                DataContext = SettingsViewModel
            };
            
            // Navigate to the advanced settings view
            await Task.Run(() => _navigationService.NavigateTo(SettingsView)).ConfigureAwait(false);
            
            Logger.LogInformation("Navigated to Advanced Settings form");
            StatusText = "Advanced Settings opened";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open Advanced Settings form");
            StatusText = "Failed to open advanced settings";
        }
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    [RelayCommand]
    private void Exit()
    {
        // TODO: Exit app
    }

    /// <summary>
    /// Opens the personal history view
    /// </summary>
    [RelayCommand]
    private void OpenPersonalHistory()
    {
        // TODO: Open history
    }

    /// <summary>
    /// Refreshes the current tab's data
    /// </summary>
    [RelayCommand]
    private void Refresh()
    {
        switch (SelectedTabIndex)
        {
            case 0: // Inventory Tab
                // TODO: Refresh inventory tab
                StatusText = "Refreshing inventory...";
                break;
            case 1: // Remove Tab
                if (IsAdvancedRemoveMode)
                {
                    if (AdvancedRemoveViewModel.LoadDataCommand.CanExecute(null))
                        AdvancedRemoveViewModel.LoadDataCommand.Execute(null);
                }
                else
                {
                    if (RemoveItemViewModel.SearchCommand.CanExecute(null))
                        RemoveItemViewModel.SearchCommand.Execute(null);
                }
                break;
            case 2: // Transfer Tab
                if (TransferItemViewModel.ExecuteSearchCommand.CanExecute(null))
                    TransferItemViewModel.ExecuteSearchCommand.Execute(null);
                break;
        }
    }

    /// <summary>
    /// Cancels the current operation
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        // TODO: Cancel current operation
    }

    /// <summary>
    /// Shows the about dialog
    /// </summary>
    [RelayCommand]
    private void OpenAbout()
    {
        // TODO: Show about dialog
    }

    /// <summary>
    /// Toggles the advanced panel visibility
    /// </summary>
    [RelayCommand]
    private void ToggleAdvancedPanel()
    {
        IsAdvancedPanelVisible = !IsAdvancedPanelVisible;
    }

    /// <summary>
    /// Toggles the quick actions panel expansion
    /// </summary>
    [RelayCommand]
    private void ToggleQuickActionsPanel()
    {
        IsQuickActionsPanelExpanded = !IsQuickActionsPanelExpanded;
    }

    /// <summary>
    /// Switches the inventory tab to advanced mode
    /// </summary>
    [RelayCommand]
    private void SwitchToAdvancedInventory()
    {
        IsAdvancedInventoryMode = true;
        UpdateInventoryContent();
        StatusText = "Advanced Inventory Mode";
        Logger.LogInformation("Switched to Advanced Inventory Mode");
    }

    /// <summary>
    /// Switches the inventory tab to normal mode
    /// </summary>
    [RelayCommand]
    private void SwitchToNormalInventory()
    {
        IsAdvancedInventoryMode = false;
        UpdateInventoryContent();
        StatusText = "Normal Inventory Mode";
        Logger.LogInformation("Switched to Normal Inventory Mode");
    }

    /// <summary>
    /// Switches the remove tab to advanced mode
    /// </summary>
    [RelayCommand]
    private void SwitchToAdvancedRemove()
    {
        IsAdvancedRemoveMode = true;
        UpdateRemoveContent();
        StatusText = "Advanced Remove Mode";
        Logger.LogInformation("Switched to Advanced Remove Mode");
    }

    /// <summary>
    /// Switches the remove tab to normal mode
    /// </summary>
    [RelayCommand]
    private void SwitchToNormalRemove()
    {
        IsAdvancedRemoveMode = false;
        UpdateRemoveContent();
        StatusText = "Normal Remove Mode";
        Logger.LogInformation("Switched to Normal Remove Mode");
    }

    #endregion

    private void HandleTabModeResets(int tabIndex)
    {
        // Reset to normal mode when switching away from tabs
        if (tabIndex != 0 && IsAdvancedInventoryMode)
        {
            // Reset to normal inventory mode when switching away from inventory tab
            IsAdvancedInventoryMode = false;
            UpdateInventoryContent();
        }
        if (tabIndex != 1 && IsAdvancedRemoveMode)
        {
            // Reset to normal remove mode when switching away from remove tab
            IsAdvancedRemoveMode = false;
            UpdateRemoveContent();
        }
    }

    private void OnInventoryItemSaved(object? sender, EventArgs e)
    {
        // TODO: Update QuickButtons with new inventory item
        // This could automatically add/update the most recent action in QuickButtons
        StatusText = "Item saved successfully";
    }

    private void OnPanelToggleRequested(object? sender, EventArgs e)
    {
        IsAdvancedPanelVisible = !IsAdvancedPanelVisible;
    }

    private void OnQuickActionExecuted(object? sender, QuickActionExecutedEventArgs e)
    {
        Logger.LogDebug("OnQuickActionExecuted event handler triggered - Sender: {SenderType}, PartId: {PartId}, Operation: {Operation}, Quantity: {Quantity}", 
            sender?.GetType().Name ?? "null", e.PartId, e.Operation, e.Quantity);
        
        try
        {
            // Populate appropriate tab fields with QuickButton data based on current tab
            Logger.LogInformation("Processing quick action for tab {TabIndex}: {Operation} - {PartId} ({Quantity} units)", 
                SelectedTabIndex, e.Operation, e.PartId, e.Quantity);
                
            switch (SelectedTabIndex)
            {
                case 0: // Inventory Tab
                    Logger.LogDebug("Populating Inventory tab with quick action data");
                    InventoryTabViewModel.SelectedPart = e.PartId;
                    InventoryTabViewModel.SelectedOperation = e.Operation;
                    InventoryTabViewModel.Quantity = e.Quantity;
                    break;
                case 1: // Remove Tab
                    Logger.LogDebug("Populating Remove tab with quick action data - Advanced mode: {IsAdvanced}", IsAdvancedRemoveMode);
                    if (IsAdvancedRemoveMode)
                    {
                        // Populate Advanced Remove text filters for wildcard search
                        AdvancedRemoveViewModel.FilterPartIDText = e.PartId;
                        AdvancedRemoveViewModel.FilterOperation = e.Operation;
                        // Note: Advanced Remove doesn't use quantity directly but could pre-filter
                    }
                    else
                    {
                        RemoveItemViewModel.SelectedPart = e.PartId;
                        RemoveItemViewModel.SelectedOperation = e.Operation;
                    }
                    break;
                case 2: // Transfer Tab
                    Logger.LogDebug("Populating Transfer tab with quick action data");
                    TransferItemViewModel.SelectedPart = e.PartId;
                    TransferItemViewModel.SelectedOperation = e.Operation;
                    TransferItemViewModel.TransferQuantity = e.Quantity;
                    break;
                default:
                    Logger.LogWarning("Unknown tab index {TabIndex} in quick action handler", SelectedTabIndex);
                    break;
            }
            
            StatusText = $"Quick action: {e.Operation} - {e.PartId} ({e.Quantity} units)";
            Logger.LogInformation("Quick action processed successfully and status updated");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing quick action: {Operation} - {PartId} ({Quantity} units)", e.Operation, e.PartId, e.Quantity);
            StatusText = "Error processing quick action";
        }
    }

    private void OnItemsRemoved(object? sender, ItemsRemovedEventArgs e)
    {
        Logger.LogDebug("OnItemsRemoved event handler triggered - Sender: {SenderType}, Items count: {ItemCount}", 
            sender?.GetType().Name ?? "null", e.RemovedItems.Count);
        
        try
        {
            // Update status with removal information
            var totalQuantity = e.TotalQuantityRemoved;
            var itemCount = e.RemovedItems.Count;
            StatusText = $"Removed: {itemCount} item(s), {totalQuantity} total quantity";
            
            Logger.LogInformation("Items removed successfully: {ItemCount} items, {TotalQuantity} total quantity", 
                itemCount, totalQuantity);
                
            // Log details of removed items for debugging
            foreach (var item in e.RemovedItems)
            {
                Logger.LogDebug("Removed item: {PartId} - {Operation} ({Quantity} units) from {Location}", 
                    item.PartId, item.Operation, item.Quantity, item.Location);
            }
            
            // TODO: Update QuickButtons or other components as needed
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnItemsRemoved event handler");
            StatusText = "Error processing item removal";
        }
    }

    private void OnItemsTransferred(object? sender, ItemsTransferredEventArgs e)
    {
        Logger.LogDebug("OnItemsTransferred event handler triggered - Sender: {SenderType}, PartId: {PartId}", 
            sender?.GetType().Name ?? "null", e.PartId);
        
        try
        {
            // Update status with transfer information
            StatusText = $"Transferred: {e.TransferredQuantity} units of {e.PartId} from {e.FromLocation} to {e.ToLocation}";
            
            Logger.LogInformation("Items transferred successfully: {Quantity} units of {PartId} from {FromLocation} to {ToLocation}", 
                e.TransferredQuantity, e.PartId, e.FromLocation, e.ToLocation);
            
            // TODO: Update QuickButtons with transfer information for future quick actions
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnItemsTransferred event handler");
            StatusText = "Error processing item transfer";
        }
    }

    private void OnAdvancedRemovalRequested(object? sender, EventArgs e)
    {
        // Switch to Advanced Remove Mode
        SwitchToAdvancedRemoveCommand.Execute(null);
        Logger.LogInformation("Advanced removal features requested - switching to Advanced Remove Mode");
    }

    /// <summary>
    /// Handles property changes from ApplicationStateService to update MainView progress
    /// This enables centralized progress communication from child ViewModels
    /// </summary>
    private void OnApplicationStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IApplicationStateService.ProgressValue))
        {
            ProgressValue = _applicationState.ProgressValue;
        }
        else if (e.PropertyName == nameof(IApplicationStateService.StatusText))
        {
            StatusText = _applicationState.StatusText;
        }
    }

    private void OnTabSelectionChanged(int tabIndex)
    {
        Logger.LogDebug("OnTabSelectionChanged triggered - Previous tab: {PreviousTab}, New tab: {NewTab}", 
            SelectedTabIndex, tabIndex);
        
        try
        {
            switch (tabIndex)
            {
                case 0: // Inventory Tab
                    StatusText = IsAdvancedInventoryMode ? "Advanced Inventory Entry" : "Inventory Entry";
                    Logger.LogInformation("Switched to Inventory tab - Mode: {Mode}", 
                        IsAdvancedInventoryMode ? "Advanced" : "Normal");
                    break;
                case 1: // Remove Tab
                    StatusText = IsAdvancedRemoveMode ? "Advanced Inventory Removal" : "Inventory Removal";
                    Logger.LogInformation("Switched to Remove tab - Mode: {Mode}", 
                        IsAdvancedRemoveMode ? "Advanced" : "Normal");
                    
                    // Load data when switching to Remove tab
                    if (IsAdvancedRemoveMode)
                    {
                        Logger.LogDebug("Loading data for Advanced Remove mode");
                        if (AdvancedRemoveViewModel.LoadDataCommand.CanExecute(null))
                        {
                            AdvancedRemoveViewModel.LoadDataCommand.Execute(null);
                            Logger.LogDebug("Advanced Remove LoadDataCommand executed");
                        }
                        else
                        {
                            Logger.LogWarning("Advanced Remove LoadDataCommand cannot execute");
                        }
                    }
                    else
                    {
                        Logger.LogDebug("Loading data for Normal Remove mode");
                        if (RemoveItemViewModel.LoadDataCommand.CanExecute(null))
                        {
                            RemoveItemViewModel.LoadDataCommand.Execute(null);
                            Logger.LogDebug("Normal Remove LoadDataCommand executed");
                        }
                        else
                        {
                            Logger.LogWarning("Normal Remove LoadDataCommand cannot execute");
                        }
                    }
                    break;
                case 2: // Transfer Tab
                    StatusText = "Inventory Transfer";
                    Logger.LogInformation("Switched to Transfer tab");
                    
                    // Load data when switching to Transfer tab
                    Logger.LogDebug("Loading data for Transfer tab");
                    // Transfer ViewModel will load data when needed
                    Logger.LogDebug("Transfer tab activated - data will load on demand");
                    break;
                default:
                    StatusText = "Ready";
                    Logger.LogWarning("Unknown tab index selected: {TabIndex}", tabIndex);
                    break;
            }
            
            Logger.LogInformation("Tab selection change completed successfully for index: {TabIndex}", tabIndex);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnTabSelectionChanged for tab: {TabIndex}", tabIndex);
            StatusText = "Error switching tabs";
        }
    }

    /// <summary>
    /// Updates the inventory tab content based on current mode (normal vs advanced)
    /// </summary>
    private void UpdateInventoryContent()
    {
        if (IsAdvancedInventoryMode)
        {
            // Switch to Advanced Inventory View
            InventoryContent = new Views.AdvancedInventoryView
            {
                DataContext = AdvancedInventoryViewModel
            };
        }
        else
        {
            // Switch to Normal Inventory View - pass service provider for proper DI
            InventoryContent = new Views.InventoryTabView(_serviceProvider)
            {
                DataContext = InventoryTabViewModel
            };
        }
    }

    /// <summary>
    /// Updates the remove tab content based on current mode (normal vs advanced)
    /// </summary>
    private void UpdateRemoveContent()
    {
        if (IsAdvancedRemoveMode)
        {
            // Switch to Advanced Remove View
            RemoveContent = new Views.AdvancedRemoveView
            {
                DataContext = AdvancedRemoveViewModel
            };
        }
        else
        {
            // Switch to Normal Remove View
            RemoveContent = new Views.RemoveTabView
            {
                DataContext = RemoveItemViewModel
            };
        }
    }

    /// <summary>
    /// Handles the Advanced Entry command from the normal inventory tab
    /// </summary>
    public void OnAdvancedEntryRequested()
    {
        SwitchToAdvancedInventoryCommand.Execute(null);
    }

    #region Resource Management

    /// <summary>
    /// Releases resources used by the MainViewViewModel
    /// </summary>
    /// <param name="disposing">True if disposing, false if finalizing</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                // Unsubscribe from events to prevent memory leaks
                if (_applicationState != null)
                {
                    _applicationState.PropertyChanged -= OnApplicationStateChanged;
                }

                if (AdvancedInventoryViewModel != null)
                {
                    AdvancedInventoryViewModel.BackToNormalRequested -= (sender, e) => SwitchToNormalInventory();
                }

                if (AdvancedRemoveViewModel != null)
                {
                    AdvancedRemoveViewModel.BackToNormalRequested -= (sender, e) => SwitchToNormalRemove();
                }

                if (QuickButtonsViewModel != null)
                {
                    QuickButtonsViewModel.QuickActionExecuted -= OnQuickActionExecuted;
                }

                if (RemoveItemViewModel != null)
                {
                    RemoveItemViewModel.PanelToggleRequested -= OnPanelToggleRequested;
                    RemoveItemViewModel.AdvancedRemovalRequested -= OnAdvancedRemovalRequested;
                }

                if (TransferItemViewModel != null)
                {
                    TransferItemViewModel.ItemsTransferred -= OnItemsTransferred;
                    TransferItemViewModel.PanelToggleRequested -= OnPanelToggleRequested;
                }

                Logger.LogInformation("MainViewViewModel resources disposed successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during MainViewViewModel disposal");
            }
        }

        // Call base class disposal
        base.Dispose(disposing);
    }

    #endregion

}