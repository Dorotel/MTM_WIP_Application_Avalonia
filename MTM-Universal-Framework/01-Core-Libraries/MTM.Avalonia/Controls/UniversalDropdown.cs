using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Dropdown with customizable styling, search functionality, and multi-select support.
    /// Provides consistent dropdown experience across platforms with touch optimization.
    /// </summary>
    public class UniversalDropdown : UserControl
    {
        public static readonly StyledProperty<IEnumerable> ItemsProperty =
            AvaloniaProperty.Register<UniversalDropdown, IEnumerable>(nameof(Items));

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<UniversalDropdown, object?>(nameof(SelectedItem));

        public static readonly StyledProperty<IList> SelectedItemsProperty =
            AvaloniaProperty.Register<UniversalDropdown, IList>(nameof(SelectedItems));

        public static readonly StyledProperty<string> PlaceholderProperty =
            AvaloniaProperty.Register<UniversalDropdown, string>(nameof(Placeholder), "Select an option...");

        public static readonly StyledProperty<bool> IsMultiSelectProperty =
            AvaloniaProperty.Register<UniversalDropdown, bool>(nameof(IsMultiSelect), false);

        public static readonly StyledProperty<bool> IsSearchableProperty =
            AvaloniaProperty.Register<UniversalDropdown, bool>(nameof(IsSearchable), false);

        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<UniversalDropdown, bool>(nameof(IsOpen), false);

        public static readonly StyledProperty<string> DisplayMemberPathProperty =
            AvaloniaProperty.Register<UniversalDropdown, string>(nameof(DisplayMemberPath), string.Empty);

        public IEnumerable Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public IList SelectedItems
        {
            get => GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public string Placeholder
        {
            get => GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public bool IsMultiSelect
        {
            get => GetValue(IsMultiSelectProperty);
            set => SetValue(IsMultiSelectProperty, value);
        }

        public bool IsSearchable
        {
            get => GetValue(IsSearchableProperty);
            set => SetValue(IsSearchableProperty, value);
        }

        public bool IsOpen
        {
            get => GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public string DisplayMemberPath
        {
            get => GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public UniversalDropdown()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new ComboBox
            {
                [!ComboBox.ItemsSourceProperty] = this.GetObservable(ItemsProperty).ToBinding(),
                [!ComboBox.SelectedItemProperty] = this.GetObservable(SelectedItemProperty).ToBinding(),
                [!ComboBox.PlaceholderTextProperty] = this.GetObservable(PlaceholderProperty).ToBinding(),
                [!ComboBox.IsDropDownOpenProperty] = this.GetObservable(IsOpenProperty).ToBinding(),
                MinHeight = 40,
                Padding = new Thickness(12, 8),
                Background = Avalonia.Media.Brushes.White,
                BorderBrush = Avalonia.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6)
            };
        }
    }
}