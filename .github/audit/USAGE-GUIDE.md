# MTM Audit System - Complete Usage Guide

## System Overview

The MTM Audit System provides comprehensive Pull Request analysis and Copilot continuation capabilities for the MTM WIP Application Avalonia project. This system enables:

- **Gap Analysis**: Detailed comparison of implementation plans vs actual code
- **Pattern Compliance**: Verification of MTM architecture and coding patterns
- **Priority Assessment**: Intelligent ordering of development tasks
- **Copilot Integration**: Automated generation of targeted development prompts
- **Multi-Session Continuity**: Seamless handoffs between development sessions

## Quick Start Guide

### 1. Basic Gap Analysis
```bash
# Navigate to your MTM project root
cd "C:\Users\jkoll\source\MTM_WIP_Application_Avalonia"

# Create a basic gap report for any feature
Copy-Item ".github\scripts\templates\gap-report-template.md" "gap-report-temp.md"

# Edit the template with your specific feature details, then analyze manually
```

### 2. Generate Copilot Continuation Prompt
```bash
# After completing gap analysis, generate targeted Copilot prompt
Copy-Item ".github\scripts\templates\copilot-prompt-template.md" "copilot-prompt-temp.md"

# Customize with findings from gap analysis
# Post to GitHub issue with #github-pull-request_copilot-coding-agent hashtag
```

### 3. Automated Full Analysis (Advanced)
```bash
# Use the comprehensive audit system prompt (future enhancement)
# Run: .\audit-system-prompt.md against your branch
```

## File Structure Reference

```
.github/audit/
├── README.md                          # This usage guide
├── audit-system-prompt.md            # Main execution prompt (Phase 1-3)
├── templates/
│   ├── gap-report-template.md         # Gap analysis template
│   ├── copilot-prompt-template.md     # Copilot continuation template
│   └── examples/
│       ├── README.md                  # Example system documentation
│       ├── print-service-gap-report-example.md    # Real gap analysis
│       └── print-service-copilot-prompt-example.md # Real Copilot prompt
```

## Template Usage Patterns

### Gap Report Template Variables
Replace these placeholders in `gap-report-template.md`:

- `{BRANCH-NAME}` → Your feature branch name
- `{FEATURE-NAME}` → Feature being developed  
- `{DATE-TIME}` → Current timestamp
- `{PLAN-PATH}` → Path to implementation plan
- `{X/Y}` → Progress numbers (completed/total)
- `{CRITICAL-GAPS-LIST}` → List of blocking issues
- `{COMPLETED-FILES-LIST}` → Files fully implemented
- `{PARTIAL-FILES-LIST}` → Files partially complete
- `{MISSING-FILES-LIST}` → Required files not created

### Copilot Prompt Template Variables
Replace these placeholders in `copilot-prompt-template.md`:

- `{CRITICAL-ISSUE-1}` → Most important blocking issue
- `{IMPACT-DESCRIPTION}` → Business impact description
- `{FILE-LIST}` → Affected file paths
- `{MTM-PATTERN}` → Required MTM pattern to follow
- `{CODE-EXAMPLE-IF-APPLICABLE}` → Code snippet if needed
- `{PRIORITY-N-TASK}` → Task in priority order
- `{DEPENDENCY-REASON}` → Why this task comes first
- `{REQUIRED-PATTERN}` → MTM pattern to implement

## Real-World Workflow Examples

### Scenario 1: New Feature Development

**Starting Point**: You have an implementation plan and need to track progress.

1. **Create Gap Report**:
   ```markdown
   # Copy template and fill in your feature details
   Copy-Item ".github\scripts\templates\gap-report-template.md" "my-feature-gap-report.md"
   
   # Fill in specifics:
   # - Feature name and branch
   # - Implementation plan path
   # - Current file status (completed/partial/missing)
   # - MTM pattern compliance issues
   ```

2. **Analyze Current State**:
   - Check which files exist vs implementation plan
   - Verify MVVM Community Toolkit usage (`[ObservableProperty]`, `[RelayCommand]`)
   - Validate Avalonia AXAML syntax (no AVLN2000 errors)
   - Confirm service registration patterns
   - Test navigation integration

3. **Generate Copilot Prompt**:
   ```markdown
   # Use findings to create targeted development prompt
   Copy-Item ".github\scripts\templates\copilot-prompt-template.md" "my-feature-copilot.md"
   
   # Include:
   # - Critical blocking issues first
   # - Specific file paths and patterns
   # - Code examples for complex implementations
   # - MTM compliance requirements
   ```

4. **Execute with Copilot**:
   - Post generated prompt to GitHub issue
   - Add hashtag: `#github-pull-request_copilot-coding-agent`
   - Copilot implements following MTM patterns

### Scenario 2: Mid-Development Progress Check

**Starting Point**: Development is partially complete, need to assess gaps.

1. **Update Gap Report**:
   - Mark completed files as ✅
   - Identify partially complete files as 🔄 with specific gaps
   - List still-missing files as ❌
   - Update compliance percentages

2. **Priority Assessment**:
   - Critical: Blocking issues that prevent feature access
   - High: Feature incomplete but foundation exists  
   - Medium: Enhancement and polish items

3. **Targeted Continuation**:
   - Generate Copilot prompt focusing on next logical steps
   - Include context from previous implementations
   - Specify integration points and dependencies

### Scenario 3: Cross-Session Development Handoffs

**Starting Point**: Different developer/session needs to continue work.

1. **Complete Context Capture**:
   - Document what was implemented in last session
   - Note any architectural decisions made
   - Identify current blocking issues
   - List next logical implementation steps

2. **Comprehensive Continuation Prompt**:
   - Rich context about current state
   - Specific MTM pattern examples
   - Priority-ordered task list
   - Integration requirements and dependencies

## MTM Pattern Compliance Checklist

### MVVM Community Toolkit (Required)
- [ ] Use `[ObservableObject]` partial class declarations
- [ ] Use `[ObservableProperty]` for all bindable properties
- [ ] Use `[RelayCommand]` for all command implementations
- [ ] Inherit from `BaseViewModel` for consistency
- [ ] NO ReactiveUI patterns anywhere

### Avalonia AXAML Syntax (Required)
- [ ] Use `x:Name` instead of `Name` on Grid definitions
- [ ] Use `xmlns="https://github.com/avaloniaui"` namespace
- [ ] Follow InventoryTabView pattern: `RowDefinitions="*,Auto"`
- [ ] ScrollViewer as root element to prevent overflow
- [ ] DynamicResource bindings for all theme elements

### Service Layer Patterns (Required)
- [ ] Constructor dependency injection with null checks
- [ ] TryAddSingleton/TryAddTransient in ServiceCollectionExtensions
- [ ] Services.ErrorHandling.HandleErrorAsync() for all exceptions
- [ ] Proper service lifetime management

### Database Patterns (If Applicable)
- [ ] Stored procedures only via Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()
- [ ] No direct SQL queries or string concatenation
- [ ] Empty collections on failure (no fallback data)
- [ ] Correct column names from database schema

### Navigation Patterns (If Applicable)
- [ ] Follow ThemeEditorViewModel navigation pattern
- [ ] Use NavigationService.NavigateTo<TView, TViewModel>()
- [ ] Full-window transitions for major features
- [ ] Proper error handling for navigation failures

### Theme Integration (Required)
- [ ] DynamicResource bindings for MTM_Shared_Logic.* resources
- [ ] Support for all MTM theme variants (Blue, Green, Dark, Red)
- [ ] IThemeService integration if runtime switching needed
- [ ] Consistent MTM design system usage

## Advanced Usage Scenarios

### Custom Pattern Development
When creating new patterns not in existing templates:

1. **Document the Pattern**:
   - Create detailed implementation example
   - Explain MTM compliance requirements
   - Show integration with existing services

2. **Update Templates**:
   - Add pattern to compliance checklists
   - Include code examples in templates
   - Update Copilot prompt patterns

3. **Validate Across Features**:
   - Test pattern with multiple feature implementations
   - Ensure consistency with MTM architecture
   - Update documentation and examples

### Large Feature Development
For complex features spanning multiple development sessions:

1. **Phase-Based Planning**:
   - Break feature into logical phases (Foundation → Core → Polish)
   - Create separate gap reports for each phase
   - Generate targeted Copilot prompts per phase

2. **Cross-Phase Dependencies**:
   - Document what Phase N needs from Phase N-1
   - Include dependency validation in gap reports
   - Update prompts with phase-specific context

3. **Integration Testing Checkpoints**:
   - Define testable milestones between phases
   - Include integration testing in gap analysis
   - Validate MTM pattern compliance at each checkpoint

## Troubleshooting Common Issues

### Template Customization Problems
**Issue**: Template placeholders not replaced correctly
**Solution**: Use consistent find/replace, validate all `{PLACEHOLDER}` instances

**Issue**: Generated prompts too generic
**Solution**: Include specific file paths, actual error messages, concrete code examples

### Pattern Compliance Issues  
**Issue**: AVLN2000 compilation errors
**Solution**: Check Name vs x:Name usage, Avalonia namespace declaration

**Issue**: ReactiveUI patterns creeping in
**Solution**: Strict adherence to MVVM Community Toolkit only, code review templates

### Copilot Integration Problems
**Issue**: Copilot doesn't follow MTM patterns
**Solution**: Include more specific code examples, pattern compliance checks in prompts

**Issue**: Development context lost between sessions
**Solution**: Richer context capture, architectural decision documentation

## Future Enhancements

### Planned Improvements
- **Automated Gap Analysis**: Script-based analysis of implementation plan vs code
- **Pattern Validation**: Automated checking of MTM compliance rules
- **Integration Testing**: Automated validation of service integration points
- **Performance Analysis**: Automated performance pattern compliance checking

### Contributing to System
To improve the audit system:

1. **Add New Templates**: Create new templates for specific scenarios
2. **Enhance Examples**: Add more real-world examples and edge cases
3. **Update Patterns**: Keep MTM pattern examples current with codebase evolution
4. **Improve Automation**: Add scripts for automated analysis and validation

---

**The MTM Audit System enables consistent, high-quality development across all features while maintaining architectural integrity and enabling seamless multi-session collaboration.**