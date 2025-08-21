using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    // TODO: Inject services
    // private readonly INavigationService _navigationService;

    private object? _currentView;
    public object? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainWindowViewModel()
    {
        // Set MainView as the current content
        CurrentView = new Views.MainView
        {
            DataContext = new MainViewViewModel()
        };
    }
}