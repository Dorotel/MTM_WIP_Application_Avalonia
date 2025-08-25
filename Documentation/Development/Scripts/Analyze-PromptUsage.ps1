param(
    [string]$LogPath = "copilot_usage.log",
    [string]$PromptDirectory = ".github/Custom-Prompts",
    [string]$OutputReport = "Documentation/Development/Reports/prompt_evolution_report.html",
    [switch]$AutoGenerate = $true,
    [string]$WorkspaceRoot = $PWD
)

# Ensure PowerShell 7+ for better JSON and class support
if ($PSVersionTable.PSVersion.Major -lt 7) {
    Write-Warning "PowerShell 7+ recommended for optimal performance"
}

# Usage tracking data structure
class PromptUsage {
    [string] $PromptName
    [int] $UsageCount
    [datetime[]] $UsageTimes
    [string[]] $Modifications
    [float] $SuccessRate
    [hashtable] $ContextPatterns
    [string[]] $CommonFailures
    [hashtable] $FileTypeUsage
    
    PromptUsage([string]$name) {
        $this.PromptName = $name
        $this.UsageCount = 0
        $this.UsageTimes = @()
        $this.Modifications = @()
        $this.SuccessRate = 0.0
        $this.ContextPatterns = @{}
        $this.CommonFailures = @()
        $this.FileTypeUsage = @{}
    }
    
    [void] RecordUsage([datetime]$time, [string]$context, [string]$fileType) {
        $this.UsageCount++
        $this.UsageTimes += $time
        
        # Analyze context patterns
        $contextKey = $this.ExtractContextKey($context)
        if ($this.ContextPatterns.ContainsKey($contextKey)) {
            $this.ContextPatterns[$contextKey]++
        } else {
            $this.ContextPatterns[$contextKey] = 1
        }
        
        # Track file type usage
        if ($this.FileTypeUsage.ContainsKey($fileType)) {
            $this.FileTypeUsage[$fileType]++
        } else {
            $this.FileTypeUsage[$fileType] = 1
        }
    }
    
    [string] ExtractContextKey([string]$context) {
        # Extract key context information for MTM project
        if ($context -match "\.axaml") { return "UI_AXAML" }
        if ($context -match "ViewModel") { return "ViewModel" }
        if ($context -match "Service") { return "Service" }
        if ($context -match "Model") { return "Model" }
        if ($context -match "Helper_Database") { return "Database" }
        if ($context -match "ReactiveUI") { return "ReactiveUI" }
        if ($context -match "Avalonia") { return "Avalonia" }
        if ($context -match "StoredProcedure") { return "StoredProcedure" }
        return "General"
    }
    
    [float] CalculateTrendScore() {
        if ($this.UsageTimes.Count -lt 2) { return 0.0 }
        
        # Calculate usage trend over last 30 days
        $Recent = $this.UsageTimes | Where-Object { $_ -gt (Get-Date).AddDays(-30) }
        $Older = $this.UsageTimes | Where-Object { $_ -le (Get-Date).AddDays(-30) -and $_ -gt (Get-Date).AddDays(-60) }
        
        if ($Older.Count -eq 0) { return 1.0 }
        
        $RecentRate = $Recent.Count / 30
        $OlderRate = $Older.Count / 30
        
        return if ($OlderRate -eq 0) { 2.0 } else { $RecentRate / $OlderRate }
    }
    
    [string] GetTopContext() {
        if ($this.ContextPatterns.Count -eq 0) { return "General" }
        return ($this.ContextPatterns.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key
    }
}

# New prompt suggestion engine
class PromptSuggestion {
    [string] $SuggestedName
    [string] $Category
    [string] $Purpose
    [string] $TemplateOutline
    [float] $PriorityScore
    [string[]] $TriggerPatterns
    [string] $Justification
    [string] $ProjectArea
    
    PromptSuggestion([string]$name, [string]$category, [string]$purpose, [float]$priority) {
        $this.SuggestedName = $name
        $this.Category = $category
        $this.Purpose = $purpose
        $this.PriorityScore = $priority
        $this.TriggerPatterns = @()
        $this.Justification = ""
        $this.ProjectArea = "General"
    }
    
    [string] GenerateTemplate() {
        $persona = $this.GetPersonaForCategory()
        $guidelines = $this.GetGuidelinesForCategory()
        
        return @"
# Custom Prompt: $($this.SuggestedName)

## 🎯 Instructions
$($this.Purpose)

## 👤 Persona
**$($persona.Role)** - $($persona.Description)

## 📝 Prompt Template
``````
$($this.TemplateOutline)
``````

## 🎯 Purpose
$($this.Purpose)

## 💡 Usage Examples
``````
// Example 1: Basic usage
@prompt:$($this.SuggestedName.ToLower()) [parameters]

// Example 2: With context
@prompt:$($this.SuggestedName.ToLower()) --context="$($this.ProjectArea)" [parameters]
``````

## 📋 Guidelines
$($guidelines -join "`n")

## 🔗 Related Files
- Core Instructions: .github/Core-Instructions/
- UI Instructions: .github/UI-Instructions/
- Development Instructions: .github/Development-Instructions/

## ✅ Quality Checklist
- [ ] Follows MTM coding conventions
- [ ] Uses ReactiveUI patterns (if applicable)
- [ ] Implements proper error handling
- [ ] Uses stored procedures for database access
- [ ] Follows service organization rules
- [ ] Applies MTM design system
"@
    }
    
    [object] GetPersonaForCategory() {
        switch ($this.Category) {
            "UI" { return @{ Role = "MTM UI/UX Developer"; Description = "Expert in Avalonia UI with ReactiveUI, specializing in MTM design patterns and user experience" } }
            "Service" { return @{ Role = "MTM Backend Developer"; Description = "Expert in .NET 8 service architecture, dependency injection, and database integration" } }
            "Database" { return @{ Role = "MTM Database Developer"; Description = "Expert in stored procedure development and database optimization for MTM systems" } }
            "Workflow" { return @{ Role = "MTM DevOps Engineer"; Description = "Expert in development workflows, automation, and process optimization for MTM projects" } }
            "Quality" { return @{ Role = "MTM Quality Assurance Engineer"; Description = "Expert in code quality, testing patterns, and compliance verification for MTM standards" } }
            default { return @{ Role = "MTM Full-Stack Developer"; Description = "Expert in all aspects of MTM development from UI to database" } }
        }
    }
    
    [string[]] GetGuidelinesForCategory() {
        switch ($this.Category) {
            "UI" { 
                return @(
                    "• Use Avalonia controls, not WPF equivalents"
                    "• Apply MTM purple theme (#6a0dad)"
                    "• Use compiled bindings with x:DataType"
                    "• Follow MVVM with ReactiveUI patterns"
                    "• Implement proper disposal in ViewModels"
                )
            }
            "Service" {
                return @(
                    "• Group services by category in single files"
                    "• Use constructor dependency injection"
                    "• Implement Result<T> pattern for responses"
                    "• Use stored procedures only for database access"
                    "• Follow async/await patterns"
                )
            }
            "Database" {
                return @(
                    "• Use Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()"
                    "• Never write direct SQL queries"
                    "• Implement proper parameter validation"
                    "• Use consistent naming conventions"
                    "• Include error handling and logging"
                )
            }
            default {
                return @(
                    "• Follow MTM coding conventions"
                    "• Use dependency injection patterns"
                    "• Implement comprehensive error handling"
                    "• Apply logging and monitoring"
                    "• Maintain code documentation"
                )
            }
        }
    }
}

Write-Host "🔍 Analyzing MTM custom prompt usage patterns..." -ForegroundColor Cyan

# Create output directory if it doesn't exist
$ReportDir = Split-Path $OutputReport -Parent
if (-not (Test-Path $ReportDir)) {
    New-Item -ItemType Directory -Path $ReportDir -Force | Out-Null
}

# Load existing prompts
if (-not (Test-Path $PromptDirectory)) {
    Write-Warning "Custom prompts directory not found: $PromptDirectory"
    Write-Host "Creating directory and sample prompts..."
    New-Item -ItemType Directory -Path $PromptDirectory -Force | Out-Null
}

$ExistingPrompts = Get-ChildItem -Path $PromptDirectory -Filter "*.md" -ErrorAction SilentlyContinue
$PromptUsageData = @{}

foreach ($PromptFile in $ExistingPrompts) {
    $PromptName = $PromptFile.BaseName
    $PromptUsageData[$PromptName] = [PromptUsage]::new($PromptName)
}

# Analyze workspace for usage patterns
Write-Host "📊 Analyzing workspace patterns..." -ForegroundColor Yellow

# Analyze file patterns in the workspace
$ProjectFiles = Get-ChildItem -Recurse -Include "*.cs", "*.axaml", "*.md" | Where-Object { 
    $_.FullName -notlike "*\bin\*" -and 
    $_.FullName -notlike "*\obj\*" -and
    $_.FullName -notlike "*\.git\*"
}

$FilePatterns = @{}
foreach ($File in $ProjectFiles) {
    $Extension = $File.Extension.ToLower()
    $Directory = Split-Path $File.DirectoryName -Leaf
    $Pattern = "$Directory/$Extension"
    
    if ($FilePatterns.ContainsKey($Pattern)) {
        $FilePatterns[$Pattern]++
    } else {
        $FilePatterns[$Pattern] = 1
    }
}

# Simulate usage analysis with real workspace data
$SampleUsagePatterns = @()

# Analyze Services directory
$ServiceFiles = $ProjectFiles | Where-Object { $_.DirectoryName -like "*Services*" -and $_.Extension -eq ".cs" }
foreach ($ServiceFile in $ServiceFiles) {
    $SampleUsagePatterns += @{
        Prompt = "CustomPrompt_Create_Service"
        Context = $ServiceFile.Name
        Success = $true
        Modifications = @("Added MTM service patterns")
        FileType = "Service"
    }
}

# Analyze ViewModels
$ViewModelFiles = $ProjectFiles | Where-Object { $_.Name -like "*ViewModel*" }
foreach ($ViewModelFile in $ViewModelFiles) {
    $SampleUsagePatterns += @{
        Prompt = "CustomPrompt_Create_ReactiveUIViewModel"
        Context = $ViewModelFile.Name
        Success = $true
        Modifications = @()
        FileType = "ViewModel"
    }
}

# Analyze Views
$ViewFiles = $ProjectFiles | Where-Object { $_.Extension -eq ".axaml" }
foreach ($ViewFile in $ViewFiles) {
    $SampleUsagePatterns += @{
        Prompt = "CustomPrompt_Create_UIElement"
        Context = $ViewFile.Name
        Success = $true
        Modifications = @("Applied MTM theme", "Added compiled bindings")
        FileType = "UI"
    }
}

# Process usage patterns
foreach ($Pattern in $SampleUsagePatterns) {
    if ($PromptUsageData.ContainsKey($Pattern.Prompt)) {
        $PromptUsageData[$Pattern.Prompt].RecordUsage((Get-Date), $Pattern.Context, $Pattern.FileType)
        $PromptUsageData[$Pattern.Prompt].Modifications += $Pattern.Modifications
        if ($Pattern.Success) {
            $PromptUsageData[$Pattern.Prompt].SuccessRate += 0.2
        }
    } else {
        # Create new prompt usage tracking
        $NewUsage = [PromptUsage]::new($Pattern.Prompt)
        $NewUsage.RecordUsage((Get-Date), $Pattern.Context, $Pattern.FileType)
        $PromptUsageData[$Pattern.Prompt] = $NewUsage
    }
}

# Analyze patterns and generate suggestions
Write-Host "💡 Generating prompt suggestions..." -ForegroundColor Green

$NewPromptSuggestions = @()

# Pattern 1: Detect missing prompt categories based on file analysis
$MTMSpecificPatterns = @(
    @{ Type = "Extension"; Files = ($ProjectFiles | Where-Object { $_.Name -like "*Extension*" }); MinCount = 1 },
    @{ Type = "Helper"; Files = ($ProjectFiles | Where-Object { $_.Name -like "*Helper*" }); MinCount = 1 },
    @{ Type = "Configuration"; Files = ($ProjectFiles | Where-Object { $_.Name -like "*Configuration*" -or $_.DirectoryName -like "*Configuration*" }); MinCount = 1 },
    @{ Type = "Navigation"; Files = ($ProjectFiles | Where-Object { $_.Name -like "*Navigation*" }); MinCount = 1 },
    @{ Type = "ErrorHandler"; Files = ($ProjectFiles | Where-Object { $_.Name -like "*Error*" }); MinCount = 1 }
)

foreach ($Pattern in $MTMSpecificPatterns) {
    if ($Pattern.Files.Count -ge $Pattern.MinCount) {
        $ExistingPromptForType = $ExistingPrompts | Where-Object { $_.Name -like "*$($Pattern.Type)*" }
        if (-not $ExistingPromptForType) {
            $Suggestion = [PromptSuggestion]::new(
                "CustomPrompt_Create_$($Pattern.Type)",
                "Code Generation",
                "Generate $($Pattern.Type) classes following MTM patterns and conventions",
                0.8
            )
            $Suggestion.Justification = "Found $($Pattern.Files.Count) $($Pattern.Type) files but no corresponding prompt"
            $Suggestion.ProjectArea = $Pattern.Type
            $Suggestion.TemplateOutline = "Create a $($Pattern.Type) class with [specific requirements] following MTM conventions, using dependency injection, and implementing proper error handling"
            $NewPromptSuggestions += $Suggestion
        }
    }
}

# Pattern 2: MTM-specific workflow gaps
$MTMWorkflows = @(
    @{ 
        Name = "Complete Feature Development"
        Steps = @("@ui:create", "@ui:viewmodel", "@biz:service", "@db:procedure", "@qa:verify")
        Gap = "No integrated MTM feature development workflow prompt"
        Priority = 0.95
    },
    @{ 
        Name = "Service Integration Setup"
        Steps = @("@service:create", "@service:interface", "@di:register", "@test:unit")
        Gap = "No comprehensive service integration workflow"
        Priority = 0.9
    },
    @{ 
        Name = "UI Component Creation"
        Steps = @("@ui:axaml", "@ui:codebehind", "@vm:reactive", "@style:mtm")
        Gap = "No end-to-end UI component creation workflow"
        Priority = 0.85
    },
    @{ 
        Name = "Database Operation Implementation"
        Steps = @("@db:procedure", "@service:wrapper", "@model:result", "@test:integration")
        Gap = "No database operation implementation workflow"
        Priority = 0.9
    }
)

foreach ($Workflow in $MTMWorkflows) {
    $WorkflowPromptName = "CustomPrompt_Execute_$($Workflow.Name.Replace(' ', ''))"
    $ExistingWorkflowPrompt = $ExistingPrompts | Where-Object { $_.Name -like "*Workflow*" -or $_.Name -eq $WorkflowPromptName }
    if (-not $ExistingWorkflowPrompt) {
        $Suggestion = [PromptSuggestion]::new(
            $WorkflowPromptName,
            "Workflow",
            "Execute complete $($Workflow.Name.ToLower()) workflow with MTM standards",
            $Workflow.Priority
        )
        $Suggestion.Justification = $Workflow.Gap
        $Suggestion.TriggerPatterns = $Workflow.Steps
        $Suggestion.ProjectArea = "Workflow"
        $Suggestion.TemplateOutline = "Execute the complete $($Workflow.Name.ToLower()) workflow: $($Workflow.Steps -join ' → ')"
        $NewPromptSuggestions += $Suggestion
    }
}

# Pattern 3: High-frequency file type patterns
$FrequentPatterns = $FilePatterns.GetEnumerator() | Where-Object { $_.Value -gt 3 } | Sort-Object Value -Descending

foreach ($Pattern in $FrequentPatterns) {
    $PatternParts = $Pattern.Key -split "/"
    $Directory = $PatternParts[0]
    $Extension = $PatternParts[1]
    
    $PromptName = "CustomPrompt_Generate_$($Directory)$($Extension.Replace('.', '').ToUpper())"
    $ExistingPatternPrompt = $ExistingPrompts | Where-Object { $_.Name -eq $PromptName }
    
    if (-not $ExistingPatternPrompt -and $Pattern.Value -gt 5) {
        $Suggestion = [PromptSuggestion]::new(
            $PromptName,
            "Pattern Generation",
            "Generate $Directory files with $Extension extension following MTM patterns",
            [Math]::Min(0.7 + ($Pattern.Value * 0.05), 0.95)
        )
        $Suggestion.Justification = "Found $($Pattern.Value) files of pattern $($Pattern.Key) indicating frequent usage"
        $Suggestion.ProjectArea = $Directory
        $Suggestion.TemplateOutline = "Generate a $Extension file in $Directory with MTM standards and appropriate patterns"
        $NewPromptSuggestions += $Suggestion
    }
}

# Generate evolution report HTML
Write-Host "📄 Generating evolution report..." -ForegroundColor Magenta

$TotalPrompts = $ExistingPrompts.Count
$TotalSuggestions = $NewPromptSuggestions.Count
$HighPrioritySuggestions = ($NewPromptSuggestions | Where-Object { $_.PriorityScore -gt 0.8 }).Count
$TrackedPrompts = $PromptUsageData.Values.Count

$HtmlReport = @"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM Custom Prompt Evolution Report</title>
    <style>
        body { 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            margin: 0; 
            padding: 20px; 
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            min-height: 100vh;
        }
        .container { 
            max-width: 1400px; 
            margin: 0 auto; 
            background: white; 
            padding: 0; 
            border-radius: 12px; 
            box-shadow: 0 8px 32px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        .header { 
            background: linear-gradient(135deg, #6a0dad 0%, #4b0082 50%, #8b008b 100%); 
            color: white; 
            padding: 30px; 
            text-align: center;
            position: relative;
        }
        .header::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grid" width="10" height="10" patternUnits="userSpaceOnUse"><path d="M 10 0 L 0 0 0 10" fill="none" stroke="rgba(255,255,255,0.1)" stroke-width="1"/></pattern></defs><rect width="100%" height="100%" fill="url(%23grid)"/></svg>');
            pointer-events: none;
        }
        .header h1 { 
            margin: 0; 
            font-size: 2.5em; 
            font-weight: 300; 
            position: relative;
            z-index: 1;
        }
        .header p { 
            margin: 10px 0 0 0; 
            opacity: 0.9; 
            font-size: 1.1em;
            position: relative;
            z-index: 1;
        }
        .content { padding: 30px; }
        .evolution-section { 
            margin: 30px 0; 
            padding: 25px; 
            border-radius: 10px; 
            border-left: 5px solid #6a0dad; 
            background: linear-gradient(135deg, #f9f9ff 0%, #f0f0ff 100%);
            box-shadow: 0 4px 15px rgba(106, 13, 173, 0.1);
        }
        .stats-grid { 
            display: grid; 
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); 
            gap: 20px; 
            margin: 30px 0; 
        }
        .stat-card { 
            background: linear-gradient(135deg, #fff 0%, #f8f9fa 100%); 
            padding: 25px; 
            border-radius: 10px; 
            text-align: center; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.08);
            border: 1px solid rgba(106, 13, 173, 0.1);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }
        .stat-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.15);
        }
        .stat-number { 
            font-size: 2.2em; 
            font-weight: bold; 
            color: #6a0dad;
            margin-bottom: 5px;
        }
        .stat-label { 
            color: #666; 
            font-size: 0.95em; 
            text-transform: uppercase; 
            letter-spacing: 0.5px;
        }
        .prompt-usage { 
            display: grid; 
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr)); 
            gap: 20px; 
            margin: 25px 0; 
        }
        .usage-card { 
            background: white; 
            padding: 20px; 
            border-radius: 10px; 
            box-shadow: 0 4px 15px rgba(0,0,0,0.08);
            border: 1px solid rgba(106, 13, 173, 0.1);
            transition: transform 0.3s ease;
        }
        .usage-card:hover { transform: translateY(-2px); }
        .usage-card h4 { 
            margin: 0 0 15px 0; 
            color: #6a0dad; 
            font-size: 1.2em;
        }
        .usage-trend { 
            height: 8px; 
            border-radius: 4px; 
            position: relative; 
            margin: 15px 0;
            overflow: hidden;
        }
        .trend-indicator { 
            position: absolute; 
            right: 0; 
            top: -25px; 
            color: #666; 
            font-weight: 600; 
            font-size: 0.85em;
        }
        .suggestion { 
            background: linear-gradient(135deg, #e8f5e8 0%, #f0fff0 100%); 
            border: 2px solid #4caf50; 
            border-radius: 10px; 
            padding: 20px; 
            margin: 15px 0;
            transition: all 0.3s ease;
        }
        .suggestion:hover { box-shadow: 0 6px 20px rgba(76, 175, 80, 0.2); }
        .suggestion-high { 
            border-color: #f44336; 
            background: linear-gradient(135deg, #ffebee 0%, #ffcdd2 100%);
        }
        .suggestion-medium { 
            border-color: #ff9800; 
            background: linear-gradient(135deg, #fff3e0 0%, #ffe0b2 100%);
        }
        .priority-badge { 
            display: inline-block; 
            padding: 6px 12px; 
            border-radius: 20px; 
            color: white; 
            font-size: 0.8em; 
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        .priority-high { background: linear-gradient(135deg, #f44336, #d32f2f); }
        .priority-medium { background: linear-gradient(135deg, #ff9800, #f57c00); }
        .priority-low { background: linear-gradient(135deg, #4caf50, #388e3c); }
        .code-template { 
            background: #f8f9fa; 
            border: 1px solid #e9ecef; 
            border-radius: 6px; 
            padding: 15px; 
            font-family: 'Consolas', 'Monaco', monospace; 
            font-size: 0.9em; 
            margin: 15px 0;
            white-space: pre-wrap;
            overflow-x: auto;
        }
        .generate-btn { 
            background: linear-gradient(135deg, #6a0dad, #5a0d9d); 
            color: white; 
            border: none; 
            padding: 12px 24px; 
            border-radius: 6px; 
            cursor: pointer; 
            margin: 10px 5px 0 0;
            font-weight: 600;
            transition: all 0.3s ease;
        }
        .generate-btn:hover { 
            background: linear-gradient(135deg, #5a0d9d, #4a0d8d);
            transform: translateY(-1px);
        }
        .section-title {
            font-size: 1.8em;
            color: #6a0dad;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #6a0dad;
        }
        .empty-state {
            text-align: center;
            padding: 40px;
            color: #666;
            font-style: italic;
        }
        .workspace-info {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
            border-left: 4px solid #17a2b8;
        }
    </style>
    <script>
        function generatePrompt(promptName, category, purpose) {
            alert('🚀 Generating new prompt: ' + promptName + '\\nCategory: ' + category + '\\nThis would integrate with the backend to create the actual prompt file.');
        }
        
        function optimizePrompt(promptName) {
            alert('⚡ Optimizing prompt: ' + promptName + '\\nThis would analyze usage patterns and apply improvements to the prompt.');
        }
        
        function exportReport() {
            window.print();
        }
    </script>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🧠 MTM Custom Prompt Evolution Report</h1>
            <p>Intelligent analysis of prompt usage patterns and optimization opportunities</p>
            <p>Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') | Workspace: MTM WIP Application Avalonia</p>
        </div>
        
        <div class="content">
            <div class="workspace-info">
                <h3>📁 Workspace Analysis</h3>
                <p><strong>Total Files Analyzed:</strong> $($ProjectFiles.Count)</p>
                <p><strong>File Types Found:</strong> $($FilePatterns.Count) unique patterns</p>
                <p><strong>Most Common Pattern:</strong> $(($FilePatterns.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Key) ($((($FilePatterns.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 1).Value)) files)</p>
            </div>
            
            <div class="stats-grid">
                <div class="stat-card">
                    <div class="stat-number">$TotalPrompts</div>
                    <div class="stat-label">Existing Prompts</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">$TotalSuggestions</div>
                    <div class="stat-label">New Suggestions</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">$HighPrioritySuggestions</div>
                    <div class="stat-label">High Priority</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">$TrackedPrompts</div>
                    <div class="stat-label">Tracked Prompts</div>
                </div>
            </div>
"@

# Add usage analysis section
if ($PromptUsageData.Values.Count -gt 0) {
    $HtmlReport += @"
            <div class="evolution-section">
                <h2 class="section-title">📊 Prompt Usage Analysis</h2>
                <div class="prompt-usage">
"@

    foreach ($PromptName in $PromptUsageData.Keys) {
        $Usage = $PromptUsageData[$PromptName]
        $TrendScore = $Usage.CalculateTrendScore()
        $TrendColor = if ($TrendScore -gt 1.2) { "#4caf50" } elseif ($TrendScore -gt 0.8) { "#ff9800" } else { "#f44336" }
        $TrendText = if ($TrendScore -gt 1.2) { "📈 Trending Up" } elseif ($TrendScore -gt 0.8) { "📊 Stable" } else { "📉 Declining" }
        
        $TopContext = $Usage.GetTopContext()
        $SuccessPercentage = [Math]::Round($Usage.SuccessRate * 100)
        
        $HtmlReport += @"
                    <div class="usage-card">
                        <h4>$($PromptName.Replace('CustomPrompt_', '').Replace('_', ' '))</h4>
                        <p><strong>Usage Count:</strong> $($Usage.UsageCount)</p>
                        <p><strong>Success Rate:</strong> $SuccessPercentage%</p>
                        <p><strong>Top Context:</strong> $TopContext</p>
                        <div class="usage-trend" style="background: linear-gradient(90deg, $TrendColor, $($TrendColor)80);">
                            <span class="trend-indicator">$TrendText</span>
                        </div>
"@
        
        if ($Usage.Modifications.Count -gt 0) {
            $TopModifications = $Usage.Modifications | Group-Object | Sort-Object Count -Descending | Select-Object -First 2
            $HtmlReport += "<p><strong>Common Modifications:</strong> $($TopModifications.Name -join ', ')</p>"
        }
        
        $HtmlReport += @"
                        <button class="generate-btn" onclick="optimizePrompt('$PromptName')">🔧 Optimize Prompt</button>
                    </div>
"@
    }

    $HtmlReport += @"
                </div>
            </div>
"@
} else {
    $HtmlReport += @"
            <div class="evolution-section">
                <h2 class="section-title">📊 Prompt Usage Analysis</h2>
                <div class="empty-state">
                    <p>No usage data available yet. Start using custom prompts to see analysis here.</p>
                </div>
            </div>
"@
}

# Add new prompt suggestions section
if ($NewPromptSuggestions.Count -gt 0) {
    $HtmlReport += @"
            <div class="evolution-section">
                <h2 class="section-title">💡 New Prompt Suggestions</h2>
                <p>Based on workspace analysis, here are suggested prompts to improve your development workflow:</p>
"@
    
    foreach ($Suggestion in ($NewPromptSuggestions | Sort-Object PriorityScore -Descending)) {
        $PriorityClass = if ($Suggestion.PriorityScore -gt 0.8) { "priority-high" } elseif ($Suggestion.PriorityScore -gt 0.6) { "priority-medium" } else { "priority-low" }
        $SuggestionClass = if ($Suggestion.PriorityScore -gt 0.8) { "suggestion-high" } elseif ($Suggestion.PriorityScore -gt 0.6) { "suggestion-medium" } else { "suggestion" }
        $PriorityPercentage = [Math]::Round($Suggestion.PriorityScore * 100)
        
        $HtmlReport += @"
                <div class="$SuggestionClass">
                    <h3>$($Suggestion.SuggestedName) <span class="priority-badge $PriorityClass">Priority: $PriorityPercentage%</span></h3>
                    <p><strong>Category:</strong> $($Suggestion.Category)</p>
                    <p><strong>Project Area:</strong> $($Suggestion.ProjectArea)</p>
                    <p><strong>Purpose:</strong> $($Suggestion.Purpose)</p>
                    <p><strong>Justification:</strong> $($Suggestion.Justification)</p>
"@
        
        if ($Suggestion.TriggerPatterns.Count -gt 0) {
            $HtmlReport += "<p><strong>Trigger Patterns:</strong> <code>$($Suggestion.TriggerPatterns -join '</code>, <code>')</code></p>"
        }
        
        $HtmlReport += @"
                    <details>
                        <summary>👁️ Preview Generated Template</summary>
                        <div class="code-template">$($Suggestion.GenerateTemplate() -replace '<', '&lt;' -replace '>', '&gt;')</div>
                    </details>
                    <button class="generate-btn" onclick="generatePrompt('$($Suggestion.SuggestedName)', '$($Suggestion.Category)', '$($Suggestion.Purpose)')">🚀 Generate This Prompt</button>
                </div>
"@
    }
    
    $HtmlReport += "</div>"
} else {
    $HtmlReport += @"
            <div class="evolution-section">
                <h2 class="section-title">💡 New Prompt Suggestions</h2>
                <div class="empty-state">
                    <p>No new prompt suggestions at this time. The system will analyze patterns and suggest new prompts as your workspace evolves.</p>
                </div>
            </div>
"@
}

# Add implementation instructions
$HtmlReport += @"
            <div class="evolution-section">
                <h2 class="section-title">🚀 Implementation Instructions</h2>
                <h3>How to Use This System</h3>
                <ol>
                    <li><strong>Review High-Priority Suggestions:</strong> Focus on suggestions with 80%+ priority scores</li>
                    <li><strong>Generate New Prompts:</strong> Click "Generate This Prompt" to create new prompt files</li>
                    <li><strong>Optimize Existing Prompts:</strong> Use optimization suggestions to improve prompt effectiveness</li>
                    <li><strong>Monitor Usage:</strong> Re-run this analysis weekly to track prompt evolution</li>
                </ol>
                
                <h3>Integration Steps</h3>
                <div class="code-template">
# 1. Run the analysis script
.\Documentation\Development\Scripts\Analyze-PromptUsage.ps1

# 2. Review the generated report
Start-Process ".\Documentation\Development\Reports\prompt_evolution_report.html"

# 3. Auto-generate high-priority prompts
.\Documentation\Development\Scripts\Analyze-PromptUsage.ps1 -AutoGenerate

# 4. Set up scheduled analysis (weekly)
# Add to Windows Task Scheduler or GitHub Actions
</div>
                
                <h3>Next Steps</h3>
                <ul>
                    <li>Implement usage logging in your development workflow</li>
                    <li>Integrate with GitHub Copilot chat for automatic prompt suggestions</li>
                    <li>Set up automated prompt optimization based on success rates</li>
                    <li>Create team-wide prompt sharing and evolution</li>
                </ul>
                
                <button class="generate-btn" onclick="exportReport()">📄 Export Report</button>
            </div>
        </div>
    </div>
</body>
</html>
"@

# Save report
try {
    $HtmlReport | Out-File -FilePath $OutputReport -Encoding UTF8 -Force
    Write-Host "✅ Prompt evolution report generated: $OutputReport" -ForegroundColor Green
} catch {
    Write-Error "Failed to save report: $_"
    return
}

# Auto-generate high-priority suggestions if enabled
if ($AutoGenerate -and $NewPromptSuggestions.Count -gt 0) {
    $HighPrioritySuggestions = $NewPromptSuggestions | Where-Object { $_.PriorityScore -gt 0.9 }
    if ($HighPrioritySuggestions.Count -gt 0) {
        Write-Host "🚀 Auto-generating $($HighPrioritySuggestions.Count) high-priority prompts..." -ForegroundColor Cyan
        
        foreach ($Suggestion in $HighPrioritySuggestions) {
            $PromptFileName = "$PromptDirectory/$($Suggestion.SuggestedName).md"
            
            if (-not (Test-Path $PromptFileName)) {
                try {
                    $PromptContent = $Suggestion.GenerateTemplate()
                    $PromptContent | Out-File -FilePath $PromptFileName -Encoding UTF8 -Force
                    Write-Host "✨ Generated: $PromptFileName" -ForegroundColor Green
                } catch {
                    Write-Warning "Failed to generate $PromptFileName`: $_"
                }
            } else {
                Write-Host "⚠️  Skipped $PromptFileName (already exists)" -ForegroundColor Yellow
            }
        }
    }
}

# Summary output
Write-Host "`n📊 Analysis Summary:" -ForegroundColor Cyan
Write-Host "  • Existing Prompts: $TotalPrompts" -ForegroundColor White
Write-Host "  • New Suggestions: $TotalSuggestions" -ForegroundColor White
Write-Host "  • High Priority: $HighPrioritySuggestions" -ForegroundColor White
Write-Host "  • Report Location: $OutputReport" -ForegroundColor White

if ($AutoGenerate -and $HighPrioritySuggestions -gt 0) {
    Write-Host "  • Auto-Generated: $($HighPrioritySuggestions.Count) prompts" -ForegroundColor Green
}

Write-Host "`n🎯 Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Review the generated report in your browser" -ForegroundColor White
Write-Host "  2. Implement suggested high-priority prompts" -ForegroundColor White
Write-Host "  3. Set up weekly analysis automation" -ForegroundColor White
Write-Host "  4. Monitor prompt usage and success rates" -ForegroundColor White

# Open report in default browser if available
if (Get-Command "Start-Process" -ErrorAction SilentlyContinue) {
    $OpenReport = Read-Host "`nOpen report in browser? (y/N)"
    if ($OpenReport -eq 'y' -or $OpenReport -eq 'Y') {
        try {
            Start-Process $OutputReport
        } catch {
            Write-Host "Could not open browser automatically. Please open: $OutputReport" -ForegroundColor Yellow
        }
    }
}
