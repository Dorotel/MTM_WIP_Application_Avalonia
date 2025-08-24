<!-- Copilot: Reading coding-conventions.instruction.md — Coding Conventions -->
# GitHub Copilot Instructions: Coding Conventions for MTM Avalonia Application

You are developing an Avalonia UI application for MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using .NET 8, MVVM pattern with ReactiveUI, and dependency injection.

## Your Development Rules

### Generate Avalonia-specific code using these patterns:
- Use ReactiveObject for ViewModels with RaiseAndSetIfChanged
- Apply compiled bindings with x:CompileBindings="True" and x:DataType
- Follow MVVM strictly - no business logic in Views
- Use ReactiveCommand for all user actions
- Apply MTM purple theme (#6a0dad) and modern card layouts

### Required Project Setup:
```csharp
// Program.cs - ALWAYS include ReactiveUI
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace()
        .UseReactiveUI(); // REQUIRED for ReactiveUI integration
```

### Use these .NET 8 patterns:
```csharp
// File-scoped namespaces
namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

// Nullable reference types enabled
public string Name { get; set; } = string.Empty;
public string? Description { get; set; }

// Record types for data models
public record InventoryItem(string PartId, string Operation, int Quantity);
```

## ViewModel Template - Always Use This Pattern

```csharp
namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

public class SampleViewModel : ReactiveObject
{
    // Observable property pattern
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    // Command pattern
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    
    public SampleViewModel()
    {
        // Async command with error handling
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.CompletedTask; // TODO: Implement
        });

        // Error handling for all commands
        LoadDataCommand.ThrownExceptions
            .Subscribe(ex =>
            {
                // TODO: Log error and show user message
            });
    }
}
```

## AXAML View Template - Always Use This Pattern

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.SampleView"
             x:CompileBindings="True"
             x:DataType="vm:SampleViewModel">
    
    <Grid RowDefinitions="Auto,*" Margin="8">
        <TextBlock Grid.Row="0" Text="{Binding Title}" FontSize="18" FontWeight="SemiBold"/>
        <Button Grid.Row="1" Content="Load Data" Command="{Binding LoadDataCommand}"/>
    </Grid>
</UserControl>
```

## MTM Data Patterns - Always Follow

```csharp
// MTM business objects
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;    // "PART001", "ABC-123"
    public string Operation { get; set; } = string.Empty; // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                     // Integer count only
    public string Location { get; set; } = string.Empty;  // Location identifier
}

// Operations are workflow steps, NOT transaction types
var operations = new[] { "90", "100", "110", "120" }; // String numbers
```

## Control Mapping - WinForms to Avalonia

When converting from WinForms:
- `Form` → `Window` or `UserControl`
- `TableLayoutPanel` → `Grid` with RowDefinitions/ColumnDefinitions
- `SplitContainer` → `Grid` with `GridSplitter`
- `DataGridView` → `DataGrid`
- `Label` → `TextBlock`
- `MenuStrip` → `Menu` with `MenuItem`

## Layout Standards

Apply these spacing and layout rules:
```xml
<!-- Container margins -->
<Grid Margin="8">
    <!-- Content padding -->
    <Border Padding="24" Background="White" CornerRadius="8">
        <!-- Control spacing -->
        <StackPanel Spacing="12">
            <TextBlock FontSize="18" FontWeight="SemiBold"/>
            <TextBox Margin="0,0,0,8"/>
        </StackPanel>
    </Border>
</Grid>
```

## Context Menu Pattern - Use for Management Features

```xml
<Button Content="Item">
    <Button.ContextMenu>
        <ContextMenu>
            <MenuItem Header="Edit" Command="{Binding EditCommand}"/>
            <MenuItem Header="Remove" Command="{Binding RemoveCommand}"/>
            <Separator/>
            <MenuItem Header="Refresh" Command="{Binding RefreshCommand}"/>
        </ContextMenu>
    </Button.ContextMenu>
</Button>
```

## Error Handling Pattern

```csharp
// In ViewModel constructor
LoadDataCommand.ThrownExceptions
    .Merge(SaveCommand.ThrownExceptions)
    .Subscribe(ex =>
    {
        // TODO: Log to MySQL and file via Helper_Database_StoredProcedure
        // TODO: Show user-friendly error message
    });
```

## Dependency Injection Preparation

Always prepare ViewModels for service injection:
```csharp
public SampleViewModel(/* TODO: Inject services when available */)
{
    // TODO: Add service dependencies:
    // private readonly IInventoryService _inventoryService;
    // private readonly ILogger<SampleViewModel> _logger;
}
```

## Modern UI Standards

Create modern interfaces with:
- **Cards**: White background, CornerRadius="8", subtle shadows
- **Purple theme**: Use #6a0dad for primary actions and accents
- **Responsive grids**: Use UniformGrid or Grid for responsive layouts
- **Typography**: FontSize="18" for headers, FontWeight="SemiBold"

## Code Generation Rules

1. **Always use Avalonia controls** - Never WPF or WinForms patterns
2. **Follow MVVM strictly** - No business logic in Views
3. **Use compiled bindings** - Include x:CompileBindings="True"
4. **Prepare for DI** - Include TODO comments for service injection
5. **Apply MTM styling** - Purple theme and card layouts
6. **Include error handling** - Subscribe to ThrownExceptions
7. **Use async patterns** - ReactiveCommand.CreateFromTask for async operations

## Never Do

- Use WPF or WinForms syntax in Avalonia code
- Put business logic in View code-behind files
- Skip error handling in commands
- Use hard-coded colors (use DynamicResource)
- Register services individually (use AddMTMServices pattern)

## Always Do

- Use ReactiveUI patterns for ViewModels
- Apply MTM data patterns (PartId as string, Operation as string numbers)
- Include proper error handling and logging preparation
- Use modern card-based layouts with proper spacing
- Follow established naming conventions (View/ViewModel pairs)