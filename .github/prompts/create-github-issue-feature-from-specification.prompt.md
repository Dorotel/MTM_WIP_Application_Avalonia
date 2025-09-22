---
mode: 'agent'
description: 'Create GitHub Issue for feature request from specification file using feature_request.yml template.'
tools: ['*']
---
# Create GitHub Issue from Specification

Create GitHub Issue for the specification at `${file}`.

## Process

1. Analyze specification file to extract requirements
2. Check existing issues using `search_issues`
3. Create new issue using `create_issue` or update existing with `update_issue`
4. Use `feature_request.yml` template (fallback to default)

## Requirements

- Single issue for the complete specification
- Clear title identifying the specification
- Include only changes required by the specification
- Verify against existing issues before creation

## Issue Content

- Title: Feature name from specification
- Description: Problem statement, proposed solution, and context
- Labels: feature, enhancement (as appropriate)

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
