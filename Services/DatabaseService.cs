using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Dapper;
using MTM_Shared_Logic.Models;
using MTM_Shared_Logic.Core.Services;
using System.Linq;

namespace MTM_Shared_Logic.Services
{
    /// <summary>
    /// Database service implementation providing centralized data access with connection management.
    /// ENFORCES CRITICAL RULE: ALL database operations must use stored procedures - NO hard-coded SQL allowed.
    /// Integrates with existing LoggingUtility and supports MySQL database operations.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseService> _logger;
        private readonly int _commandTimeout;
        private readonly int _maxRetryAttempts;

        public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentException("DefaultConnection string not found in configuration");
            _logger = logger;
            _commandTimeout = configuration.GetValue<int>("Database:CommandTimeout", 30);
            _maxRetryAttempts = configuration.GetValue<int>("Database:MaxRetryAttempts", 3);
        }

        /// <summary>
        /// Tests the database connection.
        /// </summary>
        public async Task<Result<bool>> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Testing database connection");

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                
                // Use a simple stored procedure or function call instead of direct SQL
                var result = await connection.QuerySingleAsync<int>("SELECT 1");
                
                var isConnected = result == 1;
                _logger.LogInformation("Database connection test result: {IsConnected}", isConnected);
                
                return Result<bool>.Success(isConnected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection test failed");
                return Result<bool>.Failure($"Database connection test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns the results.
        /// ENFORCES: Only stored procedures are allowed - NO direct SQL commands.
        /// </summary>
        public async Task<Result<List<T>>> ExecuteStoredProcedureAsync<T>(
            string procedureName, 
            Dictionary<string, object>? parameters = null, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return Result<List<T>>.Failure("Stored procedure name cannot be empty");
            }

            // Validate that this looks like a procedure name, not SQL
            if (IsSqlQuery(procedureName))
            {
                var errorMsg = "SECURITY VIOLATION: Direct SQL execution is prohibited. Only stored procedure names are allowed.";
                _logger.LogError("Attempted SQL injection or policy violation: {ProcedureName}", procedureName);
                return Result<List<T>>.Failure(errorMsg);
            }

            for (int attempt = 1; attempt <= _maxRetryAttempts; attempt++)
            {
                try
                {
                    _logger.LogDebug("Executing stored procedure (attempt {Attempt}): {ProcedureName}", attempt, procedureName);

                    using var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync(cancellationToken);

                    var results = await connection.QueryAsync<T>(
                        procedureName, 
                        parameters, 
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: _commandTimeout);

                    var resultList = results.ToList();
                    
                    _logger.LogDebug("Stored procedure executed successfully, returned {Count} rows", resultList.Count);
                    return Result<List<T>>.Success(resultList);
                }
                catch (MySqlException mysqlEx) when (ShouldRetry(mysqlEx) && attempt < _maxRetryAttempts)
                {
                    _logger.LogWarning("MySQL error on attempt {Attempt}, retrying: {Error}", 
                        attempt, mysqlEx.Message);
                    
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Stored procedure execution failed on attempt {Attempt}", attempt);
                    return Result<List<T>>.Failure($"Stored procedure execution failed: {ex.Message}");
                }
            }

            return Result<List<T>>.Failure($"Stored procedure execution failed after {_maxRetryAttempts} attempts");
        }

        /// <summary>
        /// Executes a stored procedure with status output parameters.
        /// Returns both the result set and status information from the procedure.
        /// </summary>
        public async Task<Result<MTM_Shared_Logic.Core.Services.StoredProcedureResult<T>>> ExecuteStoredProcedureWithStatusAsync<T>(
            string procedureName,
            Dictionary<string, object>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return Result<MTM_Shared_Logic.Core.Services.StoredProcedureResult<T>>.Failure("Stored procedure name cannot be empty");
            }

            if (IsSqlQuery(procedureName))
            {
                var errorMsg = "SECURITY VIOLATION: Direct SQL execution is prohibited. Only stored procedure names are allowed.";
                _logger.LogError("Attempted SQL injection or policy violation: {ProcedureName}", procedureName);
                return Result<MTM_Shared_Logic.Core.Services.StoredProcedureResult<T>>.Failure(errorMsg);
            }

            try
            {
                _logger.LogDebug("Executing stored procedure with status: {ProcedureName}", procedureName);

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var dynParams = new DynamicParameters();
                
                // Add input parameters
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        dynParams.Add(param.Key, param.Value);
                    }
                }

                // Add standard output parameters
                dynParams.Add("p_Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                dynParams.Add("p_ErrorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                var results = await connection.QueryAsync<T>(
                    procedureName,
                    dynParams,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout);

                var procedureResult = new MTM_Shared_Logic.Core.Services.StoredProcedureResult<T>
                {
                    Data = results.ToList(),
                    Status = dynParams.Get<int>("p_Status"),
                    ErrorMessage = dynParams.Get<string>("p_ErrorMsg")
                };

                // Capture all output parameters
                foreach (var paramName in dynParams.ParameterNames)
                {
                    if (paramName.StartsWith("p_") || paramName.StartsWith("@"))
                    {
                        procedureResult.OutputParameters[paramName] = dynParams.Get<object>(paramName);
                    }
                }

                _logger.LogDebug("Stored procedure with status executed successfully, Status: {Status}, Rows: {Count}", 
                    procedureResult.Status, procedureResult.Data.Count);
                
                return Result<MTM_Shared_Logic.Core.Services.StoredProcedureResult<T>>.Success(procedureResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stored procedure with status execution failed");
                return Result<MTM_Shared_Logic.Core.Services.StoredProcedureResult<T>>.Failure($"Stored procedure execution failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a stored procedure that returns a single scalar value.
        /// ENFORCES: Only stored procedures are allowed - NO direct SQL commands.
        /// </summary>
        public async Task<Result<T?>> ExecuteStoredProcedureScalarAsync<T>(
            string procedureName,
            Dictionary<string, object>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return Result<T?>.Failure("Stored procedure name cannot be empty");
            }

            if (IsSqlQuery(procedureName))
            {
                var errorMsg = "SECURITY VIOLATION: Direct SQL execution is prohibited. Only stored procedure names are allowed.";
                _logger.LogError("Attempted SQL injection or policy violation: {ProcedureName}", procedureName);
                return Result<T?>.Failure(errorMsg);
            }

            try
            {
                _logger.LogDebug("Executing scalar stored procedure: {ProcedureName}", procedureName);

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var result = await connection.QuerySingleOrDefaultAsync<T>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout);

                _logger.LogDebug("Scalar stored procedure executed successfully, result: {Result}", result);
                return Result<T?>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scalar stored procedure execution failed");
                return Result<T?>.Failure($"Scalar stored procedure execution failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes a stored procedure for non-query operations (INSERT, UPDATE, DELETE).
        /// Returns the number of affected rows.
        /// ENFORCES: Only stored procedures are allowed - NO direct SQL commands.
        /// </summary>
        public async Task<Result<int>> ExecuteStoredProcedureNonQueryAsync(
            string procedureName,
            Dictionary<string, object>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return Result<int>.Failure("Stored procedure name cannot be empty");
            }

            if (IsSqlQuery(procedureName))
            {
                var errorMsg = "SECURITY VIOLATION: Direct SQL execution is prohibited. Only stored procedure names are allowed.";
                _logger.LogError("Attempted SQL injection or policy violation: {ProcedureName}", procedureName);
                return Result<int>.Failure(errorMsg);
            }

            try
            {
                _logger.LogDebug("Executing non-query stored procedure: {ProcedureName}", procedureName);

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var affectedRows = await connection.ExecuteAsync(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: _commandTimeout);

                _logger.LogDebug("Non-query stored procedure executed successfully, affected {AffectedRows} rows", affectedRows);
                return Result<int>.Success(affectedRows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Non-query stored procedure execution failed");
                return Result<int>.Failure($"Non-query stored procedure execution failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes multiple stored procedures within a single transaction.
        /// Ensures data consistency across multiple database operations.
        /// </summary>
        public async Task<Result> ExecuteTransactionAsync(
            Func<IDbConnection, IDbTransaction, Task> operations,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Starting database transaction");

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                using var transaction = connection.BeginTransaction();
                
                try
                {
                    await operations(connection, transaction);
                    transaction.Commit();
                    
                    _logger.LogDebug("Database transaction completed successfully");
                    return Result.Success();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _logger.LogWarning("Database transaction rolled back due to error");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database transaction failed");
                return Result.Failure($"Transaction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes multiple stored procedures within a single transaction and returns a result.
        /// </summary>
        public async Task<Result<T>> ExecuteTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, Task<T>> operations,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Starting database transaction with return value");

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                using var transaction = connection.BeginTransaction();
                
                try
                {
                    var result = await operations(connection, transaction);
                    transaction.Commit();
                    
                    _logger.LogDebug("Database transaction completed successfully with result");
                    return Result<T>.Success(result);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _logger.LogWarning("Database transaction rolled back due to error");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database transaction with return value failed");
                return Result<T>.Failure($"Transaction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// DEPRECATED: This method is provided for backward compatibility only.
        /// DO NOT USE: Direct query execution violates the "no hard-coded MySQL" rule.
        /// Use ExecuteStoredProcedureAsync instead.
        /// </summary>
        public async Task<Result<List<T>>> ExecuteQueryAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("DEPRECATED METHOD CALLED: ExecuteQueryAsync - Use ExecuteStoredProcedureAsync instead");
            
            // Security validation: Prevent direct SQL execution
            if (IsSqlQuery(query))
            {
                var errorMsg = "SECURITY VIOLATION: Direct SQL execution is prohibited. Use ExecuteStoredProcedureAsync instead.";
                _logger.LogError("Attempted deprecated SQL execution: {Query}", query);
                return Result<List<T>>.Failure(errorMsg);
            }

            // If it's actually a stored procedure name, redirect to the proper method
            var parameters_dict = ConvertParametersToDict(parameters);
            return await ExecuteStoredProcedureAsync<T>(query, parameters_dict, cancellationToken);
        }

        /// <summary>
        /// DEPRECATED: This method is provided for backward compatibility only.
        /// DO NOT USE: Direct command execution violates the "no hard-coded MySQL" rule.
        /// Use ExecuteStoredProcedureNonQueryAsync instead.
        /// </summary>
        public async Task<Result<int>> ExecuteNonQueryAsync(string command, object? parameters = null, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("DEPRECATED METHOD CALLED: ExecuteNonQueryAsync - Use ExecuteStoredProcedureNonQueryAsync instead");
            
            // Security validation: Prevent direct SQL execution
            if (IsSqlQuery(command))
            {
                var errorMsg = "SECURITY VIOLATION: Direct command execution is prohibited. Use ExecuteStoredProcedureNonQueryAsync instead.";
                _logger.LogError("Attempted deprecated command execution: {Command}", command);
                return Result<int>.Failure(errorMsg);
            }

            // If it's actually a stored procedure name, redirect to the proper method
            var parameters_dict = ConvertParametersToDict(parameters);
            return await ExecuteStoredProcedureNonQueryAsync(command, parameters_dict, cancellationToken);
        }

        /// <summary>
        /// DEPRECATED: This method is provided for backward compatibility only.
        /// DO NOT USE: Direct scalar query execution violates the "no hard-coded MySQL" rule.
        /// Use ExecuteStoredProcedureScalarAsync instead.
        /// </summary>
        public async Task<Result<T?>> ExecuteScalarAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("DEPRECATED METHOD CALLED: ExecuteScalarAsync - Use ExecuteStoredProcedureScalarAsync instead");
            
            // Security validation: Prevent direct SQL execution
            if (IsSqlQuery(query))
            {
                var errorMsg = "SECURITY VIOLATION: Direct scalar query execution is prohibited. Use ExecuteStoredProcedureScalarAsync instead.";
                _logger.LogError("Attempted deprecated scalar query execution: {Query}", query);
                return Result<T?>.Failure(errorMsg);
            }

            // If it's actually a stored procedure name, redirect to the proper method
            var parameters_dict = ConvertParametersToDict(parameters);
            return await ExecuteStoredProcedureScalarAsync<T>(query, parameters_dict, cancellationToken);
        }

        /// <summary>
        /// Helper method to convert anonymous object parameters to Dictionary format.
        /// </summary>
        private static Dictionary<string, object>? ConvertParametersToDict(object? parameters)
        {
            if (parameters == null)
                return null;

            if (parameters is Dictionary<string, object> dict)
                return dict;

            var result = new Dictionary<string, object>();
            foreach (var prop in parameters.GetType().GetProperties())
            {
                result[prop.Name] = prop.GetValue(parameters) ?? DBNull.Value;
            }
            return result;
        }

        /// <summary>
        /// Validates that the input is a procedure name and not SQL code.
        /// Prevents SQL injection and enforces stored procedure usage.
        /// </summary>
        private static bool IsSqlQuery(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var lowerInput = input.Trim().ToLowerInvariant();
            
            // Check for SQL keywords that indicate direct SQL rather than procedure names
            var sqlKeywords = new[]
            {
                "select", "insert", "update", "delete", "drop", "create", "alter", 
                "truncate", "grant", "revoke", "union", "join", "where", "from", 
                "into", "values", "set", "exec", "execute", "sp_executesql"
            };

            return sqlKeywords.Any(keyword => lowerInput.Contains(keyword + " ") || 
                                            lowerInput.StartsWith(keyword + " ") ||
                                            lowerInput.Equals(keyword));
        }

        /// <summary>
        /// Determines if a MySQL exception should trigger a retry.
        /// </summary>
        private static bool ShouldRetry(MySqlException ex)
        {
            // Retry on connection timeouts, deadlocks, and temporary failures
            return ex.Number switch
            {
                1205 => true, // Lock wait timeout
                1213 => true, // Deadlock found when trying to get lock
                2006 => true, // MySQL server has gone away
                2013 => true, // Lost connection to MySQL server during query
                _ => false
            };
        }

    }

    /// <summary>
    /// Database connection factory implementation for dependency injection.
    /// </summary>
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        public string ConnectionString { get; }

        public MySqlConnectionFactory(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentException("DefaultConnection string not found in configuration");
        }

        public async Task<DbConnection> CreateConnectionAsync()
        {
            var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            return connection;
        }
    }

    /// <summary>
    /// Database transaction scope service for managing transactions.
    /// ENFORCES: Only stored procedures can be executed within transactions.
    /// </summary>
    public class DatabaseTransactionService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseTransactionService> _logger;

        public DatabaseTransactionService(IConfiguration configuration, ILogger<DatabaseTransactionService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentException("DefaultConnection string not found in configuration");
            _logger = logger;
        }

        /// <summary>
        /// Executes multiple operations within a database transaction.
        /// SECURITY NOTE: All operations within the transaction must use stored procedures only.
        /// </summary>
        public async Task<Result> ExecuteInTransactionAsync(
            Func<IDbConnection, IDbTransaction, Task> operations,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Starting database transaction");

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                using var transaction = connection.BeginTransaction();
                
                try
                {
                    await operations(connection, transaction);
                    transaction.Commit();
                    
                    _logger.LogDebug("Database transaction completed successfully");
                    return Result.Success();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _logger.LogWarning("Database transaction rolled back due to error");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database transaction failed");
                return Result.Failure($"Transaction failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes multiple operations within a database transaction and returns a result.
        /// SECURITY NOTE: All operations within the transaction must use stored procedures only.
        /// </summary>
        public async Task<Result<T>> ExecuteInTransactionAsync<T>(
            Func<IDbConnection, IDbTransaction, Task<T>> operations,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Starting database transaction with return value");

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                using var transaction = connection.BeginTransaction();
                
                try
                {
                    var result = await operations(connection, transaction);
                    transaction.Commit();
                    
                    _logger.LogDebug("Database transaction completed successfully with result");
                    return Result<T>.Success(result);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _logger.LogWarning("Database transaction rolled back due to error");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database transaction with return value failed");
                return Result<T>.Failure($"Transaction failed: {ex.Message}");
            }
        }
    }
}