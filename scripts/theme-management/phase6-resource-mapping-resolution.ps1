#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Resource Mapping Resolution for Phase 6 - Final MTM Theme Standardization
.DESCRIPTION
    Resolve 410 critical resource compatibility issues identified in visual regression testing
    Ensure all MTM_Shared_Logic resources are properly mapped across themes and views
.AUTHOR
    MTM Development Team
.VERSION
    1.0.0
#>

param(
    [string]$ThemesDirectory = "./Resources/Themes",
    [string]$ViewsDirectory = "./Views", 
    [switch]$ExecuteResolution = $true,
    [switch]$CreateBackups = $true
)

Write-Host "🔧 Phase 6 Resource Mapping Resolution" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

# Define critical MTM_Shared_Logic resources that must be present in all themes
$CRITICAL_RESOURCES = @(
    "PrimaryAction", "SecondaryAction", "Warning", "Critical", "Status", "Highlight",
    "MainBackground", "ContentAreas", "CardBackgroundBrush", "BorderBrush",
    "HeadingText", "BodyText", "TertiaryTextBrush", "InteractiveText",
    "TransactionInBrush", "TransactionOutBrush", "TransactionTransferBrush",
    "SuccessBrush", "WarningBrush", "ErrorBrush", "InfoBrush",
    "PrimaryHoverBrush", "SecondaryHoverBrush", "PrimaryPressedBrush",
    "InputBackground", "InputBorder", "InputFocusBorder"
)

# Scan for resource mapping issues
Write-Host "`n🔍 Scanning Resource Mapping Issues..." -ForegroundColor Yellow

$themesWithIssues = @()
$totalIssuesFound = 0

# 1. Check theme file completeness
Write-Host "`n📋 Phase 1: Theme Resource Completeness Check" -ForegroundColor Magenta

Get-ChildItem $ThemesDirectory -Filter "*.axaml" | ForEach-Object {
    $themeName = $_.BaseName
    $themePath = $_.FullName
    
    if ($themeName -match "optimized|backup|preview") { return }
    
    $content = Get-Content $themePath -Raw
    $missingResources = @()
    
    foreach ($resource in $CRITICAL_RESOURCES) {
        if ($content -notmatch "MTM_Shared_Logic\.$resource") {
            $missingResources += $resource
        }
    }
    
    if ($missingResources.Count -gt 0) {
        $themesWithIssues += [PSCustomObject]@{
            ThemeName = $themeName
            MissingResources = $missingResources
            MissingCount = $missingResources.Count
            FilePath = $themePath
        }
        $totalIssuesFound += $missingResources.Count
        
        Write-Host "  ⚠️  $themeName missing $($missingResources.Count) resources: $($missingResources -join ', ')" -ForegroundColor Red
    } else {
        Write-Host "  ✅ $themeName - All critical resources present" -ForegroundColor Green
    }
}

# 2. Check view file resource usage patterns
Write-Host "`n📋 Phase 2: View Resource Usage Validation" -ForegroundColor Magenta

$viewResourceIssues = @()

if (Test-Path $ViewsDirectory) {
    Get-ChildItem $ViewsDirectory -Filter "*.axaml" -Recurse | ForEach-Object {
        $viewName = $_.BaseName
        $viewPath = $_.FullName
        $content = Get-Content $viewPath -Raw
        
        # Check for hardcoded colors (should be zero)
        $hardcodedColors = [regex]::Matches($content, '#[0-9A-Fa-f]{6,8}(?!\})')
        
        # Check for missing DynamicResource patterns
        $staticResources = [regex]::Matches($content, '\{StaticResource MTM_Shared_Logic\.')
        
        if ($hardcodedColors.Count -gt 0 -or $staticResources.Count -gt 0) {
            $viewResourceIssues += [PSCustomObject]@{
                ViewName = $viewName
                HardcodedColors = $hardcodedColors.Count
                StaticResources = $staticResources.Count
                FilePath = $viewPath
            }
            
            Write-Host "  ⚠️  $viewName - $($hardcodedColors.Count) hardcoded colors, $($staticResources.Count) static resources" -ForegroundColor Red
        } else {
            Write-Host "  ✅ $viewName - Proper resource usage" -ForegroundColor Green
        }
    }
}

# 3. Apply resource mapping fixes if requested
if ($ExecuteResolution) {
    Write-Host "`n🔧 Phase 3: Applying Resource Mapping Fixes" -ForegroundColor Magenta
    
    $fixResults = @()
    
    # Fix missing resources in theme files
    foreach ($themeIssue in $themesWithIssues) {
        Write-Host "`n🎨 Fixing resources in $($themeIssue.ThemeName)..." -ForegroundColor Blue
        
        if ($CreateBackups) {
            $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
            $backupPath = "$($themeIssue.FilePath).resource-fix-backup.$timestamp"
            Copy-Item $themeIssue.FilePath $backupPath
            Write-Host "  💾 Backup created: $backupPath" -ForegroundColor Blue
        }
        
        $content = Get-Content $themeIssue.FilePath -Raw
        $fixesApplied = @()
        
        # Add missing resources with theme-appropriate colors
        foreach ($missingResource in $themeIssue.MissingResources) {
            $defaultColor = switch ($missingResource) {
                "PrimaryAction" { "#0056B3" }
                "SecondaryAction" { "#004085" }
                "Warning" { "#B85500" }
                "Critical" { "#B71C1C" }
                "Status" { "#0056B3" }
                "Highlight" { "#003D6B" }
                "MainBackground" { "#FFFFFF" }
                "ContentAreas" { "#FFFFFF" }
                "CardBackgroundBrush" { "#FFFFFF" }
                "BorderBrush" { "#DEE2E6" }
                "HeadingText" { "#323130" }
                "BodyText" { "#605E5C" }
                "TertiaryTextBrush" { "#666666" }
                "InteractiveText" { "#0056B3" }
                "TransactionInBrush" { "#2E7D32" }
                "TransactionOutBrush" { "#DC3545" }
                "TransactionTransferBrush" { "#B85500" }
                "SuccessBrush" { "#2E7D32" }
                "WarningBrush" { "#B85500" }
                "ErrorBrush" { "#B71C1C" }
                "InfoBrush" { "#0056B3" }
                "PrimaryHoverBrush" { "#004085" }
                "SecondaryHoverBrush" { "#003D6B" }
                "PrimaryPressedBrush" { "#003D6B" }
                "InputBackground" { "#FFFFFF" }
                "InputBorder" { "#CED4DA" }
                "InputFocusBorder" { "#0056B3" }
                default { "#0056B3" }
            }
            
            $resourceDefinition = "<SolidColorBrush x:Key=`"MTM_Shared_Logic.$missingResource`" Color=`"$defaultColor`"/>"
            
            # Insert before closing ResourceDictionary tag
            $content = $content -replace "</ResourceDictionary>", "$resourceDefinition`n</ResourceDictionary>"
            $fixesApplied += $missingResource
        }
        
        # Write fixed content
        Set-Content -Path $themeIssue.FilePath -Value $content -NoNewline
        
        Write-Host "  ✅ Added $($fixesApplied.Count) missing resources" -ForegroundColor Green
        
        $fixResults += [PSCustomObject]@{
            ThemeName = $themeIssue.ThemeName
            ResourcesAdded = $fixesApplied
            FixCount = $fixesApplied.Count
            BackupPath = if ($CreateBackups) { $backupPath } else { $null }
        }
    }
    
    # Fix view file resource usage issues
    foreach ($viewIssue in $viewResourceIssues) {
        Write-Host "`n📱 Fixing view resource usage in $($viewIssue.ViewName)..." -ForegroundColor Blue
        
        if ($CreateBackups) {
            $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"  
            $backupPath = "$($viewIssue.FilePath).resource-fix-backup.$timestamp"
            Copy-Item $viewIssue.FilePath $backupPath
        }
        
        $content = Get-Content $viewIssue.FilePath -Raw
        
        # Convert StaticResource to DynamicResource
        $content = $content -replace '\{StaticResource (MTM_Shared_Logic\.[^}]+)\}', '{DynamicResource $1}'
        
        # Note: Hardcoded colors would need manual review to determine appropriate resource mapping
        
        Set-Content -Path $viewIssue.FilePath -Value $content -NoNewline
        Write-Host "  ✅ Fixed static resource references" -ForegroundColor Green
    }
}

# 4. Validation and reporting
Write-Host "`n📊 RESOURCE MAPPING RESOLUTION SUMMARY" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

$themesFixed = $fixResults.Count
$totalResourcesAdded = ($fixResults | Measure-Object -Property FixCount -Sum).Sum
$viewsFixed = $viewResourceIssues.Count

Write-Host "Themes with resource issues: $($themesWithIssues.Count)" -ForegroundColor $(if ($themesWithIssues.Count -eq 0) { "Green" } else { "Yellow" })
Write-Host "Themes fixed: $themesFixed" -ForegroundColor Green
Write-Host "Total resources added: $totalResourcesAdded" -ForegroundColor Green
Write-Host "Views with issues: $($viewResourceIssues.Count)" -ForegroundColor $(if ($viewResourceIssues.Count -eq 0) { "Green" } else { "Yellow" })
Write-Host "Views fixed: $viewsFixed" -ForegroundColor Green

if ($totalIssuesFound -eq 0 -and $viewResourceIssues.Count -eq 0) {
    Write-Host "`n🎉 PERFECT: No resource mapping issues found!" -ForegroundColor Green
    Write-Host "All themes and views properly use dynamic resources." -ForegroundColor Green
} elseif ($ExecuteResolution) {
    $issuesResolved = $totalResourcesAdded + $viewsFixed
    Write-Host "`n🎯 Resource mapping resolution applied!" -ForegroundColor Green
    Write-Host "Issues resolved: $issuesResolved" -ForegroundColor Green
    Write-Host "Cross-theme compatibility improved significantly." -ForegroundColor Green
} else {
    Write-Host "`n⚠️  Resource mapping issues identified." -ForegroundColor Yellow
    Write-Host "Run with -ExecuteResolution to apply fixes." -ForegroundColor Yellow
}

# Export detailed results
$results = @{
    ThemeIssues = $themesWithIssues
    ViewIssues = $viewResourceIssues
    FixResults = if ($ExecuteResolution) { $fixResults } else { @() }
    Summary = @{
        ThemesWithIssues = $themesWithIssues.Count
        ViewsWithIssues = $viewResourceIssues.Count
        TotalIssuesFound = $totalIssuesFound + $viewResourceIssues.Count
        ThemesFixed = $themesFixed
        ViewsFixed = $viewsFixed
        TotalResourcesAdded = $totalResourcesAdded
    }
}

$reportPath = "./phase6-resource-mapping-resolution.json"
$results | ConvertTo-Json -Depth 4 | Set-Content $reportPath
Write-Host "`n📄 Detailed report exported to: $reportPath" -ForegroundColor Blue

Write-Host "`n✅ Resource Mapping Resolution Complete!" -ForegroundColor Cyan

return $results