# MTM Prompt Evolution System - Quick Setup for PowerShell 5.1
# Compatible with older PowerShell versions

Write-Host "🧠 Loading MTM Prompt Evolution System..." -ForegroundColor Magenta

# Core tracking function
function Track-PromptUsage {
    param(
        [Parameter(Mandatory=$true)][string]$PromptName,
        [Parameter(Mandatory=$true)][string]$Context,
        [bool]$Success = $true,
        [string[]]$Modifications = @()
    )
    
    Write-Host "📊 Tracked: $PromptName in $Context (Success: $Success)" -ForegroundColor Green
    
    $UsageEvent = @{
        Type = "Usage"
        Timestamp = (Get-Date).ToString("o")
        PromptName = $PromptName
        Context = $Context
        Success = $Success
        Modifications = $Modifications
    }
    
    $UsageEvent | ConvertTo-Json -Compress | Add-Content -Path "copilot_usage.log"
}

# Suggestion function
function Get-PromptSuggestion {
    param(
        [string]$FilePath,
        [ValidateSet("UI", "ViewModel", "Service", "Model", "Database")]
        [string]$ProjectArea
    )
    
    Write-Host "🎯 Prompt Suggestions:" -ForegroundColor Cyan
    
    if ($FilePath) {
        if ($FilePath -like "*.axaml") {
            Write-Host "  [High] CustomPrompt_Create_UIElement - Create Avalonia UI elements with MTM design patterns" -ForegroundColor Green
            Write-Host "  [Medium] CustomPrompt_Create_ModernLayoutPattern - Create modern layout patterns" -ForegroundColor Yellow
        }
        elseif ($FilePath -like "*ViewModel*") {
            Write-Host "  [High] CustomPrompt_Create_ReactiveUIViewModel - Create ReactiveUI ViewModels with MTM patterns" -ForegroundColor Green
        }
        elseif ($FilePath -like "*Service*") {
            Write-Host "  [High] CustomPrompt_Create_Service - Create service classes following MTM organization rules" -ForegroundColor Green
            Write-Host "  [Medium] CustomPrompt_Create_CRUDOperations - Create CRUD operations with Result pattern" -ForegroundColor Yellow
        }
        else {
            Write-Host "  [Medium] CustomPrompt_Quick_Analysis - Analyze current context for optimization opportunities" -ForegroundColor Yellow
        }
    }
    
    if ($ProjectArea) {
        switch ($ProjectArea) {
            "Database" {
                Write-Host "  [High] CustomPrompt_Create_StoredProcedure - Create stored procedures following MTM patterns" -ForegroundColor Green
                Write-Host "  [Medium] CustomPrompt_Database_ErrorHandling - Implement database error handling patterns" -ForegroundColor Yellow
            }
            "Service" {
                Write-Host "  [High] CustomPrompt_Create_CRUDOperations - Create CRUD operations with Result pattern" -ForegroundColor Green
                Write-Host "  [Medium] CustomPrompt_Implement_ResultPatternSystem - Implement Result pattern for service responses" -ForegroundColor Yellow
            }
        }
    }
}

# List available prompts
function Get-AvailablePrompts {
    param([string]$Category)
    
    Write-Host "📚 Available Custom Prompts:" -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkCyan
    
    $PromptFiles = Get-ChildItem -Path ".github\Custom-Prompts" -Filter "*.md" -ErrorAction SilentlyContinue
    
    if ($PromptFiles) {
        foreach ($PromptFile in $PromptFiles) {
            $PromptName = $PromptFile.BaseName.Replace("CustomPrompt_", "").Replace("_", " ")
            Write-Host "  📄 $PromptName" -ForegroundColor White
            Write-Host "     File: $($PromptFile.Name)" -ForegroundColor DarkGray
            Write-Host ""
        }
    } else {
        Write-Host "  No custom prompts found in .github\Custom-Prompts" -ForegroundColor Yellow
    }
}

# Analysis function
function Invoke-PromptEvolutionAnalysis {
    param([switch]$AutoGenerate, [switch]$OpenReport)
    
    Write-Host "🔍 Running MTM Prompt Evolution Analysis..." -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkCyan
    
    # Basic workspace analysis
    $TotalCS = (Get-ChildItem -Recurse -Filter '*.cs' | Where-Object { 
        $_.FullName -notlike '*\obj\*' -and $_.FullName -notlike '*\bin\*' 
    } | Measure-Object).Count
    
    $TotalAxaml = (Get-ChildItem -Recurse -Filter '*.axaml' | Measure-Object).Count
    $TotalPrompts = (Get-ChildItem -Path '.github\Custom-Prompts' -Filter '*.md' -ErrorAction SilentlyContinue | Measure-Object).Count
    
    Write-Host "📊 Workspace Analysis Results:" -ForegroundColor Cyan
    Write-Host "  📁 C# Files: $TotalCS" -ForegroundColor White
    Write-Host "  🎨 AXAML Files: $TotalAxaml" -ForegroundColor White
    Write-Host "  📝 Custom Prompts: $TotalPrompts" -ForegroundColor White
    
    # Pattern analysis
    $ServiceFiles = Get-ChildItem -Recurse -Filter "*Service*.cs" | Where-Object { 
        $_.FullName -notlike '*\obj\*' -and $_.FullName -notlike '*\bin\*' 
    }
    $ViewModelFiles = Get-ChildItem -Recurse -Filter "*ViewModel*.cs"
    
    Write-Host ""
    Write-Host "🎯 Pattern Analysis:" -ForegroundColor Cyan
    Write-Host "  🔧 Service Files: $($ServiceFiles.Count)" -ForegroundColor White
    Write-Host "  📱 ViewModel Files: $($ViewModelFiles.Count)" -ForegroundColor White
    Write-Host "  💡 Optimization Opportunities:" -ForegroundColor Yellow
    
    if ($TotalPrompts -lt 10) {
        Write-Host "    • Consider adding more specialized prompts" -ForegroundColor Gray
    }
    if ($ServiceFiles.Count -gt 5) {
        Write-Host "    • Service organization patterns are well established" -ForegroundColor Gray
    }
    if ($ViewModelFiles.Count -gt 3) {
        Write-Host "    • ReactiveUI patterns are actively used" -ForegroundColor Gray
    }
    
    Write-Host ""
    Write-Host "✅ Analysis completed successfully!" -ForegroundColor Green
}

# Configuration function
function Get-PromptEvolutionConfig {
    Write-Host "⚙️ MTM Prompt Evolution Configuration:" -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkCyan
    Write-Host "  📁 Log Path: copilot_usage.log" -ForegroundColor White
    Write-Host "  📝 Prompt Directory: .github\Custom-Prompts" -ForegroundColor White
    Write-Host "  📊 Reports Directory: Documentation\Development\Reports" -ForegroundColor White
    Write-Host "  ✅ Tracking Enabled: True" -ForegroundColor Green
    Write-Host "  🔄 Auto-Generate: True" -ForegroundColor Green
    Write-Host "  💻 PowerShell Version: $($PSVersionTable.PSVersion)" -ForegroundColor White
}

# Create aliases for easy use
Set-Alias -Name "track" -Value "Track-PromptUsage"
Set-Alias -Name "suggest" -Value "Get-PromptSuggestion"
Set-Alias -Name "prompts" -Value "Get-AvailablePrompts"
Set-Alias -Name "evolve" -Value "Invoke-PromptEvolutionAnalysis"

# Welcome message
Write-Host "✅ MTM Prompt Evolution System Loaded Successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Available Commands:" -ForegroundColor Cyan
Write-Host "  • track 'PromptName' 'Context' - Track prompt usage" -ForegroundColor White
Write-Host "  • suggest -FilePath 'file.cs' - Get contextual suggestions" -ForegroundColor White  
Write-Host "  • prompts - List all available prompts" -ForegroundColor White
Write-Host "  • evolve - Run evolution analysis" -ForegroundColor White
Write-Host "  • Get-PromptEvolutionConfig - View configuration" -ForegroundColor White
Write-Host ""
Write-Host "💡 Quick Start Examples:" -ForegroundColor Yellow
Write-Host "  track 'CustomPrompt_Create_Service' 'UserService.cs'" -ForegroundColor Gray
Write-Host "  suggest -FilePath 'Views\InventoryView.axaml'" -ForegroundColor Gray
Write-Host "  evolve" -ForegroundColor Gray
Write-Host ""
Write-Host "🎯 The system is ready to learn from your prompt usage patterns!" -ForegroundColor Green
