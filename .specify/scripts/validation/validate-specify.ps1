param([switch]$Json)
$result = [ordered]@{ name = 'validate-specify'; avaloniaPatterns = $true; status = 'ok'; timestamp = (Get-Date).ToString('o') }
if ($Json) { $result | ConvertTo-Json -Depth 5 } else { $result }
