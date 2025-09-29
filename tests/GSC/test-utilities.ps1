#!/usr/bin/env pwsh
#
# GSC Test Utilities
# Common functions and utilities for GSC integration and unit tests
# Provides test framework functions, assertions, and workspace management

# Test output formatting
function Write-TestHeader {
    param([string]$TestName)
    Write-Host "`n🧪 Starting Test: $TestName" -ForegroundColor Cyan
    Write-Host ("=" * 60) -ForegroundColor Cyan
}

function Write-TestStep {
    param([string]$StepDescription)
    Write-Host "`n📋 $StepDescription" -ForegroundColor Yellow
}

function Write-TestSuccess {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-TestError {
    param([string]$Message)
    Write-Host "❌ $Message" -ForegroundColor Red
}

# Test workspace management
function New-TestWorkspace {
    param([string]$Directory)
    $fullPath = Join-Path $PWD $Directory
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
    }
    return $fullPath
}

function Remove-TestWorkspace {
    param([string]$Directory)
    if (Test-Path $Directory) {
        Remove-Item -Path $Directory -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# Assertion functions
function Assert-Performance {
    param(
        [double]$ActualTime,
        [double]$MaxTime,
        [string]$Operation
    )
    if ($ActualTime -gt $MaxTime) {
        throw "Performance assertion failed: $Operation took ${ActualTime}s, expected < ${MaxTime}s"
    }
    Write-Host "⚡ Performance OK: $Operation completed in ${ActualTime}s (< ${MaxTime}s)" -ForegroundColor Green
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$ExpectedText,
        [string]$TestName
    )
    if ($Content -notmatch [regex]::Escape($ExpectedText)) {
        throw "Content assertion failed: '$TestName' - Expected text '$ExpectedText' not found"
    }
    Write-Host "✓ Content assertion passed: $TestName" -ForegroundColor Green
}

function Assert-NotNull {
    param(
        $Value,
        [string]$TestName
    )
    if ($null -eq $Value) {
        throw "Null assertion failed: '$TestName' - Value is null"
    }
    Write-Host "✓ Not-null assertion passed: $TestName" -ForegroundColor Green
}

function Assert-GreaterThanOrEqual {
    param(
        [double]$Value,
        [double]$Minimum,
        [string]$TestName
    )
    if ($Value -lt $Minimum) {
        throw "Comparison assertion failed: '$TestName' - Value $Value is less than minimum $Minimum"
    }
    Write-Host "✓ Comparison assertion passed: $TestName ($Value >= $Minimum)" -ForegroundColor Green
}

function Assert-FileExists {
    param(
        [string]$FilePath,
        [string]$TestName
    )
    if (-not (Test-Path $FilePath)) {
        throw "File existence assertion failed: '$TestName' - File '$FilePath' does not exist"
    }
    Write-Host "✓ File existence assertion passed: $TestName" -ForegroundColor Green
}

function Assert-DirectoryExists {
    param(
        [string]$DirectoryPath,
        [string]$TestName
    )
    if (-not (Test-Path $DirectoryPath -PathType Container)) {
        throw "Directory existence assertion failed: '$TestName' - Directory '$DirectoryPath' does not exist"
    }
    Write-Host "✓ Directory existence assertion passed: $TestName" -ForegroundColor Green
}

# GSC command execution helpers
function Invoke-GSCCommand {
    param(
        [string]$CommandName,
        [string[]]$Arguments = @(),
        [double]$TimeoutSeconds = 30
    )
    
    $commandPath = ".specify/scripts/gsc/gsc-$CommandName.ps1"
    if (-not (Test-Path $commandPath)) {
        throw "GSC command not found: $commandPath"
    }
    
    $startTime = Get-Date
    try {
        $result = & $commandPath @Arguments 2>&1
        $endTime = Get-Date
        $executionTime = ($endTime - $startTime).TotalSeconds
        
        return @{
            Success = $true
            Output = $result
            ExecutionTime = $executionTime
            Command = $CommandName
        }
    }
    catch {
        $endTime = Get-Date
        $executionTime = ($endTime - $startTime).TotalSeconds
        
        return @{
            Success = $false
            Error = $_.Exception.Message
            ExecutionTime = $executionTime
            Command = $CommandName
        }
    }
}

# Memory integration test helpers
function Test-MemoryFileAccess {
    param([string]$MemoryFilePath)
    
    if (-not (Test-Path $MemoryFilePath)) {
        return @{
            Accessible = $false
            Error = "Memory file not found: $MemoryFilePath"
        }
    }
    
    try {
        $content = Get-Content $MemoryFilePath -Raw
        $lineCount = ($content -split "`n").Count
        
        return @{
            Accessible = $true
            LineCount = $lineCount
            Size = $content.Length
        }
    }
    catch {
        return @{
            Accessible = $false
            Error = $_.Exception.Message
        }
    }
}

# JSON validation helpers
function Test-JsonOutput {
    param([string]$JsonString)
    
    try {
        $parsed = $JsonString | ConvertFrom-Json
        return @{
            Valid = $true
            Object = $parsed
        }
    }
    catch {
        return @{
            Valid = $false
            Error = $_.Exception.Message
        }
    }
}

# Cross-platform compatibility helpers
function Get-PlatformInfo {
    return @{
        OS = [System.Environment]::OSVersion.Platform
        PowerShellVersion = $PSVersionTable.PSVersion
        Architecture = [System.Environment]::Is64BitOperatingSystem
        WorkingDirectory = $PWD.Path
    }
}

Write-Host "📦 GSC Test Utilities Loaded" -ForegroundColor Cyan
