# MTM Theme Brush Name Update Script
# This script replaces old theme brush names with more practical names solution-wide
# Run from the solution root directory

param(
    [string]$SolutionPath = ".",
    [switch]$WhatIf = $false,
    [switch]$Verbose = $false
)

Write-Host "MTM Theme Brush Name Update Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

if ($WhatIf) {
    Write-Host "PREVIEW MODE - No files will be modified" -ForegroundColor Yellow
    Write-Host ""
}

# Define the replacement mappings
$BrushMappings = @{
    "MTM_Shared_Logic.PrimaryBrush" = "MTM_Shared_Logic.PrimaryAction"
    "MTM_Shared_Logic.SecondaryBrush" = "MTM_Shared_Logic.SecondaryAction"
    "MTM_Shared_Logic.MagentaAccentBrush" = "MTM_Shared_Logic.Warning"
    "MTM_Shared_Logic.BlueAccentBrush" = "MTM_Shared_Logic.Status"
    "MTM_Shared_Logic.PinkAccentBrush" = "MTM_Shared_Logic.Critical"
    "MTM_Shared_Logic.LightPurpleBrush" = "MTM_Shared_Logic.Highlight"
    "MTM_Shared_Logic.PrimaryDarkBrush" = "MTM_Shared_Logic.DarkNavigation"
    "MTM_Shared_Logic.PrimaryLightBrush" = "MTM_Shared_Logic.CardBackground"
    "MTM_Shared_Logic.PrimaryExtraLightBrush" = "MTM_Shared_Logic.HoverBackground"
    "MTM_Shared_Logic.AppBackgroundBrush" = "MTM_Shared_Logic.MainBackground"
    "MTM_Shared_Logic.ContentBackgroundBrush" = "MTM_Shared_Logic.ContentAreas"
    "MTM_Shared_Logic.SidebarBackgroundBrush" = "MTM_Shared_Logic.SidebarDark"
    "MTM_Shared_Logic.HeaderBackgroundBrush" = "MTM_Shared_Logic.PageHeaders"
    "MTM_Shared_Logic.PrimaryTextBrush" = "MTM_Shared_Logic.HeadingText"
    "MTM_Shared_Logic.SecondaryTextBrush" = "MTM_Shared_Logic.BodyText"
    "MTM_Shared_Logic.LightTextBrush" = "MTM_Shared_Logic.TextonDark"
    "MTM_Shared_Logic.AccentTextBrush" = "MTM_Shared_Logic.InteractiveText"
}

# File extensions to search
$FileExtensions = @("*.axaml", "*.cs", "*.xaml", "*.md")

# Get all files to process
$FilesToProcess = @()
foreach ($extension in $FileExtensions) {
    $files = Get-ChildItem -Path $SolutionPath -Filter $extension -Recurse | 
             Where-Object { $_.FullName -notmatch "\\bin\\|\\obj\\|\\.git\\|\\.vs\\|packages\\" }
    $FilesToProcess += $files
}

Write-Host "Files to process: $($FilesToProcess.Count)" -ForegroundColor Green
Write-Host ""

# Track statistics
$TotalReplacements = 0
$FilesModified = 0
$ReplacementStats = @{

}

# Initialize replacement stats
foreach ($key in $BrushMappings.Keys) {
    $ReplacementStats[$key] = 0
}

# Process each file
foreach ($file in $FilesToProcess) {
    $fileModified = $false
    $fileReplacements = 0
    
    try {
        # Read file content
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        
        if ($null -eq $content -or $content.Length -eq 0) {
            continue
        }
        
        $originalContent = $content
        
        # Apply replacements
        foreach ($oldBrush in $BrushMappings.Keys) {
            $newBrush = $BrushMappings[$oldBrush]
            
            # Count occurrences before replacement
            $matches = [regex]::Matches($content, [regex]::Escape($oldBrush))
            $matchCount = $matches.Count
            
            if ($matchCount -gt 0) {
                # Perform replacement
                $content = $content -replace [regex]::Escape($oldBrush), $newBrush
                
                # Update statistics
                $ReplacementStats[$oldBrush] += $matchCount
                $fileReplacements += $matchCount
                
                if ($Verbose) {
                    Write-Host "  $($file.Name): $oldBrush -> $newBrush ($matchCount occurrences)" -ForegroundColor Gray
                }
            }
        }
        
        # Check if file was modified
        if ($content -ne $originalContent) {
            $fileModified = $true
            $FilesModified++
            $TotalReplacements += $fileReplacements
            
            Write-Host "Modified: $($file.FullName.Replace($SolutionPath, '.'))" -ForegroundColor Yellow
            Write-Host "    $fileReplacements replacements made" -ForegroundColor Green
            
            # Write updated content if not in preview mode
            if (-not $WhatIf) {
                Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
            }
        }
    }
    catch {
        Write-Host "Error processing $($file.FullName): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "REPLACEMENT SUMMARY" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
Write-Host "Files processed: $($FilesToProcess.Count)" -ForegroundColor White
Write-Host "Files modified: $FilesModified" -ForegroundColor Green
Write-Host "Total replacements: $TotalReplacements" -ForegroundColor Green
Write-Host ""

# Show detailed replacement statistics
Write-Host "DETAILED REPLACEMENT BREAKDOWN:" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan

$sortedStats = $ReplacementStats.GetEnumerator() | Sort-Object Value -Descending

foreach ($stat in $sortedStats) {
    if ($stat.Value -gt 0) {
        $oldName = $stat.Key -replace "MTM_Shared_Logic\.", ""
        $newName = $BrushMappings[$stat.Key] -replace "MTM_Shared_Logic\.", ""
        Write-Host "Changed: $oldName -> $newName" -ForegroundColor Yellow
        Write-Host "   $($stat.Value) replacements" -ForegroundColor Gray
        Write-Host ""
    }
}

# Show brushes that weren't found
$notFoundBrushes = $sortedStats | Where-Object { $_.Value -eq 0 }
if ($notFoundBrushes.Count -gt 0) {
    Write-Host "BRUSHES NOT FOUND:" -ForegroundColor Blue
    Write-Host "==================" -ForegroundColor Blue
    foreach ($notFound in $notFoundBrushes) {
        $brushName = $notFound.Key -replace "MTM_Shared_Logic\.", ""
        Write-Host "   $brushName (not used in codebase)" -ForegroundColor Gray
    }
    Write-Host ""
}

if ($WhatIf) {
    Write-Host "PREVIEW COMPLETE - No files were modified" -ForegroundColor Yellow
    Write-Host "Run without -WhatIf to apply changes" -ForegroundColor Cyan
} else {
    Write-Host "THEME BRUSH UPDATE COMPLETE!" -ForegroundColor Green
    Write-Host ""
    Write-Host "NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "1. Build the solution to verify no compilation errors" -ForegroundColor White
    Write-Host "2. Test the application to ensure themes work correctly" -ForegroundColor White
    Write-Host "3. Commit the changes to version control" -ForegroundColor White
}

Write-Host ""
Write-Host "MTM Theme Update Script Complete!" -ForegroundColor Green
