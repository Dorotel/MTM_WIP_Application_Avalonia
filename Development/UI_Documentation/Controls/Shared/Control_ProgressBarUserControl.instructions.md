# Control_ProgressBarUserControl - Universal Progress Feedback Interface

## Overview

**Control_ProgressBarUserControl** is a specialized UserControl that provides comprehensive progress feedback capabilities throughout the MTM WIP Application. This control combines a visual progress bar, animated loading indicator, status text display, and optional cancellation functionality to create a unified progress reporting system used by forms like SplashScreenForm and any operation requiring user feedback on time-consuming tasks.

## UI Component Structure

### Primary Layout
```
Control_ProgressBarUserControl (Dynamic sizing)
├── _loadingImage (PictureBox) - Animated loading indicator
│   ├── Size: 48x48 pixels
│   ├── Position: Top center
│   ├── Style: Animated arc drawing
│   └── Color: Model_AppVariables.UserUiColors.ProgressBarForeColor
├── _progressBar (ProgressBar) - Progress percentage display
│   ├── Style: ProgressBarStyle.Continuous
│   ├── Range: 0-100
│   ├── Height: 23 pixels
│   └── Anchor: Left, Right, Top
├── _statusLabel (Label) - Status text display
│   ├── Text: Dynamic status messages
│   ├── Alignment: MiddleCenter
│   ├── Height: 20 pixels
│   └── Anchor: Left, Right, Top
└── _cancelButton (Button, Optional) - Cancellation control
    ├── Text: "Cancel"
    ├── Size: 80x30 pixels
    ├── Position: Bottom right
    └── Visibility: Controlled by EnableCancel()
```

### Dynamic Layout System
```csharp
private void LayoutControls()
{
    int spacing = 8;
    int currentY = spacing;

    // Loading image positioning (top center)
    if (_loadingImage != null)
    {
        _loadingImage.Location = new Point((Width - _loadingImage.Width) / 2, currentY);
        currentY += _loadingImage.Height + spacing;
    }

    // Progress bar positioning (full width with margins)
    if (_progressBar != null)
    {
        _progressBar.Location = new Point(spacing, currentY);
        _progressBar.Width = Width - spacing * 2;
        currentY += _progressBar.Height + spacing;
    }

    // Status label positioning (full width with margins)
    if (_statusLabel != null)
    {
        _statusLabel.Location = new Point(spacing, currentY);
        _statusLabel.Width = Width - spacing * 2;
    }

    // Auto-size control based on content
    Height = currentY + (_statusLabel?.Height ?? 0) + spacing;
}
```

## Business Logic Integration

### Properties and Data Binding
```csharp
[DefaultValue(0)]
public int ProgressValue
{
    get => _progressBar?.Value ?? 0;
    set
    {
        if (value >= 0 && value <= 100 && _progressBar != null)
        {
            _progressBar.Value = value;
            UpdateStatusText();
        }
    }
}

[DefaultValue("Loading...")]
public string StatusText
{
    get => _statusLabel?.Text ?? string.Empty;
    set
    {
        if (_statusLabel != null)
        {
            _statusLabel.Text = value;
        }
    }
}

[DefaultValue(true)]
public bool ShowLoadingImage
{
    get => _loadingImage?.Visible ?? false;
    set
    {
        if (_loadingImage != null)
        {
            _loadingImage.Visible = value;
        }
    }
}
```

### Progress Management Methods
```csharp
public void ShowProgress()
{
    if (InvokeRequired)
    {
        Invoke(new Action(ShowProgress));
        return;
    }

    Visible = true;
    ProgressValue = 0;
    StatusText = "Loading...";

    // Start animation timer for loading indicator
    if (_loadingImage != null)
    {
        Timer timer = new() { Interval = 50 };
        timer.Tick += (s, e) => _loadingImage.Invalidate();
        timer.Start();
        Tag = timer; // Store for cleanup
    }
}

public void HideProgress()
{
    if (InvokeRequired)
    {
        Invoke(new Action(HideProgress));
        return;
    }

    // Stop and dispose animation timer
    if (Tag is Timer timer)
    {
        timer.Stop();
        timer.Dispose();
        Tag = null;
    }

    Visible = false;
}

public void UpdateProgress(int value, string? status = null)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateProgress(value, status)));
        return;
    }

    ProgressValue = value;
    if (!string.IsNullOrEmpty(status))
    {
        StatusText = status;
    }
}
```

### Async Completion with Animation
```csharp
public async Task CompleteProgressAsync()
{
    // Smooth completion animation
    for (int i = ProgressValue; i <= 100; i += 2)
    {
        ProgressValue = i;
        await Task.Delay(20);
    }
    
    StatusText = "Complete!";
    await Task.Delay(500); // Brief pause before hiding
    HideProgress();
}
```

## Animation System

### Custom Loading Animation
```csharp
private void LoadingImage_Paint(object? sender, PaintEventArgs e)
{
    if (_loadingImage == null) return;

    Graphics g = e.Graphics;
    Rectangle rect = _loadingImage.ClientRectangle;
    Point center = new(rect.Width / 2, rect.Height / 2);
    int radius = Math.Min(rect.Width, rect.Height) / 2 - 4;

    // Use theme colors for consistency
    using Pen pen = new(Model_AppVariables.UserUiColors?.ProgressBarForeColor ?? Color.Blue, 3);

    // Rotating arc animation
    int startAngle = Environment.TickCount / 10 % 360;
    g.DrawArc(pen, center.X - radius, center.Y - radius, radius * 2, radius * 2, startAngle, 270);
}
```

### Animation Timer Management
```csharp
// Animation timer in ShowProgress()
Timer timer = new() { Interval = 50 };
timer.Tick += (s, e) => _loadingImage.Invalidate();
timer.Start();
Tag = timer; // Store reference for cleanup

// Timer cleanup in HideProgress()
if (Tag is Timer timer)
{
    timer.Stop();
    timer.Dispose();
    Tag = null;
}
```

## Cancellation Support

### Optional Cancel Button
```csharp
private Button? _cancelButton;
public event Action? CancelRequested;

public void EnableCancel(bool enable)
{
    if (_cancelButton == null)
    {
        _cancelButton = new Button
        {
            Text = "Cancel",
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Size = new Size(80, 30),
            Visible = enable
        };
        _cancelButton.Click += (s, e) => CancelRequested?.Invoke();
        Controls.Add(_cancelButton);
        _cancelButton.BringToFront();
        _cancelButton.Location = new Point(Width - _cancelButton.Width - 10, Height - _cancelButton.Height - 10);
    }

    _cancelButton.Visible = enable;
}
```

### Usage Pattern with Cancellation
```csharp
// In calling code
progressControl.EnableCancel(true);
progressControl.CancelRequested += () => {
    cancellationToken.Cancel();
    progressControl.HideProgress();
};

// Long-running operation with cancellation support
await LongRunningOperationAsync(cancellationToken, (progress, status) => {
    progressControl.UpdateProgress(progress, status);
});
```

## Status Text Management

### Automatic Status Updates
```csharp
private void UpdateStatusText()
{
    if (_progressBar == null) return;

    if (_progressBar.Value == 0)
    {
        StatusText = "Initializing...";
    }
    else if (_progressBar.Value == 100)
    {
        StatusText = "Complete";
    }
    else
    {
        StatusText = $"Loading... {_progressBar.Value}%";
    }
}
```

### Custom Status Messages
```csharp
// Manual status setting
progressControl.UpdateProgress(25, "Connecting to database...");
progressControl.UpdateProgress(50, "Loading user settings...");
progressControl.UpdateProgress(75, "Initializing UI components...");
progressControl.UpdateProgress(100, "Ready!");
```

## Thread Safety

### Cross-Thread Operation Safety
```csharp
public void UpdateProgress(int value, string? status = null)
{
    if (InvokeRequired)
    {
        Invoke(new Action(() => UpdateProgress(value, status)));
        return;
    }
    
    // Safe to update UI from UI thread
    ProgressValue = value;
    if (!string.IsNullOrEmpty(status))
    {
        StatusText = status;
    }
}

public void ShowProgress()
{
    if (InvokeRequired)
    {
        Invoke(new Action(ShowProgress));
        return;
    }
    
    // UI thread operations
    Visible = true;
    // ... rest of implementation
}
```

### Background Thread Usage
```csharp
// Example from calling code
Task.Run(async () => {
    for (int i = 0; i <= 100; i += 10)
    {
        // Thread-safe update from background thread
        progressControl.UpdateProgress(i, $"Processing step {i/10 + 1}/10...");
        await Task.Delay(500);
    }
    
    await progressControl.CompleteProgressAsync();
});
```

## Theme Integration

### Color Scheme Support
```csharp
private void ApplyTheme()
{
    // Apply theme colors to all components
    if (_progressBar != null && Model_AppVariables.UserUiColors != null)
    {
        _progressBar.ForeColor = Model_AppVariables.UserUiColors.ProgressBarForeColor ?? _progressBar.ForeColor;
        _progressBar.BackColor = Model_AppVariables.UserUiColors.ProgressBarBackColor ?? _progressBar.BackColor;
    }

    if (_statusLabel != null)
    {
        _statusLabel.ForeColor = Model_AppVariables.UserUiColors?.LabelForeColor ?? _statusLabel.ForeColor;
        _statusLabel.BackColor = Model_AppVariables.UserUiColors?.FormBackColor ?? _statusLabel.BackColor;
    }

    // Loading animation uses theme colors
    using Pen pen = new(Model_AppVariables.UserUiColors?.ProgressBarForeColor ?? Color.Blue, 3);
}
```

### DPI Scaling Support
```csharp
public Control_ProgressBarUserControl()
{
    InitializeComponent();
    InitializeControls();
    ApplyTheme();
    
    // Apply DPI scaling
    Core_Themes.ApplyDpiScaling(this);
    Core_Themes.ApplyRuntimeLayoutAdjustments(this);
}
```

## Integration Examples

### SplashScreenForm Integration
```csharp
// In SplashScreenForm
private Controls.Shared.Control_ProgressBarUserControl? _progressControl;

public void UpdateProgress(int progress, string status)
{
    _progressControl!.UpdateProgress(progress, status);
    Application.DoEvents();
}

public async Task CompleteSplashAsync()
{
    await _progressControl!.CompleteProgressAsync();
    Close();
}
```

### Database Operation Integration
```csharp
// In data access operations
public async Task<DataTable> LoadDataWithProgressAsync(Control_ProgressBarUserControl progressControl)
{
    progressControl.ShowProgress();
    
    try
    {
        progressControl.UpdateProgress(10, "Establishing connection...");
        await EstablishConnectionAsync();
        
        progressControl.UpdateProgress(30, "Executing query...");
        var result = await ExecuteQueryAsync();
        
        progressControl.UpdateProgress(70, "Processing results...");
        var processedData = await ProcessResultsAsync(result);
        
        progressControl.UpdateProgress(90, "Finalizing...");
        await FinalizeAsync();
        
        progressControl.UpdateProgress(100, "Complete!");
        await progressControl.CompleteProgressAsync();
        
        return processedData;
    }
    catch (Exception ex)
    {
        progressControl.HideProgress();
        throw;
    }
}
```

### Form Integration Pattern
```csharp
// In any form requiring progress feedback
private Control_ProgressBarUserControl _progressControl = new();

private void InitializeProgressControl()
{
    _progressControl.Size = new Size(300, 100);
    _progressControl.Location = new Point(10, 10);
    _progressControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    _progressControl.Visible = false;
    
    Controls.Add(_progressControl);
}

private async Task PerformLongOperationAsync()
{
    _progressControl.EnableCancel(true);
    _progressControl.CancelRequested += () => _cancellationToken.Cancel();
    
    await LongRunningOperationAsync(_cancellationToken, (progress, status) => {
        _progressControl.UpdateProgress(progress, status);
    });
}
```

## Performance Considerations

### Memory Management
- **Timer Disposal**: Animation timers are properly disposed when progress completes
- **Event Cleanup**: Event handlers are automatically cleaned up with control disposal
- **Resource Optimization**: Minimal memory footprint with efficient graphics operations

### Rendering Optimization
- **Custom Paint**: Efficient arc drawing with minimal graphics operations
- **Animation Timing**: 50ms intervals provide smooth animation without excessive CPU usage
- **Invalidation Control**: Only the loading image area is invalidated during animation

### Thread Safety
- **UI Thread Marshaling**: All UI updates properly marshaled to UI thread
- **Async Support**: Full async/await pattern support for modern .NET operations
- **Cancellation Support**: Proper cancellation token integration

This Control_ProgressBarUserControl provides a comprehensive, thread-safe, and visually appealing progress feedback system that integrates seamlessly with the MTM WIP Application's theme system and operational patterns.