# MTM Troubleshooting Guide

## Overview
This guide provides solutions to common issues encountered during MTM WIP Application development, organized by component type and aligned with Phase 1 architectural patterns.

## MVVM Community Toolkit Issues

### Property Change Notification Issues

#### **Problem: Properties not updating UI**
**Symptoms:**
- UI not reflecting property changes
- No binding errors in output window
- Property values changing in code but not in UI

**Common Causes:**
1. Missing `[ObservableProperty]` attribute
2. Using manual property implementation instead of source generators
3. Property name mismatch between generated property and binding

**Solution:**
```csharp
// ❌ WRONG: Manual property implementation
private string _partId = string.Empty;
public string PartId
{
    get => _partId;
    set => SetProperty(ref _partId, value); // This pattern is deprecated in MTM
}

// ✅ CORRECT: MVVM Community Toolkit pattern
[ObservableProperty]
private string partId = string.Empty;
// Generates public PartId property with change notification
```

**Verification Steps:**
1. Check that class has `[ObservableObject]` attribute
2. Verify `partial` keyword on class declaration
3. Ensure field name follows camelCase convention
4. Check binding syntax in AXAML uses PascalCase property name

#### **Problem: Dependent properties not updating**
**Symptoms:**
- Main property updates but calculated properties don't refresh
- CanExecute not updating when dependent properties change

**Solution:**
```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(CanExecuteTransaction))] // Add this
[NotifyPropertyChangedFor(nameof(IsFormValid))]          // And this
private string partId = string.Empty;

// Dependent properties will now update when PartId changes
public bool CanExecuteTransaction => !IsLoading && !string.IsNullOrWhiteSpace(PartId);
public bool IsFormValid => !string.IsNullOrWhiteSpace(PartId) && PartId.Length >= 3;
```

### Command Issues

#### **Problem: Commands not executing**
**Symptoms:**
- Button clicks have no effect
- Command appears bound correctly
- No exceptions in error logs

**Common Causes:**
1. CanExecute method returns false
2. Command method is not async when it should be
3. Missing `[RelayCommand]` attribute

**Solution:**
```csharp
// ✅ CORRECT: Command with proper CanExecute
[RelayCommand(CanExecute = nameof(CanSaveData))]
private async Task SaveDataAsync()
{
    IsLoading = true;
    try
    {
        // Command implementation
    }
    finally
    {
        IsLoading = false;
    }
}

// Make sure CanExecute method exists and returns appropriate value
private bool CanSaveData => !IsLoading && IsFormValid;
```

**Debugging Steps:**
1. Add logging to CanExecute method to verify it's called
2. Check if IsLoading is stuck in true state
3. Verify command is bound correctly in AXAML
4. Use debugger to check CanExecute return value

#### **Problem: Command CanExecute not updating**
**Symptoms:**
- Button remains disabled even when conditions are met
- CanExecute method returns true but UI not responding

**Solution:**
```csharp
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveDataCommand))] // Add this attribute
private bool isDataValid;

[RelayCommand(CanExecute = nameof(CanSaveData))]
private async Task SaveDataAsync() { /* implementation */ }

private bool CanSaveData => IsDataValid && !IsLoading;

// Alternative: Manual command refresh
private void SomeMethod()
{
    // When complex conditions change, manually refresh
    SaveDataCommand.NotifyCanExecuteChanged();
}
```

## Service Layer Issues

### Dependency Injection Issues

#### **Problem: Service not injected (NullReferenceException)**
**Symptoms:**
- NullReferenceException when using injected service
- Service constructor called but dependency is null

**Common Causes:**
1. Service not registered in DI container
2. Interface not registered with implementation
3. Constructor parameter name mismatch

**Solution:**
```csharp
// 1. Verify service registration in ServiceCollectionExtensions.cs
public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
{
    services.AddScoped<IInventoryService, InventoryService>(); // Add this line
    services.AddScoped<IDatabaseService, DatabaseService>();
    
    return services;
}

// 2. Verify constructor parameter names match
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly IDatabaseService _databaseService; // Names must match constructor parameters
    
    public InventoryService(
        ILogger<InventoryService> logger,        // Parameter name: logger
        IDatabaseService databaseService)        // Parameter name: databaseService
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }
}
```

#### **Problem: Circular dependency exception**
**Symptoms:**
- Application fails to start with circular dependency error
- Services depend on each other directly

**Solution:**
```csharp
// ❌ WRONG: Circular dependency
public class ServiceA : IServiceA
{
    public ServiceA(IServiceB serviceB) { } // ServiceA depends on ServiceB
}

public class ServiceB : IServiceB  
{
    public ServiceB(IServiceA serviceA) { } // ServiceB depends on ServiceA - CIRCULAR!
}

// ✅ CORRECT: Use mediator pattern or extract shared functionality
public class ServiceA : IServiceA
{
    public ServiceA(ISharedService sharedService) { } // Both depend on shared service
}

public class ServiceB : IServiceB
{
    public ServiceB(ISharedService sharedService) { } // Both depend on shared service
}

public class SharedService : ISharedService
{
    // Contains functionality needed by both services
}
```

### Database Access Issues

#### **Problem: Stored procedure not found**
**Symptoms:**
- Exception: "Procedure 'procedure_name' doesn't exist"
- Database operation fails with "Unknown routine" error

**Solution:**
```csharp
// 1. Verify stored procedure exists in database
// 2. Check exact procedure name spelling and case
var parameters = new MySqlParameter[]
{
    new("p_PartId", partId),
    new("p_Operation", operation)
};

// Make sure procedure name matches exactly (case sensitive)
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString, 
    "inv_inventory_Get_ByPartIDandOperation", // Check this name in database
    parameters);
```

**Debugging Steps:**
1. Connect to database and verify procedure exists
2. Check procedure name in database vs. code
3. Verify database user has EXECUTE permissions on procedure
4. Test procedure manually with sample parameters

#### **Problem: Parameter mismatch errors**
**Symptoms:**
- "Incorrect number of arguments for PROCEDURE"
- "Unknown parameter name" errors

**Solution:**
```csharp
// ✅ CORRECT: Parameter names must match stored procedure exactly
var parameters = new MySqlParameter[]
{
    new("p_PartId", partId),           // Parameter name in procedure: p_PartId
    new("p_Operation", operation),     // Parameter name in procedure: p_Operation  
    new("p_Quantity", quantity)        // Parameter name in procedure: p_Quantity
};

// Verify parameter names match stored procedure definition:
// CREATE PROCEDURE inv_inventory_Add_Item(
//     IN p_PartId VARCHAR(50),
//     IN p_Operation VARCHAR(10),
//     IN p_Quantity INT
// )
```

#### **Problem: Database connection timeout**
**Symptoms:**
- "Connection timeout expired" exceptions
- Long delays before database errors

**Solution:**
```csharp
// 1. Check connection string timeout settings
"Server=localhost;Database=mtm_db;Uid=user;Pwd=pass;Connection Timeout=60;Command Timeout=60;"

// 2. For long-running operations, increase timeout
var connectionString = _configuration.GetConnectionString("DefaultConnection");
// Consider adding Connection Timeout=120 for heavy operations

// 3. Optimize stored procedures
// Check query execution plans for slow procedures
// Add appropriate indexes
// Consider pagination for large result sets
```

## Avalonia AXAML Issues

### Binding Issues

#### **Problem: AVLN2000 - Name conflicts**
**Symptoms:**
- Compiler error AVLN2000: "Name 'PropertyName' already exists"
- Build fails with duplicate name errors

**Solution:**
```xml
<!-- ❌ WRONG: Using Name attribute on Grid -->
<Grid Name="MainGrid" RowDefinitions="Auto,*">
    <!-- This causes AVLN2000 errors -->
</Grid>

<!-- ✅ CORRECT: Use x:Name for Grid elements -->
<Grid x:Name="MainGrid" RowDefinitions="Auto,*">
    <TextBlock Grid.Row="0" Text="Header"/>
    <Border Grid.Row="1">
        <!-- Content -->
    </Border>
</Grid>
```

#### **Problem: Binding not working**
**Symptoms:**
- UI shows blank or default values
- Properties updating but UI not reflecting changes
- No binding errors in output

**Common Causes:**
1. DataContext not set correctly
2. Property name mismatch in binding
3. Wrong binding syntax

**Solution:**
```xml
<!-- ✅ CORRECT: Standard binding syntax -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventoryView">
    
    <!-- DataContext will be set by DI container -->
    
    <Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="Auto,*">
        <TextBlock Grid.Row="0" Grid.Column="0" Text="Part ID:"/>
        <TextBox Grid.Row="0" Grid.Column="1" 
                 Text="{Binding PartId}"/>  <!-- Property name must match ViewModel -->
        
        <Button Grid.Row="1" Grid.Column="1"
                Content="Search"
                Command="{Binding SearchCommand}"/> <!-- Command name must match -->
    </Grid>
</UserControl>
```

**Debugging Steps:**
1. Verify DataContext is set (check in debugger)
2. Check property names match exactly (case sensitive)
3. Verify ViewModel property has correct getter/setter
4. Check for binding errors in output window

#### **Problem: Grid layout not working**
**Symptoms:**
- Controls overlapping or not positioning correctly
- Grid definitions not taking effect

**Solution:**
```xml
<!-- ✅ CORRECT: Proper Grid syntax -->
<Grid x:Name="MainGrid" 
      RowDefinitions="Auto,*,Auto" 
      ColumnDefinitions="200,*">
      
    <!-- Row 0: Header -->
    <TextBlock Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2" 
               Text="Inventory Management" 
               Classes="page-header"/>
    
    <!-- Row 1: Content (spans full height due to *) -->
    <Border Grid.Row="1" Grid.Column="0" Classes="sidebar">
        <!-- Sidebar content -->
    </Border>
    
    <Border Grid.Row="1" Grid.Column="1" Classes="main-content">
        <!-- Main content -->
    </Border>
    
    <!-- Row 2: Footer -->
    <Border Grid.Row="2" Grid.Column="0" Grid.ColumnSpan="2" Classes="footer">
        <!-- Footer content -->
    </Border>
</Grid>
```

### Theme and Styling Issues

#### **Problem: MTM themes not applying**
**Symptoms:**
- Application using default Avalonia styling
- MTM colors and fonts not appearing

**Solution:**
```xml
<!-- In App.axaml - verify theme resources are included -->
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.App">
    <Application.Styles>
        <FluentTheme Mode="Light"/>
        
        <!-- MTM theme resources must be included -->
        <StyleInclude Source="/Resources/Themes/MTMComponents.axaml"/>
        <StyleInclude Source="/Resources/Themes/MTM_Blue.axaml"/>
        
    </Application.Styles>
</Application>

<!-- In UserControls - use MTM style classes -->
<Button Content="Primary Action" 
        Classes="primary-button"/>

<TextBlock Text="Status Message" 
           Classes="status-text"/>
```

## Performance Issues

### ViewModel Performance

#### **Problem: UI freezing during long operations**
**Symptoms:**
- UI becomes unresponsive
- Application appears to hang
- User cannot interact with interface

**Solution:**
```csharp
// ✅ CORRECT: Proper async implementation with UI feedback
[RelayCommand]
private async Task LoadDataAsync()
{
    IsLoading = true;
    StatusMessage = "Loading data...";
    
    try
    {
        // Use ConfigureAwait(false) for non-UI tasks
        var data = await _dataService.GetLargeDatasetAsync().ConfigureAwait(false);
        
        // Update UI on UI thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Items.Clear();
            foreach (var item in data)
            {
                Items.Add(item);
            }
        });
        
        StatusMessage = $"Loaded {data.Count} items";
    }
    catch (Exception ex)
    {
        await Services.ErrorHandling.HandleErrorAsync(ex, nameof(LoadDataAsync));
        StatusMessage = "Error loading data";
    }
    finally
    {
        IsLoading = false;
    }
}
```

#### **Problem: Memory leaks in ViewModels**
**Symptoms:**
- Application memory usage continuously increases
- Performance degrades over time
- OutOfMemory exceptions

**Solution:**
```csharp
// ✅ CORRECT: Proper resource cleanup
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel, IDisposable
{
    private readonly Timer _refreshTimer;
    private bool _disposed = false;
    
    public InventoryViewModel(ILogger<InventoryViewModel> logger) : base(logger)
    {
        _refreshTimer = new Timer(RefreshCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _refreshTimer?.Dispose();
                // Dispose other managed resources
            }
            _disposed = true;
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

// In View code-behind
protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
{
    if (DataContext is IDisposable disposableViewModel)
    {
        disposableViewModel.Dispose();
    }
    base.OnDetachedFromVisualTree(e);
}
```

### Database Performance

#### **Problem: Slow database queries**
**Symptoms:**
- Long delays for data loading
- Database timeouts
- Poor application responsiveness

**Solution:**
```csharp
// 1. Add appropriate database indexes for frequently queried columns
// CREATE INDEX idx_inventory_partid_operation ON inventory(part_id, operation);

// 2. Use pagination for large result sets
public async Task<ServiceResult<PagedResult<InventoryItem>>> GetInventoryPagedAsync(
    int page, int pageSize, string? filter = null)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PageNumber", page),
        new("p_PageSize", Math.Min(pageSize, 100)), // Limit max page size
        new("p_Filter", filter ?? string.Empty)
    };
    
    return await ExecuteStoredProcedureAsync("inv_inventory_GetPaged", parameters);
}

// 3. Use async patterns to avoid blocking UI
[RelayCommand]
private async Task LoadInventoryAsync()
{
    var result = await _inventoryService.GetInventoryPagedAsync(1, 50);
    // Process result...
}
```

## Common Development Workflow Issues

### Git and Version Control

#### **Problem: Merge conflicts in generated files**
**Symptoms:**
- Conflicts in obj/ or bin/ folders
- Generated code conflicts

**Solution:**
```bash
# Add to .gitignore (verify these entries exist)
bin/
obj/
*.user
*.suo
.vs/

# For merge conflicts in generated files, delete and regenerate
git clean -fdx
dotnet restore
dotnet build
```

### Build Issues

#### **Problem: Missing dependencies after git pull**
**Symptoms:**
- Build errors after pulling changes
- Missing NuGet packages
- Assembly load exceptions

**Solution:**
```bash
# Clean and restore solution
dotnet clean
dotnet restore
dotnet build

# If issues persist, clear NuGet cache
dotnet nuget locals all --clear
dotnet restore --force
```

## Debugging Strategies

### ViewModel Debugging
1. **Add logging to property setters and commands**
2. **Use debugger to verify property change notifications**
3. **Check DataContext binding in AXAML**
4. **Verify service dependencies are injected correctly**

### Service Debugging
1. **Add structured logging with correlation IDs**
2. **Use try-catch blocks with detailed exception logging**
3. **Verify database connectivity and permissions**
4. **Test stored procedures independently**

### UI Debugging
1. **Check AXAML binding syntax**
2. **Verify theme resources are loaded**
3. **Use Avalonia DevTools for runtime inspection**
4. **Check for AVLN compiler errors**

## Getting Help

### Internal Resources
1. **GitHub Memory Bank System**: Check `.github/memory-bank/` for context documents
2. **Architecture Documentation**: Review Phase 1 foundation documents
3. **Code Review Guidelines**: Follow established review processes
4. **Team Knowledge Base**: Consult documented patterns and decisions

### External Resources
1. **Avalonia Documentation**: https://docs.avaloniaui.net/
2. **MVVM Community Toolkit**: https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
3. **MySQL Documentation**: https://dev.mysql.com/doc/
4. **.NET 8 Documentation**: https://docs.microsoft.com/en-us/dotnet/

---

**Troubleshooting Guide Status**: ✅ Core Issues Documented  
**Coverage**: MVVM, Services, Database, AXAML, Performance  
**Last Updated**: September 4, 2025  
**Maintained By**: MTM Development Team