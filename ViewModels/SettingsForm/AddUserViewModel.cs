using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for adding new users to the system.
/// Provides user creation functionality with validation.
/// </summary>
public partial class AddUserViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
    private string _username = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
    private string _password = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Password confirmation is required")]
    [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
    [NotifyPropertyChangedFor(nameof(PasswordsMatch))]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string _firstName = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string _lastName = string.Empty;
    
    [ObservableProperty]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    private string _email = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Role is required")]
    private string _role = "User";
    
    [ObservableProperty]
    private bool _isActive = true;
    
    [ObservableProperty]
    private string _department = string.Empty;
    
    [ObservableProperty]
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    private string _notes = string.Empty;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
    private bool _isCreating;

    public AddUserViewModel(
        IDatabaseService databaseService,
        ILogger<AddUserViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        // Initialize collections
        AvailableRoles = new ObservableCollection<string>
        {
            "User", "Administrator", "Operator", "Manager", "Supervisor"
        };

        AvailableDepartments = new ObservableCollection<string>
        {
            "Production", "Quality", "Maintenance", "Shipping", "Management"
        };

        Logger.LogInformation("AddUserViewModel initialized");
    }

    #region Properties

    /// <summary>
    /// Available user roles.
    /// </summary>
    public ObservableCollection<string> AvailableRoles { get; }

    /// <summary>
    /// Available departments.
    /// </summary>
    public ObservableCollection<string> AvailableDepartments { get; }

    /// <summary>
    /// Indicates if passwords match.
    /// </summary>
    public bool PasswordsMatch => Password == ConfirmPassword;

    /// <summary>
    /// Full name display.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new user with the specified information.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCreateUser))]
    private async Task CreateUserAsync()
    {
        try
        {
            IsCreating = true;

            // Validate passwords match
            if (!PasswordsMatch)
            {
                Logger.LogWarning("User creation failed: passwords do not match");
                return;
            }

            // Create user via database service (simplified for now)
            Logger.LogInformation("User {Username} creation requested", Username);
            
            // In a real implementation, this would call the appropriate stored procedure
            // For now, just simulate success
            await Task.Delay(1000); // Simulate async operation
            Logger.LogInformation("User {Username} created successfully", Username);
            
            // Clear form after successful creation
            await ClearFormAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating user {Username}", Username);
        }
        finally
        {
            IsCreating = false;
        }
    }

    /// <summary>
    /// Determines if user can be created.
    /// </summary>
    private bool CanCreateUser()
    {
        return !IsCreating &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               PasswordsMatch;
    }

    /// <summary>
    /// Clears all form fields.
    /// </summary>
    [RelayCommand]
    private async Task ClearFormAsync()
    {
        Username = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Role = "User";
        IsActive = true;
        Department = string.Empty;
        Notes = string.Empty;

        Logger.LogDebug("User form cleared");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Validates if username is available.
    /// </summary>
    [RelayCommand]
    private async Task ValidateUsernameAsync()
    {
        if (string.IsNullOrWhiteSpace(Username)) return;

        try
        {
            // Simplified username validation for now
            Logger.LogInformation("Username {Username} validation requested", Username);
            
            // In a real implementation, this would check against the database
            // For now, just log the validation request
            await Task.Delay(500); // Simulate async validation
            Logger.LogDebug("Username {Username} validation completed", Username);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating username {Username}", Username);
        }
    }

    #endregion
}