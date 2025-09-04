# MTM Code Review Guidelines

## Overview
The MTM Code Review Guidelines ensure consistent, high-quality code reviews that enforce established Phase 1 patterns, maintain manufacturing domain alignment, and leverage the comprehensive infrastructure built in Phase 2.

## Review Process Framework

### Review Stages
1. **Automated Reviews** (GitHub Actions)
2. **Technical Reviews** (Peer developers)
3. **Architecture Reviews** (Technical leads)
4. **Domain Reviews** (Business domain experts)
5. **Final Approval** (Project maintainers)

### Review Types by Change Complexity

#### **Simple Changes** (size:XS, size:S)
- **Required Reviews**: 1 technical reviewer
- **Review Focus**: Code quality, pattern compliance, testing
- **Approval Time Target**: 24 hours
- **Escalation**: None unless issues found

#### **Moderate Changes** (size:M)
- **Required Reviews**: 1 technical reviewer + 1 domain reviewer
- **Review Focus**: Design decisions, integration patterns, business logic
- **Approval Time Target**: 48 hours
- **Escalation**: Technical lead if review takes >72 hours

#### **Complex Changes** (size:L, size:XL)
- **Required Reviews**: 2 technical reviewers + 1 architecture reviewer
- **Review Focus**: Architectural impact, performance, security
- **Approval Time Target**: 1 week
- **Escalation**: Architecture board for significant concerns

## MTM Pattern Compliance Checklist

### MVVM Community Toolkit Compliance
```csharp
// ✅ CORRECT: Standard MTM ViewModel pattern
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteTransaction))]
    private string partId = string.Empty;

    [RelayCommand(CanExecute = nameof(CanExecuteTransaction))]
    private async Task ExecuteTransactionAsync()
    {
        IsLoading = true;
        try
        {
            await _inventoryService.ProcessTransactionAsync(PartId);
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, nameof(ExecuteTransactionAsync));
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanExecuteTransaction => !IsLoading && !string.IsNullOrWhiteSpace(PartId);
}
```

**Review Checklist - MVVM Patterns**:
- [ ] **[ObservableObject]**: ViewModels use partial class with [ObservableObject] attribute
- [ ] **[ObservableProperty]**: Properties use source generators instead of manual implementation
- [ ] **[RelayCommand]**: Commands use [RelayCommand] with proper CanExecute logic
- [ ] **BaseViewModel**: Inherits from established BaseViewModel class
- [ ] **Dependency Injection**: Constructor injection follows established patterns
- [ ] **Error Handling**: Uses centralized error handling via Services.ErrorHandling
- [ ] **Async Patterns**: Proper async/await implementation with loading states

#### **Common MVVM Anti-Patterns to Flag**:
```csharp
// ❌ WRONG: Manual property implementation
public string PartId
{
    get => _partId;
    set => SetProperty(ref _partId, value);
}

// ❌ WRONG: ReactiveUI patterns (removed in MTM)
this.WhenAnyValue(x => x.PartId).Subscribe(...)

// ❌ WRONG: Direct database access in ViewModel
var result = await _database.ExecuteQueryAsync("SELECT ...");
```

### Service Architecture Compliance
```csharp
// ✅ CORRECT: Standard MTM Service pattern
public class InventoryService : IInventoryService
{
    private readonly ILogger<InventoryService> _logger;
    private readonly IDatabaseService _databaseService;

    public InventoryService(
        ILogger<InventoryService> logger,
        IDatabaseService databaseService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public async Task<ServiceResult<InventoryItem>> GetInventoryAsync(string partId, string operation)
    {
        try
        {
            _logger.LogInformation("Retrieving inventory for {PartId} at operation {Operation}", partId, operation);

            var parameters = new MySqlParameter[]
            {
                new("p_PartId", partId),
                new("p_Operation", operation)
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, "inv_inventory_Get_ByPartIDandOperation", parameters);

            if (result.Status == 1 && result.Data.Rows.Count > 0)
            {
                var inventoryItem = MapToInventoryItem(result.Data.Rows[0]);
                return ServiceResult<InventoryItem>.Success(inventoryItem);
            }

            return ServiceResult<InventoryItem>.Failure("Inventory item not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory for {PartId} at {Operation}", partId, operation);
            return ServiceResult<InventoryItem>.Failure(ex.Message, ex);
        }
    }
}
```

**Review Checklist - Service Patterns**:
- [ ] **Interface Implementation**: Service implements clear, focused interface
- [ ] **Constructor Validation**: ArgumentNullException for required dependencies  
- [ ] **Logging**: Structured logging with meaningful context
- [ ] **Database Access**: Uses Helper_Database_StoredProcedure exclusively
- [ ] **Error Handling**: Comprehensive try-catch with proper logging
- [ ] **Result Patterns**: Returns ServiceResult<T> for operation outcomes
- [ ] **Method Naming**: Clear, descriptive method names following conventions
- [ ] **Single Responsibility**: Service methods have single, clear purpose

### Database Access Compliance
```csharp
// ✅ CORRECT: MTM Database Access pattern
public async Task<DatabaseResult> UpdateInventoryAsync(InventoryUpdateRequest request)
{
    var parameters = new MySqlParameter[]
    {
        new("p_PartId", request.PartId ?? string.Empty),
        new("p_Operation", request.Operation ?? string.Empty), 
        new("p_Quantity", request.Quantity),
        new("p_Location", request.Location ?? string.Empty),
        new("p_UserId", request.UserId ?? Environment.UserName),
        new("p_TransactionType", request.TransactionType ?? string.Empty)
    };

    return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString, "inv_inventory_Update_Item", parameters);
}
```

**Review Checklist - Database Patterns**:
- [ ] **Stored Procedures Only**: No direct SQL queries or ORM usage
- [ ] **Helper Usage**: Uses Helper_Database_StoredProcedure.ExecuteDataTableWithStatus consistently
- [ ] **Parameter Binding**: Proper MySqlParameter usage with null handling
- [ ] **Result Processing**: Structured handling of DatabaseResult
- [ ] **Connection Management**: No manual connection management
- [ ] **SQL Injection Prevention**: Parameterized queries only
- [ ] **Error Handling**: Database exceptions properly caught and handled
- [ ] **Transaction Support**: Proper transaction handling for multi-step operations

### Avalonia AXAML Compliance
```xml
<!-- ✅ CORRECT: MTM AXAML Pattern -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventoryView">
    
    <Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">
        
        <TextBlock Grid.Row="0" Grid.Column="0" 
                   Text="Part ID:" 
                   Classes="form-label"/>
        
        <TextBox Grid.Row="0" Grid.Column="1"
                 Text="{Binding PartId}"
                 Watermark="Enter part ID"
                 Classes="form-input"/>
                 
        <Button Grid.Row="1" Grid.Column="1"
                Content="Search Inventory"
                Command="{Binding SearchCommand}"
                Classes="primary-button"/>
                
    </Grid>
    
</UserControl>
```

**Review Checklist - AXAML Patterns**:
- [ ] **x:Name Usage**: Uses x:Name instead of Name for Grid elements
- [ ] **Namespace Correct**: Uses https://github.com/avaloniaui namespace
- [ ] **Grid Definitions**: Uses attribute form (RowDefinitions="Auto,*") when simple
- [ ] **Binding Syntax**: Uses {Binding PropertyName} consistently
- [ ] **Theme Integration**: Uses MTM theme classes appropriately
- [ ] **Control Hierarchy**: Logical control hierarchy and organization
- [ ] **Resource Usage**: Proper use of styles and resources
- [ ] **Accessibility**: Appropriate accessibility attributes where needed

### Manufacturing Domain Compliance

**Review Checklist - Domain Alignment**:
- [ ] **Business Context**: Code reflects manufacturing inventory management domain
- [ ] **Operation Workflows**: Supports operation sequence (90→100→110→120)
- [ ] **Transaction Types**: Properly handles IN/OUT/TRANSFER based on user intent
- [ ] **Part Tracking**: Consistent PartId usage and validation
- [ ] **Data Models**: Domain entities reflect manufacturing concepts
- [ ] **Validation Rules**: Business rules properly enforced
- [ ] **User Workflows**: Supports manufacturing user personas and workflows
- [ ] **Integration**: Compatible with manufacturing systems and processes

## Review Focus Areas by Component

### ViewModels Review Focus
1. **MVVM Pattern Adherence**: Source generators, commands, properties
2. **Business Logic**: Appropriate level of business logic vs. service delegation
3. **Data Binding**: Proper property change notifications and binding support
4. **User Experience**: Loading states, error handling, user feedback
5. **Testing**: Unit test coverage for ViewModel logic

### Services Review Focus
1. **Interface Design**: Clear, focused service contracts
2. **Dependency Management**: Proper injection and lifecycle management
3. **Error Handling**: Comprehensive error handling and logging
4. **Performance**: Efficient algorithms and resource usage
5. **Testing**: Unit and integration test coverage

### Views (AXAML) Review Focus
1. **AXAML Syntax**: Compliance with Avalonia best practices
2. **Data Binding**: Proper binding expressions and patterns
3. **User Interface**: Usability, accessibility, responsive design
4. **Theme Integration**: Consistent with MTM design system
5. **Performance**: Efficient rendering and layout

### Database Review Focus
1. **Stored Procedures**: Quality, performance, maintainability
2. **Data Access**: Proper use of Helper_Database_StoredProcedure
3. **Security**: SQL injection prevention, authorization
4. **Performance**: Query optimization, indexing considerations
5. **Integration**: Compatibility with existing schema and procedures

## Review Process Workflow

### 1. Automated Review (GitHub Actions)
- **Build Verification**: Code compiles without errors
- **Test Execution**: All automated tests pass
- **Static Analysis**: Code quality metrics meet standards
- **Pattern Compliance**: Automated checks for MTM patterns
- **Security Scanning**: No security vulnerabilities introduced

### 2. Technical Review (Peer Developers)
**Review Timeline**: Within 24-48 hours of PR creation

**Review Process**:
1. **Functional Review**: Verify changes work as intended
2. **Code Quality**: Check readability, maintainability, documentation
3. **Pattern Compliance**: Validate adherence to MTM patterns
4. **Testing**: Review test coverage and quality
5. **Integration**: Ensure proper integration with existing codebase

**Review Feedback Format**:
```markdown
## Code Review Feedback

### ✅ Positive Feedback
- MVVM patterns correctly implemented
- Good error handling throughout
- Comprehensive test coverage

### 🔧 Required Changes
- [ ] Fix: Database parameter validation missing on line 42
- [ ] Pattern: Use [ObservableProperty] instead of manual property on line 28
- [ ] Test: Add unit test for error scenario

### 💡 Suggestions
- Consider adding logging for debugging purposes
- Performance could be improved with caching
- Documentation could be enhanced for complex logic

### 📋 Pattern Compliance
- [x] MVVM Community Toolkit patterns
- [x] Service architecture patterns  
- [x] Database access patterns
- [x] AXAML syntax compliance
- [x] Error handling patterns
```

### 3. Architecture Review (Technical Leads)
**Required For**: size:L, size:XL, architecture changes, breaking changes

**Review Focus**:
- **Architectural Impact**: Changes align with system architecture
- **Performance Implications**: No significant performance regressions
- **Security Considerations**: Security implications properly addressed  
- **Scalability**: Changes support system scalability requirements
- **Integration**: Proper integration with existing architecture

### 4. Domain Review (Business Experts)
**Required For**: Manufacturing domain changes, business logic changes

**Review Focus**:
- **Business Logic**: Correctly implements business requirements
- **Domain Model**: Proper representation of manufacturing concepts
- **User Workflows**: Supports intended user scenarios
- **Compliance**: Meets manufacturing industry standards
- **Integration**: Compatible with existing business processes

## Review Quality Metrics

### Review Effectiveness Metrics
- **Defect Detection Rate**: Percentage of bugs caught during review vs. production
- **Review Coverage**: Percentage of changes that receive appropriate review
- **Review Turnaround Time**: Average time from PR creation to approval
- **Pattern Compliance Rate**: Percentage of reviews passing pattern compliance checks

### Code Quality Metrics
- **Review Comments per PR**: Average number of review comments
- **Rework Rate**: Percentage of PRs requiring significant changes after review
- **Approval Rate**: Percentage of PRs approved without changes
- **Knowledge Transfer**: Evidence of knowledge sharing through reviews

### Reviewer Performance Metrics
- **Review Participation**: Distribution of review load across team members
- **Review Quality**: Effectiveness of individual reviewers at catching issues
- **Domain Expertise**: Alignment of reviewers with appropriate domain knowledge
- **Mentoring Impact**: Evidence of skill development through review process

## Tools and Automation

### GitHub Integration
- **Review Assignment**: Automatic reviewer assignment based on changed files
- **Status Checks**: Required status checks before merge approval
- **Review Templates**: Standardized review checklists and feedback templates
- **Metrics Collection**: Automated collection of review metrics and reporting

### Code Analysis Tools
- **Static Analysis**: Automated code quality and pattern compliance checking
- **Security Scanning**: Automated security vulnerability detection
- **Performance Analysis**: Basic performance impact assessment
- **Documentation Generation**: Automated documentation updates from code changes

## Training and Onboarding

### New Team Member Review Training
1. **MTM Pattern Overview**: Understanding established patterns and rationale
2. **Review Process**: Step-by-step review workflow and expectations
3. **Tool Usage**: GitHub review tools and automated assistance
4. **Domain Context**: Manufacturing industry and business requirements
5. **Quality Standards**: Code quality expectations and measuring success

### Ongoing Review Skills Development
- **Monthly Review Retrospectives**: Team review of review process effectiveness
- **Pattern Updates**: Training on new patterns and architectural decisions
- **Tool Training**: Updates on new tools and automation capabilities
- **Cross-Domain Learning**: Exposure to different areas of the codebase
- **Mentoring Program**: Pairing experienced reviewers with newer team members

---

**System Status**: ✅ Code Review Guidelines Complete  
**Integration**: Phase 1 patterns + Phase 2 automation infrastructure  
**Quality Assurance**: Automated + Manual review processes  
**Maintained By**: MTM Development Team