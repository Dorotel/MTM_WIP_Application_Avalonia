# GitHub Copilot Instructions for MTM WIP Application Avalonia

<!-- COPILOT AUTO-INCLUDE SYSTEM -->
<!-- When this file is referenced via #file:copilot-instructions.md, -->
<!-- automatically include all related instruction files: -->

<!-- Core UI Instructions -->
<!-- #file:.github/UI-Instructions/avalonia-xaml-syntax.instruction.md -->
<!-- #file:.github/UI-Instructions/ui-generation.instruction.md -->
<!-- #file:.github/UI-Instructions/ui-styling.instruction.md -->
<!-- #file:.github/UI-Instructions/ui-mapping.instruction.md -->
<!-- #file:.github/UI-Instructions/suggestion-overlay-implementation.instruction.md -->
<!-- #file:.github/UI-Instructions/suggestion-overlay-integration.instruction.md -->

<!-- Development Instructions -->  
<!-- #file:.github/Development-Instructions/database-patterns.instruction.md -->
<!-- #file:.github/Development-Instructions/stored-procedures.instruction.md -->
<!-- #file:.github/Development-Instructions/errorhandler.instruction.md -->
<!-- #file:.github/Development-Instructions/githubworkflow.instruction.md -->
<!-- #file:.github/Development-Instructions/templates-documentation.instruction.md -->

<!-- Core Instructions -->
<!-- #file:.github/Core-Instructions/dependency-injection.instruction.md -->
<!-- #file:.github/Core-Instructions/naming.conventions.instruction.md -->

<!-- New Template and Pattern Files -->
<!-- #file:.github/copilot/templates/mtm-feature-request.md -->
<!-- #file:.github/copilot/templates/mtm-ui-component.md -->
<!-- #file:.github/copilot/templates/mtm-viewmodel-creation.md -->
<!-- #file:.github/copilot/templates/mtm-database-operation.md -->
<!-- #file:.github/copilot/templates/mtm-service-implementation.md -->

<!-- Context Files -->
<!-- #file:.github/copilot/context/mtm-business-domain.md -->
<!-- #file:.github/copilot/context/mtm-technology-stack.md -->
<!-- #file:.github/copilot/context/mtm-architecture-patterns.md -->
<!-- #file:.github/copilot/context/mtm-database-procedures.md -->

<!-- Pattern Files -->
<!-- #file:.github/copilot/patterns/mtm-mvvm-community-toolkit.md -->
<!-- #file:.github/copilot/patterns/mtm-stored-procedures-only.md -->
<!-- #file:.github/copilot/patterns/mtm-avalonia-syntax.md -->

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

> **Extended Guidance**: For complete AXAML patterns, see: 
> - avalonia-xaml-syntax.instruction.md
> - ui-generation.instruction.md
> - mtm-avalonia-syntax.md

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
- **Primary Color**: Windows 11 Blue (#0078D4) for buttons and accents
- **Multiple Theme Support**: Use ThemeService for MTM_Blue, MTM_Green, MTM_Red, MTM_Dark themes
- **Card-based Layout**: Use Border controls with rounded corners and subtle shadows
- **Consistent Spacing**: 8px, 16px, 24px margins and padding
- **Typography**: Use TextBlock with consistent FontSize and FontWeight patterns

</details>

<details>
<summary><strong>🏗️ MVVM Community Toolkit Patterns (EXCLUSIVE)</strong></summary>

> **Extended Guidance**: For complete MVVM patterns, see:
> - mtm-mvvm-community-toolkit.md
> - mtm-viewmodel-creation.md

**USE ONLY MVVM Community Toolkit patterns. ReactiveUI is completely removed from this codebase.**

### **ViewModel Pattern (Source Generator Based)**
```csharp
// ✅ CORRECT: MVVM Community Toolkit pattern (found in all ViewModels)
[ObservableObject]
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

    public InventoryViewModel(ILogger<InventoryViewModel> logger, IInventoryService service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
    }
}
```

### **NEVER Use These ReactiveUI Patterns**
- ❌ `ReactiveObject` - Use `[ObservableObject]`
- ❌ `ReactiveCommand<T, R>` - Use `[RelayCommand]`
- ❌ `this.RaiseAndSetIfChanged()` - Use `[ObservableProperty]`
- ❌ `WhenAnyValue()` - Use property change handlers
- ❌ Reactive subscriptions - Use standard event handling

</details>

<details>
<summary><strong>🗄️ Database Access Patterns (STORED PROCEDURES ONLY)</strong></summary>

> **Extended Guidance**: For complete database patterns, see:
> - database-patterns.instruction.md
> - stored-procedures.instruction.md
> - mtm-stored-procedures-only.md
> - mtm-database-procedures.md

**ALL database operations MUST use stored procedures via Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()**

### **Standard Database Operation Pattern**
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

### **45+ Available Stored Procedures**
- **Inventory**: `inv_inventory_Add_Item`, `inv_inventory_Get_ByPartID`, `inv_inventory_Remove_Item`
- **Transactions**: `inv_transaction_Add`, `inv_transaction_Get_History`
- **Master Data**: `md_part_ids_Get_All`, `md_locations_Get_All`, `md_operation_numbers_Get_All`
- **Error Logging**: `log_error_Add_Error`, `log_error_Get_All`

### **NEVER Use Direct SQL**
- ❌ Manual SQL queries
- ❌ String concatenation in SQL
- ❌ Direct MySqlCommand usage

</details>

<details>
<summary><strong>🏭 MTM Manufacturing Business Domain</strong></summary>

> **Extended Guidance**: For complete business domain context, see:
> - mtm-business-domain.md

### **Transaction Type Logic (MTM-SPECIFIC)**
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

### **Core Manufacturing Entities**
```csharp
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;        // "PART001", "ABC-123"
    public string Operation { get; set; } = string.Empty;     // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                         // Integer count only
    public string Location { get; set; } = string.Empty;      // Location identifier
}
```

### **Operation Numbers Usage**
```csharp
// CORRECT: Operations are workflow steps
var operations = new[] { "90", "100", "110", "120" }; // String numbers representing workflow

// WRONG: Don't use operations to determine transaction type
if (operation == "90") transactionType = "IN"; // This is incorrect logic
```

</details>

<details>
<summary><strong>⚙️ Service Layer Architecture</strong></summary>

> **Extended Guidance**: For complete service patterns, see:
> - mtm-architecture-patterns.md
> - mtm-service-implementation.md

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

### **Service Registration Pattern (CRITICAL)**  
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
        services.TryAddScoped<IInventoryService, InventoryService>();
        services.TryAddTransient<InventoryViewModel>();
        
        return services;
    }
}
```

</details>

<details>
<summary><strong>🎨 MTM Design System & UI Patterns</strong></summary>

> **Extended Guidance**: For complete UI patterns, see:
> - ui-generation.instruction.md
> - ui-styling.instruction.md
> - mtm-ui-component.md

### **MTM Purple Theme Implementation**
```xml
<!-- Primary MTM Colors -->
<Button Background="#0078D4"      <!-- Primary Windows 11 blue -->
        Foreground="White"
        Padding="12,8"
        CornerRadius="4" />

<Border Background="#106EBE"      <!-- Secondary blue -->
        BorderBrush="#E0E0E0"
        BorderThickness="1"
        CornerRadius="8" />
```

### **Card-Based Layout System**
```xml
<Border Background="White"
        BorderBrush="#E0E0E0" 
        BorderThickness="1"
        CornerRadius="8"
        Padding="16"
        Margin="8">
    
    <Grid x:Name="CardContent" RowDefinitions="Auto,*">
        <Border Grid.Row="0" Background="#0078D4" CornerRadius="8,8,0,0" Padding="16,8">
            <TextBlock Text="Card Title" Foreground="White" FontWeight="Bold" />
        </Border>
        <StackPanel Grid.Row="1" Margin="16" Spacing="8">
            <!-- Card content -->
        </StackPanel>
    </Grid>
</Border>
```

### **Consistent Spacing System**
- **Small spacing**: 8px margins and padding
- **Medium spacing**: 16px for card padding and form spacing  
- **Large spacing**: 24px for section separation

</details>

<details>
<summary><strong>🔧 Dependency Injection & Configuration</strong></summary>

> **Extended Guidance**: For complete DI patterns, see:
> - dependency-injection.instruction.md

### **Constructor Injection Pattern**
```csharp
public class SomeService : ISomeService
{
    private readonly ILogger<SomeService> _logger;
    private readonly IConfigurationService _configurationService;
    
    public SomeService(
        ILogger<SomeService> logger,
        IConfigurationService configurationService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _logger = logger;
        _configurationService = configurationService;
    }
}
```

### **Service Lifetimes**
- **Singleton**: Theme services, configuration services
- **Scoped**: Database services, business services  
- **Transient**: ViewModels, short-lived services

</details>

<details>
<summary><strong>🚨 Error Handling & Logging</strong></summary>

> **Extended Guidance**: For complete error patterns, see:
> - errorhandler.instruction.md

### **Centralized Error Handling**
```csharp
try
{
    // Operation that might fail
    await SomeService.PerformOperationAsync();
}
catch (Exception ex)
{
    // ALWAYS use centralized error handling
    await Services.ErrorHandling.HandleErrorAsync(ex, "Operation context");
}
```

### **Structured Logging**
```csharp
// Use Microsoft.Extensions.Logging throughout
Logger.LogInformation("Operation started for {PartId}", partId);
Logger.LogWarning("Operation failed with status {Status}", status);
Logger.LogError(ex, "Critical error in {Operation}", operationName);
```

</details>

<details>
<summary><strong>✅ Instruction Loading Verification</strong></summary>

**Before generating code, verify these instruction files are available:**
- [ ] Avalonia AXAML syntax rules (AVLN2000 prevention)
- [ ] Database stored procedure patterns  
- [ ] MVVM Community Toolkit patterns
- [ ] MTM design system guidelines
- [ ] Service organization patterns
- [ ] Manufacturing business domain rules
- [ ] Error handling and logging patterns

**If any are missing, explicitly request them in your prompt.**

**Auto-Include System Status**: This file automatically includes all specialized instruction files when referenced. No manual file inclusion needed.

</details>

<details>
<summary><strong>📚 View Code-Behind Pattern</strong></summary>

### **Minimal Code-Behind Pattern**
All 33 View files follow clean Avalonia architecture without ReactiveUI dependencies:

```csharp
// ✅ Standard Avalonia UserControl pattern
public partial class SomeView : UserControl
{
    public SomeView()
    {
        InitializeComponent();
        // Minimal initialization code only
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Cleanup resources, subscriptions
        base.OnDetachedFromVisualTree(e);
    }
}
```

### **View-ViewModel Connection**
```xml
<!-- DataContext set via dependency injection in parent -->
<UserControl xmlns="https://github.com/avaloniaui"
             x:Class="MTM_WIP_Application_Avalonia.Views.SomeView">
    <!-- No code-behind ViewModel instantiation -->
</UserControl>
```

</details>

<details>
<summary><strong>🔗 Cross-Reference System</strong></summary>

**This instruction system uses an interconnected reference model:**

### **Template Files**
- `mtm-feature-request.md` - Complete feature development template
- `mtm-ui-component.md` - Avalonia UserControl creation template  
- `mtm-viewmodel-creation.md` - MVVM Community Toolkit ViewModel template
- `mtm-database-operation.md` - Stored procedure operation template
- `mtm-service-implementation.md` - Service layer implementation template

### **Context Files**
- `mtm-business-domain.md` - Manufacturing domain knowledge
- `mtm-technology-stack.md` - .NET 8, Avalonia 11.3.4, MySQL specifications
- `mtm-architecture-patterns.md` - MVVM, DI, service organization patterns
- `mtm-database-procedures.md` - Complete catalog of 45+ stored procedures

### **Pattern Files**
- `mtm-mvvm-community-toolkit.md` - Complete MVVM Community Toolkit implementation guide
- `mtm-stored-procedures-only.md` - Database access pattern enforcement
- `mtm-avalonia-syntax.md` - AXAML syntax rules and AVLN2000 prevention

**All files are automatically included when this main instruction file is referenced.**

</details>

---

## 🎯 Quick Reference Summary

**For immediate development guidance:**

1. **ViewModels**: Use `[ObservableObject]` + `[ObservableProperty]` + `[RelayCommand]` patterns only
2. **Database**: Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` with actual stored procedure names  
3. **AXAML**: Use `x:Name` (not `Name`) on Grids, `xmlns="https://github.com/avaloniaui"` namespace
4. **Services**: Category-based consolidation with dependency injection
5. **Colors**: MTM Windows 11 Blue `#0078D4` for primary elements with ThemeService support
6. **Error Handling**: `await Services.ErrorHandling.HandleErrorAsync(ex, context)`
7. **Business Logic**: Transaction types by user intent (IN/OUT/TRANSFER), operations are workflow steps

**This instruction system provides comprehensive guidance for all MTM WIP Application development scenarios while maintaining consistency with established codebase patterns.**