using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using System.Collections.ObjectModel;

namespace MTM.Tests.UnitTests.ViewModels
{
    [TestFixture]
    [Category("Unit")]
    [Category("ViewModels")]
    [Category("QuickButtons")]
    public class QuickButtonsTabViewModelTests
    {
        private QuickButtonsTabViewModel _viewModel = null!;
        private Mock<ILogger<QuickButtonsTabViewModel>> _mockLogger = null!;
        private Mock<IQuickButtonsService> _mockQuickButtonsService = null!;
        private Mock<IApplicationStateService> _mockApplicationStateService = null!;
        private Mock<IMasterDataService> _mockMasterDataService = null!;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<QuickButtonsTabViewModel>>();
            _mockQuickButtonsService = new Mock<IQuickButtonsService>();
            _mockApplicationStateService = new Mock<IApplicationStateService>();
            _mockMasterDataService = new Mock<IMasterDataService>();

            // Setup default behavior
            _mockApplicationStateService.Setup(x => x.CurrentUser).Returns("TestUser");
            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ReturnsAsync(new List<QuickButtonModel>());
            _mockMasterDataService.Setup(x => x.PartIds).Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(x => x.Operations).Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(x => x.Locations).Returns(new ObservableCollection<string>());

            _viewModel = new QuickButtonsTabViewModel(
                _mockLogger.Object,
                _mockQuickButtonsService.Object,
                _mockApplicationStateService.Object,
                _mockMasterDataService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Act & Assert
            _viewModel.Should().NotBeNull();
            _viewModel.QuickButtons.Should().NotBeNull();
            _viewModel.QuickButtons.Should().BeEmpty();
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new QuickButtonsTabViewModel(
                null!,
                _mockQuickButtonsService.Object,
                _mockApplicationStateService.Object,
                _mockMasterDataService.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*logger*");
        }

        [Test]
        public void Constructor_WithNullQuickButtonsService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new QuickButtonsTabViewModel(
                _mockLogger.Object,
                null!,
                _mockApplicationStateService.Object,
                _mockMasterDataService.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*quickButtonsService*");
        }

        #endregion

        #region Property Tests

        [Test]
        public void IsLoading_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.IsLoading))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.IsLoading = true;

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.IsLoading.Should().BeTrue();
        }

        [Test]
        public void QuickButtons_Initially_ShouldBeEmpty()
        {
            // Assert
            _viewModel.QuickButtons.Should().NotBeNull();
            _viewModel.QuickButtons.Should().BeEmpty();
        }

        [Test]
        public void StatusMessage_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.StatusMessage))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.StatusMessage = "Test Status";

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.StatusMessage.Should().Be("Test Status");
        }

        #endregion

        #region Command Tests

        [Test]
        public async Task LoadQuickButtonsCommand_WhenExecuted_ShouldCallService()
        {
            // Arrange
            var testButtons = new List<QuickButtonModel>
            {
                new() { PartId = "PART001", Operation = "100", Quantity = 5, Location = "A01" },
                new() { PartId = "PART002", Operation = "110", Quantity = 10, Location = "B02" }
            };

            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ReturnsAsync(testButtons);

            // Act
            await _viewModel.LoadQuickButtonsCommand.ExecuteAsync(null);

            // Assert
            _mockQuickButtonsService.Verify(x => x.GetQuickButtonsAsync("TestUser"), Times.Once);
            _viewModel.QuickButtons.Should().HaveCount(2);
            _viewModel.QuickButtons[0].PartId.Should().Be("PART001");
            _viewModel.QuickButtons[1].PartId.Should().Be("PART002");
        }

        [Test]
        public async Task LoadQuickButtonsCommand_WhenServiceFails_ShouldHandleErrorGracefully()
        {
            // Arrange
            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            await _viewModel.LoadQuickButtonsCommand.ExecuteAsync(null);

            // Assert
            _viewModel.QuickButtons.Should().BeEmpty();
            _viewModel.StatusMessage.Should().Contain("Failed to load");
        }

        [Test]
        public async Task ExecuteQuickButtonCommand_WithValidButton_ShouldExecuteOperation()
        {
            // Arrange
            var testButton = new QuickButtonModel
            {
                PartId = "PART001",
                Operation = "100",
                Quantity = 5,
                Location = "A01"
            };

            _mockQuickButtonsService.Setup(x => x.ExecuteQuickButtonAsync(testButton, "TestUser"))
                .ReturnsAsync(true);

            // Act
            await _viewModel.ExecuteQuickButtonCommand.ExecuteAsync(testButton);

            // Assert
            _mockQuickButtonsService.Verify(x => x.ExecuteQuickButtonAsync(testButton, "TestUser"), Times.Once);
        }

        [Test]
        public async Task ExecuteQuickButtonCommand_WithNullParameter_ShouldNotExecute()
        {
            // Act
            await _viewModel.ExecuteQuickButtonCommand.ExecuteAsync(null);

            // Assert
            _mockQuickButtonsService.Verify(x => x.ExecuteQuickButtonAsync(It.IsAny<QuickButtonModel>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task DeleteQuickButtonCommand_WithValidButton_ShouldRemoveFromCollection()
        {
            // Arrange
            var testButton = new QuickButtonModel
            {
                Id = 1,
                PartId = "PART001",
                Operation = "100",
                Quantity = 5,
                Location = "A01"
            };

            _viewModel.QuickButtons.Add(testButton);
            _mockQuickButtonsService.Setup(x => x.DeleteQuickButtonAsync(1, "TestUser"))
                .ReturnsAsync(true);

            // Act
            await _viewModel.DeleteQuickButtonCommand.ExecuteAsync(testButton);

            // Assert
            _mockQuickButtonsService.Verify(x => x.DeleteQuickButtonAsync(1, "TestUser"), Times.Once);
            _viewModel.QuickButtons.Should().NotContain(testButton);
        }

        [Test]
        public async Task RefreshCommand_WhenExecuted_ShouldReloadQuickButtons()
        {
            // Arrange
            var testButtons = new List<QuickButtonModel>
            {
                new() { PartId = "PART001", Operation = "100", Quantity = 5, Location = "A01" }
            };

            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ReturnsAsync(testButtons);

            // Act
            await _viewModel.RefreshCommand.ExecuteAsync(null);

            // Assert
            _mockQuickButtonsService.Verify(x => x.GetQuickButtonsAsync("TestUser"), Times.Once);
            _viewModel.QuickButtons.Should().HaveCount(1);
        }

        [Test]
        public async Task ClearAllCommand_WhenExecuted_ShouldClearAllQuickButtons()
        {
            // Arrange
            _viewModel.QuickButtons.Add(new QuickButtonModel { PartId = "PART001" });
            _viewModel.QuickButtons.Add(new QuickButtonModel { PartId = "PART002" });

            _mockQuickButtonsService.Setup(x => x.ClearAllQuickButtonsAsync("TestUser"))
                .ReturnsAsync(true);

            // Act
            await _viewModel.ClearAllCommand.ExecuteAsync(null);

            // Assert
            _mockQuickButtonsService.Verify(x => x.ClearAllQuickButtonsAsync("TestUser"), Times.Once);
            _viewModel.QuickButtons.Should().BeEmpty();
        }

        #endregion

        #region MTM Manufacturing Domain Tests

        [Test]
        [TestCase("PART001", true)]
        [TestCase("ABC-123", true)]
        [TestCase("TEST-999", true)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("INVALID PART ID", false)]
        public void ValidatePartId_VariousInputs_ShouldFollowMTMStandards(string partId, bool expectedValid)
        {
            // Act
            var result = _viewModel.ValidatePartId(partId);

            // Assert
            result.Should().Be(expectedValid);
        }

        [Test]
        [TestCase("90", true)]   // Receiving
        [TestCase("100", true)]  // First operation
        [TestCase("110", true)]  // Second operation
        [TestCase("120", true)]  // Final operation
        [TestCase("", false)]
        [TestCase("999", false)]
        [TestCase("ABC", false)]
        public void ValidateOperation_VariousInputs_ShouldFollowWorkflowStandards(string operation, bool expectedValid)
        {
            // Act
            var result = _viewModel.ValidateOperation(operation);

            // Assert
            result.Should().Be(expectedValid);
        }

        [Test]
        [TestCase(1, true)]
        [TestCase(10, true)]
        [TestCase(999999, true)]
        [TestCase(0, false)]
        [TestCase(-5, false)]
        public void ValidateQuantity_VariousInputs_ShouldFollowMTMStandards(int quantity, bool expectedValid)
        {
            // Act
            var result = _viewModel.ValidateQuantity(quantity);

            // Assert
            result.Should().Be(expectedValid);
        }

        #endregion

        #region Error Handling Tests

        [Test]
        public async Task LoadQuickButtonsCommand_WhenServiceReturnsNull_ShouldHandleGracefully()
        {
            // Arrange
            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ReturnsAsync((List<QuickButtonModel>?)null);

            // Act
            await _viewModel.LoadQuickButtonsCommand.ExecuteAsync(null);

            // Assert
            _viewModel.QuickButtons.Should().BeEmpty();
            _viewModel.StatusMessage.Should().NotBeEmpty();
        }

        [Test]
        public async Task ExecuteQuickButtonCommand_WhenServiceFails_ShouldShowErrorMessage()
        {
            // Arrange
            var testButton = new QuickButtonModel
            {
                PartId = "PART001",
                Operation = "100",
                Quantity = 5,
                Location = "A01"
            };

            _mockQuickButtonsService.Setup(x => x.ExecuteQuickButtonAsync(testButton, "TestUser"))
                .ThrowsAsync(new InvalidOperationException("Execution failed"));

            // Act
            await _viewModel.ExecuteQuickButtonCommand.ExecuteAsync(testButton);

            // Assert
            _viewModel.StatusMessage.Should().Contain("Failed to execute");
        }

        #endregion

        #region Integration Tests

        [Test]
        public async Task FullWorkflow_CreateExecuteDelete_ShouldWorkCorrectly()
        {
            // Arrange
            var testButton = new QuickButtonModel
            {
                Id = 1,
                PartId = "WORKFLOW_001",
                Operation = "100",
                Quantity = 15,
                Location = "STATION_A"
            };

            var initialButtons = new List<QuickButtonModel> { testButton };

            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ReturnsAsync(initialButtons);
            _mockQuickButtonsService.Setup(x => x.ExecuteQuickButtonAsync(testButton, "TestUser"))
                .ReturnsAsync(true);
            _mockQuickButtonsService.Setup(x => x.DeleteQuickButtonAsync(1, "TestUser"))
                .ReturnsAsync(true);

            // Act 1: Load buttons
            await _viewModel.LoadQuickButtonsCommand.ExecuteAsync(null);
            _viewModel.QuickButtons.Should().HaveCount(1);

            // Act 2: Execute button
            await _viewModel.ExecuteQuickButtonCommand.ExecuteAsync(testButton);

            // Act 3: Delete button
            await _viewModel.DeleteQuickButtonCommand.ExecuteAsync(testButton);

            // Assert
            _mockQuickButtonsService.Verify(x => x.GetQuickButtonsAsync("TestUser"), Times.Once);
            _mockQuickButtonsService.Verify(x => x.ExecuteQuickButtonAsync(testButton, "TestUser"), Times.Once);
            _mockQuickButtonsService.Verify(x => x.DeleteQuickButtonAsync(1, "TestUser"), Times.Once);
            _viewModel.QuickButtons.Should().BeEmpty();
        }

        #endregion

        #region Performance Tests

        [Test]
        public async Task LoadQuickButtons_WithLargeDataset_ShouldPerformEfficiently()
        {
            // Arrange
            var largeButtonList = new List<QuickButtonModel>();
            for (int i = 1; i <= 100; i++)
            {
                largeButtonList.Add(new QuickButtonModel
                {
                    Id = i,
                    PartId = $"PART{i:000}",
                    Operation = (90 + (i % 4) * 10).ToString(),
                    Quantity = i % 10 + 1,
                    Location = $"STATION_{(char)('A' + i % 5)}"
                });
            }

            _mockQuickButtonsService.Setup(x => x.GetQuickButtonsAsync("TestUser"))
                .ReturnsAsync(largeButtonList);

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await _viewModel.LoadQuickButtonsCommand.ExecuteAsync(null);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Loading 100 QuickButtons should complete within 1 second");
            _viewModel.QuickButtons.Should().HaveCount(100);
        }

        #endregion

        #region Disposal Tests

        [Test]
        public void Dispose_WhenCalled_ShouldNotThrow()
        {
            // Act & Assert
            var act = () => _viewModel.Dispose();
            act.Should().NotThrow();
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_ShouldNotThrow()
        {
            // Act & Assert
            var act = () =>
            {
                _viewModel.Dispose();
                _viewModel.Dispose();
                _viewModel.Dispose();
            };
            act.Should().NotThrow();
        }

        #endregion
    }
}