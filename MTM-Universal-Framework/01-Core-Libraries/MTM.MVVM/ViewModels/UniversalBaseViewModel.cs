using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace MTM.UniversalFramework.MVVM.ViewModels;

/// <summary>
/// Universal base ViewModel following MVVM Community Toolkit patterns with cross-platform lifecycle management.
/// Extracted from MTM WIP Application and generalized for any business domain.
/// </summary>
public abstract partial class UniversalBaseViewModel : ObservableObject, IDisposable
{
    protected readonly ILogger Logger;
    private bool _disposed = false;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool hasErrors;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// <summary>
    /// Initializes a new instance of the UniversalBaseViewModel.
    /// </summary>
    /// <param name="logger">Logger instance for structured logging</param>
    protected UniversalBaseViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        Logger.LogDebug("ViewModel {ViewModelType} initialized", GetType().Name);
    }

    /// <summary>
    /// Sets error state with message and logging.
    /// </summary>
    /// <param name="message">Error message to display</param>
    /// <param name="exception">Optional exception for detailed logging</param>
    protected virtual void SetError(string message, Exception? exception = null)
    {
        ErrorMessage = message;
        HasErrors = true;
        StatusMessage = message;
        
        if (exception != null)
        {
            Logger.LogError(exception, "Error in {ViewModelType}: {ErrorMessage}", GetType().Name, message);
        }
        else
        {
            Logger.LogWarning("Error in {ViewModelType}: {ErrorMessage}", GetType().Name, message);
        }
    }

    /// <summary>
    /// Clears error state.
    /// </summary>
    protected virtual void ClearErrors()
    {
        HasErrors = false;
        ErrorMessage = string.Empty;
        if (StatusMessage == ErrorMessage)
        {
            StatusMessage = string.Empty;
        }
    }

    /// <summary>
    /// Sets success status message.
    /// </summary>
    /// <param name="message">Success message to display</param>
    protected virtual void SetSuccess(string message)
    {
        StatusMessage = message;
        ClearErrors();
        Logger.LogInformation("Success in {ViewModelType}: {Message}", GetType().Name, message);
    }

    /// <summary>
    /// Executes an async operation with automatic loading state management and error handling.
    /// </summary>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="operationName">Name of the operation for logging</param>
    protected async Task ExecuteAsync(Func<Task> operation, [CallerMemberName] string operationName = "")
    {
        IsLoading = true;
        ClearErrors();
        
        try
        {
            Logger.LogDebug("Starting operation {OperationName} in {ViewModelType}", operationName, GetType().Name);
            await operation();
            Logger.LogDebug("Completed operation {OperationName} in {ViewModelType}", operationName, GetType().Name);
        }
        catch (Exception ex)
        {
            SetError($"Operation '{operationName}' failed: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Executes an async operation with return value, automatic loading state management and error handling.
    /// </summary>
    /// <typeparam name="T">Return type of the operation</typeparam>
    /// <param name="operation">The async operation to execute</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <returns>Result of the operation or default value on error</returns>
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, [CallerMemberName] string operationName = "")
    {
        IsLoading = true;
        ClearErrors();
        
        try
        {
            Logger.LogDebug("Starting operation {OperationName} in {ViewModelType}", operationName, GetType().Name);
            var result = await operation();
            Logger.LogDebug("Completed operation {OperationName} in {ViewModelType}", operationName, GetType().Name);
            return result;
        }
        catch (Exception ex)
        {
            SetError($"Operation '{operationName}' failed: {ex.Message}", ex);
            return default;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Override to implement ViewModel-specific initialization logic.
    /// Called during ViewModel construction or activation.
    /// </summary>
    protected virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Override to implement ViewModel-specific cleanup logic.
    /// Called when ViewModel is disposed or deactivated.
    /// </summary>
    protected virtual Task CleanupAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method following standard dispose pattern.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                CleanupAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during cleanup in {ViewModelType}", GetType().Name);
            }
            
            Logger.LogDebug("ViewModel {ViewModelType} disposed", GetType().Name);
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer for emergency cleanup.
    /// </summary>
    ~UniversalBaseViewModel()
    {
        Dispose(false);
    }
}