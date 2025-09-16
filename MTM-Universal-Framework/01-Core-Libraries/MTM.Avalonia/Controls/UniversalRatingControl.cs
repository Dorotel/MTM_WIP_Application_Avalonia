using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace MTM.Universal.Controls
{
    /// <summary>
    /// Universal rating control with star-based or numeric rating system.
    /// </summary>
    public class UniversalRatingControl : UserControl
    {
        public static readonly StyledProperty<int> RatingProperty =
            AvaloniaProperty.Register<UniversalRatingControl, int>(
                nameof(Rating), 0, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<int> MaxRatingProperty =
            AvaloniaProperty.Register<UniversalRatingControl, int>(
                nameof(MaxRating), 5);

        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<UniversalRatingControl, bool>(
                nameof(IsReadOnly), false);

        public static readonly StyledProperty<string> RatingIconProperty =
            AvaloniaProperty.Register<UniversalRatingControl, string>(
                nameof(RatingIcon), "★");

        public static readonly StyledProperty<string> EmptyIconProperty =
            AvaloniaProperty.Register<UniversalRatingControl, string>(
                nameof(EmptyIcon), "☆");

        public static readonly StyledProperty<IBrush> FilledColorProperty =
            AvaloniaProperty.Register<UniversalRatingControl, IBrush>(
                nameof(FilledColor), new SolidColorBrush(Colors.Gold));

        public static readonly StyledProperty<IBrush> EmptyColorProperty =
            AvaloniaProperty.Register<UniversalRatingControl, IBrush>(
                nameof(EmptyColor), new SolidColorBrush(Colors.LightGray));

        public static readonly StyledProperty<double> IconSizeProperty =
            AvaloniaProperty.Register<UniversalRatingControl, double>(
                nameof(IconSize), 24.0);

        public static readonly StyledProperty<bool> ShowRatingTextProperty =
            AvaloniaProperty.Register<UniversalRatingControl, bool>(
                nameof(ShowRatingText), false);

        public static readonly StyledProperty<ICommand?> RatingChangedCommandProperty =
            AvaloniaProperty.Register<UniversalRatingControl, ICommand?>(
                nameof(RatingChangedCommand));

        public int Rating
        {
            get => GetValue(RatingProperty);
            set => SetValue(RatingProperty, value);
        }

        public int MaxRating
        {
            get => GetValue(MaxRatingProperty);
            set => SetValue(MaxRatingProperty, value);
        }

        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public string RatingIcon
        {
            get => GetValue(RatingIconProperty);
            set => SetValue(RatingIconProperty, value);
        }

        public string EmptyIcon
        {
            get => GetValue(EmptyIconProperty);
            set => SetValue(EmptyIconProperty, value);
        }

        public IBrush FilledColor
        {
            get => GetValue(FilledColorProperty);
            set => SetValue(FilledColorProperty, value);
        }

        public IBrush EmptyColor
        {
            get => GetValue(EmptyColorProperty);
            set => SetValue(EmptyColorProperty, value);
        }

        public double IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public bool ShowRatingText
        {
            get => GetValue(ShowRatingTextProperty);
            set => SetValue(ShowRatingTextProperty, value);
        }

        public ICommand? RatingChangedCommand
        {
            get => GetValue(RatingChangedCommandProperty);
            set => SetValue(RatingChangedCommandProperty, value);
        }

        public event EventHandler<RatingChangedEventArgs>? RatingChanged;

        private StackPanel _iconsPanel = null!;
        private TextBlock _ratingText = null!;
        private readonly System.Collections.Generic.List<TextBlock> _iconButtons = new();

        public UniversalRatingControl()
        {
            InitializeComponent();
            RatingProperty.Changed.Subscribe(OnRatingChanged);
            MaxRatingProperty.Changed.Subscribe(OnMaxRatingChanged);
        }

        private void InitializeComponent()
        {
            var mainStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 5
            };

            // Icons panel
            _iconsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 2
            };
            mainStackPanel.Children.Add(_iconsPanel);

            // Rating text
            _ratingText = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0),
                FontSize = 12
            };
            
            var showTextBinding = new Binding(nameof(ShowRatingText)) { Source = this };
            _ratingText.Bind(TextBlock.IsVisibleProperty, showTextBinding);
            mainStackPanel.Children.Add(_ratingText);

            Content = mainStackPanel;
            CreateRatingIcons();
        }

        private void CreateRatingIcons()
        {
            _iconsPanel.Children.Clear();
            _iconButtons.Clear();

            for (int i = 1; i <= MaxRating; i++)
            {
                var iconButton = new TextBlock
                {
                    FontSize = IconSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Cursor = IsReadOnly ? Cursor.Default : new Cursor(StandardCursorType.Hand),
                    Tag = i
                };

                if (!IsReadOnly)
                {
                    iconButton.PointerPressed += OnIconPressed;
                    iconButton.PointerEntered += OnIconEntered;
                    iconButton.PointerExited += OnIconExited;
                }

                _iconButtons.Add(iconButton);
                _iconsPanel.Children.Add(iconButton);
            }

            UpdateIconsDisplay();
        }

        private void OnRatingChanged(AvaloniaPropertyChangedEventArgs e)
        {
            UpdateIconsDisplay();
            UpdateRatingText();
            
            var newRating = (int)e.NewValue!;
            RatingChanged?.Invoke(this, new RatingChangedEventArgs(newRating));
            RatingChangedCommand?.Execute(newRating);
        }

        private void OnMaxRatingChanged(AvaloniaPropertyChangedEventArgs e)
        {
            CreateRatingIcons();
        }

        private void UpdateIconsDisplay()
        {
            for (int i = 0; i < _iconButtons.Count; i++)
            {
                var icon = _iconButtons[i];
                var position = i + 1;
                
                if (position <= Rating)
                {
                    icon.Text = RatingIcon;
                    icon.Foreground = FilledColor;
                }
                else
                {
                    icon.Text = EmptyIcon;
                    icon.Foreground = EmptyColor;
                }
            }
        }

        private void UpdateRatingText()
        {
            if (_ratingText != null)
            {
                _ratingText.Text = $"{Rating}/{MaxRating}";
            }
        }

        private void OnIconPressed(object? sender, PointerPressedEventArgs e)
        {
            if (IsReadOnly || sender is not TextBlock icon) return;
            
            var newRating = (int)icon.Tag!;
            
            // Allow toggling: if clicking the current rating, set to 0
            Rating = (Rating == newRating) ? 0 : newRating;
        }

        private void OnIconEntered(object? sender, PointerEventArgs e)
        {
            if (IsReadOnly || sender is not TextBlock icon) return;
            
            var hoverRating = (int)icon.Tag!;
            
            // Preview the rating on hover
            for (int i = 0; i < _iconButtons.Count; i++)
            {
                var iconButton = _iconButtons[i];
                var position = i + 1;
                
                if (position <= hoverRating)
                {
                    iconButton.Text = RatingIcon;
                    iconButton.Foreground = FilledColor;
                    iconButton.Opacity = 0.7; // Slight transparency for preview
                }
                else
                {
                    iconButton.Text = EmptyIcon;
                    iconButton.Foreground = EmptyColor;
                    iconButton.Opacity = 1.0;
                }
            }
        }

        private void OnIconExited(object? sender, PointerEventArgs e)
        {
            if (IsReadOnly) return;
            
            // Restore actual rating display
            UpdateIconsDisplay();
            
            // Reset opacity
            foreach (var icon in _iconButtons)
            {
                icon.Opacity = 1.0;
            }
        }

        /// <summary>
        /// Set rating programmatically with validation.
        /// </summary>
        public void SetRating(int rating)
        {
            if (rating < 0) rating = 0;
            if (rating > MaxRating) rating = MaxRating;
            
            Rating = rating;
        }

        /// <summary>
        /// Get rating as percentage (0-100).
        /// </summary>
        public double GetRatingPercentage()
        {
            if (MaxRating == 0) return 0;
            return (double)Rating / MaxRating * 100;
        }

        /// <summary>
        /// Set rating from percentage (0-100).
        /// </summary>
        public void SetRatingFromPercentage(double percentage)
        {
            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;
            
            var rating = (int)Math.Round(percentage / 100 * MaxRating);
            SetRating(rating);
        }
    }

    public class RatingChangedEventArgs : EventArgs
    {
        public int NewRating { get; }

        public RatingChangedEventArgs(int newRating)
        {
            NewRating = newRating;
        }
    }
}