# Controls Directory

This directory contains detailed specifications for all UI controls used throughout the MTM WIP Application Avalonia.

## ?? Control Organization

### Primary Application Controls (`MainForm/`)
Core controls that make up the main application interface:

#### Inventory Management
- **Control_InventoryTab**: Primary inventory item creation and management
- **Control_AdvancedInventory**: Extended inventory features and bulk operations
- **Control_QuickButtons**: Rapid inventory operations with user customization

#### Inventory Operations
- **Control_TransferTab**: Inter-location inventory transfers with audit trails
- **Control_RemoveTab**: Inventory removal operations with validation
- **Control_AdvancedRemove**: Bulk removal and advanced deletion features

### Configuration Controls (`SettingsForm/`)
Application settings and administrative controls:

#### Database and System
- **Control_Database**: Database connection configuration and testing
- **Control_Theme**: User interface theme and appearance settings
- **Control_Shortcuts**: Keyboard shortcut customization
- **Control_About**: Application information and version details

#### Master Data Management
Complete CRUD operations for system reference data:

##### User Management
- **Control_Add_User**: New user account creation
- **Control_Edit_User**: User account modification
- **Control_Remove_User**: User account deletion

##### Location Management
- **Control_Add_Location**: Storage location creation
- **Control_Edit_Location**: Location information updates
- **Control_Remove_Location**: Location removal operations

##### Part Management
- **Control_Add_PartID**: Part definition creation
- **Control_Edit_PartID**: Part information updates
- **Control_Remove_PartID**: Part removal operations

##### Operation Management
- **Control_Add_Operation**: Workflow operation creation
- **Control_Edit_Operation**: Operation parameter updates
- **Control_Remove_Operation**: Operation deletion

##### Item Type Management
- **Control_Add_ItemType**: Item category creation
- **Control_Edit_ItemType**: Item type modifications
- **Control_Remove_ItemType**: Item type removal

### Shared Components (`Shared/`)
Reusable controls used across multiple contexts:

#### Utility Controls
- **Control_ProgressBarUserControl**: Progress indication with status messaging
- **ColumnOrderDialog**: DataGrid column customization and ordering

### Extension Controls (`Addons/`)
Specialized controls for enhanced functionality:

#### System Monitoring
- **Control_ConnectionStrengthControl**: Real-time database connection monitoring

## ?? Control Design Standards

### MTM Design System Implementation
All controls follow consistent MTM branding and design patterns:

#### Color Scheme Application
```xml
<!-- Primary MTM colors used throughout controls -->
<SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
<SolidColorBrush x:Key="AccentBrush" Color="#BA45ED"/>
<SolidColorBrush x:Key="SecondaryBrush" Color="#8345ED"/>
<SolidColorBrush x:Key="BlueAccentBrush" Color="#4574ED"/>
<SolidColorBrush x:Key="PinkAccentBrush" Color="#ED45E7"/>
<SolidColorBrush x:Key="LightPurpleBrush" Color="#B594ED"/>
```

#### Layout Patterns
Consistent layout structures across all controls:
- **Card-Based Design**: Clean, organized information presentation
- **Responsive Grids**: Flexible layouts that adapt to content and screen size
- **Progressive Disclosure**: Hierarchical information organization
- **Action-Oriented Design**: Clear primary and secondary action patterns

### Control Hierarchy Standards
Standardized component structure documentation:

```
Control_Name
??? Main Container (GroupBox/Border)
?   ??? Primary Layout (TableLayoutPanel/Grid)
?       ??? Header Section
?       ?   ??? Title and Description
?       ?   ??? Primary Actions
?       ??? Content Section
?       ?   ??? Input Controls
?       ?   ??? Data Display
?       ?   ??? Navigation Elements
?       ??? Footer Section
?           ??? Status Information
?           ??? Secondary Actions
```

## ?? Control Implementation Patterns

### Avalonia UserControl Pattern
Standard UserControl implementation for all components:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.ControlNameView"
             x:DataType="vm:ControlNameViewModel"
             x:CompileBindings="True">
    
    <!-- Control layout with compiled bindings -->
    <Border Classes="card" Padding="24">
        <!-- Control content -->
    </Border>
</UserControl>
```

### ReactiveUI ViewModel Integration
ViewModels following reactive programming patterns:

```csharp
public class ControlNameViewModel : ReactiveObject
{
    // Observable properties
    private string _inputValue = string.Empty;
    public string InputValue
    {
        get => _inputValue;
        set => this.RaiseAndSetIfChanged(ref _inputValue, value);
    }

    // Derived properties
    private readonly ObservableAsPropertyHelper<bool> _isValid;
    public bool IsValid => _isValid.Value;

    // Reactive commands
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public ControlNameViewModel()
    {
        // Property derivation
        _isValid = this.WhenAnyValue(vm => vm.InputValue)
                      .Select(value => !string.IsNullOrWhiteSpace(value))
                      .ToProperty(this, vm => vm.IsValid);

        // Command creation
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, 
            this.WhenAnyValue(vm => vm.IsValid));

        // Error handling
        SaveCommand.ThrownExceptions
            .Subscribe(HandleError);
    }
}
```

## ?? Business Logic Integration

### Service Integration Patterns
Controls integrate with business services following established patterns:

```csharp
// Dependency injection in ViewModel constructor
public ControlNameViewModel(
    IInventoryService inventoryService,
    IUserAndTransactionServices userService,
    IApplicationStateService applicationState)
{
    _inventoryService = inventoryService;
    _userService = userService;
    _applicationState = applicationState;
}

// Service operation with error handling
private async Task SaveAsync()
{
    try
    {
        var result = await _inventoryService.CreateInventoryItemAsync(new InventoryItem
        {
            PartId = SelectedPart,
            Operation = SelectedOperation, // Workflow step identifier
            Location = SelectedLocation,
            Quantity = Quantity,
            // TransactionType determined by user intent (IN for creation)
        });

        if (result.IsSuccess)
        {
            StatusMessage = "Item saved successfully";
            await ResetFormAsync();
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
        }
    }
    catch (Exception ex)
    {
        ErrorMessage = $"Save operation failed: {ex.Message}";
    }
}
```

### Database Operation Standards
All controls follow stored procedure-only database access:

```csharp
// ? CORRECT: Service calls stored procedures internally
var result = await _inventoryService.GetInventoryAsync(searchCriteria);

// Service implementation uses stored procedures:
// await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
//     connectionString,
//     "sp_GetInventoryByPartAndOperation",
//     parameters
// );

// ? PROHIBITED: Direct SQL in controls
// var query = "SELECT * FROM inventory"; // NEVER DO THIS
```

### Transaction Type Logic
Controls implement correct MTM business rules:

```csharp
// ? CORRECT: TransactionType based on user intent, not operation
private TransactionType GetTransactionTypeForAction(ControlAction action)
{
    return action switch
    {
        ControlAction.AddInventory => TransactionType.IN,      // User adding stock
        ControlAction.RemoveInventory => TransactionType.OUT,  // User removing stock
        ControlAction.TransferInventory => TransactionType.TRANSFER, // User moving stock
        _ => throw new ArgumentException($"Unknown action: {action}")
    };
}

// Operation numbers are workflow step identifiers only
private bool IsValidOperation(string operation)
{
    // Operations like "90", "100", "110" are workflow steps, not transaction types
    return !string.IsNullOrWhiteSpace(operation) && 
           operation.All(char.IsDigit) && 
           operation.Length <= 10;
}
```

## ?? User Experience Standards

### Form Validation Patterns
Comprehensive validation with immediate user feedback:

```csharp
// Real-time validation with visual indicators
private void ValidateInput()
{
    var isPartValid = !string.IsNullOrWhiteSpace(SelectedPart);
    var isOperationValid = !string.IsNullOrWhiteSpace(SelectedOperation);
    var isLocationValid = !string.IsNullOrWhiteSpace(SelectedLocation);
    var isQuantityValid = Quantity > 0;

    // Update UI error states
    HasPartError = !isPartValid;
    HasOperationError = !isOperationValid;
    HasLocationError = !isLocationValid;
    HasQuantityError = !isQuantityValid;

    // Enable/disable save based on overall validity
    IsFormValid = isPartValid && isOperationValid && isLocationValid && isQuantityValid;
}
```

### Accessibility Implementation
Full accessibility support in all controls:

```xml
<!-- Keyboard navigation support -->
<TextBox AutomationProperties.Name="Part ID"
         AutomationProperties.HelpText="Enter the part identifier"
         TabIndex="1"/>

<!-- Screen reader descriptions -->
<Button Content="Save"
        AutomationProperties.Name="Save Inventory Item"
        AutomationProperties.HelpText="Saves the current inventory item to the database"
        IsDefault="True"/>

<!-- High contrast theme support -->
<Border Classes="form-field error"
        Classes.error="{Binding HasPartError}"/>
```

### Progressive Enhancement
Controls provide enhanced functionality while maintaining core usability:
- **Basic Functionality**: Core operations work without advanced features
- **Enhanced Features**: Additional functionality for power users
- **Graceful Degradation**: Continues operation if advanced features fail
- **Performance Optimization**: Efficient operation under various conditions

## ?? Testing and Quality Assurance

### Control Testing Standards
Each control includes comprehensive testing requirements:

#### Unit Testing
```csharp
[Test]
public async Task SaveCommand_ValidData_SavesSuccessfully()
{
    // Arrange
    var mockService = new Mock<IInventoryService>();
    var viewModel = new InventoryControlViewModel(mockService.Object);
    
    viewModel.SelectedPart = "TEST-PART";
    viewModel.SelectedOperation = "100";
    viewModel.SelectedLocation = "TEST-LOCATION";
    viewModel.Quantity = 10;
    
    // Act
    await viewModel.SaveCommand.Execute();
    
    // Assert
    mockService.Verify(s => s.CreateInventoryItemAsync(It.IsAny<InventoryItem>()), Times.Once);
    Assert.That(viewModel.ErrorMessage, Is.Null);
}
```

#### Integration Testing
- **Service Integration**: Verify proper service interaction
- **Database Operations**: Validate stored procedure calls
- **Cross-Control Communication**: Test inter-control dependencies
- **Error Handling**: Verify proper error propagation and handling

#### UI Testing
- **Keyboard Navigation**: Complete keyboard accessibility
- **Screen Reader Compatibility**: Proper ARIA and automation properties
- **Theme Compatibility**: Correct appearance across all themes
- **Responsive Behavior**: Proper layout at different screen sizes

## ?? Documentation Standards

### Control Documentation Requirements
Each control must have complete `.instructions.md` documentation including:

1. **Purpose and Context**: What the control does and where it's used
2. **Visual Design**: Layout, appearance, and user interface elements
3. **Component Structure**: Hierarchical breakdown of all UI elements
4. **Data Flow**: Input/output and data binding specifications
5. **Business Logic**: Integration with services and business rules
6. **User Interactions**: Event handling and user workflow
7. **Error Handling**: Validation rules and error display patterns
8. **Integration Points**: Dependencies and relationships with other components

### Code Example Standards
Documentation includes practical implementation examples:
- **AXAML Markup**: Complete view implementation
- **ViewModel Code**: ReactiveUI patterns and business logic
- **Service Integration**: Proper dependency injection and usage
- **Error Handling**: Comprehensive error management patterns

## ?? Development Guidelines

### Creating New Controls
1. **Design Specification**: Create detailed design and requirements document
2. **Documentation First**: Write `.instructions.md` before implementation
3. **Follow Patterns**: Use established architectural and design patterns
4. **Implement Accessibility**: Include keyboard navigation and screen reader support
5. **Test Thoroughly**: Create comprehensive unit and integration tests
6. **Review Process**: Conduct thorough code and design review

### Maintaining Existing Controls
1. **Document Changes**: Update instruction files with any modifications
2. **Validate Integration**: Ensure changes don't break dependent components
3. **Test Regression**: Verify existing functionality continues to work
4. **Update Examples**: Keep code examples current with implementation
5. **Version Control**: Track changes and maintain compatibility

---

*This directory contains the complete specification for all UI controls in the MTM WIP Application, ensuring consistent implementation of modern, accessible, and maintainable user interface components.*