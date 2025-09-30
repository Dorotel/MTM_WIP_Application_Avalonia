#!/usr/bin/env pwsh
# GSC Rollback Command - Reset workflow to a safe checkpoint while preserving memory

[CmdletBinding()]
param(
    [switch]$Json,
    [switch]$FullReset
)

# Module imports
$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

function Reset-WorkflowState {
    param(
        [pscustomobject]$State,
        [switch]$Full
    )

    if ($Full) {
        # Full reset back to the beginning while preserving workflowId
        $workflowId = $State.workflowId
        $new = New-WorkflowState
        $new.workflowId = $workflowId
        return $new
    }

    # Roll back to the beginning of the current phase
    $current = $State.currentPhase
    if ([string]::IsNullOrWhiteSpace($current) -or $current -eq 'not_started') {
        return $State
    }

    # Remove the current phase from history and reset last result
    if ($State.PSObject.Properties.Name -contains 'phaseHistory' -and $State.phaseHistory) {
        $State.phaseHistory = @($State.phaseHistory | Where-Object { $_ -ne $current })
    }
    $State.lastExecutionResult = $null
    # Restore checkpoint data if available
    if ($State.PSObject.Properties.Name -contains 'checkpointData' -and $State.checkpointData) {
        $props = $State.checkpointData.PSObject.Properties
        if ($props.Name -contains $current) {
            $checkpoint = $State.checkpointData.$current
            foreach ($prop in $checkpoint.PSObject.Properties) {
                $name = $prop.Name
                $value = $prop.Value
                $State | Add-Member -NotePropertyName $name -NotePropertyValue $value -Force
            }
        }
    }
    return $State
}

try {
    $envInfo = Initialize-GSCEnvironment -CommandName "rollback"
    if (-not $envInfo.Success) { throw "Failed to initialize GSC environment: $($envInfo.Error)" }

    # Load a quick summary of memory files to include in the response
    $memSummary = @{ totalPatterns = 0; filesProcessed = 0 }
    $mr = Get-RelevantMemoryPatterns -CommandName "rollback" -MaxReadTime 3.0
    if ($mr.Success) { $memSummary.totalPatterns = $mr.TotalPatterns; $memSummary.filesProcessed = $mr.FilesProcessed }

    $beforePhase = $envInfo.WorkflowState.currentPhase
    $envInfo.WorkflowState = Reset-WorkflowState -State $envInfo.WorkflowState -Full:$FullReset

    # If full reset, set phase to constitution, otherwise keep current but at start
    if ($FullReset) {
        $envInfo.WorkflowState.currentPhase = "constitution"
        $envInfo.WorkflowState.phaseHistory = @()
    }

    $afterPhase = $envInfo.WorkflowState.currentPhase

    $msg = if ($FullReset) {
        "Workflow fully reset to beginning (constitution). Memory preserved."
    }
    else {
        "Workflow rolled back to the beginning of phase '$afterPhase'. Memory preserved."
    }

    $result = Complete-GSCExecution -Environment $envInfo -Success $true -Message $msg -MemoryPatternsApplied @()
    $result.rollback = [ordered]@{
        beforePhase   = $beforePhase
        afterPhase    = $afterPhase
        fullReset     = [bool]$FullReset
        memorySummary = $memSummary
        timestamp     = (Get-Date).ToString("o")
    }

    if ($Json) { $result | ConvertTo-Json -Depth 10 | Write-Output } else { $result | Write-Output }
    exit 0
}
catch {
    $fail = [ordered]@{
        Success   = $false
        Command   = "rollback"
        Message   = "Rollback failed: $($_.Exception.Message)"
        timestamp = (Get-Date).ToString("o")
    }
    [pscustomobject]$fail | Write-Output
    exit 1
}
