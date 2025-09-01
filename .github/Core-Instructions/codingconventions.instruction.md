<!-- Copilot: Reading coding-conventions.instruction.md — Coding Conventions -->

# GitHub Copilot Instructions: Coding Conventions for MTM Avalonia Application

<details>
<summary><strong>📑 Table of Contents</strong></summary>

- [Your Development Rules](#your-development-rules)
- [Service Organization Standards - CRITICAL](#service-organization-standards---critical)
- [ViewModel Template - Always Use This Pattern](#viewmodel-template---always-use-this-pattern)
- [AXAML View Template - Always Use This Pattern](#axaml-view-template---always-use-this-pattern)
- [MTM Data Patterns - Always Follow](#mtm-data-patterns---always-follow)
- [Control Mapping - WinForms to Avalonia](#control-mapping---winforms-to-avalonia)
- [Layout Standards](#layout-standards)
- [Context Menu Pattern - Use for Management Features](#context-menu-pattern---use-for-management-features)
- [Error Handling Pattern](#error-handling-pattern)
- [Dependency Injection Preparation](#dependency-injection-preparation)
- [Modern UI Standards](#modern-ui-standards)
- [Code Generation Rules](#code-generation-rules)
- [Never Do](#never-do)
- [Always Do](#always-do)

</details>

You are developing an Avalonia UI application for MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using .NET 8, MVVM pattern with standard .NET patterns, and dependency injection.


<details>
<summary><strong>🧑‍💻 Your Development Rules</strong></summary>

### Generate Avalonia-specific code using these patterns:
- Use BaseViewModel with INotifyPropertyChanged and SetProperty method
- Apply compiled bindings with x:CompileBindings="True" and x:DataType
- Follow MVVM strictly - no business logic in Views
- Use ICommand implementations (AsyncCommand/RelayCommand) for all user actions
- Apply MTM purple theme (#6a0dad) and modern card layouts

### Required Project Setup:
```csharp
// Program.cs - Standard Avalonia setup
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace();
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


</details>

<details>
<summary><strong>📁 Service Organization Standards - CRITICAL</strong></summary>

### **📋 SERVICE FILE ORGANIZATION RULE**
All service classes of the same category MUST be in the same .cs file. Interfaces must be in the `Services/Interfaces/` folder.

```csharp
// ✅ CORRECT: Multiple related services in one file
// File: Services/UserServices.cs
namespace MTM_Shared_Logic.Services
{
    public class UserService : IUserService
    {
        // Primary user management implementation
    }

    public class UserValidationService : IUserValidationService  
    {
        // User validation implementation
    }

    public class UserAuditService : IUserAuditService
    {
        // User audit implementation
    }
}

// ✅ CORRECT: Interfaces in separate files
// File: Services/Interfaces/IUserService.cs
namespace MTM_Shared_Logic.Services.Interfaces
{
    public interface IUserService
    {
        // Interface definition
    }
}
```

### **❌ NEVER Do This**:
```csharp
// WRONG: One service per file
// File: Services/UserService.cs - Only UserService
// File: Services/UserValidationService.cs - Only UserValidationService 
// File: Services/UserAuditService.cs - Only UserAuditService
```

### **📋 Service Category Guidelines**:
- **UserServices.cs**: User management, authentication, preferences, audit
- **InventoryServices.cs**: Inventory CRUD, validation, reporting
- **TransactionServices.cs**: Transaction processing, history, validation
- **LocationServices.cs**: Location management, validation, hierarchy
- **SystemServices.cs**: Configuration, caching, logging, error handling


</details>

<details>
<summary><strong>🧩 ViewModel Template - Always Use This Pattern</strong></summary>

```csharp
using System.ComponentModel;
using System.Windows.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Commands;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

public class SampleViewModel : BaseViewModel, INotifyPropertyChanged
{
    // Property with INotifyPropertyChanged pattern
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // Command pattern
    public ICommand LoadDataCommand { get; private set; }
    
    public SampleViewModel()
    {
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // Async command with error handling
        LoadDataCommand = new AsyncCommand(ExecuteLoadDataAsync);
    }

    private async Task ExecuteLoadDataAsync()
    {
        try
        {
            IsLoading = true;
            await Task.CompletedTask; // TODO: Implement
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "LoadData", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```


</details>

<details>
<summary><strong>🖼️ AXAML View Template - Always Use This Pattern</strong></summary>

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


</details>

<details>
<summary><strong>📦 MTM Data Patterns - Always Follow</strong></summary>

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


</details>

<details>
<summary><strong>🔀 Control Mapping - WinForms to Avalonia</strong></summary>

When converting from WinForms:
- `Form` → `Window` or `UserControl`
- `TableLayoutPanel` → `Grid` with RowDefinitions/ColumnDefinitions
- `SplitContainer` → `Grid` with `GridSplitter`
- `DataGridView` → `DataGrid`
- `Label` → `TextBlock`
- `MenuStrip` → `Menu` with `MenuItem`


</details>

<details>
<summary><strong>📐 Layout Standards</strong></summary>

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


</details>

<details>
<summary><strong>📋 Context Menu Pattern - Use for Management Features</strong></summary>

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


</details>

<details>
<summary><strong>🚨 Error Handling Pattern</strong></summary>

```csharp
// In command implementation methods
private async Task ExecuteLoadDataAsync()
{
    try
    {
        IsLoading = true;
        // TODO: Implement business logic
        await Task.CompletedTask;
    }
    catch (Exception ex)
    {
        // TODO: Log to database and file via ErrorHandling service
        await ErrorHandling.HandleErrorAsync(ex, "LoadData", Environment.UserName);
        // TODO: Show user-friendly error message
    }
    finally
    {
        IsLoading = false;
    }
}
```


</details>

<details>
<summary><strong>🧩 Dependency Injection Preparation</strong></summary>

Always prepare ViewModels for service injection:
```csharp
public SampleViewModel(/* TODO: Inject services when available */)
{
    // TODO: Add service dependencies:
    // private readonly IInventoryService _inventoryService;
    // private readonly ILogger<SampleViewModel> _logger;
}
```


</details>

<details>
<summary><strong>🎨 Modern UI Standards</strong></summary>

Create modern interfaces with:
- **Cards**: White background, CornerRadius="8", subtle shadows
- **Purple theme**: Use #6a0dad for primary actions and accents
- **Responsive grids**: Use UniformGrid or Grid for responsive layouts
- **Typography**: FontSize="18" for headers, FontWeight="SemiBold"


</details>

<details>
<summary><strong>⚙️ Code Generation Rules</strong></summary>

1. **Always use Avalonia controls** - Never WPF or WinForms patterns
2. **Follow MVVM strictly** - No business logic in Views
3. **Use compiled bindings** - Include x:CompileBindings="True"
4. **Prepare for DI** - Include TODO comments for service injection
5. **Apply MTM styling** - Purple theme and card layouts
6. **Include error handling** - Use try/catch in command implementations
7. **Use async patterns** - AsyncCommand for async operations
8. **📋 Group services by category** - Multiple related services in one file


</details>

<details>
<summary><strong>🚫 Never Do</strong></summary>

- Use WPF or WinForms syntax in Avalonia code
- Put business logic in View code-behind files
- Skip error handling in commands
- Use hard-coded colors (use DynamicResource)
- Register services individually (use AddMTMServices pattern)
- **📋 Create separate files for related services** - Group by category instead


</details>

<details>
<summary><strong>✅ Always Do</strong></summary>

- Use standard .NET MVVM patterns for ViewModels
- Apply MTM data patterns (PartId as string, Operation as string numbers)
- Include proper error handling and logging preparation
- Use modern card-based layouts with proper spacing
- Follow established naming conventions (View/ViewModel pairs)
- **📋 Group related service implementations in category files**
- **📋 Keep service interfaces in separate files for clarity**