#!/usr/bin/env pwsh
# =============================================================================
# MTM Theme File Size Optimization Script
# Removes Design.PreviewWith sections to achieve <5KB target file sizes
# =============================================================================

param(
    [string]$ThemePath = "../Resources/Themes",
    [switch]$DryRun,
    [switch]$Verbose
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Script configuration
$CONFIG = @{
    TargetSizeKB = 5
    BackupSuffix = "backup_preview_removed"
    PreviewStartPattern = '^\s*<!--.*Design\.PreviewWith.*-->\s*$|^\s*<Design\.PreviewWith>\s*$'
    PreviewEndPattern = '^\s*</Design\.PreviewWith>\s*$'
    ExcludeFiles = @('MTMComponents.axaml', 'MTMComponentsPreview.axaml')
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

function Write-Error($Message) {
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Get-FileSizeKB($FilePath) {
    if (Test-Path $FilePath) {
        return [Math]::Round((Get-Item $FilePath).Length / 1KB, 2)
    }
    return 0
}

function Remove-PreviewSection($FilePath, $DryRun) {
    if ($Verbose) {
        Write-Status "Processing: $FilePath"
    }
    
    $originalSize = Get-FileSizeKB $FilePath
    $content = Get-Content $FilePath -Raw
    $lines = Get-Content $FilePath
    
    $newLines = @()
    $inPreviewSection = $false
    $previewStartLine = -1
    $previewEndLine = -1
    $previewLinesCount = 0
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        
        # Check for preview section start
        if ($line -match $CONFIG.PreviewStartPattern) {
            $inPreviewSection = $true
            $previewStartLine = $i + 1
            if ($Verbose) {
                Write-Status "  Found preview section start at line $previewStartLine"
            }
            continue
        }
        
        # Check for preview section end
        if ($inPreviewSection -and ($line -match $CONFIG.PreviewEndPattern)) {
            $inPreviewSection = $false
            $previewEndLine = $i + 1
            $previewLinesCount = $previewEndLine - $previewStartLine + 1
            if ($Verbose) {
                Write-Status "  Found preview section end at line $previewEndLine ($previewLinesCount lines)"
            }
            continue
        }
        
        # Add line if not in preview section
        if (-not $inPreviewSection) {
            $newLines += $line
        }
    }
    
    if ($previewStartLine -gt 0) {
        if ($DryRun) {
            Write-Status "  [DRY RUN] Would remove $previewLinesCount lines (preview section)" -Color Yellow
        } else {
            # Create backup
            $backupPath = "$FilePath.$($CONFIG.BackupSuffix)"
            Copy-Item $FilePath $backupPath -Force
            
            # Write optimized content
            $newLines | Out-File $FilePath -Encoding UTF8
            
            $newSize = Get-FileSizeKB $FilePath
            $reduction = $originalSize - $newSize
            $reductionPercent = [Math]::Round(($reduction / $originalSize) * 100, 1)
            
            Write-Success "  Optimized: $originalSize KB → $newSize KB (saved $reduction KB, -$reductionPercent%)"
            
            if ($newSize -le $CONFIG.TargetSizeKB) {
                Write-Success "  ✅ Achieved target size (<$($CONFIG.TargetSizeKB)KB)"
            } else {
                Write-Warning "  ⚠️  Still above target size (>$($CONFIG.TargetSizeKB)KB)"
            }
        }
        
        return @{
            HasPreview = $true
            OriginalSize = $originalSize
            NewSize = if ($DryRun) { $originalSize } else { Get-FileSizeKB $FilePath }
            LinesRemoved = $previewLinesCount
        }
    } else {
        if ($Verbose) {
            Write-Status "  No preview section found (already optimized)" -Color Green
        }
        return @{
            HasPreview = $false
            OriginalSize = $originalSize
            NewSize = $originalSize
            LinesRemoved = 0
        }
    }
}

function Main {
    Write-Host ""
    Write-Host "🎨 MTM THEME FILE SIZE OPTIMIZATION" -ForegroundColor Cyan
    Write-Host "===================================" -ForegroundColor Cyan
    Write-Host ""

    # Resolve theme path
    $themePath = Resolve-Path $ThemePath -ErrorAction SilentlyContinue
    if (-not $themePath) {
        Write-Error "Theme path not found: $ThemePath"
        return
    }

    Write-Status "Theme Directory: $themePath"
    Write-Status "Target Size: <$($CONFIG.TargetSizeKB)KB per theme file"
    Write-Status "Dry Run Mode: $(if ($DryRun) { 'ENABLED' } else { 'DISABLED' })"
    Write-Host ""

    # Get all theme files
    $themeFiles = Get-ChildItem -Path $themePath -Filter "*.axaml" |
        Where-Object { 
            $_.Name -notin $CONFIG.ExcludeFiles -and 
            $_.Name -notlike "*backup*" -and
            $_.Name -notlike "*Preview*"
        } |
        Sort-Object Name

    if ($themeFiles.Count -eq 0) {
        Write-Error "No theme files found in: $themePath"
        return
    }

    Write-Status "Found $($themeFiles.Count) theme files to analyze:"
    $themeFiles | ForEach-Object { 
        $size = Get-FileSizeKB $_.FullName
        $status = if ($size -le $CONFIG.TargetSizeKB) { "✅" } else { "🔧" }
        Write-Host "  $status $($_.Name) ($size KB)"
    }
    Write-Host ""

    # Process files
    $results = @{
        TotalFiles = $themeFiles.Count
        FilesWithPreviews = 0
        FilesOptimized = 0
        TotalSizeBefore = 0
        TotalSizeAfter = 0
        TotalLinesRemoved = 0
        FilesAtTarget = 0
    }

    foreach ($file in $themeFiles) {
        $result = Remove-PreviewSection -FilePath $file.FullName -DryRun $DryRun
        
        $results.TotalSizeBefore += $result.OriginalSize
        $results.TotalSizeAfter += $result.NewSize
        
        if ($result.HasPreview) {
            $results.FilesWithPreviews++
            $results.TotalLinesRemoved += $result.LinesRemoved
            
            if (-not $DryRun) {
                $results.FilesOptimized++
            }
        }
        
        if ($result.NewSize -le $CONFIG.TargetSizeKB) {
            $results.FilesAtTarget++
        }
    }

    # Summary
    Write-Host ""
    Write-Host "📊 OPTIMIZATION SUMMARY" -ForegroundColor Cyan
    Write-Host "=======================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Files Processed: $($results.TotalFiles)" -ForegroundColor White
    Write-Host "Files with previews: $($results.FilesWithPreviews)" -ForegroundColor Yellow
    
    if ($DryRun) {
        Write-Host "Files that would be optimized: $($results.FilesWithPreviews)" -ForegroundColor Yellow
        Write-Host "Lines that would be removed: $($results.TotalLinesRemoved)" -ForegroundColor Yellow
    } else {
        Write-Host "Files optimized: $($results.FilesOptimized)" -ForegroundColor Green
        Write-Host "Lines removed: $($results.TotalLinesRemoved)" -ForegroundColor Green
    }
    
    $sizeDifference = $results.TotalSizeBefore - $results.TotalSizeAfter
    $percentReduction = if ($results.TotalSizeBefore -gt 0) { 
        [Math]::Round(($sizeDifference / $results.TotalSizeBefore) * 100, 1) 
    } else { 0 }
    
    Write-Host ""
    Write-Host "Size Results:" -ForegroundColor White
    Write-Host "  Before: $([Math]::Round($results.TotalSizeBefore, 1)) KB" -ForegroundColor White
    Write-Host "  After:  $([Math]::Round($results.TotalSizeAfter, 1)) KB" -ForegroundColor $(if ($DryRun) { 'Yellow' } else { 'Green' })
    Write-Host "  Saved:  $([Math]::Round($sizeDifference, 1)) KB (-$percentReduction%)" -ForegroundColor $(if ($DryRun) { 'Yellow' } else { 'Green' })
    Write-Host ""
    
    Write-Host "Target Achievement:" -ForegroundColor White
    Write-Host "  Files at target (<$($CONFIG.TargetSizeKB)KB): $($results.FilesAtTarget)/$($results.TotalFiles)" -ForegroundColor $(if ($results.FilesAtTarget -eq $results.TotalFiles) { 'Green' } else { 'Yellow' })
    
    if ($results.FilesAtTarget -eq $results.TotalFiles) {
        Write-Success "🎯 ALL FILES NOW MEET TARGET SIZE REQUIREMENTS!"
    } elseif ($results.FilesAtTarget -gt 0) {
        Write-Status "🎯 $($results.FilesAtTarget) files meet target, $($results.TotalFiles - $results.FilesAtTarget) still need optimization"
    }
    
    Write-Host ""
    
    if ($DryRun) {
        Write-Status "Run without -DryRun to perform actual optimization" -Color Yellow
    } else {
        Write-Success "Optimization complete! Backup files created with suffix: $($CONFIG.BackupSuffix)"
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "Script execution failed: $($_.Exception.Message)"
    exit 1
}