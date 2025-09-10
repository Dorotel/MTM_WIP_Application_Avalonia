using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Demo control for showcasing the CustomDataGrid functionality.
/// Provides sample inventory data to demonstrate the grid's capabilities
/// with proper MTM theme integration and data binding.
/// </summary>
public partial class CustomDataGridDemo : UserControl
{
    #region Properties

    private ObservableCollection<DemoInventoryItem> _sampleInventoryData = new();
    /// <summary>
    /// Sample inventory data for demonstration purposes.
    /// </summary>
    public ObservableCollection<DemoInventoryItem> SampleInventoryData
    {
        get => _sampleInventoryData;
        set 
        { 
            _sampleInventoryData = value;
            OnPropertyChanged(nameof(SampleInventoryData));
            OnPropertyChanged(nameof(ItemCount));
        }
    }

    private DemoInventoryItem? _selectedInventoryItem;
    /// <summary>
    /// Currently selected inventory item.
    /// </summary>
    public DemoInventoryItem? SelectedInventoryItem
    {
        get => _selectedInventoryItem;
        set 
        { 
            _selectedInventoryItem = value;
            OnPropertyChanged(nameof(SelectedInventoryItem));
        }
    }

    private ObservableCollection<object> _selectedItems = new();
    /// <summary>
    /// Selected items for multi-selection demonstration.
    /// </summary>
    public ObservableCollection<object> SelectedItems
    {
        get => _selectedItems;
        set 
        { 
            _selectedItems = value;
            OnPropertyChanged(nameof(SelectedItems));
            OnPropertyChanged(nameof(SelectedItemCount));
        OnPropertyChanged(nameof(HasSelectedItems));
        }
    }

    private bool _isMultiSelectEnabled = false;
    /// <summary>
    /// Gets or sets whether multi-selection is enabled for demonstration.
    /// </summary>
    public bool IsMultiSelectEnabled
    {
        get => _isMultiSelectEnabled;
        set 
        { 
            _isMultiSelectEnabled = value;
            OnPropertyChanged(nameof(IsMultiSelectEnabled));
        }
    }

    private bool _isColumnManagementVisible = false;
    /// <summary>
    /// Gets or sets whether the column management panel is visible.
    /// Phase 3 feature for advanced column management.
    /// </summary>
    public bool IsColumnManagementVisible
    {
        get => _isColumnManagementVisible;
        set 
        { 
            _isColumnManagementVisible = value;
            OnPropertyChanged(nameof(IsColumnManagementVisible));
        }
    }

    /// <summary>
    /// Gets the count of selected items.
    /// </summary>
    public int SelectedItemCount => SelectedItems.Count;

    /// <summary>
    /// Gets the count of items in the sample data.
    /// </summary>
    public int ItemCount => SampleInventoryData.Count;

    /// <summary>
    /// Gets the count of visible columns in the grid.
    /// Phase 3 feature for column management feedback.
    /// </summary>
    public int VisibleColumnCount
    {
        get
        {
            var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
            if (demoDataGrid?.Columns != null)
            {
                return demoDataGrid.Columns.Count(c => c.IsVisible);
            }
            return 6; // Default column count
        }
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to delete an item from the grid.
    /// </summary>
    public ICommand DeleteItemCommand => new RelayCommand<object>(DeleteItem);

    /// <summary>
    /// Command to edit an item in the grid.
    /// </summary>
    public ICommand EditItemCommand => new RelayCommand<object>(EditItem);

    /// <summary>
    /// Command to duplicate an item in the grid.
    /// </summary>
    public ICommand DuplicateItemCommand => new RelayCommand<object>(DuplicateItem);

    /// <summary>
    /// Command to view details of an item.
    /// </summary>
    public ICommand ViewDetailsCommand => new RelayCommand<object>(ViewDetails);

    /// <summary>
    /// Command to toggle multi-selection mode.
    /// </summary>
    public ICommand ToggleMultiSelectCommand => new RelayCommand(ToggleMultiSelect);

    /// <summary>
    /// Command to toggle column visibility.
    /// Phase 3 feature for interactive column management.
    /// </summary>
    public ICommand ToggleColumnVisibilityCommand => new RelayCommand<string>(ToggleColumnVisibility);

    /// <summary>
    /// Command to reset all columns to default layout.
    /// Phase 3 feature for column management.
    /// </summary>
    public ICommand ResetColumnsCommand => new RelayCommand(ResetColumns);

    /// <summary>
    /// Command to save current column layout.
    /// Phase 3 feature for configuration persistence.
    /// </summary>
    public ICommand SaveLayoutCommand => new RelayCommand(SaveLayout);

    /// <summary>
    /// Command to load a saved configuration.
    /// Phase 4 feature for configuration management.
    /// </summary>
    public ICommand LoadConfigurationCommand => new RelayCommand<string>(LoadConfiguration);

    /// <summary>
    /// Command to delete a saved configuration.
    /// Phase 4 feature for configuration management.
    /// </summary>
    public ICommand DeleteConfigurationCommand => new RelayCommand<string>(DeleteConfiguration);

    /// <summary>
    /// Gets the collection of available saved configurations for the demo.
    /// Phase 4 feature for configuration management.
    /// </summary>
    public ObservableCollection<string> SavedConfigurationNames { get; private set; } = new();

    /// <summary>
    /// Gets or sets whether interactive resize handles are enabled.
    /// Phase 4 feature for column resizing.
    /// </summary>
    public bool IsInteractiveResizeEnabled { get; set; } = true;

    private string? _selectedConfigurationName;
    /// <summary>
    /// Gets or sets the currently selected configuration name.
    /// Phase 4 feature for configuration management.
    /// </summary>
    public string? SelectedConfigurationName
    {
        get => _selectedConfigurationName;
        set 
        { 
            _selectedConfigurationName = value;
            OnPropertyChanged(nameof(SelectedConfigurationName));
        }
    }

    /// <summary>
    /// Gets or sets whether the filter panel is visible.
    /// Phase 5 feature for advanced filtering.
    /// </summary>
    private bool _isFilterPanelVisible;
    public bool IsFilterPanelVisible
    {
        get => _isFilterPanelVisible;
        set 
        { 
            _isFilterPanelVisible = value;
            OnPropertyChanged(nameof(IsFilterPanelVisible));
        }
    }

    /// <summary>
    /// Gets or sets the current filter statistics text.
    /// Phase 5 feature for filter result feedback.
    /// </summary>
    private string _filterStatsText = "No filters applied";
    public string FilterStatsText
    {
        get => _filterStatsText;
        set 
        { 
            _filterStatsText = value;
            OnPropertyChanged(nameof(FilterStatsText));
        }
    }

    /// <summary>
    /// Gets whether there are any selected items.
    /// Phase 6 feature for bulk operations.
    /// </summary>
    public bool HasSelectedItems => SelectedItems.Count > 0;

    /// <summary>
    /// Gets or sets the current data analytics text.
    /// Phase 6 feature for analytics feedback.
    /// </summary>
    private string _dataAnalyticsText = "No analytics run";
    public string DataAnalyticsText
    {
        get => _dataAnalyticsText;
        set 
        { 
            _dataAnalyticsText = value;
            OnPropertyChanged(nameof(DataAnalyticsText));
        }
    }

    /// <summary>
    /// Gets or sets the last operation status text.
    /// Phase 6 feature for operation feedback.
    /// </summary>
    private string _lastOperationText = "Ready";
    public string LastOperationText
    {
        get => _lastOperationText;
        set 
        { 
            _lastOperationText = value;
            OnPropertyChanged(nameof(LastOperationText));
        }
    }

    #endregion

    #region Constructor

    public CustomDataGridDemo()
    {
        InitializeComponent();
        DataContext = this;
        
        // Initialize saved configuration names for Phase 4 demo
        foreach (var configName in new[]
        {
            "Default Layout",
            "Compact View", 
            "Detailed View",
            "Essential Only"
        })
        {
            SavedConfigurationNames.Add(configName);
        }
        
        // Load initial sample data
        LoadSampleData();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the Load Sample Data button click.
    /// </summary>
    private void OnLoadSampleDataClick(object? sender, RoutedEventArgs e)
    {
        LoadSampleData();
    }

    /// <summary>
    /// Handles the Clear Data button click.
    /// </summary>
    private void OnClearDataClick(object? sender, RoutedEventArgs e)
    {
        SampleInventoryData.Clear();
        SelectedItems.Clear();
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(SelectedItemCount));
        OnPropertyChanged(nameof(HasSelectedItems));
    }

    /// <summary>
    /// Handles the Reset Columns button click.
    /// Phase 3 feature for resetting column layout.
    /// </summary>
    private void OnResetColumnsClick(object? sender, RoutedEventArgs e)
    {
        ResetColumns();
    }

    /// <summary>
    /// Handles the Save Layout button click.
    /// Phase 3 feature for saving column configuration.
    /// </summary>
    private void OnSaveLayoutClick(object? sender, RoutedEventArgs e)
    {
        SaveLayout();
    }

    /// <summary>
    /// Handles the Auto Size All button click.
    /// Phase 3 feature for auto-sizing columns.
    /// </summary>
    private void OnAutoSizeAllClick(object? sender, RoutedEventArgs e)
    {
        AutoSizeAllColumns();
    }

    /// <summary>
    /// Handles the Clear Filters button click.
    /// Phase 5 feature for clearing all active filters.
    /// </summary>
    private void OnClearFiltersClick(object? sender, RoutedEventArgs e)
    {
        ClearAllFilters();
    }

    /// <summary>
    /// Handles the Apply Low Stock Filter button click.
    /// Phase 5 feature for applying predefined filter scenarios.
    /// </summary>
    private void OnApplyLowStockFilterClick(object? sender, RoutedEventArgs e)
    {
        ApplyLowStockFilter();
    }

    /// <summary>
    /// Handles the Apply Recent Activity Filter button click.
    /// Phase 5 feature for applying predefined filter scenarios.
    /// </summary>
    private void OnApplyRecentActivityFilterClick(object? sender, RoutedEventArgs e)
    {
        ApplyRecentActivityFilter();
    }

    /// <summary>
    /// Handles the Import CSV button click.
    /// Phase 6 feature for data import functionality.
    /// </summary>
    private void OnImportCsvClick(object? sender, RoutedEventArgs e)
    {
        ImportCsvData();
    }

    /// <summary>
    /// Handles the Export CSV button click.
    /// Phase 6 feature for data export functionality.
    /// </summary>
    private void OnExportCsvClick(object? sender, RoutedEventArgs e)
    {
        ExportCsvData();
    }

    /// <summary>
    /// Handles the Export Excel button click.
    /// Phase 6 feature for Excel export functionality.
    /// </summary>
    private void OnExportExcelClick(object? sender, RoutedEventArgs e)
    {
        ExportExcelData();
    }

    /// <summary>
    /// Handles the Bulk Update button click.
    /// Phase 6 feature for batch operations.
    /// </summary>
    private void OnBulkUpdateClick(object? sender, RoutedEventArgs e)
    {
        BulkUpdateSelectedItems();
    }

    /// <summary>
    /// Handles the Bulk Delete button click.
    /// Phase 6 feature for batch operations.
    /// </summary>
    private void OnBulkDeleteClick(object? sender, RoutedEventArgs e)
    {
        BulkDeleteSelectedItems();
    }

    /// <summary>
    /// Handles the Data Analytics button click.
    /// Phase 6 feature for data analysis and insights.
    /// </summary>
    private void OnDataAnalyticsClick(object? sender, RoutedEventArgs e)
    {
        ShowDataAnalytics();
    }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Deletes the specified item from the grid.
    /// </summary>
    private void DeleteItem(object? parameter)
    {
        if (parameter is DemoInventoryItem item && SampleInventoryData.Contains(item))
        {
            SampleInventoryData.Remove(item);
            SelectedItems.Remove(item);
            OnPropertyChanged(nameof(ItemCount));
            OnPropertyChanged(nameof(SelectedItemCount));
        OnPropertyChanged(nameof(HasSelectedItems));
        }
    }

    /// <summary>
    /// Edits the specified item (demo implementation shows details).
    /// </summary>
    private void EditItem(object? parameter)
    {
        if (parameter is DemoInventoryItem item)
        {
            // For demo purposes, simulate edit action by updating notes
            item.Notes = $"{item.Notes} [Edited at {DateTime.Now:HH:mm:ss}]";
            OnPropertyChanged(nameof(SampleInventoryData));
        }
    }

    /// <summary>
    /// Duplicates the specified item.
    /// </summary>
    private void DuplicateItem(object? parameter)
    {
        if (parameter is DemoInventoryItem item)
        {
            var duplicate = DemoInventoryItem.CreateDemo(
                $"{item.PartId}-COPY",
                item.Operation,
                item.Location,
                item.Quantity,
                $"Copy of {item.PartId} - {item.Notes}"
            );
            
            SampleInventoryData.Add(duplicate);
            OnPropertyChanged(nameof(ItemCount));
        }
    }

    /// <summary>
    /// Views details of the specified item.
    /// </summary>
    private void ViewDetails(object? parameter)
    {
        if (parameter is DemoInventoryItem item)
        {
            // For demo purposes, select the item to show it's been viewed
            SelectedInventoryItem = item;
            
            // In a real implementation, this would open a details view or dialog
            System.Diagnostics.Debug.WriteLine($"Viewing details for: {item.PartId} - {item.DisplayText}");
        }
    }

    /// <summary>
    /// Toggles multi-selection mode on and off.
    /// </summary>
    private void ToggleMultiSelect()
    {
        IsMultiSelectEnabled = !IsMultiSelectEnabled;
        if (!IsMultiSelectEnabled)
        {
            SelectedItems.Clear();
            OnPropertyChanged(nameof(SelectedItemCount));
        OnPropertyChanged(nameof(HasSelectedItems));
        }
    }

    /// <summary>
    /// Toggles the visibility of a specific column.
    /// Phase 3 feature for interactive column management.
    /// </summary>
    private void ToggleColumnVisibility(string? columnName)
    {
        if (string.IsNullOrEmpty(columnName))
            return;

        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.Columns == null)
            return;

        var column = demoDataGrid.Columns.FirstOrDefault(c => c.PropertyName == columnName);
        if (column != null)
        {
            column.IsVisible = !column.IsVisible;
            OnPropertyChanged(nameof(VisibleColumnCount));
            System.Diagnostics.Debug.WriteLine($"Toggled column visibility: {columnName} = {column.IsVisible}");
        }
    }

    /// <summary>
    /// Resets all columns to their default layout and visibility.
    /// Phase 3 feature for column management.
    /// </summary>
    private void ResetColumns()
    {
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.Columns == null)
            return;

        // Reset all columns to visible and default widths
        foreach (var column in demoDataGrid.Columns)
        {
            column.IsVisible = true;
            column.Width = double.NaN; // Auto-size
            column.DisplayOrder = demoDataGrid.Columns.IndexOf(column);
        }

        OnPropertyChanged(nameof(VisibleColumnCount));
        System.Diagnostics.Debug.WriteLine("All columns reset to default layout");
    }

    /// <summary>
    /// Saves the current column layout configuration.
    /// Phase 3 feature for configuration persistence.
    /// </summary>
    private void SaveLayout()
    {
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.Columns == null)
            return;

        var config = ColumnConfiguration.FromColumns(demoDataGrid.Columns, "Demo Layout");
        
        // In a real implementation, this would save to a service or file
        System.Diagnostics.Debug.WriteLine($"Saved column configuration: {config.DisplayName}");
        System.Diagnostics.Debug.WriteLine($"Configuration has {config.ColumnSettings.Count} columns");
        
        foreach (var setting in config.ColumnSettings.OrderBy(s => s.Order))
        {
            System.Diagnostics.Debug.WriteLine($"  {setting.DisplayName}: Visible={setting.IsVisible}, Width={setting.Width:F0}px, Order={setting.Order}");
        }
    }

    /// <summary>
    /// Auto-sizes all columns to fit their content.
    /// Phase 3 feature for column width management.
    /// </summary>
    private void AutoSizeAllColumns()
    {
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.Columns == null)
            return;

        foreach (var column in demoDataGrid.Columns.Where(c => c.CanResize))
        {
            column.Width = double.NaN; // Set to auto-size
        }

        System.Diagnostics.Debug.WriteLine("All columns set to auto-size");
    }

    /// <summary>
    /// Loads a saved configuration by name.
    /// Phase 4 feature for enhanced configuration management.
    /// </summary>
    private void LoadConfiguration(string? configurationName)
    {
        if (string.IsNullOrEmpty(configurationName))
            return;

        // In a full implementation, this would load from ColumnConfigurationService
        System.Diagnostics.Debug.WriteLine($"Loading configuration: {configurationName}");

        // For demo purposes, simulate different configurations
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.Columns == null) return;

        switch (configurationName)
        {
            case "Compact View":
                ApplyCompactConfiguration(demoDataGrid.Columns);
                break;
            case "Detailed View":
                ApplyDetailedConfiguration(demoDataGrid.Columns);
                break;
            case "Essential Only":
                ApplyEssentialConfiguration(demoDataGrid.Columns);
                break;
            default:
                ResetColumns();
                break;
        }

        OnPropertyChanged(nameof(VisibleColumnCount));
    }

    /// <summary>
    /// Deletes a saved configuration by name.
    /// Phase 4 feature for configuration management.
    /// </summary>
    private void DeleteConfiguration(string? configurationName)
    {
        if (string.IsNullOrEmpty(configurationName))
            return;

        // In a full implementation, this would delete from ColumnConfigurationService
        SavedConfigurationNames.Remove(configurationName);
        System.Diagnostics.Debug.WriteLine($"Deleted configuration: {configurationName}");
    }

    /// <summary>
    /// Clears all active filters.
    /// Phase 5 feature for filter management.
    /// </summary>
    private void ClearAllFilters()
    {
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.FilterConfiguration != null)
        {
            demoDataGrid.FilterConfiguration.ClearAllFilters();
            FilterStatsText = "All filters cleared";
            System.Diagnostics.Debug.WriteLine("All filters cleared");
        }
    }

    /// <summary>
    /// Applies a low stock filter (quantity <= 10).
    /// Phase 5 feature for preset filter scenarios.
    /// </summary>
    private void ApplyLowStockFilter()
    {
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.FilterConfiguration != null)
        {
            var config = demoDataGrid.FilterConfiguration;
            config.ClearAllFilters();
            
            var quantityFilter = config.GetColumnFilter("Quantity");
            if (quantityFilter != null)
            {
                quantityFilter.IsActive = true;
                quantityFilter.FilterOperator = FilterOperator.LessThanOrEqual;
                quantityFilter.FilterValue = 10;
            }
            
            FilterStatsText = "Low stock filter applied (quantity <= 10)";
            System.Diagnostics.Debug.WriteLine("Applied low stock filter");
        }
    }

    /// <summary>
    /// Applies a recent activity filter (last 7 days).
    /// Phase 5 feature for preset filter scenarios.
    /// </summary>
    private void ApplyRecentActivityFilter()
    {
        var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
        if (demoDataGrid?.FilterConfiguration != null)
        {
            var config = demoDataGrid.FilterConfiguration;
            config.ClearAllFilters();
            
            var dateFilter = config.GetColumnFilter("LastUpdated");
            if (dateFilter != null)
            {
                dateFilter.IsActive = true;
                dateFilter.FilterOperator = FilterOperator.GreaterThanOrEqual;
                dateFilter.FilterValue = DateTime.Now.AddDays(-7);
            }
            
            FilterStatsText = "Recent activity filter applied (last 7 days)";
            System.Diagnostics.Debug.WriteLine("Applied recent activity filter");
        }
    }

    /// <summary>
    /// Applies a compact view configuration for smaller screens.
    /// </summary>
    private void ApplyCompactConfiguration(ObservableCollection<CustomDataGridColumn> columns)
    {
        foreach (var column in columns)
        {
            column.IsVisible = true;
            switch (column.PropertyName)
            {
                case "PartId":
                    column.Width = 80;
                    break;
                case "Operation":
                    column.Width = 60;
                    break;
                case "Location":
                    column.Width = 70;
                    break;
                case "Quantity":
                    column.Width = 60;
                    break;
                case "LastUpdated":
                    column.Width = 100;
                    break;
                case "Notes":
                    column.IsVisible = false; // Hide in compact view
                    break;
            }
        }
        System.Diagnostics.Debug.WriteLine("Applied compact view configuration");
    }

    /// <summary>
    /// Applies a detailed view configuration with all columns visible and wide.
    /// </summary>
    private void ApplyDetailedConfiguration(ObservableCollection<CustomDataGridColumn> columns)
    {
        foreach (var column in columns)
        {
            column.IsVisible = true;
            switch (column.PropertyName)
            {
                case "PartId":
                    column.Width = 140;
                    break;
                case "Operation":
                    column.Width = 100;
                    break;
                case "Location":
                    column.Width = 120;
                    break;
                case "Quantity":
                    column.Width = 100;
                    break;
                case "LastUpdated":
                    column.Width = 160;
                    break;
                case "Notes":
                    column.Width = 250;
                    break;
            }
        }
        System.Diagnostics.Debug.WriteLine("Applied detailed view configuration");
    }

    /// <summary>
    /// Applies an essential-only configuration showing just the key fields.
    /// </summary>
    private void ApplyEssentialConfiguration(ObservableCollection<CustomDataGridColumn> columns)
    {
        foreach (var column in columns)
        {
            switch (column.PropertyName)
            {
                case "PartId":
                case "Operation":
                case "Quantity":
                    column.IsVisible = true;
                    column.Width = double.NaN; // Auto-size
                    break;
                default:
                    column.IsVisible = false;
                    break;
            }
        }
        System.Diagnostics.Debug.WriteLine("Applied essential-only configuration");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads sample inventory data for demonstration.
    /// Creates realistic MTM manufacturing data with various scenarios.
    /// </summary>
    private void LoadSampleData()
    {
        SampleInventoryData.Clear();

        var sampleData = new[]
        {
            DemoInventoryItem.CreateDemo("MTM-001", "90", "RECV-A1", 150, "Recently received - ready for processing"),
            DemoInventoryItem.CreateDemo("ABC-123", "100", "WIP-B2", 45, "First operation in progress"),
            DemoInventoryItem.CreateDemo("XYZ-789", "110", "WIP-C3", 28, "Second operation - quality check needed"),
            DemoInventoryItem.CreateDemo("DEF-456", "120", "FINAL-D1", 75, "Final operation - ready for shipping"),
            DemoInventoryItem.CreateDemo("GHI-101", "90", "RECV-A2", 200, "Large batch received - high priority"),
            DemoInventoryItem.CreateDemo("JKL-202", "100", "WIP-B3", 60, "Standard processing"),
            DemoInventoryItem.CreateDemo("MNO-303", "110", "WIP-C1", 35, "Quality testing in progress"),
            DemoInventoryItem.CreateDemo("PQR-404", "120", "FINAL-D2", 90, "Final inspection complete"),
            DemoInventoryItem.CreateDemo("STU-505", "90", "RECV-A3", 180, "Urgent order - expedite processing"),
            DemoInventoryItem.CreateDemo("VWX-606", "100", "WIP-B1", 25, "Small batch - special handling required")
        };

        foreach (var item in sampleData)
        {
            SampleInventoryData.Add(item);
        }

        // Clear selection when loading new data
        SelectedItems.Clear();
        
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(SelectedItemCount));
        OnPropertyChanged(nameof(HasSelectedItems));
    }

    #region Phase 6 Implementation

    /// <summary>
    /// Imports data from a CSV file.
    /// Phase 6 feature for data import functionality.
    /// </summary>
    private async void ImportCsvData()
    {
        try
        {
            LastOperationText = "Importing CSV data...";
            
            // For demo purposes, simulate CSV import
            var tempFilePath = "/tmp/demo_import.csv";
            await CreateSampleCsvFileAsync(tempFilePath);
            
            var service = new CustomDataGridService(null!);
            var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
            
            if (demoDataGrid?.Columns != null)
            {
                var result = await service.ImportFromCsvAsync<DemoInventoryItem>(tempFilePath, demoDataGrid.Columns);
                
                if (result.IsSuccess)
                {
                    foreach (var item in result.ImportedData)
                    {
                        SampleInventoryData.Add(item);
                    }
                    
                    LastOperationText = $"Imported {result.ImportedData.Count} items from CSV";
                    OnPropertyChanged(nameof(ItemCount));
                }
                else
                {
                    LastOperationText = $"Import failed: {string.Join(", ", result.Errors)}";
                }
            }
            
            // Clean up temp file
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
        catch (Exception ex)
        {
            LastOperationText = $"Import error: {ex.Message}";
        }
    }

    /// <summary>
    /// Exports data to a CSV file.
    /// Phase 6 feature for data export functionality.
    /// </summary>
    private async void ExportCsvData()
    {
        try
        {
            LastOperationText = "Exporting to CSV...";
            
            var service = new CustomDataGridService(null!);
            var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
            var exportPath = $"/tmp/demo_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            
            if (demoDataGrid?.Columns != null)
            {
                var success = await service.ExportDataAsync(SampleInventoryData, demoDataGrid.Columns, exportPath, "csv");
                
                if (success)
                {
                    LastOperationText = $"Exported {SampleInventoryData.Count} items to {Path.GetFileName(exportPath)}";
                }
                else
                {
                    LastOperationText = "Export failed";
                }
            }
        }
        catch (Exception ex)
        {
            LastOperationText = $"Export error: {ex.Message}";
        }
    }

    /// <summary>
    /// Exports data to an Excel file.
    /// Phase 6 feature for Excel export functionality.
    /// </summary>
    private async void ExportExcelData()
    {
        try
        {
            LastOperationText = "Exporting to Excel...";
            
            var service = new CustomDataGridService(null!);
            var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
            var exportPath = $"/tmp/demo_export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            if (demoDataGrid?.Columns != null)
            {
                var success = await service.ExportDataAsync(SampleInventoryData, demoDataGrid.Columns, exportPath, "excel");
                
                if (success)
                {
                    LastOperationText = $"Exported {SampleInventoryData.Count} items to {Path.GetFileName(exportPath)}";
                }
                else
                {
                    LastOperationText = "Excel export not yet fully implemented - use CSV format";
                }
            }
        }
        catch (Exception ex)
        {
            LastOperationText = $"Excel export error: {ex.Message}";
        }
    }

    /// <summary>
    /// Performs bulk update on selected items.
    /// Phase 6 feature for batch operations.
    /// </summary>
    private async void BulkUpdateSelectedItems()
    {
        try
        {
            if (SelectedItems.Count == 0)
            {
                LastOperationText = "No items selected for bulk update";
                return;
            }
            
            LastOperationText = $"Bulk updating {SelectedItems.Count} items...";
            
            var service = new CustomDataGridService(null!);
            var selectedInventoryItems = SelectedItems.Cast<DemoInventoryItem>();
            
            // Example bulk update - increase quantity by 10
            var updates = new Dictionary<string, object>
            {
                { "Quantity", 0 } // Will be updated per item
            };
            
            var updatedItems = new List<DemoInventoryItem>();
            foreach (var item in selectedInventoryItems)
            {
                updates["Quantity"] = item.Quantity + 10;
                var result = await service.BulkUpdateAsync(new[] { item }, updates);
                
                if (result.IsSuccess)
                {
                    updatedItems.AddRange(result.SuccessfulItems);
                }
            }
            
            LastOperationText = $"Bulk updated {updatedItems.Count} items (added 10 to quantity)";
            OnPropertyChanged(nameof(SampleInventoryData)); // Trigger UI refresh
        }
        catch (Exception ex)
        {
            LastOperationText = $"Bulk update error: {ex.Message}";
        }
    }

    /// <summary>
    /// Performs bulk delete on selected items.
    /// Phase 6 feature for batch operations.
    /// </summary>
    private async void BulkDeleteSelectedItems()
    {
        try
        {
            if (SelectedItems.Count == 0)
            {
                LastOperationText = "No items selected for bulk delete";
                return;
            }
            
            LastOperationText = $"Bulk deleting {SelectedItems.Count} items...";
            
            var service = new CustomDataGridService(null!);
            var selectedInventoryItems = SelectedItems.Cast<DemoInventoryItem>().ToList();
            
            var result = await service.BulkDeleteAsync(selectedInventoryItems);
            
            if (result.IsSuccess)
            {
                // Remove from UI collection
                foreach (var item in selectedInventoryItems)
                {
                    SampleInventoryData.Remove(item);
                }
                
                SelectedItems.Clear();
                LastOperationText = $"Bulk deleted {result.SuccessfulItems.Count} items";
                OnPropertyChanged(nameof(ItemCount));
                OnPropertyChanged(nameof(SelectedItemCount));
        OnPropertyChanged(nameof(HasSelectedItems));
                OnPropertyChanged(nameof(HasSelectedItems));
            }
            else
            {
                LastOperationText = $"Bulk delete failed: {string.Join(", ", result.Errors)}";
            }
        }
        catch (Exception ex)
        {
            LastOperationText = $"Bulk delete error: {ex.Message}";
        }
    }

    /// <summary>
    /// Shows data analytics and insights.
    /// Phase 6 feature for data analysis.
    /// </summary>
    private async void ShowDataAnalytics()
    {
        try
        {
            LastOperationText = "Analyzing data...";
            
            var service = new CustomDataGridService(null!);
            var demoDataGrid = this.FindControl<CustomDataGrid>("DemoDataGrid");
            
            if (demoDataGrid?.Columns != null)
            {
                var analytics = await service.GetDataAnalyticsAsync(SampleInventoryData, demoDataGrid.Columns);
                
                var qualityScore = analytics.QualityMetrics.OverallScore;
                var totalItems = analytics.TotalItems;
                var insightCount = analytics.Insights.Count;
                
                DataAnalyticsText = $"Quality: {qualityScore:F1}%, Items: {totalItems}, Insights: {insightCount}";
                LastOperationText = $"Analytics complete - {insightCount} insights found";
                
                // Log insights to debug console
                foreach (var insight in analytics.Insights.Take(3))
                {
                    System.Diagnostics.Debug.WriteLine($"[{insight.Importance}] {insight.Title}: {insight.Description}");
                }
            }
        }
        catch (Exception ex)
        {
            LastOperationText = $"Analytics error: {ex.Message}";
            DataAnalyticsText = "Analytics failed";
        }
    }

    /// <summary>
    /// Creates a sample CSV file for import demonstration.
    /// </summary>
    private async Task CreateSampleCsvFileAsync(string filePath)
    {
        var csvContent = """
            PartId,Operation,Location,Quantity,Notes
            IMP-001,90,RECV-A1,50,Imported sample part 1
            IMP-002,100,WIP-B1,30,Imported sample part 2
            IMP-003,110,WIP-C1,20,Imported sample part 3
            """;
        
        await File.WriteAllTextAsync(filePath, csvContent);
    }

    #endregion

    #endregion

    #region INotifyPropertyChanged Implementation

    public new event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Initialization

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    #endregion
}