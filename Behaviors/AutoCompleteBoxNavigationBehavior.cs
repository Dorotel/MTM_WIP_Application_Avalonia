using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace MTM_WIP_Application_Avalonia.Behaviors;

/// <summary>
/// Custom behavior that enables WinForms-style navigation in AutoCompleteBox controls.
/// </summary>
public class AutoCompleteBoxNavigationBehavior : Behavior<AutoCompleteBox>
{
    #region Private Fields

    private Popup? _popup;
    private ListBox? _listBox;
    private bool _navigationMode = false;
    private string? _originalText;
    private bool _suppressTextUpdate;
    private string? _pendingTextRestore;

    #endregion

    #region Behavior Lifecycle

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            AssociatedObject.Loaded += OnAutoCompleteBoxLoaded;
            AssociatedObject.GotFocus += OnAutoCompleteBoxGotFocus;
            AssociatedObject.LostFocus += OnAutoCompleteBoxLostFocus;
            // Use AddHandler to capture events at the tunnel phase, giving us priority.
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, OnAutoCompleteBoxKeyDown, RoutingStrategies.Tunnel);
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.Loaded -= OnAutoCompleteBoxLoaded;
            AssociatedObject.GotFocus -= OnAutoCompleteBoxGotFocus;
            AssociatedObject.LostFocus -= OnAutoCompleteBoxLostFocus;
            // Correctly remove the handler.
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, OnAutoCompleteBoxKeyDown);
        }

        if (_popup != null)
        {
            _popup.Opened -= OnPopupOpened;
            _popup.Closed -= OnPopupClosed;
        }

        if (_listBox != null)
        {
            _listBox.SelectionChanged -= OnListBoxSelectionChanged;
        }

        base.OnDetaching();
    }

    #endregion

    #region Event Handlers

    private void OnAutoCompleteBoxLoaded(object? sender, RoutedEventArgs e)
    {
        WirePopupAndListBox();
    }

    private void OnAutoCompleteBoxGotFocus(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject != null)
        {
            _originalText = AssociatedObject.Text;
        }
    }

    private void OnAutoCompleteBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        _navigationMode = false;
        _suppressTextUpdate = false;
        _pendingTextRestore = null;
        _originalText = null;
    }

    private void OnPopupOpened(object? sender, EventArgs e)
    {
        if (_popup != null)
        {
            _listBox = _popup.GetVisualDescendants().OfType<ListBox>().FirstOrDefault();
            if (_listBox != null)
            {
                _listBox.SelectionChanged -= OnListBoxSelectionChanged;
                _listBox.SelectionChanged += OnListBoxSelectionChanged;
            }
        }
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        _navigationMode = false;
        _suppressTextUpdate = false;
        _pendingTextRestore = null;

        if (_listBox != null)
        {
            _listBox.SelectionChanged -= OnListBoxSelectionChanged;
            _listBox = null;
        }
    }

    private void OnAutoCompleteBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject == null) return;

        var popupOpen = _popup?.IsOpen == true;

        if (!popupOpen)
        {
            // If popup is closed, allow Up/Down to open it.
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                AssociatedObject.IsDropDownOpen = true;
                e.Handled = true;
            }
            return;
        }

        // If popup is open, handle navigation keys.
        switch (e.Key)
        {
            case Key.Down:
            case Key.Up:
                e.Handled = true;
                _navigationMode = true;
                if (_pendingTextRestore == null)
                {
                    _pendingTextRestore = AssociatedObject.Text;
                }
                _suppressTextUpdate = true;
                MoveSelection(e.Key == Key.Down ? NavigationDirection.Down : NavigationDirection.Up);
                break;

            case Key.Enter:
                e.Handled = true;
                ConfirmSelection();
                break;

            case Key.Escape:
                e.Handled = true;
                CancelNavigation();
                break;

            default:
                // For any other key (like character input), reset navigation state.
                _navigationMode = false;
                _suppressTextUpdate = false;
                _pendingTextRestore = null;
                // Do not handle the event, let the AutoCompleteBox process it.
                break;
        }
    }

    private void OnListBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressTextUpdate && AssociatedObject != null)
        {
            // When we are navigating with arrow keys, prevent the AutoCompleteBox
            // from updating its text property with the new selection.
            var restoreText = _pendingTextRestore ?? string.Empty;
            AssociatedObject.Text = restoreText;
            // Do not consume the flag here, it will be reset on the next key press
            // that is not an up/down arrow.
        }
    }

    #endregion

    #region Navigation Methods

    private void MoveSelection(NavigationDirection direction)
    {
        if (_listBox == null || _listBox.Items.Count == 0) return;

        var currentIndex = _listBox.SelectedIndex;
        if (currentIndex < 0)
        {
            currentIndex = direction == NavigationDirection.Down ? -1 : 0;
        }

        var newIndex = direction switch
        {
            NavigationDirection.Down => Math.Min(currentIndex + 1, _listBox.Items.Count - 1),
            NavigationDirection.Up => Math.Max(currentIndex - 1, 0),
            _ => currentIndex
        };

        if (newIndex != currentIndex)
        {
            _listBox.SelectedIndex = newIndex;
            _listBox.ScrollIntoView(_listBox.SelectedItem!);
        }
    }
    
    private void ConfirmSelection()
    {
        if (_listBox?.SelectedItem != null && AssociatedObject != null)
        {
            _navigationMode = false;
            _suppressTextUpdate = false;
            AssociatedObject.SelectedItem = _listBox.SelectedItem;
            AssociatedObject.Text = _listBox.SelectedItem.ToString() ?? string.Empty;
            _popup?.Close();
            _originalText = AssociatedObject.Text;
        }
    }

    private void CancelNavigation()
    {
        if (AssociatedObject != null)
        {
            if (_navigationMode && _originalText != null)
            {
                AssociatedObject.Text = _originalText;
            }
            _popup?.Close();
        }
        _navigationMode = false;
    }

    #endregion

    #region Utilities

    private void WirePopupAndListBox()
    {
        if (AssociatedObject == null) return;

        _popup = AssociatedObject.GetVisualDescendants().OfType<Popup>().FirstOrDefault();
        if (_popup != null)
        {
            _popup.Opened -= OnPopupOpened;
            _popup.Closed -= OnPopupClosed;
            _popup.Opened += OnPopupOpened;
            _popup.Closed += OnPopupClosed;
        }
    }

    #endregion

    #region Helper Types

    private enum NavigationDirection
    {
        Up,
        Down
    }

    #endregion
}
