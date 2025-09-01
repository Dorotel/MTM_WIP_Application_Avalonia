using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm
{
        public class SuggestionOverlayViewModel : BaseViewModel
        {
        private readonly Microsoft.Extensions.Logging.ILogger<SuggestionOverlayViewModel>? _logger;

        private void LogInfo(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
            try
            {
                string debugMsg = args != null && args.Length > 0 ? string.Format(message.Replace("{", "{0:"), args) : message;
                Debug.WriteLine($"[SuggestionOverlayViewModel] {debugMsg}");
            }
            catch
            {
                Debug.WriteLine($"[SuggestionOverlayViewModel] {message} (logging error)");
            }
        }

        public ObservableCollection<string> Suggestions { get; }
    private string? _selectedSuggestion;
    private static int _debugInstanceIdCounter = 0;
    private readonly int _debugInstanceId;
        public string? SelectedSuggestion
        {
            get => _selectedSuggestion;
            set
            {
                var oldValue = _selectedSuggestion;
                Debug.WriteLine($"[DBG:VM] SelectedSuggestion.SET triggered. OldValue='{oldValue}', NewValue='{value}'.");
                LogInfo($"[DBG:{_debugInstanceId}] SelectedSuggestion.SET triggered. OldValue='{{oldValue}}', NewValue='{{value}}'");
                if (SetProperty(ref _selectedSuggestion, value))
                {
                    Debug.WriteLine($"[DBG:VM] SelectedSuggestion changed from '{oldValue}' to '{_selectedSuggestion}'");
                    LogInfo($"[DBG:{_debugInstanceId}] SelectedSuggestion changed from '{{oldValue}}' to '{{_selectedSuggestion}}'");
                    IsSuggestionSelected = value != null;
                    OnPropertyChanged(nameof(IsSuggestionSelected));
                    if (SelectCommand is MTM_WIP_Application_Avalonia.Commands.RelayCommand rc)
                    {
                        Debug.WriteLine($"[DBG:VM] Raising CanExecuteChanged on SelectCommand");
                        rc.RaiseCanExecuteChanged();
                    }
                }
                else
                {
                    Debug.WriteLine($"[DBG:VM] SelectedSuggestion set called but value unchanged (still '{_selectedSuggestion}')");
                    LogInfo($"[DBG:{_debugInstanceId}] SelectedSuggestion set called but value unchanged (still '{{_selectedSuggestion}}')");
                }
            }
        }

        private bool _isSuggestionSelected;
        public bool IsSuggestionSelected
        {
            get => _isSuggestionSelected;
            set
            {
                Debug.WriteLine($"[DBG:VM] IsSuggestionSelected.SET triggered. OldValue='{_isSuggestionSelected}', NewValue='{value}'");
                if (SetProperty(ref _isSuggestionSelected, value))
                {
                    Debug.WriteLine($"[DBG:VM] IsSuggestionSelected changed to '{value}'");
                    LogInfo("IsSuggestionSelected set to: {0}", value);
                    if (SelectCommand is MTM_WIP_Application_Avalonia.Commands.RelayCommand rc)
                    {
                        Debug.WriteLine($"[DBG:VM] Raising CanExecuteChanged on SelectCommand (IsSuggestionSelected)");
                        rc.RaiseCanExecuteChanged();
                    }
                    OnPropertyChanged(nameof(IsSuggestionSelected));
                }
            }
        }

        public ICommand SelectCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action<string?>? SuggestionSelected;
        public event Action? Cancelled;

        public SuggestionOverlayViewModel(IEnumerable<string> suggestions)
        {
            _debugInstanceId = ++_debugInstanceIdCounter;
            LogInfo($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel instance created");
            System.Diagnostics.Debug.WriteLine($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel instance created");
            Suggestions = new ObservableCollection<string>(suggestions);
            Debug.WriteLine($"[DBG:VM] SuggestionOverlayViewModel constructor. Suggestions.Count={Suggestions.Count}");
            LogInfo($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel constructor. Suggestions.Count={{0}}", Suggestions.Count);
            SelectCommand = new RelayCommand(OnSelect, () =>
            {
                bool result = !string.IsNullOrEmpty(SelectedSuggestion) && Suggestions.Contains(SelectedSuggestion!);
                Debug.WriteLine($"[DBG:VM] SelectCommand.CanExecute check. SelectedSuggestion='{SelectedSuggestion}', Result={result}");
                LogInfo($"[DBG:{_debugInstanceId}] SelectCommand.CanExecute check. SelectedSuggestion='{{0}}', Result={{1}}", SelectedSuggestion, result);
                return result;
            });
            CancelCommand = new RelayCommand(OnCancel);

            // Ensure Select button state updates when Suggestions change
            Suggestions.CollectionChanged += (_, __) =>
            {
                Debug.WriteLine($"[DBG:VM] Suggestions collection changed. Suggestions.Count={Suggestions.Count}");
                LogInfo($"[DBG:{_debugInstanceId}] Suggestions collection changed. Suggestions.Count={{0}}", Suggestions.Count);
                IsSuggestionSelected = SelectedSuggestion != null;
                if (SelectCommand is MTM_WIP_Application_Avalonia.Commands.RelayCommand rc)
                {
                    Debug.WriteLine($"[DBG:VM] Raising CanExecuteChanged on SelectCommand (Suggestions.CollectionChanged)");
                    rc.RaiseCanExecuteChanged();
                }
            };

            // Initial state update
            IsSuggestionSelected = SelectedSuggestion != null;
            if (SelectCommand is MTM_WIP_Application_Avalonia.Commands.RelayCommand rcInit)
            {
                Debug.WriteLine($"[DBG:VM] Raising CanExecuteChanged on SelectCommand (constructor init)");
                rcInit.RaiseCanExecuteChanged();
            }
            Debug.WriteLine($"[DBG:VM] SuggestionOverlayViewModel initialized");
            LogInfo($"[DBG:{_debugInstanceId}] SuggestionOverlayViewModel initialized");
        }

        private void OnSelect()
        {
            var sel = SelectedSuggestion != null ? SelectedSuggestion : "null";
            Debug.WriteLine($"[DBG:VM] OnSelect called. SelectedSuggestion='{sel}'");
            LogInfo($"[DBG:{_debugInstanceId}] SelectCommand executed: SelectedSuggestion={sel}");
            SuggestionSelected?.Invoke(SelectedSuggestion);
        }
        private void OnCancel()
        {
            Debug.WriteLine($"[DBG:VM] OnCancel called");
            LogInfo($"[DBG:{_debugInstanceId}] CancelCommand executed");
            Cancelled?.Invoke();
        }
    protected override void OnPropertyChanged(string? propertyName)
        {
            Debug.WriteLine($"[DBG:VM] OnPropertyChanged: {propertyName}");
            base.OnPropertyChanged(propertyName);
            LogInfo($"[DBG:{_debugInstanceId}] PropertyChanged: {propertyName}");
        }
    }
}
