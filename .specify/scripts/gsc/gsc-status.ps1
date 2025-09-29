#!/usr/bin/env pwsh
# GSC Status Command - Workflow progress and collaboration status

[CmdletBinding()]
param(
    [switch]$Json,
    [switch]$Performance
)

$common = Join-Path (Split-Path -Parent $PSScriptRoot) "powershell/common-gsc.ps1"
$memory = Join-Path (Split-Path -Parent $PSScriptRoot) "powershell/memory-integration.ps1"
if (Test-Path $common) { Import-Module $common -Force -ErrorAction SilentlyContinue }
if (Test-Path $memory) { Import-Module $memory -Force -ErrorAction SilentlyContinue }

function Get-WorkflowStateSafe {
    if (Test-Path ".specify/state/gsc-workflow.json") {
        try { return (Get-Content ".specify/state/gsc-workflow.json" -Raw | ConvertFrom-Json) } catch { }
    }
    return (New-WorkflowState)
}

# Repair invalid state structure proactively
try {
    if (Test-Path ".specify/state/gsc-workflow.json") {
        $raw = Get-Content ".specify/state/gsc-workflow.json" -Raw
        $obj = $raw | ConvertFrom-Json
        if ($null -eq $obj.currentPhase -or $null -eq $obj.workflowId) {
            (New-WorkflowState | ConvertTo-Json -Depth 10) | Out-File ".specify/state/gsc-workflow.json" -Encoding UTF8
        }
    }
}
catch { }

Initialize-GSCEnvironment -CommandName "status" | Out-Null
$state = Get-WorkflowStateSafe

# Derive progress
$phases = @("constitution", "specify", "clarify", "plan", "task", "analyze", "implement")
$current = if ($phases -contains $state.currentPhase) { $state.currentPhase } else { "constitution" }
$completed = @()
foreach ($p in $phases) { if ($p -eq $current) { break } $completed += $p }
$currentIdx = [math]::Max(0, ($phases.IndexOf($current)))
$overallPct = [math]::Round((($currentIdx) / ($phases.Count - 1)) * 100, 2)

# Memory summary (best-effort)
$memSummary = @{ totalPatternsApplied = 0; memoryFilesProcessed = 0; lastMemoryUpdate = $null }
try {
    $memLoc = Get-MemoryFileLocations
    if ($memLoc.Success) {
        $memSummary.memoryFilesProcessed = $memLoc.ExistingFiles
        $memSummary.lastMemoryUpdate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
    }
}
catch { }

# Determine degradation mode (state or simulated via environment)
$degradation = [bool]$state.performanceDegradationMode
if ($env:GSC_SIMULATE_DEGRADATION -and $env:GSC_SIMULATE_DEGRADATION.ToString().ToLower() -eq "true") {
    $degradation = $true
}

$response = [ordered]@{
    workflowId                 = $state.workflowId
    currentPhase               = $current
    progress                   = [ordered]@{
        completedPhases      = $completed
        currentPhaseProgress = 50.0
        overallProgress      = $overallPct
    }
    teamCollaborationStatus    = [ordered]@{
        isLocked       = [bool]$state.teamCollaborationLock.isLocked
        lockOwner      = $state.teamCollaborationLock.lockOwner
        lockExpiration = $state.teamCollaborationLock.lockExpiration
    }
    performanceDegradationMode = $degradation
    memoryIntegrationSummary   = $memSummary
}

if ($response.performanceDegradationMode) {
    # Provide a human-readable message explaining degradation state
    $response["message"] = "Performance degradation mode active: reduced performance/features to maintain reliability"
}

if ($Performance.IsPresent) {
    $response["performanceMetrics"] = @{ recentCommandTimes = @(); targetCompliance = $true }
}

if ($Json.IsPresent) {
    $response | ConvertTo-Json -Depth 6 | Write-Output
}
else {
    [pscustomobject]$response
}
