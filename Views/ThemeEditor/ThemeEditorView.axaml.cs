using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// ThemeEditorView - Professional theme color editing interface
/// </summary>
public partial class ThemeEditorView : UserControl
{
    public ThemeEditorView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles color preview border clicks to open color picker
    /// </summary>
    private void ColorPreview_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && 
            border.Tag is string colorProperty && 
            DataContext is ThemeEditorViewModel viewModel)
        {
            // Execute the color picker command with the property name
            if (viewModel.OpenColorPickerCommand.CanExecute(colorProperty))
            {
                viewModel.OpenColorPickerCommand.Execute(colorProperty);
            }
        }
    }
}
