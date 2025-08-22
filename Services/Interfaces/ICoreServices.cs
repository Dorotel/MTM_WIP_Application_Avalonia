using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MTM.Models;

namespace MTM.Core.Services
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