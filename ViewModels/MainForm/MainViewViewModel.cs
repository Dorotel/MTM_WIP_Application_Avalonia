using System;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainViewViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly IServiceProvider _serviceProvider;

    // Tabs
    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set 
        {
            if (SetProperty(ref _selectedTabIndex, value))
            {
                OnTabSelectionChanged(value);
                HandleTabModeResets(value);
            }
        }
    }

    private object? _inventoryContent;
    public object? InventoryContent
    {
        get => _inventoryContent;
        set => SetProperty(ref _inventoryContent, value);
    }

    private object? _removeContent;
    public object? RemoveContent
    {
        get => _removeContent;
        set => SetProperty(ref _removeContent, value);
    }

    private object? _transferContent;
    public object? TransferContent
    {
        get => _transferContent;
        set => SetProperty(ref _transferContent, value);
    }

    private object? _testControlsContent;
    public object? TestControlsContent
    {
        get => _testControlsContent;
        set => SetProperty(ref _testControlsContent, value);
    }

    // Quick Actions panel (renamed from Advanced panel)
    private bool _isAdvancedPanelVisible = true;
    public bool IsAdvancedPanelVisible
    {
        get => _isAdvancedPanelVisible;
        set 
        {
            if (SetProperty(ref _isAdvancedPanelVisible, value))
            {
                OnPropertyChanged(nameof(AdvancedPanelToggleText));
            }
        }
    }

    public string AdvancedPanelToggleText => IsAdvancedPanelVisible ? "Hide" : "Show";

    // Collapsible QuickActions Panel Properties
    private bool _isQuickActionsPanelExpanded = true;
    public bool IsQuickActionsPanelExpanded
    {
        get => _isQuickActionsPanelExpanded;
        set => SetProperty(ref _isQuickActionsPanelExpanded, value);
    }

    // Inventory Mode Management
    private bool _isAdvancedInventoryMode;
    public bool IsAdvancedInventoryMode
    {
        get => _isAdvancedInventoryMode;
        set => SetProperty(ref _isAdvancedInventoryMode, value);
    }

    // Remove Mode Management
    private bool _isAdvancedRemoveMode;
    public bool IsAdvancedRemoveMode
    {
        get => _isAdvancedRemoveMode;
        set => SetProperty(ref _isAdvancedRemoveMode, value);
    }

    // Child ViewModels
    public QuickButtonsViewModel QuickButtonsViewModel { get; }
    public InventoryTabViewModel InventoryTabViewModel { get; }
    public AdvancedInventoryViewModel AdvancedInventoryViewModel { get; }
    public RemoveItemViewModel RemoveItemViewModel { get; }
    public AdvancedRemoveViewModel AdvancedRemoveViewModel { get; }
    public TransferItemViewModel TransferItemViewModel { get; }

    // SuggestionOverlayViewModel removed for overlay ViewModel reset

    // Status strip
    private string _connectionStatus = "Disconnected";
    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    private int _connectionStrength = 0; // 0..100
    public int ConnectionStrength
    {
        get => _connectionStrength;
        set => SetProperty(ref _connectionStrength, value);
    }

    private int _progressValue = 0; // 0..100
    public int ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    // Dev menu visibility
    private bool _showDevelopmentMenu = true; // TODO: Bind to build config/environment
    public bool ShowDevelopmentMenu
    {
        get => _showDevelopmentMenu;
        set => SetProperty(ref _showDevelopmentMenu, value);
    }

    // Commands
    public ICommand OpenSettingsCommand { get; private set; }
    public ICommand OpenAdvancedSettingsCommand { get; private set; }
    public ICommand ExitCommand { get; private set; }
    public ICommand OpenPersonalHistoryCommand { get; private set; }
    public ICommand RefreshCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand ToggleAdvancedPanelCommand { get; private set; }
    public ICommand ToggleQuickActionsPanelCommand { get; private set; }
    public ICommand OpenAboutCommand { get; private set; }
    public ICommand SwitchToAdvancedInventoryCommand { get; private set; }
    public ICommand SwitchToNormalInventoryCommand { get; private set; }
    public ICommand SwitchToAdvancedRemoveCommand { get; private set; }
    public ICommand SwitchToNormalRemoveCommand { get; private set; }

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

        // Inject child ViewModels instead of creating them
        QuickButtonsViewModel = quickButtonsViewModel ?? throw new ArgumentNullException(nameof(quickButtonsViewModel));
        InventoryTabViewModel = inventoryTabViewModel ?? throw new ArgumentNullException(nameof(inventoryTabViewModel));
        RemoveItemViewModel = removeItemViewModel ?? throw new ArgumentNullException(nameof(removeItemViewModel));
        TransferItemViewModel = transferItemViewModel ?? throw new ArgumentNullException(nameof(transferItemViewModel));
        AdvancedInventoryViewModel = advancedInventoryViewModel ?? throw new ArgumentNullException(nameof(advancedInventoryViewModel));
        AdvancedRemoveViewModel = advancedRemoveViewModel ?? throw new ArgumentNullException(nameof(advancedRemoveViewModel));
    // SuggestionOverlayViewModel removed for overlay ViewModel reset

        // Wire up Advanced Inventory events
        AdvancedInventoryViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalInventoryCommand?.Execute(null);

        // Wire up Advanced Remove events
        AdvancedRemoveViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalRemoveCommand?.Execute(null);

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

        // Initialize commands (stubs; no business logic)
        OpenSettingsCommand = new Commands.RelayCommand(OpenSettings);
        OpenAdvancedSettingsCommand = new Commands.RelayCommand(OpenAdvancedSettings);
        ExitCommand = new Commands.RelayCommand(() => { /* TODO: Exit app */ });
        OpenPersonalHistoryCommand = new Commands.RelayCommand(() => { /* TODO: Open history */ });
        RefreshCommand = new Commands.RelayCommand(() => 
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
                    if (TransferItemViewModel.SearchCommand.CanExecute(null))
                        TransferItemViewModel.SearchCommand.Execute(null);
                    break;
            }
        });
        CancelCommand = new Commands.RelayCommand(() => { /* TODO: Cancel current operation */ });
        OpenAboutCommand = new Commands.RelayCommand(() => { /* TODO: Show about dialog */ });
        
        ToggleAdvancedPanelCommand = new Commands.RelayCommand(() =>
        {
            IsAdvancedPanelVisible = !IsAdvancedPanelVisible;
        });

        ToggleQuickActionsPanelCommand = new Commands.RelayCommand(() =>
        {
            IsQuickActionsPanelExpanded = !IsQuickActionsPanelExpanded;
        });

        SwitchToAdvancedInventoryCommand = new Commands.RelayCommand(() =>
        {
            IsAdvancedInventoryMode = true;
            UpdateInventoryContent();
            StatusText = "Advanced Inventory Mode";
            Logger.LogInformation("Switched to Advanced Inventory Mode");
        });

        SwitchToNormalInventoryCommand = new Commands.RelayCommand(() =>
        {
            IsAdvancedInventoryMode = false;
            UpdateInventoryContent();
            StatusText = "Normal Inventory Mode";
            Logger.LogInformation("Switched to Normal Inventory Mode");
        });

        SwitchToAdvancedRemoveCommand = new Commands.RelayCommand(() =>
        {
            IsAdvancedRemoveMode = true;
            UpdateRemoveContent();
            StatusText = "Advanced Remove Mode";
            Logger.LogInformation("Switched to Advanced Remove Mode");
        });

        SwitchToNormalRemoveCommand = new Commands.RelayCommand(() =>
        {
            IsAdvancedRemoveMode = false;
            UpdateRemoveContent();
            StatusText = "Normal Remove Mode";
            Logger.LogInformation("Switched to Normal Remove Mode");
        });

        // Property change handling is now done in property setters
        // Command error handling is managed within individual commands using standard .NET patterns
    }

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
        if (SwitchToAdvancedRemoveCommand.CanExecute(null))
            SwitchToAdvancedRemoveCommand.Execute(null);
        Logger.LogInformation("Advanced removal features requested - switching to Advanced Remove Mode");
    }

    private void OnTabSelectionChanged(int tabIndex)
    {
        Logger.LogDebug("OnTabSelectionChanged triggered - Previous tab: {PreviousTab}, New tab: {NewTab}", 
            _selectedTabIndex, tabIndex);
        
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
                    if (TransferItemViewModel.LoadDataCommand.CanExecute(null))
                    {
                        TransferItemViewModel.LoadDataCommand.Execute(null);
                        Logger.LogDebug("Transfer LoadDataCommand executed");
                    }
                    else
                    {
                        Logger.LogWarning("Transfer LoadDataCommand cannot execute");
                    }
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
        if (SwitchToAdvancedInventoryCommand.CanExecute(null))
            SwitchToAdvancedInventoryCommand.Execute(null);
    }

    /// <summary>
    /// Opens the settings view using the navigation service
    /// </summary>
    private void OpenSettings()
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
            _navigationService.NavigateTo(settingsView);
            
            Logger.LogInformation("Navigated to Settings view");
            StatusText = "Settings opened";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open Settings view");
            StatusText = "Failed to open settings";
        }
    }

    private void OpenAdvancedSettings()
    {
        try
        {
            // Get SettingsFormViewModel from DI container
            var settingsFormViewModel = Program.GetService<SettingsFormViewModel>();
            
            // Create SettingsFormView with the ViewModel
            var settingsFormView = new Views.SettingsFormView
            {
                DataContext = settingsFormViewModel
            };
            
            // Navigate to the advanced settings view
            _navigationService.NavigateTo(settingsFormView);
            
            Logger.LogInformation("Navigated to Advanced Settings form");
            StatusText = "Advanced Settings opened";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to open Advanced Settings form");
            StatusText = "Failed to open advanced settings";
        }
    }
}