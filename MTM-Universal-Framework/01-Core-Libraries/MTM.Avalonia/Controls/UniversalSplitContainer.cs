using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Split Container control for resizable panel layouts.
    /// Provides consistent split container behavior with touch and mouse support.
    /// </summary>
    public class UniversalSplitContainer : ContentControl
    {
        public static readonly StyledProperty<object?> Panel1Property =
            AvaloniaProperty.Register<UniversalSplitContainer, object?>(nameof(Panel1));

        public static readonly StyledProperty<object?> Panel2Property =
            AvaloniaProperty.Register<UniversalSplitContainer, object?>(nameof(Panel2));

        public static readonly StyledProperty<SplitOrientation> OrientationProperty =
            AvaloniaProperty.Register<UniversalSplitContainer, SplitOrientation>(nameof(Orientation), SplitOrientation.Vertical);

        public static readonly StyledProperty<double> SplitterDistanceProperty =
            AvaloniaProperty.Register<UniversalSplitContainer, double>(nameof(SplitterDistance), 100.0);

        public static readonly StyledProperty<double> SplitterWidthProperty =
            AvaloniaProperty.Register<UniversalSplitContainer, double>(nameof(SplitterWidth), 6.0);

        public static readonly StyledProperty<bool> IsSplitterFixedProperty =
            AvaloniaProperty.Register<UniversalSplitContainer, bool>(nameof(IsSplitterFixed), false);

        public static readonly StyledProperty<double> MinPanel1SizeProperty =
            AvaloniaProperty.Register<UniversalSplitContainer, double>(nameof(MinPanel1Size), 25.0);

        public static readonly StyledProperty<double> MinPanel2SizeProperty =
            AvaloniaProperty.Register<UniversalSplitContainer, double>(nameof(MinPanel2Size), 25.0);

        /// <summary>
        /// Gets or sets the content of the first panel.
        /// </summary>
        public object? Panel1
        {
            get => GetValue(Panel1Property);
            set => SetValue(Panel1Property, value);
        }

        /// <summary>
        /// Gets or sets the content of the second panel.
        /// </summary>
        public object? Panel2
        {
            get => GetValue(Panel2Property);
            set => SetValue(Panel2Property, value);
        }

        /// <summary>
        /// Gets or sets the split orientation.
        /// </summary>
        public SplitOrientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the distance of the splitter from the top or left edge.
        /// </summary>
        public double SplitterDistance
        {
            get => GetValue(SplitterDistanceProperty);
            set => SetValue(SplitterDistanceProperty, value);
        }

        /// <summary>
        /// Gets or sets the width/height of the splitter.
        /// </summary>
        public double SplitterWidth
        {
            get => GetValue(SplitterWidthProperty);
            set => SetValue(SplitterWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the splitter is fixed in position.
        /// </summary>
        public bool IsSplitterFixed
        {
            get => GetValue(IsSplitterFixedProperty);
            set => SetValue(IsSplitterFixedProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum size for Panel1.
        /// </summary>
        public double MinPanel1Size
        {
            get => GetValue(MinPanel1SizeProperty);
            set => SetValue(MinPanel1SizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum size for Panel2.
        /// </summary>
        public double MinPanel2Size
        {
            get => GetValue(MinPanel2SizeProperty);
            set => SetValue(MinPanel2SizeProperty, value);
        }
    }

    /// <summary>
    /// Split container orientation options.
    /// </summary>
    public enum SplitOrientation
    {
        Vertical,
        Horizontal
    }
}