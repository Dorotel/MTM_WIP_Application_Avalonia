using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Quick buttons management service interface.
/// Provides functionality for managing user's quick action buttons including 
/// loading, saving, reordering, and synchronizing with transaction history.
/// </summary>
public interface IQuickButtonsService
{
    Task<List<QuickButtonData>> LoadUserQuickButtonsAsync(string userId);
    Task<List<QuickButtonData>> LoadLast10TransactionsAsync(string userId);
    Task<bool> SaveQuickButtonAsync(QuickButtonData quickButton);
    Task<bool> RemoveQuickButtonAsync(int buttonId, string userId);
    Task<bool> ClearAllQuickButtonsAsync(string userId);
    Task<bool> ReorderQuickButtonsAsync(string userId, List<QuickButtonData> reorderedButtons);
    Task<bool> AddQuickButtonFromOperationAsync(string userId, string partId, string operation, int quantity, string? notes = null);
    Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity);
    event EventHandler<QuickButtonsChangedEventArgs>? QuickButtonsChanged;
}

/// <summary>
/// Progress reporting service interface.
/// Provides centralized progress reporting for long-running operations
/// with support for detailed status messages and cancellation.
/// </summary>
public interface IProgressService : INotifyPropertyChanged
{
    bool IsOperationInProgress { get; }
    string CurrentOperationDescription { get; }
    int ProgressPercentage { get; }
    string StatusMessage { get; }
    bool CanCancel { get; }
    
    void StartOperation(string description, bool canCancel = false);
    void UpdateProgress(int percentage, string? statusMessage = null);
    void CompleteOperation(string? finalMessage = null);
    void CancelOperation();
    void ReportError(string errorMessage);
    
    event EventHandler<ProgressChangedEventArgs>? ProgressChanged;
    event EventHandler<OperationCompletedEventArgs>? OperationCompleted;
    event EventHandler? OperationCancelled;
}

/// <summary>
/// Quick buttons service implementation.
/// Manages user's quick action buttons with database persistence.
/// </summary>
public class QuickButtonsService : IQuickButtonsService
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<QuickButtonsService> _logger;

    public event EventHandler<QuickButtonsChangedEventArgs>? QuickButtonsChanged;

    public QuickButtonsService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<QuickButtonsService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<QuickButtonData>> LoadUserQuickButtonsAsync(string userId)
    {
        try
        {
            _logger.LogDebug("Loading quick buttons for user: {UserId}", userId);

            var parameters = new Dictionary<string, object>
            {
                ["p_UserID"] = userId
            };

            try
            {
                // Use proper stored procedure execution with status handling
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _databaseService.GetConnectionString(),
                    "qb_quickbuttons_Get_ByUser",
                    parameters
                );

                var quickButtons = new List<QuickButtonData>();

                if (result.IsSuccess && result.Data != null && result.Data.Rows.Count > 0)
                {
                    // Log available columns for debugging
                    if (result.Data.Columns.Count > 0)
                    {
                        var columnNames = string.Join(", ", result.Data.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                        _logger.LogDebug("Available columns in qb_quickbuttons_Get_ByUser result: {Columns}", columnNames);
                    }

                    // Convert DataTable rows to QuickButtonData objects
                    foreach (DataRow row in result.Data.Rows)
                    {
                        quickButtons.Add(new QuickButtonData
                        {
                            Id = SafeGetInt32(row, "ID", 0),
                            UserId = userId,
                            Position = SafeGetInt32(row, "Position", 0),
                            PartId = SafeGetString(row, "PartID"),
                            Operation = SafeGetString(row, "Operation"),
                            Quantity = SafeGetInt32(row, "Quantity", 0),
                            Notes = SafeGetString(row, "Notes"),
                            CreatedDate = SafeGetDateTime(row, "CreatedDate") ?? DateTime.Now,
                            LastUsedDate = SafeGetDateTime(row, "LastUsedDate") ?? DateTime.Now
                        });
                    }
                }
                else if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to load quick buttons: {Message}", result.Message);
                }
                
                _logger.LogInformation("Loaded {Count} quick buttons for user {UserId}", quickButtons.Count, userId);
                return quickButtons;
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database call failed, using sample data for development");
                // Return sample data as fallback
                return GenerateSampleQuickButtons();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load quick buttons for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(LoadUserQuickButtonsAsync), userId);
            
            // Return sample data as fallback
            return GenerateSampleQuickButtons();
        }
    }

    public async Task<List<QuickButtonData>> LoadLast10TransactionsAsync(string userId)
    {
        try
        {
            _logger.LogDebug("Loading last 10 transactions for user: {UserId}", userId);

            // Validate user ID is not empty
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Cannot load transactions: UserId is null or empty");
                return new List<QuickButtonData>();
            }

            // Use the stored procedure that reads from sys_last_10_transactions table
            var parameters = new Dictionary<string, object>
            {
                ["p_UserID"] = userId,
                ["p_Limit"] = 10
            };

            _logger.LogInformation("Calling stored procedure sys_last_10_transactions_Get_ByUser with UserId: {UserId}, Limit: 10", userId);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "sys_last_10_transactions_Get_ByUser",
                parameters
            );

            _logger.LogInformation("Stored procedure returned - Status: {Status}, Message: {Message}, RowCount: {RowCount}", 
                result.Status, result.Message, result.Data?.Rows.Count ?? 0);

            var transactions = new List<QuickButtonData>();

            if (result.IsSuccess && result.Data != null && result.Data.Rows.Count > 0)
            {
                _logger.LogInformation("Processing {RowCount} rows from stored procedure result", result.Data.Rows.Count);

                // Log available columns for debugging
                if (result.Data.Columns.Count > 0)
                {
                    var columnNames = string.Join(", ", result.Data.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    _logger.LogDebug("Available columns: {Columns}", columnNames);
                }

                // Convert DataTable rows to QuickButtonData objects
                for (int i = 0; i < result.Data.Rows.Count; i++)
                {
                    var row = result.Data.Rows[i];
                    
                    // Log the row data for debugging
                    _logger.LogDebug("Row {Index}: ID={ID}, PartID={PartID}, Operation={Operation}, Quantity={Quantity}", 
                        i, SafeGetColumnValue(row, "ID"), SafeGetColumnValue(row, "PartID"), 
                        SafeGetColumnValue(row, "Operation"), SafeGetColumnValue(row, "Quantity"));
                    
                    transactions.Add(new QuickButtonData
                    {
                        Id = SafeGetInt32(row, "ID", i + 1),
                        UserId = userId,
                        Position = SafeGetInt32(row, "Position", i + 1), // Use row index + 1 as fallback position
                        PartId = SafeGetString(row, "PartID"),
                        Operation = SafeGetString(row, "Operation"),
                        Quantity = SafeGetInt32(row, "Quantity", 0),
                        Notes = SafeGetString(row, "Notes"),
                        CreatedDate = SafeGetDateTime(row, "CreatedDate") ?? SafeGetDateTime(row, "ReceiveDate") ?? DateTime.Now,
                        LastUsedDate = SafeGetDateTime(row, "LastUsedDate") ?? SafeGetDateTime(row, "ReceiveDate") ?? DateTime.Now
                    });
                }
                Debug.WriteLine($"Loaded {transactions.Count} transactions for user {userId}");
                _logger.LogInformation("Successfully loaded {Count} recent transactions for user {UserId}", transactions.Count, userId);
            }
            else if (!result.IsSuccess)
            {
                _logger.LogWarning("Stored procedure failed for user {UserId}: Status={Status}, Message={Message}", 
                    userId, result.Status, result.Message);
                Debug.WriteLine($"Stored procedure failed: {result.Message}");
            }
            else
            {
                _logger.LogInformation("Stored procedure succeeded but returned no data for user {UserId}. Status={Status}, Message={Message}", 
                    userId, result.Status, result.Message);
                Debug.WriteLine("No transactions found.");
            }

            return transactions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recent transactions for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(LoadLast10TransactionsAsync), userId);
            
            // Return empty list instead of sample data for production
            return new List<QuickButtonData>();
        }
    }

    /// <summary>
    /// Safely get a column value from a DataRow, returning null if column doesn't exist
    /// </summary>
    private static object? SafeGetColumnValue(DataRow row, string columnName)
    {
        try
        {
            return row.Table.Columns.Contains(columnName) ? row[columnName] : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Safely get a string value from a DataRow column
    /// </summary>
    private static string SafeGetString(DataRow row, string columnName, string defaultValue = "")
    {
        try
        {
            if (!row.Table.Columns.Contains(columnName))
                return defaultValue;
                
            var value = row[columnName];
            return value == DBNull.Value ? defaultValue : value?.ToString() ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Safely get an integer value from a DataRow column
    /// </summary>
    private static int SafeGetInt32(DataRow row, string columnName, int defaultValue = 0)
    {
        try
        {
            if (!row.Table.Columns.Contains(columnName))
                return defaultValue;
                
            var value = row[columnName];
            if (value == DBNull.Value || value == null)
                return defaultValue;
                
            return Convert.ToInt32(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Safely get a DateTime value from a DataRow column
    /// </summary>
    private static DateTime? SafeGetDateTime(DataRow row, string columnName)
    {
        try
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;
                
            var value = row[columnName];
            if (value == DBNull.Value || value == null)
                return null;
                
            return Convert.ToDateTime(value);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> SaveQuickButtonAsync(QuickButtonData quickButton)
    {
        try
        {
            _logger.LogDebug("Saving quick button: Position {Position}, Part {PartId}", 
                quickButton.Position, quickButton.PartId);

            var parameters = new Dictionary<string, object>
            {
                ["p_UserID"] = quickButton.UserId,
                ["p_Position"] = quickButton.Position,
                ["p_PartID"] = quickButton.PartId,
                ["p_Operation"] = quickButton.Operation,
                ["p_Quantity"] = quickButton.Quantity,
                ["p_Notes"] = quickButton.Notes ?? string.Empty
            };

            // Use the proper stored procedure execution method that handles OUT parameters
            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "qb_quickbuttons_Save",
                parameters
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("Quick button saved successfully: {PartId} at position {Position}", 
                    quickButton.PartId, quickButton.Position);

                // Notify subscribers of changes
                QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs 
                { 
                    UserId = quickButton.UserId, 
                    ChangeType = QuickButtonChangeType.Updated,
                    AffectedButton = quickButton
                });

                return true;
            }
            else
            {
                _logger.LogError("Failed to save quick button: {PartId}, Error: {Error}", quickButton.PartId, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save quick button: {PartId}", quickButton.PartId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(SaveQuickButtonAsync), quickButton.UserId, 
                new Dictionary<string, object> { ["QuickButton"] = quickButton });
            return false;
        }
    }

    public async Task<bool> RemoveQuickButtonAsync(int buttonId, string userId)
    {
        try
        {
            _logger.LogDebug("Removing quick button: ID {ButtonId} for user {UserId}", buttonId, userId);

            var parameters = new Dictionary<string, object>
            {
                ["p_ButtonID"] = buttonId,
                ["p_UserID"] = userId
            };

            // Use the proper stored procedure execution method that handles OUT parameters
            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "qb_quickbuttons_Remove",
                parameters
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("Quick button removed successfully: ID {ButtonId}", buttonId);

                // Notify subscribers of changes
                QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs 
                { 
                    UserId = userId, 
                    ChangeType = QuickButtonChangeType.Removed
                });

                return true;
            }
            else
            {
                _logger.LogError("Failed to remove quick button: ID {ButtonId}, Error: {Error}", buttonId, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove quick button: ID {ButtonId}", buttonId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(RemoveQuickButtonAsync), userId);
            return false;
        }
    }

    public async Task<bool> ClearAllQuickButtonsAsync(string userId)
    {
        try
        {
            _logger.LogDebug("Clearing all quick buttons for user: {UserId}", userId);

            var parameters = new Dictionary<string, object>
            {
                ["p_UserID"] = userId
            };

            // Use the proper stored procedure execution method that handles OUT parameters
            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "qb_quickbuttons_Clear_ByUser",
                parameters
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("All quick buttons cleared for user: {UserId}", userId);

                // Notify subscribers of changes
                QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs 
                { 
                    UserId = userId, 
                    ChangeType = QuickButtonChangeType.Cleared
                });

                return true;
            }
            else
            {
                _logger.LogError("Failed to clear quick buttons for user: {UserId}, Error: {Error}", userId, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear quick buttons for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(ClearAllQuickButtonsAsync), userId);
            return false;
        }
    }

    public async Task<bool> ReorderQuickButtonsAsync(string userId, List<QuickButtonData> reorderedButtons)
    {
        try
        {
            _logger.LogDebug("Reordering {Count} quick buttons for user: {UserId}", 
                reorderedButtons.Count, userId);

            // For reordering, we'll call the save procedure for each button with its new position
            bool allSuccessful = true;
            
            foreach (var button in reorderedButtons)
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_UserID"] = userId,
                    ["p_Position"] = button.Position,
                    ["p_PartID"] = button.PartId,
                    ["p_Operation"] = button.Operation,
                    ["p_Quantity"] = button.Quantity,
                    ["p_Notes"] = button.Notes ?? string.Empty
                };

                // Use the proper stored procedure execution method that handles OUT parameters
                var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                    _databaseService.GetConnectionString(),
                    "qb_quickbuttons_Save",
                    parameters
                );

                if (!result.IsSuccess)
                {
                    allSuccessful = false;
                    _logger.LogError("Failed to reorder button at position {Position}: {Error}", button.Position, result.Message);
                }
            }
            
            if (allSuccessful)
            {
                _logger.LogInformation("Quick buttons reordered successfully for user: {UserId}", userId);

                // Notify subscribers of changes
                QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs 
                { 
                    UserId = userId, 
                    ChangeType = QuickButtonChangeType.Reordered
                });
            }

            return allSuccessful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reorder quick buttons for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(ReorderQuickButtonsAsync), userId);
            return false;
        }
    }

    /// <summary>
    /// Adds a transaction to the user's last 10 transactions for quick button creation.
    /// Called automatically when inventory operations are completed.
    /// </summary>
    public async Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity)
    {
        try
        {
            _logger.LogDebug("Adding transaction to last 10 for user: {UserId}, Part: {PartId}, Operation: {Operation}, Quantity: {Quantity}", 
                userId, partId, operation, quantity);

            var parameters = new Dictionary<string, object>
            {
                ["p_UserID"] = userId,
                ["p_PartID"] = partId,
                ["p_Operation"] = operation,
                ["p_Quantity"] = quantity
            };

            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "sys_last_10_transactions_Add_Transaction",
                parameters
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully added transaction to last 10 for user {UserId}: {PartId}/{Operation}/{Quantity}", 
                    userId, partId, operation, quantity);
                
                // Raise event to notify UI that quick buttons need to be refreshed
                QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs
                {
                    UserId = userId,
                    ChangeType = QuickButtonChangeType.Added,
                    AffectedButton = new QuickButtonData
                    {
                        UserId = userId,
                        PartId = partId,
                        Operation = operation,
                        Quantity = quantity
                    }
                });

                return true;
            }
            else
            {
                _logger.LogWarning("Failed to add transaction to last 10 for user {UserId}: {Message}", userId, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding transaction to last 10 for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(AddTransactionToLast10Async), userId);
            return false;
        }
    }

    public async Task<bool> AddQuickButtonFromOperationAsync(string userId, string partId, string operation, int quantity, string? notes = null)
    {
        try
        {
            _logger.LogDebug("Adding quick button from operation: {PartId}, {Operation}, {Quantity}", 
                partId, operation, quantity);

            // Find next available position or shift existing buttons
            var existingButtons = await LoadUserQuickButtonsAsync(userId);
            var nextPosition = existingButtons.Count + 1;

            var newButton = new QuickButtonData
            {
                UserId = userId,
                Position = nextPosition,
                PartId = partId,
                Operation = operation,
                Quantity = quantity,
                Notes = notes,
                CreatedDate = DateTime.Now,
                LastUsedDate = DateTime.Now
            };

            return await SaveQuickButtonAsync(newButton);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add quick button from operation");
            await ErrorHandling.HandleErrorAsync(ex, nameof(AddQuickButtonFromOperationAsync), userId);
            return false;
        }
    }

    private List<QuickButtonData> GenerateSampleQuickButtons()
    {
        var operations = new[] { "90", "100", "110", "120", "130" };
        var parts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005" };
        
        return Enumerable.Range(1, 7).Select(i => new QuickButtonData
        {
            Id = i,
            UserId = "current_user",
            Position = i,
            PartId = parts[(i - 1) % parts.Length],
            Operation = operations[(i - 1) % operations.Length],
            Quantity = i * 5,
            Notes = $"Sample button {i}",
            CreatedDate = DateTime.Now.AddDays(-i),
            LastUsedDate = DateTime.Now.AddHours(-i)
        }).ToList();
    }

    private List<QuickButtonData> GenerateSampleTransactionButtons()
    {
        var operations = new[] { "90", "100", "110", "120", "130" };
        var parts = new[] { "24733444-PKG", "24677611", "24733405-PKG", "24733403-PKG", "24733491-PKG" };
        
        return Enumerable.Range(1, 10).Select(i => new QuickButtonData
        {
            Id = i + 100,
            UserId = "current_user",
            Position = i,
            PartId = parts[(i - 1) % parts.Length],
            Operation = operations[(i - 1) % operations.Length],
            Quantity = (i * 3) + 2,
            Notes = $"Recent transaction {i}",
            CreatedDate = DateTime.Now.AddHours(-i),
            LastUsedDate = DateTime.Now.AddMinutes(-i * 10)
        }).ToList();
    }
}

/// <summary>
/// Progress service implementation.
/// Provides centralized progress reporting with thread-safe operations.
/// </summary>
public class ProgressService : IProgressService
{
    private readonly ILogger<ProgressService> _logger;
    private readonly object _lockObject = new object();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;
    public event EventHandler<OperationCompletedEventArgs>? OperationCompleted;
    public event EventHandler? OperationCancelled;

    private bool _isOperationInProgress;
    private string _currentOperationDescription = string.Empty;
    private int _progressPercentage;
    private string _statusMessage = string.Empty;
    private bool _canCancel;

    public ProgressService(ILogger<ProgressService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsOperationInProgress
    {
        get
        {
            lock (_lockObject) { return _isOperationInProgress; }
        }
        private set
        {
            lock (_lockObject)
            {
                if (_isOperationInProgress != value)
                {
                    _isOperationInProgress = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public string CurrentOperationDescription
    {
        get
        {
            lock (_lockObject) { return _currentOperationDescription; }
        }
        private set
        {
            lock (_lockObject)
            {
                if (_currentOperationDescription != value)
                {
                    _currentOperationDescription = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public int ProgressPercentage
    {
        get
        {
            lock (_lockObject) { return _progressPercentage; }
        }
        private set
        {
            lock (_lockObject)
            {
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public string StatusMessage
    {
        get
        {
            lock (_lockObject) { return _statusMessage; }
        }
        private set
        {
            lock (_lockObject)
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public bool CanCancel
    {
        get
        {
            lock (_lockObject) { return _canCancel; }
        }
        private set
        {
            lock (_lockObject)
            {
                if (_canCancel != value)
                {
                    _canCancel = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public void StartOperation(string description, bool canCancel = false)
    {
        try
        {
            lock (_lockObject)
            {
                CurrentOperationDescription = description;
                ProgressPercentage = 0;
                StatusMessage = "Starting operation...";
                CanCancel = canCancel;
                IsOperationInProgress = true;
            }

            _logger.LogInformation("Operation started: {Description} (Cancellable: {CanCancel})", 
                description, canCancel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting operation: {Description}", description);
        }
    }

    public void UpdateProgress(int percentage, string? statusMessage = null)
    {
        try
        {
            lock (_lockObject)
            {
                ProgressPercentage = Math.Clamp(percentage, 0, 100);
                if (statusMessage != null)
                {
                    StatusMessage = statusMessage;
                }
            }

            var args = new ProgressChangedEventArgs
            {
                ProgressPercentage = percentage,
                StatusMessage = statusMessage ?? StatusMessage,
                OperationDescription = CurrentOperationDescription
            };

            ProgressChanged?.Invoke(this, args);

            _logger.LogDebug("Progress updated: {Percentage}% - {Status}", percentage, statusMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating progress");
        }
    }

    public void CompleteOperation(string? finalMessage = null)
    {
        try
        {
            lock (_lockObject)
            {
                ProgressPercentage = 100;
                StatusMessage = finalMessage ?? "Operation completed successfully";
                IsOperationInProgress = false;
                CanCancel = false;
            }

            var args = new OperationCompletedEventArgs
            {
                Success = true,
                FinalMessage = finalMessage ?? "Operation completed successfully",
                OperationDescription = CurrentOperationDescription
            };

            OperationCompleted?.Invoke(this, args);

            _logger.LogInformation("Operation completed: {Description}", CurrentOperationDescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing operation");
        }
    }

    public void CancelOperation()
    {
        try
        {
            lock (_lockObject)
            {
                if (!CanCancel || !IsOperationInProgress)
                {
                    _logger.LogWarning("Cannot cancel operation: CanCancel={CanCancel}, InProgress={InProgress}", 
                        CanCancel, IsOperationInProgress);
                    return;
                }

                StatusMessage = "Operation cancelled";
                IsOperationInProgress = false;
                CanCancel = false;
            }

            OperationCancelled?.Invoke(this, EventArgs.Empty);

            _logger.LogInformation("Operation cancelled: {Description}", CurrentOperationDescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling operation");
        }
    }

    public void ReportError(string errorMessage)
    {
        try
        {
            lock (_lockObject)
            {
                StatusMessage = $"Error: {errorMessage}";
                IsOperationInProgress = false;
                CanCancel = false;
            }

            var args = new OperationCompletedEventArgs
            {
                Success = false,
                FinalMessage = errorMessage,
                OperationDescription = CurrentOperationDescription
            };

            OperationCompleted?.Invoke(this, args);

            _logger.LogError("Operation failed: {Description} - {Error}", CurrentOperationDescription, errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting operation error");
        }
    }

    private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Quick button data model.
/// </summary>
public class QuickButtonData
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Position { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUsedDate { get; set; }
}

/// <summary>
/// Event arguments for quick buttons changes.
/// </summary>
public class QuickButtonsChangedEventArgs : EventArgs
{
    public string UserId { get; set; } = string.Empty;
    public QuickButtonChangeType ChangeType { get; set; }
    public QuickButtonData? AffectedButton { get; set; }
}

/// <summary>
/// Progress changed event arguments.
/// </summary>
public class ProgressChangedEventArgs : EventArgs
{
    public int ProgressPercentage { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
    public string OperationDescription { get; set; } = string.Empty;
}

/// <summary>
/// Operation completed event arguments.
/// </summary>
public class OperationCompletedEventArgs : EventArgs
{
    public bool Success { get; set; }
    public string FinalMessage { get; set; } = string.Empty;
    public string OperationDescription { get; set; } = string.Empty;
}

/// <summary>
/// Types of quick button changes.
/// </summary>
public enum QuickButtonChangeType
{
    Added,
    Updated,
    Removed,
    Reordered,
    Cleared
}
