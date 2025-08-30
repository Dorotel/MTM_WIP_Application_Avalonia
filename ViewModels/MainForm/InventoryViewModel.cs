using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class InventoryViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Private Fields
    private ObservableCollection<InventoryItem> _inventoryItems = new();
    private InventoryItem? _selectedItem;
    private string _searchText = string.Empty;
    private bool _isLoading = false;
    private string _statusMessage = string.Empty;
    private int _totalItems = 0;
    private int _currentPage = 1;
    private int _itemsPerPage = 50;
    private string _sortColumn = "PartId";
    private bool _sortAscending = true;
    #endregion

    #region Public Properties
    public ObservableCollection<InventoryItem> InventoryItems
    {
        get => _inventoryItems;
        set => SetProperty(ref _inventoryItems, value);
    }

    public InventoryItem? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public int TotalItems
    {
        get => _totalItems;
        set => SetProperty(ref _totalItems, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int ItemsPerPage
    {
        get => _itemsPerPage;
        set => SetProperty(ref _itemsPerPage, value);
    }

    public string SortColumn
    {
        get => _sortColumn;
        set => SetProperty(ref _sortColumn, value);
    }

    public bool SortAscending
    {
        get => _sortAscending;
        set => SetProperty(ref _sortAscending, value);
    }

    public string DisplayInfo => $"Showing {InventoryItems.Count} of {TotalItems} items (Page {CurrentPage})";
    #endregion

    #region Commands
    public ICommand LoadInventoryCommand { get; private set; } = default!;
    public ICommand SearchCommand { get; private set; } = default!;
    public ICommand RefreshCommand { get; private set; } = default!;
    public ICommand SortCommand { get; private set; } = default!;
    public ICommand FirstPageCommand { get; private set; } = default!;
    public ICommand PreviousPageCommand { get; private set; } = default!;
    public ICommand NextPageCommand { get; private set; } = default!;
    public ICommand LastPageCommand { get; private set; } = default!;
    public ICommand ViewDetailsCommand { get; private set; } = default!;
    #endregion

    public InventoryViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<InventoryViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("InventoryViewModel initialized with dependency injection");

        InitializeCommands();
        _ = LoadInventoryAsync(); // Load initial data
    }

    private void InitializeCommands()
    {
        LoadInventoryCommand = new AsyncCommand(LoadInventoryAsync);
        SearchCommand = new AsyncCommand(ExecuteSearchAsync);
        RefreshCommand = new AsyncCommand(ExecuteRefreshAsync);
        SortCommand = new AsyncCommand<string>(ExecuteSortAsync);
        FirstPageCommand = new AsyncCommand(ExecuteFirstPageAsync, () => CurrentPage > 1);
        PreviousPageCommand = new AsyncCommand(ExecutePreviousPageAsync, () => CurrentPage > 1);
        NextPageCommand = new AsyncCommand(ExecuteNextPageAsync, () => InventoryItems.Count == ItemsPerPage);
        LastPageCommand = new AsyncCommand(ExecuteLastPageAsync, () => InventoryItems.Count == ItemsPerPage);
        ViewDetailsCommand = new AsyncCommand<InventoryItem>(ExecuteViewDetailsAsync);

        Logger.LogDebug("Commands initialized for InventoryViewModel");
    }

    private async Task LoadInventoryAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading inventory data...";
            Logger.LogInformation("Loading inventory data for page {Page}", CurrentPage);

            // For now, load all inventory and implement client-side paging
            // In a real application, you'd want server-side paging for performance
            var dataTable = await _databaseService.GetAllPartIDsAsync();
            
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
            
            await ErrorHandling.HandleErrorAsync(
                ex,
                "Load Inventory",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadInventoryAsync" });
        }
        finally
        {
            IsLoading = false;
        }
    }

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

    private async Task ExecuteSearchAsync()
    {
        CurrentPage = 1; // Reset to first page when searching
        await LoadInventoryAsync();
    }

    private async Task ExecuteRefreshAsync()
    {
        await LoadInventoryAsync();
    }

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

        await LoadInventoryAsync();
    }

    private async Task ExecuteFirstPageAsync()
    {
        CurrentPage = 1;
        await LoadInventoryAsync();
    }

    private async Task ExecutePreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadInventoryAsync();
        }
    }

    private async Task ExecuteNextPageAsync()
    {
        CurrentPage++;
        await LoadInventoryAsync();
    }

    private async Task ExecuteLastPageAsync()
    {
        // Calculate last page based on total items
        var lastPage = (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        CurrentPage = lastPage;
        await LoadInventoryAsync();
    }

    private async Task ExecuteViewDetailsAsync(InventoryItem? item)
    {
        if (item == null) return;

        try
        {
            SelectedItem = item;
            Logger.LogInformation("Selected inventory item: PartId={PartId}, Location={Location}", 
                item.PartId, item.Location);

            // Here you could open a details view, edit dialog, etc.
            StatusMessage = $"Selected: {item.PartId} at {item.Location}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error viewing inventory item details");
            await ErrorHandling.HandleErrorAsync(
                ex,
                "View Details",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["PartId"] = item?.PartId ?? "Unknown" });
        }
    }
}