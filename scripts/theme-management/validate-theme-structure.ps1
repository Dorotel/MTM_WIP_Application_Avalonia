# Theme Validation Script
# PowerShell script to validate MTM theme files against master template structure

param(
    [string]$ThemesPath = "Resources\Themes",
    [string]$MasterTemplate = "MTMTheme.axaml",
    [string]$OutputPath = "theme-validation-report.json",
    [switch]$VerboseOutput = $false
)

Write-Host "🎨 MTM Theme Validation Script" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan

$results = @{
    ValidationDate = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    MasterTemplate = $MasterTemplate
    ThemesValidated = @()
    Summary = @{
        TotalThemes = 0
        CompleteThemes = 0
        IncompleteThemes = 0
        TotalMissingBrushes = 0
    }
    RequiredBrushes = @()
}

function Extract-BrushKeys {
    param([string]$ThemePath)
    
    $brushKeys = @()
    $content = Get-Content $ThemePath -Raw
    
    # Match all brush definitions
    $pattern = 'x:Key\s*=\s*"([^"]+)"'
    $matches = [regex]::Matches($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    
    foreach ($match in $matches) {
        $key = $match.Groups[1].Value
        if ($key.StartsWith("MTM_Shared_Logic.")) {
            $brushKeys += $key
        }
    }
    
    return $brushKeys | Sort-Object -Unique
}

function Get-FileStats {
    param([string]$FilePath)
    
    $fileInfo = Get-Item $FilePath
    $content = Get-Content $FilePath -Raw
    
    return @{
        SizeBytes = $fileInfo.Length
        SizeKB = [Math]::Round($fileInfo.Length / 1024, 2)
        LineCount = (Get-Content $FilePath).Count
        HasPreviewSections = $content -match "Design\.PreviewWith"
    }
}

# Load master template
$masterPath = Join-Path $ThemesPath $MasterTemplate
if (-not (Test-Path $masterPath)) {
    Write-Error "Master template not found: $masterPath"
    exit 1
}

Write-Host "Loading master template: $MasterTemplate" -ForegroundColor Yellow
$masterBrushes = Extract-BrushKeys $masterPath
$masterStats = Get-FileStats $masterPath
$results.RequiredBrushes = $masterBrushes

Write-Host "Master template contains $($masterBrushes.Count) required brushes" -ForegroundColor Green

# Validate all theme files
$themeFiles = Get-ChildItem -Path $ThemesPath -Filter "MTM_*.axaml" | Where-Object { $_.Name -ne $MasterTemplate }

foreach ($themeFile in $themeFiles) {
    $themeName = $themeFile.BaseName
    
    if ($VerboseOutput) {
        Write-Host "Validating: $themeName" -ForegroundColor Gray
    }
    
    $themeBrushes = Extract-BrushKeys $themeFile.FullName
    $themeStats = Get-FileStats $themeFile.FullName
    
    # Find missing brushes
    $missingBrushes = $masterBrushes | Where-Object { $_ -notin $themeBrushes }
    $extraBrushes = $themeBrushes | Where-Object { $_ -notin $masterBrushes }
    
    $themeResult = @{
        ThemeName = $themeName
        FilePath = $themeFile.FullName
        IsComplete = $missingBrushes.Count -eq 0
        TotalBrushes = $themeBrushes.Count
        RequiredBrushes = $masterBrushes.Count
        MissingBrushes = $missingBrushes
        ExtraBrushes = $extraBrushes
        FileStats = $themeStats
        CompletionPercentage = [Math]::Round(($themeBrushes.Count / $masterBrushes.Count) * 100, 1)
        WCAGCompliant = "Unknown"  # Would need color parsing for actual validation
        HasPreviewSections = $themeStats.HasPreviewSections
        SizeReduction = @{
            CurrentSizeKB = $themeStats.SizeKB
            TargetSizeKB = 5.0
            MeetsTarget = $themeStats.SizeKB -le 5.0
        }
    }
    
    $results.ThemesValidated += $themeResult
    $results.Summary.TotalThemes++
    $results.Summary.TotalMissingBrushes += $missingBrushes.Count
    
    if ($themeResult.IsComplete) {
        $results.Summary.CompleteThemes++
        Write-Host "✅ $themeName - Complete ($($themeResult.TotalBrushes)/$($themeResult.RequiredBrushes) brushes, $($themeResult.FileStats.SizeKB)KB)" -ForegroundColor Green
    } else {
        $results.Summary.IncompleteThemes++
        Write-Host "❌ $themeName - Incomplete ($($themeResult.TotalBrushes)/$($themeResult.RequiredBrushes) brushes, $($missingBrushes.Count) missing)" -ForegroundColor Red
        
        if ($VerboseOutput) {
            foreach ($missing in $missingBrushes) {
                Write-Host "   Missing: $missing" -ForegroundColor Yellow
            }
        }
    }
}

# Generate detailed report
$results | ConvertTo-Json -Depth 6 | Out-File $OutputPath

# Display summary
Write-Host "`n📊 THEME VALIDATION SUMMARY" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan
Write-Host "Total themes: $($results.Summary.TotalThemes)"
Write-Host "Complete themes: $($results.Summary.CompleteThemes)" -ForegroundColor Green
Write-Host "Incomplete themes: $($results.Summary.IncompleteThemes)" -ForegroundColor Red
Write-Host "Total missing brushes: $($results.Summary.TotalMissingBrushes)" -ForegroundColor Red

if ($results.Summary.CompleteThemes -gt 0) {
    Write-Host "`n✅ COMPLETE THEMES:" -ForegroundColor Green
    $completeThemes = $results.ThemesValidated | Where-Object { $_.IsComplete }
    foreach ($theme in $completeThemes) {
        $sizeStatus = if ($theme.SizeReduction.MeetsTarget) { "✅" } else { "⚠️" }
        Write-Host "  • $($theme.ThemeName) ($($theme.FileStats.SizeKB)KB) $sizeStatus" -ForegroundColor Green
    }
}

if ($results.Summary.IncompleteThemes -gt 0) {
    Write-Host "`n❌ INCOMPLETE THEMES:" -ForegroundColor Red
    $incompleteThemes = $results.ThemesValidated | Where-Object { -not $_.IsComplete } | Sort-Object CompletionPercentage -Descending
    foreach ($theme in $incompleteThemes) {
        Write-Host "  • $($theme.ThemeName): $($theme.CompletionPercentage)% complete ($($theme.MissingBrushes.Count) missing)" -ForegroundColor Red
    }
}

# File size analysis
Write-Host "`n📏 FILE SIZE ANALYSIS:" -ForegroundColor Cyan
$oversizedThemes = $results.ThemesValidated | Where-Object { $_.FileStats.SizeKB -gt 5.0 }
if ($oversizedThemes.Count -gt 0) {
    Write-Host "Themes exceeding 5KB target:" -ForegroundColor Yellow
    foreach ($theme in $oversizedThemes) {
        $previewStatus = if ($theme.HasPreviewSections) { " (Has Preview Sections)" } else { "" }
        Write-Host "  • $($theme.ThemeName): $($theme.FileStats.SizeKB)KB$previewStatus" -ForegroundColor Yellow
    }
} else {
    Write-Host "All themes meet the 5KB size target! 🎉" -ForegroundColor Green
}

Write-Host "`n📋 NEXT ACTIONS:" -ForegroundColor Cyan
if ($results.Summary.IncompleteThemes -gt 0) {
    Write-Host "1. Complete missing brushes in incomplete themes"
    Write-Host "2. Use MTMTheme.axaml as reference for missing definitions"
    Write-Host "3. Ensure WCAG 2.1 AA contrast compliance"
    Write-Host "4. Remove Design.PreviewWith sections to reduce file size"
} else {
    Write-Host "1. All themes are structurally complete! 🎉"
    Write-Host "2. Run WCAG contrast validation"
    Write-Host "3. Test theme switching across all views"
    Write-Host "4. Remove any remaining preview sections"
}

Write-Host "`n📄 Detailed report saved to: $OutputPath" -ForegroundColor Green

# Exit with appropriate code
if ($results.Summary.IncompleteThemes -gt 0) {
    Write-Host "`n⚠️  Some themes are incomplete" -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "`n✅ All themes are structurally complete!" -ForegroundColor Green
    exit 0
}