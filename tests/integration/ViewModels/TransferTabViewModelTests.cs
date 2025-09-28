using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Tests.Integration.ViewModels
{
    /// <summary>
    /// Integration tests for TransferTabViewModel with MVVM Community Toolkit.
    /// These tests MUST FAIL initially following TDD principles.
    /// Tests verify ViewModel integration with services and MVVM patterns.
    /// </summary>
    public class TransferTabViewModelTests
    {
        private readonly Mock<ILogger<TransferItemViewModel>> _mockLogger;
        private readonly Mock<ITransferService> _mockTransferService;
        private readonly Mock<IColumnConfigurationService> _mockColumnConfigService;
        private readonly Mock<IMasterDataService> _mockMasterDataService;

        public TransferTabViewModelTests()
        {
            _mockLogger = new Mock<ILogger<TransferItemViewModel>>();
            _mockTransferService = new Mock<ITransferService>();
            _mockColumnConfigService = new Mock<IColumnConfigurationService>();
            _mockMasterDataService = new Mock<IMasterDataService>();
        }

        [Fact]
        public void Constructor_WithValidDependencies_ShouldInitializeProperties()
        {
            // Arrange & Act
            // This will fail initially because TransferItemViewModel doesn't have the required constructor yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            // Assert
            Assert.NotNull(viewModel);
            Assert.NotNull(viewModel.InventoryItems);
            Assert.False(viewModel.IsLoading);
            Assert.Empty(viewModel.PartText);
            Assert.Empty(viewModel.OperationText);
            Assert.Empty(viewModel.ToLocation);
            Assert.Equal(0, viewModel.TransferQuantity);
        }

        [Fact]
        public async Task SearchCommand_WithValidInput_ShouldPopulateInventoryItems()
        {
            // Arrange
            var mockInventoryItems = new List<InventoryItem>
            {
                new InventoryItem { PartId = "TEST001", Operation = "90", FromLocation = "FLOOR", AvailableQuantity = 100 }
            };

            _mockTransferService.Setup(s => s.SearchInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync(ServiceResult<List<InventoryItem>>.Success(mockInventoryItems));

            // This will fail initially because TransferItemViewModel doesn't have SearchCommand yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            viewModel.PartText = "TEST001";
            viewModel.OperationText = "90";

            // Act
            await viewModel.SearchCommand.ExecuteAsync(null);

            // Assert
            Assert.Single(viewModel.InventoryItems);
            Assert.Equal("TEST001", viewModel.InventoryItems[0].PartId);
            Assert.False(viewModel.IsLoading);
        }

        [Fact]
        public async Task TransferCommand_WithValidSelection_ShouldExecuteTransfer()
        {
            // Arrange
            var mockTransferResult = new TransferResult
            {
                TransactionId = Guid.NewGuid().ToString(),
                WasSplit = false,
                OriginalQuantity = 50,
                TransferredQuantity = 50,
                RemainingQuantity = 0,
                Message = "Transfer completed successfully"
            };

            _mockTransferService.Setup(s => s.ExecuteTransferAsync(It.IsAny<TransferOperation>()))
                              .ReturnsAsync(ServiceResult<TransferResult>.Success(mockTransferResult));

            // This will fail initially because TransferItemViewModel doesn't have TransferCommand yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            viewModel.SelectedItem = new InventoryItem
            {
                PartId = "TEST001",
                Operation = "90",
                FromLocation = "FLOOR",
                AvailableQuantity = 50
            };
            viewModel.ToLocation = "RECEIVING";
            viewModel.TransferQuantity = 50;

            // Act
            await viewModel.TransferCommand.ExecuteAsync(null);

            // Assert
            _mockTransferService.Verify(s => s.ExecuteTransferAsync(It.IsAny<TransferOperation>()), Times.Once);
            Assert.False(viewModel.IsLoading);
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public async Task SaveColumnPreferencesCommand_WithValidConfiguration_ShouldSavePreferences()
        {
            // Arrange
            _mockColumnConfigService.Setup(s => s.SaveColumnConfigAsync(It.IsAny<string>(), It.IsAny<ColumnConfiguration>()))
                                   .ReturnsAsync(ServiceResult.Success("Configuration saved"));

            // This will fail initially because TransferItemViewModel doesn't have SaveColumnPreferencesCommand yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            viewModel.SelectedColumns = new List<string> { "PartID", "Operation", "FromLocation" };

            // Act
            await viewModel.SaveColumnPreferencesCommand.ExecuteAsync(null);

            // Assert
            _mockColumnConfigService.Verify(s => s.SaveColumnConfigAsync(It.IsAny<string>(), It.IsAny<ColumnConfiguration>()), Times.Once);
        }

        [Fact]
        public async Task EditCommand_WithSelectedItem_ShouldOpenEditDialog()
        {
            // Arrange
            // This will fail initially because TransferItemViewModel doesn't have EditCommand yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            viewModel.SelectedItem = new InventoryItem
            {
                PartId = "TEST001",
                Operation = "90",
                FromLocation = "FLOOR",
                AvailableQuantity = 100
            };

            // Act
            await viewModel.EditCommand.ExecuteAsync(null);

            // Assert
            Assert.True(viewModel.IsEditDialogVisible);
            Assert.NotNull(viewModel.EditDialogViewModel);
        }

        [Fact]
        public void ObservableProperty_PartText_ShouldNotifyPropertyChanged()
        {
            // Arrange
            // This will fail initially because TransferItemViewModel doesn't use [ObservableProperty] yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            bool propertyChanged = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.PartText))
                    propertyChanged = true;
            };

            // Act
            viewModel.PartText = "NEW_PART";

            // Assert
            Assert.True(propertyChanged);
            Assert.Equal("NEW_PART", viewModel.PartText);
        }

        [Fact]
        public void ObservableProperty_IsLoading_ShouldNotifyPropertyChanged()
        {
            // Arrange
            // This will fail initially because TransferItemViewModel doesn't use [ObservableProperty] yet
            var viewModel = new TransferItemViewModel(
                _mockLogger.Object,
                _mockTransferService.Object,
                _mockColumnConfigService.Object,
                _mockMasterDataService.Object);

            bool propertyChanged = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.IsLoading))
                    propertyChanged = true;
            };

            // Act
            viewModel.IsLoading = true;

            // Assert
            Assert.True(propertyChanged);
            Assert.True(viewModel.IsLoading);
        }
    }
}
