using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services;
using MTM_Shared_Logic.Models;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay
{
    /// <summary>
    /// ViewModel for the Note Editor overlay.
    /// Handles reading and editing notes for inventory items.
    /// </summary>
    public partial class NoteEditorViewModel : ObservableObject
    {
        private readonly ILogger<NoteEditorViewModel> _logger;
        private readonly IDatabaseService _databaseService;
        
        [ObservableProperty]
        private string noteText = string.Empty;
        
        [ObservableProperty]
        private bool isLoading;
        
        [ObservableProperty]
        private bool isReadOnly;
        
        [ObservableProperty]
        private string partId = string.Empty;
        
        [ObservableProperty]
        private string operation = string.Empty;
        
        [ObservableProperty]
        private string location = string.Empty;
        
        private int _inventoryId;

        public event EventHandler<NoteEditorResult>? NoteEditCompleted;

        public NoteEditorViewModel(ILogger<NoteEditorViewModel> logger, IDatabaseService databaseService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        /// <summary>
        /// Initializes the editor with an inventory item.
        /// </summary>
        public void Initialize(InventoryItem item, bool readOnly = false)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            _inventoryId = item.ID;
            PartId = item.PartID;
            Operation = item.Operation ?? string.Empty;
            Location = item.Location;
            NoteText = item.Notes ?? string.Empty;
            IsReadOnly = readOnly;
        }

        /// <summary>
        /// Async initialization with explicit parameters.
        /// </summary>
        public Task InitializeAsync(int inventoryId, string partId, string operation, string location, string noteText, bool isReadOnly = false)
        {
            _logger.LogInformation("🔧 NoteEditor InitializeAsync called - InventoryId: {InventoryId}, PartId: '{PartId}', Operation: '{Operation}', Location: '{Location}'", 
                inventoryId, partId, operation, location);
            
            _inventoryId = inventoryId;
            PartId = partId ?? string.Empty;
            Operation = operation ?? string.Empty;
            Location = location ?? string.Empty;
            NoteText = noteText ?? string.Empty;
            IsReadOnly = isReadOnly;
            
            _logger.LogInformation("🔧 NoteEditor InitializeAsync completed - Properties set: PartId='{PartId}', Operation='{Operation}', Location='{Location}', NoteText='{NoteText}'", 
                PartId, Operation, Location, NoteText);
            
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsLoading) return;
            
            try
            {
                IsLoading = true;
                
                // Update the note in the database
                var result = await UpdateInventoryNoteAsync(_inventoryId, NoteText);
                
                if (result.Success)
                {
                    _logger.LogInformation("Note updated successfully for inventory item {InventoryId}", _inventoryId);
                    
                    // Raise completion event with success
                    NoteEditCompleted?.Invoke(this, new NoteEditorResult
                    {
                        Success = true,
                        UpdatedNote = NoteText,
                        InventoryId = _inventoryId
                    });
                }
                else
                {
                    _logger.LogWarning("Failed to update note for inventory item {InventoryId}: {Message}", _inventoryId, result.Message);
                    await ErrorHandling.HandleErrorAsync(
                        new InvalidOperationException($"Failed to update note: {result.Message}"),
                        "Note update failed",
                        "SYSTEM"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving note for inventory item {InventoryId}", _inventoryId);
                await ErrorHandling.HandleErrorAsync(ex, "Error saving note", "SYSTEM");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            // Raise completion event with cancellation
            NoteEditCompleted?.Invoke(this, new NoteEditorResult
            {
                Success = false,
                UpdatedNote = null,
                InventoryId = _inventoryId
            });
        }

        /// <summary>
        /// Updates the note for an inventory item using the stored procedure.
        /// </summary>
        private async Task<(bool Success, string Message)> UpdateInventoryNoteAsync(int inventoryId, string noteText)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_InventoryId"] = inventoryId,
                    ["p_Notes"] = noteText ?? string.Empty
                };

                var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                    _databaseService.GetConnectionString(),
                    "inv_inventory_Update_Note",
                    parameters
                );

                // MTM Status Pattern: 1 = Success, 0 = No change, -1 = Error
                if (result.Status >= 0)
                {
                    return (true, "Note updated successfully");
                }
                else
                {
                    return (false, result.Message ?? "Unknown error occurred");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error updating note for inventory {InventoryId}", inventoryId);
                return (false, $"Database error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Result class for note editing operations.
    /// </summary>
    public class NoteEditorResult
    {
        public bool Success { get; set; }
        public string? UpdatedNote { get; set; }
        public int InventoryId { get; set; }
    }
}