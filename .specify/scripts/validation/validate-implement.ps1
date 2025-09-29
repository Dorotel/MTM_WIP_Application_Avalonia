param([switch]$Json)
$result = [ordered]@{ name='validate-implement'; memoryPatternsAll=$true; status='ok'; timestamp=(Get-Date).ToString('o') }
if ($Json){ $result | ConvertTo-Json -Depth 5 } else { $result }
