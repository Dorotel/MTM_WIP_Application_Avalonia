#!/usr/bin/env pwsh
# Template: GSC Command (memory-integrated)
# Usage: Copy to .specify/scripts/gsc/gsc-<name>.ps1 and replace $commandName.
# Requirements: PowerShell Core 7+, modules in .specify/scripts/powershell/*.ps1

[CmdletBinding()]
param(
    [string[]]$Arguments = @(),
    [switch]$Json
)

$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

# Replace 'template' with actual command name when using this template
$commandName = "template"

try {
    $envInfo = Initialize-GSCEnvironment -CommandName $commandName -Arguments $Arguments
    if (-not $envInfo.Success) { throw "Failed to initialize GSC environment: $($envInfo.Error)" }

    # Optional: load memory patterns relevant to this command (target <5s)
    $memory = Get-RelevantMemoryPatterns -CommandName $commandName -MaxReadTime 5.0

    # TODO: command-specific logic here
    # Example: Validate inputs, update state, and emit JSON
    # Set-WorkflowPhase -Phase "implement" -StatePath (Join-Path $envInfo.StateRoot 'gsc-workflow.json')
    $message = "${commandName} executed via template"
    $applied = if ($memory.Success) { @($memory.Patterns | Select-Object -First 5 -ExpandProperty Title) } else { @() }

    $result = Complete-GSCExecution -Environment $envInfo -Success $true -Message $message -MemoryPatternsApplied $applied
    if ($Json) { $result | ConvertTo-Json -Depth 10 | Write-Output } else { $result | Write-Output }
    exit 0
}
catch {
    $fail = [ordered]@{ Success=$false; Command=$commandName; Message="${commandName} failed: $($_.Exception.Message)" }
    [pscustomobject]$fail | Write-Output
    exit 1
}
