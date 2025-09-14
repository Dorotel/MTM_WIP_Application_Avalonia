---
mode: 'agent'
description: 'Update an existing implementation plan file with new or update requirements to provide new features, refactoring existing code or upgrading packages, design, architecture or infrastructure.'
tools: ['changes', 'codebase', 'editFiles', 'extensions', 'fetch', 'githubRepo', 'openSimpleBrowser', 'problems', 'runTasks', 'search', 'searchResults', 'terminalLastCommand', 'terminalSelection', 'testFailure', 'usages', 'vscodeAPI']
---
# Update Implementation Plan

## Primary Directive

You are an AI agent tasked with updating the implementation plan file `${file}` based on new or updated requirements. Your output must be machine-readable, deterministic, and structured for autonomous execution by other AI systems or humans.

## Execution Context

This prompt is designed for AI-to-AI communication and automated processing. All instructions must be interpreted literally and executed systematically without human interpretation or clarification.

## Core Requirements

- Generate implementation plans that are fully executable by AI agents or humans
- Use deterministic language with zero ambiguity
- Structure all content for automated parsing and execution
- Ensure complete self-containment with no external dependencies for understanding

## Plan Structure Requirements

Plans must consist of discrete, atomic phases containing executable tasks. Each phase must be independently processable by AI agents or humans without cross-phase dependencies unless explicitly declared.

## Phase Architecture

- Each phase must have measurable completion criteria
- Tasks within phases must be executable in parallel unless dependencies are specified
- All task descriptions must include specific file paths, function names, and exact implementation details
- No task should require human interpretation or decision-making

## AI-Optimized Implementation Standards

- Use explicit, unambiguous language with zero interpretation required
- Structure all content as machine-parseable formats (tables, lists, structured data)
- Include specific file paths, line numbers, and exact code references where applicable
- Define all variables, constants, and configuration values explicitly
- Provide complete context within each task description
- Use standardized prefixes for all identifiers (REQ-, TASK-, etc.)
- Include validation criteria that can be automatically verified

## Output File Specifications

- Save implementation plan files in `/plan/` directory
- Use naming convention: `[purpose]-[component]-[version].md`
- Purpose prefixes: `upgrade|refactor|feature|data|infrastructure|process|architecture|design`
- Example: `upgrade-system-command-4.md`, `feature-auth-module-1.md`
- File must be valid Markdown with proper front matter structure

## Progress Reporting Requirements

All progress updates during implementation must use a simple, standardized format for maximum clarity and efficiency:

### Required Update Report Format
```
## Implementation Progress Report

**Progress**: [Completed Files] / [Total Files] ([Percentage]%)
**Current Phase**: [Phase Number] - [Phase Name]  
**Current Task**: TASK-[Number] - [Brief Task Description]

**Implementation Plan Updated**: ✅ Tasks [Start-Number] through [End-Number] marked complete
**Task Table Status**: [Number] tasks updated with completion dates in implementation plan

**Relevant Notes**: 
- [Key observation or issue encountered]
- [Important decision made or pattern discovered]
- [Any blockers or dependencies identified]

**Completed Files This Session**:
- [Full file path] - [Brief description of changes]
- [Full file path] - [Brief description of changes]
- [Additional files as applicable]

**Next Actions**: 
- [Immediate next task to be executed]
- [Any preparatory work needed]
- Update implementation plan task table for next batch of tasks
```

### Reporting Standards
- Updates required every 5 completed tasks (at each checkpoint)
- Keep descriptions concise and actionable
- Focus on concrete progress rather than process details
- Highlight any deviations from plan or unexpected discoveries

## Agent-Driven PR Continuation Prompt

When executing this implementation plan as an agent-driven PR, use the following prompt for systematic execution:

```prompt
# MTM Documentation Optimization - Implementation Execution

## Context
You are executing the MTM Documentation Validation and GitHub Copilot Optimization implementation plan. This is a comprehensive 8-phase, 80-task project to migrate docs/ content to .github/, eliminate duplicates, and optimize GitHub Copilot functionality.

## Your Mission
Execute tasks systematically following the implementation plan located at `.github/prompts/update-implementation-plan.prompt.md`. You must:

1. **Follow the exact task sequence** outlined in the implementation phases
2. **Provide progress reports** every 5 completed tasks using the standardized format
3. **Create/update files** as specified in each task description
4. **Validate completion** of each task before proceeding
5. **Handle the quarantine folder** operations for redundant files

## Critical Requirements
- **NEVER delete files** - always relocate to quarantine/ folder
- **Preserve content** during all migration and consolidation operations  
- **Update cross-references** when files are moved or consolidated
- **Test GitHub Copilot** functionality with updated instructions
- **Maintain checkpoint validation** every 5 tasks
- **UPDATE THE IMPLEMENTATION PLAN** - Mark tasks as completed with dates in the task tables
- **Track progress in real-time** - Update completion status to avoid duplicate work verification

## Execution Process
1. **Read the full implementation plan** to understand scope and dependencies
2. **Check existing completion status** in task tables before starting any work
3. **Start with Phase 1, Task 1** - scan and categorize all documentation files
4. **Execute tasks sequentially** within each phase
5. **IMMEDIATELY update task table** with completion date when task is finished
6. **Report progress** at each checkpoint (tasks 5, 10, 15, etc.)
7. **Validate completion criteria** before marking tasks as complete
8. **Update the implementation plan file** with completion dates and status

## Implementation Plan Maintenance
**CRITICAL**: After completing each task, you MUST update the corresponding task table entry:

```markdown
| TASK-XXX | Task description | ✅ | 2025-09-14 |
```

This prevents future agents from:
- Re-executing completed tasks
- Spending time verifying what work has been done
- Creating duplicate efforts or conflicting changes
- Losing track of actual progress

**File to Update**: `.github/prompts/update-implementation-plan.prompt.md`
**Section to Update**: Implementation Steps task tables
**Format**: Replace empty completion cells with ✅ and completion date

## Success Metrics
- Reduce file count from 502+ to ~150 core files in .github/
- All docs/ content successfully migrated to .github/ structure
- Quarantine folder organized with all redundant/duplicate files
- All cross-references updated and validated
- GitHub Copilot instructions tested and optimized

## Emergency Protocols
- If any task encounters blockers, document in progress report
- If file structure changes unexpectedly, pause and reassess
- If critical files are at risk, create backup before proceeding
- If GitHub Copilot effectiveness decreases, rollback and investigate

**Start Execution**: Begin with Implementation Phase 1, Task 1 and provide your first progress report after completing Task 5.
```

This prompt should be used to initialize any agent that will continue this implementation work.

## Mandatory Template Structure

All implementation plans must strictly adhere to the following template. Each section is required and must be populated with specific, actionable content. AI agents must validate template compliance before execution.

## Template Validation Rules

- All front matter fields must be present and properly formatted
- All section headers must match exactly (case-sensitive)
- All identifier prefixes must follow the specified format
- Tables must include all required columns
- No placeholder text may remain in the final output

## Status

The status of the implementation plan must be clearly defined in the front matter and must reflect the current state of the plan. The status can be one of the following (status_color in brackets): `Completed` (bright green badge), `In progress` (yellow badge), `Planned` (blue badge), `Deprecated` (red badge), or `On Hold` (orange badge). It should also be displayed as a badge in the introduction section.

```md
---
goal: 'MTM WIP Application - Documentation Validation and GitHub Copilot Optimization'
version: '1.0'
date_created: '2025-09-14'
last_updated: '2025-09-14'
owner: 'MTM Development Team'
status: 'Planned'
tags: ['documentation', 'copilot', 'cleanup', 'optimization', 'architecture']
---

# MTM Documentation Validation and GitHub Copilot Optimization

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

This implementation plan systematically validates, updates, and optimizes all 502+ documentation files in the MTM WIP Application repository to ensure optimal GitHub Copilot functionality. The plan addresses version inconsistencies, removes duplicates (reducing from 502+ to ~150 core files), and establishes automated validation processes with checkpoint validation every 5 completed files.

## 1. Requirements & Constraints

### Core Requirements
- **REQ-001**: Validate and update all 502+ documentation files for GitHub Copilot compatibility
- **REQ-002**: Ensure all technology version references match MTM_WIP_Application_Avalonia.csproj exactly
- **REQ-003**: Merge all docs/ folder content into .github/ folder structure for unified documentation system
- **REQ-004**: Relocate redundant/duplicate files to root-level quarantine folder instead of deletion
- **REQ-005**: Reduce final documentation count from 502+ to ~150 core files in .github/ folder
- **REQ-006**: Implement checkpoint validation every 5 completed files
- **REQ-007**: Maintain backward compatibility during documentation migration
- **REQ-008**: Preserve all essential content while eliminating redundancy

### Security Requirements  
- **SEC-001**: Ensure no sensitive information is exposed in documentation updates
- **SEC-002**: Validate all external links for security compliance

### Technical Constraints
- **CON-001**: Must maintain existing .github/copilot-instructions.md as master reference
- **CON-002**: All docs/ folder content must be merged into .github/ folder structure during transition
- **CON-003**: Cannot break existing cross-references during migration
- **CON-004**: .github/ folder becomes the single source of truth for all documentation
- **CON-005**: Quarantine folder must preserve all relocated files for recovery purposes

### Guidelines
- **GUD-001**: Follow awesome-copilot repository standards for all instruction files
- **GUD-002**: Use standardized file naming conventions (.instructions.md, .prompt.md)
- **GUD-003**: Include YAML frontmatter in all prompt and instruction files
- **GUD-004**: Maintain comprehensive cross-reference system

### Patterns to Follow
- **PAT-001**: MVVM Community Toolkit 8.3.2 patterns (ReactiveUI completely removed)
- **PAT-002**: Avalonia UI 11.3.4 syntax and namespace patterns
- **PAT-003**: .NET 8 with C# 12 language features
- **PAT-004**: MySQL 9.4.0 stored procedure only patterns
- **PAT-005**: Microsoft Extensions 9.0.8 dependency injection patterns

## 2. Implementation Steps

### Implementation Phase 1: Analysis, Preparation, and docs/ Folder Migration

- **GOAL-001**: Complete documentation inventory analysis, duplication identification, and migrate docs/ content to .github/

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-001 | Scan all 252 .md files and categorize by type (instructions, prompts, guides, etc.) | ✅ | 2025-09-14 |
| TASK-002 | Identify duplicate content across different locations (docs/, Documentation/, .github/) | ✅ | 2025-09-14 |
| TASK-003 | Create master deduplication mapping showing files to merge/quarantine | ✅ | 2025-09-14 |
| TASK-004 | Validate current .github/copilot-instructions.md as authoritative reference | ✅ | 2025-09-14 |
| TASK-005 | **CHECKPOINT**: Review first 5 tasks completion status | ✅ | 2025-09-14 |
| TASK-006 | Create root-level quarantine/ folder for relocated redundant files | | |
| TASK-007 | Migrate docs/README.md to .github/docs-overview.md with updated structure | | |
| TASK-008 | Move docs/architecture/ content to .github/architecture/ | | |
| TASK-009 | Move docs/development/ content to .github/development-guides/ | | |
| TASK-010 | **CHECKPOINT**: Review tasks 6-10 completion status | | |

### Implementation Phase 2: Complete docs/ Folder Migration and Setup Validation Infrastructure

- **GOAL-002**: Complete docs/ folder migration to .github/ and establish validation infrastructure

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-011 | Move docs/features/ content to .github/features/ | | |
| TASK-012 | Move docs/project-management/ content to .github/project-management/ | | |
| TASK-013 | Move docs/theme-development/ content to .github/ui-ux/ | | |
| TASK-014 | Move docs/ways-of-work/ content to .github/processes/ | | |
| TASK-015 | **CHECKPOINT**: Review tasks 11-15 completion status | | |
| TASK-016 | Extract actual technology versions from MTM_WIP_Application_Avalonia.csproj | | |
| TASK-017 | Create version consistency validation script | | |
| TASK-018 | Identify all cross-reference links requiring updates after migration | | |
| TASK-019 | Create backup archive of original documentation state | | |
| TASK-020 | **CHECKPOINT**: Review tasks 16-20 completion status | | |

### Implementation Phase 3: Core Instruction Files Validation

- **GOAL-003**: Update and validate all .github/instructions/ files for current technology stack

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-011 | Update avalonia-ui-guidelines.instructions.md version references (11.3.4) | | |
| TASK-012 | Update mvvm-community-toolkit.instructions.md patterns (8.3.2) | | |
| TASK-013 | Update mysql-database-patterns.instructions.md procedures (45+ confirmed) | | |
| TASK-014 | Update dotnet-architecture-good-practices.instructions.md (.NET 8) | | |
| TASK-015 | **CHECKPOINT**: Review tasks 11-15 completion status | | |
| TASK-016 | Update service-architecture.instructions.md DI patterns | | |
| TASK-017 | Update testing-standards.instructions.md framework versions | | |
| TASK-018 | Update cross-platform-testing-standards.instructions.md | | |
| TASK-019 | Update data-models.instructions.md manufacturing domain patterns | | |
| TASK-020 | **CHECKPOINT**: Review tasks 16-20 completion status | | |

### Implementation Phase 4: Template and Pattern Files Update

- **GOAL-004**: Ensure all template and pattern files reflect current implementation

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-031 | Update .github/copilot/templates/mtm-feature-request.md | | |
| TASK-032 | Update .github/copilot/templates/mtm-ui-component.md | | |
| TASK-033 | Update .github/copilot/templates/mtm-viewmodel-creation.md | | |
| TASK-034 | Update .github/copilot/templates/mtm-database-operation.md | | |
| TASK-035 | **CHECKPOINT**: Review tasks 31-35 completion status | | |
| TASK-036 | Update .github/copilot/patterns/mtm-mvvm-community-toolkit.md | | |
| TASK-037 | Update .github/copilot/patterns/mtm-avalonia-syntax.md | | |
| TASK-038 | Update .github/copilot/patterns/mtm-stored-procedures-only.md | | |
| TASK-039 | Update .github/copilot/context/mtm-technology-stack.md | | |
| TASK-040 | **CHECKPOINT**: Review tasks 36-40 completion status | | |

### Implementation Phase 5: Duplicate Removal and Quarantine

- **GOAL-005**: Remove duplicate files, consolidate overlapping content, and relocate redundant files to quarantine

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-041 | Relocate duplicate UI documentation from remaining docs/ui-theme-readiness/ to quarantine/ | | |
| TASK-042 | Consolidate essential view implementation guides in .github/development-guides/ | | |
| TASK-043 | Remove now-empty docs/ folder structure after successful migration | | |
| TASK-044 | Relocate redundant testing documentation to quarantine/ (keep .github/instructions/ versions) | | |
| TASK-045 | **CHECKPOINT**: Review tasks 41-45 completion status | | |
| TASK-046 | Consolidate scattered prompts from multiple locations into .github/prompts/ | | |
| TASK-047 | Relocate outdated implementation reports (Documentation/Development/) to quarantine/ | | |
| TASK-048 | Merge essential database documentation files into .github/database-documentation/ | | |
| TASK-049 | Relocate redundant README files to quarantine/ (keep one per functional area in .github/) | | |
| TASK-050 | **CHECKPOINT**: Review tasks 46-50 completion status | | |

### Implementation Phase 6: Content Accuracy Validation

- **GOAL-006**: Ensure all remaining documentation in .github/ accurately reflects current codebase

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-051 | Validate all ViewModels use MVVM Community Toolkit patterns (no ReactiveUI) | | |
| TASK-052 | Confirm all database calls use stored procedures only pattern | | |
| TASK-053 | Verify all AXAML examples use correct Avalonia 11.3.4 syntax | | |
| TASK-054 | Validate service registration patterns match ServiceCollectionExtensions.cs | | |
| TASK-055 | **CHECKPOINT**: Review tasks 51-55 completion status | | |
| TASK-056 | Update all error handling examples to use Services.ErrorHandling.HandleErrorAsync | | |
| TASK-057 | Confirm all theme examples use DynamicResource bindings | | |
| TASK-058 | Validate manufacturing domain examples use correct transaction types | | |
| TASK-059 | Update all dependency injection examples to match current patterns | | |
| TASK-060 | **CHECKPOINT**: Review tasks 56-60 completion status | | |

### Implementation Phase 7: Cross-Reference Updates and Link Validation

- **GOAL-007**: Update all cross-references and validate internal links within .github/ structure

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-061 | Update all #file: references in .github/copilot-instructions.md | | |
| TASK-062 | Fix broken internal links after docs/ → .github/ migration | | |
| TASK-063 | Update relative paths throughout .github/ folder structure | | |
| TASK-064 | Validate all cross-references in instruction files | | |
| TASK-065 | **CHECKPOINT**: Review tasks 61-65 completion status | | |
| TASK-066 | Update documentation index files with new .github/ structure | | |
| TASK-067 | Fix links in README files after consolidation | | |
| TASK-068 | Update navigation in .github/Documentation-Management/master_documentation-index.md | | |
| TASK-069 | Validate external links for accuracy | | |
| TASK-070 | **CHECKPOINT**: Review tasks 66-70 completion status | | |

### Implementation Phase 8: Automated Validation and Quality Assurance

- **GOAL-008**: Implement automated validation and create quality assurance processes for unified .github/ documentation system

| Task | Description | Completed | Date |
|------|-------------|-----------|------|
| TASK-071 | Create PowerShell script for ongoing version consistency validation | | |
| TASK-072 | Implement markdown linting for all .github/ documentation files | | |
| TASK-073 | Create link validation automation for .github/ structure | | |
| TASK-074 | Set up documentation freshness monitoring | | |
| TASK-075 | **CHECKPOINT**: Review tasks 71-75 completion status | | |
| TASK-076 | Create documentation contribution guidelines for .github/ system | | |
| TASK-077 | Implement GitHub Actions workflow for .github/ documentation validation | | |
| TASK-078 | Create documentation change impact analysis | | |
| TASK-079 | Establish documentation review process for unified .github/ system | | |
| TASK-080 | **CHECKPOINT**: Final validation of all 80 tasks | | |

## 3. Alternatives

Alternative approaches considered for documentation optimization:

- **ALT-001**: **Complete Documentation Rewrite**: Considered starting from scratch but rejected due to loss of valuable existing content and institutional knowledge
- **ALT-002**: **Manual Review Only**: Considered manual review without automated validation but rejected due to scale (502+ files) and risk of human error
- **ALT-003**: **Keep All Files**: Considered keeping all existing files and only updating content but rejected in favor of relocating redundant files to a quarantine folder to establish .github/ as the single documentation system while preserving all content for recovery
- **ALT-004**: **External Documentation System**: Considered moving to external documentation platform (GitBook, Confluence) but rejected to maintain GitHub integration and Copilot accessibility
- **ALT-005**: **Gradual Migration**: Considered gradual file-by-file migration but rejected due to prolonged inconsistency and complexity of managing two systems

## 4. Dependencies

## 4. Dependencies

Critical dependencies for successful documentation optimization:

- **DEP-001**: **MTM_WIP_Application_Avalonia.csproj** - Source of truth for all technology version numbers
- **DEP-002**: **Active codebase analysis** - Requires scanning ViewModels, Services, and Views for current patterns
- **DEP-003**: **GitHub Copilot compatibility** - Must validate against awesome-copilot repository standards
- **DEP-004**: **Existing .github/copilot-instructions.md** - Master reference file that must be preserved and enhanced
- **DEP-005**: **PowerShell execution environment** - Required for automated validation scripts
- **DEP-006**: **Markdown processing tools** - Required for link validation and content analysis
- **DEP-007**: **Git repository access** - Required for file operations and history preservation
- **DEP-008**: **VS Code with GitHub Copilot** - Required for testing updated instruction effectiveness

## 5. Files

## 5. Files

### Files to be Updated (Core Documentation - 150 target files)

#### Critical GitHub Copilot Integration Files
- **FILE-001**: `.github/copilot-instructions.md` - Master instruction file (preserve and enhance)
- **FILE-002**: `.github/instructions/README.md` - Instructions directory index
- **FILE-003**: `.github/instructions/avalonia-ui-guidelines.instructions.md` - Avalonia 11.3.4 patterns
- **FILE-004**: `.github/instructions/mvvm-community-toolkit.instructions.md` - MVVM Community Toolkit 8.3.2
- **FILE-005**: `.github/instructions/mysql-database-patterns.instructions.md` - 45+ stored procedures
- **FILE-006**: `.github/instructions/dotnet-architecture-good-practices.instructions.md` - .NET 8 patterns
- **FILE-007**: `.github/instructions/service-architecture.instructions.md` - DI patterns
- **FILE-008**: `.github/instructions/testing-standards.instructions.md` - Testing frameworks
- **FILE-009**: `.github/instructions/cross-platform-testing-standards.instructions.md` - Platform support
- **FILE-010**: `.github/instructions/data-models.instructions.md` - Manufacturing domain

#### Template Files (10 files)
- **FILE-011**: `.github/copilot/templates/mtm-feature-request.md`
- **FILE-012**: `.github/copilot/templates/mtm-ui-component.md`
- **FILE-013**: `.github/copilot/templates/mtm-viewmodel-creation.md`
- **FILE-014**: `.github/copilot/templates/mtm-database-operation.md`
- **FILE-015**: `.github/copilot/templates/mtm-service-implementation.md`
- **FILE-016**: `.github/copilot/templates/implementation-audit-template.md`
- **FILE-017**: `.github/copilot/templates/implementation-audit-workflow.md`
- **FILE-018**: `.github/copilot/templates/pull_request_template.md`
- **FILE-019**: `.github/PULL_REQUEST_TEMPLATE/feature_implementation.md`
- **FILE-020**: `.github/PULL_REQUEST_TEMPLATE/documentation.md`

#### Pattern Files (5 files)
- **FILE-021**: `.github/copilot/patterns/mtm-mvvm-community-toolkit.md`
- **FILE-022**: `.github/copilot/patterns/mtm-avalonia-syntax.md`
- **FILE-023**: `.github/copilot/patterns/mtm-stored-procedures-only.md`
- **FILE-024**: `.github/copilot/context/mtm-technology-stack.md`
- **FILE-025**: `.github/copilot/context/mtm-architecture-patterns.md`

#### Prompt Files (25 files)
- **FILE-026**: `.github/prompts/update-implementation-plan.prompt.md`
- **FILE-027**: `.github/prompts/create-github-issues-for-unmet-specification-requirements.prompt.md`
- **FILE-028**: `.github/prompts/breakdown-feature-implementation.prompt.md`
- **FILE-029**: `.github/prompts/documentation-writer.prompt.md`
- **FILE-030**: `.github/prompts/mtm-audit-system.prompt.md`
- **FILE-031**: `.github/copilot/prompts/create-unit-test.prompt.md`
- **FILE-032**: `.github/copilot/prompts/create-integration-test.prompt.md`
- **FILE-033**: `.github/copilot/prompts/create-ui-test.prompt.md`
- **FILE-034**: `.github/copilot/prompts/create-database-test.prompt.md`
- **FILE-035**: `.github/copilot/prompts/create-performance-test.prompt.md`
- **FILE-036**: `.github/copilot/prompts/create-cross-platform-test.prompt.md`
- **FILE-037**: `.github/prompts/csharp-nunit.prompt.md`
- **FILE-038**: `.github/prompts/csharp-xunit.prompt.md`
- **FILE-039**: `.github/prompts/csharp-mstest.prompt.md`
- **FILE-040**: `.github/prompts/csharp-tunit.prompt.md`
- **FILE-041**: `.github/prompts/ef-core.prompt.md`
- **FILE-042**: `.github/prompts/editorconfig.prompt.md`
- **FILE-043**: `.github/prompts/first-ask.prompt.md`
- **FILE-044**: `.github/prompts/generate-github-issues-from-audit.prompt.md`
- **FILE-045**: `.github/prompts/generate-html-questionnaire.prompt.md`
- **FILE-046**: `.github/prompts/prompt-builder.prompt.md`
- **FILE-047**: `.github/prompts/review-and-refactor.prompt.md`
- **FILE-048**: `.github/prompts/removetabview-implementation-audit.prompt.md`
- **FILE-049**: `.github/prompts/themeeditorview-implementation-audit.prompt.md`
- **FILE-050**: `.github/prompts/transfertabview-implementation-audit.prompt.md`

#### Core Documentation Files (.github/ Structure - 25 files)
- **FILE-051**: `.github/docs-overview.md` - Main documentation index (migrated from docs/README.md)
- **FILE-052**: `.github/architecture/project-blueprint.md` - Architecture overview (migrated from docs/)
- **FILE-053**: `.github/architecture/folder-structure.md` - Project structure (migrated from docs/)
- **FILE-054**: `.github/development-guides/Documentation-Standards.md` - Documentation standards (migrated from docs/)
- **FILE-055**: `.github/Documentation-Management/master_documentation-index.md` - Complete documentation inventory
- **FILE-056**: `.github/database-documentation/MTM-Database-Procedures-Reference.md` - Database procedures catalog
- **FILE-057**: `.github/database-documentation/MTM-Database-ERD.md` - Database entity relationship diagram
- **FILE-058**: `.github/development-guides/MTM-MVVM-Patterns-Guide.md` - MVVM implementation patterns
- **FILE-059**: `.github/development-guides/MTM-Testing-Strategy.md` - Comprehensive testing approach
- **FILE-060**: `.github/qa-framework/MTM-Comprehensive-Testing-Strategy.md` - QA testing strategy
- **FILE-061**: `.github/ui-ux/MTM-Design-System-Documentation.md` - UI/UX design system (migrated from docs/)
- **FILE-062**: `.github/ui-ux/MTM-Avalonia-Style-Guide.md` - Avalonia-specific UI patterns (migrated from docs/)
- **FILE-063**: `.github/project-management/code-review-guidelines.md` - Code review standards (migrated from docs/)
- **FILE-064**: `.github/features/feature-implementation-guide.md` - Feature development guide (migrated from docs/)
- **FILE-065**: `.github/processes/ways-of-work.md` - Development processes (migrated from docs/)
- **FILE-066**: `.github/audit/README.md` - Audit framework overview
- **FILE-067**: `.github/scripts/README.md` - Automation scripts documentation
- **FILE-068**: `Documentation/FileLogging-README.md` - File logging implementation (to be relocated)
- **FILE-069**: `scripts/README.md` - Build and utility scripts (to be relocated)
- **FILE-070**: `LICENSE.txt` - Project license (remains in root)

#### Files to be Relocated to Quarantine Folder
- **FILE-071**: `quarantine/docs-archive/` - Complete docs/ folder contents after migration
- **FILE-072**: `quarantine/redundant-documentation/` - Duplicate files from Documentation/ folder
- **FILE-073**: `quarantine/obsolete-guides/` - Outdated implementation guides and reports
- **FILE-074**: `quarantine/legacy-ui-docs/` - Obsolete UI theme readiness checklists
- **FILE-075**: `quarantine/duplicate-scripts/` - Redundant script documentation

### Files to be Relocated to Quarantine Folder (352+ redundant files)

#### Migrated docs/ Folder Content (entire folder structure)
- Relocate complete `docs/` folder to `quarantine/docs-archive/` after successful migration to `.github/`
- Preserve folder structure for reference and potential recovery

#### Redundant Theme Documentation (32+ files → quarantined)
- Relocate individual view theme checklists from remaining locations to `quarantine/legacy-ui-docs/`
- Keep 3-5 consolidated theme guides in `.github/ui-ux/`

#### Duplicate Implementation Guides (15+ files → quarantined)
- Relocate duplicate guides to `quarantine/redundant-documentation/implementation-guides/`
- Keep consolidated versions in `.github/development-guides/`

#### Outdated Documentation Reports (50+ files → quarantined)
- Relocate completed implementation reports to `quarantine/obsolete-guides/`
- Archive historical information while cleaning active documentation

#### Duplicate Database Files (25+ files → quarantined)
- Relocate redundant database documentation to `quarantine/redundant-documentation/database/`
- Keep authoritative versions in `.github/database-documentation/`

#### Miscellaneous Duplicates (200+ files → quarantined)
- Relocate various duplicate markdown files to appropriate `quarantine/` subdirectories
- Organize by original location and content type for future reference

## 6. Testing

Comprehensive testing strategy to validate documentation optimization:

- **TEST-001**: **Version Consistency Test** - Automated validation that all technology version references match MTM_WIP_Application_Avalonia.csproj
- **TEST-002**: **GitHub Copilot Integration Test** - Manual validation using VS Code with GitHub Copilot to test instruction effectiveness
- **TEST-003**: **Link Validation Test** - Automated testing of all internal and external links in documentation
- **TEST-004**: **Cross-Reference Validation Test** - Verify all #file: references and internal links work correctly
- **TEST-005**: **Markdown Linting Test** - Automated validation of markdown syntax and formatting consistency
- **TEST-006**: **Content Accuracy Test** - Manual validation that code examples reflect current codebase patterns
- **TEST-007**: **Duplicate Detection Test** - Automated scanning for remaining duplicate content after consolidation
- **TEST-008**: **Awesome-Copilot Compliance Test** - Validation against official awesome-copilot repository standards
- **TEST-009**: **Search and Discoverability Test** - Verify documentation is findable within 2 clicks from main entry points
- **TEST-010**: **Checkpoint Validation Test** - Systematic review every 5 completed tasks as specified in requirements
- **TEST-011**: **File Count Validation Test** - Confirm reduction from 502+ files to target ~150 core files in .github/ folder
- **TEST-012**: **Archive Integrity Test** - Verify backup archive and quarantine folder contain all original content
- **TEST-013**: **docs/ Migration Completeness Test** - Validate all docs/ content successfully migrated to .github/ structure
- **TEST-014**: **Quarantine Organization Test** - Verify quarantine folder properly organized by content type and origin
- **TEST-015**: **.github/ Structure Validation Test** - Confirm .github/ folder contains all essential documentation
- **TEST-013**: **Technology Stack Validation Test** - Confirm all patterns match current implementation (.NET 8, Avalonia 11.3.4, MVVM Community Toolkit 8.3.2, MySQL 9.4.0)
- **TEST-014**: **Manufacturing Domain Accuracy Test** - Validate business domain examples accurately reflect MTM manufacturing processes
- **TEST-015**: **Automation Script Test** - Validate all PowerShell and automation scripts function correctly

## 7. Risks & Assumptions

### Risks
- **RISK-001**: **Content Loss During Consolidation** - Risk of losing valuable information when merging duplicate files
  - *Mitigation*: Create comprehensive backup archive before any changes
- **RISK-002**: **Broken Cross-References** - Risk of breaking internal links during file consolidation
  - *Mitigation*: Implement automated link validation and systematic cross-reference updates
- **RISK-003**: **GitHub Copilot Effectiveness Regression** - Risk that documentation changes reduce Copilot effectiveness
  - *Mitigation*: Test with actual Copilot sessions after each phase completion
- **RISK-004**: **Version Mismatch Introduction** - Risk of introducing new version inconsistencies during updates
  - *Mitigation*: Use automated validation scripts and single source of truth (project file)
- **RISK-005**: **Development Team Disruption** - Risk of disrupting team workflow during docs/ → .github/ migration
  - *Mitigation*: Execute migration in phases, communicate changes clearly, maintain quarantine for recovery
- **RISK-006**: **Scale Complexity** - Risk of project becoming unmanageable due to 502+ file scope and docs/ migration
  - *Mitigation*: Implement checkpoint validation every 5 files, break into atomic phases, use quarantine for safety
- **RISK-007**: **Quarantine Folder Bloat** - Risk of quarantine folder becoming unmanageable with 352+ relocated files
  - *Mitigation*: Organize quarantine by content type and origin, implement regular quarantine cleanup schedule

### Assumptions
- **ASSUMPTION-001**: **Current .github/copilot-instructions.md is Authoritative** - Assumes existing master instruction file is accurate and should be preserved
- **ASSUMPTION-002**: **Technology Stack Stability** - Assumes current technology versions (.NET 8, Avalonia 11.3.4, etc.) will remain stable during documentation update
- **ASSUMPTION-003**: **Development Team Availability** - Assumes team members available for validation and testing of updated documentation
- **ASSUMPTION-004**: **Codebase Stability** - Assumes core application patterns will not change significantly during documentation update period
- **ASSUMPTION-005**: **PowerShell Environment Available** - Assumes Windows PowerShell environment available for automation scripts
- **ASSUMPTION-006**: **GitHub Copilot Access** - Assumes team has active GitHub Copilot licenses for testing updated instructions
- **ASSUMPTION-007**: **Awesome-Copilot Standards Stability** - Assumes awesome-copilot repository standards will not change significantly during implementation

## 8. Related Specifications / Further Reading

### Internal Documentation
- [MTM WIP Application Architecture Blueprint](../architecture/project-blueprint.md) - Comprehensive architecture overview (migrated from docs/)
- [GitHub Copilot Instructions Master File](../copilot-instructions.md) - Primary reference for all Copilot patterns
- [Documentation Standards](../development-guides/Documentation-Standards.md) - Writing and formatting guidelines (migrated from docs/)
- [Master Documentation Index](../Documentation-Management/master_documentation-index.md) - Complete documentation inventory

### External Standards and References  
- [Awesome Copilot Repository](https://github.com/github/awesome-copilot) - Official GitHub Copilot best practices and standards
- [Avalonia UI 11.3.4 Documentation](https://docs.avaloniaui.net/) - Official Avalonia framework documentation
- [MVVM Community Toolkit 8.3.2 Documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) - Official Microsoft MVVM patterns
- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8) - Official .NET 8 features and patterns
- [MySQL 9.4.0 Documentation](https://dev.mysql.com/doc/refman/9.4/en/) - Official MySQL database documentation

### Implementation Context Files
- [MTM Technology Stack Context](../copilot/context/mtm-technology-stack.md) - Complete technology stack specification
- [MTM Business Domain Context](../copilot/context/mtm-business-domain.md) - Manufacturing domain knowledge
- [Database Procedures Reference](../database-documentation/MTM-Database-Procedures-Reference.md) - Complete stored procedure catalog

### Quality Assurance References
- [Testing Strategy Documentation](../development-guides/MTM-Testing-Strategy.md) - Comprehensive testing approach
- [Code Review Guidelines](../project-management/code-review-guidelines.md) - Review process and standards (migrated from docs/)
- [Cross-Platform Testing Standards](../instructions/cross-platform-testing-standards.instructions.md) - Multi-platform validation requirements

---

**Implementation Plan Status**: ✅ **READY FOR EXECUTION**  
**Target Completion**: 8 phases, 80 tasks with checkpoint validation every 5 tasks  
**Expected Outcome**: 502+ files reduced to ~150 optimized files in unified .github/ documentation system, with redundant files preserved in organized quarantine structure
```
