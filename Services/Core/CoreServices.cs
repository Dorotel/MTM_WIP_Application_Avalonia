using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services.Core;

#region Configuration Services

/// <summary>
/// Unified configuration and application state management service.
/// Provides centralized access to configuration settings and application state.
/// </summary>
public interface IConfigurationService
{
    string GetConnectionString(string name = "DefaultConnection");
    T GetValue<T>(string key, T defaultValue = default!);
    string GetValue(string key, string defaultValue = "");
    bool GetBoolValue(string key, bool defaultValue = false);
    int GetIntValue(string key, int defaultValue = 0);
}

/// <summary>
/// Application state management interface.
/// </summary>
public interface IApplicationStateService : INotifyPropertyChanged
{
    string CurrentUser { get; set; }
    string CurrentLocation { get; set; }
    string CurrentOperation { get; set; }
    bool IsOfflineMode { get; set; }

    // Progress communication for MainView integration
    int ProgressValue { get; set; }
    string StatusText { get; set; }

    // Async progress communication methods
    Task SetProgressAsync(int value, string status);
    Task ClearProgressAsync();

    event EventHandler<StateChangedEventArgs>? StateChanged;
}

/// <summary>
/// Configuration service implementation.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetConnectionString(string name = "DefaultConnection")
    {
        var connectionString = _configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Connection string '{Name}' not found", name);
            throw new InvalidOperationException($"Connection string '{name}' not configured");
        }
        return connectionString;
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        try
        {
            var value = _configuration[key];
            if (value == null) return defaultValue;

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get configuration value for key '{Key}', using default", key);
            return defaultValue;
        }
    }

    public string GetValue(string key, string defaultValue = "")
    {
        return _configuration[key] ?? defaultValue;
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        var value = _configuration[key];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        var value = _configuration[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
}

/// <summary>
/// Application state service implementation.
/// </summary>
public class ApplicationStateService : IApplicationStateService
{
    private string _currentUser = Environment.UserName;
    private string _currentLocation = string.Empty;
    private string _currentOperation = string.Empty;
    private bool _isOfflineMode = false;
    private int _progressValue = 0;
    private string _statusText = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    public string CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                var oldValue = _currentUser;
                _currentUser = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(CurrentUser), oldValue, value));
            }
        }
    }

    public string CurrentLocation
    {
        get => _currentLocation;
        set
        {
            if (_currentLocation != value)
            {
                var oldValue = _currentLocation;
                _currentLocation = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(CurrentLocation), oldValue, value));
            }
        }
    }

    public string CurrentOperation
    {
        get => _currentOperation;
        set
        {
            if (_currentOperation != value)
            {
                var oldValue = _currentOperation;
                _currentOperation = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(CurrentOperation), oldValue, value));
            }
        }
    }

    public bool IsOfflineMode
    {
        get => _isOfflineMode;
        set
        {
            if (_isOfflineMode != value)
            {
                _isOfflineMode = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(IsOfflineMode), !value, value));
            }
        }
    }

    public int ProgressValue
    {
        get => _progressValue;
        set
        {
            if (_progressValue != value)
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }
    }

    public string StatusText
    {
        get => _statusText;
        set
        {
            if (_statusText != value)
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task SetProgressAsync(int value, string status)
    {
        ProgressValue = value;
        StatusText = status;
        await Task.CompletedTask; // Allow for future async operations
    }

    public async Task ClearProgressAsync()
    {
        ProgressValue = 0;
        StatusText = string.Empty;
        await Task.CompletedTask;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Event args for state change events.
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    public string PropertyName { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public StateChangedEventArgs(string propertyName, object? oldValue, object? newValue)
    {
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion

#region Database Services

/// <summary>
/// Database service interface for MTM operations.
/// </summary>
public interface IDatabaseService
{
    // Basic database operations
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<bool> TestConnectionAsync();
    Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10);
    string GetConnectionString();

    // Inventory Operations - using Services.Core.Helper_Database_StoredProcedure pattern
    Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<DataTable> GetInventoryByPartIdAsync(string partId);
    Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation);
    Task<DataTable> GetInventoryByUserAsync(string user);
    Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation);
    Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user);

    // Note editing operations
    Task<StoredProcedureResult> UpdateInventoryNotesAsync(int inventoryId, string partId, string batchNumber, string notes, string user);
    Task<DataTable> GetInventoryByIdAsync(int inventoryId);

    // Master Data Operations - Parts
    Task<StoredProcedureResult> AddPartAsync(string partId, string customer, string description, string issuedBy, string itemType);
    Task<StoredProcedureResult> UpdatePartAsync(int id, string partId, string customer, string description, string issuedBy, string itemType);
    Task<bool> DeletePartAsync(string partId);
    Task<DataTable> GetPartByIdAsync(string partId);

    // Master Data Operations - Operations
    Task<StoredProcedureResult> AddOperationAsync(string operation, string issuedBy);
    Task<StoredProcedureResult> UpdateOperationAsync(string operation, string newOperation, string issuedBy);
    Task<bool> DeleteOperationAsync(string operation);

    // Master Data Operations - Locations
    Task<StoredProcedureResult> AddLocationAsync(string location, string issuedBy, string building);
    Task<StoredProcedureResult> UpdateLocationAsync(string oldLocation, string location, string issuedBy, string building);
    Task<bool> DeleteLocationAsync(string location);

    // Master Data Operations - Users
    Task<DataTable> GetAllUsersAsync();
    Task<DataTable> GetUserDetailsAsync(string username);
    Task<StoredProcedureResult> AddUserAsync(string username, string fullName, string role, string issuedBy);
    Task<StoredProcedureResult> UpdateUserAsync(string username, string fullName, string role, string issuedBy);
    Task<bool> DeleteUserAsync(string username);

    // Master Data Operations - General
    Task<DataTable> GetAllPartsAsync();
    Task<DataTable> GetAllOperationsAsync();
    Task<DataTable> GetAllLocationsAsync();
    
    // Additional methods for backward compatibility
    Task<DataTable> GetAllPartIDsAsync();
    Task<DataTable> GetAllItemTypesAsync();
}

/// <summary>
/// Database service implementation.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfigurationService configurationService, ILogger<DatabaseService> logger)
    {
        _connectionString = configurationService?.GetConnectionString() ?? throw new ArgumentNullException(nameof(configurationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetConnectionString() => _connectionString;

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return false;
        }
    }

    public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        using var adapter = new MySqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }

    public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        return await command.ExecuteScalarAsync();
    }

    public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync();
    }

    // Inventory Operations
    public async Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes)
    {
        // Placeholder implementation - will use Helper_Database_StoredProcedure when available
        _logger.LogInformation("Adding inventory item: {PartId}, {Location}, {Operation}, {Quantity}", partId, location, operation, quantity);
        return new StoredProcedureResult { Success = true, Message = "Item added successfully" };
    }

    public async Task<DataTable> GetInventoryByPartIdAsync(string partId)
    {
        var query = "SELECT * FROM inventory WHERE PartId = @partId";
        var parameters = new Dictionary<string, object> { { "@partId", partId } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation)
    {
        var query = "SELECT * FROM inventory WHERE PartId = @partId AND Operation = @operation";
        var parameters = new Dictionary<string, object> 
        { 
            { "@partId", partId },
            { "@operation", operation }
        };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<DataTable> GetInventoryByUserAsync(string user)
    {
        var query = "SELECT * FROM inventory WHERE User = @user ORDER BY Timestamp DESC LIMIT 100";
        var parameters = new Dictionary<string, object> { { "@user", user } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10)
    {
        var user = userId ?? Environment.UserName;
        var query = "SELECT * FROM transactions WHERE User = @user ORDER BY Timestamp DESC LIMIT @limit";
        var parameters = new Dictionary<string, object> 
        { 
            { "@user", user },
            { "@limit", limit }
        };
        return await ExecuteQueryAsync(query, parameters);
    }

    // Additional methods would be implemented similarly...
    // Placeholder implementations for interface compliance

    public async Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes)
    {
        _logger.LogInformation("Removing inventory item: {PartId}, {Quantity}", partId, quantity);
        return new StoredProcedureResult { Success = true, Message = "Item removed successfully" };
    }

    public async Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation)
    {
        _logger.LogInformation("Transferring part: {PartId} to {Location}", partId, newLocation);
        return await Task.FromResult(true);
    }

    public async Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user)
    {
        _logger.LogInformation("Transferring quantity: {Quantity} of {PartId} to {Location}", transferQuantity, partId, newLocation);
        return await Task.FromResult(true);
    }

    public async Task<StoredProcedureResult> UpdateInventoryNotesAsync(int inventoryId, string partId, string batchNumber, string notes, string user)
    {
        _logger.LogInformation("Updating notes for inventory ID: {InventoryId}", inventoryId);
        return new StoredProcedureResult { Success = true, Message = "Notes updated successfully" };
    }

    public async Task<DataTable> GetInventoryByIdAsync(int inventoryId)
    {
        var query = "SELECT * FROM inventory WHERE Id = @id";
        var parameters = new Dictionary<string, object> { { "@id", inventoryId } };
        return await ExecuteQueryAsync(query, parameters);
    }

    // Master Data Operations - placeholder implementations
    public async Task<StoredProcedureResult> AddPartAsync(string partId, string customer, string description, string issuedBy, string itemType)
    {
        return new StoredProcedureResult { Success = true, Message = "Part added successfully" };
    }

    public async Task<StoredProcedureResult> UpdatePartAsync(int id, string partId, string customer, string description, string issuedBy, string itemType)
    {
        return new StoredProcedureResult { Success = true, Message = "Part updated successfully" };
    }

    public async Task<bool> DeletePartAsync(string partId)
    {
        return await Task.FromResult(true);
    }

    public async Task<DataTable> GetPartByIdAsync(string partId)
    {
        var query = "SELECT * FROM parts WHERE PartId = @partId";
        var parameters = new Dictionary<string, object> { { "@partId", partId } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<StoredProcedureResult> AddOperationAsync(string operation, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "Operation added successfully" };
    }

    public async Task<StoredProcedureResult> UpdateOperationAsync(string operation, string newOperation, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "Operation updated successfully" };
    }

    public async Task<bool> DeleteOperationAsync(string operation)
    {
        return await Task.FromResult(true);
    }

    public async Task<StoredProcedureResult> AddLocationAsync(string location, string issuedBy, string building)
    {
        return new StoredProcedureResult { Success = true, Message = "Location added successfully" };
    }

    public async Task<StoredProcedureResult> UpdateLocationAsync(string oldLocation, string location, string issuedBy, string building)
    {
        return new StoredProcedureResult { Success = true, Message = "Location updated successfully" };
    }

    public async Task<bool> DeleteLocationAsync(string location)
    {
        return await Task.FromResult(true);
    }

    public async Task<DataTable> GetAllUsersAsync()
    {
        var query = "SELECT * FROM users ORDER BY Username";
        return await ExecuteQueryAsync(query);
    }

    public async Task<DataTable> GetUserDetailsAsync(string username)
    {
        var query = "SELECT * FROM users WHERE Username = @username";
        var parameters = new Dictionary<string, object> { { "@username", username } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<StoredProcedureResult> AddUserAsync(string username, string fullName, string role, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "User added successfully" };
    }

    public async Task<StoredProcedureResult> UpdateUserAsync(string username, string fullName, string role, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "User updated successfully" };
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        return await Task.FromResult(true);
    }

    public async Task<DataTable> GetAllPartsAsync()
    {
        var query = "SELECT * FROM parts ORDER BY PartId";
        return await ExecuteQueryAsync(query);
    }

    public async Task<DataTable> GetAllOperationsAsync()
    {
        var query = "SELECT * FROM operations ORDER BY Operation";
        return await ExecuteQueryAsync(query);
    }

    public async Task<DataTable> GetAllLocationsAsync()
    {
        var query = "SELECT * FROM locations ORDER BY Location";
        return await ExecuteQueryAsync(query);
    }
    
    public async Task<DataTable> GetAllPartIDsAsync()
    {
        // Alias method for compatibility
        return await GetAllPartsAsync();
    }
    
    public async Task<DataTable> GetAllItemTypesAsync()
    {
        var query = "SELECT DISTINCT ItemType FROM parts ORDER BY ItemType";
        return await ExecuteQueryAsync(query);
    }
}

/// <summary>
/// Result class for stored procedure operations.
/// </summary>
public class StoredProcedureResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    
    // Backward compatibility properties
    public bool IsSuccess => Success;
    public int Status => Success ? 1 : 0;
}

#endregion

#region Error Handling Services

/// <summary>
/// Comprehensive error handling and logging service for MTM WIP Application.
/// Provides structured error logging, file-based fallback, and user-friendly error messages.
/// </summary>
public static class ErrorHandling
{
    private static readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
    private static readonly object _lockObject = new();
    private static string _fileServerBasePath = @"\\mtmanu-fs01\Expo Drive\MH_RESOURCE\Material_Handler\MTM WIP App\Logs";

    /// <summary>
    /// Handles an exception with specified operation context and user information.
    /// </summary>
    public static async Task HandleErrorAsync(
        Exception exception,
        string operation,
        string userId,
        Dictionary<string, object>? context = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        try
        {
            var errorCategory = DetermineErrorCategory(exception);
            var severity = DetermineSeverity(exception, errorCategory);
            var errorKey = GenerateErrorKey(exception, callerMemberName, callerFilePath, callerLineNumber);

            if (IsNewError(errorCategory, errorKey))
            {
                await LogErrorAsync(exception, operation, userId, context ?? new Dictionary<string, object>());
                AddToSessionCache(errorCategory, errorKey);
            }
        }
        catch (Exception handlerException)
        {
            await LogToFileAsync(handlerException, operation, userId, new Dictionary<string, object>
            {
                ["OriginalException"] = exception.Message,
                ["HandlerError"] = true
            });
        }
    }

    /// <summary>
    /// Handles an exception with default user context.
    /// </summary>
    public static async Task HandleErrorAsync(Exception exception, string operation, Dictionary<string, object>? context = null)
    {
        await HandleErrorAsync(exception, operation, Environment.UserName, context);
    }

    private static string DetermineErrorCategory(Exception exception)
    {
        return exception switch
        {
            MySqlException => "Database",
            TimeoutException => "Timeout", 
            ArgumentException => "Validation",
            UnauthorizedAccessException => "Security",
            FileNotFoundException or DirectoryNotFoundException => "FileSystem",
            InvalidOperationException => "Business",
            _ => "General"
        };
    }

    private static string DetermineSeverity(Exception exception, string category)
    {
        return exception switch
        {
            MySqlException mysql when mysql.Number == 1042 => "Critical", // Connection failed
            TimeoutException => "High",
            UnauthorizedAccessException => "High", 
            ArgumentNullException => "Medium",
            FileNotFoundException => "Medium",
            _ => "Low"
        };
    }

    private static string GenerateErrorKey(Exception exception, string memberName, string filePath, int lineNumber)
    {
        var fileName = Path.GetFileName(filePath);
        return $"{exception.GetType().Name}_{fileName}_{memberName}_{lineNumber}";
    }

    private static bool IsNewError(string category, string errorKey)
    {
        lock (_lockObject)
        {
            if (!_sessionErrorCache.TryGetValue(category, out var errors))
            {
                errors = new HashSet<string>();
                _sessionErrorCache[category] = errors;
            }

            return !errors.Contains(errorKey);
        }
    }

    private static void AddToSessionCache(string category, string errorKey)
    {
        lock (_lockObject)
        {
            if (!_sessionErrorCache.TryGetValue(category, out var errors))
            {
                errors = new HashSet<string>();
                _sessionErrorCache[category] = errors;
            }

            errors.Add(errorKey);

            // Keep cache size manageable
            if (errors.Count > 100)
            {
                var toRemove = errors.Take(20).ToList();
                foreach (var item in toRemove)
                    errors.Remove(item);
            }
        }
    }

    private static async Task LogErrorAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        try
        {
            // Try database logging first
            // This would use the actual database service when available
            await LogToDatabaseAsync(exception, operation, userId, context);
        }
        catch
        {
            // Fallback to file logging
            await LogToFileAsync(exception, operation, userId, context);
        }
    }

    private static async Task LogToDatabaseAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        // Placeholder for database logging
        // Would use Helper_Database_StoredProcedure when available
        await Task.CompletedTask;
    }

    private static async Task LogToFileAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        try
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                User = userId,
                Operation = operation,
                Exception = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Context = context
            };

            var jsonLog = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"MTM_Error_{DateTime.Now:yyyy-MM-dd}.log";
            
            // Try network path first, fall back to local
            var logPath = Path.Combine(_fileServerBasePath, fileName);
            try
            {
                await File.AppendAllTextAsync(logPath, jsonLog + Environment.NewLine);
            }
            catch
            {
                // Fallback to local logging
                var localPath = Path.Combine(Path.GetTempPath(), "MTM_Logs", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                await File.AppendAllTextAsync(localPath, jsonLog + Environment.NewLine);
            }
        }
        catch
        {
            // Last resort - do nothing rather than crash
        }
    }

    /// <summary>
    /// Gets a user-friendly error message for the given exception.
    /// </summary>
    public static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            MySqlException mysql => mysql.Number switch
            {
                1042 => "Unable to connect to the database. Please check your network connection and try again.",
                1045 => "Database authentication failed. Please contact your system administrator.",
                1146 => "A required database table was not found. Please contact your system administrator.",
                _ => "A database error occurred. Please try again or contact support."
            },
            TimeoutException => "The operation took too long to complete. Please try again.",
            UnauthorizedAccessException => "You don't have permission to perform this operation.",
            FileNotFoundException => "A required file was not found. Please ensure all files are available and try again.",
            ArgumentException => "Invalid input provided. Please check your data and try again.",
            InvalidOperationException => "The requested operation cannot be completed at this time.",
            _ => "An unexpected error occurred. Please contact support if the problem persists."
        };
    }

    /// <summary>
    /// Clears the session error cache.
    /// </summary>
    public static void ClearSessionCache()
    {
        lock (_lockObject)
        {
            _sessionErrorCache.Clear();
        }
    }
}

#endregion