# MTM Documentation Master Index

**Generated**: September 10, 2025  
**Purpose**: Complete inventory and reorganization guide for all MTM WIP Application documentation  
**Status**: Phase 1 - Analysis Complete, Updated for Awesome-Copilot Compliance

## Overview

This master index catalogs all documentation files identified in the MTM WIP Application repository and provides the restructuring plan to organize them into the new `.github` folder hierarchy following official GitHub Copilot awesome-copilot repository standards from https://github.com/github/awesome-copilot.

## New Documentation Structure (Awesome-Copilot Compliant)

### Folder Hierarchy Pattern
```
.github/
├── prompts/              # Task-specific prompts (.prompt.md)  
├── instructions/         # Coding standards and best practices (.instructions.md)
├── chatmodes/           # AI personas and specialized modes (.chatmode.md)
├── Project-Management/
│   ├── Planning/
│   ├── Requirements/
│   └── Implementation/
├── Development-Guides/
│   ├── Setup-Configuration/
│   ├── Code-Standards/
│   ├── Testing-Procedures/
│   ├── UI-Components/
│   ├── Components/
│   └── Services/
├── Architecture-Documentation/
│   ├── System-Design/
│   ├── Data-Models/
│   └── Service-Architecture/
├── Operations/
│   ├── Deployment/
│   ├── Monitoring/
│   ├── Maintenance/
│   └── Scripts/
└── Documentation-Management/
```

### File Naming Convention (Awesome-Copilot Standards)
- **Prompts**: `{task-name}.prompt.md` (accessible via `/` commands in Copilot Chat)
- **Instructions**: `{framework-or-pattern}.instructions.md` (apply automatically to file patterns)
- **Chat Modes**: `{persona-name}.chatmode.md` (specialized AI assistants)
- **Guides**: `guide_{name}.md` (traditional documentation)
- **Specifications**: `spec_{name}.md` (technical specifications)
- **Requirements**: `req_{name}.md` (requirements documents)

### Required Frontmatter (Awesome-Copilot Format)
```yaml
---
description: 'Brief description of the file purpose'
tools: ['codebase', 'fetch', 'search', 'usages'] # Available tools array
---
```

## Documentation Inventory & Migration Plan (Updated for Awesome-Copilot)

### Phase 1: GitHub Copilot Core Files (Awesome-Copilot Structure)

#### Current Copilot Instructions (Moving to `.github/instructions/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `.github/copilot-instructions.md` | `.github/instructions/` | `mtm-master-guide.instructions.md` | Master MTM development guide |
| `.github/UI-Instructions/avalonia-xaml-syntax.instruction.md` | `.github/instructions/` | `avalonia-xaml-syntax.instructions.md` | Avalonia AXAML syntax rules |
| `.github/UI-Instructions/ui-generation.instruction.md` | `.github/instructions/` | `mtm-ui-generation.instructions.md` | MTM UI component generation |
| `.github/UI-Instructions/ui-styling.instruction.md` | `.github/instructions/` | `mtm-ui-styling.instructions.md` | MTM design system styling |
| `.github/UI-Instructions/ui-mapping.instruction.md` | `.github/instructions/` | `ui-viewmodel-mapping.instructions.md` | UI to ViewModel mapping |
| `.github/Development-Instructions/database-patterns.instruction.md` | `.github/instructions/` | `mtm-database-patterns.instructions.md` | Database access patterns |
| `.github/Development-Instructions/stored-procedures.instruction.md` | `.github/instructions/` | `mtm-stored-procedures.instructions.md` | Stored procedure usage |
| `.github/Development-Instructions/errorhandler.instruction.md` | `.github/instructions/` | `mtm-error-handler.instructions.md` | Error handling patterns |
| `.github/Core-Instructions/dependency-injection.instruction.md` | `.github/instructions/` | `mtm-dependency-injection.instructions.md` | DI patterns |
| `.github/Core-Instructions/naming.conventions.instruction.md` | `.github/instructions/` | `mtm-naming-conventions.instructions.md` | Naming standards |

#### Current Copilot Templates (Moving to `.github/prompts/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `.github/copilot/templates/mtm-feature-request.md` | `.github/prompts/` | `mtm-feature-request.prompt.md` | Feature request creation |
| `.github/copilot/templates/mtm-ui-component.md` | `.github/prompts/` | `mtm-ui-component.prompt.md` | UI component creation |
| `.github/copilot/templates/mtm-viewmodel-creation.md` | `.github/prompts/` | `mtm-viewmodel-creation.prompt.md` | ViewModel creation |
| `.github/copilot/templates/mtm-database-operation.md` | `.github/prompts/` | `mtm-database-operation.prompt.md` | Database operation creation |
| `.github/copilot/templates/mtm-service-implementation.md` | `.github/prompts/` | `mtm-service-implementation.prompt.md` | Service implementation |
| `.github/prompts/breakdown-feature-prd.prompt.md` | `.github/prompts/` | `breakdown-feature-prd.prompt.md` | PRD creation prompt |

#### Current Copilot Context & Patterns (Moving to `.github/chatmodes/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `.github/copilot/context/mtm-business-domain.md` | `.github/chatmodes/` | `mtm-manufacturing-expert.chatmode.md` | Manufacturing domain expert mode |
| `.github/copilot/context/mtm-technology-stack.md` | `.github/chatmodes/` | `mtm-technology-advisor.chatmode.md` | Technology stack advisor mode |
| `.github/copilot/context/mtm-architecture-patterns.md` | `.github/chatmodes/` | `mtm-architect.chatmode.md` | Architecture guidance mode |
| `.github/copilot/patterns/mtm-mvvm-community-toolkit.md` | `.github/instructions/` | `mvvm-community-toolkit.instructions.md` | MVVM Community Toolkit patterns |
| `.github/copilot/patterns/mtm-stored-procedures-only.md` | `.github/instructions/` | `stored-procedures-only.instructions.md` | Stored procedures only pattern |
| `.github/copilot/patterns/mtm-avalonia-syntax.md` | `.github/instructions/` | `avalonia-ui-patterns.instructions.md` | Avalonia UI syntax patterns |

### Phase 2: Project Management Documentation

#### Current Files (Moving to `.github/Project-Management/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `docs/ways-of-work/plan/*/implementation-plan.md` | `.github/Project-Management/Implementation/` | `plan_{feature-name}-implementation.md` | Feature implementation plans |
| `docs/ways-of-work/plan/*/prd.md` | `.github/Project-Management/Requirements/` | `req_{feature-name}-prd.md` | Product requirements documents |
| `.github/prompts/breakdown-feature-prd.prompt.md` | `.github/Copilot-Templates/Feature-Templates/` | `prompt_breakdown-feature-prd.md` | PRD creation prompt |

### Phase 3: Development Documentation

#### Current Files (Moving to `.github/Development-Guides/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `Documentation/Development/UI_Documentation/Forms/*.instructions.md` | `.github/Development-Guides/UI-Components/` | `guide_{form-name}.md` | Form-specific guides |
| `Documentation/Development/success-overlay-system-implementation.md` | `.github/Development-Guides/Components/` | `guide_success-overlay-system.md` | Success overlay implementation |
| `Documentation/FileLogging-README.md` | `.github/Development-Guides/Services/` | `guide_file-logging-service.md` | File logging service guide |
| `Documentation/RemoveTabView-Implementation-Complete.md` | `.github/Development-Guides/Components/` | `guide_remove-tab-view.md` | Remove tab view implementation |

### Phase 4: Architecture Documentation  

#### Current Files (Moving to `.github/Architecture-Documentation/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `docs/architecture/*` | `.github/Architecture-Documentation/System-Design/` | `spec_{component-name}.md` | System architecture specs |
| `.github/database-documentation/*` | `.github/Architecture-Documentation/Data-Models/` | `spec_database-{model}.md` | Database model specifications |

### Phase 5: Testing & Quality Assurance

#### Current Files (Moving to `.github/Development-Guides/Testing/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `Documentation/RemoveTabView_Integration_Tests.md` | `.github/Development-Guides/Testing/` | `guide_remove-tab-view-testing.md` | Integration testing guide |
| `docs/development/view-management-questionnaires/*.html` | `.github/Development-Guides/Testing/` | `questionnaire_{type}.md` | View management questionnaires |

### Phase 6: Operational Documentation

#### Current Files (Moving to `.github/Operations/`)

| Current File | New Location | New Name | Description |
|-------------|--------------|----------|-------------|
| `scripts/README.md` | `.github/Operations/Scripts/` | `guide_automation-scripts.md` | Script usage guide |
| `docs/ci-cd/*` | `.github/Operations/Deployment/` | `guide_{deployment-type}.md` | Deployment guides |

## File-by-File User Stories & Use Cases

### Copilot Instructions
- **User Story**: As a developer using GitHub Copilot, I want clear, categorized instructions so that I can get accurate code suggestions that follow MTM patterns.
- **Use Case**: When implementing a new feature, reference the appropriate instruction files to ensure Copilot generates code following established patterns.

### Templates
- **User Story**: As a developer, I want standardized templates for common tasks so that I can consistently create features, components, and services.
- **Use Case**: When starting a new feature, use the feature request template to ensure all requirements are captured correctly.

### Context Files
- **User Story**: As a developer, I want comprehensive context about the business domain and technology stack so that I can make informed architectural decisions.
- **Use Case**: Before making changes to inventory management, reference the business domain context to understand manufacturing workflows.

### Pattern Files
- **User Story**: As a developer, I want established code patterns documented so that I can maintain consistency across the codebase.
- **Use Case**: When creating a new ViewModel, reference the MVVM Community Toolkit patterns to ensure proper implementation.

### Development Guides
- **User Story**: As a new team member, I want step-by-step guides for common development tasks so that I can be productive quickly.
- **Use Case**: When setting up the development environment, follow the setup configuration guide to ensure all dependencies are properly installed.

## Archive Plan

### Files to Archive (Original Structure Preserved)
- All files in `/docs/` directory
- All files in `/Documentation/` directory  
- Root level documentation files (`prompt.md`, `README.md`, etc.)
- Any orphaned documentation files

### Archive Structure
```
MTM_Documentation_Archive_2025-09-10.zip
├── docs/
├── Documentation/
├── scripts/README.md
├── prompt.md
└── [other root level docs]
```

## Interactive HTML Documentation System

### Overview
A comprehensive, user-friendly HTML documentation system will be created to provide:
- **Plain English explanations** of all files and their purposes
- **Real-world usage scenarios** with step-by-step examples  
- **Interactive navigation** matching the exact folder structure
- **Quick search and filtering** capabilities
- **Mobile-responsive design** for accessibility

### HTML Documentation Structure
```
.github/Documentation-Management/
├── html-documentation/
│   ├── index.html                 # Main navigation and overview
│   ├── assets/
│   │   ├── styles.css            # Custom MTM styling
│   │   ├── navigation.js         # Interactive folder tree
│   │   └── search.js             # Search and filter functionality
│   ├── pages/
│   │   ├── prompts/              # Prompts usage examples
│   │   ├── instructions/         # Instructions implementation guides  
│   │   ├── chatmodes/           # Chat mode activation tutorials
│   │   ├── project-management/   # PM workflow guides
│   │   ├── development-guides/   # Developer tutorials
│   │   └── architecture/         # Architecture documentation
│   └── scenarios/
│       ├── new-developer-onboarding.html
│       ├── feature-development-workflow.html
│       ├── code-review-process.html
│       └── troubleshooting-guide.html
```

### Real-World Scenarios Documentation

#### Scenario 1: New Developer Onboarding
**File**: `scenarios/new-developer-onboarding.html`
- **Step 1**: Set up development environment using setup guides
- **Step 2**: Install awesome-copilot extensions and configure MTM chat modes
- **Step 3**: Practice with MTM-specific prompts for common tasks
- **Step 4**: Review architecture documentation and coding standards

#### Scenario 2: Feature Development Workflow  
**File**: `scenarios/feature-development-workflow.html`
- **Step 1**: Create feature request using `mtm-feature-request.prompt.md`
- **Step 2**: Activate `mtm-architect.chatmode.md` for design guidance
- **Step 3**: Use UI component prompts for interface development
- **Step 4**: Apply database operation patterns for data layer
- **Step 5**: Implement error handling following MTM standards

#### Scenario 3: Code Review Process
**File**: `scenarios/code-review-process.html`
- **Step 1**: Use MTM coding standards instructions for review checklist
- **Step 2**: Activate quality assurance chat modes
- **Step 3**: Validate against architecture patterns
- **Step 4**: Ensure naming convention compliance

### Interactive Features

#### 1. Folder Structure Navigator
```html
<div class="folder-navigator">
  <ul class="folder-tree">
    <li class="folder">
      <span class="folder-icon">📁</span> prompts/
      <ul class="file-list">
        <li class="file" data-file="mtm-feature-request.prompt.md">
          <span class="file-icon">📄</span> mtm-feature-request.prompt.md
          <span class="file-description">Create comprehensive feature requests</span>
        </li>
      </ul>
    </li>
  </ul>
</div>
```

#### 2. Search and Filter System
```html
<div class="search-system">
  <input type="text" id="documentation-search" placeholder="Search documentation...">
  <div class="filter-options">
    <label><input type="checkbox" value="prompts"> Prompts</label>
    <label><input type="checkbox" value="instructions"> Instructions</label>
    <label><input type="checkbox" value="chatmodes"> Chat Modes</label>
  </div>
</div>
```

#### 3. Usage Examples with Copy-Paste Code
```html
<div class="usage-example">
  <h4>Using the MTM Feature Request Prompt</h4>
  <div class="step">
    <strong>Step 1:</strong> Open GitHub Copilot Chat in VS Code
  </div>
  <div class="step">
    <strong>Step 2:</strong> Type the command:
    <div class="code-example">
      <code>/mtm-feature-request</code>
      <button class="copy-btn" data-copy="/mtm-feature-request">Copy</button>
    </div>
  </div>
  <div class="step">
    <strong>Step 3:</strong> Provide your feature description when prompted
  </div>
</div>
```

## Implementation Timeline (Updated)

### Week 1: Analysis & Setup
- [x] Create master inventory
- [x] Define new folder structure following awesome-copilot standards
- [x] Create placeholder folders

### Week 2: Migration & Recreation
- [ ] Move and recreate Copilot-related documentation  
- [ ] Move and recreate development guides
- [ ] Update all cross-references
- [ ] **NEW**: Create HTML documentation structure and templates

### Week 3: Quality Assurance & HTML Development
- [ ] Comprehensive audit of migrated content
- [ ] Validate all links and references
- [ ] **NEW**: Build interactive HTML documentation system
- [ ] **NEW**: Create real-world usage scenarios
- [ ] **NEW**: Implement search and navigation features

### Week 4: Cleanup & Final Documentation
- [ ] Create archive of old documentation
- [ ] Remove old documentation files
- [ ] Final validation of new structure
- [ ] **NEW**: Complete HTML documentation with all scenarios
- [ ] **NEW**: Test HTML documentation across devices and browsers
- [ ] Update master index with completion status

## Next Actions

1. **Create Folder Structure**: Implement the defined hierarchy in `.github/`
2. **Migrate Copilot Files**: Start with highest priority GitHub Copilot documentation
3. **Recreate Content**: Generate new documentation based on current application state
4. **Update Cross-References**: Ensure all internal links point to new locations
5. **Run Audit**: Verify no content has been lost
6. **Archive & Cleanup**: Create archive and remove old files

---

This master index will be updated throughout the implementation process to track progress and ensure no documentation is forgotten during the restructure.
