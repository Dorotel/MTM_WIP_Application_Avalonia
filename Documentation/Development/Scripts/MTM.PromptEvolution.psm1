# MTM Prompt Evolution PowerShell Module
# Provides easy integration with the Advanced Custom Prompt Evolution System

#Requires -Version 7.0

# Module metadata
$ModuleVersion = "1.0.0"
$ModuleName = "MTM.PromptEvolution"

Write-Host "Loading MTM Prompt Evolution Module v$ModuleVersion..." -ForegroundColor Cyan

# Global variables
$Global:MTM_PromptEvolution_Config = @{
    LogPath = "copilot_usage.log"
    PromptDirectory = ".github\Custom-Prompts"
    ReportsDirectory = "Documentation\Development\Reports"
    Enabled = $true
}

# Function to track prompt usage
function Track-PromptUsage {
    <#
    .SYNOPSIS
    Track usage of a custom prompt for evolution analysis
    
    .DESCRIPTION
    Records prompt usage data for the MTM Prompt Evolution System to analyze patterns and optimize prompts
    
    .PARAMETER PromptName
    Name of the custom prompt being used
    
    .PARAMETER Context
    Context in which the prompt was used (file name, project area, etc.)
    
    .PARAMETER Success
    Whether the prompt usage was successful
    
    .PARAMETER Modifications
    Array of modifications made to the prompt
    
    .EXAMPLE
    Track-PromptUsage -PromptName "CustomPrompt_Create_ReactiveUIViewModel" -Context "InventoryViewModel.cs" -Success $true
    
    .EXAMPLE
    Track-PromptUsage -PromptName "CustomPrompt_Create_UIElement" -Context "MainView.axaml" -Success $false -Modifications @("Added MTM theme", "Fixed binding syntax")
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$PromptName,
        
        [Parameter(Mandatory = $true)]
        [string]$Context,
        
        [Parameter(Mandatory = $false)]
        [bool]$Success = $true,
        
        [Parameter(Mandatory = $false)]
        [string[]]$Modifications = @()
    )
    
    if (-not $Global:MTM_PromptEvolution_Config.Enabled) {
        Write-Verbose "Prompt evolution tracking is disabled"
        return
    }
    
    try {
        $UsageEvent = [PSCustomObject]@{
            Type = "Usage"
            Timestamp = (Get-Date).ToUniversalTime().ToString("o")
            Data = [PSCustomObject]@{
                PromptName = $PromptName
                Context = $Context
                Success = $Success
                Modifications = $Modifications
                FileType = Get-FileType -Context $Context
                ProjectArea = Get-ProjectArea -Context $Context
            }
        }
        
        $LogEntry = $UsageEvent | ConvertTo-Json -Compress
        Add-Content -Path $Global:MTM_PromptEvolution_Config.LogPath -Value $LogEntry
        
        Write-Verbose "Tracked usage: $PromptName in $Context (Success: $Success)"
    }
    catch {
        Write-Warning "Failed to track prompt usage: $_"
    }
}

# Function to suggest prompts based on current context
function Get-PromptSuggestion {
    <#
    .SYNOPSIS
    Get prompt suggestions based on current file or context
    
    .DESCRIPTION
    Analyzes the current context and suggests appropriate custom prompts to use
    
    .PARAMETER FilePath
    Path to the file you're working on
    
    .PARAMETER ProjectArea
    Project area (UI, Service, Database, etc.)
    
    .EXAMPLE
    Get-PromptSuggestion -FilePath "Views\InventoryView.axaml"
    
    .EXAMPLE
    Get-PromptSuggestion -ProjectArea "Service"
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [string]$FilePath,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet("UI", "ViewModel", "Service", "Model", "Database", "Configuration", "Documentation")]
        [string]$ProjectArea
    )
    
    $Suggestions = @()
    
    # Determine context from file path if provided
    if ($FilePath) {
        $FileExtension = [System.IO.Path]::GetExtension($FilePath).ToLower()
        $FileName = [System.IO.Path]::GetFileName($FilePath)
        
        switch ($FileExtension) {
            ".axaml" {
                $Suggestions += @{
                    PromptName = "CustomPrompt_Create_UIElement"
                    Description = "Create Avalonia UI elements with MTM design patterns"
                    Priority = "High"
                    Usage = "@ui:create [$FileName] with MTM theme and compiled bindings"
                }
                $Suggestions += @{
                    PromptName = "CustomPrompt_Create_ModernLayoutPattern"
                    Description = "Create modern layout patterns for Avalonia"
                    Priority = "Medium"
                    Usage = "@ui:layout [layout type] for $FileName"
                }
            }
            ".cs" {
                if ($FileName -like "*ViewModel*") {
                    $Suggestions += @{
                        PromptName = "CustomPrompt_Create_ReactiveUIViewModel"
                        Description = "Create ReactiveUI ViewModels with MTM patterns"
                        Priority = "High"
                        Usage = "@vm:create [$($FileName.Replace('.cs', ''))] with reactive commands"
                    }
                }
                elseif ($FileName -like "*Service*") {
                    $Suggestions += @{
                        PromptName = "CustomPrompt_Create_Service"
                        Description = "Create service classes following MTM organization rules"
                        Priority = "High"
                        Usage = "@service:create [$($FileName.Replace('.cs', ''))] with dependency injection"
                    }
                }
                elseif ($FileName -like "*Model*") {
                    $Suggestions += @{
                        PromptName = "CustomPrompt_Create_Model"
                        Description = "Create model classes with MTM data patterns"
                        Priority = "Medium"
                        Usage = "@model:create [$($FileName.Replace('.cs', ''))] with validation"
                    }
                }
            }
        }
    }
    
    # Add suggestions based on project area
    if ($ProjectArea) {
        switch ($ProjectArea) {
            "Database" {
                $Suggestions += @{
                    PromptName = "CustomPrompt_Create_StoredProcedure"
                    Description = "Create stored procedures following MTM patterns"
                    Priority = "High"
                    Usage = "@db:procedure [procedure name] with error handling"
                }
                $Suggestions += @{
                    PromptName = "CustomPrompt_Database_ErrorHandling"
                    Description = "Implement database error handling patterns"
                    Priority = "Medium"
                    Usage = "@db:error [operation type] with logging"
                }
            }
            "Service" {
                $Suggestions += @{
                    PromptName = "CustomPrompt_Create_CRUDOperations"
                    Description = "Create CRUD operations with Result pattern"
                    Priority = "High"
                    Usage = "@service:crud [entity name] with validation"
                }
                $Suggestions += @{
                    PromptName = "CustomPrompt_Implement_ResultPatternSystem"
                    Description = "Implement Result pattern for service responses"
                    Priority = "Medium"
                    Usage = "@pattern:result [operation type] with error handling"
                }
            }
        }
    }
    
    # Display suggestions
    if ($Suggestions.Count -gt 0) {
        Write-Host "🎯 Prompt Suggestions:" -ForegroundColor Cyan
        foreach ($Suggestion in $Suggestions) {
            $PriorityColor = switch ($Suggestion.Priority) {
                "High" { "Green" }
                "Medium" { "Yellow" }
                "Low" { "Gray" }
            }
            
            Write-Host "  [$($Suggestion.Priority)]" -ForegroundColor $PriorityColor -NoNewline
            Write-Host " $($Suggestion.PromptName)" -ForegroundColor White
            Write-Host "    $($Suggestion.Description)" -ForegroundColor Gray
            Write-Host "    Usage: $($Suggestion.Usage)" -ForegroundColor DarkGray
            Write-Host ""
        }
    } else {
        Write-Host "ℹ️  No specific prompt suggestions for current context" -ForegroundColor Yellow
        Write-Host "   Try: Get-AvailablePrompts to see all available prompts" -ForegroundColor Gray
    }
    
    return $Suggestions
}

# Function to list available prompts
function Get-AvailablePrompts {
    <#
    .SYNOPSIS
    List all available custom prompts
    
    .DESCRIPTION
    Displays all custom prompts available in the MTM system with descriptions and usage
    
    .PARAMETER Category
    Filter prompts by category
    
    .EXAMPLE
    Get-AvailablePrompts
    
    .EXAMPLE
    Get-AvailablePrompts -Category "UI"
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [ValidateSet("UI", "Service", "Database", "Workflow", "Quality", "Documentation")]
        [string]$Category
    )
    
    $PromptDirectory = $Global:MTM_PromptEvolution_Config.PromptDirectory
    
    if (-not (Test-Path $PromptDirectory)) {
        Write-Warning "Custom prompts directory not found: $PromptDirectory"
        return
    }
    
    $PromptFiles = Get-ChildItem -Path $PromptDirectory -Filter "*.md" | Sort-Object Name
    
    if ($PromptFiles.Count -eq 0) {
        Write-Host "No custom prompts found in $PromptDirectory" -ForegroundColor Yellow
        return
    }
    
    Write-Host "📚 Available Custom Prompts:" -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkCyan
    
    foreach ($PromptFile in $PromptFiles) {
        try {
            $Content = Get-Content $PromptFile.FullName -Raw
            
            # Extract category and purpose from markdown
            $CategoryMatch = [regex]::Match($Content, '(?<=## 👤 Persona\s*\*\*)[^*]+(?=\*\*)')
            $PurposeMatch = [regex]::Match($Content, '(?<=## 🎯 Purpose\s*)[^\r\n]+')
            
            $DetectedCategory = if ($CategoryMatch.Success) { 
                $CategoryMatch.Value.Split(' ')[0] 
            } else { 
                "General" 
            }
            
            $Purpose = if ($PurposeMatch.Success) { 
                $PurposeMatch.Value.Trim() 
            } else { 
                "No description available" 
            }
            
            # Filter by category if specified
            if ($Category -and $DetectedCategory -notlike "*$Category*") {
                continue
            }
            
            # Display prompt info
            $PromptName = $PromptFile.BaseName.Replace("CustomPrompt_", "").Replace("_", " ")
            Write-Host "  📄 $PromptName" -ForegroundColor White
            Write-Host "     Category: $DetectedCategory" -ForegroundColor Gray
            Write-Host "     Purpose:  $Purpose" -ForegroundColor Gray
            Write-Host "     File:     $($PromptFile.Name)" -ForegroundColor DarkGray
            Write-Host ""
        }
        catch {
            Write-Warning "Failed to parse prompt file: $($PromptFile.Name)"
        }
    }
}

# Function to run evolution analysis
function Invoke-PromptEvolutionAnalysis {
    <#
    .SYNOPSIS
    Run the prompt evolution analysis
    
    .DESCRIPTION
    Executes the full prompt evolution analysis to identify patterns and generate suggestions
    
    .PARAMETER AutoGenerate
    Automatically generate high-priority prompts
    
    .PARAMETER OpenReport
    Open the generated report in the default browser
    
    .EXAMPLE
    Invoke-PromptEvolutionAnalysis
    
    .EXAMPLE
    Invoke-PromptEvolutionAnalysis -AutoGenerate -OpenReport
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [switch]$AutoGenerate,
        
        [Parameter(Mandatory = $false)]
        [switch]$OpenReport
    )
    
    $AnalysisScript = "Documentation\Development\Scripts\Analyze-PromptUsage.ps1"
    
    if (-not (Test-Path $AnalysisScript)) {
        Write-Error "Analysis script not found: $AnalysisScript"
        return
    }
    
    Write-Host "🔍 Running prompt evolution analysis..." -ForegroundColor Cyan
    
    try {
        $Parameters = @{}
        if ($AutoGenerate) { $Parameters.AutoGenerate = $true }
        
        & $AnalysisScript @Parameters
        
        if ($OpenReport) {
            $ReportFiles = Get-ChildItem -Path $Global:MTM_PromptEvolution_Config.ReportsDirectory -Filter "*.html" | Sort-Object LastWriteTime -Descending
            if ($ReportFiles.Count -gt 0) {
                Start-Process $ReportFiles[0].FullName
            } else {
                Write-Warning "No report files found to open"
            }
        }
        
        Write-Host "✅ Analysis completed successfully!" -ForegroundColor Green
    }
    catch {
        Write-Error "Analysis failed: $_"
    }
}

# Function to setup automation
function Set-PromptEvolutionAutomation {
    <#
    .SYNOPSIS
    Setup automated prompt evolution analysis
    
    .DESCRIPTION
    Configures scheduled tasks and GitHub Actions for automated prompt analysis
    
    .PARAMETER Schedule
    Schedule frequency (Daily, Weekly, Monthly)
    
    .EXAMPLE
    Set-PromptEvolutionAutomation -Schedule Weekly
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet("Daily", "Weekly", "Monthly")]
        [string]$Schedule
    )
    
    $SetupScript = "Documentation\Development\Scripts\Setup-PromptEvolution.ps1"
    
    if (-not (Test-Path $SetupScript)) {
        Write-Error "Setup script not found: $SetupScript"
        return
    }
    
    Write-Host "⚙️  Setting up prompt evolution automation..." -ForegroundColor Cyan
    
    try {
        & $SetupScript -SetupTask -Schedule $Schedule
        Write-Host "✅ Automation setup completed for $Schedule schedule!" -ForegroundColor Green
    }
    catch {
        Write-Error "Setup failed: $_"
    }
}

# Helper functions
function Get-FileType {
    param([string]$Context)
    
    if ([string]::IsNullOrEmpty($Context)) { return "Unknown" }
    
    $extension = [System.IO.Path]::GetExtension($Context).ToLower()
    return switch ($extension) {
        ".axaml" { "UI" }
        ".cs" { 
            if ($Context -like "*ViewModel*") { "ViewModel" }
            elseif ($Context -like "*Service*") { "Service" }
            elseif ($Context -like "*Model*") { "Model" }
            else { "CSharp" }
        }
        ".md" { "Documentation" }
        ".json" { "Configuration" }
        default { "Unknown" }
    }
}

function Get-ProjectArea {
    param([string]$Context)
    
    if ([string]::IsNullOrEmpty($Context)) { return "General" }
    
    $lowerContext = $Context.ToLower()
    
    if ($lowerContext -match "views|ui") { return "UI" }
    if ($lowerContext -match "viewmodels") { return "ViewModel" }
    if ($lowerContext -match "services") { return "Service" }
    if ($lowerContext -match "models") { return "Model" }
    if ($lowerContext -match "database|db") { return "Database" }
    if ($lowerContext -match "documentation|docs") { return "Documentation" }
    
    return "General"
}

# Configuration functions
function Set-PromptEvolutionConfig {
    <#
    .SYNOPSIS
    Configure the prompt evolution system
    
    .DESCRIPTION
    Updates configuration settings for the prompt evolution system
    
    .PARAMETER LogPath
    Path to the usage log file
    
    .PARAMETER PromptDirectory
    Directory containing custom prompts
    
    .PARAMETER Enabled
    Enable or disable tracking
    
    .EXAMPLE
    Set-PromptEvolutionConfig -LogPath "custom_usage.log" -Enabled $true
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [string]$LogPath,
        
        [Parameter(Mandatory = $false)]
        [string]$PromptDirectory,
        
        [Parameter(Mandatory = $false)]
        [bool]$Enabled
    )
    
    if ($LogPath) { $Global:MTM_PromptEvolution_Config.LogPath = $LogPath }
    if ($PromptDirectory) { $Global:MTM_PromptEvolution_Config.PromptDirectory = $PromptDirectory }
    if ($PSBoundParameters.ContainsKey('Enabled')) { $Global:MTM_PromptEvolution_Config.Enabled = $Enabled }
    
    Write-Host "🔧 Configuration updated:" -ForegroundColor Green
    $Global:MTM_PromptEvolution_Config | Format-Table -AutoSize
}

function Get-PromptEvolutionConfig {
    <#
    .SYNOPSIS
    Get current prompt evolution configuration
    
    .DESCRIPTION
    Displays the current configuration settings for the prompt evolution system
    
    .EXAMPLE
    Get-PromptEvolutionConfig
    #>
    [CmdletBinding()]
    param()
    
    Write-Host "⚙️  Current MTM Prompt Evolution Configuration:" -ForegroundColor Cyan
    $Global:MTM_PromptEvolution_Config | Format-Table -AutoSize
}

# Aliases for convenience
Set-Alias -Name "track" -Value "Track-PromptUsage"
Set-Alias -Name "suggest" -Value "Get-PromptSuggestion"
Set-Alias -Name "prompts" -Value "Get-AvailablePrompts"
Set-Alias -Name "evolve" -Value "Invoke-PromptEvolutionAnalysis"

# Export functions
Export-ModuleMember -Function @(
    "Track-PromptUsage",
    "Get-PromptSuggestion", 
    "Get-AvailablePrompts",
    "Invoke-PromptEvolutionAnalysis",
    "Set-PromptEvolutionAutomation",
    "Set-PromptEvolutionConfig",
    "Get-PromptEvolutionConfig"
) -Alias @("track", "suggest", "prompts", "evolve")

Write-Host "✅ MTM Prompt Evolution Module loaded successfully!" -ForegroundColor Green
Write-Host "📖 Available commands: track, suggest, prompts, evolve" -ForegroundColor Cyan
Write-Host "❓ Use Get-Help [command] for detailed usage information" -ForegroundColor Gray
