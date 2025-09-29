# GSC Enhancement System

This document describes the GitHub Spec Commands (GSC) enhancement system integrated into this repository.

- Memory integration across commands
- Cross-platform support via PowerShell Core + Bash wrappers
- State files in `.specify/state/`
- Security and monitoring scripts in `.specify/scripts/powershell/`

## Commands

Primary commands are implemented in `.specify/scripts/gsc/*.ps1` with shell wrappers. Each command supports `-Json` for machine-readable output.

- constitution, specify, clarify, plan, task, analyze, implement
- memory (GET/POST-like operations via parameters)
- validate, status, rollback, help

## State Files

- `gsc-workflow.json` — workflow progress
- `validation-status.json` — validation results (future)

## Security/Monitoring

- `security/*` (checksums, encryption, access logging)
- `monitoring/*` (performance trackers)

## Performance Targets

- Command execution: < 30s
- Memory processing: < 5s
- State persistence: < 2s

## Developer Notes

- Commands import shared modules from `.specify/scripts/powershell/`
- Memory locations are auto-detected via `memory-paths.json`
- Use `./gsc help -Json` for a complete catalog

## Documentation

- Interactive help: `docs/gsc-interactive-help.html`
- Constitutional mandate: see Article X in `constitution.md`
