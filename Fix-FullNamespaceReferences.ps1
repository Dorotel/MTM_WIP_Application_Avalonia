#!/usr/bin/env pwsh

# PowerShell script to fix Business namespace references with full qualification
Write-Host "🔧 MTM Full Namespace Qualification Fix" -ForegroundColor Cyan

# Fix all ViewModel files to use full namespace qualification
$viewModelFiles = @(
    "ViewModels\MainForm\InventoryTabViewModel.cs",
    "ViewModels\MainForm\RemoveItemViewModel.cs", 
    "ViewModels\MainForm\TransferItemViewModel.cs",
    "ViewModels\Overlay\EditInventoryViewModel.cs",
    "ViewModels\Overlay\NewQuickButtonOverlayViewModel.cs"
)

Write-Host "📝 Replacing Business.* with full namespace qualification..." -ForegroundColor Yellow

foreach ($file in $viewModelFiles) {
    if (Test-Path $file) {
        $content = Get-Content -Path $file -Raw
        $changes = 0
        
        # Replace Business.Interface with full namespace
        $fullNamespaceReplacements = @{
            'Business\.IMasterDataService' = 'MTM_WIP_Application_Avalonia.Services.Business.IMasterDataService'
            'Business\.IInventoryEditingService' = 'MTM_WIP_Application_Avalonia.Services.Business.IInventoryEditingService'
            'Business\.IRemoveService' = 'MTM_WIP_Application_Avalonia.Services.Business.IRemoveService'
            'Business\.ItemsRemovedEventArgs' = 'MTM_WIP_Application_Avalonia.Services.Business.ItemsRemovedEventArgs'
        }
        
        foreach ($find in $fullNamespaceReplacements.Keys) {
            $replace = $fullNamespaceReplacements[$find]
            if ($content -match $find) {
                $content = $content -replace $find, $replace
                $changes++
            }
        }
        
        if ($changes -gt 0) {
            Set-Content -Path $file -Value $content -NoNewline
            Write-Host "  📝 Fixed: $(Split-Path $file -Leaf) ($changes changes)" -ForegroundColor Green
        }
    }
}

# Also fix ServiceCollectionExtensions.cs
$extensionsFile = "Extensions\ServiceCollectionExtensions.cs"
if (Test-Path $extensionsFile) {
    $content = Get-Content -Path $extensionsFile -Raw
    
    # Fix service registrations with full namespaces
    $registrationFixes = @{
        'services\.TryAddSingleton<Business\.IMasterDataService, Business\.MasterDataService>\(\);' = 'services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Business.IMasterDataService, MTM_WIP_Application_Avalonia.Services.Business.MasterDataService>();'
        'services\.TryAddScoped<Business\.IInventoryEditingService, Business\.InventoryEditingService>\(\);' = 'services.TryAddScoped<MTM_WIP_Application_Avalonia.Services.Business.IInventoryEditingService, MTM_WIP_Application_Avalonia.Services.Business.InventoryEditingService>();'
        'services\.TryAddScoped<Business\.IRemoveService, Business\.RemoveService>\(\);' = 'services.TryAddScoped<MTM_WIP_Application_Avalonia.Services.Business.IRemoveService, MTM_WIP_Application_Avalonia.Services.Business.RemoveService>();'
    }
    
    foreach ($find in $registrationFixes.Keys) {
        $replace = $registrationFixes[$find]
        if ($content -match $find) {
            $content = $content -replace $find, $replace
            Write-Host "  📝 Fixed service registration: $($find.Split('<')[1].Split('.')[1])" -ForegroundColor Green
        }
    }
    
    Set-Content -Path $extensionsFile -Value $content -NoNewline
    Write-Host "  📝 ServiceCollectionExtensions.cs updated with full namespaces" -ForegroundColor Yellow
}

Write-Host "`n✅ Full namespace qualification completed!" -ForegroundColor Green
Write-Host "🔨 Run 'dotnet build' to verify compilation..." -ForegroundColor Cyan