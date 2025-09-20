# Fix-CoreServicesReferences.ps1
# Script to fix all namespace references after moving services to Services.Core

param(
    [string]$ProjectRoot = "c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia"
)

Write-Host "🔧 Fixing Core Services namespace references..." -ForegroundColor Yellow

# Define the files that need to be updated
$FilePatternsToUpdate = @(
    "ViewModels/**/*.cs",
    "Services/*.cs",
    "Extensions/*.cs",
    "Controls/**/*.cs",
    "Converters/*.cs",
    "Core/**/*.cs"
)

# Track statistics
$FilesProcessed = 0
$FilesModified = 0

# Function to add Core services using statement if not already present
function Add-CoreServicesUsing {
    param(
        [string]$FilePath
    )
    
    $content = Get-Content $FilePath -Raw
    $modified = $false
    
    # Check if file uses any Core services interfaces
    $usesCoreServices = $content -match "(IConfigurationService|IApplicationStateService|IDatabaseService)" -and
                       $content -match "using MTM_WIP_Application_Avalonia\.Services;" -and
                       $content -notmatch "using MTM_WIP_Application_Avalonia\.Services\.Core;"
    
    if ($usesCoreServices) {
        # Find the line with Services using statement
        $lines = $content -split "`r?`n"
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match "^using MTM_WIP_Application_Avalonia\.Services;") {
                # Insert the Core using statement after the Services using
                $lines = $lines[0..$i] + "using MTM_WIP_Application_Avalonia.Services.Core;" + $lines[($i+1)..($lines.Count-1)]
                $modified = $true
                break
            }
        }
        
        if ($modified) {
            $newContent = $lines -join "`r`n"
            Set-Content -Path $FilePath -Value $newContent -NoNewline
            Write-Host "  ✅ Added Core services using to: $($FilePath | Split-Path -Leaf)" -ForegroundColor Green
            return $true
        }
    }
    
    return $false
}

# Process each file pattern
foreach ($pattern in $FilePatternsToUpdate) {
    $fullPattern = Join-Path $ProjectRoot $pattern
    Write-Host "`n📁 Processing pattern: $pattern" -ForegroundColor Cyan
    
    try {
        $files = Get-ChildItem -Path $fullPattern -Recurse -ErrorAction SilentlyContinue | Where-Object { !$_.PSIsContainer }
        
        foreach ($file in $files) {
            $FilesProcessed++
            
            if (Add-CoreServicesUsing -FilePath $file.FullName) {
                $FilesModified++
            }
        }
        
        Write-Host "  📊 Pattern processed: $($files.Count) files found" -ForegroundColor Gray
    }
    catch {
        Write-Host "  ⚠️  Could not process pattern: $pattern - $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host "`n📈 Summary:" -ForegroundColor Cyan
Write-Host "  Files processed: $FilesProcessed" -ForegroundColor White
Write-Host "  Files modified: $FilesModified" -ForegroundColor Green

# Test build to verify fixes
Write-Host "`n🔨 Testing build to verify fixes..." -ForegroundColor Yellow
try {
    $buildResult = dotnet build --no-restore --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build successful! All namespace references fixed." -ForegroundColor Green
    } else {
        Write-Host "❌ Build still has errors. Manual review may be needed." -ForegroundColor Red
        Write-Host "Run 'dotnet build' for detailed error information." -ForegroundColor Gray
    }
} catch {
    Write-Host "❌ Build test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎯 Script completed!" -ForegroundColor Cyan