<!--
Sync Impact Report:
Version change: 1.0.0 → 2.0.0
Modified principles: 
- Enhanced Performance Requirements (IV) → Added GSC execution timing requirements
- Enhanced Testing Standards (II) → Added GSC workflow validation tests
Added sections: 
- GitHub Spec Commands (GSC) Workflow Standards (V)
- Spec-Kit Compatibility Standards (VI)
- GSC Memory Integration Standards (5.5)
- Cross-Platform GSC Execution (5.6)
- Spec-Kit Integration Compliance (5.7)
Removed sections: N/A
Templates requiring updates:
✅ Updated - plan-template.md (GSC workflow constitution checks)
✅ Updated - spec-template.md (GSC compatibility requirements)
✅ Updated - tasks-template.md (GSC memory integration validation)
Follow-up TODOs: Create GSC validation scripts, implement memory file integration tests
-->

# MTM WIP Application Constitution

## Core Principles

### I. Code Quality Excellence (NON-NEGOTIABLE)

ALL code MUST adhere to enterprise manufacturing standards: .NET 8.0 with nullable reference types enabled, MVVM Community Toolkit patterns exclusively (NO ReactiveUI), dependency injection with Microsoft.Extensions, structured logging, comprehensive error handling via centralized Services.ErrorHandling.HandleErrorAsync(), and strict adherence to established naming conventions (PascalCase classes/methods/properties, camelCase with underscore prefix for fields).

**Rationale**: Manufacturing environments require zero-tolerance reliability. Code quality directly impacts operator productivity and system availability.

### II. Comprehensive Testing Standards (NON-NEGOTIABLE)

EVERY feature MUST implement 5-tier testing: Unit tests with MVVM Community Toolkit patterns, Integration tests for service interactions, UI automation tests for all workflows, Cross-platform feature tests on Windows/macOS/Linux/Android platforms, End-to-end tests covering complete manufacturing operator workflows. Test-driven development mandatory: Tests written → Tests fail → Implementation → Tests pass. GSC workflow validation tests required, Memory file integration tests mandatory, Cross-platform GSC execution tests on Windows/macOS/Linux, Spec-kit compatibility validation tests.

**Rationale**: Cross-platform manufacturing systems require validated functionality on every supported platform to ensure consistent operator experience. GSC workflows ensure predictable development processes with memory-driven quality gates.

### III. User Experience Consistency

ALL user interfaces MUST maintain consistent behavior across platforms: Avalonia UI 11.3.4 with semantic theming system, responsive layouts supporting 1024x768 to 4K resolutions, consistent navigation patterns, Material Design iconography, theme switching without application restart, accessibility compliance, and manufacturing operator-optimized workflows with minimal clicks for frequent operations.

**Rationale**: Manufacturing operators work in diverse environments and require predictable, efficient interfaces to maintain productivity across shifts and locations.

### IV. Performance Requirements

SYSTEM performance MUST meet manufacturing operational demands: Database operations complete within 30 seconds timeout, UI responsiveness maintained during concurrent operations, memory usage optimized for long-running sessions (8+ hours), MySQL connection pooling (5-100 connections), file logging with rotation (50MB max, 30-day retention), and startup time under 10 seconds on minimum hardware specifications. GSC command execution must complete within 30 seconds, Memory file reading and integration must not exceed 5 seconds per command, State persistence operations must complete within 2 seconds, Cross-platform GSC validation must maintain consistent performance.

**Rationale**: Manufacturing operations run continuously with minimal downtime tolerance. Performance degradation directly impacts production efficiency and operator satisfaction. GSC workflows require responsive execution to maintain development velocity.

### V. GitHub Spec Commands (GSC) Workflow Standards (NON-NEGOTIABLE)

ALL development MUST follow the structured GSC workflow with memory integration: Constitution validation before any changes, Feature specification with complete requirements gathering and memory file consultation, Clarification of all ambiguities using debugging memory patterns, Implementation planning with container layout and styling memory, Task execution with Avalonia UI memory patterns, Analysis using systematic debugging workflows, Implementation with memory-driven custom control patterns. Each GSC command MUST validate the previous step's completion and integrate relevant memory files from the user's prompt directory.

**Memory Integration Requirements**: Each GSC command MUST read and apply relevant memory files from `C:\Users\johnk\AppData\Roaming\Code\User\prompts` during execution: `/constitution` reads all memory files for foundational patterns, `/specify` integrates avalonia-ui-memory.instructions.md for UI specifications, `/clarify` uses debugging-memory.instructions.md for systematic clarification, `/plan` applies memory.instructions.md for universal development patterns, `/task` incorporates avalonia-custom-controls-memory.instructions.md for control development, `/analyze` uses debugging-memory.instructions.md for systematic analysis, `/implement` integrates all applicable memory patterns during implementation.

**Rationale**: Manufacturing systems require predictable, validated development workflows enhanced by accumulated development knowledge. Memory-driven development reduces repetitive mistakes and accelerates quality implementation.

### VI. Spec-Kit Compatibility Standards (NON-NEGOTIABLE)

ALL GSC commands MUST be compatible with @github/spec-kit slash command execution via Git Bash. Commands must support both PowerShell (.ps1) and shell script (.sh) execution environments. Cross-platform compatibility required for Windows (Git Bash), macOS (Bash), and Linux (Bash) environments. State persistence must work across different command execution contexts.

**Rationale**: Cross-platform development teams require consistent tooling and workflow execution regardless of operating system or shell environment.

## Quality Assurance Standards

### Code Review Requirements

ALL changes MUST pass comprehensive review gates: Automated build validation on Windows/macOS/Linux, Unit test coverage maintained above 80%, Integration tests passing for affected services, UI automation tests validating affected workflows, Cross-platform compatibility verified, Security analysis completed for data handling changes, Performance impact assessment for database/UI modifications.

### Manufacturing Domain Validation

ALL features MUST align with MTM business requirements: Valid operations (90/100/110/120), Location codes compliance (FLOOR/RECEIVING/SHIPPING/etc.), Transaction type validation (IN/OUT/TRANSFER), Part ID format consistency, Quantity validation (integer only), Session timeout enforcement (60 minutes), Quick buttons limitation (maximum 10 per user).

## Performance Standards

### System Performance Gates

PERFORMANCE benchmarks MUST be maintained: Database query execution under 5 seconds (warning) / 30 seconds (timeout), UI thread responsiveness maintained during background operations, Memory usage growth rate under 50MB/hour during 8-hour sessions, Cross-platform rendering consistency within 5% variance, Startup time optimization for manufacturing environment hardware.

### Monitoring and Observability

PRODUCTION systems MUST provide operational insight: Structured logging with correlation IDs, Performance metrics collection, Error rate monitoring with alerting, Database connection pool health monitoring, Cross-platform deployment validation, Manufacturing operator workflow analytics.

### GSC Memory Integration Standards

Memory files must be read during appropriate GSC command execution, Pattern recognition from memory files must influence decision making, Accumulated development knowledge must be applied to current context, Memory file updates must be recorded when new patterns are discovered.

### Cross-Platform GSC Execution

All GSC commands must work in Git Bash environment, PowerShell and Bash script compatibility required, State management must persist across different execution environments, JSON-based state files for cross-platform compatibility.

### Spec-Kit Integration Compliance

GSC commands must integrate with @github/spec-kit framework, Slash command execution via Git Bash must be supported, Command discovery and help integration required, Workflow orchestration compatibility with spec-kit patterns.

## Governance

### Constitutional Authority

This constitution supersedes all other development practices and guidelines. ALL development activities MUST comply with these principles. The comprehensive .github/instructions/ library provides implementation guidance while this constitution defines non-negotiable requirements.

### Amendment Process

Constitutional amendments require: Documented business justification, Impact assessment on existing codebase, Migration plan for affected components, Approval from project stakeholders, Update to all dependent templates and instruction files.

### Compliance Verification

ALL pull requests MUST verify constitutional compliance: Automated constitution checking in CI/CD pipeline, Code review checklist alignment with principles, Cross-platform testing validation, Performance benchmark verification, Manufacturing domain rule validation.

### Guidance Integration

Runtime development guidance available in `.github/copilot-instructions.md` provides detailed implementation patterns while maintaining constitutional compliance. The 34+ instruction files in `.github/instructions/` offer specialized guidance for all MTM development scenarios.

**Version**: 2.0.0 | **Ratified**: 2025-09-27 | **Last Amended**: 2025-09-28
