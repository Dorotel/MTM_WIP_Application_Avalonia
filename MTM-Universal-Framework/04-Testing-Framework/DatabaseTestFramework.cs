using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using System.Data.SQLite;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Database testing framework with support for multiple database providers and transaction management.
    /// Provides in-memory database testing with automatic cleanup and data seeding.
    /// </summary>
    public class DatabaseTestFramework : UniversalTestBase
    {
        protected IDbConnection DatabaseConnection { get; private set; }
        protected List<string> TestTables { get; private set; }
        protected Dictionary<string, object> TestData { get; private set; }

        protected DatabaseTestFramework() : base()
        {
            TestTables = new List<string>();
            TestData = new Dictionary<string, object>();
            InitializeDatabaseAsync().Wait();
        }

        /// <summary>
        /// Initialize in-memory SQLite database for testing
        /// </summary>
        protected virtual async Task InitializeDatabaseAsync()
        {
            var connectionString = "Data Source=:memory:;Version=3;New=true;";
            DatabaseConnection = new SQLiteConnection(connectionString);
            await DatabaseConnection.OpenAsync();

            // Create common test tables
            await CreateTestTablesAsync();
        }

        /// <summary>
        /// Create standard test tables for business applications
        /// </summary>
        protected virtual async Task CreateTestTablesAsync()
        {
            var createItemsTable = @"
                CREATE TABLE Items (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    Category TEXT,
                    Quantity INTEGER DEFAULT 0,
                    Price DECIMAL(10,2) DEFAULT 0.0,
                    IsActive BOOLEAN DEFAULT 1,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UpdatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    CreatedBy TEXT,
                    UpdatedBy TEXT
                )";

            var createCategoriesTable = @"
                CREATE TABLE Categories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    IsActive BOOLEAN DEFAULT 1,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            var createTransactionsTable = @"
                CREATE TABLE Transactions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ItemId INTEGER,
                    TransactionType TEXT NOT NULL,
                    Quantity INTEGER NOT NULL,
                    Notes TEXT,
                    TransactionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UserId TEXT,
                    FOREIGN KEY (ItemId) REFERENCES Items(Id)
                )";

            await ExecuteSqlAsync(createItemsTable);
            await ExecuteSqlAsync(createCategoriesTable);
            await ExecuteSqlAsync(createTransactionsTable);

            TestTables.AddRange(new[] { "Items", "Categories", "Transactions" });
        }

        /// <summary>
        /// Execute SQL command against test database
        /// </summary>
        protected async Task<int> ExecuteSqlAsync(string sql, Dictionary<string, object> parameters = null)
        {
            using var command = DatabaseConnection.CreateCommand();
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var dbParam = command.CreateParameter();
                    dbParam.ParameterName = param.Key;
                    dbParam.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParam);
                }
            }

            return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Execute query and return results
        /// </summary>
        protected async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql, Dictionary<string, object> parameters = null)
        {
            using var command = DatabaseConnection.CreateCommand();
            command.CommandText = sql;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var dbParam = command.CreateParameter();
                    dbParam.ParameterName = param.Key;
                    dbParam.Value = param.Value ?? DBNull.Value;
                    command.Parameters.Add(dbParam);
                }
            }

            var results = new List<Dictionary<string, object>>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                }
                results.Add(row);
            }

            return results;
        }

        /// <summary>
        /// Seed test data for standard scenarios
        /// </summary>
        protected virtual async Task SeedTestDataAsync()
        {
            // Insert test categories
            await ExecuteSqlAsync(
                "INSERT INTO Categories (Name, Description) VALUES (@name, @desc)",
                new Dictionary<string, object> { { "@name", "Electronics" }, { "@desc", "Electronic items and components" } }
            );

            await ExecuteSqlAsync(
                "INSERT INTO Categories (Name, Description) VALUES (@name, @desc)",
                new Dictionary<string, object> { { "@name", "Tools" }, { "@desc", "Hand tools and equipment" } }
            );

            // Insert test items
            await ExecuteSqlAsync(
                "INSERT INTO Items (Name, Description, Category, Quantity, Price, CreatedBy) VALUES (@name, @desc, @cat, @qty, @price, @user)",
                new Dictionary<string, object>
                {
                    { "@name", "Test Widget" },
                    { "@desc", "A test widget for validation" },
                    { "@cat", "Electronics" },
                    { "@qty", 10 },
                    { "@price", 29.99 },
                    { "@user", "TestUser" }
                }
            );

            await ExecuteSqlAsync(
                "INSERT INTO Items (Name, Description, Category, Quantity, Price, CreatedBy) VALUES (@name, @desc, @cat, @qty, @price, @user)",
                new Dictionary<string, object>
                {
                    { "@name", "Power Drill" },
                    { "@desc", "Cordless power drill" },
                    { "@cat", "Tools" },
                    { "@qty", 5 },
                    { "@price", 89.99 },
                    { "@user", "TestUser" }
                }
            );
        }

        /// <summary>
        /// Clean up test data between tests
        /// </summary>
        protected virtual async Task CleanupTestDataAsync()
        {
            foreach (var table in TestTables)
            {
                await ExecuteSqlAsync($"DELETE FROM {table}");
            }
        }

        /// <summary>
        /// Verify table exists and has expected structure
        /// </summary>
        protected async Task<bool> VerifyTableStructureAsync(string tableName, List<string> expectedColumns)
        {
            var results = await ExecuteQueryAsync($"PRAGMA table_info({tableName})");
            var actualColumns = new List<string>();

            foreach (var row in results)
            {
                if (row.ContainsKey("name"))
                {
                    actualColumns.Add(row["name"].ToString());
                }
            }

            foreach (var expectedColumn in expectedColumns)
            {
                if (!actualColumns.Contains(expectedColumn))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                DatabaseConnection?.Close();
                DatabaseConnection?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Example database test implementation
    /// </summary>
    public class ExampleDatabaseTests : DatabaseTestFramework
    {
        [Fact]
        public async Task Database_CreateItem_ShouldPersistCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var newItemId = await ExecuteSqlAsync(
                "INSERT INTO Items (Name, Description, Category, Quantity, CreatedBy) VALUES (@name, @desc, @cat, @qty, @user)",
                new Dictionary<string, object>
                {
                    { "@name", "New Test Item" },
                    { "@desc", "Newly created test item" },
                    { "@cat", "Electronics" },
                    { "@qty", 15 },
                    { "@user", "TestUser" }
                }
            );

            // Assert
            var results = await ExecuteQueryAsync(
                "SELECT * FROM Items WHERE Name = @name",
                new Dictionary<string, object> { { "@name", "New Test Item" } }
            );

            Assert.Single(results);
            Assert.Equal("New Test Item", results[0]["Name"]);
            Assert.Equal("Electronics", results[0]["Category"]);
            Assert.Equal(15, Convert.ToInt32(results[0]["Quantity"]));
        }

        [Fact]
        public async Task Database_UpdateItem_ShouldModifyCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            await ExecuteSqlAsync(
                "UPDATE Items SET Quantity = @qty, UpdatedBy = @user WHERE Name = @name",
                new Dictionary<string, object>
                {
                    { "@qty", 25 },
                    { "@user", "UpdateUser" },
                    { "@name", "Test Widget" }
                }
            );

            // Assert
            var results = await ExecuteQueryAsync(
                "SELECT * FROM Items WHERE Name = @name",
                new Dictionary<string, object> { { "@name", "Test Widget" } }
            );

            Assert.Single(results);
            Assert.Equal(25, Convert.ToInt32(results[0]["Quantity"]));
            Assert.Equal("UpdateUser", results[0]["UpdatedBy"]);
        }

        [Fact]
        public async Task Database_TableStructure_ShouldHaveRequiredColumns()
        {
            // Arrange
            var expectedItemColumns = new List<string>
            {
                "Id", "Name", "Description", "Category", "Quantity", 
                "Price", "IsActive", "CreatedDate", "UpdatedDate", "CreatedBy", "UpdatedBy"
            };

            // Act & Assert
            var isValid = await VerifyTableStructureAsync("Items", expectedItemColumns);
            Assert.True(isValid, "Items table should have all required columns");
        }
    }
}