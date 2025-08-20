# Control_Add_User - User Creation Interface

## Overview

**Control_Add_User** is a specialized UserControl within the SettingsForm that provides comprehensive user account creation functionality for the MTM WIP Application. This control handles the complete user registration process including personal information, authentication credentials, user type assignment, and database storage with full validation and security measures.

## UI Component Structure

### Primary Layout
```
Control_Add_User (UserControl)
├── Personal Information Section
│   ├── Control_Add_User_Label_FirstName - "First Name:"
│   ├── Control_Add_User_TextBox_FirstName - First name input
│   ├── Control_Add_User_Label_LastName - "Last Name:"
│   └── Control_Add_User_TextBox_LastName - Last name input
├── Authentication Section
│   ├── Control_Add_User_Label_UserName - "Username:"
│   ├── Control_Add_User_TextBox_UserName - Username input
│   ├── Control_Add_User_Label_Pin - "PIN:"
│   ├── Control_Add_User_TextBox_Pin - PIN input (masked)
│   ├── Control_Add_User_Label_VisualUserName - "Visual Username:"
│   ├── Control_Add_User_TextBox_VisualUserName - Display name input
│   ├── Control_Add_User_Label_VisualPassword - "Visual Password:"
│   └── Control_Add_User_TextBox_VisualPassword - Visual password input (masked)
├── User Type Selection
│   ├── Control_Add_User_RadioButton_NormalUser - "Normal User"
│   ├── Control_Add_User_RadioButton_PowerUser - "Power User"
│   └── Control_Add_User_RadioButton_Admin - "Administrator"
└── Action Buttons
    ├── Control_Add_User_Button_Add - "Add User"
    ├── Control_Add_User_Button_Clear - "Clear Form"
    └── Control_Add_User_Button_Cancel - "Cancel"
```

### Input Field Specifications
```
Personal Information:
├── First Name: Required, no spaces, alphanumeric
├── Last Name: Required, no spaces, alphanumeric
└── Purpose: User identification and display

Authentication Credentials:
├── Username: Unique identifier, no spaces, required
├── PIN: Numeric security code, masked input, required
├── Visual Username: Display name for UI, can contain spaces
└── Visual Password: Secondary authentication, masked input

User Type Selection:
├── Normal User: Standard user privileges (default)
├── Power User: Enhanced privileges for advanced operations
└── Administrator: Full system access and user management
```

## Business Logic Integration

### Initialization and Setup
```csharp
public Control_Add_User()
{
    Service_DebugTracer.TraceMethodEntry(new Dictionary<string, object>
    {
        ["ControlType"] = nameof(Control_Add_User),
        ["InitializationTime"] = DateTime.Now,
        ["Thread"] = Thread.CurrentThread.ManagedThreadId
    }, nameof(Control_Add_User), nameof(Control_Add_User));

    InitializeComponent();

    // Apply theme and DPI scaling
    Core_Themes.ApplyDpiScaling(this);
    Core_Themes.ApplyRuntimeLayoutAdjustments(this);

    // Set default user type
    Control_Add_User_RadioButton_NormalUser.Checked = true;

    // Configure input validation
    SetupInputValidation();
    SetupPasswordFields();

    Service_DebugTracer.TraceUIAction("ADD_USER_CONTROL_INITIALIZATION", nameof(Control_Add_User),
        new Dictionary<string, object>
        {
            ["Phase"] = "COMPLETE",
            ["Success"] = true
        });
}
```

### Input Validation Setup
```csharp
private void SetupInputValidation()
{
    // No spaces allowed in core authentication fields
    Control_Add_User_TextBox_FirstName.KeyPress += Control_Add_User_TextBox_NoSpaces_KeyPress;
    Control_Add_User_TextBox_LastName.KeyPress += Control_Add_User_TextBox_NoSpaces_KeyPress;
    Control_Add_User_TextBox_UserName.KeyPress += Control_Add_User_TextBox_NoSpaces_KeyPress;
    Control_Add_User_TextBox_Pin.KeyPress += Control_Add_User_TextBox_NoSpaces_KeyPress;
    Control_Add_User_TextBox_VisualUserName.KeyPress += Control_Add_User_TextBox_NoSpaces_KeyPress;
    Control_Add_User_TextBox_VisualPassword.KeyPress += Control_Add_User_TextBox_NoSpaces_KeyPress;

    // PIN field - numeric only
    Control_Add_User_TextBox_Pin.KeyPress += Control_Add_User_TextBox_Pin_KeyPress;

    // Real-time validation events
    Control_Add_User_TextBox_UserName.TextChanged += ValidateUsername;
    Control_Add_User_TextBox_Pin.TextChanged += ValidatePin;
}

private void Control_Add_User_TextBox_NoSpaces_KeyPress(object? sender, KeyPressEventArgs e)
{
    // Prevent spaces in authentication fields
    if (e.KeyChar == ' ')
    {
        e.Handled = true;
        StatusMessageChanged?.Invoke(this, "Spaces are not allowed in this field.");
    }
}

private void Control_Add_User_TextBox_Pin_KeyPress(object? sender, KeyPressEventArgs e)
{
    // Only allow numeric input for PIN
    if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
    {
        e.Handled = true;
        StatusMessageChanged?.Invoke(this, "PIN must contain only numbers.");
    }
}
```

### Password Field Configuration
```csharp
private void SetupPasswordFields()
{
    // Configure password masking
    Control_Add_User_TextBox_Pin.UseSystemPasswordChar = true;
    Control_Add_User_TextBox_VisualPassword.UseSystemPasswordChar = true;

    // Set maximum lengths
    Control_Add_User_TextBox_Pin.MaxLength = 6;
    Control_Add_User_TextBox_VisualPassword.MaxLength = 20;
    Control_Add_User_TextBox_UserName.MaxLength = 50;
    Control_Add_User_TextBox_FirstName.MaxLength = 50;
    Control_Add_User_TextBox_LastName.MaxLength = 50;
    Control_Add_User_TextBox_VisualUserName.MaxLength = 100;
}
```

## User Creation Process

### Add User Button Handler
```csharp
private async void Control_Add_User_Button_Add_ClickAsync(object? sender, EventArgs e)
{
    try
    {
        _progressHelper?.ShowProgress("Adding user...");
        Control_Add_User_Button_Add.Enabled = false;

        // Validate all inputs
        if (!ValidateAllInputs())
        {
            return;
        }

        // Check for duplicate username
        if (await CheckUserExistsAsync(Control_Add_User_TextBox_UserName.Text.Trim()))
        {
            StatusMessageChanged?.Invoke(this, "Username already exists. Please choose a different username.");
            Control_Add_User_TextBox_UserName.Focus();
            return;
        }

        // Create user model
        var newUser = CreateUserModel();

        // Add user to database
        bool success = await AddUserToDatabaseAsync(newUser);

        if (success)
        {
            StatusMessageChanged?.Invoke(this, "User added successfully.");
            UserAdded?.Invoke(this, EventArgs.Empty);
            ClearForm();
        }
        else
        {
            StatusMessageChanged?.Invoke(this, "Failed to add user. Please try again.");
        }
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        Service_ErrorHandler.HandleException(ex, ErrorSeverity.Medium,
            controlName: "Control_Add_User",
            additionalData: new Dictionary<string, object>
            {
                ["Operation"] = "AddUser",
                ["Username"] = Control_Add_User_TextBox_UserName.Text
            });
        StatusMessageChanged?.Invoke(this, "An error occurred while adding the user.");
    }
    finally
    {
        _progressHelper?.HideProgress();
        Control_Add_User_Button_Add.Enabled = true;
    }
}
```

### Input Validation System
```csharp
private bool ValidateAllInputs()
{
    // First Name validation
    if (string.IsNullOrWhiteSpace(Control_Add_User_TextBox_FirstName.Text))
    {
        ShowValidationError("First name is required.", Control_Add_User_TextBox_FirstName);
        return false;
    }

    // Last Name validation
    if (string.IsNullOrWhiteSpace(Control_Add_User_TextBox_LastName.Text))
    {
        ShowValidationError("Last name is required.", Control_Add_User_TextBox_LastName);
        return false;
    }

    // Username validation
    if (string.IsNullOrWhiteSpace(Control_Add_User_TextBox_UserName.Text))
    {
        ShowValidationError("Username is required.", Control_Add_User_TextBox_UserName);
        return false;
    }

    if (Control_Add_User_TextBox_UserName.Text.Length < 3)
    {
        ShowValidationError("Username must be at least 3 characters long.", Control_Add_User_TextBox_UserName);
        return false;
    }

    // PIN validation
    if (string.IsNullOrWhiteSpace(Control_Add_User_TextBox_Pin.Text))
    {
        ShowValidationError("PIN is required.", Control_Add_User_TextBox_Pin);
        return false;
    }

    if (Control_Add_User_TextBox_Pin.Text.Length < 4)
    {
        ShowValidationError("PIN must be at least 4 digits long.", Control_Add_User_TextBox_Pin);
        return false;
    }

    // Visual Username validation
    if (string.IsNullOrWhiteSpace(Control_Add_User_TextBox_VisualUserName.Text))
    {
        ShowValidationError("Visual username is required.", Control_Add_User_TextBox_VisualUserName);
        return false;
    }

    // Visual Password validation
    if (string.IsNullOrWhiteSpace(Control_Add_User_TextBox_VisualPassword.Text))
    {
        ShowValidationError("Visual password is required.", Control_Add_User_TextBox_VisualPassword);
        return false;
    }

    // User type validation
    if (!Control_Add_User_RadioButton_NormalUser.Checked && 
        !Control_Add_User_RadioButton_PowerUser.Checked && 
        !Control_Add_User_RadioButton_Admin.Checked)
    {
        ShowValidationError("Please select a user type.", null);
        return false;
    }

    return true;
}

private void ShowValidationError(string message, Control? controlToFocus)
{
    StatusMessageChanged?.Invoke(this, message);
    controlToFocus?.Focus();
    SystemSounds.Exclamation.Play();
}
```

### User Model Creation
```csharp
private Model_User CreateUserModel()
{
    string userType = GetSelectedUserType();
    
    return new Model_User
    {
        FirstName = Control_Add_User_TextBox_FirstName.Text.Trim(),
        LastName = Control_Add_User_TextBox_LastName.Text.Trim(),
        UserName = Control_Add_User_TextBox_UserName.Text.Trim(),
        Pin = Control_Add_User_TextBox_Pin.Text.Trim(),
        VisualUserName = Control_Add_User_TextBox_VisualUserName.Text.Trim(),
        VisualPassword = Control_Add_User_TextBox_VisualPassword.Text.Trim(),
        UserType = userType,
        IsActive = true,
        CreatedDate = DateTime.Now,
        CreatedBy = Model_AppVariables.User,
        LastModifiedDate = DateTime.Now,
        LastModifiedBy = Model_AppVariables.User
    };
}

private string GetSelectedUserType()
{
    if (Control_Add_User_RadioButton_Admin.Checked)
        return "Admin";
    if (Control_Add_User_RadioButton_PowerUser.Checked)
        return "PowerUser";
    return "NormalUser";
}
```

## Database Operations

### User Creation Database Operation
```csharp
private async Task<bool> AddUserToDatabaseAsync(Model_User user)
{
    try
    {
        _progressHelper?.UpdateProgress(25, "Creating user account...");

        // Hash passwords before storage
        string hashedPin = Security_PasswordHelper.HashPassword(user.Pin);
        string hashedVisualPassword = Security_PasswordHelper.HashPassword(user.VisualPassword);

        _progressHelper?.UpdateProgress(50, "Saving to database...");

        // Use DAO layer for database operation
        bool success = await Dao_Users.AddUserAsync(new Model_User
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            Pin = hashedPin,
            VisualUserName = user.VisualUserName,
            VisualPassword = hashedVisualPassword,
            UserType = user.UserType,
            IsActive = user.IsActive,
            CreatedDate = user.CreatedDate,
            CreatedBy = user.CreatedBy,
            LastModifiedDate = user.LastModifiedDate,
            LastModifiedBy = user.LastModifiedBy
        });

        _progressHelper?.UpdateProgress(75, "Updating user permissions...");

        if (success)
        {
            // Set default permissions based on user type
            await SetDefaultUserPermissionsAsync(user.UserName, user.UserType);
            
            _progressHelper?.UpdateProgress(100, "User created successfully!");
            
            // Log user creation
            await Dao_AuditLog.LogUserActionAsync(
                Model_AppVariables.User,
                "USER_CREATED",
                $"Created new user: {user.UserName} ({user.UserType})",
                user.UserName
            );
        }

        return success;
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        throw;
    }
}

private async Task<bool> CheckUserExistsAsync(string username)
{
    try
    {
        var existingUser = await Dao_Users.GetUserByUsernameAsync(username);
        return existingUser != null;
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        return false; // Assume user doesn't exist if check fails
    }
}

private async Task SetDefaultUserPermissionsAsync(string username, string userType)
{
    try
    {
        var defaultPermissions = GetDefaultPermissionsByUserType(userType);
        await Dao_UserPermissions.SetUserPermissionsAsync(username, defaultPermissions);
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        // Don't fail user creation if permission setting fails
    }
}
```

### Permission System Integration
```csharp
private List<string> GetDefaultPermissionsByUserType(string userType)
{
    return userType switch
    {
        "Admin" => new List<string>
        {
            "USER_MANAGEMENT",
            "INVENTORY_ADD",
            "INVENTORY_REMOVE",
            "INVENTORY_TRANSFER",
            "INVENTORY_VIEW",
            "SETTINGS_ACCESS",
            "REPORTS_ACCESS",
            "AUDIT_LOG_ACCESS"
        },
        "PowerUser" => new List<string>
        {
            "INVENTORY_ADD",
            "INVENTORY_REMOVE",
            "INVENTORY_TRANSFER",
            "INVENTORY_VIEW",
            "REPORTS_ACCESS"
        },
        "NormalUser" => new List<string>
        {
            "INVENTORY_ADD",
            "INVENTORY_VIEW"
        },
        _ => new List<string> { "INVENTORY_VIEW" }
    };
}
```

## Form Management

### Clear Form Functionality
```csharp
private void ClearForm()
{
    Control_Add_User_TextBox_FirstName.Clear();
    Control_Add_User_TextBox_LastName.Clear();
    Control_Add_User_TextBox_UserName.Clear();
    Control_Add_User_TextBox_Pin.Clear();
    Control_Add_User_TextBox_VisualUserName.Clear();
    Control_Add_User_TextBox_VisualPassword.Clear();
    
    Control_Add_User_RadioButton_NormalUser.Checked = true;
    Control_Add_User_RadioButton_PowerUser.Checked = false;
    Control_Add_User_RadioButton_Admin.Checked = false;
    
    Control_Add_User_TextBox_FirstName.Focus();
    
    StatusMessageChanged?.Invoke(this, "Form cleared successfully.");
}

private void Control_Add_User_Button_Clear_Click(object? sender, EventArgs e)
{
    DialogResult result = MessageBox.Show(
        "Are you sure you want to clear all fields?",
        "Clear Form",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

    if (result == DialogResult.Yes)
    {
        ClearForm();
    }
}
```

## Integration Points

### Progress System Integration
```csharp
public void SetProgressControls(Helper_StoredProcedureProgress progressHelper)
{
    _progressHelper = progressHelper;
}
```

### Parent Form Communication
```csharp
public event EventHandler? UserAdded;
public event EventHandler<string>? StatusMessageChanged;

// Event firing
UserAdded?.Invoke(this, EventArgs.Empty);
StatusMessageChanged?.Invoke(this, "User added successfully.");
```

### Security Integration
```csharp
// Password hashing
string hashedPin = Security_PasswordHelper.HashPassword(user.Pin);
string hashedVisualPassword = Security_PasswordHelper.HashPassword(user.VisualPassword);

// Audit logging
await Dao_AuditLog.LogUserActionAsync(
    Model_AppVariables.User,
    "USER_CREATED",
    $"Created new user: {user.UserName} ({user.UserType})",
    user.UserName
);
```

## Security Considerations

### Password Security
- **Password Hashing**: All passwords hashed using Security_PasswordHelper before storage
- **Masked Input**: PIN and Visual Password fields use masked input
- **Length Requirements**: Minimum length requirements enforced
- **No Plaintext Storage**: Passwords never stored in plaintext

### Input Validation Security
- **SQL Injection Prevention**: Parameterized queries used in DAO layer
- **Character Restrictions**: Spaces and special characters restricted in authentication fields
- **Length Limits**: Maximum field lengths enforced to prevent buffer overflows
- **Real-time Validation**: Immediate feedback prevents invalid data entry

### Access Control
- **Admin-Only Operation**: User creation typically restricted to administrators
- **Audit Trail**: All user creation operations logged for security auditing
- **Permission Assignment**: Default permissions assigned based on user type
- **Duplicate Prevention**: Username uniqueness enforced

This Control_Add_User provides comprehensive user account creation functionality with robust validation, security measures, and integration with the MTM WIP Application's permission and audit systems.