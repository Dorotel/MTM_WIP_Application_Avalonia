---
description: 'Manufacturing domain context for MTM WIP Application - inventory management, workflows, business rules'
context_type: 'business'
applies_to: '**/*'
priority: 'high'
---

# MTM Manufacturing Context - Business Domain Knowledge

## Context Overview

This context provides comprehensive knowledge about manufacturing inventory management workflows, business rules, and domain-specific requirements for the MTM WIP Application. Use this context to understand manufacturing processes, inventory operations, and business logic requirements.

## Manufacturing Domain Fundamentals

### Core Business Entities

#### Part Identification System
```csharp
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;        // "PART001", "ABC-123", "MOTOR_HOUSING_V2"
    public string Description { get; set; } = string.Empty;   // Human-readable part description
    public string Category { get; set; } = string.Empty;      // Part category for organization
    public bool IsActive { get; set; } = true;               // Part status in system
}
```

**Part ID Format Rules:**
- Alphanumeric with dashes allowed: `^[A-Za-z0-9\-]{1,50}$`
- Maximum 50 characters
- Case-insensitive but stored in uppercase
- Examples: "PART001", "ABC-123", "MOTOR-HOUSING-V2"

#### Manufacturing Operation Sequence
Manufacturing operations represent workflow steps, NOT transaction types:

```csharp
public static class ManufacturingOperations
{
    public const string Receiving = "90";        // Parts received into system
    public const string FirstOperation = "100";  // First manufacturing step
    public const string SecondOperation = "110"; // Second manufacturing step  
    public const string ThirdOperation = "120";  // Third manufacturing step
    public const string FinalOperation = "130";  // Final manufacturing step
    public const string Shipping = "140";        // Parts ready for shipping
}
```

**Critical Understanding: Operation Numbers ≠ Transaction Types**
- Operation numbers ("90", "100", "110") are **workflow steps**
- Transaction types ("IN", "OUT", "TRANSFER") are determined by **user intent**
- Same part can be in multiple operations simultaneously
- Operations represent WHERE the part is in the manufacturing process

#### Location Management System
```csharp
public class LocationInfo
{
    public string LocationId { get; set; } = string.Empty;    // "STATION_A", "WIP_001", "SHIPPING"
    public string Description { get; set; } = string.Empty;   // "Assembly Station A"
    public string Zone { get; set; } = string.Empty;          // "PRODUCTION", "SHIPPING", "RECEIVING"
    public bool IsActive { get; set; } = true;               // Location availability
}
```

**Location Types:**
- **Stations**: "STATION_A", "STATION_B" - Manufacturing work centers
- **WIP Areas**: "WIP_001", "WIP_002" - Work-in-process storage
- **Storage**: "WAREHOUSE_A", "STORAGE_B" - Inventory storage locations
- **Special**: "RECEIVING", "SHIPPING", "QUARANTINE" - Process-specific locations

## Transaction Logic and Business Rules

### Transaction Type Determination (CRITICAL)
Transaction types are determined by **user intent**, not operation numbers or system logic:

```csharp
public string DetermineTransactionType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => "IN",         // User adding inventory to system
        UserIntent.RemovingStock => "OUT",      // User removing inventory from system
        UserIntent.MovingStock => "TRANSFER",   // User moving between locations/operations
        UserIntent.AdjustingStock => "ADJUSTMENT", // Correcting inventory discrepancies
        UserIntent.ScrapStock => "SCRAP",       // Removing defective parts
        _ => throw new InvalidOperationException($"Unknown user intent: {action.Intent}")
    };
}
```

**Transaction Type Examples:**
- **"IN"**: Receiving parts, completing manufacturing step, returning from repair
- **"OUT"**: Shipping parts, moving to next operation, consuming in assembly
- **"TRANSFER"**: Moving between locations, changing operation assignments
- **"ADJUSTMENT"**: Cycle count corrections, system reconciliations
- **"SCRAP"**: Quality failures, damaged parts, obsolete inventory

### Inventory Calculation Rules
```csharp
public class InventoryCalculation
{
    public static int CalculateNewQuantity(int currentQuantity, int transactionQuantity, string transactionType)
    {
        var newQuantity = transactionType switch
        {
            "IN" => currentQuantity + transactionQuantity,
            "OUT" => currentQuantity - transactionQuantity,
            "ADJUSTMENT" => transactionQuantity, // Direct set to new value
            "SCRAP" => currentQuantity - transactionQuantity,
            "TRANSFER" => currentQuantity, // Transfer handles separately
            _ => throw new InvalidOperationException($"Unknown transaction type: {transactionType}")
        };
        
        // Business rule: Inventory cannot go negative
        if (newQuantity < 0)
        {
            throw new InvalidOperationException($"Transaction would result in negative inventory: {newQuantity}");
        }
        
        return newQuantity;
    }
}
```

### Manufacturing Workflow Patterns

#### Standard Manufacturing Flow
```
Parts Flow: RECEIVING(90) → OPERATION_1(100) → OPERATION_2(110) → OPERATION_3(120) → SHIPPING(140)

Inventory Tracking:
- Each operation maintains separate quantity tracking
- Parts can exist in multiple operations simultaneously
- Total WIP = Sum of all operation quantities for a part
- Completion moves parts from current operation to next operation
```

#### Common Workflow Scenarios

**Scenario 1: New Parts Receipt**
```csharp
// Parts received from supplier
var receiptTransaction = new InventoryTransaction
{
    PartId = "MOTOR_SHAFT_001",
    Operation = "90", // Receiving operation
    Quantity = 100,
    Location = "RECEIVING_DOCK",
    TransactionType = "IN", // Adding to system
    UserId = "RECEIVING_CLERK",
    Notes = "PO#12345 - Supplier ABC Corp"
};
```

**Scenario 2: Manufacturing Step Completion**
```csharp
// Parts completed at Operation 100, moving to Operation 110
var completionTransactions = new[]
{
    // Remove from current operation
    new InventoryTransaction
    {
        PartId = "MOTOR_SHAFT_001",
        Operation = "100",
        Quantity = 50,
        TransactionType = "OUT", // Removing from current operation
        Location = "STATION_A",
        UserId = "OPERATOR_001"
    },
    
    // Add to next operation
    new InventoryTransaction
    {
        PartId = "MOTOR_SHAFT_001", 
        Operation = "110",
        Quantity = 50,
        TransactionType = "IN", // Adding to next operation
        Location = "STATION_B",
        UserId = "OPERATOR_001"
    }
};
```

**Scenario 3: Quality Issue - Scrap Parts**
```csharp
// Defective parts found during inspection
var scrapTransaction = new InventoryTransaction
{
    PartId = "MOTOR_SHAFT_001",
    Operation = "110",
    Quantity = 5,
    TransactionType = "SCRAP",
    Location = "INSPECTION_STATION",
    UserId = "QUALITY_INSPECTOR",
    Notes = "Failed dimensional inspection - Out of tolerance"
};
```

## User Workflow Patterns

### QuickButtons Functionality
QuickButtons provide rapid access to frequently used transactions:

```csharp
public class QuickButtonLogic
{
    public static QuickButton GenerateFromRecentTransaction(TransactionRecord transaction)
    {
        return new QuickButton
        {
            PartId = transaction.PartId,
            Operation = transaction.Operation,
            Quantity = transaction.Quantity,
            Location = transaction.Location,
            TransactionType = transaction.TransactionType,
            DisplayText = $"{transaction.PartId} @ {transaction.Operation} ({transaction.Quantity})",
            CreatedFrom = "RecentTransaction",
            UserId = transaction.UserId,
            LastUsed = DateTime.Now
        };
    }
    
    public static bool ShouldCreateQuickButton(TransactionRecord transaction)
    {
        // Create QuickButton for transactions meeting criteria
        return transaction.Quantity >= 5 && // Minimum quantity threshold
               !string.IsNullOrEmpty(transaction.PartId) &&
               !string.IsNullOrEmpty(transaction.Operation) &&
               IsStandardOperation(transaction.Operation);
    }
}
```

### Operator Daily Workflow
```
1. Start Shift
   - Review WIP inventory at assigned stations
   - Check QuickButtons for common operations
   - Review any overnight transactions

2. Process Parts
   - Scan/enter part ID
   - Verify operation and location
   - Enter quantities processed
   - Record completion or move to next operation

3. Handle Exceptions
   - Report quality issues (scrap transactions)
   - Handle material shortages (transfer requests)
   - Adjust quantities for count discrepancies

4. End Shift
   - Complete pending transactions
   - Report any issues or anomalies
   - Pass information to next shift
```

## Data Validation Rules

### Part ID Validation
```csharp
public static class PartIdValidation
{
    public static ValidationResult ValidatePartId(string partId)
    {
        if (string.IsNullOrWhiteSpace(partId))
            return ValidationResult.Error("Part ID is required");
            
        if (partId.Length > 50)
            return ValidationResult.Error("Part ID cannot exceed 50 characters");
            
        if (!Regex.IsMatch(partId, @"^[A-Za-z0-9\-]{1,50}$"))
            return ValidationResult.Error("Part ID can only contain letters, numbers, and dashes");
            
        return ValidationResult.Success();
    }
}
```

### Quantity Validation
```csharp
public static class QuantityValidation
{
    public static ValidationResult ValidateQuantity(int quantity, string transactionType)
    {
        if (quantity <= 0)
            return ValidationResult.Error("Quantity must be greater than zero");
            
        if (quantity > 999999)
            return ValidationResult.Error("Quantity cannot exceed 999,999");
            
        // Special rules for certain transaction types
        if (transactionType == "ADJUSTMENT" && quantity == 0)
            return ValidationResult.Warning("Adjustment to zero quantity - confirm this is intended");
            
        return ValidationResult.Success();
    }
}
```

### Operation Validation
```csharp
public static class OperationValidation
{
    private static readonly string[] ValidOperations = { "90", "100", "110", "120", "130", "140" };
    
    public static ValidationResult ValidateOperation(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
            return ValidationResult.Error("Operation number is required");
            
        if (!ValidOperations.Contains(operation))
            return ValidationResult.Error($"Invalid operation number: {operation}. Valid operations: {string.Join(", ", ValidOperations)}");
            
        return ValidationResult.Success();
    }
}
```

## Manufacturing KPIs and Metrics

### Inventory Accuracy Metrics
```csharp
public class InventoryMetrics
{
    public decimal AccuracyPercentage { get; set; }    // Target: >99.5%
    public int TransactionsPerHour { get; set; }       // Target: varies by operation
    public decimal CycleCountVariance { get; set; }    // Target: <1%
    public int ErrorTransactions { get; set; }         // Target: <0.1%
}
```

### Manufacturing Efficiency Metrics
- **Throughput**: Parts processed per hour by operation
- **WIP Levels**: Work-in-process inventory by operation
- **Cycle Time**: Time from receipt to shipping
- **Quality Rate**: Good parts / Total parts processed

## Common Manufacturing Scenarios

### High-Volume Production
```csharp
// Batch processing for high-volume parts
public class BatchProcessing
{
    public async Task<BatchResult> ProcessBatchAsync(string partId, string operation, int batchSize)
    {
        var batchId = Guid.NewGuid().ToString();
        var transactions = new List<InventoryTransaction>();
        
        // Split into smaller transaction chunks for performance
        const int chunkSize = 100;
        for (int i = 0; i < batchSize; i += chunkSize)
        {
            var chunkQuantity = Math.Min(chunkSize, batchSize - i);
            
            transactions.Add(new InventoryTransaction
            {
                PartId = partId,
                Operation = operation,
                Quantity = chunkQuantity,
                BatchId = batchId,
                TransactionType = DetermineTransactionType(),
                Timestamp = DateTime.Now
            });
        }
        
        return await ProcessTransactionBatchAsync(transactions);
    }
}
```

### Multi-Location Operations
```csharp
// Managing parts across multiple locations
public class MultiLocationInventory
{
    public async Task<TransferResult> TransferBetweenLocationsAsync(
        string partId, string operation, int quantity, 
        string fromLocation, string toLocation, string userId)
    {
        var transferId = Guid.NewGuid().ToString();
        
        // Two-part transaction: Remove from source, Add to destination
        var transactions = new[]
        {
            new InventoryTransaction
            {
                PartId = partId,
                Operation = operation,
                Quantity = quantity,
                Location = fromLocation,
                TransactionType = "OUT",
                TransferId = transferId,
                UserId = userId
            },
            new InventoryTransaction
            {
                PartId = partId,
                Operation = operation,
                Quantity = quantity,
                Location = toLocation,
                TransactionType = "IN",
                TransferId = transferId,
                UserId = userId
            }
        };
        
        return await ProcessTransferAsync(transactions);
    }
}
```

## Manufacturing Anti-Patterns (Avoid These)

### ❌ Using Operation Numbers for Transaction Types
```csharp
// DON'T DO THIS - Operation numbers are workflow steps, not transaction indicators
if (operation == "90") 
{
    transactionType = "IN"; // Wrong logic
}
```

### ❌ Allowing Negative Inventory
```csharp
// DON'T ALLOW - Negative inventory indicates data integrity issues
var newQuantity = currentQuantity - transactionQuantity; // Could go negative
inventory.Quantity = newQuantity; // Wrong - no validation
```

### ❌ Ignoring Manufacturing Workflow
```csharp
// DON'T DO - Bypassing manufacturing sequence
// Moving directly from operation 90 to 130 without intermediate steps
// This breaks manufacturing workflow tracking
```

## Integration with MTM Technology Stack

### Database Integration
All manufacturing data operations use stored procedures exclusively:
```csharp
// Manufacturing-specific stored procedures
await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString, "inv_inventory_Add_Item", parameters);
    
await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString, "inv_transaction_Record_Manufacturing", parameters);
```

### UI Integration
Manufacturing forms follow MTM UI patterns:
```xml
<Grid x:Name="ManufacturingGrid" RowDefinitions="Auto,*,Auto">
    <!-- Part selection and operation input -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}">
        <StackPanel Orientation="Horizontal" Spacing="16">
            <TextBox Text="{Binding PartId}" Watermark="Part ID" />
            <ComboBox ItemsSource="{Binding Operations}" SelectedItem="{Binding SelectedOperation}" />
            <NumericUpDown Value="{Binding Quantity}" Minimum="1" Maximum="999999" />
        </StackPanel>
    </Border>
    
    <!-- Manufacturing data grid -->
    <DataGrid Grid.Row="1" ItemsSource="{Binding InventoryItems}" />
    
    <!-- Action buttons -->
    <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Right">
        <Button Command="{Binding ProcessCommand}" Content="Process" />
        <Button Command="{Binding TransferCommand}" Content="Transfer" />
    </StackPanel>
</Grid>
```

This manufacturing context provides the business domain knowledge essential for implementing inventory management features that align with real manufacturing operations and workflows.

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**
- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**
- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**
- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**
- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**
- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**
- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**
- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
