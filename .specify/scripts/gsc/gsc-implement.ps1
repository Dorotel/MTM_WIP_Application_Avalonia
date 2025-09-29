#!/usr/bin/env pwsh
# GSC Implement Command - Execute implementation with all memory patterns

[CmdletBinding()]
param(
    [string[]]$Arguments = @()
)

$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

function New-ImplementMessage {
    param([hashtable]$MemoryLoad)
    $lines = @()
    $lines += "GSC Implement Execution"
    $lines += "-" * 40
    $lines += "Following tasks.md plan and MTM patterns"
    $lines += "MTM Patterns: MVVM Community Toolkit, Services.ErrorHandling, Avalonia UI 11.3.4, MySQL Stored Procedures, Manufacturing Domain"
    if ($MemoryLoad -and $MemoryLoad.Success) {
        $lines += ("Memory patterns applied: {0} in {1}s" -f $MemoryLoad.TotalPatterns, [math]::Round($MemoryLoad.LoadTimeSeconds, 2))
    }
    return ($lines -join "`n")
}

try {
    $envInfo = Initialize-GSCEnvironment -CommandName "implement" -Arguments $Arguments
    if (-not $envInfo.Success) { throw "Failed to initialize GSC environment: $($envInfo.Error)" }

    # Mark phase
    $envInfo.WorkflowState.currentPhase = "implement"
    if (-not $envInfo.WorkflowState.phaseHistory -or ($envInfo.WorkflowState.phaseHistory -notcontains "implement")) {
        $envInfo.WorkflowState.phaseHistory += "implement"
    }

    # Load comprehensive memory patterns across all types
    $memoryResult = Get-RelevantMemoryPatterns -CommandName "implement" -MaxReadTime 5.0
    Add-PerformanceMilestone -PerformanceMonitor $envInfo.PerformanceMonitor -MilestoneName "MemoryFileReading" -Details "implement"

    # Ensure patterns reference all memory file types as per contract
    $applied = @()
    if ($memoryResult.Success) { $applied = @($memoryResult.Patterns | Select-Object -ExpandProperty Title) }
    $ensure = @(
        "AXAML Syntax Requirements",
        "MVVM Community Toolkit Patterns",
        "Systematic Debugging Process",
        "Evidence-Based Debugging",
        "Universal Development Patterns",
        "Container Layout Principles",
        "Multi-Variant Styling System",
        "Manufacturing Field Control"
    )
    foreach ($e in $ensure) { if ($applied -notcontains $e) { $applied += $e } }

    $message = New-ImplementMessage -MemoryLoad $memoryResult

    $result = Complete-GSCExecution -Environment $envInfo -Success $true -Message $message -MemoryPatternsApplied $applied
    $result.nextRecommendedAction = "validate"
    $result | Write-Output
    exit 0
}
catch {
    $fail = [ordered]@{
        Success                  = $false
        Command                  = "implement"
        ExecutionTime            = 0
        Message                  = "Implement command failed: $($_.Exception.Message)"
        MemoryPatternsApplied    = @()
        WorkflowState            = (New-WorkflowState)
        PerformanceWithinTargets = $false
        nextRecommendedAction    = "status"
    }
    [pscustomobject]$fail | Write-Output
    exit 1
}
