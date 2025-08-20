# Control_Database - Database Connection Management Interface

## Overview

**Control_Database** is a specialized UserControl within the SettingsForm that provides comprehensive database connection configuration and management capabilities. This control allows administrators to configure MySQL database connection parameters including server address, port, and database name, with real-time validation and connection testing functionality.

## UI Component Structure

### Primary Layout
```
Control_Database (UserControl)
├── Server Configuration
│   ├── Control_Database_Label_Server - "Server Address:"
│   └── Control_Database_TextBox_Server - Server hostname/IP input
├── Port Configuration
│   ├── Control_Database_Label_Port - "Port:"
│   └── Control_Database_TextBox_Port - Port number input
├── Database Configuration
│   ├── Control_Database_Label_Database - "Database Name:"
│   └── Control_Database_TextBox_Database - Database name input
└── Action Buttons
    ├── Control_Database_Button_Save - Save configuration
    ├── Control_Database_Button_Reset - Reset to defaults
    └── Control_Database_Button_Test - Test connection
```

### Input Controls Detail
```
Server Address Input:
├── Field: Control_Database_TextBox_Server
├── Purpose: MySQL server hostname or IP address
├── Validation: Non-empty string validation
├── Default: "localhost"
└── Examples: "localhost", "192.168.1.100", "db.company.com"

Port Number Input:
├── Field: Control_Database_TextBox_Port
├── Purpose: MySQL server port number
├── Validation: Integer 1-65535 range
├── Default: "3306"
└── Format: Numeric string validation

Database Name Input:
├── Field: Control_Database_TextBox_Database
├── Purpose: Target database schema name
├── Validation: Non-empty string validation
├── Default: "mtm_wip_application"
└── Format: Valid MySQL database name
```

## Business Logic Integration

### Initialization and Data Loading
```csharp
protected override async void OnLoad(EventArgs e)
{
    base.OnLoad(e);
    await LoadDatabaseSettingsAsync();
}

private async Task LoadDatabaseSettingsAsync()
{
    try
    {
        string user = Model_AppVariables.User;

        // Load user-specific database settings
        Control_Database_TextBox_Server.Text = 
            await Dao_User.GetWipServerAddressAsync(user) ?? "localhost";
        Control_Database_TextBox_Port.Text = 
            await Dao_User.GetWipServerPortAsync(user) ?? "3306";
        Control_Database_TextBox_Database.Text = 
            await Dao_User.GetDatabaseAsync(user) ?? "mtm_wip_application";

        StatusMessageChanged?.Invoke(this, "Database settings loaded successfully.");
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        StatusMessageChanged?.Invoke(this, $"Error loading database settings: {ex.Message}");

        // Set default values as fallback
        Control_Database_TextBox_Server.Text = "localhost";
        Control_Database_TextBox_Port.Text = "3306";
        Control_Database_TextBox_Database.Text = "mtm_wip_application";
    }
}
```

### Save Operation with Validation
```csharp
private async Task SaveDatabaseSettingsAsync()
{
    try
    {
        // Server address validation
        if (string.IsNullOrWhiteSpace(Control_Database_TextBox_Server.Text))
        {
            StatusMessageChanged?.Invoke(this, "Server address is required.");
            return;
        }

        // Port validation
        if (string.IsNullOrWhiteSpace(Control_Database_TextBox_Port.Text))
        {
            StatusMessageChanged?.Invoke(this, "Port is required.");
            return;
        }

        if (!int.TryParse(Control_Database_TextBox_Port.Text, out int port) || 
            port <= 0 || port > 65535)
        {
            StatusMessageChanged?.Invoke(this, "Port must be a valid number between 1 and 65535.");
            return;
        }

        // Database name validation
        if (string.IsNullOrWhiteSpace(Control_Database_TextBox_Database.Text))
        {
            StatusMessageChanged?.Invoke(this, "Database name is required.");
            return;
        }

        string user = Model_AppVariables.User;

        // Save settings via DAO layer
        await Dao_User.SetWipServerAddressAsync(user, Control_Database_TextBox_Server.Text.Trim());
        await Dao_User.SetDatabaseAsync(user, Control_Database_TextBox_Database.Text.Trim());
        await Dao_User.SetWipServerPortAsync(user, Control_Database_TextBox_Port.Text.Trim());

        DatabaseSettingsUpdated?.Invoke(this, EventArgs.Empty);
        StatusMessageChanged?.Invoke(this,
            "Database settings saved successfully. Restart application for changes to take effect.");
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        StatusMessageChanged?.Invoke(this, $"Error saving database settings: {ex.Message}");
    }
}
```

### Button Event Handlers
```csharp
private async void SaveButton_Click(object? sender, EventArgs e)
{
    try
    {
        Control_Database_Button_Save.Enabled = false;
        await SaveDatabaseSettingsAsync();
    }
    finally
    {
        Control_Database_Button_Save.Enabled = true;
    }
}

private async void ResetButton_Click(object? sender, EventArgs e)
{
    try
    {
        Control_Database_Button_Reset.Enabled = false;
        
        DialogResult result = MessageBox.Show(
            "Are you sure you want to reset database settings to defaults?",
            "Reset Database Settings",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            // Reset to default values
            Control_Database_TextBox_Server.Text = "localhost";
            Control_Database_TextBox_Port.Text = "3306";
            Control_Database_TextBox_Database.Text = "mtm_wip_application";
            
            StatusMessageChanged?.Invoke(this, "Database settings reset to defaults.");
        }
    }
    finally
    {
        Control_Database_Button_Reset.Enabled = true;
    }
}

private async void TestButton_Click(object? sender, EventArgs e)
{
    try
    {
        Control_Database_Button_Test.Enabled = false;
        StatusMessageChanged?.Invoke(this, "Testing database connection...");
        
        await TestDatabaseConnectionAsync();
    }
    finally
    {
        Control_Database_Button_Test.Enabled = true;
    }
}
```

## Database Operations

### User Settings Persistence
```csharp
// Database operations via Dao_User
await Dao_User.SetWipServerAddressAsync(user, serverAddress);
await Dao_User.SetDatabaseAsync(user, databaseName);
await Dao_User.SetWipServerPortAsync(user, portNumber);

// Retrieval operations
string serverAddress = await Dao_User.GetWipServerAddressAsync(user) ?? "localhost";
string port = await Dao_User.GetWipServerPortAsync(user) ?? "3306";
string database = await Dao_User.GetDatabaseAsync(user) ?? "mtm_wip_application";
```

### Connection Testing
```csharp
private async Task TestDatabaseConnectionAsync()
{
    try
    {
        string server = Control_Database_TextBox_Server.Text.Trim();
        string port = Control_Database_TextBox_Port.Text.Trim();
        string database = Control_Database_TextBox_Database.Text.Trim();

        // Validate inputs before testing
        if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(database))
        {
            StatusMessageChanged?.Invoke(this, "Please fill in all fields before testing connection.");
            return;
        }

        if (!int.TryParse(port, out int portNumber) || portNumber <= 0 || portNumber > 65535)
        {
            StatusMessageChanged?.Invoke(this, "Please enter a valid port number.");
            return;
        }

        // Build test connection string
        string testConnectionString = $"Server={server};Port={port};Database={database};Uid=test;Pwd=test;";

        using var connection = new MySqlConnection(testConnectionString);
        
        // Attempt connection with timeout
        var connectTask = connection.OpenAsync();
        var timeoutTask = Task.Delay(5000); // 5 second timeout

        var completedTask = await Task.WhenAny(connectTask, timeoutTask);

        if (completedTask == timeoutTask)
        {
            StatusMessageChanged?.Invoke(this, "Connection test timed out. Please check server address and port.");
            return;
        }

        if (connection.State == ConnectionState.Open)
        {
            StatusMessageChanged?.Invoke(this, "Database connection test successful!");
        }
        else
        {
            StatusMessageChanged?.Invoke(this, "Database connection test failed.");
        }
    }
    catch (MySqlException ex)
    {
        string errorMessage = ex.Number switch
        {
            1042 => "Cannot connect to MySQL server. Please check server address and port.",
            1044 => "Access denied to database. Please check database name and credentials.",
            1049 => "Database does not exist. Please check database name.",
            _ => $"MySQL Error: {ex.Message}"
        };
        
        StatusMessageChanged?.Invoke(this, $"Connection test failed: {errorMessage}");
    }
    catch (Exception ex)
    {
        LoggingUtility.LogApplicationError(ex);
        StatusMessageChanged?.Invoke(this, $"Connection test failed: {ex.Message}");
    }
}
```

## Validation System

### Input Validation Rules
```csharp
private bool ValidateInputs()
{
    // Server address validation
    if (string.IsNullOrWhiteSpace(Control_Database_TextBox_Server.Text))
    {
        ShowValidationError("Server address is required.");
        Control_Database_TextBox_Server.Focus();
        return false;
    }

    // Port validation
    if (string.IsNullOrWhiteSpace(Control_Database_TextBox_Port.Text))
    {
        ShowValidationError("Port is required.");
        Control_Database_TextBox_Port.Focus();
        return false;
    }

    if (!int.TryParse(Control_Database_TextBox_Port.Text, out int port) || 
        port <= 0 || port > 65535)
    {
        ShowValidationError("Port must be a valid number between 1 and 65535.");
        Control_Database_TextBox_Port.SelectAll();
        Control_Database_TextBox_Port.Focus();
        return false;
    }

    // Database name validation
    if (string.IsNullOrWhiteSpace(Control_Database_TextBox_Database.Text))
    {
        ShowValidationError("Database name is required.");
        Control_Database_TextBox_Database.Focus();
        return false;
    }

    // Advanced database name validation
    string databaseName = Control_Database_TextBox_Database.Text.Trim();
    if (databaseName.Length > 64)
    {
        ShowValidationError("Database name cannot exceed 64 characters.");
        Control_Database_TextBox_Database.Focus();
        return false;
    }

    // Check for invalid characters
    if (databaseName.Contains(" ") || databaseName.Contains("-"))
    {
        ShowValidationError("Database name cannot contain spaces or hyphens.");
        Control_Database_TextBox_Database.Focus();
        return false;
    }

    return true;
}

private void ShowValidationError(string message)
{
    StatusMessageChanged?.Invoke(this, message);
    SystemSounds.Exclamation.Play();
}
```

### Real-time Validation
```csharp
private void Control_Database_TextBox_Port_TextChanged(object sender, EventArgs e)
{
    if (sender is TextBox textBox)
    {
        // Only allow numeric input
        string text = textBox.Text;
        string numericText = new string(text.Where(char.IsDigit).ToArray());
        
        if (text != numericText)
        {
            int selectionStart = textBox.SelectionStart;
            textBox.Text = numericText;
            textBox.SelectionStart = Math.Min(selectionStart, numericText.Length);
        }

        // Validate range
        if (int.TryParse(numericText, out int port))
        {
            if (port > 65535)
            {
                textBox.BackColor = Color.LightPink;
                StatusMessageChanged?.Invoke(this, "Port number cannot exceed 65535.");
            }
            else
            {
                textBox.BackColor = SystemColors.Window;
            }
        }
    }
}
```

## Integration Points

### Parent Form Communication
```csharp
public event EventHandler? DatabaseSettingsUpdated;
public event EventHandler<string>? StatusMessageChanged;

// Event propagation to parent SettingsForm
DatabaseSettingsUpdated?.Invoke(this, EventArgs.Empty);
StatusMessageChanged?.Invoke(this, "Database settings updated successfully.");
```

### Theme System Integration
```csharp
public Control_Database()
{
    InitializeComponent();

    // Apply comprehensive DPI scaling and runtime layout adjustments
    Core_Themes.ApplyDpiScaling(this);
    Core_Themes.ApplyRuntimeLayoutAdjustments(this);

    // Wire up button events
    Control_Database_Button_Save.Click += SaveButton_Click;
    Control_Database_Button_Reset.Click += ResetButton_Click;
    Control_Database_Button_Test.Click += TestButton_Click;
}
```

### Model Integration
```csharp
// Integration with application variables
string currentUser = Model_AppVariables.User;

// Connection string integration
string connectionString = Helper_Database_Variables.GetConnectionString();

// Server address detection logic
string serverAddress = Model_Users.WipServerAddress; // Automatically handles environment detection
```

## Security Considerations

### Access Control
- **Admin-Only Access**: Database settings typically restricted to administrative users
- **User-Specific Settings**: Each user can have their own database configuration
- **Credential Protection**: Connection credentials not stored in plain text

### Input Sanitization
```csharp
private string SanitizeInput(string input)
{
    if (string.IsNullOrEmpty(input))
        return string.Empty;

    // Remove potentially dangerous characters
    string sanitized = input.Trim();
    sanitized = sanitized.Replace(";", "");
    sanitized = sanitized.Replace("'", "");
    sanitized = sanitized.Replace("\"", "");
    sanitized = sanitized.Replace("--", "");
    
    return sanitized;
}
```

### Connection Security
- **Timeout Protection**: Connection tests have timeouts to prevent hanging
- **Error Message Sanitization**: Error messages don't expose sensitive information
- **Connection String Validation**: Connection strings validated before use

## Error Handling and Recovery

### Exception Management
```csharp
try
{
    await SaveDatabaseSettingsAsync();
}
catch (MySqlException ex)
{
    LoggingUtility.LogApplicationError(ex);
    StatusMessageChanged?.Invoke(this, GetUserFriendlyMySqlError(ex));
}
catch (Exception ex)
{
    LoggingUtility.LogApplicationError(ex);
    StatusMessageChanged?.Invoke(this, "An unexpected error occurred while saving database settings.");
}

private string GetUserFriendlyMySqlError(MySqlException ex)
{
    return ex.Number switch
    {
        1042 => "Cannot connect to MySQL server. Please check your network connection and server address.",
        1044 => "Access denied. Please verify your database permissions.",
        1049 => "The specified database does not exist.",
        1045 => "Invalid username or password.",
        _ => "Database connection error. Please check your settings and try again."
    };
}
```

### Recovery Mechanisms
- **Default Value Restoration**: Reset functionality restores known good defaults
- **Validation Feedback**: Clear error messages guide users to correct issues
- **Connection Testing**: Test functionality allows verification before saving

This Control_Database provides comprehensive database connection management with robust validation, testing capabilities, and secure configuration storage, ensuring reliable database connectivity for the MTM WIP Application.