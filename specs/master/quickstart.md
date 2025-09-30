# GSC Enhancement System - Quickstart Guide

**Feature**: Complete GitHub Spec Commands Enhancement System with Memory Integration  
**Date**: September 28, 2025  
**Target Audience**: Manufacturing software developers using MTM WIP Application

## Prerequisites

### Required Software

- **PowerShell Core 7.0+**: Cross-platform PowerShell runtime

  ```bash
  # Windows (via winget)
  winget install Microsoft.PowerShell
  
  # macOS (via Homebrew)
  brew install powershell
  
  # Linux (Ubuntu/Debian)
  wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
  sudo dpkg -i packages-microsoft-prod.deb
  sudo apt-get update
  sudo apt-get install -y powershell
  ```

- **Git Bash**: For cross-platform command execution
- **@github/spec-kit**: For slash command integration (optional)

### Memory Files Setup

Ensure memory files exist in your prompts directory:

```bash
# Windows
C:\Users\[USER]\AppData\Roaming\Code\User\prompts\
├── avalonia-ui-memory.instructions.md
├── debugging-memory.instructions.md
├── memory.instructions.md
└── avalonia-custom-controls-memory.instructions.md

# macOS/Linux
~/.local/share/Code/User/prompts/
├── avalonia-ui-memory.instructions.md
├── debugging-memory.instructions.md
├── memory.instructions.md
└── avalonia-custom-controls-memory.instructions.md
```

## Installation

### 1. Clone and Setup GSC Enhancement System

```bash
# Navigate to MTM WIP Application repository
cd MTM_WIP_Application_Avalonia

# Verify existing .specify directory structure
ls .specify/

# Expected output:
# scripts/  templates/  memory/

# Install enhanced GSC scripts (implementation will add these)
# .specify/scripts/gsc/
# .specify/state/
# .specify/config/
```

### 2. Verify PowerShell Core Installation

```bash
# Test PowerShell Core availability
pwsh --version
# Expected: PowerShell 7.0.0 or higher

# Test JSON handling capabilities
pwsh -Command "ConvertTo-Json @{test='value'}"
# Expected: {"test": "value"}
```

### 3. Initialize State Management

```bash
# Create state directory (will be automated in implementation)
mkdir .specify/state

# Initialize empty state files
pwsh -Command "
@{
  workflowId = [guid]::NewGuid().ToString()
  currentPhase = 'not_started'
  phaseHistory = @()
  checkpointData = @{}
} | ConvertTo-Json | Out-File .specify/state/gsc-workflow.json
"
```

## Quick Start Workflow

### Scenario 1: New Feature Development with Memory Integration

**Objective**: Create a new Avalonia UI feature using accumulated memory patterns

```bash
# Step 1: Constitutional validation with memory integration
./gsc constitution "New inventory management panel feature"
# Expected: Constitution compliance validated, memory patterns loaded
# Time: <5 seconds

# Step 2: Feature specification with Avalonia UI memory
./gsc specify "Add real-time inventory status panel with drag-drop functionality"
# Expected: Specification created with Avalonia UI patterns applied
# Time: <10 seconds

# Step 3: Requirements clarification with debugging memory
./gsc clarify
# Expected: Interactive clarification questions using systematic problem-solving
# Time: <5 seconds per question

# Step 4: Implementation planning with universal patterns
./gsc plan
# Expected: Complete technical plan with memory-driven architecture decisions
# Time: <15 seconds

# Step 5: Task generation with custom control memory
./gsc task
# Expected: Detailed task list with Avalonia custom control patterns
# Time: <10 seconds

# Step 6: Implementation analysis
./gsc analyze
# Expected: Code quality analysis with memory-driven recommendations
# Time: <10 seconds

# Step 7: Feature implementation
./gsc implement
# Expected: Code generation using all applicable memory patterns
# Time: <30 seconds
```

**Expected Total Time**: <85 seconds (well within 30-second per command limit)

### Scenario 2: Team Collaboration with Lock Management

**Objective**: Work on shared feature with manufacturing shift handoffs

```bash
# Developer A (Day Shift) starts feature
./gsc constitution "Manufacturing operator efficiency improvements"
# Lock acquired automatically for team collaboration

# Check workflow status
./gsc status
# Expected output:
# {
#   "workflowId": "abc-123-def",
#   "currentPhase": "constitution",
#   "teamCollaborationStatus": {
#     "isLocked": true,
#     "lockOwner": "developer-a",
#     "lockExpiration": "2025-09-28T20:00:00Z"
#   }
# }

# Developer B (Night Shift) attempts to work on same feature
./gsc status
# Expected: Lock status shows Developer A owns workflow
# Action: Wait for lock expiration or coordinate handoff

# Developer A completes phase and releases lock
./gsc specify "Add operator productivity dashboard"
# Lock maintained through specification phase

# Shift handoff: Developer A explicitly releases
./gsc validate
# Validates current work and prepares for handoff
# Lock released after successful validation
```

### Scenario 3: Performance Degradation and Recovery

**Objective**: Handle performance issues with graceful degradation

```bash
# Normal operation
./gsc constitution "Large-scale inventory system enhancement"
# Memory files loaded: 4 files, ~700KB, processing time ~1-2 seconds

# Simulate performance degradation
# (System detects memory file processing taking >5 seconds)

# Automatic graceful degradation activated
./gsc plan
# Expected behavior:
# - Reduced memory file complexity
# - Core GSC functionality maintained
# - Warning message about degraded mode
# - Still completes within performance targets

# Check degradation status
./gsc status
# Expected output includes:
# "performanceDegradationMode": true

# Recovery when performance improves
# System automatically returns to normal mode
./gsc validate
# Full memory integration restored
```

### Scenario 4: Error Recovery and Rollback

**Objective**: Recover from workflow errors using full reset

```bash
# Start feature development
./gsc constitution "Complex manufacturing workflow integration"
./gsc specify "Multi-system integration with real-time sync"
./gsc clarify
./gsc plan

# Error occurs during planning phase
# (e.g., memory file corruption, invalid state)

# Check workflow status
./gsc status
# Shows error state in planning phase

# Full workflow reset to beginning of current phase
./gsc rollback
# Expected behavior:
# - Reset to beginning of 'plan' phase
# - Preserve accumulated memory patterns
# - Clear corrupted state
# - Restore from last known good checkpoint

# Resume from clean state
./gsc plan
# Continue with fresh planning phase
```

### Scenario 5: Memory File Management

**Objective**: Manage and update memory files with new patterns

```bash
# View current memory file status
./gsc memory
# Expected output:
# {
#   "memoryFiles": [
#     {
#       "filePath": "C:/Users/johnk/AppData/Roaming/Code/User/prompts/avalonia-ui-memory.instructions.md",
#       "fileType": "avalonia-ui-memory",
#       "isValid": true,
#       "patternCount": 25,
#       "checksumHash": "sha256:abc123..."
#     }
#   ],
#   "totalPatterns": 87,
#   "integrityStatus": "valid"
# }

# Update memory file with new pattern discovered during development
./gsc memory --update avalonia-ui-memory --pattern "DataGrid column binding context switching resolution"
# Expected: Pattern added to avalonia-ui-memory.instructions.md
# Conflicting patterns replaced (single source of truth)

# Validate memory file integrity
./gsc validate --memory-only
# Expected: All memory files validated for checksum and content integrity
```

## Cross-Platform Validation

### Windows (PowerShell + Git Bash)

```bash
# Test PowerShell execution
pwsh -File .specify/scripts/gsc/gsc-constitution.ps1 "test feature"

# Test Git Bash wrapper
./.specify/scripts/gsc/gsc-constitution.sh "test feature"
```

### macOS (PowerShell Core + Bash)

```bash
# Test PowerShell Core execution
pwsh -File .specify/scripts/gsc/gsc-constitution.ps1 "test feature"

# Test Bash wrapper
./.specify/scripts/gsc/gsc-constitution.sh "test feature"
```

### Linux (PowerShell Core + Bash)

```bash
# Test PowerShell Core execution
pwsh -File .specify/scripts/gsc/gsc-constitution.ps1 "test feature"

# Test Bash wrapper
chmod +x .specify/scripts/gsc/gsc-constitution.sh
./.specify/scripts/gsc/gsc-constitution.sh "test feature"
```

## Performance Benchmarks

### Expected Performance Targets

| Operation | Target Time | Measured Performance |
|-----------|-------------|---------------------|
| Memory file reading | <5 seconds | ~1-2 seconds |
| GSC command execution | <30 seconds | ~15-20 seconds |
| State persistence | <2 seconds | ~500ms-1s |
| Cross-platform validation | <60 seconds | ~45-50 seconds |

### Performance Monitoring

```bash
# Monitor GSC command performance
./gsc status --performance
# Expected output includes execution times for recent commands

# Test performance under load
for i in {1..5}; do
  time ./gsc validate
done
# Verify consistent performance across multiple executions
```

## Troubleshooting

### Common Issues

## **1. PowerShell Core Not Found**

```bash
# Error: "PowerShell Core 7.0+ required for GSC commands"
# Solution: Install PowerShell Core as shown in Prerequisites
```

## **2. Memory Files Missing**

```bash
# Error: "Memory file not found: avalonia-ui-memory.instructions.md"
# Solution: Ensure memory files exist in prompts directory
ls "C:\Users\[USER]\AppData\Roaming\Code\User\prompts\"
```

## **3. State File Corruption**

```bash
# Error: "Invalid JSON in gsc-workflow.json"
# Solution: Reset state files
./gsc rollback --full-reset
```

## **4. Lock Conflicts**

```bash
# Error: "Workflow locked by another user"
# Solution: Wait for lock expiration or coordinate with team
./gsc status
# Check lock expiration time
```

## **5. Performance Degradation**

```bash
# Warning: "Graceful degradation mode activated"
# Solution: Monitor system resources, degradation is automatic
./gsc status
# Check performanceDegradationMode status
```

## Integration with Existing Workflows

### VS Code Integration

```json
// .vscode/tasks.json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "GSC Constitution",
      "type": "shell",
      "command": "./gsc",
      "args": ["constitution", "${input:featureDescription}"],
      "group": "build",
      "presentation": {
        "echo": true,
        "reveal": "always",
        "focus": false,
        "panel": "shared"
      }
    }
  ],
  "inputs": [
    {
      "id": "featureDescription",
      "description": "Feature description for GSC workflow",
      "default": "New feature development",
      "type": "promptString"
    }
  ]
}
```

### CI/CD Pipeline Integration

```yaml
# .github/workflows/gsc-validation.yml
name: GSC Workflow Validation
on:
  pull_request:
    branches: [main]

jobs:
  validate-gsc-workflow:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Install PowerShell Core
        run: |
          wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
          sudo dpkg -i packages-microsoft-prod.deb
          sudo apt-get update
          sudo apt-get install -y powershell
      - name: Validate GSC Workflow
        run: |
          ./gsc validate --ci-mode
          ./gsc status --json > gsc-status.json
      - name: Upload GSC Results
        uses: actions/upload-artifact@v3
        with:
          name: gsc-validation-results
          path: gsc-status.json
```

## Next Steps

1. **Complete Implementation**: Follow the generated tasks.md for detailed implementation steps
2. **Cross-Platform Testing**: Validate on all target platforms (Windows/macOS/Linux)
3. **Memory Integration Testing**: Verify memory file processing and pattern application
4. **Performance Optimization**: Monitor and optimize for manufacturing 24/7 operations
5. **Team Training**: Train development team on enhanced GSC workflow

## Support and Documentation

- **Constitutional Reference**: `.specify/memory/constitution.md`
- **Memory Files**: `C:\Users\[USER]\AppData\Roaming\Code\User\prompts\*-memory.instructions.md`
- **Instruction Library**: `.github/instructions/` (34+ specialized instruction files)
- **Templates**: `.specify/templates/` for GSC command templates

---

## *Quickstart guide generated: September 28, 2025*

### Scenario 6: Updating Specs Safely with Backups (/gsc/update)

Objective: Make targeted edits to spec sections with automatic backups, lock handling, and optional validation.

```bash
# Suggest changes (no write) for a section
pwsh -File .specify/scripts/gsc/gsc-update.ps1 -File specs/master/spec.md -Section "Requirements" -Operation suggest -Json

# Insert content into a section (creates section if missing)
pwsh -File .specify/scripts/gsc/gsc-update.ps1 -File specs/master/spec.md -Section "Requirements" -Operation insert -Content "- New acceptance criteria" -ValidateAfter

# Replace a section from file contents
pwsh -File .specify/scripts/gsc/gsc-update.ps1 -File specs/master/spec.md -Section "Plan" -Operation replace -ContentPath specs/master/plan.md -Force

# Remove a section
pwsh -File .specify/scripts/gsc/gsc-update.ps1 -File specs/master/spec.md -Section "Deprecated" -Operation remove

# Notes
# - Backups are stored under .specify/state/backups
# - Decision records are written under specs/master/decisions
# - Collaboration lock is respected; use -Force to bypass when permitted
# - Use -Json for contract-compliant output (success, command, executionTime, etc.)
```
