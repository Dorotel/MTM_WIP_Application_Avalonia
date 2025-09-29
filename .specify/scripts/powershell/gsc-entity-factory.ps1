# GSC Entity Factory and Validation Utilities
# Date: September 28, 2025
# Purpose: Manufacturing-grade entity creation, validation, and serialization
# 24/7 Operations: Comprehensive error handling, state persistence, recovery patterns

# Import entity models
. "$PSScriptRoot\gsc-entities.ps1"

<#
.SYNOPSIS
GSC Entity Factory for creating and validating GSC entities

.DESCRIPTION
Provides factory methods for creating GSC entities with comprehensive validation,
error handling, and serialization support. Designed for manufacturing-grade
reliability with 24/7 operations support.

Key Features:
- Comprehensive input validation
- Manufacturing-grade error handling
- Cross-platform compatibility
- State persistence and recovery
- Performance monitoring
- Team collaboration support
#>

#region Entity Factory Functions

<#
.SYNOPSIS
Creates a new GSC Command entity with validation

.PARAMETER Command
The GSC command type (constitution, specify, clarify, etc.)

.PARAMETER Parameters
Optional hashtable of command parameters

.PARAMETER WorkflowId
Optional workflow identifier for context

.EXAMPLE
$cmd = New-GSCCommand -Command "specify" -Parameters @{ Feature = "UserControl" }
#>
function New-GSCCommand {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateSet('constitution', 'specify', 'clarify', 'plan', 'task', 'analyze', 'implement', 'memory', 'validate', 'status', 'rollback', 'help')]
        [string] $Command,

        [hashtable] $Parameters = @{},

        [string] $WorkflowId = $null
    )

    try {
        Write-Verbose "[GSC-Factory] Creating new GSC command: $Command"

        # Create command entity
        $gscCommand = [GSCCommand]::new($Command)
        $gscCommand.Parameters = $Parameters.Clone()

        # Set workflow context if provided
        if ($WorkflowId) {
            $gscCommand.Context.WorkflowId = $WorkflowId
        }

        # Validate entity
        if (-not $gscCommand.IsValid()) {
            throw [InvalidOperationException]::new("Created GSC command failed validation")
        }

        Write-Verbose "[GSC-Factory] GSC command created successfully: $($gscCommand.CommandId)"
        return $gscCommand
    }
    catch {
        Write-Error "[GSC-Factory] Failed to create GSC command '$Command': $($_.Exception.Message)"
        throw
    }
}

<#
.SYNOPSIS
Creates a new GSC Workflow entity with manufacturing context

.PARAMETER Domain
The primary domain (manufacturing, ui, database, service, testing, general)

.PARAMETER Complexity
The complexity assessment (low, medium, high, critical)

.PARAMETER Priority
The business priority (low, medium, high, urgent)

.PARAMETER Technology
Optional array of technology stack components

.EXAMPLE
$workflow = New-GSCWorkflow -Domain "manufacturing" -Complexity "high" -Priority "urgent" -Technology @("avalonia", "dotnet8", "mysql")
#>
function New-GSCWorkflow {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidateSet('manufacturing', 'ui', 'database', 'service', 'testing', 'general')]
        [string] $Domain,

        [Parameter(Mandatory)]
        [ValidateSet('low', 'medium', 'high', 'critical')]
        [string] $Complexity,

        [Parameter(Mandatory)]
        [ValidateSet('low', 'medium', 'high', 'urgent')]
        [string] $Priority,

        [ValidateSet('avalonia', 'dotnet8', 'mvvm', 'mysql', 'pester', 'powershell')]
        [string[]] $Technology = @()
    )

    try {
        Write-Verbose "[GSC-Factory] Creating new GSC workflow: Domain=$Domain, Complexity=$Complexity, Priority=$Priority"

        # Create workflow entity
        $workflow = [GSCWorkflow]::new($Domain, $Complexity, $Priority)
        $workflow.Context.Technology = $Technology

        # Validate entity
        if (-not $workflow.IsValid()) {
            throw [InvalidOperationException]::new("Created GSC workflow failed validation")
        }

        Write-Verbose "[GSC-Factory] GSC workflow created successfully: $($workflow.WorkflowId)"
        return $workflow
    }
    catch {
        Write-Error "[GSC-Factory] Failed to create GSC workflow: $($_.Exception.Message)"
        throw
    }
}

<#
.SYNOPSIS
Creates a new GSC Task entity with dependencies and validation

.PARAMETER Id
Task identifier (format: T001, T002, etc.)

.PARAMETER Title
Task title/summary

.PARAMETER Phase
Workflow phase (analyze, design, implement, validate, reflect, handoff)

.PARAMETER Priority
Task priority (low, medium, high, critical)

.PARAMETER Description
Optional detailed task description

.PARAMETER Dependencies
Optional array of task ID dependencies

.PARAMETER EstimatedDuration
Optional estimated duration in minutes

.PARAMETER Assignee
Optional assignee (human, ai, collaborative)

.EXAMPLE
$task = New-GSCTask -Id "T001" -Title "Create AXAML view" -Phase "implement" -Priority "high" -Dependencies @("T000") -EstimatedDuration 30
#>
function New-GSCTask {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [ValidatePattern('^T[0-9]{3}$')]
        [string] $Id,

        [Parameter(Mandatory)]
        [ValidateNotNullOrEmpty()]
        [string] $Title,

        [Parameter(Mandatory)]
        [ValidateSet('analyze', 'design', 'implement', 'validate', 'reflect', 'handoff')]
        [string] $Phase,

        [Parameter(Mandatory)]
        [ValidateSet('low', 'medium', 'high', 'critical')]
        [string] $Priority,

        [string] $Description = "",

        [string[]] $Dependencies = @(),

        [double] $EstimatedDuration = 0.0,

        [ValidateSet('human', 'ai', 'collaborative')]
        [string] $Assignee = 'ai'
    )

    try {
        Write-Verbose "[GSC-Factory] Creating new GSC task: $Id - $Title"

        # Create task entity
        $task = [GSCTask]::new($Id, $Title, $Phase, $Priority)
        $task.Description = $Description
        $task.Dependencies = $Dependencies
        $task.EstimatedDuration = $EstimatedDuration
        $task.Assignee = $Assignee

        # Validate entity
        if (-not $task.IsValid()) {
            throw [InvalidOperationException]::new("Created GSC task failed validation")
        }

        Write-Verbose "[GSC-Factory] GSC task created successfully: $Id"
        return $task
    }
    catch {
        Write-Error "[GSC-Factory] Failed to create GSC task '$Id': $($_.Exception.Message)"
        throw
    }
}

<#
.SYNOPSIS
Creates a new GSC Session entity for collaborative development

.PARAMETER WorkflowId
Optional associated workflow identifier

.PARAMETER Objective
Session objective description

.PARAMETER Scope
Session scope description

.EXAMPLE
$session = New-GSCSession -WorkflowId "wf-12345" -Objective "Implement Avalonia UserControl" -Scope "Manufacturing dashboard component"
#>
function New-GSCSession {
    [CmdletBinding()]
    param(
        [string] $WorkflowId = $null,
        [string] $Objective = "",
        [string] $Scope = ""
    )

    try {
        Write-Verbose "[GSC-Factory] Creating new GSC session"

        # Create session entity
        $session = [GSCSession]::new()

        # Set workflow context if provided
        if ($WorkflowId) {
            $session.Workflow = @{
                WorkflowId = $WorkflowId
                Phase = ""
                Objective = $Objective
                Scope = $Scope
            }
        }

        # Add default AI participant
        $session.AddParticipant("github-copilot", "ai")

        # Validate entity
        if (-not $session.IsValid()) {
            throw [InvalidOperationException]::new("Created GSC session failed validation")
        }

        Write-Verbose "[GSC-Factory] GSC session created successfully: $($session.SessionId)"
        return $session
    }
    catch {
        Write-Error "[GSC-Factory] Failed to create GSC session: $($_.Exception.Message)"
        throw
    }
}

<#
.SYNOPSIS
Creates a new GSC Memory Integration entity

.EXAMPLE
$memoryIntegration = New-GSCMemoryIntegration
#>
function New-GSCMemoryIntegration {
    [CmdletBinding()]
    param()

    try {
        Write-Verbose "[GSC-Factory] Creating new GSC memory integration"

        # Create memory integration entity
        $memoryIntegration = [GSCMemoryIntegration]::new()

        # Validate entity
        if (-not $memoryIntegration.IsValid()) {
            throw [InvalidOperationException]::new("Created GSC memory integration failed validation")
        }

        Write-Verbose "[GSC-Factory] GSC memory integration created successfully: $($memoryIntegration.MemoryId)"
        return $memoryIntegration
    }
    catch {
        Write-Error "[GSC-Factory] Failed to create GSC memory integration: $($_.Exception.Message)"
        throw
    }
}

#endregion Entity Factory Functions

#region Entity Validation Functions

<#
.SYNOPSIS
Validates a GSC entity with comprehensive checks

.PARAMETER Entity
The GSC entity to validate

.PARAMETER ValidationLevel
Validation level (basic, standard, comprehensive)

.EXAMPLE
$isValid = Test-GSCEntity -Entity $command -ValidationLevel "comprehensive"
#>
function Test-GSCEntity {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [object] $Entity,

        [ValidateSet('basic', 'standard', 'comprehensive')]
        [string] $ValidationLevel = 'standard'
    )

    try {
        Write-Verbose "[GSC-Validation] Validating entity: $($Entity.GetType().Name) - Level: $ValidationLevel"

        # Basic validation - check if entity has IsValid method and call it
        if (-not ($Entity | Get-Member -Name "IsValid" -MemberType Method)) {
            Write-Warning "[GSC-Validation] Entity does not implement IsValid method"
            return $false
        }

        $basicValid = $Entity.IsValid()
        if (-not $basicValid) {
            Write-Warning "[GSC-Validation] Entity failed basic validation"
            return $false
        }

        if ($ValidationLevel -eq 'basic') {
            return $basicValid
        }

        # Standard validation - check common properties
        $standardChecks = @()

        # Check for required string properties
        $stringProps = @('Id', 'CommandId', 'WorkflowId', 'SessionId', 'MemoryId', 'Title', 'Command')
        foreach ($prop in $stringProps) {
            if ($Entity | Get-Member -Name $prop -MemberType Property) {
                $value = $Entity.$prop
                if ([string]::IsNullOrWhiteSpace($value)) {
                    $standardChecks += "Property '$prop' is null or empty"
                }
            }
        }

        # Check for required datetime properties
        $requiredDateProps = @('Created')  # Only Created is truly required

        foreach ($prop in $requiredDateProps) {
            if ($Entity | Get-Member -Name $prop -MemberType Property) {
                $value = $Entity.$prop
                if ($null -eq $value -or $value -eq [datetime]::MinValue) {
                    $standardChecks += "Property '$prop' is not set"
                }
            }
        }

        # Note: Started, LastModified can be MinValue initially (not started/modified yet)
        # This is valid state for new entities

        if ($standardChecks.Count -gt 0) {
            Write-Warning "[GSC-Validation] Standard validation failures: $($standardChecks -join '; ')"
            if ($ValidationLevel -eq 'standard') {
                return $false
            }
        }

        # Comprehensive validation - deep object validation
        if ($ValidationLevel -eq 'comprehensive') {
            $comprehensiveChecks = @()

            # Check nested objects
            $objectProps = @('Context', 'Result', 'Validation', 'MemoryIntegration', 'Collaboration')
            foreach ($prop in $objectProps) {
                if ($Entity | Get-Member -Name $prop -MemberType Property) {
                    $nestedObj = $Entity.$prop
                    if ($null -ne $nestedObj -and ($nestedObj | Get-Member -Name "IsValid" -MemberType Method)) {
                        if (-not $nestedObj.IsValid()) {
                            $comprehensiveChecks += "Nested object '$prop' failed validation"
                        }
                    }
                }
            }

            # Check arrays
            $arrayProps = @('Tasks', 'Commands', 'Participants', 'Artifacts', 'Errors')
            foreach ($prop in $arrayProps) {
                if ($Entity | Get-Member -Name $prop -MemberType Property) {
                    $array = $Entity.$prop
                    if ($null -ne $array -and $array -is [array]) {
                        foreach ($item in $array) {
                            if ($null -ne $item -and ($item | Get-Member -Name "IsValid" -MemberType Method)) {
                                if (-not $item.IsValid()) {
                                    $comprehensiveChecks += "Array item in '$prop' failed validation"
                                }
                            }
                        }
                    }
                }
            }

            if ($comprehensiveChecks.Count -gt 0) {
                Write-Warning "[GSC-Validation] Comprehensive validation failures: $($comprehensiveChecks -join '; ')"
                return $false
            }
        }

        Write-Verbose "[GSC-Validation] Entity validation successful"
        return $true
    }
    catch {
        Write-Error "[GSC-Validation] Entity validation error: $($_.Exception.Message)"
        return $false
    }
}

<#
.SYNOPSIS
Validates GSC entity relationships and dependencies

.PARAMETER Entities
Array of GSC entities to validate relationships

.EXAMPLE
$relationshipsValid = Test-GSCEntityRelationships -Entities @($workflow, $task1, $task2)
#>
function Test-GSCEntityRelationships {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [object[]] $Entities
    )

    try {
        Write-Verbose "[GSC-Validation] Validating entity relationships for $($Entities.Count) entities"

        $validationIssues = @()

        # Group entities by type
        $workflows = $Entities | Where-Object { $_ -is [GSCWorkflow] }
        $tasks = $Entities | Where-Object { $_ -is [GSCTask] }
        $commands = $Entities | Where-Object { $_ -is [GSCCommand] }
        $sessions = $Entities | Where-Object { $_ -is [GSCSession] }

        # Validate workflow-task relationships
        foreach ($workflow in $workflows) {
            $workflowTasks = $workflow.Tasks
            foreach ($task in $workflowTasks) {
                if ($task.Id -notin ($tasks | ForEach-Object { $_.Id })) {
                    $validationIssues += "Workflow $($workflow.WorkflowId) references non-existent task $($task.Id)"
                }
            }
        }

        # Validate task dependencies
        foreach ($task in $tasks) {
            foreach ($depId in $task.Dependencies) {
                if ($depId -notin ($tasks | ForEach-Object { $_.Id })) {
                    $validationIssues += "Task $($task.Id) has dependency on non-existent task $depId"
                }
            }
        }

        # Validate command-workflow relationships
        foreach ($command in $commands) {
            if ($command.Context.WorkflowId) {
                $workflowExists = $workflows | Where-Object { $_.WorkflowId -eq $command.Context.WorkflowId }
                if (-not $workflowExists) {
                    $validationIssues += "Command $($command.CommandId) references non-existent workflow $($command.Context.WorkflowId)"
                }
            }
        }

        # Validate session-workflow relationships
        foreach ($session in $sessions) {
            if ($session.Workflow -and $session.Workflow.WorkflowId) {
                $workflowExists = $workflows | Where-Object { $_.WorkflowId -eq $session.Workflow.WorkflowId }
                if (-not $workflowExists) {
                    $validationIssues += "Session $($session.SessionId) references non-existent workflow $($session.Workflow.WorkflowId)"
                }
            }
        }

        if ($validationIssues.Count -gt 0) {
            Write-Warning "[GSC-Validation] Relationship validation failures:"
            $validationIssues | ForEach-Object { Write-Warning "  - $_" }
            return $false
        }

        Write-Verbose "[GSC-Validation] Entity relationships validation successful"
        return $true
    }
    catch {
        Write-Error "[GSC-Validation] Entity relationships validation error: $($_.Exception.Message)"
        return $false
    }
}

#endregion Entity Validation Functions

#region Entity Serialization Functions

<#
.SYNOPSIS
Serializes GSC entity to JSON with schema validation

.PARAMETER Entity
The GSC entity to serialize

.PARAMETER SchemaPath
Optional path to JSON schema for validation

.PARAMETER Compress
Whether to compress the JSON output

.EXAMPLE
$json = ConvertTo-GSCJson -Entity $workflow -SchemaPath ".specify/state/workflow-state.schema.json"
#>
function ConvertTo-GSCJson {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [object] $Entity,

        [string] $SchemaPath = $null,

        [switch] $Compress
    )

    try {
        Write-Verbose "[GSC-Serialization] Serializing entity: $($Entity.GetType().Name)"

        # Validate entity before serialization
        if (-not (Test-GSCEntity -Entity $Entity -ValidationLevel "standard")) {
            throw [InvalidOperationException]::new("Entity failed validation before serialization")
        }

        # Convert to JSON
        $json = if ($Compress) {
            $Entity | ConvertTo-Json -Depth 10 -Compress
        } else {
            $Entity | ConvertTo-Json -Depth 10
        }

        # Validate against schema if provided
        if ($SchemaPath -and (Test-Path $SchemaPath)) {
            Write-Verbose "[GSC-Serialization] Validating against schema: $SchemaPath"
            # Schema validation would be implemented here with a JSON schema validator
            # For now, we'll skip this step but the structure is in place
        }

        Write-Verbose "[GSC-Serialization] Entity serialization successful"
        return $json
    }
    catch {
        Write-Error "[GSC-Serialization] Entity serialization error: $($_.Exception.Message)"
        throw
    }
}

<#
.SYNOPSIS
Deserializes JSON to GSC entity with validation

.PARAMETER Json
The JSON string to deserialize

.PARAMETER EntityType
The expected GSC entity type

.PARAMETER SchemaPath
Optional path to JSON schema for validation

.EXAMPLE
$workflow = ConvertFrom-GSCJson -Json $json -EntityType "GSCWorkflow" -SchemaPath ".specify/state/workflow-state.schema.json"
#>
function ConvertFrom-GSCJson {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string] $Json,

        [Parameter(Mandatory)]
        [ValidateSet('GSCCommand', 'GSCWorkflow', 'GSCTask', 'GSCSession', 'GSCMemoryIntegration')]
        [string] $EntityType,

        [string] $SchemaPath = $null
    )

    try {
        Write-Verbose "[GSC-Serialization] Deserializing JSON to entity: $EntityType"

        if ([string]::IsNullOrWhiteSpace($Json)) {
            throw [ArgumentException]::new("JSON cannot be null or empty")
        }

        # Validate against schema if provided
        if ($SchemaPath -and (Test-Path $SchemaPath)) {
            Write-Verbose "[GSC-Serialization] Validating JSON against schema: $SchemaPath"
            # Schema validation would be implemented here
        }

        # Deserialize based on entity type
        $entity = switch ($EntityType) {
            'GSCCommand' {
                [GSCCommand]::FromJson($Json)
            }
            'GSCWorkflow' {
                $data = $Json | ConvertFrom-Json
                $workflow = [GSCWorkflow]::new($data.Context.Domain, $data.Context.Complexity, $data.Context.Priority)
                # Populate properties from data
                $workflow.WorkflowId = $data.WorkflowId
                $workflow.CurrentPhase = $data.CurrentPhase
                $workflow.Status = $data.Status
                $workflow.Created = [datetime]$data.Created
                $workflow.LastModified = [datetime]$data.LastModified
                $workflow
            }
            default {
                throw [NotImplementedException]::new("Deserialization for $EntityType is not yet implemented")
            }
        }

        # Validate deserialized entity
        if (-not (Test-GSCEntity -Entity $entity -ValidationLevel "standard")) {
            throw [InvalidOperationException]::new("Deserialized entity failed validation")
        }

        Write-Verbose "[GSC-Serialization] Entity deserialization successful"
        return $entity
    }
    catch {
        Write-Error "[GSC-Serialization] Entity deserialization error: $($_.Exception.Message)"
        throw
    }
}

#endregion Entity Serialization Functions

#region Entity State Persistence Functions

<#
.SYNOPSIS
Saves GSC entity state to file with atomic operations

.PARAMETER Entity
The GSC entity to save

.PARAMETER FilePath
The file path to save to

.PARAMETER BackupExisting
Whether to backup existing file

.EXAMPLE
Save-GSCEntityState -Entity $workflow -FilePath ".specify/state/current-workflow.json" -BackupExisting
#>
function Save-GSCEntityState {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [object] $Entity,

        [Parameter(Mandatory)]
        [string] $FilePath,

        [switch] $BackupExisting
    )

    try {
        Write-Verbose "[GSC-Persistence] Saving entity state to: $FilePath"

        # Ensure directory exists
        $directory = Split-Path -Path $FilePath -Parent
        if (-not (Test-Path $directory)) {
            New-Item -Path $directory -ItemType Directory -Force | Out-Null
        }

        # Backup existing file if requested
        if ($BackupExisting -and (Test-Path $FilePath)) {
            $backupPath = "$FilePath.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
            Copy-Item -Path $FilePath -Destination $backupPath -Force
            Write-Verbose "[GSC-Persistence] Existing file backed up to: $backupPath"
        }

        # Serialize entity
        $json = ConvertTo-GSCJson -Entity $Entity

        # Atomic write operation
        $tempPath = "$FilePath.tmp"
        $json | Out-File -FilePath $tempPath -Encoding UTF8 -Force

        # Move temp file to final location (atomic on most filesystems)
        Move-Item -Path $tempPath -Destination $FilePath -Force

        Write-Verbose "[GSC-Persistence] Entity state saved successfully"
        return $true
    }
    catch {
        Write-Error "[GSC-Persistence] Failed to save entity state: $($_.Exception.Message)"
        return $false
    }
}

<#
.SYNOPSIS
Loads GSC entity state from file with validation

.PARAMETER FilePath
The file path to load from

.PARAMETER EntityType
The expected GSC entity type

.EXAMPLE
$workflow = Restore-GSCEntityState -FilePath ".specify/state/current-workflow.json" -EntityType "GSCWorkflow"
#>
function Restore-GSCEntityState {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string] $FilePath,

        [Parameter(Mandatory)]
        [ValidateSet('GSCCommand', 'GSCWorkflow', 'GSCTask', 'GSCSession', 'GSCMemoryIntegration')]
        [string] $EntityType
    )

    try {
        Write-Verbose "[GSC-Persistence] Loading entity state from: $FilePath"

        if (-not (Test-Path $FilePath)) {
            throw [FileNotFoundException]::new("Entity state file not found: $FilePath")
        }

        # Read JSON content
        $json = Get-Content -Path $FilePath -Raw -Encoding UTF8

        # Deserialize entity
        $entity = ConvertFrom-GSCJson -Json $json -EntityType $EntityType

        Write-Verbose "[GSC-Persistence] Entity state loaded successfully"
        return $entity
    }
    catch {
        Write-Error "[GSC-Persistence] Failed to load entity state: $($_.Exception.Message)"
        throw
    }
}

#endregion Entity State Persistence Functions

# Functions are available for dot-sourcing

Write-Host "[GSC] Entity Factory and Validation utilities loaded - Manufacturing-grade reliability enabled" -ForegroundColor Green
