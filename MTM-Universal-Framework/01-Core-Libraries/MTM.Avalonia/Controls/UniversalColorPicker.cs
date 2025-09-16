using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Color Picker control for selecting colors with various input methods.
    /// Provides consistent color selection interface across platforms.
    /// </summary>
    public class UniversalColorPicker : ContentControl
    {
        public static readonly StyledProperty<Color> SelectedColorProperty =
            AvaloniaProperty.Register<UniversalColorPicker, Color>(nameof(SelectedColor), Colors.White, 
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> ShowAlphaProperty =
            AvaloniaProperty.Register<UniversalColorPicker, bool>(nameof(ShowAlpha), true);

        public static readonly StyledProperty<bool> ShowHexInputProperty =
            AvaloniaProperty.Register<UniversalColorPicker, bool>(nameof(ShowHexInput), true);

        public static readonly StyledProperty<bool> ShowRGBInputProperty =
            AvaloniaProperty.Register<UniversalColorPicker, bool>(nameof(ShowRGBInput), true);

        public static readonly StyledProperty<bool> ShowPresetColorsProperty =
            AvaloniaProperty.Register<UniversalColorPicker, bool>(nameof(ShowPresetColors), true);

        public static readonly StyledProperty<IEnumerable<Color>?> PresetColorsProperty =
            AvaloniaProperty.Register<UniversalColorPicker, IEnumerable<Color>?>(nameof(PresetColors));

        public static readonly StyledProperty<ColorPickerMode> ModeProperty =
            AvaloniaProperty.Register<UniversalColorPicker, ColorPickerMode>(nameof(Mode), ColorPickerMode.Full);

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show alpha channel control.
        /// </summary>
        public bool ShowAlpha
        {
            get => GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show hex input field.
        /// </summary>
        public bool ShowHexInput
        {
            get => GetValue(ShowHexInputProperty);
            set => SetValue(ShowHexInputProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show RGB input fields.
        /// </summary>
        public bool ShowRGBInput
        {
            get => GetValue(ShowRGBInputProperty);
            set => SetValue(ShowRGBInputProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show preset color palette.
        /// </summary>
        public bool ShowPresetColors
        {
            get => GetValue(ShowPresetColorsProperty);
            set => SetValue(ShowPresetColorsProperty, value);
        }

        /// <summary>
        /// Gets or sets the preset colors collection.
        /// </summary>
        public IEnumerable<Color>? PresetColors
        {
            get => GetValue(PresetColorsProperty);
            set => SetValue(PresetColorsProperty, value);
        }

        /// <summary>
        /// Gets or sets the color picker mode.
        /// </summary>
        public ColorPickerMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Gets the hex string representation of the selected color.
        /// </summary>
        public string HexValue => SelectedColor.ToString();

        /// <summary>
        /// Gets the RGB values of the selected color.
        /// </summary>
        public (byte R, byte G, byte B) RGBValues => (SelectedColor.R, SelectedColor.G, SelectedColor.B);

        public UniversalColorPicker()
        {
            // Set default preset colors
            PresetColors = new[]
            {
                Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow,
                Colors.Orange, Colors.Purple, Colors.Pink, Colors.Brown,
                Colors.Black, Colors.Gray, Colors.LightGray, Colors.White
            };
        }
    }

    /// <summary>
    /// Color picker display modes.
    /// </summary>
    public enum ColorPickerMode
    {
        Full,
        Compact,
        Palette,
        Swatch
    }
}