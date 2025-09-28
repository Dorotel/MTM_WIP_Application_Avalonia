# Tasks: Complete TransferTabView.axaml Implementation

**Input**: Design documents from `/specs/002-complete-transfertabview-axaml/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Execution Flow (main)

```
1. Load plan.md from feature directory ✓
   → Tech stack: .NET 8.0, Avalonia UI 11.3.4, MVVM Community Toolkit 8.3.2, MySQL 9.4.0
   → Structure: Single desktop application with Views/ViewModels/Services separation
2. Load optional design documents: ✓
   → data-model.md: TransferTabViewModel, InventoryItem, TransferOperation, ColumnConfiguration
   → contracts/: ITransferService, IColumnConfigurationService, MySQL stored procedures
   → research.md: Avalonia DataGrid replacement, MySQL persistence, Theme V2 integration
3. Generate tasks by category: ✓
   → Setup: Database scripts, service registration, dependencies
   → Tests: Contract tests, integration tests, UI tests
   → Core: Services, ViewModels, AXAML refactoring
   → Integration: Database connections, EditInventoryView integration
   → Polish: Unit tests, performance validation, documentation
4. Apply task rules: ✓
   → Different files = mark [P] for parallel
   → Same file = sequential (no [P])
   → Tests before implementation (TDD)
5. Number tasks sequentially (T001, T002...) ✓
6. Generate dependency graph ✓
7. Create parallel execution examples ✓
8. Validate task completeness: ✓
   → All contracts have tests ✓
   → All entities have models ✓
   → All AXAML changes planned ✓
9. Return: SUCCESS (tasks ready for execution) ✓
```

## Format: `[ID] [P?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- Include exact file paths in descriptions

## Path Conventions

- **Avalonia Desktop App**: Views/, ViewModels/, Services/, Models/ at repository root
- **Tests**: tests/ directory with unit/integration/ui subdirectories
- **Database**: SQL scripts in contracts/ directory

## Phase 3.1: Setup

- [x] T001 Create MySQL stored procedures from contracts/service-contracts.md
- [ ] T002 Register ITransferService and IColumnConfigurationService in Extensions/ServiceCollectionExtensions.cs
- [x] T003 [P] Backup existing TransferTabView.axaml to TransferTabView.axaml.backup

## Phase 3.2: Tests First (TDD) ⚠️ MUST COMPLETE BEFORE 3.3

**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**

- [x] T004 [P] Contract test ITransferService.SearchInventoryAsync in tests/unit/Services/TransferServiceTests.cs
- [x] T005 [P] Contract test ITransferService.ExecuteTransferAsync in tests/unit/Services/TransferServiceTests.cs
- [x] T006 [P] Contract test IColumnConfigurationService.LoadColumnConfigAsync in tests/unit/Services/ColumnConfigurationServiceTests.cs
- [x] T007 [P] Contract test IColumnConfigurationService.SaveColumnConfigAsync in tests/unit/Services/ColumnConfigurationServiceTests.cs
- [x] T008 [P] Integration test TransferTabViewModel with MVVM Community Toolkit in tests/integration/ViewModels/TransferTabViewModelTests.cs
- [x] T009 [P] Integration test column customization persistence in tests/integration/Services/ColumnConfigurationIntegrationTests.cs
- [x] T010 [P] UI test DataGrid replacement and column customization in tests/ui/Views/TransferTabViewUITests.cs
- [x] T011 [P] UI test EditInventoryView integration workflow in tests/ui/Views/EditInventoryIntegrationUITests.cs

## Phase 3.3: Core Implementation (ONLY after tests are failing)

- [x] T012 [P] ColumnConfiguration model in Models/ColumnConfiguration.cs
- [x] T013 [P] TransferOperation model in Models/TransferOperation.cs
- [x] T014 [P] TransferResult model in Models/TransferResult.cs
- [x] T015 [P] ValidationResult model in Models/ValidationResult.cs
- [ ] T016 [P] ColumnConfigurationService implementation in Services/ColumnConfigurationService.cs
- [ ] T017 [P] TransferService implementation in Services/TransferService.cs
- [ ] T018 Update TransferTabViewModel with MVVM Community Toolkit patterns in ViewModels/MainForm/TransferItemViewModel.cs
- [ ] T019 Replace TransferCustomDataGrid with standard DataGrid in Views/MainForm/Panels/TransferTabView.axaml
- [ ] T020 Add column customization ComboBox to TransferTabView.axaml
- [ ] T021 Integrate EditInventoryView with visibility binding in TransferTabView.axaml
- [ ] T022 Apply MTM Theme V2 styling with DynamicResource bindings in TransferTabView.axaml

## Phase 3.4: Integration

- [ ] T023 Connect ColumnConfigurationService to MySQL usr_ui_settings table
- [ ] T024 Connect TransferService to inventory stored procedures
- [ ] T025 Integrate EditInventoryView triggers (double-click, auto-close) in TransferTabView.axaml.cs
- [ ] T026 Add quantity auto-capping validation logic to TransferTabViewModel
- [ ] T027 Implement error handling with Services.ErrorHandling.HandleErrorAsync()
- [ ] T028 Add loading indicators and async operation feedback

## Phase 3.5: Polish

- [ ] T029 [P] Unit tests for TransferTabViewModel MVVM Community Toolkit patterns in tests/unit/ViewModels/TransferTabViewModelUnitTests.cs
- [ ] T030 [P] Unit tests for quantity validation and auto-capping in tests/unit/ViewModels/TransferTabViewModelValidationTests.cs
- [ ] T031 [P] Performance tests for database operations (<30s timeout) in tests/performance/DatabasePerformanceTests.cs
- [ ] T032 [P] Cross-platform UI tests (Windows/macOS/Linux) in tests/ui/CrossPlatform/TransferTabCrossPlatformTests.cs
- [ ] T033 [P] Memory leak validation for 8+ hour sessions in tests/performance/MemoryLeakTests.cs
- [ ] T034 Execute quickstart.md validation scenarios manually
- [ ] T035 Update documentation with new transfer workflows
- [ ] T036 Remove TransferCustomDataGrid references and cleanup

## Dependencies

- Setup (T001-T003) before everything
- Tests (T004-T011) before implementation (T012-T022)
- Models (T012-T015) before Services (T016-T017)
- Services before ViewModel updates (T018)
- ViewModel before UI changes (T019-T022)
- Core implementation before integration (T023-T028)
- Integration before polish (T029-T036)

## Parallel Example

```bash
# Launch T004-T007 together (different service contract tests):
Task: "Contract test ITransferService.SearchInventoryAsync in tests/unit/Services/TransferServiceTests.cs"
Task: "Contract test ITransferService.ExecuteTransferAsync in tests/unit/Services/TransferServiceTests.cs"
Task: "Contract test IColumnConfigurationService.LoadColumnConfigAsync in tests/unit/Services/ColumnConfigurationServiceTests.cs"
Task: "Contract test IColumnConfigurationService.SaveColumnConfigAsync in tests/unit/Services/ColumnConfigurationServiceTests.cs"

# Launch T012-T015 together (different model files):
Task: "ColumnConfiguration model in Models/ColumnConfiguration.cs"
Task: "TransferOperation model in Models/TransferOperation.cs"
Task: "TransferResult model in Models/TransferResult.cs"
Task: "ValidationResult model in Models/ValidationResult.cs"

# Launch T016-T017 together (different service implementations):
Task: "ColumnConfigurationService implementation in Services/ColumnConfigurationService.cs"
Task: "TransferService implementation in Services/TransferService.cs"
```

## Critical Success Factors

- **AVLN2000 Prevention**: Use x:Name instead of Name on Grid controls in AXAML
- **Theme V2 Compliance**: All styling via DynamicResource bindings, no hardcoded values
- **MVVM Community Toolkit**: Use [ObservableProperty] and [RelayCommand] patterns exclusively
- **Database Integration**: Use Helper_Database_StoredProcedure.ExecuteDataTableWithStatus() pattern
- **Error Handling**: All exceptions through Services.ErrorHandling.HandleErrorAsync()
- **Manufacturing Workflows**: Minimal clicks, quantity auto-capping, double-click edit triggers

## Task Generation Rules Applied

1. **From Contracts**: Each service interface → contract test task [P] (T004-T007)
2. **From Data Model**: Each entity → model creation task [P] (T012-T015)
3. **From User Stories**: Each workflow → integration test [P] (T008-T011)
4. **Ordering**: Setup → Tests → Models → Services → UI → Integration → Polish
5. **Parallel Execution**: Different files only, respecting dependencies

## Validation Checklist

- [x] All contracts have corresponding tests (ITransferService, IColumnConfigurationService)
- [x] All entities have model tasks (ColumnConfiguration, TransferOperation, TransferResult, ValidationResult)
- [x] All tests come before implementation (Phase 3.2 before 3.3)
- [x] Parallel tasks truly independent (different files, no shared dependencies)
- [x] Each task specifies exact file path
- [x] No task modifies same file as another [P] task
- [x] TDD approach: failing tests before implementation
- [x] Database setup before service implementation
- [x] Services before ViewModels before Views
- [x] Core implementation before integration features
