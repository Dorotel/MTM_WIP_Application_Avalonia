using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Tab View with customizable tab headers and content areas.
    /// Provides consistent tabbed interface experience with responsive design for mobile.
    /// </summary>
    public class UniversalTabView : UserControl
    {
        public static readonly StyledProperty<IEnumerable> TabItemsProperty =
            AvaloniaProperty.Register<UniversalTabView, IEnumerable>(nameof(TabItems));

        public static readonly StyledProperty<int> SelectedIndexProperty =
            AvaloniaProperty.Register<UniversalTabView, int>(nameof(SelectedIndex), 0);

        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<UniversalTabView, object?>(nameof(SelectedItem));

        public static readonly StyledProperty<bool> CanCloseTabsProperty =
            AvaloniaProperty.Register<UniversalTabView, bool>(nameof(CanCloseTabs), false);

        public static readonly StyledProperty<bool> IsScrollableProperty =
            AvaloniaProperty.Register<UniversalTabView, bool>(nameof(IsScrollable), true);

        public static readonly StyledProperty<TabStripPlacement> TabPlacementProperty =
            AvaloniaProperty.Register<UniversalTabView, TabStripPlacement>(nameof(TabPlacement), TabStripPlacement.Top);

        public IEnumerable TabItems
        {
            get => GetValue(TabItemsProperty);
            set => SetValue(TabItemsProperty, value);
        }

        public int SelectedIndex
        {
            get => GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public bool CanCloseTabs
        {
            get => GetValue(CanCloseTabsProperty);
            set => SetValue(CanCloseTabsProperty, value);
        }

        public bool IsScrollable
        {
            get => GetValue(IsScrollableProperty);
            set => SetValue(IsScrollableProperty, value);
        }

        public TabStripPlacement TabPlacement
        {
            get => GetValue(TabPlacementProperty);
            set => SetValue(TabPlacementProperty, value);
        }

        public UniversalTabView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new Border
            {
                Background = Avalonia.Media.Brushes.White,
                BorderBrush = Avalonia.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Child = new TabControl
                {
                    [!TabControl.ItemsSourceProperty] = this.GetObservable(TabItemsProperty).ToBinding(),
                    [!TabControl.SelectedIndexProperty] = this.GetObservable(SelectedIndexProperty).ToBinding(),
                    [!TabControl.SelectedItemProperty] = this.GetObservable(SelectedItemProperty).ToBinding(),
                    [!TabControl.TabStripPlacementProperty] = this.GetObservable(TabPlacementProperty).ToBinding(),
                    Margin = new Thickness(4)
                }
            };
        }
    }
}