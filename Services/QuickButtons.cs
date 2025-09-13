using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
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
    Task<bool> CreateQuickButtonAsync(string partId, string operation, string location, int quantity, string? notes = null);
    Task<bool> ExportQuickButtonsAsync(string userId, string fileName = "");
    Task<bool> ImportQuickButtonsAsync(string userId, string filePath);
    Task<List<string>> GetAvailableExportFilesAsync();
    List<QuickButtonData> GetQuickButtons();
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
    private readonly IFilePathService _filePathService;
    private readonly ILogger<QuickButtonsService> _logger;
    private const string DefaultItemType = "WIP"; // Align with stored procedure expectation

    public event EventHandler<QuickButtonsChangedEventArgs>? QuickButtonsChanged;

    public QuickButtonsService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IFilePathService filePathService,
        ILogger<QuickButtonsService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _filePathService = filePathService ?? throw new ArgumentNullException(nameof(filePathService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<QuickButtonData>> LoadUserQuickButtonsAsync(string userId)
    {
        try
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("UserId is null or empty - cannot load quick buttons");
                return new List<QuickButtonData>(); // Return empty list instead of null
            }

            _logger.LogDebug("Loading quick buttons for user: {UserId}", userId);

            // Use the correct parameter format for the updated stored procedure
            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = userId
            };

            try
            {
                // qb_quickbuttons_Get_ByUser now uses proper MTM status pattern with OUT parameters
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _databaseService.GetConnectionString(),
                    "qb_quickbuttons_Get_ByUser",
                    parameters
                );

                var quickButtons = new List<QuickButtonData>();

                // Check if the stored procedure executed successfully
                // Updated Status Pattern: 1=Success with data, 0=Success no data, -1=Error
                if (result.Status >= 0)
                {
                    // Status 1 means success with data, Status 0 means success but no data
                    if (result.Status == 1 && result.Data != null && result.Data.Rows.Count > 0)
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
                                Notes = SafeGetString(row, "Location"), // Map Location to Notes for compatibility
                                CreatedDate = SafeGetDateTime(row, "Created_Date") ?? DateTime.Now,
                                LastUsedDate = SafeGetDateTime(row, "DateModified") ?? DateTime.Now
                            });
                        }
                    }
                    // Status 0 means success but no data - this is OK, return empty list
                }
                else
                {
                    // Status -1 means error
                    _logger.LogWarning("qb_quickbuttons_Get_ByUser returned error status {Status}: {Message}", 
                        result.Status, result.Message);
                }
                
                _logger.LogInformation("Loaded {Count} quick buttons for user {UserId}", quickButtons.Count, userId);
                return quickButtons;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Database call failed for qb_quickbuttons_Get_ByUser");
                // Return empty list instead of sample data for production
                return new List<QuickButtonData>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load quick buttons for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(LoadUserQuickButtonsAsync), userId);
            
            // Return empty list instead of sample data for production
            return new List<QuickButtonData>();
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
                ["p_User"] = userId,
                ["p_Limit"] = 10
            };

            _logger.LogInformation("Calling stored procedure sys_last_10_transactions_Get_ByUser with UserId: {UserId}, Limit: 10", userId);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "sys_last_10_transactions_Get_ByUser",
                parameters
            );

            _logger.LogInformation("Stored procedure returned {RowCount} rows", result?.Data?.Rows.Count ?? 0);

            var transactions = new List<QuickButtonData>();

            if (result != null && result.Data != null && result.Data.Rows.Count > 0)
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
                    _logger.LogDebug("Row {Index}: PartID={PartID}, Operation={Operation}, Quantity={Quantity}", 
                        i, SafeGetColumnValue(row, "PartID"), SafeGetColumnValue(row, "Operation"), 
                        SafeGetColumnValue(row, "Quantity"));
                    
                    transactions.Add(new QuickButtonData
                    {
                        Id = i + 1, // Use row index + 1 as ID for transactions
                        UserId = userId,
                        Position = i + 1, // Use row index + 1 as position
                        PartId = SafeGetString(row, "PartID"),
                        Operation = SafeGetString(row, "Operation"),
                        Quantity = SafeGetInt32(row, "Quantity", 0),
                        Notes = SafeGetString(row, "Notes"),
                        CreatedDate = SafeGetDateTime(row, "ReceiveDate") ?? DateTime.Now,
                        LastUsedDate = SafeGetDateTime(row, "ReceiveDate") ?? DateTime.Now
                    });
                }
                Debug.WriteLine($"Loaded {transactions.Count} transactions for user {userId}");
                _logger.LogInformation("Successfully loaded {Count} recent transactions for user {UserId}", transactions.Count, userId);
            }
            else
            {
                _logger.LogInformation("Stored procedure returned no data for user {UserId}", userId);
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
            // Validate required parameters before building dictionary
            if (quickButton == null)
            {
                _logger.LogError("QuickButtonData is null - cannot save");
                return false;
            }

            if (string.IsNullOrWhiteSpace(quickButton.UserId))
            {
                _logger.LogError("QuickButton UserId is null or empty - cannot save. Position: {Position}, PartId: {PartId}", 
                    quickButton.Position, quickButton.PartId);
                return false;
            }

            if (string.IsNullOrWhiteSpace(quickButton.PartId))
            {
                _logger.LogError("QuickButton PartId is null or empty - cannot save. UserId: {UserId}, Position: {Position}", 
                    quickButton.UserId, quickButton.Position);
                return false;
            }

            if (quickButton.Position < 1 || quickButton.Position > 10)
            {
                _logger.LogError("QuickButton Position {Position} is out of range (1-10) - cannot save. PartId: {PartId}", 
                    quickButton.Position, quickButton.PartId);
                return false;
            }

            if (quickButton.Quantity <= 0)
            {
                _logger.LogError("QuickButton Quantity {Quantity} must be > 0 - cannot save. PartId: {PartId}, Position: {Position}", 
                    quickButton.Quantity, quickButton.PartId, quickButton.Position);
                return false;
            }

            _logger.LogDebug("Saving quick button: Position {Position}, Part {PartId}, User {UserId}", 
                quickButton.Position, quickButton.PartId, quickButton.UserId);

            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = quickButton.UserId,
                ["p_Position"] = quickButton.Position,
                ["p_PartID"] = quickButton.PartId,
                ["p_Operation"] = quickButton.Operation ?? string.Empty,
                ["p_Quantity"] = quickButton.Quantity,
                ["p_Location"] = quickButton.Notes ?? string.Empty, // Use Notes field as Location
                ["p_ItemType"] = DefaultItemType
            };

            // Use ExecuteWithStatus which handles MySQL status codes correctly
            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "qb_quickbuttons_Save",
                parameters
            );

            // Updated Status Pattern: 1=Success with data, 0=Success no data, -1=Error
            // For Save operations, we expect Status 1 (success with row affected)
            if (result.Status >= 0)
            {
                _logger.LogInformation("Quick button saved successfully: {PartId} at position {Position}", 
                    quickButton.PartId, quickButton.Position);

                return true;
            }
            else
            {
                _logger.LogError("Failed to save quick button: {PartId}, Status: {Status}, Error: {Error}", 
                    quickButton.PartId, result.Status, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save quick button: {PartId}", quickButton?.PartId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(SaveQuickButtonAsync), quickButton?.UserId ?? "unknown", 
                new Dictionary<string, object> { ["QuickButton"] = quickButton ?? new object() });
            return false;
        }
    }

    public async Task<bool> RemoveQuickButtonAsync(int buttonId, string userId)
    {
        try
        {
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("UserId is null or empty - cannot remove quick button. ButtonId: {ButtonId}", buttonId);
                return false;
            }

            _logger.LogDebug("Removing quick button: Position {ButtonId} for user {UserId}", buttonId, userId);

            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = userId,
                ["p_Position"] = buttonId // buttonId is actually the position in this context
            };

            // Use the proper stored procedure execution method that handles OUT parameters
            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "qb_quickbuttons_Remove",
                parameters
            );

            // Updated Status Pattern: 1=Success with data, 0=Success no data, -1=Error
            // Status 1 = row removed, Status 0 = nothing to remove (still success)
            if (result.Status >= 0)
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
            // Validate required parameters
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("UserId is null or empty - cannot clear quick buttons");
                return false;
            }

            _logger.LogDebug("Clearing all quick buttons for user: {UserId}", userId);

            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = userId
            };

            // Use the proper stored procedure execution method that handles OUT parameters
            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "qb_quickbuttons_Clear_ByUser",
                parameters
            );

            // Updated Status Pattern: 1=Success with data, 0=Success no data, -1=Error
            // Status 1 = rows deleted, Status 0 = no rows to delete (still success)
            if (result.Status >= 0)
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
                // Enforce bounds locally before hitting database
                if (button.Position < 1 || button.Position > 10)
                {
                    _logger.LogWarning("Skipping reorder save for out-of-range position {Position} (Part {PartId})", button.Position, button.PartId);
                    allSuccessful = false;
                    continue;
                }
                if (button.Quantity <= 0)
                {
                    _logger.LogWarning("Skipping reorder save for invalid quantity {Quantity} at position {Position} (Part {PartId})", button.Quantity, button.Position, button.PartId);
                    allSuccessful = false;
                    continue;
                }
                var parameters = new Dictionary<string, object>
                {
                    ["p_User"] = userId,
                    ["p_Position"] = button.Position,
                    ["p_PartID"] = button.PartId,
                    ["p_Location"] = button.Notes ?? string.Empty, // Use Notes field as Location for now
                    ["p_Operation"] = button.Operation ?? string.Empty,
                    ["p_Quantity"] = button.Quantity,
                    ["p_ItemType"] = DefaultItemType
                };

                // Use the proper stored procedure execution method that handles OUT parameters
                var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                    _databaseService.GetConnectionString(),
                    "qb_quickbuttons_Save",
                    parameters
                );

                // Updated Status Pattern: 1=Success with data, 0=Success no data, -1=Error
                // For Save operations, we expect Status 1 (success with row affected)
                if (result.Status < 0)
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
                ["p_TransactionType"] = "IN", // Default transaction type based on user intent
                ["p_BatchNumber"] = DateTime.Now.ToString("yyyyMMddHHmmss"), // Generate batch number
                ["p_PartID"] = partId,
                ["p_FromLocation"] = string.Empty, // Use empty string instead of DBNull
                ["p_ToLocation"] = string.Empty, // Use empty string instead of DBNull
                ["p_Operation"] = operation,
                ["p_Quantity"] = quantity,
                ["p_Notes"] = string.Empty, // Use empty string instead of DBNull
                ["p_User"] = userId,
                ["p_ItemType"] = "Standard", // Default item type
                ["p_ReceiveDate"] = DateTime.Now
            };

            var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                _databaseService.GetConnectionString(),
                "sys_last_10_transactions_Add_Transaction",
                parameters
            );

            // For MySQL stored procedures: Status >= 0 means SUCCESS, Status < 0 means ERROR  
            // MTM Status Pattern: -1=Error, 0=Success (no data), 1=Success (with data)
            if (result.Status >= 0)
            {
                _logger.LogInformation("Successfully added transaction to last 10 for user {UserId}: {PartId}/{Operation}/{Quantity}", 
                    userId, partId, operation, quantity);
                
                // Now add this as a quick button at position 1, shifting others down
                await AddQuickButtonFromOperationAsync(userId, partId, operation, quantity);
                
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

            // Load current quick buttons for the user
            var existingButtons = await LoadUserQuickButtonsAsync(userId);
            
            // Check if this exact part+operation combo already exists
            var duplicateButton = existingButtons.FirstOrDefault(b => 
                string.Equals(b.PartId, partId, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(b.Operation, operation, StringComparison.OrdinalIgnoreCase));
            
            if (duplicateButton != null)
            {
                // Remove the duplicate first
                _logger.LogDebug("Removing duplicate button for {PartId}/{Operation} at position {Position}", 
                    partId, operation, duplicateButton.Position);
                await RemoveQuickButtonAsync(duplicateButton.Position, userId);
                
                // Reload buttons after removal
                existingButtons = await LoadUserQuickButtonsAsync(userId);
            }
            
            // Clear all existing buttons for this user to rebuild properly
            await ClearAllQuickButtonsAsync(userId);
            
            // Create new button for position 1
            var newButton = new QuickButtonData
            {
                UserId = userId,
                Position = 1,
                PartId = partId,
                Operation = operation,
                Quantity = quantity,
                Notes = notes ?? string.Empty,
                CreatedDate = DateTime.Now,
                LastUsedDate = DateTime.Now
            };
            
            // Save the new button at position 1
            var saveResult = await SaveQuickButtonAsync(newButton);
            if (!saveResult)
            {
                _logger.LogError("Failed to save new quick button at position 1: {PartId}/{Operation}", partId, operation);
                return false;
            }
            
            // Add existing buttons shifted down, keeping only the most recent 9
            var recentButtons = existingButtons
                .OrderBy(b => b.Position)
                .Take(9)
                .ToList();
                
            for (int i = 0; i < recentButtons.Count; i++)
            {
                var button = recentButtons[i];
                button.Position = i + 2; // Start from position 2
                
                var shiftResult = await SaveQuickButtonAsync(button);
                if (!shiftResult)
                {
                    _logger.LogError("Failed to save shifted button at position {Position}: {PartId}", 
                        button.Position, button.PartId);
                }
            }
            
            _logger.LogInformation("Successfully added quick button at position 1: {PartId}/{Operation}/{Quantity}", 
                partId, operation, quantity);
                
            // Raise event to notify UI (this will be called again but it's important for immediate UI update)
            QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs
            {
                UserId = userId,
                ChangeType = QuickButtonChangeType.Added,
                AffectedButton = newButton
            });
            
            return true;
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

    /// <summary>
    /// Exports quick buttons configuration to a JSON file in MyDocuments
    /// </summary>
    public async Task<bool> ExportQuickButtonsAsync(string userId, string fileName = "")
    {
        try
        {
            _logger.LogDebug("Starting quick buttons export for user: {UserId}", userId);

            // Load current quick buttons
            var quickButtons = await LoadUserQuickButtonsAsync(userId);
            if (!quickButtons.Any())
            {
                _logger.LogWarning("No quick buttons found to export for user: {UserId}", userId);
                return false;
            }

            // Generate filename if not provided
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = _filePathService.GetTimestampedFileName($"QuickButtons_{userId}", "json");
            }
            else if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            // Get export path
            var exportPath = _filePathService.GetQuickButtonsExportPath();
            var fullPath = Path.Combine(exportPath, fileName);

            // Create export data structure
            var exportData = new
            {
                ExportedAt = DateTime.Now,
                ExportedBy = userId,
                Version = "1.0",
                QuickButtons = quickButtons.Select(qb => new
                {
                    qb.Position,
                    qb.PartId,
                    qb.Operation,
                    qb.Quantity,
                    qb.Notes,
                    qb.CreatedDate,
                    qb.LastUsedDate
                }).ToList()
            };

            // Serialize to JSON with pretty formatting
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonContent = JsonSerializer.Serialize(exportData, options);
            await File.WriteAllTextAsync(fullPath, jsonContent);

            _logger.LogInformation("Successfully exported {Count} quick buttons to: {FilePath}", 
                quickButtons.Count, fullPath);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export quick buttons for user: {UserId}", userId);
            await ErrorHandling.HandleErrorAsync(ex, nameof(ExportQuickButtonsAsync), userId);
            return false;
        }
    }

    /// <summary>
    /// Imports quick buttons configuration from a JSON file
    /// </summary>
    public async Task<bool> ImportQuickButtonsAsync(string userId, string filePath)
    {
        try
        {
            _logger.LogDebug("Starting quick buttons import for user: {UserId} from file: {FilePath}", userId, filePath);

            if (!File.Exists(filePath))
            {
                _logger.LogError("Import file does not exist: {FilePath}", filePath);
                return false;
            }

            // Read and parse JSON file
            var jsonContent = await File.ReadAllTextAsync(filePath);
            var importData = JsonSerializer.Deserialize<JsonElement>(jsonContent);

            if (!importData.TryGetProperty("quickButtons", out var quickButtonsJson))
            {
                _logger.LogError("Invalid import file format - missing quickButtons property");
                return false;
            }

            // Clear existing quick buttons for the user
            var clearResult = await ClearAllQuickButtonsAsync(userId);
            if (!clearResult)
            {
                _logger.LogWarning("Failed to clear existing quick buttons before import");
            }

            // Import each quick button
            int importedCount = 0;
            int position = 1;

            foreach (var buttonJson in quickButtonsJson.EnumerateArray())
            {
                try
                {
                    var quickButton = new QuickButtonData
                    {
                        UserId = userId,
                        Position = position++,
                        PartId = buttonJson.TryGetProperty("partId", out var partId) ? partId.GetString() ?? "" : "",
                        Operation = buttonJson.TryGetProperty("operation", out var operation) ? operation.GetString() ?? "" : "",
                        Quantity = buttonJson.TryGetProperty("quantity", out var quantity) ? quantity.GetInt32() : 1,
                        Notes = buttonJson.TryGetProperty("notes", out var notes) ? notes.GetString() ?? "" : "",
                        CreatedDate = DateTime.Now,
                        LastUsedDate = buttonJson.TryGetProperty("lastUsedDate", out var lastUsed) && lastUsed.TryGetDateTime(out var lastUsedDate) ? lastUsedDate : DateTime.Now
                    };

                    if (string.IsNullOrWhiteSpace(quickButton.PartId) || string.IsNullOrWhiteSpace(quickButton.Operation))
                    {
                        _logger.LogWarning("Skipping invalid quick button with empty PartId or Operation");
                        continue;
                    }

                    var saveResult = await SaveQuickButtonAsync(quickButton);
                    if (saveResult)
                    {
                        importedCount++;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to import quick button: {PartId}/{Operation}", quickButton.PartId, quickButton.Operation);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse quick button from import data");
                }
            }

            _logger.LogInformation("Successfully imported {ImportedCount} quick buttons for user: {UserId}", importedCount, userId);

            // Notify UI of the import
            QuickButtonsChanged?.Invoke(this, new QuickButtonsChangedEventArgs
            {
                UserId = userId,
                ChangeType = QuickButtonChangeType.Added
            });

            return importedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import quick buttons for user: {UserId} from file: {FilePath}", userId, filePath);
            await ErrorHandling.HandleErrorAsync(ex, nameof(ImportQuickButtonsAsync), userId);
            return false;
        }
    }

    /// <summary>
    /// Gets list of available export files in the export directory
    /// </summary>
    public async Task<List<string>> GetAvailableExportFilesAsync()
    {
        try
        {
            var exportPath = _filePathService.GetQuickButtonsExportPath();
            
            if (!Directory.Exists(exportPath))
            {
                return new List<string>();
            }

            var files = Directory.GetFiles(exportPath, "*.json")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .Select(f => f.FullName)
                .ToList();

            _logger.LogDebug("Found {Count} export files in {Path}", files.Count, exportPath);
            
            await Task.CompletedTask; // Make this properly async
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available export files");
            await ErrorHandling.HandleErrorAsync(ex, nameof(GetAvailableExportFilesAsync), "system");
            return new List<string>();
        }
    }

    /// <summary>
    /// Creates a new quick button.
    /// This method delegates to AddQuickButtonFromOperationAsync to maintain consistency.
    /// Note: userId is retrieved from application variables.
    /// </summary>
    public async Task<bool> CreateQuickButtonAsync(string partId, string operation, string location, int quantity, string? notes = null)
    {
        var userId = Models.Model_AppVariables.CurrentUser;
        return await AddQuickButtonFromOperationAsync(userId, partId, operation, quantity, notes);
    }

    /// <summary>
    /// Gets all quick buttons for the current user.
    /// This method delegates to LoadUserQuickButtonsAsync to maintain consistency.
    /// Note: userId is retrieved from application variables.
    /// </summary>
    public List<QuickButtonData> GetQuickButtons()
    {
        var userId = Models.Model_AppVariables.CurrentUser;
        // Since this is a synchronous interface method, we need to use GetAwaiter().GetResult()
        return LoadUserQuickButtonsAsync(userId).GetAwaiter().GetResult();
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
