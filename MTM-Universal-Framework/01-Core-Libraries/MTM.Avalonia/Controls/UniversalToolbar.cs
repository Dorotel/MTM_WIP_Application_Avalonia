using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Toolbar control for organizing action buttons and tools.
    /// Provides consistent toolbar appearance with customizable button groups.
    /// </summary>
    public class UniversalToolbar : ItemsControl
    {
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<UniversalToolbar, Orientation>(nameof(Orientation), Orientation.Horizontal);

        public static readonly StyledProperty<bool> ShowLabelsProperty =
            AvaloniaProperty.Register<UniversalToolbar, bool>(nameof(ShowLabels), true);

        public static readonly StyledProperty<bool> ShowSeparatorsProperty =
            AvaloniaProperty.Register<UniversalToolbar, bool>(nameof(ShowSeparators), true);

        public static readonly StyledProperty<double> ButtonSizeProperty =
            AvaloniaProperty.Register<UniversalToolbar, double>(nameof(ButtonSize), 32.0);

        public static readonly StyledProperty<ToolbarStyle> StyleProperty =
            AvaloniaProperty.Register<UniversalToolbar, ToolbarStyle>(nameof(Style), ToolbarStyle.Standard);

        /// <summary>
        /// Gets or sets the toolbar orientation.
        /// </summary>
        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show button labels.
        /// </summary>
        public bool ShowLabels
        {
            get => GetValue(ShowLabelsProperty);
            set => SetValue(ShowLabelsProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show separators between button groups.
        /// </summary>
        public bool ShowSeparators
        {
            get => GetValue(ShowSeparatorsProperty);
            set => SetValue(ShowSeparatorsProperty, value);
        }

        /// <summary>
        /// Gets or sets the toolbar button size.
        /// </summary>
        public double ButtonSize
        {
            get => GetValue(ButtonSizeProperty);
            set => SetValue(ButtonSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the toolbar style.
        /// </summary>
        public ToolbarStyle Style
        {
            get => GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }
    }

    /// <summary>
    /// Universal Toolbar Button.
    /// </summary>
    public class UniversalToolbarButton : Button
    {
        public static readonly StyledProperty<object?> IconProperty =
            AvaloniaProperty.Register<UniversalToolbarButton, object?>(nameof(Icon));

        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<UniversalToolbarButton, string>(nameof(Label), string.Empty);

        public static readonly StyledProperty<string> TooltipTextProperty =
            AvaloniaProperty.Register<UniversalToolbarButton, string>(nameof(TooltipText), string.Empty);

        public static readonly StyledProperty<bool> ShowLabelProperty =
            AvaloniaProperty.Register<UniversalToolbarButton, bool>(nameof(ShowLabel), true);

        /// <summary>
        /// Gets or sets the button icon.
        /// </summary>
        public object? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// Gets or sets the button label.
        /// </summary>
        public string Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Gets or sets the tooltip text.
        /// </summary>
        public string TooltipText
        {
            get => GetValue(TooltipTextProperty);
            set => SetValue(TooltipTextProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show the label.
        /// </summary>
        public bool ShowLabel
        {
            get => GetValue(ShowLabelProperty);
            set => SetValue(ShowLabelProperty, value);
        }
    }

    /// <summary>
    /// Toolbar button data model.
    /// </summary>
    public class ToolbarItem
    {
        public string Label { get; set; } = string.Empty;
        public object? Icon { get; set; }
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public string TooltipText { get; set; } = string.Empty;
        public bool IsSeparator { get; set; }
    }

    /// <summary>
    /// Toolbar visual styles.
    /// </summary>
    public enum ToolbarStyle
    {
        Standard,
        Flat,
        Raised,
        Transparent
    }
}