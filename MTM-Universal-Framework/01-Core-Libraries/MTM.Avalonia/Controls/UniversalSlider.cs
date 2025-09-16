using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Slider control with enhanced styling and value display options.
    /// Provides consistent slider appearance with customizable formatting.
    /// </summary>
    public class UniversalSlider : Slider
    {
        public static readonly StyledProperty<bool> ShowValueProperty =
            AvaloniaProperty.Register<UniversalSlider, bool>(nameof(ShowValue), true);

        public static readonly StyledProperty<string> ValueFormatProperty =
            AvaloniaProperty.Register<UniversalSlider, string>(nameof(ValueFormat), "F0");

        public static readonly StyledProperty<string> SuffixProperty =
            AvaloniaProperty.Register<UniversalSlider, string>(nameof(Suffix), string.Empty);

        public static readonly StyledProperty<bool> ShowTicksProperty =
            AvaloniaProperty.Register<UniversalSlider, bool>(nameof(ShowTicks), false);

        public static readonly StyledProperty<double> TickFrequencyProperty =
            AvaloniaProperty.Register<UniversalSlider, double>(nameof(TickFrequency), 1.0);

        public static readonly StyledProperty<Brush> ThumbBrushProperty =
            AvaloniaProperty.Register<UniversalSlider, Brush>(nameof(ThumbBrush), Brushes.DodgerBlue);

        public static readonly StyledProperty<Brush> TrackBrushProperty =
            AvaloniaProperty.Register<UniversalSlider, Brush>(nameof(TrackBrush), Brushes.LightGray);

        public static readonly StyledProperty<SliderValuePosition> ValuePositionProperty =
            AvaloniaProperty.Register<UniversalSlider, SliderValuePosition>(nameof(ValuePosition), SliderValuePosition.Above);

        /// <summary>
        /// Gets or sets whether to show the current value.
        /// </summary>
        public bool ShowValue
        {
            get => GetValue(ShowValueProperty);
            set => SetValue(ShowValueProperty, value);
        }

        /// <summary>
        /// Gets or sets the format string for the value display.
        /// </summary>
        public string ValueFormat
        {
            get => GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the suffix to display after the value.
        /// </summary>
        public string Suffix
        {
            get => GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show tick marks.
        /// </summary>
        public bool ShowTicks
        {
            get => GetValue(ShowTicksProperty);
            set => SetValue(ShowTicksProperty, value);
        }

        /// <summary>
        /// Gets or sets the frequency of tick marks.
        /// </summary>
        public double TickFrequency
        {
            get => GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }

        /// <summary>
        /// Gets or sets the slider thumb brush.
        /// </summary>
        public Brush ThumbBrush
        {
            get => GetValue(ThumbBrushProperty);
            set => SetValue(ThumbBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the slider track brush.
        /// </summary>
        public Brush TrackBrush
        {
            get => GetValue(TrackBrushProperty);
            set => SetValue(TrackBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the position of the value display.
        /// </summary>
        public SliderValuePosition ValuePosition
        {
            get => GetValue(ValuePositionProperty);
            set => SetValue(ValuePositionProperty, value);
        }

        /// <summary>
        /// Gets the formatted value string for display.
        /// </summary>
        public string FormattedValue => Value.ToString(ValueFormat) + Suffix;
    }

    /// <summary>
    /// Slider value display positions.
    /// </summary>
    public enum SliderValuePosition
    {
        Above,
        Below,
        Left,
        Right,
        OnThumb
    }
}