# Advanced GitHub Copilot Integration Scenarios - MTM WIP Application

## 🤖 Sophisticated AI-Assisted Development for Manufacturing Applications

This document provides advanced GitHub Copilot integration scenarios specifically designed for the MTM WIP Application, enabling sophisticated AI assistance for complex manufacturing inventory management development patterns.

---

## 🎯 Advanced Copilot Prompt Templates

### Manufacturing Domain Code Generation Prompts

#### Complex ViewModel Creation with Manufacturing Context
```
Generate a complete MVVM Community Toolkit ViewModel for manufacturing inventory transaction processing with:
- ObservableProperty for PartId (string), Operation (string, validate against 90,100,110,120), Quantity (int, min 1), Location (string)
- RelayCommand for SaveTransactionAsync with proper error handling using ErrorHandling.HandleErrorAsync
- Validation logic ensuring TransactionType is determined by user intent (IN/OUT/TRANSFER)
- Manufacturing business rules: no negative inventory, operation sequence validation
- Integration with IInventoryService and ITransactionService
- QuickButton creation from successful transactions
- Cross-platform compatibility considerations
Use MTM manufacturing context with part numbers like "MOTOR_HOUSING_001", operations "90","100","110","120", locations "STATION_A"
```

#### Advanced Database Integration with Connection Management
```
Create a resilient database service class for manufacturing operations with:
- Connection pooling optimized for manufacturing workload (min 5, max 50 connections)
- Retry logic with exponential backoff for network issues
- Circuit breaker pattern for external database failures
- All operations using Helper_Database_StoredProcedure.ExecuteDataTableWithStatus only
- Transaction support for multi-step manufacturing operations
- Performance monitoring for slow queries (>5 seconds)
- Comprehensive logging using ILogger with structured logging
- Manufacturing-specific stored procedures: inv_inventory_Add_Item, inv_transaction_Add, md_part_ids_Get_All
Include connection string validation and health check endpoints
```

#### Cross-Platform UI Component with Manufacturing Features
```
Generate an Avalonia UserControl for manufacturing inventory entry with:
- Cross-platform compatibility (Windows/macOS/Linux/Android)
- MVVM data binding to InventoryTabViewModel
- Manufacturing-optimized input controls: part ID autocomplete, operation dropdown (90,100,110,120), numeric quantity, location selection
- Real-time validation with red borders for invalid input
- Keyboard shortcuts: F1=operation 90, F2=operation 100, F3=operation 110, F4=operation 120
- Touch-friendly design for tablet manufacturing stations
- Theme integration using DynamicResource for MTM_Blue/Green/Red/Dark themes
- Accessibility support with screen reader labels
- Manufacturing workflow validation: prevent quantity > available for OUT transactions
Use x:Name (not Name) for grids, xmlns="https://github.com/avaloniaui" namespace, proper AXAML syntax
```

### Integration Pattern Generation Prompts

#### Service Integration with MVVM Messaging
```
Create service integration pattern using MVVM Community Toolkit messaging for:
- InventoryService publishing InventoryAddedMessage when items added
- QuickButtonsService subscribing to create quick buttons from transactions
- TransactionService recording all inventory changes with audit trail
- MasterDataService caching part IDs, operations, locations with memory cache
- Cross-service validation ensuring data consistency
- Error propagation with centralized ErrorHandling.HandleErrorAsync
- Performance monitoring with execution time tracking
- Manufacturing business rule enforcement across service boundaries
Include IMessenger registration, message definitions, and subscriber patterns with proper cleanup
```

#### External System Integration with Circuit Breaker
```
Implement external manufacturing system integration with:
- HttpClient with Polly circuit breaker for MES (Manufacturing Execution System) API
- Authentication using OAuth2 with token refresh
- Retry policy for transient failures (3 attempts, exponential backoff)
- Rate limiting to respect external system constraints (10 requests/second)
- Request/response logging with sensitive data masking
- Timeout handling (30 second default, configurable)
- Health check integration for monitoring external system availability
- Fallback mechanisms when external system unavailable
- Manufacturing data synchronization (part master, operation routing, work orders)
Include circuit breaker configuration, health check endpoints, and monitoring dashboard integration
```

## 🏭 Manufacturing-Specific AI Assistance Scenarios

### Complex Manufacturing Workflow Implementation
```
Create complete manufacturing workflow automation for:
- Part receiving (Operation 90): validate against purchase orders, update inventory, create transaction
- Manufacturing progression (90→100→110→120): validate operation sequence, check WIP levels, update locations
- Quality control integration: handle scrap transactions, quality holds, rework scenarios
- Shipping preparation (Operation 120→130): validate customer orders, allocate inventory, generate pick lists
- Manufacturing reporting: WIP levels, throughput metrics, cycle time analysis
- Cross-platform operation ensuring identical behavior on Windows/macOS/Linux production systems
Use MTM domain patterns: user intent determines transaction type, operation numbers are workflow steps
Include comprehensive error handling, transaction rollback, and audit logging
```

### Advanced Manufacturing Data Validation
```
Generate manufacturing data validation system with:
- Part ID validation: alphanumeric with dashes, max 50 characters, exists in master data
- Operation sequence validation: ensure valid progression (90→100→110→120→130)
- Quantity validation: positive integers, not exceeding available inventory for OUT transactions
- Location validation: physical location exists and is active
- Business rule validation: prevent negative inventory, validate operation transitions
- Cross-reference validation: part exists in master data, operation valid for part routing
- Manufacturing constraint validation: work order requirements, capacity constraints
- Performance optimization: batch validation for large datasets, caching frequently accessed data
Include validation result patterns, error message standardization, and performance benchmarks
```

### Manufacturing Performance Optimization Patterns
```
Create performance optimization for manufacturing operations:
- Database connection pooling optimized for shift patterns (3 shifts, 8 hours each)
- Query optimization for inventory lookups with proper indexing strategy
- Memory management for large manufacturing datasets (thousands of parts, millions of transactions)
- UI virtualization for manufacturing data grids with smooth scrolling
- Background processing for non-critical operations (reporting, data export)
- Caching strategy for master data (part IDs, operations, locations) with 5-minute expiration
- Batch processing for high-volume manufacturing transactions
- Cross-platform performance ensuring consistent response times on all platforms
Include performance monitoring, KPI tracking, and automated performance testing
```

## 🔧 Advanced Technical Pattern Prompts

### Complex MVVM Patterns with Manufacturing Context
```
Generate advanced MVVM pattern for manufacturing dashboard with:
- Multiple coordinated ViewModels: InventoryViewModel, TransactionHistoryViewModel, AlertsViewModel
- Cross-ViewModel communication using WeakReferenceMessenger
- Advanced property patterns: computed properties, property change cascading, validation dependencies
- Command orchestration: complex workflows with multiple service calls
- Error state management: comprehensive error handling with user-friendly messages
- Loading state coordination: manage loading states across multiple ViewModels
- Manufacturing KPI calculations: real-time inventory levels, transaction rates, efficiency metrics
- Memory management: proper disposal patterns for long-running manufacturing dashboards
Use manufacturing domain: inventory accuracy %, transaction throughput, WIP levels, cycle times
```

### Advanced Database Transaction Patterns
```
Create advanced database transaction pattern for manufacturing with:
- Distributed transaction support for multi-database operations
- Saga pattern for long-running manufacturing workflows
- Optimistic concurrency control for high-volume manufacturing environments  
- Dead letter queue for failed manufacturing transactions
- Event sourcing for manufacturing audit trail requirements
- Database migration patterns for manufacturing schema changes
- Cross-platform database compatibility (MySQL on Windows/Linux containers)
- Performance monitoring with slow query detection and optimization
- Backup and recovery procedures for manufacturing data integrity
Include transaction isolation levels, deadlock detection, and performance benchmarking
```

### Cross-Platform Manufacturing Deployment
```
Generate deployment automation for manufacturing environments:
- Docker containerization for Linux manufacturing servers
- Windows Service deployment for manufacturing floor PCs
- Android APK deployment for mobile manufacturing tablets
- Auto-update mechanisms for manufacturing environment constraints
- Configuration management for different manufacturing sites
- Database migration scripts for manufacturing schema updates
- Health check monitoring for manufacturing system reliability
- Rollback procedures for manufacturing environment stability
- Performance monitoring tailored to manufacturing KPIs
Include environment-specific configurations, deployment validation, and rollback procedures
```

## 🧪 Advanced Testing Scenario Prompts

### Manufacturing Integration Testing
```
Create comprehensive integration tests for manufacturing workflows:
- End-to-end manufacturing process testing (receive → manufacture → ship)
- Cross-service integration validation with manufacturing business rules
- Database transaction integrity testing with manufacturing constraints
- External system integration testing with MES/ERP systems
- Performance testing under manufacturing load (shift changeover scenarios)
- Cross-platform testing ensuring identical manufacturing operations
- Failure scenario testing: database outages, network issues, external system failures
- Data integrity validation throughout manufacturing workflows
Include test data management, manufacturing scenario simulation, and performance benchmarking
```

### Manufacturing Performance Testing
```
Generate performance testing for manufacturing workloads:
- Load testing: 100 concurrent users during shift changeover
- Stress testing: peak manufacturing periods (end of shift, month-end closing)
- Volume testing: large manufacturing datasets (millions of transactions, thousands of parts)
- Endurance testing: 24/7 manufacturing operations
- Spike testing: sudden load increases during manufacturing emergencies
- Manufacturing KPI validation: <2 second response times for transactions
- Cross-platform performance consistency validation
- Memory and resource usage monitoring under manufacturing loads
Include realistic manufacturing scenarios, performance baselines, and automated performance regression detection
```

## 🎯 Context-Aware Code Completion Scenarios

### Manufacturing Business Logic Context
When Copilot detects manufacturing-related code, it should automatically:
- Suggest MTM-specific patterns: transaction types based on user intent, not operation numbers
- Recommend appropriate validation: part ID format, operation sequence, quantity constraints
- Propose error handling: use centralized ErrorHandling.HandleErrorAsync pattern
- Suggest logging: structured logging with manufacturing context (part ID, operation, user)
- Recommend testing: include manufacturing business rule validation in unit tests

### Technology Stack Context
When Copilot detects MTM technology stack usage, it should:
- Use exact versions: .NET 8, Avalonia UI 11.3.4, MVVM Community Toolkit 8.3.2, MySQL 9.4.0
- Suggest appropriate patterns: [ObservableProperty]/[RelayCommand] for MVVM, stored procedures for database
- Prevent deprecated patterns: no ReactiveUI, no direct SQL, no WPF syntax in Avalonia
- Recommend cross-platform patterns: consistent behavior across Windows/macOS/Linux/Android
- Suggest performance optimizations: connection pooling, caching, batch processing

### Cross-Platform Development Context
When Copilot detects cross-platform scenarios, it should:
- Recommend platform-specific implementations when needed
- Suggest cross-platform testing strategies
- Propose configuration management for different platforms
- Recommend deployment patterns for each platform
- Suggest performance monitoring for cross-platform consistency

## 📊 AI-Assisted Quality Assurance

### Automated Code Review Assistance
```
Analyze this MTM code for:
- MVVM Community Toolkit pattern compliance ([ObservableProperty], [RelayCommand])
- Manufacturing business rule accuracy (transaction types, operation sequences)
- Database pattern compliance (stored procedures only, no direct SQL)
- Avalonia UI syntax compliance (x:Name, proper namespace, DynamicResource themes)
- Cross-platform compatibility considerations
- Error handling completeness (ErrorHandling.HandleErrorAsync usage)
- Performance considerations for manufacturing workloads
- Manufacturing domain accuracy (realistic examples, proper terminology)
```

### Automated Testing Assistance
```
Generate comprehensive tests for this MTM component:
- Unit tests with manufacturing domain validation
- Integration tests with database and service interaction
- Cross-platform compatibility tests
- Performance tests with manufacturing workload simulation
- UI tests with Avalonia.Headless framework
- Manufacturing business rule validation tests
- Error scenario testing with manufacturing context
Include test data builders, mock configurations, and performance benchmarks
```

## 🔄 Continuous AI Learning Integration

### Pattern Recognition Enhancement
Regular analysis of MTM codebase to improve Copilot suggestions:
- Manufacturing pattern frequency analysis
- Common error patterns and prevention suggestions  
- Performance optimization opportunity identification
- Cross-platform compatibility issue detection
- Manufacturing domain accuracy validation

### Context Improvement Framework
Continuous enhancement of AI context based on:
- Developer feedback and usage patterns
- Code review comments and suggestions
- Performance metrics and optimization opportunities
- Manufacturing domain expert validation
- Cross-platform testing results and compatibility requirements

---

**Document Status**: ✅ Complete Advanced GitHub Copilot Integration  
**AI Scenarios**: 20+ sophisticated development assistance scenarios  
**Manufacturing Context**: Complete business domain integration for AI assistance  
**Framework Versions**: .NET 8, Avalonia UI 11.3.4, MVVM Community Toolkit 8.3.2, MySQL 9.4.0  
**Last Updated**: 2025-09-15  
**AI Integration Owner**: MTM Development Team

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**
- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**
- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**
- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**
- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**
- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**
- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**
- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
