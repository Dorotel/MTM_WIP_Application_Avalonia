using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Badge control for displaying notifications, counts, or status indicators.
    /// Provides consistent badge appearance with customizable positioning.
    /// </summary>
    public class UniversalBadge : ContentControl
    {
        public static readonly StyledProperty<string> BadgeTextProperty =
            AvaloniaProperty.Register<UniversalBadge, string>(nameof(BadgeText), string.Empty);

        public static readonly StyledProperty<int> CountProperty =
            AvaloniaProperty.Register<UniversalBadge, int>(nameof(Count), 0);

        public static readonly StyledProperty<bool> ShowCountProperty =
            AvaloniaProperty.Register<UniversalBadge, bool>(nameof(ShowCount), true);

        public static readonly StyledProperty<Brush> BadgeBrushProperty =
            AvaloniaProperty.Register<UniversalBadge, Brush>(nameof(BadgeBrush), Brushes.Red);

        public static readonly StyledProperty<BadgePosition> PositionProperty =
            AvaloniaProperty.Register<UniversalBadge, BadgePosition>(nameof(Position), BadgePosition.TopRight);

        public static readonly StyledProperty<bool> IsVisibleProperty =
            AvaloniaProperty.Register<UniversalBadge, bool>(nameof(IsVisible), true);

        public static readonly StyledProperty<double> BadgeSizeProperty =
            AvaloniaProperty.Register<UniversalBadge, double>(nameof(BadgeSize), 20.0);

        /// <summary>
        /// Gets or sets the badge text content.
        /// </summary>
        public string BadgeText
        {
            get => GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }

        /// <summary>
        /// Gets or sets the count to display in the badge.
        /// </summary>
        public int Count
        {
            get => GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show the count or text.
        /// </summary>
        public bool ShowCount
        {
            get => GetValue(ShowCountProperty);
            set => SetValue(ShowCountProperty, value);
        }

        /// <summary>
        /// Gets or sets the badge background brush.
        /// </summary>
        public Brush BadgeBrush
        {
            get => GetValue(BadgeBrushProperty);
            set => SetValue(BadgeBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the badge position relative to content.
        /// </summary>
        public BadgePosition Position
        {
            get => GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        /// <summary>
        /// Gets or sets badge visibility.
        /// </summary>
        public bool IsVisible
        {
            get => GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets the badge size.
        /// </summary>
        public double BadgeSize
        {
            get => GetValue(BadgeSizeProperty);
            set => SetValue(BadgeSizeProperty, value);
        }
    }

    /// <summary>
    /// Badge positioning options.
    /// </summary>
    public enum BadgePosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }
}