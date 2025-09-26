# MTM View Implementation Guide
**Complete Documentation for Implementing Views in the MTM WIP Application**

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Technology Stack & Architecture](#technology-stack--architecture)
3. [Pre-Implementation Requirements](#pre-implementation-requirements)
4. [Step-by-Step Implementation Process](#step-by-step-implementation-process)
5. [File Structure & Naming Conventions](#file-structure--naming-conventions)
6. [AXAML Syntax & Avalonia Guidelines](#axaml-syntax--avalonia-guidelines)
7. [ViewModel Implementation](#viewmodel-implementation)
8. [Service Integration](#service-integration)
9. [Database Integration Patterns](#database-integration-patterns)
10. [UI Design System & Theming](#ui-design-system--theming)
11. [Error Handling & Validation](#error-handling--validation)
12. [Testing Strategies](#testing-strategies)
13. [Common Issues & Troubleshooting](#common-issues--troubleshooting)
14. [Resources & Reference Files](#resources--reference-files)

---

## Overview

This guide provides comprehensive instructions for implementing new views in the MTM WIP (Work-In-Progress) Application, a manufacturing inventory management system. The application follows strict architectural patterns using .NET 8, Avalonia UI, MVVM Community Toolkit, and MySQL database with stored procedures.

### Key Principles
- **Consistency**: Follow established patterns found in the existing codebase
- **Manufacturing Focus**: Understand the manufacturing business domain (parts, operations, inventory)
- **Theme Compliance**: Use MTM theme system with Windows 11 Blue (#0078D4) primary colors
- **Stored Procedures Only**: All database operations must use stored procedures
- **MVVM Community Toolkit**: Use source generator patterns exclusively

---

## Technology Stack & Architecture

### Core Technologies
- **.NET 8.0** with C# 12 and nullable reference types enabled
- **Avalonia UI 11.3.4** (NOT WPF) for cross-platform desktop UI
- **MVVM Community Toolkit 8.3.2** with source generators for properties and commands
- **MySQL 9.4.0** database with MySql.Data package
- **Microsoft Extensions 9.0.8** for dependency injection, logging, and configuration

### Architecture Patterns
- **MVVM with Service-Oriented Design**: Clean separation of concerns
- **Comprehensive Dependency Injection**: Service registration via `ServiceCollectionExtensions`
- **Category-Based Service Consolidation**: Multiple related services in single files
- **Centralized Error Handling**: Via `Services.ErrorHandling.HandleErrorAsync()`
- **Manufacturing Business Domain**: Operation numbers as workflow steps, user intent for transaction types

### Reference Files to Study First
```
📁 Essential Study Files (READ THESE FIRST):
├── .github/copilot-instructions.md (Main instruction file)
├── .github/copilot/templates/mtm-ui-component.md
├── .github/copilot/templates/mtm-viewmodel-creation.md
├── .github/copilot/patterns/mtm-avalonia-syntax.md
├── .github/copilot/patterns/mtm-mvvm-community-toolkit.md
├── .github/instructions/avalonia-ui-guidelines.instructions.md
└── Views/MainForm/InventoryTabView.axaml (Reference implementation)
```

---

## Pre-Implementation Requirements

### 1. Understand the Business Domain
**Manufacturing Context**: The MTM application manages manufacturing inventory with these key concepts:
- **Part IDs**: Unique identifiers for manufactured parts (e.g., "PART001", "ABC-123")
- **Operation Numbers**: Workflow step identifiers ("90", "100", "110", "120") - NOT transaction types
- **Locations**: Physical locations where inventory is stored
- **Transaction Types**: Determined by user intent (IN/OUT/TRANSFER), not operation numbers
- **Quantities**: Integer counts only, no fractional quantities

### 2. Study Existing View Examples
Before implementing, examine these reference views:
```
📁 Reference View Examples:
├── Views/MainForm/InventoryTabView.axaml (PRIMARY REFERENCE)
├── Views/MainForm/RemoveTabView.axaml
├── Views/MainForm/TransferTabView.axaml
├── Views/SettingsForm/ThemeSettingsView.axaml
└── Views/TransactionsForm/TransactionHistoryView.axaml
```

### 3. Review Available Services
Study the service layer structure:
```csharp
📁 Services Directory:
├── ErrorHandling.cs (Centralized error handling - MANDATORY)
├── Configuration.cs (App configuration and state)
├── Database.cs (Database helper utilities)
├── MasterDataService.cs (Part IDs, locations, operations)
├── Navigation.cs (View navigation logic)
├── QuickButtons.cs (Quick action management)
├── ThemeService.cs (UI theme management)
└── More specialized services...
```

### 4. Understand Database Patterns
**CRITICAL**: All database access uses stored procedures only. Study these files:
```
📁 Database Documentation:
├── .github/copilot/context/mtm-database-procedures.md (45+ procedures)
├── .github/copilot/patterns/mtm-stored-procedures-only.md
└── .github/instructions/mysql-database-patterns.instructions.md
```

### 5. Review Project Structure
```
📁 Project Organization:
├── Views/[Area]/ (AXAML UI files)
├── ViewModels/[Area]/ (MVVM Community Toolkit ViewModels)
├── Services/ (Business logic services)
├── Models/ (Data models and shared types)
├── Controls/ (Custom Avalonia controls)
├── Converters/ (Value converters for data binding)
└── Resources/Themes/ (MTM theme system)
```

---

## Step-by-Step Implementation Process

### Phase 1: Planning and Design

#### Step 1: Define the View Purpose
**Create a planning document** answering:
- What business function will this view serve?
- What data needs to be displayed/entered?
- What actions can users perform?
- How does it integrate with existing workflows?

#### Step 2: Identify Required Services
**Determine service dependencies**:
```csharp
// Common service dependencies for views:
- ILogger<ViewModelName> (Required for all ViewModels)
- MasterDataService (For dropdowns: parts, operations, locations)
- DatabaseService or specialized service (For data operations)
- ErrorHandling (For error management - used statically)
- ConfigurationService (For app settings)
- Navigation (For view transitions)
```

#### Step 3: Design the Data Flow
**Map the data flow**:
1. **Input**: What user inputs are needed?
2. **Processing**: What business logic will process the data?
3. **Output**: What results are displayed to the user?
4. **Storage**: What data is persisted to the database?

### Phase 2: File Creation

#### Step 4: Create the File Structure
**Create files in this order**:
```bash
# 1. ViewModel first (defines data structure)
ViewModels/[Area]/[Name]ViewModel.cs

# 2. View AXAML file
Views/[Area]/[Name]View.axaml

# 3. View code-behind
Views/[Area]/[Name]View.axaml.cs

# 4. Service if needed (optional)
Services/[Name]Service.cs

# 5. Models if needed (optional)
Models/[Name]Model.cs
```

#### Step 5: Implement the ViewModel
**Use MVVM Community Toolkit patterns** (see template below):
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.[Area];

[ObservableObject]
public partial class [Name]ViewModel : BaseViewModel
{
    #region Private Fields
    private readonly IRequiredService _service;
    #endregion

    #region Observable Properties
    [ObservableProperty]
    private string inputValue = string.Empty;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private ObservableCollection<ItemModel> items = new();
    #endregion

    #region Commands
    [RelayCommand]
    private async Task ExecuteAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Implementation
        });
    }
    #endregion

    #region Constructor
    public [Name]ViewModel(
        ILogger<[Name]ViewModel> logger,
        IRequiredService service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
        
        _service = service;
    }
    #endregion
}
```

#### Step 6: Implement the AXAML View
**Follow MTM tab view layout pattern** (MANDATORY for tab views):
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.[Area]"
             x:Class="MTM_WIP_Application_Avalonia.Views.[Area].[Name]View">

<!-- MANDATORY: ScrollViewer root for all tab views -->
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    
    <!-- Content Border -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" 
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      
      <!-- Form fields grid -->
      <Grid x:Name="FormGrid" RowDefinitions="Auto,Auto,*">
        <!-- Form content here -->
      </Grid>
    </Border>
    
    <!-- Action buttons panel -->
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"
            BorderThickness="1" CornerRadius="6" Padding="12">
      <!-- Action buttons here -->
    </Border>
  </Grid>
</ScrollViewer>
</UserControl>
```

#### Step 7: Implement the Code-Behind
**Minimal code-behind pattern**:
```csharp
public partial class [Name]View : UserControl
{
    public [Name]View()
    {
        InitializeComponent();
        // Minimal initialization only
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Cleanup resources
        if (DataContext is IDisposable disposableContext)
        {
            disposableContext.Dispose();
        }
        base.OnDetachedFromVisualTree(e);
    }
}
```

### Phase 3: Service Integration

#### Step 8: Register Services in DI Container
**Add to `ServiceCollectionExtensions.cs`**:
```csharp
public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
{
    // Add your new service and ViewModel
    services.TryAddScoped<I[Name]Service, [Name]Service>();
    services.TryAddTransient<[Name]ViewModel>();
    
    return services;
}
```

#### Step 9: Database Integration
**Use stored procedures ONLY**:
```csharp
public async Task<List<ItemModel>> GetDataAsync()
{
    var parameters = new MySqlParameter[]
    {
        new("p_Parameter1", parameterValue),
        new("p_Parameter2", parameterValue2)
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "actual_stored_procedure_name", // Use real procedure names
        parameters
    );

    if (result.Status == 1)
    {
        var items = new List<ItemModel>();
        foreach (DataRow row in result.Data.Rows)
        {
            items.Add(new ItemModel
            {
                // Use correct column names from database
                PropertyName = row["ActualColumnName"].ToString() ?? string.Empty
            });
        }
        return items;
    }

    return new List<ItemModel>(); // Return empty on failure - NO fallback data
}
```

### Phase 4: Testing and Integration

#### Step 10: Test the Implementation
**Test these aspects**:
1. **Compilation**: No AVLN2000 errors
2. **Data Binding**: Properties update correctly
3. **Commands**: Execute without errors
4. **Database**: Stored procedures work correctly
5. **Error Handling**: Errors are handled gracefully
6. **Theme**: Colors match MTM design system
7. **Layout**: No UI overflow issues

#### Step 11: Integration Testing
**Integration points to verify**:
1. **Service Registration**: ViewModel resolves from DI
2. **Navigation**: View loads in parent container
3. **Event Handling**: User actions work correctly
4. **Data Persistence**: Changes save to database
5. **Error Propagation**: Errors display to user

---

## File Structure & Naming Conventions

### Directory Organization
```
📁 MTM_WIP_Application_Avalonia/
├── Views/
│   ├── MainForm/          # Primary inventory management views
│   ├── SettingsForm/      # Configuration and settings views
│   ├── TransactionsForm/  # Transaction history and reporting
│   └── [NewArea]/         # New functional area
├── ViewModels/
│   ├── MainForm/          # ViewModels for MainForm views
│   ├── SettingsForm/      # ViewModels for settings
│   ├── Shared/            # Shared ViewModels and base classes
│   └── [NewArea]/         # ViewModels for new area
├── Services/              # Business logic services
├── Models/                # Data models and DTOs
├── Controls/              # Custom Avalonia controls
└── Resources/Themes/      # MTM theme system
```

### File Naming Patterns
```csharp
// ViewModels: [Function][Area]ViewModel.cs
InventoryTabViewModel.cs
RemoveTabViewModel.cs
ThemeSettingsViewModel.cs

// Views: [Function][Area]View.axaml
InventoryTabView.axaml
RemoveTabView.axaml
ThemeSettingsView.axaml

// Services: [Category]Service.cs or [Function].cs
MasterDataService.cs    // Category-based
ErrorHandling.cs        // Function-based
Configuration.cs        // Function-based

// Models: [Entity]Model.cs or [Purpose].cs
InventoryItemModel.cs
SessionTransaction.cs
EventArgs.cs
```

### Namespace Conventions
```csharp
// Consistent namespace structure
MTM_WIP_Application_Avalonia.Views.[Area]
MTM_WIP_Application_Avalonia.ViewModels.[Area]
MTM_WIP_Application_Avalonia.Services
MTM_WIP_Application_Avalonia.Models
MTM_WIP_Application_Avalonia.Controls
```

---

## AXAML Syntax & Avalonia Guidelines

### Critical AXAML Rules (AVLN2000 Prevention)

#### **MANDATORY Header Structure**
```xml
<!-- ALWAYS use this header for UserControls -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.[Area]"
             x:Class="MTM_WIP_Application_Avalonia.Views.[Area].[Name]View">

<!-- NEVER use WPF namespace - will cause compilation errors -->
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
```

#### **Grid Definition Rules**
```xml
<!-- ✅ CORRECT: Use x:Name, NOT Name on Grid -->
<Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">
    <!-- Content here -->
</Grid>

<!-- ❌ WRONG: Will cause AVLN2000 compilation error -->
<Grid Name="MainGrid" RowDefinitions="Auto,*">
    <!-- This will fail to compile -->
</Grid>
```

#### **Control Equivalents**
```xml
<!-- Avalonia controls (NOT WPF equivalents) -->
<TextBlock Text="Display text" />       <!-- NOT Label -->
<ComboBox ItemsSource="{Binding Items}" />  <!-- Same as WPF -->
<Button Content="Click me" />                <!-- Same as WPF -->

<!-- Avalonia-specific controls -->
<Flyout>                                <!-- NOT Popup -->
    <Border Background="White" Padding="10">
        <TextBlock Text="Flyout content" />
    </Border>
</Flyout>
```

### MTM Layout Patterns

#### **MANDATORY Tab View Pattern**
**ALL tab views MUST use this pattern**:
```xml
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    
    <!-- Content Border with proper containment -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" 
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      
      <!-- Form fields grid with structured layout -->
      <Grid x:Name="FormGrid" RowDefinitions="Auto,Auto,*">
        <!-- Form content -->
      </Grid>
    </Border>
    
    <!-- Action buttons panel -->
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}">
      <!-- Action buttons -->
    </Border>
  </Grid>
</ScrollViewer>
```

#### **Form Field Pattern**
```xml
<Grid x:Name="FormFieldsGrid" ColumnDefinitions="90,*" RowDefinitions="Auto,Auto,Auto" RowSpacing="12">
    
    <!-- Field row pattern -->
    <TextBlock Grid.Column="0" Grid.Row="0" Text="Part ID:" VerticalAlignment="Center" />
    <TextBox Grid.Column="1" Grid.Row="0" Text="{Binding PartId}" />
    
    <TextBlock Grid.Column="0" Grid.Row="1" Text="Location:" VerticalAlignment="Center" />
    <ComboBox Grid.Column="1" Grid.Row="1" 
              ItemsSource="{Binding Locations}" 
              SelectedItem="{Binding SelectedLocation}" />
</Grid>
```

#### **Button Panel Pattern**
```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8">
    <Button Content="Save Changes"
            Command="{Binding SaveCommand}"
            Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
            Foreground="White"
            Padding="16,8"
            CornerRadius="4" />
    <Button Content="Cancel"
            Command="{Binding CancelCommand}"
            Background="{DynamicResource MTM_Shared_Logic.SecondaryAction}"
            Foreground="White"
            Padding="12,6"
            CornerRadius="4" />
</StackPanel>
```

---

## ViewModel Implementation

### MVVM Community Toolkit Patterns

#### **Base ViewModel Structure**
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace MTM_WIP_Application_Avalonia.ViewModels.[Area];

/// <summary>
/// ViewModel for [describe functionality]
/// </summary>
[ObservableObject]
public partial class [Name]ViewModel : BaseViewModel
{
    #region Private Fields
    
    private readonly I[Service] _service;
    
    #endregion

    #region Observable Properties
    
    [ObservableProperty]
    private string partId = string.Empty;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string? statusMessage;
    
    [ObservableProperty]
    private ObservableCollection<ItemModel> items = new();
    
    #endregion

    #region Commands
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(PartId))
                {
                    StatusMessage = "Part ID is required";
                    return;
                }

                // Perform operation
                var result = await _service.SaveAsync(new SaveRequest
                {
                    PartId = PartId,
                    // Other properties
                });

                if (result.IsSuccess)
                {
                    StatusMessage = "Save completed successfully";
                    // Update UI state
                }
                else
                {
                    StatusMessage = $"Save failed: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Save operation failed");
                StatusMessage = "An error occurred during save";
            }
        });
    }
    
    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    private async Task ConditionalCommandAsync()
    {
        // Implementation
    }
    
    private bool CanExecuteCommand => !string.IsNullOrWhiteSpace(PartId) && !IsLoading;
    
    #endregion

    #region Constructor
    
    public [Name]ViewModel(
        ILogger<[Name]ViewModel> logger,
        I[Service] service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
        
        _service = service;
        
        // Initialize data
        _ = InitializeAsync();
    }
    
    #endregion

    #region Private Methods
    
    private async Task InitializeAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            await LoadDataAsync();
        });
    }
    
    private async Task LoadDataAsync()
    {
        try
        {
            var data = await _service.GetDataAsync();
            
            Items.Clear();
            foreach (var item in data)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load data");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Loading data");
        }
    }
    
    #endregion

    #region Property Change Handlers
    
    partial void OnPartIdChanged(string value)
    {
        // Clear status when user starts typing
        StatusMessage = null;
        
        // Update command states
        ConditionalCommandCommand.NotifyCanExecuteChanged();
    }
    
    #endregion
}
```

#### **Property Validation Patterns**
```csharp
// Simple validation
[ObservableProperty]
[NotifyDataErrorInfo]
[Required(ErrorMessage = "This field is required")]
private string requiredProperty = string.Empty;

// Complex validation
[ObservableProperty]
[NotifyDataErrorInfo]
[StringLength(50, MinimumLength = 3, ErrorMessage = "Length must be 3-50 characters")]
[RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Only uppercase letters, numbers, and hyphens allowed")]
private string partId = string.Empty;
```

#### **Collection Management**
```csharp
[ObservableProperty]
private ObservableCollection<ItemViewModel> items = new();

[RelayCommand]
private void AddItem()
{
    Items.Add(new ItemViewModel { Name = "New Item" });
}

[RelayCommand]
private void RemoveItem(ItemViewModel item)
{
    Items.Remove(item);
}

[RelayCommand]
private void ClearItems()
{
    Items.Clear();
}
```

### **NEVER Use ReactiveUI Patterns**
```csharp
// ❌ WRONG: ReactiveUI patterns (removed from MTM application)
public class BadViewModel : ReactiveObject  // Don't use ReactiveObject
{
    private string _partId = string.Empty;
    public string PartId
    {
        get => _partId;
        set => this.RaiseAndSetIfChanged(ref _partId, value);  // Don't use this
    }
    
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }  // Don't use ReactiveCommand
    
    // Don't use reactive subscriptions
    this.WhenAnyValue(x => x.PartId)
        .Subscribe(partId => { /* logic */ });
}

// ✅ CORRECT: MVVM Community Toolkit patterns
[ObservableObject]
public partial class GoodViewModel : BaseViewModel
{
    [ObservableProperty]  // Use this instead
    private string partId = string.Empty;
    
    [RelayCommand]  // Use this instead
    private async Task SaveAsync() { /* logic */ }
    
    partial void OnPartIdChanged(string value)  // Use this for property changes
    {
        // Handle property change
    }
}
```

---

## Service Integration

### Service Layer Patterns

#### **Service Registration Pattern**
```csharp
// Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Business services
        services.TryAddScoped<I[Name]Service, [Name]Service>();
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddTransient<MasterDataService>();
        
        // ViewModels
        services.TryAddTransient<[Name]ViewModel>();
        
        // Other services...
        
        return services;
    }
}
```

#### **Service Interface Pattern**
```csharp
namespace MTM_WIP_Application_Avalonia.Services;

public interface I[Name]Service
{
    Task<ServiceResult<List<ItemModel>>> GetItemsAsync();
    Task<ServiceResult<bool>> SaveItemAsync(ItemModel item);
    Task<ServiceResult<bool>> DeleteItemAsync(int id);
}

public class [Name]Service : I[Name]Service
{
    private readonly ILogger<[Name]Service> _logger;
    private readonly IConfigurationService _configurationService;
    
    public [Name]Service(
        ILogger<[Name]Service> logger,
        IConfigurationService configurationService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _logger = logger;
        _configurationService = configurationService;
    }
    
    public async Task<ServiceResult<List<ItemModel>>> GetItemsAsync()
    {
        try
        {
            // Implementation using stored procedures
            var connectionString = await _configurationService.GetConnectionStringAsync();
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "sp_GetItems",
                Array.Empty<MySqlParameter>()
            );

            if (result.Status == 1)
            {
                var items = new List<ItemModel>();
                foreach (DataRow row in result.Data.Rows)
                {
                    items.Add(new ItemModel
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString() ?? string.Empty
                    });
                }
                return ServiceResult<List<ItemModel>>.Success(items);
            }

            return ServiceResult<List<ItemModel>>.Failure("Database operation failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving items");
            return ServiceResult<List<ItemModel>>.Failure(ex.Message);
        }
    }
}
```

#### **Service Result Pattern**
```csharp
public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    
    public static ServiceResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };
    
    public static ServiceResult<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
```

---

## Database Integration Patterns

### Stored Procedures Only Pattern

#### **CRITICAL Database Rules**
1. **NEVER use direct SQL queries** - stored procedures only
2. **NEVER use ORM mappings** - use DataTable processing
3. **NEVER provide fallback data** - return empty collections on failure
4. **ALWAYS validate column names** - use actual database column names

#### **Standard Database Operation**
```csharp
public async Task<List<ItemModel>> GetInventoryAsync(string partId, string operation)
{
    try
    {
        var connectionString = await _configurationService.GetConnectionStringAsync();
        
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_Operation", operation)
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            "inv_inventory_Get_ByPartIDandOperation",  // Use actual stored procedure name
            parameters
        );

        if (result.Status == 1)
        {
            var items = new List<ItemModel>();
            foreach (DataRow row in result.Data.Rows)
            {
                items.Add(new ItemModel
                {
                    // ✅ CORRECT: Use actual database column names
                    PartId = row["PartID"].ToString() ?? string.Empty,
                    Operation = row["OperationNumber"].ToString() ?? string.Empty,
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Location = row["Location"].ToString() ?? string.Empty
                });
            }
            return items;
        }

        // Database operation failed - return empty (NO fallback data)
        await Services.ErrorHandling.HandleErrorAsync(
            new InvalidOperationException($"Database operation failed with status: {result.Status}"),
            $"Failed to retrieve inventory for {partId}"
        );
        
        return new List<ItemModel>(); // Return empty on failure
    }
    catch (Exception ex)
    {
        await Services.ErrorHandling.HandleErrorAsync(ex, "Database connection failed");
        return new List<ItemModel>(); // Return empty on failure
    }
}
```

#### **Available Stored Procedures (45+ procedures)**
**Reference**: `.github/copilot/context/mtm-database-procedures.md`

```sql
-- Inventory Operations
inv_inventory_Add_Item
inv_inventory_Get_ByPartID
inv_inventory_Get_ByPartIDandOperation
inv_inventory_Remove_Item
inv_inventory_Update_Quantity

-- Transaction Operations
inv_transaction_Add
inv_transaction_Get_History
inv_transaction_Get_ByDateRange

-- Master Data
md_part_ids_Get_All
md_locations_Get_All
md_operation_numbers_Get_All
md_users_Get_All

-- Error Logging
log_error_Add_Error
log_error_Get_All
log_error_Get_Recent
```

#### **Database Column Validation**
```csharp
// CRITICAL: Always use correct column names
public async Task<List<UserModel>> GetUsersAsync()
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "usr_users_Get_All",
        Array.Empty<MySqlParameter>()
    );

    if (result.Status == 1)
    {
        var users = new List<UserModel>();
        foreach (DataRow row in result.Data.Rows)
        {
            users.Add(new UserModel
            {
                // ✅ CORRECT: Use "User" column name (from database)
                User_Name = row["User"].ToString() ?? string.Empty  // Property name to avoid conflicts
            });
        }
        return users;
    }

    return new List<UserModel>();
}

// ❌ WRONG: Incorrect column name assumption
var userName = row["UserId"].ToString(); // Will cause ArgumentException
```

---

## UI Design System & Theming

### MTM Theme System Integration

#### **Theme Resource Usage (MANDATORY)**
```xml
<!-- Primary Action Buttons -->
<Button Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
        Foreground="White"
        Padding="16,8"
        CornerRadius="4"
        Content="Save Changes" />

<!-- Secondary Action Buttons -->
<Button Background="{DynamicResource MTM_Shared_Logic.SecondaryAction}"
        Foreground="White"
        Padding="12,6"
        CornerRadius="4"
        Content="Cancel" />

<!-- Card-Based Layout -->
<Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
        BorderThickness="1"
        CornerRadius="8"
        Padding="16"
        Margin="8">
    <!-- Card content -->
</Border>

<!-- Form Input Fields -->
<TextBox Text="{Binding PartId}"
         Background="{DynamicResource MTM_Shared_Logic.ContentAreas}"
         BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
         Margin="8,4" />

<!-- Status Messages -->
<Border Background="{DynamicResource MTM_Shared_Logic.SuccessBrush}" 
        IsVisible="{Binding HasSuccess}">
    <TextBlock Text="{Binding SuccessMessage}" Foreground="White" />
</Border>

<Border Background="{DynamicResource MTM_Shared_Logic.ErrorBrush}" 
        IsVisible="{Binding HasError}">
    <TextBlock Text="{Binding ErrorMessage}" Foreground="White" />
</Border>
```

#### **MTM Color Palette**
```xml
<!-- Core MTM Brand Colors -->
Primary Action: #0078D4 (Windows 11 Blue)
Secondary Action: #106EBE (Darker Blue)
Warning: #FFB900 (Amber Warning)
Critical: #D13438 (Red Alert)
Success: #4CAF50 (Green)

<!-- Background Colors -->
Main Background: #FAFAFA (Light Gray)
Card Background: #F3F2F1 (Card Background)
Content Areas: #FFFFFF (Pure White)
Panel Background: Various theme-specific

<!-- Text Colors -->
Heading Text: #323130 (Dark Gray)
Body Text: #605E5C (Medium Gray)
Tertiary Text: #8A8886 (Light Gray)
Interactive Text: #0078D4 (Primary Blue)
```

#### **Consistent Spacing System**
```xml
<!-- Use 8px base unit spacing -->
<StackPanel Spacing="8">         <!-- Small spacing -->
<Border Margin="16">             <!-- Medium spacing -->
<Grid RowSpacing="12">           <!-- Form spacing -->
<StackPanel Spacing="24">        <!-- Large spacing -->

<!-- Standard paddings -->
<Button Padding="16,8" />        <!-- Primary button -->
<Button Padding="12,6" />        <!-- Secondary button -->
<Border Padding="16" />          <!-- Card padding -->
```

#### **Style Classes for Reusability**
```xml
<UserControl.Styles>
    <!-- MTM Card Style -->
    <Style Selector="Border.mtm-card">
        <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
        <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="Margin" Value="8" />
    </Style>
    
    <!-- Primary Button Style -->
    <Style Selector="Button.primary">
        <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
        <Setter Property="Foreground" Value="White" />
        <Setter Property="Padding" Value="16,8" />
        <Setter Property="CornerRadius" Value="4" />
        <Setter Property="FontWeight" Value="SemiBold" />
    </Style>
</UserControl.Styles>

<!-- Usage -->
<Border Classes="mtm-card">
    <StackPanel>
        <TextBlock Text="Card Content" />
        <Button Classes="primary" Content="Primary Action" />
    </StackPanel>
</Border>
```

---

## Error Handling & Validation

### Centralized Error Handling

#### **Standard Error Handling Pattern**
```csharp
try
{
    // Operation that might fail
    var result = await _service.PerformOperationAsync();
    
    if (!result.IsSuccess)
    {
        // Business logic failure
        StatusMessage = $"Operation failed: {result.ErrorMessage}";
        Logger.LogWarning("Operation failed: {Error}", result.ErrorMessage);
    }
}
catch (Exception ex)
{
    // ALWAYS use centralized error handling for exceptions
    await Services.ErrorHandling.HandleErrorAsync(ex, "Operation context");
    StatusMessage = "An error occurred. Please try again.";
}
```

#### **Error Display in AXAML**
```xml
<!-- Error message display -->
<Border Background="{DynamicResource MTM_Shared_Logic.ErrorLightBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
        BorderThickness="2"
        CornerRadius="4"
        Padding="12,8"
        IsVisible="{Binding HasError}">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <TextBlock Text="⚠" FontSize="16" Foreground="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
        <TextBlock Text="{Binding ErrorMessage}" 
                   Foreground="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
                   TextWrapping="Wrap" />
    </StackPanel>
</Border>

<!-- Success message display -->
<Border Background="{DynamicResource MTM_Shared_Logic.SuccessLightBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.SuccessBrush}"
        BorderThickness="2"
        CornerRadius="4"
        Padding="12,8"
        IsVisible="{Binding HasSuccess}">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <TextBlock Text="✓" FontSize="16" Foreground="{DynamicResource MTM_Shared_Logic.SuccessBrush}" />
        <TextBlock Text="{Binding SuccessMessage}" 
                   Foreground="{DynamicResource MTM_Shared_Logic.SuccessBrush}"
                   TextWrapping="Wrap" />
    </StackPanel>
</Border>
```

### Input Validation Patterns

#### **ViewModel Validation**
```csharp
[ObservableProperty]
[NotifyDataErrorInfo]
[Required(ErrorMessage = "Part ID is required")]
[StringLength(20, ErrorMessage = "Part ID must be 20 characters or less")]
private string partId = string.Empty;

[ObservableProperty]
[NotifyDataErrorInfo]
[Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999,999")]
private int quantity = 1;

// Custom validation logic
partial void OnPartIdChanged(string value)
{
    ClearErrors(nameof(PartId));
    
    if (!string.IsNullOrWhiteSpace(value) && !IsValidPartIdFormat(value))
    {
        AddError("Part ID format is invalid", nameof(PartId));
    }
}

private bool IsValidPartIdFormat(string partId)
{
    // Implement business-specific validation
    return Regex.IsMatch(partId, @"^[A-Z0-9\-]+$");
}
```

#### **AXAML Validation Display**
```xml
<TextBox Text="{Binding PartId}">
    <TextBox.Styles>
        <Style Selector="TextBox:error">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
            <Setter Property="BorderThickness" Value="2" />
        </Style>
    </TextBox.Styles>
</TextBox>

<!-- Display validation errors -->
<TextBlock Text="{Binding (Validation.Errors)[0].ErrorContent}"
           IsVisible="{Binding (Validation.HasErrors)}"
           Foreground="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
           FontSize="12"
           Margin="0,4,0,0" />
```

---

## Testing Strategies

### Unit Testing ViewModels

#### **ViewModel Test Structure**
```csharp
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;

namespace MTM_WIP_Application_Avalonia.Tests.ViewModels;

public class [Name]ViewModelTests
{
    private readonly Mock<ILogger<[Name]ViewModel>> _mockLogger;
    private readonly Mock<I[Service]> _mockService;
    private readonly [Name]ViewModel _viewModel;
    
    public [Name]ViewModelTests()
    {
        _mockLogger = new Mock<ILogger<[Name]ViewModel>>();
        _mockService = new Mock<I[Service]>();
        
        _viewModel = new [Name]ViewModel(_mockLogger.Object, _mockService.Object);
    }
    
    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Assert
        Assert.NotNull(_viewModel);
        Assert.Equal(string.Empty, _viewModel.PartId);
        Assert.False(_viewModel.IsLoading);
    }
    
    [Fact]
    public void PartId_SetValue_UpdatesProperty()
    {
        // Arrange
        const string testPartId = "TEST001";
        
        // Act
        _viewModel.PartId = testPartId;
        
        // Assert
        Assert.Equal(testPartId, _viewModel.PartId);
    }
    
    [Fact]
    public async Task SaveCommand_WithValidData_CallsService()
    {
        // Arrange
        _mockService.Setup(s => s.SaveAsync(It.IsAny<SaveRequest>()))
                   .ReturnsAsync(ServiceResult<bool>.Success(true));
        
        _viewModel.PartId = "TEST001";
        
        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);
        
        // Assert
        _mockService.Verify(s => s.SaveAsync(It.IsAny<SaveRequest>()), Times.Once);
        Assert.Contains("success", _viewModel.StatusMessage?.ToLowerInvariant() ?? string.Empty);
    }
}
```

### Integration Testing

#### **View Integration Test**
```csharp
using Avalonia;
using Avalonia.Headless;
using Xunit;

namespace MTM_WIP_Application_Avalonia.Tests.Views;

public class [Name]ViewTests
{
    [Fact]
    public void View_LoadsWithCorrectInitialState()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
                                  .UseHeadless()
                                  .StartWithClassicDesktopLifetime(Array.Empty<string>());
        
        var mockViewModel = new Mock<[Name]ViewModel>();
        var view = new [Name]View { DataContext = mockViewModel.Object };
        
        // Act
        var window = new Window { Content = view };
        window.Show();
        
        // Assert
        var partIdTextBox = view.FindControl<TextBox>("PartIdTextBox");
        Assert.NotNull(partIdTextBox);
        
        var saveButton = view.FindControl<Button>("SaveButton");
        Assert.NotNull(saveButton);
    }
}
```

### Database Testing

#### **Service Database Integration Test**
```csharp
[Fact]
public async Task GetInventoryAsync_WithValidParameters_ReturnsData()
{
    // Arrange
    var service = new InventoryService(_mockLogger.Object, _mockConfigurationService.Object);
    
    // Act
    var result = await service.GetInventoryAsync("TEST001", "100");
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotEmpty(result.Data);
}

[Fact]
public async Task SaveInventoryAsync_WithValidData_SavesSuccessfully()
{
    // Arrange
    var service = new InventoryService(_mockLogger.Object, _mockConfigurationService.Object);
    var inventoryItem = new InventoryItemModel
    {
        PartId = "TEST001",
        Operation = "100",
        Quantity = 5,
        Location = "A01"
    };
    
    // Act
    var result = await service.SaveInventoryAsync(inventoryItem);
    
    // Assert
    Assert.True(result.IsSuccess);
}
```

---

## Common Issues & Troubleshooting

### AVLN2000 Compilation Errors

#### **Problem**: AVLN2000 errors in AXAML
**Cause**: Using incorrect Avalonia syntax
**Solutions**:
```xml
<!-- ❌ WRONG: Using Name instead of x:Name -->
<Grid Name="MainGrid">

<!-- ✅ CORRECT: Use x:Name -->
<Grid x:Name="MainGrid">

<!-- ❌ WRONG: WPF namespace -->
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">

<!-- ✅ CORRECT: Avalonia namespace -->
<UserControl xmlns="https://github.com/avaloniaui">

<!-- ❌ WRONG: WPF controls -->
<Label Content="Text" />
<Popup>

<!-- ✅ CORRECT: Avalonia controls -->
<TextBlock Text="Text" />
<Flyout>
```

### Database Connection Issues

#### **Problem**: "Column does not belong to table" errors
**Cause**: Using incorrect database column names
**Solution**: Always verify column names against actual database schema
```csharp
// ❌ WRONG: Assuming column names
var userName = row["UserId"].ToString(); // May not exist

// ✅ CORRECT: Use documented column names
var userName = row["User"].ToString(); // Verified column name
```

### Dependency Injection Issues

#### **Problem**: Services not resolving in ViewModels
**Cause**: Service not registered in DI container
**Solution**: Add service registration to `ServiceCollectionExtensions.cs`
```csharp
public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
{
    // Add missing service registration
    services.TryAddScoped<IMissingService, MissingService>();
    services.TryAddTransient<MissingViewModel>();
    
    return services;
}
```

### Theme Resource Issues

#### **Problem**: Theme resources not found
**Cause**: Using StaticResource instead of DynamicResource
**Solution**: Always use DynamicResource for theme colors
```xml
<!-- ❌ WRONG: StaticResource for theme colors -->
<Button Background="{StaticResource MTM_Shared_Logic.PrimaryAction}" />

<!-- ✅ CORRECT: DynamicResource for theme colors -->
<Button Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
```

### Layout Issues

#### **Problem**: UI overflow or content not contained
**Cause**: Missing ScrollViewer or incorrect grid structure
**Solution**: Use mandatory tab view layout pattern
```xml
<!-- ✅ CORRECT: ScrollViewer root prevents overflow -->
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto">
    <!-- Content properly contained -->
  </Grid>
</ScrollViewer>
```

### Command Issues

#### **Problem**: Commands not executing
**Cause**: Command conditions not met or binding issues
**Solutions**:
```csharp
// Check CanExecute logic
[RelayCommand(CanExecute = nameof(CanExecuteCommand))]
private async Task ProblematicCommandAsync()
{
    // Implementation
}

private bool CanExecuteCommand => 
    !string.IsNullOrWhiteSpace(RequiredProperty) && !IsLoading;

// Notify commands when dependent properties change
partial void OnRequiredPropertyChanged(string value)
{
    ProblematicCommandCommand.NotifyCanExecuteChanged();
}
```

---

## Resources & Reference Files

### Essential Reading List

#### **Core Instructions**
```
📁 MUST READ FIRST:
├── .github/copilot-instructions.md (Main instruction file)
├── .github/copilot/templates/mtm-ui-component.md
├── .github/copilot/templates/mtm-viewmodel-creation.md
├── .github/copilot/patterns/mtm-avalonia-syntax.md
├── .github/copilot/patterns/mtm-mvvm-community-toolkit.md
└── .github/copilot/patterns/mtm-stored-procedures-only.md
```

#### **Detailed Technical Guides**
```
📁 Technical Deep Dive:
├── .github/instructions/avalonia-ui-guidelines.instructions.md
├── .github/instructions/mvvm-community-toolkit.instructions.md
├── .github/instructions/mysql-database-patterns.instructions.md
├── .github/instructions/service-architecture.instructions.md
├── .github/instructions/data-models.instructions.md
└── .github/instructions/dotnet-architecture-good-practices.instructions.md
```

#### **Context and Domain Knowledge**
```
📁 Business Domain:
├── .github/copilot/context/mtm-business-domain.md
├── .github/copilot/context/mtm-technology-stack.md
├── .github/copilot/context/mtm-architecture-patterns.md
└── .github/copilot/context/mtm-database-procedures.md
```

### Reference Implementation Examples

#### **View Examples to Study**
```
📁 Study These Implementations:
├── Views/MainForm/InventoryTabView.axaml (PRIMARY REFERENCE)
├── Views/MainForm/RemoveTabView.axaml
├── Views/MainForm/TransferTabView.axaml
├── Views/SettingsForm/ThemeSettingsView.axaml
└── Views/TransactionsForm/TransactionHistoryView.axaml
```

#### **ViewModel Examples**
```
📁 ViewModel Patterns:
├── ViewModels/MainForm/InventoryTabViewModel.cs
├── ViewModels/SettingsForm/ThemeSettingsViewModel.cs
├── ViewModels/Shared/BaseViewModel.cs
└── ViewModels/TransactionsForm/TransactionHistoryViewModel.cs
```

#### **Service Examples**
```
📁 Service Patterns:
├── Services/MasterDataService.cs (Master data operations)
├── Services/ErrorHandling.cs (Error management)
├── Services/Configuration.cs (App configuration)
├── Services/ThemeService.cs (UI theming)
└── Services/Database.cs (Database utilities)
```

### External Resources

#### **GitHub Awesome Copilot Repository**
**URL**: https://github.com/github/awesome-copilot
**Key Files**:
- Documentation best practices
- Architecture patterns
- Code generation templates
- Testing strategies

#### **Avalonia UI Documentation**
**URL**: https://docs.avaloniaui.net/
**Focus Areas**:
- AXAML syntax rules
- Data binding patterns
- Control templates
- Styling and theming

#### **MVVM Community Toolkit**
**URL**: https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
**Key Features**:
- Source generator attributes
- Observable properties
- Relay commands
- Validation patterns

---

## Implementation Checklist

### Pre-Implementation ✅
- [ ] Read all core instruction files
- [ ] Understand the manufacturing business domain
- [ ] Study reference implementations
- [ ] Identify required services and dependencies
- [ ] Plan the data flow and user interactions

### File Creation ✅
- [ ] Create ViewModel with MVVM Community Toolkit patterns
- [ ] Create AXAML view with correct Avalonia syntax
- [ ] Create minimal code-behind
- [ ] Register services in DI container
- [ ] Add navigation integration if needed

### Implementation ✅
- [ ] Use MTM theme resources for all colors
- [ ] Implement mandatory tab view layout pattern
- [ ] Use stored procedures only for database operations
- [ ] Implement centralized error handling
- [ ] Add input validation where appropriate
- [ ] Test data binding and command execution

### Testing ✅
- [ ] No AVLN2000 compilation errors
- [ ] UI elements display correctly
- [ ] Data binding works properly
- [ ] Commands execute without errors
- [ ] Error handling displays user-friendly messages
- [ ] Theme consistency maintained
- [ ] No UI overflow issues

### Integration ✅
- [ ] Service registration works correctly
- [ ] Database operations use correct stored procedures
- [ ] Navigation integrates with existing views
- [ ] Error logging functions properly
- [ ] Performance is acceptable

---

## Conclusion

This guide provides comprehensive instructions for implementing views in the MTM WIP Application while maintaining consistency with established patterns. The key to success is following the existing architectural patterns, using the MTM theme system, and adhering to the stored procedures database pattern.

**Remember**: Always study the reference implementations first, use the provided templates, and test thoroughly to ensure your implementation meets the established quality standards.

**For Questions**: Refer to the instruction files in `.github/` folder or study existing implementations in the codebase.

---

**Document Version**: 1.0
**Last Updated**: September 7, 2025
**Created by**: MTM Development Team Documentation System
**Status**: Complete Implementation Guide