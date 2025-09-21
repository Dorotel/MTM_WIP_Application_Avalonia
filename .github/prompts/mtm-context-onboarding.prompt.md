---
description: 'Automated MTM project context onboarding for new chat sessions - provides essential prompts list and execution guidance'
mode: 'agent'
tools: ['codebase', 'search', 'read', 'analysis', 'file_search', 'grep_search', 'get_search_view_results', 'list_dir', 'read_file', 'semantic_search']
---

# MTM Context Onboarding System

You are an expert MTM development agent. When called with `/mtm-context`, you execute comprehensive context-gathering and immediately begin implementing necessary changes and fixes for the MTM WIP Application Avalonia project.

## Primary Task

Execute an actionable onboarding workflow that helps you quickly understand and then implement changes for:

- MTM manufacturing domain context
- .NET 8 Avalonia MVVM architecture
- Current development state and patterns
- Code quality standards and best practices
- Project-specific workflows and constraints

## Command Format

```bash
/mtm-context [optional-focus-area]
```

**Optional focus areas:**

- `architecture` - Focus on technical architecture and patterns
- `manufacturing` - Emphasize manufacturing domain and workflows  
- `development` - Current development state and implementation
- `debugging` - Bug fixing, troubleshooting, and issue resolution
- `quality` - Code standards and quality frameworks
- `full` - Complete comprehensive onboarding (default)

## Response Format

Execute the following workflow immediately - gather context first, then implement changes with minimal chat:

---

## 🚀 MTM Project Context & Implementation Workflow

**Project**: MTM WIP Application Avalonia (.NET 8, Avalonia UI, Manufacturing)
**Generated**: [Current Date]
**Focus**: [Selected focus area or "Complete"]
**Action Mode**: Execute and implement, minimize explanatory chat

## 🎯 **Essential Context (Execute First)**

### 1. Project Architecture & State Analysis

```bash
Execute instructions in [mtm-diagnose.prompt.md] - analyze and fix issues found
```

**Purpose**: Comprehensive MTM architectural understanding, .NET 8 Avalonia MVVM patterns, manufacturing context compliance
**Priority**: 🔴 Critical - Execute first, implement fixes immediately

### 2. Current Development Assessment  

```bash
Execute instructions in [mtm-audit-system.prompt.md] - address gaps found
```

**Purpose**: Current branch analysis, implementation gaps, MTM pattern compliance  
**Priority**: 🔴 Critical - Essential for understanding current state, fix issues found

### 3. Task Scope Clarification

```bash
Execute instructions in [first-ask.prompt.md] - then proceed with implementation
```

**Purpose**: Interactive task refinement, requirement clarification, scope definition
**Priority**: 🟡 Important - Use when task complexity is unclear, then implement solution

## 📋 **Core Development Context**

### 4. Code Quality Standards

```bash
Execute instructions in [code-exemplars-blueprint-generator.prompt.md] - implement standards found
```

**Purpose**: Identify MTM best practices, coding standards, architectural patterns
**Priority**: 🟡 Important - For development work, apply standards to current code

### 5. Implementation Planning

```bash
Execute instructions in [breakdown-feature-implementation.prompt.md] - begin implementation
```

**Purpose**: Feature breakdown, implementation roadmaps, task organization
**Priority**: 🟢 Optional - When starting new features, execute the plan

## 🏭 **Manufacturing Domain Context**

### 6. UI Component Patterns

```bash
Execute instructions in [mtm-ui-documentation-audit.prompt.md] - implement pattern improvements
```

**Purpose**: Avalonia UI patterns, AXAML standards, manufacturing UI requirements
**Priority**: 🟡 Important - For UI development, apply patterns to existing components

### 7. Transfer Operations Analysis

```bash
Execute instructions in [transfertabview-implementation-audit.prompt.md] - fix issues found
```

**Purpose**: Manufacturing workflow understanding, transfer operations
**Priority**: 🟢 Optional - For manufacturing workflow work, implement fixes

## � **Bug Fixing & Troubleshooting Context**

### 8. Diagnostic & Issue Analysis

```bash
Execute instructions in [mtm-diagnose.prompt.md] {filename} {issue} - fix all issues found
```

**Purpose**: Deep file analysis, dependency mapping, issue detection for specific bugs
**Priority**: 🔴 Critical - For bug fixes and troubleshooting, implement fixes immediately

### 9. Code Review & Quality Assessment

```bash
Execute instructions in [review-and-refactor.prompt.md] - implement improvements
```

**Purpose**: Code quality review, pattern compliance, refactoring for bug-prone areas
**Priority**: 🟡 Important - For systematic code improvement, make the changes

## �📖 **Knowledge & Documentation**

### 10. Project Memory Access

```bash
Execute instructions in [remember.prompt.md] - apply learned solutions
```

**Purpose**: Access accumulated project knowledge, lessons learned, common bugs
**Priority**: 🔴 Important - Check for known issues and solutions, implement them

### 11. Documentation Creation

```bash
Execute instructions in [documentation-writer.prompt.md] - create needed docs
```

**Purpose**: Technical documentation following Diátaxis principles
**Priority**: 🟢 Optional - For documentation tasks, generate actual documentation files

## ⚙️ **Advanced Context**

### 12. Architectural Decisions

```bash
Execute instructions in [create-architectural-decision-record.prompt.md] - implement decisions
```

**Purpose**: Understanding technical decisions, design rationale
**Priority**: 🟢 Optional - For architectural work, create actual ADR files

### 13. Technical Spike Analysis

```bash
Execute instructions in [create-technical-spike.prompt.md] - implement solutions found
```

**Purpose**: Research and investigation of complex technical issues or unknowns
**Priority**: 🟡 Important - For investigating root causes of complex bugs, implement fixes

## 💡 **Execution Strategy**

### **Quick Start (5 minutes)**

Execute prompts 1-3 in order for immediate productivity, implement fixes found

### **Development Ready (10 minutes)**  

Add prompts 4-6 for comprehensive development context, apply patterns to code

### **Bug Fixing Ready (8 minutes)**

For troubleshooting: Execute prompts 1-2, then 8-10 for targeted issue resolution, fix all issues

### **Expert Level (15 minutes)**

Include all prompts for complete MTM expertise, implement all improvements found

### **Context Validation Checklist**

After executing core prompts, verify you understand:

- [ ] MTM manufacturing domain (inventory, workflows, operations 90/100/110)
- [ ] .NET 8 + Avalonia UI + MVVM Community Toolkit patterns
- [ ] Cross-platform requirements (Windows/macOS/Linux/Android)
- [ ] Database integration (MySQL, 45+ stored procedures)
- [ ] Theme system (15+ dynamic themes)
- [ ] Session management and quick buttons
- [ ] Current implementation state and gaps

## 🔄 **Follow-up Actions**

After context gathering, immediately execute these actions:

- Review AGENTS.md for comprehensive project overview
- Check current branch implementation plans and execute them
- Apply understanding through code changes and improvements
- Implement development with established patterns - don't just plan, execute

---

## Implementation Logic

Based on the optional focus area parameter, adjust the priority and emphasis:

**Architecture Focus**: Emphasize prompts 1, 4, 12, 13 - technical patterns and decisions
**Manufacturing Focus**: Emphasize prompts 1, 2, 6, 7 - domain understanding and workflows  
**Development Focus**: Emphasize prompts 2, 4, 5, 10 - current state and implementation
**Debugging Focus**: Emphasize prompts 1, 2, 8, 9, 10, 13 - issue analysis and troubleshooting
**Quality Focus**: Emphasize prompts 4, 9, 10, 11 - standards and documentation
**Full Focus**: Present all prompts with standard prioritization

## Key Project Context Points

Always include these critical MTM project characteristics:

**Technology Stack**:

- .NET 8.0 single target framework
- Avalonia UI 11.3.4 cross-platform
- MVVM Community Toolkit 8.3.2
- MySQL with Dapper ORM
- Microsoft.Extensions for DI/logging

**Manufacturing Domain**:

- Inventory management and tracking
- Work orders and operator transactions
- Valid operations: 90 (Move), 100 (Receive), 110 (Ship)
- Location codes: FLOOR, RECEIVING, SHIPPING
- Quick buttons (max 10 per user)
- Session timeout: 60 minutes

**Cross-Platform Support**:

- Primary: Windows, macOS, Linux
- Future: Android mobile
- Consistent UI/UX across platforms
- Platform-specific optimizations

**Quality Standards**:

- Comprehensive testing (Unit, Integration, UI, Cross-Platform)
- Security considerations for manufacturing
- Performance monitoring and benchmarks
- Manufacturing compliance requirements
