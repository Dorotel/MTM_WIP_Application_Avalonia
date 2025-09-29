param([switch]$Json)
$result = @{ Success=$true; recentCommandTimes=@() }
if ($Json){ $result | ConvertTo-Json -Depth 4 } else { $result }
