# 🔄 Update All Instruction Files: Remove ReactiveUI References and Align with Standard .NET Implementation

## 📋 Overview
Comprehensive update of all `.instruction.md` files throughout the workspace to remove ReactiveUI and Avalonia.ReactiveUI references and align them with the current standard .NET implementation using INotifyPropertyChanged, ICommand, and standard MVVM patterns. Additionally, update database file references and implement new README file policy with HTML-based documentation.

## 🎯 Objectives
- Remove all ReactiveUI references from instruction documentation
- Update all code examples to use standard .NET patterns
- Align documentation with current build implementation
- Ensure consistent messaging across all instruction files
- Update custom prompts to generate standard .NET code
- Maintain instruction file quality and collapsible formatting
- **🗄️ Update SQL file references to correct Documentation/Development/Database_Files structure**
- **📄 Implement new README file policy with HTML-based documentation**
- **🎨 Create Documentation/HTML_Data folder structure for shared CSS/JS assets**

## 📊 Scope

### 🔧 Files Requiring Updates

#### Core Instructions (CRITICAL)
- ✅ `codingconventions.instruction.md` - **PRIORITY 1**: Contains extensive ReactiveUI patterns
- ✅ `dependency-injection.instruction.md` - Service registration patterns  
- ✅ `project-structure.instruction.md` - Project organization standards
- ✅ `naming.conventions.instruction.md` - Naming standards

#### UI Instructions (CRITICAL)
- ✅ `ui-generation.instruction.md` - **PRIORITY 1**: Contains ReactiveUI ViewModel templates
- ✅ `avalonia-xaml-syntax.instruction.md` - AXAML syntax standards
- ✅ `ui-mapping.instruction.md` - Control mapping standards
- ✅ `ui-styling.instruction.md` - MTM design system standards

#### Development Instructions (HIGH PRIORITY)
- ✅ `database-patterns.instruction.md` - **NEEDS REVIEW**: May contain ReactiveUI patterns + **UPDATE DATABASE FILE REFERENCES**
- ✅ `errorhandler.instruction.md` - Error handling patterns
- ✅ `templates-documentation.instruction.md` - **PRIORITY 1**: Contains ReactiveUI template examples
- ✅ `githubworkflow.instruction.md` - Development workflow standards

#### Custom Prompts (CRITICAL)
- ✅ `CustomPrompt_Create_ReactiveUIViewModel.md` - **MUST DELETE or REPLACE**
- ✅ `CustomPrompt_Create_UIElement.md` - Contains ReactiveUI references
- ✅ `CustomPrompt_Create_UIElementFromMarkdown.md` - Contains ReactiveUI patterns
- ✅ `custom-prompts-examples.md` - Contains ReactiveUI example prompts

#### Automation Instructions
- ✅ `customprompts.instruction.md` - Custom prompt guidelines
- ✅ `personas.instruction.md` - Persona definitions
- ✅ `issue-pr-creation.instruction.md` - Issue creation standards

#### Quality Instructions
- ✅ `needsrepair.instruction.md` - Quality standards + **UPDATE README REQUIREMENTS**

#### Documentation Files
- ✅ `Views_Structure_Standards.instruction.md` - May contain ReactiveUI references

### 🗄️ Database File Reference Updates (NEW)

#### Files Requiring Database Reference Corrections
- ✅ `database-patterns.instruction.md` - **Update SQL file paths to Documentation/Development/Database_Files**
- ✅ `project-structure.instruction.md` - **Update database file organization references**
- ✅ Any instruction files referencing SQL scripts or stored procedures

#### Correct Database File Structure References
```markdown
REPLACE REFERENCES TO:
- Old SQL file locations
- Incorrect database file paths
- Outdated stored procedure references

WITH CORRECT REFERENCES TO:
- Documentation/Development/Database_Files/Stored_Procedures/
- Documentation/Development/Database_Files/Table_Scripts/
- Documentation/Development/Database_Files/Database_Schema/
- Documentation/Development/Database_Files/Sample_Data/
```

### 📄 README File Policy Updates (NEW)

#### New README File Requirements
**🚨 CRITICAL POLICY CHANGE**: Remove universal README requirement and implement selective README policy with HTML-based documentation.

#### README Files to Maintain/Create
- ✅ **Root Repository README.md** - Standard markdown format for repository overview
- ✅ **Documentation/Development/Database_Files/README.html** - HTML format with CSS/JS links
- ✅ **Documentation/Development/Database_Files/[SubFolder]/README.html** - Root of each database category folder only
- ✅ **.github/README.html** - HTML format for instruction files overview

#### README Files to Remove/Not Create
- ❌ **Individual sub-folder README files** - No longer required
- ❌ **Every directory README requirement** - Removed from documentation standards
- ❌ **Universal README policy** - Replaced with selective policy

### 🎨 HTML Documentation Infrastructure (NEW)

#### Create Documentation/HTML_Data Structure
```
Documentation/
├── HTML_Data/
│   ├── css/
│   │   ├── mtm-documentation.css      # Main MTM styling with purple theme
│   │   ├── readme-styles.css          # README-specific styles
│   │   ├── database-docs.css          # Database documentation styles
│   │   └── instruction-styles.css     # Instruction file styles
│   ├── js/
│   │   ├── documentation-nav.js       # Navigation functionality
│   │   ├── collapsible-sections.js    # Collapsible section management
│   │   ├── search-functionality.js    # Search and filter capabilities
│   │   └── file-browser.js           # Dynamic file browsing
│   ├── assets/
│   │   ├── mtm-logo.png              # MTM branding
│   │   ├── icons/                    # Documentation icons
│   │   │   ├── database.svg
│   │   │   ├── instruction.svg
│   │   │   └── readme.svg
│   │   └── templates/
│   │       ├── readme-template.html   # Base README template
│   │       └── section-template.html  # Section template
```

#### HTML README Template Structure
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM Documentation - [Section Name]</title>
    <link rel="stylesheet" href="../../HTML_Data/css/mtm-documentation.css">
    <link rel="stylesheet" href="../../HTML_Data/css/readme-styles.css">
</head>
<body>
    <header class="mtm-header">
        <img src="../../HTML_Data/assets/mtm-logo.png" alt="MTM Logo">
        <h1>[Section Title]</h1>
    </header>
    
    <nav class="documentation-nav">
        <!-- Navigation will be populated by JS -->
    </nav>
    
    <main class="content">
        <section class="overview">
            <h2>Overview</h2>
            <p>Plain English explanation of contents...</p>
        </section>
        
        <section class="contents">
            <h2>Contents</h2>
            <!-- Dynamically populated content list -->
        </section>
        
        <section class="usage">
            <h2>Usage Guidelines</h2>
            <!-- How to use these files -->
        </section>
    </main>
    
    <script src="../../HTML_Data/js/documentation-nav.js"></script>
    <script src="../../HTML_Data/js/collapsible-sections.js"></script>
</body>
</html>
```

## 🛠️ Implementation Plan

### Phase 1: Critical ReactiveUI Removal (Days 1-2)
**Priority: CRITICAL - Immediate Impact**

#### 1.1 Update Core Instructions
**File: `.github/Core-Instructions/codingconventions.instruction.md`**
```markdown
REMOVE:
- ReactiveObject base class references
- this.RaiseAndSetIfChanged() patterns
- ReactiveCommand<Unit, Unit> examples
- WhenAnyValue patterns
- ObservableAsPropertyHelper (OAPH) examples
- ReactiveUserControl<T> patterns

REPLACE WITH:
- BaseViewModel with INotifyPropertyChanged
- SetProperty(ref _field, value) patterns
- ICommand (AsyncCommand, RelayCommand) examples
- Standard property change notification
- Standard data binding patterns
- UserControl inheritance
```

**File: `.github/UI-Instructions/ui-generation.instruction.md`**
```markdown
REMOVE:
- All ReactiveUI ViewModel templates
- ReactiveCommand command patterns
- Observable property helpers
- ReactiveUI error handling patterns

REPLACE WITH:
- Standard .NET ViewModel templates using BaseViewModel
- ICommand implementations (AsyncCommand/RelayCommand)
- Standard INotifyPropertyChanged patterns
- Standard exception handling patterns
```

#### 1.2 Delete/Replace ReactiveUI Custom Prompts
**Action: DELETE** `CustomPrompt_Create_ReactiveUIViewModel.md`
**Action: CREATE** `CustomPrompt_Create_StandardViewModel.md`

```markdown
# Create Standard .NET ViewModel - Custom Prompt

## Instructions
Use this prompt when you need to generate ViewModels with standard .NET patterns, ICommand implementations, and INotifyPropertyChanged.

## Prompt Template
```
Generate a standard .NET ViewModel for [Purpose] following MTM patterns.  
Include properties with INotifyPropertyChanged, ICommand implementations with proper error handling,  
standard property validation, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

## Purpose
For generating ViewModels with standard .NET patterns, commands, and observable properties.

### Basic ViewModel Template
```csharp
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Base;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class [Name]ViewModel : BaseViewModel, INotifyPropertyChanged
{
    #region Properties

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _selectedPartId = string.Empty;
    public string SelectedPartId
    {
        get => _selectedPartId;
        set => SetProperty(ref _selectedPartId, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    #endregion

    #region Collections

    public ObservableCollection<InventoryItemViewModel> InventoryItems { get; } = new();
    public ObservableCollection<string> PartIds { get; } = new();
    public ObservableCollection<string> Operations { get; } = new();
    public ObservableCollection<string> Locations { get; } = new();

    #endregion

    #region Commands

    public ICommand LoadDataCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand RefreshCommand { get; private set; }
    public ICommand ClearCommand { get; private set; }

    #endregion

    #region Constructor

    public [Name]ViewModel()
    {
        // TODO: Replace with actual DI constructor
        // public [Name]ViewModel(
        //     IInventoryService inventoryService,
        //     IUserService userService,
        //     IApplicationStateService applicationState)

        InitializeCommands();
    }

    #endregion

    #region Command Initialization

    private void InitializeCommands()
    {
        LoadDataCommand = new AsyncCommand(ExecuteLoadDataAsync);
        SaveCommand = new AsyncCommand(ExecuteSaveAsync, CanExecuteSave);
        RefreshCommand = new AsyncCommand(ExecuteRefreshAsync);
        ClearCommand = new RelayCommand(ExecuteClear);
    }

    #endregion

    #region Command Implementations

    private async Task ExecuteLoadDataAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            
            // TODO: Implement database loading
            await Task.CompletedTask;
            
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "LoadData");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteSaveAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            
            // TODO: Implement save logic via database service
            await Task.CompletedTask;
            
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Save");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanExecuteSave()
    {
        return !string.IsNullOrWhiteSpace(SelectedPartId) && 
               Quantity > 0 && 
               !IsLoading;
    }

    private async Task ExecuteRefreshAsync()
    {
        await ExecuteLoadDataAsync();
    }

    private void ExecuteClear()
    {
        SelectedPartId = string.Empty;
        Quantity = 0;
        HasError = false;
        ErrorMessage = string.Empty;
    }

    #endregion

    #region Error Handling

    private async Task HandleErrorAsync(Exception ex, string operation)
    {
        HasError = true;
        ErrorMessage = GetUserFriendlyErrorMessage(ex);
        
        // TODO: Log error via ErrorHandling service
        await ErrorHandling.HandleErrorAsync(ex, operation, Environment.UserName, 
            new Dictionary<string, object>
            {
                ["PartId"] = SelectedPartId,
                ["Quantity"] = Quantity
            });
    }

    private static string GetUserFriendlyErrorMessage(Exception ex)
    {
        return ex switch
        {
            InvalidOperationException => ex.Message,
            TimeoutException => "The operation timed out. Please try again.",
            UnauthorizedAccessException => "You don't have permission to perform this action.",
            _ => "An unexpected error occurred. Please try again or contact support."
        };
    }

    #endregion

    #region Validation

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(SelectedPartId))
        {
            ErrorMessage = "Part ID is required";
            return false;
        }

        if (Quantity <= 0)
        {
            ErrorMessage = "Quantity must be greater than zero";
            return false;
        }

        return true;
    }

    #endregion
}
```

### Phase 2: Database File Reference Updates (Days 2-3)
**Priority: HIGH - Documentation Accuracy**

#### 2.1 Update Database Pattern References
**File: `.github/Development-Instructions/database-patterns.instruction.md`**
```markdown
UPDATE ALL REFERENCES FROM:
- Incorrect SQL file paths
- Old database file organization

TO CORRECT PATHS:
- Documentation/Development/Database_Files/Stored_Procedures/[Category]/
- Documentation/Development/Database_Files/Table_Scripts/
- Documentation/Development/Database_Files/Database_Schema/
- Documentation/Development/Database_Files/Sample_Data/

SPECIFIC EXAMPLES:
- inv_inventory_Add_Item.sql → Documentation/Development/Database_Files/Stored_Procedures/Inventory/inv_inventory_Add_Item.sql
- md_part_ids_Get_All.sql → Documentation/Development/Database_Files/Stored_Procedures/Master_Data/md_part_ids_Get_All.sql
- usr_users_Get_All.sql → Documentation/Development/Database_Files/Stored_Procedures/User_Management/usr_users_Get_All.sql
```

#### 2.2 Update Project Structure References
**File: `.github/Core-Instructions/project-structure.instruction.md`**
```markdown
UPDATE DATABASE SECTION TO REFLECT:
- Correct Documentation/Development/Database_Files structure
- Proper stored procedure organization by category
- Accurate file path references in documentation
- Remove incorrect database file location references
```

### Phase 3: HTML Documentation Infrastructure (Days 3-4)
**Priority: HIGH - New Documentation System**

#### 3.1 Create Documentation/HTML_Data Structure
```
Documentation/
├── HTML_Data/
│   ├── css/
│   │   ├── mtm-documentation.css      # Main MTM styling with purple theme
│   │   ├── readme-styles.css          # README-specific styles
│   │   ├── database-docs.css          # Database documentation styles
│   │   └── instruction-styles.css     # Instruction file styles
│   ├── js/
│   │   ├── documentation-nav.js       # Navigation functionality
│   │   ├── collapsible-sections.js    # Collapsible section management
│   │   ├── search-functionality.js    # Search and filter capabilities
│   │   └── file-browser.js           # Dynamic file browsing
│   ├── assets/
│   │   ├── mtm-logo.png              # MTM branding
│   │   ├── icons/                    # Documentation icons
│   │   │   ├── database.svg
│   │   │   ├── instruction.svg
│   │   │   └── readme.svg
│   │   └── templates/
│   │       ├── readme-template.html   # Base README template
│   │       └── section-template.html  # Section template
```

#### 3.2 Create CSS Files
**File: `Documentation/HTML_Data/css/mtm-documentation.css`**
```css
/* MTM Documentation Main Styles */
:root {
    --mtm-purple: #6a0dad;
    --mtm-light-purple: #8a2be2;
    --mtm-dark-purple: #4b0082;
    --mtm-gray: #f5f5f5;
    --mtm-text: #333;
    --mtm-white: #ffffff;
}

body {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    margin: 0;
    padding: 0;
    background-color: var(--mtm-gray);
    color: var(--mtm-text);
}

.mtm-header {
    background: linear-gradient(135deg, var(--mtm-purple), var(--mtm-light-purple));
    color: var(--mtm-white);
    padding: 20px;
    display: flex;
    align-items: center;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.mtm-header img {
    height: 50px;
    margin-right: 20px;
}

.mtm-header h1 {
    margin: 0;
    font-size: 2rem;
    font-weight: 300;
}

.documentation-nav {
    background-color: var(--mtm-white);
    padding: 15px;
    border-bottom: 3px solid var(--mtm-purple);
    box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

.content {
    max-width: 1200px;
    margin: 0 auto;
    padding: 30px 20px;
    background-color: var(--mtm-white);
    box-shadow: 0 0 20px rgba(0,0,0,0.1);
    border-radius: 8px;
    margin-top: 20px;
}

.content section {
    margin-bottom: 30px;
}

.content h2 {
    color: var(--mtm-purple);
    border-bottom: 2px solid var(--mtm-light-purple);
    padding-bottom: 10px;
    margin-bottom: 20px;
}

.file-list {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 20px;
    margin-top: 20px;
}

.file-item {
    background-color: var(--mtm-gray);
    padding: 15px;
    border-radius: 8px;
    border-left: 4px solid var(--mtm-purple);
    transition: transform 0.2s, box-shadow 0.2s;
}

.file-item:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 15px rgba(106, 13, 173, 0.2);
}

.file-item h3 {
    margin: 0 0 10px 0;
    color: var(--mtm-purple);
}

.file-item p {
    margin: 0;
    color: var(--mtm-text);
    line-height: 1.5;
}

/* Responsive design */
@media (max-width: 768px) {
    .mtm-header {
        flex-direction: column;
        text-align: center;
    }
    
    .mtm-header img {
        margin: 0 0 10px 0;
    }
    
    .content {
        margin: 10px;
        padding: 20px 15px;
    }
    
    .file-list {
        grid-template-columns: 1fr;
    }
}
```

#### 3.3 Create JavaScript Files
**File: `Documentation/HTML_Data/js/documentation-nav.js`**
```javascript
// MTM Documentation Navigation
document.addEventListener('DOMContentLoaded', function() {
    const nav = document.querySelector('.documentation-nav');
    if (!nav) return;

    // Create navigation structure
    const navItems = [
        { text: 'Database Files', href: '../Database_Files/README.html', icon: 'database' },
        { text: 'Instruction Files', href: '../../.github/README.html', icon: 'instruction' },
        { text: 'Development Docs', href: '../README.html', icon: 'readme' },
        { text: 'Back to Root', href: '../../../README.md', icon: 'home' }
    ];

    const navList = document.createElement('ul');
    navList.className = 'nav-list';

    navItems.forEach(item => {
        const li = document.createElement('li');
        const a = document.createElement('a');
        a.href = item.href;
        a.innerHTML = `<span class="nav-icon ${item.icon}"></span>${item.text}`;
        a.className = 'nav-link';
        li.appendChild(a);
        navList.appendChild(li);
    });

    nav.appendChild(navList);
});
```

#### 3.4 Create README Files
**File: `Documentation/Development/Database_Files/README.html`**
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM Documentation - Database Files</title>
    <link rel="stylesheet" href="../../HTML_Data/css/mtm-documentation.css">
    <link rel="stylesheet" href="../../HTML_Data/css/readme-styles.css">
    <link rel="stylesheet" href="../../HTML_Data/css/database-docs.css">
</head>
<body>
    <header class="mtm-header">
        <img src="../../HTML_Data/assets/mtm-logo.png" alt="MTM Logo">
        <h1>MTM WIP Application - Database Files</h1>
    </header>
    
    <nav class="documentation-nav">
        <!-- Navigation will be populated by JS -->
    </nav>
    
    <main class="content">
        <section class="overview">
            <h2>📁 Database Files Overview</h2>
            <p>This directory contains all database-related files for the MTM WIP Application, including stored procedures, table scripts, database schema definitions, and sample data. All database files are organized by functional category to provide clear separation of concerns and easy navigation.</p>
            
            <p>The MTM WIP Application uses a SQL Server database with a comprehensive set of stored procedures for inventory management, master data management, user management, and system configuration. All database access in the application is performed through these stored procedures using the Helper_Database_StoredProcedure pattern.</p>
        </section>
        
        <section class="contents">
            <h2>📋 Directory Contents</h2>
            <div class="file-list" id="database-file-list">
                <!-- Content will be dynamically populated -->
            </div>
        </section>
        
        <section class="usage">
            <h2>🚀 Usage Guidelines</h2>
            <h3>For Developers:</h3>
            <ul>
                <li><strong>Stored Procedures:</strong> All database operations must use the provided stored procedures via Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()</li>
                <li><strong>Schema Changes:</strong> Use the table scripts and schema files as reference for database structure</li>
                <li><strong>Testing:</strong> Use sample data files for development and testing scenarios</li>
                <li><strong>Organization:</strong> Follow the established category-based folder structure when adding new database files</li>
            </ul>
            
            <h3>For Database Administrators:</h3>
            <ul>
                <li><strong>Deployment:</strong> Execute table scripts first, then stored procedures, finally sample data if needed</li>
                <li><strong>Maintenance:</strong> Use the schema documentation to understand table relationships and constraints</li>
                <li><strong>Backup:</strong> Ensure all custom stored procedures are included in database backups</li>
            </ul>
        </section>
        
        <section class="categories">
            <h2>📂 File Categories</h2>
            <div class="category-grid">
                <div class="category-item">
                    <h3>🔧 Stored Procedures</h3>
                    <p>Complete collection of stored procedures organized by functional area (Inventory, Master Data, User Management, System Configuration)</p>
                </div>
                <div class="category-item">
                    <h3>📊 Table Scripts</h3>
                    <p>SQL scripts for creating and modifying database tables, indexes, and constraints</p>
                </div>
                <div class="category-item">
                    <h3>🏗️ Database Schema</h3>
                    <p>Comprehensive schema documentation including table relationships, data types, and business rules</p>
                </div>
                <div class="category-item">
                    <h3>📝 Sample Data</h3>
                    <p>Test data and reference data for development, testing, and demonstration purposes</p>
                </div>
            </div>
        </section>
    </main>
    
    <script src="../../HTML_Data/js/documentation-nav.js"></script>
    <script src="../../HTML_Data/js/collapsible-sections.js"></script>
    <script src="../../HTML_Data/js/file-browser.js"></script>
</body>
</html>
```

### Phase 4: UI Instructions Update (Days 4-5)
**Priority: HIGH - UI Generation Impact**

#### 4.1 Update UI Generation Patterns
**Files to Update:**
- `ui-generation.instruction.md` - Replace all ReactiveUI templates
- `ui-mapping.instruction.md` - Update ViewModel mapping patterns
- `ui-styling.instruction.md` - Ensure standard .NET compatibility

#### 4.2 Update AXAML Templates
```xml
<!-- REPLACE ReactiveUserControl pattern -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.{Name}View"
             x:CompileBindings="True"
             x:DataType="vm:{Name}ViewModel">
    
    <!-- Standard UserControl, not ReactiveUserControl<T> -->
</UserControl>
```

### Phase 5: Custom Prompts Overhaul (Days 5-6)
**Priority: HIGH - Code Generation Impact**

#### 5.1 Update All Custom Prompt Files
**Files to Update:**
- `CustomPrompt_Create_UIElement.md` - Remove ReactiveUI patterns
- `CustomPrompt_Create_UIElementFromMarkdown.md` - Standard .NET patterns
- `custom-prompts-examples.md` - Update all examples

#### 5.2 Update Example Prompts
```markdown
REPLACE:
"Generate ReactiveUI ViewModel with RaiseAndSetIfChanged patterns"

WITH:
"Generate standard .NET ViewModel with INotifyPropertyChanged patterns"
```

### Phase 6: README Policy Implementation (Days 6-7)
**Priority: MEDIUM - Documentation Standards**

#### 6.1 Update Documentation Standards
**File: `.github/Quality-Instructions/needsrepair.instruction.md`**
```markdown
UPDATE README REQUIREMENTS SECTION:

REMOVE:
- Universal README file requirement for all directories
- Automatic README generation for every folder
- README file requirement for sub-directories

REPLACE WITH:
- Selective README policy (root, main category folders only)
- HTML-based README files with CSS/JS linking
- Plain English, thorough explanations of contents
- Professional presentation with MTM branding
```

#### 6.2 Create Instruction Files README
**File: `.github/README.html`**
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>MTM Documentation - Instruction Files</title>
    <link rel="stylesheet" href="../Documentation/HTML_Data/css/mtm-documentation.css">
    <link rel="stylesheet" href="../Documentation/HTML_Data/css/readme-styles.css">
    <link rel="stylesheet" href="../Documentation/HTML_Data/css/instruction-styles.css">
</head>
<body>
    <header class="mtm-header">
        <img src="../Documentation/HTML_Data/assets/mtm-logo.png" alt="MTM Logo">
        <h1>MTM WIP Application - Instruction Files</h1>
    </header>
    
    <nav class="documentation-nav">
        <!-- Navigation will be populated by JS -->
    </nav>
    
    <main class="content">
        <section class="overview">
            <h2>📚 Instruction Files Overview</h2>
            <p>This directory contains comprehensive instruction files for GitHub Copilot to ensure consistent, high-quality code generation throughout the MTM WIP Application development process. These files define coding standards, UI patterns, development workflows, and quality requirements using standard .NET patterns without ReactiveUI dependencies.</p>
            
            <p>The instruction files are organized into categories covering core infrastructure, UI development, development patterns, automation, and quality assurance. All instructions have been updated to reflect the current standard .NET implementation using INotifyPropertyChanged, ICommand, and modern MVVM patterns.</p>
        </section>
        
        <section class="contents">
            <h2>📁 Instruction Categories</h2>
            <div class="file-list" id="instruction-file-list">
                <!-- Content will be dynamically populated -->
            </div>
        </section>
        
        <section class="usage">
            <h2>🚀 How to Use These Instructions</h2>
            <h3>For Developers:</h3>
            <ul>
                <li><strong>Code Generation:</strong> Reference these instructions when using GitHub Copilot for consistent code patterns</li>
                <li><strong>Standard Compliance:</strong> Follow the coding conventions and naming standards defined in these files</li>
                <li><strong>UI Development:</strong> Use the UI instructions for Avalonia component generation and styling</li>
                <li><strong>Quality Assurance:</strong> Apply the quality standards for code reviews and testing</li>
            </ul>
            
            <h3>For GitHub Copilot:</h3>
            <ul>
                <li><strong>Pattern Reference:</strong> These files provide the context for generating MTM-compliant code</li>
                <li><strong>Standard .NET Focus:</strong> All patterns use INotifyPropertyChanged and ICommand implementations</li>
                <li><strong>Error Handling:</strong> Comprehensive error handling patterns using ErrorHandling service</li>
                <li><strong>Database Access:</strong> Stored procedure patterns using Helper_Database_StoredProcedure</li>
            </ul>
        </section>
    </main>
    
    <script src="../Documentation/HTML_Data/js/documentation-nav.js"></script>
    <script src="../Documentation/HTML_Data/js/collapsible-sections.js"></script>
    <script src="../Documentation/HTML_Data/js/file-browser.js"></script>
</body>
</html>
```

### Phase 7: Quality and Documentation (Days 7-8)
**Priority: LOW - Documentation Consistency**

#### 7.1 Update Documentation Standards
- `needsrepair.instruction.md` - Add standard .NET compliance checks + new README policy
- `githubworkflow.instruction.md` - Update workflow standards

#### 7.2 Update Cross-References
- Update all instruction files to reference standard .NET patterns
- Remove references to ReactiveUI-specific documentation
- Update database file path references throughout all documentation
- Ensure copilot-instructions.md alignment

## 🧪 Acceptance Criteria

### ✅ ReactiveUI Removal
- [ ] No references to `ReactiveObject` in any instruction file
- [ ] No references to `ReactiveCommand<Unit, Unit>` in any instruction file
- [ ] No references to `this.RaiseAndSetIfChanged()` in any instruction file
- [ ] No references to `WhenAnyValue` patterns in any instruction file
- [ ] No references to `ObservableAsPropertyHelper` in any instruction file
- [ ] No references to `ReactiveUserControl<T>` in any instruction file
- [ ] No references to `Avalonia.ReactiveUI` namespace in any instruction file

### ✅ Standard .NET Implementation
- [ ] All ViewModel examples use `BaseViewModel` and `INotifyPropertyChanged`
- [ ] All command examples use `ICommand` (AsyncCommand/RelayCommand)
- [ ] All property examples use `SetProperty(ref _field, value)` pattern
- [ ] All error handling uses `ErrorHandling.HandleErrorAsync()`
- [ ] All service injection uses standard DI constructor patterns
- [ ] All AXAML examples use standard `UserControl` inheritance

### ✅ Database File References (NEW)
- [ ] All SQL file references point to correct Documentation/Development/Database_Files paths
- [ ] Stored procedure references use correct category-based folder structure
- [ ] Database pattern examples reference actual file locations
- [ ] No incorrect or outdated database file path references
- [ ] All database-related instruction files updated with correct paths

### ✅ HTML Documentation Infrastructure (NEW)
- [ ] Documentation/HTML_Data folder structure created with CSS, JS, and assets
- [ ] MTM-branded CSS files with purple theme (#6a0dad) implementation
- [ ] JavaScript files for navigation, collapsible sections, and file browsing
- [ ] HTML README template with proper linking to shared assets
- [ ] Responsive design working on different screen sizes

### ✅ README Policy Implementation (NEW)
- [ ] Universal README requirement removed from all instruction files
- [ ] New selective README policy documented and implemented
- [ ] HTML-based README files created for designated directories only
- [ ] Root README.md maintained in standard markdown format
- [ ] Plain English, thorough explanations in all README files
- [ ] Professional presentation with MTM branding and navigation

### ✅ Custom Prompts Update
- [ ] `CustomPrompt_Create_ReactiveUIViewModel.md` deleted or replaced
- [ ] All custom prompts generate standard .NET code
- [ ] All prompt examples use standard .NET patterns
- [ ] All personas updated to standard .NET guidance

### ✅ Documentation Quality
- [ ] All instruction files use collapsible `<details>/<summary>` formatting
- [ ] All cross-references updated to reflect new patterns
- [ ] All code examples compile and follow current build standards
- [ ] All files maintain professional documentation quality
- [ ] All database file references use correct paths

### ✅ Build Compatibility
- [ ] All generated code examples compatible with current .NET 8 build
- [ ] All patterns align with existing working examples (InventoryTabViewModel)
- [ ] All service registration patterns match ServiceCollectionExtensions.cs
- [ ] All database patterns match Helper_Database_StoredProcedure usage

## 🔄 Cross-Reference Updates

### Files That Reference Instruction Files
- [ ] `copilot-instructions.md` - Update to reflect new patterns
- [ ] All README.html files in designated directories
- [ ] Any workflow files that reference instruction patterns
- [ ] Issue templates that reference instruction standards

### Links to Update
- [ ] All internal links between instruction files
- [ ] References to deleted ReactiveUI prompt files
- [ ] Links to updated template examples
- [ ] Cross-references to current implementation examples
- [ ] **Database file path references throughout all documentation**
- [ ] **Links to new HTML-based README files**

## 📋 Implementation Checklist

### Pre-Implementation
- [ ] Backup all existing instruction files in Backups folder in root
- [ ] Document current ReactiveUI references for complete removal
- [ ] Identify all cross-reference dependencies
- [ ] Create replacement templates for deleted prompts
- [ ] **Audit all database file path references**
- [ ] **Plan HTML documentation infrastructure**

### During Implementation
- [ ] Update files in priority order (Core Instructions first)
- [ ] Test all code examples for compilation
- [ ] Validate all cross-references work correctly
- [ ] Ensure collapsible formatting maintained
- [ ] **Create Documentation/HTML_Data structure**
- [ ] **Update all database file path references**
- [ ] **Implement new README policy**

### Post-Implementation  
- [ ] Run build to verify no ReactiveUI dependencies remain
- [ ] Test instruction file generation with GitHub Copilot
- [ ] Validate all custom prompts generate correct code
- [ ] Update any workflow documentation
- [ ] **Verify all database file references are correct**
- [ ] **Test HTML documentation functionality**
- [ ] **Validate README file policy compliance**

## 🎯 Success Metrics
- [ ] **Zero ReactiveUI references** - No ReactiveUI patterns in any instruction file
- [ ] **Standard .NET compliance** - All examples use current build patterns
- [ ] **Code generation accuracy** - Custom prompts generate buildable code
- [ ] **Documentation quality** - Professional, consistent formatting maintained
- [ ] **Cross-reference integrity** - All internal links work correctly
- [ ] **Build compatibility** - Generated code compiles without modification
- [ ] **🗄️ Database reference accuracy** - All SQL file paths are correct and functional
- [ ] **📄 README policy compliance** - Selective README policy properly implemented
- [ ] **🎨 HTML documentation functionality** - Professional, branded, responsive documentation

## 📝 Notes
- This is a comprehensive documentation update, not a code change
- Focus on instruction file accuracy and consistency
- Maintain existing file organization and structure
- Preserve all non-ReactiveUI content and standards
- Ensure backward compatibility for existing standard .NET implementations
- **🗄️ Database file references must be corrected to match actual file structure**
- **📄 README policy changes remove unnecessary documentation overhead**
- **🎨 HTML documentation provides professional, branded experience**

---

**Labels:** `documentation`, `instruction-files`, `reactiveui-removal`, `standard-net`, `database-references`, `html-documentation`, `readme-policy`, `high-priority`
**Milestone:** Standard .NET Implementation Documentation v2.0
**Assignee:** Documentation Team
**Estimate:** 8 days
