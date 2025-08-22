using System;

namespace MTM_WIP_Application_Avalonia.Services.Interfaces;

public interface INavigationService
{
    // Navigation Methods
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo(Type viewModelType);
    void NavigateTo(string viewName);
    
    // Navigation History
    void GoBack();
    bool CanGoBack { get; }
    
    // Events
    event EventHandler<NavigationEventArgs>? NavigationRequested;
}

public class NavigationEventArgs : EventArgs
{
    public Type? ViewModelType { get; set; }
    public string? ViewName { get; set; }
}