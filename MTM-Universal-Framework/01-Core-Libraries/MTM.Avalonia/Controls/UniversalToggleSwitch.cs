using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Toggle Switch with customizable styling and accessibility support.
    /// Provides consistent toggle experience across platforms with smooth animations.
    /// </summary>
    public class UniversalToggleSwitch : UserControl
    {
        public static readonly StyledProperty<bool> IsToggledProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, bool>(nameof(IsToggled), false);

        public static readonly StyledProperty<string> OnTextProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, string>(nameof(OnText), "ON");

        public static readonly StyledProperty<string> OffTextProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, string>(nameof(OffText), "OFF");

        public static readonly StyledProperty<Brush> OnBrushProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, Brush>(nameof(OnBrush), Brushes.Green);

        public static readonly StyledProperty<Brush> OffBrushProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, Brush>(nameof(OffBrush), Brushes.Gray);

        public static readonly StyledProperty<bool> ShowLabelsProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, bool>(nameof(ShowLabels), true);

        public static readonly StyledProperty<double> SwitchWidthProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, double>(nameof(SwitchWidth), 50);

        public static readonly StyledProperty<double> SwitchHeightProperty =
            AvaloniaProperty.Register<UniversalToggleSwitch, double>(nameof(SwitchHeight), 26);

        public bool IsToggled
        {
            get => GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        public string OnText
        {
            get => GetValue(OnTextProperty);
            set => SetValue(OnTextProperty, value);
        }

        public string OffText
        {
            get => GetValue(OffTextProperty);
            set => SetValue(OffTextProperty, value);
        }

        public Brush OnBrush
        {
            get => GetValue(OnBrushProperty);
            set => SetValue(OnBrushProperty, value);
        }

        public Brush OffBrush
        {
            get => GetValue(OffBrushProperty);
            set => SetValue(OffBrushProperty, value);
        }

        public bool ShowLabels
        {
            get => GetValue(ShowLabelsProperty);
            set => SetValue(ShowLabelsProperty, value);
        }

        public double SwitchWidth
        {
            get => GetValue(SwitchWidthProperty);
            set => SetValue(SwitchWidthProperty, value);
        }

        public double SwitchHeight
        {
            get => GetValue(SwitchHeightProperty);
            set => SetValue(SwitchHeightProperty, value);
        }

        public UniversalToggleSwitch()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Spacing = 12,
                Children =
                {
                    new ToggleSwitch
                    {
                        [!ToggleSwitch.IsCheckedProperty] = this.GetObservable(IsToggledProperty).ToBinding(),
                        [!ToggleSwitch.OnContentProperty] = this.GetObservable(OnTextProperty).ToBinding(),
                        [!ToggleSwitch.OffContentProperty] = this.GetObservable(OffTextProperty).ToBinding(),
                        Width = 60,
                        Height = 32
                    }
                }
            };
        }
    }
}