#!/usr/bin/env bash
pwsh -NoProfile -File "$(dirname "$0")/gsc-status.ps1" "$@"
