# MTM Comprehensive Code Analysis Prompt

**Purpose**: Complete codebase analysis for code quality improvements, performance optimization, and redundant code removal  
**Based on**: `.github/prompts/mtm-audit-system.prompt.md`  
**Created**: September 17, 2025  

---

## 🎯 Comprehensive Code Analysis Request

**Paste this exact text into your GitHub Copilot Agent window:**

```text
Execute comprehensive MTM code analysis following the mtm-audit-system.prompt.md methodology. Analyze the entire MTM WIP Application codebase to identify:

1. **Code Quality Issues**: Redundant code, unused code (not TODO placeholders), non-standard patterns
2. **Performance Problems**: Memory leaks, inefficient database calls, UI performance issues  
3. **Readability Issues**: Inconsistent naming, poor structure, missing documentation
4. **Architecture Violations**: Non-compliant MVVM patterns, improper service usage, incorrect AXAML syntax
5. **MTM Pattern Compliance**: Validate against established MTM architectural patterns

Generate a comprehensive gap report with:
- Priority classification (🚨 Critical, ⚠️ High, 📋 Medium)
- Specific file locations and line numbers
- Code examples showing violations and corrections
- Performance impact assessments
- Estimated time to fix each issue

Create a targeted Copilot continuation prompt for implementing all improvements.

Follow the complete mtm-audit-system.prompt.md workflow for thorough analysis.

#github-pull-request_copilot-coding-agent
```

---

## 📋 Analysis Categories

### 🔍 **Code Quality Assessment**

- **Redundant Code**: Duplicate methods, repeated logic patterns
- **Unused Code**: Dead code (excluding TODO placeholders)
- **Non-Standard Patterns**: Deviations from MTM architectural guidelines
- **Naming Inconsistencies**: Variables, methods, classes not following conventions

### ⚡ **Performance Analysis**

- **Memory Issues**: Memory leaks, excessive allocations, inefficient collections
- **Database Performance**: Non-optimized queries, connection handling issues
- **UI Performance**: Heavy operations on UI thread, excessive bindings
- **Resource Management**: Improper disposal, resource leaks

### 📖 **Readability & Maintainability**

- **Code Structure**: Poor organization, missing regions, unclear flow
- **Documentation**: Missing XML comments, unclear method purposes
- **Complexity**: Methods too long, excessive nesting, unclear logic
- **Consistency**: Mixed coding styles, inconsistent formatting

### 🏗️ **Architecture Compliance**

- **MVVM Community Toolkit**: Proper `[ObservableObject]`, `[ObservableProperty]`, `[RelayCommand]` usage
- **Avalonia AXAML**: Correct namespace, `x:Name` vs `Name`, proper syntax
- **Service Layer**: Dependency injection patterns, service registration
- **Database Patterns**: Stored procedures only, proper error handling
- **Error Handling**: Centralized `Services.ErrorHandling.HandleErrorAsync()` usage

---

## 📊 Expected Deliverables

### 1. **Comprehensive Gap Report**

**Location**: `docs/audit/master-gap-report.md`

**Contains**:

- Executive summary with code health metrics
- File-by-file analysis with specific issues
- Priority-ranked improvement list (🚨 Critical, ⚠️ High, 📋 Medium)
- Performance impact assessments
- Estimated effort for each improvement

### 2. **Copilot Continuation Prompt**

**Location**: `docs/audit/master-copilot-prompt.md`

**Contains**:

- Ready-to-use prompt for implementing fixes
- Specific code examples showing violations and corrections
- MTM compliance requirements with patterns
- Implementation priority order and dependencies
- Complete `#github-pull-request_copilot-coding-agent` automation

---

## 🎯 Analysis Focus Areas

### **Critical Issues** (🚨 Fix Immediately)

- Compilation errors or warnings
- Security vulnerabilities
- Memory leaks affecting performance
- Database connection issues
- Critical MVVM pattern violations

### **High Priority** (⚠️ Fix Soon)

- Performance bottlenecks
- Code duplication reducing maintainability
- Architecture pattern inconsistencies
- Missing error handling
- UI responsiveness issues

### **Medium Priority** (📋 Improvement Opportunities)

- Code readability enhancements
- Documentation improvements
- Naming convention updates
- Code organization optimizations
- Minor performance tweaks

---

## 🚀 Usage Instructions

1. **Copy the prompt** from the "Comprehensive Code Analysis Request" section above
2. **Open GitHub Copilot Agent** in VS Code
3. **Paste the complete prompt** into the agent window
4. **Wait for analysis completion** - this will take several minutes for full codebase scan
5. **Review generated reports** in `docs/audit/` directory
6. **Use continuation prompt** to implement fixes automatically

---

## 📈 Success Metrics

After running the analysis and implementing improvements, expect:

- **Code Quality**: Reduced duplication, cleaner architecture
- **Performance**: Faster UI response, reduced memory usage
- **Maintainability**: Consistent patterns, better documentation
- **Compliance**: 100% MTM architectural pattern adherence
- **Readability**: Clear naming, organized structure

---

## 🔄 Recommended Frequency

- **Full Analysis**: Monthly or before major releases
- **Focused Analysis**: Weekly for specific components
- **Post-Development**: After completing new features
- **Pre-Merge**: Before merging large pull requests

---

**Note**: This analysis uses the established MTM audit system methodology specifically designed for .NET 8 Avalonia MVVM applications with MySQL integration.
