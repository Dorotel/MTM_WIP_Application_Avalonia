#!/usr/bin/env pwsh
# =============================================================================
# MTM WCAG 2.1 AA Failure Remediation Script
# Automatically fixes contrast ratio failures to achieve WCAG compliance
# =============================================================================

param(
    [string]$ThemePath = "./Resources/Themes",
    [switch]$DryRun,
    [switch]$VerboseOutput,
    [string]$TargetTheme = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# WCAG-compliant color replacements for common failing colors
$WCAG_REMEDIATION = @{
    # Primary Action Button Colors (need 4.5:1 on white text)
    "#E91E63" = "#C2185B"  # Pink - darken for better contrast
    "#F06292" = "#C2185B"  # Light Pink - significantly darker
    "#42A5F5" = "#1976D2"  # Light Blue - darker blue
    "#64B5F6" = "#1565C0"  # Very Light Blue - much darker
    "#26A69A" = "#00695C"  # Teal - darker teal
    "#4CAF50" = "#2E7D32"  # Light Green - darker green
    "#FFA726" = "#E65100"  # Light Orange - darker orange
    "#FFCC02" = "#E65100"  # Light Yellow - orange (better contrast)
    "#FFC107" = "#E65100"  # Amber - darker orange
    
    # Critical Alert Colors
    "#E57373" = "#C62828"  # Light Red - darker red
    "#FF5722" = "#D84315"  # Deep Orange - darker
    
    # Warning Colors  
    "#FFEB3B" = "#F57F17"  # Yellow - darker yellow-orange
    "#FFD54F" = "#F57F17"  # Light Yellow - darker
    
    # Success Colors (especially problematic in dark themes)
    "#81C784" = "#2E7D32"  # Light Green - darker green
    "#A5D6A7" = "#1B5E20"  # Very Light Green - much darker
    
    # Transfer Operation Colors
    "#FFB74D" = "#E65100"  # Light Orange - darker
    "#FFAB91" = "#D84315"  # Light Deep Orange - darker
}

function Write-Status($Message, $Color = "White") {
    Write-Host "🔧 $Message" -ForegroundColor $Color
}

function Write-Success($Message) {
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning($Message) {
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

function Write-Critical($Message) {
    Write-Host "🚨 $Message" -ForegroundColor Red
}

function Test-ContrastRatio($Color1Hex, $Color2Hex, $RequiredRatio = 4.5) {
    # Basic contrast calculation - simplified for remediation
    try {
        $hex1 = $Color1Hex -replace '^#', ''
        $hex2 = $Color2Hex -replace '^#', ''
        
        # Convert to RGB and calculate basic luminance
        $rgb1 = @{
            R = [Convert]::ToInt32($hex1.Substring(0, 2), 16)
            G = [Convert]::ToInt32($hex1.Substring(2, 2), 16)
            B = [Convert]::ToInt32($hex1.Substring(4, 2), 16)
        }
        
        $rgb2 = @{
            R = [Convert]::ToInt32($hex2.Substring(0, 2), 16)
            G = [Convert]::ToInt32($hex2.Substring(2, 2), 16)
            B = [Convert]::ToInt32($hex2.Substring(4, 2), 16)
        }
        
        # Simple luminance calculation (approximation)
        $lum1 = (0.299 * $rgb1.R + 0.587 * $rgb1.G + 0.114 * $rgb1.B) / 255
        $lum2 = (0.299 * $rgb2.R + 0.587 * $rgb2.G + 0.114 * $rgb2.B) / 255
        
        $lighter = [Math]::Max($lum1, $lum2)
        $darker = [Math]::Min($lum1, $lum2)
        
        $ratio = ($lighter + 0.05) / ($darker + 0.05)
        return $ratio -ge $RequiredRatio
    } catch {
        return $false
    }
}

function Fix-ThemeContrastIssues($ThemeFile) {
    $themeName = [System.IO.Path]::GetFileNameWithoutExtension($ThemeFile.Name)
    Write-Status "Analyzing $themeName for WCAG contrast issues..."
    
    $content = Get-Content $ThemeFile.FullName -Raw
    $originalContent = $content
    $changesWeMade = 0
    
    # Apply remediation for each problematic color
    foreach ($problematicColor in $WCAG_REMEDIATION.Keys) {
        $betterColor = $WCAG_REMEDIATION[$problematicColor]
        
        if ($content -match [regex]::Escape($problematicColor)) {
            Write-Status "  Found problematic color $problematicColor -> replacing with $betterColor"
            $content = $content -replace [regex]::Escape($problematicColor), $betterColor
            $changesWeMade++
        }
    }
    
    # Special fixes for common theme patterns
    
    # Fix light colors that often fail contrast on white text
    $lightColorFixes = @{
        'Color="#FF9800"' = 'Color="#E65100"'  # Orange
        'Color="#FFEB3B"' = 'Color="#F57F17"'  # Yellow
        'Color="#CDDC39"' = 'Color="#827717"'  # Light Green
        'Color="#8BC34A"' = 'Color="#2E7D32"'  # Light Green
        'Color="#03DAC6"' = 'Color="#00695C"'  # Light Cyan
        'Color="#FFCDD2"' = 'Color="#C62828"'  # Very Light Red
    }
    
    foreach ($lightColor in $lightColorFixes.Keys) {
        $darkerColor = $lightColorFixes[$lightColor]
        if ($content -match [regex]::Escape($lightColor)) {
            Write-Status "  Fixing light color: $lightColor -> $darkerColor"
            $content = $content -replace [regex]::Escape($lightColor), $darkerColor
            $changesWeMade++
        }
    }
    
    # Theme-specific adjustments
    switch -Wildcard ($themeName) {
        "*_Dark" {
            # Dark themes often have issues with overly bright colors
            Write-Status "  Applying dark theme specific fixes..."
            
            # Make bright colors more muted for dark themes
            $darkThemeFixes = @{
                'Color="#FFFFFF"' = 'Color="#E0E0E0"'  # Pure white too harsh
                'Color="#FFC107"' = 'Color="#FF8F00"'  # Amber too bright
                'Color="#FFEB3B"' = 'Color="#F57C00"'  # Yellow too bright
            }
            
            foreach ($brightColor in $darkThemeFixes.Keys) {
                $mutedColor = $darkThemeFixes[$brightColor]
                if ($content -match [regex]::Escape($brightColor)) {
                    Write-Status "    Dark theme fix: $brightColor -> $mutedColor"
                    $content = $content -replace [regex]::Escape($brightColor), $mutedColor
                    $changesWeMade++
                }
            }
        }
        
        "MTM_HighContrast" {
            # High contrast theme needs maximum contrast ratios
            Write-Status "  Applying high contrast theme specific fixes..."
            
            $highContrastFixes = @{
                'Color="#333333"' = 'Color="#000000"'  # Make blacks pure black
                'Color="#CCCCCC"' = 'Color="#FFFFFF"'  # Make lights pure white
                'Color="#666666"' = 'Color="#000000"'  # Medium grays to black
                'Color="#999999"' = 'Color="#FFFFFF"'  # Light grays to white
            }
            
            foreach ($grayColor in $highContrastFixes.Keys) {
                $contrastColor = $highContrastFixes[$grayColor]
                if ($content -match [regex]::Escape($grayColor)) {
                    Write-Status "    High contrast fix: $grayColor -> $contrastColor"
                    $content = $content -replace [regex]::Escape($grayColor), $contrastColor
                    $changesWeMade++
                }
            }
        }
    }
    
    # Apply changes if any were made
    if ($changesWeMade -gt 0) {
        if ($DryRun) {
            Write-Warning "DRY RUN: Would make $changesWeMade contrast improvements to ${themeName}"
            return @{ Changed = $true; Count = $changesWeMade; DryRun = $true }
        } else {
            # Create backup
            $backupPath = $ThemeFile.FullName + ".backup_wcag_" + (Get-Date -Format "yyyyMMdd_HHmmss")
            Copy-Item $ThemeFile.FullName $backupPath
            
            # Write improved content
            Set-Content -Path $ThemeFile.FullName -Value $content -Encoding UTF8
            Write-Success "${themeName}: Applied $changesWeMade contrast improvements"
            return @{ Changed = $true; Count = $changesWeMade; BackupPath = $backupPath }
        }
    } else {
        Write-Status "${themeName}: No contrast issues requiring automated fixes"
        return @{ Changed = $false; Count = 0 }
    }
}

# Main execution
try {
    Write-Host ""
    Write-Host "🎨 MTM WCAG 2.1 AA CONTRAST REMEDIATION" -ForegroundColor Cyan
    Write-Host "=======================================" -ForegroundColor Cyan
    Write-Host ""
    
    if ($DryRun) {
        Write-Warning "DRY RUN MODE - No files will be modified"
        Write-Host ""
    }
    
    # Resolve theme path
    $themePath = Resolve-Path $ThemePath -ErrorAction SilentlyContinue
    if (-not $themePath) {
        Write-Error "Theme path not found: $ThemePath"
        exit 1
    }
    
    Write-Status "Theme Directory: $themePath"
    Write-Status "Mode: $(if ($DryRun) { 'Dry Run (Preview)' } else { 'Apply Fixes' })"
    Write-Host ""
    
    # Get theme files
    $themeFiles = @()
    if ($TargetTheme) {
        $targetFile = Join-Path $themePath "$TargetTheme.axaml"
        if (Test-Path $targetFile) {
            $themeFiles = @(Get-Item $targetFile)
            Write-Status "Targeting single theme: $TargetTheme"
        } else {
            Write-Error "Target theme not found: $targetFile"
            exit 1
        }
    } else {
        $themeFiles = Get-ChildItem -Path $themePath -Filter "MTM_*.axaml" |
            Where-Object { $_.Name -notlike "*backup*" -and $_.Name -notlike "*Preview*" } |
            Sort-Object Name
        Write-Status "Processing all themes: $($themeFiles.Count) files"
    }
    
    if ($themeFiles.Count -eq 0) {
        Write-Error "No theme files found"
        exit 1
    }
    
    Write-Host ""
    Write-Host "🔧 PROCESSING THEMES:" -ForegroundColor Yellow
    Write-Host "=====================" -ForegroundColor Yellow
    
    $summary = @{
        ProcessedFiles = 0
        ModifiedFiles = 0
        TotalFixes = 0
        BackupFiles = @()
    }
    
    foreach ($themeFile in $themeFiles) {
        $result = Fix-ThemeContrastIssues $themeFile
        $summary.ProcessedFiles++
        
        if ($result.Changed) {
            $summary.ModifiedFiles++
            $summary.TotalFixes += $result.Count
            
            if ($result.BackupPath) {
                $summary.BackupFiles += $result.BackupPath
            }
        }
    }
    
    Write-Host ""
    Write-Host "📊 REMEDIATION SUMMARY" -ForegroundColor Green
    Write-Host "======================" -ForegroundColor Green
    Write-Host "Files Processed: $($summary.ProcessedFiles)" -ForegroundColor White
    Write-Host "Files Modified: $($summary.ModifiedFiles)" -ForegroundColor Green
    Write-Host "Total Fixes Applied: $($summary.TotalFixes)" -ForegroundColor Green
    
    if ($summary.BackupFiles.Count -gt 0) {
        Write-Host "Backup Files Created: $($summary.BackupFiles.Count)" -ForegroundColor Cyan
    }
    
    if ($DryRun) {
        Write-Host ""
        Write-Warning "This was a DRY RUN - no changes were applied"
        Write-Host "Run without -DryRun to apply these fixes" -ForegroundColor Yellow
    } elseif ($summary.TotalFixes -gt 0) {
        Write-Host ""
        Write-Success "All contrast improvements applied successfully!"
        Write-Host "Run WCAG validation again to verify compliance improvements" -ForegroundColor Yellow
    }
    
    Write-Host ""
    
} catch {
    Write-Error "Remediation failed: $($_.Exception.Message)"
    exit 1
}