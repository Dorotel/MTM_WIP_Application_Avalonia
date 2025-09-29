#!/bin/bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PS_SCRIPT="${SCRIPT_DIR}/gsc-status.ps1"
if command -v pwsh >/dev/null 2>&1; then PSH=pwsh; else PSH=powershell; fi
"$PSH" -NoProfile -ExecutionPolicy Bypass -File "$PS_SCRIPT" "$@"
