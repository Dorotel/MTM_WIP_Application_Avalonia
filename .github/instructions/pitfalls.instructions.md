# MTM WIP Application Development Pitfalls Guide

**Critical pitfalls to avoid when developing the MTM WIP Application Avalonia**

## 🚨 Event System Integration Pitfalls

### **Issue: Missing Event Synchronization Between UI Controls and ViewModels**
**Symptoms**: UI controls (like CustomDataGrid ListBox) don't update ViewModel properties when selection changes, causing buttons to remain disabled.

**Root Cause**: Avalonia UserControls don't automatically synchronize internal control selections with bound ViewModel properties.

**Solution Pattern**:
```csharp
// ✅ CORRECT: Always add SelectionChanged handlers for internal controls
private void InitializeComponent()
{
    AvaloniaXamlLoader.Load(this);
    _dataListBox = this.FindControl<ListBox>("DataListBox");
    
    // CRITICAL: Subscribe to selection changes
    _dataListBox.SelectionChanged += OnListBoxSelectionChanged;
}

private void OnListBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
{
    // CRITICAL: Synchronize with ViewModel property
    SynchronizeSelectedItemWithListBox();
}

// CRITICAL: Always clean up event subscriptions
protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    if (_dataListBox != null)
    {
        _dataListBox.SelectionChanged -= OnListBoxSelectionChanged;
    }
    base.OnDetachedFromVisualTree(e);
}
```

**Prevention**: Always verify that UserControl internal selections propagate to ViewModel properties for button CanExecute logic.

---

## 🔄 Reset Command Implementation Pitfalls

### **Issue: Reset Commands Don't Clear UI Collections**
**Symptoms**: Reset button appears to work but UI still shows previous data.

**Root Cause**: Reset commands only clear individual properties but not ObservableCollections that populate UI controls.

**Solution Pattern**:
```csharp
// ❌ WRONG: Only clearing individual properties
[RelayCommand]
private async Task Reset()
{
    PartId = string.Empty;
    Operation = string.Empty;
    // Missing collection clearing
}

// ✅ CORRECT: Clear ALL collections and properties
[RelayCommand]
private async Task Reset()
{
    try
    {
        // CRITICAL: Use UI thread for collection operations
        await Application.Current!.Dispatcher.InvokeAsync(() =>
        {
            // Clear all collections first
            InventoryItems.Clear();
            SelectedItems.Clear();
            
            // Then clear individual properties
            SelectedItem = null;
            PartId = string.Empty;
            Operation = string.Empty;
            
            // CRITICAL: Notify property changes explicitly
            OnPropertyChanged(nameof(SelectedItem));
        });
    }
    catch (Exception ex)
    {
        await ErrorHandling.HandleErrorAsync(ex, "Reset operation failed");
    }
}
```

**Prevention**: Always identify ALL collections and properties that need clearing in Reset operations.

---

## 🏭 Business Logic Transaction Type Pitfalls

### **Issue: Hardcoded Transaction Types Don't Match Business Intent**
**Symptoms**: Transaction history shows wrong transaction types (e.g., "IN" for removal operations).

**Root Cause**: Services hardcode transaction types instead of accepting them as parameters based on business context.

**Solution Pattern**:
```csharp
// ❌ WRONG: Hardcoded transaction type
public async Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity)
{
    var transactionType = "IN"; // WRONG - hardcoded
    // ... rest of method
}

// ✅ CORRECT: Accept transaction type as parameter
public async Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity, string transactionType)
{
    // Use provided transaction type based on business context
    var parameters = new Dictionary<string, object>
    {
        ["p_TransactionType"] = transactionType, // Use provided value
        // ... other parameters
    };
}

// ✅ CORRECT: Service calls with proper business context
await _quickButtonsService.AddTransactionToLast10Async(currentUser, item.PartId, item.Operation, item.Quantity, "OUT");
```

**Prevention**: Always parameterize business values instead of hardcoding them. Transaction types should reflect user intent, not system assumptions.

---

## 🔗 Cross-Service Communication Pitfalls

### **Issue: Services Don't Notify UI of State Changes**
**Symptoms**: UI doesn't update when background services complete operations (e.g., QuickButtons history not updating after removals).

**Root Cause**: No event-driven communication between services and ViewModels.

**Solution Pattern**:
```csharp
// ✅ CORRECT: Define event arguments for cross-service communication
public class SessionTransactionEventArgs : EventArgs
{
    public string UserId { get; set; } = string.Empty;
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

// ✅ CORRECT: Add events to service interfaces
public interface IQuickButtonsService
{
    event EventHandler<QuickButtonsChangedEventArgs>? QuickButtonsChanged;
    event EventHandler<SessionTransactionEventArgs>? SessionTransactionAdded; // NEW
    // ... other methods
}

// ✅ CORRECT: Raise events after operations complete
public async Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity, string transactionType)
{
    // ... perform operation
    
    // CRITICAL: Raise event to notify UI
    SessionTransactionAdded?.Invoke(this, new SessionTransactionEventArgs
    {
        UserId = userId,
        PartId = partId,
        Operation = operation,
        Location = "Various",
        Quantity = quantity,
        TransactionType = transactionType,
        Notes = $"Transaction logged via {transactionType} operation"
    });
}

// ✅ CORRECT: Subscribe to events in ViewModels
public QuickButtonsViewModel(IQuickButtonsService quickButtonsService)
{
    _quickButtonsService = quickButtonsService;
    _quickButtonsService.QuickButtonsChanged += OnQuickButtonsChanged;
    _quickButtonsService.SessionTransactionAdded += OnSessionTransactionAdded; // NEW
}

private void OnSessionTransactionAdded(object? sender, SessionTransactionEventArgs e)
{
    if (e.UserId == _applicationState.CurrentUser)
    {
        AddSessionTransaction(e.UserId, e.PartId, e.Operation, e.Location, e.Quantity, e.Notes);
    }
}
```

**Prevention**: Always implement event-driven architecture for cross-service state changes that affect UI.

---

## 🎯 Method Overloading Pitfalls

### **Issue: Breaking Existing Code When Adding Parameters**
**Symptoms**: Compilation errors when existing service calls need new parameters.

**Root Cause**: Adding required parameters to existing methods breaks all existing callers.

**Solution Pattern**:
```csharp
// ✅ CORRECT: Create overloaded methods to maintain backward compatibility
public async Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity)
{
    // CRITICAL: Call new overload with default transaction type
    return await AddTransactionToLast10Async(userId, partId, operation, quantity, "IN");
}

public async Task<bool> AddTransactionToLast10Async(string userId, string partId, string operation, int quantity, string transactionType)
{
    // New implementation with transaction type parameter
    // ... implementation here
}
```

**Prevention**: Always use method overloading when adding parameters to maintain backward compatibility.

---

## 🧵 UI Thread Pitfalls

### **Issue: Collection Operations on Wrong Thread**
**Symptoms**: Application crashes or UI doesn't update when clearing collections from background threads.

**Root Cause**: ObservableCollections bound to UI must be modified on the UI thread.

**Solution Pattern**:
```csharp
// ❌ WRONG: Modifying UI collections from background thread
[RelayCommand]
private async Task Reset()
{
    InventoryItems.Clear(); // WRONG - may not be on UI thread
}

// ✅ CORRECT: Always use Dispatcher for UI collection operations
[RelayCommand]
private async Task Reset()
{
    await Application.Current!.Dispatcher.InvokeAsync(() =>
    {
        InventoryItems.Clear(); // CORRECT - on UI thread
        SelectedItems.Clear();
        SelectedItem = null;
        OnPropertyChanged(nameof(SelectedItem));
    });
}
```

**Prevention**: Always wrap collection modifications in Dispatcher.InvokeAsync when called from async methods.

---

## 🔍 Debugging Process Pitfalls

### **Issue: Assuming Root Cause Without Investigation**
**Symptoms**: Fixing symptoms instead of root causes leads to incomplete solutions.

**Root Cause**: Not systematically analyzing the full data flow from user action to UI update.

**Solution Pattern**:
```
✅ CORRECT: Systematic debugging approach
1. Identify the complete user workflow (e.g., Remove item → Update history)
2. Trace the data flow through all layers:
   - UI Event (RemoveTabView button click)
   - ViewModel Command (RemoveItemViewModel.RemoveSelectedItems)
   - Service Layer (RemoveService.RemoveInventoryItems)
   - Cross-Service Communication (QuickButtonsService.AddTransactionToLast10Async)
   - UI Update (QuickButtonsViewModel.AddSessionTransaction)
3. Test each layer independently
4. Identify the specific break point in the chain
5. Implement event-driven communication to bridge gaps
```

**Prevention**: Always map the complete user workflow before implementing fixes.

---

## 📝 Event Subscription Cleanup Pitfalls

### **Issue: Memory Leaks from Unsubscribed Events**
**Symptoms**: Memory usage grows over time, potential crashes.

**Root Cause**: Not unsubscribing from events when objects are disposed.

**Solution Pattern**:
```csharp
// ✅ CORRECT: Always implement proper event cleanup
public class SomeViewModel : IDisposable
{
    public SomeViewModel(ISomeService service)
    {
        _service = service;
        _service.SomeEvent += OnSomeEvent; // Subscribe
    }
    
    // CRITICAL: Always unsubscribe in Dispose
    public void Dispose()
    {
        if (_service != null)
        {
            _service.SomeEvent -= OnSomeEvent; // Unsubscribe
        }
    }
}

// For UserControls, use OnDetachedFromVisualTree
protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    if (_someControl != null)
    {
        _someControl.SomeEvent -= OnSomeEvent;
    }
    base.OnDetachedFromVisualTree(e);
}
```

**Prevention**: Always implement proper cleanup patterns for event subscriptions.

---

## 🔧 Interface Extension Pitfalls

### **Issue: Adding Events to Interfaces Without Implementation**
**Symptoms**: Compilation errors when interface events aren't implemented in concrete classes.

**Root Cause**: Forgetting to add event implementation to all interface implementers.

**Solution Pattern**:
```csharp
// ✅ CORRECT: Add events to both interface and implementation
public interface IQuickButtonsService
{
    event EventHandler<SessionTransactionEventArgs>? SessionTransactionAdded; // Add to interface
}

public class QuickButtonsService : IQuickButtonsService
{
    public event EventHandler<SessionTransactionEventArgs>? SessionTransactionAdded; // Add to implementation
    
    // Use in methods
    SessionTransactionAdded?.Invoke(this, eventArgs);
}
```

**Prevention**: Always update both interface and all implementations when adding events or methods.

---

## 📊 Summary of Critical Prevention Strategies

1. **Event Synchronization**: Always verify UI control events propagate to ViewModel properties
2. **Reset Commands**: Clear ALL collections and properties, use UI thread dispatcher
3. **Transaction Types**: Parameterize business values, never hardcode them
4. **Cross-Service Communication**: Implement event-driven architecture for state changes
5. **Method Overloading**: Maintain backward compatibility when adding parameters
6. **UI Threading**: Use Dispatcher.InvokeAsync for collection operations in async methods
7. **Systematic Debugging**: Map complete user workflows before implementing fixes
8. **Event Cleanup**: Always implement proper event unsubscription patterns
9. **Interface Consistency**: Update both interfaces and all implementations together

**These patterns prevent the most common pitfalls encountered in MTM WIP Application development.**