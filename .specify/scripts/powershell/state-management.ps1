function Initialize-StateFile {
    param([string]$Path)
    $state = @{ initialized = (Get-Date).ToString('o'); version='1.0.0' }
    $state | ConvertTo-Json -Depth 5 | Out-File $Path -Encoding UTF8
    return $true
}

if ($ExecutionContext.SessionState.Module) { Export-ModuleMember -Function Initialize-StateFile }
