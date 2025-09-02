using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using System.Diagnostics;
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
                Debug.WriteLine($"[DBG:VIEW] SuggestionOverlayView attached. DataContext type: {DataContext?.GetType().FullName ?? "null"}");
                Debug.WriteLine($"[DBG:VIEW] UserControl.IsEnabled: {this.IsEnabled}");
                if (this.Parent is Control parent)
                    Debug.WriteLine($"[DBG:VIEW] Parent control type: {parent.GetType().FullName}, IsEnabled: {parent.IsEnabled}");
                _logger?.LogInformation("[DBG:VIEW] AttachedToVisualTree triggered. DataContext type: {DataContextType}, IsEnabled: {IsEnabled}, ParentType: {ParentType}", DataContext?.GetType().FullName ?? "null", this.IsEnabled, this.Parent?.GetType().FullName ?? "null");

                // Debug the suggestions in the ViewModel
                if (DataContext is SuggestionOverlayViewModel vm)
                {
                    Debug.WriteLine($"[DBG:VIEW] ViewModel has {vm.Suggestions.Count} suggestions");
                    if (vm.Suggestions.Count > 0)
                    {
                        Debug.WriteLine($"[DBG:VIEW] First suggestion: {vm.Suggestions.First()}");
                    }
                    _logger?.LogInformation("[DBG:VIEW] ViewModel has {Count} suggestions. First: {First}", vm.Suggestions.Count, vm.Suggestions.FirstOrDefault() ?? "none");
                }

                // Focus the ListBox for immediate keyboard navigation
                var listBox = this.FindControl<ListBox>("SuggestionListBox");
                if (listBox != null)
                {
                    Debug.WriteLine($"[DBG:VIEW] Found ListBox. ItemsSource set: {listBox.ItemsSource != null}");
                    _logger?.LogInformation("[DBG:VIEW] Found ListBox. ItemsSource: {HasItemsSource}", listBox.ItemsSource != null);
                    listBox.Focus();
                }
                else
                {
                    Debug.WriteLine("[DBG:VIEW] ListBox not found!");
                    _logger?.LogWarning("[DBG:VIEW] ListBox not found!");
                }

                // TODO: Add open/close animation and accessibility improvements (screen reader hints, ARIA roles, etc.)
            };
        }

        /// <summary>
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
