using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

namespace MTM_WIP_Application_Avalonia.Views
{
    /// <summary>
    /// Note Editor overlay view.
    /// Provides a text editor interface for reading and editing inventory item notes.
    /// </summary>
    public partial class NoteEditorView : UserControl
    {
        public NoteEditorView()
        {
            InitializeComponent();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            
            // Focus the text box when the view is shown
            var textBox = this.FindControl<TextBox>("NoteTextBox");
            textBox?.Focus();
        }

        private void OnUserControlKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Cancel on Escape key
                if (DataContext is NoteEditorViewModel viewModel)
                {
                    viewModel.CancelCommand.Execute(null);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                // Save on Ctrl+S
                if (DataContext is NoteEditorViewModel viewModel && viewModel.SaveCommand.CanExecute(null))
                {
                    viewModel.SaveCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        private void OnCloseClicked(object? sender, RoutedEventArgs e)
        {
            if (DataContext is NoteEditorViewModel viewModel)
            {
                viewModel.CancelCommand.Execute(null);
            }
        }
    }
}