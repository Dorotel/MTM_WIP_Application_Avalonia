using System;
using System.Reactive;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainViewViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly MTM_Shared_Logic.Services.IInventoryService _inventoryService;

    // Tabs
    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    private object? _inventoryContent;
    public object? InventoryContent
    {
        get => _inventoryContent;
        set => this.RaiseAndSetIfChanged(ref _inventoryContent, value);
    }

    private object? _removeContent;
    public object? RemoveContent
    {
        get => _removeContent;
        set => this.RaiseAndSetIfChanged(ref _removeContent, value);
    }

    private object? _transferContent;
    public object? TransferContent
    {
        get => _transferContent;
        set => this.RaiseAndSetIfChanged(ref _transferContent, value);
    }

    // Quick Actions panel (renamed from Advanced panel)
    private bool _isAdvancedPanelVisible = true;
    public bool IsAdvancedPanelVisible
    {
        get => _isAdvancedPanelVisible;
        set => this.RaiseAndSetIfChanged(ref _isAdvancedPanelVisible, value);
    }

    public string AdvancedPanelToggleText => IsAdvancedPanelVisible ? "Hide" : "Show";

    // Collapsible QuickActions Panel Properties
    private bool _isQuickActionsPanelExpanded = true;
    public bool IsQuickActionsPanelExpanded
    {
        get => _isQuickActionsPanelExpanded;
        set => this.RaiseAndSetIfChanged(ref _isQuickActionsPanelExpanded, value);
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
        set => this.RaiseAndSetIfChanged(ref _isAdvancedInventoryMode, value);
    }

    // Remove Mode Management
    private bool _isAdvancedRemoveMode;
    public bool IsAdvancedRemoveMode
    {
        get => _isAdvancedRemoveMode;
        set => this.RaiseAndSetIfChanged(ref _isAdvancedRemoveMode, value);
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
        set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
    }

    private int _connectionStrength = 0; // 0..100
    public int ConnectionStrength
    {
        get => _connectionStrength;
        set => this.RaiseAndSetIfChanged(ref _connectionStrength, value);
    }

    private int _progressValue = 0; // 0..100
    public int ProgressValue
    {
        get => _progressValue;
        set => this.RaiseAndSetIfChanged(ref _progressValue, value);
    }

    private string _statusText = "Ready";
    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }

    // Dev menu visibility
    private bool _showDevelopmentMenu = true; // TODO: Bind to build config/environment
    public bool ShowDevelopmentMenu
    {
        get => _showDevelopmentMenu;
        set => this.RaiseAndSetIfChanged(ref _showDevelopmentMenu, value);
    }

    // Commands
    public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenPersonalHistoryCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleAdvancedPanelCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleQuickActionsPanelCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAboutCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToAdvancedInventoryCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToNormalInventoryCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToAdvancedRemoveCommand { get; }
    public ReactiveCommand<Unit, Unit> SwitchToNormalRemoveCommand { get; }

    public MainViewViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        MTM_Shared_Logic.Services.IInventoryService inventoryService,
        ILogger<MainViewViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));

        Logger.LogInformation("MainViewViewModel initialized with dependency injection");

        // Initialize child ViewModels - TODO: These should also use DI
        QuickButtonsViewModel = new QuickButtonsViewModel();
        InventoryTabViewModel = new InventoryTabViewModel();
        
        // Create loggers for child ViewModels
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var removeItemLogger = loggerFactory.CreateLogger<RemoveItemViewModel>();
        var transferItemLogger = loggerFactory.CreateLogger<TransferItemViewModel>();
        var advancedInventoryLogger = loggerFactory.CreateLogger<AdvancedInventoryViewModel>();
        var advancedRemoveLogger = loggerFactory.CreateLogger<AdvancedRemoveViewModel>();
        
        RemoveItemViewModel = new RemoveItemViewModel(inventoryService, applicationState, removeItemLogger);
        TransferItemViewModel = new TransferItemViewModel(inventoryService, applicationState, transferItemLogger);
        AdvancedInventoryViewModel = new AdvancedInventoryViewModel(advancedInventoryLogger);
        AdvancedRemoveViewModel = new AdvancedRemoveViewModel(advancedRemoveLogger);

        // Wire up Advanced Inventory events
        AdvancedInventoryViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalInventoryCommand.Execute().Subscribe();

        // Wire up Advanced Remove events
        AdvancedRemoveViewModel.BackToNormalRequested += (sender, e) => SwitchToNormalRemoveCommand.Execute().Subscribe();

        // Set up initial tab content (normal mode)
        UpdateInventoryContent();
        UpdateRemoveContent();

        TransferContent = new Views.TransferTabView
        {
            DataContext = TransferItemViewModel
        };

        // Wire up events for inter-component communication
        InventoryTabViewModel.InventoryItemSaved += OnInventoryItemSaved;
        InventoryTabViewModel.PanelToggleRequested += OnPanelToggleRequested;
        InventoryTabViewModel.AdvancedEntryRequested += (sender, e) => OnAdvancedEntryRequested();
        QuickButtonsViewModel.QuickActionExecuted += OnQuickActionExecuted;
        
        // Wire up RemoveTab events
        RemoveItemViewModel.ItemsRemoved += OnItemsRemoved;
        RemoveItemViewModel.PanelToggleRequested += OnPanelToggleRequested;
        RemoveItemViewModel.AdvancedRemovalRequested += OnAdvancedRemovalRequested;

        // Wire up TransferTab events
        TransferItemViewModel.ItemsTransferred += OnItemsTransferred;
        TransferItemViewModel.PanelToggleRequested += OnPanelToggleRequested;

        // UI-only initialization; child contents are provided elsewhere via composition
        SelectedTabIndex = 0;

        // Initialize commands (stubs; no business logic)
        OpenSettingsCommand = ReactiveCommand.Create(() => { /* TODO: Show settings */ });
        ExitCommand = ReactiveCommand.Create(() => { /* TODO: Exit app */ });
        OpenPersonalHistoryCommand = ReactiveCommand.Create(() => { /* TODO: Open history */ });
        RefreshCommand = ReactiveCommand.Create(() => 
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
        CancelCommand = ReactiveCommand.Create(() => { /* TODO: Cancel current operation */ });
        OpenAboutCommand = ReactiveCommand.Create(() => { /* TODO: Show about dialog */ });
        
        ToggleAdvancedPanelCommand = ReactiveCommand.Create(() =>
        {
            IsAdvancedPanelVisible = !IsAdvancedPanelVisible;
        });

        ToggleQuickActionsPanelCommand = ReactiveCommand.Create(() =>
        {
            IsQuickActionsPanelExpanded = !IsQuickActionsPanelExpanded;
            Logger.LogInformation("QuickActions panel toggled: {IsExpanded}", IsQuickActionsPanelExpanded);
        });

        SwitchToAdvancedInventoryCommand = ReactiveCommand.Create(() =>
        {
            IsAdvancedInventoryMode = true;
            UpdateInventoryContent();
            StatusText = "Advanced Inventory Mode";
            Logger.LogInformation("Switched to Advanced Inventory Mode");
        });

        SwitchToNormalInventoryCommand = ReactiveCommand.Create(() =>
        {
            IsAdvancedInventoryMode = false;
            UpdateInventoryContent();
            StatusText = "Normal Inventory Mode";
            Logger.LogInformation("Switched to Normal Inventory Mode");
        });

        SwitchToAdvancedRemoveCommand = ReactiveCommand.Create(() =>
        {
            IsAdvancedRemoveMode = true;
            UpdateRemoveContent();
            StatusText = "Advanced Remove Mode";
            Logger.LogInformation("Switched to Advanced Remove Mode");
        });

        SwitchToNormalRemoveCommand = ReactiveCommand.Create(() =>
        {
            IsAdvancedRemoveMode = false;
            UpdateRemoveContent();
            StatusText = "Normal Remove Mode";
            Logger.LogInformation("Switched to Normal Remove Mode");
        });

        // Subscribe to property changes to update derived properties
        this.WhenAnyValue(x => x.IsAdvancedPanelVisible)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(AdvancedPanelToggleText)));

        // Subscribe to QuickActions panel state changes to update derived properties
        this.WhenAnyValue(x => x.IsQuickActionsPanelExpanded)
            .Subscribe(_ => 
            {
                this.RaisePropertyChanged(nameof(QuickActionsPanelWidth));
                this.RaisePropertyChanged(nameof(QuickActionsCollapseButtonIcon));
            });

        // Subscribe to tab changes to trigger appropriate actions
        this.WhenAnyValue(x => x.SelectedTabIndex)
            .Subscribe(OnTabSelectionChanged);

        // Subscribe to inventory mode changes to reset to normal mode when switching tabs
        this.WhenAnyValue(x => x.SelectedTabIndex)
            .Subscribe(tabIndex =>
            {
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
            });

        // Error handling for all commands
        var allCommands = new[]
        {
            OpenSettingsCommand, ExitCommand, OpenPersonalHistoryCommand,
            RefreshCommand, CancelCommand, ToggleAdvancedPanelCommand, ToggleQuickActionsPanelCommand, OpenAboutCommand,
            SwitchToAdvancedInventoryCommand, SwitchToNormalInventoryCommand,
            SwitchToAdvancedRemoveCommand, SwitchToNormalRemoveCommand
        };

        foreach (var command in allCommands)
        {
            command.ThrownExceptions.Subscribe(ex =>
            {
                Logger.LogError(ex, "Error executing command in MainViewViewModel");
                // TODO: Log and present user-friendly error
            });
        }
    }

    private void OnInventoryItemSaved(object? sender, InventoryItemSavedEventArgs e)
    {
        // TODO: Update QuickButtons with new inventory item
        // This could automatically add/update the most recent action in QuickButtons
        StatusText = $"Saved: {e.PartId} - {e.Operation} ({e.Quantity} units)";
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
