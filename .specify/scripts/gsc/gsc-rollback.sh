#!/usr/bin/env bash
pwsh -NoProfile -File "$(dirname "$0")/gsc-rollback.ps1" "$@"
