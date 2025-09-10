# MTM WIP Application Documentation

Welcome to the comprehensive documentation system for the MTM WIP Application - a .NET 8 Avalonia MVVM desktop application for manufacturing inventory management.

## 🚀 Quick Start

### For Developers
1. **Read First**: `.github/copilot-instructions.md` - Main development guide
2. **Interactive Docs**: Open `.github/Documentation-Management/html-documentation/index.html` in your browser
3. **Chat Modes**: Use specialized AI personas in `.github/chatmodes/` for focused assistance

### For New Team Members
1. Start with the interactive HTML documentation for plain English explanations
2. Review the View Implementation Guide for hands-on development
3. Use the Feature Request template for planning new work

## 📁 Documentation Structure

### Awesome-Copilot Compliant Organization
```
.github/
├── 📝 prompts/              # Task-specific prompts (.prompt.md) - 21 files
├── 📋 instructions/         # Coding standards (.instructions.md) - 6 files  
├── 🤖 chatmodes/           # AI personas (.chatmode.md) - 4 files
├── 📊 Project-Management/   # Planning and requirements
├── 🔧 Development-Guides/   # Setup, standards, testing, components
├── 🏗️ Architecture-Documentation/ # System design and data models
├── ⚙️ Operations/          # Deployment and maintenance
└── 📚 Documentation-Management/ # Index and interactive system
```

## 🎯 Key Features

### ✅ Interactive HTML Documentation
- **Mobile-Responsive**: Works on tablets and phones
- **Search Functionality**: Quick document discovery
- **Plain English**: Understandable by junior developers  
- **Real-World Scenarios**: Step-by-step usage examples
- **MTM Theme**: Official color palette integration

### ✅ Specialized AI Chat Modes
- **MTM Architect**: System design and architecture decisions
- **MTM UI Developer**: Avalonia AXAML and design system expertise
- **MTM Database Developer**: MySQL stored procedures and data security
- **MTM Code Reviewer**: Quality assurance and standards compliance

### ✅ Comprehensive Templates
- **Feature Request**: Complete technical specifications generator
- **UI Component**: Avalonia component creation with theme integration
- **ViewModel Creation**: MVVM Community Toolkit implementation
- **Database Operation**: Secure stored procedure patterns

## 🔧 Usage Guide

### Using Prompts
```bash
# In GitHub Copilot Chat
/mtm-feature-request    # Generate feature specifications
/mtm-ui-component      # Create UI components
/mtm-viewmodel-creation # Build ViewModels
/mtm-database-operation # Implement database operations
```

### Using Chat Modes
```bash
# In GitHub Copilot Chat  
@mtm-architect         # Architecture guidance
@mtm-ui-developer      # UI development help
@mtm-database-developer # Database operations
@mtm-code-reviewer     # Code quality review
```

### Using Instructions
Instructions automatically apply to file patterns:
- `avalonia-ui-guidelines.instructions.md` → `.axaml` files
- `mvvm-community-toolkit.instructions.md` → `ViewModel.cs` files
- `mysql-database-patterns.instructions.md` → Service files with database operations

## 📊 Documentation Statistics

- **Total Markdown Files**: 217+ across repository
- **Prompt Files**: 21 (.prompt.md)
- **Instruction Files**: 6 (.instructions.md)
- **Chat Mode Files**: 4 (.chatmode.md)
- **Archive Preserved**: Original documentation safely backed up

## 🎨 MTM Design System

### Core Colors
- **Primary Action**: #0078D4 (Windows 11 Blue)
- **Secondary Action**: #106EBE (Darker Blue)
- **Success**: #4CAF50 (Green)
- **Warning**: #FFB900 (Amber)
- **Error**: #D13438 (Red)

### Technology Stack
- **.NET 8** with C# 12
- **Avalonia UI 11.3.4** (NOT WPF)
- **MVVM Community Toolkit 8.3.2**
- **MySQL 9.4.0** with stored procedures only
- **Microsoft Extensions 9.0.8** for DI and logging

## 🔍 Finding Documentation

### By Topic
- **UI Development**: `Development-Guides/UI-Components/`
- **Database Work**: `prompts/mtm-database-operation.prompt.md`
- **Architecture**: `chatmodes/mtm-architect.chatmode.md`
- **Code Standards**: `instructions/` directory

### By File Type
- **Planning**: `.github/Project-Management/`
- **Implementation**: `.github/Development-Guides/`
- **Reference**: `.github/instructions/`
- **Templates**: `.github/prompts/`

## 🔗 Key Links

- [Interactive Documentation](./Documentation-Management/html-documentation/index.html) - Browse all docs
- [Master Index](./Documentation-Management/master_documentation-index.md) - Complete file listing
- [Main Instructions](./copilot-instructions.md) - Primary development guide
- [Archive](./Documentation-Management/MTM_Documentation_Archive_2025-09-10.tar.gz) - Original docs backup

## 📱 Mobile Access

The interactive HTML documentation is fully responsive and works on:
- 📱 Smartphones (iOS/Android)
- 📱 Tablets (iPad/Android tablets)  
- 💻 Desktop browsers
- 🖥️ Development environments

## 🆘 Getting Help

1. **Quick Questions**: Use interactive HTML docs search
2. **Development Help**: Use appropriate chat mode (@mtm-ui-developer, etc.)
3. **Planning**: Use /mtm-feature-request prompt
4. **Architecture**: Consult @mtm-architect chat mode
5. **Code Review**: Use @mtm-code-reviewer chat mode

## 📈 Migration Status

### ✅ Completed
- Interactive HTML documentation system
- Awesome-copilot directory structure  
- 4 specialized AI chat modes
- Enhanced prompt templates
- Original documentation archive

### 🔄 In Progress
- Legacy content migration from /docs/ and /Documentation/
- Link validation and cross-reference updates
- File extension standardization

---

**Last Updated**: September 10, 2025  
**Documentation System**: Awesome-Copilot Compliant  
**Archive**: MTM_Documentation_Archive_2025-09-10.tar.gz  
**Status**: Phase 2 Implementation Complete