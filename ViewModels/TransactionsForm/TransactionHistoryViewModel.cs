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
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class TransactionHistoryViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

    #region Private Fields
    private ObservableCollection<TransactionRecord> _transactions = new();
    private TransactionRecord? _selectedTransaction;
    private string _searchText = string.Empty;
    private string _selectedUser = string.Empty;
    private DateTime _startDate = DateTime.Today.AddDays(-30);
    private DateTime _endDate = DateTime.Today;
    private bool _isLoading = false;
    private string _statusMessage = string.Empty;
    private int _totalTransactions = 0;
    private int _currentPage = 1;
    private int _itemsPerPage = 100;
    private ObservableCollection<string> _availableUsers = new();
    #endregion

    #region Public Properties
    public ObservableCollection<TransactionRecord> Transactions
    {
        get => _transactions;
        set => SetProperty(ref _transactions, value);
    }

    public TransactionRecord? SelectedTransaction
    {
        get => _selectedTransaction;
        set => SetProperty(ref _selectedTransaction, value);
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public string SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
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

    public int TotalTransactions
    {
        get => _totalTransactions;
        set => SetProperty(ref _totalTransactions, value);
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

    public ObservableCollection<string> AvailableUsers
    {
        get => _availableUsers;
        set => SetProperty(ref _availableUsers, value);
    }

    public string DisplayInfo => $"Showing {Transactions.Count} of {TotalTransactions} transactions (Page {CurrentPage})";
    #endregion

    #region Commands
    public ICommand LoadTransactionsCommand { get; private set; }
    public ICommand SearchCommand { get; private set; }
    public ICommand RefreshCommand { get; private set; }
    public ICommand FilterByUserCommand { get; private set; }
    public ICommand FilterByDateCommand { get; private set; }
    public ICommand ClearFiltersCommand { get; private set; }
    public ICommand FirstPageCommand { get; private set; }
    public ICommand PreviousPageCommand { get; private set; }
    public ICommand NextPageCommand { get; private set; }
    public ICommand LastPageCommand { get; private set; }
    public ICommand ViewDetailsCommand { get; private set; }
    public ICommand ExportCommand { get; private set; }
    #endregion

    public TransactionHistoryViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<TransactionHistoryViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("TransactionHistoryViewModel initialized with dependency injection");

        InitializeCommands();
        _ = LoadInitialDataAsync(); // Load initial data
    }

    private void InitializeCommands()
    {
        LoadTransactionsCommand = new AsyncCommand(LoadTransactionsAsync);
        SearchCommand = new AsyncCommand(ExecuteSearchAsync);
        RefreshCommand = new AsyncCommand(ExecuteRefreshAsync);
        FilterByUserCommand = new AsyncCommand(ExecuteFilterByUserAsync);
        FilterByDateCommand = new AsyncCommand(ExecuteFilterByDateAsync);
        ClearFiltersCommand = new RelayCommand(ExecuteClearFilters);
        FirstPageCommand = new AsyncCommand(ExecuteFirstPageAsync, () => CurrentPage > 1);
        PreviousPageCommand = new AsyncCommand(ExecutePreviousPageAsync, () => CurrentPage > 1);
        NextPageCommand = new AsyncCommand(ExecuteNextPageAsync, () => Transactions.Count == ItemsPerPage);
        LastPageCommand = new AsyncCommand(ExecuteLastPageAsync, () => Transactions.Count == ItemsPerPage);
        ViewDetailsCommand = new AsyncCommand<TransactionRecord>(ExecuteViewDetailsAsync);
        ExportCommand = new AsyncCommand(ExecuteExportAsync);

        Logger.LogDebug("Commands initialized for TransactionHistoryViewModel");
    }

    private async Task LoadInitialDataAsync()
    {
        await LoadUsersAsync();
        await LoadTransactionsAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            Logger.LogDebug("Loading available users for transaction history");

            var usersData = await _databaseService.GetAllUsersAsync();
            
            // Update collection on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var users = new ObservableCollection<string> { "All Users" }; // Add default option
                
                foreach (DataRow row in usersData.Rows)
                {
                    if (row["username"] != null)
                    {
                        users.Add(row["username"].ToString() ?? string.Empty);
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
            await ErrorHandling.HandleErrorAsync(
                ex,
                "Load Users",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadUsersAsync" });
        }
    }

    private async Task LoadTransactionsAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading transaction history...";
            Logger.LogInformation("Loading transaction history for page {Page}", CurrentPage);

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
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Transactions = new ObservableCollection<TransactionRecord>(pagedTransactions);
            });
            
            StatusMessage = $"Loaded {Transactions.Count} transactions";
            Logger.LogInformation("Successfully loaded {Count} transactions (Page {Page} of {TotalTransactions} total)", 
                Transactions.Count, CurrentPage, TotalTransactions);

            OnPropertyChanged(nameof(DisplayInfo));
        }
        catch (Exception ex)
        {
            StatusMessage = "Error loading transaction history.";
            Logger.LogError(ex, "Error loading transaction history");
            
            await ErrorHandling.HandleErrorAsync(
                ex,
                "Load Transaction History",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "LoadTransactionsAsync" });
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static List<TransactionRecord> ConvertDataTableToTransactionRecords(DataTable dataTable)
    {
        var transactions = new List<TransactionRecord>();
        
        foreach (DataRow row in dataTable.Rows)
        {
            var transaction = new TransactionRecord
            {
                TransactionId = Convert.ToInt32(row["transaction_id"] ?? 0),
                PartId = row["part_id"]?.ToString() ?? string.Empty,
                Location = row["location"]?.ToString() ?? string.Empty,
                Operation = row["operation"]?.ToString() ?? string.Empty,
                Quantity = Convert.ToInt32(row["quantity"] ?? 0),
                TransactionType = row["transaction_type"]?.ToString() ?? string.Empty,
                TransactionDate = Convert.ToDateTime(row["transaction_date"] ?? DateTime.Now),
                User = row["user"]?.ToString() ?? string.Empty,
                BatchNumber = row["batch_number"]?.ToString() ?? string.Empty,
                Notes = row["notes"]?.ToString() ?? string.Empty
            };
            transactions.Add(transaction);
        }
        
        return transactions;
    }

    private async Task ExecuteSearchAsync()
    {
        CurrentPage = 1; // Reset to first page when searching
        await LoadTransactionsAsync();
    }

    private async Task ExecuteRefreshAsync()
    {
        await LoadTransactionsAsync();
    }

    private async Task ExecuteFilterByUserAsync()
    {
        CurrentPage = 1; // Reset to first page when filtering
        await LoadTransactionsAsync();
    }

    private async Task ExecuteFilterByDateAsync()
    {
        CurrentPage = 1; // Reset to first page when filtering
        await LoadTransactionsAsync();
    }

    private void ExecuteClearFilters()
    {
        SearchText = string.Empty;
        SelectedUser = "All Users";
        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
        CurrentPage = 1;
        
        _ = LoadTransactionsAsync();
        
        Logger.LogDebug("Filters cleared in TransactionHistoryViewModel");
    }

    private async Task ExecuteFirstPageAsync()
    {
        CurrentPage = 1;
        await LoadTransactionsAsync();
    }

    private async Task ExecutePreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadTransactionsAsync();
        }
    }

    private async Task ExecuteNextPageAsync()
    {
        CurrentPage++;
        await LoadTransactionsAsync();
    }

    private async Task ExecuteLastPageAsync()
    {
        var lastPage = (int)Math.Ceiling((double)TotalTransactions / ItemsPerPage);
        CurrentPage = lastPage;
        await LoadTransactionsAsync();
    }

    private async Task ExecuteViewDetailsAsync(TransactionRecord? transaction)
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
            await ErrorHandling.HandleErrorAsync(
                ex,
                "View Transaction Details",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["TransactionId"] = transaction?.TransactionId.ToString() ?? "Unknown" });
        }
    }

    private async Task ExecuteExportAsync()
    {
        try
        {
            StatusMessage = "Exporting transaction history...";
            Logger.LogInformation("Exporting transaction history");

            // TODO: Implement export functionality (CSV, Excel, etc.)
            StatusMessage = "Export functionality not yet implemented.";
            
            Logger.LogInformation("Export functionality placeholder executed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting transaction history");
            await ErrorHandling.HandleErrorAsync(
                ex,
                "Export Transaction History",
                _applicationState.CurrentUser ?? "System",
                new Dictionary<string, object> { ["Operation"] = "ExecuteExportAsync" });
        }
    }
}
