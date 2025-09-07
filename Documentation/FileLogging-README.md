# MTM File Logging Service

The MTM File Logging Service provides structured logging to both network and local file locations with automatic categorization of business logs and MySQL error logs.

## Features

- **Dual Location Logging**: Saves logs to both network drive and local fallback
- **Automatic Categorization**: Separates business logs from MySQL/database logs
- **CSV Format**: Easy to read and analyze in Excel or other tools
- **Queue-Based**: Non-blocking logging with configurable flush intervals
- **Smart Fallback**: Automatically switches to local storage if network is unavailable

## Log File Locations

### Network Location (Primary)
```
\\mtmanu-fs01\Expo Drive\MH_RESOURCE\Material_Handler\MTM WIP App\Logs\[USERNAME]\
├── BusinessLog.csv      # Application business logic logs
├── MySQLErrors.csv      # Database operations and errors
└── SystemLog.csv        # System-level messages (network failures, etc.)
```

### Local Fallback
```
%AppData%\MTM_WIP_Application\Logs\[USERNAME]\
├── BusinessLog.csv
├── MySQLErrors.csv
└── SystemLog.csv
```

## Configuration

Update `appsettings.json` to configure logging behavior:

```json
{
  "Logging": {
    "File": {
      "BasePath": "\\\\mtmanu-fs01\\Expo Drive\\MH_RESOURCE\\Material_Handler\\MTM WIP App\\Logs",
      "LocalBasePath": "%AppData%\\MTM_WIP_Application\\Logs",
      "EnableDualLocationLogging": true,
      "FlushIntervalSeconds": 10,
      "MaxFileSizeMB": 50,
      "RetainDays": 30
    }
  },
  "ErrorHandling": {
    "FileServerPath": "\\\\mtmanu-fs01\\Expo Drive\\MH_RESOURCE\\Material_Handler\\MTM WIP App\\Logs"
  }
}
```

### Configuration Options

- **BasePath**: Primary (network) logging location  
- **LocalBasePath**: Local logging location (supports environment variables like %AppData%)
- **EnableDualLocationLogging**: 
  - `true`: Writes logs to both network and local locations simultaneously
  - `false`: Uses fallback mode (tries network first, falls back to local on failure)
- **FlushIntervalSeconds**: How often to write queued logs to files (default: 10)
- **MaxFileSizeMB**: Maximum file size before archiving (feature planned)
- **RetainDays**: How long to keep log files (feature planned)

## Log File Locations

### Dual Location Mode (EnableDualLocationLogging: true - Default)
Logs are written to both locations simultaneously:
- **Network**: `\\mtmanu-fs01\Expo Drive\MH_RESOURCE\Material_Handler\MTM WIP App\Logs\{USERNAME}\`
- **Local**: `C:\Users\{USERNAME}\AppData\Roaming\MTM_WIP_Application\Logs\{USERNAME}\`

### Fallback Mode (EnableDualLocationLogging: false)
- Primary: Network location (if accessible)
- Fallback: Local location (if network fails)

In both modes, each location contains the same log files:
```
├── BusinessLog.csv      # Normal application operations
├── MySQLErrors.csv      # Database-related errors and operations  
└── SystemLog.csv        # System events and service operations
```
```

## Automatic Log Categorization

The service automatically routes logs to the appropriate files:

### BusinessLog.csv
- ViewModel operations
- Service calls
- User interactions
- Inventory operations
- Navigation events
- Theme changes
- Configuration updates

### MySQLErrors.csv
- Database connection issues
- Stored procedure execution
- MySQL exceptions
- Transaction operations
- Query timeouts
- Data validation errors

## Usage Examples

### In Services or ViewModels
```csharp
public class InventoryService
{
    private readonly ILogger<InventoryService> _logger;
    
    public InventoryService(ILogger<InventoryService> logger)
    {
        _logger = logger;
    }
    
    public async Task AddInventoryAsync(string partId, int quantity)
    {
        // This will automatically go to BusinessLog.csv
        _logger.LogInformation("Adding inventory: Part {PartId}, Quantity {Quantity}", partId, quantity);
        
        try
        {
            // Database operations are automatically routed to MySQLErrors.csv
            await _databaseService.ExecuteStoredProcedureAsync("inv_inventory_Add_Item", parameters);
        }
        catch (MySqlException ex)
        {
            // MySQL errors automatically go to MySQLErrors.csv
            _logger.LogError(ex, "Failed to add inventory for part {PartId}", partId);
        }
    }
}
```

### Direct Service Usage
```csharp
public class SomeService
{
    private readonly IFileLoggingService _fileLoggingService;
    
    public SomeService(IFileLoggingService fileLoggingService)
    {
        _fileLoggingService = fileLoggingService;
    }
    
    public void DoBusinessOperation()
    {
        // Explicitly log to business log
        _fileLoggingService.LogBusiness(LogLevel.Information, 
            "Business operation completed successfully", 
            "Inventory", 
            new Dictionary<string, object> { ["UserId"] = "JOHNK", ["Operation"] = "ADD" });
    }
    
    public void HandleDatabaseError(Exception ex)
    {
        // Explicitly log to MySQL log
        _fileLoggingService.LogMySql(LogLevel.Error, 
            "Database operation failed", 
            "StoredProcedure", 
            ex,
            new Dictionary<string, object> { ["ProcedureName"] = "inv_inventory_Add_Item" });
    }
}
```

## CSV File Format

### BusinessLog.csv
```csv
Timestamp,Level,Category,SubCategory,UserId,Message,Context,Exception
"2025-09-06 19:16:14.123",Information,BusinessLog,Inventory,JOHNK,"Adding inventory item","{"PartId":"ABC123","Quantity":10}",""
```

### MySQLErrors.csv
```csv
Timestamp,Level,Category,SubCategory,UserId,Message,Context,Exception
"2025-09-06 19:16:14.456",Error,MySQLLog,StoredProcedure,JOHNK,"Failed to execute procedure","{"Procedure":"inv_inventory_Add_Item"}","MySqlException: Timeout expired"
```

## Integration with Existing Code

The service is automatically registered in `ServiceCollectionExtensions.cs` and works with the standard `ILogger<T>` interface. All existing logging code will automatically be categorized and saved to files without any changes required.

## Performance Notes

- Logs are queued in memory and flushed to disk every 10 seconds (configurable)
- Non-blocking logging prevents UI freezes
- Automatic fallback ensures logs are never lost
- CSV format provides excellent performance for large log files

## Troubleshooting

1. **Logs not appearing**: Check network connectivity to `\\mtmanu-fs01`
2. **Only local logs**: Network path might be inaccessible
3. **Missing categories**: Ensure your logger category names include relevant keywords
4. **Performance issues**: Increase flush interval if needed
