# CustomDataGrid - Command Implementation Guide

**Version**: 1.0  
**Created**: September 14, 2025  

---

## ⚙️ Command Implementation Guide

The CustomDataGrid uses command binding pattern with RelativeSource bindings to route actions to parent ViewModels while maintaining clean separation of concerns.

## Control Command Properties

### Command Property Definitions
```csharp
// CustomDataGrid.axaml.cs - Command Properties
public partial class CustomDataGrid : UserControl
{
    // Action Commands
    public static readonly StyledProperty<ICommand?> ReadNoteCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(ReadNoteCommand));

    public static readonly StyledProperty<ICommand?> DeleteItemCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(DeleteItemCommand));

    public static readonly StyledProperty<ICommand?> EditItemCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(EditItemCommand));

    public static readonly StyledProperty<ICommand?> DuplicateItemCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(DuplicateItemCommand));

    public static readonly StyledProperty<ICommand?> ViewDetailsCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(ViewDetailsCommand));

    // Command Properties
    public ICommand? ReadNoteCommand
    {
        get => GetValue(ReadNoteCommandProperty);
        set => SetValue(ReadNoteCommandProperty, value);
    }

    public ICommand? DeleteItemCommand
    {
        get => GetValue(DeleteItemCommandProperty);
        set => SetValue(DeleteItemCommandProperty, value);
    }

    public ICommand? EditItemCommand
    {
        get => GetValue(EditItemCommandProperty);
        set => SetValue(EditItemCommandProperty, value);
    }

    public ICommand? DuplicateItemCommand
    {
        get => GetValue(DuplicateItemCommandProperty);
        set => SetValue(DuplicateItemCommandProperty, value);
    }

    public ICommand? ViewDetailsCommand
    {
        get => GetValue(ViewDetailsCommandProperty);
        set => SetValue(ViewDetailsCommandProperty, value);
    }
}
```

## XAML Command Binding

### Action Button Command Bindings
```xml
<!-- Read Note Button -->
<Button Content="📝"
        Classes="primary-action"
        Width="24"
        Height="24"
        Command="{Binding ReadNoteCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
        CommandParameter="{Binding}"
        ToolTip.Tip="Read Note"
        IsVisible="{Binding HasNotes}" />

<!-- Edit Button -->
<Button Content="✏️"
        Classes="primary-action"
        Width="24"
        Height="24"
        Command="{Binding EditItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
        CommandParameter="{Binding}"
        ToolTip.Tip="Edit Item" />

<!-- Duplicate Button -->
<Button Content="📋"
        Classes="primary-action"
        Width="24"
        Height="24"
        Command="{Binding DuplicateItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
        CommandParameter="{Binding}"
        ToolTip.Tip="Duplicate Item" />

<!-- View Details Button -->
<Button Content="👁️"
        Classes="primary-action"
        Width="24"
        Height="24"
        Command="{Binding ViewDetailsCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
        CommandParameter="{Binding}"
        ToolTip.Tip="View Details" />

<!-- Delete Button -->
<Button Content="🗑️"
        Classes="warning-action"
        Width="24"
        Height="24"
        Command="{Binding DeleteItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
        CommandParameter="{Binding}"
        ToolTip.Tip="Delete Item" />
```

## Parent ViewModel Implementation

### Required Command Implementation Pattern
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly IConfirmationService _confirmationService;
    private readonly ISuccessOverlayService _successOverlayService;

    public InventoryViewModel(
        ILogger<InventoryViewModel> logger,
        IInventoryService inventoryService,
        IConfirmationService confirmationService,
        ISuccessOverlayService successOverlayService)
        : base(logger)
    {
        _inventoryService = inventoryService;
        _confirmationService = confirmationService;
        _successOverlayService = successOverlayService;
    }

    [RelayCommand]
    private async Task ReadNoteAsync(object? parameter)
    {
        if (parameter is not InventoryItem item)
        {
            Logger.LogWarning("ReadNoteAsync called with invalid parameter type");
            return;
        }

        if (!item.HasNotes)
        {
            Logger.LogWarning("ReadNoteAsync called on item without notes: {PartId}", item.PartId);
            return;
        }

        try
        {
            IsLoading = true;
            
            var result = await _inventoryService.GetItemNotesAsync(item.PartId, item.Operation);
            
            if (result.IsSuccess)
            {
                // Show notes in overlay or dialog
                await ShowNotesDialog(result.Data);
            }
            else
            {
                Logger.LogError("Failed to load notes for {PartId}: {Error}", item.PartId, result.ErrorMessage);
                await ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage),
                    $"Loading notes for {item.PartId}");
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Reading notes for item {item.PartId}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteItemAsync(object? parameter)
    {
        if (parameter is not InventoryItem item)
        {
            Logger.LogWarning("DeleteItemAsync called with invalid parameter type");
            return;
        }

        try
        {
            // Show confirmation dialog
            var confirmationResult = await _confirmationService.ShowConfirmationAsync(new ConfirmationRequest
            {
                Title = "Delete Inventory Item",
                Message = $"Are you sure you want to delete this inventory item?\\n\\nPart ID: {item.PartId}\\nOperation: {item.Operation}\\nLocation: {item.Location}\\nQuantity: {item.Quantity:N0}",
                ConfirmButtonText = "Delete",
                CancelButtonText = "Cancel",
                IsDestructive = true,
                Icon = MaterialIconKind.Delete
            });

            if (!confirmationResult.IsConfirmed)
            {
                Logger.LogInformation("Delete operation cancelled by user for {PartId}", item.PartId);
                return;
            }

            IsLoading = true;

            var result = await _inventoryService.DeleteInventoryAsync(item.PartId, item.Operation, item.Location);

            if (result.IsSuccess)
            {
                // Remove from collection
                if (InventoryItems.Contains(item))
                {
                    InventoryItems.Remove(item);
                }

                // Show success overlay
                await _successOverlayService.ShowSuccessAsync(new SuccessRequest
                {
                    Title = "Item Deleted",
                    Message = $"Successfully deleted {item.PartId}",
                    Duration = TimeSpan.FromSeconds(3)
                });

                Logger.LogInformation("Successfully deleted inventory item: {PartId}", item.PartId);
            }
            else
            {
                Logger.LogError("Failed to delete item {PartId}: {Error}", item.PartId, result.ErrorMessage);
                await ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage),
                    $"Deleting item {item.PartId}");
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Deleting item {item.PartId}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task EditItemAsync(object? parameter)
    {
        if (parameter is not InventoryItem item)
        {
            Logger.LogWarning("EditItemAsync called with invalid parameter type");
            return;
        }

        try
        {
            Logger.LogInformation("Opening edit dialog for inventory item: {PartId}", item.PartId);
            
            // Navigate to edit view or show edit dialog
            // Implementation depends on application navigation pattern
            await NavigateToEditView(item);
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Opening edit dialog for {item.PartId}");
        }
    }

    [RelayCommand]
    private async Task DuplicateItemAsync(object? parameter)
    {
        if (parameter is not InventoryItem item)
        {
            Logger.LogWarning("DuplicateItemAsync called with invalid parameter type");
            return;
        }

        try
        {
            Logger.LogInformation("Duplicating inventory item: {PartId}", item.PartId);

            // Create duplicate item
            var duplicateItem = new InventoryItem
            {
                PartId = item.PartId,
                Operation = item.Operation,
                Location = item.Location,
                Quantity = item.Quantity,
                // Don't copy notes or timestamps - these should be fresh
                HasNotes = false,
                LastUpdated = DateTime.Now,
                IsSelected = false
            };

            IsLoading = true;

            var result = await _inventoryService.AddInventoryAsync(duplicateItem);

            if (result.IsSuccess)
            {
                // Add to collection
                duplicateItem.LastUpdated = DateTime.Now; // Update with actual database timestamp
                InventoryItems.Insert(0, duplicateItem); // Add at top

                // Show success overlay
                await _successOverlayService.ShowSuccessAsync(new SuccessRequest
                {
                    Title = "Item Duplicated",
                    Message = $"Successfully duplicated {item.PartId}",
                    Duration = TimeSpan.FromSeconds(3)
                });

                Logger.LogInformation("Successfully duplicated inventory item: {PartId}", item.PartId);
            }
            else
            {
                Logger.LogError("Failed to duplicate item {PartId}: {Error}", item.PartId, result.ErrorMessage);
                await ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage),
                    $"Duplicating item {item.PartId}");
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Duplicating item {item.PartId}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewDetailsAsync(object? parameter)
    {
        if (parameter is not InventoryItem item)
        {
            Logger.LogWarning("ViewDetailsAsync called with invalid parameter type");
            return;
        }

        try
        {
            Logger.LogInformation("Viewing details for inventory item: {PartId}", item.PartId);

            IsLoading = true;

            // Load detailed information
            var result = await _inventoryService.GetItemDetailsAsync(item.PartId, item.Operation);

            if (result.IsSuccess)
            {
                // Show details dialog or navigate to details view
                await ShowDetailsDialog(result.Data);
            }
            else
            {
                Logger.LogError("Failed to load details for {PartId}: {Error}", item.PartId, result.ErrorMessage);
                await ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage),
                    $"Loading details for {item.PartId}");
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, $"Viewing details for {item.PartId}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Helper methods
    private async Task ShowNotesDialog(string notes)
    {
        // Implementation depends on notes display pattern
        // Could use overlay, dialog, or dedicated view
    }

    private async Task NavigateToEditView(InventoryItem item)
    {
        // Implementation depends on navigation pattern
        // Could use navigation service or direct view instantiation
    }

    private async Task ShowDetailsDialog(InventoryItemDetails details)
    {
        // Implementation depends on details display pattern
        // Could use overlay, dialog, or dedicated view
    }
}
```

## Command Parameter Validation

### Parameter Type Checking
```csharp
// Utility method for command parameter validation
private static bool ValidateCommandParameter<T>(object? parameter, out T? validParameter) where T : class
{
    validParameter = parameter as T;
    
    if (validParameter == null)
    {
        Logger.LogWarning("Command called with invalid parameter type. Expected: {ExpectedType}, Received: {ActualType}",
            typeof(T).Name,
            parameter?.GetType().Name ?? "null");
        return false;
    }
    
    return true;
}

// Usage in command methods
[RelayCommand]
private async Task SomeActionAsync(object? parameter)
{
    if (!ValidateCommandParameter<InventoryItem>(parameter, out var item))
        return;
    
    // Continue with validated item
}
```

## Command Availability

### CanExecute Implementation
```csharp
[RelayCommand(CanExecute = nameof(CanReadNote))]
private async Task ReadNoteAsync(object? parameter) { /* ... */ }

private bool CanReadNote(object? parameter)
{
    return parameter is InventoryItem { HasNotes: true } && !IsLoading;
}

[RelayCommand(CanExecute = nameof(CanDeleteItem))]
private async Task DeleteItemAsync(object? parameter) { /* ... */ }

private bool CanDeleteItem(object? parameter)
{
    return parameter is InventoryItem && !IsLoading && HasDeletePermission;
}

[RelayCommand(CanExecute = nameof(CanEditItem))]
private async Task EditItemAsync(object? parameter) { /* ... */ }

private bool CanEditItem(object? parameter)
{
    return parameter is InventoryItem && !IsLoading && HasEditPermission;
}
```

## Error Handling in Commands

### Standardized Error Handling Pattern
```csharp
private async Task ExecuteCommandSafelyAsync(Func<Task> operation, string operationName, string itemIdentifier = "")
{
    try
    {
        IsLoading = true;
        Logger.LogInformation("Starting {Operation} for {Item}", operationName, itemIdentifier);
        
        await operation();
        
        Logger.LogInformation("Completed {Operation} for {Item}", operationName, itemIdentifier);
    }
    catch (Exception ex)
    {
        Logger.LogError(ex, "Error in {Operation} for {Item}", operationName, itemIdentifier);
        await ErrorHandling.HandleErrorAsync(ex, $"{operationName} operation");
    }
    finally
    {
        IsLoading = false;
    }
}

// Usage
[RelayCommand]
private async Task SomeActionAsync(object? parameter)
{
    if (!ValidateCommandParameter<InventoryItem>(parameter, out var item))
        return;

    await ExecuteCommandSafelyAsync(
        async () =>
        {
            // Actual operation logic here
            var result = await _service.DoSomethingAsync(item);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }
        },
        "SomeAction",
        item.PartId);
}
```

## Integration with Overlay Services

### Confirmation Service Integration
```csharp
// Confirmation service setup
public interface IConfirmationService
{
    Task<ConfirmationResult> ShowConfirmationAsync(ConfirmationRequest request);
}

public class ConfirmationRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ConfirmButtonText { get; set; } = "OK";
    public string CancelButtonText { get; set; } = "Cancel";
    public bool IsDestructive { get; set; }
    public MaterialIconKind Icon { get; set; } = MaterialIconKind.Help;
}

public class ConfirmationResult
{
    public bool IsConfirmed { get; set; }
    public string UserResponse { get; set; } = string.Empty;
}
```

### Success Overlay Integration
```csharp
// Success overlay service setup
public interface ISuccessOverlayService
{
    Task ShowSuccessAsync(SuccessRequest request);
}

public class SuccessRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);
    public MaterialIconKind Icon { get; set; } = MaterialIconKind.CheckCircle;
}
```

---

**Next Implementation Phase**: [07-Supporting-Classes.md](./07-Supporting-Classes.md)