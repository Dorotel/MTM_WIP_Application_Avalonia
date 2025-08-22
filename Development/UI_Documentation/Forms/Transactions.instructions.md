# Transactions - Transaction History and Management Interface

## Overview

**Transactions** is a comprehensive transaction history and management interface that provides advanced search, filtering, pagination, and reporting capabilities for all inventory operations in the MTM WIP Application. This form enables users to view, search, analyze, and report on historical inventory transactions with sophisticated filtering options and multiple viewing modes.

## UI Component Structure

### Primary Layout
```
Transactions (Full Window)
├── Transactions_GroupBox_Main
│   └── Transactions_TableLayout_Main
│       ├── Transactions_SplitContainer_Main
│       │   ├── Panel1 (Inputs/Filters)
│       │   │   └── Transactions_TableLayout_Inputs
│       │   │       ├── Smart Search Panel
│       │   │       ├── Transaction Type Filters
│       │   │       ├── Time Range Filters  
│       │   │       ├── Building Filter
│       │   │       ├── Sort Options
│       │   │       ├── Part ID Search
│       │   │       ├── User Filter
│       │   │       ├── Date Range Picker
│       │   │       └── View Mode Selection
│       │   └── Panel2 (Results)
│       │       └── Transactions_DataGridView_Transactions
│       └── Bottom Panel
│           ├── Pagination Controls
│           ├── Reset Button
│           └── Print Button
```

### Smart Search Panel
```
Transactions_Panel_Row_SmartSearch
├── Transactions_Label_SmartSearch - "Smart Search:"
└── Transactions_TextBox_SmartSearch - Real-time search input
```

### Transaction Type Filters
```
Transactions_Panel_TransactionTypes
├── Transactions_CheckBox_IN - "IN" transactions filter
├── Transactions_CheckBox_OUT - "OUT" transactions filter
└── Transactions_CheckBox_TRANSFER - "TRANSFER" transactions filter
```

### Time Range Filters
```
Transactions_Panel_TimeRange
├── Transactions_Radio_Today - Today's transactions
├── Transactions_Radio_ThisWeek - Current week transactions
├── Transactions_Radio_ThisMonth - Current month transactions
└── Transactions_Radio_Everything - All transactions
```

### View Mode Controls
```
Transactions_Panel_ViewMode
├── Transactions_Label_ViewMode - "View Mode:"
├── Transactions_Radio_GridView - Traditional grid display
├── Transactions_Radio_ChartView - Chart visualization
└── Transactions_Radio_TimelineView - Timeline visualization
```

### Advanced Filters
```
Building Filter:
├── Transactions_Label_Building
└── Transactions_ComboBox_Building

Sort Options:
├── Transactions_Label_SortBy
└── Transactions_ComboBox_SortBy

Part ID Search:
├── Transactions_Label_SearchPartID  
└── _transactionsComboBoxSearchPartId

User Filter:
├── Transactions_Label_User
└── Transactions_ComboBox_UserFullName

Date Range:
├── Control_AdvancedRemove_CheckBox_Date - Enable date range
├── Control_AdvancedRemove_DateTimePicker_From - Start date
├── Control_AdvancedRemove_Label_DateDash - " - "
└── Control_AdvancedRemove_DateTimePicker_To - End date
```

## Business Logic Integration

### Search Infrastructure
```csharp
// Smart search with debouncing for performance
private readonly System.Windows.Forms.Timer _searchDebounceTimer = new() { Interval = 500 };
private string _lastSearchText = string.Empty;
private CancellationTokenSource _searchCancellation = new();

private void InitializeSmartSearch()
{
    Transactions_TextBox_SmartSearch.TextChanged += OnSmartSearchTextChanged;
    
    // Filter change events with async handling
    Transactions_CheckBox_IN.CheckedChanged += async (s, e) => await HandleFilterChangeAsync();
    Transactions_CheckBox_OUT.CheckedChanged += async (s, e) => await HandleFilterChangeAsync();
    Transactions_CheckBox_TRANSFER.CheckedChanged += async (s, e) => await HandleFilterChangeAsync();
    
    // Time range filters
    Transactions_Radio_Today.CheckedChanged += async (s, e) => await HandleFilterChangeAsync();
    Transactions_Radio_ThisWeek.CheckedChanged += async (s, e) => await HandleFilterChangeAsync();
    Transactions_Radio_ThisMonth.CheckedChanged += async (s, e) => await HandleFilterChangeAsync();
}
```

### Pagination System
```csharp
private int _currentPage = 1;
private const int PageSize = 20;
private const bool SortDescending = true;

public int CurrentPage 
{ 
    get => _currentPage; 
    private set => _currentPage = Math.Max(1, value); 
}

// Pagination button handlers
Transfer_Button_Next.Click += async (s, e) => {
    CurrentPage++;
    await LoadTransactionsAsync();
};

Transfer_Button_Previous.Click += async (s, e) => {
    if (CurrentPage > 1) {
        CurrentPage--;
        await LoadTransactionsAsync();
    }
};
```

### View Mode Management
```csharp
private TransactionViewMode _currentViewMode = TransactionViewMode.Grid;

private async Task HandleViewModeChangeAsync()
{
    if (Transactions_Radio_GridView.Checked)
        _currentViewMode = TransactionViewMode.Grid;
    else if (Transactions_Radio_ChartView.Checked)
        _currentViewMode = TransactionViewMode.Chart;
    else if (Transactions_Radio_TimelineView.Checked)
        _currentViewMode = TransactionViewMode.Timeline;
        
    await RefreshDisplayAsync();
}
```

## Database Operations

### Transaction Loading with Advanced Filtering
```csharp
private async Task LoadTransactionsAsync()
{
    try
    {
        _progressHelper?.ShowProgress("Loading transactions...");
        _progressHelper?.UpdateProgress(10, "Preparing search criteria...");

        DataTable dt = new();
        MySqlCommand cmd;

        // Build dynamic SQL based on filters
        StringBuilder sqlBuilder = new StringBuilder();
        sqlBuilder.Append("SELECT * FROM sys_transactions_history WHERE 1=1");

        // Apply transaction type filters
        List<string> transactionTypes = new();
        if (Transactions_CheckBox_IN.Checked) transactionTypes.Add("'IN'");
        if (Transactions_CheckBox_OUT.Checked) transactionTypes.Add("'OUT'");
        if (Transactions_CheckBox_TRANSFER.Checked) transactionTypes.Add("'TRANSFER'");
        
        if (transactionTypes.Any())
        {
            sqlBuilder.Append($" AND TransactionType IN ({string.Join(",", transactionTypes)})");
        }

        // Apply time range filters
        if (Transactions_Radio_Today.Checked)
        {
            sqlBuilder.Append(" AND DATE(TransactionDateTime) = CURDATE()");
        }
        else if (Transactions_Radio_ThisWeek.Checked)
        {
            sqlBuilder.Append(" AND YEARWEEK(TransactionDateTime, 1) = YEARWEEK(CURDATE(), 1)");
        }
        else if (Transactions_Radio_ThisMonth.Checked)
        {
            sqlBuilder.Append(" AND YEAR(TransactionDateTime) = YEAR(CURDATE()) AND MONTH(TransactionDateTime) = MONTH(CURDATE())");
        }

        // Apply smart search
        if (!string.IsNullOrEmpty(Transactions_TextBox_SmartSearch.Text))
        {
            string searchText = Transactions_TextBox_SmartSearch.Text;
            sqlBuilder.Append($" AND (PartID LIKE '%{searchText}%' OR Operation LIKE '%{searchText}%' OR Location LIKE '%{searchText}%' OR UserName LIKE '%{searchText}%')");
        }

        // Apply pagination
        sqlBuilder.Append($" ORDER BY TransactionDateTime DESC LIMIT {PageSize} OFFSET {(CurrentPage - 1) * PageSize}");

        cmd = new MySqlCommand(sqlBuilder.ToString(), connectionString);
        dt = await Helper_Database_StoredProcedure.ExecuteDataTableAsync(cmd);

        _displayedTransactions = new BindingList<Model_Transactions>(
            dt.AsEnumerable().Select(row => new Model_Transactions
            {
                TransactionID = row.Field<int>("TransactionID"),
                PartID = row.Field<string>("PartID") ?? string.Empty,
                Operation = row.Field<string>("Operation") ?? string.Empty,
                Location = row.Field<string>("Location") ?? string.Empty,
                Quantity = row.Field<decimal>("Quantity"),
                TransactionType = row.Field<string>("TransactionType") ?? string.Empty,
                TransactionDateTime = row.Field<DateTime>("TransactionDateTime"),
                UserName = row.Field<string>("UserName") ?? string.Empty,
                BatchNumber = row.Field<string>("BatchNumber") ?? string.Empty
            }).ToList()
        );

        Transactions_DataGridView_Transactions.DataSource = _displayedTransactions;
        
        _progressHelper?.HideProgress();
    }
    catch (Exception ex)
    {
        _progressHelper?.HideProgress();
        Service_ErrorHandler.HandleException(ex, ErrorSeverity.High,
            source: "LoadTransactionsAsync");
    }
}
```

### Performance Optimization with Debounced Search
```csharp
private void OnSmartSearchTextChanged(object sender, EventArgs e)
{
    _searchDebounceTimer.Stop();
    _searchDebounceTimer.Start();
}

private async Task PerformDebouncedSearchAsync()
{
    try
    {
        string currentText = Transactions_TextBox_SmartSearch.Text;
        if (currentText != _lastSearchText)
        {
            _lastSearchText = currentText;
            _searchCancellation.Cancel();
            _searchCancellation = new CancellationTokenSource();
            
            await LoadTransactionsAsync();
        }
    }
    catch (OperationCanceledException)
    {
        // Search was cancelled, this is expected
    }
    catch (Exception ex)
    {
        Service_ErrorHandler.HandleException(ex, ErrorSeverity.Medium,
            source: "PerformDebouncedSearchAsync");
    }
}
```

## User Interaction Flows

### Search and Filter Flow
1. **Smart Search Input**: User types in search box with 500ms debounce
2. **Filter Selection**: User selects transaction types, time ranges, or other filters
3. **Automatic Refresh**: Changes trigger `HandleFilterChangeAsync()` which calls `LoadTransactionsAsync()`
4. **Progress Feedback**: Progress bar shows search progress
5. **Result Display**: Filtered results populate the DataGridView

### Pagination Flow
1. **Page Navigation**: User clicks Next/Previous buttons
2. **Page Validation**: CurrentPage property ensures valid page numbers
3. **Data Loading**: New page data loaded via `LoadTransactionsAsync()`
4. **UI Update**: DataGridView refreshed with new page data

### View Mode Switch Flow
1. **Mode Selection**: User selects Grid/Chart/Timeline view
2. **Mode Change**: `HandleViewModeChangeAsync()` processes the change
3. **Display Refresh**: `RefreshDisplayAsync()` updates the visualization
4. **State Persistence**: Current view mode saved for session

## Integration Points

### Progress System Integration
```csharp
private Helper_StoredProcedureProgress? _progressHelper;

// Progress injection from parent
public void SetProgressControls(Helper_StoredProcedureProgress progressHelper)
{
    _progressHelper = progressHelper;
}

// Progress reporting during operations
_progressHelper?.ShowProgress("Loading transactions...");
_progressHelper?.UpdateProgress(50, "Processing filters...");
_progressHelper?.HideProgress();
```

### User Context Integration
```csharp
private readonly string _currentUser;
private readonly bool _isAdmin;

public Transactions()
{
    _currentUser = Model_AppVariables.User;
    _isAdmin = Model_AppVariables.UserRole == "Admin";
    
    // Admin users see all transactions, regular users see their own
    if (!_isAdmin)
    {
        // Filter by current user automatically
        ApplyUserFilter(_currentUser);
    }
}
```

### Print System Integration
```csharp
private void Transactions_Button_Print_Click(object sender, EventArgs e)
{
    try
    {
        if (_displayedTransactions?.Count > 0)
        {
            Core_DgvPrinter printer = new Core_DgvPrinter();
            printer.Title = "Transaction History Report";
            printer.SubTitle = $"Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            printer.PageNumbers = true;
            printer.PorportionalColumns = true;
            printer.HeaderCellAlignment = StringAlignment.Center;
            printer.Footer = $"User: {_currentUser} | Total Records: {_displayedTransactions.Count}";
            printer.FooterSpacing = 15;
            
            printer.PrintDataGridView(Transactions_DataGridView_Transactions);
        }
        else
        {
            MessageBox.Show("No transactions to print.", "Print", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    catch (Exception ex)
    {
        Service_ErrorHandler.HandleException(ex, ErrorSeverity.Medium,
            source: "Print Transaction Report");
    }
}
```

## Security and Access Control

### User-Based Filtering
- **Admin Users**: Can view all transactions across all users
- **Regular Users**: Can only view their own transactions
- **Role Validation**: User permissions checked at form load

### Data Protection
- **SQL Injection Prevention**: Parameterized queries and input sanitization
- **Access Logging**: All transaction views logged for audit purposes
- **Sensitive Data**: Personal information filtered based on user privileges

## Performance Considerations

### Search Optimization
- **Debounced Search**: 500ms delay prevents excessive database calls
- **Cancellation Tokens**: Cancel in-flight searches when new ones start
- **Indexed Queries**: Database operations use optimized indexes
- **Pagination**: Limits result sets to improve performance

### Memory Management
```csharp
private void ResetFilters()
{
    // Clear search cache
    _lastSearchText = string.Empty;
    _searchCancellation?.Cancel();
    _searchCancellation = new CancellationTokenSource();
    
    // Reset UI controls
    Transactions_TextBox_SmartSearch.Clear();
    Transactions_CheckBox_IN.Checked = true;
    Transactions_CheckBox_OUT.Checked = true;
    Transactions_CheckBox_TRANSFER.Checked = true;
    Transactions_Radio_Today.Checked = true;
    
    // Reset pagination
    CurrentPage = 1;
}
```

### Async Operation Management
- **Non-blocking UI**: All database operations run asynchronously
- **Progress Feedback**: Visual progress indicators for long operations
- **Error Recovery**: Graceful handling of failed operations

## Error Handling and Recovery

### Exception Management
```csharp
catch (Exception ex)
{
    _progressHelper?.HideProgress();
    Service_ErrorHandler.HandleException(ex, ErrorSeverity.High,
        source: "LoadTransactionsAsync",
        additionalData: new Dictionary<string, object>
        {
            ["CurrentPage"] = CurrentPage,
            ["SearchText"] = Transactions_TextBox_SmartSearch.Text,
            ["FiltersApplied"] = GetActiveFilters()
        });
}
```

### User Feedback Systems
- **Status Messages**: Real-time feedback on operation status
- **Error Dialogs**: User-friendly error messages with recovery options
- **Progress Indicators**: Visual feedback for long-running operations

## Data Models

### Transaction Model
```csharp
public class Model_Transactions
{
    public int TransactionID { get; set; }
    public string PartID { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string TransactionType { get; set; } = string.Empty; // IN, OUT, TRANSFER
    public DateTime TransactionDateTime { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
}
```

### View Mode Enumeration
```csharp
public enum TransactionViewMode
{
    Grid,
    Chart,
    Timeline
}
```

This Transactions form provides comprehensive transaction history management with advanced search capabilities, multiple view modes, and robust performance optimizations, serving as the primary interface for analyzing inventory transaction patterns and generating reports.