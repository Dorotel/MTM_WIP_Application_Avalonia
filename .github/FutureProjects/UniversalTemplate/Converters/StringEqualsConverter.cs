using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace YourProject.Converters
{
    /// <summary>
    /// Universal string equality converter for Avalonia UI binding scenarios.
    /// Compares a string value against a parameter value and returns boolean result.
    /// Commonly used for controlling visibility, styling, or behavior based on string values.
    /// </summary>
    public class StringEqualsConverter : IValueConverter
    {
        /// <summary>
        /// Static instance for reuse across the application
        /// </summary>
        public static readonly StringEqualsConverter Instance = new StringEqualsConverter();

        /// <summary>
        /// Gets or sets whether the string comparison should ignore case.
        /// Default is false (case-sensitive comparison).
        /// </summary>
        public bool IgnoreCase { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the comparison result should be inverted.
        /// When false (default): equal strings → true, different strings → false
        /// When true: equal strings → false, different strings → true
        /// </summary>
        public bool Invert { get; set; } = false;

        /// <summary>
        /// Converts a string value by comparing it to the parameter value.
        /// </summary>
        /// <param name="value">The string value to compare</param>
        /// <param name="targetType">The target type (typically bool)</param>
        /// <param name="parameter">The comparison value (string)</param>
        /// <param name="culture">Culture information for string comparison</param>
        /// <returns>Boolean result of the string comparison</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? valueString = value?.ToString();
            string? parameterString = parameter?.ToString();

            bool areEqual;
            
            if (valueString == null && parameterString == null)
            {
                areEqual = true;
            }
            else if (valueString == null || parameterString == null)
            {
                areEqual = false;
            }
            else
            {
                StringComparison comparison = IgnoreCase 
                    ? StringComparison.OrdinalIgnoreCase 
                    : StringComparison.Ordinal;
                    
                areEqual = string.Equals(valueString, parameterString, comparison);
            }

            return Invert ? !areEqual : areEqual;
        }

        /// <summary>
        /// Reverse conversion is not supported for string equality operations.
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}