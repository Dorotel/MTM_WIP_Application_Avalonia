@echo off
setlocal enabledelayedexpansion

echo.
echo ================================================================
echo MTM Theme Synchronization Tool
echo ================================================================
echo.
echo This tool synchronizes all MTM themes to use the same structure
echo as MTM_Amber.axaml (master template) with theme-appropriate colors
echo.

:menu
echo Please choose an option:
echo.
echo [1] Preview synchronization (see what would change)
echo [2] Synchronize all themes with master template
echo [3] Exit
echo.
set /p choice="Enter your choice (1-3): "

if "%choice%"=="1" goto preview
if "%choice%"=="2" goto synchronize
if "%choice%"=="3" goto exit
echo Invalid choice. Please try again.
echo.
goto menu

:preview
echo.
echo Running theme synchronization preview...
echo ========================================
powershell -ExecutionPolicy Bypass -File "C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Scripts\Synchronize-MTM-Themes.ps1" -WhatIf -Verbose
echo.
echo Preview completed. Press any key to return to menu...
pause >nul
goto menu

:synchronize
echo.
echo WARNING: This will modify all theme files to match the master template structure.
echo Make sure you have committed any important changes to version control.
echo.
set /p confirm="Are you sure you want to proceed? (y/N): "
if /i not "%confirm%"=="y" (
    echo Operation cancelled.
    echo.
    goto menu
)

echo.
echo Synchronizing all themes with master template...
echo ===============================================
powershell -ExecutionPolicy Bypass -File "C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Scripts\Synchronize-MTM-Themes.ps1" -Verbose

if !errorlevel! equ 0 (
    echo.
    echo ================================================================
    echo SUCCESS: All themes have been synchronized successfully!
    echo ================================================================
    echo.
    echo All MTM themes now have:
    echo - Same brush structure as MTM_Amber.axaml master template
    echo - Theme-appropriate color schemes
    echo - Complete preview sections with specialized content
    echo - 50+ standardized brushes each
    echo.
) else (
    echo.
    echo ================================================================
    echo ERROR: Theme synchronization encountered errors
    echo ================================================================
    echo Please check the output above for details.
    echo.
)

echo Press any key to continue...
pause >nul
goto menu

:exit
echo.
echo Exiting MTM Theme Synchronization Tool...
echo.
exit /b 0
