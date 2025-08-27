using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_Shared_Logic.Services.Interfaces;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_Shared_Logic.Services
{
    /// <summary>
    /// System services for validation, caching, and other utility operations.
    /// All system-related services are grouped in this file following MTM service organization patterns.
    /// </summary>

    /// <summary>
    /// Validation service for MTM business rules and data integrity.
    /// Provides comprehensive validation for inventory items, business rules, and data constraints.
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(
            ICacheService cacheService,
            ILogger<ValidationService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Validates any object implementing validation rules.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ValidationResult>> ValidateAsync<T>(T item, CancellationToken cancellationToken = default)
        {
            try
            {
                if (item == null)
                {
                    return MTM_Shared_Logic.Models.Result<ValidationResult>.Failure("Item cannot be null");
                }

                _logger.LogDebug("Validating item of type: {ItemType}", typeof(T).Name);

                // Route to specific validation based on type
                return item switch
                {
                    InventoryItem inventoryItem => await ValidateInventoryItemAsync(inventoryItem, cancellationToken),
                    InventoryTransaction transaction => await ValidateTransactionAsync(transaction, cancellationToken),
                    User user => await ValidateUserAsync(user, cancellationToken),
                    _ => MTM_Shared_Logic.Models.Result<ValidationResult>.Success(new ValidationResult { IsValid = true })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating item of type: {ItemType}", typeof(T).Name);
                return MTM_Shared_Logic.Models.Result<ValidationResult>.Failure($"Validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates an inventory item according to MTM business rules.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ValidationResult>> ValidateInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                if (item == null)
                {
                    return MTM_Shared_Logic.Models.Result<ValidationResult>.Failure("Inventory item cannot be null");
                }

                _logger.LogDebug("Validating inventory item: {PartId}", item.PartID);

                var validationResult = new ValidationResult
                {
                    IsValid = true,
                    Errors = new List<string>()
                };

                // Basic field validation
                if (string.IsNullOrWhiteSpace(item.PartID))
                {
                    validationResult.Errors.Add("Part ID is required");
                }

                if (string.IsNullOrWhiteSpace(item.Location))
                {
                    validationResult.Errors.Add("Location is required");
                }

                if (item.Quantity <= 0)
                {
                    validationResult.Errors.Add("Quantity must be greater than zero");
                }

                if (item.Quantity > Model_AppVariables.MTM.MaxTransactionQuantity)
                {
                    validationResult.Errors.Add($"Quantity cannot exceed {Model_AppVariables.MTM.MaxTransactionQuantity}");
                }

                if (string.IsNullOrWhiteSpace(item.User))
                {
                    validationResult.Errors.Add("User is required");
                }

                // Advanced business rule validation
                if (!string.IsNullOrWhiteSpace(item.PartID))
                {
                    var partValidResult = await ValidatePartIdAsync(item.PartID, cancellationToken);
                    if (!partValidResult.IsSuccess || !partValidResult.Value)
                    {
                        validationResult.Errors.Add("Part ID does not exist or is inactive");
                    }
                }

                if (!string.IsNullOrWhiteSpace(item.Location))
                {
                    var locationValidResult = await ValidateLocationAsync(item.Location, cancellationToken);
                    if (!locationValidResult.IsSuccess || !locationValidResult.Value)
                    {
                        validationResult.Errors.Add("Location does not exist or is inactive");
                    }
                }

                if (!string.IsNullOrWhiteSpace(item.Operation))
                {
                    var operationValidResult = await ValidateOperationAsync(item.Operation, cancellationToken);
                    if (!operationValidResult.IsSuccess || !operationValidResult.Value)
                    {
                        validationResult.Errors.Add("Operation does not exist");
                    }
                }

                validationResult.IsValid = validationResult.Errors.Count == 0;

                _logger.LogDebug("Inventory item validation completed. Valid: {IsValid}, Errors: {ErrorCount}",
                    validationResult.IsValid, validationResult.Errors.Count);

                return MTM_Shared_Logic.Models.Result<ValidationResult>.Success(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating inventory item: {PartId}", item?.PartID ?? "null");
                return MTM_Shared_Logic.Models.Result<ValidationResult>.Failure($"Validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates that a Part ID exists and is active in the system.
        /// Uses caching for performance optimization.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<bool>> ValidatePartIdAsync(string partId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return MTM_Shared_Logic.Models.Result<bool>.Success(false);
                }

                _logger.LogDebug("Validating Part ID: {PartId}", partId);

                // Check cache first
                var cacheKey = $"part_validation_{partId}";
                var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
                if (cachedResult.HasValue)
                {
                    _logger.LogDebug("Part ID validation found in cache: {PartId} = {IsValid}", partId, cachedResult.Value);
                    return MTM_Shared_Logic.Models.Result<bool>.Success(cachedResult.Value);
                }

                // Query database using stored procedure
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = partId
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    Model_AppVariables.ConnectionString,
                    "md_part_ids_Validate",
                    parameters);

                bool isValid = result.IsSuccess && result.Data.Rows.Count > 0;

                // Cache the result for 5 minutes
                await _cacheService.SetAsync(cacheKey, isValid, TimeSpan.FromMinutes(5));

                _logger.LogDebug("Part ID validation completed: {PartId} = {IsValid}", partId, isValid);
                return MTM_Shared_Logic.Models.Result<bool>.Success(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Part ID: {PartId}", partId);
                return MTM_Shared_Logic.Models.Result<bool>.Failure($"Part ID validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates that a location exists and is active in the system.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<bool>> ValidateLocationAsync(string location, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                {
                    return MTM_Shared_Logic.Models.Result<bool>.Success(false);
                }

                _logger.LogDebug("Validating location: {Location}", location);

                // Check cache first
                var cacheKey = $"location_validation_{location}";
                var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
                if (cachedResult.HasValue)
                {
                    return MTM_Shared_Logic.Models.Result<bool>.Success(cachedResult.Value);
                }

                // Query database using stored procedure
                var parameters = new Dictionary<string, object>
                {
                    ["p_Location"] = location
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    Model_AppVariables.ConnectionString,
                    "md_locations_Validate",
                    parameters);

                bool isValid = result.IsSuccess && result.Data.Rows.Count > 0;

                // Cache the result for 5 minutes
                await _cacheService.SetAsync(cacheKey, isValid, TimeSpan.FromMinutes(5));

                return MTM_Shared_Logic.Models.Result<bool>.Success(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating location: {Location}", location);
                return MTM_Shared_Logic.Models.Result<bool>.Failure($"Location validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates that an operation exists in the system.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<bool>> ValidateOperationAsync(string operation, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(operation))
                {
                    return MTM_Shared_Logic.Models.Result<bool>.Success(false);
                }

                _logger.LogDebug("Validating operation: {Operation}", operation);

                // Check cache first
                var cacheKey = $"operation_validation_{operation}";
                var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
                if (cachedResult.HasValue)
                {
                    return MTM_Shared_Logic.Models.Result<bool>.Success(cachedResult.Value);
                }

                // Query database using stored procedure
                var parameters = new Dictionary<string, object>
                {
                    ["p_Operation"] = operation
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    Model_AppVariables.ConnectionString,
                    "md_operation_numbers_Validate",
                    parameters);

                bool isValid = result.IsSuccess && result.Data.Rows.Count > 0;

                // Cache the result for 5 minutes
                await _cacheService.SetAsync(cacheKey, isValid, TimeSpan.FromMinutes(5));

                return MTM_Shared_Logic.Models.Result<bool>.Success(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating operation: {Operation}", operation);
                return MTM_Shared_Logic.Models.Result<bool>.Failure($"Operation validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates user permissions for a specific action.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<bool>> ValidateUserPermissionAsync(string userId, string action, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(action))
                {
                    return MTM_Shared_Logic.Models.Result<bool>.Success(false);
                }

                _logger.LogDebug("Validating user permission: {UserId} for action: {Action}", userId, action);

                // For now, return true as permissions are not fully implemented
                // TODO: Implement proper permission checking with roles
                return MTM_Shared_Logic.Models.Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user permission: {UserId}, Action: {Action}", userId, action);
                return MTM_Shared_Logic.Models.Result<bool>.Failure($"User permission validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates quantity constraints and business rules.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ValidationResult>> ValidateQuantityAsync(int quantity, string? partId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Validating quantity: {Quantity} for part: {PartId}", quantity, partId ?? "Any");

                var validationResult = new ValidationResult
                {
                    IsValid = true,
                    Errors = new List<string>()
                };

                if (quantity <= 0)
                {
                    validationResult.Errors.Add("Quantity must be greater than zero");
                }

                if (quantity > Model_AppVariables.MTM.MaxTransactionQuantity)
                {
                    validationResult.Errors.Add($"Quantity cannot exceed {Model_AppVariables.MTM.MaxTransactionQuantity}");
                }

                // Additional part-specific quantity validation could be added here
                // TODO: Add part-specific quantity constraints from database

                validationResult.IsValid = validationResult.Errors.Count == 0;
                return MTM_Shared_Logic.Models.Result<ValidationResult>.Success(validationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating quantity: {Quantity}", quantity);
                return MTM_Shared_Logic.Models.Result<ValidationResult>.Failure($"Quantity validation error: {ex.Message}");
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates an inventory transaction.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<ValidationResult>> ValidateTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
        {
            var validationResult = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };

            if (string.IsNullOrWhiteSpace(transaction.PartID))
            {
                validationResult.Errors.Add("Part ID is required");
            }

            if (transaction.Quantity <= 0)
            {
                validationResult.Errors.Add("Quantity must be greater than zero");
            }

            if (string.IsNullOrWhiteSpace(transaction.User))
            {
                validationResult.Errors.Add("User is required");
            }

            // Validate transaction type specific rules
            switch (transaction.TransactionType)
            {
                case TransactionType.TRANSFER:
                    if (string.IsNullOrWhiteSpace(transaction.FromLocation))
                        validationResult.Errors.Add("From location is required for transfers");
                    if (string.IsNullOrWhiteSpace(transaction.ToLocation))
                        validationResult.Errors.Add("To location is required for transfers");
                    break;
                case TransactionType.OUT:
                    if (string.IsNullOrWhiteSpace(transaction.FromLocation))
                        validationResult.Errors.Add("From location is required for removals");
                    break;
                case TransactionType.IN:
                    if (string.IsNullOrWhiteSpace(transaction.ToLocation))
                        validationResult.Errors.Add("To location is required for additions");
                    break;
            }

            validationResult.IsValid = validationResult.Errors.Count == 0;
            return MTM_Shared_Logic.Models.Result<ValidationResult>.Success(validationResult);
        }

        /// <summary>
        /// Validates a user object.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<ValidationResult>> ValidateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var validationResult = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };

            if (string.IsNullOrWhiteSpace(user.User_Name))
            {
                validationResult.Errors.Add("Username is required");
            }

            // Additional user validation rules can be added here

            validationResult.IsValid = validationResult.Errors.Count == 0;
            return MTM_Shared_Logic.Models.Result<ValidationResult>.Success(validationResult);
        }

        #endregion
    }

    /// <summary>
    /// In-memory cache service for performance optimization.
    /// Provides thread-safe caching capabilities with TTL support.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cache = new();
        private readonly ILogger<CacheService> _logger;
        private readonly Timer _cleanupTimer;

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;

            // Setup cleanup timer to run every 5 minutes
            _cleanupTimer = new Timer(CleanupExpiredItems, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Gets a cached value by key.
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return default(T);

                if (_cache.TryGetValue(key, out var item))
                {
                    if (item.IsExpired)
                    {
                        _cache.TryRemove(key, out _);
                        _logger.LogDebug("Cache item expired and removed: {Key}", key);
                        return default(T);
                    }

                    _logger.LogDebug("Cache hit: {Key}", key);
                    return (T)item.Value;
                }

                _logger.LogDebug("Cache miss: {Key}", key);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached value for key: {Key}", key);
                return default(T);
            }
        }

        /// <summary>
        /// Sets a value in cache with expiration.
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key) || value == null)
                    return;

                var expirationTime = expiration ?? TimeSpan.FromMinutes(10); // Default 10 minutes
                var cacheItem = new CacheItem(value, DateTime.UtcNow.Add(expirationTime));

                _cache.AddOrUpdate(key, cacheItem, (k, existing) => cacheItem);

                _logger.LogDebug("Cache item set: {Key}, Expires: {Expiration}", key, cacheItem.ExpiresAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
            }
        }

        /// <summary>
        /// Removes a value from cache.
        /// </summary>
        public async Task RemoveAsync(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return;

                _cache.TryRemove(key, out _);
                _logger.LogDebug("Cache item removed: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
            }
        }

        /// <summary>
        /// Clears all cached values.
        /// </summary>
        public async Task ClearAsync()
        {
            try
            {
                var count = _cache.Count;
                _cache.Clear();
                _logger.LogInformation("Cache cleared. {Count} items removed", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache");
            }
        }

        /// <summary>
        /// Checks if a key exists in cache.
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return false;

                if (_cache.TryGetValue(key, out var item))
                {
                    if (item.IsExpired)
                    {
                        _cache.TryRemove(key, out _);
                        return false;
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Cleanup expired cache items.
        /// </summary>
        private void CleanupExpiredItems(object? state)
        {
            try
            {
                var expiredKeys = new List<string>();

                foreach (var kvp in _cache)
                {
                    if (kvp.Value.IsExpired)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    _cache.TryRemove(key, out _);
                }

                if (expiredKeys.Count > 0)
                {
                    _logger.LogDebug("Cleanup removed {Count} expired cache items", expiredKeys.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cache cleanup");
            }
        }

        /// <summary>
        /// Disposes the cache service and cleanup timer.
        /// </summary>
        public void Dispose()
        {
            _cleanupTimer?.Dispose();
            _cache.Clear();
        }

        /// <summary>
        /// Represents a cached item with expiration.
        /// </summary>
        private class CacheItem
        {
            public object Value { get; }
            public DateTime ExpiresAt { get; }
            public bool IsExpired => DateTime.UtcNow > ExpiresAt;

            public CacheItem(object value, DateTime expiresAt)
            {
                Value = value;
                ExpiresAt = expiresAt;
            }
        }
    }

    /// <summary>
    /// Notification service for user alerts and messages.
    /// Provides centralized notification management across the application.
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Shows an information notification to the user.
        /// </summary>
        public async Task ShowInformationAsync(string title, string message)
        {
            try
            {
                _logger.LogInformation("Information notification: {Title} - {Message}", title, message);

                // TODO: Implement UI notification display
                // This would typically integrate with the UI framework to show notifications

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing information notification");
            }
        }

        /// <summary>
        /// Shows a warning notification to the user.
        /// </summary>
        public async Task ShowWarningAsync(string title, string message)
        {
            try
            {
                _logger.LogWarning("Warning notification: {Title} - {Message}", title, message);

                // TODO: Implement UI warning notification display

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing warning notification");
            }
        }

        /// <summary>
        /// Shows an error notification to the user.
        /// </summary>
        public async Task ShowErrorAsync(string title, string message)
        {
            try
            {
                _logger.LogError("Error notification: {Title} - {Message}", title, message);

                // TODO: Implement UI error notification display

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing error notification");
            }
        }

        /// <summary>
        /// Shows a success notification to the user.
        /// </summary>
        public async Task ShowSuccessAsync(string title, string message)
        {
            try
            {
                _logger.LogInformation("Success notification: {Title} - {Message}", title, message);

                // TODO: Implement UI success notification display

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing success notification");
            }
        }
    }

    /// <summary>
    /// Interface for notification services.
    /// </summary>
    public interface INotificationService
    {
        Task ShowInformationAsync(string title, string message);
        Task ShowWarningAsync(string title, string message);
        Task ShowErrorAsync(string title, string message);
        Task ShowSuccessAsync(string title, string message);
    }
}
