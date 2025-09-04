# MTM Service Implementation Template

## ⚙️ Service Implementation Instructions

For creating services in the MTM WIP Application with dependency injection:

### **Service Interface Pattern**
```csharp
namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for [Service] operations
/// </summary>
public interface I[Service]Service
{
    /// <summary>
    /// Execute primary service operation
    /// </summary>
    Task<OperationResult<TResult>> ExecuteAsync<TResult>(ServiceRequest request);
    
    /// <summary>
    /// Get data asynchronously
    /// </summary>
    Task<DatabaseResult<List<TData>>> GetDataAsync<TData>();
}
```

### **Service Implementation Pattern**
```csharp
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Service implementation for [Service] operations
/// </summary>
public class [Service]Service : I[Service]Service, IDisposable
{
    #region Private Fields
    
    private readonly ILogger<[Service]Service> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly string _connectionString;
    private bool _disposed;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// Initializes a new instance of [Service]Service
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="configurationService">Configuration service</param>
    public [Service]Service(
        ILogger<[Service]Service> logger,
        IConfigurationService configurationService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _logger = logger;
        _configurationService = configurationService;
        _connectionString = Helper_Database_Variables.GetConnectionString();
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Execute primary service operation
    /// </summary>
    /// <param name="request">Service request parameters</param>
    /// <returns>Operation result</returns>
    public async Task<OperationResult<TResult>> ExecuteAsync<TResult>(ServiceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        try
        {
            _logger.LogInformation("Executing service operation for {Operation}", request.OperationType);
            
            // Validate request
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return OperationResult<TResult>.Failed(validationResult.ErrorMessage);
            }

            // Execute database operation
            var databaseResult = await ExecuteDatabaseOperationAsync(request);
            if (!databaseResult.IsSuccess)
            {
                return OperationResult<TResult>.Failed(databaseResult.ErrorMessage);
            }

            // Process result
            var processedResult = ProcessDatabaseResult<TResult>(databaseResult.Data);
            
            _logger.LogInformation("Service operation completed successfully");
            return OperationResult<TResult>.Success(processedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing service operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Service operation");
            return OperationResult<TResult>.Failed($"Service error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get data asynchronously
    /// </summary>
    /// <returns>Database result with data</returns>
    public async Task<DatabaseResult<List<TData>>> GetDataAsync<TData>()
    {
        try
        {
            var parameters = Array.Empty<MySqlParameter>();
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "[service]_Get_Data", // Replace with actual stored procedure name
                parameters
            );

            if (result.Status == 1)
            {
                var data = ConvertDataTableToList<TData>(result.Data);
                return DatabaseResult<List<TData>>.Success(data);
            }
            
            _logger.LogWarning("Get data operation returned non-success status: {Status}", result.Status);
            return DatabaseResult<List<TData>>.Failed("Failed to retrieve data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Get data operation");
            return DatabaseResult<List<TData>>.Failed($"Data retrieval error: {ex.Message}");
        }
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Validate service request
    /// </summary>
    /// <param name="request">Request to validate</param>
    /// <returns>Validation result</returns>
    private OperationResult ValidateRequest(ServiceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RequiredProperty))
        {
            return OperationResult.Failed("Required property is missing");
        }

        if (request.NumericProperty <= 0)
        {
            return OperationResult.Failed("Numeric property must be positive");
        }

        return OperationResult.Success();
    }

    /// <summary>
    /// Execute database operation for service
    /// </summary>
    /// <param name="request">Service request</param>
    /// <returns>Database operation result</returns>
    private async Task<DatabaseResult<DataTable>> ExecuteDatabaseOperationAsync(ServiceRequest request)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_Property1", request.RequiredProperty),
            new("p_Property2", request.NumericProperty),
            new("p_User", _configurationService.GetCurrentUser()),
            new("p_Timestamp", DateTime.UtcNow)
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "[service]_Execute_Operation", // Replace with actual stored procedure name
            parameters
        );

        if (result.Status == 1)
        {
            return DatabaseResult<DataTable>.Success(result.Data);
        }
        
        return DatabaseResult<DataTable>.Failed("Database operation failed");
    }

    /// <summary>
    /// Process database result into typed result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="dataTable">Database result</param>
    /// <returns>Processed result</returns>
    private TResult ProcessDatabaseResult<TResult>(DataTable dataTable)
    {
        // Implement specific processing logic based on TResult type
        if (typeof(TResult) == typeof(string))
        {
            var firstValue = dataTable.Rows[0][0]?.ToString() ?? string.Empty;
            return (TResult)(object)firstValue;
        }
        
        if (typeof(TResult) == typeof(int))
        {
            var firstValue = Convert.ToInt32(dataTable.Rows[0][0]);
            return (TResult)(object)firstValue;
        }
        
        // Add more type conversions as needed
        throw new NotSupportedException($"Result type {typeof(TResult)} is not supported");
    }

    /// <summary>
    /// Convert DataTable to typed list
    /// </summary>
    /// <typeparam name="TData">Data type</typeparam>
    /// <param name="dataTable">Source data table</param>
    /// <returns>Typed list</returns>
    private List<TData> ConvertDataTableToList<TData>(DataTable dataTable)
    {
        // Implement conversion logic based on TData type
        // This is a placeholder - implement specific conversion logic
        var result = new List<TData>();
        
        foreach (DataRow row in dataTable.Rows)
        {
            // Convert row to TData instance
            // Implementation depends on specific data type
        }
        
        return result;
    }
    
    #endregion

    #region IDisposable Implementation
    
    /// <summary>
    /// Dispose managed resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose managed and unmanaged resources
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Dispose managed resources
            _disposed = true;
        }
    }
    
    #endregion
}
```

### **Service Registration in DI Container**
```csharp
// In ServiceCollectionExtensions.cs
public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
{
    // Register service with proper lifetime
    services.TryAddScoped<I[Service]Service, [Service]Service>();
    
    return services;
}
```

### **Service Usage in ViewModels**
```csharp
public partial class SomeViewModel : BaseViewModel
{
    private readonly I[Service]Service _service;
    
    public SomeViewModel(
        ILogger<SomeViewModel> logger,
        I[Service]Service service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }
    
    [RelayCommand]
    private async Task ExecuteServiceOperationAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            var request = new ServiceRequest
            {
                RequiredProperty = InputValue,
                NumericProperty = 1
            };
            
            var result = await _service.ExecuteAsync<string>(request);
            
            if (result.IsSuccess)
            {
                // Handle success
                OutputValue = result.Data;
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        });
    }
}
```

### **Service Models**
```csharp
/// <summary>
/// Request model for service operations
/// </summary>
public class ServiceRequest
{
    public string RequiredProperty { get; set; } = string.Empty;
    public int NumericProperty { get; set; }
    public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Generic operation result
/// </summary>
/// <typeparam name="T">Result data type</typeparam>
public class OperationResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static OperationResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static OperationResult<T> Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

/// <summary>
/// Database operation result
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class DatabaseResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static DatabaseResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static DatabaseResult<T> Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
```