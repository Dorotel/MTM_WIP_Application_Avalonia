---
name: ViewModel Code Review Checklist
description: 'Comprehensive quality assurance checklist for CommunityToolkit.Mvvm ViewModel code review in MTM (Manufacturing Transaction Management) manufacturing context. Ensures compliance with CommunityToolkit.Mvvm source generators, manufacturing domain business rules, proper error handling, and performance standards for inventory management, transaction processing, and master data operations.'
applies_to: '**/*ViewModel.cs'
manufacturing_context: true
review_type: 'code'
quality_gate: 'critical'
---

# ViewModel Code Review - Quality Assurance Checklist

## Context

- **Component Type**: ViewModel (CommunityToolkit.Mvvm)
- **Manufacturing Domain**: Inventory Management / Transaction Processing / Master Data
- **Quality Gate**: Pre-merge (Critical)
- **Reviewer**: [Name]
- **Review Date**: [Date]
- **Component File**: [File Path]
- **Related Models**: [Associated Model/Entity Classes]
- **Service Dependencies**: [Service Classes Used]

## CommunityToolkit.Mvvm Compliance (Version 8.3.2)

### Source Generator Patterns

- [ ] **[ObservableObject] attribute** applied to ViewModel class declaration
- [ ] **partial class declaration** used to support source generators
- [ ] **[ObservableProperty] attributes** used for all bindable properties (no manual INotifyPropertyChanged)
- [ ] **Private fields** with underscore naming (_fieldName) for observable properties
- [ ] **[RelayCommand] attributes** used for all commands (no manual ICommand implementations)
- [ ] **No ReactiveUI patterns** (ReactiveUI completely removed from MTM)

### Property Implementation

- [ ] **Observable properties are private fields** with `[ObservableProperty]` attribute
- [ ] **Property naming convention**: private `_camelCase` generates public `PascalCase` property
- [ ] **Property change notifications** configured with `[NotifyPropertyChangedFor(nameof(OtherProperty))]`
- [ ] **Property validation** uses appropriate validation attributes (Required, Range, etc.)
- [ ] **Computed properties** properly depend on observable properties via NotifyPropertyChangedFor
- [ ] **Collection properties** use `ObservableCollection<T>` for UI binding
- [ ] **No circular dependencies** between properties

### Command Implementation

- [ ] **[RelayCommand] attributes** used for all command methods
- [ ] **Async commands** use `async Task` methods (never `async void`)
- [ ] **Command naming**: method `DoSomethingAsync()` generates `DoSomethingCommand` property
- [ ] **CanExecute logic** implemented via `CanExecute = nameof(CanDoSomething)` parameter
- [ ] **Command state updates** triggered by `[NotifyCanExecuteChangedFor(nameof(PropertyName))]`
- [ ] **Command parameters** properly typed and validated
- [ ] **Long-running operations** show loading states and disable UI appropriately

### Example MVVM Community Toolkit Pattern

```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Observable properties (private fields)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteAddInventory))]
    private string partId = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteAddInventory))]
    private bool isLoading;
    
    // Commands with CanExecute logic
    [RelayCommand(CanExecute = nameof(CanExecuteAddInventory))]
    private async Task AddInventoryAsync()
    {
        IsLoading = true;
        try
        {
            await _inventoryService.AddInventoryAsync(PartId);
            // Success handling
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Add Inventory");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanExecuteAddInventory => 
        !IsLoading && !string.IsNullOrWhiteSpace(PartId);
}
```

## Manufacturing Domain Validation

### Business Logic

- [ ] **Manufacturing workflows** properly implemented (operations 90, 100, 110, 120, etc.)
- [ ] **Transaction types** determined by user intent (IN/OUT/TRANSFER), not operation numbers
- [ ] **Part ID validation** follows MTM format requirements (alphanumeric, max 50 chars)
- [ ] **Quantity validation** enforces positive integers only
- [ ] **Location validation** ensures valid manufacturing locations

### Data Integrity

- [ ] **Input validation** prevents invalid manufacturing data
- [ ] **Error handling** includes manufacturing-specific error messages
- [ ] **Data consistency** maintained across related properties
- [ ] **Manufacturing constraints** enforced (no negative inventory, valid operations)

## Error Handling and Logging

### Exception Management

- [ ] **Try-catch blocks** around all async operations
- [ ] **Centralized error handling** uses proper error handling patterns
- [ ] **Error messages** are user-friendly and actionable for manufacturing operators
- [ ] **Logging** includes sufficient context for troubleshooting manufacturing issues
- [ ] **No swallowed exceptions** without proper handling or logging

### Service Integration

- [ ] **Service calls wrapped** in try-catch with proper error handling
- [ ] **Service failures** don't crash the UI or leave it in inconsistent state
- [ ] **Loading states** properly managed during service operations
- [ ] **User feedback** provided for long-running manufacturing operations

## Memory Management and Performance

### Resource Management

- [ ] **Event subscriptions** properly cleaned up in Dispose or OnDeactivated
- [ ] **Large collections** use ObservableCollection with appropriate size limits
- [ ] **Background operations** properly cancelled when ViewModel is disposed
- [ ] **No memory leaks** from static event handlers or circular references

### Performance Considerations

- [ ] **Heavy operations** moved to background threads (not in property setters)
- [ ] **Collection updates** batched for large datasets
- [ ] **Property change notifications** not causing performance issues
- [ ] **Manufacturing data loading** optimized for typical operator workflows

## Avalonia-Specific Considerations

### UI Threading

- [ ] **UI updates** dispatched to UI thread when necessary
- [ ] **Cross-thread operations** handled properly
- [ ] **Data binding** works correctly with Avalonia's binding system
- [ ] **View lifecycle** properly handled (OnActivated/OnDeactivated)

## Sign-off

- [ ] **Developer Self-Review**: _________________ - _________
- [ ] **Peer Code Review**: _________________ - _________  
- [ ] **Manufacturing Domain Review**: _________________ - _________
- [ ] **Quality Gate Approval**: _________________ - _________

## Review Notes

### Issues Identified

[Document any issues found during review]

### Recommendations

[Document improvement suggestions]

### Manufacturing Domain Feedback

[Document feedback specific to manufacturing workflows and requirements]

---

**Review Status**: [ ] Approved [ ] Approved with Comments [ ] Requires Changes