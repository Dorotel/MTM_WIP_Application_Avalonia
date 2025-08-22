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

                // Log the transaction
                var transaction = new InventoryTransaction
                {
                    PartId = item.PartId,
                    Operation = item.Operation,
                    Location = item.Location,
                    Quantity = item.Quantity,
                    TransactionType = "ADD",
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

                _logger.LogInformation("Processing operation {Operation} for Part ID: {PartId}, Quantity: {Quantity}", 
                    operation, partId, quantity);

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

                // Update quantity based on operation type
                var newQuantity = DetermineNewQuantity(currentItem.Quantity, quantity, operation);
                
                if (newQuantity < 0)
                {
                    return Result.Failure($"Insufficient inventory. Current: {currentItem.Quantity}, Requested: {quantity}");
                }

                // Update the inventory item
                currentItem.Quantity = newQuantity;
                currentItem.Operation = operation;
                currentItem.Location = location;
                currentItem.LastUpdatedBy = userId;

                var updateResult = await UpdateInventoryItemAsync(currentItem, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    return Result.Failure($"Failed to update inventory: {updateResult.ErrorMessage}");
                }

                // Log the transaction
                var transaction = new InventoryTransaction
                {
                    PartId = partId,
                    Operation = operation,
                    Location = location,
                    Quantity = quantity,
                    TransactionType = GetTransactionType(operation),
                    TransactionDateTime = DateTime.UtcNow,
                    UserName = userId,
                    Comments = $"Operation {operation} processed"
                };

                await _transactionService.LogTransactionAsync(transaction, cancellationToken);

                _logger.LogInformation("Successfully processed operation {Operation} for Part ID: {PartId}", operation, partId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process operation {Operation} for Part ID {PartId}", operation, partId);
                return Result.Failure($"Failed to process operation: {ex.Message}");
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
        /// Determines the new quantity based on the operation type.
        /// MTM operations: "90" = IN, "100" = OUT, "110" = TRANSFER
        /// </summary>
        private static int DetermineNewQuantity(int currentQuantity, int operationQuantity, string operation)
        {
            return operation switch
            {
                "90" => currentQuantity + operationQuantity,  // IN operation
                "100" => currentQuantity - operationQuantity, // OUT operation
                "110" => currentQuantity,                     // TRANSFER operation (quantity stays same at origin)
                _ => currentQuantity // Unknown operation, no change
            };
        }

        /// <summary>
        /// Gets the transaction type based on the operation.
        /// </summary>
        private static string GetTransactionType(string operation)
        {
            return operation switch
            {
                "90" => "IN",
                "100" => "OUT",
                "110" => "TRANSFER",
                _ => "OTHER"
            };
        }
    }
}