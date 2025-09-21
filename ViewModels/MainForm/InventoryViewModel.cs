using MTM_WIP_Application_Avalonia.Models.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for the main inventory view, providing comprehensive inventory management capabilities.
/// Supports searching, sorting, pagination, and detailed inventory item operations.
/// Uses MVVM Community Toolkit for property change notifications and command handling.
/// </summary>
public partial class InventoryViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the collection of inventory items currently displayed.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayInfo))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteNextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteLastPageCommand))]
    private ObservableCollection<InventoryItem> inventoryItems = new();

    /// <summary>
    /// Gets or sets the currently selected inventory item.
    /// </summary>
    [ObservableProperty]
    private InventoryItem? selectedItem;

    /// <summary>
    /// Gets or sets the search text for filtering inventory items.
    /// </summary>
    [ObservableProperty]
    [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters")]
    private string searchText = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the inventory is currently loading.
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    /// <summary>
    /// Gets or sets the current status message for the inventory operations.
    /// </summary>
    [ObservableProperty]
    private string statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets the total number of inventory items matching the current filter.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayInfo))]
    private int totalItems = 0;

    /// <summary>
    /// Gets or sets the current page number for pagination.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayInfo))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteFirstPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExecutePreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteNextPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteLastPageCommand))]
    [Range(1, int.MaxValue, ErrorMessage = "Current page must be greater than 0")]
    private int currentPage = 1;

    /// <summary>
    /// Gets or sets the number of items to display per page.
    /// </summary>
    [ObservableProperty]
    [Range(10, 200, ErrorMessage = "Items per page must be between 10 and 200")]
    private int itemsPerPage = 50;

    /// <summary>
    /// Gets or sets the column name used for sorting.
    /// </summary>
    [ObservableProperty]
    private string sortColumn = "PartId";

    /// <summary>
    /// Gets or sets a value indicating whether sorting is in ascending order.
    /// </summary>
    [ObservableProperty]
    private bool sortAscending = true;

    /// <summary>
    /// Gets display information about the current inventory view including pagination details.
    /// </summary>
    public string DisplayInfo => $"Showing {InventoryItems.Count} of {TotalItems} items (Page {CurrentPage})";

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryViewModel"/> class.
    /// </summary>
    /// <param name="applicationState">The application state service for managing user context.</param>
    /// <param name="databaseService">The database service for inventory operations.</param>
    /// <param name="logger">The logger for this ViewModel.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
    public InventoryViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<InventoryViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("InventoryViewModel initialized with dependency injection");

        _ = LoadInventoryAsync(); // Load initial data
    }

    #region Command Methods

    /// <summary>
    /// Loads inventory data asynchronously with pagination, sorting, and filtering.
    /// </summary>
    /// <returns>A task representing the asynchronous load operation.</returns>
    [RelayCommand]
    private async Task LoadInventoryAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading inventory data...";
            
            using var scope = Logger.BeginScope("LoadInventory");
            Logger.LogInformation("Loading inventory data for page {Page}", CurrentPage);

            // For now, load all inventory and implement client-side paging
            // In a real application, you'd want server-side paging for performance
            var dataTable = await _databaseService.GetAllPartIDsAsync().ConfigureAwait(false);
            
            var allItems = ConvertDataTableToInventoryItems(dataTable).ToList();
            
            // Apply search filter if specified
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                allItems = allItems.Where(item => 
                    item.PartId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    item.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    item.Operation.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    item.ItemType.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply sorting
            allItems = SortAscending 
                ? allItems.OrderBy(GetSortValue).ToList()
                : allItems.OrderByDescending(GetSortValue).ToList();

            TotalItems = allItems.Count;

            // Apply paging
            var pagedItems = allItems
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            InventoryItems = new ObservableCollection<InventoryItem>(pagedItems);
            
            StatusMessage = $"Loaded {InventoryItems.Count} inventory items";
            Logger.LogInformation("Successfully loaded {Count} inventory items (Page {Page} of {TotalItems} total)", 
                InventoryItems.Count, CurrentPage, TotalItems);

            OnPropertyChanged(nameof(DisplayInfo));
        }
        catch (Exception ex)
        {
            StatusMessage = "Error loading inventory data.";
            Logger.LogError(ex, "Error loading inventory data");
            
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Load Inventory",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadInventoryAsync" }).ConfigureAwait(false);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Executes a search operation and resets to the first page.
    /// </summary>
    /// <returns>A task representing the asynchronous search operation.</returns>
    [RelayCommand]
    private async Task ExecuteSearchAsync()
    {
        CurrentPage = 1; // Reset to first page when searching
        await LoadInventoryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Refreshes the inventory data by reloading the current view.
    /// </summary>
    /// <returns>A task representing the asynchronous refresh operation.</returns>
    [RelayCommand]
    private async Task ExecuteRefreshAsync()
    {
        await LoadInventoryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Executes sorting by the specified column name.
    /// </summary>
    /// <param name="columnName">The name of the column to sort by.</param>
    /// <returns>A task representing the asynchronous sort operation.</returns>
    [RelayCommand]
    private async Task ExecuteSortAsync(string? columnName)
    {
        if (string.IsNullOrEmpty(columnName)) return;

        if (SortColumn == columnName)
        {
            SortAscending = !SortAscending;
        }
        else
        {
            SortColumn = columnName;
            SortAscending = true;
        }

        await LoadInventoryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Navigates to the first page of inventory results.
    /// </summary>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    [RelayCommand(CanExecute = nameof(CanNavigateToFirstPage))]
    private async Task ExecuteFirstPageAsync()
    {
        CurrentPage = 1;
        await LoadInventoryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether navigation to the first page is possible.
    /// </summary>
    /// <returns>True if not on the first page; otherwise, false.</returns>
    private bool CanNavigateToFirstPage() => CurrentPage > 1;

    /// <summary>
    /// Navigates to the previous page of inventory results.
    /// </summary>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    [RelayCommand(CanExecute = nameof(CanNavigateToPreviousPage))]
    private async Task ExecutePreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadInventoryAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Determines whether navigation to the previous page is possible.
    /// </summary>
    /// <returns>True if not on the first page; otherwise, false.</returns>
    private bool CanNavigateToPreviousPage() => CurrentPage > 1;

    /// <summary>
    /// Navigates to the next page of inventory results.
    /// </summary>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    [RelayCommand(CanExecute = nameof(CanNavigateToNextPage))]
    private async Task ExecuteNextPageAsync()
    {
        CurrentPage++;
        await LoadInventoryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether navigation to the next page is possible.
    /// </summary>
    /// <returns>True if there are more items to display; otherwise, false.</returns>
    private bool CanNavigateToNextPage() => InventoryItems.Count == ItemsPerPage;

    /// <summary>
    /// Navigates to the last page of inventory results.
    /// </summary>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    [RelayCommand(CanExecute = nameof(CanNavigateToLastPage))]
    private async Task ExecuteLastPageAsync()
    {
        // Calculate last page based on total items
        var lastPage = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        CurrentPage = lastPage;
        await LoadInventoryAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether navigation to the last page is possible.
    /// </summary>
    /// <returns>True if there are more items and not on the last page; otherwise, false.</returns>
    private bool CanNavigateToLastPage() => InventoryItems.Count == ItemsPerPage;

    /// <summary>
    /// Displays details for the specified inventory item.
    /// </summary>
    /// <param name="item">The inventory item to view details for.</param>
    /// <returns>A task representing the asynchronous view operation.</returns>
    [RelayCommand]
    private async Task ExecuteViewDetailsAsync(InventoryItem? item)
    {
        if (item == null) return;

        try
        {
            SelectedItem = item;
            
            using var scope = Logger.BeginScope("ViewDetails");
            Logger.LogInformation("Selected inventory item: PartId={PartId}, Location={Location}", 
                item.PartId, item.Location);

            // Here you could open a details view, edit dialog, etc.
            StatusMessage = $"Selected: {item.PartId} at {item.Location}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error viewing inventory item details");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "View Details",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["PartId"] = item?.PartId ?? "Unknown" }).ConfigureAwait(false);
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Converts a DataTable from database query results to a list of InventoryItem objects.
    /// </summary>
    /// <param name="dataTable">The DataTable containing inventory data from the database.</param>
    /// <returns>A list of InventoryItem objects converted from the DataTable.</returns>
    private static List<InventoryItem> ConvertDataTableToInventoryItems(DataTable dataTable)
    {
        var items = new List<InventoryItem>();
        
        foreach (DataRow row in dataTable.Rows)
        {
            var item = new InventoryItem
            {
                PartId = row["part_id"]?.ToString() ?? string.Empty,
                Location = row["location"]?.ToString() ?? string.Empty,
                Operation = row["operation"]?.ToString() ?? string.Empty,
                Quantity = Convert.ToInt32(row["quantity"] ?? 0),
                ItemType = row["item_type"]?.ToString() ?? string.Empty,
                BatchNumber = row["batch_number"]?.ToString() ?? string.Empty,
                DateAdded = Convert.ToDateTime(row["date_added"] ?? DateTime.Now),
                User = row["user"]?.ToString() ?? string.Empty,
                Notes = row["notes"]?.ToString() ?? string.Empty
            };
            items.Add(item);
        }
        
        return items;
    }

    /// <summary>
    /// Gets the sort value for the specified inventory item based on the current sort column.
    /// </summary>
    /// <param name="item">The inventory item to get the sort value for.</param>
    /// <returns>The value to use for sorting the item.</returns>
    private object GetSortValue(InventoryItem item)
    {
        return SortColumn switch
        {
            "PartId" => item.PartId,
            "Location" => item.Location,
            "Operation" => item.Operation,
            "Quantity" => item.Quantity,
            "ItemType" => item.ItemType,
            "DateAdded" => item.DateAdded,
            "User" => item.User,
            _ => item.PartId
        };
    }

    #endregion
}
