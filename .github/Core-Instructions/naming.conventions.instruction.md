<!-- Copilot: Reading naming-conventions.instruction.md — Naming Conventions -->


# Naming Conventions

<details>
<summary><strong>📑 Table of Contents</strong></summary>

- [Project Structure and File Naming](#project-structure-and-file-naming)
- [Core Naming Standards](#core-naming-standards)
- [Legacy Naming Patterns (for migration reference)](#legacy-naming-patterns-for-migration-reference)
- [Standard .NET Property Naming](#standard-net-property-naming)
- [Observable Collections](#observable-collections)
- [Commands](#commands)
- [ObservableAsPropertyHelper (OAPH)](#observableaspropertyhelper-oaph)
- [Service Naming Conventions](#service-naming-conventions)
- [Service Category Naming Guidelines](#service-category-naming-guidelines)
- [MTM Data Patterns](#mtm-data-patterns)
- [Avalonia-Specific Naming](#avalonia-specific-naming)
- [Event and Handler Naming](#event-and-handler-naming)
- [Class and Interface Naming](#class-and-interface-naming)
- [Style Class Naming](#style-class-naming)
- [Variable and Field Naming](#variable-and-field-naming)
- [Method Naming](#method-naming)
- [Command Execution Methods](#command-execution-methods)

</details>


<details>
<summary><strong>📁 Project Structure and File Naming</strong></summary>

### Avalonia UI Files
- **Views (AXAML):** `{Name}View.axaml` (e.g., `MainView.axaml`, `InventoryView.axaml`)
- **ViewModels:** `{Name}ViewModel.cs` (e.g., `MainViewModel.cs`, `InventoryViewModel.cs`)
- **Models:** `{Name}Model.cs` or just `{Name}.cs` for simple models
- **Service Interfaces:** `I{Name}Service.cs` (e.g., `IUserService.cs`, `IInventoryService.cs`)
- **Service Implementations:** `{Category}Services.cs` (e.g., `UserServices.cs`, `InventoryServices.cs`)

### **📋 SERVICE FILE ORGANIZATION RULE - CRITICAL**
All service classes of the same category MUST be in the same .cs file. Interfaces remain in the `Services/Interfaces/` folder.

**✅ CORRECT Service File Naming**:
```
Services/
├── Interfaces/
│   ├── IUserService.cs              # Single interface per file
│   ├── IUserValidationService.cs    # Single interface per file
│   ├── IUserAuditService.cs         # Single interface per file
│   ├── IInventoryService.cs         # Single interface per file
│   └── ITransactionService.cs       # Single interface per file
├── UserServices.cs                  # ALL user-related implementations
├── InventoryServices.cs             # ALL inventory-related implementations
├── TransactionServices.cs           # ALL transaction-related implementations
└── LocationServices.cs              # ALL location-related implementations
```

**❌ INCORRECT Service File Naming**:
```
Services/
├── UserService.cs                   # WRONG: One service per file
├── UserValidationService.cs         # WRONG: Related services separated
├── UserAuditService.cs              # WRONG: Related services separated
```

### **Core Naming Standards**
- **Views**: `{Name}View.axaml` (e.g., `MainView.axaml`)
- **ViewModels**: `{Name}ViewModel.cs` (e.g., `MainViewModel.cs`)
- **Models**: `{Name}Model.cs` or just `{Name}.cs` for simple models
- **Service Interfaces**: `I{Name}Service.cs` (separate files)
- **Service Implementations**: `{Category}Services.cs` (grouped by category)
- **Stored Procedures**: `{module}_{action}_{details}` (e.g., `inv_inventory_Get_ByPartID`)

### Directory Structure
```
/Views/           # Avalonia AXAML views
/ViewModels/      # ViewModels using standard .NET patterns
/Models/          # Data models and business entities
/Services/        # Business logic and data access services
  /Interfaces/    # Service interfaces (separate files)
/Resources/       # Styles, themes, and assets
```


</details>

<details>
<summary><strong>🕰️ Legacy Naming Patterns (for migration reference)</strong></summary>
- **Views:** `[parent]view_[child]` or `[parent]view` (e.g., `mainview`, `mainview_menubar`, `mainview_tab_inventory`)
- **Controls:** `[parent]_[type]_[name]` (e.g., `mainview_usercontrol_inventorytab`, `mainview_splitcontainer_middle`)
- **Menu Items:** `[parent]_[menutype]_[menuname]` (e.g., `mainview_menubar_file_settings`)
- **Status/Progress:** `[parent]_[statustype]_[statusname]` (e.g., `mainview_statusbar_savedstatus`, `mainview_progressbar`)
- **Events:** Use C# event handler naming: `[source]_[event]` (e.g., `mainview_menubar_file_settings_click`)
- **Fields:** Use private or internal prefix if needed. For fields: `private`, `internal`, or `public` as appropriate.
- **Consistent Prefix:** Always prefix with the parent view or control (e.g., `mainview_`, `settingsview_`, etc.)
- **Component Containers:** Name as `components` if using dependency containers or service locators.


</details>

<details>
<summary><strong>🔤 Standard .NET Property Naming</strong></summary>

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
public ICommand<Unit, Unit> LoadDataCommand { get; }
public ICommand<Unit, Unit> SaveCommand { get; }
public ICommand<string, Unit> NavigateCommand { get; }
```

### ObservableAsPropertyHelper (OAPH)
```csharp
// Private OAPH field with underscore prefix
private readonly ObservableAsPropertyHelper<string> _fullName;

// Public property accessing OAPH Value
public string FullName => _fullName.Value;
```


</details>

<details>
<summary><strong>🧩 Service Naming Conventions</strong></summary>

### Service Interfaces
```csharp
// Interface with "I" prefix - separate files in Services/Interfaces/
public interface IUserService { }
public interface IUserValidationService { }
public interface IUserAuditService { }
public interface IInventoryService { }
public interface ITransactionService { }
```

### Service Implementations - Category-Based Files
```csharp
// File: Services/UserServices.cs - ALL user-related services
namespace MTM_Shared_Logic.Services
{
    public class UserService : IUserService { }
    public class UserValidationService : IUserValidationService { }
    public class UserAuditService : IUserAuditService { }
}

// File: Services/InventoryServices.cs - ALL inventory-related services
namespace MTM_Shared_Logic.Services
{
    public class InventoryService : IInventoryService { }
    public class InventoryValidationService : IInventoryValidationService { }
    public class InventoryReportService : IInventoryReportService { }
}
```

### **📋 Service Category Naming Guidelines**:
- **UserServices.cs**: User management, authentication, preferences, audit, validation
- **InventoryServices.cs**: Inventory CRUD, validation, reporting, analysis
- **TransactionServices.cs**: Transaction processing, history, validation, reporting
- **LocationServices.cs**: Location management, validation, hierarchy
- **SystemServices.cs**: Configuration, caching, logging, error handling


</details>

<details>
<summary><strong>📦 MTM Data Patterns</strong></summary>

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


</details>

<details>
<summary><strong>🎨 Avalonia-Specific Naming</strong></summary>

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


</details>

<details>
<summary><strong>🔗 Event and Handler Naming</strong></summary>

### Standard .NET Event Patterns
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


</details>

<details>
<summary><strong>🏷️ Class and Interface Naming</strong></summary>

### ViewModel Naming
```csharp
// ViewModels inherit from BaseViewModel
public class MainViewModel : BaseViewModel
public class InventoryTabViewModel : BaseViewModel
public class QuickButtonItemViewModel : BaseViewModel
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


</details>

<details>
<summary><strong>🎨 Style Class Naming</strong></summary>

### CSS-Style Classes for Avalonia
```xml
<!-- Use lowercase with hyphens for style classes -->
<Button Classes="primary" />
<Button Classes="secondary" />
<Border Classes="card" />
<RadioButton Classes="nav-item" />
<Button Classes="quick-button" />
```


</details>

<details>
<summary><strong>🔢 Variable and Field Naming</strong></summary>

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


</details>

<details>
<summary><strong>🔣 Method Naming</strong></summary>

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

</details>