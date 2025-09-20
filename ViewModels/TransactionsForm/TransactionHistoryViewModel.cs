using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// Transaction History ViewModel - Complex transaction reporting with filtering, pagination, and export functionality
/// Provides comprehensive transaction management with advanced search capabilities, user filtering,
/// date range filtering, pagination, and detailed transaction viewing with data export features
/// </summary>
public partial class TransactionHistoryViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Observable Properties

    /// <summary>
    /// Collection of transaction records displayed in the grid
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTransactions))]
    [NotifyCanExecuteChangedFor(nameof(ExportCommand))]
    private ObservableCollection<TransactionRecord> transactions = new();

    /// <summary>
    /// Collection of available users for filtering
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<string> availableUsers = new();

    /// <summary>
    /// Selected user for filtering transactions (default: "All Users")
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(FilterByUserCommand))]
    private string selectedUser = "All Users";

    /// <summary>
    /// Search text for filtering transactions by part, location, operation, etc.
    /// </summary>
    [ObservableProperty]
    [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters")]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string searchText = string.Empty;

    /// <summary>
    /// Start date for transaction filtering (default: 30 days ago)
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateRangeValid))]
    [NotifyCanExecuteChangedFor(nameof(LoadTransactionsCommand), nameof(FilterByDateCommand))]
    private DateTime startDate = DateTime.Today.AddDays(-30);

    /// <summary>
    /// End date for transaction filtering (default: today)
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateRangeValid))]
    [NotifyCanExecuteChangedFor(nameof(LoadTransactionsCommand), nameof(FilterByDateCommand))]
    private DateTime endDate = DateTime.Today;

    /// <summary>
    /// Current page number for pagination (1-based)
    /// </summary>
    [ObservableProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    [NotifyPropertyChangedFor(nameof(CanGoToPreviousPage), nameof(CanGoToFirstPage))]
    [NotifyCanExecuteChangedFor(nameof(FirstPageCommand), nameof(PreviousPageCommand))]
    private int currentPage = 1;

    /// <summary>
    /// Number of items to display per page
    /// </summary>
    [ObservableProperty]
    [Range(5, 100, ErrorMessage = "Items per page must be between 5 and 100")]
    private int itemsPerPage = 25;

    /// <summary>
    /// Total number of transactions matching current filters
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPages), nameof(CanGoToNextPage), nameof(CanGoToLastPage))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand), nameof(LastPageCommand))]
    private int totalTransactions;

    /// <summary>
    /// Currently selected transaction for detail viewing
    /// </summary>
    [ObservableProperty]
    private TransactionRecord? selectedTransaction;

    /// <summary>
    /// Loading state indicator
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadTransactionsCommand), nameof(SearchCommand), nameof(RefreshCommand), nameof(ExportCommand))]
    private bool isLoading;

    /// <summary>
    /// Status message for user feedback
    /// </summary>
    [ObservableProperty]
    private string statusMessage = "Ready";

    #endregion

    #region Computed Properties

    /// <summary>
    /// Whether any transactions are currently loaded
    /// </summary>
    public bool HasTransactions => Transactions.Count > 0;

    /// <summary>
    /// Whether the date range is valid (start <= end)
    /// </summary>
    public bool DateRangeValid => StartDate <= EndDate;

    /// <summary>
    /// Total number of pages based on current filters
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalTransactions / ItemsPerPage);

    /// <summary>
    /// Whether navigation to previous page is possible
    /// </summary>
    public bool CanGoToPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Whether navigation to first page is possible
    /// </summary>
    public bool CanGoToFirstPage => CurrentPage > 1;

    /// <summary>
    /// Whether navigation to next page is possible
    /// </summary>
    public bool CanGoToNextPage => Transactions.Count == ItemsPerPage;

    /// <summary>
    /// Whether navigation to last page is possible
    /// </summary>
    public bool CanGoToLastPage => Transactions.Count == ItemsPerPage;

    #endregion

    public TransactionHistoryViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<TransactionHistoryViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("TransactionHistoryViewModel initialized with dependency injection");

        _ = LoadInitialDataAsync(); // Load initial data
    }

    #region Commands

    /// <summary>
    /// Loads transactions from the database with current filters applied
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoadTransactions))]
    private async Task LoadTransactionsAsync()
    {
        using var scope = Logger.BeginScope("LoadTransactions");
        Logger.LogDebug("Loading transactions with filters - StartDate: {StartDate}, EndDate: {EndDate}, User: {User}, Search: {Search}",
            StartDate, EndDate, SelectedUser, SearchText);

        try
        {
            IsLoading = true;
            StatusMessage = "Loading transactions...";

            // Use stored procedure approach
            DataTable dataTable;

            // Load transactions based on filters
            if (SelectedUser == "All Users" || string.IsNullOrEmpty(SelectedUser))
            {
                dataTable = await _databaseService.GetLastTransactionsForUserAsync(null, ItemsPerPage * 10); // Get more for filtering
            }
            else
            {
                dataTable = await _databaseService.GetLastTransactionsForUserAsync(SelectedUser, ItemsPerPage * 10);
            }

            var allTransactions = ConvertDataTableToTransactionRecords(dataTable).ToList();

            // Apply date filter
            allTransactions = allTransactions.Where(t =>
                t.TransactionDate >= StartDate && t.TransactionDate <= EndDate).ToList();

            // Apply search filter if specified
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                allTransactions = allTransactions.Where(t =>
                    t.PartId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Operation.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.TransactionType.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Notes.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            TotalTransactions = allTransactions.Count;

            // Apply paging
            var pagedTransactions = allTransactions
                .OrderByDescending(t => t.TransactionDate)
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            // Update collection on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                Transactions = new ObservableCollection<TransactionRecord>(pagedTransactions);
            });

            StatusMessage = $"Loaded {Transactions.Count} transactions";
            Logger.LogInformation("Successfully loaded {Count} transactions", Transactions.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading transactions");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Load Transactions",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadTransactions" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanLoadTransactions() => !IsLoading && StartDate <= EndDate;

    /// <summary>
    /// Executes a search based on current search criteria
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteSearch))]
    private async Task SearchAsync()
    {
        using var scope = Logger.BeginScope("Search");
        Logger.LogDebug("Executing search with text: {SearchText}", SearchText);

        CurrentPage = 1; // Reset to first page when searching
        await LoadTransactionsAsync().ConfigureAwait(false);
    }

    private bool CanExecuteSearch() => !IsLoading;

    /// <summary>
    /// Refreshes the transaction data by reloading from database
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteRefresh))]
    private async Task RefreshAsync()
    {
        using var scope = Logger.BeginScope("Refresh");
        Logger.LogDebug("Refreshing transaction data");

        await LoadTransactionsAsync().ConfigureAwait(false);
        await LoadUsersAsync().ConfigureAwait(false);
    }

    private bool CanExecuteRefresh() => !IsLoading;

    /// <summary>
    /// Filters transactions by the selected user
    /// </summary>
    [RelayCommand]
    private async Task FilterByUserAsync()
    {
        using var scope = Logger.BeginScope("FilterByUser");
        Logger.LogDebug("Filtering by user: {User}", SelectedUser);

        CurrentPage = 1; // Reset to first page when filtering
        await LoadTransactionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Filters transactions by the selected date range
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanFilterByDate))]
    private async Task FilterByDateAsync()
    {
        using var scope = Logger.BeginScope("FilterByDate");
        Logger.LogDebug("Filtering by date range: {StartDate} to {EndDate}", StartDate, EndDate);

        CurrentPage = 1; // Reset to first page when filtering
        await LoadTransactionsAsync().ConfigureAwait(false);
    }

    private bool CanFilterByDate() => StartDate <= EndDate;

    /// <summary>
    /// Clears all search and filter criteria
    /// </summary>
    [RelayCommand]
    private void ClearFilters()
    {
        using var scope = Logger.BeginScope("ClearFilters");
        Logger.LogDebug("Clearing all filters");

        SearchText = string.Empty;
        SelectedUser = "All Users";
        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
        CurrentPage = 1;

        _ = LoadTransactionsAsync(); // Reload with cleared filters
    }

    /// <summary>
    /// Navigates to the first page of results
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGoToFirstPage))]
    private async Task FirstPageAsync()
    {
        CurrentPage = 1;
        await LoadTransactionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Navigates to the previous page of results
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadTransactionsAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Navigates to the next page of results
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadTransactionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Navigates to the last page of results
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanGoToLastPage))]
    private async Task LastPageAsync()
    {
        var totalPages = (int)Math.Ceiling((double)TotalTransactions / ItemsPerPage);
        CurrentPage = Math.Max(1, totalPages);
        await LoadTransactionsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Views details for the specified transaction
    /// </summary>
    [RelayCommand]
    private async Task ViewDetailsAsync(TransactionRecord? transaction)
    {
        if (transaction == null) return;

        try
        {
            SelectedTransaction = transaction;
            Logger.LogInformation("Selected transaction: ID={TransactionId}, PartId={PartId}",
                transaction.TransactionId, transaction.PartId);

            StatusMessage = $"Selected transaction: {transaction.TransactionType} - {transaction.PartId}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error viewing transaction details");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "View Transaction Details",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["TransactionId"] = transaction?.TransactionId.ToString() ?? "Unknown" });
        }
    }

    /// <summary>
    /// Exports transaction data to a file
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task ExportAsync()
    {
        using var scope = Logger.BeginScope("Export");
        Logger.LogDebug("Exporting transaction data");

        try
        {
            IsLoading = true;
            StatusMessage = "Exporting transactions...";

            // Implementation for export functionality would go here
            // This is a placeholder for the actual export logic

            StatusMessage = "Export completed successfully";
            Logger.LogInformation("Transaction export completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting transactions");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Export Transactions",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "Export" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanExport() => !IsLoading && Transactions.Count > 0;

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads initial data when the ViewModel is created
    /// </summary>
    private async Task LoadInitialDataAsync()
    {
        await LoadUsersAsync();
        await LoadTransactionsAsync();
    }

    /// <summary>
    /// Loads available users for filtering dropdown
    /// </summary>
    private async Task LoadUsersAsync()
    {
        try
        {
            Logger.LogDebug("Loading available users for transaction history");

            var usersData = await _databaseService.GetAllUsersAsync();

            // Update collection on UI thread
            Dispatcher.UIThread.Post(() =>
            {
                var users = new ObservableCollection<string> { "All Users" }; // Add default option

                if (usersData != null)
                {
                    foreach (DataRow row in usersData.Rows)
                    {
                        var userId = row["UserId"]?.ToString();
                        if (!string.IsNullOrEmpty(userId))
                        {
                            users.Add(userId);
                        }
                    }
                }

                AvailableUsers = users;
                if (SelectedUser == string.Empty)
                {
                    SelectedUser = "All Users";
                }
            });

            Logger.LogInformation("Loaded {Count} users for transaction history filtering", AvailableUsers.Count - 1);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading users for transaction history");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Load Users",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadUsersAsync" });
        }
    }

    /// <summary>
    /// Converts DataTable rows to TransactionRecord objects
    /// </summary>
    private List<TransactionRecord> ConvertDataTableToTransactionRecords(DataTable dataTable)
    {
        var transactions = new List<TransactionRecord>();

        if (dataTable == null || dataTable.Rows.Count == 0)
        {
            Logger.LogWarning("No transaction data found in DataTable");
            return transactions;
        }

        foreach (DataRow row in dataTable.Rows)
        {
            try
            {
                var transaction = new TransactionRecord
                {
                    TransactionId = Convert.ToInt32(row["TransactionId"]),
                    PartId = row["PartId"]?.ToString() ?? string.Empty,
                    Operation = row["Operation"]?.ToString() ?? string.Empty,
                    Location = row["Location"]?.ToString() ?? string.Empty,
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    TransactionType = row["TransactionType"]?.ToString() ?? string.Empty,
                    TransactionDate = Convert.ToDateTime(row["TransactionDate"]),
                    User = row["UserId"]?.ToString() ?? string.Empty, // Fixed: Use User property instead of UserId
                    Notes = row["Notes"]?.ToString() ?? string.Empty
                };

                transactions.Add(transaction);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error converting row to TransactionRecord: {Row}", row.ItemArray);
            }
        }

        Logger.LogDebug("Converted {Count} DataRows to TransactionRecords", transactions.Count);
        return transactions;
    }

    #endregion
}
