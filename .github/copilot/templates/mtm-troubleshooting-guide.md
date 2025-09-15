---
description: 'Troubleshooting guide template for MTM WIP Application issues and solutions'
applies_to: '**/*'
---

# MTM Troubleshooting Guide Template

## 🔧 Issue Resolution Instructions

Systematic troubleshooting patterns for MTM WIP Application issues following manufacturing-grade support standards.

## Template Structure

### Issue Title Template
```
[CATEGORY] Brief Description - [COMPONENT] - [SEVERITY]

Examples:
- [DATABASE] Connection timeout during inventory save - InventoryService - HIGH
- [UI] Form validation not triggering - InventoryTabView - MEDIUM  
- [PERFORMANCE] Memory leak in transaction history - TransactionService - CRITICAL
```

## Common Issue Categories

### 1. Database Connection Issues

#### Issue Pattern Template
```markdown
## 🔴 DATABASE CONNECTION: [Specific Issue]

**Symptoms:**
- [ ] Connection timeouts during operations
- [ ] "Unable to connect to MySQL server" errors
- [ ] Operations fail with database exceptions
- [ ] Stored procedure execution failures

**Root Cause Analysis:**
1. **Connection String Validation**
   ```csharp
   // Verify connection string format
   var connectionString = "Server=localhost;Database=mtm_wip;Uid=user;Pwd=password;";
   
   // Test connection
   using var connection = new MySqlConnection(connectionString);
   await connection.OpenAsync();
   ```

2. **Database Service Verification**
   ```bash
   # Check MySQL service status
   systemctl status mysql
   
   # Check MySQL error log
   sudo tail -f /var/log/mysql/error.log
   ```

3. **Stored Procedure Availability**
   ```sql
   -- Verify stored procedures exist
   SHOW PROCEDURE STATUS WHERE Db = 'mtm_wip';
   
   -- Test specific procedure
   CALL inv_inventory_Get_All();
   ```

**Resolution Steps:**
1. [ ] Verify MySQL service is running
2. [ ] Test connection string with mysql client
3. [ ] Verify database and user permissions
4. [ ] Check stored procedure availability
5. [ ] Validate network connectivity
6. [ ] Review application logs for detailed errors

**Prevention:**
- Implement connection pooling
- Add retry logic with exponential backoff
- Monitor database performance metrics
- Regular connection string validation
```

### 2. MVVM Community Toolkit Issues

#### Issue Pattern Template
```markdown
## ⚙️ MVVM: [Specific Issue]

**Symptoms:**
- [ ] Property changes not updating UI
- [ ] Commands not executing
- [ ] Binding errors in output window
- [ ] Source generator compilation failures

**Root Cause Analysis:**
1. **Source Generator Verification**
   ```csharp
   // Ensure proper [ObservableObject] usage
   [ObservableObject]
   public partial class InventoryTabViewModel : BaseViewModel
   {
       [ObservableProperty]
       private string partId = string.Empty; // Must be private field
       
       [RelayCommand]
       private async Task SaveAsync() // Must be private method
       {
           // Implementation
       }
   }
   ```

2. **Binding Path Validation**
   ```xml
   <!-- CORRECT binding to generated property -->
   <TextBox Text="{Binding PartId}" />
   
   <!-- INCORRECT binding to private field -->
   <TextBox Text="{Binding partId}" />
   ```

3. **Build Output Analysis**
   ```
   Check Build Output for source generator errors:
   - CS0103: The name 'PropertyName' does not exist
   - CS0117: Type does not contain a definition for 'Command'
   ```

**Resolution Steps:**
1. [ ] Verify [ObservableObject] attribute on class
2. [ ] Ensure properties use [ObservableProperty] pattern
3. [ ] Check commands use [RelayCommand] pattern
4. [ ] Validate binding paths in XAML
5. [ ] Clean and rebuild solution
6. [ ] Check source generator references

**Prevention:**
- Follow MTM MVVM patterns consistently
- Use code snippets for common patterns
- Regular build validation
- Consistent naming conventions
```

### 3. Avalonia UI Rendering Issues

#### Issue Pattern Template
```markdown
## 🎨 UI RENDERING: [Specific Issue]

**Symptoms:**
- [ ] Controls not displaying correctly
- [ ] Theme colors not applying
- [ ] Layout overflow or clipping
- [ ] AVLN2000 compilation errors

**Root Cause Analysis:**
1. **AXAML Syntax Validation**
   ```xml
   <!-- CORRECT: Use x:Name NOT Name on Grid -->
   <Grid x:Name="MainGrid" RowDefinitions="Auto,*">
   
   <!-- INCORRECT: Will cause AVLN2000 -->
   <Grid Name="MainGrid" RowDefinitions="Auto,*">
   ```

2. **Theme Resource Usage**
   ```xml
   <!-- CORRECT: DynamicResource for themes -->
   <Border Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}">
   
   <!-- INCORRECT: StaticResource won't update with theme changes -->
   <Border Background="{StaticResource MTM_Shared_Logic.PrimaryAction}">
   ```

3. **Namespace Declaration**
   ```xml
   <!-- CORRECT: Avalonia namespace -->
   <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
   
   <!-- INCORRECT: WPF namespace -->
   <UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
   ```

**Resolution Steps:**
1. [ ] Validate AXAML syntax for Avalonia-specific requirements
2. [ ] Check theme resource bindings
3. [ ] Verify namespace declarations
4. [ ] Test with different themes
5. [ ] Validate layout definitions
6. [ ] Check for binding errors in debug output

**Prevention:**
- Use MTM UI component templates
- Regular theme testing
- Avalonia-specific syntax validation
- Consistent resource usage patterns
```

### 4. Performance Issues

#### Issue Pattern Template
```markdown
## ⚡ PERFORMANCE: [Specific Issue]

**Symptoms:**
- [ ] Slow UI response times
- [ ] High memory usage
- [ ] CPU spikes during operations
- [ ] Database query timeouts

**Root Cause Analysis:**
1. **Memory Profiling**
   ```csharp
   // Monitor memory usage
   var memoryBefore = GC.GetTotalMemory(false);
   
   // Perform operation
   await SomeOperation();
   
   var memoryAfter = GC.GetTotalMemory(true);
   var memoryIncrease = memoryAfter - memoryBefore;
   
   Logger.LogInformation("Memory increase: {MemoryMB} MB", 
                        memoryIncrease / 1024.0 / 1024.0);
   ```

2. **Database Query Analysis**
   ```csharp
   // Add timing to stored procedure calls
   var stopwatch = Stopwatch.StartNew();
   var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
       connectionString, "procedure_name", parameters);
   stopwatch.Stop();
   
   Logger.LogInformation("Database operation took {ElapsedMs} ms", 
                        stopwatch.ElapsedMilliseconds);
   ```

3. **UI Thread Blocking**
   ```csharp
   // INCORRECT: Blocking UI thread
   var result = SomeAsyncOperation().Result;
   
   // CORRECT: Proper async pattern
   var result = await SomeAsyncOperation();
   ```

**Resolution Steps:**
1. [ ] Profile memory usage patterns
2. [ ] Analyze database query performance
3. [ ] Check for UI thread blocking
4. [ ] Review collection usage and disposal
5. [ ] Optimize data binding scenarios
6. [ ] Implement caching where appropriate

**Prevention:**
- Regular performance testing
- Memory leak detection
- Database query optimization
- Proper async/await patterns
```

### 5. Manufacturing Domain Issues

#### Issue Pattern Template
```markdown
## 🏭 MANUFACTURING: [Specific Issue]

**Symptoms:**
- [ ] Incorrect inventory calculations
- [ ] Transaction type mismatches
- [ ] Operation number validation failures
- [ ] Part ID format inconsistencies

**Root Cause Analysis:**
1. **Transaction Type Logic**
   ```csharp
   // CORRECT: User intent determines transaction type
   public string DetermineTransactionType(UserAction action)
   {
       return action.Intent switch
       {
           UserIntent.AddingStock => "IN",      // User adding inventory
           UserIntent.RemovingStock => "OUT",   // User removing inventory
           UserIntent.MovingStock => "TRANSFER" // User moving between locations
       };
   }
   
   // INCORRECT: Don't use operation numbers for transaction types
   // Operation numbers ("90", "100", "110") are workflow steps, not transaction indicators
   ```

2. **Part ID Validation**
   ```csharp
   // Validate MTM part ID format
   public bool IsValidPartId(string partId)
   {
       if (string.IsNullOrWhiteSpace(partId)) return false;
       
       // MTM part IDs: alphanumeric with dashes, max 50 chars
       return Regex.IsMatch(partId, @"^[A-Za-z0-9\-]{1,50}$");
   }
   ```

3. **Operation Number Logic**
   ```csharp
   // Operation numbers represent manufacturing workflow steps
   public static class ManufacturingOperations
   {
       public const string Receiving = "90";      // Parts received into system
       public const string FirstOperation = "100"; // First manufacturing step
       public const string SecondOperation = "110"; // Second manufacturing step
       public const string FinalOperation = "120"; // Final manufacturing step
       public const string Shipping = "130";      // Parts ready for shipping
   }
   ```

**Resolution Steps:**
1. [ ] Verify transaction type logic matches user intent
2. [ ] Validate part ID format compliance
3. [ ] Check operation number usage patterns
4. [ ] Review inventory calculation logic
5. [ ] Validate manufacturing workflow steps
6. [ ] Test with real manufacturing scenarios

**Prevention:**
- Manufacturing domain validation rules
- Business logic unit testing
- Real-world scenario testing
- Domain expert review processes
```

## Error Message Catalog

### Common Error Patterns and Solutions

```markdown
## Error: "Unable to connect to any of the specified MySQL hosts"
**Cause**: Database connection configuration
**Solution**: Verify connection string, MySQL service status, network connectivity

## Error: "The name 'PropertyName' does not exist in the current context"
**Cause**: MVVM Community Toolkit source generator issue
**Solution**: Verify [ObservableProperty] attribute, rebuild solution

## Error: "AVLN2000: Unable to resolve type"
**Cause**: Avalonia XAML compilation error
**Solution**: Check namespace declarations, verify control references

## Error: "Stored procedure 'procedure_name' doesn't exist"
**Cause**: Database schema mismatch
**Solution**: Verify stored procedure deployment, check database version

## Error: "Object reference not set to an instance of an object"
**Cause**: Dependency injection or null reference
**Solution**: Verify service registration, add null checks
```

## Diagnostic Tools and Commands

### Development Environment Diagnostics
```bash
# Check .NET version
dotnet --version

# Check project references
dotnet list package

# Clean and rebuild
dotnet clean && dotnet build

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Database Diagnostics
```sql
-- Check stored procedures
SELECT ROUTINE_NAME, ROUTINE_TYPE 
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'mtm_wip';

-- Check table structure
DESCRIBE inventory;

-- Check recent errors
SELECT * FROM error_log ORDER BY timestamp DESC LIMIT 10;
```

### Application Diagnostics
```csharp
// Enable detailed logging
public void ConfigureLogging(ILoggingBuilder builder)
{
    builder.AddConsole()
           .AddDebug()
           .SetMinimumLevel(LogLevel.Debug);
}

// Memory diagnostics
GC.Collect();
var memoryUsage = GC.GetTotalMemory(false);
Logger.LogInformation("Current memory usage: {MemoryMB} MB", 
                     memoryUsage / 1024.0 / 1024.0);
```

## Escalation Guidelines

### When to Escalate
- Critical system failures affecting production
- Data integrity issues
- Security vulnerabilities
- Performance degradation > 50%
- Multiple users affected

### Escalation Information to Provide
1. **Issue Description**: Clear, concise problem statement
2. **Reproduction Steps**: Step-by-step instructions
3. **Environment Details**: OS, .NET version, database version
4. **Error Messages**: Complete error text and stack traces
5. **Impact Assessment**: Users affected, business impact
6. **Troubleshooting Attempted**: Steps already taken
7. **Diagnostic Data**: Logs, performance metrics, screenshots

This troubleshooting guide ensures consistent, efficient problem resolution for the MTM WIP Application.