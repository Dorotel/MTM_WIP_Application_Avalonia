# GSC Memory Integration Module
# Date: September 28, 2025
# Purpose: Memory file reading, processing, and pattern integration for GSC commands

<#
.SYNOPSIS
Memory Integration Module for GSC Enhancement System

.DESCRIPTION
Provides comprehensive memory file integration capabilities including:
- Cross-platform memory file location detection
- Pattern extraction and analysis
- Checksum validation and integrity monitoring
- Memory pattern application to GSC workflows
- Performance optimization and caching

.NOTES
Requires PowerShell Core 7.0+ for cross-platform compatibility
Integrates with memory files created by remember.prompt.md system
#>

# Module metadata
$script:MemoryModuleVersion = "1.0.0"
$script:MemoryModuleDate = "2025-09-28"

# Memory file types and their characteristics
$script:MemoryFileTypes = @{
    "avalonia-ui-memory"              = @{
        Description        = "Avalonia UI framework patterns, MVVM binding, and cross-platform development lessons"
        Priority           = "high"
        ApplicableCommands = @("specify", "plan", "implement", "help")
        PatternTypes       = @("UI Layout", "MVVM Binding", "Cross-Platform", "Theme System")
    }
    "debugging-memory"                = @{
        Description        = "Debugging workflows, systematic problem-solving approaches, and troubleshooting patterns"
        Priority           = "high"
        ApplicableCommands = @("clarify", "analyze", "validate", "help")
        PatternTypes       = @("Problem Solving", "Error Analysis", "Root Cause", "Systematic Approach")
    }
    "memory"                          = @{
        Description        = "Universal development patterns, debugging workflows, and cross-project lessons learned"
        Priority           = "medium"
        ApplicableCommands = @("plan", "analyze", "implement", "help")
        PatternTypes       = @("Universal Patterns", "Architecture", "Best Practices", "Cross-Project")
    }
    "avalonia-custom-controls-memory" = @{
        Description        = "Avalonia custom control development patterns, null safety, and crash prevention strategies"
        Priority           = "medium"
        ApplicableCommands = @("task", "implement", "help")
        PatternTypes       = @("Custom Controls", "Null Safety", "Layout Management", "Performance")
    }
}

<#
.SYNOPSIS
Detect and validate memory file locations across platforms

.DESCRIPTION
Uses memory-paths.json configuration to locate memory files on current platform

.RETURNS
Memory file location information with validation status
#>
function Get-MemoryFileLocations {
    [CmdletBinding()]
    param()

    try {
        # Load memory paths configuration
        $configPath = ".specify/config/memory-paths.json"
        if (-not (Test-Path $configPath)) {
            throw "Memory paths configuration not found: $configPath"
        }

        $memoryConfig = Get-Content $configPath | ConvertFrom-Json

        # Detect current platform
        $platform = if ($IsWindows -or ($env:OS -eq "Windows_NT")) {
            "windows"
        }
        elseif ($IsMacOS -or ($env:HOME -and (Test-Path "/System/Library/CoreServices/SystemVersion.plist"))) {
            "macos"
        }
        else {
            "linux"
        }

        # Get platform-specific configuration
        $platformConfig = $memoryConfig.memoryFilePaths.$platform
        if (-not $platformConfig) {
            throw "Platform configuration not found for: $platform"
        }

        # Resolve base directory with user placeholder
        $baseDir = $platformConfig.baseDirectory
        $userName = $env:USER ?? $env:USERNAME
        $baseDir = $baseDir -replace "\{USER\}", $userName

        # Expand home directory if needed
        if ($baseDir.StartsWith("~")) {
            $homeDir = $env:HOME ?? $env:USERPROFILE
            $baseDir = $baseDir -replace "^~", $homeDir
        }

        # Build memory file locations
        $memoryFiles = @{}
        foreach ($fileType in $platformConfig.memoryFiles.PSObject.Properties) {
            $fileName = $fileType.Value
            $fullPath = Join-Path $baseDir $fileName

            $memoryFiles[$fileType.Name] = @{
                FilePath = $fullPath
                FileName = $fileName
                Exists   = Test-Path $fullPath
                FileType = $fileType.Name
                Metadata = $script:MemoryFileTypes[$fileType.Name]
            }
        }

        return @{
            Success       = $true
            Platform      = $platform
            BaseDirectory = $baseDir
            MemoryFiles   = $memoryFiles
            TotalFiles    = $memoryFiles.Count
            ExistingFiles = ($memoryFiles.Values | Where-Object { $_.Exists }).Count
        }
    }
    catch {
        return @{
            Success     = $false
            Error       = $_.Exception.Message
            Platform    = "unknown"
            MemoryFiles = @{}
        }
    }
}

<#
.SYNOPSIS
Read and validate memory file content

.DESCRIPTION
Reads memory file content with checksum validation and integrity checks

.PARAMETER FilePath
Path to the memory file to read

.PARAMETER FileType
Type of memory file being read

.RETURNS
Memory file content with validation results
#>
function Read-MemoryFile {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string]$FileType
    )

    $readStart = Get-Date

    try {
        if (-not (Test-Path $FilePath)) {
            return @{
                Success  = $false
                Error    = "Memory file not found: $FilePath"
                FileType = $FileType
                Content  = ""
            }
        }

        # Read file content
        $content = Get-Content $FilePath -Raw -Encoding UTF8

        # Calculate checksum for integrity validation
        $checksum = Get-FileHash -Path $FilePath -Algorithm SHA256

        # Get file metadata
        $fileInfo = Get-Item $FilePath

        # Extract patterns from content (simplified pattern extraction)
        $patterns = Extract-MemoryPatterns -Content $content -FileType $FileType

        $readEnd = Get-Date
        $readTime = ($readEnd - $readStart).TotalSeconds

        return @{
            Success         = $true
            FilePath        = $FilePath
            FileType        = $FileType
            Content         = $content
            Patterns        = $patterns
            Checksum        = $checksum.Hash
            LastModified    = $fileInfo.LastWriteTime
            FileSizeKB      = [math]::Round($fileInfo.Length / 1KB, 2)
            ReadTimeSeconds = $readTime
            IsValid         = $content.Length -gt 0 -and $patterns.Count -gt 0
        }
    }
    catch {
        return @{
            Success  = $false
            Error    = $_.Exception.Message
            FilePath = $FilePath
            FileType = $FileType
            Content  = ""
        }
    }
}

<#
.SYNOPSIS
Extract patterns from memory file content

.DESCRIPTION
Analyzes memory file content and extracts actionable patterns

.PARAMETER Content
Memory file content to analyze

.PARAMETER FileType
Type of memory file being analyzed

.RETURNS
Array of extracted patterns with metadata
#>
function Extract-MemoryPatterns {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Content,

        [Parameter(Mandatory = $true)]
        [string]$FileType
    )

    $patterns = @()

    # Split content into sections based on markdown headers
    $sections = $Content -split "(?m)^##\s+" | Where-Object { $_.Trim() -ne "" }

    foreach ($section in $sections) {
        $lines = $section -split "`n"
        $sectionTitle = $lines[0].Trim()

        # Skip metadata sections
        if ($sectionTitle -match "^(---|
.SYNOPSIS|.DESCRIPTION)") {
            continue
        }

        # Extract patterns from section content
        $sectionContent = ($lines[1..($lines.Length - 1)] -join "`n").Trim()

        if ($sectionContent.Length -gt 50) {
            # Minimum pattern length
            $pattern = @{
                Title              = $sectionTitle
                Content            = $sectionContent
                FileType           = $FileType
                PatternType        = Get-PatternType -Title $sectionTitle -FileType $FileType
                ApplicableCommands = $script:MemoryFileTypes[$FileType].ApplicableCommands
                Priority           = $script:MemoryFileTypes[$FileType].Priority
                WordCount          = ($sectionContent -split "\s+").Count
            }

            $patterns += $pattern
        }
    }

    return $patterns
}

<#
.SYNOPSIS
Determine pattern type from section title and file type

.DESCRIPTION
Categorizes memory patterns based on their content and context

.PARAMETER Title
Section title from memory file

.PARAMETER FileType
Memory file type

.RETURNS
Pattern type classification
#>
function Get-PatternType {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [string]$FileType
    )

    # Pattern type classification based on title keywords
    $patternMappings = @{
        "UI|Layout|Theme|AXAML|Control"          = "UI Development"
        "Debug|Error|Problem|Issue|Troubleshoot" = "Debugging"
        "Performance|Optimization|Memory|Speed"  = "Performance"
        "Architecture|Pattern|Design|Structure"  = "Architecture"
        "Test|Unit|Integration|Validation"       = "Testing"
        "MVVM|Binding|ViewModel|Property"        = "MVVM Pattern"
        "Cross-Platform|Platform|Compatibility"  = "Cross-Platform"
        "Custom Control|UserControl|Template"    = "Custom Controls"
    }

    foreach ($pattern in $patternMappings.Keys) {
        if ($Title -match $pattern) {
            return $patternMappings[$pattern]
        }
    }

    # Default to file type's primary pattern types
    $defaultTypes = $script:MemoryFileTypes[$FileType].PatternTypes
    return $defaultTypes[0]  # Return first/primary pattern type
}

<#
.SYNOPSIS
Load relevant memory patterns for GSC command

.DESCRIPTION
Loads and filters memory patterns relevant to the specified GSC command

.PARAMETER CommandName
Name of the GSC command requesting memory patterns

.PARAMETER MaxReadTime
Maximum time allowed for memory file reading (performance target)

.RETURNS
Filtered memory patterns relevant to the command
#>
function Get-RelevantMemoryPatterns {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CommandName,

        [Parameter(Mandatory = $false)]
        [double]$MaxReadTime = 5.0
    )

    $loadStart = Get-Date

    try {
        # Get memory file locations
        $memoryLocations = Get-MemoryFileLocations

        if (-not $memoryLocations.Success) {
            return @{
                Success         = $false
                Error           = $memoryLocations.Error
                Patterns        = @()
                LoadTimeSeconds = 0
            }
        }

        $allPatterns = @()
        $filesProcessed = 0
        $errorsEncountered = @()

        # Load patterns from relevant memory files
        foreach ($memoryFile in $memoryLocations.MemoryFiles.Values) {
            # Check if this memory file is applicable to the command
            if ($memoryFile.Metadata.ApplicableCommands -contains $CommandName) {
                $currentTime = ((Get-Date) - $loadStart).TotalSeconds

                # Performance check - don't exceed time limit
                if ($currentTime -ge $MaxReadTime * 0.8) {
                    Write-Warning "Approaching memory file read time limit. Enabling graceful degradation."
                    break
                }

                if ($memoryFile.Exists) {
                    $fileResult = Read-MemoryFile -FilePath $memoryFile.FilePath -FileType $memoryFile.FileType

                    if ($fileResult.Success) {
                        $allPatterns += $fileResult.Patterns
                        $filesProcessed++
                    }
                    else {
                        $errorsEncountered += "Failed to read $($memoryFile.FileType): $($fileResult.Error)"
                    }
                }
            }
        }

        # Sort patterns by priority and relevance
        $prioritizedPatterns = $allPatterns | Sort-Object @{
            Expression = { if ($_.Priority -eq "high") { 1 } else { 2 } }
        }, @{
            Expression = { $_.WordCount }
            Descending = $true
        }

        $loadEnd = Get-Date
        $totalLoadTime = ($loadEnd - $loadStart).TotalSeconds

        return @{
            Success             = $true
            CommandName         = $CommandName
            Patterns            = $prioritizedPatterns
            FilesProcessed      = $filesProcessed
            TotalPatterns       = $prioritizedPatterns.Count
            LoadTimeSeconds     = $totalLoadTime
            WithinTimeLimit     = $totalLoadTime -le $MaxReadTime
            Errors              = $errorsEncountered
            GracefulDegradation = $totalLoadTime -gt ($MaxReadTime * 0.8)
        }
    }
    catch {
        return @{
            Success         = $false
            Error           = $_.Exception.Message
            Patterns        = @()
            LoadTimeSeconds = ((Get-Date) - $loadStart).TotalSeconds
        }
    }
}

<#
.SYNOPSIS
Validate memory file integrity

.DESCRIPTION
Performs comprehensive integrity validation on memory files

.RETURNS
Integrity validation results for all memory files
#>
function Test-MemoryFileIntegrity {
    [CmdletBinding()]
    param()

    try {
        $memoryLocations = Get-MemoryFileLocations

        if (-not $memoryLocations.Success) {
            return @{
                Success           = $false
                Error             = $memoryLocations.Error
                ValidationResults = @()
            }
        }

        $validationResults = @()

        foreach ($memoryFile in $memoryLocations.MemoryFiles.Values) {
            $validation = @{
                FileType = $memoryFile.FileType
                FilePath = $memoryFile.FilePath
                Exists   = $memoryFile.Exists
                IsValid  = $false
                Issues   = @()
            }

            if ($memoryFile.Exists) {
                $fileResult = Read-MemoryFile -FilePath $memoryFile.FilePath -FileType $memoryFile.FileType

                if ($fileResult.Success) {
                    $validation.IsValid = $fileResult.IsValid
                    $validation.PatternCount = $fileResult.Patterns.Count
                    $validation.ChecksumHash = $fileResult.Checksum
                    $validation.LastModified = $fileResult.LastModified
                    $validation.FileSizeKB = $fileResult.FileSizeKB

                    # Validate content structure
                    if ($fileResult.Patterns.Count -eq 0) {
                        $validation.Issues += "No patterns found in memory file"
                    }

                    if ($fileResult.Content.Length -lt 100) {
                        $validation.Issues += "Memory file content too short"
                    }
                }
                else {
                    $validation.Issues += $fileResult.Error
                }
            }
            else {
                $validation.Issues += "Memory file does not exist"
            }

            $validationResults += $validation
        }

        $overallValid = ($validationResults | Where-Object { -not $_.IsValid }).Count -eq 0

        return @{
            Success           = $true
            OverallValid      = $overallValid
            ValidationResults = $validationResults
            TotalFiles        = $validationResults.Count
            ValidFiles        = ($validationResults | Where-Object { $_.IsValid }).Count
            Timestamp         = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        }
    }
    catch {
        return @{
            Success           = $false
            Error             = $_.Exception.Message
            ValidationResults = @()
        }
    }
}

# Export functions for use by GSC commands
Export-ModuleMember -Function @(
    "Get-MemoryFileLocations",
    "Read-MemoryFile",
    "Extract-MemoryPatterns",
    "Get-PatternType",
    "Get-RelevantMemoryPatterns",
    "Test-MemoryFileIntegrity"
)

# Module initialization
Write-Verbose "GSC Memory Integration Module v$script:MemoryModuleVersion loaded successfully"
