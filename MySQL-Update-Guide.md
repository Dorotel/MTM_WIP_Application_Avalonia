# MySQL Update Guide for MTM WIP Application (MAMP)

## Overview
This guide contains PowerShell scripts to update your MAMP MySQL installation from version 5.7.24 to MySQL 8.4.2 LTS while preserving your MTM WIP application database. Your MySQL runs through MAMP (MySQL, Apache, MySQL, PHP) development environment.

## Files Included

1. **Update-MySQL-MAMP.ps1** - MAMP-specific MySQL update script
2. **MySQL-Compatibility-Check-MAMP.ps1** - MAMP post-update compatibility checker  
3. **Update-MySQL.ps1** - Standard MySQL update script (for reference)
4. **MySQL-Compatibility-Check.ps1** - Standard compatibility checker (for reference)
5. **MySQL-Update-Guide.md** - This guide

## Prerequisites

### System Requirements
- Windows with PowerShell 5.1 or later
- MAMP installed at C:\MAMP
- At least 2GB free disk space  
- Current MAMP MySQL installation (5.7.24 detected)

### Before You Begin
1. **Stop MAMP services** - Close MAMP application and ensure Apache and MySQL are stopped
2. **Note your current MySQL root password** - Usually blank for MAMP default installation
3. **Verify MAMP installation** - Ensure MAMP is installed at C:\MAMP
4. **Backup your data** - Though the script creates backups, consider manual backups of important data

## Current Setup Detected
- **MAMP Installation**: C:\MAMP
- **MySQL Version**: 5.7.24 (Ver 14.14 Distrib 5.7.24, for Win64 (x86_64))
- **MySQL Data**: C:\MAMP\db\mysql
- **MySQL Binaries**: C:\MAMP\bin\mysql\bin\mysql.exe
- **Target Version**: MySQL 8.4.2 LTS

## Step-by-Step Instructions

### Step 1: Test Run (Recommended)
First, run the MAMP update script in "What-If" mode to see what would happen:

```powershell
# Open PowerShell (Administrator not required for MAMP)
cd "c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia"

# Run in test mode
.\Update-MySQL-MAMP.ps1 -WhatIf
```

### Step 2: Run the Full MAMP Update
If the test run looks good, run the actual MAMP MySQL update:

```powershell
# Full MAMP update with custom backup location
.\Update-MySQL-MAMP.ps1 -BackupPath "D:\MAMP_MySQL_Backup_$(Get-Date -Format 'yyyyMMdd')"

# Or use default backup location
.\Update-MySQL-MAMP.ps1
```

#### MAMP Update Parameters:
- `-WhatIf`: Preview mode - shows what would be done without making changes
- `-MAMPPath`: Specify MAMP installation path (default: C:\MAMP)
- `-BackupPath`: Specify custom backup location (default: C:\MAMP_MySQL_Backup_YYYYMMDD_HHMMSS)

### Step 3: Run MAMP Compatibility Check
After the MAMP MySQL update completes, run the MAMP-specific compatibility checker:

```powershell
# Check MAMP MySQL compatibility and fix issues
.\MySQL-Compatibility-Check-MAMP.ps1

# Or run in preview mode first  
.\MySQL-Compatibility-Check-MAMP.ps1 -WhatIf
```

### Step 4: Test MAMP and Your Application
1. **Start MAMP** - Open MAMP application and start Apache and MySQL services
2. **Test phpMyAdmin** - Access phpMyAdmin through MAMP to verify database access
3. **Test MTM WIP Application** - Run your application and test key functionality:
   - Database connectivity
   - Add inventory items
   - Run reports
   - Check user authentication
   - Verify stored procedures work correctly

## What the Scripts Do

### Update-MySQL-MAMP.ps1 (MAMP-Specific)
1. **MAMP Detection** - Verifies MAMP installation and current MySQL version
2. **Service Check** - Ensures MAMP services are stopped before update
3. **Database Backup** - Creates backups of MAMP MySQL data files and binaries
4. **Download & Extract** - Downloads MySQL 8.4.2 ZIP archive and extracts it
5. **Installation** - Replaces MAMP MySQL binaries with new version
6. **Configuration** - Updates MAMP MySQL configuration for compatibility
7. **Verification** - Tests the new MAMP MySQL installation

### MySQL-Compatibility-Check-MAMP.ps1 (MAMP-Specific)
1. **MAMP Status Check** - Verifies MAMP is running for database tests
2. **Connection Testing** - Tests MAMP MySQL database connectivity
3. **Stored Procedure Check** - Validates critical stored procedures work with MySQL 8.4
4. **MySQL 8.4 Features** - Tests new features like JSON functions and CTEs
5. **Character Set Update** - Converts tables to UTF8MB4 for full Unicode support
6. **Configuration Review** - Checks application config files for MAMP compatibility
7. **Report Generation** - Creates detailed MAMP compatibility report

### Standard Scripts (For Reference)
- **Update-MySQL.ps1** - For standard Windows MySQL installations
- **MySQL-Compatibility-Check.ps1** - For standard MySQL service installations

## Expected Changes

### MAMP MySQL Structure
- MySQL binaries updated from 5.7.24 to 8.4.2 in C:\MAMP\bin\mysql
- Character set updated to utf8mb4 for better Unicode support
- All existing data and stored procedures preserved in C:\MAMP\db\mysql
- No changes to table structure or relationships
- MAMP configuration updated for MySQL 8.4 compatibility

### Configuration Files
Your application's connection strings in these files will be checked:
- `appsettings.json`
- `appsettings.Development.json`
- `Documentation\Development\appsettings.Development.json`

### MAMP Service
- MAMP MySQL remains accessible through MAMP control panel
- Port remains 3306 (default MAMP MySQL port)
- All user accounts and permissions preserved
- phpMyAdmin continues to work with updated MySQL
- Apache integration remains unchanged

## Troubleshooting

### Common MAMP MySQL Issues and Solutions

#### Issue: "MAMP won't start MySQL after update"
**Solutions:** 
1. Check MAMP logs in `C:\MAMP\logs\mysql_error.log`
2. Verify MAMP configuration in `C:\MAMP\conf\mysql\my.ini`
3. Try restarting MAMP application as administrator
4. Check if port 3306 is available: `netstat -an | findstr 3306`

#### Issue: "phpMyAdmin shows errors after MySQL update"
**Solutions:**
1. Clear browser cache and restart MAMP
2. Check phpMyAdmin error logs in `C:\MAMP\logs\php_error.log`
3. Verify MySQL 8.4 compatibility in phpMyAdmin configuration

#### Issue: "MTM Application can't connect to MAMP MySQL"
**Solutions:**
1. Verify MAMP MySQL is running (green light in MAMP control panel)
2. Test connection manually: `C:\MAMP\bin\mysql\bin\mysql.exe -u root -p`
3. Update connection strings to ensure localhost:3306
4. Check authentication compatibility:
   ```sql
   ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY '';
   ```

#### Issue: "Stored procedures not working in MAMP MySQL 8.4"
**Solutions:**
1. Run the MAMP compatibility checker: `.\MySQL-Compatibility-Check-MAMP.ps1`
2. Check MAMP MySQL configuration for `log_bin_trust_function_creators=1`
3. Verify procedures in phpMyAdmin or MySQL command line

#### Issue: "Performance issues after MAMP MySQL update"
**Solutions:**
1. Update table statistics through phpMyAdmin or command line
2. Adjust MAMP MySQL memory settings in `C:\MAMP\conf\mysql\my.ini`
3. Consider increasing `innodb_buffer_pool_size` for better performance

### Getting Help
1. Check generated log files for detailed error information
2. Review the MAMP compatibility report: `MAMP_MySQL_8.4_Compatibility_Report.md`
3. Check MAMP logs in `C:\MAMP\logs\`
4. Consult MAMP documentation for MySQL-specific settings

## Rollback Plan

If you need to rollback MAMP MySQL to 5.7.24:

1. **Stop MAMP services** - Close MAMP application
2. **Restore from backup** - Use the backup created during update:
   ```powershell
   # Navigate to your backup directory
   cd "C:\MAMP_MySQL_Backup_YYYYMMDD_HHMMSS"
   
   # Stop MAMP if running
   # Remove current MySQL installation
   Remove-Item "C:\MAMP\bin\mysql" -Recurse -Force
   
   # Restore MySQL binaries
   Copy-Item "mysql_bin" "C:\MAMP\bin\mysql" -Recurse -Force
   
   # Restore MySQL data (if needed)
   Remove-Item "C:\MAMP\db\mysql" -Recurse -Force  
   Copy-Item "mysql_data" "C:\MAMP\db\mysql" -Recurse -Force
   ```

3. **Restart MAMP** - Start MAMP and verify MySQL 5.7.24 is running
4. **Test application** - Verify MTM WIP Application connects and functions normally

## Post-Update Checklist

- [ ] MAMP control panel shows MySQL as running (green light)
- [ ] phpMyAdmin loads and shows databases correctly
- [ ] MAMP MySQL version shows 8.4.2: `C:\MAMP\bin\mysql\bin\mysql.exe --version`
- [ ] Database connections work from MTM WIP Application
- [ ] All stored procedures execute correctly
- [ ] Inventory operations work (add, remove, transfer)
- [ ] User authentication functions
- [ ] Reports generate correctly
- [ ] Error logging works
- [ ] Backup old MySQL installation files safely stored

## Support Information

### Log Files Location
- MAMP Update log: `MySQL_MAMP_Update_YYYYMMDD_HHMMSS.log`
- MAMP Compatibility log: `MAMP_MySQL_Compatibility_YYYYMMDD_HHMMSS.log`
- MAMP MySQL error log: `C:\MAMP\logs\mysql_error.log`
- MAMP PHP error log: `C:\MAMP\logs\php_error.log`
- MAMP Apache error log: `C:\MAMP\logs\apache_error.log`

### Backup Files Location
Default: `C:\MAMP_MySQL_Backup_YYYYMMDD_HHMMSS\`
- `mysql_data\` - Complete MySQL data directory backup
- `mysql_bin\` - Original MySQL 5.7.24 binaries backup
- `RESTORE_INSTRUCTIONS.md` - Detailed restoration guide

### MAMP Configuration Files
- MAMP MySQL config: `C:\MAMP\conf\mysql\my.ini`
- MAMP PHP config: `C:\MAMP\conf\php\php.ini`
- MAMP Apache config: `C:\MAMP\conf\apache\httpd.conf`
- MySQL data directory: `C:\MAMP\db\mysql`
- MySQL binaries: `C:\MAMP\bin\mysql\bin\`

### MAMP MySQL Access
- **Command Line**: `C:\MAMP\bin\mysql\bin\mysql.exe -u root -p`
- **phpMyAdmin**: http://localhost/phpMyAdmin (when MAMP is running)
- **Default Port**: 3306
- **Default User**: root (usually no password in MAMP)

---

**Note:** Keep this guide and all backup files until you're confident the MAMP MySQL update is stable and your MTM WIP Application is working correctly with the new MySQL version.
