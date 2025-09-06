#!/usr/bin/env pwsh
# Script Path Migration Tool
# Updates all documentation references to use the new organized script structure

param(
    [switch]$WhatIf = $false
)

# Define the script path mappings
$PathMappings = @{
    "scripts/validate-wcag-compliance.ps1" = "scripts/accessibility/validate-wcag-compliance.ps1"
    "scripts/remediate-wcag-failures.ps1" = "scripts/accessibility/remediate-wcag-failures.ps1"
    "scripts/analyze-ui-theme-readiness.ps1" = "scripts/ui-analysis/analyze-ui-theme-readiness.ps1"
    "scripts/analyze-ui-theme-readiness.sh" = "scripts/ui-analysis/analyze-ui-theme-readiness.sh"
    "scripts/validate-theme-structure.ps1" = "scripts/theme-management/validate-theme-structure.ps1"
    "scripts/detect-hardcoded-colors.ps1" = "scripts/ui-analysis/detect-hardcoded-colors.ps1"
    "scripts/fix-view-brush-combinations.ps1" = "scripts/ui-analysis/fix-view-brush-combinations.ps1"
    "scripts/validate-view-brush-combinations.ps1" = "scripts/ui-analysis/validate-view-brush-combinations.ps1"
    "scripts/performance-test-themes.ps1" = "scripts/performance-optimization/performance-test-themes.ps1"
    "scripts/theme-epic-status-report.ps1" = "scripts/reporting/theme-epic-status-report.ps1"
}

Write-Host "🔄 MTM Scripts Path Migration Tool" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

if ($WhatIf) {
    Write-Host "⚠️  RUNNING IN WHAT-IF MODE - No changes will be made" -ForegroundColor Yellow
}

# Find all markdown files in docs directory
$MarkdownFiles = Get-ChildItem -Path "docs" -Recurse -Filter "*.md"

Write-Host "📁 Found $($MarkdownFiles.Count) markdown files to check" -ForegroundColor Green

$TotalReplacements = 0
$FilesModified = 0

foreach ($File in $MarkdownFiles) {
    Write-Host "🔍 Checking: $($File.FullName)" -ForegroundColor White
    
    $Content = Get-Content $File.FullName -Raw
    $OriginalContent = $Content
    $FileReplacements = 0
    
    foreach ($Mapping in $PathMappings.GetEnumerator()) {
        $OldPath = $Mapping.Key
        $NewPath = $Mapping.Value
        
        if ($Content -match [regex]::Escape($OldPath)) {
            $MatchCount = ([regex]::Matches($Content, [regex]::Escape($OldPath))).Count
            $Content = $Content -replace [regex]::Escape($OldPath), $NewPath
            $FileReplacements += $MatchCount
            $TotalReplacements += $MatchCount
            
            Write-Host "   ✅ $OldPath → $NewPath ($MatchCount matches)" -ForegroundColor Yellow
        }
    }
    
    # Write the updated content back to file if changes were made
    if ($FileReplacements -gt 0) {
        if (-not $WhatIf) {
            Set-Content -Path $File.FullName -Value $Content -NoNewline
            Write-Host "   💾 Updated file with $FileReplacements replacements" -ForegroundColor Green
        } else {
            Write-Host "   🎯 WHAT-IF: Would update file with $FileReplacements replacements" -ForegroundColor Magenta
        }
        $FilesModified++
    }
}

Write-Host ""
Write-Host "📊 MIGRATION SUMMARY" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host "Files checked: $($MarkdownFiles.Count)" -ForegroundColor White
Write-Host "Files modified: $FilesModified" -ForegroundColor Green
Write-Host "Total replacements: $TotalReplacements" -ForegroundColor Yellow

if ($WhatIf) {
    Write-Host ""
    Write-Host "⚠️  This was a What-If run - no changes were made" -ForegroundColor Yellow
    Write-Host "   Remove -WhatIf parameter to apply the changes" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "✅ Path migration completed successfully!" -ForegroundColor Green
}
