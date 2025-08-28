using System;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainViewViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;

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
        set 
        {
            if (SetProperty(ref _isQuickActionsPanelExpanded, value))
            {
                OnPropertyChanged(nameof(QuickActionsPanelWidth));
                OnPropertyChanged(nameof(QuickActionsCollapseButtonIcon));
            }
        }
    }

    // QuickActions Panel Width - expands/collapses based on state (returns GridLength compatible values)
    public GridLength QuickActionsPanelWidth => IsQuickActionsPanelExpanded ? new GridLength(240) : new GridLength(32);

    // QuickActions Collapse Button Icon
    public string QuickActionsCollapseButtonIcon => IsQuickActionsPanelExpanded ? "▶" : "◀";

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
        ILogger<MainViewViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("MainViewViewModel initialized with dependency injection");

        // Inject child ViewModels instead of creating them
        QuickButtonsViewModel = quickButtonsViewModel ?? throw new ArgumentNullException(nameof(quickButtonsViewModel));
        InventoryTabViewModel = inventoryTabViewModel ?? throw new ArgumentNullException(nameof(inventoryTabViewModel));
        RemoveItemViewModel = removeItemViewModel ?? throw new ArgumentNullException(nameof(removeItemViewModel));
        TransferItemViewModel = transferItemViewModel ?? throw new ArgumentNullException(nameof(transferItemViewModel));
        AdvancedInventoryViewModel = advancedInventoryViewModel ?? throw new ArgumentNullException(nameof(advancedInventoryViewModel));
        AdvancedRemoveViewModel = advancedRemoveViewModel ?? throw new ArgumentNullException(nameof(advancedRemoveViewModel));

        // Wire up Advanced Inventory events
        AdvancedInventoryViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalInventoryCommand.Execute(null);

        // Wire up Advanced Remove events
        AdvancedRemoveViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalRemoveCommand.Execute(null);

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
        OpenSettingsCommand = new Commands.RelayCommand(() => { /* TODO: Show settings */ });
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
                        _ = AdvancedRemoveViewModel.LoadDataCommand.Execute();
                    else
                        _ = RemoveItemViewModel.SearchCommand.Execute();
                    break;
                case 2: // Transfer Tab
                    _ = TransferItemViewModel.SearchCommand.Execute();
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
            Logger.LogInformation("QuickActions panel toggled: {IsExpanded}", IsQuickActionsPanelExpanded);
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
        // Populate appropriate tab fields with QuickButton data based on current tab
        switch (SelectedTabIndex)
        {
            case 0: // Inventory Tab
                InventoryTabViewModel.SelectedPart = e.PartId;
                InventoryTabViewModel.SelectedOperation = e.Operation;
                InventoryTabViewModel.Quantity = e.Quantity.ToString();
                break;
            case 1: // Remove Tab
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
                TransferItemViewModel.SelectedPart = e.PartId;
                TransferItemViewModel.SelectedOperation = e.Operation;
                TransferItemViewModel.TransferQuantity = e.Quantity;
                break;
        }
        
        StatusText = $"Quick action: {e.Operation} - {e.PartId} ({e.Quantity} units)";
    }

    private void OnItemsRemoved(object? sender, ItemsRemovedEventArgs e)
    {
        // Update status with removal information
        var totalQuantity = e.TotalQuantityRemoved;
        var itemCount = e.RemovedItems.Count;
        StatusText = $"Removed: {itemCount} item(s), {totalQuantity} total quantity";
        
        // TODO: Update QuickButtons or other components as needed
        Logger.LogInformation("Items removed: {ItemCount} items, {TotalQuantity} total quantity", 
            itemCount, totalQuantity);
    }

    private void OnItemsTransferred(object? sender, ItemsTransferredEventArgs e)
    {
        // Update status with transfer information
        StatusText = $"Transferred: {e.TransferredQuantity} units of {e.PartId} from {e.FromLocation} to {e.ToLocation}";
        
        // TODO: Update QuickButtons with transfer information for future quick actions
        Logger.LogInformation("Items transferred: {Quantity} units of {PartId} from {FromLocation} to {ToLocation}", 
            e.TransferredQuantity, e.PartId, e.FromLocation, e.ToLocation);
    }

    private void OnAdvancedRemovalRequested(object? sender, EventArgs e)
    {
        // Switch to Advanced Remove Mode
        SwitchToAdvancedRemoveCommand.Execute().Subscribe();
        Logger.LogInformation("Advanced removal features requested - switching to Advanced Remove Mode");
    }

    private void OnTabSelectionChanged(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0: // Inventory Tab
                StatusText = IsAdvancedInventoryMode ? "Advanced Inventory Entry" : "Inventory Entry";
                break;
            case 1: // Remove Tab
                StatusText = IsAdvancedRemoveMode ? "Advanced Inventory Removal" : "Inventory Removal";
                // Load data when switching to Remove tab
                if (IsAdvancedRemoveMode)
                    _ = AdvancedRemoveViewModel.LoadDataCommand.Execute();
                else
                    _ = RemoveItemViewModel.LoadDataCommand.Execute();
                break;
            case 2: // Transfer Tab
                StatusText = "Inventory Transfer";
                // Load data when switching to Transfer tab
                _ = TransferItemViewModel.LoadDataCommand.Execute();
                break;
            default:
                StatusText = "Ready";
                break;
        }
        
        Logger.LogInformation("Tab changed to index: {TabIndex}", tabIndex);
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
            // Switch to Normal Inventory View
            InventoryContent = new Views.InventoryTabView
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
        SwitchToAdvancedInventoryCommand.Execute().Subscribe();
    }
}
