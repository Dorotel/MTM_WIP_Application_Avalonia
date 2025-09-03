using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Core.Startup;

/// <summary>
/// Interface for application health monitoring and diagnostics.
/// Provides comprehensive health check capabilities following .NET best practices.
/// </summary>
public interface IApplicationHealthService
{
    /// <summary>
    /// Gets the current health status of the application.
    /// </summary>
    /// <returns>Comprehensive health status information</returns>
    Task<ApplicationHealthStatus> GetHealthStatusAsync();

    /// <summary>
    /// Performs a comprehensive health check of all application components.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Detailed health check results</returns>
    Task<HealthCheckResult> PerformHealthCheckAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application performance metrics.
    /// </summary>
    /// <returns>Current performance metrics</returns>
    ApplicationPerformanceMetrics GetPerformanceMetrics();

    /// <summary>
    /// Gets application startup metrics.
    /// </summary>
    /// <returns>Startup performance information</returns>
    StartupMetrics GetStartupMetrics();
}

/// <summary>
/// Implementation of application health service with comprehensive monitoring.
/// Provides detailed health checks, performance metrics, and diagnostic information.
/// </summary>
public class ApplicationHealthService : IApplicationHealthService
{
    private readonly ILogger<ApplicationHealthService> _logger;
    private readonly DateTime _startTime;
    private readonly Process _currentProcess;

    /// <summary>
    /// Initializes a new instance of the ApplicationHealthService.
    /// </summary>
    /// <param name="logger">Logger for health monitoring events</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
    public ApplicationHealthService(ILogger<ApplicationHealthService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _startTime = DateTime.UtcNow;
        _currentProcess = Process.GetCurrentProcess();

        _logger.LogInformation("ApplicationHealthService initialized at {StartTime}", _startTime);
        Debug.WriteLine($"[HEALTH] ApplicationHealthService initialized at {_startTime:yyyy-MM-dd HH:mm:ss.fff}");
    }

    /// <summary>
    /// Gets the current health status of the application.
    /// </summary>
    /// <returns>Comprehensive health status information</returns>
    public Task<ApplicationHealthStatus> GetHealthStatusAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Getting application health status");
        
        try
        {
            var status = new ApplicationHealthStatus
            {
                Timestamp = DateTime.UtcNow,
                IsHealthy = true,
                Uptime = DateTime.UtcNow - _startTime,
                ProcessId = _currentProcess.Id,
                ThreadCount = _currentProcess.Threads.Count,
                WorkingSet = _currentProcess.WorkingSet64,
                PrivateMemorySize = _currentProcess.PrivateMemorySize64,
                VirtualMemorySize = _currentProcess.VirtualMemorySize64
            };

            // Check memory usage
            if (status.WorkingSet > 500 * 1024 * 1024) // 500MB threshold
            {
                status.IsHealthy = false;
                status.HealthIssues.Add("High memory usage detected");
            }

            // Check thread count
            if (status.ThreadCount > 100)
            {
                status.HealthIssues.Add("High thread count detected");
            }

            stopwatch.Stop();
            status.ResponseTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogDebug("Health status retrieved in {DurationMs}ms - Healthy: {IsHealthy}", 
                stopwatch.ElapsedMilliseconds, status.IsHealthy);

            return Task.FromResult(status);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error getting application health status after {DurationMs}ms", stopwatch.ElapsedMilliseconds);

            var errorStatus = new ApplicationHealthStatus
            {
                Timestamp = DateTime.UtcNow,
                IsHealthy = false,
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                HealthIssues = new List<string> { $"Health check error: {ex.Message}" }
            };

            return Task.FromResult(errorStatus);
        }
    }

    /// <summary>
    /// Performs a comprehensive health check of all application components.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Detailed health check results</returns>
    public async Task<HealthCheckResult> PerformHealthCheckAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting comprehensive health check");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] PerformHealthCheckAsync() started");
        Debug.WriteLine($"[HEALTH-CHECK] Comprehensive health check started");

        var result = new HealthCheckResult
        {
            Timestamp = DateTime.UtcNow,
            CheckDurationMs = 0,
            ComponentResults = new Dictionary<string, ComponentHealthResult>()
        };

        try
        {
            // Check system resources
            await CheckSystemResources(result, cancellationToken);

            // Check application components
            await CheckApplicationComponents(result, cancellationToken);

            // Check runtime environment
            await CheckRuntimeEnvironment(result, cancellationToken);

            stopwatch.Stop();
            result.CheckDurationMs = stopwatch.ElapsedMilliseconds;
            result.OverallHealth = result.ComponentResults.Values.All(c => c.IsHealthy) 
                ? HealthStatus.Healthy 
                : HealthStatus.Degraded;

            _logger.LogInformation("Health check completed in {DurationMs}ms - Status: {OverallHealth}", 
                stopwatch.ElapsedMilliseconds, result.OverallHealth);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health check completed - Status: {result.OverallHealth}");
            Debug.WriteLine($"[HEALTH-CHECK] Health check completed in {stopwatch.ElapsedMilliseconds}ms");

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Critical error during health check after {DurationMs}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Critical error during health check: {ex.Message}");

            result.CheckDurationMs = stopwatch.ElapsedMilliseconds;
            result.OverallHealth = HealthStatus.Unhealthy;
            result.ComponentResults["HealthCheck"] = new ComponentHealthResult
            {
                ComponentName = "HealthCheck",
                IsHealthy = false,
                ErrorMessage = ex.Message,
                CheckDurationMs = stopwatch.ElapsedMilliseconds
            };

            return result;
        }
    }

    /// <summary>
    /// Gets application performance metrics.
    /// </summary>
    /// <returns>Current performance metrics</returns>
    public ApplicationPerformanceMetrics GetPerformanceMetrics()
    {
        _logger.LogDebug("Getting performance metrics");
        Debug.WriteLine($"[HEALTH] Getting performance metrics");

        try
        {
            _currentProcess.Refresh();

            var metrics = new ApplicationPerformanceMetrics
            {
                Timestamp = DateTime.UtcNow,
                ProcessId = _currentProcess.Id,
                ThreadCount = _currentProcess.Threads.Count,
                HandleCount = _currentProcess.HandleCount,
                WorkingSet = _currentProcess.WorkingSet64,
                PrivateMemorySize = _currentProcess.PrivateMemorySize64,
                VirtualMemorySize = _currentProcess.VirtualMemorySize64,
                PagedMemorySize = _currentProcess.PagedMemorySize64,
                PeakWorkingSet = _currentProcess.PeakWorkingSet64,
                TotalProcessorTime = _currentProcess.TotalProcessorTime,
                UserProcessorTime = _currentProcess.UserProcessorTime,
                StartTime = _currentProcess.StartTime,
                Uptime = DateTime.UtcNow - _currentProcess.StartTime
            };

            _logger.LogTrace("Performance metrics retrieved - Memory: {WorkingSetMB}MB, Threads: {ThreadCount}",
                metrics.WorkingSet / (1024 * 1024), metrics.ThreadCount);

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting performance metrics");
            
            return new ApplicationPerformanceMetrics
            {
                Timestamp = DateTime.UtcNow,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Gets application startup metrics.
    /// </summary>
    /// <returns>Startup performance information</returns>
    public StartupMetrics GetStartupMetrics()
    {
        _logger.LogDebug("Getting startup metrics");
        Debug.WriteLine($"[HEALTH] Getting startup metrics");

        try
        {
            var metrics = new StartupMetrics
            {
                ApplicationStartTime = _startTime,
                ProcessStartTime = _currentProcess.StartTime,
                CurrentTime = DateTime.UtcNow,
                TotalStartupDuration = DateTime.UtcNow - _startTime,
                FrameworkVersion = Environment.Version.ToString(),
                RuntimeVersion = RuntimeInformation.FrameworkDescription,
                OperatingSystem = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                MachineName = Environment.MachineName,
                UserName = Environment.UserName,
                CommandLineArgs = Environment.GetCommandLineArgs()
            };

            _logger.LogTrace("Startup metrics retrieved - Duration: {StartupDurationMs}ms",
                metrics.TotalStartupDuration.TotalMilliseconds);

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting startup metrics");
            
            return new StartupMetrics
            {
                ApplicationStartTime = _startTime,
                CurrentTime = DateTime.UtcNow,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <summary>
    /// Checks system resource availability and health.
    /// </summary>
    /// <param name="result">Health check result to populate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task CheckSystemResources(HealthCheckResult result, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Checking system resources");
        Debug.WriteLine($"[HEALTH-CHECK] Checking system resources");

        try
        {
            var componentResult = new ComponentHealthResult
            {
                ComponentName = "SystemResources",
                IsHealthy = true,
                Details = new Dictionary<string, object>()
            };

            // Check available memory
            var gcMemory = GC.GetTotalMemory(false);
            componentResult.Details["GCMemory"] = $"{gcMemory / (1024 * 1024)} MB";

            // Check processor count
            componentResult.Details["ProcessorCount"] = Environment.ProcessorCount;

            // Check disk space for application directory
            var appDirectory = AppContext.BaseDirectory;
            var rootPath = System.IO.Path.GetPathRoot(appDirectory);
            if (!string.IsNullOrEmpty(rootPath))
            {
                var drive = new System.IO.DriveInfo(rootPath);
                var freeSpaceGB = drive.AvailableFreeSpace / (1024 * 1024 * 1024);
                componentResult.Details["DiskFreeSpace"] = $"{freeSpaceGB} GB";

                if (freeSpaceGB < 1) // Less than 1GB free
                {
                    componentResult.IsHealthy = false;
                    componentResult.ErrorMessage = "Low disk space detected";
                }
            }
            else
            {
                componentResult.Details["DiskFreeSpace"] = "Unable to determine";
            }

            stopwatch.Stop();
            componentResult.CheckDurationMs = stopwatch.ElapsedMilliseconds;
            result.ComponentResults["SystemResources"] = componentResult;

            _logger.LogDebug("System resources check completed in {DurationMs}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error checking system resources");
            
            result.ComponentResults["SystemResources"] = new ComponentHealthResult
            {
                ComponentName = "SystemResources",
                IsHealthy = false,
                ErrorMessage = ex.Message,
                CheckDurationMs = stopwatch.ElapsedMilliseconds
            };
        }

        await Task.Delay(1, cancellationToken); // Yield control
    }

    /// <summary>
    /// Checks application component health.
    /// </summary>
    /// <param name="result">Health check result to populate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task CheckApplicationComponents(HealthCheckResult result, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Checking application components");
        Debug.WriteLine($"[HEALTH-CHECK] Checking application components");

        try
        {
            var componentResult = new ComponentHealthResult
            {
                ComponentName = "ApplicationComponents",
                IsHealthy = true,
                Details = new Dictionary<string, object>()
            };

            // Check if main window exists (UI component health)
            var hasMainWindow = Avalonia.Application.Current?.ApplicationLifetime is 
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.MainWindow != null;

            componentResult.Details["MainWindowExists"] = hasMainWindow;

            // Check theme system
            var hasThemeResources = Avalonia.Application.Current?.Resources != null;
            componentResult.Details["ThemeResourcesLoaded"] = hasThemeResources;

            stopwatch.Stop();
            componentResult.CheckDurationMs = stopwatch.ElapsedMilliseconds;
            result.ComponentResults["ApplicationComponents"] = componentResult;

            _logger.LogDebug("Application components check completed in {DurationMs}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error checking application components");
            
            result.ComponentResults["ApplicationComponents"] = new ComponentHealthResult
            {
                ComponentName = "ApplicationComponents",
                IsHealthy = false,
                ErrorMessage = ex.Message,
                CheckDurationMs = stopwatch.ElapsedMilliseconds
            };
        }

        await Task.Delay(1, cancellationToken); // Yield control
    }

    /// <summary>
    /// Checks runtime environment health.
    /// </summary>
    /// <param name="result">Health check result to populate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task CheckRuntimeEnvironment(HealthCheckResult result, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Checking runtime environment");
        Debug.WriteLine($"[HEALTH-CHECK] Checking runtime environment");

        try
        {
            var componentResult = new ComponentHealthResult
            {
                ComponentName = "RuntimeEnvironment",
                IsHealthy = true,
                Details = new Dictionary<string, object>()
            };

            // Check .NET version
            var dotnetVersion = Environment.Version;
            componentResult.Details["DotNetVersion"] = dotnetVersion.ToString();

            if (dotnetVersion.Major < 8)
            {
                componentResult.IsHealthy = false;
                componentResult.ErrorMessage = $"Unsupported .NET version: {dotnetVersion}";
            }

            // Check runtime information
            componentResult.Details["RuntimeFramework"] = RuntimeInformation.FrameworkDescription;
            componentResult.Details["OSDescription"] = RuntimeInformation.OSDescription;
            componentResult.Details["ProcessArchitecture"] = RuntimeInformation.ProcessArchitecture.ToString();

            // Check garbage collection
            componentResult.Details["GCGen0Collections"] = GC.CollectionCount(0);
            componentResult.Details["GCGen1Collections"] = GC.CollectionCount(1);
            componentResult.Details["GCGen2Collections"] = GC.CollectionCount(2);

            stopwatch.Stop();
            componentResult.CheckDurationMs = stopwatch.ElapsedMilliseconds;
            result.ComponentResults["RuntimeEnvironment"] = componentResult;

            _logger.LogDebug("Runtime environment check completed in {DurationMs}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error checking runtime environment");
            
            result.ComponentResults["RuntimeEnvironment"] = new ComponentHealthResult
            {
                ComponentName = "RuntimeEnvironment",
                IsHealthy = false,
                ErrorMessage = ex.Message,
                CheckDurationMs = stopwatch.ElapsedMilliseconds
            };
        }

        await Task.Delay(1, cancellationToken); // Yield control
    }
}

/// <summary>
/// Represents the health status of the application.
/// </summary>
public class ApplicationHealthStatus
{
    public DateTime Timestamp { get; set; }
    public bool IsHealthy { get; set; }
    public TimeSpan Uptime { get; set; }
    public int ProcessId { get; set; }
    public int ThreadCount { get; set; }
    public long WorkingSet { get; set; }
    public long PrivateMemorySize { get; set; }
    public long VirtualMemorySize { get; set; }
    public long ResponseTimeMs { get; set; }
    public List<string> HealthIssues { get; set; } = new();
}

/// <summary>
/// Represents the result of a comprehensive health check.
/// </summary>
public class HealthCheckResult
{
    public DateTime Timestamp { get; set; }
    public HealthStatus OverallHealth { get; set; }
    public long CheckDurationMs { get; set; }
    public Dictionary<string, ComponentHealthResult> ComponentResults { get; set; } = new();
}

/// <summary>
/// Represents the health status of an individual component.
/// </summary>
public class ComponentHealthResult
{
    public string ComponentName { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public string? ErrorMessage { get; set; }
    public long CheckDurationMs { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// Enumeration of possible health statuses.
/// </summary>
public enum HealthStatus
{
    Healthy,
    Degraded,
    Unhealthy
}

/// <summary>
/// Represents application performance metrics.
/// </summary>
public class ApplicationPerformanceMetrics
{
    public DateTime Timestamp { get; set; }
    public int ProcessId { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public long WorkingSet { get; set; }
    public long PrivateMemorySize { get; set; }
    public long VirtualMemorySize { get; set; }
    public long PagedMemorySize { get; set; }
    public long PeakWorkingSet { get; set; }
    public TimeSpan TotalProcessorTime { get; set; }
    public TimeSpan UserProcessorTime { get; set; }
    public DateTime StartTime { get; set; }
    public TimeSpan Uptime { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents application startup metrics.
/// </summary>
public class StartupMetrics
{
    public DateTime ApplicationStartTime { get; set; }
    public DateTime ProcessStartTime { get; set; }
    public DateTime CurrentTime { get; set; }
    public TimeSpan TotalStartupDuration { get; set; }
    public string FrameworkVersion { get; set; } = string.Empty;
    public string RuntimeVersion { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public int ProcessorCount { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string[] CommandLineArgs { get; set; } = Array.Empty<string>();
    public string? ErrorMessage { get; set; }
}
