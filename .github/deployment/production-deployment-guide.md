# Production Deployment Guide - MTM WIP Application

**Framework**: .NET 8 Multi-Platform Manufacturing Application  
**Deployment Targets**: Windows, macOS, Linux, Android  
**Environment**: Production Manufacturing Systems  
**Created**: 2025-09-14  

---

## 🏭 Production Deployment Overview

### Manufacturing Environment Requirements

The MTM WIP Application is designed for deployment in professional manufacturing environments with specific reliability, performance, and scalability requirements.

#### **Deployment Architecture**
```
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│   Manufacturing │  │   Engineering   │  │   Management    │
│   Floor Devices │  │   Workstations  │  │   Dashboards    │
├─────────────────┤  ├─────────────────┤  ├─────────────────┤
│ • Windows PCs   │  │ • macOS Systems │  │ • Web Browsers  │
│ • Android       │  │ • Linux Servers │  │ • Mobile Apps   │
│   Tablets       │  │ • Development   │  │ • Reporting     │
│ • Handheld      │  │   Tools         │  │   Systems       │
│   Scanners      │  │                 │  │                 │
└─────────────────┘  └─────────────────┘  └─────────────────┘
         ↕                      ↕                      ↕
┌─────────────────────────────────────────────────────────────┐
│                 Manufacturing Network                        │
├─────────────────────────────────────────────────────────────┤
│ • MySQL Database Cluster (Primary + Backup)                │
│ • Application Servers (Load Balanced)                      │
│ • File Storage (Backups, Reports, Documentation)           │
│ • Monitoring & Alerting Systems                            │
└─────────────────────────────────────────────────────────────┘
```

---

## 🖥️ Windows Production Deployment

### System Requirements
- **Operating System**: Windows 10/11 Pro (64-bit) or Windows Server 2019/2022
- **Framework**: .NET 8 Runtime (automatically included in deployment)
- **Memory**: 4GB RAM minimum, 8GB recommended for manufacturing workloads
- **Storage**: 1GB application space, 10GB+ for data and backups
- **Network**: Gigabit Ethernet for database connectivity

### Windows Deployment Package Creation
```powershell
# Production build and packaging script
param(
    [string]$Configuration = "Release",
    [string]$TargetFramework = "net8.0-windows",
    [string]$RuntimeIdentifier = "win-x64",
    [bool]$SelfContained = $true
)

# Clean and restore
dotnet clean --configuration $Configuration
dotnet restore

# Build production package
dotnet publish `
    --configuration $Configuration `
    --framework $TargetFramework `
    --runtime $RuntimeIdentifier `
    --self-contained $SelfContained `
    --output "publish/windows" `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:AssemblyVersion="1.0.0.0" `
    -p:FileVersion="1.0.0.0"

# Create installer package (using WiX or Inno Setup)
Write-Host "Creating Windows installer package..."
# Additional installer creation logic here
```

### Windows Service Deployment
```csharp
// Program.cs modifications for Windows Service
public class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        
        if (WindowsServiceHelpers.IsWindowsService())
        {
            builder.UseWindowsService();
        }
        
        var host = builder.Build();
        
        if (args.Length > 0 && args[0] == "--install-service")
        {
            InstallWindowsService();
            return;
        }
        
        host.Run();
    }
    
    private static void InstallWindowsService()
    {
        // Windows Service installation logic
        var serviceName = "MTMWIPApplication";
        var displayName = "MTM WIP Application Service";
        var description = "Manufacturing Work-in-Process Application";
        
        // Service installation code
    }
}
```

### Windows Registry Configuration
```powershell
# Manufacturing-specific Windows configuration
$registryPath = "HKLM:\SOFTWARE\MTM\WIPApplication"

# Create registry entries for manufacturing environment
New-ItemProperty -Path $registryPath -Name "ManufacturingMode" -Value "Production" -PropertyType String
New-ItemProperty -Path $registryPath -Name "DatabaseTimeout" -Value 30 -PropertyType DWord
New-ItemProperty -Path $registryPath -Name "MaxConcurrentUsers" -Value 100 -PropertyType DWord
New-ItemProperty -Path $registryPath -Name "BackupEnabled" -Value $true -PropertyType DWord
```

---

## 🍎 macOS Production Deployment

### System Requirements
- **Operating System**: macOS 12.0 (Monterey) or later
- **Framework**: .NET 8 Runtime
- **Memory**: 4GB RAM minimum, 8GB recommended
- **Storage**: 2GB application space (larger due to macOS app bundle structure)
- **Security**: Gatekeeper approval for enterprise deployment

### macOS App Bundle Creation
```bash
#!/bin/bash
# macOS production build script

echo "Building MTM WIP Application for macOS..."

# Clean and restore
dotnet clean --configuration Release
dotnet restore

# Build macOS application
dotnet publish \
    --configuration Release \
    --framework net8.0 \
    --runtime osx-x64 \
    --self-contained true \
    --output "publish/macos" \
    -p:PublishSingleFile=true \
    -p:PublishTrimmed=true

# Create macOS app bundle
APP_NAME="MTM WIP Application"
BUNDLE_DIR="publish/macos/${APP_NAME}.app"
CONTENTS_DIR="${BUNDLE_DIR}/Contents"
MACOS_DIR="${CONTENTS_DIR}/MacOS"
RESOURCES_DIR="${CONTENTS_DIR}/Resources"

# Create bundle structure
mkdir -p "${MACOS_DIR}"
mkdir -p "${RESOURCES_DIR}"

# Copy executable
cp "publish/macos/MTM_WIP_Application_Avalonia" "${MACOS_DIR}/"

# Create Info.plist for manufacturing application
cat > "${CONTENTS_DIR}/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>MTM WIP Application</string>
    <key>CFBundleDisplayName</key>
    <string>MTM WIP Application</string>
    <key>CFBundleIdentifier</key>
    <string>com.mtm.wipapplication</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>CFBundleExecutable</key>
    <string>MTM_WIP_Application_Avalonia</string>
    <key>CFBundleIconFile</key>
    <string>AppIcon</string>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.business</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSSupportsAutomaticGraphicsSwitching</key>
    <true/>
</dict>
</plist>
EOF

echo "macOS app bundle created successfully"
```

### macOS Code Signing and Notarization
```bash
#!/bin/bash
# Code signing for enterprise deployment

DEVELOPER_ID="Developer ID Application: Your Organization Name"
APP_BUNDLE="publish/macos/MTM WIP Application.app"

# Sign the application
echo "Signing application for enterprise deployment..."
codesign --force --verify --verbose --sign "$DEVELOPER_ID" "$APP_BUNDLE"

# Create installer package
echo "Creating installer package..."
pkgbuild --root "publish/macos" \
         --identifier "com.mtm.wipapplication.pkg" \
         --version "1.0.0" \
         --install-location "/Applications" \
         "MTM_WIP_Application_Installer.pkg"

# Sign the installer
productsign --sign "$DEVELOPER_ID" \
            "MTM_WIP_Application_Installer.pkg" \
            "MTM_WIP_Application_Signed.pkg"

echo "macOS deployment package ready"
```

---

## 🐧 Linux Production Deployment

### System Requirements
- **Operating System**: Ubuntu 20.04 LTS, RHEL 8+, or SUSE Linux Enterprise 15+
- **Framework**: .NET 8 Runtime
- **Memory**: 2GB RAM minimum, 4GB recommended for server deployment
- **Storage**: 500MB application space
- **Database**: MySQL 8.0+ server or client connectivity

### Docker Container Deployment
```dockerfile
# Dockerfile for Linux production deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Manufacturing environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV MTM_MANUFACTURING_MODE=Production
ENV MTM_DATABASE_TIMEOUT=30
ENV MTM_MAX_CONCURRENT_USERS=100

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MTM_WIP_Application_Avalonia.csproj", "."]
RUN dotnet restore "./MTM_WIP_Application_Avalonia.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MTM_WIP_Application_Avalonia.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MTM_WIP_Application_Avalonia.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install manufacturing-specific dependencies
RUN apt-get update && apt-get install -y \
    libx11-6 \
    libice6 \
    libsm6 \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create manufacturing data directories
RUN mkdir -p /app/data/backups /app/data/reports /app/logs

# Set permissions for manufacturing environment
RUN chown -R app:app /app/data /app/logs
USER app

ENTRYPOINT ["dotnet", "MTM_WIP_Application_Avalonia.dll"]
```

### Linux Systemd Service Configuration
```ini
# /etc/systemd/system/mtm-wip-application.service
[Unit]
Description=MTM WIP Application Manufacturing Service
After=network.target mysql.service
Requires=mysql.service

[Service]
Type=notify
User=mtm-app
Group=mtm-app
WorkingDirectory=/opt/mtm-wip-application
ExecStart=/opt/mtm-wip-application/MTM_WIP_Application_Avalonia
Restart=always
RestartSec=10

# Manufacturing environment variables
Environment=DOTNET_ENVIRONMENT=Production
Environment=MTM_MANUFACTURING_MODE=Production
Environment=MTM_DATABASE_TIMEOUT=30
Environment=MTM_BACKUP_ENABLED=true

# Performance tuning for manufacturing workloads
Environment=DOTNET_GCServer=1
Environment=DOTNET_GCConcurrent=1
Environment=DOTNET_ThreadPool_UnfairSemaphoreSpinLimit=6

# Logging configuration
StandardOutput=journal
StandardError=journal
SyslogIdentifier=mtm-wip-application

[Install]
WantedBy=multi-user.target
```

### Linux Deployment Script
```bash
#!/bin/bash
# Linux production deployment script

set -e

echo "Deploying MTM WIP Application to Linux..."

# Configuration
APP_NAME="mtm-wip-application"
APP_USER="mtm-app"
APP_GROUP="mtm-app"
INSTALL_DIR="/opt/${APP_NAME}"
SERVICE_FILE="/etc/systemd/system/${APP_NAME}.service"

# Create application user and group
if ! id -u "$APP_USER" > /dev/null 2>&1; then
    echo "Creating application user: $APP_USER"
    sudo useradd --system --shell /bin/false --home "$INSTALL_DIR" "$APP_USER"
fi

# Create installation directory
sudo mkdir -p "$INSTALL_DIR"
sudo mkdir -p "$INSTALL_DIR/data/backups"
sudo mkdir -p "$INSTALL_DIR/data/reports"
sudo mkdir -p "$INSTALL_DIR/logs"

# Copy application files
sudo cp -r publish/linux/* "$INSTALL_DIR/"
sudo chown -R "$APP_USER:$APP_GROUP" "$INSTALL_DIR"
sudo chmod +x "$INSTALL_DIR/MTM_WIP_Application_Avalonia"

# Install systemd service
sudo cp "deployment/linux/${APP_NAME}.service" "$SERVICE_FILE"
sudo systemctl daemon-reload
sudo systemctl enable "$APP_NAME"

# Configure firewall for manufacturing network
sudo ufw allow from 10.0.0.0/8 to any port 80
sudo ufw allow from 10.0.0.0/8 to any port 443
sudo ufw allow from 192.168.0.0/16 to any port 80
sudo ufw allow from 192.168.0.0/16 to any port 443

echo "Linux deployment completed successfully"
echo "Start the service with: sudo systemctl start $APP_NAME"
```

---

## 📱 Android Production Deployment

### System Requirements
- **Operating System**: Android 8.0 (API level 26) or higher
- **Framework**: .NET 8 for Android
- **Memory**: 2GB RAM minimum, 4GB recommended for manufacturing tablets
- **Storage**: 200MB application space
- **Network**: WiFi or cellular connectivity for database access

### Android APK Build Configuration
```xml
<!-- AndroidManifest.xml for manufacturing deployment -->
<manifest xmlns:android="http://schemas.android.com/apk/res/android"
          package="com.mtm.wipapplication"
          android:versionCode="1"
          android:versionName="1.0.0">

    <!-- Manufacturing-specific permissions -->
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.WAKE_LOCK" />
    
    <!-- Manufacturing hardware features -->
    <uses-feature android:name="android.hardware.camera" android:required="false" />
    <uses-feature android:name="android.hardware.camera.autofocus" android:required="false" />
    <uses-feature android:name="android.hardware.wifi" android:required="true" />
    
    <!-- Manufacturing tablet optimization -->
    <supports-screens android:largeScreens="true"
                      android:xlargeScreens="true"
                      android:anyDensity="true" />

    <application android:label="MTM WIP Application"
                 android:icon="@mipmap/appicon"
                 android:theme="@style/MTMTheme"
                 android:hardwareAccelerated="true"
                 android:largeHeap="true">
        
        <activity android:name=".MainActivity"
                  android:exported="true"
                  android:configChanges="orientation|screenSize"
                  android:screenOrientation="landscape"
                  android:keepScreenOn="true">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>
    </application>
</manifest>
```

### Android Build Script
```bash
#!/bin/bash
# Android production build script

echo "Building MTM WIP Application for Android..."

# Build configuration
BUILD_CONFIGURATION="Release"
TARGET_FRAMEWORK="net8.0-android"

# Clean and restore
dotnet clean --configuration $BUILD_CONFIGURATION
dotnet restore

# Build Android APK
dotnet build \
    --configuration $BUILD_CONFIGURATION \
    --framework $TARGET_FRAMEWORK \
    -p:AndroidPackageFormat=apk \
    -p:AndroidUseAapt2=true \
    -p:AndroidCreatePackagePerAbi=true \
    -p:AndroidSupportedAbis="arm64-v8a;armeabi-v7a;x86_64" \
    -p:AndroidVersionCode=1 \
    -p:AndroidVersionName="1.0.0"

# Sign APK for manufacturing deployment
KEYSTORE_FILE="deployment/android/mtm-release-key.keystore"
KEYSTORE_ALIAS="mtm-release"
APK_FILE="bin/Release/net8.0-android/com.mtm.wipapplication-Signed.apk"

if [ -f "$KEYSTORE_FILE" ]; then
    echo "Signing APK for production deployment..."
    jarsigner -verbose -sigalg SHA1withRSA -digestalg SHA1 \
        -keystore "$KEYSTORE_FILE" \
        "$APK_FILE" \
        "$KEYSTORE_ALIAS"
    
    # Align APK for optimization
    zipalign -v 4 "$APK_FILE" "MTM_WIP_Application_Production.apk"
    
    echo "Production APK created: MTM_WIP_Application_Production.apk"
else
    echo "Warning: Keystore not found. APK is unsigned."
fi
```

---

## 🔧 Production Configuration Management

### Database Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=mysql-cluster.manufacturing.local;Database=mtm_wip_production;Uid=mtm_app_user;Pwd=${MTM_DB_PASSWORD};Connection Timeout=30;Command Timeout=120;Pooling=true;Min Pool Size=5;Max Pool Size=50;Connection Lifetime=300;Connection Reset=false;Allow User Variables=true;Use Compression=false;SSL Mode=Required;"
  },
  "MTMSettings": {
    "Environment": "Production",
    "DefaultOperation": "90",
    "DefaultQuantity": 1,
    "EnableAutoSave": true,
    "AutoSaveIntervalMinutes": 2,
    "MaxQuickButtons": 20,
    "MaxRecentTransactions": 1000,
    "DatabaseTimeoutSeconds": 30,
    "MaxRetryAttempts": 3,
    "EnableAuditLogging": true,
    "EnablePerformanceMonitoring": true,
    "BackupEnabled": true,
    "BackupIntervalHours": 4
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "MTM_WIP_Application_Avalonia": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    },
    "File": {
      "Path": "logs/mtm-wip-{Date}.log",
      "MaxFileSizeMB": 100,
      "MaxRollingFiles": 10
    }
  }
}
```

### Environment-Specific Configuration
```bash
# Production environment variables
export DOTNET_ENVIRONMENT=Production
export MTM_MANUFACTURING_MODE=Production
export MTM_DATABASE_TIMEOUT=30
export MTM_MAX_CONCURRENT_USERS=100
export MTM_BACKUP_ENABLED=true
export MTM_BACKUP_DIRECTORY="/data/backups"
export MTM_LOG_LEVEL=Information
export MTM_PERFORMANCE_MONITORING=true

# Performance optimization
export DOTNET_GCServer=1
export DOTNET_GCConcurrent=1
export DOTNET_ThreadPool_UnfairSemaphoreSpinLimit=6
export DOTNET_System_Net_Http_SocketsHttpHandler_MaxConnectionsPerServer=50
```

---

## 📊 Production Monitoring and Maintenance

### Health Check Configuration
```csharp
// HealthCheck configuration for production monitoring
public static class HealthCheckConfiguration
{
    public static IServiceCollection AddMTMHealthChecks(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<MemoryHealthCheck>("memory")
            .AddCheck<DiskSpaceHealthCheck>("diskspace")
            .AddCheck<NetworkHealthCheck>("network")
            .AddCheck<BackupHealthCheck>("backup");
            
        return services;
    }
}
```

### Production Backup Strategy
```bash
#!/bin/bash
# Production backup script for manufacturing data

BACKUP_DIR="/data/backups"
DATABASE="mtm_wip_production"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="${BACKUP_DIR}/mtm_backup_${TIMESTAMP}.sql"

# Create database backup
mysqldump --single-transaction --routines --triggers \
    --user=backup_user --password=${BACKUP_PASSWORD} \
    ${DATABASE} > ${BACKUP_FILE}

# Compress backup
gzip ${BACKUP_FILE}

# Upload to secure storage (optional)
# aws s3 cp ${BACKUP_FILE}.gz s3://mtm-backups/

# Cleanup old backups (keep last 30 days)
find ${BACKUP_DIR} -name "mtm_backup_*.sql.gz" -mtime +30 -delete

echo "Backup completed: ${BACKUP_FILE}.gz"
```

---

## 🚀 Deployment Automation

### CI/CD Pipeline Configuration
```yaml
# Azure DevOps Pipeline for MTM WIP Application
name: MTM WIP Application Production Deployment

trigger:
  branches:
    include:
    - main
    - release/*

variables:
  buildConfiguration: 'Release'
  dotNetVersion: '8.0.x'

stages:
- stage: Build
  displayName: 'Build Application'
  jobs:
  - job: BuildWindows
    displayName: 'Build Windows Package'
    pool:
      vmImage: 'windows-latest'
    steps:
    - task: UseDotNet@2
      inputs:
        version: $(dotNetVersion)
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration)
        dotnet publish --configuration $(buildConfiguration) --runtime win-x64 --self-contained true --output $(Build.ArtifactStagingDirectory)/windows
      displayName: 'Build Windows Application'
    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)/windows'
        artifactName: 'windows-package'

  - job: BuildMacOS
    displayName: 'Build macOS Package'
    pool:
      vmImage: 'macos-latest'
    steps:
    - task: UseDotNet@2
      inputs:
        version: $(dotNetVersion)
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration)
        dotnet publish --configuration $(buildConfiguration) --runtime osx-x64 --self-contained true --output $(Build.ArtifactStagingDirectory)/macos
      displayName: 'Build macOS Application'
    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)/macos'
        artifactName: 'macos-package'

  - job: BuildLinux
    displayName: 'Build Linux Package'
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: UseDotNet@2
      inputs:
        version: $(dotNetVersion)
    - script: |
        dotnet restore
        dotnet build --configuration $(buildConfiguration)
        dotnet publish --configuration $(buildConfiguration) --runtime linux-x64 --self-contained true --output $(Build.ArtifactStagingDirectory)/linux
      displayName: 'Build Linux Application'
    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)/linux'
        artifactName: 'linux-package'

- stage: Deploy
  displayName: 'Deploy to Production'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployProduction
    displayName: 'Deploy to Production Environment'
    environment: 'Production'
    strategy:
      runOnce:
        deploy:
          steps:
          - download: current
            artifact: windows-package
          - download: current
            artifact: macos-package
          - download: current
            artifact: linux-package
          - script: |
              echo "Deploying to production manufacturing environment"
              # Additional deployment steps here
            displayName: 'Deploy Applications'
```

This comprehensive production deployment guide ensures reliable, scalable, and maintainable deployment of the MTM WIP Application across all manufacturing environments with proper monitoring, backup, and security considerations.