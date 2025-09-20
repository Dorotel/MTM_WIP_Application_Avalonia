# Overlay Performance & Memory Management Guide

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Developers  

## 🎯 Overview

This guide provides comprehensive information about optimizing overlay performance, managing memory efficiently, and implementing monitoring systems for the MTM overlay system.

## 🚀 Performance Optimization Strategies

### **1. Overlay Pooling System**

#### **Pool Manager Implementation**

```csharp
/// <summary>
/// High-performance overlay pool manager with configurable sizing and cleanup
/// </summary>
public class OptimizedOverlayPoolManager : IOverlayPoolManager
{
    private readonly ConcurrentDictionary<Type, ObjectPool<BasePoolableOverlayViewModel>> _pools = new();
    private readonly ConcurrentDictionary<Type, PoolMetrics> _metrics = new();
    private readonly ILogger<OptimizedOverlayPoolManager> _logger;
    private readonly Timer _cleanupTimer;
    private readonly PoolConfiguration _config;

    public OptimizedOverlayPoolManager(
        ILogger<OptimizedOverlayPoolManager> logger,
        IOptions<PoolConfiguration> config)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(config);
        
        _logger = logger;
        _config = config.Value;
        
        // Cleanup timer runs every 5 minutes
        _cleanupTimer = new Timer(PerformCleanup, null, 
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public T GetOrCreate<T>() where T : BasePoolableOverlayViewModel, new()
    {
        var type = typeof(T);
        var pool = _pools.GetOrAdd(type, _ => CreatePool<T>());
        var metrics = _metrics.GetOrAdd(type, _ => new PoolMetrics());

        var instance = pool.Get();
        
        // Update metrics
        Interlocked.Increment(ref metrics.TotalGets);
        Interlocked.Increment(ref metrics.ActiveInstances);
        
        if (instance.IsPooled)
        {
            Interlocked.Increment(ref metrics.PoolHits);
            _logger.LogDebug("Retrieved pooled {Type} (hit rate: {HitRate:P})", 
                type.Name, metrics.HitRate);
        }
        else
        {
            Interlocked.Increment(ref metrics.TotalCreated);
            _logger.LogDebug("Created new {Type} instance", type.Name);
        }

        return (T)instance;
    }

    public void Return<T>(T overlay) where T : BasePoolableOverlayViewModel
    {
        if (overlay == null) return;

        var type = typeof(T);
        var pool = _pools.GetOrAdd(type, _ => CreatePool<T>());
        var metrics = _metrics.GetOrAdd(type, _ => new PoolMetrics());

        // Reset overlay state for reuse
        overlay.Reset();
        overlay.IsPooled = true;
        overlay.LastUsed = DateTime.Now;

        // Return to pool
        pool.Return(overlay);
        
        // Update metrics
        Interlocked.Decrement(ref metrics.ActiveInstances);
        Interlocked.Increment(ref metrics.TotalReturns);
        
        _logger.LogDebug("Returned {Type} to pool (pool size: {PoolSize})", 
            type.Name, pool.Count);
    }

    private ObjectPool<BasePoolableOverlayViewModel> CreatePool<T>() where T : BasePoolableOverlayViewModel, new()
    {
        return new DefaultObjectPool<BasePoolableOverlayViewModel>(
            new OverlayPoolPolicy<T>(), 
            _config.MaxPoolSize);
    }

    private void PerformCleanup(object? state)
    {
        var cutoffTime = DateTime.Now.Subtract(_config.MaxIdleTime);
        var totalCleaned = 0;

        foreach (var kvp in _pools)
        {
            var pool = kvp.Value;
            var type = kvp.Key;
            var cleaned = 0;

            // Note: This is a simplified cleanup - actual implementation would need
            // to work with the specific ObjectPool implementation
            while (pool.Count > _config.MinPoolSize)
            {
                if (pool.TryGet(out var item) && item.LastUsed < cutoffTime)
                {
                    item.Dispose();
                    cleaned++;
                }
                else if (item != null)
                {
                    // Return item that's still fresh
                    pool.Return(item);
                    break;
                }
            }

            if (cleaned > 0)
            {
                _logger.LogInformation("Cleaned {Count} idle {Type} instances from pool", 
                    cleaned, type.Name);
                totalCleaned += cleaned;
            }
        }

        if (totalCleaned > 0)
        {
            // Force garbage collection after cleanup
            GC.Collect(1, GCCollectionMode.Optimized);
            _logger.LogInformation("Pool cleanup completed: {Total} instances cleaned, GC triggered", 
                totalCleaned);
        }
    }
}

/// <summary>
/// Pool policy for creating overlay instances
/// </summary>
public class OverlayPoolPolicy<T> : IPooledObjectPolicy<BasePoolableOverlayViewModel> 
    where T : BasePoolableOverlayViewModel, new()
{
    public BasePoolableOverlayViewModel Create()
    {
        var instance = new T();
        instance.IsPooled = false;
        return instance;
    }

    public bool Return(BasePoolableOverlayViewModel obj)
    {
        if (obj == null) return false;
        
        // Validate object is in good state for reuse
        return obj.IsDisposed == false && obj.CanBeReused;
    }
}

/// <summary>
/// Pool configuration options
/// </summary>
public class PoolConfiguration
{
    public int MaxPoolSize { get; set; } = 5;
    public int MinPoolSize { get; set; } = 1;
    public TimeSpan MaxIdleTime { get; set; } = TimeSpan.FromMinutes(10);
    public bool EnableMetrics { get; set; } = true;
    public bool EnableCleanup { get; set; } = true;
}

/// <summary>
/// Pool performance metrics
/// </summary>
public class PoolMetrics
{
    public long TotalGets;
    public long TotalCreated;
    public long TotalReturns;
    public long PoolHits;
    public long ActiveInstances;
    
    public double HitRate => TotalGets > 0 ? (double)PoolHits / TotalGets : 0;
    public double PoolUtilization => TotalCreated > 0 ? (double)PoolHits / TotalCreated : 0;
}
```

### **2. Memory Management Patterns**

#### **Lazy Loading Implementation**

```csharp
/// <summary>
/// Lazy loading overlay ViewModel with memory-efficient initialization
/// </summary>
[ObservableObject]
public partial class LazyLoadingOverlayViewModel : BasePoolableOverlayViewModel
{
    // Core properties loaded immediately
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private bool isVisible = false;

    // Heavy properties loaded on-demand
    private ObservableCollection<string>? _heavyDataCollection;
    private ComplexModel? _complexData;
    private byte[]? _imageData;

    // Lazy loading flags
    private bool _heavyDataLoaded = false;
    private bool _complexDataLoaded = false;
    private bool _imageDataLoaded = false;

    /// <summary>
    /// Heavy data collection loaded only when accessed
    /// </summary>
    public ObservableCollection<string> HeavyDataCollection
    {
        get
        {
            if (!_heavyDataLoaded)
            {
                LoadHeavyDataAsync().ConfigureAwait(false);
            }
            return _heavyDataCollection ??= new ObservableCollection<string>();
        }
    }

    /// <summary>
    /// Complex data model loaded on-demand
    /// </summary>
    public ComplexModel? ComplexData
    {
        get
        {
            if (!_complexDataLoaded && _complexData == null)
            {
                LoadComplexDataAsync().ConfigureAwait(false);
            }
            return _complexData;
        }
    }

    private async Task LoadHeavyDataAsync()
    {
        if (_heavyDataLoaded) return;

        try
        {
            // Simulate heavy data loading
            await Task.Run(() =>
            {
                _heavyDataCollection = new ObservableCollection<string>();
                for (int i = 0; i < 1000; i++)
                {
                    _heavyDataCollection.Add($"Item {i}");
                }
            });
            
            _heavyDataLoaded = true;
            Logger.LogDebug("Heavy data collection loaded with {Count} items", 
                _heavyDataCollection.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load heavy data collection");
        }
    }

    private async Task LoadComplexDataAsync()
    {
        if (_complexDataLoaded) return;

        try
        {
            // Load complex data from service
            _complexData = await LoadComplexModelFromServiceAsync();
            _complexDataLoaded = true;
            
            Logger.LogDebug("Complex data model loaded");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load complex data model");
        }
    }

    protected override void OnReset()
    {
        // Clear loaded data but keep flags for smart reloading
        _heavyDataCollection?.Clear();
        _complexData = null;
        _imageData = null;
        
        // Reset flags - data will be reloaded when needed
        _heavyDataLoaded = false;
        _complexDataLoaded = false;
        _imageDataLoaded = false;
        
        Logger.LogDebug("Lazy loading overlay reset");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose of heavy resources
            _heavyDataCollection?.Clear();
            _complexData?.Dispose();
            _imageData = null;
        }
        base.Dispose(disposing);
    }
}
```

#### **Weak Reference Parent Tracking**

```csharp
/// <summary>
/// Parent-aware overlay with weak reference tracking to prevent memory leaks
/// </summary>
[ObservableObject]
public partial class ParentAwareOverlayViewModel : BasePoolableOverlayViewModel
{
    private WeakReference<UserControl>? _parentViewRef;
    private WeakReference<BaseViewModel>? _parentViewModelRef;

    /// <summary>
    /// Set parent view with weak reference to prevent circular references
    /// </summary>
    public void SetParentView(UserControl parentView)
    {
        _parentViewRef = new WeakReference<UserControl>(parentView);
        
        // Subscribe to parent disposal to cleanup overlay
        if (parentView is INotifyPropertyChanged notifyParent)
        {
            WeakEventManager.AddEventHandler(notifyParent, nameof(INotifyPropertyChanged.PropertyChanged), 
                OnParentPropertyChanged);
        }
    }

    /// <summary>
    /// Set parent ViewModel with weak reference
    /// </summary>
    public void SetParentViewModel(BaseViewModel parentViewModel)
    {
        _parentViewModelRef = new WeakReference<BaseViewModel>(parentViewModel);
    }

    /// <summary>
    /// Get parent view if still alive
    /// </summary>
    public UserControl? GetParentView()
    {
        return _parentViewRef?.TryGetTarget(out var parent) == true ? parent : null;
    }

    /// <summary>
    /// Get parent ViewModel if still alive
    /// </summary>
    public BaseViewModel? GetParentViewModel()
    {
        return _parentViewModelRef?.TryGetTarget(out var parent) == true ? parent : null;
    }

    private void OnParentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Handle parent changes, like disposal
        if (e.PropertyName == "IsDisposed" && sender is BaseViewModel vm && vm.IsDisposed)
        {
            // Parent is being disposed, cleanup overlay
            Logger.LogDebug("Parent ViewModel disposed, closing overlay");
            CloseAsync(new BaseOverlayResponse { Result = OverlayResult.Dismissed }).ConfigureAwait(false);
        }
    }

    protected override void OnReset()
    {
        // Clear weak references
        _parentViewRef = null;
        _parentViewModelRef = null;
        
        base.OnReset();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clean up weak references and event subscriptions
            if (_parentViewRef?.TryGetTarget(out var parent) == true && 
                parent is INotifyPropertyChanged notifyParent)
            {
                WeakEventManager.RemoveEventHandler(notifyParent, nameof(INotifyPropertyChanged.PropertyChanged), 
                    OnParentPropertyChanged);
            }
            
            _parentViewRef = null;
            _parentViewModelRef = null;
        }
        base.Dispose(disposing);
    }
}
```

### **3. Performance Monitoring System**

#### **Real-time Performance Tracker**

```csharp
/// <summary>
/// Comprehensive performance monitoring for overlay system
/// </summary>
public class OverlayPerformanceMonitor : IOverlayPerformanceMonitor
{
    private readonly ILogger<OverlayPerformanceMonitor> _logger;
    private readonly Timer _monitoringTimer;
    private readonly ConcurrentQueue<PerformanceSnapshot> _snapshots = new();
    private readonly ConcurrentDictionary<string, OverlayPerformanceData> _overlayMetrics = new();
    
    // Performance thresholds
    private readonly TimeSpan _slowOverlayThreshold = TimeSpan.FromMilliseconds(500);
    private readonly long _memoryLeakThreshold = 50 * 1024 * 1024; // 50 MB
    private readonly int _maxSnapshotHistory = 300; // 5 minutes at 1-second intervals

    public OverlayPerformanceMonitor(ILogger<OverlayPerformanceMonitor> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        
        // Monitor performance every second
        _monitoringTimer = new Timer(CapturePerformanceSnapshot, null, 
            TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Start tracking overlay performance
    /// </summary>
    public IDisposable TrackOverlayPerformance(string overlayType, Guid overlayId)
    {
        var key = $"{overlayType}_{overlayId}";
        var data = new OverlayPerformanceData
        {
            OverlayType = overlayType,
            OverlayId = overlayId,
            StartTime = DateTime.Now,
            StartMemory = GC.GetTotalMemory(false)
        };

        _overlayMetrics[key] = data;
        
        return new OverlayPerformanceTracker(key, this, _logger);
    }

    /// <summary>
    /// End tracking for specific overlay
    /// </summary>
    internal void EndTracking(string key, OverlayResult result)
    {
        if (_overlayMetrics.TryRemove(key, out var data))
        {
            data.EndTime = DateTime.Now;
            data.EndMemory = GC.GetTotalMemory(false);
            data.Result = result;
            
            var duration = data.Duration;
            var memoryDelta = data.MemoryDelta;

            // Log performance metrics
            if (duration > _slowOverlayThreshold)
            {
                _logger.LogWarning("Slow overlay detected: {Type} took {Duration}ms", 
                    data.OverlayType, duration.TotalMilliseconds);
            }

            if (memoryDelta > _memoryLeakThreshold)
            {
                _logger.LogWarning("Potential memory leak: {Type} increased memory by {Memory:N0} bytes", 
                    data.OverlayType, memoryDelta);
            }

            _logger.LogInformation("Overlay performance: {Type} - Duration: {Duration}ms, Memory: {Memory:+#;-#;0} bytes", 
                data.OverlayType, duration.TotalMilliseconds, memoryDelta);
        }
    }

    /// <summary>
    /// Capture system-wide performance snapshot
    /// </summary>
    private void CapturePerformanceSnapshot(object? state)
    {
        try
        {
            var snapshot = new PerformanceSnapshot
            {
                Timestamp = DateTime.Now,
                TotalMemory = GC.GetTotalMemory(false),
                ActiveOverlayCount = _overlayMetrics.Count,
                Generation0Collections = GC.CollectionCount(0),
                Generation1Collections = GC.CollectionCount(1),
                Generation2Collections = GC.CollectionCount(2)
            };

            _snapshots.Enqueue(snapshot);

            // Maintain snapshot history limit
            while (_snapshots.Count > _maxSnapshotHistory)
            {
                _snapshots.TryDequeue(out _);
            }

            // Check for performance issues
            if (_snapshots.Count >= 10) // Need at least 10 samples
            {
                AnalyzePerformanceTrends();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture performance snapshot");
        }
    }

    /// <summary>
    /// Analyze recent performance trends
    /// </summary>
    private void AnalyzePerformanceTrends()
    {
        var recentSnapshots = _snapshots.TakeLast(10).ToArray();
        if (recentSnapshots.Length < 10) return;

        var memoryTrend = CalculateMemoryTrend(recentSnapshots);
        var gcTrend = CalculateGCTrend(recentSnapshots);

        // Alert on concerning trends
        if (memoryTrend > _memoryLeakThreshold / 10) // 10% of leak threshold per snapshot
        {
            _logger.LogWarning("Memory usage trending upward: +{Trend:N0} bytes per second", memoryTrend);
        }

        if (gcTrend.Gen2Frequency > 0.5) // More than 0.5 Gen2 collections per second
        {
            _logger.LogWarning("High Gen2 GC frequency detected: {Frequency:F2} collections/second", 
                gcTrend.Gen2Frequency);
        }
    }

    /// <summary>
    /// Get comprehensive performance report
    /// </summary>
    public OverlayPerformanceReport GetPerformanceReport()
    {
        var snapshots = _snapshots.ToArray();
        var activeMetrics = _overlayMetrics.Values.ToArray();

        return new OverlayPerformanceReport
        {
            GeneratedAt = DateTime.Now,
            SnapshotCount = snapshots.Length,
            ActiveOverlayCount = activeMetrics.Length,
            AverageMemoryUsage = snapshots.Length > 0 ? snapshots.Average(s => s.TotalMemory) : 0,
            PeakMemoryUsage = snapshots.Length > 0 ? snapshots.Max(s => s.TotalMemory) : 0,
            TotalGCCollections = snapshots.Length > 0 ? snapshots.Last().Generation0Collections + 
                               snapshots.Last().Generation1Collections + snapshots.Last().Generation2Collections : 0,
            ActiveOverlays = activeMetrics.Select(m => new ActiveOverlayInfo
            {
                OverlayType = m.OverlayType,
                Duration = m.Duration,
                MemoryUsage = m.MemoryDelta,
                StartTime = m.StartTime
            }).ToList(),
            PerformanceTrends = CalculateOverallTrends(snapshots)
        };
    }

    private double CalculateMemoryTrend(PerformanceSnapshot[] snapshots)
    {
        if (snapshots.Length < 2) return 0;

        var first = snapshots.First();
        var last = snapshots.Last();
        var timeDiff = (last.Timestamp - first.Timestamp).TotalSeconds;

        return timeDiff > 0 ? (last.TotalMemory - first.TotalMemory) / timeDiff : 0;
    }

    private GCTrend CalculateGCTrend(PerformanceSnapshot[] snapshots)
    {
        if (snapshots.Length < 2) return new GCTrend();

        var first = snapshots.First();
        var last = snapshots.Last();
        var timeDiff = (last.Timestamp - first.Timestamp).TotalSeconds;

        return new GCTrend
        {
            Gen0Frequency = timeDiff > 0 ? (last.Generation0Collections - first.Generation0Collections) / timeDiff : 0,
            Gen1Frequency = timeDiff > 0 ? (last.Generation1Collections - first.Generation1Collections) / timeDiff : 0,
            Gen2Frequency = timeDiff > 0 ? (last.Generation2Collections - first.Generation2Collections) / timeDiff : 0
        };
    }

    public void Dispose()
    {
        _monitoringTimer?.Dispose();
        _overlayMetrics.Clear();
        while (_snapshots.TryDequeue(out _)) { }
    }
}

/// <summary>
/// Individual overlay performance tracking
/// </summary>
public class OverlayPerformanceTracker : IDisposable
{
    private readonly string _key;
    private readonly OverlayPerformanceMonitor _monitor;
    private readonly ILogger _logger;
    private readonly DateTime _startTime;

    public OverlayPerformanceTracker(string key, OverlayPerformanceMonitor monitor, ILogger logger)
    {
        _key = key;
        _monitor = monitor;
        _logger = logger;
        _startTime = DateTime.Now;
        
        _logger.LogDebug("Started tracking overlay performance: {Key}", key);
    }

    public void Dispose()
    {
        var duration = DateTime.Now - _startTime;
        _monitor.EndTracking(_key, OverlayResult.Dismissed);
        
        _logger.LogDebug("Stopped tracking overlay performance: {Key}, Duration: {Duration}ms", 
            _key, duration.TotalMilliseconds);
    }
}
```

### **4. Optimization Guidelines**

#### **Memory Optimization Checklist**

```markdown
## Memory Optimization Checklist

### ViewModel Level
- [ ] Implement IResettable for pooling support
- [ ] Use WeakReference for parent tracking
- [ ] Dispose of heavy resources in OnReset()
- [ ] Implement lazy loading for expensive properties
- [ ] Clear collections in Reset() method
- [ ] Unsubscribe from events in Dispose()

### Service Level  
- [ ] Configure appropriate pool sizes
- [ ] Monitor pool hit rates (target: >80%)
- [ ] Implement periodic pool cleanup
- [ ] Use object pooling for frequently created objects
- [ ] Track active overlay counts
- [ ] Set maximum concurrent overlay limits

### View Level
- [ ] Use virtualization for large lists
- [ ] Implement resource caching
- [ ] Minimize AXAML resource duplication
- [ ] Use shared resources for common elements
- [ ] Optimize image loading and caching
- [ ] Implement view state preservation
```

#### **Performance Best Practices**

```csharp
// ✅ GOOD: Efficient overlay initialization
public async Task InitializeOverlayAsync<T>(T request) where T : BaseOverlayRequest
{
    // Load only essential data immediately
    Title = request.Title;
    IsVisible = true;
    
    // Defer heavy operations
    _ = Task.Run(async () =>
    {
        await LoadHeavyDataAsync();
        await InvokeUI(() => OnHeavyDataLoaded());
    });
}

// ❌ BAD: Blocking overlay initialization
public async Task InitializeOverlayAsync<T>(T request) where T : BaseOverlayRequest
{
    Title = request.Title;
    
    // This blocks UI thread
    var heavyData = await LoadHeavyDataAsync();
    ProcessHeavyData(heavyData);
    
    IsVisible = true;
}
```

## 📊 Performance Metrics and KPIs

### **Key Performance Indicators**

1. **Overlay Display Time**: < 100ms for simple overlays, < 300ms for complex overlays
2. **Memory Pool Hit Rate**: > 80% for frequently used overlays  
3. **Memory Growth Rate**: < 1MB/minute during normal operation
4. **GC Pressure**: < 10 Gen0 collections per overlay display
5. **Resource Cleanup Time**: < 50ms for overlay disposal

### **Monitoring Dashboard Metrics**

```csharp
public class OverlayPerformanceDashboard
{
    public class MetricsModel
    {
        public double AverageDisplayTime { get; set; }
        public double PoolHitRate { get; set; }
        public long CurrentMemoryUsage { get; set; }
        public double MemoryGrowthRate { get; set; }
        public int ActiveOverlayCount { get; set; }
        public Dictionary<string, int> OverlayTypeDistribution { get; set; } = new();
        public List<PerformanceAlert> ActiveAlerts { get; set; } = new();
    }

    public enum AlertLevel { Info, Warning, Critical }
    
    public class PerformanceAlert
    {
        public AlertLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
```

### **Automated Performance Testing**

```csharp
[TestClass]
public class OverlayPerformanceTests
{
    [TestMethod]
    public async Task OverlayPool_MemoryUsage_ShouldStayWithinLimits()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        var poolManager = new OverlayPoolManager(_mockLogger.Object);
        
        // Act - Create and return many overlays
        for (int i = 0; i < 1000; i++)
        {
            var overlay = poolManager.GetOrCreate<TestOverlayViewModel>();
            poolManager.Return(overlay);
        }
        
        // Force cleanup
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var finalMemory = GC.GetTotalMemory(false);
        var memoryGrowth = finalMemory - initialMemory;
        
        // Assert - Memory growth should be minimal due to pooling
        memoryGrowth.Should().BeLessThan(10 * 1024 * 1024); // Less than 10MB growth
    }

    [TestMethod]
    public async Task OverlayDisplay_Performance_ShouldMeetTargets()
    {
        // Arrange
        var service = CreateUniversalOverlayService();
        var request = new ConfirmationOverlayRequest("Test", "Test message");
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        var response = await service.ShowConfirmationOverlayAsync(request);
        stopwatch.Stop();
        
        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(300); // Under 300ms target
    }
}
```

---

**This performance guide ensures the overlay system operates efficiently while providing comprehensive monitoring and optimization strategies for the MTM WIP Application.**
