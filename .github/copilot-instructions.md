# GitHub Copilot Instructions for MTM WIP Application Avalonia

**Generate code strictly following the established patterns found in this .NET 8 Avalonia MVVM application. Never introduce patterns not already present in the codebase.**

<details>
<summary><strong>🎯 Technology Version Detection (CRITICAL)</strong></summary>

**BEFORE generating ANY code, scan the codebase to identify these exact versions:**

### **Core Technologies (FIXED VERSIONS)**
- **.NET Version**: 8.0 (`<TargetFramework>net8.0</TargetFramework>`)
- **C# Language Version**: C# 12 with nullable reference types enabled
- **Avalonia UI**: 11.3.4 (Primary UI framework - NOT WPF)
- **MVVM Community Toolkit**: 8.3.2 (Property/Command generation via source generators)
- **MySQL Database**: 9.4.0 (MySql.Data package)
- **Microsoft Extensions**: 9.0.8 (DI, Logging, Configuration, Hosting)

### **Architecture Pattern Detection**
- **Architecture**: MVVM with service-oriented design and comprehensive dependency injection
- **Database Pattern**: Stored procedures ONLY via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- **ViewModel Pattern**: MVVM Community Toolkit with `[ObservableProperty]` and `[RelayCommand]` attributes
- **UI Pattern**: Avalonia UserControl inheritance with minimal code-behind
- **Service Pattern**: Category-based service consolidation in single files
- **Error Pattern**: Centralized error handling via `Services.ErrorHandling.HandleErrorAsync()`

</details>

<details>
<summary><strong>🚨 CRITICAL: Avalonia AXAML Syntax Requirements</strong></summary>

**BEFORE generating ANY AXAML code, follow these critical rules to prevent AVLN2000 compilation errors:**

### **Avalonia-Specific Syntax Rules**
1. **NEVER use `Name` property on Grid definitions** - Use `x:Name` only
2. **Use Avalonia namespace**: `xmlns="https://github.com/avaloniaui"` (NOT WPF namespace)
3. **Grid syntax**: Use `ColumnDefinitions="Auto,*"` attribute form when possible
4. **Control equivalents**: Use `TextBlock` instead of `Label`, `Flyout` instead of `Popup`
5. **Use standard bindings**: `{Binding PropertyName}` with INotifyPropertyChanged

### **Required AXAML Header Structure**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.SomeView">
```

### **MTM Design System Requirements**
- **Primary Color**: Purple (#6a0dad) for buttons and accents
- **Card-based Layout**: Use Border controls with rounded corners and subtle shadows
- **Consistent Spacing**: 8px, 16px, 24px margins and padding
- **Typography**: Use TextBlock with consistent FontSize and FontWeight patterns

</details>

<details>
<summary><strong>📋 Established Codebase Patterns (CRITICAL)</strong></summary>

### **MVVM Community Toolkit Patterns (REQUIRED)**
Based on analysis of `BaseViewModel.cs`, `MainViewViewModel.cs`, and 20+ other ViewModels in the codebase:

```csharp
// ✅ CORRECT: MVVM Community Toolkit pattern (found in all ViewModels)
public partial class InventoryViewModel : BaseViewModel
{
    [ObservableProperty]
    private string partId = string.Empty;

    [ObservableProperty] 
    private bool isLoading;

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            // Business logic here
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Search operation failed");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public InventoryViewModel(ILogger<InventoryViewModel> logger) : base(logger)
    {
    }
}
```

### **Service Organization Pattern (CRITICAL)**
Based on analysis of actual `Services/` folder structure:

```csharp
// ✅ CORRECT: Category-based service consolidation (actual pattern)
// File: Services/ErrorHandling.cs
namespace MTM_WIP_Application_Avalonia.Services
{
    public static class ErrorHandling { /* centralized error handling */ }
    public class ErrorEntry { /* error data model */ }
    public static class ErrorConfiguration { /* error configuration */ }
}

// File: Services/Configuration.cs  
namespace MTM_WIP_Application_Avalonia.Services
{
    public class ConfigurationService : IConfigurationService { /* actual implementation */ }
    public class ApplicationStateService : IApplicationStateService { /* actual implementation */ }
}
```

### **Database Access Pattern (CRITICAL)**
Based on analysis of `Services/Database.cs` and `Helper_Database_StoredProcedure`:

```csharp
// ✅ CORRECT: Stored procedures only (established pattern)
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Operation", operation)
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Get_ByPartIDandOperation",  // Use actual stored procedures
    parameters
);

// Process result.Status and result.Data
if (result.Status == 1)
{
    // Success - process DataTable
    var dataTable = result.Data;
}
```

### **Service Registration Pattern (CRITICAL)**  
Based on analysis of `ServiceCollectionExtensions.cs`:

```csharp
// ✅ CORRECT: Actual service registration pattern
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Use TryAdd methods as established
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        services.TryAddScoped<IDatabaseService, DatabaseService>();
        
        return services;
    }
}
```

### **Transaction Type Logic (MTM-SPECIFIC)**
Based on business domain analysis:

```csharp
// ✅ CORRECT: User intent determines transaction type (not operation numbers)
public string DetermineTransactionType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => "IN",      // User adding inventory
        UserIntent.RemovingStock => "OUT",   // User removing inventory  
        UserIntent.MovingStock => "TRANSFER" // User moving between locations
    };
}
// Operation numbers ("90", "100", "110") are workflow steps, NOT transaction indicators
```

</details>

<details>
<summary><strong>�️ Database Schema and Stored Procedures (CRITICAL)</strong></summary>

### **MySQL Database Structure**
Based on analysis of `Production_Database_Schema.sql` and `Development_Database_Schema.sql`:

**Core Tables:**
- **`inv_inventory`**: Main inventory tracking table
- **`inv_transaction`**: Transaction history (IN/OUT/TRANSFER)
- **`md_part_ids`**: Master data for part definitions
- **`md_locations`**: Master data for location definitions
- **`md_operation_numbers`**: Master data for operation/workflow steps
- **`md_item_types`**: Master data for item type classifications

### **Stored Procedure Patterns (REQUIRED)**
All database operations MUST use stored procedures. Never use direct SQL queries.

**Inventory Operations:**
```sql
-- Adding inventory items
CALL inv_inventory_Add_Item(p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, p_User, p_Notes);

-- Retrieving inventory by part ID
CALL inv_inventory_Get_ByPartID(p_PartID);

-- Retrieving inventory by part ID and operation
CALL inv_inventory_Get_ByPartIDandOperation(p_PartID, o_Operation);

-- Removing inventory items
CALL inv_inventory_Remove_Item(p_PartID, p_Location, p_Operation, p_Quantity, p_ItemType, p_User, p_BatchNumber, p_Notes, @p_Status, @p_ErrorMsg);

-- Transferring inventory between locations
CALL inv_inventory_Transfer_Part(in_BatchNumber, in_PartID, in_Operation, in_NewLocation);
CALL inv_inventory_Transfer_Quantity(in_BatchNumber, in_PartID, in_Operation, in_TransferQuantity, in_OriginalQuantity, in_NewLocation, in_User);
```

**Master Data Operations:**
```sql
-- Part IDs management
CALL md_part_ids_Add_Part(p_ItemNumber, p_Customer, p_Description, p_IssuedBy, p_ItemType);
CALL md_part_ids_Get_All();
CALL md_part_ids_Get_ByItemNumber(p_ItemNumber);
CALL md_part_ids_Update_Part(p_ID, p_ItemNumber, p_Customer, p_Description, p_IssuedBy, p_ItemType);
CALL md_part_ids_Delete_ByItemNumber(p_ItemNumber);

-- Locations management
CALL md_locations_Add_Location(p_Location, p_IssuedBy, p_Building);
CALL md_locations_Get_All();
CALL md_locations_Update_Location(p_OldLocation, p_Location, p_IssuedBy, p_Building);
CALL md_locations_Delete_ByLocation(p_Location);

-- Operation numbers management
CALL md_operation_numbers_Add_Operation(p_Operation, p_IssuedBy);
CALL md_operation_numbers_Get_All();
CALL md_operation_numbers_Update_Operation(p_Operation, p_NewOperation, p_IssuedBy);
CALL md_operation_numbers_Delete_ByOperation(p_Operation);

-- Item types management
CALL md_item_types_Add_ItemType(p_ItemType, p_IssuedBy);
CALL md_item_types_Get_All();
CALL md_item_types_Update_ItemType(p_ID, p_ItemType, p_IssuedBy);
CALL md_item_types_Delete_ByType(p_ItemType);
```

**Transaction Logging:**
```sql
-- Recording transactions
CALL inv_transaction_Add(in_TransactionType, in_PartID, in_BatchNumber, in_FromLocation, in_ToLocation, in_Operation, in_Quantity, in_Notes, in_User, in_ItemType, in_ReceiveDate);
```

**Error Logging:**
```sql
-- Error management
CALL log_error_Add_Error(p_User, p_Severity, p_ErrorType, p_ErrorMessage, p_StackTrace, p_ModuleName, p_MethodName, p_AdditionalInfo, p_MachineName, p_OSVersion, p_AppVersion, p_ErrorTime, @p_Status, @p_ErrorMsg);
CALL log_error_Get_All(@p_Status, @p_ErrorMsg);
CALL log_error_Get_ByUser(p_User, @p_Status, @p_ErrorMsg);
CALL log_error_Delete_All(@p_Status, @p_ErrorMsg);
```

### **Database Development Workflow**
1. **New Procedures**: Add to both `Updated_Stored_Procedures.sql` and `Development_Stored_Procedures.sql`
2. **Testing**: Test on Development database first (`mtm_wip_application_test`)
3. **Production**: Move tested procedures to `Production_Stored_Procedures.sql`
4. **Cleanup**: Remove from `Updated_Stored_Procedures.sql` after deployment

### **Connection String Configuration**
```csharp
// Development database
"ConnectionStrings:Development": "Server=localhost;Database=mtm_wip_application_test;..."

// Production database  
"ConnectionStrings:Production": "Server=localhost;Database=mtm_wip_application;..."
```

</details>

<details>
<summary><strong>� Code Generation Rules</strong></summary>

### When generating UI components:
1. **Always use Avalonia controls** - Not WPF or WinForms equivalents
2. **Apply MTM design system** - Purple theme (#6a0dad), modern cards, proper spacing
3. **Use standard bindings** - `{Binding PropertyName}` with INotifyPropertyChanged
4. **Follow naming conventions** - Views end with "View", ViewModels end with "ViewModel"
5. **Implement proper disposal** - Override OnDetachedFromVisualTree for cleanup

### When generating ViewModels:
1. **Inherit from BaseViewModel** - Use MVVM Community Toolkit patterns with `[ObservableProperty]` and `[RelayCommand]`
2. **Use dependency injection** - Constructor injection for services and logging
3. **Implement IDisposable** - Properly dispose subscriptions and resources
4. **Apply validation** - Use standard .NET validation patterns
5. **Follow established patterns** - Match existing ViewModel implementations

### When generating business logic:
1. **Use established services** - ErrorHandling, Configuration, Navigation, Database
2. **Apply async/await** - For all I/O operations and database calls
3. **Implement logging** - Use ILogger<T> dependency injection
4. **Add error handling** - Use ErrorHandling.HandleErrorAsync for comprehensive error handling
5. **Follow separation** - No UI dependencies in business logic

### When generating services:
1. **📋 Group by category** - Multiple related services in one file (ErrorHandling.cs, Configuration.cs, etc.)
2. **📋 Follow established patterns** - Use existing service structure as template
3. **Follow DI patterns** - Use constructor injection and proper lifetimes
4. **Use stored procedures** - Via Database service and Helper_Database_StoredProcedure
5. **Implement proper error handling** - Use ErrorHandling service for consistent error management

### When generating database operations:
1. **Use stored procedures ONLY** - Never write direct SQL queries
2. **Follow existing patterns** - Use Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
3. **Add new procedures correctly** - Place in both Updated_Stored_Procedures.sql and Development_Stored_Procedures.sql
4. **Handle results properly** - Check Status and process DataTable results
5. **Include proper error handling** - Use output parameters for status and error messages

</details>

<details>
<summary><strong>🔢 MTM-Specific Data Patterns</strong></summary>

### Part Information
```csharp
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;        // "PART001", "ABC-123"
    public string Operation { get; set; } = string.Empty;     // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                         // Integer count only
    public string Location { get; set; } = string.Empty;      // Location identifier
}
```

### Operation Numbers Usage
```csharp
// CORRECT: Operations are workflow steps
var operations = new[] { "90", "100", "110", "120" }; // String numbers representing workflow

// WRONG: Don't use operations to determine transaction type
if (operation == "90") transactionType = "IN"; // This is incorrect logic
```

</details>

<details>
<summary><strong>⚙️ Required Project Setup</strong></summary>

```csharp
using Avalonia;

public static class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
```

### Current Command Infrastructure
```csharp
// Standard ICommand implementations available
 public class AsyncCommand : ICommand
 {
     private readonly Func<Task> _execute;
     private readonly Func<bool>? _canExecute;
     // Implementation in InventoryTabViewModel.cs
 }
 
 public class RelayCommand : ICommand
 {
     private readonly Action _execute;
     private readonly Func<bool>? _canExecute;
     // Implementation in InventoryTabViewModel.cs
 }
```

</details>

<details>
<summary><strong>📚 Documentation and Collapsible Sections (CRITICAL)</strong></summary>

### Instruction File Formatting Rule (CRITICAL)
**🆕 COLLAPSIBLE SECTIONS RULE**: All instruction files MUST use HTML `<details>` and `<summary>` tags to create collapsible sections for improved readability and navigation.

```markdown
<details>
<summary><strong>📋 Section Title</strong></summary>

Section content goes here...

</details>
```

**Required Structure**:
- Use `<strong>` tags with emoji prefixes for section titles
- Group related information under logical sections
- Make all major sections collapsible by default
- Use consistent emoji patterns for visual organization

### When modifying any .md files:
1. **Maintain accuracy** - Ensure all information reflects current MVVM Community Toolkit implementation
2. **Update cross-references** - Update all related links and references to match new patterns
3. **Follow naming conventions** - Use established file naming patterns
4. **Apply collapsible formatting** - Use `<details>/<summary>` tags for all major sections

</details>

<details>
<summary><strong>🚨 CRITICAL: MVVM Community Toolkit Migration Status</strong></summary>

### ✅ **COMPLETED - Services Layer**
**All services use standard .NET patterns without ReactiveUI:**
- ✅ **ErrorHandling.cs**: Comprehensive error handling without ReactiveUI
- ✅ **Configuration.cs**: Configuration and application state with INotifyPropertyChanged
- ✅ **Navigation.cs**: Simple navigation service with standard patterns
- ✅ **Database.cs**: Complete database access with Helper_Database_StoredProcedure

### ✅ **COMPLETED - Working Examples**
- ✅ **InventoryTabViewModel**: Fully converted to MVVM Community Toolkit patterns with [ObservableProperty] and [RelayCommand]
- ✅ **BaseViewModel**: Uses MVVM Community Toolkit ObservableValidator
- ✅ **MainViewViewModel**: Full MVVM Community Toolkit implementation

### 🎯 **MVVM Community Toolkit Patterns to Use**
```csharp
// Use these patterns (actual patterns from codebase):
public partial class SomeViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _property = string.Empty;
    
    [RelayCommand]
    private async Task ExecuteSomeAsync()
    {
        // Implementation here
    }
    
    public SomeViewModel(ILogger<SomeViewModel> logger) : base(logger)
    {
    }
}
```

**NO LONGER USE:**
- ❌ ReactiveObject
- ❌ ReactiveCommand<Unit, Unit>
- ❌ this.RaiseAndSetIfChanged()
- ❌ this.WhenActivated()
- ❌ ReactiveUserControl<T>

</details>
