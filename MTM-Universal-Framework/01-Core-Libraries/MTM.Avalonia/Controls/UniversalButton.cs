using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Button with customizable styling and cross-platform touch optimization.
    /// Provides consistent button appearance across Windows, macOS, Linux, and Android.
    /// </summary>
    public class UniversalButton : Button
    {
        public static readonly StyledProperty<Brush> PrimaryBrushProperty =
            AvaloniaProperty.Register<UniversalButton, Brush>(nameof(PrimaryBrush), Brushes.DodgerBlue);

        public static readonly StyledProperty<Brush> HoverBrushProperty =
            AvaloniaProperty.Register<UniversalButton, Brush>(nameof(HoverBrush), Brushes.CornflowerBlue);

        public static readonly StyledProperty<double> CornerRadiusProperty =
            AvaloniaProperty.Register<UniversalButton, double>(nameof(CornerRadius), 8.0);

        public static readonly StyledProperty<bool> IsRoundedProperty =
            AvaloniaProperty.Register<UniversalButton, bool>(nameof(IsRounded), false);

        public static readonly StyledProperty<ButtonVariant> VariantProperty =
            AvaloniaProperty.Register<UniversalButton, ButtonVariant>(nameof(Variant), ButtonVariant.Primary);

        /// <summary>
        /// Primary color brush for the button
        /// </summary>
        public Brush PrimaryBrush
        {
            get => GetValue(PrimaryBrushProperty);
            set => SetValue(PrimaryBrushProperty, value);
        }

        /// <summary>
        /// Hover color brush for the button
        /// </summary>
        public Brush HoverBrush
        {
            get => GetValue(HoverBrushProperty);
            set => SetValue(HoverBrushProperty, value);
        }

        /// <summary>
        /// Corner radius for rounded buttons
        /// </summary>
        public double CornerRadius
        {
            get => GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Whether the button should be fully rounded (circular for square buttons)
        /// </summary>
        public bool IsRounded
        {
            get => GetValue(IsRoundedProperty);
            set => SetValue(IsRoundedProperty, value);
        }

        /// <summary>
        /// Button style variant
        /// </summary>
        public ButtonVariant Variant
        {
            get => GetValue(VariantProperty);
            set => SetValue(VariantProperty, value);
        }

        static UniversalButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UniversalButton), 
                new StyledPropertyMetadata(typeof(UniversalButton)));
        }

        public UniversalButton()
        {
            UpdateButtonStyle();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == VariantProperty ||
                change.Property == PrimaryBrushProperty ||
                change.Property == HoverBrushProperty ||
                change.Property == CornerRadiusProperty ||
                change.Property == IsRoundedProperty)
            {
                UpdateButtonStyle();
            }
        }

        private void UpdateButtonStyle()
        {
            // Apply styling based on properties
            Classes.Set("primary", Variant == ButtonVariant.Primary);
            Classes.Set("secondary", Variant == ButtonVariant.Secondary);
            Classes.Set("success", Variant == ButtonVariant.Success);
            Classes.Set("warning", Variant == ButtonVariant.Warning);
            Classes.Set("danger", Variant == ButtonVariant.Danger);
            Classes.Set("rounded", IsRounded);
        }
    }

    /// <summary>
    /// Button style variants
    /// </summary>
    public enum ButtonVariant
    {
        Primary,
        Secondary,
        Success,
        Warning,
        Danger,
        Info,
        Light,
        Dark
    }
}