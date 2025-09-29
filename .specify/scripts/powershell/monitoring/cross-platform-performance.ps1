param([switch]$Json)
$result = @{ Success=$true; platforms=@('windows') }
if ($Json){ $result | ConvertTo-Json -Depth 3 } else { $result }
