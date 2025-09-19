# Weekend Refactor Implementation Guide

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

This guide provides complete structure and resources for implementing the MTM Overlay System Refactor during weekend development sessions.

## 🚀 Quick Start

### **Before You Begin**

1. Review `WeekendRefactor/OverlayAnalysis/Comprehensive-Overlay-Analysis.md` for full context
2. Check `WeekendRefactor/Implementation/Implementation-Status.md` for current progress
3. Use `WeekendRefactor/Implementation/Task-Checklist-Template.md` for tracking individual tasks

### **Development Environment Setup**

```powershell
# Navigate to project root
cd "c:\Users\jkoll\source\MTM_WIP_Application_Avalonia"

# Ensure solution builds cleanly
dotnet build MTM_WIP_Application_Avalonia.sln

# Run existing tests to establish baseline
dotnet test
```

### **Stage Implementation Order**

1. **Stage 1** - Critical Safety Fixes (MUST DO FIRST)
2. **Stage 2** - Universal Overlay Service (FOUNDATION)
3. **Stage 3** - Critical Missing Overlays  
4. **Stage 4** - Performance Overlays
5. **Stage 5** - Developer Experience Overlays
6. **Stage 6** - Documentation & Cleanup

## 📁 File Structure Overview

### **Analysis Files (Reference Only)**

```
WeekendRefactor/OverlayAnalysis/
├── Comprehensive-Overlay-Analysis.md    # 14,000+ word complete analysis
├── Missing-Overlay-Specifications.md    # 23 missing overlays detailed
├── View-Usage-Mapping.md               # All view-to-overlay relationships
└── Documentation-Inventory.md          # All overlay documentation audit
```

### **Implementation Files (Working Documents)**

```
WeekendRefactor/Implementation/
├── Implementation-Status.md             # Master progress tracker
├── Code-Templates.md                   # Consistent code patterns
├── Weekend-Implementation-Guide.md     # This file
├── Task-Checklist-Template.md         # Individual task tracking
├── Stages4-6-Summary.md               # Later stage overview
└── Stage1-CriticalSafety/             # Immediate safety fixes
└── Stage2-UniversalService/           # Foundation service
└── Stage3-CriticalOverlays/           # Essential overlays
└── Stage4-PerformanceOverlays/        # Performance enhancements
└── Stage5-DeveloperExperience/        # Development tools
└── Stage6-Documentation/              # Final cleanup
```

## 🎯 Weekend Session Planning

### **2-Hour Session: Critical Safety**

**Goal:** Fix immediate safety issues
**Files:** Focus on `Stage1-CriticalSafety/Implementation-Guide.md`

**Priority Tasks:**

1. Remove deprecated `NoteEditorOverlay` (45 minutes)
2. Add confirmation to `AdvancedRemoveView` (30 minutes)  
3. Add batch confirmation to `AdvancedInventoryView` (30 minutes)
4. Test safety improvements (15 minutes)

**Success Criteria:** No dangerous operations without confirmation

### **4-Hour Session: Foundation Service**

**Goal:** Implement Universal Overlay Service
**Files:** Focus on `Stage2-UniversalService/Implementation-Guide.md`

**Priority Tasks:**

1. Create `IUniversalOverlayService` interface (45 minutes)
2. Implement service with dependency injection (90 minutes)
3. Create base overlay request/response models (45 minutes)
4. Add service to `ServiceCollectionExtensions` (30 minutes)
5. Test service registration and basic functionality (30 minutes)

**Success Criteria:** Universal service accessible from any ViewModel

### **6-Hour Session: Critical Overlays**

**Goal:** Implement most important missing overlays
**Files:** Focus on `Stage3-CriticalOverlays/Implementation-Guide.md`

**Priority Tasks:**

1. Global Error Overlay (2 hours)
2. Field Validation Overlay (1.5 hours)  
3. Connection Status Overlay (1.5 hours)
4. Batch Operation Progress Overlay (1 hour)

**Success Criteria:** Core overlay types available system-wide

## 🔧 Technical Implementation Notes

### **MTM Architecture Compliance**

- **MVVM Community Toolkit**: Use `[ObservableProperty]` and `[RelayCommand]` patterns
- **Avalonia Syntax**: Use `x:Name` (not `Name`) on Grid definitions
- **Database Access**: ONLY via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- **Theme Integration**: Use `{DynamicResource MTM_Shared_Logic.*}` patterns
- **Error Handling**: Always use `Services.ErrorHandling.HandleErrorAsync()`

### **Code Quality Standards**

- **Null Safety**: Enable nullable reference types, use `ArgumentNullException.ThrowIfNull()`
- **Dependency Injection**: Constructor injection with `TryAdd` service registration
- **Resource Management**: Implement `IDisposable` for overlay ViewModels
- **Testing**: Unit tests for ViewModels, integration tests for services
- **Documentation**: XML comments for public APIs

### **Performance Considerations**

- **Memory Management**: Dispose overlay ViewModels when closed
- **UI Responsiveness**: Use async patterns for database operations
- **Theme Resources**: Cache dynamic resources to prevent repeated lookups
- **Service Lifetimes**: Use appropriate scoping (Singleton/Scoped/Transient)

## 🚨 Critical Safety Rules

### **Database Operations**

```csharp
// ✅ CORRECT: Always validate column names against actual schema
var userName = row["User"].ToString(); // Column name is "User", not "UserId"

// ❌ WRONG: Never use assumed column names
var userName = row["UserId"].ToString(); // Will throw ArgumentException
```

### **AXAML Syntax**

```xml
<!-- ✅ CORRECT: Avalonia namespace and x:Name -->
<Grid x:Name="MainGrid" ColumnDefinitions="Auto,*">

<!-- ❌ WRONG: WPF patterns -->
<Grid Name="MainGrid"> <!-- This causes AVLN2000 errors -->
```

### **Service Registration**

```csharp
// ✅ CORRECT: TryAdd prevents duplicate registrations
services.TryAddSingleton<IUniversalOverlayService, UniversalOverlayService>();

// ❌ WRONG: Direct Add can cause conflicts
services.AddSingleton<IUniversalOverlayService, UniversalOverlayService>();
```

## 📊 Progress Tracking

### **Implementation Status Tracking**

Update `Implementation-Status.md` after each session:

```markdown
## Stage 1 Progress
- [x] Remove NoteEditorOverlay - Completed 2024-01-15
- [x] Add AdvancedRemoveView confirmations - Completed 2024-01-15  
- [ ] Add AdvancedInventoryView batch confirmations - In Progress
```

### **Task-Level Tracking**

Use `Task-Checklist-Template.md` for individual tasks:

```markdown
## Task: Implement Global Error Overlay
- [ ] Create ErrorOverlayRequest model
- [ ] Create ErrorOverlayViewModel  
- [ ] Create ErrorOverlayView.axaml
- [ ] Add to Universal Overlay Service
- [ ] Write unit tests
- [ ] Integration testing
```

## 🧪 Testing Strategy

### **Test-Driven Implementation**

1. **Write tests first** for ViewModels and services
2. **Implement minimal code** to pass tests
3. **Refactor** while keeping tests green
4. **Add integration tests** for end-to-end scenarios

### **Testing Tools Available**

- **MSTest**: Unit testing framework  
- **Moq**: Service mocking for unit tests
- **FluentAssertions**: Readable test assertions
- **TestHost**: Integration testing for dependency injection

### **Manual Testing Checklist**

```markdown
## Overlay Testing Checklist
- [ ] Overlay displays correctly in all themes (MTM_Blue, MTM_Green, MTM_Red, MTM_Dark)
- [ ] Overlay responds to keyboard navigation (Tab, Enter, Escape)
- [ ] Overlay blocks interaction with underlying view
- [ ] Overlay dismisses properly and cleans up resources
- [ ] Error cases display appropriate error overlays
```

## 🎨 Theme Integration

### **MTM Theme System**

All overlays must support dynamic theme switching:

```xml
<Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
        BorderThickness="1">
```

### **Required Theme Resources**

- `MTM_Shared_Logic.CardBackgroundBrush`
- `MTM_Shared_Logic.BorderBrush`  
- `MTM_Shared_Logic.PrimaryAction`
- `MTM_Shared_Logic.SecondaryTextBrush`
- `MTM_Shared_Logic.PanelBackgroundBrush`

### **Theme Testing**

Test overlays in all four themes:

1. MTM_Blue (default)
2. MTM_Green  
3. MTM_Red
4. MTM_Dark

## 📝 Documentation Requirements

### **Code Documentation**

```csharp
/// <summary>
/// Represents a request to display an error overlay with detailed information.
/// </summary>
/// <param name="Title">The title to display in the overlay header</param>
/// <param name="Message">The main error message to display</param>
/// <param name="Exception">Optional exception details for diagnostic purposes</param>
public record ErrorOverlayRequest(
    string Title,
    string Message, 
    Exception? Exception = null) : BaseOverlayRequest;
```

### **Implementation Documentation**

Update relevant documentation files:

- `docs/architecture/overlay-architecture.md`
- `docs/development/overlay-development-guide.md`  
- `Documentation/Development/Overlay-Integration-Guide.md`

## 🔄 Iterative Development Approach

### **Session Structure**

1. **Start (10 minutes)**: Review progress, plan session goals
2. **Implement (80% of time)**: Focus on coding with regular commits
3. **Test (15% of time)**: Unit tests and manual verification
4. **Document (5% of time)**: Update progress and next steps

### **Git Workflow**

```powershell
# Create feature branch for overlay work
git checkout -b feature/overlay-refactor-stage1

# Make regular commits during development
git add .
git commit -m "Stage 1: Remove deprecated NoteEditorOverlay"

# Push progress for backup
git push origin feature/overlay-refactor-stage1
```

### **Quality Gates**

Before moving to next stage:

- [ ] All new code has unit tests
- [ ] Solution builds without warnings
- [ ] Manual testing passes in primary theme
- [ ] Progress documented in Implementation-Status.md

---

**This implementation structure provides everything needed for successful weekend overlay refactoring sessions while maintaining MTM application quality standards and architectural patterns.**
