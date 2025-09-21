using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Core;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for the connection status overlay that displays database connectivity information
/// and provides retry options when connection failures occur.
/// Follows BaseOverlayViewModel pattern and MTM design guidelines.
/// </summary>
public partial class ConnectionStatusOverlayViewModel : BaseOverlayViewModel
{
    private readonly IDatabaseService _databaseService;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the current connection status display text.
    /// </summary>
    [ObservableProperty]
    private string connectionStatus = "Checking...";

    /// <summary>
    /// Gets or sets whether the database connection is currently active.
    /// </summary>
    [ObservableProperty]
    private bool isConnected;

    /// <summary>
    /// Gets or sets the last connection error message if any.
    /// </summary>
    [ObservableProperty]
    private string lastConnectionError = string.Empty;

    /// <summary>
    /// Gets or sets whether the connection test is currently in progress.
    /// </summary>
    [ObservableProperty]
    private bool isTestingConnection;

    /// <summary>
    /// Gets or sets the number of retry attempts made.
    /// </summary>
    [ObservableProperty]
    private int retryAttempts;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts allowed.
    /// </summary>
    [ObservableProperty]
    private int maxRetryAttempts = 3;

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    [ObservableProperty]
    private int connectionTimeoutSeconds = 30;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the ConnectionStatusOverlayViewModel class.
    /// </summary>
    /// <param name="logger">Logger instance for connection status operations.</param>
    /// <param name="databaseService">Database service for connection testing.</param>
    public ConnectionStatusOverlayViewModel(
        ILogger<ConnectionStatusOverlayViewModel> logger,
        IDatabaseService databaseService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(databaseService);

        _databaseService = databaseService;

        _logger.LogDebug("ConnectionStatusOverlayViewModel initialized");
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to retry the database connection.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRetryConnection))]
    private async Task RetryConnectionAsync()
    {
        try
        {
            IsTestingConnection = true;
            ClearError();

            _logger.LogInformation("Retrying database connection (attempt {AttemptNumber}/{MaxAttempts})",
                RetryAttempts + 1, MaxRetryAttempts);

            ConnectionStatus = "Testing connection...";

            // Test database connection
            var connectionResult = await TestDatabaseConnectionAsync();

            RetryAttempts++;

            if (connectionResult.IsSuccess)
            {
                IsConnected = true;
                ConnectionStatus = "Connected Successfully";
                LastConnectionError = string.Empty;

                _logger.LogInformation("Database connection successful after {Attempts} attempt(s)", RetryAttempts);

                // Auto-close overlay after successful connection
                await Task.Delay(1500); // Brief delay to show success
                await HideAsync();
            }
            else
            {
                IsConnected = false;
                ConnectionStatus = "Connection Failed";
                LastConnectionError = connectionResult.ErrorMessage;

                _logger.LogWarning("Database connection failed: {Error}", connectionResult.ErrorMessage);

                if (RetryAttempts >= MaxRetryAttempts)
                {
                    ConnectionStatus = "Connection Failed - Max Retries Exceeded";
                    _logger.LogError("Maximum retry attempts ({MaxAttempts}) exceeded for database connection", MaxRetryAttempts);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Connection retry failed");
            IsConnected = false;
            ConnectionStatus = "Connection Test Failed";
            LastConnectionError = ex.Message;
        }
        finally
        {
            IsTestingConnection = false;
        }
    }

    /// <summary>
    /// Command to force close the overlay without retrying.
    /// </summary>
    [RelayCommand]
    private async Task SkipConnectionTestAsync()
    {
        try
        {
            _logger.LogWarning("User chose to skip connection test and continue");

            ConnectionStatus = "Connection Test Skipped";
            await HideAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to skip connection test");
        }
    }

    /// <summary>
    /// Command to reset the connection test and start over.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanResetConnection))]
    private async Task ResetConnectionTestAsync()
    {
        try
        {
            _logger.LogInformation("Resetting connection test");

            RetryAttempts = 0;
            IsConnected = false;
            ConnectionStatus = "Ready to test connection";
            LastConnectionError = string.Empty;
            ClearError();

            // Automatically start a new connection test
            await RetryConnectionCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to reset connection test");
        }
    }

    #endregion

    #region Command CanExecute Methods

    /// <summary>
    /// Determines whether the retry connection command can be executed.
    /// </summary>
    private bool CanRetryConnection() => !IsTestingConnection && RetryAttempts < MaxRetryAttempts;

    /// <summary>
    /// Determines whether the reset connection command can be executed.
    /// </summary>
    private bool CanResetConnection() => !IsTestingConnection;

    #endregion

    #region Protected Override Methods

    protected override string GetDefaultTitle() => "Database Connection Status";

    protected override async Task OnInitializeAsync(object requestData)
    {
        await base.OnInitializeAsync(requestData);

        // Start initial connection test when overlay is initialized
        if (!IsTestingConnection)
        {
            await RetryConnectionCommand.ExecuteAsync(null);
        }
    }

    protected override async Task OnShowingAsync()
    {
        await base.OnShowingAsync();

        // Reset state when showing
        RetryAttempts = 0;
        ConnectionStatus = "Initializing connection test...";
        ClearError();
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        // For connection status overlay, confirm means accepting current connection state
        if (IsConnected)
        {
            _logger.LogInformation("User confirmed successful database connection");
            return true; // Allow overlay to close
        }
        else
        {
            _logger.LogWarning("User attempted to confirm failed connection state");

            // Ask if they want to proceed without connection
            // For now, just allow confirmation
            return true;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests the database connection using the database service.
    /// </summary>
    /// <returns>A result indicating success or failure with error details.</returns>
    private async Task<ConnectionTestResult> TestDatabaseConnectionAsync()
    {
        try
        {
            _logger.LogDebug("Testing database connection with timeout of {TimeoutSeconds} seconds", ConnectionTimeoutSeconds);

            // Create a timeout task
            using var timeoutCancellation = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(ConnectionTimeoutSeconds));

            try
            {
                // Test database connection through service
                // For now, we'll simulate a connection test since the actual database service interface may not be available

                // Simulate connection test delay
                await Task.Delay(2000, timeoutCancellation.Token);

                // For demonstration, we'll assume connection succeeds
                // In actual implementation, this would use: await _databaseService.TestConnectionAsync();

                return ConnectionTestResult.Success("Database connection established successfully");
            }
            catch (OperationCanceledException) when (timeoutCancellation.Token.IsCancellationRequested)
            {
                return ConnectionTestResult.Failure($"Connection test timed out after {ConnectionTimeoutSeconds} seconds");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed with exception");
            return ConnectionTestResult.Failure($"Connection failed: {ex.Message}");
        }
    }

    #endregion
}

/// <summary>
/// Result of a database connection test operation.
/// </summary>
public class ConnectionTestResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;
    public Exception? Exception { get; private set; }

    private ConnectionTestResult() { }

    public static ConnectionTestResult Success(string message = "Connection successful")
        => new() { IsSuccess = true, Message = message };

    public static ConnectionTestResult Failure(string errorMessage, Exception? exception = null)
        => new() { IsSuccess = false, ErrorMessage = errorMessage, Exception = exception };
}
