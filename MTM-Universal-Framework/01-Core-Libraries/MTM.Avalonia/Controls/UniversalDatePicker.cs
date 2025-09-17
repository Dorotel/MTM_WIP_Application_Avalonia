using System;
using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace MTM.Universal.Controls
{
    /// <summary>
    /// Universal date picker control with flexible date selection and validation.
    /// </summary>
    public class UniversalDatePicker : UserControl
    {
        public static readonly StyledProperty<DateTime?> SelectedDateProperty =
            AvaloniaProperty.Register<UniversalDatePicker, DateTime?>(
                nameof(SelectedDate), defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<DateTime?> MinDateProperty =
            AvaloniaProperty.Register<UniversalDatePicker, DateTime?>(
                nameof(MinDate));

        public static readonly StyledProperty<DateTime?> MaxDateProperty =
            AvaloniaProperty.Register<UniversalDatePicker, DateTime?>(
                nameof(MaxDate));

        public static readonly StyledProperty<string> PlaceholderTextProperty =
            AvaloniaProperty.Register<UniversalDatePicker, string>(
                nameof(PlaceholderText), "Select date");

        public static readonly StyledProperty<string> DateFormatProperty =
            AvaloniaProperty.Register<UniversalDatePicker, string>(
                nameof(DateFormat), "yyyy-MM-dd");

        public static readonly StyledProperty<bool> ShowTodayButtonProperty =
            AvaloniaProperty.Register<UniversalDatePicker, bool>(
                nameof(ShowTodayButton), true);

        public static readonly StyledProperty<bool> ShowClearButtonProperty =
            AvaloniaProperty.Register<UniversalDatePicker, bool>(
                nameof(ShowClearButton), true);

        public static readonly StyledProperty<ICommand?> DateSelectedCommandProperty =
            AvaloniaProperty.Register<UniversalDatePicker, ICommand?>(
                nameof(DateSelectedCommand));

        public DateTime? SelectedDate
        {
            get => GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public DateTime? MinDate
        {
            get => GetValue(MinDateProperty);
            set => SetValue(MinDateProperty, value);
        }

        public DateTime? MaxDate
        {
            get => GetValue(MaxDateProperty);
            set => SetValue(MaxDateProperty, value);
        }

        public string PlaceholderText
        {
            get => GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public string DateFormat
        {
            get => GetValue(DateFormatProperty);
            set => SetValue(DateFormatProperty, value);
        }

        public bool ShowTodayButton
        {
            get => GetValue(ShowTodayButtonProperty);
            set => SetValue(ShowTodayButtonProperty, value);
        }

        public bool ShowClearButton
        {
            get => GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, value);
        }

        public ICommand? DateSelectedCommand
        {
            get => GetValue(DateSelectedCommandProperty);
            set => SetValue(DateSelectedCommandProperty, value);
        }

        public event EventHandler<DateSelectedEventArgs>? DateSelected;

        private DatePicker _datePicker = null!;
        private TextBlock _placeholder = null!;
        private Button _todayButton = null!;
        private Button _clearButton = null!;

        public UniversalDatePicker()
        {
            InitializeComponent();
            SelectedDateProperty.Changed.Subscribe(OnSelectedDateChanged);
        }

        private void InitializeComponent()
        {
            var mainGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto,Auto")
            };

            // Date picker
            _datePicker = new DatePicker
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            var selectedDateBinding = new Binding(nameof(SelectedDate)) { Source = this };
            _datePicker.Bind(DatePicker.SelectedDateProperty, selectedDateBinding);
            
            _datePicker.SelectedDateChanged += OnDatePickerSelectedDateChanged;
            Grid.SetColumn(_datePicker, 0);

            // Placeholder (shown when no date selected)
            _placeholder = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0),
                Foreground = new SolidColorBrush(Colors.Gray),
                IsHitTestVisible = false
            };
            
            var placeholderBinding = new Binding(nameof(PlaceholderText)) { Source = this };
            _placeholder.Bind(TextBlock.TextProperty, placeholderBinding);
            Grid.SetColumn(_placeholder, 0);

            // Today button
            _todayButton = new Button
            {
                Content = "Today",
                Padding = new Thickness(8, 4),
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12
            };
            _todayButton.Click += OnTodayButtonClick;
            
            var showTodayBinding = new Binding(nameof(ShowTodayButton)) { Source = this };
            _todayButton.Bind(Button.IsVisibleProperty, showTodayBinding);
            Grid.SetColumn(_todayButton, 1);

            // Clear button
            _clearButton = new Button
            {
                Content = "✕",
                Width = 24,
                Height = 24,
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 10,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };
            _clearButton.Click += OnClearButtonClick;
            
            var showClearBinding = new Binding(nameof(ShowClearButton)) { Source = this };
            _clearButton.Bind(Button.IsVisibleProperty, showClearBinding);
            Grid.SetColumn(_clearButton, 2);

            // Calendar icon
            var calendarIcon = new Button
            {
                Content = "📅",
                Width = 24,
                Height = 24,
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };
            calendarIcon.Click += OnCalendarIconClick;
            Grid.SetColumn(calendarIcon, 3);

            mainGrid.Children.Add(_datePicker);
            mainGrid.Children.Add(_placeholder);
            mainGrid.Children.Add(_todayButton);
            mainGrid.Children.Add(_clearButton);
            mainGrid.Children.Add(calendarIcon);

            // Wrap in border for styling
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Background = new SolidColorBrush(Colors.White),
                Child = mainGrid
            };

            Content = border;
            UpdatePlaceholderVisibility();
        }

        private void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void OnDatePickerSelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
        {
            var newDate = e.NewDate?.Date;
            
            // Validate against min/max dates
            if (MinDate.HasValue && newDate < MinDate.Value.Date)
            {
                SelectedDate = MinDate.Value.Date;
                return;
            }
            
            if (MaxDate.HasValue && newDate > MaxDate.Value.Date)
            {
                SelectedDate = MaxDate.Value.Date;
                return;
            }

            SelectedDate = newDate;
            
            // Raise events
            DateSelected?.Invoke(this, new DateSelectedEventArgs(newDate));
            DateSelectedCommand?.Execute(newDate);
        }

        private void OnTodayButtonClick(object? sender, RoutedEventArgs e)
        {
            var today = DateTime.Today;
            
            // Validate against min/max dates
            if (MinDate.HasValue && today < MinDate.Value.Date)
                return;
            if (MaxDate.HasValue && today > MaxDate.Value.Date)
                return;

            SelectedDate = today;
        }

        private void OnClearButtonClick(object? sender, RoutedEventArgs e)
        {
            SelectedDate = null;
        }

        private void OnCalendarIconClick(object? sender, RoutedEventArgs e)
        {
            // Focus the date picker to open calendar
            _datePicker.Focus();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (_placeholder != null)
            {
                _placeholder.IsVisible = !SelectedDate.HasValue;
            }
        }

        /// <summary>
        /// Get formatted date string.
        /// </summary>
        public string GetFormattedDate()
        {
            return SelectedDate?.ToString(DateFormat, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        /// <summary>
        /// Set date from string using current format.
        /// </summary>
        public bool TrySetDateFromString(string dateString)
        {
            if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                SelectedDate = date;
                return true;
            }
            return false;
        }
    }

    public class DateSelectedEventArgs : EventArgs
    {
        public DateTime? SelectedDate { get; }

        public DateSelectedEventArgs(DateTime? selectedDate)
        {
            SelectedDate = selectedDate;
        }
    }
}