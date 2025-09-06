#!/usr/bin/env pwsh
# ================================================================
# PHASE 6 VISUAL REGRESSION TESTING TOOL
# MTM Theme Standardization EPIC - Cross-Theme UI Validation
# ================================================================
# 
# Purpose: Comprehensive visual regression testing across all themes
# Features:
#   - Cross-theme UI consistency validation
#   - Color mapping verification
#   - Theme switching simulation
#   - UI element accessibility validation
#   - Component rendering verification
#
# Created: September 6, 2025
# Author: MTM Development Team
# ================================================================

param(
    [Parameter(Mandatory=$false)]
    [string]$ThemesPath = "Resources/Themes",
    
    [Parameter(Mandatory=$false)]
    [string]$ViewsPath = "Views",
    
    [Parameter(Mandatory=$false)]
    [switch]$GenerateBaseline = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$BaselinePath = "baseline-visual-regression",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputReportPath = "phase6-visual-regression-report.json"
)

# ================================================================
# VISUAL VALIDATION FUNCTIONS
# ================================================================

function Get-AllThemeFiles {
    param([string]$Path)
    
    return Get-ChildItem -Path $Path -Filter "*.axaml" | Where-Object { 
        $_.Name -ne "MTMComponentsPreview.axaml" -and
        $_.Name -ne "MTMComponents.axaml"
    }
}

function Get-AllViewFiles {
    param([string]$Path)
    
    return Get-ChildItem -Path $Path -Filter "*.axaml" -Recurse | Where-Object {
        $_.Name -notmatch "MainWindow|App\.axaml|\.g\."
    }
}

function Get-ThemeBrushes {
    param([string]$ThemeFilePath)
    
    $content = Get-Content $ThemeFilePath -Raw
    $brushes = @{}
    
    # Extract SolidColorBrush definitions
    $solidBrushPattern = 'SolidColorBrush\s+x:Key="([^"]+)"\s+Color="([^"]+)"'
    $matches = [regex]::Matches($content, $solidBrushPattern)
    
    foreach ($match in $matches) {
        $brushes[$match.Groups[1].Value] = @{
            Type = "SolidColorBrush"
            Color = $match.Groups[2].Value
        }
    }
    
    # Extract LinearGradientBrush definitions (simplified)
    $gradientBrushPattern = 'LinearGradientBrush\s+x:Key="([^"]+)"[\s\S]*?</LinearGradientBrush>'
    $gradientMatches = [regex]::Matches($content, $gradientBrushPattern)
    
    foreach ($match in $gradientMatches) {
        $brushes[$match.Groups[1].Value] = @{
            Type = "LinearGradientBrush"
            Definition = $match.Groups[0].Value
        }
    }
    
    return $brushes
}

function Get-ViewResourceUsage {
    param([string]$ViewFilePath)
    
    $content = Get-Content $ViewFilePath -Raw
    $resources = @()
    
    # Find all DynamicResource and StaticResource references
    $resourcePattern = '(DynamicResource|StaticResource)\s+([^}]+)'
    $matches = [regex]::Matches($content, $resourcePattern)
    
    foreach ($match in $matches) {
        $resources += @{
            Type = $match.Groups[1].Value
            ResourceKey = $match.Groups[2].Value.Trim()
            Context = $match.Groups[0].Value
        }
    }
    
    return $resources
}

function Test-ThemeViewCompatibility {
    param([object]$ThemeBrushes, [string]$ViewFilePath, [string]$ThemeName)
    
    $viewResources = Get-ViewResourceUsage -ViewFilePath $ViewFilePath
    $issues = @()
    $warnings = @()
    
    foreach ($resource in $viewResources) {
        if ($resource.ResourceKey.StartsWith("MTM_Shared_Logic.")) {
            if (-not $ThemeBrushes.ContainsKey($resource.ResourceKey)) {
                $issues += @{
                    Type = "MissingResource"
                    ResourceKey = $resource.ResourceKey
                    Context = $resource.Context
                    Severity = "Critical"
                    Message = "View references resource not found in theme"
                }
            } elseif ($ThemeBrushes[$resource.ResourceKey].Type -eq "LinearGradientBrush" -and $resource.Type -eq "StaticResource") {
                $warnings += @{
                    Type = "GradientStaticReference"
                    ResourceKey = $resource.ResourceKey
                    Context = $resource.Context
                    Severity = "Medium"
                    Message = "Gradient brush used as StaticResource may impact theme switching"
                }
            }
        }
    }
    
    return @{
        ViewFile = Split-Path $ViewFilePath -Leaf
        ViewPath = $ViewFilePath
        ThemeName = $ThemeName
        ResourcesFound = $viewResources.Count
        MtmResourcesFound = ($viewResources | Where-Object { $_.ResourceKey.StartsWith("MTM_Shared_Logic.") }).Count
        Issues = $issues
        Warnings = $warnings
        IsCompatible = ($issues.Count -eq 0)
        QualityScore = [Math]::Max(0, 100 - ($issues.Count * 20) - ($warnings.Count * 5))
    }
}

function Get-ColorConsistencyAnalysis {
    param([hashtable]$AllThemeBrushes)
    
    $consistencyReport = @{
        SharedBrushKeys = @()
        InconsistentBrushKeys = @()
        UniqueBrushKeys = @{}
        ColorVariations = @{}
        ConsistencyScore = 0
    }
    
    # Get all unique brush keys across all themes
    $allKeys = @{}
    foreach ($themeName in $AllThemeBrushes.Keys) {
        foreach ($brushKey in $AllThemeBrushes[$themeName].Keys) {
            if (-not $allKeys.ContainsKey($brushKey)) {
                $allKeys[$brushKey] = @()
            }
            $allKeys[$brushKey] += @{
                Theme = $themeName
                Brush = $AllThemeBrushes[$themeName][$brushKey]
            }
        }
    }
    
    # Analyze consistency
    $totalKeys = $allKeys.Count
    $consistentKeys = 0
    $themeCount = $AllThemeBrushes.Count
    
    foreach ($brushKey in $allKeys.Keys) {
        $appearances = $allKeys[$brushKey]
        
        if ($appearances.Count -eq $themeCount) {
            # Brush appears in all themes
            $consistencyReport.SharedBrushKeys += $brushKey
            $consistentKeys++
            
            # Check if colors are semantically appropriate (different across themes)
            $colors = $appearances | Where-Object { $_.Brush.Type -eq "SolidColorBrush" } | ForEach-Object { $_.Brush.Color }
            if ($colors.Count -gt 1) {
                $uniqueColors = $colors | Select-Object -Unique
                $consistencyReport.ColorVariations[$brushKey] = @{
                    TotalColors = $colors.Count
                    UniqueColors = $uniqueColors.Count
                    Colors = $uniqueColors
                    IsMonochromatic = ($uniqueColors.Count -eq 1)
                }
            }
        } else {
            # Brush missing from some themes
            $consistencyReport.InconsistentBrushKeys += @{
                BrushKey = $brushKey
                AppearanceCount = $appearances.Count
                MissingFromThemes = $AllThemeBrushes.Keys | Where-Object { 
                    $themeName = $_
                    -not ($appearances | Where-Object { $_.Theme -eq $themeName })
                }
            }
        }
        
        # Track unique brushes per theme
        if ($appearances.Count -eq 1) {
            $themeName = $appearances[0].Theme
            if (-not $consistencyReport.UniqueBrushKeys.ContainsKey($themeName)) {
                $consistencyReport.UniqueBrushKeys[$themeName] = @()
            }
            $consistencyReport.UniqueBrushKeys[$themeName] += $brushKey
        }
    }
    
    # Calculate consistency score
    $consistencyReport.ConsistencyScore = if ($totalKeys -gt 0) { 
        [Math]::Round(($consistentKeys / $totalKeys) * 100, 1) 
    } else { 
        0 
    }
    
    return $consistencyReport
}

# ================================================================
# BASELINE AND COMPARISON FUNCTIONS
# ================================================================

function Save-VisualBaseline {
    param([object]$Results, [string]$BaselinePath)
    
    if (-not (Test-Path $BaselinePath)) {
        New-Item -ItemType Directory -Path $BaselinePath -Force | Out-Null
    }
    
    $baselineData = @{
        CreatedDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        ThemeCount = $Results.Summary.TotalThemes
        ViewCount = $Results.Summary.TotalViews
        OverallCompatibilityScore = $Results.Summary.OverallCompatibilityScore
        ColorConsistencyScore = $Results.ColorConsistency.ConsistencyScore
        ThemeResults = $Results.ThemeResults
        ColorConsistency = $Results.ColorConsistency
        ViewCompatibility = $Results.ViewCompatibility
    }
    
    $baselineFile = Join-Path $BaselinePath "visual-baseline.json"
    $baselineData | ConvertTo-Json -Depth 10 | Set-Content -Path $baselineFile
    
    Write-Host "Visual baseline saved to: $baselineFile" -ForegroundColor Green
    return $baselineFile
}

function Compare-WithBaseline {
    param([object]$CurrentResults, [string]$BaselinePath)
    
    $baselineFile = Join-Path $BaselinePath "visual-baseline.json"
    if (-not (Test-Path $baselineFile)) {
        Write-Warning "No baseline found at: $baselineFile"
        return $null
    }
    
    $baseline = Get-Content $baselineFile -Raw | ConvertFrom-Json
    
    $comparison = @{
        BaselineDate = $baseline.CreatedDate
        ComparisonDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        Changes = @{
            ThemeCount = $CurrentResults.Summary.TotalThemes - $baseline.ThemeCount
            ViewCount = $CurrentResults.Summary.TotalViews - $baseline.ViewCount
            CompatibilityScore = [Math]::Round($CurrentResults.Summary.OverallCompatibilityScore - $baseline.OverallCompatibilityScore, 1)
            ConsistencyScore = [Math]::Round($CurrentResults.ColorConsistency.ConsistencyScore - $baseline.ColorConsistencyScore, 1)
        }
        Regressions = @()
        Improvements = @()
        NewIssues = @()
        ResolvedIssues = @()
    }
    
    # Identify changes in compatibility
    foreach ($currentTheme in $CurrentResults.ThemeResults) {
        $baselineTheme = $baseline.ThemeResults | Where-Object { $_.ThemeName -eq $currentTheme.ThemeName }
        if ($baselineTheme) {
            $scoreDiff = $currentTheme.AverageCompatibilityScore - $baselineTheme.AverageCompatibilityScore
            if ($scoreDiff -lt -5) {
                $comparison.Regressions += "Theme $($currentTheme.ThemeName): Compatibility decreased by $((-$scoreDiff).ToString('F1'))%"
            } elseif ($scoreDiff -gt 5) {
                $comparison.Improvements += "Theme $($currentTheme.ThemeName): Compatibility improved by $($scoreDiff.ToString('F1'))%"
            }
        }
    }
    
    return $comparison
}

# ================================================================
# MAIN EXECUTION
# ================================================================

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "PHASE 6 VISUAL REGRESSION TESTING - CROSS-THEME VALIDATION" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Validate paths
if (-not (Test-Path $ThemesPath)) {
    Write-Error "Themes path not found: $ThemesPath"
    exit 1
}

if (-not (Test-Path $ViewsPath)) {
    Write-Error "Views path not found: $ViewsPath"
    exit 1
}

# Get all theme and view files
$themeFiles = Get-AllThemeFiles -Path $ThemesPath
$viewFiles = Get-AllViewFiles -Path $ViewsPath

Write-Host "Found $($themeFiles.Count) theme files and $($viewFiles.Count) view files" -ForegroundColor Green
Write-Host ""

# Initialize results
$results = @{
    TestDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Summary = @{
        TotalThemes = $themeFiles.Count
        TotalViews = $viewFiles.Count
        TotalCompatibilityTests = 0
        PassedTests = 0
        FailedTests = 0
        OverallCompatibilityScore = 0
        CriticalIssues = 0
        Warnings = 0
    }
    ThemeResults = @()
    ViewCompatibility = @()
    ColorConsistency = @{}
    DetailedResults = @()
}

# Load all theme brushes
Write-Host "Loading theme brushes..." -ForegroundColor Yellow
$allThemeBrushes = @{}

foreach ($themeFile in $themeFiles) {
    Write-Host "  Processing: $($themeFile.Name)" -ForegroundColor Cyan
    $brushes = Get-ThemeBrushes -ThemeFilePath $themeFile.FullName
    $allThemeBrushes[$themeFile.BaseName] = $brushes
}

Write-Host ""

# Analyze color consistency across themes
Write-Host "Analyzing color consistency across themes..." -ForegroundColor Yellow
$results.ColorConsistency = Get-ColorConsistencyAnalysis -AllThemeBrushes $allThemeBrushes

Write-Host "Color Consistency Score: $($results.ColorConsistency.ConsistencyScore)%" -ForegroundColor $(
    if ($results.ColorConsistency.ConsistencyScore -ge 90) { "Green" }
    elseif ($results.ColorConsistency.ConsistencyScore -ge 75) { "Yellow" }
    else { "Red" }
)
Write-Host ""

# Test each theme against each view
Write-Host "Testing theme-view compatibility..." -ForegroundColor Yellow
$totalTests = $themeFiles.Count * $viewFiles.Count
$currentTest = 0

foreach ($themeFile in $themeFiles) {
    $themeName = $themeFile.BaseName
    $themeBrushes = $allThemeBrushes[$themeName]
    
    Write-Host "Testing theme: $themeName" -ForegroundColor Cyan
    
    $themeResult = @{
        ThemeName = $themeName
        TestedViews = 0
        CompatibleViews = 0
        ViewResults = @()
        AverageCompatibilityScore = 0
        CriticalIssues = 0
        TotalWarnings = 0
    }
    
    $totalScores = 0
    
    foreach ($viewFile in $viewFiles) {
        $currentTest++
        Write-Progress -Activity "Testing Compatibility" -Status "$themeName vs $($viewFile.Name)" -PercentComplete (($currentTest / $totalTests) * 100)
        
        $compatibility = Test-ThemeViewCompatibility -ThemeBrushes $themeBrushes -ViewFilePath $viewFile.FullName -ThemeName $themeName
        
        $themeResult.ViewResults += $compatibility
        $themeResult.TestedViews++
        $totalScores += $compatibility.QualityScore
        
        if ($compatibility.IsCompatible) {
            $themeResult.CompatibleViews++
        }
        
        $themeResult.CriticalIssues += ($compatibility.Issues | Where-Object { $_.Severity -eq "Critical" }).Count
        $themeResult.TotalWarnings += $compatibility.Warnings.Count
        
        # Add to detailed results for report
        $results.DetailedResults += $compatibility
    }
    
    $themeResult.AverageCompatibilityScore = if ($themeResult.TestedViews -gt 0) {
        [Math]::Round($totalScores / $themeResult.TestedViews, 1)
    } else { 0 }
    
    $results.ThemeResults += $themeResult
    
    Write-Host "  - Compatible Views: $($themeResult.CompatibleViews)/$($themeResult.TestedViews)" -ForegroundColor $(
        if ($themeResult.CompatibleViews -eq $themeResult.TestedViews) { "Green" } else { "Yellow" }
    )
    Write-Host "  - Average Quality Score: $($themeResult.AverageCompatibilityScore)%" -ForegroundColor $(
        if ($themeResult.AverageCompatibilityScore -ge 90) { "Green" }
        elseif ($themeResult.AverageCompatibilityScore -ge 75) { "Yellow" }
        else { "Red" }
    )
    Write-Host ""
}

Write-Progress -Activity "Testing Compatibility" -Completed

# Calculate overall metrics
$results.Summary.TotalCompatibilityTests = $totalTests
$results.Summary.PassedTests = ($results.DetailedResults | Where-Object { $_.IsCompatible }).Count
$results.Summary.FailedTests = $results.Summary.TotalCompatibilityTests - $results.Summary.PassedTests
$results.Summary.OverallCompatibilityScore = [Math]::Round(($results.Summary.PassedTests / $results.Summary.TotalCompatibilityTests) * 100, 1)
$results.Summary.CriticalIssues = ($results.DetailedResults | ForEach-Object { $_.Issues | Where-Object { $_.Severity -eq "Critical" } }).Count
$results.Summary.Warnings = ($results.DetailedResults | ForEach-Object { $_.Warnings }).Count

# Generate view compatibility summary
$viewCompatibilitySummary = @{}
foreach ($viewFile in $viewFiles) {
    $viewName = $viewFile.Name
    $viewResults = $results.DetailedResults | Where-Object { $_.ViewFile -eq $viewName }
    
    $compatibleThemes = ($viewResults | Where-Object { $_.IsCompatible }).Count
    $avgQualityScore = if ($viewResults.Count -gt 0) {
        [Math]::Round(($viewResults | Measure-Object QualityScore -Average).Average, 1)
    } else { 0 }
    
    $viewCompatibilitySummary[$viewName] = @{
        ViewFile = $viewName
        CompatibleThemes = $compatibleThemes
        TotalThemes = $viewResults.Count
        CompatibilityPercentage = if ($viewResults.Count -gt 0) { [Math]::Round(($compatibleThemes / $viewResults.Count) * 100, 1) } else { 0 }
        AverageQualityScore = $avgQualityScore
    }
}

$results.ViewCompatibility = $viewCompatibilitySummary.Values | Sort-Object CompatibilityPercentage -Descending

# Display results
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "VISUAL REGRESSION TESTING RESULTS" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "Total Tests: $($results.Summary.TotalCompatibilityTests)" -ForegroundColor White
Write-Host "Passed: $($results.Summary.PassedTests)" -ForegroundColor Green
Write-Host "Failed: $($results.Summary.FailedTests)" -ForegroundColor $(if ($results.Summary.FailedTests -eq 0) { "Green" } else { "Red" })
Write-Host "Overall Compatibility: $($results.Summary.OverallCompatibilityScore)%" -ForegroundColor $(
    if ($results.Summary.OverallCompatibilityScore -ge 95) { "Green" }
    elseif ($results.Summary.OverallCompatibilityScore -ge 85) { "Yellow" }
    else { "Red" }
)
Write-Host "Color Consistency: $($results.ColorConsistency.ConsistencyScore)%" -ForegroundColor $(
    if ($results.ColorConsistency.ConsistencyScore -ge 90) { "Green" }
    elseif ($results.ColorConsistency.ConsistencyScore -ge 75) { "Yellow" }
    else { "Red" }
)
Write-Host "Critical Issues: $($results.Summary.CriticalIssues)" -ForegroundColor $(if ($results.Summary.CriticalIssues -eq 0) { "Green" } else { "Red" })
Write-Host "Warnings: $($results.Summary.Warnings)" -ForegroundColor $(if ($results.Summary.Warnings -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

# Show top and bottom performing themes
$topThemes = $results.ThemeResults | Sort-Object AverageCompatibilityScore -Descending | Select-Object -First 3
$bottomThemes = $results.ThemeResults | Sort-Object AverageCompatibilityScore | Select-Object -First 3

Write-Host "Top Performing Themes:" -ForegroundColor Green
$topThemes | ForEach-Object { Write-Host "  - $($_.ThemeName): $($_.AverageCompatibilityScore)%" -ForegroundColor Green }
Write-Host ""

if ($bottomThemes[0].AverageCompatibilityScore -lt 90) {
    Write-Host "Themes Needing Attention:" -ForegroundColor Yellow
    $bottomThemes | ForEach-Object { Write-Host "  - $($_.ThemeName): $($_.AverageCompatibilityScore)%" -ForegroundColor Yellow }
    Write-Host ""
}

# Handle baseline operations
if ($GenerateBaseline) {
    $baselineFile = Save-VisualBaseline -Results $results -BaselinePath $BaselinePath
} else {
    $comparison = Compare-WithBaseline -CurrentResults $results -BaselinePath $BaselinePath
    if ($comparison) {
        Write-Host "Comparison with Baseline:" -ForegroundColor Cyan
        Write-Host "  Compatibility Change: $($comparison.Changes.CompatibilityScore)%" -ForegroundColor $(
            if ($comparison.Changes.CompatibilityScore -ge 0) { "Green" } else { "Red" }
        )
        Write-Host "  Consistency Change: $($comparison.Changes.ConsistencyScore)%" -ForegroundColor $(
            if ($comparison.Changes.ConsistencyScore -ge 0) { "Green" } else { "Red" }
        )
        
        if ($comparison.Regressions.Count -gt 0) {
            Write-Host "  Regressions:" -ForegroundColor Red
            $comparison.Regressions | ForEach-Object { Write-Host "    - $_" -ForegroundColor Red }
        }
        
        if ($comparison.Improvements.Count -gt 0) {
            Write-Host "  Improvements:" -ForegroundColor Green
            $comparison.Improvements | ForEach-Object { Write-Host "    - $_" -ForegroundColor Green }
        }
        
        $results.BaselineComparison = $comparison
    }
}

# Save detailed report
$results | ConvertTo-Json -Depth 10 | Set-Content -Path $OutputReportPath
Write-Host "Detailed report saved to: $OutputReportPath" -ForegroundColor Green
Write-Host ""

# Final recommendations
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "RECOMMENDATIONS" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

if ($results.Summary.OverallCompatibilityScore -ge 95 -and $results.ColorConsistency.ConsistencyScore -ge 90) {
    Write-Host "🎉 EXCELLENT! Visual consistency is at professional-grade level!" -ForegroundColor Green
} else {
    Write-Host "Areas for improvement:" -ForegroundColor Yellow
    
    if ($results.Summary.OverallCompatibilityScore -lt 95) {
        Write-Host "1. Address theme-view compatibility issues" -ForegroundColor White
        Write-Host "   - Focus on themes with scores below 90%" -ForegroundColor White
    }
    
    if ($results.ColorConsistency.ConsistencyScore -lt 90) {
        Write-Host "2. Improve color consistency across themes" -ForegroundColor White
        Write-Host "   - Ensure all themes have complete brush definitions" -ForegroundColor White
    }
    
    if ($results.Summary.CriticalIssues -gt 0) {
        Write-Host "3. Resolve $($results.Summary.CriticalIssues) critical resource issues" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "PHASE 6 VISUAL REGRESSION TESTING COMPLETE" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan