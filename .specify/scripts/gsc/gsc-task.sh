#!/bin/bash
#
# GSC Task Command - Shell Wrapper
# Cross-platform task generation with PowerShell Core detection
# Date: September 28, 2025

# Detect PowerShell Core availability
if command -v pwsh &> /dev/null; then
    POWERSHELL_CMD="pwsh"
elif command -v powershell &> /dev/null; then
    POWERSHELL_CMD="powershell"
else
    echo "❌ Error: PowerShell Core 7.0+ is required for GSC commands"
    echo "Please install PowerShell Core:"
    echo "  Windows: winget install Microsoft.PowerShell"
    echo "  macOS:   brew install powershell"
    echo "  Linux:   https://docs.microsoft.com/powershell/scripting/install/installing-powershell-core-on-linux"
    exit 1
fi

# Get script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
POWERSHELL_SCRIPT="$SCRIPT_DIR/gsc-task.ps1"

# Check if PowerShell script exists
if [ ! -f "$POWERSHELL_SCRIPT" ]; then
    echo "❌ Error: PowerShell script not found: $POWERSHELL_SCRIPT"
    exit 1
fi

# Detect execution platform
PLATFORM="unknown"
case "$(uname -s)" in
    Darwin*)  PLATFORM="macOS" ;;
    Linux*)   PLATFORM="Linux" ;;
    CYGWIN*|MINGW*|MSYS*) PLATFORM="Windows" ;;
esac

# Parse command line arguments
ARGUMENTS="$*"
WORKFLOW_ID=""
MEMORY_INTEGRATION="true"
CROSS_PLATFORM_MODE=$(echo "$PLATFORM" | tr '[:upper:]' '[:lower:]')
EXECUTION_CONTEXT="git-bash"

# Execute PowerShell script with proper parameters
echo "🔧 GSC Task Command (Shell Wrapper)"
echo "Platform: $PLATFORM | PowerShell: $POWERSHELL_CMD"

$POWERSHELL_CMD -ExecutionPolicy Bypass -File "$POWERSHELL_SCRIPT" \
    -Arguments "$ARGUMENTS" \
    -WorkflowId "$WORKFLOW_ID" \
    -MemoryIntegrationEnabled:$$MEMORY_INTEGRATION \
    -CrossPlatformMode "$CROSS_PLATFORM_MODE" \
    -ExecutionContext "$EXECUTION_CONTEXT"

# Capture exit code
EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "✅ GSC Task command completed successfully"
else
    echo ""
    echo "❌ GSC Task command failed with exit code: $EXIT_CODE"
fi

exit $EXIT_CODE
