using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.ViewModels;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MTM.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Unit")]
    [Category("Services")]
    [Category("QuickButtons")]
    public class QuickButtonsServiceTests
    {
        private QuickButtonsService _service = null!;
        private Mock<ILogger<QuickButtonsService>> _mockLogger = null!;
        private Mock<IDatabaseService> _mockDatabaseService = null!;
        private Mock<IConfigurationService> _mockConfigurationService = null!;
        private Mock<IFilePathService> _mockFilePathService = null!;
        private Mock<IFileSelectionService> _mockFileSelectionService = null!;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<QuickButtonsService>>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockFilePathService = new Mock<IFilePathService>();
            _mockFileSelectionService = new Mock<IFileSelectionService>();

            // Setup configuration service
            _mockConfigurationService.Setup(x => x.GetConnectionString())
                .Returns("Server=localhost;Database=mtm_test;Uid=test;Pwd=test;");

            _service = new QuickButtonsService(
                _mockDatabaseService.Object,
                _mockConfigurationService.Object,
                _mockFilePathService.Object,
                _mockFileSelectionService.Object,
                _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // No disposal needed for QuickButtonsService
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new QuickButtonsService(
                null!,
                _mockCache.Object,
                _mockDatabaseService.Object,
                _testConnectionString);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*logger*");
        }

        [Test]
        public void Constructor_WithNullCache_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new QuickButtonsService(
                _mockLogger.Object,
                null!,
                _mockDatabaseService.Object,
                _testConnectionString);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*cache*");
        }

        [Test]
        public void Constructor_WithNullConnectionString_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new QuickButtonsService(
                _mockLogger.Object,
                _mockCache.Object,
                _mockDatabaseService.Object,
                null!);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*connectionString*");
        }

        #endregion

        #region GetQuickButtonsAsync Tests

        [Test]
        public async Task GetQuickButtonsAsync_WithValidUser_ShouldReturnButtons()
        {
            // Arrange
            var testUser = "TestUser";
            var expectedButtons = new List<QuickButtonItemViewModel>
            {
                new() { Id = 1, PartId = "PART001", Operation = "100", Quantity = 5, Location = "A01", DisplayText = "PART001 @ 100 (5)" },
                new() { Id = 2, PartId = "PART002", Operation = "110", Quantity = 10, Location = "B02", DisplayText = "PART002 @ 110 (10)" }
            };

            // Setup database mock to return test data
            var testDataTable = CreateQuickButtonsDataTable(expectedButtons);
            var mockResult = new DatabaseResult { Status = 1, Data = testDataTable };

            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.GetQuickButtonsAsync(testUser);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].PartId.Should().Be("PART001");
            result[1].PartId.Should().Be("PART002");

            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", 
                It.Is<MySqlParameter[]>(p => p.Length == 1 && p[0].ParameterName == "p_User" && p[0].Value.ToString() == testUser)), 
                Times.Once);
        }

        [Test]
        public async Task GetQuickButtonsAsync_WithNullUser_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act = async () => await _service.GetQuickButtonsAsync(null!);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*user*");
        }

        [Test]
        public async Task GetQuickButtonsAsync_WithEmptyUser_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act = async () => await _service.GetQuickButtonsAsync("");
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*user*");
        }

        [Test]
        public async Task GetQuickButtonsAsync_WhenDatabaseFails_ShouldReturnEmptyList()
        {
            // Arrange
            var testUser = "TestUser";
            var mockResult = new DatabaseResult { Status = -1, Data = new DataTable(), Message = "Database error" };

            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.GetQuickButtonsAsync(testUser);

            // Assert - Following MTM NO FALLBACK DATA pattern
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetQuickButtonsAsync_WhenDatabaseReturnsNull_ShouldReturnEmptyList()
        {
            // Arrange
            var testUser = "TestUser";
            var mockResult = new DatabaseResult { Status = 1, Data = null! };

            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.GetQuickButtonsAsync(testUser);

            // Assert - Following MTM NO FALLBACK DATA pattern
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region SaveQuickButtonAsync Tests

        [Test]
        public async Task SaveQuickButtonAsync_WithValidButton_ShouldReturnTrue()
        {
            // Arrange
            var testButton = new QuickButtonItemViewModel
            {
                PartId = "PART001",
                Operation = "100",
                Quantity = 5,
                Location = "A01",
                DisplayText = "PART001 @ 100 (5)"
            };
            var testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Save", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.SaveQuickButtonAsync(testButton, testUser);

            // Assert
            result.Should().BeTrue();

            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Save", 
                It.Is<MySqlParameter[]>(p => 
                    p.Any(param => param.ParameterName == "p_User" && param.Value.ToString() == testUser) &&
                    p.Any(param => param.ParameterName == "p_PartID" && param.Value.ToString() == testButton.PartId) &&
                    p.Any(param => param.ParameterName == "p_OperationNumber" && param.Value.ToString() == testButton.Operation))), 
                Times.Once);
        }

        [Test]
        public async Task SaveQuickButtonAsync_WithNullButton_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = async () => await _service.SaveQuickButtonAsync(null!, "TestUser");
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*quickButton*");
        }

        [Test]
        public async Task SaveQuickButtonAsync_WithNullUser_ShouldThrowArgumentException()
        {
            // Arrange
            var testButton = new QuickButtonItemViewModel { PartId = "PART001" };

            // Act & Assert
            var act = async () => await _service.SaveQuickButtonAsync(testButton, null!);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*user*");
        }

        [Test]
        public async Task SaveQuickButtonAsync_WhenDatabaseFails_ShouldReturnFalse()
        {
            // Arrange
            var testButton = new QuickButtonItemViewModel { PartId = "PART001", Operation = "100" };
            var testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = -1, Message = "Save failed" };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Save", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.SaveQuickButtonAsync(testButton, testUser);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region ExecuteQuickButtonAsync Tests

        [Test]
        public async Task ExecuteQuickButtonAsync_WithValidButton_ShouldExecuteTransaction()
        {
            // Arrange
            var testButton = new QuickButtonItemViewModel
            {
                PartId = "EXEC001",
                Operation = "100",
                Quantity = 5,
                Location = "A01"
            };
            var testUser = "TestUser";

            // Setup successful inventory operation
            var mockInventoryResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("inv_inventory_Add_Item", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockInventoryResult);

            // Setup successful transaction logging
            var mockTransactionResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("inv_transaction_Add", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockTransactionResult);

            // Act
            var result = await _service.ExecuteQuickButtonAsync(testButton, testUser);

            // Assert
            result.Should().BeTrue();

            // Verify inventory operation was called
            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("inv_inventory_Add_Item", 
                It.Is<MySqlParameter[]>(p => 
                    p.Any(param => param.ParameterName == "p_PartID" && param.Value.ToString() == testButton.PartId) &&
                    p.Any(param => param.ParameterName == "p_OperationNumber" && param.Value.ToString() == testButton.Operation))), 
                Times.Once);

            // Verify transaction was logged
            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("inv_transaction_Add", It.IsAny<MySqlParameter[]>()), Times.Once);
        }

        [Test]
        public async Task ExecuteQuickButtonAsync_WithNullButton_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = async () => await _service.ExecuteQuickButtonAsync(null!, "TestUser");
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("*quickButton*");
        }

        [Test]
        public async Task ExecuteQuickButtonAsync_WhenInventoryOperationFails_ShouldReturnFalse()
        {
            // Arrange
            var testButton = new QuickButtonItemViewModel
            {
                PartId = "FAIL001",
                Operation = "100",
                Quantity = 5,
                Location = "A01"
            };
            var testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = -1, Message = "Inventory operation failed" };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("inv_inventory_Add_Item", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.ExecuteQuickButtonAsync(testButton, testUser);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region DeleteQuickButtonAsync Tests

        [Test]
        public async Task DeleteQuickButtonAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            int buttonId = 123;
            string testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Remove", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.DeleteQuickButtonAsync(buttonId, testUser);

            // Assert
            result.Should().BeTrue();

            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Remove", 
                It.Is<MySqlParameter[]>(p => 
                    p.Any(param => param.ParameterName == "p_Id" && param.Value.Equals(buttonId)) &&
                    p.Any(param => param.ParameterName == "p_User" && param.Value.ToString() == testUser))), 
                Times.Once);
        }

        [Test]
        public async Task DeleteQuickButtonAsync_WithInvalidId_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act = async () => await _service.DeleteQuickButtonAsync(-1, "TestUser");
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*buttonId*");
        }

        [Test]
        public async Task DeleteQuickButtonAsync_WhenDatabaseFails_ShouldReturnFalse()
        {
            // Arrange
            int buttonId = 123;
            string testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = -1, Message = "Delete failed" };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Remove", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.DeleteQuickButtonAsync(buttonId, testUser);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region ClearAllQuickButtonsAsync Tests

        [Test]
        public async Task ClearAllQuickButtonsAsync_WithValidUser_ShouldReturnTrue()
        {
            // Arrange
            string testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Clear_ByUser", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _service.ClearAllQuickButtonsAsync(testUser);

            // Assert
            result.Should().BeTrue();

            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Clear_ByUser", 
                It.Is<MySqlParameter[]>(p => p.Length == 1 && p[0].ParameterName == "p_User" && p[0].Value.ToString() == testUser)), 
                Times.Once);
        }

        [Test]
        public async Task ClearAllQuickButtonsAsync_WithNullUser_ShouldThrowArgumentException()
        {
            // Act & Assert
            var act = async () => await _service.ClearAllQuickButtonsAsync(null!);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*user*");
        }

        #endregion

        #region MTM Manufacturing Domain Tests

        [Test]
        [TestCase("PART001", "90", 1, "RECEIVING", "IN", true)]     // Valid receiving operation
        [TestCase("ABC-123", "100", 10, "STATION_A", "IN", true)]   // Valid first operation
        [TestCase("TEST-999", "110", 25, "STATION_B", "OUT", true)] // Valid second operation
        [TestCase("FINAL-001", "120", 5, "STATION_C", "TRANSFER", true)] // Valid final operation
        [TestCase("", "100", 5, "A01", "IN", false)]                // Invalid part ID
        [TestCase("PART001", "", 5, "A01", "IN", false)]            // Invalid operation
        [TestCase("PART001", "100", 0, "A01", "IN", false)]         // Invalid quantity
        [TestCase("PART001", "100", 5, "", "IN", false)]            // Invalid location
        [TestCase("PART001", "100", 5, "A01", "", false)]           // Invalid transaction type
        public async Task ValidateQuickButtonData_VariousInputs_ShouldFollowMTMStandards(
            string partId, string operation, int quantity, string location, string transactionType, bool shouldBeValid)
        {
            // Arrange
            var quickButton = new QuickButtonItemViewModel
            {
                PartId = partId,
                Operation = operation,
                Quantity = quantity,
                Location = location,
                TransactionType = transactionType
            };

            if (shouldBeValid)
            {
                var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
                _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
                    .ReturnsAsync(mockResult);
            }

            // Act
            var result = await _service.SaveQuickButtonAsync(quickButton, "TestUser");

            // Assert
            if (shouldBeValid)
            {
                result.Should().BeTrue();
                _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Save", It.IsAny<MySqlParameter[]>()), Times.Once);
            }
            else
            {
                result.Should().BeFalse();
                _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<MySqlParameter[]>()), Times.Never);
            }
        }

        [Test]
        public async Task QuickButton_WorkflowOperations_ShouldSupportAllStandardOperations()
        {
            // Arrange - Test all standard MTM manufacturing operations
            var standardOperations = new[] { "90", "100", "110", "120", "130" };
            var testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act & Assert
            foreach (var operation in standardOperations)
            {
                var quickButton = new QuickButtonItemViewModel
                {
                    PartId = $"WORKFLOW_PART",
                    Operation = operation,
                    Quantity = 10,
                    Location = "STATION_A",
                    TransactionType = "IN"
                };

                var result = await _service.SaveQuickButtonAsync(quickButton, testUser);
                result.Should().BeTrue($"Operation {operation} should be valid");
            }

            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Save", It.IsAny<MySqlParameter[]>()), 
                Times.Exactly(standardOperations.Length));
        }

        [Test]
        public async Task QuickButton_TransactionTypes_ShouldFollowMTMStandards()
        {
            // Arrange - Test all standard MTM transaction types
            var transactionTypes = new[] { "IN", "OUT", "TRANSFER" };
            var testUser = "TestUser";

            var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act & Assert
            foreach (var transactionType in transactionTypes)
            {
                var quickButton = new QuickButtonItemViewModel
                {
                    PartId = "TRANS_TEST",
                    Operation = "100",
                    Quantity = 5,
                    Location = "STATION_A",
                    TransactionType = transactionType
                };

                var result = await _service.SaveQuickButtonAsync(quickButton, testUser);
                result.Should().BeTrue($"Transaction type {transactionType} should be valid");
            }
        }

        #endregion

        #region Cache Tests

        [Test]
        public async Task GetQuickButtonsAsync_WithCaching_ShouldUseCacheOnSecondCall()
        {
            // Arrange
            var testUser = "CacheTestUser";
            var expectedButtons = new List<QuickButtonItemViewModel>
            {
                new() { Id = 1, PartId = "CACHE001", Operation = "100", Quantity = 5 }
            };

            var testDataTable = CreateQuickButtonsDataTable(expectedButtons);
            var mockResult = new DatabaseResult { Status = 1, Data = testDataTable };

            // Setup cache to return cached data on second call
            object cachedValue = expectedButtons;
            _mockCache.Setup(x => x.TryGetValue($"QuickButtons_{testUser}", out cachedValue))
                .Returns(true);

            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act - First call should hit database
            var firstResult = await _service.GetQuickButtonsAsync(testUser);

            // Setup cache to return data for second call
            _mockCache.Setup(x => x.TryGetValue($"QuickButtons_{testUser}", out cachedValue))
                .Returns(true);

            // Act - Second call should use cache
            var secondResult = await _service.GetQuickButtonsAsync(testUser);

            // Assert
            firstResult.Should().HaveCount(1);
            secondResult.Should().HaveCount(1);

            // First call should hit database, second should use cache
            _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()), Times.Once);
        }

        #endregion

        #region Performance Tests

        [Test]
        public async Task GetQuickButtonsAsync_WithLargeDataset_ShouldPerformEfficiently()
        {
            // Arrange
            var testUser = "PerformanceTestUser";
            var largeButtonList = new List<QuickButtonItemViewModel>();

            // Create 100 QuickButton entries
            for (int i = 1; i <= 100; i++)
            {
                largeButtonList.Add(new QuickButtonItemViewModel
                {
                    Id = i,
                    PartId = $"PERF{i:000}",
                    Operation = (90 + (i % 4) * 10).ToString(),
                    Quantity = i % 20 + 1,
                    Location = $"STATION_{(char)('A' + i % 5)}",
                    DisplayText = $"PERF{i:000} @ {90 + (i % 4) * 10} ({i % 20 + 1})"
                });
            }

            var testDataTable = CreateQuickButtonsDataTable(largeButtonList);
            var mockResult = new DatabaseResult { Status = 1, Data = testDataTable };

            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _service.GetQuickButtonsAsync(testUser);
            stopwatch.Stop();

            // Assert
            result.Should().HaveCount(100);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(500, "Loading 100 QuickButtons should complete within 500ms");
        }

        [Test]
        public async Task ExecuteQuickButtonAsync_ConcurrentExecution_ShouldHandleMultipleRequests()
        {
            // Arrange
            var testButtons = new List<QuickButtonItemViewModel>();
            for (int i = 1; i <= 10; i++)
            {
                testButtons.Add(new QuickButtonItemViewModel
                {
                    PartId = $"CONCURRENT{i:00}",
                    Operation = "100",
                    Quantity = i,
                    Location = "CONCURRENT_STATION"
                });
            }

            var mockResult = new DatabaseResult { Status = 1, Data = new DataTable() };
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
                .ReturnsAsync(mockResult);

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var tasks = testButtons.Select(button => _service.ExecuteQuickButtonAsync(button, "ConcurrentUser"));
            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            results.Should().AllSatisfy(result => result.Should().BeTrue());
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000, "10 concurrent executions should complete within 2 seconds");
        }

        #endregion

        #region Error Handling Tests

        [Test]
        public async Task GetQuickButtonsAsync_WhenDatabaseThrows_ShouldReturnEmptyAndLog()
        {
            // Arrange
            var testUser = "ErrorTestUser";
            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Get_ByUser", It.IsAny<MySqlParameter[]>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            var result = await _service.GetQuickButtonsAsync(testUser);

            // Assert - Following MTM NO FALLBACK DATA pattern
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Test]
        public async Task SaveQuickButtonAsync_WhenDatabaseThrows_ShouldReturnFalseAndLog()
        {
            // Arrange
            var testButton = new QuickButtonItemViewModel { PartId = "ERROR001", Operation = "100", Quantity = 5, Location = "A01" };
            var testUser = "ErrorTestUser";

            _mockDatabaseService.Setup(x => x.ExecuteStoredProcedureAsync("qb_quickbuttons_Save", It.IsAny<MySqlParameter[]>()))
                .ThrowsAsync(new MySqlException("Connection timeout"));

            // Act
            var result = await _service.SaveQuickButtonAsync(testButton, testUser);

            // Assert
            result.Should().BeFalse();

            // Verify error was logged
            _mockLogger.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        #endregion

        #region Disposal Tests

        [Test]
        public void Dispose_WhenCalled_ShouldNotThrow()
        {
            // Act & Assert
            var act = () => _service.Dispose();
            act.Should().NotThrow();
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_ShouldNotThrow()
        {
            // Act & Assert
            var act = () =>
            {
                _service.Dispose();
                _service.Dispose();
                _service.Dispose();
            };
            act.Should().NotThrow();
        }

        #endregion

        #region Helper Methods

        private DataTable CreateQuickButtonsDataTable(IEnumerable<QuickButtonItemViewModel> buttons)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("PartID", typeof(string));
            dataTable.Columns.Add("OperationNumber", typeof(string));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("Location", typeof(string));
            dataTable.Columns.Add("DisplayText", typeof(string));
            dataTable.Columns.Add("TransactionType", typeof(string));
            dataTable.Columns.Add("ButtonOrder", typeof(int));
            dataTable.Columns.Add("CreatedDate", typeof(DateTime));

            foreach (var button in buttons)
            {
                var row = dataTable.NewRow();
                row["Id"] = button.Id;
                row["PartID"] = button.PartId ?? "";
                row["OperationNumber"] = button.Operation ?? "";
                row["Quantity"] = button.Quantity;
                row["Location"] = button.Location ?? "";
                row["DisplayText"] = button.DisplayText ?? "";
                row["TransactionType"] = button.TransactionType ?? "IN";
                row["ButtonOrder"] = button.ButtonOrder;
                row["CreatedDate"] = DateTime.Now;
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        #endregion
    }
}