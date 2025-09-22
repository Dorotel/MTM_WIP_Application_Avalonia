---
name: MTM Audit System - Continuous Quality
description: 'Quality assurance checklist for Manufacturing Tracking and Monitoring (MTM) continuous quality monitoring and manufacturing system health validation'
applies_to: 'monitoring/**/*.md'
manufacturing_context: true
review_type: 'continuous'
quality_gate: 'critical'
---

# MTM Pull Request Audit System - Main Execution Prompt

**Copy this prompt into VS Code GitHub Copilot Chat to execute the audit system:**

---

@copilot #file:.github/scripts/README.md Execute comprehensive MTM Pull Request Audit and Copilot Continuation System for current branch.

## Audit Execution Requirements

### Phase 1: Discovery & Analysis

1. **Identify Implementation Plan**:
   - Search for implementation plan: `docs/plans/*/implementation-plan/implementation-plan.md`
   - Extract feature name from path (e.g., "print-service" from path)
   - Parse requirements, file specifications, and acceptance criteria

2. **Branch State Analysis**:
   - Get current branch name via `git branch --show-current`
   - Scan all modified/added files in current branch vs master
   - Analyze file structure against implementation plan requirements
   - Check for MTM pattern compliance (MVVM Community Toolkit, Avalonia AXAML, etc.)

3. **Gap Detection**:
   - Compare required files vs existing files
   - Analyze code completeness in existing files
   - Check service registration in ServiceCollectionExtensions.cs
   - Validate navigation integration patterns
   - Verify theme system integration
   - Check error handling implementation

### Phase 2: Report Generation

1. **Clean Previous Reports** (if they exist):
   - Delete `docs/audit/{BRANCH-NAME}-gap-report.md` if exists
   - Delete `docs/audit/{BRANCH-NAME}-copilot-prompt.md` if exists
   - Create fresh reports to avoid stale progress tracking

2. **Generate Gap Report** (`docs/audit/{BRANCH-NAME}-gap-report.md`):

   ```markdown
   # MTM Feature Implementation Gap Report
   **Branch**: {BRANCH-NAME}
   **Feature**: {FEATURE-NAME}
   **Generated**: {DATE-TIME}
   **Implementation Plan**: {PLAN-PATH}

   ## Executive Summary
   - Overall Progress: X% complete
   - Critical Gaps: X items
   - Ready for Testing: Yes/No
   - Estimated Completion: X hours

   ## File Status Analysis
   ### ✅ Completed Files
   - filename.cs - Fully implemented, MTM patterns compliant
   
   ### 🔄 Partially Implemented Files  
   - filename.cs - Missing: specific methods/properties
   
   ### ❌ Missing Files
   - filename.cs - Required for: specific functionality
   
   ## Architecture Compliance
   ### ✅ MVVM Community Toolkit Patterns
   ### ✅ Avalonia AXAML Syntax  
   ### ✅ Service Registration
   ### ✅ Navigation Integration
   ### ✅ Theme System Integration
   ### ✅ Error Handling
   
   ## Priority Gaps (Critical Path)
   1. **High Priority**: Missing core service implementation
   2. **Medium Priority**: UI integration incomplete
   3. **Low Priority**: Optional features not implemented
   
   ## Next Development Session Focus
   - Specific actionable items for immediate implementation
   ```

3. **Generate Copilot Prompt** (`docs/audit/{BRANCH-SHORTDESCRIPTIONNAME}-copilot-prompt.md`):

   ```markdown
   # MTM Feature Continuation Prompt for @copilot
   **Copy and paste this prompt in PR comments or Copilot Chat**
   
   ---
   
   @copilot Continue implementation of {FEATURE-NAME} feature. 
   
   ## Current Context
   **Branch**: {BRANCH-NAME}
   **Implementation Plan**: {PLAN-PATH}
   **Last Audit**: {DATE-TIME}
   
   ## Critical Missing Components
   {LIST-OF-CRITICAL-GAPS}
   
   ## MTM Architecture Requirements
   - Use MVVM Community Toolkit: [ObservableProperty] and [RelayCommand]
   - Avalonia AXAML: xmlns="https://github.com/avaloniaui"
   - Service registration: ServiceCollectionExtensions.cs
   - Navigation: Follow ThemeEditor pattern
   - Theme: MTM_Shared_Logic.* DynamicResource bindings
   - Error handling: Services.ErrorHandling.HandleErrorAsync()
   
   ## Immediate Implementation Tasks
   {PRIORITIZED-TASK-LIST}
   
   ## Files to Focus On
   {SPECIFIC-FILES-TO-WORK-ON}
   
   ## Integration Points
   {SPECIFIC-SERVICE-DEPENDENCIES}
   
   ## Quality Requirements
   - No AVLN2000 compilation errors
   - Follow established naming conventions  
   - Proper dependency injection patterns
   - Comprehensive error handling
   
   Continue implementation focusing on critical path items first.
   ```

### Phase 3: Execution Validation

1. **Verify Report Quality**:
   - Gap report contains specific, actionable items
   - Copilot prompt includes sufficient context for continuation
   - All MTM-specific requirements included
   - File analysis is comprehensive and accurate

2. **Create Audit Directory** if needed:
   - Ensure `docs/audit/` directory exists
   - Create reports with proper naming convention

## Success Criteria

- [ ] Implementation plan successfully located and parsed
- [ ] Current branch state accurately analyzed
- [ ] Gap report contains specific, actionable gaps
- [ ] Copilot prompt provides sufficient context for continuation
- [ ] All MTM architecture patterns validated
- [ ] Reports are ready for immediate use in development workflow

## MTM-Specific Validation Checklist

- [ ] MVVM Community Toolkit patterns checked
- [ ] Avalonia AXAML syntax compliance verified  
- [ ] Service registration in ServiceCollectionExtensions validated
- [ ] NavigationService integration pattern confirmed
- [ ] ThemeService integration checked
- [ ] Error handling via Services.ErrorHandling verified
- [ ] Database patterns (stored procedures only) validated if applicable
- [ ] MTM design system compliance (DynamicResource bindings) checked

Execute this audit system now and generate comprehensive reports for current development session continuation.


## 🤖 Joyride Integration

**Use Joyride automation when safe and possible:**
- `joyride_evaluate_code` for VS Code API automation
- `joyride_request_human_input` for interactive workflows
- File template generation and pattern enforcement
- Real-time validation and consistency checking

**MTM-Specific Applications:** MVVM pattern enforcement, theme validation, database testing, cross-platform automation.

