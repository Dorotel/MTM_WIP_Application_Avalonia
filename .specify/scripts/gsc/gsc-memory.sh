#!/bin/bash
# GSC Memory Shell Wrapper
# Purpose: Cross-platform wrapper for gsc-memory.ps1

set -euo pipefail

SCRIPT_NAME="gsc-memory"
POWERSHELL_SCRIPT="gsc-memory.ps1"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PS_SCRIPT_PATH="${SCRIPT_DIR}/${POWERSHELL_SCRIPT}"

print_info() { echo -e "\033[36m[GSC-Memory] $1\033[0m"; }
print_err()  { echo -e "\033[31m[GSC-Memory] ERROR: $1\033[0m"; }

detect_pwsh() {
  if command -v pwsh >/dev/null 2>&1; then echo pwsh; return 0; fi
  if command -v powershell >/dev/null 2>&1; then echo powershell; return 0; fi
  print_err "PowerShell 7+ not found. Install pwsh to continue."; exit 1
}

transform_args() {
  local out=()
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --update) out+=("-Action" "update"); shift; out+=("-MemoryFileType" "${1:-}");;
      --pattern) out+=("-Pattern" "${2:-}"); shift;;
      --help|-h) out+=("-Action" "help");;
      *) out+=("$1");;
    esac
    shift || true
  done
  printf '%s\n' "${out[@]}"
}

main() {
  local pwsh_cmd; pwsh_cmd="$(detect_pwsh)"
  local args; IFS=$'\n' read -r -d '' -a args < <(transform_args "$@" && printf '\0')
  "${pwsh_cmd}" -NoProfile -ExecutionPolicy Bypass -File "${PS_SCRIPT_PATH}" ${args[@]:-}
}

main "$@"
