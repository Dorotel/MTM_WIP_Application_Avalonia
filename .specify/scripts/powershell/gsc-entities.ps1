# GSC Entity Models
# Date: September 28, 2025
# Purpose: Core entity models for GSC (GitHub Spec Collection) system
# Manufacturing-grade reliability patterns with comprehensive validation

<#
.SYNOPSIS
GSC Entity Models for GitHub Specification Collection system

.DESCRIPTION
Defines all core entity models for the GSC system including:
- GSCCommand: Individual command execution tracking
- GSCWorkflow: Multi-phase workflow management
- GSCTask: Task and dependency management
- GSCSession: Collaborative development sessions
- GSCMemoryIntegration: Memory pattern integration

All entities include comprehensive validation, serialization, and
manufacturing-grade reliability patterns for 24/7 operations.
#>

#region Supporting Classes

# Execution context for commands and workflows
class GSCExecutionContext {
    [string] $WorkflowId
    [string] $SessionId
    [hashtable] $Environment
    [string[]] $Tags
    [hashtable] $Metadata

    GSCExecutionContext() {
        $this.Environment = @{}
        $this.Tags = @()
        $this.Metadata = @{}
    }

    [bool] IsValid() {
        return $null -ne $this.Environment -and $null -ne $this.Tags -and $null -ne $this.Metadata
    }
}

# Execution result tracking
class GSCExecutionResult {
    [bool] $Success
    [string] $Output
    [string] $ErrorMessage
    [int] $ExitCode
    [timespan] $Duration
    [hashtable] $Metrics

    GSCExecutionResult() {
        $this.Success = $false
        $this.ExitCode = 0
        $this.Duration = [timespan]::Zero
        $this.Metrics = @{}
    }

    [bool] IsValid() {
        return $null -ne $this.Metrics
    }
}

# Validation result tracking
class GSCValidationResult {
    [bool] $IsValid
    [string[]] $Errors
    [string[]] $Warnings
    [hashtable] $Details

    GSCValidationResult() {
        $this.IsValid = $false
        $this.Errors = @()
        $this.Warnings = @()
        $this.Details = @{}
    }
}

#endregion Supporting Classes

#region Core Entity Models

# GSC Command Entity - Individual command execution tracking
class GSCCommand {
    [string] $CommandId
    [string] $Command
    [hashtable] $Parameters
    [string] $Status
    [datetime] $Created
    [datetime] $Started
    [datetime] $Completed
    [GSCExecutionContext] $Context
    [GSCExecutionResult] $Result

    # Constructor with validation
    GSCCommand([string] $command) {
        if ([string]::IsNullOrWhiteSpace($command)) {
            throw [ArgumentException]::new("Command cannot be null or empty", "command")
        }

        $this.CommandId = "gsc-" + [Guid]::NewGuid().ToString("N").Substring(0, 16)
        $this.Command = $command.ToLower()
        $this.Status = 'queued'
        $this.Created = Get-Date
        $this.Started = [datetime]::MinValue  # Will be set when command starts execution
        $this.Completed = [datetime]::MinValue
        $this.Parameters = @{}
        $this.Context = [GSCExecutionContext]::new()
        $this.Result = [GSCExecutionResult]::new()
    }

    # Start command execution
    [void] Start() {
        if ($this.Status -ne 'queued') {
            throw [InvalidOperationException]::new("Command must be in 'queued' status to start")
        }
        $this.Status = 'running'
        $this.Started = Get-Date
    }

    # Complete command execution
    [void] Complete([bool] $success, [string] $output = "", [string] $errorMessage = "") {
        if ($this.Status -ne 'running') {
            throw [InvalidOperationException]::new("Command must be in 'running' status to complete")
        }
        $this.Status = if ($success) { 'completed' } else { 'failed' }
        $this.Completed = Get-Date
        $this.Result.Success = $success
        $this.Result.Output = $output
        $this.Result.ErrorMessage = $errorMessage
        if ($this.Started -ne [datetime]::MinValue) {
            $this.Result.Duration = $this.Completed - $this.Started
        }
    }

    # Validate command state
    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.CommandId) -and
        -not [string]::IsNullOrWhiteSpace($this.Command) -and
        $this.Created -ne [datetime]::MinValue -and
        $null -ne $this.Context -and $this.Context.IsValid() -and
        $null -ne $this.Result -and $this.Result.IsValid()
    }

    # Convert to JSON for persistence
    [string] ToJson() {
        return $this | ConvertTo-Json -Depth 10 -Compress
    }

    # Create from JSON
    static [GSCCommand] FromJson([string] $json) {
        try {
            $data = $json | ConvertFrom-Json
            $cmd = [GSCCommand]::new($data.Command)
            $cmd.CommandId = $data.CommandId
            $cmd.Status = $data.Status
            $cmd.Created = [datetime]$data.Created
            if ($data.Started -and $data.Started -ne [datetime]::MinValue) {
                $cmd.Started = [datetime]$data.Started
            }
            if ($data.Completed -and $data.Completed -ne [datetime]::MinValue) {
                $cmd.Completed = [datetime]$data.Completed
            }
            if ($data.Parameters) { $cmd.Parameters = $data.Parameters }
            return $cmd
        }
        catch {
            throw [InvalidOperationException]::new("Failed to deserialize GSCCommand from JSON: $($_.Exception.Message)")
        }
    }
}

# GSC Workflow Entity - Multi-phase workflow management
class GSCWorkflow {
    [string] $WorkflowId
    [string] $Title
    [string] $Description
    [string] $Phase
    [string] $Status
    [datetime] $Created
    [datetime] $Started
    [datetime] $Completed
    [string[]] $Tasks
    [string[]] $Commands
    [GSCExecutionContext] $Context
    [GSCValidationResult] $Validation
    [hashtable] $Metrics

    # Constructor
    GSCWorkflow([string] $title) {
        if ([string]::IsNullOrWhiteSpace($title)) {
            throw [ArgumentException]::new("Title cannot be null or empty", "title")
        }

        $this.WorkflowId = "wf-" + [Guid]::NewGuid().ToString("N").Substring(0, 12)
        $this.Title = $title
        $this.Phase = 'planning'
        $this.Status = 'created'
        $this.Created = Get-Date
        $this.Started = [datetime]::MinValue
        $this.Completed = [datetime]::MinValue
        $this.Tasks = @()
        $this.Commands = @()
        $this.Context = [GSCExecutionContext]::new()
        $this.Validation = [GSCValidationResult]::new()
        $this.Metrics = @{}
    }

    # Add task to workflow
    [void] AddTask([string] $taskId) {
        if (-not [string]::IsNullOrWhiteSpace($taskId) -and $taskId -notin $this.Tasks) {
            $this.Tasks += $taskId
        }
    }

    # Add command to workflow
    [void] AddCommand([string] $commandId) {
        if (-not [string]::IsNullOrWhiteSpace($commandId) -and $commandId -notin $this.Commands) {
            $this.Commands += $commandId
        }
    }

    # Advance to next phase
    [void] AdvancePhase([string] $nextPhase) {
        $validPhases = @('planning', 'analysis', 'design', 'implementation', 'validation', 'reflection', 'completed')
        if ($nextPhase -in $validPhases) {
            $this.Phase = $nextPhase
            if ($nextPhase -eq 'completed') {
                $this.Status = 'completed'
                $this.Completed = Get-Date
            }
        }
    }

    # Start workflow execution
    [void] Start() {
        if ($this.Status -eq 'created') {
            $this.Status = 'running'
            $this.Started = Get-Date
        }
    }

    # Validate workflow state
    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.WorkflowId) -and
        -not [string]::IsNullOrWhiteSpace($this.Title) -and
        $this.Created -ne [datetime]::MinValue -and
        $null -ne $this.Context -and $this.Context.IsValid() -and
        $null -ne $this.Tasks -and $null -ne $this.Commands
    }

    # Convert to JSON
    [string] ToJson() {
        return $this | ConvertTo-Json -Depth 10 -Compress
    }

    # Create from JSON
    static [GSCWorkflow] FromJson([string] $json) {
        try {
            $data = $json | ConvertFrom-Json
            $workflow = [GSCWorkflow]::new($data.Title)
            $workflow.WorkflowId = $data.WorkflowId
            $workflow.Description = $data.Description
            $workflow.Phase = $data.Phase
            $workflow.Status = $data.Status
            $workflow.Created = [datetime]$data.Created
            if ($data.Started -and $data.Started -ne [datetime]::MinValue) {
                $workflow.Started = [datetime]$data.Started
            }
            if ($data.Completed -and $data.Completed -ne [datetime]::MinValue) {
                $workflow.Completed = [datetime]$data.Completed
            }
            if ($data.Tasks) { $workflow.Tasks = $data.Tasks }
            if ($data.Commands) { $workflow.Commands = $data.Commands }
            return $workflow
        }
        catch {
            throw [InvalidOperationException]::new("Failed to deserialize GSCWorkflow from JSON: $($_.Exception.Message)")
        }
    }
}

# GSC Task Entity - Task and dependency management
class GSCTask {
    [string] $TaskId
    [string] $Title
    [string] $Description
    [string] $Type
    [string] $Status
    [string] $Priority
    [string] $Assignee
    [datetime] $Created
    [datetime] $Started
    [datetime] $Completed
    [string[]] $Dependencies
    [string[]] $Tags
    [hashtable] $Metadata
    [GSCValidationResult] $Validation

    # Constructor
    GSCTask([string] $title, [string] $type) {
        if ([string]::IsNullOrWhiteSpace($title)) {
            throw [ArgumentException]::new("Title cannot be null or empty", "title")
        }
        if ([string]::IsNullOrWhiteSpace($type)) {
            throw [ArgumentException]::new("Type cannot be null or empty", "type")
        }

        $this.TaskId = "T" + (Get-Random -Minimum 100 -Maximum 999).ToString()
        $this.Title = $title
        $this.Type = $type
        $this.Status = 'pending'
        $this.Priority = 'medium'
        $this.Created = Get-Date
        $this.Started = [datetime]::MinValue
        $this.Completed = [datetime]::MinValue
        $this.Dependencies = @()
        $this.Tags = @()
        $this.Metadata = @{}
        $this.Validation = [GSCValidationResult]::new()
    }

    # Add dependency
    [void] AddDependency([string] $taskId) {
        if (-not [string]::IsNullOrWhiteSpace($taskId) -and $taskId -notin $this.Dependencies) {
            $this.Dependencies += $taskId
        }
    }

    # Start task
    [void] Start() {
        if ($this.Status -eq 'pending') {
            $this.Status = 'in-progress'
            $this.Started = Get-Date
        }
    }

    # Complete task
    [void] Complete([bool] $success = $true) {
        if ($this.Status -eq 'in-progress') {
            $this.Status = if ($success) { 'completed' } else { 'failed' }
            $this.Completed = Get-Date
        }
    }

    # Validate task state
    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.TaskId) -and
        -not [string]::IsNullOrWhiteSpace($this.Title) -and
        -not [string]::IsNullOrWhiteSpace($this.Type) -and
        $this.Created -ne [datetime]::MinValue -and
        $null -ne $this.Dependencies -and $null -ne $this.Tags
    }

    # Convert to JSON
    [string] ToJson() {
        return $this | ConvertTo-Json -Depth 10 -Compress
    }

    # Create from JSON
    static [GSCTask] FromJson([string] $json) {
        try {
            $data = $json | ConvertFrom-Json
            $task = [GSCTask]::new($data.Title, $data.Type)
            $task.TaskId = $data.TaskId
            $task.Description = $data.Description
            $task.Status = $data.Status
            $task.Priority = $data.Priority
            $task.Assignee = $data.Assignee
            $task.Created = [datetime]$data.Created
            if ($data.Started -and $data.Started -ne [datetime]::MinValue) {
                $task.Started = [datetime]$data.Started
            }
            if ($data.Completed -and $data.Completed -ne [datetime]::MinValue) {
                $task.Completed = [datetime]$data.Completed
            }
            if ($data.Dependencies) { $task.Dependencies = $data.Dependencies }
            if ($data.Tags) { $task.Tags = $data.Tags }
            if ($data.Metadata) { $task.Metadata = $data.Metadata }
            return $task
        }
        catch {
            throw [InvalidOperationException]::new("Failed to deserialize GSCTask from JSON: $($_.Exception.Message)")
        }
    }
}

# GSC Session Entity - Collaborative development sessions
class GSCSession {
    [string] $SessionId
    [string] $Title
    [string] $Status
    [datetime] $Created
    [datetime] $Started
    [datetime] $Ended
    [string[]] $Participants
    [string[]] $Commands
    [hashtable] $Collaboration
    [hashtable] $Metrics
    [GSCValidationResult] $Validation

    # Constructor
    GSCSession([string] $title) {
        if ([string]::IsNullOrWhiteSpace($title)) {
            throw [ArgumentException]::new("Title cannot be null or empty", "title")
        }

        $this.SessionId = "sess-" + [Guid]::NewGuid().ToString("N").Substring(0, 8)
        $this.Title = $title
        $this.Status = 'created'
        $this.Created = Get-Date
        $this.Started = [datetime]::MinValue
        $this.Ended = [datetime]::MinValue
        $this.Participants = @()
        $this.Commands = @()
        $this.Collaboration = @{
            'locks'     = @{}
            'handoffs'  = @()
            'conflicts' = @()
        }
        $this.Metrics = @{
            'commandsExecuted'    = 0
            'participantCount'    = 0
            'collaborationEvents' = 0
        }
        $this.Validation = [GSCValidationResult]::new()
    }

    # Add participant
    [void] AddParticipant([string] $participantId) {
        if (-not [string]::IsNullOrWhiteSpace($participantId) -and $participantId -notin $this.Participants) {
            $this.Participants += $participantId
            $this.Metrics['participantCount'] = $this.Participants.Count
        }
    }

    # Add command to session
    [void] AddCommand([string] $commandId) {
        if (-not [string]::IsNullOrWhiteSpace($commandId) -and $commandId -notin $this.Commands) {
            $this.Commands += $commandId
            $this.Metrics['commandsExecuted'] = $this.Commands.Count
        }
    }

    # Start session
    [void] Start() {
        if ($this.Status -eq 'created') {
            $this.Status = 'active'
            $this.Started = Get-Date
        }
    }

    # End session
    [void] End() {
        if ($this.Status -eq 'active') {
            $this.Status = 'completed'
            $this.Ended = Get-Date
            $this.CalculateFinalMetrics($this.Started, $this.Ended, $this.Commands)
        }
    }

    # Calculate final session metrics
    [void] CalculateFinalMetrics([datetime] $started, [datetime] $ended, [string[]] $commands) {
        if ($started -ne [datetime]::MinValue -and $ended -ne [datetime]::MinValue) {
            $duration = $ended - $started
            $this.Metrics['durationMinutes'] = [math]::Round($duration.TotalMinutes, 2)
            $this.Metrics['commandsPerHour'] = if ($duration.TotalHours -gt 0) {
                [math]::Round($commands.Count / $duration.TotalHours, 2)
            }
            else { 0 }
        }
    }

    # Validate session state
    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.SessionId) -and
        -not [string]::IsNullOrWhiteSpace($this.Title) -and
        $this.Created -ne [datetime]::MinValue -and
        $null -ne $this.Participants -and $null -ne $this.Commands -and
        $null -ne $this.Collaboration -and $null -ne $this.Metrics
    }

    # Convert to JSON
    [string] ToJson() {
        return $this | ConvertTo-Json -Depth 10 -Compress
    }

    # Create from JSON
    static [GSCSession] FromJson([string] $json) {
        try {
            $data = $json | ConvertFrom-Json
            $session = [GSCSession]::new($data.Title)
            $session.SessionId = $data.SessionId
            $session.Status = $data.Status
            $session.Created = [datetime]$data.Created
            if ($data.Started -and $data.Started -ne [datetime]::MinValue) {
                $session.Started = [datetime]$data.Started
            }
            if ($data.Ended -and $data.Ended -ne [datetime]::MinValue) {
                $session.Ended = [datetime]$data.Ended
            }
            if ($data.Participants) { $session.Participants = $data.Participants }
            if ($data.Commands) { $session.Commands = $data.Commands }
            if ($data.Collaboration) { $session.Collaboration = $data.Collaboration }
            if ($data.Metrics) { $session.Metrics = $data.Metrics }
            return $session
        }
        catch {
            throw [InvalidOperationException]::new("Failed to deserialize GSCSession from JSON: $($_.Exception.Message)")
        }
    }
}

# GSC Memory Integration Entity - Memory pattern integration
class GSCMemoryIntegration {
    [string] $MemoryId
    [string] $Title
    [string] $Type
    [string] $Status
    [datetime] $Created
    [datetime] $LastSync
    [hashtable] $MemoryFiles
    [hashtable] $SyncStatus
    [hashtable] $UsageAnalytics
    [GSCValidationResult] $Validation

    # Constructor
    GSCMemoryIntegration([string] $title, [string] $type) {
        if ([string]::IsNullOrWhiteSpace($title)) {
            throw [ArgumentException]::new("Title cannot be null or empty", "title")
        }
        if ([string]::IsNullOrWhiteSpace($type)) {
            throw [ArgumentException]::new("Type cannot be null or empty", "type")
        }

        $this.MemoryId = "mem-" + [Guid]::NewGuid().ToString("N").Substring(0, 8)
        $this.Title = $title
        $this.Type = $type
        $this.Status = 'initialized'
        $this.Created = Get-Date
        $this.LastSync = [datetime]::MinValue
        $this.MemoryFiles = @{}
        $this.SyncStatus = @{
            'lastSyncAttempt'     = [datetime]::MinValue
            'syncConflicts'       = @()
            'crossPlatformStatus' = @{}
        }
        $this.UsageAnalytics = @{
            'accessCount'    = 0
            'lastAccessed'   = [datetime]::MinValue
            'patternMatches' = 0
        }
        $this.Validation = [GSCValidationResult]::new()
    }

    # Add memory file
    [void] AddMemoryFile([string] $filePath, [string] $status = 'missing') {
        if (-not [string]::IsNullOrWhiteSpace($filePath)) {
            $this.MemoryFiles[$filePath] = @{
                'Status'       = $status
                'LastModified' = Get-Date
                'Size'         = 0
            }
        }
    }

    # Update sync status
    [void] UpdateSyncStatus([bool] $success, [string[]] $conflicts = @()) {
        $this.LastSync = Get-Date
        $this.SyncStatus['lastSyncAttempt'] = Get-Date
        $this.SyncStatus['syncConflicts'] = $conflicts
        if ($success) {
            $this.Status = 'synchronized'
        }
        else {
            $this.Status = 'sync-failed'
        }
    }

    # Record usage
    [void] RecordUsage([string] $pattern = '') {
        $this.UsageAnalytics['accessCount']++
        $this.UsageAnalytics['lastAccessed'] = Get-Date
        if (-not [string]::IsNullOrWhiteSpace($pattern)) {
            $this.UsageAnalytics['patternMatches']++
        }
    }

    # Validate memory integration state
    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.MemoryId) -and
        -not [string]::IsNullOrWhiteSpace($this.Title) -and
        -not [string]::IsNullOrWhiteSpace($this.Type) -and
        $this.Created -ne [datetime]::MinValue -and
        $null -ne $this.MemoryFiles -and $null -ne $this.SyncStatus -and
        $null -ne $this.UsageAnalytics
    }

    # Convert to JSON
    [string] ToJson() {
        return $this | ConvertTo-Json -Depth 10 -Compress
    }

    # Create from JSON
    static [GSCMemoryIntegration] FromJson([string] $json) {
        try {
            $data = $json | ConvertFrom-Json
            $memory = [GSCMemoryIntegration]::new($data.Title, $data.Type)
            $memory.MemoryId = $data.MemoryId
            $memory.Status = $data.Status
            $memory.Created = [datetime]$data.Created
            if ($data.LastSync -and $data.LastSync -ne [datetime]::MinValue) {
                $memory.LastSync = [datetime]$data.LastSync
            }
            if ($data.MemoryFiles) { $memory.MemoryFiles = $data.MemoryFiles }
            if ($data.SyncStatus) { $memory.SyncStatus = $data.SyncStatus }
            if ($data.UsageAnalytics) { $memory.UsageAnalytics = $data.UsageAnalytics }
            return $memory
        }
        catch {
            throw [InvalidOperationException]::new("Failed to deserialize GSCMemoryIntegration from JSON: $($_.Exception.Message)")
        }
    }
}

#endregion Core Entity Models

# Classes and types are available for dot-sourcing

Write-Host "[GSC] Entity models loaded successfully - Manufacturing-grade validation enabled" -ForegroundColor Green
