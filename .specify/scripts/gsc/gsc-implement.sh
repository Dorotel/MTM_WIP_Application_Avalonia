#!/bin/bash

#
# GSC Command: implement (Shell Wrapper)
# Cross-platform shell wrapper for gsc-implement.ps1
# Provides Git Bash and Unix shell compatibility on Windows/macOS/Linux
#

# Get the directory of this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
POWERSHELL_SCRIPT="$SCRIPT_DIR/gsc-implement.ps1"

# Source cross-platform utilities
source "$SCRIPT_DIR/../powershell/cross-platform-utils.sh" 2>/dev/null || true

# Detect PowerShell command (pwsh on all platforms, powershell.exe on Windows as fallback)
if command -v pwsh >/dev/null 2>&1; then
    PWSH_CMD="pwsh"
elif command -v powershell.exe >/dev/null 2>&1; then
    PWSH_CMD="powershell.exe"
elif command -v powershell >/dev/null 2>&1; then
    PWSH_CMD="powershell"
else
    echo "❌ ERROR: PowerShell Core 7.0+ is required for GSC commands"
    echo "Install PowerShell Core from: https://github.com/PowerShell/PowerShell"
    exit 1
fi

# Convert Unix-style paths to PowerShell-compatible paths if needed
if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "cygwin" ]]; then
    # Git Bash on Windows - convert paths
    POWERSHELL_SCRIPT=$(cygpath -w "$POWERSHELL_SCRIPT" 2>/dev/null || echo "$POWERSHELL_SCRIPT")
fi

# Build PowerShell command arguments
PWSH_ARGS=()
PWSH_ARGS+=("-File" "$POWERSHELL_SCRIPT")

# Process command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            echo "GSC Implement Command - Comprehensive Implementation with Memory Integration"
            echo ""
            echo "Usage: gsc-implement [OPTIONS] [IMPLEMENTATION_SCOPE]"
            echo ""
            echo "Arguments:"
            echo "  IMPLEMENTATION_SCOPE   Scope for implementation (default: full-implementation)"
            echo ""
            echo "Options:"
            echo "  -w, --workflow-id ID   Specify workflow ID"
            echo "  --no-memory           Disable memory integration"
            echo "  --platform PLATFORM   Target platform (windows|macos|linux)"
            echo "  --context CONTEXT     Execution context (powershell|git-bash|copilot-chat-vscode|copilot-chat-vs2022)"
            echo "  --chat-formatting     Enable chat formatting for output"
            echo "  --dry-run             Show what would be implemented without creating artifacts"
            echo "  -v, --verbose         Enable verbose output"
            echo "  -h, --help           Show this help message"
            echo ""
            echo "Examples:"
            echo "  gsc-implement                                 # Full implementation with all patterns"
            echo "  gsc-implement \"UI components\"                # Implement specific scope"
            echo "  gsc-implement --dry-run                      # Preview implementation without changes"
            echo "  gsc-implement --no-memory --verbose         # Implement without memory patterns"
            echo ""
            exit 0
            ;;
        -w|--workflow-id)
            PWSH_ARGS+=("-WorkflowId" "$2")
            shift 2
            ;;
        --no-memory)
            PWSH_ARGS+=("-MemoryIntegrationEnabled" '$false')
            shift
            ;;
        --platform)
            PWSH_ARGS+=("-CrossPlatformMode" "$2")
            shift 2
            ;;
        --context)
            PWSH_ARGS+=("-GSCExecutionContext" "$2")
            shift 2
            ;;
        --chat-formatting)
            PWSH_ARGS+=("-ChatFormatting")
            shift
            ;;
        --dry-run)
            PWSH_ARGS+=("-DryRun")
            shift
            ;;
        -v|--verbose)
            PWSH_ARGS+=("-Verbose")
            shift
            ;;
        -*)
            echo "❌ ERROR: Unknown option: $1"
            echo "Use -h or --help for usage information"
            exit 1
            ;;
        *)
            # Positional argument - implementation scope
            PWSH_ARGS+=("$1")
            shift
            ;;
    esac
done

# Set execution context for shell wrapper
PWSH_ARGS+=("-GSCExecutionContext" "git-bash")

# Detect platform for cross-platform mode
if [[ -z "${PWSH_ARGS[*]}" =~ "-CrossPlatformMode" ]]; then
    case "$OSTYPE" in
        darwin*)
            PWSH_ARGS+=("-CrossPlatformMode" "macos")
            ;;
        linux*)
            PWSH_ARGS+=("-CrossPlatformMode" "linux")
            ;;
        msys*|cygwin*)
            PWSH_ARGS+=("-CrossPlatformMode" "windows")
            ;;
        *)
            PWSH_ARGS+=("-CrossPlatformMode" "linux")  # Default fallback
            ;;
    esac
fi

# Execute PowerShell script with error handling
echo "🚀 Running GSC implementation with comprehensive memory patterns..."

if ! "$PWSH_CMD" "${PWSH_ARGS[@]}"; then
    exit_code=$?
    echo ""
    echo "❌ GSC implement command failed (exit code: $exit_code)"
    echo ""
    echo "Troubleshooting:"
    echo "1. Ensure PowerShell Core 7.0+ is installed"
    echo "2. Check that a GSC workflow is active (run 'gsc constitution' first)"
    echo "3. Verify you are in the task or analyze phase"
    echo "4. Try running with --verbose for more details"
    echo "5. Use --dry-run to preview implementation without changes"
    echo ""
    exit $exit_code
fi

echo ""
echo "✅ GSC implementation completed successfully!"
echo "🎯 All memory patterns have been applied to generate the implementation"
echo "💡 Run 'gsc validate' to validate the implementation results"
