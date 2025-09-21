@echo off
echo ========================================
echo MTM Final 45 Compilation Errors Fix
echo ========================================
echo.

:: Set parameters
set "WHAT_IF=%1"
set "BASE_PATH=C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia"

if "%WHAT_IF%"=="-WhatIf" (
    echo PREVIEW MODE: Showing file changes that WOULD be made
    echo.
    echo Phase 1: InventoryItem namespace fix in Business.InventoryEditingService.cs
    echo   WOULD CHANGE: using MTM_WIP_Application_Avalonia.Models;
    echo   TO:           using MTM_WIP_Application_Avalonia.Models.Events;
    echo.
    echo Phase 2: Interface namespace fixes in MainWindowViewModel.cs
    echo   WOULD CHANGE: using MTM_WIP_Application_Avalonia.Services.Interfaces;
    echo   TO:           using MTM_WIP_Application_Avalonia.Services.Infrastructure;
    echo.
    echo Phase 3: NavigationEventArgs namespace fix in MainWindowViewModel.cs
    echo   WOULD CHANGE: MTM_WIP_Application_Avalonia.Services.NavigationEventArgs
    echo   TO:           MTM_WIP_Application_Avalonia.Services.Infrastructure.NavigationEventArgs
    echo.
    echo Phase 4: EventArgs namespace fixes in various files
    echo   WOULD CHANGE: ItemsRemovedEventArgs, SuccessEventArgs paths to Models.Events namespace
    echo.
    echo Phase 5: Add missing using statements for Service interfaces
    echo   WOULD ADD: Infrastructure, UI, Business namespace using statements
    echo.
    echo Phase 6: Fix control references (CollapsiblePanel, CustomDataGridColumn, etc.)
    echo.
    goto :preview_end
)

echo EXECUTION MODE: Applying changes...
echo.

echo.
echo === Phase 1: Fix Service Interface Namespace Issues ===
echo.

:: Fix InventoryItem namespace in Business.InventoryEditingService.cs
echo Fixing InventoryItem namespace reference...
powershell -Command "(Get-Content '%BASE_PATH%\Services\Business\Business.InventoryEditingService.cs') -replace 'using MTM_WIP_Application_Avalonia\.Models;', 'using MTM_WIP_Application_Avalonia.Models.Events;' | Set-Content '%BASE_PATH%\Services\Business\Business.InventoryEditingService.cs'"

:: Fix MainWindowViewModel Interfaces namespace
echo Fixing MainWindowViewModel interfaces namespace...
powershell -Command "(Get-Content '%BASE_PATH%\ViewModels\MainForm\MainWindowViewModel.cs') -replace 'using MTM_WIP_Application_Avalonia\.Services\.Interfaces;', 'using MTM_WIP_Application_Avalonia.Services.Infrastructure;' | Set-Content '%BASE_PATH%\ViewModels\MainForm\MainWindowViewModel.cs'"

:: Fix NavigationEventArgs namespace in MainWindowViewModel
echo Fixing NavigationEventArgs namespace...
powershell -Command "(Get-Content '%BASE_PATH%\ViewModels\MainForm\MainWindowViewModel.cs') -replace 'MTM_WIP_Application_Avalonia\.Services\.NavigationEventArgs', 'MTM_WIP_Application_Avalonia.Services.Infrastructure.NavigationEventArgs' | Set-Content '%BASE_PATH%\ViewModels\MainForm\MainWindowViewModel.cs'"

echo.
echo === Phase 2: Fix Event Args Namespace Issues ===
echo.

:: Fix ItemsRemovedEventArgs in Business.RemoveService.cs
echo Fixing ItemsRemovedEventArgs namespace in RemoveService...
powershell -Command "(Get-Content '%BASE_PATH%\Services\Business\Business.RemoveService.cs' %PREVIEW%) -replace 'MTM_WIP_Application_Avalonia\.Models\.ItemsRemovedEventArgs', 'MTM_WIP_Application_Avalonia.Models.Events.ItemsRemovedEventArgs' | Set-Content '%BASE_PATH%\Services\Business\Business.RemoveService.cs' %PREVIEW%"

:: Fix SuccessEventArgs namespace in RemoveTabView.axaml.cs
echo Fixing SuccessEventArgs namespace in RemoveTabView...
powershell -Command "(Get-Content '%BASE_PATH%\Views\MainForm\Panels\RemoveTabView.axaml.cs' %PREVIEW%) -replace 'MTM_WIP_Application_Avalonia\.Models\.SuccessEventArgs', 'MTM_WIP_Application_Avalonia.Models.Events.SuccessEventArgs' | Set-Content '%BASE_PATH%\Views\MainForm\Panels\RemoveTabView.axaml.cs' %PREVIEW%"

:: Fix SelectionChangedEventArgs namespace in RemoveTabView.axaml.cs
echo Fixing SelectionChangedEventArgs namespace in RemoveTabView...
powershell -Command "(Get-Content '%BASE_PATH%\Views\MainForm\Panels\RemoveTabView.axaml.cs' %PREVIEW%) -replace 'MTM_WIP_Application_Avalonia\.Controls\.CustomDataGrid\.SelectionChangedEventArgs', 'MTM_WIP_Application_Avalonia.Models.UI.SelectionChangedEventArgs' | Set-Content '%BASE_PATH%\Views\MainForm\Panels\RemoveTabView.axaml.cs' %PREVIEW%"

:: Fix ItemsRemovedEventArgs in RemoveItemViewModel.cs
echo Fixing ItemsRemovedEventArgs namespace in RemoveItemViewModel...
powershell -Command "(Get-Content '%BASE_PATH%\ViewModels\MainForm\RemoveItemViewModel.cs' %PREVIEW%) -replace 'MTM_WIP_Application_Avalonia\.Services\.Business\.ItemsRemovedEventArgs', 'MTM_WIP_Application_Avalonia.Models.Events.ItemsRemovedEventArgs' | Set-Content '%BASE_PATH%\ViewModels\MainForm\RemoveItemViewModel.cs' %PREVIEW%"

:: Fix SuccessEventArgs in RemoveItemViewModel.cs
echo Fixing SuccessEventArgs namespace in RemoveItemViewModel...
powershell -Command "(Get-Content '%BASE_PATH%\ViewModels\MainForm\RemoveItemViewModel.cs' %PREVIEW%) -replace 'MTM_WIP_Application_Avalonia\.Models\.SuccessEventArgs', 'MTM_WIP_Application_Avalonia.Models.Events.SuccessEventArgs' | Set-Content '%BASE_PATH%\ViewModels\MainForm\RemoveItemViewModel.cs' %PREVIEW%"

echo.
echo === Phase 3: Fix Missing Using Statements ===
echo.

:: Add missing using statements for service interfaces
echo Adding Infrastructure using statement to ViewModels...
powershell -Command "
$files = @(
    '%BASE_PATH%\ViewModels\MainForm\MainViewViewModel.cs',
    '%BASE_PATH%\ViewModels\MainForm\TransferItemViewModel.cs'
)
foreach ($file in $files) {
    $content = Get-Content $file %PREVIEW%
    if ($content -notmatch 'using MTM_WIP_Application_Avalonia\.Services\.Infrastructure;') {
        $content = $content -replace '(using MTM_WIP_Application_Avalonia\.Services\.Business;)', '$1`nusing MTM_WIP_Application_Avalonia.Services.Infrastructure;'
        $content | Set-Content $file %PREVIEW%
    }
}"

:: Add missing using statements for UI services
echo Adding UI using statement to ViewModels and Views...
powershell -Command "
$files = @(
    '%BASE_PATH%\ViewModels\MainForm\TransferItemViewModel.cs',
    '%BASE_PATH%\Views\MainForm\Panels\TransferTabView.axaml.cs'
)
foreach ($file in $files) {
    $content = Get-Content $file %PREVIEW%
    if ($content -notmatch 'using MTM_WIP_Application_Avalonia\.Services\.UI;') {
        $content = $content -replace '(using MTM_WIP_Application_Avalonia\.Services\.Business;)', '$1`nusing MTM_WIP_Application_Avalonia.Services.UI;'
        $content | Set-Content $file %PREVIEW%
    }
}"

:: Add missing using statements for Business services
echo Adding Business using statement to QuickButtonsViewModel...
powershell -Command "
$file = '%BASE_PATH%\ViewModels\MainForm\QuickButtonsViewModel.cs'
$content = Get-Content $file %PREVIEW%
if ($content -notmatch 'using MTM_WIP_Application_Avalonia\.Services\.Business;') {
    $content = $content -replace '(using.*Services.*)', '$1`nusing MTM_WIP_Application_Avalonia.Services.Business;'
    $content | Set-Content $file %PREVIEW%
}
"

echo.
echo === Phase 4: Fix Missing Control References ===
echo.

:: Fix CollapsiblePanel namespace
echo Fixing CollapsiblePanel namespace references...
powershell -Command "
$files = @(
    '%BASE_PATH%\Views\MainForm\Panels\RemoveTabView.axaml.cs',
    '%BASE_PATH%\Views\MainForm\Panels\TransferTabView.axaml.cs'
)
foreach ($file in $files) {
    $content = Get-Content $file %PREVIEW%
    if ($content -notmatch 'using MTM_WIP_Application_Avalonia\.Views\.Overlay;') {
        $content = $content -replace '(using.*Views.*)', '$1`nusing MTM_WIP_Application_Avalonia.Views.Overlay;'
        $content | Set-Content $file %PREVIEW%
    }
}"

:: Fix CustomDataGridColumn namespace
echo Fixing CustomDataGridColumn namespace references...
powershell -Command "
$file = '%BASE_PATH%\ViewModels\Shared\CustomDataGridViewModel.cs'
$content = Get-Content $file %PREVIEW%
if ($content -notmatch 'using MTM_WIP_Application_Avalonia\.Models\.UI;') {
    $content = $content -replace '(using.*Models.*)', '$1`nusing MTM_WIP_Application_Avalonia.Models.UI;'
    $content | Set-Content $file %PREVIEW%
}
"

:: Fix EmergencyKeyboardHook reference
echo Fixing EmergencyKeyboardHook reference...
powershell -Command "(Get-Content '%BASE_PATH%\ViewModels\Overlay\SuccessOverlayViewModel.cs' %PREVIEW%) -replace 'MTM_WIP_Application_Avalonia\.Services\.EmergencyKeyboardHook', 'MTM_WIP_Application_Avalonia.Services.Infrastructure.EmergencyKeyboardHookService' | Set-Content '%BASE_PATH%\ViewModels\Overlay\SuccessOverlayViewModel.cs' %PREVIEW%"

echo.
echo === Phase 5: Fix Overlay Interface Reference ===
echo.

:: Add missing using statement for Feature services in BaseOverlayViewModel
echo Adding Feature using statement to BaseOverlayViewModel...
powershell -Command "
$file = '%BASE_PATH%\ViewModels\Overlay\BaseOverlayViewModel.cs'
$content = Get-Content $file %PREVIEW%
if ($content -notmatch 'using MTM_WIP_Application_Avalonia\.Services\.Feature;') {
    $content = $content -replace '(using.*ViewModels.*)', '$1`nusing MTM_WIP_Application_Avalonia.Services.Feature;'
    $content | Set-Content $file %PREVIEW%
}
"

echo.
if "%WHAT_IF%"=="-WhatIf" (
    echo ========================================
    echo PREVIEW MODE COMPLETED
    echo All changes shown above are PREVIEW ONLY
    echo Run without -WhatIf to apply changes
    echo ========================================
) else (
    echo ========================================
    echo VALIDATION: Checking build status...
    echo ========================================

    cd "%BASE_PATH%"
    dotnet build --no-restore 2>&1 | findstr /c:"error"
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo ❌ Errors still exist. Check output above.
    ) else (
        echo.
        echo ✅ Build completed successfully - No compilation errors!
    )
)

echo.
echo Script completed.
pause
