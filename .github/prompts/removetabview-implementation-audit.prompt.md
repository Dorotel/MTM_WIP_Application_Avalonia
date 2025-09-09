# RemoveTabView Implementation Audit Prompt

## 🎯 Comprehensive Implementation Status Analysis

**Context**: Compare current RemoveTabView implementation against #file:removetabview-complete-implementation.yml requirements and generate prioritized work plan.

**Instructions for @copilot**:

Perform a detailed audit of the current RemoveTabView implementation by analyzing:

### **1. SPECIFICATION COMPLIANCE AUDIT**

**UI Implementation Analysis:**
- Compare `Views/MainForm/Panels/RemoveTabView.axaml` against DataGrid-centric layout requirements
- Verify CollapsiblePanel integration (`HeaderPosition`, auto-collapse/expand behavior)
- Check complete TextBox + SuggestionOverlay replacement of ComboBoxes (all 4 fields)
- Validate InventoryTabView styling consistency (ScrollViewer > Grid[RowDefinitions="*,Auto"] > Border pattern)
- Audit MTM theme integration (DynamicResource bindings)

**Service Integration Analysis:**
- Audit `ViewModels/MainForm/RemoveItemViewModel.cs` for complete service integration:
  - ISuggestionOverlayService integration and methods
  - ISuccessOverlayService integration for removal confirmations  
  - IQuickButtonsService integration for field population + history logging
  - ErrorHandling service integration for centralized error management
  - Progress reporting integration with MainView status bar

**Business Logic Analysis:**
- Review batch deletion implementation with atomic transaction handling
- Verify multi-row selection support in DataGrid
- Check confirmation dialog implementation for delete operations
- Audit transaction logging (RED color coding for OUT transactions)
- Validate fuzzy validation integration (TextBoxFuzzyValidationBehavior)

**User Experience Analysis:**
- Check keyboard shortcuts implementation (F5, Delete, Ctrl+Z, Escape, Enter)
- Audit accessibility features (ToolTip.Tip, focus management, screen reader support)
- Verify loading states and "Nothing Found" indicators
- Check smooth panel animations and visual feedback

### **2. GENERATE COMPLIANCE MATRIX**

Create a structured compliance matrix:

```markdown
| Requirement Category | Requirement | Status | Implementation Details | Missing/Issues |
|---------------------|-------------|---------|----------------------|----------------|
| UI Layout | DataGrid-centric layout | ✅/🔄/❌ | Current implementation | Specific gaps |
| CollapsiblePanel | Auto-collapse/expand | ✅/🔄/❌ | Current behavior | What's missing |
| Service Integration | SuggestionOverlay | ✅/🔄/❌ | Integration status | Missing methods |
| ... | ... | ... | ... | ... |
```

### **3. PRIORITIZED ACTION PLAN**

Generate a priority-ordered action plan:

**🚨 CRITICAL (Must Complete First)**
- [ ] Critical blockers preventing basic functionality
- [ ] Build-breaking issues
- [ ] Core user workflow blockers

**⚡ HIGH PRIORITY (Next Sprint)**  
- [ ] User experience improvements
- [ ] Missing service integrations
- [ ] Performance optimizations

**📋 MEDIUM PRIORITY (Future Sprint)**
- [ ] Enhanced features
- [ ] Advanced validations
- [ ] Polish improvements

**📝 LOW PRIORITY (Backlog)**
- [ ] Edge case handling
- [ ] Documentation updates
- [ ] Optional enhancements

### **4. IMPLEMENTATION ROADMAP**

For each priority level, provide:
- **Specific files to modify** (with line references when possible)
- **New components/services needed**
- **Estimated effort** (hours/days)
- **Dependencies and blockers**
- **Acceptance criteria**
- **Testing requirements**

### **5. GITHUB ISSUES CREATION PLAN**

Generate ready-to-use GitHub issue templates for each major work item:
- Clear title and description
- Acceptance criteria checklist
- Implementation hints
- Related files and dependencies
- Labels and milestones

**Expected Output Format:**

```markdown
# RemoveTabView Implementation Audit Report

## 📊 Executive Summary
- X% of requirements completed
- Y critical items remaining
- Z estimated days to completion

## 🔍 Detailed Compliance Analysis
[Detailed analysis per category]

## 📋 Prioritized Action Plan
[Ordered list of work items]

## 🚀 Implementation Roadmap
[Week-by-week implementation plan]

## 📝 GitHub Issues Ready for Creation
[Issue templates for immediate use]
```

**Additional Context:**
- Reference MTM MVVM patterns from copilot-instructions.md
- Use established service integration patterns
- Follow Avalonia AXAML syntax requirements
- Maintain consistency with InventoryTabView patterns
- Ensure comprehensive error handling integration

**Scope**: Focus on actionable, specific recommendations that can be immediately implemented.
