using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Interfaces;

namespace MTM_WIP_Application_Avalonia.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool CanGoBack { get; private set; }

    public event EventHandler<NavigationEventArgs>? NavigationRequested;

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        NavigateTo(typeof(TViewModel));
    }

    public void NavigateTo(Type viewModelType)
    {
        _logger.LogInformation("Navigating to {ViewModelType}", viewModelType.Name);
        
        NavigationRequested?.Invoke(this, new NavigationEventArgs
        {
            ViewModelType = viewModelType
        });
    }

    public void NavigateTo(string viewName)
    {
        _logger.LogInformation("Navigating to view {ViewName}", viewName);
        
        NavigationRequested?.Invoke(this, new NavigationEventArgs
        {
            ViewName = viewName
        });
    }

    public void GoBack()
    {
        if (CanGoBack)
        {
            _logger.LogInformation("Navigating back");
            // TODO: Implement back navigation
        }
    }
}