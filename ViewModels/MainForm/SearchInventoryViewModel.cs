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
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for advanced inventory search interface.
/// Provides comprehensive search capabilities with multiple filter criteria,
/// pagination, export functionality, and integration with MTM business operations.
/// Uses MVVM Community Toolkit for modern .NET patterns.
/// </summary>
public partial class SearchInventoryViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<SearchInventoryViewModel> _logger;

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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    private string _partIdFilter = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    [StringLength(50, ErrorMessage = "Operation cannot exceed 50 characters")]
    private string _operationFilter = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    [StringLength(50, ErrorMessage = "Location cannot exceed 50 characters")]
    private string _locationFilter = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    [StringLength(50, ErrorMessage = "User cannot exceed 50 characters")]
    private string _userFilter = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    private DateTime? _dateFromFilter;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    private DateTime? _dateToFilter;

    #endregion

    #region Pagination Properties

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateToFirstPageCommand), nameof(NavigateToPreviousPageCommand))]
    [Range(1, int.MaxValue, ErrorMessage = "Current page must be 1 or greater")]
    private int _currentPage = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateToFirstPageCommand), nameof(NavigateToPreviousPageCommand), nameof(NavigateToNextPageCommand), nameof(NavigateToLastPageCommand))]
    [Range(1, int.MaxValue, ErrorMessage = "Total pages must be 1 or greater")]
    private int _totalPages = 1;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Total results must be 0 or greater")]
    private int _totalResults = 0;

    [ObservableProperty]
    [Range(1, 1000, ErrorMessage = "Results per page must be between 1 and 1000")]
    private int _resultsPerPage = 50;

    #endregion

    #region Selection Properties

    [ObservableProperty]
    private InventoryItem? _selectedResult;

    #endregion

    #region Status Properties

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSearchCommand))]
    private bool _isSearching = false;

    [ObservableProperty]
    [StringLength(500, ErrorMessage = "Status message cannot exceed 500 characters")]
    private string _statusMessage = "Ready to search";

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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogInformation("SearchInventoryViewModel initialized with dependency injection");

        _ = LoadMasterDataAsync(); // Load dropdown data from database
    }

    #endregion



    #region Data Loading

    /// <summary>
    /// Loads master data for dropdown controls
    /// </summary>
    [RelayCommand]
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
                Dispatcher.UIThread.Post(() =>
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
                Dispatcher.UIThread.Post(() =>
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
                Dispatcher.UIThread.Post(() =>
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
    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task ExecuteSearchAsync()
    {
        try
        {
            IsSearching = true;
            StatusMessage = "Searching...";

            // Clear results on UI thread
            Dispatcher.UIThread.Post(() => SearchResults.Clear());

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
            Dispatcher.UIThread.Post(() =>
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
    [RelayCommand]
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
    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExecuteExportAsync()
    {
        try
        {
            StatusMessage = "Exporting results...";

            // Implemented export functionality
            var csvContent = GenerateCSVContent();
            var fileName = $"Inventory_Search_Results_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.csv";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            
            await File.WriteAllTextAsync(filePath, csvContent);

            StatusMessage = $"Exported {SearchResults.Count} results to {fileName}";
            Logger.LogInformation("Exported {Count} search results to {FilePath}", SearchResults.Count, filePath);
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
    [RelayCommand(CanExecute = nameof(CanNavigatePrevious))]
    private async Task NavigateToFirstPageAsync()
    {
        CurrentPage = 1;
        await RefreshCurrentPageAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Navigates to the previous page
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanNavigatePrevious))]
    private async Task NavigateToPreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await RefreshCurrentPageAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Navigates to the next page
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanNavigateNext))]
    private async Task NavigateToNextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await RefreshCurrentPageAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Navigates to the last page
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanNavigateNext))]
    private async Task NavigateToLastPageAsync()
    {
        CurrentPage = TotalPages;
        await RefreshCurrentPageAsync().ConfigureAwait(false);
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

    #region Export Functionality

    /// <summary>
    /// Generates CSV content from current search results.
    /// </summary>
    private string GenerateCSVContent()
    {
        var csv = new StringBuilder();
        
        // Add CSV header
        csv.AppendLine("PartID,Operation,Location,Quantity,Item Type,User,Timestamp,Notes");
        
        // Add data rows
        foreach (var result in SearchResults)
        {
            var line = $"{EscapeCsvField(result.PartID ?? string.Empty)}," +
                      $"{EscapeCsvField(result.Operation ?? string.Empty)}," +
                      $"{EscapeCsvField(result.Location ?? string.Empty)}," +
                      $"{result.Quantity}," +
                      $"{EscapeCsvField(result.ItemType ?? string.Empty)}," +
                      $"{EscapeCsvField(result.User ?? string.Empty)}," +
                      $"{result.LastUpdated:yyyy-MM-dd HH:mm:ss}," +
                      $"{EscapeCsvField(result.Notes ?? string.Empty)}";
            
            csv.AppendLine(line);
        }
        
        return csv.ToString();
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
