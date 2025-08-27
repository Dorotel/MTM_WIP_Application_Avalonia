using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Code-behind for InventoryTabView.
/// Implements the primary inventory management interface within the MTM WIP Application.
/// Provides keyboard shortcuts, focus management, and event handling.
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
                
                // Subscribe to ViewModel events
                _viewModel.InventoryItemSaved += OnInventoryItemSaved;
                _viewModel.PanelToggleRequested += OnPanelToggleRequested;
                _viewModel.AdvancedEntryRequested += OnAdvancedEntryRequested;
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
                    // Check if Shift is pressed for hard reset
                    bool hardReset = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
                    
                    // Use ReactiveCommand.Execute with parameter
                    _viewModel.ResetCommand.Execute(hardReset).Subscribe();
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
            // Check if save command can execute using CanExecute observable
            var canExecute = await _viewModel.SaveCommand.CanExecute.Take(1);
            if (canExecute)
            {
                _viewModel.SaveCommand.Execute().Subscribe();
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
                "PartComboBox",
                "OperationComboBox", 
                "LocationComboBox",
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
    /// Moves focus to the first control in the form (Part ComboBox).
    /// </summary>
    private void MoveFocusToFirstControl()
    {
        try
        {
            var partComboBox = this.FindControl<ComboBox>("PartComboBox");
            partComboBox?.Focus();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error moving focus to first control: {ex.Message}");
        }
    }

    #region ViewModel Event Handlers

    /// <summary>
    /// Handles the InventoryItemSaved event from the ViewModel.
    /// Forwards to parent components for quick buttons integration.
    /// </summary>
    private void OnInventoryItemSaved(object? sender, InventoryItemSavedEventArgs e)
    {
        try
        {
            // This event is typically handled by the parent MainForm or MainView
            // which will update quick buttons and other related components
            
            // For now, just log the successful save
            System.Diagnostics.Debug.WriteLine(
                $"Inventory item saved: {e.PartId} - Operation {e.Operation} - Quantity {e.Quantity}");
            
            // In a full implementation, this would trigger:
            // 1. Quick button updates via MainForm
            // 2. Inventory grid refresh in other tabs
            // 3. Recent transactions update
            // 4. Progress notifications
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling inventory item saved event: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the PanelToggleRequested event from the ViewModel.
    /// Forwards to parent form to toggle quick actions panel visibility.
    /// </summary>
    private void OnPanelToggleRequested(object? sender, EventArgs e)
    {
        try
        {
            // This would typically be handled by MainForm to show/hide the quick buttons panel
            System.Diagnostics.Debug.WriteLine("Panel toggle requested");
            
            // In a full implementation, this would:
            // 1. Toggle the visibility of the right panel (quick buttons)
            // 2. Adjust the main content area layout
            // 3. Save user preference for panel state
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling panel toggle request: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the AdvancedEntryRequested event from the ViewModel.
    /// Opens the Control_AdvancedInventory interface.
    /// </summary>
    private void OnAdvancedEntryRequested(object? sender, EventArgs e)
    {
        try
        {
            // This would typically open the AdvancedInventoryView
            System.Diagnostics.Debug.WriteLine("Advanced entry requested");
            
            // In a full implementation, this would:
            // 1. Open AdvancedInventoryView as modal dialog or navigate to it
            // 2. Pass current form data as initial values
            // 3. Handle return data from advanced entry
            
            // For now, just indicate that advanced entry would be opened
            // This integration would be handled by the navigation service
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error handling advanced entry request: {ex.Message}");
        }
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Cleans up event subscriptions when the view is being disposed.
    /// </summary>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Unsubscribe from ViewModel events to prevent memory leaks
            if (_viewModel != null)
            {
                _viewModel.InventoryItemSaved -= OnInventoryItemSaved;
                _viewModel.PanelToggleRequested -= OnPanelToggleRequested;
                _viewModel.AdvancedEntryRequested -= OnAdvancedEntryRequested;
            }

            // Unsubscribe from control events
            KeyDown -= OnKeyDown;
            Loaded -= OnLoaded;
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
