using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM.Models;
using MTM.Core.Services;

namespace MTM.Services
{
    /// <summary>
    /// Interface for inventory management operations following MTM business patterns.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Gets all inventory items asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing list of inventory items</returns>
        Task<Result<List<InventoryItem>>> GetInventoryAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific inventory item by Part ID.
        /// </summary>
        /// <param name="partId">The part ID to search for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the inventory item if found</returns>
        Task<Result<InventoryItem?>> GetInventoryItemAsync(string partId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new inventory item.
        /// </summary>
        /// <param name="item">The inventory item to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result> AddInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing inventory item.
        /// </summary>
        /// <param name="item">The inventory item to update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result> UpdateInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes an MTM operation (e.g., "90", "100", "110") for a specific part.
        /// </summary>
        /// <param name="partId">The part ID</param>
        /// <param name="operation">The operation number as string</param>
        /// <param name="quantity">The quantity to process</param>
        /// <param name="location">The location for the operation</param>
        /// <param name="userId">The user performing the operation</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result> ProcessOperationAsync(string partId, string operation, int quantity, string location, string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets inventory items by location.
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing filtered inventory items</returns>
        Task<Result<List<InventoryItem>>> GetInventoryByLocationAsync(string location, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets inventory items by operation.
        /// </summary>
        /// <param name="operation">The operation to filter by</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing filtered inventory items</returns>
        Task<Result<List<InventoryItem>>> GetInventoryByOperationAsync(string operation, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for user management operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets the current authenticated user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the current user</returns>
        Task<Result<User?>> GetCurrentUserAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Authenticates a user with username and password.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing authenticated user</returns>
        Task<Result<User>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active users.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing list of active users</returns>
        Task<Result<List<User>>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates user's last login timestamp.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result> UpdateLastLoginAsync(string userId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for transaction management operations.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Gets transaction history for a specific part.
        /// </summary>
        /// <param name="partId">The part ID</param>
        /// <param name="limit">Maximum number of transactions to return</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing transaction history</returns>
        Task<Result<List<InventoryTransaction>>> GetTransactionHistoryAsync(string partId, int limit = 50, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs a new inventory transaction.
        /// </summary>
        /// <param name="transaction">The transaction to log</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result> LogTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the last N transactions for a user.
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="limit">Maximum number of transactions to return</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing user's recent transactions</returns>
        Task<Result<List<InventoryTransaction>>> GetUserTransactionsAsync(string userId, int limit = 10, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all transactions within a date range.
        /// </summary>
        /// <param name="startDate">Start date for the range</param>
        /// <param name="endDate">End date for the range</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing transactions in the date range</returns>
        Task<Result<List<InventoryTransaction>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}