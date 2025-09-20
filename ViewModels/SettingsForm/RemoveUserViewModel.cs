using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for removing user accounts from the MTM system.
/// Provides user selection, confirmation, and deletion functionality.
/// </summary>
public partial class RemoveUserViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string? _selectedUserId;

    [ObservableProperty]
    private string _confirmationText = string.Empty;

    [ObservableProperty]
    private bool _requireConfirmation = true;

    /// <summary>
    /// Available users for removal.
    /// </summary>
    public ObservableCollection<UserInfo> AvailableUsers { get; } = new();

    /// <summary>
    /// Gets the selected user details.
    /// </summary>
    public UserInfo? SelectedUser => AvailableUsers.FirstOrDefault(u => u.Id == SelectedUserId);

    public RemoveUserViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<RemoveUserViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("RemoveUserViewModel initialized");
    }

    /// <summary>
    /// Loads available users for removal.
    /// </summary>
    [RelayCommand]
    private Task LoadUsersAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading users...";

            using var scope = Logger.BeginScope("LoadUsers");
            Logger.LogInformation("Loading users for removal");

            // Implementation would load from database
            // var users = await _databaseService.GetUsersAsync().ConfigureAwait(false);
            
            AvailableUsers.Clear();
            // Add sample data for now - exclude system/admin users
            AvailableUsers.Add(new UserInfo { Id = "1", Username = "john.doe", FullName = "John Doe", Email = "john.doe@mtm.com", Role = "User", IsActive = true });
            AvailableUsers.Add(new UserInfo { Id = "2", Username = "jane.smith", FullName = "Jane Smith", Email = "jane.smith@mtm.com", Role = "Supervisor", IsActive = false });
            AvailableUsers.Add(new UserInfo { Id = "3", Username = "bob.johnson", FullName = "Bob Johnson", Email = "bob.johnson@mtm.com", Role = "User", IsActive = true });

            StatusMessage = $"Loaded {AvailableUsers.Count} users";
            Logger.LogInformation("Successfully loaded {UserCount} users for removal", AvailableUsers.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading users: {ex.Message}";
            Logger.LogError(ex, "Error loading users for removal");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes the selected user from the system.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemoveUser))]
    private Task RemoveUserAsync()
    {
        if (SelectedUser == null) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Removing user...";

            using var scope = Logger.BeginScope("RemoveUser");
            Logger.LogInformation("Removing user {Username} (ID: {UserId})", SelectedUser.Username, SelectedUser.Id);

            // Validate confirmation if required
            if (RequireConfirmation && ConfirmationText != SelectedUser.Username)
            {
                StatusMessage = "Please type the username to confirm deletion";
                return Task.CompletedTask;
            }

            // Implementation would remove from database
            // var result = await _databaseService.RemoveUserAsync(SelectedUser.Id).ConfigureAwait(false);

            // Remove from local collection
            AvailableUsers.Remove(SelectedUser);
            
            // Clear selection
            SelectedUserId = null;
            ConfirmationText = string.Empty;

            StatusMessage = $"User '{SelectedUser.Username}' removed successfully";
            Logger.LogInformation("Successfully removed user {Username}", SelectedUser.Username);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error removing user: {ex.Message}";
            Logger.LogError(ex, "Error removing user {Username}", SelectedUser?.Username);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if user can be removed.
    /// </summary>
    private bool CanRemoveUser()
    {
        if (IsLoading || SelectedUser == null) return false;
        
        // Additional validation - prevent removing admin users
        if (SelectedUser.Role == "Administrator") return false;
        
        // If confirmation is required, check that username matches
        if (RequireConfirmation && ConfirmationText != SelectedUser.Username) return false;

        return true;
    }

    /// <summary>
    /// Deactivates the selected user instead of removing.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDeactivateUser))]
    private Task DeactivateUserAsync()
    {
        if (SelectedUser == null) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Deactivating user...";

            using var scope = Logger.BeginScope("DeactivateUser");
            Logger.LogInformation("Deactivating user {Username} (ID: {UserId})", SelectedUser.Username, SelectedUser.Id);

            // Implementation would update in database
            // var result = await _databaseService.DeactivateUserAsync(SelectedUser.Id).ConfigureAwait(false);

            // Update local collection
            SelectedUser.IsActive = false;

            StatusMessage = $"User '{SelectedUser.Username}' deactivated successfully";
            Logger.LogInformation("Successfully deactivated user {Username}", SelectedUser.Username);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deactivating user: {ex.Message}";
            Logger.LogError(ex, "Error deactivating user {Username}", SelectedUser?.Username);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if user can be deactivated.
    /// </summary>
    private bool CanDeactivateUser()
    {
        return !IsLoading && SelectedUser != null && SelectedUser.IsActive && SelectedUser.Role != "Administrator";
    }

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        SelectedUserId = null;
        ConfirmationText = string.Empty;
        StatusMessage = "Form reset";

        Logger.LogInformation("User removal form reset");
        return Task.CompletedTask;
    }

    partial void OnSelectedUserIdChanged(string? value)
    {
        // Clear confirmation when selection changes
        ConfirmationText = string.Empty;
        
        // Update command states
        RemoveUserCommand.NotifyCanExecuteChanged();
        DeactivateUserCommand.NotifyCanExecuteChanged();
        
        // Update property change for SelectedUser
        OnPropertyChanged(nameof(SelectedUser));
    }

    partial void OnConfirmationTextChanged(string value)
    {
        // Update command state when confirmation text changes
        RemoveUserCommand.NotifyCanExecuteChanged();
    }
}
