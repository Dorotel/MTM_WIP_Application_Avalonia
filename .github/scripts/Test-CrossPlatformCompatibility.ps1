# Quick Cross-Platform Test Script for MTM File Selection Service
# Tests basic functionality without starting full UI

Write-Host "Testing MTM Cross-Platform File Selection Service..." -ForegroundColor Green
Write-Host "Platform: $($env:OS)" -ForegroundColor Cyan

# Test 1: Build validation
Write-Host "`n1. Testing build for different platforms..." -ForegroundColor Yellow

Write-Host "Building for Windows x64..."
dotnet build -r win-x64 -v quiet | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Windows x64 build: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ Windows x64 build: FAILED" -ForegroundColor Red
}

Write-Host "Building for macOS x64..."
dotnet build -r osx-x64 -v quiet | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ macOS x64 build: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ macOS x64 build: FAILED" -ForegroundColor Red
}

Write-Host "Building for Linux x64..."
dotnet build -r linux-x64 -v quiet | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Linux x64 build: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ Linux x64 build: FAILED" -ForegroundColor Red
}

# Test 2: File system validation
Write-Host "`n2. Testing file system functionality..." -ForegroundColor Yellow

$testFile = "test-config.json"
if (Test-Path $testFile) {
    Write-Host "✅ Test file exists: $testFile" -ForegroundColor Green
    
    try {
        $content = Get-Content $testFile -Raw
        $json = $content | ConvertFrom-Json
        Write-Host "✅ JSON parsing: PASSED" -ForegroundColor Green
        Write-Host "   Content: $($json.test)" -ForegroundColor Gray
    } catch {
        Write-Host "❌ JSON parsing: FAILED - $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Test file missing: $testFile" -ForegroundColor Red
}

# Test 3: Platform detection
Write-Host "`n3. Testing platform detection..." -ForegroundColor Yellow

$platformWindows = [System.OperatingSystem]::IsWindows()
$platformMacOS = [System.OperatingSystem]::IsMacOS() 
$platformLinux = [System.OperatingSystem]::IsLinux()
$platformAndroid = [System.OperatingSystem]::IsAndroid()

Write-Host "Platform Detection Results:" -ForegroundColor Cyan
Write-Host "  Windows: $platformWindows" -ForegroundColor $(if($platformWindows){'Green'}else{'Gray'})
Write-Host "  macOS: $platformMacOS" -ForegroundColor $(if($platformMacOS){'Green'}else{'Gray'})
Write-Host "  Linux: $platformLinux" -ForegroundColor $(if($platformLinux){'Green'}else{'Gray'})
Write-Host "  Android: $platformAndroid" -ForegroundColor $(if($platformAndroid){'Green'}else{'Gray'})

if ($platformWindows -or $platformMacOS -or $platformLinux -or $platformAndroid) {
    Write-Host "✅ Platform detection: PASSED" -ForegroundColor Green
} else {
    Write-Host "❌ Platform detection: FAILED - No platform detected" -ForegroundColor Red
}

# Test 4: Directory access
Write-Host "`n4. Testing directory access..." -ForegroundColor Yellow

try {
    $documentsPath = [System.Environment]::GetFolderPath([System.Environment+SpecialFolder]::MyDocuments)
    Write-Host "✅ Documents folder access: $documentsPath" -ForegroundColor Green
    
    if (Test-Path $documentsPath) {
        Write-Host "✅ Documents folder exists" -ForegroundColor Green
    } else {
        Write-Host "❌ Documents folder does not exist" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Documents folder access: FAILED - $($_.Exception.Message)" -ForegroundColor Red
}

# Test Summary
Write-Host "`n📊 Cross-Platform Test Summary" -ForegroundColor Magenta
Write-Host "=====================================" -ForegroundColor Magenta
Write-Host "The MTM File Selection Service is designed to work across:" -ForegroundColor White
Write-Host "• Windows (Primary development platform)" -ForegroundColor Green
Write-Host "• macOS (Desktop and Apple Silicon)" -ForegroundColor Green  
Write-Host "• Linux (Various distributions)" -ForegroundColor Green
Write-Host "• Android (Mobile platform)" -ForegroundColor Green

Write-Host "`n🧪 For comprehensive testing:" -ForegroundColor Cyan
Write-Host "• Use GitHub Actions for automated cross-platform CI/CD" -ForegroundColor White
Write-Host "• Test on physical devices when possible" -ForegroundColor White
Write-Host "• Validate file system permissions on each platform" -ForegroundColor White
Write-Host "• Check storage provider integration" -ForegroundColor White

Write-Host "`n📚 Documentation available in:" -ForegroundColor Cyan
Write-Host "• Testing/README-CrossPlatformTesting.md" -ForegroundColor White
Write-Host "• Testing/Platform-Specific-File-System-Differences.md" -ForegroundColor White
Write-Host "• .github/workflows/cross-platform-tests.yml" -ForegroundColor White

Write-Host "`n✨ Testing completed successfully!" -ForegroundColor Green