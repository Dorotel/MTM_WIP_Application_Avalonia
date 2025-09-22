---
mode: 'agent'
tools: ['codebase', 'search', 'read', 'analysis', 'file_search', 'grep_search', 'get_search_view_results', 'list_dir', 'read_file', 'semantic_search', 'joyride_evaluate_code', 'joyride_request_human_input', 'joyride_basics_for_agents', 'joyride_assisting_users_guide', 'web_search', 'run_terminal', 'edit_file', 'create_file', 'move_file', 'delete_file', 'git_operations', 'database_query', 'test_runner', 'documentation_generator', 'dependency_analyzer', 'performance_profiler', 'security_scanner', 'cross_platform_tester', 'ui_automation', 'manufacturing_domain_validator', 'copilot_optimizer']
---

## Implementation Mode

**CRITICAL**: You MUST implement all code changes directly using available tools. Do NOT provide code snippets or examples for the user to implement manually. When you identify issues, fixes, improvements, or requirements:

1. **Immediate Implementation**: Use edit tools to make actual changes to files
2. **Direct Action**: Create, modify, or update files as needed
3. **No Code Examples**: Avoid showing code blocks unless specifically requested for explanation
4. **Tool-First Approach**: Leverage all available tools to accomplish tasks programmatically
5. **Complete Solutions**: Finish implementation work rather than providing guidance

Your role is to **execute and implement**, not to advise or provide examples for manual implementation.

````prompt
# TransferTabView Implementation Audit Prompt

## Your Response must be placed in the .github/issues/TransferTabView/ Folder as a markdown file.

## 🎯 Comprehensive Implementation Status Analysis

**Context**: Compare current TransferTabView implementation against #file:transfertabview-complete-implementation.yml requirements and generate prioritized work plan.

**Instructions for @copilot**:

Perform a detailed audit of the current TransferTabView implementation by analyzing:

### **1. SPECIFICATION COMPLIANCE AUDIT**

**UI Implementation Analysis:**
- Compare `Views/MainForm/Panels/TransferTabView.axaml` against dual-panel layout requirements
- Verify CollapsiblePanel integration (`HeaderPosition`, auto-collapse/expand behavior)
- Check complete TextBox + SuggestionOverlay replacement of ComboBoxes (all 4 fields)
- Validate NumericUpDown control implementation for quantity management
- Audit InventoryTabView styling consistency (ScrollViewer > Grid[RowDefinitions="*,Auto"] > Border pattern)
- Verify MTM theme integration (DynamicResource bindings)
- Check transfer-specific UI elements (To Location field, quantity controls, transfer buttons)

**Service Integration Analysis:**
- Audit `ViewModels/MainForm/TransferItemViewModel.cs` for complete service integration:
  - ISuggestionOverlayService integration and methods for all input fields
  - ISuccessOverlayService integration for transfer confirmations with From→To location info
  - IQuickButtonsService integration for field population + YELLOW history logging
  - ErrorHandling service integration for centralized error management
  - Progress reporting integration with MainView status bar for transfer operations

**Business Logic Analysis:**
- Review transfer operation implementation with atomic transaction handling
- Verify single and multi-row selection support in DataGrid for batch transfers
- Check quantity validation logic (min=1, max=available inventory)
- Audit TRANSFER transaction logging (Yellow color coding, From→To tracking)
- Validate destination location validation (prevent same-location transfers)
- Check partial vs. complete quantity transfer logic
- Verify fuzzy validation integration (TextBoxFuzzyValidationBehavior)

**Critical Transaction Logic Audit:**
- Verify ALL transfers create "TRANSFER" transactions (regardless of operation numbers)
- Confirm operation numbers ("90", "100", "110") are treated as workflow steps ONLY
- Check transaction type determined by user action, NOT operation number
- Audit From Location and To Location recording in transaction history
- Verify complete audit trail: User, Timestamp, Part ID, Operation, From→To, Quantity
- Check Yellow color coding in QuickButtons history for TRANSFER operations

**User Experience Analysis:**
- Check keyboard shortcuts implementation (F5=Search, Enter=Transfer, Escape=Reset)
- Audit accessibility features (ToolTip.Tip, focus management, screen reader support)
- Verify loading states and "Nothing Found" indicators
- Check smooth panel animations and visual feedback
- Validate quantity management UX (increment/decrement, keyboard input, max validation)
- Review transfer confirmation workflows and success messaging

### **2. GENERATE COMPLIANCE MATRIX**

Create a structured compliance matrix:

```markdown
| Requirement Category | Requirement | Status | Implementation Details | Missing/Issues |
|---------------------|-------------|---------|----------------------|----------------|
| UI Layout | Dual-panel transfer layout | ✅/🔄/❌ | Current implementation | Specific gaps |
| Quantity Management | NumericUpDown controls | ✅/🔄/❌ | Current behavior | What's missing |
| Service Integration | SuggestionOverlay (4 fields) | ✅/🔄/❌ | Integration status | Missing methods |
| Transaction Logic | TRANSFER type enforcement | ✅/🔄/❌ | Business rules | Logic gaps |
| CollapsiblePanel | Auto-collapse/expand | ✅/🔄/❌ | Panel behavior | Missing triggers |
| DataGrid | Transfer candidate display | ✅/🔄/❌ | Grid functionality | Missing features |
| Validation | Destination location validation | ✅/🔄/❌ | Current validation | Missing rules |
| History Logging | Yellow TRANSFER coding | ✅/🔄/❌ | QuickButtons integration | Color/logging gaps |
| Progress Reporting | MainView status integration | ✅/🔄/❌ | Progress system | Missing updates |
| Success Overlay | From→To transfer confirmations | ✅/🔄/❌ | Overlay integration | Missing info |
| ... | ... | ... | ... | ... |
```

### **3. PRIORITIZED ACTION PLAN**

Generate a priority-ordered action plan:

**🚨 CRITICAL (Must Complete First)**
- [ ] Critical blockers preventing basic transfer functionality
- [ ] Missing NumericUpDown quantity controls
- [ ] Build-breaking issues in AXAML structure
- [ ] Core transfer workflow blockers
- [ ] TRANSFER transaction logic implementation

**⚡ HIGH PRIORITY (Next Sprint)**  
- [ ] Complete SuggestionOverlay integration for all 4 input fields
- [ ] To Location field implementation with validation
- [ ] Quantity validation and max limit enforcement
- [ ] CollapsiblePanel auto-collapse/expand behavior
- [ ] DataGrid transfer candidate display
- [ ] Success overlay with From→To location information

**📋 MEDIUM PRIORITY (Future Sprint)**
- [ ] Batch transfer operations (multi-row selection)
- [ ] Advanced quantity management features
- [ ] Progress reporting enhancements
- [ ] QuickButtons Yellow history logging
- [ ] Enhanced user experience features
- [ ] Print transfer report functionality

**📝 LOW PRIORITY (Backlog)**
- [ ] Advanced transfer validation rules
- [ ] Performance optimizations for large datasets
- [ ] Additional keyboard shortcuts
- [ ] Edge case handling
- [ ] Documentation updates
- [ ] Optional UI enhancements

### **4. IMPLEMENTATION ROADMAP**

For each priority level, provide:
- **Specific files to modify** (with line references when possible)
- **New components/services needed** 
- **ViewModel integration points** (TransferItemViewModel bindings)
- **Estimated effort** (hours/days)
- **Dependencies and blockers**
- **Acceptance criteria** (per specification requirements)
- **Testing requirements** (transfer operations, validation, UI)

### **5. CURRENT IMPLEMENTATION ANALYSIS**

Based on the provided TransferTabView.axaml, analyze:

**Currently Implemented (✅)**:
- Basic AXAML structure with UserControl inheritance
- MTM theme styling framework (DynamicResource bindings)  
- ScrollViewer > Grid[RowDefinitions="*,Auto"] layout foundation
- Material Icons integration (xmlns:materialIcons)
- Behaviors integration (xmlns:behaviors)
- ViewModel binding (x:DataType="vm:TransferItemViewModel")
- CollapsiblePanel structure for Transfer Configuration
- Input field foundations (Part ID, Operation, To Location, Quantity grids)
- DataGrid container structure with loading/nothing found overlays
- Action buttons foundation (Search, Transfer, Reset, Print)
- Comprehensive styling (TextBox.input-field, NumericUpDown.input-field, Button styles)

**Critical Missing Elements (❌)**:
- Complete NumericUpDown implementation in Quantity field
- To Location field icon and label implementation
- Quantity field icon and label implementation  
- Complete DataGrid column definitions and bindings
- DataGrid header content (icon, title, count badge)
- Loading overlay complete implementation
- Nothing Found indicator complete content
- Complete Search/Reset button command bindings and behavior
- SuggestionOverlay behavior integration on input fields
- CollapsiblePanel auto-collapse/expand triggers

**Partially Implemented (🔄)**:
- Input field structure exists but missing complete content
- DataGrid structure exists but missing column definitions
- Action buttons exist but may need complete command integration
- Styling framework complete but needs validation on all elements

### **6. GITHUB ISSUES CREATION PLAN**

Generate ready-to-use GitHub issue templates for each major work item:
- Clear title and description
- Acceptance criteria checklist based on specification
- Implementation hints referencing current TransferTabView.axaml structure
- Related files and dependencies (TransferItemViewModel integration)
- Labels (transfer-operations, ui, enhancement, priority-high)
- Milestones (TransferTabView Complete Implementation)

**Expected Output Format:**

```markdown
# TransferTabView Implementation Audit Report

## 📊 Executive Summary
- X% of requirements completed
- Y critical items remaining  
- Z estimated days to completion
- Current status: [Foundation Complete/In Progress/Needs Major Work]

## 🔍 Detailed Compliance Analysis
[Detailed analysis per category with specific AXAML line references]

## 📋 Prioritized Action Plan
[Ordered list of work items with effort estimates]

## 🚀 Implementation Roadmap
[Week-by-week implementation plan]

## 📝 GitHub Issues Ready for Creation
[Issue templates for immediate use]

## 🎯 Next Immediate Actions
[Top 3-5 actionable items to start immediately]
```

**Additional Context:**
- Reference MTM MVVM patterns from copilot-instructions.md
- Use established service integration patterns from SuggestionOverlay.cs
- Follow Avalonia AXAML syntax requirements (prevent AVLN2000 errors)
- Maintain consistency with InventoryTabView patterns
- Ensure comprehensive error handling integration
- Focus on TransferItemViewModel (908 lines) integration points
- Emphasize TRANSFER transaction logic vs. operation number workflow steps
- Highlight Yellow color coding requirement for transfer history

**Scope**: Focus on actionable, specific recommendations that can be immediately implemented to complete the TransferTabView functionality per specification requirements.

**Transfer-Specific Considerations:**
- NumericUpDown quantity management (min=1, max=available)
- Destination location validation (prevent same-location transfers)
- From→To location tracking in all transactions and success messages
- TRANSFER transaction type enforcement regardless of operation numbers
- Partial vs. complete quantity transfer logic
- Batch transfer operations with atomic transaction handling
- Yellow color coding for transfer operations in QuickButtons history
````

## 🤖 Joyride Automation Capabilities

**Enhanced with Joyride VS Code Extension API automation** for dynamic workflow creation and advanced VS Code manipulation:

### Core Joyride Integration

- **`joyride_evaluate_code`**: Execute ClojureScript directly in VS Code Extension Host environment
- **`joyride_request_human_input`**: Interactive human-in-the-loop workflows for domain decisions
- **`joyride_basics_for_agents`**: Access Joyride automation patterns and capabilities
- **`joyride_assisting_users_guide`**: User-focused Joyride guidance and assistance

### Advanced Automation Capabilities

**VS Code API Access**: Full Extension API access for workspace manipulation, UI automation, and system integration

**Interactive Workflows**: Dynamic user input collection for complex decision-making scenarios

**Real-time Validation**: Live code execution and testing within VS Code environment

**Custom Automation Scripts**: Create reusable ClojureScript automation for MTM-specific workflows

### MTM-Specific Joyride Applications

- **File Template Generation**: Automated ViewModel/Service creation following MTM patterns
- **MVVM Pattern Enforcement**: Dynamic validation and correction of Community Toolkit usage
- **Theme System Automation**: Automated theme switching and resource validation workflows
- **Database Integration Testing**: Live stored procedure validation and connection testing
- **Cross-Platform Validation**: Automated testing across Windows/macOS/Linux environments
- **Manufacturing Workflow Automation**: Inventory operation validation and transaction testing

### Workflow Enhancement Examples

```clojure
;; Example: Automated MVVM pattern validation
(joyride_evaluate_code 
  "(require '["vscode" :as vscode])
   (vscode/window.showInformationMessage \"Validating MVVM patterns...\")")

;; Example: Interactive domain clarification
(joyride_request_human_input 
  "Specify manufacturing operation type (90/100/110):")
```

**Integration Benefit**: Combines traditional file analysis tools with live VS Code automation for comprehensive MTM development workflow enhancement.

