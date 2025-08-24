# MTM Custom Prompts - Comprehensive Hotkey Reference System

## 🚀 **Quick Access Shortcuts for GitHub Copilot Chatbox**

This reference provides memorizable shortcuts for accessing **ALL** MTM custom prompts quickly in the GitHub Copilot chatbox.

---

## 📋 **Complete Hotkey Categories (50+ Prompts)**

### **🎨 UI Generation (@ui:)**
```sh
@ui:create          # Create UI Element - Generate Avalonia controls/views
@ui:markdown        # Create UI Element from Markdown - Parse .md to AXAML
@ui:viewmodel       # Create ReactiveUI ViewModel - Generate ViewModels
@ui:layout          # Create Modern Layout Pattern - Sidebars, cards, hero sections
@ui:context         # Create Context Menu Integration - Right-click menus
@ui:theme           # Create Avalonia Theme Resources - MTM purple palette
@ui:error           # Create UI Element for Error Messages - Error displays
```

### **🔧 Business Logic (@biz:)**
```sh
@biz:handler        # Create Business Logic Handler - Scaffold business classes
@biz:model          # Create MTM Data Model - Generate data entities
@biz:db             # Create Database Access Layer Placeholder - Repository patterns
@biz:config         # Create Configuration File Placeholder - Settings files
@biz:crud           # Create CRUD Operations - Complete CRUD functionality
```

### **🗄️ Database Operations (@db:)**
```sh
@db:procedure       # Create Stored Procedure - New database procedures
@db:update          # Update Stored Procedure - Modify existing procedures
@db:error           # Database Error Handling - Database-specific error patterns
@db:schema          # Document Database Schema - Schema documentation
@db:service         # Create Database Service Layer - Centralized database access
@db:validation      # Create Validation System - Business rule validation
```


### **✅ Quality Assurance (@qa:)**
```sh
@qa:verify          # Verify Code Compliance - Quality audits
@qa:refactor        # Refactor Code to Naming Convention - Rename elements
@qa:test            # Create Unit Test Skeleton - Test class structure
@qa:pr              # Create Pull Request - Code quality reviews
@qa:infrastructure  # Create Unit Testing Infrastructure - Testing framework
```

### **⚠️ Error Handling (@err:)**
```sh
@err:system         # Create Error System Placeholder - Error handling classes
@err:log            # Create Logging Info Placeholder - Logging helpers
@err:structured     # Implement Structured Logging - Centralized logging
```

### **📚 Documentation (@doc:)**
```sh
@doc:api            # Document Public API/Class - XML documentation
@doc:prompt         # Create Customized Prompt - New prompt templates
@doc:update         # Update Instruction File from Master - Sync instructions
```

### **🏗️ Core Systems (@sys:)**
```sh
@sys:result         # Implement Result Pattern System - Result<T> infrastructure
@sys:foundation     # Create Data Models Foundation - Complete Models namespace
@sys:di             # Setup Dependency Injection Container - Configure DI
@sys:services       # Create Core Service Interfaces - Essential interfaces
@sys:layer          # Implement Service Layer - Complete services
@sys:state          # Setup Application State Management - Global state
@sys:nav            # Create Navigation Service - MVVM navigation
@sys:cache          # Create Caching Layer - Performance caching
@sys:security       # Setup Security Infrastructure - Auth/security
@sys:config         # Implement Configuration Service - appsettings.json management
@sys:repository     # Setup Repository Pattern - Data access abstraction
@sys:theme          # Implement Theme System - MTM purple brand resources
```

### **🎯 Event Handling (@event:)**
```sh
@event:handler      # Add Event Handler Stub - Empty event methods
```

### **📋 Issue Management (@issue:)**
```sh
@issue:create       # Create Pull Request/Issue - Comprehensive issue documentation
```

### **🔧 Compliance Fixes (@fix:)**
```sh
@fix:01             # Fix Empty Development Stored Procedures - Database foundation
@fix:02             # Fix Missing Standard Output Parameters - Output standardization
@fix:03             # Fix Inadequate Error Handling Stored Procedures - Database errors
@fix:04             # Fix Missing Service Layer Database Integration - Service connection
@fix:05             # Fix Missing Data Models and DTOs - Model structure
@fix:06             # Fix Missing Dependency Injection Container - DI setup
@fix:07             # Fix Missing Theme and Resource System - UI theming
@fix:08             # Fix Missing Input Validation Stored Procedures - Input validation
@fix:09             # Fix Inconsistent Transaction Management - Transaction handling
@fix:10             # Fix Missing Navigation Service - MVVM navigation
@fix:11             # Fix Missing Configuration Service - Config management
```

---

## 🔍 **Enhanced Usage Patterns**

### **Basic Usage**
```sh
# Instead of typing the full prompt, use shortcuts:
@ui:create inventory search component
@biz:handler user authentication logic  
@qa:verify AdvancedRemoveView.axaml.cs
@db:procedure CreateInventoryItem
```

### **File Reference Combined**
```sh
# Combine with file references for context:
@Views/MainForm/AdvancedRemoveView.axaml.cs @ui:context add removal operations menu
@Services/DatabaseService.cs @db:service enhance database connection management
@Models/Shared/CoreModels.cs @biz:model add MTM data validation patterns
```

### **Compliance Fix Workflow**
```sh
# Address specific compliance issues:
@fix:01 create missing stored procedures for inventory operations
@fix:04 integrate services with new database procedures  
@fix:06 setup dependency injection container for all services
```

### **Multi-Step Development Workflows**
```sh
# Complete feature development:
@ui:create → @ui:viewmodel → @biz:handler → @db:procedure → @qa:verify

# Quality assurance workflow:
@qa:verify → @qa:refactor → @qa:test → @qa:pr

# Database enhancement workflow:
@db:schema → @db:procedure → @db:service → @db:validation
```

### **Fallback to Full Prompts**
```sh
# When you need the complete prompt template:
@Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElement.md
@Documentation/Development/Custom_Prompts/Compliance_Fix04_MissingServiceLayerDatabaseIntegration.md
```

---

## 📖 **Enhanced Quick Reference Card**

| Category | Common Shortcuts | Purpose | Priority |
|----------|------------------|---------|----------|
| **@ui:** | create, viewmodel, layout, theme, context | UI/AXAML generation | High |
| **@biz:** | handler, model, db, config, crud | Business logic & data | High |
| **@db:** | procedure, service, validation, error | Database operations | Critical |
| **@qa:** | verify, refactor, test, pr | Quality assurance | High |
| **@sys:** | result, di, services, nav, state | Core infrastructure | Critical |
| **@err:** | system, log, structured | Error handling | Medium |
| **@doc:** | api, prompt, update | Documentation | Medium |
| **@fix:** | 01-11 | Compliance fixes | Critical |
| **@event:** | handler | Event management | Low |
| **@issue:** | create | Issue tracking | Medium |

---

## 🎯 **Comprehensive Workflow Examples**

### **Creating New Components (Complete Flow)**
1. `@ui:create` → Generate AXAML structure
2. `@ui:viewmodel` → Create ReactiveUI ViewModel  
3. `@biz:handler` → Add business logic
4. `@db:procedure` → Create database operations
5. `@ui:theme` → Apply MTM styling
6. `@qa:verify` → Quality check
7. `@qa:test` → Add unit tests

### **Database Enhancement Workflow**
1. `@db:schema` → Document current schema
2. `@db:procedure` → Create new procedures
3. `@db:service` → Build service layer
4. `@db:validation` → Add validation rules
5. `@fix:04` → Integrate with services
6. `@qa:verify` → Compliance check

### **Compliance Resolution Workflow**
1. `@qa:verify` → Identify compliance issues
2. `@fix:XX` → Address specific fixes (01-11)
3. `@qa:refactor` → Apply naming conventions
4. `@qa:test` → Add test coverage
5. `@qa:pr` → Document changes

### **Core System Implementation**
1. `@sys:foundation` → Data models
2. `@sys:di` → Dependency injection
3. `@sys:services` → Service interfaces
4. `@sys:layer` → Service implementations
5. `@sys:nav` → Navigation service
6. `@sys:config` → Configuration management

---

## 💡 **Enhanced Pro Tips**

### **Memory Aids by Category**
- **@ui:** = "User Interface" development
- **@biz:** = "Business" logic implementation
- **@db:** = "Database" operations and management
- **@qa:** = "Quality Assurance" and testing
- **@sys:** = "System" infrastructure and core services
- **@err:** = "Error" handling and logging
- **@doc:** = "Documentation" generation
- **@fix:** = "Fix" compliance issues (numbered 01-11)
- **@event:** = "Event" handling and wiring
- **@issue:** = "Issue" tracking and management

### **Efficiency Shortcuts**
- Use category prefixes to narrow down prompts quickly
- Combine shortcuts with file references for context
- Use numbered @fix: shortcuts for specific compliance issues
- Chain workflows for complete feature development
- Bookmark this reference for instant access
- Use HTML documentation for visual prompt selection

### **Priority-Based Usage**
- **Critical (@db:, @sys:, @fix:)**: Infrastructure and compliance
- **High (@ui:, @biz:, @qa:)**: Core development and quality
- **Medium (@err:, @doc:, @issue:)**: Supporting systems
- **Low (@event:)**: Specific functionality

### **Integration with Existing Workflow**
- These shortcuts work with current GitHub Copilot features
- Compatible with `@filename` references
- Can be combined with slash commands like `/explain`
- Maintains full access to detailed prompt templates
- Supports multi-file operations and workflows

---

## 🔄 **Workflow Integration Patterns**

### **New Feature Development Pattern**
```sh
# Planning Phase
@qa:verify existing_file.cs          # Check current state
@doc:api document_requirements        # Document needs

# Implementation Phase  
@ui:create new_component             # Create UI
@ui:viewmodel reactive_patterns      # Add ViewModel
@biz:handler business_logic          # Business layer
@db:procedure data_operations        # Database layer

# Quality Phase
@qa:test unit_test_coverage         # Add tests
@qa:verify compliance_check         # Final verification
@qa:pr generate_documentation       # Document changes
```

### **Bug Fix Pattern**
```sh
@qa:verify identify_issues          # Find problems
@err:log add_error_tracking         # Add logging
@fix:XX address_specific_issue      # Apply fixes
@qa:refactor improve_code_quality   # Refactor
@qa:test regression_testing         # Test fixes
```

### **Compliance Improvement Pattern**
```sh
@qa:verify full_project_audit       # Comprehensive audit
@fix:01 database_foundation         # Critical database fixes
@fix:04 service_integration         # Service layer fixes
@fix:06 dependency_injection        # DI container setup
@qa:verify post_fix_validation      # Verify improvements
```

---

## 🔗 **Related Resources and Integration**

### **File Structure Integration**
- **Master Index**: `.github/Automation-Instructions/customprompts.instruction.md`
- **Detailed Prompts**: `Documentation/Development/Custom_Prompts/CustomPrompt_*.md`
- **Compliance Fixes**: `Documentation/Development/Custom_Prompts/Compliance_Fix**.md`
- **HTML Documentation**: `Documentation/HTML/Technical/custom-prompts.html`
- **Examples**: `.github/custom-prompts-examples.md`

### **Persona Integration**
- **UI Architect Copilot**: @ui: shortcuts
- **Database Specialist Copilot**: @db: shortcuts  
- **Quality Assurance Auditor Copilot**: @qa: and @fix: shortcuts
- **Business Logic Specialist Copilot**: @biz: shortcuts
- **System Infrastructure Copilot**: @sys: shortcuts

### **Instruction File Integration**
- **Core Instructions**: Coding conventions, naming standards
- **UI Instructions**: AXAML generation, design patterns
- **Development Instructions**: Error handling, database patterns
- **Quality Instructions**: Compliance standards, repair guidelines
- **Automation Instructions**: Persona definitions, prompt templates

---

## 📊 **Hotkey Coverage Statistics**

### **Total Prompts Covered: 50+**
- **UI Generation**: 7 prompts
- **Business Logic**: 5 prompts  
- **Database Operations**: 6 prompts
- **Quality Assurance**: 5 prompts
- **Core Systems**: 12 prompts
- **Error Handling**: 3 prompts
- **Documentation**: 3 prompts
- **Compliance Fixes**: 11 prompts
- **Event/Issue Management**: 2 prompts

### **Usage Frequency Optimization**
- **Most Used**: @ui:create, @qa:verify, @biz:handler
- **Critical Infrastructure**: @sys:di, @db:service, @fix:01
- **Quality Focus**: @qa:refactor, @qa:test, @qa:pr
- **Specialized**: @db:procedure, @ui:context, @sys:cache

---

## 🚀 **Quick Start Guide**

### **For New Developers**
1. **Bookmark**: This hotkey reference file
2. **Start with**: @ui:create for UI work, @biz:handler for logic
3. **Quality check**: Always use @qa:verify before completing work
4. **Learn patterns**: Review full prompts for detailed understanding

### **For Experienced Developers**
1. **Efficiency**: Use category patterns (@ui:, @biz:, @qa:, etc.)
2. **Workflows**: Chain shortcuts for complete feature development
3. **Compliance**: Use @fix: shortcuts for specific issue resolution
4. **Documentation**: Use @doc: shortcuts to maintain project docs

### **For Quality Assurance**
1. **Auditing**: @qa:verify for comprehensive reviews
2. **Compliance**: @fix:XX for specific violation fixes
3. **Testing**: @qa:test and @qa:infrastructure for test coverage
4. **Reporting**: @qa:pr for documentation generation

---

*This comprehensive hotkey reference system provides quick access to all 50+ MTM custom prompts, enabling efficient development workflows while maintaining quality and compliance standards.*