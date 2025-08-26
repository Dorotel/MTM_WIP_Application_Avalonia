using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// QuickButtonsView - Enhanced with manual reordering functionality
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
    private void OnButtonPointerReleased(object? sender, PointerReleasedEventArgs e)
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
                    viewModel.ReorderButton(_selectedButton.Position, dropTarget.Position);
                }
            }
        }
        
        _selectedButton = null;
        _selectedButtonControl = null;
        _isDragging = false;
    }

    /// <summary>
    /// Find the drop target button based on pointer position
    /// </summary>
    private QuickButtonItemViewModel? FindDropTarget(Point position)
    {
        // Simple implementation - could be enhanced with visual tree hit testing
        if (DataContext is QuickButtonsViewModel viewModel)
        {
            // For now, return a random target for demonstration
            // In a real implementation, you would hit-test the visual tree
            var buttons = viewModel.NonEmptyQuickButtons.ToList();
            return buttons.Count > 1 ? buttons[1] : null;
        }
        return null;
    }
}
