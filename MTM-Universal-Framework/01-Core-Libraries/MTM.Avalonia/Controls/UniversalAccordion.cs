using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Accordion control for expandable/collapsible content sections.
    /// Provides consistent accordion appearance with multiple expansion modes.
    /// </summary>
    public class UniversalAccordion : ItemsControl
    {
        public static readonly StyledProperty<AccordionExpandMode> ExpandModeProperty =
            AvaloniaProperty.Register<UniversalAccordion, AccordionExpandMode>(nameof(ExpandMode), AccordionExpandMode.Single);

        public static readonly StyledProperty<bool> AllowMultipleExpandedProperty =
            AvaloniaProperty.Register<UniversalAccordion, bool>(nameof(AllowMultipleExpanded), false);

        public static readonly StyledProperty<ICommand?> ItemExpandedCommandProperty =
            AvaloniaProperty.Register<UniversalAccordion, ICommand?>(nameof(ItemExpandedCommand));

        /// <summary>
        /// Gets or sets the expansion mode for accordion items.
        /// </summary>
        public AccordionExpandMode ExpandMode
        {
            get => GetValue(ExpandModeProperty);
            set => SetValue(ExpandModeProperty, value);
        }

        /// <summary>
        /// Gets or sets whether multiple items can be expanded simultaneously.
        /// </summary>
        public bool AllowMultipleExpanded
        {
            get => GetValue(AllowMultipleExpandedProperty);
            set => SetValue(AllowMultipleExpandedProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when an item is expanded.
        /// </summary>
        public ICommand? ItemExpandedCommand
        {
            get => GetValue(ItemExpandedCommandProperty);
            set => SetValue(ItemExpandedCommandProperty, value);
        }
    }

    /// <summary>
    /// Universal Accordion Item control.
    /// </summary>
    public class UniversalAccordionItem : ContentControl
    {
        public static readonly StyledProperty<string> HeaderProperty =
            AvaloniaProperty.Register<UniversalAccordionItem, string>(nameof(Header), string.Empty);

        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<UniversalAccordionItem, bool>(nameof(IsExpanded), false);

        public static readonly StyledProperty<object?> HeaderContentProperty =
            AvaloniaProperty.Register<UniversalAccordionItem, object?>(nameof(HeaderContent));

        /// <summary>
        /// Gets or sets the accordion item header text.
        /// </summary>
        public string Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the accordion item is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        public object? HeaderContent
        {
            get => GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            IsExpanded = !IsExpanded;
        }
    }

    /// <summary>
    /// Accordion expansion modes.
    /// </summary>
    public enum AccordionExpandMode
    {
        Single,
        Multiple,
        ZeroOrOne
    }
}