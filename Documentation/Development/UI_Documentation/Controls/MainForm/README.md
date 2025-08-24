# MainForm Controls Directory

This directory contains detailed specifications for all controls that comprise the main application interface of the MTM WIP Application Avalonia.

## ?? Main Interface Controls

### Core Inventory Management

#### `Control_InventoryTab.instructions.md`
**Primary inventory item creation and management interface**
- **Purpose**: Main inventory entry point for adding new items to the system
- **Key Features**: Part selection, operation assignment, location specification, quantity entry
- **Integration**: Quick buttons panel, advanced entry features, validation system
- **Business Logic**: Inventory item creation with proper transaction logging (IN transactions)

#### `Control_QuickButtons.instructions.md`
**Rapid inventory operations with user customization**
- **Purpose**: One-click inventory operations based on user's recent activity
- **Key Features**: Dynamic button generation, context menu management, user customization
- **Integration**: Last 10 transactions display, right-panel toggle, inventory tab coordination
- **Business Logic**: Quick action execution, button position management, user preference storage

#### `Control_AdvancedInventory.instructions.md`
**Extended inventory features and bulk operations**
- **Purpose**: Advanced inventory management capabilities beyond basic entry
- **Key Features**: Bulk operations, advanced search, batch processing, complex validation
- **Integration**: Main inventory tab, reporting features, data export capabilities
- **Business Logic**: Complex inventory operations, batch transaction processing, advanced analytics

### Inventory Operations

#### `Control_TransferTab.instructions.md`
**Inter-location inventory transfers with comprehensive audit trails**
- **Purpose**: Move inventory items between different storage locations
- **Key Features**: Source/destination location selection, quantity management, search capabilities
- **Integration**: DataGridView display, print functionality, progress tracking
- **Business Logic**: Transfer operations (TRANSFER transactions), inventory integrity, audit logging

#### `Control_RemoveTab.instructions.md`
**Inventory removal operations with validation and safeguards**
- **Purpose**: Remove inventory items from the system with proper documentation
- **Key Features**: Item selection, quantity specification, reason codes, confirmation dialogs
- **Integration**: Search functionality, validation system, audit trail generation
- **Business Logic**: Removal operations (OUT transactions), business rule enforcement, history tracking

#### `Control_AdvancedRemove.instructions.md`
**Bulk removal and advanced deletion features**
- **Purpose**: Advanced removal capabilities for complex scenarios
- **Key Features**: Bulk selection, conditional removal, advanced filtering, batch processing
- **Integration**: Main remove tab, reporting integration, administrative oversight
- **Business Logic**: Complex removal scenarios, batch transaction processing, compliance tracking

## ?? Design System Implementation

### MTM Brand Integration
All MainForm controls implement consistent MTM design elements:

#### Color Scheme Application
```xml
<!-- Primary MTM purple palette -->
<Border Background="{DynamicResource PrimaryBrush}" CornerRadius="8">
    <Button Classes="primary" Background="#4B45ED"/>
</Border>

<!-- Accent colors for interactive elements -->
<Button Classes="accent" Background="#BA45ED"
        x:Name="AccentButton"/>

<!-- Hero gradient for prominent sections -->
<Border Background="{DynamicResource HeroGradientBrush}">
    <!-- Gradient from #4574ED to #BA45ED -->
</Border>
```

#### Modern Card-Based Layout
```xml
<Border Classes="card" 
        Padding="24" 
        Margin="0,0,0,16"
        CornerRadius="8"
        BoxShadow="0 2 8 0 #11000000">
    
    <Grid RowDefinitions="Auto,16,Auto,24,*">
        <!-- Card header with icon -->
        <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*">
            <PathIcon Data="{StaticResource InventoryIcon}" 
                      Foreground="{DynamicResource AccentBrush}"/>
            <TextBlock Grid.Column="2" 
                       Text="Inventory Management"
                       FontSize="20" FontWeight="SemiBold"/>
        </Grid>
        
        <!-- Card content -->
        <Grid Grid.Row="4">
            <!-- Form controls and content -->
        </Grid>
    </Grid>
</Border>
```

### Responsive Design Patterns
Controls adapt to different screen sizes and orientations:

#### Flexible Grid Layouts
```xml
<Grid RowDefinitions="Auto,Auto,*,Auto" 
      ColumnDefinitions="*,2*">
    
    <!-- Header spans both columns -->
    <Border Grid.Row="0" Grid.ColumnSpan="2" 
            Classes="section-header"/>
    
    <!-- Form controls in left column -->
    <StackPanel Grid.Row="2" Grid.Column="0" 
                Spacing="8" Margin="8"/>
    
    <!-- Data display in right column -->
    <DataGrid Grid.Row="2" Grid.Column="1" 
              Margin="8"/>
</Grid>
```

#### Adaptive Navigation
```xml
<!-- Tab-based navigation that stacks on smaller screens -->
<TabControl TabStripPlacement="Top">
    <TabItem Header="Inventory">
        <views:InventoryTabView/>
    </TabItem>
    <TabItem Header="Transfer">
        <views:TransferTabView/>
    </TabItem>
    <TabItem Header="Remove">
        <views:RemoveTabView/>
    </TabItem>
</TabControl>
```

## ?? Control Integration Patterns

### Inter-Control Communication
MainForm controls coordinate through event-driven patterns:

#### Quick Button Integration
```csharp
// InventoryTab updates QuickButtons on successful save
public event EventHandler<InventoryItemSavedEventArgs>? InventoryItemSaved;

private async Task SaveInventoryItemAsync()
{
    var result = await _inventoryService.CreateInventoryItemAsync(inventoryItem);
    
    if (result.IsSuccess)
    {
        // Notify QuickButtons to update
        InventoryItemSaved?.Invoke(this, new InventoryItemSavedEventArgs
        {
            PartId = inventoryItem.PartId,
            Operation = inventoryItem.Operation,
            Quantity = inventoryItem.Quantity
        });
    }
}
```

#### Panel Toggle Coordination
```csharp
// Controls coordinate right panel visibility
public ReactiveCommand<Unit, Unit> ToggleRightPanelCommand { get; }

public MainFormViewModel()
{
    ToggleRightPanelCommand = ReactiveCommand.Create(() =>
    {
        IsRightPanelVisible = !IsRightPanelVisible;
        
        // Notify all tabs of panel state change
        PanelVisibilityChanged?.Invoke(this, new PanelVisibilityEventArgs
        {
            IsVisible = IsRightPanelVisible
        });
    });
}
```

### Service Integration Standards
All MainForm controls follow consistent service integration patterns:

#### Dependency Injection Pattern
```csharp
public class InventoryTabViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IUserAndTransactionServices _userService;
    private readonly IApplicationStateService _applicationState;

    public InventoryTabViewModel(
        IInventoryService inventoryService,
        IUserAndTransactionServices userService,
        IApplicationStateService applicationState)
    {
        _inventoryService = inventoryService;
        _userService = userService;
        _applicationState = applicationState;
        
        InitializeCommands();
    }
}
```

#### Database Operation Patterns
```csharp
// All database operations use stored procedures through services
private async Task LoadInventoryDataAsync()
{
    var searchCriteria = new InventorySearchCriteria
    {
        PartId = SelectedPart,
        Operation = SelectedOperation,
        Location = SelectedLocation
    };

    var result = await _inventoryService.SearchInventoryAsync(searchCriteria);
    
    if (result.IsSuccess)
    {
        InventoryItems.Clear();
        InventoryItems.AddRange(result.Data);
    }
    else
    {
        ShowError(result.ErrorMessage);
    }
}
```

## ?? Business Logic Implementation

### Transaction Type Logic
MainForm controls implement correct MTM business rules:

#### User Intent-Based Transaction Types
```csharp
// ? CORRECT: TransactionType determined by user action, not operation number
private async Task ProcessInventoryActionAsync(UserAction action, string operation)
{
    var transactionType = action switch
    {
        UserAction.AddInventory => TransactionType.IN,      // User adding stock
        UserAction.RemoveInventory => TransactionType.OUT,  // User removing stock
        UserAction.TransferInventory => TransactionType.TRANSFER, // User moving stock
        _ => throw new ArgumentException($"Unknown action: {action}")
    };

    var transaction = new InventoryTransaction
    {
        TransactionType = transactionType, // Based on user intent
        Operation = operation, // Just a workflow step identifier
        PartId = SelectedPart,
        Quantity = Quantity,
        Location = SelectedLocation,
        User = _applicationState.CurrentUser,
        TransactionDate = DateTime.Now
    };

    await _inventoryService.ProcessTransactionAsync(transaction);
}
```

#### Operation Number Handling
```csharp
// Operations are workflow step identifiers, not transaction type indicators
private bool ValidateOperation(string operation)
{
    // Operations like "90", "100", "110" are workflow steps
    if (string.IsNullOrWhiteSpace(operation))
        return false;

    // Validate format (typically numeric workflow steps)
    return operation.All(char.IsDigit) && 
           operation.Length <= 10 &&
           int.TryParse(operation, out int operationNumber) &&
           operationNumber > 0;
}

// ? WRONG: Don't determine TransactionType from operation
// private TransactionType GetTransactionTypeFromOperation(string operation)
// {
//     return operation switch
//     {
//         "90" => TransactionType.IN,    // NEVER DO THIS
//         "100" => TransactionType.OUT,  // WRONG!
//         _ => TransactionType.OTHER
//     };
// }
```

### Data Validation Standards
Comprehensive validation across all MainForm controls:

#### Form Validation Pattern
```csharp
// Real-time validation with immediate feedback
private readonly ObservableAsPropertyHelper<bool> _isFormValid;
public bool IsFormValid => _isFormValid.Value;

public InventoryTabViewModel()
{
    // Combine all validation rules
    _isFormValid = this.WhenAnyValue(
            vm => vm.SelectedPart,
            vm => vm.SelectedOperation,
            vm => vm.SelectedLocation,
            vm => vm.Quantity,
            (part, op, loc, qty) => 
                !string.IsNullOrWhiteSpace(part) &&
                !string.IsNullOrWhiteSpace(op) &&
                !string.IsNullOrWhiteSpace(loc) &&
                qty > 0)
        .ToProperty(this, vm => vm.IsFormValid, initialValue: false);

    // Commands respect validation state
    SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync, 
        this.WhenAnyValue(vm => vm.IsFormValid));
}
```

#### Business Rule Validation
```csharp
// Business-specific validation rules
private async Task<ValidationResult> ValidateInventoryOperationAsync(InventoryItem item)
{
    var errors = new List<string>();

    // Validate part exists
    var partExists = await _inventoryService.ValidatePartExistsAsync(item.PartId);
    if (!partExists)
        errors.Add($"Part ID '{item.PartId}' does not exist in the system");

    // Validate location is active
    var locationValid = await _inventoryService.ValidateLocationActiveAsync(item.Location);
    if (!locationValid)
        errors.Add($"Location '{item.Location}' is not active");

    // Validate operation is valid for this part
    var operationValid = await _inventoryService.ValidateOperationForPartAsync(item.PartId, item.Operation);
    if (!operationValid)
        errors.Add($"Operation '{item.Operation}' is not valid for part '{item.PartId}'");

    return new ValidationResult
    {
        IsValid = !errors.Any(),
        Errors = errors
    };
}
```

## ?? User Experience Features

### Progress Tracking and Feedback
MainForm controls provide comprehensive progress feedback:

```csharp
// Progress tracking for long-running operations
private async Task ProcessLargeInventoryBatchAsync(List<InventoryItem> items)
{
    var progressReporter = new Progress<ProgressInfo>(progress =>
    {
        ProgressPercent = progress.PercentComplete;
        StatusMessage = progress.StatusMessage;
    });

    try
    {
        IsProcessing = true;
        
        await _inventoryService.ProcessInventoryBatchAsync(items, progressReporter);
        
        StatusMessage = $"Successfully processed {items.Count} inventory items";
    }
    catch (Exception ex)
    {
        ErrorMessage = $"Batch processing failed: {ex.Message}";
    }
    finally
    {
        IsProcessing = false;
        ProgressPercent = 0;
    }
}
```

### Error Handling and Recovery
Comprehensive error management with user-friendly messaging:

```csharp
// Centralized error handling for all MainForm controls
private void HandleError(Exception ex)
{
    var userMessage = ex switch
    {
        ValidationException validationEx => validationEx.Message,
        DatabaseException => "Database connection issue. Please check your connection and try again.",
        BusinessRuleException businessEx => businessEx.Message,
        TimeoutException => "Operation timed out. Please try again.",
        _ => "An unexpected error occurred. Please contact support if the problem persists."
    };

    ErrorMessage = userMessage;
    HasError = true;

    // Log detailed error for debugging
    _logger.LogError(ex, "Error in {ControlName}: {ErrorMessage}", 
        GetType().Name, ex.Message);
}
```

### Keyboard Shortcuts and Accessibility
Full keyboard support across all MainForm controls:

```xml
<!-- Comprehensive keyboard navigation -->
<Grid>
    <!-- F5 for refresh/reset -->
    <Button Content="Reset" 
            Command="{Binding ResetCommand}"
            HotKey="F5"/>
    
    <!-- Enter for primary action -->
    <Button Content="Save" 
            Command="{Binding SaveCommand}"
            IsDefault="True"/>
    
    <!-- Escape for cancel -->
    <Button Content="Cancel" 
            Command="{Binding CancelCommand}"
            IsCancel="True"/>
    
    <!-- Tab order for logical navigation -->
    <ComboBox TabIndex="1" AutomationProperties.Name="Part ID"/>
    <ComboBox TabIndex="2" AutomationProperties.Name="Operation"/>
    <ComboBox TabIndex="3" AutomationProperties.Name="Location"/>
    <TextBox TabIndex="4" AutomationProperties.Name="Quantity"/>
</Grid>
```

## ?? Development Standards

### Control Creation Guidelines
1. **Business Analysis**: Understand the business process the control supports
2. **UI/UX Design**: Create mockups following MTM design system
3. **Documentation First**: Write complete `.instructions.md` specification
4. **Implementation**: Follow established patterns and architectural guidelines
5. **Testing**: Implement comprehensive unit and integration tests
6. **Integration**: Ensure proper coordination with other MainForm controls
7. **Accessibility**: Implement full keyboard navigation and screen reader support

### Code Quality Standards
- **Separation of Concerns**: Clear division between UI, business logic, and data access
- **Reactive Programming**: Use ReactiveUI patterns for all user interactions
- **Error Handling**: Comprehensive error management with user-friendly messages
- **Performance**: Efficient operations with progress feedback for long-running tasks
- **Maintainability**: Clean, well-documented code following established conventions

---

*This directory contains the core user interface controls that make up the primary application experience of the MTM WIP Application, implementing modern design patterns, comprehensive business logic integration, and exceptional user experience.*