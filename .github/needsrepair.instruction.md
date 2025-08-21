<!-- Copilot: Reading needsrepair.instruction.md — Quality Assurance and Code Compliance Tracking -->

# Needs Repair - Code Quality Assurance Report

## Overview
This instruction file serves as a **quality assurance report system** for identifying code that does not meet the standards defined in the MTM WIP Application instruction files. When Copilot is asked to verify code compliance, it will generate reports in this file documenting what needs to be fixed to meet project standards.

## Purpose
- **Code Standards Verification**: Track files that don't comply with instruction guidelines
- **Quality Assurance Reports**: Document specific violations and required fixes
- **Compliance Tracking**: Monitor progress on bringing code up to standards
- **Instruction File Adherence**: Ensure all code follows established patterns and conventions

---

## Report Format Template

When generating compliance reports, use this format:

### **File: [FilePath]**
**Review Date**: [Date]  
**Reviewed By**: [Copilot Persona]  
**Instruction Files Referenced**: [List of relevant .instruction.md files]  
**Compliance Status**: ? **NEEDS REPAIR** / ?? **PARTIAL COMPLIANCE** / ? **COMPLIANT**

#### **Issues Found**:
1. **[Issue Category]**: [Specific violation description]
   - **Standard**: [Reference to specific instruction guideline]
   - **Current Code**: [Example of non-compliant code]
   - **Required Fix**: [What needs to be changed]
   - **Priority**: High/Medium/Low

2. **[Next Issue]**: [Description]
   - [Continue format...]

#### **Recommendations**:
- [Specific actions needed to bring code into compliance]
- [Reference to relevant instruction files]
- [Suggested implementation approach]

---

## Common Compliance Categories

### **1. Naming Conventions Violations**
**Reference**: `naming-conventions.instruction.md`
- Incorrect file naming patterns
- Non-compliant class/method/property names
- Missing prefix/suffix conventions
- Case sensitivity violations

### **2. UI Generation Standards Violations**
**Reference**: `ui-generation.instruction.md`
- Non-Avalonia control usage
- Missing ReactiveUI patterns
- Incorrect AXAML structure
- Missing compiled bindings
- Business logic in UI code

### **3. ReactiveUI Pattern Violations**
**Reference**: `codingconventions.instruction.md`
- Missing `RaiseAndSetIfChanged` usage
- Incorrect command implementations
- Missing error handling patterns
- Non-reactive property implementations

### **4. MTM Data Pattern Violations**
**Reference**: `copilot-instructions.md`
- Incorrect Part ID format (should be string)
- Wrong Operation format (should be string numbers like "90", "100")
- Missing MTM-specific data structures
- Incorrect quantity handling

### **5. Color Scheme and Theme Violations**
**Reference**: `copilot-instructions.md`
- Hard-coded colors instead of DynamicResource
- Non-MTM color palette usage
- Missing theme resource references
- Incorrect purple brand color implementation

### **6. Architecture and Structure Violations**
**Reference**: `codingconventions.instruction.md`
- Business logic in UI components
- Missing MVVM separation
- Incorrect project structure
- Missing dependency injection preparation

### **7. Error Handling Violations**
**Reference**: `errorhandler.instruction.md`
- Missing error handling patterns
- Non-standardized error logging
- Missing user-friendly error messages
- Incorrect exception handling

### **8. Layout and Design Violations**
**Reference**: `ui-generation.instruction.md`
- Non-modern layout patterns
- Missing card-based designs
- Incorrect spacing and margins
- Missing sidebar navigation patterns

---

## Custom Prompt for Quality Verification

### **Verify Code Compliance**
**Persona:** Quality Assurance Auditor Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Review [filename] for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying any violations of naming conventions, UI generation standards, ReactiveUI patterns, MTM data patterns, color scheme requirements, architecture guidelines, error handling standards, and layout/design principles. Include specific code examples of violations and required fixes with priority levels."

**Usage Example:**  
```
Review Views/MainView.axaml and ViewModels/MainViewModel.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying violations and required fixes.
```

---

## Quality Assurance Workflow

### **Step 1: Initial Code Review**
1. Run compliance verification prompt on target files
2. Copilot generates detailed report in this file
3. Review findings and prioritize fixes

### **Step 2: Fix Implementation**
1. Address high-priority violations first
2. Use appropriate instruction files as reference
3. Apply recommended fixes and improvements

### **Step 3: Re-verification**
1. Run compliance check again on updated code
2. Update report status from ? NEEDS REPAIR to ? COMPLIANT
3. Document completion date and any remaining minor issues

### **Step 4: Continuous Monitoring**
1. Regular compliance checks on new code
2. Update reports as instruction files evolve
3. Maintain compliance tracking over time

---

## Instruction File Cross-Reference

When generating reports, reference these instruction files for standards:

| Instruction File | Primary Focus | Key Standards |
|-----------------|---------------|---------------|
| `copilot-instructions.md` | Overall project guidelines | MTM data patterns, color scheme, project structure |
| `codingconventions.instruction.md` | Coding standards | ReactiveUI patterns, MVVM, naming |
| `ui-generation.instruction.md` | UI creation guidelines | Avalonia AXAML, modern layouts, theme usage |
| `naming-conventions.instruction.md` | File and code naming | Views, ViewModels, services, controls |
| `errorhandler.instruction.md` | Error handling patterns | Exception handling, logging, user messages |
| `ui-mapping.instruction.md` | Control mapping standards | WinForms to Avalonia conversion |
| `customprompts.instruction.md` | Prompt templates | Custom prompts and persona usage |
| `personas.instruction.md` | Copilot behavior guidelines | Persona-specific behavioral patterns |

---

## Priority Classification

### **High Priority (Critical Compliance Issues)**
- Security vulnerabilities or data exposure
- Architecture violations affecting maintainability
- Missing essential ReactiveUI patterns
- Incorrect MTM data handling
- Hard-coded values preventing theming

### **Medium Priority (Standards Violations)**
- Naming convention inconsistencies
- Missing error handling
- Non-optimal UI patterns
- Incomplete documentation
- Minor architecture deviations

### **Low Priority (Style and Enhancement)**
- Code formatting improvements
- Additional documentation
- Performance optimizations
- Enhanced user experience features
- Non-critical naming adjustments

---

## Report History Template

### **Active Issues**
*Files currently needing repair*

### **Resolved Issues**
*Files brought into compliance*

### **Compliance Metrics**
- **Total Files Reviewed**: [Number]
- **Compliant Files**: [Number] ([Percentage]%
- **Files Needing Repair**: [Number] ([Percentage]%
- **Average Compliance Score**: [Score/10]

---

## Example Report Entry

### **File: ViewModels/InventoryViewModel.cs**
**Review Date**: 2025-01-27  
**Reviewed By**: Quality Assurance Auditor Copilot  
**Instruction Files Referenced**: codingconventions.instruction.md, copilot-instructions.md  
**Compliance Status**: ? **NEEDS REPAIR**

#### **Issues Found**:
1. **ReactiveUI Pattern Violation**: Properties not using RaiseAndSetIfChanged
   - **Standard**: All ViewModel properties must use `this.RaiseAndSetIfChanged(ref _field, value)`
   - **Current Code**: `public string PartId { get; set; }`
   - **Required Fix**: Convert to reactive property with backing field
   - **Priority**: High

2. **MTM Data Pattern Violation**: Operation stored as integer instead of string
   - **Standard**: Operations should be string numbers ("90", "100", "110")
   - **Current Code**: `public int Operation { get; set; }`
   - **Required Fix**: Change to `public string Operation { get; set; }`
   - **Priority**: High

3. **Missing Error Handling**: Commands don't implement exception handling
   - **Standard**: All ReactiveCommands must have ThrownExceptions subscription
   - **Current Code**: No exception handling in SaveCommand
   - **Required Fix**: Add centralized error handling pattern
   - **Priority**: Medium

#### **Recommendations**:
- Refactor all properties to use ReactiveUI patterns
- Update Operation property to use MTM string format
- Implement centralized exception handling for all commands
- Add dependency injection preparation comments
- Reference codingconventions.instruction.md for complete ReactiveUI patterns

---

*This file serves as a living document to track and improve code quality across the MTM WIP Application project.*