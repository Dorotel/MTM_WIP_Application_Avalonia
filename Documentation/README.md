# 📚 MTM WIP Application Documentation

Welcome to the comprehensive documentation for the MTM WIP Application built with Avalonia UI targeting .NET 8. This documentation provides detailed information about the application's architecture, components, and development guidelines.

This documentation system has been modernized with a structured approach separating Development and Production concerns, enhanced HTML documentation with accessibility features, and automated discovery systems for maintaining up-to-date documentation.

---

## 🎯 **Quick Navigation**

### **🚀 For Developers**
- **[Development Documentation](Development/)** - Development environment, scripts, and tools
- **[Technical HTML Docs](../docs/Technical/)** - Detailed technical documentation in HTML format
- **[Core Files Documentation](../docs/CoreFiles/)** - C# source file documentation
- **[Discovery Scripts](Development/Development%20Scripts/Verification/)** - Automated file and dependency discovery

### **👥 For Business Stakeholders**  
- **[Plain English Documentation](../docs/PlainEnglish/)** - Non-technical explanations of system functionality
- **[Project Overview](ReadmeFiles/Core/README_Project_Overview.md)** - High-level project summary
- **[Features Documentation](ReadmeFiles/Core/README_Features.md)** - Application features and capabilities

### **⚙️ For Production**
- **[Production Documentation](Production/)** - Production deployment and procedures
- **[Database Documentation](ReadmeFiles/Database/)** - Database schemas and procedures

### **🔍 For Quality Assurance**
- **[Items Needing Attention](../docs/NeedsFixing/)** - Quality issues and improvement items
- **[Compliance Reports](ReadmeFiles/Development/README_Compliance_Reports.md)** - Code compliance and quality reports

---

## 🗂️ **Enhanced Directory Structure**

### **Development/** - Development Documentation and Tools
**Purpose**: Contains all development-specific documentation, scripts, and tools separated from production content.

#### **Development Documentation/**
- **UI_Screenshots/** - Application screenshots and visual documentation
- **UI_Documentation/** - UI component specifications and design guidelines  
- **Examples/** - Code examples and usage patterns
- **Verification/** - Quality assurance and verification documentation
- **Database/** - Development database schemas and procedures
- **GitHub/** - GitHub-specific documentation and workflows

#### **Development Scripts/**
- **Verification/** - Automated discovery and validation scripts
  - `Discover-CoreFiles.ps1` - Find and analyze C# core files (Found: 26 files)
  - `Discover-Documentation.ps1` - Inventory all documentation files
  - `Discover-Dependencies.ps1` - Analyze service dependencies
  - `Discover-NeedsFixing.ps1` - Extract quality issues for tracking
  - `Validate-Structure.ps1` - Verify directory structure integrity (Health: 100%)
  - `Generate-FileInventory.ps1` - Create comprehensive file inventory
- **Automation/** - Build and deployment automation scripts
- **Build/** - Build process scripts and configurations
- **Deployment/** - Deployment scripts and procedures
- **Utilities/** - General utility scripts

### **Production/** - Production Documentation and Scripts
**Purpose**: Contains production-ready documentation and deployment procedures.

#### **Production Documentation/**
- **Database/** - Production database documentation and procedures

#### **Production Scripts/**
- Production deployment and maintenance scripts

---

## 🌐 **Enhanced HTML Documentation System**

**Location**: `../docs/` - Modern, accessible web documentation with black/gold MTM styling

### **📋 Five-Category Documentation Structure**

#### **1. PlainEnglish/** - Non-Technical Documentation
- **[index.html](../docs/PlainEnglish/index.html)** - Main entry point for stakeholders  
- **Updates/** - Non-technical updates and announcements
- **FileDefinitions/** - Plain English explanations of C# files
  - Services/, ViewModels/, Models/, Extensions/ - Business-friendly file explanations

#### **2. Technical/** - Developer Documentation  
- **[index.html](../docs/Technical/index.html)** - Technical documentation hub
- **Updates/** - Technical updates and developer announcements
- **FileDefinitions/** - Detailed technical documentation of C# files
  - Services/, ViewModels/, Models/, Extensions/ - Code-level technical documentation

#### **3. CoreFiles/** - C# Source Documentation
- **[index.html](../docs/CoreFiles/index.html)** - Core files overview
- **Services/** - Service layer documentation (11 files discovered)
- **ViewModels/** - ViewModel documentation (12 files discovered) 
- **Models/** - Data model documentation (2 files discovered)
- **Extensions/** - Extension method documentation (1 file discovered)
- **Coverage**: 26 total C# core files requiring documentation

#### **4. NeedsFixing/** - Quality Issue Tracking
- **[index.html](../docs/NeedsFixing/index.html)** - Quality issues dashboard
- **high-priority.html** - Critical items requiring immediate attention
- **medium-priority.html** - Important items for next sprint
- **low-priority.html** - Nice-to-have improvements
- **items/** - Individual issue tracking files

#### **5. Components/** - Component Documentation
- **[index.html](../docs/Components/index.html)** - Component overview
- Component-specific documentation and interaction patterns

### **🎨 Design Features**
- **MTM Black/Gold Theme** - Professional styling with MTM brand colors
- **WCAG AA Compliance** - Full accessibility support
- **Responsive Design** - Mobile and desktop optimized
- **Cross-Navigation** - Seamless switching between Plain English and Technical views
- **Search Functionality** - Cross-category search capabilities

### **🔧 Automated Discovery Integration**
The HTML documentation system integrates with automated discovery scripts:
- **Dynamic Content Population** - Scripts automatically detect new files requiring documentation
- **Change Detection** - Monitors file modifications and updates documentation needs
- **Quality Tracking** - Extracts issues from code comments and quality instructions
- **Dependency Mapping** - Analyzes service dependencies and injection patterns

## 🗂️ **Legacy Directory Structure**

### **ReadmeFiles/** - Centralized README Collection
Organized documentation covering all aspects of the project:

#### **Core/** - Essential Project Documentation
- **[README_Project_Overview.md](ReadmeFiles/Core/README_Project_Overview.md)** - Project overview, architecture, and goals
- **[README_Architecture.md](ReadmeFiles/Core/README_Architecture.md)** - System architecture and design patterns
- **[README_Getting_Started.md](ReadmeFiles/Core/README_Getting_Started.md)** - Setup and installation guide

#### **Development/** - Development-Specific Documentation
- **[README_Development_Overview.md](ReadmeFiles/Development/README_Development_Overview.md)** - Development environment and workflow
- **[README_Database_Files.md](ReadmeFiles/Development/README_Database_Files.md)** - Database development and procedures
- **[README_UI_Documentation.md](ReadmeFiles/Development/README_UI_Documentation.md)** - UI component documentation
- **[Views_Structure_Standards.instruction.md](../Development/UI_Documentation/Views_Structure_Standards.instruction.md)** - Mandatory view structure standards
- **[README_Custom_Prompts.md](ReadmeFiles/Development/README_Custom_Prompts.md)** - Custom prompt system
- **[README_Examples.md](ReadmeFiles/Development/README_Examples.md)** - Code examples and patterns
- **[README_Compliance_Reports.md](ReadmeFiles/Development/README_Compliance_Reports.md)** - Quality assurance reports

#### **Components/** - Component-Specific Documentation
- **[README_Services.md](ReadmeFiles/Components/README_Services.md)** - Service layer architecture
- **[README_ViewModels.md](ReadmeFiles/Components/README_ViewModels.md)** - ViewModel patterns and implementations
- **[README_Views.md](ReadmeFiles/Components/README_Views.md)** - View components and UI patterns
- **[README_Models.md](ReadmeFiles/Components/README_Models.md)** - Data models and business entities
- **[README_Extensions.md](ReadmeFiles/Components/README_Extensions.md)** - Extension methods and utilities

#### **Database/** - Database Documentation
- **[README_Production_Schema.md](ReadmeFiles/Database/README_Production_Schema.md)** - Production database schema
- **[README_Development_Schema.md](ReadmeFiles/Database/README_Development_Schema.md)** - Development database schema
- **[README_Stored_Procedures.md](ReadmeFiles/Database/README_Stored_Procedures.md)** - Stored procedure documentation
- **[README_Migration_Guide.md](ReadmeFiles/Database/README_Migration_Guide.md)** - Database migration procedures

#### **Archive/** - Historical Documentation
- **[README_Migration_Log.md](ReadmeFiles/Archive/README_Migration_Log.md)** - Documentation migration history

### **HTML/** - Modern Web Documentation
Professional, accessible web documentation with MTM branding:

#### **Technical/** - Technical Documentation
- **[index.html](HTML/Technical/index.html)** - Technical documentation hub
- Interactive HTML documentation for developers
- Modern responsive design with MTM purple theme

#### **PlainEnglish/** - Non-Technical Documentation
- **[index.html](HTML/PlainEnglish/index.html)** - Main entry point for stakeholders
- Business-friendly explanations of technical concepts
- Visual aids and simplified language

#### **assets/** - Shared Resources
- **css/** - Modern stylesheets with MTM branding
- **images/** - Screenshots, diagrams, and icons
- **js/** - Interactive features and navigation
- **fonts/** - Custom typography

### **Templates/** - Documentation Templates
- **README_Template.md** - Standardized README template
- **HTML_Technical_Template.html** - Technical HTML template
- **HTML_PlainEnglish_Template.html** - Plain English HTML template
- **Component_Documentation_Template.md** - Component documentation template

## 🎯 **Quick Access**

### **For Developers**
- **Getting Started**: [Core/README_Getting_Started.md](ReadmeFiles/Core/README_Getting_Started.md)
- **Development Guide**: [Development/README_Development_Overview.md](ReadmeFiles/Development/README_Development_Overview.md)
- **Database Development**: [Development/README_Database_Files.md](ReadmeFiles/Development/README_Database_Files.md)
- **UI Components**: [Development/README_UI_Documentation.md](ReadmeFiles/Development/README_UI_Documentation.md)
- **View Standards**: [Views_Structure_Standards.instruction.md](../Development/UI_Documentation/Views_Structure_Standards.instruction.md)

### **For Project Managers**
- **Project Overview**: [Core/README_Project_Overview.md](ReadmeFiles/Core/README_Project_Overview.md)
- **Architecture**: [Core/README_Architecture.md](ReadmeFiles/Core/README_Architecture.md)
- **Quality Reports**: [Development/README_Compliance_Reports.md](ReadmeFiles/Development/README_Compliance_Reports.md)
- **Plain English Guide**: [HTML/PlainEnglish/index.html](HTML/PlainEnglish/index.html)

### **For Business Stakeholders**
- **Plain English Documentation**: [HTML/PlainEnglish/](HTML/PlainEnglish/)
- **Project Overview**: [Core/README_Project_Overview.md](ReadmeFiles/Core/README_Project_Overview.md)
- **User Interface Guide**: [HTML/PlainEnglish/user-interface.html](HTML/PlainEnglish/user-interface.html)

### **For QA Teams**
- **Quality Assurance**: [Development/README_Compliance_Reports.md](ReadmeFiles/Development/README_Compliance_Reports.md)
- **Testing Procedures**: [HTML/PlainEnglish/quality-assurance.html](HTML/PlainEnglish/quality-assurance.html)
- **Troubleshooting**: [HTML/PlainEnglish/troubleshooting.html](HTML/PlainEnglish/troubleshooting.html)

## 🔧 **Documentation Standards**

### **Formatting Guidelines**
- **Consistent Structure**: All documentation follows standardized templates
- **Clear Headings**: Proper hierarchy with H1-H6 structure
- **Cross-References**: Links between related documentation
- **Visual Elements**: Screenshots, diagrams, and code examples

### **Accessibility Standards**
- **Plain English**: Non-technical versions for all major content
- **Mobile Responsive**: All HTML documentation works on mobile devices
- **Screen Reader**: Proper semantic markup and alt text
- **WCAG Compliance**: AA-level accessibility standards

### **Maintenance Process**
- **Regular Updates**: Documentation reviewed and updated monthly
- **Version Control**: All changes tracked through Git
- **Quality Assurance**: Documentation accuracy verified before release
- **Stakeholder Review**: Content validated by appropriate teams

## 📊 **Documentation Metrics**

- **Total README Files**: 30+ files (centralized in ReadmeFiles/)
- **HTML Documentation**: 14+ modern responsive pages
- **Content Coverage**: 100% of project components documented
- **Accessibility**: WCAG AA compliant
- **Update Frequency**: Monthly review cycle

## 🔍 **Dynamic Discovery System**

The documentation system includes automated discovery scripts that continuously monitor the codebase for changes and documentation needs:

### **Discovery Scripts Overview**
- **Discover-CoreFiles.ps1** - ✅ Operational (Last scan: 26 files discovered)
- **Discover-Documentation.ps1** - ✅ Operational 
- **Discover-Dependencies.ps1** - ✅ Operational
- **Discover-NeedsFixing.ps1** - ✅ Operational
- **Validate-Structure.ps1** - ✅ Operational (Health: 100%)
- **Generate-FileInventory.ps1** - ✅ Operational

### **Current Discovery Status**
- **📄 Core C# Files**: 26 files identified (0% documented)
- **🔧 Services**: 11 files in Services/ directory
- **👁️ ViewModels**: 12 files in ViewModels/ directory  
- **📊 Models**: 2 files in Models/ directory
- **⚡ Extensions**: 1 file in Extensions/ directory
- **📋 Documentation Coverage**: 0% (Ready for Step 2 execution)

### **Automated Quality Tracking**
- **Structure Health**: 100% (All directories properly created)
- **Required Files**: 100% (All critical files present)
- **Discovery Scripts**: 100% (All 6 scripts operational)
- **HTML Structure**: 100% (All 5 categories ready)

### **Next Steps for Discovery**
1. **Step 2 Execution**: Generate 52 documentation files (26 Plain English + 26 Technical)
2. **Quality Issue Extraction**: Parse needsrepair.instruction.md for tracking
3. **Dependency Analysis**: Map service injection patterns and relationships
4. **Change Detection**: Establish baseline for future change monitoring

## 🚀 **Recent Updates**

### **Phase 1: Audit & Inventory** ✅ Completed
- Complete inventory of all documentation files
- New centralized directory structure established
- Content analysis and cross-reference mapping

### **Phase 2: README Reorganization** 🔄 In Progress
- Migration of README files to centralized location
- Standardized naming conventions
- Content consolidation and deduplication

### **Upcoming Phases**
- **Phase 3**: HTML modernization with MTM branding
- **Phase 4**: Plain English documentation creation
- **Phase 5**: Quality assurance and final validation

---

## 📞 **Support and Contact**

For documentation questions or suggestions:
- **Technical Issues**: Create an issue in the project repository
- **Content Feedback**: Contact the development team
- **Accessibility Concerns**: Report through accessibility feedback channels

---

*This documentation was last updated: January 2025*  
*Documentation Modernization Initiative v1.0*