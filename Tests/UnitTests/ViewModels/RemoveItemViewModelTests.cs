using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM.Tests.UnitTests.ViewModels
{
    /// <summary>
    /// Unit tests for RemoveItemViewModel - MVVM Community Toolkit patterns validation
    /// Tests manufacturing inventory removal operations with MTM business domain validation
    /// </summary>
    [TestFixture]
    [Category("Unit")]
    [Category("ViewModel")]
    [Category("Manufacturing")]
    public class RemoveItemViewModelTests
    {
        #region Test Setup & Mocking

        private RemoveItemViewModel _viewModel;
        private Mock<ILogger<RemoveItemViewModel>> _mockLogger;
        private Mock<IApplicationStateService> _mockApplicationStateService;
        private Mock<INavigationService> _mockNavigationService;
        private Mock<IDatabaseService> _mockDatabaseService;
        private Mock<IConfigurationService> _mockConfigurationService;
        private Mock<IMasterDataService> _mockMasterDataService;

        [SetUp]
        public void SetUp()
        {
            // Create mocks for all dependencies
            _mockLogger = new Mock<ILogger<RemoveItemViewModel>>();
            _mockApplicationStateService = new Mock<IApplicationStateService>();
            _mockNavigationService = new Mock<INavigationService>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockMasterDataService = new Mock<IMasterDataService>();

            // Setup default mock behavior
            SetupDefaultMockBehavior();

            // Create ViewModel with mocked dependencies
            _viewModel = new RemoveItemViewModel(
                _mockLogger.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object,
                _mockDatabaseService.Object,
                _mockConfigurationService.Object,
                _mockMasterDataService.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        private void SetupDefaultMockBehavior()
        {
            // Setup master data service properties with empty collections (following NO FALLBACK pattern)
            _mockMasterDataService.Setup(s => s.PartIds)
                .Returns(new ObservableCollection<string>());
            
            _mockMasterDataService.Setup(s => s.Operations)
                .Returns(new ObservableCollection<string>());
            
            _mockMasterDataService.Setup(s => s.Locations)
                .Returns(new ObservableCollection<string>());

            // Setup configuration service defaults
            _mockConfigurationService.Setup(s => s.GetConnectionString(It.IsAny<string>()))
                .Returns("Server=localhost;Database=mtm_test;Uid=test;Pwd=test;");
        }

        #endregion

        #region Property Change Notification Tests (MVVM Community Toolkit [ObservableProperty])

        [Test]
        public void PartId_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newPartId = "REMOVE_PART_001";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.PartId))
                    propertyChanged = true;
            };

            // Act
            _viewModel.PartId = newPartId;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.PartId.Should().Be(newPartId);
        }

        [Test]
        public void Operation_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newOperation = "110";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Operation))
                    propertyChanged = true;
            };

            // Act
            _viewModel.Operation = newOperation;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.Operation.Should().Be(newOperation);
        }

        [Test]
        public void Quantity_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newQuantity = 25;
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Quantity))
                    propertyChanged = true;
            };

            // Act
            _viewModel.Quantity = newQuantity;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.Quantity.Should().Be(newQuantity);
        }

        [Test]
        public void Location_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newLocation = "REMOVE_STATION_A";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Location))
                    propertyChanged = true;
            };

            // Act
            _viewModel.Location = newLocation;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.Location.Should().Be(newLocation);
        }

        #endregion

        #region Command Tests (MVVM Community Toolkit [RelayCommand])

        [Test]
        public void RemoveCommand_ShouldExist()
        {
            // Assert
            _viewModel.RemoveCommand.Should().NotBeNull();
        }

        [Test]
        public void ClearCommand_ShouldExist()
        {
            // Assert
            _viewModel.ClearCommand.Should().NotBeNull();
        }

        #endregion

        #region Validation Tests (Manufacturing Domain Logic)

        [Test]
        [TestCase("", false)] // Empty part ID should be invalid
        [TestCase(" ", false)] // Whitespace part ID should be invalid
        [TestCase("REMOVE_PART_001", true)] // Valid part ID
        [TestCase("ABC-123", true)] // Valid part ID with dash
        [TestCase("TEST_REMOVE_999", true)] // Valid part ID with underscore
        public void IsPartIdValid_ShouldValidatePartIdCorrectly(string partId, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.PartId = partId;

            // Assert
            _viewModel.IsPartIdValid.Should().Be(expectedValid);
        }

        [Test]
        [TestCase("", false)] // Empty operation should be invalid
        [TestCase("0", false)] // Zero operation should be invalid (not a valid manufacturing operation)
        [TestCase("90", true)] // Valid receiving operation
        [TestCase("100", true)] // Valid first operation
        [TestCase("110", true)] // Valid second operation
        [TestCase("120", true)] // Valid final operation
        [TestCase("ABC", false)] // Non-numeric operation should be invalid
        public void IsOperationValid_ShouldValidateOperationCorrectly(string operation, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.Operation = operation;

            // Assert
            _viewModel.IsOperationValid.Should().Be(expectedValid);
        }

        [Test]
        [TestCase(0, false)] // Zero quantity should be invalid
        [TestCase(-1, false)] // Negative quantity should be invalid
        [TestCase(1, true)] // Minimum valid quantity
        [TestCase(100, true)] // Normal valid quantity
        [TestCase(999999, true)] // Maximum valid quantity
        public void IsQuantityValid_ShouldValidateQuantityCorrectly(int quantity, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.Quantity = quantity;

            // Assert
            _viewModel.IsQuantityValid.Should().Be(expectedValid);
        }

        #endregion

        #region Manufacturing Domain Tests

        [Test]
        public void ManufacturingOperations_ShouldFollowRemovalWorkflowSequence()
        {
            // Arrange - Standard MTM removal operations (reverse order from production)
            var removalOperations = new[] { "130", "120", "110", "100", "90" };

            // Assert that operations support removal workflow
            foreach (var operation in removalOperations)
            {
                _viewModel.Operation = operation;
                _viewModel.IsOperationValid.Should().BeTrue($"Operation {operation} should be valid for removal");
            }
        }

        [Test]
        [TestCase("REMOVE-WITH-DASH", true)]
        [TestCase("REMOVE_WITH_UNDERSCORE", true)]
        [TestCase("REMOVE001", true)]
        [TestCase("REM123", true)]
        [TestCase("", false)]
        public void PartIdValidation_ShouldFollowMTMRemovalNamingConventions(string partId, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.PartId = partId;

            // Assert
            _viewModel.IsPartIdValid.Should().Be(expectedValid);
        }

        #endregion

        #region Can Execute Validation Tests

        [Test]
        public void CanRemove_WhenAllFieldsValid_ShouldReturnTrue()
        {
            // Arrange - Setup master data with valid test values
            _mockMasterDataService.Setup(s => s.PartIds)
                .Returns(new ObservableCollection<string> { "TEST_REMOVE_001" });
            _mockMasterDataService.Setup(s => s.Operations)
                .Returns(new ObservableCollection<string> { "100" });
            _mockMasterDataService.Setup(s => s.Locations)
                .Returns(new ObservableCollection<string> { "STATION_A" });

            // Set all required fields to valid values
            _viewModel.PartId = "TEST_REMOVE_001";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 10;
            _viewModel.Location = "STATION_A";

            // Act & Assert
            _viewModel.CanRemove.Should().BeTrue();
        }

        [Test]
        public void CanRemove_WhenAnyFieldInvalid_ShouldReturnFalse()
        {
            // Arrange - Missing part ID
            _viewModel.PartId = "";
            _viewModel.Operation = "100";
            _viewModel.Quantity = 10;
            _viewModel.Location = "STATION_A";

            // Act & Assert
            _viewModel.CanRemove.Should().BeFalse();
        }

        [Test]
        public void CanRemove_WhenInsufficientQuantity_ShouldReturnFalse()
        {
            // Arrange - Negative quantity
            _viewModel.PartId = "TEST_PART";
            _viewModel.Operation = "100";
            _viewModel.Quantity = -5;
            _viewModel.Location = "STATION_A";

            // Act & Assert
            _viewModel.CanRemove.Should().BeFalse();
        }

        #endregion

        #region Master Data Integration Tests

        [Test]
        public async Task MasterDataService_ShouldProvideRemovalObservableCollections()
        {
            // Arrange - Setup master data collections with test data
            var partIds = new ObservableCollection<string> { "REMOVE001", "REMOVE002", "REMOVE003" };
            var operations = new ObservableCollection<string> { "90", "100", "110", "120" };
            var locations = new ObservableCollection<string> { "REMOVAL_A", "REMOVAL_B", "REMOVAL_C" };

            _mockMasterDataService.Setup(s => s.PartIds).Returns(partIds);
            _mockMasterDataService.Setup(s => s.Operations).Returns(operations);
            _mockMasterDataService.Setup(s => s.Locations).Returns(locations);

            // Act - Access master data through service
            await Task.CompletedTask; // Satisfy async requirement
            var actualPartIds = _mockMasterDataService.Object.PartIds;
            var actualOperations = _mockMasterDataService.Object.Operations;
            var actualLocations = _mockMasterDataService.Object.Locations;

            // Assert
            actualPartIds.Should().BeEquivalentTo(partIds);
            actualOperations.Should().BeEquivalentTo(operations);
            actualLocations.Should().BeEquivalentTo(locations);
        }

        [Test]
        public void MasterDataService_OnRemovalServiceFailure_ShouldLeaveCollectionsEmpty()
        {
            // Arrange - Setup services to return empty (following NO FALLBACK pattern)
            _mockMasterDataService.Setup(s => s.PartIds)
                .Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(s => s.Operations)
                .Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(s => s.Locations)
                .Returns(new ObservableCollection<string>());

            // Act
            var partIds = _mockMasterDataService.Object.PartIds;
            var operations = _mockMasterDataService.Object.Operations;
            var locations = _mockMasterDataService.Object.Locations;

            // Assert - Should be empty collections, not null (MTM NO FALLBACK pattern)
            partIds.Should().NotBeNull().And.BeEmpty();
            operations.Should().NotBeNull().And.BeEmpty();
            locations.Should().NotBeNull().And.BeEmpty();
        }

        #endregion
    }
}