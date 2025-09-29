#!/bin/bash
# GSC Clarify Shell Wrapper for Cross-Platform Compatibility
# Date: September 28, 2025
# Purpose: Cross-platform shell wrapper for PowerShell Core GSC Clarify command
# Task: T047 - Shell wrapper for gsc-clarify with PowerShell Core detection

set -euo pipefail

# Script metadata
SCRIPT_NAME="gsc-clarify"
SCRIPT_VERSION="1.0.0"
POWERSHELL_SCRIPT="gsc-clarify.ps1"

# Determine script directory (works on Linux, macOS, and Git Bash)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
POWERSHELL_SCRIPT_PATH="${SCRIPT_DIR}/${POWERSHELL_SCRIPT}"

# Color output functions
print_info() {
    echo -e "\033[36m[GSC-Clarify] $1\033[0m"
}

print_success() {
    echo -e "\033[32m[GSC-Clarify] $1\033[0m"
}

print_warning() {
    echo -e "\033[33m[GSC-Clarify] WARNING: $1\033[0m"
}

print_error() {
    echo -e "\033[31m[GSC-Clarify] ERROR: $1\033[0m"
}

print_debug() {
    if [[ "${GSC_DEBUG:-}" == "1" ]]; then
        echo -e "\033[37m[GSC-Clarify] DEBUG: $1\033[0m"
    fi
}

# PowerShell detection function
detect_powershell() {
    print_debug "Detecting PowerShell Core installation..."

    # Check for pwsh command (PowerShell Core 6+)
    if command -v pwsh >/dev/null 2>&1; then
        local pwsh_version
        pwsh_version=$(pwsh -NoProfile -Command '$PSVersionTable.PSVersion.ToString()' 2>/dev/null || echo "unknown")
        print_debug "Found PowerShell Core: pwsh (version: $pwsh_version)"
        echo "pwsh"
        return 0
    fi

    # Check for powershell command (might be PowerShell Core on some systems)
    if command -v powershell >/dev/null 2>&1; then
        local ps_version
        ps_version=$(powershell -NoProfile -Command '$PSVersionTable.PSVersion.ToString()' 2>/dev/null || echo "unknown")
        print_debug "Found PowerShell: powershell (version: $ps_version)"
        echo "powershell"
        return 0
    fi

    print_error "PowerShell Core not found. Please install PowerShell 7.0 or later."
    print_info "Installation instructions:"
    print_info "  Linux: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux"
    print_info "  macOS: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos"
    print_info "  Windows: Install PowerShell 7+ from Microsoft Store or GitHub releases"
    return 1
}

# Argument transformation for PowerShell compatibility
transform_arguments() {
    local args=()

    while [[ $# -gt 0 ]]; do
        case $1 in
            --action=*)
                args+=("-Action" "${1#*=}")
                ;;
            --action)
                args+=("-Action" "$2")
                shift
                ;;
            --requirements-file=*)
                args+=("-RequirementsFile" "${1#*=}")
                ;;
            --requirements-file)
                args+=("-RequirementsFile" "$2")
                shift
                ;;
            --ambiguity-type=*)
                args+=("-AmbiguityType" "${1#*=}")
                ;;
            --ambiguity-type)
                args+=("-AmbiguityType" "$2")
                shift
                ;;
            --output-format=*)
                args+=("-OutputFormat" "${1#*=}")
                ;;
            --output-format)
                args+=("-OutputFormat" "$2")
                shift
                ;;
            --workflow-id=*)
                args+=("-WorkflowId" "${1#*=}")
                ;;
            --workflow-id)
                args+=("-WorkflowId" "$2")
                shift
                ;;
            --memory-integration=*)
                args+=("-MemoryIntegration" "${1#*=}")
                ;;
            --memory-integration|--no-memory-integration)
                if [[ "$1" == "--no-memory-integration" ]]; then
                    args+=("-MemoryIntegration" "false")
                else
                    args+=("-MemoryIntegration" "$2")
                    shift
                fi
                ;;
            --help|-h)
                args+=("-Action" "help")
                ;;
            --debug)
                export GSC_DEBUG=1
                print_debug "Debug mode enabled"
                ;;
            *)
                # Pass through unknown arguments
                args+=("$1")
                ;;
        esac
        shift
    done

    # If no action specified, default to help
    if [[ ${#args[@]} -eq 0 ]]; then
        args=("-Action" "help")
    fi

    print_debug "Transformed arguments: ${args[*]}"
    printf '%s\n' "${args[@]}"
}

# Display usage information
show_usage() {
    cat << 'EOF'
# GSC Clarify Shell Wrapper Usage

Cross-platform wrapper for GSC Clarify command with debugging memory workflows integration.

## Usage
```bash
./gsc-clarify.sh [options]
```

## Options

### Actions
--action <action>           Clarification action (help|analyze|resolve|validate|patterns)
--action=<action>          Alternative syntax

### Parameters
--requirements-file <file>  Path to requirements file (default: ./requirements.md)
--ambiguity-type <type>     Ambiguity type (layout|styling|business|technical|integration)
--output-format <format>    Output format (markdown|json|console|report)
--workflow-id <id>          Optional workflow session identifier
--memory-integration <bool> Enable memory integration (default: true)
--no-memory-integration     Disable memory integration

### General
--help, -h                 Show this help message
--debug                    Enable debug output

## Examples

### Basic Usage
```bash
./gsc-clarify.sh --action help
./gsc-clarify.sh --action analyze
./gsc-clarify.sh --action patterns
```

### Requirements Analysis
```bash
./gsc-clarify.sh --action analyze --requirements-file "./specs/ui-feature/requirements.md"
./gsc-clarify.sh --action analyze --output-format report
```

### Ambiguity Resolution
```bash
./gsc-clarify.sh --action resolve --ambiguity-type layout
./gsc-clarify.sh --action resolve --ambiguity-type styling --output-format console
```

### Validation
```bash
./gsc-clarify.sh --action validate --requirements-file "./requirements.md"
./gsc-clarify.sh --action validate --output-format json
```

## Cross-Platform Notes

- Requires PowerShell Core 7.0+ (automatically detected)
- Compatible with Linux, macOS, Windows (Git Bash)
- All PowerShell-style arguments are automatically transformed
- Debug mode: GSC_DEBUG=1 ./gsc-clarify.sh --action help

## Memory Integration

Automatically loads debugging memory patterns from:
- debugging-memory.instructions.md - Systematic debugging workflows
- memory.instructions.md - Universal development patterns
- avalonia-ui-memory.instructions.md - Avalonia UI debugging patterns

---
*GSC Clarify Shell Wrapper v1.0.0 - Phase 3.4 Enhanced GSC Command Implementation*
EOF
}

# Main execution function
main() {
    print_debug "Starting GSC Clarify shell wrapper..."

    # Show usage if requested
    if [[ "${1:-}" == "--help" ]] || [[ "${1:-}" == "-h" ]] || [[ "${1:-}" == "help" ]]; then
        show_usage
        exit 0
    fi

    # Detect PowerShell
    print_debug "Checking PowerShell availability..."
    local powershell_cmd
    if ! powershell_cmd=$(detect_powershell); then
        exit 1
    fi

    # Check if PowerShell script exists
    if [[ ! -f "$POWERSHELL_SCRIPT_PATH" ]]; then
        print_error "PowerShell script not found: $POWERSHELL_SCRIPT_PATH"
        exit 1
    fi

    print_debug "Using PowerShell script: $POWERSHELL_SCRIPT_PATH"

    # Transform arguments
    print_debug "Processing arguments: $*"
    local -a transformed_args
    mapfile -t transformed_args < <(transform_arguments "$@")

    # Execute PowerShell script with transformed arguments
    print_debug "Executing: $powershell_cmd -NoProfile -ExecutionPolicy Bypass -File '$POWERSHELL_SCRIPT_PATH' ${transformed_args[*]}"

    exec "$powershell_cmd" -NoProfile -ExecutionPolicy Bypass -File "$POWERSHELL_SCRIPT_PATH" "${transformed_args[@]}"
}

# Handle script execution
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
