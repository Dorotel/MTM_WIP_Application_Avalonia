using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MTM_WIP_Application_Avalonia.Commands;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay
{
    /// <summary>
    /// ViewModel for SuggestionOverlayView. Designed for top-level overlay management.
    /// </summary>
    public class SuggestionOverlayViewModel : MTM_WIP_Application_Avalonia.ViewModels.Shared.BaseViewModel
    {
        private ObservableCollection<string> _suggestions = new();
        private string? _selectedSuggestion;
        private bool _isVisible;
        private readonly RelayCommand _selectCommand;
        
        public ObservableCollection<string> Suggestions
        {
            get => _suggestions;
            set { _suggestions = value; OnPropertyChanged(); }
        }
        public string? SelectedSuggestion
        {
            get => _selectedSuggestion;
            set
            {
                if (_selectedSuggestion != value)
                {
                    _selectedSuggestion = value;
                    OnPropertyChanged();
                    // Notify the SelectCommand that CanExecute may have changed
                    _selectCommand?.RaiseCanExecuteChanged();
                }
            }
        }
        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(); }
        }
        public ICommand SelectCommand => _selectCommand;
        public ICommand CancelCommand { get; }
        // Events for overlay manager to subscribe
    public event Action<string>? SuggestionSelected;
    public event Action? Cancelled;
        public SuggestionOverlayViewModel()
        {
            _selectCommand = new RelayCommand(Select, CanSelect);
            CancelCommand = new RelayCommand(Cancel);
        }
        public SuggestionOverlayViewModel(IEnumerable<string> suggestions) : this()
        {
            Suggestions = new ObservableCollection<string>(suggestions);
        }
        private void Select()
        {
            if (!string.IsNullOrEmpty(SelectedSuggestion))
            {
                SuggestionSelected?.Invoke(SelectedSuggestion);
                IsVisible = false;
            }
        }
        private bool CanSelect() => !string.IsNullOrEmpty(SelectedSuggestion);
        private void Cancel()
        {
            Cancelled?.Invoke();
            IsVisible = false;
        }
    }
}
