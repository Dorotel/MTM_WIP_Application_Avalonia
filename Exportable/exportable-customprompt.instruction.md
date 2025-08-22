# MTM Core Systems - Custom Implementation Prompts

This file contains custom prompts for implementing all MTM core systems into any C# application. These prompts are designed to be used with GitHub Copilot or similar AI coding assistants.

## ?? Table of Contents

- [Foundation Systems (Prompts 21-24)](#foundation-systems-prompts-21-24)
- [Service Layer (Prompts 25-28)](#service-layer-prompts-25-28)
- [Infrastructure (Prompts 29-32)](#infrastructure-prompts-29-32)
- [Quality Assurance (Prompts 33-36)](#quality-assurance-prompts-33-36)
- [Implementation Workflow](#implementation-workflow)
- [Integration Examples](#integration-examples)

---

## ??? Foundation Systems (Prompts 21-24)

### 21. Implement Result Pattern System

**Persona:** Data Modeling Copilot + Application Logic Copilot

**Prompt:**
```
Create the Result<T> pattern infrastructure for .NET 8 applications following modern C# patterns.
Implement Models/Result.cs with Success/Failure states, error messages, and implicit operators.
Include static factory methods Success<T>(T value) and Failure(string error), IsSuccess and IsFailure properties,
proper equality comparison, and integration patterns for async service methods. Ensure compatibility with
any .NET application framework and comprehensive XML documentation.
```

**Expected Output:**
- `Models/Result.cs` with complete implementation
- Static factory methods for easy instantiation
- Implicit operators for seamless integration
- Proper error handling patterns
- XML documentation for all public members

**Integration Example:**
```csharp
public async Task<Result<User>> GetUserAsync(string userId)
{
    try
    {
        var user = await _repository.GetByIdAsync(userId);
        return user != null 
            ? Result<User>.Success(user)
            : Result<User>.Failure("User not found");
    }
    catch (Exception ex)
    {
        return Result<User>.Failure($"Database error: {ex.Message}");
    }
}
```

---

### 22. Create Data Models Foundation

**Persona:** Data Modeling Copilot

**Prompt:**
```
Create the complete Models namespace foundation for .NET 8 application with essential data entities.
Generate Models/BaseEntity.cs with common properties (Id, CreatedAt, UpdatedAt), Models/User.cs with 
authentication properties, Models/AuditableEntity.cs for tracking changes, and Models/ValueObject.cs 
base class. Include proper validation attributes, IEquatable implementations where appropriate, and 
comprehensive XML documentation. Ensure compatibility with Entity Framework Core and other ORMs.
```

**Expected Output:**
- Base entity classes with common patterns
- User management entities
- Auditable entity pattern
- Value object implementations
- Validation attributes
- ORM compatibility

**Integration Example:**
```csharp
public class Product : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
```

---

### 23. Setup Dependency Injection Container

**Persona:** Application Logic Copilot + Configuration Wizard Copilot

**Prompt:**
```
Configure Microsoft.Extensions.DependencyInjection for .NET 8 application with proper service registration.
Create Extensions/ServiceCollectionExtensions.cs with AddCoreServices() method for registering all services.
Setup proper lifetime management (Singleton, Scoped, Transient), create service registration patterns for
repositories, services, and infrastructure components. Include development vs production service variants
and integration with logging and configuration services. Ensure compatibility with any .NET application type.
```

**Expected Output:**
- Service collection extension methods
- Proper lifetime management
- Development/Production configurations
- Auto-registration patterns
- Integration with Microsoft.Extensions.* ecosystem

**Integration Example:**
```csharp
// In Program.cs
services.AddCoreServices(configuration);

// Extension method
public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
{
    // Register all core services
    services.AddScoped<IUserService, UserService>();
    services.AddSingleton<IConfigurationService, ConfigurationService>();
    return services;
}
```

---

### 24. Create Core Service Interfaces

**Persona:** Application Logic Copilot + Data Access Copilot

**Prompt:**
```
Generate essential service interfaces for .NET 8 application following clean architecture patterns.
Create Services/IUserService.cs, Services/IConfigurationService.cs, Services/ILoggingService.cs,
and Services/ICacheService.cs. Include async methods returning Result<T> pattern, proper cancellation 
token support, and comprehensive XML documentation. Design interfaces to be framework-agnostic and
suitable for dependency injection. Include common CRUD operations and business logic abstractions.
```

**Expected Output:**
- Clean service interfaces
- Result<T> return patterns
- Async/await support
- CancellationToken support
- Framework-agnostic design

**Integration Example:**
```csharp
public interface IUserService
{
    Task<Result<User>> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<User>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}
```

---

## ?? Service Layer (Prompts 25-28)

### 25. Implement Service Layer

**Persona:** Application Logic Copilot

**Prompt:**
```
Implement complete service layer for .NET 8 application following clean architecture principles.
Create Services/UserService.cs implementing IUserService interface with full error handling,
async/await patterns, and Result<T> return types. Include comprehensive logging, proper validation,
dependency injection constructor, and integration with repository pattern. Add XML documentation
and ensure proper separation of concerns between business logic and data access.
```

**Expected Output:**
- Complete service implementations
- Error handling patterns
- Logging integration
- Repository pattern usage
- Business logic encapsulation

---

### 26. Create Database Service Layer

**Persona:** Data Access Copilot

**Prompt:**
```
Create centralized database service layer with proper connection management for .NET 8 application.
Implement Services/IDatabaseService.cs and Services/DatabaseService.cs with connection pooling,
transaction management, and async operations. Include connection string management from configuration,
retry policies for connection failures, proper disposal patterns, and support for multiple database
providers. Add stored procedure execution methods and parameter handling.
```

**Expected Output:**
- Database service abstraction
- Connection management
- Transaction support
- Multi-provider support
- Retry policies

---

### 27. Setup Application State Management

**Persona:** Application Logic Copilot

**Prompt:**
```
Create global application state service for .NET 8 application with proper encapsulation.
Implement Services/IApplicationStateService.cs and Services/ApplicationStateService.cs with
thread-safe state management, configuration tracking, user session management, and shared state
between components. Include state persistence, recovery mechanisms, and proper disposal patterns.
Ensure compatibility with web, desktop, and console applications.
```

**Expected Output:**
- Global state management
- Thread safety
- Session management
- State persistence
- Cross-platform compatibility

---

### 28. Implement Configuration Service

**Persona:** Configuration Wizard Copilot

**Prompt:**
```
Create configuration service to read and manage appsettings.json for .NET 8 application.
Implement Services/IConfigurationService.cs and Services/ConfigurationService.cs using
Microsoft.Extensions.Configuration. Include strongly-typed configuration classes, configuration
validation, environment-specific overrides, and real-time configuration reload support.
Integrate with dependency injection and provide convenient access methods for all components.
```

**Expected Output:**
- Configuration management
- Strongly-typed settings
- Environment-specific configs
- Validation support
- Hot reload capabilities

---

## ??? Infrastructure (Prompts 29-32)

### 29. Create Navigation Service

**Persona:** Application Logic Copilot

**Prompt:**
```
Implement navigation service for .NET 8 application with view-viewmodel mapping support.
Create Services/INavigationService.cs and Services/NavigationService.cs supporting parametrized
navigation, navigation history, and modal dialog management. Include view registration,
automatic ViewModel instantiation with dependency injection, navigation events, and proper
cleanup. Design for compatibility with WPF, Avalonia, or other MVVM frameworks.
```

**Expected Output:**
- MVVM navigation patterns
- View-ViewModel mapping
- Navigation history
- Dialog management
- Framework compatibility

---

### 30. Implement Theme System

**Persona:** UI/UX Design Copilot

**Prompt:**
```
Create comprehensive theme system for .NET 8 application with resource management.
Generate Resources/Themes/ folder structure with theme definitions, color palettes,
and styling resources. Include theme switching capability, dark/light mode variants,
and proper integration with application settings. Ensure framework-agnostic design
suitable for WPF, Avalonia, or custom UI implementations.
```

**Expected Output:**
- Theme management system
- Color palette definitions
- Theme switching logic
- Framework-agnostic design
- Settings integration

---

### 31. Setup Repository Pattern

**Persona:** Data Access Copilot

**Prompt:**
```
Implement data access abstraction layer with repository pattern for .NET 8 application.
Create Repositories/IRepository<T>.cs generic interface, Repositories/IUserRepository.cs,
and corresponding implementations. Include generic repository base class, unit of work pattern,
and proper async operations. Integrate with Entity Framework Core or other ORMs,
add comprehensive error handling, and ensure compatibility with different data providers.
```

**Expected Output:**
- Generic repository pattern
- Specific repository implementations
- Unit of work pattern
- ORM integration
- Error handling

---

### 32. Create Validation System

**Persona:** Application Logic Copilot + Data Modeling Copilot

**Prompt:**
```
Implement business rule validation system for .NET 8 application with comprehensive validation framework.
Create Services/IValidationService.cs and validation infrastructure using FluentValidation.
Include custom validation attributes, business rule enforcement, validation result patterns,
and integration with service layer. Add localization support and proper error message handling.
Ensure compatibility with any .NET application framework.
```

**Expected Output:**
- Validation service infrastructure
- FluentValidation integration
- Custom validation rules
- Localization support
- Service layer integration

---

## ?? Quality Assurance (Prompts 33-36)

### 33. Create Unit Testing Infrastructure

**Persona:** Quality Assurance Copilot + Testing Specialist Copilot

**Prompt:**
```
Setup comprehensive unit testing infrastructure for .NET 8 application with modern testing framework.
Create test project with xUnit, Moq, and FluentAssertions. Generate mock implementations for all
service interfaces, test data builders, and testing utilities. Include repository pattern testing,
service layer testing with proper async patterns, and testing fixtures. Add CI/CD integration
configuration and test coverage reporting setup.
```

**Expected Output:**
- Complete test project setup
- Mock implementations
- Test data builders
- Testing utilities
- CI/CD integration

---

### 34. Implement Structured Logging

**Persona:** DevOps Copilot + Application Logic Copilot

**Prompt:**
```
Add centralized structured logging throughout .NET 8 application with Microsoft.Extensions.Logging.
Create logging infrastructure with structured logging patterns, log levels, and proper categorization.
Include performance logging, user action tracking, error correlation IDs, and log enrichment.
Add Serilog integration for advanced logging capabilities, log file rotation, and structured
JSON logging. Ensure integration with existing error handling and configuration systems.
```

**Expected Output:**
- Structured logging infrastructure
- Microsoft.Extensions.Logging integration
- Serilog configuration
- Performance monitoring
- Error correlation

---

### 35. Create Caching Layer

**Persona:** Performance Optimization Copilot

**Prompt:**
```
Implement performance-oriented caching layer for .NET 8 application with Microsoft.Extensions.Caching.
Create Services/ICacheService.cs and Services/CacheService.cs with memory and distributed caching support.
Include cache invalidation strategies, expiration policies, and cache warming. Ensure thread-safety,
proper disposal, and integration with configuration service for cache settings. Include cache
metrics and monitoring capabilities.
```

**Expected Output:**
- Caching service implementation
- Memory and distributed caching
- Cache invalidation strategies
- Performance monitoring
- Configuration integration

---

### 36. Setup Security Infrastructure

**Persona:** Security Specialist Copilot

**Prompt:**
```
Implement authentication, authorization, and secure connection management for .NET 8 application.
Create Services/IAuthenticationService.cs, Services/IAuthorizationService.cs, and security middleware.
Include user authentication with secure credential storage, role-based authorization, and
connection string encryption. Add security headers, input sanitization, and audit logging.
Ensure integration with existing services and proper security event tracking.
```

**Expected Output:**
- Authentication services
- Authorization infrastructure
- Security middleware
- Audit logging
- Input sanitization

---

## ?? Implementation Workflow

### Phase 1: Foundation (Week 1)
Execute prompts 21-24 in order:
1. Result Pattern (21) - Core error handling
2. Data Models (22) - Entity foundation
3. Dependency Injection (23) - Service container
4. Core Services (24) - Service interfaces

### Phase 2: Service Layer (Week 2)
Execute prompts 25-28:
1. Service Layer (25) - Business logic
2. Database Service (26) - Data access
3. Application State (27) - State management
4. Configuration Service (28) - Settings management

### Phase 3: Infrastructure (Week 3)
Execute prompts 29-32:
1. Navigation Service (29) - UI navigation
2. Theme System (30) - UI theming
3. Repository Pattern (31) - Data abstraction
4. Validation System (32) - Business rules

### Phase 4: Quality Assurance (Week 4)
Execute prompts 33-36:
1. Unit Testing (33) - Test infrastructure
2. Structured Logging (34) - Monitoring
3. Caching Layer (35) - Performance
4. Security Infrastructure (36) - Security

---

## ?? Integration Examples

### Basic Service Registration
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Foundation
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        
        // Infrastructure
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<IValidationService, ValidationService>();
        
        return services;
    }
}
```

### Error Handling Integration
```csharp
public class BaseService
{
    protected readonly ILogger _logger;

    protected async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> operation, string operationName)
    {
        try
        {
            _logger.LogInformation("Starting {OperationName}", operationName);
            var result = await operation();
            _logger.LogInformation("Completed {OperationName}", operationName);
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed {OperationName}", operationName);
            return Result<T>.Failure($"Operation {operationName} failed: {ex.Message}");
        }
    }
}
```

### Configuration Usage
```csharp
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfigurationService configService)
    {
        _connectionString = configService.GetConnectionString("DefaultConnection");
    }
}
```

---

## ?? Notes

- All prompts are designed to be framework-agnostic
- Each prompt builds upon previous implementations
- Prompts include comprehensive error handling
- All implementations follow .NET 8 best practices
- Integration examples show real-world usage patterns

---

**Last Updated:** 2025-01-27
**Version:** 1.0.0
**Compatibility:** .NET 8.0+