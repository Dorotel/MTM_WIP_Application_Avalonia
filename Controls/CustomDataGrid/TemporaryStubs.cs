using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Temporary stub class for ColumnFilter to fix compilation errors
    /// This should be replaced with the actual ColumnFilter implementation
    /// </summary>
    public partial class ColumnFilter : ObservableObject
    {
        [ObservableProperty]
        private string _columnName = string.Empty;
        
        [ObservableProperty]
        private string _filterValue = string.Empty;
        
        [ObservableProperty]
        private bool _isEnabled = true;
    }

    /// <summary>
    /// Temporary stub class for FilterStatistics to fix compilation errors
    /// This should be replaced with the actual FilterStatistics implementation
    /// </summary>
    public partial class FilterStatistics : ObservableObject
    {
        [ObservableProperty]
        private int _totalRecords;
        
        [ObservableProperty]
        private int _filteredRecords;
        
        [ObservableProperty]
        private int _activeFilters;
    }
}