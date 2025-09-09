using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using System;

namespace MTM_WIP_Application_Avalonia.ViewModels
{
    /// <summary>
    /// Temporary stub for ThemeEditorViewModel to fix build issues
    /// </summary>
    public partial class ThemeEditorViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty] 
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _hasUnsavedChanges;

        [ObservableProperty]
        private string _currentThemeName = "MTM Theme";

        public event EventHandler? CloseRequested;

        public ThemeEditorViewModel(ILogger<ThemeEditorViewModel> logger) : base(logger)
        {
            Logger.LogInformation("ThemeEditorViewModel stub initialized");
        }
    }
}