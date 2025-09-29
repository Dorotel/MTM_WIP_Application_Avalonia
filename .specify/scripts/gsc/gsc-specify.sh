#!/bin/bash
# GSC Specify Command Shell Wrapper
# Date: September 28, 2025
# Purpose: Cross-platform shell wrapper for PowerShell Core gsc-specify command
# Phase 3.4: Enhanced GSC Command Implementation

set -euo pipefail

# Script directory detection
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
POWERSHELL_SCRIPT="$SCRIPT_DIR/gsc-specify.ps1"

# Color output functions
color_red() { echo -e "\033[31m$1\033[0m"; }
color_green() { echo -e "\033[32m$1\033[0m"; }
color_yellow() { echo -e "\033[33m$1\033[0m"; }
color_blue() { echo -e "\033[34m$1\033[0m"; }
color_cyan() { echo -e "\033[36m$1\033[0m"; }

# Help function
show_help() {
    color_cyan "GSC Specify Command - Enhanced Specification Management"
    echo ""
    echo "Usage: $0 -Action <action> -SpecType <type> [options]"
    echo ""
    color_yellow "Actions:"
    echo "  create     - Create new specification with template"
    echo "  validate   - Validate specification against patterns"
    echo "  template   - Manage specification templates"
    echo "  analyze    - Analyze specification requirements"
    echo "  memory-sync- Synchronize with memory patterns"
    echo "  patterns   - Display available patterns"
    echo "  help       - Show this help"
    echo ""
    color_yellow "Specification Types:"
    echo "  ui-component  - Avalonia UI components"
    echo "  viewmodel     - MVVM ViewModels"
    echo "  service       - Service implementations"
    echo "  database      - Database operations"
    echo "  workflow      - Manufacturing workflows"
    echo "  manufacturing - Manufacturing domain components"
    echo ""
    color_yellow "Examples:"
    echo "  $0 -Action create -SpecType ui-component -Name \"InventoryControl\""
    echo "  $0 -Action validate -SpecType viewmodel -Name \"MainViewModel\" -MemoryIntegration"
    echo "  $0 -Action memory-sync -SpecType manufacturing -OutputFormat json"
    echo ""
}

# PowerShell detection and validation
detect_powershell() {
    local pwsh_cmd=""

    # Try PowerShell Core first (cross-platform)
    if command -v pwsh >/dev/null 2>&1; then
        pwsh_cmd="pwsh"
        local version=$(pwsh -c '$PSVersionTable.PSVersion.Major')
        if [[ $version -ge 7 ]]; then
            color_green "[GSC-Specify] Found PowerShell Core $version"
            echo "$pwsh_cmd"
            return 0
        else
            color_yellow "[GSC-Specify] PowerShell Core version $version found, but version 7+ recommended"
        fi
    fi

    # Try Windows PowerShell on Windows/WSL
    if command -v powershell >/dev/null 2>&1; then
        pwsh_cmd="powershell"
        color_yellow "[GSC-Specify] Using Windows PowerShell (PowerShell Core recommended)"
        echo "$pwsh_cmd"
        return 0
    fi

    # PowerShell not found
    color_red "[GSC-Specify] PowerShell not found. Please install PowerShell Core 7+"
    color_cyan "Installation: https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell"
    return 1
}

# Argument parsing and validation
parse_arguments() {
    local action=""
    local spec_type="ui-component"
    local name=""
    local template="avalonia-usercontrol"
    local output_format="markdown"
    local memory_integration=false
    local verbose=false
    local powershell_args=()

    # Parse command line arguments
    while [[ $# -gt 0 ]]; do
        case $1 in
            -Action)
                action="$2"
                shift 2
                ;;
            -SpecType)
                spec_type="$2"
                shift 2
                ;;
            -Name)
                name="$2"
                shift 2
                ;;
            -Template)
                template="$2"
                shift 2
                ;;
            -OutputFormat)
                output_format="$2"
                shift 2
                ;;
            -MemoryIntegration)
                memory_integration=true
                shift
                ;;
            -Verbose)
                verbose=true
                shift
                ;;
            -h|--help|help)
                show_help
                exit 0
                ;;
            *)
                color_red "[GSC-Specify] Unknown argument: $1"
                show_help
                exit 1
                ;;
        esac
    done

    # Validate required parameters
    if [[ -z "$action" ]]; then
        color_red "[GSC-Specify] Action parameter is required"
        show_help
        exit 1
    fi

    # Build PowerShell arguments
    powershell_args=("-Action" "$action")

    if [[ -n "$spec_type" ]]; then
        powershell_args+=("-SpecType" "$spec_type")
    fi

    if [[ -n "$name" ]]; then
        powershell_args+=("-Name" "$name")
    fi

    if [[ -n "$template" ]]; then
        powershell_args+=("-Template" "$template")
    fi

    if [[ -n "$output_format" ]]; then
        powershell_args+=("-OutputFormat" "$output_format")
    fi

    if [[ "$memory_integration" == true ]]; then
        powershell_args+=("-MemoryIntegration")
    fi

    if [[ "$verbose" == true ]]; then
        powershell_args+=("-Verbose")
    fi

    echo "${powershell_args[@]}"
}

# Main execution function
main() {
    color_cyan "[GSC-Specify] Enhanced Specification Management v1.0"
    color_blue "[GSC-Specify] Script: $POWERSHELL_SCRIPT"

    # Detect PowerShell
    local pwsh_cmd
    pwsh_cmd=$(detect_powershell) || exit 1

    # Validate PowerShell script exists
    if [[ ! -f "$POWERSHELL_SCRIPT" ]]; then
        color_red "[GSC-Specify] PowerShell script not found: $POWERSHELL_SCRIPT"
        exit 1
    fi

    # Parse arguments
    local powershell_args
    if ! powershell_args=($(parse_arguments "$@")); then
        exit 1
    fi

    # Execute PowerShell script
    color_yellow "[GSC-Specify] Executing: $pwsh_cmd -File \"$POWERSHELL_SCRIPT\" ${powershell_args[*]}"

    # Set execution policy for current session (Windows)
    if [[ "$pwsh_cmd" == "powershell" ]] || [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
        "$pwsh_cmd" -ExecutionPolicy Bypass -File "$POWERSHELL_SCRIPT" "${powershell_args[@]}"
    else
        "$pwsh_cmd" -File "$POWERSHELL_SCRIPT" "${powershell_args[@]}"
    fi

    local exit_code=$?

    if [[ $exit_code -eq 0 ]]; then
        color_green "[GSC-Specify] Command completed successfully"
    else
        color_red "[GSC-Specify] Command failed with exit code: $exit_code"
    fi

    exit $exit_code
}

# Script execution
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
