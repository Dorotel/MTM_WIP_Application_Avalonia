# Development Directory

This directory contains all development-specific files, documentation, and resources for the MTM WIP Application Avalonia project.

## ??? Directory Structure

### Core Development Areas

#### `Database_Files/` ? **CRITICAL FIX #1 COMPLETED**
Development database schema, stored procedures, and documentation:
- **Development_Database_Schema.sql**: Development environment database structure
- **New_Stored_Procedures.sql**: ? **12 comprehensive procedures implemented with standardized error handling**
- **Updated_Stored_Procedures.sql**: ? **Ready template for future procedure updates**
- **README_NewProcedures.md**: ? **Complete documentation with examples and integration guidance**
- **README_*.md**: Comprehensive database documentation

#### `UI_Documentation/`
Complete user interface component specifications:
- **Controls/**: Individual control instruction files (.instructions.md)
- **Forms/**: Form-level documentation and specifications
- **README files**: Component organization and development guidelines

#### `Custom_Prompts/` ? **ENHANCED WITH COMPLIANCE FIXES**
Development prompt templates for consistent code generation:
- **Compliance_Fix01_EmptyDevelopmentStoredProcedures.md**: ? **COMPLETED**
- **Compliance_Fix02_MissingStandardOutputParameters.md**: Template ready (enabled by Fix #1)
- **Compliance_Fix03_InadequateErrorHandlingStoredProcedures.md**: Template ready (enabled by Fix #1)
- **Compliance_Fix04-11_*.md**: Additional compliance fix templates
- **README.md**: Complete compliance fix overview and priorities

#### `Examples/`
Code examples, usage patterns, and reference implementations:
- **ErrorHandlingUsageExample.cs**: Error handling implementation examples
- **ErrorMessageUIUsageExample.cs**: UI error message display patterns
- **Service usage examples**: Best practices for service integration

#### `Docs/`
HTML documentation and development guides:
- **index.html**: Main documentation portal
- **Various .html files**: Specific development topic guides
- **styles.css**: Documentation styling

#### `UI_Screenshots/` (if present)
Visual design references and UI mockups for development guidance.

## ?? Development Philosophy

### Code Quality Standards
- **Clean Architecture**: Separation of concerns with clear layer boundaries
- **SOLID Principles**: Following SOLID design principles throughout
- **DRY (Don't Repeat Yourself)**: Avoiding code duplication through reusable components
- **MVVM Pattern**: Strict Model-View-ViewModel implementation with ReactiveUI
- **Async-First**: All I/O operations implemented asynchronously

### Database-First Approach ? **ENHANCED WITH NEW PROCEDURES**
- **Stored Procedures Only**: No direct SQL in application code ? **12 new procedures available**
- **Business Logic in Database**: Complex business rules implemented as stored procedures ? **MTM rules enforced**
- **Transaction Integrity**: All operations wrapped in proper database transactions ? **Standardized transaction management**
- **Audit Trail**: Complete transaction logging for all data modifications ? **Comprehensive error logging**
- **Input Validation**: ? **All new procedures include comprehensive parameter validation**
- **Error Handling**: ? **Standardized error handling pattern implemented across all procedures**

### UI/UX Guidelines
- **MTM Brand Consistency**: Purple-based color scheme throughout
- **Responsive Design**: Adaptive layouts for different screen sizes
- **Accessibility**: Full keyboard navigation and screen reader support
- **Modern Patterns**: Card-based layouts, hero sections, and clean navigation

## ?? Development Tools and Processes

### Database Development ? **DEVELOPMENT UNBLOCKED**
1. **New Procedures**: Always add to `Database_Files/New_Stored_Procedures.sql` ? **12 procedures ready for use**
2. **Procedure Updates**: Copy existing to `Updated_Stored_Procedures.sql` then modify ? **Template prepared**
3. **Never Edit Production**: `Database_Files/Existing_Stored_Procedures.sql` is read-only
4. **Documentation**: Update corresponding README files with any changes ? **Complete documentation available**
5. **Error Handling**: ? **Follow standardized pattern from New_Stored_Procedures.sql**
6. **Integration**: ? **All procedures designed for Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()**

### Available Database Operations ? **READY FOR USE**

#### **Enhanced Inventory Management Procedures:**
- `inv_inventory_Add_Item_Enhanced` - Add inventory with full error handling
- `inv_inventory_Remove_Item_Enhanced` - Remove inventory with stock validation
- `inv_inventory_Transfer_Item_New` - Transfer between locations

#### **Inventory Query Procedures:**
- `inv_inventory_Get_ByLocation_New` - Get all items by location
- `inv_inventory_Get_ByOperation_New` - Get all items by operation  
- `inv_part_Get_Info_New` - Get detailed part information
- `inv_inventory_Get_Summary_New` - Get inventory summary

#### **Validation & Utility Procedures:**
- `inv_inventory_Validate_Stock_New` - Validate sufficient stock
- `inv_transaction_Log_New` - Log inventory transactions
- `inv_location_Validate_New` - Validate location exists
- `inv_operation_Validate_New` - Validate operation exists
- `sys_user_Validate_New` - Validate user exists

### UI Component Development
1. **Create Instruction File**: Document component in `UI_Documentation/`
2. **Follow Patterns**: Use established Avalonia and ReactiveUI patterns
3. **Implement Accessibility**: Include proper keyboard and screen reader support
4. **Test Responsiveness**: Verify layout works on different screen sizes

### Code Generation Workflow
1. **Use Custom Prompts**: Leverage templates in `Custom_Prompts/` for consistency
2. **Follow Examples**: Reference `Examples/` directory for implementation patterns
3. **Update Documentation**: Keep all README and instruction files current
4. **Validate Against Standards**: Ensure code meets project quality standards

## ?? Business Logic Rules

### Critical Transaction Logic ? **IMPLEMENTED IN NEW PROCEDURES**
**TransactionType is ALWAYS determined by user intent, NEVER by operation number:**

- **IN**: User is adding stock to inventory ? **inv_inventory_Add_Item_Enhanced**
- **OUT**: User is removing stock from inventory ? **inv_inventory_Remove_Item_Enhanced**
- **TRANSFER**: User is moving stock between locations ? **inv_inventory_Transfer_Item_New**

Operation numbers ("90", "100", "110", etc.) are **workflow step identifiers only** and do not determine transaction types.

### Database Interaction Patterns ? **READY FOR SERVICE LAYER**
```csharp
// ? CORRECT: Using new enhanced stored procedures
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item_Enhanced",
    new Dictionary<string, object>
    {
        ["p_PartID"] = "PART001",
        ["p_OperationID"] = "90",
        ["p_LocationID"] = "RECEIVING", 
        ["p_Quantity"] = 100,
        ["p_UserID"] = "admin"
    }
);

// Standard status handling for all new procedures
if (result.Status == 0)
{
    // Success
    return Result<bool>.Success(true, result.Message);
}
else if (result.Status == 1) 
{
    // Warning (validation issue)
    return Result<bool>.Warning(false, result.Message);
}
else
{
    // Error
    return Result<bool>.Error(result.Message);
}

// ? PROHIBITED: Direct SQL
// var query = "SELECT * FROM table"; // NEVER DO THIS
```

### Error Handling Standards ? **IMPLEMENTED IN ALL NEW PROCEDURES**
- **Comprehensive Logging**: All errors logged with full context ? **error_log table integration**
- **User-Friendly Messages**: Technical errors translated for users ? **descriptive p_ErrorMsg**
- **Graceful Degradation**: System continues operation when possible ? **status code system**
- **Transaction Rollback**: Automatic rollback on operation failures ? **EXIT HANDLER FOR SQLEXCEPTION**
- **Input Validation**: ? **All parameters validated before processing**
- **Business Rule Enforcement**: ? **Entity existence and constraint validation**

## ?? UI Development Standards

### Component Documentation Format
Each UI component requires a `.instructions.md` file with:
- **UI Element Name**: Clear, descriptive component name
- **Description**: Purpose and functionality overview
- **Visual Representation**: Layout and appearance details
- **Component Structure**: Hierarchical breakdown of controls
- **Props/Inputs**: Parameters and configuration options
- **Interactions/Events**: User interaction patterns
- **Business Logic**: Integration with backend services ? **Can now use new stored procedures**
- **Related Files**: Dependencies and integration points

### Design System Implementation
- **MTM Colors**: Consistent use of brand purple color palette
- **Modern Cards**: Clean, organized information presentation
- **Responsive Grids**: Flexible layouts that adapt to screen size
- **Progressive Disclosure**: Information hierarchy with appropriate detail levels

### Avalonia UI Patterns ? **READY FOR SERVICE INTEGRATION**
```xml
<!-- Compiled bindings with type safety -->
<UserControl x:DataType="vm:ViewModelType"
             x:CompileBindings="True">
    
    <!-- MTM brand styling -->
    <Border Classes="card" Background="{DynamicResource PrimaryBrush}">
        
        <!-- Reactive command binding -->
        <Button Command="{Binding SaveCommand}"
                Classes="primary"/>
    </Border>
</UserControl>
```

```csharp
// ViewModel ready for new stored procedure integration
public class InventoryViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> AddInventoryCommand { get; }
    
    public InventoryViewModel()
    {
        AddInventoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // ? Ready to use new stored procedures
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "inv_inventory_Add_Item_Enhanced",
                parameters
            );
            // Handle result.Status and result.Message
        });
    }
}
```

## ?? Quality Assurance

### Code Review Standards ? **ENHANCED**
- **Architecture Compliance**: Adherence to established patterns
- **Database Rules**: Verification of stored procedure usage ? **New procedures provide complete coverage**
- **UI Consistency**: Brand and design pattern compliance
- **Documentation**: Completeness of instruction and README files ? **Comprehensive documentation available**
- **Testing Coverage**: Adequate unit and integration test coverage
- **Error Handling**: ? **Standardized error handling pattern compliance**
- **Business Logic**: ? **MTM transaction type determination rules**

### Testing Requirements ? **READY FOR IMPLEMENTATION**
- **Unit Tests**: Individual component and service testing ? **Can test with new validation procedures**
- **Integration Tests**: Service interaction validation ? **New procedures ready for integration testing**
- **UI Tests**: User interface functionality verification
- **Database Tests**: Stored procedure validation and performance testing ? **12 procedures ready for testing**
- **Accessibility Tests**: Keyboard navigation and screen reader compatibility
- **Error Scenario Tests**: ? **Comprehensive error handling can be tested with new procedures**

### Performance Monitoring
- **Database Performance**: Query execution time monitoring ? **New procedures optimized for performance**
- **UI Responsiveness**: User interface interaction timing
- **Memory Usage**: Application memory consumption tracking
- **Error Rates**: System error frequency and pattern analysis ? **Enhanced with new error logging**

## ?? Continuous Improvement

### Documentation Maintenance ? **ENHANCED**
- **Regular Updates**: Keep all documentation current with code changes ? **New procedure documentation complete**
- **User Feedback**: Incorporate developer feedback into documentation
- **Best Practices**: Document successful patterns for reuse ? **Error handling patterns documented**
- **Lessons Learned**: Capture and share development insights

### Process Optimization ? **DEVELOPMENT ACCELERATED**
- **Development Efficiency**: Streamline common development tasks ? **Procedures ready for immediate use**
- **Code Generation**: Improve prompt templates and examples ? **New procedure patterns available**
- **Tool Integration**: Enhance development tool workflows
- **Knowledge Sharing**: Facilitate team knowledge transfer ? **Comprehensive documentation provided**

## ?? Integration Points

### External Dependencies
- **Avalonia UI**: Cross-platform UI framework
- **ReactiveUI**: MVVM framework for reactive programming
- **MySQL**: Database backend with stored procedures ? **Enhanced with 12 new procedures**
- **.NET 8**: Runtime platform and framework
- **Dependency Injection**: Built-in .NET DI container

### Internal Architecture ? **READY FOR ENHANCEMENT**
- **Services Layer**: Business logic and data access ? **Can now integrate with new stored procedures**
- **ViewModels**: UI state management and command handling ? **Ready for service layer integration**
- **Models**: Data structures and business entities ? **Ready for enhanced data operations**
- **Views**: User interface components and layouts
- **Extensions**: Utility methods and service registration

## ?? Development Guidelines

### Getting Started ? **STREAMLINED PROCESS**
1. **Review Documentation**: Read relevant README and instruction files ? **Enhanced documentation available**
2. **Understand Patterns**: Study examples in `Examples/` directory
3. **Follow Templates**: Use custom prompts for consistency ? **Compliance fix templates available**
4. **Validate Compliance**: Ensure adherence to established standards ? **New procedures provide compliance template**
5. **Update Documentation**: Keep files current with any changes

### Best Practices ? **ENHANCED WITH NEW STANDARDS**
- **Start with Documentation**: Document before implementing
- **Use Established Patterns**: Follow existing architectural decisions ? **New procedure patterns established**
- **Test Early and Often**: Implement tests alongside development ? **Validation procedures available for testing**
- **Seek Code Review**: Get feedback on architectural decisions
- **Maintain Consistency**: Follow established naming and styling conventions ? **MTM naming conventions established**
- **Follow Error Handling**: ? **Use standardized error handling pattern from new procedures**
- **Validate Business Rules**: ? **Use new validation procedures for entity checking**

### Common Pitfalls to Avoid ? **SOLUTIONS PROVIDED**
- **Direct SQL Usage**: Always use stored procedures ? **12 comprehensive procedures available**
- **Incorrect Transaction Logic**: Remember user intent determines transaction type ? **Implemented correctly in new procedures**
- **Missing Documentation**: Every component needs instruction files ? **Complete documentation provided**
- **UI Inconsistency**: Follow MTM brand and design patterns
- **Poor Error Handling**: Implement comprehensive error management ? **Standardized pattern available**
- **Missing Input Validation**: ? **All new procedures include comprehensive validation**
- **Inadequate Transaction Management**: ? **Proper transaction patterns implemented**

## ?? Development Status

### ? **CRITICAL MILESTONES COMPLETED**

#### **CRITICAL FIX #1 - COMPLETED** ?????
- **Issue**: Empty development stored procedure files blocking development
- **Solution**: ? **12 comprehensive stored procedures implemented**
- **Status**: ? **DEVELOPMENT UNBLOCKED**
- **Benefits**: 
  - All standard inventory operations available
  - Standardized error handling pattern established
  - Service layer integration points defined
  - Complete documentation provided

#### **Ready for Next Phase** ??
- **Fix #2**: Missing Standard Output Parameters ? **Template provided by Fix #1**
- **Fix #3**: Inadequate Error Handling ? **Pattern established by Fix #1**  
- **Fix #4-11**: Additional compliance fixes ? **Foundation laid by Fix #1**

### **Service Layer Development Ready** ??
All database operations can now be implemented using the new stored procedures:
- InventoryService ? **Ready for implementation**
- ValidationService ? **Validation procedures available**
- TransactionService ? **Transaction logging procedure available**
- UserService ? **User validation procedure available**

---

*This directory serves as the central hub for all development activities, providing the resources, documentation, and standards necessary for effective and consistent application development. With the completion of Critical Fix #1, all database development is now unblocked and ready for service layer implementation.*