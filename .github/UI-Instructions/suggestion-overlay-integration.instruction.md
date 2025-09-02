# GitHub Copilot Instructions: Integrating SuggestionOverlayView in Existing Views

This comprehensive guide explains how to integrate the SuggestionOverlayView component into existing views within the MTM WIP Application, using RemoveTabView.axaml as the primary example. The integration provides intelligent suggestion capabilities when user input doesn't match exact database records.

<details>
<summary><strong>🎯 Integration Overview</strong></summary>

The SuggestionOverlayView integration transforms basic AutoCompleteBox controls into intelligent suggestion systems that:

- **Detect No-Match Scenarios**: When user input doesn't match exact database records
- **Provide Fuzzy Matching**: Show similar items based on text similarity algorithms
- **Maintain User Experience**: Seamless integration without disrupting existing workflows
- **Support Multiple Controls**: Can be activated for any AutoCompleteBox or ComboBox in the view

**Target Integration Points in RemoveTabView:**
- Part ID AutoCompleteBox
- Operation AutoCompleteBox  
- Location fields (if added)
- Custom search fields

**Benefits:**
- Reduced user frustration from typos
- Improved data accuracy through suggestions
- Enhanced user experience with intelligent assistance
- Consistent behavior across all form inputs

</details>

<details>
<summary><strong>🏗️ Step 1: ViewModel Integration Pattern</strong></summary>

### Required ViewModel Modifications

Add these properties and methods to your target ViewModel (e.g., `RemoveItemViewModel.cs`):

```csharp
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

public class RemoveItemViewModel : BaseViewModel
{
    // Existing properties...

    #region Suggestion Overlay Integration

    private SuggestionOverlayViewModel? _suggestionOverlay;
    /// <summary>
    /// Suggestion overlay for handling no-match scenarios
    /// </summary>
    public SuggestionOverlayViewModel? SuggestionOverlay
    {
        get => _suggestionOverlay;
        set => SetProperty(ref _suggestionOverlay, value);
    }

    private bool _isOverlayVisible;
    /// <summary>
    /// Controls visibility of the suggestion overlay
    /// </summary>
    public bool IsOverlayVisible
    {
        get => _isOverlayVisible;
        set => SetProperty(ref _isOverlayVisible, value);
    }

    /// <summary>
    /// Stores the current input context for suggestion processing
    /// </summary>
    private string _currentSuggestionContext = string.Empty;

    #endregion

    // In constructor, add:
    public RemoveItemViewModel(/* existing parameters */)
    {
        // Existing initialization...
        
        // Setup suggestion overlay event handlers
        InitializeSuggestionHandlers();
    }

    #region Suggestion Integration Methods

    /// <summary>
    /// Initializes suggestion overlay event handlers
    /// </summary>
    private void InitializeSuggestionHandlers()
    {
        // Subscribe to property changes for suggestion triggers
        PropertyChanged += OnPropertyChangedForSuggestions;
    }

    /// <summary>
    /// Handles property changes that might trigger suggestions
    /// </summary>
    private void OnPropertyChangedForSuggestions(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PartText):
                _ = CheckPartSuggestionsAsync();
                break;
            case nameof(OperationText):
                _ = CheckOperationSuggestionsAsync();
                break;
        }
    }

    /// <summary>
    /// Checks if Part input requires suggestions
    /// </summary>
    private async Task CheckPartSuggestionsAsync()
    {
        if (string.IsNullOrWhiteSpace(PartText) || PartOptions.Contains(PartText))
            return;

        // Find similar parts using fuzzy matching
        var suggestions = FindSimilarParts(PartText);
        if (suggestions.Any())
        {
            await ShowSuggestionsAsync(suggestions, "Part", PartText);
        }
    }

    /// <summary>
    /// Checks if Operation input requires suggestions
    /// </summary>
    private async Task CheckOperationSuggestionsAsync()
    {
        if (string.IsNullOrWhiteSpace(OperationText) || OperationOptions.Contains(OperationText))
            return;

        // Find similar operations
        var suggestions = FindSimilarOperations(OperationText);
        if (suggestions.Any())
        {
            await ShowSuggestionsAsync(suggestions, "Operation", OperationText);
        }
    }

    /// <summary>
    /// Shows suggestion overlay with provided options
    /// </summary>
    private async Task ShowSuggestionsAsync(IEnumerable<string> suggestions, string context, string originalInput)
    {
        _currentSuggestionContext = context;
        
        SuggestionOverlay = new SuggestionOverlayViewModel(suggestions);
        SuggestionOverlay.SuggestionSelected += OnSuggestionSelected;
        SuggestionOverlay.Cancelled += OnSuggestionCancelled;
        
        IsOverlayVisible = true;
        
        Logger.LogInformation("Showing suggestions for {Context}: {Input}", context, originalInput);
    }

    /// <summary>
    /// Handles suggestion selection
    /// </summary>
    private void OnSuggestionSelected(string selectedSuggestion)
    {
        switch (_currentSuggestionContext)
        {
            case "Part":
                PartText = selectedSuggestion;
                SelectedPart = selectedSuggestion;
                break;
            case "Operation":
                OperationText = selectedSuggestion;
                SelectedOperation = selectedSuggestion;
                break;
        }

        Logger.LogInformation("Applied suggestion: {Context} = {Value}", _currentSuggestionContext, selectedSuggestion);
        
        DisposeSuggestionOverlay();
    }

    /// <summary>
    /// Handles suggestion cancellation
    /// </summary>
    private void OnSuggestionCancelled()
    {
        Logger.LogInformation("Suggestion overlay cancelled for context: {Context}", _currentSuggestionContext);
        DisposeSuggestionOverlay();
    }

    /// <summary>
    /// Cleans up suggestion overlay
    /// </summary>
    private void DisposeSuggestionOverlay()
    {
        if (SuggestionOverlay != null)
        {
            SuggestionOverlay.SuggestionSelected -= OnSuggestionSelected;
            SuggestionOverlay.Cancelled -= OnSuggestionCancelled;
            SuggestionOverlay = null;
        }
        
        IsOverlayVisible = false;
        _currentSuggestionContext = string.Empty;
    }

    #endregion

    #region Fuzzy Matching Logic

    /// <summary>
    /// Finds similar parts using fuzzy matching algorithms
    /// </summary>
    private IEnumerable<string> FindSimilarParts(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
            return Enumerable.Empty<string>();

        return PartOptions
            .Where(part => CalculateSimilarity(input, part) > 0.6) // 60% similarity threshold
            .OrderByDescending(part => CalculateSimilarity(input, part))
            .Take(5); // Limit to top 5 suggestions
    }

    /// <summary>
    /// Finds similar operations using fuzzy matching
    /// </summary>
    private IEnumerable<string> FindSimilarOperations(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Enumerable.Empty<string>();

        return OperationOptions
            .Where(op => op.Contains(input) || CalculateSimilarity(input, op) > 0.5)
            .OrderByDescending(op => CalculateSimilarity(input, op))
            .Take(3); // Limit to top 3 suggestions
    }

    /// <summary>
    /// Calculates similarity between two strings using Levenshtein distance
    /// </summary>
    private static double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        var distance = LevenshteinDistance(source.ToLower(), target.ToLower());
        var maxLength = Math.Max(source.Length, target.Length);
        return 1.0 - (double)distance / maxLength;
    }

    /// <summary>
    /// Calculates Levenshtein distance between two strings
    /// </summary>
    private static int LevenshteinDistance(string source, string target)
    {
        if (source.Length == 0) return target.Length;
        if (target.Length == 0) return source.Length;

        var matrix = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= target.Length; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[source.Length, target.Length];
    }

    #endregion
}
```

### Key Integration Points

1. **Property Monitoring**: Watch for text changes in AutoCompleteBox controls
2. **Fuzzy Matching**: Implement similarity algorithms for intelligent suggestions
3. **Context Tracking**: Track which field triggered the suggestion overlay
4. **Event Handling**: Clean event subscription and disposal patterns
5. **User Experience**: Seamless integration without disrupting existing workflows

</details>

<details>
<summary><strong>🎨 Step 2: AXAML View Integration</strong></summary>

### Add Namespace Declarations

Update the UserControl root element in your target view (e.g., `RemoveTabView.axaml`):

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             xmlns:overlayVm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             xmlns:overlayViews="using:MTM_WIP_Application_Avalonia.Views"
             xmlns:materialIcons="using:Material.Icons.Avalonia"
             xmlns:i="using:Avalonia.Xaml.Interactivity"
             xmlns:behaviors="using:MTM_WIP_Application_Avalonia.Behaviors"
             mc:Ignorable="d"
             d:DesignWidth="800"
             d:DesignHeight="600"
             x:Class="MTM_WIP_Application_Avalonia.Views.RemoveTabView"
             x:CompileBindings="True"
             x:DataType="vm:RemoveItemViewModel">
```

**Key Additions:**
- `overlayVm` namespace for SuggestionOverlayViewModel
- `overlayViews` namespace for SuggestionOverlayView

### Modify Main Grid Structure

Replace the existing main Grid with an overlay-capable structure:

```xml
<!-- Replace existing main container -->
<Grid RowDefinitions="*,Auto">
    <!-- Original content in a container -->
    <Grid Grid.Row="0" 
          RowDefinitions="*,Auto" 
          HorizontalAlignment="Stretch" 
          VerticalAlignment="Stretch"
          Margin="0"
          Background="{DynamicResource MTM_Shared_Logic.MainBackground}">

        <!-- EXISTING CONTENT GOES HERE -->
        <!-- Main Panel -->
        <Border Grid.Row="0"
                Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
                BorderThickness="1"
                CornerRadius="8"
                Padding="12"
                Margin="8"
                HorizontalAlignment="Stretch"
                VerticalAlignment="Stretch">
            <!-- ... existing grid content ... -->
        </Border>

        <!-- Bottom Action Panel -->
        <Border Grid.Row="1"
                Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"
                BorderThickness="1"
                CornerRadius="8"
                Padding="12"
                Margin="8,0,8,8">
            <!-- ... existing action panel content ... -->
        </Border>
    </Grid>

    <!-- Suggestion Overlay Integration -->
    <Border Grid.Row="0" 
            Grid.RowSpan="2"
            Background="Transparent"
            IsVisible="{Binding IsOverlayVisible}"
            HorizontalAlignment="Stretch"
            VerticalAlignment="Stretch"
            ZIndex="1000">
        
        <!-- Semi-transparent backdrop -->
        <Border Background="#80000000" 
                HorizontalAlignment="Stretch" 
                VerticalAlignment="Stretch">
            
            <!-- Centered overlay container -->
            <Border HorizontalAlignment="Center" 
                    VerticalAlignment="Center"
                    MaxWidth="400" 
                    MaxHeight="300"
                    Margin="20">
                
                <!-- Suggestion overlay view -->
                <overlayViews:SuggestionOverlayView 
                    DataContext="{Binding SuggestionOverlay}"
                    IsVisible="{Binding SuggestionOverlay, Converter={x:Static ObjectConverters.IsNotNull}}"/>
                
            </Border>
        </Border>
    </Border>
</Grid>
```

### Enhanced AutoCompleteBox Integration

Modify existing AutoCompleteBox controls to support suggestion triggers:

```xml
<!-- Enhanced Part ID AutoCompleteBox -->
<AutoCompleteBox ItemsSource="{Binding PartOptions}"
                 SelectedItem="{Binding SelectedPart, Mode=TwoWay}"
                 Text="{Binding PartText, Mode=TwoWay}"
                 Classes="input-field"
                 HorizontalAlignment="Stretch"
                 Watermark="Select part ID..."
                 ToolTip.Tip="Select part to remove - type for suggestions"
                 MinimumPrefixLength="0"
                 FilterMode="Contains"
                 MaxDropDownHeight="200"
                 IsTextCompletionEnabled="False"
                 behaviors:ComboBoxBehavior.EnableComboBoxBehavior="True"
                 x:Name="PartAutoCompleteBox"
                 LostFocus="OnPartAutoCompleteBoxLostFocus" />

<!-- Enhanced Operation AutoCompleteBox -->
<AutoCompleteBox ItemsSource="{Binding OperationOptions}"
                 SelectedItem="{Binding SelectedOperation, Mode=TwoWay}"
                 Text="{Binding OperationText, Mode=TwoWay}"
                 Classes="input-field"
                 HorizontalAlignment="Stretch"
                 Watermark="Select operation..."
                 ToolTip.Tip="Select operation - type for suggestions"
                 MinimumPrefixLength="0"
                 FilterMode="StartsWith"
                 MaxDropDownHeight="200"
                 IsTextCompletionEnabled="False"
                 behaviors:ComboBoxBehavior.EnableComboBoxBehavior="True"
                 x:Name="OperationAutoCompleteBox"
                 LostFocus="OnOperationAutoCompleteBoxLostFocus" />
```

**Key Enhancements:**
- Added `x:Name` for code-behind access
- Added `LostFocus` event handlers for suggestion triggers
- Updated ToolTip to indicate suggestion capability
- Maintained existing AutoCompleteBox functionality

</details>

<details>
<summary><strong>⚡ Step 3: Code-Behind Integration</strong></summary>

### Required Code-Behind Methods

Add these methods to your view's code-behind file (e.g., `RemoveTabView.axaml.cs`):

```csharp
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views
{
    public partial class RemoveTabView : UserControl
    {
        public RemoveTabView()
        {
            InitializeComponent();
        }

        #region Suggestion Integration Event Handlers

        /// <summary>
        /// Handles Part AutoCompleteBox focus loss to trigger suggestions
        /// </summary>
        private async void OnPartAutoCompleteBoxLostFocus(object? sender, RoutedEventArgs e)
        {
            if (DataContext is RemoveItemViewModel viewModel && 
                sender is AutoCompleteBox autoCompleteBox)
            {
                var text = autoCompleteBox.Text;
                
                // Only trigger suggestions if text doesn't match existing options
                if (!string.IsNullOrWhiteSpace(text) && 
                    !viewModel.PartOptions.Contains(text) &&
                    text.Length >= 2) // Minimum length for suggestions
                {
                    // Let ViewModel handle the suggestion logic
                    // The ViewModel's PropertyChanged handler will trigger suggestions
                    await Task.Delay(100); // Small delay to ensure property is set
                }
            }
        }

        /// <summary>
        /// Handles Operation AutoCompleteBox focus loss to trigger suggestions
        /// </summary>
        private async void OnOperationAutoCompleteBoxLostFocus(object? sender, RoutedEventArgs e)
        {
            if (DataContext is RemoveItemViewModel viewModel && 
                sender is AutoCompleteBox autoCompleteBox)
            {
                var text = autoCompleteBox.Text;
                
                // Only trigger suggestions if text doesn't match existing options
                if (!string.IsNullOrWhiteSpace(text) && 
                    !viewModel.OperationOptions.Contains(text))
                {
                    // Let ViewModel handle the suggestion logic
                    await Task.Delay(100); // Small delay to ensure property is set
                }
            }
        }

        /// <summary>
        /// Handles keyboard shortcuts for suggestion overlay
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (DataContext is RemoveItemViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        if (viewModel.IsOverlayVisible)
                        {
                            // Close suggestion overlay
                            viewModel.SuggestionOverlay?.CancelCommand.Execute(null);
                            e.Handled = true;
                        }
                        break;
                        
                    case Key.F1:
                        // Show help for suggestions
                        ShowSuggestionHelp();
                        e.Handled = true;
                        break;
                }
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Shows help information about suggestion functionality
        /// </summary>
        private void ShowSuggestionHelp()
        {
            // TODO: Implement help dialog or tooltip
            // Could show information about:
            // - How suggestions work
            // - Keyboard shortcuts
            // - Fuzzy matching capabilities
        }

        #endregion

        #region Manual Suggestion Triggers (Optional)

        /// <summary>
        /// Manually triggers suggestions for Part field
        /// Can be called from context menu or button
        /// </summary>
        public async Task TriggerPartSuggestionsAsync()
        {
            if (DataContext is RemoveItemViewModel viewModel &&
                PartAutoCompleteBox != null)
            {
                var text = PartAutoCompleteBox.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // Force suggestion check
                    await viewModel.CheckPartSuggestionsAsync();
                }
            }
        }

        /// <summary>
        /// Manually triggers suggestions for Operation field
        /// </summary>
        public async Task TriggerOperationSuggestionsAsync()
        {
            if (DataContext is RemoveItemViewModel viewModel &&
                OperationAutoCompleteBox != null)
            {
                var text = OperationAutoCompleteBox.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    // Force suggestion check
                    await viewModel.CheckOperationSuggestionsAsync();
                }
            }
        }

        #endregion
    }
}
```

### Event Handler Strategy

1. **Focus-Based Triggers**: Suggestions activate when user leaves a field with non-matching text
2. **Keyboard Support**: Escape key closes overlay, F1 shows help
3. **Manual Triggers**: Optional methods for programmatic suggestion activation
4. **Null Safety**: All handlers include proper null checks and type validation

</details>

<details>
<summary><strong>🔧 Step 4: Advanced Integration Patterns</strong></summary>

### Custom Suggestion Providers

For more sophisticated suggestion logic, implement custom providers:

```csharp
/// <summary>
/// Interface for providing custom suggestions
/// </summary>
public interface ISuggestionProvider
{
    Task<IEnumerable<string>> GetSuggestionsAsync(string query, string context);
    double GetSimilarityThreshold(string context);
    int GetMaxSuggestions(string context);
}

/// <summary>
/// MTM-specific suggestion provider with business logic
/// </summary>
public class MTMSuggestionProvider : ISuggestionProvider
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<MTMSuggestionProvider> _logger;

    public MTMSuggestionProvider(IDatabaseService databaseService, ILogger<MTMSuggestionProvider> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetSuggestionsAsync(string query, string context)
    {
        switch (context.ToLower())
        {
            case "part":
                return await GetPartSuggestionsAsync(query);
            case "operation":
                return await GetOperationSuggestionsAsync(query);
            case "location":
                return await GetLocationSuggestionsAsync(query);
            default:
                return Enumerable.Empty<string>();
        }
    }

    public double GetSimilarityThreshold(string context)
    {
        return context.ToLower() switch
        {
            "part" => 0.6,      // Parts need higher similarity
            "operation" => 0.4,  // Operations can be more flexible
            "location" => 0.7,   // Locations need high accuracy
            _ => 0.5
        };
    }

    public int GetMaxSuggestions(string context)
    {
        return context.ToLower() switch
        {
            "part" => 5,
            "operation" => 3,
            "location" => 4,
            _ => 3
        };
    }

    private async Task<IEnumerable<string>> GetPartSuggestionsAsync(string query)
    {
        // Use database to find similar parts
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _databaseService.GetConnectionString(),
            "md_part_ids_Get_Similar",
            new Dictionary<string, object> { ["SearchTerm"] = query }
        );

        if (result.IsSuccess)
        {
            return result.Data.AsEnumerable()
                .Select(row => row["PartID"]?.ToString())
                .Where(partId => !string.IsNullOrEmpty(partId))
                .Cast<string>();
        }

        return Enumerable.Empty<string>();
    }

    private async Task<IEnumerable<string>> GetOperationSuggestionsAsync(string query)
    {
        // Simple numeric-based matching for operations
        if (int.TryParse(query, out var numericQuery))
        {
            var operations = new[] { "90", "100", "110", "120", "130", "140" };
            return operations.Where(op => op.Contains(query)).Take(3);
        }

        return Enumerable.Empty<string>();
    }

    private async Task<IEnumerable<string>> GetLocationSuggestionsAsync(string query)
    {
        // Database lookup for locations
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _databaseService.GetConnectionString(),
            "md_locations_Get_Similar",
            new Dictionary<string, object> { ["SearchTerm"] = query }
        );

        if (result.IsSuccess)
        {
            return result.Data.AsEnumerable()
                .Select(row => row["Location"]?.ToString())
                .Where(location => !string.IsNullOrEmpty(location))
                .Cast<string>();
        }

        return Enumerable.Empty<string>();
    }
}
```

### Integration with Custom Provider

Update ViewModel to use custom provider:

```csharp
public class RemoveItemViewModel : BaseViewModel
{
    private readonly ISuggestionProvider _suggestionProvider;

    public RemoveItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ISuggestionProvider suggestionProvider,
        ILogger<RemoveItemViewModel> logger) : base(logger)
    {
        _suggestionProvider = suggestionProvider;
        // ... existing initialization
    }

    /// <summary>
    /// Enhanced suggestion checking using custom provider
    /// </summary>
    private async Task CheckSuggestionsAsync(string input, string context)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        var suggestions = await _suggestionProvider.GetSuggestionsAsync(input, context);
        if (suggestions.Any())
        {
            await ShowSuggestionsAsync(suggestions, context, input);
        }
    }
}
```

### Service Registration

Register the suggestion provider in your DI container:

```csharp
// In ServiceCollectionExtensions.cs
public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
{
    // ... existing registrations
    
    services.AddScoped<ISuggestionProvider, MTMSuggestionProvider>();
    
    return services;
}
```

</details>

<details>
<summary><strong>🎛️ Step 5: Context Menu Integration</strong></summary>

### Add Context Menu Support

Enhance AutoCompleteBox controls with right-click suggestion options:

```xml
<!-- Enhanced Part AutoCompleteBox with Context Menu -->
<AutoCompleteBox ItemsSource="{Binding PartOptions}"
                 SelectedItem="{Binding SelectedPart, Mode=TwoWay}"
                 Text="{Binding PartText, Mode=TwoWay}"
                 Classes="input-field"
                 x:Name="PartAutoCompleteBox">
    
    <AutoCompleteBox.ContextFlyout>
        <MenuFlyout>
            <MenuItem Header="Find Similar Parts" 
                      Command="{Binding TriggerPartSuggestionsCommand}"
                      InputGesture="Ctrl+F">
                <MenuItem.Icon>
                    <materialIcons:MaterialIcon Kind="Lightbulb" Width="16" Height="16"/>
                </MenuItem.Icon>
            </MenuItem>
            <MenuItem Header="Clear Field" 
                      Command="{Binding ClearPartCommand}">
                <MenuItem.Icon>
                    <materialIcons:MaterialIcon Kind="Clear" Width="16" Height="16"/>
                </MenuItem.Icon>
            </MenuItem>
            <Separator/>
            <MenuItem Header="Suggestion Help" 
                      Command="{Binding ShowSuggestionHelpCommand}">
                <MenuItem.Icon>
                    <materialIcons:MaterialIcon Kind="HelpCircle" Width="16" Height="16"/>
                </MenuItem.Icon>
            </MenuItem>
        </MenuFlyout>
    </AutoCompleteBox.ContextFlyout>
</AutoCompleteBox>
```

### Context Menu Commands

Add corresponding commands to ViewModel:

```csharp
public class RemoveItemViewModel : BaseViewModel
{
    // Add these commands
    public ICommand TriggerPartSuggestionsCommand { get; private set; } = null!;
    public ICommand ClearPartCommand { get; private set; } = null!;
    public ICommand ShowSuggestionHelpCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        // ... existing commands

        // Suggestion-related commands
        TriggerPartSuggestionsCommand = new AsyncCommand(
            async () => await CheckPartSuggestionsAsync(),
            () => !string.IsNullOrWhiteSpace(PartText)
        );

        ClearPartCommand = new RelayCommand(() =>
        {
            PartText = string.Empty;
            SelectedPart = null;
        });

        ShowSuggestionHelpCommand = new RelayCommand(() =>
        {
            // TODO: Show help dialog or navigate to help
        });
    }
}
```

</details>

<details>
<summary><strong>🎨 Step 6: Visual Feedback Integration</strong></summary>

### Suggestion Status Indicators

Add visual indicators to show suggestion availability:

```xml
<!-- Part ID with suggestion indicator -->
<Grid ColumnDefinitions="*,Auto" ColumnSpacing="4">
    <AutoCompleteBox Grid.Column="0"
                     ItemsSource="{Binding PartOptions}"
                     SelectedItem="{Binding SelectedPart, Mode=TwoWay}"
                     Text="{Binding PartText, Mode=TwoWay}"
                     Classes="input-field"
                     x:Name="PartAutoCompleteBox"/>
    
    <!-- Suggestion availability indicator -->
    <Border Grid.Column="1"
            Background="{DynamicResource MTM_Shared_Logic.InfoBrush}"
            CornerRadius="8"
            Width="16"
            Height="16"
            VerticalAlignment="Center"
            IsVisible="{Binding HasPartSuggestions}"
            ToolTip.Tip="Suggestions available - click for options">
        <materialIcons:MaterialIcon Kind="Lightbulb"
                                    Width="10"
                                    Height="10"
                                    Foreground="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"
                                    HorizontalAlignment="Center"
                                    VerticalAlignment="Center"/>
    </Border>
</Grid>
```

### Input Validation Feedback

Add visual feedback for input validation:

```xml
<Style Selector="AutoCompleteBox.has-suggestions">
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.InfoBrush}"/>
    <Setter Property="BorderThickness" Value="2"/>
</Style>

<Style Selector="AutoCompleteBox.no-match">
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.WarningBrush}"/>
    <Setter Property="BorderThickness" Value="2"/>
</Style>

<Style Selector="AutoCompleteBox.exact-match">
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.SuccessBrush}"/>
    <Setter Property="BorderThickness" Value="2"/>
</Style>
```

### ViewModel Properties for Visual States

```csharp
public class RemoveItemViewModel : BaseViewModel
{
    private bool _hasPartSuggestions;
    public bool HasPartSuggestions
    {
        get => _hasPartSuggestions;
        set => SetProperty(ref _hasPartSuggestions, value);
    }

    private string _partValidationState = "normal";
    public string PartValidationState
    {
        get => _partValidationState;
        set => SetProperty(ref _partValidationState, value);
    }

    private async Task CheckPartSuggestionsAsync()
    {
        if (string.IsNullOrWhiteSpace(PartText))
        {
            HasPartSuggestions = false;
            PartValidationState = "normal";
            return;
        }

        if (PartOptions.Contains(PartText))
        {
            HasPartSuggestions = false;
            PartValidationState = "exact-match";
            return;
        }

        var suggestions = FindSimilarParts(PartText);
        HasPartSuggestions = suggestions.Any();
        PartValidationState = HasPartSuggestions ? "has-suggestions" : "no-match";

        if (HasPartSuggestions)
        {
            await ShowSuggestionsAsync(suggestions, "Part", PartText);
        }
    }
}
```

</details>

<details>
<summary><strong>🧪 Step 7: Testing and Validation</strong></summary>

### Integration Testing Checklist

#### Functional Testing
- [ ] **Suggestion Triggers**: Verify suggestions appear when expected
- [ ] **Selection Handling**: Confirm suggestion selection updates fields correctly
- [ ] **Cancellation**: Ensure overlay closes properly when cancelled
- [ ] **Keyboard Navigation**: Test Enter/Escape key functionality
- [ ] **Context Menu**: Verify right-click options work correctly
- [ ] **Visual Feedback**: Check indicator states change appropriately

#### Performance Testing
- [ ] **Suggestion Speed**: Verify suggestions appear within reasonable time
- [ ] **Memory Usage**: Check for memory leaks in overlay creation/disposal
- [ ] **Database Impact**: Monitor database calls for suggestion queries
- [ ] **UI Responsiveness**: Ensure UI remains responsive during suggestion processing

#### Integration Testing
- [ ] **Existing Functionality**: Verify original AutoCompleteBox behavior is preserved
- [ ] **Theme Compatibility**: Test across different MTM themes
- [ ] **Multiple Fields**: Test suggestion overlays on multiple fields simultaneously
- [ ] **Data Consistency**: Verify suggestions match actual database records

### Test Scenarios

```csharp
// Example test cases for integration
public class SuggestionIntegrationTests
{
    [Test]
    public async Task PartSuggestion_WhenTypoInPartId_ShowsSimilarParts()
    {
        // Arrange
        var viewModel = CreateTestViewModel();
        viewModel.PartText = "PART01"; // Typo in "PART001"

        // Act
        await viewModel.CheckPartSuggestionsAsync();

        // Assert
        Assert.IsTrue(viewModel.IsOverlayVisible);
        Assert.IsNotNull(viewModel.SuggestionOverlay);
        Assert.Contains("PART001", viewModel.SuggestionOverlay.Suggestions);
    }

    [Test]
    public void SuggestionSelection_WhenPartSelected_UpdatesFields()
    {
        // Arrange
        var viewModel = CreateTestViewModel();
        viewModel.SuggestionOverlay = new SuggestionOverlayViewModel(new[] { "PART001" });

        // Act
        viewModel.OnSuggestionSelected("PART001");

        // Assert
        Assert.AreEqual("PART001", viewModel.PartText);
        Assert.AreEqual("PART001", viewModel.SelectedPart);
        Assert.IsFalse(viewModel.IsOverlayVisible);
    }
}
```

</details>

<details>
<summary><strong>🚀 Step 8: Deployment and Rollout</strong></summary>

### Gradual Rollout Strategy

1. **Phase 1**: Enable suggestions for Part ID field only
2. **Phase 2**: Add Operation field suggestions
3. **Phase 3**: Extend to additional fields (Location, etc.)
4. **Phase 4**: Add advanced features (context menus, visual indicators)

### Configuration Options

Add configuration settings for suggestion behavior:

```json
// In appsettings.json
{
  "SuggestionSettings": {
    "EnableSuggestions": true,
    "SimilarityThreshold": 0.6,
    "MaxSuggestions": 5,
    "ShowVisualIndicators": true,
    "EnableContextMenus": true,
    "DelayBeforeSuggestions": 500
  }
}
```

### User Training

Provide user documentation covering:
- How suggestion overlays work
- Keyboard shortcuts (Enter, Escape)
- Context menu options
- Visual indicator meanings
- Benefits and best practices

### Monitoring and Feedback

Implement analytics to track:
- Suggestion usage frequency
- Suggestion accuracy/user acceptance rate
- Performance metrics
- User feedback and issues

</details>

<details>
<summary><strong>📋 Step 9: Implementation Checklist</strong></summary>

### Pre-Implementation
- [ ] **Review target view** structure and identify integration points
- [ ] **Verify SuggestionOverlayView** is implemented and working
- [ ] **Check ViewModel architecture** supports property change notifications
- [ ] **Confirm database services** are available for suggestion queries

### ViewModel Integration
- [ ] **Add overlay properties** (SuggestionOverlay, IsOverlayVisible)
- [ ] **Implement suggestion methods** (CheckPartSuggestionsAsync, etc.)
- [ ] **Add fuzzy matching logic** (LevenshteinDistance, CalculateSimilarity)
- [ ] **Setup event handlers** for suggestion lifecycle
- [ ] **Add context tracking** for multiple field support

### AXAML Integration  
- [ ] **Update namespace declarations** for overlay components
- [ ] **Modify main Grid structure** to support overlay layer
- [ ] **Add overlay container** with backdrop and centering
- [ ] **Enhance AutoCompleteBox controls** with event handlers
- [ ] **Apply visual feedback styles** and indicators

### Code-Behind Integration
- [ ] **Add focus event handlers** for suggestion triggers
- [ ] **Implement keyboard shortcuts** (Escape, F1)
- [ ] **Add manual trigger methods** for programmatic access
- [ ] **Include proper null checks** and type validation

### Advanced Features
- [ ] **Implement custom suggestion provider** (optional)
- [ ] **Add context menu support** with suggestion options
- [ ] **Include visual feedback indicators** for suggestion states
- [ ] **Setup configuration options** for suggestion behavior

### Testing and Validation
- [ ] **Test suggestion triggers** across all integrated fields
- [ ] **Verify selection handling** updates fields correctly
- [ ] **Check overlay disposal** prevents memory leaks
- [ ] **Validate keyboard navigation** works as expected
- [ ] **Test visual feedback** shows appropriate states
- [ ] **Verify existing functionality** remains unchanged

### Documentation and Training
- [ ] **Update user documentation** with suggestion features
- [ ] **Create developer guides** for future integrations
- [ ] **Document configuration options** and customization
- [ ] **Prepare training materials** for end users

</details>

---

This comprehensive guide provides complete implementation details for integrating SuggestionOverlayView into existing views within the MTM WIP Application. Following these patterns ensures consistent behavior, maintainable code, and excellent user experience across all form inputs that benefit from intelligent suggestions.
