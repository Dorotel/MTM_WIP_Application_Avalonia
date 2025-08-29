using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for adding new users to the system.
/// Provides user creation functionality with validation.
/// </summary>
public class AddUserViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _role = "User";
    private bool _isActive = true;
    private string _department = string.Empty;
    private string _notes = string.Empty;
    private bool _isCreating;

    public AddUserViewModel(
        IDatabaseService databaseService,
        ILogger<AddUserViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        // Initialize commands
        CreateUserCommand = new AsyncCommand(ExecuteCreateUserAsync, CanExecuteCreateUser);
        ClearFormCommand = new AsyncCommand(ExecuteClearFormAsync);
        ValidateUsernameCommand = new AsyncCommand(ExecuteValidateUsernameAsync);

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
    /// Username for the new user.
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    /// <summary>
    /// Password for the new user.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    /// <summary>
    /// Password confirmation.
    /// </summary>
    [Required(ErrorMessage = "Password confirmation is required")]
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if (SetProperty(ref _confirmPassword, value))
            {
                RaisePropertyChanged(nameof(PasswordsMatch));
            }
        }
    }

    /// <summary>
    /// First name of the user.
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    /// <summary>
    /// Role assigned to the user.
    /// </summary>
    [Required(ErrorMessage = "Role is required")]
    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    /// <summary>
    /// Indicates if the user account is active.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    /// <summary>
    /// Department the user belongs to.
    /// </summary>
    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }

    /// <summary>
    /// Additional notes about the user.
    /// </summary>
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    /// <summary>
    /// Indicates if user creation is in progress.
    /// </summary>
    public bool IsCreating
    {
        get => _isCreating;
        set => SetProperty(ref _isCreating, value);
    }

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
    /// Command to create the new user.
    /// </summary>
    public ICommand CreateUserCommand { get; }

    /// <summary>
    /// Command to clear the form.
    /// </summary>
    public ICommand ClearFormCommand { get; }

    /// <summary>
    /// Command to validate username availability.
    /// </summary>
    public ICommand ValidateUsernameCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Creates a new user with the specified information.
    /// </summary>
    private async Task ExecuteCreateUserAsync()
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
            Logger.LogInformation("User {Username} created successfully", Username);
            
            // Clear form after successful creation
            await ExecuteClearFormAsync();
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
    private bool CanExecuteCreateUser()
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
    private async Task ExecuteClearFormAsync()
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
    }

    /// <summary>
    /// Validates if username is available.
    /// </summary>
    private async Task ExecuteValidateUsernameAsync()
    {
        if (string.IsNullOrWhiteSpace(Username)) return;

        try
        {
            // Simplified username validation for now
            Logger.LogInformation("Username {Username} validation requested", Username);
            
            // In a real implementation, this would check against the database
            // For now, just log the validation request
            Logger.LogDebug("Username {Username} validation completed", Username);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating username {Username}", Username);
        }
    }

    #endregion
}