#!/bin/bash
# GSC Plan Shell Wrapper for Cross-Platform Compatibility
# Date: September 28, 2025
# Purpose: Cross-platform shell wrapper for PowerShell Core GSC Plan command
# Task: T049 - Shell wrapper for gsc-plan with PowerShell Core detection

set -euo pipefail

# Script metadata
SCRIPT_NAME="gsc-plan"
SCRIPT_VERSION="1.0.0"
POWERSHELL_SCRIPT="gsc-plan.ps1"

# Determine script directory (works on Linux, macOS, and Git Bash)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
POWERSHELL_SCRIPT_PATH="${SCRIPT_DIR}/${POWERSHELL_SCRIPT}"

# Color output functions
print_info() {
    echo -e "\033[36m[GSC-Plan] $1\033[0m"
}

print_success() {
    echo -e "\033[32m[GSC-Plan] $1\033[0m"
}

print_warning() {
    echo -e "\033[33m[GSC-Plan] WARNING: $1\033[0m"
}

print_error() {
    echo -e "\033[31m[GSC-Plan] ERROR: $1\033[0m"
}

print_debug() {
    if [[ "${GSC_DEBUG:-}" == "1" ]]; then
        echo -e "\033[37m[GSC-Plan] DEBUG: $1\033[0m"
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
            --target=*)
                args+=("-Target" "${1#*=}")
                ;;
            --target)
                args+=("-Target" "$2")
                shift
                ;;
            --output-format=*)
                args+=("-OutputFormat" "${1#*=}")
                ;;
            --output-format)
                args+=("-OutputFormat" "$2")
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
            --validation-level=*)
                args+=("-ValidationLevel" "${1#*=}")
                ;;
            --validation-level)
                args+=("-ValidationLevel" "$2")
                shift
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
# GSC Plan Shell Wrapper Usage

Cross-platform wrapper for GSC Plan command with universal development patterns integration.

## Usage
```bash
./gsc-plan.sh [options]
```

## Options

### Actions
--action <action>           Planning action (help|create|update|validate|patterns)
--action=<action>          Alternative syntax

### Parameters
--target <target>          Target for planning (required for create/update/validate)
--output-format <format>   Output format (console|markdown|json|html)
--memory-integration <bool> Enable memory integration (default: true)
--no-memory-integration    Disable memory integration
--validation-level <level> Validation strictness (basic|standard|comprehensive)

### General
--help, -h                Show this help message
--debug                   Enable debug output

## Examples

### Basic Usage
```bash
./gsc-plan.sh --action help
./gsc-plan.sh --action patterns
./gsc-plan.sh --action patterns --no-memory-integration
```

### Plan Creation
```bash
./gsc-plan.sh --action create --target "InventoryManagement"
./gsc-plan.sh --action create --target "UIComponent" --output-format markdown
./gsc-plan.sh --action create --target "ServiceLayer" --output-format json
```

### Plan Updates and Validation
```bash
./gsc-plan.sh --action update --target "./implementation-plan.md"
./gsc-plan.sh --action validate --target "./plan.md" --validation-level comprehensive
./gsc-plan.sh --action validate --target "./plan.md" --validation-level basic
```

### Pattern Display
```bash
./gsc-plan.sh --action patterns --output-format markdown
./gsc-plan.sh --action patterns --memory-integration true --debug
```

## Cross-Platform Notes

- Requires PowerShell Core 7.0+ (automatically detected)
- Compatible with Linux, macOS, Windows (Git Bash)
- All PowerShell-style arguments are automatically transformed
- Debug mode: GSC_DEBUG=1 ./gsc-plan.sh --action help

## Universal Patterns Integration

Automatically loads universal development patterns from:
- memory.instructions.md - Universal development patterns
- debugging-memory.instructions.md - Systematic debugging workflows
- avalonia-ui-memory.instructions.md - Avalonia UI patterns
- avalonia-custom-controls-memory.instructions.md - Custom control patterns

### Pattern Categories
- Container Layout Principles - UI constraint management
- Style Architecture Patterns - Cascading style organization
- Evidence-Based Development - Reliable development practices
- Systematic Debugging - Problem resolution workflows
- Multi-Platform Development - Cross-framework compatibility
- Documentation and Learning - Knowledge transformation

## Performance
- Target execution time: <30 seconds
- Memory integration: Auto-discovery from standard locations
- Built-in fallback patterns when memory files unavailable

---
*GSC Plan Shell Wrapper v1.0.0 - Phase 3.4 Enhanced GSC Command Implementation*
EOF
}

# Main execution function
main() {
    print_debug "Starting GSC Plan shell wrapper..."

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
