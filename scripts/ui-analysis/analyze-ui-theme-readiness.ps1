# UI Theme Readiness Analysis Script
# Systematically analyzes all AXAML view files for theme compliance and UI/UX guideline adherence

param(
    [string]$ViewFile = "",
    [switch]$AllViews = $false,
    [switch]$GenerateReports = $true,
    [switch]$Verbose = $false
)

# MTM Theme Brush Definitions for Validation
$MTMBrushes = @{
    # Core Colors
    "MTM_Shared_Logic.PrimaryAction" = "#0056B3"
    "MTM_Shared_Logic.SecondaryAction" = "#004085"
    "MTM_Shared_Logic.Warning" = "#B85500"
    "MTM_Shared_Logic.Critical" = "#B71C1C"
    "MTM_Shared_Logic.Highlight" = "#003D6B"
    
    # Layout Colors
    "MTM_Shared_Logic.MainBackground" = "#FFFFFF"
    "MTM_Shared_Logic.ContentAreas" = "#FFFFFF"
    "MTM_Shared_Logic.CardBackground" = "#F8F9FA"
    "MTM_Shared_Logic.CardBackgroundBrush" = "#FFFFFF"
    "MTM_Shared_Logic.PanelBackgroundBrush" = "#F8F9FA"
    "MTM_Shared_Logic.SidebarDark" = "#212529"
    "MTM_Shared_Logic.BorderBrush" = "#DEE2E6"
    "MTM_Shared_Logic.BorderDarkBrush" = "#ADB5BD"
    "MTM_Shared_Logic.HoverBackground" = "#E9ECEF"
    
    # Typography
    "MTM_Shared_Logic.HeadingText" = "#0056B3"
    "MTM_Shared_Logic.BodyText" = "#666666"
    "MTM_Shared_Logic.TertiaryTextBrush" = "#666666"
    "MTM_Shared_Logic.PlaceholderTextBrush" = "#6C757D"
    "MTM_Shared_Logic.DisabledTextBrush" = "#ADB5BD"
    "MTM_Shared_Logic.InteractiveText" = "#0056B3"
    "MTM_Shared_Logic.LinkTextBrush" = "#0056B3"
    "MTM_Shared_Logic.OverlayTextBrush" = "#FFFFFF"
    
    # Form Controls
    "MTM_Shared_Logic.InputBackground" = "#FFFFFF"
    "MTM_Shared_Logic.InputBorder" = "#CED4DA"
    "MTM_Shared_Logic.InputFocusBorder" = "#0056B3"
    "MTM_Shared_Logic.InputHoverBorder" = "#004085"
    "MTM_Shared_Logic.InputDisabledBackground" = "#F8F9FA"
    "MTM_Shared_Logic.InputErrorBorder" = "#B71C1C"
    
    # Status/Feedback
    "MTM_Shared_Logic.SuccessBrush" = "#2E7D32"
    "MTM_Shared_Logic.WarningBrush" = "#B85500"
    "MTM_Shared_Logic.ErrorBrush" = "#B71C1C"
    "MTM_Shared_Logic.InfoBrush" = "#0056B3"
    
    # Transaction Colors
    "MTM_Shared_Logic.TransactionInBrush" = "#2E7D32"
    "MTM_Shared_Logic.TransactionOutBrush" = "#DC3545"
    "MTM_Shared_Logic.TransactionTransferBrush" = "#B85500"
    
    # Interactive States
    "MTM_Shared_Logic.PrimaryHoverBrush" = "#004085"
    "MTM_Shared_Logic.SelectionBrush" = "#E3F2FD"
    "MTM_Shared_Logic.FocusBrush" = "#0056B3"
    "MTM_Shared_Logic.ActiveBrush" = "#0056B3"
    "MTM_Shared_Logic.InactiveBrush" = "#ADB5BD"
}

# WCAG Contrast Calculation Functions
function Get-ContrastRatio {
    param([string]$Color1, [string]$Color2)
    
    $lum1 = Get-RelativeLuminance $Color1
    $lum2 = Get-RelativeLuminance $Color2
    
    $lighter = [Math]::Max($lum1, $lum2)
    $darker = [Math]::Min($lum1, $lum2)
    
    return ($lighter + 0.05) / ($darker + 0.05)
}

function Get-RelativeLuminance {
    param([string]$hexColor)
    
    # Remove # if present
    $hex = $hexColor -replace '#', ''
    
    # Convert to RGB
    $r = [Convert]::ToInt32($hex.Substring(0, 2), 16) / 255.0
    $g = [Convert]::ToInt32($hex.Substring(2, 2), 16) / 255.0
    $b = [Convert]::ToInt32($hex.Substring(4, 2), 16) / 255.0
    
    # Apply gamma correction
    $r = if ($r -le 0.03928) { $r / 12.92 } else { [Math]::Pow(($r + 0.055) / 1.055, 2.4) }
    $g = if ($g -le 0.03928) { $g / 12.92 } else { [Math]::Pow(($g + 0.055) / 1.055, 2.4) }
    $b = if ($b -le 0.03928) { $b / 12.92 } else { [Math]::Pow(($b + 0.055) / 1.055, 2.4) }
    
    # Calculate relative luminance
    return 0.2126 * $r + 0.7152 * $g + 0.0722 * $b
}

function Analyze-AxamlFile {
    param([string]$FilePath)
    
    $fileName = Split-Path $FilePath -Leaf
    $baseName = $fileName -replace '\.axaml$', ''
    
    Write-Host "🔍 Analyzing: $fileName" -ForegroundColor Cyan
    
    if (-not (Test-Path $FilePath)) {
        Write-Host "❌ File not found: $FilePath" -ForegroundColor Red
        return
    }
    
    $content = Get-Content $FilePath -Raw
    $analysis = @{
        FileName = $fileName
        FilePath = $FilePath
        HardcodedColors = @()
        ThemeResourceUsage = @()
        UIGuidelines = @{
            HasCorrectNamespace = $false
            HasProperGridUsage = $false
            HasConsistentSpacing = $false
            HasCardBasedLayout = $false
            UsesTabViewPattern = $false
        }
        WCagCompliance = @{
            ContrastIssues = @()
            AccessibilityFeatures = @()
        }
        ComplianceScore = @{
            ThemeUsage = 0
            UIGuidelines = 0
            WCAGCompliance = 0
            Overall = 0
        }
        Issues = @()
        Recommendations = @()
    }
    
    # 1. Check for hardcoded colors
    $hexColorPattern = '#[A-Fa-f0-9]{6}|#[A-Fa-f0-9]{8}'
    $namedColorPattern = 'Color="(Red|Blue|Green|Yellow|Orange|Purple|Pink|Brown|Gray|Grey|Black|White|Silver|Gold)"'
    $inlineColorPattern = 'Background="[^{]|Foreground="[^{]|BorderBrush="[^{]'
    
    $hexMatches = [regex]::Matches($content, $hexColorPattern)
    $namedMatches = [regex]::Matches($content, $namedColorPattern)
    
    foreach ($match in $hexMatches) {
        $analysis.HardcodedColors += @{
            Type = "Hex Color"
            Value = $match.Value
            Position = $match.Index
        }
        $analysis.Issues += "Hardcoded hex color found: $($match.Value)"
    }
    
    foreach ($match in $namedMatches) {
        $analysis.HardcodedColors += @{
            Type = "Named Color"
            Value = $match.Value
            Position = $match.Index
        }
        $analysis.Issues += "Hardcoded named color found: $($match.Value)"
    }
    
    # 2. Check theme resource usage
    $themeResourcePattern = '\{DynamicResource\s+(MTM_Shared_Logic\.[^}]+)\}'
    $themeMatches = [regex]::Matches($content, $themeResourcePattern)
    
    foreach ($match in $themeMatches) {
        $resourceKey = $match.Groups[1].Value
        $analysis.ThemeResourceUsage += $resourceKey
        if ($MTMBrushes.ContainsKey($resourceKey)) {
            # Valid theme resource
        } else {
            $analysis.Issues += "Unknown theme resource used: $resourceKey"
        }
    }
    
    # 3. Check UI/UX guidelines
    if ($content -match 'xmlns="https://github\.com/avaloniaui"') {
        $analysis.UIGuidelines.HasCorrectNamespace = $true
    } else {
        $analysis.Issues += "Missing or incorrect Avalonia namespace"
    }
    
    if ($content -match 'x:Name="[^"]*Grid[^"]*"' -or $content -match '<Grid[^>]*x:Name') {
        $analysis.UIGuidelines.HasProperGridUsage = $true
    }
    
    if ($content -match 'Name="[^"]*Grid[^"]*"') {
        $analysis.Issues += "Grid uses 'Name' attribute instead of 'x:Name' (AVLN2000 violation)"
    }
    
    # Check for consistent spacing (8px, 16px, 24px)
    $spacingMatches = [regex]::Matches($content, 'Margin="([^"]*)"')
    $hasConsistentSpacing = $false
    foreach ($match in $spacingMatches) {
        $spacing = $match.Groups[1].Value
        if ($spacing -match '^(8|16|24)') {
            $hasConsistentSpacing = $true
        }
    }
    $analysis.UIGuidelines.HasConsistentSpacing = $hasConsistentSpacing
    
    # Check for card-based layout
    if ($content -match '<Border[^>]*CornerRadius|<Border[^>]*Background="\{DynamicResource.*Card') {
        $analysis.UIGuidelines.HasCardBasedLayout = $true
    }
    
    # Check for tab view pattern (ScrollViewer + Grid with RowDefinitions)
    if ($content -match '<ScrollViewer[^>]*>.*<Grid[^>]*RowDefinitions="[*],Auto"' -or 
        $content -match '<ScrollViewer.*<Grid.*RowDefinitions="[*],Auto"') {
        $analysis.UIGuidelines.UsesTabViewPattern = $true
    }
    
    # 4. Calculate compliance scores
    $totalChecks = 10
    $passedChecks = 0
    
    if ($analysis.HardcodedColors.Count -eq 0) { $passedChecks += 2 }
    if ($analysis.ThemeResourceUsage.Count -gt 0) { $passedChecks += 2 }
    if ($analysis.UIGuidelines.HasCorrectNamespace) { $passedChecks++ }
    if ($analysis.UIGuidelines.HasProperGridUsage) { $passedChecks++ }
    if ($analysis.UIGuidelines.HasConsistentSpacing) { $passedChecks++ }
    if ($analysis.UIGuidelines.HasCardBasedLayout) { $passedChecks++ }
    if ($analysis.UIGuidelines.UsesTabViewPattern) { $passedChecks++ }
    if ($analysis.Issues.Count -lt 3) { $passedChecks++ }
    
    $analysis.ComplianceScore.Overall = [math]::Round(($passedChecks / $totalChecks) * 100, 1)
    $analysis.ComplianceScore.ThemeUsage = if ($analysis.HardcodedColors.Count -eq 0 -and $analysis.ThemeResourceUsage.Count -gt 0) { 100 } else { 50 }
    $analysis.ComplianceScore.UIGuidelines = [math]::Round((($analysis.UIGuidelines.Values | Where-Object { $_ -eq $true }).Count / $analysis.UIGuidelines.Count) * 100, 1)
    
    # Generate recommendations
    if ($analysis.HardcodedColors.Count -gt 0) {
        $analysis.Recommendations += "Replace hardcoded colors with appropriate MTM theme resources"
    }
    if (-not $analysis.UIGuidelines.HasCorrectNamespace) {
        $analysis.Recommendations += "Add correct Avalonia namespace: xmlns=`"https://github.com/avaloniaui`""
    }
    if (-not $analysis.UIGuidelines.UsesTabViewPattern -and $fileName -match "Tab.*View\.axaml") {
        $analysis.Recommendations += "Implement mandatory Tab View pattern: ScrollViewer + Grid with RowDefinitions=`"*,Auto`""
    }
    
    return $analysis
}

function Generate-ChecklistReport {
    param($Analysis, $OutputPath)
    
    $template = Get-Content "$PSScriptRoot/../docs/ui-theme-readyness/UI-THEME-READINESS-CHECKLIST-TEMPLATE.md" -Raw
    $reportDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    
    # Replace template placeholders
    $report = $template -replace '\[ViewName\]', ($Analysis.FileName -replace '\.axaml$', '')
    $report = $report -replace '\[DATE\]', $reportDate
    
    # Fill in analysis results
    $compliantElements = ""
    $issuesFound = ""
    $recommendedFixes = ""
    
    if ($Analysis.ThemeResourceUsage.Count -gt 0) {
        $compliantElements = "✅ Uses " + ($Analysis.ThemeResourceUsage.Count) + " MTM theme resources:`n"
        foreach ($resource in ($Analysis.ThemeResourceUsage | Sort-Object | Get-Unique)) {
            $compliantElements += "- $resource`n"
        }
    }
    
    if ($Analysis.Issues.Count -gt 0) {
        foreach ($issue in $Analysis.Issues) {
            $issuesFound += "⚠️ $issue`n"
        }
    }
    
    if ($Analysis.Recommendations.Count -gt 0) {
        foreach ($rec in $Analysis.Recommendations) {
            $recommendedFixes += "🔧 $rec`n"
        }
    }
    
    $report = $report -replace '\[List elements that are properly using theme resources\]', $compliantElements
    $report = $report -replace '\[List any hardcoded colors, improper brush usage, or guideline violations\]', $issuesFound
    $report = $report -replace '\[List specific fixes needed to achieve full compliance\]', $recommendedFixes
    
    # Fill in scores
    $report = $report -replace '\[X\]% compliant', "$($Analysis.ComplianceScore.Overall)% compliant"
    
    # Set status
    $status = if ($Analysis.ComplianceScore.Overall -ge 90) { "COMPLIANT" } 
              elseif ($Analysis.ComplianceScore.Overall -ge 70) { "NEEDS-WORK" } 
              else { "PENDING" }
    $report = $report -replace '\[PENDING/COMPLIANT/NEEDS-WORK\]', $status
    
    # Write report
    Set-Content -Path $OutputPath -Value $report -Encoding UTF8
    
    return $status
}

# Main execution
$rootPath = Split-Path $PSScriptRoot -Parent
$viewsPath = "$rootPath/Views"
$reportsPath = "$rootPath/docs/ui-theme-readyness"

# Ensure reports directory exists
if (-not (Test-Path $reportsPath)) {
    New-Item -Path $reportsPath -ItemType Directory -Force | Out-Null
}

Write-Host "🎯 MTM UI Theme Readiness Analysis" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green

if ($AllViews -or $ViewFile -eq "") {
    # Analyze all view files
    $viewFiles = Get-ChildItem -Path $viewsPath -Recurse -Filter "*.axaml" | 
                 Where-Object { $_.Name -notmatch '\.backup\.|\.resource-fix-backup\.' }
    
    $allResults = @()
    $summary = @{
        TotalFiles = $viewFiles.Count
        CompliantFiles = 0
        NeedsWorkFiles = 0
        PendingFiles = 0
        TotalIssues = 0
        TotalHardcodedColors = 0
    }
    
    foreach ($viewFile in $viewFiles) {
        $analysis = Analyze-AxamlFile $viewFile.FullName
        $allResults += $analysis
        
        if ($GenerateReports) {
            $reportFileName = ($analysis.FileName -replace '\.axaml$', '') + "_theme_readiness_checklist.md"
            $reportPath = Join-Path $reportsPath $reportFileName
            $status = Generate-ChecklistReport $analysis $reportPath
            
            Write-Host "📄 Generated report: $reportFileName (Status: $status)" -ForegroundColor Yellow
            
            switch ($status) {
                "COMPLIANT" { $summary.CompliantFiles++ }
                "NEEDS-WORK" { $summary.NeedsWorkFiles++ }
                "PENDING" { $summary.PendingFiles++ }
            }
        }
        
        $summary.TotalIssues += $analysis.Issues.Count
        $summary.TotalHardcodedColors += $analysis.HardcodedColors.Count
        
        # Display brief results
        $statusColor = switch ($analysis.ComplianceScore.Overall) {
            { $_ -ge 90 } { "Green" }
            { $_ -ge 70 } { "Yellow" } 
            default { "Red" }
        }
        
        Write-Host "  $($analysis.FileName): $($analysis.ComplianceScore.Overall)% compliant" -ForegroundColor $statusColor
        if ($analysis.Issues.Count -gt 0 -and $Verbose) {
            foreach ($issue in $analysis.Issues) {
                Write-Host "    ⚠️ $issue" -ForegroundColor Red
            }
        }
    }
    
    # Generate summary report
    $summaryReport = @"
# UI Theme Readiness Analysis Summary

**Analysis Date**: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**Total View Files Analyzed**: $($summary.TotalFiles)

## 📊 Overall Results

### Compliance Status Distribution
- ✅ **Compliant Files (90%+)**: $($summary.CompliantFiles) files
- ⚠️ **Needs Work Files (70-89%)**: $($summary.NeedsWorkFiles) files  
- 🔄 **Pending Files (<70%)**: $($summary.PendingFiles) files

### Issue Summary
- 🎨 **Total Issues Found**: $($summary.TotalIssues)
- 🚫 **Hardcoded Colors Found**: $($summary.TotalHardcodedColors)
- 📈 **Average Compliance**: $([math]::Round(($allResults | Measure-Object -Property { $_.ComplianceScore.Overall } -Average).Average, 1))%

## 📋 Individual File Reports

$(foreach ($result in ($allResults | Sort-Object { $_.ComplianceScore.Overall } -Descending)) {
"- [$($result.FileName -replace '\.axaml$', '')]($($result.FileName -replace '\.axaml$', '')_theme_readiness_checklist.md) - $($result.ComplianceScore.Overall)% compliant"
})

## 🎯 Top Priority Files for Improvement

$(foreach ($result in ($allResults | Where-Object { $_.ComplianceScore.Overall -lt 70 } | Sort-Object { $_.ComplianceScore.Overall })) {
"### $($result.FileName)
- **Compliance Score**: $($result.ComplianceScore.Overall)%
- **Issues**: $($result.Issues.Count)
- **Hardcoded Colors**: $($result.HardcodedColors.Count)
"
})

## 🏆 Best Performing Files

$(foreach ($result in ($allResults | Where-Object { $_.ComplianceScore.Overall -ge 90 } | Sort-Object { $_.ComplianceScore.Overall } -Descending)) {
"- **$($result.FileName)**: $($result.ComplianceScore.Overall)% compliant"
})

---

**Next Steps**: 
1. Review individual file checklists for detailed compliance information
2. Address hardcoded colors in priority order
3. Implement MTM UI/UX guideline compliance
4. Validate WCAG 2.1 AA contrast ratios
5. Test theme switching across all views

**Generated by**: MTM UI Theme Readiness Analysis Tool
"@
    
    Set-Content -Path "$reportsPath/ANALYSIS-SUMMARY.md" -Value $summaryReport -Encoding UTF8
    
    Write-Host "`n📊 Analysis Complete!" -ForegroundColor Green
    Write-Host "Summary: $($summary.CompliantFiles) compliant, $($summary.NeedsWorkFiles) need work, $($summary.PendingFiles) pending" -ForegroundColor Cyan
    Write-Host "Reports generated in: $reportsPath" -ForegroundColor Yellow
    
} else {
    # Analyze single file
    if (-not (Test-Path $ViewFile)) {
        Write-Host "❌ File not found: $ViewFile" -ForegroundColor Red
        exit 1
    }
    
    $analysis = Analyze-AxamlFile $ViewFile
    
    if ($GenerateReports) {
        $reportFileName = ($analysis.FileName -replace '\.axaml$', '') + "_theme_readiness_checklist.md"
        $reportPath = Join-Path $reportsPath $reportFileName
        $status = Generate-ChecklistReport $analysis $reportPath
        
        Write-Host "📄 Generated report: $reportPath (Status: $status)" -ForegroundColor Yellow
    }
    
    # Display results
    Write-Host "`n📋 Analysis Results for $($analysis.FileName):" -ForegroundColor Cyan
    Write-Host "Overall Compliance: $($analysis.ComplianceScore.Overall)%" -ForegroundColor Green
    Write-Host "Theme Resource Usage: $($analysis.ThemeResourceUsage.Count) resources" -ForegroundColor Yellow
    Write-Host "Issues Found: $($analysis.Issues.Count)" -ForegroundColor $(if ($analysis.Issues.Count -eq 0) { "Green" } else { "Red" })
    Write-Host "Hardcoded Colors: $($analysis.HardcodedColors.Count)" -ForegroundColor $(if ($analysis.HardcodedColors.Count -eq 0) { "Green" } else { "Red" })
}

Write-Host "`n🎯 UI Theme Readiness Analysis Complete!" -ForegroundColor Green