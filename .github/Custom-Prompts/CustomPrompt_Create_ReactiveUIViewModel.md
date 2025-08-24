# Custom Prompt: Create ReactiveUI ViewModel

## ?? **Instructions**
Use this prompt when you need to generate ViewModels that follow ReactiveUI patterns with proper observable properties, commands, and reactive programming paradigms. This prompt creates ViewModels that integrate seamlessly with Avalonia views and follow MTM architectural standards.

## ?? **Persona**
**ViewModel Generator Copilot** - Specializes in creating ReactiveUI ViewModels with proper observable patterns, reactive commands, and MVVM architecture compliance.

## ?? **Prompt Template**

```
Act as ViewModel Generator Copilot. Create a ReactiveUI ViewModel for [VIEWMODEL_NAME] with the following requirements:

**Purpose:** [Describe what this ViewModel manages]
**Properties:** [List observable properties needed]
**Commands:** [List commands and their purposes]
**Data Operations:** [Describe any data loading/saving operations]
**Validation:** [Specify any validation requirements]
**Events:** [List any events this ViewModel should raise]

**Requirements:**
- Inherit from ReactiveObject
- Use RaiseAndSetIfChanged for all observable properties
- Create ReactiveCommand instances for all user actions
- Implement WhenAnyValue for computed properties and validation
- Include proper error handling with ThrownExceptions subscription
- Add constructor with dependency injection parameters
- Leave database operations as TODO comments with stored procedure patterns
- Include logging statements for debugging and audit trails

**MTM-Specific Requirements:**
- Operations are string numbers ("90", "100", "110") representing workflow steps
- Part IDs are strings (e.g., "PART001")
- Quantities are integers with positive validation
- TransactionType determined by user intent, not operation numbers
- Use MTM data patterns for inventory operations

**Additional Context:** [Any specific business logic or integration requirements]
```

## ?? **Purpose**
This prompt generates ReactiveUI ViewModels that properly implement the MVVM pattern with reactive programming paradigms, error handling, dependency injection preparation, and MTM-specific business rules.

## ?? **Usage Examples**

### **Example 1: Creating an Inventory Management ViewModel**
```
Act as ViewModel Generator Copilot. Create a ReactiveUI ViewModel for InventoryManagementViewModel with the following requirements:

**Purpose:** Manages inventory operations including adding, removing, and transferring parts
**Properties:** PartId (string), Operation (string), Quantity (int), Location (string), IsLoading (bool), ErrorMessage (string)
**Commands:** AddPartCommand, RemovePartCommand, TransferPartCommand, LoadInventoryCommand, ClearErrorCommand
**Data Operations:** Load current inventory, validate part operations, execute inventory transactions
**Validation:** PartId required, Operation must be valid number, Quantity must be positive, Location required
**Events:** PartAdded, PartRemoved, PartTransferred, ErrorOccurred

**Requirements:**
- Inherit from ReactiveObject
- Use RaiseAndSetIfChanged for all observable properties
- Create ReactiveCommand instances for all user actions
- Implement WhenAnyValue for computed properties and validation
- Include proper error handling with ThrownExceptions subscription
- Add constructor with dependency injection parameters
- Leave database operations as TODO comments with stored procedure patterns
- Include logging statements for debugging and audit trails

**MTM-Specific Requirements:**
- Operations are string numbers ("90", "100", "110") representing workflow steps
- Part IDs are strings (e.g., "PART001")
- Quantities are integers with positive validation
- TransactionType determined by user intent, not operation numbers
- Use MTM data patterns for inventory operations

**Additional Context:** ViewModel should support bulk operations and maintain operation history for audit purposes
```

### **Example 2: Creating a Configuration Settings ViewModel**
```
Act as ViewModel Generator Copilot. Create a ReactiveUI ViewModel for SettingsViewModel with the following requirements:

**Purpose:** Manages application configuration settings and preferences
**Properties:** DatabaseConnectionString (string), LogLevel (LogLevel enum), AutoSaveEnabled (bool), Theme (string), IsDirty (bool computed)
**Commands:** SaveSettingsCommand, ResetToDefaultsCommand, TestConnectionCommand, LoadSettingsCommand
**Data Operations:** Load settings from configuration, save settings to file, validate database connection
**Validation:** Connection string format validation, log level enum validation
**Events:** SettingsChanged, SettingsSaved, ConnectionTested

**Requirements:**
- Inherit from ReactiveObject
- Use RaiseAndSetIfChanged for all observable properties
- Create ReactiveCommand instances for all user actions
- Implement WhenAnyValue for computed properties and validation
- Include proper error handling with ThrownExceptions subscription
- Add constructor with dependency injection parameters
- Leave database operations as TODO comments with stored procedure patterns
- Include logging statements for debugging and audit trails

**MTM-Specific Requirements:**
- Operations are string numbers ("90", "100", "110") representing workflow steps
- Part IDs are strings (e.g., "PART001")
- Quantities are integers with positive validation
- TransactionType determined by user intent, not operation numbers
- Use MTM data patterns for inventory operations

**Additional Context:** Settings should support real-time validation and preview changes before saving
```

## ?? **Guidelines**

### **Technical Requirements**
- Always inherit from `ReactiveObject`
- Use `RaiseAndSetIfChanged` for all mutable properties
- Create `ReactiveCommand` instances for all user actions
- Implement computed properties using `WhenAnyValue` and `ToProperty`
- Include centralized error handling with `ThrownExceptions` subscription
- Prepare constructors for dependency injection

### **ReactiveUI Patterns**
```csharp
// Observable Property Pattern
private string _partId = string.Empty;
public string PartId
{
    get => _partId;
    set => this.RaiseAndSetIfChanged(ref _partId, value);
}

// Computed Property Pattern
private readonly ObservableAsPropertyHelper<bool> _canExecute;
public bool CanExecute => _canExecute.Value;

// Command Pattern
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

// Constructor Pattern
public SampleViewModel(ILogger<SampleViewModel> logger)
{
    // Initialize computed properties
    _canExecute = this.WhenAnyValue(vm => vm.PartId, partId => !string.IsNullOrWhiteSpace(partId))
                      .ToProperty(this, vm => vm.CanExecute);

    // Initialize commands
    LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);

    // Error handling
    LoadDataCommand.ThrownExceptions.Subscribe(ex => 
    {
        // TODO: Log error and display user-friendly message
    });
}
```

### **MTM-Specific Data Patterns**
- **Part ID**: Always string format (e.g., "PART001")
- **Operation**: String numbers representing workflow steps ("90", "100", "110")
- **Quantity**: Integer with positive validation
- **TransactionType**: Determined by user intent (IN/OUT/TRANSFER), not operation numbers
- **1-Based Indexing**: UI positions use 1-based indexing for display

### **Error Handling Standards**
- Subscribe to `ThrownExceptions` for all commands
- Log errors with appropriate detail level
- Display user-friendly error messages
- Maintain application stability during error conditions
- Clear error state when user takes corrective action

## ?? **Related Files**
- [../Core-Instructions/codingconventions.instruction.md](../Core-Instructions/codingconventions.instruction.md) - ReactiveUI and MVVM patterns
- [../Core-Instructions/dependency-injection.instruction.md](../Core-Instructions/dependency-injection.instruction.md) - DI patterns and service registration
- [../Development-Instructions/database-patterns.instruction.md](../Development-Instructions/database-patterns.instruction.md) - MTM business logic and database patterns
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - ViewModel Generator Copilot persona details

## ? **Quality Checklist**

### **ReactiveUI Implementation**
- [ ] Inherits from ReactiveObject
- [ ] All mutable properties use RaiseAndSetIfChanged
- [ ] Commands are ReactiveCommand instances
- [ ] Computed properties use WhenAnyValue and ToProperty
- [ ] Error handling implemented for all commands

### **MVVM Compliance**
- [ ] No direct UI dependencies in ViewModel
- [ ] Business logic separated from presentation logic
- [ ] Proper data binding support through observable properties
- [ ] Commands expose user actions appropriately

### **MTM Business Rules**
- [ ] Operations treated as string numbers (workflow steps)
- [ ] Part IDs use string format
- [ ] Quantities validated as positive integers
- [ ] TransactionType logic based on user intent
- [ ] MTM data patterns consistently applied

### **Code Quality**
- [ ] Constructor prepared for dependency injection
- [ ] Database operations left as TODO comments with stored procedure patterns
- [ ] Proper logging statements included
- [ ] Error handling comprehensive and user-friendly
- [ ] Validation logic appropriate and complete