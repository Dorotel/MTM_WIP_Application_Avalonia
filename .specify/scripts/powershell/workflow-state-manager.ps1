function Set-WorkflowPhase {
    param([string]$Phase)
    $file = ".specify/state/gsc-workflow.json"
    $state = if (Test-Path $file) { Get-Content $file -Raw | ConvertFrom-Json } else { @{ workflowId=[guid]::NewGuid().ToString(); phaseHistory=@() } }
    $state.currentPhase = $Phase
    if ($state.phaseHistory -notcontains $Phase) { $state.phaseHistory += $Phase }
    $state | ConvertTo-Json -Depth 10 | Out-File $file -Encoding UTF8
}

if ($ExecutionContext.SessionState.Module) { Export-ModuleMember -Function Set-WorkflowPhase }
