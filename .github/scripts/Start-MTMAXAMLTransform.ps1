#!/usr/bin/env pwsh
<#
.SYNOPSIS
Start MTM AXAML StyleSystem Transformation with @Agent guided workflow

.DESCRIPTION
This script provides an interactive guided workflow for transforming AXAML files to use
MTM Theme V2 + StyleSystem. It works collaboratively with @Agent to execute each phase
of the transformation with confirmation and validation at each step.

.PARAMETER TargetFile
The AXAML file to transform (without .axaml extension)

.PARAMETER SkipValidation
Skip initial validation checks (for debugging)

.PARAMETER AutoConfirm
Automatically confirm each step (for batch processing)

.EXAMPLE
.\Start-MTMAXAMLTransform.ps1 "RemoveTabView"

.EXAMPLE
.\Start-MTMAXAMLTransform.ps1 "TransferTabView" "Remove CustomDataGrid dependency and integrate EditInventoryView"

.EXAMPLE
.\Start-MTMAXAMLTransform.ps1 -TargetFile "TransferTabView" -SkipValidation

.NOTES
This script is designed to work with @Agent for collaborative AXAML transformation.
It creates comprehensive tracking files and guides through each phase systematically.
#>

[CmdletBinding()]
param(
  [Parameter(Mandatory = $true, Position = 0, HelpMessage = "Target AXAML file name (without extension)")]
  [string]$TargetFile,

  [Parameter(Mandatory = $false, Position = 1, HelpMessage = "Additional requirements or customizations")]
  [string]$AdditionalRequirements = "",

  [Parameter(Mandatory = $false)]
  [switch]$SkipValidation,

  [Parameter(Mandatory = $false)]
  [switch]$AutoConfirm
)

# Configuration
$script:Config = @{
  RootPath       = "c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia"
  GitHubPath     = ".github"
  TrackingPath   = ".copilot-tracking"
  DateStamp      = Get-Date -Format "yyyyMMdd"
  TimeStamp      = Get-Date -Format "HH:mm:ss"
  TargetFileName = $TargetFile.ToLower()
}

# Colors for output
$script:Colors = @{
  Header  = "Cyan"
  Success = "Green"
  Warning = "Yellow"
  Error   = "Red"
  Info    = "Blue"
  Prompt  = "Magenta"
  Code    = "Gray"
}

#region Helper Functions

function Write-Header {
  param([string]$Title, [string]$SubTitle = "")

  Write-Host ""
  Write-Host "=" * 80 -ForegroundColor $script:Colors.Header
  Write-Host " $Title" -ForegroundColor $script:Colors.Header
  if ($SubTitle) {
    Write-Host " $SubTitle" -ForegroundColor $script:Colors.Info
  }
  Write-Host "=" * 80 -ForegroundColor $script:Colors.Header
  Write-Host ""
}

function Write-Step {
  param([string]$StepNumber, [string]$Description)

  Write-Host ""
  Write-Host "🔸 Step $StepNumber`: $Description" -ForegroundColor $script:Colors.Info
  Write-Host "-" * 60 -ForegroundColor $script:Colors.Info
}

function Write-Success {
  param([string]$Message)
  Write-Host "✅ $Message" -ForegroundColor $script:Colors.Success
}

function Write-Warning {
  param([string]$Message)
  Write-Host "⚠️  $Message" -ForegroundColor $script:Colors.Warning
}

function Write-Error {
  param([string]$Message)
  Write-Host "❌ $Message" -ForegroundColor $script:Colors.Error
}

function Write-Info {
  param([string]$Message)
  Write-Host "ℹ️  $Message" -ForegroundColor $script:Colors.Info
}

function Request-AgentConfirmation {
  param(
    [string]$Question,
    [string]$Context = "",
    [string[]]$Options = @("Yes", "No"),
    [string]$DefaultOption = "Yes"
  )

  if ($AutoConfirm) {
    Write-Info "Auto-confirming: $Question"
    return $DefaultOption
  }

  Write-Host ""
  Write-Host "🤖 @Agent Collaboration Required" -ForegroundColor $script:Colors.Prompt
  Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor $script:Colors.Prompt

  if ($Context) {
    Write-Host "Context: $Context" -ForegroundColor $script:Colors.Info
    Write-Host ""
  }

  Write-Host "Question: $Question" -ForegroundColor $script:Colors.Prompt
  Write-Host ""

  $optionText = ($Options -join " / ")
  $response = Read-Host "Please respond [$optionText] (default: $DefaultOption)"

  if ([string]::IsNullOrWhiteSpace($response)) {
    return $DefaultOption
  }

  return $response
}

function Test-Prerequisites {
  Write-Header "Prerequisites Validation" "Ensuring environment is ready for transformation"

  $allValid = $true

  # Test 1: Root directory exists
  Write-Step "1" "Validating project root directory"
  if (Test-Path $script:Config.RootPath) {
    Write-Success "Project root found: $($script:Config.RootPath)"
  }
  else {
    Write-Error "Project root not found: $($script:Config.RootPath)"
    $allValid = $false
  }

  # Test 2: Target AXAML file exists
  Write-Step "2" "Validating target AXAML file"
  $targetFilePath = Join-Path $script:Config.RootPath "Views" "$TargetFile.axaml"
  if (Test-Path $targetFilePath) {
    Write-Success "Target file found: $targetFilePath"
  }
  else {
    Write-Error "Target file not found: $targetFilePath"
    $allValid = $false
  }

  # Test 3: GitHub structure exists
  Write-Step "3" "Validating .github structure"
  $githubPath = Join-Path $script:Config.RootPath $script:Config.GitHubPath
  $requiredFolders = @("prompts", "templates", "scripts")

  foreach ($folder in $requiredFolders) {
    $folderPath = Join-Path $githubPath $folder
    if (Test-Path $folderPath) {
      Write-Success ".github/$folder exists"
    }
    else {
      Write-Error ".github/$folder missing"
      $allValid = $false
    }
  }

  # Test 4: Template files exist
  Write-Step "4" "Validating template files"
  $templatePath = Join-Path $githubPath "templates\mtm-transform-prompt"
  $requiredTemplates = @("research-template.md", "plans-template.md", "details-template.md", "changes-template.md")

  foreach ($template in $requiredTemplates) {
    $templateFile = Join-Path $templatePath $template
    if (Test-Path $templateFile) {
      Write-Success "Template exists: $template"
    }
    else {
      Write-Error "Template missing: $template"
      $allValid = $false
    }
  }

  # Test 5: Tracking directory exists
  Write-Step "5" "Validating tracking directory structure"
  $trackingPath = Join-Path $script:Config.RootPath $script:Config.TrackingPath
  $trackingFolders = @("research", "plans", "details", "changes")

  foreach ($folder in $trackingFolders) {
    $folderPath = Join-Path $trackingPath $folder
    if (Test-Path $folderPath) {
      Write-Success "Tracking folder exists: $folder"
    }
    else {
      Write-Warning "Creating tracking folder: $folder"
      New-Item -Path $folderPath -ItemType Directory -Force | Out-Null
      Write-Success "Created: $folder"
    }
  }

  return $allValid
}

function New-TrackingFiles {
  Write-Header "Phase 1: Documentation Files Creation" "Creating comprehensive tracking files from templates"

  $templatePath = Join-Path $script:Config.RootPath ".github\templates\mtm-transform-prompt"
  $trackingPath = Join-Path $script:Config.RootPath $script:Config.TrackingPath

  $fileMap = @{
    "research-template.md" = "research\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-research.md"
    "plans-template.md"    = "plans\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-plans.md"
    "details-template.md"  = "details\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-details.md"
    "changes-template.md"  = "changes\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-changes.md"
  }

  $createdFiles = @()

  foreach ($templateFile in $fileMap.Keys) {
    Write-Step ($fileMap.Keys.IndexOf($templateFile) + 1) "Creating $($fileMap[$templateFile])"

    $templatePath_Full = Join-Path $templatePath $templateFile
    $targetPath = Join-Path $trackingPath $fileMap[$templateFile]

    if (Test-Path $templatePath_Full) {
      # Read template content
      $templateContent = Get-Content $templatePath_Full -Raw

      # Replace template variables
      $processedContent = $templateContent -replace '\{TARGET_FILE\}', $TargetFile
      $processedContent = $processedContent -replace '\{target-file-name\}', $script:Config.TargetFileName
      $processedContent = $processedContent -replace '\{YYYYMMDD\}', $script:Config.DateStamp
      $processedContent = $processedContent -replace '\{ADDITIONAL_REQUIREMENTS\}', $AdditionalRequirements

      # Write processed content to target file
      $processedContent | Set-Content -Path $targetPath -Encoding UTF8 -Force

      Write-Success "Created: $(Split-Path $targetPath -Leaf)"
      $createdFiles += $targetPath
    }
    else {
      Write-Error "Template not found: $templateFile"
    }
  }

  if ($createdFiles.Count -eq 4) {
    Write-Success "All tracking files created successfully!"

    $confirmation = Request-AgentConfirmation -Question "Ready to proceed to Phase 2 (Research Analysis)?" `
      -Context "All tracking files have been created. @Agent should now fill out the research file with comprehensive analysis of $TargetFile.axaml"

    return $confirmation -eq "Yes"
  }
  else {
    Write-Error "Failed to create all tracking files. Cannot proceed."
    return $false
  }
}

function Start-ResearchPhase {
  Write-Header "Phase 2: Research Analysis" "@Agent collaboration for comprehensive file analysis"

  $researchFile = Join-Path $script:Config.RootPath "$($script:Config.TrackingPath)\research\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-research.md"

  Write-Info "Research file created at: $researchFile"
  Write-Info "@Agent should now:"
  Write-Host "  1. Analyze $TargetFile.axaml completely" -ForegroundColor $script:Colors.Code
  Write-Host "  2. Document business requirements" -ForegroundColor $script:Colors.Code
  Write-Host "  3. Identify required StyleSystem classes" -ForegroundColor $script:Colors.Code
  Write-Host "  4. Identify required Theme V2 tokens" -ForegroundColor $script:Colors.Code
  Write-Host "  5. Complete risk assessment" -ForegroundColor $script:Colors.Code

  $confirmation = Request-AgentConfirmation -Question "Has @Agent completed the research analysis?" `
    -Context "Research file should be fully populated with analysis before proceeding to planning phase"

  if ($confirmation -eq "Yes") {
    Write-Success "Research phase completed!"
    return $true
  }
  else {
    Write-Warning "Research phase not completed. Please complete analysis before continuing."
    return $false
  }
}

function Start-PlanningPhase {
  Write-Header "Phase 3: Implementation Planning" "@Agent collaboration for detailed implementation plan"

  $plansFile = Join-Path $script:Config.RootPath "$($script:Config.TrackingPath)\plans\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-plans.md"
  $detailsFile = Join-Path $script:Config.RootPath "$($script:Config.TrackingPath)\details\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-details.md"

  Write-Info "Planning file: $plansFile"
  Write-Info "Details file: $detailsFile"
  Write-Info "@Agent should now:"
  Write-Host "  1. Create detailed implementation strategy" -ForegroundColor $script:Colors.Code
  Write-Host "  2. Plan missing StyleSystem components" -ForegroundColor $script:Colors.Code
  Write-Host "  3. Plan missing Theme V2 tokens" -ForegroundColor $script:Colors.Code
  Write-Host "  4. Create implementation checklist" -ForegroundColor $script:Colors.Code
  Write-Host "  5. Define success metrics" -ForegroundColor $script:Colors.Code

  $confirmation = Request-AgentConfirmation -Question "Has @Agent completed the implementation planning?" `
    -Context "Both plans and details files should be populated with comprehensive implementation strategy"

  if ($confirmation -eq "Yes") {
    Write-Success "Planning phase completed!"
    return $true
  }
  else {
    Write-Warning "Planning phase not completed. Please complete planning before continuing."
    return $false
  }
}

function Start-ImplementationPhase {
  Write-Header "Phase 4: StyleSystem Implementation" "@Agent guided implementation with progress tracking"

  Write-Info "@Agent should now execute implementation in this order:"
  Write-Host "  1. Create missing StyleSystem components first" -ForegroundColor $script:Colors.Code
  Write-Host "  2. Create missing Theme V2 tokens second" -ForegroundColor $script:Colors.Code
  Write-Host "  3. Update StyleSystem includes third" -ForegroundColor $script:Colors.Code
  Write-Host "  4. Create backup of $TargetFile.axaml" -ForegroundColor $script:Colors.Code
  Write-Host "  5. Transform $TargetFile.axaml with StyleSystem" -ForegroundColor $script:Colors.Code
  Write-Host "  6. Update details file with progress" -ForegroundColor $script:Colors.Code

  # Sub-phase confirmations
  $componentsReady = Request-AgentConfirmation -Question "Have missing StyleSystem components been created?" `
    -Context "@Agent should create any missing StyleSystem classes identified in research"

  if ($componentsReady -eq "Yes") {
    Write-Success "StyleSystem components ready!"
  }
  else {
    Write-Warning "Please create missing StyleSystem components before continuing."
    return $false
  }

  $tokensReady = Request-AgentConfirmation -Question "Have missing Theme V2 tokens been created?" `
    -Context "@Agent should create any missing semantic tokens identified in research"

  if ($tokensReady -eq "Yes") {
    Write-Success "Theme V2 tokens ready!"
  }
  else {
    Write-Warning "Please create missing Theme V2 tokens before continuing."
    return $false
  }

  $transformationReady = Request-AgentConfirmation -Question "Has $TargetFile.axaml been transformed successfully?" `
    -Context "@Agent should have completed AXAML transformation with StyleSystem classes and Theme V2 tokens"

  if ($transformationReady -eq "Yes") {
    Write-Success "AXAML transformation completed!"
    return $true
  }
  else {
    Write-Warning "Please complete AXAML transformation before continuing."
    return $false
  }
}

function Start-ValidationPhase {
  Write-Header "Phase 5: Validation & Testing" "@Agent guided validation and quality assurance"

  Write-Info "@Agent should now validate:"
  Write-Host "  1. Project builds without errors (Debug & Release)" -ForegroundColor $script:Colors.Code
  Write-Host "  2. Both light and dark themes work correctly" -ForegroundColor $script:Colors.Code
  Write-Host "  3. All business logic preserved and functional" -ForegroundColor $script:Colors.Code
  Write-Host "  4. No AVLN2000 syntax errors" -ForegroundColor $script:Colors.Code
  Write-Host "  5. Performance maintained or improved" -ForegroundColor $script:Colors.Code

  $buildValid = Request-AgentConfirmation -Question "Does the project build successfully without errors?" `
    -Context "Both Debug and Release builds should compile cleanly"

  if ($buildValid -ne "Yes") {
    Write-Error "Build validation failed. Please fix build errors before continuing."
    return $false
  }

  $themesValid = Request-AgentConfirmation -Question "Do both light and dark themes work correctly?" `
    -Context "All UI elements should render properly in both theme modes"

  if ($themesValid -ne "Yes") {
    Write-Error "Theme validation failed. Please fix theme issues before continuing."
    return $false
  }

  $functionalValid = Request-AgentConfirmation -Question "Is all business logic preserved and working?" `
    -Context "All original functionality should be intact after transformation"

  if ($functionalValid -ne "Yes") {
    Write-Error "Functional validation failed. Please fix functionality issues before continuing."
    return $false
  }

  Write-Success "All validation checks passed!"
  return $true
}

function Complete-Documentation {
  Write-Header "Phase 6: Documentation Completion" "Finalizing changes tracking and lessons learned"

  $changesFile = Join-Path $script:Config.RootPath "$($script:Config.TrackingPath)\changes\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-changes.md"

  Write-Info "Changes file: $changesFile"
  Write-Info "@Agent should now complete:"
  Write-Host "  1. Detailed change log with before/after examples" -ForegroundColor $script:Colors.Code
  Write-Host "  2. Success metrics and measurements" -ForegroundColor $script:Colors.Code
  Write-Host "  3. Issues encountered and resolutions" -ForegroundColor $script:Colors.Code
  Write-Host "  4. Lessons learned and recommendations" -ForegroundColor $script:Colors.Code
  Write-Host "  5. Final summary and deployment readiness" -ForegroundColor $script:Colors.Code

  $documentationComplete = Request-AgentConfirmation -Question "Has @Agent completed all documentation?" `
    -Context "Changes file should be fully populated with comprehensive transformation results"

  if ($documentationComplete -eq "Yes") {
    Write-Success "Documentation phase completed!"
    return $true
  }
  else {
    Write-Warning "Please complete documentation before finishing."
    return $false
  }
}

function Show-CompletionSummary {
  Write-Header "🎉 MTM AXAML Transformation Complete!" "Summary and next steps"

  $trackingPath = Join-Path $script:Config.RootPath $script:Config.TrackingPath

  Write-Success "Transformation of $TargetFile.axaml completed successfully!"
  Write-Host ""

  Write-Info "Generated documentation files:"
  Write-Host "  📄 Research: $trackingPath\research\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-research.md" -ForegroundColor $script:Colors.Code
  Write-Host "  📄 Plans: $trackingPath\plans\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-plans.md" -ForegroundColor $script:Colors.Code
  Write-Host "  📄 Details: $trackingPath\details\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-details.md" -ForegroundColor $script:Colors.Code
  Write-Host "  📄 Changes: $trackingPath\changes\$($script:Config.DateStamp)-$($script:Config.TargetFileName)-style-transformation-changes.md" -ForegroundColor $script:Colors.Code

  Write-Host ""
  Write-Info "Next steps:"
  Write-Host "  ✅ Code review the transformed $TargetFile.axaml" -ForegroundColor $script:Colors.Success
  Write-Host "  ✅ Run comprehensive testing" -ForegroundColor $script:Colors.Success
  Write-Host "  ✅ Deploy to staging environment" -ForegroundColor $script:Colors.Success
  Write-Host "  ✅ Archive documentation files" -ForegroundColor $script:Colors.Success

  Write-Host ""
  Write-Host "🚀 Ready for deployment!" -ForegroundColor $script:Colors.Success
}

#endregion

#region Main Execution

function Start-MainWorkflow {
  Write-Header "MTM AXAML StyleSystem Transformation" "Guided @Agent collaboration workflow for $TargetFile.axaml"

  Write-Info "Starting transformation workflow..."
  Write-Info "Target File: $TargetFile.axaml"
  Write-Info "Date: $($script:Config.DateStamp)"
  Write-Info "Time: $($script:Config.TimeStamp)"
  if ($AdditionalRequirements) {
    Write-Info "Additional Requirements: $AdditionalRequirements"
  }

  # Phase 0: Prerequisites
  if (-not $SkipValidation) {
    if (-not (Test-Prerequisites)) {
      Write-Error "Prerequisites validation failed. Cannot continue."
      return $false
    }
  }

  # Phase 1: Create tracking files
  if (-not (New-TrackingFiles)) {
    Write-Error "Failed to create tracking files. Cannot continue."
    return $false
  }

  # Phase 2: Research
  if (-not (Start-ResearchPhase)) {
    Write-Error "Research phase not completed. Cannot continue."
    return $false
  }

  # Phase 3: Planning
  if (-not (Start-PlanningPhase)) {
    Write-Error "Planning phase not completed. Cannot continue."
    return $false
  }

  # Phase 4: Implementation
  if (-not (Start-ImplementationPhase)) {
    Write-Error "Implementation phase not completed. Cannot continue."
    return $false
  }

  # Phase 5: Validation
  if (-not (Start-ValidationPhase)) {
    Write-Error "Validation phase failed. Cannot continue."
    return $false
  }

  # Phase 6: Documentation
  if (-not (Complete-Documentation)) {
    Write-Error "Documentation phase not completed. Cannot continue."
    return $false
  }

  # Completion
  Show-CompletionSummary
  return $true
}

# Script entry point
try {
  $success = Start-MainWorkflow

  if ($success) {
    Write-Host ""
    Write-Host "🎯 MTM AXAML Transformation completed successfully!" -ForegroundColor $script:Colors.Success
    exit 0
  }
  else {
    Write-Host ""
    Write-Host "❌ MTM AXAML Transformation failed or was incomplete." -ForegroundColor $script:Colors.Error
    exit 1
  }
}
catch {
  Write-Host ""
  Write-Host "💥 Fatal error during transformation:" -ForegroundColor $script:Colors.Error
  Write-Host $_.Exception.Message -ForegroundColor $script:Colors.Error
  Write-Host $_.ScriptStackTrace -ForegroundColor $script:Colors.Code
  exit 1
}

#endregion
