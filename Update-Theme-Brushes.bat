@echo off
echo ========================================
echo MTM Theme Brush Name Update Script
echo ========================================
echo.

REM Check if PowerShell is available
powershell -Command "Write-Host 'PowerShell is available'" >nul 2>&1
if errorlevel 1 (
    echo ERROR: PowerShell is not available or not in PATH
    echo Please ensure PowerShell is installed and accessible
    pause
    exit /b 1
)

echo Choose an option:
echo 1. Preview changes (no files will be modified)
echo 2. Apply changes to all files
echo 3. Apply changes with verbose output
echo.
set /p choice="Enter your choice (1-3): "

if "%choice%"=="1" (
    echo.
    echo Running in PREVIEW mode...
    powershell -ExecutionPolicy Bypass -File "C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Scripts\Update-MTM-Theme-Brushes.ps1" -WhatIf
) else if "%choice%"=="2" (
    echo.
    echo Applying changes to all files...
    powershell -ExecutionPolicy Bypass -File "C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Scripts\Update-MTM-Theme-Brushes.ps1"
) else if "%choice%"=="3" (
    echo.
    echo Applying changes with verbose output...
    powershell -ExecutionPolicy Bypass -File "C:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\Scripts\Update-MTM-Theme-Brushes.ps1" -Verbose
) else (
    echo Invalid choice. Please run the script again.
    pause
    exit /b 1
)

echo.
echo Script execution completed.
pause
