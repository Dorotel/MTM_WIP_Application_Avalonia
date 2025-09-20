using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services.Business;

/// <summary>
/// Progress reporting service interface.
/// Provides centralized progress reporting for long-running operations
/// with support for detailed status messages and cancellation.
/// </summary>
public interface IProgressService : INotifyPropertyChanged
{
    bool IsOperationInProgress { get; }
    string CurrentOperationDescription { get; }
    int ProgressPercentage { get; }
    string StatusMessage { get; }
    bool CanCancel { get; }

    void StartOperation(string description, bool canCancel = false);
    void UpdateProgress(int percentage, string? statusMessage = null);
    void CompleteOperation(string? finalMessage = null);
    void CancelOperation();
    void ReportError(string errorMessage);

    event EventHandler<ProgressChangedEventArgs>? ProgressChanged;
    event EventHandler<OperationCompletedEventArgs>? OperationCompleted;
    event EventHandler? OperationCancelled;
}

/// <summary>
/// Progress service implementation.
/// Provides centralized progress reporting with thread-safe operations.
/// </summary>
public class ProgressService : IProgressService
{
    private readonly ILogger<ProgressService> _logger;
    private readonly object _lockObject = new object();

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ProgressChangedEventArgs>? ProgressChanged;
    public event EventHandler<OperationCompletedEventArgs>? OperationCompleted;
    public event EventHandler? OperationCancelled;

    public ProgressService(ILogger<ProgressService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsOperationInProgress { get; private set; }
    public string CurrentOperationDescription { get; private set; } = string.Empty;
    public int ProgressPercentage { get; private set; }
    public string StatusMessage { get; private set; } = string.Empty;
    public bool CanCancel { get; private set; }

    public void StartOperation(string description, bool canCancel = false)
    {
        lock (_lockObject)
        {
            IsOperationInProgress = true;
            CurrentOperationDescription = description;
            ProgressPercentage = 0;
            StatusMessage = "Starting operation...";
            CanCancel = canCancel;

            _logger.LogInformation("Progress operation started: {Description}", description);
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
            {
                ProgressPercentage = 0,
                StatusMessage = description,
                OperationDescription = description
            });
        }
    }

    public void UpdateProgress(int percentage, string? statusMessage = null)
    {
        lock (_lockObject)
        {
            ProgressPercentage = Math.Max(0, Math.Min(100, percentage));
            if (!string.IsNullOrEmpty(statusMessage))
            {
                StatusMessage = statusMessage;
            }

            _logger.LogDebug("Progress updated: {Percentage}% - {Status}", ProgressPercentage, StatusMessage);
            ProgressChanged?.Invoke(this, new ProgressChangedEventArgs
            {
                ProgressPercentage = ProgressPercentage,
                StatusMessage = StatusMessage,
                OperationDescription = CurrentOperationDescription
            });
        }
    }

    public void CompleteOperation(string? finalMessage = null)
    {
        lock (_lockObject)
        {
            ProgressPercentage = 100;
            StatusMessage = finalMessage ?? "Operation completed";
            IsOperationInProgress = false;

            _logger.LogInformation("Progress operation completed: {Message}", StatusMessage);
            OperationCompleted?.Invoke(this, new OperationCompletedEventArgs
            {
                Success = true,
                FinalMessage = StatusMessage,
                OperationDescription = CurrentOperationDescription
            });
        }
    }

    public void CancelOperation()
    {
        lock (_lockObject)
        {
            if (CanCancel)
            {
                IsOperationInProgress = false;
                StatusMessage = "Operation cancelled";

                _logger.LogInformation("Progress operation cancelled: {Operation}", CurrentOperationDescription);
                OperationCancelled?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void ReportError(string errorMessage)
    {
        lock (_lockObject)
        {
            IsOperationInProgress = false;
            StatusMessage = $"Error: {errorMessage}";

            _logger.LogError("Progress operation failed: {Error}", errorMessage);
            OperationCompleted?.Invoke(this, new OperationCompletedEventArgs
            {
                Success = false,
                FinalMessage = errorMessage,
                OperationDescription = CurrentOperationDescription
            });
        }
    }
}
