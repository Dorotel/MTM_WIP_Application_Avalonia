param([switch]$Json)
$scriptRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$memoryModule = Join-Path $scriptRoot "powershell\memory-integration.ps1"
try { if (Test-Path $memoryModule) { Import-Module $memoryModule -Force -ErrorAction Stop } } catch { if (Test-Path $memoryModule) { . $memoryModule } }

$result = @{ Success=$true; files=@() }
try {
    $loc = Get-MemoryFileLocations
    foreach ($mf in $loc.MemoryFiles.Values) {
        $entry = @{ fileType=$mf.FileType; filePath=$mf.FilePath; exists=$mf.Exists; checksum=$null }
        if ($mf.Exists) {
            $hash = Get-FileHash -Path $mf.FilePath -Algorithm SHA256
            $entry.checksum = $hash.Hash
        }
        $result.files += $entry
    }
} catch { $result.Success=$false; $result.Error=$_.Exception.Message }
if ($Json){ $result | ConvertTo-Json -Depth 6 } else { $result }
