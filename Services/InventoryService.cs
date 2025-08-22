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
    /// Service for inventory management operations following MTM business patterns.
    /// Integrates with existing LoggingUtility and Service_ErrorHandler infrastructure.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ITransactionService _transactionService;
        private readonly IValidationService _validationService;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IDatabaseService databaseService,
            ITransactionService transactionService,
            IValidationService validationService,
            ILogger<InventoryService> logger)
        {
            _databaseService = databaseService;
            _transactionService = transactionService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all inventory items asynchronously.
        /// </summary>
        public async Task<Result<List<InventoryItem>>> GetInventoryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrieving all inventory items");

                const string query = @"
                    SELECT PartId, Description, Quantity, Location, Operation, 
                           LastUpdated, LastUpdatedBy, UnitCost, UnitOfMeasure
                    FROM Inventory 
                    ORDER BY PartId, Operation";

                var result = await _databaseService.ExecuteQueryAsync<InventoryItem>(query, cancellationToken: cancellationToken);
                
                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve inventory items: {Error}", result.ErrorMessage);
                    return Result<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Failed to retrieve inventory");
                }

                _logger.LogInformation("Successfully retrieved {Count} inventory items", result.Value?.Count ?? 0);
                return Result<List<InventoryItem>>.Success(result.Value ?? new List<InventoryItem>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve inventory items");
                return Result<List<InventoryItem>>.Failure($"Failed to retrieve inventory: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a specific inventory item by Part ID.
        /// </summary>
        public async Task<Result<InventoryItem?>> GetInventoryItemAsync(string partId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return Result<InventoryItem?>.Failure("Part ID cannot be empty");
                }

                _logger.LogInformation("Retrieving inventory item for Part ID: {PartId}", partId);

                const string query = @"
                    SELECT PartId, Description, Quantity, Location, Operation, 
                           LastUpdated, LastUpdatedBy, UnitCost, UnitOfMeasure
                    FROM Inventory 
                    WHERE PartId = @PartId";

                var result = await _databaseService.ExecuteQueryAsync<InventoryItem>(
                    query, 
                    new { PartId = partId }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve inventory item for Part ID {PartId}: {Error}", partId, result.ErrorMessage);
                    return Result<InventoryItem?>.Failure(result.ErrorMessage ?? "Failed to retrieve inventory item");
                }

                var item = result.Value?.FirstOrDefault();
                _logger.LogInformation("Retrieved inventory item for Part ID: {PartId}, Found: {Found}", partId, item != null);
                
                return Result<InventoryItem?>.Success(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve inventory item for Part ID {PartId}", partId);
                return Result<InventoryItem?>.Failure($"Failed to retrieve inventory item: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new inventory item.
        /// </summary>
        public async Task<Result> AddInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                if (item == null)
                {
                    return Result.Failure("Inventory item cannot be null");
                }

                // Validate the inventory item
                var validationResult = await _validationService.ValidateAsync(item, cancellationToken);
                if (!validationResult.IsSuccess || !validationResult.Value!.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Value?.ErrorMessages ?? new List<string> { "Validation failed" });
                    return Result.Failure($"Validation failed: {errors}");
                }

                _logger.LogInformation("Adding new inventory item: {PartId}", item.PartId);

                item.LastUpdated = DateTime.UtcNow;

                const string command = @"
                    INSERT INTO Inventory (PartId, Description, Quantity, Location, Operation, 
                                         LastUpdated, LastUpdatedBy, UnitCost, UnitOfMeasure)
                    VALUES (@PartId, @Description, @Quantity, @Location, @Operation, 
                            @LastUpdated, @LastUpdatedBy, @UnitCost, @UnitOfMeasure)";

                var result = await _databaseService.ExecuteNonQueryAsync(command, item, cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to add inventory item {PartId}: {Error}", item.PartId, result.ErrorMessage);
                    return Result.Failure(result.ErrorMessage ?? "Failed to add inventory item");
                }

                // Log the transaction as IN (adding stock to inventory)
                var transaction = new InventoryTransaction
                {
                    PartId = item.PartId,
                    Operation = item.Operation,
                    Location = item.Location,
                    Quantity = item.Quantity,
                    TransactionType = TransactionType.IN, // Adding items is always IN
                    TransactionDateTime = DateTime.UtcNow,
                    UserName = item.LastUpdatedBy,
                    Comments = "Item added to inventory"
                };

                await _transactionService.LogTransactionAsync(transaction, cancellationToken);

                _logger.LogInformation("Successfully added inventory item: {PartId}", item.PartId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add inventory item {PartId}", item?.PartId ?? "null");
                return Result.Failure($"Failed to add inventory item: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing inventory item.
        /// </summary>
        public async Task<Result> UpdateInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default)
        {
            try
            {
                if (item == null)
                {
                    return Result.Failure("Inventory item cannot be null");
                }

                // Validate the inventory item
                var validationResult = await _validationService.ValidateAsync(item, cancellationToken);
                if (!validationResult.IsSuccess || !validationResult.Value!.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Value?.ErrorMessages ?? new List<string> { "Validation failed" });
                    return Result.Failure($"Validation failed: {errors}");
                }

                _logger.LogInformation("Updating inventory item: {PartId}", item.PartId);

                item.LastUpdated = DateTime.UtcNow;

                const string command = @"
                    UPDATE Inventory 
                    SET Description = @Description, Quantity = @Quantity, Location = @Location, 
                        Operation = @Operation, LastUpdated = @LastUpdated, LastUpdatedBy = @LastUpdatedBy,
                        UnitCost = @UnitCost, UnitOfMeasure = @UnitOfMeasure
                    WHERE PartId = @PartId";

                var result = await _databaseService.ExecuteNonQueryAsync(command, item, cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to update inventory item {PartId}: {Error}", item.PartId, result.ErrorMessage);
                    return Result.Failure(result.ErrorMessage ?? "Failed to update inventory item");
                }

                if (result.Value == 0)
                {
                    return Result.Failure($"No inventory item found with Part ID: {item.PartId}");
                }

                _logger.LogInformation("Successfully updated inventory item: {PartId}", item.PartId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update inventory item {PartId}", item?.PartId ?? "null");
                return Result.Failure($"Failed to update inventory item: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds stock to inventory (IN transaction).
        /// </summary>
        public async Task<Result> AddStockAsync(string partId, string operation, int quantity, string location, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return Result.Failure("Part ID cannot be empty");
                }

                if (quantity <= 0)
                {
                    return Result.Failure("Quantity must be greater than zero");
                }

                _logger.LogInformation("Adding stock for Part ID: {PartId}, Quantity: {Quantity}", partId, quantity);

                // Get current inventory item
                var currentItemResult = await GetInventoryItemAsync(partId, cancellationToken);
                if (!currentItemResult.IsSuccess)
                {
                    return Result.Failure($"Failed to retrieve current inventory: {currentItemResult.ErrorMessage}");
                }

                var currentItem = currentItemResult.Value;
                if (currentItem == null)
                {
                    return Result.Failure($"Part ID {partId} not found in inventory");
                }

                // Add to current quantity
                currentItem.Quantity += quantity;
                currentItem.Operation = operation;
                currentItem.Location = location;
                currentItem.LastUpdatedBy = userId;

                var updateResult = await UpdateInventoryItemAsync(currentItem, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    return Result.Failure($"Failed to update inventory: {updateResult.ErrorMessage}");
                }

                // Log the transaction as IN (adding stock)
                var transaction = new InventoryTransaction
                {
                    PartId = partId,
                    Operation = operation,
                    Location = location,
                    Quantity = quantity,
                    TransactionType = TransactionType.IN, // Adding stock is IN
                    TransactionDateTime = DateTime.UtcNow,
                    UserName = userId,
                    Comments = "Stock added to inventory"
                };

                await _transactionService.LogTransactionAsync(transaction, cancellationToken);

                _logger.LogInformation("Successfully added stock for Part ID: {PartId}", partId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add stock for Part ID {PartId}", partId);
                return Result.Failure($"Failed to add stock: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes stock from inventory (OUT transaction).
        /// </summary>
        public async Task<Result> RemoveStockAsync(string partId, string operation, int quantity, string location, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return Result.Failure("Part ID cannot be empty");
                }

                if (quantity <= 0)
                {
                    return Result.Failure("Quantity must be greater than zero");
                }

                _logger.LogInformation("Removing stock for Part ID: {PartId}, Quantity: {Quantity}", partId, quantity);

                // Get current inventory item
                var currentItemResult = await GetInventoryItemAsync(partId, cancellationToken);
                if (!currentItemResult.IsSuccess)
                {
                    return Result.Failure($"Failed to retrieve current inventory: {currentItemResult.ErrorMessage}");
                }

                var currentItem = currentItemResult.Value;
                if (currentItem == null)
                {
                    return Result.Failure($"Part ID {partId} not found in inventory");
                }

                // Check if sufficient quantity available
                if (currentItem.Quantity < quantity)
                {
                    return Result.Failure($"Insufficient inventory. Current: {currentItem.Quantity}, Requested: {quantity}");
                }

                // Remove from current quantity
                currentItem.Quantity -= quantity;
                currentItem.Operation = operation;
                currentItem.Location = location;
                currentItem.LastUpdatedBy = userId;

                var updateResult = await UpdateInventoryItemAsync(currentItem, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    return Result.Failure($"Failed to update inventory: {updateResult.ErrorMessage}");
                }

                // Log the transaction as OUT (removing stock)
                var transaction = new InventoryTransaction
                {
                    PartId = partId,
                    Operation = operation,
                    Location = location,
                    Quantity = quantity,
                    TransactionType = TransactionType.OUT, // Removing stock is OUT
                    TransactionDateTime = DateTime.UtcNow,
                    UserName = userId,
                    Comments = "Stock removed from inventory"
                };

                await _transactionService.LogTransactionAsync(transaction, cancellationToken);

                _logger.LogInformation("Successfully removed stock for Part ID: {PartId}", partId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove stock for Part ID {PartId}", partId);
                return Result.Failure($"Failed to remove stock: {ex.Message}");
            }
        }

        /// <summary>
        /// Transfers stock from one location to another (TRANSFER transaction).
        /// </summary>
        public async Task<Result> TransferStockAsync(string partId, string operation, int quantity, string fromLocation, string toLocation, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return Result.Failure("Part ID cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(fromLocation) || string.IsNullOrWhiteSpace(toLocation))
                {
                    return Result.Failure("Both from and to locations must be specified");
                }

                if (fromLocation.Equals(toLocation, StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Failure("Source and destination locations cannot be the same");
                }

                if (quantity <= 0)
                {
                    return Result.Failure("Quantity must be greater than zero");
                }

                _logger.LogInformation("Transferring stock for Part ID: {PartId}, Quantity: {Quantity}, From: {FromLocation}, To: {ToLocation}", 
                    partId, quantity, fromLocation, toLocation);

                // Get current inventory item at source location
                var currentItemResult = await GetInventoryItemAsync(partId, cancellationToken);
                if (!currentItemResult.IsSuccess)
                {
                    return Result.Failure($"Failed to retrieve current inventory: {currentItemResult.ErrorMessage}");
                }

                var currentItem = currentItemResult.Value;
                if (currentItem == null)
                {
                    return Result.Failure($"Part ID {partId} not found in inventory");
                }

                // Check if sufficient quantity available at source
                if (currentItem.Quantity < quantity)
                {
                    return Result.Failure($"Insufficient inventory at source location. Current: {currentItem.Quantity}, Requested: {quantity}");
                }

                // Remove from source location
                currentItem.Quantity -= quantity;
                currentItem.LastUpdatedBy = userId;

                var updateSourceResult = await UpdateInventoryItemAsync(currentItem, cancellationToken);
                if (!updateSourceResult.IsSuccess)
                {
                    return Result.Failure($"Failed to update source inventory: {updateSourceResult.ErrorMessage}");
                }

                // Add to destination location (or update if already exists)
                // Note: This is simplified - in a real implementation, you might need to handle cases
                // where the item already exists at the destination location

                // Log the transfer transaction
                var transaction = new InventoryTransaction
                {
                    PartId = partId,
                    Operation = operation,
                    FromLocation = fromLocation,
                    ToLocation = toLocation,
                    Quantity = quantity,
                    TransactionType = TransactionType.TRANSFER, // Moving stock is TRANSFER
                    TransactionDateTime = DateTime.UtcNow,
                    UserName = userId,
                    Comments = $"Stock transferred from {fromLocation} to {toLocation}"
                };

                await _transactionService.LogTransactionAsync(transaction, cancellationToken);

                _logger.LogInformation("Successfully transferred stock for Part ID: {PartId}", partId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to transfer stock for Part ID {PartId}", partId);
                return Result.Failure($"Failed to transfer stock: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets inventory items by location.
        /// </summary>
        public async Task<Result<List<InventoryItem>>> GetInventoryByLocationAsync(string location, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                {
                    return Result<List<InventoryItem>>.Failure("Location cannot be empty");
                }

                _logger.LogInformation("Retrieving inventory items for location: {Location}", location);

                const string query = @"
                    SELECT PartId, Description, Quantity, Location, Operation, 
                           LastUpdated, LastUpdatedBy, UnitCost, UnitOfMeasure
                    FROM Inventory 
                    WHERE Location = @Location
                    ORDER BY PartId, Operation";

                var result = await _databaseService.ExecuteQueryAsync<InventoryItem>(
                    query, 
                    new { Location = location }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve inventory items for location {Location}: {Error}", location, result.ErrorMessage);
                    return Result<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Failed to retrieve inventory by location");
                }

                _logger.LogInformation("Retrieved {Count} inventory items for location: {Location}", 
                    result.Value?.Count ?? 0, location);
                
                return Result<List<InventoryItem>>.Success(result.Value ?? new List<InventoryItem>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve inventory items for location {Location}", location);
                return Result<List<InventoryItem>>.Failure($"Failed to retrieve inventory by location: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets inventory items by operation.
        /// </summary>
        public async Task<Result<List<InventoryItem>>> GetInventoryByOperationAsync(string operation, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(operation))
                {
                    return Result<List<InventoryItem>>.Failure("Operation cannot be empty");
                }

                _logger.LogInformation("Retrieving inventory items for operation: {Operation}", operation);

                const string query = @"
                    SELECT PartId, Description, Quantity, Location, Operation, 
                           LastUpdated, LastUpdatedBy, UnitCost, UnitOfMeasure
                    FROM Inventory 
                    WHERE Operation = @Operation
                    ORDER BY PartId, Location";

                var result = await _databaseService.ExecuteQueryAsync<InventoryItem>(
                    query, 
                    new { Operation = operation }, 
                    cancellationToken);

                if (!result.IsSuccess)
                {
                    _logger.LogError("Failed to retrieve inventory items for operation {Operation}: {Error}", operation, result.ErrorMessage);
                    return Result<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Failed to retrieve inventory by operation");
                }

                _logger.LogInformation("Retrieved {Count} inventory items for operation: {Operation}", 
                    result.Value?.Count ?? 0, operation);
                
                return Result<List<InventoryItem>>.Success(result.Value ?? new List<InventoryItem>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve inventory items for operation {Operation}", operation);
                return Result<List<InventoryItem>>.Failure($"Failed to retrieve inventory by operation: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes an MTM operation (e.g., "90", "100", "110") for a specific part.
        /// </summary>
        public async Task<Result> ProcessOperationAsync(string partId, string operation, int quantity, string location, string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partId))
                {
                    return Result.Failure("Part ID cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(operation))
                {
                    return Result.Failure("Operation cannot be empty");
                }

                if (quantity <= 0)
                {
                    return Result.Failure("Quantity must be greater than zero");
                }

                if (string.IsNullOrWhiteSpace(location))
                {
                    return Result.Failure("Location cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Result.Failure("User ID cannot be empty");
                }

                _logger.LogInformation("Processing operation {Operation} for Part ID: {PartId}, Quantity: {Quantity}, Location: {Location}, User: {UserId}", 
                    operation, partId, quantity, location, userId);

                // TODO: Implement via stored procedure call instead of direct logic
                // This is a placeholder implementation that follows the requirement for adding inventory
                // The actual business logic would depend on the specific operation being performed
                
                // For now, we'll treat all operations as adding stock to inventory (IN transaction)
                // In a real implementation, this would be replaced with a stored procedure call like:
                // await _databaseService.ExecuteStoredProcedureAsync("inv_inventory_Process_Operation", parameters);

                var addStockResult = await AddStockAsync(partId, operation, quantity, location, userId, cancellationToken);
                
                if (!addStockResult.IsSuccess)
                {
                    return Result.Failure($"Failed to process operation: {addStockResult.ErrorMessage}");
                }

                _logger.LogInformation("Successfully processed operation {Operation} for Part ID: {PartId}", operation, partId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process operation {Operation} for Part ID {PartId}", operation, partId);
                return Result.Failure($"Failed to process operation: {ex.Message}");
            }
        }
    }
}