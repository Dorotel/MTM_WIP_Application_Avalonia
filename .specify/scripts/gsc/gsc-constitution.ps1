#!/usr/bin/env pwsh
# GSC Constitution Command - Enhanced with Memory Integration
# Date: September 28, 2025
# Purpose: Constitution management with memory pattern integration and manufacturing-grade reliability

<#
.SYNOPSIS
GSC Constitution command - Display and manage project constitution with memory integration

.DESCRIPTION
Enhanced constitution command that displays project constitution with integrated
memory pattern processing and manufacturing domain context.

Key Features:
- Memory pattern integration from constitution.md analysis
- Manufacturing-grade error handling and validation
- Cross-platform compatibility (Windows, macOS, Linux)
- TDD-compliant implementation with comprehensive testing
- 24/7 operations support with graceful degradation

.PARAMETER Action
The action to perform (display, validate, update, memory-sync)

.PARAMETER Format
Output format (console, markdown, json, interactive)

.PARAMETER MemoryIntegration
Enable memory pattern integration (default: true)

.PARAMETER Validate
Validate constitution against current project state

.EXAMPLE
gsc-constitution -Action "display" -Format "console"
Displays constitution in console format

.EXAMPLE
gsc-constitution -Action "memory-sync" -MemoryIntegration $true
Syncs constitution with memory patterns

.NOTES
This command integrates with:
- Memory pattern files (universal patterns, debugging workflows)
- Manufacturing domain context (24/7 operations, team collaboration)
- Cross-platform PowerShell Core 7.0+ execution
- TDD validation framework
#>

[CmdletBinding(PositionalBinding=$true)]
param(
    [string] $Action = 'display',

    [ValidateSet('console', 'markdown', 'json', 'interactive', 'copilot-chat')]
    [string] $Format = 'console',

    [bool] $MemoryIntegration = $true,

    [bool] $Validate = $false,

    [string] $OutputPath = "",

    [hashtable] $Context = @{}
)

# Support positional argument fallback used by integration tests
if ($args -and $args.Count -gt 0) {
    # If the first arg is not a known action, treat it as description/context for display
    $known = @('display','validate','update','memory-sync','interactive')
    if ($known -notcontains $args[0]) {
        # Put description into Context for downstream formatting
        if (-not $Context) { $Context = @{} }
        $Context.Description = ($args -join ' ')
        $Action = 'display'
    }
}

# Import required modules
$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
$factoryModule = Join-Path $scriptRoot "powershell\gsc-entity-factory.ps1"

if (Test-Path $commonModule) { . $commonModule }
if (Test-Path $memoryModule) { . $memoryModule }
if (Test-Path $factoryModule) { . $factoryModule }

# GSC Constitution Command Implementation
function Invoke-GSCConstitution {
    [CmdletBinding()]
    param(
        [string] $Action,
        [string] $Format,
        [bool] $MemoryIntegration,
        [bool] $Validate,
        [string] $OutputPath,
        [hashtable] $Context
    )

    try {
        Write-Host "[GSC-Constitution] Starting constitution command - Action: $Action, Format: $Format" -ForegroundColor Cyan

        # Create GSC command entity for tracking
        if (Get-Command "New-GSCCommand" -ErrorAction SilentlyContinue) {
            $gscCommand = New-GSCCommand -Command "constitution"
            $gscCommand.Parameters = @{
                'Action' = $Action
                'Format' = $Format
                'MemoryIntegration' = $MemoryIntegration
                'Validate' = $Validate
            }
            $gscCommand.Start()
        }

        # Find constitution file
        $workspaceRoot = Find-WorkspaceRoot
        $constitutionPath = Join-Path $workspaceRoot "constitution.md"

        if (-not (Test-Path $constitutionPath)) {
            $constitutionPath = Join-Path $workspaceRoot ".github\constitution.md"
            if (-not (Test-Path $constitutionPath)) {
                throw [FileNotFoundException]::new("Constitution file not found in workspace root or .github folder")
            }
        }

        Write-Verbose "[GSC-Constitution] Using constitution file: $constitutionPath"

        # Process based on action
        $result = switch ($Action.ToLower()) {
            'display' {
                Invoke-ConstitutionDisplay -ConstitutionPath $constitutionPath -Format $Format -MemoryIntegration $MemoryIntegration
            }
            'validate' {
                Invoke-ConstitutionValidation -ConstitutionPath $constitutionPath -Context $Context
            }
            'update' {
                Invoke-ConstitutionUpdate -ConstitutionPath $constitutionPath -Context $Context
            }
            'memory-sync' {
                Invoke-ConstitutionMemorySync -ConstitutionPath $constitutionPath -Context $Context
            }
            'interactive' {
                Invoke-ConstitutionInteractive -ConstitutionPath $constitutionPath -Context $Context
            }
            default {
                throw [ArgumentException]::new("Unknown action: $Action")
            }
        }

        # Handle output
        if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
            $result.Content | Out-File -FilePath $OutputPath -Encoding UTF8
            Write-Host "[GSC-Constitution] Output saved to: $OutputPath" -ForegroundColor Green
        }

        # Complete command tracking
        if ($null -ne $gscCommand) {
            $gscCommand.Complete($result.Success, $result.Summary, $result.Error)
        }

        Write-Host "[GSC-Constitution] Constitution command completed successfully" -ForegroundColor Green
        return $result

    } catch {
        $errorMsg = "[GSC-Constitution] Command failed: $($_.Exception.Message)"
        Write-Error $errorMsg

        # Complete command with error
        if ($null -ne $gscCommand) {
            $gscCommand.Complete($false, "", $_.Exception.Message)
        }

        throw
    }
}

# Display constitution with memory integration
function Invoke-ConstitutionDisplay {
    [CmdletBinding()]
    param(
        [string] $ConstitutionPath,
        [string] $Format,
        [bool] $MemoryIntegration
    )

    try {
        Write-Verbose "[GSC-Constitution] Reading constitution file: $ConstitutionPath"
        $constitutionContent = Get-Content -Path $ConstitutionPath -Raw -Encoding UTF8

        # Process memory integration if enabled
        $memoryPatterns = @()
        if ($MemoryIntegration -and (Get-Command "Get-MemoryPatterns" -ErrorAction SilentlyContinue)) {
            Write-Host "[GSC-Constitution] Integrating memory patterns..." -ForegroundColor Yellow
            $memoryPatterns = Get-MemoryPatterns -Domain "constitution" -Type "universal"
        }

        # Format output based on requested format
        $formattedOutput = switch ($Format.ToLower()) {
            'console' {
                Format-ConstitutionConsole -Content $constitutionContent -MemoryPatterns $memoryPatterns
            }
            'markdown' {
                Format-ConstitutionMarkdown -Content $constitutionContent -MemoryPatterns $memoryPatterns
            }
            'json' {
                Format-ConstitutionJson -Content $constitutionContent -MemoryPatterns $memoryPatterns
            }
            'interactive' {
                Format-ConstitutionInteractive -Content $constitutionContent -MemoryPatterns $memoryPatterns
            }
            'copilot-chat' {
                Format-ConstitutionCopilotChat -Content $constitutionContent -MemoryPatterns $memoryPatterns
            }
            default {
                Format-ConstitutionConsole -Content $constitutionContent -MemoryPatterns $memoryPatterns
            }
        }

        return @{
            Success = $true
            Content = $formattedOutput
            Summary = "Constitution displayed in $Format format with $($memoryPatterns.Count) memory patterns"
            Error = ""
            MemoryPatterns = $memoryPatterns.Count
        }

    } catch {
        return @{
            Success = $false
            Content = ""
            Summary = ""
            Error = $_.Exception.Message
            MemoryPatterns = 0
        }
    }
}

# Validate constitution against current project state
function Invoke-ConstitutionValidation {
    [CmdletBinding()]
    param(
        [string] $ConstitutionPath,
        [hashtable] $Context
    )

    try {
        Write-Host "[GSC-Constitution] Validating constitution against project state..." -ForegroundColor Yellow

        $validationResults = @{
            'FileExists' = Test-Path $ConstitutionPath
            'FileReadable' = $false
            'ContentValid' = $false
            'MemoryPatternConsistency' = $false
            'ManufacturingCompliance' = $false
        }

        if ($validationResults.FileExists) {
            try {
                $content = Get-Content -Path $ConstitutionPath -Raw -Encoding UTF8
                $validationResults.FileReadable = $content.Length -gt 0
                $validationResults.ContentValid = $content -match "# Constitution" -or $content -match "constitution"
            } catch {
                $validationResults.FileReadable = $false
            }
        }

        # Validate memory pattern consistency
        if (Get-Command "Test-MemoryPatternConsistency" -ErrorAction SilentlyContinue) {
            $validationResults.MemoryPatternConsistency = Test-MemoryPatternConsistency -ConstitutionPath $ConstitutionPath
        }

        # Validate manufacturing domain compliance
        $validationResults.ManufacturingCompliance = Test-ManufacturingCompliance -ConstitutionPath $ConstitutionPath

        $overallValid = $validationResults.Values | Where-Object { $_ -eq $false } | Measure-Object | Select-Object -ExpandProperty Count
        $isValid = $overallValid -eq 0

        $summary = if ($isValid) {
            "Constitution validation passed all checks"
        } else {
            "Constitution validation failed $overallValid checks"
        }

        return @{
            Success = $isValid
            Content = $validationResults | ConvertTo-Json -Depth 3
            Summary = $summary
            Error = if (-not $isValid) { "Validation failures detected" } else { "" }
            ValidationResults = $validationResults
        }

    } catch {
        return @{
            Success = $false
            Content = ""
            Summary = "Constitution validation failed with error"
            Error = $_.Exception.Message
            ValidationResults = @{}
        }
    }
}

# Memory sync for constitution
function Invoke-ConstitutionMemorySync {
    [CmdletBinding()]
    param(
        [string] $ConstitutionPath,
        [hashtable] $Context
    )

    try {
        Write-Host "[GSC-Constitution] Syncing constitution with memory patterns..." -ForegroundColor Yellow

        $syncResults = @{
            'UniversalPatterns' = 0
            'DebuggingPatterns' = 0
            'ManufacturingPatterns' = 0
            'ConflictsResolved' = 0
        }

        # Sync with universal patterns
        if (Get-Command "Sync-MemoryPatterns" -ErrorAction SilentlyContinue) {
            $universalSync = Sync-MemoryPatterns -Source $ConstitutionPath -Type "universal"
            $syncResults.UniversalPatterns = $universalSync.PatternsProcessed

            $debuggingSync = Sync-MemoryPatterns -Source $ConstitutionPath -Type "debugging"
            $syncResults.DebuggingPatterns = $debuggingSync.PatternsProcessed

            $manufacturingSync = Sync-MemoryPatterns -Source $ConstitutionPath -Type "manufacturing"
            $syncResults.ManufacturingPatterns = $manufacturingSync.PatternsProcessed

            $syncResults.ConflictsResolved = $universalSync.ConflictsResolved + $debuggingSync.ConflictsResolved + $manufacturingSync.ConflictsResolved
        }

        $totalPatterns = $syncResults.UniversalPatterns + $syncResults.DebuggingPatterns + $syncResults.ManufacturingPatterns

        return @{
            Success = $true
            Content = $syncResults | ConvertTo-Json -Depth 3
            Summary = "Constitution synced with $totalPatterns memory patterns, $($syncResults.ConflictsResolved) conflicts resolved"
            Error = ""
            SyncResults = $syncResults
        }

    } catch {
        return @{
            Success = $false
            Content = ""
            Summary = "Constitution memory sync failed"
            Error = $_.Exception.Message
            SyncResults = @{}
        }
    }
}

# Console formatting
function Format-ConstitutionConsole {
    [CmdletBinding()]
    param(
        [string] $Content,
        [array] $MemoryPatterns
    )

    $output = @()
    $output += "=" * 80
    $output += "PROJECT CONSTITUTION"
    $output += "=" * 80
    $output += ""
    $output += $Content

    if ($MemoryPatterns.Count -gt 0) {
        $output += ""
        $output += "-" * 80
        $output += "INTEGRATED MEMORY PATTERNS ($($MemoryPatterns.Count) patterns)"
        $output += "-" * 80
        foreach ($pattern in $MemoryPatterns) {
            $output += "• $($pattern.Title): $($pattern.Description)"
        }
    }

    $output += ""
    $output += "=" * 80
    $output += "Generated by GSC Constitution Command - $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    $output += "=" * 80

    return $output -join "`n"
}

# Manufacturing compliance check
function Test-ManufacturingCompliance {
    [CmdletBinding()]
    param([string] $ConstitutionPath)

    try {
        $content = Get-Content -Path $ConstitutionPath -Raw -Encoding UTF8

        # Check for manufacturing domain requirements
        $manufacturingKeywords = @(
            '24/7', 'operations', 'reliability', 'manufacturing',
            'team', 'collaboration', 'handoff', 'shift'
        )

        $foundKeywords = $manufacturingKeywords | Where-Object { $content -match $_ }
        return $foundKeywords.Count -ge 3  # At least 3 manufacturing concepts

    } catch {
        return $false
    }
}

# Helper function to find workspace root
function Find-WorkspaceRoot {
    $currentPath = Get-Location
    $searchPath = $currentPath

    # Look for common workspace indicators
    $indicators = @('.git', '.specify', 'constitution.md', '*.sln', '*.csproj')

    while ($searchPath -ne [System.IO.Path]::GetPathRoot($searchPath)) {
        foreach ($indicator in $indicators) {
            $testPath = Join-Path $searchPath $indicator
            if (Test-Path $testPath) {
                return $searchPath
            }
        }
        $searchPath = Split-Path $searchPath -Parent
    }

    # Fallback to current directory
    return $currentPath
}

# Main execution
try {
    $result = Invoke-GSCConstitution -Action $Action -Format $Format -MemoryIntegration $MemoryIntegration -Validate $Validate -OutputPath $OutputPath -Context $Context

    # Display result
    if ($result.Success) {
        # Output the full result object for programmatic consumption
        [pscustomobject]$result | Write-Output
        if ($Validate -and $result.ValidationResults) {
            Write-Host "`n[GSC-Constitution] Validation Results:" -ForegroundColor Cyan
            $result.ValidationResults.GetEnumerator() | ForEach-Object {
                $status = if ($_.Value) { "✅ PASS" } else { "❌ FAIL" }
                Write-Host "  $($_.Key): $status" -ForegroundColor $(if ($_.Value) { "Green" } else { "Red" })
            }
        }
    } else {
        Write-Error "[GSC-Constitution] Command failed: $($result.Error)"
        exit 1
    }

} catch {
    Write-Error "[GSC-Constitution] Fatal error: $($_.Exception.Message)"
    exit 1
}

Write-Host "[GSC-Constitution] Constitution command completed successfully" -ForegroundColor Green
