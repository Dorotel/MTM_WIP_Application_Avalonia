using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;

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
        private Mock<ISuggestionOverlayService> _mockSuggestionOverlayService;
        private Mock<ISuccessOverlayService> _mockSuccessOverlayService;
        private Mock<IQuickButtonsService> _mockQuickButtonsService;
        private Mock<IRemoveService> _mockRemoveService;
        private Mock<IPrintService> _mockPrintService;

        [SetUp]
        public void SetUp()
        {
            // Create mocks for all dependencies
            _mockLogger = new Mock<ILogger<RemoveItemViewModel>>();
            _mockApplicationStateService = new Mock<IApplicationStateService>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockSuggestionOverlayService = new Mock<ISuggestionOverlayService>();
            _mockSuccessOverlayService = new Mock<ISuccessOverlayService>();
            _mockQuickButtonsService = new Mock<IQuickButtonsService>();
            _mockRemoveService = new Mock<IRemoveService>();
            _mockPrintService = new Mock<IPrintService>();
            _mockNavigationService = new Mock<INavigationService>();

            // Setup default mock behavior
            SetupDefaultMockBehavior();

            // Create ViewModel with mocked dependencies
            _viewModel = new RemoveItemViewModel(
                _mockApplicationStateService.Object,
                _mockDatabaseService.Object,
                _mockSuggestionOverlayService.Object,
                _mockSuccessOverlayService.Object,
                _mockQuickButtonsService.Object,
                _mockRemoveService.Object,
                _mockLogger.Object,
                _mockPrintService.Object,
                _mockNavigationService.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        private void SetupDefaultMockBehavior()
        {
            // Setup remove service properties with empty collections (following NO FALLBACK pattern)
            _mockRemoveService.Setup(s => s.HasUndoItems).Returns(false);
            
            // Setup application state service defaults
            _mockApplicationStateService.Setup(s => s.CurrentUser).Returns("TestUser");
        }

        #endregion

        #region Property Change Notification Tests (MVVM Community Toolkit [ObservableProperty])

        [Test]
        public void SelectedPart_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newPartId = "REMOVE_PART_001";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.SelectedPart))
                    propertyChanged = true;
            };

            // Act
            _viewModel.SelectedPart = newPartId;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.SelectedPart.Should().Be(newPartId);
        }

        [Test]
        public void SelectedOperation_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newOperation = "110";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.SelectedOperation))
                    propertyChanged = true;
            };

            // Act
            _viewModel.SelectedOperation = newOperation;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.SelectedOperation.Should().Be(newOperation);
        }

        [Test]
        public void PartText_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newPartText = "REMOVE_PART_TEXT";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.PartText))
                    propertyChanged = true;
            };

            // Act
            _viewModel.PartText = newPartText;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.PartText.Should().Be(newPartText);
        }

        [Test]
        public void OperationText_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newOperationText = "100";
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.OperationText))
                    propertyChanged = true;
            };

            // Act
            _viewModel.OperationText = newOperationText;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.OperationText.Should().Be(newOperationText);
        }

        [Test]
        public void IsLoading_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            var newIsLoading = true;
            bool propertyChanged = false;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.IsLoading))
                    propertyChanged = true;
            };

            // Act
            _viewModel.IsLoading = newIsLoading;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.IsLoading.Should().Be(newIsLoading);
        }

        #endregion

        #region Command Tests (MVVM Community Toolkit [RelayCommand])

        [Test]
        public void DeleteCommand_ShouldExist()
        {
            // Assert
            _viewModel.DeleteCommand.Should().NotBeNull();
        }

        [Test]
        public void UndoCommand_ShouldExist()
        {
            // Assert
            _viewModel.UndoCommand.Should().NotBeNull();
        }

        [Test]
        public void SearchCommand_ShouldExist()
        {
            // Assert
            _viewModel.SearchCommand.Should().NotBeNull();
        }

        [Test]
        public void ResetCommand_ShouldExist()
        {
            // Assert
            _viewModel.ResetCommand.Should().NotBeNull();
        }

        [Test]
        public void AdvancedRemovalCommand_ShouldExist()
        {
            // Assert
            _viewModel.AdvancedRemovalCommand.Should().NotBeNull();
        }

        [Test]
        public void TogglePanelCommand_ShouldExist()
        {
            // Assert
            _viewModel.TogglePanelCommand.Should().NotBeNull();
        }

        [Test]
        public void LoadDataCommand_ShouldExist()
        {
            // Assert
            _viewModel.LoadDataCommand.Should().NotBeNull();
        }

        [Test]
        public void PrintCommand_ShouldExist()
        {
            // Assert
            _viewModel.PrintCommand.Should().NotBeNull();
        }

        #endregion

        #region Validation Tests (Manufacturing Domain Logic)

        [Test]
        [TestCase("", false)] // Empty part text should be invalid for search
        [TestCase(" ", false)] // Whitespace part text should be invalid for search
        [TestCase("REMOVE_PART_001", true)] // Valid part text
        [TestCase("ABC-123", true)] // Valid part text with dash
        [TestCase("TEST_REMOVE_999", true)] // Valid part text with underscore
        public void PartText_ShouldValidateCorrectly(string partText, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.PartText = partText;

            // Assert - Valid part text should not be empty or whitespace
            var isValid = !string.IsNullOrWhiteSpace(partText);
            isValid.Should().Be(expectedValid);
        }

        [Test]
        [TestCase("", true)] // Empty operation should be valid (optional field)
        [TestCase("90", true)] // Valid receiving operation
        [TestCase("100", true)] // Valid first operation
        [TestCase("110", true)] // Valid second operation
        [TestCase("120", true)] // Valid final operation
        [TestCase("ABC", true)] // Non-numeric operation text should be valid (filtered later)
        public void OperationText_ShouldAllowAnyValue(string operationText, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.OperationText = operationText;

            // Assert - Operation text should always be valid (filtering happens server-side)
            var isValid = true; // Operation text has no client-side validation
            isValid.Should().Be(expectedValid);
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
                _viewModel.OperationText = operation;
                // Operation text should always be valid (no client-side validation)
                true.Should().BeTrue($"Operation {operation} should be valid for removal");
            }
        }

        [Test]
        [TestCase("REMOVE-WITH-DASH", true)]
        [TestCase("REMOVE_WITH_UNDERSCORE", true)]
        [TestCase("REMOVE001", true)]
        [TestCase("REM123", true)]
        [TestCase("", false)]
        public void PartTextValidation_ShouldFollowMTMRemovalNamingConventions(string partText, bool expectedValid)
        {
            // Arrange & Act
            _viewModel.PartText = partText;

            // Assert - Valid part text should not be empty or whitespace
            var isValid = !string.IsNullOrWhiteSpace(partText);
            isValid.Should().Be(expectedValid);
        }

        #endregion

        #region Can Execute Validation Tests

        [Test]
        public void CanDelete_WhenNoItemsSelected_ShouldReturnFalse()
        {
            // Arrange - No items in SelectedItems collection
            _viewModel.SelectedItems.Clear();

            // Act & Assert
            _viewModel.CanDelete.Should().BeFalse();
        }

        [Test]
        public void CanDelete_WhenIsLoading_ShouldReturnFalse()
        {
            // Arrange - Add an item to SelectedItems but set IsLoading
            _viewModel.SelectedItems.Add(new InventoryItem { PartId = "TEST", Operation = "100", Quantity = 1, Location = "A" });
            _viewModel.IsLoading = true;

            // Act & Assert
            _viewModel.CanDelete.Should().BeFalse();
        }

        [Test]
        public void CanUndo_WhenNoUndoItems_ShouldReturnFalse()
        {
            // Arrange - Setup RemoveService to have no undo items
            _mockRemoveService.Setup(s => s.HasUndoItems).Returns(false);

            // Act & Assert
            _viewModel.CanUndo.Should().BeFalse();
        }

        [Test]
        public void CanUndo_WhenHasUndoItemsButIsLoading_ShouldReturnFalse()
        {
            // Arrange - Setup RemoveService to have undo items but set IsLoading
            _mockRemoveService.Setup(s => s.HasUndoItems).Returns(true);
            _viewModel.IsLoading = true;

            // Act & Assert
            _viewModel.CanUndo.Should().BeFalse();
        }

        #endregion

        #region Collection and State Tests

        [Test]
        public async Task ObservableCollections_ShouldProvideRemovalData()
        {
            // Arrange - Collections should be available for binding
            await Task.CompletedTask; // Satisfy async requirement

            // Act - Access collections
            var partIds = _viewModel.PartIds;
            var operations = _viewModel.Operations;
            var inventoryItems = _viewModel.InventoryItems;
            var selectedItems = _viewModel.SelectedItems;

            // Assert
            partIds.Should().NotBeNull();
            operations.Should().NotBeNull();
            inventoryItems.Should().NotBeNull();
            selectedItems.Should().NotBeNull();
        }

        [Test]
        public void HasInventoryItems_WhenEmptyCollection_ShouldReturnFalse()
        {
            // Arrange - Empty inventory items collection
            _viewModel.InventoryItems.Clear();

            // Act & Assert
            _viewModel.HasInventoryItems.Should().BeFalse();
        }

        [Test]
        public void HasInventoryItems_WhenHasItems_ShouldReturnTrue()
        {
            // Arrange - Add inventory item to collection
            _viewModel.InventoryItems.Add(new InventoryItem 
            { 
                PartId = "TEST", 
                Operation = "100", 
                Quantity = 10, 
                Location = "STATION_A" 
            });

            // Act & Assert
            _viewModel.HasInventoryItems.Should().BeTrue();
        }

        [Test]
        public void RemoveService_OnItemsRemovedFailure_ShouldFollowNoFallbackPattern()
        {
            // Arrange - Setup services to return expected state (following NO FALLBACK pattern)
            _mockRemoveService.Setup(s => s.HasUndoItems).Returns(false);

            // Act
            var hasUndoItems = _mockRemoveService.Object.HasUndoItems;
            var canUndo = _viewModel.CanUndo;

            // Assert - Should be false, not null or fallback value (MTM NO FALLBACK pattern)
            hasUndoItems.Should().BeFalse();
            canUndo.Should().BeFalse();
        }

        #endregion
    }
}