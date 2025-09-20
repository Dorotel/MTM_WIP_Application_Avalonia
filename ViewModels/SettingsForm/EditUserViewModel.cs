using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
/// ViewModel for editing existing user accounts in the MTM system.
/// Provides user selection, modification, and update functionality.
/// </summary>
public partial class EditUserViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    private string _username = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    private string _fullName = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _role = "User";

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string? _selectedUserId;

    /// <summary>
    /// Available users for editing.
    /// </summary>
    public ObservableCollection<UserInfo> AvailableUsers { get; } = new();

    /// <summary>
    /// Available roles for assignment.
    /// </summary>
    public ObservableCollection<string> AvailableRoles { get; } = new()
    {
        "User", "Supervisor", "Manager", "Administrator"
    };

    public EditUserViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<EditUserViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        Logger.LogInformation("EditUserViewModel initialized");
    }

    /// <summary>
    /// Loads available users for editing.
    /// </summary>
    [RelayCommand]
    private Task LoadUsersAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading users...";

            using var scope = Logger.BeginScope("LoadUsers");
            Logger.LogInformation("Loading users for editing");

            // Implementation would load from database
            // var users = await _databaseService.GetUsersAsync().ConfigureAwait(false);
            
            AvailableUsers.Clear();
            // Add sample data for now
            AvailableUsers.Add(new UserInfo { Id = "1", Username = "john.doe", FullName = "John Doe", Email = "john.doe@mtm.com", Role = "User", IsActive = true });
            AvailableUsers.Add(new UserInfo { Id = "2", Username = "jane.smith", FullName = "Jane Smith", Email = "jane.smith@mtm.com", Role = "Supervisor", IsActive = true });

            StatusMessage = $"Loaded {AvailableUsers.Count} users";
            Logger.LogInformation("Successfully loaded {UserCount} users", AvailableUsers.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading users: {ex.Message}";
            Logger.LogError(ex, "Error loading users for editing");
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads selected user details for editing.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanLoadUserDetails))]
    private Task LoadUserDetailsAsync()
    {
        if (string.IsNullOrEmpty(SelectedUserId)) return Task.CompletedTask;

        try
        {
            IsLoading = true;
            StatusMessage = "Loading user details...";

            using var scope = Logger.BeginScope("LoadUserDetails");
            Logger.LogInformation("Loading details for user {UserId}", SelectedUserId);

            var selectedUser = AvailableUsers.FirstOrDefault(u => u.Id == SelectedUserId);
            if (selectedUser != null)
            {
                Username = selectedUser.Username;
                FullName = selectedUser.FullName;
                Email = selectedUser.Email;
                Role = selectedUser.Role;
                IsActive = selectedUser.IsActive;

                StatusMessage = "User details loaded";
                Logger.LogInformation("Successfully loaded details for user {Username}", Username);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading user details: {ex.Message}";
            Logger.LogError(ex, "Error loading user details for {UserId}", SelectedUserId);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if user details can be loaded.
    /// </summary>
    private bool CanLoadUserDetails() => !string.IsNullOrEmpty(SelectedUserId) && !IsLoading;

    /// <summary>
    /// Updates the selected user with modified information.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanUpdateUser))]
    private Task UpdateUserAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Updating user...";

            using var scope = Logger.BeginScope("UpdateUser");
            Logger.LogInformation("Updating user {Username}", Username);

            // Validate input
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
            {
                StatusMessage = "Please fill in all required fields";
                return Task.CompletedTask;
            }

            // Implementation would update in database
            // var result = await _databaseService.UpdateUserAsync(SelectedUserId, Username, FullName, Email, Role, IsActive).ConfigureAwait(false);

            // Update local collection
            var user = AvailableUsers.FirstOrDefault(u => u.Id == SelectedUserId);
            if (user != null)
            {
                user.Username = Username;
                user.FullName = FullName;
                user.Email = Email;
                user.Role = Role;
                user.IsActive = IsActive;
            }

            StatusMessage = "User updated successfully";
            Logger.LogInformation("Successfully updated user {Username}", Username);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error updating user: {ex.Message}";
            Logger.LogError(ex, "Error updating user {Username}", Username);
        }
        finally
        {
            IsLoading = false;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if user can be updated.
    /// </summary>
    private bool CanUpdateUser() => !string.IsNullOrEmpty(SelectedUserId) && 
                                   !string.IsNullOrWhiteSpace(Username) && 
                                   !string.IsNullOrWhiteSpace(FullName) && 
                                   !string.IsNullOrWhiteSpace(Email) && 
                                   !IsLoading;

    /// <summary>
    /// Resets the form to default state.
    /// </summary>
    [RelayCommand]
    private Task ResetFormAsync()
    {
        Username = string.Empty;
        FullName = string.Empty;
        Email = string.Empty;
        Role = "User";
        IsActive = true;
        SelectedUserId = null;
        StatusMessage = "Form reset";

        Logger.LogInformation("User edit form reset");
        return Task.CompletedTask;
    }
}

/// <summary>
/// User information model for display and editing.
/// </summary>
public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
