#!/usr/bin/env pwsh
# =============================================================================
# MTM View Brush Combination Validator
# Validates that UI elements in view files use compatible brush combinations
# Prevents scenarios like dark text on dark backgrounds across theme switching
# =============================================================================

param(
    [string]$ViewsPath = "Views",
    [string]$ThemesPath = "Resources/Themes", 
    [string]$OutputPath = "view-brush-validation-report.json",
    [switch]$VerboseOutput,
    [switch]$InteractiveMode,
    [string]$TargetView = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# WCAG 2.1 AA Standards
$WCAG_MIN_CONTRAST = 4.5
$WCAG_LARGE_MIN_CONTRAST = 3.0

# Known problematic brush combinations that should never be used together
$FORBIDDEN_COMBINATIONS = @(
    @{ Foreground = "MTM_Shared_Logic.HeadingText"; Background = "MTM_Shared_Logic.DarkNavigation"; Issue = "Dark text on dark background" },
    @{ Foreground = "MTM_Shared_Logic.BodyText"; Background = "MTM_Shared_Logic.DarkNavigation"; Issue = "Dark text on dark background" },
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.MainBackground"; Issue = "Light text on light background" },
    @{ Foreground = "MTM_Shared_Logic.DarkNavigation"; Background = "MTM_Shared_Logic.HeadingText"; Issue = "Dark on dark combination" }
)

# Recommended brush combinations for different UI contexts
$RECOMMENDED_COMBINATIONS = @{
    "TextBlock" = @{
        SafePairs = @(
            @{ Foreground = "MTM_Shared_Logic.HeadingText"; Background = "MTM_Shared_Logic.MainBackground" },
            @{ Foreground = "MTM_Shared_Logic.BodyText"; Background = "MTM_Shared_Logic.CardBackgroundBrush" },
            @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.PrimaryAction" },
            @{ Foreground = "MTM_Shared_Logic.InteractiveText"; Background = "MTM_Shared_Logic.MainBackground" }
        )
    }
    "Button" = @{
        SafePairs = @(
            @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.PrimaryAction" },
            @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.SecondaryAction" },
            @{ Foreground = "MTM_Shared_Logic.HeadingText"; Background = "MTM_Shared_Logic.CardBackgroundBrush" }
        )
    }
    "Border" = @{
        SafePairs = @(
            @{ Foreground = "MTM_Shared_Logic.HeadingText"; Background = "MTM_Shared_Logic.CardBackgroundBrush" },
            @{ Foreground = "MTM_Shared_Logic.BodyText"; Background = "MTM_Shared_Logic.MainBackground" }
        )
    }
}

function Write-Status($Message, $Color = "White") {
    Write-Host "🔍 $Message" -ForegroundColor $Color
}

function Write-Success($Message) {
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning($Message) {
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

function Write-Error($Message) {
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Critical($Message) {
    Write-Host "🚨 $Message" -ForegroundColor Red
}

function Get-BrushCombinations($ViewContent, $ViewFile) {
    $combinations = @()
    if (-not $ViewContent) {
        return $combinations
    }
    
    try {
        $lines = $ViewContent -split "`n"
    } catch {
        Write-Warning "Could not parse lines in $ViewFile"
        return $combinations
    }
    
    # Patterns to match UI elements with foreground/background combinations
    $elementPatterns = @(
        # Simple pattern for any element with both foreground and background DynamicResource
        'Foreground\s*=\s*"\{DynamicResource\s+([^}]+)\}"[^>]*Background\s*=\s*"\{DynamicResource\s+([^}]+)\}"',
        'Background\s*=\s*"\{DynamicResource\s+([^}]+)\}"[^>]*Foreground\s*=\s*"\{DynamicResource\s+([^}]+)\}"'
    )
    
    foreach ($pattern in $elementPatterns) {
        try {
            $regexMatches = [regex]::Matches($ViewContent, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            
            if ($regexMatches -and $regexMatches.Count -gt 0) {
                foreach ($match in $regexMatches) {
                    $elementType = "UI Element"
                    $foregroundBrush = ""
                    $backgroundBrush = ""
                    
                    # Extract brush names from match groups
                    if ($match.Groups -and $match.Groups.Count -ge 3) {
                        if ($pattern -like "*Foreground*Background*") {
                            $foregroundBrush = $match.Groups[1].Value
                            $backgroundBrush = $match.Groups[2].Value
                        } else {
                            $backgroundBrush = $match.Groups[1].Value  
                            $foregroundBrush = $match.Groups[2].Value
                        }
                    }
                    
                    # Find line number
                    $lineNumber = 1
                    try {
                        $position = 0
                        foreach ($line in $lines) {
                            $position += $line.Length + 1
                            if ($position -gt $match.Index) {
                                break
                            }
                            $lineNumber++
                        }
                    } catch {
                        $lineNumber = 0
                    }
                    
                    if ($foregroundBrush -and $backgroundBrush) {
                        $combinations += @{
                            ElementType = $elementType
                            ForegroundBrush = $foregroundBrush.Trim()
                            BackgroundBrush = $backgroundBrush.Trim()
                            LineNumber = $lineNumber
                            MatchText = $match.Value
                            ViewFile = $ViewFile
                        }
                    }
                }
            }
        } catch {
            Write-Warning "Error processing pattern '$pattern' in $ViewFile`: $($_.Exception.Message)"
            continue
        }
    }
    
    # Also check for inherited combinations (child elements inheriting parent background)
    try {
        $inheritedCombinations = Get-InheritedBrushCombinations $ViewContent $ViewFile
        if ($inheritedCombinations) {
            $combinations += $inheritedCombinations
        }
    } catch {
        Write-Warning "Error getting inherited combinations in $ViewFile`: $($_.Exception.Message)"
    }
    
    return $combinations
}

function Get-InheritedBrushCombinations($ViewContent, $ViewFile) {
    $inherited = @()
    
    # Find containers with background that contain text elements
    $containerPattern = '(?s)<(Border|Grid|StackPanel|DockPanel)[^>]*Background\s*=\s*"\{[^}]*Resource\s+([^}]+)\}"[^>]*>(.*?)</\1>'
    $containerMatches = [regex]::Matches($ViewContent, $containerPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    
    foreach ($containerMatch in $containerMatches) {
        $containerType = $containerMatch.Groups[1].Value
        $backgroundBrush = $containerMatch.Groups[2].Value.Trim()
        $containerContent = $containerMatch.Groups[3].Value
        
        # Look for TextBlock elements inside container that have foreground but no background
        $textPattern = '<TextBlock[^>]*Foreground\s*=\s*"\{[^}]*Resource\s+([^}]+)\}"(?![^>]*Background)[^>]*>'
        $textMatches = [regex]::Matches($containerContent, $textPattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        
        foreach ($textMatch in $textMatches) {
            $foregroundBrush = $textMatch.Groups[1].Value.Trim()
            
            $inherited += @{
                ElementType = "TextBlock (Inherited)"
                ForegroundBrush = $foregroundBrush
                BackgroundBrush = $backgroundBrush
                LineNumber = 0  # Complex to calculate for nested elements
                MatchText = "$containerType > TextBlock"
                ViewFile = $ViewFile
            }
        }
    }
    
    return $inherited
}

function Test-BrushCombination($ForegroundBrush, $BackgroundBrush, $ElementType) {
    $result = @{
        IsForbidden = $false
        IsRecommended = $false
        WarningLevel = "SAFE"
        Issues = @()
        Recommendations = @()
    }
    
    # Check forbidden combinations
    foreach ($forbidden in $FORBIDDEN_COMBINATIONS) {
        if ($forbidden.Foreground -eq $ForegroundBrush -and $forbidden.Background -eq $BackgroundBrush) {
            $result.IsForbidden = $true
            $result.WarningLevel = "CRITICAL"
            $result.Issues += $forbidden.Issue
        }
    }
    
    # Check if combination is in recommended list
    if ($RECOMMENDED_COMBINATIONS.ContainsKey($ElementType)) {
        $safePairs = $RECOMMENDED_COMBINATIONS[$ElementType].SafePairs
        $isRecommended = $safePairs | Where-Object { 
            $_.Foreground -eq $ForegroundBrush -and $_.Background -eq $BackgroundBrush 
        }
        $result.IsRecommended = $null -ne $isRecommended
    }
    
    # Analyze brush names for potential issues
    $fgType = Get-BrushType $ForegroundBrush
    $bgType = Get-BrushType $BackgroundBrush
    
    # Check for likely problematic combinations based on naming
    if (($fgType -eq "Dark" -and $bgType -eq "Dark") -or 
        ($fgType -eq "Light" -and $bgType -eq "Light")) {
        $result.WarningLevel = "HIGH"
        $result.Issues += "Potential same-tone combination: $fgType foreground on $bgType background"
    }
    
    # Check for semantic color misuse
    if ($ForegroundBrush -like "*OverlayTextBrush*" -and $BackgroundBrush -like "*MainBackground*") {
        $result.WarningLevel = "MEDIUM"
        $result.Issues += "OverlayText typically used on colored backgrounds, not main background"
        $result.Recommendations += "Consider MTM_Shared_Logic.HeadingText or BodyText instead"
    }
    
    return $result
}

function Get-BrushType($BrushName) {
    if ($BrushName -like "*Dark*" -or $BrushName -like "*Navigation*" -or $BrushName -like "*Heading*") {
        return "Dark"
    } elseif ($BrushName -like "*Light*" -or $BrushName -like "*Background*" -or $BrushName -like "*Overlay*") {
        return "Light"  
    } elseif ($BrushName -like "*Primary*" -or $BrushName -like "*Secondary*") {
        return "Colored"
    }
    return "Unknown"
}

function Test-ViewFile($ViewPath) {
    Write-Status "Analyzing view file: $(Split-Path $ViewPath -Leaf)"
    
    try {
        $viewContent = Get-Content $ViewPath -Raw -ErrorAction Stop
    } catch {
        Write-Warning "Could not read file $ViewPath`: $($_.Exception.Message)"
        return @{
            ViewFile = $ViewPath
            ViewName = (Split-Path $ViewPath -Leaf)
            TotalCombinations = 0
            CriticalIssues = @()
            HighWarnings = @()
            MediumWarnings = @()
            SafeCombinations = @()
            OverallStatus = "ERROR"
        }
    }
    
    $combinations = Get-BrushCombinations $viewContent $ViewPath
    
    $result = @{
        ViewFile = $ViewPath
        ViewName = (Split-Path $ViewPath -Leaf)
        TotalCombinations = if ($combinations) { $combinations.Count } else { 0 }
        CriticalIssues = @()
        HighWarnings = @()
        MediumWarnings = @()
        SafeCombinations = @()
        OverallStatus = "SAFE"
    }
    
    if ($combinations -and $combinations.Count -gt 0) {
        foreach ($combination in $combinations) {
            try {
                $test = Test-BrushCombination $combination.ForegroundBrush $combination.BackgroundBrush $combination.ElementType
                
                $combinationResult = @{
                    ElementType = $combination.ElementType
                    ForegroundBrush = $combination.ForegroundBrush
                    BackgroundBrush = $combination.BackgroundBrush
                    LineNumber = $combination.LineNumber
                    WarningLevel = $test.WarningLevel
                    Issues = $test.Issues
                    Recommendations = $test.Recommendations
                    IsForbidden = $test.IsForbidden
                    IsRecommended = $test.IsRecommended
                }
                
                switch ($test.WarningLevel) {
                    "CRITICAL" { 
                        $result.CriticalIssues += $combinationResult
                        $result.OverallStatus = "CRITICAL"
                    }
                    "HIGH" { 
                        $result.HighWarnings += $combinationResult
                        if ($result.OverallStatus -eq "SAFE") { $result.OverallStatus = "HIGH" }
                    }
                    "MEDIUM" { 
                        $result.MediumWarnings += $combinationResult
                        if ($result.OverallStatus -eq "SAFE") { $result.OverallStatus = "MEDIUM" }
                    }
                    "SAFE" { $result.SafeCombinations += $combinationResult }
                }
            } catch {
                Write-Warning "Error testing combination in $($result.ViewName): $($_.Exception.Message)"
            }
        }
    }
    
    return $result
}

function Main {
    Write-Host ""
    Write-Host "🎨 MTM VIEW BRUSH COMBINATION VALIDATOR" -ForegroundColor Cyan
    Write-Host "=======================================" -ForegroundColor Cyan
    Write-Host ""

    # Resolve paths
    $viewsPath = Resolve-Path $ViewsPath -ErrorAction SilentlyContinue
    if (-not $viewsPath) {
        Write-Error "Views path not found: $ViewsPath"
        return
    }

    Write-Status "Views Directory: $viewsPath"
    Write-Status "Validation Focus: Foreground/Background brush combinations"
    Write-Status "Target: Prevent poor contrast combinations across themes"
    Write-Host ""

    # Get view files
    $viewFiles = @()
    if ($TargetView) {
        $targetFile = Get-ChildItem -Path $viewsPath -Filter "*$TargetView*" -Recurse | Select-Object -First 1
        if ($targetFile) {
            $viewFiles = @($targetFile)
            Write-Status "Testing single view: $($targetFile.Name)"
        } else {
            Write-Error "Target view not found: $TargetView"
            return
        }
    } else {
        $viewFiles = Get-ChildItem -Path $viewsPath -Filter "*.axaml" -Recurse |
            Where-Object { $_.Name -notlike "*backup*" -and $_.Name -notlike "*temp*" } |
            Sort-Object Name
        Write-Status "Testing all views: $($viewFiles.Count) files"
    }

    if ($viewFiles.Count -eq 0) {
        Write-Error "No view files found"
        return
    }

    # Test all view files
    $allResults = @{
        ValidationDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        ValidationTarget = "UI Element Brush Combinations"
        ViewResults = @()
        Summary = @{
            TotalViews = $viewFiles.Count
            SafeViews = 0
            ViewsWithWarnings = 0
            ViewsWithCriticalIssues = 0
            TotalCriticalIssues = 0
            TotalHighWarnings = 0
            TotalMediumWarnings = 0
        }
    }

    Write-Host "🧪 TESTING VIEW FILES:" -ForegroundColor Yellow
    Write-Host "======================" -ForegroundColor Yellow
    Write-Host ""

    foreach ($file in $viewFiles) {
        $result = Test-ViewFile $file.FullName
        $allResults.ViewResults += $result
        
        # Update summary
        switch ($result.OverallStatus) {
            "CRITICAL" { 
                $allResults.Summary.ViewsWithCriticalIssues++
                Write-Critical "$($result.ViewName) - CRITICAL ISSUES ($($result.CriticalIssues.Count) forbidden combinations)"
            }
            "HIGH" { 
                $allResults.Summary.ViewsWithWarnings++
                Write-Warning "$($result.ViewName) - HIGH WARNINGS ($($result.HighWarnings.Count) potential issues)"
            }
            "MEDIUM" { 
                $allResults.Summary.ViewsWithWarnings++
                Write-Warning "$($result.ViewName) - MEDIUM WARNINGS ($($result.MediumWarnings.Count) minor issues)"
            }
            "SAFE" { 
                $allResults.Summary.SafeViews++
                Write-Success "$($result.ViewName) - SAFE ($($result.TotalCombinations) combinations validated)"
            }
        }
        
        $allResults.Summary.TotalCriticalIssues += $result.CriticalIssues.Count
        $allResults.Summary.TotalHighWarnings += $result.HighWarnings.Count  
        $allResults.Summary.TotalMediumWarnings += $result.MediumWarnings.Count
        
        if ($VerboseOutput -and ($result.CriticalIssues.Count -gt 0 -or $result.HighWarnings.Count -gt 0)) {
            Write-Host "   Details:" -ForegroundColor Gray
            foreach ($issue in $result.CriticalIssues) {
                Write-Host "   🚨 Line $($issue.LineNumber): $($issue.ForegroundBrush) on $($issue.BackgroundBrush) ($($issue.ElementType))" -ForegroundColor Red
            }
            foreach ($warning in $result.HighWarnings) {
                Write-Host "   ⚠️  Line $($warning.LineNumber): $($warning.ForegroundBrush) on $($warning.BackgroundBrush) ($($warning.ElementType))" -ForegroundColor Yellow
            }
        }
        
        Write-Host ""
    }

    # Summary report
    Write-Host "📊 BRUSH COMBINATION VALIDATION SUMMARY" -ForegroundColor Cyan
    Write-Host "=======================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Views Tested: $($allResults.Summary.TotalViews)" -ForegroundColor White
    Write-Host "✅ Safe Views: $($allResults.Summary.SafeViews)" -ForegroundColor Green
    Write-Host "⚠️  Views with Warnings: $($allResults.Summary.ViewsWithWarnings)" -ForegroundColor Yellow
    Write-Host "🚨 Views with Critical Issues: $($allResults.Summary.ViewsWithCriticalIssues)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Critical Issues: $($allResults.Summary.TotalCriticalIssues)" -ForegroundColor $(if ($allResults.Summary.TotalCriticalIssues -eq 0) { 'Green' } else { 'Red' })
    Write-Host "High Warnings: $($allResults.Summary.TotalHighWarnings)" -ForegroundColor Yellow
    Write-Host "Medium Warnings: $($allResults.Summary.TotalMediumWarnings)" -ForegroundColor Yellow
    
    # Export results
    $allResults | ConvertTo-Json -Depth 10 | Out-File $OutputPath -Encoding UTF8
    Write-Host ""
    Write-Status "Detailed report saved to: $OutputPath"
    
    if ($allResults.Summary.TotalCriticalIssues -eq 0) {
        Write-Success "🎉 NO CRITICAL BRUSH COMBINATION ISSUES FOUND!"
    } else {
        Write-Critical "⚠️  $($allResults.Summary.TotalCriticalIssues) critical brush combination issues need attention"
    }
    
    # Interactive mode for detailed analysis
    if ($InteractiveMode -and $allResults.Summary.TotalCriticalIssues -gt 0) {
        Write-Host ""
        Write-Host "🔍 CRITICAL ISSUES DETAIL:" -ForegroundColor Red
        Write-Host "==========================" -ForegroundColor Red
        
        foreach ($viewResult in $allResults.ViewResults) {
            if ($viewResult.CriticalIssues.Count -gt 0) {
                Write-Host ""
                Write-Host "$($viewResult.ViewName):" -ForegroundColor Yellow
                foreach ($issue in $viewResult.CriticalIssues) {
                    Write-Host "  🚨 Line $($issue.LineNumber): $($issue.ElementType)" -ForegroundColor Red
                    Write-Host "     Combination: $($issue.ForegroundBrush) on $($issue.BackgroundBrush)" -ForegroundColor Red
                    foreach ($issueDesc in $issue.Issues) {
                        Write-Host "     Issue: $issueDesc" -ForegroundColor Red
                    }
                    foreach ($recommendation in $issue.Recommendations) {
                        Write-Host "     Suggestion: $recommendation" -ForegroundColor Green
                    }
                }
            }
        }
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "View brush validation failed: $($_.Exception.Message)"
    exit 1
}
