using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MTM.Models;

namespace MTM.Services.Interfaces
{
    /// <summary>
    /// Interface for user management service providing authentication, user data access, and role management.
    /// CRITICAL: All database operations must use stored procedures only - NO direct SQL.
    /// Integrates with MTM user authentication system and supports user session management.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates a user with username and password/pin.
        /// Returns user information if authentication is successful.
        /// </summary>
        /// <param name="username">User name or identifier</param>
        /// <param name="pin">User PIN for authentication</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing authenticated user information</returns>
        Task<Result<User>> AuthenticateUserAsync(string username, string pin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user information by username.
        /// </summary>
        /// <param name="username">User name to lookup</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing user information if found</returns>
        Task<Result<User>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user information by user ID.
        /// </summary>
        /// <param name="userId">User ID to lookup</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing user information if found</returns>
        Task<Result<User>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active users in the system.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing list of all active users</returns>
        Task<Result<List<User>>> GetAllUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">User information to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success/failure and returning created user ID</returns>
        Task<Result<int>> CreateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates existing user information.
        /// </summary>
        /// <param name="user">Updated user information</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success/failure of update operation</returns>
        Task<Result> UpdateUserAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates user preferences (theme, font size, etc.).
        /// </summary>
        /// <param name="userId">User ID to update</param>
        /// <param name="preferences">User preferences to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success/failure of preferences update</returns>
        Task<Result> UpdateUserPreferencesAsync(int userId, UserPreferences preferences, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user preferences for theme and display settings.
        /// </summary>
        /// <param name="userId">User ID to get preferences for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing user preferences</returns>
        Task<Result<UserPreferences>> GetUserPreferencesAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates user permissions for specific operations.
        /// </summary>
        /// <param name="userId">User ID to check permissions for</param>
        /// <param name="operation">Operation to check permission for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating if user has permission for the operation</returns>
        Task<Result<bool>> ValidateUserPermissionAsync(int userId, string operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Records user activity for audit tracking.
        /// </summary>
        /// <param name="userId">User ID performing the activity</param>
        /// <param name="activity">Description of the activity</param>
        /// <param name="details">Additional activity details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success/failure of activity logging</returns>
        Task<Result> LogUserActivityAsync(int userId, string activity, string? details = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets user activity history for audit purposes.
        /// </summary>
        /// <param name="userId">User ID to get activity for (null for all users)</param>
        /// <param name="startDate">Start date for activity range</param>
        /// <param name="endDate">End date for activity range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing list of user activities</returns>
        Task<Result<List<UserActivity>>> GetUserActivityHistoryAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes user PIN for authentication.
        /// </summary>
        /// <param name="userId">User ID to change PIN for</param>
        /// <param name="currentPin">Current PIN for verification</param>
        /// <param name="newPin">New PIN to set</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success/failure of PIN change</returns>
        Task<Result> ChangeUserPinAsync(int userId, string currentPin, string newPin, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates if a username is available for registration.
        /// </summary>
        /// <param name="username">Username to check availability for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating if username is available</returns>
        Task<Result<bool>> IsUsernameAvailableAsync(string username, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// User preferences model for theme and display settings.
    /// </summary>
    public class UserPreferences
    {
        public string ThemeName { get; set; } = "Default";
        public int FontSize { get; set; } = 12;
        public string LastShownVersion { get; set; } = string.Empty;
        public bool HideChangeLog { get; set; } = false;
        public string? VisualUserName { get; set; }
        public string? VisualPassword { get; set; }
        public string? WipServerAddress { get; set; }
        public string? WIPDatabase { get; set; }
        public string? WipServerPort { get; set; }
    }

    /// <summary>
    /// User activity model for audit tracking.
    /// </summary>
    public class UserActivity
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IPAddress { get; set; }
        public string? SessionId { get; set; }
    }
}