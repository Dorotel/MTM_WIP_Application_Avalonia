# Hardcoded Color Detection Script
# PowerShell script to detect hardcoded colors in AXAML files for MTM Theme Standardization EPIC

param(
    [string]$ViewsPath = "Views",
    [string]$OutputPath = "hardcoded-colors-report.json",
    [switch]$VerboseOutput = $false,
    [switch]$FixMode = $false
)

Write-Host "🔍 MTM Hardcoded Color Detection Script" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

$results = @{
    ScanDate = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    ScanPath = $ViewsPath
    FilesScanned = 0
    FilesWithIssues = 0
    TotalIssues = 0
    FileResults = @()
    Summary = @{
        HighPriorityFiles = @()
        MediumPriorityFiles = @()
        LowPriorityFiles = @()
    }
}

# Comprehensive hardcoded color patterns
$hardcodedPatterns = @(
    # Hex color patterns
    @{ Pattern = 'Color\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "6-digit hex color"; Severity = "HIGH" },
    @{ Pattern = 'Color\s*=\s*"#[0-9A-Fa-f]{8}"'; Description = "8-digit hex color with alpha"; Severity = "HIGH" },
    
    # Background/Foreground with hex colors
    @{ Pattern = 'Background\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "Hardcoded background hex"; Severity = "CRITICAL" },
    @{ Pattern = 'Background\s*=\s*"#[0-9A-Fa-f]{8}"'; Description = "Hardcoded background hex with alpha"; Severity = "CRITICAL" },
    @{ Pattern = 'Foreground\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "Hardcoded foreground hex"; Severity = "CRITICAL" },
    @{ Pattern = 'Foreground\s*=\s*"#[0-9A-Fa-f]{8}"'; Description = "Hardcoded foreground hex with alpha"; Severity = "CRITICAL" },
    
    # Named colors
    @{ Pattern = 'Background\s*=\s*"(White|Black|Red|Blue|Green|Yellow|Gray|Orange|Purple|Pink|Brown|Cyan|Magenta)"'; Description = "Named background color"; Severity = "HIGH" },
    @{ Pattern = 'Foreground\s*=\s*"(White|Black|Red|Blue|Green|Yellow|Gray|Orange|Purple|Pink|Brown|Cyan|Magenta)"'; Description = "Named foreground color"; Severity = "HIGH" },
    @{ Pattern = 'Color\s*=\s*"(White|Black|Red|Blue|Green|Yellow|Gray|Orange|Purple|Pink|Brown|Cyan|Magenta)"'; Description = "Named color value"; Severity = "MEDIUM" },
    
    # BorderBrush hardcoded colors  
    @{ Pattern = 'BorderBrush\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "Hardcoded border hex"; Severity = "HIGH" },
    @{ Pattern = 'BorderBrush\s*=\s*"(White|Black|Red|Blue|Green|Yellow|Gray)"'; Description = "Named border color"; Severity = "MEDIUM" },
    
    # Fill and Stroke for shapes
    @{ Pattern = 'Fill\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "Hardcoded fill hex"; Severity = "MEDIUM" },
    @{ Pattern = 'Stroke\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "Hardcoded stroke hex"; Severity = "MEDIUM" },
    
    # SolidColorBrush inline definitions (not theme references)
    @{ Pattern = '<SolidColorBrush\s+Color\s*=\s*"#[0-9A-Fa-f]{6}"'; Description = "Inline SolidColorBrush hex"; Severity = "HIGH" }
)

# MTM-specific correct patterns to suggest as replacements
$replacementSuggestions = @{
    # Primary colors
    "1E88E5" = "MTM_Shared_Logic.PrimaryAction"
    "0078D4" = "MTM_Shared_Logic.PrimaryAction"
    "106EBE" = "MTM_Shared_Logic.SecondaryAction"
    
    # Semantic colors  
    "4CAF50" = "MTM_Shared_Logic.SuccessBrush"
    "F44336" = "MTM_Shared_Logic.ErrorBrush"
    "FFA726" = "MTM_Shared_Logic.Warning"
    "FF9800" = "MTM_Shared_Logic.WarningBrush"
    
    # Background colors
    "FFFFFF" = "MTM_Shared_Logic.MainBackground"
    "F8F9FA" = "MTM_Shared_Logic.CardBackgroundBrush"
    "000000" = "MTM_Shared_Logic.DarkNavigation"
    
    # Text colors
    "323130" = "MTM_Shared_Logic.HeadingText"
    "605E5C" = "MTM_Shared_Logic.BodyText"
    
    # Named color replacements
    "White" = "MTM_Shared_Logic.MainBackground"
    "Black" = "MTM_Shared_Logic.HeadingText"
    "Red" = "MTM_Shared_Logic.ErrorBrush"
    "Green" = "MTM_Shared_Logic.SuccessBrush"
    "Blue" = "MTM_Shared_Logic.PrimaryAction"
    "Gray" = "MTM_Shared_Logic.BodyText"
    "Orange" = "MTM_Shared_Logic.Warning"
}

function Get-MTMReplacementSuggestion {
    param([string]$ColorValue, [string]$PropertyType)
    
    # Remove # from hex values
    $cleanColor = $ColorValue -replace '#', ''
    
    # Check direct color mapping
    if ($replacementSuggestions.ContainsKey($cleanColor)) {
        return $replacementSuggestions[$cleanColor]
    }
    
    # Property-based suggestions
    switch ($PropertyType) {
        "Background" {
            return "MTM_Shared_Logic.CardBackgroundBrush"
        }
        "Foreground" {
            return "MTM_Shared_Logic.BodyText"
        }
        "BorderBrush" {
            return "MTM_Shared_Logic.BorderBrush"
        }
        default {
            return "MTM_Shared_Logic.PrimaryAction"
        }
    }
}

function Analyze-ViewFile {
    param([string]$FilePath)
    
    $fileResult = @{
        FilePath = $FilePath
        FileName = (Split-Path $FilePath -Leaf)
        Category = ""
        Issues = @()
        Priority = "LOW"
        TotalIssues = 0
    }
    
    # Determine file category
    if ($FilePath -like "*MainForm*") {
        $fileResult.Category = "MainForm"
    } elseif ($FilePath -like "*SettingsForm*") {
        $fileResult.Category = "SettingsForm"
    } elseif ($FilePath -like "*TransactionsForm*") {
        $fileResult.Category = "TransactionsForm"
    }
    
    # Priority files
    if ($fileResult.FileName -eq "ThemeBuilderView.axaml") {
        $fileResult.Priority = "CRITICAL"
    }
    
    $content = Get-Content $FilePath -Raw
    $lines = Get-Content $FilePath
    
    foreach ($patternInfo in $hardcodedPatterns) {
        $matches = [regex]::Matches($content, $patternInfo.Pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        
        foreach ($match in $matches) {
            # Find line number
            $lineNumber = 1
            $position = 0
            foreach ($line in $lines) {
                $position += $line.Length + 1  # +1 for newline
                if ($position -gt $match.Index) {
                    break
                }
                $lineNumber++
            }
            
            # Extract color value and property type
            $matchText = $match.Value
            $colorValue = ""
            $propertyType = ""
            
            if ($matchText -match '(\w+)\s*=\s*"([^"]+)"') {
                $propertyType = $matches[1]
                $colorValue = $matches[2]
            }
            
            $issue = @{
                LineNumber = $lineNumber
                LineContent = $lines[$lineNumber - 1].Trim()
                MatchedPattern = $patternInfo.Description
                Severity = $patternInfo.Severity
                ColorValue = $colorValue
                PropertyType = $propertyType
                SuggestedReplacement = Get-MTMReplacementSuggestion $colorValue $propertyType
                RecommendedFix = ""
            }
            
            # Generate recommended fix
            if ($propertyType -and $colorValue) {
                $suggestion = $issue.SuggestedReplacement
                $issue.RecommendedFix = $matchText -replace '"[^"]+"', "{DynamicResource $suggestion}"
            }
            
            $fileResult.Issues += $issue
            $fileResult.TotalIssues++
            
            # Escalate priority based on severity
            if ($patternInfo.Severity -eq "CRITICAL" -and $fileResult.Priority -ne "CRITICAL") {
                $fileResult.Priority = "HIGH"
            } elseif ($patternInfo.Severity -eq "HIGH" -and $fileResult.Priority -eq "LOW") {
                $fileResult.Priority = "MEDIUM"
            }
        }
    }
    
    return $fileResult
}

# Scan all AXAML view files
Write-Host "Scanning for AXAML files in: $ViewsPath" -ForegroundColor Yellow

Get-ChildItem -Path $ViewsPath -Filter "*.axaml" -Recurse | ForEach-Object {
    $results.FilesScanned++
    
    if ($VerboseOutput) {
        Write-Host "Scanning: $($_.FullName)" -ForegroundColor Gray
    }
    
    $fileResult = Analyze-ViewFile $_.FullName
    
    if ($fileResult.TotalIssues -gt 0) {
        $results.FilesWithIssues++
        $results.TotalIssues += $fileResult.TotalIssues
        $results.FileResults += $fileResult
        
        # Categorize by priority
        switch ($fileResult.Priority) {
            "CRITICAL" { $results.Summary.HighPriorityFiles += $fileResult.FileName }
            "HIGH" { $results.Summary.HighPriorityFiles += $fileResult.FileName }
            "MEDIUM" { $results.Summary.MediumPriorityFiles += $fileResult.FileName }
            "LOW" { $results.Summary.LowPriorityFiles += $fileResult.FileName }
        }
        
        Write-Host "❌ $($_.Name): $($fileResult.TotalIssues) issues ($($fileResult.Priority) priority)" -ForegroundColor Red
        
        if ($VerboseOutput) {
            foreach ($issue in $fileResult.Issues) {
                Write-Host "   Line $($issue.LineNumber): $($issue.MatchedPattern)" -ForegroundColor Yellow
                Write-Host "   Suggestion: $($issue.SuggestedReplacement)" -ForegroundColor Green
            }
        }
    } else {
        Write-Host "✅ $($_.Name): No hardcoded colors detected" -ForegroundColor Green
    }
}

# Generate report
$results | ConvertTo-Json -Depth 6 | Out-File $OutputPath

# Display summary
Write-Host "`n📊 HARDCODED COLOR DETECTION SUMMARY" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Files scanned: $($results.FilesScanned)"
Write-Host "Files with issues: $($results.FilesWithIssues)" -ForegroundColor Red
Write-Host "Total issues found: $($results.TotalIssues)" -ForegroundColor Red
Write-Host "High priority files: $($results.Summary.HighPriorityFiles.Count)" -ForegroundColor Red
Write-Host "Medium priority files: $($results.Summary.MediumPriorityFiles.Count)" -ForegroundColor Yellow  
Write-Host "Low priority files: $($results.Summary.LowPriorityFiles.Count)" -ForegroundColor Yellow

if ($results.Summary.HighPriorityFiles.Count -gt 0) {
    Write-Host "`n🚨 HIGH PRIORITY FILES:" -ForegroundColor Red
    foreach ($file in $results.Summary.HighPriorityFiles) {
        Write-Host "  • $file" -ForegroundColor Red
    }
}

if ($results.FileResults.Count -gt 0) {
    Write-Host "`n🔍 TOP ISSUES TO FIX:" -ForegroundColor Yellow
    $topIssues = $results.FileResults | Sort-Object { 
        switch ($_.Priority) {
            "CRITICAL" { 0 }
            "HIGH" { 1 }
            "MEDIUM" { 2 }  
            "LOW" { 3 }
        }
    } | Select-Object -First 5
    
    foreach ($file in $topIssues) {
        Write-Host "`n📄 $($file.FileName) ($($file.Category)) - Priority: $($file.Priority)" -ForegroundColor Cyan
        $criticalIssues = $file.Issues | Where-Object { $_.Severity -eq "CRITICAL" -or $_.Severity -eq "HIGH" } | Select-Object -First 3
        
        foreach ($issue in $criticalIssues) {
            Write-Host "   Line $($issue.LineNumber): $($issue.ColorValue) → $($issue.SuggestedReplacement)" -ForegroundColor Yellow
            Write-Host "   Fix: $($issue.RecommendedFix)" -ForegroundColor Green
        }
    }
}

Write-Host "`n📋 NEXT STEPS:" -ForegroundColor Cyan
Write-Host "1. Review high priority files first"
Write-Host "2. Use suggested MTM_Shared_Logic replacements"
Write-Host "3. Test theme switching after fixes"
Write-Host "4. Run WCAG contrast validation"

Write-Host "`n📄 Detailed report saved to: $OutputPath" -ForegroundColor Green

# Exit with error code if critical issues found
if ($results.Summary.HighPriorityFiles.Count -gt 0) {
    Write-Host "`n⚠️  Exiting with error code due to high priority issues" -ForegroundColor Red
    exit 1
} else {
    Write-Host "`n✅ No critical hardcoded color issues detected" -ForegroundColor Green
    exit 0
}