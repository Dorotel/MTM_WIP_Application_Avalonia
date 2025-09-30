#!/usr/bin/env pwsh
# GSC Help Command - Interactive help summary with memory-aware guidance

[CmdletBinding()]
param(
    [switch]$Json,
    [string]$Command = ''
)

$scriptRoot = Split-Path -Parent $PSScriptRoot
$commonModule = Join-Path $scriptRoot "powershell\common-gsc.ps1"
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

function Get-CommandList {
    $cmds = @("constitution","specify","clarify","plan","task","analyze","implement","memory","validate","status","update","rollback","help")
    return $cmds
}

function Build-HelpEntry {
    param([string]$Name, [hashtable]$Patterns)
    $desc = switch ($Name) {
        'constitution' { 'Validate constitutional compliance with memory integration' }
        'specify'      { 'Create feature specification using Avalonia UI patterns' }
        'clarify'      { 'Clarify requirements with debugging workflows' }
        'plan'         { 'Generate technical plan with universal patterns' }
        'task'         { 'Create tasks.md following MTM patterns' }
        'analyze'      { 'Analyze repository and specs with systematic debugging' }
        'implement'    { 'Execute implementation with all memory patterns' }
        'memory'       { 'Manage memory files (GET/POST) and integrity' }
        'validate'     { 'Run validation quality gates and checks' }
        'status'       { 'Show workflow progress and performance' }
        'update'       { 'Safely update spec files (backups, locks, optional validation)' }
        'rollback'     { 'Reset workflow to safe checkpoint or full reset' }
        'help'         { 'Show this help with memory-aware guidance' }
        default        { 'GSC command' }
    }
    $related = @()
    if ($Patterns -and $Patterns.Success) {
        $related = @($Patterns.Patterns | Select-Object -First 5 | ForEach-Object { $_.Title })
    }
    return [ordered]@{ name = $Name; description = $desc; relatedPatterns = $related }
}

try {
    $envInfo = Initialize-GSCEnvironment -CommandName "help"
    if (-not $envInfo.Success) { throw "Failed to initialize GSC environment: $($envInfo.Error)" }

    $cmds = Get-CommandList
    $patterns = Get-RelevantMemoryPatterns -CommandName "help" -MaxReadTime 4.0
    $entries = @()
    foreach ($c in $cmds) {
        if ([string]::IsNullOrWhiteSpace($Command) -or $Command -eq $c) {
            $entries += (Build-HelpEntry -Name $c -Patterns $patterns)
        }
    }

    $msg = if ([string]::IsNullOrWhiteSpace($Command)) { "GSC Help - available commands" } else { "GSC Help - $Command" }
    $result = Complete-GSCExecution -Environment $envInfo -Success $true -Message $msg -MemoryPatternsApplied @()
    $result.help = [ordered]@{
        commands      = $entries
        totalCommands = $entries.Count
        memoryContext = @{ totalPatterns = ($patterns.TotalPatterns ?? 0); filesProcessed = ($patterns.FilesProcessed ?? 0) }
    }
    if ($Json) { $result | ConvertTo-Json -Depth 10 | Write-Output } else { $result | Write-Output }
    exit 0
}
catch {
    $fail = [ordered]@{ Success = $false; Command = 'help'; Message = "Help failed: $($_.Exception.Message)" }
    [pscustomobject]$fail | Write-Output
    exit 1
}
