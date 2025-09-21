using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// QuickButtonsView - Simple view for quick action buttons
/// </summary>
public partial class QuickButtonsView : UserControl
{
    public QuickButtonsView()
    {
        InitializeComponent();
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
