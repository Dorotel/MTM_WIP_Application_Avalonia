<!-- Copilot: Reading coding-conventions.instruction.md — Coding Conventions -->
# Coding Conventions

## Project Overview
This is an Avalonia UI application for MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using MVVM pattern with Avalonia.ReactiveUI.

### Required Packages
- Avalonia
- Avalonia.Desktop
- Avalonia.Themes.Fluent
- Avalonia.Diagnostics (dev only)
- Avalonia.ReactiveUI

### Program Setup
Ensure ReactiveUI is enabled in Program.cs:
```csharp
using Avalonia;
using Avalonia.ReactiveUI;

public static class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI(); // Enable ReactiveUI integration
}
```

## General C# Conventions
- Use PascalCase for classes, methods, and properties.
- Use camelCase for variables and parameters.
- Use _camelCase for private fields.
- Prefix interfaces with "I".
- Add XML documentation for public members.

## Project Structure
```
/
├── Views/           # Avalonia AXAML views
├── ViewModels/      # ViewModels using ReactiveUI
├── Models/          # Data models and business entities
├── Services/        # Business logic and data access services
└── Resources/       # Styles, themes, and assets
```

## Project-Specific File Naming
- **Views**: `{Name}View.axaml` (e.g., `MainView.axaml`)
- **ViewModels**: `{Name}ViewModel.cs` (e.g., `MainViewModel.cs`)
- **Models**: `{Name}Model.cs` or just `{Name}.cs` for simple models
- **Services**: `{Name}Service.cs` or `I{Name}Service.cs` for interfaces

## UI and Business Logic Separation
- Do **NOT** add any business logic in UI files.  
  Only navigation logic (e.g., navigation between views, dialogs, or windows) is allowed.
- Always include navigation event handlers as specified; other events should be empty stubs with TODO comments.
- **Keep code-behind minimal** - only use for view-specific logic that can't be in ViewModel
- **No business logic** in generated UI code - only structure and bindings

## AXAML View Template
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:YourApp.ViewModels"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="YourApp.Views.{Name}View"
             x:DataType="vm:{Name}ViewModel"
             x:CompileBindings="True">
    <!-- UI Elements here -->
</UserControl>
```

## ReactiveUI and MVVM Patterns

### ViewModel Template
```csharp
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace YourApp.ViewModels;

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
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public {Name}ViewModel()
    {
        // Async command
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.CompletedTask; // TODO: Implement
        });

        // Sync command with CanExecute
        var canSave = this.WhenAnyValue(vm => vm.Title, t => !string.IsNullOrWhiteSpace(t));
        SaveCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Implement
        }, canSave);

        // Error handling pattern
        LoadDataCommand.ThrownExceptions
            .Merge(SaveCommand.ThrownExceptions)
            .Subscribe(ex =>
            {
                // TODO: Log and present user-friendly error
            });
    }
}
```

### Observable Properties
```csharp
private string _title = string.Empty;
public string Title
{
    get => _title;
    set => this.RaiseAndSetIfChanged(ref _title, value);
}
```

### Derived Properties (OAPH - ObservableAsPropertyHelper)
```csharp
private readonly ObservableAsPropertyHelper<string> _fullName;
public string FullName => _fullName.Value;

public SampleViewModel()
{
    _fullName = this.WhenAnyValue(vm => vm.FirstName)
                    .Select(fn => $"Name: {fn}")
                    .ToProperty(this, vm => vm.FullName, initialValue: string.Empty);
}
```

### Commands
```csharp
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public SampleViewModel()
{
    // Async command
    LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        await Task.CompletedTask; // TODO: Implement
    });

    // Sync command with CanExecute
    var canSave = this.WhenAnyValue(vm => vm.Title, t => !string.IsNullOrWhiteSpace(t));
    SaveCommand = ReactiveCommand.Create(() =>
    {
        // TODO: Implement
    }, canSave);

    // Error handling pattern
    LoadDataCommand.ThrownExceptions
        .Merge(SaveCommand.ThrownExceptions)
        .Subscribe(ex =>
        {
            // TODO: Log and present user-friendly error
        });
}
```

### Collections
```csharp
public ObservableCollection<ItemViewModel> Items { get; } = new();
```

## Async/Await Guidelines
- **Use async/await** for any operation that isn't directly UI-related
- Use `ReactiveCommand.CreateFromTask` for async commands
- Always include proper error handling for async operations

## Dependency Injection
- **Use dependency injection** - prepare constructors for DI even if not implementing services
- **Include placeholders** for service injection:
  ```csharp
  // TODO: Inject services
  // private readonly IDataService _dataService;
  ```

## Error Handling Pattern
```csharp
public ReactiveCommand<Unit, Unit> PerformOperationCommand { get; }

public SampleViewModel()
{
    PerformOperationCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        // Operation
        await Task.CompletedTask;
    });

    PerformOperationCommand.ThrownExceptions
        .Subscribe(ex =>
        {
            // TODO: Log to MySQL and file
            // await _errorService.LogErrorAsync(ex);
            // Show user-friendly message
        });
}
```

## Event-Driven Communication Patterns
For inter-component communication:
```csharp
// Events for parent-child communication
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Fire events instead of direct method calls
QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
{
    PartId = button.PartId,
    Operation = button.Operation,
    Quantity = button.Quantity
});
```

## MTM Data Patterns
Operations in MTM are typically numbers, not actions:
- **Part ID**: String (e.g., "PART001")
- **Operation**: String number (e.g., "90", "100", "110")
- **Quantity**: Integer
- **Position**: 1-based indexing for UI display

## Control Bindings
- **Always use compiled bindings** with `x:CompileBindings="True"` and `x:DataType`
- Use `{Binding PropertyName}` for one-way bindings
- Use `{Binding PropertyName, Mode=TwoWay}` for input controls
- Commands: bind to ReactiveCommand (implements ICommand)

## MTM-Specific UI Generation Guidelines

### Component Hierarchy Mapping
When creating controls, follow these patterns:
```xml
<ItemsControl ItemsSource="{Binding QuickButtons}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Rows="10" Columns="1"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate DataType="vm:QuickButtonItemViewModel">
            <Button Classes="quick-button">
                <Button.ContextMenu>
                    <ContextMenu>
                        <MenuItem Header="Edit Button"/>
                        <MenuItem Header="Remove Button"/>
                    </ContextMenu>
                </Button.ContextMenu>
            </Button>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### Business Logic Integration Points
When database operations or business logic are needed:
```csharp
// In ViewModel - Leave as TODO comments
// TODO: Implement database loading
// var dataResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
//     Model_AppVariables.ConnectionString,
//     "sys_last_10_transactions_Get_ByUser",
//     new Dictionary<string, object> { ["User"] = currentUser }
// );
```

### Context Menu Integration
For components with management features, prefer context menus:
```xml
<Button.ContextMenu>
    <ContextMenu>
        <MenuItem Header="Edit Button" Command="{Binding EditCommand}"/>
        <MenuItem Header="Remove Button" Command="{Binding RemoveCommand}"/>
        <Separator/>
        <MenuItem Header="Clear All" Command="{Binding ClearAllCommand}"/>
        <MenuItem Header="Refresh" Command="{Binding RefreshCommand}"/>
    </ContextMenu>
</Button.ContextMenu>
```

### Space Optimization Patterns
When removing UI elements, optimize space usage:
- Use `UniformGrid` for equal distribution
- Implement `VerticalAlignment="Stretch"` for full height usage
- Remove `ScrollViewer` when all items fit in available space
- Increase font sizes and padding when more space is available

### Quick Button Specific Patterns
For button collections that populate other controls:
```csharp
// Simple field population, no tab switching
private async Task ExecuteQuickActionAsync(QuickButtonItemViewModel button)
{
    QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
    {
        PartId = button.PartId,
        Operation = button.Operation, // Just a number
        Quantity = button.Quantity
    });
}
```

### Progress Integration Patterns
When progress tracking is needed:
```csharp
// TODO: Inject services
// private readonly IProgressService _progressService;

// Commands with progress support
LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
{
    // TODO: Show progress during operation
    await LoadLast10TransactionsAsync();
});
```

## Testing Support
- Generate ViewModels with parameterless constructors for design-time support
- Make properties virtual for mocking when needed
- Include `Design.DataContext` in AXAML for design-time data

## Code Generation Rules
1. **Keep code-behind minimal** - only use for view-specific logic that can't be in ViewModel
2. **Use async/await** for any operation that isn't directly UI-related
3. **Use dependency injection** - prepare constructors for DI even if not implementing services
4. **No business logic** in generated UI code - only structure and bindings
5. **Include placeholders** for service injection

## Special Instructions for Creating from MD Files
1. **Focus on structure** - Create the visual hierarchy exactly as described
2. **Map controls** - Convert WinForms controls to Avalonia equivalents
3. **Create bindings** - Set up all properties as observable with proper bindings
4. **Add commands** - Create command stubs for all interactions mentioned
5. **Skip implementation** - Leave business logic as TODO comments
6. **Preserve relationships** - Maintain parent-child control relationships

### Example Conversion
If MD file shows:
```
├── MainForm_TabControl (TabControl)
│   ├── Inventory Tab → Control_InventoryTab
```

Generate:
```xml
<TabControl x:Name="MainTabControl">
    <TabItem Header="Inventory">
        <views:InventoryTabView />
    </TabItem>
</TabControl>
```

And in ViewModel:
```csharp
private int _selectedTabIndex;
public int SelectedTabIndex
{
    get => _selectedTabIndex;
    set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
}

public ReactiveCommand<Unit, Unit> OnTabSelectionChangedCommand { get; }

public SampleViewModel()
{
    OnTabSelectionChangedCommand = ReactiveCommand.Create(() =>
    {
        // TODO: Handle tab change logic
    });
}
```

## Code Quality Rules
1. Generate clean, readable code with proper spacing
2. Add XML comments only where helpful for understanding purpose
3. Use ReactiveUI's reactive programming paradigms (WhenAnyValue, OAPH, etc.)
4. Follow MVVM pattern strictly with ReactiveUI
5. Keep views and ViewModels paired and consistently named
6. This is an Avalonia app, not WPF or WinForms
7. Use Avalonia-specific syntax and controls

## Theme System Preparation
- Use `{DynamicResource ResourceKey}` for colors and brushes
- Prepare for theme switching by avoiding hard-coded colors
- Common theme resources to expect:
  - `PrimaryBrush` (#4B45ED), `SecondaryBrush` (#8345ED), `AccentBrush` (#4B45ED)
  - `MagentaAccentBrush` (#BA45ED), `BlueAccentBrush` (#4574ED), `PinkAccentBrush` (#ED45E7)
  - `LightPurpleBrush` (#B594ED) for disabled states
  - `BackgroundBrush`, `ForegroundBrush`
  - `CardBackgroundBrush`, `BorderBrush`
  - `SidebarBackgroundBrush`, `ContentBackgroundBrush`
  - `StatusBarBackgroundBrush`
  - `PrimaryGradientBrush`, `HeroGradientBrush` (MTM purple gradients)

## Remember
- This is an Avalonia app, not WPF or WinForms
- Use Avalonia-specific syntax and controls
- Follow MVVM pattern strictly with ReactiveUI
- Keep views and ViewModels paired and consistently named
- Generate clean, readable code with proper spacing
- Add XML comments only where helpful for understanding UI purpose
- Follow modern UI patterns with cards, sidebars, and clean layouts
- Use ReactiveUI's reactive programming paradigms (WhenAnyValue, OAPH, etc.)
- Apply the MTM brand gradient and color scheme consistently throughout the application

> For UI structure and event stubs, see [UI Generation Guidelines](ui-generation.instruction.md).
> For complete naming conventions, see [Naming Conventions](naming-conventions.instruction.md)