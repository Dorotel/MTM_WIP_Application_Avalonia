using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// QuickButtonsView - Enhanced with manual reordering functionality and server-side persistence
/// </summary>
public partial class QuickButtonsView : UserControl
{
    private QuickButtonItemViewModel? _selectedButton;
    private Button? _selectedButtonControl;
    private bool _isDragging;
    private Point _startPoint;

    public QuickButtonsView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handle pointer press to select button for potential reordering
    /// </summary>
    private void OnButtonPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Button button && button.DataContext is QuickButtonItemViewModel buttonViewModel)
        {
            _startPoint = e.GetPosition(button);
            _selectedButton = buttonViewModel;
            _selectedButtonControl = button;
            _isDragging = false;
            
            // Visual feedback that button is selected for dragging
            button.Classes.Add("selected-for-drag");
        }
    }

    /// <summary>
    /// Handle pointer movement to detect drag intention
    /// </summary>
    private void OnButtonPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_selectedButton != null && _selectedButtonControl != null)
        {
            var currentPoint = e.GetPosition(_selectedButtonControl);
            var deltaY = Math.Abs(currentPoint.Y - _startPoint.Y);
            
            // Start visual dragging feedback if moved beyond threshold
            if (!_isDragging && deltaY > 15)
            {
                _isDragging = true;
                _selectedButtonControl.Classes.Add("dragging");
            }
        }
    }

    /// <summary>
    /// Handle pointer release to potentially drop at new position
    /// </summary>
    private async void OnButtonPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_selectedButtonControl != null)
        {
            _selectedButtonControl.Classes.Remove("selected-for-drag");
            _selectedButtonControl.Classes.Remove("dragging");
            
            // If we were dragging, try to find drop target
            if (_isDragging && DataContext is QuickButtonsViewModel viewModel)
            {
                var dropTarget = FindDropTarget(e.GetPosition(this));
                if (dropTarget != null && dropTarget != _selectedButton)
                {
                    try
                    {
                        await viewModel.ReorderButtonAsync(_selectedButton.Position, dropTarget.Position);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't throw - UI should remain responsive
                        System.Diagnostics.Debug.WriteLine($"Error reordering buttons: {ex.Message}");
                    }
                }
            }
        }
        
        _selectedButton = null;
        _selectedButtonControl = null;
        _isDragging = false;
    }

    /// <summary>
    /// Find the drop target button based on pointer position using proper visual tree hit testing
    /// </summary>
    private QuickButtonItemViewModel? FindDropTarget(Point position)
    {
        try
        {
            // Use Avalonia's visual tree hit testing
            var hitResult = this.InputHitTest(position);
            if (hitResult is Control control)
            {
                // Walk up the visual tree to find a button with QuickButtonItemViewModel
                var current = control;
                while (current != null)
                {
                    if (current is Button button && button.DataContext is QuickButtonItemViewModel buttonViewModel)
                    {
                        return buttonViewModel;
                    }
                    current = current.Parent as Control;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error finding drop target: {ex.Message}");
        }
        
        return null;
    }

    /// <summary>
    /// Handle context menu remove button action
    /// </summary>
    private async void OnRemoveButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && 
            menuItem.DataContext is QuickButtonItemViewModel buttonViewModel &&
            DataContext is QuickButtonsViewModel viewModel)
        {
            try
            {
                // Show confirmation dialog first
                var result = await ShowConfirmationDialog(
                    "Remove Quick Button", 
                    $"Are you sure you want to remove the quick button for {buttonViewModel.PartId}?");
                
                if (result)
                {
                    // Use the ViewModel's command to remove the button (which handles server-side removal)
                    if (viewModel.RemoveButtonCommand.CanExecute(buttonViewModel))
                    {
                        viewModel.RemoveButtonCommand.Execute(buttonViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing button: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    private async Task<bool> ShowConfirmationDialog(string title, string message)
    {
        try
        {
            var dialog = new Window
            {
                Title = title,
                Width = 350,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };
            
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = message, 
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var buttonPanel = new StackPanel 
            { 
                Orientation = Avalonia.Layout.Orientation.Horizontal, 
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right 
            };
            
            var yesButton = new Button 
            { 
                Content = "Yes", 
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            
            var noButton = new Button 
            { 
                Content = "No",
                IsCancel = true
            };

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);
            stackPanel.Children.Add(buttonPanel);

            dialog.Content = stackPanel;

            bool result = false;

            yesButton.Click += (s, e) => 
            { 
                result = true;
                dialog.Close();
            };

            noButton.Click += (s, e) => dialog.Close();

            if (TopLevel.GetTopLevel(this) is Window parentWindow)
            {
                await dialog.ShowDialog(parentWindow);
            }
            else
            {
                dialog.Show();
            }

            return result;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing confirmation dialog: {ex.Message}");
            return false;
        }
    }
}
