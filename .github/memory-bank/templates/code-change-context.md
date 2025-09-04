# Code Change Context Template

**Change Date**: [YYYY-MM-DD]  
**Developer(s)**: [Names/Usernames]  
**Change Type**: [Feature | Bug Fix | Refactoring | Performance | Security | Documentation]  
**Impact Level**: [Major | Minor | Patch]

## Change Overview
<!-- Brief summary of what was changed and why -->

## Related Context
**Related Issues**: #  
**Related PR**: #  
**Related ADR**: [Link to Architecture Decision Record if applicable]  
**Session Context**: [Link to development session context if applicable]

## Business Justification
<!-- Why was this change necessary from a business perspective? -->

## Technical Justification
<!-- What technical problems does this change solve? -->

## Change Details

### Components Modified
**ViewModels**:
- [File]: [Brief description of changes]

**Services**:
- [File]: [Brief description of changes]

**Views (AXAML)**:
- [File]: [Brief description of changes]

**Models**:
- [File]: [Brief description of changes]

**Database**:
- [Stored Procedure]: [Description of changes]

### New Components Created
- [File]: [Purpose and functionality]

### Components Removed
- [File]: [Reason for removal]

## MTM Pattern Implementation

### MVVM Community Toolkit Usage
```csharp
// Example of MVVM Community Toolkit patterns implemented

[ObservableObject]
public partial class ExampleViewModel : BaseViewModel
{
    [ObservableProperty]
    private string exampleProperty = string.Empty;
    
    [RelayCommand]
    private async Task ExampleCommandAsync()
    {
        // Implementation following MTM patterns
    }
}
```

**Pattern Compliance**:
- [ ] **[ObservableProperty]**: Used instead of manual property notification
- [ ] **[RelayCommand]**: Used for all command implementations
- [ ] **BaseViewModel**: Inherits from established base class
- [ ] **Dependency Injection**: Proper constructor injection patterns

### Service Architecture Integration
```csharp
// Example of service integration patterns

public class ExampleService : IExampleService
{
    private readonly ILogger<ExampleService> _logger;
    private readonly IDatabaseService _databaseService;
    
    public ExampleService(ILogger<ExampleService> logger, IDatabaseService databaseService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }
    
    public async Task<ServiceResult> ProcessAsync(ExampleRequest request)
    {
        try
        {
            // Service implementation following MTM patterns
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, nameof(ProcessAsync));
            return ServiceResult.Failure(ex.Message);
        }
    }
}
```

**Service Pattern Compliance**:
- [ ] **Interface-based**: Service implements clear interface
- [ ] **Constructor Injection**: Dependencies injected via constructor
- [ ] **Error Handling**: Uses centralized error handling
- [ ] **Result Patterns**: Returns structured result objects

### Database Access Patterns
```csharp
// Example of database access following MTM patterns

var parameters = new MySqlParameter[]
{
    new("p_PartId", request.PartId),
    new("p_Operation", request.Operation),
    new("p_Quantity", request.Quantity)
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    _connectionString,
    "inv_inventory_example_operation",
    parameters
);

if (result.Status == 1)
{
    // Process successful result
    return ProcessDatabaseResult(result.Data);
}
else
{
    // Handle database error
    throw new InvalidOperationException($"Database operation failed with status: {result.Status}");
}
```

**Database Pattern Compliance**:
- [ ] **Stored Procedures Only**: No direct SQL queries used
- [ ] **Helper Usage**: Uses Helper_Database_StoredProcedure consistently
- [ ] **Parameter Binding**: Proper parameter binding to prevent SQL injection
- [ ] **Result Handling**: Structured handling of database results

### Avalonia UI Patterns
```xml
<!-- Example of AXAML following MTM patterns -->

<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Views.ExampleView">
    
    <Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">
        
        <TextBlock Grid.Row="0" Grid.Column="0" 
                   Text="Example Label" 
                   Classes="section-header"/>
        
        <TextBox Grid.Row="0" Grid.Column="1"
                 Text="{Binding ExampleProperty}"
                 Watermark="Enter example value"/>
                 
    </Grid>
    
</UserControl>
```

**AXAML Pattern Compliance**:
- [ ] **x:Name Usage**: Uses x:Name instead of Name for Grid elements
- [ ] **Proper Namespace**: Uses correct Avalonia namespace
- [ ] **Grid Definitions**: Uses attribute form for simple grid definitions
- [ ] **Binding Syntax**: Uses standard {Binding} syntax

## Manufacturing Domain Integration

### Business Context
<!-- How does this change support the manufacturing inventory management domain? -->

### Workflow Integration
**Operation Numbers**: [How the change handles operation workflow steps like 90, 100, 110, 120]  
**Transaction Types**: [How the change determines IN/OUT/TRANSFER based on user intent]  
**Part Tracking**: [How the change supports part ID and inventory tracking]

### Domain Model Impact
```csharp
// Example of domain model changes

public class InventoryItem
{
    public string PartId { get; set; } = string.Empty;        // Manufacturing part identifier
    public string Operation { get; set; } = string.Empty;     // Workflow step (90, 100, 110, 120)
    public int Quantity { get; set; }                         // Integer quantity only
    public string Location { get; set; } = string.Empty;      // Physical location
    public string TransactionType { get; set; } = string.Empty; // User intent (IN/OUT/TRANSFER)
    
    // New properties added in this change
    public string NewProperty { get; set; } = string.Empty;   // Description of new property
}
```

## Implementation Challenges and Solutions

### Challenge 1: [Description]
**Problem**: [What was the specific challenge]  
**Solution**: [How it was addressed]  
**Trade-offs**: [What compromises were made]  
**Future Considerations**: [What to watch out for]

### Challenge 2: [Description]
**Problem**: [What was the specific challenge]  
**Solution**: [How it was addressed]  

## Testing Strategy and Results

### Unit Testing
```csharp
// Example unit tests for the changes

[Test]
public async Task ExampleService_ProcessAsync_ReturnsSuccess()
{
    // Arrange
    var mockLogger = new Mock<ILogger<ExampleService>>();
    var mockDatabase = new Mock<IDatabaseService>();
    var service = new ExampleService(mockLogger.Object, mockDatabase.Object);
    
    // Act
    var result = await service.ProcessAsync(new ExampleRequest());
    
    // Assert
    Assert.That(result.IsSuccess, Is.True);
}
```

**Test Coverage**:
- [ ] **Unit Tests**: [Number] new unit tests added
- [ ] **Integration Tests**: [Number] integration tests added
- [ ] **Manual Tests**: [Description of manual testing performed]

### Test Results
- **Before Change**: [Test results before implementation]
- **After Change**: [Test results after implementation]
- **Performance Impact**: [Any performance changes observed]

## Performance Impact

### Before vs After Metrics
| Metric | Before | After | Change |
|--------|--------|-------|--------|
| Response Time | [value] | [value] | [+/-X%] |
| Memory Usage | [value] | [value] | [+/-X%] |
| Database Queries | [count] | [count] | [+/-X] |

### Performance Optimizations Made
- [Optimization 1 and its impact]
- [Optimization 2 and its impact]

## Security Considerations
- [ ] **Input Validation**: All inputs properly validated
- [ ] **SQL Injection Prevention**: Stored procedures prevent injection attacks
- [ ] **Authorization**: Proper authorization checks implemented
- [ ] **Error Information**: No sensitive information leaked in errors

## GitHub Copilot Learning

### Context Enhancement
<!-- How does this change improve GitHub Copilot's understanding of the codebase? -->

### Pattern Recognition
<!-- What new patterns are established that Copilot can learn from? -->

### Future Code Generation
<!-- How will this change help Copilot generate better code in the future? -->

## Documentation Updates Made
- [ ] **Inline Comments**: Code properly commented for complex logic
- [ ] **XML Documentation**: Public APIs have XML documentation  
- [ ] **GitHub Instructions**: [Which instruction files were updated]
- [ ] **Architecture Docs**: [Architecture documentation updates]

## Knowledge Preservation

### Lessons Learned
1. **Lesson**: [What was learned during implementation]
   - **Application**: [How this applies to future development]

2. **Pattern**: [New pattern discovered or refined]
   - **Usage**: [When and how to use this pattern]

### Anti-Patterns Discovered
1. **Anti-Pattern**: [What should be avoided]
   - **Why**: [Why this approach is problematic]
   - **Alternative**: [What to do instead]

## Future Maintenance Considerations

### Monitoring Requirements
<!-- What should be monitored to ensure this change continues to work well? -->

### Evolution Path
<!-- How might this change need to evolve in the future? -->

### Deprecated Patterns
<!-- Are any patterns being deprecated by this change? -->

## References and Links

### Related Documentation
- [GitHub Instructions updated]: [Links to instruction files]
- [Architecture docs]: [Links to architecture documentation]
- [External resources]: [Links to external documentation used]

### Code References
- **Commit SHA**: [Git commit hash]
- **Branch**: [Feature branch name]
- **PR**: #[Pull request number]

---

**Template Version**: 1.0  
**Created**: September 4, 2025  
**Part of**: MTM Memory Bank System - Code Change History Tracking  
**Integration**: Phase 1 GitHub Instructions System