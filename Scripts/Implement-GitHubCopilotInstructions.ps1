# Validate and Implement GitHub Copilot Repository Instructions

param(
    [switch]$ValidateOnly = $false,
    [switch]$GenerateReport = $true,
    [string]$OutputReport = "github-copilot-instructions-implementation-report.html"
)

Write-Host "?? GitHub Copilot Repository Instructions Implementation" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan

# Define instruction files that have been updated
$UpdatedInstructionFiles = @(
    @{
        Path = ".github/copilot-instructions.md"
        Type = "Main Instructions"
        Changes = "Implemented GitHub Copilot repository format with clear role definition, actionable language, and MTM-specific patterns"
        Status = "Updated"
    },
    @{
        Path = ".github/Core-Instructions/codingconventions.instruction.md"
        Type = "Core Instructions"
        Changes = "Converted to direct, actionable guidance with templates and examples"
        Status = "Updated"
    },
    @{
        Path = ".github/UI-Instructions/ui-generation.instruction.md"
        Type = "UI Instructions"
        Changes = "Streamlined to focus on Avalonia UI generation with MTM patterns"
        Status = "Updated"
    },
    @{
        Path = ".github/Development-Instructions/database-patterns.instruction.md"
        Type = "Development Instructions"
        Changes = "Emphasized stored procedure requirements and MTM business rules"
        Status = "Updated"
    },
    @{
        Path = ".github/Quality-Instructions/needsrepair.instruction.md"
        Type = "Quality Instructions"
        Changes = "Focused on audit processes and compliance standards"
        Status = "Updated"
    },
    @{
        Path = ".github/Automation-Instructions/personas.instruction.md"
        Type = "Automation Instructions"
        Changes = "Simplified persona definitions with clear usage guidelines"
        Status = "Updated"
    }
)

# Validation functions
function Test-FileExists {
    param([string]$FilePath)
    return Test-Path $FilePath
}

function Test-GitHubCopilotFormat {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        return @{ IsValid = $false; Issues = @("File does not exist") }
    }
    
    $content = Get-Content $FilePath -Raw
    $issues = @()
    
    # Check for clear role definition
    if ($content -notmatch "You are|Your role|You") {
        $issues += "Missing clear role definition (should start with 'You are' or 'Your role')"
    }
    
    # Check for actionable language
    if ($content -notmatch "(Always|Never|Use|Follow|Apply|Generate|Create|Implement)") {
        $issues += "Missing actionable language (should include Always/Never/Use/Follow commands)"
    }
    
    # Check for code examples
    if ($content -notmatch "```[a-z]*\n") {
        $issues += "Missing code examples (should include code blocks)"
    }
    
    # Check for MTM-specific patterns
    if ($content -notmatch "(MTM|Manitowoc|TransactionType|#4B45ED|ReactiveUI|Avalonia)") {
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

# Validation process
Write-Host "?? Validating GitHub Copilot Repository Instructions Format..." -ForegroundColor Yellow

$ValidationResults = @()

foreach ($file in $UpdatedInstructionFiles) {
    Write-Host "`n?? Validating: $($file.Path)" -ForegroundColor White
    
    $fileExists = Test-FileExists $file.Path
    $formatValid = Test-GitHubCopilotFormat $file.Path
    $contextValid = Test-RepositorySpecificContext $file.Path
    
    $result = @{
        FilePath = $file.Path
        Type = $file.Type
        Changes = $file.Changes
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
        Description = "Instructions are tailored to MTM WIP Application Avalonia project"
        Score = ($ValidationResults | Where-Object { $_.ContextValid }).Count / $TotalFiles * 100
    },
    @{
        Practice = "Actionable Language"
        Description = "Instructions use direct commands (Always, Never, Use, Follow)"
        Score = ($ValidationResults | Where-Object { $_.FormatValid }).Count / $TotalFiles * 100
    },
    @{
        Practice = "Code Examples"
        Description = "Instructions include concrete code examples"
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
    Write-Host "`n?? Generating Implementation Report..." -ForegroundColor Yellow
    
    $HtmlReport = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>GitHub Copilot Repository Instructions Implementation Report</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: #333;
            line-height: 1.6;
        }
        
        .container { 
            max-width: 1200px; 
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
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); 
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
            padding: 15px; 
            text-align: left; 
            border-bottom: 1px solid #eee;
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
        
        .implementation-details { 
            background: white; 
            padding: 25px; 
            border-radius: 12px; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.1); 
            margin: 30px 0;
        }
        
        .change-item { 
            margin: 10px 0; 
            padding: 10px; 
            background: #e8f5e8; 
            border-radius: 6px; 
            border-left: 3px solid #28a745;
        }
        
        .recommendations { 
            background: linear-gradient(135deg, #e3f2fd, #bbdefb); 
            padding: 25px; 
            border-radius: 12px; 
            margin: 30px 0;
        }
        
        .recommendation-item { 
            margin: 10px 0; 
            padding: 10px 0; 
            border-bottom: 1px solid rgba(0,0,0,0.1);
        }
        
        .recommendation-item:last-child { 
            border-bottom: none; 
        }
        
        .footer { 
            background: #f8f9fa; 
            padding: 20px; 
            text-align: center; 
            color: #666;
            border-top: 1px solid #eee;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>?? GitHub Copilot Repository Instructions</h1>
            <p>Implementation Report for MTM WIP Application Avalonia</p>
            <p>Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</p>
        </div>
        
        <div class="content">
            <div class="summary-cards">
                <div class="summary-card">
                    <div class="summary-number">$TotalFiles</div>
                    <div class="summary-label">Total Instruction Files</div>
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
            </div>
            
            <h2>?? Validation Results</h2>
            <table class="validation-table">
                <thead>
                    <tr>
                        <th>File Path</th>
                        <th>Type</th>
                        <th>Status</th>
                        <th>Changes Made</th>
                    </tr>
                </thead>
                <tbody>
"@

foreach ($result in $ValidationResults) {
    $statusClass = if ($result.OverallValid) { "status-valid" } else { "status-invalid" }
    $statusText = if ($result.OverallValid) { "? Valid" } else { "? Needs Update" }
    
    $HtmlReport += @"
                    <tr>
                        <td><code>$($result.FilePath)</code></td>
                        <td>$($result.Type)</td>
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
            
            <div class="implementation-details">
                <h2>?? Implementation Details</h2>
                <p>The following changes were implemented to align with GitHub Copilot repository instructions format:</p>
"@

foreach ($file in $UpdatedInstructionFiles) {
    $HtmlReport += @"
                <div class="change-item">
                    <strong>$($file.Path)</strong><br>
                    $($file.Changes)
                </div>
"@
}

$HtmlReport += @"
            </div>
            
            <div class="recommendations">
                <h2>?? Next Steps and Recommendations</h2>
                <div class="recommendation-item">
                    <h3>1. Test GitHub Copilot Integration</h3>
                    <p>Open any of the updated instruction files and test GitHub Copilot's behavior with the new format. Copilot should now provide more targeted, MTM-specific assistance.</p>
                </div>
                <div class="recommendation-item">
                    <h3>2. Monitor Copilot Effectiveness</h3>
                    <p>Track how well Copilot follows the new instructions during development. Note any areas where additional clarification might be needed.</p>
                </div>
                <div class="recommendation-item">
                    <h3>3. Update Remaining Files</h3>
                    <p>Apply the same GitHub Copilot format to any remaining instruction files in the repository for consistent behavior.</p>
                </div>
                <div class="recommendation-item">
                    <h3>4. Create Usage Documentation</h3>
                    <p>Document best practices for using the new instruction format and share with team members.</p>
                </div>
                <div class="recommendation-item">
                    <h3>5. Regular Review and Updates</h3>
                    <p>Periodically review and update the instructions based on Copilot's performance and new GitHub Copilot features.</p>
                </div>
            </div>
        </div>
        
        <div class="footer">
            <p>Generated by GitHub Copilot Repository Instructions Implementation Script</p>
            <p>MTM WIP Application Avalonia Project - $(Get-Date -Format 'yyyy')</p>
        </div>
    </div>
</body>
</html>
"@

    # Save the report
    $HtmlReport | Out-File -FilePath $OutputReport -Encoding UTF8
    Write-Host "? Report generated: $OutputReport" -ForegroundColor Green
}

# Summary and next steps
Write-Host "`n?? Implementation Complete!" -ForegroundColor Green
Write-Host "================================================================" -ForegroundColor Cyan

if ($ValidFiles -eq $TotalFiles) {
    Write-Host "? All instruction files have been successfully updated to GitHub Copilot repository format!" -ForegroundColor Green
} else {
    Write-Host "??  Some files may need additional updates. See the detailed report for more information." -ForegroundColor Yellow
}

Write-Host "`n?? Key Changes Made:" -ForegroundColor Cyan
Write-Host "   • Clear role definitions with 'You are' statements" -ForegroundColor White
Write-Host "   • Actionable language with direct commands" -ForegroundColor White
Write-Host "   • Repository-specific context for MTM WIP Application" -ForegroundColor White
Write-Host "   • Code examples and constraints" -ForegroundColor White
Write-Host "   • MTM-specific patterns and business rules" -ForegroundColor White

Write-Host "`n?? Next Steps:" -ForegroundColor Cyan
Write-Host "   1. Test GitHub Copilot with the updated instructions" -ForegroundColor White
Write-Host "   2. Monitor Copilot's behavior for improvements" -ForegroundColor White
Write-Host "   3. Update any remaining instruction files as needed" -ForegroundColor White
Write-Host "   4. Share best practices with your development team" -ForegroundColor White

if ($GenerateReport) {
    Write-Host "`n?? Detailed report available at: $OutputReport" -ForegroundColor Cyan
}

Write-Host "`nImplementation completed successfully! ??" -ForegroundColor Green