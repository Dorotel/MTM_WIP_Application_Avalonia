using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

/// <summary>
/// Base overlay view providing common UI functionality for MTM overlay dialogs.
/// Follows MTM design system patterns with theme integration and accessibility support.
/// </summary>
public partial class BaseOverlayView : UserControl
{
    public BaseOverlayView()
    {
        InitializeComponent();

        // Set up default behaviors
        SetupDefaultBehavior();
    }

    /// <summary>
    /// Sets up default overlay behavior such as keyboard handling and focus management.
    /// </summary>
    protected virtual void SetupDefaultBehavior()
    {
        // Focus management - set initial focus to first input control when overlay is loaded
        Loaded += OnOverlayLoaded;

        // Keyboard handling - ESC to close if allowed
        KeyDown += OnOverlayKeyDown;
    }

    /// <summary>
    /// Handles the overlay loaded event to set initial focus.
    /// </summary>
    protected virtual void OnOverlayLoaded(object? sender, RoutedEventArgs e)
    {
        // Try to find and focus the first input control
        var contentPresenter = this.FindControl<Control>("OverlayContentPresenter");
        Control? firstInput = null;

        if (contentPresenter != null)
        {
            firstInput = FindDescendantOfType<TextBox>(contentPresenter);
            if (firstInput == null)
            {
                firstInput = FindDescendantOfType<ComboBox>(contentPresenter);
            }
        }

        if (firstInput != null)
        {
            // Use a small delay to ensure the control is ready to receive focus
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                firstInput.Focus();
            }, Avalonia.Threading.DispatcherPriority.Background);
        }
    }

    /// <summary>
    /// Handles keyboard input for overlay navigation and shortcuts.
    /// </summary>
    protected virtual void OnOverlayKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Escape)
        {
            // Close overlay on ESC key if closing is allowed
            if (DataContext is ViewModels.Overlay.BaseOverlayViewModel viewModel && viewModel.CanClose)
            {
                viewModel.CloseCommand.Execute(null);
                e.Handled = true;
            }
        }
        else if (e.Key == Avalonia.Input.Key.Enter)
        {
            // Execute confirm command on Enter key if available
            if (DataContext is ViewModels.Overlay.BaseOverlayViewModel viewModel && viewModel.ConfirmCommand.CanExecute(null))
            {
                viewModel.ConfirmCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Override in derived classes to provide custom content layout.
    /// </summary>
    protected virtual Control? CreateCustomContent()
    {
        return null;
    }

    /// <summary>
    /// Override in derived classes to customize action buttons.
    /// </summary>
    protected virtual void CustomizeActionButtons()
    {
        // Base implementation does nothing - override in derived classes
    }

    /// <summary>
    /// Helper method to find a descendant control of a specific type.
    /// </summary>
    protected T? FindDescendantOfType<T>() where T : Control
    {
        return FindDescendantOfType<T>(this);
    }

    /// <summary>
    /// Helper method to find a descendant control of a specific type starting from a parent.
    /// </summary>
    private T? FindDescendantOfType<T>(Control parent) where T : Control
    {
        try
        {
            if (parent is T typedControl)
                return typedControl;

            var children = parent.GetVisualChildren().OfType<Control>();
            foreach (var child in children)
            {
                var result = FindDescendantOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
