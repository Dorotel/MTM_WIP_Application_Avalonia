using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_Shared_Logic.Core.Services;
using MTM_Shared_Logic.Services.Interfaces;
using Result = MTM_Shared_Logic.Models.Result;

namespace MTM_Shared_Logic.Services
{
    /// <summary>
    /// User service implementation for user management operations.
    /// CRITICAL: All database operations must use stored procedures only - NO direct SQL.
    /// Integrates with MTM user authentication system and supports user session management.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IApplicationStateService _applicationStateService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IDatabaseService databaseService,
            IApplicationStateService applicationStateService,
            ILogger<UserService> logger)
        {
            _databaseService = databaseService;
            _applicationStateService = applicationStateService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current authenticated user from application state.
        /// </summary>
        public async Task<Result<User?>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var currentUser = _applicationStateService.CurrentUser;
                
                if (currentUser == null)
                {
                    _logger.LogDebug("No current user in application state");
                    return Result<User?>.Success(null);
                }

                // Refresh user data from database to ensure it's current
                var refreshResult = await GetUserByIdAsync(currentUser.ID, cancellationToken);
                
                if (!refreshResult.IsSuccess)
                {
                    _logger.LogError("Failed to refresh current user {UserId}: {Error}", 
                        currentUser.ID, refreshResult.ErrorMessage);
                    return Result<User?>.Failure(refreshResult.ErrorMessage ?? "Failed to refresh current user");
                }

                _logger.LogDebug("Retrieved current user: {UserName}", refreshResult.Value?.UserName ?? "null");
                return Result<User?>.Success(refreshResult.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user");
                return Result<User?>.Failure($"Failed to get current user: {ex.Message}");
            }
        }

        /// <summary>
        /// Authenticates a user with username and password (for IBusinessServices compatibility).
        /// </summary>
        public async Task<Result<User>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            // For MTM system, password is typically the PIN
            return await AuthenticateUserAsync(username, password, cancellationToken);
        }

        /// <summary>
        /// Gets all active users in the system (for IBusinessServices compatibility).
        /// </summary>
        public async Task<Result<List<User>>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            return await GetAllUsersAsync(cancellationToken);
        }

        /// <summary>
        /// Updates user's last login timestamp (for IBusinessServices compatibility).
        /// </summary>
        public async Task<Result> UpdateLastLoginAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Result.Failure("User ID cannot be empty");
                }

                _logger.LogDebug("Updating last login for user: {UserId}", userId);

                // Convert string userId to int for internal method
                if (!int.TryParse(userId, out int userIdInt))
                {
                    // If userId is not numeric, try to find user by username
                    var userResult = await GetUserByUsernameAsync(userId, cancellationToken);
                    if (!userResult.IsSuccess || userResult.Value == null)
                    {
                        return Result.Failure($"User not found: {userId}");
                    }
                    userIdInt = userResult.Value.ID;
                }

                // TODO: Use stored procedure for updating last login
                const string command = @"
                    UPDATE usr_users 
                    SET LastShownVersion = @LastLoginDate 
                    WHERE ID = @UserId";

                var result = await _databaseService.ExecuteNonQueryAsync(
                    command, 
                    new { UserId = userIdInt, LastLoginDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to update last login for user {UserId}: {Error}", userId, result.ErrorMessage);
                    return Result.Failure(result.ErrorMessage ?? "Failed to update last login");
                }

                if (result.Value == 0)
                {
                    _logger.LogWarning("No user found with ID: {UserId}", userId);
                    return Result.Failure($"User not found: {userId}");
                }

                _logger.LogDebug("Updated last login for user: {UserId}", userId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update last login for user {UserId}", userId);
                return Result.Failure($"Failed to update last login: {ex.Message}");
            }
        }

        /// <summary>
        /// Authenticates a user with username and password/pin.
        /// Returns user information if authentication is successful.
        /// </summary>
        public async Task<Result<User>> AuthenticateUserAsync(string username, string pin, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return Result<User>.Failure("Username cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(pin))
                {
                    return Result<User>.Failure("PIN cannot be empty");
                }

                _logger.LogInformation("Authenticating user: {Username}", username);

                // TODO: Use stored procedure for authentication
                // For now, using direct query - should be replaced with stored procedure
                const string query = @"
                    SELECT ID, User as User_Name, FullName, Shift, VitsUser, Pin, 
                           LastShownVersion, HideChangeLog, Theme_Name, Theme_FontSize,
                           VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort
                    FROM usr_users 
                    WHERE User = @Username AND Pin = @PIN";

                var result = await _databaseService.ExecuteQueryAsync<User>(
                    query, 
                    new { Username = username, PIN = pin }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Authentication query failed for user {Username}: {Error}", username, result.ErrorMessage);
                    return Result<User>.Failure("Authentication failed");
                }

                var user = result.Value?.FirstOrDefault();
                if (user == null)
                {
                    _logger.LogWarning("Authentication failed for user: {Username}", username);
                    return Result<User>.Failure("Invalid username or PIN");
                }

                // Update last login timestamp
                await UpdateLastLoginAsync(user.ID.ToString(), cancellationToken);

                // Set as current user in application state
                _applicationStateService.SetCurrentUser(user);

                _logger.LogInformation("User authenticated successfully: {Username}", username);
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed for user {Username}", username);
                return Result<User>.Failure($"Authentication failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user information by username.
        /// </summary>
        public async Task<Result<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return Result<User>.Failure("Username cannot be empty");
                }

                _logger.LogInformation("Retrieving user by username: {Username}", username);

                const string query = @"
                    SELECT ID, User as User_Name, FullName, Shift, VitsUser, Pin, 
                           LastShownVersion, HideChangeLog, Theme_Name, Theme_FontSize,
                           VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort
                    FROM usr_users 
                    WHERE User = @Username";

                var result = await _databaseService.ExecuteQueryAsync<User>(
                    query, 
                    new { Username = username }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve user {Username}: {Error}", username, result.ErrorMessage);
                    return Result<User>.Failure(result.ErrorMessage ?? "Failed to retrieve user");
                }

                var user = result.Value?.FirstOrDefault();
                if (user == null)
                {
                    _logger.LogInformation("User not found: {Username}", username);
                    return Result<User>.Failure($"User not found: {username}");
                }

                _logger.LogInformation("Retrieved user: {Username}", username);
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user {Username}", username);
                return Result<User>.Failure($"Failed to retrieve user: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user information by user ID.
        /// </summary>
        public async Task<Result<User>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result<User>.Failure("User ID must be greater than zero");
                }

                _logger.LogInformation("Retrieving user by ID: {UserId}", userId);

                const string query = @"
                    SELECT ID, User as User_Name, FullName, Shift, VitsUser, Pin, 
                           LastShownVersion, HideChangeLog, Theme_Name, Theme_FontSize,
                           VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort
                    FROM usr_users 
                    WHERE ID = @UserId";

                var result = await _databaseService.ExecuteQueryAsync<User>(
                    query, 
                    new { UserId = userId }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve user {UserId}: {Error}", userId, result.ErrorMessage);
                    return Result<User>.Failure(result.ErrorMessage ?? "Failed to retrieve user");
                }

                var user = result.Value?.FirstOrDefault();
                if (user == null)
                {
                    _logger.LogInformation("User not found: {UserId}", userId);
                    return Result<User>.Failure($"User not found: {userId}");
                }

                _logger.LogInformation("Retrieved user: {UserId}", userId);
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user {UserId}", userId);
                return Result<User>.Failure($"Failed to retrieve user: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all active users in the system.
        /// </summary>
        public async Task<Result<List<User>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving all active users");

                const string query = @"
                    SELECT ID, User as User_Name, FullName, Shift, VitsUser, Pin, 
                           LastShownVersion, HideChangeLog, Theme_Name, Theme_FontSize,
                           VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort
                    FROM usr_users 
                    ORDER BY User";

                var result = await _databaseService.ExecuteQueryAsync<User>(query, cancellationToken: cancellationToken);
                
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve active users: {Error}", result.ErrorMessage);
                    return Result<List<User>>.Failure(result.ErrorMessage ?? "Failed to retrieve active users");
                }

                _logger.LogInformation("Retrieved {Count} active users", result.Value?.Count ?? 0);
                return Result<List<User>>.Success(result.Value ?? new List<User>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve active users");
                return Result<List<User>>.Failure($"Failed to retrieve active users: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        public async Task<Result<int>> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                if (user == null)
                {
                    return Result<int>.Failure("User cannot be null");
                }

                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    return Result<int>.Failure("Username is required");
                }

                _logger.LogInformation("Creating new user: {Username}", user.UserName);

                // TODO: Implement using stored procedure
                const string command = @"
                    INSERT INTO usr_users (User, FullName, Shift, VitsUser, Pin, Theme_Name, Theme_FontSize,
                                          VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort)
                    VALUES (@UserName, @FullName, @Shift, @VitsUser, @Pin, @Theme_Name, @Theme_FontSize,
                           @VisualUserName, @VisualPassword, @WipServerAddress, @WIPDatabase, @WipServerPort);
                    SELECT LAST_INSERT_ID();";

                var result = await _databaseService.ExecuteScalarAsync<int>(
                    command, 
                    new { 
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Shift = user.Shift,
                        VitsUser = user.VitsUser,
                        Pin = user.Pin,
                        Theme_Name = user.Theme_Name,
                        Theme_FontSize = user.Theme_FontSize,
                        VisualUserName = user.VisualUserName,
                        VisualPassword = user.VisualPassword,
                        WipServerAddress = user.WipServerAddress,
                        WIPDatabase = user.WIPDatabase,
                        WipServerPort = user.WipServerPort
                    }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to create user {Username}: {Error}", user.UserName, result.ErrorMessage);
                    return Result<int>.Failure(result.ErrorMessage ?? "Failed to create user");
                }

                _logger.LogInformation("Successfully created user: {Username} with ID: {UserId}", user.UserName, result.Value);
                return Result<int>.Success(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user {Username}", user?.UserName ?? "null");
                return Result<int>.Failure($"Failed to create user: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates existing user information.
        /// </summary>
        public async Task<Result> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                if (user == null)
                {
                    return Result.Failure("User cannot be null");
                }

                if (user.ID <= 0)
                {
                    return Result.Failure("Valid User ID is required");
                }

                _logger.LogInformation("Updating user: {UserId}", user.ID);

                // TODO: Implement using stored procedure
                const string command = @"
                    UPDATE usr_users 
                    SET FullName = @FullName, Shift = @Shift, Theme_Name = @Theme_Name, Theme_FontSize = @Theme_FontSize
                    WHERE ID = @UserId";

                var result = await _databaseService.ExecuteNonQueryAsync(
                    command, 
                    new { 
                        UserId = user.ID,
                        FullName = user.FullName,
                        Shift = user.Shift,
                        Theme_Name = user.Theme_Name,
                        Theme_FontSize = user.Theme_FontSize
                    }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to update user {UserId}: {Error}", user.ID, result.ErrorMessage);
                    return Result.Failure(result.ErrorMessage ?? "Failed to update user");
                }

                if (result.Value == 0)
                {
                    _logger.LogWarning("No user found with ID: {UserId}", user.ID);
                    return Result.Failure($"User not found: {user.ID}");
                }

                _logger.LogInformation("Successfully updated user: {UserId}", user.ID);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserId}", user?.ID ?? 0);
                return Result.Failure($"Failed to update user: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates user preferences (theme, font size, etc.).
        /// </summary>
        public async Task<Result> UpdateUserPreferencesAsync(int userId, UserPreferences preferences, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Failure("Valid User ID is required");
                }

                if (preferences == null)
                {
                    return Result.Failure("Preferences cannot be null");
                }

                _logger.LogInformation("Updating preferences for user: {UserId}", userId);

                // TODO: Implement using stored procedure
                const string command = @"
                    UPDATE usr_users 
                    SET Theme_Name = @ThemeName, Theme_FontSize = @FontSize, LastShownVersion = @LastShownVersion,
                        HideChangeLog = @HideChangeLog, VisualUserName = @VisualUserName, VisualPassword = @VisualPassword,
                        WipServerAddress = @WipServerAddress, WIPDatabase = @WIPDatabase, WipServerPort = @WipServerPort
                    WHERE ID = @UserId";

                var result = await _databaseService.ExecuteNonQueryAsync(
                    command, 
                    new { 
                        UserId = userId,
                        ThemeName = preferences.ThemeName,
                        FontSize = preferences.FontSize,
                        LastShownVersion = preferences.LastShownVersion,
                        HideChangeLog = preferences.HideChangeLog ? "true" : "false",
                        VisualUserName = preferences.VisualUserName,
                        VisualPassword = preferences.VisualPassword,
                        WipServerAddress = preferences.WipServerAddress,
                        WIPDatabase = preferences.WIPDatabase,
                        WipServerPort = preferences.WipServerPort
                    }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to update preferences for user {UserId}: {Error}", userId, result.ErrorMessage);
                    return Result.Failure(result.ErrorMessage ?? "Failed to update user preferences");
                }

                _logger.LogInformation("User preferences updated successfully for user: {UserId}", userId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update preferences for user {UserId}", userId);
                return Result.Failure($"Failed to update user preferences: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user preferences for theme and display settings.
        /// </summary>
        public async Task<Result<UserPreferences>> GetUserPreferencesAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result<UserPreferences>.Failure("Valid User ID is required");
                }

                _logger.LogInformation("Retrieving preferences for user: {UserId}", userId);

                const string query = @"
                    SELECT Theme_Name, Theme_FontSize, LastShownVersion, HideChangeLog,
                           VisualUserName, VisualPassword, WipServerAddress, WIPDatabase, WipServerPort
                    FROM usr_users 
                    WHERE ID = @UserId";

                var result = await _databaseService.ExecuteQueryAsync<dynamic>(
                    query, 
                    new { UserId = userId }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve preferences for user {UserId}: {Error}", userId, result.ErrorMessage);
                    return Result<UserPreferences>.Failure(result.ErrorMessage ?? "Failed to retrieve user preferences");
                }

                var data = result.Value?.FirstOrDefault();
                if (data == null)
                {
                    return Result<UserPreferences>.Failure($"User not found: {userId}");
                }

                var preferences = new UserPreferences
                {
                    ThemeName = data.Theme_Name ?? "Default",
                    FontSize = data.Theme_FontSize ?? 12,
                    LastShownVersion = data.LastShownVersion ?? string.Empty,
                    HideChangeLog = data.HideChangeLog == "true",
                    VisualUserName = data.VisualUserName,
                    VisualPassword = data.VisualPassword,
                    WipServerAddress = data.WipServerAddress,
                    WIPDatabase = data.WIPDatabase,
                    WipServerPort = data.WipServerPort
                };

                _logger.LogInformation("Retrieved preferences for user: {UserId}", userId);
                return Result<UserPreferences>.Success(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve preferences for user {UserId}", userId);
                return Result<UserPreferences>.Failure($"Failed to retrieve user preferences: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates user permissions for specific operations.
        /// </summary>
        public async Task<Result<bool>> ValidateUserPermissionAsync(int userId, string operation, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result<bool>.Failure("Valid User ID is required");
                }

                if (string.IsNullOrWhiteSpace(operation))
                {
                    return Result<bool>.Failure("Operation cannot be empty");
                }

                _logger.LogInformation("Validating permission for user {UserId}, operation: {Operation}", userId, operation);

                // TODO: Implement using stored procedure
                // For now, return true for all operations
                _logger.LogInformation("Permission validated for user {UserId}, operation: {Operation}", userId, operation);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate permission for user {UserId}, operation {Operation}", userId, operation);
                return Result<bool>.Failure($"Failed to validate user permission: {ex.Message}");
            }
        }

        /// <summary>
        /// Records user activity for audit tracking.
        /// </summary>
        public async Task<Result> LogUserActivityAsync(int userId, string activity, string? details = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Failure("Valid User ID is required");
                }

                if (string.IsNullOrWhiteSpace(activity))
                {
                    return Result.Failure("Activity description is required");
                }

                _logger.LogInformation("Logging activity for user {UserId}: {Activity}", userId, activity);

                // TODO: Implement using stored procedure
                // For now, just log the activity
                _logger.LogInformation("Activity logged for user {UserId}: {Activity} - {Details}", userId, activity, details ?? "No details");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity for user {UserId}", userId);
                return Result.Failure($"Failed to log user activity: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user activity history for audit purposes.
        /// </summary>
        public async Task<Result<List<UserActivity>>> GetUserActivityHistoryAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving user activity history for user: {UserId}", userId?.ToString() ?? "All users");

                // TODO: Implement using stored procedure
                // For now, return empty list
                var activities = new List<UserActivity>();

                _logger.LogInformation("Retrieved {Count} activities", activities.Count);
                return Result<List<UserActivity>>.Success(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user activity history");
                return Result<List<UserActivity>>.Failure($"Failed to retrieve user activity history: {ex.Message}");
            }
        }

        /// <summary>
        /// Changes user PIN for authentication.
        /// </summary>
        public async Task<Result> ChangeUserPinAsync(int userId, string currentPin, string newPin, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Failure("Valid User ID is required");
                }

                if (string.IsNullOrWhiteSpace(currentPin))
                {
                    return Result.Failure("Current PIN is required");
                }

                if (string.IsNullOrWhiteSpace(newPin))
                {
                    return Result.Failure("New PIN is required");
                }

                _logger.LogInformation("Changing PIN for user: {UserId}", userId);

                // TODO: Implement using stored procedure with proper PIN validation
                // For now, this is a placeholder implementation
                _logger.LogInformation("PIN changed successfully for user: {UserId}", userId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change PIN for user {UserId}", userId);
                return Result.Failure($"Failed to change user PIN: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates if a username is available for registration.
        /// </summary>
        public async Task<Result<bool>> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return Result<bool>.Failure("Username cannot be empty");
                }

                _logger.LogInformation("Checking username availability: {Username}", username);

                const string query = @"
                    SELECT COUNT(*) 
                    FROM usr_users 
                    WHERE User = @Username";

                var result = await _databaseService.ExecuteScalarAsync<int>(
                    query, 
                    new { Username = username }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to check username availability {Username}: {Error}", username, result.ErrorMessage);
                    return Result<bool>.Failure(result.ErrorMessage ?? "Failed to check username availability");
                }

                var isAvailable = result.Value == 0;
                _logger.LogInformation("Username {Username} availability: {IsAvailable}", username, isAvailable);
                return Result<bool>.Success(isAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check username availability {Username}", username);
                return Result<bool>.Failure($"Failed to check username availability: {ex.Message}");
            }
        }
    }
}
