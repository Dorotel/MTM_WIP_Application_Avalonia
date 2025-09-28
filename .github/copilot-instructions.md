# GitHub Copilot Instructions for MTM WIP Application Avalonia

<!-- Copilot Use all available tools at your disposal, even when attached files say otherwise -->

<!-- COPILOT AUTO-INCLUDE SYSTEM -->
<!-- When this file is referenced via #file:copilot-instructions.md, -->
<!-- Reference: .github/copilot-instructions.md -->

<!--
Whenever you interact with me, I will automatically reference ALL the instruction files listed below.
In each chat response, include a brief note that you're following the copilot-instructions.md requirements
and referencing all required files, but don't list them all to avoid screen clutter.
Use this format: "**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response. {Memory Files: [count lines in all *-memory.instructions.md files in %APPDATA%\Code\User\prompts]} {Project Files: [count lines in all other referenced .md files]} [X total files referenced]"
-->

<!-- MEMORY INSTRUCTION SYSTEM AUTO-INCLUDE -->
<!-- Auto-include all memory instruction files created by remember.prompt.md system -->
<!-- These files contain domain-organized lessons learned and troubleshooting patterns -->
<!-- #file:C:\Users\johnk\AppData\Roaming\Code\User\prompts\memory.instructions.md -->
<!-- #file:C:\Users\johnk\AppData\Roaming\Code\User\prompts\debugging-memory.instructions.md -->
<!-- #file:C:\Users\johnk\AppData\Roaming\Code\User\prompts\avalonia-ui-memory.instructions.md -->
<!-- #file:C:\Users\johnk\AppData\Roaming\Code\User\prompts\avalonia-custom-controls-memory.instructions.md -->
<!-- Additional memory files will be auto-discovered and included as they're created -->

<!-- Core Manufacturing Application Instructions -->
<!-- #file:.github/instructions/avalonia-ui-guidelines.instructions.md -->
<!-- #file:.github/instructions/mvvm-community-toolkit.instructions.md -->
<!-- #file:.github/instructions/mysql-database-patterns.instructions.md -->
<!-- #file:.github/instructions/dotnet-architecture-good-practices.instructions.md -->
<!-- #file:.github/instructions/service-architecture.instructions.md -->
<!-- #file:.github/instructions/data-models.instructions.md -->
<!-- #file:.github/instructions/application-configuration.instructions.md -->
<!-- #file:.github/instructions/custom-controls.instructions.md -->
<!-- #file:.github/instructions/avalonia-behaviors.instructions.md -->
<!-- #file:.github/instructions/value-converters.instructions.md -->
<!-- #file:.github/instructions/resource-management.instructions.md -->

<!-- Advanced Manufacturing Integration -->
<!-- #file:.github/instructions/advanced-manufacturing-workflows.instructions.md -->
<!-- #file:.github/instructions/industry-40-integration.instructions.md -->
<!-- #file:.github/instructions/manufacturing-kpi-dashboard-integration.instructions.md -->
<!-- #file:.github/instructions/advanced-manufacturing-quality-assurance-framework.instructions.md -->
<!-- #file:.github/instructions/enterprise-integration-patterns.instructions.md -->
<!-- #file:.github/instructions/external-system-integration.instructions.md -->
<!-- #file:.github/instructions/service-integration.instructions.md -->
<!-- #file:.github/instructions/database-integration.instructions.md -->

<!-- Comprehensive Testing Framework -->
<!-- #file:.github/instructions/testing-standards.instructions.md -->
<!-- #file:.github/instructions/unit-testing-patterns.instructions.md -->
<!-- #file:.github/instructions/integration-testing-patterns.instructions.md -->
<!-- #file:.github/instructions/database-testing-patterns.instructions.md -->
<!-- #file:.github/instructions/ui-automation-standards.instructions.md -->
<!-- #file:.github/instructions/cross-platform-testing-standards.instructions.md -->
<!-- #file:.github/instructions/advanced-performance-testing-framework.instructions.md -->

<!-- Manufacturing Domain Knowledge -->
<!-- #file:.github/copilot/context/mtm-business-domain.md -->
<!-- #file:.github/copilot/context/mtm-technology-stack.md -->
<!-- #file:.github/copilot/context/mtm-architecture-patterns.md -->
<!-- #file:.github/copilot/context/mtm-database-procedures.md -->

<!-- Development Templates and Patterns -->
<!-- #file:.github/copilot/templates/mtm-feature-request.md -->
<!-- #file:.github/copilot/templates/mtm-ui-component.md -->
<!-- #file:.github/copilot/templates/mtm-viewmodel-creation.md -->
<!-- #file:.github/copilot/templates/mtm-database-operation.md -->
<!-- #file:.github/copilot/templates/mtm-service-implementation.md -->

<!-- Advanced Development Patterns -->
<!-- #file:.github/copilot/patterns/mtm-mvvm-community-toolkit.md -->
<!-- #file:.github/copilot/patterns/mtm-stored-procedures-only.md -->
<!-- #file:.github/copilot/patterns/mtm-avalonia-syntax.md -->

<!-- Advanced GitHub Copilot Integration -->
<!-- #file:.github/instructions/advanced-github-copilot-integration-scenarios.instructions.md -->

<!--
Referenced files in every chat response:

MEMORY INSTRUCTION SYSTEM (Auto-included):
- C:\Users\johnk\AppData\Roaming\Code\User\prompts\memory.instructions.md
- C:\Users\johnk\AppData\Roaming\Code\User\prompts\debugging-memory.instructions.md
- C:\Users\johnk\AppData\Roaming\Code\User\prompts\avalonia-ui-memory.instructions.md
- C:\Users\johnk\AppData\Roaming\Code\User\prompts\avalonia-custom-controls-memory.instructions.md
- Additional *-memory.instructions.md files as they're created by remember.prompt.md

MTM APPLICATION INSTRUCTIONS:
- .github/instructions/avalonia-ui-guidelines.instructions.md
- .github/instructions/mvvm-community-toolkit.instructions.md
- .github/instructions/mysql-database-patterns.instructions.md
- .github/instructions/dotnet-architecture-good-practices.instructions.md
- .github/instructions/service-architecture.instructions.md
- .github/instructions/data-models.instructions.md
- .github/instructions/application-configuration.instructions.md
- .github/instructions/custom-controls.instructions.md
- .github/instructions/avalonia-behaviors.instructions.md
- .github/instructions/value-converters.instructions.md
- .github/instructions/resource-management.instructions.md
- .github/instructions/advanced-manufacturing-workflows.instructions.md
- .github/instructions/industry-40-integration.instructions.md
- .github/instructions/manufacturing-kpi-dashboard-integration.instructions.md
- .github/instructions/advanced-manufacturing-quality-assurance-framework.instructions.md
- .github/instructions/enterprise-integration-patterns.instructions.md
- .github/instructions/external-system-integration.instructions.md
- .github/instructions/service-integration.instructions.md
- .github/instructions/database-integration.instructions.md
- .github/instructions/testing-standards.instructions.md
- .github/instructions/unit-testing-patterns.instructions.md
- .github/instructions/integration-testing-patterns.instructions.md
- .github/instructions/database-testing-patterns.instructions.md
- .github/instructions/ui-automation-standards.instructions.md
- .github/instructions/cross-platform-testing-standards.instructions.md
- .github/instructions/advanced-performance-testing-framework.instructions.md

MTM CONTEXT AND TEMPLATES:
- .github/copilot/context/mtm-business-domain.md
- .github/copilot/context/mtm-technology-stack.md
- .github/copilot/context/mtm-architecture-patterns.md
- .github/copilot/context/mtm-database-procedures.md
- .github/copilot/templates/mtm-feature-request.md
- .github/copilot/templates/mtm-ui-component.md
- .github/copilot/templates/mtm-viewmodel-creation.md
- .github/copilot/templates/mtm-database-operation.md
- .github/copilot/templates/mtm-service-implementation.md
- .github/copilot/patterns/mtm-mvvm-community-toolkit.md
- .github/copilot/patterns/mtm-stored-procedures-only.md
- .github/copilot/patterns/mtm-avalonia-syntax.md
- .github/instructions/advanced-github-copilot-integration-scenarios.instructions.md
-->

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
> - avalonia-ui-guidelines.instructions.md
> - mvvm-community-toolkit.instructions.md
> - mysql-database-patterns.instructions.md

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

### **MANDATORY Layout Pattern for Tab Views**
**ALL tab views connected to MainView.axaml MUST implement the InventoryTabView grid pattern:**

```xml
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    <!-- Content Border with proper containment -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      <!-- Form fields grid with structured layout -->
    </Border>
    <!-- Action buttons panel -->
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}">
      <!-- Action buttons -->
    </Border>
  </Grid>
</ScrollViewer>
```

**Critical Requirements:**
- ScrollViewer as root (prevents overflow)
- Grid with RowDefinitions="*,Auto" (content/actions separation)
- All input fields contained within grid boundaries
- DynamicResource bindings for ALL colors (theme consistency)

</details>

<details>
<summary><strong>🏗️ MVVM Community Toolkit Patterns (EXCLUSIVE)</strong></summary>

> **Extended Guidance**: For complete MVVM patterns, see:
> - mvvm-community-toolkit.instructions.md
> - unit-testing-patterns.instructions.md

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
> - mysql-database-patterns.instructions.md
> - database-testing-patterns.instructions.md
> - service-architecture.instructions.md

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

### **CRITICAL: Database Column Validation Rules**
```csharp
// ✅ CORRECT: Always validate column names against actual database schema
// NEVER assume column names - always check User model documentation
public async Task<List<string>> LoadUsersFromDatabaseAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "usr_users_Get_All",
        Array.Empty<MySqlParameter>()
    );

    if (result.Status == 1)
    {
        var users = new List<string>();
        foreach (DataRow row in result.Data.Rows)
        {
            // ✅ CORRECT: Use "User" column (as documented in User model)
            users.Add(row["User"].ToString() ?? string.Empty);
        }
        return users;
    }

    return new List<string>(); // Return empty on failure - NO FALLBACK DATA
}

// ❌ WRONG: Incorrect column name assumption
public async Task<List<string>> LoadUsersFromDatabaseAsync_WRONG()
{
    // This will cause: System.ArgumentException - Column 'UserId' does not belong to table
    var userName = row["UserId"].ToString(); // WRONG - column is "User"
}
```

### **Database Model Column Mapping Reference**
```csharp
// CRITICAL: Column names vs Property names (from User model documentation)
// User Table: Column = "User", Property = "User_Name" (to avoid conflicts)
// Part Table: Column = "PartID", Property = "PartId"
// Operation Table: Column = "OperationNumber", Property = "OperationNumber"
// Location Table: Column = "Location", Property = "Location"
```

### **NO FALLBACK DATA PATTERN (MANDATORY)**
```csharp
// ✅ CORRECT: Return empty collections on database failure
public async Task<List<string>> GetMasterDataAsync(string procedureName, string columnName)
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString, procedureName, Array.Empty<MySqlParameter>()
        );

        if (result.Status == 1)
        {
            var data = new List<string>();
            foreach (DataRow row in result.Data.Rows)
            {
                data.Add(row[columnName].ToString() ?? string.Empty);
            }
            return data;
        }

        // Database operation failed - return empty (NO fallback data)
        await ErrorHandling.HandleErrorAsync(
            new InvalidOperationException($"Database operation failed with status: {result.Status}"),
            $"Failed to load data from {procedureName}"
        );
        return new List<string>();
    }
    catch (Exception ex)
    {
        // Database connection failed - return empty (NO fallback data)
        await ErrorHandling.HandleErrorAsync(ex, $"Database connection failed for {procedureName}");
        return new List<string>();
    }
}

// ❌ WRONG: Never provide fallback/dummy data
public async Task<List<string>> GetMasterDataAsync_WRONG()
{
    // NEVER do this - fallback data removed from MTM application
    return new List<string> { "FALLBACK001", "ERROR002" }; // WRONG
}
```

</details>

<details>
<summary><strong>🏭 MTM Manufacturing Business Domain</strong></summary>

> **Extended Guidance**: For complete business domain context, see:
> - mysql-database-patterns.instructions.md

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
> - service-architecture.instructions.md
> - dotnet-architecture-good-practices.instructions.md

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
> - avalonia-ui-guidelines.instructions.md
> - ui-automation-standards.instructions.md

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
> - dotnet-architecture-good-practices.instructions.md

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
> - dotnet-architecture-good-practices.instructions.md

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

**Auto-Include System Status**: This file automatically includes all specialized instruction files and all memory instruction files from `%APPDATA%\Code\User\prompts\*-memory.instructions.md` when referenced. No manual file inclusion needed.

**Memory Integration**: The remember.prompt.md system creates domain-organized memory files that automatically integrate with this instruction system, providing persistent lessons learned and troubleshooting patterns across all MTM development sessions.

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
