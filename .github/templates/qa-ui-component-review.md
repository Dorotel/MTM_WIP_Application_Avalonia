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

## Avalonia AXAML Syntax Compliance (Version 11.3.4)

### Mandatory AXAML Patterns

- [ ] **Correct namespace** `xmlns="https://github.com/avaloniaui"`
- [ ] **NOT WPF namespace** (`http://schemas.microsoft.com/winfx/2006/xaml/presentation` is incorrect)
- [ ] **x:Name attribute** used for named elements (prevents AVLN2000 errors)
- [ ] **Never use Name attribute** on Grid elements (causes Avalonia compilation errors)
- [ ] **DynamicResource** used for all theme colors and brushes
- [ ] **Grid definitions** use attribute form when possible: `RowDefinitions="Auto,*,Auto"`
- [ ] **TextBlock** used instead of Label for text display (Avalonia best practice)
- [ ] **Flyout** used instead of Popup for overlay content

### Required AXAML Header Structure

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="MTM_WIP_Application_Avalonia.Views.ExampleView">
```

- [ ] **xmlns declarations** properly included
- [ ] **x:Class** properly defined matching code-behind class  
- [ ] **Design data** included for development experience
- [ ] **Design dimensions** appropriate for content

### Avalonia-Specific Layout Patterns

- [ ] **ScrollViewer** as root container for scrollable content
- [ ] **Grid with proper definitions**: `<Grid x:Name="MainGrid" RowDefinitions="Auto,*">`
- [ ] **DockPanel** used appropriately for header/footer layouts
- [ ] **StackPanel** used for linear layouts with appropriate `Orientation`
- [ ] **Border** used for visual grouping with rounded corners
- [ ] **Consistent spacing** using `Margin` and `Padding` properties

## MTM Design System Integration

### Theme Compliance (19 Available Themes)

- [ ] **MTM theme resources** used exclusively via DynamicResource
- [ ] **Available themes**: MTM_Blue, MTM_Blue_Dark, MTM_Green, MTM_Green_Dark, MTM_Red, MTM_Red_Dark, etc.
- [ ] **DynamicResource bindings** for ALL colors and brushes:
  - `{DynamicResource MTM_Shared_Logic.PrimaryAction}`
  - `{DynamicResource MTM_Shared_Logic.BackgroundColor}`
  - `{DynamicResource MTM_Shared_Logic.BorderColor}`
- [ ] **Theme switching** properly supported (no hardcoded colors)
- [ ] **Dark/Light theme variants** compatibility verified
- [ ] **High contrast support** included

### MTM UI Standards

- [ ] **Manufacturing card-based layout** with proper spacing and elevation
- [ ] **Status indicators** clearly visible for manufacturing operations
- [ ] **Action buttons** follow MTM patterns:
  - Primary actions: Blue theme colors
  - Secondary actions: Neutral colors  
  - Destructive actions: Red theme colors
- [ ] **Loading indicators** appropriate for database operations
- [ ] **Manufacturing icons** from Material.Icons.Avalonia package
- [ ] **Consistent spacing**: 8px, 16px, 24px increments

## Data Binding and MVVM Integration

### Binding Patterns

- [ ] **Proper binding syntax** `{Binding PropertyName}`
- [ ] **Two-way binding** used appropriately for input controls
- [ ] **Collection binding** properly implemented with ItemsSource
- [ ] **Command binding** for all user actions
- [ ] **Value converters** used appropriately for data transformation

### ViewModel Integration

- [ ] **DataContext** properly set via dependency injection
- [ ] **No business logic** in code-behind
- [ ] **Event handlers** minimal and UI-focused only
- [ ] **Property change notifications** properly observed
- [ ] **Command execution** properly handled

## Manufacturing Usability

### Operator Experience

- [ ] **Keyboard navigation** efficient for data entry workflows
- [ ] **Tab order** logical for data entry sequences
- [ ] **Focus management** appropriate for workflows
- [ ] **Error indicators** clear and visible
- [ ] **Success feedback** immediate and clear

### Data Entry Efficiency

- [ ] **Input validation** real-time and context-specific
- [ ] **Quick actions** accessible for common operations
- [ ] **Keyboard shortcuts** for frequent tasks
- [ ] **Auto-complete** implemented where appropriate
- [ ] **Form validation** comprehensive and user-friendly

### Manufacturing Context

- [ ] **Workflow steps** clearly represented in UI
- [ ] **Data display** formatted appropriately for context
- [ ] **Status updates** real-time and accurate
- [ ] **Manufacturing terminology** used consistently
- [ ] **Error messages** actionable and context-specific

## Accessibility and Cross-Platform

### Accessibility Compliance

- [ ] **AutomationProperties** set for screen readers
- [ ] **High contrast** mode supported
- [ ] **Keyboard accessibility** complete
- [ ] **Focus indicators** visible and clear
- [ ] **Alternative text** provided for visual elements

### Cross-Platform Compatibility

- [ ] **Windows** - Native behavior and rendering
- [ ] **macOS** - Platform conventions followed
- [ ] **Linux** - GTK rendering verified
- [ ] **Platform-specific features** properly abstracted
- [ ] **Touch interface** optimized where applicable

## Performance and Memory Management

### Rendering Performance

- [ ] **UI virtualization** used for large datasets
- [ ] **Layout complexity** kept reasonable
- [ ] **Heavy operations** moved off UI thread
- [ ] **Resource usage** optimized
- [ ] **Animation performance** smooth and responsive

### Memory Management

- [ ] **Event handlers** properly unsubscribed
- [ ] **Data binding** doesn't create memory leaks
- [ ] **Large collections** properly managed
- [ ] **Background operations** cancelled appropriately
- [ ] **Resource cleanup** implemented properly

## Code-Behind Patterns

### Minimal Code-Behind Standard

- [ ] **Constructor** only contains InitializeComponent() and setup
- [ ] **Business logic** absent from code-behind
- [ ] **Event handlers** only for UI-specific logic
- [ ] **Resource cleanup** implemented in disposal methods
- [ ] **No direct service calls** from code-behind

### Disposal Patterns

- [ ] **Cleanup methods** implemented for resource disposal
- [ ] **Event unsubscription** implemented
- [ ] **Timer disposal** implemented where applicable
- [ ] **Background task cancellation** implemented
- [ ] **DataContext disposal** handled appropriately

## Manufacturing Domain Integration

### Business Context

- [ ] **Domain terminology** used consistently in UI
- [ ] **Workflow representation** clear and intuitive
- [ ] **Data validation** appropriate for domain context
- [ ] **Error messages** relevant and actionable
- [ ] **Help text** contextual and useful

### Data Presentation

- [ ] **Data formatting** appropriate for domain context
- [ ] **Status indicators** meaningful and clear
- [ ] **Progress indicators** relevant to operations
- [ ] **Alerts and warnings** appropriate for context
- [ ] **Historical data** presented effectively

## Testing Requirements

### UI Testing Coverage

- [ ] **User workflows** covered in automation tests
- [ ] **Data binding** tested with mock data
- [ ] **Command execution** tested
- [ ] **Error scenarios** tested with invalid data
- [ ] **Domain scenarios** tested with realistic data

### Cross-Platform Testing

- [ ] **Rendering consistency** verified across platforms
- [ ] **Input handling** tested on different platforms
- [ ] **Performance** validated on target platforms
- [ ] **Accessibility** tested with assistive technologies
- [ ] **Touch interface** tested where applicable

## Sign-off

- [ ] **Developer Self-Review**: _________________ - _________
- [ ] **UI/UX Review**: _________________ - _________
- [ ] **Domain Expert Review**: _________________ - _________
- [ ] **Accessibility Review**: _________________ - _________
- [ ] **Quality Gate Approval**: _________________ - _________

## Review Notes

### UI/UX Feedback

[Document user interface and experience feedback]

### Manufacturing Workflow Feedback

[Document feedback specific to operator workflows]

### Performance Considerations

[Document performance-related observations]

### Accessibility Issues

[Document accessibility compliance issues]

---

**Review Status**: [ ] Approved [ ] Approved with Comments [ ] Requires Changes  
**UI/UX Validation**: [ ] Complete [ ] Needs Revision  
**Manufacturing Usability**: [ ] Validated [ ] Needs Improvement  
**Cross-Platform Testing**: [ ] Complete [ ] Pending