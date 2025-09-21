using MTM_WIP_Application_Avalonia.Models.UI;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.ViewModels.Shared;

/// <summary>
/// ViewModel for CustomDataGrid control using MVVM Community Toolkit patterns.
/// Provides data binding, command handling, and state management for the custom grid.
/// Follows established MTM patterns with proper error handling and logging.
/// </summary>
public partial class CustomDataGridViewModel : BaseViewModel
{
    #region Properties

    private ObservableCollection<object> _itemsSource = new();
    /// <summary>
    /// Gets or sets the data source for the grid.
    /// </summary>
    public ObservableCollection<object> ItemsSource
    {
        get => _itemsSource;
        set => SetProperty(ref _itemsSource, value);
    }

    private ObservableCollection<CustomDataGridColumn> _columns = new();
    /// <summary>
    /// Gets or sets the column definitions for the grid.
    /// </summary>
    public ObservableCollection<CustomDataGridColumn> Columns
    {
        get => _columns;
        set => SetProperty(ref _columns, value);
    }

    private object? _selectedItem;
    /// <summary>
    /// Gets or sets the currently selected item.
    /// </summary>
    public object? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
            {
                OnPropertyChanged(nameof(HasSelection));
                DeleteSelectedCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private ObservableCollection<object> _selectedItems = new();
    /// <summary>
    /// Gets or sets the collection of selected items for multi-selection.
    /// </summary>
    public ObservableCollection<object> SelectedItems
    {
        get => _selectedItems;
        set
        {
            if (SetProperty(ref _selectedItems, value))
            {
                OnPropertyChanged(nameof(HasMultipleSelection));
                DeleteSelectedCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private bool _isMultiSelectEnabled = false;
    /// <summary>
    /// Gets or sets whether multiple selection is enabled.
    /// </summary>
    public bool IsMultiSelectEnabled
    {
        get => _isMultiSelectEnabled;
        set => SetProperty(ref _isMultiSelectEnabled, value);
    }

    private bool _isLoading = false;
    /// <summary>
    /// Gets or sets whether the grid is currently loading data.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value))
            {
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private string _searchText = string.Empty;
    /// <summary>
    /// Gets or sets the search/filter text.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    private bool _showRowNumbers = true;
    /// <summary>
    /// Gets or sets whether the grid shows row numbers.
    /// </summary>
    public bool ShowRowNumbers
    {
        get => _showRowNumbers;
        set => SetProperty(ref _showRowNumbers, value);
    }

    private double _rowHeight = 28.0;
    /// <summary>
    /// Gets or sets the row height for the grid.
    /// </summary>
    public double RowHeight
    {
        get => _rowHeight;
        set => SetProperty(ref _rowHeight, value);
    }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets whether there is a current selection.
    /// </summary>
    public bool HasSelection => SelectedItem != null;

    /// <summary>
    /// Gets whether there are multiple items selected.
    /// </summary>
    public bool HasMultipleSelection => SelectedItems.Count > 1;

    /// <summary>
    /// Gets the total number of items in the data source.
    /// </summary>
    public int TotalItemCount => ItemsSource.Count;

    /// <summary>
    /// Gets the number of selected items.
    /// </summary>
    public int SelectedItemCount => IsMultiSelectEnabled ? SelectedItems.Count : (HasSelection ? 1 : 0);

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the CustomDataGridViewModel.
    /// </summary>
    public CustomDataGridViewModel(ILogger<CustomDataGridViewModel> logger) : base(logger)
    {
        Logger.LogDebug("CustomDataGridViewModel initialized");

        // Initialize event handlers
        SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;

        // Set up default columns - can be overridden by consumer
        SetupDefaultColumns();
    }

    #endregion

    #region Commands

    private RelayCommand? _refreshCommand;
    /// <summary>
    /// Command to refresh the grid data.
    /// </summary>
    public RelayCommand RefreshCommand => _refreshCommand ??= new RelayCommand(async () => await RefreshAsync(), () => CanRefresh);

    private RelayCommand? _deleteSelectedCommand;
    /// <summary>
    /// Command to delete the selected item(s).
    /// </summary>
    public RelayCommand DeleteSelectedCommand => _deleteSelectedCommand ??= new RelayCommand(async () => await DeleteSelectedAsync(), () => CanDeleteSelected);

    private RelayCommand? _clearSelectionCommand;
    /// <summary>
    /// Command to clear the current selection.
    /// </summary>
    public RelayCommand ClearSelectionCommand => _clearSelectionCommand ??= new RelayCommand(ClearSelection);

    private RelayCommand? _selectAllCommand;
    /// <summary>
    /// Command to select all items.
    /// </summary>
    public RelayCommand SelectAllCommand => _selectAllCommand ??= new RelayCommand(SelectAll);

    private RelayCommand<string>? _exportDataCommand;
    /// <summary>
    /// Command to export grid data.
    /// </summary>
    public RelayCommand<string> ExportDataCommand => _exportDataCommand ??= new RelayCommand<string>(async format => await ExportDataAsync(format));

    #endregion

    #region Command Implementations

    /// <summary>
    /// Refreshes the grid data.
    /// </summary>
    private async Task RefreshAsync()
    {
        IsLoading = true;
        try
        {
            Logger.LogInformation("Refreshing grid data");

            // Refresh logic would be implemented by derived classes or injected services
            await RefreshDataAsync();

            Logger.LogDebug("Grid data refreshed successfully, item count: {Count}", ItemsSource.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing grid data");
            await HandleErrorAsync(ex, "Refresh grid data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Deletes the selected item(s).
    /// </summary>
    private async Task DeleteSelectedAsync()
    {
        try
        {
            Logger.LogInformation("Deleting selected items, count: {Count}", SelectedItemCount);

            if (IsMultiSelectEnabled && SelectedItems.Count > 0)
            {
                await DeleteMultipleItemsAsync(SelectedItems);
            }
            else if (SelectedItem != null)
            {
                await DeleteSingleItemAsync(SelectedItem);
            }

            Logger.LogDebug("Selected items deleted successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting selected items");
            await HandleErrorAsync(ex, "Delete selected items");
        }
    }

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    private void ClearSelection()
    {
        try
        {
            SelectedItem = null;
            SelectedItems.Clear();
            Logger.LogDebug("Selection cleared");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error clearing selection");
        }
    }

    /// <summary>
    /// Selects all items.
    /// </summary>
    private void SelectAll()
    {
        try
        {
            if (IsMultiSelectEnabled)
            {
                SelectedItems.Clear();
                foreach (var item in ItemsSource)
                {
                    SelectedItems.Add(item);
                }
                Logger.LogDebug("All items selected, count: {Count}", SelectedItems.Count);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting all items");
        }
    }

    /// <summary>
    /// Exports grid data to the specified format.
    /// </summary>
    private async Task ExportDataAsync(string? format = "csv")
    {
        try
        {
            Logger.LogInformation("Exporting grid data to format: {Format}", format);

            // Export logic would be implemented by derived classes or injected services
            await ExportGridDataAsync(format ?? "csv");

            Logger.LogDebug("Grid data exported successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting grid data");
            await HandleErrorAsync(ex, "Export grid data");
        }
    }

    #endregion

    #region Command Can Execute Methods

    private bool CanRefresh => !IsLoading;

    private bool CanDeleteSelected => HasSelection && !IsLoading;

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a new column to the grid.
    /// </summary>
    public void AddColumn(CustomDataGridColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);

        try
        {
            Columns.Add(column);
            Logger.LogDebug("Column added: {DisplayName} ({PropertyName})", column.DisplayName, column.PropertyName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding column");
        }
    }

    /// <summary>
    /// Removes a column from the grid.
    /// </summary>
    public bool RemoveColumn(string propertyName)
    {
        try
        {
            var column = Columns.FirstOrDefault(c => c.PropertyName == propertyName);
            if (column != null)
            {
                Columns.Remove(column);
                Logger.LogDebug("Column removed: {DisplayName} ({PropertyName})", column.DisplayName, column.PropertyName);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error removing column: {PropertyName}", propertyName);
            return false;
        }
    }

    /// <summary>
    /// Sets the data source for the grid.
    /// </summary>
    public void SetItemsSource<T>(ObservableCollection<T> items) where T : class
    {
        try
        {
            ItemsSource.Clear();
            foreach (var item in items)
            {
                ItemsSource.Add(item);
            }

            Logger.LogDebug("ItemsSource set with {Count} items of type {Type}", items.Count, typeof(T).Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting ItemsSource");
        }
    }

    #endregion

    #region Protected Virtual Methods

    /// <summary>
    /// Override this method in derived classes to implement specific data refresh logic.
    /// </summary>
    protected virtual async Task RefreshDataAsync()
    {
        // Default implementation - derived classes should override
        await Task.Delay(100); // Simulate async operation
        Logger.LogDebug("Default refresh data implementation completed");
    }

    /// <summary>
    /// Override this method in derived classes to implement specific single item deletion logic.
    /// </summary>
    protected virtual async Task DeleteSingleItemAsync(object item)
    {
        // Default implementation - remove from collection
        ItemsSource.Remove(item);
        await Task.Delay(1); // Simulate async operation
        Logger.LogDebug("Default single item deletion completed");
    }

    /// <summary>
    /// Override this method in derived classes to implement specific multiple item deletion logic.
    /// </summary>
    protected virtual async Task DeleteMultipleItemsAsync(ObservableCollection<object> items)
    {
        // Default implementation - remove all from collection
        foreach (var item in items.ToList())
        {
            ItemsSource.Remove(item);
        }
        await Task.Delay(1); // Simulate async operation
        Logger.LogDebug("Default multiple item deletion completed for {Count} items", items.Count);
    }

    /// <summary>
    /// Override this method in derived classes to implement specific export logic.
    /// </summary>
    protected virtual async Task ExportGridDataAsync(string format)
    {
        // Default implementation - placeholder
        await Task.Delay(100); // Simulate async operation
        Logger.LogDebug("Default export implementation completed for format: {Format}", format);
    }

    #endregion

    #region Private Methods

    private void SetupDefaultColumns()
    {
        // Default columns - consumers can clear and add their own
        Logger.LogDebug("Setting up default columns");
    }

    private void OnSelectedItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        try
        {
            OnPropertyChanged(nameof(SelectedItemCount));
            OnPropertyChanged(nameof(HasMultipleSelection));

            // Update SelectedItem to the first selected item if we have selections
            if (SelectedItems.Count > 0 && SelectedItem != SelectedItems.First())
            {
                SelectedItem = SelectedItems.First();
            }
            else if (SelectedItems.Count == 0)
            {
                SelectedItem = null;
            }

            Logger.LogDebug("Selected items collection changed, new count: {Count}", SelectedItems.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling selected items collection change");
        }
    }

    private Task HandleErrorAsync(Exception ex, string context)
    {
        try
        {
            // Simple error logging for now - MTM error handling integration can be added later
            Logger.LogError(ex, "Error in {Context}: {Message}", context, ex.Message);
        }
        catch (Exception handlingEx)
        {
            Logger.LogCritical(handlingEx, "Error in error handling for context: {Context}", context);
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Cleanup

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
            Logger.LogDebug("CustomDataGridViewModel disposed");
        }

        base.Dispose(disposing);
    }

    #endregion
}
