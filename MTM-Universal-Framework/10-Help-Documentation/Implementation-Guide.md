# MTM Universal Framework - Complete Implementation Guide

## 🚀 Quick Start (5-Minute Setup)

### Prerequisites
- .NET 8 SDK installed
- Visual Studio 2022 or JetBrains Rider
- Android SDK (for Android development)

### 1. Install Framework Templates

```bash
# Install MTM Universal Framework templates
dotnet new install MTM.UniversalFramework.Templates

# Verify installation
dotnet new list | findstr mtm
```

### 2. Create Your First Application

```bash
# Create new cross-platform business application
dotnet new mtm-business-app -n MyBusinessApp -o ./MyBusinessApp --BusinessDomain "Inventory"

# Navigate to project
cd MyBusinessApp

# Restore packages
dotnet restore

# Run on Windows
dotnet run --framework net8.0

# Build for Android (requires Android SDK)
dotnet build -t:Run --framework net8.0-android
```

### 3. Project Structure Overview

```
MyBusinessApp/
├── ViewModels/              # MVVM Community Toolkit ViewModels
│   ├── MainViewModel.cs     # Main application ViewModel
│   └── Business/            # Business-specific ViewModels
├── Views/                   # Avalonia AXAML Views
│   ├── MainWindow.axaml     # Main application window
│   └── Business/            # Business-specific Views
├── Services/                # Business services
│   ├── IBusinessService.cs  # Service interfaces
│   └── BusinessService.cs   # Service implementations
├── Models/                  # Data models
│   └── BusinessItem.cs      # Business domain models
├── App.axaml                # Application resources
├── Program.cs               # Application entry point
└── appsettings.json         # Configuration
```

## 📖 Architecture Deep Dive

### MVVM Community Toolkit Integration

The framework leverages source generators for optimal performance:

```csharp
[ObservableObject]
public partial class BusinessViewModel : UniversalBaseViewModel
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private bool isActive = true;

    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteAsync(async () =>
        {
            var item = new BusinessItem { Name = Name, IsActive = IsActive };
            await _businessService.SaveAsync(item);
            SetSuccess($"Successfully saved {Name}");
        });
    }

    // CanExecute automatically managed
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task DeleteAsync()
    {
        // Implementation
    }

    private bool CanSave => !IsLoading && !string.IsNullOrWhiteSpace(Name);
}
```

### Service-Oriented Architecture

Services are registered with dependency injection:

```csharp
// Program.cs - Service registration
var builder = Host.CreateApplicationBuilder(args);

// Register MTM Universal Framework services
builder.Services.AddMTMUniversalServices(builder.Configuration);

// Register business-specific services
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IDataRepository, DatabaseRepository>();

// Register ViewModels
builder.Services.AddTransient<BusinessViewModel>();
builder.Services.AddTransient<MainViewModel>();

var app = builder.Build();
```

### Cross-Platform Configuration

The framework automatically adapts to platform capabilities:

```json
{
  "Universal": {
    "DatabaseProvider": "SQLite",
    "EnableOfflineMode": true,
    "Platform": {
      "Windows": {
        "EnableFileAssociations": true,
        "UseNativeDialogs": true
      },
      "Android": {
        "UseStorageAccessFramework": true,
        "EnableBackgroundSync": true
      }
    }
  }
}
```

## 🎨 UI Development Patterns

### Responsive Layout Design

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MyApp.Views.BusinessView">
  
  <!-- Responsive grid that adapts to screen size -->
  <Grid RowDefinitions="Auto,*,Auto" ColumnDefinitions="*">
    
    <!-- Header -->
    <StackPanel Grid.Row="0" Orientation="Horizontal" Spacing="16">
      <TextBlock Text="Business Management" 
                 Classes="header-text"/>
      <Button Content="Add New" 
              Classes="primary"
              Command="{Binding AddCommand}"/>
    </StackPanel>
    
    <!-- Content Area with Universal Card -->
    <ScrollViewer Grid.Row="1" Margin="0,16">
      <ItemsControl ItemsSource="{Binding BusinessItems}">
        <ItemsControl.ItemTemplate>
          <DataTemplate>
            <controls:UniversalCard Title="{Binding Name}"
                                   Subtitle="{Binding Description}"
                                   IsSelectable="True"
                                   IsSelected="{Binding IsSelected}">
              <controls:UniversalCard.ActionContent>
                <StackPanel Orientation="Horizontal" Spacing="8">
                  <Button Content="Edit" Classes="secondary"/>
                  <Button Content="Delete" Classes="danger"/>
                </StackPanel>
              </controls:UniversalCard.ActionContent>
            </controls:UniversalCard>
          </DataTemplate>
        </ItemsControl.ItemTemplate>
      </ItemsControl>
    </ScrollViewer>
    
    <!-- Status Bar -->
    <Border Grid.Row="2" Background="{DynamicResource Universal_CardBrush}">
      <TextBlock Text="{Binding StatusMessage}" Margin="16,8"/>
    </Border>
    
  </Grid>
</UserControl>
```

### Theme Customization

Create custom themes by extending the universal theme:

```xml
<ResourceDictionary xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  
  <!-- Merge universal theme -->
  <ResourceDictionary.MergedDictionaries>
    <ResourceInclude Source="/MTM.Avalonia/Themes/Universal_Blue.axaml"/>
  </ResourceDictionary.MergedDictionaries>
  
  <!-- Override colors for custom branding -->
  <Color x:Key="Universal_Primary">#8B5CF6</Color> <!-- Purple -->
  <Color x:Key="Universal_Success">#10B981</Color> <!-- Emerald -->
  
</ResourceDictionary>
```

## 🗄️ Database Integration

### Universal Data Service

```csharp
public class BusinessService : IBusinessService
{
    private readonly IUniversalRepository<BusinessItem> _repository;
    private readonly ILogger<BusinessService> _logger;

    public BusinessService(
        IUniversalRepository<BusinessItem> repository,
        ILogger<BusinessService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> SaveAsync(BusinessItem item)
    {
        try
        {
            if (item.Id == 0)
            {
                await _repository.AddAsync(item);
            }
            else
            {
                await _repository.UpdateAsync(item);
            }

            _logger.LogInformation("Business item saved: {Name}", item.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving business item: {Name}", item.Name);
            return false;
        }
    }

    public async Task<IEnumerable<BusinessItem>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}
```

### Multi-Provider Database Support

```csharp
// Configure database provider in Program.cs
builder.Services.AddUniversalDatabase(builder.Configuration, options =>
{
    // SQLite for mobile/desktop
    options.UseSQLite("Data Source=business.db");
    
    // Or PostgreSQL for server applications
    // options.UsePostgreSQL(connectionString);
    
    // Or SQL Server for enterprise applications
    // options.UseSqlServer(connectionString);
});
```

## 🧪 Testing Your Application

### ViewModel Unit Testing

```csharp
[TestFixture]
public class BusinessViewModelTests : TestBase
{
    [Test]
    public async Task SaveCommand_ValidItem_SavesSuccessfully()
    {
        // Arrange
        var mockService = new Mock<IBusinessService>();
        var viewModel = new BusinessViewModel(mockService.Object, Logger);
        
        viewModel.Name = "Test Business";
        mockService.Setup(s => s.SaveAsync(It.IsAny<BusinessItem>()))
                  .ReturnsAsync(true);

        // Act
        await viewModel.SaveCommand.ExecuteAsync();

        // Assert
        mockService.Verify(s => s.SaveAsync(It.IsAny<BusinessItem>()), Times.Once);
        Assert.That(viewModel.StatusMessage, Contains.Substring("success"));
    }
}
```

### Integration Testing

```csharp
[TestFixture]
[Category("Integration")]
public class BusinessServiceIntegrationTests : IntegrationTestBase
{
    [Test]
    public async Task BusinessService_SaveAndRetrieve_WorksCorrectly()
    {
        // Arrange
        var service = ServiceProvider.GetRequiredService<IBusinessService>();
        var item = new BusinessItem { Name = "Integration Test" };

        // Act
        var saved = await service.SaveAsync(item);
        var retrieved = await service.GetAllAsync();

        // Assert
        Assert.That(saved, Is.True);
        Assert.That(retrieved.Any(i => i.Name == "Integration Test"), Is.True);
    }
}
```

## 🚀 Deployment Strategies

### Windows Desktop Deployment

```bash
# Create Windows executable
dotnet publish -c Release -r win-x64 --self-contained

# Create MSI installer (requires WiX)
dotnet build -c Release -p:PublishProfile=WindowsInstaller
```

### Android Deployment

```bash
# Build APK for testing
dotnet build -t:Run --framework net8.0-android -c Release

# Build AAB for Play Store
dotnet publish -f net8.0-android -c Release -p:AndroidPackageFormat=aab
```

### Cross-Platform CI/CD

```yaml
# .github/workflows/build.yml
name: Build and Test

on: [push, pull_request]

jobs:
  test:
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
        
    steps:
    - uses: actions/checkout@v4
    - uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test
      run: dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```

## 🎯 Best Practices

### 1. ViewModel Design
- Inherit from `UniversalBaseViewModel` for common functionality
- Use `[ObservableProperty]` for properties with change notification
- Use `[RelayCommand]` with proper `CanExecute` logic
- Implement proper disposal for resource cleanup

### 2. Service Design
- Follow single responsibility principle
- Use dependency injection for all dependencies
- Implement comprehensive error handling and logging
- Provide both synchronous and asynchronous APIs where appropriate

### 3. Cross-Platform Considerations
- Test on all target platforms regularly
- Use platform abstractions for platform-specific features
- Handle different screen sizes and input methods
- Consider offline scenarios for mobile applications

### 4. Performance Optimization
- Use lazy loading for large datasets
- Implement proper data virtualization for lists
- Cache frequently accessed data appropriately
- Monitor memory usage and dispose resources properly

This comprehensive guide provides everything needed to build enterprise-grade applications using the MTM Universal Framework.