#!/usr/bin/env bash
# GSC Task Shell Wrapper - Detect pwsh and forward arguments

if command -v pwsh >/dev/null 2>&1; then
  pwsh -NoLogo -NoProfile -ExecutionPolicy Bypass -File "$(dirname "$0")/gsc-task.ps1" "$@"
else
  echo "PowerShell Core (pwsh) is required to run this command" >&2
  exit 1
fi
