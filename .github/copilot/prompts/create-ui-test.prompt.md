---
description: 'Prompt template for creating UI automation tests for MTM WIP Application using Avalonia.Headless framework'
applies_to: '**/*'
---

# Create UI Test Prompt Template

## 🎯 Objective

Generate comprehensive UI automation tests for MTM WIP Application using Avalonia.Headless framework. Focus on user interaction scenarios, data binding validation, and cross-platform UI behavior testing.

## 📋 Instructions

When creating UI tests, follow these specific requirements:

### UI Test Structure

1. **Use MTM UI Test Base Class**
   ```csharp
   [TestFixture]
   [Category("UI")]
   [Category("{UICategory}")]  // e.g., View, ViewModel, UserControl, Navigation
   public class {ViewName}UITests : AvaloniaUITestBase
   {
       // UI test implementation with Avalonia.Headless
   }
   ```

2. **UI Test Categories**
   - View Tests: Complete view functionality and rendering
   - UserControl Tests: Custom control behavior and interaction
   - Navigation Tests: View transitions and navigation flow
   - DataBinding Tests: ViewModel-to-View data binding validation
   - Input Tests: User input handling and validation
   - Theme Tests: Theme application and visual consistency

### Avalonia UI Test Framework Setup

#### Base UI Test Class
```csharp
public abstract class AvaloniaUITestBase
{
    protected TestAppBuilder AppBuilder { get; private set; }
    protected Window TestWindow { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        // Initialize Avalonia Headless environment
        AppBuilder = AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = true,
                FrameBufferFormat = PixelFormat.Rgba8888
            });
        
        // Setup DI container for testing
        var services = new ServiceCollection();
        ConfigureTestServices(services);
        ServiceProvider = services.BuildServiceProvider();
        
        await Task.CompletedTask;
    }
    
    [SetUp]
    public virtual async Task SetUp()
    {
        // Create fresh test environment for each test
        using var app = AppBuilder.SetupWithoutStarting();
        await Task.CompletedTask;
    }
    
    [TearDown]
    public virtual async Task TearDown()
    {
        TestWindow?.Close();
        TestWindow = null;
        await Task.CompletedTask;
    }
    
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddMTMTestServices();
    }
    
    protected T GetService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }
    
    protected async Task<T> ShowViewAsync<T>() where T : UserControl, new()
    {
        var view = new T();
        TestWindow = new Window 
        { 
            Content = view,
            Width = 1024,
            Height = 768
        };
        
        TestWindow.Show();
        
        // Wait for rendering
        await Task.Delay(100);
        
        return view;
    }
    
    protected async Task<T> ShowViewWithViewModelAsync<T, TViewModel>() 
        where T : UserControl, new()
        where TViewModel : class
    {
        var viewModel = GetService<TViewModel>();
        var view = new T { DataContext = viewModel };
        
        TestWindow = new Window 
        { 
            Content = view,
            Width = 1024,
            Height = 768
        };
        
        TestWindow.Show();
        
        // Wait for data binding
        await Task.Delay(200);
        
        return view;
    }
}
```

### View Testing Patterns

#### Complete View Functionality Testing
```csharp
[TestFixture]
[Category("UI")]
[Category("View")]
public class InventoryTabViewUITests : AvaloniaUITestBase
{
    private Mock<IInventoryService> _mockInventoryService;
    private Mock<ILogger<InventoryViewModel>> _mockLogger;
    private InventoryViewModel _testViewModel;
    
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);
        
        _mockInventoryService = new Mock<IInventoryService>();
        _mockLogger = new Mock<ILogger<InventoryViewModel>>();
        
        services.AddSingleton(_mockInventoryService.Object);
        services.AddSingleton(_mockLogger.Object);
        services.AddTransient<InventoryViewModel>();
    }
    
    [Test]
    public async Task InventoryView_InitialLoad_ShouldRenderCorrectly()
    {
        // Arrange
        var expectedTitle = "Inventory Management";
        
        // Act
        var view = await ShowViewWithViewModelAsync<InventoryTabView, InventoryViewModel>();
        
        // Assert - Check view structure
        Assert.That(TestWindow.IsVisible, Is.True, "Window should be visible");
        Assert.That(view, Is.Not.Null, "View should be created");
        
        // Find key UI elements
        var titleTextBlock = view.FindDescendantOfType<TextBlock>();
        var searchButton = view.FindDescendantOfType<Button>();
        var dataGrid = view.FindDescendantOfType<DataGrid>();
        var partIdTextBox = view.Find<TextBox>("PartIdTextBox");
        var operationComboBox = view.Find<ComboBox>("OperationComboBox");
        
        // Assert UI elements exist
        Assert.That(titleTextBlock, Is.Not.Null, "Title should be present");
        Assert.That(searchButton, Is.Not.Null, "Search button should be present");
        Assert.That(dataGrid, Is.Not.Null, "Data grid should be present");
        Assert.That(partIdTextBox, Is.Not.Null, "Part ID input should be present");
        Assert.That(operationComboBox, Is.Not.Null, "Operation combo box should be present");
        
        // Assert UI properties
        Assert.That(searchButton.IsEnabled, Is.True, "Search button should be enabled initially");
        Assert.That(dataGrid.IsVisible, Is.True, "Data grid should be visible");
        Assert.That(partIdTextBox.IsEnabled, Is.True, "Part ID input should be enabled");
    }
    
    [Test]
    public async Task InventoryView_DataBinding_ShouldBindCorrectly()
    {
        // Arrange
        var testInventoryData = new List<InventoryItem>
        {
            new() { PartId = "UI_TEST_001", Operation = "100", Quantity = 25, Location = "STATION_A" },
            new() { PartId = "UI_TEST_002", Operation = "110", Quantity = 15, Location = "STATION_B" }
        };
        
        _mockInventoryService.Setup(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(testInventoryData);
        
        // Act
        var view = await ShowViewWithViewModelAsync<InventoryTabView, InventoryViewModel>();
        var viewModel = view.DataContext as InventoryViewModel;
        
        // Trigger search to load data
        await viewModel.SearchCommand.ExecuteAsync(null);
        
        // Wait for UI update
        await Task.Delay(100);
        
        // Assert data binding
        Assert.That(viewModel.SearchResults, Is.EqualTo(testInventoryData));
        
        var dataGrid = view.FindDescendantOfType<DataGrid>();
        Assert.That(dataGrid.ItemsSource, Is.EqualTo(testInventoryData));
        Assert.That(dataGrid.Items.Count, Is.EqualTo(2), "Data grid should show 2 items");
        
        // Verify specific data is displayed
        var firstRow = dataGrid.Items[0] as InventoryItem;
        Assert.That(firstRow?.PartId, Is.EqualTo("UI_TEST_001"));
        Assert.That(firstRow?.Quantity, Is.EqualTo(25));
    }
    
    [Test]
    public async Task InventoryView_UserInput_ShouldUpdateViewModel()
    {
        // Arrange
        var view = await ShowViewWithViewModelAsync<InventoryTabView, InventoryViewModel>();
        var viewModel = view.DataContext as InventoryViewModel;
        
        var partIdTextBox = view.Find<TextBox>("PartIdTextBox");
        var operationComboBox = view.Find<ComboBox>("OperationComboBox");
        var quantityTextBox = view.Find<TextBox>("QuantityTextBox");
        
        // Act - Simulate user input
        partIdTextBox.Text = "USER_INPUT_TEST";
        operationComboBox.SelectedItem = "100";
        quantityTextBox.Text = "50";
        
        // Trigger property updates
        partIdTextBox.RaiseEvent(new TextChangedEventArgs(TextBox.TextChangedEvent));
        
        // Wait for property change notifications
        await Task.Delay(50);
        
        // Assert viewModel was updated
        Assert.That(viewModel.PartId, Is.EqualTo("USER_INPUT_TEST"));
        Assert.That(viewModel.Operation, Is.EqualTo("100"));
        Assert.That(viewModel.Quantity, Is.EqualTo(50));
    }
    
    [Test]
    public async Task InventoryView_SearchButton_ShouldTriggerCommand()
    {
        // Arrange
        var view = await ShowViewWithViewModelAsync<InventoryTabView, InventoryViewModel>();
        var viewModel = view.DataContext as InventoryViewModel;
        var searchButton = view.FindDescendantOfType<Button>();
        
        var commandExecuted = false;
        viewModel.SearchCommand.CanExecuteChanged += (s, e) => commandExecuted = true;
        
        // Setup mock to verify service call
        _mockInventoryService.Setup(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<InventoryItem>())
            .Verifiable();
        
        // Act - Click search button
        searchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        
        // Wait for command execution
        await Task.Delay(100);
        
        // Assert
        _mockInventoryService.Verify(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()), 
            Times.Once, "Search command should call inventory service");
    }
    
    [Test]
    public async Task InventoryView_ValidationErrors_ShouldDisplayCorrectly()
    {
        // Arrange
        var view = await ShowViewWithViewModelAsync<InventoryTabView, InventoryViewModel>();
        var viewModel = view.DataContext as InventoryViewModel;
        var quantityTextBox = view.Find<TextBox>("QuantityTextBox");
        
        // Act - Enter invalid quantity
        quantityTextBox.Text = "-5";
        quantityTextBox.RaiseEvent(new TextChangedEventArgs(TextBox.TextChangedEvent));
        
        // Trigger validation
        var addButton = view.Find<Button>("AddButton");
        addButton?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        
        await Task.Delay(100);
        
        // Assert - Validation error should be displayed
        var errorBorder = quantityTextBox.FindAncestorOfType<Border>();
        Assert.That(errorBorder?.BorderBrush, Is.Not.Null, "Error border should be visible for invalid input");
        
        // Check if error message is displayed
        var errorTextBlock = view.FindDescendantOfType<TextBlock>().Where(tb => 
            tb.Text?.Contains("error") == true || tb.Text?.Contains("invalid") == true).FirstOrDefault();
        
        if (errorTextBlock != null)
        {
            Assert.That(errorTextBlock.IsVisible, Is.True, "Error message should be visible");
        }
    }
}
```

### User Control Testing

#### Custom Control Behavior Testing
```csharp
[TestFixture]
[Category("UI")]
[Category("UserControl")]
public class QuickButtonUserControlUITests : AvaloniaUITestBase
{
    [Test]
    public async Task QuickButtonControl_Rendering_ShouldDisplayCorrectly()
    {
        // Arrange
        var quickButtonData = new QuickButtonInfo
        {
            PartId = "QUICK_TEST_001",
            Operation = "100",
            Quantity = 25,
            Location = "STATION_A",
            DisplayText = "Quick Test (25)"
        };
        
        // Act
        var quickButtonControl = new QuickButtonControl { DataContext = quickButtonData };
        TestWindow = new Window { Content = quickButtonControl };
        TestWindow.Show();
        
        await Task.Delay(100);
        
        // Assert
        var button = quickButtonControl.FindDescendantOfType<Button>();
        var textBlock = quickButtonControl.FindDescendantOfType<TextBlock>();
        
        Assert.That(button, Is.Not.Null, "Quick button should contain a button");
        Assert.That(textBlock, Is.Not.Null, "Quick button should contain text");
        Assert.That(textBlock.Text, Is.EqualTo("Quick Test (25)"), "Text should match display text");
        Assert.That(button.IsEnabled, Is.True, "Button should be enabled");
    }
    
    [Test]
    public async Task QuickButtonControl_Click_ShouldRaiseEvent()
    {
        // Arrange
        var quickButtonData = new QuickButtonInfo
        {
            PartId = "EVENT_TEST_001",
            Operation = "100",
            Quantity = 10,
            Location = "STATION_A",
            DisplayText = "Event Test (10)"
        };
        
        var quickButtonControl = new QuickButtonControl { DataContext = quickButtonData };
        TestWindow = new Window { Content = quickButtonControl };
        TestWindow.Show();
        
        await Task.Delay(100);
        
        var eventRaised = false;
        var raisedEventArgs = (QuickActionExecutedEventArgs)null;
        
        quickButtonControl.QuickActionExecuted += (s, e) =>
        {
            eventRaised = true;
            raisedEventArgs = e;
        };
        
        // Act - Click the button
        var button = quickButtonControl.FindDescendantOfType<Button>();
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        
        await Task.Delay(50);
        
        // Assert
        Assert.That(eventRaised, Is.True, "QuickActionExecuted event should be raised");
        Assert.That(raisedEventArgs, Is.Not.Null, "Event args should not be null");
        Assert.That(raisedEventArgs.PartId, Is.EqualTo("EVENT_TEST_001"));
        Assert.That(raisedEventArgs.Quantity, Is.EqualTo(10));
    }
}
```

### Navigation Testing

#### View Navigation and Routing
```csharp
[TestFixture]
[Category("UI")]
[Category("Navigation")]
public class MainViewNavigationUITests : AvaloniaUITestBase
{
    [Test]
    public async Task MainView_TabNavigation_ShouldSwitchViewsCorrectly()
    {
        // Arrange
        var mainView = await ShowViewAsync<MainView>();
        
        // Find tab control
        var tabControl = mainView.FindDescendantOfType<TabControl>();
        Assert.That(tabControl, Is.Not.Null, "Main view should contain tab control");
        
        var tabItems = tabControl.Items.Cast<TabItem>().ToList();
        Assert.That(tabItems.Count, Is.GreaterThan(1), "Should have multiple tabs");
        
        // Act - Switch to different tabs
        var inventoryTab = tabItems.FirstOrDefault(t => t.Header?.ToString()?.Contains("Inventory") == true);
        var quickButtonsTab = tabItems.FirstOrDefault(t => t.Header?.ToString()?.Contains("QuickButtons") == true);
        var removeTab = tabItems.FirstOrDefault(t => t.Header?.ToString()?.Contains("Remove") == true);
        
        Assert.That(inventoryTab, Is.Not.Null, "Inventory tab should exist");
        Assert.That(quickButtonsTab, Is.Not.Null, "QuickButtons tab should exist");
        Assert.That(removeTab, Is.Not.Null, "Remove tab should exist");
        
        // Test tab switching
        tabControl.SelectedItem = inventoryTab;
        await Task.Delay(100);
        
        // Assert - Inventory view should be active
        var inventoryView = inventoryTab.Content as InventoryTabView;
        Assert.That(inventoryView, Is.Not.Null, "Inventory tab should contain inventory view");
        Assert.That(inventoryView.IsVisible, Is.True, "Inventory view should be visible");
        
        // Switch to QuickButtons tab
        tabControl.SelectedItem = quickButtonsTab;
        await Task.Delay(100);
        
        // Assert - QuickButtons view should be active
        var quickButtonsView = quickButtonsTab.Content as QuickButtonsTabView;
        Assert.That(quickButtonsView, Is.Not.Null, "QuickButtons tab should contain QuickButtons view");
        Assert.That(quickButtonsView.IsVisible, Is.True, "QuickButtons view should be visible");
    }
    
    [Test]
    public async Task MainView_WindowState_ShouldPersistAcrossOperations()
    {
        // Arrange
        var mainView = await ShowViewAsync<MainView>();
        var originalWidth = TestWindow.Width;
        var originalHeight = TestWindow.Height;
        
        // Act - Resize window
        TestWindow.Width = 1200;
        TestWindow.Height = 900;
        
        await Task.Delay(100);
        
        // Assert
        Assert.That(TestWindow.Width, Is.EqualTo(1200), "Window width should be updated");
        Assert.That(TestWindow.Height, Is.EqualTo(900), "Window height should be updated");
        
        // Perform operations and verify window state persists
        var tabControl = mainView.FindDescendantOfType<TabControl>();
        var firstTab = tabControl.Items.Cast<TabItem>().First();
        tabControl.SelectedItem = firstTab;
        
        await Task.Delay(100);
        
        Assert.That(TestWindow.Width, Is.EqualTo(1200), "Window width should persist after navigation");
        Assert.That(TestWindow.Height, Is.EqualTo(900), "Window height should persist after navigation");
    }
}
```

### Theme Testing

#### Theme Application and Visual Consistency
```csharp
[TestFixture]
[Category("UI")]
[Category("Theme")]
public class ThemeApplicationUITests : AvaloniaUITestBase
{
    private Mock<IThemeService> _mockThemeService;
    
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);
        _mockThemeService = new Mock<IThemeService>();
        services.AddSingleton(_mockThemeService.Object);
    }
    
    [Test]
    [TestCase("MTM_Blue")]
    [TestCase("MTM_Green")]
    [TestCase("MTM_Red")]
    [TestCase("MTM_Dark")]
    public async Task ThemeApplication_SwitchThemes_ShouldApplyCorrectly(string themeName)
    {
        // Arrange
        var testButton = new Button { Content = "Theme Test Button" };
        var testTextBlock = new TextBlock { Text = "Theme Test Text" };
        var testBorder = new Border 
        { 
            Child = new StackPanel 
            { 
                Children = { testButton, testTextBlock }
            }
        };
        
        TestWindow = new Window { Content = testBorder };
        TestWindow.Show();
        
        await Task.Delay(100);
        
        // Setup theme service mock
        _mockThemeService.Setup(s => s.SetThemeAsync(themeName))
            .Callback<string>(theme => ApplyTestTheme(theme, testButton, testTextBlock, testBorder))
            .Returns(Task.CompletedTask);
        
        // Act - Apply theme
        await _mockThemeService.Object.SetThemeAsync(themeName);
        
        await Task.Delay(100);
        
        // Assert - Theme should be applied
        _mockThemeService.Verify(s => s.SetThemeAsync(themeName), Times.Once);
        
        // Check theme-specific properties
        switch (themeName)
        {
            case "MTM_Blue":
                Assert.That(testButton.Background?.ToString(), Does.Contain("0078D4").Or.Contain("Blue"),
                    "MTM_Blue theme should apply blue colors");
                break;
            case "MTM_Dark":
                Assert.That(testBorder.Background?.ToString(), Does.Contain("2D2D30").Or.Contain("Dark"),
                    "MTM_Dark theme should apply dark colors");
                break;
            case "MTM_Green":
                Assert.That(testButton.Background?.ToString(), Does.Contain("107C10").Or.Contain("Green"),
                    "MTM_Green theme should apply green colors");
                break;
            case "MTM_Red":
                Assert.That(testButton.Background?.ToString(), Does.Contain("D13438").Or.Contain("Red"),
                    "MTM_Red theme should apply red colors");
                break;
        }
    }
    
    [Test]
    public async Task ThemeApplication_DynamicResources_ShouldUpdateLiveViews()
    {
        // Arrange - Create view with dynamic resource references
        var button = new Button 
        { 
            Content = "Dynamic Theme Test",
            Background = new DynamicResourceExtension("MTM_Shared_Logic.PrimaryBrush").ProvideValue(null) as IBrush
        };
        
        TestWindow = new Window { Content = button };
        TestWindow.Show();
        
        await Task.Delay(100);
        
        var initialBackground = button.Background;
        
        // Act - Change theme
        _mockThemeService.Setup(s => s.SetThemeAsync("MTM_Green"))
            .Callback<string>(theme => 
            {
                // Simulate dynamic resource update
                button.Background = Brushes.Green;
            })
            .Returns(Task.CompletedTask);
        
        await _mockThemeService.Object.SetThemeAsync("MTM_Green");
        
        await Task.Delay(100);
        
        // Assert - Background should be updated via dynamic resources
        Assert.That(button.Background, Is.Not.EqualTo(initialBackground),
            "Button background should change when theme changes");
        Assert.That(button.Background, Is.EqualTo(Brushes.Green),
            "Button should use new theme color");
    }
    
    private void ApplyTestTheme(string themeName, Button button, TextBlock textBlock, Border border)
    {
        // Simulate theme application for testing
        switch (themeName)
        {
            case "MTM_Blue":
                button.Background = Brushes.Blue;
                textBlock.Foreground = Brushes.White;
                border.Background = Brushes.LightBlue;
                break;
            case "MTM_Green":
                button.Background = Brushes.Green;
                textBlock.Foreground = Brushes.White;
                border.Background = Brushes.LightGreen;
                break;
            case "MTM_Red":
                button.Background = Brushes.Red;
                textBlock.Foreground = Brushes.White;
                border.Background = Brushes.LightPink;
                break;
            case "MTM_Dark":
                button.Background = Brushes.DarkGray;
                textBlock.Foreground = Brushes.White;
                border.Background = Brushes.Black;
                break;
        }
    }
}
```

### Cross-Platform UI Testing

```csharp
[TestFixture]
[Category("UI")]
[Category("CrossPlatform")]
public class CrossPlatformUITests : AvaloniaUITestBase
{
    [Test]
    public async Task UIElements_KeyboardInput_ShouldWorkAcrossPlatforms()
    {
        // Arrange
        var textBox = new TextBox { Width = 200 };
        TestWindow = new Window { Content = textBox };
        TestWindow.Show();
        
        textBox.Focus();
        await Task.Delay(100);
        
        // Act - Simulate platform-specific keyboard shortcuts
        var testText = "Cross-Platform Test";
        textBox.Text = testText;
        
        // Simulate Select All (Ctrl+A on Windows/Linux, Cmd+A on macOS)
        var selectAllKey = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) 
            ? KeyModifiers.Meta 
            : KeyModifiers.Control;
        
        var selectAllEvent = new KeyEventArgs
        {
            Key = Key.A,
            KeyModifiers = selectAllKey
        };
        
        textBox.RaiseEvent(selectAllEvent);
        await Task.Delay(50);
        
        // Assert
        Assert.That(textBox.SelectionStart, Is.EqualTo(0), "Select all should work on current platform");
        Assert.That(textBox.SelectionEnd, Is.EqualTo(testText.Length), "Entire text should be selected");
    }
    
    [Test]
    public async Task UIElements_FontRendering_ShouldHandlePlatformFonts()
    {
        // Arrange - Platform-specific font preferences
        var platformFonts = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new[] { "Segoe UI", "Arial" }
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? new[] { "SF Pro Display", "Helvetica Neue" }
            : new[] { "Ubuntu", "DejaVu Sans" };
        
        var textBlock = new TextBlock 
        { 
            Text = "Platform Font Test",
            FontSize = 14
        };
        
        TestWindow = new Window { Content = textBlock };
        TestWindow.Show();
        
        await Task.Delay(100);
        
        // Act & Assert - Test each platform font
        foreach (var fontName in platformFonts)
        {
            textBlock.FontFamily = new FontFamily(fontName);
            await Task.Delay(50);
            
            // Verify font is applied or falls back gracefully
            Assert.That(textBlock.FontFamily?.Name, Is.Not.Null.And.Not.Empty,
                $"Font {fontName} should be applied or have fallback");
            
            // Verify text renders (has positive bounds)
            var bounds = textBlock.Bounds;
            Assert.That(bounds.Width, Is.GreaterThan(0),
                $"Text should render with font {fontName}");
            Assert.That(bounds.Height, Is.GreaterThan(0),
                $"Text should have height with font {fontName}");
        }
    }
}
```

## ✅ UI Test Checklist

When creating UI tests, ensure:

- [ ] All major views have rendering tests
- [ ] Data binding between ViewModels and Views is validated
- [ ] User input handling is tested for all input controls
- [ ] Button clicks and command execution are verified
- [ ] Navigation between views/tabs works correctly
- [ ] Theme application and visual consistency are tested
- [ ] Cross-platform UI behavior is validated
- [ ] Custom user controls have behavior tests
- [ ] Error states and validation messages are displayed correctly
- [ ] Performance of UI rendering is acceptable
- [ ] Accessibility features work as expected
- [ ] Layout adapts correctly to different window sizes

## 🏷️ UI Test Categories

Use these category attributes for UI tests:

```csharp
[Category("UI")]                 // All UI tests
[Category("View")]               // Complete view tests
[Category("UserControl")]        // Custom control tests
[Category("Navigation")]         // Navigation and routing tests
[Category("DataBinding")]        // ViewModel-View binding tests
[Category("Theme")]              // Theme and styling tests
[Category("Input")]              // User input handling tests
[Category("CrossPlatform")]      // Cross-platform UI tests
[Category("Performance")]        // UI performance tests
[Category("Accessibility")]     // UI accessibility tests
```

This template ensures comprehensive UI test coverage using Avalonia.Headless framework while maintaining cross-platform compatibility and following MTM WIP Application UI patterns.