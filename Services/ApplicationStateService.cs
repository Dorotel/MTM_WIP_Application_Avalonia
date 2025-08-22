using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using MTM.Models;
using MTM.Core.Services;

namespace MTM.Services
{
    /// <summary>
    /// Application state service implementation providing centralized state management with ReactiveUI patterns.
    /// Thread-safe implementation with reactive property notifications.
    /// </summary>
    public class ApplicationStateService : ReactiveObject, IApplicationStateService
    {
        private readonly ILogger<ApplicationStateService> _logger;
        private readonly ReaderWriterLockSlim _lock = new();
        
        // Reactive properties
        private User? _currentUser;
        private ConnectionStatus _connectionStatus = ConnectionStatus.Disconnected;
        private readonly Dictionary<string, object> _settings = new();

        // Reactive subjects for broadcasting changes
        private readonly Subject<UserChangedEventArgs> _userChangedSubject = new();
        private readonly Subject<ConnectionStatusChangedEventArgs> _connectionStatusChangedSubject = new();
        private readonly Subject<SettingChangedEventArgs> _settingChangedSubject = new();

        public ApplicationStateService(ILogger<ApplicationStateService> logger)
        {
            _logger = logger;
            _logger.LogInformation("ApplicationStateService initialized");
        }

        /// <summary>
        /// Gets the current authenticated user.
        /// </summary>
        public User? CurrentUser
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _currentUser;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            private set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }

        /// <summary>
        /// Gets a value indicating whether a user is currently logged in.
        /// </summary>
        public bool IsUserLoggedIn => CurrentUser != null;

        /// <summary>
        /// Gets the current connection status.
        /// </summary>
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _connectionStatus;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            private set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
        }

        /// <summary>
        /// Gets the application settings dictionary.
        /// </summary>
        public Dictionary<string, object> Settings
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return new Dictionary<string, object>(_settings);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Event fired when the current user changes.
        /// </summary>
        public event EventHandler<UserChangedEventArgs>? CurrentUserChanged
        {
            add => _userChangedSubject.Subscribe(args => value?.Invoke(this, args));
            remove { /* ReactiveUI handles unsubscription */ }
        }

        /// <summary>
        /// Event fired when the connection status changes.
        /// </summary>
        public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged
        {
            add => _connectionStatusChangedSubject.Subscribe(args => value?.Invoke(this, args));
            remove { /* ReactiveUI handles unsubscription */ }
        }

        /// <summary>
        /// Event fired when a setting value changes.
        /// </summary>
        public event EventHandler<SettingChangedEventArgs>? SettingChanged
        {
            add => _settingChangedSubject.Subscribe(args => value?.Invoke(this, args));
            remove { /* ReactiveUI handles unsubscription */ }
        }

        /// <summary>
        /// Sets the current authenticated user.
        /// </summary>
        public void SetCurrentUser(User? user)
        {
            _lock.EnterWriteLock();
            try
            {
                var previousUser = _currentUser;
                CurrentUser = user;

                _logger.LogInformation("Current user changed from {PreviousUser} to {CurrentUser}", 
                    previousUser?.UserName ?? "null", user?.UserName ?? "null");

                // Fire event using reactive subject
                var eventArgs = new UserChangedEventArgs(previousUser, user);
                _userChangedSubject.OnNext(eventArgs);

                // Also fire PropertyChanged for IsUserLoggedIn
                this.RaisePropertyChanged(nameof(IsUserLoggedIn));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sets the connection status.
        /// </summary>
        public void SetConnectionStatus(ConnectionStatus status)
        {
            _lock.EnterWriteLock();
            try
            {
                var previousStatus = _connectionStatus;
                ConnectionStatus = status;

                _logger.LogInformation("Connection status changed from {PreviousStatus} to {CurrentStatus}", 
                    previousStatus, status);

                // Fire event using reactive subject
                var eventArgs = new ConnectionStatusChangedEventArgs(previousStatus, status);
                _connectionStatusChangedSubject.OnNext(eventArgs);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets a setting value with type conversion.
        /// </summary>
        public T? GetSetting<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default;
            }

            _lock.EnterReadLock();
            try
            {
                if (!_settings.TryGetValue(key, out var value))
                {
                    return default;
                }

                // Handle type conversion
                if (value is T directValue)
                {
                    return directValue;
                }

                // Attempt type conversion
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to convert setting {Key} from {FromType} to {ToType}: {Error}", 
                        key, value.GetType().Name, typeof(T).Name, ex.Message);
                    return default;
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Sets a setting value.
        /// </summary>
        public void SetSetting(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Setting key cannot be null or empty", nameof(key));
            }

            _lock.EnterWriteLock();
            try
            {
                var previousValue = _settings.TryGetValue(key, out var prev) ? prev : null;
                _settings[key] = value;

                _logger.LogDebug("Setting {Key} changed from {PreviousValue} to {NewValue}", 
                    key, previousValue ?? "null", value);

                // Fire event using reactive subject
                var eventArgs = new SettingChangedEventArgs(key, previousValue, value);
                _settingChangedSubject.OnNext(eventArgs);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Clears all application state.
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                _logger.LogInformation("Clearing all application state");

                // Clear user
                SetCurrentUser(null);

                // Reset connection status
                SetConnectionStatus(ConnectionStatus.Disconnected);

                // Clear all settings
                var settingKeys = new List<string>(_settings.Keys);
                _settings.Clear();

                // Fire events for each cleared setting
                foreach (var key in settingKeys)
                {
                    var eventArgs = new SettingChangedEventArgs(key, "cleared", null!);
                    _settingChangedSubject.OnNext(eventArgs);
                }

                _logger.LogInformation("Application state cleared successfully");
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets a snapshot of the current application state for debugging.
        /// </summary>
        public ApplicationStateSnapshot GetStateSnapshot()
        {
            _lock.EnterReadLock();
            try
            {
                return new ApplicationStateSnapshot
                {
                    CurrentUser = _currentUser,
                    ConnectionStatus = _connectionStatus,
                    Settings = new Dictionary<string, object>(_settings),
                    Timestamp = DateTime.UtcNow
                };
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Loads application state from a previous session (e.g., from persisted storage).
        /// </summary>
        public async Task<Result> LoadStateAsync(ApplicationStateSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            try
            {
                if (snapshot == null)
                {
                    return Result.Failure("State snapshot cannot be null");
                }

                _logger.LogInformation("Loading application state from snapshot dated {Timestamp}", snapshot.Timestamp);

                _lock.EnterWriteLock();
                try
                {
                    // Load user (but don't automatically authenticate)
                    if (snapshot.CurrentUser != null)
                    {
                        _logger.LogInformation("Loading user from snapshot: {UserName}", snapshot.CurrentUser.UserName);
                        // Note: In production, you would validate the user session is still valid
                        SetCurrentUser(snapshot.CurrentUser);
                    }

                    // Load connection status (but force a fresh connection test)
                    SetConnectionStatus(ConnectionStatus.Disconnected);

                    // Load settings
                    _settings.Clear();
                    foreach (var setting in snapshot.Settings)
                    {
                        SetSetting(setting.Key, setting.Value);
                    }

                    _logger.LogInformation("Application state loaded successfully from snapshot");
                    return Result.Success();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load application state from snapshot dated {Timestamp}", 
                    snapshot?.Timestamp.ToString() ?? "null");
                return Result.Failure($"Failed to load application state: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves the current application state for persistence.
        /// </summary>
        public async Task<Result<ApplicationStateSnapshot>> SaveStateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Saving current application state");

                var snapshot = GetStateSnapshot();
                
                // In a production application, you would persist this to storage
                // For now, we just return the snapshot
                
                _logger.LogInformation("Application state saved successfully");
                return Result<ApplicationStateSnapshot>.Success(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save application state");
                return Result<ApplicationStateSnapshot>.Failure($"Failed to save application state: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of the service and its resources.
        /// </summary>
        public void Dispose()
        {
            _lock?.Dispose();
            _userChangedSubject?.Dispose();
            _connectionStatusChangedSubject?.Dispose();
            _settingChangedSubject?.Dispose();
        }
    }

    /// <summary>
    /// Represents a snapshot of the application state at a point in time.
    /// </summary>
    public class ApplicationStateSnapshot
    {
        public User? CurrentUser { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Extended application state service with additional MTM-specific functionality.
    /// </summary>
    public class MTMApplicationStateService : ApplicationStateService
    {
        public MTMApplicationStateService(ILogger<MTMApplicationStateService> logger) : base(logger)
        {
        }

        /// <summary>
        /// Gets or sets the current Part ID being worked on.
        /// </summary>
        public string? CurrentPartId
        {
            get => GetSetting<string>("CurrentPartId");
            set => SetSetting("CurrentPartId", value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the current operation being performed.
        /// </summary>
        public string? CurrentOperation
        {
            get => GetSetting<string>("CurrentOperation");
            set => SetSetting("CurrentOperation", value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        public string? CurrentLocation
        {
            get => GetSetting<string>("CurrentLocation");
            set => SetSetting("CurrentLocation", value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the selected tab index.
        /// </summary>
        public int SelectedTabIndex
        {
            get => GetSetting<int>("SelectedTabIndex");
            set => SetSetting("SelectedTabIndex", value);
        }

        /// <summary>
        /// Gets or sets user preferences for the quick buttons.
        /// </summary>
        public List<QuickButtonItem> QuickButtons
        {
            get => GetSetting<List<QuickButtonItem>>("QuickButtons") ?? new List<QuickButtonItem>();
            set => SetSetting("QuickButtons", value);
        }

        /// <summary>
        /// Gets or sets the window size and position preferences.
        /// </summary>
        public WindowPreferences WindowPreferences
        {
            get => GetSetting<WindowPreferences>("WindowPreferences") ?? new WindowPreferences();
            set => SetSetting("WindowPreferences", value);
        }
    }

    /// <summary>
    /// User preferences for window size and position.
    /// </summary>
    public class WindowPreferences
    {
        public double Width { get; set; } = 1200;
        public double Height { get; set; } = 700;
        public double Left { get; set; } = 100;
        public double Top { get; set; } = 100;
        public bool IsMaximized { get; set; } = false;
    }
}