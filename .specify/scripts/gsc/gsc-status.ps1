#!/usr/bin/env pwsh
# GSC Status Command - Workflow progress and compliance status

[CmdletBinding()]
param(
    [switch]$Json,
    [switch]$Performance
)

# Module imports
$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

function Get-WorkflowStateOrDefault {
    if (Test-Path ".specify/state/gsc-workflow.json") {
        try { return (Get-Content ".specify/state/gsc-workflow.json" -Raw | ConvertFrom-Json) } catch { }
    }
    return (New-WorkflowState)
}

function Get-MemorySummary {
    $filesProcessed = 0
    $patternsApplied = 0
    $lastUpdate = $null
    try {
        $loc = Get-MemoryFileLocations
        if ($loc.Success) {
            foreach ($mf in $loc.MemoryFiles.Values) {
                if ($mf.Exists) {
                    $read = Read-MemoryFile -FilePath $mf.FilePath -FileType $mf.FileType
                    if ($read.Success) {
                        $filesProcessed++
                        $patternsApplied += ($read.Patterns | Measure-Object).Count
                        if ($null -eq $lastUpdate -or $read.LastModified -gt $lastUpdate) { $lastUpdate = $read.LastModified }
                    }
                }
            }
        }
    } catch { }
    return @{
        totalPatternsApplied = [int]$patternsApplied
        memoryFilesProcessed = [int]$filesProcessed
        lastMemoryUpdate     = if ($lastUpdate) { $lastUpdate.ToString("o") } else { $null }
    }
}

function Get-PerformanceMetrics {
    return @{
        recentCommandTimes = @()
        targetCompliance   = $true
    }
}

try {
    $state = Get-WorkflowStateOrDefault

    $phase = if ($state.currentPhase -and $state.currentPhase -ne 'not_started') { $state.currentPhase } else { 'constitution' }
    $completed = @()
    if ($state.phaseHistory) { $completed = @($state.phaseHistory) } else { $completed = @() }

    $progress = @{
        completedPhases       = $completed
        currentPhaseProgress  = [double]0
        overallProgress       = [double](($completed.Count / 7.0) * 100.0)
    }

    $team = @{
        isLocked      = [bool]($state.teamCollaborationLock.isLocked)
        lockOwner     = $state.teamCollaborationLock.lockOwner
        lockExpiration= $state.teamCollaborationLock.lockExpiration
    }

    $response = @{
        workflowId                 = $state.workflowId
        currentPhase               = $phase
        progress                   = $progress
        teamCollaborationStatus    = $team
        performanceDegradationMode = [bool]$state.performanceDegradationMode
        memoryIntegrationSummary   = (Get-MemorySummary)
    }

    if ($Performance) {
        $response.performanceMetrics = Get-PerformanceMetrics
    }

    if ($Json) {
        $response | ConvertTo-Json -Depth 10 | Write-Output
    } else {
        Write-Output $response
    }
    exit 0
} catch {
    Write-Error "[GSC-Status] Error: $($_.Exception.Message)"
    exit 1
}
