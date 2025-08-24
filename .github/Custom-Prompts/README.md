# Custom Copilot Prompts - Master Index

This file serves as the **master index** for all custom prompt types used in the MTM WIP Application Avalonia project. Each prompt type has a detailed implementation file in this folder with complete usage examples, guidelines, and technical requirements.

> **See [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) for complete persona definitions and behavioral guidelines.**

---

## ?? **Folder Organization**

### **Custom Prompt Files Location**
All custom prompt implementation files are located in `.github/Custom-Prompts/` with the naming format `CustomPrompt_{Action}_{Where}.md`.

### **HTML Documentation Integration**
Each custom prompt must maintain corresponding HTML documentation files in:
- `Documentation/HTML/PlainEnglish/custom-prompts.html` - Business-friendly explanations
- `Documentation/HTML/Technical/custom-prompts.html` - Developer-focused documentation

---

## ?? **Quick Reference - Prompt Types**

### **UI Generation and Development**
- **[Create UI Element](CustomPrompt_Create_UIElement.md)** - For generating Avalonia controls or views based on mapping and instructions
- **[Create UI Element from Markdown Instructions](CustomPrompt_Create_UIElementFromMarkdown.md)** - For generating Avalonia AXAML and ReactiveUI ViewModels from parsed markdown files
- **[Create ReactiveUI ViewModel](CustomPrompt_Create_ReactiveUIViewModel.md)** - For generating ViewModels with ReactiveUI patterns, commands, and observable properties
- **[Create Modern Layout Pattern](CustomPrompt_Create_ModernLayoutPattern.md)** - For generating modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns
- **[Create Context Menu Integration](CustomPrompt_Create_ContextMenuIntegration.md)** - For adding context menus to components with management features following MTM patterns
- **[Create Avalonia Theme Resources](CustomPrompt_Create_AvaloniaThemeResources.md)** - For generating MTM-specific color schemes and theme resources using the purple brand palette

### **Error Handling and Logging**
- **[Create Error System Placeholder](CustomPrompt_Create_ErrorSystemPlaceholder.md)** - For scaffolding error handling classes with standard conventions
- **[Create Logging Info Placeholder](CustomPrompt_Create_LoggingInfoPlaceholder.md)** - For generating logging helpers and configuration according to project standards
- **[Create UI Element for Error Messages](CustomPrompt_Create_UIElementForErrorMessages.md)** - For generating error message UI components with MTM theme integration

### **Business Logic and Data Access**
- **[Create Business Logic Handler](CustomPrompt_Create_BusinessLogicHandler.md)** - For scaffolding business logic classes with proper naming and separation from UI
- **[Create Database Access Layer Placeholder](CustomPrompt_Create_DatabaseAccessLayerPlaceholder.md)** - For generating database interaction classes or repositories
- **[Create MTM Data Model](CustomPrompt_Create_MTMDataModel.md)** - For generating models that follow MTM-specific data patterns

### **Testing and Configuration**
- **[Create Unit Test Skeleton](CustomPrompt_Create_UnitTestSkeleton.md)** - For generating basic unit test classes with appropriate structure
- **[Create Configuration File Placeholder](CustomPrompt_Create_ConfigurationFilePlaceholder.md)** - For generating config/settings files with standard project structure

### **Code Quality and Maintenance**
- **[Verify Code Compliance](CustomPrompt_Verify_CodeCompliance.md)** - For conducting comprehensive quality assurance reviews against MTM instruction guidelines
- **[Refactor Code to Naming Convention](CustomPrompt_Refactor_CodeToNamingConvention.md)** - For requesting code element renaming to fit project naming rules
- **[Add Event Handler Stub](CustomPrompt_Add_EventHandlerStub.md)** - For generating empty event handler methods with TODOs for specific controls

### **Documentation and Project Management**
- **[Document Public API/Class](CustomPrompt_Document_PublicAPIClass.md)** - For generating XML documentation for public members
- **[Create Customized Prompt](CustomPrompt_Create_CustomizedPrompt.md)** - For drafting new actionable custom prompts and assigning appropriate personas
- **[Update Instruction File from Master](CustomPrompt_Update_InstructionFileFromMaster.md)** - For synchronizing instruction files with relevant content from the main copilot-instructions.md

---

## ??? **Missing Core Systems Implementation Prompts**

*Based on analysis from needsrepair.instruction.md identifying missing core systems*

### **Phase 1: Foundation Systems**
- **[Implement Result Pattern System](CustomPrompt_Implement_ResultPatternSystem.md)** - For creating the Result<T> pattern infrastructure for consistent service responses
- **[Create Data Models Foundation](CustomPrompt_Create_DataModelsFoundation.md)** - For generating the complete Models namespace with MTM-specific data entities
- **[Setup Dependency Injection Container](CustomPrompt_Setup_DependencyInjectionContainer.md)** - For configuring Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs
- **[Create Core Service Interfaces](CustomPrompt_Create_CoreServiceInterfaces.md)** - For generating all essential service interfaces following MTM patterns

### **Phase 2: Service Layer Implementation**
- **[Implement Service Layer](CustomPrompt_Implement_ServiceLayer.md)** - For creating complete service implementations with MTM patterns and error handling
- **[Create Database Service Layer](CustomPrompt_Create_DatabaseServiceLayer.md)** - For implementing centralized database access with proper connection management
- **[Setup Application State Management](CustomPrompt_Setup_ApplicationStateManagement.md)** - For creating global application state service with proper encapsulation
- **[Implement Configuration Service](CustomPrompt_Implement_ConfigurationService.md)** - For creating service to read and manage appsettings.json configuration

### **Phase 3: Infrastructure Systems**
- **[Create Navigation Service](CustomPrompt_Create_NavigationService.md)** - For implementing proper MVVM navigation patterns with view-viewmodel mapping
- **[Implement Theme System](CustomPrompt_Implement_ThemeSystem.md)** - For creating MTM purple brand theme resources and DynamicResource patterns
- **[Setup Repository Pattern](CustomPrompt_Setup_RepositoryPattern.md)** - For implementing data access abstraction layer with repository interfaces
- **[Create Validation System](CustomPrompt_Create_ValidationSystem.md)** - For implementing business rule validation with MTM-specific patterns

### **Phase 4: Quality Assurance Systems**
- **[Create Unit Testing Infrastructure](CustomPrompt_Create_UnitTestingInfrastructure.md)** - For setting up comprehensive testing framework with mocks and fixtures
- **[Implement Structured Logging](CustomPrompt_Implement_StructuredLogging.md)** - For adding centralized logging throughout the application with proper levels
- **[Create Caching Layer](CustomPrompt_Create_CachingLayer.md)** - For implementing performance-oriented caching for ComboBox data and user preferences
- **[Setup Security Infrastructure](CustomPrompt_Setup_SecurityInfrastructure.md)** - For implementing authentication, authorization, and secure connection management

---

## ?? **Rules for Managing Custom Prompts**

### **Rule 1: File Creation and Organization**
- **Whenever a new custom prompt is added to this master index, a corresponding detailed implementation file must be created in `.github/Custom-Prompts/` folder using the naming format `CustomPrompt_{Action}_{Where}.md`.**
- **Each prompt file must include the complete template structure: Instructions, Persona, Prompt Template, Purpose, Usage Examples, Guidelines, Related Files, and Quality Checklist.**
- **All prompts must reference appropriate personas from [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md).**

### **Rule 2: HTML Documentation Synchronization** ? **NEW**
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
1. **Create/Edit Prompt**: Make changes to prompt files in `.github/Custom-Prompts/`
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

### **Rule 3: Quality and Compliance**
- All custom prompt files must follow MTM coding conventions and instruction guidelines
- Each prompt must include proper error handling patterns
- Reference organized instruction files appropriately
- Maintain consistency with existing documentation structure

---

## ?? **Usage Guidelines**

### **How to Use Custom Prompts**

1. **Navigate to Specific Prompt**: Click on the prompt link to access the detailed implementation file
2. **Review Persona Requirements**: Check the assigned persona and behavioral guidelines
3. **Copy Template**: Use the provided prompt template as-is or customize for your specific needs
4. **Follow Guidelines**: Adhere to the technical requirements and MTM-specific patterns
5. **Check Examples**: Review usage examples for context and proper implementation
6. **Update HTML Documentation**: If creating/editing prompts, update corresponding HTML files

### **Prompt File Structure**

Each custom prompt file in `.github/Custom-Prompts/` contains:
- **Instructions**: Clear guidance on when and how to use the prompt
- **Persona**: Assigned Copilot persona with behavioral guidelines
- **Prompt Template**: Copy-paste ready prompt text
- **Purpose**: Clear explanation of what the prompt accomplishes
- **Usage Examples**: Real-world scenarios and example implementations
- **Guidelines**: Technical requirements and MTM-specific patterns
- **Related Files**: Cross-references to related documentation
- **Quality Checklist**: Verification points for successful implementation

---

## ?? **MTM-Specific Workflow Patterns**

### **UI Generation from Markdown Files**
When working with markdown instruction files that describe UI components:

1. **Parse Component Hierarchy** - Extract the tree structure and convert to Avalonia AXAML
2. **Map MTM Data Types** - Use Part ID (string), Operation (string numbers), Quantity (integer)
3. **Apply Context Menu Patterns** - Add management features via right-click menus
4. **Implement Space Optimization** - Use UniformGrid and proper alignment for component removal
5. **Follow MTM Color Scheme** - Apply purple brand colors consistently
6. **Create ReactiveUI Bindings** - Generate ViewModels with proper observable patterns

### **Quality Assurance Workflow**
For comprehensive code compliance verification:

1. **Initial Assessment** - Review code files against all instruction guidelines
2. **Violation Identification** - Document specific non-compliance issues with examples
3. **Priority Classification** - Categorize issues as High/Medium/Low priority
4. **Fix Recommendations** - Provide specific guidance for bringing code into compliance
5. **Report Generation** - Update needsrepair.instruction.md with structured findings
6. **Re-verification** - Follow up reviews to confirm compliance improvements

### **Missing Systems Implementation Workflow**
For implementing missing core systems identified in compliance analysis:

1. **Foundation Phase** - Implement data models, Result pattern, and DI container
2. **Service Layer Phase** - Create all service interfaces and implementations
3. **Infrastructure Phase** - Add navigation, themes, repositories, and validation
4. **Quality Assurance Phase** - Implement testing, logging, caching, and security

### **Instruction File Synchronization**
For keeping instruction files synchronized with the main copilot-instructions.md:

**Pattern:** `I have the main copilot-instructions.md file loaded with [context description]. Update [target-file.md] with any missing [specific content type] from the main file. Focus only on [scope] and [behavioral guidelines].`

**Available Synchronization Targets:**
- `codingconventions.instruction.md` - Coding standards, ReactiveUI patterns, MVVM guidelines
- `ui-generation.instruction.md` - AXAML templates, layout patterns, MTM design guidelines  
- `errorhandler.instruction.md` - ReactiveUI error patterns, Avalonia error display
- `naming.conventions.instruction.md` - File naming, ViewModel naming, service naming
- `personas.instruction.md` - Copilot personas, behavioral guidelines
- `ui-mapping.instruction.md` - Control mappings, screenshot relationships
- `needsrepair.instruction.md` - Quality assurance patterns, compliance tracking

---

## ?? **Persona Behavioral Guidelines**

### **MTM-Specific Behaviors**
All personas should follow these MTM-specific behavioral patterns:

- **Operations are Numbers**: Always treat operations as string numbers ("90", "100", "110"), not action descriptions
- **Part ID Format**: Use string format for Part IDs (e.g., "PART001")
- **1-Based Indexing**: UI positions use 1-based indexing for display
- **Purple Brand Consistency**: Apply MTM purple color scheme (#4B45ED primary, #BA45ED accents)
- **ReactiveUI Patterns**: Use ReactiveObject, RaiseAndSetIfChanged, ReactiveCommand, WhenAnyValue
- **Modern Layout Principles**: Prefer Grid over StackPanel, use cards and sidebars
- **Context Menu Management**: Add right-click functionality for component management
- **Business Logic Separation**: Leave implementation as TODO comments, focus on structure

### **Avalonia-Specific Behaviors**
- **AXAML Generation**: Use proper xmlns declarations and compiled bindings
- **Control Mapping**: Convert WinForms controls to Avalonia equivalents
- **Theme Resources**: Use DynamicResource for colors to support theming
- **Modern UI Elements**: Implement cards, shadows, gradients, and proper spacing

### **Quality Assurance Behaviors**
- **Comprehensive Review**: Check all aspects of code against instruction guidelines
- **Structured Reporting**: Use standardized format for violation documentation
- **Priority Assignment**: Classify issues based on impact and compliance severity
- **Solution-Oriented**: Provide specific fix recommendations with instruction references
- **Progress Tracking**: Update compliance status and maintain audit trail

### **Missing Systems Implementation Behaviors**
- **Foundation First**: Always implement foundational systems before dependent systems
- **Clean Architecture**: Follow dependency inversion and separation of concerns
- **Error Handling**: Integrate Service_ErrorHandler in all new implementations
- **Testing Support**: Include testability considerations in all implementations
- **Documentation**: Provide comprehensive XML documentation for all public APIs
- **DI Preparation**: Ensure all services are prepared for dependency injection

---

## ?? **Integration with Development Workflow**

### **Pre-Implementation Verification**
**CRITICAL**: Before continuing with any business logic or UI development:
1. **Implement missing core systems** identified in analysis
2. **Generate compliance reports** for all new systems
3. **Achieve minimum 80% compliance** on foundational systems

### **Pre-Commit Verification**
Use compliance auditing as part of code review process:
1. Generate compliance report for changed files
2. Address critical and high-priority violations
3. Include compliance status in pull request review
4. **Verify no new missing systems introduced**

### **Continuous Compliance**
- Schedule regular compliance audits
- Monitor compliance trends
- Update instruction files based on common violations
- **Track missing systems implementation progress**

### **Team Collaboration**
- Share compliance reports with team members
- Use custom fix prompts for consistent remediation
- Maintain project-wide compliance standards
- **Coordinate core systems implementation** across team

---

*This master index provides centralized access to all custom prompts for the MTM WIP Application Avalonia project. Each prompt is designed to maintain consistency with MTM standards and accelerate development while ensuring quality and compliance.*