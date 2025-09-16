using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Radio Group control for managing mutually exclusive selections.
    /// Provides consistent radio button grouping with customizable layout.
    /// </summary>
    public class UniversalRadioGroup : ItemsControl
    {
        public static readonly StyledProperty<object?> SelectedValueProperty =
            AvaloniaProperty.Register<UniversalRadioGroup, object?>(nameof(SelectedValue), 
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<int> SelectedIndexProperty =
            AvaloniaProperty.Register<UniversalRadioGroup, int>(nameof(SelectedIndex), -1, 
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<RadioGroupOrientation> OrientationProperty =
            AvaloniaProperty.Register<UniversalRadioGroup, RadioGroupOrientation>(nameof(Orientation), RadioGroupOrientation.Vertical);

        public static readonly StyledProperty<string> GroupNameProperty =
            AvaloniaProperty.Register<UniversalRadioGroup, string>(nameof(GroupName), string.Empty);

        public static readonly StyledProperty<double> ItemSpacingProperty =
            AvaloniaProperty.Register<UniversalRadioGroup, double>(nameof(ItemSpacing), 8.0);

        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        public object? SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        public int SelectedIndex
        {
            get => GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        /// <summary>
        /// Gets or sets the radio group orientation.
        /// </summary>
        public RadioGroupOrientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the radio button group name.
        /// </summary>
        public string GroupName
        {
            get => GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the spacing between radio items.
        /// </summary>
        public double ItemSpacing
        {
            get => GetValue(ItemSpacingProperty);
            set => SetValue(ItemSpacingProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedIndexProperty)
            {
                UpdateSelectedValue();
            }
            else if (change.Property == SelectedValueProperty)
            {
                UpdateSelectedIndex();
            }
        }

        private void UpdateSelectedValue()
        {
            var index = SelectedIndex;
            if (index >= 0 && index < Items.Count)
            {
                SelectedValue = Items[index];
            }
        }

        private void UpdateSelectedIndex()
        {
            var value = SelectedValue;
            if (value != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Equals(Items[i], value))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Radio button item data model.
    /// </summary>
    public class RadioItem
    {
        public string Text { get; set; } = string.Empty;
        public object? Value { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string? Description { get; set; }
    }

    /// <summary>
    /// Radio group orientation options.
    /// </summary>
    public enum RadioGroupOrientation
    {
        Vertical,
        Horizontal,
        Grid
    }
}