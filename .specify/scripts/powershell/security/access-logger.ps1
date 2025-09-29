param([switch]$Json)
$logFile = ".specify/state/memory-access.log"
$entry = "$(Get-Date -Format o) memory-access-check"
Add-Content -Path $logFile -Value $entry -Encoding UTF8 -ErrorAction SilentlyContinue
$result = @{ Success=$true; logPath=$logFile }
if ($Json){ $result | ConvertTo-Json -Depth 4 } else { $result }
