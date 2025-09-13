# Test Project Configuration
## MTM WIP Application Testing Framework

This project file should be created by the GitHub Copilot agent to establish the comprehensive testing infrastructure for the MTM manufacturing application.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <EnableDefaultTestHost>true</EnableDefaultTestHost>
    
    <!-- Platform support -->
    <RuntimeIdentifiers>win-x64;win-x86;osx-x64;osx-arm64;linux-x64;linux-arm64</RuntimeIdentifiers>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core Testing Framework -->
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="NUnit" Version="4.1.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />

    <!-- Mocking and Test Utilities -->
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="AutoFixture" Version="4.18.1" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />

    <!-- Avalonia Testing Support -->
    <PackageReference Include="Avalonia.Headless" Version="11.3.4" />
    <PackageReference Include="Avalonia.Headless.NUnit" Version="11.3.4" />

    <!-- Database Testing -->
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.8" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.8" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.8" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.8" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.8" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.8" />

    <!-- Performance Testing -->
    <PackageReference Include="NBomber" Version="5.9.2" />
    <PackageReference Include="BenchmarkDotNet" Version="0.13.12" />

    <!-- MySQL Testing Support -->
    <PackageReference Include="MySql.Data" Version="9.4.0" />

    <!-- MVVM Community Toolkit Support -->
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
  </ItemGroup>

  <ItemGroup>
    <!-- Reference to main application -->
    <ProjectReference Include="..\MTM_WIP_Application_Avalonia.csproj" />
  </ItemGroup>

  <ItemGroup>
    <!-- Test Data Files -->
    <Content Include="TestData\*.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="TestData\*.sql">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

  <ItemGroup>
    <!-- Test Configuration Files -->
    <Content Include="test-appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    <Content Include="test-appsettings.Development.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

  <!-- Platform-specific test execution settings -->
  <PropertyGroup Condition="$([MSBuild]::IsOSPlatform('Windows'))">
    <DefineConstants>$(DefineConstants);WINDOWS_TESTS</DefineConstants>
  </PropertyGroup>
  
  <PropertyGroup Condition="$([MSBuild]::IsOSPlatform('OSX'))">
    <DefineConstants>$(DefineConstants);MACOS_TESTS</DefineConstants>
  </PropertyGroup>
  
  <PropertyGroup Condition="$([MSBuild]::IsOSPlatform('Linux'))">
    <DefineConstants>$(DefineConstants);LINUX_TESTS</DefineConstants>
  </PropertyGroup>

</Project>
```

## Test Configuration Files

### test-appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mtm_test;Uid=test_user;Pwd=test_password;Allow Zero Datetime=true;Convert Zero Datetime=true;",
    "WindowsConnection": "Server=localhost;Database=mtm_test_win;Uid=mtm_win_user;Pwd=test_password;Allow Zero Datetime=true;Convert Zero Datetime=true;",
    "MacOSConnection": "Server=localhost;Database=mtm_test_mac;Uid=mtm_mac_user;Pwd=test_password;Allow Zero Datetime=true;Convert Zero Datetime=true;",
    "LinuxConnection": "Server=localhost;Database=mtm_test_linux;Uid=mtm_linux_user;Pwd=test_password;Allow Zero Datetime=true;Convert Zero Datetime=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "MTMSettings": {
    "TestMode": true,
    "DatabaseTimeout": 30,
    "EnableTestDataGeneration": true,
    "TestDataDirectory": "TestData",
    "MaxConcurrentTests": 10
  },
  "TestSettings": {
    "UITestTimeout": 5000,
    "DatabaseTestTimeout": 10000,
    "PerformanceTestDuration": 60000,
    "EnableHeadlessMode": true,
    "GenerateTestReports": true,
    "TestReportDirectory": "TestResults"
  }
}
```

### test-appsettings.Development.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mtm_dev_test;Uid=dev_test_user;Pwd=dev_password;Allow Zero Datetime=true;Convert Zero Datetime=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Trace",
      "MTM_WIP_Application_Avalonia": "Trace"
    }
  },
  "MTMSettings": {
    "TestMode": true,
    "EnableVerboseLogging": true,
    "EnableTestDataGeneration": true
  },
  "TestSettings": {
    "EnableDebugMode": true,
    "UITestTimeout": 30000,
    "GenerateDetailedReports": true
  }
}
```

## NUnit Configuration

### NUnit.runsettings
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <TestRunParameters>
    <Parameter name="TestEnvironment" value="Development" />
    <Parameter name="DatabaseProvider" value="MySQL" />
    <Parameter name="EnableParallelExecution" value="true" />
    <Parameter name="MaxParallelThreads" value="4" />
  </TestRunParameters>
  
  <NUnit>
    <NumberOfTestWorkers>4</NumberOfTestWorkers>
    <DefaultTimeout>60000</DefaultTimeout>
    <WorkDirectory>TestResults</WorkDirectory>
    <InternalTraceLevel>Info</InternalTraceLevel>
  </NUnit>
  
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="Code Coverage" uri="datacollector://Microsoft/CodeCoverage/2.0" assemblyQualifiedName="Microsoft.VisualStudio.Coverage.DynamicCoverageDataCollector, Microsoft.VisualStudio.TraceCollector, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a">
        <Configuration>
          <CodeCoverage>
            <ModulePaths>
              <Include>
                <ModulePath>MTM_WIP_Application_Avalonia.dll</ModulePath>
              </Include>
            </ModulePaths>
          </CodeCoverage>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

## GitHub Actions CI/CD Configuration

### .github/workflows/comprehensive-tests.yml
```yaml
name: Comprehensive MTM Test Suite

on:
  push:
    branches: [ master, develop ]
  pull_request:
    branches: [ master, develop ]

env:
  DOTNET_VERSION: '8.0.x'
  MYSQL_VERSION: '8.0'

jobs:
  unit-tests:
    name: Unit Tests
    strategy:
      matrix:
        os: [windows-latest, ubuntu-latest, macos-latest]
        include:
          - os: windows-latest
            runtime: win-x64
          - os: ubuntu-latest
            runtime: linux-x64
          - os: macos-latest
            runtime: osx-x64
    
    runs-on: ${{ matrix.os }}
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Setup MySQL
      uses: mirromutth/mysql-action@v1.1
      with:
        mysql version: ${{ env.MYSQL_VERSION }}
        mysql database: mtm_test
        mysql user: test_user
        mysql password: test_password
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build application
      run: dotnet build --no-restore --configuration Release
    
    - name: Run unit tests
      run: |
        dotnet test Tests/MTM.Tests.csproj \
          --no-build \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --collect:"XPlat Code Coverage" \
          --filter "Category=Unit"
    
    - name: Upload test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: unit-test-results-${{ matrix.os }}
        path: TestResults
  
  integration-tests:
    name: Integration Tests
    runs-on: ubuntu-latest
    needs: unit-tests
    
    services:
      mysql:
        image: mysql:8.0
        env:
          MYSQL_ROOT_PASSWORD: root_password
          MYSQL_DATABASE: mtm_test
          MYSQL_USER: test_user
          MYSQL_PASSWORD: test_password
        ports:
          - 3306:3306
        options: --health-cmd="mysqladmin ping" --health-interval=10s --health-timeout=5s --health-retries=3
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Setup test database
      run: |
        mysql -h127.0.0.1 -utest_user -ptest_password mtm_test < Tests/TestData/DatabaseTestSchema.sql
    
    - name: Run integration tests
      run: |
        dotnet test Tests/MTM.Tests.csproj \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --filter "Category=Integration"
    
    - name: Upload integration test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: integration-test-results
        path: TestResults
  
  ui-automation-tests:
    name: UI Automation Tests
    strategy:
      matrix:
        os: [windows-latest, ubuntu-latest, macos-latest]
    
    runs-on: ${{ matrix.os }}
    needs: unit-tests
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Setup display (Linux)
      if: runner.os == 'Linux'
      run: |
        sudo apt-get update
        sudo apt-get install -y xvfb
        export DISPLAY=:99
        Xvfb :99 -screen 0 1024x768x24 > /dev/null 2>&1 &
    
    - name: Run UI automation tests
      run: |
        dotnet test Tests/MTM.Tests.csproj \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --filter "Category=UIAutomation"
      env:
        DISPLAY: :99
    
    - name: Upload UI test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: ui-test-results-${{ matrix.os }}
        path: TestResults
  
  performance-tests:
    name: Performance Tests
    runs-on: ubuntu-latest
    needs: [unit-tests, integration-tests]
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Run performance tests
      run: |
        dotnet test Tests/MTM.Tests.csproj \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --filter "Category=Performance"
    
    - name: Upload performance test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: performance-test-results
        path: TestResults
  
  e2e-tests:
    name: End-to-End Tests
    runs-on: ubuntu-latest
    needs: [unit-tests, integration-tests, ui-automation-tests]
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Run E2E tests
      run: |
        dotnet test Tests/MTM.Tests.csproj \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --filter "Category=E2E"
    
    - name: Upload E2E test results
      uses: actions/upload-artifact@v4
      if: always()
      with:
        name: e2e-test-results
        path: TestResults

  test-report:
    name: Generate Test Report
    runs-on: ubuntu-latest
    needs: [unit-tests, integration-tests, ui-automation-tests, performance-tests, e2e-tests]
    if: always()
    
    steps:
    - name: Download all test results
      uses: actions/download-artifact@v4
      with:
        path: AllTestResults
    
    - name: Generate combined test report
      run: |
        echo "# MTM Application Test Results" > test-summary.md
        echo "" >> test-summary.md
        echo "## Test Execution Summary" >> test-summary.md
        echo "- Unit Tests: ${{ needs.unit-tests.result }}" >> test-summary.md
        echo "- Integration Tests: ${{ needs.integration-tests.result }}" >> test-summary.md
        echo "- UI Automation Tests: ${{ needs.ui-automation-tests.result }}" >> test-summary.md
        echo "- Performance Tests: ${{ needs.performance-tests.result }}" >> test-summary.md
        echo "- E2E Tests: ${{ needs.e2e-tests.result }}" >> test-summary.md
    
    - name: Upload test summary
      uses: actions/upload-artifact@v4
      with:
        name: test-summary
        path: test-summary.md
```

This comprehensive test project configuration provides everything the GitHub Copilot agent needs to implement the full testing infrastructure for your MTM application.