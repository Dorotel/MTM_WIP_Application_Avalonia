param([switch]$Json)
$result = @{ Success=$true; lastLoadSeconds=0.0 }
if ($Json){ $result | ConvertTo-Json -Depth 3 } else { $result }
