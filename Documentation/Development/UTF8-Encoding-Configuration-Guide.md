# UTF-8 Encoding Configuration Guide

## **Problem Description**
Special characters and emojis (??, ??, ?, ?, ??, etc.) are being corrupted or lost when editing Markdown files in Visual Studio or other editors.

## **Root Causes**
1. **File Encoding Issues** - Files not saved as UTF-8 with BOM
2. **Visual Studio Default Settings** - Default encoding not set to UTF-8
3. **Git Configuration** - Line ending and encoding conflicts
4. **Copy/Paste Operations** - Character encoding lost during transfers
5. **Font Support** - Display fonts missing Unicode support

## **Solutions**

### **1. Visual Studio 2022 Configuration**

#### **Set Default Encoding to UTF-8**
```
Tools ? Options ? Environment ? Documents
?? "Save documents as Unicode when data cannot be saved in codepage"
?? "Automatically detect UTF-8 encoding without signature"
```

#### **Advanced Save Options**
```
File ? Advanced Save Options...
Encoding: Unicode (UTF-8 with signature) - Codepage 65001
Line Endings: Windows (CR LF)
```

#### **Force UTF-8 for Specific File Types**
```
Tools ? Options ? Text Editor ? File Extension
Extension: md
Editor: Microsoft Visual Studio Text Editor with Encoding
Default Encoding: UTF-8
```

### **2. Git Configuration**

#### **Configure Git for UTF-8**
```bash
git config --global core.quotepath false
git config --global core.autocrlf true
git config --global i18n.commitencoding utf-8
git config --global i18n.logoutputencoding utf-8
```

#### **Create/Update .gitattributes**
```gitattributes
# Force UTF-8 encoding for text files
*.md text eol=crlf
*.txt text eol=crlf
*.json text eol=crlf
*.xml text eol=crlf
*.cs text eol=crlf
*.csproj text eol=crlf
*.sln text eol=crlf

# Ensure line endings
* text=auto eol=crlf
```

### **3. EditorConfig Setup**

#### **Create/Update .editorconfig**
```ini
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

[*.md]
charset = utf-8
max_line_length = off
trim_trailing_whitespace = false

[*.{cs,csproj,sln}]
charset = utf-8
```

### **4. PowerShell UTF-8 Configuration**

#### **Set PowerShell Default Encoding**
```powershell
# Add to PowerShell profile
$PSDefaultParameterValues['Out-File:Encoding'] = 'utf8'
$PSDefaultParameterValues['Set-Content:Encoding'] = 'utf8'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
```

### **5. Font Configuration**

#### **Recommended Fonts with Unicode Support**
- **Visual Studio**: Cascadia Code, Consolas, Fira Code
- **Windows Terminal**: Cascadia Code PL, JetBrains Mono
- **General**: Segoe UI, Arial Unicode MS

#### **Visual Studio Font Settings**
```
Tools ? Options ? Environment ? Fonts and Colors
Font: Cascadia Code (or Consolas)
Size: 10-12pt
?? Show only fixed-width fonts
```

## **File Recovery and Repair**

### **Manual Character Restoration**
Replace corrupted characters with proper UTF-8 equivalents:

```markdown
# Common Replacements Needed
?? ? ?? (for compliance reports)
?? ? ?? (for automation/prompts)
?? ? ??? (for development tools)
?? ? ?? (for important notes)
? ? ? (proper arrow)
```

### **Batch File Encoding Conversion**
```powershell
# Convert all .md files to UTF-8
Get-ChildItem -Path . -Filter "*.md" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -Encoding Default
    Set-Content $_.FullName -Value $content -Encoding UTF8
}
```

### **Verification Script**
```powershell
# Check file encodings
Get-ChildItem -Path . -Filter "*.md" -Recurse | ForEach-Object {
    $bytes = [System.IO.File]::ReadAllBytes($_.FullName)
    $encoding = if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) { "UTF-8 BOM" }
                elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) { "UTF-16 LE" }
                elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFE -and $bytes[1] -eq 0xFF) { "UTF-16 BE" }
                else { "ASCII/UTF-8 No BOM" }
    Write-Host "$($_.Name): $encoding"
}
```

## **Prevention Best Practices**

### **1. Workflow Guidelines**
- Always use "Advanced Save Options" when saving files with special characters
- Verify encoding before committing files to Git
- Use consistent editing environments across team members
- Test character display after major environment changes

### **2. Copy/Paste Guidelines**
- When copying from external sources, paste into a UTF-8 editor first
- Verify special characters display correctly before saving
- Use "Paste Special" ? "Unicode Text" when available
- Avoid copying from PDF or image sources that may corrupt encoding

### **3. Regular Maintenance**
- Periodically check file encodings across the repository
- Update font configurations when upgrading development tools
- Verify UTF-8 settings after Visual Studio updates
- Document encoding issues in team knowledge base

## **Team Configuration Checklist**

### **For Each Developer**
- [ ] Visual Studio configured for UTF-8 default encoding
- [ ] Git configured with UTF-8 settings
- [ ] EditorConfig installed and recognized
- [ ] Unicode-compatible fonts installed
- [ ] PowerShell profile configured for UTF-8
- [ ] File encoding verification script available

### **For Repository**
- [ ] .gitattributes file configured for UTF-8
- [ ] .editorconfig file present and configured
- [ ] README includes encoding requirements
- [ ] CI/CD checks for encoding consistency
- [ ] Team documentation updated with guidelines

## **Troubleshooting**

### **If Characters Still Appear Corrupted**
1. Check if file was saved with correct encoding
2. Verify font supports Unicode characters
3. Clear Visual Studio component cache
4. Restart Visual Studio with clean environment
5. Check Windows regional settings

### **If Git Shows Encoding Conflicts**
1. Verify .gitattributes configuration
2. Check Git global encoding settings
3. Use `git config --list` to verify settings
4. Re-clone repository if corruption is widespread

### **Emergency Recovery**
If files are severely corrupted:
1. Restore from Git history: `git checkout HEAD~1 -- filename.md`
2. Use backup copies from file history
3. Manually recreate files with proper encoding
4. Apply this configuration guide to prevent recurrence

## **Success Verification**

After implementing these configurations:
1. Create a test file with emojis: ?????????????????
2. Save, close, and reopen the file
3. Verify all characters display correctly
4. Commit and push to Git, then pull on another machine
5. Confirm characters remain intact across environments

This configuration should eliminate the UTF-8 encoding issues and preserve all special characters and emojis in your Markdown documentation files.