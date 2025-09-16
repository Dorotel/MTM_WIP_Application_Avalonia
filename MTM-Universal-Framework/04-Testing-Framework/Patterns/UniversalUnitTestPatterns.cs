using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Moq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM.UniversalFramework.Testing.Patterns;

/// <summary>
/// Comprehensive unit testing patterns for MTM Universal Framework applications.
/// Provides standardized testing approaches for ViewModels, Services, and Business Logic.
/// </summary>
public static class UniversalUnitTestPatterns
{
    /// <summary>
    /// Base test class for ViewModel unit tests using MVVM Community Toolkit.
    /// </summary>
    public abstract class ViewModelTestBase<TViewModel> where TViewModel : ObservableObject
    {
        protected TViewModel ViewModel { get; private set; }
        protected Mock<ILogger<TViewModel>> MockLogger { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public virtual async Task SetUp()
        {
            MockLogger = new Mock<ILogger<TViewModel>>();
            
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            ViewModel = CreateViewModel();
            await InitializeViewModelAsync();
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            if (ViewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (ServiceProvider is IDisposable serviceDisposable)
            {
                serviceDisposable.Dispose();
            }

            await CleanupAsync();
        }

        protected abstract TViewModel CreateViewModel();
        protected abstract void ConfigureServices(IServiceCollection services);
        protected virtual async Task InitializeViewModelAsync() => await Task.CompletedTask;
        protected virtual async Task CleanupAsync() => await Task.CompletedTask;

        /// <summary>
        /// Validates that ObservableProperty triggers PropertyChanged events.
        /// </summary>
        protected void AssertPropertyChanged<T>(Action<T> propertySet, string propertyName, T value) where T : notnull
        {
            var propertyChangedRaised = false;
            
            ViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == propertyName)
                    propertyChangedRaised = true;
            };

            propertySet(value);

            Assert.That(propertyChangedRaised, Is.True, 
                $"PropertyChanged should be raised for {propertyName}");
        }

        /// <summary>
        /// Validates command execution and state management.
        /// </summary>
        protected async Task AssertCommandExecution(IAsyncRelayCommand command, 
            Func<Task> setup = null, 
            Func<Task> verification = null,
            bool shouldExecute = true)
        {
            // Setup
            if (setup != null)
                await setup();

            // Verify CanExecute
            Assert.That(command.CanExecute(null), Is.EqualTo(shouldExecute),
                $"Command CanExecute should return {shouldExecute}");

            if (shouldExecute)
            {
                // Execute command
                await command.ExecuteAsync(null);

                // Verify results
                if (verification != null)
                    await verification();
            }
        }
    }

    /// <summary>
    /// Base test class for Service unit tests with dependency injection.
    /// </summary>
    public abstract class ServiceTestBase<TService> where TService : class
    {
        protected TService Service { get; private set; }
        protected Mock<ILogger<TService>> MockLogger { get; private set; }
        protected IServiceProvider ServiceProvider { get; private set; }

        [SetUp]
        public virtual async Task SetUp()
        {
            MockLogger = new Mock<ILogger<TService>>();
            
            var services = new ServiceCollection();
            services.AddSingleton(MockLogger.Object);
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            Service = CreateService();
            await InitializeServiceAsync();
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            if (Service is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (ServiceProvider is IDisposable serviceDisposable)
            {
                serviceDisposable.Dispose();
            }

            await CleanupAsync();
        }

        protected abstract TService CreateService();
        protected abstract void ConfigureServices(IServiceCollection services);
        protected virtual async Task InitializeServiceAsync() => await Task.CompletedTask;
        protected virtual async Task CleanupAsync() => await Task.CompletedTask;

        /// <summary>
        /// Validates service method execution with exception handling.
        /// </summary>
        protected async Task<TResult> AssertServiceMethodExecution<TResult>(
            Func<Task<TResult>> serviceMethod,
            bool shouldSucceed = true,
            Type expectedExceptionType = null)
        {
            if (shouldSucceed)
            {
                var result = await serviceMethod();
                Assert.That(result, Is.Not.Null, "Service method should return a result");
                return result;
            }
            else
            {
                if (expectedExceptionType != null)
                {
                    Assert.ThrowsAsync(expectedExceptionType, async () => await serviceMethod(),
                        $"Service method should throw {expectedExceptionType.Name}");
                }
                else
                {
                    Assert.ThrowsAsync<Exception>(async () => await serviceMethod(),
                        "Service method should throw an exception");
                }

                return default(TResult);
            }
        }

        /// <summary>
        /// Verifies that logging occurred with specific level and message pattern.
        /// </summary>
        protected void VerifyLogging(LogLevel level, string messagePattern)
        {
            MockLogger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(messagePattern)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce,
                $"Expected logging at {level} level with message containing '{messagePattern}'");
        }
    }

    /// <summary>
    /// Test patterns for business logic validation and domain rules.
    /// </summary>
    public abstract class BusinessLogicTestBase
    {
        /// <summary>
        /// Validates business rule enforcement with multiple test cases.
        /// </summary>
        protected void AssertBusinessRule<T>(
            Func<T, bool> businessRule,
            IEnumerable<(T input, bool expectedResult, string description)> testCases)
        {
            foreach (var testCase in testCases)
            {
                var actualResult = businessRule(testCase.input);
                Assert.That(actualResult, Is.EqualTo(testCase.expectedResult),
                    $"Business rule validation failed for: {testCase.description}");
            }
        }

        /// <summary>
        /// Validates data transformation operations.
        /// </summary>
        protected void AssertDataTransformation<TInput, TOutput>(
            Func<TInput, TOutput> transformation,
            IEnumerable<(TInput input, TOutput expected, string description)> testCases,
            IEqualityComparer<TOutput> comparer = null)
        {
            foreach (var testCase in testCases)
            {
                var actualResult = transformation(testCase.input);
                
                if (comparer != null)
                {
                    Assert.That(comparer.Equals(actualResult, testCase.expected), Is.True,
                        $"Data transformation failed for: {testCase.description}");
                }
                else
                {
                    Assert.That(actualResult, Is.EqualTo(testCase.expected),
                        $"Data transformation failed for: {testCase.description}");
                }
            }
        }
    }

    /// <summary>
    /// Test patterns for validation logic and input sanitization.
    /// </summary>
    public static class ValidationTestPatterns
    {
        /// <summary>
        /// Tests validation rules with comprehensive edge cases.
        /// </summary>
        public static void AssertValidationRule<T>(
            Func<T, ValidationResult> validator,
            T validInput,
            IEnumerable<(T input, string expectedError)> invalidInputs)
        {
            // Test valid input
            var validResult = validator(validInput);
            Assert.That(validResult.IsValid, Is.True, 
                "Valid input should pass validation");

            // Test invalid inputs
            foreach (var invalidCase in invalidInputs)
            {
                var invalidResult = validator(invalidCase.input);
                Assert.That(invalidResult.IsValid, Is.False,
                    $"Invalid input should fail validation: {invalidCase.expectedError}");
                Assert.That(invalidResult.ErrorMessage, Does.Contain(invalidCase.expectedError),
                    "Validation error message should contain expected text");
            }
        }

        /// <summary>
        /// Tests input sanitization and normalization.
        /// </summary>
        public static void AssertInputSanitization<T>(
            Func<T, T> sanitizer,
            IEnumerable<(T input, T expected, string description)> testCases)
        {
            foreach (var testCase in testCases)
            {
                var result = sanitizer(testCase.input);
                Assert.That(result, Is.EqualTo(testCase.expected),
                    $"Input sanitization failed for: {testCase.description}");
            }
        }
    }

    /// <summary>
    /// Validation result for testing validation logic.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ValidationResult Success() => new() { IsValid = true };
        public static ValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
    }

    /// <summary>
    /// Performance testing utilities for unit tests.
    /// </summary>
    public static class PerformanceTestUtilities
    {
        /// <summary>
        /// Measures execution time and validates performance requirements.
        /// </summary>
        public static async Task AssertPerformance<T>(
            Func<Task<T>> operation,
            TimeSpan maxExecutionTime,
            string operationName = "Operation")
        {
            var startTime = DateTime.UtcNow;
            var result = await operation();
            var executionTime = DateTime.UtcNow - startTime;

            Assert.That(executionTime, Is.LessThan(maxExecutionTime),
                $"{operationName} took {executionTime.TotalMilliseconds:F2}ms, " +
                $"which exceeds maximum allowed time of {maxExecutionTime.TotalMilliseconds:F2}ms");

            Assert.That(result, Is.Not.Null, $"{operationName} should return a result");
        }

        /// <summary>
        /// Validates memory usage during operation execution.
        /// </summary>
        public static async Task AssertMemoryUsage<T>(
            Func<Task<T>> operation,
            long maxMemoryIncreaseBytes = 10 * 1024 * 1024, // 10MB default
            string operationName = "Operation")
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var initialMemory = GC.GetTotalMemory(false);
            var result = await operation();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(false);
            var memoryIncrease = finalMemory - initialMemory;

            Assert.That(memoryIncrease, Is.LessThan(maxMemoryIncreaseBytes),
                $"{operationName} increased memory usage by {memoryIncrease:N0} bytes, " +
                $"which exceeds maximum allowed increase of {maxMemoryIncreaseBytes:N0} bytes");

            Assert.That(result, Is.Not.Null, $"{operationName} should return a result");
        }
    }
}