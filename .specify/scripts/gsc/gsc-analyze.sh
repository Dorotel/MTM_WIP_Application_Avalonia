#!/bin/bash
# GSC Analyze Command - Shell Wrapper
set -euo pipefail

SCRIPT_NAME="gsc-analyze"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PS_SCRIPT="$SCRIPT_DIR/gsc-analyze.ps1"

log() { echo -e "\033[36m[${SCRIPT_NAME}]\033[0m $*"; }
err() { echo -e "\033[31m[${SCRIPT_NAME}] ERROR:\033[0m $*" >&2; }

detect_pwsh() {
  if command -v pwsh >/dev/null 2>&1; then echo pwsh; return 0; fi
  if command -v powershell >/dev/null 2>&1; then echo powershell; return 0; fi
  err "PowerShell 7+ is required"; exit 1
}

main() {
  local pwsh
  pwsh=$(detect_pwsh)
  if [ ! -f "$PS_SCRIPT" ]; then err "Missing PowerShell script: $PS_SCRIPT"; exit 1; fi
  log "Executing analyze command via PowerShell"
  "$pwsh" -File "$PS_SCRIPT" "$@"
}

main "$@"
