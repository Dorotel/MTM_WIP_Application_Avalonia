using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Linq;

namespace MTM.Tests.UnitTests.ViewModels
{
    [TestFixture]
    [Category("Unit")]
    [Category("ViewModel")]
    [Category("TransferItem")]
    public class TransferItemViewModelTests
    {
        private TransferItemViewModel _viewModel;
        private Mock<ILogger<TransferItemViewModel>> _mockLogger;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IApplicationStateService> _mockApplicationStateService;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IMasterDataService> _mockMasterDataService;
        private Mock<ISuggestionOverlayService> _mockSuggestionOverlayService;
        private Mock<ISuccessOverlayService> _mockSuccessOverlayService;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<TransferItemViewModel>>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockApplicationStateService = new Mock<IApplicationStateService>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockMasterDataService = new Mock<IMasterDataService>();
            _mockSuggestionOverlayService = new Mock<ISuggestionOverlayService>();
            _mockSuccessOverlayService = new Mock<ISuccessOverlayService>();

            // Setup default return values
            _mockMasterDataService.Setup(s => s.GetPartIdsAsync())
                .ReturnsAsync(new ObservableCollection<string> { "PART001", "PART002", "PART003" });
            _mockMasterDataService.Setup(s => s.GetOperationsAsync())
                .ReturnsAsync(new ObservableCollection<string> { "90", "100", "110", "120" });
            _mockMasterDataService.Setup(s => s.GetLocationsAsync())
                .ReturnsAsync(new ObservableCollection<string> { "STATION_A", "STATION_B", "STATION_C" });

            _viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockConfigurationService.Object,
                _mockApplicationStateService.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockSuggestionOverlayService.Object,
                _mockSuccessOverlayService.Object
            );
        }

        [Test]
        public void Constructor_WithValidParameters_InitializesCorrectly()
        {
            // Assert
            Assert.That(_viewModel, Is.Not.Null);
            Assert.That(_viewModel.PartId, Is.Empty);
            Assert.That(_viewModel.FromOperation, Is.Empty);
            Assert.That(_viewModel.ToOperation, Is.Empty);
            Assert.That(_viewModel.Quantity, Is.EqualTo(1));
            Assert.That(_viewModel.FromLocation, Is.Empty);
            Assert.That(_viewModel.ToLocation, Is.Empty);
            Assert.That(_viewModel.IsLoading, Is.False);
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TransferItemViewModel(
                null,
                _mockConfigurationService.Object,
                _mockApplicationStateService.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockSuggestionOverlayService.Object,
                _mockSuccessOverlayService.Object
            ));
        }

        [Test]
        public void PartId_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TransferItemViewModel.PartId))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.PartId = "PART001";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.PartId, Is.EqualTo("PART001"));
        }

        [Test]
        public void FromOperation_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TransferItemViewModel.FromOperation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.FromOperation = "100";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.FromOperation, Is.EqualTo("100"));
        }

        [Test]
        public void ToOperation_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TransferItemViewModel.ToOperation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.ToOperation = "110";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.ToOperation, Is.EqualTo("110"));
        }

        [Test]
        public void FromLocation_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TransferItemViewModel.FromLocation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.FromLocation = "STATION_A";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.FromLocation, Is.EqualTo("STATION_A"));
        }

        [Test]
        public void ToLocation_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TransferItemViewModel.ToLocation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.ToLocation = "STATION_B";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.ToLocation, Is.EqualTo("STATION_B"));
        }

        [Test]
        public async Task LoadMasterDataAsync_ShouldLoadAllRequiredData()
        {
            // Act
            await _viewModel.LoadMasterDataAsync();

            // Assert
            Assert.That(_viewModel.PartIds.Count, Is.EqualTo(3));
            Assert.That(_viewModel.Operations.Count, Is.EqualTo(4));
            Assert.That(_viewModel.Locations.Count, Is.EqualTo(3));
            
            Assert.That(_viewModel.PartIds.Contains("PART001"), Is.True);
            Assert.That(_viewModel.Operations.Contains("100"), Is.True);
            Assert.That(_viewModel.Locations.Contains("STATION_A"), Is.True);
        }

        [Test]
        public async Task TransferInventoryAsync_WithValidData_CallsDatabaseService()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Transfer_Between_Operations",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _viewModel.TransferInventoryAsync();

            // Assert
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Transfer_Between_Operations",
                It.Is<Dictionary<string, object>>(p => 
                    p["p_PartID"].ToString() == "PART001" &&
                    p["p_FromOperationNumber"].ToString() == "100" &&
                    p["p_ToOperationNumber"].ToString() == "110" &&
                    Convert.ToInt32(p["p_Quantity"]) == 5 &&
                    p["p_FromLocation"].ToString() == "STATION_A" &&
                    p["p_ToLocation"].ToString() == "STATION_B")), 
                Times.Once);
        }

        [Test]
        public async Task TransferInventoryAsync_WithSameFromToOperation_DoesNotCallDatabase()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "100"; // Same as from
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";

            // Act
            await _viewModel.TransferInventoryAsync();

            // Assert
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()), 
                Times.Never);
        }

        [Test]
        public async Task TransferInventoryAsync_WithSameFromToLocation_DoesNotCallDatabase()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_A"; // Same as from

            // Act
            await _viewModel.TransferInventoryAsync();

            // Assert
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()), 
                Times.Never);
        }

        [Test]
        public async Task TransferInventoryAsync_DatabaseSuccess_ShowsSuccessMessage()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _viewModel.TransferInventoryAsync();

            // Assert
            _mockSuccessOverlayService.Verify(s => s.ShowAsync(
                It.IsAny<string>(), It.IsAny<TimeSpan>()), 
                Times.Once);
        }

        [Test]
        public async Task TransferInventoryAsync_DatabaseFailure_LogsError()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 0, Message = "Database Error" });

            // Act
            await _viewModel.TransferInventoryAsync();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to transfer inventory")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void ResetForm_ClearsAllFields()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 10;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";

            // Act
            _viewModel.ResetForm();

            // Assert
            Assert.That(_viewModel.PartId, Is.Empty);
            Assert.That(_viewModel.FromOperation, Is.Empty);
            Assert.That(_viewModel.ToOperation, Is.Empty);
            Assert.That(_viewModel.Quantity, Is.EqualTo(1));
            Assert.That(_viewModel.FromLocation, Is.Empty);
            Assert.That(_viewModel.ToLocation, Is.Empty);
        }

        [Test]
        public void CanTransfer_WithValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";
            _viewModel.IsLoading = false;

            // Act
            var canTransfer = _viewModel.CanTransfer;

            // Assert
            Assert.That(canTransfer, Is.True);
        }

        [Test]
        public void CanTransfer_WithEmptyPartId_ReturnsFalse()
        {
            // Arrange
            _viewModel.PartId = "";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";
            _viewModel.IsLoading = false;

            // Act
            var canTransfer = _viewModel.CanTransfer;

            // Assert
            Assert.That(canTransfer, Is.False);
        }

        [Test]
        public void CanTransfer_WhenLoading_ReturnsFalse()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";
            _viewModel.IsLoading = true;

            // Act
            var canTransfer = _viewModel.CanTransfer;

            // Assert
            Assert.That(canTransfer, Is.False);
        }

        [Test]
        public async Task CheckAvailableQuantityAsync_WithValidPartAndOperation_ReturnsQuantity()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";

            var mockData = new System.Data.DataTable();
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
            var availableQuantity = await _viewModel.CheckAvailableQuantityAsync();

            // Assert
            Assert.That(availableQuantity, Is.EqualTo(25));
        }

        [Test]
        public async Task CheckAvailableQuantityAsync_WithNoInventory_ReturnsZero()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";

            var emptyData = new System.Data.DataTable();
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
            var availableQuantity = await _viewModel.CheckAvailableQuantityAsync();

            // Assert
            Assert.That(availableQuantity, Is.EqualTo(0));
        }

        [Test]
        public async Task TransferInventoryAsync_SetsLoadingState_DuringOperation()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.FromOperation = "100";
            _viewModel.ToOperation = "110";
            _viewModel.Quantity = 5;
            _viewModel.FromLocation = "STATION_A";
            _viewModel.ToLocation = "STATION_B";

            var loadingStates = new List<bool>();
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TransferItemViewModel.IsLoading))
                    loadingStates.Add(_viewModel.IsLoading);
            };

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _viewModel.TransferInventoryAsync();

            // Assert
            Assert.That(loadingStates.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(loadingStates.First(), Is.True, "Should set loading to true at start");
            Assert.That(loadingStates.Last(), Is.False, "Should set loading to false at end");
        }

        [Test]
        public void ValidateTransferOperations_WithValidSequence_ReturnsTrue()
        {
            // Test normal manufacturing workflow progression
            var validTransfers = new[]
            {
                new { From = "90", To = "100" },   // Receiving to First Operation
                new { From = "100", To = "110" },  // First to Second Operation  
                new { From = "110", To = "120" },  // Second to Final Operation
                new { From = "120", To = "130" }   // Final to Shipping
            };

            foreach (var transfer in validTransfers)
            {
                // Arrange
                _viewModel.FromOperation = transfer.From;
                _viewModel.ToOperation = transfer.To;

                // Act
                var isValidSequence = _viewModel.ValidateTransferOperations();

                // Assert
                Assert.That(isValidSequence, Is.True, 
                    $"Transfer from {transfer.From} to {transfer.To} should be valid");
            }
        }

        [Test]
        public void ValidateTransferOperations_WithInvalidBackwardsSequence_ReturnsFalse()
        {
            // Test invalid backwards progression
            var invalidTransfers = new[]
            {
                new { From = "100", To = "90" },   // First back to Receiving
                new { From = "110", To = "100" },  // Second back to First
                new { From = "120", To = "110" }   // Final back to Second
            };

            foreach (var transfer in invalidTransfers)
            {
                // Arrange
                _viewModel.FromOperation = transfer.From;
                _viewModel.ToOperation = transfer.To;

                // Act
                var isValidSequence = _viewModel.ValidateTransferOperations();

                // Assert
                Assert.That(isValidSequence, Is.False, 
                    $"Backwards transfer from {transfer.From} to {transfer.To} should be invalid");
            }
        }
    }
}