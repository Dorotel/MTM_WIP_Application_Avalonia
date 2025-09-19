# Stage 3: Critical Missing Overlays - Implementation Guide

**Priority**: 🟡 High  
**Timeline**: Weekend Day 2  
**Estimated Time**: 6-8 hours  

## 🎯 Overview

This stage implements the most critical missing overlays that significantly improve user experience and application safety.

## 📋 Task Breakdown

### **Task 3.1: Field Validation Overlay**

**Estimated Time**: 2-3 hours  
**Risk Level**: Medium  
**Dependencies**: Universal Overlay Service  

#### **Files to Create**

```
ViewModels/Overlay/ValidationOverlayViewModel.cs
Views/Overlay/ValidationOverlayView.axaml
Views/Overlay/ValidationOverlayView.axaml.cs
Services/ValidationOverlayService.cs
Models/Overlay/ValidationError.cs
```

#### **Implementation Details**

**ValidationOverlayViewModel**

```csharp
[ObservableObject]
public partial class ValidationOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private List<ValidationError> validationErrors = new();
    [ObservableProperty] private string primaryMessage = string.Empty;
    [ObservableProperty] private ValidationSeverity severity = ValidationSeverity.Error;
    [ObservableProperty] private Control? targetControl;
    [ObservableProperty] private bool showAllErrors = false;
    [ObservableProperty] private bool canProceedWithWarnings = false;
    
    [RelayCommand]
    private async Task FixFirstErrorAsync()
    {
        if (ValidationErrors.Count == 0) return;
        
        var firstError = ValidationErrors.First();
        if (firstError.TargetControl != null)
        {
            firstError.TargetControl.Focus();
            // Scroll into view if needed
        }
    }
    
    [RelayCommand]
    private async Task IgnoreWarningsAsync()
    {
        if (!CanProceedWithWarnings) return;
        
        // Filter out warnings, keep only errors
        ValidationErrors = ValidationErrors
            .Where(e => e.Severity == ValidationSeverity.Error)
            .ToList();
            
        if (ValidationErrors.Count == 0)
        {
            await DismissAsync();
        }
    }
}
```

**ValidationOverlayView.axaml**

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             x:Class="MTM_WIP_Application_Avalonia.Views.Overlay.ValidationOverlayView"
             x:DataType="vm:ValidationOverlayViewModel">

    <Border Background="#80000000" 
            IsVisible="{Binding IsVisible}"
            HorizontalAlignment="Center"
            VerticalAlignment="Center">
            
        <Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
                BorderThickness="2"
                CornerRadius="8"
                MaxWidth="500"
                Padding="20">
                
            <StackPanel Spacing="16">
                <!-- Header -->
                <StackPanel Orientation="Horizontal" Spacing="12">
                    <Path Fill="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
                          Data="M12,2L13.09,8.26L22,9L17,14L18.18,23L12,19.77L5.82,23L7,14L2,9L10.91,8.26L12,2Z"
                          Width="24" Height="24" />
                    <TextBlock Text="{Binding PrimaryMessage}"
                              FontSize="16"
                              FontWeight="SemiBold" />
                </StackPanel>
                
                <!-- Error List -->
                <ItemsControl ItemsSource="{Binding ValidationErrors}"
                             MaxHeight="200">
                    <ItemsControl.ItemTemplate>
                        <DataTemplate>
                            <Border Background="{DynamicResource MTM_Shared_Logic.ErrorLightBrush}"
                                   CornerRadius="4"
                                   Padding="12,8"
                                   Margin="0,0,0,4">
                                <StackPanel>
                                    <TextBlock Text="{Binding FieldName}"
                                              FontWeight="SemiBold"
                                              FontSize="12" />
                                    <TextBlock Text="{Binding ErrorMessage}"
                                              TextWrapping="Wrap"
                                              FontSize="11" />
                                </StackPanel>
                            </Border>
                        </DataTemplate>
                    </ItemsControl.ItemTemplate>
                </ItemsControl>
                
                <!-- Actions -->
                <StackPanel Orientation="Horizontal" 
                           HorizontalAlignment="Right" 
                           Spacing="8">
                    <Button Content="Fix First Error"
                           Command="{Binding FixFirstErrorCommand}"
                           IsVisible="{Binding ValidationErrors.Count}" />
                    <Button Content="Ignore Warnings"
                           Command="{Binding IgnoreWarningsCommand}"
                           IsVisible="{Binding CanProceedWithWarnings}" />
                    <Button Content="Close"
                           Command="{Binding DismissCommand}" />
                </StackPanel>
            </StackPanel>
        </Border>
    </Border>
</UserControl>
```

#### **Integration with Forms**

```csharp
// Usage pattern in ViewModels
public async Task ValidateAndProceedAsync()
{
    var validationErrors = ValidateCurrentForm();
    if (validationErrors.Any())
    {
        await _universalOverlayService.ShowValidationAsync(new ValidationOverlayRequest
        {
            Title = "Validation Required",
            Message = $"Please correct {validationErrors.Count} validation errors",
            ValidationErrors = validationErrors,
            CanProceedWithWarnings = validationErrors.All(e => e.Severity == ValidationSeverity.Warning)
        });
        return;
    }
    
    // Proceed with operation
    await ExecuteOperationAsync();
}
```

---

### **Task 3.2: Progress Overlay**

**Estimated Time**: 2-3 hours  
**Risk Level**: Medium  
**Dependencies**: Universal Overlay Service  

#### **Files to Create**

```
ViewModels/Overlay/ProgressOverlayViewModel.cs
Views/Overlay/ProgressOverlayView.axaml
Services/ProgressTracker.cs
Models/Overlay/ProgressUpdate.cs
```

#### **Implementation Details**

**ProgressOverlayViewModel**

```csharp
[ObservableObject]
public partial class ProgressOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private string operationTitle = string.Empty;
    [ObservableProperty] private string currentStep = string.Empty;
    [ObservableProperty] private double progressPercentage = 0.0;
    [ObservableProperty] private bool canCancel = true;
    [ObservableProperty] private TimeSpan estimatedTimeRemaining = TimeSpan.Zero;
    [ObservableProperty] private string progressDetails = string.Empty;
    [ObservableProperty] private bool showPercentage = true;
    [ObservableProperty] private bool showTimeRemaining = true;
    [ObservableProperty] private CancellationTokenSource? cancellationTokenSource;
    
    [RelayCommand]
    private async Task CancelOperationAsync()
    {
        if (!CanCancel || CancellationTokenSource == null) return;
        
        try
        {
            CancellationTokenSource.Cancel();
            CurrentStep = "Cancelling operation...";
            CanCancel = false;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error cancelling operation");
        }
    }
    
    public async Task UpdateProgressAsync(double percentage, string step, string details = "")
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            ProgressPercentage = percentage;
            CurrentStep = step;
            ProgressDetails = details;
            
            // Update ETA calculation
            if (ShowTimeRemaining && percentage > 0)
            {
                var elapsed = DateTime.Now - _startTime;
                var totalEstimated = elapsed.TotalSeconds / (percentage / 100.0);
                EstimatedTimeRemaining = TimeSpan.FromSeconds(totalEstimated - elapsed.TotalSeconds);
            }
        });
    }
}
```

**ProgressTracker Implementation**

```csharp
public interface IProgressTracker : IDisposable
{
    Task UpdateAsync(double percentage, string step, string details = "");
    Task CompleteAsync(string finalMessage = "Operation completed");
    Task FailAsync(string errorMessage, Exception? exception = null);
    CancellationToken CancellationToken { get; }
}

public class ProgressTracker : IProgressTracker
{
    private readonly ProgressOverlayViewModel _viewModel;
    private readonly IUniversalOverlayService _overlayService;
    private bool _disposed = false;
    
    public CancellationToken CancellationToken => 
        _viewModel.CancellationTokenSource?.Token ?? CancellationToken.None;
    
    public async Task UpdateAsync(double percentage, string step, string details = "")
    {
        if (_disposed) return;
        await _viewModel.UpdateProgressAsync(percentage, step, details);
    }
    
    public async Task CompleteAsync(string finalMessage = "Operation completed")
    {
        if (_disposed) return;
        await UpdateAsync(100, finalMessage);
        await Task.Delay(1000); // Show completion briefly
        await _overlayService.HideProgressAsync();
    }
    
    public async Task FailAsync(string errorMessage, Exception? exception = null)
    {
        if (_disposed) return;
        await _overlayService.ShowErrorAsync("Operation Failed", errorMessage, exception);
        await _overlayService.HideProgressAsync();
    }
}
```

---

### **Task 3.3: Connection Status Overlay**

**Estimated Time**: 1-2 hours  
**Risk Level**: Low  
**Dependencies**: Universal Overlay Service  

#### **Files to Create**

```
ViewModels/Overlay/ConnectionStatusOverlayViewModel.cs
Views/Overlay/ConnectionStatusOverlayView.axaml
Services/ConnectionStatusService.cs
```

#### **Implementation Details**

**ConnectionStatusOverlayViewModel**

```csharp
[ObservableObject]
public partial class ConnectionStatusOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private ConnectionStatus databaseStatus = ConnectionStatus.Unknown;
    [ObservableProperty] private string lastConnectionTest = string.Empty;
    [ObservableProperty] private List<ServiceStatus> serviceStatuses = new();
    [ObservableProperty] private bool autoReconnectEnabled = true;
    [ObservableProperty] private TimeSpan connectionTimeout = TimeSpan.FromSeconds(30);
    [ObservableProperty] private bool isReconnecting = false;
    
    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        try
        {
            IsReconnecting = true;
            
            // Test database connection
            using var cts = new CancellationTokenSource(ConnectionTimeout);
            var testResult = await TestDatabaseConnectionAsync(cts.Token);
            
            DatabaseStatus = testResult ? ConnectionStatus.Connected : ConnectionStatus.Disconnected;
            LastConnectionTest = DateTime.Now.ToString("HH:mm:ss");
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Connection test failed");
            DatabaseStatus = ConnectionStatus.Error;
        }
        finally
        {
            IsReconnecting = false;
        }
    }
    
    private async Task<bool> TestDatabaseConnectionAsync(CancellationToken cancellationToken)
    {
        // Use existing database service to test connection
        try
        {
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString: "your_connection_string", // Get from config
                procedureName: "SELECT 1", // Simple test query
                parameters: Array.Empty<MySqlParameter>()
            );
            
            return result.Status == 1;
        }
        catch
        {
            return false;
        }
    }
}
```

---

### **Task 3.4: Batch Confirmation Overlay**

**Estimated Time**: 2 hours  
**Risk Level**: Low  
**Dependencies**: ConfirmationOverlayViewModel pattern  

#### **Files to Create**

```
ViewModels/Overlay/BatchConfirmationOverlayViewModel.cs
Views/Overlay/BatchConfirmationOverlayView.axaml
Models/Overlay/BatchOperationItem.cs
```

#### **Implementation Details**

**BatchConfirmationOverlayViewModel**

```csharp
[ObservableObject]
public partial class BatchConfirmationOverlayViewModel : BaseViewModel
{
    [ObservableProperty] private string batchOperationType = string.Empty;
    [ObservableProperty] private int totalItemsAffected = 0;
    [ObservableProperty] private List<BatchOperationItem> affectedItems = new();
    [ObservableProperty] private bool showPreview = true;
    [ObservableProperty] private List<string> warnings = new();
    [ObservableProperty] private bool canProceed = true;
    [ObservableProperty] private bool showItemDetails = false;
    
    [RelayCommand]
    private async Task ProceedWithBatchAsync()
    {
        if (!CanProceed) return;
        
        Result = OverlayResult.Confirmed;
        await DismissAsync();
    }
    
    [RelayCommand]
    private async Task ModifySelectionAsync()
    {
        Result = OverlayResult.ModifyRequested;
        await DismissAsync();
    }
    
    [RelayCommand]
    private async Task PreviewChangesAsync()
    {
        ShowItemDetails = !ShowItemDetails;
    }
    
    public void SetBatchOperation(string operationType, List<BatchOperationItem> items, List<string>? warnings = null)
    {
        BatchOperationType = operationType;
        AffectedItems = items ?? new List<BatchOperationItem>();
        TotalItemsAffected = AffectedItems.Count;
        Warnings = warnings ?? new List<string>();
        CanProceed = !Warnings.Any(w => w.Contains("CRITICAL", StringComparison.OrdinalIgnoreCase));
    }
}
```

## 🧪 Testing Strategy

### **Individual Overlay Testing**

1. **Validation Overlay**: Test with various validation scenarios
2. **Progress Overlay**: Test cancellation and completion flows
3. **Connection Status**: Test with network disconnection scenarios
4. **Batch Confirmation**: Test with different batch sizes and warning types

### **Integration Testing**

1. **Form Integration**: Validate overlays work with real form validation
2. **Long Operations**: Progress overlays with actual database operations
3. **Error Scenarios**: Connection and operation failure handling

## ⚠️ Risk Mitigation

### **Potential Issues**

1. **Performance Impact**: Progress updates may affect UI responsiveness
2. **Memory Usage**: Large batch operations may consume significant memory
3. **Thread Safety**: UI updates from background threads need proper marshaling

### **Mitigation Strategies**

1. **Throttle Updates**: Limit progress update frequency
2. **Pagination**: Show large batch items in pages
3. **Dispatcher Usage**: Ensure all UI updates use proper dispatcher

## ✅ Stage Completion Criteria

- [ ] Field validation overlay provides real-time feedback
- [ ] Progress overlay handles cancellation and completion properly
- [ ] Connection status overlay accurately reflects database state
- [ ] Batch confirmation overlay prevents unintended bulk operations
- [ ] All overlays integrate with Universal Overlay Service
- [ ] Performance impact is within acceptable bounds
- [ ] Memory usage is optimized for large operations

**Stage 3 Success**: Critical missing overlays significantly improve user safety and experience across all major application workflows.
