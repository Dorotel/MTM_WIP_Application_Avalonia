# MAMP MySQL Update Script for MTM WIP Application
# This script updates MAMP MySQL from 5.7.24 to 8.4.2 LTS
# Author: GitHub Copilot Assistant
# Date: September 4, 2025

param(
    [switch]$WhatIf,
    [string]$MAMPPath = "C:\MAMP",
    [string]$BackupPath = "C:\MAMP_MySQL_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')",
    [string]$LogFile = "MySQL_MAMP_Update_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"
)

$ErrorActionPreference = "Stop"
$LatestVersion = "8.4.2"
$MAMPDataPath = "$MAMPPath\db\mysql"
$MAMPMySQLPath = "$MAMPPath\bin\mysql"

function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "INFO",
        [ConsoleColor]$Color = "White"
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = "[$timestamp] [$Level] $Message"
    
    Write-Host $logEntry -ForegroundColor $Color
    Add-Content -Path $LogFile -Value $logEntry
}

function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Test-MAMPInstallation {
    Write-Log "Checking MAMP installation..." -Color Yellow
    
    if (-not (Test-Path $MAMPPath)) {
        throw "MAMP not found at $MAMPPath. Please install MAMP first."
    }
    
    if (-not (Test-Path "$MAMPPath\bin\mysql")) {
        throw "MAMP MySQL not found at $MAMPPath\bin\mysql"
    }
    
    Write-Log "✓ MAMP installation found" -Color Green
}

function Get-CurrentMAMPMySQLVersion {
    $mysqlBin = "$MAMPPath\bin\mysql\bin\mysql.exe"
    if (Test-Path $mysqlBin) {
        try {
            $versionOutput = & $mysqlBin --version 2>$null
            if ($versionOutput -match "Distrib (\d+\.\d+\.\d+)") {
                return $matches[1]
            }
        } catch {
            Write-Log "Could not determine MySQL version: $($_.Exception.Message)" -Color Yellow
        }
    }
    return "Unknown"
}

function Stop-MAMPServices {
    Write-Log "Stopping MAMP services..." -Color Yellow
    
    # Check if MAMP is running
    $mampProcesses = Get-Process -Name "MAMP*" -ErrorAction SilentlyContinue
    $mysqlProcesses = Get-Process -Name "mysql*" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*MAMP*" }
    $apacheProcesses = Get-Process -Name "httpd" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*MAMP*" }
    
    if ($mampProcesses -or $mysqlProcesses -or $apacheProcesses) {
        if (-not $WhatIf) {
            Write-Log "Warning: MAMP appears to be running. This may interfere with the update." -Color Yellow
            $response = Read-Host "Do you want to continue anyway? (y/N)"
            if ($response.ToLower() -ne "y") {
                throw "MAMP is running. Please stop MAMP and try again."
            }
        } else {
            Write-Log "WHAT-IF: Would warn about MAMP running and prompt user" -Color Magenta
        }
    } else {
        Write-Log "✓ MAMP is not running" -Color Green
    }
}

function Backup-MAMPDatabases {
    param([string]$BackupPath)
    
    Write-Log "Creating MAMP database backups..." -Color Yellow
    
    if (-not $WhatIf) {
        if (-not (Test-Path $BackupPath)) {
            New-Item -Path $BackupPath -ItemType Directory -Force | Out-Null
            Write-Log "Created backup directory: $BackupPath" -Color Green
        }
        
        # Backup MySQL data files
        Write-Log "Backing up MySQL data files..." -Color Yellow
        
        if (Test-Path $MAMPDataPath) {
            Copy-Item -Path $MAMPDataPath -Destination "$BackupPath\mysql_data" -Recurse -Force
            Write-Log "✓ MySQL data files backed up" -Color Green
        } else {
            Write-Log "Warning: MySQL data path not found: $MAMPDataPath" -Color Yellow
        }
        
        # Also backup the MySQL binaries
        Write-Log "Backing up MySQL binaries..." -Color Yellow
        
        if (Test-Path $MAMPMySQLPath) {
            Copy-Item -Path $MAMPMySQLPath -Destination "$BackupPath\mysql_bin" -Recurse -Force
            Write-Log "✓ MySQL binaries backed up" -Color Green
        }
        
        # Create restore instructions
        $restoreInstructions = @"
# MAMP MySQL Backup Restore Instructions
Created: $(Get-Date)
Original Version: $(Get-CurrentMAMPMySQLVersion)
Backup Location: $BackupPath

## To restore MySQL data:
1. Stop MAMP services
2. Remove current MySQL data: $MAMPDataPath
3. Copy restored data: Copy-Item "$BackupPath\mysql_data" "$MAMPDataPath" -Recurse -Force

## To restore MySQL binaries:
1. Stop MAMP services
2. Remove current MySQL binaries: $MAMPMySQLPath
3. Copy restored binaries: Copy-Item "$BackupPath\mysql_bin" "$MAMPMySQLPath" -Recurse -Force

## Verify restore:
1. Start MAMP
2. Test database connectivity
3. Check application functionality
"@
        
        $restoreInstructions | Out-File -FilePath "$BackupPath\RESTORE_INSTRUCTIONS.md" -Encoding UTF8
        Write-Log "✓ Restore instructions created" -Color Green
        
    } else {
        Write-Log "WHAT-IF: Would create backup directory at $BackupPath" -Color Magenta
        Write-Log "WHAT-IF: Would backup MySQL data from $MAMPDataPath" -Color Magenta
        Write-Log "WHAT-IF: Would backup MySQL binaries from $MAMPMySQLPath" -Color Magenta
    }
}

function Get-MySQLForMAMP {
    Write-Log "Downloading MySQL $LatestVersion for MAMP..." -Color Yellow
    
    # For MAMP, we need to download the MySQL ZIP archive, not the installer
    $downloadUrl = "https://dev.mysql.com/get/Downloads/MySQL-8.4/mysql-8.4.2-winx64.zip"
    $zipPath = "$env:TEMP\mysql-8.4.2-winx64.zip"
    
    try {
        # Remove old download if exists
        if (Test-Path $zipPath) {
            Remove-Item $zipPath -Force
        }
        
        if (-not $WhatIf) {
            Write-Log "Downloading MySQL ZIP archive..." -Color Yellow
            Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath -UseBasicParsing
            
            if (Test-Path $zipPath) {
                $fileSize = (Get-Item $zipPath).Length / 1MB
                Write-Log "Downloaded MySQL archive ($([math]::Round($fileSize, 2)) MB)" -Color Green
                return $zipPath
            } else {
                throw "Failed to download MySQL archive"
            }
        } else {
            Write-Log "WHAT-IF: Would download $downloadUrl to $zipPath" -Color Magenta
            return $zipPath
        }
        
    } catch {
        throw "Failed to download MySQL: $($_.Exception.Message)"
    }
}

function Install-MySQLToMAMP {
    param([string]$ZipPath)
    
    Write-Log "Installing MySQL $LatestVersion to MAMP..." -Color Yellow
    
    $tempExtractPath = "$env:TEMP\mysql-8.4.2-winx64"
    
    try {
        if (-not $WhatIf) {
            # Extract the ZIP file
            Write-Log "Extracting MySQL archive..." -Color Yellow
            if (Test-Path $tempExtractPath) {
                Remove-Item $tempExtractPath -Recurse -Force
            }
            
            Add-Type -AssemblyName System.IO.Compression.FileSystem
            [System.IO.Compression.ZipFile]::ExtractToDirectory($ZipPath, $env:TEMP)
            
            # Remove old MAMP MySQL installation
            if (Test-Path $MAMPMySQLPath) {
                Write-Log "Removing old MAMP MySQL installation..." -Color Yellow
                Remove-Item $MAMPMySQLPath -Recurse -Force
            }
            
            # Copy new MySQL to MAMP
            Write-Log "Installing new MySQL to MAMP..." -Color Yellow
            Copy-Item -Path $tempExtractPath -Destination $MAMPMySQLPath -Recurse -Force
            
            # Cleanup
            Remove-Item $tempExtractPath -Recurse -Force
            Remove-Item $ZipPath -Force
            
            Write-Log "✓ MySQL $LatestVersion installed to MAMP" -Color Green
            
        } else {
            Write-Log "WHAT-IF: Would extract $ZipPath to $tempExtractPath" -Color Magenta
            Write-Log "WHAT-IF: Would remove old MySQL from $MAMPMySQLPath" -Color Magenta
            Write-Log "WHAT-IF: Would copy new MySQL to $MAMPMySQLPath" -Color Magenta
        }
        
    } catch {
        throw "Failed to install MySQL to MAMP: $($_.Exception.Message)"
    }
}

function Update-MAMPConfiguration {
    Write-Log "Updating MAMP configuration for MySQL 8.4..." -Color Yellow
    
    $mampConfigPath = "$MAMPPath\conf\mysql"
    
    if (Test-Path $mampConfigPath) {
        $configFiles = @(
            "$mampConfigPath\my.ini",
            "$mampConfigPath\my.cnf"
        )
        
        foreach ($configFile in $configFiles) {
            if (Test-Path $configFile) {
                Write-Log "Updating MySQL configuration: $configFile" -Color Cyan
                
                if (-not $WhatIf) {
                    # Create backup of config file
                    Copy-Item $configFile "$configFile.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
                    
                    # Read current config
                    $config = Get-Content $configFile -Raw
                    
                    # Add MySQL 8.4 compatibility settings
                    $mysql8Config = @"

# MySQL 8.4 Compatibility Settings Added by Update Script
# $(Get-Date)

# Authentication for backward compatibility
default_authentication_plugin=mysql_native_password

# SQL Mode for compatibility
sql_mode=STRICT_TRANS_TABLES,NO_ZERO_DATE,NO_ZERO_IN_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION

# Character set for full UTF8 support
character-set-server=utf8mb4
collation-server=utf8mb4_unicode_ci

# Enable stored procedure creation
log_bin_trust_function_creators=1

# Performance optimizations
innodb_buffer_pool_size=256M
innodb_log_file_size=64M

"@
                    
                    # Append new settings to config
                    $config += $mysql8Config
                    
                    # Write updated config
                    $config | Out-File -FilePath $configFile -Encoding UTF8
                    
                    Write-Log "✓ Configuration updated: $configFile" -Color Green
                } else {
                    Write-Log "WHAT-IF: Would update configuration in $configFile" -Color Magenta
                }
            }
        }
    } else {
        Write-Log "MAMP MySQL configuration directory not found: $mampConfigPath" -Color Yellow
    }
}

function Test-MAMPMySQLInstallation {
    Write-Log "Testing MAMP MySQL installation..." -Color Yellow
    
    $mysqlBin = "$MAMPPath\bin\mysql\bin\mysql.exe"
    
    if (Test-Path $mysqlBin) {
        try {
            $versionOutput = & $mysqlBin --version
            Write-Log "MySQL version: $versionOutput" -Color Green
            
            if ($versionOutput -match "8\.4\.2") {
                Write-Log "✓ MySQL 8.4.2 successfully installed" -Color Green
                return $true
            } else {
                Write-Log "Warning: Expected MySQL 8.4.2 but got: $versionOutput" -Color Yellow
                return $false
            }
        } catch {
            Write-Log "Error testing MySQL installation: $($_.Exception.Message)" -Color Red
            return $false
        }
    } else {
        Write-Log "MySQL executable not found at: $mysqlBin" -Color Red
        return $false
    }
}

function New-UpdateSummary {
    $summaryContent = @"
# MAMP MySQL Update Summary
Updated: $(Get-Date)

## Update Details
- **Previous Version**: $(Get-CurrentMAMPMySQLVersion)
- **New Version**: MySQL $LatestVersion LTS
- **MAMP Path**: $MAMPPath
- **Backup Location**: $BackupPath

## Files Modified
- MAMP MySQL binaries: $MAMPPath\bin\mysql
- MAMP configuration: $MAMPPath\conf\mysql
- Database data: Preserved in $MAMPPath\db\mysql

## Next Steps
1. **Start MAMP**: Open MAMP and start Apache and MySQL services
2. **Test phpMyAdmin**: Verify database access through MAMP's phpMyAdmin
3. **Test MTM Application**: Run your MTM WIP Application and test database connectivity
4. **Run Compatibility Check**: Execute MySQL-Compatibility-Check-MAMP.ps1
5. **Monitor Logs**: Check MAMP logs for any startup issues

## MAMP Configuration Updated
- Authentication: Set to mysql_native_password for compatibility
- Character Set: Updated to utf8mb4 for full Unicode support
- SQL Mode: Configured for backward compatibility
- Stored Procedures: Enabled log_bin_trust_function_creators
- Performance: Basic optimizations added

## Connection String Validation
Your application connection strings should continue to work:
- **Host**: localhost
- **Port**: 3306 (default MAMP MySQL port)
- **Database**: mtm_wip_application / mtm_wip_application_test

## Troubleshooting
If MAMP won't start:
1. Check MAMP logs in `$MAMPPath\logs`
2. Verify port 3306 is not in use by other services
3. Restore from backup if needed: $BackupPath

## Support Files Created
- Log file: $LogFile
- Backup: $BackupPath
- This summary: MAMP_MySQL_Update_Summary.md

For any issues, refer to the backup and restore instructions in the backup directory.
"@

    $summaryFile = "MAMP_MySQL_Update_Summary_$(Get-Date -Format 'yyyyMMdd_HHmmss').md"
    if (-not $WhatIf) {
        $summaryContent | Out-File -FilePath $summaryFile -Encoding UTF8
        Write-Log "Update summary saved to: $summaryFile" -Color Green
    } else {
        Write-Log "WHAT-IF: Would create update summary in $summaryFile" -Color Magenta
    }
}

function Main {
    Write-Log "Starting MAMP MySQL update from 5.7.24 to $LatestVersion" -Color Cyan
    Write-Log "Log file: $LogFile" -Color Cyan
    Write-Log "MAMP Path: $MAMPPath" -Color Cyan
    
    if ($WhatIf) {
        Write-Log "WHAT-IF MODE: No changes will be made" -Color Magenta
    }
    
    try {
        # Pre-flight checks
        Write-Log "`n=== PRE-FLIGHT CHECKS ===" -Color Yellow
        
        if (-not (Test-Administrator)) {
            Write-Log "Warning: Not running as Administrator. Some operations may fail." -Color Yellow
        }
        
        Test-MAMPInstallation
        $currentVersion = Get-CurrentMAMPMySQLVersion
        Write-Log "Current MAMP MySQL version: $currentVersion" -Color Cyan
        
        if ($currentVersion -eq $LatestVersion) {
            Write-Log "MySQL $LatestVersion is already installed in MAMP" -Color Green
            return
        }
        
        # Stop MAMP services
        Write-Log "`n=== STOPPING MAMP SERVICES ===" -Color Yellow
        Stop-MAMPServices
        
        # Create backups
        Write-Log "`n=== CREATING BACKUPS ===" -Color Yellow
        Backup-MAMPDatabases -BackupPath $BackupPath
        
        # Download MySQL
        Write-Log "`n=== DOWNLOADING MYSQL $LatestVersion ===" -Color Yellow
        $zipPath = Get-MySQLForMAMP
        
        # Install MySQL to MAMP
        Write-Log "`n=== INSTALLING MYSQL TO MAMP ===" -Color Yellow
        Install-MySQLToMAMP -ZipPath $zipPath
        
        # Update configuration
        Write-Log "`n=== UPDATING MAMP CONFIGURATION ===" -Color Yellow
        Update-MAMPConfiguration
        
        # Test installation
        Write-Log "`n=== TESTING INSTALLATION ===" -Color Yellow
        if (-not $WhatIf) {
            $installSuccess = Test-MAMPMySQLInstallation
            
            if ($installSuccess) {
                Write-Log "`n✓ MAMP MySQL update completed successfully!" -Color Green
                Write-Log "Backup created at: $BackupPath" -Color Cyan
            } else {
                Write-Log "`n✗ Installation may have issues. Check the log file." -Color Red
            }
        } else {
            Write-Log "WHAT-IF: Would test MySQL installation" -Color Magenta
        }
        
        # Create summary
        Write-Log "`n=== GENERATING UPDATE SUMMARY ===" -Color Yellow
        New-UpdateSummary
        
        Write-Log "`n=== UPDATE PROCESS COMPLETED ===" -Color Green
        Write-Log "Next: Start MAMP and test your database connectivity" -Color Cyan
        Write-Log "Run MySQL-Compatibility-Check-MAMP.ps1 to verify everything is working" -Color Cyan
        
    } catch {
        Write-Log "`nERROR: $($_.Exception.Message)" -Level "ERROR" -Color Red
        Write-Log "Stack Trace: $($_.ScriptStackTrace)" -Level "ERROR" -Color Red
        Write-Log "`nUpdate failed. You may need to restore from backup: $BackupPath" -Color Yellow
        exit 1
    }
}

# Run the main function
try {
    Main
} catch {
    Write-Log "FATAL ERROR: $($_.Exception.Message)" -Level "ERROR" -Color Red
    exit 1
}
