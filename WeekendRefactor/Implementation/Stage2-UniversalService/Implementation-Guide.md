# Stage 2: Universal Service Foundation - Implementation Guide

**Priority**: 🟡 High  
**Timeline**: Weekend Day 1-2  
**Estimated Time**: 6-8 hours  

## 🎯 Overview

This stage creates the Universal Overlay Service that consolidates all overlay management into a single, consistent interface. This is the foundation for all future overlay improvements.

## 📋 Task Breakdown

### **Task 2.1: Design Universal Overlay Service Interface**

**Estimated Time**: 2 hours  
**Risk Level**: Medium  
**Dependencies**: None  

#### **Files to Create**

```
Services/IUniversalOverlayService.cs
Models/Overlay/OverlayRequest.cs
Models/Overlay/OverlayResult.cs
Models/Overlay/OverlayConfiguration.cs
```

#### **Core Interface Design**

```csharp
// Services/IUniversalOverlayService.cs
namespace MTM_WIP_Application_Avalonia.Services;

public interface IUniversalOverlayService
{
    // Message Overlays
    Task<OverlayResult> ShowMessageAsync(MessageOverlayRequest request);
    Task<OverlayResult> ShowErrorAsync(string title, string message, Exception? exception = null);
    Task<OverlayResult> ShowSuccessAsync(string message, string? details = null, int duration = 2000);
    Task<OverlayResult> ShowWarningAsync(string title, string message, string? details = null);
    Task<OverlayResult> ShowInfoAsync(string title, string message, string? details = null);
    
    // Confirmation Overlays
    Task<OverlayResult> ShowConfirmationAsync(ConfirmationOverlayRequest request);
    Task<OverlayResult> ShowBatchConfirmationAsync(string operation, int itemCount, List<string>? warnings = null);
    Task<OverlayResult> ShowDeleteConfirmationAsync(string itemName, bool isPermanent = true);
    
    // Progress Overlays
    Task<IProgressTracker> ShowProgressAsync(ProgressOverlayRequest request);
    Task HideProgressAsync();
    
    // Specialized Overlays
    Task<OverlayResult> ShowSuggestionAsync(SuggestionOverlayRequest request);
    Task<OverlayResult> ShowEditDialogAsync(EditDialogOverlayRequest request);
    Task<OverlayResult> ShowValidationAsync(ValidationOverlayRequest request);
    
    // Container Management
    Task<Control?> FindOverlayContainerAsync(Control? sourceControl = null);
    Task RegisterOverlayContainerAsync(Control container, string? containerName = null);
    Task<bool> IsOverlayActiveAsync(string overlayType);
    Task DismissAllOverlaysAsync();
}
```

#### **Supporting Models**

```csharp
// Models/Overlay/OverlayRequest.cs
public abstract class OverlayRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Control? SourceControl { get; set; }
    public Control? TargetContainer { get; set; }
    public bool IsModal { get; set; } = true;
    public TimeSpan? AutoDismissAfter { get; set; }
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}

public class MessageOverlayRequest : OverlayRequest
{
    public MessageType Type { get; set; } = MessageType.Information;
    public string IconKind { get; set; } = "Information";
    public List<OverlayAction> Actions { get; set; } = new();
}

public class ConfirmationOverlayRequest : OverlayRequest
{
    public string PrimaryActionText { get; set; } = "Confirm";
    public string SecondaryActionText { get; set; } = "Cancel";
    public ConfirmationType Type { get; set; } = ConfirmationType.Question;
    public List<string> Warnings { get; set; } = new();
}

public class ProgressOverlayRequest : OverlayRequest
{
    public bool CanCancel { get; set; } = true;
    public bool ShowPercentage { get; set; } = true;
    public bool ShowTimeRemaining { get; set; } = true;
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}
```

#### **Validation Checklist**

- [ ] Interface covers all existing overlay types
- [ ] Request/Response pattern is consistent
- [ ] Container management is flexible
- [ ] Extension points for future overlay types

---

### **Task 2.2: Implement Universal Overlay Service**

**Estimated Time**: 3-4 hours  
**Risk Level**: High  
**Dependencies**: Task 2.1  

#### **Files to Create**

```
Services/UniversalOverlayService.cs
Services/OverlayContainerManager.cs
ViewModels/Overlay/UniversalOverlayViewModel.cs
```

#### **Core Service Implementation**

```csharp
// Services/UniversalOverlayService.cs
[ServiceLifetime(ServiceLifetime.Singleton)]
public class UniversalOverlayService : IUniversalOverlayService
{
    private readonly ILogger<UniversalOverlayService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly OverlayContainerManager _containerManager;
    private readonly Dictionary<string, WeakReference<UserControl>> _activeOverlays = new();
    
    public UniversalOverlayService(
        ILogger<UniversalOverlayService> logger,
        IServiceProvider serviceProvider,
        OverlayContainerManager containerManager)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _containerManager = containerManager ?? throw new ArgumentNullException(nameof(containerManager));
    }
    
    public async Task<OverlayResult> ShowMessageAsync(MessageOverlayRequest request)
    {
        try
        {
            var container = await FindOrCreateOverlayContainerAsync(request.SourceControl);
            if (container == null)
            {
                _logger.LogWarning("No suitable overlay container found for message overlay");
                return OverlayResult.Failed("No container available");
            }
            
            // Route to appropriate specialized service or create unified message overlay
            return request.Type switch
            {
                MessageType.Success => await ShowSuccessOverlayAsync(request, container),
                MessageType.Error => await ShowErrorOverlayAsync(request, container),
                MessageType.Warning => await ShowWarningOverlayAsync(request, container),
                MessageType.Information => await ShowInfoOverlayAsync(request, container),
                _ => throw new ArgumentOutOfRangeException(nameof(request.Type))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show message overlay: {Message}", request.Message);
            return OverlayResult.Failed($"Failed to show overlay: {ex.Message}");
        }
    }
    
    private async Task<Control?> FindOrCreateOverlayContainerAsync(Control? sourceControl)
    {
        // Try to find existing container
        var container = await _containerManager.FindBestContainerAsync(sourceControl);
        if (container != null) return container;
        
        // Create new container if needed
        return await _containerManager.CreateOverlayContainerAsync(sourceControl);
    }
}
```

#### **Container Management**

```csharp
// Services/OverlayContainerManager.cs
public class OverlayContainerManager
{
    private readonly ILogger<OverlayContainerManager> _logger;
    private readonly Dictionary<string, WeakReference<Control>> _registeredContainers = new();
    
    public async Task<Control?> FindBestContainerAsync(Control? sourceControl)
    {
        // Strategy 1: Use explicitly provided target container
        if (sourceControl?.Tag is Control explicitContainer)
            return explicitContainer;
        
        // Strategy 2: Walk up visual tree to find suitable container
        var current = sourceControl;
        while (current != null)
        {
            // Look for containers with overlay support
            if (current.Classes.Contains("overlay-container") ||
                current.Name?.EndsWith("Container") == true ||
                current.FindNameScope()?.Find("OverlayContainer") != null)
            {
                return current;
            }
            current = current.Parent as Control;
        }
        
        // Strategy 3: Find MainView or MainWindow
        return await FindMainContainerAsync();
    }
    
    private async Task<Control?> FindMainContainerAsync()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow;
            
            // Look for MainView container
            var mainView = mainWindow?.FindNameScope()?.Find("MainView") as Control;
            if (mainView != null) return mainView;
            
            // Use MainWindow as fallback
            return mainWindow;
        }
        
        return null;
    }
    
    public async Task<Control?> CreateOverlayContainerAsync(Control? sourceControl)
    {
        // For views that don't have overlay containers, we need to add them
        // This might require updating the view structure
        
        if (sourceControl is Panel panel)
        {
            // Add overlay container to existing panel
            var overlayContainer = new Border
            {
                Name = "DynamicOverlayContainer",
                IsVisible = false,
                Background = new SolidColorBrush(Colors.Transparent),
                Child = new Grid()
            };
            
            Panel.SetZIndex(overlayContainer, 1000);
            panel.Children.Add(overlayContainer);
            
            return overlayContainer;
        }
        
        return null;
    }
}
```

#### **Validation Checklist**

- [ ] Service implements all interface methods
- [ ] Container finding logic works across different view types
- [ ] Error handling and logging is comprehensive
- [ ] Memory management prevents leaks

---

### **Task 2.3: Update Service Registration Patterns**

**Estimated Time**: 1-2 hours  
**Risk Level**: Medium  
**Dependencies**: Task 2.2  

#### **Files to Update**

```
Extensions/ServiceCollectionExtensions.cs
Program.cs (or App.axaml.cs)
```

#### **Service Registration Updates**

```csharp
// Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMOverlayServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Universal Overlay Service (Primary)
        services.AddSingleton<IUniversalOverlayService, UniversalOverlayService>();
        services.AddSingleton<OverlayContainerManager>();
        
        // Specialized Services (Legacy Support)
        services.AddScoped<ISuggestionOverlayService, SuggestionOverlayService>();
        services.AddScoped<ISuccessOverlayService, SuccessOverlayService>();
        
        // Overlay ViewModels
        services.AddTransient<GlobalErrorOverlayViewModel>();
        services.AddTransient<ProgressOverlayViewModel>();
        services.AddTransient<ValidationOverlayViewModel>();
        services.AddTransient<BatchConfirmationOverlayViewModel>();
        
        // Configuration
        services.Configure<OverlayConfiguration>(configuration.GetSection("Overlays"));
        
        return services;
    }
    
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Existing services...
        
        // Add overlay services
        services.AddMTMOverlayServices(configuration);
        
        return services;
    }
}
```

#### **Configuration Setup**

```json
// appsettings.json - Add overlay configuration section
{
  "Overlays": {
    "DefaultAnimationDuration": "300ms",
    "AutoDismissTimeout": "3000ms",
    "MaxSimultaneousOverlays": 3,
    "EnableOverlayPooling": true,
    "ContainerSearchStrategy": "WalkUpVisualTree",
    "FallbackToMainWindow": true
  }
}
```

#### **Validation Checklist**

- [ ] Universal service is registered as singleton
- [ ] Existing services remain functional during transition
- [ ] Configuration binding works correctly
- [ ] DI resolution works in all contexts

---

### **Task 2.4: Create Service Integration Tests**

**Estimated Time**: 1-2 hours  
**Risk Level**: Low  
**Dependencies**: Tasks 2.1-2.3  

#### **Files to Create**

```
Tests/Services/UniversalOverlayServiceTests.cs
Tests/Services/OverlayContainerManagerTests.cs
Tests/Integration/OverlayServiceIntegrationTests.cs
```

#### **Core Service Tests**

```csharp
// Tests/Services/UniversalOverlayServiceTests.cs
[TestClass]
public class UniversalOverlayServiceTests
{
    private IUniversalOverlayService _service;
    private Mock<ILogger<UniversalOverlayService>> _mockLogger;
    private Mock<IServiceProvider> _mockServiceProvider;
    
    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<UniversalOverlayService>>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        
        var containerManager = new OverlayContainerManager(
            Mock.Of<ILogger<OverlayContainerManager>>());
            
        _service = new UniversalOverlayService(
            _mockLogger.Object, 
            _mockServiceProvider.Object, 
            containerManager);
    }
    
    [TestMethod]
    public async Task ShowMessageAsync_WithValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var request = new MessageOverlayRequest
        {
            Title = "Test Message",
            Message = "This is a test",
            Type = MessageType.Information
        };
        
        // Act
        var result = await _service.ShowMessageAsync(request);
        
        // Assert
        Assert.IsNotNull(result);
        // Add appropriate assertions based on expected behavior
    }
    
    [TestMethod]
    public async Task FindOverlayContainerAsync_WithNullSource_FindsMainContainer()
    {
        // Arrange & Act
        var container = await _service.FindOverlayContainerAsync(null);
        
        // Assert
        // Add assertions based on container finding logic
    }
}
```

#### **Integration Tests**

```csharp
// Tests/Integration/OverlayServiceIntegrationTests.cs
[TestClass]
public class OverlayServiceIntegrationTests
{
    [TestMethod]
    public async Task OverlayService_IntegratesWithExistingServices_Successfully()
    {
        // Test that Universal service works alongside existing ISuggestionOverlayService
        // Test that service registration doesn't break existing functionality
        // Test that container finding works across different view types
    }
}
```

#### **Validation Checklist**

- [ ] Unit tests cover core service functionality
- [ ] Integration tests verify service compatibility
- [ ] Container finding logic is thoroughly tested
- [ ] Error handling scenarios are covered

## 🔄 Migration Strategy

### **Gradual Migration Approach**

1. **Phase A**: Create Universal Service alongside existing services
2. **Phase B**: Update new overlay implementations to use Universal Service
3. **Phase C**: Gradually migrate existing overlays to Universal Service
4. **Phase D**: Deprecate individual overlay services (future stage)

### **Backward Compatibility**

- Keep existing `ISuggestionOverlayService` and `ISuccessOverlayService` functional
- Universal Service can delegate to existing services where appropriate
- No breaking changes to existing overlay usage

## 🧪 Testing Strategy

### **Service Testing**

1. **Unit Tests**: Core service logic and container management
2. **Integration Tests**: Service registration and DI resolution
3. **UI Tests**: Overlay display and interaction in real views

### **Performance Testing**

1. **Memory Usage**: Monitor overlay creation and disposal
2. **Container Finding**: Measure performance of container detection
3. **Service Resolution**: Verify DI performance with new registrations

## ⚠️ Risk Mitigation

### **Potential Issues**

1. **Container Detection Complexity**
   - Different view structures may not have suitable containers
   - Visual tree walking may be performance-intensive

2. **Service Registration Conflicts**
   - New registrations might interfere with existing services
   - DI resolution order may affect behavior

3. **Memory Management**
   - WeakReference usage needs careful handling
   - Overlay disposal must prevent memory leaks

### **Mitigation Strategies**

1. **Extensive Testing**: Test with all major view types
2. **Gradual Rollout**: Implement incrementally with fallbacks
3. **Performance Monitoring**: Track memory and performance impact
4. **Documentation**: Clear migration path for developers

## ✅ Stage Completion Criteria

- [ ] Universal Overlay Service interface is complete and well-designed
- [ ] Core service implementation handles all overlay types
- [ ] Service registration works without breaking existing functionality
- [ ] Container management successfully finds appropriate parents
- [ ] Integration tests verify service compatibility
- [ ] Performance impact is within acceptable bounds
- [ ] Documentation covers service usage and migration

**Stage 2 Success**: Universal Overlay Service provides consistent, extensible foundation for all overlay management across the application.
