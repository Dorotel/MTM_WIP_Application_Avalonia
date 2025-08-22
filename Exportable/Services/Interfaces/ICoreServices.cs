using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MTM.Core.Models;
using System.Linq;

namespace MTM.Core.Services
{
    /// <summary>
    /// Interface for user management operations.
    /// Provides CRUD operations and authentication-related functionality for users.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the user if found</returns>
        Task<Result<User>> GetUserAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the user if found</returns>
        Task<Result<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all active users.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the list of active users</returns>
        Task<Result<IEnumerable<User>>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the created user</returns>
        Task<Result<User>> CreateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> UpdateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates a user (soft delete).
        /// </summary>
        /// <param name="userId">The unique identifier of the user to deactivate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates a user with username and password.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the authenticated user if successful</returns>
        Task<Result<User>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Records a login for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> RecordLoginAsync(string userId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for configuration management operations.
    /// Provides access to application settings and configuration values.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets a configuration value by key.
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <returns>The configuration value or null if not found</returns>
        string? GetValue(string key);

        /// <summary>
        /// Gets a strongly-typed configuration value.
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="key">The configuration key</param>
        /// <returns>The typed configuration value or default if not found</returns>
        T? GetValue<T>(string key);

        /// <summary>
        /// Gets a connection string by name.
        /// </summary>
        /// <param name="name">The connection string name</param>
        /// <returns>The connection string or null if not found</returns>
        string? GetConnectionString(string name);

        /// <summary>
        /// Gets a configuration section as a strongly-typed object.
        /// </summary>
        /// <typeparam name="T">The type to bind to</typeparam>
        /// <param name="sectionName">The section name</param>
        /// <returns>The bound configuration object</returns>
        T? GetSection<T>(string sectionName) where T : class, new();

        /// <summary>
        /// Validates the current configuration.
        /// </summary>
        /// <returns>A result indicating if configuration is valid</returns>
        Result ValidateConfiguration();

        /// <summary>
        /// Reloads configuration from source.
        /// </summary>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> ReloadConfigurationAsync();
    }

    /// <summary>
    /// Interface for database operations.
    /// Provides data access functionality with connection management.
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Executes a query and returns a single result.
        /// </summary>
        /// <typeparam name="T">The type of result</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the query result</returns>
        Task<Result<T?>> QuerySingleAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a query and returns multiple results.
        /// </summary>
        /// <typeparam name="T">The type of result</typeparam>
        /// <param name="sql">The SQL query</param>
        /// <param name="parameters">Query parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the query results</returns>
        Task<Result<IEnumerable<T>>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a command that doesn't return data.
        /// </summary>
        /// <param name="sql">The SQL command</param>
        /// <param name="parameters">Command parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing the number of affected rows</returns>
        Task<Result<int>> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes multiple commands within a transaction.
        /// </summary>
        /// <param name="commands">The commands to execute</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> ExecuteTransactionAsync(IEnumerable<(string Sql, object? Parameters)> commands, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tests the database connection.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating if connection is successful</returns>
        Task<Result> TestConnectionAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for application state management.
    /// Provides centralized state management across the application.
    /// </summary>
    public interface IApplicationStateService
    {
        /// <summary>
        /// Gets the current user.
        /// </summary>
        User? CurrentUser { get; }

        /// <summary>
        /// Gets a value indicating whether a user is logged in.
        /// </summary>
        bool IsUserLoggedIn { get; }

        /// <summary>
        /// Gets the current connection status.
        /// </summary>
        ConnectionStatus ConnectionStatus { get; }

        /// <summary>
        /// Gets application-wide settings.
        /// </summary>
        Dictionary<string, object> Settings { get; }

        /// <summary>
        /// Sets the current user.
        /// </summary>
        /// <param name="user">The user to set as current</param>
        void SetCurrentUser(User? user);

        /// <summary>
        /// Sets the connection status.
        /// </summary>
        /// <param name="status">The connection status</param>
        void SetConnectionStatus(ConnectionStatus status);

        /// <summary>
        /// Gets a setting value.
        /// </summary>
        /// <typeparam name="T">The type of the setting</typeparam>
        /// <param name="key">The setting key</param>
        /// <returns>The setting value or default</returns>
        T? GetSetting<T>(string key);

        /// <summary>
        /// Sets a setting value.
        /// </summary>
        /// <param name="key">The setting key</param>
        /// <param name="value">The setting value</param>
        void SetSetting(string key, object value);

        /// <summary>
        /// Clears all application state.
        /// </summary>
        void Clear();

        /// <summary>
        /// Event raised when the current user changes.
        /// </summary>
        event EventHandler<UserChangedEventArgs>? CurrentUserChanged;

        /// <summary>
        /// Event raised when the connection status changes.
        /// </summary>
        event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;

        /// <summary>
        /// Event raised when a setting changes.
        /// </summary>
        event EventHandler<SettingChangedEventArgs>? SettingChanged;
    }

    /// <summary>
    /// Interface for caching operations.
    /// Provides memory and distributed caching functionality.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a cached value.
        /// </summary>
        /// <typeparam name="T">The type of the cached value</typeparam>
        /// <param name="key">The cache key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The cached value or null if not found</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a cached value.
        /// </summary>
        /// <typeparam name="T">The type of the value to cache</typeparam>
        /// <param name="key">The cache key</param>
        /// <param name="value">The value to cache</param>
        /// <param name="expiration">Cache expiration time</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a cached value.
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all cached values matching a pattern.
        /// </summary>
        /// <param name="pattern">The pattern to match</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears all cached values.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating success or failure</returns>
        Task<Result> ClearAllAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for validation operations.
    /// Provides business rule validation functionality.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates an entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to validate</typeparam>
        /// <param name="entity">The entity to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result containing validation results</returns>
        Task<Result<ValidationResult>> ValidateAsync<T>(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates a specific business rule.
        /// </summary>
        /// <param name="ruleName">The name of the rule to validate</param>
        /// <param name="context">The validation context</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A result indicating if the rule is valid</returns>
        Task<Result<bool>> ValidateRuleAsync(string ruleName, object context, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents connection status states.
    /// </summary>
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Failed
    }

    /// <summary>
    /// Event arguments for user changes.
    /// </summary>
    public class UserChangedEventArgs : EventArgs
    {
        public User? PreviousUser { get; }
        public User? CurrentUser { get; }

        public UserChangedEventArgs(User? previousUser, User? currentUser)
        {
            PreviousUser = previousUser;
            CurrentUser = currentUser;
        }
    }

    /// <summary>
    /// Event arguments for connection status changes.
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatus PreviousStatus { get; }
        public ConnectionStatus CurrentStatus { get; }

        public ConnectionStatusChangedEventArgs(ConnectionStatus previousStatus, ConnectionStatus currentStatus)
        {
            PreviousStatus = previousStatus;
            CurrentStatus = currentStatus;
        }
    }

    /// <summary>
    /// Event arguments for setting changes.
    /// </summary>
    public class SettingChangedEventArgs : EventArgs
    {
        public string Key { get; }
        public object? PreviousValue { get; }
        public object? CurrentValue { get; }

        public SettingChangedEventArgs(string key, object? previousValue, object? currentValue)
        {
            Key = key;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    /// <summary>
    /// Represents validation results.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; }
        public IReadOnlyList<string> Errors { get; }

        public ValidationResult(bool isValid, IEnumerable<string>? errors = null)
        {
            IsValid = isValid;
            Errors = errors?.ToList() ?? new List<string>();
        }

        public static ValidationResult Success() => new(true);
        public static ValidationResult Failure(params string[] errors) => new(false, errors);
        public static ValidationResult Failure(IEnumerable<string> errors) => new(false, errors);
    }
}