# Research Report: Copilot Chat Integration for GSC Workflow Control

**Feature**: Copilot Chat Integration for GSC Workflow Control  
**Date**: September 29, 2025

## Decision Log

### 1. Copilot Chat Command Registration

- **Decision**: Use the @github/copilot-extensions SDK to register twelve slash commands with explicit parameter metadata and autocomplete descriptions sourced from `contracts/copilot-chat-commands.json`.
- **Rationale**: The SDK provides first-class support for slash commands in both VS Code and Visual Studio, ensuring parity with future Copilot-hosted experiences.
- **Alternatives Considered**:
  - *Manual chat prompt parsing*: Rejected due to lack of structured validation and poor discoverability.
  - *Hybrid command palette integration*: Rejected because it would not surface commands consistently inside Copilot Chat.

### 2. PowerShell Bridge Invocation Strategy

- **Decision**: Invoke `.specify/scripts/gsc/*.ps1` through their shell wrappers using `--json` mode, normalize responses, and log stderr to `Services.ErrorHandling`.
- **Rationale**: Maintains a single execution path for CLI, Copilot Chat, and future automation, protecting against drift between tooling.
- **Alternatives Considered**:
  - *Direct PowerShell host embedding*: Added complexity for limited benefit and complicates cross-platform parity.
  - *Re-implementing commands in TypeScript*: High risk of behavior divergence and increased maintenance burden.

### 3. Workflow State Synchronization

- **Decision**: Read and write workflow state exclusively through the existing JSON files (`gsc-workflow.json`, `validation-status.json`, `memory-integration.json`) using the PowerShell state manager module, with change detection to highlight external updates in chat responses.
- **Rationale**: Reuses hardened logic, preserves atomic updates, and guarantees that Copilot Chat reflects terminal-triggered changes.
- **Alternatives Considered**:
  - *Custom TypeScript state cache*: Rejected because it risks inconsistencies and bypasses lock enforcement.
  - *Database-backed session store*: Unnecessary overhead for single-user scope and violates "stored procedures only" principle.

### 4. Performance and Availability Targets

- **Decision**: Enforce ≤12 second chat response SLA, ≤5 second memory integration, ≤2 second state persistence, and ≥99.5% availability by wiring telemetry into chat outputs and reusing performance-monitor scripts.
- **Rationale**: Aligns with clarifications and constitution requirements, keeping manufacturing workflows responsive.
- **Alternatives Considered**:
  - *Best-effort responses without metrics*: Rejected; would make compliance unverifiable.
  - *Aggressive batching*: Unnecessary given single-user workload and could hide slowdowns.

### 5. Concurrency Handling Model

- **Decision**: Maintain single-user semantics by allowing the first state-changing command to execute and returning a descriptive lock message for any overlapping request; no automatic queueing or override prompts.
- **Rationale**: Meets clarified expectation, keeps implementation lightweight, and avoids accidental double-execution scenarios.
- **Alternatives Considered**:
  - *Automatic queuing*: Adds complexity and risks surprise actions if the user has moved on.
  - *Override prompts*: Unneeded for current single-user workflow; revisit if multi-user demand emerges.

## Open Questions

None. All critical ambiguities addressed during the clarification session.
