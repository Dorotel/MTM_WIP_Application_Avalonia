using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTM_WIP_Application_Avalonia.ViewModels
{
    public class ThemeEditorViewModel
    {
        public ObservableCollection<string> AvailableThemes { get; set; }

        public ThemeEditorViewModel()
        {
            AvailableThemes = new ObservableCollection<string>
            {
                "Light",
                "Dark",
                "Custom"
            };
        }
    }
}
