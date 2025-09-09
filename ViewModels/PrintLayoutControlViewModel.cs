using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// PrintLayoutControlViewModel manages column visibility and layout customization for print operations.
/// Provides drag-and-drop column ordering, visibility toggles, and print style selection.
/// Uses MVVM Community Toolkit patterns following MTM established conventions.
/// </summary>
public partial class PrintLayoutControlViewModel : BaseViewModel
{
    #region Status and Loading

    [ObservableProperty]
    private string statusMessage = "Ready to customize print layout";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasUnsavedChanges = false;

    #endregion

    #region Layout Configuration

    [ObservableProperty]
    private PrintStyle selectedStyle = PrintStyle.Simple;

    [ObservableProperty]
    private bool includeHeaders = true;

    [ObservableProperty]
    private bool includeFooters = true;

    [ObservableProperty]
    private bool includeGridLines = true;

    [ObservableProperty]
    private bool includeRowNumbers = false;

    [ObservableProperty]
    private bool includeTimestamp = true;

    [ObservableProperty]
    private bool includeUserInfo = true;

    #endregion

    #region Column Management

    [ObservableProperty]
    private ObservableCollection<PrintColumnViewModel> columns = new();

    [ObservableProperty]
    private PrintColumnViewModel? selectedColumn;

    [ObservableProperty]
    private int visibleColumnCount = 0;

    [ObservableProperty]
    private int totalColumnCount = 0;

    #endregion

    #region Quick Actions

    [ObservableProperty]
    private bool allColumnsVisible = true;

    #endregion

    #region Collections for ComboBoxes

    public ObservableCollection<PrintStyle> StyleOptions { get; } = new()
    {
        PrintStyle.Simple,
        PrintStyle.Stylized
    };

    public ObservableCollection<PrintAlignment> AlignmentOptions { get; } = new()
    {
        PrintAlignment.Left,
        PrintAlignment.Center,
        PrintAlignment.Right
    };

    #endregion

    public PrintLayoutControlViewModel(ILogger<PrintLayoutControlViewModel> logger) : base(logger)
    {
        Logger.LogDebug("PrintLayoutControlViewModel initialized");
    }

    #region Initialization

    /// <summary>
    /// Initialize the layout control with column data
    /// </summary>
    public void InitializeColumns(ObservableCollection<PrintColumnInfo> printColumns)
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading column configuration...";

            Columns.Clear();

            foreach (var column in printColumns.OrderBy(c => c.DisplayOrder))
            {
                var columnViewModel = new PrintColumnViewModel
                {
                    ColumnInfo = column,
                    Header = column.Header,
                    PropertyName = column.PropertyName,
                    IsVisible = column.IsVisible,
                    DisplayOrder = column.DisplayOrder,
                    Width = column.Width,
                    Alignment = column.Alignment
                };

                // Subscribe to property changes
                columnViewModel.PropertyChanged += OnColumnPropertyChanged;
                
                Columns.Add(columnViewModel);
            }

            UpdateColumnCounts();
            StatusMessage = $"Loaded {Columns.Count} columns for customization";

            Logger.LogDebug("Initialized PrintLayoutControl with {ColumnCount} columns", Columns.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing columns");
            StatusMessage = "Error loading columns";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OnColumnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is PrintColumnViewModel column)
        {
            // Update the underlying ColumnInfo when ViewModel properties change
            if (e.PropertyName == nameof(PrintColumnViewModel.IsVisible))
            {
                column.ColumnInfo.IsVisible = column.IsVisible;
                UpdateColumnCounts();
                HasUnsavedChanges = true;
            }
            else if (e.PropertyName == nameof(PrintColumnViewModel.DisplayOrder))
            {
                column.ColumnInfo.DisplayOrder = column.DisplayOrder;
                HasUnsavedChanges = true;
            }
            else if (e.PropertyName == nameof(PrintColumnViewModel.Width))
            {
                column.ColumnInfo.Width = column.Width;
                HasUnsavedChanges = true;
            }
            else if (e.PropertyName == nameof(PrintColumnViewModel.Alignment))
            {
                column.ColumnInfo.Alignment = column.Alignment;
                HasUnsavedChanges = true;
            }
        }
    }

    private void UpdateColumnCounts()
    {
        TotalColumnCount = Columns.Count;
        VisibleColumnCount = Columns.Count(c => c.IsVisible);
        AllColumnsVisible = VisibleColumnCount == TotalColumnCount;
        
        StatusMessage = $"{VisibleColumnCount} of {TotalColumnCount} columns visible";
    }

    #endregion

    #region Column Visibility Commands

    /// <summary>
    /// Toggle visibility for all columns
    /// </summary>
    [RelayCommand]
    private void ToggleAllColumns()
    {
        try
        {
            IsLoading = true;
            
            var makeVisible = !AllColumnsVisible;
            
            foreach (var column in Columns)
            {
                column.IsVisible = makeVisible;
            }

            UpdateColumnCounts();
            HasUnsavedChanges = true;
            
            StatusMessage = makeVisible ? "All columns made visible" : "All columns hidden";
            Logger.LogDebug("Toggled all columns visibility to: {Visible}", makeVisible);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling all columns");
            StatusMessage = "Error toggling columns";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Reset columns to default visibility
    /// </summary>
    [RelayCommand]
    private void ResetToDefaults()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Resetting columns to default configuration...";

            foreach (var column in Columns)
            {
                // Set default visibility (all visible by default)
                column.IsVisible = true;
                
                // Reset width to default
                column.Width = 100;
                
                // Reset alignment based on column type/name
                column.Alignment = DetermineDefaultAlignment(column.PropertyName);
                
                // Reset display order to original order
                column.DisplayOrder = Columns.IndexOf(column);
            }

            UpdateColumnCounts();
            HasUnsavedChanges = true;
            
            StatusMessage = "Columns reset to default configuration";
            Logger.LogDebug("Reset {ColumnCount} columns to default configuration", Columns.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting columns to defaults");
            StatusMessage = "Error resetting columns";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private PrintAlignment DetermineDefaultAlignment(string columnName)
    {
        // Determine default alignment based on column name/type
        if (columnName.Contains("Quantity", StringComparison.OrdinalIgnoreCase) ||
            columnName.Contains("Count", StringComparison.OrdinalIgnoreCase) ||
            columnName.Contains("Number", StringComparison.OrdinalIgnoreCase))
        {
            return PrintAlignment.Right;
        }
        else if (columnName.Contains("Date", StringComparison.OrdinalIgnoreCase) ||
                 columnName.Contains("Time", StringComparison.OrdinalIgnoreCase) ||
                 columnName.Contains("Operation", StringComparison.OrdinalIgnoreCase) ||
                 columnName.Contains("Status", StringComparison.OrdinalIgnoreCase))
        {
            return PrintAlignment.Center;
        }
        else
        {
            return PrintAlignment.Left;
        }
    }

    #endregion

    #region Column Ordering Commands

    /// <summary>
    /// Move selected column up in display order
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    private void MoveColumnUp()
    {
        try
        {
            if (SelectedColumn == null) return;

            var currentIndex = Columns.IndexOf(SelectedColumn);
            if (currentIndex > 0)
            {
                var previousColumn = Columns[currentIndex - 1];
                
                // Swap display orders
                var tempOrder = SelectedColumn.DisplayOrder;
                SelectedColumn.DisplayOrder = previousColumn.DisplayOrder;
                previousColumn.DisplayOrder = tempOrder;

                // Move in collection
                Columns.Move(currentIndex, currentIndex - 1);

                HasUnsavedChanges = true;
                StatusMessage = $"Moved '{SelectedColumn.Header}' up";
                Logger.LogDebug("Moved column '{Header}' up in display order", SelectedColumn.Header);

                // Update command states
                MoveColumnUpCommand.NotifyCanExecuteChanged();
                MoveColumnDownCommand.NotifyCanExecuteChanged();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error moving column up");
            StatusMessage = "Error moving column";
        }
    }

    private bool CanMoveUp() => SelectedColumn != null && Columns.IndexOf(SelectedColumn) > 0;

    /// <summary>
    /// Move selected column down in display order
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    private void MoveColumnDown()
    {
        try
        {
            if (SelectedColumn == null) return;

            var currentIndex = Columns.IndexOf(SelectedColumn);
            if (currentIndex < Columns.Count - 1)
            {
                var nextColumn = Columns[currentIndex + 1];
                
                // Swap display orders
                var tempOrder = SelectedColumn.DisplayOrder;
                SelectedColumn.DisplayOrder = nextColumn.DisplayOrder;
                nextColumn.DisplayOrder = tempOrder;

                // Move in collection
                Columns.Move(currentIndex, currentIndex + 1);

                HasUnsavedChanges = true;
                StatusMessage = $"Moved '{SelectedColumn.Header}' down";
                Logger.LogDebug("Moved column '{Header}' down in display order", SelectedColumn.Header);

                // Update command states
                MoveColumnUpCommand.NotifyCanExecuteChanged();
                MoveColumnDownCommand.NotifyCanExecuteChanged();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error moving column down");
            StatusMessage = "Error moving column";
        }
    }

    private bool CanMoveDown() => SelectedColumn != null && Columns.IndexOf(SelectedColumn) < Columns.Count - 1;

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handle changes to layout settings
    /// </summary>
    partial void OnSelectedStyleChanged(PrintStyle value)
    {
        HasUnsavedChanges = true;
        StatusMessage = $"Print style changed to: {value}";
    }

    partial void OnIncludeHeadersChanged(bool value)
    {
        HasUnsavedChanges = true;
        StatusMessage = value ? "Headers will be included in print" : "Headers will be excluded from print";
    }

    partial void OnIncludeFootersChanged(bool value)
    {
        HasUnsavedChanges = true;
        StatusMessage = value ? "Footers will be included in print" : "Footers will be excluded from print";
    }

    partial void OnIncludeGridLinesChanged(bool value)
    {
        HasUnsavedChanges = true;
        StatusMessage = value ? "Grid lines will be included in print" : "Grid lines will be excluded from print";
    }

    partial void OnSelectedColumnChanged(PrintColumnViewModel? value)
    {
        // Update command states when selection changes
        MoveColumnUpCommand.NotifyCanExecuteChanged();
        MoveColumnDownCommand.NotifyCanExecuteChanged();
        
        if (value != null)
        {
            StatusMessage = $"Selected column: {value.Header}";
        }
        else
        {
            StatusMessage = "No column selected";
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Apply changes to the original print columns
    /// </summary>
    public void ApplyChanges(ObservableCollection<PrintColumnInfo> targetColumns)
    {
        try
        {
            foreach (var columnVM in Columns)
            {
                var targetColumn = targetColumns.FirstOrDefault(c => c.PropertyName == columnVM.PropertyName);
                if (targetColumn != null)
                {
                    targetColumn.IsVisible = columnVM.IsVisible;
                    targetColumn.DisplayOrder = columnVM.DisplayOrder;
                    targetColumn.Width = columnVM.Width;
                    targetColumn.Alignment = columnVM.Alignment;
                    targetColumn.Header = columnVM.Header;
                }
            }

            HasUnsavedChanges = false;
            StatusMessage = "Layout changes applied successfully";
            Logger.LogDebug("Applied layout changes to {ColumnCount} columns", Columns.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying layout changes");
            StatusMessage = "Error applying changes";
        }
    }

    #endregion
}

/// <summary>
/// ViewModel wrapper for PrintColumnInfo to enable MVVM binding and notifications
/// </summary>
public partial class PrintColumnViewModel : ObservableObject
{
    /// <summary>
    /// Reference to the underlying column info
    /// </summary>
    public PrintColumnInfo ColumnInfo { get; set; } = new();

    [ObservableProperty]
    private string header = string.Empty;

    [ObservableProperty]
    private string propertyName = string.Empty;

    [ObservableProperty]
    private bool isVisible = true;

    [ObservableProperty]
    private int displayOrder = 0;

    [ObservableProperty]
    private double width = 100;

    [ObservableProperty]
    private PrintAlignment alignment = PrintAlignment.Left;

    /// <summary>
    /// Display text for column type/preview
    /// </summary>
    public string TypeDisplayText => $"{PropertyName} ({Alignment})";

    /// <summary>
    /// Width display text for UI
    /// </summary>
    public string WidthDisplayText => $"{Width:F0}px";
}