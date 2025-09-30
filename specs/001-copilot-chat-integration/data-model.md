# Data Model: Copilot Chat Integration for GSC Workflow Control

**Feature**: Copilot Chat Integration for GSC Workflow Control  
**Date**: September 29, 2025

## Core Entities

### ChatCommandSession

- **Purpose**: Represents a single slash command invocation initiated inside Copilot Chat.
- **Key Attributes**:
  - `CommandName` (string): One of the supported GSC commands.
  - `Arguments` (array of string): Parameters passed from the chat UI.
  - `ExecutionMode` (string): `"chat"`, `"cli"`, or `"automation"`.
  - `RequestedFormat` (string): `"markdown"`, `"json"`, or `"plain"`.
  - `StartTimestamp` / `EndTimestamp` (DateTime): Execution window for SLA monitoring.
  - `ResultEnvelope` (object): Structured response forwarded to chat.
- **Relationships**: Produces a `PerformanceTelemetry` record; associates with a single `WorkflowStateSnapshot` upon completion.
- **Validation Rules**: Command name must match contract; requested format must be supported by contract; execution duration must remain ≤12 seconds under normal load.

### WorkflowStateSnapshot

- **Purpose**: Captures workflow position and compliance signals shown to the developer after a command runs.
- **Key Attributes**:
  - `WorkflowId` (string/GUID): Current workflow session identifier.
  - `CurrentPhase` (enum): Constitution, Specify, Clarify, Plan, Task, Analyze, Implement.
  - `LockStatus` (object): `IsLocked` (bool), `Owner` (string), `ExpiresAt` (DateTime).
  - `ValidationGates` (array): Items describing quality gates and their pass/fail state.
  - `ExternalUpdateDetected` (bool): Indicates state changed outside chat before response was rendered.
- **Relationships**: Sourced from JSON state files managed by PowerShell; referenced by `ChatCommandSession` responses.
- **Validation Rules**: Snapshot must mirror persisted state exactly; lock information required whenever `IsLocked` is true.

### MemoryInsightSummary

- **Purpose**: Summarizes the memory patterns applied to satisfy FR-004.
- **Key Attributes**:
  - `MemoryFiles` (array): File paths consulted during execution.
  - `PatternsApplied` (array of string): Short descriptors of the guidance surfaced.
  - `ChecksumStatus` (enum): `Valid`, `Fallback`, or `Error`.
  - `Recommendations` (array): Targeted follow-ups exposed to the developer.
- **Relationships**: Bound to a `ChatCommandSession`; draws data from `memory-integration.json`.
- **Validation Rules**: At least one memory file entry per command; checksum status must be reported; fallback requires clear remediation guidance.

### LockNotice

- **Purpose**: Communicates single-user locking behavior for overlapping commands.
- **Key Attributes**:
  - `LockOwner` (string): User who holds the current lock.
  - `LockExpiresAt` (DateTime): Scheduled expiration.
  - `CommandRejected` (string): Name of the command denied due to lock.
  - `SuggestedNextAction` (string): Guidance (`"wait"`, `"retry"`, `"use-force"`).
- **Relationships**: Returned when `ChatCommandSession` detects conflicting state changes; sourced from PowerShell lock manager.
- **Validation Rules**: Must be emitted whenever a conflicting command occurs; suggested action defaults to "wait" for single-user operation.

### PerformanceTelemetry

- **Purpose**: Tracks response-time and availability metrics surfaced in chat.
- **Key Attributes**:
  - `ExecutionTimeMs` (integer): Total duration from invocation to completion.
  - `MemoryLoadMs` (integer): Time spent integrating memory files.
  - `StatePersistMs` (integer): Time spent reading/writing state.
  - `SlaBreached` (bool): True if execution exceeds 12 seconds or memory/state budgets.
  - `AvailabilityWindow` (string): Rolling 30-day window aggregated for 99.5% target.
- **Relationships**: Generated from `ChatCommandSession`; feeds telemetry checks and optional logging dashboards.
- **Validation Rules**: Execution time must be recorded for every command; SLA breaches trigger warnings in chat output.

## Supporting Artifacts

- **JSON Contracts**: `contracts/copilot-chat-commands.json` defines property names, required parameters, and output envelope structure for each command.
- **State Files**: No schema changes required; `WorkflowStateSnapshot` maps directly to the existing serialized structure.
- **Telemetry Logging**: Performance data will be forwarded to `Services.GSCOrchestrationService` for optional persistence within existing logging framework.
