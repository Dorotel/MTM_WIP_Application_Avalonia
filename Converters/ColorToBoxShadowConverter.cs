using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace MTM_WIP_Application_Avalonia.Converters
{
    /// <summary>
    /// Converter that creates a BoxShadow from a Color or SolidColorBrush.
    /// Used to enable theme-aware BoxShadow properties by binding to theme color resources.
    /// </summary>
    public class ColorToBoxShadowConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Color shadowColor;

            // Extract Color from various input types
            if (value is Color color)
            {
                shadowColor = color;
            }
            else if (value is SolidColorBrush brush)
            {
                shadowColor = brush.Color;
            }
            else if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                if (Color.TryParse(colorString, out Color parsedColor))
                {
                    shadowColor = parsedColor;
                }
                else
                {
                    return new BoxShadows(); // Return empty shadow if parsing fails
                }
            }
            else
            {
                return new BoxShadows(); // Return empty shadow for invalid input
            }

            // Parse shadow parameters from parameter string or use defaults
            double offsetX = 0;
            double offsetY = 4;
            double blurRadius = 12;
            double spreadRadius = 0;

            // Parameter format: "offsetX,offsetY,blur,spread" (e.g., "0,2,8,0")
            if (parameter is string paramString && !string.IsNullOrEmpty(paramString))
            {
                var parts = paramString.Split(',');
                if (parts.Length >= 2)
                {
                    if (double.TryParse(parts[0], out double x)) offsetX = x;
                    if (double.TryParse(parts[1], out double y)) offsetY = y;
                }
                if (parts.Length >= 3)
                {
                    if (double.TryParse(parts[2], out double blur)) blurRadius = blur;
                }
                if (parts.Length >= 4)
                {
                    if (double.TryParse(parts[3], out double spread)) spreadRadius = spread;
                }
            }

            // Create and return BoxShadows with the theme-aware color
            return new BoxShadows(new BoxShadow
            {
                OffsetX = offsetX,
                OffsetY = offsetY,
                Blur = blurRadius,
                Spread = spreadRadius,
                Color = shadowColor
            });
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // ConvertBack is not typically needed for BoxShadow scenarios
            throw new NotImplementedException("ConvertBack is not supported for ColorToBoxShadowConverter");
        }
    }
}