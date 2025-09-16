using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Windows.Input;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Chip control for displaying tags, filters, or small data items.
    /// Provides consistent chip appearance with optional close functionality.
    /// </summary>
    public class UniversalChip : ContentControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<UniversalChip, string>(nameof(Text), string.Empty);

        public static readonly StyledProperty<bool> IsCloseableProperty =
            AvaloniaProperty.Register<UniversalChip, bool>(nameof(IsCloseable), false);

        public static readonly StyledProperty<ICommand?> CloseCommandProperty =
            AvaloniaProperty.Register<UniversalChip, ICommand?>(nameof(CloseCommand));

        public static readonly StyledProperty<Brush> ChipBrushProperty =
            AvaloniaProperty.Register<UniversalChip, Brush>(nameof(ChipBrush), Brushes.LightGray);

        public static readonly StyledProperty<ChipVariant> VariantProperty =
            AvaloniaProperty.Register<UniversalChip, ChipVariant>(nameof(Variant), ChipVariant.Default);

        /// <summary>
        /// Gets or sets the chip text content.
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the chip can be closed/removed.
        /// </summary>
        public bool IsCloseable
        {
            get => GetValue(IsCloseableProperty);
            set => SetValue(IsCloseableProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when the chip is closed.
        /// </summary>
        public ICommand? CloseCommand
        {
            get => GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the chip background brush.
        /// </summary>
        public Brush ChipBrush
        {
            get => GetValue(ChipBrushProperty);
            set => SetValue(ChipBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the chip variant for styling.
        /// </summary>
        public ChipVariant Variant
        {
            get => GetValue(VariantProperty);
            set => SetValue(VariantProperty, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            
            if (IsCloseable && CloseCommand?.CanExecute(this) == true)
            {
                CloseCommand.Execute(this);
            }
        }
    }

    /// <summary>
    /// Chip visual variants for different use cases.
    /// </summary>
    public enum ChipVariant
    {
        Default,
        Primary,
        Secondary,
        Success,
        Warning,
        Error,
        Info
    }
}