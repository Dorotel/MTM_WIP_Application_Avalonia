# GSC Cross-Platform Utilities Module
# Date: September 28, 2025
# Purpose: Cross-platform compatibility utilities for GSC Enhancement System

<#
.SYNOPSIS
Cross-Platform Utilities for GSC Enhancement System

.DESCRIPTION
Provides cross-platform compatibility utilities including:
- Platform detection and adaptation
- File path handling across Windows, macOS, and Linux
- Shell integration and command execution
- Environment variable management
- PowerShell Core feature detection

.NOTES
Requires PowerShell Core 7.0+ for full cross-platform functionality
Designed to work with Git Bash wrappers and native PowerShell execution
#>

# Module metadata
$script:CrossPlatformModuleVersion = "1.0.0"
$script:CrossPlatformModuleDate = "2025-09-28"

# Platform capability matrix
$script:PlatformCapabilities = @{
    "windows" = @{
        NativeShell               = "PowerShell"
        AlternativeShells         = @("cmd", "git-bash")
        PathSeparator             = "\\"
        LineEnding                = "`r`n"
        EnvironmentVariablePrefix = "$env:"
        HomeVariable              = "USERPROFILE"
        UserVariable              = "USERNAME"
        PowerShellExecutable      = "pwsh.exe"
        ShellWrapperExtension     = ".sh"
    }
    "macos"   = @{
        NativeShell               = "bash"
        AlternativeShells         = @("zsh", "fish")
        PathSeparator             = "/"
        LineEnding                = "`n"
        EnvironmentVariablePrefix = "$env:"
        HomeVariable              = "HOME"
        UserVariable              = "USER"
        PowerShellExecutable      = "pwsh"
        ShellWrapperExtension     = ".sh"
    }
    "linux"   = @{
        NativeShell               = "bash"
        AlternativeShells         = @("zsh", "fish", "sh")
        PathSeparator             = "/"
        LineEnding                = "`n"
        EnvironmentVariablePrefix = "$env:"
        HomeVariable              = "HOME"
        UserVariable              = "USER"
        PowerShellExecutable      = "pwsh"
        ShellWrapperExtension     = ".sh"
    }
}

<#
.SYNOPSIS
Detect current platform with detailed capabilities

.DESCRIPTION
Provides comprehensive platform detection including OS type,
shell environment, and available capabilities

.RETURNS
Platform information object with capabilities
#>
function Get-PlatformInfo {
    [CmdletBinding()]
    param()

    # Detect platform using multiple methods for reliability
    $platform = "unknown"

    if ($IsWindows -or ($env:OS -eq "Windows_NT") -or ($env:USERPROFILE -and -not $env:HOME)) {
        $platform = "windows"
    }
    elseif ($IsMacOS -or ($env:HOME -and (Test-Path "/System/Library/CoreServices/SystemVersion.plist"))) {
        $platform = "macos"
    }
    elseif ($IsLinux -or ($env:HOME -and -not (Test-Path "/System/Library/CoreServices/SystemVersion.plist"))) {
        $platform = "linux"
    }

    # Get platform capabilities
    $capabilities = if ($script:PlatformCapabilities.ContainsKey($platform)) {
        $script:PlatformCapabilities[$platform]
    }
    else {
        @{}
    }

    # Detect execution environment
    $executionEnvironment = "powershell"
    if ($env:TERM_PROGRAM -eq "vscode") {
        $executionEnvironment = "vscode-terminal"
    }
    elseif ($env:SHELL -and ($env:SHELL -like "*bash*" -or $env:SHELL -like "*zsh*")) {
        $executionEnvironment = "unix-shell"
    }
    elseif ($env:MSYSTEM -or $env:MINGW_PREFIX) {
        $executionEnvironment = "git-bash"
    }

    return @{
        Platform             = $platform
        Capabilities         = $capabilities
        ExecutionEnvironment = $executionEnvironment
        PowerShellVersion    = $PSVersionTable.PSVersion.ToString()
        PowerShellEdition    = $PSVersionTable.PSEdition
        OperatingSystem      = $PSVersionTable.OS
        HomeDirectory        = Get-HomeDirectory
        UserName             = Get-UserName
        WorkingDirectory     = (Get-Location).Path
        PathSeparator        = Get-PathSeparator
        IsElevated           = Test-ElevatedPermissions
    }
}

<#
.SYNOPSIS
Get cross-platform home directory

.DESCRIPTION
Returns the user's home directory using platform-appropriate variables

.RETURNS
Home directory path
#>
function Get-HomeDirectory {
    [CmdletBinding()]
    param()

    return $env:HOME ?? $env:USERPROFILE ?? (Join-Path $env:HOMEDRIVE $env:HOMEPATH)
}

<#
.SYNOPSIS
Get cross-platform user name

.DESCRIPTION
Returns the current user name using platform-appropriate variables

.RETURNS
User name string
#>
function Get-UserName {
    [CmdletBinding()]
    param()

    return $env:USER ?? $env:USERNAME ?? $env:LOGNAME ?? "unknown"
}

<#
.SYNOPSIS
Get platform-appropriate path separator

.DESCRIPTION
Returns the correct path separator for the current platform

.RETURNS
Path separator character
#>
function Get-PathSeparator {
    [CmdletBinding()]
    param()

    return [System.IO.Path]::DirectorySeparatorChar
}

<#
.SYNOPSIS
Test for elevated/administrator permissions

.DESCRIPTION
Checks if the current PowerShell session has elevated permissions

.RETURNS
Boolean indicating elevated status
#>
function Test-ElevatedPermissions {
    [CmdletBinding()]
    param()

    try {
        if ($IsWindows -or ($env:OS -eq "Windows_NT")) {
            $currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
            return $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
        }
        else {
            # On Unix-like systems, check if running as root
            return (id -u) -eq 0
        }
    }
    catch {
        return $false
    }
}

<#
.SYNOPSIS
Join file paths using cross-platform approach

.DESCRIPTION
Joins file path components using the appropriate separator for the current platform

.PARAMETER Paths
Array of path components to join

.RETURNS
Joined file path
#>
function Join-PathCrossPlatform {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Paths
    )

    # Use PowerShell's built-in Join-Path which handles cross-platform correctly
    $result = $Paths[0]

    for ($i = 1; $i -lt $Paths.Length; $i++) {
        $result = Join-Path $result $Paths[$i]
    }

    return $result
}

<#
.SYNOPSIS
Convert file path to platform-appropriate format

.DESCRIPTION
Normalizes file paths to use correct separators and format for the current platform

.PARAMETER Path
File path to normalize

.RETURNS
Normalized file path
#>
function ConvertTo-PlatformPath {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    # Normalize path separators
    $separator = Get-PathSeparator
    $normalizedPath = $Path -replace '[/\\]', $separator

    # Handle tilde expansion on Unix-like systems
    if ($normalizedPath.StartsWith("~") -and ($env:HOME)) {
        $normalizedPath = $normalizedPath -replace "^~", $env:HOME
    }

    return $normalizedPath
}

<#
.SYNOPSIS
Test PowerShell Core availability and version

.DESCRIPTION
Checks if PowerShell Core 7.0+ is available and meets GSC requirements

.RETURNS
PowerShell compatibility information
#>
function Test-PowerShellCoreCompatibility {
    [CmdletBinding()]
    param()

    $compatibility = @{
        IsAvailable     = $false
        Version         = $null
        IsCompatible    = $false
        Issues          = @()
        Recommendations = @()
    }

    try {
        # Check current PowerShell version
        $version = $PSVersionTable.PSVersion
        $compatibility.Version = $version.ToString()
        $compatibility.IsAvailable = $true

        # Check version compatibility (GSC requires 7.0+)
        $requiredMajor = 7
        $requiredMinor = 0

        if ($version.Major -gt $requiredMajor -or
            ($version.Major -eq $requiredMajor -and $version.Minor -ge $requiredMinor)) {
            $compatibility.IsCompatible = $true
        }
        else {
            $compatibility.Issues += "PowerShell version $($version.ToString()) is below required 7.0+"
            $compatibility.Recommendations += "Install PowerShell Core 7.0 or later"
        }

        # Check for required features
        $requiredFeatures = @(
            "ConvertTo-Json",
            "ConvertFrom-Json",
            "Get-FileHash",
            "Test-Path",
            "Join-Path"
        )

        foreach ($feature in $requiredFeatures) {
            if (-not (Get-Command $feature -ErrorAction SilentlyContinue)) {
                $compatibility.Issues += "Required cmdlet not available: $feature"
                $compatibility.IsCompatible = $false
            }
        }

        # Check JSON handling capability
        try {
            $testObject = @{ test = "value" }
            $json = $testObject | ConvertTo-Json
            $restored = $json | ConvertFrom-Json

            if ($restored.test -ne "value") {
                $compatibility.Issues += "JSON serialization/deserialization not working correctly"
                $compatibility.IsCompatible = $false
            }
        }
        catch {
            $compatibility.Issues += "JSON handling test failed: $($_.Exception.Message)"
            $compatibility.IsCompatible = $false
        }
    }
    catch {
        $compatibility.Issues += "PowerShell compatibility check failed: $($_.Exception.Message)"
    }

    return $compatibility
}

<#
.SYNOPSIS
Generate shell wrapper script for GSC command

.DESCRIPTION
Creates a shell script wrapper that can invoke PowerShell scripts from Git Bash or other shells

.PARAMETER CommandName
Name of the GSC command

.PARAMETER PowerShellScriptPath
Path to the PowerShell script to wrap

.PARAMETER ShellScriptPath
Output path for the shell wrapper script

.RETURNS
Shell wrapper creation result
#>
function New-ShellWrapper {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$CommandName,

        [Parameter(Mandatory = $true)]
        [string]$PowerShellScriptPath,

        [Parameter(Mandatory = $true)]
        [string]$ShellScriptPath
    )

    try {
        # Generate shell wrapper content
        $wrapperContent = @"
#!/bin/bash
# GSC Shell Wrapper for $CommandName
# Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
# Purpose: Cross-platform shell wrapper for PowerShell GSC commands

# Check if PowerShell Core is available
if command -v pwsh &> /dev/null; then
    # Use PowerShell Core (preferred)
    pwsh -File "$PowerShellScriptPath" `$@
elif command -v powershell &> /dev/null; then
    # Fallback to Windows PowerShell (Windows only)
    powershell -File "$PowerShellScriptPath" `$@
else
    echo "Error: PowerShell Core 7.0+ is required for GSC commands"
    echo "Please install PowerShell Core from https://github.com/PowerShell/PowerShell"
    echo ""
    echo "Installation commands:"
    echo "  Windows: winget install Microsoft.PowerShell"
    echo "  macOS:   brew install powershell"
    echo "  Linux:   See https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux"
    exit 1
fi

# Propagate exit code from PowerShell script
exit `$?
"@

        # Write wrapper script
        $wrapperContent | Out-File -FilePath $ShellScriptPath -Encoding UTF8

        # Make script executable on Unix-like systems
        if (-not ($IsWindows -or ($env:OS -eq "Windows_NT"))) {
            try {
                Invoke-Expression "chmod +x `"$ShellScriptPath`""
            }
            catch {
                Write-Warning "Could not make shell wrapper executable: $($_.Exception.Message)"
            }
        }

        return @{
            Success              = $true
            CommandName          = $CommandName
            PowerShellScriptPath = $PowerShellScriptPath
            ShellScriptPath      = $ShellScriptPath
            IsExecutable         = Test-Path $ShellScriptPath
        }
    }
    catch {
        return @{
            Success              = $false
            Error                = $_.Exception.Message
            CommandName          = $CommandName
            PowerShellScriptPath = $PowerShellScriptPath
            ShellScriptPath      = $ShellScriptPath
        }
    }
}

<#
.SYNOPSIS
Execute cross-platform command with proper error handling

.DESCRIPTION
Executes commands across platforms with consistent error handling and output capture

.PARAMETER Command
Command to execute

.PARAMETER Arguments
Command arguments

.PARAMETER WorkingDirectory
Working directory for command execution

.RETURNS
Command execution result
#>
function Invoke-CrossPlatformCommand {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Command,

        [Parameter(Mandatory = $false)]
        [string[]]$Arguments = @(),

        [Parameter(Mandatory = $false)]
        [string]$WorkingDirectory = $null
    )

    $executionStart = Get-Date

    try {
        $processInfo = @{
            FileName               = $Command
            Arguments              = $Arguments -join " "
            UseShellExecute        = $false
            RedirectStandardOutput = $true
            RedirectStandardError  = $true
            CreateNoWindow         = $true
        }

        if ($WorkingDirectory) {
            $processInfo.WorkingDirectory = $WorkingDirectory
        }

        $process = Start-Process @processInfo -PassThru -Wait

        $standardOutput = if ($process.StandardOutput) {
            $process.StandardOutput.ReadToEnd()
        }
        else { "" }

        $standardError = if ($process.StandardError) {
            $process.StandardError.ReadToEnd()
        }
        else { "" }

        $executionEnd = Get-Date
        $executionTime = ($executionEnd - $executionStart).TotalSeconds

        return @{
            Success              = $process.ExitCode -eq 0
            ExitCode             = $process.ExitCode
            StandardOutput       = $standardOutput
            StandardError        = $standardError
            ExecutionTimeSeconds = $executionTime
            Command              = $Command
            Arguments            = $Arguments -join " "
        }
    }
    catch {
        return @{
            Success              = $false
            Error                = $_.Exception.Message
            ExitCode             = -1
            StandardOutput       = ""
            StandardError        = $_.Exception.Message
            ExecutionTimeSeconds = ((Get-Date) - $executionStart).TotalSeconds
            Command              = $Command
            Arguments            = $Arguments -join " "
        }
    }
}

# Export functions for use by GSC commands (only when loaded as a module)
if ($ExecutionContext.SessionState.Module) {
    Export-ModuleMember -Function @(
        "Get-PlatformInfo",
        "Get-HomeDirectory",
        "Get-UserName",
        "Get-PathSeparator",
        "Test-ElevatedPermissions",
        "Join-PathCrossPlatform",
        "ConvertTo-PlatformPath",
        "Test-PowerShellCoreCompatibility",
        "New-ShellWrapper",
        "Invoke-CrossPlatformCommand"
    )
}

# Module initialization
Write-Verbose "GSC Cross-Platform Utilities Module v$script:CrossPlatformModuleVersion loaded successfully"
