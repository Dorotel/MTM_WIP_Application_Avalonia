using System;
using System.Reactive;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainViewViewModel : ReactiveObject
{
    // TODO: Inject services
    // private readonly IConnectionRecoveryManager _connectionRecoveryManager;
    // private readonly IProgressHelper _progressHelper;
    // private readonly IConnectionStrengthChecker _connectionStrengthChecker;

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

    // Child ViewModels
    public QuickButtonsViewModel QuickButtonsViewModel { get; }
    public InventoryTabViewModel InventoryTabViewModel { get; }

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
    public ReactiveCommand<Unit, Unit> OpenAboutCommand { get; }

    public MainViewViewModel()
    {
        // Initialize child ViewModels
        QuickButtonsViewModel = new QuickButtonsViewModel();
        InventoryTabViewModel = new InventoryTabViewModel();

        // Set up InventoryTab content
        InventoryContent = new Views.InventoryTabView
        {
            DataContext = InventoryTabViewModel
        };

        // Wire up events for inter-component communication
        InventoryTabViewModel.InventoryItemSaved += OnInventoryItemSaved;
        InventoryTabViewModel.PanelToggleRequested += OnPanelToggleRequested;
        QuickButtonsViewModel.QuickActionExecuted += OnQuickActionExecuted;

        // UI-only initialization; child contents are provided elsewhere via composition
        SelectedTabIndex = 0;

        // Initialize commands (stubs; no business logic)
        OpenSettingsCommand = ReactiveCommand.Create(() => { /* TODO: Show settings */ });
        ExitCommand = ReactiveCommand.Create(() => { /* TODO: Exit app */ });
        OpenPersonalHistoryCommand = ReactiveCommand.Create(() => { /* TODO: Open history */ });
        RefreshCommand = ReactiveCommand.Create(() => { /* TODO: Refresh current tab */ });
        CancelCommand = ReactiveCommand.Create(() => { /* TODO: Cancel current operation */ });
        OpenAboutCommand = ReactiveCommand.Create(() => { /* TODO: Show about dialog */ });
        
        ToggleAdvancedPanelCommand = ReactiveCommand.Create(() =>
        {
            IsAdvancedPanelVisible = !IsAdvancedPanelVisible;
        });

        // Subscribe to property changes to update derived properties
        this.WhenAnyValue(x => x.IsAdvancedPanelVisible)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(AdvancedPanelToggleText)));

        // Error handling for all commands
        var allCommands = new[]
        {
            OpenSettingsCommand, ExitCommand, OpenPersonalHistoryCommand,
            RefreshCommand, CancelCommand, ToggleAdvancedPanelCommand, OpenAboutCommand
        };

        foreach (var command in allCommands)
        {
            command.ThrownExceptions.Subscribe(ex =>
            {
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
        // Populate InventoryTab fields with QuickButton data
        InventoryTabViewModel.SelectedPart = e.PartId;
        InventoryTabViewModel.SelectedOperation = e.Operation;
        InventoryTabViewModel.Quantity = e.Quantity.ToString();
        
        // Switch to Inventory tab if not already selected
        SelectedTabIndex = 0;
        
        StatusText = $"Quick action: {e.Operation} - {e.PartId} ({e.Quantity} units)";
    }
}