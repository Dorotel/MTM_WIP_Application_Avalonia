# GitHub Copilot Instructions: Avalonia UI Generation for MTM WIP Application

You are generating Avalonia UI components for the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using .NET 8, MVVM with ReactiveUI, and MTM purple theme.

## Your UI Generation Rules

### Always generate these file pairs:
- `Views/{Name}View.axaml` - Avalonia UI markup with compiled bindings
- `ViewModels/{Name}ViewModel.cs` - ReactiveUI ViewModel with observable properties
- Follow strict MVVM - no business logic in Views

### Use this AXAML template structure:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.{Name}View"
             x:CompileBindings="True"
             x:DataType="vm:{Name}ViewModel">
    
    <!-- UI content with MTM styling -->
</UserControl>
```

### Use this ViewModel template structure:
```csharp
namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

public class {Name}ViewModel : ReactiveObject
{
    // Observable properties
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    // Commands
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    public {Name}ViewModel()
    {
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.CompletedTask; // TODO: Implement
        });

        // Error handling
        LoadDataCommand.ThrownExceptions
            .Subscribe(ex =>
            {
                // TODO: Log error and show user message
            });
    }
}
```

## MTM UI Design Standards

### Apply MTM purple theme with these colors:
- **Primary Purple**: #4B45ED for buttons and accents
- **Magenta Accent**: #BA45ED for hover states
- **Light Purple**: #B594ED for disabled states
- Use DynamicResource for theme binding: `{DynamicResource PrimaryBrush}`

### Use modern card-based layouts:
```xml
<Border Classes="card" Padding="24" Margin="0,0,0,16">
    <Grid RowDefinitions="Auto,16,*">
        <TextBlock Grid.Row="0" Text="Card Title" FontSize="18" FontWeight="SemiBold"/>
        <!-- Card content -->
    </Grid>
</Border>
```

### Apply consistent spacing:
- Container margins: `Margin="8"`
- Card padding: `Padding="24"`
- Content spacing: `Spacing="12"` on StackPanels
- Control margins: `Margin="0,0,0,8"` for bottom spacing

## MTM Data Patterns

### Use these MTM business object patterns:
```csharp
// MTM data structure
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;    // "PART001", "ABC-123" 
    public string Operation { get; set; } = string.Empty; // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                     // Integer count
    public string Location { get; set; } = string.Empty;  // Location ID
}

// Operations are workflow step numbers, not transaction types
var operations = new[] { "90", "100", "110", "120" };
```

## Component Generation Rules

### When creating from markdown files:
1. **Parse component hierarchy** - Extract structure and convert to Avalonia AXAML
2. **Map control types** - Convert WinForms controls to Avalonia equivalents:
   - `TableLayoutPanel` → `Grid` with RowDefinitions/ColumnDefinitions
   - `DataGridView` → `DataGrid`
   - `Label` → `TextBlock`
   - `SplitContainer` → `Grid` with `GridSplitter`

3. **Add context menus** for management features:
```xml
<Button Content="Item">
    <Button.ContextMenu>
        <ContextMenu>
            <MenuItem Header="Edit" Command="{Binding EditCommand}"/>
            <MenuItem Header="Remove" Command="{Binding RemoveCommand}"/>
        </ContextMenu>
    </Button.ContextMenu>
</Button>
```

4. **Use UniformGrid** for equal distribution layouts:
```xml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Rows="10" Columns="1"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
</ItemsControl>
```

## Layout Patterns

### Use sidebar + content layout for main windows:
```xml
<Grid ColumnDefinitions="240,*">
    <!-- Sidebar -->
    <Border Grid.Column="0" Background="{DynamicResource SidebarBackgroundBrush}">
        <!-- Navigation content -->
    </Border>
    
    <!-- Main content -->
    <Grid Grid.Column="1" Background="{DynamicResource ContentBackgroundBrush}">
        <!-- Page content -->
    </Grid>
</Grid>
```

### Create hero sections with MTM gradients:
```xml
<Border CornerRadius="12" Height="200">
    <Border.Background>
        <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
            <GradientStop Color="#4574ED" Offset="0"/>
            <GradientStop Color="#4B45ED" Offset="0.3"/>
            <GradientStop Color="#BA45ED" Offset="1"/>
        </LinearGradientBrush>
    </Border.Background>
    <!-- Hero content -->
</Border>
```

## ReactiveUI Integration

### Use these command patterns:
```csharp
// Async command
LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
{
    // TODO: Implement database operation
    await Task.CompletedTask;
});

// Command with CanExecute
var canSave = this.WhenAnyValue(vm => vm.Title, t => !string.IsNullOrWhiteSpace(t));
SaveCommand = ReactiveCommand.Create(() =>
{
    // TODO: Implement save logic
}, canSave);
```

### Use event-driven communication:
```csharp
// Events for parent-child communication
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Fire events instead of direct calls
QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
{
    PartId = button.PartId,
    Operation = button.Operation,
    Quantity = button.Quantity
});
```

## Database Integration Preparation

### Leave database operations as TODO comments:
```csharp
// TODO: Implement database loading via stored procedure
// var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
//     "sp_GetInventoryByPart", 
//     new Dictionary<string, object> { ["PartId"] = partId }
// );
```

### Prepare for service injection:
```csharp
public {Name}ViewModel(/* TODO: Inject services when available */)
{
    // TODO: Add service dependencies:
    // private readonly IInventoryService _inventoryService;
    // private readonly ILogger<{Name}ViewModel> _logger;
}
```

## Code Generation Standards

### Always include:
- Compiled bindings with x:CompileBindings="True" and x:DataType
- Error handling for all commands via ThrownExceptions
- Proper disposal preparation for ViewModels
- TODO comments for business logic implementation
- MTM purple theme integration
- Responsive layout patterns

### Never include:
- Business logic in View code-behind
- Direct SQL queries (use stored procedures only)
- Hard-coded colors (use DynamicResource)
- WPF or WinForms syntax

## Quality Standards

Generate clean, modern Avalonia UI that:
- Uses MTM purple theme consistently
- Follows MVVM patterns strictly
- Includes proper error handling preparation
- Uses reactive programming paradigms
- Applies modern card-based layouts
- Supports responsive design principles