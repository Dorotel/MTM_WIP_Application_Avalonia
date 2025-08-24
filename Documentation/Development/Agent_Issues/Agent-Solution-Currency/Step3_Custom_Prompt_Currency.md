# **?? Step 3: Custom Prompt Currency**

**Phase:** Custom Prompt Currency (High Priority Maintenance)  
**Priority:** HIGH - Ensures prompt accuracy and usability with current patterns  
**Links to:** [MasterPrompt.md](MasterPrompt.md) | [ContinueWork.md](ContinueWork.md)  
**Depends on:** [Step2_Critical_Fix_Implementation.md](Step2_Critical_Fix_Implementation.md)

---

## **?? Step Overview**

Update all custom prompts to reflect current database integration patterns, service layer implementations, and MTM business rules. This step ensures that all development guidance remains accurate and useful after the service layer integration work.

---

## **?? Sub-Steps**

### **Step 3.1: Custom Prompt Inventory and Assessment**

**Objective:** Catalog all custom prompts and assess currency against current implementation

**Inventory Process:**
```
CUSTOM PROMPT CURRENCY ASSESSMENT:

?? Development/Custom_Prompts/:
   - CustomPrompt_Create_UIElement.md
   - CustomPrompt_Create_UIElementFromMarkdown.md
   - CustomPrompt_Create_ReactiveUIViewModel.md
   - CustomPrompt_Create_ModernLayoutPattern.md
   - CustomPrompt_Create_ErrorSystemPlaceholder.md
   - CustomPrompt_Verify_CodeCompliance.md
   - CustomPrompt_Implement_ResultPatternSystem.md
   - Compliance_Fix01_EmptyDevelopmentStoredProcedures.md ? COMPLETED
   - Compliance_Fix02_MissingStandardOutputParameters.md
   - Compliance_Fix03_InadequateErrorHandlingStoredProcedures.md
   - Compliance_Fix04_MissingServiceLayerDatabaseIntegration.md
   - Compliance_Fix05_MissingDataModelsAndDTOs.md
   - Compliance_Fix06_MissingDependencyInjectionContainer.md
   - [Additional compliance fixes and prompts...]

?? .github/customprompts.instruction.md:
   - Master index of all prompts
   - Categorized organization
   - Usage guidelines and patterns

?? Assessment Criteria:
   - Database integration examples accurate?
   - Service patterns match AddMTMServices implementation?
   - MTM business rules correctly represented?
   - Error handling patterns current?
   - Code examples compilable and correct?
```

**Assessment Framework:**
```markdown
# Custom Prompt Currency Assessment

## Prompt Accuracy Categories

### ? CURRENT (No Updates Needed)
- Prompt accurately reflects current implementation
- Examples compile and work correctly
- Business rules properly represented
- Integration patterns are current

### ?? NEEDS MINOR UPDATES (Quick Fixes)
- Examples need database integration updates
- Service injection patterns need updating
- Minor pattern corrections needed
- Documentation references need updating

### ?? NEEDS MAJOR UPDATES (Significant Changes)
- Core patterns have changed
- Database integration completely different
- Service patterns incompatible
- Business logic corrections needed

### ? OBSOLETE (Remove or Archive)
- Patterns no longer used
- Superseded by new implementations
- Incorrect guidance that blocks development
- Contradicts established patterns
```

### **Step 3.2: Database Integration Pattern Updates**

**Objective:** Update all prompts with current database integration patterns

**Update Requirements:**
```
DATABASE INTEGRATION PROMPT UPDATES:

?? Service Layer Prompts:
   - Update to use AddMTMServices pattern
   - Include stored procedure integration examples
   - Add error handling with status codes
   - Include logging patterns with database integration

?? ViewModel Prompts:
   - Update constructor injection patterns
   - Include service resolution examples
   - Add async/await patterns for database operations
   - Update ReactiveUI patterns with service integration

?? Error Handling Prompts:
   - Update to use database error logging
   - Include status code handling patterns
   - Add validation procedure integration
   - Update exception handling with database logging

?? Data Model Prompts:
   - Update validation patterns using stored procedures
   - Include DTO examples with database integration
   - Add MTM business rule enforcement
   - Update serialization patterns
```

**Required Updates to Key Prompts:**

#### **CustomPrompt_Create_ReactiveUIViewModel.md Updates**
```markdown
## Updated ViewModel Template (Post-Service Integration)

```csharp
public class {Name}ViewModel : ReactiveObject
{
    private readonly MTM.Services.I{ServiceName}Service _{serviceName}Service;
    private readonly MTM.Core.Services.IValidationService _validationService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<{Name}ViewModel> _logger;

    public {Name}ViewModel(
        MTM.Services.I{ServiceName}Service {serviceName}Service,
        MTM.Core.Services.IValidationService validationService,
        INavigationService navigationService,
        ILogger<{Name}ViewModel> logger)
    {
        _{serviceName}Service = {serviceName}Service ?? throw new ArgumentNullException(nameof({serviceName}Service));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeCommands();
        _logger.LogInformation("{ViewModelName} initialized with dependency injection");
    }

    private void InitializeCommands()
    {
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                _logger.LogInformation("Loading data using database integration");
                var result = await _{serviceName}Service.GetDataAsync();
                
                if (result.IsSuccess)
                {
                    // Process successful result
                    Items = new ObservableCollection<ItemViewModel>(
                        result.Value.Select(item => new ItemViewModel(item))
                    );
                    _logger.LogInformation("Data loaded successfully: {Count} items", Items.Count);
                }
                else
                {
                    _logger.LogWarning("Data loading failed: {Error}", result.ErrorMessage);
                    // Handle error using database error logging
                    await HandleErrorAsync(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during data loading");
                await HandleErrorAsync(ex.Message);
            }
        });
    }

    private async Task HandleErrorAsync(string errorMessage)
    {
        // Log error to database using stored procedure
        // Display user-friendly message
        // Navigate to error state if needed
    }
}
```
```

#### **CustomPrompt_Verify_CodeCompliance.md Updates**
```markdown
## Updated Compliance Verification (Post-Database Integration)

### Additional Compliance Areas to Review

#### 5. Service Integration Violations
**Reference**: AddMTMServices pattern from copilot-instructions.md
- Incorrect service registration (individual vs. AddMTMServices)
- Missing dependency injection in ViewModels
- Service lifetime violations (Singleton vs. Scoped vs. Transient)
- Missing service interface implementations

#### 6. Database Integration Violations  
**Reference**: Critical Fix #1 database procedures
- Direct SQL usage instead of stored procedures
- Missing error handling with status codes
- Incorrect async/await patterns
- Missing database logging integration

#### 7. MTM Business Logic Violations (UPDATED)
**Reference**: Updated business rules in copilot-instructions.md
- TransactionType determination errors (CRITICAL: Based on user intent, NOT operation)
- Missing validation procedure integration
- Incorrect data model usage
- Missing business rule enforcement

### Updated Priority Classification

#### **CRITICAL Priority** (Updated with Service Integration)
- Missing AddMTMServices pattern usage
- Direct SQL instead of stored procedures
- Service resolution failures
- Missing dependency injection in ViewModels
- TransactionType determination errors

### Updated Report Format Template
[Include sections for service integration compliance, database pattern compliance, and dependency injection compliance]
```

### **Step 3.3: Service Pattern Updates**

**Objective:** Update prompts to reflect AddMTMServices and dependency injection patterns

**Service Pattern Updates:**
```
SERVICE PATTERN PROMPT UPDATES:

?? Dependency Injection Patterns:
   - Update all DI examples to use AddMTMServices
   - Include service resolution validation
   - Add constructor injection examples
   - Update service lifetime documentation

?? Service Interface Patterns:
   - Update service interface examples
   - Include async method signatures
   - Add Result<T> pattern integration
   - Update error handling interfaces

?? Service Implementation Patterns:
   - Update with stored procedure integration
   - Include logging and validation patterns
   - Add transaction management examples
   - Update async/await best practices

?? ViewModel Service Integration:
   - Update constructor injection patterns
   - Include service method calling examples
   - Add error handling integration
   - Update navigation service usage
```

**Key Service Pattern Updates:**

#### **Service Registration Pattern (Critical Update)**
```csharp
// ? OLD PATTERN (Remove from all prompts):
services.AddScoped<MTM.Services.IInventoryService, MTM.Services.InventoryService>();

// ? NEW PATTERN (Update all prompts):
services.AddMTMServices(configuration);

// ? Avalonia-specific overrides AFTER AddMTMServices:
services.AddSingleton<MTM_WIP_Application_Avalonia.Services.IConfigurationService, 
                     MTM_WIP_Application_Avalonia.Services.ConfigurationService>();
```

#### **Service Method Pattern Updates**
```csharp
// Update all service method examples:
public async Task<Result<List<InventoryItem>>> GetAllItemsAsync()
{
    try
    {
        var parameters = new Dictionary<string, object>();
        
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Get_All",
            parameters
        );

        if (result.Status == "Success")
        {
            var items = ParseDataTableToInventoryItems(result.Data);
            return Result<List<InventoryItem>>.Success(items);
        }
        else
        {
            _logger.LogWarning("Database operation failed: {Error}", result.ErrorMessage);
            return Result<List<InventoryItem>>.Failure(result.ErrorMessage);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Exception in GetAllItemsAsync");
        return Result<List<InventoryItem>>.Failure($"Database error: {ex.Message}");
    }
}
```

### **Step 3.4: MTM Business Rule Updates**

**Objective:** Ensure all prompts correctly represent MTM business logic

**Business Rule Updates:**
```
MTM BUSINESS RULE PROMPT UPDATES:

?? TransactionType Determination (CRITICAL):
   - Update ALL examples to show user intent determination
   - Remove any operation-based transaction type logic
   - Add correct business rule examples
   - Include validation examples

?? Data Pattern Updates:
   - Part ID validation patterns
   - Operation workflow step documentation  
   - Quantity validation rules
   - Location management patterns

?? Workflow Integration:
   - Manufacturing process integration
   - Inventory tracking patterns
   - User authentication patterns
   - Audit logging requirements

?? Validation Rule Updates:
   - Entity validation procedure integration
   - Business rule enforcement examples
   - Data integrity patterns
   - Error message standardization
```

**Critical Business Rule Corrections:**

#### **TransactionType Logic (Must Update All Prompts)**
```csharp
// ? REMOVE THIS PATTERN FROM ALL PROMPTS:
private static TransactionType GetTransactionType(string operation)
{
    return operation switch
    {
        "90" => TransactionType.IN,    // WRONG!
        "100" => TransactionType.OUT,  // WRONG!
        "110" => TransactionType.TRANSFER, // WRONG!
        _ => TransactionType.OTHER
    };
}

// ? ADD THIS PATTERN TO ALL RELEVANT PROMPTS:
public async Task<Result> AddStockAsync(string partId, string operation, int quantity, string location, string userId)
{
    // TransactionType is ALWAYS based on user intent, NOT operation
    var transaction = new InventoryTransaction
    {
        TransactionType = TransactionType.IN, // User is adding stock
        Operation = operation, // Just a workflow step number (e.g., "90", "100", "110")
        PartId = partId,
        Quantity = quantity,
        Location = location,
        UserId = userId
    };
    
    // Use stored procedure for transaction
    return await ProcessInventoryTransactionAsync(transaction);
}
```

### **Step 3.5: Validation and Testing Framework**

**Objective:** Create framework to validate prompt accuracy and maintain currency

**Validation Framework:**
```
PROMPT VALIDATION FRAMEWORK:

?? Code Example Testing:
   - Extract all code examples from prompts
   - Verify syntax correctness
   - Test compilation against current codebase
   - Validate service resolution patterns

?? Pattern Compliance Testing:
   - Check AddMTMServices usage in examples
   - Verify stored procedure patterns
   - Validate MTM business rule examples
   - Test error handling patterns

?? Integration Testing:
   - Test service injection examples
   - Verify database integration patterns
   - Validate ViewModel construction examples
   - Test command implementation patterns

?? Documentation Accuracy:
   - Cross-reference with instruction files
   - Verify links and references
   - Check consistency with implementation
   - Validate example accuracy
```

**Validation Implementation:**
```powershell
# PowerShell script: Validate-CustomPrompts.ps1
param(
    [string]$PromptsDirectory = "Development/Custom_Prompts",
    [string]$OutputReport = "prompt_validation_report.md"
)

Write-Host "Validating Custom Prompts for Currency..."

# Extract and validate code examples
$PromptFiles = Get-ChildItem -Path $PromptsDirectory -Filter "*.md"
$ValidationResults = @()

foreach ($File in $PromptFiles) {
    Write-Host "Validating: $($File.Name)"
    
    # Extract code blocks
    $Content = Get-Content $File.FullName -Raw
    $CodeBlocks = [regex]::Matches($Content, '```csharp\r?\n(.*?)\r?\n```', [System.Text.RegularExpressions.RegexOptions]::Singleline)
    
    foreach ($Block in $CodeBlocks) {
        $Code = $Block.Groups[1].Value
        
        # Check for patterns
        $HasAddMTMServices = $Code -match "AddMTMServices"
        $HasDirectSQL = $Code -match "(SELECT|INSERT|UPDATE|DELETE).*FROM"
        $HasStoredProcedure = $Code -match "Helper_Database_StoredProcedure"
        $HasCorrectTransactionType = $Code -match 'TransactionType\.(IN|OUT|TRANSFER).*// User'
        
        $ValidationResults += [PSCustomObject]@{
            File = $File.Name
            HasAddMTMServices = $HasAddMTMServices
            HasDirectSQL = $HasDirectSQL
            HasStoredProcedure = $HasStoredProcedure
            HasCorrectTransactionType = $HasCorrectTransactionType
            CodeBlock = $Code.Substring(0, [Math]::Min(100, $Code.Length))
        }
    }
}

# Generate validation report
$Report = @"
# Custom Prompt Validation Report
Generated: $(Get-Date)

## Summary
- Files Validated: $($PromptFiles.Count)
- Code Blocks Analyzed: $($ValidationResults.Count)

## Validation Results
$(
    $ValidationResults | ForEach-Object {
        "### $($_.File)
- AddMTMServices Pattern: $(if($_.HasAddMTMServices){'?'}else{'?'})
- No Direct SQL: $(if(-not $_.HasDirectSQL){'?'}else{'?'})
- Uses Stored Procedures: $(if($_.HasStoredProcedure){'?'}else{'?'})
- Correct TransactionType Logic: $(if($_.HasCorrectTransactionType){'?'}else{'?'})
"
    }
)

## Action Items
$(
    $ValidationResults | Where-Object { -not $_.HasAddMTMServices -or $_.HasDirectSQL -or -not $_.HasStoredProcedure } | ForEach-Object {
        "- ?? Update $($_.File): Pattern compliance issues"
    }
)
"@

$Report | Out-File -FilePath $OutputReport -Encoding UTF8
Write-Host "Validation complete. Report saved to: $OutputReport"
```

---

## **?? Integration with Master Process**

### **Links to MasterPrompt.md:**
- **Step 2:** Critical Fix Implementation (provides updated patterns)
- **Step 3:** Custom Prompt Currency (this step)
- **Step 4:** Documentation Synchronization (uses updated prompts)
- **Step 5:** Automated Currency Framework (monitors prompt accuracy)

### **Supports Subsequent Steps:**
- **Step 4:** Provides current prompts for documentation synchronization
- **Step 5:** Establishes prompt validation framework for ongoing monitoring

---

## **? Success Criteria**

**Step 3.1 Complete When:**
- ? All custom prompts inventoried and assessed
- ? Currency gaps identified and prioritized
- ? Validation framework designed
- ? Update requirements documented

**Step 3.2 Complete When:**
- ? Database integration patterns updated in all relevant prompts
- ? Stored procedure examples replace direct SQL
- ? Error handling patterns updated
- ? Service integration examples current

**Step 3.3 Complete When:**
- ? AddMTMServices pattern used in all service examples
- ? Dependency injection patterns updated
- ? Service method signatures current
- ? ViewModel injection patterns updated

**Step 3.4 Complete When:**
- ? TransactionType logic corrected in all prompts
- ? MTM business rules accurately represented
- ? Data validation patterns updated
- ? Workflow integration examples current

**Step 3.5 Complete When:**
- ? Validation framework implemented and tested
- ? All code examples verified for compilation
- ? Pattern compliance validated
- ? Documentation accuracy confirmed

---

## **?? Emergency Continuation**

**If this step is interrupted, use:**

```
EXECUTE STEP 3 CONTINUATION:

Act as Solution Currency Maintenance Copilot and Development Compliance Auditor Copilot.

1. ASSESS current Step 3 completion state:
   ?? Check custom prompt inventory and assessment progress
   ?? Review database integration pattern updates
   ?? Verify service pattern updates status
   ?? Check MTM business rule updates
   ?? Review validation framework implementation

2. IDENTIFY incomplete sub-step:
   - If 3.1 incomplete: Complete prompt inventory and assessment
   - If 3.2 incomplete: Finish database integration pattern updates
   - If 3.3 incomplete: Complete service pattern updates
   - If 3.4 incomplete: Finish MTM business rule corrections
   - If 3.5 incomplete: Implement validation framework

3. VALIDATE completion before proceeding to Step 4

CRITICAL: All prompts must reflect current AddMTMServices pattern and stored procedure usage.

BUSINESS RULE PRIORITY: TransactionType determination logic must be corrected in ALL relevant prompts.

VALIDATION REQUIREMENT: All code examples must compile and work with current service integration.
```

---

## **??? Technical Requirements**

- **Pattern Accuracy**: All examples must use current AddMTMServices and stored procedure patterns
- **Business Rule Compliance**: MTM business logic must be correctly represented
- **Code Validation**: All examples must compile and work with current implementation
- **Documentation Standards**: Consistent formatting and cross-referencing
- **Validation Framework**: Automated testing for prompt accuracy

**Estimated Time:** 6-8 hours  
**Risk Level:** LOW (documentation updates, no code changes)  
**Dependencies:** Step 2 completion, service layer integration patterns  
**Critical Path:** Ensures development guidance remains accurate and useful