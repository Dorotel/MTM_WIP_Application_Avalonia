# Database Integration - MTM WIP Application Instructions

**Database**: MySQL 9.4.0 with MySql.Data  
**Pattern**: Advanced Stored Procedure Integration  
**Created**: 2025-09-14  

---

## 🎯 Advanced Database Integration Patterns

### Connection Pool Management for Manufacturing Workloads

```csharp
// Advanced connection string configuration for manufacturing operations
public class DatabaseConnectionManager : IDatabaseConnectionManager, IDisposable
{
    private readonly string _primaryConnectionString;
    private readonly string _readOnlyConnectionString;
    private readonly ILogger<DatabaseConnectionManager> _logger;
    private readonly ConcurrentDictionary<string, MySqlConnection> _connectionPool;
    private readonly SemaphoreSlim _connectionSemaphore;
    private bool _disposed = false;
    
    public DatabaseConnectionManager(IConfiguration configuration, ILogger<DatabaseConnectionManager> logger)
    {
        _logger = logger;
        _connectionPool = new ConcurrentDictionary<string, MySqlConnection>();
        
        // Manufacturing-optimized connection strings
        _primaryConnectionString = BuildManufacturingConnectionString(
            configuration.GetConnectionString("DefaultConnection"));
        _readOnlyConnectionString = BuildReadOnlyConnectionString(
            configuration.GetConnectionString("ReadOnlyConnection"));
            
        // Limit concurrent connections for manufacturing stability
        _connectionSemaphore = new SemaphoreSlim(50, 50);
    }
    
    private string BuildManufacturingConnectionString(string baseConnectionString)
    {
        var builder = new MySqlConnectionStringBuilder(baseConnectionString)
        {
            // Manufacturing-optimized settings
            Pooling = true,
            MinimumPoolSize = 10,           // Always maintain connections for shift operations
            MaximumPoolSize = 100,          // Handle peak manufacturing loads
            ConnectionLifeTime = 600,       // 10 minutes - balance between freshness and performance
            ConnectionTimeout = 30,         // 30 seconds to establish connection
            DefaultCommandTimeout = 120,    // 2 minutes for complex manufacturing operations
            
            // Performance optimizations
            UseCompression = false,         // Disable for local network performance
            AllowBatch = true,             // Enable batch operations
            RespectBinaryFlags = false,    // Optimize for application usage
            
            // Manufacturing-specific settings
            UseAffectedRows = true,        // Important for transaction verification
            ConvertZeroDateTime = true,    // Handle legacy manufacturing data
            AllowZeroDateTime = true,      // Manufacturing systems may have zero dates
            
            // Reliability settings
            ConnectionReset = false,       // Maintain state for manufacturing sessions
            InteractiveSession = false     // Background service optimization
        };
        
        return builder.ConnectionString;
    }
    
    public async Task<MySqlConnection> GetConnectionAsync(ConnectionType connectionType = ConnectionType.Primary)
    {
        await _connectionSemaphore.WaitAsync();
        
        try
        {
            var connectionString = connectionType == ConnectionType.ReadOnly 
                ? _readOnlyConnectionString 
                : _primaryConnectionString;
                
            var connectionKey = $"{connectionType}_{Thread.CurrentThread.ManagedThreadId}";
            
            if (_connectionPool.TryGetValue(connectionKey, out var existingConnection) 
                && existingConnection.State == ConnectionState.Open)
            {
                return existingConnection;
            }
            
            var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            _connectionPool.AddOrUpdate(connectionKey, connection, (key, old) => 
            {
                old?.Dispose();
                return connection;
            });
            
            _logger.LogDebug("Created new database connection for {ConnectionType}", connectionType);
            return connection;
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var connection in _connectionPool.Values)
            {
                connection?.Dispose();
            }
            _connectionPool.Clear();
            _connectionSemaphore?.Dispose();
            _disposed = true;
        }
    }
}

public enum ConnectionType
{
    Primary,    // Read-write operations
    ReadOnly    // Reporting and analytics
}
```

---

## 📚 Related Documentation

- **Service Integration**: [Cross-Service Communication](./service-integration.instructions.md)
- **MySQL Patterns**: [Basic Database Patterns](./mysql-database-patterns.instructions.md)
- **Configuration Management**: [Application Configuration](./application-configuration.instructions.md)

---

**Document Status**: ✅ Complete Database Integration Reference  
**Database Version**: MySQL 9.4.0  
**Last Updated**: 2025-09-14  
**Database Integration Owner**: MTM Development Team
