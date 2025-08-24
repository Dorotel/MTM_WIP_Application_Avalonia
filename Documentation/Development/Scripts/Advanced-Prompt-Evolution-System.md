# Advanced Custom Prompt Evolution System

## Overview
Intelligent system that learns from your usage patterns and automatically evolves custom prompts to be more effective, creates new prompts for recurring tasks, and optimizes the prompt library based on actual development workflows.

## Features

### **Usage Pattern Analysis**
- **Frequency Tracking**: Monitor which prompts are used most often
- **Success Rate Analysis**: Track which prompts produce the best results
- **Modification Patterns**: Learn how users modify prompts in practice
- **Context Analysis**: Understand when specific prompts work best
- **Failure Pattern Detection**: Identify prompts that consistently need modification

### **Automatic Prompt Generation**
- **Task Pattern Recognition**: Detect recurring development tasks without existing prompts
- **Workflow Analysis**: Identify multi-step processes that could be automated
- **Code Pattern Mining**: Extract common code patterns into reusable prompts
- **Error Pattern Prompts**: Create prompts to fix frequently occurring issues

### **Prompt Optimization**
- **Template Refinement**: Improve prompt templates based on usage data
- **Parameter Optimization**: Adjust prompt parameters for better results
- **Context Enhancement**: Add better context based on successful interactions
- **Redundancy Elimination**: Merge or remove overlapping prompts

## Implementation

### **Prompt Analytics Engine: Analyze-PromptUsage.ps1**
```powershell
param(
    [string]$LogPath = "copilot_usage.log",
    [string]$PromptDirectory = "Documentation/Development/Custom_Prompts",
    [string]$OutputReport = "prompt_evolution_report.html"
)

# Usage tracking data structure
class PromptUsage {
    [string] $PromptName
    [int] $UsageCount
    [datetime[]] $UsageTimes
    [string[]] $Modifications
    [float] $SuccessRate
    [hashtable] $ContextPatterns
    [string[]] $CommonFailures
    
    PromptUsage([string]$name) {
        $this.PromptName = $name
        $this.UsageCount = 0
        $this.UsageTimes = @()
        $this.Modifications = @()
        $this.SuccessRate = 0.0
        $this.ContextPatterns = @{}
        $this.CommonFailures = @()
    }
    
    [void] RecordUsage([datetime]$time, [string]$context) {
        $this.UsageCount++
        $this.UsageTimes += $time
        
        # Analyze context patterns
        $contextKey = $this.ExtractContextKey($context)
        if ($this.ContextPatterns.ContainsKey($contextKey)) {
            $this.ContextPatterns[$contextKey]++
        } else {
            $this.ContextPatterns[$contextKey] = 1
        }
    }
    
    [string] ExtractContextKey([string]$context) {
        # Extract key context information (file type, project area, etc.)
        if ($context -match "\.axaml") { return "UI_AXAML" }
        if ($context -match "ViewModel") { return "ViewModel" }
        if ($context -match "Service") { return "Service" }
        if ($context -match "Model") { return "Model" }
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
        
        return $RecentRate / $OlderRate
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
    
    PromptSuggestion([string]$name, [string]$category, [string]$purpose, [float]$priority) {
        $this.SuggestedName = $name
        $this.Category = $category
        $this.Purpose = $purpose
        $this.PriorityScore = $priority
        $this.TriggerPatterns = @()
        $this.Justification = ""
    }
    
    [string] GenerateTemplate() {
        return @"
# Custom Prompt: $($this.SuggestedName)

## ??? Instructions
$($this.Purpose)

## ?? Persona
**[Appropriate Persona]** - [Role description based on category]

## ?? Prompt Template
```
[Generated template based on analysis]
```

## ?? Purpose
$($this.Purpose)

## ?? Usage Examples
[Generated from pattern analysis]

## ?? Guidelines
[Best practices for this prompt type]

## ?? Related Files
[Cross-references to relevant instruction files]

## ? Quality Checklist
[Verification points for successful usage]
"@
    }
}

Write-Host "?? Analyzing custom prompt usage patterns..."

# Load existing prompts
$ExistingPrompts = Get-ChildItem -Path $PromptDirectory -Filter "CustomPrompt_*.md"
$PromptUsageData = @{}

foreach ($PromptFile in $ExistingPrompts) {
    $PromptName = $PromptFile.BaseName
    $PromptUsageData[$PromptName] = [PromptUsage]::new($PromptName)
}

# Simulate usage analysis (in real implementation, this would parse actual usage logs)
# For demonstration, we'll create some sample data
$SampleUsagePatterns = @(
    @{ Prompt = "CustomPrompt_Create_UIElement"; Context = "AdvancedRemoveView.axaml"; Success = $true; Modifications = @("Added MTM theme") },
    @{ Prompt = "CustomPrompt_Verify_CodeCompliance"; Context = "AdvancedRemoveView.axaml.cs"; Success = $true; Modifications = @() },
    @{ Prompt = "CustomPrompt_Create_ReactiveUIViewModel"; Context = "AdvancedRemoveViewModel.cs"; Success = $false; Modifications = @("Needed better command examples") }
)

foreach ($Pattern in $SampleUsagePatterns) {
    if ($PromptUsageData.ContainsKey($Pattern.Prompt)) {
        $PromptUsageData[$Pattern.Prompt].RecordUsage((Get-Date), $Pattern.Context)
        $PromptUsageData[$Pattern.Prompt].Modifications += $Pattern.Modifications
        if ($Pattern.Success) {
            $PromptUsageData[$Pattern.Prompt].SuccessRate += 0.1
        }
    }
}

# Analyze patterns and generate suggestions
$NewPromptSuggestions = @()

# Pattern 1: Detect missing prompt categories
$CodeFiles = Get-ChildItem -Recurse -Filter "*.cs" | Where-Object { $_.FullName -notlike "*Test*" }
$FileTypes = $CodeFiles | ForEach-Object { 
    if ($_.Name -like "*Extension*") { "Extension" }
    elseif ($_.Name -like "*Helper*") { "Helper" }
    elseif ($_.Name -like "*Utility*") { "Utility" }
    elseif ($_.FullName -like "*Configuration*") { "Configuration" }
    else { "Unknown" }
} | Group-Object | Where-Object { $_.Count -gt 2 }

foreach ($FileType in $FileTypes) {
    $ExistingPromptForType = $ExistingPrompts | Where-Object { $_.Name -like "*$($FileType.Name)*" }
    if (-not $ExistingPromptForType) {
        $Suggestion = [PromptSuggestion]::new(
            "CustomPrompt_Create_$($FileType.Name)",
            "Code Generation",
            "Generate $($FileType.Name) classes with MTM patterns",
            0.8
        )
        $Suggestion.Justification = "Found $($FileType.Count) $($FileType.Name) files but no corresponding prompt"
        $NewPromptSuggestions += $Suggestion
    }
}

# Pattern 2: Detect workflow gaps
$CommonWorkflows = @(
    @{ Name = "Complete Feature Development"; Steps = @("@ui:create", "@ui:viewmodel", "@biz:handler", "@qa:verify"); Gap = "No integrated workflow prompt" },
    @{ Name = "Database Integration Setup"; Steps = @("@db:procedure", "@db:service", "@sys:di"); Gap = "No end-to-end database setup prompt" },
    @{ Name = "Quality Assurance Workflow"; Steps = @("@qa:verify", "@qa:refactor", "@qa:test"); Gap = "No comprehensive QA workflow prompt" }
)

foreach ($Workflow in $CommonWorkflows) {
    $WorkflowPrompt = $ExistingPrompts | Where-Object { $_.Name -like "*Workflow*" -or $_.Name -like "*$($Workflow.Name.Replace(' ', ''))*" }
    if (-not $WorkflowPrompt) {
        $Suggestion = [PromptSuggestion]::new(
            "CustomPrompt_Execute_$($Workflow.Name.Replace(' ', ''))",
            "Workflow",
            "Execute complete $($Workflow.Name.ToLower()) workflow",
            0.9
        )
        $Suggestion.Justification = $Workflow.Gap
        $Suggestion.TriggerPatterns = $Workflow.Steps
        $NewPromptSuggestions += $Suggestion
    }
}

# Generate evolution report
$HtmlReport = @"
<!DOCTYPE html>
<html>
<head>
    <title>Custom Prompt Evolution Report</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 1400px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #6a0dad, #4b0082); color: white; padding: 20px; margin: -20px -20px 20px -20px; border-radius: 8px 8px 0 0; }
        .evolution-section { margin: 20px 0; padding: 20px; border-radius: 8px; border-left: 4px solid #6a0dad; background: #f9f9ff; }
        .prompt-usage { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 15px; margin: 20px 0; }
        .usage-card { background: white; padding: 15px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
        .usage-trend { height: 40px; background: linear-gradient(90deg, #4caf50, #8bc34a); border-radius: 4px; position: relative; }
        .trend-indicator { position: absolute; right: 10px; top: 50%; transform: translateY(-50%); color: white; font-weight: bold; }
        .suggestion { background: #e8f5e8; border: 1px solid #4caf50; border-radius: 8px; padding: 15px; margin: 10px 0; }
        .suggestion-high { border-color: #f44336; background: #ffebee; }
        .suggestion-medium { border-color: #ff9800; background: #fff3e0; }
        .priority-badge { display: inline-block; padding: 4px 8px; border-radius: 12px; color: white; font-size: 0.8em; }
        .priority-high { background: #f44336; }
        .priority-medium { background: #ff9800; }
        .priority-low { background: #4caf50; }
        .code-template { background: #f8f8f8; border: 1px solid #ddd; border-radius: 4px; padding: 10px; font-family: monospace; font-size: 0.9em; margin: 10px 0; }
        .generate-btn { background: #6a0dad; color: white; border: none; padding: 10px 20px; border-radius: 4px; cursor: pointer; margin: 10px 0; }
        .generate-btn:hover { background: #5a0d9d; }
        .stats-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 15px; margin: 20px 0; }
        .stat-card { background: #f8f9fa; padding: 15px; border-radius: 8px; text-align: center; }
        .stat-number { font-size: 1.8em; font-weight: bold; color: #6a0dad; }
    </style>
    <script>
        function generatePrompt(promptName, category, purpose) {
            alert('Generating new prompt: ' + promptName + ' in category: ' + category);
            // Would integrate with backend to actually generate the prompt file
        }
        
        function optimizePrompt(promptName) {
            alert('Optimizing prompt: ' + promptName);
            // Would integrate with backend to apply optimizations
        }
    </script>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>?? Custom Prompt Evolution Report</h1>
            <p>Intelligent analysis of prompt usage patterns and optimization opportunities</p>
            <p>Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</p>
        </div>
        
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-number">$($ExistingPrompts.Count)</div>
                <div>Existing Prompts</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$($NewPromptSuggestions.Count)</div>
                <div>New Suggestions</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$(($NewPromptSuggestions | Where-Object { $_.PriorityScore -gt 0.8 }).Count)</div>
                <div>High Priority</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">$($PromptUsageData.Values.Count)</div>
                <div>Tracked Prompts</div>
            </div>
        </div>
"@

# Add usage analysis section
$HtmlReport += @"
        <div class="evolution-section">
            <h2>?? Prompt Usage Analysis</h2>
            <div class="prompt-usage">
"@

foreach ($PromptName in $PromptUsageData.Keys) {
    $Usage = $PromptUsageData[$PromptName]
    $TrendScore = $Usage.CalculateTrendScore()
    $TrendColor = if ($TrendScore -gt 1.2) { "#4caf50" } elseif ($TrendScore -gt 0.8) { "#ff9800" } else { "#f44336" }
    $TrendText = if ($TrendScore -gt 1.2) { "Trending Up" } elseif ($TrendScore -gt 0.8) { "Stable" } else { "Declining" }
    
    $HtmlReport += @"
                <div class="usage-card">
                    <h4>$($PromptName.Replace('CustomPrompt_', '').Replace('_', ' '))</h4>
                    <p><strong>Usage Count:</strong> $($Usage.UsageCount)</p>
                    <p><strong>Success Rate:</strong> $([Math]::Round($Usage.SuccessRate * 100))%</p>
                    <div class="usage-trend" style="background: linear-gradient(90deg, $TrendColor, $($TrendColor)80);">
                        <span class="trend-indicator">$TrendText</span>
                    </div>
                    <p><strong>Common Context:</strong> $(($Usage.ContextPatterns.Keys | Sort-Object { $Usage.ContextPatterns[$_] } -Descending | Select-Object -First 1) -join ', ')</p>
                    <button class="generate-btn" onclick="optimizePrompt('$PromptName')">Optimize Prompt</button>
                </div>
"@
}

$HtmlReport += @"
            </div>
        </div>
"@

# Add new prompt suggestions section
if ($NewPromptSuggestions.Count -gt 0) {
    $HtmlReport += @"
        <div class="evolution-section">
            <h2>?? New Prompt Suggestions</h2>
"@
    
    foreach ($Suggestion in $NewPromptSuggestions | Sort-Object PriorityScore -Descending) {
        $PriorityClass = if ($Suggestion.PriorityScore -gt 0.8) { "priority-high" } elseif ($Suggestion.PriorityScore -gt 0.6) { "priority-medium" } else { "priority-low" }
        $SuggestionClass = if ($Suggestion.PriorityScore -gt 0.8) { "suggestion-high" } elseif ($Suggestion.PriorityScore -gt 0.6) { "suggestion-medium" } else { "suggestion" }
        
        $HtmlReport += @"
            <div class="$SuggestionClass">
                <h3>$($Suggestion.SuggestedName) <span class="priority-badge $PriorityClass">Priority: $([Math]::Round($Suggestion.PriorityScore * 100))%</span></h3>
                <p><strong>Category:</strong> $($Suggestion.Category)</p>
                <p><strong>Purpose:</strong> $($Suggestion.Purpose)</p>
                <p><strong>Justification:</strong> $($Suggestion.Justification)</p>
"@
        
        if ($Suggestion.TriggerPatterns.Count -gt 0) {
            $HtmlReport += "<p><strong>Trigger Patterns:</strong> $($Suggestion.TriggerPatterns -join ', ')</p>"
        }
        
        $HtmlReport += @"
                <div class="code-template">$($Suggestion.GenerateTemplate())</div>
                <button class="generate-btn" onclick="generatePrompt('$($Suggestion.SuggestedName)', '$($Suggestion.Category)', '$($Suggestion.Purpose)')">Generate This Prompt</button>
            </div>
"@
    }
    
    $HtmlReport += "</div>"
}

$HtmlReport += @"
    </div>
</body>
</html>
"@

# Save report
$HtmlReport | Out-File -FilePath $OutputReport -Encoding UTF8
Write-Host "? Prompt evolution report generated: $OutputReport"

# Auto-generate high-priority suggestions
$HighPrioritySuggestions = $NewPromptSuggestions | Where-Object { $_.PriorityScore -gt 0.9 }
if ($HighPrioritySuggestions.Count -gt 0) {
    Write-Host "?? Auto-generating $($HighPrioritySuggestions.Count) high-priority prompts..."
    
    foreach ($Suggestion in $HighPrioritySuggestions) {
        $PromptFileName = "$PromptDirectory/$($Suggestion.SuggestedName).md"
        $PromptContent = $Suggestion.GenerateTemplate()
        
        if (-not (Test-Path $PromptFileName)) {
            $PromptContent | Out-File -FilePath $PromptFileName -Encoding UTF8
            Write-Host "? Generated: $PromptFileName"
        }
    }
}
```