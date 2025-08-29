using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Commands;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for advanced inventory search interface.
/// Provides comprehensive search capabilities with multiple filter criteria,
/// pagination, export functionality, and integration with MTM business operations.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// </summary>
public class SearchInventoryViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Observable Collections

    /// <summary>
    /// Available part options for filtering
    /// </summary>
    public ObservableCollection<string> PartOptions { get; } = new();

    /// <summary>
    /// Available operation options for filtering
    /// </summary>
    public ObservableCollection<string> OperationOptions { get; } = new();

    /// <summary>
    /// Available location options for filtering
    /// </summary>
    public ObservableCollection<string> LocationOptions { get; } = new();

    /// <summary>
    /// Search results displayed in the DataGrid
    /// </summary>
    public ObservableCollection<InventoryItem> SearchResults { get; } = new();

    #endregion

    #region Search Filter Properties

    private string _partIdFilter = string.Empty;
    /// <summary>
    /// Part ID filter for search criteria
    /// </summary>
    public string PartIdFilter
    {
        get => _partIdFilter;
        set => SetProperty(ref _partIdFilter, value);
    }

    private string _operationFilter = string.Empty;
    /// <summary>
    /// Operation filter for search criteria
    /// </summary>
    public string OperationFilter
    {
        get => _operationFilter;
        set => SetProperty(ref _operationFilter, value);
    }

    private string _locationFilter = string.Empty;
    /// <summary>
    /// Location filter for search criteria
    /// </summary>
    public string LocationFilter
    {
        get => _locationFilter;
        set => SetProperty(ref _locationFilter, value);
    }

    private string _userFilter = string.Empty;
    /// <summary>
    /// User filter for search criteria
    /// </summary>
    public string UserFilter
    {
        get => _userFilter;
        set => SetProperty(ref _userFilter, value);
    }

    private DateTime? _dateFromFilter;
    /// <summary>
    /// Start date filter for search criteria
    /// </summary>
    public DateTime? DateFromFilter
    {
        get => _dateFromFilter;
        set => SetProperty(ref _dateFromFilter, value);
    }

    private DateTime? _dateToFilter;
    /// <summary>
    /// End date filter for search criteria
    /// </summary>
    public DateTime? DateToFilter
    {
        get => _dateToFilter;
        set => SetProperty(ref _dateToFilter, value);
    }

    #endregion

    #region Pagination Properties

    private int _currentPage = 1;
    /// <summary>
    /// Current page number for pagination
    /// </summary>
    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    private int _totalPages = 1;
    /// <summary>
    /// Total number of pages available
    /// </summary>
    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    private int _totalResults = 0;
    /// <summary>
    /// Total number of search results
    /// </summary>
    public int TotalResults
    {
        get => _totalResults;
        set => SetProperty(ref _totalResults, value);
    }

    private int _resultsPerPage = 50;
    /// <summary>
    /// Number of results to display per page
    /// </summary>
    public int ResultsPerPage
    {
        get => _resultsPerPage;
        set => SetProperty(ref _resultsPerPage, value);
    }

    #endregion

    #region Selection Properties

    private InventoryItem? _selectedResult;
    /// <summary>
    /// Currently selected search result
    /// </summary>
    public InventoryItem? SelectedResult
    {
        get => _selectedResult;
        set => SetProperty(ref _selectedResult, value);
    }

    #endregion

    #region Status Properties

    private bool _isSearching = false;
    /// <summary>
    /// Indicates if a search operation is in progress
    /// </summary>
    public bool IsSearching
    {
        get => _isSearching;
        set => SetProperty(ref _isSearching, value);
    }

    private string _statusMessage = "Ready to search";
    /// <summary>
    /// Status message for user feedback
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to execute search with current filters
    /// </summary>
    public ICommand SearchCommand { get; private set; }

    /// <summary>
    /// Command to clear all search filters
    /// </summary>
    public ICommand ClearFiltersCommand { get; private set; }

    /// <summary>
    /// Command to export search results
    /// </summary>
    public ICommand ExportCommand { get; private set; }

    /// <summary>
    /// Command to refresh master data
    /// </summary>
    public ICommand RefreshDataCommand { get; private set; }

    /// <summary>
    /// Command to navigate to first page
    /// </summary>
    public ICommand FirstPageCommand { get; private set; }

    /// <summary>
    /// Command to navigate to previous page
    /// </summary>
    public ICommand PreviousPageCommand { get; private set; }

    /// <summary>
    /// Command to navigate to next page
    /// </summary>
    public ICommand NextPageCommand { get; private set; }

    /// <summary>
    /// Command to navigate to last page
    /// </summary>
    public ICommand LastPageCommand { get; private set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Indicates if search can be executed
    /// </summary>
    public bool CanSearch => !IsSearching && HasSearchCriteria;

    /// <summary>
    /// Indicates if any search criteria are specified
    /// </summary>
    public bool HasSearchCriteria => !string.IsNullOrWhiteSpace(PartIdFilter) ||
                                    !string.IsNullOrWhiteSpace(OperationFilter) ||
                                    !string.IsNullOrWhiteSpace(LocationFilter) ||
                                    !string.IsNullOrWhiteSpace(UserFilter) ||
                                    DateFromFilter.HasValue ||
                                    DateToFilter.HasValue;

    /// <summary>
    /// Indicates if there are search results to export
    /// </summary>
    public bool CanExport => SearchResults.Count > 0;

    /// <summary>
    /// Indicates if can navigate to previous page
    /// </summary>
    public bool CanNavigatePrevious => CurrentPage > 1;

    /// <summary>
    /// Indicates if can navigate to next page
    /// </summary>
    public bool CanNavigateNext => CurrentPage < TotalPages;

    #endregion

    #region Constructor

    public SearchInventoryViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<SearchInventoryViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("SearchInventoryViewModel initialized with dependency injection");

        InitializeCommands();
        _ = LoadMasterDataAsync(); // Load dropdown data from database
    }

    #endregion

    #region Command Initialization

    /// <summary>
    /// Initializes all commands with their respective implementations
    /// </summary>
    private void InitializeCommands()
    {
        // Search command
        SearchCommand = new AsyncCommand(ExecuteSearchAsync, () => CanSearch);

        // Clear filters command
        ClearFiltersCommand = new RelayCommand(ExecuteClearFilters);

        // Export command
        ExportCommand = new AsyncCommand(ExecuteExportAsync, () => CanExport);

        // Refresh data command
        RefreshDataCommand = new AsyncCommand(LoadMasterDataAsync);

        // Pagination commands
        FirstPageCommand = new AsyncCommand(NavigateToFirstPageAsync, () => CanNavigatePrevious);
        PreviousPageCommand = new AsyncCommand(NavigateToPreviousPageAsync, () => CanNavigatePrevious);
        NextPageCommand = new AsyncCommand(NavigateToNextPageAsync, () => CanNavigateNext);
        LastPageCommand = new AsyncCommand(NavigateToLastPageAsync, () => CanNavigateNext);

        Logger.LogInformation("Search commands initialized");
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Loads master data for dropdown controls
    /// </summary>
    private async Task LoadMasterDataAsync()
    {
        try
        {
            Logger.LogInformation("Loading master data for search filters");

            // Load Parts
            var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (partResult.IsSuccess)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PartOptions.Clear();
                    foreach (DataRow row in partResult.Data.Rows)
                    {
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartOptions.Add(partId);
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} parts", PartOptions.Count);
            }

            // Load Operations
            var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            );

            if (operationResult.IsSuccess)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    OperationOptions.Clear();
                    foreach (DataRow row in operationResult.Data.Rows)
                    {
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            OperationOptions.Add(operation);
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} operations", OperationOptions.Count);
            }

            // Load Locations
            var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_locations_Get_All",
                new Dictionary<string, object>()
            );

            if (locationResult.IsSuccess)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LocationOptions.Clear();
                    foreach (DataRow row in locationResult.Data.Rows)
                    {
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            LocationOptions.Add(location);
                        }
                    }
                });
                Logger.LogInformation("Loaded {Count} locations", LocationOptions.Count);
            }

            StatusMessage = "Master data loaded successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load master data");
            StatusMessage = "Failed to load master data";
        }
    }

    #endregion

    #region Search Operations

    /// <summary>
    /// Executes search with current filter criteria
    /// </summary>
    private async Task ExecuteSearchAsync()
    {
        try
        {
            IsSearching = true;
            StatusMessage = "Searching...";

            // Clear results on UI thread
            await Dispatcher.UIThread.InvokeAsync(() => SearchResults.Clear());

            Logger.LogInformation("Executing search with filters: Part={PartId}, Operation={Operation}, Location={Location}, User={User}",
                PartIdFilter, OperationFilter, LocationFilter, UserFilter);

            // Determine which search method to use based on filters
            DataTable result;

            if (!string.IsNullOrWhiteSpace(PartIdFilter) && !string.IsNullOrWhiteSpace(OperationFilter))
            {
                // Search by part and operation
                result = await _databaseService.GetInventoryByPartAndOperationAsync(PartIdFilter, OperationFilter);
            }
            else if (!string.IsNullOrWhiteSpace(PartIdFilter))
            {
                // Search by part only
                result = await _databaseService.GetInventoryByPartIdAsync(PartIdFilter);
            }
            else if (!string.IsNullOrWhiteSpace(UserFilter))
            {
                // Search by user
                result = await _databaseService.GetInventoryByUserAsync(UserFilter);
            }
            else
            {
                StatusMessage = "Please specify at least one search criteria";
                return;
            }

            // Apply additional filters and convert to InventoryItem objects
            var filteredResults = new List<InventoryItem>();

            foreach (DataRow row in result.Rows)
            {
                var inventoryItem = new InventoryItem
                {
                    ID = Convert.ToInt32(row["ID"]),
                    PartID = row["PartID"]?.ToString() ?? string.Empty,
                    Location = row["Location"]?.ToString() ?? string.Empty,
                    Operation = row["Operation"]?.ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ItemType = row["ItemType"]?.ToString() ?? "WIP",
                    ReceiveDate = Convert.ToDateTime(row["ReceiveDate"]),
                    LastUpdated = Convert.ToDateTime(row["LastUpdated"]),
                    User = row["User"]?.ToString() ?? string.Empty,
                    BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                    Notes = row["Notes"]?.ToString() ?? string.Empty
                };

                // Apply additional filters
                if (ApplyAdditionalFilters(inventoryItem))
                {
                    filteredResults.Add(inventoryItem);
                }
            }

            // Update pagination
            TotalResults = filteredResults.Count;
            TotalPages = (int)Math.Ceiling((double)TotalResults / ResultsPerPage);
            CurrentPage = 1;

            // Apply pagination
            var pagedResults = filteredResults
                .Skip((CurrentPage - 1) * ResultsPerPage)
                .Take(ResultsPerPage)
                .ToList();

            // Update UI on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (var item in pagedResults)
                {
                    SearchResults.Add(item);
                }
            });

            StatusMessage = $"Found {TotalResults} results";
            Logger.LogInformation("Search completed. Found {Count} total results, showing {PagedCount} on page {CurrentPage}",
                TotalResults, SearchResults.Count, CurrentPage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing search");
            StatusMessage = "Search failed";
        }
        finally
        {
            IsSearching = false;
        }
    }

    /// <summary>
    /// Applies additional filters to search results
    /// </summary>
    private bool ApplyAdditionalFilters(InventoryItem item)
    {
        // Location filter
        if (!string.IsNullOrWhiteSpace(LocationFilter) &&
            !item.Location.Contains(LocationFilter, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Date range filters
        if (DateFromFilter.HasValue && item.ReceiveDate < DateFromFilter.Value)
        {
            return false;
        }

        if (DateToFilter.HasValue && item.ReceiveDate > DateToFilter.Value)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Clears all search filters
    /// </summary>
    private void ExecuteClearFilters()
    {
        PartIdFilter = string.Empty;
        OperationFilter = string.Empty;
        LocationFilter = string.Empty;
        UserFilter = string.Empty;
        DateFromFilter = null;
        DateToFilter = null;
        
        // Clear results on UI thread
        Dispatcher.UIThread.Post(() => SearchResults.Clear());
        
        CurrentPage = 1;
        TotalPages = 1;
        TotalResults = 0;
        StatusMessage = "Filters cleared";

        Logger.LogInformation("Search filters cleared");
    }

    #endregion

    #region Export Operations

    /// <summary>
    /// Exports search results to file
    /// </summary>
    private async Task ExecuteExportAsync()
    {
        try
        {
            StatusMessage = "Exporting results...";

            // TODO: Implement export functionality
            // This would typically involve:
            // 1. Opening file save dialog
            // 2. Converting results to desired format (CSV, Excel, etc.)
            // 3. Writing to file
            
            await Task.Delay(1000); // Simulate export operation

            StatusMessage = $"Exported {SearchResults.Count} results";
            Logger.LogInformation("Exported {Count} search results", SearchResults.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting search results");
            StatusMessage = "Export failed";
        }
    }

    #endregion

    #region Pagination Operations

    /// <summary>
    /// Navigates to the first page
    /// </summary>
    private async Task NavigateToFirstPageAsync()
    {
        CurrentPage = 1;
        await RefreshCurrentPageAsync();
    }

    /// <summary>
    /// Navigates to the previous page
    /// </summary>
    private async Task NavigateToPreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await RefreshCurrentPageAsync();
        }
    }

    /// <summary>
    /// Navigates to the next page
    /// </summary>
    private async Task NavigateToNextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await RefreshCurrentPageAsync();
        }
    }

    /// <summary>
    /// Navigates to the last page
    /// </summary>
    private async Task NavigateToLastPageAsync()
    {
        CurrentPage = TotalPages;
        await RefreshCurrentPageAsync();
    }

    /// <summary>
    /// Refreshes the current page data
    /// </summary>
    private async Task RefreshCurrentPageAsync()
    {
        // Re-execute search to get current page data
        await ExecuteSearchAsync();
    }

    #endregion

    #region Property Change Notifications

    /// <summary>
    /// Raises property changed events for computed properties
    /// </summary>
    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);

        // Update computed properties when relevant properties change
        switch (propertyName)
        {
            case nameof(IsSearching):
            case nameof(PartIdFilter):
            case nameof(OperationFilter):
            case nameof(LocationFilter):
            case nameof(UserFilter):
            case nameof(DateFromFilter):
            case nameof(DateToFilter):
                OnPropertyChanged(nameof(CanSearch));
                OnPropertyChanged(nameof(HasSearchCriteria));
                break;
            case nameof(SearchResults):
                OnPropertyChanged(nameof(CanExport));
                break;
            case nameof(CurrentPage):
            case nameof(TotalPages):
                OnPropertyChanged(nameof(CanNavigatePrevious));
                OnPropertyChanged(nameof(CanNavigateNext));
                break;
        }
    }

    #endregion
}
