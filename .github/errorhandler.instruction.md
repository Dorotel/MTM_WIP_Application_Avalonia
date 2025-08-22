<!-- Copilot: Reading error_handler-instruction.md — Error Handler Requirements and Instructions -->
# Error Handler Requirements and Instructions

> For prompt and persona standards related to error and logging, see [custom-prompts.instruction.md](custom-prompts.instruction.md) and [personas-instruction.md](personas-instruction.md).

## Logging to File Server

- **Each user** gets their own subfolder in a central folder on the work file server.
    - The application creates the subfolder automatically if it doesn't exist.
    - Subfolder naming is based on the user's identity (`userId` or equivalent).
- **Log files** are in CSV format, one file per error category (e.g., `ui_errors.csv`, `mysql_errors.csv`), per user.
    - Each error is logged as a new row.
    - The file includes a header row.
    - Do not log duplicate errors for the current session.

## Log Entry Details

Each log entry (CSV row) must include:
- Timestamp
- User ID
- Machine name (computer name)
- Error category
- Error message
- File name
- Method name
- Line number
- Stack trace

Include any extra information that would make it easier to diagnose and fix the error if the log entry was pasted into a chat window (as long as it doesn't duplicate existing fields).

## Error Categories

Errors are grouped into these categories:
- UI
- Business Logic
- MySQL
- Network
- Other

Each category has a specific user-facing message and recommended actions.

## MySQL Logging

- Log errors to MySQL as well.
- Each category has a separate table, but all tables use the same structure/columns as above.

## Duplicate Error Detection

- For the current application session, do not log the same error more than once (per user, per category).

## Additional Info

- The error handler should NOT capture process/thread details.
- The log files and database tables should be focused on user, machine, and error details only.

## Configuration Source

- Primary configuration file: `Config/appsettings.json`
  - Error handling settings are located in the `ErrorHandling` section.
  - Implement loading in `ErrorHandlingConfiguration.LoadFromConfiguration()` to read:
    - `EnableFileServerLogging`
    - `EnableMySqlLogging`
    - `EnableConsoleLogging`
    - `FileServerBasePath`
    - `MySqlConnectionString`

---

## ReactiveUI Error Handling Patterns

### Command Error Handling
All ReactiveCommand implementations should include centralized error handling using the ThrownExceptions observable:

```csharp
public ReactiveCommand<Unit, Unit> PerformOperationCommand { get; }
public ReactiveCommand<Unit, Unit> SaveCommand { get; }
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

public SampleViewModel()
{
    // Async command
    LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        // Operation
        await Task.CompletedTask;
    });

    // Sync command with CanExecute
    var canSave = this.WhenAnyValue(vm => vm.SomeProperty, s => !string.IsNullOrWhiteSpace(s));
    SaveCommand = ReactiveCommand.Create(() =>
    {
        // TODO: Implement
    }, canSave);

    // Centralized error handling for all commands
    LoadDataCommand.ThrownExceptions
        .Merge(SaveCommand.ThrownExceptions)
        .Subscribe(ex =>
        {
            // TODO: Log to MySQL and file using Service_ErrorHandler
            // Service_ErrorHandler.HandleException(ex, ErrorSeverity.Medium, "ViewName_CommandName");
            // Show user-friendly message using Control_ErrorMessage or ErrorDialog_Enhanced
        });
}
```

### Command Error Patterns
- **ThrownExceptions Observable**: Subscribe to command exceptions using `.ThrownExceptions.Subscribe()`
- **Merge Multiple Commands**: Use `.Merge()` to handle errors from multiple commands in one place
- **Error Context**: Include command name and control context when logging errors
- **User Feedback**: Always provide user-friendly error messages through UI components

### Reactive Property Error Handling
```csharp
// For properties that can cause validation errors
private readonly ObservableAsPropertyHelper<string> _validationMessage;
public string ValidationMessage => _validationMessage.Value;

public SampleViewModel()
{
    _validationMessage = this.WhenAnyValue(vm => vm.SomeProperty)
        .Select(value => ValidateProperty(value))
        .Catch(Observable.Return("Validation error occurred"))
        .ToProperty(this, vm => vm.ValidationMessage, initialValue: string.Empty);
}

private string ValidateProperty(string value)
{
    try
    {
        // Validation logic
        return string.Empty;
    }
    catch (Exception ex)
    {
        // Log validation errors
        Service_ErrorHandler.HandleException(ex, ErrorSeverity.Low, "PropertyValidation");
        return "Invalid input";
    }
}
```

---

## Avalonia UI Error Display Patterns

### Inline Error Display (Non-blocking)
Use `Controls/Control_ErrorMessage` for inline error presentation:

```csharp
// In ViewModel or code-behind
var errorControl = new Control_ErrorMessage();
errorControl.Initialize(ex, ErrorSeverity.Medium, "MyView_Button_Save");

// Add to existing container
parentContainer.Children.Add(errorControl);
```

### Modal Error Display (Blocking)
Use `Views/ErrorDialog_Enhanced` for detailed error analysis:

```csharp
// Show modal error dialog
await ErrorDialog_Enhanced.ShowErrorAsync(this, ex, ErrorSeverity.High, "MyView_Button_Save");
```

### Error UI Integration in AXAML
```xml
<!-- Placeholder for inline error messages -->
<StackPanel x:Name="ErrorContainer" 
           Orientation="Vertical" 
           Spacing="8"
           Margin="0,8,0,0"/>

<!-- Error message binding in ViewModels -->
<Border IsVisible="{Binding HasError}"
        Classes="error-container">
    <TextBlock Text="{Binding ErrorMessage}"
               Classes="error-text"/>
</Border>
```

### Error Severity Visual Mapping
- **Low**: Information icon, light blue accent
- **Medium**: Warning icon, orange/yellow accent  
- **High**: Error icon, red accent
- **Critical**: Critical icon, dark red accent with elevated styling

---

## MTM-Specific Error Scenarios

### Inventory System Errors
- **Part Not Found**: When scanning non-existent part IDs
- **Operation Invalid**: When operation numbers don't match part routing
- **Quantity Errors**: Negative quantities, exceeding available stock
- **Database Connection**: MySQL connectivity issues during inventory updates
- **File Server Access**: CSV export/import failures to network drives

### MTM Data Validation Errors
- **Part ID Format**: Invalid part number patterns
- **Operation Numbers**: Non-numeric or out-of-range operation values
- **User Authorization**: Insufficient permissions for specific operations
- **Session Timeouts**: Extended idle periods requiring re-authentication

### Business Logic Error Categories
```csharp
// MTM-specific error categories
public enum MTMErrorCategory
{
    InventoryData,
    PartValidation,
    OperationRouting,
    UserAuthorization,
    DatabaseAccess,
    FileServerAccess,
    NetworkConnectivity,
    SessionManagement
}
```

---

## Theme-Aware Error Styling

### MTM Purple Color Scheme Integration
Apply MTM brand colors to error UI components:

```xml
<!-- Error styling using MTM color palette -->
<Style Selector="Border.error-container">
    <Setter Property="Background" Value="#FFF5F5"/>
    <Setter Property="BorderBrush" Value="#ED45E7"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="CornerRadius" Value="6"/>
    <Setter Property="Padding" Value="12"/>
</Style>

<Style Selector="TextBlock.error-text">
    <Setter Property="Foreground" Value="#ED45E7"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
</Style>

<!-- Warning styling -->
<Style Selector="Border.warning-container">
    <Setter Property="Background" Value="#FFFBF5"/>
    <Setter Property="BorderBrush" Value="#BA45ED"/>
    <Setter Property="BorderThickness" Value="1"/>
</Style>

<!-- Info styling -->
<Style Selector="Border.info-container">
    <Setter Property="Background" Value="#F5F8FF"/>
    <Setter Property="BorderBrush" Value="#4574ED"/>
    <Setter Property="BorderThickness" Value="1"/>
</Style>
```

### Dynamic Resource Usage
```xml
<!-- Use dynamic resources for theme switching -->
<Border Background="{DynamicResource ErrorBackgroundBrush}"
        BorderBrush="{DynamicResource ErrorBorderBrush}">
    <TextBlock Foreground="{DynamicResource ErrorForegroundBrush}"
               Text="{Binding ErrorMessage}"/>
</Border>
```

### Severity-Based Theme Resources
```xml
<!-- Define in App.axaml or theme files -->
<SolidColorBrush x:Key="ErrorHighBrush" Color="#ED45E7"/>      <!-- Pink Accent -->
<SolidColorBrush x:Key="ErrorMediumBrush" Color="#BA45ED"/>    <!-- Magenta Accent -->
<SolidColorBrush x:Key="ErrorLowBrush" Color="#4574ED"/>       <!-- Blue Accent -->
<SolidColorBrush x:Key="ErrorCriticalBrush" Color="#8345ED"/>  <!-- Secondary Purple -->

<!-- Background variations -->
<SolidColorBrush x:Key="ErrorBackgroundBrush" Color="#FFF5F8"/>
<SolidColorBrush x:Key="WarningBackgroundBrush" Color="#FFFBF5"/>
<SolidColorBrush x:Key="InfoBackgroundBrush" Color="#F5F8FF"/>
```

---

## Reference Implementation in This Repository

This repository includes a scaffolded error system that follows the above requirements:

- Services:
  - `Service_ErrorHandler` — central exception handling and session-level de-duplication.
  - `LoggingUtility` — CSV file logging (per-user, per-category) and MySQL logging stubs (with table ensure).
  - `ErrorEntry`, `ErrorCategory`, `ErrorSeverity` — shared models/enums.
  - `ErrorHandlingConfiguration` — configuration flags/paths and validation placeholders.
  - `ErrorMessageProvider` — user-facing messages, titles, and recommendations.
  - `ErrorHandlingInitializer` — startup helpers (production and development modes) and self-test.

- UI elements for user display (Avalonia):
  - `Controls/Control_ErrorMessage` — inline, non-blocking error UI control with MTM theme support.
  - `Views/ErrorDialog_Enhanced` — modal dialog for detailed error analysis and reporting.

### Usage

Initialize the system at app startup:
```csharp
ErrorHandlingInitializer.Initialize();
// or
ErrorHandlingInitializer.InitializeForDevelopment();
```

Log exceptions from any layer:
```csharp
try
{
    // risky operation
}
catch (Exception ex)
{
    Service_ErrorHandler.HandleException(
        ex,
        ErrorSeverity.Medium,
        source: "MyView_Button_Save",
        additionalData: new Dictionary<string, object> { ["Operation"] = "SaveData" });
}
```

Log from ReactiveCommand ThrownExceptions:
```csharp
SomeCommand.ThrownExceptions
    .Subscribe(ex =>
    {
        Service_ErrorHandler.HandleException(
            ex,
            ErrorSeverity.Medium,
            source: "ViewName_CommandName",
            additionalData: new Dictionary<string, object> 
            { 
                ["Command"] = nameof(SomeCommand),
                ["ViewModel"] = GetType().Name
            });
    });
```

Display error UI (when appropriate):
```csharp
// Inline
var ctrl = new Control_ErrorMessage();
ctrl.Initialize(ex, ErrorSeverity.Medium, "MyView_Button_Save");

// Modal
await ErrorDialog_Enhanced.ShowErrorAsync(this, ex, ErrorSeverity.High, "MyView_Button_Save");
```

CSV filenames by category (per user folder):
- UI → `ui_errors.csv`
- Business Logic → `business_logic_errors.csv`
- MySQL → `mysql_errors.csv`
- Network → `network_errors.csv`
- Other → `other_errors.csv`

> Note: The MySQL logging uses `MySql.Data` package and will need proper connection string configuration via `ErrorHandlingConfiguration`.