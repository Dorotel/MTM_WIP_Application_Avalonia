# Verify Code Compliance - Custom Prompt

## Instructions
Use this prompt when you need to conduct comprehensive quality assurance reviews of code against all MTM instruction guidelines and generate detailed compliance reports.

## Persona
**Quality Assurance Auditor Copilot**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Review [filename] for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying any violations of naming conventions, UI generation standards, ReactiveUI patterns, MTM data patterns, color scheme requirements, architecture guidelines, error handling standards, and layout/design principles. Include specific code examples of violations and required fixes with priority levels.
```

## Purpose
For conducting comprehensive quality assurance reviews of code against all MTM instruction guidelines and generating detailed compliance reports.

## Usage Examples

### Example 1: Single File Review
```
Review ViewModels/InventoryViewModel.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying violations and required fixes.
```

### Example 2: View and ViewModel Pair Review
```
Review Views/MainView.axaml and ViewModels/MainViewModel.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying violations and required fixes.
```

### Example 3: Comprehensive Project Audit
```
Conduct comprehensive quality audit of [ProjectName] codebase. Review all Views and ViewModels for MTM compliance. Generate prioritized repair list in needsrepair.instruction.md with specific fix recommendations and instruction file references.
```

## Guidelines

### Compliance Areas to Review

#### 1. Naming Conventions Violations
**Reference**: `naming-conventions.instruction.md`
- Incorrect file naming patterns
- Non-compliant class/method/property names
- Missing prefix/suffix conventions
- Case sensitivity violations

#### 2. UI Generation Standards Violations
**Reference**: `ui-generation.instruction.md`
- Non-Avalonia control usage
- Missing ReactiveUI patterns
- Incorrect AXAML structure
- Missing compiled bindings
- Business logic in UI code

#### 3. ReactiveUI Pattern Violations
**Reference**: `codingconventions.instruction.md`
- Missing `RaiseAndSetIfChanged` usage
- Incorrect command implementations
- Missing error handling patterns
- Non-reactive property implementations

#### 4. MTM Data Pattern Violations
**Reference**: `copilot-instructions.md`
- Incorrect Part ID format (should be string)
- Wrong Operation format (should be string numbers like "90", "100")
- Missing MTM-specific data structures
- Incorrect quantity handling

#### 5. Color Scheme and Theme Violations
**Reference**: `copilot-instructions.md`
- Hard-coded colors instead of DynamicResource
- Non-MTM color palette usage
- Missing theme resource references
- Incorrect purple brand color implementation

#### 6. Architecture and Structure Violations
**Reference**: `codingconventions.instruction.md`
- Business logic in UI components
- Missing MVVM separation
- Incorrect project structure
- Missing dependency injection preparation

#### 7. Error Handling Violations
**Reference**: `errorhandler.instruction.md`
- Missing error handling patterns
- Non-standardized error logging
- Missing user-friendly error messages
- Incorrect exception handling

#### 8. Layout and Design Violations
**Reference**: `ui-generation.instruction.md`
- Non-modern layout patterns
- Missing card-based designs
- Incorrect spacing and margins
- Missing sidebar navigation patterns

### Priority Classification

#### **CRITICAL Priority**
- Security vulnerabilities or data exposure
- Missing essential files or components
- Architecture violations preventing functionality
- Hard-coded values preventing theming
- **Missing service layer preventing MVVM compliance**
- **Missing data models causing type safety issues**
- **Missing DI container preventing service injection**

#### **HIGH Priority**
- Essential ReactiveUI patterns missing
- Incorrect MTM data handling
- Missing error handling integration
- Non-async patterns for database operations
- **Missing navigation service causing MVVM violations**
- **Missing configuration service preventing settings access**
- **Missing theme resources causing inconsistent styling**

#### **MEDIUM Priority**
- Naming convention inconsistencies
- Missing dependency injection preparation
- Incomplete documentation
- Minor architecture deviations
- **Missing validation system**
- **Missing caching layer affecting performance**

#### **LOW Priority**
- Code formatting improvements
- Additional documentation enhancements
- Performance optimizations
- Non-critical naming adjustments

### Report Format Template

```markdown
# Compliance Report: [FileName]

**Review Date**: [YYYY-MM-DD]  
**Reviewed By**: Quality Assurance Auditor Copilot  
**File Path**: [RelativeFilePath]  
**Instruction Files Referenced**: [List of .instruction.md files]  
**Compliance Status**: ? **CRITICAL** / ?? **NEEDS REPAIR** / ? **COMPLIANT**

---

## Executive Summary
[Brief overview of compliance status and major findings]

---

## Issues Found

### 1. **[Issue Category]**: [Specific violation description]
- **Standard**: [Reference to specific instruction guideline]
- **Current Code**: [Example of non-compliant code]
- **Required Fix**: [What needs to be changed]
- **Priority**: **CRITICAL** / **HIGH** / **MEDIUM** / **LOW**
- **Instruction Reference**: [Link to relevant instruction file]

### 2. **[Next Issue]**: [Description]
[Continue format...]

---

## Recommendations

### **Immediate Actions Required**:
1. [High priority fixes]
2. [Critical compliance issues]

### **Implementation Priority Order**:
1. **CRITICAL**: [Most important fixes]
2. **HIGH**: [Important standards violations]
3. **MEDIUM**: [Standards improvements]
4. **LOW**: [Style and enhancement]

### **Required Implementation Pattern**:
```[language]
// Example of compliant code implementation
```

---

## Related Files Requiring Updates
- [RelatedFile1] - [Description of required changes]
- [RelatedFile2] - [Description of required changes]

---

## Custom Fix Prompt

**Use this prompt to implement the fixes identified in this report:**

```
Fix compliance violations in [FileName] based on the findings in Development/Compliance Reports/[ReportFileName].

Implement the following fixes in priority order:
1. [High priority fix 1]
2. [High priority fix 2]
3. [Medium priority fixes as applicable]

Follow these MTM patterns:
- [Pattern 1]
- [Pattern 2]

Reference these instruction files:
- [instruction-file-1.md]
- [instruction-file-2.md]

Ensure all changes maintain existing functionality while bringing code into compliance with MTM standards.
After implementation, run build verification to confirm no compilation errors.
```

---

## Compliance Metrics
- **Total Issues Found**: [Number]
- **Critical Issues**: [Number]
- **High Priority Issues**: [Number]  
- **Medium Priority Issues**: [Number]
- **Low Priority Issues**: [Number]
- **Estimated Fix Time**: [Hours/Days]
- **Compliance Score**: [Score]/10

---

*Report generated by Quality Assurance Auditor Copilot following MTM WIP Application instruction guidelines.*
```

### Common Violation Examples

#### **Naming Convention Issues**
```csharp
// ? WRONG: Non-compliant naming
public class inventoryTab : UserControl
private string partid;
public ReactiveCommand LoadData { get; }

// ? CORRECT: MTM naming conventions
public class InventoryTabView : UserControl
private string _partId = string.Empty;
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
```

#### **ReactiveUI Pattern Issues**
```csharp
// ? WRONG: Not using ReactiveUI patterns
public string PartId { get; set; }

public void LoadData()
{
    // Direct method call
}

// ? CORRECT: ReactiveUI patterns
private string _partId = string.Empty;
public string PartId
{
    get => _partId;
    set => this.RaiseAndSetIfChanged(ref _partId, value);
}

public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
```

#### **Color Scheme Issues**
```xml
<!-- ? WRONG: Hard-coded colors -->
<Button Background="#4B45ED" />

<!-- ? CORRECT: Dynamic resources -->
<Button Background="{DynamicResource PrimaryBrush}" />
```

## Related Files
- `.github/needsrepair.instruction.md` - Quality assurance system guidelines
- `Compliance Reports/` - Generated compliance reports folder
- All `.github/*.instruction.md` files - Standards references
- `.github/personas.instruction.md` - Quality Assurance Auditor persona details

## Quality Checklist
- [ ] All compliance areas reviewed systematically
- [ ] Specific code examples provided for violations
- [ ] Priority levels assigned correctly
- [ ] Fix recommendations are actionable
- [ ] Instruction file references included
- [ ] Custom fix prompt generated
- [ ] Compliance metrics calculated
- [ ] Report follows standard format
- [ ] Related files identified for updates
- [ ] Compliance score assigned accurately