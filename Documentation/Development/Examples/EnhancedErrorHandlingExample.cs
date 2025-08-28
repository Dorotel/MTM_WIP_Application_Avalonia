using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using Avalonia.ReactiveUI;

namespace MTM_WIP_Application_Avalonia.Documentation.Development.Examples
{
    /// <summary>
    /// Enhanced example class demonstrating the new robust error handling system
    /// with ReactiveUI integration, Result patterns, and MTM business context.
    /// </summary>
    public class EnhancedErrorHandlingExample
    {
        private readonly string _currentUserId = "USER123";

        /// <summary>
        /// Example of modern async error handling with Result pattern and business context.
        /// </summary>
        public async Task<Result<string>> Example_ModernAsyncErrorHandling()
        {
            try
            {
                // Simulate a business operation with context
                var businessContext = new Dictionary<string, object>
                {
                    ["PartId"] = "PART001",
                    ["Operation"] = "110",
                    ["TransactionType"] = "IN",
                    ["Quantity"] = 25,
                    ["Location"] = "WIP-A"
                };

                // Simulate operation that might fail
                if (DateTime.Now.Millisecond % 2 == 0)
                {
                    throw new InvalidOperationException("Simulated business logic error");
                }

                return Result<string>.Success("Operation completed successfully");
            }
            catch (Exception ex)
            {
                // Use new enhanced error handling
                await Service_ErrorHandler.HandleErrorAsync(
                    ex, 
                    "InventoryAddOperation", 
                    _currentUserId,
                    new Dictionary<string, object>
                    {
                        ["PartId"] = "PART001",
                        ["Operation"] = "110",
                        ["Quantity"] = 25,
                        ["StoredProcedure"] = "inv_inventory_Add_Item_Enhanced"
                    });

                // Return user-friendly error message
                var userMessage = Service_ErrorHandler.GetUserFriendlyMessage(ex);
                return Result<string>.Failure(userMessage, "BUSINESS_ERROR", ex);
            }
        }

        /// <summary>
        /// Example of ReactiveUI command with enhanced error handling.
        /// </summary>
        public class EnhancedViewModel : ReactiveObject
        {
            public ReactiveCommand<Unit, Unit> SaveInventoryCommand { get; }
            public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

            private string _errorMessage = string.Empty;
            public string ErrorMessage
            {
                get => _errorMessage;
                set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
            }

            private bool _isLoading;
            public bool IsLoading
            {
                get => _isLoading;
                set => this.RaiseAndSetIfChanged(ref _isLoading, value);
            }

            public EnhancedViewModel()
            {
                // Command with enhanced error handling
                SaveInventoryCommand = ReactiveCommand.CreateFromTask(SaveInventoryAsync);
                
                // Use new ReactiveUI extension for centralized error handling
                SaveInventoryCommand.SubscribeToErrorsWithNotification(
                    "SaveInventoryOperation",
                    Environment.UserName,
                    error => ErrorMessage = error,
                    new Dictionary<string, object>
                    {
                        ["Component"] = "InventoryViewModel",
                        ["Action"] = "Save"
                    });

                LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
                
                // Simple error subscription for background operations
                LoadDataCommand.SubscribeToErrors(
                    "LoadDataOperation",
                    Environment.UserName,
                    new Dictionary<string, object>
                    {
                        ["Component"] = "InventoryViewModel",
                        ["Action"] = "Load"
                    });
            }

            private async Task SaveInventoryAsync()
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                try
                {
                    var result = await PerformInventoryOperationAsync()
                        .LogBusinessOperationAsync(
                            "SaveInventoryItem",
                            Environment.UserName,
                            new Dictionary<string, object>
                            {
                                ["PartId"] = "PART001",
                                ["Quantity"] = 10
                            });

                    result
                        .OnSuccess(_ => ErrorMessage = "Inventory saved successfully!")
                        .OnFailure(error => ErrorMessage = error);
                }
                finally
                {
                    IsLoading = false;
                }
            }

            private async Task LoadDataAsync()
            {
                // Background operation - errors logged but not shown to user
                var result = await LoadInventoryDataAsync();
                
                if (result.IsFailure)
                {
                    // Log audit trail for failed data load
                    await LoggingUtility.LogAuditTrailAsync(
                        "LoadInventoryData",
                        Environment.UserName,
                        "InventoryData",
                        "ALL_ITEMS");
                }
            }

            private async Task<Result<string>> PerformInventoryOperationAsync()
            {
                // Simulate stored procedure call pattern
                try
                {
                    // TODO: Replace with actual Helper_Database_StoredProcedure call
                    // var parameters = new Dictionary<string, object>
                    // {
                    //     ["p_PartID"] = "PART001",
                    //     ["p_Quantity"] = 10,
                    //     ["p_Operation"] = "110",
                    //     ["p_TransactionType"] = "IN",
                    //     ["p_UserID"] = Environment.UserName
                    // };
                    //
                    // var dbResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    //     Model_AppVariables.ConnectionString,
                    //     "inv_inventory_Add_Item_Enhanced",
                    //     parameters
                    // );
                    //
                    // return MTMResultExtensions.FromStoredProcedureResult<string>(
                    //     dbResult.Status,
                    //     dbResult.Message,
                    //     "Inventory operation completed");

                    // Simulate for demonstration
                    await Task.Delay(500);
                    
                    if (DateTime.Now.Millisecond % 3 == 0)
                        throw new TimeoutException("Database operation timed out");

                    return Result<string>.Success("Inventory operation completed");
                }
                catch (Exception ex)
                {
                    return Result<string>.Failure(
                        "Failed to save inventory item. Please try again.",
                        "DB_ERROR",
                        ex);
                }
            }

            private async Task<Result<List<string>>> LoadInventoryDataAsync()
            {
                try
                {
                    // Simulate data loading
                    await Task.Delay(300);
                    
                    var data = new List<string> { "Item1", "Item2", "Item3" };
                    return Result<List<string>>.Success(data);
                }
                catch (Exception ex)
                {
                    return Result<List<string>>.Failure(
                        "Failed to load inventory data",
                        "LOAD_ERROR",
                        ex);
                }
            }
        }

        /// <summary>
        /// Example of service layer error handling with Result patterns.
        /// </summary>
        public class EnhancedInventoryService
        {
            public async Task<Result<InventoryItem>> GetInventoryItemAsync(string partId)
            {
                try
                {
                    var context = new Dictionary<string, object>
                    {
                        ["PartId"] = partId,
                        ["Operation"] = "GetInventoryItem",
                        ["StoredProcedure"] = "inv_inventory_Get_ByPartID"
                    };

                    // TODO: Replace with actual stored procedure call
                    // var parameters = new Dictionary<string, object>
                    // {
                    //     ["p_PartID"] = partId
                    // };
                    //
                    // var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    //     Model_AppVariables.ConnectionString,
                    //     "inv_inventory_Get_ByPartID",
                    //     parameters
                    // );
                    //
                    // return MTMResultExtensions.FromStoredProcedureResult(
                    //     result.Status,
                    //     result.Message,
                    //     MapToInventoryItem(result.Data),
                    //     () => new InventoryItem { PartId = partId, Quantity = 0 });

                    // Simulation
                    await Task.Delay(200);
                    
                    if (string.IsNullOrWhiteSpace(partId))
                        return Result<InventoryItem>.Failure("Part ID is required", "VALIDATION_ERROR");

                    if (partId == "INVALID")
                        throw new ArgumentException("Invalid part ID format");

                    var item = new InventoryItem
                    {
                        PartId = partId,
                        Quantity = 100,
                        Location = "WIP-A"
                    };

                    return Result<InventoryItem>.Success(item);
                }
                catch (Exception ex)
                {
                    await Service_ErrorHandler.HandleErrorAsync(
                        ex,
                        "GetInventoryItem",
                        Environment.UserName,
                        new Dictionary<string, object> { ["PartId"] = partId });

                    return Result<InventoryItem>.Failure(
                        "Unable to retrieve inventory item. Please try again.",
                        "SERVICE_ERROR",
                        ex);
                }
            }

            public async Task<Result> UpdateInventoryAsync(string partId, int quantity, string transactionType)
            {
                try
                {
                    var context = new Dictionary<string, object>
                    {
                        ["PartId"] = partId,
                        ["Quantity"] = quantity,
                        ["TransactionType"] = transactionType,
                        ["Operation"] = "UpdateInventory",
                        ["StoredProcedure"] = "inv_inventory_Update_Quantity"
                    };

                    // Log audit trail for inventory changes
                    await LoggingUtility.LogAuditTrailAsync(
                        "UpdateInventory",
                        Environment.UserName,
                        "InventoryItem",
                        partId,
                        new Dictionary<string, object> { ["Action"] = "BeforeUpdate" },
                        new Dictionary<string, object> 
                        { 
                            ["NewQuantity"] = quantity,
                            ["TransactionType"] = transactionType
                        });

                    // Simulate database operation
                    await Task.Delay(300);

                    if (quantity < 0)
                        return Result.Failure("Quantity cannot be negative", "VALIDATION_ERROR");

                    return Result.Success();
                }
                catch (Exception ex)
                {
                    await Service_ErrorHandler.HandleErrorAsync(
                        ex,
                        "UpdateInventory",
                        Environment.UserName,
                        new Dictionary<string, object>
                        {
                            ["PartId"] = partId,
                            ["Quantity"] = quantity,
                            ["TransactionType"] = transactionType
                        });

                    return Result.Failure(
                        "Unable to update inventory. Please try again.",
                        "SERVICE_ERROR",
                        ex);
                }
            }
        }

        /// <summary>
        /// Example inventory item model.
        /// </summary>
        public class InventoryItem
        {
            public string PartId { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public string Location { get; set; } = string.Empty;
            public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        }

        /// <summary>
        /// Example of combining multiple operations with error handling.
        /// </summary>
        public async Task<Result> Example_CombinedOperations()
        {
            var service = new EnhancedInventoryService();

            try
            {
                // Get current inventory
                var getResult = await service.GetInventoryItemAsync("PART001");
                if (getResult.IsFailure)
                    return getResult.ToResult();

                // Update inventory
                var updateResult = await service.UpdateInventoryAsync("PART001", 50, "IN");
                if (updateResult.IsFailure)
                    return updateResult;

                // Log performance metrics
                await LoggingUtility.LogPerformanceMetricsAsync(
                    "CombinedInventoryOperation",
                    TimeSpan.FromMilliseconds(800),
                    Environment.UserName,
                    new Dictionary<string, object>
                    {
                        ["OperationsCompleted"] = 2,
                        ["Success"] = true
                    });

                return Result.Success();
            }
            catch (Exception ex)
            {
                await Service_ErrorHandler.HandleErrorAsync(
                    ex,
                    "CombinedInventoryOperation",
                    Environment.UserName);

                return Result.Failure("Combined operation failed", "COMBINED_ERROR", ex);
            }
        }

        /// <summary>
        /// Example of fluent error handling with Result pattern.
        /// </summary>
        public async Task<Result<string>> Example_FluentErrorHandling()
        {
            var service = new EnhancedInventoryService();

            return await service.GetInventoryItemAsync("PART001")
                .BindAsync(async item => 
                {
                    if (item.Quantity > 0)
                        return Result<string>.Success($"Item {item.PartId} has {item.Quantity} units");
                    else
                        return Result<string>.Warning($"Item {item.PartId} has zero quantity", "Item has zero quantity");
                })
                .OnSuccessAsync(async message => 
                {
                    await LoggingUtility.LogAuditTrailAsync(
                        "InventoryCheck",
                        Environment.UserName,
                        "InventoryItem",
                        "PART001");
                })
                .OnFailureAsync(async error =>
                {
                    Console.WriteLine($"Operation failed: {error}");
                });
        }
    }
}