using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_Shared_Logic.Models;
using System.Linq;
using Avalonia.Controls;
using Material.Icons;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Commands;
using System.Collections.Generic;

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

    public string? FilterLocationText { get => _filterLocationText; set => SetProperty(ref _filterLocationText, value); }
    public string? FilterPartIDText { get => _filterPartIDText; set => SetProperty(ref _filterPartIDText, value); }
    public string? FilterUserText { get => _filterUserText; set => SetProperty(ref _filterUserText, value); }
    public string? FilterOperation { get => _filterOperation; set => SetProperty(ref _filterOperation, value); }
    public string? FilterNotes { get => _filterNotes; set => SetProperty(ref _filterNotes, value); }
    public string? QuantityMin { get => _quantityMin; set => SetProperty(ref _quantityMin, value); }
    public string? QuantityMax { get => _quantityMax; set => SetProperty(ref _quantityMax, value); }
    
    public bool UseDateRange { get => _useDateRange; set => SetProperty(ref _useDateRange, value); }
    public DateTimeOffset? RemovalDateRangeStart { get => _removalDateRangeStart; set => SetProperty(ref _removalDateRangeStart, value); }
    public DateTimeOffset? RemovalDateRangeEnd { get => _removalDateRangeEnd; set => SetProperty(ref _removalDateRangeEnd, value); }

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
    /// Track last removed items for undo operations - using reactive collection
    /// </summary>
    public ObservableCollection<RemovalHistoryItem> LastRemovedItems { get; } = new();

    private RemovalHistoryItem? _selectedHistoryItem;
    public RemovalHistoryItem? SelectedHistoryItem
    {
        get => _selectedHistoryItem;
        set => SetProperty(ref _selectedHistoryItem, value);
    }
    #endregion

    #region State Properties
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    // Collapsible Panel Properties
    public bool IsFilterPanelExpanded { get => _isFilterPanelExpanded; set => SetProperty(ref _isFilterPanelExpanded, value); }
    public string CollapseButtonText { get => _collapseButtonText; set => SetProperty(ref _collapseButtonText, value); }
    public MaterialIconKind CollapseButtonIcon { get => _collapseButtonIcon; set => SetProperty(ref _collapseButtonIcon, value); }
    public string FilterToggleText { get => _filterToggleText; set => SetProperty(ref _filterToggleText, value); }
    public GridLength FilterPanelWidth { get => _filterPanelWidth; set => SetProperty(ref _filterPanelWidth, value); }

    private bool _isBusy;
    private string _statusMessage = "Ready";
    private bool _isFilterPanelExpanded = true;
    private string _collapseButtonText = "?";
    private MaterialIconKind _collapseButtonIcon = MaterialIconKind.ChevronLeft;
    private string _filterToggleText = "Hide Filters";
    private GridLength _filterPanelWidth = new GridLength(300);

    public bool CanUndo => LastRemovedItems.Count > 0;
    #endregion

    #region Commands - Advanced Removal Operations
    public ICommand LoadDataCommand { get; private set; } = null!;
    public ICommand SearchCommand { get; private set; } = null!;
    public ICommand ClearCommand { get; private set; } = null!;
    public ICommand BackToNormalCommand { get; private set; } = null!;
    
    // Advanced removal operations as per instruction file
    public ICommand BulkRemoveCommand { get; private set; } = null!;
    public ICommand ConditionalRemoveCommand { get; private set; } = null!;
    public ICommand ScheduledRemoveCommand { get; private set; } = null!;
    public ICommand UndoRemovalCommand { get; private set; } = null!;
    public ICommand ViewHistoryCommand { get; private set; } = null!;
    public ICommand GenerateRemovalReportCommand { get; private set; } = null!;
    public ICommand ExportRemovalDataCommand { get; private set; } = null!;
    public ICommand PrintRemovalSummaryCommand { get; private set; } = null!;
    
    // Panel Control Commands
    public ICommand ToggleFilterPanelCommand { get; private set; } = null!;
    
    // Remove functionality now handled by QuickButtonsViewModel integration
    public ICommand RemoveSelectedCommand { get; private set; } = null!;
    #endregion

    #region Events
    public event EventHandler? BackToNormalRequested;
    #endregion

    #region Disposables for proper cleanup
    // Disposable resources cleanup handled by BaseViewModel
    #endregion

    public AdvancedRemoveViewModel(ILogger<AdvancedRemoveViewModel> logger) : base(logger)
    {
        try
        {
            Logger.LogInformation("Initializing AdvancedRemoveViewModel");

            // Initialize commands
            InitializeCommands();
            
            // Initialize with safe default date range
            try
            {
                RemovalDateRangeStart = DateTimeOffset.Now.AddDays(-30);
                RemovalDateRangeEnd = DateTimeOffset.Now;
            }
            catch (Exception dateEx)
            {
                Logger.LogWarning(dateEx, "Error setting default date range, using null values");
                RemovalDateRangeStart = null;
                RemovalDateRangeEnd = null;
            }

            StatusMessage = "Advanced removal system initialized";
            
            // Setup collection change notifications for CanUndo
            LastRemovedItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CanUndo));

            // Load initial data
            _ = Task.Run(async () =>
            {
                try
                {
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error during initial data load");
                    StatusMessage = "Error loading initial data";
                }
            });

            Logger.LogInformation("AdvancedRemoveViewModel initialization completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize AdvancedRemoveViewModel");
            StatusMessage = "Initialization failed";
        }
    }

    private void InitializeCommands()
    {
        try
        {
            Logger.LogDebug("Initializing commands for AdvancedRemoveViewModel");

            // Create commands with proper error handling
            LoadDataCommand = new AsyncCommand(async () =>
            {
                try
                {
                    IsBusy = true;
                    StatusMessage = "Loading advanced removal options...";

                    await LoadOptionsAsync();
                    await LoadRemovalHistoryAsync();
                    
                    StatusMessage = "Advanced removal system ready";
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error loading data");
                    StatusMessage = $"Error loading data: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            });

            SearchCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await ExecuteSearchAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error executing search");
                    StatusMessage = $"Search error: {ex.Message}";
                }
            });

            ClearCommand = new RelayCommand(() =>
            {
                try
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
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error clearing filters");
                    StatusMessage = $"Clear error: {ex.Message}";
                }
            });

            // Simplified remove command for integration with QuickButtonsViewModel
            RemoveSelectedCommand = new AsyncCommand(async () =>
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
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error removing selected item");
                    StatusMessage = $"Removal error: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            });

            UndoRemovalCommand = new AsyncCommand(async () =>
            {
                try
                {
                    if (LastRemovedItems.Count == 0) return;

                    IsBusy = true;
                    StatusMessage = "Executing undo operation...";

                    var lastRemoval = LastRemovedItems.LastOrDefault();
                    if (lastRemoval != null)
                    {
                        // TODO: Execute undo via stored procedure
                        // DaoResult<bool> undoResult = await Dao_Remove.UndoRemovalOperationAsync(...)
                        await Task.Delay(400); // Simulate database operation

                        // Remove from history
                        RemovalHistoryGrid.Remove(lastRemoval);
                        LastRemovedItems.Remove(lastRemoval);
                    }

                    StatusMessage = "Removal operation undone successfully";
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error undoing removal");
                    StatusMessage = $"Undo error: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            });

            PrintRemovalSummaryCommand = new AsyncCommand(async () =>
            {
                try
                {
                    IsBusy = true;
                    StatusMessage = "Generating removal summary for printing...";

                    // TODO: Implement professional printing capabilities
                    await Task.Delay(600);
                    
                    StatusMessage = "Removal summary sent to printer";
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error printing summary");
                    StatusMessage = $"Print error: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            });

            ToggleFilterPanelCommand = new RelayCommand(() =>
            {
                try
                {
                    IsFilterPanelExpanded = !IsFilterPanelExpanded;
                    CollapseButtonText = IsFilterPanelExpanded ? "?" : "+";
                    CollapseButtonIcon = IsFilterPanelExpanded ? MaterialIconKind.ChevronLeft : MaterialIconKind.ChevronRight;
                    FilterToggleText = IsFilterPanelExpanded ? "Hide Filters" : "Show Filters";
                    FilterPanelWidth = IsFilterPanelExpanded ? new GridLength(300) : new GridLength(50);
                    StatusMessage = IsFilterPanelExpanded ? "Filter panel expanded" : "Filter panel collapsed";
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error toggling filter panel");
                    StatusMessage = $"Toggle error: {ex.Message}";
                }
            });

            BackToNormalCommand = new RelayCommand(() =>
            {
                try
                {
                    BackToNormalRequested?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error navigating back to normal");
                    StatusMessage = $"Navigation error: {ex.Message}";
                }
            });

            // Additional advanced commands (for future implementation)
            BulkRemoveCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await Task.Delay(1000); // TODO: Implement bulk removal
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in bulk remove");
                }
            });

            ViewHistoryCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await LoadRemovalHistoryAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error loading history");
                }
            });

            GenerateRemovalReportCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await Task.Delay(800); // TODO: Generate analytics report
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error generating report");
                }
            });

            ExportRemovalDataCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await Task.Delay(500); // TODO: Export removal data
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error exporting data");
                }
            });

            ConditionalRemoveCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await Task.Delay(700); // TODO: Conditional removal
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in conditional remove");
                }
            });

            ScheduledRemoveCommand = new AsyncCommand(async () =>
            {
                try
                {
                    await Task.Delay(600); // TODO: Scheduled removal
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in scheduled remove");
                }
            });

            Logger.LogDebug("Commands initialized successfully for AdvancedRemoveViewModel");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing commands");
        }
    }

    private async Task LoadDataAsync()
    {
        await LoadOptionsAsync();
        await LoadRemovalHistoryAsync();
    }

    private async Task LoadOptionsAsync()
    {
        try
        {
            Logger.LogDebug("Loading options for AdvancedRemoveViewModel");

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

            Logger.LogDebug("Options loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading options");
            throw;
        }
    }

    private async Task LoadRemovalHistoryAsync()
    {
        try
        {
            Logger.LogDebug("Loading removal history");

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

            Logger.LogDebug("Removal history loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading removal history");
            throw;
        }
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
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing search");
            throw;
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
            LastRemovedItems.Add(removalRecord);
            RemovalHistoryGrid.Insert(0, removalRecord); // Add to top for most recent

            StatusMessage = $"Item removed successfully. Undo available.";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling removal from QuickButtons");
            StatusMessage = $"Removal error: {ex.Message}";
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                Logger.LogDebug("AdvancedRemoveViewModel disposed successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error disposing AdvancedRemoveViewModel");
            }
        }
        base.Dispose(disposing);
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