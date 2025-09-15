---
name: ViewModel Code Review Checklist
description: 'Quality assurance checklist for MVVM Community Toolkit ViewModel code review in MTM manufacturing context'
applies_to: '**/*ViewModel.cs'
manufacturing_context: true
review_type: 'code'
quality_gate: 'critical'
---

# ViewModel Code Review - Quality Assurance Checklist

## Context
- **Component Type**: ViewModel (MVVM Community Toolkit)
- **Manufacturing Domain**: Inventory Management / Transaction Processing / Master Data
- **Quality Gate**: Pre-merge (Critical)
- **Reviewer**: [Name]
- **Review Date**: [Date]

## MVVM Community Toolkit Compliance

### Source Generator Patterns
- [ ] **[ObservableObject] attribute applied** to ViewModel class
- [ ] **[ObservableProperty] attributes** used for all bindable properties (no manual INotifyPropertyChanged)
- [ ] **[RelayCommand] attributes** used for all commands (no manual ICommand implementations)
- [ ] **No ReactiveUI patterns** present (ReactiveObject, ReactiveCommand, etc.)
- [ ] **Property naming follows convention** (private field with underscore, public property auto-generated)

### Property Implementation
- [ ] **Observable properties are private fields** with [ObservableProperty] attribute
- [ ] **Property change notifications** properly configured with [NotifyPropertyChangedFor]
- [ ] **Property validation** uses DataAnnotations attributes where applicable
- [ ] **Computed properties** properly depend on observable properties
- [ ] **No circular dependencies** between properties

### Command Implementation
- [ ] **[RelayCommand] attributes** used for all commands
- [ ] **Async commands** use async Task methods (not void)
- [ ] **CanExecute logic** implemented where appropriate
- [ ] **Command state updates** triggered by property changes ([NotifyCanExecuteChangedFor])
- [ ] **Command parameters** properly typed and validated

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
- [ ] **Centralized error handling** uses Services.ErrorHandling.HandleErrorAsync()
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
- [ ] **Event subscriptions** properly cleaned up in Dispose or detachment
- [ ] **Large collections** use ObservableCollection with appropriate size limits
- [ ] **Background operations** properly cancelled when ViewModel is disposed
- [ ] **No memory leaks** from static event handlers or circular references

### Performance Considerations
- [ ] **Heavy operations** moved to background threads (not in property setters)
- [ ] **Collection updates** batched for large datasets
- [ ] **Property change notifications** not causing performance issues
- [ ] **Manufacturing data loading** optimized for typical operator workflows

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