#!/usr/bin/env pwsh
# =============================================================================
# MTM WCAG 2.1 AA Compliance Validation Script
# Comprehensive color contrast testing for all theme combinations
# =============================================================================

param(
    [string]$ThemePath = "../Resources/Themes",
    [string]$OutputPath = "wcag-validation-report.json",
    [switch]$VerboseOutput,
    [switch]$InteractiveMode,
    [string]$TargetTheme = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# WCAG 2.1 AA Standards
$WCAG = @{
    NormalTextMinContrast = 4.5
    LargeTextMinContrast = 3.0
    NonTextMinContrast = 3.0
    EnhancedAAMinContrast = 7.0
}

# Critical color pair combinations for manufacturing UI
$CRITICAL_PAIRS = @(
    @{ Foreground = "MTM_Shared_Logic.HeadingText"; Background = "MTM_Shared_Logic.MainBackground"; Context = "Page Headers" }
    @{ Foreground = "MTM_Shared_Logic.BodyText"; Background = "MTM_Shared_Logic.ContentAreas"; Context = "Content Text" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.PrimaryAction"; Context = "Primary Buttons" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.SecondaryAction"; Context = "Secondary Buttons" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.Critical"; Context = "Critical Alerts" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.Warning"; Context = "Warning Messages" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.SuccessBrush"; Context = "Success Indicators" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.TransactionInBrush"; Context = "IN Transactions" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.TransactionOutBrush"; Context = "OUT Transactions" }
    @{ Foreground = "MTM_Shared_Logic.OverlayTextBrush"; Background = "MTM_Shared_Logic.TransactionTransferBrush"; Context = "Transfer Operations" }
    @{ Foreground = "MTM_Shared_Logic.InteractiveText"; Background = "MTM_Shared_Logic.ContentAreas"; Context = "Interactive Links" }
    @{ Foreground = "MTM_Shared_Logic.TertiaryTextBrush"; Background = "MTM_Shared_Logic.MainBackground"; Context = "Secondary Text" }
    @{ Foreground = "MTM_Shared_Logic.HeadingText"; Background = "MTM_Shared_Logic.CardBackgroundBrush"; Context = "Card Headers" }
    @{ Foreground = "MTM_Shared_Logic.BodyText"; Background = "MTM_Shared_Logic.CardBackgroundBrush"; Context = "Card Content" }
)

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

function Convert-HexToRgb($HexColor) {
    $hex = $HexColor -replace '^#', ''
    
    if ($hex.Length -eq 3) {
        # Convert shorthand hex (e.g., #F0F) to full hex
        $hex = $hex[0] + $hex[0] + $hex[1] + $hex[1] + $hex[2] + $hex[2]
    }
    
    if ($hex.Length -ne 6) {
        throw "Invalid hex color format: $HexColor"
    }
    
    return @{
        R = [Convert]::ToInt32($hex.Substring(0, 2), 16)
        G = [Convert]::ToInt32($hex.Substring(2, 2), 16)
        B = [Convert]::ToInt32($hex.Substring(4, 2), 16)
    }
}

function Get-RelativeLuminance($Rgb) {
    function Get-SrgbComponent($Component) {
        $sRGB = $Component / 255.0
        if ($sRGB -le 0.03928) {
            return $sRGB / 12.92
        } else {
            return [Math]::Pow(($sRGB + 0.055) / 1.055, 2.4)
        }
    }
    
    $r = Get-SrgbComponent $Rgb.R
    $g = Get-SrgbComponent $Rgb.G
    $b = Get-SrgbComponent $Rgb.B
    
    return 0.2126 * $r + 0.7152 * $g + 0.0722 * $b
}

function Calculate-ContrastRatio($Color1Hex, $Color2Hex) {
    try {
        $rgb1 = Convert-HexToRgb $Color1Hex
        $rgb2 = Convert-HexToRgb $Color2Hex
        
        $l1 = Get-RelativeLuminance $rgb1
        $l2 = Get-RelativeLuminance $rgb2
        
        $lighter = [Math]::Max($l1, $l2)
        $darker = [Math]::Min($l1, $l2)
        
        return ($lighter + 0.05) / ($darker + 0.05)
    } catch {
        Write-Warning "Failed to calculate contrast ratio for $Color1Hex vs $Color2Hex"
        return 0
    }
}

function Get-WcagCompliance($ContrastRatio) {
    $compliance = @{
        AANormal = $ContrastRatio -ge $WCAG.NormalTextMinContrast
        AALarge = $ContrastRatio -ge $WCAG.LargeTextMinContrast
        AAANormal = $ContrastRatio -ge $WCAG.EnhancedAAMinContrast
        NonText = $ContrastRatio -ge $WCAG.NonTextMinContrast
        Score = ""
        Level = ""
    }
    
    if ($compliance.AAANormal) {
        $compliance.Score = "AAA"
        $compliance.Level = "Enhanced"
    } elseif ($compliance.AANormal) {
        $compliance.Score = "AA"
        $compliance.Level = "Standard"
    } elseif ($compliance.AALarge -or $compliance.NonText) {
        $compliance.Score = "AA Large/Non-Text"
        $compliance.Level = "Limited"
    } else {
        $compliance.Score = "FAIL"
        $compliance.Level = "Non-Compliant"
    }
    
    return $compliance
}

function Extract-ColorFromTheme($ThemeContent, $BrushKey) {
    # Match SolidColorBrush definitions
    $pattern = '<SolidColorBrush\s+x:Key="' + [regex]::Escape($BrushKey) + '"\s+Color="([^"]+)"'
    $match = [regex]::Match($ThemeContent, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    
    if ($match.Success) {
        return $match.Groups[1].Value
    }
    
    # Try LinearGradientBrush (extract first gradient stop)
    $gradientPattern = '<LinearGradientBrush\s+x:Key="' + [regex]::Escape($BrushKey) + '"[^>]*>.*?<GradientStop\s+Color="([^"]+)"[^>]*>.*?</LinearGradientBrush>'
    $gradientMatch = [regex]::Match($ThemeContent, $gradientPattern, [System.Text.RegularExpressions.RegexOptions]::Singleline -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    
    if ($gradientMatch.Success) {
        return $gradientMatch.Groups[1].Value
    }
    
    return $null
}

function Test-ThemeWcagCompliance($ThemeFile, $ThemeName) {
    Write-Status "Testing WCAG compliance for $ThemeName..."
    
    $themeContent = Get-Content $ThemeFile -Raw
    $results = @{
        ThemeName = $ThemeName
        ThemeFile = $ThemeFile
        TestResults = @()
        CriticalFailures = @()
        OverallCompliance = @{
            PassedTests = 0
            FailedTests = 0
            TotalTests = 0
            CompliancePercentage = 0
            MinimumCompliance = $true
        }
    }
    
    foreach ($pair in $CRITICAL_PAIRS) {
        $foregroundColor = Extract-ColorFromTheme $themeContent $pair.Foreground
        $backgroundColor = Extract-ColorFromTheme $themeContent $pair.Background
        
        $testResult = @{
            Context = $pair.Context
            ForegroundBrush = $pair.Foreground
            BackgroundBrush = $pair.Background
            ForegroundColor = $foregroundColor
            BackgroundColor = $backgroundColor
            ContrastRatio = 0
            Compliance = $null
            Issues = @()
        }
        
        if (-not $foregroundColor) {
            $testResult.Issues += "Missing foreground brush: $($pair.Foreground)"
        }
        
        if (-not $backgroundColor) {
            $testResult.Issues += "Missing background brush: $($pair.Background)"
        }
        
        if ($foregroundColor -and $backgroundColor) {
            $testResult.ContrastRatio = Calculate-ContrastRatio $foregroundColor $backgroundColor
            $testResult.Compliance = Get-WcagCompliance $testResult.ContrastRatio
            
            if ($testResult.Compliance.Level -eq "Non-Compliant") {
                $results.CriticalFailures += @{
                    Context = $pair.Context
                    ContrastRatio = [Math]::Round($testResult.ContrastRatio, 2)
                    Required = $WCAG.NormalTextMinContrast
                    ForegroundColor = $foregroundColor
                    BackgroundColor = $backgroundColor
                }
                $results.OverallCompliance.MinimumCompliance = $false
                Write-Critical "WCAG FAILURE: $($pair.Context) - Contrast $([Math]::Round($testResult.ContrastRatio, 2)):1 (Required: $($WCAG.NormalTextMinContrast):1)"
            } elseif ($testResult.Compliance.Level -eq "Enhanced") {
                Write-Success "$($pair.Context) - Excellent contrast $([Math]::Round($testResult.ContrastRatio, 2)):1 (AAA compliant)"
            } elseif ($testResult.Compliance.Level -eq "Standard") {
                Write-Success "$($pair.Context) - Good contrast $([Math]::Round($testResult.ContrastRatio, 2)):1 (AA compliant)"
            } else {
                Write-Warning "$($pair.Context) - Limited compliance $([Math]::Round($testResult.ContrastRatio, 2)):1"
            }
            
            if ($testResult.Compliance.Score -ne "FAIL") {
                $results.OverallCompliance.PassedTests++
            } else {
                $results.OverallCompliance.FailedTests++
            }
        } else {
            $results.OverallCompliance.FailedTests++
        }
        
        $results.OverallCompliance.TotalTests++
        $results.TestResults += $testResult
    }
    
    $results.OverallCompliance.CompliancePercentage = 
        if ($results.OverallCompliance.TotalTests -gt 0) {
            [Math]::Round(($results.OverallCompliance.PassedTests / $results.OverallCompliance.TotalTests) * 100, 1)
        } else { 0 }
    
    return $results
}

function Main {
    Write-Host ""
    Write-Host "🌈 MTM WCAG 2.1 AA COMPLIANCE VALIDATION" -ForegroundColor Cyan
    Write-Host "=========================================" -ForegroundColor Cyan
    Write-Host ""

    # Resolve theme path
    $themePath = Resolve-Path $ThemePath -ErrorAction SilentlyContinue
    if (-not $themePath) {
        Write-Error "Theme path not found: $ThemePath"
        return
    }

    Write-Status "Theme Directory: $themePath"
    Write-Status "WCAG Standard: 2.1 AA (4.5:1 normal text, 3.0:1 large text)"
    Write-Status "Critical UI Contexts: $($CRITICAL_PAIRS.Count) test scenarios"
    Write-Host ""

    # Get theme files
    $themeFiles = @()
    if ($TargetTheme) {
        $targetFile = Join-Path $themePath "$TargetTheme.axaml"
        if (Test-Path $targetFile) {
            $themeFiles = @(Get-Item $targetFile)
            Write-Status "Testing single theme: $TargetTheme"
        } else {
            Write-Error "Target theme not found: $targetFile"
            return
        }
    } else {
        $themeFiles = Get-ChildItem -Path $themePath -Filter "MTM_*.axaml" |
            Where-Object { $_.Name -notlike "*backup*" -and $_.Name -notlike "*Preview*" } |
            Sort-Object Name
        Write-Status "Testing all themes: $($themeFiles.Count) files"
    }

    if ($themeFiles.Count -eq 0) {
        Write-Error "No theme files found"
        return
    }

    # Test all themes
    $allResults = @{
        ValidationDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        WcagStandard = "2.1 AA"
        TestedScenarios = $CRITICAL_PAIRS.Count
        ThemeResults = @()
        Summary = @{
            TotalThemes = $themeFiles.Count
            CompliantThemes = 0
            PartiallyCompliantThemes = 0
            NonCompliantThemes = 0
            TotalCriticalFailures = 0
            AverageCompliancePercentage = 0
        }
    }

    Write-Host "🧪 TESTING THEMES:" -ForegroundColor Yellow
    Write-Host "==================" -ForegroundColor Yellow
    Write-Host ""

    foreach ($file in $themeFiles) {
        $themeName = $file.BaseName
        $results = Test-ThemeWcagCompliance $file.FullName $themeName
        $allResults.ThemeResults += $results
        
        # Categorize theme compliance
        if ($results.OverallCompliance.CompliancePercentage -eq 100 -and $results.OverallCompliance.MinimumCompliance) {
            $allResults.Summary.CompliantThemes++
            Write-Success "$themeName - FULLY COMPLIANT ($($results.OverallCompliance.CompliancePercentage)%)"
        } elseif ($results.OverallCompliance.CompliancePercentage -ge 80) {
            $allResults.Summary.PartiallyCompliantThemes++
            Write-Warning "$themeName - PARTIALLY COMPLIANT ($($results.OverallCompliance.CompliancePercentage)%)"
        } else {
            $allResults.Summary.NonCompliantThemes++
            Write-Error "$themeName - NON-COMPLIANT ($($results.OverallCompliance.CompliancePercentage)%)"
        }
        
        $allResults.Summary.TotalCriticalFailures += $results.CriticalFailures.Count
        Write-Host ""
    }

    # Calculate average
    $totalPercentage = ($allResults.ThemeResults | Measure-Object -Property { $_.OverallCompliance.CompliancePercentage } -Sum).Sum
    $allResults.Summary.AverageCompliancePercentage = 
        if ($allResults.ThemeResults.Count -gt 0) {
            [Math]::Round($totalPercentage / $allResults.ThemeResults.Count, 1)
        } else { 0 }

    # Summary report
    Write-Host "📊 WCAG COMPLIANCE SUMMARY" -ForegroundColor Cyan
    Write-Host "==========================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Themes Tested: $($allResults.Summary.TotalThemes)" -ForegroundColor White
    Write-Host "✅ Fully Compliant: $($allResults.Summary.CompliantThemes)" -ForegroundColor Green
    Write-Host "⚠️  Partially Compliant: $($allResults.Summary.PartiallyCompliantThemes)" -ForegroundColor Yellow
    Write-Host "❌ Non-Compliant: $($allResults.Summary.NonCompliantThemes)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Critical Failures: $($allResults.Summary.TotalCriticalFailures)" -ForegroundColor $(if ($allResults.Summary.TotalCriticalFailures -eq 0) { 'Green' } else { 'Red' })
    Write-Host "Average Compliance: $($allResults.Summary.AverageCompliancePercentage)%" -ForegroundColor $(
        if ($allResults.Summary.AverageCompliancePercentage -ge 90) { 'Green' }
        elseif ($allResults.Summary.AverageCompliancePercentage -ge 80) { 'Yellow' }
        else { 'Red' }
    )
    
    # Export results
    $allResults | ConvertTo-Json -Depth 10 | Out-File $OutputPath -Encoding UTF8
    Write-Host ""
    Write-Status "Detailed report saved to: $OutputPath"
    
    if ($allResults.Summary.TotalCriticalFailures -eq 0) {
        Write-Success "🎉 ALL THEMES MEET WCAG 2.1 AA MINIMUM REQUIREMENTS!"
    } else {
        Write-Critical "⚠️  $($allResults.Summary.TotalCriticalFailures) critical WCAG failures need attention"
    }
    
    # Interactive mode for detailed analysis
    if ($InteractiveMode -and $allResults.Summary.TotalCriticalFailures -gt 0) {
        Write-Host ""
        Write-Host "🔍 CRITICAL FAILURES DETAIL:" -ForegroundColor Red
        Write-Host "============================" -ForegroundColor Red
        
        foreach ($themeResult in $allResults.ThemeResults) {
            if ($themeResult.CriticalFailures.Count -gt 0) {
                Write-Host ""
                Write-Host "$($themeResult.ThemeName):" -ForegroundColor Yellow
                foreach ($failure in $themeResult.CriticalFailures) {
                    Write-Host "  ❌ $($failure.Context)" -ForegroundColor Red
                    Write-Host "     Contrast: $($failure.ContrastRatio):1 (Required: $($failure.Required):1)" -ForegroundColor Red
                    Write-Host "     Colors: $($failure.ForegroundColor) on $($failure.BackgroundColor)" -ForegroundColor Red
                }
            }
        }
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "WCAG validation failed: $($_.Exception.Message)"
    exit 1
}