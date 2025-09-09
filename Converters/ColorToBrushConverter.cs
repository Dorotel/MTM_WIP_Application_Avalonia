using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace MTM_WIP_Application_Avalonia.Converters
{
    /// <summary>
    /// Converter that creates a SolidColorBrush from a Color.
    /// Used to enable proper binding of Color properties to Background attributes.
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }
            else if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                if (Color.TryParse(colorString, out Color parsedColor))
                {
                    return new SolidColorBrush(parsedColor);
                }
            }
            
            // Return transparent brush for invalid input
            return new SolidColorBrush(Colors.Transparent);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color;
            }
            
            throw new NotImplementedException("ConvertBack is not fully supported for ColorToBrushConverter");
        }
    }
}