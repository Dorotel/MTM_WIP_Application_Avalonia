# Quickstart: TransferTabView Implementation

## Development Setup

### Prerequisites

- .NET 8.0 SDK installed
- Visual Studio 2022 or VS Code with C# Dev Kit
- MySQL Server 9.4.0 with MTM database
- Avalonia extension for XAML editing

### Environment Setup

1. Clone MTM_WIP_Application_Avalonia repository
2. Navigate to feature branch: `git checkout 002-complete-transfertabview-axaml`
3. Restore dependencies: `dotnet restore`
4. Update connection string in `Config/appsettings.Development.json`

## Implementation Validation

### Phase 1: Database Setup

1. **Execute SQL scripts** for new stored procedures:

   ```sql
   -- Run these in MySQL Workbench or similar tool
   source contracts/usr_ui_settings_Get_TransferColumns.sql;
   source contracts/usr_ui_settings_Set_TransferColumns.sql;
   source contracts/inv_transfer_Execute_WithSplit.sql;
   ```

2. **Verify table structure**:

   ```sql
   DESCRIBE usr_ui_settings;
   -- Should show: UserId, SettingsJson, ShortcutsJson, UpdatedAt
   ```

3. **Test data insertion**:

   ```sql
   INSERT INTO usr_ui_settings (UserId, SettingsJson) 
   VALUES ('testuser', '{"TransferTabColumns": {"VisibleColumns": ["PartID", "Operation"]}}');
   ```

### Phase 2: AXAML Implementation

1. **Backup current TransferTabView.axaml**:

   ```powershell
   Copy-Item "Views/MainForm/Panels/TransferTabView.axaml" "Views/MainForm/Panels/TransferTabView.axaml.backup"
   ```

2. **Replace TransferCustomDataGrid with standard DataGrid**:
   - Remove `xmlns:customDataGrid` namespace
   - Replace `<customDataGrid:TransferCustomDataGrid>` with `<DataGrid>`
   - Update binding properties to match standard DataGrid API

3. **Add column customization dropdown**:

   ```xml
   <ComboBox ItemsSource="{Binding AvailableColumns}"
             SelectedItems="{Binding SelectedColumns}"
             Classes="ManufacturingDropdown" />
   ```

4. **Integrate EditInventoryView**:

   ```xml
   <Border IsVisible="{Binding IsEditDialogVisible}">
       <views:EditInventoryView DataContext="{Binding EditDialogViewModel}" />
   </Border>
   ```

### Phase 3: ViewModel Updates

1. **Update TransferItemViewModel**:
   - Add `[ObservableProperty]` attributes for new properties
   - Implement `[RelayCommand]` methods for column customization
   - Add EditInventoryView integration logic

2. **Add service dependencies**:

   ```csharp
   public TransferItemViewModel(
       ILogger<TransferItemViewModel> logger,
       ITransferService transferService,
       IColumnConfigurationService columnService)
       : base(logger)
   {
       // Constructor implementation
   }
   ```

### Phase 4: Service Implementation

1. **Create ColumnConfigurationService**:
   - Implement MySQL usr_ui_settings integration
   - Add JSON serialization/deserialization
   - Implement caching for session performance

2. **Update TransferService**:
   - Add quantity auto-capping logic
   - Implement inventory splitting for partial transfers
   - Add validation methods

### Phase 5: Testing Validation

#### Unit Tests

```powershell
# Run unit tests for ViewModels
dotnet test --filter "Category=Unit&TestClass=*TransferTabViewModel*"

# Verify MVVM Community Toolkit patterns
dotnet test --filter "TestName=*ObservableProperty*"
```

#### Integration Tests

```powershell
# Test database operations
dotnet test --filter "Category=Integration&TestClass=*ColumnConfiguration*"

# Test transfer service operations
dotnet test --filter "Category=Integration&TestClass=*Transfer*"
```

#### UI Automation Tests

```powershell
# Test DataGrid functionality
dotnet test --filter "Category=UI&TestName=*DataGrid*"

# Test EditInventoryView integration
dotnet test --filter "Category=UI&TestName=*EditInventory*"
```

## Manual Testing Scenarios

### Scenario 1: Basic Transfer Operation

1. Launch application and navigate to Transfer tab
2. Enter Part ID: "TEST001" and Operation: "90"
3. Click Search - verify DataGrid displays results
4. Select inventory row, enter destination location
5. Enter transfer quantity ≤ available quantity
6. Click Transfer - verify success message and inventory update

### Scenario 2: Column Customization

1. Click column customization dropdown
2. Deselect "Notes" column
3. Verify DataGrid updates to hide Notes column
4. Restart application
5. Verify column preference persisted (Notes still hidden)

### Scenario 3: EditInventoryView Integration

1. Double-click inventory row in DataGrid
2. Verify EditInventoryView overlay displays
3. Make changes and save
4. Verify overlay closes and DataGrid refreshes
5. Execute transfer operation
6. Verify EditInventoryView auto-closes after successful transfer

### Scenario 4: Error Handling

1. Disconnect database connection
2. Attempt transfer operation
3. Verify error message displays with manual retry option
4. Reconnect database and retry
5. Verify operation completes successfully

### Scenario 5: Cross-Platform Validation

1. Test on Windows (primary development platform)
2. Test on macOS (if available)
3. Test on Linux (if available)
4. Verify consistent UI behavior and styling
5. Verify Theme V2 DynamicResource bindings work across platforms

## Performance Validation

### Memory Usage

```powershell
# Monitor memory during 8-hour simulation
dotnet run --configuration Release
# Use Task Manager/Activity Monitor to track memory growth
# Expected: <50MB/hour growth rate
```

### Database Performance

```powershell
# Run database stress test
dotnet test --filter "Category=Performance&TestName=*Database*"
# Expected: All operations complete within 5 seconds (warning at 30 seconds)
```

### UI Responsiveness

```powershell
# Test concurrent operations
dotnet test --filter "Category=Performance&TestName=*Concurrent*"
# Expected: UI thread remains responsive during background operations
```

## Troubleshooting

### Common Issues

#### AVLN2000 Compilation Errors

- **Cause**: Using `Name` instead of `x:Name` on Grid controls
- **Solution**: Replace all `Name=` with `x:Name=` in AXAML files

#### Theme Resources Not Found

- **Cause**: Missing DynamicResource bindings or incorrect resource keys
- **Solution**: Verify all styling uses `{DynamicResource ThemeV2.*}` patterns

#### Database Connection Failures

- **Cause**: Incorrect connection string or MySQL server not running
- **Solution**: Verify connection string in appsettings.json and MySQL service status

#### EditInventoryView Not Displaying

- **Cause**: DataContext not properly inherited or binding errors
- **Solution**: Verify `DataContext="{Binding EditDialogViewModel}"` and ViewModel property exists

### Debug Commands

```powershell
# Enable detailed logging
$env:MTM_LOG_LEVEL="Debug"
dotnet run

# Check database connectivity
dotnet run --verify-connection

# Validate AXAML syntax
dotnet build --verbosity detailed
```

## Success Criteria Checklist

- [ ] TransferCustomDataGrid completely removed
- [ ] Standard Avalonia DataGrid implemented with customizable columns
- [ ] Column preferences persist in MySQL usr_ui_settings table
- [ ] EditInventoryView integrates seamlessly with proper triggers
- [ ] All styling uses MTM Theme V2 DynamicResource bindings
- [ ] Quantity auto-capping prevents user input errors
- [ ] Transfer operations handle partial quantities with inventory splitting
- [ ] Single transaction audit trail recorded for all transfers
- [ ] Database connectivity failures show error with manual retry
- [ ] Cross-platform compatibility maintained
- [ ] All unit, integration, and UI tests pass
- [ ] Performance benchmarks met (startup <10s, operations <30s, memory <50MB/hour)
- [ ] Manufacturing operator workflows optimized (minimal clicks, double-click edit)
