using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using System.IO;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Universal test base class providing common testing infrastructure for any business domain.
    /// Includes service mocking, configuration setup, and cross-platform testing utilities.
    /// </summary>
    public abstract class UniversalTestBase : IDisposable
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IServiceCollection Services { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        protected Mock<ILogger> MockLogger { get; private set; }
        protected bool IsDisposed { get; private set; }
        protected string TestDataDirectory { get; private set; }

        protected UniversalTestBase()
        {
            Services = new ServiceCollection();
            MockLogger = new Mock<ILogger>();
            Configuration = CreateTestConfiguration();
            TestDataDirectory = CreateTestDataDirectory();
            
            ConfigureTestServices();
            ServiceProvider = Services.BuildServiceProvider();
        }

        /// <summary>
        /// Creates a test-specific data directory for file operations.
        /// </summary>
        protected virtual string CreateTestDataDirectory()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "UniversalFrameworkTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDir);
            return testDir;
        }

        /// <summary>
        /// Creates test configuration with common settings.
        /// </summary>
        protected virtual IConfiguration CreateTestConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            
            var testSettings = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",
                ["Logging:LogLevel:Default"] = "Debug",
                ["Testing:Environment"] = "Test",
                ["Testing:UseInMemoryDatabase"] = "true",
                ["Application:Name"] = "UniversalFrameworkTest",
                ["Application:Version"] = "1.0.0-test"
            };

            configBuilder.AddInMemoryCollection(testSettings);
            return configBuilder.Build();
        }

        /// <summary>
        /// Configure common test services. Override to add domain-specific services.
        /// </summary>
        protected virtual void ConfigureTestServices()
        {
            // Add logging
            Services.AddSingleton(MockLogger.Object);
            Services.AddSingleton<ILoggerFactory>(_ => new Mock<ILoggerFactory>().Object);

            // Add configuration
            Services.AddSingleton(Configuration);

            // Add common test services
            Services.AddTransient<TestDataGenerator>();
            Services.AddTransient<TestAssertionHelper>();
        }

        /// <summary>
        /// Creates a scoped service provider for isolated testing.
        /// </summary>
        protected IServiceScope CreateScope()
        {
            return ServiceProvider.CreateScope();
        }

        /// <summary>
        /// Gets a service from the test container with null safety.
        /// </summary>
        protected T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>() ?? throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }

        /// <summary>
        /// Gets a required service from the test container.
        /// </summary>
        protected T GetRequiredService<T>() where T : notnull
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Asserts that an async operation completes within the specified timeout.
        /// </summary>
        protected async Task AssertCompletesWithinAsync(Func<Task> operation, TimeSpan timeout, string? message = null)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                await operation().WaitAsync(cts.Token);
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                throw new TimeoutException(message ?? $"Operation did not complete within {timeout}");
            }
        }

        /// <summary>
        /// Creates a temporary file for testing and ensures cleanup.
        /// </summary>
        protected string CreateTempFile(string content = "", string extension = ".tmp")
        {
            var filePath = Path.Combine(TestDataDirectory, $"test_{Guid.NewGuid():N}{extension}");
            File.WriteAllText(filePath, content);
            return filePath;
        }

        /// <summary>
        /// Verifies mock interactions and resets mocks for clean state.
        /// </summary>
        protected void VerifyAndResetMocks()
        {
            MockLogger.VerifyAll();
            MockLogger.Reset();
        }

        public virtual void Dispose()
        {
            if (IsDisposed) return;

            try
            {
                ServiceProvider?.Dispose();
                
                // Cleanup test data directory
                if (Directory.Exists(TestDataDirectory))
                {
                    Directory.Delete(TestDataDirectory, recursive: true);
                }
            }
            catch (Exception ex)
            {
                // Log cleanup errors but don't fail tests
                Console.WriteLine($"Warning: Test cleanup failed: {ex.Message}");
            }
            finally
            {
                IsDisposed = true;
            }
        }
    }

    /// <summary>
    /// Helper class for generating test data across different domains.
    /// </summary>
    public class TestDataGenerator
    {
        private readonly Random _random = new();

        public string GenerateString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public int GenerateInt(int min = 0, int max = 1000) => _random.Next(min, max);

        public DateTime GenerateDateTime() => DateTime.Now.AddDays(_random.Next(-365, 365));

        public T GenerateEnum<T>() where T : struct, Enum
        {
            var values = Enum.GetValues<T>();
            return values[_random.Next(values.Length)];
        }
    }

    /// <summary>
    /// Helper class for common test assertions.
    /// </summary>
    public class TestAssertionHelper
    {
        public void AssertPropertyChanged<T>(T obj, string propertyName, Action action) where T : INotifyPropertyChanged
        {
            var eventRaised = false;
            obj.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                    eventRaised = true;
            };

            action();

            Assert.True(eventRaised, $"PropertyChanged event was not raised for property {propertyName}");
        }

        public async Task AssertThrowsAsync<TException>(Func<Task> action, string? expectedMessage = null) where TException : Exception
        {
            var exception = await Assert.ThrowsAsync<TException>(action);
            
            if (expectedMessage != null)
            {
                Assert.Contains(expectedMessage, exception.Message);
            }
        }

        public void AssertCollectionsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string? message = null)
        {
            Assert.True(expected.SequenceEqual(actual), message ?? "Collections are not equal");
        }
            ServiceProvider = Services.BuildServiceProvider();
        }

        /// <summary>
        /// Override this method to configure domain-specific services for testing
        /// </summary>
        protected virtual void ConfigureTestServices()
        {
            // Add universal test services
            Services.AddSingleton(Configuration);
            Services.AddSingleton(MockLogger.Object);
            Services.AddLogging(builder => builder.AddConsole());
        }

        /// <summary>
        /// Creates test configuration with common test values
        /// </summary>
        protected virtual IConfiguration CreateTestConfiguration()
        {
            var configData = new Dictionary<string, string>
            {
                {"ApplicationName", "UniversalFramework.Tests"},
                {"Environment", "Testing"},
                {"ConnectionStrings:DefaultConnection", "Data Source=:memory:"},
                {"Logging:LogLevel:Default", "Debug"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }

        /// <summary>
        /// Helper method to create mock services
        /// </summary>
        protected Mock<T> CreateMockService<T>() where T : class
        {
            var mock = new Mock<T>();
            Services.AddSingleton(mock.Object);
            return mock;
        }

        /// <summary>
        /// Helper method for async test setup
        /// </summary>
        protected virtual async Task SetupAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Helper method for async test cleanup
        /// </summary>
        protected virtual async Task TearDownAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Validates service result patterns commonly used in the framework
        /// </summary>
        protected void AssertServiceResult<T>(ServiceResult<T> result, bool shouldSucceed, string expectedMessage = null)
        {
            if (shouldSucceed)
            {
                Assert.True(result.IsSuccess, $"Expected success but got: {result.ErrorMessage}");
                Assert.False(string.IsNullOrEmpty(result.Message));
            }
            else
            {
                Assert.False(result.IsSuccess, "Expected failure but operation succeeded");
                Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
            }

            if (!string.IsNullOrEmpty(expectedMessage))
            {
                var messageToCheck = shouldSucceed ? result.Message : result.ErrorMessage;
                Assert.Contains(expectedMessage, messageToCheck);
            }
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                ServiceProvider?.Dispose();
                IsDisposed = true;
            }
        }
    }

    /// <summary>
    /// Service result class matching the framework patterns
    /// </summary>
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public Exception Exception { get; set; }

        public static ServiceResult<T> Success(T data, string message = "")
            => new() { IsSuccess = true, Data = data, Message = message };

        public static ServiceResult<T> Failure(string errorMessage, Exception exception = null)
            => new() { IsSuccess = false, ErrorMessage = errorMessage, Exception = exception };
    }
}