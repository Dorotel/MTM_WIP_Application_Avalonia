#!/usr/bin/env pwsh
# =============================================================================
# MTM View Brush Combination Fixer
# Automatically fixes problematic brush combinations found by validation
# Replaces inappropriate brush usage with recommended alternatives
# =============================================================================

param(
    [string]$ViewsPath = "Views",
    [string]$ValidationReportPath = "view-brush-validation-report.json",
    [string]$OutputPath = "view-brush-fixes-report.json",
    [switch]$WhatIf,
    [switch]$VerboseOutput,
    [bool]$BackupFiles = $true,
    [string]$TargetView = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Brush replacement rules based on common issues
$BRUSH_REPLACEMENTS = @{
    # OverlayTextBrush should not be used on light backgrounds
    "OverlayTextBrush_on_MainBackground" = @{
        From = "MTM_Shared_Logic.OverlayTextBrush"
        To = "MTM_Shared_Logic.HeadingText"
        Reason = "OverlayText is for colored backgrounds, not main background"
    }
    "OverlayTextBrush_on_CardBackground" = @{
        From = "MTM_Shared_Logic.OverlayTextBrush"
        To = "MTM_Shared_Logic.HeadingText"
        Reason = "OverlayText is for colored backgrounds, not card background"
    }
    "OverlayTextBrush_on_ContentAreas" = @{
        From = "MTM_Shared_Logic.OverlayTextBrush"
        To = "MTM_Shared_Logic.BodyText"
        Reason = "OverlayText is for colored backgrounds, not content areas"
    }
    "OverlayTextBrush_on_PanelBackground" = @{
        From = "MTM_Shared_Logic.OverlayTextBrush"
        To = "MTM_Shared_Logic.HeadingText"
        Reason = "OverlayText is for colored backgrounds, not panel background"
    }
    
    # Dark text on dark backgrounds
    "HeadingText_on_DarkNavigation" = @{
        From = "MTM_Shared_Logic.HeadingText"
        To = "MTM_Shared_Logic.OverlayTextBrush"
        Reason = "Dark text needs light brush on dark background"
    }
    "BodyText_on_DarkNavigation" = @{
        From = "MTM_Shared_Logic.BodyText"
        To = "MTM_Shared_Logic.OverlayTextBrush"
        Reason = "Dark text needs light brush on dark background"
    }
}

# Context-specific replacements for better semantics
$CONTEXT_REPLACEMENTS = @{
    "Button" = @{
        "HeadingText" = "OverlayTextBrush"  # Buttons typically need overlay text
        "BodyText" = "OverlayTextBrush"
    }
    "TextBlock" = @{
        "OverlayTextBrush" = "HeadingText"  # TextBlocks typically need regular text
    }
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

function Write-Fix($Message) {
    Write-Host "🔨 $Message" -ForegroundColor Cyan
}

function Get-ReplacementBrush($ForegroundBrush, $BackgroundBrush, $ElementType, $Context = "") {
    # Check for specific problematic combinations
    $combinationKey = "${ForegroundBrush}_on_${BackgroundBrush}".Replace("MTM_Shared_Logic.", "")
    
    if ($BRUSH_REPLACEMENTS.ContainsKey($combinationKey)) {
        return $BRUSH_REPLACEMENTS[$combinationKey]
    }
    
    # Check context-specific replacements
    if ($CONTEXT_REPLACEMENTS.ContainsKey($ElementType)) {
        $contextRules = $CONTEXT_REPLACEMENTS[$ElementType]
        $brushKey = $ForegroundBrush.Replace("MTM_Shared_Logic.", "")
        
        if ($contextRules.ContainsKey($brushKey)) {
            return @{
                From = $ForegroundBrush
                To = "MTM_Shared_Logic." + $contextRules[$brushKey]
                Reason = "Context-appropriate brush for $ElementType"
            }
        }
    }
    
    # Default rules based on brush analysis
    if ($ForegroundBrush -eq "MTM_Shared_Logic.OverlayTextBrush") {
        # OverlayText should only be used on colored backgrounds
        if ($BackgroundBrush -like "*Background*" -or $BackgroundBrush -like "*Card*") {
            return @{
                From = $ForegroundBrush
                To = "MTM_Shared_Logic.HeadingText"
                Reason = "OverlayText replaced with HeadingText for light backgrounds"
            }
        }
    }
    
    return $null
}

function Backup-ViewFile($FilePath) {
    if (-not $BackupFiles) {
        return $null
    }
    
    $backupPath = $FilePath + ".backup." + (Get-Date -Format "yyyyMMdd-HHmmss")
    
    try {
        Copy-Item $FilePath $backupPath
        Write-Status "Created backup: $backupPath"
        return $backupPath
    } catch {
        Write-Warning "Failed to create backup for $FilePath`: $($_.Exception.Message)"
        return $null
    }
}

function Repair-BrushCombination($ViewPath, $Issue) {
    $replacement = Get-ReplacementBrush $Issue.ForegroundBrush $Issue.BackgroundBrush $Issue.ElementType
    
    if (-not $replacement) {
        Write-Warning "No replacement rule found for: $($Issue.ForegroundBrush) on $($Issue.BackgroundBrush)"
        return $null
    }
    
    try {
        $content = Get-Content $ViewPath -Raw -ErrorAction Stop
        $originalContent = $content
        
        # Pattern to find the specific foreground brush usage
        $pattern = 'Foreground\s*=\s*"\{DynamicResource\s+' + [regex]::Escape($replacement.From) + '\}"'
        $replacementText = 'Foreground="{DynamicResource ' + $replacement.To + '}"'
        
        # Count matches before replacement
        $beforeMatches = [regex]::Matches($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        $matchCount = $beforeMatches.Count
        
        if ($matchCount -eq 0) {
            Write-Warning "No matches found for pattern in $ViewPath"
            return $null
        }
        
        # Apply the replacement
        $content = [regex]::Replace($content, $pattern, $replacementText, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        
        # Verify the replacement worked
        $afterMatches = [regex]::Matches($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        $remainingMatches = $afterMatches.Count
        $actualReplacements = $matchCount - $remainingMatches
        
        if ($actualReplacements -eq 0) {
            Write-Warning "No replacements made in $ViewPath"
            return $null
        }
        
        $fixResult = @{
            ViewPath = $ViewPath
            ViewName = Split-Path $ViewPath -Leaf
            OriginalBrush = $replacement.From
            ReplacementBrush = $replacement.To
            Reason = $replacement.Reason
            ElementType = $Issue.ElementType
            BackgroundBrush = $Issue.BackgroundBrush
            MatchesFound = $matchCount
            ReplacementsMade = $actualReplacements
            NewContent = $content
            OriginalContent = $originalContent
        }
        
        return $fixResult
    } catch {
        Write-Error "Failed to fix brush combination in $ViewPath`: $($_.Exception.Message)"
        return $null
    }
}

function Invoke-ViewFixes($ViewPath, $Issues) {
    Write-Status "Fixing brush combinations in: $(Split-Path $ViewPath -Leaf)"
    
    $fixes = @()
    $backupPath = $null
    
    # Group issues by the brush being replaced to avoid multiple changes to same brush
    $groupedIssues = $Issues | Group-Object { "$($_.ForegroundBrush)|$($_.BackgroundBrush)" }
    
    foreach ($group in $groupedIssues) {
        $issue = $group.Group[0]  # Take first issue from group
        
        $fix = Repair-BrushCombination $ViewPath $issue
        if ($fix) {
            $fixes += $fix
            
            if ($VerboseOutput) {
                Write-Fix "$(Split-Path $ViewPath -Leaf): $($fix.OriginalBrush) → $($fix.ReplacementBrush)"
                Write-Host "   Reason: $($fix.Reason)" -ForegroundColor Gray
                Write-Host "   Matches: $($fix.MatchesFound), Replaced: $($fix.ReplacementsMade)" -ForegroundColor Gray
            }
        }
    }
    
    if (-not $fixes -or $fixes.Count -eq 0) {
        Write-Warning "No fixes applied to $(Split-Path $ViewPath -Leaf)"
        return @()
    }
    
    # Apply all fixes by using the final content from the last fix
    if ($fixes -and $fixes.Count -gt 0) {
        $finalContent = $fixes[-1].NewContent
    } else {
        return @()
    }
    
    if (-not $WhatIf) {
        # Create backup before making changes
        $backupPath = Backup-ViewFile $ViewPath
        
        try {
            # Write the fixed content
            Set-Content $ViewPath $finalContent -Encoding UTF8
            Write-Success "Applied $($fixes.Count) fixes to $(Split-Path $ViewPath -Leaf)"
        } catch {
            Write-Error "Failed to write fixes to $ViewPath`: $($_.Exception.Message)"
            
            # Restore from backup if available
            if ($backupPath -and (Test-Path $backupPath)) {
                try {
                    Copy-Item $backupPath $ViewPath
                    Write-Warning "Restored original file from backup"
                } catch {
                    Write-Error "Failed to restore from backup: $($_.Exception.Message)"
                }
            }
            return @()
        }
    } else {
        Write-Host "WHAT-IF: Would apply $($fixes.Count) fixes to $(Split-Path $ViewPath -Leaf)" -ForegroundColor Yellow
        foreach ($fix in $fixes) {
            Write-Host "  → $($fix.OriginalBrush) to $($fix.ReplacementBrush): $($fix.ReplacementsMade) replacements" -ForegroundColor Yellow
        }
    }
    
    # Add backup path to fixes
    foreach ($fix in $fixes) {
        $fix.BackupPath = $backupPath
    }
    
    return $fixes
}

function Import-ValidationReport($ReportPath) {
    if (-not (Test-Path $ReportPath)) {
        Write-Error "Validation report not found: $ReportPath"
        Write-Warning "Please run validate-view-brush-combinations.ps1 first to generate the report"
        return $null
    }
    
    try {
        $reportJson = Get-Content $ReportPath -Raw | ConvertFrom-Json
        return $reportJson
    } catch {
        Write-Error "Failed to load validation report: $($_.Exception.Message)"
        return $null
    }
}

function Main {
    Write-Host ""
    Write-Host "🔨 MTM VIEW BRUSH COMBINATION FIXER" -ForegroundColor Cyan
    Write-Host "===================================" -ForegroundColor Cyan
    Write-Host ""

    if ($WhatIf) {
        Write-Warning "RUNNING IN WHAT-IF MODE - No changes will be made"
        Write-Host ""
    }

    # Load validation report
    Write-Status "Loading validation report from: $ValidationReportPath"
    $validationReport = Import-ValidationReport $ValidationReportPath
    
    if (-not $validationReport) {
        return
    }

    Write-Status "Report loaded: $($validationReport.Summary.TotalViews) views analyzed"
    Write-Status "Issues to fix: $($validationReport.Summary.TotalHighWarnings) high warnings, $($validationReport.Summary.TotalCriticalIssues) critical issues"
    Write-Host ""

    # Filter to problematic views only
    $problematicViews = $validationReport.ViewResults | Where-Object { 
        $_.OverallStatus -eq "HIGH" -or $_.OverallStatus -eq "CRITICAL" -or $_.OverallStatus -eq "MEDIUM"
    }

    if ($problematicViews.Count -eq 0) {
        Write-Success "No problematic brush combinations found to fix!"
        return
    }

    # Apply target view filter if specified
    if ($TargetView) {
        $problematicViews = $problematicViews | Where-Object { $_.ViewName -like "*$TargetView*" }
        if ($problematicViews.Count -eq 0) {
            Write-Warning "No problematic views found matching '$TargetView'"
            return
        }
        Write-Status "Filtering to target view: $TargetView ($($problematicViews.Count) views)"
    }

    Write-Host "🎯 VIEWS TO FIX:" -ForegroundColor Yellow
    Write-Host "=================" -ForegroundColor Yellow
    foreach ($view in $problematicViews) {
        $criticalCount = if ($view.CriticalIssues) { $view.CriticalIssues.Count } else { 0 }
        $highCount = if ($view.HighWarnings) { $view.HighWarnings.Count } else { 0 }
        $mediumCount = if ($view.MediumWarnings) { $view.MediumWarnings.Count } else { 0 }
        $totalIssues = $criticalCount + $highCount + $mediumCount
        Write-Host "  • $($view.ViewName): $totalIssues issues ($($view.OverallStatus))" -ForegroundColor Yellow
    }
    Write-Host ""

    # Process each problematic view
    $allFixes = @()
    $processedViews = 0
    $successfullyFixed = 0

    Write-Host "🔧 APPLYING FIXES:" -ForegroundColor Cyan
    Write-Host "==================" -ForegroundColor Cyan
    Write-Host ""

    foreach ($view in $problematicViews) {
        $processedViews++
        
        # Collect all issues for this view
        $allIssues = @()
        $allIssues += $view.CriticalIssues
        $allIssues += $view.HighWarnings
        $allIssues += $view.MediumWarnings
        
        if ($allIssues -and $allIssues.Count -eq 0) {
            continue
        }

        Write-Status "Processing $($view.ViewName) ($processedViews/$($problematicViews.Count))"
        
        try {
            $fixes = Invoke-ViewFixes $view.ViewFile $allIssues
            
            if ($fixes -and $fixes.Count -gt 0) {
                $allFixes += $fixes
                $successfullyFixed++
            }
        } catch {
            Write-Error "Failed to process $($view.ViewName): $($_.Exception.Message)"
        }
        
        Write-Host ""
    }

    # Generate summary report
    $summary = @{
        FixDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        Mode = if ($WhatIf) { "What-If" } else { "Applied" }
        ProcessedViews = $processedViews
        SuccessfullyFixed = $successfullyFixed
        TotalFixes = if ($allFixes) { $allFixes.Count } else { 0 }
        BackupsCreated = @($allFixes | Where-Object { $_.BackupPath }).Count
        FixDetails = $allFixes
    }

    # Export results
    $summary | ConvertTo-Json -Depth 10 | Out-File $OutputPath -Encoding UTF8
    
    # Display final summary
    Write-Host "📊 BRUSH COMBINATION FIXES SUMMARY" -ForegroundColor Cyan
    Write-Host "===================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Views processed: $processedViews" -ForegroundColor White
    Write-Host "Successfully fixed: $successfullyFixed" -ForegroundColor Green
    Write-Host "Total brush replacements: $(if ($allFixes) { $allFixes.Count } else { 0 })" -ForegroundColor Green
    
    if ($BackupFiles -and -not $WhatIf) {
        Write-Host "Backup files created: $($summary.BackupsCreated)" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Status "Detailed fix report saved to: $OutputPath"
    
    if ($allFixes -and $allFixes.Count -gt 0) {
        Write-Host ""
        Write-Host "🔍 TOP FIXES APPLIED:" -ForegroundColor Yellow
        $topFixes = $allFixes | Group-Object { "$($_.OriginalBrush) → $($_.ReplacementBrush)" } | 
                   Sort-Object Count -Descending | Select-Object -First 5
        
        foreach ($fixGroup in $topFixes) {
            $totalReplacements = ($fixGroup.Group | Measure-Object -Property ReplacementsMade -Sum).Sum
            Write-Host "  • $($fixGroup.Name): $totalReplacements replacements in $($fixGroup.Count) views" -ForegroundColor Yellow
        }
    }
    
    if (-not $WhatIf) {
        Write-Success "🎉 BRUSH COMBINATION FIXES COMPLETE!"
        Write-Host ""
        Write-Status "Next steps:"
        Write-Host "  1. Test the application with different themes"
        Write-Host "  2. Run validate-view-brush-combinations.ps1 again to verify fixes"
        Write-Host "  3. Run validate-wcag-compliance.ps1 to check contrast ratios"
        
        if ($BackupFiles) {
            Write-Host "  4. Remove .backup files once you're satisfied with the changes"
        }
    } else {
        Write-Warning "This was a What-If run - no changes were made"
        Write-Host "  • Remove -WhatIf parameter to apply the fixes"
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "Brush combination fixer failed: $($_.Exception.Message)"
    exit 1
}
