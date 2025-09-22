# Universal Progress Overlay Implementation Guide

## Overview

The Universal Progress Overlay system provides a consistent, cross-application progress indication mechanism for the MTM WIP Application. This system replaces the previous MainView progress bar with a centralized, overlay-based approach that works across all views and operations.

## Architecture

### Core Components

1. **`IProgressOverlayService`** - Service interface for progress overlay management
2. **`ProgressOverlayService`** - Concrete implementation with proper UI thread handling
3. **`ProgressOverlayViewModel`** - MVVM Community Toolkit ViewModel for overlay state
4. **`ProgressOverlayView.axaml`** - Avalonia UserControl with MTM theme integration

### Service Registration

```csharp
// In ServiceCollectionExtensions.cs
services.TryAddSingleton<IProgressOverlayService, ProgressOverlayService>();
```

## Implementation Guide

### Step 1: Service Integration

#### ViewModel Constructor Injection

```csharp
public class YourViewModel : BaseViewModel
{
    private readonly IProgressOverlayService _progressOverlayService;

    public YourViewModel(
        ILogger<YourViewModel> logger,
        IProgressOverlayService progressOverlayService)
        : base(logger)
    {
        _progressOverlayService = progressOverlayService ?? throw new ArgumentNullException(nameof(progressOverlayService));
    }

    // Expose service for UI binding
    public IProgressOverlayService ProgressOverlayService => _progressOverlayService;
}
```

### Step 2: View Integration

#### AXAML Overlay Panel

```xml
<!-- Add to any view that needs progress overlay support -->
<Panel x:Name="ProgressOverlayPanel"
       ZIndex="9999"
       Background="rgba(0,0,0,0.6)"
       IsVisible="{Binding ProgressOverlayService.IsOverlayVisible, FallbackValue=False}"
       HorizontalAlignment="Stretch"
       VerticalAlignment="Stretch">
  
  <overlay:ProgressOverlayView
    x:Name="ProgressOverlay"
    DataContext="{Binding ProgressOverlayService.CurrentProgressOverlay}"
    HorizontalAlignment="Center"
    VerticalAlignment="Center" />
</Panel>
```

### Step 3: Usage Patterns

#### Basic Determinate Progress

```csharp
public async Task PerformOperationAsync()
{
    try
    {
        // Show progress overlay
        await _progressOverlayService.ShowProgressOverlayAsync(
            "Processing Data",
            "Initializing operation...",
            isDeterminate: true,
            cancellable: true);

        // Simulate work with progress updates
        for (int i = 0; i <= 100; i += 10)
        {
            await Task.Delay(100); // Simulate work
            await _progressOverlayService.UpdateProgressOverlayAsync(
                i,
                $"Processing step {i / 10 + 1} of 11...",
                $"Current progress: {i}%");
        }

        // Complete with auto-dismiss
        await _progressOverlayService.CompleteProgressOverlayAsync(
            "Operation completed successfully!",
            autoCloseDelayMs: 2000);
    }
    catch (Exception ex)
    {
        await _progressOverlayService.SetProgressOverlayErrorAsync(
            "Operation failed",
            ex.Message);
    }
}

```

#### Indeterminate Progress

```csharp
public async Task LoadDataAsync()
{
    try
    {
        await _progressOverlayService.ShowProgressOverlayAsync(
            "Loading Data",
            "Please wait while data is being loaded...",
            isDeterminate: false,
            cancellable: false);

        // Perform async operation
        var result = await SomeAsyncOperation();

        await _progressOverlayService.CompleteProgressOverlayAsync(
            "Data loaded successfully",
            autoCloseDelayMs: 1500);
    }
    catch (Exception ex)
    {
        await _progressOverlayService.SetProgressOverlayErrorAsync(
            "Failed to load data",
            ex.Message);
    }

}
```

#### Quick Operations (No Progress Updates)

```csharp
public async Task QuickOperationAsync()
{
    await _progressOverlayService.ShowProgressOverlayAsync(
        "Saving Changes",
        "Please wait...",
        isDeterminate: false);

    try
    {
        await SaveChangesAsync();
        await _progressOverlayService.CompleteProgressOverlayAsync(
            "Changes saved successfully!",
            autoCloseDelayMs: 1000);
    }
    catch (Exception ex)
    {
        await _progressOverlayService.SetProgressOverlayErrorAsync(
            "Save failed",
            ex.Message);
    }
}
```

### Step 4: View Switching Progress Overlays

#### Tab Switching Implementation

For main view tab switching between Inventory, Remove, and Transfer:

```csharp
private async void OnTabSelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    try
    {
        var tabControl = sender as TabControl;
        if (tabControl == null) return;

        var newIndex = tabControl.SelectedIndex;

        // Show progress overlay for view switching
        var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
        if (progressOverlayService != null)
        {
            var tabNames = new[] { "Inventory", "Remove", "Transfer" };
            var targetTabName = newIndex >= 0 && newIndex < tabNames.Length ? 
                tabNames[newIndex] : "View";
            
            await progressOverlayService.ShowProgressOverlayAsync(
                title: "Switching View",
                statusMessage: $"Loading {targetTabName} tab...",
                isDeterminate: false,
                cancellable: false
            );
        }

        // Clear inputs and perform view switching logic
        ClearAllTabInputsImmediate();
        await Task.Delay(200); // Brief loading time

        // Hide progress overlay
        if (progressOverlayService != null)
        {
            await progressOverlayService.HideProgressOverlayAsync();
        }
    }
    catch (Exception ex)
    {
        // Always hide progress overlay on error
        var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
        if (progressOverlayService != null)
        {
            await progressOverlayService.HideProgressOverlayAsync();
        }
        
        System.Diagnostics.Debug.WriteLine($"Error handling tab selection change: {ex.Message}");
    }
}
```

#### Advanced View Switching

For switching to advanced entry/removal views:

```csharp
private async void OnAdvancedEntryRequested(object? sender, EventArgs e)
{
    try
    {
        // Show progress overlay for advanced view switching
        var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
        if (progressOverlayService != null)
        {
            await progressOverlayService.ShowProgressOverlayAsync(
                title: "Loading Advanced View",
                statusMessage: "Preparing advanced inventory entry...",
                isDeterminate: false,
                cancellable: false
            );
        }

        // Clear inputs and prepare advanced view
        ClearInventoryTabInputs();
        await Task.Delay(150); // Preparation time

        // Hide progress overlay
        if (progressOverlayService != null)
        {
            await progressOverlayService.HideProgressOverlayAsync();
        }
    }
    catch (Exception ex)
    {
        // Hide progress overlay on error
        var progressOverlayService = Program.GetOptionalService<Services.IProgressOverlayService>();
        if (progressOverlayService != null)
        {
            await progressOverlayService.HideProgressOverlayAsync();
        }
        
        System.Diagnostics.Debug.WriteLine($"Error handling Advanced Entry request: {ex.Message}");
    }
}
```

#### Key Implementation Notes

- **Non-cancellable overlays**: View switching operations use `cancellable: false`
- **Brief timing**: View switches use 150-200ms delay for smooth transitions
- **Error handling**: Always hide overlay in catch blocks to prevent stuck states
- **Descriptive messages**: Use specific messages like "Loading Inventory tab..." or "Preparing advanced inventory entry..."
- **Service availability**: Use `Program.GetOptionalService<>()` to safely access the service

## API Reference

### IProgressOverlayService Methods

#### ShowProgressOverlayAsync

```csharp

Task ShowProgressOverlayAsync(
    string title,
    string? statusMessage = null,
    bool isDeterminate = false,
    bool cancellable = true)
```

- **title**: Main operation title
- **statusMessage**: Optional detailed status
- **isDeterminate**: true for progress bar, false for spinner
- **cancellable**: Whether operation can be cancelled

#### UpdateProgressOverlayAsync

```csharp
Task UpdateProgressOverlayAsync(
    double value,

    string? statusMessage = null,
    string? details = null)
```

- **value**: Progress percentage (0-100)

- **statusMessage**: Updated status message
- **details**: Additional progress details

#### CompleteProgressOverlayAsync

```csharp
Task CompleteProgressOverlayAsync(
    string? completionMessage = null,
    int autoCloseDelayMs = 2000)

```

- **completionMessage**: Success message to display
- **autoCloseDelayMs**: Auto-dismiss delay (0 = no auto-dismiss)

#### SetProgressOverlayErrorAsync

```csharp
Task SetProgressOverlayErrorAsync(
    string errorMessage,
    string? details = null)
```

- **errorMessage**: Error description
- **details**: Additional error details

#### HideProgressOverlayAsync

```csharp
Task HideProgressOverlayAsync()
```

Manually hide the overlay (not usually needed due to auto-dismiss)

### Properties

#### CurrentProgressOverlay

```csharp
ProgressOverlayViewModel? CurrentProgressOverlay { get; }
```

Current overlay ViewModel (null when not visible)

#### IsOverlayVisible

```csharp
bool IsOverlayVisible { get; }
```

Boolean indicating overlay visibility state

## UI Design Specifications

### Visual Elements

- **Progress Bar Width**: Fixed 240px, center-aligned
- **Overlay Background**: Semi-transparent black (rgba(0,0,0,0.6))
- **Card Design**: MTM theme-compliant with rounded corners and shadows
- **Icons**: Material Design icons with rotation animation for processing
- **Typography**: MTM typography scale with proper font weights

### Theme Integration

The overlay automatically adapts to all MTM themes:

- Uses `DynamicResource` bindings for colors
- Supports light and dark theme variations
- Maintains consistent spacing and typography

### Accessibility

- Proper ARIA labels and roles
- Keyboard navigation support
- Screen reader compatibility
- Focus management during overlay display

## Threading Considerations

### UI Thread Safety

The service automatically handles UI thread dispatching:

```csharp
// Property changes are automatically dispatched to UI thread
if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
{
    OnPropertyChanged(nameof(IsOverlayVisible));
}
else
{
    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
    {
        OnPropertyChanged(nameof(IsOverlayVisible));
    });
}
```

### Auto-Dismiss Mechanism

Auto-dismiss is handled via background tasks:

```csharp
// In ProgressOverlayViewModel
_ = Task.Run(async () =>
{
    await Task.Delay(autoCloseDelayMs);
    if (IsCompleted && !_disposed)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            DismissRequested?.Invoke();
        });
    }
});
```

## Error Handling

### Exception Management

All service methods include comprehensive error handling:

```csharp
try
{
    // Operation logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error in progress overlay operation");
    await ErrorHandling.HandleErrorAsync(ex, "Progress overlay operation", SystemUserId);
    throw; // Re-throw for caller handling
}
```

### Error States

The overlay supports error display modes:

- Error icon (red exclamation)
- Error message display
- Error details expansion
- Manual dismissal required for errors

## Performance Considerations

### Memory Management

- Automatic disposal of completed overlays
- Proper event handler cleanup
- Minimal memory footprint during idle state

### Resource Usage

- Lightweight XAML structure
- Efficient binding patterns
- Minimal CPU usage during display

## Integration Examples

### View Switching Progress

```csharp
// In MainViewViewModel
private async Task SwitchToViewAsync(string viewName)
{
    await _progressOverlayService.ShowProgressOverlayAsync(
        $"Loading {viewName}",
        "Preparing view...",
        isDeterminate: false);

    try
    {
        await LoadViewDataAsync(viewName);
        await _progressOverlayService.CompleteProgressOverlayAsync(
            $"{viewName} loaded",
            autoCloseDelayMs: 800);
    }
    catch (Exception ex)
    {
        await _progressOverlayService.SetProgressOverlayErrorAsync(
            $"Failed to load {viewName}",
            ex.Message);
    }
}
```

### Database Operations

```csharp
// In InventoryService
public async Task<List<InventoryItem>> SearchInventoryAsync(string partId)
{
    await _progressOverlayService.ShowProgressOverlayAsync(
        "Searching Inventory",
        $"Looking for part: {partId}",
        isDeterminate: false);

    try
    {
        var results = await DatabaseSearch(partId);
        await _progressOverlayService.CompleteProgressOverlayAsync(
            $"Found {results.Count} items",
            autoCloseDelayMs: 1000);
        return results;
    }
    catch (Exception ex)
    {
        await _progressOverlayService.SetProgressOverlayErrorAsync(
            "Search failed",
            ex.Message);
        throw;
    }
}
```

## Testing Guidelines

### Unit Testing

```csharp
[Test]
public async Task ShowProgressOverlay_SetsCorrectProperties()
{
    // Arrange
    var mockLogger = Mock.Of<ILogger<ProgressOverlayService>>();
    var mockLoggerFactory = Mock.Of<ILoggerFactory>();
    var service = new ProgressOverlayService(mockLogger, mockLoggerFactory);

    // Act
    await service.ShowProgressOverlayAsync("Test Title", "Test Message");

    // Assert
    Assert.IsNotNull(service.CurrentProgressOverlay);
    Assert.IsTrue(service.IsOverlayVisible);
    Assert.AreEqual("Test Title", service.CurrentProgressOverlay.Title);
}
```

### Integration Testing

Test overlay behavior in actual views:

```csharp
[Test]
public async Task ViewSwitch_ShowsProgressOverlay()

{
    // Arrange
    var viewModel = CreateTestViewModel();

    // Act
    await viewModel.SwitchToInventoryCommand.ExecuteAsync(null);

    // Assert
    Assert.IsTrue(viewModel.ProgressOverlayService.IsOverlayVisible);

    // Wait for completion and verify dismissal
    await Task.Delay(2000);
    Assert.IsFalse(viewModel.ProgressOverlayService.IsOverlayVisible);
}
```

## Best Practices

### DO

- Always provide meaningful titles and status messages
- Use determinate progress when operation progress can be measured
- Set appropriate auto-dismiss delays (1-3 seconds)
- Handle exceptions and show error states
- Test overlay behavior on all target platforms

### DON'T

- Use overlays for instantaneous operations
- Set auto-dismiss delays longer than 5 seconds
- Forget to handle cancellation tokens
- Show overlays without proper error handling
- Use hardcoded colors (always use theme resources)

## Migration from Previous Progress System

### Old MainView Progress Bar

```xml
<!-- OLD: Remove this pattern -->

<ProgressBar x:Name="MainProgressBar" 
             IsVisible="{Binding IsLoading}"
             IsIndeterminate="True" />
```

### New Universal Overlay

```xml
<!-- NEW: Use this pattern -->
<Panel x:Name="ProgressOverlayPanel"
       ZIndex="9999"
       IsVisible="{Binding ProgressOverlayService.IsOverlayVisible}">
  <overlay:ProgressOverlayView
    DataContext="{Binding ProgressOverlayService.CurrentProgressOverlay}" />
</Panel>
```

## Troubleshooting

### Common Issues

1. **Overlay not appearing**: Check service registration and binding
2. **Not auto-dismissing**: Verify UI thread dispatching
3. **Performance issues**: Ensure proper disposal and cleanup
4. **Theme compatibility**: Use DynamicResource bindings only

### Debug Logging

Enable debug logging for troubleshooting:

```json
{
  "Logging": {
    "LogLevel": {
      "MTM_WIP_Application_Avalonia.Services.ProgressOverlayService": "Debug"
    }
  }
}
```

## Conclusion

The Universal Progress Overlay system provides a robust, theme-aware, and user-friendly progress indication mechanism for the MTM WIP Application. By following this implementation guide, developers can ensure consistent progress feedback across all application features while maintaining performance and accessibility standards.
