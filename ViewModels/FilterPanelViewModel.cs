using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for the FilterPanel control.
/// Phase 5 implementation providing advanced filtering and search functionality
/// with MVVM Community Toolkit patterns and proper error handling.
/// 
/// Features:
/// - Global search with debouncing
/// - Column-specific filters with type-appropriate controls
/// - Filter presets for common inventory scenarios
/// - Real-time filter statistics
/// - Configuration persistence and management
/// </summary>
public partial class FilterPanelViewModel : INotifyPropertyChanged
{
    #region Fields
    
    private readonly ILogger<FilterPanelViewModel> _logger;
    private readonly Timer _debounceTimer;
    private readonly SemaphoreSlim _filterSemaphore = new(1, 1);
    
    #endregion
    
    #region Properties
    
    /// <summary>
    /// Gets or sets the current filter configuration.
    /// </summary>
    public FilterConfiguration CurrentFilterConfiguration { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the global search text.
    /// </summary>
    public string GlobalSearchText { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets whether global search should be case-sensitive.
    /// </summary>
    public bool IsGlobalSearchCaseSensitive { get; set; }
    
    /// <summary>
    /// Gets the collection of column filters.
    /// </summary>
    public ObservableCollection<ColumnFilter> ColumnFilters { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the available filter presets.
    /// </summary>
    public ObservableCollection<FilterConfiguration> AvailablePresets { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the currently selected preset.
    /// </summary>
    public FilterConfiguration? SelectedPreset { get; set; }
    
    /// <summary>
    /// Gets or sets the current filter statistics.
    /// </summary>
    public FilterStatistics FilterStatistics { get; set; } = new();
    
    /// <summary>
    /// Gets or sets whether filter debouncing is enabled.
    /// </summary>
    public bool EnableFilterDebouncing { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the filter debounce delay in milliseconds.
    /// </summary>
    public int FilterDebounceDelayMs { get; set; } = 300;
    
    /// <summary>
    /// Gets or sets whether to auto-clear filters when data changes.
    /// </summary>
    public bool AutoClearFiltersOnDataChange { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to show filter tooltips.
    /// </summary>
    public bool ShowFilterTooltips { get; set; } = true;
    
    /// <summary>
    /// Gets whether there is global search text entered.
    /// </summary>
    public bool HasGlobalSearchText => !string.IsNullOrWhiteSpace(GlobalSearchText);
    
    /// <summary>
    /// Gets text describing the global search results.
    /// </summary>
    public string GlobalSearchResultText => HasGlobalSearchText 
        ? $"Searching for '{GlobalSearchText}' across all columns"
        : string.Empty;
    
    /// <summary>
    /// Gets text describing the number of active column filters.
    /// </summary>
    public string ActiveColumnFilterCountText
    {
        get
        {
            var activeCount = ColumnFilters.Count(f => f.IsActive && f.HasValidCriteria);
            return activeCount switch
            {
                0 => "No column filters active",
                1 => "1 column filter active",
                _ => $"{activeCount} column filters active"
            };
        }
    }
    
    /// <summary>
    /// Gets whether there are no filterable columns.
    /// </summary>
    public bool HasNoFilterableColumns => ColumnFilters.Count == 0;

    /// <summary>
    /// Property changed event for INotifyPropertyChanged.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    #endregion
    
    #region Events
    
    /// <summary>
    /// Event raised when filters change and data needs to be re-filtered.
    /// </summary>
    public event EventHandler<FilterChangedEventArgs>? FiltersChanged;
    
    /// <summary>
    /// Event raised when the filter panel should be closed.
    /// </summary>
    public event EventHandler? CloseRequested;
    
    #endregion
    
    #region Commands
    
    /// <summary>
    /// Command to clear all filters.
    /// </summary>
    [RelayCommand]
    private void ClearAllFilters()
    {
        try
        {
            _logger.LogDebug("Clearing all filters");
            
            GlobalSearchText = string.Empty;
            IsGlobalSearchCaseSensitive = false;
            
            foreach (var filter in ColumnFilters)
            {
                filter.Clear();
            }
            
            CurrentFilterConfiguration.ClearAllFilters();
            
            OnFiltersChanged();
            
            _logger.LogInformation("All filters cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all filters");
        }
    }
    
    /// <summary>
    /// Command to clear a specific column filter.
    /// </summary>
    [RelayCommand]
    private void ClearColumnFilter(ColumnFilter filter)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filter);
            
            _logger.LogDebug("Clearing filter for column: {ColumnName}", filter.DisplayName);
            
            filter.Clear();
            OnFiltersChanged();
            
            _logger.LogInformation("Cleared filter for column: {ColumnName}", filter.DisplayName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing column filter");
        }
    }
    
    /// <summary>
    /// Command to apply a selected preset.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanApplyPreset))]
    private async Task ApplyPresetAsync()
    {
        try
        {
            if (SelectedPreset == null) return;
            
            _logger.LogDebug("Applying preset: {PresetName}", SelectedPreset.DisplayName);
            
            // Apply the preset configuration
            await ApplyFilterConfigurationAsync(SelectedPreset);
            
            _logger.LogInformation("Applied preset: {PresetName}", SelectedPreset.DisplayName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying preset");
        }
    }
    
    private bool CanApplyPreset() => SelectedPreset != null;
    
    /// <summary>
    /// Command to save the current filter configuration as a new preset.
    /// </summary>
    [RelayCommand]
    private async Task SaveCurrentAsPresetAsync()
    {
        try
        {
            _logger.LogDebug("Saving current filters as preset");
            
            // In a full implementation, this would show a dialog to get the preset name
            var presetName = $"Custom Preset {DateTime.Now:yyyy-MM-dd HH:mm}";
            var preset = CreatePresetFromCurrentFilters(presetName);
            
            AvailablePresets.Add(preset);
            
            // In a full implementation, this would persist to storage
            await Task.Delay(50);
            
            _logger.LogInformation("Saved current filters as preset: {PresetName}", presetName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving current filters as preset");
        }
    }
    
    /// <summary>
    /// Command to close the filter panel.
    /// </summary>
    [RelayCommand]
    private void CloseFilterPanel()
    {
        try
        {
            _logger.LogDebug("Closing filter panel");
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing filter panel");
        }
    }
    
    #endregion
    
    #region Constructor
    
    /// <summary>
    /// Initializes a new instance of the FilterPanelViewModel.
    /// </summary>
    public FilterPanelViewModel(ILogger<FilterPanelViewModel> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        _logger = logger;
        _debounceTimer = new Timer(OnDebounceTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
        
        // Initialize preset filters
        InitializePresets();
        
        // Set up property change handlers
        PropertyChanged += OnPropertyChanged;
        
        _logger.LogDebug("FilterPanelViewModel initialized");
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Initializes the filter panel with columns from the data grid.
    /// </summary>
    public void InitializeFromColumns(ObservableCollection<CustomDataGridColumn> columns)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(columns);
            
            _logger.LogDebug("Initializing filters from {ColumnCount} columns", columns.Count);
            
            ColumnFilters.Clear();
            
            foreach (var column in columns.Where(c => c.CanFilter))
            {
                var filter = new ColumnFilter(column.PropertyName, column.DisplayName, column.DataType);
                
                // Set up property change notification for the filter
                filter.PropertyChanged += OnColumnFilterPropertyChanged;
                
                ColumnFilters.Add(filter);
            }
            
            CurrentFilterConfiguration.InitializeFromColumns(columns);
            
            OnPropertyChanged(nameof(HasNoFilterableColumns));
            OnPropertyChanged(nameof(ActiveColumnFilterCountText));
            
            _logger.LogInformation("Initialized {FilterCount} column filters", ColumnFilters.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing filters from columns");
        }
    }
    
    /// <summary>
    /// Updates filter statistics based on filtering results.
    /// </summary>
    public void UpdateFilterStatistics(int totalCount, int filteredCount)
    {
        try
        {
            FilterStatistics = new FilterStatistics
            {
                TotalCount = totalCount,
                FilteredCount = filteredCount,
                HasActiveFilters = HasActiveFilters()
            };
            
            _logger.LogDebug("Updated filter statistics: {FilteredCount}/{TotalCount} items visible", 
                filteredCount, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating filter statistics");
        }
    }
    
    /// <summary>
    /// Evaluates whether an item matches the current filter criteria.
    /// </summary>
    public bool MatchesFilters<T>(T item)
    {
        try
        {
            if (!HasActiveFilters()) return true;
            
            // Check global search
            if (!string.IsNullOrWhiteSpace(GlobalSearchText))
            {
                if (!MatchesGlobalSearch(item))
                    return false;
            }
            
            // Check column-specific filters
            foreach (var filter in ColumnFilters.Where(f => f.IsActive && f.HasValidCriteria))
            {
                var propertyInfo = typeof(T).GetProperty(filter.PropertyName);
                var value = propertyInfo?.GetValue(item);
                
                if (!filter.MatchesCriteria(value))
                    return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating filter match for item");
            return true; // Default to showing item on error
        }
    }
    
    #endregion
    
    #region Private Methods
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(GlobalSearchText) || e.PropertyName == nameof(IsGlobalSearchCaseSensitive))
            {
                if (EnableFilterDebouncing)
                {
                    RestartDebounceTimer();
                }
                else
                {
                    OnFiltersChanged();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling property change");
        }
    }
    
    private void OnColumnFilterPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(ColumnFilter.IsActive) ||
                e.PropertyName == nameof(ColumnFilter.FilterValue) ||
                e.PropertyName == nameof(ColumnFilter.FilterOperator) ||
                e.PropertyName == nameof(ColumnFilter.FilterValueTo))
            {
                OnPropertyChanged(nameof(ActiveColumnFilterCountText));
                
                if (EnableFilterDebouncing)
                {
                    RestartDebounceTimer();
                }
                else
                {
                    OnFiltersChanged();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling column filter property change");
        }
    }
    
    private void RestartDebounceTimer()
    {
        try
        {
            _debounceTimer.Change(FilterDebounceDelayMs, Timeout.Infinite);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restarting debounce timer");
            // Fall back to immediate update
            OnFiltersChanged();
        }
    }
    
    private void OnDebounceTimerElapsed(object? state)
    {
        try
        {
            // Timer callback runs on a background thread, marshal to UI thread if needed
            OnFiltersChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in debounce timer callback");
        }
    }
    
    private void OnFiltersChanged()
    {
        try
        {
            var args = new FilterChangedEventArgs
            {
                GlobalSearchText = GlobalSearchText,
                IsGlobalSearchCaseSensitive = IsGlobalSearchCaseSensitive,
                ColumnFilters = ColumnFilters.Where(f => f.IsActive && f.HasValidCriteria).ToList(),
                HasActiveFilters = HasActiveFilters()
            };
            
            FiltersChanged?.Invoke(this, args);
            
            _logger.LogDebug("Filters changed: Global={HasGlobal}, Columns={ColumnCount}", 
                !string.IsNullOrWhiteSpace(GlobalSearchText), args.ColumnFilters.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying filter changes");
        }
    }
    
    private bool HasActiveFilters()
    {
        return !string.IsNullOrWhiteSpace(GlobalSearchText) || 
               ColumnFilters.Any(f => f.IsActive && f.HasValidCriteria);
    }
    
    private bool MatchesGlobalSearch<T>(T item)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(GlobalSearchText))
                return true;
                
            var searchText = IsGlobalSearchCaseSensitive ? GlobalSearchText : GlobalSearchText.ToLowerInvariant();
            
            // Search in all string columns
            var searchableFilters = ColumnFilters.Where(f => f.DataType == typeof(string));
            
            foreach (var filter in searchableFilters)
            {
                var propertyInfo = typeof(T).GetProperty(filter.PropertyName);
                var value = propertyInfo?.GetValue(item)?.ToString();
                
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var valueToSearch = IsGlobalSearchCaseSensitive ? value : value.ToLowerInvariant();
                    if (valueToSearch.Contains(searchText))
                        return true;
                }
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating global search match");
            return true; // Default to match on error
        }
    }
    
    private void InitializePresets()
    {
        try
        {
            AvailablePresets.Clear();
            
            // Common inventory filtering presets
            AvailablePresets.Add(new FilterConfiguration("recent-activity", "Recent Activity")
            {
                Description = "Items updated in the last 7 days",
                IsPreset = true
            });
            
            AvailablePresets.Add(new FilterConfiguration("low-inventory", "Low Inventory")
            {
                Description = "Items with quantity below threshold",
                IsPreset = true
            });
            
            AvailablePresets.Add(new FilterConfiguration("operation-90", "Receiving (Operation 90)")
            {
                Description = "Items in receiving operation",
                IsPreset = true
            });
            
            AvailablePresets.Add(new FilterConfiguration("operation-100", "First Operation (100)")
            {
                Description = "Items in first manufacturing operation",
                IsPreset = true
            });
            
            AvailablePresets.Add(new FilterConfiguration("high-quantity", "High Quantity Items")
            {
                Description = "Items with quantity above 100",
                IsPreset = true
            });
            
            _logger.LogDebug("Initialized {PresetCount} filter presets", AvailablePresets.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing filter presets");
        }
    }
    
    private async Task ApplyFilterConfigurationAsync(FilterConfiguration configuration)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(configuration);
            
            // Clear existing filters
            ClearAllFilters();
            
            // Apply preset-specific logic
            switch (configuration.ConfigurationId)
            {
                case "recent-activity":
                    await ApplyRecentActivityFilterAsync();
                    break;
                    
                case "low-inventory":
                    await ApplyLowInventoryFilterAsync();
                    break;
                    
                case "operation-90":
                    await ApplyOperationFilterAsync("90");
                    break;
                    
                case "operation-100":
                    await ApplyOperationFilterAsync("100");
                    break;
                    
                case "high-quantity":
                    await ApplyHighQuantityFilterAsync();
                    break;
                    
                default:
                    // Apply custom configuration
                    GlobalSearchText = configuration.GlobalSearchText;
                    IsGlobalSearchCaseSensitive = configuration.IsGlobalSearchCaseSensitive;
                    break;
            }
            
            await Task.Delay(1); // Simulate async operation
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying filter configuration");
        }
    }
    
    private async Task ApplyRecentActivityFilterAsync()
    {
        var lastUpdatedFilter = ColumnFilters.FirstOrDefault(f => f.PropertyName == "LastUpdated");
        if (lastUpdatedFilter != null)
        {
            lastUpdatedFilter.IsActive = true;
            lastUpdatedFilter.FilterOperator = FilterOperator.GreaterThanOrEqual;
            lastUpdatedFilter.FilterValue = DateTime.Now.AddDays(-7);
        }
        
        await Task.Delay(1);
    }
    
    private async Task ApplyLowInventoryFilterAsync()
    {
        var quantityFilter = ColumnFilters.FirstOrDefault(f => f.PropertyName == "Quantity");
        if (quantityFilter != null)
        {
            quantityFilter.IsActive = true;
            quantityFilter.FilterOperator = FilterOperator.LessThanOrEqual;
            quantityFilter.FilterValue = 10;
        }
        
        await Task.Delay(1);
    }
    
    private async Task ApplyOperationFilterAsync(string operation)
    {
        var operationFilter = ColumnFilters.FirstOrDefault(f => f.PropertyName == "Operation");
        if (operationFilter != null)
        {
            operationFilter.IsActive = true;
            operationFilter.FilterOperator = FilterOperator.Equals;
            operationFilter.FilterValue = operation;
        }
        
        await Task.Delay(1);
    }
    
    private async Task ApplyHighQuantityFilterAsync()
    {
        var quantityFilter = ColumnFilters.FirstOrDefault(f => f.PropertyName == "Quantity");
        if (quantityFilter != null)
        {
            quantityFilter.IsActive = true;
            quantityFilter.FilterOperator = FilterOperator.GreaterThan;
            quantityFilter.FilterValue = 100;
        }
        
        await Task.Delay(1);
    }
    
    private FilterConfiguration CreatePresetFromCurrentFilters(string name)
    {
        var preset = new FilterConfiguration(Guid.NewGuid().ToString(), name)
        {
            GlobalSearchText = GlobalSearchText,
            IsGlobalSearchCaseSensitive = IsGlobalSearchCaseSensitive,
            IsPreset = false,
            Description = "Custom filter configuration"
        };
        
        // Copy active column filters
        foreach (var filter in ColumnFilters.Where(f => f.IsActive && f.HasValidCriteria))
        {
            preset.ColumnFilters.Add(new ColumnFilter(filter.PropertyName, filter.DisplayName, filter.DataType)
            {
                IsActive = filter.IsActive,
                FilterOperator = filter.FilterOperator,
                FilterValue = filter.FilterValue,
                FilterValueTo = filter.FilterValueTo,
                IsCaseSensitive = filter.IsCaseSensitive
            });
        }
        
        return preset;
    }
    
    #endregion
    
    #region Disposal
    
    /// <summary>
    /// Disposes of resources used by the FilterPanelViewModel.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _debounceTimer?.Dispose();
            _filterSemaphore?.Dispose();
            
            // Unsubscribe from events
            foreach (var filter in ColumnFilters)
            {
                filter.PropertyChanged -= OnColumnFilterPropertyChanged;
            }
        }
    }
    
    /// <summary>
    /// Disposes of the FilterPanelViewModel.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    #endregion
}

/// <summary>
/// Event arguments for filter change notifications.
/// </summary>
public class FilterChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the global search text.
    /// </summary>
    public string GlobalSearchText { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets whether global search is case-sensitive.
    /// </summary>
    public bool IsGlobalSearchCaseSensitive { get; set; }
    
    /// <summary>
    /// Gets or sets the active column filters.
    /// </summary>
    public List<ColumnFilter> ColumnFilters { get; set; } = new();
    
    /// <summary>
    /// Gets or sets whether there are any active filters.
    /// </summary>
    public bool HasActiveFilters { get; set; }
}