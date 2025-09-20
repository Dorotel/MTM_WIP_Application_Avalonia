#!/usr/bin/env pwsh

# PowerShell automation script to fix API namespace references
# Phase 1 Core Services Directory Structure - Final namespace cleanup
Write-Host "🔧 MTM API Namespace Reference Cleanup Script" -ForegroundColor Cyan

# Define the file paths and replacements
$replacements = @{
    # Namespace declarations
    'namespace API.ViewModels.MainForm;' = 'namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;'
    'namespace API.Services;' = 'namespace MTM_WIP_Application_Avalonia.Services;'  
    'namespace API.Controls.CustomDataGrid;' = 'namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;'
    
    # Using statements
    'using API.ViewModels.MainForm;' = 'using MTM_WIP_Application_Avalonia.ViewModels.MainForm;'
    'using API.Controls.CustomDataGrid;' = 'using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;'
}

# Get all C# files in the project
$csharpFiles = Get-ChildItem -Path "." -Filter "*.cs" -Recurse | Where-Object { 
    $_.FullName -notmatch "\\bin\\|\\obj\\|\\packages\\" 
}

$totalChanges = 0
$modifiedFiles = @()

foreach ($file in $csharpFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $fileChanges = 0
    
    # Apply each replacement
    foreach ($find in $replacements.Keys) {
        $replace = $replacements[$find]
        if ($content -match [regex]::Escape($find)) {
            $content = $content -replace [regex]::Escape($find), $replace
            $fileChanges++
            Write-Host "  ✅ Fixed: $find → $replace" -ForegroundColor Green
        }
    }
    
    # Write back if changes were made
    if ($fileChanges -gt 0) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $modifiedFiles += $file.FullName
        $totalChanges += $fileChanges
        Write-Host "📝 Modified: $($file.Name) ($fileChanges changes)" -ForegroundColor Yellow
    }
}

Write-Host "`n📊 Summary:" -ForegroundColor Cyan
Write-Host "  - Files processed: $($csharpFiles.Count)" -ForegroundColor White
Write-Host "  - Files modified: $($modifiedFiles.Count)" -ForegroundColor Green  
Write-Host "  - Total changes: $totalChanges" -ForegroundColor Green
Write-Host "  - Modified files:" -ForegroundColor White
foreach ($file in $modifiedFiles) {
    Write-Host "    • $($file -replace [regex]::Escape((Get-Location).Path + '\'), '')" -ForegroundColor Gray
}

if ($totalChanges -gt 0) {
    Write-Host "`n✅ API namespace references cleanup completed!" -ForegroundColor Green
    Write-Host "🔨 Run 'dotnet build' to verify compilation..." -ForegroundColor Cyan
} else {
    Write-Host "`n✅ No API namespace references found - all clean!" -ForegroundColor Green
}