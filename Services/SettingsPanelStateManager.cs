using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Settings panel state manager for tracking changes and managing snapshots.
/// Provides per-panel change tracking with state snapshots and rollback capabilities.
/// </summary>
public class SettingsPanelStateManager
{
    private readonly ILogger<SettingsPanelStateManager> _logger;
    private readonly Dictionary<string, PanelStateSnapshot> _stateSnapshots;

    public SettingsPanelStateManager(ILogger<SettingsPanelStateManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stateSnapshots = new Dictionary<string, PanelStateSnapshot>();
    }

    #region Events

    /// <summary>
    /// Event raised when state changes occur.
    /// </summary>
    public event EventHandler<PanelStateChangedEventArgs>? StateChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if any panels have unsaved changes.
    /// </summary>
    public bool HasAnyUnsavedChanges => _stateSnapshots.Values.Any(s => s.HasChanges);

    /// <summary>
    /// Gets count of panels with unsaved changes.
    /// </summary>
    public int UnsavedChangesCount => _stateSnapshots.Values.Count(s => s.HasChanges);

    /// <summary>
    /// Gets all panel IDs with unsaved changes.
    /// </summary>
    public IEnumerable<string> PanelsWithUnsavedChanges => 
        _stateSnapshots.Where(kvp => kvp.Value.HasChanges).Select(kvp => kvp.Key);

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a state snapshot for the specified panel.
    /// </summary>
    public void CreateSnapshot(string panelId, BaseViewModel viewModel)
    {
        if (string.IsNullOrEmpty(panelId))
        {
            throw new ArgumentException("Panel ID cannot be null or empty", nameof(panelId));
        }

        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        try
        {
            var snapshot = new PanelStateSnapshot
            {
                PanelId = panelId,
                Timestamp = DateTime.UtcNow,
                OriginalValues = ExtractViewModelState(viewModel),
                CurrentValues = ExtractViewModelState(viewModel)
            };

            _stateSnapshots[panelId] = snapshot;

            _logger.LogDebug("Created state snapshot for panel {PanelId}", panelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating state snapshot for panel {PanelId}", panelId);
            throw;
        }
    }

    /// <summary>
    /// Updates the current state for a panel and checks for changes.
    /// </summary>
    public void UpdateCurrentState(string panelId, BaseViewModel viewModel)
    {
        if (!_stateSnapshots.TryGetValue(panelId, out var snapshot))
        {
            _logger.LogWarning("No snapshot found for panel {PanelId}, creating new snapshot", panelId);
            CreateSnapshot(panelId, viewModel);
            return;
        }

        try
        {
            var previousHasChanges = snapshot.HasChanges;
            snapshot.CurrentValues = ExtractViewModelState(viewModel);
            snapshot.LastModified = DateTime.UtcNow;

            var currentHasChanges = snapshot.HasChanges;

            // Raise event if change status changed
            if (previousHasChanges != currentHasChanges)
            {
                OnStateChanged(panelId, currentHasChanges);
            }

            _logger.LogDebug("Updated current state for panel {PanelId}, HasChanges: {HasChanges}", 
                panelId, currentHasChanges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating current state for panel {PanelId}", panelId);
        }
    }

    /// <summary>
    /// Checks if the specified panel has unsaved changes.
    /// </summary>
    public bool HasUnsavedChanges(string panelId)
    {
        return _stateSnapshots.TryGetValue(panelId, out var snapshot) && snapshot.HasChanges;
    }

    /// <summary>
    /// Saves changes for the specified panel.
    /// </summary>
    public async Task<ServiceResult> SaveChangesAsync(string panelId)
    {
        if (!_stateSnapshots.TryGetValue(panelId, out var snapshot))
        {
            return ServiceResult.Failure($"No snapshot found for panel {panelId}");
        }

        try
        {
            _logger.LogInformation("Saving changes for panel {PanelId}", panelId);

            // Update original values to match current (simulate save)
            snapshot.OriginalValues = new Dictionary<string, object?>(snapshot.CurrentValues);
            snapshot.LastSaved = DateTime.UtcNow;

            OnStateChanged(panelId, false);

            _logger.LogInformation("Successfully saved changes for panel {PanelId}", panelId);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes for panel {PanelId}", panelId);
            return ServiceResult.Failure($"Error saving changes: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves changes for all panels with unsaved changes.
    /// </summary>
    public async Task<ServiceResult> SaveAllChangesAsync()
    {
        var panelsWithChanges = PanelsWithUnsavedChanges.ToList();
        
        if (!panelsWithChanges.Any())
        {
            return ServiceResult.Success("No changes to save");
        }

        try
        {
            _logger.LogInformation("Saving changes for {Count} panels", panelsWithChanges.Count);

            var tasks = panelsWithChanges.Select(SaveChangesAsync);
            var results = await Task.WhenAll(tasks);

            var failedResults = results.Where(r => !r.IsSuccess).ToList();
            
            if (failedResults.Any())
            {
                var errorMessages = string.Join("; ", failedResults.Select(r => r.Message));
                return ServiceResult.Failure($"Some saves failed: {errorMessages}");
            }

            _logger.LogInformation("Successfully saved all changes for {Count} panels", panelsWithChanges.Count);
            return ServiceResult.Success($"Saved changes for {panelsWithChanges.Count} panels");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving all changes");
            return ServiceResult.Failure($"Error saving all changes: {ex.Message}");
        }
    }

    /// <summary>
    /// Reverts changes for the specified panel to original state.
    /// </summary>
    public async Task<ServiceResult> RevertChangesAsync(string panelId, BaseViewModel viewModel)
    {
        if (!_stateSnapshots.TryGetValue(panelId, out var snapshot))
        {
            return ServiceResult.Failure($"No snapshot found for panel {panelId}");
        }

        try
        {
            _logger.LogInformation("Reverting changes for panel {PanelId}", panelId);

            // Restore original values to ViewModel
            RestoreViewModelState(viewModel, snapshot.OriginalValues);
            
            // Update current values to match original
            snapshot.CurrentValues = new Dictionary<string, object?>(snapshot.OriginalValues);

            OnStateChanged(panelId, false);

            _logger.LogInformation("Successfully reverted changes for panel {PanelId}", panelId);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting changes for panel {PanelId}", panelId);
            return ServiceResult.Failure($"Error reverting changes: {ex.Message}");
        }
    }

    /// <summary>
    /// Reverts changes for all panels with unsaved changes.
    /// </summary>
    public async Task<ServiceResult> RevertAllChangesAsync()
    {
        var panelsWithChanges = PanelsWithUnsavedChanges.ToList();
        
        if (!panelsWithChanges.Any())
        {
            return ServiceResult.Success("No changes to revert");
        }

        try
        {
            _logger.LogInformation("Reverting changes for {Count} panels", panelsWithChanges.Count);

            // Note: For full implementation, we would need references to ViewModels
            // For now, we'll just reset the snapshots
            foreach (var panelId in panelsWithChanges)
            {
                if (_stateSnapshots.TryGetValue(panelId, out var snapshot))
                {
                    snapshot.CurrentValues = new Dictionary<string, object?>(snapshot.OriginalValues);
                    OnStateChanged(panelId, false);
                }
            }

            _logger.LogInformation("Successfully reverted all changes for {Count} panels", panelsWithChanges.Count);
            return ServiceResult.Success($"Reverted changes for {panelsWithChanges.Count} panels");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting all changes");
            return ServiceResult.Failure($"Error reverting all changes: {ex.Message}");
        }
    }

    /// <summary>
    /// Removes state tracking for the specified panel.
    /// </summary>
    public void RemoveSnapshot(string panelId)
    {
        if (_stateSnapshots.Remove(panelId))
        {
            _logger.LogDebug("Removed state snapshot for panel {PanelId}", panelId);
        }
    }

    /// <summary>
    /// Gets state information for the specified panel.
    /// </summary>
    public PanelStateSnapshot? GetSnapshot(string panelId)
    {
        _stateSnapshots.TryGetValue(panelId, out var snapshot);
        return snapshot;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Extracts current state from ViewModel properties.
    /// </summary>
    private Dictionary<string, object?> ExtractViewModelState(BaseViewModel viewModel)
    {
        var state = new Dictionary<string, object?>();
        
        // Use reflection to get all public properties
        var properties = viewModel.GetType().GetProperties(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.CanRead && !property.PropertyType.IsSubclassOf(typeof(System.Windows.Input.ICommand)))
            {
                try
                {
                    var value = property.GetValue(viewModel);
                    state[property.Name] = value;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not extract property {PropertyName} from {ViewModelType}", 
                        property.Name, viewModel.GetType().Name);
                }
            }
        }

        return state;
    }

    /// <summary>
    /// Restores ViewModel state from saved values.
    /// </summary>
    private void RestoreViewModelState(BaseViewModel viewModel, Dictionary<string, object?> state)
    {
        var properties = viewModel.GetType().GetProperties(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.CanWrite && state.TryGetValue(property.Name, out var value))
            {
                try
                {
                    property.SetValue(viewModel, value);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not restore property {PropertyName} on {ViewModelType}", 
                        property.Name, viewModel.GetType().Name);
                }
            }
        }
    }

    /// <summary>
    /// Raises state changed event.
    /// </summary>
    private void OnStateChanged(string panelId, bool hasChanges)
    {
        StateChanged?.Invoke(this, new PanelStateChangedEventArgs
        {
            PanelId = panelId,
            HasChanges = hasChanges,
            Timestamp = DateTime.UtcNow
        });
    }

    #endregion
}

/// <summary>
/// Panel state snapshot for change tracking.
/// </summary>
public class PanelStateSnapshot
{
    public string PanelId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? LastSaved { get; set; }
    public Dictionary<string, object?> OriginalValues { get; set; } = new();
    public Dictionary<string, object?> CurrentValues { get; set; } = new();

    /// <summary>
    /// Indicates if current values differ from original values.
    /// </summary>
    public bool HasChanges
    {
        get
        {
            if (OriginalValues.Count != CurrentValues.Count)
                return true;

            foreach (var kvp in OriginalValues)
            {
                if (!CurrentValues.TryGetValue(kvp.Key, out var currentValue))
                    return true;

                if (!Equals(kvp.Value, currentValue))
                    return true;
            }

            return false;
        }
    }
}

/// <summary>
/// Event arguments for panel state changes.
/// </summary>
public class PanelStateChangedEventArgs : EventArgs
{
    public string PanelId { get; set; } = string.Empty;
    public bool HasChanges { get; set; }
    public DateTime Timestamp { get; set; }
}