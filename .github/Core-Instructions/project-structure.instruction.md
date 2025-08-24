# Project Structure Guidelines

## **Repository Organization**

### **Root Level Structure**
```
/
??? Views/                  # Avalonia AXAML views
??? ViewModels/            # ViewModels using ReactiveUI
??? Models/                # Data models and business entities
??? Services/              # Business logic and data access services
??? Resources/             # Styles, themes, and assets
??? Database_Files/        # ?? PRODUCTION database schema and stored procedures (READ-ONLY)
??? Development/           # Development-specific files and documentation
??? Documentation/         # Project documentation and updates
??? .github/              # GitHub workflows and instruction files
```

### **Production Database Files (READ-ONLY)**
```
Database_Files/
??? Production_Database_Schema.sql       # Current production schema
??? Existing_Stored_Procedures.sql      # READ-ONLY production procedures
??? README_*.md                          # Production documentation
```

**?? CRITICAL: These files are READ-ONLY**
- Never modify files in `Database_Files/` directly
- Use `Development/Database_Files/` for new development work

### **Documentation Directory Structure**
```
Documentation/
??? Development/           # Development-focused documentation
?   ??? Updates/          # ?? SYSTEM UPDATE SUMMARIES AND CHANGE LOGS
?   ?   ??? Error-Handling-Upgrade-Summary.md
?   ?   ??? Service-Layer-Enhancement-Summary.md
?   ?   ??? Database-Integration-Summary.md
?   ?   ??? [Feature]-[Component]-Summary.md
?   ??? Compliance Reports/ # ?? COMPLIANCE ANALYSIS AND QUALITY REPORTS
?   ?   ??? Views/        # UI/View compliance reports
?   ?   ?   ??? Views_Compliance_Report.md
?   ?   ?   ??? Modern_Layout_Compliance.md
?   ?   ?   ??? UI_Standards_Analysis.md
?   ?   ??? Services/     # Service layer compliance reports
?   ?   ?   ??? Service_Architecture_Compliance.md
?   ?   ?   ??? DI_Container_Compliance.md
?   ?   ?   ??? Error_Handling_Compliance.md
?   ?   ??? Database/     # Database compliance reports
?   ?   ?   ??? Schema_Compliance_Report.md
?   ?   ?   ??? StoredProcedure_Standards_Report.md
?   ?   ?   ??? Data_Access_Compliance.md
?   ?   ??? Overall/      # System-wide compliance reports
?   ?       ??? Architecture_Compliance_Summary.md
?   ?       ??? Code_Quality_Report.md
?   ?       ??? Standards_Adherence_Report.md
?   ??? Custom_Prompts/   # ?? CUSTOM DEVELOPMENT PROMPTS AND AUTOMATION
?   ?   ??? CustomPrompt_Create_UIElement.md
?   ?   ??? CustomPrompt_Create_ReactiveUIViewModel.md
?   ?   ??? CustomPrompt_Verify_CodeCompliance.md
?   ?   ??? Compliance_Fix01_EmptyDevelopmentStoredProcedures.md
?   ?   ??? [Additional CustomPrompt files...]
?   ??? Issues/           # Development issue tracking and resolution
?   ??? README.md         # Development documentation overview
??? Production/           # Production-ready documentation
??? ReadmeFiles/          # Organized README documentation
??? HTML/                 # Web-based documentation
??? Templates/            # Documentation templates
```

**?? Documentation/Development/Updates Guidelines:**
- **Purpose**: Comprehensive summaries of completed system upgrades, enhancements, and major changes
- **Naming Pattern**: `[Feature]-[Component]-Summary.md` (e.g., `Error-Handling-Upgrade-Summary.md`)
- **Content**: Before/after comparisons, implementation details, migration guides, benefits analysis
- **When to Create**: After completing major system upgrades, feature implementations, or architectural changes
- **Required Sections**: Overview, completed changes, architecture improvements, benefits, integration checklists, usage migration guides

**?? Documentation/Development/Compliance Reports Guidelines:**
- **Purpose**: Quality assurance analysis, standards compliance verification, and system health reports
- **Structure**: Organized by system component (Views, Services, Database, Overall)
- **Naming Pattern**: `[Component]_Compliance_Report.md` or `[Standard]_[Area]_Analysis.md`
- **Content**: Compliance scores, detailed analysis, issue identification, recommendations, action plans
- **When to Create**: After compliance audits, quality assessments, standards reviews, or system evaluations
- **Required Sections**: Executive summary, analysis results, compliance scores, critical issues, recommendations, action plans

**?? Documentation/Development/Custom_Prompts Guidelines:**
- **Purpose**: Specialized GitHub Copilot prompts for development automation and consistency
- **Structure**: Organized by prompt type and complexity level
- **Naming Pattern**: `CustomPrompt_{Action}_{Where}.md` (e.g., `CustomPrompt_Create_UIElement.md`)
- **Content**: Prompt templates, persona assignments, usage examples, technical requirements
- **When to Create**: For recurring development tasks requiring standardized output and MTM compliance
- **Required Sections**: Instructions, persona, prompt template, purpose, usage examples, guidelines, quality checklist

### **Development Directory Structure**
```
Development/
??? Database_Files/          # ?? Development database files (EDITABLE)
?   ??? Development_Database_Schema.sql   # Development schema changes
?   ??? New_Stored_Procedures.sql        # New procedures for development
?   ??? Updated_Stored_Procedures.sql    # Modified existing procedures
?   ??? README_*.md                      # Development documentation
??? UI_Documentation/        # UI component instruction files
??? Examples/               # Code examples and usage patterns
??? Docs/                  # Development documentation
??? UI_Screenshots/        # UI design screenshots and mockups
```

**?? IMPORTANT**: Custom prompts and compliance reports have been moved to `Documentation/Development/` for better organization and consistency with the overall documentation structure.

## **File Placement Guidelines**

### **New Components**
- **Views**: Place in `Views/` folder with `.axaml` extension
- **ViewModels**: Place in `ViewModels/` folder with `ViewModel.cs` suffix
- **Services**: Place in `Services/` folder with `Service.cs` suffix
- **Models**: Place in `Models/` folder with descriptive names

### **Documentation**
- **Component-specific instructions**: `Development/UI_Documentation/`
- **Code examples**: `Development/Examples/`
- **API documentation**: `Development/Docs/`
- **Visual references**: `Development/UI_Screenshots/`
- **?? System upgrade summaries**: `Documentation/Development/Updates/`
- **Feature enhancement documentation**: `Documentation/Development/Updates/`
- **Change logs and migration guides**: `Documentation/Development/Updates/`
- **?? Compliance reports and quality analysis**: `Documentation/Development/Compliance Reports/[Component]/`
- **?? Custom development prompts**: `Documentation/Development/Custom_Prompts/`

### **Documentation Update Files (MANDATORY PLACEMENT)**
**Location**: `Documentation/Development/Updates/`

**File Types That MUST Go Here**:
- System upgrade summaries (e.g., `Error-Handling-Upgrade-Summary.md`)
- Service layer enhancement documentation
- Database integration summaries
- Feature implementation summaries
- Architecture change documentation
- Migration guides for major updates
- Before/after analysis documents
- Integration checklists and compliance reports

**Naming Convention**: `[System/Feature]-[Component/Area]-Summary.md`

**Examples**:
- `Error-Handling-Upgrade-Summary.md`
- `Service-Layer-Enhancement-Summary.md`
- `Database-Foundation-Integration-Summary.md`
- `ReactiveUI-Implementation-Summary.md`
- `Custom-Prompts-Currency-Summary.md`

### **Compliance Report Files (MANDATORY PLACEMENT)**
**Location**: `Documentation/Development/Compliance Reports/[Component]/`

**Component Categories**:
- **Views/**: UI compliance, layout standards, AXAML quality analysis
- **Services/**: Service layer architecture, dependency injection, business logic compliance
- **Database/**: Schema compliance, stored procedure standards, data access patterns
- **Overall/**: System-wide compliance, cross-component analysis, architecture adherence

**File Types That MUST Go Here**:
- Standards compliance analysis (e.g., `Views_Compliance_Report.md`)
- Quality assessment reports
- Architecture adherence evaluations
- Code review summaries
- Performance analysis reports
- Security compliance audits
- Accessibility compliance reports
- Best practices adherence analysis

**Naming Convention**: `[Component]_Compliance_Report.md` or `[Standard]_[Area]_Analysis.md`

**Examples**:
- `Views_Compliance_Report.md` ? `Documentation/Development/Compliance Reports/Views/`
- `Modern_Layout_Compliance.md` ? `Documentation/Development/Compliance Reports/Views/`
- `Service_Architecture_Compliance.md` ? `Documentation/Development/Compliance Reports/Services/`
- `DI_Container_Compliance.md` ? `Documentation/Development/Compliance Reports/Services/`
- `Schema_Compliance_Report.md` ? `Documentation/Development/Compliance Reports/Database/`
- `Architecture_Compliance_Summary.md` ? `Documentation/Development/Compliance Reports/Overall/`

### **Custom Prompt Files (MANDATORY PLACEMENT)**
**Location**: `Documentation/Development/Custom_Prompts/`

**File Types That MUST Go Here**:
- GitHub Copilot automation prompts (e.g., `CustomPrompt_Create_UIElement.md`)
- Compliance fix prompts (e.g., `Compliance_Fix01_EmptyDevelopmentStoredProcedures.md`)
- Code generation templates
- Quality assurance automation
- Development workflow prompts
- System scaffolding prompts

**Naming Convention**: `CustomPrompt_{Action}_{Where}.md` or `Compliance_Fix{Number}_{Issue}.md`

**Examples**:
- `CustomPrompt_Create_UIElement.md` ? `Documentation/Development/Custom_Prompts/`
- `CustomPrompt_Verify_CodeCompliance.md` ? `Documentation/Development/Custom_Prompts/`
- `Compliance_Fix01_EmptyDevelopmentStoredProcedures.md` ? `Documentation/Development/Custom_Prompts/`

### **Database Changes**
- **New stored procedures**: `Development/Database_Files/New_Stored_Procedures.sql`
- **Modified procedures**: `Development/Database_Files/Updated_Stored_Procedures.sql`
- **Schema changes**: `Development/Database_Files/Development_Database_Schema.sql`

## **Naming and Organization Principles**

### **Folder Hierarchy**
1. **Group by function** (Views, ViewModels, Services)
2. **Use clear, descriptive names**
3. **Maintain consistent depth** (avoid deep nesting)
4. **Separate development from production** files

### **File Organization**
- **Related files together**: Keep View and ViewModel pairs in their respective folders
- **Logical grouping**: Group related services and models
- **Clear separation**: Distinguish between production and development files
- **?? Documentation updates**: Centralize in `Documentation/Development/Updates/`
- **?? Compliance reports**: Organize by component in `Documentation/Development/Compliance Reports/`

### **Cross-Reference Management**
- **Documentation links**: Use relative paths for internal references
- **Code dependencies**: Follow dependency injection patterns
- **Asset references**: Use resource dictionaries for shared assets
- **Update documentation**: Link to related upgrade summaries from component documentation
- **Compliance tracking**: Cross-reference compliance reports with related documentation

## **GitHub Organization**

### **.github Directory Structure**
```
.github/
??? workflows/                    # CI/CD pipeline definitions
??? Core-Instructions/           # Essential development guidelines
??? UI-Instructions/            # UI generation and design guidelines
??? Development-Instructions/   # Workflow and tooling guidance
??? Quality-Instructions/       # Quality assurance standards
??? Automation-Instructions/    # Custom prompts and automation
??? copilot-instructions.md    # Main instruction coordination file
```

### **Instruction File Organization**
- **Specialized categories** for different aspects of development
- **Cross-referenced structure** with main instruction file
- **Comprehensive coverage** of all development scenarios

## **Development Workflow Integration**

### **New Feature Development**
1. **Documentation first**: Create instruction files in `Development/UI_Documentation/`
2. **Component creation**: Generate Views and ViewModels
3. **Service integration**: Add business logic services
4. **Database support**: Add stored procedures to development files
5. **Testing and validation**: Use examples and documentation for verification
6. **?? Completion documentation**: Create upgrade summary in `Documentation/Development/Updates/`
7. **?? Quality assessment**: Generate compliance report in `Documentation/Development/Compliance Reports/[Component]/`

### **System Upgrade Workflow**
1. **Pre-upgrade analysis**: Document current state and requirements
2. **Implementation**: Execute upgrade following established patterns
3. **Validation**: Verify functionality and integration
4. **?? Summary creation**: **MANDATORY** - Create comprehensive upgrade summary in `Documentation/Development/Updates/`
5. **?? Compliance verification**: **RECOMMENDED** - Generate compliance report in `Documentation/Development/Compliance Reports/[Component]/`
6. **Cross-reference updates**: Update related documentation with references to upgrade summary

### **Quality Assurance Workflow**
1. **Standards review**: Analyze components against established standards
2. **Compliance assessment**: Evaluate adherence to guidelines and best practices
3. **?? Report generation**: **MANDATORY** - Create compliance report in `Documentation/Development/Compliance Reports/[Component]/`
4. **Issue identification**: Document critical issues and recommendations
5. **Action planning**: Prioritize improvements and next steps
6. **Follow-up tracking**: Monitor compliance improvements over time

### **Code Organization Best Practices**
- **Single responsibility**: Each file should have a clear, single purpose
- **Consistent structure**: Follow established patterns across all components
- **Clear dependencies**: Use dependency injection for service relationships
- **Proper layering**: Maintain separation between UI, business logic, and data access
- **?? Change documentation**: Document all major changes in appropriate update summaries
- **?? Quality tracking**: Regularly assess compliance and generate reports

## **Migration and Maintenance**

### **From Legacy Systems**
- **Gradual migration**: Move components systematically to new structure
- **Documentation preservation**: Maintain instruction files during migration
- **Reference integrity**: Update all cross-references during moves
- **?? Migration tracking**: Document migration progress in update summaries
- **?? Compliance verification**: Ensure migrated components meet current standards

### **Long-term Maintenance**
- **Regular cleanup**: Remove obsolete files and update references
- **Structure validation**: Ensure new additions follow established patterns
- **Documentation updates**: Keep instruction files current with code changes
- **?? Change history**: Maintain update summaries for audit trail and knowledge transfer
- **?? Quality monitoring**: Regular compliance assessments to maintain standards

## **Quality Assurance Requirements**

### **Documentation Synchronization (CRITICAL)**
When creating files in `Documentation/Development/Updates/` or `Documentation/Development/Compliance Reports/`:
1. **HTML Synchronization**: Update ALL corresponding HTML files that reference the documentation
2. **Cross-reference Updates**: Update related instruction files and documentation
3. **Link Validation**: Ensure all internal references remain functional
4. **Template Compliance**: Follow established documentation templates and formatting standards

### **Mandatory Update Summary Sections**
All files in `Documentation/Development/Updates/` MUST include:
- **Overview**: Clear description of what was upgraded/changed
- **Completed Changes**: Detailed list of modifications with before/after comparisons
- **Architecture Improvements**: System-level enhancements and benefits
- **Integration Checklist**: Status of completed and pending integration tasks
- **Usage Migration Guide**: Clear instructions for adopting new patterns
- **Benefits Analysis**: Developer, operations, and user benefits
- **Compliance Impact**: How changes improve system compliance and standards

### **Mandatory Compliance Report Sections**
All files in `Documentation/Development/Compliance Reports/[Component]/` MUST include:
- **Executive Summary**: High-level compliance overview and key findings
- **Analysis Results**: Detailed component-by-component analysis with scores
- **Compliance Scores**: Quantitative assessment of standards adherence
- **Critical Issues**: Priority-ranked issues requiring immediate attention
- **Recommendations**: Specific actions to improve compliance
- **Action Plans**: Prioritized implementation roadmap
- **Success Criteria**: Measurable goals for compliance improvement