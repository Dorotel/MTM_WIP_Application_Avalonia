---
name: Unit Test Validation Checklist
description: 'Quality assurance checklist for unit test completeness and quality in MTM manufacturing context'
applies_to: '**/*Tests.cs,**/*Test.cs'
manufacturing_context: true
review_type: 'testing'
quality_gate: 'important'
---

# Unit Test Validation - Quality Assurance Checklist

## Context
- **Test Type**: Unit Tests (xUnit + Moq)
- **Manufacturing Domain**: Manufacturing Logic Testing
- **Quality Gate**: Pre-merge (Important)
- **Coverage Requirements**: ViewModels 95%+, Services 90%+, Models 80%+

## Test Coverage Requirements

### Minimum Coverage Targets
- [ ] **ViewModels: 95%+ coverage** (critical for UI reliability)
- [ ] **Services: 90%+ coverage** (critical for business logic)
- [ ] **Models: 80%+ coverage** (data validation and logic)
- [ ] **Utilities: 90%+ coverage** (helper methods and extensions)
- [ ] **Manufacturing Logic: 95%+ coverage** (business-critical operations)

### Coverage Validation
- [ ] **Line coverage** meets minimum requirements
- [ ] **Branch coverage** adequate for conditional logic
- [ ] **Critical paths** fully covered (manufacturing workflows)
- [ ] **Error scenarios** covered with exception testing
- [ ] **Edge cases** covered for manufacturing constraints

## Test Structure and Organization

### Test Class Organization
- [ ] **Test classes** follow naming convention ([ComponentName]Tests)
- [ ] **Test methods** follow naming pattern (Method_Scenario_ExpectedResult)
- [ ] **Test categories** properly applied ([Category("Unit"), Category("Manufacturing")])
- [ ] **Setup/Teardown** methods properly implemented
- [ ] **Test data** organized and reusable

### Test Method Quality
- [ ] **Arrange-Act-Assert** pattern followed consistently
- [ ] **Single responsibility** per test method
- [ ] **Test names** clearly describe scenario and expectation
- [ ] **Test isolation** - no interdependencies between tests
- [ ] **Deterministic tests** - same result on every run

## MVVM Community Toolkit Testing

### ViewModel Testing Patterns
- [ ] **[ObservableProperty] change notifications** tested
- [ ] **[RelayCommand] execution** tested with valid/invalid scenarios
- [ ] **Command CanExecute** tested with various state combinations
- [ ] **Property validation** tested with DataAnnotations
- [ ] **Service interaction** tested with mocked dependencies

### Command Testing
- [ ] **Async commands** properly tested with async/await patterns
- [ ] **Command parameters** validated and tested
- [ ] **Exception handling** in commands tested
- [ ] **Loading states** tested during command execution
- [ ] **CanExecute changes** tested when dependent properties change

## Manufacturing Domain Testing

### Business Logic Validation
- [ ] **Manufacturing workflows** tested with domain-specific scenarios
- [ ] **Transaction type logic** tested (IN/OUT/TRANSFER determination)
- [ ] **Part ID validation** tested with valid/invalid formats
- [ ] **Quantity validation** tested (positive integers, limits)
- [ ] **Operation sequence** tested (90→100→110→120 workflow)

### Manufacturing Constraints
- [ ] **Inventory rules** tested (no negative quantities)
- [ ] **Manufacturing data validation** tested
- [ ] **Cross-service business rules** tested
- [ ] **Manufacturing error scenarios** tested
- [ ] **Data consistency** tested across operations

## Service Layer Testing

### Database Service Testing
- [ ] **Stored procedure calls** mocked appropriately
- [ ] **Database errors** simulated and tested
- [ ] **Connection failures** tested with appropriate exceptions
- [ ] **Transaction rollback** scenarios tested
- [ ] **Manufacturing data operations** tested with domain-specific data

### Dependency Injection Testing
- [ ] **Constructor validation** tested (ArgumentNullException)
- [ ] **Service dependencies** properly mocked
- [ ] **Configuration services** mocked with test data
- [ ] **Service interaction** tested with realistic scenarios
- [ ] **Error propagation** tested between service layers

## Mock and Test Data Management

### Mocking Best Practices
- [ ] **Mock objects** created for all external dependencies
- [ ] **Mock setup** realistic for manufacturing scenarios
- [ ] **Mock verification** validates expected interactions
- [ ] **Mock state** properly reset between tests
- [ ] **Mock complexity** kept reasonable and maintainable

### Test Data Quality
- [ ] **Manufacturing test data** realistic and comprehensive
- [ ] **Test data builders** used for complex object creation
- [ ] **Invalid data scenarios** included in test data sets
- [ ] **Edge case data** included (boundary values, limits)
- [ ] **Test data cleanup** implemented where necessary

## Performance and Reliability Testing

### Test Performance
- [ ] **Test execution time** reasonable (< 1 second per test typically)
- [ ] **Heavy operations** mocked to avoid slow tests
- [ ] **Database operations** mocked in unit tests
- [ ] **File I/O operations** mocked or abstracted
- [ ] **Network operations** mocked in unit tests

### Test Reliability
- [ ] **Flaky tests** identified and fixed
- [ ] **Race conditions** eliminated in test code
- [ ] **Time-dependent tests** use controlled time sources
- [ ] **Random data** generated with fixed seeds for reproducibility
- [ ] **Environment dependencies** minimized or mocked

## Error Handling Testing

### Exception Testing
- [ ] **Expected exceptions** tested with Assert.Throws patterns
- [ ] **Error messages** validated for manufacturing context
- [ ] **Exception types** appropriate for error scenarios
- [ ] **Error logging** validated with mock loggers
- [ ] **Error recovery** tested where applicable

### Manufacturing Error Scenarios
- [ ] **Invalid manufacturing data** error handling tested
- [ ] **Business rule violations** properly tested
- [ ] **Service failures** error handling tested
- [ ] **Database constraint violations** tested
- [ ] **Manufacturing workflow errors** tested

## Sign-off

- [ ] **Developer Self-Review**: _________________ - _________
- [ ] **Test Coverage Validation**: _________________ - _________
- [ ] **Manufacturing Domain Testing**: _________________ - _________
- [ ] **Quality Gate Approval**: _________________ - _________

## Test Metrics

### Coverage Report
- **Overall Coverage**: ____%
- **ViewModel Coverage**: ____%  
- **Service Coverage**: ____%
- **Manufacturing Logic Coverage**: ____%

### Test Quality Metrics
- **Total Tests**: ______
- **Passing Tests**: ______
- **Test Execution Time**: ______ seconds
- **Flaky Tests**: ______

---

**Test Validation Status**: [ ] Approved [ ] Needs Improvement [ ] Insufficient Coverage  
**Manufacturing Domain Coverage**: [ ] Complete [ ] Partial [ ] Missing