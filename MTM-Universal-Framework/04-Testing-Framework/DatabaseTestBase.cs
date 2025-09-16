using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace MTM.Universal.Testing
{
    /// <summary>
    /// Base class for database integration tests with in-memory SQLite.
    /// </summary>
    public abstract class DatabaseTestBase : TestBase
    {
        protected DbConnection DatabaseConnection { get; private set; } = null!;
        protected string ConnectionString { get; private set; } = string.Empty;

        protected override async Task InitializeAsync()
        {
            // Create in-memory SQLite database for testing
            ConnectionString = "Data Source=:memory:";
            DatabaseConnection = new SqliteConnection(ConnectionString);
            await DatabaseConnection.OpenAsync();
            
            // Create test schema
            await CreateTestSchemaAsync();
            
            await base.InitializeAsync();
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            
            // Add configuration for database testing
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("ConnectionStrings:DefaultConnection", ConnectionString),
                    new KeyValuePair<string, string?>("Testing:DatabaseProvider", "SQLite"),
                    new KeyValuePair<string, string?>("Testing:EnableTransactions", "true")
                })
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
        }

        /// <summary>
        /// Create test database schema. Override in derived classes.
        /// </summary>
        protected virtual async Task CreateTestSchemaAsync()
        {
            // Base implementation creates common test tables
            await using var command = DatabaseConnection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS TestEntities (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt TEXT DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS TestAuditLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    EntityId INTEGER,
                    Action TEXT NOT NULL,
                    OldValues TEXT,
                    NewValues TEXT,
                    UserId TEXT,
                    Timestamp TEXT DEFAULT CURRENT_TIMESTAMP
                );
            ";
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Clean database between tests.
        /// </summary>
        protected override async Task AfterEachTestAsync()
        {
            await CleanDatabaseAsync();
            await base.AfterEachTestAsync();
        }

        /// <summary>
        /// Clean all test data from database.
        /// </summary>
        protected virtual async Task CleanDatabaseAsync()
        {
            await using var command = DatabaseConnection.CreateCommand();
            command.CommandText = @"
                DELETE FROM TestAuditLog;
                DELETE FROM TestEntities;
                DELETE FROM sqlite_sequence WHERE name IN ('TestEntities', 'TestAuditLog');
            ";
            await command.ExecuteNonQueryAsync();
        }

        protected override async Task CleanupAsync()
        {
            await DatabaseConnection?.DisposeAsync()!;
            await base.CleanupAsync();
        }

        /// <summary>
        /// Execute SQL command for testing.
        /// </summary>
        protected async Task<int> ExecuteAsync(string sql, params (string Name, object Value)[] parameters)
        {
            await using var command = DatabaseConnection.CreateCommand();
            command.CommandText = sql;
            
            foreach (var (name, value) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
            
            return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Execute scalar query for testing.
        /// </summary>
        protected async Task<T?> ExecuteScalarAsync<T>(string sql, params (string Name, object Value)[] parameters)
        {
            await using var command = DatabaseConnection.CreateCommand();
            command.CommandText = sql;
            
            foreach (var (name, value) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
            
            var result = await command.ExecuteScalarAsync();
            return result is DBNull ? default : (T?)result;
        }
    }
}