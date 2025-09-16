# MTM Universal Framework - GitHub Copilot Integration Template

**Framework**: Universal Cross-Platform Framework  
**Pattern**: AI-Assisted Development for Any Business Domain  
**Created**: January 2025  

---

## 🤖 **Universal GitHub Copilot Configuration**

This template provides AI-assisted development capabilities that can be adapted to any business domain while maintaining the high-quality patterns and practices from the MTM WIP Application.

### **Core Instructions Template**

```markdown
# GitHub Copilot Instructions for [YOUR_PROJECT_NAME]

**Generate code strictly following the established patterns found in this .NET 8 Avalonia MVVM application. Never introduce patterns not already present in the codebase.**

**Target Framework**: .NET 8 (`<TargetFramework>net8.0</TargetFramework>`)
- **C# Language Version**: C# 12 with nullable reference types enabled
- **Avalonia UI**: 11.3.4 (Primary UI framework - NOT WPF)
- **MVVM Community Toolkit**: 8.3.2 (Property/Command generation via source generators)
- **Database**: [YOUR_DATABASE] (Replace with your database technology)
- **Microsoft Extensions**: 9.0.8 (DI, Logging, Configuration, Hosting)

### **Architecture Pattern Detection**
- **Architecture**: MVVM with service-oriented design and comprehensive dependency injection
- **Database Pattern**: [YOUR_DATABASE_PATTERN] (Replace with your data access patterns)
- **ViewModel Pattern**: MVVM Community Toolkit with `[ObservableProperty]` and `[RelayCommand]` attributes
- **UI Pattern**: Avalonia UserControl inheritance with minimal code-behind
- **Service Pattern**: Category-based service consolidation in single files
- **Error Pattern**: Centralized error handling via `UniversalErrorHandling.HandleErrorAsync()`

**NEVER use ReactiveUI patterns** - Use `[ObservableProperty]` and `[RelayCommand]` only
```

### **Domain-Agnostic Instruction Templates**

#### **Universal ViewModel Pattern Template**
```csharp
// ✅ CORRECT: Universal MVVM Community Toolkit pattern
[ObservableObject]
public partial class [EntityName]ViewModel : BaseViewModel
{
    [ObservableProperty]
    private string entityId = string.Empty;

    [ObservableProperty] 
    private bool isLoading;

    [RelayCommand]
    private async Task SaveAsync()
    {
        IsLoading = true;
        try
        {
            // Business logic here - replace with your domain operations
            await _[serviceType]Service.SaveAsync(EntityId);
        }
        catch (Exception ex)
        {
            await UniversalErrorHandling.HandleErrorAsync(ex, "Save [entity] operation");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public [EntityName]ViewModel(
        ILogger<[EntityName]ViewModel> logger,
        I[ServiceType]Service [serviceType]Service)
        : base(logger)
    {
        _[serviceType]Service = [serviceType]Service;
    }
}
```

#### **Universal Service Pattern Template**
```csharp
// ✅ CORRECT: Universal service pattern
public class [EntityName]Service : I[EntityName]Service
{
    private readonly ILogger<[EntityName]Service> _logger;
    private readonly IUniversalConfigurationService _configuration;

    public [EntityName]Service(
        ILogger<[EntityName]Service> logger,
        IUniversalConfigurationService configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<ServiceResult> SaveAsync([EntityName] entity)
    {
        try
        {
            // Replace with your business logic and data access patterns
            _logger.LogInformation("Saving {EntityType} with ID {EntityId}", 
                typeof([EntityName]).Name, entity.Id);

            // Your domain-specific save logic here
            await Task.CompletedTask; // Replace with actual implementation

            return ServiceResult.Success("Entity saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save {EntityType} with ID {EntityId}", 
                typeof([EntityName]).Name, entity.Id);
            return ServiceResult.Failure($"Failed to save entity: {ex.Message}", ex);
        }
    }
}
```

### **Cross-Platform UI Template**
```xml
<!-- Universal Avalonia AXAML template -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="[YourNamespace].[EntityName]View">

    <ScrollViewer>
        <Grid RowDefinitions="*,Auto" Margin="24">
            
            <!-- Main content area -->
            <Border Grid.Row="0" 
                    Background="{DynamicResource MTM_Shared_Logic.ContentAreas}"
                    BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
                    BorderThickness="1"
                    CornerRadius="8"
                    Padding="24">
                
                <StackPanel Spacing="16">
                    <!-- Replace with your domain-specific input fields -->
                    <TextBox Text="{Binding EntityId}" 
                             Watermark="[Entity] ID"/>
                    
                    <!-- Add more fields as needed for your domain -->
                </StackPanel>
                
            </Border>
            
            <!-- Action buttons -->
            <StackPanel Grid.Row="1" 
                        Orientation="Horizontal" 
                        HorizontalAlignment="Right" 
                        Spacing="12" 
                        Margin="0,16,0,0">
                
                <Button Content="Save" 
                        Command="{Binding SaveCommand}"
                        Classes="primary"/>
                
                <Button Content="Cancel" 
                        Command="{Binding CancelCommand}"/>
                        
            </StackPanel>
            
        </Grid>
    </ScrollViewer>
    
</UserControl>
```

## 🔧 **Customization Guide**

### **Step 1: Replace Domain-Specific Terms**

Replace these placeholders with your business domain:

| Placeholder | Replace With | Example |
|------------|--------------|---------|
| `[YOUR_PROJECT_NAME]` | Your project name | `InventoryManagement`, `CustomerPortal` |
| `[EntityName]` | Your main entity | `Product`, `Customer`, `Order` |
| `[ServiceType]` | Your service type | `Inventory`, `Customer`, `Order` |
| `[YOUR_DATABASE]` | Your database | `PostgreSQL`, `SQL Server`, `SQLite` |
| `[YOUR_DATABASE_PATTERN]` | Your data pattern | `Entity Framework`, `Stored Procedures`, `Repository Pattern` |

### **Step 2: Adapt Business Rules**

Update the instruction templates with your specific business rules:

```csharp
// Example: Replace manufacturing operations with your workflow
// Instead of: "90", "100", "110", "120" (manufacturing operations)
// Use your domain: "Draft", "Review", "Approved", "Published" (document workflow)

// Replace validation rules
public static class [YourDomain]ValidationRules
{
    public static ValidationResult Validate[EntityName]([EntityName] entity)
    {
        // Your domain-specific validation logic
        if (string.IsNullOrWhiteSpace(entity.Id))
            return ValidationResult.Error("[Entity] ID is required");
            
        // Add your business rules here
        return ValidationResult.Success();
    }
}
```

### **Step 3: Configure AI Context**

Create context files specific to your domain:

```markdown
# [your-domain]-business-context.instructions.md

**Business Domain**: [Your Domain Description]
**Primary Entities**: [Entity1], [Entity2], [Entity3]
**Key Workflows**: [Workflow1], [Workflow2], [Workflow3]
**Business Rules**: 
- [Rule 1]: [Description]
- [Rule 2]: [Description]
- [Rule 3]: [Description]

**Data Patterns**:
- [Pattern 1]: [Description and usage]
- [Pattern 2]: [Description and usage]

**Integration Points**:
- [System 1]: [Integration pattern]
- [System 2]: [Integration pattern]
```

## 📚 **Advanced AI Assistance Scenarios**

### **Domain-Specific Code Generation Prompts**

```
Generate a complete MVVM Community Toolkit ViewModel for [your domain] [entity] management with:
- ObservableProperty for [field1] (string), [field2] (int), [field3] (DateTime)
- RelayCommand for Save[Entity]Async with proper error handling using UniversalErrorHandling.HandleErrorAsync
- Validation logic ensuring [your business rules]
- [Your domain-specific business rules]
- Integration with I[ServiceType]Service
- Cross-platform compatibility considerations
Use [your domain] context with examples like "[example1]", "[example2]", "[example3]"
```

### **Service Integration Patterns**

```
Create service integration pattern using MVVM Community Toolkit messaging for:
- [EntityService] publishing [Entity]UpdatedMessage when items updated
- [RelatedService] subscribing to update related data
- [AuditService] recording all changes with audit trail
- Cross-service validation ensuring [your business rules]
- Error propagation with centralized UniversalErrorHandling.HandleErrorAsync
- Performance monitoring with execution time tracking
- [Your domain-specific business rule enforcement] across service boundaries
Include IMessenger registration, message definitions, and subscriber patterns with proper cleanup
```

## 🎯 **Quality Assurance Templates**

### **Code Review Checklist Template**

```markdown
# Code Review Checklist - [Your Project Name]

## ✅ MVVM Community Toolkit Compliance
- [ ] Uses `[ObservableProperty]` attributes (not manual property implementation)
- [ ] Uses `[RelayCommand]` attributes (not manual command implementation)
- [ ] No ReactiveUI patterns present
- [ ] BaseViewModel inheritance used correctly

## ✅ [Your Domain] Business Rules
- [ ] [Business Rule 1] validated correctly
- [ ] [Business Rule 2] enforced
- [ ] [Business Rule 3] implemented
- [ ] Data integrity maintained

## ✅ Error Handling
- [ ] All exceptions use UniversalErrorHandling.HandleErrorAsync
- [ ] User-friendly error messages provided
- [ ] Logging includes sufficient context
- [ ] Business context preserved in error handling

## ✅ Cross-Platform Compatibility
- [ ] Avalonia UI syntax correct (not WPF)
- [ ] Platform-specific code properly abstracted
- [ ] Touch and mouse input both supported
- [ ] Theme system integration complete
```

---

**Template Version**: 1.0.0  
**Last Updated**: January 2025  
**Compatible With**: Any business domain using .NET 8 + Avalonia UI  
**AI Integration**: Complete GitHub Copilot optimization

This template maintains the manufacturing-grade quality standards of the original MTM system while being adaptable to any business domain and development scenario.