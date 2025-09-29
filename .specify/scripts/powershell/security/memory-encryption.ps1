param([Parameter(Mandatory=$true)][string]$FilePath,[switch]$Decrypt,[switch]$Json)
function Convert-StringSecure([string]$s){ return ConvertTo-SecureString -String $s -AsPlainText -Force }
function Convert-SecureToPlain([securestring]$ss){ $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($ss); try { [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr) } finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr) } }
$result = @{ Success=$true; path=$FilePath; mode= if($Decrypt){'decrypt'}else{'encrypt'} }
try {
    if (-not (Test-Path $FilePath)) { throw "File not found: $FilePath" }
    $content = Get-Content $FilePath -Raw -Encoding UTF8
    if ($Decrypt) {
        $plain = $content # placeholder for real decryption
        $result.outputPreview = $plain.Substring(0, [Math]::Min(64,$plain.Length))
    } else {
        $sec = Convert-StringSecure $content
        $enc = Convert-SecureToPlain $sec  # placeholder - same content for stub
        $result.outputPreview = $enc.Substring(0, [Math]::Min(64,$enc.Length))
    }
} catch { $result.Success=$false; $result.Error=$_.Exception.Message }
if ($Json){ $result | ConvertTo-Json -Depth 5 } else { $result }
