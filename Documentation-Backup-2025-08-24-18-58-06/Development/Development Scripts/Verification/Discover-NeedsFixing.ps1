#!/usr/bin/env pwsh
# Discover-NeedsFixing.ps1  
# Extract items from needsrepair.instruction.md and other quality issues for tracking

param(
    [string]$ProjectRoot = ".",
    [switch]$Verbose = $false
)

Write-Host "=== MTM WIP Application - NeedsFixing Discovery ===" -ForegroundColor Cyan
Write-Host "Extracting items requiring attention from quality instruction files..." -ForegroundColor Yellow

# Define quality instruction files to analyze
$QualityFiles = @(
    ".github/Quality-Instructions/needsrepair.instruction.md",
    ".github/copilot-instructions.md",
    "Documentation/Agent-Documentation-Enhancement/*.md"
)

$NeedsFixingItems = @()
$PriorityLevels = @{
    High = @()
    Medium = @()
    Low = @()
}

foreach ($Pattern in $QualityFiles) {
    $Files = Get-ChildItem -Path (Join-Path $ProjectRoot $Pattern) -ErrorAction SilentlyContinue
    
    foreach ($File in $Files) {
        if ($Verbose) {
            Write-Host "  🔍 Analyzing: $($File.Name)" -ForegroundColor Gray
        }
        
        $Content = Get-Content $File.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $Content) { continue }
        
        $RelativePath = $File.FullName.Replace((Resolve-Path $ProjectRoot).Path + [System.IO.Path]::DirectorySeparatorChar, "")
        
        # Split content into lines for analysis
        $Lines = $Content -split "`n"
        $CurrentSection = ""
        $LineNumber = 0
        
        foreach ($Line in $Lines) {
            $LineNumber++
            $TrimmedLine = $Line.Trim()
            
            if ([string]::IsNullOrWhiteSpace($TrimmedLine)) { continue }
            
            # Track current section
            if ($TrimmedLine -match '^#+ (.+)') {
                $CurrentSection = $Matches[1]
                continue
            }
            
            # Priority indicators and issue patterns
            $Priority = "Medium"  # Default priority
            $IsIssue = $false
            $IssueType = "General"
            
            # High priority patterns
            if ($TrimmedLine -match '🔥|CRITICAL|HIGH|⚠️|❌|MUST|REQUIRED|BREAKING') {
                $Priority = "High"
                $IsIssue = $true
                $IssueType = "Critical"
            }
            # Medium priority patterns  
            elseif ($TrimmedLine -match '⚡|MEDIUM|TODO|FIXME|HACK|TEMP|INCOMPLETE') {
                $Priority = "Medium"
                $IsIssue = $true
                $IssueType = "Enhancement"
            }
            # Low priority patterns
            elseif ($TrimmedLine -match '💡|LOW|NICE.*TO.*HAVE|CONSIDER|OPTIONAL|FUTURE') {
                $Priority = "Low"
                $IsIssue = $true
                $IssueType = "Improvement"
            }
            # Issue indicators without explicit priority
            elseif ($TrimmedLine -match '^- \[ \]|^- ❌|^- ⚠️|^- 🔧|BUG|ISSUE|PROBLEM|ERROR') {
                $IsIssue = $true
                $IssueType = "Task"
            }
            
            # Extract specific technical issues
            if ($TrimmedLine -match 'WRONG|INCORRECT|BROKEN|MISSING|DEPRECATED|OUTDATED') {
                $IsIssue = $true
                $IssueType = "Technical Debt"
                if ($Priority -eq "Medium") { $Priority = "High" }
            }
            
            if ($IsIssue) {
                # Clean up the line for display
                $CleanedLine = $TrimmedLine -replace '^[-*]\s*', '' -replace '^[\[\]x\s]*', ''
                $CleanedLine = $CleanedLine -replace '[🔥⚠️❌⚡💡🔧]', '' 
                $CleanedLine = $CleanedLine.Trim()
                
                if ([string]::IsNullOrWhiteSpace($CleanedLine)) { continue }
                
                # Extract action items
                $ActionRequired = ""
                if ($CleanedLine -match '(implement|create|fix|update|add|remove|refactor|migrate)', 'IgnoreCase') {
                    $ActionRequired = $Matches[1].ToLower()
                }
                
                $Item = [PSCustomObject]@{
                    Id = [System.Guid]::NewGuid().ToString("N")[0..7] -join ""
                    Priority = $Priority
                    Type = $IssueType
                    Description = $CleanedLine
                    Section = $CurrentSection
                    SourceFile = $File.Name
                    SourcePath = $RelativePath
                    LineNumber = $LineNumber
                    ActionRequired = $ActionRequired
                    DateDiscovered = Get-Date -Format "yyyy-MM-dd"
                    Status = "Open"
                    EstimatedEffort = if ($Priority -eq "High") { "High" } elseif ($Priority -eq "Low") { "Low" } else { "Medium" }
                }
                
                $NeedsFixingItems += $Item
                $PriorityLevels[$Priority] += $Item
            }
        }
    }
}

# Additional discovery from code comments
Write-Host "`n🔍 Scanning code files for TODO/FIXME comments..." -ForegroundColor Yellow

$CodeFiles = Get-ChildItem -Path $ProjectRoot -Include "*.cs", "*.axaml", "*.md" -Recurse | Where-Object {
    $_.Name -notlike "*Test*" -and $_.Directory.Name -ne "bin" -and $_.Directory.Name -ne "obj"
}

foreach ($File in $CodeFiles) {
    $Content = Get-Content $File.FullName -Raw -ErrorAction SilentlyContinue
    if (-not $Content) { continue }
    
    $RelativePath = $File.FullName.Replace((Resolve-Path $ProjectRoot).Path + [System.IO.Path]::DirectorySeparatorChar, "")
    $Lines = $Content -split "`n"
    $LineNumber = 0
    
    foreach ($Line in $Lines) {
        $LineNumber++
        $TrimmedLine = $Line.Trim()
        
        # Look for code comments with action items
        if ($TrimmedLine -match '//.*?(TODO|FIXME|HACK|BUG|UNDONE|REVIEW)[:.]?\s*(.+)', 'IgnoreCase') {
            $CommentType = $Matches[1].ToUpper()
            $CommentText = $Matches[2].Trim()
            
            $Priority = switch ($CommentType) {
                "BUG" { "High" }
                "FIXME" { "High" }
                "TODO" { "Medium" }
                "HACK" { "Medium" }
                "REVIEW" { "Low" }
                "UNDONE" { "Medium" }
                default { "Medium" }
            }
            
            $Item = [PSCustomObject]@{
                Id = [System.Guid]::NewGuid().ToString("N")[0..7] -join ""
                Priority = $Priority
                Type = "Code Comment"
                Description = "$CommentType: $CommentText"
                Section = "Code Analysis"
                SourceFile = $File.Name
                SourcePath = $RelativePath
                LineNumber = $LineNumber
                ActionRequired = $CommentType.ToLower()
                DateDiscovered = Get-Date -Format "yyyy-MM-dd"
                Status = "Open"
                EstimatedEffort = if ($Priority -eq "High") { "High" } else { "Low" }
            }
            
            $NeedsFixingItems += $Item
            $PriorityLevels[$Priority] += $Item
        }
    }
}

# Results summary
Write-Host "`n=== NeedsFixing Discovery Results ===" -ForegroundColor Green

Write-Host "`n📊 Priority Distribution:" -ForegroundColor Cyan
Write-Host "  🔥 High Priority: $($PriorityLevels.High.Count) items" -ForegroundColor Red
Write-Host "  ⚡ Medium Priority: $($PriorityLevels.Medium.Count) items" -ForegroundColor Yellow  
Write-Host "  💡 Low Priority: $($PriorityLevels.Low.Count) items" -ForegroundColor Green
Write-Host "  📄 Total Items: $($NeedsFixingItems.Count)" -ForegroundColor White

# Type analysis
$TypeGroups = $NeedsFixingItems | Group-Object Type | Sort-Object Count -Descending
Write-Host "`n📋 Issue Types:" -ForegroundColor Cyan
foreach ($Group in $TypeGroups) {
    Write-Host "  🔧 $($Group.Name): $($Group.Count) items" -ForegroundColor White
}

# Source file analysis
$SourceGroups = $NeedsFixingItems | Group-Object SourceFile | Sort-Object Count -Descending | Select-Object -First 5
Write-Host "`n📁 Top Source Files:" -ForegroundColor Cyan
foreach ($Group in $SourceGroups) {
    Write-Host "  📄 $($Group.Name): $($Group.Count) items" -ForegroundColor White
}

# Generate HTML tracking files
Write-Host "`n🌐 Generating HTML tracking pages..." -ForegroundColor Yellow

$NeedsFixingPath = Join-Path $ProjectRoot "docs/NeedsFixing"
if (-not (Test-Path $NeedsFixingPath)) {
    New-Item -Path $NeedsFixingPath -ItemType Directory -Force | Out-Null
}

# Create priority-specific HTML pages
foreach ($Priority in @("High", "Medium", "Low")) {
    $Items = $PriorityLevels[$Priority]
    $PriorityFile = Join-Path $NeedsFixingPath "$($Priority.ToLower())-priority.html"
    
    $HtmlContent = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM WIP - $Priority Priority Items</title>
    <style>
        body { font-family: 'Segoe UI', sans-serif; margin: 0; padding: 20px; background: #1a1a1a; color: #e0e0e0; }
        .header { background: linear-gradient(135deg, #4B45ED, #BA45ED); padding: 30px; border-radius: 12px; margin-bottom: 30px; }
        .header h1 { margin: 0; color: white; font-size: 2.5rem; }
        .header p { margin: 10px 0 0 0; color: rgba(255,255,255,0.9); font-size: 1.1rem; }
        .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }
        .stat-card { background: #2a2a2a; padding: 20px; border-radius: 8px; border-left: 4px solid #BA45ED; }
        .stat-card h3 { margin: 0 0 10px 0; color: #BA45ED; font-size: 1.1rem; }
        .stat-card .value { font-size: 2rem; font-weight: bold; color: white; }
        .items { display: grid; gap: 15px; }
        .item { background: #2a2a2a; padding: 20px; border-radius: 8px; border-left: 4px solid $(if ($Priority -eq "High") { "#ff4444" } elseif ($Priority -eq "Medium") { "#ffaa00" } else { "#44ff44" }); }
        .item-header { display: flex; justify-content: between; align-items: center; margin-bottom: 10px; }
        .item-id { font-family: monospace; background: #3a3a3a; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem; }
        .item-type { background: #4B45ED; color: white; padding: 4px 8px; border-radius: 4px; font-size: 0.8rem; }
        .item-description { font-size: 1.1rem; margin-bottom: 10px; line-height: 1.5; }
        .item-meta { color: #999; font-size: 0.9rem; display: grid; gap: 5px; }
        .navigation { margin-bottom: 20px; }
        .nav-link { display: inline-block; background: #3a3a3a; color: #e0e0e0; padding: 10px 15px; text-decoration: none; border-radius: 6px; margin-right: 10px; }
        .nav-link:hover { background: #4B45ED; color: white; }
    </style>
</head>
<body>
    <div class="header">
        <h1>$Priority Priority Items</h1>
        <p>Issues requiring attention in MTM WIP Application</p>
    </div>
    
    <div class="navigation">
        <a href="index.html" class="nav-link">🏠 Overview</a>
        <a href="high-priority.html" class="nav-link">🔥 High Priority</a>
        <a href="medium-priority.html" class="nav-link">⚡ Medium Priority</a>
        <a href="low-priority.html" class="nav-link">💡 Low Priority</a>
    </div>
    
    <div class="stats">
        <div class="stat-card">
            <h3>Total Items</h3>
            <div class="value">$($Items.Count)</div>
        </div>
        <div class="stat-card">
            <h3>Estimated Effort</h3>
            <div class="value">$(($Items | Where-Object { $_.EstimatedEffort -eq "High" }).Count + ($Items | Where-Object { $_.EstimatedEffort -eq "Medium" }).Count * 0.5 + ($Items | Where-Object { $_.EstimatedEffort -eq "Low" }).Count * 0.25) hours</div>
        </div>
    </div>
    
    <div class="items">
"@

    foreach ($Item in $Items) {
        $HtmlContent += @"
        <div class="item">
            <div class="item-header">
                <span class="item-id">ID: $($Item.Id)</span>
                <span class="item-type">$($Item.Type)</span>
            </div>
            <div class="item-description">$($Item.Description)</div>
            <div class="item-meta">
                <div>📁 Source: $($Item.SourceFile) (Line $($Item.LineNumber))</div>
                <div>📋 Section: $($Item.Section)</div>
                <div>🔧 Action: $($Item.ActionRequired)</div>
                <div>📅 Discovered: $($Item.DateDiscovered)</div>
            </div>
        </div>
"@
    }

    $HtmlContent += @"
    </div>
</body>
</html>
"@

    $HtmlContent | Out-File -FilePath $PriorityFile -Encoding UTF8
}

# Export detailed JSON report
$ReportPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/needs-fixing-report.json"
$NeedsFixingItems | ConvertTo-Json -Depth 3 | Out-File -FilePath $ReportPath -Encoding UTF8
Write-Host "`n📋 Detailed report saved to: $ReportPath" -ForegroundColor Cyan

Write-Host "`n=== Next Steps ===" -ForegroundColor Yellow
Write-Host "  📋 Review HTML tracking pages in docs/NeedsFixing/" -ForegroundColor Cyan
Write-Host "  🔧 Prioritize High Priority items for immediate attention" -ForegroundColor Cyan
Write-Host "  📅 Schedule Medium Priority items for next sprint" -ForegroundColor Cyan

Write-Host "`n✅ NeedsFixing Discovery Complete!" -ForegroundColor Green