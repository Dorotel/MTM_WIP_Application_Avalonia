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
using MTM.Models;
using MTM.Core.Services;
using System.Linq;

namespace MTM.Services
{
    /// <summary>
    /// Database service implementation providing centralized data access with connection management.
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
                
                // Execute a simple query to verify connection
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
        /// Executes a query and returns the results.
        /// </summary>
        public async Task<Result<List<T>>> ExecuteQueryAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Result<List<T>>.Failure("Query cannot be empty");
            }

            for (int attempt = 1; attempt <= _maxRetryAttempts; attempt++)
            {
                try
                {
                    _logger.LogDebug("Executing query (attempt {Attempt}): {Query}", attempt, query);

                    using var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync(cancellationToken);

                    var results = await connection.QueryAsync<T>(
                        query, 
                        parameters, 
                        commandTimeout: _commandTimeout);

                    var resultList = results.ToList();
                    
                    _logger.LogDebug("Query executed successfully, returned {Count} rows", resultList.Count);
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
                    _logger.LogError(ex, "Query execution failed on attempt {Attempt}", attempt);
                    return Result<List<T>>.Failure($"Query execution failed: {ex.Message}");
                }
            }

            return Result<List<T>>.Failure($"Query execution failed after {_maxRetryAttempts} attempts");
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE).
        /// </summary>
        public async Task<Result<int>> ExecuteNonQueryAsync(string command, object? parameters = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return Result<int>.Failure("Command cannot be empty");
            }

            for (int attempt = 1; attempt <= _maxRetryAttempts; attempt++)
            {
                try
                {
                    _logger.LogDebug("Executing non-query command (attempt {Attempt}): {Command}", attempt, command);

                    using var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync(cancellationToken);

                    var affectedRows = await connection.ExecuteAsync(
                        command, 
                        parameters, 
                        commandTimeout: _commandTimeout);

                    _logger.LogDebug("Non-query command executed successfully, affected {AffectedRows} rows", affectedRows);
                    return Result<int>.Success(affectedRows);
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
                    _logger.LogError(ex, "Command execution failed on attempt {Attempt}", attempt);
                    return Result<int>.Failure($"Command execution failed: {ex.Message}");
                }
            }

            return Result<int>.Failure($"Command execution failed after {_maxRetryAttempts} attempts");
        }

        /// <summary>
        /// Executes a scalar query returning a single value.
        /// </summary>
        public async Task<Result<T?>> ExecuteScalarAsync<T>(string query, object? parameters = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Result<T?>.Failure("Query cannot be empty");
            }

            for (int attempt = 1; attempt <= _maxRetryAttempts; attempt++)
            {
                try
                {
                    _logger.LogDebug("Executing scalar query (attempt {Attempt}): {Query}", attempt, query);

                    using var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync(cancellationToken);

                    var result = await connection.QuerySingleOrDefaultAsync<T>(
                        query, 
                        parameters, 
                        commandTimeout: _commandTimeout);

                    _logger.LogDebug("Scalar query executed successfully, result: {Result}", result);
                    return Result<T?>.Success(result);
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
                    _logger.LogError(ex, "Scalar query execution failed on attempt {Attempt}", attempt);
                    return Result<T?>.Failure($"Scalar query execution failed: {ex.Message}");
                }
            }

            return Result<T?>.Failure($"Scalar query execution failed after {_maxRetryAttempts} attempts");
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

        /// <summary>
        /// Masks sensitive information in connection string for logging.
        /// </summary>
        private static string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return "null";

            // Simple masking - replace password value
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString, 
                @"password\s*=\s*[^;]*", 
                "password=***", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
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