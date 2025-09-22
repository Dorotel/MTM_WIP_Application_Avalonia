---
mode: 'agent'
tools: ['*']
---
# GitHub Issues Generator for Implementation Gaps

## 🎯 Convert Audit Results to Trackable GitHub Issues

**Context**: Take audit results from removetabview-implementation-audit.prompt.md and generate specific GitHub issues for project tracking.

**Instructions for @copilot**:

Based on the implementation audit results, create detailed GitHub issues using the following templates:

### **CRITICAL ISSUE TEMPLATE**

```markdown
---
title: "[CRITICAL] [Specific Issue Title]"
labels: ["critical", "bug", "feature-blocker", "removetabview"]
assignees: []
---

## 🚨 Critical Issue Description

**Impact**: [Specific impact on user workflow]
**Priority**: Critical (blocks core functionality)

### **Current State**
- [ ] What currently exists/doesn't work

### **Required Implementation**
- [ ] Specific requirement from specification
- [ ] Exact implementation details needed

### **Acceptance Criteria**
- [ ] Testable condition 1
- [ ] Testable condition 2
- [ ] Testable condition 3

### **Files to Modify**
- `path/to/file1.cs` - [specific changes needed]
- `path/to/file2.axaml` - [specific changes needed]

### **Implementation Hints**
```csharp
// Code example or pattern to follow
```

### **Dependencies**

- Depends on: [other issues/requirements]
- Blocks: [issues that can't proceed without this]

### **Definition of Done**

- [ ] Implementation complete
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Code review approved
- [ ] Documentation updated

**Estimated Effort**: [1-13 story points]

```

### **HIGH PRIORITY ISSUE TEMPLATE**

```markdown
---
title: "[ENHANCEMENT] [Specific Feature Title]"
labels: ["enhancement", "high-priority", "removetabview"]  
assignees: []
---

## ⚡ Enhancement Description

**User Value**: [How this improves user experience]
**Priority**: High (next sprint)

### **Current State**
[What works now and what's missing]

### **Desired State**
[What should happen after implementation]

### **Implementation Approach**
1. [Step 1 with file references]
2. [Step 2 with code patterns]
3. [Step 3 with testing approach]

### **Acceptance Criteria**
- [ ] [Specific testable criteria]
- [ ] [User workflow criteria]
- [ ] [Technical criteria]

**Estimated Effort**: [1-13 story points]
```

### **DOCUMENTATION ISSUE TEMPLATE**

```markdown
---
title: "[DOCS] [Documentation Area]"  
labels: ["documentation", "low-priority"]
assignees: []
---

## 📚 Documentation Request

**Purpose**: [Why this documentation is needed]

### **Content Requirements**
- [ ] [Specific documentation item]
- [ ] [Code examples needed]
- [ ] [Usage patterns to document]

### **Target Audience**
- [ ] Developers working on RemoveTabView
- [ ] Future maintainers
- [ ] Integration developers

**Estimated Effort**: [1-5 story points]
```

### **TESTING ISSUE TEMPLATE**

```markdown
---
title: "[TEST] [Testing Scope]"
labels: ["testing", "quality-assurance"]
assignees: []
---

## 🧪 Testing Requirements

**Coverage Area**: [What needs testing]
**Test Types**: [Unit/Integration/UI/Performance]

### **Test Scenarios**
- [ ] [Specific test scenario 1]
- [ ] [Specific test scenario 2]
- [ ] [Edge case testing]

### **Implementation Requirements**
- Test framework: [xUnit/Avalonia.Headless/etc]
- Mock requirements: [What needs mocking]
- Performance targets: [Specific metrics]

### **Acceptance Criteria**
- [ ] All tests pass
- [ ] Code coverage meets standards
- [ ] Performance tests meet targets

**Estimated Effort**: [1-8 story points]
```

### **ISSUE GENERATION INSTRUCTIONS**

For each gap identified in the audit:

1. **Categorize by priority** (Critical/High/Medium/Low)
2. **Use appropriate template** based on issue type
3. **Fill in specific details** from audit findings
4. **Reference exact files and line numbers** when possible
5. **Include code examples** for complex requirements
6. **Set realistic effort estimates** based on complexity
7. **Link related issues** to show dependencies

### **MILESTONE PLANNING**

Create milestone suggestions:

- **RemoveTabView MVP** (Critical + High priority issues)
- **RemoveTabView Complete** (All remaining issues)
- **RemoveTabView Polish** (Documentation + optimization)

### **LABEL STRATEGY**

Consistent labeling for filtering:

- `critical`, `high-priority`, `medium-priority`, `low-priority`
- `bug`, `enhancement`, `documentation`, `testing`
- `removetabview`, `ui`, `service-integration`, `accessibility`
- `feature-blocker`, `quality-improvement`

**Expected Output**: Ready-to-paste GitHub issue content with proper formatting, complete details, and actionable requirements that can be immediately created and assigned.

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
