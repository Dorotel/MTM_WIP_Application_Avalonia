# GSC Entity Models Contract Test
# Date: September 28, 2025
# Purpose: Comprehensive validation of GSC entity models and factory functions
# CRITICAL: This test validates our Phase 3.3 implementation is working correctly

<#
.SYNOPSIS
GSC entity models contract validation test

.DESCRIPTION
Tests all GSC entity models, factory functions, validation utilities,
and serialization capabilities. Validates manufacturing-grade reliability
requirements and cross-platform compatibility.

This test validates our TDD contracts from Phase 3.2 are being satisfied.
#>

# Import GSC entity models and factory
# PSScriptRoot is at tests\GSC\unit, need to go up 3 levels to reach workspace root
$workspaceRoot = (Split-Path -Path (Split-Path -Path (Split-Path -Path $PSScriptRoot -Parent) -Parent) -Parent)
$factoryScript = Join-Path -Path $workspaceRoot -ChildPath ".specify\scripts\powershell\gsc-entity-factory.ps1"

if (-not (Test-Path $factoryScript)) {
    Write-Host "Debug: PSScriptRoot = $PSScriptRoot"
    Write-Host "Debug: WorkspaceRoot = $workspaceRoot"
    Write-Host "Debug: FactoryScript = $factoryScript"
    throw "Factory script not found at: $factoryScript"
}

. $factoryScript

Describe "GSC Entity Models Contract Tests" {
    BeforeAll {
        # Test setup - entity models should now exist and work
        Write-Host "[TDD-Validation] Testing GSC Entity Models Phase 3.3 Implementation" -ForegroundColor Cyan
        $script:testDataPath = ".specify\state\test-data"

        # Ensure test directory exists
        if (-not (Test-Path $script:testDataPath)) {
            New-Item -Path $script:testDataPath -ItemType Directory -Force | Out-Null
        }
    }

    Context "GSC Command Entity Contract" {
        It "Should create valid GSC Command entities" {
            # Create command with all supported command types
            $commandTypes = @('constitution', 'specify', 'clarify', 'plan', 'task', 'analyze', 'implement', 'memory', 'validate', 'status', 'rollback', 'help')

            foreach ($cmdType in $commandTypes) {
                $command = New-GSCCommand -Command $cmdType -Parameters @{ Test = "Value" }

                # Validate command properties
                $command.Command | Should -Be $cmdType
                $command.Status | Should -Be 'queued'
                $command.CommandId | Should -Match '^gsc-[a-f0-9]{16}$'
                $command.Created | Should -BeOfType [datetime]
                $command.Parameters.Test | Should -Be "Value"

                # Validate entity validation
                $command.IsValid() | Should -Be $true
                Test-GSCEntity -Entity $command -ValidationLevel "comprehensive" | Should -Be $true
            }
        }

        It "Should handle command lifecycle operations" {
            $command = New-GSCCommand -Command "implement" -Parameters @{ Feature = "UserControl" }

            # Test start operation
            $command.Start()
            $command.Status | Should -Be 'running'
            $command.Started | Should -BeOfType [datetime]

            # Test complete operation
            $command.Complete($true, "Implementation successful")
            $command.Status | Should -Be 'completed'
            $command.Completed | Should -BeOfType [datetime]
            $command.Result.Success | Should -Be $true
            $command.Result.Output | Should -Be "Implementation successful"

            # Test duration calculation
            $duration = $command.GetDuration()
            $duration | Should -BeOfType [timespan]
            $duration.TotalSeconds | Should -BeGreaterThan 0
        }

        It "Should serialize and deserialize commands correctly" {
            $originalCommand = New-GSCCommand -Command "specify" -Parameters @{ Domain = "Manufacturing"; Priority = "High" }

            # Test JSON serialization
            $json = ConvertTo-GSCJson -Entity $originalCommand
            $json | Should -Not -BeNullOrEmpty
            $json | Should -Match '"Command":\s*"specify"'
            $json | Should -Match '"Domain":\s*"Manufacturing"'

            # Test JSON deserialization
            $deserializedCommand = ConvertFrom-GSCJson -Json $json -EntityType "GSCCommand"
            $deserializedCommand.Command | Should -Be $originalCommand.Command
            $deserializedCommand.CommandId | Should -Be $originalCommand.CommandId
            $deserializedCommand.Parameters.Domain | Should -Be "Manufacturing"
        }
    }

    Context "GSC Workflow Entity Contract" {
        It "Should create valid GSC Workflow entities" {
            $domains = @('manufacturing', 'ui', 'database', 'service', 'testing', 'general')
            $complexities = @('low', 'medium', 'high', 'critical')
            $priorities = @('low', 'medium', 'high', 'urgent')

            foreach ($domain in $domains) {
                $workflow = New-GSCWorkflow -Domain $domain -Complexity "medium" -Priority "high" -Technology @("avalonia", "dotnet8")

                # Validate workflow properties
                $workflow.WorkflowId | Should -Match '^wf-[a-f0-9]{16}$'
                $workflow.CurrentPhase | Should -Be 'analyze'
                $workflow.Status | Should -Be 'active'
                $workflow.Context.Domain | Should -Be $domain
                $workflow.Context.Complexity | Should -Be "medium"
                $workflow.Context.Priority | Should -Be "high"
                $workflow.Context.Technology | Should -Contain "avalonia"
                $workflow.Context.Technology | Should -Contain "dotnet8"

                # Validate entity validation
                $workflow.IsValid() | Should -Be $true
                Test-GSCEntity -Entity $workflow -ValidationLevel "comprehensive" | Should -Be $true
            }
        }

        It "Should manage workflow phases correctly" {
            $workflow = New-GSCWorkflow -Domain "manufacturing" -Complexity "high" -Priority "urgent"

            # Validate initial phase state
            $workflow.Phases.Keys | Should -Contain 'analyze'
            $workflow.Phases.Keys | Should -Contain 'design'
            $workflow.Phases.Keys | Should -Contain 'implement'
            $workflow.Phases.Keys | Should -Contain 'validate'
            $workflow.Phases.Keys | Should -Contain 'reflect'
            $workflow.Phases.Keys | Should -Contain 'handoff'

            # All phases should start as not_started
            $workflow.Phases['analyze'].Status | Should -Be 'not_started'
            $workflow.Phases['implement'].Status | Should -Be 'not_started'
        }

        It "Should manage workflow tasks correctly" {
            $workflow = New-GSCWorkflow -Domain "ui" -Complexity "medium" -Priority "high"
            $task1 = New-GSCTask -Id "T001" -Title "Create AXAML view" -Phase "implement" -Priority "high"
            $task2 = New-GSCTask -Id "T002" -Title "Create ViewModel" -Phase "implement" -Priority "medium" -Dependencies @("T001")

            # Add tasks to workflow
            $workflow.AddTask($task1)
            $workflow.AddTask($task2)

            # Validate task management
            $workflow.Tasks.Count | Should -Be 2
            $workflow.Tasks[0].Id | Should -Be "T001"
            $workflow.Tasks[1].Id | Should -Be "T002"

            # Test task filtering by phase
            $implementTasks = $workflow.GetTasksByPhase("implement")
            $implementTasks.Count | Should -Be 2

            # Test completion percentage
            $initialCompletion = $workflow.GetCompletionPercentage()
            $initialCompletion | Should -Be 0.0

            # Complete one task
            $task1.Complete($true)
            $completionAfterOne = $workflow.GetCompletionPercentage()
            $completionAfterOne | Should -Be 50.0
        }
    }

    Context "GSC Task Entity Contract" {
        It "Should create valid GSC Task entities" {
            $phases = @('analyze', 'design', 'implement', 'validate', 'reflect', 'handoff')
            $priorities = @('low', 'medium', 'high', 'critical')
            $assignees = @('human', 'ai', 'collaborative')

            foreach ($phase in $phases) {
                $task = New-GSCTask -Id "T100" -Title "Test Task" -Phase $phase -Priority "medium" -Assignee "ai"

                # Validate task properties
                $task.Id | Should -Be "T100"
                $task.Phase | Should -Be $phase
                $task.Priority | Should -Be "medium"
                $task.Status | Should -Be 'pending'
                $task.Assignee | Should -Be "ai"
                $task.Created | Should -BeOfType [datetime]

                # Validate entity validation
                $task.IsValid() | Should -Be $true
                Test-GSCEntity -Entity $task -ValidationLevel "comprehensive" | Should -Be $true
            }
        }

        It "Should handle task lifecycle operations" {
            $task = New-GSCTask -Id "T200" -Title "Lifecycle Test" -Phase "implement" -Priority "high" -EstimatedDuration 45

            # Test start operation
            $task.Start()
            $task.Status | Should -Be 'in_progress'
            $task.Started | Should -BeOfType [datetime]

            # Simulate some work time
            Start-Sleep -Milliseconds 100

            # Test complete operation
            $task.Complete($true)
            $task.Status | Should -Be 'completed'
            $task.Completed | Should -BeOfType [datetime]
            $task.ActualDuration | Should -BeGreaterThan 0
            $task.ActualDuration | Should -BeLessThan 1  # Should be very small (< 1 minute)
        }

        It "Should validate task dependencies correctly" {
            $task1 = New-GSCTask -Id "T301" -Title "Base Task" -Phase "design" -Priority "high"
            $task2 = New-GSCTask -Id "T302" -Title "Dependent Task" -Phase "implement" -Priority "medium" -Dependencies @("T301")
            $task3 = New-GSCTask -Id "T303" -Title "Independent Task" -Phase "implement" -Priority "low"

            $allTasks = @($task1, $task2, $task3)

            # T301 has no dependencies
            $task1.DependenciesSatisfied($allTasks) | Should -Be $true

            # T302 depends on T301 (not completed yet)
            $task2.DependenciesSatisfied($allTasks) | Should -Be $false

            # Complete T301
            $task1.Complete($true)
            $task2.DependenciesSatisfied($allTasks) | Should -Be $true

            # T303 has no dependencies
            $task3.DependenciesSatisfied($allTasks) | Should -Be $true
        }
    }

    Context "GSC Session Entity Contract" {
        It "Should create valid GSC Session entities" {
            $session = New-GSCSession -Objective "Implement manufacturing dashboard" -Scope "Avalonia UserControl with MVVM"

            # Validate session properties
            $session.SessionId | Should -Match '^ses-[a-f0-9]{16}$'
            $session.Status | Should -Be 'active'
            $session.Started | Should -BeOfType [datetime]
            $session.Participants.Count | Should -BeGreaterThan 0
            $session.Environment.Platform | Should -Not -BeNullOrEmpty
            $session.Environment.PowerShellVersion | Should -Not -BeNullOrEmpty

            # Validate entity validation
            $session.IsValid() | Should -Be $true
            Test-GSCEntity -Entity $session -ValidationLevel "comprehensive" | Should -Be $true
        }

        It "Should manage session participants correctly" {
            $session = New-GSCSession

            # Add participants
            $session.AddParticipant("human-developer", "human")
            $session.AddParticipant("system-monitor", "system")

            # Validate participants (should have AI + 2 new ones)
            $session.Participants.Count | Should -BeGreaterOrEqual 3

            $humanParticipant = $session.Participants | Where-Object { $_.Id -eq "human-developer" }
            $humanParticipant | Should -Not -BeNull
            $humanParticipant.Role | Should -Be "human"
            $humanParticipant.Joined | Should -BeOfType [datetime]
        }

        It "Should track session commands correctly" {
            $session = New-GSCSession
            $command1 = New-GSCCommand -Command "specify" -Parameters @{ Feature = "Dashboard" }
            $command2 = New-GSCCommand -Command "implement" -Parameters @{ Component = "UserControl" }

            # Add commands to session
            $session.AddCommand($command1)
            $session.AddCommand($command2)

            # Validate command tracking
            $session.Commands.Count | Should -Be 2
            $session.Metrics.CommandCount | Should -Be 2
            $session.Commands[0].Command | Should -Be "specify"
            $session.Commands[1].Command | Should -Be "implement"
        }
    }

    Context "GSC Memory Integration Entity Contract" {
        It "Should create valid GSC Memory Integration entities" {
            $memoryIntegration = New-GSCMemoryIntegration

            # Validate memory integration properties
            $memoryIntegration.MemoryId | Should -Match '^mem-[a-f0-9]{16}$'
            $memoryIntegration.Status | Should -Be 'unavailable'
            $memoryIntegration.Usage | Should -Not -BeNull
            $memoryIntegration.Synchronization | Should -Not -BeNull
            $memoryIntegration.Performance | Should -Not -BeNull

            # Validate entity validation
            $memoryIntegration.IsValid() | Should -Be $true
            Test-GSCEntity -Entity $memoryIntegration -ValidationLevel "comprehensive" | Should -Be $true
        }

        It "Should manage memory file tracking correctly" {
            $memoryIntegration = New-GSCMemoryIntegration
            $testFilePath = "$PSScriptRoot\..\..\.specify\scripts\powershell\gsc-entities.ps1"

            # Create memory file tracking
            $memoryFile = [GSCMemoryFile]::new($testFilePath)
            $memoryIntegration.AddMemoryFile("gsc-entities", $memoryFile)

            # Validate memory file tracking
            $memoryFile.Path | Should -Be $testFilePath
            $memoryFile.Status | Should -Be 'current'  # File should exist
            $memoryFile.Size | Should -BeGreaterThan 0
            $memoryFile.IsValid() | Should -Be $true

            $memoryIntegration.MemoryFiles["gsc-entities"] | Should -Not -BeNull
        }

        It "Should track memory usage analytics" {
            $memoryIntegration = New-GSCMemoryIntegration

            # Simulate memory usage
            $memoryIntegration.Usage.RecordAccess("avalonia-pattern", 0.15)
            $memoryIntegration.Usage.RecordAccess("mvvm-pattern", 0.08)
            $memoryIntegration.Usage.RecordAccess("avalonia-pattern", 0.12)

            # Validate usage analytics
            $memoryIntegration.Usage.TotalAccess | Should -Be 3
            $memoryIntegration.Usage.PatternHits["avalonia-pattern"] | Should -Be 2
            $memoryIntegration.Usage.PatternHits["mvvm-pattern"] | Should -Be 1
            $memoryIntegration.Usage.AverageResponseTime | Should -BeGreaterThan 0
            $memoryIntegration.Usage.LastUsed["avalonia-pattern"] | Should -BeOfType [datetime]
        }
    }

    Context "Entity Validation Framework Contract" {
        It "Should validate entities at different levels" {
            $command = New-GSCCommand -Command "validate"

            # Test validation levels
            Test-GSCEntity -Entity $command -ValidationLevel "basic" | Should -Be $true
            Test-GSCEntity -Entity $command -ValidationLevel "standard" | Should -Be $true
            Test-GSCEntity -Entity $command -ValidationLevel "comprehensive" | Should -Be $true
        }

        It "Should validate entity relationships correctly" {
            $workflow = New-GSCWorkflow -Domain "ui" -Complexity "medium" -Priority "high"
            $task1 = New-GSCTask -Id "T401" -Title "Task 1" -Phase "design" -Priority "high"
            $task2 = New-GSCTask -Id "T402" -Title "Task 2" -Phase "implement" -Priority "medium" -Dependencies @("T401")
            $command = New-GSCCommand -Command "implement"

            # Set relationships
            $workflow.AddTask($task1)
            $workflow.AddTask($task2)
            $command.Context.WorkflowId = $workflow.WorkflowId

            $entities = @($workflow, $task1, $task2, $command)

            # Test relationship validation
            Test-GSCEntityRelationships -Entities $entities | Should -Be $true
        }

        It "Should detect invalid entity relationships" {
            $task = New-GSCTask -Id "T501" -Title "Invalid Task" -Phase "implement" -Priority "high" -Dependencies @("T999")  # Non-existent dependency
            $command = New-GSCCommand -Command "implement"
            $command.Context.WorkflowId = "wf-nonexistent"  # Non-existent workflow

            $entities = @($task, $command)

            # Test invalid relationship detection
            Test-GSCEntityRelationships -Entities $entities | Should -Be $false
        }
    }

    Context "Entity Serialization Framework Contract" {
        It "Should serialize entities to valid JSON" {
            $command = New-GSCCommand -Command "specify" -Parameters @{ Test = "Serialization" }
            $workflow = New-GSCWorkflow -Domain "testing" -Complexity "low" -Priority "medium"

            # Test JSON serialization
            $commandJson = ConvertTo-GSCJson -Entity $command
            $workflowJson = ConvertTo-GSCJson -Entity $workflow -Compress

            $commandJson | Should -Not -BeNullOrEmpty
            $workflowJson | Should -Not -BeNullOrEmpty

            # Validate JSON structure
            $commandJson | Should -Match '"CommandId":'
            $commandJson | Should -Match '"Command":\s*"specify"'
            $workflowJson | Should -Match '"WorkflowId":'
            $workflowJson | Should -Match '"Domain":\s*"testing"'
        }

        It "Should deserialize JSON to valid entities" {
            $originalCommand = New-GSCCommand -Command "memory" -Parameters @{ Source = "Test" }
            $json = ConvertTo-GSCJson -Entity $originalCommand

            # Test deserialization
            $deserializedCommand = ConvertFrom-GSCJson -Json $json -EntityType "GSCCommand"

            $deserializedCommand | Should -Not -BeNull
            $deserializedCommand.Command | Should -Be "memory"
            $deserializedCommand.Parameters.Source | Should -Be "Test"
            $deserializedCommand.IsValid() | Should -Be $true
        }
    }

    Context "Entity State Persistence Contract" {
        It "Should save and restore entity state" {
            $command = New-GSCCommand -Command "status" -Parameters @{ Persistent = "True" }
            $testPath = "$($script:testDataPath)\test-command-persistence.json"

            # Test state saving
            $saveResult = Save-GSCEntityState -Entity $command -FilePath $testPath -BackupExisting
            $saveResult | Should -Be $true
            Test-Path $testPath | Should -Be $true

            # Test state restoration
            $restoredCommand = Restore-GSCEntityState -FilePath $testPath -EntityType "GSCCommand"
            $restoredCommand | Should -Not -BeNull
            $restoredCommand.Command | Should -Be "status"
            $restoredCommand.Parameters.Persistent | Should -Be "True"
            $restoredCommand.IsValid() | Should -Be $true
        }

        It "Should handle atomic write operations" {
            $workflow = New-GSCWorkflow -Domain "manufacturing" -Complexity "critical" -Priority "urgent"
            $testPath = "$($script:testDataPath)\test-workflow-atomic.json"

            # Test atomic write (should create temp file first, then move)
            $saveResult = Save-GSCEntityState -Entity $workflow -FilePath $testPath
            $saveResult | Should -Be $true

            # Temp file should not exist after successful operation
            Test-Path "$testPath.tmp" | Should -Be $false

            # Final file should exist and be valid
            Test-Path $testPath | Should -Be $true
            $content = Get-Content $testPath -Raw
            $content | Should -Not -BeNullOrEmpty
            $content | Should -Match '"WorkflowId":'
        }
    }

    AfterAll {
        # Cleanup test data
        if (Test-Path $script:testDataPath) {
            Remove-Item -Path $script:testDataPath -Recurse -Force -ErrorAction SilentlyContinue
        }

        Write-Host "[TDD-Validation] GSC Entity Models Phase 3.3 validation complete" -ForegroundColor Green
    }
}

# Mark test completion
Write-Host "[GSC] Entity Models Contract Test - Phase 3.3 TDD Validation Complete" -ForegroundColor Yellow
