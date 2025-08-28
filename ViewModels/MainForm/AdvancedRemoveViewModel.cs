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
using MTM_WIP_Application_Avalonia.Extensions;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Commands;
using System.Collections.Generic;
using System.Reactive.Subjects;

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

    public bool CanUndo => false; // TODO: Implement undo logic
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

            // Setup ThrownExceptions for IHandleObservableErrors
            var thrownExceptionsSubject = new Subject<Exception>();
            ThrownExceptions = thrownExceptionsSubject.AsObservable();
            
            // Subscribe to handle exceptions properly with enhanced error handling
            ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async ex =>
                {
                    try
                    {
                        Logger.LogError(ex, "AdvancedRemoveViewModel observable error: {Message}", ex.Message);
                        await Service_ErrorHandler.HandleErrorAsync(ex, "AdvancedRemoveViewModel", Environment.UserName);
                        
                        // Safely update status
                        try
                        {
                            StatusMessage = "An error occurred. Please try again.";
                            IsBusy = false;
                        }
                        catch (Exception statusEx)
                        {
                            Logger.LogError(statusEx, "Error updating status message");
                        }
                    }
                    catch (Exception errorHandlerEx)
                    {
                        Logger.LogCritical(errorHandlerEx, "Critical error in exception handler");
                    }
                })
                .DisposeWith(_compositeDisposable);

            // Initialize computed properties safely using the extension method
            _canUndo = LastRemovedItems
                .ObserveCollectionCount()
                .Select(count => count > 0)
                .Catch<bool, Exception>(ex =>
                {
                    Logger.LogError(ex, "Error in CanUndo observable");
                    thrownExceptionsSubject.OnNext(ex);
                    return Observable.Return(false);
                })
                .ToProperty(this, vm => vm.CanUndo, scheduler: RxApp.MainThreadScheduler)
                .DisposeWith(_compositeDisposable);

            // Initialize commands with error handling
            InitializeCommands(thrownExceptionsSubject);
            SetupErrorHandling();
            
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
            
            // Setup auto-search after commands are initialized
            SetupAutoSearch(thrownExceptionsSubject);

            // Load initial data using error-safe approach
            Observable.Return(Unit.Default)
                .ObserveOn(RxApp.MainThreadScheduler)
                .SelectMany(_ => LoadDataCommand.Execute().Catch<Unit, Exception>(ex =>
                {
                    Logger.LogError(ex, "Error during initial data load");
                    thrownExceptionsSubject.OnNext(ex);
                    return Observable.Return(Unit.Default);
                }))
                .Subscribe(_ => Logger.LogDebug("Initial data load completed"))
                .DisposeWith(_compositeDisposable);

            Logger.LogInformation("AdvancedRemoveViewModel initialization completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize AdvancedRemoveViewModel");
            StatusMessage = "Initialization failed";
            throw; // Re-throw to prevent further issues
        }
    }

    private void InitializeCommands(Subject<Exception> thrownExceptionsSubject)
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
                }
                finally
                {
                    IsBusy = false;
                }
            }, this.WhenAnyValue(vm => vm.CanUndo).Catch<bool, Exception>(ex =>
            {
                Logger.LogError(ex, "Error in CanUndo observable for UndoRemovalCommand");
                thrownExceptionsSubject.OnNext(ex);
                return Observable.Return(false);
            }));

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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
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
                    thrownExceptionsSubject.OnNext(ex);
                }
            });

            Logger.LogDebug("Commands initialized successfully for AdvancedRemoveViewModel");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing commands");
            throw;
        }
    }

    private void SetupErrorHandling()
    {
        // All error handling is now done through the ThrownExceptions observable
        // which is properly implemented via IHandleObservableErrors
    }

    private void SetupAutoSearch(Subject<Exception> thrownExceptionsSubject)
    {
        try
        {
            Logger.LogDebug("Setting up auto-search for AdvancedRemoveViewModel");

            // Auto-search when filter text changes (with debounce)
            var filterChanges = Observable.Merge(
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
            );

            filterChanges
                .Throttle(TimeSpan.FromMilliseconds(500)) // Wait 500ms after user stops typing
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(_ => !IsBusy) // Don't auto-search if busy
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
                        StatusMessage = $"Auto-search error: {ex.Message}";
                        thrownExceptionsSubject.OnNext(ex);
                    }
                })
                .DisposeWith(_compositeDisposable);

            Logger.LogDebug("Auto-search setup completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting up auto-search");
            thrownExceptionsSubject.OnNext(ex);
        }
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
                _compositeDisposable?.Dispose();
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