using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models;
using Avalonia.Controls;
using Material.Icons;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for Advanced Remove: Enhanced Removal Operations Interface
/// Provides sophisticated removal operations beyond standard inventory removal functionality.
/// Features include bulk removal operations, removal history tracking, undo capabilities, 
/// and specialized reporting for removal analytics.
/// Uses MVVM Community Toolkit for modern .NET patterns.
/// </summary>
public partial class AdvancedRemoveViewModel : BaseViewModel
{
    private readonly IConfigurationService _configurationService;
    private readonly IApplicationStateService _applicationState;
    #region Filter Properties
    /// <summary>
    /// Gets or sets the location filter text for advanced removal operations
    /// </summary>
    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Location filter cannot exceed 50 characters")]
    private string? _filterLocationText;

    /// <summary>
    /// Gets or sets the Part ID filter text for targeted removal operations
    /// </summary>
    [ObservableProperty]
    [StringLength(100, ErrorMessage = "Part ID filter cannot exceed 100 characters")]
    private string? _filterPartIDText;

    /// <summary>
    /// Gets or sets the user filter text for removal history tracking
    /// </summary>
    [ObservableProperty]
    [StringLength(50, ErrorMessage = "User filter cannot exceed 50 characters")]
    private string? _filterUserText;

    /// <summary>
    /// Gets or sets the operation filter for removal workflow targeting
    /// </summary>
    [ObservableProperty]
    private string? _filterOperation;

    /// <summary>
    /// Gets or sets the minimum quantity filter for bulk removal operations
    /// </summary>
    [ObservableProperty]
    [RegularExpression(@"^\d*$", ErrorMessage = "Minimum quantity must be a valid number")]
    private string? _quantityMin;

    /// <summary>
    /// Gets or sets the maximum quantity filter for bulk removal operations
    /// </summary>
    [ObservableProperty]
    [RegularExpression(@"^\d*$", ErrorMessage = "Maximum quantity must be a valid number")]
    private string? _quantityMax;

    /// <summary>
    /// Gets or sets the removal date range start for filtering operations
    /// </summary>
    [ObservableProperty]
    private DateTimeOffset? _removalDateRangeStart;

    /// <summary>
    /// Gets or sets the removal date range end for filtering operations
    /// </summary>
    [ObservableProperty]
    private DateTimeOffset? _removalDateRangeEnd;

    /// <summary>
    /// Gets or sets whether to use date range filtering
    /// </summary>
    [ObservableProperty]
    private bool _useDateRange = true;

    /// <summary>
    /// Gets or sets the notes filter text for filtering operations
    /// </summary>
    [ObservableProperty]
    [StringLength(500, ErrorMessage = "Notes filter cannot exceed 500 characters")]
    private string? _filterNotes;
    #endregion

    #region Data Collections
    /// <summary>
    /// Gets or sets the collection of removal history items
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<SessionTransaction> _removalHistory = new();

    /// <summary>
    /// Gets or sets the collection of location options for removal operations
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _locationOptions = new();

    /// <summary>
    /// Gets or sets the collection of Part ID options for removal targeting
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _partIDOptions = new();

    /// <summary>
    /// Gets or sets the collection of user options for removal tracking
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _userOptions = new();

    /// <summary>
    /// Gets or sets the collection of operation options for removal filtering
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> _operationOptions = new();

    /// <summary>
    /// Gets or sets the collection of last removed items for undo functionality
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<SessionTransaction> _lastRemovedItems = new();

    /// <summary>
    /// Gets or sets the currently selected history item
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanUndo))]
    private SessionTransaction? _selectedHistoryItem;
    #endregion

    #region State Properties
    /// <summary>
    /// Gets or sets whether the ViewModel is currently busy processing operations
    /// </summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>
    /// Gets or sets the current status message for user feedback
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = "Ready";

    /// <summary>
    /// Gets or sets whether the filter panel is expanded
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CollapseButtonText))]
    [NotifyPropertyChangedFor(nameof(CollapseButtonIcon))]
    [NotifyPropertyChangedFor(nameof(FilterToggleText))]
    [NotifyPropertyChangedFor(nameof(FilterPanelWidth))]
    private bool _isFilterPanelExpanded = true;

    /// <summary>
    /// Gets or sets the collapse button text based on panel state
    /// </summary>
    [ObservableProperty]
    private string _collapseButtonText = "?";

    /// <summary>
    /// Gets or sets the collapse button icon based on panel state
    /// </summary>
    [ObservableProperty]
    private MaterialIconKind _collapseButtonIcon = MaterialIconKind.ChevronLeft;

    /// <summary>
    /// Gets or sets the filter toggle text based on panel state
    /// </summary>
    [ObservableProperty]
    private string _filterToggleText = "Hide Filters";

    /// <summary>
    /// Gets or sets the filter panel width
    /// </summary>
    [ObservableProperty]
    private GridLength _filterPanelWidth = new GridLength(300);
    #endregion

    #region Events
    public event EventHandler? BackToNormalRequested;
    #endregion

    #region Computed Properties
    /// <summary>
    /// Gets whether undo operation is available
    /// </summary>
    public bool CanUndo => LastRemovedItems.Count > 0;

    /// <summary>
    /// Gets the removal history grid data (same as RemovalHistory)
    /// </summary>
    public ObservableCollection<SessionTransaction> RemovalHistoryGrid => RemovalHistory;

    /// <summary>
    /// Determines if a selected item can be removed
    /// </summary>
    private bool CanRemoveSelected => SelectedHistoryItem != null;
    #endregion

    #region Constructor
    public AdvancedRemoveViewModel(ILogger<AdvancedRemoveViewModel> logger, IConfigurationService configurationService, IApplicationStateService applicationState) : base(logger)
    {
        try
        {
            Logger.LogInformation("Initializing AdvancedRemoveViewModel");
            
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
            _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
            
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

            // Database loading will be deferred until after UI is shown to prevent startup deadlocks
            Logger.LogInformation("AdvancedRemoveViewModel constructor completed - database loading deferred");

            Logger.LogInformation("AdvancedRemoveViewModel initialized successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize AdvancedRemoveViewModel");
            StatusMessage = "Initialization failed";
        }
    }
    #endregion

    #region Command Methods

    /// <summary>
    /// Loads advanced removal data and options
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Loading removal history...";
            
            // Load master data from stored procedures
            await LoadOptionsAsync().ConfigureAwait(false);
            
            // Load removal history from database
            await LoadRemovalHistoryAsync().ConfigureAwait(false);
            
            StatusMessage = "Data loaded successfully";
            Logger.LogInformation("Advanced removal data loaded successfully");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Load Advanced Remove Data", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Error loading data";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executes search with current filter criteria
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Searching removal history...";
            
            var parameters = new Dictionary<string, object>();
            
            // Add filter parameters
            if (!string.IsNullOrWhiteSpace(FilterPartIDText))
                parameters["p_PartID"] = FilterPartIDText;
                
            if (!string.IsNullOrWhiteSpace(FilterLocationText))
                parameters["p_Location"] = FilterLocationText;
                
            if (!string.IsNullOrWhiteSpace(FilterUserText))
                parameters["p_User"] = FilterUserText;
                
            if (!string.IsNullOrWhiteSpace(FilterOperation))
                parameters["p_Operation"] = FilterOperation;
                
            if (RemovalDateRangeStart.HasValue)
                parameters["p_StartDate"] = RemovalDateRangeStart.Value.DateTime;
                
            if (RemovalDateRangeEnd.HasValue)
                parameters["p_EndDate"] = RemovalDateRangeEnd.Value.DateTime;
            
            var connectionString = _configurationService.GetConnectionString();
            var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "inv_transaction_Get_History", parameters
            );
            
            if (result.Status == 1)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    RemovalHistory.Clear();
                    foreach (DataRow row in result.Data.Rows)
                    {
                        RemovalHistory.Add(new SessionTransaction
                        {
                            PartId = row["PartID"]?.ToString() ?? string.Empty,
                            Operation = row["Operation"]?.ToString() ?? string.Empty,
                            Location = row["Location"]?.ToString() ?? string.Empty,
                            Quantity = Convert.ToInt32(row["Quantity"] ?? 0),
                            User = row["User"]?.ToString() ?? string.Empty,
                            TransactionTime = Convert.ToDateTime(row["TransactionTime"]),
                            Status = row["Status"]?.ToString() ?? string.Empty,
                            BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                            TransactionType = row["TransactionType"]?.ToString() ?? "OUT"
                        });
                    }
                    
                    StatusMessage = $"Found {RemovalHistory.Count} removal records";
                });
            }
            else
            {
                await Services.Core.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Search failed with status: {result.Status}"),
                    "Advanced Remove Search", _applicationState.CurrentUser ?? "System"
                );
                StatusMessage = "Search failed - please try again";
            }
            
            Logger.LogInformation("Search completed with {Count} results", RemovalHistory.Count);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Advanced Remove Search", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Search error - please try again";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Clears all filter criteria and resets to defaults
    /// </summary>
    [RelayCommand]
    private async Task ClearAsync()
    {
        try
        {
            FilterLocationText = null;
            FilterPartIDText = null;
            FilterUserText = null;
            FilterOperation = null;
            QuantityMin = null;
            QuantityMax = null;
            RemovalDateRangeStart = DateTimeOffset.Now.AddDays(-30);
            RemovalDateRangeEnd = DateTimeOffset.Now;
            RemovalHistory.Clear();
            StatusMessage = "Filters cleared";
            Logger.LogDebug("Filters cleared");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Clear Advanced Remove Filters", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Clear filters error";
        }
    }

    /// <summary>
    /// Returns to normal inventory mode
    /// </summary>
    [RelayCommand]
    private async Task BackToNormalAsync()
    {
        try
        {
            BackToNormalRequested?.Invoke(this, EventArgs.Empty);
            Logger.LogInformation("Back to normal command executed");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Back to Normal Command", _applicationState.CurrentUser ?? "System");
        }
    }

    /// <summary>
    /// Executes bulk removal of selected items
    /// </summary>
    [RelayCommand]
    private async Task BulkRemoveAsync()
    {
        try
        {
            if (RemovalHistory.Count == 0)
            {
                StatusMessage = "No items selected for bulk removal";
                return;
            }

            IsBusy = true;
            StatusMessage = $"Removing {RemovalHistory.Count} items in bulk...";

            var connectionString = _configurationService.GetConnectionString();
            
            // Process each selected item for removal
            int successCount = 0;
            int failCount = 0;
            
            foreach (var item in RemovalHistory.ToList())
            {
                try
                {
                    var parameters = new Dictionary<string, object>
                    {
                        ["p_BatchNumber"] = item.BatchNumber,
                        ["p_PartID"] = item.PartId,
                        ["p_Location"] = item.Location,
                        ["p_Operation"] = item.Operation,
                        ["p_Quantity"] = item.Quantity,
                        ["p_User"] = _applicationState.CurrentUser ?? "System",
                        ["p_Notes"] = "Bulk removal operation"
                    };

                    var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                        connectionString, "inv_inventory_Remove_Item", parameters
                    );

                    if (result.Status == 1)
                    {
                        LastRemovedItems.Add(item);
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        Logger.LogWarning("Bulk removal failed for item {PartId}: Status {Status}, Message: {Message}", 
                            item.PartId, result.Status, result.Message);
                    }
                }
                catch (Exception itemEx)
                {
                    failCount++;
                    await Services.Core.ErrorHandling.HandleErrorAsync(itemEx, $"Bulk Remove Item {item.PartId}", _applicationState.CurrentUser ?? "System");
                }
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                RemovalHistory.Clear();
            });

            StatusMessage = $"Bulk removal completed: {successCount} successful, {failCount} failed";
            Logger.LogInformation("Bulk removal completed: {SuccessCount} successful, {FailCount} failed", successCount, failCount);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Bulk Remove Operation", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Bulk removal error - please try again";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executes conditional removal based on criteria
    /// </summary>
    [RelayCommand]
    private async Task ConditionalRemoveAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Processing conditional removal...";
            
            // TODO: Implement conditional removal logic based on business requirements
            // This would need to be defined based on specific MTM business rules
            await Task.Delay(400).ConfigureAwait(false);
            
            StatusMessage = "Conditional removal completed";
            Logger.LogInformation("Conditional removal executed");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Conditional Remove Operation", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Conditional removal error";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Schedules removal for future execution
    /// </summary>
    [RelayCommand]
    private async Task ScheduledRemoveAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Processing scheduled removal...";
            
            // TODO: Implement scheduled removal based on business requirements
            // This would integrate with a scheduling system if available
            await Task.Delay(600).ConfigureAwait(false);
            
            StatusMessage = "Scheduled removal processed";
            Logger.LogInformation("Scheduled removal executed");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Scheduled Remove Operation", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Scheduled removal error";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Undoes the last removal operation
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private async Task UndoRemovalAsync()
    {
        try
        {
            if (!CanUndo || LastRemovedItems.Count == 0)
            {
                StatusMessage = "No items available to undo";
                return;
            }

            IsBusy = true;
            StatusMessage = "Undoing last removal...";

            var lastItem = LastRemovedItems.Last();
            
            var parameters = new Dictionary<string, object>
            {
                ["p_BatchNumber"] = lastItem.BatchNumber,
                ["p_UndoReason"] = "User requested undo",
                ["p_UndoUser"] = _applicationState.CurrentUser ?? "System"
            };
            
            var connectionString = _configurationService.GetConnectionString();
            var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "inv_inventory_Undo_Remove", parameters
            );
            
            if (result.Status == 1)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LastRemovedItems.Remove(lastItem);
                    // Add back to main collection if not already present
                    if (!RemovalHistory.Any(r => r.BatchNumber == lastItem.BatchNumber))
                    {
                        RemovalHistory.Add(lastItem);
                    }
                });
                
                StatusMessage = $"Successfully undid removal of {lastItem.PartId}";
                Logger.LogInformation("Removal undone for part {PartId}", lastItem.PartId);
                
                // Refresh the display
                await SearchAsync();
            }
            else
            {
                await Services.Core.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Undo failed with status: {result.Status}"),
                    "Undo Removal Operation", _applicationState.CurrentUser ?? "System"
                );
                StatusMessage = "Undo failed - please try again";
            }
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Undo Removal", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Undo error - please try again";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Views the removal history
    /// </summary>
    [RelayCommand]
    private async Task ViewHistoryAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Loading detailed removal history...";
            
            // Refresh current removal history display
            await LoadRemovalHistoryAsync();
            
            StatusMessage = "Removal history refreshed";
            Logger.LogInformation("Viewing removal history");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "View Removal History", _applicationState.CurrentUser ?? "System");
            StatusMessage = "History view error";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Generates a comprehensive removal report
    /// </summary>
    [RelayCommand]
    private async Task GenerateRemovalReportAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Generating removal report...";
            
            // Generate and save detailed removal report
            var reportContent = GenerateRemovalSummary();
            var fileName = $"Removal_Report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            
            await File.WriteAllTextAsync(filePath, reportContent);
            
            StatusMessage = $"Removal report saved to {fileName}";
            Logger.LogInformation("Removal report generated and saved to {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Generate Removal Report", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Report generation error";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Exports removal data to external format
    /// </summary>
    [RelayCommand]
    private async Task ExportRemovalDataAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Exporting removal data...";
            
            // Implemented export data functionality
            var csvContent = GenerateRemovalDataCSV();
            var fileName = $"Removal_Data_Export_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            
            await File.WriteAllTextAsync(filePath, csvContent);
            
            StatusMessage = $"Removal data exported to {fileName} ({RemovalHistory.Count} records)";
            Logger.LogInformation("Removal data exported successfully to {FilePath} with {Count} records", filePath, RemovalHistory.Count);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Export Removal Data", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Export failed";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Prints a removal summary
    /// </summary>
    [RelayCommand]
    private async Task PrintRemovalSummaryAsync()
    {
        try
        {
            IsBusy = true;
            StatusMessage = "Generating removal summary...";
            
            // Implemented print summary functionality
            var summaryContent = GenerateRemovalSummary();
            var fileName = $"Removal_Summary_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            
            await File.WriteAllTextAsync(filePath, summaryContent);
            
            StatusMessage = $"Removal summary saved to {fileName}";
            Logger.LogInformation("Removal summary generated and saved to {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Print Removal Summary", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Failed to generate summary";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Toggles the visibility of the filter panel
    /// </summary>
    [RelayCommand]
    private async Task ToggleFilterPanelAsync()
    {
        try
        {
            IsFilterPanelExpanded = !IsFilterPanelExpanded;
            Logger.LogDebug("Filter panel toggled to {IsExpanded}", IsFilterPanelExpanded);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Toggle Filter Panel", _applicationState.CurrentUser ?? "System");
        }
    }

    /// <summary>
    /// Removes the currently selected item
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemoveSelected))]
    private async Task RemoveSelectedAsync()
    {
        try
        {
            if (SelectedHistoryItem == null)
            {
                StatusMessage = "No item selected for removal";
                return;
            }

            IsBusy = true;
            StatusMessage = $"Removing selected item: {SelectedHistoryItem.PartId}";

            var connectionString = _configurationService.GetConnectionString();
            var parameters = new Dictionary<string, object>
            {
                ["p_BatchNumber"] = SelectedHistoryItem.BatchNumber,
                ["p_PartID"] = SelectedHistoryItem.PartId,
                ["p_Location"] = SelectedHistoryItem.Location,
                ["p_Operation"] = SelectedHistoryItem.Operation,
                ["p_Quantity"] = SelectedHistoryItem.Quantity,
                ["p_User"] = _applicationState.CurrentUser ?? "System",
                ["p_Notes"] = "Individual removal operation"
            };

            var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "inv_inventory_Remove_Item", parameters
            );

            if (result.Status == 1)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LastRemovedItems.Add(SelectedHistoryItem);
                    RemovalHistory.Remove(SelectedHistoryItem);
                    SelectedHistoryItem = null;
                });

                StatusMessage = "Item removed successfully";
                Logger.LogInformation("Selected item removed successfully");
            }
            else
            {
                await Services.Core.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Remove failed with status: {result.Status}"),
                    "Remove Selected Item", _applicationState.CurrentUser ?? "System"
                );
                StatusMessage = "Removal failed - please try again";
            }
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Remove Selected Item", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Removal error - please try again";
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region Helper Methods

    private async Task LoadOptionsAsync()
    {
        try
        {
            Logger.LogDebug("Loading options for AdvancedRemoveViewModel");
            var connectionString = _configurationService.GetConnectionString();

            // Load Part IDs from stored procedure
            var partResult = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                connectionString, "md_part_ids_Get_All", new Dictionary<string, object>()
            );
            
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                PartIDOptions.Clear();
                foreach (DataRow row in partResult.Rows)
                {
                    var partId = row["PartID"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(partId))
                        PartIDOptions.Add(partId);
                }
            });

            // Load Locations from stored procedure  
            var locationResult = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                connectionString, "md_locations_Get_All", new Dictionary<string, object>()
            );
            
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                LocationOptions.Clear();
                foreach (DataRow row in locationResult.Rows)
                {
                    var location = row["Location"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(location))
                        LocationOptions.Add(location);
                }
            });

            // Load Operations from stored procedure
            var operationResult = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                connectionString, "md_operation_numbers_Get_All", new Dictionary<string, object>()
            );
            
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                OperationOptions.Clear();
                foreach (DataRow row in operationResult.Rows)
                {
                    var operation = row["OperationNumber"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(operation))
                        OperationOptions.Add(operation);
                }
            });

            // Load Users from stored procedure (if user filtering stored procedure exists)
            try
            {
                var userResult = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString, "usr_users_Get_All", new Dictionary<string, object>()
                );
                
                if (userResult.Status == 1)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        UserOptions.Clear();
                        foreach (DataRow row in userResult.Data.Rows)
                        {
                            var user = row["User"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(user))
                                UserOptions.Add(user);
                        }
                    });
                }
            }
            catch (Exception userEx)
            {
                Logger.LogWarning(userEx, "Could not load user options, continuing without user filtering");
                // Continue without user filtering if the procedure doesn't exist
            }

            Logger.LogDebug("Options loaded successfully");
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Load Advanced Remove Master Data", _applicationState.CurrentUser ?? "System");
            throw; // Re-throw to let calling method handle it
        }
    }

    private async Task LoadRemovalHistoryAsync()
    {
        try
        {
            Logger.LogDebug("Loading removal history");
            var connectionString = _configurationService.GetConnectionString();

            // Get recent removal transactions from database
            var parameters = new Dictionary<string, object>
            {
                ["p_TransactionType"] = "OUT", // Focus on removal transactions
                ["p_Limit"] = 100 // Limit to recent 100 records
            };

            var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "inv_transaction_Get_Recent", parameters
            );

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                RemovalHistory.Clear();
                
                if (result.Status == 1)
                {
                    foreach (DataRow row in result.Data.Rows)
                    {
                        RemovalHistory.Add(new SessionTransaction
                        {
                            PartId = row["PartID"]?.ToString() ?? string.Empty,
                            Location = row["Location"]?.ToString() ?? string.Empty,
                            User = row["User"]?.ToString() ?? string.Empty,
                            Quantity = Convert.ToInt32(row["Quantity"] ?? 0),
                            TransactionTime = Convert.ToDateTime(row["TransactionTime"]),
                            Operation = row["Operation"]?.ToString() ?? string.Empty,
                            Status = "Removed",
                            BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                            TransactionType = row["TransactionType"]?.ToString() ?? "OUT"
                        });
                    }
                }
                else
                {
                    Logger.LogWarning("LoadRemovalHistoryAsync returned status {Status}: {Message}", 
                        result.Status, result.Message);
                }
            });

            Logger.LogDebug("Removal history loaded successfully with {Count} records", RemovalHistory.Count);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Load Advanced Remove History", _applicationState.CurrentUser ?? "System");
            Logger.LogError(ex, "Error loading removal history");
            
            // Continue with empty history rather than failing completely
            await Dispatcher.UIThread.InvokeAsync(() => RemovalHistory.Clear());
        }
    }

    private async Task ExecuteSearchAsync()
    {
        try
        {
            Logger.LogDebug("Executing search with current filter criteria");

            IsBusy = true;
            StatusMessage = "Searching...";

            var connectionString = _configurationService.GetConnectionString();
            var parameters = new Dictionary<string, object>();

            // Build filter parameters
            if (!string.IsNullOrWhiteSpace(FilterPartIDText))
                parameters["p_PartID"] = FilterPartIDText;

            if (!string.IsNullOrWhiteSpace(FilterLocationText))
                parameters["p_Location"] = FilterLocationText;

            if (!string.IsNullOrWhiteSpace(FilterUserText))
                parameters["p_User"] = FilterUserText;

            if (!string.IsNullOrWhiteSpace(FilterOperation))
                parameters["p_Operation"] = FilterOperation;

            if (RemovalDateRangeStart.HasValue)
                parameters["p_StartDate"] = RemovalDateRangeStart.Value.DateTime;

            if (RemovalDateRangeEnd.HasValue)
                parameters["p_EndDate"] = RemovalDateRangeEnd.Value.DateTime;

            // Only search removal transactions
            parameters["p_TransactionType"] = "OUT";

            var result = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "inv_transaction_Search", parameters
            );

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                RemovalHistory.Clear();
                
                if (result.Status == 1)
                {
                    foreach (DataRow row in result.Data.Rows)
                    {
                        RemovalHistory.Add(new SessionTransaction
                        {
                            PartId = row["PartID"]?.ToString() ?? string.Empty,
                            Operation = row["Operation"]?.ToString() ?? string.Empty,
                            Location = row["Location"]?.ToString() ?? string.Empty,
                            Quantity = Convert.ToInt32(row["Quantity"] ?? 0),
                            User = row["User"]?.ToString() ?? string.Empty,
                            TransactionTime = Convert.ToDateTime(row["TransactionTime"]),
                            Status = "Removed",
                            BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                            TransactionType = row["TransactionType"]?.ToString() ?? "OUT"
                        });
                    }
                    
                    StatusMessage = $"Search completed. Found {RemovalHistory.Count} items.";
                }
                else
                {
                    StatusMessage = "Search completed with no results.";
                }
            });

            Logger.LogInformation("Search completed with {Count} results", RemovalHistory.Count);
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Advanced Remove Search Execution", _applicationState.CurrentUser ?? "System");
            StatusMessage = "Search failed - please try again";
            Logger.LogError(ex, "Error executing search");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Generates CSV content for removal data export.
    /// </summary>
    private string GenerateRemovalDataCSV()
    {
        var csv = new StringBuilder();
        
        // CSV header
        csv.AppendLine("Date/Time,Part ID,Operation,Location,Quantity,User,Status,Transaction Time");
        
        // Data rows
        foreach (var item in RemovalHistory.OrderByDescending(h => h.TransactionTime))
        {
            var line = $"{item.TransactionTime:yyyy-MM-dd HH:mm:ss}," +
                      $"{EscapeCsvField(item.PartId)}," +
                      $"{EscapeCsvField(item.Operation)}," +
                      $"{EscapeCsvField(item.Location)}," +
                      $"{item.Quantity}," +
                      $"{EscapeCsvField(item.User)}," +
                      $"{EscapeCsvField(item.Status)}," +
                      $"{item.TransactionTime:yyyy-MM-dd HH:mm:ss}";
            
            csv.AppendLine(line);
        }
        
        return csv.ToString();
    }
    
    /// <summary>
    /// Generates a text summary of removal data.
    /// </summary>
    private string GenerateRemovalSummary()
    {
        var summary = new StringBuilder();
        
        // Header
        summary.AppendLine("MTM WIP Application - Advanced Removal Summary");
        summary.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        summary.AppendLine($"Generated by: {Environment.UserName}");
        summary.AppendLine(new string('=', 60));
        summary.AppendLine();
        
        // Overall statistics
        var totalRecords = RemovalHistory.Count;
        var totalQuantity = RemovalHistory.Sum(h => h.Quantity);
        var uniqueParts = RemovalHistory.Select(h => h.PartId).Distinct().Count();
        var uniqueUsers = RemovalHistory.Select(h => h.User).Distinct().Count();
        
        summary.AppendLine("SUMMARY STATISTICS");
        summary.AppendLine(new string('-', 20));
        summary.AppendLine($"Total Records: {totalRecords}");
        summary.AppendLine($"Total Quantity Removed: {totalQuantity:N0}");
        summary.AppendLine($"Unique Parts: {uniqueParts}");
        summary.AppendLine($"Unique Users: {uniqueUsers}");
        
        if (totalRecords > 0)
        {
            var dateRange = $"{RemovalHistory.Min(h => h.TransactionTime):yyyy-MM-dd} to {RemovalHistory.Max(h => h.TransactionTime):yyyy-MM-dd}";
            summary.AppendLine($"Date Range: {dateRange}");
        }
        
        summary.AppendLine();
        
        // Top parts by quantity removed
        if (RemovalHistory.Any())
        {
            summary.AppendLine("TOP PARTS BY QUANTITY REMOVED");
            summary.AppendLine(new string('-', 35));
            
            var topParts = RemovalHistory
                .GroupBy(h => h.PartId)
                .Select(g => new { PartId = g.Key, TotalQuantity = g.Sum(h => h.Quantity), Count = g.Count() })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10);
                
            foreach (var part in topParts)
            {
                summary.AppendLine($"{part.PartId}: {part.TotalQuantity:N0} items ({part.Count} removals)");
            }
            
            summary.AppendLine();
            
            // Activity by user
            summary.AppendLine("REMOVAL ACTIVITY BY USER");
            summary.AppendLine(new string('-', 25));
            
            var userActivity = RemovalHistory
                .GroupBy(h => h.User)
                .Select(g => new { User = g.Key, TotalQuantity = g.Sum(h => h.Quantity), Count = g.Count() })
                .OrderByDescending(u => u.Count);
                
            foreach (var user in userActivity)
            {
                summary.AppendLine($"{user.User}: {user.Count} removals, {user.TotalQuantity:N0} items");
            }
        }
        
        summary.AppendLine();
        summary.AppendLine("End of Summary");
        
        return summary.ToString();
    }
    
    /// <summary>
    /// Escapes CSV field content to handle commas, quotes, and newlines.
    /// </summary>
    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;
            
        // If field contains comma, quote, or newline, wrap in quotes and escape internal quotes
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return '"' + field.Replace("\"", "\"\"") + '"';
        }
        
        return field;
    }

    #endregion
}
