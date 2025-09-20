using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MTM_WIP_Application_Avalonia.Views.Overlays
{
    /// <summary>
    /// View for the Field Validation overlay providing real-time field validation UI
    /// </summary>
    public partial class FieldValidationOverlayView : UserControl
    {
        public FieldValidationOverlayView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // Focus the field value text box when the overlay loads
            var textBox = this.FindControl<TextBox>("FieldValueTextBox");
            textBox?.Focus();
        }
    }
}
