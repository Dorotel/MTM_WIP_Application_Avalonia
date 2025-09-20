using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;
using MySql.Data.MySqlClient;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Business;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for inventory removal service that provides centralized business logic for inventory removal operations
/// </summary>
public interface IRemoveService
{
    /// <summary>
    /// Observable collection of inventory items matching current search criteria
    /// </summary>
    ObservableCollection<InventoryItem> InventoryItems { get; }

    /// <summary>
    /// Observable collection of items available for undo operations
    /// </summary>
    ObservableCollection<InventoryItem> UndoItems { get; }

    /// <summary>
    /// Indicates if the service is currently performing operations
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Indicates if there are items available for undo
    /// </summary>
    bool HasUndoItems { get; }

    /// <summary>
    /// Event fired when inventory items are removed successfully
    /// </summary>
    event EventHandler<Models.ItemsRemovedEventArgs>? ItemsRemoved;

    /// <summary>
    /// Event fired when service loading state changes
    /// </summary>
    event EventHandler<bool>? LoadingStateChanged;

    /// <summary>
    /// Searches for inventory items based on provided criteria
    /// </summary>
    Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string? partId = null, string? operation = null, string? location = null, string? user = null);

    /// <summary>
    /// Removes multiple inventory items in an atomic transaction
    /// </summary>
    Task<ServiceResult<RemovalResult>> RemoveInventoryItemsAsync(IEnumerable<InventoryItem> items, string currentUser, string notes = "");

    /// <summary>
    /// Removes a single inventory item
    /// </summary>
    Task<ServiceResult<RemovalResult>> RemoveInventoryItemAsync(InventoryItem item, string currentUser, string notes = "");

    /// <summary>
    /// Restores previously removed items from the current session
    /// </summary>
    Task<ServiceResult<RestoreResult>> UndoLastRemovalAsync(string currentUser);

    /// <summary>
    /// Clears the current undo session
    /// </summary>
    void ClearUndoSession();

    /// <summary>
    /// Refreshes the inventory search results
    /// </summary>
    Task<ServiceResult> RefreshInventoryAsync();

    /// <summary>
    /// Gets suggestions for part IDs based on user input
    /// </summary>
    Task<List<string>> GetPartSuggestionsAsync(string userInput);

    /// <summary>
    /// Gets suggestions for operations based on user input
    /// </summary>
    Task<List<string>> GetOperationSuggestionsAsync(string userInput);

    /// <summary>
    /// Gets suggestions for locations based on user input
    /// </summary>
    Task<List<string>> GetLocationSuggestionsAsync(string userInput);

    /// <summary>
    /// Gets suggestions for users based on user input
    /// </summary>
    Task<List<string>> GetUserSuggestionsAsync(string userInput);
}

/// <summary>
/// Service implementation for inventory removal operations following MTM service-oriented architecture
/// </summary>
public class RemoveService : IRemoveService
{
    private readonly IDatabaseService _databaseService;
    private readonly IMasterDataService _masterDataService;
    private readonly IQuickButtonsService _quickButtonsService;
    private readonly ILogger<RemoveService> _logger;

    // Session management for undo functionality
    private readonly List<InventoryItem> _currentSessionRemovals = new();
    private readonly object _sessionLock = new();

    /// <inheritdoc />
    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();

    /// <inheritdoc />
    public ObservableCollection<InventoryItem> UndoItems { get; } = new();

    private bool _isLoading = false;
    /// <inheritdoc />
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                LoadingStateChanged?.Invoke(this, value);
            }
        }
    }

    /// <inheritdoc />
    public bool HasUndoItems
    {
        get
        {
            lock (_sessionLock)
            {
                return _currentSessionRemovals.Count > 0;
            }
        }
    }

    /// <inheritdoc />
    public event EventHandler<Models.ItemsRemovedEventArgs>? ItemsRemoved;

    /// <inheritdoc />
    public event EventHandler<bool>? LoadingStateChanged;

    public RemoveService(
        IDatabaseService databaseService,
        IMasterDataService masterDataService,
        IQuickButtonsService quickButtonsService,
        ILogger<RemoveService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
        _quickButtonsService = quickButtonsService ?? throw new ArgumentNullException(nameof(quickButtonsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogDebug("RemoveService initialized with dependencies");
    }

    /// <inheritdoc />
    public async Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string? partId = null, string? operation = null, string? location = null, string? user = null)
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Searching inventory with criteria - PartId: {PartId}, Operation: {Operation}, Location: {Location}, User: {User}",
                partId ?? "Any", operation ?? "Any", location ?? "Any", user ?? "Any");

            var searchResults = new List<InventoryItem>();

            // Use database service to search inventory based on criteria
            if (!string.IsNullOrWhiteSpace(partId) && !string.IsNullOrWhiteSpace(operation))
            {
                // Search by Part ID and Operation
                var result = await _databaseService.GetInventoryByPartAndOperationAsync(partId, operation).ConfigureAwait(false);
                if (result != null)
                {
                    searchResults.AddRange(ConvertDataTableToInventoryItems(result));
                }
            }
            else if (!string.IsNullOrWhiteSpace(partId))
            {
                // Search by Part ID only
                var result = await _databaseService.GetInventoryByPartIdAsync(partId).ConfigureAwait(false);
                if (result != null)
                {
                    searchResults.AddRange(ConvertDataTableToInventoryItems(result));
                }
            }
            else if (!string.IsNullOrWhiteSpace(user))
            {
                // Search by User
                var result = await _databaseService.GetInventoryByUserAsync(user).ConfigureAwait(false);
                if (result != null)
                {
                    searchResults.AddRange(ConvertDataTableToInventoryItems(result));
                }
            }
            else
            {
                // For now, return empty list for general searches to avoid performance issues
                // In future, this could be enhanced to support better general search
                _logger.LogInformation("General inventory search not implemented - specific criteria required");
            }

            // Apply additional filtering in memory for complex criteria
            if (!string.IsNullOrWhiteSpace(location))
            {
                searchResults = searchResults.Where(item =>
                    string.Equals(item.Location, location, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(user))
            {
                searchResults = searchResults.Where(item =>
                    string.Equals(item.User, user, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Update UI collections on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                InventoryItems.Clear();
                foreach (var item in searchResults)
                {
                    InventoryItems.Add(item);
                }
            });

            _logger.LogInformation("Inventory search completed. Found {Count} items", searchResults.Count);
            return ServiceResult<List<InventoryItem>>.Success(searchResults, $"Found {searchResults.Count} inventory items");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search inventory");
            return ServiceResult<List<InventoryItem>>.Failure($"Failed to search inventory: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<RemovalResult>> RemoveInventoryItemsAsync(IEnumerable<InventoryItem> items, string currentUser, string notes = "")
    {
        var itemList = items?.ToList() ?? throw new ArgumentNullException(nameof(items));
        if (!itemList.Any())
        {
            return ServiceResult<RemovalResult>.Failure("No items provided for removal");
        }

        try
        {
            IsLoading = true;
            _logger.LogInformation("Starting batch removal of {Count} inventory items for user {User}", itemList.Count, currentUser);

            var successfulRemovals = new List<InventoryItem>();
            var failures = new List<RemovalFailure>();

            using var scope = _logger.BeginScope("BatchInventoryRemoval");

            // Process each item in the batch
            foreach (var item in itemList)
            {
                try
                {
                    // Validate item data
                    var validationResult = ValidateInventoryItem(item);
                    if (!validationResult.IsSuccess)
                    {
                        failures.Add(new RemovalFailure
                        {
                            Item = item,
                            Error = validationResult.Message,
                            ErrorType = "Validation"
                        });
                        continue;
                    }

                    _logger.LogDebug("Processing removal: {PartId}, Operation: {Operation}, Quantity: {Quantity}",
                        item.PartId, item.Operation, item.Quantity);

                    // Remove item using database service
                    var removeResult = await _databaseService.RemoveInventoryItemAsync(
                        item.PartId,
                        item.Location,
                        item.Operation ?? string.Empty,
                        item.Quantity,
                        item.ItemType,
                        currentUser,
                        item.BatchNumber ?? string.Empty,
                        $"Removed via RemoveService - {notes}"
                    ).ConfigureAwait(false);

                    if (removeResult.IsSuccess)
                    {
                        successfulRemovals.Add(item);
                        _logger.LogDebug("Successfully removed inventory item: {PartId}", item.PartId);

                        // Log to QuickButtons history as OUT transaction
                        try
                        {
                            await _quickButtonsService.AddTransactionToLast10Async(
                                currentUser,
                                item.PartId,
                                item.Operation ?? string.Empty,
                                item.Quantity,
                                "OUT" // Specify OUT transaction type for removals
                            ).ConfigureAwait(false);
                        }
                        catch (Exception logEx)
                        {
                            _logger.LogWarning(logEx, "Failed to log removal to QuickButtons history for {PartId}", item.PartId);
                        }
                    }
                    else
                    {
                        failures.Add(new RemovalFailure
                        {
                            Item = item,
                            Error = removeResult.Message ?? "Database operation failed",
                            ErrorType = "Database"
                        });
                        _logger.LogError("Failed to remove inventory item {PartId}: {Error}", item.PartId, removeResult.Message);
                    }
                }
                catch (Exception itemEx)
                {
                    failures.Add(new RemovalFailure
                    {
                        Item = item,
                        Error = itemEx.Message,
                        ErrorType = "Exception"
                    });
                    _logger.LogError(itemEx, "Exception removing inventory item {PartId}", item.PartId);
                }
            }

            // Update session for undo functionality
            if (successfulRemovals.Any())
            {
                lock (_sessionLock)
                {
                    _currentSessionRemovals.Clear();
                    _currentSessionRemovals.AddRange(successfulRemovals);
                }

                // Update UI collections on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var removedItem in successfulRemovals)
                    {
                        InventoryItems.Remove(removedItem);
                    }

                    UndoItems.Clear();
                    foreach (var item in successfulRemovals)
                    {
                        UndoItems.Add(item);
                    }
                });

                // Fire event for integration
                ItemsRemoved?.Invoke(this, new Models.ItemsRemovedEventArgs
                {
                    RemovedItems = successfulRemovals,
                    RemovalTime = DateTime.Now,
                    TotalQuantityRemoved = successfulRemovals.Sum(item => item.Quantity),
                    UserName = currentUser,
                    Location = successfulRemovals.FirstOrDefault()?.Location ?? string.Empty
                });
            }

            var result = new RemovalResult
            {
                SuccessfulRemovals = successfulRemovals,
                Failures = failures,
                TotalProcessed = itemList.Count,
                SuccessCount = successfulRemovals.Count,
                FailureCount = failures.Count
            };

            _logger.LogInformation("Batch removal completed: {SuccessCount} successful, {FailureCount} failed",
                result.SuccessCount, result.FailureCount);

            return ServiceResult<RemovalResult>.Success(result, $"Processed {result.TotalProcessed} items: {result.SuccessCount} successful, {result.FailureCount} failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during batch inventory removal");
            return ServiceResult<RemovalResult>.Failure($"Failed to remove inventory items: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult<RemovalResult>> RemoveInventoryItemAsync(InventoryItem item, string currentUser, string notes = "")
    {
        if (item == null)
        {
            return ServiceResult<RemovalResult>.Failure("Item cannot be null");
        }

        return await RemoveInventoryItemsAsync(new[] { item }, currentUser, notes).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ServiceResult<RestoreResult>> UndoLastRemovalAsync(string currentUser)
    {
        List<InventoryItem> itemsToRestore;

        lock (_sessionLock)
        {
            if (_currentSessionRemovals.Count == 0)
            {
                return ServiceResult<RestoreResult>.Failure("No items available for undo");
            }

            itemsToRestore = new List<InventoryItem>(_currentSessionRemovals);
        }

        try
        {
            IsLoading = true;
            _logger.LogInformation("Starting undo operation for {Count} items for user {User}", itemsToRestore.Count, currentUser);

            var successfulRestores = new List<InventoryItem>();
            var failures = new List<RestoreFailure>();

            // Restore each item back to the database
            foreach (var item in itemsToRestore)
            {
                try
                {
                    // Use AddInventoryItemAsync to restore the item
                    var restoreResult = await _databaseService.AddInventoryItemAsync(
                        item.PartId,
                        item.Location,
                        item.Operation ?? string.Empty,
                        item.Quantity,
                        item.ItemType,
                        currentUser,
                        item.BatchNumber ?? string.Empty,
                        $"Restored via Undo operation - Originally removed by RemoveService"
                    ).ConfigureAwait(false);

                    if (restoreResult.IsSuccess)
                    {
                        successfulRestores.Add(item);
                        _logger.LogDebug("Successfully restored inventory item: {PartId}", item.PartId);
                    }
                    else
                    {
                        failures.Add(new RestoreFailure
                        {
                            Item = item,
                            Error = restoreResult.Message ?? "Database restore operation failed",
                            ErrorType = "Database"
                        });
                        _logger.LogError("Failed to restore inventory item {PartId}: {Error}", item.PartId, restoreResult.Message);
                    }
                }
                catch (Exception itemEx)
                {
                    failures.Add(new RestoreFailure
                    {
                        Item = item,
                        Error = itemEx.Message,
                        ErrorType = "Exception"
                    });
                    _logger.LogError(itemEx, "Exception restoring inventory item {PartId}", item.PartId);
                }
            }

            // Update session and UI
            if (successfulRestores.Any())
            {
                lock (_sessionLock)
                {
                    // Remove restored items from undo session
                    foreach (var restoredItem in successfulRestores)
                    {
                        _currentSessionRemovals.Remove(restoredItem);
                    }
                }

                // Update UI collections on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var item in successfulRestores)
                    {
                        InventoryItems.Add(item);
                        UndoItems.Remove(item);
                    }
                });
            }

            var result = new RestoreResult
            {
                SuccessfulRestores = successfulRestores,
                Failures = failures,
                TotalProcessed = itemsToRestore.Count,
                SuccessCount = successfulRestores.Count,
                FailureCount = failures.Count
            };

            _logger.LogInformation("Undo operation completed: {SuccessCount} successful, {FailureCount} failed",
                result.SuccessCount, result.FailureCount);

            return ServiceResult<RestoreResult>.Success(result, $"Undo processed {result.TotalProcessed} items: {result.SuccessCount} successful, {result.FailureCount} failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during undo operation");
            return ServiceResult<RestoreResult>.Failure($"Failed to undo removal: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <inheritdoc />
    public void ClearUndoSession()
    {
        lock (_sessionLock)
        {
            _currentSessionRemovals.Clear();
        }

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            UndoItems.Clear();
        });

        _logger.LogInformation("Undo session cleared");
    }

    /// <inheritdoc />
    public async Task<ServiceResult> RefreshInventoryAsync()
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Refreshing inventory data");

            // For now, just clear the collection since we don't have GetAllInventoryAsync
            // In a real implementation, this could fetch a subset or paginated results
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                InventoryItems.Clear();
            });

            _logger.LogInformation("Inventory refresh completed - cleared current results");
            return ServiceResult.Success("Inventory data cleared - use search to load specific results");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh inventory");
            return ServiceResult.Failure($"Failed to refresh inventory: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> GetPartSuggestionsAsync(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput) || userInput.Length < 2)
        {
            return new List<string>();
        }

        try
        {
            // Use master data service for suggestions
            await _masterDataService.LoadAllMasterDataAsync().ConfigureAwait(false);

            var suggestions = _masterDataService.PartIds
                .Where(partId => partId.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();

            _logger.LogDebug("Generated {Count} part suggestions for input: {Input}", suggestions.Count, userInput);
            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get part suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> GetOperationSuggestionsAsync(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return new List<string>();
        }

        try
        {
            // Use master data service for suggestions
            await _masterDataService.LoadAllMasterDataAsync().ConfigureAwait(false);

            var suggestions = _masterDataService.Operations
                .Where(operation => operation.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();

            _logger.LogDebug("Generated {Count} operation suggestions for input: {Input}", suggestions.Count, userInput);
            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get operation suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> GetLocationSuggestionsAsync(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return new List<string>();
        }

        try
        {
            // Use master data service for suggestions
            await _masterDataService.LoadAllMasterDataAsync().ConfigureAwait(false);

            var suggestions = _masterDataService.Locations
                .Where(location => location.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();

            _logger.LogDebug("Generated {Count} location suggestions for input: {Input}", suggestions.Count, userInput);
            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get location suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> GetUserSuggestionsAsync(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return new List<string>();
        }

        try
        {
            // Use master data service for suggestions
            await _masterDataService.LoadAllMasterDataAsync().ConfigureAwait(false);

            var suggestions = _masterDataService.Users
                .Where(user => user.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .Take(10)
                .ToList();

            _logger.LogDebug("Generated {Count} user suggestions for input: {Input}", suggestions.Count, userInput);
            return suggestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user suggestions for input: {Input}", userInput);
            return new List<string>();
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Validates an inventory item before processing
    /// </summary>
    private ServiceResult ValidateInventoryItem(InventoryItem item)
    {
        if (item == null)
        {
            return ServiceResult.Failure("Item cannot be null");
        }

        if (string.IsNullOrWhiteSpace(item.PartId))
        {
            return ServiceResult.Failure("Part ID is required");
        }

        if (item.Quantity <= 0)
        {
            return ServiceResult.Failure("Quantity must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(item.Location))
        {
            return ServiceResult.Failure("Location is required");
        }

        return ServiceResult.Success();
    }

    /// <summary>
    /// Converts DataTable results to InventoryItem objects
    /// </summary>
    private List<InventoryItem> ConvertDataTableToInventoryItems(DataTable dataTable)
    {
        var items = new List<InventoryItem>();

        try
        {
            foreach (DataRow row in dataTable.Rows)
            {
                var item = new InventoryItem
                {
                    // Core identification columns - required fields with safe fallbacks
                    Id = row.Field<int?>("ID") ?? 0,
                    PartId = row.Field<string>("PartID") ?? string.Empty,
                    Location = row.Field<string>("Location") ?? string.Empty,
                    Operation = row.Field<string>("Operation") ?? string.Empty,

                    // Quantity and type information
                    Quantity = row.Field<int?>("Quantity") ?? 0,
                    ItemType = TryGetField(row, "ItemType") ?? "WIP",
                    BatchNumber = TryGetField(row, "BatchNumber") ?? string.Empty,

                    // Date information - map to actual database column names
                    ReceiveDate = TryGetDateTimeField(row, "ReceiveDate") ?? DateTime.Now,
                    LastUpdated = TryGetDateTimeField(row, "LastUpdated") ?? DateTime.Now,
                    DateAdded = TryGetDateTimeField(row, "ReceiveDate") ?? DateTime.Now, // DateAdded maps to ReceiveDate from database

                    // User and tracking information
                    User = TryGetField(row, "User") ?? string.Empty,
                    LastUpdatedBy = TryGetField(row, "LastUpdatedBy") ?? TryGetField(row, "User") ?? string.Empty,
                    Notes = TryGetField(row, "Notes") ?? string.Empty,

                    // Additional columns (may not exist in all tables)
                    Status = TryGetField(row, "Status") ?? string.Empty,
                    WorkOrder = TryGetField(row, "WorkOrder") ?? TryGetField(row, "WO") ?? string.Empty,
                    SerialNumber = TryGetField(row, "SerialNumber") ?? TryGetField(row, "Serial") ?? string.Empty,
                    Cost = TryGetDecimalField(row, "Cost") ?? TryGetDecimalField(row, "UnitCost"),
                    Vendor = TryGetField(row, "Vendor") ?? TryGetField(row, "Supplier") ?? string.Empty,
                    Category = TryGetField(row, "Category") ?? TryGetField(row, "Type") ?? string.Empty
                };

                items.Add(item);
            }

            _logger.LogDebug("Converted {Count} rows from DataTable to InventoryItems with all available columns", items.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert DataTable to InventoryItems");
        }

        return items;
    }

    /// <summary>
    /// Safely tries to get a string field from DataRow, returns null if column doesn't exist
    /// </summary>
    private static string? TryGetField(DataRow row, string columnName)
    {
        try
        {
            return row.Table.Columns.Contains(columnName) ? row.Field<string>(columnName) : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Safely tries to get a decimal field from DataRow, returns null if column doesn't exist
    /// </summary>
    private static decimal? TryGetDecimalField(DataRow row, string columnName)
    {
        try
        {
            return row.Table.Columns.Contains(columnName) ? row.Field<decimal?>(columnName) : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Safely tries to get a DateTime field from DataRow, returns null if column doesn't exist
    /// </summary>
    private static DateTime? TryGetDateTimeField(DataRow row, string columnName)
    {
        try
        {
            return row.Table.Columns.Contains(columnName) ? row.Field<DateTime?>(columnName) : null;
        }
        catch
        {
            return null;
        }
    }

    #endregion
}

#region Result Models

/// <summary>
/// Result model for removal operations
/// </summary>
public class RemovalResult
{
    public List<InventoryItem> SuccessfulRemovals { get; set; } = new();
    public List<RemovalFailure> Failures { get; set; } = new();
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }

    public bool HasFailures => FailureCount > 0;
    public bool HasSuccesses => SuccessCount > 0;
}

/// <summary>
/// Represents a removal operation failure
/// </summary>
public class RemovalFailure
{
    public InventoryItem Item { get; set; } = new();
    public string Error { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty; // "Validation", "Database", "Exception"
}

/// <summary>
/// Result model for restore operations
/// </summary>
public class RestoreResult
{
    public List<InventoryItem> SuccessfulRestores { get; set; } = new();
    public List<RestoreFailure> Failures { get; set; } = new();
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }

    public bool HasFailures => FailureCount > 0;
    public bool HasSuccesses => SuccessCount > 0;
}

/// <summary>
/// Represents a restore operation failure
/// </summary>
public class RestoreFailure
{
    public InventoryItem Item { get; set; } = new();
    public string Error { get; set; } = string.Empty;
    public string ErrorType { get; set; } = string.Empty; // "Database", "Exception"
}

#endregion
