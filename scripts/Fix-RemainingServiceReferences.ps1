# Fix-RemainingServiceReferences.ps1
# Script to fix remaining service files that need Core namespace

param(
    [string]$ProjectRoot = "c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia"
)

Write-Host "🔧 Fixing remaining Services namespace references..." -ForegroundColor Yellow

# Specific files that need the Core services using statement
$FilesToFix = @(
    "Services\MasterDataService.cs",
    "Services\RemoveService.cs", 
    "Services\QuickButtons.cs",
    "Services\SettingsService.cs",
    "Services\ThemeService.cs"
)

$FilesModified = 0

foreach ($file in $FilesToFix) {
    $fullPath = Join-Path $ProjectRoot $file
    
    if (Test-Path $fullPath) {
        $content = Get-Content $fullPath -Raw
        
        # Check if already has Core using statement
        if ($content -notmatch "using MTM_WIP_Application_Avalonia\.Services\.Core;") {
            $lines = $content -split "`r?`n"
            
            # Find where to insert the using statement (after other MTM using statements)
            $insertIndex = -1
            for ($i = 0; $i -lt $lines.Count; $i++) {
                if ($lines[$i] -match "^using MTM_WIP_Application_Avalonia") {
                    $insertIndex = $i
                }
                elseif ($insertIndex -ge 0 -and $lines[$i] -notmatch "^using MTM_WIP_Application_Avalonia" -and $lines[$i] -notmatch "^\s*$") {
                    break
                }
            }
            
            if ($insertIndex -ge 0) {
                # Insert after the last MTM using statement
                $lines = $lines[0..$insertIndex] + "using MTM_WIP_Application_Avalonia.Services.Core;" + $lines[($insertIndex+1)..($lines.Count-1)]
                
                $newContent = $lines -join "`r`n"
                Set-Content -Path $fullPath -Value $newContent -NoNewline
                
                Write-Host "  ✅ Fixed: $file" -ForegroundColor Green
                $FilesModified++
            }
            else {
                Write-Host "  ⚠️  Could not find insertion point in: $file" -ForegroundColor Yellow
            }
        }
        else {
            Write-Host "  ℹ️  Already fixed: $file" -ForegroundColor Gray
        }
    }
    else {
        Write-Host "  ❌ File not found: $file" -ForegroundColor Red
    }
}

Write-Host "`n📈 Summary:" -ForegroundColor Cyan
Write-Host "  Files modified: $FilesModified" -ForegroundColor Green

# Test build
Write-Host "`n🔨 Testing build..." -ForegroundColor Yellow
try {
    dotnet build --no-restore --verbosity quiet | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build successful! All Core Services references fixed." -ForegroundColor Green
    } else {
        Write-Host "❌ Build still has errors. Running detailed build..." -ForegroundColor Red
        dotnet build --no-restore
    }
} catch {
    Write-Host "❌ Build test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎯 Manual fix script completed!" -ForegroundColor Cyan