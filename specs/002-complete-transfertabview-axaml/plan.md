
# Implementation Plan: Complete TransferTabView.axaml Implementation

**Branch**: `002-complete-transfertabview-axaml` | **Date**: September 28, 2025 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-complete-transfertabview-axaml/spec.md`

## Execution Flow (/plan command scope)

```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:

- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary

Replace TransferCustomDataGrid with standard Avalonia DataGrid in TransferTabView.axaml, implementing column customization dropdown with MySQL user preferences persistence, integrating EditInventoryView seamlessly, and applying MTM Theme V2 styling patterns following RemoveTabView.axaml reference implementation.

## Technical Context

**Language/Version**: C# 12 with .NET 8.0, nullable reference types enabled  
**Primary Dependencies**: Avalonia UI 11.3.4, MVVM Community Toolkit 8.3.2, MySQL 9.4.0, Microsoft.Extensions 9.0.8  
**Storage**: MySQL database with usr_ui_settings table, JSON column storage for user preferences  
**Testing**: Unit tests with MVVM Community Toolkit patterns, Integration tests for MySQL operations, UI automation tests for Avalonia  
**Target Platform**: Cross-platform (Windows, macOS, Linux, Android) via Avalonia UI
**Project Type**: Single desktop application with manufacturing domain focus  
**Performance Goals**: Database operations <30s timeout, UI responsiveness during concurrent operations, startup <10s  
**Constraints**: Manufacturing operator workflows (minimal clicks), 8+ hour session memory optimization, AVLN2000 error prevention  
**Scale/Scope**: Manufacturing facility scale, 45+ stored procedures integration, Theme V2 semantic token system

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Code Quality Excellence Gate

- [x] .NET 8.0 with nullable reference types enabled (existing MTM architecture)
- [x] MVVM Community Toolkit patterns (NO ReactiveUI) - TransferTabView follows established patterns
- [x] Microsoft.Extensions dependency injection (established MTM pattern)
- [x] Centralized error handling via Services.ErrorHandling.HandleErrorAsync() (MTM standard)
- [x] Naming conventions: PascalCase classes/methods/properties, camelCase with underscore prefix for fields

### Testing Standards Gate

- [x] Unit tests with MVVM Community Toolkit patterns planned (TransferTabViewModel testing)
- [x] Integration tests for service interactions planned (MySQL usr_ui_settings integration)
- [x] UI automation tests for workflows planned (DataGrid, EditInventoryView integration)
- [x] Cross-platform tests (Windows/macOS/Linux/Android) planned (Avalonia compatibility)
- [x] End-to-end manufacturing operator workflow tests planned (transfer operations)

### User Experience Consistency Gate

- [x] Avalonia UI 11.3.4 with semantic theming system (MTM Theme V2)
- [x] Responsive layouts (1024x768 to 4K) planned (ScrollViewer with MinWidth/MinHeight)
- [x] Material Design iconography usage (materialIcons namespace)
- [x] Manufacturing operator-optimized workflows (minimal clicks) - quantity auto-capping, double-click edit

### Performance Requirements Gate

- [x] Database operations under 30 seconds timeout (MySQL connection configuration)
- [x] UI responsiveness during concurrent operations (async/await patterns)
- [x] Memory optimization for 8+ hour sessions (proper disposal patterns)
- [x] MySQL connection pooling (5-100 connections) - existing infrastructure
- [x] Startup time under 10 seconds target (no additional startup overhead)

## Project Structure

### Documentation (this feature)

```
specs/[###-feature]/
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output (/plan command)
├── data-model.md        # Phase 1 output (/plan command)
├── quickstart.md        # Phase 1 output (/plan command)
├── contracts/           # Phase 1 output (/plan command)
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)

```
Views/MainForm/Panels/
├── TransferTabView.axaml        # Primary implementation target
├── TransferTabView.axaml.cs     # Minimal code-behind
└── RemoveTabView.axaml          # Reference implementation pattern

ViewModels/MainForm/
├── TransferItemViewModel.cs     # MVVM Community Toolkit ViewModel
└── RemoveItemViewModel.cs       # Reference ViewModel pattern

Views/Overlay/
├── EditInventoryView.axaml      # Integration target
└── EditInventoryView.axaml.cs   # Existing overlay view

Services/
├── Configuration.cs             # User settings persistence
├── Database.cs                  # MySQL operations
└── ErrorHandling.cs             # Centralized error handling

Models/
├── EditInventoryModel.cs        # Existing data models
└── ServiceResult.cs             # Standard result patterns

Resources/ThemesV2/
├── *.axaml                      # Theme V2 semantic tokens
└── StyleSystem.axaml            # Component styling system

tests/
├── ViewModels/                  # Unit tests for ViewModels
├── Services/                    # Integration tests for Services
└── UI/                          # UI automation tests
```

**Structure Decision**: Single Avalonia desktop application following established MTM architecture patterns with Views/ViewModels/Services separation and comprehensive testing structure.

## Phase 0: Outline & Research

1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task

2. **Generate and dispatch research agents**:

   ```
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```

3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts

*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable

2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`

3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)

4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps

5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType copilot`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach

*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:

- Load `.specify/templates/tasks-template.md` as base
- Generate tasks from Phase 1 design docs (contracts, data-model.md, quickstart.md)
- Database tasks: Create stored procedures for usr_ui_settings and transfer operations
- Service implementation tasks: ITransferService and IColumnConfigurationService [P]
- ViewModel update tasks: TransferItemViewModel with MVVM Community Toolkit patterns
- AXAML refactoring tasks: Replace TransferCustomDataGrid with standard DataGrid
- Integration tasks: EditInventoryView seamless integration with proper triggers
- Testing tasks: Unit tests for ViewModels, Integration tests for Services, UI automation tests

**Ordering Strategy**:

- TDD order: Tests before implementation (contract tests → failing tests → implementation)
- Dependency order: Database → Services → ViewModels → Views → Integration
- Mark [P] for parallel execution: Service implementations, test creation, AXAML styling
- Critical path: Database setup → Service contracts → ViewModel updates → AXAML refactoring

**Estimated Output**: 28-32 numbered, ordered tasks focusing on:

1. Database setup and stored procedures (4-5 tasks)
2. Service contract implementation (6-8 tasks)
3. ViewModel MVVM Community Toolkit updates (5-6 tasks)
4. AXAML refactoring and Theme V2 compliance (8-10 tasks)
5. Integration and testing validation (5-7 tasks)

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation

*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)  
**Phase 4**: Implementation (execute tasks.md following constitutional principles)  
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking

*Fill ONLY if Constitution Check has violations that must be justified*

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |

## Progress Tracking

*This checklist is updated during execution flow*

**Phase Status**:

- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:

- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS (all design artifacts follow MTM constitution)
- [x] All NEEDS CLARIFICATION resolved (5 clarifications documented)
- [x] Complexity deviations documented (none required)

---
*Based on Constitution v2.1.1 - See `/memory/constitution.md`*
