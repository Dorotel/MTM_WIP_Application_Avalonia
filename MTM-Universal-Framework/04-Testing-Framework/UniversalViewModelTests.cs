using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Universal ViewModel test patterns for MVVM Community Toolkit ViewModels.
    /// Provides common testing utilities for any business domain ViewModels.
    /// </summary>
    public abstract class UniversalViewModelTestBase<TViewModel> : UniversalTestBase
        where TViewModel : ObservableObject
    {
        protected TViewModel ViewModel { get; private set; }
        protected Mock<TViewModel> MockViewModel { get; private set; }

        protected override async Task SetupAsync()
        {
            await base.SetupAsync();
            ViewModel = CreateViewModel();
            MockViewModel = new Mock<TViewModel>();
        }

        /// <summary>
        /// Override this method to create the ViewModel instance with required dependencies
        /// </summary>
        protected abstract TViewModel CreateViewModel();

        /// <summary>
        /// Tests that ObservableProperty attributes generate proper property change notifications
        /// </summary>
        protected void TestObservableProperty<TProperty>(
            Func<TViewModel, TProperty> propertyGetter,
            Action<TViewModel, TProperty> propertySetter,
            TProperty testValue,
            string propertyName)
        {
            // Arrange
            var propertyChangedFired = false;
            string changedPropertyName = null;

            ViewModel.PropertyChanged += (sender, e) =>
            {
                propertyChangedFired = true;
                changedPropertyName = e.PropertyName;
            };

            // Act
            propertySetter(ViewModel, testValue);

            // Assert
            Assert.True(propertyChangedFired, $"PropertyChanged event should fire for {propertyName}");
            Assert.Equal(propertyName, changedPropertyName);
            Assert.Equal(testValue, propertyGetter(ViewModel));
        }

        /// <summary>
        /// Tests that RelayCommand attributes generate proper command implementations
        /// </summary>
        protected async Task TestRelayCommandAsync(
            Func<TViewModel, IAsyncRelayCommand> commandGetter,
            Func<Task> setupAction = null,
            Func<Task> verifyAction = null)
        {
            // Arrange
            if (setupAction != null)
                await setupAction();

            var command = commandGetter(ViewModel);
            Assert.NotNull(command);

            // Act
            var canExecuteBefore = command.CanExecute(null);
            await command.ExecuteAsync(null);
            var canExecuteAfter = command.CanExecute(null);

            // Assert - Commands should be executable by default unless explicitly disabled
            Assert.True(canExecuteBefore, "Command should be executable initially");

            if (verifyAction != null)
                await verifyAction();
        }

        /// <summary>
        /// Tests command CanExecute logic changes based on ViewModel state
        /// </summary>
        protected void TestCommandCanExecuteLogic<TProperty>(
            Func<TViewModel, IRelayCommand> commandGetter,
            Func<TViewModel, TProperty> propertyGetter,
            Action<TViewModel, TProperty> propertySetter,
            TProperty enableValue,
            TProperty disableValue)
        {
            // Arrange
            var command = commandGetter(ViewModel);

            // Test with disable value
            propertySetter(ViewModel, disableValue);
            var canExecuteWhenDisabled = command.CanExecute(null);

            // Test with enable value
            propertySetter(ViewModel, enableValue);
            var canExecuteWhenEnabled = command.CanExecute(null);

            // Assert
            Assert.False(canExecuteWhenDisabled, "Command should be disabled with disable value");
            Assert.True(canExecuteWhenEnabled, "Command should be enabled with enable value");
        }

        /// <summary>
        /// Tests validation logic in ViewModels
        /// </summary>
        protected void TestValidationLogic<TProperty>(
            Func<TViewModel, TProperty> propertyGetter,
            Action<TViewModel, TProperty> propertySetter,
            TProperty validValue,
            TProperty invalidValue,
            string propertyName)
        {
            // Test valid value
            propertySetter(ViewModel, validValue);
            var hasErrorsWithValidValue = ViewModel.HasErrors;

            // Test invalid value
            propertySetter(ViewModel, invalidValue);
            var hasErrorsWithInvalidValue = ViewModel.HasErrors;

            // Assert
            Assert.False(hasErrorsWithValidValue, $"Should not have errors with valid value for {propertyName}");
            Assert.True(hasErrorsWithInvalidValue, $"Should have errors with invalid value for {propertyName}");
        }

        protected override async Task TearDownAsync()
        {
            await base.TearDownAsync();
            ViewModel = null;
            MockViewModel = null;
        }
    }

    /// <summary>
    /// Sample ViewModel test implementation showing how to use the base class
    /// </summary>
    public class SampleViewModelTests : UniversalViewModelTestBase<SampleViewModel>
    {
        protected override SampleViewModel CreateViewModel()
        {
            return new SampleViewModel();
        }

        [Fact]
        public void TestName_PropertyChangeNotification_ShouldFireCorrectly()
        {
            TestObservableProperty(
                vm => vm.Name,
                (vm, value) => vm.Name = value,
                "Test Name",
                nameof(SampleViewModel.Name)
            );
        }

        [Fact]
        public async Task TestSaveCommand_Execution_ShouldCompleteSuccessfully()
        {
            await TestRelayCommandAsync(
                vm => vm.SaveCommand,
                setupAction: async () =>
                {
                    ViewModel.Name = "Test Name";
                    await Task.CompletedTask;
                },
                verifyAction: async () =>
                {
                    Assert.False(ViewModel.IsBusy);
                    await Task.CompletedTask;
                }
            );
        }

        [Fact]
        public void TestSaveCommand_CanExecute_ShouldRespectNameProperty()
        {
            TestCommandCanExecuteLogic(
                vm => vm.SaveCommand,
                vm => vm.Name,
                (vm, value) => vm.Name = value,
                enableValue: "Valid Name",
                disableValue: ""
            );
        }
    }

    /// <summary>
    /// Sample ViewModel for demonstration purposes
    /// </summary>
    [ObservableObject]
    public partial class SampleViewModel
    {
        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private bool isBusy = false;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            IsBusy = true;
            try
            {
                // Simulate save operation
                await Task.Delay(100);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSave => !string.IsNullOrWhiteSpace(Name) && !IsBusy;
    }
}