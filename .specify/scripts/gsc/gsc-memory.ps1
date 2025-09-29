#!/usr/bin/env pwsh
#
# GSC Memory Command - Memory File Management and Display
# Parameter-based GET/POST operations for memory file display and updates
# Date: September 28, 2025

param(
    [string]$Operation = "get",  # get, post, status, update
    [string]$FileType = "",     # avalonia-ui-memory, debugging-memory, memory, avalonia-custom-controls-memory
    [string]$Pattern = "",      # New pattern to add
    [bool]$ReplaceConflicting = $true,
    [string]$Context = "",
    [string]$WorkflowId = "",
    [string]$GSCExecutionContext = "powershell"
)# Import required modules
$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$CommonGSCPath = Join-Path (Split-Path -Parent $ScriptRoot) "powershell" "common-gsc.ps1"
$MemoryIntegrationPath = Join-Path (Split-Path -Parent $ScriptRoot) "powershell" "memory-integration.ps1"
$MemoryEntityPath = Join-Path (Split-Path -Parent $ScriptRoot) "powershell" "entities" "MemoryFileEntity.ps1"

if (Test-Path $CommonGSCPath) { . $CommonGSCPath }
if (Test-Path $MemoryIntegrationPath) { . $MemoryIntegrationPath }
if (Test-Path $MemoryEntityPath) { . $MemoryEntityPath }

# GSC Memory Command Implementation
function Invoke-GSCMemoryCommand {
    param(
        [string]$Operation,
        [string]$FileType,
        [string]$Pattern,
        [bool]$ReplaceConflicting,
        [string]$Context,
        [string]$WorkflowId,
        [string]$GSCExecutionContext
    )

    $startTime = Get-Date

    try {
        Write-Host "🧠 GSC Memory File Management" -ForegroundColor Magenta
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkMagenta        # Memory file paths based on platform
        $memoryBasePath = Get-MemoryFilePath
        $memoryFiles = @{
            "avalonia-ui-memory" = Join-Path $memoryBasePath "avalonia-ui-memory.instructions.md"
            "debugging-memory" = Join-Path $memoryBasePath "debugging-memory.instructions.md"
            "memory" = Join-Path $memoryBasePath "memory.instructions.md"
            "avalonia-custom-controls-memory" = Join-Path $memoryBasePath "avalonia-custom-controls-memory.instructions.md"
        }

        switch ($Operation.ToLower()) {
            "get" {
                return Invoke-MemoryGet -MemoryFiles $memoryFiles -FileType $FileType
            }
            "post" {
                return Invoke-MemoryPost -MemoryFiles $memoryFiles -FileType $FileType -Pattern $Pattern -Context $Context -ReplaceConflicting $ReplaceConflicting
            }
            "status" {
                return Invoke-MemoryStatus -MemoryFiles $memoryFiles
            }
            "update" {
                return Invoke-MemoryUpdate -MemoryFiles $memoryFiles -FileType $FileType -Pattern $Pattern -Context $Context
            }
            default {
                throw "Invalid operation: $Operation. Valid operations: get, post, status, update"
            }
        }

    }
    catch {
        $executionTime = ([int]((Get-Date) - $startTime).TotalSeconds)

        $errorResponse = @{
            "success" = $false
            "command" = "memory"
            "executionTime" = $executionTime
            "message" = "Memory operation failed: $($_.Exception.Message)"
            "error" = $_.Exception.Message
        }

        Write-Host "❌ Memory Operation Failed: $($_.Exception.Message)" -ForegroundColor Red
        return $errorResponse | ConvertTo-Json -Depth 10
    }
}

# GET operation - Display memory file contents and recommendations
function Invoke-MemoryGet {
    param(
        [hashtable]$MemoryFiles,
        [string]$FileType
    )

    Write-Host "📋 Memory File Status and Recommendations" -ForegroundColor Cyan

    $memoryFileStatuses = @()
    $totalPatterns = 0
    $integrityStatus = "valid"

    foreach ($type in $MemoryFiles.Keys) {
        $filePath = $MemoryFiles[$type]
        $entity = New-MemoryFileEntity -FilePath $filePath -FileType $type

        $status = @{
            "filePath" = $filePath
            "fileType" = $type
            "isValid" = $false
            "lastModified" = ""
            "patternCount" = 0
            "checksumHash" = ""
            "applicableToCommands" = $entity.IntegrationTriggers
        }

        if ($entity.LoadContent()) {
            $status.isValid = $true
            $status.lastModified = $entity.LastProcessed.ToString("yyyy-MM-ddTHH:mm:ssZ")
            $status.patternCount = $entity.PatternCount
            $status.checksumHash = $entity.ChecksumHash
            $totalPatterns += $entity.PatternCount

            Write-Host "✓ $type : $($entity.PatternCount) patterns" -ForegroundColor Green

            # Show specific file content if requested
            if ($FileType -and $type -eq $FileType) {
                Write-Host "📄 Content Preview ($type):" -ForegroundColor Yellow
                $lines = $entity.Content -split "`n" | Select-Object -First 10
                foreach ($line in $lines) {
                    Write-Host "   $line" -ForegroundColor Gray
                }
                if ($entity.Content -split "`n" | Measure-Object | Select-Object -ExpandProperty Count -gt 10) {
                    Write-Host "   ... (content truncated)" -ForegroundColor DarkGray
                }
            }
        } else {
            Write-Host "❌ $type : File not found or corrupted" -ForegroundColor Red
            $integrityStatus = "error"
        }

        $memoryFileStatuses += $status
    }

    # Generate recommendations based on current context
    $recommendations = @(
        "Consider updating avalonia-ui-memory patterns for current UI development",
        "Review debugging-memory patterns for systematic problem-solving",
        "Validate memory file integrity with checksum verification",
        "Apply universal development patterns from memory file"
    )

    $response = @{
        "success" = $true
        "command" = "memory"
        "executionTime" = 2
        "message" = "Memory status retrieved successfully"
        "memoryFiles" = $memoryFileStatuses
        "totalPatterns" = $totalPatterns
        "recommendationsForContext" = $recommendations
        "integrityStatus" = $integrityStatus
    }

    Write-Host "📊 Summary: $totalPatterns total patterns across $($memoryFileStatuses.Count) files" -ForegroundColor Cyan
    Write-Host "🎯 Integrity: $integrityStatus" -ForegroundColor $(if ($integrityStatus -eq "valid") { "Green" } else { "Red" })

    return $response | ConvertTo-Json -Depth 10
}

# POST operation - Update memory files with new patterns
function Invoke-MemoryPost {
    param(
        [hashtable]$MemoryFiles,
        [string]$FileType,
        [string]$Pattern,
        [string]$Context,
        [bool]$ReplaceConflicting
    )

    if (-not $FileType -or -not $Pattern) {
        throw "FileType and Pattern are required for POST operation"
    }

    if (-not $MemoryFiles.ContainsKey($FileType)) {
        throw "Invalid FileType: $FileType"
    }

    Write-Host "📝 Updating memory file: $FileType" -ForegroundColor Yellow

    $filePath = $MemoryFiles[$FileType]
    $entity = New-MemoryFileEntity -FilePath $filePath -FileType $FileType

    if (-not $entity.LoadContent()) {
        throw "Failed to load memory file: $filePath"
    }

    # Add new pattern to memory file
    $newSection = @"

## $Context

$Pattern
"@

    # Check for conflicting patterns if ReplaceConflicting is true
    if ($ReplaceConflicting) {
        # Simple conflict detection based on similar headings
        $existingContent = $entity.Content
        $contextPattern = [regex]::Escape($Context)
        if ($existingContent -match "##\s+$contextPattern") {
            Write-Host "⚠️ Replacing conflicting pattern in $Context" -ForegroundColor Yellow
            # Replace the existing section
            $entity.Content = $existingContent -replace "##\s+$contextPattern[\s\S]*?(?=##|\z)", "## $Context`n`n$Pattern`n"
        } else {
            # Append new section
            $entity.Content += $newSection
        }
    } else {
        # Simply append
        $entity.Content += $newSection
    }

    # Save updated content
    try {
        Set-Content -Path $filePath -Value $entity.Content -Encoding UTF8
        $entity.LoadContent() # Reload to update metadata

        Write-Host "✅ Pattern added to $FileType" -ForegroundColor Green

        $response = @{
            "success" = $true
            "command" = "memory"
            "executionTime" = 1
            "message" = "Memory pattern updated successfully"
            "updatedFile" = $FileType
            "newPatternCount" = $entity.PatternCount
            "context" = $Context
        }

        return $response | ConvertTo-Json -Depth 10

    } catch {
        throw "Failed to save memory file: $($_.Exception.Message)"
    }
}

# STATUS operation - Memory file integrity and status
function Invoke-MemoryStatus {
    param(
        [hashtable]$MemoryFiles
    )

    Write-Host "🔍 Memory File Integrity Check" -ForegroundColor Blue

    $statusResults = @()
    $overallStatus = "valid"

    foreach ($type in $MemoryFiles.Keys) {
        $filePath = $MemoryFiles[$type]
        $entity = New-MemoryFileEntity -FilePath $filePath -FileType $type

        $fileStatus = @{
            "fileType" = $type
            "exists" = Test-Path $filePath
            "readable" = $false
            "integrityValid" = $false
            "patternCount" = 0
            "lastModified" = ""
        }

        if ($fileStatus.exists) {
            if ($entity.LoadContent()) {
                $fileStatus.readable = $true
                $fileStatus.integrityValid = $entity.ValidateIntegrity()
                $fileStatus.patternCount = $entity.PatternCount
                $fileStatus.lastModified = $entity.LastProcessed.ToString("yyyy-MM-ddTHH:mm:ssZ")

                Write-Host "✓ $type - $($entity.PatternCount) patterns, integrity OK" -ForegroundColor Green
            } else {
                Write-Host "❌ $type - Failed to read file" -ForegroundColor Red
                $overallStatus = "error"
            }
        } else {
            Write-Host "⚠️ $type - File not found" -ForegroundColor Yellow
            $overallStatus = "warning"
        }

        $statusResults += $fileStatus
    }

    $response = @{
        "success" = $true
        "command" = "memory"
        "operation" = "status"
        "executionTime" = 1
        "message" = "Memory file status check completed"
        "overallStatus" = $overallStatus
        "fileStatuses" = $statusResults
        "summary" = @{
            "totalFiles" = $MemoryFiles.Count
            "validFiles" = ($statusResults | Where-Object { $_.integrityValid }).Count
            "totalPatterns" = ($statusResults | Measure-Object -Property patternCount -Sum).Sum
        }
    }

    Write-Host "📊 Overall Status: $overallStatus" -ForegroundColor $(if ($overallStatus -eq "valid") { "Green" } elseif ($overallStatus -eq "warning") { "Yellow" } else { "Red" })

    return $response | ConvertTo-Json -Depth 10
}

# UPDATE operation - Batch update patterns
function Invoke-MemoryUpdate {
    param(
        [hashtable]$MemoryFiles,
        [string]$FileType,
        [string]$Pattern,
        [string]$Context
    )

    # Alias for POST operation
    return Invoke-MemoryPost -MemoryFiles $MemoryFiles -FileType $FileType -Pattern $Pattern -Context $Context -ReplaceConflicting $true
}

# Helper function to get memory file base path
function Get-MemoryFilePath {
    # Cross-platform memory file path detection
    if ($IsWindows -or $env:OS -eq "Windows_NT") {
        return "$env:APPDATA\Code\User\prompts"
    } elseif ($IsMacOS) {
        return "$env:HOME/Library/Application Support/Code/User/prompts"
    } else {
        return "$env:HOME/.local/share/Code/User/prompts"
    }
}

# Execute the command based on parameters
$result = Invoke-GSCMemoryCommand -Operation $Operation -FileType $FileType -Pattern $Pattern -ReplaceConflicting $ReplaceConflicting -Context $Context -WorkflowId $WorkflowId -GSCExecutionContext $GSCExecutionContext

# Output results
Write-Output $result
