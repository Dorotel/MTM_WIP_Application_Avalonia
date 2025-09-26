# MTM New View Implementation Guide
**Complete Guide for Creating New Views in the MTM WIP Application**

---

## 📋 Overview

This guide provides step-by-step instructions for implementing completely new views in the MTM WIP Application. Use this when creating new functionality that doesn't exist in the current system.

## 🎯 Pre-Implementation Planning Questions

Before starting implementation, answer these critical questions to ensure proper planning:

### Business Requirements
1. **What is the primary business function this view will serve?**
   - Inventory management operation
   - Manufacturing workflow step
   - Reporting and analytics
   - Configuration and settings
   - User management
   - Other (specify)

2. **Who are the primary users of this view?**
   - Shop floor operators
   - Manufacturing supervisors
   - Inventory managers
   - System administrators
   - Quality control staff
   - Other (specify)

3. **What manufacturing domain concepts does this view handle?**
   - Part IDs and operations
   - Inventory quantities and locations
   - Transaction processing
   - Master data management
   - Error logging and reporting
   - Other (specify)

### Technical Architecture
4. **What is the primary view type?**
   - Tab view within MainWindow
   - Standalone dialog window
   - Settings panel
   - Report display
   - Data entry form
   - Dashboard/summary view

5. **What data sources will this view require?**
   - Master data (parts, operations, locations)
   - Inventory data
   - Transaction history
   - Configuration settings
   - User data
   - External system data
   - Other (specify)

6. **What database operations are needed?**
   - Read operations only
   - Create new records
   - Update existing records
   - Delete records
   - Complex reporting queries
   - Bulk operations

7. **What services need to be created or modified?**
   - New dedicated service required
   - Extend existing MasterDataService
   - Extend existing InventoryService
   - No new services needed
   - Other (specify)

### UI/UX Requirements
8. **What is the expected layout pattern?**
   - Standard MTM tab layout (ScrollViewer + Grid)
   - Master-detail layout
   - List view with actions
   - Form-based layout
   - Dashboard with cards
   - Other (specify)

9. **What input controls are required?**
   - Text boxes for data entry
   - Dropdown/ComboBox selections
   - Date/time pickers
   - Numeric input controls
   - Checkboxes and radio buttons
   - File selection controls
   - Other (specify)

10. **What actions/commands will users perform?**
    - Save/create operations
    - Update/edit operations
    - Delete operations
    - Search and filter
    - Export data
    - Print reports
    - Navigate to related views
    - Other (specify)

### Integration Requirements
11. **How does this view integrate with existing navigation?**
    - New tab in MainWindow
    - Accessible from menu system
    - Modal dialog from existing view
    - Standalone window
    - Context menu action
    - Other (specify)

12. **What error handling scenarios need consideration?**
    - Database connection failures
    - Validation errors
    - Permission/access issues
    - External system integration failures
    - Data consistency issues
    - Other (specify)

---

## 🏗️ Implementation Steps

### Phase 1: Setup and Planning
1. **Create the basic file structure**
   - ViewModel: `ViewModels/[Area]/[Name]ViewModel.cs`
   - View: `Views/[Area]/[Name]View.axaml`
   - Code-behind: `Views/[Area]/[Name]View.axaml.cs`

2. **Define the data models**
   - Create DTOs in `Models/` folder
   - Define request/response models
   - Set up validation attributes

3. **Plan the service layer**
   - Identify required stored procedures
   - Design service interfaces
   - Plan error handling strategies

### Phase 2: Core Implementation
1. **Implement the ViewModel**
   - Use MVVM Community Toolkit patterns
   - Implement `[ObservableProperty]` for data binding
   - Create `[RelayCommand]` for user actions
   - Add proper constructor with DI

2. **Create the AXAML view**
   - Use correct Avalonia namespace
   - Implement MTM tab layout pattern
   - Apply MTM theme resources
   - Set up proper data binding

3. **Implement minimal code-behind**
   - Standard UserControl pattern
   - Resource cleanup in OnDetachedFromVisualTree

### Phase 3: Service Integration
1. **Create or extend services**
   - Implement database operations using stored procedures
   - Add proper error handling
   - Return empty collections on failure (no fallback data)

2. **Register services in DI**
   - Add to `ServiceCollectionExtensions.cs`
   - Use appropriate service lifetimes
   - Register ViewModel as transient

### Phase 4: Testing and Integration
1. **Unit testing**
   - Test ViewModel logic
   - Test service operations
   - Test command execution

2. **Integration testing**
   - Test database operations
   - Test UI data binding
   - Test error scenarios

3. **UI testing**
   - Verify MTM theme compliance
   - Test responsive layout
   - Validate accessibility

---

## 📋 Implementation Checklist

### Planning Phase ✅
- [ ] Business requirements clearly defined
- [ ] Technical architecture planned
- [ ] UI/UX requirements specified
- [ ] Integration points identified
- [ ] Required stored procedures identified

### File Structure ✅
- [ ] ViewModel created with proper namespace
- [ ] AXAML view created with correct header
- [ ] Code-behind implements minimal pattern
- [ ] Models created for data structures
- [ ] Services planned or extended

### Implementation ✅
- [ ] MVVM Community Toolkit patterns used
- [ ] MTM tab layout pattern implemented
- [ ] Theme resources used for all colors
- [ ] Stored procedures used for database access
- [ ] Centralized error handling implemented

### Testing ✅
- [ ] No AVLN2000 compilation errors
- [ ] Unit tests created and passing
- [ ] Integration tests verify functionality
- [ ] UI displays correctly
- [ ] Error handling works properly

### Integration ✅
- [ ] Services registered in DI container
- [ ] Navigation integration complete
- [ ] Database operations tested
- [ ] Theme consistency maintained
- [ ] Performance acceptable

---

## 🎨 MTM Design Patterns to Follow

### Mandatory Layout Pattern
```xml
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}">
      <!-- Content here -->
    </Border>
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}">
      <!-- Actions here -->
    </Border>
  </Grid>
</ScrollViewer>
```

### ViewModel Pattern
```csharp
[ObservableObject]
public partial class [Name]ViewModel : BaseViewModel
{
    [ObservableProperty]
    private string inputValue = string.Empty;
    
    [RelayCommand]
    private async Task ExecuteAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Implementation
        });
    }
}
```

### Database Pattern
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "stored_procedure_name",
    parameters
);
```

---

## 🚨 Common Pitfalls to Avoid

1. **AVLN2000 Errors**: Never use `Name` on Grid - use `x:Name`
2. **Wrong Namespace**: Always use `xmlns="https://github.com/avaloniaui"`
3. **Fallback Data**: Never provide dummy data on database failures
4. **Direct SQL**: Always use stored procedures
5. **StaticResource for Themes**: Use `DynamicResource` for theme colors
6. **ReactiveUI Patterns**: Use MVVM Community Toolkit only

---

**Reference Files**: 
- Main Implementation Guide: `docs/development/view-management-md-files/MTM-View-Implementation-Guide.md`
- Templates: `.github/copilot/templates/`
- Patterns: `.github/copilot/patterns/`
- Instructions: `.github/instructions/`


## 🚀 Joyride Development Enhancement

**Priority Directive**: Leverage Joyride automation for enhanced MTM development workflows.

### Core Joyride Tools
- **`joyride_evaluate_code`**: Execute ClojureScript in VS Code Extension Host
- **`joyride_request_human_input`**: Interactive decision-making workflows
- **`joyride_basics_for_agents`**: Access automation patterns and capabilities
- **`joyride_assisting_users_guide`**: User assistance automation

### MTM-Optimized Automation
- **Pattern Enforcement**: Automated MVVM Community Toolkit validation
- **Domain Validation**: Manufacturing operation codes and business rules
- **UI Consistency**: Avalonia theme system and component standards
- **Database Integration**: MySQL connection testing and stored procedure validation
- **Cross-Platform**: Automated testing across Windows/macOS/Linux

**Implementation**: Use Joyride first when safe, fall back to traditional tools as needed.