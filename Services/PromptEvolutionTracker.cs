using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTM_Shared_Logic.Development.PromptEvolution
{
    /// <summary>
    /// Tracks custom prompt usage for the MTM Prompt Evolution System
    /// </summary>
    public class PromptUsageTracker
    {
        private readonly string _logPath;
        private readonly object _lockObject = new object();

        public PromptUsageTracker(string logPath = "copilot_usage.log")
        {
            _logPath = logPath;
            EnsureLogDirectory();
        }

        /// <summary>
        /// Record a prompt usage event
        /// </summary>
        public async Task TrackUsageAsync(string promptName, string context, bool success = true, string[] modifications = null)
        {
            var usageEvent = new PromptUsageEvent
            {
                PromptName = promptName,
                Context = context,
                Success = success,
                Timestamp = DateTime.UtcNow,
                Modifications = modifications ?? Array.Empty<string>(),
                FileType = ExtractFileType(context),
                ProjectArea = ExtractProjectArea(context)
            };

            await LogUsageEventAsync(usageEvent);
        }

        /// <summary>
        /// Track when a prompt needs modification
        /// </summary>
        public async Task TrackModificationAsync(string promptName, string originalPrompt, string modifiedPrompt, string reason)
        {
            var modificationEvent = new PromptModificationEvent
            {
                PromptName = promptName,
                OriginalPrompt = originalPrompt,
                ModifiedPrompt = modifiedPrompt,
                Reason = reason,
                Timestamp = DateTime.UtcNow
            };

            await LogModificationEventAsync(modificationEvent);
        }

        /// <summary>
        /// Track prompt success/failure outcomes
        /// </summary>
        public async Task TrackOutcomeAsync(string promptName, bool success, string feedback = null, TimeSpan? duration = null)
        {
            var outcomeEvent = new PromptOutcomeEvent
            {
                PromptName = promptName,
                Success = success,
                Feedback = feedback,
                Duration = duration,
                Timestamp = DateTime.UtcNow
            };

            await LogOutcomeEventAsync(outcomeEvent);
        }

        private async Task LogUsageEventAsync(PromptUsageEvent usageEvent)
        {
            var logEntry = new LogEntry
            {
                Type = "Usage",
                Data = usageEvent,
                Timestamp = usageEvent.Timestamp
            };

            await WriteLogEntryAsync(logEntry);
        }

        private async Task LogModificationEventAsync(PromptModificationEvent modificationEvent)
        {
            var logEntry = new LogEntry
            {
                Type = "Modification",
                Data = modificationEvent,
                Timestamp = modificationEvent.Timestamp
            };

            await WriteLogEntryAsync(logEntry);
        }

        private async Task LogOutcomeEventAsync(PromptOutcomeEvent outcomeEvent)
        {
            var logEntry = new LogEntry
            {
                Type = "Outcome",
                Data = outcomeEvent,
                Timestamp = outcomeEvent.Timestamp
            };

            await WriteLogEntryAsync(logEntry);
        }

        private async Task WriteLogEntryAsync(LogEntry entry)
        {
            try
            {
                var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                lock (_lockObject)
                {
                    File.AppendAllText(_logPath, json + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // Log to console if file logging fails
                Console.WriteLine($"Failed to log prompt usage: {ex.Message}");
            }
        }

        private void EnsureLogDirectory()
        {
            var directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private string ExtractFileType(string context)
        {
            if (string.IsNullOrEmpty(context)) return "Unknown";

            var extension = Path.GetExtension(context).ToLowerInvariant();
            return extension switch
            {
                ".axaml" => "UI",
                ".cs" when context.Contains("ViewModel") => "ViewModel",
                ".cs" when context.Contains("Service") => "Service",
                ".cs" when context.Contains("Model") => "Model",
                ".cs" when context.Contains("Helper") => "Helper",
                ".cs" => "CSharp",
                ".md" => "Documentation",
                ".json" => "Configuration",
                ".xml" => "Configuration",
                _ => "Unknown"
            };
        }

        private string ExtractProjectArea(string context)
        {
            if (string.IsNullOrEmpty(context)) return "General";

            var lowerContext = context.ToLowerInvariant();
            
            if (lowerContext.Contains("views") || lowerContext.Contains("ui")) return "UI";
            if (lowerContext.Contains("viewmodels")) return "ViewModel";
            if (lowerContext.Contains("services")) return "Service";
            if (lowerContext.Contains("models")) return "Model";
            if (lowerContext.Contains("database") || lowerContext.Contains("db")) return "Database";
            if (lowerContext.Contains("documentation") || lowerContext.Contains("docs")) return "Documentation";
            if (lowerContext.Contains("configuration") || lowerContext.Contains("config")) return "Configuration";
            
            return "General";
        }
    }

    public class PromptUsageEvent
    {
        public string PromptName { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }
        public string[] Modifications { get; set; } = Array.Empty<string>();
        public string FileType { get; set; } = string.Empty;
        public string ProjectArea { get; set; } = string.Empty;
    }

    public class PromptModificationEvent
    {
        public string PromptName { get; set; } = string.Empty;
        public string OriginalPrompt { get; set; } = string.Empty;
        public string ModifiedPrompt { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class PromptOutcomeEvent
    {
        public string PromptName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Feedback { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LogEntry
    {
        public string Type { get; set; } = string.Empty;
        public object Data { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Static helper for easy integration into existing code
    /// </summary>
    public static class PromptEvolution
    {
        private static readonly Lazy<PromptUsageTracker> _tracker = new(() => new PromptUsageTracker());
        
        public static PromptUsageTracker Tracker => _tracker.Value;

        /// <summary>
        /// Quick method to track prompt usage
        /// </summary>
        public static async Task TrackAsync(string promptName, string context, bool success = true)
        {
            await Tracker.TrackUsageAsync(promptName, context, success);
        }

        /// <summary>
        /// Track prompt modification
        /// </summary>
        public static async Task TrackModificationAsync(string promptName, string reason)
        {
            await Tracker.TrackModificationAsync(promptName, string.Empty, string.Empty, reason);
        }

        /// <summary>
        /// Track prompt outcome
        /// </summary>
        public static async Task TrackOutcomeAsync(string promptName, bool success, string feedback = null)
        {
            await Tracker.TrackOutcomeAsync(promptName, success, feedback);
        }
    }
}

/*
Usage Examples:

// In a ViewModel creation scenario
await PromptEvolution.TrackAsync("CustomPrompt_Create_ReactiveUIViewModel", "InventoryViewModel.cs", true);

// When a prompt needs modification
await PromptEvolution.TrackModificationAsync("CustomPrompt_Create_UIElement", "Needed MTM theme integration");

// Track success/failure outcome
await PromptEvolution.TrackOutcomeAsync("CustomPrompt_Create_Service", false, "Missing dependency injection pattern");

// In a service class
public class MyService 
{
    public async Task DoSomething()
    {
        var startTime = DateTime.UtcNow;
        
        // ... your code here ...
        
        var duration = DateTime.UtcNow - startTime;
        await PromptEvolution.Tracker.TrackOutcomeAsync("CustomPrompt_Create_CRUDOperations", true, duration: duration);
    }
}
*/
