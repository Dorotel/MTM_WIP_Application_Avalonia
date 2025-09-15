# Color Contrast Testing Procedure

*Automated and manual procedures for WCAG 2.1 AA contrast validation in MTM Theme Standardization*

## Overview

This document provides comprehensive procedures for testing color contrast ratios across all MTM theme files to ensure WCAG 2.1 AA compliance (minimum 4.5:1 contrast ratio).

**Target**: Validate contrast ratios across 22 theme files and 64 view files  
**Tools**: C# ContrastValidator utility + PowerShell automation  
**Standard**: WCAG 2.1 AA (4.5:1 normal text, 3:1 large text)

## Automated Contrast Validation Tool

### ContrastValidator C# Utility Class

```csharp
using System;
using System.Drawing;

namespace MTM_WIP_Application_Avalonia.Utils
{
    /// <summary>
    /// WCAG 2.1 AA contrast ratio validation utility
    /// Implements W3C contrast calculation standards
    /// </summary>
    public class ContrastValidator 
    {
        /// <summary>
        /// Calculate contrast ratio between two colors
        /// Returns ratio as decimal (e.g., 4.5 for 4.5:1)
        /// </summary>
        public static double CalculateContrastRatio(Color foreground, Color background)
        {
            var lum1 = CalculateRelativeLuminance(foreground);
            var lum2 = CalculateRelativeLuminance(background);
            
            var lighter = Math.Max(lum1, lum2);
            var darker = Math.Min(lum1, lum2);
            
            return (lighter + 0.05) / (darker + 0.05);
        }
        
        /// <summary>
        /// Check if color combination meets WCAG 2.1 AA standards
        /// </summary>
        public static bool MeetsWCAG_AA(Color foreground, Color background, bool isLargeText = false)
        {
            var ratio = CalculateContrastRatio(foreground, background);
            return isLargeText ? ratio >= 3.0 : ratio >= 4.5;
        }
        
        /// <summary>
        /// Check if color combination meets WCAG 2.1 AAA standards  
        /// </summary>
        public static bool MeetsWCAG_AAA(Color foreground, Color background, bool isLargeText = false)
        {
            var ratio = CalculateContrastRatio(foreground, background);
            return isLargeText ? ratio >= 4.5 : ratio >= 7.0;
        }
        
        /// <summary>
        /// Calculate relative luminance according to WCAG formula
        /// </summary>
        private static double CalculateRelativeLuminance(Color color)
        {
            // Convert RGB to linear values
            var r = GetLinearValue(color.R / 255.0);
            var g = GetLinearValue(color.G / 255.0);  
            var b = GetLinearValue(color.B / 255.0);
            
            // Calculate relative luminance
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }
        
        /// <summary>
        /// Convert color channel to linear value for luminance calculation
        /// </summary>
        private static double GetLinearValue(double colorChannel)
        {
            if (colorChannel <= 0.03928)
                return colorChannel / 12.92;
            else
                return Math.Pow((colorChannel + 0.055) / 1.055, 2.4);
        }
        
        /// <summary>
        /// Parse hex color string to Color object
        /// </summary>
        public static Color ParseHexColor(string hex)
        {
            hex = hex.TrimStart('#');
            
            if (hex.Length == 6)
            {
                var r = Convert.ToInt32(hex.Substring(0, 2), 16);
                var g = Convert.ToInt32(hex.Substring(2, 2), 16);  
                var b = Convert.ToInt32(hex.Substring(4, 2), 16);
                return Color.FromArgb(r, g, b);
            }
            else if (hex.Length == 8)
            {
                var a = Convert.ToInt32(hex.Substring(0, 2), 16);
                var r = Convert.ToInt32(hex.Substring(2, 2), 16);
                var g = Convert.ToInt32(hex.Substring(4, 2), 16);
                var b = Convert.ToInt32(hex.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            
            throw new ArgumentException($"Invalid hex color format: {hex}");
        }
        
        /// <summary>
        /// Generate detailed contrast report for color combination
        /// </summary>
        public static ContrastReport GenerateReport(Color foreground, Color background, string description = "")
        {
            var ratio = CalculateContrastRatio(foreground, background);
            
            return new ContrastReport
            {
                Description = description,
                Foreground = foreground,
                Background = background,
                ContrastRatio = ratio,
                MeetsAA = MeetsWCAG_AA(foreground, background),
                MeetsAAA = MeetsWCAG_AAA(foreground, background),
                MeetsAA_Large = MeetsWCAG_AA(foreground, background, true),
                MeetsAAA_Large = MeetsWCAG_AAA(foreground, background, true)
            };
        }
    }
    
    /// <summary>
    /// Detailed contrast validation report
    /// </summary>
    public class ContrastReport
    {
        public string Description { get; set; } = string.Empty;
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public double ContrastRatio { get; set; }
        public bool MeetsAA { get; set; }
        public bool MeetsAAA { get; set; }
        public bool MeetsAA_Large { get; set; }
        public bool MeetsAAA_Large { get; set; }
        
        public string GetGrade()
        {
            if (MeetsAAA) return "AAA";
            if (MeetsAA) return "AA";
            return "FAIL";
        }
        
        public override string ToString()
        {
            return $"{Description}: {ContrastRatio:F2}:1 ({GetGrade()})";
        }
    }
}
```

### Theme Color Extraction Utility

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MTM_WIP_Application_Avalonia.Utils
{
    /// <summary>
    /// Extract colors from MTM theme files for validation
    /// </summary>
    public class ThemeColorExtractor
    {
        private static readonly Regex BrushPattern = new Regex(
            @"<SolidColorBrush\s+x:Key=""([^""]+)""\s+Color=""([^""]+)""",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
            
        /// <summary>
        /// Extract all brush colors from theme file
        /// </summary>
        public static Dictionary<string, Color> ExtractThemeColors(string themePath)
        {
            var colors = new Dictionary<string, Color>();
            
            if (!File.Exists(themePath))
            {
                throw new FileNotFoundException($"Theme file not found: {themePath}");
            }
            
            var content = File.ReadAllText(themePath);
            var matches = BrushPattern.Matches(content);
            
            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var colorValue = match.Groups[2].Value;
                
                try
                {
                    var color = ContrastValidator.ParseHexColor(colorValue);
                    colors[key] = color;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not parse color '{colorValue}' for key '{key}': {ex.Message}");
                }
            }
            
            return colors;
        }
        
        /// <summary>
        /// Validate all critical color combinations in theme
        /// </summary>
        public static List<ContrastReport> ValidateThemeContrast(string themePath)
        {
            var colors = ExtractThemeColors(themePath);
            var reports = new List<ContrastReport>();
            
            // Define critical color combinations
            var combinations = new[]
            {
                // Text combinations
                ("MTM_Shared_Logic.HeadingText", "MTM_Shared_Logic.MainBackground", "Header text"),
                ("MTM_Shared_Logic.BodyText", "MTM_Shared_Logic.CardBackgroundBrush", "Body text"),
                ("MTM_Shared_Logic.InteractiveText", "MTM_Shared_Logic.MainBackground", "Link text"),
                
                // Button combinations  
                ("MTM_Shared_Logic.OverlayTextBrush", "MTM_Shared_Logic.PrimaryAction", "Primary button"),
                ("MTM_Shared_Logic.OverlayTextBrush", "MTM_Shared_Logic.SecondaryAction", "Secondary button"),
                ("MTM_Shared_Logic.OverlayTextBrush", "MTM_Shared_Logic.PrimaryHoverBrush", "Button hover"),
                
                // Semantic combinations
                ("MTM_Shared_Logic.MainBackground", "MTM_Shared_Logic.SuccessBrush", "Success indicator"),
                ("MTM_Shared_Logic.MainBackground", "MTM_Shared_Logic.WarningBrush", "Warning indicator"),
                ("MTM_Shared_Logic.MainBackground", "MTM_Shared_Logic.ErrorBrush", "Error indicator"),
                
                // Navigation combinations
                ("MTM_Shared_Logic.OverlayTextBrush", "MTM_Shared_Logic.DarkNavigation", "Navigation text"),
            };
            
            foreach (var (fgKey, bgKey, description) in combinations)
            {
                if (colors.TryGetValue(fgKey, out var fg) && colors.TryGetValue(bgKey, out var bg))
                {
                    var report = ContrastValidator.GenerateReport(fg, bg, description);
                    reports.Add(report);
                }
                else
                {
                    Console.WriteLine($"Warning: Missing colors for combination {fgKey} + {bgKey}");
                }
            }
            
            return reports;
        }
    }
}
```

## PowerShell Automation Script

### Theme Contrast Validation Script

```powershell
# validate-theme-contrast.ps1
# Automated contrast validation for all MTM themes

param(
    [string]$ThemesPath = "Resources\Themes",
    [string]$OutputPath = "contrast-validation-report.json",
    [switch]$VerboseOutput = $false
)

$results = @{
    ValidationDate = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    ThemesValidated = @()
    OverallSummary = @{
        TotalThemes = 0
        PassingThemes = 0  
        FailingThemes = 0
        TotalCombinations = 0
        PassingCombinations = 0
        FailingCombinations = 0
    }
    FailedCombinations = @()
}

# Function to extract colors from theme file
function Extract-ThemeColors {
    param([string]$ThemePath)
    
    $colors = @{}
    $content = Get-Content $ThemePath -Raw
    
    # Match SolidColorBrush definitions
    $pattern = '<SolidColorBrush\s+x:Key="([^"]+)"\s+Color="([^"]+)"'
    $matches = [regex]::Matches($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
    
    foreach ($match in $matches) {
        $key = $match.Groups[1].Value
        $colorValue = $match.Groups[2].Value
        $colors[$key] = $colorValue
    }
    
    return $colors
}

# Function to calculate contrast ratio
function Calculate-ContrastRatio {
    param([string]$Foreground, [string]$Background)
    
    # Parse hex colors
    $fg = [System.Drawing.ColorTranslator]::FromHtml($Foreground)
    $bg = [System.Drawing.ColorTranslator]::FromHtml($Background)
    
    # Calculate relative luminance
    $fgLum = Get-RelativeLuminance $fg
    $bgLum = Get-RelativeLuminance $bg
    
    # Calculate contrast ratio
    $lighter = [Math]::Max($fgLum, $bgLum)
    $darker = [Math]::Min($fgLum, $bgLum)
    
    return ($lighter + 0.05) / ($darker + 0.05)
}

function Get-RelativeLuminance {
    param([System.Drawing.Color]$Color)
    
    $r = Get-LinearValue ($Color.R / 255.0)
    $g = Get-LinearValue ($Color.G / 255.0)  
    $b = Get-LinearValue ($Color.B / 255.0)
    
    return 0.2126 * $r + 0.7152 * $g + 0.0722 * $b
}

function Get-LinearValue {
    param([double]$Value)
    
    if ($Value -le 0.03928) {
        return $Value / 12.92
    } else {
        return [Math]::Pow(($Value + 0.055) / 1.055, 2.4)
    }
}

# Critical color combinations to validate
$criticalCombinations = @(
    @{ Fg = "MTM_Shared_Logic.HeadingText"; Bg = "MTM_Shared_Logic.MainBackground"; Name = "Header text" },
    @{ Fg = "MTM_Shared_Logic.BodyText"; Bg = "MTM_Shared_Logic.CardBackgroundBrush"; Name = "Body text" },
    @{ Fg = "MTM_Shared_Logic.OverlayTextBrush"; Bg = "MTM_Shared_Logic.PrimaryAction"; Name = "Primary button" },
    @{ Fg = "MTM_Shared_Logic.OverlayTextBrush"; Bg = "MTM_Shared_Logic.SecondaryAction"; Name = "Secondary button" },
    @{ Fg = "MTM_Shared_Logic.InteractiveText"; Bg = "MTM_Shared_Logic.MainBackground"; Name = "Link text" }
)

# Validate all theme files
Get-ChildItem -Path $ThemesPath -Filter "MTM_*.axaml" | ForEach-Object {
    $themeName = $_.BaseName
    $themeColors = Extract-ThemeColors $_.FullName
    
    $themeResult = @{
        ThemeName = $themeName
        FilePath = $_.FullName  
        ValidationStatus = "PASS"
        Combinations = @()
        FailedCombinations = 0
        TotalCombinations = 0
    }
    
    foreach ($combo in $criticalCombinations) {
        $fgColor = $themeColors[$combo.Fg]
        $bgColor = $themeColors[$combo.Bg]
        
        if ($fgColor -and $bgColor) {
            $ratio = Calculate-ContrastRatio $fgColor $bgColor
            $passes = $ratio -ge 4.5
            
            $comboResult = @{
                Name = $combo.Name
                Foreground = $fgColor
                Background = $bgColor  
                ContrastRatio = [Math]::Round($ratio, 2)
                MeetsWCAG_AA = $passes
                Status = if ($passes) { "PASS" } else { "FAIL" }
            }
            
            $themeResult.Combinations += $comboResult
            $themeResult.TotalCombinations++
            $results.OverallSummary.TotalCombinations++
            
            if ($passes) {
                $results.OverallSummary.PassingCombinations++
            } else {
                $themeResult.FailedCombinations++
                $themeResult.ValidationStatus = "FAIL"
                $results.OverallSummary.FailingCombinations++
                
                $results.FailedCombinations += @{
                    Theme = $themeName
                    Combination = $combo.Name  
                    ContrastRatio = [Math]::Round($ratio, 2)
                    RequiredRatio = 4.5
                }
            }
            
            if ($VerboseOutput) {
                Write-Host "$themeName - $($combo.Name): $($ratio.ToString("F2")):1 ($($comboResult.Status))"
            }
        } else {
            Write-Warning "Missing colors in $themeName for combination: $($combo.Name)"
        }
    }
    
    $results.ThemesValidated += $themeResult
    $results.OverallSummary.TotalThemes++
    
    if ($themeResult.ValidationStatus -eq "PASS") {
        $results.OverallSummary.PassingThemes++
        Write-Host "✅ $themeName - All contrast ratios pass WCAG 2.1 AA" -ForegroundColor Green
    } else {
        $results.OverallSummary.FailingThemes++
        Write-Host "❌ $themeName - $($themeResult.FailedCombinations) combinations fail WCAG 2.1 AA" -ForegroundColor Red
    }
}

# Output results
$results | ConvertTo-Json -Depth 4 | Out-File $OutputPath

Write-Host "`n📊 CONTRAST VALIDATION SUMMARY" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host "Total themes validated: $($results.OverallSummary.TotalThemes)"
Write-Host "Themes passing: $($results.OverallSummary.PassingThemes)" -ForegroundColor Green  
Write-Host "Themes failing: $($results.OverallSummary.FailingThemes)" -ForegroundColor Red
Write-Host "Total combinations tested: $($results.OverallSummary.TotalCombinations)"
Write-Host "Combinations passing: $($results.OverallSummary.PassingCombinations)" -ForegroundColor Green
Write-Host "Combinations failing: $($results.OverallSummary.FailingCombinations)" -ForegroundColor Red

if ($results.FailedCombinations.Count -gt 0) {
    Write-Host "`n❌ FAILED COMBINATIONS:" -ForegroundColor Red
    foreach ($failure in $results.FailedCombinations) {
        Write-Host "  $($failure.Theme) - $($failure.Combination): $($failure.ContrastRatio):1 (Required: $($failure.RequiredRatio):1)" -ForegroundColor Red
    }
}

Write-Host "`nReport saved to: $OutputPath" -ForegroundColor Cyan
```

## Manual Testing Procedures

### Visual Contrast Verification

1. **Theme Switching Test**:
   - Switch between all 22 themes in ThemeBuilderView
   - Visually inspect text readability in each theme
   - Document any themes with poor readability

2. **Component State Testing**:
   - Test button hover/pressed states across themes
   - Verify focus indicators are visible
   - Check disabled state visibility

3. **Cross-Browser Validation**:
   - Test themes in different operating system themes (light/dark)
   - Verify high contrast mode compatibility
   - Check color blindness accessibility

### Documentation Template

```yaml
ManualContrastTest:
  Tester: "[Name]"
  Date: "[YYYY-MM-DD]"
  Environment: "[OS/Browser details]"
  
  ThemeTests:
    - ThemeName: "MTM_Blue"
      VisualReadability: "✅ Excellent | ⚠️ Acceptable | ❌ Poor"
      ButtonStates: "✅ Clear | ❌ Unclear"  
      FocusIndicators: "✅ Visible | ❌ Invisible"
      Notes: ""
      
  HighContrastMode: "✅/❌ Compatible"
  ColorBlindnessCheck: "✅/❌ Accessible"
```

## Integration with Validation Pipeline

### Automated CI/CD Integration

```yaml
# GitHub Actions workflow step
- name: Validate Theme Contrast
  run: |
    pwsh -File scripts/validate-theme-contrast.ps1 -VerboseOutput
    if (Test-Path "contrast-validation-report.json") {
      $report = Get-Content "contrast-validation-report.json" | ConvertFrom-Json
      if ($report.OverallSummary.FailingThemes -gt 0) {
        exit 1  # Fail the build
      }
    }
```

### Integration with WCAG Checklist

This contrast testing procedure integrates with the WCAG validation checklist:

1. **Automated Results**: Import PowerShell validation results into checklist
2. **Manual Verification**: Complete visual testing sections  
3. **Report Generation**: Combine automated and manual results
4. **Continuous Validation**: Run on every theme file change

## Success Criteria

### Theme File Validation
- [ ] All critical color combinations meet 4.5:1 contrast ratio
- [ ] High contrast theme meets 7:1 ratio (WCAG AAA)
- [ ] No manual accessibility issues identified
- [ ] Automated validation passes for all themes

### Process Validation  
- [ ] Automated contrast testing pipeline functional
- [ ] Manual testing procedures documented and validated
- [ ] Integration with development workflow complete
- [ ] Results integration with validation checklist working

---

**Document Status**: ✅ Complete Contrast Testing Framework  
**Created**: [Current Date]
**Validation Tools**: C# ContrastValidator + PowerShell Automation  
**Testing Framework Owner**: MTM Development Team