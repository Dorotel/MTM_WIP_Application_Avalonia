# Constitutional Validation Script
# Validates MTM WIP Application code against constitutional requirements
# Version: 1.0.0
# Date: September 28, 2025

[CmdletBinding()]
param(
  [Parameter(Mandatory = $false)]
  [ValidateSet("FULL", "INCREMENTAL", "QUICK")]
  [string]$ValidationScope = "FULL",

  [Parameter(Mandatory = $false)]
  [string]$ProjectPath = ".",

  [Parameter(Mandatory = $false)]
  [switch]$Json,

  [Parameter(Mandatory = $false)]
  [switch]$Detailed,

  [Parameter(Mandatory = $false)]
  [string]$OutputFile,

  [Parameter(Mandatory = $false)]
  [switch]$FailFast
)

# Constitutional validation results
$ValidationResults = @{
  Timestamp           = Get-Date -Format "yyyy-MM-dd HH:mm:ss UTC"
  ValidationScope     = $ValidationScope
  ProjectPath         = Resolve-Path $ProjectPath
  OverallStatus       = "PENDING"
  Principles          = @{
    CodeQualityExcellence   = @{ Status = "PENDING"; Violations = @(); Score = 0 }
    TestingStandards        = @{ Status = "PENDING"; Violations = @(); Score = 0 }
    UXConsistency           = @{ Status = "PENDING"; Violations = @(); Score = 0 }
    PerformanceRequirements = @{ Status = "PENDING"; Violations = @(); Score = 0 }
  }
  ManufacturingDomain = @{ Status = "PENDING"; Violations = @(); Score = 0 }
  CrossPlatform       = @{ Status = "PENDING"; Violations = @(); Score = 0 }
  Summary             = @{
    TotalViolations    = 0
    CriticalViolations = 0
    WarningViolations  = 0
    PassedChecks       = 0
    TotalChecks        = 0
  }
}

function Write-ConstitutionalHeader {
  Write-Host "====================================================" -ForegroundColor Blue
  Write-Host "MTM WIP APPLICATION CONSTITUTIONAL VALIDATION" -ForegroundColor Blue
  Write-Host "====================================================" -ForegroundColor Blue
  Write-Host "Version: 1.0.0" -ForegroundColor Gray
  Write-Host "Scope: $ValidationScope" -ForegroundColor Gray
  Write-Host "Path: $($ValidationResults.ProjectPath)" -ForegroundColor Gray
  Write-Host "Time: $($ValidationResults.Timestamp)" -ForegroundColor Gray
  Write-Host ""
}

function Add-Violation {
  param(
    [string]$Principle,
    [string]$Severity,
    [string]$Description,
    [string]$File = "",
    [int]$Line = 0,
    [string]$Suggestion = ""
  )

  $violation = @{
    Severity                = $Severity
    Description             = $Description
    File                    = $File
    Line                    = $Line
    Suggestion              = $Suggestion
    ConstitutionalReference = "Article $Principle"
  }

  $ValidationResults.Principles[$Principle].Violations += $violation

  if ($Severity -eq "CRITICAL") {
    $ValidationResults.Summary.CriticalViolations++
  }
  elseif ($Severity -eq "WARNING") {
    $ValidationResults.Summary.WarningViolations++
  }

  $ValidationResults.Summary.TotalViolations++

  if ($Detailed) {
    $color = if ($Severity -eq "CRITICAL") { "Red" } elseif ($Severity -eq "WARNING") { "Yellow" } else { "Gray" }
    Write-Host "  [$Severity] $Description" -ForegroundColor $color
    if ($File) { Write-Host "    File: $File$(if ($Line -gt 0) { ":$Line" })" -ForegroundColor Gray }
    if ($Suggestion) { Write-Host "    Suggestion: $Suggestion" -ForegroundColor Gray }
  }
}

function Test-CodeQualityExcellence {
  Write-Host "🔍 Constitutional Principle I: Code Quality Excellence" -ForegroundColor Cyan
  $principle = "CodeQualityExcellence"
  $score = 0
  $maxScore = 100

  # Check .NET 8.0 requirement
  $csprojFiles = Get-ChildItem -Path $ProjectPath -Filter "*.csproj" -Recurse
  foreach ($csproj in $csprojFiles) {
    $content = Get-Content $csproj.FullName -Raw
    if ($content -match '<TargetFramework>net8\.0</TargetFramework>') {
      $score += 15
      Write-Host "  ✅ .NET 8.0 target framework detected: $($csproj.Name)" -ForegroundColor Green
    }
    else {
      Add-Violation -Principle $principle -Severity "CRITICAL" -Description ".NET 8.0 target framework required" -File $csproj.FullName -Suggestion "Set <TargetFramework>net8.0</TargetFramework>"
    }

    # Check nullable reference types
    if ($content -match '<Nullable>enable</Nullable>') {
      $score += 15
      Write-Host "  ✅ Nullable reference types enabled: $($csproj.Name)" -ForegroundColor Green
    }
    else {
      Add-Violation -Principle $principle -Severity "CRITICAL" -Description "Nullable reference types must be enabled" -File $csproj.FullName -Suggestion "Add <Nullable>enable</Nullable>"
    }

    # Check MVVM Community Toolkit
    if ($content -match 'CommunityToolkit\.Mvvm') {
      $score += 10
      Write-Host "  ✅ MVVM Community Toolkit detected: $($csproj.Name)" -ForegroundColor Green
    }
    else {
      Add-Violation -Principle $principle -Severity "WARNING" -Description "MVVM Community Toolkit package not detected" -File $csproj.FullName -Suggestion "Add CommunityToolkit.Mvvm package reference"
    }
  }

  # Check for ReactiveUI violations (prohibited)
  $csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse
  foreach ($csFile in $csFiles) {
    $content = Get-Content $csFile.FullName -Raw
    $lineNumber = 1

    foreach ($line in (Get-Content $csFile.FullName)) {
      if ($line -match 'ReactiveObject|ReactiveCommand|RaiseAndSetIfChanged') {
        Add-Violation -Principle $principle -Severity "CRITICAL" -Description "ReactiveUI patterns prohibited" -File $csFile.FullName -Line $lineNumber -Suggestion "Use MVVM Community Toolkit patterns instead"
      }

      # Check for centralized error handling
      if ($line -match 'Services\.ErrorHandling\.HandleErrorAsync') {
        $score += 1  # Small incremental points for good patterns
      }

      # Check for proper naming conventions
      if ($line -match 'class\s+([a-z][a-zA-Z0-9]*)') {
        Add-Violation -Principle $principle -Severity "WARNING" -Description "Class names should use PascalCase" -File $csFile.FullName -Line $lineNumber -Suggestion "Use PascalCase for class names"
      }

      $lineNumber++
    }
  }

  # Calculate final score
  $finalScore = [Math]::Min([Math]::Max($score, 0), $maxScore)
  $ValidationResults.Principles[$principle].Score = $finalScore

  if ($finalScore -ge 80) {
    $ValidationResults.Principles[$principle].Status = "PASS"
    Write-Host "  🎉 Code Quality Excellence: PASSED (Score: $finalScore/$maxScore)" -ForegroundColor Green
  }
  elseif ($finalScore -ge 60) {
    $ValidationResults.Principles[$principle].Status = "WARNING"
    Write-Host "  ⚠️ Code Quality Excellence: WARNING (Score: $finalScore/$maxScore)" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.Principles[$principle].Status = "FAIL"
    Write-Host "  ❌ Code Quality Excellence: FAILED (Score: $finalScore/$maxScore)" -ForegroundColor Red
  }

  $ValidationResults.Summary.TotalChecks++
  if ($ValidationResults.Principles[$principle].Status -eq "PASS") {
    $ValidationResults.Summary.PassedChecks++
  }
}

function Test-TestingStandards {
  Write-Host "🧪 Constitutional Principle II: Testing Standards" -ForegroundColor Cyan
  $principle = "TestingStandards"
  $score = 0
  $maxScore = 100

  # Check for test projects
  $testProjects = Get-ChildItem -Path $ProjectPath -Filter "*Test*.csproj" -Recurse
  if ($testProjects.Count -gt 0) {
    $score += 25
    Write-Host "  ✅ Test projects detected: $($testProjects.Count)" -ForegroundColor Green

    foreach ($testProject in $testProjects) {
      $content = Get-Content $testProject.FullName -Raw

      # Check for test frameworks
      if ($content -match 'Microsoft\.NET\.Test\.Sdk|xunit|NUnit|MSTest') {
        $score += 15
        Write-Host "  ✅ Test framework detected: $($testProject.Name)" -ForegroundColor Green
      }

      # Check for MVVM testing patterns
      if ($content -match 'CommunityToolkit\.Mvvm\.UnitTests|MVVM.*Test') {
        $score += 10
        Write-Host "  ✅ MVVM testing patterns detected: $($testProject.Name)" -ForegroundColor Green
      }
    }
  }
  else {
    Add-Violation -Principle $principle -Severity "CRITICAL" -Description "No test projects found" -Suggestion "Create test projects with naming pattern *Test*.csproj"
  }

  # Check for test files
  $testFiles = Get-ChildItem -Path $ProjectPath -Filter "*Test*.cs" -Recurse
  if ($testFiles.Count -gt 0) {
    $score += 25
    Write-Host "  ✅ Test files detected: $($testFiles.Count)" -ForegroundColor Green

    foreach ($testFile in $testFiles) {
      $content = Get-Content $testFile.FullName -Raw

      # Check for test attributes
      if ($content -match '\[Test\]|\[Fact\]|\[TestMethod\]') {
        $score += 2
      }

      # Check for async test patterns
      if ($content -match 'async Task.*Test|TestAsync') {
        $score += 2
      }
    }
  }
  else {
    Add-Violation -Principle $principle -Severity "CRITICAL" -Description "No test files found" -Suggestion "Create test files with naming pattern *Test*.cs"
  }

  # Simulate coverage check (would integrate with actual coverage tools)
  if ($testFiles.Count -gt 0) {
    $estimatedCoverage = [Math]::Min(($testFiles.Count * 10), 90)  # Simplified estimation
    if ($estimatedCoverage -ge 80) {
      $score += 25
      Write-Host "  ✅ Estimated test coverage: $estimatedCoverage%" -ForegroundColor Green
    }
    else {
      Add-Violation -Principle $principle -Severity "WARNING" -Description "Test coverage below 80% (estimated: $estimatedCoverage%)" -Suggestion "Add more unit tests to increase coverage"
    }
  }

  $finalScore = [Math]::Min([Math]::Max($score, 0), $maxScore)
  $ValidationResults.Principles[$principle].Score = $finalScore

  if ($finalScore -ge 80) {
    $ValidationResults.Principles[$principle].Status = "PASS"
    Write-Host "  🎉 Testing Standards: PASSED (Score: $finalScore/$maxScore)" -ForegroundColor Green
  }
  elseif ($finalScore -ge 60) {
    $ValidationResults.Principles[$principle].Status = "WARNING"
    Write-Host "  ⚠️ Testing Standards: WARNING (Score: $finalScore/$maxScore)" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.Principles[$principle].Status = "FAIL"
    Write-Host "  ❌ Testing Standards: FAILED (Score: $finalScore/$maxScore)" -ForegroundColor Red
  }

  $ValidationResults.Summary.TotalChecks++
  if ($ValidationResults.Principles[$principle].Status -eq "PASS") {
    $ValidationResults.Summary.PassedChecks++
  }
}

function Test-UXConsistency {
  Write-Host "🎨 Constitutional Principle III: UX Consistency" -ForegroundColor Cyan
  $principle = "UXConsistency"
  $score = 0
  $maxScore = 100

  # Check for Avalonia UI
  $csprojFiles = Get-ChildItem -Path $ProjectPath -Filter "*.csproj" -Recurse
  foreach ($csproj in $csprojFiles) {
    $content = Get-Content $csproj.FullName -Raw
    if ($content -match 'Avalonia.*11\.3\.4|Avalonia.*11\.3') {
      $score += 25
      Write-Host "  ✅ Avalonia UI 11.3.4 detected: $($csproj.Name)" -ForegroundColor Green
    }
    elseif ($content -match 'Avalonia') {
      $score += 15
      Add-Violation -Principle $principle -Severity "WARNING" -Description "Avalonia UI version should be 11.3.4" -File $csproj.FullName -Suggestion "Upgrade to Avalonia UI 11.3.4"
    }

    # Check for Material Design icons
    if ($content -match 'Material\.Icons|MaterialDesign') {
      $score += 15
      Write-Host "  ✅ Material Design integration detected: $($csproj.Name)" -ForegroundColor Green
    }
    else {
      Add-Violation -Principle $principle -Severity "WARNING" -Description "Material Design icons not detected" -File $csproj.FullName -Suggestion "Add Material.Icons.Avalonia package"
    }
  }

  # Check for AXAML files
  $axamlFiles = Get-ChildItem -Path $ProjectPath -Filter "*.axaml" -Recurse
  if ($axamlFiles.Count -gt 0) {
    $score += 25
    Write-Host "  ✅ AXAML files detected: $($axamlFiles.Count)" -ForegroundColor Green

    foreach ($axamlFile in $axamlFiles) {
      $content = Get-Content $axamlFile.FullName -Raw

      # Check for proper Avalonia namespace
      if ($content -match 'xmlns="https://github\.com/avaloniaui"') {
        $score += 2
      }
      else {
        Add-Violation -Principle $principle -Severity "WARNING" -Description "Incorrect Avalonia namespace" -File $axamlFile.FullName -Suggestion 'Use xmlns="https://github.com/avaloniaui"'
      }

      # Check for theme resources
      if ($content -match 'DynamicResource|StaticResource.*Theme') {
        $score += 1
      }
    }
  }
  else {
    Add-Violation -Principle $principle -Severity "WARNING" -Description "No AXAML files found" -Suggestion "Create Avalonia AXAML views"
  }

  # Check for theme files
  $themeFiles = Get-ChildItem -Path $ProjectPath -Filter "*Theme*.axaml" -Recurse
  if ($themeFiles.Count -gt 0) {
    $score += 20
    Write-Host "  ✅ Theme files detected: $($themeFiles.Count)" -ForegroundColor Green
  }
  else {
    Add-Violation -Principle $principle -Severity "WARNING" -Description "No theme files detected" -Suggestion "Create theme files for consistent styling"
  }

  $finalScore = [Math]::Min([Math]::Max($score, 0), $maxScore)
  $ValidationResults.Principles[$principle].Score = $finalScore

  if ($finalScore -ge 80) {
    $ValidationResults.Principles[$principle].Status = "PASS"
    Write-Host "  🎉 UX Consistency: PASSED (Score: $finalScore/$maxScore)" -ForegroundColor Green
  }
  elseif ($finalScore -ge 60) {
    $ValidationResults.Principles[$principle].Status = "WARNING"
    Write-Host "  ⚠️ UX Consistency: WARNING (Score: $finalScore/$maxScore)" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.Principles[$principle].Status = "FAIL"
    Write-Host "  ❌ UX Consistency: FAILED (Score: $finalScore/$maxScore)" -ForegroundColor Red
  }

  $ValidationResults.Summary.TotalChecks++
  if ($ValidationResults.Principles[$principle].Status -eq "PASS") {
    $ValidationResults.Summary.PassedChecks++
  }
}

function Test-PerformanceRequirements {
  Write-Host "⚡ Constitutional Principle IV: Performance Requirements" -ForegroundColor Cyan
  $principle = "PerformanceRequirements"
  $score = 0
  $maxScore = 100

  # Check for MySQL integration
  $csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse
  $configFiles = Get-ChildItem -Path $ProjectPath -Filter "appsettings*.json" -Recurse

  $mysqlDetected = $false
  foreach ($file in ($csFiles + $configFiles)) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match 'MySql|MySQL') {
      $mysqlDetected = $true
      $score += 15
      Write-Host "  ✅ MySQL integration detected: $($file.Name)" -ForegroundColor Green
      break
    }
  }

  if (-not $mysqlDetected) {
    Add-Violation -Principle $principle -Severity "WARNING" -Description "MySQL integration not detected" -Suggestion "Ensure MySQL 9.4.0 integration"
  }

  # Check for connection pooling configuration
  foreach ($configFile in $configFiles) {
    $content = Get-Content $configFile.FullName -Raw
    if ($content -match 'MinPoolSize|MaxPoolSize|ConnectionPooling') {
      $score += 20
      Write-Host "  ✅ Connection pooling configuration detected: $($configFile.Name)" -ForegroundColor Green
    }

    # Check for timeout configuration
    if ($content -match 'CommandTimeout.*30|Timeout.*30') {
      $score += 15
      Write-Host "  ✅ 30-second timeout configuration detected: $($configFile.Name)" -ForegroundColor Green
    }
    else {
      Add-Violation -Principle $principle -Severity "WARNING" -Description "30-second database timeout not configured" -File $configFile.FullName -Suggestion "Set CommandTimeout to 30 seconds"
    }
  }

  # Check for performance-related code patterns
  foreach ($csFile in $csFiles) {
    $content = Get-Content $csFile.FullName -Raw

    # Check for async patterns
    if ($content -match 'async\s+Task|await\s+') {
      $score += 5
    }

    # Check for memory disposal patterns
    if ($content -match 'using\s*\(|IDisposable') {
      $score += 5
    }

    # Check for caching patterns
    if ($content -match 'Cache|Caching|MemoryCache') {
      $score += 10
    }
  }

  $finalScore = [Math]::Min([Math]::Max($score, 0), $maxScore)
  $ValidationResults.Principles[$principle].Score = $finalScore

  if ($finalScore -ge 80) {
    $ValidationResults.Principles[$principle].Status = "PASS"
    Write-Host "  🎉 Performance Requirements: PASSED (Score: $finalScore/$maxScore)" -ForegroundColor Green
  }
  elseif ($finalScore -ge 60) {
    $ValidationResults.Principles[$principle].Status = "WARNING"
    Write-Host "  ⚠️ Performance Requirements: WARNING (Score: $finalScore/$maxScore)" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.Principles[$principle].Status = "FAIL"
    Write-Host "  ❌ Performance Requirements: FAILED (Score: $finalScore/$maxScore)" -ForegroundColor Red
  }

  $ValidationResults.Summary.TotalChecks++
  if ($ValidationResults.Principles[$principle].Status -eq "PASS") {
    $ValidationResults.Summary.PassedChecks++
  }
}

function Test-ManufacturingDomain {
  Write-Host "🏭 Manufacturing Domain Validation" -ForegroundColor Cyan
  $score = 0
  $maxScore = 100

  $csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse

  # Check for manufacturing operations
  $operations = @("90", "100", "110", "120")
  $operationsFound = @()

  foreach ($csFile in $csFiles) {
    $content = Get-Content $csFile.FullName -Raw
    foreach ($op in $operations) {
      if ($content -match "`"$op`"|'$op'") {
        if ($op -notin $operationsFound) {
          $operationsFound += $op
          $score += 10
          Write-Host "  ✅ Manufacturing operation $op detected: $($csFile.Name)" -ForegroundColor Green
        }
      }
    }

    # Check for location codes
    $locations = @("FLOOR", "RECEIVING", "SHIPPING")
    foreach ($loc in $locations) {
      if ($content -match $loc) {
        $score += 10
        Write-Host "  ✅ Manufacturing location $loc detected: $($csFile.Name)" -ForegroundColor Green
      }
    }

    # Check for transaction types
    $transactionTypes = @("IN", "OUT", "TRANSFER")
    foreach ($type in $transactionTypes) {
      if ($content -match "`"$type`"|'$type'") {
        $score += 5
        Write-Host "  ✅ Transaction type $type detected: $($csFile.Name)" -ForegroundColor Green
      }
    }

    # Check for part ID patterns
    if ($content -match 'PartID|PartId|partId') {
      $score += 5
    }
  }

  if ($operationsFound.Count -eq 0) {
    Add-Violation -Principle "ManufacturingDomain" -Severity "WARNING" -Description "No manufacturing operations (90, 100, 110, 120) detected" -Suggestion "Implement manufacturing operation validation"
  }

  $finalScore = [Math]::Min([Math]::Max($score, 0), $maxScore)
  $ValidationResults.ManufacturingDomain.Score = $finalScore

  if ($finalScore -ge 80) {
    $ValidationResults.ManufacturingDomain.Status = "PASS"
    Write-Host "  🎉 Manufacturing Domain: PASSED (Score: $finalScore/$maxScore)" -ForegroundColor Green
  }
  elseif ($finalScore -ge 60) {
    $ValidationResults.ManufacturingDomain.Status = "WARNING"
    Write-Host "  ⚠️ Manufacturing Domain: WARNING (Score: $finalScore/$maxScore)" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.ManufacturingDomain.Status = "FAIL"
    Write-Host "  ❌ Manufacturing Domain: FAILED (Score: $finalScore/$maxScore)" -ForegroundColor Red
  }

  $ValidationResults.Summary.TotalChecks++
  if ($ValidationResults.ManufacturingDomain.Status -eq "PASS") {
    $ValidationResults.Summary.PassedChecks++
  }
}

function Test-CrossPlatform {
  Write-Host "🌐 Cross-Platform Validation" -ForegroundColor Cyan
  $score = 0
  $maxScore = 100

  $csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse

  # Check for platform-specific violations
  foreach ($csFile in $csFiles) {
    $content = Get-Content $csFile.FullName -Raw
    $lineNumber = 1

    foreach ($line in (Get-Content $csFile.FullName)) {
      # Check for Windows-specific code
      if ($line -match 'System\.Windows|WinForms|Microsoft\.Win32') {
        Add-Violation -Principle "CrossPlatform" -Severity "CRITICAL" -Description "Platform-specific Windows code detected" -File $csFile.FullName -Line $lineNumber -Suggestion "Use cross-platform alternatives"
      }

      # Check for proper path handling
      if ($line -match 'Path\.Combine|Path\.DirectorySeparatorChar') {
        $score += 2
      }

      # Check for file I/O best practices
      if ($line -match 'File\.OpenRead|using.*FileStream') {
        $score += 1
      }

      $lineNumber++
    }
  }

  # Check for Avalonia cross-platform patterns
  $axamlFiles = Get-ChildItem -Path $ProjectPath -Filter "*.axaml" -Recurse
  if ($axamlFiles.Count -gt 0) {
    $score += 30
    Write-Host "  ✅ Cross-platform UI framework (Avalonia) detected" -ForegroundColor Green
  }

  # Check project file for cross-platform targets
  $csprojFiles = Get-ChildItem -Path $ProjectPath -Filter "*.csproj" -Recurse
  foreach ($csproj in $csprojFiles) {
    $content = Get-Content $csproj.FullName -Raw
    if ($content -match 'net8\.0') {
      $score += 30
      Write-Host "  ✅ Cross-platform .NET 8.0 target detected: $($csproj.Name)" -ForegroundColor Green
    }
  }

  $finalScore = [Math]::Min([Math]::Max($score, 0), $maxScore)
  $ValidationResults.CrossPlatform.Score = $finalScore

  if ($finalScore -ge 80) {
    $ValidationResults.CrossPlatform.Status = "PASS"
    Write-Host "  🎉 Cross-Platform: PASSED (Score: $finalScore/$maxScore)" -ForegroundColor Green
  }
  elseif ($finalScore -ge 60) {
    $ValidationResults.CrossPlatform.Status = "WARNING"
    Write-Host "  ⚠️ Cross-Platform: WARNING (Score: $finalScore/$maxScore)" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.CrossPlatform.Status = "FAIL"
    Write-Host "  ❌ Cross-Platform: FAILED (Score: $finalScore/$maxScore)" -ForegroundColor Red
  }

  $ValidationResults.Summary.TotalChecks++
  if ($ValidationResults.CrossPlatform.Status -eq "PASS") {
    $ValidationResults.Summary.PassedChecks++
  }
}

function Write-ConstitutionalSummary {
  Write-Host ""
  Write-Host "====================================================" -ForegroundColor Blue
  Write-Host "CONSTITUTIONAL COMPLIANCE SUMMARY" -ForegroundColor Blue
  Write-Host "====================================================" -ForegroundColor Blue

  # Determine overall status
  $allPassed = $true
  $hasCritical = $false

  foreach ($principle in $ValidationResults.Principles.Keys) {
    if ($ValidationResults.Principles[$principle].Status -eq "FAIL") {
      $allPassed = $false
    }
  }

  if ($ValidationResults.ManufacturingDomain.Status -eq "FAIL" -or $ValidationResults.CrossPlatform.Status -eq "FAIL") {
    $allPassed = $false
  }

  if ($ValidationResults.Summary.CriticalViolations -gt 0) {
    $hasCritical = $true
    $allPassed = $false
  }

  if ($allPassed -and -not $hasCritical) {
    $ValidationResults.OverallStatus = "PASS"
    Write-Host "🎉 CONSTITUTIONAL COMPLIANCE: PASSED" -ForegroundColor Green
  }
  elseif (-not $hasCritical) {
    $ValidationResults.OverallStatus = "WARNING"
    Write-Host "⚠️ CONSTITUTIONAL COMPLIANCE: WARNING" -ForegroundColor Yellow
  }
  else {
    $ValidationResults.OverallStatus = "FAIL"
    Write-Host "❌ CONSTITUTIONAL COMPLIANCE: FAILED" -ForegroundColor Red
  }

  Write-Host ""
  Write-Host "Principle Results:" -ForegroundColor White
  foreach ($principle in $ValidationResults.Principles.Keys) {
    $status = $ValidationResults.Principles[$principle].Status
    $score = $ValidationResults.Principles[$principle].Score
    $color = if ($status -eq "PASS") { "Green" } elseif ($status -eq "WARNING") { "Yellow" } else { "Red" }
    $icon = if ($status -eq "PASS") { "✅" } elseif ($status -eq "WARNING") { "⚠️" } else { "❌" }
    Write-Host "  $icon $principle`: $status ($score/100)" -ForegroundColor $color
  }

  Write-Host "  $(if ($ValidationResults.ManufacturingDomain.Status -eq "PASS") { "✅" } elseif ($ValidationResults.ManufacturingDomain.Status -eq "WARNING") { "⚠️" } else { "❌" }) Manufacturing Domain: $($ValidationResults.ManufacturingDomain.Status) ($($ValidationResults.ManufacturingDomain.Score)/100)" -ForegroundColor $(if ($ValidationResults.ManufacturingDomain.Status -eq "PASS") { "Green" } elseif ($ValidationResults.ManufacturingDomain.Status -eq "WARNING") { "Yellow" } else { "Red" })
  Write-Host "  $(if ($ValidationResults.CrossPlatform.Status -eq "PASS") { "✅" } elseif ($ValidationResults.CrossPlatform.Status -eq "WARNING") { "⚠️" } else { "❌" }) Cross-Platform: $($ValidationResults.CrossPlatform.Status) ($($ValidationResults.CrossPlatform.Score)/100)" -ForegroundColor $(if ($ValidationResults.CrossPlatform.Status -eq "PASS") { "Green" } elseif ($ValidationResults.CrossPlatform.Status -eq "WARNING") { "Yellow" } else { "Red" })

  Write-Host ""
  Write-Host "Summary Statistics:" -ForegroundColor White
  Write-Host "  Total Checks: $($ValidationResults.Summary.TotalChecks)" -ForegroundColor Gray
  Write-Host "  Passed Checks: $($ValidationResults.Summary.PassedChecks)" -ForegroundColor Green
  Write-Host "  Total Violations: $($ValidationResults.Summary.TotalViolations)" -ForegroundColor $(if ($ValidationResults.Summary.TotalViolations -eq 0) { "Green" } else { "Red" })
  Write-Host "  Critical Violations: $($ValidationResults.Summary.CriticalViolations)" -ForegroundColor $(if ($ValidationResults.Summary.CriticalViolations -eq 0) { "Green" } else { "Red" })
  Write-Host "  Warning Violations: $($ValidationResults.Summary.WarningViolations)" -ForegroundColor $(if ($ValidationResults.Summary.WarningViolations -eq 0) { "Green" } else { "Yellow" })

  $passRate = if ($ValidationResults.Summary.TotalChecks -gt 0) {
    [Math]::Round(($ValidationResults.Summary.PassedChecks / $ValidationResults.Summary.TotalChecks) * 100, 1)
  }
  else { 0 }
  Write-Host "  Pass Rate: $passRate%" -ForegroundColor $(if ($passRate -ge 80) { "Green" } elseif ($passRate -ge 60) { "Yellow" } else { "Red" })
}

# Main execution
try {
  Write-ConstitutionalHeader

  if ($ValidationScope -eq "FULL" -or $ValidationScope -eq "INCREMENTAL") {
    Test-CodeQualityExcellence
    if ($FailFast -and $ValidationResults.Principles.CodeQualityExcellence.Status -eq "FAIL") {
      throw "Critical constitutional violation in Code Quality Excellence"
    }

    Test-TestingStandards
    if ($FailFast -and $ValidationResults.Principles.TestingStandards.Status -eq "FAIL") {
      throw "Critical constitutional violation in Testing Standards"
    }

    Test-UXConsistency
    if ($FailFast -and $ValidationResults.Principles.UXConsistency.Status -eq "FAIL") {
      throw "Critical constitutional violation in UX Consistency"
    }

    Test-PerformanceRequirements
    if ($FailFast -and $ValidationResults.Principles.PerformanceRequirements.Status -eq "FAIL") {
      throw "Critical constitutional violation in Performance Requirements"
    }

    Test-ManufacturingDomain
    if ($FailFast -and $ValidationResults.ManufacturingDomain.Status -eq "FAIL") {
      throw "Critical constitutional violation in Manufacturing Domain"
    }

    Test-CrossPlatform
    if ($FailFast -and $ValidationResults.CrossPlatform.Status -eq "FAIL") {
      throw "Critical constitutional violation in Cross-Platform requirements"
    }
  }
  elseif ($ValidationScope -eq "QUICK") {
    # Quick validation - just check core requirements
    Test-CodeQualityExcellence
    Test-ManufacturingDomain
  }

  Write-ConstitutionalSummary

  # Output results
  if ($Json) {
    $jsonOutput = $ValidationResults | ConvertTo-Json -Depth 10
    if ($OutputFile) {
      $jsonOutput | Out-File -FilePath $OutputFile -Encoding UTF8
      Write-Host "Results written to: $OutputFile" -ForegroundColor Gray
    }
    else {
      Write-Host $jsonOutput
    }
  }

  # Set exit code based on overall status
  if ($ValidationResults.OverallStatus -eq "FAIL") {
    exit 1
  }
  elseif ($ValidationResults.OverallStatus -eq "WARNING") {
    exit 2
  }
  else {
    exit 0
  }

}
catch {
  Write-Host "❌ Constitutional validation failed with error: $_" -ForegroundColor Red
  if ($Json) {
    $errorResult = @{
      Error     = $_.Exception.Message
      Status    = "ERROR"
      Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss UTC"
    }
    if ($OutputFile) {
      $errorResult | ConvertTo-Json | Out-File -FilePath $OutputFile -Encoding UTF8
    }
    else {
      $errorResult | ConvertTo-Json
    }
  }
  exit 3
}
