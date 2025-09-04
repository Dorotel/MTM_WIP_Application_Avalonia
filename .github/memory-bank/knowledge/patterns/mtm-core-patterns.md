# MTM Pattern Library

## Overview
The MTM Pattern Library documents established code patterns, best practices, and architectural decisions for the MTM WIP Application. This serves as both developer reference and GitHub Copilot context enhancement.

## MVVM Community Toolkit Patterns

### ViewModel Patterns

#### **Standard ViewModel Pattern**
```csharp
/// <summary>
/// Standard MTM ViewModel pattern with MVVM Community Toolkit integration
/// </summary>
[ObservableObject]
public partial class ExampleViewModel : BaseViewModel
{
    private readonly IExampleService _exampleService;
    private readonly ILogger<ExampleViewModel> _logger;

    public ExampleViewModel(
        ILogger<ExampleViewModel> logger,
        IExampleService exampleService) : base(logger)
    {
        _exampleService = exampleService ?? throw new ArgumentNullException(nameof(exampleService));
        _logger = logger;
        
        // Initialize collections
        Items = new ObservableCollection<ExampleItem>();
    }

    #region Properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteAction))]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    private string inputValue = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ExampleItem> items = new();

    [ObservableProperty]
    private ExampleItem? selectedItem;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    // Computed properties
    public bool CanExecuteAction => !IsLoading && !string.IsNullOrWhiteSpace(InputValue);
    public bool IsFormValid => !string.IsNullOrWhiteSpace(InputValue) && InputValue.Length >= 3;

    #endregion

    #region Commands

    [RelayCommand(CanExecute = nameof(CanExecuteAction))]
    private async Task ExecuteActionAsync()
    {
        IsLoading = true;
        ClearErrors();
        
        try
        {
            StatusMessage = "Processing...";
            
            var result = await _exampleService.ProcessAsync(InputValue);
            
            if (result.IsSuccess)
            {
                StatusMessage = "Action completed successfully";
                await RefreshDataAsync();
            }
            else
            {
                AddError(result.ErrorMessage);
                StatusMessage = "Action failed";
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, nameof(ExecuteActionAsync));
            StatusMessage = "An error occurred";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        // Refresh implementation
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializeAsync()
    {
        await RefreshDataAsync();
    }

    #endregion
}
```

**Usage Guidelines**:
- Always inherit from `BaseViewModel`
- Use `[ObservableProperty]` for all properties
- Use `[NotifyPropertyChangedFor]` for dependent properties
- Include `CanExecute` methods for commands where appropriate
- Handle exceptions with centralized error handling
- Provide user feedback through status messages and loading states

#### **Complex ViewModel with Validation Pattern**
```csharp
[ObservableObject]
public partial class InventoryFormViewModel : BaseViewModel, INotifyDataErrorInfo
{
    private readonly IInventoryService _inventoryService;
    private readonly IMasterDataService _masterDataService;

    public InventoryFormViewModel(
        ILogger<InventoryFormViewModel> logger,
        IInventoryService inventoryService,
        IMasterDataService masterDataService) : base(logger)
    {
        _inventoryService = inventoryService;
        _masterDataService = masterDataService;
        
        // Initialize validation
        _validationContext = new ValidationContext(this);
        _validationResults = new Dictionary<string, List<string>>();
    }

    #region Properties with Validation

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    private string partId = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    [NotifyDataErrorInfo]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    private int quantity = 1;

    #endregion

    #region Validation Implementation

    private readonly ValidationContext _validationContext;
    private readonly Dictionary<string, List<string>> _validationResults;

    public bool HasErrors => _validationResults.Any(r => r.Value.Any());

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _validationResults.SelectMany(r => r.Value);
            
        return _validationResults.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
    }

    partial void OnPartIdChanged(string value)
    {
        ValidateProperty(value, nameof(PartId));
    }

    partial void OnQuantityChanged(int value)
    {
        ValidateProperty(value, nameof(Quantity));
    }

    private void ValidateProperty(object value, string propertyName)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(this) { MemberName = propertyName };
        
        Validator.TryValidateProperty(value, context, results);
        
        _validationResults[propertyName] = results.Select(r => r.ErrorMessage ?? "").ToList();
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #endregion

    #region Commands

    public bool CanSave => !HasErrors && !IsLoading && !string.IsNullOrWhiteSpace(PartId) && Quantity > 0;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        // Save implementation with validation
    }

    #endregion
}
```

## Service Layer Patterns

### Standard Service Pattern
```csharp
/// <summary>
/// Standard MTM Service pattern with comprehensive error handling and logging
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly string _connectionString;
    private readonly IDatabaseService _databaseService;

    public InventoryService(
        ILogger<InventoryService> logger,
        IDatabaseService databaseService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _connectionString = _databaseService.GetConnectionString();
    }

    public async Task<ServiceResult<InventoryItem>> GetInventoryItemAsync(string partId, string operation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partId);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        try
        {
            _logger.LogInformation("Retrieving inventory for {PartId} at operation {Operation}", partId, operation);

            var parameters = new MySqlParameter[]
            {
                new("p_PartId", partId),
                new("p_Operation", operation)
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "inv_inventory_Get_ByPartIDandOperation", parameters);

            if (result.Status == 1 && result.Data.Rows.Count > 0)
            {
                var item = MapDataRowToInventoryItem(result.Data.Rows[0]);
                _logger.LogInformation("Successfully retrieved inventory item for {PartId}", partId);
                return ServiceResult<InventoryItem>.Success(item);
            }

            _logger.LogWarning("No inventory found for {PartId} at operation {Operation}", partId, operation);
            return ServiceResult<InventoryItem>.Failure("Inventory item not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory for {PartId} at operation {Operation}", partId, operation);
            return ServiceResult<InventoryItem>.Failure($"Database error: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult> UpdateInventoryAsync(InventoryUpdateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        // Validation
        if (string.IsNullOrWhiteSpace(request.PartId))
            return ServiceResult.Failure("Part ID is required");
            
        if (request.Quantity <= 0)
            return ServiceResult.Failure("Quantity must be greater than zero");

        try
        {
            _logger.LogInformation("Updating inventory for {PartId}: {Quantity} at {Location}", 
                request.PartId, request.Quantity, request.Location);

            var parameters = new MySqlParameter[]
            {
                new("p_PartId", request.PartId),
                new("p_Operation", request.Operation ?? string.Empty),
                new("p_Quantity", request.Quantity),
                new("p_Location", request.Location ?? string.Empty),
                new("p_TransactionType", request.TransactionType ?? string.Empty),
                new("p_UserId", request.UserId ?? Environment.UserName),
                new("p_Notes", request.Notes ?? string.Empty)
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "inv_inventory_Update_Item", parameters);

            if (result.Status == 1)
            {
                _logger.LogInformation("Successfully updated inventory for {PartId}", request.PartId);
                return ServiceResult.Success("Inventory updated successfully");
            }

            var errorMessage = $"Database operation failed with status: {result.Status}";
            _logger.LogWarning(errorMessage);
            return ServiceResult.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating inventory for {PartId}", request.PartId);
            return ServiceResult.Failure($"Update failed: {ex.Message}", ex);
        }
    }

    #region Private Helpers

    private static InventoryItem MapDataRowToInventoryItem(DataRow row)
    {
        return new InventoryItem
        {
            PartId = row["PartID"]?.ToString() ?? string.Empty,
            Operation = row["Operation"]?.ToString() ?? string.Empty,
            Quantity = Convert.ToInt32(row["Quantity"] ?? 0),
            Location = row["Location"]?.ToString() ?? string.Empty,
            LastUpdated = Convert.ToDateTime(row["LastUpdated"] ?? DateTime.Now),
            LastUpdatedBy = row["LastUpdatedBy"]?.ToString() ?? string.Empty
        };
    }

    #endregion
}
```

## Database Access Patterns

### Stored Procedure Pattern
```csharp
/// <summary>
/// Standard pattern for database operations using stored procedures
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _connectionString = _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    /// <summary>
    /// Execute a stored procedure with parameters and return structured result
    /// </summary>
    public async Task<DatabaseOperationResult<T>> ExecuteStoredProcedureAsync<T>(
        string procedureName, 
        MySqlParameter[] parameters,
        Func<DataTable, List<T>> mapper) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(procedureName);
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(mapper);

        try
        {
            _logger.LogDebug("Executing stored procedure {ProcedureName} with {ParameterCount} parameters", 
                procedureName, parameters.Length);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, procedureName, parameters);

            if (result.Status == 1)
            {
                var mappedData = mapper(result.Data);
                _logger.LogDebug("Successfully executed {ProcedureName}, returned {RecordCount} records", 
                    procedureName, mappedData.Count);
                
                return DatabaseOperationResult<T>.Success(mappedData);
            }

            var errorMessage = $"Stored procedure {procedureName} failed with status: {result.Status}";
            _logger.LogWarning(errorMessage);
            return DatabaseOperationResult<T>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            return DatabaseOperationResult<T>.Failure($"Database error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Execute a stored procedure for data modification (INSERT/UPDATE/DELETE)
    /// </summary>
    public async Task<DatabaseOperationResult> ExecuteNonQueryStoredProcedureAsync(
        string procedureName,
        MySqlParameter[] parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(procedureName);
        ArgumentNullException.ThrowIfNull(parameters);

        try
        {
            _logger.LogDebug("Executing non-query stored procedure {ProcedureName}", procedureName);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, procedureName, parameters);

            if (result.Status == 1)
            {
                _logger.LogDebug("Successfully executed {ProcedureName}", procedureName);
                return DatabaseOperationResult.Success($"Procedure {procedureName} executed successfully");
            }

            var errorMessage = $"Stored procedure {procedureName} failed with status: {result.Status}";
            _logger.LogWarning(errorMessage);
            return DatabaseOperationResult.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
            return DatabaseOperationResult.Failure($"Database error: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Result pattern for database operations
/// </summary>
public class DatabaseOperationResult<T> where T : class
{
    public bool IsSuccess { get; private set; }
    public List<T> Data { get; private set; } = new();
    public string Message { get; private set; } = string.Empty;
    public Exception? Exception { get; private set; }

    public static DatabaseOperationResult<T> Success(List<T> data)
        => new() { IsSuccess = true, Data = data, Message = "Operation successful" };

    public static DatabaseOperationResult<T> Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}

public class DatabaseOperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public Exception? Exception { get; private set; }

    public static DatabaseOperationResult Success(string message)
        => new() { IsSuccess = true, Message = message };

    public static DatabaseOperationResult Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}
```

## Manufacturing Domain Patterns

### Inventory Transaction Pattern
```csharp
/// <summary>
/// Pattern for handling manufacturing inventory transactions
/// </summary>
public class InventoryTransactionService : IInventoryTransactionService
{
    private readonly ILogger<InventoryTransactionService> _logger;
    private readonly IDatabaseService _databaseService;

    public InventoryTransactionService(
        ILogger<InventoryTransactionService> logger,
        IDatabaseService databaseService)
    {
        _logger = logger;
        _databaseService = databaseService;
    }

    /// <summary>
    /// Process inventory transaction based on user intent
    /// </summary>
    public async Task<ServiceResult<TransactionResult>> ProcessTransactionAsync(
        InventoryTransactionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validate manufacturing business rules
        var validationResult = ValidateTransactionRequest(request);
        if (!validationResult.IsValid)
        {
            return ServiceResult<TransactionResult>.Failure(validationResult.ErrorMessage);
        }

        try
        {
            // Determine transaction type based on user intent (not operation number)
            var transactionType = DetermineTransactionType(request.UserIntent);
            
            _logger.LogInformation("Processing {TransactionType} transaction for {PartId} at operation {Operation}",
                transactionType, request.PartId, request.Operation);

            var parameters = new MySqlParameter[]
            {
                new("p_PartId", request.PartId),
                new("p_Operation", request.Operation), // Operation is workflow step (90, 100, 110, 120)
                new("p_Quantity", request.Quantity),
                new("p_Location", request.Location),
                new("p_TransactionType", transactionType), // IN/OUT/TRANSFER based on user intent
                new("p_UserId", request.UserId ?? Environment.UserName),
                new("p_Notes", request.Notes ?? string.Empty),
                new("p_WorkOrder", request.WorkOrder ?? string.Empty)
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(), "inv_transaction_Add", parameters);

            if (result.Status == 1)
            {
                var transactionId = result.Data.Rows[0]["TransactionId"];
                var transactionResult = new TransactionResult
                {
                    TransactionId = Convert.ToInt32(transactionId),
                    TransactionType = transactionType,
                    ProcessedQuantity = request.Quantity,
                    Timestamp = DateTime.Now
                };

                _logger.LogInformation("Successfully processed transaction {TransactionId} for {PartId}",
                    transactionResult.TransactionId, request.PartId);

                return ServiceResult<TransactionResult>.Success(transactionResult);
            }

            return ServiceResult<TransactionResult>.Failure($"Transaction failed with status: {result.Status}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transaction for {PartId}", request.PartId);
            return ServiceResult<TransactionResult>.Failure($"Transaction error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Determine transaction type based on user intent, not operation number
    /// </summary>
    private static string DetermineTransactionType(UserTransactionIntent intent)
    {
        return intent switch
        {
            UserTransactionIntent.AddingStock => "IN",      // User adding inventory
            UserTransactionIntent.RemovingStock => "OUT",   // User removing inventory
            UserTransactionIntent.MovingStock => "TRANSFER", // User moving between locations/operations
            UserTransactionIntent.AdjustingStock => "ADJUSTMENT", // Stock count adjustment
            _ => throw new ArgumentOutOfRangeException(nameof(intent), $"Unsupported intent: {intent}")
        };
    }

    /// <summary>
    /// Validate manufacturing business rules
    /// </summary>
    private static ValidationResult ValidateTransactionRequest(InventoryTransactionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PartId))
            return ValidationResult.Invalid("Part ID is required");

        if (string.IsNullOrWhiteSpace(request.Operation))
            return ValidationResult.Invalid("Operation is required");

        // Validate operation numbers (manufacturing workflow steps)
        if (!IsValidOperation(request.Operation))
            return ValidationResult.Invalid($"Invalid operation: {request.Operation}. Valid operations: 90, 100, 110, 120");

        if (request.Quantity <= 0)
            return ValidationResult.Invalid("Quantity must be greater than zero");

        if (string.IsNullOrWhiteSpace(request.Location))
            return ValidationResult.Invalid("Location is required");

        return ValidationResult.Valid();
    }

    /// <summary>
    /// Check if operation number is valid manufacturing workflow step
    /// </summary>
    private static bool IsValidOperation(string operation)
    {
        var validOperations = new[] { "90", "100", "110", "120" }; // Manufacturing workflow steps
        return validOperations.Contains(operation);
    }
}

/// <summary>
/// User intent for inventory transactions (determines transaction type)
/// </summary>
public enum UserTransactionIntent
{
    AddingStock,    // User is adding inventory (results in IN transaction)
    RemovingStock,  // User is removing inventory (results in OUT transaction)  
    MovingStock,    // User is moving inventory (results in TRANSFER transaction)
    AdjustingStock  // User is correcting inventory count (results in ADJUSTMENT transaction)
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    public static ValidationResult Valid() => new() { IsValid = true };
    public static ValidationResult Invalid(string message) => new() { IsValid = false, ErrorMessage = message };
}
```

## Error Handling Patterns

### Centralized Error Handling Pattern
```csharp
/// <summary>
/// Centralized error handling service following MTM patterns
/// </summary>
public static class ErrorHandling
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder =>
        builder.AddConsole()).CreateLogger(nameof(ErrorHandling));

    /// <summary>
    /// Handle exceptions with logging, user notification, and optional database logging
    /// </summary>
    public static async Task HandleErrorAsync(Exception exception, string context, bool logToDatabase = true)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentException.ThrowIfNullOrWhiteSpace(context);

        // Structured logging
        Logger.LogError(exception, "Error in {Context}: {Message}", context, exception.Message);

        // Log to database for audit trail (if requested and possible)
        if (logToDatabase)
        {
            await TryLogErrorToDatabaseAsync(exception, context);
        }

        // Show user-friendly message (avoid technical details in production)
        var userMessage = GetUserFriendlyMessage(exception);
        await ShowUserNotificationAsync(userMessage, context);
    }

    /// <summary>
    /// Handle service result failures with appropriate logging and user notification
    /// </summary>
    public static async Task HandleServiceFailureAsync(ServiceResult result, string context)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(context);

        if (result.IsSuccess) return; // Nothing to handle

        Logger.LogWarning("Service failure in {Context}: {Message}", context, result.Message);

        if (result.Exception != null)
        {
            await HandleErrorAsync(result.Exception, context);
        }
        else
        {
            await ShowUserNotificationAsync(result.Message, context);
        }
    }

    /// <summary>
    /// Get user-friendly error message based on exception type
    /// </summary>
    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            MySqlException => "Database operation failed. Please try again or contact support.",
            TimeoutException => "The operation took too long to complete. Please try again.",
            UnauthorizedAccessException => "You don't have permission to perform this operation.",
            ArgumentException => "Invalid input provided. Please check your data and try again.",
            InvalidOperationException => "The requested operation cannot be completed at this time.",
            _ => "An unexpected error occurred. Please contact support if the problem persists."
        };
    }

    /// <summary>
    /// Attempt to log error to database (with fallback handling)
    /// </summary>
    private static async Task TryLogErrorToDatabaseAsync(Exception exception, string context)
    {
        try
        {
            var parameters = new MySqlParameter[]
            {
                new("p_ErrorMessage", exception.Message),
                new("p_StackTrace", exception.StackTrace ?? string.Empty),
                new("p_Context", context),
                new("p_UserId", Environment.UserName),
                new("p_MachineName", Environment.MachineName),
                new("p_Timestamp", DateTime.Now)
            };

            // Use a separate connection string for error logging to avoid recursive failures
            var connectionString = GetErrorLoggingConnectionString();
            await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "log_error_Add_Error", parameters);
        }
        catch (Exception logException)
        {
            // Log to system event log or file as fallback
            Logger.LogCritical(logException, "Failed to log error to database for context {Context}", context);
        }
    }

    /// <summary>
    /// Show user notification (implementation depends on UI framework)
    /// </summary>
    private static async Task ShowUserNotificationAsync(string message, string context)
    {
        // Implementation would integrate with UI notification system
        // For now, just ensure async pattern is followed
        await Task.Delay(1); // Placeholder for actual UI integration
    }

    private static string GetErrorLoggingConnectionString()
    {
        // Implementation to get connection string specifically for error logging
        // This might use a separate database or schema to avoid circular dependencies
        return ""; // Placeholder
    }
}
```

## Usage Examples and Best Practices

### Putting It All Together - Complete Feature Implementation
```csharp
/// <summary>
/// Example of complete feature implementation following all MTM patterns
/// </summary>

// ViewModel Implementation
[ObservableObject]
public partial class InventoryTransactionViewModel : BaseViewModel
{
    private readonly IInventoryTransactionService _transactionService;
    private readonly IMasterDataService _masterDataService;

    public InventoryTransactionViewModel(
        ILogger<InventoryTransactionViewModel> logger,
        IInventoryTransactionService transactionService,
        IMasterDataService masterDataService) : base(logger)
    {
        _transactionService = transactionService;
        _masterDataService = masterDataService;
        
        // Initialize collections
        AvailableOperations = new ObservableCollection<string>();
        AvailableLocations = new ObservableCollection<string>();
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanProcessTransaction))]
    private string partId = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanProcessTransaction))]
    private string selectedOperation = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanProcessTransaction))]
    private int quantity = 1;

    [ObservableProperty]
    private ObservableCollection<string> availableOperations = new();

    [ObservableProperty]
    private ObservableCollection<string> availableLocations = new();

    public bool CanProcessTransaction => 
        !IsLoading && 
        !string.IsNullOrWhiteSpace(PartId) &&
        !string.IsNullOrWhiteSpace(SelectedOperation) &&
        Quantity > 0;

    [RelayCommand(CanExecute = nameof(CanProcessTransaction))]
    private async Task ProcessTransactionAsync()
    {
        IsLoading = true;
        ClearErrors();
        StatusMessage = "Processing transaction...";

        try
        {
            var request = new InventoryTransactionRequest
            {
                PartId = PartId,
                Operation = SelectedOperation,
                Quantity = Quantity,
                Location = SelectedLocation,
                UserIntent = DetermineUserIntent(), // Based on UI context
                UserId = Environment.UserName
            };

            var result = await _transactionService.ProcessTransactionAsync(request);

            if (result.IsSuccess)
            {
                StatusMessage = $"Transaction {result.Data.TransactionId} completed successfully";
                await ResetFormAsync();
            }
            else
            {
                await Services.ErrorHandling.HandleServiceFailureAsync(result, nameof(ProcessTransactionAsync));
            }
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, nameof(ProcessTransactionAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected override async Task OnInitializeAsync()
    {
        // Load master data
        await LoadMasterDataAsync();
    }

    private UserTransactionIntent DetermineUserIntent()
    {
        // Logic to determine user intent based on UI state/context
        // This is where business logic determines IN/OUT/TRANSFER
        return UserTransactionIntent.AddingStock; // Example
    }
}
```

---

**Pattern Library Status**: ✅ Core Patterns Documented  
**Coverage**: MVVM, Services, Database, Domain, Error Handling  
**Integration**: GitHub Copilot context enhancement  
**Maintained By**: MTM Development Team