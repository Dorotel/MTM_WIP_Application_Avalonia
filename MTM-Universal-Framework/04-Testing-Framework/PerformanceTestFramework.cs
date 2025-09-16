using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Performance testing framework with load testing capabilities and benchmark validation.
    /// Provides tools for testing application performance under various load conditions.
    /// </summary>
    public class PerformanceTestFramework : UniversalTestBase
    {
        protected TimeSpan DefaultTimeout => TimeSpan.FromSeconds(30);
        protected int DefaultIterations => 100;
        protected Dictionary<string, PerformanceMetrics> BenchmarkResults { get; private set; }

        protected PerformanceTestFramework() : base()
        {
            BenchmarkResults = new Dictionary<string, PerformanceMetrics>();
        }

        /// <summary>
        /// Execute performance test with specified parameters
        /// </summary>
        protected async Task<PerformanceMetrics> ExecutePerformanceTestAsync<T>(
            string testName,
            Func<Task<T>> operation,
            int iterations = 0,
            TimeSpan? timeout = null)
        {
            iterations = iterations == 0 ? DefaultIterations : iterations;
            timeout = timeout ?? DefaultTimeout;

            var metrics = new PerformanceMetrics { TestName = testName };
            var stopwatch = new Stopwatch();
            var results = new List<long>();
            var exceptions = new List<Exception>();

            using var cancellationTokenSource = new CancellationTokenSource(timeout.Value);

            try
            {
                // Warm-up run
                await operation();

                // Performance test runs
                for (int i = 0; i < iterations; i++)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    try
                    {
                        stopwatch.Restart();
                        await operation();
                        stopwatch.Stop();
                        results.Add(stopwatch.ElapsedMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                // Calculate metrics
                if (results.Count > 0)
                {
                    results.Sort();
                    metrics.TotalExecutions = results.Count;
                    metrics.SuccessfulExecutions = results.Count;
                    metrics.FailedExecutions = exceptions.Count;
                    metrics.AverageExecutionTime = CalculateAverage(results);
                    metrics.MinExecutionTime = results[0];
                    metrics.MaxExecutionTime = results[results.Count - 1];
                    metrics.MedianExecutionTime = CalculateMedian(results);
                    metrics.PercentileP95 = CalculatePercentile(results, 95);
                    metrics.PercentileP99 = CalculatePercentile(results, 99);
                    metrics.StandardDeviation = CalculateStandardDeviation(results, metrics.AverageExecutionTime);
                }

                metrics.Exceptions = exceptions;
                BenchmarkResults[testName] = metrics;

                return metrics;
            }
            catch (OperationCanceledException)
            {
                metrics.TimedOut = true;
                return metrics;
            }
        }

        /// <summary>
        /// Execute concurrent load test
        /// </summary>
        protected async Task<LoadTestResults> ExecuteLoadTestAsync<T>(
            string testName,
            Func<Task<T>> operation,
            int concurrentUsers,
            TimeSpan duration,
            TimeSpan? rampUpTime = null)
        {
            rampUpTime = rampUpTime ?? TimeSpan.FromSeconds(10);

            var loadResults = new LoadTestResults
            {
                TestName = testName,
                ConcurrentUsers = concurrentUsers,
                Duration = duration,
                StartTime = DateTime.UtcNow
            };

            var tasks = new List<Task>();
            var userMetrics = new List<UserMetrics>();
            var overallStopwatch = Stopwatch.StartNew();

            using var cancellationTokenSource = new CancellationTokenSource(duration);

            // Ramp up users gradually
            var rampUpDelay = rampUpTime.Value.TotalMilliseconds / concurrentUsers;

            for (int userId = 0; userId < concurrentUsers; userId++)
            {
                var userMetric = new UserMetrics { UserId = userId };
                userMetrics.Add(userMetric);

                var userTask = SimulateUserLoadAsync(operation, userMetric, cancellationTokenSource.Token);
                tasks.Add(userTask);

                if (rampUpDelay > 0)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(rampUpDelay));
                }
            }

            // Wait for all user simulations to complete
            await Task.WhenAll(tasks);
            overallStopwatch.Stop();

            // Calculate load test results
            loadResults.ActualDuration = overallStopwatch.Elapsed;
            loadResults.TotalRequests = userMetrics.Sum(u => u.TotalRequests);
            loadResults.SuccessfulRequests = userMetrics.Sum(u => u.SuccessfulRequests);
            loadResults.FailedRequests = userMetrics.Sum(u => u.FailedRequests);
            loadResults.RequestsPerSecond = loadResults.TotalRequests / loadResults.ActualDuration.TotalSeconds;
            loadResults.AverageResponseTime = userMetrics.Average(u => u.AverageResponseTime);
            loadResults.UserMetrics = userMetrics;

            return loadResults;
        }

        /// <summary>
        /// Simulate individual user load
        /// </summary>
        private async Task SimulateUserLoadAsync<T>(
            Func<Task<T>> operation,
            UserMetrics userMetric,
            CancellationToken cancellationToken)
        {
            var responseTimes = new List<long>();
            var stopwatch = new Stopwatch();

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        stopwatch.Restart();
                        await operation();
                        stopwatch.Stop();

                        userMetric.TotalRequests++;
                        userMetric.SuccessfulRequests++;
                        responseTimes.Add(stopwatch.ElapsedMilliseconds);
                    }
                    catch
                    {
                        userMetric.TotalRequests++;
                        userMetric.FailedRequests++;
                    }

                    // Small delay between requests to simulate real user behavior
                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when test duration expires
            }

            if (responseTimes.Count > 0)
            {
                userMetric.AverageResponseTime = CalculateAverage(responseTimes);
                userMetric.MinResponseTime = responseTimes.Min();
                userMetric.MaxResponseTime = responseTimes.Max();
            }
        }

        /// <summary>
        /// Validate performance benchmarks
        /// </summary>
        protected void ValidatePerformanceBenchmarks(PerformanceMetrics metrics, PerformanceBenchmarks benchmarks)
        {
            Assert.True(metrics.AverageExecutionTime <= benchmarks.MaxAverageExecutionTime,
                $"Average execution time {metrics.AverageExecutionTime}ms exceeds benchmark {benchmarks.MaxAverageExecutionTime}ms");

            Assert.True(metrics.PercentileP95 <= benchmarks.MaxP95ExecutionTime,
                $"P95 execution time {metrics.PercentileP95}ms exceeds benchmark {benchmarks.MaxP95ExecutionTime}ms");

            Assert.True(metrics.SuccessRate >= benchmarks.MinSuccessRate,
                $"Success rate {metrics.SuccessRate:P} is below benchmark {benchmarks.MinSuccessRate:P}");
        }

        #region Helper Methods

        private double CalculateAverage(List<long> values)
        {
            return values.Count > 0 ? values.Average() : 0;
        }

        private long CalculateMedian(List<long> sortedValues)
        {
            if (sortedValues.Count == 0) return 0;
            int mid = sortedValues.Count / 2;
            return sortedValues.Count % 2 == 0
                ? (sortedValues[mid - 1] + sortedValues[mid]) / 2
                : sortedValues[mid];
        }

        private long CalculatePercentile(List<long> sortedValues, int percentile)
        {
            if (sortedValues.Count == 0) return 0;
            int index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Count) - 1;
            return sortedValues[Math.Max(0, Math.Min(index, sortedValues.Count - 1))];
        }

        private double CalculateStandardDeviation(List<long> values, double average)
        {
            if (values.Count <= 1) return 0;
            var variance = values.Average(v => Math.Pow(v - average, 2));
            return Math.Sqrt(variance);
        }

        #endregion
    }

    /// <summary>
    /// Performance metrics container
    /// </summary>
    public class PerformanceMetrics
    {
        public string TestName { get; set; }
        public int TotalExecutions { get; set; }
        public int SuccessfulExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public double AverageExecutionTime { get; set; }
        public long MinExecutionTime { get; set; }
        public long MaxExecutionTime { get; set; }
        public long MedianExecutionTime { get; set; }
        public long PercentileP95 { get; set; }
        public long PercentileP99 { get; set; }
        public double StandardDeviation { get; set; }
        public bool TimedOut { get; set; }
        public List<Exception> Exceptions { get; set; } = new List<Exception>();

        public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions : 0;
    }

    /// <summary>
    /// Load test results container
    /// </summary>
    public class LoadTestResults
    {
        public string TestName { get; set; }
        public int ConcurrentUsers { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan ActualDuration { get; set; }
        public DateTime StartTime { get; set; }
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double RequestsPerSecond { get; set; }
        public double AverageResponseTime { get; set; }
        public List<UserMetrics> UserMetrics { get; set; } = new List<UserMetrics>();

        public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests : 0;
    }

    /// <summary>
    /// Individual user metrics in load test
    /// </summary>
    public class UserMetrics
    {
        public int UserId { get; set; }
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public long MinResponseTime { get; set; }
        public long MaxResponseTime { get; set; }
    }

    /// <summary>
    /// Performance benchmarks for validation
    /// </summary>
    public class PerformanceBenchmarks
    {
        public double MaxAverageExecutionTime { get; set; }
        public long MaxP95ExecutionTime { get; set; }
        public double MinSuccessRate { get; set; }
    }

    /// <summary>
    /// Example performance test implementation
    /// </summary>
    public class ExamplePerformanceTests : PerformanceTestFramework
    {
        [Fact]
        public async Task BusinessOperation_Performance_ShouldMeetBenchmarks()
        {
            // Arrange
            var benchmarks = new PerformanceBenchmarks
            {
                MaxAverageExecutionTime = 100, // 100ms
                MaxP95ExecutionTime = 200,     // 200ms
                MinSuccessRate = 0.99          // 99%
            };

            // Act
            var metrics = await ExecutePerformanceTestAsync(
                "BusinessOperation",
                () => SimulateBusinessOperationAsync(),
                iterations: 1000
            );

            // Assert
            ValidatePerformanceBenchmarks(metrics, benchmarks);
            Assert.True(metrics.StandardDeviation < 50, "Response time should be consistent");
        }

        [Fact]
        public async Task BusinessOperation_LoadTest_ShouldHandleConcurrentUsers()
        {
            // Act
            var loadResults = await ExecuteLoadTestAsync(
                "ConcurrentBusinessOperations",
                () => SimulateBusinessOperationAsync(),
                concurrentUsers: 10,
                duration: TimeSpan.FromMinutes(1)
            );

            // Assert
            Assert.True(loadResults.SuccessRate >= 0.95, "Load test should maintain 95% success rate");
            Assert.True(loadResults.RequestsPerSecond >= 50, "Should handle at least 50 requests per second");
            Assert.True(loadResults.AverageResponseTime <= 500, "Average response time should be under 500ms");
        }

        private async Task<string> SimulateBusinessOperationAsync()
        {
            // Simulate business operation with variable processing time
            var random = new Random();
            var delay = random.Next(10, 100); // 10-100ms processing time
            await Task.Delay(delay);
            
            // Simulate occasional failures (1% failure rate)
            if (random.NextDouble() < 0.01)
            {
                throw new InvalidOperationException("Simulated business operation failure");
            }

            return $"Operation completed in {delay}ms";
        }
    }
}