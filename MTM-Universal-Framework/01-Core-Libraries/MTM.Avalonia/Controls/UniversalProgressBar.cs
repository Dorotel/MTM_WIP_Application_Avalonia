using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Progress Bar with customizable styling and labeling.
    /// Provides consistent progress indication across all platforms.
    /// </summary>
    public class UniversalProgressBar : UserControl
    {
        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<UniversalProgressBar, double>(nameof(Value), 0.0);

        public static readonly StyledProperty<double> MinimumProperty =
            AvaloniaProperty.Register<UniversalProgressBar, double>(nameof(Minimum), 0.0);

        public static readonly StyledProperty<double> MaximumProperty =
            AvaloniaProperty.Register<UniversalProgressBar, double>(nameof(Maximum), 100.0);

        public static readonly StyledProperty<bool> IsIndeterminateProperty =
            AvaloniaProperty.Register<UniversalProgressBar, bool>(nameof(IsIndeterminate), false);

        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<UniversalProgressBar, string>(nameof(Label), string.Empty);

        public static readonly StyledProperty<bool> ShowPercentageProperty =
            AvaloniaProperty.Register<UniversalProgressBar, bool>(nameof(ShowPercentage), false);

        public static readonly StyledProperty<ProgressBarStyle> StyleProperty =
            AvaloniaProperty.Register<UniversalProgressBar, ProgressBarStyle>(nameof(Style), ProgressBarStyle.Default);

        public static readonly StyledProperty<double> HeightProperty =
            AvaloniaProperty.Register<UniversalProgressBar, double>(nameof(Height), 8.0);

        private StackPanel _container;
        private TextBlock _labelBlock;
        private ProgressBar _progressBar;
        private TextBlock _percentageBlock;

        /// <summary>
        /// Current progress value
        /// </summary>
        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Minimum progress value
        /// </summary>
        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// Maximum progress value
        /// </summary>
        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// Whether the progress bar shows indeterminate progress
        /// </summary>
        public bool IsIndeterminate
        {
            get => GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        /// <summary>
        /// Label text displayed above the progress bar
        /// </summary>
        public string Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Whether to show percentage text
        /// </summary>
        public bool ShowPercentage
        {
            get => GetValue(ShowPercentageProperty);
            set => SetValue(ShowPercentageProperty, value);
        }

        /// <summary>
        /// Visual style of the progress bar
        /// </summary>
        public new ProgressBarStyle Style
        {
            get => GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }

        /// <summary>
        /// Height of the progress bar
        /// </summary>
        public new double Height
        {
            get => GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public UniversalProgressBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _container = new StackPanel
            {
                Spacing = 6
            };

            // Label
            _labelBlock = new TextBlock
            {
                FontWeight = FontWeight.SemiBold,
                IsVisible = false
            };
            _container.Children.Add(_labelBlock);

            // Progress bar container with percentage
            var progressContainer = new Grid();
            progressContainer.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            progressContainer.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            _progressBar = new ProgressBar
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(_progressBar, 0);
            progressContainer.Children.Add(_progressBar);

            _percentageBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0),
                FontFamily = new FontFamily("monospace"), // For consistent width
                IsVisible = false
            };
            Grid.SetColumn(_percentageBlock, 1);
            progressContainer.Children.Add(_percentageBlock);

            _container.Children.Add(progressContainer);

            Content = _container;
            UpdateProgressDisplay();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ValueProperty ||
                change.Property == MinimumProperty ||
                change.Property == MaximumProperty ||
                change.Property == IsIndeterminateProperty ||
                change.Property == LabelProperty ||
                change.Property == ShowPercentageProperty ||
                change.Property == StyleProperty ||
                change.Property == HeightProperty)
            {
                UpdateProgressDisplay();
            }
        }

        private void UpdateProgressDisplay()
        {
            if (_progressBar == null) return;

            // Progress bar properties
            _progressBar.Value = Value;
            _progressBar.Minimum = Minimum;
            _progressBar.Maximum = Maximum;
            _progressBar.IsIndeterminate = IsIndeterminate;
            _progressBar.Height = Height;

            // Label
            if (_labelBlock != null)
            {
                _labelBlock.Text = Label;
                _labelBlock.IsVisible = !string.IsNullOrEmpty(Label);
            }

            // Percentage
            if (_percentageBlock != null)
            {
                _percentageBlock.IsVisible = ShowPercentage && !IsIndeterminate;
                if (ShowPercentage && !IsIndeterminate)
                {
                    var range = Maximum - Minimum;
                    var percentage = range > 0 ? ((Value - Minimum) / range) * 100 : 0;
                    _percentageBlock.Text = $"{percentage:F0}%";
                }
            }

            // Styling based on style property
            Classes.Set("default", Style == ProgressBarStyle.Default);
            Classes.Set("success", Style == ProgressBarStyle.Success);
            Classes.Set("warning", Style == ProgressBarStyle.Warning);
            Classes.Set("error", Style == ProgressBarStyle.Error);
            Classes.Set("info", Style == ProgressBarStyle.Info);
            Classes.Set("thin", Height <= 4);
            Classes.Set("thick", Height >= 16);

            // Apply color based on style
            if (_progressBar != null)
            {
                var foreground = Style switch
                {
                    ProgressBarStyle.Success => Brushes.Green,
                    ProgressBarStyle.Warning => Brushes.Orange,
                    ProgressBarStyle.Error => Brushes.Red,
                    ProgressBarStyle.Info => Brushes.DodgerBlue,
                    _ => null
                };

                if (foreground != null)
                    _progressBar.Foreground = foreground;
            }
        }

        /// <summary>
        /// Calculate percentage from current value
        /// </summary>
        public double GetPercentage()
        {
            var range = Maximum - Minimum;
            return range > 0 ? ((Value - Minimum) / range) * 100 : 0;
        }

        /// <summary>
        /// Set progress from percentage (0-100)
        /// </summary>
        public void SetPercentage(double percentage)
        {
            var range = Maximum - Minimum;
            Value = Minimum + (percentage / 100.0) * range;
        }
    }

    /// <summary>
    /// Progress bar style options
    /// </summary>
    public enum ProgressBarStyle
    {
        Default,
        Success,
        Warning,
        Error,
        Info
    }
}