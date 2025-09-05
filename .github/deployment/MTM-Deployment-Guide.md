# MTM WIP Application - Deployment Documentation

## 📋 Overview

This document provides comprehensive deployment guidelines for the MTM WIP Application across different environments, ensuring consistent, secure, and reliable deployments in manufacturing environments.

## 🏗️ **Deployment Architecture**

### **Environment Strategy**
```
Production Environment (Manufacturing Floor)
├── Primary Database Server (MySQL 9.4.0)
├── Application Servers (2+ instances for redundancy)  
├── File Server (Reports, Logs, Backups)
└── Network Infrastructure (Isolated manufacturing network)

Staging Environment (Pre-Production Testing)
├── Staging Database Server (MySQL 9.4.0)
├── Staging Application Server
└── Testing Tools Integration

Development Environment (Developer Workstations)
├── Local Database (MySQL 9.4.0 or Docker)
├── Local Application Instance
└── Development Tools
```

## 🚀 **Production Deployment Guide**

### **Prerequisites**
- **Operating System**: Windows Server 2022 or Windows 11 Pro
- **.NET Runtime**: .NET 8.0 Runtime (not SDK)
- **Database**: MySQL Server 9.4.0 
- **Memory**: Minimum 8GB RAM (16GB recommended)
- **Storage**: 100GB available disk space
- **Network**: Isolated manufacturing network access

### **Step-by-Step Deployment Process**

#### **1. Database Server Setup**
```sql
-- Create production database
CREATE DATABASE mtm_production 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

-- Create application user
CREATE USER 'mtm_app_user'@'%' IDENTIFIED BY 'SecurePassword123!';
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON mtm_production.* TO 'mtm_app_user'@'%';
FLUSH PRIVILEGES;

-- Deploy all stored procedures
SOURCE deployment/stored_procedures/001_inventory_procedures.sql;
SOURCE deployment/stored_procedures/002_transaction_procedures.sql;
SOURCE deployment/stored_procedures/003_master_data_procedures.sql;
SOURCE deployment/stored_procedures/004_error_logging_procedures.sql;
SOURCE deployment/stored_procedures/005_system_maintenance_procedures.sql;

-- Insert master data
SOURCE deployment/data/master_data_insert.sql;
```

#### **2. Application Server Configuration**
```powershell
# Create application directory
New-Item -ItemType Directory -Path "C:\MTM_WIP_Application" -Force

# Extract application files
Expand-Archive -Path "MTM_WIP_Application_v1.0.0.zip" -DestinationPath "C:\MTM_WIP_Application\"

# Install .NET 8.0 Runtime (if not already installed)
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0

# Configure application settings
Copy-Item "deployment/appsettings.Production.json" -Destination "C:\MTM_WIP_Application\appsettings.json"
```

#### **3. Configuration Files**

**appsettings.Production.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PROD-DB-SERVER;Database=mtm_production;Uid=mtm_app_user;Pwd=SecurePassword123!;SslMode=Required;CharSet=utf8mb4;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "MTM_WIP_Application_Avalonia": "Information"
    },
    "File": {
      "Path": "C:\\Logs\\MTM_WIP_Application\\",
      "MaxFileSize": "50MB",
      "MaxFiles": 30,
      "LogLevel": "Information"
    }
  },
  "MTM": {
    "Environment": "Production",
    "Theme": "MTM_Blue",
    "AutoBackup": {
      "Enabled": true,
      "IntervalHours": 24,
      "BackupPath": "\\\\FILE-SERVER\\Backups\\MTM_WIP\\"
    },
    "Performance": {
      "DatabaseTimeout": 30,
      "ConnectionPoolSize": 100,
      "EnableMetrics": true
    },
    "Security": {
      "EnableAuditLogging": true,
      "SessionTimeoutMinutes": 120,
      "RequireWindowsAuth": true
    }
  }
}
```

#### **4. Windows Service Installation**
```powershell
# Install as Windows Service using NSSM (Non-Sucking Service Manager)
# Download NSSM from: https://nssm.cc/

# Install the service
nssm install "MTM WIP Application" "C:\MTM_WIP_Application\MTM_WIP_Application_Avalonia.exe"

# Configure service parameters
nssm set "MTM WIP Application" AppDirectory "C:\MTM_WIP_Application"
nssm set "MTM WIP Application" AppStdout "C:\Logs\MTM_WIP_Application\service.log"
nssm set "MTM WIP Application" AppStderr "C:\Logs\MTM_WIP_Application\service_error.log"
nssm set "MTM WIP Application" AppRotateFiles 1
nssm set "MTM WIP Application" AppRotateOnline 1
nssm set "MTM WIP Application" AppRotateSeconds 86400

# Set service to start automatically
nssm set "MTM WIP Application" Start SERVICE_AUTO_START

# Start the service
Start-Service "MTM WIP Application"
```

#### **5. Network Configuration**
```powershell
# Configure Windows Firewall (if enabled)
New-NetFirewallRule -DisplayName "MTM WIP Application" -Direction Inbound -Port 8080 -Protocol TCP -Action Allow

# Configure application port binding (if using Kestrel)
netsh http add urlacl url=http://+:8080/ user="NT AUTHORITY\NETWORK SERVICE"
```

## 🔧 **Environment Configuration Guidelines**

### **Development Environment Setup**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mtm_development;Uid=dev_user;Pwd=DevPassword123!;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "MTM_WIP_Application_Avalonia": "Debug"
    }
  },
  "MTM": {
    "Environment": "Development",
    "Theme": "MTM_Blue",
    "Performance": {
      "DatabaseTimeout": 60,
      "EnableMetrics": false
    }
  }
}
```

### **Staging Environment Setup**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=STAGING-DB-SERVER;Database=mtm_staging;Uid=staging_user;Pwd=StagingPassword123!;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "MTM_WIP_Application_Avalonia": "Debug"
    }
  },
  "MTM": {
    "Environment": "Staging",
    "Theme": "MTM_Blue",
    "Performance": {
      "DatabaseTimeout": 30,
      "EnableMetrics": true
    },
    "Testing": {
      "EnableTestData": true,
      "MockExternalSystems": true
    }
  }
}
```

## 📊 **Database Migration Strategy**

### **Version Control for Database Changes**
```
deployment/
├── database/
│   ├── migrations/
│   │   ├── v1.0.0/
│   │   │   ├── 001_initial_schema.sql
│   │   │   ├── 002_stored_procedures.sql
│   │   │   └── 003_master_data.sql
│   │   ├── v1.1.0/
│   │   │   ├── 004_new_inventory_fields.sql
│   │   │   └── 005_updated_procedures.sql
│   │   └── rollback/
│   │       ├── rollback_v1.1.0.sql
│   │       └── rollback_v1.0.0.sql
│   ├── stored_procedures/
│   └── data/
└── scripts/
    ├── deploy.ps1
    ├── rollback.ps1
    └── validate.ps1
```

### **Automated Migration Script**
```powershell
# deploy.ps1
param(
    [Parameter(Mandatory=$true)]
    [string]$TargetVersion,
    [string]$Environment = "Development"
)

# Load configuration
$config = Get-Content "deployment/config/$Environment.json" | ConvertFrom-Json
$connectionString = $config.ConnectionString

Write-Host "Deploying to $Environment environment..."
Write-Host "Target Version: $TargetVersion"

# Get current database version
$currentVersion = Invoke-MySqlQuery -ConnectionString $connectionString -Query "SELECT version FROM schema_version ORDER BY applied_date DESC LIMIT 1"

# Find migration scripts to apply
$migrationPath = "deployment/database/migrations"
$scriptsToApply = Get-ChildItem "$migrationPath" -Recurse -Filter "*.sql" | 
    Where-Object { $_.Directory.Name -gt $currentVersion -and $_.Directory.Name -le $TargetVersion } |
    Sort-Object Directory, Name

# Apply migrations
foreach ($script in $scriptsToApply) {
    Write-Host "Applying migration: $($script.Name)"
    try {
        Invoke-MySqlScript -ConnectionString $connectionString -FilePath $script.FullName
        
        # Record successful migration
        $insertQuery = "INSERT INTO schema_version (version, script_name, applied_date) VALUES ('$($script.Directory.Name)', '$($script.Name)', NOW())"
        Invoke-MySqlQuery -ConnectionString $connectionString -Query $insertQuery
        
        Write-Host "✅ Successfully applied: $($script.Name)"
    }
    catch {
        Write-Error "❌ Failed to apply migration: $($script.Name)"
        Write-Error $_.Exception.Message
        exit 1
    }
}

Write-Host "🎉 Deployment completed successfully!"
```

## 🔒 **Security Configuration**

### **Database Security**
```sql
-- Create read-only user for reporting
CREATE USER 'mtm_readonly'@'%' IDENTIFIED BY 'ReadOnlyPassword123!';
GRANT SELECT ON mtm_production.* TO 'mtm_readonly'@'%';

-- Create backup user
CREATE USER 'mtm_backup'@'localhost' IDENTIFIED BY 'BackupPassword123!';
GRANT SELECT, LOCK TABLES, SHOW VIEW, EVENT, TRIGGER ON mtm_production.* TO 'mtm_backup'@'localhost';

-- Enable SSL for all connections
ALTER USER 'mtm_app_user'@'%' REQUIRE SSL;
ALTER USER 'mtm_readonly'@'%' REQUIRE SSL;
```

### **Application Security**
```json
{
  "Security": {
    "Authentication": {
      "RequireWindowsAuth": true,
      "AllowedDomains": ["MANUFACTURING", "MTM-CORP"]
    },
    "Authorization": {
      "Roles": {
        "Administrator": ["MANUFACTURING\\MTM-Admins"],
        "Operator": ["MANUFACTURING\\Production-Staff"],
        "Viewer": ["MANUFACTURING\\Quality-Assurance"]
      }
    },
    "DataProtection": {
      "EncryptionKey": "Base64EncodedKey==",
      "RequireHttps": true
    }
  }
}
```

## 📈 **Performance Optimization**

### **Application Performance Settings**
```json
{
  "Performance": {
    "Database": {
      "ConnectionPooling": {
        "MinPoolSize": 5,
        "MaxPoolSize": 100,
        "ConnectionTimeout": 30,
        "CommandTimeout": 60
      },
      "QueryOptimization": {
        "EnableQueryCache": true,
        "CacheSizeGB": 2,
        "CacheTimeoutMinutes": 60
      }
    },
    "Memory": {
      "MaxWorkingSet": "500MB",
      "GCMode": "Workstation",
      "EnableLargeObjectHeap": true
    },
    "Threading": {
      "MaxConcurrentOperations": 50,
      "ThreadPoolMinThreads": 10,
      "ThreadPoolMaxThreads": 100
    }
  }
}
```

### **Database Performance Tuning**
```sql
-- MySQL Configuration Recommendations
-- Add to my.cnf or my.ini

[mysqld]
# Memory Settings
innodb_buffer_pool_size = 4G
innodb_log_file_size = 256M
innodb_log_buffer_size = 64M
query_cache_size = 256M

# Performance Settings
innodb_flush_log_at_trx_commit = 1
sync_binlog = 1
max_connections = 200
thread_cache_size = 16

# Timeout Settings
wait_timeout = 28800
interactive_timeout = 28800
```

## 🔄 **Backup and Recovery Procedures**

### **Automated Backup Strategy**
```powershell
# automated-backup.ps1
param(
    [string]$BackupPath = "\\FILE-SERVER\Backups\MTM_WIP",
    [string]$Environment = "Production"
)

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFileName = "mtm_backup_$Environment_$timestamp.sql"
$backupFullPath = Join-Path $BackupPath $backupFileName

# Create database backup
mysqldump --user=mtm_backup --password=BackupPassword123! `
    --host=PROD-DB-SERVER `
    --single-transaction `
    --routines `
    --triggers `
    mtm_production > $backupFullPath

# Compress backup
7z a "$backupFullPath.7z" $backupFullPath
Remove-Item $backupFullPath

# Cleanup old backups (keep 30 days)
Get-ChildItem $BackupPath -Filter "*.7z" | 
    Where-Object { $_.CreationTime -lt (Get-Date).AddDays(-30) } | 
    Remove-Item

Write-Host "✅ Backup completed: $backupFileName.7z"
```

### **Disaster Recovery Plan**
1. **Database Recovery**: Restore from latest backup + transaction logs
2. **Application Recovery**: Deploy from known-good release package
3. **Configuration Recovery**: Restore from configuration backup
4. **Data Validation**: Run integrity checks after recovery
5. **User Communication**: Notify manufacturing team of system status

## 📊 **Monitoring and Alerting**

### **Application Health Monitoring**
```json
{
  "HealthChecks": {
    "Database": {
      "ConnectionString": "Server=PROD-DB-SERVER;Database=mtm_production;Uid=health_check;Pwd=HealthPassword123!;",
      "Timeout": "00:00:10",
      "Query": "SELECT 1"
    },
    "DiskSpace": {
      "Drives": ["C:", "D:"],
      "MinimumFreeSpaceGB": 10
    },
    "Memory": {
      "MaxWorkingSetMB": 500,
      "MaxPrivateMemoryMB": 600
    }
  },
  "Alerts": {
    "Email": {
      "SmtpServer": "mail.mtm-corp.com",
      "Recipients": ["it-support@mtm-corp.com", "production-mgr@mtm-corp.com"],
      "From": "mtm-wip-app@mtm-corp.com"
    },
    "Thresholds": {
      "DatabaseResponseTimeMs": 1000,
      "HighMemoryUsageMB": 450,
      "DiskSpaceWarningGB": 20
    }
  }
}
```

### **Log Management**
```json
{
  "Logging": {
    "File": {
      "Path": "C:\\Logs\\MTM_WIP_Application\\",
      "FileName": "mtm-app-{Date}.log",
      "RollingInterval": "Day",
      "RetainedFileCountLimit": 30,
      "FileSizeLimitBytes": 52428800
    },
    "EventLog": {
      "Source": "MTM WIP Application",
      "LogName": "Application",
      "MinimumLevel": "Warning"
    },
    "Database": {
      "Enabled": true,
      "ConnectionString": "DefaultConnection",
      "TableName": "ApplicationLogs",
      "MinimumLevel": "Information"
    }
  }
}
```

## 🚀 **Deployment Checklist**

### **Pre-Deployment**
- [ ] Database backup completed
- [ ] Application configuration reviewed
- [ ] Security settings validated
- [ ] Performance baseline established
- [ ] Rollback plan prepared

### **During Deployment**
- [ ] Stop application service
- [ ] Deploy database changes
- [ ] Deploy application files
- [ ] Update configuration files
- [ ] Start application service

### **Post-Deployment**
- [ ] Health checks passed
- [ ] Performance monitoring active
- [ ] User acceptance testing completed
- [ ] Documentation updated
- [ ] Deployment report generated

This deployment documentation ensures consistent, reliable, and secure deployments of the MTM WIP Application across all environments while maintaining manufacturing-grade reliability and security standards.
