# Architectural Decision Records (ADRs) - MTM WIP Application

## 📋 Overview

This document contains all architectural decision records for the MTM WIP Application, providing context and rationale for major technical choices made throughout the development process.

## 🏗️ **ADR Index**

| ADR # | Title | Status | Date |
|-------|-------|--------|------|
| ADR-001 | Avalonia UI Framework Selection | Accepted | 2024-10-01 |
| ADR-002 | MVVM Community Toolkit Migration | Accepted | 2024-11-15 |
| ADR-003 | Stored Procedures Only Database Pattern | Accepted | 2024-10-01 |
| ADR-004 | Category-Based Service Consolidation | Accepted | 2024-11-01 |
| ADR-005 | Windows 11 Blue Design System | Accepted | 2024-12-01 |
| ADR-006 | GitHub Actions Automation Architecture | Accepted | 2024-12-15 |

---

## **ADR-001: Avalonia UI Framework Selection**

**Status**: Accepted  
**Date**: 2024-10-01  
**Participants**: Development Team, Architecture Committee

### **Context**
The MTM WIP Application required a cross-platform desktop UI framework for manufacturing inventory management. The application needed to support Windows primarily, with potential future Linux deployment for manufacturing environments.

### **Decision**
Selected **Avalonia UI 11.3.4** as the primary UI framework over WPF, WinUI 3, and Electron alternatives.

### **Rationale**
- **Cross-platform compatibility**: Native Linux support for manufacturing environments
- **XAML familiarity**: Leverages team's existing WPF/XAML expertise
- **Performance**: Native performance better than web-based alternatives (Electron)
- **Styling flexibility**: Advanced theming capabilities for MTM branding requirements
- **Active development**: Strong community and regular updates
- **Modern patterns**: Support for latest .NET 8 features and MVVM patterns

### **Consequences**
**Positive:**
- Cross-platform deployment capability
- High performance native UI
- Familiar XAML development patterns
- Strong theming and styling support

**Negative:**
- Smaller ecosystem compared to WPF
- Some third-party control limitations
- Learning curve for Avalonia-specific patterns (Grid syntax differences)

### **Implementation Notes**
- Use `xmlns="https://github.com/avaloniaui"` namespace (not WPF)
- Grid column definitions use attribute form when possible
- `x:Name` required for Grid definitions (not `Name` property)

---

## **ADR-002: MVVM Community Toolkit Migration**

**Status**: Accepted  
**Date**: 2024-11-15  
**Participants**: Development Team

### **Context**
The application originally used ReactiveUI for MVVM implementation. However, ReactiveUI added complexity with reactive programming patterns that were not well-suited for the straightforward manufacturing workflows. The team needed a simpler, more maintainable MVVM solution.

### **Decision**
Migrate from **ReactiveUI** to **MVVM Community Toolkit 8.3.2** for all ViewModel implementations.

### **Rationale**
- **Simplicity**: Source generators eliminate boilerplate code
- **Performance**: Compile-time code generation vs runtime reflection
- **Maintainability**: Straightforward property and command patterns
- **Microsoft support**: Official Microsoft toolkit with long-term support
- **Team familiarity**: Closer to traditional MVVM patterns team understands
- **Debugging**: Easier to debug than reactive stream compositions

### **Migration Strategy**
1. **Phase 1**: Create base ViewModel pattern with MVVM Community Toolkit
2. **Phase 2**: Migrate all 42 ViewModels to new pattern
3. **Phase 3**: Remove all ReactiveUI dependencies
4. **Phase 4**: Update all Views to use standard binding patterns

### **Consequences**
**Positive:**
- Reduced complexity in ViewModel implementations
- Better performance with source generators
- Easier onboarding for new developers
- Consistent patterns across all ViewModels

**Negative:**
- Migration effort required for all existing ViewModels
- Loss of reactive programming benefits (not needed for this application)
- Learning curve for MVVM Community Toolkit specifics

### **Implementation Pattern**
```csharp
[ObservableObject]
public partial class ExampleViewModel : BaseViewModel
{
    [ObservableProperty]
    private string property = string.Empty;

    [RelayCommand]
    private async Task ActionAsync()
    {
        // Implementation
    }
}
```

---

## **ADR-003: Stored Procedures Only Database Pattern**

**Status**: Accepted  
**Date**: 2024-10-01  
**Participants**: Database Team, Development Team, Security Team

### **Context**
The MTM manufacturing environment requires high security, audit trails, and consistent data access patterns. The application needed a database access strategy that ensures security, performance, and maintainability.

### **Decision**
Implement a **stored procedures only** database access pattern using `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`.

### **Rationale**
- **Security**: Eliminates SQL injection risks completely
- **Performance**: Pre-compiled execution plans
- **Audit trails**: All operations logged at database level
- **Consistency**: Single pattern for all database operations
- **Change control**: Database schema changes controlled through versioned procedures
- **Business logic centralization**: Complex business rules implemented in database layer
- **Manufacturing compliance**: Meets audit requirements for manufacturing environments

### **Implementation Pattern**
```csharp
var parameters = new MySqlParameter[]
{
    new("p_PartID", partId),
    new("p_Operation", operation),
    new("p_Quantity", quantity)
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    parameters
);
```

### **Consequences**
**Positive:**
- Maximum security against SQL injection
- Consistent error handling and logging
- Optimal database performance
- Clear separation of concerns
- Audit-compliant operations

**Negative:**
- Database deployment complexity (45+ stored procedures)
- Limited ORM benefits (no Entity Framework)
- Requires database development skills
- More complex unit testing (database dependencies)

### **45+ Stored Procedures Catalog**
- **Inventory**: `inv_inventory_*` (12 procedures)
- **Transactions**: `inv_transaction_*` (8 procedures)  
- **Master Data**: `md_*` (15 procedures)
- **Error Logging**: `log_error_*` (4 procedures)
- **System Maintenance**: `sys_maintenance_*` (6+ procedures)

---

## **ADR-004: Category-Based Service Consolidation**

**Status**: Accepted  
**Date**: 2024-11-01  
**Participants**: Development Team, Architecture Committee

### **Context**
The application initially had many small, single-purpose services leading to excessive dependency injection complexity and difficult service discovery. A more organized approach was needed.

### **Decision**
Implement **category-based service consolidation** where related functionality is grouped into comprehensive service files.

### **Rationale**
- **Reduced complexity**: Fewer service registrations and dependencies
- **Better discoverability**: Related functionality grouped together
- **Easier maintenance**: Single file contains related operations
- **Clearer ownership**: Each category has clear responsibility boundaries
- **Dependency reduction**: Less circular dependency issues

### **Service Organization Pattern**
```
Services/
├── Configuration.cs          # ConfigurationService + ApplicationStateService
├── Database.cs              # Database connection and helper methods
├── ErrorHandling.cs         # ErrorHandling + ErrorEntry + ErrorConfiguration
├── MasterDataService.cs     # All master data operations
├── Navigation.cs            # Navigation and routing services
├── QuickButtons.cs          # Quick button management
└── ThemeService.cs          # Theme management and application
```

### **Implementation Example**
```csharp
// File: Services/Configuration.cs
namespace MTM_WIP_Application_Avalonia.Services
{
    public class ConfigurationService : IConfigurationService { /* implementation */ }
    public class ApplicationStateService : IApplicationStateService { /* implementation */ }
}
```

### **Consequences**
**Positive:**
- Simplified service registration
- Better code organization
- Easier to find related functionality
- Reduced dependencies between services

**Negative:**
- Larger service files require careful organization
- Potential for services to become too large
- Need clear boundaries within consolidated services

---

## **ADR-005: Windows 11 Blue Design System**

**Status**: Accepted  
**Date**: 2024-12-01  
**Participants**: UX Team, Development Team, MTM Stakeholders

### **Context**
The application needed a consistent design system that aligned with modern UI expectations while maintaining professional appearance suitable for manufacturing environments.

### **Decision**
Adopt **Windows 11 Blue (#0078D4)** as the primary color with a **card-based layout system** and comprehensive theme support.

### **Rationale**
- **Modern appearance**: Aligns with Windows 11 design language
- **Professional**: Suitable for manufacturing/enterprise environments
- **Accessibility**: High contrast ratios and WCAG compliance
- **Brand consistency**: MTM can overlay brand colors while maintaining base system
- **Theme flexibility**: Support for MTM_Blue, MTM_Green, MTM_Red, MTM_Dark variants

### **Design System Specifications**
```xml
<!-- Primary Colors -->
Primary Blue: #0078D4
Secondary Blue: #106EBE
Background: White (#FFFFFF)
Border: #E0E0E0
Text: #323130

<!-- Spacing System -->
Small: 8px margins/padding
Medium: 16px for cards and forms
Large: 24px for section separation

<!-- Card Pattern -->
<Border Background="White" 
        BorderBrush="#E0E0E0" 
        BorderThickness="1" 
        CornerRadius="8" 
        Padding="16" />
```

### **Theme Service Architecture**
- `ThemeService` manages theme switching
- Theme resources stored in `Resources/Themes/`
- Runtime theme switching supported
- User preferences persisted in configuration

### **Consequences**
**Positive:**
- Consistent professional appearance
- Excellent accessibility
- Easy theme customization
- Modern UI expectations met

**Negative:**
- Custom styling required for unique MTM branding
- Theme switching adds complexity
- Careful color contrast management needed

---

## **ADR-006: GitHub Actions Automation Architecture**

**Status**: Accepted  
**Date**: 2024-12-15  
**Participants**: DevOps Team, Development Team

### **Context**
The project required comprehensive automation for code quality, documentation, issue management, and development workflow optimization.

### **Decision**
Implement a **comprehensive GitHub Actions automation system** with 5 specialized workflows for different aspects of development lifecycle.

### **Rationale**
- **Developer productivity**: Automate repetitive tasks and quality checks
- **Consistency**: Standardized processes across all development activities
- **Quality assurance**: Automated code quality gates and security scanning
- **Documentation**: Keep documentation synchronized with code changes
- **Project management**: Automated issue creation and sprint planning

### **Workflow Architecture**
```
.github/workflows/
├── automated-issue-creation.yml    # Daily technical debt analysis
├── code-quality-gates.yml         # SonarQube/CodeQL integration
├── pr-status-automation.yml       # PR lifecycle management
├── sprint-planning-automation.yml # Weekly sprint automation
└── technical-debt-automation.yml  # Comprehensive codebase analysis
```

### **Integration Points**
- **GitHub Projects**: Sprint planning and issue tracking
- **SonarCloud**: Code quality and security analysis
- **CodeQL**: Security vulnerability scanning
- **Artifact Storage**: Quality reports and analysis results
- **PR Comments**: Automated quality feedback

### **Consequences**
**Positive:**
- 80% reduction in manual development overhead
- Consistent quality gates across all code changes
- Automated technical debt monitoring
- Comprehensive project management automation

**Negative:**
- Complex workflow maintenance requirements
- GitHub Actions compute costs
- Potential workflow failures blocking development
- Learning curve for workflow customization

### **Quality Gates Implementation**
- **Code Coverage**: Minimum 70% (warning at 60%)
- **Security**: No critical vulnerabilities allowed
- **Complexity**: Cyclomatic complexity monitoring
- **Dependencies**: Automated vulnerability scanning

---

## 🔄 **Decision Review Process**

### **When to Create an ADR**
- Major architectural changes
- Technology stack decisions
- Significant design pattern changes
- Integration approach selections
- Security or compliance decisions

### **ADR Review Cycle**
- **Quarterly**: Review all accepted ADRs for continued relevance
- **Major releases**: Evaluate ADR impacts on new features
- **Technology updates**: Assess if dependencies affect existing decisions

### **ADR Status Lifecycle**
- **Proposed**: Under discussion and review
- **Accepted**: Approved and implemented
- **Deprecated**: Replaced by newer decision
- **Superseded**: Replaced by specific ADR

### **Documentation Standards**
Each ADR must include:
- Clear context and problem statement
- Decision with specific technology/approach
- Detailed rationale with pros/cons analysis
- Implementation consequences and trade-offs
- Code examples where applicable

This ADR collection provides comprehensive documentation of all major architectural decisions in the MTM WIP Application, ensuring future developers understand the reasoning behind technical choices and can make informed decisions about modifications or extensions.


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