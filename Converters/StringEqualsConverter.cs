using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MTM_WIP_Application_Avalonia.Converters
{
    /// <summary>
    /// Converter that compares a string value with a parameter and returns true if they are equal
    /// </summary>
    public class StringEqualsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null || parameter is null)
                return false;
                
            return string.Equals(value.ToString(), parameter.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}