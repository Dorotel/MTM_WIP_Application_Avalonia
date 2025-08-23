# Instruction Files Discovery & Inventory Report

**Discovery Date:** 2025-01-27  
**Purpose:** Complete inventory of all instruction files for consolidation planning  
**Total Files Found:** 43 instruction files

## Files Discovered

### .github/ Directory Instruction Files (11 files)
| File | Lines | Category | Status |
|------|-------|----------|--------|
| codingconventions.instruction.md | 577 | Core-Instructions | ✅ Large, needs accuracy check |
| copilot-instructions-master.md | - | Master | ⚠️ Check if duplicate |
| copilot-instructions.md | - | Master | ✅ Main file (keep in root) |
| customprompts.instruction.md | 233 | Automation-Instructions | ✅ Medium size |
| errorhandler.instruction.md | 359 | Development-Instructions | ✅ Medium size |
| githubworkflow.instruction.md | 432 | Development-Instructions | ✅ Medium size |
| naming.conventions.instruction.md | 262 | Core-Instructions | ✅ Medium size |
| needsrepair.instruction.md | 630 | Quality-Instructions | ✅ Large, critical for QA |
| personas.instruction.md | 640 | Automation-Instructions | ✅ Large, important |
| ui-generation.instruction.md | 823 | UI-Instructions | ✅ Largest file, very important |
| ui-mapping.instruction.md | 290 | UI-Instructions | ✅ Medium size |

### Development/UI_Documentation/ Instruction Files (32 files)
**Controls/MainForm/** (7 files):
- Control_AdvancedInventory.instructions.md
- Control_AdvancedRemove.instructions.md  
- Control_InventoryTab.instructions.md
- Control_QuickButtons.instructions.md
- Control_RemoveTab.instructions.md
- Control_TransferTab.instructions.md

**Controls/SettingsForm/** (16 files):
- Control_About.instructions.md
- Control_Add_ItemType.instructions.md
- Control_Add_Location.instructions.md
- Control_Add_Operation.instructions.md
- Control_Add_PartID.instructions.md
- Control_Add_User.instructions.md
- Control_Database.instructions.md
- Control_Edit_ItemType.instructions.md
- Control_Edit_Location.instructions.md
- Control_Edit_Operation.instructions.md
- Control_Edit_PartID.instructions.md
- Control_Edit_User.instructions.md
- Control_Remove_ItemType.instructions.md
- Control_Remove_Location.instructions.md
- Control_Remove_Operation.instructions.md
- Control_Remove_PartID.instructions.md
- Control_Remove_User.instructions.md
- Control_Shortcuts.instructions.md
- Control_Theme.instructions.md

**Controls/Addons/** (1 file):
- Control_ConnectionStrengthControl.instructions.md

**Controls/Shared/** (2 files):
- ColumnOrderDialog.instructions.md
- Control_ProgressBarUserControl.instructions.md

**Forms/** (5 files):
- EnhancedErrorDialog.instructions.md
- MainForm.instructions.md
- SettingsForm.instructions.md
- SplashScreenForm.instructions.md
- Transactions.instructions.md

**Root UI Documentation/** (1 file):
- Views_Structure_Standards.instruction.md

## Consolidation Planning

### Proposed .github/ Organization Structure
```
.github/
├── copilot-instructions.md                     # 🎯 MASTER FILE (stays in root)
├── Core-Instructions/                          
│   ├── codingconventions.instruction.md        # From current file
│   ├── naming-conventions.instruction.md       # From current file  
│   ├── project-structure.instruction.md        # New consolidated file
│   ├── dependency-injection.instruction.md     # Extract from coding conventions
│   └── README.md                               # Core instructions index
├── UI-Instructions/                            
│   ├── ui-generation.instruction.md            # From current file (largest)
│   ├── ui-mapping.instruction.md               # From current file
│   ├── avalonia-patterns.instruction.md        # Extract modern patterns
│   ├── mtm-design-system.instruction.md        # Extract MTM branding
│   └── README.md                               # UI instructions index
├── Development-Instructions/                   
│   ├── errorhandler.instruction.md             # From current file
│   ├── githubworkflow.instruction.md           # From current file
│   ├── database-patterns.instruction.md        # New consolidated file
│   ├── testing-standards.instruction.md        # New file needed
│   └── README.md                               # Development instructions index
├── Quality-Instructions/                       
│   ├── needsrepair.instruction.md              # From current file (critical)
│   ├── compliance-tracking.instruction.md      # New file needed
│   ├── code-review.instruction.md              # New file needed
│   └── README.md                               # Quality instructions index
├── Automation-Instructions/                    
│   ├── customprompts.instruction.md            # From current file
│   ├── personas.instruction.md                 # From current file (important)
│   ├── prompt-examples.instruction.md          # Extract examples
│   ├── workflow-automation.instruction.md      # New file needed
│   └── README.md                               # Automation instructions index
└── Archive/                                    
    ├── copilot-instructions-master.md          # Archive if duplicate
    └── README.md                               # Archive documentation
```

### UI Documentation Consolidation Strategy
- **Keep Development/UI_Documentation/ structure intact** for detailed component docs
- **Create summary instruction files** in .github/UI-Instructions/ that reference the detailed docs
- **Update cross-references** between .github/ and Development/ files
- **Maintain hierarchical reading system** where copilot-instructions.md references appropriate child files

## Next Steps
1. **Content Accuracy Verification**: Check each file against current .NET 8 Avalonia implementation
2. **Cross-Reference Mapping**: Document dependencies between files  
3. **Consolidation Execution**: Move files to organized structure
4. **Update Master File**: Modify copilot-instructions.md to reference child files appropriately