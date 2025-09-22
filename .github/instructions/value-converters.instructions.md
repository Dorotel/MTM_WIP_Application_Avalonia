# Value Converters - MTM WIP Application Instructions

**Framework**: Avalonia UI 11.3.4  
**Pattern**: Data Conversion and Formatting  
**Created**: 2025-09-14  

---

## 🎯 Core Value Converter Patterns

### IValueConverter Implementation
```csharp
// Standard value converter pattern for MTM application
public class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Type safety and null checking
        if (value is not Color color)
            return AvaloniaProperty.UnsetValue;
        
        // Manufacturing-specific color conversion with caching
        return ColorToSolidColorBrushCache.GetOrCreate(color);
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
            return brush.Color;
        
        return AvaloniaProperty.UnsetValue;
    }
}

// Performance-optimized brush caching for manufacturing UI
public static class ColorToSolidColorBrushCache
{
    private static readonly ConcurrentDictionary<Color, SolidColorBrush> _cache = new();
    
    public static SolidColorBrush GetOrCreate(Color color)
    {
        return _cache.GetOrAdd(color, c => new SolidColorBrush(c));
    }
    
    public static void ClearCache()
    {
        _cache.Clear();
    }
}
```

### Generic Type-Safe Converter Base
```csharp
// Type-safe converter base for manufacturing data conversions
public abstract class TypeSafeConverter<TSource, TTarget> : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TSource sourceValue)
        {
            try
            {
                return ConvertTyped(sourceValue, parameter, culture);
            }
            catch (Exception ex)
            {
                // Manufacturing-grade error handling
                Console.WriteLine($"Converter error: {GetType().Name} - {ex.Message}");
                return GetDefaultTargetValue();
            }
        }
        
        return AvaloniaProperty.UnsetValue;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TTarget targetValue)
        {
            try
            {
                return ConvertBackTyped(targetValue, parameter, culture);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ConvertBack error: {GetType().Name} - {ex.Message}");
                return GetDefaultSourceValue();
            }
        }
        
        return AvaloniaProperty.UnsetValue;
    }
    
    protected abstract TTarget ConvertTyped(TSource source, object? parameter, CultureInfo culture);
    
    protected virtual TSource ConvertBackTyped(TTarget target, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException($"ConvertBack not supported in {GetType().Name}");
    }
    
    protected virtual TTarget GetDefaultTargetValue() => default!;
    protected virtual TSource GetDefaultSourceValue() => default!;
}
```

### Multi-Value Converter Pattern
```csharp
// Manufacturing validation converter using multiple inputs
public class ManufacturingValidationConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // Manufacturing validation requires: PartId, Operation, Quantity, Location
        if (values.Count < 4)
            return false;
        
        var partId = values[0]?.ToString() ?? string.Empty;
        var operation = values[1]?.ToString() ?? string.Empty;
        var quantity = values[2] as int? ?? 0;
        var location = values[3]?.ToString() ?? string.Empty;
        
        // Manufacturing business rules validation
        var isValidPartId = !string.IsNullOrWhiteSpace(partId) && 
                           partId.Length <= 50 && 
                           IsValidPartIdFormat(partId);
        
        var isValidOperation = !string.IsNullOrWhiteSpace(operation) && 
                              IsValidOperationNumber(operation);
        
        var isValidQuantity = quantity > 0 && quantity <= 999999;
        
        var isValidLocation = !string.IsNullOrWhiteSpace(location) &&
                             location.Length <= 20;
        
        // Return validation result or validation details based on parameter
        if (parameter?.ToString() == "Details")
        {
            return new ValidationResult
            {
                IsValid = isValidPartId && isValidOperation && isValidQuantity && isValidLocation,
                PartIdValid = isValidPartId,
                OperationValid = isValidOperation,
                QuantityValid = isValidQuantity,
                LocationValid = isValidLocation
            };
        }
        
        return isValidPartId && isValidOperation && isValidQuantity && isValidLocation;
    }
    
    private static bool IsValidPartIdFormat(string partId)
    {
        // Manufacturing part ID format validation
        return System.Text.RegularExpressions.Regex.IsMatch(partId, @"^[A-Z0-9\-]+$");
    }
    
    private static bool IsValidOperationNumber(string operation)
    {
        // Manufacturing operation validation (90, 100, 110, 120, etc.)
        return int.TryParse(operation, out var number) && 
               number >= 90 && number <= 999 && 
               number % 10 == 0;
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public bool PartIdValid { get; set; }
    public bool OperationValid { get; set; }
    public bool QuantityValid { get; set; }
    public bool LocationValid { get; set; }
}
```

---

## 🏭 Manufacturing-Specific Converter Patterns

### Manufacturing Operation Status Converter
```csharp
// Convert manufacturing operation numbers to visual indicators
public class OperationStatusConverter : TypeSafeConverter<string, object>
{
    public static readonly OperationStatusConverter Instance = new();
    
    private static readonly Dictionary<string, OperationInfo> OperationMap = new()
    {
        ["90"] = new("Receiving", Colors.Blue, "📦"),
        ["100"] = new("First Operation", Colors.Green, "🔧"),
        ["110"] = new("Second Operation", Colors.Orange, "⚙️"),
        ["120"] = new("Final Operation", Colors.Red, "✅"),
        ["130"] = new("Shipping", Colors.Purple, "🚚")
    };
    
    protected override object ConvertTyped(string operation, object? parameter, CultureInfo culture)
    {
        if (!OperationMap.TryGetValue(operation, out var info))
        {
            info = new OperationInfo($"Operation {operation}", Colors.Gray, "❓");
        }
        
        return parameter?.ToString() switch
        {
            "Name" => info.Name,
            "Color" => info.Color,
            "Brush" => new SolidColorBrush(info.Color),
            "Icon" => info.Icon,
            "ToolTip" => $"{info.Icon} {info.Name} (Op: {operation})",
            _ => info
        };
    }
    
    private record OperationInfo(string Name, Color Color, string Icon);
}

// Usage in XAML with converter parameters
/*
<TextBlock Text="{Binding Operation, Converter={x:Static converters:OperationStatusConverter.Instance}, ConverterParameter=Name}" />
<Rectangle Fill="{Binding Operation, Converter={x:Static converters:OperationStatusConverter.Instance}, ConverterParameter=Brush}" />
<TextBlock Text="{Binding Operation, Converter={x:Static converters:OperationStatusConverter.Instance}, ConverterParameter=Icon}" />
*/
```

### Manufacturing Quantity Formatter
```csharp
// Format manufacturing quantities with units and colors
public class ManufacturingQuantityConverter : TypeSafeConverter<int, object>
{
    protected override object ConvertTyped(int quantity, object? parameter, CultureInfo culture)
    {
        return parameter?.ToString() switch
        {
            "Formatted" => FormatQuantity(quantity),
            "Color" => GetQuantityColor(quantity),
            "Brush" => new SolidColorBrush(GetQuantityColor(quantity)),
            "IsLowStock" => IsLowStock(quantity),
            "StockLevel" => GetStockLevel(quantity),
            "DisplayText" => GetDisplayText(quantity),
            _ => quantity.ToString("N0")
        };
    }
    
    private string FormatQuantity(int quantity)
    {
        // Manufacturing quantity formatting with appropriate units
        return quantity switch
        {
            >= 1_000_000 => $"{quantity / 1_000_000.0:F1}M",
            >= 1_000 => $"{quantity / 1_000.0:F1}K",
            _ => quantity.ToString("N0")
        };
    }
    
    private Color GetQuantityColor(int quantity)
    {
        // Manufacturing stock level color coding
        return quantity switch
        {
            0 => Colors.Red,           // Out of stock
            < 10 => Colors.Orange,     // Low stock
            < 50 => Colors.Yellow,     // Medium stock  
            _ => Colors.Green          // Good stock
        };
    }
    
    private bool IsLowStock(int quantity) => quantity < 10;
    
    private string GetStockLevel(int quantity)
    {
        return quantity switch
        {
            0 => "OUT_OF_STOCK",
            < 10 => "LOW_STOCK",
            < 50 => "MEDIUM_STOCK",
            _ => "GOOD_STOCK"
        };
    }
    
    private string GetDisplayText(int quantity)
    {
        var formatted = FormatQuantity(quantity);
        var level = GetStockLevel(quantity);
        
        return level switch
        {
            "OUT_OF_STOCK" => $"⚠️ {formatted} (Out of Stock)",
            "LOW_STOCK" => $"⚠️ {formatted} (Low Stock)",
            _ => formatted
        };
    }
}
```

### Manufacturing Transaction Type Converter
```csharp
// Convert transaction types to visual representations
public class TransactionTypeConverter : TypeSafeConverter<string, object>
{
    private static readonly Dictionary<string, TransactionTypeInfo> TransactionTypes = new()
    {
        ["IN"] = new("Incoming", Colors.Green, "⬇️", "Material received or added to inventory"),
        ["OUT"] = new("Outgoing", Colors.Red, "⬆️", "Material removed from inventory"),
        ["TRANSFER"] = new("Transfer", Colors.Blue, "↔️", "Material moved between locations"),
        ["ADJUSTMENT"] = new("Adjustment", Colors.Orange, "⚖️", "Inventory quantity correction"),
        ["SCRAP"] = new("Scrap", Colors.DarkRed, "🗑️", "Defective material scrapped")
    };
    
    protected override object ConvertTyped(string transactionType, object? parameter, CultureInfo culture)
    {
        if (!TransactionTypes.TryGetValue(transactionType.ToUpperInvariant(), out var info))
        {
            info = new TransactionTypeInfo(transactionType, Colors.Gray, "❓", "Unknown transaction type");
        }
        
        return parameter?.ToString() switch
        {
            "DisplayName" => info.DisplayName,
            "Color" => info.Color,
            "Brush" => new SolidColorBrush(info.Color),
            "Icon" => info.Icon,
            "Description" => info.Description,
            "Badge" => $"{info.Icon} {info.DisplayName}",
            "ToolTip" => $"{info.Icon} {info.DisplayName}: {info.Description}",
            _ => info.DisplayName
        };
    }
    
    private record TransactionTypeInfo(string DisplayName, Color Color, string Icon, string Description);
}
```

### Manufacturing Date/Time Converter
```csharp
// Convert manufacturing timestamps to relative time with context
public class ManufacturingDateTimeConverter : TypeSafeConverter<DateTime, object>
{
    protected override object ConvertTyped(DateTime dateTime, object? parameter, CultureInfo culture)
    {
        return parameter?.ToString() switch
        {
            "Relative" => GetRelativeTime(dateTime),
            "ShiftContext" => GetShiftContext(dateTime),
            "Formatted" => dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            "DateOnly" => dateTime.ToString("yyyy-MM-dd"),
            "TimeOnly" => dateTime.ToString("HH:mm:ss"),
            "IsToday" => dateTime.Date == DateTime.Today,
            "IsCurrentShift" => IsCurrentShift(dateTime),
            "Age" => GetAge(dateTime),
            "ShiftDisplay" => GetShiftDisplay(dateTime),
            _ => dateTime.ToString("g")
        };
    }
    
    private string GetRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        return timeSpan switch
        {
            { TotalMinutes: < 1 } => "Just now",
            { TotalMinutes: < 60 } => $"{(int)timeSpan.TotalMinutes} minutes ago",
            { TotalHours: < 24 } => $"{(int)timeSpan.TotalHours} hours ago",
            { TotalDays: < 7 } => $"{(int)timeSpan.TotalDays} days ago",
            { TotalDays: < 30 } => $"{(int)timeSpan.TotalDays / 7} weeks ago",
            _ => dateTime.ToString("yyyy-MM-dd")
        };
    }
    
    private string GetShiftContext(DateTime dateTime)
    {
        // Manufacturing shift context (assuming 3 shifts: Day, Evening, Night)
        var hour = dateTime.Hour;
        var shift = hour switch
        {
            >= 6 and < 14 => "Day Shift",
            >= 14 and < 22 => "Evening Shift", 
            _ => "Night Shift"
        };
        
        return $"{shift} ({dateTime:HH:mm})";
    }
    
    private bool IsCurrentShift(DateTime dateTime)
    {
        var currentHour = DateTime.Now.Hour;
        var dateHour = dateTime.Hour;
        
        var currentShift = GetShiftNumber(currentHour);
        var dateShift = GetShiftNumber(dateHour);
        
        return currentShift == dateShift && dateTime.Date == DateTime.Today;
    }
    
    private int GetShiftNumber(int hour)
    {
        return hour switch
        {
            >= 6 and < 14 => 1,  // Day shift
            >= 14 and < 22 => 2, // Evening shift
            _ => 3               // Night shift
        };
    }
    
    private string GetAge(DateTime dateTime)
    {
        var age = DateTime.Now - dateTime;
        
        if (age.TotalDays >= 1)
            return $"{(int)age.TotalDays}d";
        if (age.TotalHours >= 1)
            return $"{(int)age.TotalHours}h";
        
        return $"{(int)age.TotalMinutes}m";
    }
    
    private string GetShiftDisplay(DateTime dateTime)
    {
        var shift = GetShiftContext(dateTime);
        var relative = GetRelativeTime(dateTime);
        
        return $"{shift} • {relative}";
    }
}
```

### String Validation Converter with Manufacturing Context
```csharp
// Enhanced string equals converter for manufacturing validation
public class StringEqualsConverter : TypeSafeConverter<string, bool>
{
    protected override bool ConvertTyped(string source, object? parameter, CultureInfo culture)
    {
        if (parameter is null)
            return false;
        
        var target = parameter.ToString();
        if (target is null)
            return false;
        
        // Manufacturing-specific string comparison options
        var comparisonOptions = GetComparisonOptions(target);
        
        return string.Equals(source, ExtractComparisonValue(target), comparisonOptions);
    }
    
    private StringComparison GetComparisonOptions(string target)
    {
        // Manufacturing part IDs and operations are case-insensitive
        if (target.StartsWith("PART:", StringComparison.OrdinalIgnoreCase) ||
            target.StartsWith("OP:", StringComparison.OrdinalIgnoreCase) ||
            target.StartsWith("LOC:", StringComparison.OrdinalIgnoreCase))
        {
            return StringComparison.OrdinalIgnoreCase;
        }
        
        // Default to case-sensitive comparison
        return StringComparison.Ordinal;
    }
    
    private string ExtractComparisonValue(string target)
    {
        // Handle prefixed values for manufacturing context
        if (target.Contains(':'))
        {
            var parts = target.Split(':', 2);
            return parts.Length > 1 ? parts[1] : target;
        }
        
        return target;
    }
}
```

---

## ❌ Anti-Patterns (Avoid These)

### Heavy Operations in Converters
```csharp
// ❌ WRONG: Expensive operations in converter methods
public class ExpensiveConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string partId)
        {
            // BAD: Database call in converter
            var partDetails = DatabaseService.GetPartDetails(partId).Result;
            
            // BAD: File I/O in converter
            var config = File.ReadAllText("config.json");
            
            // BAD: Complex calculations on every binding update
            var complexResult = PerformComplexCalculation(partDetails);
            
            return complexResult;
        }
        
        return value;
    }
}

// ✅ CORRECT: Lightweight converter with pre-computed data
public class EfficientConverter : IValueConverter
{
    private readonly IMemoryCache _cache;
    private static readonly ConcurrentDictionary<string, object> _computedResults = new();
    
    public EfficientConverter(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string partId)
        {
            // GOOD: Use caching for expensive operations
            return _computedResults.GetOrAdd(partId, id => 
            {
                // Pre-computed or cached result
                return GetCachedPartInfo(id);
            });
        }
        
        return value;
    }
    
    private object GetCachedPartInfo(string partId)
    {
        // Efficient cached lookup or pre-computed value
        return _cache.GetOrCreate($"part_{partId}", factory =>
        {
            factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return ComputePartInfo(partId);
        });
    }
}
```

### Converter State Management Anti-Patterns
```csharp
// ❌ WRONG: Stateful converter with shared mutable state
public class StatefulConverter : IValueConverter
{
    // BAD: Shared mutable state in converter
    private int _conversionCount = 0;
    private readonly List<object> _previousValues = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // BAD: Modifying state during conversion
        _conversionCount++;
        _previousValues.Add(value);
        
        // BAD: Converter behavior depends on previous calls
        if (_conversionCount > 10)
        {
            return "Too many conversions";
        }
        
        return value?.ToString();
    }
}

// ✅ CORRECT: Stateless converter with pure functions
public class StatelessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // GOOD: Pure function without side effects
        return ConvertValue(value, parameter);
    }
    
    private static object ConvertValue(object? value, object? parameter)
    {
        // GOOD: Deterministic conversion based only on inputs
        return value?.ToString() ?? string.Empty;
    }
}
```

### Exception Handling Anti-Patterns
```csharp
// ❌ WRONG: Poor exception handling in converters
public class BadExceptionHandlingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // BAD: No exception handling - crashes UI
        var result = int.Parse(value.ToString());
        return result * 2;
    }
}

public class SilentFailureConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            return int.Parse(value.ToString()) * 2;
        }
        catch
        {
            // BAD: Silent failure - hides problems
            return 0;
        }
    }
}

// ✅ CORRECT: Proper exception handling with logging
public class ProperExceptionHandlingConverter : TypeSafeConverter<object, object>
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    protected override object ConvertTyped(object source, object? parameter, CultureInfo culture)
    {
        try
        {
            if (source is string str && int.TryParse(str, out var number))
            {
                return number * 2;
            }
            
            return GetDefaultTargetValue();
        }
        catch (Exception ex)
        {
            // GOOD: Log the error for debugging
            Logger.Error(ex, "Conversion error for value: {Value}, parameter: {Parameter}", 
                source, parameter);
            
            // GOOD: Return reasonable default
            return GetDefaultTargetValue();
        }
    }
    
    protected override object GetDefaultTargetValue() => 0;
}
```

---

## 🔧 Manufacturing Converter Troubleshooting

### Issue: Converter Not Triggering Updates
**Symptoms**: UI doesn't update when bound property changes

**Solution**: Ensure proper INotifyPropertyChanged implementation
```csharp
// ✅ CORRECT: ViewModel property that triggers converter
[ObservableProperty]
private string partId = string.Empty;

[ObservableProperty] 
private int quantity;

// Converter will be called when these properties change
```

```xml
<!-- ✅ CORRECT: Binding that triggers converter updates -->
<TextBlock Text="{Binding PartId, Converter={StaticResource PartIdConverter}}" />
<Rectangle Fill="{Binding Quantity, Converter={StaticResource QuantityColorConverter}}" />
```

### Issue: Performance Problems with Frequent Conversions
**Symptoms**: UI becomes sluggish during data updates

**Solution**: Implement caching and optimize converter logic
```csharp
// ✅ CORRECT: Cached converter for performance
public class CachedManufacturingConverter : IValueConverter
{
    private readonly ConcurrentDictionary<object, object> _cache = new();
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var cacheKey = $"{value}_{parameter}_{targetType}";
        
        return _cache.GetOrAdd(cacheKey, _ => PerformConversion(value, targetType, parameter, culture));
    }
    
    private object PerformConversion(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Actual conversion logic here
        return ConvertValue(value, parameter);
    }
    
    public void ClearCache()
    {
        _cache.Clear();
    }
}
```

### Issue: Multi-Value Converter Binding Problems
**Symptoms**: Multi-value converter receives null or incomplete values

**Solution**: Implement proper null handling and validation
```csharp
// ✅ CORRECT: Robust multi-value converter
public class RobustMultiValueConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object parameter, CultureInfo culture)
    {
        // Validate input count
        if (values.Count < ExpectedValueCount)
        {
            return GetDefaultResult();
        }
        
        // Handle null values gracefully
        var processedValues = values.Select(ProcessValue).ToArray();
        
        // Validate all required values are present
        if (processedValues.Any(v => v == null))
        {
            return GetDefaultResult();
        }
        
        return ProcessValidValues(processedValues, parameter);
    }
    
    protected virtual int ExpectedValueCount => 2;
    
    private object? ProcessValue(object? value)
    {
        // Handle AvaloniaProperty.UnsetValue and null
        return value == AvaloniaProperty.UnsetValue ? null : value;
    }
    
    protected virtual object GetDefaultResult() => false;
    
    protected virtual object ProcessValidValues(object[] values, object? parameter)
    {
        // Implement specific conversion logic
        return true;
    }
}
```

---

## 🧪 Converter Testing Patterns

### Unit Testing Value Converters
```csharp
[TestFixture]
public class ManufacturingConverterTests
{
    private OperationStatusConverter _operationConverter;
    private ManufacturingQuantityConverter _quantityConverter;
    
    [SetUp]
    public void SetUp()
    {
        _operationConverter = new OperationStatusConverter();
        _quantityConverter = new ManufacturingQuantityConverter();
    }
    
    [Test]
    public void OperationStatusConverter_ValidOperation_ReturnsCorrectName()
    {
        // Act
        var result = _operationConverter.Convert("100", typeof(string), "Name", CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo("First Operation"));
    }
    
    [TestCase("90", "Name", "Receiving")]
    [TestCase("100", "Name", "First Operation")]
    [TestCase("110", "Name", "Second Operation")]
    [TestCase("120", "Name", "Final Operation")]
    [TestCase("999", "Name", "Operation 999")] // Unknown operation
    public void OperationStatusConverter_VariousOperations_ReturnsExpectedNames(
        string operation, string parameter, string expectedResult)
    {
        // Act
        var result = _operationConverter.Convert(operation, typeof(string), parameter, CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void ManufacturingQuantityConverter_LowStock_ReturnsOrangeColor()
    {
        // Act
        var result = _quantityConverter.Convert(5, typeof(Color), "Color", CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo(Colors.Orange));
    }
    
    [TestCase(0, "OUT_OF_STOCK")]
    [TestCase(5, "LOW_STOCK")]
    [TestCase(25, "MEDIUM_STOCK")]
    [TestCase(100, "GOOD_STOCK")]
    public void ManufacturingQuantityConverter_StockLevels_ReturnsCorrectLevel(
        int quantity, string expectedLevel)
    {
        // Act
        var result = _quantityConverter.Convert(quantity, typeof(string), "StockLevel", CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo(expectedLevel));
    }
    
    [Test]
    public void StringEqualsConverter_ManufacturingPartId_CaseInsensitive()
    {
        // Arrange
        var converter = new StringEqualsConverter();
        
        // Act
        var result = converter.Convert("part001", typeof(bool), "PART:PART001", CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo(true));
    }
}
```

### Multi-Value Converter Testing
```csharp
[TestFixture]
public class ManufacturingValidationConverterTests
{
    private ManufacturingValidationConverter _converter;
    
    [SetUp]
    public void SetUp()
    {
        _converter = new ManufacturingValidationConverter();
    }
    
    [Test]
    public void ManufacturingValidationConverter_ValidInputs_ReturnsTrue()
    {
        // Arrange
        var values = new object[] { "PART001", "100", 10, "STATION_A" };
        
        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo(true));
    }
    
    [Test]
    public void ManufacturingValidationConverter_InvalidPartId_ReturnsFalse()
    {
        // Arrange
        var values = new object[] { "", "100", 10, "STATION_A" };
        
        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.EqualTo(false));
    }
    
    [Test]
    public void ManufacturingValidationConverter_DetailsParameter_ReturnsValidationResult()
    {
        // Arrange
        var values = new object[] { "PART001", "100", 10, "STATION_A" };
        
        // Act
        var result = _converter.Convert(values, typeof(ValidationResult), "Details", CultureInfo.InvariantCulture);
        
        // Assert
        Assert.That(result, Is.InstanceOf<ValidationResult>());
        var validationResult = (ValidationResult)result;
        Assert.That(validationResult.IsValid, Is.True);
        Assert.That(validationResult.PartIdValid, Is.True);
        Assert.That(validationResult.OperationValid, Is.True);
        Assert.That(validationResult.QuantityValid, Is.True);
        Assert.That(validationResult.LocationValid, Is.True);
    }
}
```

---

## 🔗 AXAML Usage Examples

### Basic Value Converter Usage
```xml
<!-- Operation status with different parameters -->
<TextBlock Text="{Binding Operation, Converter={StaticResource OperationStatusConverter}, ConverterParameter=Name}" />
<Rectangle Fill="{Binding Operation, Converter={StaticResource OperationStatusConverter}, ConverterParameter=Brush}" 
           Width="20" Height="20" />
<TextBlock Text="{Binding Operation, Converter={StaticResource OperationStatusConverter}, ConverterParameter=Icon}" />
```

### Manufacturing Quantity Display
```xml
<!-- Quantity with color coding and formatting -->
<StackPanel Orientation="Horizontal">
    <TextBlock Text="{Binding Quantity, Converter={StaticResource QuantityConverter}, ConverterParameter=Formatted}" />
    <Rectangle Fill="{Binding Quantity, Converter={StaticResource QuantityConverter}, ConverterParameter=Brush}" 
               Width="10" Height="10" Margin="5,0" />
    <TextBlock Text="{Binding Quantity, Converter={StaticResource QuantityConverter}, ConverterParameter=StockLevel}"
               FontSize="10" Opacity="0.7" />
</StackPanel>
```

### Multi-Value Converter for Manufacturing Validation
```xml
<!-- Form validation using multi-value converter -->
<Button Content="Save Transaction" IsEnabled="{MultiBinding ManufacturingValidationConverter}">
    <MultiBinding.Bindings>
        <Binding Path="PartId" />
        <Binding Path="Operation" />
        <Binding Path="Quantity" />
        <Binding Path="Location" />
    </MultiBinding.Bindings>
</Button>

<!-- Validation details display -->
<Border Background="LightYellow" IsVisible="{MultiBinding ManufacturingValidationConverter, ConverterParameter=Details}">
    <TextBlock Text="Please complete all required fields" />
</Border>
```

### DateTime Converters for Manufacturing Context
```xml
<!-- Manufacturing timestamp with shift context -->
<StackPanel>
    <TextBlock Text="{Binding LastUpdated, Converter={StaticResource DateTimeConverter}, ConverterParameter=ShiftDisplay}" />
    <TextBlock Text="{Binding LastUpdated, Converter={StaticResource DateTimeConverter}, ConverterParameter=Relative}" 
               FontSize="10" Opacity="0.7" />
</StackPanel>
```

### Transaction Type Visual Representation
```xml
<!-- Transaction type with icon and color -->
<StackPanel Orientation="Horizontal" Spacing="8">
    <TextBlock Text="{Binding TransactionType, Converter={StaticResource TransactionTypeConverter}, ConverterParameter=Icon}" />
    <TextBlock Text="{Binding TransactionType, Converter={StaticResource TransactionTypeConverter}, ConverterParameter=DisplayName}"
               Foreground="{Binding TransactionType, Converter={StaticResource TransactionTypeConverter}, ConverterParameter=Brush}" />
</StackPanel>
```

### Resource Declaration
```xml
<!-- App.axaml or ResourceDictionary -->
<Application.Resources>
    <converters:OperationStatusConverter x:Key="OperationStatusConverter" />
    <converters:ManufacturingQuantityConverter x:Key="QuantityConverter" />
    <converters:TransactionTypeConverter x:Key="TransactionTypeConverter" />
    <converters:ManufacturingDateTimeConverter x:Key="DateTimeConverter" />
    <converters:ManufacturingValidationConverter x:Key="ManufacturingValidationConverter" />
    <converters:StringEqualsConverter x:Key="StringEqualsConverter" />
</Application.Resources>
```

---

## 📚 Related Documentation

- **Custom Controls**: [Custom Control Implementation](./custom-controls.instructions.md)
- **Avalonia Behaviors**: [Behavior Patterns](./avalonia-behaviors.instructions.md)
- **UI Guidelines**: [Avalonia UI Guidelines](./avalonia-ui-guidelines.instructions.md)
- **MVVM Patterns**: [MVVM Community Toolkit](./mvvm-community-toolkit.instructions.md)

---

**Document Status**: ✅ Complete Value Converters Reference  
**Framework Version**: Avalonia UI 11.3.4  
**Last Updated**: 2025-09-14  
**Value Converters Owner**: MTM Development Team

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
