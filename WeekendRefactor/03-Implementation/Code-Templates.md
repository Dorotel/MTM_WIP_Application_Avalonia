# Implementation Code Templates

This file contains code templates and patterns to ensure consistency across all overlay implementations.

## Universal Service Integration Pattern

### **Service Registration Template**

```csharp
// Add to ServiceCollectionExtensions.cs
public static IServiceCollection AddNewOverlayService(this IServiceCollection services)
{
    // Register ViewModel
    services.AddTransient<NewOverlayViewModel>();
    
    // Register specialized service (if needed)
    services.AddScoped<INewOverlayService, NewOverlayService>();
    
    return services;
}
```

### **ViewModel Base Pattern**

```csharp
[ObservableObject]
public partial class NewOverlayViewModel : BaseViewModel
{
    // Required properties
    [ObservableProperty] private bool isVisible = false;
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string message = string.Empty;
    
    // Optional properties
    [ObservableProperty] private string? details;
    [ObservableProperty] private OverlayResult result = OverlayResult.None;
    
    // Commands
    [RelayCommand]
    private async Task DismissAsync()
    {
        IsVisible = false;
        Result = OverlayResult.Dismissed;
        // Notify completion if needed
    }
    
    // Constructor with required services
    public NewOverlayViewModel(ILogger<NewOverlayViewModel> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
    }
}
```

### **View AXAML Template**

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             xmlns:materialIcons="using:Material.Icons.Avalonia"
             x:Class="MTM_WIP_Application_Avalonia.Views.Overlay.NewOverlayView"
             x:DataType="vm:NewOverlayViewModel">

    <!-- Standard overlay container -->
    <Border Background="#80000000" 
            IsVisible="{Binding IsVisible}"
            HorizontalAlignment="Center"
            VerticalAlignment="Center">
            
        <Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
                BorderThickness="1"
                CornerRadius="8"
                MaxWidth="500"
                Padding="20">
                
            <StackPanel Spacing="16">
                <!-- Header with icon -->
                <StackPanel Orientation="Horizontal" Spacing="12">
                    <materialIcons:MaterialIcon Kind="Information" 
                                              Width="24" Height="24"
                                              Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
                    <TextBlock Text="{Binding Title}"
                              FontSize="16"
                              FontWeight="SemiBold" />
                </StackPanel>
                
                <!-- Message -->
                <TextBlock Text="{Binding Message}"
                          TextWrapping="Wrap"
                          FontSize="14" />
                          
                <!-- Details (if any) -->
                <TextBlock Text="{Binding Details}"
                          TextWrapping="Wrap"
                          FontSize="12"
                          Foreground="{DynamicResource MTM_Shared_Logic.SecondaryTextBrush}"
                          IsVisible="{Binding Details, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
                
                <!-- Action buttons -->
                <StackPanel Orientation="Horizontal" 
                           HorizontalAlignment="Right" 
                           Spacing="8">
                    <Button Content="Close"
                           Command="{Binding DismissCommand}"
                           Classes="secondary" />
                </StackPanel>
            </StackPanel>
        </Border>
    </Border>
</UserControl>
```

### **View Code-Behind Template**

```csharp
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

public partial class NewOverlayView : UserControl
{
    public NewOverlayView()
    {
        InitializeComponent();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Cleanup if needed
        base.OnDetachedFromVisualTree(e);
    }
}
```

## View Integration Patterns

### **Adding Overlay Container to Existing View**

```xml
<!-- Add to existing View.axaml -->
<Grid>
    <!-- Existing content -->
    <StackPanel>
        <!-- Existing view content here -->
    </StackPanel>
    
    <!-- Add overlay container -->
    <Border x:Name="OverlayContainer" 
            IsVisible="False"
            Background="Transparent"
            ZIndex="100">
        <Grid>
            <!-- Individual overlays will be added here programmatically or via binding -->
        </Grid>
    </Border>
</Grid>
```

### **ViewModel Integration Pattern**

```csharp
[ObservableObject]
public partial class ExistingViewModel : BaseViewModel
{
    // Add overlay-related properties
    [ObservableProperty] private bool showNewOverlay = false;
    [ObservableProperty] private NewOverlayViewModel? newOverlayViewModel;
    
    // Add overlay service injection
    private readonly IUniversalOverlayService _overlayService;
    
    public ExistingViewModel(
        ILogger<ExistingViewModel> logger,
        IUniversalOverlayService overlayService) : base(logger)
    {
        _overlayService = overlayService ?? throw new ArgumentNullException(nameof(overlayService));
    }
    
    // Example usage method
    [RelayCommand]
    private async Task ShowOverlayExampleAsync()
    {
        var request = new MessageOverlayRequest
        {
            Title = "Example Overlay",
            Message = "This is an example overlay message",
            Type = MessageType.Information,
            SourceControl = null // Will be determined automatically
        };
        
        var result = await _overlayService.ShowMessageAsync(request);
        
        if (result.IsConfirmed)
        {
            // Handle confirmation
        }
    }
}
```

## Service Implementation Patterns

### **Specialized Service Template**

```csharp
// Interface
public interface INewOverlayService
{
    Task<OverlayResult> ShowNewOverlayAsync(NewOverlayRequest request);
}

// Implementation
public class NewOverlayService : INewOverlayService
{
    private readonly ILogger<NewOverlayService> _logger;
    private readonly IUniversalOverlayService _universalService;
    
    public NewOverlayService(
        ILogger<NewOverlayService> logger,
        IUniversalOverlayService universalService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _universalService = universalService ?? throw new ArgumentNullException(nameof(universalService));
    }
    
    public async Task<OverlayResult> ShowNewOverlayAsync(NewOverlayRequest request)
    {
        try
        {
            _logger.LogDebug("Showing new overlay: {Title}", request.Title);
            
            // Use Universal Service for display
            return await _universalService.ShowMessageAsync(new MessageOverlayRequest
            {
                Title = request.Title,
                Message = request.Message,
                Type = MessageType.Information,
                SourceControl = request.SourceControl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show new overlay");
            return OverlayResult.Failed($"Failed to show overlay: {ex.Message}");
        }
    }
}
```

## Testing Patterns

### **ViewModel Unit Test Template**

```csharp
[TestClass]
public class NewOverlayViewModelTests
{
    private NewOverlayViewModel _viewModel;
    private Mock<ILogger<NewOverlayViewModel>> _mockLogger;
    
    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<NewOverlayViewModel>>();
        _viewModel = new NewOverlayViewModel(_mockLogger.Object);
    }
    
    [TestMethod]
    public async Task DismissAsync_SetsIsVisibleToFalse()
    {
        // Arrange
        _viewModel.IsVisible = true;
        
        // Act
        await _viewModel.DismissCommand.ExecuteAsync(null);
        
        // Assert
        Assert.IsFalse(_viewModel.IsVisible);
        Assert.AreEqual(OverlayResult.Dismissed, _viewModel.Result);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _viewModel?.Dispose();
    }
}
```

### **Service Integration Test Template**

```csharp
[TestClass]
public class NewOverlayServiceIntegrationTests
{
    private ServiceProvider _serviceProvider;
    private INewOverlayService _service;
    
    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMTMOverlayServices(new ConfigurationBuilder().Build());
        
        _serviceProvider = services.BuildServiceProvider();
        _service = _serviceProvider.GetRequiredService<INewOverlayService>();
    }
    
    [TestMethod]
    public async Task ShowNewOverlayAsync_WithValidRequest_ReturnsResult()
    {
        // Arrange
        var request = new NewOverlayRequest
        {
            Title = "Test",
            Message = "Test message"
        };
        
        // Act
        var result = await _service.ShowNewOverlayAsync(request);
        
        // Assert
        Assert.IsNotNull(result);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _serviceProvider?.Dispose();
    }
}
```

## Error Handling Patterns

### **Service Error Handling**

```csharp
public async Task<OverlayResult> OperationWithOverlayAsync()
{
    try
    {
        // Perform operation
        var result = await SomeOperation();
        
        // Show success overlay
        await _overlayService.ShowSuccessAsync("Operation completed successfully");
        
        return OverlayResult.Success;
    }
    catch (DatabaseException dbEx)
    {
        // Show specific database error overlay
        await _overlayService.ShowErrorAsync(
            "Database Error", 
            "Unable to complete operation due to database issue", 
            dbEx);
            
        return OverlayResult.Failed("Database error");
    }
    catch (ValidationException valEx)
    {
        // Show validation overlay
        await _overlayService.ShowValidationAsync(new ValidationOverlayRequest
        {
            Title = "Validation Required",
            Message = "Please correct the following errors",
            ValidationErrors = valEx.Errors
        });
        
        return OverlayResult.ValidationFailed;
    }
    catch (Exception ex)
    {
        // Show general error overlay
        await _overlayService.ShowErrorAsync(
            "Unexpected Error", 
            "An unexpected error occurred", 
            ex);
            
        return OverlayResult.Failed($"Unexpected error: {ex.Message}");
    }
}
```

## Memory Management Patterns

### **Overlay Disposal Pattern**

```csharp
public class ManagedOverlayViewModel : BaseViewModel, IDisposable
{
    private bool _disposed = false;
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Dispose managed resources
            _cancellationTokenSource?.Dispose();
            
            // Clear event handlers
            PropertyChanged = null;
            
            // Clear collections
            ValidationErrors?.Clear();
        }
        
        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

These templates ensure consistent implementation across all overlay types while following established MTM WIP Application patterns.
