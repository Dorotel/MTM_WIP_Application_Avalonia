# MTM WIP Application Avalonia - GitHub Copilot Shortcuts

## ?? Custom Prompt Commands for Visual Studio 2022

This project includes 50+ custom GitHub Copilot shortcuts optimized for Visual Studio 2022. Type `@` followed by a category in GitHub Copilot Chat window:

### ?? UI Generation (@ui:)
- `@ui:create` - Generate Avalonia controls/views
- `@ui:viewmodel` - Create ReactiveUI ViewModels  
- `@ui:layout` - Modern layout patterns
- `@ui:theme` - MTM purple theme resources
- `@ui:context` - Context menu integration
- `@ui:markdown` - UI from markdown specs
- `@ui:error` - Error message displays

### ?? Business Logic (@biz:)
- `@biz:handler` - Business logic classes
- `@biz:model` - MTM data models
- `@biz:db` - Database access layers
- `@biz:config` - Configuration files
- `@biz:crud` - Complete CRUD operations

### ??? Database (@db:)
- `@db:procedure` - Stored procedures
- `@db:service` - Database services
- `@db:validation` - Business rule validation
- `@db:error` - Database error handling
- `@db:schema` - Schema documentation
- `@db:update` - Update procedures

### ? Quality Assurance (@qa:)
- `@qa:verify` - Code compliance audits
- `@qa:refactor` - Apply naming conventions
- `@qa:test` - Unit test skeletons
- `@qa:pr` - Pull request documentation
- `@qa:infrastructure` - Testing framework

### ??? Core Systems (@sys:)
- `@sys:di` - Dependency injection setup
- `@sys:services` - Core service interfaces
- `@sys:result` - Result pattern implementation
- `@sys:nav` - Navigation service
- `@sys:state` - Application state management
- `@sys:config` - Configuration service
- `@sys:cache` - Caching layer
- `@sys:security` - Security infrastructure
- `@sys:repository` - Repository pattern
- `@sys:theme` - Theme system
- `@sys:layer` - Service layer
- `@sys:foundation` - Data models foundation

### ?? Compliance Fixes (@fix:)
- `@fix:01` - Empty development stored procedures (COMPLETED ?)
- `@fix:02` - Missing standard output parameters
- `@fix:03` - Inadequate error handling procedures
- `@fix:04` - Missing service layer database integration
- `@fix:05` - Missing data models and DTOs
- `@fix:06` - Missing dependency injection container
- `@fix:07` - Missing theme and resource system
- `@fix:08` - Missing input validation procedures
- `@fix:09` - Inconsistent transaction management
- `@fix:10` - Missing navigation service
- `@fix:11` - Missing configuration service

### ?? Other Commands
- `@err:system` - Error handling classes
- `@err:log` - Logging helpers
- `@err:structured` - Structured logging
- `@doc:api` - XML documentation
- `@doc:prompt` - Custom prompts
- `@doc:update` - Sync instructions
- `@event:handler` - Event handlers
- `@issue:create` - Issue/PR creation

## ?? Usage Examples for Visual Studio 2022

### In GitHub Copilot Chat Window:
```
@ui:create inventory search component
@biz:handler user authentication logic
@qa:verify AdvancedRemoveView.axaml.cs
@db:procedure CreateInventoryItem
@sys:di configure dependency injection for services
@fix:04 integrate services with database procedures
```

### Context-Aware for Current File:
When you have **AdvancedRemoveView.axaml** open:
- `@qa:verify` - Check code compliance
- `@ui:context` - Add context menus to removal operations
- `@err:system` - Improve error handling
- `@ui:theme` - Apply MTM styling improvements

### Multi-Step Workflows:
```
# Complete Feature Development
@ui:create ? @ui:viewmodel ? @biz:handler ? @db:procedure ? @qa:verify

# Quality Assurance
@qa:verify ? @qa:refactor ? @qa:test ? @qa:pr

# Database Enhancement
@db:schema ? @db:procedure ? @db:service ? @fix:04
```

## ?? Reference Files
- **Complete Hotkey Guide**: `Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md`
- **Master Index**: `.github/Automation-Instructions/customprompts.instruction.md`
- **All Prompt Files**: `Documentation/Development/Custom_Prompts/CustomPrompt_*.md`

## ?? How to Use in Visual Studio 2022
1. **Open GitHub Copilot Chat** (View ? Other Windows ? GitHub Copilot Chat)
2. **Type `@` followed by category** (e.g., `@ui:`, `@biz:`, `@qa:`)
3. **Use autocomplete suggestions** as they appear
4. **Chain commands for workflows** using ? notation

The more you use these patterns in Visual Studio 2022, the better Copilot will recognize and suggest them!