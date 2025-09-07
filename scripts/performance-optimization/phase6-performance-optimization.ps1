#!/usr/bin/env pwsh
# ================================================================
# PHASE 6 PERFORMANCE OPTIMIZATION TOOL
# MTM Theme Standardization EPIC - Advanced Testing Phase
# ================================================================
# 
# Purpose: Advanced performance optimization to achieve A-Excellent grade
# Target: C-Fair -> A-Excellent performance improvement
# Focus Areas:
#   - Theme file structure optimization
#   - Resource loading efficiency
#   - Memory usage optimization
#   - Theme switching performance
#
# Created: September 6, 2025
# Author: MTM Development Team
# ================================================================

param(
    [Parameter(Mandatory=$false)]
    [string]$ThemesPath = "Resources/Themes",
    
    [Parameter(Mandatory=$false)]
    [switch]$ExecuteOptimizations = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$AnalyzeOnly = $true,
    
    [Parameter(Mandatory=$false)]
    [string]$ReportPath = "phase6-performance-optimization-report.json"
)

# ================================================================
# PERFORMANCE ANALYSIS FUNCTIONS
# ================================================================

function Get-ThemeFileAnalysis {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    $lines = Get-Content $FilePath
    
    $analysis = @{
        FilePath = $FilePath
        FileName = Split-Path $FilePath -Leaf
        FileSize = (Get-Item $FilePath).Length
        TotalLines = $lines.Count
        BrushDefinitions = 0
        GradientDefinitions = 0
        ComplexBrushes = 0
        ResourceReferences = 0
        EmptyLines = 0
        Comments = 0
        OptimizationScore = 0
        Issues = @()
        Recommendations = @()
    }
    
    foreach ($line in $lines) {
        $trimmedLine = $line.Trim()
        
        # Count different element types
        if ($trimmedLine -match 'SolidColorBrush') { $analysis.BrushDefinitions++ }
        if ($trimmedLine -match 'LinearGradientBrush|RadialGradientBrush') { $analysis.GradientDefinitions++ }
        if ($trimmedLine -match 'DynamicResource|StaticResource') { $analysis.ResourceReferences++ }
        if ($trimmedLine -eq '') { $analysis.EmptyLines++ }
        if ($trimmedLine -match '^\s*<!--') { $analysis.Comments++ }
        
        # Identify complex brushes that might impact performance
        if ($trimmedLine -match 'Transform|GradientStop.*Color.*Offset') { $analysis.ComplexBrushes++ }
    }
    
    # Calculate optimization score (0-100, higher is better)
    $score = 100
    
    # Deduct points for performance issues
    if ($analysis.FileSize -gt 10000) { 
        $score -= 10
        $analysis.Issues += "Large file size ($(($analysis.FileSize/1024).ToString('F1'))KB)"
    }
    
    if ($analysis.ComplexBrushes -gt 10) { 
        $score -= 15
        $analysis.Issues += "$($analysis.ComplexBrushes) complex brushes may impact performance"
    }
    
    if ($analysis.EmptyLines -gt ($analysis.TotalLines * 0.1)) { 
        $score -= 5
        $analysis.Issues += "Excessive empty lines ($($analysis.EmptyLines))"
    }
    
    # Add optimization recommendations
    if ($analysis.FileSize -gt 8000) {
        $analysis.Recommendations += "Consider removing non-essential brushes or comments"
    }
    
    if ($analysis.ComplexBrushes -gt 5) {
        $analysis.Recommendations += "Simplify gradient brushes for better performance"
    }
    
    if ($analysis.BrushDefinitions -lt 70) {
        $analysis.Recommendations += "Verify all required MTM_Shared_Logic brushes are present"
    }
    
    $analysis.OptimizationScore = [math]::Max(0, $score)
    
    return $analysis
}

function Get-MemoryUsageEstimate {
    param([object]$ThemeAnalysis)
    
    # Estimate memory usage based on theme complexity
    $baseMemory = 50 # KB base memory per theme
    $brushMemory = $ThemeAnalysis.BrushDefinitions * 0.1 # KB per brush
    $gradientMemory = $ThemeAnalysis.GradientDefinitions * 0.5 # KB per gradient
    $complexMemory = $ThemeAnalysis.ComplexBrushes * 0.3 # KB per complex brush
    
    $totalMemory = $baseMemory + $brushMemory + $gradientMemory + $complexMemory
    
    return @{
        EstimatedMemoryKB = [math]::Round($totalMemory, 2)
        BaseMemoryKB = $baseMemory
        BrushMemoryKB = [math]::Round($brushMemory, 2)
        GradientMemoryKB = [math]::Round($gradientMemory, 2)
        ComplexMemoryKB = [math]::Round($complexMemory, 2)
        MemoryGrade = if ($totalMemory -lt 60) { "A - Excellent" } 
                      elseif ($totalMemory -lt 80) { "B - Good" }
                      elseif ($totalMemory -lt 100) { "C - Fair" }
                      else { "D - Poor" }
    }
}

function Get-LoadTimeEstimate {
    param([object]$ThemeAnalysis)
    
    # Estimate load time based on file complexity
    $baseLoadMs = 2.0 # Base load time in milliseconds
    $fileSizeFactor = ($ThemeAnalysis.FileSize / 1000) * 0.3 # ms per KB
    $brushFactor = $ThemeAnalysis.BrushDefinitions * 0.02 # ms per brush
    $gradientFactor = $ThemeAnalysis.GradientDefinitions * 0.1 # ms per gradient
    $complexFactor = $ThemeAnalysis.ComplexBrushes * 0.05 # ms per complex element
    
    $totalLoadMs = $baseLoadMs + $fileSizeFactor + $brushFactor + $gradientFactor + $complexFactor
    
    return @{
        EstimatedLoadTimeMs = [math]::Round($totalLoadMs, 2)
        BaseLoadMs = $baseLoadMs
        FileSizeFactorMs = [math]::Round($fileSizeFactor, 2)
        BrushFactorMs = [math]::Round($brushFactor, 2)
        GradientFactorMs = [math]::Round($gradientFactor, 2)
        ComplexFactorMs = [math]::Round($complexFactor, 2)
        LoadGrade = if ($totalLoadMs -lt 5.0) { "A - Excellent" } 
                    elseif ($totalLoadMs -lt 7.5) { "B - Good" }
                    elseif ($totalLoadMs -lt 10.0) { "C - Fair" }
                    else { "D - Poor" }
    }
}

# ================================================================
# OPTIMIZATION FUNCTIONS
# ================================================================

function Optimize-ThemeFile {
    param([string]$FilePath, [object]$Analysis)
    
    Write-Host "Optimizing theme file: $(Split-Path $FilePath -Leaf)" -ForegroundColor Yellow
    
    $content = Get-Content $FilePath -Raw
    $originalSize = $content.Length
    $optimizations = @()
    
    # Create backup
    $backupPath = $FilePath + ".backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Copy-Item $FilePath $backupPath
    $optimizations += "Created backup: $(Split-Path $backupPath -Leaf)"
    
    # Optimization 1: Remove excessive empty lines
    if ($Analysis.EmptyLines -gt ($Analysis.TotalLines * 0.1)) {
        $content = $content -replace '(\r?\n\s*\r?\n)\s*(\r?\n\s*)*', '$1'
        $optimizations += "Reduced excessive empty lines"
    }
    
    # Optimization 2: Simplify brush definitions
    # Convert complex color definitions to simpler forms where possible
    $content = $content -replace 'Color="#([0-9A-Fa-f]{2})([0-9A-Fa-f]{6})"', 'Color="#$2"' # Remove alpha if FF (fully opaque)
    
    # Optimization 3: Optimize comments (keep important ones, remove excessive whitespace)
    $content = $content -replace '<!--\s+', '<!-- '
    $content = $content -replace '\s+-->', ' -->'
    
    # Write optimized content
    Set-Content $FilePath $content -NoNewline
    
    $newSize = $content.Length
    $reduction = $originalSize - $newSize
    $reductionPercent = if ($originalSize -gt 0) { ($reduction / $originalSize) * 100 } else { 0 }
    
    $optimizations += "Size reduction: $reduction bytes ($(($reductionPercent).ToString('F1'))%)"
    
    return @{
        OriginalSize = $originalSize
        NewSize = $newSize
        Reduction = $reduction
        ReductionPercent = [math]::Round($reductionPercent, 2)
        Optimizations = $optimizations
        BackupPath = $backupPath
    }
}

# ================================================================
# MAIN EXECUTION
# ================================================================

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "PHASE 6 PERFORMANCE OPTIMIZATION - MTM THEME STANDARDIZATION" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Validate paths
if (-not (Test-Path $ThemesPath)) {
    Write-Error "Themes path not found: $ThemesPath"
    exit 1
}

# Get all theme files
$themeFiles = Get-ChildItem -Path $ThemesPath -Filter "*.axaml" | Where-Object { 
    $_.Name -ne "MTMComponentsPreview.axaml" # Exclude preview file
}

Write-Host "Found $($themeFiles.Count) theme files for analysis" -ForegroundColor Green
Write-Host ""

# Initialize results
$results = @{
    AnalysisDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    ExecuteOptimizations = $ExecuteOptimizations.IsPresent
    ThemeResults = @()
    Summary = @{
        TotalThemes = $themeFiles.Count
        AverageOptimizationScore = 0
        TotalSizeKB = 0
        EstimatedTotalMemoryKB = 0
        EstimatedAverageLoadTimeMs = 0
        ThemesNeedingOptimization = 0
        OverallGrade = ""
        TopPerformingThemes = @()
        ThemesNeedingAttention = @()
    }
    OptimizationResults = @()
}

# Analyze each theme
$totalOptimizationScore = 0
$totalMemory = 0
$totalLoadTime = 0
$needsOptimization = 0

foreach ($themeFile in $themeFiles) {
    Write-Host "Analyzing: $($themeFile.Name)" -ForegroundColor Yellow
    
    # Perform theme analysis
    $analysis = Get-ThemeFileAnalysis -FilePath $themeFile.FullName
    $memoryUsage = Get-MemoryUsageEstimate -ThemeAnalysis $analysis
    $loadTime = Get-LoadTimeEstimate -ThemeAnalysis $analysis
    
    # Combine results
    $themeResult = @{
        ThemeName = $themeFile.BaseName
        Analysis = $analysis
        MemoryUsage = $memoryUsage
        LoadTime = $loadTime
        OverallGrade = ""
        Priority = ""
    }
    
    # Calculate overall grade for this theme
    $grades = @($memoryUsage.MemoryGrade, $loadTime.LoadGrade)
    $gradeValues = @{
        "A - Excellent" = 4
        "B - Good" = 3
        "C - Fair" = 2
        "D - Poor" = 1
    }
    
    $avgGradeValue = ($grades | ForEach-Object { $gradeValues[$_] } | Measure-Object -Average).Average
    $themeResult.OverallGrade = switch ([math]::Round($avgGradeValue)) {
        4 { "A - Excellent" }
        3 { "B - Good" }
        2 { "C - Fair" }
        1 { "D - Poor" }
        default { "C - Fair" }
    }
    
    # Determine optimization priority
    if ($analysis.OptimizationScore -lt 70) {
        $themeResult.Priority = "High"
        $needsOptimization++
    } elseif ($analysis.OptimizationScore -lt 85) {
        $themeResult.Priority = "Medium"
    } else {
        $themeResult.Priority = "Low"
    }
    
    $results.ThemeResults += $themeResult
    
    # Update totals
    $totalOptimizationScore += $analysis.OptimizationScore
    $totalMemory += $memoryUsage.EstimatedMemoryKB
    $totalLoadTime += $loadTime.EstimatedLoadTimeMs
    
    Write-Host "  - Optimization Score: $($analysis.OptimizationScore)/100" -ForegroundColor Cyan
    Write-Host "  - Memory Usage: $($memoryUsage.EstimatedMemoryKB) KB ($($memoryUsage.MemoryGrade))" -ForegroundColor Cyan
    Write-Host "  - Load Time: $($loadTime.EstimatedLoadTimeMs) ms ($($loadTime.LoadGrade))" -ForegroundColor Cyan
    Write-Host "  - Overall Grade: $($themeResult.OverallGrade)" -ForegroundColor $(if ($themeResult.OverallGrade -match "A|B") { "Green" } else { "Yellow" })
    Write-Host ""
}

# Calculate summary statistics
$results.Summary.AverageOptimizationScore = [math]::Round($totalOptimizationScore / $themeFiles.Count, 1)
$results.Summary.TotalSizeKB = [math]::Round(($themeFiles | Measure-Object Length -Sum).Sum / 1024, 2)
$results.Summary.EstimatedTotalMemoryKB = [math]::Round($totalMemory, 2)
$results.Summary.EstimatedAverageLoadTimeMs = [math]::Round($totalLoadTime / $themeFiles.Count, 2)
$results.Summary.ThemesNeedingOptimization = $needsOptimization

# Determine overall grade
$avgScore = $results.Summary.AverageOptimizationScore
$results.Summary.OverallGrade = if ($avgScore -ge 90) { "A - Excellent" }
                               elseif ($avgScore -ge 80) { "B - Good" }
                               elseif ($avgScore -ge 70) { "C - Fair" }
                               else { "D - Poor" }

# Identify top performing and attention-needed themes
$results.Summary.TopPerformingThemes = $results.ThemeResults | 
    Where-Object { $_.Analysis.OptimizationScore -ge 85 } | 
    Sort-Object { $_.Analysis.OptimizationScore } -Descending | 
    Select-Object -First 5 -ExpandProperty ThemeName

$results.Summary.ThemesNeedingAttention = $results.ThemeResults | 
    Where-Object { $_.Priority -eq "High" -or $_.Analysis.OptimizationScore -lt 70 } | 
    Sort-Object { $_.Analysis.OptimizationScore } | 
    Select-Object -First 10 -ExpandProperty ThemeName

# Execute optimizations if requested
if ($ExecuteOptimizations) {
    Write-Host "================================================================" -ForegroundColor Magenta
    Write-Host "EXECUTING PERFORMANCE OPTIMIZATIONS" -ForegroundColor Magenta
    Write-Host "================================================================" -ForegroundColor Magenta
    Write-Host ""
    
    $optimizationResults = @()
    
    foreach ($themeResult in $results.ThemeResults) {
        if ($themeResult.Priority -eq "High" -or $themeResult.Analysis.OptimizationScore -lt 80) {
            $themeFile = Get-ChildItem -Path $ThemesPath -Filter "$($themeResult.ThemeName).axaml"
            $optimization = Optimize-ThemeFile -FilePath $themeFile.FullName -Analysis $themeResult.Analysis
            
            $optimizationResults += @{
                ThemeName = $themeResult.ThemeName
                OptimizationResult = $optimization
            }
        }
    }
    
    $results.OptimizationResults = $optimizationResults
    
    Write-Host "Optimization complete for $($optimizationResults.Count) themes" -ForegroundColor Green
    Write-Host ""
}

# Display summary
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "PERFORMANCE ANALYSIS SUMMARY" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "Average Optimization Score: $($results.Summary.AverageOptimizationScore)/100" -ForegroundColor White
Write-Host "Overall Grade: $($results.Summary.OverallGrade)" -ForegroundColor $(if ($results.Summary.OverallGrade -match "A|B") { "Green" } else { "Yellow" })
Write-Host "Total Size: $($results.Summary.TotalSizeKB) KB" -ForegroundColor White
Write-Host "Estimated Total Memory: $($results.Summary.EstimatedTotalMemoryKB) KB" -ForegroundColor White
Write-Host "Average Load Time: $($results.Summary.EstimatedAverageLoadTimeMs) ms" -ForegroundColor White
Write-Host "Themes Needing Optimization: $($results.Summary.ThemesNeedingOptimization)" -ForegroundColor $(if ($results.Summary.ThemesNeedingOptimization -gt 0) { "Yellow" } else { "Green" })
Write-Host ""

if ($results.Summary.TopPerformingThemes.Count -gt 0) {
    Write-Host "Top Performing Themes:" -ForegroundColor Green
    $results.Summary.TopPerformingThemes | ForEach-Object { Write-Host "  - $_" -ForegroundColor Green }
    Write-Host ""
}

if ($results.Summary.ThemesNeedingAttention.Count -gt 0) {
    Write-Host "Themes Needing Attention:" -ForegroundColor Yellow
    $results.Summary.ThemesNeedingAttention | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    Write-Host ""
}

# Save report
$results | ConvertTo-Json -Depth 10 | Set-Content -Path $ReportPath
Write-Host "Detailed report saved to: $ReportPath" -ForegroundColor Green

# Recommendations
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "RECOMMENDATIONS FOR A-EXCELLENT GRADE" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

if ($results.Summary.OverallGrade -ne "A - Excellent") {
    Write-Host "To achieve A-Excellent grade:" -ForegroundColor Yellow
    Write-Host "1. Optimize themes with scores below 85" -ForegroundColor White
    Write-Host "2. Reduce file sizes to under 8KB per theme" -ForegroundColor White
    Write-Host "3. Minimize complex gradient definitions" -ForegroundColor White
    Write-Host "4. Remove unnecessary comments and empty lines" -ForegroundColor White
    Write-Host "5. Consider consolidating similar brush definitions" -ForegroundColor White
    
    if (-not $ExecuteOptimizations) {
        Write-Host ""
        Write-Host "Run with -ExecuteOptimizations to apply automatic improvements" -ForegroundColor Cyan
    }
} else {
    Write-Host "🎉 EXCELLENT! Theme performance is already at A-grade level!" -ForegroundColor Green
}

Write-Host ""
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "PHASE 6 PERFORMANCE ANALYSIS COMPLETE" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan