#!/usr/bin/env pwsh
<#
.SYNOPSIS
MTM Theme Performance Testing Script

.DESCRIPTION
Comprehensive performance testing for all MTM themes including:
- Theme file size analysis
- Theme switching performance simulation
- Memory usage estimation
- Resource loading time analysis
- Performance regression detection

.PARAMETER ThemeDirectory
Directory containing theme files (default: Resources/Themes)

.PARAMETER OutputReport
Generate detailed performance report (default: true)

.EXAMPLE
./performance-test-themes.ps1
#>

param(
    [string]$ThemeDirectory = "Resources/Themes",
    [bool]$OutputReport = $true
)

# Performance testing configuration
$script:PerformanceReport = @{
    TestDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    TestResults = @()
    Summary = @{
        TotalThemes = 0
        AverageFileSize = 0
        TotalFileSize = 0
        FastestLoadingTheme = ""
        SlowestLoadingTheme = ""
        MemoryEfficientTheme = ""
        LargestTheme = ""
        SmallestTheme = ""
        PerformanceGrade = ""
    }
}

function Test-ThemePerformance {
    param(
        [string]$ThemeFile
    )
    
    $themeName = [System.IO.Path]::GetFileNameWithoutExtension($ThemeFile)
    $fileInfo = Get-Item $ThemeFile
    
    Write-Host "🔬 Testing performance for $themeName..." -ForegroundColor Cyan
    
    # File size analysis
    $fileSizeKB = [Math]::Round($fileInfo.Length / 1024, 2)
    
    # Content analysis
    $content = Get-Content $ThemeFile -Raw
    $brushCount = ($content | Select-String -Pattern 'SolidColorBrush\s+x:Key=' -AllMatches).Matches.Count
    $resourceCount = ($content | Select-String -Pattern 'x:Key=' -AllMatches).Matches.Count
    $commentCount = ($content | Select-String -Pattern '<!--.*?-->' -AllMatches).Matches.Count
    
    # Complexity analysis
    $complexityScore = $resourceCount + ($brushCount * 0.5) + ($content.Length / 1000)
    
    # Simulated loading time (based on file size and complexity)
    $estimatedLoadTimeMs = [Math]::Round(($fileSizeKB * 0.1) + ($complexityScore * 0.05), 2)
    
    # Memory usage estimation (simplified)
    $estimatedMemoryKB = [Math]::Round($resourceCount * 0.5 + $brushCount * 0.2, 2)
    
    # Performance grade calculation
    $performanceGrade = switch ($true) {
        ($fileSizeKB -lt 10 -and $estimatedLoadTimeMs -lt 5) { "A+" }
        ($fileSizeKB -lt 15 -and $estimatedLoadTimeMs -lt 10) { "A" }
        ($fileSizeKB -lt 20 -and $estimatedLoadTimeMs -lt 15) { "B+" }
        ($fileSizeKB -lt 25 -and $estimatedLoadTimeMs -lt 20) { "B" }
        ($fileSizeKB -lt 35 -and $estimatedLoadTimeMs -lt 30) { "C" }
        default { "D" }
    }
    
    $testResult = @{
        ThemeName = $themeName
        FileSizeKB = $fileSizeKB
        BrushCount = $brushCount
        ResourceCount = $resourceCount
        ComplexityScore = $complexityScore
        EstimatedLoadTimeMs = $estimatedLoadTimeMs
        EstimatedMemoryKB = $estimatedMemoryKB
        PerformanceGrade = $performanceGrade
        ThemeFile = $ThemeFile
        IsOptimized = $fileSizeKB -lt 20
        LoadingEfficiency = if ($estimatedLoadTimeMs -gt 0) { [Math]::Round($resourceCount / $estimatedLoadTimeMs, 2) } else { 0 }
        MemoryEfficiency = if ($estimatedMemoryKB -gt 0) { [Math]::Round($resourceCount / $estimatedMemoryKB, 2) } else { 0 }
    }
    
    Write-Host "   📊 File Size: $($testResult.FileSizeKB) KB"
    Write-Host "   🎨 Resources: $($testResult.ResourceCount) ($($testResult.BrushCount) brushes)"
    Write-Host "   ⚡ Est. Load Time: $($testResult.EstimatedLoadTimeMs) ms"
    Write-Host "   💾 Est. Memory: $($testResult.EstimatedMemoryKB) KB" 
    Write-Host "   🏆 Grade: $($testResult.PerformanceGrade)" -ForegroundColor $(if ($testResult.PerformanceGrade -like "A*") { "Green" } elseif ($testResult.PerformanceGrade -like "B*") { "Yellow" } else { "Red" })
    
    return $testResult
}

function Test-ThemeSwitchingPerformance {
    param(
        [array]$ThemeResults
    )
    
    Write-Host "🔄 Testing theme switching performance simulation..." -ForegroundColor Cyan
    
    # Simulate theme switching overhead
    $switchingResults = @()
    
    foreach ($theme in $ThemeResults) {
        $switchingTime = [Math]::Round($theme.EstimatedLoadTimeMs * 1.2 + 2, 2) # Add switching overhead
        $memoryCleanupTime = [Math]::Round($theme.EstimatedMemoryKB * 0.01, 2)
        $totalSwitchTime = $switchingTime + $memoryCleanupTime
        
        $switchingResults += @{
            ThemeName = $theme.ThemeName
            SwitchingTimeMs = $switchingTime
            MemoryCleanupMs = $memoryCleanupTime
            TotalSwitchTimeMs = $totalSwitchTime
            SwitchingGrade = switch ($true) {
                ($totalSwitchTime -lt 10) { "Excellent" }
                ($totalSwitchTime -lt 20) { "Good" }
                ($totalSwitchTime -lt 35) { "Fair" }
                default { "Poor" }
            }
        }
    }
    
    $fastestSwitch = $switchingResults | Sort-Object TotalSwitchTimeMs | Select-Object -First 1
    $slowestSwitch = $switchingResults | Sort-Object TotalSwitchTimeMs -Descending | Select-Object -First 1
    
    Write-Host "   ⚡ Fastest switching: $($fastestSwitch.ThemeName) ($($fastestSwitch.TotalSwitchTimeMs) ms)"
    Write-Host "   🐌 Slowest switching: $($slowestSwitch.ThemeName) ($($slowestSwitch.TotalSwitchTimeMs) ms)"
    
    return $switchingResults
}

function Analyze-PerformanceRegression {
    param(
        [array]$ThemeResults
    )
    
    Write-Host "📈 Analyzing performance regression..." -ForegroundColor Cyan
    
    # Define performance baselines (optimized targets)
    $baselines = @{
        MaxFileSizeKB = 15
        MaxLoadTimeMs = 10
        MaxMemoryKB = 20
        MinResourceEfficiency = 20
    }
    
    $regressions = @()
    
    foreach ($theme in $ThemeResults) {
        $issues = @()
        
        if ($theme.FileSizeKB -gt $baselines.MaxFileSizeKB) {
            $issues += "File size exceeds baseline ($($theme.FileSizeKB) KB > $($baselines.MaxFileSizeKB) KB)"
        }
        
        if ($theme.EstimatedLoadTimeMs -gt $baselines.MaxLoadTimeMs) {
            $issues += "Load time exceeds baseline ($($theme.EstimatedLoadTimeMs) ms > $($baselines.MaxLoadTimeMs) ms)"
        }
        
        if ($theme.EstimatedMemoryKB -gt $baselines.MaxMemoryKB) {
            $issues += "Memory usage exceeds baseline ($($theme.EstimatedMemoryKB) KB > $($baselines.MaxMemoryKB) KB)"
        }
        
        if ($theme.LoadingEfficiency -lt $baselines.MinResourceEfficiency) {
            $issues += "Loading efficiency below baseline ($($theme.LoadingEfficiency) < $($baselines.MinResourceEfficiency))"
        }
        
        if ($issues.Count -gt 0) {
            $regressions += @{
                ThemeName = $theme.ThemeName
                Issues = $issues
                Severity = switch ($issues.Count) {
                    1 { "Low" }
                    2 { "Medium" }
                    3 { "High" }
                    default { "Critical" }
                }
            }
        }
    }
    
    if ($regressions.Count -gt 0) {
        Write-Host "   ⚠️  $($regressions.Count) themes have performance regressions"
        foreach ($regression in $regressions) {
            Write-Host "   🔍 $($regression.ThemeName): $($regression.Severity) severity" -ForegroundColor $(if ($regression.Severity -eq "Critical") { "Red" } elseif ($regression.Severity -eq "High") { "Magenta" } elseif ($regression.Severity -eq "Medium") { "Yellow" } else { "White" })
        }
    } else {
        Write-Host "   ✅ No performance regressions detected"
    }
    
    return $regressions
}

# Main execution
Write-Host "🚀 MTM THEME PERFORMANCE TESTING" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host ""

# Validate theme directory
if (!(Test-Path $ThemeDirectory)) {
    Write-Error "Theme directory not found: $ThemeDirectory"
    exit 1
}

# Get all theme files
$themeFiles = Get-ChildItem -Path $ThemeDirectory -Filter "*.axaml" | Where-Object { $_.Name -ne "MTMTheme.axaml" }

if ($themeFiles.Count -eq 0) {
    Write-Error "No theme files found in $ThemeDirectory"
    exit 1
}

Write-Host "🔧 Theme Directory: $(Resolve-Path $ThemeDirectory)"
Write-Host "🔧 Testing $($themeFiles.Count) theme files"
Write-Host ""

# Test each theme
$themeResults = @()
foreach ($themeFile in $themeFiles) {
    $result = Test-ThemePerformance -ThemeFile $themeFile.FullName
    $themeResults += $result
    $script:PerformanceReport.TestResults += $result
    Write-Host ""
}

# Overall performance analysis
Write-Host "📊 OVERALL PERFORMANCE ANALYSIS" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green

$totalFileSize = ($themeResults | Measure-Object -Property FileSizeKB -Sum).Sum
$averageFileSize = [Math]::Round(($themeResults | Measure-Object -Property FileSizeKB -Average).Average, 2)
$averageLoadTime = [Math]::Round(($themeResults | Measure-Object -Property EstimatedLoadTimeMs -Average).Average, 2)
$averageMemory = [Math]::Round(($themeResults | Measure-Object -Property EstimatedMemoryKB -Average).Average, 2)

$fastestTheme = $themeResults | Sort-Object EstimatedLoadTimeMs | Select-Object -First 1
$slowestTheme = $themeResults | Sort-Object EstimatedLoadTimeMs -Descending | Select-Object -First 1
$smallestTheme = $themeResults | Sort-Object FileSizeKB | Select-Object -First 1
$largestTheme = $themeResults | Sort-Object FileSizeKB -Descending | Select-Object -First 1
$mostEfficientTheme = $themeResults | Sort-Object LoadingEfficiency -Descending | Select-Object -First 1

Write-Host "📈 Total themes tested: $($themeResults.Count)"
Write-Host "📊 Total file size: $totalFileSize KB"
Write-Host "📊 Average file size: $averageFileSize KB"
Write-Host "⚡ Average load time: $averageLoadTime ms"
Write-Host "💾 Average memory usage: $averageMemory KB"
Write-Host ""
Write-Host "🏆 Performance Champions:"
Write-Host "   ⚡ Fastest loading: $($fastestTheme.ThemeName) ($($fastestTheme.EstimatedLoadTimeMs) ms)"
Write-Host "   📦 Smallest file: $($smallestTheme.ThemeName) ($($smallestTheme.FileSizeKB) KB)"
Write-Host "   💾 Most memory efficient: $($mostEfficientTheme.ThemeName) (efficiency: $($mostEfficientTheme.LoadingEfficiency))"
Write-Host ""
Write-Host "⚠️  Performance Concerns:"
Write-Host "   🐌 Slowest loading: $($slowestTheme.ThemeName) ($($slowestTheme.EstimatedLoadTimeMs) ms)"
Write-Host "   📦 Largest file: $($largestTheme.ThemeName) ($($largestTheme.FileSizeKB) KB)"
Write-Host ""

# Theme switching performance
$switchingResults = Test-ThemeSwitchingPerformance -ThemeResults $themeResults
Write-Host ""

# Performance regression analysis
$regressions = Analyze-PerformanceRegression -ThemeResults $themeResults
Write-Host ""

# Performance grade distribution
$gradeDistribution = $themeResults | Group-Object PerformanceGrade | Sort-Object Name
Write-Host "🎓 PERFORMANCE GRADE DISTRIBUTION" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
foreach ($grade in $gradeDistribution) {
    $percentage = [Math]::Round(($grade.Count / $themeResults.Count) * 100, 1)
    Write-Host "   $($grade.Name): $($grade.Count) themes ($percentage%)"
}
Write-Host ""

# Overall performance grade
$overallGrade = switch ($true) {
    (($gradeDistribution | Where-Object { $_.Name -like "A*" } | Measure-Object -Property Count -Sum).Sum / $themeResults.Count -gt 0.8) { "A - Excellent" }
    (($gradeDistribution | Where-Object { $_.Name -like "A*" -or $_.Name -like "B*" } | Measure-Object -Property Count -Sum).Sum / $themeResults.Count -gt 0.8) { "B - Good" }
    (($gradeDistribution | Where-Object { $_.Name -ne "D" } | Measure-Object -Property Count -Sum).Sum / $themeResults.Count -gt 0.8) { "C - Fair" }
    default { "D - Needs Improvement" }
}

Write-Host "🏆 OVERALL PERFORMANCE GRADE: $overallGrade" -ForegroundColor $(if ($overallGrade -like "A*") { "Green" } elseif ($overallGrade -like "B*") { "Yellow" } else { "Red" })
Write-Host ""

# Update summary
$script:PerformanceReport.Summary.TotalThemes = $themeResults.Count
$script:PerformanceReport.Summary.AverageFileSize = $averageFileSize
$script:PerformanceReport.Summary.TotalFileSize = $totalFileSize
$script:PerformanceReport.Summary.FastestLoadingTheme = $fastestTheme.ThemeName
$script:PerformanceReport.Summary.SlowestLoadingTheme = $slowestTheme.ThemeName
$script:PerformanceReport.Summary.MemoryEfficientTheme = $mostEfficientTheme.ThemeName
$script:PerformanceReport.Summary.LargestTheme = $largestTheme.ThemeName
$script:PerformanceReport.Summary.SmallestTheme = $smallestTheme.ThemeName
$script:PerformanceReport.Summary.PerformanceGrade = $overallGrade

# Add switching and regression data to report
$script:PerformanceReport.ThemeSwitchingResults = $switchingResults
$script:PerformanceReport.PerformanceRegressions = $regressions

# Output detailed report if requested
if ($OutputReport) {
    $reportFile = "theme-performance-report.json"
    $script:PerformanceReport | ConvertTo-Json -Depth 10 | Out-File -FilePath $reportFile -Encoding UTF8
    Write-Host "📋 Detailed performance report saved to: $reportFile" -ForegroundColor Green
}

# Performance recommendations
Write-Host "💡 PERFORMANCE RECOMMENDATIONS" -ForegroundColor Green
Write-Host "===============================" -ForegroundColor Green

$recommendations = @()

if ($averageFileSize -gt 15) {
    $recommendations += "Consider further theme file optimization to reduce average file size below 15KB"
}

if ($averageLoadTime -gt 10) {
    $recommendations += "Optimize theme loading performance to achieve sub-10ms average load times"
}

if ($regressions.Count -gt 0) {
    $recommendations += "Address $($regressions.Count) performance regressions identified in themes"
}

$poorPerformingThemes = $themeResults | Where-Object { $_.PerformanceGrade -in @("C", "D") }
if ($poorPerformingThemes.Count -gt 0) {
    $recommendations += "Optimize $($poorPerformingThemes.Count) themes with C or D performance grades"
}

if ($recommendations.Count -eq 0) {
    Write-Host "✅ No performance recommendations needed - all themes performing optimally!"
} else {
    foreach ($i in 0..($recommendations.Count - 1)) {
        Write-Host "   $($i + 1). $($recommendations[$i])"
    }
}

Write-Host ""
Write-Host "✅ Performance testing completed successfully!" -ForegroundColor Green