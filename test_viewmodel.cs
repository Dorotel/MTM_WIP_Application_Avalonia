using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;

// Simple console test
var loggerFactory = LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(LogLevel.Warning));
var logger = loggerFactory.CreateLogger<ThemeEditorViewModel>();

Console.WriteLine("Testing ThemeEditorViewModel...");

try {
    var vm = new ThemeEditorViewModel(logger);
    Console.WriteLine($"✅ ViewModel created successfully");
    Console.WriteLine($"   Theme Name: {vm.CurrentThemeName}");
    Console.WriteLine($"   Status: {vm.StatusMessage}");
    Console.WriteLine($"   Primary Color: {vm.PrimaryActionColor}");
    Console.WriteLine($"   Categories: {vm.ColorCategories.Count}");
    
    // Test that commands exist via reflection
    var type = vm.GetType();
    var commands = new[] { "ApplyThemeCommand", "ResetThemeCommand", "CloseCommand" };
    foreach(var cmd in commands) {
        var prop = type.GetProperty(cmd);
        Console.WriteLine($"   {cmd}: {(prop != null ? "✅ Found" : "❌ Missing")}");
    }
} catch (Exception ex) {
    Console.WriteLine($"❌ Error: {ex.Message}");
}
