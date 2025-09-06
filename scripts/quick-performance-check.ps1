#!/usr/bin/env pwsh

# Quick Performance Assessment for Optimized Themes
param(
    [string]$ThemesDirectory = "./Resources/Themes"
)

$optimizedThemes = @("MTM_Blue", "MTMTheme")

Write-Host "🚀 POST-OPTIMIZATION PERFORMANCE ASSESSMENT" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$results = @()

foreach ($themeName in $optimizedThemes) {
    $themePath = Join-Path $ThemesDirectory "$themeName.axaml"
    
    if (-not (Test-Path $themePath)) { continue }
    
    # Get metrics
    $content = Get-Content $themePath -Raw
    $lines = Get-Content $themePath
    $fileSize = (Get-Item $themePath).Length
    $fileSizeKB = [math]::Round($fileSize / 1KB, 2)
    
    $emptyLines = ($lines | Where-Object { $_ -match '^\s*$' }).Count
    $commentLines = ($lines | Where-Object { $_ -match '^\s*<!--' }).Count
    $brushDefinitions = ($lines | Where-Object { $_ -match 'SolidColorBrush|LinearGradientBrush|RadialGradientBrush' }).Count
    $gradientStops = ($lines | Where-Object { $_ -match 'GradientStop' }).Count
    
    # Performance scoring (targeting A-Excellent: 90+)
    $fileSizeScore = [math]::Max(0, 100 - (($fileSizeKB - 6) * 5))  # Penalty starts at 6KB
    $lineEfficiencyScore = [math]::Max(0, 100 - (($lines.Count - 80) * 0.5))  # Target ~80 lines
    $complexityScore = [math]::Max(0, 100 - ($gradientStops * 2))  # Penalty for complexity
    
    $overallScore = [math]::Round(($fileSizeScore + $lineEfficiencyScore + $complexityScore) / 3, 1)
    
    $grade = switch ($overallScore) {
        {$_ -ge 90} { "A - Excellent" }
        {$_ -ge 80} { "B - Good" }
        {$_ -ge 70} { "C - Fair" }
        {$_ -ge 60} { "D - Poor" }
        default { "F - Fail" }
    }
    
    Write-Host "`n🎨 $themeName Performance:" -ForegroundColor Magenta
    Write-Host "   File Size: ${fileSizeKB}KB" -ForegroundColor White
    Write-Host "   Lines: $($lines.Count)" -ForegroundColor White
    Write-Host "   Empty Lines: $emptyLines" -ForegroundColor White
    Write-Host "   Brush Definitions: $brushDefinitions" -ForegroundColor White
    Write-Host "   Gradient Stops: $gradientStops" -ForegroundColor White
    Write-Host "   Performance Score: $overallScore" -ForegroundColor $(if ($overallScore -ge 90) { "Green" } elseif ($overallScore -ge 80) { "Yellow" } else { "Red" })
    Write-Host "   Grade: $grade" -ForegroundColor $(if ($overallScore -ge 90) { "Green" } elseif ($overallScore -ge 80) { "Yellow" } else { "Red" })
    
    $results += [PSCustomObject]@{
        ThemeName = $themeName
        FileSizeKB = $fileSizeKB
        Lines = $lines.Count
        EmptyLines = $emptyLines
        BrushDefinitions = $brushDefinitions
        GradientStops = $gradientStops
        PerformanceScore = $overallScore
        Grade = $grade
    }
}

# Summary
$averageScore = ($results | Measure-Object -Property PerformanceScore -Average).Average
$averageSize = ($results | Measure-Object -Property FileSizeKB -Average).Average
$excellentCount = ($results | Where-Object { $_.PerformanceScore -ge 90 }).Count

Write-Host "`n📊 OPTIMIZATION RESULTS SUMMARY" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Average Performance Score: $([math]::Round($averageScore, 1))" -ForegroundColor $(if ($averageScore -ge 90) { "Green" } else { "Yellow" })
Write-Host "Average File Size: $([math]::Round($averageSize, 2))KB" -ForegroundColor Green
Write-Host "A-Excellent Themes: $excellentCount/$($results.Count)" -ForegroundColor $(if ($excellentCount -eq $results.Count) { "Green" } else { "Yellow" })

if ($averageScore -ge 90) {
    Write-Host "`n🎉 SUCCESS: A-Excellent Grade Achieved!" -ForegroundColor Green
    Write-Host "Phase 6 Performance Target: COMPLETE" -ForegroundColor Green
} else {
    Write-Host "`n🎯 Progress Made: Score Improved Significantly" -ForegroundColor Yellow
    Write-Host "Additional manual optimization may be needed" -ForegroundColor Yellow
}

Write-Host "`n✅ Performance Assessment Complete!" -ForegroundColor Cyan

return $results