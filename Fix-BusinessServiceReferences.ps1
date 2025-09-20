#!/usr/bin/env pwsh

# PowerShell script to fix ambiguous Business Service references
Write-Host "🔧 MTM Business Services Reference Fix Script" -ForegroundColor Cyan

# Step 1: Fix ServiceCollectionExtensions.cs registration syntax
Write-Host "📝 Fixing service registrations..." -ForegroundColor Yellow

$extensionsFile = "Extensions\ServiceCollectionExtensions.cs"
if (Test-Path $extensionsFile) {
    $content = Get-Content -Path $extensionsFile -Raw
    
    # Fix the service registrations - use full namespace qualification
    $registrationFixes = @{
        'services.TryAddSingleton<IMasterDataService, Business.MasterDataService>\(\);' = 'services.TryAddSingleton<Business.IMasterDataService, Business.MasterDataService>();'
        'services.TryAddScoped<IInventoryEditingService, Business.InventoryEditingService>\(\);' = 'services.TryAddScoped<Business.IInventoryEditingService, Business.InventoryEditingService>();'
        'services.TryAddScoped<IRemoveService, Business.RemoveService>\(\);' = 'services.TryAddScoped<Business.IRemoveService, Business.RemoveService>();'
    }
    
    foreach ($find in $registrationFixes.Keys) {
        $replace = $registrationFixes[$find]
        if ($content -match [regex]::Escape($find)) {
            $content = $content -replace [regex]::Escape($find), $replace
            Write-Host "  ✅ Fixed registration: $($find.Split('<')[1].Split(',')[0])" -ForegroundColor Green
        }
    }
    
    Set-Content -Path $extensionsFile -Value $content -NoNewline
    Write-Host "  📝 ServiceCollectionExtensions.cs updated" -ForegroundColor Yellow
}

# Step 2: Fix ViewModels to use fully qualified Business service interfaces
Write-Host "`n🔍 Fixing ViewModel service references..." -ForegroundColor Yellow

$viewModelFiles = @(
    "ViewModels\MainForm\InventoryTabViewModel.cs",
    "ViewModels\MainForm\RemoveItemViewModel.cs", 
    "ViewModels\MainForm\TransferItemViewModel.cs",
    "ViewModels\Overlay\EditInventoryViewModel.cs",
    "ViewModels\Overlay\NewQuickButtonOverlayViewModel.cs"
)

foreach ($file in $viewModelFiles) {
    if (Test-Path $file) {
        $content = Get-Content -Path $file -Raw
        $originalContent = $content
        $changes = 0
        
        # Replace interface usage with fully qualified names
        $interfaceReplacements = @{
            ': IMasterDataService' = ': Business.IMasterDataService'
            'IMasterDataService ' = 'Business.IMasterDataService '
            'IInventoryEditingService ' = 'Business.IInventoryEditingService '
            'IRemoveService ' = 'Business.IRemoveService '
            'ItemsRemovedEventArgs' = 'Business.ItemsRemovedEventArgs'
        }
        
        foreach ($find in $interfaceReplacements.Keys) {
            $replace = $interfaceReplacements[$find]
            if ($content -match [regex]::Escape($find)) {
                $content = $content -replace [regex]::Escape($find), $replace
                $changes++
            }
        }
        
        if ($changes -gt 0) {
            Set-Content -Path $file -Value $content -NoNewline
            Write-Host "  📝 Fixed: $(Split-Path $file -Leaf) ($changes changes)" -ForegroundColor Green
        }
    }
}

# Step 3: Check for any remaining service files that might have duplicate interfaces
Write-Host "`n🔍 Looking for remaining duplicate service interfaces..." -ForegroundColor Yellow

$remainingServiceFiles = Get-ChildItem -Path "Services" -Filter "*.cs" -Exclude "Business*" | Where-Object { 
    $_.Name -notmatch "Core|CustomDataGrid|Column" 
}

$duplicateInterfaces = @()
foreach ($file in $remainingServiceFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    
    if ($content -match "public interface (IMasterDataService|IInventoryEditingService|IRemoveService)") {
        $duplicateInterfaces += $file.FullName
        Write-Host "  ⚠️  Found duplicate interface in: $($file.Name)" -ForegroundColor Yellow
    }
}

if ($duplicateInterfaces.Count -gt 0) {
    Write-Host "`n❌ Found $($duplicateInterfaces.Count) files with duplicate interfaces that need manual review" -ForegroundColor Red
} else {
    Write-Host "  ✅ No duplicate interfaces found in remaining service files" -ForegroundColor Green
}

Write-Host "`n📊 Summary:" -ForegroundColor Cyan
Write-Host "  - ServiceCollectionExtensions.cs updated with full qualification" -ForegroundColor Green
Write-Host "  - ViewModel files updated to use Business namespace" -ForegroundColor Green
Write-Host "  - Duplicate interface check completed" -ForegroundColor Green

Write-Host "`n✅ Business Services reference fix completed!" -ForegroundColor Green
Write-Host "🔨 Run 'dotnet build' to verify compilation..." -ForegroundColor Cyan