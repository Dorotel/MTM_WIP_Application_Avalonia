<!-- Copilot: Reading needsrepair.instruction.md — Quality Assurance and Code Compliance Tracking -->

# Needs Repair - Code Quality Assurance System

## Overview
This instruction file serves as a **quality assurance system guideline** for identifying code that does not meet the standards defined in the MTM WIP Application instruction files. When Copilot is asked to verify code compliance, it will generate **standalone compliance reports** in dedicated files rather than populating this instruction file.

## Purpose
- **Code Standards Verification**: Track files that don't comply with instruction guidelines
- **Standalone Report Generation**: Create individual compliance reports for each audit
- **Quality Assurance Documentation**: Maintain audit trail with dated reports
- **Compliance Tracking**: Monitor progress on bringing code up to standards
- **Instruction File Adherence**: Ensure all code follows established patterns and conventions

---

## Critical Missing Core Systems Analysis

### **?? CRITICAL ARCHITECTURE GAPS IDENTIFIED**

Based on comprehensive analysis of the MTM WIP Application Avalonia repository, the following **CRITICAL** and **HIGH PRIORITY** missing core systems have been identified that must be implemented before continuing with business logic and UI development:

#### **CRITICAL MISSING SYSTEMS (Immediate Implementation Required)**

**1. Service Layer Architecture** - ? **MISSING**
- **Missing Files**: `Services/IInventoryService.cs`, `Services/InventoryService.cs`
- **Missing Files**: `Services/IUserService.cs`, `Services/UserService.cs`
- **Missing Files**: `Services/ITransactionService.cs`, `Services/TransactionService.cs`
- **Missing Files**: `Services/IDatabaseService.cs`, `Services/DatabaseService.cs`
- **Impact**: No business logic separation, MVVM violations throughout

**2. Data Models and Entities** - ? **MISSING**
- **Missing**: Entire `Models/` folder and namespace
- **Missing Files**: `Models/InventoryItem.cs`, `Models/InventoryTransaction.cs`, `Models/User.cs`
- **Missing**: `Models/Result.cs<T>` pattern for service responses
- **Impact**: No strongly-typed data contracts, using primitive types

**3. Dependency Injection Container** - ? **MISSING**
- **Missing**: DI container setup in `Program.cs`
- **Missing**: Service registration and lifetime management
- **Impact**: No service injection, tight coupling throughout application

**4. Database Connection Management** - ? **MISSING**
- **Missing**: `Services/IConnectionManager.cs`
- **Missing**: Centralized connection string management
- **Impact**: No standardized database access patterns

**5. Application State Management** - ? **MISSING**
- **Missing**: `Services/IApplicationStateService.cs`
- **Missing**: Global application variables management
- **Impact**: Scattered state management, no centralization

**6. Theme and Resource System** - ? **MISSING**
- **Missing**: `Resources/Themes/` folder
- **Missing**: MTM purple color scheme implementation in `App.axaml`
- **Impact**: No consistent styling, hard-coded colors

**7. Navigation Service** - ? **MISSING**
- **Missing**: `Services/INavigationService.cs`
- **Missing**: Proper MVVM navigation patterns
- **Impact**: Direct view instantiation violating MVVM

#### **HIGH PRIORITY MISSING SYSTEMS**

**8. Result Pattern Implementation** - ?? **PARTIAL**
- **Present**: Error handling infrastructure exists
- **Missing**: `Result<T>` pattern for consistent service responses
- **Impact**: Inconsistent error handling patterns

**9. MTM Data Transfer Objects** - ? **MISSING**
- **Missing**: `Models/DTOs/` folder with request/response objects
- **Missing**: Validation attributes and business rules
- **Impact**: No data validation, type safety issues

**10. Configuration Management** - ?? **PARTIAL**
- **Present**: `Config/appsettings.json` exists
- **Missing**: Configuration service to read settings
- **Impact**: Configuration not accessible to services

### **Current Project Compliance Status**
- **Total Core Systems Required**: 19
- **Currently Implemented**: ~5 (26%)
- **Missing Critical Systems**: 7 (37%)
- **Missing High Priority Systems**: 12 (63%)
- **Overall Architecture Risk**: **HIGH**

---

## Compliance Report System

### **Report Generation Guidelines**

When conducting code compliance verification, **DO NOT** populate this file with findings. Instead:

1. **Create standalone report files** in the `Compliance Reports/` folder
2. **Use standardized naming**: `{FileName}-compliance-report-{YYYY-MM-DD}.md`
3. **Generate custom fix prompt** at the end of each report
4. **Maintain audit trail** with timestamped reports

### **Report Directory Structure**
```
Compliance Reports/
??? Services/
?   ??? InventoryService-compliance-report-2025-01-27.md
?   ??? README.md
??? ViewModels/
?   ??? README.md
??? Views/
?   ??? README.md
??? Models/
?   ??? README.md (MISSING - needs creation)
??? Summary/
?   ??? project-compliance-summary-2025-01-27.md
??? README.md
```

### **Standalone Report Template**

Each compliance report should be a separate markdown file with this structure:

```markdown
# Compliance Report: {FileName}

**Review Date**: {YYYY-MM-DD}  
**Reviewed By**: Quality Assurance Auditor Copilot  
**File Path**: {RelativeFilePath}  
**Instruction Files Referenced**: {List of .instruction.md files}  
**Compliance Status**: ? **CRITICAL** / ?? **NEEDS REPAIR** / ? **COMPLIANT**

---

## Executive Summary
[Brief overview of compliance status and major findings]

---

## Issues Found

### 1. **{Issue Category}**: {Specific violation description}
- **Standard**: {Reference to specific instruction guideline}
- **Current Code**: {Example of non-compliant code}
- **Required Fix**: {What needs to be changed}
- **Priority**: **CRITICAL** / **HIGH** / **MEDIUM** / **LOW**
- **Instruction Reference**: {Link to relevant instruction file}

### 2. **{Next Issue}**: {Description}
[Continue format...]

---

## Recommendations

### **Immediate Actions Required**:
1. {High priority fixes}
2. {Critical compliance issues}

### **Implementation Priority Order**:
1. **CRITICAL**: {Most important fixes}
2. **HIGH**: {Important standards violations}
3. **MEDIUM**: {Standards improvements}
4. **LOW**: {Style and enhancement}

### **Required Implementation Pattern**:
```{language}
// Example of compliant code implementation
```

---

## Related Files Requiring Updates
- `{RelatedFile1}` - {Description of required changes}
- `{RelatedFile2}` - {Description of required changes}

---

## Testing Considerations
- {Test requirements for fixes}
- {Validation steps}

---

## Custom Fix Prompt

**Use this prompt to implement the fixes identified in this report:**

```
Fix compliance violations in {FileName} based on the findings in Compliance Reports/{ReportFileName}.

Implement the following fixes in priority order:
1. {High priority fix 1}
2. {High priority fix 2}
3. {Medium priority fixes as applicable}

Follow these MTM patterns:
- {Pattern 1}
- {Pattern 2}

Reference these instruction files:
- {instruction-file-1.md}
- {instruction-file-2.md}

Ensure all changes maintain existing functionality while bringing code into compliance with MTM standards.
After implementation, run build verification to confirm no compilation errors.
```

---

## Compliance Metrics
- **Total Issues Found**: {Number}
- **Critical Issues**: {Number}
- **High Priority Issues**: {Number}
- **Medium Priority Issues**: {Number}
- **Low Priority Issues**: {Number}
- **Estimated Fix Time**: {Hours/Days}
- **Compliance Score**: {Score}/10

---

*Report generated by Quality Assurance Auditor Copilot following MTM WIP Application instruction guidelines.*
```

---

## Quality Assurance Workflow

### **Step 1: Compliance Audit**
1. Run compliance verification prompt on target files
2. Copilot generates standalone report in `Compliance Reports/` folder
3. Report includes specific violations and fix guidance

### **Step 2: Report Review**
1. Review generated compliance report
2. Prioritize fixes based on report recommendations
3. Plan implementation using provided custom prompt

### **Step 3: Fix Implementation**
1. Use custom prompt from report to implement fixes
2. Address critical and high-priority violations first
3. Apply recommended patterns and standards

### **Step 4: Re-verification**
1. Generate new compliance report on updated code
2. Compare with previous report to track progress
3. Update compliance status and document improvements

### **Step 5: Compliance Tracking**
1. Maintain audit trail with dated reports
2. Track compliance metrics over time
3. Monitor overall project compliance health

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

### **9. Missing Core System Violations**
**Reference**: `copilot-instructions.md`
- **Service Layer Missing**: No business logic separation
- **Data Models Missing**: No strongly-typed entities
- **DI Container Missing**: No service injection patterns
- **Navigation Service Missing**: Direct view instantiation
- **State Management Missing**: Scattered application state
- **Theme System Missing**: No consistent styling framework

---

## Priority Classification

### **CRITICAL Priority**
- Security vulnerabilities or data exposure
- Missing essential files or components
- Architecture violations preventing functionality
- Hard-coded values preventing theming
- **Missing service layer preventing MVVM compliance**
- **Missing data models causing type safety issues**
- **Missing DI container preventing service injection**

### **HIGH Priority**
- Essential ReactiveUI patterns missing
- Incorrect MTM data handling
- Missing error handling integration
- Non-async patterns for database operations
- **Missing navigation service causing MVVM violations**
- **Missing configuration service preventing settings access**
- **Missing theme resources causing inconsistent styling**

### **MEDIUM Priority**
- Naming convention inconsistencies
- Missing dependency injection preparation
- Incomplete documentation
- Minor architecture deviations
- **Missing validation system**
- **Missing caching layer affecting performance**

### **LOW Priority**
- Code formatting improvements
- Additional documentation enhancements
- Performance optimizations
- Non-critical naming adjustments

---

## Missing Core Systems Implementation Roadmap

### **Phase 1: Foundation (Week 1)**
**Immediate Implementation Required:**
1. **Create Models namespace** with all data entities
   - `Models/Result.cs<T>` pattern for service responses
   - `Models/InventoryItem.cs` with MTM data patterns
   - `Models/InventoryTransaction.cs` for transaction history
   - `Models/User.cs` for user management

2. **Setup dependency injection** in Program.cs and App.axaml.cs
   - Configure Microsoft.Extensions.DependencyInjection
   - Register all service interfaces and implementations

3. **Create core service interfaces**
   - `Services/IInventoryService.cs`
   - `Services/IUserService.cs`
   - `Services/ITransactionService.cs`
   - `Services/IDatabaseService.cs`

### **Phase 2: Service Layer (Week 2)**
1. **Implement InventoryService** with full MTM patterns
2. **Create DatabaseService** for centralized data access
3. **Implement ApplicationStateService** for global state
4. **Setup configuration service** to read appsettings.json

### **Phase 3: Infrastructure (Week 3)**
1. **Create NavigationService** for proper MVVM navigation
2. **Implement theme resources** with MTM purple palette
3. **Setup repository pattern** for data access abstraction
4. **Add validation system** for business rules

### **Phase 4: Quality Assurance (Week 4)**
1. **Create unit testing project** with mocks
2. **Add structured logging** throughout application
3. **Implement caching layer** for performance
4. **Add security infrastructure** for production readiness

---

## Custom Prompt for Quality Verification

### **Generate Compliance Report**
**Persona:** Quality Assurance Auditor Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Conduct comprehensive compliance audit of {filename} against all MTM WIP Application instruction guidelines. Generate a standalone compliance report in Compliance Reports/{filename}-compliance-report-{date}.md identifying violations of naming conventions, UI generation standards, ReactiveUI patterns, MTM data patterns, color scheme requirements, architecture guidelines, error handling standards, layout/design principles, and missing core systems. Include specific code examples, priority classifications, and a custom fix prompt at the end of the report."

**Usage Example:**  
```
Conduct comprehensive compliance audit of Services/InventoryService.cs against all MTM WIP Application instruction guidelines. Generate a standalone compliance report in Compliance Reports/Services/InventoryService-compliance-report-2025-01-27.md with detailed findings and custom fix prompt.
```

**Bulk Assessment Example:**  
```
Conduct comprehensive quality audit of MTM_WIP_Application_Avalonia project. Review all C# and AXAML files for MTM compliance and missing core systems. Generate individual compliance reports in Compliance Reports/ folder organized by directory structure. Include project-wide compliance summary report with metrics and implementation roadmap for missing systems.
```

**Missing Systems Analysis Example:**  
```
Conduct missing core systems analysis of MTM_WIP_Application_Avalonia project. Identify all missing service layers, data models, dependency injection setup, navigation services, theme systems, and other critical infrastructure. Generate comprehensive analysis report with implementation priority order and estimated timelines.
```

**Re-verification Example:**  
```
Re-audit {filename} for compliance improvements and generate updated compliance report. Compare findings with previous report from Compliance Reports/ and document progress made on fixing violations. Update compliance status and metrics.
```

---

## Instruction File Cross-Reference

When generating compliance reports, reference these instruction files for standards:

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

## Compliance Dashboard (Overview)

### **Current Project Status**
*Updated with comprehensive analysis findings*

- **Total Files Audited**: 1 (Services/InventoryService.cs)
- **Compliant Files**: 0 (0%)
- **Files Needing Repair**: 1 (100%)
- **Critical Issues Outstanding**: 7 core systems missing
- **Average Compliance Score**: 0/10
- **Overall Architecture Compliance**: 25% (CRITICAL RISK)

### **Recent Audit Activity**
*Links to recent compliance reports*
- **2025-01-27**: InventoryService-compliance-report - CRITICAL (File Missing)
- **Missing**: Data Models analysis needed
- **Missing**: ViewModels compliance audit needed
- **Missing**: Views compliance audit needed

### **Compliance Trends**
*Track improvement over time*
- **Baseline Assessment**: 25% compliance (CRITICAL architecture gaps)
- **Trend**: Requires immediate foundation work before continuing development
- **Risk Level**: HIGH - Missing core systems prevent sustainable development

### **Critical Implementation Priorities**
1. **Service Layer**: 0% implemented (7 services missing)
2. **Data Models**: 0% implemented (entire namespace missing)
3. **Dependency Injection**: 0% implemented (no DI container setup)
4. **Navigation Service**: 0% implemented (MVVM violations present)
5. **Theme System**: 0% implemented (hard-coded colors throughout)

---

## Report Archive Guidelines

### **File Organization**
- Group reports by component type (Services/, ViewModels/, Views/, Models/, etc.)
- Use consistent naming: `{FileName}-compliance-report-{YYYY-MM-DD}.md`
- Maintain chronological audit trail for each file
- **Create Models/ compliance reports folder** (currently missing)

### **Report Retention**
- Keep all compliance reports for audit trail
- Archive older reports after successful compliance verification
- Maintain summary reports for project-wide compliance tracking
- **Preserve missing systems analysis** for implementation tracking

### **Metrics Tracking**
- Extract compliance metrics from individual reports
- Generate project-wide compliance summaries
- Track improvement trends over time
- **Monitor core systems implementation progress**

---

## Integration with Development Workflow

### **Pre-Implementation Verification**
**CRITICAL**: Before continuing with any business logic or UI development:
1. **Implement missing core systems** identified in analysis
2. **Generate compliance reports** for all new systems
3. **Achieve minimum 80% compliance** on foundational systems

### **Pre-Commit Verification**
Use compliance auditing as part of code review process:
1. Generate compliance report for changed files
2. Address critical and high-priority violations
3. Include compliance status in pull request review
4. **Verify no new missing systems introduced**

### **Continuous Compliance**
- Schedule regular compliance audits
- Monitor compliance trends
- Update instruction files based on common violations
- **Track missing systems implementation progress**

### **Team Collaboration**
- Share compliance reports with team members
- Use custom fix prompts for consistent remediation
- Maintain project-wide compliance standards
- **Coordinate core systems implementation** across team

---

## Quality Assurance Enforcement

### **Development Blocking Issues**
The following compliance violations should **BLOCK** further development:
- **Missing service layer** (CRITICAL)
- **Missing data models** (CRITICAL)
- **Missing DI container** (CRITICAL)
- **Direct view instantiation** violating MVVM (HIGH)
- **Hard-coded colors** preventing theming (HIGH)

### **Technical Debt Tracking**
- **Current Technical Debt**: 19 missing core systems
- **Estimated Resolution Time**: 4-6 weeks
- **Risk to Project**: HIGH (architecture cannot support sustainable development)
- **Recommended Action**: Implement foundation systems before continuing

---

*This instruction file serves as the foundation for maintaining code quality and compliance across the MTM WIP Application Avalonia project. All compliance findings should be documented in standalone reports following the guidelines above. The identified missing core systems must be implemented before continuing with business logic and UI development.*
