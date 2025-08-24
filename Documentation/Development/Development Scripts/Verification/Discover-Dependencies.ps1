#!/usr/bin/env pwsh
# Discover-Dependencies.ps1
# Find new service dependencies and analyze dependency injection patterns

param(
    [string]$ProjectRoot = ".",
    [switch]$Verbose = $false
)

Write-Host "=== MTM WIP Application - Dependency Discovery ===" -ForegroundColor Cyan
Write-Host "Analyzing service dependencies and DI patterns..." -ForegroundColor Yellow

# Define files to analyze for dependencies
$SourceFiles = @(
    "Services/*.cs",
    "ViewModels/*.cs", 
    "Extensions/*.cs",
    "*.cs"
)

$DependencyAnalysis = @()
$ServiceInterfaces = @()
$ServiceImplementations = @()
$DIRegistrations = @()

foreach ($Pattern in $SourceFiles) {
    $Files = Get-ChildItem -Path (Join-Path $ProjectRoot $Pattern) -Recurse -ErrorAction SilentlyContinue | Where-Object {
        $_.Name -notlike "*Test*" -and $_.Name -notlike "*Temp*"
    }
    
    foreach ($File in $Files) {
        if ($Verbose) {
            Write-Host "  🔍 Analyzing: $($File.Name)" -ForegroundColor Gray
        }
        
        $Content = Get-Content $File.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $Content) { continue }
        
        $RelativePath = $File.FullName.Replace((Resolve-Path $ProjectRoot).Path + [System.IO.Path]::DirectorySeparatorChar, "")
        
        # Extract using statements
        $UsingStatements = [regex]::Matches($Content, 'using\s+([^;]+);') | ForEach-Object {
            $_.Groups[1].Value.Trim()
        }
        
        # Extract interface definitions
        $InterfaceMatches = [regex]::Matches($Content, 'public\s+interface\s+(\w+)')
        foreach ($Match in $InterfaceMatches) {
            $ServiceInterfaces += [PSCustomObject]@{
                Interface = $Match.Groups[1].Value
                File = $File.Name
                Path = $RelativePath
            }
        }
        
        # Extract service implementations
        $ServiceMatches = [regex]::Matches($Content, 'public\s+class\s+(\w+)\s*:\s*[^{]*I(\w+Service)')
        foreach ($Match in $ServiceMatches) {
            $ServiceImplementations += [PSCustomObject]@{
                Implementation = $Match.Groups[1].Value
                Interface = "I" + $Match.Groups[2].Value + "Service"
                File = $File.Name
                Path = $RelativePath
            }
        }
        
        # Extract constructor dependencies
        $ConstructorPattern = '(?s)public\s+' + [regex]::Escape($File.BaseName) + '\s*\([^)]*\)'
        $ConstructorMatches = [regex]::Matches($Content, $ConstructorPattern)
        
        $Dependencies = @()
        foreach ($ConstructorMatch in $ConstructorMatches) {
            $ConstructorContent = $ConstructorMatch.Value
            $ParamMatches = [regex]::Matches($ConstructorContent, '(\w+(?:\.\w+)*(?:\.I\w+)?)\s+(\w+)')
            
            foreach ($ParamMatch in $ParamMatches) {
                $Type = $ParamMatch.Groups[1].Value
                $Name = $ParamMatch.Groups[2].Value
                
                # Filter for likely service dependencies
                if ($Type -match 'Service|I\w+Service|ILogger|IConfiguration|IMemoryCache|IDbConnection') {
                    $Dependencies += [PSCustomObject]@{
                        Type = $Type
                        ParameterName = $Name
                        IsInterface = $Type.StartsWith('I') -and $Type -match '^I[A-Z]'
                        IsService = $Type -match 'Service'
                        IsInfrastructure = $Type -match 'ILogger|IConfiguration|IMemoryCache|IDbConnection'
                    }
                }
            }
        }
        
        # Extract DI registration patterns
        $DIPatterns = @(
            'services\.Add\w+<([^>]+)>',
            'services\.Add\w+<([^,>]+),\s*([^>]+)>',
            'AddMTMServices',
            'services\.Configure<([^>]+)>'
        )
        
        $RegistrationMatches = @()
        foreach ($Pattern in $DIPatterns) {
            $Matches = [regex]::Matches($Content, $Pattern)
            foreach ($Match in $Matches) {
                $RegistrationMatches += $Match.Value
            }
        }
        
        if ($Dependencies.Count -gt 0 -or $RegistrationMatches.Count -gt 0) {
            $DependencyAnalysis += [PSCustomObject]@{
                File = $File.Name
                Path = $RelativePath
                Dependencies = $Dependencies
                DIRegistrations = $RegistrationMatches
                UsingStatements = $UsingStatements | Where-Object { $_ -match 'MTM|Service|Microsoft\.Extensions' }
                HasAddMTMServices = $Content -match 'AddMTMServices'
                HasServiceRegistration = $RegistrationMatches.Count -gt 0
            }
        }
    }
}

# Analyze findings
Write-Host "`n=== Dependency Analysis Results ===" -ForegroundColor Green

Write-Host "`n📋 Service Interfaces Found:" -ForegroundColor Cyan
$ServiceInterfaces | Sort-Object Interface | ForEach-Object {
    Write-Host "  🔌 $($_.Interface) in $($_.File)" -ForegroundColor White
}

Write-Host "`n🏗️  Service Implementations Found:" -ForegroundColor Cyan
$ServiceImplementations | Sort-Object Implementation | ForEach-Object {
    Write-Host "  ⚙️  $($_.Implementation) -> $($_.Interface) in $($_.File)" -ForegroundColor White
}

Write-Host "`n🔗 Dependency Injection Patterns:" -ForegroundColor Cyan
$FilesWithDI = $DependencyAnalysis | Where-Object { $_.HasServiceRegistration -or $_.HasAddMTMServices }
foreach ($File in $FilesWithDI) {
    Write-Host "  📄 $($File.File)" -ForegroundColor White
    
    if ($File.HasAddMTMServices) {
        Write-Host "    ✅ Uses AddMTMServices() extension method" -ForegroundColor Green
    }
    
    foreach ($Registration in $File.DIRegistrations) {
        if ($Registration -ne 'AddMTMServices') {
            Write-Host "    🔧 $Registration" -ForegroundColor Yellow
        }
    }
}

Write-Host "`n🔍 Service Dependencies Analysis:" -ForegroundColor Cyan
$FilesWithDependencies = $DependencyAnalysis | Where-Object { $_.Dependencies.Count -gt 0 }
foreach ($File in $FilesWithDependencies) {
    Write-Host "  📄 $($File.File)" -ForegroundColor White
    
    $ServiceDeps = $File.Dependencies | Where-Object { $_.IsService }
    $InfraDeps = $File.Dependencies | Where-Object { $_.IsInfrastructure }
    
    if ($ServiceDeps.Count -gt 0) {
        Write-Host "    🔧 Business Services: $($ServiceDeps.Type -join ', ')" -ForegroundColor Cyan
    }
    
    if ($InfraDeps.Count -gt 0) {
        Write-Host "    🏗️  Infrastructure: $($InfraDeps.Type -join ', ')" -ForegroundColor Yellow
    }
}

# Check for missing dependencies
Write-Host "`n❗ Potential Missing Dependencies:" -ForegroundColor Red

$AllDependencyTypes = $DependencyAnalysis | ForEach-Object { $_.Dependencies } | Where-Object { $_.IsService } | ForEach-Object { $_.Type } | Sort-Object | Get-Unique
$AllInterfaceTypes = $ServiceInterfaces | ForEach-Object { $_.Interface } | Sort-Object | Get-Unique

foreach ($DepType in $AllDependencyTypes) {
    if ($DepType -notin $AllInterfaceTypes -and $DepType -notmatch '^MTM\.' -and $DepType -notmatch '^Microsoft\.') {
        Write-Host "  ⚠️  $DepType - Interface not found in codebase" -ForegroundColor Yellow
    }
}

# Generate recommendations
Write-Host "`n=== Recommendations ===" -ForegroundColor Yellow

$FilesWithoutAddMTMServices = $DependencyAnalysis | Where-Object { 
    $_.HasServiceRegistration -and -not $_.HasAddMTMServices -and $_.File -match 'Program\.cs|Startup\.cs|Configuration'
}

if ($FilesWithoutAddMTMServices.Count -gt 0) {
    Write-Host "  📋 Consider using AddMTMServices() in these files:" -ForegroundColor Cyan
    $FilesWithoutAddMTMServices | ForEach-Object {
        Write-Host "    📄 $($_.File)" -ForegroundColor White
    }
}

$ComplexDependencies = $DependencyAnalysis | Where-Object { $_.Dependencies.Count -gt 5 }
if ($ComplexDependencies.Count -gt 0) {
    Write-Host "  🔍 Files with complex dependencies (>5 dependencies):" -ForegroundColor Cyan
    $ComplexDependencies | ForEach-Object {
        Write-Host "    📄 $($_.File) - $($_.Dependencies.Count) dependencies" -ForegroundColor White
    }
}

# Export detailed report
$ReportPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/dependency-analysis-report.json"
$Report = @{
    ServiceInterfaces = $ServiceInterfaces
    ServiceImplementations = $ServiceImplementations  
    DependencyAnalysis = $DependencyAnalysis
    Summary = @{
        TotalInterfaces = $ServiceInterfaces.Count
        TotalImplementations = $ServiceImplementations.Count
        FilesWithDependencies = $FilesWithDependencies.Count
        FilesWithDIRegistration = $FilesWithDI.Count
        FilesUsingAddMTMServices = ($DependencyAnalysis | Where-Object { $_.HasAddMTMServices }).Count
    }
}

$Report | ConvertTo-Json -Depth 4 | Out-File -FilePath $ReportPath -Encoding UTF8
Write-Host "`n📋 Detailed dependency report saved to: $ReportPath" -ForegroundColor Cyan

Write-Host "`n✅ Dependency Discovery Complete!" -ForegroundColor Green