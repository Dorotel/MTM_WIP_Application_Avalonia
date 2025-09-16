using System;
using Microsoft.Extensions.Logging;

namespace MTM.UniversalFramework.Core.Configuration;

/// <summary>
/// Universal application state service interface for cross-platform state management.
/// </summary>
public interface IUniversalApplicationStateService
{
    /// <summary>
    /// Gets or sets the current user identifier.
    /// </summary>
    string CurrentUser { get; set; }

    /// <summary>
    /// Gets or sets the current application state.
    /// </summary>
    string ApplicationState { get; set; }

    /// <summary>
    /// Gets the application startup time.
    /// </summary>
    DateTime StartupTime { get; }

    /// <summary>
    /// Gets whether the application is initialized.
    /// </summary>
    bool IsInitialized { get; set; }
}

/// <summary>
/// Universal application state service implementation.
/// </summary>
public class UniversalApplicationStateService : IUniversalApplicationStateService
{
    private readonly ILogger<UniversalApplicationStateService> _logger;

    /// <summary>
    /// Initializes a new instance of the UniversalApplicationStateService.
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public UniversalApplicationStateService(ILogger<UniversalApplicationStateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        StartupTime = DateTime.UtcNow;
        CurrentUser = Environment.UserName;
        ApplicationState = "Initializing";
    }

    /// <inheritdoc />
    public string CurrentUser { get; set; }

    /// <inheritdoc />
    public string ApplicationState { get; set; }

    /// <inheritdoc />
    public DateTime StartupTime { get; }

    /// <inheritdoc />
    public bool IsInitialized { get; set; }
}