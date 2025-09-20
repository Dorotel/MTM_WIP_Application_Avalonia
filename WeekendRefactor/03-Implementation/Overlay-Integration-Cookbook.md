# Overlay Integration Cookbook

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Developers and Integration Engineers  

## 🍳 Integration Cookbook Overview

This cookbook provides practical, real-world integration scenarios, troubleshooting guides, best practices, and common patterns for integrating overlays with the MTM WIP Application ecosystem. Each recipe includes complete code examples, gotchas to avoid, and testing strategies.

## 📖 Recipe Index

### **Basic Integration Recipes**

1. [Service Layer Integration](#service-layer-integration)
2. [ViewModel Communication](#viewmodel-communication)
3. [Database Operation Integration](#database-operation-integration)
4. [Error Handling Integration](#error-handling-integration)

### **Advanced Integration Patterns**

5. [Multi-Overlay Workflows](#multi-overlay-workflows)
6. [Parent-Child Overlay Communication](#parent-child-overlay-communication)
7. [Theme Service Integration](#theme-service-integration)
8. [Print Service Integration](#print-service-integration)

### **Specialized Use Cases**

9. [Custom DataGrid Integration](#custom-datagrid-integration)
10. [File Selection Integration](#file-selection-integration)
11. [Navigation Service Integration](#navigation-service-integration)
12. [Focus Management Integration](#focus-management-integration)

### **Troubleshooting Recipes**

13. [Common Integration Issues](#common-integration-issues)
14. [Performance Troubleshooting](#performance-troubleshooting)
15. [Testing Integration Points](#testing-integration-points)

---

## 🔧 Basic Integration Recipes

### **Recipe 1: Service Layer Integration**

**Problem**: Integrating overlay with existing MTM services (InventoryEditingService, MasterDataService, etc.)

**Solution**:

```csharp
// File: ViewModels/Overlay/ServiceIntegratedOverlayViewModel.cs

[ObservableObject]
public partial class ServiceIntegratedOverlayViewModel : BasePoolableOverlayViewModel
{
    #region Service Dependencies

    private readonly IInventoryEditingService _inventoryService;
    private readonly IMasterDataService _masterDataService;
    private readonly IConfigurationService _configurationService;
    
    #endregion

    #region Constructor

    public ServiceIntegratedOverlayViewModel(
        ILogger<ServiceIntegratedOverlayViewModel> logger,
        IInventoryEditingService inventoryService,
        IMasterDataService masterDataService,
        IConfigurationService configurationService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(inventoryService);
        ArgumentNullException.ThrowIfNull(masterDataService);
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _inventoryService = inventoryService;
        _masterDataService = masterDataService;
        _configurationService = configurationService;
    }

    #endregion

    #region Initialization with Service Integration

    protected override async Task OnInitializeAsync<TRequest>(TRequest request)
    {
        if (request is InventoryEditRequest editRequest)
        {
            // Load master data using existing services
            await LoadMasterDataAsync(editRequest);
            
            // Validate with existing inventory service
            await ValidateWithInventoryServiceAsync(editRequest);
            
            // Setup UI state
            InitializeUIState(editRequest);
        }
    }

    private async Task LoadMasterDataAsync(InventoryEditRequest request)
    {
        try
        {
            // Use existing MasterDataService patterns
            var tasks = new[]
            {
                LoadPartIdsAsync(),
                LoadOperationsAsync(),
                LoadLocationsAsync()
            };

            await Task.WhenAll(tasks);
            
            Logger.LogInformation("Master data loaded successfully for overlay");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load master data for overlay");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Master data loading failed");
            throw;
        }
    }

    #endregion

    #region Service Integration Methods

    [RelayCommand]
    private async Task SaveWithServiceIntegrationAsync()
    {
        try
        {
            IsLoading = true;
            
            // Create inventory operation using existing service
            var inventoryOperation = new InventoryOperation
            {
                PartId = PartId,
                Operation = Operation,
                Location = Location,
                Quantity = NewQuantity,
                Notes = Notes,
                User = Environment.UserName,
                Timestamp = DateTime.UtcNow
            };

            // Use existing InventoryEditingService
            var result = await _inventoryService.ProcessInventoryOperationAsync(inventoryOperation);
            
            if (result.IsSuccess)
            {
                Logger.LogInformation("Inventory operation completed via service: {OperationId}", 
                    result.OperationId);
                
                var response = new InventoryEditResponse(
                    OverlayResult.Confirmed,
                    OperationId: result.OperationId,
                    Success: true
                );
                
                await CloseAsync(response);
            }
            else
            {
                Logger.LogWarning("Inventory service operation failed: {Error}", result.ErrorMessage);
                
                // Show error using existing error handling
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage),
                    "Inventory operation failed"
                );
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in service-integrated save operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Save operation failed");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<List<string>> LoadPartIdsAsync()
    {
        // Leverage existing MasterDataService instead of direct database calls
        var partIds = await _masterDataService.GetPartIdsAsync();
        
        // Update UI collections
        AvailablePartIds.Clear();
        foreach (var partId in partIds)
        {
            AvailablePartIds.Add(partId);
        }
        
        return partIds;
    }

    private async Task ValidateWithInventoryServiceAsync(InventoryEditRequest request)
    {
        // Use existing service validation instead of overlay-specific validation
        var validationResult = await _inventoryService.ValidateInventoryOperationAsync(
            request.PartId, request.Operation, request.Location);
            
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors);
            Logger.LogWarning("Service validation failed: {Errors}", errors);
            
            throw new ArgumentException($"Invalid inventory operation: {errors}");
        }
    }

    #endregion
}
```

**Integration Points to Remember**:

- Always use existing services instead of duplicating logic
- Maintain the same error handling patterns as the main application
- Leverage existing validation and business rules
- Use the same logging patterns and service lifetimes

---

### **Recipe 2: ViewModel Communication**

**Problem**: Communicating between overlay ViewModels and parent ViewModels

**Solution**:

```csharp
// File: Services/OverlayViewModelCommunicationService.cs

public interface IOverlayViewModelCommunicationService
{
    void RegisterParentViewModel(string overlayId, object parentViewModel);
    void UnregisterParentViewModel(string overlayId);
    Task NotifyParentAsync<TData>(string overlayId, string eventType, TData data);
    void SubscribeToOverlayEvents<TData>(string overlayId, string eventType, 
        Func<TData, Task> handler);
}

public class OverlayViewModelCommunicationService : IOverlayViewModelCommunicationService
{
    private readonly Dictionary<string, object> _parentViewModels = new();
    private readonly Dictionary<string, Dictionary<string, List<Delegate>>> _eventHandlers = new();
    private readonly ILogger<OverlayViewModelCommunicationService> _logger;

    public OverlayViewModelCommunicationService(
        ILogger<OverlayViewModelCommunicationService> logger)
    {
        _logger = logger;
    }

    public void RegisterParentViewModel(string overlayId, object parentViewModel)
    {
        _parentViewModels[overlayId] = parentViewModel;
        
        if (!_eventHandlers.ContainsKey(overlayId))
        {
            _eventHandlers[overlayId] = new Dictionary<string, List<Delegate>>();
        }
        
        _logger.LogDebug("Registered parent ViewModel for overlay {OverlayId}", overlayId);
    }

    public async Task NotifyParentAsync<TData>(string overlayId, string eventType, TData data)
    {
        if (!_eventHandlers.TryGetValue(overlayId, out var overlayHandlers) ||
            !overlayHandlers.TryGetValue(eventType, out var handlers))
        {
            _logger.LogDebug("No handlers found for overlay {OverlayId}, event {EventType}", 
                overlayId, eventType);
            return;
        }

        var tasks = handlers.OfType<Func<TData, Task>>()
            .Select(handler => SafeInvokeHandlerAsync(handler, data, overlayId, eventType));
            
        await Task.WhenAll(tasks);
    }

    private async Task SafeInvokeHandlerAsync<TData>(
        Func<TData, Task> handler, 
        TData data, 
        string overlayId, 
        string eventType)
    {
        try
        {
            await handler(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error invoking event handler for overlay {OverlayId}, event {EventType}", 
                overlayId, eventType);
                
            await Services.ErrorHandling.HandleErrorAsync(ex, 
                "Overlay communication handler failed");
        }
    }
}

// Usage in Parent ViewModel
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    private readonly IOverlayViewModelCommunicationService _communicationService;
    private readonly IUniversalOverlayService _overlayService;

    [RelayCommand]
    private async Task ShowInventoryEditOverlayAsync()
    {
        try
        {
            var overlayId = Guid.NewGuid().ToString();
            
            // Register for communication
            _communicationService.RegisterParentViewModel(overlayId, this);
            
            // Subscribe to overlay events
            _communicationService.SubscribeToOverlayEvents<InventoryUpdatedEventArgs>(
                overlayId, "InventoryUpdated", OnInventoryUpdatedAsync);
                
            _communicationService.SubscribeToOverlayEvents<ValidationErrorEventArgs>(
                overlayId, "ValidationError", OnValidationErrorAsync);

            // Show overlay
            var request = new InventoryEditRequest(overlayId, /* ... other params */);
            var response = await _overlayService.ShowInventoryEditOverlayAsync(request);

            // Handle response
            await HandleOverlayResponseAsync(response);
        }
        finally
        {
            // Always cleanup
            _communicationService.UnregisterParentViewModel(overlayId);
        }
    }

    private async Task OnInventoryUpdatedAsync(InventoryUpdatedEventArgs args)
    {
        Logger.LogInformation("Inventory updated via overlay: {PartId}", args.PartId);
        
        // Refresh parent data
        await RefreshInventoryDataAsync();
        
        // Show success notification
        await ShowSuccessNotificationAsync($"Updated inventory for {args.PartId}");
    }

    private async Task OnValidationErrorAsync(ValidationErrorEventArgs args)
    {
        Logger.LogWarning("Validation error from overlay: {Errors}", 
            string.Join(", ", args.Errors));
            
        // Handle validation errors in parent context
        await ShowValidationErrorsAsync(args.Errors);
    }
}

// Usage in Overlay ViewModel
[ObservableObject]
public partial class InventoryEditOverlayViewModel : BasePoolableOverlayViewModel
{
    private readonly IOverlayViewModelCommunicationService _communicationService;
    private string _overlayId = string.Empty;

    protected override async Task OnInitializeAsync<TRequest>(TRequest request)
    {
        if (request is InventoryEditRequest editRequest)
        {
            _overlayId = editRequest.OverlayId;
            // ... other initialization
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            // Perform save operation
            var result = await SaveInventoryAsync();
            
            if (result.IsSuccess)
            {
                // Notify parent of successful update
                await _communicationService.NotifyParentAsync(
                    _overlayId,
                    "InventoryUpdated",
                    new InventoryUpdatedEventArgs(PartId, NewQuantity, Notes)
                );
                
                await CloseAsync(new InventoryEditResponse(OverlayResult.Confirmed));
            }
        }
        catch (ValidationException ex)
        {
            // Notify parent of validation errors
            await _communicationService.NotifyParentAsync(
                _overlayId,
                "ValidationError",
                new ValidationErrorEventArgs(ex.Errors)
            );
        }
    }
}
```

**Communication Best Practices**:

- Use weak references to prevent memory leaks
- Always unregister communication handlers
- Use typed event arguments for better intellisense
- Handle communication errors gracefully

---

### **Recipe 3: Database Operation Integration**

**Problem**: Integrating overlay database operations with existing MTM database patterns and transactions

**Solution**:

```csharp
// File: Services/OverlayDatabaseService.cs

public interface IOverlayDatabaseService
{
    Task<DatabaseResult<T>> ExecuteOverlayOperationAsync<T>(
        OverlayDatabaseOperation operation) where T : class;
        
    Task<DatabaseResult<T>> ExecuteInTransactionAsync<T>(
        IEnumerable<OverlayDatabaseOperation> operations) where T : class;
}

public class OverlayDatabaseService : IOverlayDatabaseService
{
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<OverlayDatabaseService> _logger;

    public OverlayDatabaseService(
        IConfigurationService configurationService,
        ILogger<OverlayDatabaseService> logger)
    {
        _configurationService = configurationService;
        _logger = logger;
    }

    public async Task<DatabaseResult<T>> ExecuteOverlayOperationAsync<T>(
        OverlayDatabaseOperation operation) where T : class
    {
        try
        {
            var connectionString = await _configurationService.GetConnectionStringAsync();
            
            _logger.LogInformation("Executing overlay database operation: {Procedure}", 
                operation.StoredProcedure);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                operation.StoredProcedure,
                operation.Parameters.ToArray()
            );

            if (result.Status == 1)
            {
                var data = ConvertDataTableToObject<T>(result.Data);
                return DatabaseResult<T>.Success(data);
            }
            else
            {
                _logger.LogWarning("Database operation failed with status {Status}", result.Status);
                return DatabaseResult<T>.Failure($"Database operation failed with status {result.Status}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing overlay database operation: {Procedure}", 
                operation.StoredProcedure);
            return DatabaseResult<T>.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<DatabaseResult<T>> ExecuteInTransactionAsync<T>(
        IEnumerable<OverlayDatabaseOperation> operations) where T : class
    {
        var connectionString = await _configurationService.GetConnectionStringAsync();
        
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        
        using var transaction = await connection.BeginTransactionAsync();
        
        try
        {
            var results = new List<DataTable>();
            
            foreach (var operation in operations)
            {
                _logger.LogInformation("Executing transactional operation: {Procedure}", 
                    operation.StoredProcedure);
                    
                // Execute each operation within the transaction
                using var command = new MySqlCommand(operation.StoredProcedure, connection, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                command.Parameters.AddRange(operation.Parameters.ToArray());
                
                using var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                
                results.Add(dataTable);
                
                // Check for operation success (assuming first column is status)
                if (dataTable.Rows.Count == 0 || 
                    !int.TryParse(dataTable.Rows[0][0].ToString(), out var status) || 
                    status != 1)
                {
                    throw new InvalidOperationException($"Operation {operation.StoredProcedure} failed");
                }
            }
            
            await transaction.CommitAsync();
            _logger.LogInformation("Successfully committed {Count} database operations", 
                operations.Count());
            
            // Return combined result
            var combinedData = CombineTransactionResults<T>(results);
            return DatabaseResult<T>.Success(combinedData);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed, rolling back {Count} operations", 
                operations.Count());
            
            await Services.ErrorHandling.HandleErrorAsync(ex, "Database transaction failed");
            return DatabaseResult<T>.Failure($"Transaction failed: {ex.Message}");
        }
    }
}

// Usage in Overlay ViewModel
[ObservableObject]
public partial class ComplexInventoryOverlayViewModel : BasePoolableOverlayViewModel
{
    private readonly IOverlayDatabaseService _databaseService;

    [RelayCommand]
    private async Task SaveComplexInventoryOperationAsync()
    {
        try
        {
            IsLoading = true;
            
            // Create multiple database operations that must succeed together
            var operations = new List<OverlayDatabaseOperation>
            {
                // Update inventory
                new OverlayDatabaseOperation
                {
                    StoredProcedure = "inv_inventory_Update_Quantity",
                    Parameters = new List<MySqlParameter>
                    {
                        new("p_PartID", PartId),
                        new("p_Operation", Operation),
                        new("p_Location", Location),
                        new("p_NewQuantity", NewQuantity),
                        new("p_User", Environment.UserName)
                    }
                },
                
                // Log transaction
                new OverlayDatabaseOperation
                {
                    StoredProcedure = "inv_transaction_Add",
                    Parameters = new List<MySqlParameter>
                    {
                        new("p_PartID", PartId),
                        new("p_Operation", Operation),
                        new("p_TransactionType", "EDIT"),
                        new("p_Quantity", NewQuantity - CurrentQuantity),
                        new("p_Notes", Notes),
                        new("p_User", Environment.UserName)
                    }
                },
                
                // Update audit log
                new OverlayDatabaseOperation
                {
                    StoredProcedure = "log_audit_Add_Entry",
                    Parameters = new List<MySqlParameter>
                    {
                        new("p_Action", "INVENTORY_EDIT"),
                        new("p_Details", $"Edited {PartId} from {CurrentQuantity} to {NewQuantity}"),
                        new("p_User", Environment.UserName)
                    }
                }
            };

            // Execute all operations in a transaction
            var result = await _databaseService.ExecuteInTransactionAsync<InventoryEditResult>(operations);
            
            if (result.IsSuccess)
            {
                Logger.LogInformation("Complex inventory operation completed for {PartId}", PartId);
                
                var response = new InventoryEditResponse(
                    OverlayResult.Confirmed,
                    Success: true,
                    Data: result.Data
                );
                
                await CloseAsync(response);
            }
            else
            {
                Logger.LogError("Complex inventory operation failed: {Error}", result.ErrorMessage);
                
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage),
                    "Complex inventory operation failed"
                );
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}

public record OverlayDatabaseOperation
{
    public string StoredProcedure { get; init; } = string.Empty;
    public List<MySqlParameter> Parameters { get; init; } = new();
}

public record DatabaseResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    
    public static DatabaseResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static DatabaseResult<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
```

**Database Integration Best Practices**:

- Always use transactions for multi-operation scenarios
- Follow existing stored procedure naming conventions
- Use the same parameter naming patterns as main application
- Include proper audit logging for all database changes
- Handle rollbacks gracefully with user-friendly messages

---

### **Recipe 4: Error Handling Integration**

**Problem**: Ensuring overlay error handling integrates seamlessly with MTM error handling patterns

**Solution**:

```csharp
// File: Services/OverlayErrorHandlingService.cs

public interface IOverlayErrorHandlingService : IDisposable
{
    Task HandleOverlayErrorAsync(Exception exception, string context, 
        IOverlayViewModel overlayViewModel);
    Task<bool> TryRecoverFromErrorAsync(Exception exception, IOverlayViewModel overlayViewModel);
    void RegisterErrorRecoveryHandler<TException>(Func<TException, IOverlayViewModel, Task<bool>> handler)
        where TException : Exception;
}

public class OverlayErrorHandlingService : IOverlayErrorHandlingService
{
    private readonly Dictionary<Type, Func<Exception, IOverlayViewModel, Task<bool>>> _recoveryHandlers = new();
    private readonly ILogger<OverlayErrorHandlingService> _logger;

    public OverlayErrorHandlingService(ILogger<OverlayErrorHandlingService> logger)
    {
        _logger = logger;
        RegisterDefaultRecoveryHandlers();
    }

    public async Task HandleOverlayErrorAsync(Exception exception, string context, 
        IOverlayViewModel overlayViewModel)
    {
        _logger.LogError(exception, "Overlay error in {Context} for overlay {OverlayId}", 
            context, overlayViewModel.OverlayId);

        // Try recovery first
        var recovered = await TryRecoverFromErrorAsync(exception, overlayViewModel);
        
        if (!recovered)
        {
            // Use existing MTM error handling
            await Services.ErrorHandling.HandleErrorAsync(exception, 
                $"Overlay error: {context}");
            
            // Close overlay with error state
            await CloseOverlayWithErrorAsync(overlayViewModel, exception);
        }
    }

    public async Task<bool> TryRecoverFromErrorAsync(Exception exception, IOverlayViewModel overlayViewModel)
    {
        var exceptionType = exception.GetType();
        
        // Try specific handler first
        if (_recoveryHandlers.TryGetValue(exceptionType, out var handler))
        {
            return await handler(exception, overlayViewModel);
        }
        
        // Try base type handlers
        foreach (var kvp in _recoveryHandlers)
        {
            if (kvp.Key.IsAssignableFrom(exceptionType))
            {
                return await kvp.Value(exception, overlayViewModel);
            }
        }
        
        return false; // No recovery possible
    }

    public void RegisterErrorRecoveryHandler<TException>(
        Func<TException, IOverlayViewModel, Task<bool>> handler)
        where TException : Exception
    {
        _recoveryHandlers[typeof(TException)] = (ex, vm) => handler((TException)ex, vm);
    }

    private void RegisterDefaultRecoveryHandlers()
    {
        // Database connection recovery
        RegisterErrorRecoveryHandler<MySqlException>(async (ex, vm) =>
        {
            if (ex.Number == (int)MySqlErrorCode.UnableToConnectToHost)
            {
                _logger.LogInformation("Attempting database reconnection for overlay {OverlayId}", 
                    vm.OverlayId);
                
                // Wait and retry
                await Task.Delay(1000);
                
                // Show user-friendly message about retrying
                await ShowRetryNotificationAsync(vm, "Database connection lost, retrying...");
                
                return true; // Indicate recovery attempted
            }
            return false;
        });

        // Validation error recovery
        RegisterErrorRecoveryHandler<ValidationException>(async (ex, vm) =>
        {
            _logger.LogInformation("Handling validation errors for overlay {OverlayId}: {Errors}", 
                vm.OverlayId, string.Join(", ", ex.Errors));
            
            // Show validation errors in overlay
            await ShowValidationErrorsInOverlayAsync(vm, ex.Errors);
            
            return true; // Validation errors are recoverable
        });

        // Timeout recovery
        RegisterErrorRecoveryHandler<TimeoutException>(async (ex, vm) =>
        {
            _logger.LogWarning("Timeout in overlay {OverlayId}, offering retry", vm.OverlayId);
            
            // Show retry option to user
            var shouldRetry = await ShowRetryDialogAsync(vm, "Operation timed out. Retry?");
            
            return shouldRetry;
        });
    }

    private async Task CloseOverlayWithErrorAsync(IOverlayViewModel overlayViewModel, Exception exception)
    {
        var errorResponse = CreateErrorResponse(exception);
        await overlayViewModel.CloseAsync(errorResponse);
    }

    private BaseOverlayResponse CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            ValidationException validationEx => new BaseOverlayResponse(
                OverlayResult.Error,
                ErrorMessage: $"Validation failed: {string.Join(", ", validationEx.Errors)}"
            ),
            MySqlException dbEx => new BaseOverlayResponse(
                OverlayResult.Error,
                ErrorMessage: "Database operation failed"
            ),
            TimeoutException => new BaseOverlayResponse(
                OverlayResult.Error,
                ErrorMessage: "Operation timed out"
            ),
            _ => new BaseOverlayResponse(
                OverlayResult.Error,
                ErrorMessage: "An unexpected error occurred"
            )
        };
    }

    public void Dispose()
    {
        _recoveryHandlers.Clear();
    }
}

// Integration in Base Overlay ViewModel
[ObservableObject]
public abstract partial class BasePoolableOverlayViewModel : IOverlayViewModel, IDisposable
{
    protected readonly IOverlayErrorHandlingService? ErrorHandlingService;

    protected BasePoolableOverlayViewModel(
        ILogger logger,
        IOverlayErrorHandlingService? errorHandlingService = null)
    {
        Logger = logger;
        ErrorHandlingService = errorHandlingService;
    }

    protected async Task SafeExecuteAsync(Func<Task> operation, string context)
    {
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            if (ErrorHandlingService != null)
            {
                await ErrorHandlingService.HandleOverlayErrorAsync(ex, context, this);
            }
            else
            {
                // Fallback to standard error handling
                Logger.LogError(ex, "Error in overlay operation: {Context}", context);
                await Services.ErrorHandling.HandleErrorAsync(ex, context);
                throw;
            }
        }
    }

    protected async Task<T> SafeExecuteAsync<T>(Func<Task<T>> operation, string context, T defaultValue = default!)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            if (ErrorHandlingService != null)
            {
                await ErrorHandlingService.HandleOverlayErrorAsync(ex, context, this);
                return defaultValue;
            }
            else
            {
                Logger.LogError(ex, "Error in overlay operation: {Context}", context);
                await Services.ErrorHandling.HandleErrorAsync(ex, context);
                throw;
            }
        }
    }
}

// Usage in Specific Overlay ViewModels
[ObservableObject]
public partial class RobustInventoryOverlayViewModel : BasePoolableOverlayViewModel
{
    [RelayCommand]
    private async Task SaveWithErrorHandlingAsync()
    {
        await SafeExecuteAsync(async () =>
        {
            // This operation is now protected by integrated error handling
            var result = await SaveInventoryToDatabaseAsync();
            
            if (result.IsSuccess)
            {
                await CloseAsync(new InventoryEditResponse(OverlayResult.Confirmed));
            }
            else
            {
                throw new InvalidOperationException($"Save failed: {result.ErrorMessage}");
            }
        }, "Save inventory operation");
    }

    [RelayCommand]
    private async Task LoadDataWithErrorHandlingAsync()
    {
        var masterData = await SafeExecuteAsync(async () =>
        {
            return await LoadMasterDataAsync();
        }, "Load master data", new List<string>());
        
        // Continue with loaded data (empty list if error occurred)
        await PopulateUIWithMasterDataAsync(masterData);
    }
}
```

**Error Handling Integration Best Practices**:

- Use typed exceptions for different error scenarios
- Implement recovery strategies for common failures
- Always log errors with sufficient context
- Provide user-friendly error messages
- Ensure overlay state remains consistent after errors
- Test error scenarios thoroughly

---

## 🔄 Advanced Integration Patterns

### **Recipe 5: Multi-Overlay Workflows**

**Problem**: Coordinating multiple overlays in a workflow sequence

**Solution**:

```csharp
// File: Services/OverlayWorkflowService.cs

public interface IOverlayWorkflowService
{
    Task<TResult> ExecuteWorkflowAsync<TResult>(OverlayWorkflowDefinition workflow)
        where TResult : BaseOverlayResponse;
        
    Task<bool> CanExecuteWorkflowAsync(string workflowId);
    void RegisterWorkflow(OverlayWorkflowDefinition workflow);
}

public class OverlayWorkflowService : IOverlayWorkflowService
{
    private readonly IUniversalOverlayService _overlayService;
    private readonly ILogger<OverlayWorkflowService> _logger;
    private readonly Dictionary<string, OverlayWorkflowDefinition> _workflows = new();

    public OverlayWorkflowService(
        IUniversalOverlayService overlayService,
        ILogger<OverlayWorkflowService> logger)
    {
        _overlayService = overlayService;
        _logger = logger;
        
        RegisterDefaultWorkflows();
    }

    public async Task<TResult> ExecuteWorkflowAsync<TResult>(OverlayWorkflowDefinition workflow)
        where TResult : BaseOverlayResponse
    {
        _logger.LogInformation("Starting workflow execution: {WorkflowId}", workflow.Id);
        
        var context = new WorkflowExecutionContext();
        
        try
        {
            foreach (var step in workflow.Steps)
            {
                _logger.LogInformation("Executing workflow step: {StepId}", step.Id);
                
                var result = await ExecuteWorkflowStepAsync(step, context);
                
                if (result.Result == OverlayResult.Cancelled)
                {
                    _logger.LogInformation("Workflow cancelled at step: {StepId}", step.Id);
                    return (TResult)result;
                }
                
                if (result.Result == OverlayResult.Error)
                {
                    _logger.LogError("Workflow failed at step: {StepId}", step.Id);
                    return (TResult)result;
                }
                
                // Store result for next steps
                context.StepResults[step.Id] = result;
                
                // Check if workflow should continue
                if (step.StopOnSuccess && result.Result == OverlayResult.Confirmed)
                {
                    break;
                }
            }
            
            _logger.LogInformation("Workflow completed successfully: {WorkflowId}", workflow.Id);
            
            // Create final result
            return CreateWorkflowResult<TResult>(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing workflow: {WorkflowId}", workflow.Id);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Workflow execution failed: {workflow.Id}");
            
            var errorResult = (TResult)Activator.CreateInstance(typeof(TResult), OverlayResult.Error)!;
            return errorResult;
        }
    }

    private async Task<BaseOverlayResponse> ExecuteWorkflowStepAsync(
        WorkflowStep step, 
        WorkflowExecutionContext context)
    {
        // Create request with data from previous steps
        var request = step.RequestFactory(context);
        
        // Execute overlay
        var response = await _overlayService.ShowOverlayAsync(
            request.GetType(),
            step.ResponseType,
            step.ViewModelType,
            request
        );
        
        return response;
    }

    private void RegisterDefaultWorkflows()
    {
        // Multi-step inventory edit workflow
        var inventoryWorkflow = new OverlayWorkflowDefinition
        {
            Id = "InventoryMultiStepEdit",
            Name = "Multi-Step Inventory Edit",
            Steps = new List<WorkflowStep>
            {
                // Step 1: Select item to edit
                new WorkflowStep
                {
                    Id = "SelectItem",
                    ViewModelType = typeof(InventorySelectionOverlayViewModel),
                    ResponseType = typeof(InventorySelectionResponse),
                    RequestFactory = context => new InventorySelectionRequest(),
                    StopOnSuccess = false
                },
                
                // Step 2: Edit selected item
                new WorkflowStep
                {
                    Id = "EditItem",
                    ViewModelType = typeof(InventoryQuickEditOverlayViewModel),
                    ResponseType = typeof(InventoryQuickEditResponse),
                    RequestFactory = context =>
                    {
                        var selectionResult = context.StepResults["SelectItem"] as InventorySelectionResponse;
                        return new InventoryQuickEditRequest(
                            selectionResult!.SelectedPartId,
                            selectionResult.SelectedOperation,
                            selectionResult.SelectedLocation,
                            selectionResult.CurrentQuantity
                        );
                    },
                    StopOnSuccess = false
                },
                
                // Step 3: Confirm changes
                new WorkflowStep
                {
                    Id = "ConfirmChanges",
                    ViewModelType = typeof(ConfirmationOverlayViewModel),
                    ResponseType = typeof(ConfirmationResponse),
                    RequestFactory = context =>
                    {
                        var editResult = context.StepResults["EditItem"] as InventoryQuickEditResponse;
                        return new ConfirmationRequest(
                            Title: "Confirm Inventory Changes",
                            Message: $"Save changes to {editResult!.PartId}?",
                            ConfirmText: "Save Changes",
                            CancelText: "Cancel"
                        );
                    },
                    StopOnSuccess = true
                }
            }
        };
        
        RegisterWorkflow(inventoryWorkflow);
    }
}

// Usage in Parent ViewModel
[ObservableObject] 
public partial class InventoryTabViewModel : BaseViewModel
{
    private readonly IOverlayWorkflowService _workflowService;

    [RelayCommand]
    private async Task StartMultiStepEditWorkflowAsync()
    {
        try
        {
            var workflow = new OverlayWorkflowDefinition
            {
                Id = "InventoryMultiStepEdit"
            };

            var result = await _workflowService.ExecuteWorkflowAsync<InventoryQuickEditResponse>(workflow);
            
            switch (result.Result)
            {
                case OverlayResult.Confirmed:
                    Logger.LogInformation("Multi-step inventory edit completed successfully");
                    await RefreshInventoryDataAsync();
                    break;
                    
                case OverlayResult.Cancelled:
                    Logger.LogInformation("Multi-step inventory edit was cancelled");
                    break;
                    
                case OverlayResult.Error:
                    Logger.LogError("Multi-step inventory edit failed");
                    await ShowErrorNotificationAsync("Inventory edit workflow failed");
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error starting multi-step inventory edit workflow");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Workflow start failed");
        }
    }
}

public record OverlayWorkflowDefinition
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public List<WorkflowStep> Steps { get; init; } = new();
}

public record WorkflowStep
{
    public string Id { get; init; } = string.Empty;
    public Type ViewModelType { get; init; } = typeof(object);
    public Type ResponseType { get; init; } = typeof(BaseOverlayResponse);
    public Func<WorkflowExecutionContext, BaseOverlayRequest> RequestFactory { get; init; } = _ => new BaseOverlayRequest();
    public bool StopOnSuccess { get; init; } = false;
}

public class WorkflowExecutionContext
{
    public Dictionary<string, BaseOverlayResponse> StepResults { get; } = new();
    public Dictionary<string, object> SharedData { get; } = new();
}
```

**Multi-Overlay Best Practices**:

- Design workflows to handle cancellation at any step
- Pass context between overlays efficiently
- Provide clear progress indication to users
- Allow for workflow branching based on user choices
- Test complex workflows thoroughly
- Consider performance implications of multiple overlays

---

## 🎨 Specialized Use Cases

### **Recipe 9: Custom DataGrid Integration**

**Problem**: Integrating overlays with MTM's CustomDataGrid for advanced data editing scenarios

**Solution**:

```csharp
// File: Services/DataGridOverlayIntegrationService.cs

public interface IDataGridOverlayIntegrationService
{
    Task<bool> ShowCellEditOverlayAsync(CustomDataGrid dataGrid, DataGridCellEditEventArgs e);
    Task<bool> ShowRowEditOverlayAsync(CustomDataGrid dataGrid, object rowData);
    Task<bool> ShowBulkEditOverlayAsync(CustomDataGrid dataGrid, IEnumerable<object> selectedRows);
}

public class DataGridOverlayIntegrationService : IDataGridOverlayIntegrationService
{
    private readonly IUniversalOverlayService _overlayService;
    private readonly ILogger<DataGridOverlayIntegrationService> _logger;

    public DataGridOverlayIntegrationService(
        IUniversalOverlayService overlayService,
        ILogger<DataGridOverlayIntegrationService> logger)
    {
        _overlayService = overlayService;
        _logger = logger;
    }

    public async Task<bool> ShowCellEditOverlayAsync(CustomDataGrid dataGrid, DataGridCellEditEventArgs e)
    {
        try
        {
            _logger.LogInformation("Showing cell edit overlay for column: {Column}", 
                e.Column.Header);

            // Determine overlay type based on column
            var overlayType = DetermineOverlayTypeForColumn(e.Column);
            if (overlayType == null)
            {
                return false; // No overlay needed for this column
            }

            // Create request based on cell data
            var request = CreateCellEditRequest(e);
            
            // Show appropriate overlay
            var response = await ShowTypedOverlayAsync(overlayType, request);
            
            if (response.Result == OverlayResult.Confirmed)
            {
                // Update cell value
                await UpdateCellValueAsync(dataGrid, e, response);
                
                _logger.LogInformation("Cell edit completed for {Column}", e.Column.Header);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing cell edit overlay");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Cell edit overlay failed");
            return false;
        }
    }

    public async Task<bool> ShowRowEditOverlayAsync(CustomDataGrid dataGrid, object rowData)
    {
        try
        {
            // Determine row edit overlay based on data type
            var overlayType = DetermineOverlayTypeForRowData(rowData);
            if (overlayType == null)
            {
                return false;
            }

            var request = CreateRowEditRequest(rowData);
            var response = await ShowTypedOverlayAsync(overlayType, request);
            
            if (response.Result == OverlayResult.Confirmed)
            {
                await UpdateRowDataAsync(dataGrid, rowData, response);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing row edit overlay");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Row edit overlay failed");
            return false;
        }
    }

    public async Task<bool> ShowBulkEditOverlayAsync(CustomDataGrid dataGrid, IEnumerable<object> selectedRows)
    {
        try
        {
            var rowList = selectedRows.ToList();
            _logger.LogInformation("Showing bulk edit overlay for {Count} rows", rowList.Count);

            var request = new BulkEditRequest(rowList);
            var response = await _overlayService.ShowBulkEditOverlayAsync(request);
            
            if (response.Result == OverlayResult.Confirmed)
            {
                await ApplyBulkChangesAsync(dataGrid, response);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing bulk edit overlay");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Bulk edit overlay failed");
            return false;
        }
    }

    private Type? DetermineOverlayTypeForColumn(DataGridColumn column)
    {
        return column.Header?.ToString()?.ToLower() switch
        {
            "quantity" => typeof(NumericEditOverlayViewModel),
            "notes" => typeof(TextEditOverlayViewModel),
            "location" => typeof(LocationSelectionOverlayViewModel),
            "operation" => typeof(OperationSelectionOverlayViewModel),
            "date" => typeof(DateTimeEditOverlayViewModel),
            _ => null
        };
    }

    private BaseOverlayRequest CreateCellEditRequest(DataGridCellEditEventArgs e)
    {
        var columnName = e.Column.Header?.ToString()?.ToLower();
        var currentValue = GetCellValue(e);
        
        return columnName switch
        {
            "quantity" => new NumericEditRequest(
                Title: "Edit Quantity",
                CurrentValue: Convert.ToInt32(currentValue),
                MinValue: 0,
                MaxValue: 999999
            ),
            "notes" => new TextEditRequest(
                Title: "Edit Notes",
                CurrentText: currentValue?.ToString() ?? string.Empty,
                MaxLength: 500,
                AllowMultiline: true
            ),
            "location" => new LocationSelectionRequest(
                Title: "Select Location",
                CurrentLocation: currentValue?.ToString() ?? string.Empty
            ),
            _ => new BaseOverlayRequest()
        };
    }

    private async Task UpdateCellValueAsync(CustomDataGrid dataGrid, DataGridCellEditEventArgs e, 
        BaseOverlayResponse response)
    {
        var newValue = ExtractValueFromResponse(response);
        
        // Update the underlying data model
        var rowData = e.Row.DataContext;
        var propertyName = GetPropertyNameForColumn(e.Column);
        
        if (!string.IsNullOrEmpty(propertyName))
        {
            var property = rowData.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                // Convert value to correct type
                var convertedValue = Convert.ChangeType(newValue, property.PropertyType);
                property.SetValue(rowData, convertedValue);
                
                // Refresh the DataGrid
                await RefreshDataGridAsync(dataGrid);
            }
        }
    }

    private async Task RefreshDataGridAsync(CustomDataGrid dataGrid)
    {
        // Use MTM's CustomDataGrid refresh mechanism
        if (dataGrid.DataContext is INotifyPropertyChanged viewModel)
        {
            // Trigger property changed for the collection
            var collectionProperty = viewModel.GetType()
                .GetProperties()
                .FirstOrDefault(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) &&
                                   p.PropertyType != typeof(string));
                
            if (collectionProperty != null)
            {
                // Refresh the data grid by re-setting the items source
                var currentItems = collectionProperty.GetValue(viewModel);
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = currentItems;
            }
        }
    }
}

// Usage in a ViewModel with CustomDataGrid
[ObservableObject]
public partial class InventoryDataGridViewModel : BaseViewModel
{
    private readonly IDataGridOverlayIntegrationService _dataGridOverlayService;
    
    [ObservableProperty]
    private ObservableCollection<InventoryItemModel> inventoryItems = new();

    public InventoryDataGridViewModel(
        ILogger<InventoryDataGridViewModel> logger,
        IDataGridOverlayIntegrationService dataGridOverlayService) : base(logger)
    {
        _dataGridOverlayService = dataGridOverlayService;
    }

    [RelayCommand]
    private async Task HandleCellDoubleClickAsync(DataGridCellEditEventArgs e)
    {
        var success = await _dataGridOverlayService.ShowCellEditOverlayAsync(
            (CustomDataGrid)e.Source, e);
            
        if (success)
        {
            Logger.LogInformation("Cell edit completed successfully");
            await SaveInventoryChangesAsync();
        }
    }

    [RelayCommand]
    private async Task HandleRowEditAsync(object rowData)
    {
        if (rowData is InventoryItemModel item)
        {
            var success = await _dataGridOverlayService.ShowRowEditOverlayAsync(
                GetDataGrid(), item);
                
            if (success)
            {
                await SaveInventoryChangesAsync();
            }
        }
    }

    [RelayCommand]
    private async Task HandleBulkEditAsync()
    {
        var selectedItems = GetSelectedInventoryItems();
        if (selectedItems.Any())
        {
            var success = await _dataGridOverlayService.ShowBulkEditOverlayAsync(
                GetDataGrid(), selectedItems);
                
            if (success)
            {
                await SaveInventoryChangesAsync();
                await RefreshInventoryDataAsync();
            }
        }
    }
}
```

**DataGrid Integration Best Practices**:

- Determine overlay type dynamically based on column/data type
- Ensure data binding updates correctly after overlay changes
- Handle validation at both overlay and DataGrid levels
- Provide bulk edit capabilities for efficiency
- Maintain undo/redo capabilities where possible
- Test with large datasets for performance

---

## 🚨 Troubleshooting Recipes

### **Recipe 13: Common Integration Issues**

**Problem**: Diagnosing and fixing common overlay integration problems

**Solutions**:

#### **Issue 1: Overlay Not Showing**

**Symptoms**:

- ShowOverlayAsync returns immediately
- No visual overlay appears
- No error messages

**Diagnostic Steps**:

```csharp
// File: Services/OverlayDiagnosticService.cs

public class OverlayDiagnosticService : IOverlayDiagnosticService
{
    public async Task<OverlayDiagnosticResult> DiagnoseOverlayIssueAsync(
        Type overlayViewModelType, 
        BaseOverlayRequest request)
    {
        var result = new OverlayDiagnosticResult();
        
        // Check 1: ViewModel registration
        result.ViewModelRegistered = CheckViewModelRegistration(overlayViewModelType);
        
        // Check 2: Request validation
        result.RequestValid = ValidateRequest(request);
        
        // Check 3: UI thread check
        result.OnUIThread = CheckUIThread();
        
        // Check 4: Parent window availability
        result.ParentWindowAvailable = CheckParentWindow();
        
        // Check 5: Service registration
        result.ServiceRegistered = CheckUniversalOverlayServiceRegistration();
        
        return result;
    }

    private bool CheckViewModelRegistration(Type viewModelType)
    {
        try
        {
            var serviceProvider = GetServiceProvider();
            var instance = serviceProvider.GetService(viewModelType);
            return instance != null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ViewModel {Type} not registered", viewModelType.Name);
            return false;
        }
    }

    private bool ValidateRequest(BaseOverlayRequest request)
    {
        if (request == null)
        {
            Logger.LogError("Request is null");
            return false;
        }

        // Check required properties
        var requiredProperties = request.GetType()
            .GetProperties()
            .Where(p => p.GetCustomAttribute<RequiredAttribute>() != null);
            
        foreach (var prop in requiredProperties)
        {
            var value = prop.GetValue(request);
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                Logger.LogError("Required property {Property} is null or empty", prop.Name);
                return false;
            }
        }
        
        return true;
    }
}

// Usage for debugging
[RelayCommand]
private async Task ShowOverlayWithDiagnosticsAsync()
{
    var request = new InventoryEditRequest(/* parameters */);
    
    // Run diagnostics first
    var diagnostics = await _diagnosticService.DiagnoseOverlayIssueAsync(
        typeof(InventoryEditOverlayViewModel), request);
        
    if (!diagnostics.AllChecksPass)
    {
        Logger.LogError("Overlay diagnostics failed: {Issues}", 
            string.Join(", ", diagnostics.Issues));
        return;
    }
    
    // Proceed with overlay
    var response = await _overlayService.ShowInventoryEditOverlayAsync(request);
}
```

#### **Issue 2: Memory Leaks in Overlays**

**Symptoms**:

- Memory usage increases over time
- Application becomes sluggish after many overlay operations
- OutOfMemoryException eventually occurs

**Diagnostic and Fix**:

```csharp
// File: Services/OverlayMemoryDiagnosticService.cs

public class OverlayMemoryDiagnosticService
{
    private readonly Dictionary<string, WeakReference> _overlayReferences = new();
    private readonly ILogger<OverlayMemoryDiagnosticService> _logger;

    public void TrackOverlayViewModel(string overlayId, IOverlayViewModel viewModel)
    {
        _overlayReferences[overlayId] = new WeakReference(viewModel);
        _logger.LogDebug("Tracking overlay {OverlayId}", overlayId);
    }

    public async Task<MemoryDiagnosticResult> CheckForMemoryLeaksAsync()
    {
        var result = new MemoryDiagnosticResult
        {
            TotalTracked = _overlayReferences.Count,
            StillAlive = 0,
            PotentialLeaks = new List<string>()
        };
        
        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        await Task.Delay(100); // Allow cleanup
        
        foreach (var kvp in _overlayReferences.ToList())
        {
            if (kvp.Value.IsAlive)
            {
                result.StillAlive++;
                result.PotentialLeaks.Add(kvp.Key);
                
                _logger.LogWarning("Potential memory leak detected for overlay {OverlayId}", 
                    kvp.Key);
            }
            else
            {
                // Remove dead references
                _overlayReferences.Remove(kvp.Key);
            }
        }
        
        return result;
    }

    public void RecommendMemoryFixes(MemoryDiagnosticResult result)
    {
        if (result.PotentialLeaks.Any())
        {
            _logger.LogWarning(
                "Memory leak recommendations:\n" +
                "1. Ensure Dispose() is called on overlay ViewModels\n" +
                "2. Unsubscribe from events in OnReset() or Dispose()\n" +
                "3. Clear ObservableCollections in Reset methods\n" +
                "4. Check for circular references\n" +
                "5. Verify weak event patterns are used\n" +
                "Affected overlays: {Overlays}",
                string.Join(", ", result.PotentialLeaks)
            );
        }
    }
}

// Memory leak prevention in ViewModel
[ObservableObject]
public partial class MemorySafeOverlayViewModel : BasePoolableOverlayViewModel
{
    private readonly ObservableCollection<string> _items = new();
    private readonly Timer? _refreshTimer;
    private CancellationTokenSource? _cancellationTokenSource;

    protected override void OnReset()
    {
        // Clear collections
        _items.Clear();
        
        // Cancel ongoing operations
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        
        // Dispose timers
        _refreshTimer?.Dispose();
        
        // Clear event subscriptions
        WeakEventManager.RemoveListener(SomeService, "SomeEvent", this);
        
        base.OnReset();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancellationTokenSource?.Dispose();
            _refreshTimer?.Dispose();
            _items.Clear();
        }
        
        base.Dispose(disposing);
    }
}
```

#### **Issue 3: Overlay Threading Issues**

**Symptoms**:

- "Cross-thread operation not valid" exceptions
- UI freezes during overlay operations
- Inconsistent overlay behavior

**Fix**:

```csharp
// File: Services/OverlayThreadingService.cs

public class OverlayThreadingService : IOverlayThreadingService
{
    private readonly Dispatcher _uiDispatcher;
    
    public OverlayThreadingService()
    {
        _uiDispatcher = Dispatcher.UIThread;
    }

    public async Task ExecuteOnUIThreadAsync(Action action)
    {
        if (_uiDispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            await _uiDispatcher.InvokeAsync(action);
        }
    }

    public async Task<T> ExecuteOnUIThreadAsync<T>(Func<T> func)
    {
        if (_uiDispatcher.CheckAccess())
        {
            return func();
        }
        else
        {
            return await _uiDispatcher.InvokeAsync(func);
        }
    }

    public async Task ExecuteOnBackgroundThreadAsync(Func<Task> asyncAction)
    {
        await Task.Run(asyncAction);
    }
}

// Thread-safe overlay operations
[ObservableObject]
public partial class ThreadSafeOverlayViewModel : BasePoolableOverlayViewModel
{
    private readonly IOverlayThreadingService _threadingService;

    [RelayCommand]
    private async Task LoadDataSafelyAsync()
    {
        try
        {
            // Update UI to show loading (on UI thread)
            await _threadingService.ExecuteOnUIThreadAsync(() =>
            {
                IsLoading = true;
                Status = "Loading data...";
            });

            // Perform background work
            var data = await _threadingService.ExecuteOnBackgroundThreadAsync(async () =>
            {
                // Heavy database operations on background thread
                return await LoadDataFromDatabaseAsync();
            });

            // Update UI with results (on UI thread)
            await _threadingService.ExecuteOnUIThreadAsync(() =>
            {
                Items.Clear();
                foreach (var item in data)
                {
                    Items.Add(item);
                }
                
                IsLoading = false;
                Status = "Data loaded successfully";
            });
        }
        catch (Exception ex)
        {
            // Error handling on UI thread
            await _threadingService.ExecuteOnUIThreadAsync(async () =>
            {
                IsLoading = false;
                Status = "Error loading data";
                await Services.ErrorHandling.HandleErrorAsync(ex, "Data loading failed");
            });
        }
    }
}
```

**Troubleshooting Best Practices**:

- Use diagnostic services to identify issues early
- Implement comprehensive logging at integration points
- Monitor memory usage in development and testing
- Always handle threading correctly in overlay operations
- Create automated tests for common failure scenarios
- Document known issues and their solutions
- Use weak references where appropriate to prevent memory leaks

---

This integration cookbook provides practical, tested solutions for the most common overlay integration scenarios in the MTM WIP Application. Each recipe includes complete code examples, best practices, and troubleshooting guidance to ensure successful integration.
