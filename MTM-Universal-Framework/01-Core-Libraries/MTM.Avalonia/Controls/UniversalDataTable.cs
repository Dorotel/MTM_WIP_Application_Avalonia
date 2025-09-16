using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Data Table with responsive design and cross-platform touch optimization.
    /// Provides consistent data display across Windows, macOS, Linux, and Android.
    /// </summary>
    public class UniversalDataTable : UserControl
    {
        public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.Register<UniversalDataTable, IEnumerable>(nameof(ItemsSource));

        public static readonly StyledProperty<object> SelectedItemProperty =
            AvaloniaProperty.Register<UniversalDataTable, object>(nameof(SelectedItem), 
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<bool> AllowSelectionProperty =
            AvaloniaProperty.Register<UniversalDataTable, bool>(nameof(AllowSelection), true);

        public static readonly StyledProperty<bool> AllowMultipleSelectionProperty =
            AvaloniaProperty.Register<UniversalDataTable, bool>(nameof(AllowMultipleSelection), false);

        public static readonly StyledProperty<bool> ShowHeaderProperty =
            AvaloniaProperty.Register<UniversalDataTable, bool>(nameof(ShowHeader), true);

        public static readonly StyledProperty<bool> IsStripedProperty =
            AvaloniaProperty.Register<UniversalDataTable, bool>(nameof(IsStriped), true);

        public static readonly StyledProperty<TableSize> SizeProperty =
            AvaloniaProperty.Register<UniversalDataTable, TableSize>(nameof(Size), TableSize.Medium);

        private DataGrid _dataGrid;

        /// <summary>
        /// Items to display in the table
        /// </summary>
        public IEnumerable ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Currently selected item
        /// </summary>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// Whether row selection is allowed
        /// </summary>
        public bool AllowSelection
        {
            get => GetValue(AllowSelectionProperty);
            set => SetValue(AllowSelectionProperty, value);
        }

        /// <summary>
        /// Whether multiple rows can be selected
        /// </summary>
        public bool AllowMultipleSelection
        {
            get => GetValue(AllowMultipleSelectionProperty);
            set => SetValue(AllowMultipleSelectionProperty, value);
        }

        /// <summary>
        /// Whether to show the table header
        /// </summary>
        public bool ShowHeader
        {
            get => GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        /// <summary>
        /// Whether to use striped rows
        /// </summary>
        public bool IsStriped
        {
            get => GetValue(IsStripedProperty);
            set => SetValue(IsStripedProperty, value);
        }

        /// <summary>
        /// Size/density of the table
        /// </summary>
        public TableSize Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public UniversalDataTable()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _dataGrid = new DataGrid
            {
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                IsReadOnly = true,
                CanUserReorderColumns = false,
                CanUserResizeColumns = true,
                SelectionMode = DataGridSelectionMode.Single,
                AutoGenerateColumns = true
            };

            Content = _dataGrid;
            UpdateDataGridProperties();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ItemsSourceProperty)
            {
                if (_dataGrid != null)
                    _dataGrid.ItemsSource = ItemsSource;
            }
            else if (change.Property == SelectedItemProperty)
            {
                if (_dataGrid != null)
                    _dataGrid.SelectedItem = SelectedItem;
            }
            else if (change.Property == AllowSelectionProperty ||
                     change.Property == AllowMultipleSelectionProperty ||
                     change.Property == ShowHeaderProperty ||
                     change.Property == IsStripedProperty ||
                     change.Property == SizeProperty)
            {
                UpdateDataGridProperties();
            }
        }

        private void UpdateDataGridProperties()
        {
            if (_dataGrid == null) return;

            // Selection
            if (!AllowSelection)
            {
                _dataGrid.SelectionMode = DataGridSelectionMode.Single;
                _dataGrid.CanUserSortColumns = false;
            }
            else
            {
                _dataGrid.SelectionMode = AllowMultipleSelection 
                    ? DataGridSelectionMode.Extended 
                    : DataGridSelectionMode.Single;
            }

            // Header visibility
            _dataGrid.HeadersVisibility = ShowHeader 
                ? DataGridHeadersVisibility.Column 
                : DataGridHeadersVisibility.None;

            // Styling classes
            Classes.Set("striped", IsStriped);
            Classes.Set("compact", Size == TableSize.Small);
            Classes.Set("comfortable", Size == TableSize.Large);
            Classes.Set("no-selection", !AllowSelection);

            // Row height based on size
            switch (Size)
            {
                case TableSize.Small:
                    _dataGrid.RowHeight = 32;
                    break;
                case TableSize.Medium:
                    _dataGrid.RowHeight = 40;
                    break;
                case TableSize.Large:
                    _dataGrid.RowHeight = 48;
                    break;
            }
        }
    }

    /// <summary>
    /// Table size options for different densities
    /// </summary>
    public enum TableSize
    {
        Small,   // Compact for mobile
        Medium,  // Standard
        Large    // Comfortable for desktop
    }
}