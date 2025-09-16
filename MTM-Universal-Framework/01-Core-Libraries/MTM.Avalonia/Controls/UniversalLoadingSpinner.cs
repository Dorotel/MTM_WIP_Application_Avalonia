using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Loading Spinner with customizable styling and animation.
    /// Provides consistent loading indication across all platforms.
    /// </summary>
    public class UniversalLoadingSpinner : UserControl
    {
        public static readonly StyledProperty<bool> IsLoadingProperty =
            AvaloniaProperty.Register<UniversalLoadingSpinner, bool>(nameof(IsLoading), false);

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<UniversalLoadingSpinner, string>(nameof(Message), "Loading...");

        public static readonly StyledProperty<SpinnerSize> SizeProperty =
            AvaloniaProperty.Register<UniversalLoadingSpinner, SpinnerSize>(nameof(Size), SpinnerSize.Medium);

        public static readonly StyledProperty<SpinnerType> TypeProperty =
            AvaloniaProperty.Register<UniversalLoadingSpinner, SpinnerType>(nameof(Type), SpinnerType.Dots);

        private Grid _container;
        private ProgressBar _progressBar;
        private TextBlock _messageBlock;

        /// <summary>
        /// Whether the loading spinner is currently active
        /// </summary>
        public bool IsLoading
        {
            get => GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        /// <summary>
        /// Message to display below the spinner
        /// </summary>
        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>
        /// Size of the loading spinner
        /// </summary>
        public SpinnerSize Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        /// <summary>
        /// Type/style of the loading spinner
        /// </summary>
        public SpinnerType Type
        {
            get => GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public UniversalLoadingSpinner()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _container = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            _container.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            _container.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            // Progress indicator
            _progressBar = new ProgressBar
            {
                IsIndeterminate = true,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(_progressBar, 0);
            _container.Children.Add(_progressBar);

            // Message
            _messageBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 12, 0, 0)
            };
            Grid.SetRow(_messageBlock, 1);
            _container.Children.Add(_messageBlock);

            Content = _container;
            UpdateSpinnerDisplay();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsLoadingProperty ||
                change.Property == MessageProperty ||
                change.Property == SizeProperty ||
                change.Property == TypeProperty)
            {
                UpdateSpinnerDisplay();
            }
        }

        private void UpdateSpinnerDisplay()
        {
            if (_container == null) return;

            // Visibility
            IsVisible = IsLoading;

            if (_messageBlock != null)
            {
                _messageBlock.Text = Message;
                _messageBlock.IsVisible = !string.IsNullOrEmpty(Message);
            }

            if (_progressBar != null)
            {
                // Size configuration
                var spinnerSize = Size switch
                {
                    SpinnerSize.Small => 24,
                    SpinnerSize.Medium => 32,
                    SpinnerSize.Large => 48,
                    SpinnerSize.ExtraLarge => 64,
                    _ => 32
                };

                _progressBar.Width = spinnerSize * 3; // Width is typically 3x height for progress bars
                _progressBar.Height = 4; // Thin progress bar

                // For circular spinner, we'd make it square
                if (Type == SpinnerType.Circle)
                {
                    _progressBar.Width = spinnerSize;
                    _progressBar.Height = spinnerSize;
                }
            }

            // Apply styling classes
            Classes.Set("small", Size == SpinnerSize.Small);
            Classes.Set("medium", Size == SpinnerSize.Medium);
            Classes.Set("large", Size == SpinnerSize.Large);
            Classes.Set("extra-large", Size == SpinnerSize.ExtraLarge);
            Classes.Set("dots", Type == SpinnerType.Dots);
            Classes.Set("bar", Type == SpinnerType.Bar);
            Classes.Set("circle", Type == SpinnerType.Circle);
        }
    }

    /// <summary>
    /// Loading spinner size options
    /// </summary>
    public enum SpinnerSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    /// <summary>
    /// Loading spinner type/style options
    /// </summary>
    public enum SpinnerType
    {
        Dots,
        Bar,
        Circle,
        Pulse
    }
}