# Universal Overlay Service Architecture Document

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Stage**: Stage 2 - Universal Service Foundation  

## 🎯 Architecture Overview

The Universal Overlay Service provides a centralized, type-safe, and consistent approach to overlay management across the MTM WIP Application. It consolidates overlay functionality, implements pooling for performance, and ensures consistent user experience.

## 🏗️ Core Architecture Components

### **1. Service Interface Design**

```csharp
/// <summary>
/// Universal service for managing all overlay types with consistent patterns
/// Implements pooling, theming, and lifecycle management
/// </summary>
public interface IUniversalOverlayService : IDisposable
{
    // Core overlay display method - generic pattern for all overlay types
    Task<TResponse> ShowOverlayAsync<TRequest, TResponse, TViewModel>(TRequest request)
        where TRequest : BaseOverlayRequest
        where TResponse : BaseOverlayResponse
        where TViewModel : BasePoolableOverlayViewModel;

    // Specialized convenience methods for common overlay types
    Task<ConfirmationOverlayResponse> ShowConfirmationOverlayAsync(ConfirmationOverlayRequest request);
    Task<SuccessOverlayResponse> ShowSuccessOverlayAsync(SuccessOverlayRequest request);
    Task<ValidationOverlayResponse> ShowValidationOverlayAsync(ValidationOverlayRequest request);
    
    // Progress overlay with disposable tracker pattern
    Task<IProgressTracker> ShowProgressAsync(ProgressOverlayRequest request);
    
    // Overlay management
    Task HideAllOverlaysAsync();
    Task<bool> IsAnyOverlayVisibleAsync();
    
    // Service health and diagnostics
    OverlayServiceStatistics GetStatistics();
    Task<bool> ValidateServiceHealthAsync();
}
```

### **2. Request/Response Pattern Architecture**

```csharp
/// <summary>
/// Base request model with common overlay properties
/// All specific overlay requests inherit from this
/// </summary>
public abstract record BaseOverlayRequest
{
    public string Title { get; init; } = string.Empty;
    public OverlayPriority Priority { get; init; } = OverlayPriority.Normal;
    public bool CanDismiss { get; init; } = true;
    public bool CanDismissOnBackgroundClick { get; init; } = false;
    public TimeSpan? AutoDismissAfter { get; init; }
    public string? CustomStyleClass { get; init; }
    public Dictionary<string, object> CustomProperties { get; init; } = new();
}

/// <summary>
/// Base response model with overlay result information
/// Provides timing and interaction data
/// </summary>
public abstract record BaseOverlayResponse
{
    public OverlayResult Result { get; init; } = OverlayResult.None;
    public DateTime ShownAt { get; init; } = DateTime.Now;
    public DateTime DismissedAt { get; init; } = DateTime.Now;
    public TimeSpan Duration => DismissedAt - ShownAt;
    public string? DismissalReason { get; init; }
    public Dictionary<string, object> ResponseData { get; init; } = new();
}

/// <summary>
/// Overlay interaction results
/// </summary>
public enum OverlayResult
{
    None = 0,
    Confirmed = 1,
    Cancelled = 2,
    Dismissed = 3,
    Error = 4,
    Timeout = 5
}

/// <summary>
/// Overlay display priority for stacking
/// </summary>
public enum OverlayPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3,
    System = 4
}
```

### **3. ViewModel Architecture Pattern**

```csharp
/// <summary>
/// Base class for all poolable overlay ViewModels
/// Provides consistent lifecycle management and pooling support
/// </summary>
[ObservableObject]
public abstract partial class BasePoolableOverlayViewModel : BaseViewModel, IResettable
{
    // Core overlay state
    [ObservableProperty] protected bool isVisible = false;
    [ObservableProperty] protected string title = string.Empty;
    [ObservableProperty] protected OverlayResult result = OverlayResult.None;
    [ObservableProperty] protected bool canDismiss = true;
    [ObservableProperty] protected DateTime shownAt = DateTime.Now;

    // Animation and styling
    [ObservableProperty] protected string styleClass = string.Empty;
    [ObservableProperty] protected bool useAnimation = true;

    // Result notification
    public event EventHandler<OverlayResultEventArgs>? ResultChanged;

    protected BasePoolableOverlayViewModel(ILogger logger) : base(logger) { }

    /// <summary>
    /// Initialize ViewModel from request - called by Universal Service
    /// </summary>
    public virtual Task InitializeAsync<TRequest>(TRequest request) where TRequest : BaseOverlayRequest
    {
        Title = request.Title;
        CanDismiss = request.CanDismiss;
        StyleClass = request.CustomStyleClass ?? string.Empty;
        ShownAt = DateTime.Now;
        IsVisible = true;

        return OnInitializeAsync(request);
    }

    /// <summary>
    /// Override in derived classes for specific initialization
    /// </summary>
    protected virtual Task OnInitializeAsync<TRequest>(TRequest request) where TRequest : BaseOverlayRequest
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Close overlay with result - triggers response creation
    /// </summary>
    protected virtual async Task CloseAsync<TResponse>(TResponse response) where TResponse : BaseOverlayResponse
    {
        IsVisible = false;
        Result = response.Result;
        
        ResultChanged?.Invoke(this, new OverlayResultEventArgs(response));
        
        await OnClosedAsync(response);
    }

    /// <summary>
    /// Override for cleanup when overlay closes
    /// </summary>
    protected virtual Task OnClosedAsync<TResponse>(TResponse response) where TResponse : BaseOverlayResponse
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Reset ViewModel state for pooling reuse
    /// </summary>
    public virtual void Reset()
    {
        IsVisible = false;
        Title = string.Empty;
        Result = OverlayResult.None;
        CanDismiss = true;
        StyleClass = string.Empty;
        UseAnimation = true;
        ShownAt = DateTime.Now;

        OnReset();
    }

    /// <summary>
    /// Override for specific reset behavior
    /// </summary>
    protected virtual void OnReset() { }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Reset();
            ResultChanged = null;
        }
        base.Dispose(disposing);
    }
}

public class OverlayResultEventArgs : EventArgs
{
    public BaseOverlayResponse Response { get; }
    
    public OverlayResultEventArgs(BaseOverlayResponse response)
    {
        Response = response;
    }
}
```

### **4. Service Implementation Architecture**

```csharp
/// <summary>
/// Universal Overlay Service implementation with pooling and lifecycle management
/// </summary>
public class UniversalOverlayService : IUniversalOverlayService
{
    private readonly IOverlayPoolManager _poolManager;
    private readonly IOverlayAnimationService _animationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UniversalOverlayService> _logger;
    private readonly IConfiguration _configuration;

    // Active overlay tracking
    private readonly ConcurrentDictionary<Guid, ActiveOverlayInfo> _activeOverlays = new();
    private readonly object _overlayLock = new();

    // Service configuration
    private readonly UniversalOverlayConfiguration _config;

    public UniversalOverlayService(
        IOverlayPoolManager poolManager,
        IOverlayAnimationService animationService,
        IServiceProvider serviceProvider,
        ILogger<UniversalOverlayService> logger,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(poolManager);
        ArgumentNullException.ThrowIfNull(animationService);
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);

        _poolManager = poolManager;
        _animationService = animationService;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;

        _config = configuration.GetSection("UniversalOverlay").Get<UniversalOverlayConfiguration>() 
                  ?? new UniversalOverlayConfiguration();
    }

    /// <summary>
    /// Core overlay display method - handles all overlay types consistently
    /// </summary>
    public async Task<TResponse> ShowOverlayAsync<TRequest, TResponse, TViewModel>(TRequest request)
        where TRequest : BaseOverlayRequest
        where TResponse : BaseOverlayResponse
        where TViewModel : BasePoolableOverlayViewModel
    {
        try
        {
            // Generate unique overlay ID for tracking
            var overlayId = Guid.NewGuid();
            
            // Get ViewModel from pool
            var viewModel = _poolManager.GetOrCreate<TViewModel>();
            
            // Create tracking info
            var overlayInfo = new ActiveOverlayInfo
            {
                Id = overlayId,
                ViewModel = viewModel,
                RequestType = typeof(TRequest).Name,
                Priority = request.Priority,
                StartTime = DateTime.Now
            };

            lock (_overlayLock)
            {
                _activeOverlays[overlayId] = overlayInfo;
            }

            try
            {
                // Initialize ViewModel with request data
                await viewModel.InitializeAsync(request);
                
                // Set up result handling
                var resultTaskSource = new TaskCompletionSource<TResponse>();
                
                void OnResultChanged(object? sender, OverlayResultEventArgs e)
                {
                    if (e.Response is TResponse response)
                    {
                        resultTaskSource.SetResult(response);
                    }
                    else
                    {
                        resultTaskSource.SetException(new InvalidOperationException(
                            $"Expected response type {typeof(TResponse).Name} but got {e.Response.GetType().Name}"));
                    }
                }

                viewModel.ResultChanged += OnResultChanged;

                try
                {
                    // Show overlay with animation if enabled
                    if (request.CustomStyleClass != "no-animation" && _config.EnableAnimations)
                    {
                        await _animationService.AnimateInAsync(viewModel as Control, 
                            _config.DefaultInAnimation);
                    }

                    // Handle auto-dismiss
                    if (request.AutoDismissAfter.HasValue)
                    {
                        var cancellationTokenSource = new CancellationTokenSource(request.AutoDismissAfter.Value);
                        cancellationTokenSource.Token.Register(() =>
                        {
                            if (!resultTaskSource.Task.IsCompleted)
                            {
                                var timeoutResponse = CreateTimeoutResponse<TResponse>();
                                resultTaskSource.SetResult(timeoutResponse);
                            }
                        });
                    }

                    // Wait for overlay result
                    var result = await resultTaskSource.Task;

                    // Update tracking info
                    overlayInfo.EndTime = DateTime.Now;
                    overlayInfo.Result = result.Result;

                    _logger.LogInformation("Overlay {OverlayType} completed with result {Result} in {Duration}ms", 
                        typeof(TViewModel).Name, result.Result, overlayInfo.Duration.TotalMilliseconds);

                    return result;
                }
                finally
                {
                    viewModel.ResultChanged -= OnResultChanged;
                    
                    // Animate out if enabled
                    if (request.CustomStyleClass != "no-animation" && _config.EnableAnimations)
                    {
                        await _animationService.AnimateOutAsync(viewModel as Control, 
                            _config.DefaultOutAnimation);
                    }
                }
            }
            finally
            {
                // Return ViewModel to pool
                _poolManager.Return(viewModel);
                
                // Remove from tracking
                lock (_overlayLock)
                {
                    _activeOverlays.TryRemove(overlayId, out _);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show overlay {OverlayType}", typeof(TViewModel).Name);
            
            var errorResponse = CreateErrorResponse<TResponse>(ex);
            return errorResponse;
        }
    }

    /// <summary>
    /// Specialized confirmation overlay with validation
    /// </summary>
    public async Task<ConfirmationOverlayResponse> ShowConfirmationOverlayAsync(
        ConfirmationOverlayRequest request)
    {
        ValidateConfirmationRequest(request);
        
        return await ShowOverlayAsync<
            ConfirmationOverlayRequest,
            ConfirmationOverlayResponse,
            ConfirmationOverlayViewModel>(request);
    }

    /// <summary>
    /// Progress overlay with disposable tracker pattern
    /// </summary>
    public async Task<IProgressTracker> ShowProgressAsync(ProgressOverlayRequest request)
    {
        var viewModel = _poolManager.GetOrCreate<ProgressOverlayViewModel>();
        await viewModel.InitializeAsync(request);
        
        var tracker = new ProgressTracker(viewModel, _poolManager, _logger);
        return tracker;
    }

    /// <summary>
    /// Get service statistics for monitoring
    /// </summary>
    public OverlayServiceStatistics GetStatistics()
    {
        lock (_overlayLock)
        {
            return new OverlayServiceStatistics
            {
                ActiveOverlayCount = _activeOverlays.Count,
                ActiveOverlays = _activeOverlays.Values.ToList(),
                PoolStatistics = _poolManager.GetStatistics(),
                ServiceUptime = DateTime.Now - _serviceStartTime,
                TotalOverlaysShown = _totalOverlaysShown,
                AverageResponseTime = _averageResponseTime
            };
        }
    }

    // Private helper methods
    private void ValidateConfirmationRequest(ConfirmationOverlayRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Confirmation title cannot be empty", nameof(request));
            
        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Confirmation message cannot be empty", nameof(request));
    }

    private TResponse CreateTimeoutResponse<TResponse>() where TResponse : BaseOverlayResponse
    {
        return (TResponse)Activator.CreateInstance(typeof(TResponse), 
            OverlayResult.Timeout, DateTime.Now, "Auto-dismissed due to timeout")!;
    }

    private TResponse CreateErrorResponse<TResponse>(Exception ex) where TResponse : BaseOverlayResponse
    {
        return (TResponse)Activator.CreateInstance(typeof(TResponse), 
            OverlayResult.Error, DateTime.Now, $"Error: {ex.Message}")!;
    }

    // Service lifecycle
    private readonly DateTime _serviceStartTime = DateTime.Now;
    private long _totalOverlaysShown = 0;
    private double _averageResponseTime = 0;

    public void Dispose()
    {
        _poolManager?.Dispose();
        _activeOverlays.Clear();
    }
}
```

### **5. Configuration Architecture**

```csharp
/// <summary>
/// Configuration for Universal Overlay Service behavior
/// </summary>
public class UniversalOverlayConfiguration
{
    public bool EnableAnimations { get; set; } = true;
    public AnimationType DefaultInAnimation { get; set; } = AnimationType.FadeIn;
    public AnimationType DefaultOutAnimation { get; set; } = AnimationType.FadeOut;
    public int MaxConcurrentOverlays { get; set; } = 3;
    public TimeSpan DefaultAutoDismissTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableOverlayPooling { get; set; } = true;
    public int MaxPoolSize { get; set; } = 5;
    public bool EnablePerformanceTracking { get; set; } = true;
}

/// <summary>
/// Statistics and monitoring data
/// </summary>
public class OverlayServiceStatistics
{
    public int ActiveOverlayCount { get; set; }
    public List<ActiveOverlayInfo> ActiveOverlays { get; set; } = new();
    public PoolStatistics PoolStatistics { get; set; } = new();
    public TimeSpan ServiceUptime { get; set; }
    public long TotalOverlaysShown { get; set; }
    public double AverageResponseTime { get; set; }
}

public class ActiveOverlayInfo
{
    public Guid Id { get; set; }
    public BasePoolableOverlayViewModel? ViewModel { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public OverlayPriority Priority { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public OverlayResult Result { get; set; }
    public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
}
```

## 🔧 Service Registration Pattern

```csharp
/// <summary>
/// Extension methods for registering overlay services in DI container
/// </summary>
public static class OverlayServiceRegistrationExtensions
{
    /// <summary>
    /// Register all overlay-related services following MTM patterns
    /// </summary>
    public static IServiceCollection AddUniversalOverlayServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Core Universal Service - Singleton for shared state
        services.TryAddSingleton<IUniversalOverlayService, UniversalOverlayService>();
        
        // Supporting services
        services.TryAddSingleton<IOverlayPoolManager, OverlayPoolManager>();
        services.TryAddScoped<IOverlayAnimationService, OverlayAnimationService>();
        
        // Configuration binding
        services.Configure<UniversalOverlayConfiguration>(
            configuration.GetSection("UniversalOverlay"));
        
        // Register all overlay ViewModels as Transient (for pooling)
        RegisterOverlayViewModels(services);
        
        // Register overlay Views
        RegisterOverlayViews(services);
        
        return services;
    }

    private static void RegisterOverlayViewModels(IServiceCollection services)
    {
        // Core overlay ViewModels
        services.TryAddTransient<ConfirmationOverlayViewModel>();
        services.TryAddTransient<SuccessOverlayViewModel>();
        services.TryAddTransient<ProgressOverlayViewModel>();
        services.TryAddTransient<ValidationOverlayViewModel>();
        services.TryAddTransient<ConnectionStatusOverlayViewModel>();
        
        // Specialized overlay ViewModels
        services.TryAddTransient<EditInventoryViewModel>();
        services.TryAddTransient<SuggestionOverlayViewModel>();
        
        // Developer tools (only in Debug builds)
        #if DEBUG
        services.TryAddTransient<OverlayDebugPanelViewModel>();
        #endif
    }

    private static void RegisterOverlayViews(IServiceCollection services)
    {
        // Views are typically registered automatically by Avalonia
        // But can be explicitly registered if needed for dependency injection
        
        // Example explicit registration:
        // services.TryAddTransient<ConfirmationOverlayView>();
    }
}
```

## 🎯 Usage Patterns

### **Basic Overlay Display Pattern**

```csharp
// In any ViewModel that needs overlay functionality
[ObservableObject]
public partial class ExampleViewModel : BaseViewModel
{
    private readonly IUniversalOverlayService _overlayService;

    public ExampleViewModel(
        ILogger<ExampleViewModel> logger,
        IUniversalOverlayService overlayService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(overlayService);
        _overlayService = overlayService;
    }

    [RelayCommand]
    private async Task ShowConfirmationExampleAsync()
    {
        var request = new ConfirmationOverlayRequest(
            Title: "Confirm Action",
            Message: "Are you sure you want to proceed with this action?",
            ConfirmationType: ConfirmationType.Generic,
            PrimaryActionText: "Proceed",
            SecondaryActionText: "Cancel",
            CanDismiss: true
        );

        var response = await _overlayService.ShowConfirmationOverlayAsync(request);

        switch (response.Result)
        {
            case OverlayResult.Confirmed:
                await ProcessConfirmedActionAsync();
                break;
            case OverlayResult.Cancelled:
                Logger.LogInformation("User cancelled action");
                break;
            case OverlayResult.Dismissed:
                Logger.LogInformation("User dismissed dialog");
                break;
        }
    }
}
```

### **Progress Tracking Pattern**

```csharp
[RelayCommand]
private async Task ProcessWithProgressAsync()
{
    using var progress = await _overlayService.ShowProgressAsync(
        new ProgressOverlayRequest(
            Title: "Processing Data",
            InitialStep: "Initializing...",
            CanCancel: true,
            ShowETA: true
        )
    );

    try
    {
        for (int i = 0; i <= 100; i += 10)
        {
            // Check for cancellation
            if (progress.CancellationToken.IsCancellationRequested)
            {
                Logger.LogInformation("Operation cancelled by user");
                return;
            }

            // Update progress
            await progress.UpdateProgressAsync(i, $"Processing step {i/10 + 1}...");
            
            // Simulate work
            await Task.Delay(500, progress.CancellationToken);
        }

        await progress.CompleteAsync("Processing completed successfully");
    }
    catch (OperationCanceledException)
    {
        await progress.CancelAsync("Operation was cancelled");
    }
    catch (Exception ex)
    {
        await progress.CancelAsync($"Operation failed: {ex.Message}");
        throw;
    }
}
```

## 🚀 Performance Considerations

### **Memory Management**

- **Overlay Pooling**: Reuse ViewModel instances to reduce GC pressure
- **Weak References**: Use weak references for parent container tracking
- **Disposal Patterns**: Implement proper disposal for all overlay components
- **Resource Cleanup**: Clean up event subscriptions and timers

### **Threading Considerations**

- **UI Thread Safety**: All overlay operations must execute on UI thread
- **Background Operations**: Progress overlays support background work with cancellation
- **Thread-Safe Collections**: Use concurrent collections for active overlay tracking

### **Animation Performance**

- **GPU Acceleration**: Use GPU-accelerated animations where possible
- **Animation Pooling**: Reuse animation instances
- **Performance Profiling**: Monitor animation frame rates

## 📊 Monitoring and Diagnostics

### **Service Health Monitoring**

```csharp
public async Task<bool> ValidateServiceHealthAsync()
{
    try
    {
        // Validate pool manager health
        var poolStats = _poolManager.GetStatistics();
        if (poolStats.PoolSizes.Any(p => p.Value > _config.MaxPoolSize * 2))
        {
            _logger.LogWarning("Pool sizes are excessive, may indicate memory leak");
            return false;
        }

        // Validate active overlay count
        if (_activeOverlays.Count > _config.MaxConcurrentOverlays)
        {
            _logger.LogWarning("Too many concurrent overlays active");
            return false;
        }

        // Validate animation service
        // (implementation depends on animation service design)

        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Service health validation failed");
        return false;
    }
}
```

## 🔒 Error Handling Strategy

### **Error Classification**

1. **User Errors**: Invalid input, cancelled operations
2. **System Errors**: Resource exhaustion, service unavailability  
3. **Programming Errors**: Invalid requests, missing dependencies
4. **Infrastructure Errors**: Database connection, network issues

### **Error Response Strategy**

```csharp
private async Task<TResponse> HandleOverlayErrorAsync<TResponse>(Exception ex, string context) 
    where TResponse : BaseOverlayResponse
{
    // Log error with context
    _logger.LogError(ex, "Overlay error in {Context}", context);

    // Increment error metrics
    Interlocked.Increment(ref _errorCount);

    // Create error response
    var errorResponse = CreateErrorResponse<TResponse>(ex);

    // Optionally show error overlay to user (avoid infinite loops)
    if (!context.Contains("ErrorOverlay"))
    {
        await ShowErrorOverlayAsync(ex, context);
    }

    return errorResponse;
}
```

---

**This Universal Overlay Service architecture provides a robust, scalable, and maintainable foundation for all overlay functionality in the MTM WIP Application, following established patterns and ensuring consistent user experience.**
