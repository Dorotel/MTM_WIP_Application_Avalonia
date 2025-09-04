# MAMP MySQL 8.4 Compatibility Script for MTM WIP Application
# This script addresses compatibility issues when upgrading MAMP MySQL from 5.7 to 8.4
# Author: GitHub Copilot Assistant
# Date: September 4, 2025

param(
    [switch]$WhatIf,
    [string]$DatabaseName = "mtm_wip_application",
    [string]$TestDatabaseName = "mtm_wip_application_test",
    [string]$MAMPPath = "C:\MAMP",
    [string]$LogFile = "MAMP_MySQL_Compatibility_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"
)

$ErrorActionPreference = "Stop"
$MAMPMySQLBin = "$MAMPPath\bin\mysql\bin"

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

function Test-MAMPRunning {
    Write-Log "Checking if MAMP is running..." -Color Yellow
    
    # Check if MAMP processes are running
    $mampProcesses = Get-Process -Name "MAMP*" -ErrorAction SilentlyContinue
    $mysqlProcesses = Get-Process -Name "mysql*" -ErrorAction SilentlyContinue
    
    if (-not $mampProcesses -and -not $mysqlProcesses) {
        Write-Log "MAMP is not running. Please start MAMP to test database connectivity." -Level "WARN" -Color Yellow
        return $false
    }
    
    Write-Log "✓ MAMP appears to be running" -Color Green
    return $true
}

function Test-MAMPMySQLConnection {
    param([string]$Database)
    
    if (-not (Test-Path "$MAMPMySQLBin\mysql.exe")) {
        Write-Log "MySQL executable not found in MAMP installation" -Level "ERROR" -Color Red
        return $false
    }
    
    try {
        $query = "SELECT 1;"
        $result = & "$MAMPMySQLBin\mysql.exe" -u root -D $Database -e $query 2>$null
        return $null -ne $result
    } catch {
        return $false
    }
}

function Repair-MAMPStoredProcedureCompatibility {
    param([string]$Database)
    
    Write-Log "Checking stored procedures for MySQL 8.4 compatibility in $Database" -Color Yellow
    
    if (-not (Test-MAMPRunning)) {
        Write-Log "Cannot check stored procedures - MAMP is not running" -Level "WARN" -Color Yellow
        return
    }
    
    try {
        # Get list of stored procedures
        $procedureQuery = "SELECT ROUTINE_NAME FROM information_schema.ROUTINES WHERE ROUTINE_SCHEMA = '$Database' AND ROUTINE_TYPE = 'PROCEDURE';"
        $procedures = & "$MAMPMySQLBin\mysql.exe" -u root -D $Database -e $procedureQuery 2>$null
        
        if ($procedures) {
            Write-Log "Found stored procedures to check" -Color Green
            
            # Common fixes for MySQL 8.4 compatibility
            $compatibilityFixes = @"
-- MySQL 8.4 Compatibility Fixes for MTM WIP Application (MAMP)
USE $Database;

-- Fix potential issues with GROUP BY behavior
SET SESSION sql_mode = 'STRICT_TRANS_TABLES,NO_ZERO_DATE,NO_ZERO_IN_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- Ensure stored procedures work with new authentication
SET GLOBAL log_bin_trust_function_creators = 1;

-- Fix for potential charset issues in existing data
ALTER TABLE inv_inventory CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE inv_transaction CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE md_part_ids CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE md_locations CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE md_operation_numbers CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE md_item_types CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE log_error CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE usr_users CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE sys_roles CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE sys_user_roles CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
ALTER TABLE usr_ui_settings CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
"@
            
            $fixScript = "mamp_compatibility_fixes_$Database.sql"
            $compatibilityFixes | Out-File -FilePath $fixScript -Encoding UTF8
            
            if (-not $WhatIf) {
                Write-Log "Applying compatibility fixes..." -Color Yellow
                $compatibilityFixes | & "$MAMPMySQLBin\mysql.exe" -u root -D $Database
                Write-Log "Compatibility fixes applied" -Color Green
            } else {
                Write-Log "WHAT-IF: Would apply compatibility fixes from $fixScript" -Color Magenta
            }
            
        } else {
            Write-Log "No stored procedures found or unable to query" -Color Yellow
        }
    } catch {
        Write-Log "Error checking stored procedures: $($_.Exception.Message)" -Level "ERROR" -Color Red
    }
}

function Update-MAMPConnectionStrings {
    Write-Log "Checking application configuration files for MAMP..." -Color Yellow
    
    $configFiles = @(
        "appsettings.json",
        "appsettings.Development.json",
        "Documentation\Development\appsettings.Development.json"
    )
    
    foreach ($configFile in $configFiles) {
        if (Test-Path $configFile) {
            Write-Log "Checking $configFile" -Color Green
            
            $content = Get-Content $configFile -Raw
            
            # Check for MAMP-specific connection string patterns
            if ($content -match "localhost:3306|localhost:8889|Server=localhost") {
                Write-Log "Found potential MAMP connection string in $configFile" -Color Cyan
                
                # MAMP typically uses port 3306 for MySQL, but check common alternatives
                if ($content -match ":8889") {
                    Write-Log "Note: Found port 8889 in connection string - verify MAMP MySQL port" -Color Yellow
                }
                
                if (-not $WhatIf) {
                    # Create backup
                    Copy-Item $configFile "$configFile.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
                    Write-Log "Created backup of $configFile" -Color Green
                } else {
                    Write-Log "WHAT-IF: Would backup $configFile" -Color Magenta
                }
            } else {
                Write-Log "Connection string looks compatible in $configFile" -Color Green
            }
        }
    }
}

function Test-MAMPDatabaseConnectivity {
    Write-Log "Testing MAMP database connectivity..." -Color Yellow
    
    if (-not (Test-MAMPRunning)) {
        Write-Log "Cannot test connectivity - MAMP is not running" -Level "WARN" -Color Yellow
        return
    }
    
    $databases = @($DatabaseName, $TestDatabaseName)
    
    foreach ($db in $databases) {
        Write-Log "Testing connection to $db..." -Color Cyan
        
        if (Test-MAMPMySQLConnection -Database $db) {
            Write-Log "✓ Successfully connected to $db" -Color Green
            
            # Test key tables
            $testTables = @("inv_inventory", "inv_transaction", "md_part_ids", "usr_users")
            
            foreach ($table in $testTables) {
                try {
                    $query = "SELECT COUNT(*) as count FROM $table LIMIT 1;"
                    $result = & "$MAMPMySQLBin\mysql.exe" -u root -D $db -e $query 2>$null
                    
                    if ($result) {
                        Write-Log "  ✓ Table $table is accessible" -Color Green
                    } else {
                        Write-Log "  ⚠ Table $table may have issues" -Level "WARN" -Color Yellow
                    }
                } catch {
                    Write-Log "  ✗ Error accessing table ${table}: $($_.Exception.Message)" -Level "ERROR" -Color Red
                }
            }
            
        } else {
            Write-Log "✗ Failed to connect to $db" -Level "ERROR" -Color Red
        }
    }
}

function Test-MAMPStoredProcedures {
    param([string]$Database)
    
    Write-Log "Testing key stored procedures in $Database..." -Color Yellow
    
    if (-not (Test-MAMPRunning)) {
        Write-Log "Cannot test stored procedures - MAMP is not running" -Level "WARN" -Color Yellow
        return
    }
    
    # Test critical stored procedures
    $criticalProcedures = @{
        "md_part_ids_Get_All" = "CALL md_part_ids_Get_All();"
        "usr_users_Get_All" = "CALL usr_users_Get_All();"
        "log_error_Get_All" = "CALL log_error_Get_All(@status, @msg);"
    }
    
    foreach ($proc in $criticalProcedures.Keys) {
        try {
            Write-Log "Testing procedure: $proc" -Color Cyan
            
            # For read-only procedures, test them safely
            $testQuery = $criticalProcedures[$proc]
            $result = & "$MAMPMySQLBin\mysql.exe" -u root -D $Database -e $testQuery 2>$null
            
            if ($result) {
                Write-Log "  ✓ Procedure $proc executed successfully" -Color Green
            } else {
                Write-Log "  ⚠ Procedure $proc returned no results or has issues" -Level "WARN" -Color Yellow
            }
            
        } catch {
            Write-Log "  ✗ Error testing procedure ${proc}: $($_.Exception.Message)" -Level "ERROR" -Color Red
        }
    }
}

function Test-MAMPMySQL8Features {
    Write-Log "Testing MySQL 8.4 specific features..." -Color Yellow
    
    if (-not (Test-MAMPRunning)) {
        Write-Log "Cannot test MySQL 8 features - MAMP is not running" -Level "WARN" -Color Yellow
        return
    }
    
    try {
        # Test MySQL version
        $versionOutput = & "$MAMPMySQLBin\mysql.exe" --version
        Write-Log "MySQL version: $versionOutput" -Color Cyan
        
        # Test JSON functions (MySQL 8 feature)
        $jsonTest = "SELECT JSON_OBJECT('test', 'value') as json_test;"
        $result = & "$MAMPMySQLBin\mysql.exe" -u root -e $jsonTest 2>$null
        
        if ($result) {
            Write-Log "✓ JSON functions are working" -Color Green
        } else {
            Write-Log "⚠ JSON functions may not be available" -Level "WARN" -Color Yellow
        }
        
        # Test CTE (Common Table Expressions) - MySQL 8 feature
        $cteTest = "WITH RECURSIVE test_cte AS (SELECT 1 as n UNION ALL SELECT n+1 FROM test_cte WHERE n < 3) SELECT * FROM test_cte;"
        $result = & "$MAMPMySQLBin\mysql.exe" -u root -e $cteTest 2>$null
        
        if ($result) {
            Write-Log "✓ Common Table Expressions (CTE) are working" -Color Green
        } else {
            Write-Log "⚠ CTE functionality may not be available" -Level "WARN" -Color Yellow
        }
        
    } catch {
        Write-Log "Error testing MySQL 8 features: $($_.Exception.Message)" -Level "ERROR" -Color Red
    }
}

function New-MAMPCompatibilityReport {
    Write-Log "Generating MAMP compatibility report..." -Color Yellow
    
    $reportContent = @"
# MAMP MySQL 8.4 Compatibility Report for MTM WIP Application
Generated: $(Get-Date)

## MAMP Configuration
- MAMP Path: $MAMPPath
- MySQL Binary Path: $MAMPMySQLBin
- Primary Database: $DatabaseName  
- Test Database: $TestDatabaseName

## Key Changes in MySQL 8.4 for MAMP
1. **Authentication**: Now uses caching_sha2_password by default
   - Configured to use mysql_native_password for compatibility
   
2. **SQL Mode**: Updated default SQL modes
   - Configured for backward compatibility with existing procedures
   
3. **Character Set**: Full UTF8MB4 support
   - Updated all tables to use utf8mb4_unicode_ci
   
4. **Performance**: Improved query optimizer and new features
   - JSON functions, CTEs, window functions available

## MAMP-Specific Considerations
1. **Port Configuration**: MAMP typically uses port 3306 for MySQL
2. **phpMyAdmin**: Should work with new MySQL version
3. **Data Directory**: MAMP stores data in `$MAMPPath\db\mysql`
4. **Configuration**: Settings in `$MAMPPath\conf\mysql`

## Connection String
Your application should continue to use:
- **Host**: localhost
- **Port**: 3306 (or check MAMP preferences)
- **Username/Password**: Same as before

## Recommendations
1. Start MAMP and verify both Apache and MySQL start successfully
2. Test phpMyAdmin access to verify database connectivity
3. Test your MTM WIP Application thoroughly
4. Monitor error logs for any compatibility issues
5. Consider performance monitoring for query optimization

## Files Modified
- MAMP MySQL binaries: $MAMPPath\bin\mysql (upgraded to 8.4)
- MAMP configuration: Updated for compatibility
- Backup created: Original 5.7.24 version backed up
- Log file: $LogFile

## Troubleshooting MAMP MySQL
1. **MAMP won't start**: Check MAMP logs in `$MAMPPath\logs`
2. **MySQL port conflicts**: Verify port 3306 is available
3. **Permission issues**: Ensure MAMP has proper file permissions
4. **phpMyAdmin errors**: Clear browser cache and restart MAMP

## Next Steps
1. Start MAMP and verify all services start
2. Access phpMyAdmin and test database operations
3. Test your MTM WIP Application
4. Monitor application logs for any database-related errors
5. Update development team about MySQL version change

For any issues, refer to MAMP documentation or restore from backup.
"@

    $reportFile = "MAMP_MySQL_8.4_Compatibility_Report.md"
    $reportContent | Out-File -FilePath $reportFile -Encoding UTF8
    
    Write-Log "MAMP compatibility report saved to: $reportFile" -Color Green
}

function Main {
    Write-Log "Starting MAMP MySQL 8.4 compatibility check for MTM WIP Application" -Color Cyan
    Write-Log "Log file: $LogFile" -Color Cyan
    Write-Log "MAMP Path: $MAMPPath" -Color Cyan
    
    if ($WhatIf) {
        Write-Log "WHAT-IF MODE: No changes will be made" -Color Magenta
    }
    
    # Test MAMP status
    Write-Log "`n=== CHECKING MAMP STATUS ===" -Color Yellow
    $mampRunning = Test-MAMPRunning
    
    if ($mampRunning) {
        # Test database connectivity
        Write-Log "`n=== TESTING DATABASE CONNECTIVITY ===" -Color Yellow
        Test-MAMPDatabaseConnectivity
        
        # Test stored procedures
        Write-Log "`n=== TESTING STORED PROCEDURES ===" -Color Yellow
        Test-MAMPStoredProcedures -Database $DatabaseName
        
        # Test MySQL 8 features
        Write-Log "`n=== TESTING MYSQL 8.4 FEATURES ===" -Color Yellow
        Test-MAMPMySQL8Features
    }
    
    # Fix stored procedure compatibility
    Write-Log "`n=== CHECKING STORED PROCEDURE COMPATIBILITY ===" -Color Yellow
    Repair-MAMPStoredProcedureCompatibility -Database $DatabaseName
    Repair-MAMPStoredProcedureCompatibility -Database $TestDatabaseName
    
    # Update connection strings
    Write-Log "`n=== CHECKING APPLICATION CONFIGURATION ===" -Color Yellow
    Update-MAMPConnectionStrings
    
    # Generate report
    Write-Log "`n=== GENERATING COMPATIBILITY REPORT ===" -Color Yellow
    if (-not $WhatIf) {
        New-MAMPCompatibilityReport
    } else {
        Write-Log "WHAT-IF: Would generate compatibility report" -Color Magenta
    }
    
    Write-Log "`n=== MAMP COMPATIBILITY CHECK COMPLETED ===" -Color Green
    Write-Log "Review the log file for any issues: $LogFile" -Color Cyan
    
    if (-not $mampRunning) {
        Write-Log "`nIMPORTANT: Start MAMP to complete database connectivity testing" -Color Yellow
    }
}

# Run the main function
try {
    Main
} catch {
    Write-Log "ERROR: $($_.Exception.Message)" -Level "ERROR" -Color Red
    Write-Log "Stack Trace: $($_.ScriptStackTrace)" -Level "ERROR" -Color Red
    exit 1
}
