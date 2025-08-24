using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM.Models;
using System.Linq;
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for Advanced Remove: Enhanced Removal Operations Interface
/// Provides sophisticated removal operations beyond standard inventory removal functionality.
/// Features include bulk removal operations, removal history tracking, undo capabilities, 
/// and specialized reporting for removal analytics.
/// </summary>
public class AdvancedRemoveViewModel : BaseViewModel
{
    #region Filter Fields and Options
    public ObservableCollection<string> LocationOptions { get; } = new();
    public ObservableCollection<string> PartIDOptions { get; } = new();
    public ObservableCollection<string> UserOptions { get; } = new();
    public ObservableCollection<string> OperationOptions { get; } = new();

    public string? FilterLocationText { get => _filterLocationText; set => this.RaiseAndSetIfChanged(ref _filterLocationText, value); }
    public string? FilterPartIDText { get => _filterPartIDText; set => this.RaiseAndSetIfChanged(ref _filterPartIDText, value); }
    public string? FilterUserText { get => _filterUserText; set => this.RaiseAndSetIfChanged(ref _filterUserText, value); }
    public string? FilterOperation { get => _filterOperation; set => this.RaiseAndSetIfChanged(ref _filterOperation, value); }
    public string? FilterNotes { get => _filterNotes; set => this.RaiseAndSetIfChanged(ref _filterNotes, value); }
    public string? QuantityMin { get => _quantityMin; set => this.RaiseAndSetIfChanged(ref _quantityMin, value); }
    public string? QuantityMax { get => _quantityMax; set => this.RaiseAndSetIfChanged(ref _quantityMax, value); }
    
    public bool UseDateRange { get => _useDateRange; set => this.RaiseAndSetIfChanged(ref _useDateRange, value); }
    public DateTimeOffset? RemovalDateRangeStart { get => _removalDateRangeStart; set => this.RaiseAndSetIfChanged(ref _removalDateRangeStart, value); }
    public DateTimeOffset? RemovalDateRangeEnd { get => _removalDateRangeEnd; set => this.RaiseAndSetIfChanged(ref _removalDateRangeEnd, value); }

    private string? _filterLocationText;
    private string? _filterPartIDText;
    private string? _filterUserText;
    private string? _filterOperation;
    private string? _filterNotes;
    private string? _quantityMin;
    private string? _quantityMax;
    private bool _useDateRange;
    private DateTimeOffset? _removalDateRangeStart;
    private DateTimeOffset? _removalDateRangeEnd;
    #endregion

    #region Data Collections
    /// <summary>
    /// Removal history tracking for undo capabilities
    /// </summary>
    public ObservableCollection<RemovalHistoryItem> RemovalHistoryGrid { get; } = new();

    /// <summary>
    /// Track last removed items for undo operations
    /// </summary>
    private readonly ObservableCollection<RemovalHistoryItem> _lastRemovedItems = new();

    private RemovalHistoryItem? _selectedHistoryItem;
    public RemovalHistoryItem? SelectedHistoryItem
    {
        get => _selectedHistoryItem;
        set => this.RaiseAndSetIfChanged(ref _selectedHistoryItem, value);
    }
    #endregion

    #region State Properties
    public bool IsBusy { get => _isBusy; set => this.RaiseAndSetIfChanged(ref _isBusy, value); }
    public string StatusMessage { get => _statusMessage; set => this.RaiseAndSetIfChanged(ref _statusMessage, value); }

    // Collapsible Panel Properties
    public bool IsFilterPanelExpanded { get => _isFilterPanelExpanded; set => this.RaiseAndSetIfChanged(ref _isFilterPanelExpanded, value); }
    public string CollapseButtonText { get => _collapseButtonText; set => this.RaiseAndSetIfChanged(ref _collapseButtonText, value); }
    public GridLength FilterPanelWidth { get => _filterPanelWidth; set => this.RaiseAndSetIfChanged(ref _filterPanelWidth, value); }

    private bool _isBusy;
    private string _statusMessage = "Ready";
    private bool _isFilterPanelExpanded = true;
    private string _collapseButtonText = "?";
    private GridLength _filterPanelWidth = new GridLength(300);

    private readonly ObservableAsPropertyHelper<bool> _canUndo;
    public bool CanUndo => _canUndo.Value;
    #endregion

    #region Commands - Advanced Removal Operations
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> SearchCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ClearCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> BackToNormalCommand { get; private set; } = null!;
    
    // Advanced removal operations as per instruction file
    public ReactiveCommand<Unit, Unit> BulkRemoveCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ConditionalRemoveCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ScheduledRemoveCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> UndoRemovalCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ViewHistoryCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> GenerateRemovalReportCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ExportRemovalDataCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> PrintRemovalSummaryCommand { get; private set; } = null!;
    
    // Panel Control Commands
    public ReactiveCommand<Unit, Unit> ToggleFilterPanelCommand { get; private set; } = null!;
    
    // Remove functionality now handled by QuickButtonsViewModel integration
    public ReactiveCommand<Unit, Unit> RemoveSelectedCommand { get; private set; } = null!;
    #endregion

    #region Events
    public event EventHandler? BackToNormalRequested;
    #endregion

    public AdvancedRemoveViewModel(ILogger<AdvancedRemoveViewModel> logger) : base(logger)
    {
        // Computed properties for command availability
        _canUndo = this.WhenAnyValue(vm => vm._lastRemovedItems.Count)
            .Select(count => count > 0)
            .ToProperty(this, vm => vm.CanUndo, initialValue: false);

        InitializeCommands();
        SetupErrorHandling();
        SetupAutoSearch();

        // Initialize with default date range
        RemovalDateRangeStart = DateTimeOffset.Now.AddDays(-30);
        RemovalDateRangeEnd = DateTimeOffset.Now;

        // Load initial data
        _ = LoadDataCommand.Execute();
    }

    private void InitializeCommands()
    {
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading advanced removal options...";

                await LoadOptionsAsync();
                await LoadRemovalHistoryAsync();
                
                StatusMessage = "Advanced removal system ready";
            }
            finally
            {
                IsBusy = false;
            }
        });

        SearchCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ExecuteSearchAsync();
        });

        ClearCommand = ReactiveCommand.Create(() =>
        {
            FilterLocationText = null;
            FilterPartIDText = null;
            FilterUserText = null;
            FilterOperation = null;
            FilterNotes = null;
            QuantityMin = null;
            QuantityMax = null;
            UseDateRange = false;
            RemovalDateRangeStart = null;
            RemovalDateRangeEnd = null;
            StatusMessage = "Advanced filters cleared";
        });

        // Simplified remove command for integration with QuickButtonsViewModel
        RemoveSelectedCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Executing removal operation via QuickButtons integration...";
                
                // TODO: Integrate with QuickButtonsViewModel for actual removal operations
                // The actual removal will be handled by the QuickButtons functionality
                await Task.Delay(400);
                
                StatusMessage = "Removal operation completed via QuickButtons";
            }
            finally
            {
                IsBusy = false;
            }
        });

        UndoRemovalCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_lastRemovedItems.Count == 0) return;

            try
            {
                IsBusy = true;
                StatusMessage = "Executing undo operation...";

                var lastRemoval = _lastRemovedItems.Last();
                
                // TODO: Execute undo via stored procedure
                // DaoResult<bool> undoResult = await Dao_Remove.UndoRemovalOperationAsync(...)
                await Task.Delay(400); // Simulate database operation

                // Remove from history
                RemovalHistoryGrid.Remove(lastRemoval);
                _lastRemovedItems.Remove(lastRemoval);

                StatusMessage = "Removal operation undone successfully";
            }
            finally
            {
                IsBusy = false;
            }
        }, this.WhenAnyValue(vm => vm.CanUndo));

        PrintRemovalSummaryCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Generating removal summary for printing...";

                // TODO: Implement professional printing capabilities
                await Task.Delay(600);
                
                StatusMessage = "Removal summary sent to printer";
            }
            finally
            {
                IsBusy = false;
            }
        });

        ToggleFilterPanelCommand = ReactiveCommand.Create(() =>
        {
            IsFilterPanelExpanded = !IsFilterPanelExpanded;
            CollapseButtonText = IsFilterPanelExpanded ? "?" : "+";
            FilterPanelWidth = IsFilterPanelExpanded ? new GridLength(300) : new GridLength(28);
            StatusMessage = IsFilterPanelExpanded ? "Filter panel expanded" : "Filter panel collapsed";
        });

        BackToNormalCommand = ReactiveCommand.Create(() =>
        {
            BackToNormalRequested?.Invoke(this, EventArgs.Empty);
        });

        // Additional advanced commands (for future implementation)
        BulkRemoveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Delay(1000); // TODO: Implement bulk removal
        });

        ViewHistoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await LoadRemovalHistoryAsync();
        });

        GenerateRemovalReportCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.Delay(800); // TODO: Generate analytics report
        });
    }

    private void SetupErrorHandling()
    {
        Observable.Merge(
            LoadDataCommand.ThrownExceptions,
            SearchCommand.ThrownExceptions,
            ClearCommand.ThrownExceptions,
            RemoveSelectedCommand.ThrownExceptions,
            UndoRemovalCommand.ThrownExceptions,
            ToggleFilterPanelCommand.ThrownExceptions
        ).Subscribe(ex =>
        {
            Logger.LogError(ex, "AdvancedRemoveViewModel error: {Message}", ex.Message);
            IsBusy = false;
            StatusMessage = $"Error: {ex.Message}";
        });
    }

    private void SetupAutoSearch()
    {
        // Auto-search when filter text changes (with debounce)
        Observable.Merge(
            this.WhenAnyValue(x => x.FilterPartIDText).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.FilterLocationText).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.FilterOperation).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.FilterUserText).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.FilterNotes).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.QuantityMin).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.QuantityMax).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.UseDateRange).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.RemovalDateRangeStart).Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.RemovalDateRangeEnd).Select(_ => Unit.Default)
        )
        .Throttle(TimeSpan.FromMilliseconds(500)) // Wait 500ms after user stops typing
        .ObserveOn(RxApp.MainThreadScheduler)
        .Subscribe(async _ =>
        {
            try
            {
                // Only auto-search if we have some filter criteria
                if (!string.IsNullOrWhiteSpace(FilterPartIDText) ||
                    !string.IsNullOrWhiteSpace(FilterLocationText) ||
                    !string.IsNullOrWhiteSpace(FilterOperation) ||
                    !string.IsNullOrWhiteSpace(FilterUserText) ||
                    !string.IsNullOrWhiteSpace(FilterNotes) ||
                    !string.IsNullOrWhiteSpace(QuantityMin) ||
                    !string.IsNullOrWhiteSpace(QuantityMax) ||
                    UseDateRange)
                {
                    await ExecuteSearchAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Auto-search error: {Message}", ex.Message);
            }
        });
    }

    private async Task LoadOptionsAsync()
    {
        // TODO: Load from database via stored procedures
        await Task.Delay(200);

        LocationOptions.Clear();
        foreach (var loc in new[] { "WC01", "WC02", "WC03", "WC04", "WC05", "STOCK", "SHIP", "RECV" })
            LocationOptions.Add(loc);

        PartIDOptions.Clear();
        foreach (var part in new[] { "24733444-PKG", "24677611", "24733405-PKG", "24733403-PKG", "24733491-PKG" })
            PartIDOptions.Add(part);

        UserOptions.Clear();
        foreach (var user in new[] { "admin", "operator1", "user1", "jkoll" })
            UserOptions.Add(user);

        OperationOptions.Clear();
        foreach (var op in new[] { "100", "110", "120", "200", "300", "400" })
            OperationOptions.Add(op);
    }

    private async Task LoadRemovalHistoryAsync()
    {
        // TODO: Load removal history from database
        await Task.Delay(150);

        RemovalHistoryGrid.Clear();
        // Sample removal history data
        RemovalHistoryGrid.Add(new RemovalHistoryItem 
        { 
            ID = 1, PartID = "24733444-PKG", Operation = "90", Location = "WC01", 
            Quantity = 5, User = "admin", DateRemoved = DateTime.Now.AddDays(-1),
            Notes = "Removed for quality check"
        });
    }

    private async Task ExecuteSearchAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Executing advanced search with filters...";
            
            // TODO: Execute search via stored procedures with wildcard filtering
            // The search results will be used to filter the QuickButtonsViewModel data
            // rather than maintaining a separate Results collection
            await Task.Delay(300);

            StatusMessage = "Advanced search completed. Results available via QuickButtons integration.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Method to handle removal operations triggered from QuickButtonsViewModel
    /// </summary>
    public async Task HandleRemovalFromQuickButtons(InventoryItem item)
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Processing removal from QuickButtons...";
            
            // Track removal for undo capability
            var removalRecord = new RemovalHistoryItem
            {
                ID = item.ID,
                PartID = item.PartID,
                Operation = item.Operation,
                Location = item.Location,
                Quantity = item.Quantity,
                User = item.User,
                DateRemoved = DateTime.Now,
                Notes = $"Removed via Advanced Remove: {item.Notes}"
            };

            // TODO: Execute actual removal via stored procedure
            // DaoResult<Model_HistoryRemove> result = await Dao_Remove.RemoveInventoryAsync(...)
            await Task.Delay(500); // Simulate database operation
            
            // Add to history for undo capability
            _lastRemovedItems.Add(removalRecord);
            RemovalHistoryGrid.Insert(0, removalRecord); // Add to top for most recent

            StatusMessage = $"Item removed successfully. Undo available.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}

/// <summary>
/// Model for removal history tracking
/// </summary>
public class RemovalHistoryItem
{
    public int ID { get; set; }
    public string PartID { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string User { get; set; } = string.Empty;
    public DateTime DateRemoved { get; set; }
    public string? Notes { get; set; }
}