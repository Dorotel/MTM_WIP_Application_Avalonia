using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Defines the type of focus request being made
/// </summary>
public enum FocusRequestType
{
    /// <summary>
    /// Focus request during application startup
    /// </summary>
    Startup,
    
    /// <summary>
    /// Focus request when switching between tabs
    /// </summary>
    TabSwitch,
    
    /// <summary>
    /// Focus request when switching between views within a tab
    /// </summary>
    ViewSwitch
}

/// <summary>
/// Event arguments for focus management requests from ViewModels to Views
/// </summary>
public class FocusManagementEventArgs : EventArgs
{
    /// <summary>
    /// The type of focus request
    /// </summary>
    public FocusRequestType FocusType { get; set; }
    
    /// <summary>
    /// The tab index to focus (for tab-based focus requests)
    /// </summary>
    public int TabIndex { get; set; }
    
    /// <summary>
    /// Delay in milliseconds before applying focus (allows for UI transitions)
    /// </summary>
    public int DelayMs { get; set; } = 100;
    
    /// <summary>
    /// Optional: Specific control name to focus (if not using TabIndex=1 logic)
    /// </summary>
    public string? ControlName { get; set; }
    
    /// <summary>
    /// Optional: Additional context for the focus request
    /// </summary>
    public string? Context { get; set; }
}
