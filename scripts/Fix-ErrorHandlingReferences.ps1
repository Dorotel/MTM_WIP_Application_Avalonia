# Fix-ErrorHandlingReferences.ps1
# Script to fix ErrorHandling references to use Services.Core namespace

param(
    [string]$ProjectRoot = "c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia"
)

Write-Host "🔧 Fixing ErrorHandling references to use Services.Core..." -ForegroundColor Yellow

# Get all C# files that might reference ErrorHandling
$files = Get-ChildItem -Path $ProjectRoot -Recurse -Include "*.cs" | Where-Object { 
    !$_.PSIsContainer -and 
    $_.FullName -notlike "*\bin\*" -and 
    $_.FullName -notlike "*\obj\*"
}

$FilesModified = 0
$TotalChanges = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $changes = 0
    
    # Fix Services.ErrorHandling -> Services.Core.ErrorHandling
    $newContent = $content -replace 'Services\.ErrorHandling', 'Services.Core.ErrorHandling'
    if ($newContent -ne $content) {
        $changes += ($content.Split('Services.ErrorHandling').Count - 1)
        $content = $newContent
    }
    
    # Fix namespace references - CS0234 errors for Core services
    $newContent = $content -replace 'MTM_WIP_Application_Avalonia\.Services\.IConfigurationService', 'MTM_WIP_Application_Avalonia.Services.Core.IConfigurationService'
    if ($newContent -ne $content) {
        $changes++
        $content = $newContent
    }
    
    $newContent = $content -replace 'MTM_WIP_Application_Avalonia\.Services\.IApplicationStateService', 'MTM_WIP_Application_Avalonia.Services.Core.IApplicationStateService'
    if ($newContent -ne $content) {
        $changes++
        $content = $newContent
    }
    
    $newContent = $content -replace 'MTM_WIP_Application_Avalonia\.Services\.IDatabaseService', 'MTM_WIP_Application_Avalonia.Services.Core.IDatabaseService'
    if ($newContent -ne $content) {
        $changes++
        $content = $newContent
    }
    
    $newContent = $content -replace 'MTM_WIP_Application_Avalonia\.Services\.ErrorHandling', 'MTM_WIP_Application_Avalonia.Services.Core.ErrorHandling'
    if ($newContent -ne $content) {
        $changes++
        $content = $newContent
    }
    
    # Fix static ErrorHandling references (without Services prefix)
    $newContent = $content -replace '\bErrorHandling\.HandleErrorAsync', 'Services.Core.ErrorHandling.HandleErrorAsync'
    if ($newContent -ne $content -and $content -notmatch 'Services\.Core\.ErrorHandling') {
        $changes++
        $content = $newContent
    }
    
    # Fix Helper_Database_StoredProcedure references
    $newContent = $content -replace '\bHelper_Database_StoredProcedure', 'Services.Core.Helper_Database_StoredProcedure'
    if ($newContent -ne $content) {
        $changes++
        $content = $newContent
    }
    
    if ($changes -gt 0) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  ✅ Fixed $changes references in: $($file.Name)" -ForegroundColor Green
        $FilesModified++
        $TotalChanges += $changes
    }
}

Write-Host "`n📈 Summary:" -ForegroundColor Cyan
Write-Host "  Files processed: $($files.Count)" -ForegroundColor White
Write-Host "  Files modified: $FilesModified" -ForegroundColor Green
Write-Host "  Total changes: $TotalChanges" -ForegroundColor Green

# Test build
Write-Host "`n🔨 Testing build..." -ForegroundColor Yellow
try {
    dotnet build --no-restore --verbosity quiet | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Build successful! All ErrorHandling references fixed." -ForegroundColor Green
    } else {
        Write-Host "❌ Build still has errors. Running detailed build..." -ForegroundColor Red
        dotnet build --no-restore
    }
} catch {
    Write-Host "❌ Build test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎯 ErrorHandling fix script completed!" -ForegroundColor Cyan