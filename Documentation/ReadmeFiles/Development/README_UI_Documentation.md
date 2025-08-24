# UI Documentation Directory

This directory contains comprehensive documentation for all user interface components in the MTM WIP Application Avalonia.

## ?? Documentation Structure

### Component Organization

**NEW: Mandatory View Structure Standards**
- **[Views_Structure_Standards.instruction.md](../../Documentation/Development/UI_Documentation/Views_Structure_Standards.instruction.md)**: Mandatory standards for all Views implementation including proportional scaling, layout patterns, and MTM styling requirements

#### `Controls/`
Individual control specifications organized by functional area:

##### `MainForm/`
Primary application interface controls:
- **Control_InventoryTab.instructions.md**: Main inventory management interface
- **Control_TransferTab.instructions.md**: Inventory transfer operations
- **Control_RemoveTab.instructions.md**: Inventory removal operations
- **Control_QuickButtons.instructions.md**: Quick action buttons panel
- **Control_AdvancedInventory.instructions.md**: Advanced inventory features
- **Control_AdvancedRemove.instructions.md**: Advanced removal features

##### `SettingsForm/`
Application configuration and settings controls:
- **Control_Database.instructions.md**: Database connection settings
- **Control_Theme.instructions.md**: Theme and appearance settings
- **Control_Shortcuts.instructions.md**: Keyboard shortcut configuration
- **Control_About.instructions.md**: Application information display
- **Add/Edit/Remove controls**: CRUD operations for master data
- **README.md**: Settings form organization guide

##### `Shared/`
Reusable components used across multiple forms:
- **Control_ProgressBarUserControl.instructions.md**: Progress indication component
- **ColumnOrderDialog.instructions.md**: Data grid column management

##### `Addons/`
Special purpose and extension controls:
- **Control_ConnectionStrengthControl.instructions.md**: Database connection monitoring

#### `Forms/`
Complete form-level documentation:
- **MainForm.instructions.md**: Primary application window
- **SettingsForm.instructions.md**: Configuration interface
- **EnhancedErrorDialog.instructions.md**: Error display dialog
- **SplashScreenForm.instructions.md**: Application startup screen
- **Transactions.instructions.md**: Transaction history interface

## ?? Documentation Standards

### Component Documentation Format
Each `.instructions.md` file follows a standardized format:

#### Required Sections
1. **UI Element Name**: Clear, descriptive component identifier
2. **Description**: Purpose, functionality, and context overview
3. **Visual Representation**: Layout details, control types, and appearance
4. **Component Structure**: Hierarchical breakdown of all controls
5. **Props/Inputs**: Parameters, configuration options, and data sources
6. **Interactions/Events**: User interaction patterns and event handling
7. **Business Logic**: Integration with services and backend operations
8. **Related Files**: Dependencies, integration points, and file relationships
9. **Notes**: Implementation details, performance considerations, and special requirements

#### Documentation Quality Standards
- **Comprehensive Coverage**: Every control and interaction documented
- **Code Examples**: Actual implementation snippets where relevant
- **Integration Details**: Clear explanation of component relationships
- **Business Context**: Connection to MTM business processes
- **Technical Accuracy**: Precise technical details and specifications

### Naming Conventions
- **Controls**: `Control_{Name}.instructions.md`
- **Forms**: `{Name}Form.instructions.md`
- **Dialogs**: `{Name}Dialog.instructions.md`
- **User Controls**: `{Name}UserControl.instructions.md`

## ?? UI Architecture Patterns

### Avalonia UI Implementation
All components follow modern Avalonia UI patterns:

#### MVVM with ReactiveUI
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.ComponentView"
             x:DataType="vm:ComponentViewModel"
             x:CompileBindings="True">
    <!-- Component layout with compiled bindings -->
</UserControl>
```

#### Reactive Command Patterns
```csharp
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public ComponentViewModel()
{
    var canSave = this.WhenAnyValue(vm => vm.IsFormValid);
    SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, canSave);
    
    SaveCommand.ThrownExceptions
        .Subscribe(HandleError);
}
```

### MTM Design System
Consistent application of MTM brand elements:

#### Color Palette Implementation
```xml
<!-- MTM Primary Colors -->
<SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
<SolidColorBrush x:Key="AccentBrush" Color="#BA45ED"/>
<SolidColorBrush x:Key="SecondaryBrush" Color="#8345ED"/>

<!-- Hero Gradient -->
<LinearGradientBrush x:Key="HeroGradientBrush" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#4574ED" Offset="0"/>
    <GradientStop Color="#4B45ED" Offset="0.3"/>
    <GradientStop Color="#8345ED" Offset="0.7"/>
    <GradientStop Color="#BA45ED" Offset="1"/>
</LinearGradientBrush>
```

#### Modern UI Patterns
- **Card-Based Layout**: Clean, organized information presentation
- **Responsive Design**: Adaptive layouts for different screen sizes
- **Progressive Disclosure**: Hierarchical information organization
- **Contextual Actions**: Right-click context menus and hover states

### Component Hierarchy Standards
Documented component structures follow consistent patterns:

```
Control_ComponentName
??? Main Container (GroupBox/Border)
?   ??? Layout Container (Grid/StackPanel)
?       ??? Header Section
?       ?   ??? Title and Description
?       ?   ??? Action Buttons
?       ??? Content Section
?       ?   ??? Form Controls
?       ?   ??? Data Display
?       ?   ??? Interactive Elements
?       ??? Footer Section
?           ??? Status Information
?           ??? Secondary Actions
```

## ?? Business Logic Integration

### Database Operation Patterns
UI components integrate with services using stored procedures only:

```csharp
// ? CORRECT: Service integration with stored procedures
private async Task LoadDataAsync()
{
    var result = await _inventoryService.GetInventoryAsync(searchCriteria);
    if (result.IsSuccess)
    {
        Items.Clear();
        Items.AddRange(result.Data);
    }
    else
    {
        ShowError(result.ErrorMessage);
    }
}

// Service internally uses stored procedures:
// await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
//     connectionString, "sp_GetInventory", parameters);
```

### Transaction Type Logic
Components correctly implement MTM business rules:

```csharp
// ? CORRECT: TransactionType based on user intent
private async Task ProcessInventoryActionAsync(UserAction action)
{
    var transactionType = action switch
    {
        UserAction.AddStock => TransactionType.IN,      // User adding stock
        UserAction.RemoveStock => TransactionType.OUT,  // User removing stock
        UserAction.TransferStock => TransactionType.TRANSFER, // User moving stock
        _ => throw new ArgumentException($"Unknown action: {action}")
    };
    
    // Operation is just a workflow step identifier
    await _inventoryService.ProcessTransactionAsync(
        partId, operation, transactionType, quantity, location);
}
```

### Validation and Error Handling
Comprehensive validation patterns documented for each component:

```csharp
// Form validation with visual feedback
private void ValidateForm()
{
    var isValid = true;
    
    if (string.IsNullOrWhiteSpace(PartId))
    {
        PartIdControl.SetErrorState(true);
        isValid = false;
    }
    
    SaveCommand.CanExecute = isValid;
}
```

## ?? User Experience Guidelines

### Accessibility Standards
All components implement comprehensive accessibility:
- **Keyboard Navigation**: Full keyboard accessibility for all functions
- **Screen Reader Support**: Proper labeling and ARIA attributes
- **High Contrast Support**: Theme-aware color schemes
- **Focus Management**: Logical tab order and focus indicators

### Responsive Design Principles
Components adapt to different screen sizes and orientations:
- **Flexible Layouts**: Grid and panel layouts that adjust to available space
- **Scalable Typography**: Font sizes that scale with system settings
- **Touch-Friendly**: Controls sized appropriately for touch interaction
- **Density Adaptation**: UI density adjustment for different DPI settings

### Performance Optimization
UI components implement performance best practices:
- **Virtualization**: Large data sets use virtualized controls
- **Lazy Loading**: Content loaded only when needed
- **Efficient Binding**: Compiled bindings for optimal performance
- **Resource Management**: Proper disposal of resources and subscriptions

## ?? Testing and Validation

### Component Testing Standards
Each documented component includes testing considerations:
- **Unit Tests**: ViewModel logic and business rule validation
- **Integration Tests**: Service interaction and data flow validation
- **UI Tests**: User interface functionality and accessibility testing
- **Performance Tests**: Response time and resource usage validation

### Documentation Validation
Regular validation ensures documentation accuracy:
- **Code Synchronization**: Documentation matches actual implementation
- **Completeness Checks**: All components have complete documentation
- **Link Validation**: All references and links are current and functional
- **Example Testing**: Code examples compile and execute correctly

## ?? Development Workflow

### Creating New Component Documentation
1. **Analysis Phase**: Understand component purpose and requirements
2. **Design Documentation**: Create detailed specification following template
3. **Implementation Guidance**: Provide clear implementation instructions
4. **Integration Details**: Document relationships with other components
5. **Testing Specifications**: Define testing requirements and scenarios

### Updating Existing Documentation
1. **Change Assessment**: Evaluate impact of modifications
2. **Documentation Updates**: Revise affected sections thoroughly
3. **Cross-Reference Validation**: Update related documentation
4. **Review Process**: Conduct thorough review of changes
5. **Validation Testing**: Verify documentation accuracy

### Documentation Maintenance
- **Regular Reviews**: Periodic assessment of documentation currency
- **User Feedback**: Incorporation of developer feedback and suggestions
- **Best Practice Updates**: Integration of new patterns and standards
- **Tool Integration**: Enhancement of documentation generation tools

## ?? Integration with Development Process

### Code Generation Support
Documentation supports automated code generation:
- **Template Compatibility**: Works with custom prompt templates
- **Pattern Standardization**: Consistent patterns across components
- **Metadata Inclusion**: Structured data for automated processing
- **Quality Assurance**: Built-in validation for generated code

### Architecture Compliance
Documentation enforces architectural standards:
- **MVVM Adherence**: Clear separation of concerns
- **Service Integration**: Proper dependency injection patterns
- **Error Handling**: Comprehensive error management strategies
- **Performance Guidelines**: Optimization recommendations and requirements

---

*This directory provides the complete specification for all user interface components, ensuring consistent implementation of the MTM WIP Application's modern, accessible, and maintainable user experience.*