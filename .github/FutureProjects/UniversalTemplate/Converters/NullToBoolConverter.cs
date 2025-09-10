using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace YourProject.Converters
{
    /// <summary>
    /// Universal Avalonia value converter that converts null values to boolean values for UI binding scenarios.
    /// Commonly used to control visibility, enable/disable states, and validation styling
    /// based on whether data is available.
    /// 
    /// This converter supports both normal and inverted conversion modes:
    /// - Normal mode: null → false, non-null → true
    /// - Inverted mode: null → true, non-null → false
    /// 
    /// Typical usage scenarios:
    /// - Show loading indicators when data is null
    /// - Enable save buttons only when required fields are not null
    /// - Display error messages when validation fails (returns null)
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Static instance for reuse across the application to improve performance
        /// and reduce object allocation in UI scenarios with frequent updates.
        /// </summary>
        public static readonly NullToBoolConverter Instance = new NullToBoolConverter();
        
        /// <summary>
        /// Gets or sets whether the conversion logic should be inverted.
        /// When false (default): null → false, non-null → true
        /// When true: null → true, non-null → false
        /// Useful for scenarios like hiding elements when data exists versus showing when data is missing.
        /// </summary>
        public bool Invert { get; set; } = false;

        /// <summary>
        /// Converts a value to a boolean based on null checking, with optional inversion.
        /// Used extensively in UI to control element states based on data availability.
        /// </summary>
        /// <param name="value">The value to check for null (typically data objects)</param>
        /// <param name="targetType">The target type (typically bool for UI binding)</param>
        /// <param name="parameter">Optional parameter (not used in this converter)</param>
        /// <param name="culture">Culture information for conversion (not used in this converter)</param>
        /// <returns>Boolean result based on null check and inversion setting</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool result = value != null;
            return Invert ? !result : result;
        }

        /// <summary>
        /// Reverse conversion is not supported for null-to-boolean operations.
        /// This converter is designed for one-way binding scenarios in UI.
        /// </summary>
        /// <param name="value">The boolean value to convert back</param>
        /// <param name="targetType">The target type for reverse conversion</param>
        /// <param name="parameter">Optional parameter (not used)</param>
        /// <param name="culture">Culture information (not used)</param>
        /// <exception cref="NotSupportedException">Always thrown as reverse conversion is not meaningful</exception>
        /// <returns>Never returns as exception is always thrown</returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}