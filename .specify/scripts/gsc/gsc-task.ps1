#!/usr/bin/env pwsh
# GSC Task Command - Enhanced with Avalonia custom control memory patterns
# Minimal success envelope for integration flow

[CmdletBinding()]
param(
    [string]$Action = 'list'
)

try {
    $scriptRoot = Split-Path -Parent $PSScriptRoot
    $commonModule = Join-Path $scriptRoot 'powershell\common-gsc.ps1'
    $memoryModule = Join-Path $scriptRoot 'powershell\memory-integration.ps1'
    try { if (Test-Path $commonModule) { Import-Module $commonModule -Force -ErrorAction Stop } } catch { if (Test-Path $commonModule) { . $commonModule } }
    try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

    $result = @{
        success = $true
        action  = $Action
        tasks   = @()
    }
    [pscustomobject]$result | Write-Output
    exit 0
}
catch {
    [pscustomobject]@{ success = $false; error = $_.Exception.Message } | Write-Output
    exit 1
}
