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
    [Category("AddItem")]
    public class AddItemViewModelTests
    {
        private AddItemViewModel _viewModel;
        private Mock<ILogger<AddItemViewModel>> _mockLogger;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IApplicationStateService> _mockApplicationStateService;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IMasterDataService> _mockMasterDataService;
        private Mock<ISuggestionOverlayService> _mockSuggestionOverlayService;
        private Mock<ISuccessOverlayService> _mockSuccessOverlayService;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<AddItemViewModel>>();
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

            _viewModel = new AddItemViewModel(
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
            Assert.That(_viewModel.Operation, Is.Empty);
            Assert.That(_viewModel.Quantity, Is.EqualTo(1));
            Assert.That(_viewModel.Location, Is.Empty);
            Assert.That(_viewModel.IsLoading, Is.False);
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AddItemViewModel(
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
                if (args.PropertyName == nameof(AddItemViewModel.PartId))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.PartId = "PART001";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.PartId, Is.EqualTo("PART001"));
        }

        [Test]
        public void Operation_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AddItemViewModel.Operation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.Operation = "100";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.Operation, Is.EqualTo("100"));
        }

        [Test]
        public void Quantity_WhenSetToValidValue_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AddItemViewModel.Quantity))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.Quantity = 10;

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.Quantity, Is.EqualTo(10));
        }

        [Test]
        public void Location_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AddItemViewModel.Location))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.Location = "STATION_A";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.Location, Is.EqualTo("STATION_A"));
        }

        [Test]
        public void IsLoading_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var propertyChangedRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AddItemViewModel.IsLoading))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.IsLoading = true;

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(_viewModel.IsLoading, Is.True);
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
        public async Task SaveInventoryAsync_WithValidData_CallsDatabaseService()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";

            var expectedParameters = new Dictionary<string, object>
            {
                ["p_PartID"] = "PART001",
                ["p_OperationNumber"] = "100",
                ["p_Quantity"] = 5,
                ["p_Location"] = "STATION_A",
                ["p_User"] = Environment.UserName
            };

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Add_Item",
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _viewModel.SaveInventoryAsync();

            // Assert
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                "inv_inventory_Add_Item",
                It.Is<Dictionary<string, object>>(p => 
                    p["p_PartID"].ToString() == "PART001" &&
                    p["p_OperationNumber"].ToString() == "100" &&
                    Convert.ToInt32(p["p_Quantity"]) == 5 &&
                    p["p_Location"].ToString() == "STATION_A")), 
                Times.Once);
        }

        [Test]
        public async Task SaveInventoryAsync_WithEmptyPartId_DoesNotCallDatabase()
        {
            // Arrange
            _viewModel.PartId = "";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";

            // Act
            await _viewModel.SaveInventoryAsync();

            // Assert
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()), 
                Times.Never);
        }

        [Test]
        public async Task SaveInventoryAsync_WithZeroQuantity_DoesNotCallDatabase()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 0;
            _viewModel.Location = "STATION_A";

            // Act
            await _viewModel.SaveInventoryAsync();

            // Assert
            _mockDatabaseService.Verify(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()), 
                Times.Never);
        }

        [Test]
        public async Task SaveInventoryAsync_DatabaseSuccess_ShowsSuccessMessage()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _viewModel.SaveInventoryAsync();

            // Assert
            _mockSuccessOverlayService.Verify(s => s.ShowAsync(
                It.IsAny<string>(), It.IsAny<TimeSpan>()), 
                Times.Once);
        }

        [Test]
        public async Task SaveInventoryAsync_DatabaseFailure_LogsError()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 0, Message = "Database Error" });

            // Act
            await _viewModel.SaveInventoryAsync();

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to save inventory")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void ResetForm_ClearsAllFields()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 10;
            _viewModel.Location = "STATION_A";

            // Act
            _viewModel.ResetForm();

            // Assert
            Assert.That(_viewModel.PartId, Is.Empty);
            Assert.That(_viewModel.Operation, Is.Empty);
            Assert.That(_viewModel.Quantity, Is.EqualTo(1));
            Assert.That(_viewModel.Location, Is.Empty);
        }

        [Test]
        public void CanSave_WithValidData_ReturnsTrue()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";
            _viewModel.IsLoading = false;

            // Act
            var canSave = _viewModel.CanSave;

            // Assert
            Assert.That(canSave, Is.True);
        }

        [Test]
        public void CanSave_WithEmptyPartId_ReturnsFalse()
        {
            // Arrange
            _viewModel.PartId = "";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";
            _viewModel.IsLoading = false;

            // Act
            var canSave = _viewModel.CanSave;

            // Assert
            Assert.That(canSave, Is.False);
        }

        [Test]
        public void CanSave_WhenLoading_ReturnsFalse()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";
            _viewModel.IsLoading = true;

            // Act
            var canSave = _viewModel.CanSave;

            // Assert
            Assert.That(canSave, Is.False);
        }

        [Test]
        public async Task SaveInventoryAsync_SetsLoadingState_DuringOperation()
        {
            // Arrange
            _viewModel.PartId = "PART001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 5;
            _viewModel.Location = "STATION_A";

            var loadingStates = new List<bool>();
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AddItemViewModel.IsLoading))
                    loadingStates.Add(_viewModel.IsLoading);
            };

            _mockDatabaseService.Setup(s => s.ExecuteStoredProcedureAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });

            // Act
            await _viewModel.SaveInventoryAsync();

            // Assert
            Assert.That(loadingStates.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(loadingStates.First(), Is.True, "Should set loading to true at start");
            Assert.That(loadingStates.Last(), Is.False, "Should set loading to false at end");
        }
    }
}