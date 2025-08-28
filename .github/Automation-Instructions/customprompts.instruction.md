<!-- Copilot: Reading custom-prompts.instruction.md — Custom Prompts and Copilot Personas -->

# Custom Copilot Prompts - Master Index

This file serves as the **master index** for all custom prompt types used in the MTM WIP Application Avalonia project. Each prompt type has a detailed implementation file in the `Documentation/Development/Custom_Prompts/` folder with complete usage examples, guidelines, and technical requirements.

> **See [personas.instruction.md](personas.instruction.md) for complete persona definitions and behavioral guidelines.**

---

## 🚀 **NEW: Comprehensive Hotkey System** ⭐

**Quick Access**: Use the **[MTM Hotkey Reference System](../../Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md)** for rapid prompt access!

### **Quick Shortcuts (50+ Prompts)**
```sh
# UI Generation
@ui:create @ui:viewmodel @ui:layout @ui:theme @ui:context

# Business Logic  
@biz:handler @biz:model @biz:db @biz:config @biz:crud

# Database Operations
@db:procedure @db:service @db:validation @db:error @db:schema

# Quality Assurance
@qa:verify @qa:refactor @qa:test @qa:pr @qa:infrastructure

# Core Systems
@sys:result @sys:di @sys:services @sys:nav @sys:state

# Compliance Fixes
@fix:01 @fix:02 @fix:03 @fix:04 @fix:05 @fix:06 @fix:07 @fix:08 @fix:09 @fix:10 @fix:11

# Error Handling & Documentation
@err:system @err:log @doc:api @doc:prompt @event:handler @issue:create
```

**Example Usage**: `@ui:create inventory search component` instead of typing full file paths!

---

## 📁 **Folder Organization**

### **Custom Prompt Files Location**
All custom prompt implementation files are located in `Documentation/Development/Custom_Prompts/` with the naming format `CustomPrompt_{Action}_{Where}.md`.

### **HTML Documentation Integration**
Each custom prompt must maintain corresponding HTML documentation files in:
- `Documentation/HTML/PlainEnglish/custom-prompts.html` - Business-friendly explanations
- `Documentation/HTML/Technical/custom-prompts.html` - Developer-focused documentation

### **Hotkey Integration** 🆕
- **[Comprehensive Hotkey Reference](../../Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md)** - All 50+ shortcuts with workflows
- **[Hotkey System Prompt](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_HotkeySystem.md)** - Template for hotkey enhancements

---

## 📋 **Quick Reference - Prompt Types**

### **🎨 UI Generation and Development**
- **[Create UI Element](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElement.md)** - For generating Avalonia controls or views based on mapping and instructions [`@ui:create`]
- **[Create UI Element from Markdown Instructions](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElementFromMarkdown.md)** - For generating Avalonia AXAML and Standard .NET ViewModels from parsed markdown files [`@ui:markdown`]
- **[Create Standard ViewModel](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_StandardViewModel.md)** - For generating ViewModels with standard .NET MVVM patterns, commands, and INotifyPropertyChanged [`@ui:viewmodel`]
- **[Create Modern Layout Pattern](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ModernLayoutPattern.md)** - For generating modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns [`@ui:layout`]
- **[Create Context Menu Integration](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ContextMenuIntegration.md)** - For adding context menus to components with management features following MTM patterns [`@ui:context`]
- **[Create Avalonia Theme Resources](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_AvaloniaThemeResources.md)** - For generating MTM-specific color schemes and theme resources using the purple brand palette [`@ui:theme`]

### **⚠️ Error Handling and Logging**
- **[Create Error System Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ErrorSystemPlaceholder.md)** - For scaffolding error handling classes with standard conventions [`@err:system`]
- **[Create Logging Info Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_LoggingInfoPlaceholder.md)** - For generating logging helpers and configuration according to project standards [`@err:log`]
- **[Create UI Element for Error Messages](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElementForErrorMessages.md)** - For generating error message UI components with MTM theme integration [`@ui:error`]

### **🔧 Business Logic and Data Access**
- **[Create Business Logic Handler](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_BusinessLogicHandler.md)** - For scaffolding business logic classes with proper naming and separation from UI [`@biz:handler`]
- **[Create Database Access Layer Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DatabaseAccessLayerPlaceholder.md)** - For generating database interaction classes or repositories [`@biz:db`]
- **[Create MTM Data Model](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_MTMDataModel.md)** - For generating models that follow MTM-specific data patterns [`@biz:model`]
- **[Create CRUD Operations](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CRUDOperations.md)** - For generating complete CRUD functionality [`@biz:crud`]

### **🗄️ Database Operations** 🆕
- **[Create Stored Procedure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_StoredProcedure.md)** - For creating new database stored procedures [`@db:procedure`]
- **[Update Stored Procedure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Update_StoredProcedure.md)** - For modifying existing stored procedures [`@db:update`]
- **[Database Error Handling](../../Documentation/Development/Custom_Prompts/CustomPrompt_Database_ErrorHandling.md)** - For database-specific error handling patterns [`@db:error`]
- **[Document Database Schema](../../Documentation/Development/Custom_Prompts/CustomPrompt_Document_DatabaseSchema.md)** - For creating database schema documentation [`@db:schema`]
- **[Create Database Service Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DatabaseServiceLayer.md)** - For implementing centralized database access [`@db:service`]
- **[Create Validation System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ValidationSystem.md)** - For implementing business rule validation [`@db:validation`]

### **📋 Testing and Configuration**
- **[Create Pull Request](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_Issue.md)** - Conducts comprehensive code quality reviews against all MTM WIP Application instruction guidelines [`@issue:create`]
- **[Create Unit Test Skeleton](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UnitTestSkeleton.md)** - For generating basic unit test classes with appropriate structure [`@qa:test`]
- **[Create Configuration File Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ConfigurationFilePlaceholder.md)** - For generating config/settings files with standard project structure [`@biz:config`]
- **[Create Unit Testing Infrastructure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UnitTestingInfrastructure.md)** - For setting up comprehensive testing framework with mocks and fixtures [`@qa:infrastructure`]

### **✅ Code Quality and Maintenance**
- **[Verify Code Compliance](../../Documentation/Development/Custom_Prompts/CustomPrompt_Verify_CodeCompliance.md)** - For conducting comprehensive quality assurance reviews against MTM instruction guidelines [`@qa:verify`]
- **[Refactor Code to Naming Convention](../../Documentation/Development/Custom_Prompts/CustomPrompt_Refactor_CodeToNamingConvention.md)** - For requesting code element renaming to fit project naming rules [`@qa:refactor`]
- **[Add Event Handler Stub](../../Documentation/Development/Custom_Prompts/CustomPrompt_Add_EventHandlerStub.md)** - For generating empty event handler methods with TODOs for specific controls [`@event:handler`]

### **📚 Documentation and Project Management**
- **[Document Public API/Class](../../Documentation/Development/Custom_Prompts/CustomPrompt_Document_PublicAPIClass.md)** - For generating XML documentation for public members [`@doc:api`]
- **[Create Customized Prompt](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CustomizedPrompt.md)** - For drafting new actionable custom prompts and assigning appropriate personas [`@doc:prompt`]
- **[Update Instruction File from Master](../../Documentation/Development/Custom_Prompts/CustomPrompt_Update_InstructionFileFromMaster.md)** - For synchronizing instruction files with relevant content from the main copilot-instructions.md [`@doc:update`]

---

## 🏗️ **Missing Core Systems Implementation Prompts**

*Based on analysis from needsrepair.instruction.md identifying missing core systems*

### **Phase 1: Foundation Systems**
- **[Implement Result Pattern System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ResultPatternSystem.md)** - For creating the Result<T> pattern infrastructure for consistent service responses [`@sys:result`]
- **[Create Data Models Foundation](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DataModelsFoundation.md)** - For generating the complete Models namespace with MTM-specific data entities [`@sys:foundation`]
- **[Setup Dependency Injection Container](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_DependencyInjectionContainer.md)** - For configuring Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs [`@sys:di`]
- **[Create Core Service Interfaces](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CoreServiceInterfaces.md)** - For generating all essential service interfaces following MTM patterns [`@sys:services`]

### **Phase 2: Service Layer Implementation**
- **[Implement Service Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ServiceLayer.md)** - For creating complete service implementations with MTM patterns and error handling [`@sys:layer`]
- **[Setup Application State Management](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_ApplicationStateManagement.md)** - For creating global application state service with proper encapsulation [`@sys:state`]
- **[Implement Configuration Service](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ConfigurationService.md)** - For creating service to read and manage appsettings.json configuration [`@sys:config`]

### **Phase 3: Infrastructure Systems**
- **[Create Navigation Service](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_NavigationService.md)** - For implementing proper MVVM navigation patterns with view-viewmodel mapping [`@sys:nav`]
- **[Implement Theme System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ThemeSystem.md)** - For creating MTM purple brand theme resources and DynamicResource patterns [`@sys:theme`]
- **[Setup Repository Pattern](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_RepositoryPattern.md)** - For implementing data access abstraction layer with repository interfaces [`@sys:repository`]

### **Phase 4: Quality Assurance Systems**
- **[Implement Structured Logging](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_StructuredLogging.md)** - For adding centralized logging throughout the application with proper levels [`@err:structured`]
- **[Create Caching Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CachingLayer.md)** - For implementing performance-oriented caching for ComboBox data and user preferences [`@sys:cache`]
- **[Setup Security Infrastructure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_SecurityInfrastructure.md)** - For implementing authentication, authorization, and secure connection management [`@sys:security`]

---

## 🔧 **Compliance Fix Prompts** 🆕

*Critical system fixes identified in quality analysis*

### **Database Foundation (COMPLETED ✅)**
- **[Fix 01: Empty Development Stored Procedures](../../Documentation/Development/Custom_Prompts/Compliance_Fix01_EmptyDevelopmentStoredProcedures.md)** - **COMPLETED** - Database foundation with 12 comprehensive procedures [`@fix:01`]

### **Foundation Fixes (READY 🚀)**
- **[Fix 02: Missing Standard Output Parameters](../../Documentation/Development/Custom_Prompts/Compliance_Fix02_MissingStandardOutputParameters.md)** - Template provided by Fix #1 [`@fix:02`]
- **[Fix 03: Inadequate Error Handling Stored Procedures](../../Documentation/Development/Custom_Prompts/Compliance_Fix03_InadequateErrorHandlingStoredProcedures.md)** - Pattern established by Fix #1 [`@fix:03`]
- **[Fix 04: Missing Service Layer Database Integration](../../Documentation/Development/Custom_Prompts/Compliance_Fix04_MissingServiceLayerDatabaseIntegration.md)** - Can use new procedures [`@fix:04`]
- **[Fix 05: Missing Data Models and DTOs](../../Documentation/Development/Custom_Prompts/Compliance_Fix05_MissingDataModelsAndDTOs.md)** - Can integrate with procedures [`@fix:05`]
- **[Fix 06: Missing Dependency Injection Container](../../Documentation/Development/Custom_Prompts/Compliance_Fix06_MissingDependencyInjectionContainer.md)** - Service layer foundation [`@fix:06`]

### **Infrastructure Fixes (ENABLED 🔧)**
- **[Fix 07: Missing Theme and Resource System](../../Documentation/Development/Custom_Prompts/Compliance_Fix07_MissingThemeAndResourceSystem.md)** - UI layer integration [`@fix:07`]
- **[Fix 08: Missing Input Validation Stored Procedures](../../Documentation/Development/Custom_Prompts/Compliance_Fix08_MissingInputValidationStoredProcedures.md)** - **RESOLVED** by Fix #1 [`@fix:08`]
- **[Fix 09: Inconsistent Transaction Management](../../Documentation/Development/Custom_Prompts/Compliance_Fix09_InconsistentTransactionManagement.md)** - **RESOLVED** by Fix #1 [`@fix:09`]
- **[Fix 10: Missing Navigation Service](../../Documentation/Development/Custom_Prompts/Compliance_Fix10_MissingNavigationService.md)** - UI infrastructure [`@fix:10`]
- **[Fix 11: Missing Configuration Service](../../Documentation/Development/Custom_Prompts/Compliance_Fix11_MissingConfigurationService.md)** - Service layer integration [`@fix:11`]

---

## 📘 **Rules for Managing Custom Prompts**

### **Rule 1: File Creation and Organization**
- **Whenever a new custom prompt is added to this master index, a corresponding detailed implementation file must be created in `Documentation/Development/Custom_Prompts/` folder using the naming format `CustomPrompt_{Action}_{Where}.md`.**
- **Each prompt file must include the complete template structure: Instructions, Persona, Prompt Template, Purpose, Usage Examples, Guidelines, Related Files, and Quality Checklist.**
- **All prompts must reference appropriate personas from [personas.instruction.md](personas.instruction.md).**

### **Rule 2: HTML Documentation Synchronization** ⭐ **NEW**
**Any time a custom prompt is created, removed, or edited, the corresponding HTML documentation files MUST be updated:**

#### **Required HTML Updates**
1. **Plain English Documentation**: Update `Documentation/HTML/PlainEnglish/custom-prompts.html`
   - Add business-friendly explanations of what the prompt accomplishes
   - Use manufacturing analogies and non-technical language
   - Focus on workflow benefits and process improvements
   - Include when and why to use each prompt from a business perspective

2. **Technical Documentation**: Update `Documentation/HTML/Technical/custom-prompts.html` 
   - Add detailed technical implementation guidance
   - Include code examples and integration patterns
   - Reference MTM-specific requirements and constraints
   - Provide troubleshooting and edge case handling

#### **HTML Update Process**
1. **Create/Edit Prompt**: Make changes to prompt files in `Documentation/Development/Custom_Prompts/`
2. **Update Master Index**: Update this file with new prompt references
3. **Update Plain English HTML**: Modify `Documentation/HTML/PlainEnglish/custom-prompts.html`
4. **Update Technical HTML**: Modify `Documentation/HTML/Technical/custom-prompts.html`
5. **Test Navigation**: Ensure all links work correctly between HTML files
6. **Validate Content**: Verify HTML content matches prompt file information

#### **HTML Content Standards**
- **Responsive Design**: Use all CSS files (modern-styles.css, mtm-theme.css, plain-english.css, responsive.css)
- **Cross-Reference Navigation**: Include navigation buttons between Plain English and Technical versions
- **MTM Branding**: Apply appropriate color schemes (Black/Gold for Plain English, Purple for Technical)
- **Accessibility**: Follow WCAG AA guidelines for all HTML content
- **Mobile Support**: Ensure proper display on mobile devices

### **Rule 3: Hotkey Integration** 🆕 **NEW**
**Any time a custom prompt is created, removed, or edited, the hotkey system MUST be updated:**

#### **Required Hotkey Updates**
1. **Update Hotkey Reference**: Modify `Documentation/Development/Custom_Prompts/MTM_Hotkey_Reference.md`
   - Add new shortcuts following category conventions (@ui:, @biz:, @db:, @qa:, @sys:, @err:, @doc:, @fix:, @event:, @issue:)
   - Include usage examples and workflow integration
   - Update quick reference tables and statistics

2. **Update Master Index**: Add hotkey shortcuts in brackets after each prompt link (e.g., [`@ui:create`])

3. **Test Shortcuts**: Verify hotkey patterns are memorable and consistent with existing conventions

### **Rule 4: Quality and Compliance**
- All custom prompt files must follow MTM coding conventions and instruction guidelines
- Each prompt must include proper error handling patterns
- Reference organized instruction files appropriately
- Maintain consistency with existing documentation structure

---

## 🎯 **Usage Guidelines**

For detailed usage guidelines, MTM-specific workflow patterns, persona behavioral guidelines, and integration with development workflow, see the comprehensive guide in [Documentation/Development/Custom_Prompts/README.md](../../Documentation/Development/Custom_Prompts/README.md).

### **Quick Usage Steps**
1. **Use Hotkeys**: Try the shortcut first (e.g., `@ui:create component_name`)
2. **Navigate to Specific Prompt**: Click on the prompt link to access the detailed implementation file
3. **Review Persona Requirements**: Check the assigned persona and behavioral guidelines
4. **Copy Template**: Use the provided prompt template as-is or customize for your specific needs
5. **Follow Guidelines**: Adhere to the technical requirements and MTM-specific patterns
6. **Check Examples**: Review usage examples for context and proper implementation
7. **Update HTML Documentation**: If creating/editing prompts, update corresponding HTML files

### **Hotkey Workflow Examples** 🆕
```sh
# Quick UI Development
@ui:create → @ui:viewmodel → @ui:theme → @qa:verify

# Database Operations
@db:schema → @db:procedure → @db:service → @fix:04

# Compliance Resolution
@qa:verify → @fix:XX → @qa:refactor → @qa:test → @qa:pr

# Complete Feature Development
@ui:create → @biz:handler → @db:procedure → @qa:test → @qa:verify
```

---

*This master index provides centralized access to all 50+ custom prompts for the MTM WIP Application Avalonia project, now with comprehensive hotkey integration for maximum development efficiency. Each prompt is designed to maintain consistency with MTM standards and accelerate development while ensuring quality and compliance.*