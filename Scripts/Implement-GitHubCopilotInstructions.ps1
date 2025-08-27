# Validate and Implement GitHub Copilot Repository Instructions

param(
    [switch]$ValidateOnly = $false,
    [switch]$GenerateReport = $true,
    [string]$OutputReport = "github-copilot-instructions-implementation-report.html"
)

Write-Host "GitHub Copilot Repository Instructions Implementation" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

# Automatically discover all GitHub Copilot related files in .github directory
Write-Host "Discovering GitHub Copilot files in .github directory..." -ForegroundColor Yellow

$AllGitHubFiles = @()

# Get all .instruction.md files in .github directory
$InstructionFiles = Get-ChildItem -Path ".github" -Recurse -Filter "*.instruction.md" -ErrorAction SilentlyContinue

# Get all CustomPrompt files in both Custom-Prompt and Custom-Prompts directories
$CustomPromptFiles = @()
if (Test-Path ".github/Custom-Prompt") {
    $CustomPromptFiles += Get-ChildItem -Path ".github/Custom-Prompt" -Recurse -Filter "CustomPrompt_*.md" -ErrorAction SilentlyContinue
}
if (Test-Path ".github/Custom-Prompts") {
    $CustomPromptFiles += Get-ChildItem -Path ".github/Custom-Prompts" -Recurse -Filter "CustomPrompt_*.md" -ErrorAction SilentlyContinue
}

# Get copilot-instructions.md
$CopilotInstructionsFile = Get-ChildItem -Path ".github" -Filter "copilot-instructions.md" -ErrorAction SilentlyContinue

# Get README.md files in .github directory
$ReadmeFiles = Get-ChildItem -Path ".github" -Recurse -Filter "README.md" -ErrorAction SilentlyContinue

# Get other relevant .md files in .github directory
$OtherRelevantFiles = Get-ChildItem -Path ".github" -Recurse -Filter "*.md" -ErrorAction SilentlyContinue | Where-Object { 
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

# Add all instruction files with improved categorization
foreach ($file in $InstructionFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    # Determine category and type based on actual folder structure
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

# Add all custom prompt files with improved categorization
foreach ($file in $CustomPromptFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    # Determine subcategory based on file name
    $subcategory = "General"
    $fileName = $file.BaseName
    
    if ($fileName -like "*UI*" -or $fileName -like "*Layout*" -or $fileName -like "*Element*") {
        $subcategory = "UI Development"
    }
    elseif ($fileName -like "*Database*" -or $fileName -like "*StoredProcedure*" -or $fileName -like "*CRUD*") {
        $subcategory = "Database"
    }
    elseif ($fileName -like "*ReactiveUI*" -or $fileName -like "*ViewModel*") {
        $subcategory = "ReactiveUI"
    }
    elseif ($fileName -like "*Error*" -or $fileName -like "*Result*") {
        $subcategory = "Error Handling"
    }
    elseif ($fileName -like "*Hotkey*" -or $fileName -like "*Context*") {
        $subcategory = "System Integration"
    }
    elseif ($fileName -like "*Issue*" -or $fileName -like "*Verify*" -or $fileName -like "*Document*") {
        $subcategory = "Development Tools"
    }
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = "Custom Prompt"
        Changes = "Custom prompt for $($file.BaseName.Replace('CustomPrompt_', '').Replace('_', ' '))"
        Status = "Secondary"
        Category = "Custom Prompts - $subcategory"
        FileType = "Prompts"
    }
}

# Add README files with improved categorization
foreach ($file in $ReadmeFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    # Determine category based on path
    $category = "Documentation"
    if ($relativePath -like "*Core-Instructions*") { $category = "Core Documentation" }
    elseif ($relativePath -like "*UI-Instructions*") { $category = "UI Documentation" }
    elseif ($relativePath -like "*Development-Instructions*") { $category = "Development Documentation" }
    elseif ($relativePath -like "*Quality-Instructions*") { $category = "Quality Documentation" }
    elseif ($relativePath -like "*Automation-Instructions*") { $category = "Automation Documentation" }
    elseif ($relativePath -like "*Custom-Prompt*") { $category = "Custom Prompts Documentation" }
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = "README Documentation"
        Changes = "Documentation for $($file.Directory.Name.Replace('-', ' '))"
        Status = "Secondary"
        Category = $category
        FileType = "Documentation"
    }
}

# Add other relevant files with improved categorization
foreach ($file in $OtherRelevantFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "")
    
    # Determine specific categories for known files
    $category = "Supporting"
    $type = "Supporting File"
    
    if ($file.Name -eq "all-copilot-files-list.instructions.md") {
        $category = "File Management"
        $type = "File Inventory"
    }
    elseif ($file.Name -eq "document-reorganization.instructions.md") {
        $category = "Organization"
        $type = "Documentation Organization"
    }
    elseif ($file.Name -eq "missing-components.instruction.md") {
        $category = "Project Status"
        $type = "Component Tracking"
    }
    elseif ($file.Name -like "*hotkey*" -or $file.Name -like "*reference*") {
        $category = "Reference"
        $type = "Reference Guide"
    }
    elseif ($file.Name -like "*examples*") {
        $category = "Examples"
        $type = "Example Documentation"
    }
    
    $AllGitHubFiles += @{
        Path = $relativePath
        Type = $type
        Changes = "Supporting file: $($file.BaseName.Replace('-', ' '))"
        Status = "Secondary"
        Category = $category
        FileType = "Supporting"
    }
}

Write-Host "Found $($AllGitHubFiles.Count) GitHub Copilot related files:" -ForegroundColor Green

# Group by file type for display
$FileTypeGroups = $AllGitHubFiles | Group-Object FileType
foreach ($group in $FileTypeGroups) {
    Write-Host "`n   $($group.Name) ($($group.Count) files):" -ForegroundColor Cyan
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
    
    try {
        $content = Get-Content $FilePath -Raw -ErrorAction Stop
    }
    catch {
        return @{ IsValid = $false; Issues = @("Unable to read file: $($_.Exception.Message)") }
    }
    
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
            
            # Check for collapsible sections (HTML details/summary tags)
            if ($content -notmatch "<details>" -or $content -notmatch "<summary>") {
                $issues += "Missing collapsible sections (should use details/summary HTML tags)"
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
    
    try {
        $content = Get-Content $FilePath -Raw -ErrorAction Stop
    }
    catch {
        return @{ IsValid = $false; Issues = @("Unable to read file: $($_.Exception.Message)") }
    }
    
    $issues = @()
    
    # Check for repository-specific context
    if ($content -notmatch "MTM.*WIP.*Application") {
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

# Function to generate detailed explanations for issues
function Get-IssueExplanation {
    param([string]$Issue)
    
    switch ($Issue) {
        "Missing clear role definition (should start with 'You are' or 'Your role')" {
            return "GitHub Copilot needs clear role definitions to understand its behavior context. Files should start with phrases like 'You are an expert...' or 'Your role is to...' to establish the AI's persona and responsibilities."
        }
        "Missing actionable language (should include Always/Never/Use/Follow commands)" {
            return "Effective GitHub Copilot instructions require direct, actionable commands. Include words like 'Always', 'Never', 'Use', 'Follow', 'Apply', 'Generate', 'Create', 'Implement', 'CRITICAL', 'Required', or 'Prohibited' to provide clear guidance."
        }
        "Missing code examples (should include code blocks)" {
            return "Code examples in markdown blocks (```) help GitHub Copilot understand the expected coding patterns and syntax. Examples demonstrate the desired output format and coding standards."
        }
        "Missing collapsible sections (should use details/summary HTML tags)" {
            return "Modern documentation uses HTML <details> and <summary> tags to create collapsible sections for better organization and readability. This improves navigation and reduces visual clutter."
        }
        "Missing prompt structure (should include prompt template or usage examples)" {
            return "Custom prompts need clear structure with templates, usage examples, or instructions on how to use the prompt effectively. This helps users understand when and how to apply the prompt."
        }
        "Missing persona assignment" {
            return "Prompts should assign a specific persona or role to GitHub Copilot (e.g., 'act as a senior developer', 'you are a UI expert') to provide context and improve response quality."
        }
        "Missing usage guidelines" {
            return "Clear usage guidelines help users understand when to use the prompt, what inputs are expected, and what outputs will be generated. Include sections like 'When to use', 'Guidelines', or 'Instructions'."
        }
        "Missing documentation structure (should include purpose or overview)" {
            return "Documentation files should include structural elements like purpose statements, overviews, file listings, or usage explanations to help users understand the content and organization."
        }
        "File appears to be too short or empty" {
            return "The file content is insufficient (less than 100 characters). GitHub Copilot files should contain substantial guidance, examples, or documentation to be useful."
        }
        "Missing MTM-specific context" {
            return "Files should include MTM (Manitowoc Tool and Manufacturing) specific context such as company name, project details, color schemes (#6a0dad, #4B45ED), or technology stack (ReactiveUI, Avalonia) to maintain project consistency."
        }
        "Missing repository-specific context (MTM WIP Application)" {
            return "Files should reference the specific repository context 'MTM WIP Application' to ensure GitHub Copilot understands it's working with this particular inventory management system project."
        }
        "Missing technology stack context" {
            return "Files should mention the core technology stack (.NET 8, Avalonia, ReactiveUI) to help GitHub Copilot generate appropriate code that aligns with the project's technical architecture."
        }
        "Missing explicit constraints and preferences" {
            return "Files should include explicit constraints and preferences using strong language like 'CRITICAL', 'Never', 'Always', 'Required', or 'Prohibited' to ensure GitHub Copilot follows important project rules and standards."
        }
        default {
            return "This issue requires attention to improve GitHub Copilot's effectiveness and ensure compliance with project standards."
        }
    }
}

# Validation process
Write-Host "`nValidating GitHub Copilot Repository Format..." -ForegroundColor Yellow

$ValidationResults = @()

foreach ($file in $AllGitHubFiles) {
    Write-Host "`nValidating: $($file.Path)" -ForegroundColor White
    
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
        AllIssues = $formatValid.Issues + $contextValid.Issues
    }
    
    $ValidationResults += $result
    
    if ($result.OverallValid) {
        Write-Host "   Valid GitHub Copilot format" -ForegroundColor Green
    } else {
        Write-Host "   Issues found:" -ForegroundColor Red
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

Write-Host "`nValidation Summary:" -ForegroundColor Cyan
Write-Host "   Total Files: $TotalFiles" -ForegroundColor White
Write-Host "   Valid: $ValidFiles" -ForegroundColor Green
Write-Host "   Invalid: $InvalidFiles" -ForegroundColor Red

# File type breakdown
Write-Host "`nFile Type Breakdown:" -ForegroundColor Cyan
$FileTypeGroups = $ValidationResults | Group-Object FileType
foreach ($group in $FileTypeGroups) {
    $validInType = ($group.Group | Where-Object { $_.OverallValid }).Count
    $totalInType = $group.Group.Count
    $typeColor = if ($validInType -eq $totalInType) { "Green" } elseif ($validInType -gt 0) { "Yellow" } else { "Red" }
    Write-Host "   $($group.Name): $validInType/$totalInType valid" -ForegroundColor $typeColor
}

# Category breakdown
Write-Host "`nCategory Breakdown:" -ForegroundColor Cyan
$CategoryGroups = $ValidationResults | Group-Object Category
foreach ($group in $CategoryGroups) {
    $validInCategory = ($group.Group | Where-Object { $_.OverallValid }).Count
    $totalInCategory = $group.Group.Count
    $categoryColor = if ($validInCategory -eq $totalInCategory) { "Green" } elseif ($validInCategory -gt 0) { "Yellow" } else { "Red" }
    Write-Host "   $($group.Name): $validInCategory/$totalInCategory valid" -ForegroundColor $categoryColor
}

# Directory structure analysis
Write-Host "`nDirectory Structure Analysis:" -ForegroundColor Cyan
$DirectoryStats = @{}
foreach ($file in $AllGitHubFiles) {
    $dir = Split-Path $file.Path -Parent
    if (-not $DirectoryStats.ContainsKey($dir)) {
        $DirectoryStats[$dir] = @{ Total = 0; Valid = 0 }
    }
    $DirectoryStats[$dir].Total++
    if (($ValidationResults | Where-Object { $_.FilePath -eq $file.Path }).OverallValid) {
        $DirectoryStats[$dir].Valid++
    }
}

foreach ($dir in $DirectoryStats.Keys | Sort-Object) {
    $stats = $DirectoryStats[$dir]
    $dirColor = if ($stats.Valid -eq $stats.Total) { "Green" } elseif ($stats.Valid -gt 0) { "Yellow" } else { "Red" }
    Write-Host "   $dir : $($stats.Valid)/$($stats.Total) valid" -ForegroundColor $dirColor
}

# Generate HTML report if requested
if ($GenerateReport) {
    Write-Host "`nGenerating Comprehensive Implementation Report with Issue Explanations..." -ForegroundColor Yellow
    
    $reportDate = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    $complianceRate = [Math]::Round($ValidFiles / $TotalFiles * 100)
    $currentYear = Get-Date -Format 'yyyy'
    
    $HtmlContent = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>GitHub Copilot Complete Repository Analysis</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: #333; line-height: 1.6; }
        .container { max-width: 1600px; margin: 0 auto; background: rgba(255, 255, 255, 0.95); min-height: 100vh; backdrop-filter: blur(10px); }
        .header { background: linear-gradient(135deg, #6a0dad, #4b0082); color: white; padding: 30px; text-align: center; box-shadow: 0 2px 20px rgba(0,0,0,0.2); }
        .header h1 { font-size: 2.5em; margin-bottom: 10px; text-shadow: 2px 2px 4px rgba(0,0,0,0.3); }
        .content { padding: 30px; }
        .summary-cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 20px; margin: 30px 0; }
        .summary-card { background: white; padding: 20px; border-radius: 12px; box-shadow: 0 8px 25px rgba(0,0,0,0.1); border-left: 5px solid #6a0dad; text-align: center; }
        .summary-number { font-size: 2.5em; font-weight: bold; color: #6a0dad; margin-bottom: 10px; }
        .summary-label { font-size: 1.1em; color: #666; }
        .validation-table { width: 100%; border-collapse: collapse; margin: 20px 0; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
        .validation-table th, .validation-table td { padding: 12px; text-align: left; border-bottom: 1px solid #eee; font-size: 0.9em; vertical-align: top; }
        .validation-table th { background: #6a0dad; color: white; font-weight: 600; }
        .validation-table tr:hover { background: #f8f9fa; }
        .validation-table tr.expandable { cursor: pointer; }
        .validation-table tr.expandable:hover { background: #e3f2fd; }
        .status-valid { color: #28a745; font-weight: bold; }
        .status-invalid { color: #dc3545; font-weight: bold; }
        .file-type-badge { padding: 4px 8px; border-radius: 12px; font-size: 0.75em; color: white; font-weight: bold; display: inline-block; }
        .type-instructions { background: #6a0dad; }
        .type-prompts { background: #28a745; }
        .type-documentation { background: #17a2b8; }
        .type-supporting { background: #ffc107; color: #333; }
        .footer { background: #f8f9fa; padding: 20px; text-align: center; color: #666; border-top: 1px solid #eee; }
        .issue-details { background: #fff3cd; padding: 20px; border-left: 4px solid #ffc107; margin: 10px 0; border-radius: 8px; }
        .issue-item { margin: 15px 0; padding: 15px; background: white; border-radius: 8px; border-left: 4px solid #dc3545; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
        .issue-title { font-weight: bold; color: #dc3545; margin-bottom: 8px; font-size: 0.95em; }
        .issue-explanation { color: #555; font-size: 0.9em; line-height: 1.5; }
        .expand-button { background: #6a0dad; color: white; border: none; cursor: pointer; font-size: 0.8em; padding: 6px 12px; border-radius: 4px; margin-left: 8px; }
        .expand-button:hover { background: #5a0ca0; }
        .issues-count { font-size: 0.8em; color: #666; margin-left: 8px; font-style: italic; }
        .details-row { display: none; }
        .details-row.show { display: table-row; }
        .details-cell { background: #f8f9fa; padding: 0; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>GitHub Copilot Complete Repository Analysis</h1>
            <p>Comprehensive Analysis of MTM WIP Application Avalonia GitHub Copilot Files</p>
            <p>Generated: $reportDate</p>
        </div>
        <div class="content">
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
                    <div class="summary-number">$complianceRate%</div>
                    <div class="summary-label">Compliance Rate</div>
                </div>
            </div>
            <h2>Complete File Analysis with Detailed Issue Explanations</h2>
            <p style="margin-bottom: 20px; color: #666;">Click the "Show Issues" button for files that need updates to see detailed explanations and remediation guidance.</p>
            <table class="validation-table">
                <thead>
                    <tr>
                        <th style="width: 40%;">File Path</th>
                        <th style="width: 12%;">Type</th>
                        <th style="width: 18%;">Category</th>
                        <th style="width: 15%;">Status</th>
                        <th style="width: 15%;">Description</th>
                    </tr>
                </thead>
                <tbody>
"@

    $rowCounter = 0
    foreach ($result in $ValidationResults | Sort-Object FileType, Category, FilePath) {
        $statusClass = if ($result.OverallValid) { "status-valid" } else { "status-invalid" }
        $statusText = if ($result.OverallValid) { "✅ Valid" } else { "❌ Needs Update" }
        $fileTypeClass = "type-" + $result.FileType.ToLower()
        $issuesCount = $result.AllIssues.Count
        
        $HtmlContent += @"
                    <tr class="main-row">
                        <td><code style="font-size: 0.85em;">$($result.FilePath)</code></td>
                        <td><span class="file-type-badge $fileTypeClass">$($result.FileType)</span></td>
                        <td>$($result.Category)</td>
                        <td>
                            <span class="$statusClass">$statusText</span>
"@

        if ($issuesCount -gt 0) {
            $HtmlContent += @"
                            <button class="expand-button" onclick="toggleIssues($rowCounter)">📋 Show Issues</button>
                            <span class="issues-count">($issuesCount issues)</span>
"@
        }

        $HtmlContent += @"
                        </td>
                        <td>$($result.Changes)</td>
                    </tr>
"@

        # Add detailed issues row if there are issues
        if ($issuesCount -gt 0) {
            $HtmlContent += @"
                    <tr id="details-$rowCounter" class="details-row">
                        <td colspan="5" class="details-cell">
                            <div class="issue-details">
                                <h4 style="margin-bottom: 15px; color: #dc3545; font-size: 1.1em;">🔍 Issues Requiring Attention:</h4>
"@

            foreach ($issue in $result.AllIssues) {
                $explanation = Get-IssueExplanation $issue
                $HtmlContent += @"
                                <div class="issue-item">
                                    <div class="issue-title">⚠️ $issue</div>
                                    <div class="issue-explanation">$explanation</div>
                                </div>
"@
            }

            $HtmlContent += @"
                                <div style="margin-top: 15px; padding: 10px; background: #e8f4fd; border-radius: 6px; border-left: 4px solid #007bff;">
                                    <strong>💡 Next Steps:</strong> Review each issue above and apply the suggested remediation to improve GitHub Copilot effectiveness for this file.
                                </div>
                            </div>
                        </td>
                    </tr>
"@
        }

        $rowCounter++
    }

    $HtmlContent += @"
                </tbody>
            </table>
        </div>
        <div class="footer">
            <p>Generated by GitHub Copilot Complete Repository Analysis Script v2.1</p>
            <p>MTM WIP Application Avalonia Project - $currentYear - $TotalFiles Files Analyzed across $($DirectoryStats.Count) Directories</p>
            <p style="margin-top: 10px; font-style: italic;">Click "Show Issues" buttons to expand detailed explanations and remediation guidance.</p>
        </div>
    </div>
    
    <script>
        function toggleIssues(rowId) {
            const detailsRow = document.getElementById('details-' + rowId);
            const buttons = document.querySelectorAll('button[onclick="toggleIssues(' + rowId + ')"]');
            
            if (detailsRow) {
                if (detailsRow.classList.contains('show')) {
                    detailsRow.classList.remove('show');
                    detailsRow.style.display = 'none';
                    buttons.forEach(btn => btn.textContent = '📋 Show Issues');
                } else {
                    detailsRow.classList.add('show');
                    detailsRow.style.display = 'table-row';
                    buttons.forEach(btn => btn.textContent = '📖 Hide Issues');
                }
            }
        }
        
        // Initialize all details rows as hidden
        document.addEventListener('DOMContentLoaded', function() {
            const detailsRows = document.querySelectorAll('.details-row');
            detailsRows.forEach(row => {
                row.style.display = 'none';
            });
        });
    </script>
</body>
</html>
"@

    # Save the report
    $HtmlContent | Out-File -FilePath $OutputReport -Encoding UTF8
    Write-Host "Comprehensive report with detailed issue explanations generated: $OutputReport" -ForegroundColor Green
}

# Summary and next steps
Write-Host "`nComplete Analysis Finished!" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Cyan

if ($ValidFiles -eq $TotalFiles) {
    Write-Host "All $TotalFiles GitHub Copilot files are properly formatted!" -ForegroundColor Green
} else {
    Write-Host "$InvalidFiles out of $TotalFiles files may need updates. See the detailed report for specifics." -ForegroundColor Yellow
}

Write-Host "`nComplete Discovery Summary:" -ForegroundColor Cyan
Write-Host "   Total Files Analyzed: $TotalFiles" -ForegroundColor White
Write-Host "   File Types: $($FileTypeGroups.Count) different types" -ForegroundColor White
Write-Host "   Categories: $($CategoryGroups.Count) different categories" -ForegroundColor White
Write-Host "   Directories: $($DirectoryStats.Count) analyzed" -ForegroundColor White

Write-Host "`nRecommended Actions:" -ForegroundColor Cyan
Write-Host "   1. Test GitHub Copilot with the complete enhanced file ecosystem" -ForegroundColor White
Write-Host "   2. Validate custom prompt integration across specialized subcategories" -ForegroundColor White
Write-Host "   3. Test cross-directory workflow scenarios" -ForegroundColor White
Write-Host "   4. Monitor performance across all categories and subcategories" -ForegroundColor White
Write-Host "   5. Consider consolidating Custom-Prompt and Custom-Prompts directories" -ForegroundColor White
Write-Host "   6. Establish maintenance process for all files" -ForegroundColor White

if ($GenerateReport) {
    Write-Host "`nEnhanced comprehensive analysis report with detailed issue explanations: $OutputReport" -ForegroundColor Cyan
}

Write-Host "`nComplete enhanced ecosystem analysis finished!" -ForegroundColor Green
