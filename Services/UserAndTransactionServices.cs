using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM.Models;
using MTM.Core.Services;

namespace MTM.Services
{
    /// <summary>
    /// User service implementation for user management operations.
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
        /// Gets the current authenticated user.
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
                const string query = @"
                    SELECT UserId, UserName, DisplayName, Email, IsActive, 
                           CreatedDate, LastLoginDate, Role
                    FROM Users 
                    WHERE UserId = @UserId AND IsActive = 1";

                var result = await _databaseService.ExecuteQueryAsync<User>(
                    query, 
                    new { UserId = currentUser.UserId }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve current user {UserId}: {Error}", 
                        currentUser.UserId, result.ErrorMessage);
                    return Result<User?>.Failure(result.ErrorMessage ?? "Failed to retrieve current user");
                }

                var refreshedUser = result.Value?.FirstOrDefault();
                _logger.LogDebug("Retrieved current user: {UserName}", refreshedUser?.UserName ?? "null");
                
                return Result<User?>.Success(refreshedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user");
                return Result<User?>.Failure($"Failed to get current user: {ex.Message}");
            }
        }

        /// <summary>
        /// Authenticates a user with username and password.
        /// </summary>
        public async Task<Result<User>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return Result<User>.Failure("Username cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return Result<User>.Failure("Password cannot be empty");
                }

                _logger.LogInformation("Authenticating user: {Username}", username);

                // In a production system, you would hash the password and compare
                // For this implementation, we'll do a simple comparison
                const string query = @"
                    SELECT UserId, UserName, DisplayName, Email, IsActive, 
                           CreatedDate, LastLoginDate, Role
                    FROM Users 
                    WHERE UserName = @Username AND Password = @Password AND IsActive = 1";

                var result = await _databaseService.ExecuteQueryAsync<User>(
                    query, 
                    new { Username = username, Password = password }, // Note: In production, use hashed passwords
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
                    return Result<User>.Failure("Invalid username or password");
                }

                // Update last login timestamp
                await UpdateLastLoginAsync(user.UserId.ToString(), cancellationToken);

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
        /// Gets all active users.
        /// </summary>
        public async Task<Result<List<User>>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving all active users");

                const string query = @"
                    SELECT UserId, UserName, DisplayName, Email, IsActive, 
                           CreatedDate, LastLoginDate, Role
                    FROM Users 
                    WHERE IsActive = 1
                    ORDER BY UserName";

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
        /// Updates user's last login timestamp.
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

                const string command = @"
                    UPDATE Users 
                    SET LastLoginDate = @LastLoginDate 
                    WHERE UserId = @UserId";

                var result = await _databaseService.ExecuteNonQueryAsync(
                    command, 
                    new { UserId = userId, LastLoginDate = DateTime.UtcNow }, 
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
    }

    /// <summary>
    /// Transaction service implementation for transaction management operations.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(IDatabaseService databaseService, ILogger<TransactionService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// Gets transaction history for a specific part.
        /// </summary>
        public async Task<Result<List<InventoryTransaction>>> GetTransactionHistoryAsync(string partId, int limit = 50, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return Result<List<InventoryTransaction>>.Failure("Part ID cannot be empty");
                }

                if (limit <= 0 || limit > 1000)
                {
                    return Result<List<InventoryTransaction>>.Failure("Limit must be between 1 and 1000");
                }

                _logger.LogInformation("Retrieving transaction history for Part ID: {PartId}, Limit: {Limit}", partId, limit);

                const string query = @"
                    SELECT TransactionId, PartId, Operation, Location, Quantity, 
                           TransactionType, TransactionDateTime, UserName, BatchNumber, Comments
                    FROM InventoryTransactions 
                    WHERE PartId = @PartId
                    ORDER BY TransactionDateTime DESC
                    LIMIT @Limit";

                var result = await _databaseService.ExecuteQueryAsync<InventoryTransaction>(
                    query, 
                    new { PartId = partId, Limit = limit }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve transaction history for Part ID {PartId}: {Error}", 
                        partId, result.ErrorMessage);
                    return Result<List<InventoryTransaction>>.Failure(result.ErrorMessage ?? "Failed to retrieve transaction history");
                }

                _logger.LogInformation("Retrieved {Count} transactions for Part ID: {PartId}", 
                    result.Value?.Count ?? 0, partId);
                
                return Result<List<InventoryTransaction>>.Success(result.Value ?? new List<InventoryTransaction>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve transaction history for Part ID {PartId}", partId);
                return Result<List<InventoryTransaction>>.Failure($"Failed to retrieve transaction history: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a new inventory transaction.
        /// </summary>
        public async Task<Result> LogTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
        {
            try
            {
                if (transaction == null)
                {
                    return Result.Failure("Transaction cannot be null");
                }

                if (string.IsNullOrWhiteSpace(transaction.PartId))
                {
                    return Result.Failure("Part ID is required");
                }

                _logger.LogInformation("Logging transaction for Part ID: {PartId}, Type: {TransactionType}", 
                    transaction.PartId, transaction.TransactionType);

                // Set timestamp if not already set
                if (transaction.TransactionDateTime == default)
                {
                    transaction.TransactionDateTime = DateTime.UtcNow;
                }

                const string command = @"
                    INSERT INTO InventoryTransactions 
                    (PartId, Operation, Location, Quantity, TransactionType, 
                     TransactionDateTime, UserName, BatchNumber, Comments)
                    VALUES 
                    (@PartId, @Operation, @Location, @Quantity, @TransactionType, 
                     @TransactionDateTime, @UserName, @BatchNumber, @Comments)";

                var result = await _databaseService.ExecuteNonQueryAsync(command, transaction, cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to log transaction for Part ID {PartId}: {Error}", 
                        transaction.PartId, result.ErrorMessage);
                    return Result.Failure(result.ErrorMessage ?? "Failed to log transaction");
                }

                _logger.LogInformation("Successfully logged transaction for Part ID: {PartId}", transaction.PartId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log transaction for Part ID {PartId}", 
                    transaction?.PartId ?? "null");
                return Result.Failure($"Failed to log transaction: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the last N transactions for a user.
        /// </summary>
        public async Task<Result<List<InventoryTransaction>>> GetUserTransactionsAsync(string userId, int limit = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Result<List<InventoryTransaction>>.Failure("User ID cannot be empty");
                }

                if (limit <= 0 || limit > 100)
                {
                    return Result<List<InventoryTransaction>>.Failure("Limit must be between 1 and 100");
                }

                _logger.LogInformation("Retrieving user transactions for User: {UserId}, Limit: {Limit}", userId, limit);

                const string query = @"
                    SELECT TransactionId, PartId, Operation, Location, Quantity, 
                           TransactionType, TransactionDateTime, UserName, BatchNumber, Comments
                    FROM InventoryTransactions 
                    WHERE UserName = @UserName
                    ORDER BY TransactionDateTime DESC
                    LIMIT @Limit";

                var result = await _databaseService.ExecuteQueryAsync<InventoryTransaction>(
                    query, 
                    new { UserName = userId, Limit = limit }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve user transactions for User {UserId}: {Error}", 
                        userId, result.ErrorMessage);
                    return Result<List<InventoryTransaction>>.Failure(result.ErrorMessage ?? "Failed to retrieve user transactions");
                }

                _logger.LogInformation("Retrieved {Count} transactions for User: {UserId}", 
                    result.Value?.Count ?? 0, userId);
                
                return Result<List<InventoryTransaction>>.Success(result.Value ?? new List<InventoryTransaction>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user transactions for User {UserId}", userId);
                return Result<List<InventoryTransaction>>.Failure($"Failed to retrieve user transactions: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all transactions within a date range.
        /// </summary>
        public async Task<Result<List<InventoryTransaction>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            try
            {
                if (startDate >= endDate)
                {
                    return Result<List<InventoryTransaction>>.Failure("Start date must be before end date");
                }

                var dateRange = endDate - startDate;
                if (dateRange.TotalDays > 365)
                {
                    return Result<List<InventoryTransaction>>.Failure("Date range cannot exceed 365 days");
                }

                _logger.LogInformation("Retrieving transactions for date range: {StartDate} to {EndDate}", 
                    startDate, endDate);

                const string query = @"
                    SELECT TransactionId, PartId, Operation, Location, Quantity, 
                           TransactionType, TransactionDateTime, UserName, BatchNumber, Comments
                    FROM InventoryTransactions 
                    WHERE TransactionDateTime >= @StartDate AND TransactionDateTime <= @EndDate
                    ORDER BY TransactionDateTime DESC
                    LIMIT 10000"; // Safety limit

                var result = await _databaseService.ExecuteQueryAsync<InventoryTransaction>(
                    query, 
                    new { StartDate = startDate, EndDate = endDate }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve transactions by date range {StartDate} to {EndDate}: {Error}", 
                        startDate, endDate, result.ErrorMessage);
                    return Result<List<InventoryTransaction>>.Failure(result.ErrorMessage ?? "Failed to retrieve transactions by date range");
                }

                _logger.LogInformation("Retrieved {Count} transactions for date range: {StartDate} to {EndDate}", 
                    result.Value?.Count ?? 0, startDate, endDate);
                
                return Result<List<InventoryTransaction>>.Success(result.Value ?? new List<InventoryTransaction>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve transactions by date range {StartDate} to {EndDate}", 
                    startDate, endDate);
                return Result<List<InventoryTransaction>>.Failure($"Failed to retrieve transactions by date range: {ex.Message}");
            }
        }
    }
}