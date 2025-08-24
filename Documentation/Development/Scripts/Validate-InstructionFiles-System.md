# Dynamic Instruction File Validation System

## Overview
This system automatically validates instruction files for completeness, accuracy, and cross-reference integrity.

## Validation Categories

### **Cross-Reference Validation**
- **Broken Links**: Automatically scan for broken references between instruction files
- **Orphaned Files**: Identify instruction files not referenced anywhere
- **Circular Dependencies**: Detect circular references that could cause confusion
- **Missing Backlinks**: Find instruction files that should reference each other but don't

### **Content Consistency Validation**
- **MTM Business Rules**: Verify all files consistently state TransactionType logic
- **Database Patterns**: Ensure all files consistently prohibit direct SQL
- **Service Registration**: Verify all files use AddMTMServices pattern
- **Naming Conventions**: Check for consistent naming pattern documentation

### **Completeness Validation**
- **Required Sections**: Verify all instruction files have required headers/sections
- **Code Examples**: Ensure code examples are syntactically valid
- **Version Alignment**: Check that .NET 8 and Avalonia versions are consistent
- **Prompt Integration**: Verify instruction files integrate with custom prompts

## Implementation

### **Validation Script: Validate-InstructionFiles.ps1**
```powershell
param(
    [string]$InstructionPath = ".github",
    [string]$OutputReport = "instruction_validation_report.html"
)

# Cross-reference validation
$AllInstructionFiles = Get-ChildItem -Path $InstructionPath -Recurse -Filter "*.instruction.md"
$CrossReferences = @{}
$BrokenLinks = @()
$OrphanedFiles = @()

foreach ($File in $AllInstructionFiles) {
    $Content = Get-Content $File.FullName -Raw
    
    # Extract all markdown links
    $Links = [regex]::Matches($Content, '\[([^\]]+)\]\(([^)]+)\)')
    
    foreach ($Link in $Links) {
        $LinkTarget = $Link.Groups[2].Value
        if ($LinkTarget -like "*.instruction.md") {
            # Validate link exists
            $FullLinkPath = Join-Path (Split-Path $File.FullName) $LinkTarget
            if (-not (Test-Path $FullLinkPath)) {
                $BrokenLinks += @{
                    File = $File.Name
                    Link = $LinkTarget
                    Context = $Link.Groups[1].Value
                }
            }
        }
    }
    
    # Check for MTM business rule consistency
    $HasTransactionTypeLogic = $Content -match "TransactionType.*user intent"
    $HasDirectSQLProhibition = $Content -match "stored procedures only|no direct SQL"
    $HasAddMTMServices = $Content -match "AddMTMServices"
    
    # Content quality scoring
    $QualityScore = 0
    if ($HasTransactionTypeLogic) { $QualityScore += 25 }
    if ($HasDirectSQLProhibition) { $QualityScore += 25 }
    if ($HasAddMTMServices) { $QualityScore += 25 }
    if ($Content.Length -gt 1000) { $QualityScore += 25 }
    
    $CrossReferences[$File.Name] = @{
        HasTransactionTypeLogic = $HasTransactionTypeLogic
        HasDirectSQLProhibition = $HasDirectSQLProhibition
        HasAddMTMServices = $HasAddMTMServices
        QualityScore = $QualityScore
        WordCount = ($Content -split '\s+').Count
        LastModified = $File.LastWriteTime
    }
}

# Generate HTML report
$HtmlReport = @"
<!DOCTYPE html>
<html>
<head>
    <title>MTM Instruction File Validation Report</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #6a0dad, #4b0082); color: white; padding: 20px; margin: -20px -20px 20px -20px; border-radius: 8px 8px 0 0; }
        .validation-section { margin: 20px 0; padding: 15px; border-left: 4px solid #6a0dad; background: #f9f9ff; }
        .broken-link { background: #ffebee; border-left-color: #f44336; padding: 10px; margin: 5px 0; border-radius: 4px; }
        .quality-score { display: inline-block; padding: 4px 8px; border-radius: 4px; color: white; font-weight: bold; }
        .score-high { background: #4caf50; }
        .score-medium { background: #ff9800; }
        .score-low { background: #f44336; }
        table { width: 100%; border-collapse: collapse; margin: 10px 0; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background: #6a0dad; color: white; }
        .mtm-compliant { background: #e8f5e8; }
        .mtm-missing { background: #ffebee; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>?? MTM Instruction File Validation Report</h1>
            <p>Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</p>
        </div>
        
        <div class="validation-section">
            <h2>?? Overall Statistics</h2>
            <table>
                <tr><th>Metric</th><th>Value</th></tr>
                <tr><td>Total Instruction Files</td><td>$($AllInstructionFiles.Count)</td></tr>
                <tr><td>Broken Links Found</td><td>$($BrokenLinks.Count)</td></tr>
                <tr><td>Files with TransactionType Logic</td><td>$(($CrossReferences.Values | Where-Object { $_.HasTransactionTypeLogic }).Count)</td></tr>
                <tr><td>Files with SQL Prohibition</td><td>$(($CrossReferences.Values | Where-Object { $_.HasDirectSQLProhibition }).Count)</td></tr>
                <tr><td>Files with AddMTMServices</td><td>$(($CrossReferences.Values | Where-Object { $_.HasAddMTMServices }).Count)</td></tr>
            </table>
        </div>
"@

if ($BrokenLinks.Count -gt 0) {
    $HtmlReport += @"
        <div class="validation-section">
            <h2>?? Broken Links Detected</h2>
"@
    foreach ($BrokenLink in $BrokenLinks) {
        $HtmlReport += @"
            <div class="broken-link">
                <strong>File:</strong> $($BrokenLink.File)<br>
                <strong>Broken Link:</strong> $($BrokenLink.Link)<br>
                <strong>Context:</strong> $($BrokenLink.Context)
            </div>
"@
    }
    $HtmlReport += "</div>"
}

$HtmlReport += @"
        <div class="validation-section">
            <h2>?? File Quality Scores</h2>
            <table>
                <tr><th>File</th><th>Quality Score</th><th>Word Count</th><th>Last Modified</th><th>MTM Compliance</th></tr>
"@

foreach ($File in $CrossReferences.Keys | Sort-Object) {
    $FileData = $CrossReferences[$File]
    $ScoreClass = if ($FileData.QualityScore -ge 75) { "score-high" } elseif ($FileData.QualityScore -ge 50) { "score-medium" } else { "score-low" }
    $ComplianceClass = if ($FileData.HasTransactionTypeLogic -and $FileData.HasDirectSQLProhibition) { "mtm-compliant" } else { "mtm-missing" }
    
    $HtmlReport += @"
                <tr class="$ComplianceClass">
                    <td>$File</td>
                    <td><span class="quality-score $ScoreClass">$($FileData.QualityScore)%</span></td>
                    <td>$($FileData.WordCount)</td>
                    <td>$($FileData.LastModified.ToString('yyyy-MM-dd'))</td>
                    <td>$(if ($FileData.HasTransactionTypeLogic -and $FileData.HasDirectSQLProhibition) { "? Compliant" } else { "?? Needs Review" })</td>
                </tr>
"@
}

$HtmlReport += @"
            </table>
        </div>
    </div>
</body>
</html>
"@

# Save report
$HtmlReport | Out-File -FilePath $OutputReport -Encoding UTF8
Write-Host "Validation report generated: $OutputReport"
```

### **Auto-Correction Suggestions**
```powershell
# Generate-InstructionFixes.ps1
# Automatically suggest fixes for common instruction file issues
```