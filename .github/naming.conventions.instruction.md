<!-- Copilot: Reading naming-conventions.instruction.md — Naming Conventions -->

# Naming Conventions

## Project Structure and File Naming

### Avalonia UI Files
- **Views (AXAML):** `{Name}View.axaml` (e.g., `MainView.axaml`, `InventoryView.axaml`)
- **ViewModels:** `{Name}ViewModel.cs` (e.g., `MainViewModel.cs`, `InventoryViewModel.cs`)
- **Models:** `{Name}Model.cs` or just `{Name}.cs` for simple models
- **Services:** `{Name}Service.cs` or `I{Name}Service.cs` for interfaces

### Directory Structure
```
/Views/           # Avalonia AXAML views
/ViewModels/      # ViewModels using ReactiveUI
/Models/          # Data models and business entities
/Services/        # Business logic and data access services
/Resources/       # Styles, themes, and assets
```

## Legacy Naming Patterns (for migration reference)
- **Views:** `[parent]view_[child]` or `[parent]view` (e.g., `mainview`, `mainview_menubar`, `mainview_tab_inventory`)
- **Controls:** `[parent]_[type]_[name]` (e.g., `mainview_usercontrol_inventorytab`, `mainview_splitcontainer_middle`)
- **Menu Items:** `[parent]_[menutype]_[menuname]` (e.g., `mainview_menubar_file_settings`)
- **Status/Progress:** `[parent]_[statustype]_[statusname]` (e.g., `mainview_statusbar_savedstatus`, `mainview_progressbar`)
- **Events:** Use C# event handler naming: `[controlname]_[event]` (e.g., `mainview_menubar_file_settings_click`)
- **Fields:** Use private or internal prefix if needed. For fields: `private`, `internal`, or `public` as appropriate.
- **Consistent Prefix:** Always prefix with the parent view or control (e.g., `mainview_`, `settingsview_`, etc.)
- **Component Containers:** Name as `components` if using dependency containers or service locators.

## ReactiveUI Property Naming

### Observable Properties
```csharp
// Private backing field with underscore prefix
private string _firstName = string.Empty;

// Public property with PascalCase
public string FirstName
{
    get => _firstName;
    set => this.RaiseAndSetIfChanged(ref _firstName, value);
}
```

### Observable Collections
```csharp
// Use PascalCase for public collections
public ObservableCollection<ItemViewModel> Items { get; } = new();
```

### Commands
```csharp
// Commands should end with "Command" suffix
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
public ReactiveCommand<Unit, Unit> SaveCommand { get; }
public ReactiveCommand<string, Unit> NavigateCommand { get; }
```

### ObservableAsPropertyHelper (OAPH)
```csharp
// Private OAPH field with underscore prefix
private readonly ObservableAsPropertyHelper<string> _fullName;

// Public property accessing OAPH Value
public string FullName => _fullName.Value;
```

## Service Naming Conventions

### Service Interfaces
```csharp
// Interface with "I" prefix
public interface IDataService { }
public interface IErrorService { }
public interface IProgressService { }
```

### Service Implementations
```csharp
// Implementation without "I" prefix
public class DataService : IDataService { }
public class ErrorService : IErrorService { }
public class ProgressService : IProgressService { }
```

## MTM Data Patterns

### MTM-Specific Naming
- **Part ID:** `PartId` (string, e.g., "PART001")
- **Operation:** `Operation` (string number, e.g., "90", "100", "110")
- **Quantity:** `Quantity` (integer)
- **Position:** `Position` (1-based indexing for UI display)

### Database Helper Naming
```csharp
// Static helper class naming
Helper_Database_StoredProcedure

// Method naming for database operations
ExecuteDataTableWithStatus()
ExecuteNonQueryWithStatus()
```

### Model Variable Naming
```csharp
// Application variables class
Model_AppVariables.ConnectionString
Model_AppVariables.CurrentUser
```

## Avalonia-Specific Naming

### AXAML Controls
```xml
<!-- Use x:Name with PascalCase for named controls -->
<Button x:Name="SaveButton" />
<TextBox x:Name="PartIdTextBox" />
<DataGrid x:Name="InventoryDataGrid" />
```

### Resource Keys
```xml
<!-- Brush resource naming with descriptive suffix -->
<SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
<SolidColorBrush x:Key="AccentBrush" Color="#4B45ED"/>
<SolidColorBrush x:Key="MagentaAccentBrush" Color="#BA45ED"/>

<!-- Style selector naming -->
<Style Selector="Button.primary">
<Style Selector="Border.card">
<Style Selector="RadioButton.nav-item">
```

### Theme Resource Naming
```xml
<!-- Standard theme resource keys -->
PrimaryBrush, SecondaryBrush, AccentBrush
MagentaAccentBrush, BlueAccentBrush, PinkAccentBrush, LightPurpleBrush
BackgroundBrush, ForegroundBrush, CardBackgroundBrush, BorderBrush
SidebarBackgroundBrush, ContentBackgroundBrush, StatusBarBackgroundBrush
PrimaryGradientBrush, HeroGradientBrush
```

## Event and Handler Naming

### ReactiveUI Event Patterns
```csharp
// Event with descriptive name and EventArgs suffix
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Event args class naming
public class QuickActionExecutedEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
```

### Command Handlers
```csharp
// Async command handler methods
private async Task LoadDataAsync()
private async Task SaveDataAsync()
private async Task ExecuteQuickActionAsync(QuickButtonItemViewModel button)
```

## Class and Interface Naming

### ViewModel Naming
```csharp
// ViewModels inherit from ReactiveObject
public class MainViewModel : ReactiveObject
public class InventoryTabViewModel : ReactiveObject
public class QuickButtonItemViewModel : ReactiveObject
```

### View Naming
```csharp
// Views as UserControl or Window
public partial class MainView : UserControl
public partial class InventoryTabView : UserControl
public partial class MainWindow : Window
```

### Model Naming
```csharp
// Simple data models
public class InventoryItem
public class PartNumber
public class Transaction

// Complex models with "Model" suffix when needed
public class InventorySearchModel
public class ReportConfigurationModel
```

## Style Class Naming

### CSS-Style Classes for Avalonia
```xml
<!-- Use lowercase with hyphens for style classes -->
<Button Classes="primary" />
<Button Classes="secondary" />
<Border Classes="card" />
<RadioButton Classes="nav-item" />
<Button Classes="quick-button" />
```

## Variable and Field Naming

### Local Variables
```csharp
// Use camelCase for local variables
var connectionString = Model_AppVariables.ConnectionString;
var currentUser = Model_AppVariables.CurrentUser;
var dataResult = await LoadDataAsync();
```

### Constants
```csharp
// Use PascalCase for constants
public const int MaxQuickButtons = 10;
public const string DefaultConnectionString = "";
```

### Private Fields
```csharp
// Use underscore prefix for private fields
private readonly IDataService _dataService;
private readonly IErrorService _errorService;
private string _selectedPartId = string.Empty;
```

## Method Naming

### Async Methods
```csharp
// Always suffix async methods with "Async"
private async Task LoadDataAsync()
private async Task SaveInventoryAsync()
private async Task ExecuteOperationAsync()
```

### Event Handlers
```csharp
// Use descriptive names for event handlers
private void OnQuickActionExecuted(object? sender, QuickActionExecutedEventArgs e)
private void OnTabSelectionChanged()
private void OnDataLoaded()
```

### Command Execution Methods
```csharp
// Use Execute prefix for command methods
private void ExecuteLoadData()
private void ExecuteSave()
private async Task ExecuteQuickActionAsync(QuickButtonItemViewModel button)
```

> For details on control/event layout and UI mapping, see [ui-generation.instruction.md](ui-generation.instruction.md) and [ui-mapping.instruction.md](ui-mapping.instruction.md)