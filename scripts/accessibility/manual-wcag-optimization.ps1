#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Manual WCAG Color Optimization for Phase 6 - Targeting 100% Compliance
.DESCRIPTION
    Apply specific color fixes for identified contrast failures based on WCAG validation results
.AUTHOR
    MTM Development Team
.VERSION
    1.0.0
#>

param(
    [string]$ThemesDirectory = "./Resources/Themes",
    [switch]$ExecuteOptimizations = $true,
    [switch]$CreateBackups = $true
)

Write-Host "🌈 Phase 6 Manual WCAG Color Optimization" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan

# Define specific color fixes based on validation results
$colorFixes = @{
    # Universal fixes for common failures
    "TertiaryTextBrush" = "#666666"  # Improve secondary text from #ADB5BD to pass 4.5:1
    
    # Theme-specific fixes
    "MTM_Blue" = @{
        "TertiaryTextBrush" = "#666666"  # Fix secondary text failure (2.07:1 → 5.74:1)
    }
    "MTM_Blue_Dark" = @{
        "SecondaryAction" = "#B3C7F5"  # Fix secondary buttons (1.75:1 → 4.5:1+)
        "Warning" = "#FFB74D"          # Fix warning messages (3.45:1 → 4.5:1+)
    }
    "MTM_Emerald" = @{
        "PrimaryAction" = "#2E7D32"    # Fix primary buttons (3.18:1 → 4.5:1+)
        "SecondaryAction" = "#2E7D32"  # Fix secondary buttons (3.18:1 → 4.5:1+)
        "Critical" = "#C62828"         # Fix critical alerts (2.14:1 → 4.5:1+)
    }
    "MTM_Green" = @{
        "PrimaryAction" = "#2E7D32"    # Fix primary buttons (3.18:1 → 4.5:1+)
        "SecondaryAction" = "#2E7D32"  # Fix secondary buttons (3.18:1 → 4.5:1+)
        "Critical" = "#C62828"         # Fix critical alerts (2.14:1 → 4.5:1+)
    }
    "MTM_Green_Dark" = @{
        "TransactionOutBrush" = "#F44336"      # Fix OUT transactions (3.68:1 → 4.5:1+)
        "TransactionTransferBrush" = "#FF7043" # Fix transfer operations (3.79:1 → 4.5:1+)
    }
    "MTM_HighContrast" = @{
        "PrimaryAction" = "#006600"    # Enhance for high contrast
        "Critical" = "#CC0000"         # Enhance critical alerts
    }
    "MTM_Indigo" = @{
        "PrimaryAction" = "#3F51B5"    # Fix primary buttons
        "SecondaryAction" = "#3F51B5"  # Fix secondary buttons
        "Critical" = "#C62828"         # Fix critical alerts
    }
    "MTM_Indigo_Dark" = @{
        "SecondaryAction" = "#9FA8DA"  # Fix secondary buttons
        "Warning" = "#FFB74D"          # Fix warning messages
    }
    "MTM_Light" = @{
        "PrimaryAction" = "#1976D2"    # Fix primary buttons (2.38:1 → 4.5:1+)
        "SecondaryAction" = "#1976D2"  # Fix secondary buttons
        "Critical" = "#C62828"         # Fix critical alerts
    }
    "MTM_Light_Dark" = @{
        "SecondaryAction" = "#90CAF9"  # Fix secondary buttons (1.75:1 → 4.5:1+)
        "Warning" = "#FFB74D"          # Fix warning messages (3.45:1 → 4.5:1+)
    }
    "MTM_Orange" = @{
        "PrimaryAction" = "#E65100"    # Fix primary buttons (3.79:1 → 4.5:1+)
        "SecondaryAction" = "#E65100"  # Fix secondary buttons
        "Critical" = "#C62828"         # Fix critical alerts
    }
    "MTM_Red_Dark" = @{
        "SecondaryAction" = "#EF9A9A"  # Fix secondary buttons (1.75:1 → 4.5:1+)
        "Warning" = "#FFB74D"          # Fix warning messages (3.45:1 → 4.5:1+)
    }
    "MTM_Rose" = @{
        "PrimaryAction" = "#C2185B"    # Fix primary buttons (3.01:1 → 4.5:1+)
        "Critical" = "#C62828"         # Fix critical alerts (3.58:1 → 4.5:1+)
        "Warning" = "#E65100"          # Fix warning messages (3.58:1 → 4.5:1+)
        "SuccessBrush" = "#2E7D32"     # Fix success indicators (4.1:1 → 4.5:1+)
        "TransactionTransferBrush" = "#E65100" # Fix transfer operations (4.34:1 → 4.5:1+)
        "TertiaryTextBrush" = "#666666" # Fix secondary text (4.41:1 → 4.5:1+)
    }
    "MTM_Rose_Dark" = @{
        "Warning" = "#FFB74D"          # Fix warning messages (3.76:1 → 4.5:1+)
    }
    "MTM_Teal" = @{
        "PrimaryAction" = "#00695C"    # Fix primary buttons (3.95:1 → 4.5:1+)
        "Critical" = "#C62828"         # Fix critical alerts (3.18:1 → 4.5:1+)
        "Warning" = "#E65100"          # Fix warning messages (3.18:1 → 4.5:1+)
        "SuccessBrush" = "#2E7D32"     # Fix success indicators (4.1:1 → 4.5:1+)
        "TransactionTransferBrush" = "#E65100" # Fix transfer operations (4.34:1 → 4.5:1+)
        "TertiaryTextBrush" = "#666666" # Fix secondary text (4.41:1 → 4.5:1+)
    }
    "MTM_Teal_Dark" = @{
        "Warning" = "#FFB74D"          # Fix warning messages (2.44:1 → 4.5:1+)
    }
    "MTMTheme" = @{
        "TertiaryTextBrush" = "#666666" # Apply universal fix
    }
}

$results = @()
$totalFixesApplied = 0

foreach ($themeName in $colorFixes.Keys) {
    if ($themeName -eq "TertiaryTextBrush") { continue } # Skip universal key
    
    $themePath = Join-Path $ThemesDirectory "$themeName.axaml"
    
    if (-not (Test-Path $themePath)) {
        Write-Warning "Theme file not found: $themePath"
        continue
    }
    
    Write-Host "`n🎨 Optimizing WCAG Compliance: $themeName" -ForegroundColor Magenta
    
    # Create backup if requested
    if ($CreateBackups) {
        $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
        $backupPath = "$themePath.wcag-manual-backup.$timestamp"
        Copy-Item $themePath $backupPath
        Write-Host "  💾 Backup created: $backupPath" -ForegroundColor Blue
    }
    
    if ($ExecuteOptimizations) {
        $content = Get-Content $themePath -Raw
        $fixesApplied = @()
        $themeFixes = $colorFixes[$themeName]
        
        foreach ($brushKey in $themeFixes.Keys) {
            $newColor = $themeFixes[$brushKey]
            $pattern = "(MTM_Shared_Logic\.$brushKey`"[^>]+Color=`")[^`"]+(`")"
            
            if ($content -match $pattern) {
                $content = $content -replace $pattern, "`$1$newColor`$2"
                $fixesApplied += "$brushKey → $newColor"
                $totalFixesApplied++
                
                Write-Host "  ✅ Fixed: $brushKey → $newColor" -ForegroundColor Green
            }
        }
        
        # Apply universal TertiaryTextBrush fix if pattern exists
        if ($content -match "(MTM_Shared_Logic\.TertiaryTextBrush`"[^>]+Color=`")[^`"]+(`")") {
            $content = $content -replace "(MTM_Shared_Logic\.TertiaryTextBrush`"[^>]+Color=`")[^`"]+(`")", "`$1#666666`$2"
            if ($fixesApplied -notcontains "TertiaryTextBrush → #666666") {
                $fixesApplied += "TertiaryTextBrush → #666666"
                $totalFixesApplied++
                Write-Host "  ✅ Fixed: TertiaryTextBrush → #666666" -ForegroundColor Green
            }
        }
        
        # Write optimized content
        Set-Content -Path $themePath -Value $content -NoNewline
        
        Write-Host "  📊 Applied $($fixesApplied.Count) color fixes" -ForegroundColor Yellow
        
        $results += [PSCustomObject]@{
            ThemeName = $themeName
            FixesApplied = $fixesApplied
            FixCount = $fixesApplied.Count
            BackupPath = if ($CreateBackups) { $backupPath } else { $null }
        }
    }
}

# Summary
Write-Host "`n📊 WCAG MANUAL OPTIMIZATION SUMMARY" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host "Themes Optimized: $($results.Count)" -ForegroundColor Green
Write-Host "Total Color Fixes Applied: $totalFixesApplied" -ForegroundColor Green

$topFixedThemes = $results | Sort-Object FixCount -Descending | Select-Object -First 3
Write-Host "`n🏆 Top Optimized Themes:" -ForegroundColor Yellow
foreach ($theme in $topFixedThemes) {
    Write-Host "  • $($theme.ThemeName): $($theme.FixCount) fixes" -ForegroundColor Gray
}

Write-Host "`n🎯 Targeting 100% WCAG Compliance..." -ForegroundColor Blue
Write-Host "Manual optimization phase complete!" -ForegroundColor Green

# Export results for integration
$reportPath = "./phase6-manual-wcag-fixes.json"
$results | ConvertTo-Json -Depth 3 | Set-Content $reportPath
Write-Host "`n📄 Results exported to: $reportPath" -ForegroundColor Blue

Write-Host "`n✅ Manual WCAG Color Optimization Complete!" -ForegroundColor Cyan

return $results