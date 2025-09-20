#!/usr/bin/env pwsh

# PowerShell automation script for Business Services consolidation
# Phase 1 Task 1.1.3: Create Business Services Group
Write-Host "🏭 MTM Business Services Consolidation Script" -ForegroundColor Cyan

# Step 1: Move original service files to backup location
$servicesToMove = @(
    "MasterDataService.cs",
    "InventoryEditingService.cs", 
    "RemoveService.cs"
)

$backupDir = "Services\Business\Original"
New-Item -ItemType Directory -Force -Path $backupDir | Out-Null

Write-Host "📦 Backing up original service files..." -ForegroundColor Yellow
foreach ($service in $servicesToMove) {
    $sourcePath = "Services\$service"
    $backupPath = "$backupDir\$service"
    
    if (Test-Path $sourcePath) {
        Move-Item -Path $sourcePath -Destination $backupPath -Force
        Write-Host "  ✅ Moved: $service → $backupPath" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  File not found: $service" -ForegroundColor Yellow
    }
}

# Step 2: Update service registrations in ServiceCollectionExtensions.cs
Write-Host "`n🔧 Updating service registrations..." -ForegroundColor Cyan

$extensionsFile = "Extensions\ServiceCollectionExtensions.cs"
if (Test-Path $extensionsFile) {
    $content = Get-Content -Path $extensionsFile -Raw
    
    # Add using statement for Business namespace
    if ($content -notmatch "using MTM_WIP_Application_Avalonia\.Services\.Business;") {
        $content = $content -replace "(using MTM_WIP_Application_Avalonia\.Services\.Core;)", "`$1`nusing MTM_WIP_Application_Avalonia.Services.Business;"
        Write-Host "  ✅ Added Business services using statement" -ForegroundColor Green
    }
    
    # Update service registrations to use Business namespace
    $businessServiceUpdates = @{
        'services.TryAddSingleton<IMasterDataService, MasterDataService>\(\);' = 'services.TryAddSingleton<IMasterDataService, Business.MasterDataService>();'
        'services.TryAddScoped<IInventoryEditingService, InventoryEditingService>\(\);' = 'services.TryAddScoped<IInventoryEditingService, Business.InventoryEditingService>();'
        'services.TryAddScoped<IRemoveService, RemoveService>\(\);' = 'services.TryAddScoped<IRemoveService, Business.RemoveService>();'
    }
    
    foreach ($find in $businessServiceUpdates.Keys) {
        $replace = $businessServiceUpdates[$find]
        if ($content -match $find) {
            $content = $content -replace $find, $replace
            Write-Host "  ✅ Updated service registration: $($find.Split('<')[1].Split(',')[0])" -ForegroundColor Green
        }
    }
    
    Set-Content -Path $extensionsFile -Value $content -NoNewline
    Write-Host "  📝 ServiceCollectionExtensions.cs updated" -ForegroundColor Yellow
}

# Step 3: Find and update all references to the moved services
Write-Host "`n🔍 Updating service references throughout application..." -ForegroundColor Cyan

# Get all C# files that might reference the services
$csharpFiles = Get-ChildItem -Path "." -Filter "*.cs" -Recurse | Where-Object { 
    $_.FullName -notmatch "\\bin\\|\\obj\\|\\packages\\" -and
    $_.FullName -notmatch "\\Services\\Business\\Original\\"
}

$totalChanges = 0
$modifiedFiles = @()

foreach ($file in $csharpFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $fileChanges = 0
    
    # Add using statement for Business namespace if file uses the services
    if (($content -match "IMasterDataService|IInventoryEditingService|IRemoveService") -and
        ($content -notmatch "using MTM_WIP_Application_Avalonia\.Services\.Business;")) {
        
        # Find the last using statement to insert after
        if ($content -match "using MTM_WIP_Application_Avalonia\.Services\.Core;") {
            $content = $content -replace "(using MTM_WIP_Application_Avalonia\.Services\.Core;)", "`$1`nusing MTM_WIP_Application_Avalonia.Services.Business;"
            $fileChanges++
        } elseif ($content -match "using MTM_WIP_Application_Avalonia\.Services;") {
            $content = $content -replace "(using MTM_WIP_Application_Avalonia\.Services;)", "`$1`nusing MTM_WIP_Application_Avalonia.Services.Business;"
            $fileChanges++
        }
    }
    
    # Write back if changes were made
    if ($fileChanges -gt 0) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $modifiedFiles += $file.FullName
        $totalChanges += $fileChanges
        Write-Host "  📝 Modified: $($file.Name) ($fileChanges changes)" -ForegroundColor Yellow
    }
}

Write-Host "`n📊 Summary:" -ForegroundColor Cyan
Write-Host "  - Files processed: $($csharpFiles.Count)" -ForegroundColor White
Write-Host "  - Files modified: $($modifiedFiles.Count)" -ForegroundColor Green  
Write-Host "  - Total changes: $totalChanges" -ForegroundColor Green
Write-Host "  - Services moved to backup: $($servicesToMove.Count)" -ForegroundColor Green
Write-Host "  - New consolidated file: Services\Business\BusinessServices.cs" -ForegroundColor Green

if ($modifiedFiles.Count -gt 0) {
    Write-Host "  - Modified files:" -ForegroundColor White
    foreach ($file in $modifiedFiles) {
        Write-Host "    • $($file -replace [regex]::Escape((Get-Location).Path + '\'), '')" -ForegroundColor Gray
    }
}

Write-Host "`n✅ Business Services consolidation completed!" -ForegroundColor Green
Write-Host "🔨 Run 'dotnet build' to verify compilation..." -ForegroundColor Cyan