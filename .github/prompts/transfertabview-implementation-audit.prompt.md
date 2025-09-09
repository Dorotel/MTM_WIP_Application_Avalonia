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
