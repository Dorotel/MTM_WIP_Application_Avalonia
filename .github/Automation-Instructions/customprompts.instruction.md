<!-- Copilot: Reading custom-prompts.instruction.md — Custom Prompts and Copilot Personas -->

# Custom Copilot Prompts - Master Index

This file serves as the **master index** for all custom prompt types used in the MTM WIP Application Avalonia project. Each prompt type has a detailed implementation file in the `Documentation/Development/Custom_Prompts/` folder with complete usage examples, guidelines, and technical requirements.

> **See [personas.instruction.md](personas.instruction.md) for complete persona definitions and behavioral guidelines.**

---

## 📁 **Folder Organization**

### **Custom Prompt Files Location**
All custom prompt implementation files are located in `Documentation/Development/Custom_Prompts/` with the naming format `CustomPrompt_{Action}_{Where}.md`.

### **HTML Documentation Integration**
Each custom prompt must maintain corresponding HTML documentation files in:
- `Documentation/HTML/PlainEnglish/custom-prompts.html` - Business-friendly explanations
- `Documentation/HTML/Technical/custom-prompts.html` - Developer-focused documentation

---

## 📋 **Quick Reference - Prompt Types**

### **UI Generation and Development**
- **[Create UI Element](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElement.md)** - For generating Avalonia controls or views based on mapping and instructions
- **[Create UI Element from Markdown Instructions](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElementFromMarkdown.md)** - For generating Avalonia AXAML and ReactiveUI ViewModels from parsed markdown files
- **[Create ReactiveUI ViewModel](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ReactiveUIViewModel.md)** - For generating ViewModels with ReactiveUI patterns, commands, and observable properties
- **[Create Modern Layout Pattern](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ModernLayoutPattern.md)** - For generating modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns
- **[Create Context Menu Integration](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ContextMenuIntegration.md)** - For adding context menus to components with management features following MTM patterns
- **[Create Avalonia Theme Resources](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_AvaloniaThemeResources.md)** - For generating MTM-specific color schemes and theme resources using the purple brand palette

### **Error Handling and Logging**
- **[Create Error System Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ErrorSystemPlaceholder.md)** - For scaffolding error handling classes with standard conventions
- **[Create Logging Info Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_LoggingInfoPlaceholder.md)** - For generating logging helpers and configuration according to project standards
- **[Create UI Element for Error Messages](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElementForErrorMessages.md)** - For generating error message UI components with MTM theme integration

### **Business Logic and Data Access**
- **[Create Business Logic Handler](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_BusinessLogicHandler.md)** - For scaffolding business logic classes with proper naming and separation from UI
- **[Create Database Access Layer Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DatabaseAccessLayerPlaceholder.md)** - For generating database interaction classes or repositories
- **[Create MTM Data Model](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_MTMDataModel.md)** - For generating models that follow MTM-specific data patterns

### **Testing and Configuration**
- **[Create Pull Request](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_Issue.md)** - Conducts comprehensive code quality reviews against all MTM WIP Application instruction guidelines. Specializes in identifying compliance violations, generating detailed audit reports, and providing specific remediation guidance. Expert in maintaining code quality standards and tracking compliance improvements over time.
- **[Create Unit Test Skeleton](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UnitTestSkeleton.md)** - For generating basic unit test classes with appropriate structure
- **[Create Configuration File Placeholder](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ConfigurationFilePlaceholder.md)** - For generating config/settings files with standard project structure

### **Code Quality and Maintenance**
- **[Verify Code Compliance](../../Documentation/Development/Custom_Prompts/CustomPrompt_Verify_CodeCompliance.md)** - For conducting comprehensive quality assurance reviews against MTM instruction guidelines
- **[Refactor Code to Naming Convention](../../Documentation/Development/Custom_Prompts/CustomPrompt_Refactor_CodeToNamingConvention.md)** - For requesting code element renaming to fit project naming rules
- **[Add Event Handler Stub](../../Documentation/Development/Custom_Prompts/CustomPrompt_Add_EventHandlerStub.md)** - For generating empty event handler methods with TODOs for specific controls

### **Documentation and Project Management**
- **[Document Public API/Class](../../Documentation/Development/Custom_Prompts/CustomPrompt_Document_PublicAPIClass.md)** - For generating XML documentation for public members
- **[Create Customized Prompt](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CustomizedPrompt.md)** - For drafting new actionable custom prompts and assigning appropriate personas
- **[Update Instruction File from Master](../../Documentation/Development/Custom_Prompts/CustomPrompt_Update_InstructionFileFromMaster.md)** - For synchronizing instruction files with relevant content from the main copilot-instructions.md

---

## 🏗️ **Missing Core Systems Implementation Prompts**

*Based on analysis from needsrepair.instruction.md identifying missing core systems*

### **Phase 1: Foundation Systems**
- **[Implement Result Pattern System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ResultPatternSystem.md)** - For creating the Result<T> pattern infrastructure for consistent service responses
- **[Create Data Models Foundation](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DataModelsFoundation.md)** - For generating the complete Models namespace with MTM-specific data entities
- **[Setup Dependency Injection Container](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_DependencyInjectionContainer.md)** - For configuring Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs
- **[Create Core Service Interfaces](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CoreServiceInterfaces.md)** - For generating all essential service interfaces following MTM patterns

### **Phase 2: Service Layer Implementation**
- **[Implement Service Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ServiceLayer.md)** - For creating complete service implementations with MTM patterns and error handling
- **[Create Database Service Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_DatabaseServiceLayer.md)** - For implementing centralized database access with proper connection management
- **[Setup Application State Management](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_ApplicationStateManagement.md)** - For creating global application state service with proper encapsulation
- **[Implement Configuration Service](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ConfigurationService.md)** - For creating service to read and manage appsettings.json configuration

### **Phase 3: Infrastructure Systems**
- **[Create Navigation Service](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_NavigationService.md)** - For implementing proper MVVM navigation patterns with view-viewmodel mapping
- **[Implement Theme System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_ThemeSystem.md)** - For creating MTM purple brand theme resources and DynamicResource patterns
- **[Setup Repository Pattern](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_RepositoryPattern.md)** - For implementing data access abstraction layer with repository interfaces
- **[Create Validation System](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_ValidationSystem.md)** - For implementing business rule validation with MTM-specific patterns

### **Phase 4: Quality Assurance Systems**
- **[Create Unit Testing Infrastructure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_UnitTestingInfrastructure.md)** - For setting up comprehensive testing framework with mocks and fixtures
- **[Implement Structured Logging](../../Documentation/Development/Custom_Prompts/CustomPrompt_Implement_StructuredLogging.md)** - For adding centralized logging throughout the application with proper levels
- **[Create Caching Layer](../../Documentation/Development/Custom_Prompts/CustomPrompt_Create_CachingLayer.md)** - For implementing performance-oriented caching for ComboBox data and user preferences
- **[Setup Security Infrastructure](../../Documentation/Development/Custom_Prompts/CustomPrompt_Setup_SecurityInfrastructure.md)** - For implementing authentication, authorization, and secure connection management

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

### **Rule 3: Quality and Compliance**
- All custom prompt files must follow MTM coding conventions and instruction guidelines
- Each prompt must include proper error handling patterns
- Reference organized instruction files appropriately
- Maintain consistency with existing documentation structure

---

## 🎯 **Usage Guidelines**

For detailed usage guidelines, MTM-specific workflow patterns, persona behavioral guidelines, and integration with development workflow, see the comprehensive guide in [Documentation/Development/Custom_Prompts/README.md](../../Documentation/Development/Custom_Prompts/README.md).

### **Quick Usage Steps**
1. **Navigate to Specific Prompt**: Click on the prompt link to access the detailed implementation file
2. **Review Persona Requirements**: Check the assigned persona and behavioral guidelines
3. **Copy Template**: Use the provided prompt template as-is or customize for your specific needs
4. **Follow Guidelines**: Adhere to the technical requirements and MTM-specific patterns
5. **Check Examples**: Review usage examples for context and proper implementation
6. **Update HTML Documentation**: If creating/editing prompts, update corresponding HTML files

---

*This master index provides centralized access to all custom prompts for the MTM WIP Application Avalonia project. Each prompt is designed to maintain consistency with MTM standards and accelerate development while ensuring quality and compliance.*