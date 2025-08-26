# Validate and Implement GitHub Copilot Repository Instructions

param(
    [switch]$ValidateOnly = $false,
    [switch]$GenerateReport = $true,
    [string]$OutputReport = "github-copilot-instructions-implementation-report.html"
)

Write-Host "?? GitHub Copilot Repository Instructions Implementation" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

# Automatically discover all GitHub Copilot related files in .github directory
Write-Host "?? Discovering GitHub Copilot files in .github directory..." -ForegroundColor Yellow

$AllGitHubFiles = @()

# Get all .instruction.md files in .github directory
$InstructionFiles = Get-ChildItem -Path ".github" -Recurse -Filter "*.instruction.md"

# Get all CustomPrompt files in .github directory
$CustomPromptFiles = Get-ChildItem -Path ".github" -Recurse -Filter "CustomPrompt_*.md"

# Get copilot-instructions.md
$CopilotInstructionsFile = Get-ChildItem -Path ".github" -Filter "copilot-instructions.md"

# Get README.md files in .github directory
$ReadmeFiles = Get-ChildItem -Path ".github" -Recurse -Filter "README.md"

# Get other relevant files
$OtherRelevantFiles = Get-ChildItem -Path ".github" -Recurse -Filter "*.md" | Where-Object { 
    $_.Name -notlike "*.instruction.md" -and 
    $_.Name -notlike "CustomPrompt_*.md" -and 
    $_.Name -ne "README.md" -and
    $_.Name -ne "copilot-instructions.md"
}

# Add copilot-instructions.md first
if ($CopilotInstructionsFile) {
    $AllGitHubFiles += @{
        Path = $CopilotInstructionsFile.FullName.Replace((Get-Location).Path + "\", "")
        Type = "Main Instructions"
        Changes = "Main GitHub Copilot repository instructions"
        Status = "Primary"
        Category = "Main"
        FileType = "Instructions"
    }
}

# Add all instruction files
foreach ($file in $InstructionFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    # Determine category and type based on path
    $category = "General"
    $type = "Instruction File"
    
    if ($relativePath -like "*Core-Instructions*") {
        $category = "Core"
        $type = "Core Instructions"
    }
    elseif ($relativePath -like "*UI-Instructions*") {
        $category = "UI"
        $type = "UI Instructions"
    }
    elseif ($relativePath -like "*Development-Instructions*") {
        $category = "Development"
        $type = "Development Instructions"
    }
    elseif ($relativePath -like "*Quality-Instructions*") {
        $category = "Quality"
        $type = "Quality Instructions"
    }
    elseif ($relativePath -like "*Automation-Instructions*") {
        $category = "Automation"
        $type = "Automation Instructions"
    }
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = $type
        Changes = "Instruction file for $($file.BaseName.Replace('.instruction', '').Replace('-', ' '))"
        Status = "Secondary"
        Category = $category
        FileType = "Instructions"
    }
}

# Add all custom prompt files
foreach ($file in $CustomPromptFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = "Custom Prompt"
        Changes = "Custom prompt for $($file.BaseName.Replace('CustomPrompt_', '').Replace('_', ' '))"
        Status = "Secondary"
        Category = "Custom Prompts"
        FileType = "Prompts"
    }
}

# Add README files
foreach ($file in $ReadmeFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    # Determine category based on path
    $category = "Documentation"
    if ($relativePath -like "*Core-Instructions*") { $category = "Core Documentation" }
    elseif ($relativePath -like "*UI-Instructions*") { $category = "UI Documentation" }
    elseif ($relativePath -like "*Development-Instructions*") { $category = "Development Documentation" }
    elseif ($relativePath -like "*Quality-Instructions*") { $category = "Quality Documentation" }
    elseif ($relativePath -like "*Automation-Instructions*") { $category = "Automation Documentation" }
    elseif ($relativePath -like "*Custom-Prompts*") { $category = "Custom Prompts Documentation" }
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = "README Documentation"
        Changes = "Documentation for $($file.Directory.Name.Replace('-', ' '))"
        Status = "Secondary"
        Category = $category
        FileType = "Documentation"
    }
}

# Add other relevant files
foreach ($file in $OtherRelevantFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = "Supporting File"
        Changes = "Supporting file: $($file.BaseName.Replace('-', ' '))"
        Status = "Secondary"
        Category = "Supporting"
        FileType = "Supporting"
    }
}

Write-Host "?? Found $($AllGitHubFiles.Count) GitHub Copilot related files:" -ForegroundColor Green

# Group by file type for display
$FileTypeGroups = $AllGitHubFiles | Group-Object FileType
foreach ($group in $FileTypeGroups) {
    Write-Host "`n   ?? $($group.Name) ($($group.Count) files):" -ForegroundColor Cyan
    foreach ($file in $group.Group | Sort-Object Category, Path) {
        Write-Host "      - $($file.Path) [$($file.Category)]" -ForegroundColor Gray
    }
}

# Validation functions
function Test-FileExists {
    param([string]$FilePath)
    return Test-Path $FilePath
}

function Test-GitHubCopilotFormat {
    param([string]$FilePath, [string]$FileType)
    
    if (-not (Test-Path $FilePath)) {
        return @{ IsValid = $false; Issues = @("File does not exist") }
    }
    
    $content = Get-Content $FilePath -Raw
    $issues = @()
    
    # Different validation rules based on file type
    switch ($FileType) {
        "Instructions" {
            # Check for clear role definition
            if ($content -notmatch "(?i)(You are|Your role|You|act as)") {
                $issues += "Missing clear role definition (should start with 'You are' or 'Your role')"
            }
            
            # Check for actionable language
            if ($content -notmatch "(?i)(Always|Never|Use |Follow |Apply |Generate |Create |Implement |CRITICAL|Required|Prohibited)") {
                $issues += "Missing actionable language (should include Always/Never/Use/Follow commands)"
            }
            
            # Check for code examples
            if ($content -notmatch "```[\w]*\s") {
                $issues += "Missing code examples (should include code blocks)"
            }
        }
        "Prompts" {
            # Check for prompt structure
            if ($content -notmatch "(?i)(prompt|template|usage|example)") {
                $issues += "Missing prompt structure (should include prompt template or usage examples)"
            }
            
            # Check for persona assignment
            if ($content -notmatch "(?i)(persona|copilot|role)") {
                $issues += "Missing persona assignment"
            }
            
            # Check for usage guidelines
            if ($content -notmatch "(?i)(guidelines|instructions|when to use)") {
                $issues += "Missing usage guidelines"
            }
        }
        "Documentation" {
            # Check for structure and organization
            if ($content -notmatch "(?i)(files|purpose|usage|overview)") {
                $issues += "Missing documentation structure (should include purpose or overview)"
            }
        }
        default {
            # Basic checks for other files
            if ($content.Length -lt 100) {
                $issues += "File appears to be too short or empty"
            }
        }
    }
    
    # Check for MTM-specific patterns (applies to all file types)
    if ($content -notmatch "(MTM|Manitowoc|TransactionType|#6a0dad|#4B45ED|ReactiveUI|Avalonia)") {
        $issues += "Missing MTM-specific context"
    }
    
    return @{
        IsValid = $issues.Count -eq 0
        Issues = $issues
    }
}

function Test-RepositorySpecificContext {
    param([string]$FilePath)
    
    $content = Get-Content $FilePath -Raw
    $issues = @()
    
    # Check for repository-specific context
    if ($content -notmatch "MTM_Shared_Logic.*WIP.*Application") {
        $issues += "Missing repository-specific context (MTM WIP Application)"
    }
    
    # Check for technology stack mentions
    if ($content -notmatch "(\.NET 8|Avalonia|ReactiveUI)") {
        $issues += "Missing technology stack context"
    }
    
    # Check for constraints and preferences
    if ($content -notmatch "(CRITICAL|Never|Always|Required|Prohibited)") {
        $issues += "Missing explicit constraints and preferences"
    }
    
    return @{
        IsValid = $issues.Count -eq 0
        Issues = $issues
    }
}

# Validation process
Write-Host "`n?? Validating GitHub Copilot Repository Format..." -ForegroundColor Yellow

$ValidationResults = @()

foreach ($file in $AllGitHubFiles) {
    Write-Host "`n?? Validating: $($file.Path)" -ForegroundColor White
    
    $fileExists = Test-FileExists $file.Path
    $formatValid = Test-GitHubCopilotFormat $file.Path $file.FileType
    $contextValid = Test-RepositorySpecificContext $file.Path
    
    $result = @{
        FilePath = $file.Path
        Type = $file.Type
        Category = $file.Category
        FileType = $file.FileType
        Changes = $file.Changes
        Status = $file.Status
        FileExists = $fileExists
        FormatValid = $formatValid.IsValid
        FormatIssues = $formatValid.Issues
        ContextValid = $contextValid.IsValid
        ContextIssues = $contextValid.Issues
        OverallValid = $fileExists -and $formatValid.IsValid -and $contextValid.IsValid
    }
    
    $ValidationResults += $result
    
    if ($result.OverallValid) {
        Write-Host "   ? Valid GitHub Copilot format" -ForegroundColor Green
    } else {
        Write-Host "   ? Issues found:" -ForegroundColor Red
        if (-not $result.FileExists) {
            Write-Host "      - File does not exist" -ForegroundColor Red
        }
        $result.FormatIssues | ForEach-Object {
            Write-Host "      - $_" -ForegroundColor Red
        }
        $result.ContextIssues | ForEach-Object {
            Write-Host "      - $_" -ForegroundColor Red
        }
    }
}

# Generate summary
$TotalFiles = $ValidationResults.Count
$ValidFiles = ($ValidationResults | Where-Object { $_.OverallValid }).Count
$InvalidFiles = $TotalFiles - $ValidFiles

Write-Host "`n?? Validation Summary:" -ForegroundColor Cyan
Write-Host "   Total Files: $TotalFiles" -ForegroundColor White
Write-Host "   Valid: $ValidFiles" -ForegroundColor Green
Write-Host "   Invalid: $InvalidFiles" -ForegroundColor Red

# File type breakdown
Write-Host "`n?? File Type Breakdown:" -ForegroundColor Cyan
$FileTypeGroups = $ValidationResults | Group-Object FileType
foreach ($group in $FileTypeGroups) {
    $validInType = ($group.Group | Where-Object { $_.OverallValid }).Count
    $totalInType = $group.Group.Count
    $typeColor = if ($validInType -eq $totalInType) { "Green" } elseif ($validInType -gt 0) { "Yellow" } else { "Red" }
    Write-Host "   $($group.Name): $validInType/$totalInType valid" -ForegroundColor $typeColor
}

# Category breakdown
Write-Host "`n?? Category Breakdown:" -ForegroundColor Cyan
$CategoryGroups = $ValidationResults | Group-Object Category
foreach ($group in $CategoryGroups) {
    $validInCategory = ($group.Group | Where-Object { $_.OverallValid }).Count
    $totalInCategory = $group.Group.Count
    $categoryColor = if ($validInCategory -eq $totalInCategory) { "Green" } elseif ($validInCategory -gt 0) { "Yellow" } else { "Red" }
    Write-Host "   $($group.Name): $validInCategory/$totalInCategory valid" -ForegroundColor $categoryColor
}

# Best practices analysis
Write-Host "`n?? GitHub Copilot Best Practices Analysis:" -ForegroundColor Cyan

$BestPractices = @(
    @{
        Practice = "Clear Role Definition"
        Description = "Instructions start with 'You are' or 'Your role' to define Copilot's behavior"
        Score = ($ValidationResults | Where-Object { $_.FormatValid }).Count / $TotalFiles * 100
    },
    @{
        Practice = "Repository-Specific Context"
        Description = "Files are tailored to MTM WIP Application Avalonia project"
        Score = ($ValidationResults | Where-Object { $_.ContextValid }).Count / $TotalFiles * 100
    },
    @{
        Practice = "Actionable Language"
        Description = "Files use direct commands and clear guidance"
        Score = ($ValidationResults | Where-Object { $_.FormatValid }).Count / $TotalFiles * 100
    },
    @{
        Practice = "MTM Integration"
        Description = "Files include MTM-specific patterns and examples"
        Score = ($ValidationResults | Where-Object { $_.FormatValid }).Count / $TotalFiles * 100
    }
)

foreach ($practice in $BestPractices) {
    $scoreColor = if ($practice.Score -ge 80) { "Green" } elseif ($practice.Score -ge 60) { "Yellow" } else { "Red" }
    Write-Host "   $($practice.Practice): $([Math]::Round($practice.Score))%" -ForegroundColor $scoreColor
    Write-Host "      $($practice.Description)" -ForegroundColor Gray
}

# Generate HTML report if requested
if ($GenerateReport) {
    Write-Host "`n?? Generating Comprehensive Implementation Report..." -ForegroundColor Yellow
    
    $HtmlReport = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>GitHub Copilot Complete Repository Analysis</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #333;
            line-height: 1.6;
        }
        
        .container { 
            max-width: 1600px; 
            margin: 0 auto; 
            background: rgba(255, 255, 255, 0.95);
            min-height: 100vh;
            backdrop-filter: blur(10px);
        }
        
        .header { 
            background: linear-gradient(135deg, #6a0dad, #4b0082); 
            color: white; 
            padding: 30px; 
            text-align: center;
            box-shadow: 0 2px 20px rgba(0,0,0,0.2);
        }
        
        .header h1 { 
            font-size: 2.5em; 
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }
        
        .content { 
            padding: 30px; 
        }
        
        .summary-cards { 
            display: grid; 
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); 
            gap: 20px; 
            margin: 30px 0;
        }
        
        .summary-card { 
            background: white; 
            padding: 20px; 
            border-radius: 12px; 
            box-shadow: 0 8px 25px rgba(0,0,0,0.1); 
            border-left: 5px solid #6a0dad;
            text-align: center;
        }
        
        .summary-number { 
            font-size: 2.5em; 
            font-weight: bold; 
            color: #6a0dad; 
            margin-bottom: 10px;
        }
        
        .summary-label { 
            font-size: 1.1em; 
            color: #666;
        }
        
        .breakdown-section {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            margin: 30px 0;
        }
        
        .breakdown-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }
        
        .breakdown-item {
            display: flex;
            justify-content: space-between;
            padding: 8px 0;
            border-bottom: 1px solid #eee;
        }
        
        .breakdown-item:last-child {
            border-bottom: none;
        }
        
        .validation-table { 
            width: 100%; 
            border-collapse: collapse; 
            margin: 20px 0; 
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }
        
        .validation-table th, .validation-table td { 
            padding: 10px; 
            text-align: left; 
            border-bottom: 1px solid #eee;
            font-size: 0.85em;
        }
        
        .validation-table th { 
            background: #6a0dad; 
            color: white; 
            font-weight: 600;
        }
        
        .validation-table tr:hover { 
            background: #f8f9fa; 
        }
        
        .status-valid { 
            color: #28a745; 
            font-weight: bold;
        }
        
        .status-invalid { 
            color: #dc3545; 
            font-weight: bold;
        }
        
        .file-type-badge {
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 0.7em;
            color: white;
            font-weight: bold;
        }
        
        .type-instructions { background: #6a0dad; }
        .type-prompts { background: #28a745; }
        .type-documentation { background: #17a2b8; }
        .type-supporting { background: #ffc107; color: #333; }
        
        .category-badge {
            padding: 2px 6px;
            border-radius: 8px;
            font-size: 0.7em;
            color: white;
            font-weight: bold;
        }
        
        .cat-main { background: #6a0dad; }
        .cat-core { background: #28a745; }
        .cat-ui { background: #17a2b8; }
        .cat-development { background: #ffc107; color: #333; }
        .cat-quality { background: #dc3545; }
        .cat-automation { background: #6f42c1; }
        .cat-custom-prompts { background: #fd7e14; }
        .cat-documentation { background: #6c757d; }
        .cat-supporting { background: #868e96; }
        
        .best-practices { 
            background: white; 
            padding: 25px; 
            border-radius: 12px; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.1); 
            margin: 30px 0;
        }
        
        .practice-item { 
            margin: 15px 0; 
            padding: 15px; 
            background: #f8f9fa; 
            border-radius: 8px; 
            border-left: 4px solid #6a0dad;
        }
        
        .practice-score { 
            float: right; 
            padding: 5px 15px; 
            border-radius: 20px; 
            color: white; 
            font-weight: bold;
        }
        
        .score-high { background: #28a745; }
        .score-medium { background: #ffc107; color: #333; }
        .score-low { background: #dc3545; }
        
        .success-banner {
            background: linear-gradient(135deg, #28a745, #20c997);
            color: white;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
            text-align: center;
            font-size: 1.2em;
            font-weight: bold;
        }
        
        .discovery-section {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            margin: 30px 0;
        }
        
        .footer { 
            background: #f8f9fa; 
            padding: 20px; 
            text-align: center; 
            color: #666;
            border-top: 1px solid #eee;
        }
        
        .recommendations {
            background: linear-gradient(135deg, #e3f2fd, #bbdefb);
            padding: 25px;
            border-radius: 12px;
            margin: 30px 0;
        }
        
        .recommendation-item {
            margin: 15px 0;
            padding: 15px 0;
            border-bottom: 1px solid rgba(0,0,0,0.1);
        }
        
        .recommendation-item:last-child {
            border-bottom: none;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>?? GitHub Copilot Complete Repository Analysis</h1>
            <p>Comprehensive Analysis of MTM WIP Application Avalonia GitHub Copilot Files</p>
            <p>Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</p>
        </div>
        
        <div class="content">
"@

    if ($ValidFiles -eq $TotalFiles) {
        $HtmlReport += @"
            <div class="success-banner">
                ? All $TotalFiles GitHub Copilot files successfully implement repository format standards!
            </div>
"@
    }

    $HtmlReport += @"
            <div class="discovery-section">
                <h2>?? Complete Discovery Results</h2>
                <p>Automatically discovered <strong>$TotalFiles files</strong> across the entire .github directory structure:</p>
                <div class="breakdown-section">
                    <div class="breakdown-card">
                        <h3>?? By File Type</h3>
"@

    foreach ($group in $FileTypeGroups) {
        $validInType = ($group.Group | Where-Object { $_.OverallValid }).Count
        $totalInType = $group.Group.Count
        $HtmlReport += @"
                        <div class="breakdown-item">
                            <span>$($group.Name)</span>
                            <span>$validInType / $totalInType</span>
                        </div>
"@
    }

    $HtmlReport += @"
                    </div>
                    <div class="breakdown-card">
                        <h3>?? By Category</h3>
"@

    foreach ($group in $CategoryGroups) {
        $validInCategory = ($group.Group | Where-Object { $_.OverallValid }).Count
        $totalInCategory = $group.Group.Count
        $HtmlReport += @"
                        <div class="breakdown-item">
                            <span>$($group.Name)</span>
                            <span>$validInCategory / $totalInCategory</span>
                        </div>
"@
    }

    $HtmlReport += @"
                    </div>
                </div>
            </div>
            
            <div class="summary-cards">
                <div class="summary-card">
                    <div class="summary-number">$TotalFiles</div>
                    <div class="summary-label">Total Files</div>
                </div>
                <div class="summary-card">
                    <div class="summary-number">$ValidFiles</div>
                    <div class="summary-label">Valid Format</div>
                </div>
                <div class="summary-card">
                    <div class="summary-number">$InvalidFiles</div>
                    <div class="summary-label">Need Updates</div>
                </div>
                <div class="summary-card">
                    <div class="summary-number">$([Math]::Round($ValidFiles / $TotalFiles * 100))%</div>
                    <div class="summary-label">Compliance Rate</div>
                </div>
                <div class="summary-card">
                    <div class="summary-number">$(($ValidationResults | Where-Object { $_.FileType -eq 'Instructions' }).Count)</div>
                    <div class="summary-label">Instruction Files</div>
                </div>
                <div class="summary-card">
                    <div class="summary-number">$(($ValidationResults | Where-Object { $_.FileType -eq 'Prompts' }).Count)</div>
                    <div class="summary-label">Custom Prompts</div>
                </div>
            </div>
            
            <h2>?? Complete File Analysis</h2>
            <table class="validation-table">
                <thead>
                    <tr>
                        <th>File Path</th>
                        <th>Type</th>
                        <th>Category</th>
                        <th>Status</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody>
"@

foreach ($result in $ValidationResults | Sort-Object FileType, Category, FilePath) {
    $statusClass = if ($result.OverallValid) { "status-valid" } else { "status-invalid" }
    $statusText = if ($result.OverallValid) { "? Valid" } else { "? Needs Update" }
    $fileTypeClass = "type-" + $result.FileType.ToLower()
    $categoryClass = "cat-" + $result.Category.ToLower().Replace(' ', '-').Replace('custom prompts', 'custom-prompts')
    
    $HtmlReport += @"
                    <tr>
                        <td><code>$($result.FilePath)</code></td>
                        <td><span class="file-type-badge $fileTypeClass">$($result.FileType)</span></td>
                        <td><span class="category-badge $categoryClass">$($result.Category)</span></td>
                        <td><span class="$statusClass">$statusText</span></td>
                        <td>$($result.Changes)</td>
                    </tr>
"@
}

$HtmlReport += @"
                </tbody>
            </table>
            
            <div class="best-practices">
                <h2>?? GitHub Copilot Best Practices Analysis</h2>
"@

foreach ($practice in $BestPractices) {
    $scoreClass = if ($practice.Score -ge 80) { "score-high" } elseif ($practice.Score -ge 60) { "score-medium" } else { "score-low" }
    
    $HtmlReport += @"
                <div class="practice-item">
                    <span class="practice-score $scoreClass">$([Math]::Round($practice.Score))%</span>
                    <h3>$($practice.Practice)</h3>
                    <p>$($practice.Description)</p>
                </div>
"@
}

$HtmlReport += @"
            </div>
            
            <div class="recommendations">
                <h2>?? Comprehensive Recommendations</h2>
                <div class="recommendation-item">
                    <h3>1. Test Complete GitHub Copilot Integration</h3>
                    <p>With $TotalFiles files analyzed across $($FileTypeGroups.Count) different types, test GitHub Copilot's behavior comprehensively. The system now includes instructions, custom prompts, and documentation.</p>
                </div>
                <div class="recommendation-item">
                    <h3>2. Leverage Custom Prompt Library</h3>
                    <p>Your $(($ValidationResults | Where-Object { $_.FileType -eq 'Prompts' }).Count) custom prompts provide specialized workflows. Test these prompts to ensure they work effectively with the main instructions.</p>
                </div>
                <div class="recommendation-item">
                    <h3>3. Validate Cross-File Integration</h3>
                    <p>Ensure that instructions, prompts, and documentation work together seamlessly for complex development tasks spanning multiple categories.</p>
                </div>
                <div class="recommendation-item">
                    <h3>4. Monitor Category-Specific Performance</h3>
                    <p>Track how well GitHub Copilot performs in each category: Core, UI, Development, Quality, Automation, and Custom Prompts.</p>
                </div>
                <div class="recommendation-item">
                    <h3>5. Establish Maintenance Process</h3>
                    <p>With $TotalFiles files to maintain, establish a process for keeping all GitHub Copilot files current as the project evolves.</p>
                </div>
            </div>
        </div>
        
        <div class="footer">
            <p>Generated by GitHub Copilot Complete Repository Analysis Script</p>
            <p>MTM WIP Application Avalonia Project - $(Get-Date -Format 'yyyy') | $TotalFiles Files Analyzed</p>
        </div>
    </div>
</body>
</html>
"@

    # Save the report
    $HtmlReport | Out-File -FilePath $OutputReport -Encoding UTF8
    Write-Host "? Comprehensive report generated: $OutputReport" -ForegroundColor Green
}

# Summary and next steps
Write-Host "`n?? Complete Analysis Finished!" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Cyan

if ($ValidFiles -eq $TotalFiles) {
    Write-Host "? All $TotalFiles GitHub Copilot files are properly formatted!" -ForegroundColor Green
} else {
    Write-Host "??  $InvalidFiles out of $TotalFiles files may need updates. See the detailed report for specifics." -ForegroundColor Yellow
}

Write-Host "`n?? Complete Discovery Summary:" -ForegroundColor Cyan
Write-Host "   • Total Files Analyzed: $TotalFiles" -ForegroundColor White
Write-Host "   • File Types: $($FileTypeGroups.Count) ($(($FileTypeGroups.Name) -join ', '))" -ForegroundColor White
Write-Host "   • Categories: $($CategoryGroups.Count) different categories" -ForegroundColor White
Write-Host "   • Comprehensive GitHub Copilot ecosystem discovered" -ForegroundColor White

Write-Host "`n?? Recommended Actions:" -ForegroundColor Cyan
Write-Host "   1. Test GitHub Copilot with the complete file ecosystem" -ForegroundColor White
Write-Host "   2. Validate custom prompt integration with main instructions" -ForegroundColor White
Write-Host "   3. Test cross-category workflow scenarios" -ForegroundColor White
Write-Host "   4. Monitor performance across all $($CategoryGroups.Count) categories" -ForegroundColor White
Write-Host "   5. Establish maintenance process for $TotalFiles files" -ForegroundColor White

if ($GenerateReport) {
    Write-Host "`n?? Comprehensive analysis report: $OutputReport" -ForegroundColor Cyan
}

Write-Host "`nComplete ecosystem analysis of $TotalFiles GitHub Copilot files finished! ??" -ForegroundColor Green