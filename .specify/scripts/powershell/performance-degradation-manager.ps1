function Set-PerformanceDegradationMode {
    param([bool]$Enabled)
    $file = ".specify/state/gsc-workflow.json"
    $state = if (Test-Path $file) { Get-Content $file -Raw | ConvertFrom-Json } else { @{ } }
    $state.performanceDegradationMode = $Enabled
    $state | ConvertTo-Json -Depth 10 | Out-File $file -Encoding UTF8
}

if ($ExecutionContext.SessionState.Module) { Export-ModuleMember -Function Set-PerformanceDegradationMode }
