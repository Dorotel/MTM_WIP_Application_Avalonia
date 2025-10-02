# CustomDataGrid - Overlay Integration Guide

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 🎨 Overlay Integration Guide

The CustomDataGrid integrates seamlessly with the MTM overlay system (ConfirmationOverlayView and SuccessOverlayView) to provide consistent user feedback for data operations.

## ConfirmationOverlayView Integration

### Enhanced Confirmation Scenarios for CustomDataGrid

The existing `ConfirmationOverlayView.axaml` supports the CustomDataGrid scenarios with these enhancements:

### Delete Item Confirmation

```csharp
// Parent ViewModel - Delete item confirmation
private async Task ShowDeleteConfirmationAsync(InventoryItem item)
{
    var confirmationRequest = new ConfirmationRequest
    {
        Title = "Delete Inventory Item",
        Message = $"Are you sure you want to delete this inventory item?\\n\\n" +
                 $"Part ID: {item.PartId}\\n" +
                 $"Operation: {item.Operation}\\n" +
                 $"Location: {item.Location}\\n" +
                 $"Quantity: {item.Quantity:N0}\\n\\n" +
                 $"This action cannot be undone.",
        ConfirmButtonText = "Delete Item",
        CancelButtonText = "Cancel",
        OverlayType = ConfirmationOverlayType.Warning,
        Icon = MaterialIconKind.Delete,
        IsDestructive = true,
        ShowInTaskbar = false,
        IsModal = true
    };

    var result = await _confirmationService.ShowConfirmationAsync(confirmationRequest);
    
    if (result.IsConfirmed)
    {
        await ExecuteDeleteAsync(item);
    }
}
```

### Bulk Delete Confirmation

```csharp
// Parent ViewModel - Bulk delete confirmation
private async Task ShowBulkDeleteConfirmationAsync(List<InventoryItem> selectedItems)
{
    var itemCount = selectedItems.Count;
    var itemList = selectedItems.Take(5).Select(item => $"• {item.PartId} ({item.Operation})").ToList();
    
    if (selectedItems.Count > 5)
    {
        itemList.Add($"• ... and {selectedItems.Count - 5} more items");
    }

    var confirmationRequest = new ConfirmationRequest
    {
        Title = "Delete Multiple Items",
        Message = $"Are you sure you want to delete {itemCount} inventory items?\\n\\n" +
                 string.Join("\\n", itemList) + "\\n\\n" +
                 $"This action cannot be undone.",
        ConfirmButtonText = $"Delete {itemCount} Items",
        CancelButtonText = "Cancel",
        OverlayType = ConfirmationOverlayType.Error,
        Icon = MaterialIconKind.DeleteMultiple,
        IsDestructive = true,
        ShowProgress = true
    };

    var result = await _confirmationService.ShowConfirmationAsync(confirmationRequest);
    
    if (result.IsConfirmed)
    {
        await ExecuteBulkDeleteAsync(selectedItems);
    }
}
```

### Column Management Confirmation (Phase 3)

```csharp
// Column management reset confirmation
private async Task ShowColumnResetConfirmationAsync()
{
    var confirmationRequest = new ConfirmationRequest
    {
        Title = "Reset Column Configuration",
        Message = "Are you sure you want to reset all column settings to their default values?\\n\\n" +
                 "This will reset:\\n" +
                 "• Column widths\\n" +
                 "• Column visibility\\n" +
                 "• Column order\\n\\n" +
                 "Your current configuration will be lost.",
        ConfirmButtonText = "Reset to Defaults",
        CancelButtonText = "Keep Current Settings",
        OverlayType = ConfirmationOverlayType.Warning,
        Icon = MaterialIconKind.Restore
    };

    var result = await _confirmationService.ShowConfirmationAsync(confirmationRequest);
    
    if (result.IsConfirmed)
    {
        ResetColumnConfiguration();
        await ShowColumnResetSuccessAsync();
    }
}
```

## SuccessOverlayView Integration

### Enhanced Success Scenarios for CustomDataGrid

The existing `SuccessOverlayView.axaml` supports CustomDataGrid operations with these patterns:

### Single Item Success

```csharp
// Success notification for single item operations
private async Task ShowItemOperationSuccessAsync(string operation, InventoryItem item)
{
    var successRequest = new SuccessRequest
    {
        Title = $"Item {operation}",
        Message = $"Successfully {operation.ToLower()} {item.PartId}",
        Details = $"Operation: {item.Operation} | Location: {item.Location}",
        ShowDetails = true,
        Duration = TimeSpan.FromSeconds(3),
        Icon = operation switch
        {
            "Added" => MaterialIconKind.PlusCircle,
            "Updated" => MaterialIconKind.CheckCircle,
            "Deleted" => MaterialIconKind.DeleteCircle,
            "Duplicated" => MaterialIconKind.ContentCopy,
            _ => MaterialIconKind.CheckCircle
        },
        AutoDismiss = true,
        AllowManualDismiss = true
    };

    await _successOverlayService.ShowSuccessAsync(successRequest);
}
```

### Bulk Operation Success

```csharp
// Success notification for bulk operations
private async Task ShowBulkOperationSuccessAsync(string operation, int itemCount, TimeSpan duration)
{
    var successRequest = new SuccessRequest
    {
        Title = $"Bulk {operation} Complete",
        Message = $"Successfully {operation.ToLower()} {itemCount} items",
        Details = $"Operation completed in {duration.TotalSeconds:F1} seconds",
        ShowDetails = true,
        Duration = TimeSpan.FromSeconds(4),
        Icon = MaterialIconKind.CheckboxMultipleMarked,
        ShowProgress = false,
        AutoDismiss = true,
        AllowManualDismiss = true
    };

    await _successOverlayService.ShowSuccessAsync(successRequest);
}
```

### Data Load Success

```csharp
// Success notification for data loading operations
private async Task ShowDataLoadSuccessAsync(int itemCount, TimeSpan loadTime)
{
    var successRequest = new SuccessRequest
    {
        Title = "Data Loaded",
        Message = $"Loaded {itemCount:N0} inventory items",
        Details = $"Load time: {loadTime.TotalMilliseconds:F0}ms",
        ShowDetails = itemCount > 1000, // Show details only for large datasets
        Duration = TimeSpan.FromSeconds(2),
        Icon = MaterialIconKind.DatabaseCheck,
        AutoDismiss = true,
        AllowManualDismiss = false // Auto-dismiss for routine operations
    };

    await _successOverlayService.ShowSuccessAsync(successRequest);
}
```

## Service Integration Pattern

### Confirmation Service Interface Enhancement

```csharp
// Enhanced confirmation service for CustomDataGrid scenarios
public interface IConfirmationService
{
    Task<ConfirmationResult> ShowConfirmationAsync(ConfirmationRequest request);
    Task<ConfirmationResult> ShowDeleteConfirmationAsync(object item, string itemDescription);
    Task<ConfirmationResult> ShowBulkDeleteConfirmationAsync(IList<object> items, string itemType);
    Task<ConfirmationResult> ShowDataLossWarningAsync(string operationName, string dataDescription);
}

public class ConfirmationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool ShowDetails { get; set; }
    public string ConfirmButtonText { get; set; } = "OK";
    public string CancelButtonText { get; set; } = "Cancel";
    public ConfirmationOverlayType OverlayType { get; set; } = ConfirmationOverlayType.Confirmation;
    public MaterialIconKind Icon { get; set; } = MaterialIconKind.Help;
    public bool IsDestructive { get; set; }
    public bool IsModal { get; set; } = true;
    public bool ShowInTaskbar { get; set; } = false;
    public bool ShowProgress { get; set; } = false;
    public bool AllowBackgroundClose { get; set; } = true;
}

public class ConfirmationResult
{
    public bool IsConfirmed { get; set; }
    public string UserInput { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public enum ConfirmationOverlayType
{
    Confirmation,
    Warning,
    Error,
    Information
}
```

### Success Service Interface Enhancement

```csharp
// Enhanced success service for CustomDataGrid scenarios
public interface ISuccessOverlayService
{
    Task ShowSuccessAsync(SuccessRequest request);
    Task ShowOperationSuccessAsync(string operation, object item, string itemDescription);
    Task ShowBulkOperationSuccessAsync(string operation, int itemCount, TimeSpan duration);
    Task ShowQuickSuccessAsync(string message);
}

public class SuccessRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public bool ShowDetails { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);
    public MaterialIconKind Icon { get; set; } = MaterialIconKind.CheckCircle;
    public bool AutoDismiss { get; set; } = true;
    public bool AllowManualDismiss { get; set; } = true;
    public bool ShowProgress { get; set; } = false;
    public SuccessAnimationType AnimationType { get; set; } = SuccessAnimationType.FadeIn;
}

public enum SuccessAnimationType
{
    FadeIn,
    SlideDown,
    ScaleIn,
    None
}
```

## Command Integration Examples

### Complete Delete Command with Overlay Integration

```csharp
[RelayCommand]
private async Task DeleteItemAsync(object? parameter)
{
    if (!ValidateCommandParameter<InventoryItem>(parameter, out var item))
        return;

    try
    {
        // Show confirmation overlay
        var confirmationResult = await _confirmationService.ShowDeleteConfirmationAsync(
            item,
            $"{item.PartId} (Operation: {item.Operation}, Location: {item.Location})");

        if (!confirmationResult.IsConfirmed)
        {
            Logger.LogInformation("Delete operation cancelled by user for {PartId}", item.PartId);
            return;
        }

        IsLoading = true;
        var stopwatch = Stopwatch.StartNew();

        var result = await _inventoryService.DeleteInventoryAsync(item.PartId, item.Operation, item.Location);

        if (result.IsSuccess)
        {
            // Remove from UI collection
            if (InventoryItems.Contains(item))
            {
                InventoryItems.Remove(item);
            }

            stopwatch.Stop();

            // Show success overlay
            await _successOverlayService.ShowOperationSuccessAsync("Deleted", item, item.PartId);

            Logger.LogInformation("Successfully deleted inventory item: {PartId} in {Duration}ms", 
                item.PartId, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            Logger.LogError("Failed to delete item {PartId}: {Error}", item.PartId, result.ErrorMessage);
            
            // Show error confirmation (non-destructive)
            await _confirmationService.ShowConfirmationAsync(new ConfirmationRequest
            {
                Title = "Delete Failed",
                Message = $"Failed to delete {item.PartId}:\\n\\n{result.ErrorMessage}",
                ConfirmButtonText = "OK",
                CancelButtonText = "",
                OverlayType = ConfirmationOverlayType.Error,
                Icon = MaterialIconKind.AlertCircle,
                IsDestructive = false
            });
        }
    }
    catch (Exception ex)
    {
        await ErrorHandling.HandleErrorAsync(ex, $"Deleting item {item?.PartId}");
    }
    finally
    {
        IsLoading = false;
    }
}
```

### Bulk Operations with Progress Overlay

```csharp
[RelayCommand]
private async Task DeleteSelectedItemsAsync()
{
    var selectedItems = InventoryItems.Where(item => item.IsSelected).ToList();
    
    if (selectedItems.Count == 0)
    {
        Logger.LogWarning("DeleteSelectedItems called with no selected items");
        return;
    }

    try
    {
        // Show bulk confirmation
        var confirmationResult = await _confirmationService.ShowBulkDeleteConfirmationAsync(
            selectedItems.Cast<object>().ToList(),
            "inventory items");

        if (!confirmationResult.IsConfirmed)
        {
            Logger.LogInformation("Bulk delete cancelled by user for {Count} items", selectedItems.Count);
            return;
        }

        IsLoading = true;
        var stopwatch = Stopwatch.StartNew();
        var successCount = 0;
        var failureCount = 0;

        foreach (var item in selectedItems)
        {
            var result = await _inventoryService.DeleteInventoryAsync(item.PartId, item.Operation, item.Location);
            
            if (result.IsSuccess)
            {
                InventoryItems.Remove(item);
                successCount++;
            }
            else
            {
                Logger.LogError("Failed to delete item {PartId}: {Error}", item.PartId, result.ErrorMessage);
                failureCount++;
            }
        }

        stopwatch.Stop();

        // Show appropriate success/failure message
        if (failureCount == 0)
        {
            await _successOverlayService.ShowBulkOperationSuccessAsync("Deleted", successCount, stopwatch.Elapsed);
        }
        else
        {
            await _confirmationService.ShowConfirmationAsync(new ConfirmationRequest
            {
                Title = "Bulk Delete Results",
                Message = $"Bulk delete completed:\\n\\n" +
                         $"Successfully deleted: {successCount} items\\n" +
                         $"Failed to delete: {failureCount} items\\n\\n" +
                         $"Check the error log for details on failed items.",
                ConfirmButtonText = "OK",
                OverlayType = failureCount > successCount ? ConfirmationOverlayType.Error : ConfirmationOverlayType.Warning,
                Icon = MaterialIconKind.InformationOutline
            });
        }

        Logger.LogInformation("Bulk delete completed: {Success} success, {Failures} failures in {Duration}ms",
            successCount, failureCount, stopwatch.ElapsedMilliseconds);
    }
    catch (Exception ex)
    {
        await ErrorHandling.HandleErrorAsync(ex, "Bulk delete operation");
    }
    finally
    {
        IsLoading = false;
    }
}
```

## Overlay Positioning and Parent Integration

### Parent View Integration Pattern

```xml
<!-- Parent View with CustomDataGrid and Overlays -->
<Grid>
  <!-- Main content -->
  <customControls:CustomDataGrid 
      x:Name="DataGrid"
      ItemsSource="{Binding InventoryItems}"
      ReadNoteCommand="{Binding ReadNoteCommand}"
      DeleteItemCommand="{Binding DeleteItemCommand}"
      EditItemCommand="{Binding EditItemCommand}"
      DuplicateItemCommand="{Binding DuplicateItemCommand}"
      ViewDetailsCommand="{Binding ViewDetailsCommand}" />
  
  <!-- Confirmation Overlay -->
  <overlay:ConfirmationOverlayView
      x:Name="ConfirmationOverlay"
      IsVisible="{Binding ShowConfirmationOverlay}"
      DataContext="{Binding ConfirmationViewModel}" />
  
  <!-- Success Overlay -->
  <overlay:SuccessOverlayView
      x:Name="SuccessOverlay"
      IsVisible="{Binding ShowSuccessOverlay}"
      DataContext="{Binding SuccessViewModel}" />
</Grid>
```

### Parent ViewModel Overlay Management

```csharp
// Parent ViewModel overlay state management
[ObservableObject]
public partial class InventoryManagementViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool _showConfirmationOverlay;

    [ObservableProperty]
    private bool _showSuccessOverlay;

    [ObservableProperty]
    private ConfirmationOverlayViewModel _confirmationViewModel;

    [ObservableProperty]
    private SuccessOverlayViewModel _successViewModel;

    public InventoryManagementViewModel(
        ILogger<InventoryManagementViewModel> logger,
        IConfirmationService confirmationService,
        ISuccessOverlayService successOverlayService)
        : base(logger)
    {
        _confirmationService = confirmationService;
        _successOverlayService = successOverlayService;
        
        ConfirmationViewModel = new ConfirmationOverlayViewModel(logger);
        SuccessViewModel = new SuccessOverlayViewModel(logger);
        
        // Subscribe to overlay events
        ConfirmationViewModel.OverlayDismissed += OnConfirmationOverlayDismissed;
        SuccessViewModel.OverlayDismissed += OnSuccessOverlayDismissed;
    }

    private void OnConfirmationOverlayDismissed(object sender, EventArgs e)
    {
        ShowConfirmationOverlay = false;
    }

    private void OnSuccessOverlayDismissed(object sender, EventArgs e)
    {
        ShowSuccessOverlay = false;
    }
}
```

---

**Next Implementation Phase**: [09-HTML-ThemeV2-Implementation-Guide.md](./09-HTML-ThemeV2-Implementation-Guide.md)
