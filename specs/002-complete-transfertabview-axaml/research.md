# Research: TransferTabView.axaml Implementation

## Technology Decisions

### Avalonia DataGrid vs TransferCustomDataGrid

**Decision**: Replace TransferCustomDataGrid with standard Avalonia DataGrid
**Rationale**:

- Eliminates custom dependency maintenance burden
- Ensures Avalonia UI 11.3.4 compatibility and future upgrade path
- Follows MTM architecture principle of using standard framework components
- RemoveTabView.axaml successfully demonstrates this pattern
**Alternatives considered**:
- Keep TransferCustomDataGrid: Rejected due to maintenance overhead and AVLN2000 error risks
- Third-party DataGrid: Rejected per MTM constitution (standard components only)

### Column Customization Implementation

**Decision**: Standard Avalonia ComboBox with MySQL persistence
**Rationale**:

- Leverages existing usr_ui_settings table infrastructure
- JSON storage provides flexible column configuration
- Per-user persistence aligns with manufacturing operator workflows
- ComboBox provides familiar UI pattern from other MTM interfaces
**Alternatives considered**:
- Local storage: Rejected due to cross-workstation operator requirements
- Custom dropdown: Rejected per MTM Theme V2 compliance requirements

### EditInventoryView Integration Strategy

**Decision**: Direct AXAML namespace reference with DataContext inheritance
**Rationale**:

- Maintains clean separation of concerns (no code-behind instantiation)
- Follows established MVVM Community Toolkit patterns
- Ensures proper disposal and lifecycle management
- Consistent with RemoveTabView.axaml overlay patterns
**Alternatives considered**:
- Code-behind instantiation: Rejected per MVVM principles
- Separate dialog window: Rejected due to manufacturing operator workflow efficiency requirements

### Theme V2 Integration Approach

**Decision**: DynamicResource bindings exclusively with StyleSystem.axaml patterns
**Rationale**:

- Ensures consistent theming across all MTM interfaces
- Supports runtime theme switching without restart
- Prevents hardcoded styling that breaks theme compliance
- Follows RemoveTabView.axaml implementation patterns
**Alternatives considered**:
- Mixed hardcoded/dynamic styling: Rejected per MTM Theme V2 strict compliance requirements
- Custom styling: Rejected due to maintenance and consistency concerns

## Database Integration Patterns

### User Settings Persistence

**Pattern**: MySQL usr_ui_settings table with JSON column storage
**Implementation**: New stored procedure for "TransferTabColumns" setting
**Schema**: Leverages existing UserId, SettingsJson columns
**Performance**: Single query lookup with JSON parsing, cached in memory during session

### Transfer Operation Logic

**Pattern**: Single transaction with inventory splitting for partial transfers
**Implementation**: Preserve batch numbers, update quantities atomically
**Audit Trail**: Single transaction record with original quantity and split details
**Error Handling**: Manual retry on database connectivity failures per clarification

## UI/UX Research

### Manufacturing Operator Workflows

**Pattern**: Minimal clicks for frequent operations
**Implementation**:

- Double-click inventory row opens EditInventoryView
- Quantity auto-capping prevents operator errors
- Auto-close on successful transfer maintains workflow efficiency
**Reference**: RemoveTabView.axaml demonstrates similar operator-optimized patterns

### Cross-Platform Considerations

**Pattern**: Responsive layouts with ScrollViewer containers
**Implementation**: MinWidth/MinHeight constraints with flexible content areas
**Testing**: Validated on Windows/macOS/Linux with 1024x768 to 4K resolutions
**Performance**: No platform-specific code, pure Avalonia UI implementation

## Technical Constraints Resolution

### AVLN2000 Error Prevention

**Strategy**: Strict adherence to Avalonia AXAML patterns
**Implementation**: Use x:Name instead of Name on Grid controls, proper namespace declarations
**Validation**: Follow RemoveTabView.axaml as error-free reference implementation

### Memory Optimization

**Strategy**: Proper disposal patterns for 8+ hour manufacturing sessions
**Implementation**: IDisposable patterns, event unsubscription, DataContext cleanup
**Monitoring**: Memory growth tracking during extended operation sessions

### Performance Optimization

**Strategy**: Async database operations with UI responsiveness
**Implementation**: Background operations with loading indicators, cancellation token support
**Benchmarks**: Database operations <30s timeout, UI thread unblocking during transfers
