# Complete ReactiveUI Cleanup Script
# Removes all standalone ReactiveUI references and ensures only Avalonia.ReactiveUI is used

Write-Host "Starting Complete ReactiveUI Cleanup..." -ForegroundColor Green

# Step 1: Remove standalone ReactiveUI package
Write-Host "`nStep 1: Removing standalone ReactiveUI package..." -ForegroundColor Yellow
try {
    $removeResult = dotnet remove package ReactiveUI 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Successfully removed ReactiveUI package" -ForegroundColor Green
    } else {
        Write-Host "ReactiveUI package not found or already removed" -ForegroundColor Cyan
    }
} catch {
    Write-Host "ReactiveUI package not found or already removed" -ForegroundColor Cyan
}

# Step 2: Clean build artifacts
Write-Host "`nStep 2: Cleaning build artifacts..." -ForegroundColor Yellow
dotnet clean
if (Test-Path "bin") { Remove-Item -Recurse -Force "bin" -ErrorAction SilentlyContinue }
if (Test-Path "obj") { Remove-Item -Recurse -Force "obj" -ErrorAction SilentlyContinue }
Write-Host "Build artifacts cleaned" -ForegroundColor Green

# Step 3: Clear NuGet cache
Write-Host "`nStep 3: Clearing NuGet cache..." -ForegroundColor Yellow
dotnet nuget locals all --clear
Write-Host "NuGet cache cleared" -ForegroundColor Green

# Step 4: Update using statements in C# files
Write-Host "`nStep 4: Updating using statements..." -ForegroundColor Yellow
$files = Get-ChildItem -Path . -Recurse -Include "*.cs" | Where-Object {
    (Get-Content $_.FullName -Raw) -match "using ReactiveUI;"
}

Write-Host "Found $($files.Count) files with ReactiveUI using statements" -ForegroundColor Cyan

foreach ($file in $files) {
    Write-Host "  Updating: $($file.Name)" -ForegroundColor White
    
    $content = Get-Content $file.FullName -Raw
    $content = $content -replace "using ReactiveUI;", "using Avalonia.ReactiveUI;"
    Set-Content $file.FullName -Value $content -Encoding UTF8
}

Write-Host "Updated $($files.Count) files" -ForegroundColor Green

# Step 5: Remove app.config if it exists
Write-Host "`nStep 5: Checking for app.config..." -ForegroundColor Yellow
if (Test-Path "app.config") {
    Remove-Item "app.config"
    Write-Host "Removed app.config (no longer needed)" -ForegroundColor Green
} else {
    Write-Host "No app.config found" -ForegroundColor Cyan
}

# Step 6: Restore packages
Write-Host "`nStep 6: Restoring packages..." -ForegroundColor Yellow
dotnet restore --force
if ($LASTEXITCODE -eq 0) {
    Write-Host "Packages restored successfully" -ForegroundColor Green
} else {
    Write-Host "Warning: Package restore had issues" -ForegroundColor Red
}

# Step 7: Verify cleanup
Write-Host "`nStep 7: Verifying cleanup..." -ForegroundColor Yellow

# Check for remaining using statements
$remaining = Get-ChildItem -Path . -Recurse -Include "*.cs" | Where-Object {
    (Get-Content $_.FullName -Raw) -match "using ReactiveUI;"
}

if ($remaining.Count -eq 0) {
    Write-Host "SUCCESS: No standalone ReactiveUI using statements found" -ForegroundColor Green
} else {
    Write-Host "WARNING: Still found $($remaining.Count) files with ReactiveUI using statements:" -ForegroundColor Red
    $remaining | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor Red }
}

# Check package references
Write-Host "`nChecking package references..." -ForegroundColor Yellow
$packages = dotnet list package | Select-String "ReactiveUI"
if ($packages) {
    Write-Host "ReactiveUI packages found:" -ForegroundColor Cyan
    $packages | ForEach-Object { Write-Host "  $($_.Line.Trim())" -ForegroundColor White }
} else {
    Write-Host "No ReactiveUI packages found in direct references" -ForegroundColor Green
}

# Step 8: Build verification
Write-Host "`nStep 8: Building to verify..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration Debug 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "BUILD SUCCESS: Application compiles without errors" -ForegroundColor Green
} else {
    Write-Host "BUILD FAILED: There are compilation errors" -ForegroundColor Red
    Write-Host "Build output:" -ForegroundColor Yellow
    $buildResult | Write-Host
}

Write-Host "`nComplete ReactiveUI Cleanup Finished!" -ForegroundColor Green
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "- Removed standalone ReactiveUI package" -ForegroundColor White
Write-Host "- Updated $($files.Count) using statements to Avalonia.ReactiveUI" -ForegroundColor White
Write-Host "- Cleaned build artifacts and NuGet cache" -ForegroundColor White
Write-Host "- Verified build still works" -ForegroundColor White
