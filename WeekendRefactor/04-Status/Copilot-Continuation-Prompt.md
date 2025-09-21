# MTM Copilot Development Continuation Prompt

**Generated**: MTM Audit System v1.0  
**Branch**: copilot/fix-ceed4abc-44a8-47a5-a008-f4549b04c054  
**Context**: Complete MTM Refactor Phase 1-2 - Project Reorganization Foundation + Universal Overlay System Implementation

---

## @copilot Critical Development Context

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

### 🚨 IMMEDIATE PRIORITY: Service Registration (Critical Path)

The MTM Refactor Phase 1-2 has successfully implemented 16 new service files but they are **NOT REGISTERED** in the DI container. Application startup will fail when trying to inject these services.

**CRITICAL ACTION REQUIRED:**

Update `Extensions/ServiceCollectionExtensions.cs` with these exact service registrations:

```csharp
// Infrastructure Services (Singleton lifetime)
services.TryAddSingleton<IEmergencyKeyboardHookService, EmergencyKeyboardHookService>();
services.TryAddSingleton<IFileLoggingService, FileLoggingService>();
services.TryAddSingleton<IFilePathService, FilePathService>();
services.TryAddSingleton<IFileSelectionService, FileSelectionService>();
services.TryAddSingleton<IMTMFileLoggerProvider, MTMFileLoggerProvider>();
services.TryAddSingleton<INavigationService, NavigationService>();
services.TryAddSingleton<IPrintService, PrintService>();

// UI Services (Scoped lifetime for per-session state)
services.TryAddScoped<IColumnConfigurationService, ColumnConfigurationService>();
services.TryAddScoped<IFocusManagementService, FocusManagementService>();
services.TryAddScoped<ISettingsPanelStateManager, SettingsPanelStateManager>();
services.TryAddScoped<ISuccessOverlayService, SuccessOverlayService>();
services.TryAddScoped<ISuggestionOverlayService, SuggestionOverlayService>();
services.TryAddScoped<IThemeService, ThemeService>();
services.TryAddScoped<IVirtualPanelManager, VirtualPanelManager>();
```

**WHY THIS IS CRITICAL:**

- Application will crash at startup when ViewModels try to inject these services
- DI container cannot resolve interfaces without proper registration
- 15 new services are fully implemented but unusable without DI registration

---

### 🏗️ CONTEXT: Successfully Implemented Architecture

**Current Implementation Status (85% Complete):**

✅ **Services/Infrastructure/** - 7 complete service implementations

- `Infrastructure.EmergencyKeyboardHookService.cs` - Keyboard hook with proper disposal
- `Infrastructure.FileLoggingService.cs` - Async file logging with retention policies
- `Infrastructure.FilePathService.cs` - Safe path operations with validation
- `Infrastructure.FileSelectionService.cs` - Avalonia storage provider integration
- `Infrastructure.MTMFileLoggerProvider.cs` - Custom logging provider with MySQL detection
- `Infrastructure.NavigationService.cs` - Navigation with history stack management
- `Infrastructure.PrintService.cs` - Cross-platform print functionality

✅ **Services/UI/** - 8 complete service implementations

- `UI.ColumnConfigurationService.cs` - Column persistence with intelligent caching
- `UI.FocusManagementService.cs` - Tab index management with SuggestionOverlay integration
- `UI.SettingsPanelStateManager.cs` - Complete state tracking with change detection
- `UI.SuccessOverlayService.cs` - Success/error overlay with MainView integration
- `UI.SuggestionOverlayService.cs` - Autocomplete with wildcard search and focus management
- `UI.ThemeService.cs` - Complete theme management with ServiceResult patterns
- `UI.VirtualPanelManager.cs` - Performance-aware panel management with resource optimization

✅ **Views/Overlay/ConfirmationOverlayView** - Complete overlay implementation

- Material Design icons integration
- Proper MTM theme integration with DynamicResource bindings
- Keyboard navigation support
- AXAML compliance with proper xmlns declarations

---

### 📋 HIGH PRIORITY: Models Reorganization Implementation

The Models reorganization plan exists in `WeekendRefactor/02-Reorganization/Models-Reorganization-Plan.md` but **physical file moves are not executed**.

**REQUIRED ACTIONS:**

1. Create Models subdirectories: Core, Events, UI, Print, Shared
2. Move and rename model files using {Folder}.{Model}.cs pattern
3. Update namespace declarations in moved files
4. Fix all `using MTM_WIP_Application_Avalonia.Models;` statements across codebase
5. Test compilation after each category migration

**CRITICAL:** This must be done carefully to prevent breaking changes across ViewModels, Services, and Views.

---

### 🎯 MTM ARCHITECTURAL COMPLIANCE REQUIREMENTS

**MANDATORY PATTERNS (100% Required):**

**MVVM Community Toolkit Only:**

- All ViewModels MUST use `[ObservableObject]`, `[ObservableProperty]`, `[RelayCommand]`
- NO ReactiveUI patterns allowed (ReactiveObject, ReactiveCommand, WhenAnyValue)
- Proper BaseViewModel inheritance with ILogger integration

**Avalonia AXAML Syntax Rules:**

- `xmlns="https://github.com/avaloniaui"` namespace (NOT WPF namespace)
- Use `x:Name` instead of `Name` on Grid definitions
- DynamicResource bindings for all MTM theme elements
- ColumnDefinitions attribute form: `ColumnDefinitions="Auto,*"`

**Database Access Pattern:**

- ALL database operations MUST use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- NO direct SQL, NO string concatenation, NO MySqlCommand usage
- Use actual stored procedure names from existing 45+ procedure catalog

**Service Layer Pattern:**

- Constructor dependency injection with `ArgumentNullException.ThrowIfNull`
- Centralized error handling via `Services.ErrorHandling.HandleErrorAsync()`
- Proper service lifetime management (Singleton/Scoped/Transient)
- TryAdd service registration methods

---

### 🔧 DEVELOPMENT ENVIRONMENT CONTEXT

**Technology Stack:**

- .NET 8.0 with C# 12 nullable reference types
- Avalonia UI 11.3.4 (PRIMARY UI framework)
- MVVM Community Toolkit 8.3.2 with source generators
- MySQL 9.4.0 via MySql.Data package
- Microsoft.Extensions 9.0.8 (DI, Logging, Configuration, Hosting)

**Project Structure:**

- **Services organized by category**: Infrastructure (system services), UI (user interface services)
- **{Folder}.{Service}.cs naming pattern**: Enforced across all new implementations
- **Universal Overlay System**: ConfirmationOverlayView implemented, integration pending
- **WeekendRefactor documentation**: Complete implementation plans and progress tracking

---

### � IMMEDIATE NEXT STEPS (Priority Order)

**Step 1: Service Registration (15 minutes - CRITICAL)**

- Update ServiceCollectionExtensions.cs with all 15 service registrations above
- Test application startup to verify DI resolution
- Check for circular dependencies

**Step 2: Models Reorganization (3 hours - HIGH PRIORITY)**

- Execute Models-Reorganization-Plan.md systematically
- Create folder structure: Models/{Core,Events,UI,Print,Shared}/
- Move files with {Folder}.{Model}.cs naming pattern
- Update all namespace references
- Validate compilation after each category

**Step 3: Universal Overlay Integration (1 hour - HIGH PRIORITY)**

- Connect ConfirmationOverlayView to MainView overlay panel
- Add overlay service orchestration logic
- Test overlay display functionality

---

### �️ VALIDATION CHECKLIST

Before marking complete:

- [ ] All 15 services resolve correctly from DI container
- [ ] Application starts without errors
- [ ] Models follow {Folder}.{Model}.cs naming consistently
- [ ] All namespace references updated correctly
- [ ] ConfirmationOverlayView displays through overlay system
- [ ] Focus management integration works seamlessly
- [ ] Theme system integration maintained
- [ ] No AXAML compilation errors (AVLN2000)
- [ ] All services implement proper MTM error handling patterns

---

### 📊 SUCCESS METRICS

**When implementation is complete:**

- ✅ Clean application startup with all services registered
- ✅ Zero namespace compilation errors
- ✅ Universal overlay system fully functional
- ✅ Models organized with consistent {Folder}.{Model}.cs naming
- ✅ 100% MTM architectural pattern compliance
- ✅ Service layer integration complete with proper dependency injection

---

**MTM Refactor Phase 1-2 is 85% complete with excellent architectural compliance. The remaining 15% consists primarily of service registration and models reorganization - both are well-documented and ready for implementation.**

**Priority focus should be on service registration first (prevents application startup failure), then Models reorganization (prevents future compilation errors), then overlay system integration (completes Universal Overlay System feature).**
