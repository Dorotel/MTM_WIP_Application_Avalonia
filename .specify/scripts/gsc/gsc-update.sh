#!/bin/bash
# GSC Update Command - Cross-Platform Shell Wrapper
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../.." && pwd)"

if command -v pwsh >/dev/null 2>&1; then
  PWSH="pwsh"
elif command -v powershell >/dev/null 2>&1; then
  PWSH="powershell"
else
  echo "PowerShell 7+ is required." >&2
  exit 1
fi

"$PWSH" -NoProfile -ExecutionPolicy Bypass -File "$SCRIPT_DIR/gsc-update.ps1" "$@"
