# MTM Theme Standardization Script
# This script ensures all MTM theme files have the complete set of brush definitions
# Run from the solution root directory

param(
    [string]$ThemesPath = "Resources/Themes",
    [switch]$WhatIf = $false,
    [switch]$Verbose = $false
)

Write-Host "MTM Theme Standardization Script" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

if ($WhatIf) {
    Write-Host "PREVIEW MODE - No files will be modified" -ForegroundColor Yellow
    Write-Host ""
}

# Determine the correct path to themes
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = Split-Path -Parent $scriptDir
$fullThemesPath = Join-Path $solutionRoot $ThemesPath

# Fallback to current directory if running from solution root
if (-not (Test-Path $fullThemesPath)) {
    $fullThemesPath = $ThemesPath
}

Write-Host "Script directory: $scriptDir" -ForegroundColor Gray
Write-Host "Solution root: $solutionRoot" -ForegroundColor Gray
Write-Host "Looking for themes in: $fullThemesPath" -ForegroundColor Gray
Write-Host ""

# Verify themes directory exists
if (-not (Test-Path $fullThemesPath)) {
    Write-Host "ERROR: Themes directory not found at: $fullThemesPath" -ForegroundColor Red
    Write-Host "Please ensure you're running this script from the solution root directory." -ForegroundColor Red
    Write-Host "Or specify the correct path with -ThemesPath parameter." -ForegroundColor Red
    exit 1
}

# Complete brush definitions that should be in every theme
$RequiredBrushes = @(
    # Core Action Colors
    "MTM_Shared_Logic.PrimaryAction",
    "MTM_Shared_Logic.SecondaryAction", 
    "MTM_Shared_Logic.Warning",
    "MTM_Shared_Logic.Status",
    "MTM_Shared_Logic.Critical",
    "MTM_Shared_Logic.Highlight",
    
    # Extended Palette
    "MTM_Shared_Logic.DarkNavigation",
    "MTM_Shared_Logic.CardBackground",
    "MTM_Shared_Logic.HoverBackground",
    
    # Interactive State Colors
    "MTM_Shared_Logic.PrimaryHoverBrush",
    "MTM_Shared_Logic.SecondaryHoverBrush",
    "MTM_Shared_Logic.MagentaHoverBrush",
    "MTM_Shared_Logic.PrimaryPressedBrush",
    "MTM_Shared_Logic.SecondaryPressedBrush",
    "MTM_Shared_Logic.MagentaPressedBrush",
    "MTM_Shared_Logic.PrimaryDisabledBrush",
    "MTM_Shared_Logic.SecondaryDisabledBrush",
    
    # UI Layout Colors
    "MTM_Shared_Logic.MainBackground",
    "MTM_Shared_Logic.ContentAreas",
    "MTM_Shared_Logic.SidebarDark",
    "MTM_Shared_Logic.PageHeaders",
    "MTM_Shared_Logic.FooterBackgroundBrush",
    "MTM_Shared_Logic.StatusBarBackgroundBrush",
    "MTM_Shared_Logic.CardBackgroundBrush",
    "MTM_Shared_Logic.PanelBackgroundBrush",
    "MTM_Shared_Logic.BorderBrush",
    "MTM_Shared_Logic.BorderDarkBrush",
    
    # Text Colors
    "MTM_Shared_Logic.HeadingText",
    "MTM_Shared_Logic.BodyText",
    "MTM_Shared_Logic.TertiaryTextBrush",
    "MTM_Shared_Logic.OverlayTextBrush",
    "MTM_Shared_Logic.InteractiveText",
    "MTM_Shared_Logic.LinkTextBrush",
    
    # Semantic Colors
    "MTM_Shared_Logic.SuccessBrush",
    "MTM_Shared_Logic.WarningBrush",
    "MTM_Shared_Logic.ErrorBrush",
    "MTM_Shared_Logic.InfoBrush"
)

# Optional gradient brushes (only some themes have these)
$OptionalBrushes = @(
    "MTM_Shared_Logic.PrimaryGradientBrush",
    "MTM_Shared_Logic.HeroGradientBrush"
)

# Get all theme files
$themeFiles = Get-ChildItem -Path $fullThemesPath -Filter "MTM_*.axaml" | Where-Object { $_.Name -ne "MTM_Base.axaml" }

if ($themeFiles.Count -eq 0) {
    Write-Host "ERROR: No MTM theme files found in: $fullThemesPath" -ForegroundColor Red
    Write-Host "Expected files like MTM_Blue.axaml, MTM_Red.axaml, etc." -ForegroundColor Red
    exit 1
}

Write-Host "Found $($themeFiles.Count) theme files to process" -ForegroundColor Green
foreach ($file in $themeFiles) {
    Write-Host "  - $($file.Name)" -ForegroundColor Gray
}
Write-Host ""

# Track statistics
$FilesModified = 0
$TotalBrushesAdded = 0

foreach ($file in $themeFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Yellow
    
    try {
        # Read file content
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        
        if ($null -eq $content -or $content.Length -eq 0) {
            Write-Host "  Skipping empty file" -ForegroundColor Gray
            continue
        }
        
        $originalContent = $content
        $brushesAdded = 0
        $missingBrushes = @()
        
        # Check which brushes are missing
        foreach ($brush in $RequiredBrushes) {
            if ($content -notmatch [regex]::Escape($brush)) {
                $missingBrushes += $brush
            }
        }
        
        if ($missingBrushes.Count -eq 0) {
            Write-Host "  All required brushes present" -ForegroundColor Green
            continue
        }
        
        Write-Host "  Missing $($missingBrushes.Count) brushes" -ForegroundColor Orange
        
        if ($Verbose) {
            foreach ($missing in $missingBrushes) {
                Write-Host "    - $missing" -ForegroundColor Gray
            }
        }
        
        # Extract theme name for color generation
        $themeName = $file.BaseName -replace "MTM_", ""
        
        # Generate missing brush definitions based on theme
        $newBrushDefinitions = GenerateMissingBrushes -ThemeName $themeName -MissingBrushes $missingBrushes -ExistingContent $content
        
        # Insert new brushes before the closing </ResourceDictionary> tag
        $insertionPoint = $content.LastIndexOf("</ResourceDictionary>")
        if ($insertionPoint -gt 0) {
            $beforeClosing = $content.Substring(0, $insertionPoint)
            $afterClosing = $content.Substring($insertionPoint)
            
            # Add comment and new brushes
            $newContent = $beforeClosing + "`n    <!-- Additional Standard Brushes Added by Standardization Script -->`n" + $newBrushDefinitions + "`n" + $afterClosing
            
            $brushesAdded = $missingBrushes.Count
            $TotalBrushesAdded += $brushesAdded
            $FilesModified++
            
            # Write updated content if not in preview mode
            if (-not $WhatIf) {
                Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8 -NoNewline
                Write-Host "  Added $brushesAdded missing brushes" -ForegroundColor Green
            } else {
                Write-Host "  Would add $brushesAdded missing brushes" -ForegroundColor Yellow
            }
        } else {
            Write-Host "  Error: Could not find </ResourceDictionary> tag" -ForegroundColor Red
        }
        
    } catch {
        Write-Host "  Error processing file: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "STANDARDIZATION SUMMARY" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host "Files processed: $($themeFiles.Count)" -ForegroundColor White
Write-Host "Files modified: $FilesModified" -ForegroundColor Green
Write-Host "Total brushes added: $TotalBrushesAdded" -ForegroundColor Green

if ($WhatIf) {
    Write-Host ""
    Write-Host "PREVIEW COMPLETE - No files were modified" -ForegroundColor Yellow
    Write-Host "Run without -WhatIf to apply changes" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "THEME STANDARDIZATION COMPLETE!" -ForegroundColor Green
}

# Function to generate missing brush definitions based on theme colors
function GenerateMissingBrushes {
    param(
        [string]$ThemeName,
        [string[]]$MissingBrushes,
        [string]$ExistingContent
    )
    
    # Extract primary color from existing content to base other colors on
    $primaryColorMatch = [regex]::Match($ExistingContent, 'MTM_Shared_Logic\.PrimaryAction.*?Color="([^"]+)"')
    $primaryColor = if ($primaryColorMatch.Success) { $primaryColorMatch.Groups[1].Value } else { "#6A5ACD" }
    
    # Theme-specific color palettes
    $themeColors = @{
        "Amber" = @{
            Primary = "#FF8F00"; Secondary = "#FFB300"; Warning = "#FFC107"; Status = "#F57F17"
            Critical = "#FF6F00"; Highlight = "#FFCA28"; DarkNav = "#E65100"; CardBg = "#FFF8E1"
            HoverBg = "#FFFDF7"; MainBg = "#FFFDF7"; ContentBg = "#FFFFFF"; SidebarDark = "#E65100"
            PageHeaders = "#FF8F00"; HeadingText = "#E65100"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#FF8F00"
        }
        "Blue" = @{
            Primary = "#1E88E5"; Secondary = "#42A5F5"; Warning = "#5C6BC0"; Status = "#1976D2"
            Critical = "#3F51B5"; Highlight = "#90CAF9"; DarkNav = "#0D47A1"; CardBg = "#E3F2FD"
            HoverBg = "#F8FEFF"; MainBg = "#F8FEFF"; ContentBg = "#FFFFFF"; SidebarDark = "#0D47A1"
            PageHeaders = "#1E88E5"; HeadingText = "#0D47A1"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#1E88E5"
        }
        "Blue_Dark" = @{
            Primary = "#42A5F5"; Secondary = "#64B5F6"; Warning = "#7986CB"; Status = "#1E88E5"
            Critical = "#1976D2"; Highlight = "#90CAF9"; DarkNav = "#0D47A1"; CardBg = "#1A237E"
            HoverBg = "#0D1421"; MainBg = "#121212"; ContentBg = "#1E1E1E"; SidebarDark = "#0D47A1"
            PageHeaders = "#42A5F5"; HeadingText = "#90CAF9"; BodyText = "#BBBBBB"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#64B5F6"
        }
        "Red" = @{
            Primary = "#E53935"; Secondary = "#EF5350"; Warning = "#F44336"; Status = "#D32F2F"
            Critical = "#C62828"; Highlight = "#E57373"; DarkNav = "#B71C1C"; CardBg = "#FFEBEE"
            HoverBg = "#FFF8F8"; MainBg = "#FFF8F8"; ContentBg = "#FFFFFF"; SidebarDark = "#B71C1C"
            PageHeaders = "#E53935"; HeadingText = "#B71C1C"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#E53935"
        }
        "Green" = @{
            Primary = "#4CAF50"; Secondary = "#66BB6A"; Warning = "#8BC34A"; Status = "#2E7D32"
            Critical = "#1B5E20"; Highlight = "#A5D6A7"; DarkNav = "#1B5E20"; CardBg = "#E8F5E8"
            HoverBg = "#F8FFF8"; MainBg = "#F8FFF8"; ContentBg = "#FFFFFF"; SidebarDark = "#1B5E20"
            PageHeaders = "#4CAF50"; HeadingText = "#1B5E20"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#4CAF50"
        }
        "Green_Dark" = @{
            Primary = "#66BB6A"; Secondary = "#81C784"; Warning = "#9CCC65"; Status = "#4CAF50"
            Critical = "#2E7D32"; Highlight = "#A5D6A7"; DarkNav = "#1B5E20"; CardBg = "#1B3A1B"
            HoverBg = "#0D1F0D"; MainBg = "#121212"; ContentBg = "#1E1E1E"; SidebarDark = "#1B5E20"
            PageHeaders = "#66BB6A"; HeadingText = "#A5D6A7"; BodyText = "#BBBBBB"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#81C784"
        }
        "Dark" = @{
            Primary = "#BB86FC"; Secondary = "#CF6679"; Warning = "#FFB74D"; Status = "#03DAC6"
            Critical = "#F44336"; Highlight = "#FFCDD2"; DarkNav = "#3700B3"; CardBg = "#2C2C2C"
            HoverBg = "#1A1A1A"; MainBg = "#121212"; ContentBg = "#1E1E1E"; SidebarDark = "#3700B3"
            PageHeaders = "#BB86FC"; HeadingText = "#E1BEE7"; BodyText = "#BBBBBB"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#BB86FC"
        }
        "Light" = @{
            Primary = "#6200EE"; Secondary = "#3700B3"; Warning = "#FF6D00"; Status = "#018786"
            Critical = "#B00020"; Highlight = "#E1BEE7"; DarkNav = "#3700B3"; CardBg = "#F5F5F5"
            HoverBg = "#FAFAFA"; MainBg = "#FFFFFF"; ContentBg = "#FFFFFF"; SidebarDark = "#3700B3"
            PageHeaders = "#6200EE"; HeadingText = "#3700B3"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#6200EE"
        }
        "HighContrast" = @{
            Primary = "#000000"; Secondary = "#333333"; Warning = "#FFD700"; Status = "#0000FF"
            Critical = "#FF0000"; Highlight = "#CCCCCC"; DarkNav = "#000000"; CardBg = "#F8F8F8"
            HoverBg = "#FFFFFF"; MainBg = "#FFFFFF"; ContentBg = "#FFFFFF"; SidebarDark = "#000000"
            PageHeaders = "#000000"; HeadingText = "#000000"; BodyText = "#000000"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#0000FF"
        }
        "Emerald" = @{
            Primary = "#00C853"; Secondary = "#00E676"; Warning = "#76FF03"; Status = "#00BFA5"
            Critical = "#DD2C00"; Highlight = "#B9F6CA"; DarkNav = "#00695C"; CardBg = "#E0F2F1"
            HoverBg = "#F0FFF0"; MainBg = "#F0FFF0"; ContentBg = "#FFFFFF"; SidebarDark = "#00695C"
            PageHeaders = "#00C853"; HeadingText = "#00695C"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#00C853"
        }
        "Indigo" = @{
            Primary = "#3F51B5"; Secondary = "#5C6BC0"; Warning = "#7E57C2"; Status = "#303F9F"
            Critical = "#D32F2F"; Highlight = "#C5CAE9"; DarkNav = "#1A237E"; CardBg = "#E8EAF6"
            HoverBg = "#F3F4F6"; MainBg = "#F3F4F6"; ContentBg = "#FFFFFF"; SidebarDark = "#1A237E"
            PageHeaders = "#3F51B5"; HeadingText = "#1A237E"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#3F51B5"
        }
        "Indigo_Dark" = @{
            Primary = "#5C6BC0"; Secondary = "#7986CB"; Warning = "#9575CD"; Status = "#3F51B5"
            Critical = "#F44336"; Highlight = "#C5CAE9"; DarkNav = "#1A237E"; CardBg = "#232F5F"
            HoverBg = "#0F1419"; MainBg = "#121212"; ContentBg = "#1E1E1E"; SidebarDark = "#1A237E"
            PageHeaders = "#5C6BC0"; HeadingText = "#C5CAE9"; BodyText = "#BBBBBB"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#7986CB"
        }
        "Rose" = @{
            Primary = "#E91E63"; Secondary = "#F06292"; Warning = "#FF8A65"; Status = "#C2185B"
            Critical = "#AD1457"; Highlight = "#F8BBD9"; DarkNav = "#880E4F"; CardBg = "#FCE4EC"
            HoverBg = "#FFF8FA"; MainBg = "#FFF8FA"; ContentBg = "#FFFFFF"; SidebarDark = "#880E4F"
            PageHeaders = "#E91E63"; HeadingText = "#880E4F"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#E91E63"
        }
        "Default" = @{
            Primary = "#6A5ACD"; Secondary = "#7B68EE"; Warning = "#FFD700"; Status = "#4169E1"
            Critical = "#DC143C"; Highlight = "#DDA0DD"; DarkNav = "#483D8B"; CardBg = "#F0F8FF"
            HoverBg = "#FAFAFA"; MainBg = "#FAFAFA"; ContentBg = "#FFFFFF"; SidebarDark = "#483D8B"
            PageHeaders = "#6A5ACD"; HeadingText = "#483D8B"; BodyText = "#666666"; OverlayTextBrush = "#FFFFFF"
            InteractiveText = "#6A5ACD"
        }
    }
    
    # Use PowerShell-compatible conditional logic instead of ??
    $colors = if ($themeColors.ContainsKey($ThemeName)) { 
        $themeColors[$ThemeName] 
    } else { 
        $themeColors["Default"] 
    }
    
    $brushDefinitions = ""
    
    foreach ($brush in $MissingBrushes) {
        $colorValue = switch ($brush) {
            "MTM_Shared_Logic.PrimaryAction" { $colors.Primary }
            "MTM_Shared_Logic.SecondaryAction" { $colors.Secondary }
            "MTM_Shared_Logic.Warning" { $colors.Warning }
            "MTM_Shared_Logic.Status" { $colors.Status }
            "MTM_Shared_Logic.Critical" { $colors.Critical }
            "MTM_Shared_Logic.Highlight" { $colors.Highlight }
            "MTM_Shared_Logic.DarkNavigation" { $colors.DarkNav }
            "MTM_Shared_Logic.CardBackground" { $colors.CardBg }
            "MTM_Shared_Logic.HoverBackground" { $colors.HoverBg }
            "MTM_Shared_Logic.MainBackground" { $colors.MainBg }
            "MTM_Shared_Logic.ContentAreas" { $colors.ContentBg }
            "MTM_Shared_Logic.SidebarDark" { $colors.SidebarDark }
            "MTM_Shared_Logic.PageHeaders" { $colors.PageHeaders }
            "MTM_Shared_Logic.HeadingText" { $colors.HeadingText }
            "MTM_Shared_Logic.BodyText" { $colors.BodyText }
            "MTM_Shared_Logic.OverlayTextBrush" { $colors.OverlayTextBrush }
            "MTM_Shared_Logic.InteractiveText" { $colors.InteractiveText }
            
            # Interactive States - derived from primary colors
            "MTM_Shared_Logic.PrimaryHoverBrush" { $colors.Secondary }
            "MTM_Shared_Logic.SecondaryHoverBrush" { $colors.Highlight }
            "MTM_Shared_Logic.MagentaHoverBrush" { $colors.Warning }
            "MTM_Shared_Logic.PrimaryPressedBrush" { $colors.Status }
            "MTM_Shared_Logic.SecondaryPressedBrush" { $colors.Primary }
            "MTM_Shared_Logic.MagentaPressedBrush" { $colors.Critical }
            "MTM_Shared_Logic.PrimaryDisabledBrush" { $colors.Highlight }
            "MTM_Shared_Logic.SecondaryDisabledBrush" { $colors.CardBg }
            
            # UI Layout - derived colors
            "MTM_Shared_Logic.FooterBackgroundBrush" { $colors.DarkNav }
            "MTM_Shared_Logic.StatusBarBackgroundBrush" { $colors.DarkNav }
            "MTM_Shared_Logic.CardBackgroundBrush" { $colors.ContentBg }
            "MTM_Shared_Logic.PanelBackgroundBrush" { "#FAFAFA" }
            "MTM_Shared_Logic.BorderBrush" { $colors.CardBg }
            "MTM_Shared_Logic.BorderDarkBrush" { $colors.Highlight }
            
            # Text variations
            "MTM_Shared_Logic.TertiaryTextBrush" { "#999999" }
            "MTM_Shared_Logic.LinkTextBrush" { $colors.Status }
            
            # Semantic colors - standardized across themes
            "MTM_Shared_Logic.SuccessBrush" { "#4CAF50" }
            "MTM_Shared_Logic.WarningBrush" { "#FF9800" }
            "MTM_Shared_Logic.ErrorBrush" { "#F44336" }
            "MTM_Shared_Logic.InfoBrush" { "#2196F3" }
            
            default { $colors.Primary }
        }
        
        $brushDefinitions += "    <SolidColorBrush x:Key=`"$brush`" Color=`"$colorValue`"/>`n"
    }
    
    return $brushDefinitions.TrimEnd()
}
