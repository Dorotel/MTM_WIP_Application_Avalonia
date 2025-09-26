# Implementation Audit Template

## 🔍 **Current State Analysis**

This template provides a systematic approach to compare current implementation against specifications and generate actionable work plans.

### **Phase 1: Specification Compliance Analysis**

**Prompt for @copilot:**
```
Using the specification in #file:removetabview-complete-implementation.yml, perform a comprehensive audit of the current RemoveTabView implementation. For each requirement category:

1. **UI Implementation**: 
   - Compare current RemoveTabView.axaml against DataGrid-centric layout requirements
   - Verify CollapsiblePanel integration and auto-behavior
   - Check TextBox + SuggestionOverlay replacement of ComboBoxes
   - Validate InventoryTabView styling consistency

2. **Service Integration**:
   - Audit QuickButtons, SuccessOverlay, ErrorHandling integration
   - Verify progress reporting in MainView status bar
   - Check SuggestionOverlay service connections

3. **Business Logic**:
   - Review RemoveItemViewModel batch operations
   - Verify transaction logging and audit trails
   - Check validation and error handling patterns

4. **Accessibility & UX**:
   - Audit keyboard shortcuts implementation
   - Check accessibility features (ToolTip.Tip, focus management)
   - Verify multi-row selection and user feedback

Generate a structured report with:
- ✅ Completed requirements (with file references)
- 🔄 Partially implemented (specific gaps)
- ❌ Missing requirements (priority order)
- 📋 Next action items (ordered by dependencies)
```

### **Phase 2: Implementation Priority Matrix**

**Prompt for @copilot:**
```
Based on the audit results from Phase 1, create a priority matrix for remaining work:

**Critical Path Items** (Blockers):
- Items that prevent core functionality
- Dependencies for other features
- User-facing critical issues

**High Priority** (Next Sprint):
- Enhanced user experience features
- Performance optimizations
- Integration completions

**Medium Priority** (Future Sprint):
- Nice-to-have enhancements
- Advanced features
- Optimization improvements

**Low Priority** (Backlog):
- Edge case handling
- Additional validations
- Documentation updates

Format as GitHub Issues with:
- Clear acceptance criteria
- Estimated effort (1-13 points)
- Dependencies and blockers
- Implementation hints
```

### **Phase 3: Implementation Roadmap**

**Prompt for @copilot:**
```
Create a detailed implementation roadmap based on the priority matrix:

**Week 1 Focus**: Critical Path Resolution
- List specific files to modify
- Required new components
- Service integrations
- Testing requirements

**Week 2 Focus**: High Priority Features
- Enhanced UX implementations
- Performance optimizations
- Integration completions

**Week 3+ Focus**: Polish & Optimization
- Remaining medium/low priority items
- Comprehensive testing
- Documentation completion

Include for each item:
- Estimated hours/days
- Required knowledge areas
- Potential risks/blockers
- Definition of done criteria
```

### **Usage Instructions**

1. **Run Phase 1 Audit**: Execute the specification compliance analysis prompt
2. **Generate Priority Matrix**: Use Phase 2 prompt with audit results
3. **Create Implementation Plan**: Execute Phase 3 with priority matrix output
4. **Create GitHub Issues**: Convert roadmap items to trackable GitHub issues
5. **Track Progress**: Use existing gap report template for ongoing status updates

This approach leverages:
- ✅ Your existing gap report structure
- ✅ Awesome-copilot systematic analysis patterns
- ✅ Your comprehensive specification requirements
- ✅ GitHub integration for project management
- ✅ Copilot's ability to analyze code against requirements

## 🎯 **Alternative Options**

### **Option 2: Automated GitHub Actions Workflow**

Create a workflow that:
- Runs on PR creation/updates
- Compares implementation against specification
- Generates automated gap reports
- Creates tracking issues for missing items

### **Option 3: Interactive Copilot Chat Mode**

Use a specialized chat mode for:
- Real-time implementation auditing
- Interactive requirement verification
- Dynamic priority adjustment
- Live progress tracking

## 📋 **Implementation Steps**

1. **Immediate**: Use Phase 1 prompt to audit current RemoveTabView state
2. **Next**: Generate priority matrix for remaining work
3. **Then**: Create implementation roadmap with timelines
4. **Finally**: Set up ongoing tracking system

This systematic approach will give you the most comprehensive view of what work remains and in what order it should be completed, leveraging both your existing infrastructure and awesome-copilot best practices.