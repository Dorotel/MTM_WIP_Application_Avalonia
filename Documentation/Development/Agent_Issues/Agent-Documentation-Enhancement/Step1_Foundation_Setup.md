# **?? Step 1: Foundation & Structure Setup**

**Phase:** Foundation & Structure Setup (Safest First)  
**Priority:** CRITICAL - Must complete before all other steps  
**Links to:** [MasterPrompt.md](MasterPrompt.md) | [ContinueWork.md](ContinueWork.md)

---

## **?? Step Overview**

Establish the foundational structure and consolidate all documentation requirements into self-contained prompts. This step creates the framework for all subsequent work while minimizing risk and ensuring automatic discovery of new files.

---

## **?? Sub-Steps**

### **Step 1.1: Create New Directory Structure**

**Objective:** Establish the organized Documentation folder hierarchy without moving files yet

**New Structure:**
```
Documentation/
??? Development/
?   ??? Development Documentation/
?   ?   ??? UI_Screenshots/
?   ?   ??? UI_Documentation/
?   ?   ??? Examples/
?   ?   ??? Verification/
?   ?   ??? Database/
?   ?   ??? GitHub/
?   ??? Development Scripts/
?       ??? Verification/
?       ??? Automation/
?       ??? Build/
?       ??? Deployment/
?       ??? Utilities/
??? Production/
?   ??? Production Documentation/
?   ?   ??? Database/
?   ??? Production Scripts/
??? README.md

docs/
??? assets/
?   ??? css/
?   ??? js/
??? PlainEnglish/
?   ??? Updates/                    # NEW: PlainEnglish Updates
?   ??? FileDefinitions/           # NEW: Plain English File Definitions
?   ??? index.html
??? Technical/
?   ??? Updates/                    # NEW: Complex Updates  
?   ??? FileDefinitions/           # NEW: Complex File Definitions
?   ??? index.html
??? CoreFiles/                     # NEW: Plain English C# docs
?   ??? Services/
?   ??? ViewModels/
?   ??? Models/
?   ??? Extensions/
?   ??? index.html
??? NeedsFixing/                   # NEW: Items requiring attention
?   ??? index.html
?   ??? items/
??? Templates/
```

**Actions:**
- Create all directory structures
- Verify directory creation success
- Document structure in root README.md
- **CREATE DISCOVERY SCRIPTS**: PowerShell/bash scripts to find new files automatically

### **Step 1.2: Create Dynamic Discovery Scripts**

**Objective:** Enable automatic detection of new files, changed dependencies, and documentation needs

**New Scripts to Create:**
```
Documentation/Development/Development Scripts/Verification/
??? Discover-CoreFiles.ps1          # Find all C# core files automatically
??? Discover-Documentation.ps1       # Find all documentation files
??? Discover-Dependencies.ps1        # Find new service dependencies
??? Discover-NeedsFixing.ps1        # Find items from needsrepair.instruction.md
??? Validate-Structure.ps1           # Verify directory structure
??? Generate-FileInventory.ps1       # Create current file inventory
```

**Dynamic Discovery Features:**
- **Core File Discovery**: Automatically find Services/*.cs, ViewModels/*.cs, Models/*.cs, Extensions/*.cs
- **Instruction File Discovery**: Scan for new .instruction.md files
- **README File Discovery**: Find all README*.md files throughout repository
- **HTML File Discovery**: Locate all .html files in Documentation/ and subdirectories
- **Dependency Discovery**: Analyze using statements and constructor parameters
- **Change Detection**: Compare against previous inventories to detect additions/removals
- **NeedsFixing Discovery**: Extract items from needsrepair.instruction.md for tracking

### **Step 1.3: Create Root Documentation/README.md**

**Objective:** Establish navigation hub for entire documentation system

**Actions:**
- Create comprehensive navigation to Development vs Production
- Include quick links to key documentation areas
- Add purpose explanation for each folder
- Document the efficient organization system
- **ADD DYNAMIC DISCOVERY SECTION**: Instructions for finding new documentation needs

### **Step 1.4: Create Enhanced HTML Structure Templates**

**Objective:** Prepare template structure for the enhanced HTML categorization system

**HTML Category Templates:**
```
docs/
??? PlainEnglish/
?   ??? Updates/
?   ?   ??? index.html             # Hub for all plain English updates
?   ?   ??? recent.html            # Most recent updates
?   ?   ??? archive.html           # Historical updates
?   ??? FileDefinitions/
?   ?   ??? index.html             # All C# files in plain English
?   ?   ??? Services/              # Service explanations
?   ?   ??? ViewModels/            # ViewModel explanations
?   ?   ??? Models/                # Model explanations
?   ?   ??? Extensions/            # Extension explanations
?   ??? index.html
??? Technical/
?   ??? Updates/
?   ?   ??? index.html             # Hub for complex technical updates
?   ?   ??? recent.html            # Latest technical changes
?   ?   ??? archive.html           # Historical technical docs
?   ??? FileDefinitions/
?   ?   ??? index.html             # Technical C# file documentation
?   ?   ??? Services/              # Detailed service documentation
?   ?   ??? ViewModels/            # Technical ViewModel docs
?   ?   ??? Models/                # Technical model docs
?   ?   ??? Extensions/            # Technical extension docs
?   ??? index.html
??? NeedsFixing/
?   ??? index.html                 # Main tracking page
?   ??? high-priority.html         # Critical items
?   ??? medium-priority.html       # Important items
?   ??? low-priority.html          # Nice-to-have items
?   ??? items/                     # Individual tracking files
??? index.html                     # Master navigation
```

**Actions:**
- Create template HTML files with proper structure
- Implement navigation between categories
- Set up black/gold styling framework
- Prepare for dynamic content population

### **Step 1.5: Establish File Change Detection System**

**Objective:** Create system to track changes and automatically update documentation

**Change Detection Features:**
- **Git Integration**: Track file modifications and additions
- **Timestamp Tracking**: Monitor last modification dates
- **Content Hashing**: Detect when file content changes
- **Dependency Mapping**: Track when dependencies change
- **Documentation Gap Detection**: Identify missing documentation

**Files to Create:**
```
Documentation/Development/Development Scripts/Verification/
??? Track-Changes.ps1               # Monitor file system changes
??? Generate-ChangeReport.ps1      # Create change reports
??? Update-Documentation.ps1       # Trigger documentation updates
??? Validate-Completeness.ps1      # Check documentation coverage
```

---

## **?? Integration with Master Process**

### **Links to MasterPrompt.md:**
- **Step 1**: Foundation & Structure Setup (this step)
- **Step 2**: Core C# Documentation (requires docs/CoreFiles/ structure + dynamic discovery)
- **Step 3**: File Organization (requires Development/Production structure + change detection)
- **Step 4**: HTML Modernization (requires enhanced HTML structure + categorization)
- **Step 5**: Verification & Quality (requires new verification structure + automated validation)

### **Supports Subsequent Steps:**
- **Step 2:** Core C# Documentation (requires docs/CoreFiles/ structure + dynamic discovery)
- **Step 3:** File Organization (requires Development/Production structure + change detection)
- **Step 4:** HTML Modernization (requires enhanced HTML structure + 5-category system)
- **Step 5:** Verification & Quality (requires new verification structure + automated validation)

---

## **? Success Criteria**

**Step 1.1 Complete When:**
- ? All Documentation/Development/ directories created
- ? All Documentation/Production/ directories created
- ? All docs/ directories created including enhanced structure
- ? Enhanced HTML category structure (PlainEnglish/Technical/NeedsFixing) created
- ? Directory structure verified and functional

**Step 1.2 Complete When:**
- ? All 6 discovery scripts created and functional
- ? Dynamic file discovery working
- ? Change detection operational
- ? File inventory generation successful
- ? NeedsFixing discovery operational

**Step 1.3 Complete When:**
- ? Root Documentation/README.md created
- ? Navigation hub functional
- ? Clear purpose documentation for each folder
- ? Dynamic discovery instructions documented

**Step 1.4 Complete When:**
- ? All HTML category templates created
- ? Navigation structure between categories functional
- ? Black/gold styling framework prepared
- ? Content population system ready

**Step 1.5 Complete When:**
- ? Change detection system operational
- ? Git integration working
- ? Documentation gap detection functional
- ? Automated update triggers working

---

## **?? Emergency Continuation**

**If this step is interrupted, use:**

```
EXECUTE STEP 1 CONTINUATION:

Act as Documentation Web Publisher Copilot and Quality Assurance Auditor Copilot.

1. ASSESS current Step 1 completion state:
   ? Check directory structure creation progress
   ? Verify discovery scripts implementation status
   ? Check root README.md creation status
   ? Verify HTML template structure creation
   ? Check change detection system status

2. RESUME from incomplete sub-step:
   - If 1.1 incomplete: Finish directory structure creation
   - If 1.2 incomplete: Create missing discovery scripts
   - If 1.3 incomplete: Create root Documentation/README.md
   - If 1.4 incomplete: Create missing HTML templates
   - If 1.5 incomplete: Complete change detection system

3. VALIDATE completion before proceeding to Step 2

CRITICAL: Step 1 must be 100% complete before any file movements or C# documentation can begin.

DYNAMIC DISCOVERY: Before proceeding, run discovery scripts to identify:
- New C# core files requiring documentation
- Changed instruction files needing updates
- New README files requiring verification
- Additional HTML files needing migration
- Items from needsrepair.instruction.md requiring attention
- New service dependencies requiring documentation

ENHANCED HTML STRUCTURE: Ensure all 5 categories are properly set up:
1. PlainEnglish/Updates/ - For non-technical updates
2. Technical/Updates/ - For developer-focused updates  
3. PlainEnglish/FileDefinitions/ - Plain English C# file explanations
4. Technical/FileDefinitions/ - Technical C# file documentation
5. NeedsFixing/ - Items requiring attention from needsrepair.instruction.md
```

---

## **?? Technical Requirements**

- **MTM Compliance:** All work follows .NET 8 Avalonia standards
- **Quality Standards:** Documentation accuracy and completeness
- **Structure Standards:** Efficient Development/Production separation with enhanced HTML categorization
- **Safety Standards:** No destructive operations, backup-friendly approach
- **Dynamic Discovery:** Automatic detection of new files and changes
- **Change Resilience:** System adapts to codebase evolution automatically
- **Category Organization:** 5-category HTML system for comprehensive documentation

**Discovery Script Templates:**

**Discover-CoreFiles.ps1:**
```powershell
# Discover all C# core files for documentation
$CoreFiles = @{
    "Services" = Get-ChildItem -Path "Services" -Filter "*.cs" -Recurse | Where-Object { $_.Name -notlike "*Test*" }
    "ViewModels" = Get-ChildItem -Path "ViewModels" -Filter "*.cs" -Recurse | Where-Object { $_.Name -notlike "*Test*" }
    "Models" = Get-ChildItem -Path "Models" -Filter "*.cs" -Recurse | Where-Object { $_.Name -notlike "*Test*" }
    "Extensions" = Get-ChildItem -Path "Extensions" -Filter "*.cs" -Recurse | Where-Object { $_.Name -notlike "*Test*" }
}

# Generate documentation requirements for each discovered file
foreach ($Category in $CoreFiles.Keys) {
    Write-Host "=== $Category Files Requiring Documentation ==="
    foreach ($File in $CoreFiles[$Category]) {
        Write-Host "- $($File.FullName)"
        
        # Check PlainEnglish documentation
        $PlainDocPath = "docs/PlainEnglish/FileDefinitions/$Category/$($File.BaseName).html"
        if (-not (Test-Path $PlainDocPath)) {
            Write-Host "  ? MISSING: Plain English documentation needed at $PlainDocPath"
        } else {
            Write-Host "  ? EXISTS: $PlainDocPath"
        }
        
        # Check Technical documentation
        $TechDocPath = "docs/Technical/FileDefinitions/$Category/$($File.BaseName).html"
        if (-not (Test-Path $TechDocPath)) {
            Write-Host "  ? MISSING: Technical documentation needed at $TechDocPath"
        } else {
            Write-Host "  ? EXISTS: $TechDocPath"
        }
    }
}
```

**Discover-NeedsFixing.ps1:**
```powershell
# Extract items from needsrepair.instruction.md for tracking
$NeedsRepairFile = ".github/Quality-Instructions/needsrepair.instruction.md"

if (Test-Path $NeedsRepairFile) {
    $Content = Get-Content $NeedsRepairFile -Raw
    
    # Extract different priority levels
    $HighPriorityItems = @()
    $MediumPriorityItems = @()
    $LowPriorityItems = @()
    
    # Parse content for priority indicators
    $Lines = $Content -split "`n"
    
    foreach ($Line in $Lines) {
        if ($Line -match "?.*CRITICAL|?.*HIGH|??") {
            $HighPriorityItems += $Line.Trim()
        }
        elseif ($Line -match "??.*MEDIUM|??") {
            $MediumPriorityItems += $Line.Trim()
        }
        elseif ($Line -match "??.*LOW|??") {
            $LowPriorityItems += $Line.Trim()
        }
    }
    
    Write-Host "=== NeedsFixing Items Discovered ==="
    Write-Host "High Priority: $($HighPriorityItems.Count) items"
    Write-Host "Medium Priority: $($MediumPriorityItems.Count) items"
    Write-Host "Low Priority: $($LowPriorityItems.Count) items"
    
    # Generate HTML tracking files
    $NeedsFixingPath = "docs/NeedsFixing"
    if (-not (Test-Path $NeedsFixingPath)) {
        New-Item -Path $NeedsFixingPath -ItemType Directory -Force
    }
    
    # Create priority-specific tracking pages
    # (HTML generation logic would go here)
}