using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    public AdvancedRemoveViewModel(ILogger<AdvancedRemoveViewModel> logger) : base(logger)
    {
        try
        {
            Logger.LogInformation("Initializing AdvancedRemoveViewModel");
            
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
            StatusMessage = "Loading advanced removal options...";

            await LoadOptionsAsync().ConfigureAwait(false);
            await LoadRemovalHistoryAsync().ConfigureAwait(false);
            
            StatusMessage = "Advanced removal system ready";
            Logger.LogInformation("Advanced removal data loaded successfully");
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
    }

    /// <summary>
    /// Executes search with current filter criteria
    /// </summary>
    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            await ExecuteSearchAsync().ConfigureAwait(false);
            Logger.LogInformation("Search executed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing search");
            StatusMessage = $"Search error: {ex.Message}";
        }
    }

    /// <summary>
    /// Clears all filter criteria and resets to defaults
    /// </summary>
    [RelayCommand]
    private void Clear()
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
            Logger.LogError(ex, "Error clearing filters");
            StatusMessage = $"Clear error: {ex.Message}";
        }
    }

    /// <summary>
    /// Returns to normal inventory mode
    /// </summary>
    [RelayCommand]
    private void BackToNormal()
    {
        try
        {
            BackToNormalRequested?.Invoke(this, EventArgs.Empty);
            Logger.LogInformation("Back to normal command executed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in back to normal command");
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

            // Simulate bulk removal - replace with actual business logic
            await Task.Delay(1000).ConfigureAwait(false);

            // Move to last removed for undo capability
            foreach (var item in RemovalHistory.ToList())
            {
                LastRemovedItems.Add(item);
            }
            RemovalHistory.Clear();

            StatusMessage = $"Bulk removal completed successfully";
            Logger.LogInformation("Bulk removal completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in bulk remove");
            StatusMessage = $"Bulk removal error: {ex.Message}";
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
            await Task.Delay(400).ConfigureAwait(false); // TODO: Conditional removal logic
            Logger.LogInformation("Conditional removal executed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in conditional remove");
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
            await Task.Delay(600).ConfigureAwait(false); // TODO: Scheduled removal
            Logger.LogInformation("Scheduled removal executed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in scheduled remove");
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
            if (!CanUndo)
            {
                StatusMessage = "No items available to undo";
                return;
            }

            IsBusy = true;
            StatusMessage = "Undoing last removal...";

            // Simulate undo operation
            await Task.Delay(300).ConfigureAwait(false);

            var lastItem = LastRemovedItems.LastOrDefault();
            if (lastItem != null)
            {
                LastRemovedItems.Remove(lastItem);
                // Add back to main collection
                RemovalHistory.Add(lastItem);
                StatusMessage = $"Undid removal of {lastItem.PartId}";
                Logger.LogInformation("Removal undone for part {PartId}", lastItem.PartId);
            }
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
    }

    /// <summary>
    /// Views the removal history
    /// </summary>
    [RelayCommand]
    private async Task ViewHistoryAsync()
    {
        try
        {
            await Task.Delay(200).ConfigureAwait(false); // TODO: Show history dialog
            StatusMessage = "Viewing removal history";
            Logger.LogInformation("Viewing removal history");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error viewing history");
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
            await Task.Delay(800).ConfigureAwait(false); // TODO: Generate report
            StatusMessage = "Removal report generated";
            Logger.LogInformation("Removal report generated");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating report");
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
            Logger.LogError(ex, "Error exporting data");
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
            Logger.LogError(ex, "Error printing summary");
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
    private void ToggleFilterPanel()
    {
        try
        {
            IsFilterPanelExpanded = !IsFilterPanelExpanded;
            Logger.LogDebug("Filter panel toggled to {IsExpanded}", IsFilterPanelExpanded);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling filter panel");
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

            // Simulate removal
            await Task.Delay(400).ConfigureAwait(false);

            LastRemovedItems.Add(SelectedHistoryItem);
            RemovalHistory.Remove(SelectedHistoryItem);
            SelectedHistoryItem = null;

            StatusMessage = "Item removed successfully";
            Logger.LogInformation("Selected item removed successfully");
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
    }

    #endregion

    #region Helper Methods

    private async Task LoadOptionsAsync()
    {
        try
        {
            Logger.LogDebug("Loading options for AdvancedRemoveViewModel");

            // TODO: Load from database via stored procedures
            await Task.Delay(200).ConfigureAwait(false);

            // Update collections on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                LocationOptions.Clear();
                foreach (var loc in new[] { "WC01", "WC02", "WC03", "WC04", "WC05", "STOCK", "SHIP", "RECV" })
                    LocationOptions.Add(loc);

                PartIDOptions.Clear();
                foreach (var part in new[] { "24733444-PKG", "24677611", "24733405-PKG", "24733403-PKG", "24733491-PKG" })
                    PartIDOptions.Add(part);

                UserOptions.Clear();
                foreach (var user in new[] { "jbautista", "production", "admin", "supervisor" })
                    UserOptions.Add(user);

                OperationOptions.Clear();
                foreach (var operation in new[] { "10", "20", "30", "90", "100", "110", "120", "130" })
                    OperationOptions.Add(operation);
            });

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

            // TODO: Load from database via stored procedures
            await Task.Delay(300).ConfigureAwait(false);

            // Simulate loading removal history data
            Dispatcher.UIThread.Post(() =>
            {
                RemovalHistory.Clear();
                // Add sample data
                var sampleData = new[]
                {
                    new SessionTransaction 
                    { 
                        PartId = "24733444-PKG", 
                        Location = "WC01", 
                        User = "jbautista", 
                        Quantity = 5, 
                        TransactionTime = DateTime.Now.AddHours(-2),
                        Operation = "90",
                        Status = "Removed"
                    },
                    new SessionTransaction 
                    { 
                        PartId = "24677611", 
                        Location = "WC02", 
                        User = "production", 
                        Quantity = 10, 
                        TransactionTime = DateTime.Now.AddHours(-4),
                        Operation = "100",
                        Status = "Removed"
                    },
                    new SessionTransaction 
                    { 
                        PartId = "24733405-PKG", 
                        Location = "STOCK", 
                        User = "admin", 
                        Quantity = 3, 
                        TransactionTime = DateTime.Now.AddHours(-6),
                        Operation = "110",
                        Status = "Removed"
                    }
                };

                foreach (var item in sampleData)
                {
                    RemovalHistory.Add(item);
                }
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
            Logger.LogDebug("Executing search with current filter criteria");

            IsBusy = true;
            StatusMessage = "Searching...";

            // TODO: Implement actual search logic with database
            await Task.Delay(500).ConfigureAwait(false);

            // Update UI on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                // Simulate search results based on filters
                var filteredResults = RemovalHistory.Where(item =>
                    (string.IsNullOrEmpty(FilterLocationText) || item.Location.Contains(FilterLocationText, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(FilterPartIDText) || item.PartId.Contains(FilterPartIDText, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(FilterUserText) || item.User.Contains(FilterUserText, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                RemovalHistory.Clear();
                foreach (var item in filteredResults)
                {
                    RemovalHistory.Add(item);
                }
            });

            StatusMessage = $"Search completed. Found {RemovalHistory.Count} items.";
            Logger.LogInformation("Search completed with {Count} results", RemovalHistory.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing search");
            StatusMessage = "Search failed";
            throw;
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
