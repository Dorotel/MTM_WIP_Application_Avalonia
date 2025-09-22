# MTM WIP Application - Documentation Standards

**Framework**: .NET 8 with C# 12  
**Documentation Standard**: XML Documentation Comments + Inline Comments  
**Created**: January 6, 2025  
**Status**: Active Development Standard

---

## 📋 Documentation Quality Gates

### Current Status
- **Files Documented**: 80+ out of 111 C# files (72%+ coverage)
- **Target Coverage**: 95% (105+ files documented)
- **Documentation Quality**: High (comprehensive XML comments with business context)
- **Maintenance**: Automated reviews via GitHub Actions

---

## 🎯 XML Documentation Standards

### **Class Documentation Pattern**
```csharp
/// <summary>
/// [Brief description of the class purpose]
/// [Explanation of its role in the MTM manufacturing domain]
/// [Technical implementation details if complex]
/// [Usage patterns and dependencies]
/// [Business context specific to manufacturing operations]
/// </summary>
public partial class ExampleViewModel : BaseViewModel
{
    // Implementation
}
```

### **Method Documentation Pattern**
```csharp
/// <summary>
/// [Description of what the method does]
/// [Business logic explanation if applicable]
/// [Important error handling notes]
/// </summary>
/// <param name="parameterName">Description of parameter and its business meaning</param>
/// <returns>Description of return value and its business significance</returns>
/// <exception cref="ExceptionType">When this exception is thrown</exception>
public async Task<ServiceResult<InventoryItem>> AddInventoryAsync(string partId, string operation, int quantity)
{
    // Implementation
}
```

---

## 🏭 Manufacturing Domain Context

### **Business Term Documentation**
All documentation should include manufacturing domain context:

- **Part ID**: Unique identifier for manufactured parts (e.g., "PART001", "ABC-123")
- **Operation**: Manufacturing workflow step (e.g., "90" = Receiving, "100" = First Op)
- **Location**: Physical manufacturing area where operations occur
- **Transaction Types**: "IN" (receiving), "OUT" (shipping), "TRANSFER" (between operations)
- **Inventory**: Work-in-process tracking for manufacturing operations

---

## 📝 View Code-Behind Documentation

### **Minimal Code-Behind Pattern**
```csharp
/// <summary>
/// Code-behind for [ViewName].
/// Implements the [business function] interface within the MTM WIP Application.
/// [Specific manufacturing purpose - e.g., inventory management, part tracking]
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// </summary>
public partial class ExampleView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the [ViewName].
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public ExampleView()
    {
        InitializeComponent();
    }
}
```

---

## 📋 Architecture Decision Documentation

### **Recent ADRs**
- **ADR-001**: MVVM Community Toolkit over ReactiveUI
- **ADR-002**: Stored Procedures Only database pattern
- **ADR-003**: Category-based service organization
- **ADR-004**: Windows 11 Blue primary color
- **ADR-005**: Comprehensive XML documentation standards

---

**Document Status**: ✅ Active Development Standard  
**Coverage Target**: 95% XML documentation coverage  
**Last Updated**: January 6, 2025  
**Maintained By**: MTM Development Team

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

