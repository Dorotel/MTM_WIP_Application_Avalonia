#!/bin/bash
#
# GSC Memory Command - Shell Wrapper
# Cross-platform memory file management with PowerShell Core detection
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
POWERSHELL_SCRIPT="$SCRIPT_DIR/gsc-memory.ps1"

# Check if PowerShell script exists
if [ ! -f "$POWERSHELL_SCRIPT" ]; then
    echo "❌ Error: PowerShell script not found: $POWERSHELL_SCRIPT"
    exit 1
fi

# Default parameters
OPERATION="get"
FILE_TYPE=""
PATTERN=""
REPLACE_CONFLICTING="true"
CONTEXT=""
WORKFLOW_ID=""
EXECUTION_CONTEXT="git-bash"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --operation|-o)
            OPERATION="$2"
            shift 2
            ;;
        --file-type|-f)
            FILE_TYPE="$2"
            shift 2
            ;;
        --pattern|-p)
            PATTERN="$2"
            shift 2
            ;;
        --context|-c)
            CONTEXT="$2"
            shift 2
            ;;
        --no-replace)
            REPLACE_CONFLICTING="false"
            shift
            ;;
        --workflow-id|-w)
            WORKFLOW_ID="$2"
            shift 2
            ;;
        --help|-h)
            echo "GSC Memory Command - Memory File Management"
            echo ""
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  -o, --operation <op>    Operation: get, post, status, update (default: get)"
            echo "  -f, --file-type <type>  Memory file type (avalonia-ui-memory, debugging-memory, memory, avalonia-custom-controls-memory)"
            echo "  -p, --pattern <text>    New pattern to add (for post/update operations)"
            echo "  -c, --context <text>    Context/section name for the pattern"
            echo "      --no-replace        Don't replace conflicting patterns"
            echo "  -w, --workflow-id <id>  Workflow ID for tracking"
            echo "  -h, --help              Show this help message"
            echo ""
            echo "Examples:"
            echo "  $0                                           # Show all memory files status"
            echo "  $0 -f avalonia-ui-memory                    # Show specific memory file"
            echo "  $0 -o post -f memory -p 'New pattern' -c 'Section Name'  # Add new pattern"
            echo "  $0 -o status                                 # Check integrity of all files"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Detect execution platform
PLATFORM="unknown"
case "$(uname -s)" in
    Darwin*)  PLATFORM="macOS" ;;
    Linux*)   PLATFORM="Linux" ;;
    CYGWIN*|MINGW*|MSYS*) PLATFORM="Windows" ;;
esac

# Execute PowerShell script with proper parameters
echo "🧠 GSC Memory Command (Shell Wrapper)"
echo "Platform: $PLATFORM | PowerShell: $POWERSHELL_CMD | Operation: $OPERATION"

$POWERSHELL_CMD -ExecutionPolicy Bypass -File "$POWERSHELL_SCRIPT" \
    -Operation "$OPERATION" \
    -FileType "$FILE_TYPE" \
    -Pattern "$PATTERN" \
    -ReplaceConflicting:$$REPLACE_CONFLICTING \
    -Context "$CONTEXT" \
    -WorkflowId "$WORKFLOW_ID" \
    -ExecutionContext "$EXECUTION_CONTEXT"

# Capture exit code
EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "✅ GSC Memory command completed successfully"
else
    echo ""
    echo "❌ GSC Memory command failed with exit code: $EXIT_CODE"
fi

exit $EXIT_CODE
