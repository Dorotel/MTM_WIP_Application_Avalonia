# MTM Feature Request Template

## 🎯 Feature Development Instructions

When implementing new features for the MTM WIP Application, follow these patterns:

### **ViewModel Creation Pattern**
```csharp
// Use MVVM Community Toolkit patterns exclusively
[ObservableObject]
public partial class [Feature]ViewModel : BaseViewModel
{
    [ObservableProperty]
    private string property = string.Empty;
    
    [RelayCommand]
    private async Task ExecuteActionAsync()
    {
        IsLoading = true;
        try
        {
            // Feature implementation
            await SomeService.PerformActionAsync();
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Feature operation failed");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    public [Feature]ViewModel(ILogger<[Feature]ViewModel> logger, [Dependencies])
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        // Initialize dependencies
    }
}
```

### **Service Integration Pattern**
```csharp
// Database operations via stored procedures only
public class [Feature]Service : I[Feature]Service
{
    public async Task<OperationResult> ProcessFeatureAsync(FeatureData data)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_Parameter1", data.Value1),
            new("p_Parameter2", data.Value2)
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString,
            "[feature]_stored_procedure_name",
            parameters
        );

        if (result.Status == 1)
        {
            // Process successful result
            return OperationResult.Success(result.Data);
        }
        
        return OperationResult.Failed($"Database operation failed");
    }
}
```

### **View Integration Pattern**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.[Area]"
             x:Class="MTM_WIP_Application_Avalonia.Views.[Area].[Feature]View">
    
    <Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,*">
        <TextBlock Grid.Column="0" Grid.Row="0" 
                   Text="Feature Title" 
                   FontWeight="Bold"
                   Foreground="#6a0dad" />
        
        <!-- Feature UI components -->
    </Grid>
</UserControl>
```

### **Error Handling Integration**
```csharp
try
{
    // Feature operation
}
catch (Exception ex)
{
    await Services.ErrorHandling.HandleErrorAsync(ex, "Feature-specific context");
}
```

### **Dependency Injection Registration**
```csharp
// In ServiceCollectionExtensions.cs
services.TryAddScoped<I[Feature]Service, [Feature]Service>();
services.TryAddTransient<[Feature]ViewModel>();
```