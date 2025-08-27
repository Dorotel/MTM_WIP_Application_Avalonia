using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MTM_Shared_Logic.Models;

namespace MTM_Shared_Logic.Services
{
    /// <summary>
    /// Interface for validation services in the MTM WIP Application.
    /// Provides validation capabilities for business objects and data integrity.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates any object implementing validation rules.
        /// </summary>
        /// <typeparam name="T">Type of object to validate</typeparam>
        /// <param name="item">Object to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing validation result</returns>
        Task<Result<ValidationResult>> ValidateAsync<T>(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates an inventory item according to business rules.
        /// </summary>
        /// <param name="item">Inventory item to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing validation result</returns>
        Task<Result<ValidationResult>> ValidateInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates that a Part ID exists and is active in the system.
        /// </summary>
        /// <param name="partId">Part ID to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating if the Part ID is valid</returns>
        Task<Result<bool>> ValidatePartIdAsync(string partId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates that a location exists and is active in the system.
        /// </summary>
        /// <param name="location">Location to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating if the location is valid</returns>
        Task<Result<bool>> ValidateLocationAsync(string location, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates that an operation exists in the system.
        /// </summary>
        /// <param name="operation">Operation to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating if the operation is valid</returns>
        Task<Result<bool>> ValidateOperationAsync(string operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates user permissions for a specific action.
        /// </summary>
        /// <param name="userId">User ID to check</param>
        /// <param name="action">Action to validate permission for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating if the user has permission</returns>
        Task<Result<bool>> ValidateUserPermissionAsync(string userId, string action, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates quantity constraints and business rules.
        /// </summary>
        /// <param name="quantity">Quantity to validate</param>
        /// <param name="partId">Part ID for context-specific validation</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing validation result</returns>
        Task<Result<ValidationResult>> ValidateQuantityAsync(int quantity, string? partId = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for caching services in the MTM WIP Application.
    /// Provides caching capabilities for performance optimization.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        /// <typeparam name="T">Type of cached object</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Cached value or default if not found</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Sets a value in cache with expiration.
        /// </summary>
        /// <typeparam name="T">Type of object to cache</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Value to cache</param>
        /// <param name="expiration">Expiration time</param>
        /// <returns>Task representing the operation</returns>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Removes a value from cache.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>Task representing the operation</returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// Clears all cached values.
        /// </summary>
        /// <returns>Task representing the operation</returns>
        Task ClearAsync();

        /// <summary>
        /// Checks if a key exists in cache.
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>True if key exists, false otherwise</returns>
        Task<bool> ExistsAsync(string key);
    }
}
