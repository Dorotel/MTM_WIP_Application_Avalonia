using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using System.Linq;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

/// <summary>
/// Code-behind for SuggestionOverlayView following WeekendRefactor minimal pattern.
/// Handles only essential UI interactions - business logic is in ViewModel.
/// </summary>
public partial class SuggestionOverlayView : UserControl
{
    public SuggestionOverlayView()
    {
        InitializeComponent();

        // Set up keyboard handling
        KeyDown += OnViewKeyDown;

        // Focus management when attached to visual tree
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    /// <summary>
    /// Handles focus management when view is attached to visual tree.
    /// </summary>
    private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        // Focus the ListBox for keyboard navigation when overlay appears
        var listBox = this.FindControl<ListBox>("SuggestionListBox");
        if (listBox != null && DataContext is SuggestionOverlayViewModel vm && vm.Suggestions.Any())
        {
            // Ensure first item is selected and ListBox is focused
            if (listBox.SelectedIndex == -1 && vm.Suggestions.Count > 0)
            {
                listBox.SelectedIndex = 0;
            }

            listBox.Focus();
        }
    }

    /// <summary>
    /// Handles keyboard shortcuts at the View level.
    /// </summary>
    private void OnViewKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not SuggestionOverlayViewModel vm) return;

        switch (e.Key)
        {
            case Key.Escape:
                vm.CancelCommand.Execute(null);
                e.Handled = true;
                break;

            case Key.Enter:
                if (vm.SelectCommand.CanExecute(null))
                {
                    vm.SelectCommand.Execute(null);
                    e.Handled = true;
                }
                break;
        }
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        // Cleanup when view is removed
        KeyDown -= OnViewKeyDown;
        AttachedToVisualTree -= OnAttachedToVisualTree;
        base.OnDetachedFromVisualTree(e);
    }
}
