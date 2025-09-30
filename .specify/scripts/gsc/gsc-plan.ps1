#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Enhanced GitHub Spec Command: Plan Implementation with Universal Development Patterns
    Part of the Complete GitHub Spec Commands Enhancement System

.DESCRIPTION
    Creates comprehensive implementation plans with universal development patterns integration.
    Integrates systematic planning workflows from memory.instructions.md including:
    - Evidence-based development workflows
    - Container layout principles
    - Style architecture patterns
    - Systematic problem resolution
    - Multi-platform development considerations

.PARAMETER Action
    The planning action to perform:
    - create: Create new implementation plan
    - update: Update existing plan
    - validate: Validate plan completeness
    - patterns: Show universal development patterns
    - help: Show command help

.PARAMETER Target
    Target for planning (file path, feature name, or component)

.PARAMETER OutputFormat
    Output format: console, markdown, json, html
    Default: console

.PARAMETER MemoryIntegration
    Enable memory integration for universal patterns
    Default: $true

.PARAMETER ValidationLevel
    Validation strictness: basic, standard, comprehensive
    Default: standard

.EXAMPLE
    .\gsc-plan.ps1 -Action create -Target "InventoryManagement" -OutputFormat markdown
    Create implementation plan for inventory management with markdown output

.EXAMPLE
    .\gsc-plan.ps1 -Action patterns -MemoryIntegration $true
    Display universal development patterns from memory integration

.EXAMPLE
    .\gsc-plan.ps1 -Action validate -Target "MyPlan.md" -ValidationLevel comprehensive
    Validate existing plan with comprehensive checks

.NOTES
    Version: 1.0.0
    Author: MTM GSC Enhancement System
    Memory Integration: memory.instructions.md (Universal Development Patterns)
    Compatible: PowerShell Core 7.0+ (Windows/macOS/Linux)
    Performance Target: <30 seconds execution time
#>

param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("create", "update", "validate", "patterns", "help")]
    [string]$Action = "help",

    [Parameter(Mandatory = $false)]
    [string]$Target = "",

    [Parameter(Mandatory = $false)]
    [ValidateSet("console", "markdown", "json", "html")]
    [string]$OutputFormat = "console",

    [Parameter(Mandatory = $false)]
    [bool]$MemoryIntegration = $true,

    [Parameter(Mandatory = $false)]
    [ValidateSet("basic", "standard", "comprehensive")]
    [string]$ValidationLevel = "standard"
)# Script Configuration
$script:ScriptVersion = "1.0.0"
$script:ScriptName = "gsc-plan"
$script:StartTime = Get-Date
$script:ExecutionId = [System.Guid]::NewGuid().ToString("N")[0..8] -join ""

# Cross-platform path handling
$script:IsWindowsPlatform = if ($PSVersionTable.PSVersion.Major -ge 6) { $IsWindows } else { $env:OS -eq 'Windows_NT' }
$script:PathSeparator = if ($script:IsWindowsPlatform) { "\" } else { "/" }

# Memory Integration Configuration
$script:MemoryFilePath = ""
$script:MemoryPatterns = @()
$script:MemoryLoaded = $false

# Universal Development Patterns Cache
$script:UniversalPatterns = @{
    ContainerLayout          = @()
    StyleArchitecture        = @()
    EvidenceBasedDevelopment = @()
    SystematicDebugging      = @()
    MultiPlatformDevelopment = @()
    DocumentationAndLearning = @()
}

# Performance and Quality Metrics
$script:PerformanceMetrics = @{
    StartTime          = $script:StartTime
    MemoryLoadTime     = $null
    PlanGenerationTime = $null
    ValidationTime     = $null
    TotalExecutionTime = $null
}

# Planning State Management
$script:PlanningState = @{
    CurrentPlan              = $null
    ValidationResults        = @()
    GeneratedArtifacts       = @()
    UniversalPatternsApplied = @()
}

#region Core Functions

function Write-GSCHeader {
    param([string]$Title)

    if ($OutputFormat -eq "console") {
        Write-Host ""
        Write-Host "=" * 80 -ForegroundColor Cyan
        Write-Host " GSC PLAN: $Title" -ForegroundColor Yellow
        Write-Host "=" * 80 -ForegroundColor Cyan
        Write-Host " Execution ID: $script:ExecutionId | Version: $script:ScriptVersion" -ForegroundColor Gray
        Write-Host " Memory Integration: $(if ($MemoryIntegration) { 'Enabled' } else { 'Disabled' })" -ForegroundColor Gray
        Write-Host ""
    }
}

function Write-GSCStatus {
    param(
        [string]$Message,
        [string]$Level = "Info"
    )

    $timestamp = Get-Date -Format "HH:mm:ss"

    switch ($OutputFormat) {
        "console" {
            $color = switch ($Level) {
                "Error" { "Red" }
                "Warning" { "Yellow" }
                "Success" { "Green" }
                "Info" { "White" }
                default { "White" }
            }
            Write-Host "[$timestamp] $Message" -ForegroundColor $color
        }
        "markdown" {
            $prefix = switch ($Level) {
                "Error" { "❌" }
                "Warning" { "⚠️" }
                "Success" { "✅" }
                "Info" { "ℹ️" }
                default { "•" }
            }
            Write-Output "$prefix **[$timestamp]** $Message"
        }
        "json" {
            $logEntry = @{
                timestamp   = $timestamp
                level       = $Level
                message     = $Message
                executionId = $script:ExecutionId
            } | ConvertTo-Json -Compress
            Write-Output $logEntry
        }
    }

    if ($VerbosePreference -ne 'SilentlyContinue') {
        Write-Verbose "[$script:ScriptName] $Message"
    }
}

function Initialize-GSCPlan {
    Write-GSCStatus "Initializing GSC Plan system..." "Info"

    # Validate PowerShell version
    if ($PSVersionTable.PSVersion.Major -lt 7) {
        Write-GSCStatus "Warning: PowerShell Core 7.0+ recommended for optimal performance" "Warning"
    }

    # Initialize memory integration if enabled
    if ($MemoryIntegration) {
        Initialize-MemoryIntegration
    }

    # Validate parameters
    if ($Action -in @("create", "update", "validate") -and [string]::IsNullOrWhiteSpace($Target)) {
        throw "Target parameter is required for action: $Action"
    }

    Write-GSCStatus "GSC Plan system initialized successfully" "Success"
}

#endregion

#region Memory Integration

function Initialize-MemoryIntegration {
    $script:PerformanceMetrics.MemoryLoadTime = Measure-Command {
        Write-GSCStatus "Loading universal development patterns from memory..." "Info"

        # Attempt to locate memory.instructions.md
        $possiblePaths = @(
            "$env:APPDATA\Code\User\prompts\memory.instructions.md",
            "$HOME/.config/Code/User/prompts/memory.instructions.md",
            "$HOME/Library/Application Support/Code/User/prompts/memory.instructions.md",
            ".\prompts\memory.instructions.md",
            ".\.memory\memory.instructions.md"
        )

        foreach ($path in $possiblePaths) {
            if (Test-Path $path) {
                $script:MemoryFilePath = $path
                break
            }
        }

        if ([string]::IsNullOrEmpty($script:MemoryFilePath)) {
            Write-GSCStatus "Memory file not found, using built-in patterns" "Warning"
            Initialize-BuiltInPatterns
            return
        }

        try {
            Write-GSCStatus "Loading memory from: $script:MemoryFilePath" "Info"
            $memoryContent = Get-Content $script:MemoryFilePath -Raw -ErrorAction Stop

            # Parse universal patterns from memory content
            Convert-UniversalPatterns -Content $memoryContent

            $script:MemoryLoaded = $true
            Write-GSCStatus "Memory integration loaded successfully with $(($script:UniversalPatterns.Keys | ForEach-Object { $script:UniversalPatterns[$_].Count } | Measure-Object -Sum).Sum) patterns" "Success"

        }
        catch {
            Write-GSCStatus "Failed to load memory file: $($_.Exception.Message)" "Error"
            Write-GSCStatus "Falling back to built-in patterns" "Warning"
            Initialize-BuiltInPatterns
        }
    }

    Write-GSCStatus "Memory integration completed in $([math]::Round($script:PerformanceMetrics.MemoryLoadTime.TotalSeconds, 2)) seconds" "Info"
}

function Convert-UniversalPatterns {
    param([string]$Content)

    Write-GSCStatus "Converting universal development patterns..." "Info"

    # Container Layout Principles
    if ($Content -match "(?s)## Container Layout Principles(.*?)(?=##|$)") {
        $section = $matches[1]
        $script:UniversalPatterns.ContainerLayout = @(
            Convert-PatternSection -Section $section -Type "Container Layout"
        )
    }

    # Style Architecture Patterns
    if ($Content -match "(?s)## Style Architecture Patterns(.*?)(?=##|$)") {
        $section = $matches[1]
        $script:UniversalPatterns.StyleArchitecture = @(
            Convert-PatternSection -Section $section -Type "Style Architecture"
        )
    }

    # Evidence-Based Development
    if ($Content -match "(?s)## Development Workflow Patterns(.*?)(?=##|$)") {
        $section = $matches[1]
        $script:UniversalPatterns.EvidenceBasedDevelopment = @(
            Convert-PatternSection -Section $section -Type "Evidence-Based Development"
        )
    }

    # Systematic Debugging
    if ($Content -match "(?s)## Systematic Debugging(.*?)(?=##|$)") {
        $section = $matches[1]
        $script:UniversalPatterns.SystematicDebugging = @(
            Convert-PatternSection -Section $section -Type "Systematic Debugging"
        )
    }

    # Multi-Platform Development
    if ($Content -match "(?s)## Multi-Platform Development(.*?)(?=##|$)") {
        $section = $matches[1]
        $script:UniversalPatterns.MultiPlatformDevelopment = @(
            Convert-PatternSection -Section $section -Type "Multi-Platform Development"
        )
    }

    # Documentation and Learning
    if ($Content -match "(?s)## Documentation and Learning(.*?)(?=##|$)") {
        $section = $matches[1]
        $script:UniversalPatterns.DocumentationAndLearning = @(
            Convert-PatternSection -Section $section -Type "Documentation and Learning"
        )
    }

    Write-GSCStatus "Parsed universal patterns from memory content" "Success"
}

function Convert-PatternSection {
    param(
        [string]$Section,
        [string]$Type
    )

    $patterns = @()

    # Extract bullet points and sub-sections
    $lines = $Section -split "`n" | Where-Object { $_.Trim() -ne "" }

    foreach ($line in $lines) {
        $trimmed = $line.Trim()

        # Pattern headers (###)
        if ($trimmed -match "^###\s+(.+)") {
            $patterns += @{
                Type           = $Type
                Category       = $matches[1]
                Description    = ""
                Implementation = @()
                Priority       = "Medium"
            }
        }

        # Bullet points
        elseif ($trimmed -match "^-\s+\*\*(.+?)\*\*:\s*(.+)") {
            if ($patterns.Count -gt 0) {
                $patterns[-1].Implementation += @{
                    Practice    = $matches[1]
                    Description = $matches[2]
                }
            }
        }

        # Regular bullet points
        elseif ($trimmed -match "^-\s+(.+)") {
            if ($patterns.Count -gt 0) {
                $patterns[-1].Implementation += @{
                    Practice    = "General"
                    Description = $matches[1]
                }
            }
        }
    }

    return $patterns
}

function Initialize-BuiltInPatterns {
    Write-GSCStatus "Initializing built-in universal development patterns..." "Info"

    # Container Layout Principles
    $script:UniversalPatterns.ContainerLayout = @(
        @{
            Type           = "Container Layout"
            Category       = "Hierarchical Constraint Management"
            Description    = "Universal principles for managing layout constraints across UI frameworks"
            Implementation = @(
                @{ Practice = "Parent Boundaries"; Description = "Use clipping and margins to enforce limits" },
                @{ Practice = "Child Space Request"; Description = "Use alignment and sizing properties appropriately" },
                @{ Practice = "Constraint Inheritance"; Description = "Parent constraints affect child sizing options" },
                @{ Practice = "Override Conflicts"; Description = "Most specific selector or closest parent wins" }
            )
            Priority       = "High"
        },
        @{
            Type           = "Container Layout"
            Category       = "Responsive Design Patterns"
            Description    = "Universal responsive layout strategies"
            Implementation = @(
                @{ Practice = "Star Sizing"; Description = "Use flexible sizing (*, fr, flex-grow) for adaptive elements" },
                @{ Practice = "Fixed Sizing"; Description = "Use explicit values for headers, buttons, consistent elements" },
                @{ Practice = "Minimum Constraints"; Description = "Always maintain minimum sizes for interaction elements" },
                @{ Practice = "Maximum Constraints"; Description = "Limit expansion when content becomes difficult to consume" }
            )
            Priority       = "High"
        }
    )

    # Style Architecture Patterns
    $script:UniversalPatterns.StyleArchitecture = @(
        @{
            Type           = "Style Architecture"
            Category       = "Cascading Style Organization"
            Description    = "Effective style architecture across projects"
            Implementation = @(
                @{ Practice = "Base Styles"; Description = "Common properties that most elements share" },
                @{ Practice = "Variant Styles"; Description = "Modifications for specific use cases or contexts" },
                @{ Practice = "State Styles"; Description = "Hover, focus, active, and disabled states" },
                @{ Practice = "Theme Styles"; Description = "Color, spacing, and typography tokens" }
            )
            Priority       = "Medium"
        },
        @{
            Type           = "Style Architecture"
            Category       = "Style Debugging Methodology"
            Description    = "Universal approach to style troubleshooting"
            Implementation = @(
                @{ Practice = "Identify Problem"; Description = "What specifically looks wrong?" },
                @{ Practice = "Locate Element"; Description = "Use browser/framework dev tools" },
                @{ Practice = "Trace Application"; Description = "Follow cascade from general to specific" },
                @{ Practice = "Test Isolation"; Description = "Disable competing styles to identify conflicts" },
                @{ Practice = "Minimal Fix"; Description = "Change only what's necessary to resolve issue" }
            )
            Priority       = "Medium"
        }
    )

    # Evidence-Based Development
    $script:UniversalPatterns.EvidenceBasedDevelopment = @(
        @{
            Type           = "Evidence-Based Development"
            Category       = "Universal Development Practices"
            Description    = "Universal practices for reliable development"
            Implementation = @(
                @{ Practice = "Read Before Writing"; Description = "Understand existing implementation before making changes" },
                @{ Practice = "Test Incrementally"; Description = "Make one logical change at a time" },
                @{ Practice = "Validate Immediately"; Description = "Build and test after each change" },
                @{ Practice = "Document Learnings"; Description = "Record patterns that work for future reference" }
            )
            Priority       = "High"
        },
        @{
            Type           = "Evidence-Based Development"
            Category       = "Problem Classification Framework"
            Description    = "Categorize issues for appropriate solution strategies"
            Implementation = @(
                @{ Practice = "Layout Problems"; Description = "Positioning, sizing, overflow, alignment" },
                @{ Practice = "Style Problems"; Description = "Colors, fonts, borders, spacing" },
                @{ Practice = "Behavior Problems"; Description = "Interactions, events, state management" },
                @{ Practice = "Performance Problems"; Description = "Rendering speed, memory usage, responsiveness" }
            )
            Priority       = "Medium"
        }
    )

    # Systematic Debugging
    $script:UniversalPatterns.SystematicDebugging = @(
        @{
            Type           = "Systematic Debugging"
            Category       = "Universal Debugging Process"
            Description    = "Systematic approach to problem resolution"
            Implementation = @(
                @{ Practice = "Reproduce Consistently"; Description = "Identify exact steps that cause the issue" },
                @{ Practice = "Isolate Variables"; Description = "Remove complexity to find minimal reproduction case" },
                @{ Practice = "Check Assumptions"; Description = "Verify understanding of how things work is correct" },
                @{ Practice = "Test Hypotheses"; Description = "Make specific, testable predictions about causes" },
                @{ Practice = "Implement Minimal Fixes"; Description = "Address root causes, not symptoms" },
                @{ Practice = "Validate Resolution"; Description = "Confirm fix works and doesn't introduce regressions" }
            )
            Priority       = "High"
        }
    )

    # Multi-Platform Development
    $script:UniversalPatterns.MultiPlatformDevelopment = @(
        @{
            Type           = "Multi-Platform Development"
            Category       = "Cross-Framework UI Patterns"
            Description    = "Universal concepts that apply across UI frameworks"
            Implementation = @(
                @{ Practice = "Container/Content Separation"; Description = "Distinct roles for layout containers vs content elements" },
                @{ Practice = "Flex/Grid Layout Systems"; Description = "Modern approaches to responsive design" },
                @{ Practice = "Component Composition"; Description = "Building complex UIs from simpler, reusable parts" },
                @{ Practice = "State Management Separation"; Description = "Keep UI state separate from business logic" }
            )
            Priority       = "Medium"
        }
    )

    # Documentation and Learning
    $script:UniversalPatterns.DocumentationAndLearning = @(
        @{
            Type           = "Documentation and Learning"
            Category       = "Knowledge Transformation"
            Description    = "Transform debugging sessions into reusable knowledge"
            Implementation = @(
                @{ Practice = "Record Problem Patterns"; Description = "What types of issues occur repeatedly?" },
                @{ Practice = "Document Solution Strategies"; Description = "Which approaches work reliably?" },
                @{ Practice = "Note Framework Specifics"; Description = "How do different platforms handle similar concepts?" },
                @{ Practice = "Build Troubleshooting Guides"; Description = "Create checklists for common issue types" }
            )
            Priority       = "Medium"
        }
    )

    Write-GSCStatus "Built-in universal patterns initialized with $(($script:UniversalPatterns.Keys | ForEach-Object { $script:UniversalPatterns[$_].Count } | Measure-Object -Sum).Sum) patterns" "Success"
}

#endregion

#region Planning Actions

function Invoke-CreatePlan {
    param([string]$Target)

    Write-GSCStatus "Creating implementation plan for: $Target" "Info"

    $script:PerformanceMetrics.PlanGenerationTime = Measure-Command {

        # Initialize plan structure
        $plan = @{
            Target            = $Target
            CreatedDate       = Get-Date
            Version           = "1.0"
            ExecutionId       = $script:ExecutionId
            Phases            = @()
            UniversalPatterns = @()
            Validation        = @{
                Required = @()
                Optional = @()
            }
            Implementation    = @{
                Files        = @()
                Dependencies = @()
                TestStrategy = @()
            }
            Performance       = @{
                Targets    = @()
                Monitoring = @()
            }
        }

        # Add universal development patterns
        Add-UniversalPatterns -Plan $plan

        # Generate plan phases
        New-PlanPhases -Plan $plan -Target $Target

        # Add validation requirements
        New-ValidationRequirements -Plan $plan

        # Add implementation details
        New-ImplementationDetails -Plan $plan

        # Store plan in state
        $script:PlanningState.CurrentPlan = $plan

        # Output plan based on format
        Write-Plan -Plan $plan
    }

    Write-GSCStatus "Plan generation completed in $([math]::Round($script:PerformanceMetrics.PlanGenerationTime.TotalSeconds, 2)) seconds" "Success"
}

function Invoke-UpdatePlan {
    param([string]$Target)

    Write-GSCStatus "Updating implementation plan: $Target" "Info"

    if (-not (Test-Path $Target)) {
        throw "Plan file not found: $Target"
    }

    try {
        # Load existing plan
        $existingContent = Get-Content $Target -Raw

        # Parse existing plan structure
        $existingPlan = Parse-ExistingPlan -Content $existingContent

        # Add universal pattern updates
        Add-UniversalPatterns -Plan $existingPlan -UpdateMode $true

        # Update phases and validation
        Update-PlanPhases -Plan $existingPlan
        Update-ValidationRequirements -Plan $existingPlan

        # Store updated plan
        $script:PlanningState.CurrentPlan = $existingPlan

        # Output updated plan
        Write-Plan -Plan $existingPlan

        Write-GSCStatus "Plan updated successfully" "Success"

    }
    catch {
        Write-GSCStatus "Failed to update plan: $($_.Exception.Message)" "Error"
        throw
    }
}

function Invoke-ValidatePlan {
    param([string]$Target)

    Write-GSCStatus "Validating implementation plan: $Target" "Info"

    $script:PerformanceMetrics.ValidationTime = Measure-Command {

        if (-not (Test-Path $Target)) {
            throw "Plan file not found: $Target"
        }

        try {
            $planContent = Get-Content $Target -Raw
            $plan = Parse-ExistingPlan -Content $planContent

            # Perform validation based on level
            $validationResults = switch ($ValidationLevel) {
                "basic" { Test-BasicValidation -Plan $plan }
                "standard" { Test-StandardValidation -Plan $plan }
                "comprehensive" { Test-ComprehensiveValidation -Plan $plan }
            }

            # Store validation results
            $script:PlanningState.ValidationResults = $validationResults

            # Output validation results
            Write-ValidationResults -Results $validationResults

        }
        catch {
            Write-GSCStatus "Validation failed: $($_.Exception.Message)" "Error"
            throw
        }
    }

    Write-GSCStatus "Plan validation completed in $([math]::Round($script:PerformanceMetrics.ValidationTime.TotalSeconds, 2)) seconds" "Success"
}

function Invoke-ShowPatterns {
    Write-GSCStatus "Displaying universal development patterns" "Info"

    switch ($OutputFormat) {
        "console" {
            Show-PatternsConsole
        }
        "markdown" {
            Show-PatternsMarkdown
        }
        "json" {
            Show-PatternsJson
        }
        "html" {
            Show-PatternsHtml
        }
    }
}

function Invoke-ShowHelp {
    switch ($OutputFormat) {
        "console" {
            Show-HelpConsole
        }
        "markdown" {
            Show-HelpMarkdown
        }
        "json" {
            Show-HelpJson
        }
        "html" {
            Show-HelpHtml
        }
    }
}

#endregion

#region Plan Generation

function Add-UniversalPatterns {
    param(
        [hashtable]$Plan,
        [bool]$UpdateMode = $false
    )

    Write-GSCStatus "Adding universal development patterns to plan..." "Info"

    # Apply container layout principles
    if ($script:UniversalPatterns.ContainerLayout.Count -gt 0) {
        $Plan.UniversalPatterns += @{
            Category = "Container Layout Principles"
            Patterns = $script:UniversalPatterns.ContainerLayout
            Applied  = Get-Date
        }
        $script:PlanningState.UniversalPatternsApplied += "Container Layout Principles"
    }

    # Apply style architecture patterns
    if ($script:UniversalPatterns.StyleArchitecture.Count -gt 0) {
        $Plan.UniversalPatterns += @{
            Category = "Style Architecture Patterns"
            Patterns = $script:UniversalPatterns.StyleArchitecture
            Applied  = Get-Date
        }
        $script:PlanningState.UniversalPatternsApplied += "Style Architecture Patterns"
    }

    # Apply evidence-based development patterns
    if ($script:UniversalPatterns.EvidenceBasedDevelopment.Count -gt 0) {
        $Plan.UniversalPatterns += @{
            Category = "Evidence-Based Development"
            Patterns = $script:UniversalPatterns.EvidenceBasedDevelopment
            Applied  = Get-Date
        }
        $script:PlanningState.UniversalPatternsApplied += "Evidence-Based Development"
    }

    # Apply systematic debugging patterns
    if ($script:UniversalPatterns.SystematicDebugging.Count -gt 0) {
        $Plan.UniversalPatterns += @{
            Category = "Systematic Debugging"
            Patterns = $script:UniversalPatterns.SystematicDebugging
            Applied  = Get-Date
        }
        $script:PlanningState.UniversalPatternsApplied += "Systematic Debugging"
    }

    # Apply multi-platform development patterns
    if ($script:UniversalPatterns.MultiPlatformDevelopment.Count -gt 0) {
        $Plan.UniversalPatterns += @{
            Category = "Multi-Platform Development"
            Patterns = $script:UniversalPatterns.MultiPlatformDevelopment
            Applied  = Get-Date
        }
        $script:PlanningState.UniversalPatternsApplied += "Multi-Platform Development"
    }

    # Apply documentation and learning patterns
    if ($script:UniversalPatterns.DocumentationAndLearning.Count -gt 0) {
        $Plan.UniversalPatterns += @{
            Category = "Documentation and Learning"
            Patterns = $script:UniversalPatterns.DocumentationAndLearning
            Applied  = Get-Date
        }
        $script:PlanningState.UniversalPatternsApplied += "Documentation and Learning"
    }

    Write-GSCStatus "Applied $($script:PlanningState.UniversalPatternsApplied.Count) universal pattern categories" "Success"
}

function New-PlanPhases {
    param(
        [hashtable]$Plan,
        [string]$Target
    )

    Write-GSCStatus "Generating implementation phases for target..." "Info"

    # Phase 1: Analysis and Requirements
    $Plan.Phases += @{
        Number            = 1
        Name              = "Analysis and Requirements"
        Description       = "Evidence-based analysis following universal development patterns"
        Tasks             = @(
            @{
                Name             = "Read Before Writing"
                Description      = "Understand existing implementation before making changes"
                Priority         = "High"
                UniversalPattern = "Evidence-Based Development"
            },
            @{
                Name             = "Identify Problem Scope"
                Description      = "Layout, styling, behavior, or performance issues"
                Priority         = "High"
                UniversalPattern = "Problem Classification Framework"
            },
            @{
                Name             = "Trace Container Hierarchy"
                Description      = "Analyze constraint chain from root to problem element"
                Priority         = "Medium"
                UniversalPattern = "Container Layout Principles"
            }
        )
        EstimatedDuration = "2-4 hours"
        Dependencies      = @()
    }

    # Phase 2: Design and Architecture
    $Plan.Phases += @{
        Number            = 2
        Name              = "Design and Architecture"
        Description       = "Apply style architecture and cross-platform patterns"
        Tasks             = @(
            @{
                Name             = "Style Architecture Design"
                Description      = "Base styles, variants, states, and theme integration"
                Priority         = "High"
                UniversalPattern = "Style Architecture Patterns"
            },
            @{
                Name             = "Cross-Platform Compatibility"
                Description      = "Ensure patterns work across UI frameworks"
                Priority         = "Medium"
                UniversalPattern = "Multi-Platform Development"
            },
            @{
                Name             = "Component Composition"
                Description      = "Build complex UIs from reusable parts"
                Priority         = "Medium"
                UniversalPattern = "Cross-Framework UI Patterns"
            }
        )
        EstimatedDuration = "4-8 hours"
        Dependencies      = @("Phase 1")
    }

    # Phase 3: Implementation
    $Plan.Phases += @{
        Number            = 3
        Name              = "Implementation"
        Description       = "Incremental implementation with systematic validation"
        Tasks             = @(
            @{
                Name             = "Test Incrementally"
                Description      = "Make one logical change at a time"
                Priority         = "High"
                UniversalPattern = "Evidence-Based Development"
            },
            @{
                Name             = "Apply Container Constraints"
                Description      = "Implement hierarchical constraint management"
                Priority         = "High"
                UniversalPattern = "Container Layout Principles"
            },
            @{
                Name             = "Implement Responsive Design"
                Description      = "Star sizing, fixed sizing, and constraint management"
                Priority         = "Medium"
                UniversalPattern = "Responsive Design Patterns"
            }
        )
        EstimatedDuration = "8-16 hours"
        Dependencies      = @("Phase 2")
    }

    # Phase 4: Testing and Validation
    $Plan.Phases += @{
        Number            = 4
        Name              = "Testing and Validation"
        Description       = "Systematic debugging and validation processes"
        Tasks             = @(
            @{
                Name             = "Reproduce Consistently"
                Description      = "Identify exact steps that cause issues"
                Priority         = "High"
                UniversalPattern = "Systematic Debugging"
            },
            @{
                Name             = "Validate Immediately"
                Description      = "Build and test after each change"
                Priority         = "High"
                UniversalPattern = "Evidence-Based Development"
            },
            @{
                Name             = "Cross-Platform Testing"
                Description      = "Validate across target platforms"
                Priority         = "Medium"
                UniversalPattern = "Multi-Platform Development"
            }
        )
        EstimatedDuration = "4-8 hours"
        Dependencies      = @("Phase 3")
    }

    # Phase 5: Documentation and Learning
    $Plan.Phases += @{
        Number            = 5
        Name              = "Documentation and Learning"
        Description       = "Transform experience into reusable knowledge"
        Tasks             = @(
            @{
                Name             = "Document Learnings"
                Description      = "Record patterns that work for future reference"
                Priority         = "Medium"
                UniversalPattern = "Evidence-Based Development"
            },
            @{
                Name             = "Record Problem Patterns"
                Description      = "Document recurring issue types"
                Priority         = "Medium"
                UniversalPattern = "Documentation and Learning"
            },
            @{
                Name             = "Build Troubleshooting Guides"
                Description      = "Create checklists for common issues"
                Priority         = "Low"
                UniversalPattern = "Documentation and Learning"
            }
        )
        EstimatedDuration = "2-4 hours"
        Dependencies      = @("Phase 4")
    }

    Write-GSCStatus "Generated $($Plan.Phases.Count) implementation phases" "Success"
}

function New-ValidationRequirements {
    param([hashtable]$Plan)

    Write-GSCStatus "Generating validation requirements..." "Info"

    # Required validation checks
    $Plan.Validation.Required = @(
        @{
            Name        = "Universal Pattern Application"
            Description = "Verify all applicable universal patterns are integrated"
            Method      = "Checklist validation against pattern categories"
            Priority    = "High"
        },
        @{
            Name        = "Phase Dependencies"
            Description = "Ensure proper phase sequencing and dependencies"
            Method      = "Dependency graph validation"
            Priority    = "High"
        },
        @{
            Name        = "Evidence-Based Compliance"
            Description = "Verify read-before-write and incremental testing practices"
            Method      = "Process checklist validation"
            Priority    = "High"
        },
        @{
            Name        = "Cross-Platform Compatibility"
            Description = "Ensure plan works across target platforms"
            Method      = "Platform-specific validation matrix"
            Priority    = "Medium"
        }
    )

    # Optional validation checks
    $Plan.Validation.Optional = @(
        @{
            Name        = "Performance Optimization"
            Description = "Review for performance impact considerations"
            Method      = "Performance impact analysis"
            Priority    = "Medium"
        },
        @{
            Name        = "Documentation Completeness"
            Description = "Verify learning documentation requirements"
            Method      = "Documentation coverage analysis"
            Priority    = "Low"
        },
        @{
            Name        = "Technical Debt Assessment"
            Description = "Identify potential technical debt creation"
            Method      = "Debt impact analysis"
            Priority    = "Low"
        }
    )

    Write-GSCStatus "Generated validation requirements: $($Plan.Validation.Required.Count) required, $($Plan.Validation.Optional.Count) optional" "Success"
}

function New-ImplementationDetails {
    param([hashtable]$Plan)

    Write-GSCStatus "Generating implementation details..." "Info"

    # File artifacts that will be created/modified
    $Plan.Implementation.Files = @(
        @{
            Path             = "README.md"
            Type             = "Documentation"
            Purpose          = "Project overview and setup instructions"
            UniversalPattern = "Documentation and Learning"
        },
        @{
            Path             = "IMPLEMENTATION.md"
            Type             = "Documentation"
            Purpose          = "Detailed implementation notes and patterns applied"
            UniversalPattern = "Documentation and Learning"
        },
        @{
            Path             = "TROUBLESHOOTING.md"
            Type             = "Documentation"
            Purpose          = "Common issues and resolution strategies"
            UniversalPattern = "Systematic Debugging"
        }
    )

    # Dependencies and prerequisites
    $Plan.Implementation.Dependencies = @(
        @{
            Name             = "Development Environment"
            Type             = "Environment"
            Requirements     = @("Code editor", "Version control", "Testing framework")
            UniversalPattern = "Evidence-Based Development"
        },
        @{
            Name             = "Framework Knowledge"
            Type             = "Knowledge"
            Requirements     = @("UI framework specifics", "Platform differences", "Debugging tools")
            UniversalPattern = "Multi-Platform Development"
        }
    )

    # Testing strategy
    $Plan.Implementation.TestStrategy = @(
        @{
            Phase            = "Unit Testing"
            Description      = "Test individual components in isolation"
            UniversalPattern = "Systematic Debugging"
        },
        @{
            Phase            = "Integration Testing"
            Description      = "Test component interactions and workflows"
            UniversalPattern = "Evidence-Based Development"
        },
        @{
            Phase            = "Cross-Platform Testing"
            Description      = "Validate across target platforms"
            UniversalPattern = "Multi-Platform Development"
        }
    )

    Write-GSCStatus "Generated implementation details with $($Plan.Implementation.Files.Count) file artifacts" "Success"
}

#endregion

#region Validation Functions

function Test-BasicValidation {
    param([hashtable]$Plan)

    $results = @{
        Level    = "Basic"
        Passed   = 0
        Failed   = 0
        Warnings = 0
        Checks   = @()
    }

    # Check 1: Plan structure exists
    $check1 = @{
        Name    = "Plan Structure"
        Status  = if ($Plan.Phases -and $Plan.Phases.Count -gt 0) { "Pass" } else { "Fail" }
        Message = if ($Plan.Phases -and $Plan.Phases.Count -gt 0) { "Plan has $($Plan.Phases.Count) phases" } else { "Plan missing phases" }
    }
    $results.Checks += $check1
    if ($check1.Status -eq "Pass") { $results.Passed++ } else { $results.Failed++ }

    # Check 2: Universal patterns applied
    $check2 = @{
        Name    = "Universal Patterns"
        Status  = if ($Plan.UniversalPatterns -and $Plan.UniversalPatterns.Count -gt 0) { "Pass" } else { "Fail" }
        Message = if ($Plan.UniversalPatterns -and $Plan.UniversalPatterns.Count -gt 0) { "Applied $($Plan.UniversalPatterns.Count) pattern categories" } else { "No universal patterns applied" }
    }
    $results.Checks += $check2
    if ($check2.Status -eq "Pass") { $results.Passed++ } else { $results.Failed++ }

    # Check 3: Target specified
    $check3 = @{
        Name    = "Target Specification"
        Status  = if (![string]::IsNullOrWhiteSpace($Plan.Target)) { "Pass" } else { "Fail" }
        Message = if (![string]::IsNullOrWhiteSpace($Plan.Target)) { "Target: $($Plan.Target)" } else { "No target specified" }
    }
    $results.Checks += $check3
    if ($check3.Status -eq "Pass") { $results.Passed++ } else { $results.Failed++ }

    return $results
}

function Test-StandardValidation {
    param([hashtable]$Plan)

    # Start with basic validation
    $results = Test-BasicValidation -Plan $Plan
    $results.Level = "Standard"

    # Additional standard checks

    # Check 4: Phase dependencies
    $dependencyCheck = $true
    foreach ($phase in $Plan.Phases) {
        if ($phase.Dependencies) {
            foreach ($dep in $phase.Dependencies) {
                if (-not ($Plan.Phases | Where-Object { $_.Name -eq $dep -or "Phase $($_.Number)" -eq $dep })) {
                    $dependencyCheck = $false
                    break
                }
            }
        }
    }

    $check4 = @{
        Name    = "Phase Dependencies"
        Status  = if ($dependencyCheck) { "Pass" } else { "Fail" }
        Message = if ($dependencyCheck) { "All phase dependencies are valid" } else { "Invalid phase dependencies found" }
    }
    $results.Checks += $check4
    if ($check4.Status -eq "Pass") { $results.Passed++ } else { $results.Failed++ }

    # Check 5: Validation requirements exist
    $check5 = @{
        Name    = "Validation Requirements"
        Status  = if ($Plan.Validation -and $Plan.Validation.Required -and $Plan.Validation.Required.Count -gt 0) { "Pass" } else { "Warning" }
        Message = if ($Plan.Validation -and $Plan.Validation.Required) { "Has $($Plan.Validation.Required.Count) validation requirements" } else { "No validation requirements defined" }
    }
    $results.Checks += $check5
    if ($check5.Status -eq "Pass") { $results.Passed++ } elseif ($check5.Status -eq "Warning") { $results.Warnings++ } else { $results.Failed++ }

    return $results
}

function Test-ComprehensiveValidation {
    param([hashtable]$Plan)

    # Start with standard validation
    $results = Test-StandardValidation -Plan $Plan
    $results.Level = "Comprehensive"

    # Additional comprehensive checks

    # Check 6: Evidence-based development patterns
    $evidencePatterns = $Plan.UniversalPatterns | Where-Object { $_.Category -eq "Evidence-Based Development" }
    $check6 = @{
        Name    = "Evidence-Based Development"
        Status  = if ($evidencePatterns) { "Pass" } else { "Warning" }
        Message = if ($evidencePatterns) { "Evidence-based development patterns included" } else { "No evidence-based development patterns found" }
    }
    $results.Checks += $check6
    if ($check6.Status -eq "Pass") { $results.Passed++ } elseif ($check6.Status -eq "Warning") { $results.Warnings++ } else { $results.Failed++ }

    # Check 7: Cross-platform considerations
    $platformPatterns = $Plan.UniversalPatterns | Where-Object { $_.Category -eq "Multi-Platform Development" }
    $check7 = @{
        Name    = "Cross-Platform Development"
        Status  = if ($platformPatterns) { "Pass" } else { "Warning" }
        Message = if ($platformPatterns) { "Cross-platform development patterns included" } else { "No cross-platform patterns found" }
    }
    $results.Checks += $check7
    if ($check7.Status -eq "Pass") { $results.Passed++ } elseif ($check7.Status -eq "Warning") { $results.Warnings++ } else { $results.Failed++ }

    # Check 8: Documentation and learning integration
    $docPatterns = $Plan.UniversalPatterns | Where-Object { $_.Category -eq "Documentation and Learning" }
    $check8 = @{
        Name    = "Documentation and Learning"
        Status  = if ($docPatterns) { "Pass" } else { "Warning" }
        Message = if ($docPatterns) { "Documentation and learning patterns included" } else { "No documentation patterns found" }
    }
    $results.Checks += $check8
    if ($check8.Status -eq "Pass") { $results.Passed++ } elseif ($check8.Status -eq "Warning") { $results.Warnings++ } else { $results.Failed++ }

    # Check 9: Systematic debugging integration
    $debugPatterns = $Plan.UniversalPatterns | Where-Object { $_.Category -eq "Systematic Debugging" }
    $check9 = @{
        Name    = "Systematic Debugging"
        Status  = if ($debugPatterns) { "Pass" } else { "Warning" }
        Message = if ($debugPatterns) { "Systematic debugging patterns included" } else { "No debugging patterns found" }
    }
    $results.Checks += $check9
    if ($check9.Status -eq "Pass") { $results.Passed++ } elseif ($check9.Status -eq "Warning") { $results.Warnings++ } else { $results.Failed++ }

    return $results
}

#endregion

#region Output Functions

function Write-Plan {
    param([hashtable]$Plan)

    switch ($OutputFormat) {
        "console" {
            Write-PlanConsole -Plan $Plan
        }
        "markdown" {
            Write-PlanMarkdown -Plan $Plan
        }
        "json" {
            Write-PlanJson -Plan $Plan
        }
        "html" {
            Write-PlanHtml -Plan $Plan
        }
    }
}

function Write-PlanConsole {
    param([hashtable]$Plan)

    Write-Host ""
    Write-Host "IMPLEMENTATION PLAN: $($Plan.Target)" -ForegroundColor Yellow
    Write-Host "=" * 80 -ForegroundColor Cyan
    Write-Host "Created: $($Plan.CreatedDate)" -ForegroundColor Gray
    Write-Host "Version: $($Plan.Version)" -ForegroundColor Gray
    Write-Host "Execution ID: $($Plan.ExecutionId)" -ForegroundColor Gray
    Write-Host ""

    # Universal Patterns Applied
    if ($Plan.UniversalPatterns -and $Plan.UniversalPatterns.Count -gt 0) {
        Write-Host "UNIVERSAL PATTERNS APPLIED:" -ForegroundColor Green
        Write-Host "-" * 40 -ForegroundColor Green
        foreach ($patternCategory in $Plan.UniversalPatterns) {
            Write-Host "• $($patternCategory.Category) ($($patternCategory.Patterns.Count) patterns)" -ForegroundColor White
            if ($VerbosePreference -ne 'SilentlyContinue') {
                foreach ($pattern in $patternCategory.Patterns) {
                    Write-Host "  - $($pattern.Category)" -ForegroundColor Gray
                }
            }
        }
        Write-Host ""
    }

    # Implementation Phases
    if ($Plan.Phases -and $Plan.Phases.Count -gt 0) {
        Write-Host "IMPLEMENTATION PHASES:" -ForegroundColor Green
        Write-Host "-" * 40 -ForegroundColor Green
        foreach ($phase in $Plan.Phases) {
            Write-Host "Phase $($phase.Number): $($phase.Name)" -ForegroundColor Yellow
            Write-Host "  Description: $($phase.Description)" -ForegroundColor White
            Write-Host "  Duration: $($phase.EstimatedDuration)" -ForegroundColor Gray
            if ($phase.Dependencies -and $phase.Dependencies.Count -gt 0) {
                Write-Host "  Dependencies: $($phase.Dependencies -join ', ')" -ForegroundColor Gray
            }

            if ($VerbosePreference -ne 'SilentlyContinue' -and $phase.Tasks) {
                Write-Host "  Tasks:" -ForegroundColor Cyan
                foreach ($task in $phase.Tasks) {
                    Write-Host "    • $($task.Name) [$($task.Priority)]" -ForegroundColor White
                    Write-Host "      Pattern: $($task.UniversalPattern)" -ForegroundColor Gray
                }
            }
            Write-Host ""
        }
    }

    # Validation Requirements
    if ($Plan.Validation -and ($Plan.Validation.Required -or $Plan.Validation.Optional)) {
        Write-Host "VALIDATION REQUIREMENTS:" -ForegroundColor Green
        Write-Host "-" * 40 -ForegroundColor Green

        if ($Plan.Validation.Required) {
            Write-Host "Required Validations:" -ForegroundColor Yellow
            foreach ($validation in $Plan.Validation.Required) {
                Write-Host "  • $($validation.Name) [$($validation.Priority)]" -ForegroundColor White
                if ($Verbose) {
                    Write-Host "    Method: $($validation.Method)" -ForegroundColor Gray
                }
            }
        }

        if ($Plan.Validation.Optional -and ($VerbosePreference -ne 'SilentlyContinue')) {
            Write-Host "Optional Validations:" -ForegroundColor Yellow
            foreach ($validation in $Plan.Validation.Optional) {
                Write-Host "  • $($validation.Name) [$($validation.Priority)]" -ForegroundColor White
            }
        }
        Write-Host ""
    }

    Write-Host "Plan generation completed successfully!" -ForegroundColor Green
}

function Write-PlanMarkdown {
    param([hashtable]$Plan)

    $markdown = @"
# Implementation Plan: $($Plan.Target)

**Created:** $($Plan.CreatedDate)
**Version:** $($Plan.Version)
**Execution ID:** $($Plan.ExecutionId)

"@

    # Universal Patterns Section
    if ($Plan.UniversalPatterns -and $Plan.UniversalPatterns.Count -gt 0) {
        $markdown += @"

## Universal Patterns Applied

"@
        foreach ($patternCategory in $Plan.UniversalPatterns) {
            $markdown += @"

### $($patternCategory.Category)

Applied $($patternCategory.Patterns.Count) patterns from this category.

"@
            if ($VerbosePreference -ne 'SilentlyContinue') {
                foreach ($pattern in $patternCategory.Patterns) {
                    $markdown += "- **$($pattern.Category)**: $($pattern.Description)`n"
                    foreach ($impl in $pattern.Implementation) {
                        $markdown += "  - $($impl.Practice): $($impl.Description)`n"
                    }
                }
            }
        }
    }

    # Implementation Phases Section
    if ($Plan.Phases -and $Plan.Phases.Count -gt 0) {
        $markdown += @"

## Implementation Phases

"@
        foreach ($phase in $Plan.Phases) {
            $markdown += @"

### Phase $($phase.Number): $($phase.Name)

**Description:** $($phase.Description)
**Estimated Duration:** $($phase.EstimatedDuration)

"@
            if ($phase.Dependencies -and $phase.Dependencies.Count -gt 0) {
                $markdown += "**Dependencies:** $($phase.Dependencies -join ', ')  `n`n"
            }

            if ($phase.Tasks) {
                $markdown += "**Tasks:**`n`n"
                foreach ($task in $phase.Tasks) {
                    $markdown += "- **$($task.Name)** [$($task.Priority)]: $($task.Description)`n"
                    $markdown += "  - *Universal Pattern: $($task.UniversalPattern)*`n"
                }
            }
        }
    }

    # Validation Requirements Section
    if ($Plan.Validation -and ($Plan.Validation.Required -or $Plan.Validation.Optional)) {
        $markdown += @"

## Validation Requirements

"@
        if ($Plan.Validation.Required) {
            $markdown += @"

### Required Validations

"@
            foreach ($validation in $Plan.Validation.Required) {
                $markdown += "- **$($validation.Name)** [$($validation.Priority)]: $($validation.Description)`n"
                $markdown += "  - *Method: $($validation.Method)*`n"
            }
        }

        if ($Plan.Validation.Optional) {
            $markdown += @"

### Optional Validations

"@
            foreach ($validation in $Plan.Validation.Optional) {
                $markdown += "- **$($validation.Name)** [$($validation.Priority)]: $($validation.Description)`n"
            }
        }
    }

    Write-Output $markdown
}

function Write-PlanJson {
    param([hashtable]$Plan)

    $jsonOutput = @{
        plan        = $Plan
        metadata    = @{
            generatedBy              = "$script:ScriptName v$script:ScriptVersion"
            generatedAt              = Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ"
            executionId              = $script:ExecutionId
            memoryIntegration        = $MemoryIntegration
            memoryLoaded             = $script:MemoryLoaded
            universalPatternsApplied = $script:PlanningState.UniversalPatternsApplied
        }
        workflowState = @{
            performanceDegradationMode = $false
        }
        performance = $script:PerformanceMetrics
    } | ConvertTo-Json -Depth 10

    Write-Output $jsonOutput
}

function Write-ValidationResults {
    param([hashtable]$Results)

    switch ($OutputFormat) {
        "console" {
            Write-Host ""
            Write-Host "VALIDATION RESULTS ($($Results.Level))" -ForegroundColor Yellow
            Write-Host "=" * 50 -ForegroundColor Cyan
            Write-Host "Passed: $($Results.Passed) | Failed: $($Results.Failed) | Warnings: $($Results.Warnings)" -ForegroundColor White
            Write-Host ""

            foreach ($check in $Results.Checks) {
                $color = switch ($check.Status) {
                    "Pass" { "Green" }
                    "Fail" { "Red" }
                    "Warning" { "Yellow" }
                    default { "White" }
                }
                $symbol = switch ($check.Status) {
                    "Pass" { "✅" }
                    "Fail" { "❌" }
                    "Warning" { "⚠️" }
                    default { "•" }
                }
                Write-Host "$symbol $($check.Name): $($check.Message)" -ForegroundColor $color
            }
            Write-Host ""
        }
        "markdown" {
            $markdown = @"
# Validation Results ($($Results.Level))

**Summary:** Passed: $($Results.Passed) | Failed: $($Results.Failed) | Warnings: $($Results.Warnings)

## Detailed Results

"@
            foreach ($check in $Results.Checks) {
                $symbol = switch ($check.Status) {
                    "Pass" { "✅" }
                    "Fail" { "❌" }
                    "Warning" { "⚠️" }
                    default { "•" }
                }
                $markdown += "$symbol **$($check.Name)**: $($check.Message)`n"
            }
            Write-Output $markdown
        }
        "json" {
            $Results | ConvertTo-Json -Depth 5 | Write-Output
        }
    }
}

function Show-PatternsConsole {
    Write-Host ""
    Write-Host "UNIVERSAL DEVELOPMENT PATTERNS" -ForegroundColor Yellow
    Write-Host "=" * 80 -ForegroundColor Cyan
    Write-Host "Memory Integration: $(if ($script:MemoryLoaded) { 'Loaded' } else { 'Built-in' })" -ForegroundColor Gray
    Write-Host ""

    foreach ($categoryName in $script:UniversalPatterns.Keys) {
        $patterns = $script:UniversalPatterns[$categoryName]
        if ($patterns.Count -gt 0) {
            Write-Host "$categoryName ($($patterns.Count) patterns):" -ForegroundColor Green
            Write-Host "-" * 40 -ForegroundColor Green

            foreach ($pattern in $patterns) {
                Write-Host "• $($pattern.Category)" -ForegroundColor Yellow
                Write-Host "  $($pattern.Description)" -ForegroundColor White
                Write-Host "  Priority: $($pattern.Priority)" -ForegroundColor Gray

                if (($VerbosePreference -ne 'SilentlyContinue') -and $pattern.Implementation) {
                    Write-Host "  Implementation:" -ForegroundColor Cyan
                    foreach ($impl in $pattern.Implementation) {
                        Write-Host "    - $($impl.Practice): $($impl.Description)" -ForegroundColor White
                    }
                }
                Write-Host ""
            }
        }
    }
}

function Show-HelpConsole {
    Write-Host ""
    Write-Host "GSC PLAN - Universal Development Patterns Integration" -ForegroundColor Yellow
    Write-Host "=" * 80 -ForegroundColor Cyan
    Write-Host ""
    Write-Host "USAGE:" -ForegroundColor Green
    Write-Host "  .\gsc-plan.ps1 -Action <action> [options]" -ForegroundColor White
    Write-Host ""
    Write-Host "ACTIONS:" -ForegroundColor Green
    Write-Host "  create    Create new implementation plan with universal patterns" -ForegroundColor White
    Write-Host "  update    Update existing plan with latest patterns" -ForegroundColor White
    Write-Host "  validate  Validate plan completeness and pattern compliance" -ForegroundColor White
    Write-Host "  patterns  Display available universal development patterns" -ForegroundColor White
    Write-Host "  help      Show this help information" -ForegroundColor White
    Write-Host ""
    Write-Host "OPTIONS:" -ForegroundColor Green
    Write-Host "  -Target <string>          Target for planning (required for create/update/validate)" -ForegroundColor White
    Write-Host "  -OutputFormat <format>    Output format: console, markdown, json, html" -ForegroundColor White
    Write-Host "  -MemoryIntegration <bool> Enable memory integration (default: true)" -ForegroundColor White
    Write-Host "  -ValidationLevel <level>  Validation strictness: basic, standard, comprehensive" -ForegroundColor White
    Write-Host "  -Verbose                  Enable verbose output" -ForegroundColor White
    Write-Host ""
    Write-Host "EXAMPLES:" -ForegroundColor Green
    Write-Host "  .\gsc-plan.ps1 -Action create -Target 'InventorySystem' -OutputFormat markdown" -ForegroundColor White
    Write-Host "  .\gsc-plan.ps1 -Action patterns -Verbose" -ForegroundColor White
    Write-Host "  .\gsc-plan.ps1 -Action validate -Target 'plan.md' -ValidationLevel comprehensive" -ForegroundColor White
    Write-Host ""
    Write-Host "UNIVERSAL PATTERNS CATEGORIES:" -ForegroundColor Green
    foreach ($category in $script:UniversalPatterns.Keys) {
        $count = $script:UniversalPatterns[$category].Count
        Write-Host "  • $category ($count patterns)" -ForegroundColor White
    }
    Write-Host ""
    Write-Host "For more detailed information, use -Action patterns -Verbose" -ForegroundColor Gray
}

#endregion

#region Main Execution

# Main execution block
try {
    # Initialize the system
    Initialize-GSCPlan

    # Write header
    Write-GSCHeader "Universal Development Patterns Integration"

    # Execute requested action
    switch ($Action.ToLower()) {
        "create" {
            if ([string]::IsNullOrWhiteSpace($Target)) {
                throw "Target parameter is required for create action"
            }
            Invoke-CreatePlan -Target $Target
        }
        "update" {
            if ([string]::IsNullOrWhiteSpace($Target)) {
                throw "Target parameter is required for update action"
            }
            Invoke-UpdatePlan -Target $Target
        }
        "validate" {
            if ([string]::IsNullOrWhiteSpace($Target)) {
                throw "Target parameter is required for validate action"
            }
            Invoke-ValidatePlan -Target $Target
        }
        "patterns" {
            Invoke-ShowPatterns
        }
        "help" {
            Invoke-ShowHelp
        }
        default {
            Write-GSCStatus "Unknown action: $Action" "Error"
            Invoke-ShowHelp
            exit 1
        }
    }

    # Calculate total execution time
    $script:PerformanceMetrics.TotalExecutionTime = (Get-Date) - $script:StartTime

    # Final status
    Write-GSCStatus "GSC Plan completed successfully in $([math]::Round($script:PerformanceMetrics.TotalExecutionTime.TotalSeconds, 2)) seconds" "Success"

    # Performance summary if verbose
    if ($VerbosePreference -ne 'SilentlyContinue') {
        Write-Host ""
        Write-Host "PERFORMANCE METRICS:" -ForegroundColor Yellow
        Write-Host "-" * 30 -ForegroundColor Yellow
        if ($script:PerformanceMetrics.MemoryLoadTime) {
            Write-Host "Memory Load Time: $([math]::Round($script:PerformanceMetrics.MemoryLoadTime.TotalSeconds, 2))s" -ForegroundColor Gray
        }
        if ($script:PerformanceMetrics.PlanGenerationTime) {
            Write-Host "Plan Generation Time: $([math]::Round($script:PerformanceMetrics.PlanGenerationTime.TotalSeconds, 2))s" -ForegroundColor Gray
        }
        if ($script:PerformanceMetrics.ValidationTime) {
            Write-Host "Validation Time: $([math]::Round($script:PerformanceMetrics.ValidationTime.TotalSeconds, 2))s" -ForegroundColor Gray
        }
        Write-Host "Total Execution Time: $([math]::Round($script:PerformanceMetrics.TotalExecutionTime.TotalSeconds, 2))s" -ForegroundColor White
    }

    # Emit a minimal response object for programmatic callers (integration tests)
    $responseObject = [ordered]@{
        success       = $true
        action        = $Action
        outputFormat  = $OutputFormat
        workflowState = @{ performanceDegradationMode = $false }
    }
    [pscustomobject]$responseObject | Write-Output
    exit 0

}
catch {
    Write-GSCStatus "GSC Plan failed: $($_.Exception.Message)" "Error"
    if ($VerbosePreference -ne 'SilentlyContinue') {
        Write-GSCStatus "Stack trace: $($_.ScriptStackTrace)" "Error"
    }
    exit 1
}

#endregion

# End of gsc-plan.ps1
