using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Service interface for CustomDataGrid operations.
/// Provides data management, configuration, and utility methods for the custom grid control.
/// </summary>
public interface ICustomDataGridService
{
    /// <summary>
    /// Creates default column definitions for inventory data.
    /// </summary>
    Task<ObservableCollection<CustomDataGridColumn>> CreateInventoryColumnsAsync();
    
    /// <summary>
    /// Creates default column definitions for transaction data.
    /// </summary>
    Task<ObservableCollection<CustomDataGridColumn>> CreateTransactionColumnsAsync();
    
    /// <summary>
    /// Exports grid data to the specified format.
    /// </summary>
    Task<bool> ExportDataAsync<T>(IEnumerable<T> data, ObservableCollection<CustomDataGridColumn> columns, string filePath, string format);
    
    /// <summary>
    /// Exports grid data to the specified format with selection filtering support.
    /// </summary>
    Task<bool> ExportDataAsync<T>(IEnumerable<T> data, ObservableCollection<CustomDataGridColumn> columns, string filePath, string format, bool exportOnlySelected, IEnumerable<T>? selectedItems = null);
    
    /// <summary>
    /// Gets selection statistics for the provided data and selected items.
    /// </summary>
    SelectionStatistics GetSelectionStatistics<T>(IEnumerable<T> totalItems, IEnumerable<T>? selectedItems = null);
}

/// <summary>
/// MTM Custom Data Grid Service - Phase 2 Implementation
/// 
/// Provides data management and configuration services for CustomDataGrid controls.
/// Follows established MTM service patterns with category-based consolidation,
/// proper error handling, and MVVM Community Toolkit integration.
/// 
/// Phase 1 Features:
/// - Default column configuration for MTM data types
/// - Basic CSV export functionality  
/// - Performance optimization for large datasets
/// - Integration with MTM error handling and logging patterns
/// 
/// Phase 2 Features:
/// - Multi-selection support with export filtering
/// - Selection statistics and metrics
/// - Enhanced action command support
/// - Improved user experience with selection state management
/// </summary>
public class CustomDataGridService : ICustomDataGridService
{
    private readonly ILogger<CustomDataGridService> _logger;

    /// <summary>
    /// Initializes a new instance of the CustomDataGridService.
    /// </summary>
    public CustomDataGridService(ILogger<CustomDataGridService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        
        _logger.LogDebug("CustomDataGridService initialized");
    }

    /// <summary>
    /// Creates default column definitions for inventory data following MTM patterns.
    /// Includes standard inventory fields: PartId, Operation, Location, Quantity, etc.
    /// </summary>
    public async Task<ObservableCollection<CustomDataGridColumn>> CreateInventoryColumnsAsync()
    {
        try
        {
            _logger.LogDebug("Creating inventory columns");
            
            var columns = new ObservableCollection<CustomDataGridColumn>
            {
                new CustomDataGridColumn
                {
                    PropertyName = "PartId",
                    DisplayName = "Part ID",
                    DataType = typeof(string),
                    Width = 120,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Operation",
                    DisplayName = "Operation",
                    DataType = typeof(string),
                    Width = 80,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Location",
                    DisplayName = "Location", 
                    DataType = typeof(string),
                    Width = 100,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Quantity",
                    DisplayName = "Quantity",
                    DataType = typeof(int),
                    Width = 90,
                    CanSort = true,
                    CanFilter = true,
                    StringFormat = "N0"
                },
                new CustomDataGridColumn
                {
                    PropertyName = "LastUpdated",
                    DisplayName = "Last Updated",
                    DataType = typeof(DateTime),
                    Width = 140,
                    CanSort = true,
                    CanFilter = false,
                    StringFormat = "MM/dd/yy HH:mm"
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Notes",
                    DisplayName = "Notes",
                    DataType = typeof(string),
                    Width = double.NaN, // Star width
                    CanSort = false,
                    CanFilter = true
                }
            };

            await Task.Delay(1); // Simulate async operation
            
            _logger.LogDebug("Created {Count} inventory columns", columns.Count);
            return columns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating inventory columns");
            await HandleErrorAsync(ex, "Create inventory columns");
            return new ObservableCollection<CustomDataGridColumn>();
        }
    }

    /// <summary>
    /// Creates default column definitions for transaction data following MTM patterns.
    /// Includes standard transaction fields: TransactionId, PartId, TransactionType, etc.
    /// </summary>
    public async Task<ObservableCollection<CustomDataGridColumn>> CreateTransactionColumnsAsync()
    {
        try
        {
            _logger.LogDebug("Creating transaction columns");
            
            var columns = new ObservableCollection<CustomDataGridColumn>
            {
                new CustomDataGridColumn
                {
                    PropertyName = "TransactionId",
                    DisplayName = "Transaction ID",
                    DataType = typeof(int),
                    Width = 100,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "PartId",
                    DisplayName = "Part ID",
                    DataType = typeof(string),
                    Width = 120,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Operation",
                    DisplayName = "Operation",
                    DataType = typeof(string),
                    Width = 80,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "TransactionType",
                    DisplayName = "Type",
                    DataType = typeof(string),
                    Width = 70,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Quantity",
                    DisplayName = "Quantity",
                    DataType = typeof(int),
                    Width = 90,
                    CanSort = true,
                    CanFilter = true,
                    StringFormat = "N0"
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Location",
                    DisplayName = "Location",
                    DataType = typeof(string),
                    Width = 100,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "UserId",
                    DisplayName = "User",
                    DataType = typeof(string),
                    Width = 100,
                    CanSort = true,
                    CanFilter = true
                },
                new CustomDataGridColumn
                {
                    PropertyName = "Timestamp",
                    DisplayName = "Date/Time",
                    DataType = typeof(DateTime),
                    Width = 140,
                    CanSort = true,
                    CanFilter = false,
                    StringFormat = "MM/dd/yy HH:mm"
                }
            };

            await Task.Delay(1); // Simulate async operation
            
            _logger.LogDebug("Created {Count} transaction columns", columns.Count);
            return columns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction columns");
            await HandleErrorAsync(ex, "Create transaction columns");
            return new ObservableCollection<CustomDataGridColumn>();
        }
    }

    /// <summary>
    /// Exports grid data to the specified format (CSV, Excel, etc.).
    /// Phase 1 implementation provides basic CSV export functionality.
    /// Phase 2 enhancement: Supports exporting only selected items.
    /// </summary>
    public Task<bool> ExportDataAsync<T>(
        IEnumerable<T> data, 
        ObservableCollection<CustomDataGridColumn> columns, 
        string filePath, 
        string format)
    {
        return ExportDataAsync(data, columns, filePath, format, exportOnlySelected: false);
    }

    /// <summary>
    /// Exports grid data to the specified format with selection filtering support.
    /// Phase 2 implementation supports exporting only selected items.
    /// </summary>
    public async Task<bool> ExportDataAsync<T>(
        IEnumerable<T> data, 
        ObservableCollection<CustomDataGridColumn> columns, 
        string filePath, 
        string format,
        bool exportOnlySelected,
        IEnumerable<T>? selectedItems = null)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(data);
            ArgumentNullException.ThrowIfNull(columns);
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
            ArgumentException.ThrowIfNullOrWhiteSpace(format);
            
            _logger.LogInformation("Exporting data to {Format} format: {FilePath} (Selected only: {SelectedOnly})", 
                format.ToUpper(), filePath, exportOnlySelected);

            // Filter data if only exporting selected items
            var exportData = exportOnlySelected && selectedItems != null
                ? selectedItems
                : data;
            
            switch (format.ToLowerInvariant())
            {
                case "csv":
                    return await ExportToCsvAsync(exportData, columns, filePath);
                    
                case "excel":
                case "xlsx":
                    // Future implementation - Phase 6
                    _logger.LogWarning("Excel export not yet implemented in Phase 2");
                    return false;
                    
                default:
                    _logger.LogWarning("Unsupported export format: {Format}", format);
                    return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data to {Format}", format);
            await HandleErrorAsync(ex, $"Export data to {format}");
            return false;
        }
    }

    /// <summary>
    /// Gets selection statistics for the provided data and selected items.
    /// Useful for displaying selection information in UI.
    /// </summary>
    public SelectionStatistics GetSelectionStatistics<T>(
        IEnumerable<T> totalItems,
        IEnumerable<T>? selectedItems = null)
    {
        try
        {
            var total = totalItems.Count();
            var selected = selectedItems?.Count() ?? 0;
            
            return new SelectionStatistics
            {
                TotalCount = total,
                SelectedCount = selected,
                HasSelection = selected > 0,
                HasMultipleSelection = selected > 1,
                SelectionPercentage = total > 0 ? (double)selected / total * 100 : 0,
                IsAllSelected = total > 0 && selected == total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating selection statistics");
            return new SelectionStatistics();
        }
    }

    #region Private Methods

    private async Task<bool> ExportToCsvAsync<T>(
        IEnumerable<T> data, 
        ObservableCollection<CustomDataGridColumn> columns, 
        string filePath)
    {
        try
        {
            var visibleColumns = columns.Where(c => c.IsVisible).ToList();
            var dataList = data.ToList();
            
            using var writer = new System.IO.StreamWriter(filePath);
            
            // Write header
            var headers = visibleColumns.Select(c => EscapeCsvValue(c.DisplayName));
            await writer.WriteLineAsync(string.Join(",", headers));
            
            // Write data rows
            foreach (var item in dataList)
            {
                var values = new List<string>();
                
                foreach (var column in visibleColumns)
                {
                    var propertyInfo = typeof(T).GetProperty(column.PropertyName);
                    var value = propertyInfo?.GetValue(item);
                    
                    string stringValue;
                    if (value == null)
                    {
                        stringValue = string.Empty;
                    }
                    else if (!string.IsNullOrEmpty(column.StringFormat) && value is IFormattable formattable)
                    {
                        stringValue = formattable.ToString(column.StringFormat, null);
                    }
                    else
                    {
                        stringValue = value.ToString() ?? string.Empty;
                    }
                    
                    values.Add(EscapeCsvValue(stringValue));
                }
                
                await writer.WriteLineAsync(string.Join(",", values));
            }
            
            _logger.LogDebug("CSV export completed: {RowCount} rows, {ColumnCount} columns", 
                dataList.Count, visibleColumns.Count);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CSV export");
            return false;
        }
    }

    private static string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
            
        // Escape commas, quotes, and newlines
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        
        return value;
    }

    private Task HandleErrorAsync(Exception ex, string context)
    {
        try
        {
            // Simple error logging for Phase 1 - full MTM error handling integration can be added later
            _logger.LogError(ex, "Error in {Context}: {Message}", context, ex.Message);
        }
        catch (Exception handlingEx)
        {
            _logger.LogCritical(handlingEx, "Error in error handling for context: {Context}", context);
        }
        
        return Task.CompletedTask;
    }

    #endregion
}

/// <summary>
/// Statistics about the current selection state in a data grid.
/// Provides useful metrics for UI display and business logic decisions.
/// </summary>
public class SelectionStatistics
{
    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the number of selected items.
    /// </summary>
    public int SelectedCount { get; set; }

    /// <summary>
    /// Gets whether there are any selected items.
    /// </summary>
    public bool HasSelection { get; set; }

    /// <summary>
    /// Gets whether multiple items are selected.
    /// </summary>
    public bool HasMultipleSelection { get; set; }

    /// <summary>
    /// Gets the percentage of items selected (0-100).
    /// </summary>
    public double SelectionPercentage { get; set; }

    /// <summary>
    /// Gets whether all items are selected.
    /// </summary>
    public bool IsAllSelected { get; set; }

    /// <summary>
    /// Gets a summary string describing the selection.
    /// </summary>
    public string SelectionSummary => 
        SelectedCount == 0 ? "No items selected" :
        SelectedCount == 1 ? "1 item selected" :
        IsAllSelected ? $"All {TotalCount} items selected" :
        $"{SelectedCount} of {TotalCount} items selected ({SelectionPercentage:F0}%)";
}