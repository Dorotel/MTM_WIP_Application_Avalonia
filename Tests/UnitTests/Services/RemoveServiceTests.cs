using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace MTM.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Unit")]
    [Category("Service")]
    [Category("RemoveService")]
    public class RemoveServiceTests
    {
        private RemoveService _service;
        private Mock<ILogger<RemoveService>> _mockLogger;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IConfigurationService> _mockConfigurationService;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<RemoveService>>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockConfigurationService = new Mock<IConfigurationService>();

            _service = new RemoveService(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockConfigurationService.Object
            );
        }

        [Test]
        public void Constructor_WithValidParameters_InitializesCorrectly()
        {
            // Assert
            Assert.That(_service, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RemoveService(
                null,
                _mockDatabaseService.Object,
                _mockConfigurationService.Object
            ));
        }

        [Test]
        public void Constructor_WithNullDatabaseService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new RemoveService(
                _mockLogger.Object,
                null,
                _mockConfigurationService.Object
            ));
        }

        [Test]
        public async Task RemoveInventoryAsync_WithValidData_CallsDatabase()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var quantity = 5;
            var location = "STATION_A";
            var user = "TestUser";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Remove_Item",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            var result = await _service.RemoveInventoryAsync(partId, operation, quantity, location, user);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Remove_Item",
                It.Is<Dictionary<string, object>>(p =>
                    p["p_PartID"].ToString() == partId &&
                    p["p_OperationNumber"].ToString() == operation &&
                    Convert.ToInt32(p["p_Quantity"]) == quantity &&
                    p["p_Location"].ToString() == location &&
                    p["p_User"].ToString() == user)),
                Times.Once);
        }

        [Test]
        public async Task RemoveInventoryAsync_WithEmptyPartId_ReturnsError()
        {
            // Arrange
            var partId = "";
            var operation = "100";
            var quantity = 5;
            var location = "STATION_A";
            var user = "TestUser";

            // Act
            var result = await _service.RemoveInventoryAsync(partId, operation, quantity, location, user);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Part ID is required"));
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
        }

        [Test]
        public async Task RemoveInventoryAsync_WithZeroQuantity_ReturnsError()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var quantity = 0;
            var location = "STATION_A";
            var user = "TestUser";

            // Act
            var result = await _service.RemoveInventoryAsync(partId, operation, quantity, location, user);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Quantity must be greater than zero"));
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
        }

        [Test]
        public async Task RemoveInventoryAsync_DatabaseFailure_ReturnsError()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var quantity = 5;
            var location = "STATION_A";
            var user = "TestUser";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 0, Message = "Database error" });

            // Act
            var result = await _service.RemoveInventoryAsync(partId, operation, quantity, location, user);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Database error"));
        }

        [Test]
        public async Task CheckAvailableQuantityAsync_WithValidData_ReturnsQuantity()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var location = "STATION_A";

            var mockData = new DataTable();
            mockData.Columns.Add("Quantity", typeof(int));
            mockData.Rows.Add(25);

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Get_CurrentQty_ByPartIDandOperation",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult 
                { 
                    Status = 1, 
                    Data = mockData, 
                    Message = "Success" 
                });

            // Act
            var availableQuantity = await _service.CheckAvailableQuantityAsync(partId, operation, location);

            // Assert
            Assert.That(availableQuantity, Is.EqualTo(25));
        }

        [Test]
        public async Task CheckAvailableQuantityAsync_WithNoInventory_ReturnsZero()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var location = "STATION_A";

            var emptyData = new DataTable();
            emptyData.Columns.Add("Quantity", typeof(int));

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Get_CurrentQty_ByPartIDandOperation",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult 
                { 
                    Status = 1, 
                    Data = emptyData, 
                    Message = "Success" 
                });

            // Act
            var availableQuantity = await _service.CheckAvailableQuantityAsync(partId, operation, location);

            // Assert
            Assert.That(availableQuantity, Is.EqualTo(0));
        }

        [Test]
        public async Task RemoveBulkInventoryAsync_WithValidItems_CallsDatabaseForEach()
        {
            // Arrange
            var removeItems = new List<InventoryItem>
            {
                new InventoryItem { PartId = "PART001", Operation = "100", Quantity = 5, Location = "STATION_A" },
                new InventoryItem { PartId = "PART002", Operation = "110", Quantity = 3, Location = "STATION_B" },
                new InventoryItem { PartId = "PART003", Operation = "90", Quantity = 10, Location = "STATION_C" }
            };

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Remove_Item", It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            var results = await _service.RemoveBulkInventoryAsync(removeItems, "TestUser");

            // Assert
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.All(r => r.IsSuccess), Is.True);
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Remove_Item", It.IsAny<Dictionary<string, object>>()), Times.Exactly(3));
        }

        [Test]
        public async Task RemoveBulkInventoryAsync_WithMixedResults_ReturnsCorrectResults()
        {
            // Arrange
            var removeItems = new List<InventoryItem>
            {
                new InventoryItem { PartId = "PART001", Operation = "100", Quantity = 5, Location = "STATION_A" },
                new InventoryItem { PartId = "PART002", Operation = "110", Quantity = 3, Location = "STATION_B" }
            };

            _mockDatabaseService.SetupSequence(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Remove_Item", It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" })
                .ReturnsAsync(new DatabaseResult { Status = 0, Message = "Insufficient quantity" });

            // Act
            var results = await _service.RemoveBulkInventoryAsync(removeItems, "TestUser");

            // Assert
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0].IsSuccess, Is.True);
            Assert.That(results[1].IsSuccess, Is.False);
            Assert.That(results[1].ErrorMessage, Does.Contain("Insufficient quantity"));
        }

        [Test]
        public async Task ValidateRemovalAsync_WithSufficientQuantity_ReturnsTrue()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var requestedQuantity = 5;
            var location = "STATION_A";

            var mockData = new DataTable();
            mockData.Columns.Add("Quantity", typeof(int));
            mockData.Rows.Add(10); // More than requested

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Get_CurrentQty_ByPartIDandOperation",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult 
                { 
                    Status = 1, 
                    Data = mockData, 
                    Message = "Success" 
                });

            // Act
            var result = await _service.ValidateRemovalAsync(partId, operation, requestedQuantity, location);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public async Task ValidateRemovalAsync_WithInsufficientQuantity_ReturnsFalse()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var requestedQuantity = 15;
            var location = "STATION_A";

            var mockData = new DataTable();
            mockData.Columns.Add("Quantity", typeof(int));
            mockData.Rows.Add(10); // Less than requested

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Get_CurrentQty_ByPartIDandOperation",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult 
                { 
                    Status = 1, 
                    Data = mockData, 
                    Message = "Success" 
                });

            // Act
            var result = await _service.ValidateRemovalAsync(partId, operation, requestedQuantity, location);

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Insufficient quantity"));
        }

        [Test]
        public async Task GetRemovalHistoryAsync_WithValidParameters_ReturnsHistory()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var startDate = DateTime.Today.AddDays(-7);
            var endDate = DateTime.Today;

            var mockData = new DataTable();
            mockData.Columns.Add("PartID", typeof(string));
            mockData.Columns.Add("OperationNumber", typeof(string));
            mockData.Columns.Add("Quantity", typeof(int));
            mockData.Columns.Add("TransactionType", typeof(string));
            mockData.Columns.Add("User", typeof(string));
            mockData.Columns.Add("Timestamp", typeof(DateTime));

            mockData.Rows.Add("PART001", "100", 5, "OUT", "TestUser", DateTime.Now.AddDays(-1));
            mockData.Rows.Add("PART001", "100", 3, "OUT", "TestUser", DateTime.Now.AddDays(-2));

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_transaction_Get_History_ByDateRange",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult 
                { 
                    Status = 1, 
                    Data = mockData, 
                    Message = "Success" 
                });

            // Act
            var history = await _service.GetRemovalHistoryAsync(partId, operation, startDate, endDate);

            // Assert
            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history.All(h => h.PartId == partId), Is.True);
            Assert.That(history.All(h => h.Operation == operation), Is.True);
            Assert.That(history.All(h => h.TransactionType == "OUT"), Is.True);
        }

        [Test]
        public async Task GetRemovalSummaryAsync_WithValidData_ReturnsSummary()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(-30);
            var endDate = DateTime.Today;

            var mockData = new DataTable();
            mockData.Columns.Add("PartID", typeof(string));
            mockData.Columns.Add("TotalRemoved", typeof(int));
            mockData.Columns.Add("TransactionCount", typeof(int));

            mockData.Rows.Add("PART001", 50, 5);
            mockData.Rows.Add("PART002", 25, 3);
            mockData.Rows.Add("PART003", 75, 8);

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_transaction_Get_RemovalSummary",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult 
                { 
                    Status = 1, 
                    Data = mockData, 
                    Message = "Success" 
                });

            // Act
            var summary = await _service.GetRemovalSummaryAsync(startDate, endDate);

            // Assert
            Assert.That(summary.Count, Is.EqualTo(3));
            Assert.That(summary.Sum(s => s.TotalRemoved), Is.EqualTo(150));
            Assert.That(summary.Sum(s => s.TransactionCount), Is.EqualTo(16));
        }

        [Test]
        public async Task RemoveInventoryAsync_LogsSuccessfulOperation()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var quantity = 5;
            var location = "STATION_A";
            var user = "TestUser";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _service.RemoveInventoryAsync(partId, operation, quantity, location, user);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully removed inventory")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task RemoveInventoryAsync_WithException_LogsError()
        {
            // Arrange
            var partId = "PART001";
            var operation = "100";
            var quantity = 5;
            var location = "STATION_A";
            var user = "TestUser";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _service.RemoveInventoryAsync(partId, operation, quantity, location, user);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error removing inventory")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}