using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Tests.Integration.Services
{
    /// <summary>
    /// Integration tests for column configuration persistence with MySQL.
    /// These tests MUST FAIL initially following TDD principles.
    /// Tests verify end-to-end MySQL usr_ui_settings integration.
    /// </summary>
    public class ColumnConfigurationIntegrationTests : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IColumnConfigurationService _columnConfigService;
        private readonly string _testUserId = $"test_user_{Guid.NewGuid():N}";

        public ColumnConfigurationIntegrationTests()
        {
            // Setup test service container
            var services = new ServiceCollection();

            // This will fail initially because services aren't registered yet
            services.AddLogging();
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IColumnConfigurationService, TransferColumnConfigurationService>();

            _serviceProvider = services.BuildServiceProvider();
            _columnConfigService = _serviceProvider.GetRequiredService<IColumnConfigurationService>();
        }

        [Fact]
        public async Task SaveAndLoadColumnConfiguration_WithValidData_ShouldPersistInDatabase()
        {
            // Arrange
            var configuration = new ColumnConfiguration
            {
                VisibleColumns = new List<string> { "PartID", "Operation", "FromLocation", "AvailableQuantity" },
                ColumnOrder = new Dictionary<string, int>
                {
                    { "PartID", 0 }, { "Operation", 1 }, { "FromLocation", 2 }, { "AvailableQuantity", 3 }
                },
                ColumnWidths = new Dictionary<string, int>
                {
                    { "PartID", 150 }, { "Operation", 90 }, { "FromLocation", 110 }, { "AvailableQuantity", 130 }
                }
            };

            // Act
            var saveResult = await _columnConfigService.SaveColumnConfigAsync(_testUserId, configuration);
            var loadResult = await _columnConfigService.LoadColumnConfigAsync(_testUserId);

            // Assert
            Assert.True(saveResult.IsSuccess);
            Assert.True(loadResult.IsSuccess);
            Assert.NotNull(loadResult.Data);
            Assert.Equal(4, loadResult.Data.VisibleColumns.Count);
            Assert.Contains("PartID", loadResult.Data.VisibleColumns);
            Assert.Contains("Operation", loadResult.Data.VisibleColumns);
            Assert.Equal(150, loadResult.Data.ColumnWidths["PartID"]);
            Assert.Equal(90, loadResult.Data.ColumnWidths["Operation"]);
        }

        [Fact]
        public async Task LoadColumnConfiguration_ForNewUser_ShouldReturnDefaults()
        {
            // Arrange
            var newUserId = $"new_user_{Guid.NewGuid():N}";

            // Act
            var result = await _columnConfigService.LoadColumnConfigAsync(newUserId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(6, result.Data.VisibleColumns.Count);
            Assert.Contains("PartID", result.Data.VisibleColumns);
            Assert.Contains("Operation", result.Data.VisibleColumns);
            Assert.Contains("FromLocation", result.Data.VisibleColumns);
            Assert.Contains("AvailableQuantity", result.Data.VisibleColumns);
            Assert.Contains("TransferQuantity", result.Data.VisibleColumns);
            Assert.Contains("Notes", result.Data.VisibleColumns);
        }

        [Fact]
        public async Task ResetToDefaults_WithExistingConfiguration_ShouldRestoreDefaults()
        {
            // Arrange
            var customConfig = new ColumnConfiguration
            {
                VisibleColumns = new List<string> { "PartID", "Operation" },
                ColumnOrder = new Dictionary<string, int> { { "PartID", 0 }, { "Operation", 1 } },
                ColumnWidths = new Dictionary<string, int> { { "PartID", 200 }, { "Operation", 150 } }
            };

            await _columnConfigService.SaveColumnConfigAsync(_testUserId, customConfig);

            // Act
            var resetResult = await _columnConfigService.ResetToDefaultsAsync(_testUserId);
            var loadResult = await _columnConfigService.LoadColumnConfigAsync(_testUserId);

            // Assert
            Assert.True(resetResult.IsSuccess);
            Assert.True(loadResult.IsSuccess);
            Assert.Equal(6, loadResult.Data.VisibleColumns.Count);
            Assert.Equal(120, loadResult.Data.ColumnWidths["PartID"]); // Default width
            Assert.Equal(80, loadResult.Data.ColumnWidths["Operation"]); // Default width
        }

        [Fact]
        public async Task SaveColumnConfiguration_WithInvalidData_ShouldReturnFailure()
        {
            // Arrange
            ColumnConfiguration? nullConfig = null;

            // Act
            var result = await _columnConfigService.SaveColumnConfigAsync(_testUserId, nullConfig!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("invalid", result.Message.ToLower());
        }

        [Fact]
        public async Task MultipleSaves_ForSameUser_ShouldUpdateExistingRecord()
        {
            // Arrange
            var config1 = new ColumnConfiguration
            {
                VisibleColumns = new List<string> { "PartID", "Operation" },
                ColumnOrder = new Dictionary<string, int> { { "PartID", 0 }, { "Operation", 1 } },
                ColumnWidths = new Dictionary<string, int> { { "PartID", 100 }, { "Operation", 80 } }
            };

            var config2 = new ColumnConfiguration
            {
                VisibleColumns = new List<string> { "PartID", "Operation", "FromLocation" },
                ColumnOrder = new Dictionary<string, int> { { "PartID", 0 }, { "Operation", 1 }, { "FromLocation", 2 } },
                ColumnWidths = new Dictionary<string, int> { { "PartID", 120 }, { "Operation", 90 }, { "FromLocation", 110 } }
            };

            // Act
            await _columnConfigService.SaveColumnConfigAsync(_testUserId, config1);
            await _columnConfigService.SaveColumnConfigAsync(_testUserId, config2);
            var loadResult = await _columnConfigService.LoadColumnConfigAsync(_testUserId);

            // Assert
            Assert.True(loadResult.IsSuccess);
            Assert.Equal(3, loadResult.Data.VisibleColumns.Count); // Should have latest config
            Assert.Contains("FromLocation", loadResult.Data.VisibleColumns);
            Assert.Equal(120, loadResult.Data.ColumnWidths["PartID"]);
        }

        [Fact]
        public void GetDefaultConfiguration_ShouldReturnValidDefaults()
        {
            // Act
            var config = _columnConfigService.GetDefaultConfiguration();

            // Assert
            Assert.NotNull(config);
            Assert.Equal(6, config.VisibleColumns.Count);
            Assert.Equal(6, config.ColumnOrder.Count);
            Assert.Equal(6, config.ColumnWidths.Count);

            // Verify all default columns are present
            var expectedColumns = new[] { "PartID", "Operation", "FromLocation", "AvailableQuantity", "TransferQuantity", "Notes" };
            foreach (var column in expectedColumns)
            {
                Assert.Contains(column, config.VisibleColumns);
                Assert.True(config.ColumnOrder.ContainsKey(column));
                Assert.True(config.ColumnWidths.ContainsKey(column));
            }
        }

        public void Dispose()
        {
            // Cleanup test data
            try
            {
                // This cleanup will also fail initially since the service doesn't exist
                _columnConfigService?.ResetToDefaultsAsync(_testUserId).Wait();
            }
            catch
            {
                // Ignore cleanup errors during testing
            }

            _serviceProvider?.Dispose();
        }
    }
}
