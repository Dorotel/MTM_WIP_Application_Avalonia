# MTM WIP Application - Interactive Individual Package Update Script
# ================================================================

Write-Host "MTM WIP Application - Individual Package Update Script" -ForegroundColor Green
Write-Host "======================================================" -ForegroundColor Green
Write-Host ""

Write-Host "This script will update each NuGet package individually." -ForegroundColor Yellow
Write-Host "You can choose which packages to update." -ForegroundColor Yellow
Write-Host ""

$showCurrent = Read-Host "Do you want to see current package versions first? (Y/N)"
if ($showCurrent -match "^[Yy]") {
    Write-Host ""
    Write-Host "Current package list:" -ForegroundColor Cyan
    dotnet list MTM_WIP_Application_Avalonia.csproj package
    Write-Host ""
}

Write-Host "Starting individual package updates..." -ForegroundColor Green
Write-Host ""

# Function to update a package
function Update-Package {
    param(
        [string]$PackageName,
        [string]$CurrentVersion
    )

    Write-Host "----------------------------------------" -ForegroundColor DarkGray
    Write-Host "Package: $PackageName" -ForegroundColor White
    Write-Host "Current version: $CurrentVersion" -ForegroundColor Gray
    Write-Host "----------------------------------------" -ForegroundColor DarkGray

    $choice = Read-Host "Update $PackageName? (Y/N/S=Skip)"

    switch ($choice.ToUpper()) {
        "Y" {
            Write-Host "Updating $PackageName..." -ForegroundColor Yellow
            dotnet add MTM_WIP_Application_Avalonia.csproj package $PackageName
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ $PackageName updated successfully" -ForegroundColor Green
            }
            else {
                Write-Host "❌ Failed to update $PackageName" -ForegroundColor Red
            }
        }
        "YES" {
            Write-Host "Updating $PackageName..." -ForegroundColor Yellow
            dotnet add MTM_WIP_Application_Avalonia.csproj package $PackageName
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ $PackageName updated successfully" -ForegroundColor Green
            }
            else {
                Write-Host "❌ Failed to update $PackageName" -ForegroundColor Red
            }
        }
        "S" {
            Write-Host "⏭️  Skipping $PackageName" -ForegroundColor DarkYellow
        }
        "SKIP" {
            Write-Host "⏭️  Skipping $PackageName" -ForegroundColor DarkYellow
        }
        default {
            Write-Host "⏭️  Skipping $PackageName" -ForegroundColor DarkYellow
        }
    }
    Write-Host ""
}

# Core Avalonia packages
Update-Package "Avalonia" "11.3.4"
Update-Package "Avalonia.Controls.DataGrid" "11.3.4"
Update-Package "Avalonia.Desktop" "11.3.4"
Update-Package "Avalonia.Headless" "11.3.6"
Update-Package "Avalonia.Themes.Fluent" "11.3.4"
Update-Package "Avalonia.Fonts.Inter" "11.3.4"
Update-Package "Avalonia.Diagnostics" "11.3.4"
Update-Package "Avalonia.Xaml.Interactivity" "11.3.0.6"

# Dialog and UI packages
Update-Package "DialogHost.Avalonia" "0.9.3"

# Microsoft Extensions packages
Update-Package "Microsoft.Extensions.Hosting" "9.0.8"
Update-Package "Microsoft.Extensions.Logging.Debug" "9.0.8"
Update-Package "Microsoft.Extensions.Caching.Memory" "9.0.8"
Update-Package "Microsoft.Extensions.Configuration" "9.0.8"
Update-Package "Microsoft.Extensions.Configuration.Binder" "9.0.8"
Update-Package "Microsoft.Extensions.Configuration.Json" "9.0.8"
Update-Package "Microsoft.Extensions.Configuration.EnvironmentVariables" "9.0.8"
Update-Package "Microsoft.Extensions.DependencyInjection" "9.0.8"
Update-Package "Microsoft.Extensions.Hosting.Abstractions" "9.0.8"
Update-Package "Microsoft.Extensions.Logging" "9.0.8"
Update-Package "Microsoft.Extensions.Logging.Console" "9.0.8"
Update-Package "Microsoft.Extensions.Options.ConfigurationExtensions" "9.0.8"

# MVVM and Utility packages
Update-Package "CommunityToolkit.Mvvm" "8.3.2"

# Database packages
Update-Package "Dapper" "2.1.66"
Update-Package "MySql.Data" "9.4.0"

# Testing packages
Update-Package "Moq" "4.20.72"
Update-Package "xunit" "2.9.3"
Update-Package "xunit.runner.visualstudio" "3.1.5"

# Material Design packages
Update-Package "Material.Icons.Avalonia" "2.4.1"
Update-Package "Material.Avalonia" "3.13.1"
Update-Package "Material.Avalonia.DataGrid" "3.13.1"
Update-Package "Material.Avalonia.Dialogs" "3.8.0"

# System packages
Update-Package "System.Drawing.Common" "8.0.0"

Write-Host ""
Write-Host "============================================" -ForegroundColor Green
Write-Host "All package updates completed!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Green
Write-Host ""

Write-Host "Running final restore and build..." -ForegroundColor Cyan
dotnet restore MTM_WIP_Application_Avalonia.csproj
dotnet build MTM_WIP_Application_Avalonia.csproj --configuration Debug

Write-Host ""
Write-Host "Final package list:" -ForegroundColor Cyan
dotnet list MTM_WIP_Application_Avalonia.csproj package

Write-Host ""
Write-Host "Update process completed!" -ForegroundColor Green
Read-Host "Press Enter to exit"
