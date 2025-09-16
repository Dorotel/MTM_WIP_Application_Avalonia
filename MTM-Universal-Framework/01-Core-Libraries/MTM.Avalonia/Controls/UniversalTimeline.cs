using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Timeline control for displaying chronological events or steps.
    /// Provides consistent timeline appearance with customizable items.
    /// </summary>
    public class UniversalTimeline : ItemsControl
    {
        public static readonly StyledProperty<Brush> LineColorProperty =
            AvaloniaProperty.Register<UniversalTimeline, Brush>(nameof(LineColor), Brushes.Gray);

        public static readonly StyledProperty<double> LineThicknessProperty =
            AvaloniaProperty.Register<UniversalTimeline, double>(nameof(LineThickness), 2.0);

        public static readonly StyledProperty<TimelineOrientation> OrientationProperty =
            AvaloniaProperty.Register<UniversalTimeline, TimelineOrientation>(nameof(Orientation), TimelineOrientation.Vertical);

        public static readonly StyledProperty<double> ItemSpacingProperty =
            AvaloniaProperty.Register<UniversalTimeline, double>(nameof(ItemSpacing), 20.0);

        /// <summary>
        /// Gets or sets the timeline line color.
        /// </summary>
        public Brush LineColor
        {
            get => GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the timeline line thickness.
        /// </summary>
        public double LineThickness
        {
            get => GetValue(LineThicknessProperty);
            set => SetValue(LineThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the timeline orientation.
        /// </summary>
        public TimelineOrientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the spacing between timeline items.
        /// </summary>
        public double ItemSpacing
        {
            get => GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }
    }

    /// <summary>
    /// Timeline item data model.
    /// </summary>
    public class TimelineItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public object? Content { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }
        public Brush? IndicatorBrush { get; set; }
        public object? Icon { get; set; }
    }

    /// <summary>
    /// Timeline orientation options.
    /// </summary>
    public enum TimelineOrientation
    {
        Vertical,
        Horizontal
    }
}