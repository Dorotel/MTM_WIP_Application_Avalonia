using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using System;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
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
            this.DataContextChanged += SuggestionOverlayView_DataContextChanged;
            this.AttachedToVisualTree += (s, e) =>
            {
                Debug.WriteLine($"[DBG:VIEW] SuggestionOverlayView attached. DataContext type: {DataContext?.GetType().FullName ?? "null"}");
                Debug.WriteLine($"[DBG:VIEW] UserControl.IsEnabled: {this.IsEnabled}");
                if (this.Parent is Control parent)
                    Debug.WriteLine($"[DBG:VIEW] Parent control type: {parent.GetType().FullName}, IsEnabled: {parent.IsEnabled}");
                _logger?.LogInformation("[DBG:VIEW] AttachedToVisualTree triggered. DataContext type: {DataContextType}, IsEnabled: {IsEnabled}, ParentType: {ParentType}", DataContext?.GetType().FullName ?? "null", this.IsEnabled, this.Parent?.GetType().FullName ?? "null");

                // Recreate DataContext (ViewModel) if needed
                if (DataContext is not MTM_WIP_Application_Avalonia.ViewModels.MainForm.SuggestionOverlayViewModel)
                {
                    // If DataContext is null or wrong type, create a new ViewModel with empty suggestions
                    DataContext = new MTM_WIP_Application_Avalonia.ViewModels.MainForm.SuggestionOverlayViewModel(Array.Empty<string>());
                    Debug.WriteLine("[DBG:VIEW] SuggestionOverlayView DataContext recreated as new SuggestionOverlayViewModel (empty suggestions)");
                }
            };
        }

        // Handles double-click on a suggestion item
        public void OnSuggestionItemDoubleTapped(object? sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[DBG:CLICK] DoubleTapped event: sender={sender?.GetType().Name}, e={e}");
            _logger?.LogInformation("[DBG:CLICK] OnSuggestionItemDoubleTapped triggered. sender={Sender}, e={Event}", sender?.GetType().Name, e);
            if (DataContext is SuggestionOverlayViewModel vm && vm.SelectCommand != null)
            {
                Debug.WriteLine($"[DBG:CLICK] DoubleTapped: SelectedSuggestion='{vm.SelectedSuggestion}', Suggestions.Count={vm.Suggestions?.Count}");
                _logger?.LogInformation("[DBG:CLICK] DoubleTapped: SelectedSuggestion={SelectedSuggestion}, Suggestions.Count={Count}", vm.SelectedSuggestion, vm.Suggestions?.Count);
                if (vm.SelectCommand.CanExecute(vm.SelectedSuggestion))
                {
                    Debug.WriteLine($"[DBG:CLICK] SelectCommand.CanExecute returned true. Executing...");
                    _logger?.LogInformation("[DBG:CLICK] SelectCommand executed via double-tap");
                    vm.SelectCommand.Execute(vm.SelectedSuggestion);
                }
                else
                {
                    Debug.WriteLine($"[DBG:CLICK] SelectCommand.CanExecute returned false. Not executing.");
                }
            }
            else
            {
                Debug.WriteLine($"[DBG:CLICK] DataContext is not SuggestionOverlayViewModel or SelectCommand is null");
            }
        }

        // Handles Enter key on the ListBox
        public void OnSuggestionListBoxKeyDown(object? sender, KeyEventArgs e)
        {
            Debug.WriteLine($"[DBG:KEYDOWN] Key event: Key={e.Key}, Modifiers={e.KeyModifiers}, Source={sender?.GetType().Name}");
            _logger?.LogInformation("[DBG:KEYDOWN] Key event: Key={Key}, Modifiers={Modifiers}, Source={Source}", e.Key, e.KeyModifiers, sender?.GetType().Name);

            if (DataContext is SuggestionOverlayViewModel vm)
            {
                Debug.WriteLine($"[DBG:KEYDOWN] DataContext is SuggestionOverlayViewModel: {vm.GetHashCode()}");
                Debug.WriteLine($"[DBG:KEYDOWN] Suggestions count: {vm.Suggestions?.Count}");
                Debug.WriteLine($"[DBG:KEYDOWN] SelectedSuggestion: '{vm.SelectedSuggestion}'");
                Debug.WriteLine($"[DBG:KEYDOWN] IsSuggestionSelected: {vm.IsSuggestionSelected}");
                Debug.WriteLine($"[DBG:KEYDOWN] SelectCommand: {vm.SelectCommand?.GetType().Name}");
                Debug.WriteLine($"[DBG:KEYDOWN] CanExecute(SelectedSuggestion): {vm.SelectCommand?.CanExecute(vm.SelectedSuggestion)}");
            }
            else
            {
                Debug.WriteLine($"[DBG:KEYDOWN] DataContext is not SuggestionOverlayViewModel: {DataContext?.GetType().FullName ?? "null"}");
            }

            if (e.Key == Avalonia.Input.Key.Enter)
            {
                Debug.WriteLine($"[DBG:KEYDOWN] Enter key pressed");
                if (DataContext is SuggestionOverlayViewModel vm2 && vm2.SelectCommand != null)
                {
                    Debug.WriteLine($"[DBG:KEYDOWN] Attempting SelectCommand execution. SelectedSuggestion: '{vm2.SelectedSuggestion}'");
                    _logger?.LogInformation("[DBG:KEYDOWN] Enter pressed: SelectedSuggestion={SelectedSuggestion}", vm2.SelectedSuggestion);
                    if (vm2.SelectCommand.CanExecute(vm2.SelectedSuggestion))
                    {
                        Debug.WriteLine($"[DBG:KEYDOWN] SelectCommand.CanExecute returned true. Executing...");
                        _logger?.LogInformation("[DBG:KEYDOWN] SelectCommand executed via Enter key");
                        vm2.SelectCommand.Execute(vm2.SelectedSuggestion);
                        e.Handled = true;
                    }
                    else
                    {
                        Debug.WriteLine($"[DBG:KEYDOWN] SelectCommand.CanExecute returned false. Not executing.");
                    }
                }
                else
                {
                    Debug.WriteLine($"[DBG:KEYDOWN] DataContext is not SuggestionOverlayViewModel or SelectCommand is null");
                }
            }
        }

        public Task<string?> ShowAsync(Window parent, SuggestionOverlayViewModel viewModel)
        {
            Debug.WriteLine($"[DBG:SHOWASYNC] ShowAsync called. parent={parent?.GetType().Name}, viewModel={viewModel?.GetHashCode()}");
            _logger?.LogInformation("ShowAsync called");
            var tcs = new TaskCompletionSource<string?>();
            viewModel.SuggestionSelected += result => {
                Debug.WriteLine($"[DBG:SHOWASYNC] SuggestionSelected event: result={result}");
                _logger?.LogInformation("SuggestionSelected event: result={Result}", result);
                tcs.TrySetResult(result);
            };
            viewModel.Cancelled += () => {
                Debug.WriteLine($"[DBG:SHOWASYNC] Cancelled event triggered");
                _logger?.LogInformation("Cancelled event triggered");
                tcs.TrySetResult(null);
            };
            DataContext = viewModel;
            Debug.WriteLine($"[DBG:ShowAsync] DataContext set to viewModel instance: {viewModel.GetHashCode()}");
            _logger?.LogInformation($"DataContext set in ShowAsync. viewModel instance: {viewModel.GetHashCode()}");
            // Add to parent visual tree, show overlay, etc. (handled by parent)
            return tcs.Task;
        }

        private void SuggestionOverlayView_DataContextChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"[DBG:VIEW] SuggestionOverlayView_DataContextChanged triggered. sender={sender?.GetType().Name}, e={e}");
            _logger?.LogInformation("DataContextChanged event triggered");
            Debug.WriteLine($"[DBG:VIEW] DataContextChanged: type={DataContext?.GetType().FullName ?? "null"}");
            if (DataContext is SuggestionOverlayViewModel vm)
            {
                _logger?.LogInformation("DataContext is SuggestionOverlayViewModel");
                Debug.WriteLine($"[DBG:VIEW] DataContext is SuggestionOverlayViewModel instance: {vm.GetHashCode()}");
            }
        }
    }
}
