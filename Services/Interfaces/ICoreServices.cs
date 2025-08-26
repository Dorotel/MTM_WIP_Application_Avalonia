using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MTM_Shared_Logic.Models;

namespace MTM_Shared_Logic.Core.Services
{
    /// <summary>
    /// Interface for configuration management service.
    /// </summary>
    public interface IConfigurationService
    {
        string? GetValue(string key);
        T? GetValue<T>(string key);
        string? GetConnectionString(string name);
        T? GetSection<T>(string sectionName) where T : class, new();
        Result ValidateConfiguration();
        Task<Result> ReloadConfigurationAsync();
    }

    /// <summary>
    /// Interface for database service providing centralized data access.
    /// CRITICAL RULE: ALL database operations must use stored procedures - NO hard-coded SQL allowed.
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Tests the database connection.
        /// </summary>
        Task<Result<bool>> TestConnectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a stored procedure and returns the results.
        /// ENFORCES: Only stored procedures are allowed - NO direct SQL commands.
        /// </summary>
        Task<Result<List<T>>> ExecuteStoredProcedureAsync<T>(
            string procedureName, 
            Dictionary<string, object>? parameters = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a stored procedure with status output parameters.
        /// Returns both the result set and status information from the procedure.
        /// </summary>
        Task<Result<StoredProcedureResult<T>>> ExecuteStoredProcedureWithStatusAsync<T>(
            string procedureName,
            Dictionary<string, object>? parameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a stored procedure that returns a single scalar value.
        /// ENFORCES: Only stored procedures are allowed - NO direct SQL commands.
        /// </summary>
        Task<Result<T?>> ExecuteStoredProcedureScalarAsync<T>(
            string procedureName,
            Dictionary<string, object>? parameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a stored procedure for non-query operations (INSERT, UPDATE, DELETE).
        /// Returns the number of affected rows.
        /// ENFORCES: Only stored procedures are allowed - NO direct SQL commands.
        /// </summary>
        Task<Result<int>> ExecuteStoredProcedureNonQueryAsync(
            string procedureName,
            Dictionary<string, object>? parameters = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes multiple stored procedures within a single transaction.
        /// Ensures data consistency across multiple database operations.
        /// </summary>
        Task<Result> ExecuteTransactionAsync(
            Func<IDbConnection, IDbTransaction, Task> operations,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes multiple stored procedures within a single transaction and returns a result.
        /// </summary>
        Task<Result<T>> ExecuteTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, Task<T>> operations,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// DEPRECATED: This method is provided for backward compatibility only.
        /// DO NOT USE: Direct query execution violates the "no hard-coded MySQL" rule.
        /// Use ExecuteStoredProcedureAsync instead.
        /// </summary>
        // [Obsolete("Direct query execution is prohibited. Use ExecuteStoredProcedureAsync instead.", error: true)]
        Task<Result<List<T>>> ExecuteQueryAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// DEPRECATED: This method is provided for backward compatibility only.
        /// DO NOT USE: Direct command execution violates the "no hard-coded MySQL" rule.
        /// Use ExecuteStoredProcedureNonQueryAsync instead.
        /// </summary>
        // [Obsolete("Direct command execution is prohibited. Use ExecuteStoredProcedureNonQueryAsync instead.", error: true)]
        Task<Result<int>> ExecuteNonQueryAsync(string command, object? parameters = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// DEPRECATED: This method is provided for backward compatibility only.
        /// DO NOT USE: Direct scalar query execution violates the "no hard-coded MySQL" rule.
        /// Use ExecuteStoredProcedureScalarAsync instead.
        /// </summary>
        // [Obsolete("Direct scalar query execution is prohibited. Use ExecuteStoredProcedureScalarAsync instead.", error: true)]
        Task<Result<T?>> ExecuteScalarAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Result container for stored procedures that return both data and status information.
    /// </summary>
    public class StoredProcedureResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object> OutputParameters { get; set; } = new();
        
        public bool IsSuccess => Status == 0;
        public bool HasData => Data.Count > 0;
    }

    /// <summary>
    /// Interface for application state management service.
    /// </summary>
    public interface IApplicationStateService
    {
        User? CurrentUser { get; }
        bool IsUserLoggedIn { get; }
        ConnectionStatus ConnectionStatus { get; }
        Dictionary<string, object> Settings { get; }

        event EventHandler<UserChangedEventArgs>? CurrentUserChanged;
        event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;
        event EventHandler<SettingChangedEventArgs>? SettingChanged;

        void SetCurrentUser(User? user);
        void SetConnectionStatus(ConnectionStatus status);
        T? GetSetting<T>(string key);
        void SetSetting(string key, object value);
        void Clear();
    }

    /// <summary>
    /// Interface for caching service.
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<Result> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task<Result> RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<Result> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
        Task<Result> ClearAllAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for validation service.
    /// </summary>
    public interface IValidationService
    {
        Task<Result<ValidationResult>> ValidateAsync<T>(T entity, CancellationToken cancellationToken = default);
        Task<Result<bool>> ValidateRuleAsync(string ruleName, object context, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for database connection factory.
    /// </summary>
    public interface IDbConnectionFactory
    {
        Task<System.Data.Common.DbConnection> CreateConnectionAsync();
        string ConnectionString { get; }
    }

    /// <summary>
    /// Event argument classes for application state service.
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

    public class SettingChangedEventArgs : EventArgs
    {
        public string Key { get; }
        public object? PreviousValue { get; }
        public object CurrentValue { get; }

        public SettingChangedEventArgs(string key, object? previousValue, object currentValue)
        {
            Key = key;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }
}