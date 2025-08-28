Write-Host "Cleaning up ReactiveUI references..." -ForegroundColor Green

# Find all C# files with ReactiveUI using statements
$files = Get-ChildItem -Path . -Recurse -Include "*.cs" | Where-Object {
    (Get-Content $_.FullName -Raw) -match "using ReactiveUI;"
}

Write-Host "Found $($files.Count) files with ReactiveUI using statements" -ForegroundColor Yellow

foreach ($file in $files) {
    Write-Host "  Updating: $($file.Name)" -ForegroundColor Cyan
    
    $content = Get-Content $file.FullName -Raw
    
    # Replace ReactiveUI using with Avalonia.ReactiveUI
    $content = $content -replace "using ReactiveUI;", "using Avalonia.ReactiveUI;"
    
    # Write back to file
    Set-Content $file.FullName -Value $content -Encoding UTF8
}

Write-Host "Updated $($files.Count) files" -ForegroundColor Green

# Verify no standalone ReactiveUI references remain
Write-Host "`nVerifying cleanup..." -ForegroundColor Yellow
$remaining = Get-ChildItem -Path . -Recurse -Include "*.cs" | Where-Object {
    (Get-Content $_.FullName -Raw) -match "using ReactiveUI;"
}

if ($remaining.Count -eq 0) {
    Write-Host "No standalone ReactiveUI using statements found" -ForegroundColor Green
} else {
    Write-Host "Still found $($remaining.Count) files with ReactiveUI using statements:" -ForegroundColor Red
    $remaining | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor Red }
}

Write-Host "`nCleanup complete!" -ForegroundColor Green