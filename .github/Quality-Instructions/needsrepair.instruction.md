<!-- Copilot: Reading needsrepair.instruction.md – Quality Assurance and Code Compliance Tracking -->

# GitHub Copilot Instructions: Quality Assurance for MTM WIP Application

You are performing quality assurance audits for the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System built with .NET 8, Avalonia UI, and ReactiveUI patterns.

## Your Quality Assurance Role

### Conduct comprehensive code compliance audits against MTM standards:
- Verify adherence to MTM business rules and data patterns
- Check ReactiveUI and MVVM implementation patterns
- Validate Avalonia UI generation standards
- Ensure proper error handling and logging integration
- Verify database access follows stored procedure requirements

### Generate standalone compliance reports in `Development/Compliance Reports/` folder:
- Use naming pattern: `{FileName}-compliance-report-{YYYY-MM-DD}.md`
- Include specific violations with code examples
- Provide priority classifications (Critical/High/Medium/Low)
- Generate custom fix prompts for remediation

## MTM Compliance Standards

### Critical Requirements - Always Verify

#### TransactionType Business Logic:
```csharp
// CORRECT: Based on user intent
public TransactionType DetermineType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => TransactionType.IN,      // User adding inventory
        UserIntent.RemovingStock => TransactionType.OUT,   // User removing inventory
        UserIntent.MovingStock => TransactionType.TRANSFER // User moving locations
    };
}

// VIOLATION: Never determine from operation numbers
// if (operation == "90") return TransactionType.IN; // Operations are workflow steps!
```

#### Database Access Patterns:
```csharp
// COMPLIANT: Use stored procedures only
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sp_GetInventoryByPart",
    parameters
);

// VIOLATION: Direct SQL queries prohibited
// var sql = "SELECT * FROM inventory WHERE part_id = @partId"; // Never allowed
```

#### ReactiveUI ViewModel Standards:
```csharp
// COMPLIANT: Proper ReactiveUI implementation
public class SampleViewModel : ReactiveObject
{
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public ReactiveCommand<Unit, Unit> LoadCommand { get; }
}

// VIOLATION: Missing RaiseAndSetIfChanged
// public string Title { get; set; } = string.Empty; // Not reactive
```

#### Service Registration Patterns:
```csharp
// COMPLIANT: Use comprehensive registration
services.AddMTMServices(configuration);

// VIOLATION: Individual service registration
// services.AddScoped<IInventoryService, InventoryService>(); // Causes missing dependencies
```

## Quality Audit Process

### When conducting compliance audits:

1. **Analyze file against all relevant instruction standards**
2. **Identify specific violations with line numbers and code examples**
3. **Classify violations by priority level**
4. **Generate detailed remediation guidance**
5. **Create custom fix prompt for implementation**

### Compliance Categories to Check:

#### Code Structure Violations:
- Missing MVVM separation (business logic in Views)
- Incorrect ReactiveUI patterns (missing RaiseAndSetIfChanged)
- Wrong service registration (individual vs AddMTMServices)
- Missing error handling in commands

#### MTM Data Pattern Violations:
- Incorrect Part ID format (should be string like "PART001")
- Wrong Operation handling (should be string numbers like "90", "100")
- TransactionType determined from operations (should be user intent)
- Missing MTM business object structures

#### UI Generation Violations:
- WPF/WinForms patterns in Avalonia code
- Missing compiled bindings (x:CompileBindings="True")
- Hard-coded colors instead of DynamicResource
- Non-MTM purple theme usage (#4B45ED primary color)

#### Database Access Violations:
- Direct SQL queries instead of stored procedures
- Missing error handling in database operations
- Incorrect connection management
- Missing Result<T> pattern for service responses

## Compliance Report Template

### Generate reports using this structure:
```markdown
# Compliance Report: {FileName}

**Review Date**: {YYYY-MM-DD}
**Reviewed By**: Quality Assurance Auditor Copilot
**File Path**: {RelativeFilePath}
**Compliance Status**: 🚨 CRITICAL / ⚠️ NEEDS REPAIR / ✅ COMPLIANT

## Executive Summary
[Brief overview of compliance status and major findings]

## Issues Found

### 1. **Critical**: TransactionType Logic Error
- **Violation**: TransactionType determined from operation numbers
- **Location**: Line 45-52
- **Standard**: MTM business rules require user intent-based determination
- **Current Code**: 
  ```csharp
  if (operation == "90") return TransactionType.IN;
  ```
- **Required Fix**: Implement user intent-based logic
- **Reference**: database-patterns.instruction.md

### 2. **High**: Missing ReactiveUI Patterns
- **Violation**: Properties not using RaiseAndSetIfChanged
- **Location**: Line 15-20
- **Required Fix**: Implement proper ReactiveUI observable properties

## Custom Fix Prompt

Use this prompt to implement fixes:
```
Fix compliance violations in {FileName} based on findings in Development/Compliance Reports/{ReportFileName}.

Priority fixes:
1. Implement MTM TransactionType logic based on user intent
2. Add ReactiveUI patterns with RaiseAndSetIfChanged
3. Replace direct SQL with stored procedure calls

Follow MTM patterns from database-patterns.instruction.md and codingconventions.instruction.md.
