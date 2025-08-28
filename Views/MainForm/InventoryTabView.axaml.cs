using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for InventoryTabView.
/// Implements the primary inventory management interface within the MTM WIP Application.
/// Provides keyboard shortcuts, focus management, and event handling.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// </summary>
public partial class InventoryTabView : UserControl
{
    private InventoryTabViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the InventoryTabView.
    /// </summary>
    public InventoryTabView()
    {
        InitializeComponent();
        
        // Set up keyboard event handling
        KeyDown += OnKeyDown;
        
        // Set up loaded event to initialize ViewModel
        Loaded += OnLoaded;
    }

    /// <summary>
    /// Handles the Loaded event to set up the ViewModel via dependency injection.
    /// </summary>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Set up ViewModel if not already set
            if (DataContext is InventoryTabViewModel vm)
            {
                _viewModel = vm;
                
                // No event subscriptions needed since we're not using custom events
                System.Diagnostics.Debug.WriteLine("InventoryTabView ViewModel connected successfully");
            }
        }
        catch (Exception ex)
        {
            // Log error but don't crash the UI
            System.Diagnostics.Debug.WriteLine($"Error setting up InventoryTabView ViewModel: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles keyboard shortcuts as specified in MTM requirements.
    /// F5: Reset form (same as Reset button)
    /// Enter: Move to next control or save if on Save button
    /// Escape: Cancel current operation
    /// Shift+F5: Hard reset (refresh all data from database)
    /// </summary>
    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel == null) return;

        try
        {
            switch (e.Key)
            {
                case Key.F5:
                    e.Handled = true;
                    // Execute reset command
                    if (_viewModel.ResetCommand.CanExecute(null))
                    {
                        _viewModel.ResetCommand.Execute(null);
                    }
                    break;

                case Key.Enter:
                    e.Handled = true;
                    await HandleEnterKeyAsync(e.Source);
                    break;

                case Key.Escape:
                    e.Handled = true;
                    // Focus the first control to "cancel" current operation
                    MoveFocusToFirstControl();
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling keyboard shortcut: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles Enter key navigation - moves to next logical control or executes save.
    /// </summary>
    private async Task HandleEnterKeyAsync(object? source)
    {
        if (_viewModel == null) return;

        // If focused on Save button and can save, execute save command
        if (source is Button button && button.Name?.Contains("Save") == true)
        {
            if (_viewModel.SaveCommand.CanExecute(null))
            {
                _viewModel.SaveCommand.Execute(null);
                return;
            }
        }

        // Otherwise, move focus to next control
        MoveFocusToNextControl();
    }

    /// <summary>
    /// Moves focus to the next logical control in the form.
    /// Follows the order: Part -> Operation -> Location -> Quantity -> Notes -> Save button.
    /// </summary>
    private void MoveFocusToNextControl()
    {
        try
        {
            var currentFocus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            
            // Define the logical tab order based on the form layout
            var tabOrder = new[]
            {
                "PartAutoCompleteBox",
                "OperationAutoCompleteBox", 
                "LocationAutoCompleteBox",
                "QuantityTextBox",
                "NotesTextBox",
                "SaveButton"
            };

            // Find current control in tab order
            string? currentName = (currentFocus as Control)?.Name;
            int currentIndex = Array.IndexOf(tabOrder, currentName);
            
            // Move to next control, or start from beginning if not found
            int nextIndex = currentIndex >= 0 ? (currentIndex + 1) % tabOrder.Length : 0;
            string nextControlName = tabOrder[nextIndex];
            
            // Find and focus the next control
            var nextControl = this.FindControl<Control>(nextControlName);
            nextControl?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to next control: {ex.Message}");
        }
    }

    /// <summary>
    /// Moves focus to the first control in the form (Part AutoCompleteBox).
    /// </summary>
    private void MoveFocusToFirstControl()
    {
        try
        {
            var partAutoCompleteBox = this.FindControl<AutoCompleteBox>("PartAutoCompleteBox");
            partAutoCompleteBox?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to first control: {ex.Message}");
        }
    }

    #region Cleanup

    /// <summary>
    /// Cleans up event subscriptions when the view is being disposed.
    /// </summary>
    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Clean up event subscriptions
            KeyDown -= OnKeyDown;
            Loaded -= OnLoaded;
            
            System.Diagnostics.Debug.WriteLine("InventoryTabView cleanup completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during InventoryTabView cleanup: {ex.Message}");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion
}
