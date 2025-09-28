using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services.Interfaces;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Transfer service implementation for inventory transfer operations
    /// Handles transfer execution, validation, and inventory search functionality
    /// </summary>
    public class TransferService : ITransferService
    {
        private readonly ILogger<TransferService> _logger;
        private readonly string _connectionString;

        public TransferService(ILogger<TransferService> logger, IConfigurationService configurationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ArgumentNullException.ThrowIfNull(configurationService);
            _connectionString = configurationService.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection string not found");
        }

        public async Task<ServiceResult<List<InventoryItem>>> SearchInventoryAsync(string? partId = null, string? operation = null)
        {
            try
            {
                _logger.LogDebug("Searching inventory with PartId: {PartId}, Operation: {Operation}", partId, operation);

                MySqlParameter[] parameters;
                string procedureName;

                // Use specific procedures based on search criteria
                if (!string.IsNullOrWhiteSpace(partId) && !string.IsNullOrWhiteSpace(operation))
                {
                    procedureName = "inv_inventory_Get_ByPartIDandOperation";
                    parameters = new MySqlParameter[]
                    {
                        new("p_PartID", partId),
                        new("p_Operation", operation)
                    };
                }
                else if (!string.IsNullOrWhiteSpace(partId))
                {
                    procedureName = "inv_inventory_Get_ByPartID";
                    parameters = new MySqlParameter[]
                    {
                        new("p_PartID", partId)
                    };
                }
                else
                {
                    procedureName = "inv_inventory_Get_All";
                    parameters = Array.Empty<MySqlParameter>();
                }

                var paramDict = new Dictionary<string, object>();
                foreach (var param in parameters)
                {
                    paramDict[param.ParameterName] = param.Value ?? DBNull.Value;
                }

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString, procedureName, paramDict);

                if (result.Status != 1)
                {
                    _logger.LogWarning("Inventory search failed with status: {Status}, Message: {Message}",
                        result.Status, result.Message);
                    return ServiceResult<List<InventoryItem>>.Failure($"Search failed: {result.Message}");
                }

                var inventoryItems = new List<InventoryItem>();

                if (result.Data != null)
                {
                    foreach (DataRow row in result.Data.Rows)
                    {
                        var item = new InventoryItem
                        {
                            Id = Convert.ToInt32(row["ID"]),
                            PartId = row["PartID"]?.ToString() ?? string.Empty,
                            Location = row["Location"]?.ToString() ?? string.Empty,
                            Operation = row["Operation"]?.ToString() ?? string.Empty,
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            ItemType = row["ItemType"]?.ToString() ?? "WIP",
                            ReceiveDate = Convert.ToDateTime(row["ReceiveDate"]),
                            LastUpdated = Convert.ToDateTime(row["LastUpdated"]),
                            User = row["User"]?.ToString() ?? string.Empty,
                            BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                            Notes = row["Notes"]?.ToString() ?? string.Empty
                        };
                        inventoryItems.Add(item);
                    }
                }

                _logger.LogDebug("Found {Count} inventory items", inventoryItems.Count);
                return ServiceResult<List<InventoryItem>>.Success(inventoryItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching inventory");
                return ServiceResult<List<InventoryItem>>.Failure($"Search error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TransferResult>> ExecuteTransferAsync(TransferOperation transferOperation)
        {
            try
            {
                _logger.LogDebug("Executing transfer: {FromLocation} -> {ToLocation}, Quantity: {Quantity}",
                    transferOperation.FromLocation, transferOperation.ToLocation, transferOperation.TransferQuantity);

                var transferValidationResult = await ValidateTransferAsync(transferOperation);
                if (!transferValidationResult.Value?.IsValid == true)
                {
                    return ServiceResult<TransferResult>.Failure($"Transfer validation failed: {string.Join(", ", transferValidationResult.Value?.Errors ?? new List<string>())}");
                }

                var parameters = new MySqlParameter[]
                {
                    new("p_PartId", transferOperation.PartId),
                    new("p_Operation", transferOperation.Operation),
                    new("p_FromLocation", transferOperation.FromLocation),
                    new("p_ToLocation", transferOperation.ToLocation),
                    new("p_TransferQuantity", transferOperation.TransferQuantity),
                    new("p_UserId", transferOperation.UserId),
                    new("p_Notes", transferOperation.Notes ?? string.Empty)
                };

                var paramDict = new Dictionary<string, object>();
                foreach (var param in parameters)
                {
                    paramDict[param.ParameterName] = param.Value ?? DBNull.Value;
                }

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString, "inv_transfer_Execute_WithSplit", paramDict);

                if (result.Status == 1)
                {
                    _logger.LogInformation("Transfer executed successfully for part: {PartId}", transferOperation.PartId);
                    return ServiceResult<TransferResult>.Success(TransferResult.Success(
                        Guid.NewGuid().ToString(),
                        transferOperation.TransferQuantity,
                        transferOperation.TransferQuantity,
                        0,
                        "Transfer completed successfully"));
                }
                else
                {
                    _logger.LogWarning("Transfer failed with status: {Status}, Message: {Message}", result.Status, result.Message);
                    return ServiceResult<TransferResult>.Success(TransferResult.Failure(
                        result.Message ?? "Transfer failed"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing transfer for part: {PartId}", transferOperation.PartId);
                return ServiceResult<TransferResult>.Failure($"Transfer execution error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TransferValidationResult>> ValidateTransferAsync(TransferOperation transferOperation)
        {
            try
            {
                _logger.LogDebug("Validating transfer for part: {PartId}", transferOperation.PartId);

                var validation = new TransferValidationResult();

                // Use the validation method from the model
                var modelErrors = transferOperation.GetValidationErrors();
                validation.Errors.AddRange(modelErrors);

                // Additional database validation
                if (validation.Errors.Count == 0)
                {
                    // Validate locations exist
                    var validLocations = await GetValidLocationsAsync();
                    if (validLocations.IsSuccess && validLocations.Value != null)
                    {
                        if (!validLocations.Value.Contains(transferOperation.FromLocation))
                            validation.Errors.Add($"Invalid from location: {transferOperation.FromLocation}");

                        if (!validLocations.Value.Contains(transferOperation.ToLocation))
                            validation.Errors.Add($"Invalid to location: {transferOperation.ToLocation}");
                    }
                }

                _logger.LogDebug("Transfer validation completed. Valid: {IsValid}, Errors: {ErrorCount}",
                    validation.IsValid, validation.Errors.Count);

                return ServiceResult<TransferValidationResult>.Success(validation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating transfer");
                var validation = new TransferValidationResult();
                validation.Errors.Add($"Validation error: {ex.Message}");
                return ServiceResult<TransferValidationResult>.Success(validation);
            }
        }

        public async Task<ServiceResult<List<string>>> GetValidLocationsAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving valid locations");

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString, "md_locations_Get_All", new Dictionary<string, object>());

                if (result.Status != 1)
                {
                    _logger.LogWarning("Failed to retrieve locations with status: {Status}", result.Status);
                    return ServiceResult<List<string>>.Failure($"Failed to retrieve locations: {result.Message}");
                }

                var locations = new List<string>();

                if (result.Data != null)
                {
                    foreach (DataRow row in result.Data.Rows)
                    {
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(location))
                        {
                            locations.Add(location);
                        }
                    }
                }

                _logger.LogDebug("Retrieved {Count} valid locations", locations.Count);
                return ServiceResult<List<string>>.Success(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving valid locations");
                return ServiceResult<List<string>>.Failure($"Error retrieving locations: {ex.Message}");
            }
        }
    }
}
