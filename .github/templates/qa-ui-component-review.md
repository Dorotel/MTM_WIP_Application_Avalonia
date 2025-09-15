---
name: UI Component Code Review Checklist
description: 'Quality assurance checklist for Avalonia UI component code review in MTM manufacturing context'
applies_to: '**/*.axaml,**/*View.cs,**/*Control.cs'
manufacturing_context: true
review_type: 'code'
quality_gate: 'critical'
---

# UI Component Code Review - Quality Assurance Checklist

## Context
- **Component Type**: Avalonia UI Component (.axaml + code-behind)
- **Manufacturing Domain**: Manufacturing Operator Interface / Inventory Management UI
- **Quality Gate**: Pre-merge (Critical)
- **Reviewer**: [Name]
- **Review Date**: [Date]

## Avalonia AXAML Syntax Compliance

### Mandatory AXAML Patterns
- [ ] **Correct namespace** `xmlns="https://github.com/avaloniaui"`
- [ ] **x:Name attribute** used for Grid elements (NOT Name attribute - prevents AVLN2000)
- [ ] **DynamicResource** used for all theme colors and brushes
- [ ] **Grid definitions** use attribute form when possible `RowDefinitions="Auto,*"`
- [ ] **TextBlock** used instead of Label
- [ ] **Flyout** used instead of Popup

### Required AXAML Header Structure
- [ ] **xmlns declarations** properly included
- [ ] **x:Class** properly defined
- [ ] **Design data** included for development experience
- [ ] **Theme resources** properly referenced

### Layout Patterns
- [ ] **ScrollViewer** as root container for tab views (prevents overflow)
- [ ] **Grid with RowDefinitions="*,Auto"** pattern for content/actions separation
- [ ] **Consistent spacing** (8px, 16px, 24px) following MTM design system
- [ ] **Responsive design** considerations for different screen sizes

## MTM Design System Integration

### Theme Compliance
- [ ] **MTM color palette** used exclusively (MTM_Shared_Logic.* resources)
- [ ] **DynamicResource bindings** for ALL colors
- [ ] **Theme switching** properly supported
- [ ] **Manufacturing-specific themes** (MTM_Blue, MTM_Green, MTM_Red, MTM_Dark) supported
- [ ] **Consistent visual hierarchy** maintained

### Manufacturing UI Standards
- [ ] **Card-based layout** with proper spacing and shadows
- [ ] **Manufacturing operation colors** used appropriately
- [ ] **Status indicators** clearly visible for manufacturing states
- [ ] **Action buttons** follow primary/secondary/warning color patterns
- [ ] **Loading indicators** appropriate for manufacturing operations

## Data Binding and MVVM Integration

### Binding Patterns
- [ ] **Proper binding syntax** `{Binding PropertyName}`
- [ ] **Two-way binding** used appropriately for input controls
- [ ] **Collection binding** properly implemented with ItemsSource
- [ ] **Command binding** for all user actions
- [ ] **Value converters** used appropriately for data transformation

### ViewModel Integration
- [ ] **DataContext** properly set (via DI, not direct instantiation)
- [ ] **No business logic** in code-behind
- [ ] **Event handlers** minimal and UI-focused only
- [ ] **Property change notifications** properly observed
- [ ] **Command execution** properly handled

## Manufacturing Usability

### Operator Experience
- [ ] **Keyboard navigation** efficient for data entry workflows
- [ ] **Tab order** logical for manufacturing data entry sequences
- [ ] **Focus management** appropriate for manufacturing workflows
- [ ] **Error indicators** clear and visible to operators
- [ ] **Success feedback** immediate and clear

### Data Entry Efficiency
- [ ] **Auto-complete** implemented for part IDs and operations
- [ ] **Input validation** real-time and manufacturing-specific
- [ ] **Quick actions** accessible for common manufacturing operations
- [ ] **Keyboard shortcuts** for frequent manufacturing tasks
- [ ] **Barcode scanning** support where applicable

### Manufacturing Context
- [ ] **Operation workflows** clearly represented in UI
- [ ] **Inventory levels** clearly displayed with appropriate colors
- [ ] **Transaction history** easily accessible and readable
- [ ] **Manufacturing alerts** prominently displayed
- [ ] **Status indicators** update in real-time

## Accessibility and Cross-Platform

### Accessibility Compliance
- [ ] **AutomationProperties** set for screen readers
- [ ] **High contrast** mode supported
- [ ] **Keyboard accessibility** complete
- [ ] **Focus indicators** visible and clear
- [ ] **Alternative text** provided for visual elements

### Cross-Platform Compatibility
- [ ] **Windows** - Native look and feel maintained
- [ ] **macOS** - Cocoa integration working properly
- [ ] **Linux** - GTK backend rendering correctly
- [ ] **Android** - Touch interface optimized (where applicable)
- [ ] **Platform-specific features** properly abstracted

## Performance and Memory Management

### Rendering Performance
- [ ] **UI virtualization** used for large datasets
- [ ] **Layout complexity** kept reasonable
- [ ] **Heavy operations** moved off UI thread
- [ ] **Image resources** optimized for manufacturing UI
- [ ] **Animation performance** smooth and non-blocking

### Memory Management
- [ ] **Event handlers** properly unsubscribed in OnDetachedFromVisualTree
- [ ] **Data binding** doesn't create memory leaks
- [ ] **Large collections** properly managed with limits
- [ ] **Background operations** cancelled when view is disposed
- [ ] **Resource cleanup** implemented in code-behind dispose patterns

## Code-Behind Patterns

### Minimal Code-Behind Standard
- [ ] **Constructor** only contains InitializeComponent() and minimal setup
- [ ] **Business logic** absent from code-behind
- [ ] **Event handlers** only for UI-specific logic (focus, selection, etc.)
- [ ] **Resource cleanup** implemented in OnDetachedFromVisualTree
- [ ] **No direct service calls** from code-behind

### Disposal Patterns
- [ ] **OnDetachedFromVisualTree** overridden for cleanup
- [ ] **Event unsubscription** implemented
- [ ] **Timer disposal** implemented where applicable
- [ ] **Background task cancellation** implemented
- [ ] **DataContext disposal** handled appropriately

## Manufacturing Domain Integration

### Business Context
- [ ] **Manufacturing terminology** used consistently in UI
- [ ] **Workflow steps** clearly represented
- [ ] **Data validation** messages specific to manufacturing context
- [ ] **Error messages** actionable for manufacturing operators
- [ ] **Help text** relevant to manufacturing processes

### Data Presentation
- [ ] **Manufacturing data** formatted appropriately (quantities, dates, IDs)
- [ ] **Status indicators** meaningful for manufacturing context
- [ ] **Progress indicators** relevant to manufacturing operations
- [ ] **Alerts and warnings** appropriate for manufacturing risks
- [ ] **Historical data** presented in manufacturing-relevant format

## Testing Requirements

### UI Testing Coverage
- [ ] **User workflows** covered in UI automation tests
- [ ] **Data binding** tested with mock ViewModels
- [ ] **Command execution** tested
- [ ] **Error scenarios** tested with invalid data
- [ ] **Manufacturing scenarios** tested with domain-specific data

### Cross-Platform Testing
- [ ] **Rendering consistency** tested across platforms
- [ ] **Input handling** tested on different platforms
- [ ] **Performance** validated on target platforms
- [ ] **Accessibility** tested with assistive technologies
- [ ] **Touch interface** tested on mobile platforms (where applicable)

## Sign-off

- [ ] **Developer Self-Review**: _________________ - _________
- [ ] **UI/UX Review**: _________________ - _________
- [ ] **Manufacturing Domain Review**: _________________ - _________
- [ ] **Accessibility Review**: _________________ - _________
- [ ] **Quality Gate Approval**: _________________ - _________

## Review Notes

### UI/UX Feedback
[Document user interface and experience feedback]

### Manufacturing Usability
[Document feedback specific to manufacturing operator workflows]

### Performance Considerations
[Document performance-related observations]

### Accessibility Issues
[Document accessibility compliance issues]

---

**Review Status**: [ ] Approved [ ] Approved with Comments [ ] Requires Changes  
**UI/UX Validation**: [ ] Complete [ ] Needs Revision  
**Manufacturing Usability**: [ ] Validated [ ] Needs Improvement  
**Cross-Platform Testing**: [ ] Complete [ ] Pending