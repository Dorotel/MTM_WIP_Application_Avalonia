using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Tree View control for hierarchical data display.
    /// Provides consistent tree navigation with customizable node templates.
    /// </summary>
    public class UniversalTreeView : TreeView
    {
        public static readonly StyledProperty<bool> ShowLinesProperty =
            AvaloniaProperty.Register<UniversalTreeView, bool>(nameof(ShowLines), true);

        public static readonly StyledProperty<bool> ShowRootLinesProperty =
            AvaloniaProperty.Register<UniversalTreeView, bool>(nameof(ShowRootLines), true);

        public static readonly StyledProperty<ICommand?> NodeExpandedCommandProperty =
            AvaloniaProperty.Register<UniversalTreeView, ICommand?>(nameof(NodeExpandedCommand));

        public static readonly StyledProperty<ICommand?> NodeSelectedCommandProperty =
            AvaloniaProperty.Register<UniversalTreeView, ICommand?>(nameof(NodeSelectedCommand));

        public static readonly StyledProperty<bool> AllowMultiSelectProperty =
            AvaloniaProperty.Register<UniversalTreeView, bool>(nameof(AllowMultiSelect), false);

        public static readonly StyledProperty<TreeViewSelectionMode> SelectionModeProperty =
            AvaloniaProperty.Register<UniversalTreeView, TreeViewSelectionMode>(nameof(SelectionMode), TreeViewSelectionMode.Single);

        /// <summary>
        /// Gets or sets whether to show connecting lines between nodes.
        /// </summary>
        public bool ShowLines
        {
            get => GetValue(ShowLinesProperty);
            set => SetValue(ShowLinesProperty, value);
        }

        /// <summary>
        /// Gets or sets whether to show lines at the root level.
        /// </summary>
        public bool ShowRootLines
        {
            get => GetValue(ShowRootLinesProperty);
            set => SetValue(ShowRootLinesProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when a node is expanded.
        /// </summary>
        public ICommand? NodeExpandedCommand
        {
            get => GetValue(NodeExpandedCommandProperty);
            set => SetValue(NodeExpandedCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when a node is selected.
        /// </summary>
        public ICommand? NodeSelectedCommand
        {
            get => GetValue(NodeSelectedCommandProperty);
            set => SetValue(NodeSelectedCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets whether multiple nodes can be selected.
        /// </summary>
        public bool AllowMultiSelect
        {
            get => GetValue(AllowMultiSelectProperty);
            set => SetValue(AllowMultiSelectProperty, value);
        }

        /// <summary>
        /// Gets or sets the tree view selection mode.
        /// </summary>
        public TreeViewSelectionMode SelectionMode
        {
            get => GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }
    }

    /// <summary>
    /// Universal Tree View Item.
    /// </summary>
    public class UniversalTreeViewItem : TreeViewItem
    {
        public static readonly StyledProperty<object?> IconProperty =
            AvaloniaProperty.Register<UniversalTreeViewItem, object?>(nameof(Icon));

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<UniversalTreeViewItem, string>(nameof(Text), string.Empty);

        public static readonly StyledProperty<object?> ValueProperty =
            AvaloniaProperty.Register<UniversalTreeViewItem, object?>(nameof(Value));

        /// <summary>
        /// Gets or sets the node icon.
        /// </summary>
        public object? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// Gets or sets the node text.
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        public object? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }

    /// <summary>
    /// Tree node data model.
    /// </summary>
    public class TreeNode
    {
        public string Text { get; set; } = string.Empty;
        public object? Value { get; set; }
        public object? Icon { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }
        public ObservableCollection<TreeNode> Children { get; set; } = new();
    }

    /// <summary>
    /// Tree view selection modes.
    /// </summary>
    public enum TreeViewSelectionMode
    {
        Single,
        Multiple,
        Extended
    }
}