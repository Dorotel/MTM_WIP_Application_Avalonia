#!/usr/bin/env pwsh
# GSC Update Command - Safely update spec sections with backups, lock handling, and validation

[CmdletBinding()]
param(
    [string]$File = "specs/master/spec.md",
    [string]$Section = "",
    [ValidateSet('insert', 'append', 'replace', 'remove', 'suggest', 'help')]
    [string]$Operation = 'suggest',
    [string]$Content = "",
    [string]$ContentPath = "",
    [switch]$ValidateAfter,
    [switch]$Force,
    [switch]$Json
)

# Module imports
$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$lockModule = Join-Path $scriptRoot "powershell\collaboration-lock-manager.ps1"
try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
try { if (Test-Path $lockModule) { Import-Module $lockModule -Force -ErrorAction Stop } } catch { if (Test-Path $lockModule) { . $lockModule } }

function Get-CurrentUserName { return ($env:USER ?? $env:USERNAME ?? 'unknown') }

function Get-WorkflowStateInternal {
    if (Test-Path ".specify/state/gsc-workflow.json") {
        try { return (Get-Content ".specify/state/gsc-workflow.json" -Raw | ConvertFrom-Json) } catch { }
    }
    return (New-WorkflowState)
}

function Test-LockOrBypass {
    param([pscustomobject]$State, [switch]$Force)
    if ($null -ne $State.teamCollaborationLock -and $State.teamCollaborationLock.isLocked) {
        $owner = $State.teamCollaborationLock.lockOwner
        $exp = $State.teamCollaborationLock.lockExpiration
        if (-not $Force) {
            throw "Workflow is locked by '$owner' (expires: $exp). Use -Force to bypass if permitted."
        }
    }
}

function Backup-File {
    param([string]$Path)
    $ts = Get-Date -Format 'yyyyMMdd-HHmmss'
    $dir = ".specify/state/backups"
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
    $name = (Split-Path $Path -Leaf)
    $dest = Join-Path $dir ("$name.$ts.bak")
    Copy-Item -Path $Path -Destination $dest -Force
    return $dest
}

function New-SectionIfMissing {
    param([string[]]$Lines, [string]$Section)
    $h = "## $Section"
    $idx = $Lines | Select-String -Pattern "^##\s+\Q$Section\E\s*$" -SimpleMatch | Select-Object -First 1
    if ($idx) { return [pscustomobject]@{ Exists = $true; Index = $idx.LineNumber - 1 } }
    # Append new section at end
    $newLines = @()
    $newLines += $Lines
    if ($newLines.Count -gt 0 -and $newLines[-1] -ne '') { $newLines += '' }
    $newLines += $h
    $newLines += ''
    return [pscustomobject]@{ Exists = $false; Index = $newLines.Count - 1; Lines = $newLines }
}

function Set-SectionContent {
    param([string[]]$Lines, [string]$Section, [string[]]$InsertLines, [ValidateSet('insert', 'append', 'replace')]$Mode)
    $match = $Lines | Select-String -Pattern "^##\s+\Q$Section\E\s*$" -SimpleMatch | Select-Object -First 1
    if (-not $match) {
        # create section and append content
        $ensure = New-SectionIfMissing -Lines $Lines -Section $Section
        $linesToUse = if ($ensure.Lines) { $ensure.Lines } else { $Lines }
        $startIdx = $ensure.Index + 1
        $prefix = $linesToUse[0..$ensure.Index]
        $suffix = if ($startIdx -le ($linesToUse.Count - 1)) { $linesToUse[$startIdx..($linesToUse.Count - 1)] } else { @() }
        $result = @()
        $result += $prefix
        if ($Mode -in @('insert', 'append', 'replace')) { $result += $InsertLines }
        if ($suffix.Count -gt 0) { $result += $suffix }
        return $result
    }
    $start = $match.LineNumber
    # find next section or end
    $next = ($Lines | Select-String -Pattern '^##\s+' | Where-Object { $_.LineNumber -gt $start } | Select-Object -First 1)
    $endIdx = if ($next) { $next.LineNumber - 2 } else { $Lines.Count - 1 }
    $before = if ($start -gt 1) { $Lines[0..($start - 1)] } else { @() }
    $after = if ($endIdx + 1 -le $Lines.Count - 1) { $Lines[($endIdx + 1)..($Lines.Count - 1)] } else { @() }
    $body = if ($Mode -eq 'replace') { @() } else { $Lines[$start..$endIdx] }
    if ($Mode -eq 'insert' -or $Mode -eq 'append' -or $Mode -eq 'replace') {
        $body += $InsertLines
    }
    $result = @()
    if ($before.Count -gt 0) { $result += $before }
    if ($body.Count -gt 0) { $result += $body }
    if ($after.Count -gt 0) { $result += $after }
    return $result
}

function Get-Suggestions {
    param([string[]]$Lines)
    $sugs = @()
    for ($i = 0; $i -lt $Lines.Count; $i++) {
        $line = $Lines[$i]
        if ($line -match '^#(?!#)') { continue }
        if ($line -match '^\s*\-\s+\[NEEDS CLARIFICATION\]') {
            $sugs += @{ line = $i + 1; suggestion = 'Resolve [NEEDS CLARIFICATION] marker' }
        }
    }
    if (-not ($Lines -match '^##\s+Requirements')) { $sugs += @{ line = ($Lines.Count); suggestion = 'Add "## Requirements" section' } }
    return $sugs
}

try {
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $envInfo = Initialize-GSCEnvironment -CommandName "update"
    if (-not $envInfo.Success) { throw "Failed to initialize GSC environment: $($envInfo.Error)" }

    $state = Get-WorkflowStateInternal
    Test-LockOrBypass -State $state -Force:$Force

    if (-not (Test-Path $File)) { throw "Spec file not found: $File" }
    $original = Get-Content -Path $File -Raw
    $lines = $original -split "`n"

    if ($Operation -eq 'help') {
        $help = @"
# GSC Update Command Help

Usage:
  gsc-update [-File <path>] [-Section <name>] [-Operation insert|append|replace|remove|suggest] [-Content <text>] [-ContentPath <path>] [-ValidateAfter] [-Force] [-Json]

Notes:
  - Creates section automatically if missing (insert/append/replace)
  - Backs up file to .specify/state/backups before modifying
  - Respects collaboration lock; use -Force to bypass if permitted
  - After update, optionally runs validation (gsc-validate)
"@
        $sw.Stop()
        $res = [ordered]@{
            # Contract-compliant fields
            success       = $true
            command       = 'update'
            executionTime = [math]::Round(($sw.Elapsed.TotalSeconds), 3)
            # Back-compat aliases (distinct keys to avoid duplicates)
            SuccessAlias  = $true
            CommandAlias  = 'update'
            Help          = $help
        }
        if ($Json) { $res | ConvertTo-Json -Depth 5 | Write-Output } else { $res | Write-Output }
        exit 0
    }

    $backup = Backup-File -Path $File

    # Load content if path provided
    if ($ContentPath -and (Test-Path $ContentPath)) { $Content = Get-Content -Path $ContentPath -Raw }
    $insertLines = @()
    if ($Content) { $insertLines = ($Content -split "`n") }

    $updated = $lines
    $changeSummary = @{}

    switch ($Operation) {
        'suggest' {
            $sugs = Get-Suggestions -Lines $lines
            $changeSummary = @{ suggestions = $sugs }
        }
        'remove' {
            if (-not $Section) { throw "-Section is required for remove" }
            $match = $lines | Select-String -Pattern "^##\s+\Q$Section\E\s*$" -SimpleMatch | Select-Object -First 1
            if ($match) {
                $start = $match.LineNumber
                $next = ($lines | Select-String -Pattern '^##\s+' | Where-Object { $_.LineNumber -gt $start } | Select-Object -First 1)
                $endIdx = if ($next) { $next.LineNumber - 2 } else { $lines.Count - 1 }
                $before = if ($start -gt 1) { $lines[0..($start - 2)] } else { @() }
                $after = if ($endIdx + 1 -le $lines.Count - 1) { $lines[($endIdx + 1)..($lines.Count - 1)] } else { @() }
                $updated = @(); if ($before) { $updated += $before }; if ($after) { $updated += $after }
                $changeSummary = @{ removedSection = $Section; removedLines = ($endIdx - $start + 2) }
            }
            else { $changeSummary = @{ removedSection = $Section; removedLines = 0; note = 'section not found' } }
        }
        default {
            # insert|append|replace
            if (-not $Section) { throw "-Section is required for $Operation" }
            if ($insertLines.Count -eq 0 -and $Operation -ne 'replace') { throw "-Content or -ContentPath is required for $Operation" }
            $updated = Set-SectionContent -Lines $lines -Section $Section -InsertLines $insertLines -Mode $Operation
            $changeSummary = @{ operation = $Operation; section = $Section; linesAdded = $insertLines.Count }
        }
    }

    $didWrite = $false
    if ($Operation -ne 'suggest') {
        ($updated -join "`n") | Out-File -FilePath $File -Encoding UTF8
        $didWrite = $true
    }

    # Decision record
    $decisionsDir = "specs/master/decisions"
    if (-not (Test-Path $decisionsDir)) { New-Item -ItemType Directory -Path $decisionsDir | Out-Null }
    $drPath = Join-Path $decisionsDir ("decision-" + (Get-Date -Format 'yyyyMMdd-HHmmss') + "-update.md")
    $dr = @()
    $dr += "### Decision - $(Get-Date -Format 'u')"
    $dr += "**Decision**: Update spec section via gsc-update ($Operation $Section)"
    $dr += "**Context**: Automated update with backup and optional validation"
    $dr += "**Impact**: Keeps spec current; backup created at $backup"
    $dr -join "`n" | Out-File -FilePath $drPath -Encoding UTF8

    $validation = $null
    if ($ValidateAfter) {
        $gscValidate = Join-Path $PSScriptRoot 'gsc-validate.ps1'
        if (Test-Path $gscValidate) {
            try { $validation = (& $gscValidate -Json) | ConvertFrom-Json } catch { $validation = @{ Success = $false; Message = $_.Exception.Message } }
        }
    }

    $sw.Stop()
    $result = [ordered]@{
        # Contract-compliant fields
        success        = $true
        command        = 'update'
        executionTime  = [math]::Round(($sw.Elapsed.TotalSeconds), 3)
        # Extended payload
        operation      = $Operation
        section        = $Section
        file           = $File
        backupPath     = $backup
        wroteChanges   = $didWrite
        changeSummary  = $changeSummary
        decisionRecord = $drPath
        validation     = $validation
        timestamp      = (Get-Date).ToString('o')
    }

    if ($Json) { $result | ConvertTo-Json -Depth 10 | Write-Output } else { $result | Write-Output }
    exit 0
}
catch {
    $fail = [ordered]@{
        # Contract-compliant fields
        success       = $false
        command       = 'update'
        executionTime = 0
        Message       = "Update failed: $($_.Exception.Message)"
        timestamp     = (Get-Date).ToString('o')
    }
    [pscustomobject]$fail | Write-Output
    exit 1
}
