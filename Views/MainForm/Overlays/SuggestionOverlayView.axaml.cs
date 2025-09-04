using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System;
using System.Linq;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Views
{

    public partial class SuggestionOverlayView : UserControl
    {
        private readonly Microsoft.Extensions.Logging.ILogger<SuggestionOverlayView>? _logger;

        public SuggestionOverlayView() : this(null) { }

    public SuggestionOverlayView(Microsoft.Extensions.Logging.ILogger<SuggestionOverlayView>? logger)
    {
        InitializeComponent();
        _logger = logger;
        _logger?.LogInformation("SuggestionOverlayView initialized");
        
        // Focus management: set focus to ListBox when overlay appears
        this.AttachedToVisualTree += (s, e) =>
        {
            _logger?.LogInformation("SuggestionOverlayView attached to visual tree. DataContext type: {DataContextType}, IsEnabled: {IsEnabled}, ParentType: {ParentType}", 
                DataContext?.GetType().FullName ?? "null", 
                this.IsEnabled, 
                this.Parent?.GetType().FullName ?? "null");

            // Debug the suggestions in the ViewModel
            if (DataContext is SuggestionOverlayViewModel vm)
            {
                _logger?.LogInformation("ViewModel has {Count} suggestions. First: {First}", 
                    vm.Suggestions.Count, 
                    vm.Suggestions.FirstOrDefault() ?? "none");
            }

            // Focus the ListBox for immediate keyboard navigation
            var listBox = this.FindControl<ListBox>("SuggestionListBox");
            if (listBox != null)
            {
                _logger?.LogInformation("Found ListBox. ItemsSource: {HasItemsSource}", listBox.ItemsSource != null);
                listBox.Focus();
            }
            else
            {
                _logger?.LogWarning("ListBox not found!");
            }
        };
    }        /// <summary>
        /// Handles double-tap on the ListBox to select the currently selected suggestion.
        /// </summary>
        public void OnSuggestionListBoxDoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (DataContext is SuggestionOverlayViewModel vm && vm.SelectCommand != null && vm.SelectedSuggestion != null)
            {
                if (vm.SelectCommand.CanExecute(null))
                {
                    vm.SelectCommand.Execute(null);
                    _logger?.LogInformation("Suggestion '{Suggestion}' selected via double-tap.", vm.SelectedSuggestion);
                }
            }
        }

        /// <summary>
        /// Handles double-tap on a suggestion item in the ListBox.
        /// </summary>
        public void OnSuggestionItemDoubleTapped(object? sender, RoutedEventArgs e)
        {
            if (DataContext is SuggestionOverlayViewModel vm && vm.SelectCommand != null)
            {
                if (vm.SelectCommand.CanExecute(null))
                {
                    vm.SelectCommand.Execute(null);
                    _logger?.LogInformation("Suggestion selected via double-tap.");
                }
            }
        }

        /// <summary>
        /// Handles clicking the close button.
        /// </summary>
        public void OnCloseClicked(object? sender, RoutedEventArgs e)
        {
            if (DataContext is SuggestionOverlayViewModel vm && vm.CancelCommand != null)
            {
                if (vm.CancelCommand.CanExecute(null))
                {
                    vm.CancelCommand.Execute(null);
                    _logger?.LogInformation("Overlay closed via close button.");
                }
            }
        }

        /// <summary>
        /// Handles Enter key on the ListBox to select a suggestion.
        /// </summary>
        public void OnSuggestionListBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Enter && DataContext is SuggestionOverlayViewModel vm && vm.SelectCommand != null)
            {
                if (vm.SelectCommand.CanExecute(null))
                {
                    vm.SelectCommand.Execute(null);
                    _logger?.LogInformation("Suggestion selected via Enter key.");
                    e.Handled = true;
                }
            }
        }
    }
}
