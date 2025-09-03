using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for user management functionality using MVVM Community Toolkit patterns
/// </summary>
public partial class UserManagementViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;

    [ObservableProperty]
    private string _currentUser = string.Empty;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private ObservableCollection<string> _users = new();

    public UserManagementViewModel(
        IApplicationStateService applicationState,
        ILogger<UserManagementViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("UserManagementViewModel initialized with MVVM Community Toolkit patterns");

        InitializeData();
    }

    private void InitializeData()
    {
        // Initialize with current user if available
        CurrentUser = _applicationState.CurrentUser ?? Environment.UserName;
        
        // Initialize user collection
        Users.Add(CurrentUser);
        
        Logger.LogDebug("UserManagementViewModel initialized with user: {CurrentUser}", CurrentUser);
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        try
        {
            IsLoading = true;
            Logger.LogInformation("Loading users...");

            // TODO: Implement user loading logic via service
            await Task.Delay(1000); // Placeholder for actual user loading

            Logger.LogInformation("Users loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading users");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to load users", CurrentUser);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshUsersAsync()
    {
        Logger.LogInformation("Refreshing user list");
        await LoadUsersAsync();
    }

    [RelayCommand]
    private void SelectUser(string? userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            CurrentUser = userId;
            Logger.LogInformation("User selected: {UserId}", userId);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Users.Clear();
            Logger.LogDebug("UserManagementViewModel disposed");
        }
        base.Dispose(disposing);
    }
}