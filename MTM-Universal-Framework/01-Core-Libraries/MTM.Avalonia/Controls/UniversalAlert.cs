using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Notification/Alert component for displaying messages.
    /// Provides consistent notification appearance across all platforms.
    /// </summary>
    public class UniversalAlert : UserControl
    {
        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<UniversalAlert, string>(nameof(Message), string.Empty);

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<UniversalAlert, string>(nameof(Title), string.Empty);

        public static readonly StyledProperty<AlertType> TypeProperty =
            AvaloniaProperty.Register<UniversalAlert, AlertType>(nameof(Type), AlertType.Info);

        public static readonly StyledProperty<bool> IsDismissibleProperty =
            AvaloniaProperty.Register<UniversalAlert, bool>(nameof(IsDismissible), true);

        public static readonly StyledProperty<bool> IsVisibleProperty =
            AvaloniaProperty.Register<UniversalAlert, bool>(nameof(IsVisible), true);

        public static readonly StyledProperty<ICommand> DismissCommandProperty =
            AvaloniaProperty.Register<UniversalAlert, ICommand>(nameof(DismissCommand));

        public static readonly StyledProperty<bool> ShowIconProperty =
            AvaloniaProperty.Register<UniversalAlert, bool>(nameof(ShowIcon), true);

        private Border _container;
        private StackPanel _contentPanel;
        private TextBlock _titleBlock;
        private TextBlock _messageBlock;
        private Button _dismissButton;
        private TextBlock _iconBlock;

        /// <summary>
        /// Main message text of the alert
        /// </summary>
        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>
        /// Title text of the alert
        /// </summary>
        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Type/severity of the alert
        /// </summary>
        public AlertType Type
        {
            get => GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        /// <summary>
        /// Whether the alert can be dismissed by the user
        /// </summary>
        public bool IsDismissible
        {
            get => GetValue(IsDismissibleProperty);
            set => SetValue(IsDismissibleProperty, value);
        }

        /// <summary>
        /// Whether the alert is currently visible
        /// </summary>
        public new bool IsVisible
        {
            get => GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        /// <summary>
        /// Command executed when alert is dismissed
        /// </summary>
        public ICommand DismissCommand
        {
            get => GetValue(DismissCommandProperty);
            set => SetValue(DismissCommandProperty, value);
        }

        /// <summary>
        /// Whether to show the type icon
        /// </summary>
        public bool ShowIcon
        {
            get => GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public UniversalAlert()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _container = new Border
            {
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(16, 12),
                BorderThickness = new Thickness(1)
            };

            var mainPanel = new Grid();
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Icon
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));  // Content
            mainPanel.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto)); // Dismiss button

            // Icon
            _iconBlock = new TextBlock
            {
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 12, 0)
            };
            Grid.SetColumn(_iconBlock, 0);
            mainPanel.Children.Add(_iconBlock);

            // Content
            _contentPanel = new StackPanel
            {
                Spacing = 4
            };

            _titleBlock = new TextBlock
            {
                FontWeight = FontWeight.SemiBold,
                IsVisible = false
            };
            _contentPanel.Children.Add(_titleBlock);

            _messageBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap
            };
            _contentPanel.Children.Add(_messageBlock);

            Grid.SetColumn(_contentPanel, 1);
            mainPanel.Children.Add(_contentPanel);

            // Dismiss button
            _dismissButton = new Button
            {
                Content = "×",
                FontSize = 18,
                Width = 24,
                Height = 24,
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(12, 0, 0, 0)
            };
            _dismissButton.Click += OnDismissClick;
            Grid.SetColumn(_dismissButton, 2);
            mainPanel.Children.Add(_dismissButton);

            _container.Child = mainPanel;
            Content = _container;

            UpdateAlertDisplay();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == MessageProperty ||
                change.Property == TitleProperty ||
                change.Property == TypeProperty ||
                change.Property == IsDismissibleProperty ||
                change.Property == IsVisibleProperty ||
                change.Property == ShowIconProperty)
            {
                UpdateAlertDisplay();
            }
        }

        private void UpdateAlertDisplay()
        {
            if (_container == null) return;

            // Visibility
            base.IsVisible = IsVisible;

            // Title
            if (_titleBlock != null)
            {
                _titleBlock.Text = Title;
                _titleBlock.IsVisible = !string.IsNullOrEmpty(Title);
            }

            // Message
            if (_messageBlock != null)
            {
                _messageBlock.Text = Message;
            }

            // Dismiss button
            if (_dismissButton != null)
            {
                _dismissButton.IsVisible = IsDismissible;
            }

            // Icon and styling based on type
            if (_iconBlock != null)
            {
                _iconBlock.IsVisible = ShowIcon;
                
                var (icon, backgroundBrush, borderBrush) = Type switch
                {
                    AlertType.Success => ("✓", Brushes.LightGreen, Brushes.Green),
                    AlertType.Warning => ("⚠", Brushes.LightYellow, Brushes.Orange),
                    AlertType.Error => ("⚠", Brushes.LightCoral, Brushes.Red),
                    AlertType.Info => ("ℹ", Brushes.LightBlue, Brushes.DodgerBlue),
                    _ => ("ℹ", Brushes.LightGray, Brushes.Gray)
                };

                _iconBlock.Text = icon;
                _container.Background = backgroundBrush;
                _container.BorderBrush = borderBrush;
            }

            // Apply styling classes
            Classes.Set("success", Type == AlertType.Success);
            Classes.Set("warning", Type == AlertType.Warning);
            Classes.Set("error", Type == AlertType.Error);
            Classes.Set("info", Type == AlertType.Info);
            Classes.Set("dismissible", IsDismissible);
        }

        private void OnDismissClick(object sender, RoutedEventArgs e)
        {
            DismissCommand?.Execute(null);
            IsVisible = false;
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Dismiss()
        {
            IsVisible = false;
        }
    }

    /// <summary>
    /// Alert type/severity options
    /// </summary>
    public enum AlertType
    {
        Info,
        Success,
        Warning,
        Error
    }
}