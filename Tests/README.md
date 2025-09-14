# MTM WIP Application - Testing Framework

## Overview

This directory contains the comprehensive testing framework for the MTM WIP Application. The testing infrastructure follows a 5-tier strategy as outlined in the testing standards documentation.

## Testing Framework Architecture

### 5-Tier Testing Strategy

1. **Tier 1: Unit Tests** - Individual component validation (ViewModels, Services, Models)
2. **Tier 2: Integration Tests** - Multi-service workflows and database validation
3. **Tier 3: UI Automation Tests** - Avalonia.Headless UI testing framework
4. **Tier 4: Database Tests** - MySQL stored procedure testing (45+ procedures)
5. **Tier 5: Performance Tests** - NBomber load testing and performance validation

### Current Status

✅ **Testing Infrastructure Setup Complete**
- Modern testing stack: NUnit 4.1.0, Moq 4.20.70, FluentAssertions 6.12.0
- Avalonia.Headless 11.3.4 for UI automation testing
- NBomber 6.0.0 for performance testing
- Cross-platform support (Windows, macOS, Linux, Android)
- Test project successfully integrated into solution

✅ **Framework Validation**
- All 10 framework tests passing
- NUnit parameterized testing working
- FluentAssertions integration working
- Category-based test filtering implemented

## Directory Structure

```
Tests/
├── MTM.Tests.csproj                     # Test project configuration
├── README.md                            # This file
├── NUnit.runsettings                    # NUnit test configuration
├── test-appsettings.json                # Test configuration
├── test-appsettings.Development.json    # Development test configuration
├── TestData/                            # Test data files
├── UnitTests/                           # Tier 1: Unit Tests
│   ├── InfrastructureTests.cs          # Basic infrastructure tests
│   ├── Framework/                       # Framework validation tests
│   ├── ViewModels/                      # ViewModel unit tests (planned)
│   └── Services/                        # Service unit tests (planned)
├── IntegrationTests/                    # Tier 2: Integration Tests (planned)
├── UITests/                             # Tier 3: UI Automation Tests (planned)
├── DatabaseTests/                       # Tier 4: Database Tests (planned)
├── PerformanceTests/                    # Tier 5: Performance Tests (planned)
└── CrossPlatformTests/                  # Cross-platform compatibility tests
    ├── CrossPlatformSupportTests.cs    # Basic cross-platform support validation
    ├── PlatformSpecificTests.cs        # Platform-specific feature tests
    └── FileSelectionServiceTests.cs    # File service cross-platform tests
```

## Testing Technology Stack

### Core Testing Framework
- **NUnit 4.1.0** - Primary test framework with modern features
- **Moq 4.20.70** - Mocking framework for dependency injection testing
- **FluentAssertions 6.12.0** - Expressive assertion library
- **AutoFixture 4.18.1** - Test data generation

### UI Testing
- **Avalonia.Headless 11.3.4** - Headless UI testing for Avalonia applications  
- **Avalonia.Headless.NUnit 11.3.4** - NUnit integration for Avalonia UI tests

### Performance Testing
- **NBomber 6.0.0** - Load testing and performance validation
- **BenchmarkDotNet 0.13.12** - Micro-benchmarking for performance analysis

### Database Testing
- **MySql.Data 9.4.0** - MySQL database connectivity for integration tests
- Support for all 45+ MTM stored procedures

## Running Tests

### All Tests
```bash
dotnet test
```

### By Category
```bash
# Run only unit tests
dotnet test --filter "Category=Unit"

# Run only framework tests  
dotnet test --filter "Category=Framework"

# Run only integration tests
dotnet test --filter "Category=Integration"
```

### By Tier
```bash
# Run specific test tiers
dotnet test --filter "Category=Unit|Category=Service"
dotnet test --filter "Category=UI"
dotnet test --filter "Category=Database"
```

### With Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Implementation Guidelines

### Unit Tests (Tier 1)
- Test individual ViewModels using MVVM Community Toolkit patterns
- Mock all dependencies using Moq
- Focus on [ObservableProperty] and [RelayCommand] testing
- Achieve 95%+ code coverage on business logic

### Integration Tests (Tier 2)  
- Test multi-service workflows with real database connections
- Validate service orchestration and communication
- Test error handling and recovery scenarios
- Follow MTM "NO FALLBACK DATA PATTERN"

### UI Tests (Tier 3)
- Use Avalonia.Headless for headless UI testing
- Test data binding, user interactions, and navigation
- Validate theme consistency and responsive behavior
- Test accessibility and keyboard navigation

### Database Tests (Tier 4)
- Test all 45+ MySQL stored procedures
- Validate data integrity and transaction consistency
- Test concurrent access and performance
- Follow MTM stored procedure patterns only

### Performance Tests (Tier 5)
- Use NBomber for load testing manufacturing workflows
- Test memory leak detection and resource monitoring
- Validate response time requirements (<200ms for operations)
- Test high-volume scenarios (50+ operations/second)

## Cross-Platform Testing

The testing framework supports cross-platform validation:
- **Windows** - Primary development and manufacturing platform
- **macOS** - Office management and reporting systems  
- **Linux** - Manufacturing automation and server systems
- **Android** - Mobile manufacturing operations

Platform-specific tests are marked with appropriate attributes:
```csharp
[Test]
[Platform("Win")]
public void Windows_SpecificTest() { }

[Test]  
[Platform("Linux")]
public void Linux_SpecificTest() { }
```

## Manufacturing Domain Context

All tests maintain MTM manufacturing domain context:
- Part IDs are strings ("PART001", "ABC-123")
- Operations are string numbers ("90", "100", "110", "120") representing workflow steps
- Transaction types follow user intent ("IN", "OUT", "TRANSFER")
- Quantities are integers only (no decimals)
- All database access via stored procedures only

## CI/CD Integration

The testing framework is designed for GitHub Actions CI/CD integration:
- Automated test execution on PR creation
- Cross-platform test matrix
- Performance regression detection
- Test result reporting and coverage metrics

## Next Steps

### Phase 2A: Foundation Test Infrastructure (In Progress)
- [ ] Create comprehensive ViewModel unit tests
- [ ] Implement Service layer unit tests  
- [ ] Build UI automation test suite
- [ ] Establish database integration tests

### Phase 2B: Advanced Testing (Planned)
- [ ] Implement performance testing with NBomber
- [ ] Create cross-platform compatibility test suite
- [ ] Build database performance validation
- [ ] Implement load testing scenarios

### Phase 2C: CI/CD Integration (Planned)
- [ ] GitHub Actions workflow integration
- [ ] Automated test execution pipeline
- [ ] Test data management and fixtures
- [ ] Performance monitoring and alerting

## Contributing

When adding new tests:
1. Follow the established directory structure
2. Use appropriate test categories for filtering
3. Maintain manufacturing domain context
4. Follow MTM architecture patterns (MVVM Community Toolkit, stored procedures only)
5. Ensure cross-platform compatibility where applicable
6. Add tests to maintain 95%+ code coverage target

## Related Documentation

- [Testing Standards Instructions](../.github/instructions/testing-standards.instructions.md)
- [Unit Testing Patterns](../.github/instructions/unit-testing-patterns.instructions.md)
- [Integration Testing Patterns](../.github/instructions/integration-testing-patterns.instructions.md)
- [UI Automation Standards](../.github/instructions/ui-automation-standards.instructions.md)
- [Database Testing Patterns](../.github/instructions/database-testing-patterns.instructions.md)
- [Cross-Platform Testing Standards](../.github/instructions/cross-platform-testing-standards.instructions.md)