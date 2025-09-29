function Set-WorkflowLock {
    param([string]$Owner, [int]$Minutes=60)
    $file = ".specify/state/gsc-workflow.json"
    $state = if (Test-Path $file) { Get-Content $file -Raw | ConvertFrom-Json } else { @{ teamCollaborationLock=@{} } }
    $state.teamCollaborationLock = @{ isLocked=$true; lockOwner=$Owner; lockExpiration=(Get-Date).AddMinutes($Minutes).ToString('o') }
    $state | ConvertTo-Json -Depth 10 | Out-File $file -Encoding UTF8
    return $true
}

function Clear-WorkflowLock {
    $file = ".specify/state/gsc-workflow.json"
    if (Test-Path $file){ $state = Get-Content $file -Raw | ConvertFrom-Json; $state.teamCollaborationLock = @{ isLocked=$false }; $state | ConvertTo-Json -Depth 10 | Out-File $file -Encoding UTF8 }
}

if ($ExecutionContext.SessionState.Module) { Export-ModuleMember -Function Set-WorkflowLock,Clear-WorkflowLock }
