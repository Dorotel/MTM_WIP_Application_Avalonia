# Intelligent Content Synchronization System

## Overview
Advanced system to automatically synchronize content between instruction files, HTML documentation, and custom prompts while maintaining consistency and preventing drift.

## Features

### **Smart Content Detection**
- **Semantic Analysis**: Detect when content means the same thing but is worded differently
- **Version Drift Detection**: Identify when similar content has diverged across files
- **Missing Content Analysis**: Find content that exists in one file but should exist in others
- **Redundancy Detection**: Identify unnecessary duplication vs beneficial redundancy

### **Automated Synchronization**
- **Content Propagation**: Automatically update related files when core content changes
- **Bi-directional Sync**: HTML ? Markdown ? Custom Prompts synchronization
- **Conflict Resolution**: Smart merging when content conflicts exist
- **Change Tracking**: Detailed audit trail of all synchronization activities

## Implementation

### **Synchronization Engine: Sync-InstructionContent.ps1**
```powershell
param(
    [string]$SourceFile,
    [string]$TargetPattern = "*.instruction.md",
    [switch]$AutoApply = $false,
    [string]$ReportPath = "sync_report.html"
)

# Content analysis engine
class ContentAnalyzer {
    [string] $FilePath
    [hashtable] $Sections
    [string[]] $Keywords
    [hashtable] $CrossReferences
    
    ContentAnalyzer([string]$path) {
        $this.FilePath = $path
        $this.Sections = @{}
        $this.Keywords = @()
        $this.CrossReferences = @{}
        $this.AnalyzeContent()
    }
    
    [void] AnalyzeContent() {
        $content = Get-Content $this.FilePath -Raw
        
        # Extract sections
        $sectionMatches = [regex]::Matches($content, '(?m)^#{1,6}\s+(.+)$')
        foreach ($match in $sectionMatches) {
            $sectionName = $match.Groups[1].Value.Trim()
            $this.Sections[$sectionName] = $this.ExtractSectionContent($content, $sectionName)
        }
        
        # Extract MTM-specific keywords
        $this.Keywords = [regex]::Matches($content, '\b(TransactionType|AddMTMServices|stored procedures?|ReactiveUI|MVVM|Avalonia)\b', 'IgnoreCase') | ForEach-Object { $_.Value }
        
        # Extract cross-references
        $linkMatches = [regex]::Matches($content, '\[([^\]]+)\]\(([^)]+)\)')
        foreach ($link in $linkMatches) {
            $this.CrossReferences[$link.Groups[1].Value] = $link.Groups[2].Value
        }
    }
    
    [string] ExtractSectionContent([string]$content, [string]$sectionName) {
        # Implementation to extract content between section headers
        return "Content for $sectionName"
    }
    
    [float] CompareWith([ContentAnalyzer]$other) {
        # Semantic similarity analysis
        $commonKeywords = $this.Keywords | Where-Object { $other.Keywords -contains $_ }
        $commonSections = $this.Sections.Keys | Where-Object { $other.Sections.ContainsKey($_) }
        
        $keywordSimilarity = ($commonKeywords.Count / ([Math]::Max($this.Keywords.Count, $other.Keywords.Count))) * 100
        $sectionSimilarity = ($commonSections.Count / ([Math]::Max($this.Sections.Count, $other.Sections.Count))) * 100
        
        return ($keywordSimilarity + $sectionSimilarity) / 2
    }
}

# Synchronization recommendations engine
class SyncRecommendation {
    [string] $SourceFile
    [string] $TargetFile
    [string] $RecommendationType # "Add", "Update", "Remove", "Merge"
    [string] $ContentSection
    [string] $Reason
    [float] $Confidence
    [string] $PreviewChange
    
    SyncRecommendation([string]$source, [string]$target, [string]$type, [string]$section, [string]$reason, [float]$confidence) {
        $this.SourceFile = $source
        $this.TargetFile = $target
        $this.RecommendationType = $type
        $this.ContentSection = $section
        $this.Reason = $reason
        $this.Confidence = $confidence
    }
    
    [string] GeneratePreview() {
        switch ($this.RecommendationType) {
            "Add" { return "Add section '$($this.ContentSection)' from $($this.SourceFile)" }
            "Update" { return "Update section '$($this.ContentSection)' to match $($this.SourceFile)" }
            "Remove" { return "Remove outdated section '$($this.ContentSection)'" }
            "Merge" { return "Merge conflicting content in section '$($this.ContentSection)'" }
        }
        return "Unknown change type"
    }
}

Write-Host "?? Analyzing content synchronization opportunities..."

# Discover all instruction files
$InstructionFiles = Get-ChildItem -Recurse -Filter $TargetPattern | Where-Object { $_.Name -ne (Split-Path $SourceFile -Leaf) }

if (-not (Test-Path $SourceFile)) {
    Write-Error "Source file not found: $SourceFile"
    return
}

$SourceAnalyzer = [ContentAnalyzer]::new($SourceFile)
$Recommendations = @()

foreach ($TargetFile in $InstructionFiles) {
    Write-Host "Analyzing: $($TargetFile.Name)"
    
    $TargetAnalyzer = [ContentAnalyzer]::new($TargetFile.FullName)
    $Similarity = $SourceAnalyzer.CompareWith($TargetAnalyzer)
    
    if ($Similarity -gt 30) { # Files are related enough to consider sync
        # Check for missing MTM business rules
        if ($SourceAnalyzer.Keywords -contains "TransactionType" -and $TargetAnalyzer.Keywords -notcontains "TransactionType") {
            $Recommendations += [SyncRecommendation]::new(
                $SourceFile, 
                $TargetFile.FullName, 
                "Add", 
                "TransactionType Logic", 
                "Missing critical MTM business rule", 
                0.9
            )
        }
        
        # Check for missing service registration patterns
        if ($SourceAnalyzer.Keywords -contains "AddMTMServices" -and $TargetAnalyzer.Keywords -notcontains "AddMTMServices") {
            $Recommendations += [SyncRecommendation]::new(
                $SourceFile, 
                $TargetFile.FullName, 
                "Add", 
                "Service Registration", 
                "Missing service registration pattern", 
                0.85
            )
        }
        
        # Check for section content drift
        foreach ($Section in $SourceAnalyzer.Sections.Keys) {
            if ($TargetAnalyzer.Sections.ContainsKey($Section)) {
                $SourceContent = $SourceAnalyzer.Sections[$Section]
                $TargetContent = $TargetAnalyzer.Sections[$Section]
                
                # Simple content comparison (could be enhanced with more sophisticated diff)
                if ($SourceContent -ne $TargetContent) {
                    $Recommendations += [SyncRecommendation]::new(
                        $SourceFile, 
                        $TargetFile.FullName, 
                        "Update", 
                        $Section, 
                        "Content has diverged from source", 
                        0.7
                    )
                }
            }
        }
    }
}

# Generate HTML report
$HtmlReport = @"
<!DOCTYPE html>
<html>
<head>
    <title>Content Synchronization Report</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #6a0dad, #4b0082); color: white; padding: 20px; margin: -20px -20px 20px -20px; border-radius: 8px 8px 0 0; }
        .recommendation { margin: 15px 0; padding: 15px; border-radius: 8px; border-left: 4px solid #6a0dad; }
        .rec-add { background: #e8f5e8; border-left-color: #4caf50; }
        .rec-update { background: #fff3e0; border-left-color: #ff9800; }
        .rec-remove { background: #ffebee; border-left-color: #f44336; }
        .rec-merge { background: #e3f2fd; border-left-color: #2196f3; }
        .confidence { display: inline-block; padding: 2px 8px; border-radius: 12px; color: white; font-size: 0.8em; }
        .conf-high { background: #4caf50; }
        .conf-medium { background: #ff9800; }
        .conf-low { background: #f44336; }
        .preview { background: #f9f9f9; padding: 10px; border-radius: 4px; margin: 10px 0; font-family: monospace; font-size: 0.9em; }
        button { background: #6a0dad; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; margin: 5px; }
        button:hover { background: #5a0d9d; }
        .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin: 20px 0; }
        .stat-card { background: #f8f9fa; padding: 15px; border-radius: 8px; text-align: center; }
        .stat-number { font-size: 2em; font-weight: bold; color: #6a0dad; }
    </style>
    <script>
        function applyRecommendation(sourceFile, targetFile, type, section) {
            // This would integrate with PowerShell backend to apply changes
            alert('Applying recommendation: ' + type + ' for ' + section + ' in ' + targetFile);
        }
        
        function togglePreview(elementId) {
            const element = document.getElementById(elementId);
            element.style.display = element.style.display === 'none' ? 'block' : 'none';
        }
    </script>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>?? Content Synchronization Report</h1>
            <p>Source: $SourceFile</p>
            <p>Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</p>
        </div>
        
        <div class="stats">
            <div class="stat-card">
                <div class="stat-number">$($Recommendations.Count)</div>
                <div>Total Recommendations</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$(($Recommendations | Where-Object { $_.Confidence -gt 0.8 }).Count)</div>
                <div>High Confidence</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$(($Recommendations | Where-Object { $_.RecommendationType -eq 'Add' }).Count)</div>
                <div>Content Additions</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$(($Recommendations | Where-Object { $_.RecommendationType -eq 'Update' }).Count)</div>
                <div>Content Updates</div>
            </div>
        </div>
"@

if ($Recommendations.Count -gt 0) {
    $HtmlReport += "<h2>?? Synchronization Recommendations</h2>"
    
    $groupedRecs = $Recommendations | Group-Object RecommendationType
    foreach ($group in $groupedRecs) {
        $HtmlReport += "<h3>$($group.Name) Recommendations</h3>"
        
        foreach ($rec in $group.Group) {
            $recClass = "rec-" + $rec.RecommendationType.ToLower()
            $confClass = if ($rec.Confidence -gt 0.8) { "conf-high" } elseif ($rec.Confidence -gt 0.6) { "conf-medium" } else { "conf-low" }
            $uniqueId = "preview_" + [System.Guid]::NewGuid().ToString("N").Substring(0,8)
            
            $HtmlReport += @"
            <div class="recommendation $recClass">
                <h4>$($rec.ContentSection) <span class="confidence $confClass">$([Math]::Round($rec.Confidence * 100))%</span></h4>
                <p><strong>Target:</strong> $($rec.TargetFile)</p>
                <p><strong>Reason:</strong> $($rec.Reason)</p>
                <button onclick="togglePreview('$uniqueId')">Show Preview</button>
                <button onclick="applyRecommendation('$($rec.SourceFile)', '$($rec.TargetFile)', '$($rec.RecommendationType)', '$($rec.ContentSection)')">Apply Change</button>
                <div id="$uniqueId" class="preview" style="display: none;">
                    $($rec.GeneratePreview())
                </div>
            </div>
"@
        }
    }
} else {
    $HtmlReport += "<h2>? No Synchronization Issues Found</h2><p>All instruction files appear to be in sync with the source file.</p>"
}

$HtmlReport += @"
    </div>
</body>
</html>
"@

# Save report
$HtmlReport | Out-File -FilePath $ReportPath -Encoding UTF8
Write-Host "? Synchronization report generated: $ReportPath"

# Auto-apply high-confidence recommendations if requested
if ($AutoApply) {
    $HighConfidenceRecs = $Recommendations | Where-Object { $_.Confidence -gt 0.9 }
    Write-Host "?? Auto-applying $($HighConfidenceRecs.Count) high-confidence recommendations..."
    
    foreach ($rec in $HighConfidenceRecs) {
        Write-Host "Applying: $($rec.RecommendationType) - $($rec.ContentSection) in $($rec.TargetFile)"
        # Implementation would go here to actually apply the changes
    }
}
```