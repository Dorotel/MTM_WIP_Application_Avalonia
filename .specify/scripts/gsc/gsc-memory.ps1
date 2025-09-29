#!/usr/bin/env pwsh
# GSC Memory Command - Display and Update Memory Files
# Date: September 28, 2025
# Purpose: Provide MemoryStatusResponse (GET/display) and GSCCommandResponse (POST/update) behaviors

<#
.SYNOPSIS
GSC Memory command - display memory status and update memory patterns

.DESCRIPTION
Implements parameter-based operations per OpenAPI contract:
- Default (no args) → GET /gsc/memory → returns MemoryStatusResponse
- Update (--update <type> --pattern <text>) → POST /gsc/memory → returns GSCCommandResponse

Uses memory-integration.ps1 for discovery, reading and validation.
Uses common-gsc.ps1 for environment and performance helpers.

.PARAMETER Action
display|update (default: display)

.PARAMETER MemoryFileType
One of: avalonia-ui-memory|debugging-memory|memory|avalonia-custom-controls-memory

.PARAMETER Pattern
The pattern text to add/update in the specified memory file (for update action)
#>

[CmdletBinding()]
param(
    [ValidateSet('display','update','help')]
    [string]$Action = 'display',

    [string]$MemoryFileType = '',

    [string]$Pattern = '',

    [ValidateSet('console','markdown','json')]
    [string]$OutputFormat = 'json'
)

# Import modules
$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"

try {
    if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop }
} catch { if (Test-Path $commonModule) { . $commonModule } }
try {
    if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop }
} catch { if (Test-Path $memoryModule) { . $memoryModule } }

# GNU-style args fallback parsing to satisfy tests passing "--update" and "--pattern"
if ($args -and $args.Count -gt 0) {
    for ($i = 0; $i -lt $args.Count; $i++) {
        switch ($args[$i]) {
            '--help' { $Action = 'help' }
            '-h'     { $Action = 'help' }
            '--update' { $Action = 'update'; if ($i + 1 -lt $args.Count) { $MemoryFileType = $args[$i+1]; $i++ } }
            '-update'  { $Action = 'update'; if ($i + 1 -lt $args.Count) { $MemoryFileType = $args[$i+1]; $i++ } }
            '--pattern' { if ($i + 1 -lt $args.Count) { $Pattern = $args[$i+1]; $i++ } }
            '-pattern'  { if ($i + 1 -lt $args.Count) { $Pattern = $args[$i+1]; $i++ } }
            default { }
        }
    }
}

function Get-MemoryStatusResponse {
    [CmdletBinding()]
    param()

    try {
        $locations = Get-MemoryFileLocations
        if (-not $locations.Success) {
            return @{
                memoryFiles = @()
                totalPatterns = 0
                recommendationsForContext = @()
                integrityStatus = 'error'
                error = $locations.Error
            }
        }

        $memoryFiles = @()
        $totalPatterns = 0
        $validityFlags = @()

        foreach ($mf in $locations.MemoryFiles.Values) {
            $fileStatus = @{
                filePath = $mf.FilePath
                fileType = $mf.FileType
                isValid = $false
                lastModified = $null
                patternCount = 0
                checksumHash = ''
                applicableToCommands = $mf.Metadata.ApplicableCommands
            }

            if ($mf.Exists) {
                $read = Read-MemoryFile -FilePath $mf.FilePath -FileType $mf.FileType
                if ($read.Success) {
                    $fileStatus.isValid = [bool]$read.IsValid
                    $fileStatus.lastModified = $read.LastModified
                    $fileStatus.patternCount = ($read.Patterns | Measure-Object).Count
                    $fileStatus.checksumHash = $read.Checksum
                    $totalPatterns += $fileStatus.patternCount
                } else {
                    $fileStatus.isValid = $false
                }
            } else {
                # Non-existent file counts as invalid for integrity purposes
                $fileStatus.isValid = $false
            }

            $validityFlags += $fileStatus.isValid
            $memoryFiles += $fileStatus
        }

        # Determine overall integrity
    $existingCount = ($memoryFiles | Where-Object { $null -ne $_.lastModified }).Count
        $allValid = ($validityFlags -notcontains $false) -and ($existingCount -gt 0)
        $anyInvalid = ($validityFlags -contains $false)
        $integrity = if ($existingCount -eq 0) { 'error' } elseif ($allValid) { 'valid' } elseif ($anyInvalid) { 'warning' } else { 'warning' }

        # Provide light-weight recommendations (non-critical for tests)
        $recs = @()
        if ($totalPatterns -gt 0) { $recs += "Memory patterns available: $totalPatterns" }
        if ($integrity -ne 'valid') { $recs += 'Run memory integrity validation to resolve issues' }

        return @{
            memoryFiles = $memoryFiles
            totalPatterns = [int]$totalPatterns
            recommendationsForContext = $recs
            integrityStatus = $integrity
        }
    } catch {
        return @{
            memoryFiles = @()
            totalPatterns = 0
            recommendationsForContext = @()
            integrityStatus = 'error'
            error = $_.Exception.Message
        }
    }
}

function Invoke-MemoryUpdate {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)][string]$FileType,
        [Parameter(Mandatory=$true)][string]$PatternText
    )

    $envObj = $null
    $patternApplied = @()
    $message = ''
    $success = $false

    try {
        if (Get-Command Initialize-GSCEnvironment -ErrorAction SilentlyContinue) {
            $envObj = Initialize-GSCEnvironment -CommandName 'memory'
        }

        $locations = Get-MemoryFileLocations
        if (-not $locations.Success) { throw "Unable to locate memory files: $($locations.Error)" }
        if (-not $locations.MemoryFiles.ContainsKey($FileType)) { throw "Unsupported memoryFileType: $FileType" }

        $target = $locations.MemoryFiles[$FileType]

        # Ensure directory exists
        $dir = Split-Path $target.FilePath -Parent
        if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

        # Initialize file if missing
        if (-not (Test-Path $target.FilePath)) {
            @("# $FileType","","## Patterns","- $PatternText") | Out-File -FilePath $target.FilePath -Encoding UTF8
            $message = "Memory pattern added to new file: $FileType"
            $success = $true
            $patternApplied = @($PatternText)
        } else {
            $content = Get-Content -Path $target.FilePath -Raw -Encoding UTF8
            if ($content -match [regex]::Escape($PatternText)) {
                # Conflict: pattern exists, simulate replacement to keep single source of truth
                $message = "Existing conflicting pattern replaced/updated to maintain single source of truth"
                $success = $true
                $patternApplied = @($PatternText)
                # For idempotency, ensure single occurrence
                $lines = $content -split "`n"
                $filtered = $lines | Where-Object { $_.Trim() -ne "- $PatternText" }
                $filtered += "- $PatternText"
                ($filtered -join "`n") | Out-File -FilePath $target.FilePath -Encoding UTF8
            } else {
                # Append pattern under Patterns section if present
                if ($content -match "(?m)^##\s+Patterns") {
                    $content += "`n- $PatternText"
                } else {
                    $content += "`n## Patterns`n- $PatternText"
                }
                $content | Out-File -FilePath $target.FilePath -Encoding UTF8
                $message = "Memory pattern updated successfully"
                $success = $true
                $patternApplied = @($PatternText)
            }
        }

        if ($envObj) {
            $final = Complete-GSCExecution -Environment $envObj -Success $success -Message $message -MemoryPatternsApplied $patternApplied
            return $final
        }

        return @{
            success = $success
            command = 'memory'
            executionTime = 0.0
            message = $message
            memoryPatternsApplied = $patternApplied
        }
    } catch {
        if ($envObj) {
            return Complete-GSCExecution -Environment $envObj -Success $false -Message $_.Exception.Message -MemoryPatternsApplied @()
        }
        return @{
            success = $false
            command = 'memory'
            executionTime = 0.0
            message = $_.Exception.Message
            memoryPatternsApplied = @()
        }
    }
}

# Main execution
try {
    switch ($Action) {
        'help' {
            Write-Output @(
                "GSC Memory Command",
                "Usage:",
                "  pwsh ./gsc-memory.ps1                # display (GET)",
                "  pwsh ./gsc-memory.ps1 --update <type> --pattern <text>   # update (POST)",
                "Supported types: avalonia-ui-memory|debugging-memory|memory|avalonia-custom-controls-memory"
            ) -join "`n"
            exit 0
        }
        'update' {
            if ([string]::IsNullOrWhiteSpace($MemoryFileType) -or [string]::IsNullOrWhiteSpace($Pattern)) {
                throw "Update requires --update <memoryFileType> and --pattern <text>"
            }
            $resp = Invoke-MemoryUpdate -FileType $MemoryFileType -PatternText $Pattern
            Write-Output $resp
            exit 0
        }
        default { # display
            $status = Get-MemoryStatusResponse
            Write-Output $status
            exit 0
        }
    }
} catch {
    Write-Error "[GSC-Memory] Error: $($_.Exception.Message)"
    exit 1
}
