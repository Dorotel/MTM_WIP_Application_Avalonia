# MTM Theme Synchronization Script
# This script uses MTM_Amber.axaml as the master template and updates all other themes
# to have the same structure with theme-appropriate colors
# Run from the solution root directory

param(
    [string]$ThemesPath = "C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Resources\Themes",
    [string]$MasterTheme = "MTM_Amber.axaml",
    [switch]$WhatIf = $false,
    [switch]$Verbose = $false
)

Write-Host "MTM Theme Synchronization Script" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

if ($WhatIf) {
    Write-Host "PREVIEW MODE - No files will be modified" -ForegroundColor Yellow
    Write-Host ""
}

# Determine the correct path to themes
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = Split-Path -Parent $scriptDir
$fullThemesPath = Join-Path $solutionRoot $ThemesPath
$masterThemePath = Join-Path $fullThemesPath $MasterTheme

# Fallback to current directory if running from solution root
if (-not (Test-Path $fullThemesPath)) {
    $fullThemesPath = $ThemesPath
    $masterThemePath = Join-Path $fullThemesPath $MasterTheme
}

Write-Host "Script directory: $scriptDir" -ForegroundColor Gray
Write-Host "Solution root: $solutionRoot" -ForegroundColor Gray
Write-Host "Looking for themes in: $fullThemesPath" -ForegroundColor Gray
Write-Host "Master template: $masterThemePath" -ForegroundColor Gray
Write-Host ""

# Verify master theme exists
if (-not (Test-Path $masterThemePath)) {
    Write-Host "ERROR: Master theme not found at: $masterThemePath" -ForegroundColor Red
    exit 1
}

# Theme-specific color mappings
$ThemeColorMappings = @{
    "MTM_Blue" = @{
        Title = "MTM Professional Blue Theme"
        Subtitle = "Maximum Robustness Edition - Corporate Professional Aesthetic"
        Badge1 = "Corporate Grade"
        Badge2 = "Premium Effects"
        Badge3 = "Complete Coverage"
        SectionTitle = "Complete Professional Blue Palette"
        SpecializedTitle = "Specialized Professional Blue Collection"
        ComponentTitle = "Corporate Manufacturing Components"
        NavigationTitle = "Corporate Navigation System"
        FooterTitle = "MTM Professional Blue Theme - Maximum Robustness Edition"
        FooterSubtitle = "Complete corporate-grade theme system with 50+ specialized brushes and premium effects"
        Colors = @{
            "MTM_Shared_Logic.PrimaryAction" = "#1E88E5"
            "MTM_Shared_Logic.SecondaryAction" = "#42A5F5"
            "MTM_Shared_Logic.Warning" = "#5C6BC0"
            "MTM_Shared_Logic.Status" = "#1976D2"
            "MTM_Shared_Logic.Critical" = "#3F51B5"
            "MTM_Shared_Logic.Highlight" = "#90CAF9"
            "MTM_Shared_Logic.DarkNavigation" = "#0D47A1"
            "MTM_Shared_Logic.CardBackground" = "#E3F2FD"
            "MTM_Shared_Logic.HoverBackground" = "#F8FEFF"
            "MTM_Shared_Logic.PrimaryHoverBrush" = "#2196F3"
            "MTM_Shared_Logic.SecondaryHoverBrush" = "#64B5F6"
            "MTM_Shared_Logic.MagentaHoverBrush" = "#7986CB"
            "MTM_Shared_Logic.PrimaryPressedBrush" = "#1565C0"
            "MTM_Shared_Logic.SecondaryPressedBrush" = "#1976D2"
            "MTM_Shared_Logic.MagentaPressedBrush" = "#303F9F"
            "MTM_Shared_Logic.PrimaryDisabledBrush" = "#90CAF9"
            "MTM_Shared_Logic.SecondaryDisabledBrush" = "#BBDEFB"
            "MTM_Shared_Logic.MainBackground" = "#F8FEFF"
            "MTM_Shared_Logic.ContentAreas" = "#FFFFFF"
            "MTM_Shared_Logic.SidebarDark" = "#0D47A1"
            "MTM_Shared_Logic.PageHeaders" = "#1E88E5"
            "MTM_Shared_Logic.FooterBackgroundBrush" = "#0D47A1"
            "MTM_Shared_Logic.StatusBarBackgroundBrush" = "#0D47A1"
            "MTM_Shared_Logic.CardBackgroundBrush" = "#FFFFFF"
            "MTM_Shared_Logic.PanelBackgroundBrush" = "#FAFAFA"
            "MTM_Shared_Logic.BorderBrush" = "#E3F2FD"
            "MTM_Shared_Logic.BorderDarkBrush" = "#BBDEFB"
            "MTM_Shared_Logic.BorderLightBrush" = "#F0F8FF"
            "MTM_Shared_Logic.BorderAccentBrush" = "#81D4FA"
            "MTM_Shared_Logic.ShadowBrush" = "#33000000"
            "MTM_Shared_Logic.DropShadowBrush" = "#660D47A1"
            "MTM_Shared_Logic.GlowBrush" = "#8042A5F5"
            "MTM_Shared_Logic.HeadingText" = "#0D47A1"
            "MTM_Shared_Logic.BodyText" = "#666666"
            "MTM_Shared_Logic.TertiaryTextBrush" = "#999999"
            "MTM_Shared_Logic.OverlayTextBrush" = "#FFFFFF"
            "MTM_Shared_Logic.InteractiveText" = "#1E88E5"
            "MTM_Shared_Logic.LinkTextBrush" = "#1976D2"
            "MTM_Shared_Logic.MutedTextBrush" = "#CCCCCC"
            "MTM_Shared_Logic.HighlightTextBrush" = "#3F51B5"
            "MTM_Shared_Logic.PlaceholderTextBrush" = "#AAAAAA"
            "MTM_Shared_Logic.Specialized1" = "#1E88E5"
            "MTM_Shared_Logic.Specialized2" = "#42A5F5"
            "MTM_Shared_Logic.Specialized3" = "#5C6BC0"
            "MTM_Shared_Logic.Specialized4" = "#0D47A1"
            "MTM_Shared_Logic.Specialized5" = "#90CAF9"
            "MTM_Shared_Logic.FocusBrush" = "#1E88E5"
            "MTM_Shared_Logic.SelectionBrush" = "#8042A5F5"
            "MTM_Shared_Logic.ActiveBrush" = "#42A5F5"
            "MTM_Shared_Logic.InactiveBrush" = "#CCCCCC"
            "MTM_Shared_Logic.LoadingBrush" = "#90CAF9"
            "MTM_Shared_Logic.ProcessingBrush" = "#5C6BC0"
        }
        Gradients = @{
            "PrimaryGradientBrush" = @("#1E88E5", "#42A5F5", "#5C6BC0")
            "HeroGradientBrush" = @("#0D47A1", "#1976D2", "#1E88E5", "#42A5F5")
            "SidebarGradientBrush" = @("#0D47A1", "#1565C0", "#1976D2")
            "CardHoverGradientBrush" = @("#F8FEFF", "#E3F2FD", "#90CAF9")
            "AccentRadialBrush" = @("#42A5F5", "#1E88E5", "#0D47A1")
        }
    }
    "MTM_Blue_Dark" = @{
        Title = "MTM Professional Blue Dark Theme"
        Subtitle = "Maximum Robustness Edition - Professional Dark Mode Experience"
        Badge1 = "Dark Mode"
        Badge2 = "Premium Effects"
        Badge3 = "Complete Coverage"
        SectionTitle = "Complete Professional Blue Dark Palette"
        SpecializedTitle = "Specialized Professional Blue Dark Collection"
        ComponentTitle = "Dark Mode Manufacturing Components"
        NavigationTitle = "Professional Dark Navigation System"
        FooterTitle = "MTM Professional Blue Dark Theme - Maximum Robustness Edition"
        FooterSubtitle = "Complete dark-mode professional theme system with 50+ specialized brushes and premium effects"
        Colors = @{
            "MTM_Shared_Logic.PrimaryAction" = "#42A5F5"
            "MTM_Shared_Logic.SecondaryAction" = "#64B5F6"
            "MTM_Shared_Logic.Warning" = "#7986CB"
            "MTM_Shared_Logic.Status" = "#1E88E5"
            "MTM_Shared_Logic.Critical" = "#1976D2"
            "MTM_Shared_Logic.Highlight" = "#90CAF9"
            "MTM_Shared_Logic.DarkNavigation" = "#01579B"
            "MTM_Shared_Logic.CardBackground" = "#1A237E"
            "MTM_Shared_Logic.HoverBackground" = "#0D1421"
            "MTM_Shared_Logic.PrimaryHoverBrush" = "#64B5F6"
            "MTM_Shared_Logic.SecondaryHoverBrush" = "#90CAF9"
            "MTM_Shared_Logic.MagentaHoverBrush" = "#9575CD"
            "MTM_Shared_Logic.PrimaryPressedBrush" = "#1E88E5"
            "MTM_Shared_Logic.SecondaryPressedBrush" = "#42A5F5"
            "MTM_Shared_Logic.MagentaPressedBrush" = "#5C6BC0"
            "MTM_Shared_Logic.PrimaryDisabledBrush" = "#90CAF9"
            "MTM_Shared_Logic.SecondaryDisabledBrush" = "#1A237E"
            "MTM_Shared_Logic.MainBackground" = "#121212"
            "MTM_Shared_Logic.ContentAreas" = "#1E1E1E"
            "MTM_Shared_Logic.SidebarDark" = "#01579B"
            "MTM_Shared_Logic.PageHeaders" = "#42A5F5"
            "MTM_Shared_Logic.FooterBackgroundBrush" = "#01579B"
            "MTM_Shared_Logic.StatusBarBackgroundBrush" = "#01579B"
            "MTM_Shared_Logic.CardBackgroundBrush" = "#2A2A2A"
            "MTM_Shared_Logic.PanelBackgroundBrush" = "#1A1A1A"
            "MTM_Shared_Logic.BorderBrush" = "#404040"
            "MTM_Shared_Logic.BorderDarkBrush" = "#303030"
            "MTM_Shared_Logic.BorderLightBrush" = "#505050"
            "MTM_Shared_Logic.BorderAccentBrush" = "#42A5F5"
            "MTM_Shared_Logic.ShadowBrush" = "#66000000"
            "MTM_Shared_Logic.DropShadowBrush" = "#8001579B"
            "MTM_Shared_Logic.GlowBrush" = "#8042A5F5"
            "MTM_Shared_Logic.HeadingText" = "#90CAF9"
            "MTM_Shared_Logic.BodyText" = "#BBBBBB"
            "MTM_Shared_Logic.TertiaryTextBrush" = "#888888"
            "MTM_Shared_Logic.OverlayTextBrush" = "#FFFFFF"
            "MTM_Shared_Logic.InteractiveText" = "#64B5F6"
            "MTM_Shared_Logic.LinkTextBrush" = "#42A5F5"
            "MTM_Shared_Logic.MutedTextBrush" = "#666666"
            "MTM_Shared_Logic.HighlightTextBrush" = "#90CAF9"
            "MTM_Shared_Logic.PlaceholderTextBrush" = "#777777"
            "MTM_Shared_Logic.Specialized1" = "#42A5F5"
            "MTM_Shared_Logic.Specialized2" = "#64B5F6"
            "MTM_Shared_Logic.Specialized3" = "#7986CB"
            "MTM_Shared_Logic.Specialized4" = "#01579B"
            "MTM_Shared_Logic.Specialized5" = "#90CAF9"
            "MTM_Shared_Logic.FocusBrush" = "#42A5F5"
            "MTM_Shared_Logic.SelectionBrush" = "#8042A5F5"
            "MTM_Shared_Logic.ActiveBrush" = "#64B5F6"
            "MTM_Shared_Logic.InactiveBrush" = "#666666"
            "MTM_Shared_Logic.LoadingBrush" = "#90CAF9"
            "MTM_Shared_Logic.ProcessingBrush" = "#7986CB"
        }
        Gradients = @{
            "PrimaryGradientBrush" = @("#42A5F5", "#64B5F6", "#7986CB")
            "HeroGradientBrush" = @("#01579B", "#1E88E5", "#42A5F5", "#64B5F6")
            "SidebarGradientBrush" = @("#01579B", "#1565C0", "#1976D2")
            "CardHoverGradientBrush" = @("#0D1421", "#1A237E", "#42A5F5")
            "AccentRadialBrush" = @("#64B5F6", "#42A5F5", "#01579B")
        }
    }
    "MTM_Red" = @{
        Title = "MTM Alert Red Theme"
        Subtitle = "Maximum Robustness Edition - High-Alert Manufacturing Environment"
        Badge1 = "Alert Grade"
        Badge2 = "Premium Effects"
        Badge3 = "Complete Coverage"
        SectionTitle = "Complete Alert Red Palette"
        SpecializedTitle = "Specialized Alert Red Collection"
        ComponentTitle = "Alert Manufacturing Components"
        NavigationTitle = "Alert Navigation System"
        FooterTitle = "MTM Alert Red Theme - Maximum Robustness Edition"
        FooterSubtitle = "Complete alert-grade theme system with 50+ specialized brushes and premium effects"
        Colors = @{
            "MTM_Shared_Logic.PrimaryAction" = "#E53935"
            "MTM_Shared_Logic.SecondaryAction" = "#EF5350"
            "MTM_Shared_Logic.Warning" = "#F44336"
            "MTM_Shared_Logic.Status" = "#D32F2F"
            "MTM_Shared_Logic.Critical" = "#C62828"
            "MTM_Shared_Logic.Highlight" = "#E57373"
            "MTM_Shared_Logic.DarkNavigation" = "#B71C1C"
            "MTM_Shared_Logic.CardBackground" = "#FFEBEE"
            "MTM_Shared_Logic.HoverBackground" = "#FFF8F8"
            "MTM_Shared_Logic.PrimaryHoverBrush" = "#F44336"
            "MTM_Shared_Logic.SecondaryHoverBrush" = "#E57373"
            "MTM_Shared_Logic.MagentaHoverBrush" = "#EF5350"
            "MTM_Shared_Logic.PrimaryPressedBrush" = "#D32F2F"
            "MTM_Shared_Logic.SecondaryPressedBrush" = "#E53935"
            "MTM_Shared_Logic.MagentaPressedBrush" = "#C62828"
            "MTM_Shared_Logic.PrimaryDisabledBrush" = "#E57373"
            "MTM_Shared_Logic.SecondaryDisabledBrush" = "#FFCDD2"
            "MTM_Shared_Logic.MainBackground" = "#FFF8F8"
            "MTM_Shared_Logic.ContentAreas" = "#FFFFFF"
            "MTM_Shared_Logic.SidebarDark" = "#B71C1C"
            "MTM_Shared_Logic.PageHeaders" = "#E53935"
            "MTM_Shared_Logic.FooterBackgroundBrush" = "#B71C1C"
            "MTM_Shared_Logic.StatusBarBackgroundBrush" = "#B71C1C"
            "MTM_Shared_Logic.CardBackgroundBrush" = "#FFFFFF"
            "MTM_Shared_Logic.PanelBackgroundBrush" = "#FAFAFA"
            "MTM_Shared_Logic.BorderBrush" = "#FFEBEE"
            "MTM_Shared_Logic.BorderDarkBrush" = "#FFCDD2"
            "MTM_Shared_Logic.BorderLightBrush" = "#FFF8F8"
            "MTM_Shared_Logic.BorderAccentBrush" = "#EF9A9A"
            "MTM_Shared_Logic.ShadowBrush" = "#33000000"
            "MTM_Shared_Logic.DropShadowBrush" = "#66B71C1C"
            "MTM_Shared_Logic.GlowBrush" = "#80F44336"
            "MTM_Shared_Logic.HeadingText" = "#B71C1C"
            "MTM_Shared_Logic.BodyText" = "#666666"
            "MTM_Shared_Logic.TertiaryTextBrush" = "#999999"
            "MTM_Shared_Logic.OverlayTextBrush" = "#FFFFFF"
            "MTM_Shared_Logic.InteractiveText" = "#E53935"
            "MTM_Shared_Logic.LinkTextBrush" = "#D32F2F"
            "MTM_Shared_Logic.MutedTextBrush" = "#CCCCCC"
            "MTM_Shared_Logic.HighlightTextBrush" = "#C62828"
            "MTM_Shared_Logic.PlaceholderTextBrush" = "#AAAAAA"
            "MTM_Shared_Logic.Specialized1" = "#E53935"
            "MTM_Shared_Logic.Specialized2" = "#EF5350"
            "MTM_Shared_Logic.Specialized3" = "#F44336"
            "MTM_Shared_Logic.Specialized4" = "#B71C1C"
            "MTM_Shared_Logic.Specialized5" = "#E57373"
            "MTM_Shared_Logic.FocusBrush" = "#E53935"
            "MTM_Shared_Logic.SelectionBrush" = "#80F44336"
            "MTM_Shared_Logic.ActiveBrush" = "#EF5350"
            "MTM_Shared_Logic.InactiveBrush" = "#CCCCCC"
            "MTM_Shared_Logic.LoadingBrush" = "#E57373"
            "MTM_Shared_Logic.ProcessingBrush" = "#F44336"
        }
        Gradients = @{
            "PrimaryGradientBrush" = @("#E53935", "#EF5350", "#F44336")
            "HeroGradientBrush" = @("#B71C1C", "#D32F2F", "#E53935", "#EF5350")
            "SidebarGradientBrush" = @("#B71C1C", "#C62828", "#D32F2F")
            "CardHoverGradientBrush" = @("#FFF8F8", "#FFEBEE", "#E57373")
            "AccentRadialBrush" = @("#F44336", "#E53935", "#B71C1C")
        }
    }
}

# Get all theme files except the master
$themeFiles = Get-ChildItem -Path $fullThemesPath -Filter "MTM_*.axaml" | 
    Where-Object { $_.Name -ne $MasterTheme -and $_.Name -ne "MTM_Base.axaml" }

if ($themeFiles.Count -eq 0) {
    Write-Host "ERROR: No theme files found to update in: $fullThemesPath" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($themeFiles.Count) theme files to synchronize with master template" -ForegroundColor Green
foreach ($file in $themeFiles) {
    Write-Host "  - $($file.Name)" -ForegroundColor Gray
}
Write-Host ""

# Read master template
$masterContent = Get-Content -Path $masterThemePath -Raw -Encoding UTF8
if ($null -eq $masterContent -or $masterContent.Length -eq 0) {
    Write-Host "ERROR: Could not read master template: $masterThemePath" -ForegroundColor Red
    exit 1
}

Write-Host "Successfully loaded master template: $MasterTheme" -ForegroundColor Green
Write-Host ""

# Track statistics
$FilesUpdated = 0
$TotalReplacements = 0

foreach ($file in $themeFiles) {
    $themeName = $file.BaseName
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    if (-not $ThemeColorMappings.ContainsKey($themeName)) {
        Write-Host "  Skipping - No color mapping defined for theme: $themeName" -ForegroundColor DarkYellow
        continue
    }
    
    $mapping = $ThemeColorMappings[$themeName]
    $newContent = $masterContent
    $replacements = 0
    
    try {
        # Replace theme-specific text content
        $newContent = $newContent -replace 'MTM Industrial Amber Theme', $mapping.Title
        $newContent = $newContent -replace 'Maximum Robustness Edition - Premium Industrial Aesthetic', $mapping.Subtitle
        $newContent = $newContent -replace 'Industrial Grade', $mapping.Badge1
        $newContent = $newContent -replace 'Complete Industrial Amber Palette', $mapping.SectionTitle
        $newContent = $newContent -replace 'Specialized Industrial Amber Collection', $mapping.SpecializedTitle
        $newContent = $newContent -replace 'Advanced Manufacturing Components', $mapping.ComponentTitle
        $newContent = $newContent -replace 'Industrial Navigation System', $mapping.NavigationTitle
        $newContent = $newContent -replace 'MTM Industrial Amber Theme - Maximum Robustness Edition', $mapping.FooterTitle
        $newContent = $newContent -replace 'Complete industrial-grade theme system with 50\+ specialized brushes and premium effects', $mapping.FooterSubtitle
        
        # Replace header comment
        $newContent = $newContent -replace 'MTM Industrial Amber Theme - Maximum Robustness Edition', "$($mapping.Title) - Maximum Robustness Edition"
        $newContent = $newContent -replace 'Complete premium theme with all visual effects and interactions', "Complete premium theme with all visual effects and interactions"
        
        # Replace all color values
        foreach ($colorKey in $mapping.Colors.Keys) {
            $newColor = $mapping.Colors[$colorKey]
            $pattern = "(`"$([regex]::Escape($colorKey))`"\s+Color=`")[^`"]+(`")"
            $replacement = "`${1}$newColor`${2}"
            $newContent = $newContent -replace $pattern, $replacement
            $replacements++
        }
        
        # Replace gradient colors
        foreach ($gradientKey in $mapping.Gradients.Keys) {
            $gradientColors = $mapping.Gradients[$gradientKey]
            
            if ($gradientKey -eq "PrimaryGradientBrush" -and $gradientColors.Count -eq 3) {
                $newContent = $newContent -replace '(<GradientStop Color=")[^"]+(" Offset="0"/>)', "`${1}$($gradientColors[0])`${2}"
                $newContent = $newContent -replace '(<GradientStop Color=")[^"]+(" Offset="0\.5"/>)', "`${1}$($gradientColors[1])`${2}"
                $newContent = $newContent -replace '(<GradientStop Color=")[^"]+(" Offset="1"/>)', "`${1}$($gradientColors[2])`${2}"
            } elseif ($gradientKey -eq "HeroGradientBrush" -and $gradientColors.Count -eq 4) {
                $pattern1 = '(<LinearGradientBrush x:Key="MTM_Shared_Logic\.HeroGradientBrush"[^>]*>\s*<GradientStop Color=")[^"]+(" Offset="0"/>[^<]*<GradientStop Color=")[^"]+(" Offset="0\.3"/>[^<]*<GradientStop Color=")[^"]+(" Offset="0\.7"/>[^<]*<GradientStop Color=")[^"]+(" Offset="1"/>)'
                $replacement1 = "`${1}$($gradientColors[0])`${2}$($gradientColors[1])`${3}$($gradientColors[2])`${4}$($gradientColors[3])`${5}"
                $newContent = $newContent -replace $pattern1, $replacement1
            }
        }
        
        # Replace specialized names to use generic names
        $newContent = $newContent -replace 'Industrial', 'Specialized 1'
        $newContent = $newContent -replace 'Burnished', 'Specialized 2'
        $newContent = $newContent -replace 'Molten', 'Specialized 3'
        $newContent = $newContent -replace 'Deep', 'Specialized 4'
        $newContent = $newContent -replace 'Crystal', 'Specialized 5'
        
        # Write updated content
        if (-not $WhatIf) {
            Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
            Write-Host "  Successfully synchronized with master template" -ForegroundColor Green
        } else {
            Write-Host "  Would synchronize with master template ($replacements color replacements)" -ForegroundColor Yellow
        }
        
        $FilesUpdated++
        $TotalReplacements += $replacements
        
        if ($Verbose) {
            Write-Host "    Color replacements made: $replacements" -ForegroundColor Gray
        }
        
    } catch {
        Write-Host "  Error processing file: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "SYNCHRONIZATION SUMMARY" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host "Master template: $MasterTheme" -ForegroundColor White
Write-Host "Files processed: $($themeFiles.Count)" -ForegroundColor White
Write-Host "Files synchronized: $FilesUpdated" -ForegroundColor Green
Write-Host "Total color replacements: $TotalReplacements" -ForegroundColor Green

if ($WhatIf) {
    Write-Host ""
    Write-Host "PREVIEW COMPLETE - No files were modified" -ForegroundColor Yellow
    Write-Host "Run without -WhatIf to apply synchronization" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "THEME SYNCHRONIZATION COMPLETE!" -ForegroundColor Green
    Write-Host "All themes now have the same structure as the master template with theme-appropriate colors" -ForegroundColor Green
}
