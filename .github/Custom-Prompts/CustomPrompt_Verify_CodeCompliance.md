# Custom Prompt: Verify Code Compliance

## ?? **Instructions**
Use this prompt when you need to conduct comprehensive quality assurance reviews of code files against MTM instruction guidelines. This prompt performs systematic compliance checking and generates structured reports with specific violation details and fix recommendations.

## ?? **Persona**
**Quality Assurance Copilot** - Specializes in conducting thorough code compliance reviews against MTM instruction guidelines, identifying violations, and providing specific remediation guidance.

## ?? **Prompt Template**

```
Act as Quality Assurance Copilot. Conduct comprehensive code compliance verification for [FILE_PATH] against MTM instruction guidelines with the following requirements:

**Target File:** [Specific file path to review]
**Review Scope:** [All guidelines | Specific category - UI/Database/Naming/etc.]
**Priority Focus:** [High priority violations | All violations | Specific pattern compliance]

**Compliance Review Areas:**
1. **Naming Conventions** - Check against naming.conventions.instruction.md
2. **Coding Standards** - Verify codingconventions.instruction.md compliance
3. **UI Patterns** - Review ui-generation.instruction.md requirements
4. **Database Patterns** - Validate database-patterns.instruction.md compliance
5. **MTM Business Rules** - Verify MTM-specific patterns and requirements
6. **Architecture Compliance** - Check MVVM, ReactiveUI, and dependency injection patterns

**Analysis Requirements:**
- Identify specific line numbers and code sections with violations
- Classify violations by priority (Critical/High/Medium/Low)
- Provide specific fix recommendations with code examples
- Reference relevant instruction file sections
- Generate summary compliance score and status
- Document missing systems or components
- Track compliance trends if reviewing multiple times

**Output Format:**
- Structured violation report with priority classification
- Specific fix recommendations for each violation
- Code examples showing compliant alternatives
- References to relevant instruction guidelines
- Overall compliance assessment and next steps

**Additional Context:** [Any specific compliance concerns or focus areas]
```

## ?? **Purpose**
This prompt generates comprehensive compliance reports that identify specific violations against MTM instruction guidelines, provide actionable fix recommendations, and maintain quality standards across the codebase.

## ?? **Usage Examples**

### **Example 1: Full Compliance Review**
```
Act as Quality Assurance Copilot. Conduct comprehensive code compliance verification for ViewModels/InventoryViewModel.cs against MTM instruction guidelines with the following requirements:

**Target File:** ViewModels/InventoryViewModel.cs
**Review Scope:** All guidelines
**Priority Focus:** All violations

**Compliance Review Areas:**
1. **Naming Conventions** - Check against naming.conventions.instruction.md
2. **Coding Standards** - Verify codingconventions.instruction.md compliance
3. **UI Patterns** - Review ui-generation.instruction.md requirements
4. **Database Patterns** - Validate database-patterns.instruction.md compliance
5. **MTM Business Rules** - Verify MTM-specific patterns and requirements
6. **Architecture Compliance** - Check MVVM, ReactiveUI, and dependency injection patterns

**Analysis Requirements:**
- Identify specific line numbers and code sections with violations
- Classify violations by priority (Critical/High/Medium/Low)
- Provide specific fix recommendations with code examples
- Reference relevant instruction file sections
- Generate summary compliance score and status
- Document missing systems or components
- Track compliance trends if reviewing multiple times

**Output Format:**
- Structured violation report with priority classification
- Specific fix recommendations for each violation
- Code examples showing compliant alternatives
- References to relevant instruction guidelines
- Overall compliance assessment and next steps

**Additional Context:** This ViewModel handles inventory operations and should follow all MTM business rules for TransactionType determination and operation handling.
```

### **Example 2: Database Pattern Focus Review**
```
Act as Quality Assurance Copilot. Conduct comprehensive code compliance verification for Services/DatabaseService.cs against MTM instruction guidelines with the following requirements:

**Target File:** Services/DatabaseService.cs
**Review Scope:** Database patterns and MTM business rules
**Priority Focus:** High priority violations

**Compliance Review Areas:**
1. **Naming Conventions** - Check against naming.conventions.instruction.md
2. **Coding Standards** - Verify codingconventions.instruction.md compliance
3. **UI Patterns** - Review ui-generation.instruction.md requirements
4. **Database Patterns** - Validate database-patterns.instruction.md compliance
5. **MTM Business Rules** - Verify MTM-specific patterns and requirements
6. **Architecture Compliance** - Check MVVM, ReactiveUI, and dependency injection patterns

**Analysis Requirements:**
- Identify specific line numbers and code sections with violations
- Classify violations by priority (Critical/High/Medium/Low)
- Provide specific fix recommendations with code examples
- Reference relevant instruction file sections
- Generate summary compliance score and status
- Document missing systems or components
- Track compliance trends if reviewing multiple times

**Output Format:**
- Structured violation report with priority classification
- Specific fix recommendations for each violation
- Code examples showing compliant alternatives
- References to relevant instruction guidelines
- Overall compliance assessment and next steps

**Additional Context:** Focus on stored procedure usage, TransactionType logic, and MTM operation number handling. Ensure no direct SQL queries are present.
```

## ?? **Guidelines**

### **Compliance Review Process**
1. **File Analysis** - Read and understand the code structure and purpose
2. **Guideline Cross-Reference** - Check against all relevant instruction files
3. **Violation Identification** - Document specific non-compliance issues
4. **Priority Classification** - Categorize violations by impact and severity
5. **Fix Recommendations** - Provide specific, actionable remediation steps
6. **Report Generation** - Create structured output with clear next steps

### **Priority Classification System**
- **Critical**: Breaks compilation, security issues, data corruption risks
- **High**: Violates core MTM business rules, architecture violations
- **Medium**: Naming convention violations, code quality issues
- **Low**: Documentation gaps, minor style inconsistencies

### **Violation Documentation Format**
```markdown
## Violation [Priority]: [Brief Description]
**File:** [Path]
**Lines:** [Specific line numbers]
**Issue:** [Detailed description of the violation]
**Guideline:** [Reference to specific instruction file and section]
**Fix:** [Specific steps to resolve]
**Example:**
```csharp
// Current (incorrect)
[Current code]

// Corrected
[Compliant code example]
```
```

### **MTM-Specific Compliance Checks**
- **TransactionType Logic**: Must be based on user intent, not operation numbers
- **Database Access**: Only stored procedures allowed, no direct SQL
- **Operation Numbers**: Must be treated as string numbers (workflow steps)
- **Part IDs**: Must use string format
- **ReactiveUI Patterns**: Proper observable properties and command implementation
- **Service Registration**: Must use AddMTMServices, not individual registrations

## ?? **Related Files**
- [../Core-Instructions/naming.conventions.instruction.md](../Core-Instructions/naming.conventions.instruction.md) - Naming standards for all components
- [../Core-Instructions/codingconventions.instruction.md](../Core-Instructions/codingconventions.instruction.md) - Coding standards and patterns
- [../Development-Instructions/database-patterns.instruction.md](../Development-Instructions/database-patterns.instruction.md) - Database access and MTM business rules
- [../UI-Instructions/ui-generation.instruction.md](../UI-Instructions/ui-generation.instruction.md) - UI component standards
- [../Quality-Instructions/needsrepair.instruction.md](../Quality-Instructions/needsrepair.instruction.md) - Quality standards and tracking

## ? **Quality Checklist**

### **Review Completeness**
- [ ] All applicable instruction guidelines checked
- [ ] Specific line numbers identified for violations
- [ ] Priority classification applied consistently
- [ ] Fix recommendations provided for each violation
- [ ] Code examples included for corrections

### **Report Quality**
- [ ] Structured format with clear sections
- [ ] References to specific instruction file sections
- [ ] Actionable recommendations with clear steps
- [ ] Overall compliance score and assessment
- [ ] Next steps and priorities identified

### **MTM Compliance Verification**
- [ ] TransactionType logic reviewed against user intent rules
- [ ] Database operations checked for stored procedure usage
- [ ] Operation numbers verified as string workflow steps
- [ ] ReactiveUI patterns validated for proper implementation
- [ ] Service registration patterns checked for AddMTMServices usage

### **Follow-up Actions**
- [ ] Violations documented in tracking system
- [ ] Priority fixes scheduled for implementation
- [ ] Compliance trends tracked over time
- [ ] Team notification provided for critical issues